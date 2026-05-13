using AppliedAccounts.Authentication;
using Menus;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Menu
{
    public partial class MainMenu
    {
        //[Parameter] public MenusClass MenuClass { get; set; }

        private string buildNum { get; set; } = "2.0.12";
        private bool SubMenu { get; set; } = false;
        public async Task Beep() { await js.InvokeVoidAsync("playBeep"); }

        private async Task MenuClick(int MenuID)
        {
            //await js.InvokeVoidAsync("playBeep"); await Task.Delay(100);
            await Beep();


            var _SelectedMenu = MenuClass.GetMenu(MenuID);
            if (_SelectedMenu == null) { return; }

            #region Logout User
            if (MenuID == (int)MenuEnum.Menus.Logout)
            {
                var UserService = (UserAuthenticationStateProvider)AuthState;
                await UserService.Logout();  //UpdateAuthenticateState(null);                    // Logout User
                NavManager.NavigateTo("/");
                return;
            }
            #endregion

            #region Home Button
            if (MenuID == (int)MenuEnum.Menus.Home)
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
        //private async Task MenuClick(int MenuID)
        //{
        //    await Beep(); await Task.Delay(100);

        //    var _SelectedMenu = MenuClass.GetMenu(MenuID);
        //    if (_SelectedMenu == null) { return; }

        //    #region Logout User
        //    if (MenuID == (int)MenuEnum.Menus.Logout)
        //    {
        //        var UserService = (UserAuthenticationStateProvider)AuthState;
        //        await UserService.UpdateAuthenticateState(null);                    // Logout User
        //        NavManager.NavigateTo("/");
        //        return;
        //    }
        //    #endregion

        //    #region Home Button
        //    if (MenuID == (int)MenuEnum.Menus.Home)
        //    {
        //        NavManager.NavigateTo("/");
        //        return;
        //    }
        //    #endregion

        //    #region Menu button Clieck

        //    if (_SelectedMenu.Level == 1)
        //    {
        //        SubMenu = !SubMenu;
        //        MenuClass.TopSelected(_SelectedMenu, SubMenu);
        //    }
        //    if (_SelectedMenu.Level == 2)
        //    {
        //        MenuClass.SecondLevel(_SelectedMenu);
        //    }
        //    #endregion

        //    if (!string.IsNullOrEmpty(_SelectedMenu.NavigateTo))
        //    {
        //        NavManager.NavigateTo(_SelectedMenu.NavigateTo);
        //    }

        //}


    }
}
