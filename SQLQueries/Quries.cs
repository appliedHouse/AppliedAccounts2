using System.Text;

namespace SQLQueries
{
    public static class Quries
    {
        #region Receipt
        public static string Receipt(int _ReceiptID)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT [R].*,");
            _Text.AppendLine("[C].[Title] AS [PayerTitle],");
            _Text.AppendLine("[E].[Title] AS [EmployeeTitle], ");
            _Text.AppendLine("[A].[Title] AS [AccountTitle], ");
            _Text.AppendLine("[P].[Title] As [ProjectTitle]");
            _Text.AppendLine("FROM [Receipts] [R]");
            _Text.AppendLine("LEFT JOIN [COA]       [A] ON [A].[ID] = [R].[COA]");
            _Text.AppendLine("LEFT JOIN [Project]   [P] ON [P].[ID] = [R].[Project]");
            _Text.AppendLine("LEFT JOIN [Employees] [E] ON [E].[ID] = [R].[Employee]");
            _Text.AppendLine("LEFT JOIN [Customers] [C] ON [C].[ID] = [R].[Payer]");
            _Text.AppendLine($"WHERE [R].[ID] = {_ReceiptID}");
            return _Text.ToString();
        }

        public static string ReceiptList(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT ");
            _Text.AppendLine("[R].*,");
            _Text.AppendLine("[A].[Title] AS[TitleAccount],");
            _Text.AppendLine("[C].[Title] AS[TitlePayer]");
            _Text.AppendLine("FROM[Receipt][R]");
            _Text.AppendLine("LEFT JOIN[COA] [A] ON[A].[ID] = [R].[COA]");
            _Text.AppendLine("LEFT JOIN[Customers] [C] ON[C].[ID] = [R].[Payer]");
            _Text.AppendLine("");
            if (!string.IsNullOrEmpty(_Filter))
            {
                _Text.AppendLine("WHERE " + _Filter);
            }
            return _Text.ToString();
        }

        public static string Receipts()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM [Receipts]");
            return _Text.ToString();
        }

        #endregion
    }
}
