namespace MiniTools.Web.Options;

public class ApplicationSettings
{  
   public string Name { get; set; } = string.Empty;

   public string Version { get; set; } = string.Empty;

   public ApplicationSettings()
   {
   }

   public ApplicationSettings(IConfigurationSection configurationSection)
   {
      Name = configurationSection.GetValue<string>("Name");
      Version = configurationSection.GetValue<string>("Version");
   }
}  
