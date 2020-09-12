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
    using ServerlessMicroservices.Voyages.Infrastructure;
    using System.Collections.Generic;

    public class VoyageCreatedExternalization2Transaction
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Transaction> transactions;

        public VoyageCreatedExternalization2Transaction(
            MongoClient mongoClient,
            ILogger<TransactionTracker> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Settings.DATABASE_NAME]);
            transactions = database.GetCollection<Transaction>("Transactions");
        }

        [FunctionName(nameof(VoyageCreatedExternalization2Transaction))]
        public async Task<IActionResult> Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation($"Process_VoyageCreatedExternalization2Transaction triggered....EventGridEvent" +
                            $"\n\tId:{ eventGridEvent.Id }" +
                            $"\n\tTopic:{ eventGridEvent.Topic }" +
                            $"\n\tSubject:{ eventGridEvent.Subject }" +
                            $"\n\tType:{ eventGridEvent.EventType }" +
                            $"\n\tData:{ eventGridEvent.Data }");

            try
            {
                VoyageCreatedEvent @event = JsonConvert.DeserializeObject<VoyageCreatedEvent>(eventGridEvent.Data.ToString());

                
                var transaction = await transactions.Find(t => t.Id == @event.Id).FirstOrDefaultAsync();

                if (transaction == null)
                {
                    logger.LogInformation($"Process_VoyageCreatedExternalization2Transaction: That item does not exist: {@event.TransactionId}");
                    throw new Exception($"Couldn't find voyage with id: {@event.TransactionId}");
                }

                transaction.OrganizationName = @event.OrganizationName;
                transaction.StarCollected = @event.StarCollected;
                transaction.VoyageId = @event.VoyageId;

                var replacedItem = await transactions.ReplaceOneAsync(t => t.Id == transaction.Id, transaction);

                return new OkResult();
            }
            catch (Exception e)
            {
                var error = $"VoyageCreatedExternalization2Transaction failed: {e.Message}";
                logger.LogError(error);
                
                return new BadRequestObjectResult(error);
            }
        }
    }
}
