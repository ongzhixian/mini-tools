using Microsoft.Extensions.Options;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Options;
using MongoDB.Driver;

namespace MiniTools.Web.Services;

public class AuthenticationService
{
    private class On
    {
        internal static EventId NEW = new EventId(1, "New");
        internal static EventId VIEW_HOME = new EventId(101, "View home");
    }
    private readonly ILogger<AuthenticationService> logger;
    private readonly UserCollectionService userCollectionService;

    public AuthenticationService(ILogger<AuthenticationService> logger, UserCollectionService userCollectionService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userCollectionService = userCollectionService ?? throw new ArgumentNullException(nameof(userCollectionService));
    }

    internal async Task<bool> ValidCredentialsAsync(LoginRequest model)
    {
        if (model == null || model.Username == null || model.Password == null)
            return false;

        // Get UserAccount

        var user = await userCollectionService.FindUserByUsernameAsync(model.Username);

        if (user == null)
            return false;

        // Check password

        return user.Password == model.Password;
    }

    internal async Task<IOutcome> GetValidUserAsync(LoginRequest model)
    {
        if (model == null || model.Username == null || model.Password == null)
            return Outcome.Negative;

        // Get UserAccount

        var user = await userCollectionService.FindUserByUsernameAsync(model.Username);

        if (user == null)
            return Outcome.Negative;

        // Check password

        if (user.Password == model.Password)
            return new PositiveOutcome<MongoEntities.User>(user);

        return Outcome.Negative;
    }


    //private readonly IMongoClient mongoClient;

    //public AuthenticationService(ILogger<AuthenticationService> logger, UserCollectionService userCollectionService,
    //    IMongoClient mongoClient, 
    //    IOptionsMonitor<MongoDbSettings> optionsMonitor)
    //{
    //    this.logger = logger ?? throw new Exception(nameof(logger));
    //    this.mongoClient = mongoClient ?? throw new Exception(nameof(mongoClient));

    //    // var client = new MongoClient(_configuration["mongoDb:safeTravel"]);
    //    // var database = client.GetDatabase("minitools");
    //    // _countries = database.GetCollection<Country>("country");

    //    logger.LogInformation("mongoClient exists: {exist}", (mongoClient != null));

    //    logger.LogInformation("optionsMonitor exists: {exist}", (optionsMonitor != null));

    //    if (optionsMonitor != null)
    //    {
    //        MongoDbSettings mongoDbSettings = optionsMonitor.Get("mongodb:minitools");
    //        logger.LogInformation("mongoDbSettings exists: {exist}", (mongoDbSettings != null));
    //        logger.LogInformation("mongoDbSettings connectionString: {connectionString}", mongoDbSettings.ConnectionString);
    //    }

    //    // mongoClient = new MongoClient()

    //    // IOptionsMonitor<MongoDbSettings> optionsMonitor)

    //    // var client = new MongoClient(_configuration["mongoDb:safeTravel"]);
    //    // var database = client.GetDatabase("safe_travel");
    //    // _countries = database.GetCollection<Country>("country");

    //    // mongoClient = new MongoClient()

    //}


}

public interface IOutcome
{
    // A Result has a Outcome (Success/Failure) (true/false)
    bool Positive { get; }

    object Value { get; }
}

public sealed class Outcome
{
    private static readonly object objectLock = new object();

    private static NegativeOutcome? negativeResult = null;
    private static PositiveOutcome? positiveResult = null;

    public static NegativeOutcome Negative
    {
        get
        {
            if (negativeResult == null)
            {
                lock (objectLock)
                {
                    if (negativeResult == null)
                    {
                        negativeResult = new NegativeOutcome();
                    }
                }
            }
            return negativeResult;
        }
    }

    public static PositiveOutcome Positive
    {
        get
        {
            if (positiveResult == null)
            {
                lock (objectLock)
                {
                    if (positiveResult == null)
                    {
                        positiveResult = new PositiveOutcome();
                    }
                }
            }
            return positiveResult;
        }
    }

    //public static PositiveResult<T> Positive<T>(T a) where T : class
    //{
    //    return new PositiveResult<T>(a);
    //}

    private Outcome() {}
}

public class PositiveOutcome : IOutcome
{
    public bool Positive => true;

    public object? Value => null;
}

public class NegativeOutcome : IOutcome
{
    public bool Positive
    {
        get => false;
    }

    private static NegativeOutcome? instance = null;

    private static readonly object _lock = new object();

    public NegativeOutcome()
    {

    }

    public static NegativeOutcome Instance
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new NegativeOutcome();
                    }
                }
            }
            return instance;
        }
    }

}

public class PositiveOutcome<T> : IOutcome where T : class
{
    public bool Positive => true;

    public T Value;

    public PositiveOutcome()
    {

    }

    public PositiveOutcome(T obj)
    {
        this.Value = obj;
    }

}