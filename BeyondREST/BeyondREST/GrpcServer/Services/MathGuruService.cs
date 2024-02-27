using Grpc.Core;
using GrpcDemo;
using GrpcServer.Services;

namespace GrpcServer;

public class MathGuruService(MathAlgorithms math) : MathGuru.MathGuruBase
{
    public override async Task GetFibonacci(FromTo request, IServerStreamWriter<NumericResult> responseStream, ServerCallContext context)
    {
        // Check input parameters
        if (request.From > request.To)
        {
            // Note simple error indication with status code and message
            context.Status = new Status(StatusCode.InvalidArgument, "FromTo.From must be <= FromTo.To");
            return;
        }

        foreach (var current in math.GetFibonacci())
        {
            if (current < request.From)
            {
                // Fibonacci number is too low
                continue;
            }
            else if (current <= request.To)
            {
                // We got a Fibonacci number that the client is interested in
                await responseStream.WriteAsync(new NumericResult { Result = current });

                // Simulate some long running calculation
                await Task.Delay(250);
            }
            else
            {
                // Fibonacci number is too high
                break;
            }
        }
    }

    public override async Task GetFibonacciStepByStep(IAsyncStreamReader<FromTo> requestStream, IServerStreamWriter<StepByStepResult> responseStream, ServerCallContext context)
    {
        var fib = math.GetFibonacci().GetEnumerator();
        FromTo? previousFromTo = null;

        // Read requests from client
        await foreach (var item in requestStream.ReadAllAsync())
        {
            // Check if request is valid
            if (item.From > item.To)
            {
                // Note returning error details using a oneof field in ProtoBuf.
                // See also https://cloud.google.com/apis/design/errors#error_model for
                // details about Google.Rpc.Status and Google.Rpc.Code
                await responseStream.WriteAsync(new StepByStepResult
                {
                    Error = new Google.Rpc.Status
                    {
                        Code = (int)Google.Rpc.Code.InvalidArgument,
                        Message = "To must be >= From"
                    }
                });

                // Wait for next client request
                continue;
            }

            // Make sure new request is not < previous one
            if (previousFromTo != null && item.From < previousFromTo.To)
            {
                await responseStream.WriteAsync(new StepByStepResult
                {
                    Error = new Google.Rpc.Status
                    {
                        Code = (int)Google.Rpc.Code.InvalidArgument,
                        Message = "From must be >= previously sent To"
                    }
                });

                continue;
            }

            previousFromTo = item;

            // Calculate Fibi
            while (fib.Current < item.From)
            {
                fib.MoveNext();
            }

            if (fib.Current > item.To)
            {
                continue;
            }

            while (fib.Current <= item.To)
            {
                await responseStream.WriteAsync(new StepByStepResult
                {
                    Result = new NumericResult { Result = fib.Current }
                });
                fib.MoveNext();
                await Task.Delay(250);
            }
        }
    }
}
