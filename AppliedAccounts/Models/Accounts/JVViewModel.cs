namespace AppliedAccounts.Models.Accounts
{
    public class JVViewModel
    {
        public long ID { get; set; }
        public long TranId { get; set; }
        public string Vou_Type { get; set; }
        public string Vou_No { get; set; } = "New";
        public DateTime Vou_Date { get; set; }
        public int Sr_No { get; set; }
        public string Ref_No { get; set; }
        public long BookID { get; set; }
        public long COA { get; set; }
        public decimal DR { get; set; }
        public decimal CR { get; set; }
        public long Company { get; set; }
        public long Employee { get; set; }
        public long Inventory { get; set; }
        public long Project { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }

        public string TitleAccount { get; set; }
        public string TitleCompany { get; set; }
        public string TitleProject { get; set; }
        public string TitleEmployee { get; set; }
        public string TitleStock { get; set; }

        public JVViewModel Clone()
        {
            return (JVViewModel)this.MemberwiseClone();
        }
    }
}
