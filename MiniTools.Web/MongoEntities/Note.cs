using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTools.Web.MongoEntities;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("date_created")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [BsonElement("tags")]
    public string[] Tags { get; set; } = new string[] { };


    public Note()
    {
    }
}
