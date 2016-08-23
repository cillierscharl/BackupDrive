using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Backup.DataAccess;

namespace Backup.Domain.Models
{
    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserType { get; set; }

        [DataMember]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class UserAccountDTO 
    {
        [DataMember]
        public string AccountUniqueId {get; set;}

        [DataMember]
        public int AccountDataQuota {get; set;}
    }

    [DataContract]
    public class BackupFolderDTO
    {
        [DataMember]
        public string AbsoluteFolderPath { get; set; }
        [DataMember]
        public List<BackupFileDTO> BackupFiles { get; set; }
    }


    [DataContract]
    public class BackupFileDTO
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public Int64 FileSize { get; set; }
        [DataMember]
        public string BackupFileState { get; set; }
    }

    [DataContract]
    public class UserAccountsDTO
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public string UserType { get; set; }

        [DataMember]
        public List<UserAccountDTO> UserAccounts { get; set; }

    }

    [DataContract]
    public class UserTypeDTO
    {
        [DataMember]
        public string Type { get; set; }
    }
}
