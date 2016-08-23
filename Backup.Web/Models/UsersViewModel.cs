using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Backup.Domain.Models;
using Backup.MVC;


namespace Backup.Web.Models
{
    public class UsersViewModel
    {
        public List<UserAccountsDTO> UsersAccounts { get; set; }

        public UsersViewModel()
        {
            var client = BackupServiceUtility.GetServiceClient();
            UsersAccounts = client.GetUsersAccounts();
        }
    }
}