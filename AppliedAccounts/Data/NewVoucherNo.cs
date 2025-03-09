using AppliedDB;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Data
{
    public class NewVoucherNo
    {
        public static string GetNewVoucherNo(string _DBFile, Tables _Table, string _Prefix)
        {
            return GetNewVoucherNo(_DBFile, _Table, _Prefix, DateTime.Now);
        }

        public static string GetNewVoucherNo(string _DBFile, Tables _Table, string _Prefix, DateTime _VouDate)
        {
            if (string.IsNullOrEmpty(_Prefix)) { return string.Empty; }
            if (string.IsNullOrEmpty(_DBFile)) { return string.Empty; }


            var _DataTable = DataSource.GetDataTable(_DBFile, _Table.ToString());
            var _Date = _VouDate;
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            var _View = _DataTable.AsDataView();
            var _NewNum = $"{_Prefix}{_Year}{_Month}";
            _View.RowFilter = $"[Vou_No] like '{_NewNum}%'";

            if (_View.Count == 0) { return $"{_NewNum}-0001"; }
            else
            {
                string MaxVouNo = (string)_DataTable.Compute("MAX(Vou_No)", _View.RowFilter);
                string _Vou_No1 = MaxVouNo.Substring(0, 6);
                string _Vou_No2 = MaxVouNo.Substring(7, 4);
                int _Number = Conversion.ToInteger(_Vou_No2) + 1;
                string _MaxNum = Conversion.ToInteger(_Number).ToString("0000");
                return $"{_Vou_No1}-{_MaxNum}";
            }
        }
        
        public static string GetPurchaseVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillPayable, "AP", _VouDate);
        }

        public static string GetSaleInvoiceVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillReceivable, "AR", _VouDate);
        }

        public static string GetCashVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillReceivable, "CV", _VouDate);
        }

        public static string GetBankVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillReceivable, "BV", _VouDate);
        }

        public static string GetReceiptVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillReceivable, "RV", _VouDate);
        }

        public static string GetJournalVoucher(string _DBFile, DateTime _VouDate)
        {
            return GetNewVoucherNo(_DBFile, Tables.BillReceivable, "JV", _VouDate);
        }

    }


}
