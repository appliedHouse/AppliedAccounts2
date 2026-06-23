using AppliedDB;
using AppMessages;
using System.Data;
using Enums = AppliedDB.Enums;
using Messages = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Projects
    {


        public ProjectsViewModel MyModel { get; set; }
        public DataTable ProjectList { get; set; }
        public List<CodeTitle> Clients { get; set; }
        public List<CodeTitle> Employees { get; set; }
        //public MessageClass MsgClass { get; set; }

        public DataSource Source { get; set; }


        protected override void OnInitialized()
        {
            Source ??= new(AppGlobal.AppPaths);
            MyModel = new ProjectsViewModel();
            LoadData();

            MsgService.Success(Messages.NoMessage);

        }

        private async Task HandleValidSubmit()
        {
            IsSubmitting = true;

            await Task.Delay(1000); // Simulate async operation

            SubmitMessage = "Project saved successfully!";
            IsSubmitting = false;
        }


        public void LoadData()
        {
            Source ??= new(AppGlobal.AppPaths);
            Clients = Source.GetCodeTitle(Enums.Tables.Customers, "Title");
            Employees = Source.GetCodeTitle(Enums.Tables.Employees, "Title");
            ProjectList = Source.GetTable(Enums.Tables.Project);
        }

        private void CompanyIDChanged(long _ID)
        {
            MyModel.Client = _ID;
            MyModel.TitleClient = Clients
                .Where(e => e.ID == MyModel.Client)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void EmployeeIDChanged(long _ID)
        {
            MyModel.ProjectManager = _ID;
            MyModel.TitleManager = Employees
                .Where(e => e.ID == MyModel.ProjectManager)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void Save()
        {
            DataRow? _Row = Source.GetDataRow(Enums.Tables.Project, MyModel.ID);
            if (_Row == null)
            {
                _Row = Source.GetNewRow(Enums.Tables.Project);
                _Row["ID"] = 0;
            }

            var _Terms = string.IsNullOrEmpty(MyModel.Terms) ? DBNull.Value : (object)MyModel.Terms;
            var _Location = string.IsNullOrEmpty(MyModel.Location) ? DBNull.Value : (object)MyModel.Location;

            _Row["Code"] = MyModel.Code;
            _Row["Title"] = MyModel.Title;
            _Row["Comments"] = MyModel.Comments;
            _Row["Location"] = _Location;
            _Row["Client"] = MyModel.Client;
            _Row["ProjectManager"] = MyModel.ProjectManager;
            _Row["StartDate"] = MyModel.StartDate;
            _Row["EndDate"] = MyModel.EndDate;
            _Row["ActualCost"] = MyModel.ActualCost;
            _Row["Budget"] = MyModel.Budget;
            _Row["IsActive"] = MyModel.IsActive;
            _Row["IsCompleted"] = MyModel.IsCompleted;
            _Row["Terms"] = _Terms;

            Source.Save(_Row);
            MsgService.MsgClass = Source.MsgClass;
            LoadData();
            EditMode = false;
        }

        public void New()
        {
            EditMode = true;
            //DataRow _Row = Source.GetNewRow(Enums.Tables.Project);

            MyModel = new()
            {
                ID = 0,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Client = 0,
                ProjectManager = 0,
            };

            InvokeAsync(StateHasChanged);

        }

        public void Edit(long _ID)
        {
            if (_ID > 0)
            {

                DataView dv = ProjectList.AsDataView();
                dv.RowFilter = $"ID = {_ID}";

                if (dv.Count > 0)
                {
                    var row = dv[0].Row;
                    MyModel = new ProjectsViewModel()
                    {
                        ID = row.Field<long>("ID"),
                        Code = row.Field<string>("Code") ?? "",
                        Title = row.Field<string>("Title") ?? "",
                        Comments = row.Field<string>("Comments") ?? "",
                        Client = row.Field<long>("Client"),
                        ProjectManager = row.Field<long>("ProjectManager"),
                        StartDate = row["StartDate"] != DBNull.Value ? row.Field<DateTime>("StartDate") : DateTime.Now,
                        EndDate = row["EndDate"] != DBNull.Value ? row.Field<DateTime>("EndDate") : DateTime.Now,
                        IsActive = row.Field<bool>("IsActive"),
                        IsCompleted = row.Field<bool>("IsCompleted"),
                        Terms = row.Field<string>("Terms") ?? "",
                        Location = row.Field<string>("Location") ?? "",
                        ActualCost = row.Field<decimal>("ActualCost"),
                        Budget = row.Field<decimal>("Budget")
                    };

                    EditMode = true;
                }
                else
                {
                    MsgService.Alert($"Projct Id {_ID} not found to edit.");
                }
            }
        }

        public void Delete(long _ID)
        {
            if (_ID > 0)
            {

                DataRow? _Row = Source.GetDataRow(Enums.Tables.Project, _ID);
                if (_Row != null)
                {
                    if (DelValidaded(_ID))
                    {
                        Source.Delete(_Row);
                        LoadData();
                    }
                    else
                    {
                        MsgService.Warning(Messages.RecordCanNotDelete);
                    }
                }
            }
            else
            {
                MsgService.Alert(Messages.NoRecordFound);
            }
        }

        private bool DelValidaded(long _ID)
        {
            bool _Result = true;
            DataRow?
            _Row = Source.GetDataRow($"SELECT * FROM [Ledger] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [BillPayable2] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [BillReceivable2] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [CashBook] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [Receipt2] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [BankBook] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [CashBook] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }
            _Row = Source.GetDataRow($"SELECT * FROM [FinishedGoods] WHERE [Project] = {_ID} LIMIT 1;"); if (_Row != null) { return false; }

            return _Result;
        }

        public void BackPage()
        {
            AppGlobal.NavManager.NavigateTo("Accounts/Projects", true);
        }

    }

    public class ProjectsViewModel
    {
        public long ID { get; set; } = 0;
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;


        public long Client { get; set; } = 0;
        public decimal ActualCost { get; set; }
        public decimal Budget { get; set; } = 0.00M;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IsActive { get; set; } = true;
        public bool IsCompleted { get; set; } = false;
        public long ProjectManager { get; set; } = 0;
        public string Terms { get; set; } = string.Empty;

        public string TitleClient { get; set; } = string.Empty;
        public string TitleManager { get; set; } = string.Empty;

    }
}
