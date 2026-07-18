using AppliedAccounts.Authentication;
using Menus;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Menu
{
    public partial class MainMenu
    {
        //[Parameter] public MenusClass MenuClass { get; set; }
        [Inject] private IConfiguration Configuration { get; set; } = default!;


        private List<MenuItem> MainMenus = new();
        private Dictionary<int, bool> _selectedTopMenus = new();
        private Dictionary<int, bool> _selectedSubMenus = new();

        [Parameter]
        public MenusClass MenuClass { get; set; } = new MenusClass();
        private string buildNum => Configuration["Build:Number"] ?? "";
        private bool SubMenu { get; set; } = false;
        public async Task Beep() { await js.InvokeVoidAsync("playBeep"); }

        private async Task MenuClick(int MenuID)
        {
            await Beep();

            var _SelectedMenu = MenuClass.GetMenu(MenuID);
            if (_SelectedMenu == null) { return; }

            #region Logout User
            if (MenuID == (int)Menus.MenuID.Logout)
            {
                var UserService = (UserAuthenticationStateProvider)AuthState;
                await UserService.LogoutAsync();  //UpdateAuthenticateState(null);                    // Logout User
                NavManager.NavigateTo("/");
                return;
            }
            #endregion

            #region Home Button
            if (MenuID == (int)Menus.MenuID.Home)
            {
                NavManager.NavigateTo("/");
                return;
            }
            #endregion

            #region Menu button Click

            if (_SelectedMenu.Level == 1)
            {
                SubMenu = !SubMenu;
                MenuClass.TopSelected(_SelectedMenu, SubMenu);
            }
            if (_SelectedMenu.Level == 2)
            {
                MenuClass.SecondLevel(_SelectedMenu);
            }
            #endregion

            if (!string.IsNullOrEmpty(_SelectedMenu.NavigateTo))
            {
                NavManager.NavigateTo(_SelectedMenu.NavigateTo);
            }
        }
    }
}
