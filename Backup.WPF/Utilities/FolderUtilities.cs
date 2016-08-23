using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Backup.Domain.Models;
using Backup.WPF.Domain;

namespace Backup.WPF.Utilities
{
    public static class FolderUtilities
    {
        #region Physical Folder Routines
        public static Folder GetDirectoryStructure(string path)
        {
            var results = Directory.EnumerateDirectories(path);
            var parentFolder = new Folder(){
                Name = new DirectoryInfo(path).Name,
                AbsoluteFolderPath = path
            };
            parentFolder.Files = BuildFileStructure(path);
            parentFolder.Children = BuildFolderHierarchy(parentFolder, results);
            return parentFolder;
        }


        public static ObservableCollection<Domain.File> BuildFileStructure(string folder)
        {
            var fileList = new ObservableCollection<Domain.File>();
            var results = Directory.EnumerateFiles(folder);
            foreach (var file in results)
            {
                var info = new FileInfo(file);
                fileList.Add(new Domain.File()
                {
                    FileName = info.Name,
                    AbsoluteFileName = file,
                    Extension = info.Extension,
                    FriendlyFileName = info.Name//info.Name.Substring(0,info.Name.IndexOf('.'))
                });
            }
            return fileList;
        }

        private static ObservableCollection<Folder> BuildFolderHierarchy(Folder folder, IEnumerable<string> childrenFolders)
        {
            var children = new ObservableCollection<Folder>();
            foreach (var f in childrenFolders)
            {
                var newFolder = new Folder();
                newFolder.Name = new DirectoryInfo(f).Name;
                newFolder.AbsoluteFolderPath = f;
                newFolder.Files = BuildFileStructure(f);
                newFolder.Children = BuildFolderHierarchy(newFolder, Directory.EnumerateDirectories(f));
                children.Add(newFolder);
            }
            return children;
        }
        #endregion

        #region Tag files that need to be synced. Master - > Child
        public static void UpdateFoldersSyncStatus(ObservableCollection<Domain.Folder> localFolders, List<BackupFolderDTO> serverFolderCollection)
        {
            foreach(var localTopLevelFolder in localFolders)
            {
                var localTopLevelFolderName = localTopLevelFolder.AbsoluteFolderPath;
                var serverTopLevelFolder = serverFolderCollection.Where((f) => f.AbsoluteFolderPath == localTopLevelFolderName);
                if(serverTopLevelFolder.Any())
                {
                    var localServerDirInfo = new DirectoryInfo(localTopLevelFolderName);
                    UpdateFolderSyncStatus(localTopLevelFolder, serverTopLevelFolder.First(), localServerDirInfo.Name);
                }else {
                    // Cant find the top level folder mapped locally. (this is a problem :P)
                }
            }
        }

        private static void UpdateFolderSyncStatus(Domain.Folder localFolder, BackupFolderDTO serverFolder,string topLevelDirectory)
        {
            
            foreach(var serverFile in serverFolder.BackupFiles.Where((f) =>
                f.FileName.Substring(0,f.FileName.LastIndexOf('/')) == topLevelDirectory))
            {
                var directoryLessServerName = serverFile.FileName.Substring(serverFile.FileName.LastIndexOf('/') + 1);

                foreach (var localFile in localFolder.Files)
                {
                    if (localFile.FileName == directoryLessServerName)
                    {
                        // check size etc.
                        localFile.SyncState = FileSyncState.Sync;
                    }
                }

            }

            foreach (var localFolderChild in localFolder.Children)
            {
                UpdateFolderSyncStatus(localFolderChild, serverFolder, topLevelDirectory + "/" + localFolderChild.Name);
            }
        }
        #endregion

        #region Sync files that are tagged
        public static void SyncFolders(ObservableCollection<Domain.Folder> localFolders)
        {
            foreach (var localFolder in localFolders)
            {
                SyncFiles(localFolder,localFolder.Name,localFolder.AbsoluteFolderPath);
            }
        }


        private static void SyncFiles(Domain.Folder folder,string topLevelDirectory, string rootDirectory)
        {
            var client = BackupServiceUtility.GetServiceClient();

            var unsyncedFiles = folder.Files.Where((f) => f.SyncState == FileSyncState.Unsync);
            foreach (var unsyncedFile in unsyncedFiles)
            {
                unsyncedFile.SyncState = FileSyncState.New;
                client.UploadFile(rootDirectory, topLevelDirectory + "/" + unsyncedFile.FileName, "Charl", System.IO.File.OpenRead(unsyncedFile.AbsoluteFileName));
                unsyncedFile.SyncState = FileSyncState.Sync;
            }

            foreach (var childDirectory in folder.Children)
            {
                SyncFiles(childDirectory, topLevelDirectory + "/" + childDirectory.Name,rootDirectory);
            }
        }
        #endregion

    }
}
