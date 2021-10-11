using System.Threading.Tasks;

namespace Server
{
    public interface IMessageClient
    {
        Task Send(NewMessage msg);
    }
}
