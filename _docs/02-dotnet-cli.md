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


## Templates

dotnet new -i SpecFlow.Templates.DotNet
dotnet new -i BenchmarkDotNet.Templates

## Tools

dotnet tool install --global Microsoft.Playwright.CLI
dotnet tool install --global SpecFlow.Plus.LivingDoc.CLI
dotnet tool install JetBrains.dotCover.GlobalTool -g
dotnet tool install -g JetBrains.ReSharper.GlobalTools


### cleanupcode

```tldr;

jb cleanupcode  .\MiniTools.Web\

jb cleanupcode  --include=DemoController.cs .\MiniTools.Web\

jb cleanupcode  .\MiniTools.Web\ --include=**/Controllers/*.cs

jb cleanupcode --profile="Built-in: Reformat Code" --include=DemoController.cs .\MiniTools.Web\

jb cleanupcode YourSolution.sln

jb cleanupcode --help 
```

Note: Use --include/--exclude to filter files. 
      Changes made to all files in project and solution can be scary.

The 3 built-in profiles:

`Built-in: Reformat Code`                   that only applies code formatting preferences
`Built-in: Reformat & Apply Syntax Style`   that applies code formatting preferences and code syntax styles.
`Built-in: Full Cleanup`                    that applies all available cleanup tasks except updating file header.


## inspectcode

```tldr;
jb inspectcode --build --output=inspectcode-result.html --format=Html --include=**/Controllers/*.cs .\MiniTools.Web\MiniTools.Web.csproj

jb inspectcode --build --output=inspectcode-result.html --format=Html .\MiniTools.Web\MiniTools.Web.csproj

jb inspectcode --help
```

## dotcover

dotnet dotcover test  --dcReportType=HTML --dcOutput=dotcover.html  .\Dn6Poc.DocuMgmtPortal.Tests\

dotnet watch dotcover test  --dcReportType=HTML --dcOutput=dotcover.html  --project .\Dn6Poc.DocuMgmtPortal.Tests\




# Use the below when resolving file-by-file

jb cleanupcode  --include=DemoController.cs .\MiniTools.Web\

jb inspectcode --build --output=ignore/inspectcode-result.html --format=Html --include=**/Controllers/*.cs .\MiniTools.Web\MiniTools.Web.csproj

jb inspectcode --build --output=ignore/full-inspectcode-result.html --format=Html --exclude=wwwroot/css/bootswatch/** .\MiniTools.Web\MiniTools.Web.csproj

jb inspectcode --build --output=ignore/full-inspectcode-result.html --format=Html --exclude=wwwroot/css/bootswatch/** --exclude=Helpers/HttpMessageLogging/** .\MiniTools.Web\MiniTools.Web.csproj

# Use the below when resolving on project level

jb cleanupcode .\MiniTools.Web\

jb inspectcode --build --output=inspectcode-result.html --format=Html .\MiniTools.Web\MiniTools.Web.csproj

# test coverage

dotnet dotcover test  --dcReportType=HTML --dcOutput=dotcover.html  .\Dn6Poc.DocuMgmtPortal.Tests\

dotnet watch dotcover test  --dcReportType=HTML --dcOutput=dotcover.html  --project .\Dn6Poc.DocuMgmtPortal.Tests\


# Testing

Unit tests          -- test discrete unit of code
Functional tests    -- test a story (feature)
End-to-end tests    -- test a UI step

dotnet new mstest -n MiniTools.Web.UnitTests

mkdir MiniTools.Web.FunctionTests
dotnet new specflowproject -n MiniTools.Web.FunctionTests

dotnet new mstest -n MiniTools.Web.E2eTests
dotnet add .\MiniTools.Web.E2eTests\ package Microsoft.Playwright

