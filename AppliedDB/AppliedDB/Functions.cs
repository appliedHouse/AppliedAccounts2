using System.ComponentModel;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedDB
{
    public static class Functions
    {

        public static bool Seek(DataRow CurrentRow, string DBFile, long ID)
        {
            var SQLQuery = $"SELECT [ID] FROM {CurrentRow.Table.TableName} WHERE [ID]={ID}";
            DataTable _Table = DataSource.GetDataTable(DBFile, SQLQuery, CurrentRow.Table.TableName);
            if (_Table != null) { return false; }
            if (_Table?.Rows.Count == 0) { return false; }
            return true;
        }
        public static DataRow Seek(string _DBFile, Tables _Table, long ID)
        {
            string _Text = $"SELECT * FROM {_Table} WHERE ID={ID}";
            DataTable _DataTable = DataSource.GetDataTable(_DBFile, _Text, _Table.ToString());
            if (_DataTable.Rows.Count > 0)
            {
                return _DataTable.Rows[0];
            }

            return null!;


        }
        //public static DataRow RemoveNull(DataRow CurrentRow)
        //{
        //    if (CurrentRow != null)
        //    {
        //        foreach (DataColumn Column in CurrentRow.Table.Columns)
        //        {
        //            if (CurrentRow[Column] == DBNull.Value)
        //            {
        //                var _Type = CurrentRow.Table.Columns[Column.ColumnName]?.DataType;

        //                if (_Type is not null)
        //                {
        //                    if (_Type == typeof(string)) { CurrentRow[Column] = ""; }
        //                    if (_Type == typeof(int)) { CurrentRow[Column] = 0; }
        //                    if (_Type == typeof(long)) { CurrentRow[Column] = 0; }
        //                    if (_Type == typeof(short)) { CurrentRow[Column] = 0; }
        //                    if (_Type == typeof(decimal)) { CurrentRow[Column] = 0.00M; }
        //                    if (_Type == typeof(DateTime)) { CurrentRow[Column] = DateTime.Now; }
        //                }
        //            }
        //        }
        //        return CurrentRow;
        //    }
        //    return null!;
        //}
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

        public static string GetTitle(string DBFile, Tables _Table, long Id)
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

        public static long Code2long(string DBFile, Tables _Table, string _Code)
        {
            var SQLQuery = $"SELECT [ID] FROM [{_Table}] WHERE [Code]='{_Code}'";
            DataTable _DataTable = DataSource.GetDataTable(DBFile, SQLQuery, "Code");
            if (_DataTable.Rows.Count > 0)
            {
                return (long)_DataTable.Rows[0][0];
            }
            return 0;
        }


        public static decimal Code2Rate(string DBFile, long _ID)
        {
            var SQLQuery = $"SELECT [Rate] FROM [{Tables.Taxes}] WHERE [ID]={_ID}";
            DataTable _DataTable = DataSource.GetDataTable(DBFile, SQLQuery, "Tax");
            if (_DataTable.Rows.Count > 0)
            {
                return (decimal)_DataTable.Rows[0][0];
            }
            return 0.00M;

        }

        public static decimal GetTaxRate(string DBFile, long ID)
        {
            DataRow? _DataRow = Seek(DBFile, Tables.Taxes, ID);
            if (_DataRow != null)
            {
                return (decimal)_DataRow["Rate"];
            }
            return 0.00M;
        }

        public static string GetDateFilter(DateTime[] Dates)
        {
            return $" Date(Vou_Date)>='{Dates[0]:yyyy-MM-dd}' AND Date(Vou_Date)<='{Dates[1]:yyyy-MM-dd}'";
        }

        public static DataRow RemoveDBNull(this DataRow row)
        {
            try
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (row[column] != DBNull.Value)
                        continue;

                    Type type = Nullable.GetUnderlyingType(column.DataType) ?? column.DataType;

                    if (type == typeof(string))
                        row[column] = string.Empty;
                    else if (type == typeof(bool))
                        row[column] = false;
                    else if (type == typeof(byte))
                        row[column] = (byte)0;
                    else if (type == typeof(sbyte))
                        row[column] = (sbyte)0;
                    else if (type == typeof(short))
                        row[column] = (short)0;
                    else if (type == typeof(ushort))
                        row[column] = (ushort)0;
                    else if (type == typeof(int))
                        row[column] = 0;
                    else if (type == typeof(uint))
                        row[column] = 0U;
                    else if (type == typeof(long))
                        row[column] = 0L;
                    else if (type == typeof(ulong))
                        row[column] = 0UL;
                    else if (type == typeof(float))
                        row[column] = 0f;
                    else if (type == typeof(double))
                        row[column] = 0d;
                    else if (type == typeof(decimal))
                        row[column] = 0m;
                    else if (type == typeof(char))
                        row[column] = '\0';
                    else if (type == typeof(DateTime))
                        row[column] = DateTime.MinValue;
                    else if (type == typeof(DateTimeOffset))
                        row[column] = DateTimeOffset.MinValue;
                    else if (type == typeof(TimeSpan))
                        row[column] = TimeSpan.Zero;
                    else if (type == typeof(Guid))
                        row[column] = Guid.Empty;
                    else if (type == typeof(byte[]))
                        row[column] = Array.Empty<byte>();
                    else if (type.IsEnum)
                        row[column] = Enum.GetValues(type).GetValue(0)!;
                    else
                        row[column] = Activator.CreateInstance(type) ?? DBNull.Value;
                }

                return row;
            }
            catch
            {
                return row;
            }
        }

    }
}

