using SIS.WebServer.Api;
using SIS.WebServer.Routing;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SIS.WebServer
{
    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int port;

        private readonly TcpListener listener;

        private readonly ServerRoutingTable serverRoutingTable;

        private readonly IHandleable handler;
        
        private readonly IHandleable resourceRouter;

        private bool isRunning;

        public Server(int port)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), this.port);
        }

        public Server(int port, ServerRoutingTable serverRoutingTable, IHandleable resourceRouter)
            : this(port)
        {
            this.serverRoutingTable = serverRoutingTable;
            this.resourceRouter = resourceRouter;
        }

        public Server(int port, IHandleable handler, IHandleable resourceRouter)
            : this(port)
        {
            this.handler = handler;
            this.resourceRouter = resourceRouter;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server started at http://{LocalhostIpAddress}:{port}");

            var task = Task.Run(this.ListenLoop);
            task.Wait();
        }

        public async Task ListenLoop()
        {
            while (this.isRunning)
            {
                var client = await this.listener.AcceptSocketAsync();

                ConnectionHandler connectionHandler = null;

                if (this.handler == null)
                {
                    connectionHandler = new ConnectionHandler(client, this.serverRoutingTable, this.resourceRouter);
                }
                else
                {
                    connectionHandler = new ConnectionHandler(client, this.handler, this.resourceRouter);
                }

                var responseTask = connectionHandler.ProcessRequestAsync();
                responseTask.Wait();

            }
        }

    }
}
