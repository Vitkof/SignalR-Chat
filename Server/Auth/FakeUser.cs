using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Auth
{
    public class FakeUser
    {
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
