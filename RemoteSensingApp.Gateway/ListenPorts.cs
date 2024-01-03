using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteSensingApp.Gateway
{
    internal class ListenPorts
    {
        private readonly Socket[] sockets;
        private readonly IPEndPoint[] ipEndPoints;

        internal ListenPorts(IPEndPoint[] ipEndPoints)
        {
            this.ipEndPoints = ipEndPoints;
            sockets = new Socket[ipEndPoints.Length];

            sockets[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sockets[1] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void BeginListen()
        {
            for (int i = 0; i < ipEndPoints.Length; i++)
            {
                sockets[i].Bind(ipEndPoints[i]);

                Console.WriteLine("Gateway: The gateway is listening at port: " + ipEndPoints[i].Port
                    + " over " + sockets[i].ProtocolType.ToString().ToUpper());

                Thread thread = new(ThreadListen!);
                thread.Start(sockets[i]);
            }
        }

        private async void ThreadListen(object objs)
        {
            try
            {
                Socket listener = (Socket)objs;
                var buffer = new byte[1_024];

                if (listener.ProtocolType == ProtocolType.Tcp)
                {
                    while (true)
                    {
                        int checkTempSensor = 0;
                        listener.Listen(100);
                        var handler = listener.Accept();
                        Console.WriteLine("Gateway: Connection accepted for temperature sensor from "
                            + handler.RemoteEndPoint);

                        while (true)
                        {
                            DateTime dateTimeNow = DateTime.Now;
                            if (handler.Available > 0)
                            {
                                // Receive message over TCP
                                int received = handler.Receive(buffer, SocketFlags.None);
                                var response = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                                // Print data
                                if (!string.IsNullOrEmpty(response) && received > 0)
                                {
                                    Console.WriteLine("Gateway: " + response);
                                    checkTempSensor = 0;
                                }
                            }

                            // Print sensor alarm
                            if (checkTempSensor == 3)
                            {
                                Console.WriteLine("Gateway: TEMP SENSOR OFF Date: " + dateTimeNow);
                                break;
                            }

                            // Wait for a second
                            Thread.Sleep(1000);

                            // Increment temperature sensor counter
                            checkTempSensor++;
                        }
                    }
                }
                else if (listener.ProtocolType == ProtocolType.Udp)
                {
                    int checkHumSensor = 0;
                    while (true)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (listener.Available > 0)
                        {
                            // Receive message over UDP
                            int received = await listener.ReceiveAsync(buffer, SocketFlags.None);
                            var response = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                            // Humidity ALIVE message arrived
                            if (response.Contains("ALIVE") && received > 0)
                                checkHumSensor = 0;

                            // Print data
                            if (!string.IsNullOrEmpty(response) && received > 0)
                                Console.WriteLine("Gateway: " + response);
                        }

                        // Print sensor alarm
                        if (checkHumSensor == 7)
                            Console.WriteLine("Gateway: HUMIDITY SENSOR OFF Date: " + dateTimeNow);

                        // Wait for a second
                        Thread.Sleep(1000);

                        // Increment humidity sensor counter
                        checkHumSensor++;
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}