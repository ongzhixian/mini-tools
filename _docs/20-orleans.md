# Orleans


Microsoft.Orleans.Server
Microsoft.Orleans.Client

Microsoft.Orleans.Core.Abstractions
Microsoft.Orleans.CodeGenerator.MSBuild

Microsoft.Extensions.Logging.Console
Microsoft.Extensions.Logging.Abstractions


Silo 	            Microsoft.Orleans.Server
Silo 	            Microsoft.Extensions.Logging.Console

Client 	            Microsoft.Extensions.Logging.Console
Client 	            Microsoft.Orleans.Client

Grain Interfaces 	Microsoft.Orleans.Core.Abstractions
Grain Interfaces 	Microsoft.Orleans.CodeGenerator.MSBuild

Grains 	            Microsoft.Orleans.CodeGenerator.MSBuild
Grains 	            Microsoft.Orleans.Core.Abstractions
Grains 	            Microsoft.Extensions.Logging.Abstractions