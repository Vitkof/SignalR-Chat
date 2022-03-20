using Microsoft.AspNetCore.SignalR.Client;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Commands.Base;
using WpfClient.ViewModels;

namespace WpfClient.Commands
{
    internal class SendCmd : Command
    {
        private readonly IMainViewModel _vm;

        public SendCmd(IMainViewModel vm)
        {
            _vm = vm;
        }

        public override bool CanExecute(object param) =>
            _vm.IsConnected &&
            !string.IsNullOrWhiteSpace(_vm.AccessToken) &&
            !string.IsNullOrWhiteSpace(_vm.Nickname) && 
            !string.IsNullOrWhiteSpace(_vm.MessageText);

        public override async void Execute(object param)
        {
            await SendMessageAsync(_vm.MessageText);
            _vm.MessageText = string.Empty;
        }


        private async Task SendMessageAsync(string? message)
        {
            var msg = new Message
            {
                Text = message
            };

            await _vm.Connection.SendAsync("SendToOthers", msg);

            var newMsg = new NewMessage
            {
                Text = message,
                Sender = "Me",
                Direction = ValueDirection.Myself
            };
            _vm.MessagesList.Add(newMsg);
        }
    }
}
