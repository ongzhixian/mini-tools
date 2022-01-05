using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.MongoEntities;
using Moq;
using System.Threading.Tasks;

namespace MiniTools.Web.Services.Tests;

[TestClass()]
public class AuthenticationServiceTests
{
    Mock<ILogger<AuthenticationService>> mockLogger = new Mock<ILogger<AuthenticationService>>();
    Mock<IUserCollectionService> mockUserCollectionService = new Mock<IUserCollectionService>();

    [TestInitialize]
    public void BeforeTest()
    {
        mockLogger = new Mock<ILogger<AuthenticationService>>();
        mockUserCollectionService = new Mock<IUserCollectionService>();
    }

    [TestMethod()]
    public void AuthenticationServiceTest()
    {
        AuthenticationService service = new AuthenticationService(
            mockLogger.Object,
            mockUserCollectionService.Object
            );

        Assert.IsNotNull(service);
    }

    [TestMethod()]
    public async Task GetValidUserAsyncTestAsync()
    {
        LoginRequest model = new LoginRequest()
        {
            Username = "someUsername",
            Password = "somePassword"
        };

        mockUserCollectionService.Setup(m => m.FindUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User
        {
            Username = "someUsername",
            Password = "somePassword"
        });

        AuthenticationService service = new AuthenticationService(
            mockLogger.Object,
            mockUserCollectionService.Object
            );

        var result = await service.GetValidUserAsync(model);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Models.OperationResult<DataEntities.UserAccount>));
        Assert.IsTrue(result.Success);
    }

    [TestMethod()]
    public async Task GetValidUserAsyncEmptyModelTestAsync()
    {
        LoginRequest model = new LoginRequest();

        mockUserCollectionService.Setup(m => m.FindUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User
        {
            Username = "someUsername",
            Password = "somePassword"
        });

        AuthenticationService service = new AuthenticationService(
            mockLogger.Object,
            mockUserCollectionService.Object
            );

        var result = await service.GetValidUserAsync(model);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Models.OperationResult<DataEntities.UserAccount>));
        Assert.IsFalse(result.Success);
    }


    [TestMethod()]
    public async Task GetValidUserAsyncNoRecordTestAsync()
    {
        LoginRequest model = new LoginRequest()
        {
            Username = "someUsername",
            Password = "somePassword"
        };

        mockUserCollectionService.Setup(m => m.FindUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        AuthenticationService service = new AuthenticationService(
            mockLogger.Object,
            mockUserCollectionService.Object
            );

        var result = await service.GetValidUserAsync(model);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Models.OperationResult<DataEntities.UserAccount>));
        Assert.IsFalse(result.Success);
    }

    [TestMethod()]
    public async Task GetValidUserAsyncInvalidPasswordTestAsync()
    {
        LoginRequest model = new LoginRequest()
        {
            Username = "someUsername",
            Password = "somePassword"
        };

        mockUserCollectionService.Setup(m => m.FindUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User
        {
            Username = "someUsername",
            Password = "somePasswordX"
        });

        AuthenticationService service = new AuthenticationService(
            mockLogger.Object,
            mockUserCollectionService.Object
            );

        var result = await service.GetValidUserAsync(model);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Models.OperationResult<DataEntities.UserAccount>));
        Assert.IsFalse(result.Success);
    }
}
