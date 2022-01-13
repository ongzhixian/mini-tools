using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

namespace MiniTools.HostApp.Services;

public struct Jwt
{
    public string? Token { get; set; }
}

public class UserAuthenticationApiService
{
    private readonly ILogger<UserAuthenticationApiService> logger;
    private readonly HttpClient httpClient;

    public UserAuthenticationApiService(ILogger<UserAuthenticationApiService> logger, HttpClient httpClient)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.httpClient = httpClient;
        httpClient.BaseAddress = new Uri("https://localhost:7001");
    }

    public async Task GetBearerCredentialsAsync()
    {

        HttpResponseMessage? responseMessage = await httpClient.PostAsync("/api/UserAuthentication", JsonContent.Create<LoginRequest>(new LoginRequest
        {
            Username = "dev",
            Password = "dev"
        }, mediaType: new MediaTypeHeaderValue(MediaTypeNames.Application.Json)));

        responseMessage.EnsureSuccessStatusCode();

        if (MediaTypeNames.Application.Json != responseMessage.Content.Headers.ContentType?.MediaType)
        {
            return;
        }

        LoginResponse? loginResponse = await responseMessage.Content.ReadFromJsonAsync<LoginResponse>();

        Console.WriteLine("Got response");

        //EventBus<string>.Instance.NewData += (sender, e) =>
        //{
        //    Console.Write("SO");
        //}; 

        //EventBus.Instance.NewMessage += async (sender, e) =>
        //{
        //    await Task.Delay(1000);
        //};

        //EventBus.Instance.NewMessage += Instance_NewMessageAsync;

        if (loginResponse != null)
            MessageBus<string>.Instance.Send(loginResponse.Jwt);

        MessageBus<Jwt>.Instance.Send(new Jwt { Token = loginResponse?.Jwt });

        EventBus<Jwt>.Instance.Send(this, new Jwt { Token = loginResponse?.Jwt });
    }

    private sealed class LoginRequest
    {
        public string? Username { get; set; }

        public string? Password { get; set; }
    }

    private sealed class LoginResponse
    {
        public string Jwt { get; set; } = String.Empty;

        public DateTime ExpiryDateTime { get; set; }
    }

}
