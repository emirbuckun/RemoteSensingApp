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
            httpListener.Prefixes.Add($"http://{ipAddress}:{port}/");
            httpListener.Prefixes.Add($"http://localhost:{port}/");
            Console.WriteLine($"HTTP server started at port {port}.");
        }

        public void Start()
        {
            httpListener.Start();

            while (true)
            {
                HttpListenerContext ctx = httpListener.GetContext();
                string? path = ctx.Request.Url?.LocalPath;

                if (path == "/temperature")
                    SendTemperatureData(ctx);
                else if (path == "/humidity")
                    SendHumidityData(ctx);
                else
                    NotFound(ctx);
            }
        }

        private void SendTemperatureData(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            using Stream ros = resp.OutputStream;

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            string readText = File.ReadAllText("server-received-temp-log.txt");

            byte[] ebuf = Encoding.UTF8.GetBytes(readText);
            resp.ContentLength64 = ebuf.Length;

            ros.Write(ebuf, 0, ebuf.Length);
        }

        private void SendHumidityData(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            using Stream ros = resp.OutputStream;

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            string readText = File.ReadAllText("server-received-hum-log.txt");

            byte[] ebuf = Encoding.UTF8.GetBytes(readText);
            resp.ContentLength64 = ebuf.Length;

            ros.Write(ebuf, 0, ebuf.Length);
        }

        private void NotFound(HttpListenerContext ctx)
        {
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            using Stream ros = resp.OutputStream;

            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            string err = "404 - Not Found";

            byte[] ebuf = Encoding.UTF8.GetBytes(err);
            resp.ContentLength64 = ebuf.Length;

            ros.Write(ebuf, 0, ebuf.Length);
        }
    }
}
