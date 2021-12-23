# dotnet CLI

Target site: https://mini-tools.azurewebsites.net/

## Commands

dotnet new globaljson
dotnet new sln -n MiniTools
dotnet new mvc -n MiniTools.Web
dotnet sln .\MiniTools.sln add .\MiniTools.Web\
dotnet user-secrets init --project .\MiniTools.Web\

## User-Secrets

The values are stored in a JSON file in the local machine's user profile folder:

 `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`

dotnet user-secrets set <some-key> <some-value> --project .\MiniTools.Web\

dotnet user-secrets set "Movies:ServiceApiKey" "someVal12345" --project .\MiniTools.Web\

```json
"Jwt": {
    "ValidAudience": "https://localhost:5001/",
    "ValidIssuer": "https://localhost:5001/",
    "Secret": "secret09172301287"
}
```

dotnet user-secrets set "Jwt:SecretKey" "secret09172301287" --project .\MiniTools.Web\
dotnet user-secrets set "Jwt:ValidAudience" "https://localhost:7001/" --project .\MiniTools.Web\
dotnet user-secrets set "Jwt:ValidIssuer" "https://localhost:7001/" --project .\MiniTools.Web\


## Packages

dotnet add .\MiniTools.Web\ package nnn

dotnet add .\MiniTools.Web\ package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add .\MiniTools.Web\ package Serilog
dotnet add .\MiniTools.Web\ package Serilog.Extensions.Hosting
dotnet add .\MiniTools.Web\ package Serilog.Formatting.Compact
dotnet add .\MiniTools.Web\ package Serilog.Settings.Configuration
dotnet add .\MiniTools.Web\ package Serilog.Sinks.Console
dotnet add .\MiniTools.Web\ package Serilog.Sinks.File
dotnet add .\MiniTools.Web\ package Swashbuckle.AspNetCore
dotnet add .\MiniTools.Web\ package MongoDB.Driver


dotnet add .\Benchmarks\ package BenchmarkDotNet
dotnet add .\Benchmarks\ package BenchmarkDotNet.Diagnostics.Windows
