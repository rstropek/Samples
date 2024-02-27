using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SignalRDrawingServer.Hubs;

public class DrawingHub(ILogger<DrawingHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("New client connected");

        // Note that we can store connection-specific data in the Context object
        var rand = new Random();
        var color = $"rgb({rand.Next(255)},{rand.Next(255)},{rand.Next(255)})";
        Context.Items["color"] = color;

        // Send caller a random color
        await Clients.Caller.SendAsync("setColor", color);
    }

    public async Task Draw(Point previousPoint, Point newPoint)
    {
        // Send other clients that caller has drawn something
        await Clients.Others.SendAsync("draw", previousPoint, newPoint, Context.Items["color"]);
    }
}
