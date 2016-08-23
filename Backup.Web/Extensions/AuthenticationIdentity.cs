using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Backup.Web.Extensions
{
    public class AuthenticationIdentity : IIdentity
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public string UserRoleName { get; set; }

        public IIdentity Identity { get; set; }

        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return Identity.IsAuthenticated; }
        }

        public string Name
        {
            get { return Identity.Name; }
        }

        public AuthenticationIdentity(IIdentity identity)
        {
            Identity = identity;

            var membershipUser = (AuthenticationUser)Membership.GetUser(Identity.Name);
            if (membershipUser != null)
            {
                Username = membershipUser.Username;
                Email = membershipUser.Email;
                UserRoleName = membershipUser.UserRoleName;
            }
        }

    }
}