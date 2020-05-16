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
    using System.Linq;

    public class EVGH_TransactionCreatedExternalization2Voyages
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Voyage> voyages;

        public EVGH_TransactionCreatedExternalization2Voyages(
            MongoClient mongoClient,
            ILogger<TransactionTracker> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            voyages = database.GetCollection<Voyage>("Voyages");
        }

        [FunctionName(nameof(EVGH_TransactionCreatedExternalization2Voyages))]
        public async void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation($"Process_TransactionExternalization2Voyages triggered....EventGridEvent" +
                            $"\n\tId:{ eventGridEvent.Id }" +
                            $"\n\tTopic:{ eventGridEvent.Topic }" +
                            $"\n\tSubject:{ eventGridEvent.Subject }" +
                            $"\n\tType:{ eventGridEvent.EventType }" +
                            $"\n\tData:{ eventGridEvent.Data }");

            TransactionEvent transactionEvent = null;

            try
            {
                transactionEvent = JsonConvert.DeserializeObject<TransactionEvent>(eventGridEvent.Data.ToString());

                if (transactionEvent == null)
                    throw new Exception("transaction is null!");

                log.LogInformation($"Process_TransactionExternalization2Voyages transaction {transactionEvent.TransactionId}");

               
                if (transactionEvent == null || string.IsNullOrEmpty(transactionEvent.MobileNumber))
                    throw new Exception("A voyager with a valid mobile number must be attached to the voyage!!");

                var existingVoyage = await voyages.Find(v => v.Pilot.OrganizerId == transactionEvent.OrganizationId &&
                   v.Voyager.MobileNumber == transactionEvent.MobileNumber).FirstOrDefaultAsync();

                ( Voyage voyage, int starCollected) = await ConvertTransactionToVoyage(transactionEvent, existingVoyage);
                
                if (existingVoyage == null)
                {
                    voyages.InsertOne(voyage);
                    await RaiseVoyagesCreatedEvent(transactionEvent, voyage, starCollected, Constants.EVG_EVENT_VOYAGES_CREATED);
                }
                else
                {
                    var replacedItem = voyages.ReplaceOne(v => v.Id == voyage.Id, voyage);
                    await RaiseVoyagesUpdatedEvent(transactionEvent, voyage, starCollected, Constants.EVG_EVENT_VOYAGES_UPDATED);
                }
            }
            catch (Exception e)
            {
                var error = $"Voyage Creation failed: {e.Message}";
                logger.LogError(error);

                await RaiseVoyagesFailedEvent(transactionEvent, error, Constants.EVG_EVENT_TRANSACTION_CREATED);
            }
        }

        private async Task<(Voyage, int)> ConvertTransactionToVoyage(TransactionEvent transaction, Voyage voyage)
        {
            var tripMakerInfo = await HttpHelper.Get<VoyageMaker>(null, Settings.GetTripMakerApi_GetTripMakerUrl(), new Dictionary<string, string>());

            if (tripMakerInfo == null)
            {
                throw new Exception("No tripmaker found");
            }

            voyage.Pilot = new Pilot
            {
                OrganizerId = tripMakerInfo.Id,
                OrganizationName = tripMakerInfo.OrganizationName
            };

            //if chapter is not set, build Voyages chapter from trip maker chapter
            if(voyage.VoyageChapters == null )
            {
                voyage.VoyageChapters = new List<VoyageChapter>();

                tripMakerInfo.Chapters.ForEach(c =>
                {

                    voyage.VoyageChapters.Add(
                        new VoyageChapter()
                        {
                            ChapterId = c.Id,
                            Title = c.Title,
                            Description = c.Description,
                            StarNeededToComplete = c.StarNeededToComplete,
                            LockedRewards = c.ChapterRewards
                                .Select(cr => new VoyageChapterLockedReward
                                {
                                    RewardId = cr.RewardId,
                                    Description = cr.Description,
                                    Title = cr.Title,
                                    StarNeededToUnlock = cr.StarNeededToUnlock
                                }).ToList()
                        }
                    );
                });
            }

            var starCollected = await CalculateStarForTransaction(transaction);

            voyage.ApplyNewlyAddedStar(starCollected);

            return (voyage, starCollected);
        }

        private async Task<int> CalculateStarForTransaction(TransactionEvent transaction)
        {
            var starCount = await HttpHelper.Get<int>(null, Settings.GetTripMakerApi_CalculateStarForTransactionUrl(), new Dictionary<string, string>());

            return starCount;
        }

        private static async Task RaiseVoyagesCreatedEvent(TransactionEvent transaction, Voyage voyage, int starCollected,  string subject)
        {
            await EventPublisher.TriggerEventGridTopic<VoyageCreatedEvent>(null,
                new VoyageCreatedEvent()
                {
                    TransactionId = transaction.TransactionId,
                    OrganizationName = voyage.Pilot.OrganizationName,
                    StarCollected = starCollected,
                    VoyageId = voyage.Id,
                    MobileNumber = voyage.Voyager.MobileNumber,
                    Email = voyage.Voyager.Email,
                    CreatedByUser = transaction.CreatedByUser,
                    OrganizationId = transaction.OrganizationId,
                    BranchId = transaction.BranchId

                },
                Constants.EVG_EVENT_VOYAGES_CREATED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseVoyagesUpdatedEvent(TransactionEvent transaction, Voyage voyage, int starCollected, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<VoyageUpdatedEvent>(null,
                new VoyageUpdatedEvent()
                {
                    TransactionId = transaction.TransactionId,
                    OrganizationName = voyage.Pilot.OrganizationName,
                    StarCollected = starCollected,
                    VoyageId = voyage.Id,
                    MobileNumber = voyage.Voyager.MobileNumber,
                    Email = voyage.Voyager.Email,
                    CreatedByUser = transaction.CreatedByUser,
                    OrganizationId = transaction.OrganizationId,
                    BranchId = transaction.BranchId
                },
                Constants.EVG_EVENT_VOYAGES_UPDATED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseVoyagesFailedEvent(TransactionEvent transaction, string failedReason, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<VoyageFailedEvent>(null,
                new VoyageFailedEvent()
                {
                    TransactionId = transaction.TransactionId,
                    FailReason = failedReason
                },
                Constants.EVG_EVENT_VOYAGES_FAILED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }
    }
}
