# dotnet CLI

Target site: https://mini-tools.azurewebsites.net/

## Commands

dotnet new globaljson
dotnet new sln -n MiniTools
dotnet new mvc -n MiniTools.Web
dotnet sln .\MiniTools.sln add .\MiniTools.Web\
dotnet user-secrets init --project .\MiniTools.Web\

dotnet new console -n MiniTools.HostApp
dotnet sln .\MiniTools.sln add .\MiniTools.HostApp\
dotnet user-secrets --project .\MiniTools.HostApp\ init



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

dotnet add .\MiniTools.Web\ package Grpc.AspNetCore


dotnet add .\Benchmarks\ package BenchmarkDotNet
dotnet add .\Benchmarks\ package BenchmarkDotNet.Diagnostics.Windows


-- For a host app, `Microsoft.Extensions.Hosting` will add the other packages (DI, Logging, Config, ..etc).

dotnet add .\MiniTools.HostApp\ package Microsoft.Extensions.Hosting

dotnet add .\MiniTools.HostApp\ package Microsoft.Extensions.Configuration.UserSecrets

dotnet add .\MiniTools.HostApp\ package Azure.Storage.Queues
dotnet add .\MiniTools.HostApp\ package Azure.Messaging.WebPubSub

dotnet add .\MiniTools.HostApp\ package Azure.Data.Tables
Azure.Storage.Blobs


dotnet add .\MiniTools.HostApp\ package Grpc.Net.Client
dotnet add .\MiniTools.HostApp\ package Google.Protobuf
dotnet add .\MiniTools.HostApp\ package Grpc.Tools
dotnet add .\MiniTools.HostApp\ package Microsoft.AspNetCore.SignalR.Client

dotnet add .\MiniTools.HostApp\ package Google.OrTools
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.Probabilistic
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.Probabilistic.Compiler
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.Probabilistic.Learners

dotnet add .\MiniTools.HostApp\ package Microsoft.ML
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.TimeSeries
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.Recommender
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.AutoML

// Yes this got to be version 2.3.1; Latest version 2.7.0 will break :-(
dotnet add .\MiniTools.HostApp\ package SciSharp.TensorFlow.Redist --version 2.3.1


dotnet add .\MiniTools.HostApp\ package System.Data.SqlClient

Microsoft.ML.FastTree
Microsoft.ML.Vision
Microsoft.ML.ImageAnalytics
Microsoft.ML.SampleUtils

Needed to save to Onnx format
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.OnnxConverter 
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.OnnxTransformer 
dotnet add .\MiniTools.HostApp\ package Microsoft.ML.OnnxRuntime

Microsoft.Data.Analysis
Microsoft.ML


Microsoft.ML.Probabilistic (Infer.net)
Microsoft.Z3

dotnet add .\MiniTools.HostApp\ package Microsoft.Spark

dotnet add .\MiniTools.HostApp\ package Confluent.Kafka

Yarp.ReverseProxy 
 

// Add to HostApp and WebApp if we want to use MessagePack
Microsoft.AspNetCore.SignalR.Protocols.MessagePack
See: https://docs.microsoft.com/en-us/aspnet/core/signalr/messagepackhubprotocol?view=aspnetcore-6.0


## Templates

dotnet new -i SpecFlow.Templates.DotNet
dotnet new -i BenchmarkDotNet.Templates

## Tools

dotnet tool install --global Microsoft.Playwright.CLI
dotnet tool install --global SpecFlow.Plus.LivingDoc.CLI
dotnet tool install JetBrains.dotCover.GlobalTool -g
dotnet tool install -g JetBrains.ReSharper.GlobalTools
dotnet tool install -g dotnet-grpc
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
dotnet tool install -g mlnet
dotnet tool install --global Apache.Avro.Tools

### cleanupcode

```tldr;

jb cleanupcode  .\MiniTools.Web\

jb cleanupcode  --include=DemoController.cs .\MiniTools.Web\

jb cleanupcode  .\MiniTools.Web\ --include=**/Controllers/*.cs

jb cleanupcode --profile="Built-in: Reformat Code" --include=DemoController.cs .\MiniTools.Web\

jb cleanupcode YourSolution.sln

jb cleanupcode --help 

jb cleanupcode --config-create=cleanupcode.xml
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

jb inspectcode --config-create=inspectcode.xml
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

dotnet dotcover test  --dcReportType=HTML --dcOutput=dotcover.html .\MiniTools.Web.UnitTests\

dotnet watch dotcover test  --dcReportType=HTML --dcOutput=dotcover.html  --project .\MiniTools.Web.UnitTests\


# Testing

Unit tests          -- test discrete unit of code
Functional tests    -- test a story (feature)
End-to-end tests    -- test a UI step

dotnet new mstest -n MiniTools.Web.UnitTests
dotnet add .\MiniTools.Web.UnitTests\MiniTools.Web.UnitTests.csproj package Moq --version 4.16.1

dotnet add package Moq --version 4.16.1

mkdir MiniTools.Web.FunctionTests
dotnet new specflowproject -n MiniTools.Web.FunctionTests --framework net6.0 --unittestprovider mstest
--Yeah, its weird, but not sure why specflowproject does not include MSTest.TestFramework
dotnet add .\MiniTools.Web.FunctionTests package MSTest.TestFramework


## Specflow livingdoc

C:\src\github.com\ongzhixian\mini-tools\MiniTools.Web.FunctionTests\bin\Debug\net6.0\MiniTools.Web.FunctionTests.dll

livingdoc test-assembly C:\src\github.com\ongzhixian\mini-tools\MiniTools.Web.FunctionTests\bin\Debug\net6.0\MiniTools.Web.FunctionTests.dll -t C:\src\github.com\ongzhixian\mini-tools\MiniTools.Web.FunctionTests\bin\Debug\net6.0\TestExecution.json 

## E2E using Playwright

-- Installation
dotnet new mstest -n MiniTools.Web.E2eTests
dotnet add .\MiniTools.Web.E2eTests\ package Microsoft.Playwright
dotnet add .\MiniTools.Web.E2eTests\ package Microsoft.Playwright.MSTest
dotnet build .\MiniTools.Web.E2eTests\
playwright install chromium
playwright install msedge
playwright install chrome

-- Create tests
playwright codegen --channel msedge https://localhost:7241/
playwright codegen http://localhost

playwright codegen https://localhost:7001/Login

dotnet test .\MiniTools.Web.E2eTests\

dotnet test --filter TestCategory!=e2e

dotnet test --filter FullyQualifiedName!~E2eTests

dotnet test --filter FullyQualifiedName!~AspNetCoreGeneratedDocument.*

dotnet dotcover test  --dcReportType=HTML --dcOutput=ignore/dotcover.html .\MiniTools.Web.UnitTests\

dotnet dotcover test --dcAttributeFilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute --dcFilters="-:type=AspNetCoreGeneratedDocument.*;-:type=Program" --dcReportType=HTML --dcOutput=ignore/dotcover.html .\MiniTools.Web.UnitTests\


-- Using XML config file systax; not working; need to investigate
-- Problem: When using XML config file, dotcover will ignore the `.\MiniTools.Web.UnitTests\` parameters 
   and cover whatever it can find in the current directory.
   A less than ideal workaround is to navigate to the directory where the tests are located and run dotcover.

dotnet dotcover test --dcXml=dotcover.xml  .\MiniTools.Web.UnitTests\
dotnet dotcover test --help 


Operators:
=   exact match
!=  not exact match
~   contains
!~  doesn't contain

Properties:
FullyQualifiedName
Name
ClassName
Priority
TestCategory


Available browsers: chromium, chrome, chrome-beta, msedge, msedge-beta, msedge-dev, firefox, webkit
Note: Its probably a good idea to install the browser manually before `playwright install <browser>`
Note: The default use "chromium"; to use another browser as default, we can set a environment variable `BROWSER`.
      But it does not seems to work well with `msedge`
      See: https://playwright.dev/dotnet/docs/test-runners
      See: https://playwright.dev/dotnet/docs/browsers#when-to-use-google-chrome--microsoft-edge-and-when-not-to

```Does not work for now; 
$env:BROWSER="msedge"
dotnet test .\MiniTools.Web.E2eTests\
```

`playwright codegen https://localhost:7001/Login`

To codegen with authentication:

1.  Perform authentication and exit; auth.json will contain the storage state.  
`playwright codegen --save-storage=auth.json https://localhost:7001/Login`
2.  Load the storage state from the file and open web site in code gen mode.
`playwright codegen --load-storage=auth.json https://localhost:7001/Login`
--OR (without not using codegen)--
`playwright open --load-storage=auth.json https://localhost:7001/Login`

[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.ClassLevel)]
[DoNotParallelize()]

```xml:Sample `test.runsettings` file
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <!-- MSTest adapter -->
    <MSTest>
        <Parallelize>
            <Workers>4</Workers>
            <Scope>ClassLevel</Scope>
        </Parallelize>
    </MSTest>
</RunSettings>
```

Alternatives:
https://docs.microsoft.com/en-us/microsoft-edge/test-and-automation/devtools-protocol
https://docs.microsoft.com/en-us/microsoft-edge/puppeteer/
https://docs.microsoft.com/en-us/microsoft-edge/webdriver-chromium/?tabs=c-sharp
https://webhint.io/

