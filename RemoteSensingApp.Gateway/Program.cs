using System.Net;
using RemoteSensingApp.Gateway;

// Arrange sensor endpoints ("127.0.0.1" = localhost)
IPEndPoint ipEndPointTemp = new(IPAddress.Parse("127.0.0.1"), 8081); // Temperature sensor
IPEndPoint ipEndPointHum = new(IPAddress.Parse("127.0.0.1"), 8082); // Humidity sensor
ListenPorts lp = new(ipEndPointTemp, ipEndPointHum);

Console.WriteLine("\nStarts Listening\n");
lp.Start();
