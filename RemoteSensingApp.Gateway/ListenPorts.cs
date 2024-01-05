using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteSensingApp.Gateway
{
    internal class ListenPorts
    {
        private readonly Socket tcpSocket;
        private readonly Socket udpSocket;
        private readonly IPEndPoint temperatureEndPoint;
        private readonly IPEndPoint humidityIpEndPoint;

        internal ListenPorts(IPEndPoint temperatureEndPoint, IPEndPoint humidityIpEndPoint)
        {
            // Declare sensor endpoints
            this.temperatureEndPoint = temperatureEndPoint;
            this.humidityIpEndPoint = humidityIpEndPoint;

            // Initialise different sockets for TCP and UDP protocols
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Clear log files
            File.WriteAllText("gateway-received-log.txt", string.Empty);
            File.WriteAllText("gateway-sent-log.txt", string.Empty);
        }

        public void Start()
        {
            // Connect sockets with endpoints
            tcpSocket.Bind(temperatureEndPoint);
            udpSocket.Bind(humidityIpEndPoint);

            // Create TCP & UDP connections with multi-thread implementation
            Thread threadTcp = new(ThreadListenTcp!);
            Thread threadUdp = new(ThreadListenUdp!);

            // Run related threads to listen sockets
            threadTcp.Start(tcpSocket);
            threadUdp.Start(udpSocket);
        }

        private async void ThreadListenTcp(object objs)
        {
            try
            {
                Socket listener = (Socket)objs;
                var buffer = new byte[1_024];

                while (true)
                {
                    // Start listening and accept connection
                    listener.Listen(1);
                    var handler = listener.Accept();
                    int checkTempSensor = 0;

                    // Receive new messages when the connection is accepted
                    while (true)
                    {
                        DateTime now = DateTime.Now;

                        if (handler.Available > 0)
                        {
                            // Receive message
                            int received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                            var response = Encoding.UTF8.GetString(buffer, 0, received);

                            // Print received data
                            if (!string.IsNullOrEmpty(response) && received > 0)
                            {
                                checkTempSensor = 0;
                                Console.WriteLine($"{now} | Received: {response}");

                                // Log the received data
                                await File.AppendAllTextAsync("gateway-received-log.txt", response + Environment.NewLine);

                                // Send data to the server
                                SendReceivedData(response);
                            }
                        }

                        // Print sensor alarm
                        if (checkTempSensor >= 3)
                        {
                            string tempSensorOffMsg = $"{now} | TEMP SENSOR OFF";
                            Console.WriteLine(tempSensorOffMsg);

                            // Log the sensor alarm
                            File.AppendAllText("gateway-received-log.txt", tempSensorOffMsg + Environment.NewLine);
                            Console.WriteLine($"{now} | TEMP SENSOR OFF");
                            break; // Leave while loop to reconnect
                        }

                        // Wait for a second
                        Thread.Sleep(500);

                        // Increment temperature sensor counter
                        checkTempSensor++;
                    }
                }
            }
            catch (SocketException ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }

        private async void ThreadListenUdp(object objs)
        {
            try
            {
                Socket listener = (Socket)objs;
                var buffer = new byte[1_024];
                int checkHumSensor = 0;

                while (true)
                {
                    DateTime now = DateTime.Now;

                    if (listener.Available > 0)
                    {
                        // Receive message
                        int received = await listener.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        // Print data
                        if (!string.IsNullOrEmpty(response) && received > 0)
                        {
                            Console.WriteLine($"{now} | Received: {response}");

                            // Log the received data
                            File.AppendAllText("gateway-received-log.txt", response + Environment.NewLine);

                            // Send data if not alive message
                            if (response.Contains("ALIVE"))
                                checkHumSensor = 0;
                            else
                                SendReceivedData(response);
                        }
                    }

                    // Print sensor alarm
                    if (checkHumSensor == 7)
                    {
                        string humSensorOffMsg = $"{now} | HUMIDITY SENSOR OFF";
                        Console.WriteLine(humSensorOffMsg);

                        // Log the sensor alarm
                        File.AppendAllText("gateway-received-log.txt", humSensorOffMsg + Environment.NewLine);
                    }

                    // Wait for a second
                    Thread.Sleep(1000);

                    // Increment humidity sensor counter
                    checkHumSensor++;
                }
            }
            catch (SocketException ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }

        public void SendReceivedData(string data)
        {
            try
            {
                // Connect to the server
                IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 8083);
                using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(ipEndPoint);

                // Prepare message
                var message = data;
                var messageBytes = Encoding.UTF8.GetBytes(message);

                // Send message
                client.Send(messageBytes, SocketFlags.None);

                // Print sent data
                DateTime now = DateTime.Now;
                Console.WriteLine($"{now} | Sent: {message}");

                // Log sent data
                File.AppendAllText("gateway-sent-log.txt", message + Environment.NewLine);

                // Wait for a second
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }
    }
}