namespace AppliedAccounts.Pages.Purchase
{
    public partial class Purchased
    {
    }

    public class PayableVM
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Vou_No { get; set; } = string.Empty;
        public DateTime Vou_Date { get; set; }

        public int Company { get; set; }
        public string TitleCompany { get; set; }
        public int? Employee { get; set; }
        public string TitleEmployee { get; set; }
        public string? Ref_No { get; set; }

        public string Inv_No { get; set; } = string.Empty;
        public DateTime? Inv_Date { get; set; }

        public DateTime Pay_Date { get; set; }

        public decimal Amount { get; set; } = 0;

        public string Description { get; set; } = string.Empty;
        public string? Comments { get; set; }

        public string Status { get; set; } = "Submitted";

        // 🔹 Child Records
        public List<PayableItemVM> Items { get; set; } = new();
    }

    public class PayableItemVM
    {
        public long ID { get; set; }

        public int Sr_No { get; set; }

        public long TranID { get; set; }   // FK → BillPayable.ID

        public long Inventory { get; set; }
        public string TitleInventory { get; set;  }

        public string Batch { get; set; } = string.Empty;

        public decimal Qty { get; set; }
        public decimal Rate { get; set; }

        public long? Tax { get; set; }
        public decimal? Tax_Rate { get; set; }

        public string? Description { get; set; }

        public long? Project { get; set; }

        public long? Unit { get; set; }

        // 🔹 Optional Calculated Fields (very useful in UI)
        public decimal Total => Qty * Rate;

        public decimal TaxAmount => Tax_Rate.HasValue
            ? (Qty * Rate * Tax_Rate.Value / 100)
            : 0;

        public decimal NetTotal => Total + TaxAmount;
    }
}
