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
        [Authorize(Policy = "BadWordsPolicy")]
        public Task SendToOthers(Message msg)
        {
            var msgForClient = new NewMessage(Context.UserIdentifier, msg);
            return Clients.Others.Send(msgForClient);
        }


        public Task<string> GetClientName()
        {
            if (Context.Items.ContainsKey("Name"))
                return Task.FromResult(Context.Items["Name"] as string);
            return Task.FromResult("Anonymous");
        }


        public Task SetClientName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
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
