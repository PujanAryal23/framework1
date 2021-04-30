using System;
using System.Collections.Generic;
using System.Linq;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.Support.Menu
{
    public class SubMenu
    {


        #region Private VAriables
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;
        #endregion


        #region CONSTRUCTOR

        public SubMenu(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }

        #endregion


        #region SUBMENU

        public const string MyAppeals = "My Appeals";
        public const string AppealSearch = "Appeal Search";
        public const string AppealCreator = "Appeal Creator";
        public const string AppealManager = "Appeal Manager";
        public const string AppealRationaleManager = "Rationale Manager";
        public const string AppealCategoryManager = "Appeal Category Manager";
        public const string BatchSearch = "Batch Search";
        public const string CiuReferralSearch = "CIUReferral Search";
        public const string CategoryManagement = "Category Management";
        public const string ClaimSearch = "Claim Search";
        public const string NewClaimSearch = "Claim Search";
        public const string ClaimWorkList = "Claim Work List";
        public const string BillWorkList = "Bill Work List";
        public const string FciClaimsWorkList = "FCI Claims";
        public const string CVClaimsWorkList = "CV Claims";
        public const string DciClaimsWorkList = "DCA Claims";
        public const string CVCodersClaims = "CV Coder Claims";
        public const string FfpClaimsWorkList = "FFP Claims";
        public const string ClientSearch = "Client Search";
        public const string CrossWalk = "Crosswalk";
        public const string DentalBoard = "Dental Board";
        public const string GroupSearch = "Group Search";
        public const string InvoiceParameterSearch = "Invoice Parameter Search";
        public const string InvoiceSearch = "Invoice Search";
        public const string LogicSearch = "Logic Search";
        public const string MedicalBoard = "Medical Board";
        public const string AlertsList = "Alerts List";
        public const string ProviderSearch = "Provider Search";
        public const string ReportCenter = "Report Center";
        public const string User = "User";
        public const string MaintenanceNotices = "Maintenance Banners";
        public const string BillSearch = "Bill Search";
        public const string PciBillWorkList = "CV Bills";
        public const string CVQCWorkList = "CV QC Claims";
        public const string CVRnWorkList = "CV RN Claims";
        public const string QaManager = "Analyst Manager";
        public const string QaClaimSearch = "QA Claim Search";
        public const string QaAppealSearch = "QA Appeal Search";
        public const string MicrostrategyMaintenance = "MicroStrategy";
        public const string PreAuthCreator = "Pre-Auth Creator";
        public const string PreAuthSearch = "Pre-Auth Search";
       

        public const string SubMenuLocatorTemplate = "//div[@id='master_navigation']//ul/li/a[contains(text(),'{0}')] | //div[@id='master_navigation']//ul/li/span[contains(text(),'{0}')]";
        public const string SubMenuXPathTemplate = ".//div[@id='master_navigation']/ul/li[{0}]/ul/li[{1}]/a | .//div[@id='master_navigation']/ul/li[{0}]/ul/li[{1}]/span";

        public const string SecondarySubMenuXPathTemplate =
            "//ul[contains(@class,'main_sub_nav secondary')]/li/a[normalize-space()='{0}'] | //ul[contains(@class,'main_sub_nav secondary')]/li/span[normalize-space()='{0}']";
        public const string Repository = "Repository";
        public const string ReferenceManager = "Reference Manager";
        public const string SuspectProviders = "Suspect Providers";
        public const string SuspectProvidersWorkList = "Suspect Providers Work List";

        public const string SecondarySubMenuListXPathTemplate =
            "//li[header[normalize-space()='{0}']]/ul[contains(@class,'secondary')]/li";



        #endregion

        #region USER SUBMENU

        public const string MyProfile = "My Profile";
        public const string NewUserProfileSearch = "User Profile Search";
        public const string RoleManager = "Role Manager";
        public const string UserSubMenuLocatorTemplate = ".//ul/li/header[contains(normalize-space(),'{0}')]";
        public const string SubMenuByTextXPathTemplate = "//a[text()[contains(.,'{0}')]] | //span[text()[contains(.,'{0}')]]";
        public const string AllSubMenuListCssLocator = "ul.main_sub_nav>li>a ,ul.main_sub_nav>li>span";
        public const string SubMenuListByHeaderMenuXPathTemplate = "//header[text()='{0}']/../ul/li/a |//header[text()='{0}']/../ul/li/span";
        public const string AllSubMenuListByHeaderMenuXPathTemplate = "//header[text()='{0}']/../ul/li//a |//header[text()='{0}']/../ul/li//span";

        #endregion

        /// <summary>
        /// Get sub menu locator
        /// </summary>
        /// <param name="strSubMenu"></param>
        /// <returns>SubMenuLocatorTemplate</returns>
        public string GetSubMenuLocator(String strSubMenu)
        {
            return String.Format(SubMenuLocatorTemplate, strSubMenu);
        }

        public  string GetSubMenuOption(int menuposition, int submenuposition)
        {
            return SiteDriver.FindElement(String.Format(SubMenuXPathTemplate, menuposition, submenuposition), How.XPath).Text;
        }

        //public bool IsSecondarySubMenuOptionPresent(string menuName, string submenuName, string secondarySubMenu)
        //{
        //    Mouseover.MouseOver(HeaderMenu.Claim, ClaimWorkList, PciQaWorkList);
        //    var result = SiteDriver.IsElementPresent(string.Format(SecondarySubMenuXPathTemplate, secondarySubMenu),
        //        How.XPath);
        //    Mouseover.MouseOutClaimMenu();
        //    return result;
        //}

        //public  bool IsSecondarySubMenuOptionPresent(string menuName, string submenuName, string secondarySubMenu)
        //{
        //    Mouseover.MouseOver(HeaderMenu.Claim, ClaimWorkList, CVQCWorkList);
        //    var result = SiteDriver.IsElementPresent(string.Format(SecondarySubMenuXPathTemplate, secondarySubMenu),
        //        How.XPath);
        //    Mouseover.MouseOutClaimMenu();
        //    return result;
        //}

        //public  List<string> GetSecondarySubMenuOptionList(string menuName, string submenuName)
        //{
        //    Mouseover.MouseOverToSubMenuOnly(HeaderMenu.Claim, ClaimWorkList);
        //    SiteDriver.WaitToLoadNew(500);
        //    var result = SiteDriver.FindElements(string.Format(SecondarySubMenuListXPathTemplate,"Claim Work List"),
        //        How.XPath,"Text");
        //    Mouseover.MouseOutClaimMenu();
        //    return result;
        //}


        public  List<string> GetAllSubMenuList()
        {
            return SiteDriver.FindElementAndGetAttributeByAttributeName(AllSubMenuListCssLocator, How.CssSelector,
                "innerHTML");

        }

        public  List<string> GetAllSubMenuListByHeaderMenu(string headerName)
        {
            return JavaScriptExecutor.FindElements(String.Format(SubMenuListByHeaderMenuXPathTemplate, headerName), How.XPath, "Text");

        }

        public  List<string> GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(string headerName)
        {
            var list = JavaScriptExecutor.FindElements(String.Format(AllSubMenuListByHeaderMenuXPathTemplate, headerName),
                "Text", selector: How.XPath);
            list = list.Where(x => !String.IsNullOrEmpty(x)).ToList();
            return list;

        }
    }
}
