using System.Security.Principal;
using System.Web.Security;

namespace Backup.Web.Extensions
{
    public class AuthenticationPrincipal : IPrincipal
    {
        public IIdentity Identity { private set; get; }

        public AuthenticationPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return Roles.Provider.IsUserInRole(Identity.Name,role);
        }
    }
}