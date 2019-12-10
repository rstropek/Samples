using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemo;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main()
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Create channel.
            // Represents long-lived connection to gRPC service.
            // The port number(5001) must match the port of the gRPC server.
            // Tip: In ASP.NET Core apps, use client factory (similar to
            //      IHttpClientFactory (see https://docs.microsoft.com/en-us/aspnet/core/grpc/clientfactory).
            var channel = GrpcChannel.ForAddress("http://localhost:8080");

            await UnaryCall(channel);

            // Tip: Reuse channel for gRPC calls, create multiple gRPC clients
            //      from it (including different types of clients).
            var mathClient = new MathGuru.MathGuruClient(channel);
            await ServerStreaming(mathClient);

            await BidirectionalStreaming(mathClient);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task UnaryCall(GrpcChannel channel)
        {
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
        }

        private static async Task ServerStreaming(MathGuru.MathGuruClient mathClient)
        {
            // Note usage of new using statement (C# 8)
            using var fibResult = mathClient.GetFibonacci(new FromTo() { From = 10, To = 50 });

            // Note usage of new streaming enumerable
            await foreach (var fib in fibResult.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(fib);
            }
        }

        private static async Task BidirectionalStreaming(MathGuru.MathGuruClient mathClient)
        {
            using var duplexStream = mathClient.GetFibonacciStepByStep();

            // Note that client processes streamed results from server asynchronously
            var receiverTask = Task.Run(async () =>
            {
                // Note that this task will end when the server has closed the response stream
                await foreach (var fib in duplexStream.ResponseStream.ReadAllAsync())
                {
                    switch (fib.PayloadCase)
                    {
                        case StepByStepResult.PayloadOneofCase.Error:
                            Console.Error.WriteLine($"ERROR: {fib.Error.Message}");
                            break;
                        case StepByStepResult.PayloadOneofCase.Result:
                            Console.WriteLine(fib.Result.Result);
                            if (fib.Result.Result == 34)
                            {
                                // Ok, we got the highest Fibonacci number we were looking for.
                                // We can close the request stream so the server can gracefully stop.
                                await duplexStream.RequestStream.CompleteAsync();
                            }
                            break;
                        default:
                            // This should never happen
                            throw new InvalidOperationException($"Unexpected payload {fib.PayloadCase}");
                    }
                }
            });

            // 1, 1, 2, 3, 5, 8, 13, 21, 34, 55
            await duplexStream.RequestStream.WriteAsync(new FromTo() { From = 1, To = 10 });

            // Note that the following request will result in an error
            // because From (5) is < previous To (10)
            await duplexStream.RequestStream.WriteAsync(new FromTo() { From = 5, To = 15 });

            // Request a meaningful result
            await duplexStream.RequestStream.WriteAsync(new FromTo() { From = 20, To = 40 });

            // Wait until the async processing task has ended
            await receiverTask;
        }
    }
}
