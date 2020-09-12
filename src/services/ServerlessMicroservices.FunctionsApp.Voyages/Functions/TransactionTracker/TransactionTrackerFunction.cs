

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
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain;
    using ServerlessMicroservices.Voyages.Core;

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

            var database = this.mongoClient.GetDatabase(config[Settings.DATABASE_NAME]);
            transactions = database.GetCollection<Transaction>("Transaction");
        }

        [FunctionName(nameof(TransactionTracker))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tt/")] HttpRequest req)
        {
            logger.LogInformation("TransactionTracker triggered...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<Transaction>(requestBody);

                //Validate here
                var transaction = new Transaction
                {
                    CustomerNumber = input.CustomerNumber,
                    MobileNumber = input.MobileNumber,
                    Email = input.Email,
                    OrganizationId = input.OrganizationId,
                    Amount = input.Amount
                };

                try
                {
                    transactions.InsertOne(transaction);
                    return new OkObjectResult(transactions);
                }
                catch (Exception ex)
                {
                    //Log the data so that we can later create it in case of error
                    logger.LogError($"Exception thrown: {ex.Message}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            catch(Exception e)
            {
                var error = $"TransactionTracker failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
