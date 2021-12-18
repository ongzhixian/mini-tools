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