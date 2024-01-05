using System.Net.WebSockets;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Remote Sensing App API",
        Description = "ASP.NET Core 7 Web API"
    });
});

var app = builder.Build();

// Web socket connection
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1_024];
                var received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (received.Count > 0)
                {
                    var response = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    Console.WriteLine(response);
                    break;
                }
            }
        }
        else context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
    else await next(context);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();