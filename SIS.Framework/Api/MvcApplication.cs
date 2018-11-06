using SIS.Framework.Api.Contracts;
using SIS.Framework.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Framework.Api
{
    public class MvcApplication : IMvcApplication
    {
        public virtual void Configure() { }

        public virtual void ConfigureServices(IDependencyContainer dependencyContainer) { }
    }
}
