using System;
using System.Collections.Generic;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common.Constants;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.Support.Menu
{
    
    /// <summary>
    /// Mouse over for menu 
    /// </summary>
    public sealed class Mouseover
    {

        #region Private VAriables
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;
        private readonly SubMenu _subMenu;
        #endregion


        #region CONSTRUCTOR

        public Mouseover(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
            _subMenu = new SubMenu(SiteDriver, JavaScriptExecutor);
        }

        #endregion


        #region PUBLIC METHODS


        public bool IsDciClaimsWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.DciClaimsWorkList);
        }
        /// <summary>
        /// Mouse over claim search
        /// </summary>
        public bool IsClaimSearch()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimSearch);
        }

        /// <summary>
        /// Mouse over Bill search
        /// </summary>
        public bool IsBillSearch()
        {
            return MouseOver(HeaderMenu.Bill, SubMenu.BillSearch);
        }



        /// <summary>
        /// Mouse over Bill header menu
        /// </summary>
        public void MouseOverBillMenu()
        {
            MouseOver(HeaderMenu.Bill);
        }

        /// <summary>
        /// Mouse over Claim header menu
        /// </summary>
        public void MouseOverClaimMenu()
        {
            MouseOver(HeaderMenu.Claim);
            SiteDriver.WaitToLoadNew(400);
        }

        /// <summary>
        /// Mouse over Batch header menu
        /// </summary>
        public void MouseOverBatchMenu()
        {
            MouseOver(HeaderMenu.Batch);
            SiteDriver.WaitToLoadNew(400);
        }

        /// <summary>
        /// Mouse over Claim header menu
        /// </summary>
        public void MouseOutClaimMenu()
        {
            MouseOut(HeaderMenu.Claim);
            SiteDriver.WaitToLoadNew(400);
        }

        /// <summary>
        /// Mouse over Appeal header menu
        /// </summary>
        public void MouseOutAppealMenu()
        {
            MouseOut(HeaderMenu.Appeal);
            SiteDriver.WaitToLoadNew(400);
        }
        
        /// <summary>
        /// Mouse over fci claims worklist
        /// </summary>
        /// <returns></returns>
        public bool IsFciClaimsWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.FciClaimsWorkList);
        }

        /// <summary>
        /// Mouse over pci claims worklist
        /// </summary>
        /// <returns></returns>
        public bool IsCVClaimsWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVClaimsWorkList);
        }

        public bool IsFFPClaimsWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.FfpClaimsWorkList);
        }

        public bool IsPCI_FCIClaimsWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, new List<string>{ SubMenu.CVClaimsWorkList , SubMenu.FciClaimsWorkList });
        }

        /// <summary>
        /// Mouse over pci claims worklist
        /// </summary>
        /// <returns></returns>
        public  bool IsCVCodersClaim()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVCodersClaims);
        }

        public  bool IsPciBillWorkList()
        {
            return MouseOver(HeaderMenu.Bill, SubMenu.BillWorkList, SubMenu.PciBillWorkList);
        }

        public  bool IsCVQCClaims()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVQCWorkList);
        }

        public  bool IsCvQcWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVQCWorkList);
        }
        public  bool IsCVRnWorkList()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVRnWorkList);
        }

        /// <summary>
        /// Mouse over appeal search
        /// </summary>
        public  bool IsAppealSearch()
        {
            return MouseOver(HeaderMenu.Appeal, SubMenu.AppealSearch);
        }
        public bool IsAppealCategory()
        {
            return MouseOver(HeaderMenu.Manager, SubMenu.AppealCategoryManager);
        }
        public bool IsAppealRationalemanager()
        {
            return MouseOver(HeaderMenu.Manager, SubMenu.AppealRationaleManager);
        }

        /// <summary>
        /// Mouse over my appeals
        /// </summary>
        public  bool IsMyAppeals()
        {
            return MouseOver(HeaderMenu.Appeal, SubMenu.MyAppeals);
        }

        /// <summary>
        /// Mouse over appeal creator
        /// </summary>
        public  bool IsAppealCreator()
        {
            return MouseOver(HeaderMenu.Appeal, SubMenu.AppealCreator);
        }

        /// <summary>
        /// Mouse over appeal manager
        /// </summary>
        public  bool IsAppealManager()
        {
            return MouseOver(HeaderMenu.Manager, SubMenu.AppealManager);
        }

       
        /// <summary>
        /// Mouse over new provider search
        /// </summary>
        /// <returns></returns>
        public  bool IsProviderSearch()
        {
            return MouseOver(HeaderMenu.Provider, SubMenu.ProviderSearch);
        }


        ///<summary>
        /// Mouse over report center
        /// </summary>
        public  bool IsReportCenter()
        {
            return MouseOver(HeaderMenu.Reports, SubMenu.ReportCenter);
        }


        ///<summary>
        /// Mouse over new client search
        /// </summary>
        public  bool IsNewClientSearch()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.ClientSearch);
        }



        ///<summary>
        /// Mouse over user from settings
        /// </summary>
        public  bool IsSettingsUserMyProfile()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.User, SubMenu.MyProfile);
        }

        ///<summary>
        /// Mouse over user from settings
        /// </summary>
        public  bool IsSettingsMaintenanceNotices()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.MaintenanceNotices);
        }

        ///<summary>
        /// Mouse over user profile search from settings
        /// </summary>
        public  bool IsSettingsUserProfileSearch()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.User, SubMenu.NewUserProfileSearch);
        }

        ///<summary>
        /// Mouse over new user profile search from settings
        /// </summary>
        public  bool IsSettingsNewUserProfileSearch()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.User,SubMenu.NewUserProfileSearch);
        }

        ///<summary>
        /// Mouse over role manager from settings
        /// </summary>
        public  bool IsSettingsRoleManager()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.User, SubMenu.RoleManager);
        }
        ///<summary>
        /// Mouse over user 
        /// </summary>
        public  bool IsUser()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.User);
        }

        /// <summary>
        /// Mouse over invoice search
        /// </summary>
        /// <returns></returns>
        public  bool IsInvoiceSearch()
        {
            return MouseOver(HeaderMenu.Invoice, SubMenu.InvoiceSearch);
        }

        /// <summary>
        /// Mouse over new invoice search
        /// </summary>
        /// <returns></returns>
        public  bool IsNewInvoiceSearch()
        {
            return MouseOver(HeaderMenu.Invoice, SubMenu.InvoiceSearch);
        }

        /// <summary>
        /// Mouse over new batch search
        /// </summary>
        /// <returns></returns>
        public  bool IsBatchSearch()
        {
            return MouseOver(HeaderMenu.Batch, SubMenu.BatchSearch);
        }
        public  bool IsLogicSearch()
        {
            return MouseOver(HeaderMenu.Claim, SubMenu.LogicSearch);
        }

        public  bool IsMicrostrategyMaintainacePresent()
        {
            return MouseOver(HeaderMenu.Settings, SubMenu.MicrostrategyMaintenance);
        }

        public  bool IsPreAuthCreatorPresent()
        {
            return MouseOver(HeaderMenu.PreAuthorization, SubMenu.PreAuthCreator);
        }

        public  bool IsPreAuthSearchPresent()
        {
            return MouseOver(HeaderMenu.PreAuthorization, SubMenu.PreAuthSearch);
        }

        /// <summary>
        /// Mouse over alerts list
        /// </summary>
        /// <returns></returns>
        public  bool IsAlertsList()
        {
            return MouseOver(HeaderMenu.Alliance, SubMenu.AlertsList);
        }

        /// <summary>
        /// Mouse over QA Manager
        /// </summary>
        /// <returns></returns>

        public bool IsAnalystManager()
        {
            return MouseOver(HeaderMenu.Manager, SubMenu.QaManager);
        }

        /// <summary>
        /// Mouse over QA Claim Search
        /// </summary>
        /// <returns></returns>
        public  bool IsQAClaimSearch()
        {
            return MouseOver(HeaderMenu.Qa, SubMenu.QaClaimSearch);
        }

        
        public  bool IsQAAppealSearch()
        {
            return MouseOver(HeaderMenu.Qa, SubMenu.QaAppealSearch);
        }

        public  bool IsReferenceRepository()
        {
            return MouseOver(HeaderMenu.Reference, SubMenu.Repository);
        }

        public  bool IsReferenceManager()
        {
            return MouseOver(HeaderMenu.Manager, SubMenu.ReferenceManager);
        }

        public  bool IsSuspectProviders()
        {
            return MouseOver(HeaderMenu.Provider, SubMenu.SuspectProviders);
        }

        public  bool IsSuspectProvidersWorkList()
        {
            return MouseOver(HeaderMenu.Provider, SubMenu.SuspectProvidersWorkList);
        }

        public  bool IsParticularPage(string headerMenu,string subMenu)
        {

            return MouseOver(headerMenu, subMenu);
        }
        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// General MouseOver method
        /// </summary>
        /// <param name="headerMenuLocator">HeaderMenuLocator</param>
        /// <param name="subMenuLocator">SubMenuLocator</param>
        /// <returns>true or false</returns>
        private bool MouseOver(String headerMenuLocator, String subMenuLocator)
        {
            //get the element that shows menu with the mouseOver event
            var headerMenu =
                HeaderMenu.GetHeaderMenuLocator(headerMenuLocator);
            //SiteDriver mouseover event
            SiteDriver.MouseOver(headerMenu, How.XPath);
            //JavaScript mouseover event
            JavaScriptExecutor.ExecuteMouseOver(headerMenu, How.XPath);

            StringFormatter.PrintMessage("Mouse over : " + subMenuLocator);
            return SiteDriver.IsElementPresent(_subMenu.GetSubMenuLocator(subMenuLocator), How.XPath);
        }

        /// <summary>
        /// General MouseOver method
        /// </summary>
        /// <param name="headerMenuLocator">HeaderMenuLocator</param>
        /// <param name="subMenuLocator">SubMenuLocator</param>
        /// <returns>true or false</returns>
        public bool MouseOverForSubMenu(String headerMenuLocator, String subMenuLocator)
        {
            //get the element that shows menu with the mouseOver event
            var headerMenu =
                HeaderMenu.GetHeaderMenuLocator(headerMenuLocator);
            //SiteDriver mouseover event
            SiteDriver.MouseOver(headerMenu, How.XPath);
            //JavaScript mouseover event
            JavaScriptExecutor.ExecuteMouseOver(headerMenu, How.XPath);

            StringFormatter.PrintMessage("Mouse over : " + subMenuLocator);
            return SiteDriver.IsElementPresent(string.Format( SubMenu.UserSubMenuLocatorTemplate,subMenuLocator), How.XPath);
        }

        private void MouseOver(String headerMenuLocator)
        {
            //get the element that shows menu with the mouseOver event
            var headerMenu =
                HeaderMenu.GetHeaderMenuLocator(headerMenuLocator);
            //SiteDriver mouseover event
            SiteDriver.MouseOver(headerMenu, How.XPath);
            //JavaScript mouseover event
            JavaScriptExecutor.ExecuteMouseOver(headerMenu, How.XPath);
        }

        private void MouseOut(String headerMenuLocator)
        {
            //get the element that shows menu with the mouseOver event
            var headerMenu =
                HeaderMenu.GetHeaderMenuLocator(headerMenuLocator);
            
           
            //JavaScript mouseover event
            JavaScriptExecutor.ExecuteMouseOut(headerMenu, How.XPath);
        }

        /// <summary>
        /// General MouseOver method
        /// </summary>
        /// <param name="headerMenuLocator">HeaderMenuLocator</param>
        /// <param name="subMenuLocator">SubMenuLocator</param>
        /// <param name="userSubMenuEnum"></param>
        /// <returns>true or false</returns>
        public bool MouseOver(String headerMenuLocator, String subMenuLocator, string userSubMenuEnum)
        {
            if (MouseOverForSubMenu(headerMenuLocator, subMenuLocator))
            {
                //XPath of User sub menu
                string userSubMenu = string.Format(SubMenu.UserSubMenuLocatorTemplate, subMenuLocator);

                //SiteDriver mouseover event for User sub menu
                //SiteDriver.MouseOver(userSubMenu, How.XPath);

                //JavaScript mouseover event for USer sub menu
                JavaScriptExecutor.ExecuteMouseOver(userSubMenu, How.XPath);

                StringFormatter.PrintMessage("Mouse over : " + userSubMenuEnum);
                return SiteDriver.IsElementPresent(_subMenu.GetSubMenuLocator(userSubMenuEnum), How.XPath);
            }
            return false;
        }

        /// <summary>
        /// General MouseOver method
        /// </summary>
        /// <param name="headerMenuLocator">HeaderMenuLocator</param>
        /// <param name="subMenuLocator">SubMenuLocator</param>
        /// <param name="userSubMenuEnum"></param>
        /// <returns>true or false</returns>
        public bool MouseOver(String headerMenuLocator, string subMenuLocator, List<string> userSubMenuEnum)
        {
            if (MouseOverForSubMenu(headerMenuLocator, subMenuLocator))
            {
                //XPath of User sub menu
                string userSubMenu = string.Format(SubMenu.UserSubMenuLocatorTemplate, subMenuLocator);

                //SiteDriver mouseover event for User sub menu
                SiteDriver.MouseOver(userSubMenu, How.XPath);

                //JavaScript mouseover event for USer sub menu
                JavaScriptExecutor.ExecuteMouseOver(userSubMenu, How.XPath);

                StringFormatter.PrintMessage("Mouse over : " + userSubMenuEnum);
                var returnStatus = false;
                foreach (var subMenu in userSubMenuEnum)
                {
                    returnStatus=SiteDriver.IsElementPresent(_subMenu.GetSubMenuLocator(subMenu), How.XPath);
                    if (returnStatus == false)
                        return false;
                }

                return returnStatus;
            }
            return false;
        }



        /// <summary>
        /// General MouseOver method
        /// </summary>
        /// <param name="headerMenuLocator">HeaderMenuLocator</param>
        /// <param name="subMenuLocator">SubMenuLocator</param>
        /// <param name="userSubMenuEnum"></param>
        /// <returns>true or false</returns>
        public bool MouseOverToSubMenuOnly(String headerMenuLocator, String subMenuLocator)
        {
            if (MouseOverForSubMenu(headerMenuLocator, subMenuLocator))
            {
                //XPath of User sub menu
                string userSubMenu = string.Format(SubMenu.UserSubMenuLocatorTemplate, subMenuLocator);

                

                //JavaScript mouseover event for USer sub menu
                JavaScriptExecutor.ExecuteMouseOver(userSubMenu, How.XPath);

                StringFormatter.PrintMessage("Mouse over SubMenu: " + subMenuLocator);
            }
            return false;
        }

        public bool IsSecondarySubMenuOptionPresent(string menuName, string submenuName, string secondarySubMenu)
        {
            MouseOver(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVQCWorkList);
            var result = SiteDriver.IsElementPresent(string.Format(SubMenu.SecondarySubMenuXPathTemplate, secondarySubMenu),
                How.XPath);
            MouseOutClaimMenu();
            return result;
        }

        public List<string> GetSecondarySubMenuOptionList(string menuName, string submenuName)
        {
            MouseOverToSubMenuOnly(HeaderMenu.Claim, SubMenu.ClaimWorkList);
            SiteDriver.WaitToLoadNew(500);
            var result = JavaScriptExecutor.FindElements(string.Format(SubMenu.SecondarySubMenuListXPathTemplate, "Claim Work List"),
                How.XPath, "Text");
            MouseOutClaimMenu();
            return result;
        }
        #endregion
    }
}
