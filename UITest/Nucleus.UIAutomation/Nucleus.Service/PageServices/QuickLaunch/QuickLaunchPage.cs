using System;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using System.Collections.Generic;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.HelpCenter;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageObjects.Report;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.HelpCenter;
//using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.PageServices.Report;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Database;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Nucleus.Service.SqlScriptObjects.SwitchClient;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Pre_Auth;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.Service.PageServices.QuickLaunch
{
    public class QuickLaunchPage : NewDefaultPage
    {
        #region PUBLIC FIELDS

        public readonly QuickLaunchPageObjects _quickLaunchPage;
        private SideBarPanelSearch _sideBarPanelSearch;
        private GridViewSection _gridViewSection;
        private readonly NewDataHelper _dataHelper;


        #endregion

        #region CONSTRUCTOR

        public QuickLaunchPage(INewNavigator navigator, QuickLaunchPageObjects quickLaunchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, quickLaunchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _quickLaunchPage = (QuickLaunchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public NewDataHelper DataHelper
        {
            get { return _dataHelper; }
        }

        public bool IsGoToLinkPresent(string linkName)
        {
            return SiteDriver.IsElementPresent(Format(QuickLaunchPageObjects.GoToLinkXPathTemplate, linkName), How.XPath);
        }

        public T ClickOnGoToLinkPresent<T>(string linkName)
        {
             var target = typeof(T);
             Action action;
                 action = () => SiteDriver.FindElement(Format(QuickLaunchPageObjects.GoToLinkXPathTemplate, linkName), How.XPath).Click();
             
             if (typeof(MyProfilePage) == target)
             {
                 PageObject = Navigator.Navigate<MyProfilePageObjects>(action);
             }
             else if (typeof(HelpCenterPage) == target)
             {
                 PageObject = Navigator.Navigate<HelpCenterPageObjects>(action);
             }

             WaitForPageHeader();
             return (T)Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public bool IsAppealManagerSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.AppealManager), How.XPath);
        }

        public bool IsUserProfileSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.NewUserProfileSearch), How.XPath);
        }

        public bool IsPciCoderClaimsSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.CVCodersClaims), How.XPath);
        }
        public bool IsRepositorySubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.Repository), How.XPath);
        }
        public bool IsAlertsListPresent()
        {
            return SiteDriver.FindElement(QuickLaunchPageObjects.AlertsListId, How.Id).Displayed;
        }
        public bool IsLogicSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.LogicSearch), How.XPath);
        }


        public bool IsPasswordChangeAlertModalPresent()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.PasswordChangeAlertModalId, How.Id);
        }
        public void ClosePasswordChangeAlertModal()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.PasswordChangeAlertModalCloseButtonId, How.Id).Click();
        }

        public bool IsElementPresent(string select)
        {
            return SiteDriver.IsElementPresent(select, How.XPath);
        }

        
        public List<string> GetMainNavigationMenuTabList()
        {
            return JavaScriptExecutor.FindElements(QuickLaunchPageObjects.TopNavigationTabListCssSelector, How.CssSelector,"Text");            
        }
      
        public bool IsHelpIconPresent()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.HelpIconCssSelector, How.CssSelector);
        }


        public List<string> GetAllSubMenuListByHeaderMenu(string headerName)
        {
            return GetSubMenu.GetAllSubMenuListByHeaderMenu(headerName);
        }

        public ClaimActionPage ClickOnPendedWorkList()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.PendedWorkListCssSelector, How.CssSelector).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public void ClickOnCustomizeLink()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CustomizeLinkCssSelector, How.CssSelector).Click();
            SiteDriver.WaitToLoadNew(500);
        }

        public void ClickOnCloseCustomize()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CloseCustomizeButtonXpath, How.XPath).Click();
        }

        public bool IsDashboardPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.DashboardQuickLaunchSettingsXpath, How.XPath);
        }

        public bool IsClaimSearchPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.ClaimSearchQuickLaunchSettingsXpath, How.XPath);
        }

        public bool IsFlaggedClaimsPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.FlaggedClaimsQuickLaunchSettingsXpath, How.XPath);
        }

        public bool IsPendedClaimPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.PendedClaimsQuickLaunchSettingsXpath, How.XPath);
        }

        public bool IsUnreviewedClaimsPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.UnreviewedClaimsQuickLaunchSettingsXpath, How.XPath);
        }

        public bool IsUnreviewedFFPClaimsPresentInQuickLaunchSetting()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.UnreviewedFFPClaimsQuickLaunchSettingsXpath, How.XPath);
        }

        /// <summary>
        /// Click on Dashboard icon
        /// </summary>
        public void ClickOnDashboardIcon()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.DashboardIconCssSelector,How.CssSelector).Click();
            Console.WriteLine("Clicked on Dashboard icon.");
            

        }

        public string SwitchToNavigateRetroClaimSearchViaUrlAndGetTitle(string url)
        {
            BrowserOptions.SetBrowserOptions(_dataHelper.EnviromentVariables);
            SiteDriver.Open(BrowserOptions.ApplicationUrl + url);
            SiteDriver.WaitForPageToLoad();
            var title = SiteDriver.Title;
            SiteDriver.Back();
            return title;
        }


        public string ClickOnClickOnCIWIconAndGetFileName( )
        {
            ClickOnCIWIcon();
            ClickOkCancelOnConfirmationModal(true);
            var downloadPage = NavigateToChromeDownLoadPage();
            var fileName = downloadPage.GetFileName();

            SiteDriver.WaitForCondition(() =>
            {
                if (fileName == "")
                {
                    fileName = downloadPage.GetFileName();
                    return false;
                }
                else
                    return true;
            }, 5000);

            return fileName;
        }


        public bool IsInvoiceSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.InvoiceSearch), How.XPath);
        }

        public int GetLockedClaimCountByuser(string username)
        {
           return  (int) Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.LockedClaimCountByUser,username));
        }

        public int GetAppealLockCountByUser(string username)
        {
            return (int)Executor.GetSingleValue(string.Format(AppealSqlScriptObjects.AppealCountByUser, username));
        }

        public int GetLockedPreauthCountByUser(string username)
        {
            return (int)Executor.GetSingleValue(Format(PreAuthSQLScriptObjects.PreAuthLockCountByUser, username));
        }

        #region QuickLaunchTilesSmokeTests

        public bool IsClaimSearchTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.ClaimSearchQuickLaunchTileXpath, How.XPath);
        }

        public ClaimSearchPage ClickOnClaimSearchTileAndNavigateToClaimSearchPage()
        {

            var newClaimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.ClaimSearchQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector));

            });
            return new ClaimSearchPage(Navigator, newClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public bool IsLogicSearchTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.LogicSearchQuickLaunchTileXpath, How.XPath);
        }
        
        public LogicSearchPage ClickOnLogicSearchTileAndNavigateToLogicSearchPage()
        {

            var newLogicSearchPage = Navigator.Navigate<LogicSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.LogicSearchQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(LogicSearchPageObjects.SearchButtonCssLocator, How.CssSelector));

            });
            return new LogicSearchPage(Navigator, newLogicSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsInvoiceSearchTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.InvoiceSearchQuickLaunchTileXpath, How.XPath);
        }

        public InvoiceSearchPage ClickOnInvoiceSearchTileAndNavigateToLogicSearchPage()
        {

            var newInvoiceSearchPage = Navigator.Navigate<InvoiceSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.InvoiceSearchQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(InvoiceSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(InvoiceSearchPageObjects.FindButtonCssLocator, How.CssSelector));

            });
            return new InvoiceSearchPage(Navigator, newInvoiceSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsCotivitiFlaggedProviderTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.CotivitiFlaggedProvidersQuickLaunchTileXpath, How.XPath);
        }

        public ProviderSearchPage ClickOnCotivitiFlaggedProviderSearchTileAndNavigateToProviderSearchPage()
        {

            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.CotivitiFlaggedProvidersQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ProviderSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsClientFlaggedProviderTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.ClientFlaggedProvidersQuickLaunchTileXpath, How.XPath);
        }

        public ProviderSearchPage ClickOnClientFlaggedProviderSearchTileAndNavigateToProviderSearchPage()
        {

            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.ClientFlaggedProvidersQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ProviderSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public bool IsDashboardTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.DashboardQuickLaunchTileXpath, How.XPath);
        }
        
        public DashboardPage ClickOnDashboardTileAndNavigateDashboardPage()
        {

            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.DashboardQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(DashboardPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DashboardPageObjects.DashboardLabelCssSelector, How.CssSelector));
            });
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsProviderSearchTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.ProviderQuickLaunchTileXpath, How.XPath);
        }

        public ProviderSearchPage ClickOnProviderSearchTileAndNavigateToProviderSearchPage()
        {

            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.ProviderQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ProviderSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAppealCreatorTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.AppealCreatorQuickLaunchTileXpath, How.XPath);
        }

        public AppealCreatorPage ClickOnAppealCreatorTileAndNavigateToAppealCreatorPage()
        {

            var appealCreatorPage = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.AppealCreatorQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(AppealCreatorPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealCreatorPageObjects.FindClaimSectionCssLocator, How.CssSelector));
            });
            return new AppealCreatorPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAppealSearchTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.AppealSearchQuickLaunchTileXpath, How.XPath);
        }


        public AppealSearchPage ClickOnAppealSearchTileAndNavigateToAppealSearchPage()
        {
            var appealSearchPage = Navigator.Navigate<AppealSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.AppealSearchQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(AppealSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealSearchPageObjects.FindButtonCssLocator, How.CssSelector));
            });
            return new AppealSearchPage(Navigator, appealSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsMyProfileTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.MyProfileQuickLaunchTileXpath, How.XPath);
        }

        public MyProfilePage ClickOnMyProfileTileAndNavigateToMyProfilePage()
        {
            var myProfilePage = Navigator.Navigate<MyProfilePageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.MyProfileQuickLaunchTileXpath, How.XPath).Click();
                
                SiteDriver.WaitForCondition(() =>
                    JavaScriptExecutor.IsElementPresent(MyProfilePageObjects.SaveButtonCssSelector));
            });
            return new MyProfilePage(Navigator, myProfilePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsReportCenterPresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.ReportCenterQuickLaunchTileXpath, How.XPath);
        }

        public ReportCenterPage ClickOnReportCenterTileAndNavigateToReportCenterPage()
        {
            var reportCenterPage = Navigator.Navigate<ReportCenterPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.ReportCenterQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ReportCenterPageObjects.SpinnerCssLocator, How.CssSelector));
               
            });
            return new ReportCenterPage(Navigator, reportCenterPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsSuspectProviderTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.SuspectProviderQuickLaunchTileXpath, How.XPath);
        }

        public SuspectProvidersPage ClickOnSuspectProviderTileAndNavigateToSuspectProviderPage()
        {
            var suspectProvidersPage = Navigator.Navigate<SuspectProvidersPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.SuspectProviderQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(SuspectProvidersPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(SuspectProvidersPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new SuspectProvidersPage(Navigator, suspectProvidersPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsPendedWorkListTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.PendedWorkListQuickLaunchTileXpath, How.XPath);
        }

        public ClaimActionPage ClickOnPendedWorkListTileAndNavigateToClaimActionPage(bool handlePopup=false)
        {
            var claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.PendedWorkListQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, claimActionPage,SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }

        public bool IsUnreviewedWorkListTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.UnreviewedWorkListQuickLaunchTileXpath, How.XPath);
        }

        public ClaimActionPage ClickOnUnreviewedWorkListTileAndNavigateToClaimActionPage(bool handlepopup = false)
        {
            var claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.UnreviewedWorkListQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, claimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor,handlepopup);
        }

        public bool IsHelpCenterTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.HelpCenterQuickLaunchTileXpath, How.XPath);
        }

        public HelpCenterPage ClickOnHelpCenterTileAndNavigateToHelpCenterPage()
        {
            var helpCenterPage = Navigator.Navigate<HelpCenterPageObjects>(() =>
            {
                SiteDriver.FindElement(QuickLaunchPageObjects.HelpCenterQuickLaunchTileXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(HelpCenterPageObjects.SpinnerCssLocator, How.CssSelector));
      
            });
            return new HelpCenterPage(Navigator, helpCenterPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsCotivitiTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.CotivitiQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnCotivitiTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CotivitiQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsCMSDatabaseTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.CMSDatabaseQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnCMSDatabaseTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CMSDatabaseQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsCotivitiBlogTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.CotivitiBlogQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnCotivitiBlogTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CotivitiBlogQuickLaunchTileXpath, How.XPath).Click();
        }


        public bool IsKnowledgeBankTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.KnowledgeBankQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnKnowledgeBankTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.KnowledgeBankQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsQuestDxTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.QuestDxQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnQuestDxTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.QuestDxQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsEnclarityTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.EnclarityQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnEnclarityTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.EnclarityQuickLaunchTileXpath, How.XPath).Click();
        }

       

        public bool IsFDATilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.FDAQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnFDATile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.FDAQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsLabCorpTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.LabCorpQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnLabCorpTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.LabCorpQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsOIGExclusionsTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.OIGExclusionsQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnOIGExclusionsTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.OIGExclusionsQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsHealthGradesTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.HealthGradesQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnHealthGradesTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.HealthGradesQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsUCompareTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.UCompareQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnUCompareTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.UCompareQuickLaunchTileXpath, How.XPath).Click();
        }

        public bool IsCodeCorrectTilePresentInQuickLaunchPage()
        {
            return SiteDriver.IsElementPresent(QuickLaunchPageObjects.CodeCorrectQuickLaunchTileXpath, How.XPath);
        }

        public void ClickOnCodeCorrectTile()
        {
            SiteDriver.FindElement(QuickLaunchPageObjects.CodeCorrectQuickLaunchTileXpath, How.XPath).Click();
        }


        #endregion

        #region SwitchClient
        public List<List<string>> GetMostRecentClientDetailsFromDatabase(string userId)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(String.Format(SwitchClientSqlScriptObject.GetMostRecentClients, userId));

            if (list != null)
            {
                foreach (DataRow row in list)
                {
                    var t = row.ItemArray.Select(x => x.ToString()).ToList();
                    infoList.Add(t);
                }

                
            }

            return infoList;

        }

        public List<List<string>> GetAllClientDetailsFromDatabase(string userId)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(String.Format(SwitchClientSqlScriptObject.GetAllClients, userId));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList;

        }


        #endregion


    }

    #endregion

}
