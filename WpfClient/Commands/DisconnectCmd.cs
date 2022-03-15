using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WpfClient.Commands.Base;
using WpfClient.ViewModels;

namespace WpfClient.Commands
{
    internal class DisconnectCmd : Command
    {
        private readonly IMainViewModel _vm;


        public DisconnectCmd(IMainViewModel vm)
        {
            _vm = vm;
        }

        public override bool CanExecute(object param) => 
            _vm.IsConnected;

        public async override void Execute(object param)
        {
            await DisconnectFromChatAsync();
        }


        internal async Task DisconnectFromChatAsync()
        {
            await _vm.Connection.StopAsync();
            _vm.IsConnected = false;
            await _vm.Connection.DisposeAsync();
            _vm.UsersList = new ObservableCollection<string>();
            _vm.MessagesList = new ObservableCollection<string>();
        }
    }
}
