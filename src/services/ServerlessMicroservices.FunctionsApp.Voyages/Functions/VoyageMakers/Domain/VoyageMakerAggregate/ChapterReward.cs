namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class ChapterReward
    {
        [JsonProperty(PropertyName = "id")]
        public string RewardId { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "starNeededToUnlock")]
        public int StarNeededToUnlock { get; set; }
    }
}
