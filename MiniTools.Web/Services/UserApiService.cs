using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.Options;

namespace MiniTools.Web.Services;


public class UserApiService
{
    private const string _configurationKey = "Api:CommonApi:ServerUrl";

    private readonly HttpClient httpClient;
    private readonly ILogger<UserApiService> logger;
    private readonly HttpContext httpContext;

    public UserApiService(
        IConfiguration configuration, 
        ILogger<UserApiService> logger, 
        HttpClient httpClient, 
        IOptionsMonitor<ApiSettings> optionsMonitor,
        IHttpContextAccessor httpContextAccessor)
    {
        // string serverUrl = configuration.GetValue<string>(_configurationKey) ?? throw new Exception($"Invalid configuration key: [{_configurationKey}]");

        //string serverUrl = "https://localhost:7001";

        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        this.httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No HttpContext");

        // optionsMonitor
        if (optionsMonitor != null)
        {
            ApiSettings apiSettings = optionsMonitor.Get("api");

            logger.LogInformation("apiSettings exists: {exist}", (apiSettings != null));

            // Original implementation when we thought using `Api` property
            //if ((apiSettings != null) && (apiSettings.Api != null))
            //{
            //    logger.LogInformation("NNNNNNNNNNNNNNNN");
            //    foreach (var x in apiSettings.Api)
            //    {
            //        logger.LogInformation(x.Key);
            //        logger.LogInformation(x.Value);
            //    }    
            //}

            //if (apiSettings != null)
            //{
            //    logger.LogInformation("NNNNNNNNNNNNNNNN");
            //    foreach (var x in apiSettings)
            //    {
            //        logger.LogInformation(x.Key);
            //        logger.LogInformation(x.Value);
            //    }
            //}

            // logger.LogInformation("apiSettings", apiSettings.Api["CommonApi"]);
            // logger.LogInformation("mongoDbSettings connectionString: {connectionString}", mongoDbSettings.ConnectionString);

            if ((apiSettings != null) && (apiSettings.ContainsKey("CommonApi")))
            {
                string serverUrl = apiSettings["CommonApi"];

                this.httpClient.BaseAddress = new Uri($"{serverUrl}"); // Example Uri: https://api.github.com/

                //Microsoft.Net.Http.Headers.HeaderNames.TraceParent
            }
        }
        

        string? jwt = this.httpContext.Session.GetString("JWT");

        if (jwt != null)
            this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

        
        //this.httpClient.DefaultRequestHeaders.Add(HeaderNames.TraceParent)
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
    }


    public async Task AddUserAsync(AddUserViewModel model)
    {
        var result = await httpClient.PostAsJsonAsync<AddUserRequest>("/api/User", new AddUserRequest(model));

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Oh no! What now?");
        }

        // AddUserRequest postData = new AddUserRequest(model);
        // // var result = await httpClient.PostAsJsonAsync<LoginRequest>("login", postData);
        // this.httpClient.PostAsJsonAsync<AddUserRequest>("login", postData);
    }

    public async Task<PageData<UserAccount>?> GetUserListAsync(ushort pageNumber, ushort pageSize)
    {
        string url = $"/api/User?page={pageNumber}&pageSize={pageSize}";

        // httpClient.GetFromJsonAsync<> // <GetUserRequest>
        var result = await httpClient.GetAsync(url);

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Oh no! What now?");
        }

        try
        {
            PageData<UserAccount>? res = await result.Content.ReadFromJsonAsync<PageData<UserAccount>>();

            return res;
        }
        catch (Exception)
        {
            throw;
        }

    }


    //public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
    //    await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
    //        "repos/dotnet/AspNetCore.Docs/branches");
}
