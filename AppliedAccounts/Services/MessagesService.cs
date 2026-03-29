using AppMessages;
using Microsoft.Data.Sqlite;
using static AppMessages.Enums;

namespace AppliedAccounts.Services
{
    public class MessagesService : IMessagesService
    {
        public MessageClass MsgClass { get; set; }
        public long LanguageID { get; set; } = 1;            // Default Language 1 is English

        private SqliteConnection SqlConnection { get; set; }
        private readonly string MsgConnectionString;

        public MessagesService(IConfiguration configuration)
        {
            try
            {
                var MsgPath = configuration.GetSection("Paths:MessagesPath").Value;
                if (MsgPath != null)
                {
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", MsgPath, "Messages.db");
                    MsgConnectionString = $"Data Source={FilePath}";
                    SqlConnection = GetConnection();
                    MsgClass = new() { MsgConnection = SqlConnection };
                    SqlConnection = null!;
                }
            }
            catch (Exception ex)
            {

                MsgClass!.Error(ex.Message);
            }
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(MsgConnectionString);
        }

        public void AddRange(MessageClass msgClass)
        {
            MsgClass = msgClass;
        }

        public void Add(string _message)
        {
            MsgClass.Add(_message);
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
    }

    public interface IMessagesService
    {
        void AddRange(MessageClass msgClass);
        void Add(string message);

        void Error(Messages code);
        void Error(string text);

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
    }
}
