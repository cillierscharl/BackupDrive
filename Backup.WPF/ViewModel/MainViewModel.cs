using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Backup.Domain.Models;
using Backup.WPF.Domain;
using Backup.WPF.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Backup.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            #region Init VM Defaults
            FolderCollection = new ObservableCollection<Folder>();

            _currentUser = new User()
            {
                Username = "log in"
            };
            #endregion

            #region Register for cross-vm communication
            MessengerInstance.Register<User>(this, user =>
            {
                CurrentUser = user;
                LoadTrackedDirectories();
            });


            //MessengerInstance.Register<Folder>(this, folder =>
            //{
            //    App.Current.Dispatcher.Invoke(() =>
            //    {
            //        FileCollection = folder.Files;
            //    });
            //});
            #endregion

        }

        private void LoadTrackedDirectories()
        {
            var client = BackupServiceUtility.GetServiceClient();
            List<BackupFolderDTO> result = client.GetUserFolders(CurrentUser.Username,System.Environment.MachineName);

            result.ForEach((folder) =>
            {


                Task.Factory.StartNew<Folder>(() =>
                {
                    var topLevelFolder = new Folder()
                    {
                        AbsoluteFolderPath = folder.AbsoluteFolderPath
                    };

                    topLevelFolder.UpdateFolderPhysicalStructure();
                    topLevelFolder.UpdateServerFiles(folder.BackupFiles);
                    topLevelFolder.InitializeFolderWatcher();


                    return topLevelFolder;

                }).ContinueWith<Folder>((res) =>
                {
                    FolderCollection.Add(res.Result);
                    return res.Result;
                }, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith((t) =>
                {
                    t.Result.SyncFolder();
                });

            });
            
        }

        #region VM INPC + Properties
        public ObservableCollection<Folder> FolderCollection { get; set; }

        public const string CurrentUserPropertyName = "CurrentUser";
        private User _currentUser = null;
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }

            set
            {
                if (_currentUser == value)
                {
                    return;
                }
                _currentUser = value;
                RaisePropertyChanged(CurrentUserPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedFolder" /> property's name.
        /// </summary>
        public const string SelectedFolderPropertyName = "SelectedFolder";

        private Folder _selectedFolder = null;

        /// <summary>
        /// Sets and gets the SelectedFolder property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Folder SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }

            set
            {
                if (_selectedFolder == value)
                {
                    return;
                }

                _selectedFolder = value;
                RaisePropertyChanged(SelectedFolderPropertyName);
            }
        }

        #endregion

        #region Events

        RelayCommand _addFolderCommand;
        public ICommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                {
                    _addFolderCommand = new RelayCommand(AddFolder);
                    return _addFolderCommand;
                }
                return _addFolderCommand;
            }
        }

        private void AddFolder()
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                Task.Factory.StartNew<Folder>(() =>
                {
                    var client = BackupServiceUtility.GetServiceClient();
                    var res = client.AddUserFolder(CurrentUser.Username, folderBrowser.SelectedPath);

                    var topLevelFolder = new Folder()
                    {
                        AbsoluteFolderPath = folderBrowser.SelectedPath
                    };

                    topLevelFolder.UpdateFolderPhysicalStructure();
                    topLevelFolder.UpdateServerFiles(new List<Backup.Domain.Models.BackupFileDTO>());
                    topLevelFolder.InitializeFolderWatcher();

                    return topLevelFolder;

                }).ContinueWith<Folder>((res) =>
                {
                    FolderCollection.Add(res.Result);
                    return res.Result;
                }, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith((t) =>
                {
                    t.Result.SyncFolder();
                });
            }
            
        }


        #endregion

    }
}
