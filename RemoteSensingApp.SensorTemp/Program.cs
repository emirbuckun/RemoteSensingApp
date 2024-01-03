using System.Net;
using System.Net.Sockets;
using System.Text;

IPEndPoint ipEndPoint = new(IPAddress.Any, 8081);
using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);

// Send message
while (true)
{
    // Create random number btw 20-30
    Random random = new();
    int randomInt = random.Next(20, 30);

    DateTime dateTime = DateTime.Now;

    var message = "Temperature Sensor: " + randomInt.ToString() + "°C Date: " + dateTime.ToString();
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);

    Console.WriteLine("Temperature Sensor Sent Data: " + message);
    Thread.Sleep(1000);
}