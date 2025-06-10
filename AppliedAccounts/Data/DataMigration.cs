using AppliedAccounts.Services;
using AppliedDB;
using System.Data;

namespace AppliedAccounts.Data
{
    public class DataMigration
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource Source { get; set; }
        public DataTable CashBook { get; set; }

        public DataMigration(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
        }

        public void Cash2Book()
        {

            CashBook = Source.GetTable(Enums.Tables.CashBook);
            var _Book1 = Source.GetTable(Enums.Tables.Book);
            var _Book2 = Source.GetTable(Enums.Tables.Book2);
            var _Num = 0;

            if (CashBook.Rows.Count > 0)
            {
                foreach (DataRow _Row in CashBook.Rows)
                {
                    var IsExist = _Book1.AsEnumerable().Any(r => r.Field<string>("Vou_No") == _Row.Field<string>("Vou_No"));

                    if (!IsExist)
                    {
                        _Num++; if(_Num > 10000) { break; }

                        var _NewRow1 = _Book1.NewRow();
                        var _NewRow2 = _Book2.NewRow();

                        _NewRow1["ID"] = 0;
                        _NewRow1["BookID"] = _Row["BookID"];
                        _NewRow1["Vou_No"] = _Row["Vou_No"];
                        _NewRow1["Vou_Date"] = _Row["Vou_Date"];
                        _NewRow1["Amount"] = ((decimal)_Row["DR"] - (decimal)_Row["CR"]);
                        _NewRow1["Ref_No"] = _Row["Ref_No"];
                        _NewRow1["SheetNo"] = _Row["Sheet_No"];
                        _NewRow1["Remarks"] = _Row["Description"];
                        _NewRow1["Status"] = _Row["Status"];

                        _NewRow2["ID"] = 0;
                        _NewRow2["TranId"] = _Row["ID"];
                        _NewRow2["Sr_No"] = 1;
                        _NewRow2["COA"] = _Row["COA"];
                        _NewRow2["Company"] = _Row["Customer"];
                        _NewRow2["Employee"] = _Row["Employee"];
                        _NewRow2["Project"] = _Row["Project"];
                        _NewRow2["DR"] = _Row["DR"];
                        _NewRow2["CR"] = _Row["CR"];
                        _NewRow2["Description"] = _Row["Description"];
                        _NewRow2["Comments"] = _Row["Comments"];

                        Source.Save(_NewRow1);

                        _NewRow2["TranId"] = Source.MyCommands.PrimaryKeyID;
                        Source.Save(_NewRow2);
                    }
                }
            }
        }
    }
}
