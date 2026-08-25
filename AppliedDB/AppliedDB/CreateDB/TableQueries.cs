
namespace AppliedDB.CreateDB
{
    public static class TableQueries
    {
        public static string GetTableQuery(string tableName)
        {
            switch (tableName)
            {
                case "BankBook": return CreateBankBook();
                case "Employees": return CreateEmployees();
                case "BillPayable": return CreateBillPayable();
                case "BillPayable2": return CreateBillPayable2();
                case "BillReceivable": return CreateBillReceivable();
                case "BillReceivable2": return CreateBillReceivable2();
                case "BOMProfile": return CreateBOMProfile();
                case "BOMProfile2": return CreateBOMProfile2();
                case "Book": return CreateBook();
                case "Book2": return CreateBook2();
                case "CashBook": return CreateCashBook();
                case "ChequeStatus": return CreateChequeStatus();
                case "ChequeTranType": return CreateChequeTranType();
                case "City": return CreateCity();
                case "COA": return CreateCOA();
                case "COA_CLASS": return CreateCOA_CLASS();
                case "COA_Map": return CreateCOA_Map();
                case "COA_NATURE": return CreateCOA_NATURE();
                case "COA_NOTES": return CreateCOA_NOTES();
                case "Country": return CreateCountry();
                case "Customers": return CreateCustomers();
                case "Directories": return CreateDirectories();
                case "FinishedGoods": return CreateFinishedGoods();
                case "IdGenerator": return CreateIdGenerator();
                case "Inv_Category": return CreateInv_Category();
                case "Inventory": return CreateInventory();
                case "Inv_Packing": return CreateInv_Packing();
                case "Inv_Size": return CreateInv_Size();
                case "Inv_SubCategory": return CreateInv_SubCategory();
                case "Inv_UOM": return CreateInv_UOM();
                case "Inv_Price": return CreateInv_Price();
                case "Ledger": return CreateLedger();
                case "OBALCompany": return CreateOBALCompany();
                case "OBALStock": return CreateOBALStock();
                case "Production2": return CreateProduction2();
                case "Profile": return CreateProfile();
                case "Project": return CreateProject();
                case "Receipt": return CreateReceipt();
                case "Receipt2": return CreateReceipt2();
                case "Receipts": return CreateReceipts();
                case "Registry": return CreateRegistry();
                case "Role": return CreateRole();
                case "SaleReturn": return CreateSaleReturn();
                case "StockInHand": return CreateStockInHand();
                case "Taxes": return CreateTaxes();
                case "WriteCheques": return CreateWriteCheques();


                // Data Views
                case "view_Book": return CreateViewBook();
                case "view_BillPayable": return CreateView_BillPayable();
                case "view_BillReceivable": return CreateView_BillReceivable();
                case "view_Ledger": return CreateView_Ledger();
                case "view_Purchased":return CreateView_Purchased();
                case "view_Sold": return CreateView_Sold();
                case "view_Receipts": return CreateView_Receipts();
                
                default: return string.Empty;
            }
        }
        public static string CreateBankBook()
        {
            return @"
                    CREATE TABLE [BankBook](
                    [ID] INT NOT NULL UNIQUE,
                    [Vou_Date] DATETIME NOT NULL, 
                    [Vou_No] TEXT(10) NOT NULL,
                    [BookID] INT NOT NULL, 
                    [COA] INT NOT NULL, 
                    [Ref_No] NVARCHAR(10),  
                    [Sheet_No] NVARCHAR(12), 
                    [DR] DECIMAL NOT NULL, 
                    [CR] DECIMAL NOT NULL,
                    [Customer] INT,
                    [Employee] INT, 
                    [Project] INT, 
                    [Description] NVARCHAR(60) NOT NULL,
                    [Comments] NVARCHAR(500), 
                    [Status] NVARCHAR(10) NOT NULL DEFAULT Submitted);";
        }
        public static string CreateEmployees()
        {
            return @"
                    CREATE TABLE [Employees](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(100) NOT NULL UNIQUE,
                        [Designation] NVARCHAR,
                        [Full_Name] NVARCHAR(100),
                        [Contact] NVARCHAR(60),
                        [Address] NVARCHAR(200),
                        [City] NVARCHAR(60),
                        [Join] DATE,
                        [left] DATE,
                        [DOB] DATE,
                        [CNIC] NVARCHAR(14)
                    );";
        }
        public static string CreateBillPayable()
        {
            return @"
                    CREATE TABLE [BillPayable](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL,
                        [Vou_No] NVARCHAR(12) NOT NULL,
                        [Vou_Date] DATETIME NOT NULL,
                        [Company] INT NOT NULL,
                        [Employee] INT REFERENCES [Employees]([ID]),
                        [Ref_No],
                        [Inv_No] NVARCHAR(20) NOT NULL,
                        [Inv_Date] DATETIME,
                        [Pay_Date] DATETIME NOT NULL,
                        [Amount] DECIMAL NOT NULL DEFAULT (0.00),
                        [Description] NVARCHAR(100) NOT NULL,
                        [Comments] NVARCHAR(500),
                        [Status] NVARCHAR(12) NOT NULL DEFAULT 'Submitted'
                    );";
        }

        #region Inventory and allied
        public static string CreateInventory()
        {
            return @"
                    CREATE TABLE [Inventory](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(10) NOT NULL UNIQUE,
                        [Title] NVARCHAR(60) NOT NULL UNIQUE,
                        [Qty_Packing] INT64,
                        [Packing] INT64 REFERENCES [Inv_Packing]([ID]),
                        [UOM] INT64 REFERENCES [Inv_UOM]([ID]),
                        [SubCategory] INT64 REFERENCES [Inv_SubCategory]([ID]),
                        [Notes] NVARCHAR(500)
                    );";
        }
        public static string CreateInv_Packing()
        {
            return @"
                    CREATE TABLE [Inv_Packing](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(30) NOT NULL UNIQUE,
                        [Qty] INT
                    );";
        }
        public static string CreateInv_SubCategory()
        {
            return @"
                    CREATE TABLE [Inv_SubCategory](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(30) NOT NULL UNIQUE,
                        [Category] INT
                    );";
        }
        public static string CreateInv_UOM()
        {
            return @"
                    CREATE TABLE [Inv_UOM](
                        [ID] INT PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(15) NOT NULL UNIQUE
                    );";
        }
        public static string CreateInv_Category()
        {
            return @"
                    CREATE TABLE [Inv_Category](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(30) NOT NULL UNIQUE
                    );";
        }
        public static string CreateInv_Size()
        {
            return @"
                    CREATE TABLE [Inv_Size](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(30) NOT NULL UNIQUE
                    );";
        }
        public static string CreateInv_Price()
        {
            return @"
                    CREATE TABLE[Inv_Price](
                      [ID] INT64 NOT NULL UNIQUE,
                      [StockID] INT64 NOT NULL, 
                      [PriceDate] DATETIME NOT NULL, 
                      [MRP] DECIMAL NOT NULL, 
                      [TPRate] DECIMAL NOT NULL, 
                      [Discount] DECIMAL, 
                      [Bonus] INT);";

        }
        #endregion


        public static string CreateProject()
        {
            return @"
                    CREATE TABLE [Project](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Code] NVARCHAR(6) NOT NULL UNIQUE,
                        [Title] NVARCHAR(100) NOT NULL UNIQUE,
                        [Comments] NVARCHAR(500),
                        Client INT64 NOT NULL DEFAULT 0,
                        ActualCost DECIMAL NOT NULL DEFAULT 0.00,
                        Budget DECIMAL NOT NULL DEFAULT 0.00,
                        Location NVARCHAR,
                        StartDate DATETIME,
                        EndDate DATETIME,
                        IsActive BOOLEAN NOT NULL DEFAULT True,
                        IsCompleted BOOLEAN NOT NULL DEFAULT False,
                        ProjectManager INT64 NOT NULL DEFAULT 0,
                        Terms NVARCHAR
                    );";
        }
        public static string CreateCOA_CLASS()
        {
            return @"
                    CREATE TABLE [COA_CLASS](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE DEFAULT 0,
                        [CODE] TEXT(2) DEFAULT '000',
                        [TITLE] VARCHAR(100) NOT NULL UNIQUE
                    );";
        }
        public static string CreateCOA_NATURE()
        {
            return @"
                    CREATE TABLE [COA_NATURE](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [CODE] TEXT(3),
                        [TITLE] VARCHAR(100)
                    );";
        }
        public static string CreateCOA_NOTES()
        {
            return @"
                    CREATE TABLE [COA_NOTES](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [CODE] TEXT(3),
                        [TITLE] VARCHAR(100) NOT NULL UNIQUE
                    );";
        }
        public static string CreateCOA()
        {
            return @"
                    CREATE TABLE [COA](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [CODE] TEXT(6) DEFAULT '000000',
                        [TITLE] VARCHAR(100) NOT NULL UNIQUE,
                        [CLASS] INT64 REFERENCES [COA_CLASS]([ID]),
                        [NATURE] INT64 REFERENCES [COA_NATURE]([ID]),
                        [NOTES] INT64 REFERENCES [COA_NOTES]([ID]),
                        [OPENING_BALANCE] DECIMAL DEFAULT (0.00)
                    );";
        }
        public static string CreateTaxes()
        {
            return @"
                    CREATE TABLE [Taxes](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL UNIQUE,
                        [Title] NVARCHAR(100) NOT NULL UNIQUE,
                        [Rate] DECIMAL NOT NULL,
                        [TaxType] INT,
                        [COA] INT64 REFERENCES [COA]([ID])
                    );";
        }
        public static string CreateBillPayable2()
        {
            return @"
                    CREATE TABLE [BillPayable2](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Sr_No] INT NOT NULL,
                        [TranID] INT64 NOT NULL,
                        [Inventory] INT64 NOT NULL REFERENCES [Inventory]([ID]),
                        [Batch] NVARCHAR(20) NOT NULL,
                        [Qty] DECIMAL NOT NULL,
                        [Rate] DECIMAL NOT NULL,
                        [Tax] INT64 REFERENCES [Taxes]([ID]),
                        [Tax_Rate] DECIMAL,
                        [Description] NVARCHAR(100),
                        [Project] INT64 REFERENCES [Project]([ID]),
                        [Unit] INT64
                    );";
        }
        public static string CreateBillReceivable()
        {
            return @"
                    CREATE TABLE [BillReceivable](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Vou_No] NVARCHAR(12) NOT NULL,
                        [Vou_Date] DATETIME NOT NULL,
                        [Company] INT64 NOT NULL,
                        [Employee] INT64 REFERENCES [Employees]([ID]),
                        [Ref_No] NVARCHAR(20),
                        [Inv_No] NVARCHAR(20) NOT NULL,
                        [Inv_Date] DATETIME,
                        [Pay_Date] DATETIME NOT NULL,
                        [Amount] DECIMAL NOT NULL DEFAULT (0.00),
                        [Description] NVARCHAR(100) NOT NULL,
                        [Comments] NVARCHAR(500),
                        [Status] NVARCHAR(12) NOT NULL DEFAULT 'Submitted'
                    );";
        }
        public static string CreateBillReceivable2()
        {
            return @"
                    CREATE TABLE [BillReceivable2](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Sr_No] INT NOT NULL,
                        [TranID] INT64 NOT NULL REFERENCES [BillReceivable]([ID]),
                        [Inventory] INT64 NOT NULL REFERENCES [Inventory]([ID]),
                        [Batch] NVARCHAR(20) NOT NULL,
                        [Qty] DECIMAL NOT NULL,
                        [Rate] DECIMAL NOT NULL,
                        [Tax] INT64 REFERENCES [Taxes]([ID]),
                        [Tax_Rate] DECIMAL,
                        [Description] NVARCHAR(100),
                        [Project] INT64 REFERENCES [Project]([ID]),
                        [Unit] INT64
                    );";
        }
        public static string CreateBOMProfile()
        {
            return @"
                    CREATE TABLE [BOMProfile](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Code] NVARCHAR(30) NOT NULL UNIQUE,
                        [Title] NVARCHAR(100) NOT NULL UNIQUE,
                        [Status] NVARCHAR(15) NOT NULL
                    );";
        }
        public static string CreateBOMProfile2()
        {
            return @"
                    CREATE TABLE [BOMProfile2](
                        [ID] INT64 NOT NULL UNIQUE,
                        [TranID] INT64 NOT NULL,
                        [IN_OUT] NVARCHAR(3) NOT NULL,
                        [Inventory] INT64 NOT NULL,
                        [UOM] INT64 NOT NULL,
                        [Qty] DECIMAL NOT NULL,
                        [Rate] DECIMAL NOT NULL,
                        [Westage] DECIMAL DEFAULT 0
                    );";
        }
        public static string CreateBook()
        {
            return @"
                    CREATE TABLE [Book](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE DEFAULT 0,
                        [BookID] INT64 NOT NULL DEFAULT 0 REFERENCES [COA]([ID]),
                        [Vou_No] NVARCHAR(11) NOT NULL UNIQUE,
                        [Vou_Date] DATETIME NOT NULL,
                        [Amount] DECIMAL NOT NULL DEFAULT (0.00),
                        [Ref_No] NVARCHAR(20),
                        [SheetNo] NVARCHAR(20),
                        [Remarks] NVARCHAR NOT NULL,
                        [Status] NVARCHAR(10) NOT NULL DEFAULT 'Submitted'
                    );";
        }
        public static string CreateBook2()
        {
            return @"
                    CREATE TABLE [Book2](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [TranID] INT64 NOT NULL REFERENCES [Book]([ID]),
                        [SR_NO] INT NOT NULL DEFAULT 0,
                        [COA] INT64 NOT NULL REFERENCES [COA]([ID]),
                        [Company] INT64,
                        [Employee] INT64,
                        [Project] INT64,
                        [DR] DECIMAL NOT NULL DEFAULT (0.00),
                        [CR] DECIMAL NOT NULL DEFAULT (0.00),
                        [Description] NVARCHAR NOT NULL,
                        [Comments] NVARCHAR
                    );";
        }
        public static string CreateCashBook()
        {
            return @"
                    CREATE TABLE [CashBook](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Vou_Date] DATETIME NOT NULL,
                        [Vou_No] TEXT(10) NOT NULL,
                        [BookID] INT64 NOT NULL,
                        [COA] INT64 NOT NULL,
                        [Ref_No] NVARCHAR(10),
                        [Sheet_No] NVARCHAR(12),
                        [DR] DECIMAL NOT NULL,
                        [CR] DECIMAL NOT NULL,
                        [Customer] INT64,
                        [Employee] INT64,
                        [Project] INT64,
                        [Description] NVARCHAR(60) NOT NULL,
                        [Comments] NVARCHAR(500),
                        [Status] NVARCHAR(10) NOT NULL DEFAULT 'Submitted'
                    );";
        }
        public static string CreateChequeStatus()
        {
            return @"
                    CREATE TABLE [ChequeStatus](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL UNIQUE,
                        [Title] NVARCHAR(60) NOT NULL UNIQUE
                    );";
        }
        public static string CreateChequeTranType()
        {
            return @"
                    CREATE TABLE [ChequeTranType](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL UNIQUE,
                        [Title] NVARCHAR(60) NOT NULL UNIQUE
                    );";
        }
        public static string CreateCity()
        {
            return @"
                    CREATE TABLE [City](
                        [ID] INT64 NOT NULL UNIQUE,
                        [City] NVARCHAR(30) NOT NULL UNIQUE,
                        [Country] NVARCHAR(30)
                    );";
        }
        public static string CreateCOA_Map()
        {
            return @"
                    CREATE TABLE [COA_Map](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [COA] INT64 NOT NULL UNIQUE REFERENCES [COA]([ID]),
                        [Stock] INT64 NOT NULL REFERENCES [Inventory]([ID])
                    );";
        }
        public static string CreateCountry()
        {
            return @"
                    CREATE TABLE [Country](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Country] NVARCHAR NOT NULL UNIQUE,
                        [DialCode] INT,
                        [CountryCode] TEXT(2) NOT NULL UNIQUE
                    );";
        }
        public static string CreateCustomers()
        {
            return @"
                    CREATE TABLE [Customers](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(8) NOT NULL UNIQUE,
                        [Title] NVARCHAR(100) NOT NULL UNIQUE,
                        [Address1] NVARCHAR(60),
                        [Address2] NVARCHAR(60),
                        [City] NVARCHAR(30),
                        [State] NVARCHAR(30),
                        [Country] NVARCHAR(30),
                        [Phone] NVARCHAR(30),
                        [Mobile] NVARCHAR(30),
                        [Email] NVARCHAR(100),
                        [NTN] NVARCHAR(9),
                        [CNIC] NVARCHAR(15),
                        [Notes] NVARCHAR(500),
                        [Status] INT64,
                        [Address3] NVARCHAR(60)
                    );";
        }
        public static string CreateDirectories()
        {
            return @"
                    CREATE TABLE [Directories](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Directory] NVARCHAR NOT NULL,
                        [Key] INT NOT NULL,
                        [Value] NVARCHAR NOT NULL
                    );";
        }
        public static string CreateFinishedGoods()
        {
            return @"
                    CREATE TABLE [FinishedGoods](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Batch] NVARCHAR(30) NOT NULL UNIQUE,
                        [MFDate] DATETIME NOT NULL,
                        [EXPDate] DATETIME NOT NULL,
                        [Process] INT NOT NULL,
                        [Product] INT NOT NULL,
                        [Qty] DECIMAL(12, 4) NOT NULL,
                        [Rate] DECIMAL(12, 4) NOT NULL,
                        [Amount] DECIMAL(12, 4),
                        [Remarks] NVARCHAR(100),
                        [Project] INT64,
                        [Employee] INT64,
                        [Status] NVARCHAR(12)
                    );";
        }
        public static string CreateIdGenerator()
        {
            return @"
                    CREATE TABLE IdGenerator (
                        TableName TEXT PRIMARY KEY,
                        LastId INTEGER NOT NULL
                    );";
        }

        public static string CreateLedger()
        {
            return @"
                    CREATE TABLE [Ledger](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [TranID] INT,
                        [Vou_Type] TEXT(10) NOT NULL,
                        [Vou_Date] DATETIME NOT NULL,
                        [Vou_No] TEXT(12) NOT NULL,
                        [SR_NO] INT NOT NULL,
                        [Ref_No] NVARCHAR(12),
                        [BookID] INT64,
                        [COA] INT64 NOT NULL,
                        [DR] DECIMAL NOT NULL,
                        [CR] DECIMAL NOT NULL,
                        [CUSTOMER] INT64,
                        [EMPLOYEE] INT64,
                        [INVENTORY] INT64,
                        [PROJECT] INT64,
                        [Description] NVARCHAR(60) NOT NULL,
                        [Comments] NVARCHAR(500),
                        [Status] NVARCHAR(10)
                    );";
        }
        public static string CreateOBALCompany()
        {
            return @"
                    CREATE TABLE [OBALCompany](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Company] INT64 NOT NULL DEFAULT 0 REFERENCES [Customers]([ID]),
                        [COA] INT64 NOT NULL DEFAULT 0 REFERENCES [COA]([ID]),
                        [Amount] DECIMAL NOT NULL DEFAULT 0,
                        [Project] INT64,
                        [Employee] INT64
                    );";
        }
        public static string CreateOBALStock()
        {
            return @"
                    CREATE TABLE [OBALStock](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Inventory] INT64 NOT NULL REFERENCES [Inventory]([ID]),
                        [Batch] NVARCHAR NOT NULL,
                        [Project] INT64 NOT NULL REFERENCES [Project]([ID]),
                        [QTY] DECIMAL NOT NULL,
                        [Rate] DECIMAL NOT NULL,
                        [Amount] DECIMAL
                    );";
        }
        public static string CreateProduction2()
        {
            return @"
                    CREATE TABLE [Production2](
                        [ID] INT PRIMARY KEY NOT NULL UNIQUE,
                        [TranID] INT NOT NULL REFERENCES [Production]([ID]),
                        [Stock] INT NOT NULL REFERENCES [Inventory]([ID]),
                        [Flow] TEXT(3) NOT NULL,
                        [Qty] DECIMAL NOT NULL,
                        [UOM] DECIMAL NOT NULL,
                        [Rate] DECIMAL NOT NULL,
                        [Remarks] NVARCHAR(100)
                    );";
        }
        public static string CreateProfile()
        {
            return @"
                    CREATE TABLE [Profile](
                        [ID] INTEGER PRIMARY KEY NOT NULL UNIQUE,
                        [Tag] NVARCHAR(100) NOT NULL UNIQUE,
                        [Title] NVARCHAR NOT NULL UNIQUE,
                        [Description] NVARCHAR NOT NULL,
                        [LastLogin] DATETEXT,
                        [Session] NVARCHAR,
                        [CurrencyFormat] VARCHAR(25),
                        [DateFormat] VARCHAR(25),
                        [FiscalFrom] DATETIME,
                        [FiscalTo] DATETIME
                    );";
        }
        public static string CreateReceipt()
        {
            return @"
                    CREATE TABLE [Receipt](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Vou_No] NVARCHAR(11) NOT NULL UNIQUE,
                        [Vou_Date] DATETIME NOT NULL,
                        [COA] INT64 NOT NULL,
                        [Ref_No] NVARCHAR(12),
                        [Payer] INT64 NOT NULL,
                        [Doc_No] NVARCHAR(20),
                        [Doc_Date] DATETIME,
                        [Pay_Mode] NVARCHAR(10),
                        [Amount] DECIMAL,
                        [Remarks] NVARCHAR(100),
                        [Comments] NVARCHAR,
                        [Status] NVARCHAR(10)
                    );";
        }
        public static string CreateReceipt2()
        {
            return @"
                    CREATE TABLE [Receipt2](
                        [ID] INT64 NOT NULL UNIQUE,
                        [Sr_No] INT NOT NULL,
                        [TranID] INT64 NOT NULL,
                        [Ref_No] NVARCHAR(20),
                        [Inv_No] INT,
                        [Account] INT NOT NULL,
                        [DR] DECIMAL,
                        [CR] DECIMAL,
                        [Employee] INT64,
                        [Project] INT64,
                        [Description] NVARCHAR NOT NULL
                    );";
        }
        public static string CreateReceipts()
        {
            return @"
                    CREATE TABLE [Receipts](
                        [ID] INT64 PRIMARY KEY,
                        [Vou_No] TEXT(10),
                        [Vou_Date] DATE NOT NULL,
                        [Ref_No] NVARCHAR(12),
                        [COA] INT64,
                        [COACash] INT64,
                        [Payer] INT64 NOT NULL,
                        [Project] INT64 NOT NULL,
                        [Employee] INT64,
                        [Amount] DECIMAL NOT NULL,
                        [Description] NVARCHAR NOT NULL,
                        [Status] NVARCHAR(10) NOT NULL
                    );";
        }
        public static string CreateRegistry()
        {
            return @"
                    CREATE TABLE [Registry](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL UNIQUE,
                        [Title] NVARCHAR(60),
                        [nValue] INT,
                        [mValue] DECIMAL,
                        [dValue] DATETIME,
                        [cValue] NVARCHAR,
                        [bValue] BOOLEAN,
                        [UserName] NVARCHAR(25),
                        [From] DATETIME,
                        [To] DATETIME
                    );";
        }
        public static string CreateRole()
        {
            return @"
                    CREATE TABLE [Role](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Tag] NVARCHAR(100) NOT NULL UNIQUE,
                        [Title] NVARCHAR NOT NULL UNIQUE,
                        [Description] NVARCHAR NOT NULL
                    );";
        }
        public static string CreateSaleReturn()
        {
            return @"
                    CREATE TABLE [SaleReturn](
                        [ID] INT64 PRIMARY KEY NOT NULL UNIQUE,
                        [Vou_No] TEXT(12) NOT NULL UNIQUE,
                        [Vou_Date] DATETIME NOT NULL,
                        [TranID] INT NOT NULL UNIQUE REFERENCES [BillReceivable2]([ID]),
                        [QTY] DECIMAL NOT NULL DEFAULT 0,
                        [Status] TEXT(12) NOT NULL DEFAULT 'Submitted'
                    );";
        }
        public static string CreateStockInHand()
        {
            return @"
                    CREATE TABLE [StockInHand](
                        [StockID] INT64,
                        [GTitle] NVARCHAR(100),
                        [Vou_No] NVARCHAR(15),
                        [Vou_Date] DATETIME,
                        [Title] NVARCHAR(100),
                        [PRQty] DECIMAL,
                        [PRAmount] DECIMAL,
                        [SLQty] DECIMAL,
                        [SLAmount] DECIMAL,
                        [PDQty] DECIMAL,
                        [PQAmount] DECIMAL,
                        [NetQty] DECIMAL,
                        [NetAmount] DECIMAL,
                        [AvgRate] DECIMAL,
                        [SoldCost] DECIMAL
                    );";
        }
        public static string CreateWriteCheques()
        {
            return @"
                    CREATE TABLE [WriteCheques](
                        [ID] INT64 PRIMARY KEY UNIQUE,
                        [Code] NVARCHAR(12) NOT NULL,
                        [TranType] INT64 NOT NULL REFERENCES [ChequeTranType]([ID]),
                        [TranDate] DATE NOT NULL,
                        [Bank] INT NOT NULL REFERENCES [COA]([ID]),
                        [ChqDate] DATE NOT NULL,
                        [ChqNo] NVARCHAR(20) NOT NULL,
                        [ChqAmount] DECIMAL NOT NULL,
                        [Company] INT64 NOT NULL,
                        [TaxID] INT64 REFERENCES [Taxes]([ID]),
                        [TaxableAmount] DECIMAL,
                        [TaxRate] DECIMAL,
                        [TaxAmount] DECIMAL,
                        [Description] NVARCHAR(200),
                        [Status] INT64 NOT NULL,
                        [Project] INT64 REFERENCES [Project]([ID]),
                        [Employee] INT64 REFERENCES [Employees]([ID])
                    );";
        }

        // Create Data View

        public static string CreateViewBook()
        {
            return @" CREATE VIEW [view_Book] AS 
                    SELECT
                    [B1].[ID] AS [ID1],
                    [B1].[BookID],
                    [B].[Title] As [TitleBook],
                    [B1].[Vou_No],
                    [B1].[Vou_Date],
                    [B1].[Amount],
                    [B1].[Ref_No],
                    [B1].[SheetNo],
                    [B1].[Remarks],
                    [B1].[Status],
                    [B2].[ID] AS [ID2],
                    [B2].[TranID],
                    [B2].[SR_NO],
                    [B2].[COA],
                    [A].[Title] As [TitleCOA],
                    [B2].[Company],
                    [C].[Title] As [TitleCompany],
                    [B2].[Employee],
                    [E].[Title] As [TitleEmployee],
                    [B2].[Project],
                    [P].[Title] As [TitleProject],
                    [B2].[DR],
                    [B2].[CR],
                    [B2].[Description],
                    [B2].[Comments]
                    FROM [Book2] [B2]
                    LEFT JOIN [Book]      [B1] ON [B1].[ID] = [B2].[TranID]
                    LEFT JOIN [Customers] [C]  ON  [C].[ID] = [B2].[Company]
                    LEFT JOIN [COA]       [A]  ON  [A].[ID] = [B2].[COA]
                    LEFT JOIN [COA]       [B]  ON  [B].[ID] = [B1].[BookID]
                    LEFT JOIN [Employees] [E]  ON  [E].[ID] = [B2].[Employee]
                    LEFT JOIN [Project]   [P]  ON  [P].[ID] = [B2].[Project];"
            ;
        }
        public static string CreateView_BillReceivable()
        {
            return @"
                    CREATE VIEW [view_BillReceivable]
                    AS
                    SELECT 
                           [BillReceivable].[ID] AS [ID], 
                           [BillReceivable].[ID] AS [ID1], 
                           [BillReceivable].[Vou_No], 
                           [BillReceivable].[Vou_Date], 
                           [BillReceivable].[Company], 
                           [BillReceivable].[Employee], 
                           [BillReceivable].[Ref_No], 
                           [BillReceivable].[Inv_No], 
                           [BillReceivable].[Inv_Date], 
                           [BillReceivable].[Pay_Date], 
                           [BillReceivable].[Amount], 
                           [BillReceivable].[Description], 
                           [BillReceivable].[Comments], 
                           [BillReceivable].[Status], 
                           [BillReceivable2].[ID] AS [ID2], 
                           [BillReceivable2].[Sr_No], 
                           [BillReceivable2].[TranID], 
                           [BillReceivable2].[Inventory], 
                           [BillReceivable2].[Batch], 
                           [BillReceivable2].[Qty], 
                           [BillReceivable2].[Rate], 
                           [BillReceivable2].[Tax], 
                           [BillReceivable2].[Tax_Rate], 
                           [BillReceivable2].[Description] AS [Description2], 
                           [BillReceivable2].[Project]
                    FROM   [BillReceivable]
                           LEFT JOIN [BillReceivable2] ON [BillReceivable].[ID] = [BillReceivable2].[TranID];";
        }
        public static string CreateView_BillPayable()
        {
            return @"
                    CREATE VIEW [view_BillPayable]
                    AS
                    SELECT 
                           [BillPayable].[ID] AS [ID], 
                           [BillPayable].[ID] AS [ID1], 
                           [BillPayable].[Code], 
                           [BillPayable].[Vou_No], 
                           [BillPayable].[Vou_Date], 
                           [BillPayable].[Company], 
                           [BillPayable].[Employee], 
                           [BillPayable].[Ref_No], 
                           [BillPayable].[Inv_No], 
                           [BillPayable].[Inv_Date], 
                           [BillPayable].[Pay_Date], 
                           [BillPayable].[Amount], 
                           [BillPayable].[Description], 
                           [BillPayable].[Comments], 
                           [BillPayable].[Status], 
                           [BillPayable2].[ID] AS [ID2], 
                           [BillPayable2].[Sr_No], 
                           [BillPayable2].[TranID], 
                           [BillPayable2].[Inventory], 
                           [BillPayable2].[Batch], 
                           [BillPayable2].[Qty], 
                           [BillPayable2].[Rate], 
                           [BillPayable2].[Tax], 
                           [BillPayable2].[Tax_Rate], 
                           [BillPayable2].[Description] AS [Description2], 
                           [BillPayable2].[Project]
                    FROM   [BillPayable]
                           LEFT JOIN [BillPayable2] ON [BillPayable].[ID] = [BillPayable2].[TranID];";
        }
        public static string CreateView_Receipts()
        {
            return @"
                    CREATE VIEW [view_Receipts] AS
                    SELECT 
                    [R1].[Vou_No],
                    [R1].[Vou_Date],
                    [R1].[COA],
                    [A1].[Title] AS[TitleCOA],
                    [R1].[Ref_No], 
                    [R1].[Payer], 
                    [R1].[ID] AS[ID1],
                     [C].[Title] AS[TitlePayer],
                    [R1].[Doc_No],
                    [R1].[Doc_Date],
                    [R1].[Pay_Mode],
                    [R1].[Amount],
                    [R1].[Remarks],
                    [R1].[Comments],
                    [R1].[Status],
                    [R2].[ID] AS[ID2],
                    [R2].[Sr_No],
                    [R2].[TranID],
                    [R2].[Ref_No],
                    [R2].[Account],
                    [A2].[Title] AS[TitleAccount],
                    [R2].[DR],
                    [R2].[CR],
                    [R2].[Employee],
                     [E].[Title] AS[TitleEmployee],
                    [R2].[Project],
                     [P].[Title] AS[TitleProject],
                    [R2].[Description]
                    FROM [Receipt2] [R2]
                    LEFT JOIN[Receipt]   [R1] ON [R1].[ID] = [R2].[TranID]
                    LEFT JOIN[COA]       [A1] ON [A1].[ID] = [R1].[COA]
                    LEFT JOIN[COA]       [A2] ON [A2].[ID] = [R2].[Account]
                    LEFT JOIN[Customers]  [C] ON  [C].[ID] = [R1].[Payer]
                    LEFT JOIN[Employees]  [E] ON  [E].[ID] = [R2].[Employee]
                    LEFT JOIN[Project]    [P] ON  [P].[ID] = [R2].[Project];";
        }
        public static string CreateView_Ledger()
        {
            return @"CREATE VIEW [view_Ledger]
                    AS
                    SELECT 
                           [ID], 
                           [Vou_Type], 
                           [Vou_Date], 
                           [Vou_No],
                           [SR_NO], 
                           [Description], 
                           [DR], 
                           [CR], 
                           0 AS [BAL],
                           "" [Status]
                    FROM   [Ledger]
                    WHERE  [ID] < 0;
                    ";
        }
        public static string CreateView_Purchased()
        {
            return @"
                        CREATE VIEW [view_Purchased] AS 
                        SELECT 
                        [B1].[Vou_No], 
                        [B1].[Vou_Date], 
                        [B2].[Inventory], 
                        [B2].[Qty], 
                        [B2].[Rate], 
                        [B2].[Qty] *[B2].[Rate] AS [Amount], 
                        [T].[Rate] AS [TaxRate], 
                        CAST(([B2].[Qty] *[B2].[Rate]) * [T].[Rate] AS Float) AS [TaxAmount] 
                        FROM [BillPayable] [B1] 
                        LEFT JOIN [BillPayable2] [B2] ON [B2].[TranID] = [B1].[ID] 
                        LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax];";
        }
        public static string CreateView_Sold()
        {
            return @"
                        CREATE VIEW [view_Purchased] AS 
                        SELECT 
                        [B1].[Vou_No], 
                        [B1].[Vou_Date], 
                        [B2].[Inventory], 
                        [B2].[Qty], 
                        [B2].[Rate], 
                        [B2].[Qty] *[B2].[Rate] AS [Amount], 
                        [T].[Rate] AS [TaxRate], 
                        CAST(([B2].[Qty] *[B2].[Rate]) * [T].[Rate] AS Float) AS [TaxAmount] 
                        FROM [BillPayable] [B1] 
                        LEFT JOIN [BillPayable2] [B2] ON [B2].[TranID] = [B1].[ID] 
                        LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax];";
        }
    }
}
