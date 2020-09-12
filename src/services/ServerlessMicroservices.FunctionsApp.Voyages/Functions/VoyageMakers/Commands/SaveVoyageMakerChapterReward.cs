namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Command
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class SaveVoyageMakerReward
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "starNeededToUnlock")]
        public int StarNeededToUnlock { get; set; }
    }

}
