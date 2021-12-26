using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MiniTools.Web.MongoEntities
{
    public class User
    {
        [JsonPropertyName("id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        [BsonElement("status")]
        public byte Status { get; set; }

    }
}
