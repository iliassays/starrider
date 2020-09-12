namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Reward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "starNeededToAvail")]
        public string StarNeededToAvail { get; set; } = "";

        [JsonProperty(PropertyName = "voyageMakerId")]
        public string VoyageMakerId { get; set; } = "";

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime? CreatedAt { get; set; } = null;

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
