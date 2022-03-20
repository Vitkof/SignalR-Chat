using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public interface IMessageClient
    {
        Task Send(NewMessage msg);

        /// <param name="name"></param>
        /// <param name="message"></param>
        Task SendMessageAsync(string name, string message);

        /// <summary>
        /// Update users list
        /// </summary>
        /// <param name="users"></param>
        Task UpdateUsersAsync(IEnumerable<string> users);
    }
}
