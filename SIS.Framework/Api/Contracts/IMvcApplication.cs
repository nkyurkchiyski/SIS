using SIS.Framework.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Framework.Api.Contracts
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IDependencyContainer dependencyContainer);
    }
}
