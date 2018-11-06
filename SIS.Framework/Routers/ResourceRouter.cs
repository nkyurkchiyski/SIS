using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Api;
using SIS.WebServer.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIS.Framework.Routers
{
    public class ResourceRouter : IHandleable
    {
        private const string RootDirectoryRelativePath = "../../..";

        public IHttpResponse Handle(IHttpRequest request)
        {
            bool isResourceRequest = this.IsResourceRequest(request);

            if (isResourceRequest)
            {
                return this.HandleResourceResponse(request.Path);
            }
            
            return null;
        }

        private bool IsResourceRequest(IHttpRequest httpRequest)
        {
            string requestPath = httpRequest.Path;

            if (requestPath.Contains('.'))
            {
                var requestPathExtension = requestPath
                    .Substring(requestPath.LastIndexOf('.'));
                return GlobalConstants.ResourceExtensions.Contains(requestPathExtension);
            }
            return false;
        }

        private IHttpResponse HandleResourceResponse(string path)
        {
            var indexOfStartExtension = path.LastIndexOf('.');

            var indexOfStartOfNameOfResouce = path.LastIndexOf('/');

            string requestPathExtension = path
                     .Substring(path.LastIndexOf('.'));

            var resourceName = path.Substring(indexOfStartOfNameOfResouce);

            string resourcePath = RootDirectoryRelativePath
                + "/Resources"
                + $"/{requestPathExtension.Substring(1)}"
                + resourceName;

            if (!File.Exists(resourcePath))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            var fileContent = File.ReadAllBytes(resourcePath);

            return new InlineResourceResult(fileContent, HttpResponseStatusCode.Ok);

        }
    }
}
