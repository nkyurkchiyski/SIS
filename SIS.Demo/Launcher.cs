using SIS.Framework;
using SIS.Framework.Routers;
using SIS.Framework.Services;
using SIS.Framework.Services.Contracts;
using SIS.WebServer;

namespace SIS.Demo
{
    class Launcher
    {
        static void Main(string[] args)
        {
            WebHost.Start(new StartUp());
        }
    }
}
