namespace AppliedAccounts.Pages
{
    public partial class Test
    {

        public IConfiguration MyConfig { get; set; }
        public IWebHostEnvironment MyEnvir { get; set; }
        public string? Value1 { get; set; }
        public string? Value2 { get; set; }

        public Test() { }

        public Test(IConfiguration _Config, IWebHostEnvironment _Envir)
        {
            MyConfig = _Config;
            MyEnvir = _Envir;

            Value1 = MyConfig.GetSection("Paths:DBPath").ToString();
            Value2 = MyEnvir.ContentRootPath;

        }


    }
}
