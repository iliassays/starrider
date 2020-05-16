namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Command
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class SaveVoyageMakerCommand
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; } = "";

        [JsonProperty(PropertyName = "organizationPhone")]
        public string OrganizationPhone { get; set; } = "";

        [JsonProperty(PropertyName = "transactionRuleTypeId")]
        public int TransactionRuleTypeId { get; set; } = 1;

        [JsonProperty(PropertyName = "transactionRuleBaseAmount")]
        public decimal TransactionRuleBaseAmount { get; set; } = 50;

        [JsonProperty(PropertyName = "transactionRuleNumberOfStartAwarded")]
        public int TransactionRuleNumberOfStartAwarded { get; set; } = 1;

        [JsonProperty(PropertyName = "transactionRuleNumberOfBuyNeeded")]
        public int TransactionRuleNumberOfBuyNeeded { get; set; } = 1;

        [JsonProperty(PropertyName = "transactionRuleNumberOfFreeItem")]
        public int TransactionRuleNumberOfFreeItem { get; set; } = 1;

    }
}
