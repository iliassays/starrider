using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using ServerlessMicroservices.Voyages.Core;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ServerlessMicroservices.Voyages.Core
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            //var config = (IConfiguration) builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(config[Settings.MONGO_CONNECTION_STRING]);

                return client;
            });
        }
    }
}


