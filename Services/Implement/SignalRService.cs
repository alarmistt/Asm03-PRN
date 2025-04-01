using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Services.Interface;

namespace Services.Implement
{
    public class SignalRService : IAsyncDisposable
    {
        private HubConnection? hubConnection;
        private readonly string hubUrl;
        private bool isConnected;

        public SignalRService(IConfiguration configuration)
        {
            hubUrl = configuration["SignalR:HubUrl"] ?? "https://localhost:7115/chatHub";
        }

        public async Task StartConnection()
        {
            if (isConnected) return;

            try
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                hubConnection.On<string>("ReceiveMessage", (message) =>
                {
                    Console.WriteLine($"Received message: {message}");
                });

                await hubConnection.StartAsync();
                isConnected = true;
                Console.WriteLine("SignalR Connected Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR Connection Error: {ex.Message}");
                isConnected = false;
            }
        }

        public async Task SendMessage(string message)
        {
            try
            {
                if (hubConnection is not null && isConnected)
                {
                    await hubConnection.SendAsync("SendMessage", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        public bool IsConnected => isConnected;
    }
} 