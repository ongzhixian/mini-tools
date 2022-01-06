//using Microsoft.Extensions.Options;
//using MongoDB.Bson;
//using MongoDB.Driver;

//namespace MiniTools.Web.MongoEntities
//{
//    public interface IMiniToolsMongoDbContext
//    {
//        IMongoCollection<T> GetCollection<T>(string name);
//    }

//    public class MiniToolsMongoDbContext : IMiniToolsMongoDbContext
//    {
//        private IMongoDatabase _db { get; set; }
        
//        private MongoClient _mongoClient { get; set; }
        
//        public IClientSessionHandle Session { get; set; }
        
//        public MiniToolsMongoDbContext(
//            //IOptions<Mongosettings> configuration
//            )
//        {
//            //_mongoClient = new MongoClient(configuration.Value.Connection);
//            //_db = _mongoClient.GetDatabase(configuration.Value.DatabaseName);
//        }

//        public IMongoCollection<T> GetCollection<T>(string name)
//        {
//            return _db.GetCollection<T>(name);
//        }
//    }

//    public interface IBaseRepository<TEntity> where TEntity : class
//    {
//        Task Create(TEntity obj);
//        void Update(TEntity obj);
//        void Delete(string id);
//        Task<TEntity> Get(string id);
//        Task<IEnumerable<TEntity>> Get();
//    }

//    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
//    {
//        protected readonly IMiniToolsMongoDbContext _mongoContext;
//        protected IMongoCollection<TEntity> _dbCollection;

//        protected BaseRepository(IMiniToolsMongoDbContext context)
//        {
//            _mongoContext = context;
//            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
//        }

//        public async Task Create(TEntity obj)
//        {
//            if (obj == null)
//            {
//                throw new ArgumentNullException(typeof(TEntity).Name + " object is null");
//            }
//            await _dbCollection.InsertOneAsync(obj);
//        }

//        public void Delete(string id)
//        {
//            //ex. 5dc1039a1521eaa36835e541

//            var objectId = new ObjectId(id);
//            _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId));

//        }

//        public async Task<TEntity> Get(string id)
//        {
//            //ex. 5dc1039a1521eaa36835e541

//            var objectId = new ObjectId(id);

//            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

//            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

//        }


//        public async Task<IEnumerable<TEntity>> Get()
//        {
//            var all = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
//            return await all.ToListAsync();
//        }

//        public virtual void Update(TEntity obj)
//        {
//            _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetId()), obj);
//        }
//    }
//}
