
using System.Collections.Generic;

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
        private List<MenuItem> FullMenus { get; set; }

        public MenusClass()
        {
            FullMenus = MenusFromDB.Get();
            MyMenus = MenusFromDB.Get();
        }

        public void Reset()
        {
            MyMenus = FullMenus;
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

        public List<MenuItem> SelectedMenu(int MenuID)
        {
            List<MenuItem> _Menus = [];
            var menu1 = MyMenus.FirstOrDefault(m => m.ID == 1);
            if (menu1 != null)  { _Menus.Add(menu1); }

            var menus2 = MyMenus.Where(m => m.ID == MenuID || m.ParentID == MenuID).ToList();

            _Menus.AddRange(menus2);

            foreach (var Menu in _Menus)
            {
                if(Menu.Level > 1)
                {
                    Menu.Level = Menu.Level - 1;
                }
            }

            return _Menus;
        }
    }
}
