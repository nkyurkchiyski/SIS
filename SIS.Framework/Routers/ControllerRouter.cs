using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;
using SIS.Framework.Controllers;
using SIS.Framework.Services.Contracts;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Api;
using SIS.WebServer.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SIS.Framework.Routers
{
    public class ControllerRouter : IHandleable
    {
        private readonly IDependencyContainer provider;

        public ControllerRouter()
        {
        }

        public ControllerRouter(IDependencyContainer provider)
        {
            this.provider = provider;
        }

        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (controllerName != null)
            {
                string controllerTypeName = string.Format(
                    "{0}.{1}.{2}, {0}",
                    MvcContext.Get.AssemblyName,
                    MvcContext.Get.ControllersFolder,
                    controllerName);

                var controllerType = Type.GetType(controllerTypeName);

                var constructorInfo = controllerType.GetConstructors().FirstOrDefault();

                var parameters = constructorInfo.GetParameters()
                    .Select(p => this.provider.CreateInstance(p.ParameterType))
                    .ToArray();

                var controller = (Controller)Activator.CreateInstance(controllerType, parameters);

                if (controller != null)
                {
                    controller.Request = request;
                }

                return controller;
            }

            return null;
        }

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            MethodInfo method = null;

            foreach (var methodInfo in GetSuitableMethods(controller, actionName))
            {
                var attributes = methodInfo
                    .GetCustomAttributes()
                    .Where(attr => attr is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>();

                if (!attributes.Any() && requestMethod.ToUpper() == "GET")
                {
                    return methodInfo;
                }

                foreach (var attribute in attributes)
                {
                    if (attribute.IsValid(requestMethod))
                    {
                        return methodInfo;
                    }
                }
            }
            return method;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(methodInfo => methodInfo.Name.ToLower() == actionName.ToLower());
        }

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            string invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);
            }

            if (actionResult is IRedirectable)
            {
                return new RedirectResult(invocationResult);
            }

            throw new InvalidOperationException("The view result is not supported.");
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
        {
            return (IActionResult)action.Invoke(controller, actionParameters);
        }

        private object[] MapActionParameters(Controller controller, MethodInfo action, IHttpRequest request)
        {
            ParameterInfo[] actionParametersInfo = action.GetParameters();
            object[] mappedActionParameters = new object[actionParametersInfo.Length];

            for (int i = 0; i < actionParametersInfo.Length; i++)
            {
                ParameterInfo currentParameterInfo = actionParametersInfo[i];

                if (currentParameterInfo.ParameterType.IsPrimitive || currentParameterInfo.ParameterType == typeof(string))
                {
                    mappedActionParameters[i] = ProcessPrimitiveParameter(currentParameterInfo, request);
                }
                else
                {
                    object bindingModel = ProcessBindingParameters(currentParameterInfo, request);
                    controller.ModelState.IsValid = this.IsValidModel(bindingModel);
                    mappedActionParameters[i] = bindingModel;

                }
            }

            return mappedActionParameters;
        }

        private bool? IsValidModel(object bindingModel)
        {
            Type bindingModelType = bindingModel.GetType();
            var properties = bindingModelType.GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(ValidationAttribute), true)
                    .Cast<ValidationAttribute>()
                    .FirstOrDefault();

                var value = property.GetValue(bindingModel);

                var isValid = attribute.IsValid(value);

                if (isValid == false)
                {
                    return false;
                }
            }

            return true;
        }

        private object ProcessBindingParameters(ParameterInfo paramInfo, IHttpRequest request)
        {
            Type bindingModelType = paramInfo.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    object value = this.GetParameterFromRequestData(request, property.Name);
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value, property.PropertyType));
                }
                catch
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);

        }

        private object ProcessPrimitiveParameter(ParameterInfo currentParameterInfo, IHttpRequest request)
        {
            object value = this.GetParameterFromRequestData(request, currentParameterInfo.Name);

            return Convert.ChangeType(value, currentParameterInfo.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string name)
        {
            if (request.QueryData.ContainsKey(name)) return request.QueryData[name];

            if (request.FormData.ContainsKey(name)) return request.FormData[name];

            return null;
        }

        private IHttpResponse Authorize(Controller controller, MethodInfo action)
        {
            bool notAuthorized = action
                .GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .Any(a => !a.IsAuthorized(controller.Identity));

            if (notAuthorized)
            {
                return new UnauthorizedResult();
            }

            return null;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var path = request.Path;

            if (path == "/" || path == "/favicon.ico")
            {
                path = "Home/Index";
            }

            var pathTockens = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

            var controllerName = pathTockens[pathTockens.Length - 2] + MvcContext.Get.ControllersSuffix;
            var actionName = pathTockens[pathTockens.Length - 1];
            var requestMethod = request.RequestMethod.ToString();

            var controller = this.GetController(controllerName, request);

            MethodInfo action = this.GetMethod(requestMethod, controller, actionName);

            if (action == null)
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            object[] actionParameters = this.MapActionParameters(controller, action, request);
            
            return
                this.Authorize(controller, action) ??
                this.PrepareResponse(this.InvokeAction(controller, action, actionParameters));
        }


    }
}
