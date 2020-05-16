namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Transaction
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; } = "";

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } = "";

        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; } = "";

        [JsonProperty(PropertyName = "branchId")]
        public string BranchId { get; set; } = "";

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = "createdByUser")]
        public string createdByUser { get; set; }

        [JsonProperty(PropertyName = "voyageId")]
        public string VoyageId { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; } = "";

        [JsonProperty(PropertyName = "type")]
        public VoyageTypes Type { get; set; } = VoyageTypes.Normal;

        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; } = Status.New;

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; } = 0;

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

    }

    public enum VoyageTypes
    {
        Normal,
        Demo
    }

    public enum Status
    {
        New,
        Processed,
        Failed
    }

    //Events

    public class TransactionEvent
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; } = "";

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } = "";

        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; } = "";

        [JsonProperty(PropertyName = "branchId")]
        public string BranchId { get; set; } = "";

        [JsonProperty(PropertyName = "createdByUser")]
        public string CreatedByUser { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "type")]
        public VoyageTypes Type { get; set; } = VoyageTypes.Normal;

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; } = 0;

    }

    public class VoyageCreatedEvent
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = "voyageId")]
        public string VoyageId { get; set; }

        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; } = "";

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } = "";

        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; } = "";

        [JsonProperty(PropertyName = "branchId")]
        public string BranchId { get; set; } = "";

        [JsonProperty(PropertyName = "createdByUser")]
        public string CreatedByUser { get; set; }
    }

    public class VoyageUpdatedEvent
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = "isChapterMoved")]
        public bool IsChapterMoved { get; set; } = false;

        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; } = "";

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } = "";

        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; } = "";

        [JsonProperty(PropertyName = "branchId")]
        public string BranchId { get; set; } = "";

        [JsonProperty(PropertyName = "createdByUser")]
        public string CreatedByUser { get; set; }
    }

    public class VoyageFailedEvent
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string FailReason { get; set; }
    }
}
