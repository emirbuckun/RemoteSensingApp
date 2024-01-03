using System.Net;
using System.Net.Sockets;
using System.Text;

// Set TCP connection
IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 8081);
using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
await client.ConnectAsync(ipEndPoint);

// Send message
while (true)
{
    // Create temperature data btw 20-30
    Random random = new();
    int temperature = random.Next(20, 30);
    DateTime dateTime = DateTime.Now;

    // Send temperature data
    var message = "Temperature Sensor: " + temperature.ToString() + "°C Date: " + dateTime.ToString();
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);

    // Print sent data
    Console.WriteLine("Temperature Sensor Sent Data: " + message);

    // Wait for a second
    Thread.Sleep(1000);
}
