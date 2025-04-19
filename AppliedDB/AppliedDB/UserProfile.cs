﻿using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Data;

namespace AppliedDB
{
    public class UserProfile
    {
        public AppUserModel Profile { get; set; }

        public DataRow? UserData { get; set; }

        public bool Valid => GetUserValidation(Profile);

        private bool GetUserValidation(AppUserModel _UserModel)
        {
            if (_UserModel != null)
            {
                if (_UserModel.UserID == (string)UserData["UserID"] && _UserModel.Password == (string)UserData["Password"])
                { return true; }
            }
            return false;
        }

        public UserProfile()
        {
            Profile = new AppUserModel();
            UserData = GetEmptyRow();
        }

        public UserProfile(AppUserModel _UserModel)
        {
            Profile = new AppUserModel();
            if (_UserModel != null)
            {
                UserData = GetUsersData(_UserModel.UserID);
                if (UserData != null)
                {
                    Profile = new()
                    {
                        UserID = UserData["UserID"].ToString() ?? "",
                        Password = UserData["Password"].ToString() ?? "",
                        DisplayName = UserData["DisplayName"].ToString() ?? "",
                        Designation = UserData["Designation"].ToString() ?? "",
                        UserEmail = UserData["UserEmail"].ToString() ?? "",
                        Role = UserData["Role"].ToString() ?? "",
                        LastLogin = UserData["LastLogin"].ToString() ?? "",
                        Session = Guid.NewGuid().ToString(),
                        DataFile = UserData["DataFile"].ToString() ?? "",
                        Company = UserData["Company"].ToString() ?? "",
                    };

                }
            }
        }

        private static DataRow GetUsersData(string _UserID)
        {
            var _CommandText = $"SELECT * FROM [Users] WHERE [UserID] = '{_UserID}'";
            var _Connection = Connections.GetUsersConnection();
            if (_Connection is not null)
            {

                _Connection.Open();
                SQLiteCommand _Command = new(_CommandText, _Connection);
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, "Users");
                _Connection.Close();

                if (_DataSet.Tables.Count == 1)
                {
                    return _DataSet.Tables[0].Rows[0];
                }
            }
            return GetEmptyRow();
        }

        private static DataRow GetEmptyRow()
        {
            // Crete this method for remove null reference for GetUserData .... do not remove..
            DataTable _Table = new();
            _Table.Columns.Add("ID", typeof(int));
            return _Table.NewRow();
        }

        public static DataRow RemoveNulls(DataRow _Row)
        {
            foreach (DataColumn Column in _Row.Table.Columns)
            {
                if (_Row[Column] == DBNull.Value)
                {
                    var _Type = Column.DataType;
                    if (_Type == typeof(string)) { _Row[Column] = "---"; }
                    if (_Type == typeof(int)) { _Row[Column] = 0; }
                    if (_Type == typeof(long)) { _Row[Column] = 0; }
                    if (_Type == typeof(short)) { _Row[Column] = 0; }
                    if (_Type == typeof(decimal)) { _Row[Column] = 0.00M; }
                    if (_Type == typeof(float)) { _Row[Column] = 0.00F; }
                    if (_Type == typeof(bool)) { _Row[Column] = false; }
                    if (_Type == typeof(DateTime)) { _Row[Column] = new DateTime(2000, 1, 1); }
                }
            }
            return _Row;
        }
    }

    public class AppUserModel
    {
        [Required]
        public string UserID { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string LastLogin { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string DataFile { get; set; } = string.Empty;
        public string DataPath { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string PIN { get; set; } = "0000";
        public bool IsSaved { get; set; }
        public int LanguageID { get; set; } = 1;                // Default Language English.


        public string BasePath { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;
        public string ClientFolder { get; set; } = string.Empty;
        public string UsersFolder { get; set; } = string.Empty;
        public string ReportFolder { get; set; } = string.Empty;
        public string PDFFolder { get; set; } = string.Empty;
        public string MessageFolder { get; set; } = string.Empty;
        public string LanguageFolder { get; set; } = string.Empty;
        public string ImagesFolder { get; set; } = string.Empty;
        public string SystemFolder { get; set; } = string.Empty;
        public string SessionFolder { get; set; } = string.Empty;
        public string TempDBFolder { get; set; } = string.Empty;
    }
}
