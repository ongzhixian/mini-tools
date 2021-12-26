using MiniTools.Web.Api.Requests;
using MiniTools.Web.Models;

namespace MiniTools.Web.Services
{
    public class UserService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<UserService> logger;

        public UserService(ILogger<UserService> logger, HttpClient httpClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            //_httpClient.BaseAddress = new Uri("https://api.github.com/");

            //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
            //_httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");

        }

        //public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
        //    await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
        //        "repos/dotnet/AspNetCore.Docs/branches");

        public void AddUser(AddUserViewModel model)
        {
            AddUserRequest postData = new AddUserRequest(model);
            
            // var result = await httpClient.PostAsJsonAsync<LoginRequest>("login", postData);
            this.httpClient.PostAsJsonAsync<AddUserRequest>("login", postData);

            
        }
    }
}
