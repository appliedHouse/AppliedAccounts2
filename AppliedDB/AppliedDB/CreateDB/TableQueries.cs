

using static AppliedDB.Enums;

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

    }
}
