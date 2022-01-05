using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.DataEntities;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTools.Web.Services.Tests
{
    [TestClass()]
    public class UserCollectionServiceTests
    {

        Mock<ILogger<UserCollectionService>> mockLogger = new Mock<ILogger<UserCollectionService>>();

        Mock<IMongoCollection<User>> mockUserCollection = new Mock<IMongoCollection<User>>();

        [TestInitialize]
        public void BeforeTest()
        {
            mockLogger = new Mock<ILogger<UserCollectionService>>();
            mockUserCollection = new Mock<IMongoCollection<User>>();
        }

        [TestMethod()]
        public void UserCollectionServiceTest()
        {
            UserCollectionService service = new UserCollectionService(
                mockLogger.Object,
                mockUserCollection.Object
                );

            Assert.IsNotNull(service);
        }

        [TestMethod()]
        public async Task AddUserAsyncTestAsync()
        {
            mockUserCollection.Setup(m => m.InsertOneAsync(
                It.IsAny<User>(), 
                It.IsAny< InsertOneOptions >(),
                It.IsAny<CancellationToken>())
            ).Returns(Task.CompletedTask);

            UserCollectionService service = new UserCollectionService(
                mockLogger.Object,
                mockUserCollection.Object
                );
            
            UserAccount doc = new UserAccount()
            {
            };

            UserAccount? result = await service.AddUserAsync(doc);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetUserAccountListAsyncTestAsync()
        {
            // 
            Mock<IFindFluent<User, User>> result2 = new Mock<IFindFluent<User, User>>();
            // FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken 
            //mockUserCollection.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<FindOptions>())).Returns(result2.Object);

            Mock<IAsyncCursor<UserAccount>> result3 = new Mock<IAsyncCursor<UserAccount>>();
            result3.Setup(m => m.Current).Returns(new List<UserAccount>());
            result3.Setup(m => m.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            result3.Setup(m => m.MoveNext(It.IsAny<CancellationToken>())).Returns(true);

            var expectedEntities = new List<UserAccount>()
            {
                new UserAccount()
            };

            var mockCursor = new Mock<IAsyncCursor<UserAccount>>();
            mockCursor.Setup(_ => _.Current).Returns(expectedEntities); //<-- Note the entities here
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            //mockMongoCollectionAdapter
            //    .Setup(x => x.FindAsync(
            //            It.IsAny<Expression<Func<Entity, bool>>>(),
            //            null,
            //            It.IsAny<CancellationToken>()
            //        ))
            //    .ReturnsAsync(mockCursor.Object); //<-- return the cursor here.



            //.ReturnsAsync(new List<UserAccount>());

            //result3.Setup(m => m.Current).Returns(new List<UserAccount>());
            mockUserCollection.Setup(m => m.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, UserAccount>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(mockCursor.Object);

            UserCollectionService service = new UserCollectionService(
                mockLogger.Object,
                mockUserCollection.Object
                );

            List<User>? result = await service.GetUserAccountListAsync(0, 10, SortDirection.Ascending, "Username");

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetUserAccountListAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindUserByUsernameAsyncTest()
        {
            Assert.Fail();
        }
    }
}