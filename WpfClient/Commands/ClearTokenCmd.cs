using System;
using System.Collections.Generic;
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
    internal class ClearTokenCmd : Command
    {
        private readonly IMainViewModel _vm;

        public ClearTokenCmd(IMainViewModel vm)
        {
            _vm = vm;
        }

        public override bool CanExecute(object param) => true;

        public async override void Execute(object param)
        {
            _vm.IsAuthenticated = false;
            _vm.AccessToken = null;
            var dis = new DisconnectCmd(_vm);
            await dis.DisconnectFromChatAsync();
        }
    }
}
