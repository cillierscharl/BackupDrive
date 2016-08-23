using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Backup.WPF.Domain;
using Backup.WPF.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Backup.WPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            _loginWindowVisible = true;
        }

        #region INPC + Properties
        public const string UsernamePropertyName = "Username";
        private string _username = "";
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (_username == value)
                {
                    return;
                }

                RaisePropertyChanging(UsernamePropertyName);
                _username = value;
                RaisePropertyChanged(UsernamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="LoginWindowVisible" /> property's name.
        /// </summary>
        public const string LoginWindowVisiblePropertyName = "LoginWindowVisible";

        private bool _loginWindowVisible = false;

        public bool LoginWindowVisible
        {
            get
            {
                return _loginWindowVisible;
            }

            set
            {
                if (_loginWindowVisible == value)
                {
                    return;
                }

                _loginWindowVisible = value;
                RaisePropertyChanged(LoginWindowVisiblePropertyName);
            }
        }


        #endregion

        #region Events
        RelayCommand<object> _loginCommand;
        public ICommand LoginCommand
        {
            get {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand<object>(Login);
                    return _loginCommand;
                }
                return _loginCommand;
            }
        }

        public void Login(object password)
        {

            var client = BackupServiceUtility.GetServiceClient();

            Task t = new Task(() =>
            {
                var result = client.AuthenticateUser(Username, ((PasswordBox)password).Password);
                App.Current.Dispatcher.Invoke(() =>
                 {
                     if (result == null)
                     {
                         Username = "";
                         ((PasswordBox)password).Clear();
                         return;
                     }

                     var user = new User()
                     {
                         Username = result.Username
                     };

                     LoginWindowVisible = false;
                     MessengerInstance.Send<User>(user);
                 });
            });

            t.Start();
        }
        #endregion
    }
}
