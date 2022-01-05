using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Api.Responses;
using MiniTools.Web.Models;
using MiniTools.Web.Options;
using MiniTools.Web.UnitTests.Helpers;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTools.Web.Services.Tests;

[TestClass()]
public class AuthenticationApiServiceTests
{

    //ILogger<AuthenticationApiService> logger,
    //HttpClient httpClient,
    //IOptionsMonitor<ApiSettings> optionsMonitor,
    //IHttpContextAccessor httpContextAccessor

    Mock<ILogger<AuthenticationApiService>> mockLogger = new Mock<ILogger<AuthenticationApiService>>();
    Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    Mock<IOptionsMonitor<ApiSettings>> mockOptionsMonitor = new Mock<IOptionsMonitor<ApiSettings>>();
    Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

    [TestInitialize]
    public void BeforeTest()
    {
        mockLogger = new Mock<ILogger<AuthenticationApiService>>();
        mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockOptionsMonitor = new Mock<IOptionsMonitor<ApiSettings>>();
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        mockOptionsMonitor.Setup(m => m.Get("api")).Returns(new ApiSettings
            {
                { "CommonApi", "http://someUrlForCommonApi" }
            });
    }


    [TestMethod()]
    public void AuthenticationApiServiceTest()
    {
        AuthenticationApiService service = new AuthenticationApiService();

        Assert.IsNotNull(service);
    }

    [TestMethod()]
    public void AuthenticationApiServiceTest1()
    {
        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        Assert.IsNotNull(service);
    }

    [TestMethod()]
    public void AuthenticationApiServiceWithSessionTest1()
    {
        Mock<ISession> mockSession = new Mock<ISession>();
        byte[]? jwtBytes = Encoding.UTF8.GetBytes("mockJwtValue");
        mockSession.Setup(m => m.TryGetValue("JWT", out jwtBytes)).Returns(true);

        var httpContext = MockHelper.MakeHttpContext();
        httpContext.Session = mockSession.Object;

        mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        Assert.IsNotNull(service);
    }

    [TestMethod()]
    public async Task IsValidCredentialsAsyncForbiddenTestAsync()
    {
        LoginViewModel model = new LoginViewModel();

        mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden
            });

        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        var result = await service.IsValidCredentialsAsync(model);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OperationResult<LoginResponse>));
        Assert.IsFalse(result.Success);
    }

    [TestMethod()]
    public async Task IsValidCredentialsAsyncTestAsync()
    {
        LoginViewModel model = new LoginViewModel();

        LoginResponse response = new LoginResponse();

        mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            });

        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        var result = await service.IsValidCredentialsAsync(model);

        Assert.IsNotNull(result);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OperationResult<LoginResponse>));
        Assert.IsTrue(result.Success);
    }

    [TestMethod()]
    public async Task IsValidCredentialsAsyncNoPayloadTestAsync()
    {
        LoginViewModel model = new LoginViewModel();

        LoginResponse response = null;

        mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            });

        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        var result = await service.IsValidCredentialsAsync(model);

        Assert.IsNotNull(result);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OperationResult<LoginResponse>));
        Assert.IsFalse(result.Success);
    }

    [TestMethod()]
    public async Task IsValidCredentialsAsyncNoContentTestAsync()
    {
        LoginViewModel model = new LoginViewModel();

        LoginResponse response = new LoginResponse();

        mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        AuthenticationApiService service = new AuthenticationApiService(
            mockLogger.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            mockOptionsMonitor.Object,
            mockHttpContextAccessor.Object);

        var result = await service.IsValidCredentialsAsync(model);

        Assert.IsNotNull(result);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OperationResult<LoginResponse>));
        Assert.IsFalse(result.Success);
    }
}
