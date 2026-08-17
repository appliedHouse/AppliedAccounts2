using Microsoft.Data.Sqlite;
using System.Data;
using AppliedGlobals;
using static AppMessages.Enums;

namespace AppMessages
{
    public class MessageClass
    {
        #region Variables
        public AppValues AppGlobal { get; set; }
        public Message MyMessage { get; set; }
        public List<Message> MessageList { get; set; } = [];
        public List<Message> Errors { get; set; } = [];
        public DataTable MessagesTable { get; set; }
        public Message Empty { get; set; } = new();
        public int Count => MessageList.Count + Errors.Count;
        public int CountError => Errors.Count;
        public int CountMessages => MessageList.Count;
        public SqliteConnection? MsgConnection { get; set; }
        public long LanguageID { get; set; } = 1;             // Default Language English, Id = 1
        public string FilePath { get; set; }

        #endregion

        #region Constructor
        public MessageClass()
        {
            LanguageID = 1;             // Default Language English, Id = 1
        }

        public MessageClass(SqliteConnection msgConnection)
        {
            MsgConnection = msgConnection;
            LanguageID = 1;             // Default Language English, Id = 1
        }
        public MessageClass(SqliteConnection msgConnection, long _LanguageID)
        {
            MsgConnection = msgConnection;
            LanguageID = _LanguageID;
        }
        #endregion

        #region Clear Message / Error List
        public void ClearMessages()
        {
            MessageList.Clear();
            Errors.Clear();
        }
        #endregion

        #region Add Message in the List
        public void Add(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Alert));
        }
        
        #endregion
        
        #region Success
        public void Success(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Success));
        }

        public void Success(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Success));
        }
        #endregion

        #region Alert
        public void Alert(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Alert));
        }

        public void Alert(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Alert));
        }
        #endregion

        #region Warrning
        public void Warning(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Warning));
        }
        public void Warning(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Warning));
        }
        #endregion

        #region Error
        public void Error(string _Text)
        {
            Errors.Add(GetMessage(_Text, Class.Error));
        }

        public void Error(Messages _Code)
        {
            Errors.Add(GetMessage(_Code, Class.Error));
        }

        public void Error(Exception error)
        {
            Errors.Add(GetMessage(error.Message, Class.Error));
        }

        #endregion

        #region Danger
        public void Danger(string _Text)
        {
            Errors.Add(GetMessage(_Text, Class.Danger));
        }
        public void Danger(Messages _Code)
        {
            Errors.Add(GetMessage(_Code, Class.Danger));

        }
        #endregion

        #region Critical

        public void Critical(string _Text)
        {
            Errors.Add(GetMessage(_Text, Class.Critical));
        }

        public void Critical(Messages _Code)
        {
            Errors.Add(GetMessage(_Code, Class.Critical));
        }
        #endregion

        #region Get Single message or error
        public Message GetMessage(Messages _Code, Class _Class)
        {
            var _Message = new Message();

            if (MsgConnection != null)
            {
                try
                {
                    if (MsgConnection.State != ConnectionState.Open) { MsgConnection.Open(); }
                    var _Query = "SELECT * FROM Messages WHERE [Code] = @Code AND [Language] = @Language";
                    using var _Command = new SqliteCommand(_Query, MsgConnection);

                    _Command.Parameters.AddWithValue("@Code", _Code.ToString());
                    _Command.Parameters.AddWithValue("@Language", LanguageID);

                    using var reader = _Command.ExecuteReader();

                    if (reader.Read())
                    {
                        _Message.Code = reader["Code"]?.ToString() ?? "";
                        _Message.MessageText = reader["MessageText"]?.ToString() ?? "";
                        _Message.MessageClass = (Class)Convert.ToInt64(reader["Class"]);
                        _Message.MessageID = Convert.ToInt64(reader["ID"]);
                    }

                    else
                    {
                        _Message.Code = _Code.ToString();
                        _Message.MessageText = $"{_Code} : Not Found in Message List";
                        _Message.MessageClass = Class.Error;
                        _Message.MessageID = -1;

                        //_ = InsertDB(_Message); // Add the message to the database if not found
                    }
                }
                catch (Exception)
                {
                }
            }
            return _Message;
        }
        public Message GetMessage(string _Text, Class _Class)
        {
            var _Message = new Message(); ;
            _Message.Code = "NoCode";
            _Message.MessageText = _Text;
            _Message.MessageClass = _Class;
            return _Message;
        }
        public Message GetMessage(string _Text)
        {
            var _Message = new Message(); ;
            _Message.Code = "NoCode";
            _Message.MessageText = _Text;
            _Message.MessageClass = Class.Alert;
            return _Message;
        }
        #endregion

        #region Add Range of Messages
        public void AddRange(List<Message> messageList)
        {
            if (messageList.Count > 0)
            {
                // Create a copy to avoid modifying the collection being iterated
                var messagesToAdd = messageList.ToList();

                foreach (Message message in messagesToAdd)
                {
                    MessageList.Add(message);
                }
            }
        }

        public void AddRange(MessageClass messageClass)
        {
            if (messageClass.MessageList.Count > 0)
            {
                // If they're the same list, we need to copy first
                if (object.ReferenceEquals(messageClass.MessageList, MessageList))
                {
                    // Create a copy of the source before adding
                    var messagesToAdd = messageClass.MessageList.ToList();
                    foreach (Message message in messagesToAdd)
                    {
                        MessageList.Add(message);
                    }
                }
                else
                {
                    foreach (Message message in messageClass.MessageList)
                    {
                        MessageList.Add(message);
                    }
                }
            }
        }

        #endregion

        #region Add Message if not in DB rows

        public bool InsertDB(string _Code, string _Text, Class _Class)
        {

            if(MsgConnection == null) { return false; }
            if (string.IsNullOrEmpty(_Code) || string.IsNullOrEmpty(_Text)) { return false; }

            var IsExist = GetMessage(_Code, _Class);
            if (IsExist.Code == "NoCode")
            {
                string _Query = "INSERT [MessageText] VALUES (@ID, @Key, @Language, @TextValue, @Section)";
                SqliteCommand _Command = new SqliteCommand(_Query, MsgConnection);
                _Command.Parameters.AddWithValue("@ID", -1);
                _Command.Parameters.AddWithValue("@Key", _Code);
                _Command.Parameters.AddWithValue("@Language", LanguageID);
                _Command.Parameters.AddWithValue("@TextValue", _Text);
                _Command.Parameters.AddWithValue("@Section", _Class.ToString());
                _Command.ExecuteNonQuery();
            }
            return true;
        }

        public bool InsertDB(Message message)
        {
            InsertDB(message.Code, message.MessageText, message.MessageClass);
            return true;
        }

        public bool InsertDB(string messageText)
        {
            InsertDB(Guid.NewGuid().ToString(), messageText, Class.Error);
            return true;
        }
        #endregion

        #region Get Error Message 
        public static Message GetError(string text)
        {
            Message message = new Message();
            message.Code = "NoCode";
            message.MessageText = text;
            message.MessageClass = Class.Error;

            return message;
        }

        public static Message ErrorMessage(Exception error)
        {
            Message message = new Message()
            {
                Code = error.ToString(),
                MessageText = error.Message,
                MessageClass = Class.Error

            };
            return message;
        }

        #endregion
    }
}
