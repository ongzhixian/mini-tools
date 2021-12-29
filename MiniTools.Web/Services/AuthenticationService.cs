using Microsoft.Extensions.Options;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Options;
using MongoDB.Driver;

namespace MiniTools.Web.Services;

public class AuthenticationService
{
    private static class On
    {
        internal static readonly EventId NEW = new EventId(1, "New");
        internal static readonly EventId VIEW_HOME = new EventId(101, "View home");

        internal static readonly EventId RECORD_FOUND = new EventId(201, "Record found");

        internal static readonly EventId INVALID_MODEL = new EventId(501, "Invalid model");
        internal static readonly EventId RECORD_NOT_FOUND = new EventId(502, "Record not found");
        internal static readonly EventId INVALID_CREDENTIAL = new EventId(503, "Invalid credential");
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

    public async Task<OperationResult<UserAccount>> GetValidUserAsync(LoginRequest model)
    {
        if (model == null || model.Username == null || model.Password == null)
        {
            logger.LogInformation(On.INVALID_MODEL, "{@model}", model);
            return OperationResult<UserAccount>.Fail(On.INVALID_MODEL);
        }

        // Get UserAccount

        User user = await userCollectionService.FindUserByUsernameAsync(model.Username);

        if (user == null)
        {
            logger.LogInformation(On.RECORD_NOT_FOUND, "{@model}", model);
            return OperationResult<UserAccount>.Fail(On.RECORD_NOT_FOUND);
        }

        // Check password

        if (user.Password == model.Password)
        {
            logger.LogInformation(On.RECORD_FOUND, "{@user}", user);
            return OperationResult<UserAccount>.Ok(On.RECORD_FOUND, user);
        }

        logger.LogInformation(On.INVALID_CREDENTIAL, "{@model}", model);
        return OperationResult<UserAccount>.Fail(On.INVALID_CREDENTIAL);
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

