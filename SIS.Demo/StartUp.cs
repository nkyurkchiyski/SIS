using SIS.Framework.Api;
using SIS.Framework.Services;
using SIS.Framework.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Demo
{
    public class StartUp : MvcApplication
    {
        public override void ConfigureServices(IDependencyContainer dependencyContainer)
        {
            dependencyContainer
                .RegisterDependency<IHashService, HashService>();
        }
    }
}
