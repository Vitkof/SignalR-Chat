using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Commands;
using WpfClient.Commands.Base;
using WpfClient.ViewModels.Base;


namespace WpfClient.ViewModels
{
    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly Command _loginCmd;
        private readonly Command _connectCmd;
        private readonly Command _disconnectCmd;
        private readonly Command _sendCmd;
        private readonly Command _clearTokenCmd;


        public MainViewModel()
        {
            _sendCmd = new SendCmd(this);
            _loginCmd = new LoginCmd(this);
            _connectCmd = new ConnectCmd(this);
            _disconnectCmd = new DisconnectCmd(this);
            _clearTokenCmd = new ClearTokenCmd(this);
        }

        #region property DisplayHeader
        private string _displayHeader = "WPF Client";

        public string DisplayHeader
        {
            get => _displayHeader;
            set => Set(ref _displayHeader, value);
        }
        #endregion

        #region property Nickname
        private string _nickname = "mike_999";

        public string Nickname
        {
            get => _nickname;
            set
            {
                Set(ref _nickname, value);
                LoginCmd.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region property IsAuthenticated
        private bool _isAuthenticated;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                Set(ref _isAuthenticated, value);
                ConnectCmd.RaiseCanExecuteChanged();
                DisconnectCmd.RaiseCanExecuteChanged();
            } 
        }
        #endregion

        #region property IsConnected
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                Set(ref _isConnected, value);
                ConnectCmd.RaiseCanExecuteChanged();
                DisconnectCmd.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region property AccessToken
        private string? _accessToken;

        public string? AccessToken
        {
            get => _accessToken;
            set => Set(ref _accessToken, value);
        }
        #endregion

        #region property AuthServUrl
        private string _authServUrl = "http://localhost:8334/api/auth/token";

        public string AuthServUrl
        {
            get => _authServUrl;
            set => Set(ref _authServUrl, value);
        }
        #endregion

        #region property ChatServUrl
        private string _chatServUrl = "http://localhost:8334";

        public string ChatServUrl
        {
            get => _chatServUrl;
            set => Set(ref _chatServUrl, value);
        }
        #endregion

        #region property Connection
        private HubConnection _connection = null!;

        public HubConnection Connection
        {
            get => _connection;
            set
            {
                Set(ref _connection, value);
            }
        }
        #endregion

        #region property UsersList
        private ObservableCollection<string> _usersList = new();

        public ObservableCollection<string> UsersList
        {
            get => _usersList;
            set => Set(ref _usersList, value);
        }
        #endregion

        #region property MessagesList
        private ObservableCollection<string> _messagesList = new();

        public ObservableCollection<string> MessagesList
        {
            get => _messagesList;
            set => Set(ref _messagesList, value);
        }
        #endregion

        #region property MessageText
        private string? _messageText;

        public string? MessageText
        {
            get => _messageText;
            set
            {
                Set(ref _messageText, value);
                SendCmd!.RaiseCanExecuteChanged();
            }
        }
        #endregion


        public Command LoginCmd => _loginCmd;
        public Command SendCmd => _sendCmd;
        public Command ConnectCmd => _connectCmd;
        public Command DisconnectCmd => _disconnectCmd;
        public Command ClearTokenCmd => _clearTokenCmd;
    }
}
