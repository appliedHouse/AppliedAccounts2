using System.Data;
using AppliedGlobals;
using static AppliedDB.Enums;

namespace AppliedDB
{
    interface IGetDataRow{
        DataRow? GetRow(int id);
    }


    public class GetDataRow : IGetDataRow 
    {
        public DataRow? _DataRow;
        public GetDataRow(AppValues.AppPath _AppPaths, Tables _Tables, int _ID)
        {
            DataSource _Source = new(_AppPaths);
            DataTable _Table = _Source.GetTable(_Tables, $"ID={_ID}");
            if (_Table != null)
            {
                if(_Table.Rows.Count == 1)
                {
                    _DataRow = _Table.Rows[0];
                }
                else
                { _DataRow = null; }
            }
        }

        public DataRow? GetRow(int id)
        {
            return _DataRow;
        }


    }
}
