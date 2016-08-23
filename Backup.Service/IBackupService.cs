using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Backup.Domain.Models;

namespace Backup.Service
{
    [ServiceContract]
    public interface IBackupService
    {
        [OperationContract]
        void UploadFile(BackupStream stream);

        [OperationContract]
        UserDTO AuthenticateUser(string username, string password);

        [OperationContract]
        List<BackupFolderDTO> GetUserFolders(string username, string systemIdentifier);

        [OperationContract]
        bool AddUserFolder(string username, string folderAbsolutePath);


        // Web Site Service Calls

        [OperationContract]
        List<UserAccountsDTO> GetUsersAccounts();

        [OperationContract]
        bool CreateUser(UserDTO user);

        [OperationContract]
        List<UserTypeDTO> GetUserRoles();
    }

    [MessageContract]
    public class BackupStream
    {
        [MessageHeader(Name = "User", MustUnderstand = true)]
        public string User { get; set; }

        [MessageHeader(Name = "Name", MustUnderstand = true)]
        public string Name { get; set; }

        [MessageHeader(Name = "Directory", MustUnderstand = true)]
        public string Directory { get; set; }

        [MessageBodyMember]
        public Stream DataStream { get; set; }
    }
}
