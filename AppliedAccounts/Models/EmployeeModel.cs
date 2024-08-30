namespace AppliedAccounts.Models
{
    public class EmployeeModel
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Full_Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime Join { get; set; }
        public DateTime Left { get; set; }
        public DateTime DOB { get; set; }
        public string CNIC { get; set; } = string.Empty;

    }
}
