using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using MiniTools.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MiniTools.Web.Models;
using MiniTools.Web.Api.Responses;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MiniTools.Web.Controllers.Tests
{
    [TestClass()]
    public class LoginControllerTests
    {
        Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
        Mock<IAuthenticationApiService> mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
        Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

        [TestInitialize]
        public void BeforeTest()
        {
            mockLogger = new Mock<ILogger<LoginController>>();
            mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
            mockJwtService = new Mock<IJwtService>();
        }

        [TestMethod()]
        public void LoginControllerTest()
        {
            LoginController loginController = GetValidLoginController();

            Assert.IsNotNull(loginController);
        }

        [TestMethod()]
        public void LoginControllerNoLoggerTest()
        {
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
            LoginController loginController = GetValidLoginController();

            IActionResult? result = loginController.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginViewModel));
        }

        [TestMethod()]
        public async Task IndexAsyncTestAsync()
        {
            OperationResult<LoginResponse> operationResult = new(true, new LoginResponse()
            {
                ExpiryDateTime = DateTime.UtcNow,
                Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            });

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);

            LoginController loginController = GetValidLoginController();

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);

        }

        [TestMethod()]
        public async Task IndexAsyncAuthenticationFailTestAsync()
        {
            OperationResult<LoginResponse> operationResult = new(false);

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);
            
            LoginController loginController = GetValidLoginController();

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginViewModel));
        }

        [TestMethod()]
        public async Task IndexAsyncAuthenticationEmptyPayloadTestAsync()
        {
            OperationResult<LoginResponse> operationResult = new(true);

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);

            LoginController loginController = GetValidLoginController();

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginViewModel));
        }

        [TestMethod()]
        public async Task IndexAsyncInvalidModelStateTestAsync()
        {
            OperationResult<LoginResponse> operationResult = new(true, new LoginResponse()
            {
                ExpiryDateTime = DateTime.UtcNow,
                Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            });

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);

            LoginController loginController = GetValidLoginController();

            loginController.ModelState.AddModelError("Error", "Mock error");

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod()]
        public async Task IndexAsyncExceptionHandlingTestAsync()
        {
            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).Throws(new Exception("Mock Exception"));

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            LoginController loginController = GetValidLoginController();

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginViewModel));
        }

        [TestMethod()]
        public async Task IndexAsyncHttpContextTestAsync()
        {
            OperationResult<LoginResponse> operationResult = new(true, new LoginResponse()
            {
                ExpiryDateTime = DateTime.UtcNow,
                Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            });

            mockAuthenticationApiService.Setup(a => a.IsValidCredentialsAsync(It.IsAny<LoginViewModel>())).ReturnsAsync(operationResult);

            mockJwtService.Setup(m => m.GetClaims("jwt")).Returns(new List<Claim>());

            LoginController loginController = GetValidLoginController();

            loginController.ControllerContext.HttpContext = MakeHttpContext();

            IActionResult? result = await loginController.IndexAsync(new LoginViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        private LoginController GetValidLoginController()
        {
            LoginController loginController = new LoginController(
                mockLogger.Object,
                mockAuthenticationApiService.Object,
                mockJwtService.Object
                );
            return loginController;
        }

        // Test helpers (drivers)

        DefaultHttpContext MakeHttpContext()
        {
            // Need mock of IAuthenticationService to handle HttpContext.SignIn
            var mockAuthenticationService = new Mock<IAuthenticationService>();

            mockAuthenticationService
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Need mock of IUrlHelperFactory to properly handle RedirectToAction
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();

            // Setup mock service provider

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationService.Object);

            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            return new DefaultHttpContext()
            {
                RequestServices = serviceProviderMock.Object
            };
        }
    }
}