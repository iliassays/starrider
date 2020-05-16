

namespace ServerlessMicroservices.FunctionsApp.Voyages.Function
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.Voyages.Core;
    using ServerlessMicroservices.Voyages.Infrastructure;
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Transaction;

    public class TransactionTracker
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Transaction> transactions;

        public TransactionTracker(
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

        [FunctionName(nameof(TransactionTracker))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tt/")] HttpRequest req)
        {
            logger.LogInformation("TransactionTracker triggered...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var transaction = JsonConvert.DeserializeObject<Transaction>(requestBody);

                transaction.Status = Status.New;

                await transactions.InsertOneAsync(transaction);
                await RaiseTransactionCreatedEvent(transaction, Constants.EVG_EVENT_TRANSACTION_CREATED);

                return new OkObjectResult(transaction);
            }
            catch(Exception e)
            {
                var error = $"TransactionTracker failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }

        private static async Task RaiseTransactionCreatedEvent(Transaction transaction, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<TransactionEvent>(null,
                new TransactionEvent() {
                    TransactionId = transaction.Id,
                    Amount = transaction.Amount,
                    BranchId = transaction.BranchId,
                    OrganizationId = transaction.OrganizationId,
                    CreatedAt = transaction.CreatedAt,
                    UpdatedAt = transaction.UpdatedAt,
                    CreatedByUser = transaction.createdByUser,
                    Type = transaction.Type,
                    Email = transaction.Email,
                    MobileNumber = transaction.MobileNumber
                },
                Constants.EVG_EVENT_TRANSACTION_CREATED,
                subject,
                Settings.GetTransactionExternalizationsEventGridTopicUrl(),
                Settings.GetTransactionExternalizationsEventGridTopicApiKey());
        }
    }
}
