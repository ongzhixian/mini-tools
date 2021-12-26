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

        public async Task AddUserAsync(User userDocument)
        {
            await userCollection.InsertOneAsync(userDocument);
        }

        public async Task<User> GetUserAsync(string id)
        {
            return await this.userCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }


        public async Task<List<User>> FindUserAsync(string searchClause)
        {
            return await userCollection.Find(r =>
                r.Username.Contains(searchClause, StringComparison.InvariantCultureIgnoreCase)
            ).ToListAsync();
        }

        public async Task<UpdateResult> UpdateUserAsync(string id)
        {
            UpdateDefinition<User>? update = Builders<User>.Update.Set(x => x.Status, 1);

            return await userCollection.UpdateOneAsync<User>(r => r.Id == id, update);
        }

        public async Task<UpdateResult> RemoveUserAsync(string id)
        {
            UpdateDefinition<User>? update = Builders<User>.Update.Set(x => x.Status, 1);

            return await userCollection.UpdateOneAsync<User>(r => r.Id == id, update);
        }
    }
}
