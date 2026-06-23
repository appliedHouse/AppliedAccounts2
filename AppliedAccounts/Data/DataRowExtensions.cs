using System;
using System.Data;
using System.Globalization;

namespace AppliedAccounts.Data
{
    public static class DataRowExtensions
    {
        private static bool HasColumn(DataRow row, string column)
            => row?.Table?.Columns.Contains(column) == true;

        private static object? GetValue(DataRow row, string column)
        {
            if (!HasColumn(row, column)) return null;
            var value = row[column];
            return value == DBNull.Value ? null : value;
        }

        // ===================== LONG =====================

        public static long GetInt64(this DataRow row, string column, long defaultValue = 0)
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return long.TryParse(value.ToString(), out var result) ? result : defaultValue;
            }
        }

        public static long? GetInt64Nullable(this DataRow row, string column)
        {
            var value = GetValue(row, column);
            if (value == null) return null;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return long.TryParse(value.ToString(), out var result) ? result : null;
            }
        }

        // ===================== INT =====================

        public static int GetInt32(this DataRow row, string column, int defaultValue = 0)
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return int.TryParse(value.ToString(), out var result) ? result : defaultValue;
            }
        }

        public static int? GetInt32Nullable(this DataRow row, string column)
        {
            var value = GetValue(row, column);
            if (value == null) return null;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return int.TryParse(value.ToString(), out var result) ? result : null;
            }
        }

        // ===================== DECIMAL =====================

        public static decimal GetDecimal(this DataRow row, string column, decimal defaultValue = 0.00M)
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                    ? result
                    : defaultValue;
            }
        }

        public static decimal? GetDecimalNullable(this DataRow row, string column)
        {
            var value = GetValue(row, column);
            if (value == null) return null;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                    ? result
                    : null;
            }
        }

        // ===================== DATETIME =====================

        public static DateTime GetDate(this DataRow row, string column, DateTime? defaultValue = null)
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue ?? DateTime.MinValue;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.TryParse(value.ToString(), out var result)
                    ? result
                    : defaultValue ?? DateTime.MinValue;
            }
        }

        public static DateTime? GetDateNullable(this DataRow row, string column)
        {
            var value = GetValue(row, column);
            if (value == null) return null;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.TryParse(value.ToString(), out var result)
                    ? result
                    : null;
            }
        }

        // ===================== STRING =====================

        public static string GetString(this DataRow row, string column, string defaultValue = "")
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue;

            return value.ToString()?.Trim() ?? defaultValue;
        }

        // ===================== BOOL =====================

        public static bool GetBool(this DataRow row, string column, bool defaultValue = false)
        {
            var value = GetValue(row, column);
            if (value == null) return defaultValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                var str = value.ToString()?.ToLower();
                return str == "1" || str == "true" || str == "yes";
            }
        }

        public static bool? GetBoolNullable(this DataRow row, string column)
        {
            var value = GetValue(row, column);
            if (value == null) return null;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                var str = value.ToString()?.ToLower();
                if (str == "1" || str == "true" || str == "yes") return true;
                if (str == "0" || str == "false" || str == "no") return false;
                return null;
            }
        }
    }
}