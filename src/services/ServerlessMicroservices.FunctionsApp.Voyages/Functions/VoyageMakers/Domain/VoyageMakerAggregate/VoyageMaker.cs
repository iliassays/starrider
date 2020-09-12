namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{ 
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class VoyageMaker
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; } = "";

        [JsonProperty(PropertyName = "organizationPhone")]
        public string OrganizationPhone { get; set; } = "";

        [JsonProperty(PropertyName = "headline")]
        public string Headline { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "transactionRuleType")]
        public TransactionRuleTypes TransactionRuleType { get; private set; }

        [JsonProperty(PropertyName = "transactionRule")]
        public TransactionRule TransactionRule { get; private set; }

        public VoyageMaker(string organizationName, string organizationPhone, string headLine, string title)
        {
            this.OrganizationName = organizationName;
            this.OrganizationPhone = organizationPhone;
            this.TransactionRuleType = TransactionRuleTypes.PointBasedRule;
        }

        public void SetPointTransactionRule(decimal baseAmount, int numberOfStartAwarded)
        {
            this.TransactionRule = new PointBasedRule(baseAmount, numberOfStartAwarded);
            this.TransactionRuleType = TransactionRuleTypes.PointBasedRule;
        }

        public void SetBuyNGetNBasedRule(int numberOfBuyNeeded, int numberOfFreeItem)
        {
            this.TransactionRule = new BuyNGetNBasedRule(numberOfBuyNeeded, numberOfFreeItem);
            this.TransactionRuleType = TransactionRuleTypes.BuyNGetNBasedRule;
        }

        public int CalculateStarFromTransaction(decimal amount)
        {
            var pointBasedRule = this.TransactionRule as PointBasedRule;
            return Convert.ToInt32(amount / pointBasedRule.BaseAmount) * pointBasedRule.NumberOfStartToBeAwarded;
        }
    }
}
