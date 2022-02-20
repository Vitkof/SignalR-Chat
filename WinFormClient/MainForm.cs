using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormClient
{
    public partial class MainForm : Form
    {
        private HubConnection _hub;
        private string _token = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitConnect()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl($"http://localhost:8334/messages?token={_token}")
                .WithAutomaticReconnect()
                .Build();

            _hub.On<NewMessage>("Send", message =>
            {
                AppendTextToChat(message.Sender, message.Text);
            });

            _hub.Closed += ex =>
            {
                MessageBox.Show($"Connection closed. {ex?.Message}");
                return Task.CompletedTask;
            };
            _hub.Reconnected += id =>
            {
                MessageBox.Show($"Connection reconnected with id: {id}");
                return Task.CompletedTask;
            };
            _hub.Reconnecting += ex =>
            {
                MessageBox.Show($"Connection reconnecting. {ex?.Message}");
                return Task.CompletedTask;
            };
        }


        private void AppendTextToChat(string sender, string text)
        {
            textBoxChat.SelectionStart = textBoxChat.TextLength;
            textBoxChat.SelectionLength = 0;
            textBoxChat.SelectionColor = sender=="Me"
                ? Color.Blue
                : Color.Black;           
            textBoxChat.AppendText($"{sender}: {text}{Environment.NewLine}");
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if (_hub == null)
                InitConnect();

            if (_hub.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hub.StartAsync();
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if(_hub.State == HubConnectionState.Connected)
                {
                    labelStatus.Text = "Connected";
                    labelStatus.ForeColor = Color.Green;
                    buttonConnect.Text = "Disconnect";
                }
            }
            else if(_hub.State == HubConnectionState.Connected)
            {
                await _hub.StopAsync();
                labelStatus.Text = "Disconnected";
                labelStatus.ForeColor = Color.Red;
                buttonConnect.Text = "Connect";
            }
        }

        private async void BtnGetToken_Click(object sender, EventArgs e)
        {
            var token = await GetToken();

            if (!String.IsNullOrEmpty(token))
            {
                _token = token;
                InitConnect();

                string[] parts = token.Split(".");
                StringBuilder decodedToken = new();
                
                for(byte i = 0; i<2; i++)
                {
                    var tokenBytes = WebEncoders.Base64UrlDecode(parts[i]);
                    var decodedPart = Encoding.UTF8.GetString(tokenBytes);
                    decodedToken.AppendLine(decodedPart.PrettifyJsonString());
                }
                decodedToken.AppendLine(parts[2]);
                MessageBox.Show(decodedToken.ToString());
            }
        }


        private async void BtnStreamFromServer_Click(object sender, EventArgs e)
        {
            var stream = _hub.StreamAsync<int>("DownloadStream");

            await foreach (var n in stream)
            {
                Debug.WriteLine(n);
            }
            Debug.WriteLine("Stream Serv->Client completed");
        }

        private async void BtnStreamToServer_Click(object sender, EventArgs e)
        {
            var asyncEnumerable = Test();
            await _hub.SendAsync("UploadStream", asyncEnumerable);
        }

        async IAsyncEnumerable<int> Test()
        {
            for (int i = 11; i > 0; i--)
            {
                yield return i;
                await Task.Delay(500);
            }
        }


        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (_hub.State == HubConnectionState.Connected)
            {
                var message = new Message {
                    Text = messageTextBox.Text };
                try
                {
                    await _hub.SendAsync("SendToOthers", message);
                    AppendTextToChat("Me", message.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    messageTextBox.Clear();
                }
            }
        }

        private void MessageRichTextBox_EnterUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) buttonSend.PerformClick();
        }


        private async Task<string> GetToken()
        {
            using var httpCl = new HttpClient();

            var authModel = new { 
                Login = textBoxName.Text, 
                Password = textBoxPassword.Text 
            };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpCl.PostAsync("http://localhost:8334/api/auth/token", content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
                return string.Empty;
            }
        }

    }
}
