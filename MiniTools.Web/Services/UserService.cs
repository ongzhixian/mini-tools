using MiniTools.Web.Api.Requests;
using MiniTools.Web.Models;

namespace MiniTools.Web.Services
{
    public class UserService
    {
        private const string _configurationKey = "Api:CommonApi:ServerUrl";

        private readonly HttpClient httpClient;
        private readonly ILogger<UserService> logger;
        private readonly HttpContext httpContext;

        public UserService(IConfiguration configuration, ILogger<UserService> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            string serverUrl = configuration.GetValue<string>(_configurationKey) ?? throw new Exception($"Invalid configuration key: [{_configurationKey}]");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No HttpContext");

            string? jwt = this.httpContext.Session.GetString("JWT");

            // Configure HTTP Client

            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            //_httpClient.BaseAddress = new Uri("https://api.github.com/");
            this.httpClient.BaseAddress = new Uri($"{serverUrl}");

            if (jwt != null)
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
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


        //public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
        //    await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
        //        "repos/dotnet/AspNetCore.Docs/branches");
    }
}
