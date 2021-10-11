using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormClient
{
    public partial class MainForm : Form
    {
        private readonly HubConnection _hub;
        public MainForm()
        {
            InitializeComponent();
            _hub = new HubConnectionBuilder()
                .WithUrl("http://localhost:8334/messages")
                .WithAutomaticReconnect()
                .Build();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _hub.On<NewMessage>("Send", message =>
            {
                AppendTextToChat(message.Sender, message.Text);
            });

            _hub.Closed += ex =>
            {
                MessageBox.Show($"Connection closed. {ex.Message}");
                return Task.CompletedTask;
            };
            _hub.Reconnected += id =>
            {
                MessageBox.Show($"Connection reconnected with id {id}");
                return Task.CompletedTask;
            };
            _hub.Reconnecting += ex =>
            {
                MessageBox.Show($"Connection reconnecting. {ex.Message}");
                return Task.CompletedTask;
            };
        }

        private void AppendTextToChat(string sender, string text)
        {
            textBoxChat.SelectionStart = textBoxChat.TextLength;
            textBoxChat.SelectionLength = 0;
            textBoxChat.SelectionColor = sender=="Me"
                ? Color.Green
                : Color.Black;
            textBoxChat.AppendText($"{sender}: {text}{Environment.NewLine}");
            textBoxChat.SelectionColor = textBoxChat.ForeColor;


        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if(_hub.State == HubConnectionState.Disconnected)
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

        private async void BtnGet_Click(object sender, EventArgs e)
        {
            if(_hub.State == HubConnectionState.Connected)
            {
                var name = await _hub.InvokeAsync<string>("GetClientName");
                if (!String.IsNullOrWhiteSpace(name))
                    textBoxName.Text = name;
                else
                    textBoxName.Text = "Anonymous";
            }
        }

        private async void BtnSet_Click(object sender, EventArgs e)
        {
            if(_hub.State == HubConnectionState.Connected)
            {
                try { await _hub.SendAsync("SetClientName", textBoxName.Text); }
                catch(Exception ex) {
                    MessageBox.Show(ex.Message);}
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
    }
}
