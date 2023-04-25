using System.Security.Claims;
using Grpc.Core;
using GrpcDemo;

namespace GrpcServer;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Saying hello to {request.Name}");

        var user = context.GetHttpContext().User;
        var userName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unauthenticated";

        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name} ({userName})"
        });
    }
}
