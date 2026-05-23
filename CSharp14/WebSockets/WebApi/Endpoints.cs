using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApi;

public static class WebSocketEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapWebSocketEndpoints()
        {
            app.Map("/ws", async (HttpContext context, ApplicationDataContext dbContext) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    // Note the use of WebSocketStream. The stream abstraction on top of WebSocket
                    // is brand new in .NET 10/C# 14 and makes working with WebSockets much easier.
                    // It allows you to use StreamReader/StreamWriter or any other stream-based API
                    // on top of WebSockets.
                    using var stream = WebSocketStream.Create(webSocket, WebSocketMessageType.Text, true);
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    string? message;
                    while ((message = await reader.ReadLineAsync()) != null)
                    {
                        dbContext.Messages.Add(new Message { Text = message });
                        await dbContext.SaveChangesAsync();

                        // Here we are using System.Text.Json to serialize a JSON object.
                        // We make use of the new WebSocketStream to write directly to the WebSocket.
                        await JsonSerializer.SerializeAsync(stream, new { echo = message });
                        if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });

            return app;
        }
    }
}