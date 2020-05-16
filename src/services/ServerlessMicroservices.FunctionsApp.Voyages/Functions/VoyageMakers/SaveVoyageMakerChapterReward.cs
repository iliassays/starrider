

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
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain;
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Command;

    public class SaveVoyageMakerChapter
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<VoyageMaker> voyageMakers;

        public SaveVoyageMakerChapter(
            MongoClient mongoClient,
            ILogger<SaveVoyageMakerChapter> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            voyageMakers = database.GetCollection<VoyageMaker>("VoyageMakers");
        }

        [FunctionName(nameof(SaveVoyageMakerChapter))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "voyagemakers/{voyageMakerId}/chapter")] HttpRequest req,
            string voyageMakerId)
        {
            logger.LogInformation("SaveVoyageMakerChapter triggered...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var command = JsonConvert.DeserializeObject<SaveVoyageMakerCommand>(requestBody);

                if (command == null || string.IsNullOrEmpty(command.OrganizationName) || string.IsNullOrEmpty(command.OrganizationPhone))
                    throw new Exception("A voyage maker with a valid data must be attached!!");

                var existingVoyageMaker = await voyageMakers.Find(v => v.Id == command.Id).FirstOrDefaultAsync();

                var voyageMaker = existingVoyageMaker != null ? existingVoyageMaker : new VoyageMaker(command.OrganizationName, command.OrganizationPhone);

                TransactionRuleTypes transactionRuleType = TransactionRuleTypes.From(command.TransactionRuleTypeId);

                if(transactionRuleType == TransactionRuleTypes.PointBasedRule)
                {
                    voyageMaker.SetPointTransactionRule(command.TransactionRuleBaseAmount, command.TransactionRuleNumberOfStartAwarded);
                }
                else if (transactionRuleType == TransactionRuleTypes.BuyNGetNBasedRule)
                {
                    voyageMaker.SetBuyNGetNBasedRule(command.TransactionRuleNumberOfBuyNeeded, command.TransactionRuleNumberOfFreeItem);
                }

                if (existingVoyageMaker == null)
                {
                    voyageMakers.InsertOne(voyageMaker);
                }
                else
                {
                    voyageMaker.OrganizationName = command.OrganizationName;
                    voyageMaker.OrganizationPhone = command.OrganizationPhone;

                    var replacedItem = voyageMakers.ReplaceOne(v => v.Id == voyageMaker.Id, voyageMaker);
                }

                return new OkObjectResult(voyageMaker);
            }
            catch(Exception e)
            {
                var error = $"CreateVoyageMaker failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
