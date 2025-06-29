﻿using AppliedDB;
using AppMessages;
using System.Data;
using Format = AppliedGlobals.AppValues.Format;

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


            // _Message = (new MessageClass()).Empty;

            try
            {
                _Table.DefaultView.RowFilter = $"ID={_ID}";
                if (_Table.DefaultView.Count == 1)
                {
                    var _Command = Commands.UpDate(_Row, DataFile);
                    if (_Command is not null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        //_Message = new(Messages.Save);
                        //Message.RowEffected = _Effected;
                    }
                    else
                    {
                        //_Message = MessageClass.GetMessage(Messages.cmd_UpdateNull);
                    }
                }

                if (_Table.DefaultView.Count == 0)
                {
                    var _Command = Commands.Insert(_Row, DataFile);
                    if (_Command is not null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        //_Message = MessageClass.Messages.GetMessage(Messages.RowInserted);
                        //_Message.RowEffected = _Effected;
                    }
                    else
                    {
                        //_Message = MessageClass.GetMessage(Messages.cmd_InsertNull);
                    }

                }
            }
            catch (Exception)
            {
                //_Message = MessageClass.GetMessage(Messages.NotSave);
            }

            return new Message();
        }

        public static Message Delete(string DataFile, DataTable _Table, DataRow _Row)
        {
            var _ID = (int)_Row["ID"];
            var _Effected = 0;
            //Message _Message = new();

            try
            {
                _Table.DefaultView.RowFilter = $"ID={_ID}";
                if (_Table.DefaultView.Count == 1)
                {
                    var _Command = Commands.Delete(_Row, DataFile);
                    if (_Command != null)
                    {
                        _Effected = _Command.ExecuteNonQuery();
                        //_Message = MessageClass.GetMessage(Messages.RowDeleted);
                        //_Message.RowEffected = _Effected;
                    }
                    else
                    {
                        //_Message = MessageClass.GetMessage(Messages.cmd_DeleteNull);
                    }
                }
            }
            catch (Exception)
            {
                //_Message = MessageClass.GetMessage(Messages.NotSave);
            }

            return new Message(); //_Message;

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


        public static object RemoveNull(object _Value)
        {
            var _Type = _Value.GetType();


            if (_Value == null || _Value == DBNull.Value)
            {
                if (_Type == typeof(string))
                {
                    return string.Empty;
                }
                else if (_Type == typeof(int) || _Type == typeof(long) || _Type == typeof(short))
                {
                    return 0;
                }
                else if (_Type == typeof(decimal) || _Type == typeof(double) || _Type == typeof(float))
                {
                    return 0.0;
                }
                else if (_Type == typeof(DateTime))
                {
                    return DateTime.MinValue;
                }

                return string.Empty;
            }
            return _Value;
        }

    }


}
