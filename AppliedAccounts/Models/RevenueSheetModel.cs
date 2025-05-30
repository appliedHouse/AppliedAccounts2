using AppliedAccounts.Data;
using AppliedDB;
using System.Data;
using SQLType = AppliedDB.Enums.SQLType;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class RevenueSheetModel
    {

        #region Variables
        public int ID { get; set; }
        public string SheetNo { get; set; } = string.Empty;
        public DateTime SheetDate { get; set; }
        public int Company { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        public int StockID { get; set; }
        public decimal Qty { get; set; } = 0;
        public decimal Rate { get; set; } = 0;
        public decimal TaxID { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;


        public virtual decimal Amount => Qty * Rate;
        public virtual decimal TaxRate { get; set; } = 0;
        public virtual decimal TaxAmount => Amount * TaxRate;
        public virtual decimal NetAmount => Amount + TaxAmount;


        public string DataFile { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DataTable TB_RevSheet { get; set; } = new();
        public DataRow CurrentRow { get; set; }


        public bool FoundRow { get; set; } = false;
        public string MyMessage { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public RevenueSheetModel()
        {
            CurrentRow = GetDataRow(0);
        }
        #endregion

        #region Get Row
        private DataRow GetDataRow(int _Index)
        {
            FoundRow = false;
            DataRow _CurrentRow;
            if (TB_RevSheet != null)
            {

                if (TB_RevSheet.Rows.Count == 1)
                {
                    FoundRow = true;
                    _CurrentRow = TB_RevSheet.Rows[_Index];
                }
                else
                {
                    _CurrentRow = TB_RevSheet.NewRow();
                }
            }
            else
            {
                TB_RevSheet = new();
                _CurrentRow = TB_RevSheet.NewRow();
            }
            return _CurrentRow;
        }
        #endregion

        #region Variable -> Row
        public void GetCurrentRow()
        {
            if (CurrentRow != null)
            {
                CurrentRow["ID"] = ID;
                CurrentRow["SheetNo"] = SheetNo;
                CurrentRow["SheetDate"] = SheetDate;
                CurrentRow["Company"] = Company;
                CurrentRow["Project"] = Project;
                CurrentRow["Employee"] = Employee;
                CurrentRow["StockID"] = StockID;
                CurrentRow["TaxID"] = TaxID;
                CurrentRow["Qty"] = Qty;
                CurrentRow["Rate"] = Rate;
                CurrentRow["Remarks"] = Description;
                CurrentRow["Comments"] = Comments;
            }
            else
            {
                if (TB_RevSheet == null) { TB_RevSheet = new(); }
                CurrentRow = TB_RevSheet.NewRow();
            }
        }


        #endregion

        #region Row -> Variables
        public void GetVariables(int _ID)
        {
            CurrentRow = GetDataRow(_ID);
            if (FoundRow)
            {
                ID = (int)CurrentRow["ID"];
                SheetNo = (string)CurrentRow["SheetNo"];
                SheetDate = (DateTime)CurrentRow["SheetDate"];
                Company = (int)CurrentRow["Company"];
                Project = (int)CurrentRow["Project"];
                Employee = (int)CurrentRow["Employee"];
                StockID = (int)CurrentRow["Stock"];
                TaxID = (int)CurrentRow["TaxID"];
                Qty = (decimal)CurrentRow["Qty"];
                Rate = (decimal)CurrentRow["Rate"];
                Description = (string)CurrentRow["Remarks"];
                Comments = (string)CurrentRow["Comments"];
            }
            else
            {
                ID = 0;
                SheetNo = string.Empty;
                SheetDate = (DateTime)CurrentRow["SheetDate"];
                Company = 0;
                Project = 0;
                Employee = 0;
                StockID = 0;
                TaxID = 0;
                Qty = 0.00M;
                Rate = 0.00M;
                Description = string.Empty; ;
                Comments = string.Empty; ;
            }



        }
        #endregion

        #region Record Save - (Insert - Update) 
        public void Save()
        {
            var Effected = 0;

            if (TB_RevSheet is not null)
            {
                if (CurrentRow != null)
                {
                    if ((int)CurrentRow["ID"] == 0)
                    {
                        if (Validated(SQLType.Insert))
                        {
                            var _Command = Commands.Insert(CurrentRow, DataFile);
                            Effected = _Command.ExecuteNonQuery();
                            MyMessage = $"{Effected} Record(s) updated.";

                        }
                    }

                    if ((int)CurrentRow["ID"] > 0)
                    {
                        if (Validated(SQLType.Update))
                        {
                            var _Command = Commands.UpDate(CurrentRow, DataFile);
                            Effected = _Command.ExecuteNonQuery();
                            MyMessage = $"{Effected} Record(s) updated.";
                        }
                    }
                    TB_RevSheet = DataSource.GetDataTable(DataFile, Tables.RevSheet);
                }
            }
        }

        #endregion


        #region Record Delete 
        public void Delete()
        {
            if (TB_RevSheet is not null)
            {
                if (CurrentRow != null)
                {
                    GetCurrentRow();

                    var _Command = Commands.Delete(CurrentRow, DataFile);
                    var Effected = _Command.ExecuteNonQuery();
                    MyMessage = $"{Effected} Record(s) deleted.";
                    TB_RevSheet = DataSource.GetDataTable(DataFile, Tables.RevSheet);
                }
            }
        }
        #endregion

        #region Validate Current row
        public bool Validated(SQLType _SQLType)
        {
            bool _Valid = true;
            if (CurrentRow != null)
            {
                if (CurrentRow["ID"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["SheetNo"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["SheetDate"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Company"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Project"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Employee"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["StockID"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["TaxID"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Rate"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Amount"] == DBNull.Value) { _Valid = false; }
                if (CurrentRow["Remarks"] == DBNull.Value) { _Valid = false; }

                if (_SQLType.Equals(SQLType.Insert))
                {
                    if ((int)CurrentRow["ID"] != 0) { }
                }


                if (_SQLType.Equals(SQLType.Update))
                {
                    if ((int)CurrentRow["ID"] < 1) { }
                }


                if (CurrentRow["SheetNo"].ToString().Length == 0) { }
                if ((DateTime)CurrentRow["SheetDate"] < AppRegistry.MinDate) { }

            }
            return _Valid;
        }
        #endregion
    }
}