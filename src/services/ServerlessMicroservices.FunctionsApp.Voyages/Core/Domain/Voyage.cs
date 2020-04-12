namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Voyage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "voyager")]
        public Voyager Voyager { get; set; }

        [JsonProperty(PropertyName = "pilot")]
        public Pilot Pilot { get; set; } = null;

        [JsonProperty(PropertyName = "voyageChapters")]
        public List<VoyageChapter> VoyageChapters { get; set; } = new List<VoyageChapter>();

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate { get; set; } = null;

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; } = "";

        [JsonProperty(PropertyName = "type")]
        public VoyageTypes Type { get; set; } = VoyageTypes.Normal;

    }

    public enum VoyageTypes
    {
        Normal,
        Demo
    }

    public class VoyageChapter
    {
        [JsonProperty(PropertyName = "starNeededToComplete")]
        public double StarNeededToComplete { get; set; } = 0;

        [JsonProperty(PropertyName = "starCollected")]
        public double StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "journeyBeginAt")]
        public DateTime JourneyBeginAt { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "journeyEndAt")]
        public DateTime? JourneyEndAt { get; set; } = null;

        [JsonProperty(PropertyName = "unlockedRewards")]
        public List<Reward> UnlockedRewards { get; set; } = new List<Reward>();

        [JsonProperty(PropertyName = "lockedRewards")]
        public List<Reward> LockedRewards { get; set; } = new List<Reward>();
    }

    public class Reward
    {
        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "unlockedAt")]
        public DateTime? UnlockedAt { get; set; } = null;

        [JsonProperty(PropertyName = "IsRedeemed")]
        public bool IsRedeemed { get; set; } = false;

        [JsonProperty(PropertyName = "RedeemedAt")]
        public DateTime? RedeemedAt { get; set; } = null;

        [JsonProperty(PropertyName = "RedeemedBy")]
        public string RedeemedBy { get; set; } = "";

        [JsonProperty(PropertyName = "RedeemedLocation")]
        public string RedeemedLocation { get; set; } = "";
    }

    public class Voyager
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; } = "";

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; } = "";

        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; } = "";

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } = "";

    }

    public class Pilot
    {
        [JsonProperty(PropertyName = "organizerId")]
        public string OrganizerId { get; set; } = "";

        [JsonProperty(PropertyName = "organizerName")]
        public string OrganizerName { get; set; } = "";
    }
}
