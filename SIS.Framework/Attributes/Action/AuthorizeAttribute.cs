using SIS.Framework.Security.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIS.Framework.Attributes.Action
{
    public class AuthorizeAttribute : Attribute
    {
        private readonly string role;

        public AuthorizeAttribute() { }

        public AuthorizeAttribute(string role)
        {
            this.role = role;
        }

        private bool IsIdentityPresent(IIdentity identity)
        => identity != null;

        private bool IsIdentityInRole(IIdentity identity)
        {
            if (!IsIdentityPresent(identity))
            {
                return false;
            }

            if (!identity.Roles.Select(r => r.ToLower()).Contains(role.ToLower()))
            {
                return false;
            }

            return true;
        }

        public bool IsAuthorized(IIdentity user)
        {
            if (role == null)
            {
                return IsIdentityPresent(user);
            }
            else
            {
                return IsIdentityInRole(user);
            }
        }

    }
}
