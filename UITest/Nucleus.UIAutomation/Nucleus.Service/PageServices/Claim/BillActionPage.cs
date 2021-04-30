using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Claim
{
    public class BillActionPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly BillActionPageObjects _billActionPage;
        private readonly HeaderMenu _headerMenu;
      
        private string _originalWindow;

        #endregion

        #region PUBLIC FIELDS

        public static int FlagCount;

        #endregion

        #region CONSTRUCTOR

        public BillActionPage(INewNavigator navigator, BillActionPageObjects billActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, billActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _billActionPage = (BillActionPageObjects)PageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _headerMenu = new HeaderMenu(SiteDriver, JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Return true if successful else return false
        /// </summary>
        /// <returns></returns>
        public bool HandleAutomaticallyOpenedPatientBillHistoryPopup()
        {
            bool isHandled = false;
            try
            {
               
                SiteDriver.WaitForCondition(() =>
                {
                    if (SiteDriver.WindowHandles.Count > 1)
                        isHandled = SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageBillHistory.GetStringValue());
                    return isHandled;
                },7000);

                SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                Console.WriteLine("Automatically opened Patient Bill History page closed");
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window.");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return false;
        }

        
        public string GetClaSeqTextLabel()
        {
            return SiteDriver.FindElement(BillActionPageObjects.ClaSeqColumnCssLocator, How.CssSelector).Text;
        }

        public bool IsClaimDetailsSectionPresent()
        {
            return SiteDriver.IsElementPresent("section.claim_details", How.CssSelector);
        }

        public string GetMenuOption(int v)
        {
            return _headerMenu.GetMenuOption(v);
        }

        public void WaitForPageToLoad()
        {
            SiteDriver.WaitForCondition(IsClaimDetailsSectionPresent);
            SiteDriver.WaitForPageToLoad();
            
            
        }




        #endregion

        #region PRIVATE METHODS

        public void MouseOverBillMenu() => GetMouseOver.MouseOverBillMenu();

        public List<string> GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(string headerName) =>
            GetSubMenu.GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(headerName);

        #endregion
    }
}
