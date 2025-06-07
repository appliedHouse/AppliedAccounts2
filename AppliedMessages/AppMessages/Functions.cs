using System.Data;
using static AppMessages.Enums;

namespace AppMessages
{
    public class Functions
    {

        public static Message GetMessage(DataTable _Table, Message _Message)
        {
            if (_Table is not null)
            {
                DataView _View = _Table.AsDataView();

                _View.RowFilter = $"Code='{_Message.Code}'";
                if (_View.Count == 1)
                {
                    DataRow _Row = _View[0].Row;
                    _Message.MessageID = (int)_Row["ID"];
                    _Message.MessageText = (string)_Row["MessageText"];
                    _Message.MessageClass = (Class)_Row["Class"];
                    _Message.MessageDate = DateTime.Now;
                }
                return _Message;
            }
            return Message.NotFound();
        }

        public static Message NewMessage() { return new Message(); }
    }
}
