using System.Data;
using static AppMessages.Enums;
//using Messages = AppMessages.Message ;

namespace AppMessages
{
    public class AppMessages
    {
        public List<Message> MyMessages { get; set; } = new();
        //public Message MyMessage { get; set; } = new();
        public DataTable MessagesTable { get; set; } = new();

        public int Count => MyMessages.Count;

        #region Constructor
        public AppMessages()
        {


        }
        public AppMessages(DataTable _Table)
        {
            MessagesTable = _Table;
        }
        #endregion

        #region Clear Message / Error List
        //public void ClearErrors() { MyErrors.Clear(); }
        public void ClearMessages() { MyMessages.Clear(); }
        #endregion

        #region Add Message in the List

        public void Add(Messages _Code)
        {
            MyMessages.Add(GetMessage(_Code));
        }

        public void Add(string _Text)
        {
            MyMessages.Add(GetMessage(_Text));
        }

        

        #endregion

        #region Get Single message or error
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

        private Message GetMessage(string _Text)
        {
            return new Message()
            {
                MessageID = -1,
                MessageClass = Class.Error,
                MessageDate = DateTime.Now,
                MessageText = _Text
            };
        }
        #endregion
    }
}
