using SIS.Framework.ActionResults;
using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Models;
using SIS.Framework.Security.Contracts;
using SIS.Framework.Utilities;
using SIS.Framework.Views;
using SIS.HTTP.Requests.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SIS.Framework.Controllers
{
    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
        }

        public Model ModelState { get; } = new Model();

        private ViewEngine ViewEngine { get; } = new ViewEngine();

        protected ViewModel Model { get; }

        public IHttpRequest Request { get; set; }

        public IIdentity Identity
            => this.Request.Session.ContainsParameter("auth")
                ? (IIdentity)this.Request.Session.GetParameter("auth")
                : null;

        protected IViewable View([CallerMemberName] string actionName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, actionName);
            }
            catch (FileNotFoundException e)
            {
                this.Model.Data["Error"] = e.Message;
                viewContent = this.ViewEngine.GetErrorContent();
            }

            string renderedContent = this.ViewEngine.RenderHtml(viewContent, this.Model.Data);
            return new ViewResult(new View(renderedContent));
        }

        protected IRedirectable RedirectToAction(string redirectUrl) => new RedirectResult(redirectUrl);

        protected void SignIn(IIdentity auth)
        {
            this.Request.Session.AddParameter("auth", auth);
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }

    }
}
