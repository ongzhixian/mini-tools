using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.Options;
using MiniTools.Web.Services;
using MiniTools.Web.UnitTests.Helpers;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTools.Web.Services.Tests
{
    [TestClass()]
    public class UserApiServiceTests
    {
        //Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
        //Mock<IAuthenticationApiService> mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
        //Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

        Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
        Mock<ILogger<UserApiService>> mockLogger = new Mock<ILogger<UserApiService>>();
        Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        Mock<IOptionsMonitor<ApiSettings>> mockOptionsMonitor = new Mock<IOptionsMonitor<ApiSettings>>();
        Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        [TestInitialize]
        public void BeforeTest()
        {
            mockConfiguration = new Mock<IConfiguration>();
            mockLogger = new Mock<ILogger<UserApiService>>();
            mockOptionsMonitor = new Mock<IOptionsMonitor<ApiSettings>>();
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockOptionsMonitor.Setup(m => m.Get("api")).Returns(new ApiSettings
            {
                { "CommonApi", "http://someUrlForCommonApi" }
            });

            Mock<ISession> mockSession = new Mock<ISession>();
            byte[]? jwtBytes = Encoding.UTF8.GetBytes("mockJwtValue");
            mockSession.Setup(m => m.TryGetValue("JWT", out jwtBytes)).Returns(true);
            var httpContext = MockHelper.MakeHttpContext();
            httpContext.Session = mockSession.Object;
            mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);
        }

        [TestMethod()]
        public void UserApiServiceTest()
        {
            UserApiService service = new UserApiService(
                mockConfiguration.Object,
                mockLogger.Object,
                new HttpClient(),
                mockOptionsMonitor.Object,
                mockHttpContextAccessor.Object);

            Assert.IsNotNull(service);
        }

        [TestMethod()]
        public async Task AddUserAsyncTestAsync()
        {
            AddUserViewModel model = new AddUserViewModel();

            string response = string.Empty;

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

            UserApiService service = new UserApiService(
                mockConfiguration.Object,
                mockLogger.Object,
                new HttpClient(mockHttpMessageHandler.Object),
                mockOptionsMonitor.Object,
                mockHttpContextAccessor.Object);

            await service.AddUserAsync(model);

            Assert.Inconclusive("Bad implementation");
        }

        [TestMethod()]
        public async Task AddUserAsyncFailTestAsync()
        {
            AddUserViewModel model = new AddUserViewModel();

            string response = string.Empty;

            mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonSerializer.Serialize(response))
                });

            UserApiService service = new UserApiService(
                mockConfiguration.Object,
                mockLogger.Object,
                new HttpClient(mockHttpMessageHandler.Object),
                mockOptionsMonitor.Object,
                mockHttpContextAccessor.Object);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await service.AddUserAsync(model));
        }

        [TestMethod()]
        public async Task GetUserListAsyncTestAsync()
        {
            AddUserViewModel model = new AddUserViewModel();

            PageData<UserAccount> response = new PageData<UserAccount>();

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

            UserApiService service = new UserApiService(
                mockConfiguration.Object,
                mockLogger.Object,
                new HttpClient(mockHttpMessageHandler.Object),
                mockOptionsMonitor.Object,
                mockHttpContextAccessor.Object);

            var result = await service.GetUserListAsync(1, 10);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PageData<UserAccount>));
        }

        [TestMethod()]
        public async Task GetUserListAsyncFailTestAsync()
        {
            AddUserViewModel model = new AddUserViewModel();

            PageData<UserAccount> response = new PageData<UserAccount>();

            mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonSerializer.Serialize(response))
                });

            UserApiService service = new UserApiService(
                mockConfiguration.Object,
                mockLogger.Object,
                new HttpClient(mockHttpMessageHandler.Object),
                mockOptionsMonitor.Object,
                mockHttpContextAccessor.Object);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await service.GetUserListAsync(1, 10));

        }
    }
}