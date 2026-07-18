using Microsoft.AspNetCore.Components;

namespace Menus
{
    public static class NavigateExtensions
    {
        private static List<MenuItem>? _cachedMenus;
        private static readonly object _lock = new();

        private static List<MenuItem> GetMenus()
        {
            if (_cachedMenus == null)
            {
                lock (_lock)
                {
                    _cachedMenus ??= MenusFromDB.Get2();
                }
            }
            return _cachedMenus;
        }

        // Renamed from NavTo to GetPath
        public static string GetPath(this MenuID menuId)
        {
            var menu = GetMenus().FirstOrDefault(m => m.ID == (int)menuId);
            return menu?.NavigateTo ?? "/";
        }

        // Direct navigation
        public static void GoTo(this NavigationManager navManager, MenuID menuId)
        {
            navManager.NavigateTo(menuId.GetPath());
        }

        // Get full MenuItem
        public static MenuItem GetMenuItem(this MenuID menuId)
        {
            return GetMenus().FirstOrDefault(m => m.ID == (int)menuId)!;
        }

        // Get sub menus
        public static List<MenuItem> GetSubMenus(this MenuID menuId)
        {
            return GetMenus().Where(m => m.ParentID == (int)menuId).ToList();
        }

        // Get title
        public static string GetTitle(this MenuID menuId)
        {
            var menu = GetMenus().FirstOrDefault(m => m.ID == (int)menuId);
            return menu?.Title ?? menuId.ToString();
        }

        // Get icon
        public static string GetIcon(this MenuID menuId)
        {
            var menu = GetMenus().FirstOrDefault(m => m.ID == (int)menuId);
            return menu?.Icon ?? "";
        }

        // Check if exists
        public static bool Exists(this MenuID menuId)
        {
            return GetMenus().Any(m => m.ID == (int)menuId);
        }
    }
}