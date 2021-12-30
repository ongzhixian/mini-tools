using Serilog.Core;
using Serilog.Events;

namespace MiniTools.Web.Helpers;

// For console we want minimal but succinct output; we also don't want empty all the empty square brackets []

// Someone else propose doing DelimitedEnricher
// See: https://stackoverflow.com/questions/49090613/c-sharp-serilog-enrichers-leaving-blank-entries

//[15:10:08 WRN] [Microsoft.AspNetCore.Server.Kestrel] [] Overriding address(es) 'https://localhost:7026, http://localhost:5256'. Binding to endpoints defined via IConfiguration and/or UseKestrel() instead. []
//[15:10:08 INF] [Microsoft.Hosting.Lifetime][{ Id: 14, Name: "ListeningOnAddress" }] Now listening on: http://localhost:7000 []
//[15:10:08 INF] [Microsoft.Hosting.Lifetime] [{ Id: 14, Name: "ListeningOnAddress" }] Now listening on: https://localhost:7001 []
//[15:10:08 INF] [Microsoft.Hosting.Lifetime] [] Application started.Press Ctrl+C to shut down. []
//[15:10:08 INF] [Microsoft.Hosting.Lifetime] [] Hosting environment: Development[]
//[15:10:08 INF] [Microsoft.Hosting.Lifetime][] Content root path: D:\src\github\mini-tools\MiniTools.Web\ []
//[15:17:08 INF] [MiniTools.Web.Controllers.LoginController] [{ Id: 123, Name: "IndexAsync" }] 1111[0HMEBCHHOS1UR: 0000000F]
//[15:17:10 INF] [MiniTools.Web.Services.AuthenticationService][{ Id: 503, Name: "Invalid credential" }] { "Username": "dev", "Password": "asd", "$type": "LoginRequest"}[0HMEBCHHOS1UR:0000000F,0HMEBCHHOS1US:00000002]
//[15:17:10 INF] [MiniTools.Web.Api.UserAuthenticationController] [{ Id: 234, Name: "AuthAsync" }] 12345[0HMEBCHHOS1UR: 0000000F, 0HMEBCHHOS1US: 00000002]
//[15:17:10 INF][MiniTools.Web.Services.AuthenticationApiService][{ Id: 1, Name: "Credential validation failure." }] False[0HMEBCHHOS1UR: 0000000F]
//[15:17:10 INF][MiniTools.Web.Controllers.LoginController][{ Id: 1, Name: "MVC View" }] Login - Index { "Username": "dev", "Password": "asd", "$type": "LoginViewModel"} [0HMEBCHHOS1UR:0000000F]
public class EventSourceOrNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string logValue = string.Empty;

        if (logEvent.Properties.GetValueOrDefault("SourceContext") is ScalarValue scalarValue)
            logValue = scalarValue.ToString();

        if (logEvent.Properties.GetValueOrDefault("EventId") is StructureValue structureValue)
        {
            LogEventProperty? nameProperty = structureValue.Properties.FirstOrDefault(r => r.Name == "Name");

            if (nameProperty != null)
                logValue = nameProperty.Value.ToString();
        }

        logValue = logValue.Replace("\"", string.Empty);

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("EventSourceOrName", logValue));
    }
}

