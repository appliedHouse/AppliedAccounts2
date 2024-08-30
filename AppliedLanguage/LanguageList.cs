using AppliedDB;
using System.Data;

namespace AppLanguages
{

    public interface ILanguageList
    {
        public List<DataRow> LangList { get; set; }
    }

    public class LanguageList
    {
        public List<DataRow> LangList { get; set; } = new();

        public LanguageList()
        {
            GetLanguageList();
        }

        public static List<DataRow> GetLanguageList()
        {
            List<DataRow> _List = new();
            DataTable _Table = new Languages().GetLanguageList();

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
    }
}
