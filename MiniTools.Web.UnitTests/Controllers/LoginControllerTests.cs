using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using MiniTools.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MiniTools.Web.Models;
using System.Diagnostics.CodeAnalysis;
using MiniTools.Web.Api.Responses;
using Microsoft.Extensions.Options;
using MiniTools.Web.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace MiniTools.Web.Controllers.Tests
{
    [TestClass()]
    public class LoginControllerTests
    {
        [TestMethod()]
        public void LoginControllerTest()
        {
            Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
            Mock<AuthenticationApiService> mockAuthenticationApiService = new Mock<AuthenticationApiService>();
            Mock<JwtService> mockJwtService = new Mock<JwtService>();

            LoginController loginController = new LoginController(
                mockLogger.Object,
                mockAuthenticationApiService.Object,
                mockJwtService.Object
                );

            Assert.IsNotNull(loginController);
        }

        [TestMethod()]
        public void LoginControllerNoLoggerTest()
        {
            Mock<AuthenticationApiService> mockAuthenticationApiService = new Mock<AuthenticationApiService>();
            Mock<JwtService> mockJwtService = new Mock<JwtService>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new LoginController(
                null,
                mockAuthenticationApiService.Object,
                mockJwtService.Object
                )
            );
        }

        [TestMethod()]
        public void LoginControllerNoAuthenticationApiServiceTest()
        {
            Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
            Mock<JwtService> mockJwtService = new Mock<JwtService>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new LoginController(
                    mockLogger.Object,
                    null,
                    mockJwtService.Object
                )
            );
        }


        [TestMethod()]
        public void LoginControllerNoJwtServiceTest()
        {
            Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
            Mock<AuthenticationApiService> mockAuthenticationApiService = new Mock<AuthenticationApiService>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new LoginController(
                    mockLogger.Object,
                    mockAuthenticationApiService.Object,
                    null)
            );

        }

        [TestMethod()]
        public void IndexTest()
        {
            Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
            Mock<AuthenticationApiService> mockAuthenticationApiService = new Mock<AuthenticationApiService>();
            Mock<JwtService> mockJwtService = new Mock<JwtService>();

            LoginController loginController = new LoginController(
                mockLogger.Object,
                mockAuthenticationApiService.Object,
                mockJwtService.Object
                );

            IActionResult? result = loginController.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginViewModel));
        }

        [TestMethod()]
        public async Task IndexAsyncTestAsync()
        {
            Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
            Mock<IAuthenticationApiService> mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
            Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

            //Mock<IOptionsMonitor<JwtSettings>> mockOptionsMonitor = new Mock<IOptionsMonitor<JwtSettings>>();
            //mockOptionsMonitor.Setup(m => m.Get("jwt")).Returns(new JwtSettings
            //{
            //    SecretKey = "placeHolderSecretKey",
            //    ValidIssuer = "placeHolderValidIssuer",
            //    ValidAudience = "placeHolderValidAudience"
            //});

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            OperationResult<LoginResponse> operationResult = new(true, new LoginResponse()
            {
                ExpiryDateTime = DateTime.UtcNow,
                Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            });

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);

            LoginController loginController = new LoginController(
                mockLogger.Object,
                mockAuthenticationApiService.Object,
                mockJwtService.Object
                );

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);

        }
    }
}