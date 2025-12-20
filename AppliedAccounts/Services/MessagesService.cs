
using AppliedDB;

namespace AppliedAccounts.Services
{
    public class MessagesService
    {
        public AppMessages.MessageClass MsgClass { get; set; }

        public MessagesService()
        {
            MsgClass = new AppMessages.MessageClass(Msg.GetMessages());
        }

        
    }
}
