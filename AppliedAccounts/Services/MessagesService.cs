using AppliedDB;
using AppMessages;
using static AppMessages.Enums;

namespace AppliedAccounts.Services
{
    public class MessagesService
    {
        public MessageClass MsgClass { get; set; }

        public MessagesService()
        {
            MsgClass = new MessageClass(Msg.GetMessages());
        }

        internal void AddRange(MessageClass msgClass)
        {
            MsgClass = msgClass;
        }

        internal void Add(string _message)
        {
            MsgClass.Add(_message);
        }

        // Errors

        internal void Error(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Error));
        }

        internal void Error(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Error));
        }

        // Danger
        internal void Danger(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Danger));
        }

        internal void Danger(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Danger));
        }

        // Critical
        internal void Critical(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Critical));
        }
        internal void Critical(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Critical));
        }

        // Success
        internal void Success(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Success));
        }
        internal void Success(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Success));
        }

        //Warrning
        internal void Warning(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Warning));
        }
        internal void Warning(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Warning));

        }

        // Alert
        internal void Alert(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Alert));
        }
        internal void Alert(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Alert));
        }
    }
}
