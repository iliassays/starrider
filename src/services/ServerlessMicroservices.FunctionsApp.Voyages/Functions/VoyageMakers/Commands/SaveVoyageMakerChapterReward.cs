namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Command
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class SaveVoyageMakerChapterReward
    {
        [JsonProperty(PropertyName = "rewardId")]
        public string RewardId { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "starNeededToUnlock")]
        public int StarNeededToUnlock { get; set; }
    }

}
