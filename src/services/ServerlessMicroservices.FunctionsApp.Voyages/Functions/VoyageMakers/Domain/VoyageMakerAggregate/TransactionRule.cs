namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public abstract class TransactionRule
    {
        
    }

    public class PointBasedRule : TransactionRule
    {
        [JsonProperty(PropertyName = "baseAmount")]
        public decimal BaseAmount { get; private set; } = 50;

        [JsonProperty(PropertyName = "numberOfStartAwarded")]
        public int NumberOfStartToBeAwarded { get; private set; } = 1;

        public PointBasedRule(decimal baseAmount, int numberOfStartAwarded)
        {
            BaseAmount = baseAmount;
            NumberOfStartToBeAwarded = numberOfStartAwarded;
        }
    }

    public class BuyNGetNBasedRule : TransactionRule
    {
        [JsonProperty(PropertyName = "numberOfBuyNeeded")]
        public int NumberOfBuyNeeded { get; set; } = 1;

        [JsonProperty(PropertyName = "numberOfFreeItem")]
        public int NumberOfFreeItem { get; set; } = 1;

        public BuyNGetNBasedRule(int numberOfBuyNeeded, int numberOfFreeItem)
        {

        }
    }
}
