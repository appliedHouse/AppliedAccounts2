﻿using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using ExcelDataReader;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace AppliedAccounts.Models
{
    public class ImportExcelFile
    {
        public IBrowserFile ExcelFile { get; set; }
        public DataSource Source { get; set; }
        public GlobalService AppGlobal { get; set; }
        public DataSet ImportDataSet { get; set; }
        public bool IsImported { get; set; } = false;
        public string MyMessages { get; set; }


        public ImportExcelFile(IBrowserFile excelFile, GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            //AppUser = appUser;
            ExcelFile = excelFile;
        }


        #region Import Data From Excel file into DataSet

        public async Task ImportDataAsync()
        {
            try
            {
                var _Path = Path.Combine(AppGlobal.AppPaths.FirstPath, AppGlobal.AppPaths.RootPath);
                var _Directory = Path.Combine(_Path, "ExcelFiles");
                if (!Directory.Exists(_Directory)) { Directory.CreateDirectory(_Directory); }

                var _ExcelFile = Path.Combine(_Directory, ExcelFile.Name);

                if (File.Exists(_ExcelFile)) { File.Delete(_ExcelFile); }

                using (FileStream fs = new(_ExcelFile, FileMode.Create, FileAccess.ReadWrite))
                { await ExcelFile.OpenReadStream().CopyToAsync(fs); }


                using (var stream = File.Open(_ExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    ImportDataSet = reader.AsDataSet();

                    if (ImportDataSet is not null)
                    {
                        SaveInTable(ImportDataSet);
                        IsImported = true;
                    }
                }


                if (File.Exists(_ExcelFile)) { File.Delete(_ExcelFile); }
            }
            catch (Exception error)
            {
                var _Message = error.Message;
                throw;
            }
        }

        #endregion


        #region Save Imported DataSet into SQL Lite DB Temp with GUID Name.

        internal bool SaveInTable(DataSet importDataSet)
        {
            bool _Result = false;
            int _Records = 0;
            string _Path = Connections.GetTempDBPath();
            string _GUID = Guid.NewGuid().ToString();
            string _Title = $"Import file {ExcelFile} dated {DateTime.Now}";
            string _OldFile = AppRegistry.GetText(AppGlobal.DBFile, "ExcelImport");
            bool _FirstRow = true;

            try
            {
                if (_OldFile.Length > 0)
                {
                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }

                    var _OldFilePath = Path.Combine(_Path, _OldFile + ".db");
                    if (File.Exists(_OldFilePath)) { File.Delete(_OldFilePath); }
                }
            }
            catch (Exception) { }



            AppRegistry.SetKey(AppGlobal.DBFile, "ExcelImport", _GUID, KeyType.Text, _Title);

            string _ConnText = $"";
            string _ImportDBPath = Path.Combine(_Path, _GUID + ".db");
            SQLiteConnection.CreateFile(_ImportDBPath);
            SQLiteConnection _TempDBConnection = new($"Data Source={_ImportDBPath}");

            if (_TempDBConnection.State != ConnectionState.Open) { _TempDBConnection.Open(); }

            if (importDataSet is not null)
            {
                foreach (DataTable _Table in importDataSet.Tables)
                {
                    _FirstRow = true;
                    var _Text = new StringBuilder();
                    _Text.Append($"CREATE TABLE [{_Table.TableName}] (");

                    string _TableName = _Table.TableName;
                    string _LastColumn = _Table.Columns[_Table.Columns.Count - 1].ColumnName;

                    if (_FirstRow)
                    {
                        int _ColumnNo = 1;
                        DataRow _FirstDataRow = _Table.Rows[0];
                        foreach (DataColumn _Column in _Table.Columns)
                        {
                            string? _ColumnValue = _FirstDataRow[_Column.ColumnName].ToString();

                            if (string.IsNullOrEmpty(_ColumnValue))
                            {
                                _ColumnValue = $"Column{_ColumnNo}";
                                _ColumnNo++;
                            }
                            _Text.Append($"[{_ColumnValue}] ");
                            _Text.Append($"NVARCHAR");

                            if (_Column.ColumnName != _LastColumn) { _Text.Append(','); }
                        }
                    }
                    else
                    {
                        foreach (DataColumn _Column in _Table.Columns)
                        {
                            _Text.AppendLine($"[{_Column.ColumnName}] ");
                            _Text.Append($"NVARCHAR");
                            //_Text.Append($"[{_Column.DataType}]");

                            if (_Column.ColumnName != _LastColumn) { _Text.Append(','); }
                        }
                    }

                    _Text.Append(')');
                    SQLiteCommand _Command = new(_Text.ToString(), _TempDBConnection);
                    _Command.ExecuteNonQuery();

                    foreach (DataRow _Row in _Table.Rows)
                    {
                        if (_FirstRow) { _FirstRow = false; continue; }


                        _Text.Clear();
                        _Text.Append($"INSERT INTO [{_Table.TableName}] VALUES (");
                        foreach (DataColumn _Column in _Table.Columns)
                        {
                            string RowValue = _Row[_Column.ColumnName].ToString() ?? "";
                            if (RowValue.Contains("'"))
                            {
                                RowValue = RowValue.Replace("'", ",");
                            }

                            _Text.AppendLine($"'{RowValue}'");
                            if (_Column.ColumnName != _LastColumn) { _Text.Append(','); }
                        }
                        _Text.Append(')');

                        _Command.CommandText = _Text.ToString();
                        _Records += _Command.ExecuteNonQuery();
                        _Result = true;

                    }
                }
            }

            _TempDBConnection.Close();
            return _Result;

        }
        #endregion
    }
}
