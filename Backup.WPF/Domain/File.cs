using System.ComponentModel;
using System.Windows.Media.Imaging;
using Backup.WPF.Utilities;
using GalaSoft.MvvmLight;

namespace Backup.WPF.Domain
{
    public class File : INotifyPropertyChanged
    {
        public const string SyncStatePropertyName = "SyncState";
        private FileSyncState _syncState;
        public FileSyncState SyncState
        {
            get
            {
                return _syncState;;
            }

            set
            {
                if (_syncState == value)
                {
                    return;
                }

                _syncState = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(SyncStatePropertyName));
                }
                
            }
        }

        //
        public string FileName { get; set; }
        public string AbsoluteFileName { get; set; }
        public string FriendlyFileName { get; set; }
        public string Extension { get; set; }

        public BitmapSource Icon
        {
            get
            {
                return IconUtilities.ResolveIcon(this.AbsoluteFileName,this.Extension);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
