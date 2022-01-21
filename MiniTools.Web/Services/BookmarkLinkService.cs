using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MongoDB.Driver;

namespace MiniTools.Web.Services;

public interface IBookmarkLinkService
{
    Task AddLinksAsync(string linkTextBlock);

    Task<long> CountTotalLinksAsync();

    Task<List<Bookmark>> GetBookmarkListAsync(
        int page = 0,
        int pageSize = 10,
        SortDirection sortDirection = SortDirection.Ascending,
        string sortField = "date_created");

    Task<List<Bookmark>> GetBookmarkListAsync();
    void AddTags(string id, string tagList);
    void RemoveTag(string id, string tag);

    //Task<PageData<Bookmark>?> GetBookmarkListAsync(Options.DataPageOption dataPageOption);
}

public class BookmarkLinkService : IBookmarkLinkService
{
    readonly ILogger<BookmarkLinkService> logger;

    readonly IMongoCollection<Bookmark> bookmarkCollection;

    public BookmarkLinkService(ILogger<BookmarkLinkService> logger, IMongoCollection<Bookmark> bookmarkCollection)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.bookmarkCollection = bookmarkCollection;
    }

    public async Task AddLinksAsync(string linkTextBlock)
    {
        List<Bookmark> bookmarks = new List<Bookmark>();
        
        using (var reader = new StringReader(linkTextBlock))
        {
            string? line;

            while (true)
            {
                line = reader.ReadLine();

                if (line == null)
                    break;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // KIV: To add some kind of auto tagging here via ML here ;-)

                bookmarks.Add(new Bookmark
                {
                    Url = line.Trim()
                });
            }
        }

        var bulkOps = new List<WriteModel<Bookmark>>();

        foreach (var item in bookmarks)
        {
            FilterDefinition<Bookmark>? filter = Builders<Bookmark>.Filter.Eq(r => r.Url, item.Url);
            UpdateDefinition<Bookmark>? update = Builders<Bookmark>.Update
                .Inc(r => r.Count, 1)
                .SetOnInsert(r => r.Tags, item.Tags)
                .SetOnInsert(r => r.DateCreated, item.DateCreated);

            var upsertOne = new UpdateOneModel<Bookmark>(filter, update)
            {
                IsUpsert = true
            };
            bulkOps.Add(upsertOne);
        }

        //BulkWriteResult<Bookmark>? writeResult =
        await bookmarkCollection.BulkWriteAsync(bulkOps);
    }

    public void AddTags(string id, string tagList)
    {
        var list = tagList.Split(",", StringSplitOptions.RemoveEmptyEntries);

        var filter = Builders<Bookmark>.Filter.Eq(r => r.Id, id);
        
        FieldDefinition<Bookmark> tagsField = "tags";

        var update = Builders<Bookmark>.Update.AddToSetEach(tagsField, list);

        bookmarkCollection.FindOneAndUpdate(filter, update);
    }

    public async Task<long> CountTotalLinksAsync()
    {
        return await bookmarkCollection.CountDocumentsAsync(Builders<Bookmark>.Filter.Empty);
        
    }

    public async Task<List<Bookmark>> GetBookmarkListAsync(
        int page = 0, 
        int pageSize = 10, 
        SortDirection sortDirection = SortDirection.Ascending, 
        string sortField = "date_created")
    {
        SortDefinition<Bookmark> dataSort;

        if (sortDirection == SortDirection.Ascending)
            dataSort = Builders<Bookmark>.Sort.Ascending(sortField);
        else
            dataSort = Builders<Bookmark>.Sort.Descending(sortField);

        var result = await bookmarkCollection
            .Find(Builders<Bookmark>.Filter.Empty)
            .Sort(dataSort)
            .Skip(page * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return result;
    }

    public async Task<List<Bookmark>> GetBookmarkListAsync()
    {
        IAsyncCursor<Bookmark>? result = await bookmarkCollection.FindAsync(Builders<Bookmark>.Filter.Empty);

        return await result.ToListAsync();
    }

    public void RemoveTag(string id, string tag)
    {
        
        var filter = Builders<Bookmark>.Filter.Eq(r => r.Id, id);

        FieldDefinition<Bookmark> tagsField = "tags";

        var update = Builders<Bookmark>.Update.Pull(tagsField, tag);

        bookmarkCollection.FindOneAndUpdate(filter, update);
    }
}
