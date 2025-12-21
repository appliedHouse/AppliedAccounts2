using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;

namespace AppliedAccounts.Models.Interface
{
    public interface IVoucher
    {
        GlobalService AppGlobal { get; set; }
        DateTime LastVoucherDate { get; set; }
        DateTime MaxVouDate { get; set; }
        MessageClass MsgClass { get; set; }
        PrintService ReportService { get; set; }
        DataSource Source { get; set; }
        List<CodeTitle> Companies { get; set; }
        List<CodeTitle> Employees { get; set; }
        List<CodeTitle> Projects { get; set; }
        List<CodeTitle> Accounts { get; set; }

        int Index { get; set; }
        int Count { get; }

        decimal Tot_DR { get; set; }
        decimal Tot_CR { get; set; }

        bool IsWaiting { get; set; }


        void CalculateTotal();



        #region Load Data
        bool LoadData();
        #endregion

        #region Voucher Validation
        bool IsTransValidated();
        bool IsVoucherValidated();
        #endregion

        #region New and Edit Record

        void Edit(long _ID2);
        void New();
        #endregion

        #region Navigation
        void Top();
        void Next();
        void Back();
        void Last();
        #endregion

        #region Remove record from list
        //void Remove();
        void Remove(int _SrNo);
        #endregion

        #region Add and Save Voucher
        void Save();
        Task<bool> SaveAllAsync();
        #endregion

    }
}