using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Api;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTools.Web.Api.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        //Mock<ILogger<LoginController>> mockLogger = new Mock<ILogger<LoginController>>();
        //Mock<IAuthenticationApiService> mockAuthenticationApiService = new Mock<IAuthenticationApiService>();
        //Mock<IJwtService> mockJwtService = new Mock<IJwtService>();

        Mock<ILogger<UserController>> mockLogger = new Mock<ILogger<UserController>>();
        Mock<IUserCollectionService> mockUserCollectionService = new Mock<IUserCollectionService>();

        [TestInitialize]
        public void BeforeTest()
        {
            mockLogger = new Mock<ILogger<UserController>>();
            mockUserCollectionService = new Mock<IUserCollectionService>();
        }

        [TestMethod()]
        public void UserControllerTest()
        {
            UserController controller = new UserController(
                mockLogger.Object,
                mockUserCollectionService.Object);

            Assert.IsNotNull(controller);
        }

        [TestMethod()]
        public async Task GetAsyncTestAsync()
        {
            PageData<UserAccount> data = new PageData<UserAccount>
            {
                DataList = new List<UserAccount>()
            };
            
            mockUserCollectionService.Setup(m => m.GetUserAccountListAsync(It.IsAny<Options.DataPageOption>())).ReturnsAsync(data);

            UserController controller = new UserController(
                mockLogger.Object,
                mockUserCollectionService.Object);

            IActionResult? result = await controller.GetAsync(1, 10);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(((OkObjectResult)result).Value, typeof(PageData<UserAccount>));
        }

        [TestMethod()]
        public async Task PostAsyncTestAsync()
        {
            AddUserRequest request = new AddUserRequest();

            UserAccount userAccount = new UserAccount();

            mockUserCollectionService.Setup(m => m.AddUserAsync(userAccount)).ReturnsAsync(userAccount);

            UserController controller = new UserController(
                mockLogger.Object,
                mockUserCollectionService.Object);

            var result = await controller.PostAsync(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsInstanceOfType(((CreatedAtActionResult)result).Value, typeof(UserAccount));
        }

        [TestMethod()]
        public async Task PostAsyncBadRequestTestAsync()
        {
            AddUserRequest request = new AddUserRequest();

            UserAccount userAccount = new UserAccount();

            mockUserCollectionService.Setup(m => m.AddUserAsync(userAccount)).ReturnsAsync(userAccount);

            UserController controller = new UserController(
                mockLogger.Object,
                mockUserCollectionService.Object);
            controller.ModelState.AddModelError("mockError", "someMockErrorMessage");

            var result = await controller.PostAsync(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod()]
        public async Task PostAsyncExceptionTestAsync()
        {
            AddUserRequest request = new AddUserRequest();

            UserAccount userAccount = new UserAccount();

            mockUserCollectionService.Setup(m => m.AddUserAsync(It.IsAny<UserAccount>())).ThrowsAsync(new Exception());

            UserController controller = new UserController(
                mockLogger.Object,
                mockUserCollectionService.Object);

            var result = await controller.PostAsync(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ProblemDetails));
        }
    }
}