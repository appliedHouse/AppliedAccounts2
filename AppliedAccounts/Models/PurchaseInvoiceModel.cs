using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using SQLQueries;
using System.Data;
using static AppliedAccounts.Data.AppRegistry;
using static AppliedDB.Enums.Status;
using Format = AppliedGlobals.AppValues.Format;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;


namespace AppliedAccounts.Models
{
    public class PurchaseInvoiceModel
    {

        #region Variables
        public long PurchaseInvoiceID { get; set; }
        public DataSource Source { get; set; }
        public Voucher MyVoucher { get; set; } = new();
        public List<Detail> Deleted { get; set; } = new();
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
        public int ListType { get; set; }               // List type for display in View Table at page
        public GlobalService AppGlobal { get; set; }


        #endregion

        #region Constructor
        public PurchaseInvoiceModel() { }
        public PurchaseInvoiceModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new DataSource(AppGlobal.AppPaths);
        }

        public PurchaseInvoiceModel(GlobalService _AppGlobal, long _PurchaseInvoiceID)
        {
            AppGlobal = _AppGlobal;
            PurchaseInvoiceID = _PurchaseInvoiceID;
            Source = new DataSource(AppGlobal.AppPaths);
            Start(PurchaseInvoiceID);

        }
        #endregion

        #region Start
        public void Start(long _PurchaseInvoiceID)
        {
            try
            {
                if (AppGlobal is null) { return; }
                Source ??= new(AppGlobal.AppPaths);

                MsgClass = new();
                MyVoucher = new();
                LastVoucherDate = GetDate(Source.DBFile, "PInvDate");           // Purchase Invoice Date

                PurchaseInvoiceID = _PurchaseInvoiceID;
                //DataFile = UserProfile.DataFile;
                Companies = Source.GetCustomers();
                Accounts = Source.GetAccounts();
                Employees = Source.GetEmployees();
                Projects = Source.GetProjects();
                Inventory = Source.GetInventory();
                Taxes = Source.GetTaxes();
                Units = Source.GetUnits();

                if (PurchaseInvoiceID == 0) { MyVoucher = NewVoucher(); }
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
            _MyVoucher.Detail.Sr_No = 1;
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
                var _Filter = $"[ID]={PurchaseInvoiceID}";
                var _Query = Quries.BillPayable(_Filter);

                var VoucherData = Source.GetTable(_Query).AsEnumerable().ToList(); // Get Data 

                if (VoucherData?.Count > 0)
                {
                    // MASTER (only first row)
                    MyVoucher.Master = VoucherData
                        .Select(first => new Master
                        {
                            ID1 = first.GetInt64("ID1"),
                            Vou_No = first.GetString("Vou_No"),
                            Vou_Date = first.GetDate("Vou_Date"),
                            Company = first.GetInt64("Company"),
                            Employee = first.GetInt64("Employee"),
                            Ref_No = first.GetString("Ref_No"),
                            Inv_No = first.GetString("Inv_No"),
                            Inv_Date = first.GetDate("Inv_Date"),
                            Pay_Date = first.GetDate("Pay_Date"),
                            Amount = first.GetDecimal("Amount"),
                            Remarks = first.GetString("Remarks"),
                            Comments = first.GetString("Comments"),
                            Status = first.GetString("Status"),

                            TitleCompany = first.GetString("TitleSupplier"),
                            TitleEmployee = first.GetString("TitleEmployee"),
                        })
                        .FirstOrDefault() ?? new Master();

                    // DETAILS
                    MyVoucher.Details = VoucherData
                        .Select(row =>
                        {
                            var qty = row.GetDecimal("Qty");
                            var rate = row.GetDecimal("Rate");
                            var taxRate = row.GetDecimal("Tax_Rate");

                            return new Detail
                            {
                                ID2 = row.GetInt64("ID2"),
                                TranID = row.GetInt64("TranID"),
                                Sr_No = row.GetInt32("Sr_No"),
                                Inventory = row.GetInt64("Inventory"),
                                Batch = row.GetString("Batch"),
                                Unit = row.GetInt64("Unit"),

                                Qty = qty,
                                Rate = rate,

                                TaxID = row.GetInt64("Tax"),
                                TaxRate = taxRate,

                                Description = row.GetString("Description"),
                                Project = row.GetInt64("Project"),

                                TitleInventory = row.GetString("TitleStock"),
                                TitleProject = row.GetString("TitleProject"),
                                TitleTaxID = row.GetString("TitleTax"),
                                TitleUnit = row.GetString("TitleUnit"),
                            };
                        })
                        .ToList();

                    // CURRENT DETAIL
                    MyVoucher.Detail = MyVoucher.Details.FirstOrDefault();

                    return true;
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
        public bool IsVoucherValidated()            // Validate Master and Detail both of all record
        {
            bool IsValid = true;
            MsgClass.ClearMessages();
            if (!IsTransValidated())                // Temp code. update in future 20-DEC-2025
            {
                IsValid = false;
            }
            return IsValid;
        }

        public bool IsTransValidated()
        {
            bool IsValid = true;
            MsgClass ??= new();
            MsgClass.ClearMessages();

            if (MyVoucher.Master == null) { MsgClass.Add(MESSAGE.MasterRecordisNull); return false; }
            if (MyVoucher.Details == null) { MsgClass.Add(MESSAGE.DetailRecordsisNull); return false; }

            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (!MyVoucher.Master.Vou_No.ToLower().Equals("new"))
            {
                if (MyVoucher.Master.Vou_No.Length < 11) { MsgClass.Add(MESSAGE.VouNoNotDefineProperly); }
            }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.Company == 0) { MsgClass.Add(MESSAGE.Row_CompanyIDZero); }
           
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }

            if (MyVoucher.Detail.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
            if (MyVoucher.Detail.Inventory == 0) { MsgClass.Add(MESSAGE.Row_InventoryIDZero); }

            if (MyVoucher.Detail.Unit == 0) { MsgClass.Add(MESSAGE.Row_UnitIDZero); }
            if (MyVoucher.Detail.Qty == 0) { MsgClass.Add(MESSAGE.Row_QtyZero); }
            if (MyVoucher.Detail.Rate == 0) { MsgClass.Add(MESSAGE.Row_RateZero); }
            if (MyVoucher.Detail.Gross == 0) { MsgClass.Add(MESSAGE.Row_GrossAmountZero); }
            if (MyVoucher.Detail.Description.Length == 0) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
            if (MsgClass.Count > 0) { IsValid = false; }

            if (MyVoucher.Detail.TaxRate == 0)
            {
                if (MyVoucher.Detail.TaxID == 0) { MsgClass.Add(MESSAGE.Row_TaxIDZero); }
            }
            return IsValid;
        }
        #endregion

        #region Edit and New Voucher
        public void Edit(long _ID2)
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

            var _SrNo = 1;
            if (MyVoucher.Details.Count > 0)
            {
                _SrNo = MyVoucher.Details.Max(x => x.Sr_No) + 1;
            }

            var _Detail = new Detail(); ;
            _Detail.ID2 = 0;
            _Detail.Sr_No = _SrNo;
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

        #region Display detail List
        public List<Detail> GetDisplayList(bool _Deleted)
        {
            if (_Deleted)
            {
                return Deleted;
            }
            return MyVoucher.Details;
        }
        #endregion

        #region Remove

        public void Remove(int _SrNo)
        {
            if (_SrNo > 0 && _SrNo <= MyVoucher.Details.Max(sr => sr.Sr_No))
            {
                var _Trans = MyVoucher.Details.FirstOrDefault(sr => sr.Sr_No == _SrNo);
                if (_Trans != null)
                {
                    // Save a deleted record to Deleted list
                    _Trans.Sr_No = _Trans.Sr_No * -1;
                    Deleted.Add(_Trans);                     // Marked record as deleted.

                    MyVoucher.Details.Remove(_Trans);

                    if (MyVoucher.Details.Count > 0)
                    {
                        var _NewSrNo = 1;
                        foreach (var trans in MyVoucher.Details)
                        {
                            trans.Sr_No = _NewSrNo; _SrNo++;
                        }
                    }
                }
            }
        }

     
        #endregion


        #region Save & Save All


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

        public async Task<bool> SaveAllAsync()
        {
            if (IsWaiting) { return false; }


            IsWaiting = true;
            MsgClass = new();
            bool isSaved = true;

            try
            {
                // Generate Voucher No if NEW
                if (MyVoucher.Master.Vou_No?.ToUpper() == "NEW")
                {
                    MyVoucher.Master.Vou_No = NewVoucherNo.GetNewVoucherNo(Source.DBFile, Tables.BillPayable, "BP");
                }

                // =============================
                // 1️⃣ CREATE MASTER ROW
                // =============================
                DataRow masterRow = Source.GetNewRow(Tables.BillPayable);

                masterRow["ID"] = MyVoucher.Master.ID1;
                masterRow["Code"] = MyVoucher.Master.Code;
                masterRow["Vou_No"] = MyVoucher.Master.Vou_No;
                masterRow["Vou_Date"] = MyVoucher.Master.Vou_Date;
                masterRow["Company"] = MyVoucher.Master.Company;
                masterRow["Employee"] = MyVoucher.Master.Employee;
                masterRow["Inv_No"] = MyVoucher.Master.Inv_No;
                masterRow["Inv_Date"] = MyVoucher.Master.Inv_Date;
                masterRow["Pay_Date"] = MyVoucher.Master.Pay_Date;
                masterRow["Amount"] = CalculateNetAmount();
                masterRow["Description"] = MyVoucher.Master.Remarks;
                masterRow["Comments"] = MyVoucher.Master.Comments;
                masterRow["Status"] = Submitted.ToString();

                if (!Validate_Master(masterRow))
                {
                    MsgClass.Add(MESSAGE.RecordNotValidated);
                    return false;
                }

                // =============================
                // BEGIN TRANSACTION
                // =============================
                Source.BeginTransaction();

                // =============================
                // 2️⃣ SAVE MASTER
                // =============================
                CommandClass masterCommand = new(masterRow, Source.MyConnection, Source.DBtransaction!);

                if (!masterCommand.SaveChanges())
                {
                    MsgClass.Add(MESSAGE.RowNotUpdated);
                    isSaved = false;
                }

                if (isSaved && MyVoucher.Master.ID1 == 0)
                {
                    MyVoucher.Master.ID1 = masterCommand.PrimaryKeyID;
                }

                long purchaseInvoiceID = MyVoucher.Master.ID1;

                // =============================
                // 3️⃣ DELETE REMOVED DETAILS
                // =============================
                if (isSaved && Deleted != null && Deleted.Count > 0)
                {
                    foreach (var item in Deleted)
                    {
                        DataRow deleteRow = Source.GetNewRow(Tables.BillPayable2);
                        deleteRow["ID"] = item.ID2;

                        CommandClass deleteCommand = new(deleteRow, Source.MyConnection, Source.DBtransaction!);

                        if (!deleteCommand.DeleteRow())
                        {
                            MsgClass.Add(MESSAGE.RowNotDeleted);
                            isSaved = false;
                            break;
                        }
                    }

                    // Reset SR_NO only if deletion successful
                    if (isSaved)
                    {
                        int sr = 1;
                        foreach (var item in MyVoucher.Details)
                        {
                            item.Sr_No = sr++;
                        }
                    }
                }

                // =============================
                // 4️⃣ SAVE DETAILS
                // =============================
                if (isSaved)
                {
                    foreach (var item in MyVoucher.Details)
                    {
                        DataRow detailRow = Source.GetNewRow(Tables.BillPayable2);

                        detailRow["ID"] = item.ID2;
                        detailRow["TranID"] = purchaseInvoiceID;
                        detailRow["SR_NO"] = item.Sr_No;
                        detailRow["Inventory"] = item.Inventory;
                        detailRow["Batch"] = item.Batch;
                        detailRow["Unit"] = item.Unit;
                        detailRow["Qty"] = item.Qty;
                        detailRow["Rate"] = item.Rate;
                        detailRow["Tax"] = item.TaxID;
                        detailRow["Tax_Rate"] = item.TaxRate;
                        detailRow["Description"] = item.Description;
                        detailRow["Project"] = item.Project;

                        if (!Validate_Detail(detailRow))
                        {
                            MsgClass.Add(MESSAGE.RecordNotValidated);
                            isSaved = false;
                            break;
                        }

                        CommandClass detailCommand = new(detailRow, Source.MyConnection, Source.DBtransaction!);

                        if (!detailCommand.SaveChanges())
                        {
                            MsgClass.Add(MESSAGE.RowNotUpdated);
                            isSaved = false;
                            break;
                        }
                    }
                }

                // =============================
                // 5️⃣ COMMIT / ROLLBACK
                // =============================
                if (isSaved)
                    Source.CommitTransaction();
                else
                    Source.RollbackTransaction();

                // =============================
                // 6️⃣ REFRESH DATA
                // =============================
                if (isSaved)
                {
                    CalculateTotal();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Source.RollbackTransaction();
                MsgClass.Critical(ex.Message);
                isSaved = false;
            }
            finally
            {
                IsWaiting = false;
            }

            return isSaved;
        }

        #endregion

        #region Validation
        private bool Validate_Detail(DataRow rowDetail)
        {
            return true;
        }

        private bool Validate_Master(DataRow newRow1)
        {
            return true;
        }
        #endregion

        #region Calculations

        private decimal CalculateNetAmount()
        {
            decimal _NetAmount = 0.00M;
            if (MyVoucher.Details.Count > 0)
            {
                foreach (var item in MyVoucher.Details)
                {
                    _NetAmount += item.NetAmount;
                }
            }
            return _NetAmount;
        }


        #endregion

        #region Print
        public async void Print(ReportType _rptType)
        {
            await Task.Run(() =>
            {
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = _rptType;
                ReportService.Data = GetReportData();
                ReportService.Model = CreateReportModel();

            });

            try
            {
                ReportService.Print();
            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }
        }
        public ReportData GetReportData()
        {
            var _Query = Quries.PurchaseInvoice(PurchaseInvoiceID);
            var _Table = Source.GetTable(_Query);
            var _ReportData = new ReportData()
            {
                ReportTable = _Table,
                DataSetName = "ds_PurchaseInvoice"
            };
            return _ReportData;
        }
        public ReportModel CreateReportModel()
        {
            var _Heading1 = "Purchase Invoice";
            var _Heading2 = $"{_Heading1} [{MyVoucher.Master.Vou_No}]";
            var _ReportPath = AppGlobal.AppPaths.ReportPath;
            var _CompanyName = AppGlobal.Client.Company;
            var _ReportFooter = AppFunctions.ReportFooter();

            ReportModel rptModel = new();

            rptModel.InputReport.FileName = $"PurchaseInvoice";

            rptModel.OutputReport.FileName = $"PurchaseInvoice_{PurchaseInvoiceID}";

            rptModel.AddReportParameter("CompanyName", _CompanyName);
            rptModel.AddReportParameter("Heading1", _Heading1);
            rptModel.AddReportParameter("Heading2", _Heading2);
            rptModel.AddReportParameter("Footer", _ReportFooter);

            return rptModel;
        }

        #endregion

        #region Test Voucher Data
        public void TestData()
        {
            MyVoucher.Master.ID1 = 0;
            MyVoucher.Master.Vou_No = "New";
            MyVoucher.Master.Inv_No = "SRB-001";
            MyVoucher.Master.Vou_Date = new DateTime(2024, 10, 23);
            MyVoucher.Master.Company = 2;
            MyVoucher.Master.Employee = 2;
            MyVoucher.Master.Vou_Date = new DateTime(2024, 10, 23);
            MyVoucher.Master.Pay_Date = new DateTime(2024, 10, 23);
            MyVoucher.Master.Ref_No = "Test";
            MyVoucher.Master.Remarks = "D-Type Rubber Fender";
            MyVoucher.Master.Comments = "Damaged Ship Side Rubber Fender Length 200 Meters, Renewed with MS Channel & Stainless Steel (Marine Grade 316L) & Nuts, Bolts, Washers, conform to marine standard complete with fitting accessories.";

            MyVoucher.Detail.Sr_No = 1;
            MyVoucher.Detail.TranID = 0;
            MyVoucher.Detail.ID2 = 0;
            MyVoucher.Detail.Inventory = 4;
            MyVoucher.Detail.Project = 1;
            MyVoucher.Detail.Unit = 11;
            MyVoucher.Detail.Qty = 1;
            MyVoucher.Detail.Rate = 3750000;
            MyVoucher.Detail.TaxID = 0;
            MyVoucher.Detail.TaxRate = 15;
            MyVoucher.Detail.Batch = "#01";

            MyVoucher.Detail.Description = "D-Type Rubber Fender";

            MyVoucher.Details.Add(MyVoucher.Detail);
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
            public long ID1 { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Vou_No { get; set; } = string.Empty;
            public DateTime Vou_Date { get; set; }
            public long Company { get; set; }
            public long Employee { get; set; }
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
            public long ID2 { get; set; }
            public long TranID { get; set; }
            public int Sr_No { get; set; }
            public long Inventory { get; set; }
            public string Batch { get; set; } = string.Empty;
            public decimal Qty { get; set; } = 0.00M;
            public decimal Rate { get; set; } = 0.00M;
            public long TaxID { get; set; }
            public decimal TaxRate { get; set; } = 0.00M;
            public string Description { get; set; } = string.Empty;
            public long Project { get; set; }

            // Non DataBase Variables

            public decimal Gross => decimal.Parse((Qty * Rate).ToString());
            public decimal TaxAmount => decimal.Parse((Gross * (TaxRate / 100)).ToString());
            public decimal NetAmount => decimal.Parse((Gross + TaxAmount).ToString());

            public string TitleInventory { get; set; } = string.Empty;
            public string TitleProject { get; set; } = string.Empty;
            public string TitleTaxID { get; set; } = string.Empty;
            public long Unit { get; set; }                                       // Unit ID get from inventory
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
