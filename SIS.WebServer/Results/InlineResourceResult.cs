using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.WebServer.Results
{
    public class InlineResourceResult : HttpResponse
    {
        private const string InlineValue = "inline";

        public InlineResourceResult(byte[] content, HttpResponseStatusCode statusCode) : base(statusCode)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, InlineValue));
            this.Content = content;
        }
    }
}
