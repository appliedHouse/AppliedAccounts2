using System.Data;
using Format = AppliedGlobals.AppValues.Format;
using Tables = AppliedDB.Enums.Tables;
using KeyType = AppliedGlobals.AppErums.KeyTypes;

namespace AppliedAccounts.Data
{
    public class AppRegistry : IAppRegistry
    {
        public static readonly DateTime MinDate = new(2020, 01, 01);
        public static readonly DateTime MinVouDate = new(2020, 06, 01);
        public static readonly DateTime MaxVouDate = new(2030, 01, 01);

        public static string GetFormatCurrency(string DataFile)
        {
            if (string.IsNullOrEmpty(DataFile)) { return string.Empty; }
            return GetText(DataFile, "FMTCurrency");
        }
        public static string GetFormatDate(string DataFile)
        {
            if (string.IsNullOrEmpty(DataFile)) { return string.Empty; }
            return GetText(DataFile, "FMTDate");
        }
        public static string Currency(string DataFile, object Amount)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            var _Format = GetText(DataFile, "FMTCurrency");
            var _Sign = GetCurrencySign(DataFile);
            var _Amount = ((decimal)Amount).ToString(_Format);
            return string.Concat(_Amount, " ", _Sign);
        }
        public static string Amount(string DataFile, object Amount)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            var _Format = GetText(DataFile, "FMTCurrency");
            return ((decimal)Amount).ToString(_Format);

        }
        public static string Date(string DataFile, DateTime Date)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            var _Format = GetText(DataFile, "FMTDate");
            return Date.ToString(_Format);
        }
        public static string YMD(DateTime Date)
        {
            return Date.ToString(Format.YMD);
        }
        public static string GetCurrencySign(string DataFile)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            var sign = GetText(DataFile, "CurrencySign");
            if (sign.Length > 0) { return sign; }
            return "Rs.";
        }
        public static object GetKey(string DataFile, string Key, KeyType keytype)
        {
            if (DataFile == null || DataFile == string.Empty) { return null; }
            object ReturnValue;
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat("Code='", Key, "'");
            if (VW_Registry.Count == 1)
            {
                var row = VW_Registry[0];
                ReturnValue = keytype switch
                {
                    KeyType.Number => row["nValue"],
                    KeyType.Currency => row["mValue"],
                    KeyType.Boolean => row["bValue"],
                    KeyType.Date => row["dValue"],
                    KeyType.Text => row["cValue"],
                    KeyType.From => row["From"],
                    KeyType.To => row["To"],
                    _ => string.Empty
                };
                if (ReturnValue == DBNull.Value)
                {
                    ReturnValue = keytype switch
                    {
                        KeyType.Number => 0,
                        KeyType.Currency => 0.00M,
                        KeyType.Boolean => false,
                        KeyType.Date => DateTime.Now,
                        KeyType.Text => string.Empty,
                        KeyType.From => MinDate,
                        KeyType.To => DateTime.Now,
                        _ => string.Empty
                    };
                }
            }
            else
            {
                ReturnValue = keytype switch
                {
                    KeyType.Number => 0,
                    KeyType.Currency => 0.00,
                    KeyType.Boolean => false,
                    KeyType.Date => DateTime.Now,
                    KeyType.Text => string.Empty,
                    _ => string.Empty
                };
            }
            return ReturnValue;
        }
        public static DateTime GetDate(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return DateTime.Now; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat("Code='", Key, "'");
            if (VW_Registry.Count == 1)
            {
                var val = VW_Registry[0].Row.Field<DateTime?>("dValue");
                return val ?? DateTime.Now;
            }
            return DateTime.Now;
        }
        public static int GetNumber(string DataFile, string Key)
        {
            try
            {
                if (DataFile == null || DataFile == string.Empty) { return 0; }
                var obj = GetKey(DataFile, Key, KeyType.Number);
                return obj == null ? 0 : Convert.ToInt32(obj);
            }
            catch (Exception)
            {

                return 0;
            }

            
        }
        public static decimal GetCurrency(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return 0.00M; }
            var obj = GetKey(DataFile, Key, KeyType.Currency);
            return obj == null ? 0.00M : Convert.ToDecimal(obj);
        }
        public static string GetText(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = $"Code='{Key}'";
            if (VW_Registry.Count == 1)
            {
                var value = VW_Registry[0].Row.Field<string>("cValue");
                return value ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
        public static bool GetBool(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return false; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat("Code='", Key, "'");
            if (VW_Registry.Count == 1)
            {
                var value = VW_Registry[0].Row.Field<bool?>("bValue");
                return value ?? false;
            }
            else
            {
                return false;
            }
        }
        public static DateTime[] GetDates(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return new DateTime[2]; }
            DateTime[] Dates = new DateTime[2];
            Dates[0] = GetFrom(DataFile, Key);
            Dates[1] = GetTo(DataFile, Key);
            return Dates;
        }

        public static DateTime GetFrom(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return MinDate; }

            DataView VW_Registry = GetRegistryView(DataFile, Key);
            if (VW_Registry.Count == 1)
            {
                var val = VW_Registry[0].Row.Field<DateTime?>("From");
                return val ?? MinDate;
            }
            return MinDate;
        }

        public static DateTime GetTo(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return MinDate; }

            DataView VW_Registry = GetRegistryView(DataFile, Key);
            if (VW_Registry.Count == 1)
            {
                var val = VW_Registry[0].Row.Field<DateTime?>("To");
                return val ?? MinDate;
            }
            return MinDate;
        }

        public static bool SetKey(string DataFile, string _Key, object KeyValue, KeyType _KeyType)
        {
            if (DataFile == null || DataFile == string.Empty) { return false; ; }
            return SetKey(DataFile, _Key, KeyValue, _KeyType, "");
        }
        public static bool SetKey(string DataFile, string Key, object KeyValue, KeyType keytype, string _Title)
        {

            if (DataFile == null || DataFile == string.Empty) { return false; }
            DataTable TB_Registry = GetRegistryTable(DataFile,Key);
            DataRow CurrentRow;
            string SQLAction;

            TB_Registry.DefaultView.RowFilter = string.Concat("Code='", Key, "'");
            if (TB_Registry.DefaultView.Count > 0)
            {
                SQLAction = "Update";
                CurrentRow = TB_Registry.DefaultView[0].Row;
                CurrentRow.AcceptChanges();
            }
            else
                {
                    SQLAction = "Insert";
                    CurrentRow = TB_Registry.NewRow();
                    CurrentRow["ID"] = DBNull.Value;
                }
            CurrentRow["Code"] = Key;
            CurrentRow["Title"] = _Title;
            CurrentRow["UserName"] = DataFile;
            switch (keytype)
            {
                case KeyType.Number:
                    CurrentRow["nValue"] = KeyValue;
                    break;
                case KeyType.Currency:
                    CurrentRow["mValue"] = KeyValue;
                    break;
                case KeyType.Date:
                    CurrentRow["dValue"] = KeyValue;
                    break;
                case KeyType.Boolean:
                    CurrentRow["bValue"] = KeyValue;
                    break;
                case KeyType.Text:
                    CurrentRow["cValue"] = KeyValue;
                    break;
                case KeyType.From:
                    CurrentRow["From"] = KeyValue;
                    break;
                case KeyType.To:
                    CurrentRow["To"] = KeyValue;
                    break;
                default:
                    break;
            }

            if (SQLAction == "Insert") { var cmd = AppliedDB.CommandConstructor.Commands.Insert(CurrentRow, DataFile); cmd?.Connection.Open(); cmd?.ExecuteNonQuery(); cmd?.Connection.Close(); return true; }
            if (SQLAction == "Update") { var cmd = AppliedDB.CommandConstructor.Commands.UpDate(CurrentRow, DataFile); cmd?.Connection.Open(); cmd?.ExecuteNonQuery(); cmd?.Connection.Close(); return true; }
            return false;
        }
        public static int ExpDays(string DataFile)
        {
            if (DataFile == null || DataFile == string.Empty) { return 0; }
            int Days = (int)GetKey(DataFile, "StockExpiry", KeyType.Number);
            return Days;   // One Year of Expiry Date
        }
        public static string GetReportFooter(string DataFile)
        {
            var _Footer = GetText(DataFile, "ReportFooter");
            if (_Footer != null && _Footer.Length == 0)
            {
                return "Power by Applied Software House";
            }
            return _Footer;

        }
        private static DataView GetRegistryView(string DataFile)
        {
            return AppliedDB.DataSource.GetDataTable(DataFile, Tables.Registry).AsDataView();
        }
        private static DataView GetRegistryView(string DataFile, string Key)
        {
            return AppliedDB.DataSource.GetDataTable(DataFile, Tables.Registry, Key).AsDataView();
        }
        private static DataTable GetRegistryTable(string DataFile)
        {
            return AppliedDB.DataSource.GetDataTable(DataFile, Tables.Registry);
        }
        private static DataTable GetRegistryTable(string DataFile, string Key)
        {
            return AppliedDB.DataSource.GetDataTable(DataFile, Tables.Registry, Key);
        }
    }

    public interface IAppRegistry
    {
        public static readonly string DateYMD = string.Empty;
        public static readonly string FormatCurrency = string.Empty;
        public static readonly string FormatCurrency1 = string.Empty;
        public static readonly string FormatCurrency2 = string.Empty;
        public static readonly string FormatDate = string.Empty;
        public static readonly string FormatDateY2 = string.Empty;
        public static readonly string FormatDateM2 = string.Empty;
    }

    //public enum KeyType
    //{
    //    Number,
    //    Currency,
    //    Date,
    //    Boolean,
    //    Text,
    //    UserName,
    //    From,
    //    To,
    //    FromTo
    //}

}
