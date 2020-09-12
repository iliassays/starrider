namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.Domain.Voyage
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

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "voyager")]
        public Voyager Voyager { get; set; }

        [JsonProperty(PropertyName = "pilot")]
        public Pilot Pilot { get; set; } = null;

        [JsonProperty(PropertyName = "voyageChapters")]
        public List<VoyageChapter> VoyageChapters { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate { get; set; } = null;

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; } = "";

        [JsonProperty(PropertyName = "type")]
        public VoyageTypes Type { get; set; } = VoyageTypes.Normal;

        [JsonProperty(PropertyName = "activeChapter")]
        public VoyageChapter ActiveChapter { get; private set; }

        private VoyageChapter GetActiveChapter()
        {
            var currentlyActiveChapter = VoyageChapters.Find(c => c.StarCollected >= 0 && c.StarCollected < c.StarNeededToComplete);

            return currentlyActiveChapter;
        }

        private VoyageChapter SetActiveChapter(int newlyAddedStar)
        {
            var currentlyActiveChapter = this.GetActiveChapter();

            if (currentlyActiveChapter.JourneyBeginAt == null)
            {
                currentlyActiveChapter.JourneyBeginAt = DateTime.UtcNow;
            }

            if(currentlyActiveChapter.StarCollected + newlyAddedStar > currentlyActiveChapter.StarNeededToComplete)
            {
                int starNeedsMoveToNextChapter = (currentlyActiveChapter.StarCollected + newlyAddedStar) - currentlyActiveChapter.StarNeededToComplete;
                currentlyActiveChapter.StarCollected = currentlyActiveChapter.StarNeededToComplete;
                currentlyActiveChapter.SetUnlockReward();

                currentlyActiveChapter = this.GetActiveChapter();
                currentlyActiveChapter.StarCollected += starNeedsMoveToNextChapter;
                currentlyActiveChapter.JourneyBeginAt = DateTime.UtcNow;
            }
            else
            {
                currentlyActiveChapter.StarCollected += newlyAddedStar;
                currentlyActiveChapter.SetUnlockReward();
            }

            return currentlyActiveChapter;
        }

        public void ApplyNewlyAddedStar(int newlyAddedStar)
        {
            this.StarCollected += newlyAddedStar;
            SetActiveChapter(newlyAddedStar);
        }
    }

    public enum VoyageTypes
    {
        Normal,
        Demo
    }

    public class VoyageChapter
    {
        [JsonProperty(PropertyName = "chapterId")]
        public string ChapterId { get; set; } = "";

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "starNeededToComplete")]
        public int StarNeededToComplete { get; set; }

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "journeyBeginAt")]
        public DateTime JourneyBeginAt { get; set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "journeyEndAt")]
        public DateTime? JourneyEndAt { get; set; } = null;

        [JsonProperty(PropertyName = "unlockedRewards")]
        public List<VoyageChapterLockedReward> LockedRewards { get; set; } = new List<VoyageChapterLockedReward>();

        [JsonProperty(PropertyName = "unlockedRewards")]
        public List<VoyageChapterUnlockedReward> UnlockedRewards { get; set; } = new List<VoyageChapterUnlockedReward>();

        [JsonProperty(PropertyName = "isMissionCompleted")]
        public bool IsMissionCompleted {
            get
            {
                return StarNeededToComplete == StarCollected;
            }
        }

        public void SetUnlockReward()
        {
            var unlockedReward = new List<VoyageChapterUnlockedReward>();

            LockedRewards.ForEach(lr =>
            {
                if (this.StarCollected >= lr.StarNeededToUnlock)
                {
                    unlockedReward.Add(new VoyageChapterUnlockedReward
                    {
                        RewardId = lr.RewardId,
                        Title = lr.Title,
                        Description = lr.Description,
                        StarNeededToUnlock = lr.StarNeededToUnlock,
                    });

                    LockedRewards.Remove(lr);
                }

                if (unlockedReward.Count > 0)
                {
                    this.UnlockedRewards.AddRange(unlockedReward);
                }
            });
        }
    }

    public class VoyageChapterLockedReward
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

    public class VoyageChapterUnlockedReward
    {
        [JsonProperty(PropertyName = "rewardId")]
        public string RewardId { get; set; } = "";

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "starNeededToUnlock")]
        public int StarNeededToUnlock { get; set; }

        [JsonProperty(PropertyName = "unlockedAt")]
        public DateTime? UnlockedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "isRedeemed")]
        public bool IsRedeemed { get; set; } = false;

        [JsonProperty(PropertyName = "redeemedAt")]
        public DateTime? RedeemedAt { get; set; } = null;
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

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; } = "";
    }

    //From transaction api
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

    //From voyage maker  api
    public class VoyageMaker
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; } = "";

        [JsonProperty(PropertyName = "organizationPhone")]
        public string OrganizationPhone { get; set; } = "";

        [JsonProperty(PropertyName = "chapters")]
        public List<Chapter> Chapters { get; set; }

        internal object CalculateStarFromTransaction(decimal transactionAmount)
        {
            throw new NotImplementedException();
        }
    }

    public class Chapter
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "";

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "starNeededToComplete")]
        public int StarNeededToComplete { get; set; }

        [JsonProperty(PropertyName = "rewards")]
        public List<ChapterReward> ChapterRewards { get; set; }
    }

    public class ChapterReward
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

    // Events

    public class VoyageCreatedEvent
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "voyageId")]
        public string VoyageId { get; set; }

        [JsonProperty(PropertyName = "starCollected")]
        public int StarCollected { get; set; } = 0;

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

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

        [JsonProperty(PropertyName = "voyageId")]
        public string VoyageId { get; set; }

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
