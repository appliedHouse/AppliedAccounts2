using AppliedDB;
using AppMessages;
using System.Data;
using System.Globalization;
using Format = AppliedGlobals.AppValues.Format;

namespace AppliedAccounts.Data
{
    public static class AppFunctions
    {
        public static string GetRootPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
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

        public static string QueryDate (this DateTime _Date)
        {
            return _Date.ToString("yyyy-MM-dd");
        }

        public static string DisplayDate(this DateTime _Date)
        {
            return _Date.ToString("dd-MMM-yyyy");
        }

        public static string Display (this decimal _decimal)
        {
            // Show Display decimal as 123,456,789.99
            return _decimal.ToString("N2", CultureInfo.CurrentCulture);
        }

        public static string DR_CR(this decimal _decimal)
        {
            // Show Display decimal as 123,456,789.99 DR or 123,456,789.99 CR
            string formattedNumber = Math.Abs(_decimal).ToString("N2", CultureInfo.CurrentCulture);
            string suffix = _decimal < 0 ? "CR" : "DR";
            return $"{formattedNumber} {suffix}";
        }
    }


}
