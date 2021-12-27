using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MiniTools.Web.MongoEntities
{
    //public class User
    //{
    //    [JsonPropertyName("id")]
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string Id { get; set; } = string.Empty;

    //    [JsonPropertyName("username")]
    //    [BsonElement("username")]
    //    public string Username { get; set; } = string.Empty;

    //    [JsonPropertyName("password")]
    //    [BsonElement("password")]
    //    public string Password { get; set; } = string.Empty;

    //    [JsonPropertyName("status")]
    //    [BsonElement("status")]
    //    public byte Status { get; set; }


    //    public User()
    //    {
    //    }

    //    public User(AddUserRequest req)
    //    {
    //        this.Username = req.Username;
    //        this.Password = req.Password;
    //    }
    //}

    // Try using a metadata class to specify BsonElement attributes; Does not work
    //public class UserMongoDbMetadata
    //{
    //    [BsonElement("username")]
    //    public string Username { get; set; } = string.Empty;

    //    [BsonElement("password")]
    //    public string Password { get; set; } = string.Empty;

    //    [BsonElement("status")]
    //    public UserAccountStatus Status { get; set; } = UserAccountStatus.Inactive;

    //    [BsonElement("date_created")]
    //    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    //    [BsonElement("password_expiry_date")]
    //    public DateTime PasswordExpiryDate { get; set; } = DateTime.MaxValue;
    //}


    //[MetadataType(typeof(UserMongoDbMetadata))] // Does not work for MongoDb
    public class User : UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public User()
        {
        }

        public User(User copy) : base(copy)
        {
            Id = copy.Id;
        }

        public User(UserAccount userAccount) : base(userAccount)
        {
        }
    }
}
