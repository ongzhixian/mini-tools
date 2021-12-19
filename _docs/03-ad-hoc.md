# Ad-hoc

Ad-hoc notes.

## CLI Repeat warnings

tldr;

`dotnet build .\MiniTools.Web\ /clp:NoSummary`

Observation:

When you build a project (using `dotnet build .\MiniTools.Web\` for example) in command-line,
you might find that the same set of messages (warnings/errors) are repeated!

The repetition (that reports `Build <status:succeed|failure>`)  is part of the console logger's summary. 
This can be disabled by passing in `/clp:NoSummary` to msbuild. 

See: https://stackoverflow.com/questions/44582185/duplicate-error-messages-in-net-core-error-cs0116

## Feature

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/request-features?view=aspnetcore-6.0

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/extensibility?view=aspnetcore-6.0

https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.features?view=aspnetcore-6.0


## Filters

https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-6.0


TypeFilter vs ServiceFilter

When your ActionFilter implementation is itself registered as a Service, 
then we need to use the ServiceFilter attribute to invoke the Filter. 
Otherwise TypeFilter attribute would do the job for us.

## IOptions


			        Reload w/o app restart	Singleton	Named Options
IOptions<T>		    No			            Yes		    No
IOptionsSnaphot<T>	Yes			            No		    Yes
IOptionsMonitor<T>	Yes			            Yes		    Yes

https://www.c-sharpcorner.com/article/asp-net-core-accessing-configurations-using-named-options/


