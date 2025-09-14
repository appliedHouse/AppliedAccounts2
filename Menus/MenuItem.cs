
namespace Menus
{
    public class MenuItem
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Icon { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ParentID { get; set; }
        public string NavigateTo { get; set; }
    }

    
}
