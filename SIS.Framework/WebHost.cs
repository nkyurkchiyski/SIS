using SIS.Framework.Api.Contracts;
using SIS.Framework.Routers;
using SIS.Framework.Services;
using SIS.Framework.Services.Contracts;
using SIS.WebServer;
using SIS.WebServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Framework
{
    public static class WebHost
    {
        private const int HostingPort = 8000;

        public static void Start(IMvcApplication application)
        {
            IDependencyContainer container = new DependencyContainer();

            application.ConfigureServices(container);

            IHandleable controllerRouter = new ControllerRouter(container);
            IHandleable resourceRouter = new ResourceRouter();

            application.Configure();

            Server server = new Server(HostingPort, controllerRouter, resourceRouter);
            MvcEngine.Run(server);
        }
    }
}
