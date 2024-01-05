using RemoteSensingApp.HttpServer;
using RemoteSensingApp.Gateway;

// Start listening HTTP server and gateway
HttpServer httpServer = new("127.0.0.1", 8080);
ListenGateway listenGateway = new("127.0.0.1", 8083);

listenGateway.Start();
httpServer.Start();