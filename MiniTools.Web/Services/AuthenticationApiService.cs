using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.Api.Responses;
using MiniTools.Web.Models;
using MiniTools.Web.Options;
using System.Net.Mime;

namespace MiniTools.Web.Services;

public interface IAuthenticationApiService
{
    Task<OperationResult<LoginResponse>> IsValidCredentialsAsync(LoginViewModel model);
}

public class AuthenticationApiService : IAuthenticationApiService
{
    private static class On
    {
        internal static readonly EventId VALIDATE_FAIL = new EventId(1, "Credential validation failure.");

        internal static readonly EventId VALIDATE_SUCCESS = new EventId(1, "Credential validation success");

        internal static readonly EventId READ_FAIL = new EventId(1, "Cannot read credential validation result");

        internal static readonly EventId EMPTY_RESPONSE_FAIL = new EventId(1, "credential validation result is empty");

    }

    private readonly ILogger<AuthenticationApiService> logger;

    private readonly HttpClient httpClient;

    public AuthenticationApiService()
    {

    }

    public AuthenticationApiService(
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

            this.httpClient.DefaultRequestHeaders.Add(HeaderNames.TraceParent, httpContext.TraceIdentifier);
            //this.httpClient.DefaultRequestHeaders.Add(HeaderNames.RequestId, httpContext.TraceIdentifier);

            //logger.LogInformation("TraceIdentifier {0}", httpContext.TraceIdentifier);

            //logger.LogInformation("RequestHeader count:{0}", httpContext.Request.Headers.Count());
            //foreach (var item in httpContext.Request.Headers)
            //{
            //    logger.LogInformation("RequestHeader: [{key}] >> [{value}]", item.Key, item.Value);
            //}

            //logger.LogInformation("ResponseHeader count:{0}", httpContext.Response.Headers.Count());
            //foreach (var item in httpContext.Response.Headers)
            //{
            //    logger.LogInformation("ResponseHeader: [{key}] >> [{value}]", item.Key, item.Value);
            //}

        }
    }

    public async Task<OperationResult<LoginResponse>> IsValidCredentialsAsync(LoginViewModel model)
    {
        HttpResponseMessage? result = await httpClient.PostAsJsonAsync<LoginRequest>("/api/UserAuthentication", new LoginRequest(model));

         //== MediaTypeNames.Text.Html

        if ((!result.IsSuccessStatusCode) || (MediaTypeNames.Application.Json != result.Content.Headers.ContentType?.MediaType))
        {
            logger.LogInformation(On.VALIDATE_FAIL, "{@IsSuccessStatusCode}", result.IsSuccessStatusCode);
            return OperationResult<LoginResponse>.Fail();
        }

        try
        {
            LoginResponse? loginResponse = await result.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponse != null)
            {
                logger.LogInformation(On.VALIDATE_SUCCESS, "{@IsSuccessStatusCode} {@response}", result.IsSuccessStatusCode);
                return OperationResult<LoginResponse>.Ok(On.VALIDATE_SUCCESS, loginResponse);
            }

            logger.LogInformation(On.EMPTY_RESPONSE_FAIL, "{@IsSuccessStatusCode} {@response}", result.IsSuccessStatusCode);
            return OperationResult<LoginResponse>.Fail(On.EMPTY_RESPONSE_FAIL);
        }
        catch (Exception ex)
        {
            logger.LogError(On.READ_FAIL, ex, "{@IsSuccessStatusCode} {@response}", result.IsSuccessStatusCode);
            return OperationResult<LoginResponse>.Fail(On.READ_FAIL);
        }
    }
}
