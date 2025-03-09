using static AppMessages.Enums;

namespace AppMessages
{
    public class Message
    {
        public Message() { }
        public Message(string _Code)
        {
            Code = _Code;
        }

        public Message(string _Code, string _WebPage, string _Method, string _User)
        {
            Code = _Code;
            WebPage = _WebPage;
            Method = _Method;
            User = _User;
        }
        public static Message NotFound()
        {
            var _Message = new Message()
            {
                MessageID = -1,
                Code = "NoMsg",
                RowEffected = 0,
                MessageText = "No Message Found",
                MessageClass = Class.Critical,
                MessageDate = DateTime.Now,
                WebPage = "",
                Method = "",
                User = ""
            };

            return _Message;
        }

        #region Message Body
        public int MessageID { get; set; } = 0;
        public string Code { get; set; } = string.Empty;
        public int RowEffected { get; set; } = 0;
        public string MessageText { get; set; } = "Empty...";
        public Class MessageClass { get; set; } = new();
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public string WebPage { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        #endregion
    }
}

