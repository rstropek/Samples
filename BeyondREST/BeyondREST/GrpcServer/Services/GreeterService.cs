using System.Security.Claims;
using Grpc.Core;
using GrpcDemo;

namespace GrpcServer;

public class GreeterService(ILogger<GreeterService> logger) : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        logger.LogInformation($"Saying hello to {request.Name}");

        var user = context.GetHttpContext().User;
        var userName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            ?? user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? "Unauthenticated";

        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name} ({userName})"
        });
    }
}
