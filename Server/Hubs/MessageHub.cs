using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class MessageHub : Hub<IMessageClient>
    {
        public Task SendToOthers(Message msg)
        {
            var msgForClient = new NewMessage(Context.Items["Name"] as string, msg);
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
    }
}
