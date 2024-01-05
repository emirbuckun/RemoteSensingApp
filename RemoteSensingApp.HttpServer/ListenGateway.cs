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
            this.ipEndPoint = new(IPAddress.Parse(ipAddress), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            socket.Bind(ipEndPoint);

            Console.WriteLine("Server: The server is listening for gateway at port: " + ipEndPoint.Port
                + " over " + socket.ProtocolType.ToString().ToUpper());

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
                    Console.WriteLine("Server: Listening for the gateway...");
                    listener.Listen(100);

                    var handler = listener.Accept();
                    Console.WriteLine("Server: Connection accepted for the gateway");

                    while (true)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (handler.Available > 0)
                        {
                            // Receive message over TCP
                            int received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                            var response = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                            // Print received data
                            if (!string.IsNullOrEmpty(response) && received > 0)
                            {
                                Console.WriteLine("Server: " + response);

                                // Send data to the server
                                // StoreData(response);
                            }
                        }

                        // Wait for a second
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (SocketException ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }

        public void StoreData(string data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"{ex.Message} | {now}");
            }
        }
    }
}