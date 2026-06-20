namespace AppMessages
{
    public class Enums
    {
        public enum Messages
        {

            NoMessage,
            Default = 1,
            Save = 2,
            NotSave = 3,
            Delete = 4,
            NotDelete = 5,
            Insert = 6,
            NotInsert = 7,

            RowInserted = 101,
            RowNotInserted = 102,
            RowDeleted = 103,
            RowNotDeleted = 103,
            RecordCanNotDelete = 105,

            // Reports
            prtReportError = 201,
            rptRDLCNotExist = 202,
            rptOutputFileIsZero = 203,
            rptDataTableIsNull = 204,
            rptDataRowIsZero = 205,
            rptParametersNotValid = 206,
            rptNotValidToPrint = 207,
            rptDataSetNameNotValid = 208,

            // Row and column
            Row_QtyZero = 301,
            Row_RateZero = 302,
            Row_COAIsZero = 303,
            Row_IDZero = 304,
            Row_TaxIDZero = 305,
            Row_UnitIDZero = 306,
            Row_GrossAmountZero = 306,
            Row_TaxAmountZero = 307,
            Row_CompanyIDZero = 308,
            Row_EmployeeIDZero = 309,
            Row_InventoryIDZero = 310,
            Row_ProjectIDZero = 311,
            Row_NoRemarks = 312,
            Row_NoComments = 313,
            Row_NoDescription = 314,
            Row_NoVou_No = 315,
            Row_LessVou_Date = 316,
            Row_MoreVou_Date = 317,
            Row_LessPay_Date = 318,
            Row_MorePay_Date = 319,
            Row_LessInv_Date = 320,
            Row_MoreInv_Date = 321,
            RowValueNull = 322,
            RowNotValidated = 323,
            Row_SrNoIsZero = 324,
            Row_TranIDIsZero = 325,
            Row_BatchIsNull = 326,
            Row_SrNoIsNull = 327,
            Row_TranIDIsNull = 328,
            Row_InventoryIsNull = 329,

            ColumnIsNull = 401,
            ColumnDBNullValue = 402,
            ColumnValueZero = 403,
            ClassIsNull = 404,
            NatureIsNull = 405,
            NotesIsNull = 406,

            //Sale Invoice
            Sale_PayLessInvDate = 501,
            Sale_TaxRateNotMatch = 502,


            // Sqlite Commands
            cmd_InsertNull = 601,
            cmd_UpdateNull = 602,
            cmd_DeleteNull = 603,

            // Error Messages
            NoError = 701,
            Conn_Established = 702,
            Conn_NotEstablished = 703,

            DataRowIsNull = 801,
            IDIsNull = 802,
            IDIsZero = 803,
            CodeIsNull = 804,
            CodeLengthZero = 805,
            TitleLengthZero = 806,

            AccClassZero = 901,
            AccNatureZero = 902,
            AccNotesZero = 903,

            DataTableNotFound = 1001,
            RecordNotSaved = 1002,
            RecordNotSavedError = 1003,
            RecordNotValidated = 1004,
            DataLoadSucesscully = 1005,
            DataLoadFailed = 1006,

            SQLQueryError = 1007,
            CodeLength6 = 1008,
            TitleIsNull = 1009,
            CityIsZero = 1010,
            CodeIsZero = 1011,
            TitleIsZero = 1012,
            CommendError = 1013,
            RowNotUpdated = 1014,
            SQLQueryIsNull = 1015,
            UserProfileIsNull = 1016,

            BookIDIsZero = 1017,
            BookNatureIsZero = 1019,
            BookNatureIsNull = 1020,
            ConnectionsIsNull = 1021,
            RecordIsNull = 1022,
            DataSourceIsNull = 1023,
            CompanyListIsNull = 1024,
            EmployeeListIsNull = 1025,
            ProjectListIsNull = 1026,
            AccountListIsNull = 1027,
            BookTableIsNull = 1028,
            CurrentRowIsNull = 1029,
            DataNotLoaded = 1030,
            BookIDIsNull = 1031,
            COAIsNull = 1032,
            CustomerIDIsNull = 1033,
            EmployeeIDIsNull = 1034,
            ProjectIDIsNull = 1035,
            DescriptionIsNull = 1036,
            DRIsNull = 1037,
            CRIsNull = 1038,
            DRnCRHaveValue = 1039,
            DRnCRAreZero = 1040,
            DRIsNegative = 1041,
            CRIsNegative = 1042,
            DescriptionIsNothing = 1043,
            ProjectIDIsZero = 1044,
            EmployeeIDIsZero = 1045,
            AccountIDIsZero = 1046,
            CustomerIDIsZero = 1047,

            NoRecordFound = 1048,
            PageIsNotValid = 1049,

            Row_NoStatus = 1050,

            SerialNoIsZero = 1051,
            MasterRecordisNull = 1052,
            DetailRecordsisNull = 1053,
            DetailRecordsAreZero = 1054,
            AmountNotEqual = 1055,
            GlobalValueIsNull = 1056,

            VouNoIsNull = 1101,
            VouNoNotDefine = 1102,
            VouNoNotDefineProperly = 1103,
            VouDateLess = 1104,
            VouDateMore = 1105,
            VouTransNotDeleted = 1106,
            VouMasterDeleted = 1107,
            VouMasterNotDeleted = 1108,
            VouDateNotAllowed = 1109,

            PostingMasterRecordNotFound = 1201,
            PostingDetailRecordNotFound = 1202,
            PostingMasterIsNull = 1203,
            PostingDataIsNull = 1204,
            VoucherAlreadyPosted = 1205,
            VoucherNotPosted = 1206,
            VoucherNumberEmpty = 1207,

            TransactionRollback = 1301,
            TransactionCommited = 1302,

            SalesInvoiceIDIsNull = 1401,
            SalesInvoiceRowCountZero = 1402,
            SalesInvoiceIsAlreadyPosted = 1403,
            SalesInvoiceVoucherAlreadyPosted = 1404,
            SalesInvoiceVoucherNotMatched = 1405,
            SalesInvoicePostingFailed = 1406,
            SalesInvoiceAccountNotValid = 1407,

            PurchasedInvoiceIDIsNull = 1501,
            PurchasedInvoiceRowCountZero = 1502,
            PurchasedInvoiceIsAlreadyPosted = 1503,
            PurchasedInvoiceVoucherAlreadyPosted = 1504,
            PurchasedInvoiceVoucherNotMatched = 1505,
            PurchasedInvoicePostingFailed = 1506,
            PurchasedInvoiceAccountNotValid = 1507,

            VoucherNotFound = 1601,
            VoucherAmountIsZero = 1602,
            VoucherDateNotSame = 1603,
            VoucherAmountNotEqual = 1604,
            PackingIdIsZero = 1605,
            Row_PackingIdZero = 1606,
            Row_SizeIDZero = 1607,
            Row_SubCategoryIDZero = 1608,
            CompanyLedgerAC_Notdefined = 1609,
            RefNoIsNull = 1610,
            
        }


        public enum Class
        {
            Success,
            Alert,
            Warning,
            Critical,
            Danger,
            Error,
            Log
        }
    }
}
