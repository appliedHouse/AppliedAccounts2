﻿namespace AppMessages
{
    public class Enums
    {
        public enum Messages
        {

            NoMessage,
            Default = 1,
            Save = 2,
            NotSave =5 ,
            Delete = 4,
            NotDelete = 6,
            Insert = 3,
            NotInsert = 7,

            RowInserted,
            RowNotInserted,
            RowDeleted,
            RowNotDeleted,

            // Reports
            prtReportError,

            // Row and column
            Row_QtyZero,
            Row_RateZero,
            Row_COAIsZero,
            Row_IDZero,
            Row_TaxIDZero,
            Row_TaxAmountZero,
            Row_CompanyIDZero,
            Row_EmployeeIDZero,
            Row_InventoryIDZero,
            Row_ProjectIDZero,
            Row_NoRemarks,
            Row_NoComments,
            Row_NoDescription,
            Row_NoVou_No,
            Row_LessVou_Date,
            Row_MoreVou_Date,
            Row_LessPay_Date,
            Row_MorePay_Date,
            Row_LessInv_Date,
            Row_MoreInv_Date,
            RowValueNull,
            RowNotValidated,

            ColumnIsNull,
            ColumnDBNullValue,
            ColumnValueZero,


            //Sale Invoice
            Sale_PayLessInvDate,
            Sale_TaxRateNotMatch,


            // SQLite Commands
            cmd_InsertNull,
            cmd_UpdateNull,
            cmd_DeleteNull,

            // Error Messages
            NoError,
            Conn_Established,
            Conn_NotEstablished,

            DataRowIsNull,
            IDIsNull,
            IDIsZero,
            CodeIsNull,
            CodeLengthZero,
            TitleLengthZero,

            AccClassZero,
            AccNatureZero,
            AccNotesZero,

            DataTableNotFound,
            RecordNotSaved,
            RecordNotSavedError,
            RecordNotValidated,
            DataLoadSucesscully,
            DataLoadFailed,


            SQLQueryError,
            CodeLength6,
            TitleIsNull,
            CityIsZero,
            CodeIsZero,
            TitleIsZero,
            CommendError,
            RowNotUpdated,
            SQLQueryIsNull,
            UserProfileIsNull,
            VouNoNotDefine,
            BookIDIsZero,
            BookNatureIsZero,
            BookNatureIsNull,
            ConnectionsIsNull,
            RecordIsNull,
            RecordsAreNull,
            DataSourceIsNull,
            CompanyListIsNull,
            EmployeeListIsNull,
            ProjectListIsNull,
            AccountListIsNull,
            BookTableIsNull,
            CurrentRowIsNull,
            DataNotLoaded,
            BookIDIsNull,
            COAIsNull,
            CustomerIDIsNull,
            EmployeeIDIsNull,
            ProjectIDIsNull,
            DescriptionIsNull,
            DRIsNull,
            CRIsNull,
            DRnCRHaveValue,
            DRnCRAreZero,
            DRIsNegative,
            CRIsNegative,
            DescriptionIsNothing,
            ProjectIDIsZero,
            EmployeeIDIsZero,
            AccountIDIsZero,
            CustomerIDIsZero,
            VouNoIsNull,
            NoRecordFound,
            PageIsNotValid,
            VouDateLess,
            VouDateMore,
            Row_NoStatus,
            VouNoNotDefineProperly,
            SerialNoIsZero,
            MasterRecordisNull,
            DetailRecordsisNull,
            DetailRecordsAreZero,
            
        }

        public enum Class
        {
            Sucess,
            Alert,
            Warrning,
            Critical,
            Danger,
            Error,
            Log
        }
    }
}
