using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Serilog;

namespace CreditApplicationAnalyzer
{
    internal static class Startup
    {
        private static IConfiguration Configuration { get; set; }

        static Startup()
        {
            Configuration = LoadConfiguration();
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
            Configuration = builder.Build();

            return Configuration;
        }

        public static ILogger CreateSerilog()
        {
            return new LoggerConfiguration().
                MinimumLevel.Verbose().
                WriteTo.Console().
                WriteTo.MongoDB(new MongoClient().GetDatabase(Configuration.GetSection("MongoDB:DBName").Value),
                                collectionName: Configuration.GetSection("MongoDB:LogsCollection").Value).
                CreateLogger();
        }
    }
}