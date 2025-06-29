﻿using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using AppliedDB;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class BooksList
    {

        //public AppliedGlobals.AppUserModel UserProfile { get; set; }
        public BookListModel MyModel { get; set; } = new();
        public NavigationManager NavManager { get; set; }

        public BooksList() { }

        #region Back Page
        public void Back() { AppGlobal.NavManager.NavigateTo("/Menu/Accounts"); }
        #endregion

        #region New Voucher
        public void New()
        {
            if (MyModel.BookID > 0)
            {
                NavManager.NavigateTo($"/Accounts/Books/{MyModel.VoucherID}/{MyModel.BookID}");
            }
        }
        #endregion

        #region Refresh Page
        public async void Refresh()
        {
            MyModel.SetKeys();              // Save the current page setting in Registry 
            MyModel.Pages = new();          // Reset the page model
            //MyModel.LoadData();             // Load Data according to dates and search text.
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        public string GetTitle(List<CodeTitle> _List, int _Value)
        {
            if (_List == null || _List.Count == 0) { return string.Empty; }
            return _List.FirstOrDefault(x => x.ID == _Value)!.Title ?? "";

        }

        #region Print
        public async Task Print(ReportActionClass reportAction)
        {
            MyModel.IsWaiting = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 

            try
            {
                MyModel.VoucherID = reportAction.VoucherID;
                await Task.Run(() => { MyModel.Print(reportAction.PrintType); });
            }
            catch (Exception)
            {
                MyModel.MsgClass.Add(AppMessages.Enums.Messages.prtReportError);
            }

            MyModel.IsWaiting = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 
        }
        #endregion
    }


    public class BookView
    {
        public int ID { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Description { get; set; }
        public decimal Recevied { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public string TReceived { get; set; }
        public string TPaid { get; set; }
        public string TBalance { get; set; }

    }

}
