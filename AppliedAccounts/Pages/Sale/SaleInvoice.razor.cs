using AppliedAccounts.Models;
using AppliedDB;
using AppReports;
using Format = AppliedGlobals.AppValues.Format;

namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoice
    {
        #region Variables

        public SaleInvoiceModel MyModel { get; set; } = new();
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }
        private bool IsWaiting { get; set; }
        private string SpinnerMessage { get; set; }

        #endregion

        #region Constructor
        public SaleInvoice()
        {
            IsPageValid = true;
            IsWaiting = false;
            ErrorMessage = string.Empty;
        }
        #endregion

        #region DropDown List change
        public void CompanyChanged(long _NewValue)
        {
            MyModel.MyVoucher.Master.Company = _NewValue;
            MyModel.MyVoucher.Master.TitleCompany = MyModel.Companies.First(e => e.ID == _NewValue).Title ?? "";
        }
        public void EmployeeChanged(long _NewValue)
        {
            MyModel.MyVoucher.Master.Employee = _NewValue;
            MyModel.MyVoucher.Master.TitleEmployee = MyModel.Employees.First(e => e.ID == _NewValue).Title ?? "";
        }
        public void InventoryChanged(long _NewValue)
        {
            MyModel.MyVoucher.Detail.Inventory = _NewValue;
            MyModel.MyVoucher.Detail.TitleInventory = MyModel.Inventory.First(e => e.ID == _NewValue).Title ?? "";
        }
        public void TaxChanged(long _NewValue)
        {
            MyModel.MyVoucher.Detail.TaxID = _NewValue;
            MyModel.MyVoucher.Detail.TitleTaxID = MyModel.Taxes.First(e => e.ID == _NewValue).Title ?? "";
            MyModel.MyVoucher.Detail.TaxRate = MyModel.Source.SeekTaxRate(MyModel.MyVoucher.Detail.TaxID);
        }
        public void UnitChanged(long _NewValue)
        {
            MyModel.MyVoucher.Detail.Unit = _NewValue;
            MyModel.MyVoucher.Detail.TitleUnit = MyModel.Units.First(e => e.ID == _NewValue).Title ?? "";
        }
        public void ProjectChanged(long _NewValue)
        {
            MyModel.MyVoucher.Detail.Project = _NewValue;
            MyModel.MyVoucher.Detail.TitleProject = MyModel.Projects.First(e => e.ID == _NewValue).Title ?? "";
        }
        #endregion

        #region Submit & Digitis
        public void Submit()
        {

        }


        public bool ShowDigits = false;
        public void Digits()
        {
            if (ShowDigits)
            {
                MyModel.Totals.NumberFormat = Format.Number;
            }
            else
            {
                MyModel.Totals.NumberFormat = Format.Digit;
            }
            //MyModel.CalculateTotal();
        }
        #endregion

        #region Save Invoice to DB
        public async void SaveAll()
        {
            var IsSaved = await MyModel.SaveAllAsync();

            await InvokeAsync(StateHasChanged);

            if (IsSaved)
            {
                ToastService.ShowSuccess($"Successfully saved {MyModel.MyVoucher.Master.Vou_No}"); // show the toast
                NavManager.NavigateTo($"/Sale/SaleInvoice/{MyModel.MyVoucher.Master.ID1}");
            }
            else
            {
                ToastService.ShowError($"Failed to save {MyModel.MyVoucher.Master.Vou_No}"); // show the toast
            }
        }
        #endregion

        #region Delete
        public void Delete(int _Sr_No)
        {
            MyModel.MyVoucher.Detail = MyModel.MyVoucher.Details.Where(row => row.Sr_No == _Sr_No).First();
            if (MyModel.MyVoucher.Detail is not null)
            {
                MyModel.Deleted.Add(MyModel.MyVoucher.Detail);                  // Save in deleted list
                MyModel.MyVoucher.Details.Remove(MyModel.MyVoucher.Detail);     // remove from detail list
                MyModel.MsgClass.Add(AppMessages.Enums.Messages.RowDeleted);    // Set message to display after deleted.
            }

        }
        #endregion

        #region Home & Back Buttons
        public void GotoHome()
        {
            NavManager.NavigateTo("/");
        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Sale/SaleInvoiceList");
        }

       

        #endregion

        #region Print
        public void Print()
        {
            //ReportModel Reportmodel = Model.Print();

        }

        public void Print(ReportType RptType)
        {

        }


        #endregion


        #region Test
        public void Test()
        {
            MyModel = new(AppGlobal);
            MyModel.Source ??= new(AppGlobal.AppPaths);

            MyModel.MyVoucher.Master.ID1 = 0;
            MyModel.MyVoucher.Master.Inv_Date = new DateTime(2026, 08, 12);
            MyModel.MyVoucher.Master.Pay_Date = new DateTime(2026, 08, 12);
            MyModel.MyVoucher.Master.Vou_Date = new DateTime(2026, 08, 12);
            MyModel.MyVoucher.Master.Vou_No = "New";
            MyModel.MyVoucher.Master.Employee = 0;
            MyModel.MyVoucher.Master.Company = 3;
            MyModel.MyVoucher.Master.Ref_No = "FBR-001";
            MyModel.MyVoucher.Master.Inv_No = "FBR-001";
            MyModel.MyVoucher.Master.Amount = 0;
            MyModel.MyVoucher.Master.Remarks = "FBR Invoice 001";
            MyModel.MyVoucher.Master.Comments = "FBR Invoice 001";
            MyModel.MyVoucher.Master.Status = VoucherTypeClass.VoucherStatus.Submitted.ToString();
            MyModel.MyVoucher.Master.TitleCompany = MyModel.Source.SeekTitle(Enums.Tables.Customers, 3);
            MyModel.MyVoucher.Master.TitleEmployee = "";

            MyModel.MyVoucher.Detail.ID2 = 0;
            MyModel.MyVoucher.Detail.TranID = 0;
            MyModel.MyVoucher.Detail.Sr_No = 1;
            MyModel.MyVoucher.Detail.Inventory = 7;
            MyModel.MyVoucher.Detail.Batch = "FBR-May2026";
            MyModel.MyVoucher.Detail.Qty = 1;
            MyModel.MyVoucher.Detail.Rate = 561000.00M;
            MyModel.MyVoucher.Detail.Unit = 2;
            MyModel.MyVoucher.Detail.TaxID = 2;
            MyModel.MyVoucher.Detail.TaxRate = 18.00M;
            MyModel.MyVoucher.Detail.Description = "FBR Invoice 001";
            MyModel.MyVoucher.Detail.Project = 0;



        }
        #endregion

    }
}