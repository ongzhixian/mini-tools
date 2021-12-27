using MiniTools.Web.DataEntities;
using MiniTools.Web.MongoEntities;
using MongoDB.Driver;

namespace MiniTools.Web.Services
{
    public interface IUserAccountService
    {
        public void AddUserAccount(UserAccount userAccount);

        public void RemoveUserAccount(string userAccountId);

        public void UpdateUserAccount(UserAccount userAccount);

        public void FindUserAccount(string username);
        
        public void FindUserAccount(string firstName, string lastName);

        public void GetUserAccount(string userAccountId);

        public void GetUserAccountList();

        public void GetUserAccountList(uint pageSize, uint page);
    }

    //public interface IUserCollectionService
    //{
    //    Task<User> AddUserAsync(UserAccount userDocument);
    //    Task<User> GetUserAsync(string id);
    //    Task<UpdateResult> RemoveUserAsync(string id);
    //    Task<UpdateResult> UpdateUserAsync(string id);
    //}

    public class UserCollectionService 
    {
        private class On
        {
            internal static EventId NEW = new EventId(1, "New");
            internal static EventId VIEW_HOME = new EventId(101, "View home");

            internal static EventId SUCCESS = new EventId(200, "Success"); // Say no to generic success?
            internal static EventId ADD_USER_ASYNC_SUCCESS = new EventId(201, "AddUserAsync Success");


            internal static EventId ERROR = new EventId(500, "Error"); // Say no to generic errors?
            internal static EventId ADD_USER_ASYNC_ERROR = new EventId(501, "AddUserAsync Error");
        }

        readonly ILogger<UserCollectionService> logger;

        readonly IMongoCollection<User> userCollection;

        public UserCollectionService(ILogger<UserCollectionService> logger, IMongoCollection<User> userCollection)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.userCollection = userCollection;
        }

        public async Task<User> AddUserAsync(UserAccount userDocument)
        {
            try
            {
                User user = new User(userDocument);

                await userCollection.InsertOneAsync(user);

                logger.LogInformation(On.ADD_USER_ASYNC_SUCCESS, "user {@user}", user);

                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(On.ADD_USER_ASYNC_ERROR, ex, "userDocument {@userDocument}", userDocument);
                throw;
            }
        }



        public async Task<User> GetUserAsync(string id)
        {
            return await this.userCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }


        //public async Task<List<UserAccount>> FindUserAsync(string searchClause)
        //{
        //    return await userCollection.Find(r =>
        //        r.Username.Contains(searchClause, StringComparison.InvariantCultureIgnoreCase)
        //    ).ToListAsync();
        //}

        //public async Task<UpdateResult> UpdateUserAsync(string id)
        //{
        //    UpdateDefinition<User>? update = Builders<User>.Update.Set(x => x.Status, 1);

        //    return await userCollection.UpdateOneAsync<User>(r => r.Id == id, update);
        //}

        //public async Task<UpdateResult> RemoveUserAsync(string id)
        //{
        //    UpdateDefinition<User>? update = Builders<User>.Update.Set(x => x.Status, 1);

        //    return await userCollection.UpdateOneAsync<User>(r => r.Id == id, update);
        //}
    }
}
