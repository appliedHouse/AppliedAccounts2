namespace AppliedAccounts.Models
{
    public class CompanyModel
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NTN { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int Status { get; set; }

    }
}
