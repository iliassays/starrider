

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
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Voyage;

    public class GetVoyage
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Voyage> voyages;

        public GetVoyage(
            MongoClient mongoClient,
            ILogger<GetVoyage> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            voyages = database.GetCollection<Voyage>(config[Constants.COLLECTION_NAME]);
        }

        [FunctionName(nameof(GetVoyage))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Voyages/{id}")] HttpRequest req,
            string id)
        {
            logger.LogInformation("GetVoyage triggered...");

            try
            {
                //await Utilities.ValidateToken(req);
                var result = await voyages.Find(trip => trip.Id == id).FirstOrDefaultAsync();

                if(result == null)
                {
                    logger.LogInformation("That item does not exist!");
                    throw new Exception($"Couldn't find voyage with id: {id}");
                }
                
                return new OkObjectResult(result);
            }
            catch(Exception e)
            {
                var error = $"GetVoyage failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
