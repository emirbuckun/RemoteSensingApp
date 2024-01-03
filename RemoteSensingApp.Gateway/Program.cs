using System.Net;
using System.Net.Sockets;
using System.Text;

IPEndPoint ipEndPoint = new(IPAddress.Any, 8081);
using Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(2);

Console.WriteLine("The server is running at port " + ipEndPoint.Port + "...");
Console.WriteLine("The local end point address is: " + ipEndPoint.Address);
Console.WriteLine("Waiting for a connection.....");

var handler = await listener.AcceptAsync();

Console.WriteLine("Connection accepted from " + handler.RemoteEndPoint);

int checkTempSensor = 0;
int checkHumSensor = 0;

while (true)
{
    // Receive message
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    DateTime dateTimeNow = DateTime.Now;

    checkHumSensor++;
    checkTempSensor++;

    if (response.Contains("Temperature Sensor")) // Temperature data arrived
        checkTempSensor = 0;
    else if (response.Contains("ALIVE")) // Humidity alive message arrived
        checkHumSensor = 0;

    // Print data
    if (!string.IsNullOrEmpty(response))
        Console.WriteLine("Gateway: " + response);

    // Print sensor alarms
    if (checkTempSensor >= 3)
        Console.WriteLine("Gateway: TEMP SENSOR OFF Date: " + dateTimeNow);
    if (checkHumSensor >= 7)
        Console.WriteLine("Gateway: HUMIDITY SENSOR OFF Date: " + dateTimeNow);

    Thread.Sleep(1000);
}