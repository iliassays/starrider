namespace ServerlessMicroservices.Voyages.Core
{
    public class Constants
    {
        public const string MONGO_CONNECTION_STRING = "MONGO_CONNECTION_STRING";
        public const string DATABASE_NAME = "DATABASE_NAME";
        public const string COLLECTION_NAME = "COLLECTION_NAME";

        public const string EVG_EVENT_TRANSACTION_CREATED = "TransactionCreated";

        public const string EVG_EVENT_VOYAGES_CREATED = "VoyageCreated";
        public const string EVG_EVENT_VOYAGES_UPDATED = "VoyageUpdated";
        public const string EVG_EVENT_VOYAGES_FAILED = "VoyageFailed";
    }
}