using Grpc.Core;
using MiniTools.GrpcServices;

namespace MiniTools.Web.Services;

public class Greeter : GreetService.GreetServiceBase
{
    private readonly ILogger<Greeter> _logger;
    public Greeter(ILogger<Greeter> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
