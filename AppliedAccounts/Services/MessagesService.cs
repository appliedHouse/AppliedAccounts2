using AppMessages;
using Microsoft.Data.Sqlite;
using static AppMessages.Enums;

namespace AppliedAccounts.Services
{
    public class MessagesService : IMessagesService
    {
        public long LanguageID { get; set; } = 1;            // Default Language 1 is English
        public MessageClass MsgClass { get; set; } = new();
        private SqliteConnection MyConnection { get; set; }

        public MessagesService(IConfiguration configuration)
        {
            try
            {
                var MsgPath = configuration.GetSection("Paths:MessagesPath").Value;
                if (MsgPath != null)
                {
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", MsgPath, "Messages.db");
                    var MsgConnectionString = $"Data Source={FilePath}";
                    MyConnection = new SqliteConnection(MsgConnectionString);
                }
            }
            catch (Exception ex)
            {

                Error(ex.Message);
            }
        }

        public void AddRange(MessageClass msgClass)
        {
            foreach (var msg in msgClass.MessageList)
            {
                MsgClass.MessageList.Add(msg);
            }

            foreach (var err in msgClass.Errors)
            {
                MsgClass.Errors.Add(err);
            }

            MsgClass = msgClass;
        }

        public void AddRange(MessagesService msgService)
        {
            foreach (var msg in msgService.MsgClass.MessageList)
            {
                MsgClass.MessageList.Add(msg);
            }

            foreach (var err in msgService.MsgClass.Errors)
            {
                MsgClass.Errors.Add(err);
            }



            MsgClass = msgService.MsgClass;
        }


        public void InsertDB(string _message)
        {
            MsgClass.InsertDB(_message);
        }

        // Errors

        public void Error(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Error));
        }

        public void Error(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Error));
        }
        public void Error(Exception _error)
        {
            MsgClass.Errors.Add(MessageClass.ErrorMessage(_error));
        }

        // Danger
        public void Danger(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Danger));
        }

        public void Danger(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Danger));
        }

        // Critical
        public void Critical(Messages _code)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_code, Class.Critical));
        }
        public void Critical(string _text)
        {
            MsgClass.Errors.Add(MsgClass.GetMessage(_text, Class.Critical));
        }

        // Success
        public void Success(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Success));
        }
        public void Success(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Success));
        }

        //Warrning
        public void Warning(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Warning));
        }
        public void Warning(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Warning));

        }

        // Alert
        public void Alert(Messages _code)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_code, Class.Alert));
        }
        public void Alert(string _text)
        {
            MsgClass.MessageList.Add(MsgClass.GetMessage(_text, Class.Alert));
        }

        public void Clear()
        {
            MsgClass.MessageList.Clear();
            MsgClass.Errors.Clear();
        }

        

        public int Count => MsgClass.Count;
        public int MessageCount => MsgClass.MessageList.Count;
        public int ErrorCount => MsgClass.Errors.Count;

    }

    public interface IMessagesService
    {
        void AddRange(MessageClass msgClass);
        void AddRange(MessagesService msgService);
        
        void Error(Messages code);
        void Error(string text);
        void Error(Exception error);

        void Danger(Messages code);
        void Danger(string text);

        void Critical(Messages code);
        void Critical(string text);

        void Success(Messages code);
        void Success(string text);

        void Warning(Messages code);
        void Warning(string text);

        void Alert(Messages code);
        void Alert(string text);

        void Clear();

        int Count { get; }
        int MessageCount { get; }
        int ErrorCount { get; }
    }
}
