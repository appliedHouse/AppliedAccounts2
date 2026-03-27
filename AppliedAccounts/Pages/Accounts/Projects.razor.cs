using AppliedDB;
using AppMessages;
using System.Data;
using Enums = AppliedDB.Enums;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Projects
    {


        public ProjectsViewModel MyModel { get; set; }
        public DataTable ProjectList { get; set; }
        public List<CodeTitle> Clients { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public MessageClass MsgClass { get; set; } = new();

        public DataSource Source { get; set; }
        public bool EditMode { get; set; } = false;


        protected override void OnInitialized()
        {
            Source = new(AppGlobal.AppPaths);
            MyModel = new ProjectsViewModel();
            LoadData();
        }

        public Projects()
        {
            //Source ??= new(AppGlobal.AppPaths);
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
            MyModel.Title = Clients
                .Where(e => e.ID == MyModel.Client)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void EmployeeIDChanged(long _ID)
        {
            MyModel.ProjectManager = _ID;
            MyModel.Title = Employees
                .Where(e => e.ID == MyModel.ProjectManager)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void Save()
        {
            EditMode = false;
            LoadData();
        }

        public void New()
        {

        }

        public void Edit(long _ID)
        {
            EditMode = true;

            var _Row = Source.Seek(Enums.Tables.Project, _ID);
            MyModel = new()
            {
                ID = _Row.Field<long>("ID"),
                Title = _Row.Field<string>("Title") ?? "",
                Comments = _Row.Field<string>("Comments") ?? "",

                Client = 0,
                ActualCost = 0.00M,
                Budget = 0.00M,
                Location = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                IsCompleted = false,
                IsActive = true,
                Terms = ""
            };
        }

        public void Delete(long _ID)
        {

        }

        public void BackPage()
        {
            AppGlobal.NavManager.NavigateTo("/Accounts/Dictionery", true);
        }

    }

    public class ProjectsViewModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }


        public long Client { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Budget { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }  
        public bool IsCompleted { get; set; }
        public long ProjectManager { get; set; }
        public string Terms { get; set; }

    }
}
