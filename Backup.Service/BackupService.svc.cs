using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoMapper;
using Backup.DataAccess;
using Backup.Domain.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Backup.Service
{
    public class BackupService : IBackupService
    {
        #region WPF Application
        public void UploadFile(BackupStream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[1024 * 8];
            int length = 0;


            while ((length = stream.DataStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, length);
            }

            using (var db = new ApertureBackupContext())
            {
                var userQuery = db.Users.Where(u => u.Username == stream.User).First();
                var account = db.UserAccounts.Where(u => u.UserId == userQuery.Id).First();

                if (userQuery != null)
                {
                    var file = new DataAccess.BackupFile();
                    file.FileName = stream.Name;
                    file.FileSize = ms.Length;
                    file.BackupFolderId = db.BackupFolders.Where(f => f.UserAccountId == account.Id && f.AbsoluteFolderPath == stream.Directory).First().Id;
                    file.BackupFileState = db.BackupFileStates.Where(s => s.State == "New").First();

                    db.BackupFiles.Add(file);
                    db.SaveChanges();
                }
            }
            

            //using (var fs = new FileStream(@"C:\Users\charl.cilliers\Documents\Backup\bk\" + stream.Name, FileMode.OpenOrCreate))
            //{
            //    ms.CopyTo(fs);
            //}

            //using (ms)
            //{
            //    WriteToAzure(ms, stream.Name);
            //}

        }

        public UserDTO AuthenticateUser(string username, string password)
        {
            using (var db = new ApertureBackupContext())
            {
                var query = db.Users
                    .Where(b => b.Username == username && b.Password == password);

                var result = query.FirstOrDefault();

                return Mapper.Map<User, UserDTO>(result);
            }
        }

        public List<BackupFolderDTO> GetUserFolders(string username, string systemIdentifier)
        {
            using (var db = new ApertureBackupContext())
            {
                var userAccountQuery = db.Users.Where((u) => u.Username == username).Include((b) => b.UserAccounts).First();
                var mappedAccounts = userAccountQuery.UserAccounts.Where(u => u.AccountUniqueId == systemIdentifier);

                if (!mappedAccounts.Any())
                {
                    db.Users.Where((u) => u.Username == username).First().UserAccounts.Add(
                        new UserAccount()
                        {
                            AccountUniqueId = systemIdentifier
                        });
                    db.SaveChanges();
                }

                if (userAccountQuery != null)
                {
                    var userAccount = userAccountQuery.UserAccounts.First().Id;

                    var accountFolderQuery = db.BackupFolders.Where(
                        b => b.UserAccountId == userAccount)
                        .Include(i => i.BackupFiles).ToList();

                    return Mapper.Map<List<BackupFolder>, List<BackupFolderDTO>>(accountFolderQuery);
                }
                return null;
            }
        }

        public bool AddUserFolder(string username, string folderAbsolutePath)
        {
            using (var db = new ApertureBackupContext())
            {
                var userAccountQuery = db.Users.Where((u) => u.Username == username).Include((b) => b.UserAccounts).First();

                var folder = new BackupFolder()
                {
                    AbsoluteFolderPath = folderAbsolutePath,
                    UserAccount = userAccountQuery.UserAccounts.First()
                };

                db.BackupFolders.Add(folder);
                db.SaveChanges();

                return true;
            }
        }

        private void WriteToAzure(MemoryStream dataStream, string name)
        {

            dataStream.Position = 0;

            CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureConnectionString"]);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("charlcilliers");

            container.CreateIfNotExists();
            
            CloudBlockBlob blob = container.GetBlockBlobReference(name);

            blob.UploadFromStream(dataStream);

            container.FetchAttributes();

            foreach (var b in container.ListBlobs())
            {
                if (b is CloudBlockBlob)
                {
                    Trace.WriteLine(((CloudBlockBlob)b).Properties.Length);
                }
                
            }
            
        }
        #endregion

        #region Web Application
        public List<UserAccountsDTO> GetUsersAccounts()
        {
            using (var db = new ApertureBackupContext())
            {

                var results = db.Users
                    .Include(ut=>ut.UserType)
                    .Include(ua => ua.UserAccounts).ToList();

                var resultList = Mapper.Map<List<User>, List<UserAccountsDTO>>(results);
                return resultList;
            }
        }

        public bool CreateUser(UserDTO user)
        {
            using (var db = new ApertureBackupContext())
            {
                db.Users.Add(new User()
                {
                    Username = user.Username,
                    Enabled = true,
                    Email = user.Email,
                    Password = "Password",
                    UserType = db.UserTypes.Where(ut=>ut.Type == user.UserType).First()
                });
                db.SaveChanges();
                return true;
            }
        }

        public List<UserTypeDTO> GetUserRoles()
        {
            using (var db = new ApertureBackupContext())
            {
                var results = db.UserTypes.ToList();
                return Mapper.Map<List<UserType>, List<UserTypeDTO>>(results);
            }
        }
        #endregion
    }
}
