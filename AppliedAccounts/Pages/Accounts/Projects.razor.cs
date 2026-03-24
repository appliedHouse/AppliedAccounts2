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


        public Projects()
        {
            
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
            
            LoadData();
        }

        public void New()
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
