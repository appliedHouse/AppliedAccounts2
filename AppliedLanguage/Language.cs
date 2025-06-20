﻿using System.Data;

namespace AppLanguages
{
    public class Language
    {
        //private DataTable? LanguageDataTable { get; set; }
        public readonly List<DataRow> Languages = new LanguageList().LangList;
        public int LanguagesCount => Languages.Count;
        public List<DataRow> LanguageText { get; set; } = new();

        public int LanguageID { get; set; }
        public string Section { get; set; } = string.Empty;


        public Language(int _LanguageID, string _Section)
        {
            LanguageText = GetLanguageText(_LanguageID, _Section);
            Languages = GetLanguageList();
        }

        private static List<DataRow> GetLanguageText(int _LanguageID, string _Section)
        {
            List<DataRow> _List = new();
            DataTable _Table = new AppliedDB.Languages().GetLanguageText(_LanguageID, _Section);

            if (_Table.Rows.Count > 0)
            {
                foreach (DataRow _Row in _Table.Rows)
                {
                    _List.Add(_Row);
                }
                return _List;
            }
            return null;
        }

        private static List<DataRow> GetLanguageList()
        {
            List<DataRow> _List = new();
            DataTable _Table = AppliedDB.Languages.GetLanguageList();

            if (_Table.Rows.Count > 0)
            {
                foreach (DataRow _Row in _Table.Rows)
                {
                    _List.Add(_Row);
                }
                return _List;
            }
            return null;
        }


        public string GetValue(string _Key)
        {
            return LanguageText.Where(Row => (string)Row["Key"] == _Key.Trim()).Select(Row => (string)Row["TextValue"]).FirstOrDefault() ?? "No Value.";
        }
    }
}

