using SIS.HTTP.Enums;
using System;

namespace SIS.HTTP.Extensions
{
    public static class HttpResponseStatusExtensions
    {
        public static string GetResponseLine(this HttpResponseStatusCode statusCode)
        {
            return $"{(int)statusCode} {Enum.GetName(typeof(HttpResponseStatusCode), statusCode)}";
        }

    }
}
