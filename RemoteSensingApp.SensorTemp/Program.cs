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

    // Clear log file
    await File.WriteAllTextAsync("temp-sent-log.txt", string.Empty);

    // Send message
    while (true)
    {
        // Create temperature data btw 20-30
        Random random = new();
        int temperature = random.Next(20, 30);
        DateTime now = DateTime.Now;

        // Prepare message
        var message = $"TEMP | {temperature} | {now}";
        var messageBytes = Encoding.UTF8.GetBytes(message);

        // Send message
        _ = await client.SendAsync(messageBytes, SocketFlags.None);

        // Log the sent data
        await File.AppendAllTextAsync("temp-sent-log.txt", message + Environment.NewLine);

        // Print sent data
        Console.WriteLine($"Sent: {message}");

        // Wait for a second
        Thread.Sleep(1000);
    }
}
catch (SocketException ex)
{
    DateTime now = DateTime.Now;
    Console.WriteLine("\nThere is a problem with the gateway connection." +
                    $"\nPlease try again. Error details: {ex.Message} | {now}.\n");
}
