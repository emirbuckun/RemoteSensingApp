using System.Net;
using System.Net.Sockets;
using System.Text;

// Set UDP connection
IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 8082);
using Socket client = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
await client.ConnectAsync(ipEndPoint);

// Send message
int aliveCounter = 0;
while (true)
{
    DateTime dateTime = DateTime.Now;

    // Send alive message
    if (aliveCounter >= 3)
    {
        var aliveMsg = "Humidity Sensor: ALIVE Date: " + dateTime.ToString();
        var aliveMsgBytes = Encoding.UTF8.GetBytes(aliveMsg);
        _ = await client.SendAsync(aliveMsgBytes, SocketFlags.None);

        // Print sent data
        Console.WriteLine("Humidity Sensor Sent Data: " + aliveMsg);
        aliveCounter = 0;
    }

    // Create humidity data btw 40-90
    Random random = new();
    int humidity = random.Next(40, 90);

    // Send humidity data
    if (humidity > 80)
    {
        var message = "Humidity Sensor: " + humidity.ToString() + " Date: " + dateTime.ToString();
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);

        // Print sent data
        Console.WriteLine("Humidity Sensor Sent Data: " + message);
    }

    // Wait for a second
    Thread.Sleep(1000);

    // Increment alive message counter
    aliveCounter++;
}
