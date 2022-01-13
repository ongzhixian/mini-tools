using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using MiniTools.Messages.Requests;
using MiniTools.Messages.Responses;
using MiniTools.Services;

namespace MiniTools.Web.Services;

[Authorize(AuthenticationSchemes = "Cookies,Bearer")]
//[AllowAnonymous]
public class Greeter : GreetService.GreetServiceBase
{
    private readonly ILogger<Greeter> _logger;
    public Greeter(ILogger<Greeter> logger)
    {
        _logger = logger;
    }

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloResponse
        {
            Message = "Hello " + request.Name
        });
    }

    public override async Task<HelloResponse> StreamingFromClient(
        IAsyncStreamReader<HelloRequest> requestStream,
        ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            HelloRequest? message = requestStream.Current;
            Console.WriteLine("Received message");
            // ...
        }

        return new HelloResponse
        {
            Message = "All done."
        };
    }
}
