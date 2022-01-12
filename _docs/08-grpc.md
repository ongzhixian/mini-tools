# GRPC

The main benefits of gRPC are:

1.  Modern?, high-performance, lightweight RPC framework.
2.  Contract-first API development, using Protocol Buffers by default, allowing for language agnostic implementations.
3.  Tooling available for many languages to generate strongly-typed servers and clients.
4.  Supports client, server, and bi-directional streaming calls.
5.  Reduced network usage with Protobuf binary serialization.

These benefits make gRPC ideal for:

1.  Lightweight microservices where efficiency is critical.
2.  Polyglot systems where multiple languages are required for development.
3.  Point-to-point real-time services that need to handle streaming requests or responses.



dotnet-grpc list --project .\MiniTools.HostApp\MiniTools.HostApp.csproj


# Packages

Grpc
Grpc.Tools
Google.Protobuf

Grpc.AspNetCore

dotnet tool install -g dotnet-grpc
https://docs.microsoft.com/en-us/aspnet/core/grpc/dotnet-grpc?view=aspnetcore-6.0

# Problems with GRPC

1.  No HTTP/2, no GRPC

2.  Client-Server architecture
    Only the client can initiate events. The server can only respond to those.

Pro GRPC

1. Much easier API versioning

2. Libraries available in pretty much any language

3. Easier to apply in microservice architecture


```
services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri("https://localhost:5001");
});
```


## Grpc-dotnet

https://blog.jetbrains.com/dotnet/2021/07/19/getting-started-with-asp-net-core-and-grpc/

https://www.telerik.com/blogs/introduction-to-grpc-dotnet-core-and-dotnet-5

## Older (deprecated Grpc.Code)

https://codelabs.developers.google.com/codelabs/cloud-grpc-csharp#1

https://github.com/grpc/grpc/tree/master/examples/csharp/Helloworld


## Remoting (channels)

https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/create-remote-server