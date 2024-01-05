using System.Net;
using System.Net.Sockets;
using System.Text;

// Set UDP connection
Console.Title = "Humidity Sensor";
IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 8082);
using Socket client = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

try
{
    // Connect to the gateway
    await client.ConnectAsync(ipEndPoint);

    // Clear log file
    await File.WriteAllTextAsync("hum-sent-log.txt", string.Empty);

    // Send message
    int aliveCounter = 0;
    while (true)
    {
        DateTime now = DateTime.Now;

        // Send alive message
        if (aliveCounter >= 3)
        {
            // Prepare alive message
            var aliveMsg = $"HUM | ALIVE | " + now.ToString();
            var aliveMsgBytes = Encoding.UTF8.GetBytes(aliveMsg);
            _ = await client.SendAsync(aliveMsgBytes, SocketFlags.None);

            // Print sent data
            Console.WriteLine($"Sent: {aliveMsg}");

            // Log the sent data
            await File.AppendAllTextAsync("hum-sent-log.txt", aliveMsg + Environment.NewLine);

            // Reset alive counter
            aliveCounter = 0;
        }

        // Create humidity data btw 40-90
        Random random = new();
        int humidity = random.Next(40, 90);

        // Send humidity data
        if (humidity > 80)
        {
            // Prepare message
            var message = $"HUM | {humidity} | {now}";
            var messageBytes = Encoding.UTF8.GetBytes(message);

            // Send message
            _ = await client.SendAsync(messageBytes, SocketFlags.None);

            // Print sent data
            Console.WriteLine($"Sent: {message}");

            // Log the sent data
            await File.AppendAllTextAsync("hum-sent-log.txt", message + Environment.NewLine);
        }

        // Wait for a second
        Thread.Sleep(1000);

        // Increment alive message counter
        aliveCounter++;
    }
}
catch (SocketException ex)
{
    Console.WriteLine("\n\tThere is a problem with the gateway connection." +
                    $"\n\tPlease try again. Error details: {ex.Message}.\n");
}
