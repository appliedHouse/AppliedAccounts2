using System.ComponentModel;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedDB
{
    public static class Functions
    {

        public static bool Seek(DataRow CurrentRow, string DBFile, int ID)
        {
            var SQLQuery = $"SELECT [ID] FROM {CurrentRow.Table.TableName} WHERE [ID]={ID}";
            DataTable _Table = DataSource.GetDataTable(DBFile, SQLQuery, CurrentRow.Table.TableName);
            if (_Table != null) { return false; }
            if (_Table?.Rows.Count == 0) { return false; }
            return true;
        }
        public static DataRow Seek(string _DBFile, Tables _Table, int ID)
        {
            string _Text = $"SELECT * FROM {_Table} WHERE ID={ID}";
            DataTable _DataTable = DataSource.GetDataTable(_DBFile, _Text, _Table.ToString());
            if (_DataTable.Rows.Count > 0)
            {
                return _DataTable.Rows[0];
            }

            return null;


        }
        public static DataRow RemoveNull(DataRow CurrentRow)
        {
            foreach (DataColumn Column in CurrentRow.Table.Columns)
            {
                if (CurrentRow[Column] == DBNull.Value)
                {
                    var _Type = CurrentRow.Table.Columns[Column.ColumnName]?.DataType;

                    if (_Type is not null)
                    {
                        if (_Type == typeof(string)) { CurrentRow[Column] = ""; }
                        if (_Type == typeof(int)) { CurrentRow[Column] = 0; }
                        if (_Type == typeof(long)) { CurrentRow[Column] = 0; }
                        if (_Type == typeof(short)) { CurrentRow[Column] = 0; }
                        if (_Type == typeof(decimal)) { CurrentRow[Column] = 0.00M; }
                        if (_Type == typeof(DateTime)) { CurrentRow[Column] = DateTime.Now; }
                    }
                }
            }
            return CurrentRow;
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public static int MaxID(string DBFile, Tables _Table)
        {
            var _DataTable = DataSource.GetDataTable(DBFile, _Table);
            var _MaxID = (int)_DataTable.Compute("MAX(ID)", "");
            _DataTable.Dispose();
            return _MaxID + 1; ;
        }
        public static string GetTitle(string DBFile, Tables _Table, int Id)
        {
            if (Id > 0)
            {
                var _DataList = DataSource.GetDataList(DBFile, _Table);
                var _Title = _DataList.FirstOrDefault(e => e.Keys.Contains(Id))?.First().Value;
                if (_Title != null) { return _Title; }
            }
            return "";
        }

        public static int Code2Int(string DBFile, Tables _Table, string _Code)
        {
            var SQLQuery = $"SELECT [ID] FROM [{_Table}] WHERE [Code]='{_Code}'";
            DataTable _DataTable = DataSource.GetDataTable(DBFile, SQLQuery, "Code");
            if (_DataTable.Rows.Count > 0)
            {
                return (int)_DataTable.Rows[0][0];
            }
            return 0;

        }

        public static decimal Code2Rate(string DBFile, int _ID)
        {
            var SQLQuery = $"SELECT [Rate] FROM [{Tables.Taxes}] WHERE [ID]={_ID}";
            DataTable _DataTable = DataSource.GetDataTable(DBFile, SQLQuery, "Tax");
            if (_DataTable.Rows.Count > 0)
            {
                return (decimal)_DataTable.Rows[0][0];
            }
            return 0.00M;

        }

        public static decimal GetTaxRate(string DBFile, int ID)
        {
            DataRow? _DataRow = Seek(DBFile, Tables.Taxes, ID);
            if (_DataRow != null)
            {
                return (decimal)_DataRow["Rate"];
            }
            return 0.00M;
        }
    }
}
