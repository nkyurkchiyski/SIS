﻿using SIS.Framework.Attributes.Methods;

namespace SIS.Framework.Attributes
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToUpper() == "GET")
            {
                return true;
            }

            return false;
        }
    }
}
