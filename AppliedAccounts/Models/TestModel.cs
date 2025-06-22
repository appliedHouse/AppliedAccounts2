using AppliedAccounts.Services;

namespace AppliedAccounts.Models
{
    public class TestModel
    {

        public GlobalService AppGlobal { get; set; }
        public string SpinnerMessage { get; set; }
        public TestModel(GlobalService appGlobal)
        {
            AppGlobal = appGlobal;
            SpinnerMessage = "Loading...";
        }
    }
}
