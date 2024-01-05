using System.Net;
using System.Text;

namespace RemoteSensingApp.HttpServer
{
    public class HttpServer
    {
        private readonly HttpListener httpListener;

        public HttpServer(string ipAddress, int port)
        {
            httpListener = new();
            httpListener.Prefixes.Add("http://" + ipAddress + ":" + port + "/");
            Console.WriteLine($"Server: HTTP server started on port {port}.");
        }

        public void Start()
        {
            Console.WriteLine("Server: HTTP server listening for requests...");

            httpListener.Start();

            while (true)
            {
                HttpListenerContext ctx = httpListener.GetContext();
                HttpListenerRequest req = ctx.Request;

                string? path = ctx.Request.Url?.LocalPath;
                Console.WriteLine("Path: " + path);

                if (path == "/temperature")
                    SendTemperatureData(ctx);
                else if (path == "/humidity")
                    SendHumidityData(ctx);
                else
                    NotFound(ctx);
            }
        }

        private void NotFound(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            using Stream ros = resp.OutputStream;

            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            string err = "404 - not found";

            byte[] ebuf = Encoding.UTF8.GetBytes(err);
            resp.ContentLength64 = ebuf.Length;

            ros.Write(ebuf, 0, ebuf.Length);
        }

        private void SendTemperatureData(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "image/png");

            byte[] buf = File.ReadAllBytes("public/img/sid.png");
            resp.ContentLength64 = buf.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buf, 0, buf.Length);
        }

        private void SendHumidityData(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "image/png");

            byte[] buf = File.ReadAllBytes("public/img/sid.png");
            resp.ContentLength64 = buf.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buf, 0, buf.Length);
        }
    }
}
