using AppliedDB;
using System.Data;

namespace VoucherPosting
{
   

    public class LedgerModel 
    {
        public DataSource Source { get; set; }
        public DataTable LedgerTable { get; set; }
        public int Count => LedgerTable.Rows.Count;
        public string Vou_No { get; set; }
        public long RecID { get; set; } = 0;
       

        public LedgerModel(DataSource _Source, string _VouNo)
        {
            Source = _Source;
            Vou_No = _VouNo;
            GetData();
        }

        public void GetData()
        {
            if(Source == null) { return; }
            if(string.IsNullOrEmpty(Vou_No)) { return; }
            LedgerTable = Source.GetTable(Enums.Tables.Ledger, $"Vou_No={Vou_No}");

            if(LedgerTable.Rows.Count > 0)
            {
                RecID = (long)LedgerTable.Rows[0]["Vou_ID"];
            }
            else
            {
                LedgerTable = Source.CloneTable(Enums.Tables.Ledger);
            }
        }

        public long GetVouID()
        {
            if(LedgerTable==null) { return 0; }
            else
            {
                if(LedgerTable.Rows.Count > 0)
                {
                    return LedgerTable.Rows[0].Field<long>("ID");
                }
            }
            return 0;

        }

        public long MaxID()
        {
            return Source.GetMaxID(Enums.Tables.Ledger);
        }
    }
}
