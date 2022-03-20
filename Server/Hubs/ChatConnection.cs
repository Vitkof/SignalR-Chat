using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    /// <summary>
    /// User's connection form one of the device
    /// </summary>
    public class ChatConnection
    {
        public string ConnectionId { get; set; } = null!;
        public DateTime ConnectedAt { get; set; }
    }
}
