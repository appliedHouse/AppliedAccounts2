using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using static AppMessages.Enums;

namespace AppMessages
{
    /// <summary>
    /// Holds the current messages (MessageList) and errors (Errors) and looks up
    /// message text from the Messages.db SQLite file.
    ///
    /// Conventions:
    ///   Errors      : Error, Danger, Critical
    ///   MessageList : Success, Warning, Alert
    ///   Blocking    : anything in Errors, plus Alert items (used for validation failures)
    /// </summary>
    public class MessageClass
    {
        #region Cache (shared by all instances, loaded once per DB + language)

        private static readonly ConcurrentDictionary<string, Message> _cache = new();
        private static readonly ConcurrentDictionary<string, bool> _loaded = new();
        private static readonly object _loadLock = new();

        #endregion

        #region Variables
        private readonly object _sync = new();

        public List<Message> MessageList { get; set; } = [];
        public List<Message> Errors { get; set; } = [];
        public Message Empty { get; set; } = new();
        public SqliteConnection? MsgConnection { get; set; }
        public long LanguageID { get; set; } = 1;             // Default Language English, Id = 1

        /// <summary>Last problem found while reading or writing the messages DB (null if none).</summary>
        public string? DbError { get; private set; }

        /// <summary>Every item, including success and warnings.</summary>
        public int Count { get { lock (_sync) return MessageList.Count + Errors.Count; } }
        public int CountError { get { lock (_sync) return Errors.Count; } }
        public int CountMessages { get { lock (_sync) return MessageList.Count; } }

        public bool HasErrors => CountError > 0;
        public bool HasMessages => CountMessages > 0;

        /// <summary>
        /// True when something should stop a save: any error, or any Alert.
        /// Success and Warning messages do not block.
        /// </summary>
        public bool HasBlocking
        {
            get
            {
                lock (_sync)
                {
                    return Errors.Count > 0 || MessageList.Any(m => m.MessageClass == Class.Alert);
                }
            }
        }
        #endregion

        #region Constructor
        public MessageClass() { }

        public MessageClass(SqliteConnection msgConnection)
        {
            MsgConnection = msgConnection;
        }

        public MessageClass(SqliteConnection msgConnection, long languageID)
        {
            MsgConnection = msgConnection;
            LanguageID = languageID;
        }
        #endregion

        #region Add helpers (thread-safe)
        private void AddToMessages(Message message) { lock (_sync) MessageList.Add(message); }
        private void AddToErrors(Message message) { lock (_sync) Errors.Add(message); }
        #endregion

        #region Clear
        public void ClearMessages()
        {
            lock (_sync)
            {
                MessageList.Clear();
                Errors.Clear();
            }
        }

        public void Clear() => ClearMessages();
        #endregion

        #region Add Message in the List
        public void Add(Messages code) => AddToMessages(GetMessage(code, Class.Alert));
        #endregion

        #region Success
        public void Success(string text) => AddToMessages(GetMessage(text, Class.Success));
        public void Success(Messages code) => AddToMessages(GetMessage(code, Class.Success));
        #endregion

        #region Alert
        public void Alert(string text) => AddToMessages(GetMessage(text, Class.Alert));
        public void Alert(Messages code) => AddToMessages(GetMessage(code, Class.Alert));
        #endregion

        #region Warning
        public void Warning(string text) => AddToMessages(GetMessage(text, Class.Warning));
        public void Warning(Messages code) => AddToMessages(GetMessage(code, Class.Warning));
        #endregion

        #region Error
        public void Error(string text) => AddToErrors(GetMessage(text, Class.Error));
        public void Error(Messages code) => AddToErrors(GetMessage(code, Class.Error));
        public void Error(Exception error) => AddToErrors(ErrorMessage(error));
        #endregion

        #region Danger
        public void Danger(string text) => AddToErrors(GetMessage(text, Class.Danger));
        public void Danger(Messages code) => AddToErrors(GetMessage(code, Class.Danger));
        #endregion

        #region Critical
        public void Critical(string text) => AddToErrors(GetMessage(text, Class.Critical));
        public void Critical(Messages code) => AddToErrors(GetMessage(code, Class.Critical));
        #endregion

        #region Get Single message or error
        /// <summary>
        /// Looks the message text up by code. The severity shown is the one the caller asked for
        /// (_class), not whatever is stored in the DB row, so the list a message goes into
        /// and the severity displayed always agree.
        /// Never returns an empty message: if the DB or the code is missing, the code name is used.
        /// </summary>
        public Message GetMessage(Messages code, Class messageClass)
        {
            var name = code.ToString();

            if (MsgConnection == null)
            {
                return new Message { Code = name, MessageText = name, MessageClass = messageClass, MessageID = -1 };
            }

            EnsureLoaded();

            if (_cache.TryGetValue(CacheKey(name), out var found))
            {
                return new Message
                {
                    Code = found.Code,
                    MessageText = found.MessageText,
                    MessageClass = messageClass,
                    MessageID = found.MessageID
                };
            }

            return new Message
            {
                Code = name,
                MessageText = DbError is null
                    ? $"{name} : Not Found in Message List"
                    : $"{name} : message lookup failed ({DbError})",
                MessageClass = messageClass,
                MessageID = -1
            };
        }

        public Message GetMessage(string text, Class messageClass)
        {
            return new Message { Code = "NoCode", MessageText = text, MessageClass = messageClass };
        }

        public Message GetMessage(string text) => GetMessage(text, Class.Alert);
        #endregion

        #region Add Range of Messages
        public void AddRange(List<Message> messages)
        {
            if (messages == null || messages.Count == 0) { return; }
            var copy = messages.ToList();               // safe even if it is our own list
            lock (_sync) MessageList.AddRange(copy);
        }

        public void AddRange(MessageClass other)
        {
            if (other is null || ReferenceEquals(other, this)) { return; }

            List<Message> messages, errors;
            lock (other._sync)
            {
                messages = other.MessageList.ToList();
                errors = other.Errors.ToList();
            }

            lock (_sync)
            {
                MessageList.AddRange(messages);
                Errors.AddRange(errors);
            }
        }
        #endregion

        #region Message DB (cache load and insert)
        private string CacheKey(string code) => $"{MsgConnection?.ConnectionString}|{LanguageID}|{code}";

        private void EnsureLoaded()
        {
            if (MsgConnection == null) { return; }

            var loadKey = $"{MsgConnection.ConnectionString}|{LanguageID}";
            if (_loaded.ContainsKey(loadKey)) { return; }

            lock (_loadLock)
            {
                if (_loaded.ContainsKey(loadKey)) { return; }

                try
                {
                    // Own short-lived connection: the shared one is never opened or left open.
                    using var conn = new SqliteConnection(MsgConnection.ConnectionString);
                    conn.Open();

                    using var cmd = new SqliteCommand(
                        "SELECT ID, Code, MessageText, Class FROM Messages WHERE [Language] = @Language", conn);
                    cmd.Parameters.AddWithValue("@Language", LanguageID);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var code = reader["Code"]?.ToString() ?? "";
                        if (code.Length == 0) { continue; }

                        _cache[$"{loadKey}|{code}"] = new Message
                        {
                            Code = code,
                            MessageText = reader["MessageText"]?.ToString() ?? "",
                            MessageClass = reader.IsDBNull(3) ? Class.Alert : (Class)Convert.ToInt64(reader["Class"]),
                            MessageID = reader.IsDBNull(0) ? -1 : Convert.ToInt64(reader["ID"])
                        };
                    }
                }
                catch (Exception ex)
                {
                    DbError = ex.Message;
                }
                finally
                {
                    _loaded[loadKey] = true;    // do not retry on every message
                }
            }
        }

        /// <summary>Forces the next lookup to reload the messages from the DB.</summary>
        public void ReloadCache()
        {
            if (MsgConnection == null) { return; }
            var prefix = $"{MsgConnection.ConnectionString}|{LanguageID}";
            foreach (var key in _cache.Keys.Where(k => k.StartsWith(prefix + "|")).ToList())
            {
                _cache.TryRemove(key, out _);
            }
            _loaded.TryRemove(prefix, out _);
            DbError = null;
        }

        /// <summary>
        /// Adds a message to the DB if the code does not exist yet.
        /// Assumes Messages(ID auto-generated, Code, MessageText, Class, Language).
        /// </summary>
        public bool InsertDB(string code, string text, Class messageClass)
        {
            if (MsgConnection == null) { return false; }
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(text)) { return false; }

            EnsureLoaded();
            if (_cache.ContainsKey(CacheKey(code))) { return true; }

            try
            {
                using var conn = new SqliteConnection(MsgConnection.ConnectionString);
                conn.Open();

                using var cmd = new SqliteCommand(
                    "INSERT INTO Messages (Code, MessageText, Class, [Language]) VALUES (@Code, @Text, @Class, @Language)",
                    conn);
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@Text", text);
                cmd.Parameters.AddWithValue("@Class", Convert.ToInt64(messageClass));
                cmd.Parameters.AddWithValue("@Language", LanguageID);
                cmd.ExecuteNonQuery();

                _cache[CacheKey(code)] = new Message
                {
                    Code = code,
                    MessageText = text,
                    MessageClass = messageClass,
                    MessageID = -1
                };
                return true;
            }
            catch (Exception ex)
            {
                DbError = ex.Message;
                return false;
            }
        }

        public bool InsertDB(Message message) =>
            InsertDB(message.Code, message.MessageText, message.MessageClass);

        /// <summary>Inserts a text message using a stable code derived from the text (no duplicates).</summary>
        public bool InsertDB(string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText)) { return false; }
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(messageText)))[..12];
            return InsertDB($"Msg_{hash}", messageText, Class.Error);
        }
        #endregion

        #region Get Error Message
        public static Message GetError(string text) =>
            new() { Code = "NoCode", MessageText = text, MessageClass = Class.Error };

        /// <summary>Short code (exception type) so the UI never shows a stack trace.</summary>
        public static Message ErrorMessage(Exception error) =>
            new() { Code = error.GetType().Name, MessageText = error.Message, MessageClass = Class.Error };
        #endregion
    }
}

//using Microsoft.Data.Sqlite;
//using System.Data;
//using AppliedGlobals;
//using static AppMessages.Enums;

//namespace AppMessages
//{
//    public class MessageClass
//    {
//        #region Variables
//        public AppValues AppGlobal { get; set; }
//        public Message MyMessage { get; set; }
//        public List<Message> MessageList { get; set; } = [];
//        public List<Message> Errors { get; set; } = [];
//        public DataTable MessagesTable { get; set; }
//        public Message Empty { get; set; } = new();
//        public int Count => MessageList.Count + Errors.Count;
//        public int CountError => Errors.Count;
//        public int CountMessages => MessageList.Count;
//        public SqliteConnection? MsgConnection { get; set; }
//        public long LanguageID { get; set; } = 1;             // Default Language English, Id = 1
//        public string FilePath { get; set; }

//        #endregion

//        #region Constructor
//        public MessageClass()
//        {
//            LanguageID = 1;             // Default Language English, Id = 1
//        }

//        public MessageClass(SqliteConnection msgConnection)
//        {
//            MsgConnection = msgConnection;
//            LanguageID = 1;             // Default Language English, Id = 1
//        }
//        public MessageClass(SqliteConnection msgConnection, long _LanguageID)
//        {
//            MsgConnection = msgConnection;
//            LanguageID = _LanguageID;
//        }
//        #endregion

//        #region Clear Message / Error List
//        public void ClearMessages()
//        {
//            MessageList.Clear();
//            Errors.Clear();
//        }
//        #endregion

//        #region Add Message in the List
//        public void Add(Messages _Code)
//        {
//            MessageList.Add(GetMessage(_Code, Class.Alert));
//        }

//        #endregion

//        #region Success
//        public void Success(string _Text)
//        {
//            MessageList.Add(GetMessage(_Text, Class.Success));
//        }

//        public void Success(Messages _Code)
//        {
//            MessageList.Add(GetMessage(_Code, Class.Success));
//        }
//        #endregion

//        #region Alert
//        public void Alert(string _Text)
//        {
//            MessageList.Add(GetMessage(_Text, Class.Alert));
//        }

//        public void Alert(Messages _Code)
//        {
//            MessageList.Add(GetMessage(_Code, Class.Alert));
//        }
//        #endregion

//        #region Warrning
//        public void Warning(string _Text)
//        {
//            MessageList.Add(GetMessage(_Text, Class.Warning));
//        }
//        public void Warning(Messages _Code)
//        {
//            MessageList.Add(GetMessage(_Code, Class.Warning));
//        }
//        #endregion

//        #region Error
//        public void Error(string _Text)
//        {
//            Errors.Add(GetMessage(_Text, Class.Error));
//        }

//        public void Error(Messages _Code)
//        {
//            Errors.Add(GetMessage(_Code, Class.Error));
//        }

//        public void Error(Exception error)
//        {
//            Errors.Add(GetMessage(error.Message, Class.Error));
//        }

//        #endregion

//        #region Danger
//        public void Danger(string _Text)
//        {
//            Errors.Add(GetMessage(_Text, Class.Danger));
//        }
//        public void Danger(Messages _Code)
//        {
//            Errors.Add(GetMessage(_Code, Class.Danger));

//        }
//        #endregion

//        #region Critical

//        public void Critical(string _Text)
//        {
//            Errors.Add(GetMessage(_Text, Class.Critical));
//        }

//        public void Critical(Messages _Code)
//        {
//            Errors.Add(GetMessage(_Code, Class.Critical));
//        }
//        #endregion

//        #region Get Single message or error
//        public Message GetMessage(Messages _Code, Class _Class)
//        {
//            var _Message = new Message();

//            if (MsgConnection != null)
//            {
//                try
//                {
//                    if (MsgConnection.State != ConnectionState.Open) { MsgConnection.Open(); }
//                    var _Query = "SELECT * FROM Messages WHERE [Code] = @Code AND [Language] = @Language";
//                    using var _Command = new SqliteCommand(_Query, MsgConnection);

//                    _Command.Parameters.AddWithValue("@Code", _Code.ToString());
//                    _Command.Parameters.AddWithValue("@Language", LanguageID);

//                    using var reader = _Command.ExecuteReader();

//                    if (reader.Read())
//                    {
//                        _Message.Code = reader["Code"]?.ToString() ?? "";
//                        _Message.MessageText = reader["MessageText"]?.ToString() ?? "";
//                        _Message.MessageClass = (Class)Convert.ToInt64(reader["Class"]);
//                        _Message.MessageID = Convert.ToInt64(reader["ID"]);
//                    }

//                    else
//                    {
//                        _Message.Code = _Code.ToString();
//                        _Message.MessageText = $"{_Code} : Not Found in Message List";
//                        _Message.MessageClass = Class.Error;
//                        _Message.MessageID = -1;

//                        //_ = InsertDB(_Message); // Add the message to the database if not found
//                    }
//                }
//                catch (Exception)
//                {
//                }
//            }
//            return _Message;
//        }
//        public Message GetMessage(string _Text, Class _Class)
//        {
//            var _Message = new Message(); ;
//            _Message.Code = "NoCode";
//            _Message.MessageText = _Text;
//            _Message.MessageClass = _Class;
//            return _Message;
//        }
//        public Message GetMessage(string _Text)
//        {
//            var _Message = new Message(); ;
//            _Message.Code = "NoCode";
//            _Message.MessageText = _Text;
//            _Message.MessageClass = Class.Alert;
//            return _Message;
//        }
//        #endregion

//        #region Add Range of Messages
//        public void AddRange(List<Message> messageList)
//        {
//            if (messageList.Count > 0)
//            {
//                // Create a copy to avoid modifying the collection being iterated
//                var messagesToAdd = messageList.ToList();

//                foreach (Message message in messagesToAdd)
//                {
//                    MessageList.Add(message);
//                }
//            }
//        }

//        public void AddRange(MessageClass messageClass)
//        {
//            if (messageClass.MessageList.Count > 0)
//            {
//                // If they're the same list, we need to copy first
//                if (object.ReferenceEquals(messageClass.MessageList, MessageList))
//                {
//                    // Create a copy of the source before adding
//                    var messagesToAdd = messageClass.MessageList.ToList();
//                    foreach (Message message in messagesToAdd)
//                    {
//                        MessageList.Add(message);
//                    }
//                }
//                else
//                {
//                    foreach (Message message in messageClass.MessageList)
//                    {
//                        MessageList.Add(message);
//                    }
//                }
//            }
//        }

//        #endregion

//        #region Add Message if not in DB rows

//        public bool InsertDB(string _Code, string _Text, Class _Class)
//        {

//            if(MsgConnection == null) { return false; }
//            if (string.IsNullOrEmpty(_Code) || string.IsNullOrEmpty(_Text)) { return false; }

//            var IsExist = GetMessage(_Code, _Class);
//            if (IsExist.Code == "NoCode")
//            {
//                string _Query = "INSERT [MessageText] VALUES (@ID, @Key, @Language, @TextValue, @Section)";
//                SqliteCommand _Command = new SqliteCommand(_Query, MsgConnection);
//                _Command.Parameters.AddWithValue("@ID", -1);
//                _Command.Parameters.AddWithValue("@Key", _Code);
//                _Command.Parameters.AddWithValue("@Language", LanguageID);
//                _Command.Parameters.AddWithValue("@TextValue", _Text);
//                _Command.Parameters.AddWithValue("@Section", _Class.ToString());
//                _Command.ExecuteNonQuery();
//            }
//            return true;
//        }

//        public bool InsertDB(Message message)
//        {
//            InsertDB(message.Code, message.MessageText, message.MessageClass);
//            return true;
//        }

//        public bool InsertDB(string messageText)
//        {
//            InsertDB(Guid.NewGuid().ToString(), messageText, Class.Error);
//            return true;
//        }
//        #endregion

//        #region Get Error Message 
//        public static Message GetError(string text)
//        {
//            Message message = new Message();
//            message.Code = "NoCode";
//            message.MessageText = text;
//            message.MessageClass = Class.Error;

//            return message;
//        }

//        public static Message ErrorMessage(Exception error)
//        {
//            Message message = new Message()
//            {
//                Code = error.ToString(),
//                MessageText = error.Message,
//                MessageClass = Class.Error

//            };
//            return message;
//        }

//        #endregion
//    }
//}
