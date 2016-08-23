using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Backup.Web.Models;

namespace Backup.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Users", "Home");
                }
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Users", "Home");
                }

            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var valid = Membership.ValidateUser(vm.Email, vm.Password);
                if (valid)
                {
                    FormsAuthentication.RedirectFromLoginPage(vm.Email, false);
                }
                ModelState.AddModelError("", "Incorrect User Name/Password combination");

                return View();
            }
            return View(vm);
        }

    }
}
