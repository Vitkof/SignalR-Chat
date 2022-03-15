using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfClient.Commands.Base;
using WpfClient.ViewModels;

namespace WpfClient.Commands
{
    internal class LoginCmd : Command
    {
        private readonly IMainViewModel _vm;

        public LoginCmd(IMainViewModel vm)
        {
            _vm = vm;
        }

        public override bool CanExecute(object param) =>
            !string.IsNullOrWhiteSpace(_vm.Nickname) &&
            param is PasswordBox;

        public async override void Execute(object param)
        {
            PasswordBox pbox = (PasswordBox)param;
            string pass = pbox.Password;
            if (!CanExecute(param)) return;


            using var http = new HttpClient();

            var authModel = new
            {
                Login = _vm.Nickname,
                Password = pass
            };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await http.PostAsync("http://localhost:8334/api/auth/token", content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _vm.AccessToken = await response.Content.ReadAsStringAsync();
                _vm.IsAuthenticated = !string.IsNullOrWhiteSpace(_vm.AccessToken);
                return;
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
                _vm.AccessToken = string.Empty;
                return;
            }
        }
    }
}
