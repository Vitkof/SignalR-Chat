using Microsoft.AspNetCore.SignalR.Client;
using Server;
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
    internal class ConnectCmd : Command
    {
        private readonly IMainViewModel _vm;


        public ConnectCmd(IMainViewModel vm)
        {
            _vm = vm;
        }


        public override bool CanExecute(object param) => 
            _vm.IsAuthenticated && 
            !_vm.IsConnected;

        public async override void Execute(object param)
        {
            await ConnectToChatAsync();
        }


        private async Task ConnectToChatAsync()
        {
            _vm.Connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:8334/messages?token={_vm.AccessToken}")
                .WithAutomaticReconnect()
                .Build();


            _vm.Connection.On<NewMessage>("Send", message =>
            {
                _vm.MessagesList.Add(message);
            });

            _vm.Connection.On<string, string>("SendMessageAsync", (user, message) =>
            {
                var newMsg = new NewMessage
                {
                    Text = message,
                    Sender = user
                };
                _vm.MessagesList.Add(newMsg);
            });

            _vm.Connection.On<IEnumerable<string>>("UpdateUsersAsync", users =>
            {
                _vm.UsersList = new ObservableCollection<string>(users);
            });

            _vm.Connection.Reconnected += id =>
            {
                _vm.MessagesList.Add(new NewMessage
                {
                    Text = $"Connection reconnected with id: {id}"
                });
                return Task.CompletedTask;
            };
            _vm.Connection.Reconnecting += ex =>
            {
                _vm.MessagesList.Add(new NewMessage { Text = $"Connection reconnecting. {ex?.Message}" });
                return Task.CompletedTask;
            };

            try
            {
                await _vm.Connection.StartAsync();
                _vm.IsConnected = true;
            }
            catch (Exception exception)
            {
                _vm.MessagesList.Add(new NewMessage
                {
                    Text = exception.Message,
                    Direction = ValueDirection.Error
                });
            }
        }

        private void AppendTextToChat(string sender, string text)
        {
            var newMsg = new NewMessage
            {
                Text = text,
                Sender = sender
            };

            _vm.MessagesList.Add(newMsg);
        }
    }
}
