using AppliedDB;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Data
{
    public class NewVoucherNo
    {

        public static string GetPurchaseVouNo(string _DBFile)
        {
            var _Table = DataSource.GetDataTable(_DBFile, Tables.BillPayable);
            var _Date = DateTime.Now;
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            var _View = _Table.AsDataView();
            var _NewNum = $"PR{_Year}{_Month}";
            _View.RowFilter = $"[Vou_No] like '{_NewNum}%'";

            if (_View.Count == 0) { return $"{_NewNum}-0001"; }
            else
            {
                string MaxVouNo = (string)_Table.Compute("MAX(Vou_No)", _View.RowFilter);
                string _Vou_No1 = MaxVouNo.Substring(0, 6);
                string _Vou_No2 = MaxVouNo.Substring(7, 4);
                int _Number = Conversion.ToInteger(_Vou_No2) + 1;
                string _MaxNum = Conversion.ToInteger(_Number).ToString("0000");
                return $"{_Vou_No1}-{_MaxNum}";
            }
        }

        public static string GetSaleVouNo(string _DBFile)
        {
            var _Table = DataSource.GetDataTable(_DBFile, Tables.BillReceivable);
            var _Date = DateTime.Now;
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            var _View = _Table.AsDataView();
            var _NewNum = $"SL{_Year}{_Month}";
            _View.RowFilter = $"[Vou_No] like '{_NewNum}%'";

            if (_View.Count == 0) { return $"{_NewNum}-0001"; }
            else
            {
                string MaxVouNo = (string)_Table.Compute("MAX(Vou_No)", _View.RowFilter);
                string _Vou_No1 = MaxVouNo.Substring(0, 6);
                string _Vou_No2 = MaxVouNo.Substring(7, 4);
                int _Number = Conversion.ToInteger(_Vou_No2) + 1;
                string _MaxNum = Conversion.ToInteger(_Number).ToString("0000");
                return $"{_Vou_No1}-{_MaxNum}";
            }

        }
    }


}
