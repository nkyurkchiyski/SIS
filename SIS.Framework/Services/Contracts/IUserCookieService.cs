using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Framework.Services.Contracts
{
    public interface IUserCookieService
    {
        string GetUserCookie(string userName);

        string GetUserData(string cookieContent);
    }
}
