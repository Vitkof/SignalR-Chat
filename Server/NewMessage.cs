using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public sealed class NewMessage : Message
    {
        public string Sender { get; set; }

        public NewMessage(string sender, Message msg)
        {
            Sender = !String.IsNullOrWhiteSpace(sender)
                    ? sender
                    : "Anonymous";
            Text = msg.Text;
        }
    }
}
