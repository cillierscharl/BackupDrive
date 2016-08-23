using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Backup.Web.Extensions
{
    public class AuthenticationUser : MembershipUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserRoleName { get; set; }
    }
}