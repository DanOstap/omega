using System.Net.WebSockets;
using System.Text;

namespace Omega.Service;

public class WebSocketsService: BackgroundService
{
    private readonly IConfiguration _configuration;

    public WebSocketsService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Uri _serverUri = new Uri(_configuration["Url"]);
        using var webSocket = new ClientWebSocket();
        try
        {
            
            // request from client 
            await webSocket.ConnectAsync(_serverUri, stoppingToken);
            Console.WriteLine("WebSocket client connected.");
            var serverMessage = "Server is running... ";
            var buffer = Encoding.UTF8.GetBytes(serverMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
            Console.WriteLine("Was sending to server: " + serverMessage);
            
            // request to client
            var bufferClient = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(bufferClient), CancellationToken.None);
            var responseServerMessage = Encoding.UTF8.GetString(bufferClient, 0, result.Count);
            Console.WriteLine($"Received from server: {responseServerMessage}");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
