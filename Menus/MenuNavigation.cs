namespace Menus
{
    public static class MenuNavigation
    {
        private static readonly Dictionary<MenuID, string> _paths;
        private static readonly Dictionary<MenuID, string> _titles;
        private static readonly Dictionary<MenuID, string> _icons;

        static MenuNavigation()
        {
            // Load data from your existing Get2() method
            var menus = MenusFromDB.Get2();

            _paths = menus.ToDictionary(
                m => (MenuID)m.ID,
                m => m.NavigateTo
            );

            _titles = menus.ToDictionary(
                m => (MenuID)m.ID,
                m => m.Title
            );

            _icons = menus.ToDictionary(
                m => (MenuID)m.ID,
                m => m.Icon
            );
        }

        public static string NavTo(this MenuID menuId)
        {
            return _paths.TryGetValue(menuId, out var path) ? path : "/";
        }

        public static string GetTitle(this MenuID menuId)
        {
            return _titles.TryGetValue(menuId, out var title) ? title : menuId.ToString();
        }

        public static string GetIcon(this MenuID menuId)
        {
            return _icons.TryGetValue(menuId, out var icon) ? icon : "";
        }

        public static MenuItem GetMenuItem(this MenuID menuId)
        {
            var menus = MenusFromDB.Get2();
            return menus.FirstOrDefault(m => m.ID == (int)menuId)!;

        }
    }
}