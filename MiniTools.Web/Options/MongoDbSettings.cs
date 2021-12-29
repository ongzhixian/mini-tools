using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Options;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}  

// public class MongoDbOptions
// {
//     public const string MyConfig = "MongoDbOptions";

//     // [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$")]
//     // public string Key1 { get; set; }

//     // [Range(0, 1000,ErrorMessage = "Value for {0} must be between {1} and {2}.")]
//     // public int Key2 { get; set; }
//     // public int Key3 { get; set; }
// }