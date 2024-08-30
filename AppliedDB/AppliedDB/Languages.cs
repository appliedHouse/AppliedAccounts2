﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AppliedDB
{
    public class Languages
    {
        public SQLiteConnection LangConnection = Connections.GetLanguageConnection() ?? new();
        public DataTable GetLanguageList()
        {
            var _Connection = Connections.GetLanguageConnection();
            if (_Connection is not null)
            {
                // SELECT * FROM [LanguageList]
                _Connection.Open();
                var _Command = new SQLiteCommand($"SELECT * FROM [LanguageList]", _Connection);
                var _Adapter = new SQLiteDataAdapter(_Command);
                var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, "LanguageList");
                _Connection.Close();
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }
            }
            return null;

        }

        public DataTable GetLanguageText(int _Language, string _Section)
        {
            var _Connection = Connections.GetLanguageConnection();
            if (_Connection is not null)
            {
                // SELECT * FROM [Language records of specific language ]
                _Connection.Open();
                var _Filter = $"WHERE Language={_Language} AND Section IN ('{_Section}','Common')";
                var _Command = new SQLiteCommand($"SELECT * FROM [LanguageText] {_Filter}", _Connection);
                var _Adapter = new SQLiteDataAdapter(_Command);
                var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, "LanguageText");
                _Connection.Close();
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }
            }
            return null;

        }

    }
}
