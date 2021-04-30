using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.Common
{
    class CommonValidations : NewAutomatedBase
    {
        private NewDefaultPage CurrentPage;
        private UserProfileSearchPage _newuserProfileSearch;

        public CommonValidations(NewDefaultPage currentPage)
        {
            CurrentPage = currentPage;
        }


        public virtual void ValidateSecurityAndNavigationOfAPage
            (string headerMenu, List<string> subMenu, string roleName, List<string> requiredPageHeader, Func<QuickLaunchPage> loginActionForUserWithoutAuthority,
            string[] firstLastNamesForUserWithoutAuthority, Func<QuickLaunchPage> loginActionForInitialUser = null)
        {
            ValidateSecurityOfPage(roleName, true);

            ValidateSecurityOfPage(roleName, false, false, false, "", firstLastNamesForUserWithoutAuthority, loginActionForInitialUser);

            if (subMenu.Count > 1 && requiredPageHeader.Count == 1)
                ValidatePageNavigationForMultiLevelMenu(true, headerMenu, subMenu, requiredPageHeader[0]);
            else
                for (var i = 0; i < subMenu.Count; i++)
                    ValidatePageNavigation(true, headerMenu, subMenu[i], requiredPageHeader[i]);

            CurrentPage.Logout();
            loginActionForUserWithoutAuthority(); //Any Retro Page

            if (subMenu.Count > 1 && requiredPageHeader.Count == 1)
                ValidatePageNavigationForMultiLevelMenu(false, headerMenu, subMenu, requiredPageHeader[0]);
            else
                for (var i = 0; i < subMenu.Count; i++)
                    ValidatePageNavigation(false, headerMenu, subMenu[i], requiredPageHeader[i]);

            CurrentPage.SwitchTab(CurrentPage.GetCurrentWindowHandle(), true);
        }



        /// <summary>
        /// Method toggles assigning the particular 'privilege' to the user each time it is called. 
        /// </summary>
        /// <param name="privilege"></param>
        /// <returns>The information whether the privilege is assigned or not is returned by 'isPrivilegeAssigned'.</returns>
        public void ValidateSecurityOfPage(string roleName, bool authorityPresent, bool checkRole = false, bool rolePresent = true, string role = "", string[] firstLastNames = null, Func<QuickLaunchPage> initialLogin = null)
        {

            bool adminRequired = string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0;
            // True suggests user whose authority is to be changed is not an admin user

            if (CurrentPage.GetWindowHandlesCount() > 1 && !adminRequired)
                CurrentPage.SwitchTab(CurrentPage.GetCurrentWindowHandle());
            else
            {
                firstLastNames = CurrentPage.GetLoggedInUserFullName().Split(' ');
                if (CurrentPage.GetWindowHandlesCount() > 1)
                    CurrentPage.SwitchTab(CurrentPage.GetCurrentWindowHandle(), true);
                CurrentPage.SwitchTabAndNavigateToQuickLaunchPage(adminRequired);

            }


            if (!CurrentPage.IsPageHeaderPresent() || CurrentPage.GetPageHeader() != PageHeaderEnum.UserProfileSearch.GetStringValue())
                _newuserProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
            _newuserProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
            _newuserProfileSearch.SearchUserByNameOrId(firstLastNames.ToList(), false, true);
            _newuserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(firstLastNames[0] + " " + firstLastNames[1]);

            _newuserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities.GetStringValue());

            if (authorityPresent)
                _newuserProfileSearch.IsAvailableAssignedRowPresent(1, roleName).ShouldBeTrue(
                    $"{roleName} role must be in assigned for current user");
            else
                _newuserProfileSearch.IsAvailableAssignedRowPresent(1, roleName, false).ShouldBeTrue(
                    $"{roleName} role must be in available for current user");

            if (checkRole)
            {
                Console.WriteLine($"This role is checked<{role}>");
                if (rolePresent)
                    ProfileManager.IsRoleAssigned(role).ShouldBeTrue(string.Format("{0} role must be provided for current user", role));
                else
                {
                    ProfileManager.IsRoleAssigned(role).ShouldBeFalse(string.Format("{0} role must not be provided for current user", role));

                }
            }

            if (initialLogin != null)
            {
                CurrentPage.Logout();
                initialLogin();
            }
            CurrentPage.SwitchTab(CurrentPage.GetCurrentWindowHandle());
            CurrentPage.WaitForStaticTime(1000);
            if (CurrentPage.CurrentPageTitle.Equals(PageTitleEnum.Login.GetStringValue()))
            {
                CurrentPage.ClickOnBrowserBackButton();
                CurrentPage.WaitForStaticTime(1000);
                CurrentPage.WaitForPageToLoad();
                var handles = CurrentPage.GetWindowHandles();
                var current = CurrentPage.GetCurrentWindowHandle();
                if (handles.Count == 3)
                {
                    CurrentPage.SwitchWindow(handles[2]);
                    CurrentPage.CloseWindow();
                    CurrentPage.SwitchWindow(current);
                }
            }


        }

        public virtual void ValidatePageNavigation(bool isAuthorityGiven, string header, string subMenu, string requiredPageHeader)
        {
            CurrentPage.RefreshPage(false, 5000);
            var handles = CurrentPage.GetWindowHandles();
            var current = CurrentPage.GetCurrentWindowHandle();
            if (handles.Count == 3)
            {
                CurrentPage.SwitchWindow(handles[2]);
                CurrentPage.CloseWindow();
                CurrentPage.SwitchWindow(current);
            }
            if (isAuthorityGiven)
            {
                CurrentPage.IsSubMenuPresent(subMenu).ShouldBeTrue(string.Format("Submenu '{0}' is present", subMenu));
                CurrentPage.NavigateToAPage(header, subMenu).GetPageHeader().ShouldBeEqual(requiredPageHeader, "Page Header should be as required");
                handles = CurrentPage.GetWindowHandles();
                current = CurrentPage.GetCurrentWindowHandle();
                if (handles.Count == 3)
                {
                    CurrentPage.SwitchWindow(handles[2]);
                    CurrentPage.CloseWindow();
                    CurrentPage.SwitchWindow(current);
                }
            }
            else
            {
                CurrentPage.IsSubMenuPresent(subMenu).ShouldBeFalse(string.Format("Submenu '{0}' should not be present", subMenu));
            }
        }


        public virtual void ValidatePageNavigationForMultiLevelMenu(bool isAuthorityGiven, string header, List<string> subMenu, string requiredPageHeader)
        {
            CurrentPage.RefreshPage(false, 5000);
            var handles = CurrentPage.GetWindowHandles();
            var current = CurrentPage.GetCurrentWindowHandle();
            if (handles.Count == 3)
            {
                CurrentPage.SwitchWindow(handles[2]);
                CurrentPage.CloseWindow();
                CurrentPage.SwitchWindow(current);
            }
            if (isAuthorityGiven)
            {
                CurrentPage.NavigateToAPage(header, subMenu).GetPageHeader().ShouldBeEqual(requiredPageHeader, "Page Header should be as required");
                handles = CurrentPage.GetWindowHandles();
                current = CurrentPage.GetCurrentWindowHandle();
                if (handles.Count == 3)
                {
                    CurrentPage.SwitchWindow(handles[2]);
                    CurrentPage.CloseWindow();
                    CurrentPage.SwitchWindow(current);
                }
            }
            else
            {
                CurrentPage.IsSubMenuPresent(subMenu.Last()).ShouldBeFalse(string.Format("Submenu '{0}' should not be present", subMenu));
            }
        }

    }
}
