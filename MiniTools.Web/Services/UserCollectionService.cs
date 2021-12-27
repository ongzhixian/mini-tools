using MiniTools.Web.DataEntities;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Options;
using MongoDB.Driver;

namespace MiniTools.Web.Services
{
    public interface IUserAccountService
    {
        void AddUserAccount(UserAccount userAccount);

        void RemoveUserAccount(string userAccountId);

        void UpdateUserAccount(UserAccount userAccount);

        void FindUserAccount(string username);
        
        void FindUserAccount(string firstName, string lastName);

        void GetUserAccount(string userAccountId);

        void GetUserAccountList();

        void GetUserAccountList(uint pageSize, uint page);
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

            internal static EventId GET_USER_ACCOUNT_LIST_ASYNC_ERROR = new EventId(501, "GetUserAccountListAsync Error");
            
        }

        readonly ILogger<UserCollectionService> logger;

        readonly IMongoCollection<User> userCollection;

        public UserCollectionService(ILogger<UserCollectionService> logger, IMongoCollection<User> userCollection)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.userCollection = userCollection;
        }

        public async Task<UserAccount> AddUserAsync(UserAccount userDocument)
        {
            try
            {
                User user = new User(userDocument);

                await userCollection.InsertOneAsync(user);

                logger.LogInformation(On.ADD_USER_ASYNC_SUCCESS, "user {@user}", user);

                return (UserAccount)user;
            }
            catch (Exception ex)
            {
                logger.LogError(On.ADD_USER_ASYNC_ERROR, ex, "userDocument {@userDocument}", userDocument);
                throw;
            }
        }

        public async Task<List<UserAccount>> GetUserAccountListAsync(DataPageOption option)
        {
            IList<SortDefinition<User>> sortDefinitions = new List<SortDefinition<User>>();

            foreach (var item in option.SortItems)
                if (item.SortDirection == Options.SortDirection.Ascending)
                    sortDefinitions.Add(Builders<User>.Sort.Ascending(item.FieldName));
                else
                    sortDefinitions.Add(Builders<User>.Sort.Descending(item.FieldName));


            var result = await userCollection
                .Find(Builders<User>.Filter.Empty)
                .Sort(Builders<User>.Sort.Combine(sortDefinitions))
                .Skip((option.Page - 1) * option.PageSize)
                .Limit(option.PageSize)
                .ToListAsync();

            return result.ToList<UserAccount>();
        }

        public async Task<List<User>> GetUserAccountListAsync(
            int page = 0, int pageSize = 10, MongoDB.Driver.SortDirection sortDirection = MongoDB.Driver.SortDirection.Ascending, string sortField = "Username")
        {

            
            try
            {
                SortDefinition<User> dataSort;

                if (sortDirection == MongoDB.Driver.SortDirection.Ascending)
                    dataSort = Builders<User>.Sort.Ascending(sortField);
                else
                    dataSort = Builders<User>.Sort.Descending(sortField);

                var result = await userCollection
                    .Find(Builders<User>.Filter.Empty)
                    .Sort(dataSort)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                //logger.LogError(On.GET_USER_ACCOUNT_LIST_ASYNC_ERROR, ex, "userDocument {@userDocument}", userDocument);
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
