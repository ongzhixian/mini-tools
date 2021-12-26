using MiniTools.Web.MongoEntities;
using MongoDB.Driver;

namespace MiniTools.Web.Services
{
    public class UserCollectionService
    {
        // IMongoCollection<User>

        ILogger<UserCollectionService> logger;
        IMongoCollection<User> userCollection;

        public UserCollectionService(ILogger<UserCollectionService> logger, IMongoCollection<User> userCollection)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userCollection = userCollection;
        }

        public void AddUser(User userDocument)
        {
            userCollection.InsertOneAsync(userDocument);
        }

    }
}
