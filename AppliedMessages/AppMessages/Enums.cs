namespace AppMessages
{
    public class Enums
    {
        public enum Messages
        {

            NoMessage,
            Default = 1,
            Save = 2,
            NotSave = 5,
            Delete = 4,
            NotDelete = 6,
            Insert = 3,
            NotInsert = 7,

            RowInserted = 52,
            RowNotInserted = 54,
            RowDeleted = 56,
            RowNotDeleted = 58,

            // Reports
            prtReportError = 600101,
            rptRDLCNotExist = 600102,
            rptOutputFileIsZero = 600103,
            rptDataTableIsNull = 600104,
            rptDataRowIsZero = 600105,
            rptParametersNotValid = 600106,
            rptNotValidToPrint = 600107,
            rptDataSetNameNotValid = 600108,

            // Row and column
            Row_QtyZero = 30,
            Row_RateZero = 31,
            Row_COAIsZero = 95,
            Row_IDZero = 97,
            Row_TaxIDZero = 220,
            Row_UnitIDZero = 219,
            Row_GrossAmountZero = 33,
            Row_TaxAmountZero = 32,
            Row_CompanyIDZero = 99,
            Row_EmployeeIDZero = 81,
            Row_InventoryIDZero = 101,
            Row_ProjectIDZero = 189,
            Row_NoRemarks = 38,
            Row_NoComments = 39,
            Row_NoDescription = 40,
            Row_NoVou_No = 103,
            Row_LessVou_Date = 105,
            Row_MoreVou_Date = 107,
            Row_LessPay_Date = 109,
            Row_MorePay_Date = 111,
            Row_LessInv_Date = 207,
            Row_MoreInv_Date = 113,
            RowValueNull = 115,
            RowNotValidated = 217,

            ColumnIsNull = 14,
            ColumnDBNullValue = 117,
            ColumnValueZero = 15,
            ClassIsNull = 16,
            NatureIsNull = 17,
            NotesIsNull = 18,

            //Sale Invoice
            Sale_PayLessInvDate = 119,
            Sale_TaxRateNotMatch = 121,


            // Sqlite Commands
            cmd_InsertNull = 44,
            cmd_UpdateNull = 45,
            cmd_DeleteNull = 46,

            // Error Messages
            NoError = 123,
            Conn_Established = 50,
            Conn_NotEstablished = 125,

            DataRowIsNull = 127,
            IDIsNull = 11,
            IDIsZero = 129,
            CodeIsNull = 65,
            CodeLengthZero = 211,
            TitleLengthZero = 213,

            AccClassZero = 19,
            AccNatureZero = 20,
            AccNotesZero = 21,

            DataTableNotFound = 131,
            RecordNotSaved = 133,
            RecordNotSavedError = 135,
            RecordNotValidated = 137,
            DataLoadSucesscully = 48,
            DataLoadFailed = 49,

            SQLQueryError = 139,
            CodeLength6 = 10,
            TitleIsNull = 13,
            CityIsZero = 24,
            CodeIsZero = 12,
            TitleIsZero = 14,
            CommendError = 141,
            RowNotUpdated = 143,
            SQLQueryIsNull = 215,
            UserProfileIsNull = 145,
           
            BookIDIsZero = 147,
            BookNatureIsZero = 149,
            BookNatureIsNull = 209,
            ConnectionsIsNull = 151,
            RecordIsNull = 153,
            DataSourceIsNull = 155,
            CompanyListIsNull = 157,
            EmployeeListIsNull = 159,
            ProjectListIsNull = 161,
            AccountListIsNull = 163,
            BookTableIsNull = 165,
            CurrentRowIsNull = 167,
            DataNotLoaded = 169,
            BookIDIsNull = 171,
            COAIsNull = 95,
            CustomerIDIsNull = 33,
            EmployeeIDIsNull = 173,
            ProjectIDIsNull = 175,
            DescriptionIsNull = 84,
            DRIsNull = 177,
            CRIsNull = 179,
            DRnCRHaveValue = 181,
            DRnCRAreZero = 183,
            DRIsNegative = 185,
            CRIsNegative = 187,
            DescriptionIsNothing = 193,
            ProjectIDIsZero = 36,
            EmployeeIDIsZero = 37,
            AccountIDIsZero = 191,
            CustomerIDIsZero = 192,
            
            NoRecordFound = 193,
            PageIsNotValid = 195,
           
            Row_NoStatus = 197,
            
            SerialNoIsZero = 199,
            MasterRecordisNull = 201,
            DetailRecordsisNull = 203,
            DetailRecordsAreZero = 205,
            AmountNotEqual = 218,
            GlobalValueIsNull = 219,

            VouNoIsNull = 401,
            VouNoNotDefine = 402,
            VouNoNotDefineProperly = 403,
            VouDateLess = 404,
            VouDateMore = 405,
            VouTransNotDeleted = 406,
            VouMasterDeleted = 407,
            VouMasterNotDeleted = 408,
            
            PostingMasterRecordNotFound = 6001,
            PostingDetailRecordNotFound = 6002,
            PostingMasterIsNull = 6003,
            PostingDataIsNull = 6004,
            VoucherAlreadyPosted = 6005,
            VoucherNotPosted = 6006,
            VoucherNumberEmpty = 6007,

            TransactionRollback = 7001,
            TransactionCommited = 7002,
            
            SalesInvoiceIDIsNull = 8001,
            SalesInvoiceRowCountZero = 8002,
            SalesInvoiceIsAlreadyPosted = 8003,
            SalesInvoiceVoucherAlreadyPosted = 8004,
            SalesInvoiceVoucherNotMatched = 8005,
            SalesInvoicePostingFailed = 8006,
            SalesInvoiceAccountNotValid = 8007,
            VoucherNotFound = 600109,
            VoucherAmountIsZero = 600110,
            VoucherDateNotSame = 600111,
            VoucherAmountNotEqual = 600112,
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
