using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteSensingApp.Gateway
{
    internal class ListenGateway
    {
        private readonly Socket socket;
        private readonly IPEndPoint ipEndPoint;

        internal ListenGateway(string ipAddress, int port)
        {
            ipEndPoint = new(IPAddress.Parse(ipAddress), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Clear log files
            File.WriteAllText("server-received-temp-log.txt", string.Empty);
            File.WriteAllText("server-received-hum-log.txt", string.Empty);
        }

        public void Start()
        {
            socket.Bind(ipEndPoint);

            Console.WriteLine($"Listening for gateway at port {ipEndPoint.Port}.");

            Thread thread = new(ThreadListen!);
            thread.Start(socket);
        }

        private async void ThreadListen(object objs)
        {
            try
            {
                Socket listener = (Socket)objs;
                var buffer = new byte[1_024];

                while (true)
                {
                    listener.Listen(100);
                    var handler = listener.Accept();

                    DateTime dateTimeNow = DateTime.Now;
                    if (handler.Available > 0)
                    {
                        // Receive message over TCP
                        int received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        // Print received data
                        if (!string.IsNullOrEmpty(response) && received > 0)
                        {
                            Console.WriteLine($"Received: {response}");

                            // Store the data
                            StoreData(response);
                        }

                        // Wait for a second
                        Thread.Sleep(100);
                    }
                }
            }
            catch (SocketException ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }

        public async void StoreData(string data)
        {
            DateTime now = DateTime.Now;
            try
            {
                // Log the sent data
                if (data.Contains("TEMP"))
                    await File.AppendAllTextAsync("server-received-temp-log.txt", $"{now} | {data}" + Environment.NewLine);
                else // Contains "HUM"
                    await File.AppendAllTextAsync("server-received-hum-log.txt", $"{now} | {data}" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }
    }
}