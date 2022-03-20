using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;


namespace WpfClient.ViewModels
{
    internal interface IMainViewModel
    {
        public string DisplayHeader { get; set; }
        public string Nickname { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsConnected { get; set; }
        public string AccessToken { get; set; }
        public string AuthServUrl { get; set; }
        public string ChatServUrl { get; set; }
        public HubConnection Connection { get; set; }
        public ObservableCollection<string> UsersList { get; set; }
        public ObservableCollection<NewMessage> MessagesList { get; set; }
        public string MessageText { get; set; }
    }
}
