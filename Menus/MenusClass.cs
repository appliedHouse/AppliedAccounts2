using System.Collections.Generic;

namespace Menus
{
    public interface IMenusClass
    {
        List<MenuItem> MyMenus { get; set; }
        void GetTopLevel();
        void TopSelected(MenuItem MenuItem, bool ShowSubMenu);
        void SecondLevel(MenuItem MenuItem);
        MenuItem? GetMenu(int _MenuID); // Change return type to nullable
    }

    public class MenusClass : IMenusClass
    {
        public List<MenuItem> MyMenus { get; set; }
        private List<MenuItem> FullMenus { get; set; }
        public bool ShowSubMenu { get; set; }
        public int TopMenuLevel { get; set; } = 1;

        public MenusClass()
        {
            FullMenus = MenusFromDB.Get();
            GetTopLevel();
        }

        public void Reset()
        {
            GetTopLevel();
        }

        public void HomeButton(bool ShowSubMenu)
        {
            List<MenuItem> _Menus = [];
            TopMenuLevel = 1;
            var _HomeBtn = FullMenus.Where(m => m.Title.ToUpper().Equals("HOME")).First();
            _HomeBtn.Level = TopMenuLevel;

            foreach (var menu in FullMenus)
            {
                if (menu.Level == 1) { _Menus.Add(menu); }
                if(ShowSubMenu)
                {
                    if (menu.ID == _HomeBtn.ID)
                    {
                        _Menus.AddRange(FullMenus.Where(mnu => mnu.ParentID == _HomeBtn.ID));
                    }
                }
            }

            MyMenus = _Menus;
        }


        public void GetTopLevel()   // Main menu or first menu show at start of page
        {
            List<MenuItem> _Menus = [];
            TopMenuLevel = 1;
            _Menus = [.. FullMenus.Where(m => m.Level == 1)];
            _Menus.Where(mnu => mnu.Title.ToUpper().Equals("HOME")).First().Level = TopMenuLevel;
            MyMenus = _Menus;
        }

        public void TopSelected(MenuItem MenuItem, bool ShowSubMenu)
        {
            List<MenuItem> _Menus = [];
            TopMenuLevel = MenuItem.Level;
            
            foreach(var menu in FullMenus)
            {
                if(menu.Level==1) { _Menus.Add(menu); }
                if (ShowSubMenu)
                {
                    if (menu.ID == MenuItem.ID)
                    {
                        _Menus.AddRange(FullMenus.Where(mnu => mnu.ParentID == MenuItem.ID));
                    }
                }
            }
            
            MyMenus = _Menus;
        }

        public void SecondLevel(MenuItem MenuItem)
        {
            List<MenuItem> _Menus = [];
            TopMenuLevel = MenuItem.Level;

            _Menus.Add(FullMenus.FirstOrDefault(m => m.Title.ToUpper().Equals("HOME"))!); // Home menu always show
            _Menus.Add(MenuItem);
            _Menus.AddRange(FullMenus.Where(mnu=> mnu.ParentID==MenuItem.ID)); // Other top level menu
            _Menus.Where(mnu => mnu.Title.ToUpper().Equals("HOME")).First()!.Level = TopMenuLevel;
            
            MyMenus = _Menus;

        }

        public MenuItem? GetMenu(int _MenuID)
        {
            return FullMenus.FirstOrDefault(mnu => mnu.ID == _MenuID);
        }
    }
}
