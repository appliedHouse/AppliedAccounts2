using System.Data;
using System.Security.Cryptography;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Data
{
    public class AppRegistry : IAppRegistry
    {

        //public static readonly string DateYMD = "yyyy-MM-dd";
        //public static readonly string FormatCurrency1 = "#,##0.00";
        //public static readonly string FormatCurrency2 = "#,##0";
        //public static readonly string FormatDate = "dd-MMM-yyyy";
        //public static readonly string FormatDateY2 = "dd-MMM-yy";
        //public static readonly string FormatDateM2 = "dd-MM-yy";
        public static readonly DateTime MinDate = new(2020, 01, 01);
        public static readonly DateTime MinVouDate = new(2024, 06, 01);
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
                ReturnValue = keytype switch
                {
                    KeyType.Number => VW_Registry[0]["nValue"],
                    KeyType.Currency => VW_Registry[0]["mValue"],
                    KeyType.Boolean => VW_Registry[0]["bValue"],
                    KeyType.Date => VW_Registry[0]["dValue"],
                    KeyType.Text => VW_Registry[0]["cValue"],
                    _ => string.Empty
                };
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
                return (DateTime)VW_Registry[0]["dValue"];
            }
            else
            {
                return DateTime.Now;
            }
        }
        public static int GetNumber(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return 0; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat("Code='", Key, "'");
            if (VW_Registry.Count == 1)
            {
                return (int)VW_Registry[0]["nValue"];
            }
            else
            {
                return 0;
            }
        }
        public static decimal GetCurrency(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return 0.00M; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat("Code='", Key, "'");
            if (VW_Registry.Count == 1)
            {
                return (decimal)VW_Registry[0]["mValue"];
            }
            else
            {
                return 0.00M;
            }
        }
        public static string GetText(string DataFile, string Key)
        {
            if (DataFile == null || DataFile == string.Empty) { return string.Empty; }
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = $"Code='{Key}'";
            if (VW_Registry.Count == 1)
            {
                var value = VW_Registry[0]["cValue"];
                if (value == DBNull.Value) { return ""; }
                return (string)value;
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
                var value = VW_Registry[0]["bValue"];
                if (value == DBNull.Value) { return false; }
                return (bool)value;
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
            DataView VW_Registry = GetRegistryView(DataFile);
            VW_Registry.RowFilter = string.Concat($"Code='{Key}'");
            if (VW_Registry.Count == 1)
            {
                Dates[0] = (DateTime)VW_Registry[0]["From"];
                Dates[1] = (DateTime)VW_Registry[0]["To"];
            }
            return Dates;
        }
        public static bool SetKey(string DataFile, string _Key, object KeyValue, KeyType _KeyType)
        {
            if (DataFile == null || DataFile == string.Empty) { return false; ; }
            return SetKey(DataFile, _Key, KeyValue, _KeyType, "");
        }
        public static bool SetKey(string DataFile, string Key, object KeyValue, KeyType keytype, string _Title)
        {

            if (DataFile == null || DataFile == string.Empty) { return false; }
            DataTable TB_Registry = GetRegistryTable(DataFile);
            DataRow CurrentRow;
            string SQLAction;

            TB_Registry.DefaultView.RowFilter = string.Concat("Code='", Key, "'");
            if (TB_Registry.DefaultView.Count == 1)
            {
                SQLAction = "Update";
                CurrentRow = TB_Registry.DefaultView[0].Row;
            }
            else
            {
                SQLAction = "Insert";
                CurrentRow = TB_Registry.NewRow();
                CurrentRow["ID"] = 0;
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
           

            if (SQLAction == "Insert") { var cmd = AppliedDB.Commands.Insert(CurrentRow, DataFile); cmd.Connection.Open(); cmd.ExecuteNonQuery(); cmd.Connection.Close(); return true; }
            if (SQLAction == "Update") { var cmd = AppliedDB.Commands.UpDate(CurrentRow, DataFile); cmd.Connection.Open();  cmd.ExecuteNonQuery(); cmd.Connection.Close(); return true; }
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
        private static DataTable GetRegistryTable(string DataFile)
        {
            return AppliedDB.DataSource.GetDataTable(DataFile, Tables.Registry);
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

    public enum KeyType
    {
        Number,
        Currency,
        Date,
        Boolean,
        Text,
        UserName,
        From,
        To,
        FromTo,
    }

}
