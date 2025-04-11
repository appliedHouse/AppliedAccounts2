namespace AppliedAccounts.Data
{
    public class Conversion
    {
        public static string Row2Money(object _Value, string _Format)
        {
            return ToDecimal(_Value).ToString(_Format);
        }

        public static string Row2Date(object _Value)
        {
            string _Result = string.Empty;
            if (_Value == null) { return _Result; }

            try
            {
                var _Date = (DateTime)_Value;
                _Result = _Date.ToString(Format.DDMMYY);
            }
            catch (Exception)
            {
                _Result = "No Date";
            }
            return _Result;
        }

        public static int ToInteger(object _Value)
        {
            _Value ??= "";
            try
            {
                var type = _Value.GetType();

                if (type == typeof(string)) { return int.Parse((string)_Value); }
                if (type == typeof(decimal)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(long)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(float)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(double)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(int)) { return (int)_Value; }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal ToDecimal(object _Value)
        {
            try
            {
                var type = _Value.GetType();

                if (type == typeof(string)) { return decimal.Parse((string)_Value); }
                if (type == typeof(int)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(long)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(short)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(float)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(double)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(decimal)) { return (decimal)_Value; }
                return 0.00M;
            }
            catch (Exception)
            {
                return 0.00M;
            }
        }

        public static string ToAmount(object _Value, string Format)
        {
            _Value ??= string.Empty;
            var _Type = _Value.GetType();

            if (_Type == typeof(string)) { return ((string)_Value).ToString(); }
            if (_Type == typeof(decimal)) { return ((decimal)_Value).ToString(Format); }
            if (_Type == typeof(float)) { return ((float)_Value).ToString(Format); }
            if (_Type == typeof(double)) { return ((double)_Value).ToString(Format); }
            if (_Type == typeof(int)) { return ((int)_Value).ToString(Format); }
            if (_Type == typeof(short)) { return ((short)_Value).ToString(Format); }
            if (_Type == typeof(long)) { return ((long)_Value).ToString(Format); }

            return string.Empty;
        }

        public static DateTime ToDateTime(object _Value)
        {
            try
            {
                if (DateTime.TryParse(_Value.ToString(), out DateTime _DateTime))
                {
                    return _DateTime;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
