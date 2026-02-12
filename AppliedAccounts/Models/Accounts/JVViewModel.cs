namespace AppliedAccounts.Models.Accounts
{
    public class JVViewModel
    {
        public long ID { get; set; }
        public long TranId { get; set; }
        public string Vou_Type { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public int Sr_No { get; set; }
        public string Ref_No { get; set; }
        public long BookID { get; set; }
        public long COA { get; set; }
        public decimal DR {get; set;}
        public decimal CR {get; set;}
        public long Customer { get; set; }
        public long Employee { get; set; }
        public long Inventory { get; set; }
        public long Project { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }

    }


}
