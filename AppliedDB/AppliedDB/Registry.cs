using System.Data;
using System.Data.SQLite;
using static AppliedGlobals.AppErums;

namespace AppliedDB
{
    public class Registry(SQLiteConnection _Connection, string _UserName)
    {
        public SQLiteConnection MyConnection { get; set; } = _Connection;
        public List<string> Messages { get; set; } = new();
        public bool IsConnected { get; set; }
        public string UserName { get; set; } = _UserName;

        #region Set Registry Key
        public bool SetKey(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }

            DataTable TB_Registry = DataSource.GetDataTable(Enums.Tables.Registry, MyConnection, $"Code = '{Key}'");
            DataRow CurrentRow ;
            string SQLAction;

            if (TB_Registry.Rows.Count == 1)
            {
                SQLAction = "Update";
                CurrentRow = TB_Registry.DefaultView[0].Row;
            }
            else
            {
                SQLAction = "Insert";
                CurrentRow = TB_Registry.NewRow();
                CurrentRow["ID"] = 0;
            }

            CurrentRow["Code"] = Key;
            CurrentRow["Title"] = _Title;
            CurrentRow["UserName"] = UserName;
            switch (keytype)
            {
                case KeyTypes.Number:
                    CurrentRow["nValue"] = KeyValue;
                    break;
                case KeyTypes.Currency:
                    CurrentRow["mValue"] = KeyValue;
                    break;
                case KeyTypes.Date:
                    CurrentRow["dValue"] = KeyValue;
                    break;
                case KeyTypes.Boolean:
                    CurrentRow["bValue"] = KeyValue;
                    break;
                case KeyTypes.Text:
                    CurrentRow["cValue"] = KeyValue;
                    break;
                case KeyTypes.From:
                    CurrentRow["From"] = KeyValue;
                    break;
                case KeyTypes.To:
                    CurrentRow["To"] = KeyValue;
                    break;
                default:
                    break;
            }

            if (SQLAction == "Insert") { var cmd = Commands.Insert(CurrentRow, MyConnection); cmd?.Connection.Open(); cmd?.ExecuteNonQuery(); cmd?.Connection.Close(); return true; }
            if (SQLAction == "Update") { var cmd = Commands.UpDate(CurrentRow, MyConnection); cmd?.Connection.Open(); cmd?.ExecuteNonQuery(); cmd?.Connection.Close(); return true; }

            MyConnection.Close();
            return false;
        }
        #endregion

        #region Get Registry Key
        public object GetKey(string Key, KeyTypes keytype)
        {

            object ReturnValue;
            string _SQLQuery = $"SELECT * FROM [Registry] WHERE Code = '{Key}'";
            DataTable Registry = DataSource.GetQueryTable(_SQLQuery, MyConnection);

            if (Registry.Rows.Count == 1)
            {
                DataRow Row = Registry.Rows[0];
                ReturnValue = keytype switch
                {
                    KeyTypes.Number => Row["nValue"],
                    KeyTypes.Currency => Row["mValue"],
                    KeyTypes.Boolean => Row["bValue"],
                    KeyTypes.Date => Row["dValue"],
                    KeyTypes.Text => Row["cValue"],
                    _ => string.Empty
                };
            }
            else
            {
                ReturnValue = keytype switch
                {
                    KeyTypes.Number => 0,
                    KeyTypes.Currency => 0.00,
                    KeyTypes.Boolean => false,
                    KeyTypes.Date => DateTime.Now,
                    KeyTypes.Text => string.Empty,
                    _ => string.Empty
                };
            }
            return ReturnValue;
        }
        #endregion

    }




}

