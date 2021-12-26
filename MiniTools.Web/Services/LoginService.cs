using Microsoft.Extensions.Options;
using MiniTools.Web.Options;
using MongoDB.Driver;

namespace MiniTools.Web.Services;

public class LoginService
{
    private class On
    {
        internal static EventId NEW = new EventId(1, "New");
        internal static EventId VIEW_HOME = new EventId(101, "View home");
    }
    private readonly ILogger<LoginService> logger;
    private readonly IMongoClient mongoClient;

    public LoginService(ILogger<LoginService> logger, IMongoClient mongoClient, 
        IOptionsMonitor<MongoDbSettings> optionsMonitor)
    {
        this.logger = logger ?? throw new Exception(nameof(logger));
        this.mongoClient = mongoClient ?? throw new Exception(nameof(mongoClient));
        
        // var client = new MongoClient(_configuration["mongoDb:safeTravel"]);
        // var database = client.GetDatabase("minitools");
        // _countries = database.GetCollection<Country>("country");
        
        logger.LogInformation("mongoClient exists: {exist}", (mongoClient != null));

        logger.LogInformation("optionsMonitor exists: {exist}", (optionsMonitor != null));

        if (optionsMonitor != null)
        {
            MongoDbSettings mongoDbSettings = optionsMonitor.Get("mongodb:minitools");
            logger.LogInformation("mongoDbSettings exists: {exist}", (mongoDbSettings != null));
            logger.LogInformation("mongoDbSettings connectionString: {connectionString}", mongoDbSettings.ConnectionString);
        }

        // mongoClient = new MongoClient()

        // IOptionsMonitor<MongoDbSettings> optionsMonitor)

        // var client = new MongoClient(_configuration["mongoDb:safeTravel"]);
        // var database = client.GetDatabase("safe_travel");
        // _countries = database.GetCollection<Country>("country");

        // mongoClient = new MongoClient()

    }

    public void GetJwt()
    {

    }
}