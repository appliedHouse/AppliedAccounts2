using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;

namespace AppliedAccounts.Models.Accounts
{
    public class JVModel : IVoucher
    {
        #region Variables
        public JVViewModel JvVM { get; set; }
        public GlobalService AppGlobal { get; set; }
        public DateTime LastVoucherDate {get; set;}
        public DateTime MaxVouDate {get; set;}
        public MessageClass MsgClass {get; set;}
        public PrintService ReportService {get; set;}
        public DataSource Source {get; set;}
        public List<CodeTitle> Companies {get; set;}
        public List<CodeTitle> Employees {get; set;}
        public List<CodeTitle> Projects {get; set;}
        public List<CodeTitle> Accounts {get; set;}
        public int Index {get; set;}

        public int Count => throw new NotImplementedException();

        public decimal Tot_DR {get; set;}
        public decimal Tot_CR {get; set;}
        public bool IsWaiting {get; set;}
        #endregion

        public JVModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            
            MsgClass = new();
            JvVM = new();
            ReportService = new();

            Source = new(AppGlobal.AppPaths);

            Companies = Source.GetCustomers();
            Employees = Source.GetEmployees();
            Projects = Source.GetProjects();
            Accounts = Source.GetAccounts();
        }


        public void CalculateTotal()
        {
            throw new NotImplementedException();
        }

        public bool LoadData()
        {

            return true;
        }

        public bool IsTransValidated()
        {
            throw new NotImplementedException();
        }

        public bool IsVoucherValidated()
        {
            throw new NotImplementedException();
        }

        public void Edit(long _ID2)
        {
            throw new NotImplementedException();
        }

        public void New()
        {
            throw new NotImplementedException();
        }

        public void Top()
        {
            throw new NotImplementedException();
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void Back()
        {
            throw new NotImplementedException();
        }

        public void Last()
        {
            throw new NotImplementedException();
        }

        public void Remove(int _SrNo)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
