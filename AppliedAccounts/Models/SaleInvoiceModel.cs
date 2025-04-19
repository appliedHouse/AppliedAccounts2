using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using System.Data;
using static AppliedAccounts.Data.AppRegistry;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;
using static AppliedDB.Enums.Status;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using AppliedAccounts.Services;
using SQLQueries;

namespace AppliedAccounts.Models
{
    public class SaleInvoiceModel : IVoucher
    {
        #region Variables
        public int SaleInvoiceID { get; set; }
        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public Voucher MyVoucher { get; set; } = new();
        public Total Totals { get; set; } = new();

        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Inventory { get; set; }
        public List<CodeTitle> Taxes { get; set; }
        public List<CodeTitle> Units { get; set; }
        public MessageClass MsgClass { get; set; } = new();
        public PrintService ReportService { get; set; }
        public DateTime LastVoucherDate { get; set; }
        public DateTime MaxVouDate { get; set; }
        public NavigationManager NavManager { get; set; }
        public string DataFile { get; set; }
        public int Index { get; set; }                  // Index point of Detail Table
        public decimal Tot_DR { get; set; }             // Total DR of Voucher in DB 
        public decimal Tot_CR { get; set; }             // Total CR of Voucher in DB
        public bool IsWaiting { get; set; }             // Page is wait for completion of process like save or data load
        public int Count => MyVoucher.Details.Count;    // total records in detail list.


        #endregion

        #region Constructor
        public SaleInvoiceModel() { }
        public SaleInvoiceModel(AppUserModel _UserProfile)
        {
            UserProfile = _UserProfile;
            Source = new DataSource(UserProfile);
        }

        public SaleInvoiceModel(AppUserModel _UserProfile, int _SaleInvoiceID)
        {
            UserProfile = _UserProfile;
            SaleInvoiceID = _SaleInvoiceID;
            Source = new DataSource(UserProfile);
            Start(SaleInvoiceID);

        }
        #endregion

        #region Start
        public void Start(int _SaleInvoiceID)
        {
            try
            {
                if (UserProfile is null) { return; }
                Source ??= new(UserProfile);

                MsgClass = new();
                MyVoucher = new();
                LastVoucherDate = GetDate(Source.DBFile, "SInvDate");           // Sale Invoice Date

                SaleInvoiceID = _SaleInvoiceID;
                DataFile = UserProfile.DataFile;
                Companies = Source.GetCustomers();
                Accounts = Source.GetAccounts();
                Employees = Source.GetEmployees();
                Projects = Source.GetProjects();
                Inventory = Source.GetInventory();
                Taxes = Source.GetTaxes();
                Units = Source.GetUnits();

                if (SaleInvoiceID == 0) { MyVoucher = NewVoucher(); }
                else
                {
                    LoadData();
                    CalculateTotal();
                }

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
        }

        private Voucher NewVoucher()
        {
            var _MyVoucher = new Voucher();
            _MyVoucher.Master.ID1 = 0;
            _MyVoucher.Master.Vou_No = "NEW";
            _MyVoucher.Master.Vou_Date = DateTime.Now;
            _MyVoucher.Master.Company = 0;
            _MyVoucher.Master.Employee = 0;
            _MyVoucher.Master.Ref_No = "";
            _MyVoucher.Master.Inv_No = "";
            _MyVoucher.Master.Inv_Date = DateTime.Now;
            _MyVoucher.Master.Pay_Date = DateTime.Now;
            _MyVoucher.Master.Remarks = "";
            _MyVoucher.Master.Comments = "";
            _MyVoucher.Master.Status = Submitted.ToString();
            _MyVoucher.Master.TitleCompany = "";
            _MyVoucher.Master.TitleEmployee = "";


            _MyVoucher.Detail.ID2 = 0;
            _MyVoucher.Detail.Sr_No = 0;
            _MyVoucher.Detail.Inventory = 0;
            _MyVoucher.Detail.Batch = "";
            _MyVoucher.Detail.Qty = 0.00M;
            _MyVoucher.Detail.Rate = 0.00M;
            _MyVoucher.Detail.TaxID = 0;
            _MyVoucher.Detail.TaxRate = 0.00M;
            _MyVoucher.Detail.Description = "";
            _MyVoucher.Detail.Project = 0;
            _MyVoucher.Detail.TitleInventory = "";
            _MyVoucher.Detail.TitleProject = "";
            _MyVoucher.Detail.TitleTaxID = "";
            return _MyVoucher;


        }
        #endregion

        #region Save
        public void Save()
        {
            if (IsVoucherValidated())
            {
                var IsSrNo = MyVoucher.Details.Where(e => e.Sr_No == MyVoucher.Detail.Sr_No).Any();
                if (!IsSrNo)
                {
                    MyVoucher.Detail.Action = "save";
                    MyVoucher.Details.Add(MyVoucher.Detail);
                }
            }
            else
            {
                MsgClass.Add(MESSAGE.RecordNotValidated);
            }
            CalculateTotal();

        }
        #endregion

        #region Get Report table
        //public DataTable GetReportTable()
        //{
        //    return Source.GetTable(AppliedDB.Enums.Query.SaleInvoice, InvoiceID);
        //}
        #endregion

        #region Convert decimal to String Text
        public string ToAmount(object _Object)
        {
            return Conversion.ToAmount(_Object, Format.Number);
        }
        #endregion

        #region Calculation of totals
        public void CalculateTotal()
        {
            Totals.Tot_Qty = MyVoucher.Details.Sum(x => x.Qty);
            Totals.Tot_Gross = MyVoucher.Details.Sum(x => x.Gross);
            Totals.Tot_TaxAmount = MyVoucher.Details.Sum(x => x.TaxAmount);
            Totals.Tot_NetAmount = MyVoucher.Details.Sum(x => x.NetAmount);

            Tot_CR = MyVoucher.Details.Sum(x => x.NetAmount);
            Tot_DR = MyVoucher.Details.Sum(x => x.NetAmount);
        }

        public decimal GetGross() { return MyVoucher.Detail.Qty * MyVoucher.Detail.Rate; }
        public decimal GetTaxAmount() { return GetGross() * MyVoucher.Detail.TaxRate; }
        public decimal GetNetAmount() { return GetGross() + GetTaxAmount(); }

        #endregion

        #region Load Data
        public bool LoadData()
        {
            try
            {
                var VoucherData = Source.GetSalesInvoice(SaleInvoiceID).AsEnumerable().ToList();

                if (VoucherData != null)
                {
                    if (VoucherData.Count > 0)
                    {

                        MyVoucher.Master = VoucherData!.Select(first => new Master()
                        {

                            ID1 = first.Field<int>("ID1"),
                            Vou_No = first.Field<string>("Vou_No") ?? "",
                            Vou_Date = first.Field<DateTime>("Vou_Date"),
                            Company = first.Field<int>("Company"),
                            Employee = first.Field<int>("Employee"),
                            Ref_No = first.Field<string>("Ref_No") ?? "",
                            Inv_No = first.Field<string>("Inv_No") ?? "",
                            Inv_Date = first.Field<DateTime>("Inv_Date"),
                            Pay_Date = first.Field<DateTime>("Pay_Date"),
                            Amount = first.Field<decimal>("Amount"),
                            Remarks = first.Field<string>("Description") ?? "",
                            Comments = first.Field<string>("Comments") ?? "",
                            Status = first.Field<string>("Status") ?? "",
                        }).First() ?? new();

                        MyVoucher.Details = [.. VoucherData.Select(row => new Detail()
                        {
                            ID2 = row.Field<int>("ID2"),
                            TranID = row.Field<int>("TranID"),
                            Sr_No = row.Field<int>("Sr_No"),
                            Inventory = row.Field<int>("Inventory"),
                            Batch = row.Field<string>("Batch") ?? "",
                            Qty = row.Field<decimal>("Qty"),
                            Rate = row.Field<decimal>("Rate"),
                            TaxID = row.Field<int>("Tax"),
                            TaxRate = row.Field<decimal>("Tax_Rate"),
                            Description = row.Field<string>("Description2") ?? "",
                            Project = row.Field<int>("Project"),
                            Unit = row.Field<int>("Unit"),

                            TitleInventory = Source.SeekTitle(Tables.Inventory, row.Field<int>("Inventory")),
                            TitleProject = Source.SeekTitle(Tables.Project, row.Field<int>("Project")),
                            TitleTaxID = Source.SeekTitle(Tables.Taxes, row.Field<int>("Tax")),
                            TitleUnit = Source.SeekTitle(Tables.Inv_UOM, row.Field<int>("Unit")),

                            })];

                        if (MyVoucher.Details.Count > 0)
                        {
                            MyVoucher.Detail = MyVoucher.Details.First();
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MsgClass.Error(ex.Message);
            }
            return false;

        }
        #endregion

        #region Is Voucher is valided 
        public bool IsVoucherValidated()
        {
            bool IsValid = true;
            MsgClass = new();

            if (MyVoucher.Master == null) { MsgClass.Add(MESSAGE.MasterRecordisNull); return false; }
            if (MyVoucher.Details == null) { MsgClass.Add(MESSAGE.DetailRecordsisNull); return false; }

            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (!MyVoucher.Master.Vou_No.ToLower().Equals("new"))
            {
                if (MyVoucher.Master.Vou_No.Length != 11) { MsgClass.Add(MESSAGE.VouNoNotDefineProperly); }
            }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.Company == 0) { MsgClass.Add(MESSAGE.Row_CompanyIDZero); }
            if (MyVoucher.Master.Employee == 0) { MsgClass.Add(MESSAGE.Row_EmployeeIDZero); }
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }

            if (MyVoucher.Detail.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
            if (MyVoucher.Detail.Inventory == 0) { MsgClass.Add(MESSAGE.Row_InventoryIDZero); }
            if (MyVoucher.Detail.TaxID == 0) { MsgClass.Add(MESSAGE.Row_TaxIDZero); }
            if (MyVoucher.Detail.Unit == 0) { MsgClass.Add(MESSAGE.Row_UnitIDZero); }
            if (MyVoucher.Detail.Qty == 0) { MsgClass.Add(MESSAGE.Row_QtyZero); }
            if (MyVoucher.Detail.Rate == 0) { MsgClass.Add(MESSAGE.Row_RateZero); }
            if (MyVoucher.Detail.Gross == 0) { MsgClass.Add(MESSAGE.Row_GrossAmountZero); }
            if (MyVoucher.Detail.Description.Length == 0) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
            if (MsgClass.Count > 0) { IsValid = false; }

            return IsValid;
        }
        #endregion

        #region Edit and New Voucher
        public void Edit(int _ID2)
        {
            var _Detail = MyVoucher.Detail;
            MyVoucher.Detail = MyVoucher.Details.Where(e => e.ID2 == _ID2).First() ?? _Detail;
        }

        public void New()
        {
            MyVoucher.Detail = NewDetail();
        }
        #endregion

        #region Get New Detail (Empty)
        public Detail NewDetail()
        {
            MyVoucher.Details ??= [];           // Construct new if found null;

            var _Detail = new Detail(); ;
            _Detail.ID2 = 0;
            _Detail.Sr_No = MyVoucher.Details.Max(x => x.Sr_No) + 1;
            _Detail.Inventory = 0;
            _Detail.Batch = "";
            _Detail.Qty = 0.00M;
            _Detail.Rate = 0.00M;
            _Detail.TaxID = 0;
            _Detail.TaxRate = 0.00M;
            _Detail.Description = "";
            _Detail.Project = 0;
            _Detail.Unit = 0;
            return _Detail;
        }
        #endregion

        #region Navigation of Records
        public void Top()
        {
            Index = 1;
            if (MyVoucher.Details.Count > 0)
            { MyVoucher.Detail = MyVoucher.Details.First(); }

        }

        public void Next()
        {
            if (MyVoucher.Details.Count > 0)
            {
                Index = MyVoucher.Details.IndexOf(MyVoucher.Detail) + 1;
                var Counter = MyVoucher.Details.Count - 1;
                if (Index > Counter) { Index = Counter; }
                MyVoucher.Detail = MyVoucher.Details[Index];
            }
        }

        public void Back()
        {
            if (MyVoucher.Details.Count > 0)
            {
                Index = MyVoucher.Details.IndexOf(MyVoucher.Detail) - 1;
                if (Index < 0) { Index = 0; }
                MyVoucher.Detail = MyVoucher.Details[Index];
            }
        }

        public void Last()
        {
            Index = MyVoucher.Details.Count - 1;
            if (MyVoucher.Details.Count > 0)
            { MyVoucher.Detail = MyVoucher.Details.Last(); }
        }
        #endregion

        #region Remove
        public void Remove()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Save All
        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Print
        public void Print(int _ID)
        {
            ReportService = new()
            {
                RptData = GetReportData(_ID),              // always generate Data for report
                RptModel = CreateReportModel(_ID),         // and then generate report parameters
            };
            ReportService.Generate();                       // Generate Report & Create reports byte[]
        }
        public ReportData GetReportData(int ID)
        {
            var _Query = Quries.SaleInvoice(ID);
            var _Table = Source.GetTable(_Query);
            var _ReportData = new ReportData()
            {
                ReportTable = _Table,
                DataSetName = "ds_SaleInvoice"
            };
            return _ReportData;
        }
        public ReportModel CreateReportModel(int _ID)
        {
            var _Heading1 = "Sale Invoice";
            var _Heading2 = $"{_Heading1} [{MyVoucher.Master.Vou_No}]";
            var _ReportPath = UserProfile!.ReportFolder;
            var _CompanyName = UserProfile.Company;
            var _ReportFooter = AppFunctions.ReportFooter();

            ReportModel rptModel = new();

            rptModel.InputReport.FileName = $"SalesInvoice";
            rptModel.InputReport.FileExtention = ".rdl";
            rptModel.InputReport.FilePath = UserProfile!.ReportFolder;

            rptModel.OutputReport.FileName = $"Receipt_{_ID}";
            rptModel.OutputReport.FilePath = UserProfile!.PDFFolder;

            rptModel.AddReportParameter("CompanyName", _CompanyName);
            rptModel.AddReportParameter("Heading1", _Heading1);
            rptModel.AddReportParameter("Heading2", _Heading2);
            rptModel.AddReportParameter("Footer", _ReportFooter);

            return rptModel;
        }

        #endregion

        #region Model
        public class Voucher
        {
            public Voucher()
            {
                Master = new();
                Detail = new();
                Details = [];
            }

            public Master Master { get; set; } = new();
            public Detail Detail { get; set; } = new();
            public List<Detail> Details { get; set; } = new();
        }
        public class Master
        {
            public Master() { }
            public int ID1 { get; set; }
            public string Vou_No { get; set; } = string.Empty;
            public DateTime Vou_Date { get; set; }
            public int Company { get; set; }
            public int Employee { get; set; }
            public string Ref_No { get; set; } = string.Empty;
            public string Inv_No { get; set; } = string.Empty;
            public DateTime Inv_Date { get; set; }
            public DateTime Pay_Date { get; set; }
            public decimal Amount { get; set; }
            public string Remarks { get; set; } = string.Empty;
            public string Comments { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;

            // Titles  Non-Database Variabels

            public string TitleCompany { get; set; } = string.Empty;
            public string TitleEmployee { get; set; } = string.Empty;

        }
        public class Detail
        {
            public Detail() { }
            public int ID2 { get; set; }
            public int TranID { get; set; }
            public int Sr_No { get; set; }
            public int Inventory { get; set; }
            public string Batch { get; set; } = string.Empty;
            public decimal Qty { get; set; } = 0.00M;
            public decimal Rate { get; set; } = 0.00M;
            public int TaxID { get; set; }
            public decimal TaxRate { get; set; } = 0.00M;
            public string Description { get; set; } = string.Empty;
            public int Project { get; set; }

            // Non DataBase Variables

            public decimal Gross => decimal.Parse((Qty * Rate).ToString());
            public decimal TaxAmount => decimal.Parse((Gross * (TaxRate / 100)).ToString());
            public decimal NetAmount => decimal.Parse((Gross + TaxAmount).ToString());

            public string TitleInventory { get; set; } = string.Empty;
            public string TitleProject { get; set; } = string.Empty;
            public string TitleTaxID { get; set; } = string.Empty;
            public int Unit { get; set; }                                       // Unit ID get from inventory
            public string TitleUnit { get; set; } = string.Empty;
            public string Action { get; set; } = string.Empty; // action like save, delete, update

        }
        public class Total
        {
            public string NumberFormat { get; set; } = Format.Digit;
            public decimal Tot_Qty { get; set; } = 0.00M;
            public decimal Tot_Gross { get; set; } = 0.00M;
            public decimal Tot_TaxAmount { get; set; } = 0.00M;
            public decimal Tot_NetAmount { get; set; } = 0.00M;

            public string Txt_Qty => Conversion.ToAmount(Tot_Qty, NumberFormat);
            public string Txt_Amount => Conversion.ToAmount(Tot_Gross, NumberFormat);
            public string Txt_TaxAmount => Conversion.ToAmount(Tot_TaxAmount, NumberFormat);
            public string Txt_NetAmount => Conversion.ToAmount(Tot_NetAmount, NumberFormat);

        }
        #endregion

    }
}
