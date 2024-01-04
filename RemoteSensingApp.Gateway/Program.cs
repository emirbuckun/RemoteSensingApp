using System.Net;
using RemoteSensingApp.Gateway;

// Arrange sensor endpoints ("127.0.0.1" = localhost)
IPEndPoint ipEndPointTemp = new(IPAddress.Parse("127.0.0.1"), 8081); // Temperature sensor
IPEndPoint ipEndPointHum = new(IPAddress.Parse("127.0.0.1"), 8082); // Humidity sensor

IPEndPoint[] ipPoints = new IPEndPoint[2] { ipEndPointTemp, ipEndPointHum };
ListenPorts lp = new(ipPoints);

Console.WriteLine("Gateway: Starts Listening");
lp.BeginListen();