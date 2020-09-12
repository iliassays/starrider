

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
    using System.Linq;
    using ServerlessMicroservices.Voyages.Core;
    using ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Voyage;

    public class GetVoyaageMaker
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<VoyageMaker> voyageMakers;

        public GetVoyaageMaker(
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

        [FunctionName(nameof(GetVoyaageMaker))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "VoyageMaker/{id}")] HttpRequest req,
            string id)
        {
            logger.LogInformation("GetVoyageMaker triggered...");

            try
            {
                //await Utilities.ValidateToken(req);
                var result = await voyageMakers.Find(trip => trip.Id == id).FirstOrDefaultAsync();

                if(result == null)
                {
                    logger.LogInformation("That item does not exist!");
                    throw new Exception($"Couldn't find voyage maker with id: {id}");
                }
                
                return new OkObjectResult(result);
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
