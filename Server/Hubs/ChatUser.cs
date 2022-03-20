using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    /// <summary>
    /// User in chat 
    /// </summary>
    public class ChatUser
    {
        private readonly List<ChatConnection> _connections;

        //ctor
        public ChatUser(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _connections = new List<ChatConnection>();
        }

        #region Properties
        public string Name { get; }
        public IEnumerable<ChatConnection> Connections => _connections;
        public DateTime? ConnectedAt
        {
            get
            {
                return Connections.Any()
                    ? Connections
                        .OrderByDescending(c => c.ConnectedAt)
                        .Select(c => c.ConnectedAt)
                        .First()
                    : null;
            }
        }
        #endregion

        public void AddConnection(string connectId)
        {
            if (connectId == null)
                throw new ArgumentNullException(nameof(connectId));

            ChatConnection connect = new ChatConnection
            {
                ConnectionId = connectId,
                ConnectedAt = DateTime.UtcNow
            };

            _connections.Add(connect);
        }

        public void RemoveConnection(string connectId)
        {
            if (connectId == null)
                throw new ArgumentNullException(nameof(connectId));

            ChatConnection connect = Connections.SingleOrDefault(c =>
                c.ConnectionId.Equals(connectId, StringComparison.Ordinal));

            if (connect != null)
                _connections.Remove(connect);
        }
    }
}
