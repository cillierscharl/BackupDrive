using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Backup.Domain.Models;
using Backup.MVC;

namespace Backup.Web.Models
{
    public class CreateUserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name="User Role")]
        public string UserRole { get; set; }
        [Required]
        public int UserDataQuota { get; set; }
        
        public List<UserTypeDTO> UserRoles { get; set; }


        public CreateUserViewModel()
        {
            var client = BackupServiceUtility.GetServiceClient();
            UserRoles = client.GetUserRoles();
        }
    }
}