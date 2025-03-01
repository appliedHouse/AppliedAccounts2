using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedDB
{
    public class Enums
    {
        public enum Tables
        {
            Registry = 1,

            COA = 101,
            COA_Nature = 102,
            COA_Class = 103,
            COA_Notes = 104,
            CashBook = 105,
            WriteCheques = 106,
            Taxes = 107,
            ChequeTranType = 108,
            ChequeStatus = 109,
            TaxTypeTitle = 110,
            BillPayable = 111,
            BillPayable2 = 112,
            TB = 113,
            BillReceivable = 114,
            BillReceivable2 = 115,
            view_BillReceivable = 116,
            OBALCompany = 117,
            JVList = 118,
            SaleReturn = 120,
            BankBook = 121,
            RevSheet = 122,
            view_BillPayable = 123,
            Receipts = 124,
            Book = 125,
            Book2 = 126,
            view_Book = 127,

            Customers = 201,
            City = 202,
            Country = 203,
            Project = 204,
            Employees = 205,
            Directories = 206,

            Inventory = 301,
            Inv_Category = 302,
            Inv_SubCategory = 303,
            Inv_Packing = 304,
            Inv_UOM = 305,
            FinishedGoods = 306,
            OBALStock = 308,
            BOMProfile = 309,
            BOMProfile2 = 310,
            StockPositionData = 311,
            StockPosition = 312,
            StockPositionSUM = 313,
            view_Purchased = 314,
            view_Sold = 315,
            Production = 316,
            Production2 = 317,
            view_Production = 318,
            StockCategory = 319,
            StockInHand=320,

            Ledger = 401,
            view_Ledger = 402,
            CashBookTitles = 403,
            VouMax_JV = 404,
            VouMax = 405,

            PostCashBook = 501,
            PostBankBook = 502,
            PostWriteCheque = 503,
            PostBillReceivable = 504,
            PostBillPayable = 505,
            PostPayments = 506,
            PostReceipts = 507,
            UnpostCashBook = 508,
            UnpostBillPayable = 509,

            fun_BillPayableAmounts = 601,                    // Function of Bill Payable Amount and Tax Amount
            fun_BillPayableEntry = 602,

            Chk_BillReceivable1 = 701,
            Chk_BillReceivable2 = 702,

            TempLedger = 9999,
            
        }
        public enum SQLType
        {
            Insert,
            Update,
            Delete
        }
        public enum Query
        {
            SaleInvoice,
            SaleInvoiceList,
            SaleInvoiceView,

            PurchaseInvoice,
            PurchaseInvoiceList,
            PurchaseInvoiceView,


            COAList,
            Customers,
            CustomersList,
            COAClassList,
            COANatureList,
            COANotesList,

            Book,
            CashBook,
            BankBook,

            Doner,
            Donation,
            DonationType,
            PaymentMode,
            Currency,
            View_Production,
            View_Sold,
            View_Purchased,
            Chk_BillReceivable2,
            Chk_BillReceivable1,
            StockPosition,
            StockPositionData,
            StockPositionSUM,
            Book2
        }

    }
}
