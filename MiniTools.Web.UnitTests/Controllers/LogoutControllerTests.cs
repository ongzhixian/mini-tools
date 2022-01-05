using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Controllers;
using MiniTools.Web.Services;
using MiniTools.Web.UnitTests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTools.Web.Controllers.Tests
{
    [TestClass()]
    public class LogoutControllerTests
    {
        Mock<ILogger<LogoutController>> mockLogger = new Mock<ILogger<LogoutController>>();
        Mock<IAuthenticationApiService> mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
        Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

        [TestInitialize]
        public void BeforeTest()
        {
            mockLogger = new Mock<ILogger<LogoutController>>();
            mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
            mockJwtService = new Mock<IJwtService>();
        }

        [TestMethod()]
        public async Task IndexAsyncTestAsync()
        {
            LogoutController logoutController = new LogoutController();

            logoutController.ControllerContext.HttpContext = MockHelper.MakeHttpContext();

            IActionResult? result = await logoutController.IndexAsync();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual("Home", ((RedirectToActionResult)result).ControllerName);

        }
    }
}