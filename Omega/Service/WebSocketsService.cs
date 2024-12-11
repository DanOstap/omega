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
            await webSocket.ConnectAsync(_serverUri, stoppingToken);
            Console.WriteLine("WebSocket client connected.");

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
