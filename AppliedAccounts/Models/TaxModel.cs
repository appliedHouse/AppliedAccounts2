using AppliedAccounts.Data;
using System.Data;

namespace AppliedAccounts.Models
{
    public class TaxModel
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Rate { get; set; } = 0.00M;
        public int TaxType { get; set; }
        public int COA { get; set; }

        public DataTable TB_Tax { get; set; } = new();
        public DataRow CurrentRow { get; set; }
        public bool FoundRow { get; set; } = false;
        public string DataFile { get; set; } = string.Empty;


        public TaxModel(string _DataFile)
        {
            DataFile = _DataFile;
            CurrentRow = GetFirstRow();
        }

        private DataRow GetFirstRow()
        {

            if (TB_Tax != null)
            {
                return TB_Tax.NewRow();
            }
            else
            {
                TB_Tax = new DataTable();
                return TB_Tax.NewRow();
            }
        }

        public DataRow GetDataRow(int _ID)
        {
            FoundRow = false;
            DataRow _CurrentRow;
            if (TB_Tax is not null)
            {
                TB_Tax.DefaultView.RowFilter = $"ID={ID}";
                if (TB_Tax.DefaultView.Count > 0)
                {
                    FoundRow = true;
                    _CurrentRow = TB_Tax.DefaultView[0].Row;
                }
                else
                {
                    _CurrentRow = TB_Tax.NewRow();
                }
            }
            else
            {
                // if Book is null
                TB_Tax = new();
                _CurrentRow = TB_Tax.NewRow();
            }

            return _CurrentRow;
        }

        public DataRow GetRowIndex(int _Index)
        {
            FoundRow = false;
            DataRow _CurrentRow;
            if (TB_Tax != null)
            {

                if (TB_Tax.Rows.Count == 1)
                {
                    FoundRow = true;
                    _CurrentRow = TB_Tax.Rows[_Index];
                }
                else
                {
                    _CurrentRow = TB_Tax.NewRow();
                }
            }
            else
            {
                TB_Tax = new();
                _CurrentRow = TB_Tax.NewRow();
            }
            return _CurrentRow;
        }

        //private bool Validate(DataRow _Row)
        //{
        //    if (TB_Tax != null) { return true; }
        //    return false;
        //}


        public void GetCurrentRow()
        {
            if (TB_Tax is not null)
            {
                CurrentRow = TB_Tax.NewRow();
                CurrentRow["ID"] = ID;
                CurrentRow["Code"] = Code;
                CurrentRow["Title"] = Title;
                CurrentRow["Rate"] = Rate;
                CurrentRow["TaxType"] = TaxType;
                CurrentRow["COA"] = COA;
            }
        }

        public void GetVariables(int _ID)
        {
            CurrentRow = GetDataRow(_ID);
            if (FoundRow)
            {
                ID = (int)CurrentRow["ID"];
                Code = (string)CurrentRow["Code"];
                Title = (string)CurrentRow["Title"];
                Rate = (decimal)CurrentRow["Rate"];
                TaxType = (int)CurrentRow["TaxType"];
                COA = (int)CurrentRow["COA"];

            }
            else
            {
                ID = 0;
                Code = string.Empty;
                Title = string.Empty;
                Rate = 0.00M;
                TaxType = 0;
                COA = 0;
            }
        }

        //public int SaveChanges()
        //{
        //    AppMessages.Message _Effected = new();
        //    if (Validate(CurrentRow))
        //    {
        //        _Effected = AppFunctions.Save(DataFile, TB_Tax, CurrentRow);
        //    }
        //    return _Effected.RowEffected;
        //}
    }
}
