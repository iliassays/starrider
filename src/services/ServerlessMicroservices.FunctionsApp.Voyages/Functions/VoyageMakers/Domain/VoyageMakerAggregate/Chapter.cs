namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Chapter
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } = "Level 1";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "Level 1";

        [JsonProperty(PropertyName = "starNeededToComplete")]
        public double StarNeededToComplete { get; set; } = 100;

        [JsonProperty(PropertyName = "rewards")]
        public List<ChapterReward> Rewards { get; set; } = new List<ChapterReward>();
    }
}
