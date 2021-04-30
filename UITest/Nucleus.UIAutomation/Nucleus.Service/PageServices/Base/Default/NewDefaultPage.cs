using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using Microsoft.SqlServer.Server;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Batch;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.HelpCenter;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageObjects.Report;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageObjects.SwitchClient;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.HelpCenter;
//using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Report;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.SwitchClient;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageServices.Claim;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.MicroStrategy;
using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageObjects.Reference;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.PageServices.Batch;
using UIAutomation.Framework.Core.Driver;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Microstrategy;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.PageServices.Reference;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Database;
using OpenQA.Selenium.Interactions;
using Nucleus.Service.PageObjects.Microstrategy;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using static System.Console;
using static System.String;


namespace Nucleus.Service.PageServices.Base.Default
{
    public class NewDefaultPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private readonly NewDefaultPageObjects _defaultPage;
        private SwitchClientPageObjects _switchClientPage;
        private SwitchClientPage _switchClientPageService;
        private readonly string _originalWindow;
        private readonly CommonSQLObjects _commonSqlObjects;
        private static Random random = new Random();
        private LoginPage _nucleusLoginPage;
        private readonly GridViewSection _getGridViewSection;
        private readonly SideBarPanelSearch _getSideBarPanelSearch;
        private ClientSearchPage _clientSearch;
        private UserProfileSearchPage _newUserProfileSearch;
        private readonly SideWindow _sideWindow;
        private readonly Mouseover Mouseover;
        private readonly SubMenu SubMenu;


        #endregion

        #region CONSTRUCTOR

        public NewDefaultPage(INewNavigator navigator, NewPageBase pageObject, ISiteDriver siteDriver,
            IJavaScriptExecutors javaScriptExecutor, IEnvironmentManager environmentManager,
            IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, pageObject, siteDriver, javaScriptExecutor, environmentManager, browserOptions, executor)
        {
            _defaultPage = (NewDefaultPageObjects) pageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _getGridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _getSideBarPanelSearch =
                new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _commonSqlObjects = new CommonSQLObjects(Executor);
            Mouseover = new Mouseover(SiteDriver, JavaScriptExecutor);
            SubMenu = new SubMenu(SiteDriver, JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch => _getSideBarPanelSearch;
        public GridViewSection GetGridViewSection => _getGridViewSection;

        public ChromeDownLoadPage ChromeDownLoadPage
        {
            get
            {
                var url = "Chrome://Downloads/";
                if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"], "edge", StringComparison.OrdinalIgnoreCase) == 0)
                    url = "edge://downloads/all";
                return new ChromeDownLoadPage(Navigator,
                    Navigator.Navigate<ChromeDownLoadPageObjects>(() => SiteDriver.Open(url)), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
        }

        public Mouseover GetMouseOver => Mouseover;

        public SubMenu GetSubMenu => SubMenu;

        public bool IsWindowOpenedAsTab()
        {
            return JavaScriptExecutor.IsWindowOpenedAsTab();

        }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public string ScreenshotFileName
        {
            get { return SiteDriver.GetScreenshotFilename(); }
        }

        public void CaptureScreenShot(string testName)
        {
            SiteDriver.CaptureScreenShot(testName);
        }

        #region UnAuthorizedPage

        public string GetUnAuthorizedMessage()
        {
            return SiteDriver.FindElement(NewDefaultPageObjects.UnAuthorizedMessageCssSelector, How.XPath)
                .Text;
        }

        public T ClickOnReturnToLastPage<T>()
        {
            var target = typeof(T);
            if (typeof(ClaimSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimSearchPageObjects>(ClickOnReturnToLastPageOnly);
            }

            if (typeof(AppealSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<AppealSearchPageObjects>(ClickOnReturnToLastPageOnly);
            }

            if (typeof(ClaimActionPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimActionPageObjects>(ClickOnReturnToLastPageOnly);
                WaitForWorkingAjaxMessage();
                return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
            }

            if (typeof(QuickLaunchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<QuickLaunchPageObjects>(ClickOnReturnToLastPageOnly);
            }

            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            WaitForStaticTime(1000);
            return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public T VisitAndReturnPageByUrlFoAuthorizedPage<T>(string url)
        {
            var target = typeof(T);
            if (typeof(ClaimSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimSearchPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
            }

            if (typeof(AppealSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<AppealSearchPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
            }

            if (typeof(AppealSummaryPage) == target)
            {
                PageObject =
                    Navigator.Navigate<AppealSummaryPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
            }

            if (typeof(AppealActionPage) == target)
            {
                PageObject =
                    Navigator.Navigate<AppealActionPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
                WaitForWorkingAjaxMessage();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForStaticTime(1000);
                return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor,true);
            }

            if (typeof(ClaimActionPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimActionPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
                WaitForWorkingAjaxMessage();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForStaticTime(1000);
                return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor,false);
            }

            if (typeof(QuickLaunchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<QuickLaunchPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));

            }

            if (typeof(ClaimHistoryPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimHistoryPageObjects>(VisitAndGetUrlByUrlForPopUpPage(url));
                return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }

            if (typeof(ProfileManagerPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ProfileManagerPageObjects>(VisitAndGetUrlByUrlForPopUpPage(url));
                return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }

            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            WaitForStaticTime(1000);
            return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string VisitAndGetUrlByUrlForUnAuthorizedPage(string url)
        {
            var applicationUrl = BrowserOptions.ApplicationUrl;
            SiteDriver.Open(applicationUrl + url);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.ReturnToTheLastPageLinkCssSelector, How.XPath));
            return SiteDriver.Url;
        }

        public string GetClientCookie()
        {
            var clientcookie = SiteDriver.GetCookies.GetCookieNamed("clientCookie").ToString().Split(';')[0];
            var client = clientcookie.Split('=')[1];
            return client;

        }



        public Action VisitAndGetUrlByUrlForPopUpPage(string url)
        {
            return () =>
            {
                var applicationUrl = BrowserOptions.ApplicationUrl;
                SiteDriver.Open(applicationUrl + url);

            };
        }

        public Action VisitAndGetUrlByUrlFoAuthorizedPage(string url)
        {
            return () =>
            {
                var applicationUrl = BrowserOptions.ApplicationUrl;
                if (url.Contains(applicationUrl))
                    SiteDriver.Open(url);
                else
                    SiteDriver.Open(applicationUrl + url);
                if (url.Contains("Retro") || url.Contains("retro"))
                {
                    SiteDriver.WaitForPageToLoad();
                }
                else
                {
                    WaitForWorkingAjaxMessage();
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                }
            };
        }

        public void ClickOnReturnToLastPageOnly()
        {
            JavaScriptExecutor.ExecuteClick(
                NewDefaultPageObjects.ReturnToTheLastPageLinkCssSelector, How.XPath);
        }

        public void ClickOnBrowserBackButton()
        {
            SiteDriver.WebDriver.Navigate().Back();
            WriteLine("Clicked on Browser's back button");
        }

        #endregion

        public bool IsDashboardIconPresent()
        {
            MouseOverOnUserMenu();
            var result = SiteDriver.IsElementPresent(NewDefaultPageObjects.DashboardIconXPath, How.XPath);
            ClickOnPageFooter();
            return result;
        }

        public QuickLaunchPage SwitchClientTo(ClientEnum clientName, bool isPopup = false, bool typeAhead = false)
        {
            WriteLine("Switching client to " + clientName.ToString());

            if (!SiteDriver.IsElementPresent(NewDefaultPageObjects.SwitchClientListCssLocator, How.CssSelector))
                ClickOnSwitchClient();

            var switchClient = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                if (typeAhead)
                    TypeAheadInSwitchClientPage(clientName);

                SiteDriver.FindElement(Format(NewDefaultPageObjects.SwitchClientListValueXPathTemplate,
                    clientName.ToString()), How.XPath).Click();

                if (isPopup)
                {
                    ClickOkCancelOnConfirmationModal(true);
                }

                SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(NewDefaultPageObjects.WorkingAjaxMessageCssLocator,
                            How.CssSelector),
                    5000);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(NewDefaultPageObjects.LandingImageCssLocator, How.CssSelector),
                    10000);


                SiteDriver.WaitForPageToLoad();

            });
            try
            {
                ClickOnPageFooter();
            }
            catch (Exception e)
            {
                WriteLine("Page Header is overlapped by the menu dropdown \n" + e.Message);
            }

            return new QuickLaunchPage(Navigator, switchClient, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public string GetPageHeader()
        {
            if (IsPageHeaderPresent())
                return SiteDriver.FindElement(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
            if (CurrentPageUrl.EndsWith(PageUrlEnum.QuickLaunch.GetStringValue()) ||
                CurrentPageUrl.EndsWith(PageUrlEnum.QuickLaunch.GetStringValue() + "/"))
                return PageHeaderEnum.QuickLaunch.GetStringValue();
            return SiteDriver.FindElement(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public bool IsPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector);
        }

        public void WaitForPageHeader()
        {
            SiteDriver.WaitForCondition(IsPageHeaderPresent);
        }

        public bool IsNucleusHeaderPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.NucleusHeaderCssLocator, How.CssSelector);
        }

        public bool IsClientNucleusHeaderPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.ClientNucleusHeaderCssLocator, How.CssSelector);
        }

        public string GetCurrentUrl()
        {
            return SiteDriver.Url;
        }

        public void WaitForFileExists(string fileLocation)
        {
            SiteDriver.WaitForCondition(() => File.Exists(fileLocation), 10000);
        }

        public T ClickBrowserBackButton<T>(string url = null)
        {
            var target = typeof(T);
            Action action;

            if (url != null)
                action = () => SiteDriver.Open(url);
            else
                action = SiteDriver.Back;
            if (typeof(ProviderSearchPage) == target)
            {

                PageObject = Navigator.Navigate<ProviderSearchPageObjects>(action);
            }
            //else
            //if (typeof(ClaimsDetailPage) == target)
            //{
            //    PageObject = Navigator.Navigate<ClaimsDetailPageObjects>(action);
            //}
            //else if (typeof(AppealsDetailPage) == target)
            //{
            //    PageObject = Navigator.Navigate<AppealsDetailPageObjects>(action);
            //}
            //else if (typeof(DashboardLogicRequestsDetailsPage) == target)
            //{
            //    PageObject = Navigator.Navigate<DashboardLogicRequestsDetailPageObjects>(action);
            //}
            else if (typeof(QuickLaunchPage) == target)
            {
                PageObject = Navigator.Navigate<QuickLaunchPageObjects>(action);
            }
            //else if (typeof(InvoiceSearchPage) == target)
            //{
            //    PageObject = Navigator.Navigate<InvoiceSearchPageObjects>(action);
            //}

            return (T) Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public bool IsRepositoryTabPresent()
        {
            return SiteDriver.IsElementPresent(RepositoryPageObjects.referenceLabelXpath, How.XPath);
        }

        public bool IsLogicSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(SubMenu.GetSubMenuLocator(SubMenu.LogicSearch), How.XPath);
        }
        public bool IsReferenceManagerSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.ReferenceManager), How.XPath);
        }
        public void WaitToLoadPageErrorPopupModal(int waitTime = 500)
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, waitTime);
        }

        public bool IsPageErrorPopupModalPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id), 500);
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id, 200);
        }

        public string GetPageErrorMessage()
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 1500);
            return SiteDriver.FindElement(NewDefaultPageObjects.PageErrorMessageId, How.Id).Text;
        }

        public void ClosePageError()
        {
            JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.PageErrorCloseId, How.Id);
            WriteLine("Closed the modal popup");
        }

        public void ClickOkCancelOnConfirmationModal(bool confirmation,int time=10000)
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 500);
            if (!SiteDriver.IsElementPresent(NewDefaultPageObjects.OkConfirmationCssSelector, How.CssSelector)
            ) return;
            if (confirmation)
            {
                JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
                WaitForWorking(time);
                WriteLine("Ok Button is Clicked");

            }
            else
            {
                JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.CancelConfirmationCssSelector, How.CssSelector);
                WaitForWorking(time);
                WriteLine("Cancel Button is Clicked");

            }

        }

        public bool IsOkButtonPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
        }

        public bool IsCancelLinkPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.CancelConfirmationCssSelector, How.CssSelector);
        }


        public void RefreshPage(bool waitSidebar = true, int milliseconds = 0, bool isRetroPage = false)
        {
            SiteDriver.Refresh();
            if (SiteDriver.IsAlertBoxPresent())
                SiteDriver.AcceptAlertBox();
            SiteDriver.WaitForCondition(
                () => !SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector),
                milliseconds);
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector),
                milliseconds);
            if (!isRetroPage)
                SiteDriver.WaitForCondition(
                    () => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector),
                    milliseconds);
            if (waitSidebar)
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector),
                    milliseconds);
        }

        public void WaitForWorking(int time=0)
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent(),time);
            SiteDriver.WaitToLoadNew(300);
        }

        public bool IsLoadMoreLinkable()
        {
            return GetGridViewSection.IsLoadMorePresent();
        }

        public void WaitForPageToLoad()
        {
            SiteDriver.WaitForPageToLoad();
            JavaScriptExecutor.WaitForJqueryStatusCondition();
        }

        public string GetTitle()
        {
            return SiteDriver.Title;
        }

        public string GetPopupPageTitle()
        {
            return SiteDriver.FindElement(NewDefaultPageObjects.PopupPageTitleCssLocator, How.CssSelector).Text;
        }

        #region SWITCH CLIENT

        public bool IsDefaultTestClient(ClientEnum clientName)
        {
            return !SiteDriver.IsElementPresent(NewDefaultPageObjects.ClientSmallBrandingImgId, How.Id);

        }

        public bool IsCurrentPageTypeRetro(string url = null)
        {
            var checkUrl = !String.IsNullOrEmpty(url) ? url : CurrentPageUrl;
            return checkUrl.ToLower().Contains(PageHeaderEnum.RetroPage.GetStringValue());
        }

        public bool IsCurrentClientAsExpected(ClientEnum clientName)
        {
            if (IsCurrentPageTypeRetro(CurrentPageUrl))
            {
                return CheckIfCurrentClientIsExpectedFromClaimUrlForRetroPages(
                    EnvironmentManager.TestClient);
            }

            return IsDefaultTestClientForEmberPage(
                EnvironmentManager.TestClient);

        }

        public bool CheckIfCurrentClientIsExpectedFromClaimUrlForRetroPages(ClientEnum clientName)
        {
            return SiteDriver.IsElementPresent(
                string.Format(NewDefaultPageObjects.CheckCurrentClientFromClaimUrl, clientName), How.CssSelector);
        }

        public bool IsDefaultTestClientForEmberPage(ClientEnum clientName)
        {

            var client = SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector)
                             .GetAttribute("value") ??
                         SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector)
                             .Text;
            return client.Equals(clientName.GetStringValue());

        }

        public string GetCurrentClient()
        {
            var client = SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector)
                             .GetAttribute("value") ??
                         SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector)
                             .Text;
            return client;
        }


        public bool IsClientLogoPresentForRetroPage(ClientEnum clientName, bool wait = false)
        {
            if (wait)
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(NewDefaultPageObjects.ClientSmallBrandingImgId, How.Id), 5000);
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.ClientSmallBrandingImgId, How.Id);

        }



        public bool IsClientLogoPresent(ClientEnum clientName, bool mstr = false)
        {
            bool client =
                SiteDriver.IsElementPresent(NewDefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);
            if (mstr)
                return client;
            if (client)
            {
                var element =
                    SiteDriver.FindElement(NewDefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);
                return element.GetAttribute("Alt").Equals(clientName.ToString());
            }

            return false;
        }

        public string GetClientLogoFromDB(string clientcode)
        {
            return Executor.GetFileBlobValue(String.Format(CommonSQLObjects.GetCLientLogoFromDB, clientcode));
        }


        public string GetClientCode() => SiteDriver
            .FindElement(NewDefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector)
            .GetAttribute("alt");

        public NewDefaultPage ClickOnSwitchClient()
        {
            SiteDriver.WaitToLoadNew(3000);
            var switchClient = Navigator.Navigate<NewDefaultPageObjects>(() =>
                SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector).Click());
            SiteDriver.WaitToLoadNew(2000);
            return new NewDefaultPage(Navigator, switchClient, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }


        public List<string> GetSwitchClientList()
        {
            return JavaScriptExecutor.FindElements(NewDefaultPageObjects.SwitchClientListCssLocator, How.CssSelector,
                "Text");
        }

        public bool IsMostRecentPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.MostRecentHeaderXpath, How.XPath);
        }

        public bool IsAllClientsPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.AllClientsHeaderXpath, How.XPath);
        }

        public List<string> GetClientCodesOrClientNames(bool mostRecentClients, bool getClientNames)
        {
            var headerName = mostRecentClients ? "Most Recent" : "All Clients";
            var clientCodeOrName = getClientNames ? 2 : 1;

            var list = JavaScriptExecutor.FindElements(
                Format(NewDefaultPageObjects.MostRecentClientsCodesCss, headerName, clientCodeOrName),
                "Text");
            return list;
        }
        public void Reload()
        {
            SiteDriver.Reload();
        }

        public void TypeAheadInSwitchClientPage(ClientEnum clientName)
        {
            var element = SiteDriver.FindElement(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(clientName.GetStringValue());
        }

        public bool IsSwitchClientIconPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.SwitchClientCssSelector, How.CssSelector);
        }

        public bool IsSwitchClientCaretSignPresent() =>
            SiteDriver.IsElementPresent(NewDefaultPageObjects.SwitchClientCaretIconCssSelector, How.CssSelector);

        public bool IsValueCurrency(string value)
        {
            return value.Equals($"{value:C}");
        }


        public bool IsCorrectClientLogoDisplayed(ClientEnum clientName)
        {
            var logovalue = GetClientLogoFromDB(clientName.ToString());
            var client = SiteDriver.FindElement(NewDefaultPageObjects.ClientSmallBrandingImgId, How.Id);
            return client.GetAttribute("src").Contains(logovalue);

        }

        public bool IsCorrectClientLogoDisplayedForEmberPgae(ClientEnum clientName)
        {

            var logovalue = GetClientLogoFromDB(clientName.ToString());
            var client =
                SiteDriver.FindElement(NewDefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);

            return client.GetAttribute("src").Contains(logovalue) &&
                   client.GetAttribute("Alt").Equals(clientName.ToString());
        }




        #endregion

        #region LOGOUT

        public LoginPage Logout()
        {
            EnvironmentManager.UserFullName = null;
            if (IsPageErrorPopupModalPresent())
                ClosePageError();
            MouseOverOnUserMenu();
            WaitForStaticTime(2000);
            if (CurrentPageUrl.Contains(PageUrlEnum.Microstrategy.GetStringValue()))
                JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.LogOutButtonXPath, How.XPath);
            else
            {
                var element = SiteDriver.FindElement(NewDefaultPageObjects.LogOutButtonXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }
            AcceptAlertBoxIfPresent();
            WriteLine("Clicked Logout Button.");
            // return new LoginPage(Navigator, new LoginPageObjects());
            return new LoginPage(Navigator, new LoginPageObjects(), SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Checks if Alert is present
        /// </summary>
        /// <returns></returns>
        public bool IsAlertBoxPresent()
        {
            return Navigator.IsAlertBoxPresent();
        }

        /// <summary>
        /// Accepts the alert
        /// </summary>
        public void AcceptAlertBoxIfPresent()
        {
            if (IsAlertBoxPresent())
            {
                Navigator.AcceptAlertBox();
                WriteLine("Alert Box Accepted");
            }
            else
            {
                WriteLine("Alert Box wasn't found.");
            }
        }

        public bool IsLogoutIconPresent()
        {

            return SiteDriver.IsElementPresent(NewDefaultPageObjects.LogOutButtonXPath, How.XPath);

        }

        public void ClickOnPageFooter()
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.NucleusFooterId, How.Id),
                5000);
            var element = SiteDriver.FindElement(NewDefaultPageObjects.NucleusFooterId, How.Id);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            JavaScriptExecutor.ExecuteMouseOver("footer", How.Id);
        }

        public void ClickOnLogo()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(NewDefaultPageObjects.NucleusLogoId, How.Id),
                5000);
            var element = SiteDriver.FindElement(NewDefaultPageObjects.NucleusLogoId, How.Id);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.LandingImageCssLocator, How.CssSelector),
                10000);
        }

        public void MouseOverOnUserMenu()
        {
            SiteDriver.MouseOver(NewDefaultPageObjects.UserMenuId, How.Id);
        }

        public void MouseOverManagerMenu()
        {
            SiteDriver.MouseOver(NewDefaultPageObjects.ManagerTab, How.CssSelector);
        }

        public void MouseOverOnQuickLinks()
        {
            SiteDriver.MouseOver(NewDefaultPageObjects.QuickLinksXPath, How.XPath, release: true);
        }

        public void MouseOverOnCotivitiLinks()
        {
            SiteDriver.MouseOver(NewDefaultPageObjects.CotivitiLinksCssSelector, How.CssSelector, release: true);
        }

        #endregion

        #region HELPCENTER

        public HelpCenterPage NavigateToHelpCenter(bool popUp = false)
        {
            MouseOverOnUserMenu();
            WaitForStaticTime(2000);
            //_defaultPage.HelpCenterButton.Click();
            SiteDriver.FindElement(NewDefaultPageObjects.HelpCenterXPath, How.XPath).Click();

            if (popUp)
                ClickOkCancelOnConfirmationModal(true);

            ClickOnPageFooter();
            return new HelpCenterPage(Navigator, new HelpCenterPageObjects(), SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }



        public bool IsHelpCenterIconPresent()
        {
            MouseOverOnUserMenu();

            var isHelpCenterPresent = SiteDriver.IsElementPresent(NewDefaultPageObjects.HelpCenterXPath, How.XPath);
            ClickOnPageFooter();
            return isHelpCenterPresent;
        }

        #endregion



        public QuickLaunchPage SwitchTabAndOpenQuickLaunchPageCloseFirstTab(Func<QuickLaunchPage> Login)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() => { Login(); });
            return new QuickLaunchPage(Navigator, quickLaunch, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public QuickLaunchPage ClickOnQuickLaunch(bool popUp = false)
        {
            //MouseOverOnUserMenu();


            //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(NewDefaultPageObjects.QuickLaunchButtonXPath, How.XPath));
            var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                ClickOnLogo();
                if (popUp)
                {
                    Navigator.AcceptAlertBox();
                }
            });
            ClickOnPageFooter();
            return new QuickLaunchPage(Navigator, quickLaunch, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public bool IsQuickLaunchIconPresent()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.QuickLaunchButtonXPath, How.XPath);
        }

        public string GetSubMenuOption(int menuposition, int submenuposition)
        {
            return SubMenu.GetSubMenuOption(menuposition, submenuposition);
        }

        public string GetHeaderMenuText(int headerMenuPosition)
        {
            return SiteDriver.FindElement(
                    string.Format(QuickLaunchPageObjects.HeaderMenuXpathLocatorTemplate, headerMenuPosition), How.XPath)
                .Text;
        }


        #endregion

        #region NAVIGATION THROUGH HEADER MENU

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Claim => Claim Search
        /// </summary>
        /// <returns>Claim Search</returns>
        /// 
        public ClaimSearchPage NavigateToClaimSearch(bool fromClaimAction = false)
        {
            var claimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                Mouseover.MouseOverClaimMenu();
                ClickOnNormalizedMenu(SubMenu.ClaimSearch);
                if (IsPageErrorPopupModalPresent())
                    ClickOkCancelOnConfirmationModal(true);

                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                if (fromClaimAction == false)
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));

            });

            return new ClaimSearchPage(Navigator, claimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        /// <summary>
        /// Claim => Logic Search
        /// </summary>
        /// <returns>logic Search</returns>



        public ClaimSearchPage NavigateToBillSearch()
        {
            var claimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                if (Mouseover.IsBillSearch())
                    ClickOnMenu(SubMenu.BillSearch);
            });

            return new ClaimSearchPage(Navigator, claimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        /// <summary>
        /// Claim => FCI Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToFciClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsFciClaimsWorkList())
                    ClickOnMenu(SubMenu.FciClaimsWorkList);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }

        /// <summary>
        /// Claim => PCI Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToCVClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsCVClaimsWorkList())
                    ClickOnMenu(SubMenu.CVClaimsWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForIe(7000);

            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }


        /// <summary>
        /// Claim => DCA Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToDciClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
                {
                    if(Mouseover.IsDciClaimsWorkList())
                        ClickOnMenu(SubMenu.DciClaimsWorkList);
                    SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                    SiteDriver.WaitForCondition(() =>
                        !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                }

            );
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }

        /// <summary>
        /// Claim => FFP Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToFfpClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
                {
                    if(Mouseover.IsFFPClaimsWorkList())
                        ClickOnMenu(SubMenu.FfpClaimsWorkList);
                    SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                    SiteDriver.WaitForCondition(() =>
                        !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                }

            );
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }


        public ClaimActionPage NavigateToPciClaimsPatientHistoryPage()
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsCVClaimsWorkList())
                    ClickOnMenu(SubMenu.CVClaimsWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForIe(7000);

            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Claim => PCI Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToCvQcWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsCVQCClaims())
                    ClickOnMenu(SubMenu.CVQCWorkList);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }

        /// <summary>
        /// Claim => PCI RN Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToCVRnWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsCVRnWorkList())
                    ClickOnMenu(SubMenu.CVRnWorkList);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }

        public BillActionPage NavigateToPciBillWorkList()
        {
            var billActionPage = Navigator.Navigate<BillActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsPciBillWorkList())
                    ClickOnMenu(SubMenu.PciBillWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));

            });
            return new BillActionPage(Navigator, billActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public RepositoryPage NavigateToRepository()
        {
            var repositoryPage = Navigator.Navigate<RepositoryPageObjects>(() =>
            {
                Mouseover.IsReferenceRepository();
                ClickOnMenu(SubMenu.Repository);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

            });

            return new RepositoryPage(Navigator, repositoryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor
            );
        }

        /// <summary>
        /// References => ReferenceManager
        /// </summary>
        /// <returns>Reference Manager</returns>
        public ReferenceManagerPage NavigateToReferenceManager()
        {
            var referenceManagerPage = Navigator.Navigate<ReferenceManagerPageObjects>(() =>
            {
                Mouseover.IsReferenceManager();
                ClickOnMenu(SubMenu.ReferenceManager);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new ReferenceManagerPage(Navigator, referenceManagerPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }


        /// <summary>
        /// Invoice => New Invoice Search
        /// </summary>
        /// <returns>New Invoice Search page</returns>
        public InvoiceSearchPage NavigateToInvoiceSearch()
        {
            var newInvoiceSearchPage = Navigator.Navigate<InvoiceSearchPageObjects>(() =>
            {
                if (Mouseover.IsNewInvoiceSearch())
                    ClickOnMenu(SubMenu.InvoiceSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new InvoiceSearchPage(Navigator, newInvoiceSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }


        ///<summary>
        /// Appeal =>My Appeal 
        /// </summary>
        /// <returns>New Appeal Action</returns>
        public AppealActionPage NavigateToMyAppeals(bool handlePopup = true)
        {
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                if (Mouseover.IsMyAppeals())
                    ClickOnMenu(SubMenu.MyAppeals);
                WaitForWorkingAjaxMessage();
                SiteDriver.WaitForCondition(IsPageHeaderPresent);
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
            });

            return new AppealActionPage(Navigator, newAppealActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }




        /// <summary>
        /// Settings => User => MyProfile
        /// </summary>
        /// <returns>Client Search</returns>
        public ProfileManagerPage NavigateToProfileManager()
        {
            var profileManagerPage = Navigator.Navigate<ProfileManagerPageObjects>(() =>
            {
                if (Mouseover.IsSettingsUserMyProfile())
                {
                    ClickOnMenu(SubMenu.MyProfile);
                }
            });
            return new ProfileManagerPage(Navigator, profileManagerPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Settings => User => MyProfile
        /// </summary>
        /// <returns>Client Search</returns>
        public MyProfilePage NavigateToMyProfilePageFromMenu()
        {
            var profileManagerPage = Navigator.Navigate<MyProfilePageObjects>(() =>
            {
                if (Mouseover.IsSettingsUserMyProfile())
                {
                    ClickOnMenu(SubMenu.MyProfile);
                }

                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new MyProfilePage(Navigator, profileManagerPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        /// <summary>
        /// Appeal => (new) Appeal Rationale Manager
        /// </summary>
        /// <returns>New Appeal Rationale Manager</returns>
        public AppealRationaleManagerPage NavigateToAppealRationaleManager()
        {
            var newAppealRationaleManagerPage = Navigator.Navigate<AppealRationaleManagerPageObjects>(() =>
            {
                if (Mouseover.IsAppealRationalemanager())
                    ClickOnMenu(SubMenu.AppealRationaleManager);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

            });

            return new AppealRationaleManagerPage(Navigator, newAppealRationaleManagerPage, SiteDriver,
                JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public MyProfilePage NavigateToMyProfilePage()
        {
            var myProfilePage = Navigator.Navigate<MyProfilePageObjects>(() =>
            {
                MouseOverOnUserMenu();
                var element = SiteDriver.FindElement(NewDefaultPageObjects.MyProfileIconXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForStaticTime(200);
                SiteDriver.WaitForPageToLoad();
                ClickOnPageFooter();

                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

            });

            return new MyProfilePage(Navigator, myProfilePage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        /// <summary>
        /// Appeal => Appeal Category Manager
        /// </summary>
        /// <returns>Appeal Category Manager</returns>
        public AppealCategoryManagerPage NavigateToAppealCategoryManager()
        {
            var appealCategoryManagerPage = Navigator.Navigate<AppealCategoryManagerPageObjects>(() =>
            {
                if (Mouseover.IsAppealCategory())
                    ClickOnMenu(SubMenu.AppealCategoryManager);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.PageInsideTitleCssLocator,
                        How.CssSelector));
            });

            return new AppealCategoryManagerPage(Navigator, appealCategoryManagerPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Appeal => Appeal Category Manager
        /// </summary>
        /// <returns>Appeal Category Manager</returns>
        public MaintenanceNoticesPage NavigateToMaintenanceNotices()
        {
            var maintenanceNoticesPage = Navigator.Navigate<MaintenanceNoticesPageObjects>(() =>
            {
                if (Mouseover.IsSettingsMaintenanceNotices())
                    ClickOnMenu(SubMenu.MaintenanceNotices);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.PageInsideTitleCssLocator,
                        How.CssSelector));
            });

            return new MaintenanceNoticesPage(Navigator, maintenanceNoticesPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        ///<summary>
        /// Appeal => Appeal creator
        /// </summary>
        /// <returns>Appeal creator</returns>
        public AppealCreatorPage NavigateToAppealCreator()
        {
            var appealCreatorPage = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                if (Mouseover.IsAppealCreator())
                    ClickOnMenu(SubMenu.AppealCreator);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            });

            return new AppealCreatorPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        ///<summary>
        /// New Appeal Search
        /// </summary>
        /// <returns>New Appeal Search</returns>
        public AppealSearchPage NavigateToAppealSearch()
        {
            var newappealSearchPage = Navigator.Navigate<AppealSearchPageObjects>(() =>
            {
                if (Mouseover.IsAppealSearch())
                    ClickOnMenu(SubMenu.AppealSearch);
            });
            JavaScriptExecutor.WaitForJqueryStatusCondition();
            WaitForPageToLoadWithSideBarPanel();
            return new AppealSearchPage(Navigator, newappealSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }



        public DashboardPage NavigateToDashboard()
        {
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnDashboardIcon);
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            if (IsPageErrorPopupModalPresent())
            {
                CaptureScreenShot("Dashbord_issue_popup");
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }

            CaptureScreenShot("Dashbord_page_load_after");
            if (!SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector))
                return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                    BrowserOptions, Executor);
            WriteLine("Dashboard does not load sucessfully");
            RefreshPage(false);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            CaptureScreenShot("Dashbord_page_load_after1");
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public DashboardPage NavigateToFFPDashboard()
        {
            //TruncateDashboardClaimFFPTable();
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnFFPDashboard);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public DashboardPage NavigateToCVDashboard()
        {
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnCVDashboard);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public DashboardPage NavigateToCOBDashboard()
        {

            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnCOBDashboard);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public COBAppealsDetailPage NavigateToCobAppealsDetailPage()
        {
            NavigateToCOBDashboard();
            var cobAppealsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate,
                    DashboardOverviewTitlesEnum.AppealsOverview.GetStringValue())).Click();
                WriteLine("Clicked Appeals Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.COBAppealsDetail.GetStringValue()));
            }, () => new COBAppealsDetailPageObjects(PageUrlEnum.COBAppealsDetail.GetStringValue()));
            WaitForStaticTime(5000);
            return new COBAppealsDetailPage(Navigator, cobAppealsDetailPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor
            );
        }

        public COBClaimsDetailPage NavigateToCobClaimsDetailPage()
        {
            NavigateToCOBDashboard();
            var cobClaimsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate,
                    DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue())).Click();
                WriteLine("Clicked Claims Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue()));
            }, () => new COBClaimsDetailpageObject(PageUrlEnum.COBClaimsDetail.GetStringValue()));
            WaitForStaticTime(5000);
            return new COBClaimsDetailPage(Navigator, cobClaimsDetailPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor
            );

        }

        private void ClickOnCOBDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }


            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.COBProductCssLocator);
            WriteLine("Clicked on CV Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.FindElement(DashboardPageObjects.COBProductCssLocator).Click();
        }

        private void ClickOnFFPDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }


            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.FFPProductCssLocator);
            WriteLine("Clicked on CV Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.FindElement(DashboardPageObjects.FFPProductCssLocator).Click();
        }

        private void ClickOnCVDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.PCIProductCssLocator);
            WriteLine("Clicked on CV Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
        }


        public DashboardPage NavigateToMyDashboard()
        {
            //TruncateDashboardClaimFFPTable();
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnMyDashboard);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        private void ClickOnMyDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                CaptureScreenShot("My_dashboard_issue_before");
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MyDashboardCssLocator);
            WriteLine("Clicked on My Dashboard Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                JavaScriptExecutor.IsElementPresent(
                    string.Format(DashboardPageObjects.MyDashboardWidgetCssLocatorTemplate, "Claim Performance")));
        }

        private void ClickOnMicrostrategy()
        {
            JavaScriptExecutor.ExecuteClick(DashboardPageObjects.DashboardIconByCss, How.CssSelector);
            WriteLine("Clicked on Dashboard icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MicrostrategyDashboardCssLocator);
            SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
            SiteDriver.SwitchBackToMainFrame();
            WriteLine("Clicked on Microstrategy Filter.");
        }

        private void clickonMicrostrategyToViewListOfReports()
        {

            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.ExecuteClick(DashboardPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
            WriteLine("Clicked on Filter Dashboard icon.");
            SiteDriver.WaitForCondition(() =>
                JavaScriptExecutor.IsElementPresent(DashboardPageObjects.MicrostrategyDashboardCssLocator));
            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MicrostrategyDashboardCssLocator);

        }

        public void TruncateDashboardClaimFFPTable()
        {
            Executor.ExecuteQuery(DashboardSqlScriptObjects.TruncateDashboardClaimFFP);
            WriteLine("Truncate Dashboard table to get fresh data in page load");
        }

        ///<summary>
        /// Appeal => Appeal Manager
        /// </summary>
        /// <returns>Appeal Manager</returns>
        public AppealManagerPage NavigateToAppealManager()
        {
            var appealCreatorPage = Navigator.Navigate<AppealManagerPageObjects>(() =>
            {
                if (Mouseover.IsAppealManager())
                    ClickOnMenu(SubMenu.AppealManager);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealManagerPageObjects.PageHeaderCssLocator, How.CssSelector));
            });

            return new AppealManagerPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }



        public AppealSummaryPage SwitchTabAndOpenAppealSummaryByUrlInClientUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsClientUser();
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.Open(url));

            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public AppealSummaryPage SwitchTabAndOpenAppealSummaryByUrlInCotivitiUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsHciAdminUser();
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.Open(url));

            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public AppealActionPage SwitchTabAndOpenAppealActionByUrl(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsHciAdminUser();
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl(url));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));

            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public AppealSummaryPage SwitchOriginalAppealSummaryCotivitiUserTab()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count == 2)
            {
                if (CurrentPageUrl.Contains("appeal_summary"))
                {
                    SiteDriver.SwitchForwardTab();
                    SiteDriver.SwitchWindow(handles[1]);

                }

                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(handles[0]);
            }

            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.WaitForCondition(
                () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue())));
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public ClaimActionPage SwitchTabAndOpenNewClaimActionByUrlByClaimViewRestrictedUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsClaimViewRestrictionUser();
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            WaitForWorkingAjaxMessage();
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public ProfileManagerPage SwitchTabAndNavigateToProfileManager(string currenthandle, bool closeCurrent = false)
        {
            var handles = SiteDriver.WindowHandles.ToList();

            if (handles.Count == 2)
            {

                var toSwitch = handles.IndexOf(currenthandle) == 0 ? handles[1] : handles[0];
                if (closeCurrent)
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(toSwitch);
            }

            return new ProfileManagerPage(Navigator, new ProfileManagerPageObjects(), SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// QA => QA Manager
        /// </summary>
        /// <returns>QA Manager</returns>
        public QaManagerPage NavigateToAnalystManager()
        {
            var qaManagerPage = Navigator.Navigate<QaManagerPageObjects>(() =>
            {
                if (Mouseover.IsAnalystManager())
                    ClickOnMenu(SubMenu.QaManager);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(QaManagerPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaManagerPage(Navigator, qaManagerPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }


        /// <summary>
        /// QA => QA Claim search
        /// </summary>
        /// <returns>QA Claim search</returns>
        public QaClaimSearchPage NavigateToQaClaimSearch()
        {
            var qaClaimSearchPage = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsQAClaimSearch())
                    ClickOnMenu(SubMenu.QaClaimSearch);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(QaClaimSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaClaimSearchPage(Navigator, qaClaimSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor
            );
        }


        public QaAppealSearchPage NavigateToQaAppealSearch()
        {
            var qaAppealSearchPage = Navigator.Navigate<QaAppealSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsQAAppealSearch())
                    ClickOnMenu(SubMenu.QaAppealSearch);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(QaAppealSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaAppealSearchPage(Navigator, qaAppealSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderSearchPage NavigateToProviderSearch()
        {
            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsProviderSearch())
                    ClickOnMenu(SubMenu.ProviderSearch);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public SuspectProvidersPage NavigateToSuspectProviders()
        {
            var suspectProvidersPage = Navigator.Navigate<SuspectProvidersPageObjects>(() =>
            {
                if (Mouseover
                    .IsSuspectProviders())
                    ClickOnMenu(SubMenu.SuspectProviders);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(SuspectProvidersPageObjects.SpinnerCssLocator, How.CssSelector));
                WaitForStaticTime(4000);
            });
            return new SuspectProvidersPage(Navigator, suspectProvidersPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderActionPage NavigateToSuspectProvidersWorkList()
        {
            var providerActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                if (Mouseover
                    .IsSuspectProvidersWorkList())
                    ClickOnMenu(SubMenu.SuspectProvidersWorkList);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForSpinner();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderActionPageObjects.BasicProviderDetailsDivCssSelector,
                        How.CssSelector));

            });
            return new ProviderActionPage(Navigator, providerActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }


        public NewDefaultPage NavigateToAPage(string menuString, string subMenuString)
        {
            var pageToReturn = Navigator.Navigate<NewDefaultPageObjects>(() =>
            {
                if (Mouseover
                    .IsParticularPage(menuString, subMenuString))
                    ClickOnMenu(subMenuString);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                WaitForWorking();
            });
            return new NewDefaultPage(Navigator, pageToReturn, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);

        }

        public NewDefaultPage NavigateToAPage(string menuString, List<string> subMenuString)
        {
            var pageToReturn = Navigator.Navigate<NewDefaultPageObjects>(() =>
            {
                if (Mouseover.MouseOver(menuString, subMenuString[0], subMenuString[1]))
                    ClickOnMenu(subMenuString[1]);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                WaitForWorking();
            });
            return new NewDefaultPage(Navigator, pageToReturn, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);

        }

        /// <summary>
        /// Microstrategy
        /// </summary>
        /// <returns>Microstrategy</returns>
        public MicrostrategyPage NavigateToMicrostrategy()
        {
            var microstrategyPage = Navigator.Navigate<MicroStrategyPageObjects>(ClickOnMicrostrategy);
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(2000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
            return new MicrostrategyPage(Navigator, microstrategyPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public MicrostrategyPage NavigateToMicrostrategyWithMultipleReports()
        {
            var microstrategyPage =
                Navigator.Navigate<MicroStrategyPageObjects>(clickonMicrostrategyToViewListOfReports);
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
            return new MicrostrategyPage(Navigator, microstrategyPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }






        public UserProfileSearchPage NavigateToCreateNewUserAccount()
        {
            _newUserProfileSearch = NavigateToNewUserProfileSearch();
            _newUserProfileSearch.NavigateToCreateNewUser();
            return new UserProfileSearchPage(Navigator, new UserProfileSearchPageObjects(), SiteDriver,
                JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }


        #endregion

        #region CHECK PAGE

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_defaultPage != null)
                hashCode = _defaultPage.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return string.Equals(this.ToString(), obj.ToString());
        }

        #endregion

        public bool IsWorkingAjaxMessagePresent(int time=0)
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector,time);
        }



        public void WaitForWorkingAjaxMessage(int time=0)
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent(),time);
            SiteDriver.WaitForPageToLoad();
        }

        public void WaitForLogoutIcon()
        {
            SiteDriver.WaitForCondition(() => IsLogoutIconPresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void WaitForWorkingAjaxMessageForBothDisplayAndHide()
        {
            SiteDriver.WaitForCondition(()=>IsWorkingAjaxMessagePresent(), 5000);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void WaitForSpinner()
        {
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
        }

        public void CloseAnyPopupIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealCreator.GetStringValue());
                }
            }
        }

        public void CloseCreateUserPopupIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.UserProfileSearch.GetStringValue());
                }
            }
        }

        public void CloseAnyTabIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindow(SiteDriver.WindowHandles[0]);
                }
            }
        }

        public void SwitchWindowByTitle(PageTitleEnum pageTitleEnum)
        {
            SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
            //bool value=SiteDriver.SwitchWindowByTitle(pageTitleEnum.AppealLetter.GetStringValue());
        }

        public void CloseAnyPopupIfExistInAppealManager()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealManager.GetStringValue());
                }
            }
        }

        public int GetWindowHandlesCount()
        {
            return SiteDriver.WindowHandles.Count;
        }

        public string GetCurrentWindowHandle()
        {
            return SiteDriver.CurrentWindowHandle;
        }

        public List<string> GetWindowHandles()
        {
            return SiteDriver.WindowHandles;
        }

        public void SwitchWindowByTitle(string windowTitle)
        {
            SiteDriver.SwitchWindowByTitle(windowTitle);
        }

        public void SwitchWindow(string windowName)
        {
            SiteDriver.SwitchWindow(windowName);
        }

        public void CloseWindow()
        {
            SiteDriver.CloseWindow();
        }

        public bool IsSubMenuPresent(string subMenu)
        {
            return SiteDriver.IsElementPresent(SubMenu.GetSubMenuLocator(subMenu), How.XPath);
        }

        public void SwitchToClaimActionByUrl()
        {
            SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimAction.GetStringValue());
        }

        public bool CheckIfPdfDownloadPageIsOpenAndCloseAndReturnToFirstWindow()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count == 0) return false;
            SiteDriver.SwitchWindow(handles[1]);
            var isPdfOpen = SiteDriver.FindElement("body>embed", How.CssSelector).GetAttribute("type")
                .Equals("application/pdf");
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindow(handles[0]);
            return isPdfOpen;
        }

        public ClaimActionPage SwitchToOpenNewClaimActionByUrl(string url, string user)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                if (SiteDriver.IsAlertBoxPresent())
                {
                    SiteDriver.AcceptAlertBox();
                }

                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            if (user == EnvironmentManager.HciAdminUsername1)
                new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                    Executor).LoginAsHciAdminUser1();
            else
                new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                    Executor).LoginAsHciAdminUser();
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newClaimAction = new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return newClaimAction;
        }

        public ClaimActionPage SwitchToOpenNewClaimActionByUrlForAdmin1FromLogicSearch(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {

                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsHciAdminUser1();
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newClaimAction = new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //newClaimAction.HandleAutomaticallyOpenedLogicManagerPopup();
            return newClaimAction;
        }

        public ClaimActionPage SwitchToOpenNewClaimActionByUrlForClientUserFromLogicSearchClient(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                Executor).LoginAsClientUser();
            var claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var claimAction = new ClaimActionPage(Navigator, claimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
            claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return claimAction;
        }





        public void WaitForStaticTime(int ms)
        {
            SiteDriver.WaitToLoadNew(ms);
        }

        public string GetLoggedInUserFullName()
        {
            if (GetPageHeader() == PageHeaderEnum.MyProfile.GetStringValue())
            {
                EnvironmentManager.UserFullName = GetSideWindow.GetInputValueByLabel("First Name") + " " +
                                                  GetSideWindow.GetInputValueByLabel("Last Name");
                return EnvironmentManager.UserFullName;
            }

            if (EnvironmentManager.UserFullName == null)
            {
                JavaScriptExecutor.ExecuteScript("window.open()");
                var handles = SiteDriver.WindowHandles.ToList();
                var myProfile = Navigator.Navigate<MyProfilePageObjects>(() =>
                {
                    SiteDriver.SwitchToLastWindow();
                    SiteDriver.Open(BrowserOptions.ApplicationUrl + PageUrlEnum.MyProfileSettings.GetStringValue());
                    SiteDriver.WaitForCondition(
                        () => SiteDriver.SwitchWindowByUrl(PageUrlEnum.MyProfileSettings.GetStringValue()));
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                });


                EnvironmentManager.UserFullName = GetSideWindow.GetInputValueByLabel("First Name") + " " +
                                                  GetSideWindow.GetInputValueByLabel("Last Name");
                SiteDriver.CloseWindowAndSwitchTo(handles[0]);
                return EnvironmentManager.UserFullName;
            }

            return EnvironmentManager.UserFullName;
        }


        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~`!@#$%^&*()_+=/<>{}[]";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public AppealActionPage SwitchToOpenAppealActionByUrlForAdmin(string url, int user = 1)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });
            if (user == 1)
                new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                    Executor).LoginAsHciAdminUser();
            else if (user == 2)
                new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                    Executor).LoginAsHciAdminUser1();
            else if (user == 5)
                new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions,
                    Executor).LoginAsHciAdminUserClaim5();
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newAppealAction = new AppealActionPage(Navigator, newAppealActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
            return newAppealAction;
        }

         public void ClickOnCIWIcon()
        {
            SiteDriver.FindElement(NewDefaultPageObjects.DownloadCIWIconCssSelector, How.CssSelector).Click();
            WriteLine("Clicked on Download CIW icon.");

        }
        public bool IsCIWIconDisplayed()
        {
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.DownloadCIWIconCssSelector, How.CssSelector);
        }

        public string GetCIWIconTooltip()
        {
            return SiteDriver.FindElement(NewDefaultPageObjects.DownloadCIWIconCssSelector, How.CssSelector).GetAttribute("title");
        }

        public void WaitForPageToLoadWithSideBarPanel()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
        }


        /// <summary>
        /// Batch => New Batch Search
        /// </summary>
        /// <returns>New Batch page</returns>
        public BatchSearchPage NavigateToBatchSearch()
        {
            var newBatchSearchPage = Navigator.Navigate<BatchSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsBatchSearch())
                    ClickOnMenu(SubMenu.BatchSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new BatchSearchPage(Navigator, newBatchSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }




        /// <summary>
        /// Client Search => New Client Search
        /// </summary>
        /// <returns>New Client Search page</returns>
        public ClientSearchPage NavigateToClientSearch(bool mouseOver = true)
        {
            var newClientSearchPage = Navigator.Navigate<ClientSearchPageObjects>(() =>
            {
                if (mouseOver)
                {
                    if (Mouseover
                        .IsNewClientSearch())
                        ClickOnMenu(SubMenu.ClientSearch);
                }
                else
                {
                    ClickOnMenu(SubMenu.ClientSearch);
                }

                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new ClientSearchPage(Navigator, newClientSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// User Profile Search => New User Profile Search
        /// </summary>
        /// <returns>New User Profile Search Page</returns>
        public UserProfileSearchPage NavigateToNewUserProfileSearch(bool waitForSideBar = true)
        {
            var newUserProfileSearchPage = Navigator.Navigate<UserProfileSearchPageObjects>(() =>
            {
                WaitForWorking();
                if (Mouseover
                    .IsSettingsNewUserProfileSearch())
                    ClickOnMenu(SubMenu.NewUserProfileSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                if (waitForSideBar)
                    WaitForPageToLoadWithSideBarPanel();
            });

            return new UserProfileSearchPage(Navigator, newUserProfileSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public RoleManagerPage NavigateToRoleManager()
        {
            var newRoleManagerPage = Navigator.Navigate<RoleManagerPageObject>(() =>
            {
                if (Mouseover
                    .IsSettingsRoleManager())
                    ClickOnMenu(SubMenu.RoleManager);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new RoleManagerPage(Navigator, newRoleManagerPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }


        public LogicSearchPage NavigateToLogicSearch()
        {
            var newLogicSearchPage = Navigator.Navigate<LogicSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsLogicSearch())
                    ClickOnMenu(SubMenu.LogicSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

                WaitForPageToLoadWithSideBarPanel();
            });

            return new LogicSearchPage(Navigator, newLogicSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor
            );
        }

        public MicrostrategyMaintenancePage NavigateToMicrostrategyMaintenance()
        {
            var newMicrostratergyPage = Navigator.Navigate<MicrostrategyMaintenancePageObjects>(() =>
                {
                    if (Mouseover
                        .IsMicrostrategyMaintainacePresent())
                        ClickOnMenu(SubMenu.MicrostrategyMaintenance);
                    //SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                    SiteDriver.WaitForCondition(() =>
                        !SiteDriver.IsElementPresent(NewDefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

                }
            );
            return new MicrostrategyMaintenancePage(Navigator, newMicrostratergyPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);

        }


        // <summary>
        /// PreAuthorization => Pre-Auth Creator Page
        /// </summary>
        /// <returns>Pre-Auth Creator Page</returns>
        public PreAuthCreatorPage NavigateToPreAuthCreatorPage()
        {
            var newPreAuthCreatorPage = Navigator.Navigate<PreAuthCreatorPageObjects>(() =>
            {
                if (Mouseover
                    .IsPreAuthCreatorPresent())
                    ClickOnMenu(SubMenu.PreAuthCreator);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

            });

            return new PreAuthCreatorPage(Navigator, newPreAuthCreatorPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }




        // <summary>
        // Claim => PCI Coders Claim
        // </summary>
        // <returns>PCI Coders Claims</returns>
        public ClaimActionPage NavigateToCVCodersClaim(bool handlePopup = false, bool isFromRetroPage = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover
                        .IsCVCodersClaim())
                    ClickOnMenu(SubMenu.CVCodersClaims);
                WriteLine("First Wait");
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WriteLine("Second Wait");
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                WriteLine("Third Wait");
                if (isFromRetroPage)
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                else
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                            How.CssSelector) ||
                        SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator,
                            How.CssSelector));
                SiteDriver.WaitForIe(7000);
                CaptureScreenShot("pcicoder");

            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public QuickLaunchPage SwitchTabAndNavigateToQuickLaunchPage(bool changeUserToAdmin = false, int tabno = 1)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();


            QuickLaunchPageObjects redirectPage = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[tabno]);
                if (changeUserToAdmin)
                {

                    SiteDriver.Open(BrowserOptions.ApplicationUrl);
                    new LoginPage(Navigator, new LoginPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager,
                        BrowserOptions, Executor).LoginAsHciAdminUser();

                }
                else
                {
                    SiteDriver.Open(BrowserOptions.ApplicationUrl + PageUrlEnum.QuickLaunch.GetStringValue());
                    SiteDriver.WaitForCondition(
                        () => SiteDriver.IsElementPresent(NewDefaultPageObjects.LandingImageCssLocator,
                            How.CssSelector), 10000);
                }

            });

            return new QuickLaunchPage(Navigator, redirectPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);

        }


        public UserProfileSearchPage SearchByUserIdToNavigateToUserSettingsForm(string userId)
        {
            NavigateToNewUserProfileSearch();
            _getSideBarPanelSearch.OpenSidebarPanel();
            _getSideBarPanelSearch.SetInputFieldByLabel("User ID", userId);
            _getSideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
            _getGridViewSection.ClickOnGridRowByRow();
            return new UserProfileSearchPage(Navigator, new UserProfileSearchPageObjects(), SiteDriver,
                JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void SwitchToLastWindow()
        {
            SiteDriver.SwitchToLastWindow();
        }

        public void SwitchTab(string currenthandle, bool closeCurrent = false)
        {
            var handles = SiteDriver.WindowHandles.ToList();

            if (handles.Count == 2)
            {

                var toSwitch = handles.IndexOf(currenthandle) == 0 ? handles[1] : handles[0];
                if (closeCurrent)
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(toSwitch);
            }


        }

        public void CloseNewTabIfExists(PageTitleEnum pageTitle)
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count != 2) return;
            SiteDriver.SwitchWindow(handles[0]);
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(pageTitle.GetStringValue());
        }



        public string CurrentDateTimeInMstStandard(DateTime? date)
        {
            var utcTime = date ?? DateTime.UtcNow;
            var serverZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone).ToString("MM/dd/yyyy hh:mm tt");
        }

        public string CurrentDateInMstStandard(DateTime? date)
        {
            var utcTime = date ?? DateTime.UtcNow;
            var serverZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone).ToString("MM/dd/yyyy");
        }

        public DateTime CurrentDateTimeInMst(DateTime? date)
        {
            var utcTime = date ?? DateTime.UtcNow;
            var serverZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone);
        }


        public PreAuthSearchPage NavigateToPreAuthSearch()
        {
            var preAuthSearchPage = Navigator.Navigate<PreAuthSearchPageObjects>(() =>
            {
                if (Mouseover
                    .IsPreAuthSearchPresent())
                    ClickOnMenu(SubMenu.PreAuthSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new PreAuthSearchPage(Navigator, preAuthSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public EditSettingsManagerPage NavigateToEditSettingsManager(bool mouseOver = true, bool isInactive = false)
        {
            var editSettingsManagerPage = Navigator.Navigate<EditSettingsManagerPageObject>(() =>
            {
                if (mouseOver)
                {
                    if (Mouseover
                        .IsNewClientSearch())
                        ClickOnMenu(SubMenu.ClientSearch);
                }
                else
                {
                    ClickOnMenu(SubMenu.ClientSearch);
                }

                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
                if (isInactive)
                {
                    _getSideBarPanelSearch.ClickOnClearLink();
                    _getSideBarPanelSearch.SetInputFieldByLabel("Status", "Inactive", sendTabKey: true);
                    _getSideBarPanelSearch.ClickOnFindButton();
                    WaitForWorking();
                    if (_getGridViewSection.GetGridRowCount() > 0)
                    {
                        _getGridViewSection.ClickOnGridRowByRow();

                    }
                }
                else
                {

                    _getSideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.SMTST.ToString());
                    _getSideBarPanelSearch.ClickOnFindButton();
                    WaitForWorking();
                    _getGridViewSection.ClickOnGridByRowCol(1, 2);
                }

                WaitForWorking();
                ClickOnEditSettingsIcon();
                WaitForPageToLoadWithSideBarPanel();
                //SiteDriver.Open(EnvironmentManager.Instance.ApplicationUrl + PageUrlEnum.EditSettingsManager.GetStringValue());
                //WaitForPageToLoadWithSideBarPanel();
            });

            return new EditSettingsManagerPage(Navigator, editSettingsManagerPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderSearchPage SwitchTabAndNavigateToProviderSearchPage(string url, bool changeUserToAdmin = false)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();

            ProviderSearchPageObjects redirectPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                if (changeUserToAdmin)
                {

                    SiteDriver.Open(url);
                    new LoginPage(Navigator, new LoginPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager,
                        BrowserOptions, Executor).LoginAsHciAdminUser();

                }
                else
                {
                    SiteDriver.Open(url);
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(ProviderSearchPageObjects.PageHeaderCssLocator, How.CssSelector));
                }

            });

            return new ProviderSearchPage(Navigator, redirectPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);

        }

        public ClientSearchPage ClickOnReturnToClientSearch()
        {
            SiteDriver.FindElement(EditSettingsManagerPageObject.ReturnToClientSearchCssSelector,
                How.CssSelector).Click();
            return new ClientSearchPage(Navigator, new ClientSearchPageObjects(), SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsUserNameGreetingMenuPresent()
        {
            MouseOverOnUserMenu();
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.UserMenuId, How.Id);
        }

        public bool IsManagerMenuPresent()
        {
            return JavaScriptExecutor.IsElementPresent(NewDefaultPageObjects.ManagerTabByCss);

        }

        public List<string> GetManagerSubMenu()
        {
            MouseOverManagerMenu();
            return JavaScriptExecutor.FindElements(NewDefaultPageObjects.ManagerTabSubMenu);
        }

        public string GetUserNameGreetingMenuDropdownLabel()
        {
            return
                SiteDriver.FindElement(NewDefaultPageObjects.UserGreetingMenuLabel, How.CssSelector)
                    .Text;
        }

        public List<string> GetUserNameGreetingMenuDropdownOptions()
        {
            MouseOverOnUserMenu();
            var list = JavaScriptExecutor.FindElements(NewDefaultPageObjects.UserGreetingMenuList, "Text");
            return list;

        }

        public List<string> GetQuickLinksMenuOptions()
        {
            MouseOverQuickLinks();
            var list =
                JavaScriptExecutor.FindElements(NewDefaultPageObjects.QuickLinksOptionsCssSelector, "Text");
            return list;
        }

        public List<string> GetCotivitiLinksMenuOptions()
        {
            MouseOverCotivitiLinks();
            var list = JavaScriptExecutor.FindElements(NewDefaultPageObjects.CotivitiLinksOptionsCssSelector, "Text");
            return list;
        }

        public List<string> GetTopNavigationMenuOptions()
        {
            var list = JavaScriptExecutor.FindElements(NewDefaultPageObjects.TopNavMenuOptions, "Text");
            return list;
        }


        public ProviderSearchPage ClickOnProviderSearchLinkAndNavigateToProviderSearchPage(string linkName)
        {

            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(String.Format(NewDefaultPageObjects.CotivitiLinksByLinkNameXPath, linkName),
                    How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ProviderSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }



        public void ClickOnCotivitiLinksByLinkName(string linkName) =>
            SiteDriver.FindElement(string.Format(NewDefaultPageObjects.CotivitiLinksByLinkNameXPath, linkName),
                How.XPath).Click();

        public void MouseOverCotivitiLinks()
        {
            ClickOnPageFooter();
            WaitForStaticTime(500);
            MouseOverOnUserMenu();
            WaitForStaticTime(500);
            MouseOverOnQuickLinks();
            WaitForStaticTime(500);
            MouseOverOnCotivitiLinks();
            WaitForStaticTime(500);
        }

        public void MouseOverQuickLinks()
        {
            ClickOnPageFooter();
            WaitForStaticTime(500);
            MouseOverOnUserMenu();
            WaitForStaticTime(500);
            MouseOverOnQuickLinks();
            WaitForStaticTime(500);
        }


        #region PRIVATE/PROTECTED METHODS

        private void ClickOnMenu(String menuOption)
        {
            var menuToClick = HeaderMenu.GetElementLocatorTemplate(menuOption);
            //JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);

            WaitForStaticTime(2000);
            try
            {
                SiteDriver.FindElement(menuToClick, How.XPath).Click();
                WaitForWorking();
            }
            catch (Exception e)
            {
                WriteLine(e);
                JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            }
           
            SiteDriver.WaitForPageToLoad();
            SiteDriver.MouseOver("footer", How.Id, release: true);

            WriteLine("Navigated to {0}", menuOption);
            try
            {
                ClickOnPageFooter();
            }
            catch (Exception e)
            {
                WriteLine("Page Header is overlapped by the menu dropdown \n" + e.Message);
            }

            if (SiteDriver.IsElementPresent(menuToClick, How.XPath))
                JavaScriptExecutor.ExecuteMouseOut(menuToClick, How.XPath);
        }

        private void ClickOnNormalizedMenu(String menuOption)
        {
            var menuToClick = HeaderMenu.GetNormalizedElementLocatorTemplate(menuOption);
            //JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);

            try
            {
                SiteDriver.FindElement(menuToClick, How.XPath).Click();
            }
            catch (Exception e)
            {
                WriteLine(e);
                JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            }


            SiteDriver.MouseOver("footer", How.Id, release: true);

            WriteLine("Navigated to {0}", menuOption);
            try
            {
                ClickOnPageFooter();
            }
            catch (Exception e)
            {
                WriteLine("Page Header is overlapped by the menu dropdown \n" + e.Message);
            }
            if (SiteDriver.IsElementPresent(menuToClick, How.XPath))
                JavaScriptExecutor.ExecuteMouseOut(menuToClick, How.XPath);
        }

        public void ClickOnDashboardIcon()
        {
            MouseOverOnUserMenu();
            JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.DashboardIconXPath, How.XPath);
            SiteDriver.WaitForPageToLoad();
            ClickOnPageFooter();
            WriteLine("Clicked on Dashboard icon.");
        }

        public MicrostrategyReportPage ClickOnDashboardIconToNavigateToMicrostrategyHome()
        {
            var microstrategyPage = Navigator.Navigate<MicrostrategyReportPageObjects>(() =>
            {
                ClickOnDashboardIcon();
                WriteLine("Clicked on Dashboard icon.");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(MicrostrategyReportPageObjects.DashboardLabelCssSelector,
                        How.CssSelector));
                SiteDriver.WaitToLoadNew(2000);
                SiteDriver.WaitForPageToLoad();
                //SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
                //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.SwitchBackToMainFrame();

            });

            return new MicrostrategyReportPage(Navigator, microstrategyPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);

        }

        public MicrostrategyPage ClickOnDashboardIconToNavigateToMicrostrategy()
        {
            var microstrategyPage = Navigator.Navigate<MicroStrategyPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.DashboardIconByCss, How.CssSelector);
                WriteLine("Clicked on Dashboard icon.");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
                SiteDriver.WaitToLoadNew(2000);
                SiteDriver.WaitForPageToLoad();
                //SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
                //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.SwitchBackToMainFrame();
            });

            return new MicrostrategyPage(Navigator, microstrategyPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }

        protected int GetElementHeight(IWebElement webElement)
        {
            string style = webElement.GetAttribute("style").ToLower();
            string heightStr = style.Substring(style.IndexOf("height")).Trim();
            int startIndex = heightStr.IndexOf(":") + 1;
            int endIndex = heightStr.IndexOf("px");
            return Convert.ToInt32(heightStr.Substring(startIndex, endIndex - startIndex));
        }

        #endregion

        #region EditSettingsManager

        public void ClickOnEditSettingsIcon()
        {
            SiteDriver.FindElement(NewDefaultPageObjects.EditSettingsIcon, How.CssSelector).Click();
            WaitForPageToLoad();
        }

        #endregion

        #region Invalid Input

        private const string InvalidInputByLabelXPathSelector =
            "//label[text()='{0}']/..//input[contains(@class,'invalid')]";

        public bool IsInvalidInputPresentByLabel(string label)
        {
            bool firstCondition = SiteDriver.IsElementPresent(
                Format(NewDefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                How.XPath);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement(
                    Format(NewDefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                    How.XPath).GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public string GetInvalidInputToolTipByLabel(string label)
        {
            return SiteDriver.FindElement(
                Format(NewDefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                How.XPath).GetAttribute("title");
        }

        public string GetInvalidTooltipMessageOnNote()
        {
            var element = SiteDriver.FindElement(NewDefaultPageObjects.NoteDivCssLocator,
                How.CssSelector);
            return element.GetAttribute("title");
        }

        public bool IsInvalidInputPresentOnNote()
        {
            WaitForStaticTime(1000);
            bool firstCondition = SiteDriver.IsElementPresent(NewDefaultPageObjects.NoteDivCssLocator,
                How.CssSelector);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement(NewDefaultPageObjects.NoteDivCssLocator, How.CssSelector)
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public void ClickSelectAllOrDeselectAll(bool selectAll = true)
        {
            if (selectAll)
                SiteDriver.FindElement(NewDefaultPageObjects.SelectAllLinkInTransferComponentCssSelector, How.CssSelector).Click();

            else
                SiteDriver.FindElement(NewDefaultPageObjects.DeselectAllLinkInTransferComponentCssSelector, How.CssSelector).Click();
        }

        public string GetInvalidTooltipMessageOnNoteByLabel(string label)
        {
            return
                SiteDriver.FindElement(Format(NewDefaultPageObjects.NoteDivByLabelXPathTemplate, label),
                    How.XPath).GetAttribute("title");
        }

        public bool IsInvalidInputPresentOnNoteByLabel(string label)
        {
            bool firstCondition = SiteDriver.IsElementPresent(
                Format(NewDefaultPageObjects.NoteDivByLabelXPathTemplate, label),
                How.XPath);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver
                    .FindElement(Format(NewDefaultPageObjects.NoteDivByLabelXPathTemplate, label), How.XPath)
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        // Check for Invalid highlight for Transfer Components (eg. click and assign roles)
        public bool IsInvalidInputPresentOnTransferComponentByLabel(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(Format(NewDefaultPageObjects.TransferComponentByLabelCssSelector,
                    label));
            var secondCondition = false;

            if (firstCondition)
                secondCondition = JavaScriptExecutor
                    .FindElement(Format(NewDefaultPageObjects.TransferComponentByLabelCssSelector, label))
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public bool IsValidInputPresentOnTransferComponentByLabel(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(
                    Format(NewDefaultPageObjects.DisplayedTransferComponentByLabelCssSelector, label));

            return firstCondition;
        }


        public bool IsInvalidDropdownInputFieldPresent(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(Format(NewDefaultPageObjects.InvalidDropdownInputFieldCssSelector,
                    label));
            var secondCondition = false;

            if (firstCondition)
                secondCondition = JavaScriptExecutor
                    .FindElement(Format(NewDefaultPageObjects.InvalidDropdownInputFieldCssSelector, label))
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            ;
            return firstCondition && secondCondition;
        }

        public int GetCountOfInvalidRed()
        {
            return SiteDriver.FindElementsCount(NewDefaultPageObjects.AllInvalidCssLocator, How.CssSelector);
        }

        #endregion


        #region Available/Assigned 

        public bool IsRoleAssigned<T>(List<string> userId, string roleName, bool isSearchedByUserId = true,
            bool isAssigned = true, bool isDefaultPageUserProfile = false)
        {
            var target = typeof(T);

            if (isDefaultPageUserProfile || !IsPageHeaderPresent() ||
                GetPageHeader() != PageHeaderEnum.UserProfileSearch.GetStringValue())
                _newUserProfileSearch = NavigateToNewUserProfileSearch();
            else
            {
                _newUserProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                _newUserProfileSearch.SideBarPanelSearch.ClickOnClearLink();
            }

            _newUserProfileSearch.SearchUserByNameOrId(userId, isSearchedByUserId, !isSearchedByUserId);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(isSearchedByUserId
                ? userId[0]
                : userId[0] + " " + userId[1]);

            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities.GetStringValue());

            var isPresent = _newUserProfileSearch.IsAvailableAssignedRowPresent(1, roleName, isAssigned);
            if (typeof(ClaimSearchPage) == target)
            {
                NavigateToClaimSearch();
            }

            if (typeof(AppealSearchPage) == target)
            {
                NavigateToAppealSearch();
            }

            if (typeof(InvoiceSearchPage) == target)
            {
                NavigateToInvoiceSearch();
            }
            else if (typeof(QuickLaunchPage) == target)
            {
                ClickOnQuickLaunch();
            }

            return isPresent;
        }

        public void ClickOnAvailableAssignedRow(int headerType, string value, bool isAvailableAssigned = true)
        {
            var headerLabel = "";
            switch (headerType)
            {
                case 1:
                    headerLabel = "Roles";
                    break;
                case 2:
                    headerLabel = "Clients";
                    break;
                case 3:
                    headerLabel = "Access";
                    break;
                case 4:
                    headerLabel = "Flags";
                    break;
            }

            if (isAvailableAssigned)
                SiteDriver.FindElement(
                    Format(NewDefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Available {headerLabel}", value),
                    How.XPath).Click();
            else
            {
                var label = headerType == 4 ? $"Selected {headerLabel}" : $"Assigned {headerLabel}";
                SiteDriver.FindElement(
                    Format(NewDefaultPageObjects.AvailableAssignedRowXPathTemplte, label, value),
                    How.XPath).Click();
            }
                

        }


        public void TransferAllAvailableAssignedRow(int headerType, bool isAvailableAssigned = true)
        {
            var headerLabel = "";

            switch (headerType)
            {
                case 1:
                    headerLabel = "Roles";
                    break;
                case 2:
                    headerLabel = "Clients";
                    break;
                case 3:
                    headerLabel = "Access";
                    break;
                case 4:
                    headerLabel = "Flags";
                    break;

            }

            if (isAvailableAssigned)
            {
                var elements = SiteDriver.FindElements(
                    Format(NewDefaultPageObjects.AllAvailableListCssLocator, $"Available {headerLabel}"), How.XPath);
                ClickAllElements(elements);
            }



            else
            {
                var elements =
                    SiteDriver.FindElements(
                        Format(NewDefaultPageObjects.AllAssignedListXPath, $"Assigned {headerLabel}"), How.XPath);
                ClickAllElements(elements);
            }
        }

        public void GenericTransferAll(string headerName)
        {
            var elements = JavaScriptExecutor
                .FindWebElements(Format(NewDefaultPageObjects.AvailableAssignedListCssLocator, headerName));

            ClickAllElements(elements);

            WriteLine($"Clicked all of the options under {headerName} column in the transfer component");
        }

        public List<string> GenericGetAvailableAssignedList(string headerName)
        {
            return JavaScriptExecutor.FindElements(Format
                (NewDefaultPageObjects.AvailableAssignedListCssLocator, headerName), "Text");
        }

        public bool IsAvailableAssignedRowPresent(int headerType, string value, bool isAssigned = true)
        {
            var headerLabel = "";
            switch (headerType)
            {
                case 1:
                    headerLabel = "Roles";
                    break;
                case 2:
                    headerLabel = "Clients";
                    break;
                case 3:
                    headerLabel = "Access";
                    break;

            }

            return SiteDriver.IsElementPresent(
                !isAssigned
                    ? Format(NewDefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Available {headerLabel}", value)
                    : Format(NewDefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Assigned {headerLabel}", value),
                How.XPath);
        }

        public List<string> GetAvailableAssignedList(int? headerType = null, bool isAvailable = true )
        {
            var headerLabel = "";
            switch (headerType)
            {
                case 1:
                    headerLabel = "Roles";
                    break;
                case 2:
                    headerLabel = "Clients";
                    break;
                case 3:
                    headerLabel = "Access";
                    break;
                default:
                    headerLabel = "Clients";
                    break;
            }

            if (isAvailable)
            {
                //return JavaScriptExecutors.FindElements(Format
                //    (NewDefaultPageObjects.AvailableAssignedListCssLocator, "left_list", $"Available {headerLabel}"), "Text");
                return JavaScriptExecutor.FindElements(Format
                    (NewDefaultPageObjects.AvailableAssignedListCssLocator, $"Available {headerLabel}"), "Text");
            }

            //return JavaScriptExecutors.FindElements(Format
            //    (NewDefaultPageObjects.AvailableAssignedListCssLocator, "right_list", $"Assigned {headerLabel}"), "Text");
            return JavaScriptExecutor.FindElements(Format
                (NewDefaultPageObjects.AvailableAssignedListCssLocator, $"Assigned {headerLabel}"), "Text");
        }

        public void ClickAllElements(IEnumerable<IWebElement> elements)
        {
            foreach (var ele in elements)
                ele.Click();
        }

        public bool IsAvailableAssignedClientsComponentPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(
                NewDefaultPageObjects.DisplayedTransferComponentByLabelCssSelector,
                label));

        public bool IsAvailableAssignedClientsDisabled(string header, string client) => JavaScriptExecutor
            .FindElement(Format(NewDefaultPageObjects.AvailableAssignedClientsDivCssSelectorTemplate, header, client))
            .GetAttribute("class").Contains("is_disabled");

        #endregion


        public ChromeDownLoadPage NavigateToChromeDownLoadPage()
        {
            var url = "Chrome://Downloads/";
            if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"], "edge", StringComparison.OrdinalIgnoreCase) == 0)
                url = "edge://downloads/all";
            var chromeDownLoad =
                Navigator.Navigate<ChromeDownLoadPageObjects>(() => SiteDriver.Open(url));
            return new ChromeDownLoadPage(Navigator, chromeDownLoad, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }

        public string CalculateAndGetAppealDueDateFromDatabase(string turnaroundTime, string type = "Business")
        {
            if (type == "Business")
            {
                return Executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDate, turnaroundTime));
            }

            else
            {
                return Executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDateForCalendarType, turnaroundTime));

            }
        }

        public string GetAppealLevel(string claseq, ClientEnum client)
        {
            var claimSeq = claseq.Split('-');
            return Executor.GetSingleStringValue(
                string.Format(AppealSqlScriptObjects.GetAppealLevel, claimSeq[0], claimSeq[1], client));


        }
        
        public bool IsElementPresent(string select, How selector, int timeOut = 0) =>
            SiteDriver.IsElementPresent(select, selector, timeOut);

        public List<string> WindowHandles => SiteDriver.WindowHandles;

        public string CurrentWindowHandle => SiteDriver.CurrentWindowHandle;

        public bool SwitchWindowByUrl(string windowUrl) => SiteDriver.SwitchWindowByUrl(windowUrl);


        #region Mouseover
        public List<string> GetSecondarySubMenuOptionList(string menuName, string submenuName) =>
            GetMouseOver.GetSecondarySubMenuOptionList(menuName, submenuName);

        public bool IsSecondarySubMenuOptionPresent(string menuName, string submenuName, string secondarySubMenu) =>
            GetMouseOver.IsSecondarySubMenuOptionPresent(menuName, submenuName, secondarySubMenu);

        public bool IsPCI_FCIClaimsWorkList() => GetMouseOver.IsPCI_FCIClaimsWorkList();

        #endregion

        public List<string> GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(string headerName) =>
            GetSubMenu.GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(headerName);

        public string Title => SiteDriver.Title;

        public void DeleteClaimDocumentRecord(string claSeq, string audit_date)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimDocumentRecord, claSeq.Split('-')[0],
                claSeq.Split('-')[1], audit_date));
            WriteLine("Delete Claim Document Record from database  for claseq<{0}>", claSeq);
        }

        public void SendValuesOnTextArea(string note, string text,bool handlePopup=true)
        {
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField(true);
            SiteDriver.WaitToLoadNew(1000);
            if (IsNullOrEmpty(text))
            {
                SiteDriver.FindElement("body", How.CssSelector).SendKeys(Keys.Backspace);

            }

            else
            {
                if (text.Length >= 1990)
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(0, text.Length - 4), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);//wait for removing last character which will take few ms
                    WriteLine("Note set to {0}", text);
                }
                else
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(1, text.Length - 1), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement("body", How.CssSelector).SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                }
            }
            WriteLine("{0} set to {1}", note, text);
            SiteDriver.SwitchBackToMainFrame();
            if(handlePopup  && IsPageErrorPopupModalPresent())
                ClosePageError();
        }
    }
}
    

