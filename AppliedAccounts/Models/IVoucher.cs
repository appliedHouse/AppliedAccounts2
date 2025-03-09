using AppliedDB;
using AppMessages;

namespace AppliedAccounts.Models
{
    public interface IVoucher
    {
        DateTime LastVoucherDate { get; set; }
        DateTime MaxVouDate { get; set; }
        MessageClass MsgClass { get; set; }
        Voucher MyVoucher { get; set; }
        bool Processing { get; set; }
        DataSource Source { get; set; }
        string DataFile { get; set; }


        void Edit(int _SrNo);
        void New();
        
        #region Navigation
        void Top();
        void Next();
        void Back();
        void Last();
        #endregion

        void Remove(int _SrNo);

        #region Add and Save Voucher
        void Save();
        Task SaveAllAsync();
        #endregion

        bool IsVoucherValidated();
        bool LoadData();


    }
}