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
            if (_Table.Rows.Count == 0) { return false; }
            return true;
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
    }
}
