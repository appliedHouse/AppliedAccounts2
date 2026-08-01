namespace AppliedAccounts.Data.Mapping
{
    public static class MappingDate
    {

        public static string ToYMD(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToQuery(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static DateTime MinDate()
        {
            return new DateTime(2000, 1, 1);
        }

        public static DateTime MaxDate()
        {
            return new DateTime(2030, 12, 31);
        }


        public static string ToDisplay(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }


    }
}
