using AsyncBlazor.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsyncBlazor.Client.Pages
{
    public partial class Index
    {
        private HubConnection? hubConnection;
        private string Token { get; set; } = string.Empty;
        private List<string> NotificationMessages { get; } = new();

        [Inject]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private HttpClient HttpClient { get; set; }

        [Inject]
        private AuthenticationService Auth { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


        private async Task LoginAsync(string username)
        {
            // Get token for user. In this sample, real-world authentication is not in scope.
            // Therefore, a symmetrically signed JWT with dummy secret, issuer and audience is used.
            // In real world, use OpenID Connect ideally with Azure Active Directory for acquiring
            // tokens.
            Token = await Auth.AcquireTokenAsync(username);

            // Get SignalR connection info (URL and access token)
            var signalRConn = await GetSignalRConnectionAsync();

            // Build hub connection with URL and access token from
            // negotiate endpoint.
            hubConnection = new HubConnectionBuilder()
                .WithUrl(signalRConn.Url, 
                    conn => conn.AccessTokenProvider = () => Task.FromResult(signalRConn.AccessToken))
                .Build();

            // In practice, you should probably add some auto-reconnect logic here.
            // hubConnection.Closed += ...

            // Handle order processed event
            hubConnection.On<Order>("OrderProcessed", order =>
            {
                // Display information message that order has been processed
                NotificationMessages.Add($"Received async result for order");
                StateHasChanged();
            });

            // Start hub connection
            await hubConnection.StartAsync();
        }

        private async Task LogoutAsync()
        {
            // Forget token, clean messages, close SignalR connection
            Token = string.Empty;
            NotificationMessages.Clear();
            if (hubConnection != null)
            {
                await hubConnection.StopAsync();
            }

            hubConnection = null;
        }

        private async Task SendOrder()
        {
            // Build dummy order
            var order = new Order { CustomerID = "Foo Bar", Product = "Something", Amount = 42 };

            // Post dummy order to web API
            NotificationMessages.Add("Sending order to server for processing");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(order)),
                RequestUri = new Uri("http://localhost:7071/api/Orders")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            NotificationMessages.Add("Order sent, async result pending");
        }

        private async Task SayHelloToServerAsync()
        {
            // Send demo message to server via SignalR
            await hubConnection.SendAsync("SayHelloAsync", "Hello");
        }

        /// <summary>
        /// Get SignalR connection info by calling the negotiate endpoint.
        /// </summary>
        /// <returns>
        /// Negotiate result with SignalR URL and access token.
        /// </returns>
        private async Task<NegotiateResult> GetSignalRConnectionAsync()
        {
            // Call negotiate endpoint with the current user's access token.
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:7071/api/negotiate")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<NegotiateResult>();
        }
    }
}
