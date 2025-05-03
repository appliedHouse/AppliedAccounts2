using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Models.Interface
{
    public interface IVoucher
    {
        DateTime LastVoucherDate { get; set; }
        DateTime MaxVouDate { get; set; }
        MessageClass MsgClass { get; set; }
        PrintService ReportService { get; set; }
        DataSource Source { get; set; }
        List<CodeTitle> Companies { get; set; } 
        List<CodeTitle> Employees { get; set; } 
        List<CodeTitle> Projects { get; set; } 
        List<CodeTitle> Accounts { get; set; } 
        string DataFile { get; set; }
        AppUserModel? UserProfile { get; set; }
        int Index { get; set; }
        int Count { get; }

        decimal Tot_DR { get; set; }
        decimal Tot_CR { get; set; }

        bool IsWaiting { get; set; }

        GlobalService AppGlobals { get; set; }


        void CalculateTotal();



        #region Load Data
        bool LoadData();
        #endregion

        #region Voucher Validation
        bool IsVoucherValidated();
        #endregion

        #region New and Edit Record

        void Edit(int _ID2);
        void New();
        #endregion

        #region Navigation
        void Top();
        void Next();
        void Back();
        void Last();
        #endregion

        #region Remove record from list
        void Remove();
        #endregion

        #region Add and Save Voucher
        void Save();
        Task<bool> SaveAllAsync();
        #endregion

        #region Print
        void Print(ReportType _rptType);
        ReportData GetReportData();
        ReportModel CreateReportModel();

        #endregion
    }
}