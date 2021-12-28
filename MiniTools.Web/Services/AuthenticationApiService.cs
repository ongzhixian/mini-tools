using Microsoft.Extensions.Options;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.Options;

namespace MiniTools.Web.Services;

public class AuthenticationApiService
{
    private const string _configurationKey = "Api:CommonApi:ServerUrl";

    private readonly HttpClient httpClient;
    private readonly ILogger<AuthenticationApiService> logger;
    //private readonly HttpContext httpContext;

    public AuthenticationApiService(
        IConfiguration configuration, 
        ILogger<AuthenticationApiService> logger, 
        HttpClient httpClient, 
        IOptionsMonitor<ApiSettings> optionsMonitor,
        IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (optionsMonitor != null)
        {
            ApiSettings apiSettings = optionsMonitor.Get("api");

            if (apiSettings.ContainsKey("CommonApi"))
            {
                string serverUrl = apiSettings["CommonApi"];

                this.httpClient.BaseAddress = new Uri($"{serverUrl}");
            }
        }

        if ((httpContextAccessor != null) && (httpContextAccessor.HttpContext != null))
        {
            HttpContext httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No HttpContext");

            string? jwt = httpContext.Session.GetString("JWT");

            if (jwt != null)
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }
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


    internal async Task<bool> IsValidCredentialsAsync(LoginViewModel model)
    {
        var result = await httpClient.PostAsJsonAsync<LoginRequest>("/api/Authentication", new LoginRequest(model));

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Oh no! What now?");
        }

        return true;
    }

    public async Task<PageData<UserAccount>?> GetUserListAsync(ushort pageNumber, ushort pageSize)
    {
        string url = $"/api/User?page={pageNumber}&pageSize={pageSize}";

        // httpClient.GetFromJsonAsync<> // <GetUserRequest>
        var result = await httpClient.GetAsync(url);

        var res = await result.Content.ReadFromJsonAsync<PageData<UserAccount>>();

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Oh no! What now?");
        }

        return res;
    }
}
