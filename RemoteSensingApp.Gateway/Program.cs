using System.Net;
using RemoteSensingApp.Gateway;

IPEndPoint ipEndPointTemp = new(IPAddress.Parse("127.0.0.1"), 8081);
IPEndPoint ipEndPointHum = new(IPAddress.Parse("127.0.0.1"), 8082);

IPEndPoint[] ipPoints = new IPEndPoint[2] { ipEndPointTemp, ipEndPointHum };
ListenPorts lp = new(ipPoints);

Console.WriteLine("Gateway: Starts Listening");
lp.BeginListen();