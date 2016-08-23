using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Backup.Domain.Models;
using Backup.MVC;
using Backup.Web.Models;

namespace Backup.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles="Administrator")]
        public ActionResult Users()
        {
            var client = BackupServiceUtility.GetServiceClient();
            var vm = new UsersViewModel();
            return View(vm);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult CreateUser(CreateUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var client = BackupServiceUtility.GetServiceClient();

                client.CreateUser(new UserDTO()
                {
                    Username = vm.Username,
                    Email = vm.Email,
                    UserType = vm.UserRole
                });

                return Json(new { success = true });
            }

            return PartialView(vm);
        }

    }
}
