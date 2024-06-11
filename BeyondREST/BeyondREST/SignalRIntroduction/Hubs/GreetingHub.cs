using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRIntroduction;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRDrawingServer.Hubs;

public class GreetingHub(ILogger<GreetingHub> logger, MathAlgorithms mathAlgorithms) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("New client, saying hello");

        await Clients.Caller.SendAsync("sayHello", "Hello from Server");
    }

    public void SayHello(string message)
    {
        logger.LogInformation("Received hello with message '{0}'", message);
    }

    public async Task StreamSayHello(ChannelReader<string> messageStream)
    {
        while (await messageStream.WaitToReadAsync())
        {
            while (messageStream.TryRead(out var item))
            {
                // do something with the stream item
                logger.LogInformation("Received hello VIA STREAM with message '{0}'", item);
            }
        }
    }

    public async IAsyncEnumerable<int> GetFibonacci(int from, int to)
    {
        // Check input parameters
        if (from > to)
        {
            throw new ArgumentException();
        }

        foreach (var current in mathAlgorithms.GetFibonacci())
        {
            if (current < from)
            {
                // Fibonacci number is too low
                continue;
            }
            else if (current <= to)
            {
                // We got a Fibonacci number that the client is interested in
                yield return current;

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
}
