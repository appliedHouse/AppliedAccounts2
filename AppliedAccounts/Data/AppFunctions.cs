using AppliedDB;
using AppMessages;
using System.Data;
using Messages = AppMessages.Enums.Messages;

namespace AppliedAccounts.Data
{
    public static class AppFunctions
    {
        public static string GetRootPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public static Message Save(string DataFile, DataTable _Table, DataRow _Row)
        {
            var _ID = (int)_Row["ID"];
            int _Effected;


            Message _Message = MessageClass.NewMessage;

            try
            {
                _Table.DefaultView.RowFilter = $"ID={_ID}";
                if (_Table.DefaultView.Count == 1)
                {
                    var _Command = Commands.UpDate(_Row, DataFile);
                    if (_Command is not null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        _Message = MessageClass.Messages.GetMessage(Messages.Save);
                        _Message.RowEffected = _Effected;
                    }
                    else
                    {
                        _Message = MessageClass.GetMessage(Messages.cmd_UpdateNull);
                    }
                }

                if (_Table.DefaultView.Count == 0)
                {
                    var _Command = Commands.Insert(_Row, DataFile);
                    if (_Command is not null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        _Message = MessageClass.Messages.GetMessage(Messages.RowInserted);
                        _Message.RowEffected = _Effected;
                    }
                    else
                    {
                        _Message = MessageClass.GetMessage(Messages.cmd_InsertNull);
                    }

                }
            }
            catch (Exception)
            {
                _Message = MessageClass.GetMessage(Messages.NotSave);
            }

            return _Message;
        }

        public static Message Delete(string DataFile, DataTable _Table, DataRow _Row)
        {
            var _ID = (int)_Row["ID"];
            var _Effected = 0;
            Message _Message = new();

            try
            {
                _Table.DefaultView.RowFilter = $"ID={_ID}";
                if (_Table.DefaultView.Count == 1)
                {
                    var _Command = Commands.Delete(_Row, DataFile);
                    if (_Command != null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        _Message = MessageClass.GetMessage(Messages.RowDeleted);
                        _Message.RowEffected = _Effected;
                    }
                    else
                    {
                        _Message = MessageClass.GetMessage(Messages.cmd_DeleteNull);
                    }
                }
            }
            catch (Exception)
            {
                _Message = MessageClass.GetMessage(Messages.NotSave);
            }

            return _Message;

        }

        public static string? Date2Text(object _DateTime)
        {
            var _Format = Format.DDMMYY;

            if (_DateTime.GetType() == typeof(DateTime))
            {
                return ((DateTime)_DateTime).ToString(_Format);
            }

            return _DateTime.ToString();

        }

        public static string ReportFooter()
        {
            return "Powered by Applied Software House.";

        }


    }

    public enum downloadOption
    {
        displayPDF,
        downloadFile
    }
}
