

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

    public class SaveReward
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Reward> rewards;

        public SaveReward(
            MongoClient mongoClient,
            ILogger<SaveReward> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            rewards = database.GetCollection<Reward>("Rewards");
        }

        [FunctionName(nameof(SaveReward))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "VoyageMakers/Rewards")] HttpRequest req)
        {
            logger.LogInformation("SaveReward triggered...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var reward = JsonConvert.DeserializeObject<Reward>(requestBody);

                if (reward == null || string.IsNullOrEmpty(reward.Title) || string.IsNullOrEmpty(reward.Description))
                    throw new Exception("A reward with a valid data must be attached!!");

                var existingReward = await rewards.Find(v => v.Id == reward.Id).FirstOrDefaultAsync();

                if (existingReward == null)
                {
                    rewards.InsertOne(reward);
                }
                else
                {
                    var replacedItem = rewards.ReplaceOne(v => v.Id == reward.Id, reward);
                }

                return new OkObjectResult(reward);
            }
            catch(Exception e)
            {
                var error = $"SaveReward failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
