using SIS.Demo.ViewModels;
using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes;
using SIS.Framework.Controllers;
using SIS.Framework.Services;
using SIS.Framework.Services.Contracts;
using System.Collections.Generic;

namespace SIS.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHashService hashService;

        public HomeController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        public IActionResult Index()
        {
            this.Model["Demo"] = this.hashService.Hash("Demo");

            var demoViewModels = new List<DemoViewModel>();

            for (int i = 0; i < 12; i++)
            {
                demoViewModels.Add(new DemoViewModel
                {
                    Title = $"Some Title {i}",
                    Type = $"Some Type {i}",
                    Count = i
                });
            }

            this.Model["DemoViewModels"] = demoViewModels;

            return View();
        }
    }
}
