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
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Transaction;

    public class VoyageFailedExternalization2Transaction
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Transaction> transactions;

        public VoyageFailedExternalization2Transaction(
            MongoClient mongoClient,
            ILogger<TransactionTracker> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            transactions = database.GetCollection<Transaction>("Transactions");
        }

        [FunctionName(nameof(VoyageFailedExternalization2Transaction))]
        public async void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation($"Process_VoyageFailedExternalization2Transaction triggered....EventGridEvent" +
                            $"\n\tId:{ eventGridEvent.Id }" +
                            $"\n\tTopic:{ eventGridEvent.Topic }" +
                            $"\n\tSubject:{ eventGridEvent.Subject }" +
                            $"\n\tType:{ eventGridEvent.EventType }" +
                            $"\n\tData:{ eventGridEvent.Data }");

            try
            {
                VoyageFailedEvent @event = JsonConvert.DeserializeObject<VoyageFailedEvent>(eventGridEvent.Data.ToString());
                
                var transaction = await transactions.Find(t => t.Id == @event.TransactionId).FirstOrDefaultAsync();

                if (transaction == null)
                {
                    logger.LogInformation($"Process_VoyageFailedExternalization2Transaction: That item does not exist: {@event.TransactionId}");
                    throw new Exception($"Couldn't find voyage with id: {@event.TransactionId}");
                }

                transaction.Error = @event.FailReason;
                transaction.Status = Status.Failed;

                var replacedItem = await transactions.ReplaceOneAsync(t => t.Id == transaction.Id, transaction);
            }
            catch (Exception e)
            {
                var error = $"VoyageFailedExternalization2Transaction failed: {e.Message}";
                logger.LogError(error);
            }
        }
    }
}
