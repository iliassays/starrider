namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Command
{
    using Newtonsoft.Json;

    public class SaveVoyageMakerChapter
    {
        [JsonProperty(PropertyName = "starNeededToComplete")]
        public double StarNeededToComplete { get; set; } = 100;

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "chapterFor")]
        public ChapterForType ChapterFor { get; set; } = ChapterForType.BasicCustomer;
    }

    public enum ChapterForType
    {
        BasicCustomer,
        GoldCustomer,
        PlatinumCustomer
    }
}
