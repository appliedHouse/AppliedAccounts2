
namespace AppliedDB
{
    public static class VoucherTypeClass
    {
        public enum VoucherType
        {
            JV,
            Cash,
            Bank,
            Payable,
            Receivable,
            Sale,
            Purchase,
            Production,
            Payroll
        }

        public enum VoucherStatus
        {
            New,
            Submitted,
            Posted,
            Deleted,
            Updated,
        }
    }
}
