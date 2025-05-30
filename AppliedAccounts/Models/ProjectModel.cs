namespace AppliedAccounts.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;

        public virtual DateTime DateStart { get; set; }
        public virtual DateTime DateEnd { get; set; }
        public virtual int Company { get; set; }



    }
}
