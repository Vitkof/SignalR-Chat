using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient
{
    public class NewMessage : Message
    {
        public string Sender { get; set; }
        public ValueDirection Direction { get; set; }
    }

    public enum ValueDirection
    {
        Error = 2,
        Myself = 1,
        Otherself = -1
    }
}
