using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.Api.Responses;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;
using Moq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniTools.Web.Api.Tests;

[TestClass()]
public class UserAuthenticationControllerTests
{
    Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
    Mock<ILogger<UserAuthenticationController>> mockLogger = new Mock<ILogger<UserAuthenticationController>>();
    Mock<IAuthenticationService> mockAuthenticationService = new Mock<IAuthenticationService>();
    Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

    [TestInitialize]
    public void BeforeTest()
    {
        mockConfiguration = new Mock<IConfiguration>();
        mockLogger = new Mock<ILogger<UserAuthenticationController>>();
        mockAuthenticationService = new Mock<IAuthenticationService>();
        mockJwtService = new Mock<IJwtService>();
    }

    [TestMethod()]
    public void UserAuthenticationControllerTest()
    {
        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        Assert.IsNotNull(controller);
    }

    [TestMethod()]
    public async Task AuthenticateUnauthorizedAsyncTest()
    {
        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        IActionResult? result = await controller.AuthenticateAsync(new LoginRequest());

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }

    [TestMethod()]
    public async Task AuthenticateAsyncTest()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Username = "validUsername",
            Password = "validPassword"
        };
        User user = new User
        {
        };

        OperationResult<UserAccount> op = OperationResult<UserAccount>.Ok(AuthenticationService.On.RECORD_FOUND, user);

        mockAuthenticationService.Setup(m => m.GetValidUserAsync(loginRequest)).ReturnsAsync(op);

        mockJwtService.Setup(m => m.CreateToken(It.IsAny<List<Claim>>())).Returns(new JwtSecurityToken());

        mockJwtService.Setup(m => m.ToCompactSerializationFormat(It.IsAny<JwtSecurityToken>())).Returns("mockJwtTokenString");

        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        IActionResult? result = await controller.AuthenticateAsync(loginRequest);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.IsNotNull(((OkObjectResult)result).Value);
        Assert.IsInstanceOfType(((OkObjectResult)result).Value, typeof(LoginResponse));
    }


    [TestMethod()]
    public async Task AuthenticateGetValidUserFailAsyncTest()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Username = "validUsername",
            Password = "validPassword"
        };
        User user = new User
        {
        };

        OperationResult<UserAccount> op = OperationResult<UserAccount>.Fail();

        mockAuthenticationService.Setup(m => m.GetValidUserAsync(loginRequest)).ReturnsAsync(op);

        mockJwtService.Setup(m => m.CreateToken(It.IsAny<List<Claim>>())).Returns(new JwtSecurityToken());

        mockJwtService.Setup(m => m.ToCompactSerializationFormat(It.IsAny<JwtSecurityToken>())).Returns("mockJwtTokenString");

        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        IActionResult? result = await controller.AuthenticateAsync(loginRequest);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }


    [TestMethod()]
    public async Task AuthenticateGetValidUserNoPayloadAsyncTest()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Username = "validUsername",
            Password = "validPassword"
        };
        User user = new User
        {
        };

        OperationResult<UserAccount> op = OperationResult<UserAccount>.Ok(AuthenticationService.On.RECORD_FOUND, null);

        mockAuthenticationService.Setup(m => m.GetValidUserAsync(loginRequest)).ReturnsAsync(op);

        mockJwtService.Setup(m => m.CreateToken(It.IsAny<List<Claim>>())).Returns(new JwtSecurityToken());

        mockJwtService.Setup(m => m.ToCompactSerializationFormat(It.IsAny<JwtSecurityToken>())).Returns("mockJwtTokenString");

        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        IActionResult? result = await controller.AuthenticateAsync(loginRequest);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }


    [TestMethod()]
    public void GetRolesForDevTest()
    {
        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        var result = controller.GetRoles("dev");

        Assert.AreEqual(3, result.Length);
        Assert.IsTrue(result.Contains("Administrator"));
        Assert.IsTrue(result.Contains("Developer"));
        Assert.IsTrue(result.Contains("MyProfile"));
    }

    [TestMethod()]
    public void GetRolesDefaultTest()
    {
        UserAuthenticationController controller = new UserAuthenticationController(
            mockLogger.Object,
            mockConfiguration.Object,
            mockAuthenticationService.Object,
            mockJwtService.Object);

        string[]? result = controller.GetRoles(string.Empty);

        Assert.AreEqual(1, result.Length);
        Assert.IsTrue(result.Contains("MyProfile"));
    }
}
