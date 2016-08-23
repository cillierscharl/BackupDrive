using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Backup.Domain.Models;
using Backup.WPF.Utilities;

namespace Backup.WPF.Domain
{
    public class Folder
    {
        public string Name { get; set; }
        public string AbsoluteFolderPath { get; set; }
        //
        public ObservableCollection<File> Files { get; set; }
        public List<BackupFileDTO> ServerFiles { get; set; }
        //
        public ObservableCollection<Folder> Children { get; set; }
        private FileSystemWatcher fileSystemWatcher { get; set; }
        //
        private bool FileChangeDetected { get; set; }
        //
        private SyncManager syncManager;

        public Folder()
        {
            syncManager = new SyncManager(this);
        }

        public void UpdateFolderPhysicalStructure()
        {
            this.Name = new DirectoryInfo(AbsoluteFolderPath).Name;
            this.Children = RecursiveFolderStructureBuilder(this, Directory.EnumerateDirectories(this.AbsoluteFolderPath));
            this.Files = BuildFileStructure(this.AbsoluteFolderPath);
        }

        public void UpdateServerFiles(List<BackupFileDTO> serverFiles)
        {
            this.ServerFiles = serverFiles;
            UpdateFolderSyncStatus(this, this.ServerFiles, this.Name);
            
        }

        private void UpdateFolderSyncStatus(Domain.Folder localFolder, List<BackupFileDTO> serverFiles, string topLevelDirectory)
        {

            foreach (var serverFile in serverFiles.Where((f) =>
                f.FileName.Substring(0, f.FileName.LastIndexOf('/')) == topLevelDirectory))
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
                UpdateFolderSyncStatus(localFolderChild, serverFiles, topLevelDirectory + "/" + localFolderChild.Name);
            }
        }

        #region FileUpdateChecks
        public void DeletedFilesCheck()
        {
            var comparisonTopLevelFolder = new Folder();
            comparisonTopLevelFolder.Name = this.Name;
            comparisonTopLevelFolder.AbsoluteFolderPath = this.AbsoluteFolderPath;
            comparisonTopLevelFolder.Files = BuildFileStructure(this.AbsoluteFolderPath);

            comparisonTopLevelFolder.Children = RecursiveFolderStructureBuilder(comparisonTopLevelFolder, Directory.EnumerateDirectories(this.AbsoluteFolderPath));

            CheckFilesRemoved(this,comparisonTopLevelFolder);
            CheckFilesAdded(this, comparisonTopLevelFolder);
            CheckFolderAdded(this, comparisonTopLevelFolder);
            CheckFolderRemoved(this, comparisonTopLevelFolder);

            SyncFolder();
        }

        private void CheckFilesRemoved(Folder localFolder, Folder comparisonFolder)
        {
            for (var i = 0; i < localFolder.Files.Count; i++)
            {
                var found = false;
                for (var j = 0; j < comparisonFolder.Files.Count; j++)
                {
                    if (comparisonFolder.Files[j].AbsoluteFileName == localFolder.Files[i].AbsoluteFileName)
                        found = true;
                }
                if (!found)
                {
                    localFolder.Files[i].SyncState = FileSyncState.Deleted;
                }

            }

            for (var i = 0; i < localFolder.Children.Count; i++)
            {
                var nextMatchingFolder = comparisonFolder.Children.Where(f => f.AbsoluteFolderPath == localFolder.Children[i].AbsoluteFolderPath);
                if (nextMatchingFolder.Any())
                {
                    CheckFilesRemoved(localFolder.Children[i], nextMatchingFolder.First());
                }
                
            }

        }

        private void CheckFilesAdded(Folder localFolder, Folder comparisonFolder)
        {
            for (var i = 0; i < comparisonFolder.Files.Count; i++)
            {
                var found = false;
                for (var j = 0; j < localFolder.Files.Count; j++)
                {
                    if (comparisonFolder.Files[i].AbsoluteFileName == localFolder.Files[j].AbsoluteFileName)
                        found = true;
                }
                if (!found)
                {
                    App.Current.Dispatcher.Invoke(() =>
                        {
                            localFolder.Files.Add(comparisonFolder.Files[i]);
                        });
                }

            }

            for (var i = 0; i < localFolder.Children.Count; i++)
            {

                var nextMatchingFolder = comparisonFolder.Children.Where(f => f.AbsoluteFolderPath == localFolder.Children[i].AbsoluteFolderPath);
                if (nextMatchingFolder.Any())
                {
                    CheckFilesAdded(localFolder.Children[i], nextMatchingFolder.First());
                }
            }

        }

        private void CheckFolderAdded(Folder localFolder, Folder comparisonFolder)
        {
            var found = false;
            for (var i = 0; i < comparisonFolder.Children.Count; i++)
            {

                var exists = localFolder.Children.Where(f => f.AbsoluteFolderPath == comparisonFolder.Children[i].AbsoluteFolderPath);
                if (!exists.Any())
                {
                    App.Current.Dispatcher.Invoke(() =>
                        {
                            localFolder.Children.Add(comparisonFolder.Children[i]);
                        });
                }
            }


            for (var i = 0; i < comparisonFolder.Children.Count; i++)
            {

                var nextMatchingFolder = localFolder.Children.Where(f => f.AbsoluteFolderPath == comparisonFolder.Children[i].AbsoluteFolderPath);
                if (nextMatchingFolder.Any())
                {
                    CheckFolderAdded(nextMatchingFolder.First(), comparisonFolder.Children[i]);
                }
            }
            
        }

        private void CheckFolderRemoved(Folder localFolder, Folder comparisonFolder)
        {
            var found = false;
            for (var i = 0; i < localFolder.Children.Count; i++)
            {

                var exists = comparisonFolder.Children.Where(f => f.AbsoluteFolderPath == localFolder.Children[i].AbsoluteFolderPath);
                if (!exists.Any())
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        localFolder.Children.Remove(localFolder.Children[i]);
                    });
                }
            }


            for (var i = 0; i < localFolder.Children.Count; i++)
            {

                var nextMatchingFolder = comparisonFolder.Children.Where(f => f.AbsoluteFolderPath == localFolder.Children[i].AbsoluteFolderPath);
                if (nextMatchingFolder.Any())
                {
                    CheckFolderRemoved(localFolder.Children[i], nextMatchingFolder.First());
                }
            }

        }

        #endregion

        #region Physical Folder Functions
        private ObservableCollection<Folder> RecursiveFolderStructureBuilder(Folder parent, IEnumerable<string> childrenFolders)
        {
            var children = new ObservableCollection<Folder>();

            foreach (var f in childrenFolders)
            {
                var newFolder = new Folder();
                newFolder.Name = new DirectoryInfo(f).Name;
                newFolder.AbsoluteFolderPath = f;
                newFolder.Files = BuildFileStructure(f);
                newFolder.Children = RecursiveFolderStructureBuilder(newFolder, Directory.EnumerateDirectories(f));
                children.Add(newFolder);
            }

            return children;
        }

        public ObservableCollection<Domain.File> BuildFileStructure(string folder)
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

        public void SyncFolder()
        {
            SyncFiles(this, this.Name, this.AbsoluteFolderPath);
        }

        public void SyncFiles(Domain.Folder folder, string topLevelDirectory, string rootDirectory)
        {
            var client = BackupServiceUtility.GetServiceClient();

            var unsyncedFiles = folder.Files.Where((f) => f.SyncState == FileSyncState.Unsync);
            foreach (var unsyncedFile in unsyncedFiles)
            {
                unsyncedFile.SyncState = FileSyncState.New;
                using(var fileStream = System.IO.File.OpenRead(unsyncedFile.AbsoluteFileName))
                {
                    client.UploadFile(rootDirectory, topLevelDirectory + "/" + unsyncedFile.FileName, "Charl", fileStream);
                    fileStream.Close();
                }
                
                unsyncedFile.SyncState = FileSyncState.Sync;
            }

            foreach (var childDirectory in folder.Children)
            {
                SyncFiles(childDirectory, topLevelDirectory + "/" + childDirectory.Name, rootDirectory);
            }
        }

        #endregion

        #region File System Watcher Functions
        public void InitializeFolderWatcher()
        {
            fileSystemWatcher = new FileSystemWatcher(this.AbsoluteFolderPath);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Created += fileSystemWatcher_Created;
            fileSystemWatcher.Renamed += fileSystemWatcher_Renamed;
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += fileSystemWatcher_Deleted;
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            syncManager.SyncRequest();

 	        Trace.WriteLine("Changed : " + this.Name);
            Trace.WriteLine(e);
        }

        private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {

            syncManager.SyncRequest();

 	        Trace.WriteLine("Deleted : " + this.Name);
            Trace.WriteLine(e);
        }

        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            syncManager.SyncRequest();

 	        Trace.WriteLine("Renamed : " + this.Name);
            Trace.WriteLine(e);
        }

        private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {

            syncManager.SyncRequest();

            Trace.WriteLine("Created : " + this.Name);
            Trace.WriteLine(e);
        }
        #endregion

    }

    #region SyncManager
    public class SyncManager
    {
        Folder _managedFolder;
        public SyncManager(Folder topLevelDirectory)
        {
            _managedFolder = topLevelDirectory;
        }

        Timer t;
        public void SyncRequest()
        {
            
            if(t != null)
            {
                Trace.WriteLine("Cancelling sync");
                t.Dispose();
            }
                
            Trace.WriteLine("Calling SyncRequest");
            t = new Timer(new TimerCallback(CallSync),null,2000,100000);
        }

        private void CallSync(object state)
        {
            Trace.WriteLine("Executing Sync");
            _managedFolder.DeletedFilesCheck();
            t.Dispose();
        }
    }

    #endregion
}
