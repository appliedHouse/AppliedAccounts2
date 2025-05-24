using System.Data;
using static AppMessages.Enums;
//using Messages = AppMessages.Message ;

namespace AppMessages
{
    public class MessageClass
    {
        #region Variables
        public Message MyMessage { get; set; }
        public List<Message> MessageList { get; set; } = [];
        public DataTable MessagesTable { get; set; } = new();
        public Message Empty { get; set; } = new();
        public int Count => MessageList.Count;
        #endregion

        #region Constructor
        public MessageClass()
        {


        }
        public MessageClass(DataTable _Table)
        {
            MessagesTable = _Table;
        }
        #endregion

        #region Clear Message / Error List
        public void ClearMessages() { MessageList.Clear(); }
        #endregion

        #region Add Message in the List
        public void Add(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code));
        }
        public void Add(string _Text)
        {
            MessageList.Add(GetMessage(_Text));
        }
        public void Add(Messages _Code, Class _Class)
        {
            MessageList.Add(GetMessage(_Code, _Class));
        }
        public void Add(string _Text, Class _Class)
        {
            MessageList.Add(GetMessage(_Text, _Class));
        }
        #endregion

        #region Error
        public void Error(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Error));
        }

        public void Error(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Error));
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
            MessageList.Add(GetMessage(_Text, Class.Warrning));
        }
        public void Warning(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Warrning));
        }
        #endregion

        #region Danger
        public void Danger(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Danger));
        }
        public void Danger(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Danger));

        }
        #endregion

        #region Critical

        public void Critical(string _Text)
        {
            MessageList.Add(GetMessage(_Text, Class.Critical));
        }

        public void Critical(Messages _Code)
        {
            MessageList.Add(GetMessage(_Code, Class.Critical));
        }
        #endregion

        #region Get Single message or error
        public Message GetMessage(Messages _Code, Class _Class)
        {
            var _Message = new Message(); ;
            if (MessagesTable is not null)
            {
                if (MessagesTable.Rows.Count > 0)
                {
                    _Message.Code = _Code.ToString() ?? "---";
                    MessagesTable.DefaultView.RowFilter = $"Code='{_Message.Code}'";
                    if (MessagesTable.DefaultView.Count == 1)
                    {
                        DataRow _Row = MessagesTable.DefaultView[0].Row;
                        _Message.MessageID = (int)_Row["ID"];
                        _Message.MessageText = (string)_Row["MessageText"];
                        _Message.MessageClass = _Class;
                        _Message.MessageDate = DateTime.Now;
                        return _Message;
                    }
                }
            }
            _Message.MessageText += " " + _Code.ToString();
            return _Message; ;
        }

        public Message GetMessage(Messages _Code)
        {
            var _Message = new Message(); ;
            if (MessagesTable is not null)
            {
                if (MessagesTable.Rows.Count > 0)
                {
                    _Message.Code = _Code.ToString();
                    MessagesTable.DefaultView.RowFilter = $"Code='{_Message.Code}'";
                    if (MessagesTable.DefaultView.Count == 1)
                    {
                        DataRow _Row = MessagesTable.DefaultView[0].Row;
                        _Message.MessageID = (int)_Row["ID"];
                        _Message.MessageText = (string)_Row["MessageText"];
                        _Message.MessageClass = (Class)_Row["Class"];
                        _Message.MessageDate = DateTime.Now;
                        return _Message;
                    }
                }
            }
            _Message.MessageText += " " + _Code.ToString();
            return _Message; ;
        }

        public static Message GetMessage(string _Text)
        {
            return new Message()
            {
                MessageID = -1,
                MessageClass = Class.Error,
                MessageDate = DateTime.Now,
                MessageText = _Text
            };
        }

        public static Message GetMessage(string _Text, Class _Class)
        {
            return new Message()
            {
                MessageID = -1,
                MessageClass = _Class,
                MessageDate = DateTime.Now,
                MessageText = _Text
            };
        }

        public void Add(object classIsNull)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
