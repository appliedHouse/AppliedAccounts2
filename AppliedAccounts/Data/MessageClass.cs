﻿using AppliedDB;
using AppMessages;

namespace AppliedAccounts.Data
{
    public class MessageClass
    {
        public static AppMessages.AppMessages Messages => new AppMessages.AppMessages(DataSource.Messages());
        public static Message NewMessage => AppMessages.Functions.NewMessage();
        public static Message GetMessage(AppMessages.Enums.Messages Code)
        {
            var _Messages = new AppMessages.AppMessages(DataSource.Messages());
            return _Messages.GetMessage(Code);
        }

        public List<Message> MyMessages { get; set; }

        public void Add(Message _Message)
        {
            MyMessages.Add(_Message);
        }

        internal void Add(AppMessages.Enums.Messages accClassZero)
        {
            
        }
    }
}
