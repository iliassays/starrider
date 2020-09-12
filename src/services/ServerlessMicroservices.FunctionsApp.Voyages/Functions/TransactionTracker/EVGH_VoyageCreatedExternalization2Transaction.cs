// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=EVGH_TransactionExternalization2Voyages

namespace ServerlessMicroservices.FunctionsApp.Voyages.Function
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.Voyages.Core;
    using Microsoft.Azure.WebJobs.Extensions.EventGrid;
    using Microsoft.Azure.EventGrid.Models;
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Voyage;
    using ServerlessMicroservices.Voyages.Infrastructure;
    using System.Collections.Generic;

    public class EVGH_TransactionExternalization2Voyages
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Voyage> voyages;

        public EVGH_TransactionExternalization2Voyages(
            MongoClient mongoClient,
            ILogger<TransactionTracker> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Settings.DATABASE_NAME]);
            voyages = database.GetCollection<Voyage>(config[Settings.COLLECTION_NAME]);
        }

        [FunctionName(nameof(EVGH_TransactionExternalization2Voyages))]
        public async Task<IActionResult> Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation($"Process_TransactionExternalization2Voyages triggered....EventGridEvent" +
                            $"\n\tId:{ eventGridEvent.Id }" +
                            $"\n\tTopic:{ eventGridEvent.Topic }" +
                            $"\n\tSubject:{ eventGridEvent.Subject }" +
                            $"\n\tType:{ eventGridEvent.EventType }" +
                            $"\n\tData:{ eventGridEvent.Data }");

            try
            {
                Transaction transaction = JsonConvert.DeserializeObject<Transaction>(eventGridEvent.Data.ToString());

                if (transaction == null)
                    throw new Exception("transaction is null!");

                log.LogInformation($"Process_TransactionExternalization2Voyages customer {transaction.MobileNumber}");

               
                if (transaction == null || string.IsNullOrEmpty(transaction.MobileNumber))
                    throw new Exception("A voyager with a valid mobile number must be attached to the voyage!!");

                var existingVoyage = await voyages.Find(v => v.Pilot.OrganizerId == transaction.OrganizationId &&
                   v.Voyager.MobileNumber == transaction.MobileNumber).FirstOrDefaultAsync();

                var voyage = await ConvertTransactionToVoyages(transaction ,existingVoyage);
                // get the RewardRules
                // 
                if (existingVoyage == null)
                {
                    voyages.InsertOne(voyage);
                    await RaiseVoyagesCreatedEvent(transaction, voyage, Settings.EVG_EVENT_VOYAGES_CREATED);
                }
                else
                {
                    var replacedItem = voyages.ReplaceOne(v => v.Id == voyage.Id, voyage);
                    await RaiseVoyagesUpdatedEvent(transaction, voyage, Settings.EVG_EVENT_VOYAGES_UPDATED);
                }

                return new OkResult();
            }
            catch (Exception e)
            {
                var error = $"TransactionTracker failed: {e.Message}";
                logger.LogError(error);
                await RaiseVoyagesFailedEvent(transaction, Settings.EVG_EVENT_TRANSACTION_CREATED);

                return new BadRequestObjectResult(error);
            }
        }

        private async Task<Voyage> ConvertTransactionToVoyages(Transaction transaction, Voyage voyage)
        {
            var tripMakerInfo = await HttpHelper.Get<VoyageMaker>(null, Settings.GetTripMakerApiUrl(), new Dictionary<string, string>());
            //List<PassengerItem> passengers = await Utilities.Get<List<PassengerItem>>(null, $"{passengersUrl}/api/passengers", new Dictionary<string, string>());

            if (tripMakerInfo == null)
            {
                throw new Exception("No trip maker found");
            }

            voyage.Pilot = new Pilot
            {
                OrganizerId = tripMakerInfo.Id,
                OrganizationName = tripMakerInfo.OrganizationName
            };

            //if chapter is not set, build Voyages chaper from trip maker chapter
            if(voyage.VoyageChapters == null )
            {
                voyage.VoyageChapters = new List<VoyageChapter>();

                tripMakerInfo.Chapters.ForEach(c =>
                {
                    voyage.VoyageChapters.Add(
                        new VoyageChapter()
                        {
                            ChapterDefination = c
                        }
                    );
                });
            }

            voyage.ApplyNewlyAddedStar(await CalculateStarForTransaction(transaction));

            return voyage;
        }

        private async Task<int> CalculateStarForTransaction(Transaction transaction)
        {
            var starCount = await HttpHelper.Get<int>(null, Settings.CalculateStarForTransactionApiUrl(), new Dictionary<string, string>());

            return starCount;
        }

        private static async Task RaiseVoyagesCreatedEvent(Transaction transaction, Voyage voyage,  string subject)
        {
            await EventPublisher.TriggerEventGridTopic<Transaction>(null, transaction,
                Settings.EVG_EVENT_VOYAGES_CREATED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseVoyagesUpdatedEvent(Transaction transaction, Voyage voyage, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<Transaction>(null, transaction,
                Settings.EVG_EVENT_VOYAGES_UPDATED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseVoyagesFailedEvent(Transaction transaction, Voyage voyage, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<Transaction>(null, transaction,
                Settings.EVG_EVENT_VOYAGES_FAILED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }
    }
}
