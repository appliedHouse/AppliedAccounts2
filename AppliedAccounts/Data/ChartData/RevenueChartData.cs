namespace AppliedAccounts.Data.ChartData
{
    public class RevenueChartData
    {

        public static object Revenue = new
        {
            type = "bar",
            data = new
            {
                labels = new[] { "January", "February", "March", "April" },
                datasets = new[]
            {
                new
                {
                    label = "Sales",
                    data = new[] { 10, 20, 30, 40 },
                    backgroundColor = new[] { "rgba(75, 192, 192, 0.2)" },
                    borderColor = new[] { "rgba(75, 192, 192, 1)" },
                    borderWidth = 1
                }
            }
            },
            options = new
            {
                scales = new
                {
                    y = new { beginAtZero = true }
                }
            }
        };
    }
}
