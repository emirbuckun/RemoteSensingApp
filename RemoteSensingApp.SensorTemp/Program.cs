using System.Net;
using System.Net.Sockets;
using System.Text;

// Set TCP connection
Console.Title = "Temperature Sensor";
IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 8081);
using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    // Connect to the gateway
    await client.ConnectAsync(ipEndPoint);

    // Send message
    while (true)
    {
        // Create temperature data btw 20-30
        Random random = new();
        string temperature = random.Next(20, 30).ToString();
        string now = DateTime.Now.ToString();

        // Send temperature data
        var message = $"TEMP | {temperature} | {now}";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);

        // Print sent data
        Console.WriteLine($"Sent: {message}");

        // Wait for a second
        Thread.Sleep(1000);
    }
}
catch (SocketException ex)
{
    Console.WriteLine("\n\tThere is a problem with the gateway connection." +
                    $"\n\tPlease try again. Error Details: {ex.Message}.\n");
}
