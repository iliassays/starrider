

namespace ServerlessMicroservices.FunctionsApp.Voyages.Function
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.Voyages.Core;
    using System.IO;
    using Newtonsoft.Json;
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Voyage;

    public class CalculateStarForTransactionData
    {
        [JsonProperty(PropertyName = "transactionAmount")]
        public decimal TransactionAmount { get; set; }
    }

    public class CalculateStarForTransaction
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<VoyageMaker> voyageMakers;

        public CalculateStarForTransaction(
            MongoClient mongoClient,
            ILogger<GetVoyage> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            voyageMakers = database.GetCollection<VoyageMaker>("VoyageMakers");
        }

        [FunctionName(nameof(CalculateStarForTransaction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "VoyageMaker/{id}/calculate")] HttpRequest req,
            string id)
        {
            logger.LogInformation("CalculateStarForTransaction triggered...");

            try
            {
                //await Utilities.ValidateToken(req);
                var result = await voyageMakers.Find(trip => trip.Id == id).FirstOrDefaultAsync();

                if(result == null)
                {
                    logger.LogInformation("That item does not exist!");
                    throw new Exception($"Couldn't find voyage maker with id: {id}");
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var calculateStarForTransactionData = JsonConvert.DeserializeObject<CalculateStarForTransactionData>(requestBody);

                return new OkObjectResult(result.CalculateStarFromTransaction(calculateStarForTransactionData.TransactionAmount));
            }
            catch(Exception e)
            {
                var error = $"GetVoyageMaker failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
