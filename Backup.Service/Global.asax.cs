using System;
using AutoMapper;
using Backup.DataAccess;
using Backup.Domain.Models;

namespace Backup.Service
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Mapper.CreateMap<User, UserDTO>();
            Mapper.CreateMap<UserAccount, UserAccountDTO>();
            Mapper.CreateMap<User, UserAccountsDTO>()
                .ForMember(d => d.UserType, opt => opt.MapFrom(s => s.UserType.Type));
            Mapper.CreateMap<UserType, UserTypeDTO>();
            Mapper.CreateMap<BackupFolder, BackupFolderDTO>();
            Mapper.CreateMap<BackupFile, BackupFileDTO>();
            Mapper.AssertConfigurationIsValid();
        }
    }
}