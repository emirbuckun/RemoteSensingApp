using System.Net;
using RemoteSensingApp.Gateway;

// Arrange sensor endpoints ("127.0.0.1" = localhost)
string localhost = "127.0.0.1";
int tempPort = 8081;
int humPort = 8082;

IPEndPoint ipEndPointTemp = new(IPAddress.Parse(localhost), tempPort); // Temperature sensor
IPEndPoint ipEndPointHum = new(IPAddress.Parse(localhost), humPort); // Humidity sensor
ListenPorts lp = new(ipEndPointTemp, ipEndPointHum);

// Start listening sensors
Console.WriteLine($"Listening for temperature sensor at port {tempPort}\n" +
                    $"Listening for humidity sensor at port {humPort}\n");
lp.Start();
