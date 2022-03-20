using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    /// <summary>
    /// Users manager for chat
    /// </summary>
    public class ChatManager
    {
        public List<ChatUser> Users { get; } = new();


        public void ConnectUser(string name, string connectId)
        {
            ChatUser userExist = GetUserByName(name);
            if (userExist != null)
            {
                userExist.AddConnection(connectId);
                return;
            }

            ChatUser user = new ChatUser(name);
            user.AddConnection(connectId);
            Users.Add(user);
        }

        public bool DisconnectUser(string connectId)
        {
            ChatUser userExist = GetUserById(connectId);
            if (userExist != null)
            {
                if (userExist.Connections.Count() == 1)
                {
                    Users.Remove(userExist);
                }
                else userExist.RemoveConnection(connectId);
                return true;
            }
            else
                return false;
        }

        /// <param name="connectId"></param>
        private ChatUser? GetUserById(string connectId)
        {
            return Users
                .FirstOrDefault(u => u.Connections
                .Select(c => c.ConnectionId)
                .Contains(connectId));
        }

        /// <param name="name"></param>
        private ChatUser? GetUserByName(string name)
        {
            return Users
                .FirstOrDefault(u => string.Equals(
                    u.Name, name,
                    StringComparison.CurrentCultureIgnoreCase)
                );
        }
    }
}
