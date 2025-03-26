using System.Text;

namespace SQLQueries
{
    public static class Quries
    {
        #region Receipt
        public static string Receipt()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM [Receipt]");
            return _Text.ToString();
        }

        public static string ReceiptList()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM [Receipt]");
            return _Text.ToString();
        }

        #endregion
    }
}
