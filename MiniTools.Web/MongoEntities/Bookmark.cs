using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTools.Web.MongoEntities;

public class Bookmark
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("date_created")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [BsonElement("tags")]
    public string[] Tags { get; set; } = new string[] { };

    [BsonElement("count")]
    public int Count { get; set; } = 0;


    public Bookmark()
    {
    }
}
