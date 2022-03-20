using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        private readonly ChatManager _chatManager;
        private const string _defaultGroup = "Subscribers";

        public MessageHub(ChatManager manager)
        {
            _chatManager = manager;
        }


        #region overrides
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var connectionId = Context.ConnectionId;
            _chatManager.ConnectUser(userName, connectionId);
            await Groups.AddToGroupAsync(connectionId, _defaultGroup);
            await UpdateUsersAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var isUserRemoved = _chatManager.DisconnectUser(Context.ConnectionId);
            if (!isUserRemoved)
            {
                await base.OnDisconnectedAsync(exception);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _defaultGroup);
            await UpdateUsersAsync();
            await base.OnDisconnectedAsync(exception);
        }
        #endregion


        [Authorize(Policy = "BadWordsPolicy")]
        public Task SendToOthers(Message msg)
        {
            var msgForClient = new NewMessage(Context.UserIdentifier, msg);
            return Clients.Others.Send(msgForClient);
        }
        
        [Authorize(Policy = "BadWordsPolicy")]
        public async Task SendMessageAsync(string name, string msg)
        {
            await Clients.All.SendMessageAsync(name, msg);
        }


        public async Task UpdateUsersAsync()
        {
            var users = _chatManager.Users.Select(u => u.Name).ToList();
            await Clients.All.UpdateUsersAsync(users);
        }
        

        public Task<string> GetClientName()
        {
            if (Context.Items.ContainsKey("Name"))
                return Task.FromResult(Context.Items["Name"] as string);
            return Task.FromResult("Anonymous");
        }


        public Task SetClientName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Task.CompletedTask;

            if (Context.Items.ContainsKey("Name"))
                Context.Items["Name"] = name;
            else Context.Items.Add("Name", name);
            return Task.CompletedTask;
        }


        public Task Subscribe()
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, "Subscribers");
        }


        public Task Unsubscribe()
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, "Subscribers");
        }


        public async IAsyncEnumerable<int> DownloadStream([EnumeratorCancellation] CancellationToken token)
        {
            for (int i = 0; i < 11 && !token.IsCancellationRequested; i++)
            {
                yield return i;
                await Task.Delay(500, token);
            }
        }


        public async Task UploadStream(IAsyncEnumerable<int> async)
        {
            await foreach (var n in async)
            {
                Debug.WriteLine(n);
            }
            Debug.WriteLine("Stream Client->Serv end");
        }
    }
}
