
namespace Menus
{
    public interface IMenusClass
    {
        public List<MenuItem> MyMenus { get; set; }
        public List<MenuItem> GetTopLevel();
        public List<MenuItem> GetSubMenu(int _Level, int _Parent);
        public Task MenuActive(int TopMenuID);
    }

        public class MenusClass : IMenusClass
    {
        public List<MenuItem> MyMenus { get; set; }

        public MenusClass()
        {
            MyMenus = MenusFromDB.Get();
        }

        public List<MenuItem> GetSubMenu(int _Level, int _Parent)
        {
            return [.. MyMenus.Where(m => m.Level == 2 && m.ParentID == _Parent)];
        }

        public List<MenuItem> GetTopLevel()
        {
            return [.. MyMenus.Where(m => m.Level == 1)];
        }

        public async Task MenuActive(int TopMenuID)
        {
            foreach (var item in MyMenus.Where(m => m.Level == 2 && m.ParentID == TopMenuID))
            {
                item.Active = !item.Active;
            }
            await Task.Delay(1000);
        }
    }
}
