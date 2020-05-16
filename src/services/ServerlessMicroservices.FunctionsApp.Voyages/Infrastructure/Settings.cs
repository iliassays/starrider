using System;

namespace ServerlessMicroservices.Voyages.Core
{
    public class Settings
    {
        private const string EnableAuthKey = "EnableAuth";

        // Storage
        private const string StorageAccountKey = "AzureWebJobsStorage";

        private const string InsightsInstrumentationKey = "InsightsInstrumentationKey";

        // Event Grid
        private const string TransactionExternalizationsEventGridTopicUrl = "TransactionExternalizationsEventGridTopicUrl";
        private const string TransactionExternalizationsEventGridTopicApiKey = "TransactionExternalizationsEventGridTopicApiKey";

        private const string VoyageExternalizationsEventGridTopicUrl = "VoyageExternalizationsEventGridTopicUrl";
        private const string VoyageExternalizationsEventGridTopicApiKey = "VoyageExternalizationsEventGridTopicApiKey";

        //Urls
        private const string TripMakerApi_GetTripMakerUrl = "TripMakerApi_GetTripMakerUrl";
        private const string TripMakerApi_CalculateStarForTransactionUrl = "TripMakerApi_CalculateStarForTransactionUrl";

        public bool EnableAuth()
        {
            if (
                GetEnvironmentVariable(EnableAuthKey) != null &&
                !string.IsNullOrEmpty(GetEnvironmentVariable(EnableAuthKey).ToString()) &&
                GetEnvironmentVariable(EnableAuthKey).ToString().ToLower() == "true"
            )
            {
                return true;
            }

            return false;
        }

        public static string GetStorageAccount()
        {
            return GetEnvironmentVariable(StorageAccountKey);
        }

        public static string GetInsightsInstrumentationKey()
        {
            return GetEnvironmentVariable(InsightsInstrumentationKey);
        }

        // Urls

        public static string GetTransactionExternalizationsEventGridTopicUrl()
        {
            return GetEnvironmentVariable(TransactionExternalizationsEventGridTopicUrl);
        }

        public static string GetTransactionExternalizationsEventGridTopicApiKey()
        {
            return GetEnvironmentVariable(TransactionExternalizationsEventGridTopicApiKey);
        }

        // Event Grid Urls
        public static string GetTripMakerApi_GetTripMakerUrl()
        {
            return GetEnvironmentVariable(TripMakerApi_GetTripMakerUrl);
        }

        public static string GetTripMakerApi_CalculateStarForTransactionUrl()
        {
            return GetEnvironmentVariable(TripMakerApi_CalculateStarForTransactionUrl);
        }

        public static string GetVoyageExternalizationsEventGridTopicUrl()
        {
            return GetEnvironmentVariable(VoyageExternalizationsEventGridTopicUrl);
        }

        public static string GetVoyageExternalizationsEventGridTopicApiKey()
        {
            return GetEnvironmentVariable(VoyageExternalizationsEventGridTopicApiKey);
        }

        //*** PRIVATE ***//
        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}