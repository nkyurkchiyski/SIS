using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Controllers;
using SIS.Framework.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Demo.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Login()
        {
            this.SignIn(new IdentityUser { Username = "Pesho", Password = "123" });
            return this.View();
        }

        [Authorize]
        public IActionResult Authorized()
        {
            this.Model["Username"] = this.Identity.Username;
            return this.View();
        }
    }
}
