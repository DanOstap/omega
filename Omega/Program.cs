using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/start-connection", async (string url) =>
{
    using var ws = new ClientWebSocket();
    var serverUrl = new Uri(url);
    try
    {
        await ws.ConnectAsync(serverUrl, CancellationToken.None);
        Console.WriteLine("Connection complited");

        // Receive a message from the server
        var buffer = new byte[1024 * 4];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received from server: {response}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
