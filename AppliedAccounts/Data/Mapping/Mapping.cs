using AppliedAccounts.Models;
using System.Data;

namespace AppliedAccounts.Data.Mapping
{
    public static class Mapping
    {
        public static DataRow? ToDataRow(this COARecord rec, DataRow row)
        {
            if (rec == null) { return null; }
            row["ID"] = rec.ID;
            row["Code"] = rec.Code;
            row["Title"] = rec.Title;
            row["Class"] = rec.Class;
            row["Nature"] = rec.Nature;
            row["Notes"] = rec.Notes;
            row["OPENING_BALANCE"] = rec.OBal;
            return row;
        }

        public static DataRow ToDataRow(this COANatureRecord rec, DataRow row)
        {
            row["Id"] =    rec.ID;
            row["Code"] =  rec.Code;
            row["Title"] = rec.Title;
            return row;
        }
    }
}
