using System;
using System.Collections.Generic;
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
using Nucleus.Service.PageServices.Invoice;
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
using Nucleus.Service.PageServices.PreAuthorization;
using static System.String;
using Nucleus.Service.SqlScriptObjects.Appeal;

namespace Nucleus.Service.PageServices.Base.Default
{
    public class DefaultPage : BasePageService
    {
        #region PRIVATE FIELDS

        private readonly DefaultPageObjects _defaultPage;
        private SwitchClientPageObjects _switchClientPage;
        private SwitchClientPage _switchClientPageService;
        private readonly string _originalWindow;
        private readonly OracleStatementExecutor _executor;
        private readonly CommonSQLObjects _commonSqlObjects;
        private static Random random = new Random();
        private LoginPage _nucleusLoginPage;
        private readonly GridViewSection _getGridViewSection;
        private readonly SideBarPanelSearch _getSideBarPanelSearch;
        private ClientSearchPage _clientSearch;
        private UserProfileSearchPage _newUserProfileSearch;
        private readonly SideWindow _sideWindow;
        private COBAppealsDetailPage _cobAppealsDetail;
        private DashboardPage _dashboard;

        #endregion

        #region CONSTRUCTOR

        public DefaultPage(INavigator navigator, PageBase pageObject)
            : base(navigator, pageObject)
        {
            _defaultPage = (DefaultPageObjects)pageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _commonSqlObjects = new CommonSQLObjects();
            _executor = new OracleStatementExecutor();
            _getGridViewSection = new GridViewSection();
            _getSideBarPanelSearch = new SideBarPanelSearch();
            _sideWindow = new SideWindow();
        }

        #endregion

        #region PUBLIC METHODS

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public GridViewSection GetGridViewSection() => _getGridViewSection;

        public bool IsWindowOpenedAsTab()
        {
            return JavaScriptExecutor.IsWindowOpenedAsTab();

        }
        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        #region UnAuthorizedPage

        public string GetUnAuthorizedMessage()
        {
            return SiteDriver.FindElement<Section>(DefaultPageObjects.UnAuthorizedMessageCssSelector, How.XPath)
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
                return (T)Activator.CreateInstance(target, Navigator, PageObject, false);
            }
            if (typeof(QuickLaunchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<QuickLaunchPageObjects>(ClickOnReturnToLastPageOnly);
            }
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            WaitForStaticTime(1000);
            return (T)Activator.CreateInstance(target, Navigator, PageObject);
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
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForStaticTime(1000);
                return (T)Activator.CreateInstance(target, Navigator, PageObject, true);
            }
            if (typeof(ClaimActionPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimActionPageObjects>(VisitAndGetUrlByUrlFoAuthorizedPage(url));
                WaitForWorkingAjaxMessage();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForStaticTime(1000);
                return (T)Activator.CreateInstance(target, Navigator, PageObject, false);
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
                return (T)Activator.CreateInstance(target, Navigator, PageObject);
            }

            if (typeof(ProfileManagerPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ProfileManagerPageObjects>(VisitAndGetUrlByUrlForPopUpPage(url));
                return (T)Activator.CreateInstance(target, Navigator, PageObject);
            }

            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            WaitForStaticTime(1000);
            return (T)Activator.CreateInstance(target, Navigator, PageObject);
        }

        public string VisitAndGetUrlByUrlForUnAuthorizedPage(string url)
        {
            var browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
            var applicationUrl = browserOptions.ApplicationUrl;
            SiteDriver.Open(applicationUrl + url);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.ReturnToTheLastPageLinkCssSelector, How.XPath));
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
                var browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                var applicationUrl = browserOptions.ApplicationUrl;
                SiteDriver.Open(applicationUrl + url);

            };
        }
        public Action VisitAndGetUrlByUrlFoAuthorizedPage(string url)
        {
            return () =>
            {
                var browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                var applicationUrl = browserOptions.ApplicationUrl;
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
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                }
            };
        }

        public void ClickOnReturnToLastPageOnly()
        {
            JavaScriptExecutor.ExecuteClick(
                DefaultPageObjects.ReturnToTheLastPageLinkCssSelector, How.XPath);
        }
        public void ClickOnBrowserBackButton()
        {
            SiteDriver.WebDriver.Navigate().Back();
            Console.WriteLine("Clicked on Browser's back button");
        }
        #endregion

        public bool IsDashboardIconPresent()
        {
            MouseOverOnUserMenu();
            var result = SiteDriver.IsElementPresent(DefaultPageObjects.DashboardIconXPath, How.XPath);
            ClickOnPageFooter();
            return result;
        }
        public string GetPageHeader()
        {
            if (IsPageHeaderPresent())
                return SiteDriver.FindElement<Section>(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
            if (CurrentPageUrl.EndsWith(PageUrlEnum.QuickLaunch.GetStringValue()) || CurrentPageUrl.EndsWith(PageUrlEnum.QuickLaunch.GetStringValue() + "/"))
                return PageHeaderEnum.QuickLaunch.GetStringValue();
            return SiteDriver.FindElement<Section>(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public bool IsPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector);
        }

        public void WaitForPageHeader()
        {
            SiteDriver.WaitForCondition(IsPageHeaderPresent);
        }

        public bool IsNucleusHeaderPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.NucleusHeaderCssLocator, How.CssSelector);
        }

        public bool IsClientNucleusHeaderPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.ClientNucleusHeaderCssLocator, How.CssSelector);
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
            //if (typeof(ProviderSearchPage) == target)
            //{

            //    PageObject = Navigator.Navigate<ProviderSearchPageObjects>(action);
            //}
            //else
            if (typeof(ClaimsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<ClaimsDetailPageObjects>(action);
            }
            else if (typeof(AppealsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<AppealsDetailPageObjects>(action);
            }
            else if (typeof(DashboardLogicRequestsDetailsPage) == target)
            {
                PageObject = Navigator.Navigate<DashboardLogicRequestsDetailPageObjects>(action);
            }
            else if (typeof(QuickLaunchPage) == target)
            {
                PageObject = Navigator.Navigate<QuickLaunchPageObjects>(action);
            }
            else if (typeof(InvoiceSearchPage) == target)
            {
                PageObject = Navigator.Navigate<InvoiceSearchPageObjects>(action);
            }
            return (T)Activator.CreateInstance(target, Navigator, PageObject);
        }

        public bool IsRepositoryTabPresent()
        {
            return SiteDriver.IsElementPresent(RepositoryPageObjects.referenceLabelXpath, How.XPath);
        }
        public bool IsLogicSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(SubMenu.GetSubMenuLocator(SubMenu.LogicSearch), How.XPath);
        }

        public void WaitToLoadPageErrorPopupModal(int waitTime = 500)
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, waitTime);
        }
        public bool IsPageErrorPopupModalPresent()
        {
            SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageErrorPopupModelId, How.Id), 500);
            return SiteDriver.IsElementPresent(DefaultPageObjects.PageErrorPopupModelId, How.Id, 200);
        }
        public string GetPageErrorMessage()
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 500);
            return SiteDriver.FindElement<TextLabel>(DefaultPageObjects.PageErrorMessageId, How.Id).Text;
        }

        public void ClosePageError()
        {
            JavaScriptExecutor.ExecuteClick(DefaultPageObjects.PageErrorCloseId, How.Id);
            WaitForStaticTime(500);
            Console.WriteLine("Closed the modal popup");
        }

        public void ClickOkCancelOnConfirmationModal(bool confirmation)
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 500);
            if (!SiteDriver.IsElementPresent(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector)
            ) return;
            if (confirmation)
            {
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
                WaitForWorking();
                Console.WriteLine("Ok Button is Clicked");

            }
            else
            {
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.CancelConfirmationCssSelector, How.CssSelector);
                WaitForWorking();
                Console.WriteLine("Cancel Button is Clicked");

            }

        }

        public bool IsOkButtonPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
        }

        public bool IsCancelLinkPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.CancelConfirmationCssSelector, How.CssSelector);
        }
        public void RefreshPage(bool waitSidebar = true, int milliseconds = 0, bool isRetroPage = false)
        {
            SiteDriver.Refresh();
            if (SiteDriver.IsAlertBoxPresent())
                SiteDriver.AcceptAlertBox();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector), milliseconds);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector), milliseconds);
            if (!isRetroPage)
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector), milliseconds);
            if (waitSidebar)
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector), milliseconds);
        }
        public void WaitForWorking()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitToLoadNew(300);
        }

        public void WaitForPageToLoad()
        {
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForJqueryStatusCondition();
        }

        public string GetTitle()
        {
            return SiteDriver.Title;
        }

        public string GetPopupPageTitle()
        {
            return SiteDriver.FindElement<TextLabel>(DefaultPageObjects.PopupPageTitleCssLocator, How.CssSelector).Text;
        }

        #region SWITCH CLIENT

        public bool IsDefaultTestClient(ClientEnum clientName)
        {
            return !SiteDriver.IsElementPresent(DefaultPageObjects.ClientSmallBrandingImgId, How.Id);

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
                    EnvironmentManager.Instance.TestClient);
            }
            return IsDefaultTestClientForEmberPage(
                    EnvironmentManager.Instance.TestClient);

        }

        public bool CheckIfCurrentClientIsExpectedFromClaimUrlForRetroPages(ClientEnum clientName)
        {
            return SiteDriver.IsElementPresent(string.Format(DefaultPageObjects.CheckCurrentClientFromClaimUrl, clientName), How.CssSelector);
        }

        public bool IsDefaultTestClientForEmberPage(ClientEnum clientName)
        {

            var client = SiteDriver.FindElement<Section>(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector).GetAttribute("value") ??
                            SiteDriver.FindElement<Section>(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector)
                                .Text;
            return client.Equals(clientName.GetStringValue());

        }

        public string GetCurrentClient()
        {
            var client = SiteDriver.FindElement<Section>(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector).GetAttribute("value") ??
                         SiteDriver.FindElement<Section>(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector).Text;
            return client;
        }


        public bool IsClientLogoPresentForRetroPage(ClientEnum clientName, bool wait = false)
        {
            if (wait)
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.ClientSmallBrandingImgId, How.Id), 5000);
            return SiteDriver.IsElementPresent(DefaultPageObjects.ClientSmallBrandingImgId, How.Id);

        }



        public bool IsClientLogoPresent(ClientEnum clientName, bool mstr = false)
        {
            bool client = SiteDriver.IsElementPresent(DefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);
            if (mstr)
                return client;
            if (client)
            {
                var element =
                     SiteDriver.FindElement<Link>(DefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);
                return element.GetAttribute("Alt").Equals(clientName.ToString());
            }

            return false;
        }

        public string GetClientLogoFromDB(string clientcode)
        {
            return _executor.GetFileBlobValue(String.Format(CommonSQLObjects.GetCLientLogoFromDB, clientcode));
        }


        public string GetClientCode() => SiteDriver
            .FindElement<Section>(DefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector)
            .GetAttribute("alt");

        public DefaultPage ClickOnSwitchClient()
        {
            SiteDriver.WaitToLoadNew(3000);
            var switchClient = Navigator.Navigate<DefaultPageObjects>(() => _defaultPage.SwitchClientButton.Click());
            SiteDriver.WaitToLoadNew(2000);
            return new DefaultPage(Navigator, switchClient);
        }


        public List<string> GetSwitchClientList()
        {
            return SiteDriver.FindElements(DefaultPageObjects.SwitchClientListCssLocator, How.CssSelector,
                "Text");
        }

        public bool IsMostRecentPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.MostRecentHeaderXpath, How.XPath);
        }

        public bool IsAllClientsPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.AllClientsHeaderXpath, How.XPath);
        }

        public List<string> GetClientCodesOrClientNames(bool mostRecentClients, bool getClientNames)
        {
            var headerName = mostRecentClients ? "Most Recent" : "All Clients";
            var clientCodeOrName = getClientNames ? 2 : 1;

            var list = JavaScriptExecutor.FindElements(Format(DefaultPageObjects.MostRecentClientsCodesCss, headerName, clientCodeOrName),
                "Text");
            return list;
        }


        public void TypeAheadInSwitchClientPage(ClientEnum clientName)
        {
            SiteDriver.FindElement<TextField>(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector).Clear()
                .SendKeys(clientName.GetStringValue());
        }

        public QuickLaunchPage SwitchClientTo(ClientEnum clientName, bool isPopup = false, bool typeAhead = false)
        {
            Console.WriteLine("Switching client to " + clientName.ToString());

            if (!SiteDriver.IsElementPresent(DefaultPageObjects.SwitchClientListCssLocator, How.CssSelector))
                ClickOnSwitchClient();

            var switchClient = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                if (typeAhead)
                    TypeAheadInSwitchClientPage(clientName);

                SiteDriver.FindElement<Section>(Format(DefaultPageObjects.SwitchClientListValueXPathTemplate,
                        clientName.ToString()), How.XPath).Click();

                if (isPopup)
                {
                    ClickOkCancelOnConfirmationModal(true);
                }
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector), 5000);
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector), 10000);


                SiteDriver.WaitForPageToLoad();

            });
            try
            {
                ClickOnPageFooter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Page Header is overlapped by the menu dropdown \n" + e.Message);
            }
            return new QuickLaunchPage(Navigator, switchClient);
        }

        public bool IsSwitchClientIconPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector);
        }

        public bool IsSwitchClientCaretSignPresent() =>
            SiteDriver.IsElementPresent(DefaultPageObjects.SwitchClientCaretIconCssSelector, How.CssSelector);

        public bool IsValueCurrency(string value)
        {
            return value.Equals($"{value:C}");
        }


        public bool IsCorrectClientLogoDisplayed(ClientEnum clientName)
        {
            var logovalue = GetClientLogoFromDB(clientName.ToString());
            var client = SiteDriver.FindElement<Link>(DefaultPageObjects.ClientSmallBrandingImgId, How.Id);
            return client.GetAttribute("src").Contains(logovalue);

        }

        public bool IsCorrectClientLogoDisplayedForEmberPgae(ClientEnum clientName)
        {

            var logovalue = GetClientLogoFromDB(clientName.ToString());
            var client = SiteDriver.FindElement<Link>(DefaultPageObjects.ClientSmallBrandingCssLocator, How.CssSelector);

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
            if (CurrentPageUrl.Contains(PageUrlEnum.Microstrategy.GetStringValue()))
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.LogOutButtonXPath, How.XPath);
            else
                SiteDriver.FindElement<Section>(DefaultPageObjects.LogOutButtonXPath, How.XPath).Click();
            AcceptAlertBoxIfPresent();
            Console.WriteLine("Clicked Logout Button.");
            return new LoginPage(Navigator, new LoginPageObjects());
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
                Console.WriteLine("Alert Box Accepted");
            }
            else
            {
                Console.WriteLine("Alert Box wasn't found.");
            }
        }

        public bool IsLogoutIconPresent()
        {

            return SiteDriver.IsElementPresent(DefaultPageObjects.LogOutButtonXPath, How.XPath);

        }

        public static void ClickOnPageFooter()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.NucleusFooterId, How.Id), 5000);
            SiteDriver.FindElement<Section>(DefaultPageObjects.NucleusFooterId, How.Id).Click();
        }

        public void ClickOnLogo()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.NucleusLogoId, How.Id), 5000);
            SiteDriver.FindElement<Section>(DefaultPageObjects.NucleusLogoId, How.Id).Click();
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector), 10000);
        }

        public void MouseOverOnUserMenu()
        {
            SiteDriver.MouseOver(DefaultPageObjects.UserMenuId, How.Id);
        }
        public void MouseOverManagerMenu()
        {
            SiteDriver.MouseOver(DefaultPageObjects.ManagerTab, How.CssSelector);
        }
        public void MouseOverOnQuickLinks()
        {
            SiteDriver.MouseOver(DefaultPageObjects.QuickLinksXPath, How.XPath, release: true);
        }

        public void MouseOverOnCotivitiLinks()
        {
            SiteDriver.MouseOver(DefaultPageObjects.CotivitiLinksCssSelector, How.CssSelector, release: true);
        }
        #endregion

        #region HELPCENTER

        public HelpCenterPage NavigateToHelpCenter(bool popUp = false)
        {
            MouseOverOnUserMenu();

            _defaultPage.HelpCenterButton.Click();

            if (popUp)
                ClickOkCancelOnConfirmationModal(true);

            ClickOnPageFooter();
            return new HelpCenterPage(Navigator, new HelpCenterPageObjects());
        }


        public bool IsHelpCenterIconPresent()
        {
            MouseOverOnUserMenu();

            var isHelpCenterPresent = SiteDriver.IsElementPresent(DefaultPageObjects.HelpCenterXPath, How.XPath);
            ClickOnPageFooter();
            return isHelpCenterPresent;
        }

        #endregion

        #region QUICKLAUNCH

        public QuickLaunchPage SwitchTabAndOpenQuickLaunchPageCloseFirstTab(Func<QuickLaunchPage> Login)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(_browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() => { Login(); });
            return new QuickLaunchPage(Navigator, quickLaunch);
        }

        public QuickLaunchPage ClickOnQuickLaunch(bool popUp = false)
        {
            //MouseOverOnUserMenu();


            //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.QuickLaunchButtonXPath, How.XPath));
            var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                ClickOnLogo();
                if (popUp)
                {
                    Navigator.AcceptAlertBox();
                }
            });
            ClickOnPageFooter();
            return new QuickLaunchPage(Navigator, quickLaunch);
        }

        public bool IsQuickLaunchIconPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.QuickLaunchButtonXPath, How.XPath);
        }

        public string GetSubMenuOption(int menuposition, int submenuposition)
        {
            return SubMenu.GetSubMenuOption(menuposition, submenuposition);
        }

        public string GetHeaderMenuText(int headerMenuPosition)
        {
            return SiteDriver.FindElement<TextField>(
                string.Format(QuickLaunchPageObjects.HeaderMenuXpathLocatorTemplate, headerMenuPosition), How.XPath).Text;
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
        public ClaimSearchPage NavigateToClaimSearch()
        {
            var claimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                Mouseover.MouseOverClaimMenu();
                ClickOnNormalizedMenu(SubMenu.ClaimSearch);
                if (IsPageErrorPopupModalPresent())
                    ClickOkCancelOnConfirmationModal(true);

                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));

            });

            return new ClaimSearchPage(Navigator, claimSearchPage);
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

            return new ClaimSearchPage(Navigator, claimSearchPage);
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
                    Mouseover.
                        IsFciClaimsWorkList())
                    ClickOnMenu(SubMenu.FciClaimsWorkList);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }

        /// <summary>
        /// Claim => CV Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToPciClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.
                        IsPciClaimsWorkList())
                    ClickOnMenu(SubMenu.PciClaimsWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForIe(7000);

            });
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }


        /// <summary>
        /// Claim => DCI Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToDciClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
                {
                    ClickOnMenu(SubMenu.DciClaimsWorkList);
                    SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                    SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                }

            );
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }

        /// <summary>
        /// Claim => FFP Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToFfpClaimsWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
                {
                    ClickOnMenu(SubMenu.FfpClaimsWorkList);
                    SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                    SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                }

            );
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }


        public ClaimActionPage NavigateToPciClaimsPatientHistoryPage()
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.
                        IsPciClaimsWorkList())
                    ClickOnMenu(SubMenu.PciClaimsWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForIe(7000);

            });
            return new ClaimActionPage(Navigator, newClaimActionPage);
        }
        /// <summary>
        /// Claim => CV Claims Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToCvQcWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsCVQCClaims())
                    ClickOnMenu(SubMenu.CVQCWorkList);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }

        /// <summary>
        /// Claim => CV RN Work List
        /// </summary>
        /// <returns>Claim Search</returns>
        public ClaimActionPage NavigateToPciRnWorkList(bool handlePopup = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.IsPciRnWorkList())
                    ClickOnMenu(SubMenu.PciRnWorkList);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, handlePopup);
        }

        public BillActionPage NavigateToPciBillWorkList()
        {
            var billActionPage = Navigator.Navigate<BillActionPageObjects>(() =>
            {
                if (
                    Mouseover.
                        IsPciBillWorkList())
                    ClickOnMenu(SubMenu.PciBillWorkList);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));

            });
            return new BillActionPage(Navigator, billActionPage);
        }

        public RepositoryPage NavigateToRepository()
        {
            var repositoryPage = Navigator.Navigate<RepositoryPageObjects>(() =>
            {
                Mouseover.IsReferenceRepository();
                ClickOnMenu(SubMenu.Repository);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

            });

            return new RepositoryPage(Navigator, repositoryPage);
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

            return new ReferenceManagerPage(Navigator, referenceManagerPage);
        }


        /// <summary>
        /// Invoice => New Invoice Search
        /// </summary>
        /// <returns>New Invoice Search page</returns>
        //public InvoiceSearchPage NavigateToInvoiceSearch()
        //{
        //    var newInvoiceSearchPage = Navigator.Navigate<InvoiceSearchPageObjects>(() =>
        //    {
        //        if (Mouseover.IsNewInvoiceSearch())
        //            ClickOnMenu(SubMenu.InvoiceSearch);
        //        SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
        //        WaitForPageToLoadWithSideBarPanel();
        //    });

        //    return new InvoiceSearchPage(Navigator, newInvoiceSearchPage);
        //}


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
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
            });

            return new AppealActionPage(Navigator, newAppealActionPage, handlePopup);
        }

        ///// <summary>
        ///// Provider => Provider Search
        ///// </summary>
        ///// <returns>Provider Search</returns>
        //public ProviderSearchPage NavigateToProviderSearch()
        //{
        //    var providerSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
        //    {
        //        if (Mouseover.IsProviderSearch())
        //            ClickOnMenu(SubMenu.ProviderSearch);
        //    });
        //    return new ProviderSearchPage(Navigator, providerSearchPage);
        //}


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
            return new ProfileManagerPage(Navigator, profileManagerPage);
        }

        /// <summary>
        /// Settings => User => MyProfile
        /// </summary>
        /// <returns>Client Search</returns>
        /*public MyProfilePage NavigateToMyProfilePageFromMenu()
        {
            var profileManagerPage = Navigator.Navigate<MyProfilePageObjects>(() =>
            {
                if (Mouseover.IsSettingsUserMyProfile())
                {
                    ClickOnMenu(SubMenu.MyProfile);
                }
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new MyProfilePage(Navigator, profileManagerPage);
        }*/

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
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

            });

            return new AppealRationaleManagerPage(Navigator, newAppealRationaleManagerPage);
        }


        public MyProfilePage NavigateToMyProfilePage()
        {
            var myProfilePage = Navigator.Navigate<MyProfilePageObjects>(() =>
            {
                MouseOverOnUserMenu();
                SiteDriver.FindElement<ImageButton>(DefaultPageObjects.MyProfileIconXPath, How.XPath).Click();
                SiteDriver.WaitForPageToLoad();
                ClickOnPageFooter();

                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

            });

            return new MyProfilePage(Navigator, myProfilePage);
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
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });

            return new AppealCategoryManagerPage(Navigator, appealCategoryManagerPage);
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
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });

            return new MaintenanceNoticesPage(Navigator, maintenanceNoticesPage);
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
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            });

            return new AppealCreatorPage(Navigator, appealCreatorPage);
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
            SiteDriver.WaitForJqueryStatusCondition();
            WaitForPageToLoadWithSideBarPanel();
            return new AppealSearchPage(Navigator, newappealSearchPage);
        }


        public DashboardPage NavigateToDashboard()
        {
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnDashboardIcon);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            if (IsPageErrorPopupModalPresent())
            {
                EnvironmentManager.Instance.CaptureScreenShot("Dashbord_issue_popup");
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }
            EnvironmentManager.Instance.CaptureScreenShot("Dashbord_page_load_after");
            if (!SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector))
                return new DashboardPage(Navigator, dashboardPage);
            Console.WriteLine("Dashboard does not load sucessfully");
            RefreshPage(false);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            EnvironmentManager.Instance.CaptureScreenShot("Dashbord_page_load_after1");
            return new DashboardPage(Navigator, dashboardPage);
        }

        public DashboardPage NavigateToFFPDashboard()
        {
            //TruncateDashboardClaimFFPTable();
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnFFPDashboard);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage);
        }

        public DashboardPage NavigateToCVDashboard()
        {
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnCVDashboard);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage);
        }

        public DashboardPage NavigateToCOBDashboard()
        {
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnCOBDashboard);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage);
        }

        public COBAppealsDetailPage NavigateToCobAppealsDetailPage()
        {
            NavigateToCOBDashboard();
            var cobAppealsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate, DashboardOverviewTitlesEnum.AppealsOverview.GetStringValue())).Click();
                Console.WriteLine("Clicked Appeals Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.COBAppealsDetail.GetStringValue()));
            }, () => new COBAppealsDetailPageObjects(PageUrlEnum.COBAppealsDetail.GetStringValue()));
            WaitForStaticTime(5000);
            return new COBAppealsDetailPage(Navigator, cobAppealsDetailPage);
        }

        public COBClaimsDetailPage NavigateToCobClaimsDetailPage()
        {
            NavigateToCOBDashboard();
            var cobClaimsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate, DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue())).Click();
                Console.WriteLine("Clicked Claims Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue()));
            }, () => new COBClaimsDetailpageObject(PageUrlEnum.COBClaimsDetail.GetStringValue()));
            WaitForStaticTime(5000);
            return new COBClaimsDetailPage(Navigator, cobClaimsDetailPage);

        }

        private void ClickOnCOBDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }


            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.COBProductCssLocator);
            Console.WriteLine("Clicked on COB Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.FindElement(DashboardPageObjects.COBProductCssLocator).Click();
        }

        private void ClickOnFFPDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }


            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.FFPProductCssLocator);
            Console.WriteLine("Clicked on CV Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.FindElement(DashboardPageObjects.FFPProductCssLocator).Click();
        }

        private void ClickOnCVDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.PCIProductCssLocator);
            Console.WriteLine("Clicked on CV Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
        }

        public DashboardPage NavigateToMyDashboard()
        {
            //TruncateDashboardClaimFFPTable();
            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(ClickOnMyDashboard);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage);
        }

        private void ClickOnMyDashboard()
        {
            ClickOnDashboardIcon();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);

            if (IsPageErrorPopupModalPresent())
            {
                EnvironmentManager.Instance.CaptureScreenShot("My_dashboard_issue_before");
                ClosePageError();
                RefreshPage(false);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            }

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MyDashboardCssLocator);
            Console.WriteLine("Clicked on My Dashboard Filter.");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                JavaScriptExecutor.IsElementPresent(
                    string.Format(DashboardPageObjects.MyDashboardWidgetCssLocatorTemplate, "Claim Performance")));
        }

        private void ClickOnMicrostrategy()
        {
            JavaScriptExecutor.ExecuteClick(DashboardPageObjects.DashboardIconByCss, How.CssSelector);
            Console.WriteLine("Clicked on Dashboard icon.");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector), 2000);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));

            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MicrostrategyDashboardCssLocator);
            SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
            SiteDriver.SwitchBackToMainFrame();
            Console.WriteLine("Clicked on Microstrategy Filter.");
        }

        private void clickonMicrostrategyToViewListOfReports()
        {

            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            JavaScriptExecutor.ExecuteClick(DashboardPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Filter Dashboard icon.");
            SiteDriver.WaitForCondition(() => JavaScriptExecutor.IsElementPresent(DashboardPageObjects.MicrostrategyDashboardCssLocator));
            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MicrostrategyDashboardCssLocator);

        }

        //public void TruncateDashboardClaimFFPTable()
        //{
        //    _executor.ExecuteQuery(DashboardSqlScriptObjects.TruncateDashboardClaimFFP);
        //    Console.WriteLine("Truncate Dashboard table to get fresh data in page load");
        //}
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
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealManagerPageObjects.PageHeaderCssLocator, How.CssSelector));
            });

            return new AppealManagerPage(Navigator, appealCreatorPage);
        }



        public AppealSummaryPage SwitchTabAndOpenAppealSummaryByUrlInClientUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(_browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsClientUser();
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.Open(url));

            return new AppealSummaryPage(Navigator, appealSummary);
        }

        public AppealSummaryPage SwitchTabAndOpenAppealSummaryByUrlInCotivitiUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(_browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsHciAdminUser();
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.Open(url));

            return new AppealSummaryPage(Navigator, appealSummary);
        }

        public AppealActionPage SwitchTabAndOpenAppealActionByUrl(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(_browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsHciAdminUser();
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));

            return new AppealActionPage(Navigator, newAppealAction);
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
            return new AppealSummaryPage(Navigator, appealSummary);
        }

        public ClaimActionPage SwitchTabAndOpenNewClaimActionByUrlByClaimViewRestrictedUser(string url)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(_browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsClaimViewRestrictionUser();
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            WaitForWorkingAjaxMessage();
            return new ClaimActionPage(Navigator, newClaimAction);
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

            return new ProfileManagerPage(Navigator, new ProfileManagerPageObjects());
        }

        /// <summary>
        /// QA => QA Manager
        /// </summary>
        /// <returns>QA Manager</returns>
        public QaManagerPage NavigateToQaManager()
        {
            var qaManagerPage = Navigator.Navigate<QaManagerPageObjects>(() =>
            {
                if (Mouseover.IsAnalystManager())
                    ClickOnMenu(SubMenu.QaManager);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(QaManagerPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaManagerPage(Navigator, qaManagerPage);
        }


        /// <summary>
        /// QA => QA Claim search
        /// </summary>
        /// <returns>QA Claim search</returns>
        public QaClaimSearchPage NavigateToQaClaimSearch()
        {
            var qaClaimSearchPage = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                if (Mouseover.IsQAClaimSearch())
                    ClickOnMenu(SubMenu.QaClaimSearch);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(QaClaimSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaClaimSearchPage(Navigator, qaClaimSearchPage);
        }


        public QaAppealSearchPage NavigateToQaAppealSearch()
        {
            var qaAppealSearchPage = Navigator.Navigate<QaAppealSearchPageObjects>(() =>
            {
                if (Mouseover.IsQAAppealSearch())
                    ClickOnMenu(SubMenu.QaAppealSearch);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(QaAppealSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });
            return new QaAppealSearchPage(Navigator, qaAppealSearchPage);
        }

        public ProviderSearchPage NavigateToProviderSearch()
        {
            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                if (Mouseover.IsProviderSearch())
                    ClickOnMenu(SubMenu.ProviderSearch);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage);
        }

        public SuspectProvidersPage NavigateToSuspectProviders()
        {
            var suspectProvidersPage = Navigator.Navigate<SuspectProvidersPageObjects>(() =>
            {
                if (Mouseover.IsSuspectProviders())
                    ClickOnMenu(SubMenu.SuspectProviders);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(SuspectProvidersPageObjects.SpinnerCssLocator, How.CssSelector));
                WaitForStaticTime(4000);
            });
            return new SuspectProvidersPage(Navigator, suspectProvidersPage);
        }

        public ProviderActionPage NavigateToSuspectProvidersWorkList()
        {
            var providerActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                if (Mouseover.IsSuspectProvidersWorkList())
                    ClickOnMenu(SubMenu.SuspectProvidersWorkList);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                WaitForSpinner();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProviderActionPageObjects.BasicProviderDetailsDivCssSelector, How.CssSelector));

            });
            return new ProviderActionPage(Navigator, providerActionPage);
        }


        public DefaultPage NavigateToAPage(string menuString, string subMenuString)
        {
            var pageToReturn = Navigator.Navigate<DefaultPageObjects>(() =>
            {
                if (Mouseover.IsParticularPage(menuString, subMenuString))
                    ClickOnMenu(subMenuString);
                SiteDriver.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                WaitForWorking();
            });
            return new DefaultPage(Navigator, pageToReturn);

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
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
            return new MicrostrategyPage(Navigator, microstrategyPage);
        }

        public MicrostrategyPage NavigateToMicrostrategyWithMultipleReports()
        {
            var microstrategyPage = Navigator.Navigate<MicroStrategyPageObjects>(clickonMicrostrategyToViewListOfReports);
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
            return new MicrostrategyPage(Navigator, microstrategyPage);
        }




        #endregion

        public UserProfileSearchPage NavigateToCreateNewUserAccount()
        {
            _newUserProfileSearch = NavigateToNewUserProfileSearch();
            _newUserProfileSearch.NavigateToCreateNewUser();
            return new UserProfileSearchPage(Navigator, new UserProfileSearchPageObjects());

        }

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

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void WaitForLogoutIcon()
        {
            SiteDriver.WaitForCondition(() => IsLogoutIconPresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void WaitForWorkingAjaxMessageForBothDisplayAndHide()
        {
            SiteDriver.WaitForCondition(IsWorkingAjaxMessagePresent, 5000);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void WaitForSpinner()
        {
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
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

        public void SwitchToClaimActionByUrl()
        {
            SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimAction.GetStringValue());
        }

        public bool CheckIfPdfDownloadPageIsOpenAndCloseAndReturnToFirstWindow()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count == 0) return false;
            SiteDriver.SwitchWindow(handles[1]);
            var isPdfOpen = SiteDriver.FindElement<TextLabel>("body>embed", How.CssSelector).GetAttribute("type")
                .Equals("application/pdf");
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindow(handles[0]);
            return isPdfOpen;
        }

        public ClaimActionPage SwitchToOpenNewClaimActionByUrl(string url, string user)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(browserOptions.ApplicationUrl);
                if (SiteDriver.IsAlertBoxPresent())
                {
                    SiteDriver.AcceptAlertBox();
                }
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            if (user == EnvironmentManager.Instance.HciAdminUsername1)
                new LoginPage(Navigator, login).LoginAsHciAdminUser1();
            else
                new LoginPage(Navigator, login).LoginAsHciAdminUser();
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newClaimAction = new ClaimActionPage(Navigator, newClaimActionPage);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return newClaimAction;
        }

        public ClaimActionPage SwitchToOpenNewClaimActionByUrlForAdmin1FromLogicSearch(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsHciAdminUser1();
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newClaimAction = new ClaimActionPage(Navigator, newClaimActionPage);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //newClaimAction.HandleAutomaticallyOpenedLogicManagerPopup();
            return newClaimAction;
        }
        public ClaimActionPage SwitchToOpenNewClaimActionByUrlForClientUserFromLogicSearchClient(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login).LoginAsClientUser();
            var claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var claimAction = new ClaimActionPage(Navigator, claimActionPage);
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
                    IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                    SiteDriver.Open(browserOptions.ApplicationUrl + PageUrlEnum.MyProfileSettings.GetStringValue());
                    SiteDriver.WaitForCondition(
                        () => SiteDriver.SwitchWindowByUrl(PageUrlEnum.MyProfileSettings.GetStringValue()));
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
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
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                SiteDriver.Open(browserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });
            if (user == 1)
                new LoginPage(Navigator, login).LoginAsHciAdminUser();
            else if (user == 2)
                new LoginPage(Navigator, login).LoginAsHciAdminUser1();
            else if (user == 5)
                new LoginPage(Navigator, login).LoginAsHciAdminUserClaim5();
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            var newAppealAction = new AppealActionPage(Navigator, newAppealActionPage);
            return newAppealAction;
        }

        public bool IsCIWIconDisplayed()
        {
            return _defaultPage.DownloadCIWIcon.Displayed;
        }

        public string GetCIWIconTooltip()
        {
            return _defaultPage.DownloadCIWIcon.GetAttribute("title");
        }

        public void WaitForPageToLoadWithSideBarPanel()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
        }

        /// <summary>
        /// Batch => New Batch Search
        /// </summary>
        /// <returns>New Batch page</returns>
        public BatchSearchPage NavigateToBatchSearch()
        {
            var newBatchSearchPage = Navigator.Navigate<BatchSearchPageObjects>(() =>
            {
                if (Mouseover.IsBatchSearch())
                    ClickOnMenu(SubMenu.BatchSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new BatchSearchPage(Navigator, newBatchSearchPage);
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
                    if (Mouseover.IsNewClientSearch())
                        ClickOnMenu(SubMenu.ClientSearch);
                }
                else
                {
                    ClickOnMenu(SubMenu.ClientSearch);
                }
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new ClientSearchPage(Navigator, newClientSearchPage);
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
                if (Mouseover.IsSettingsNewUserProfileSearch())
                    ClickOnMenu(SubMenu.NewUserProfileSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                if (waitForSideBar)
                    WaitForPageToLoadWithSideBarPanel();
            });

            return new UserProfileSearchPage(Navigator, newUserProfileSearchPage);
        }

        public RoleManagerPage NavigateToRoleManager()
        {
            var newRoleManagerPage = Navigator.Navigate<RoleManagerPageObject>(() =>
            {
                if (Mouseover.IsSettingsRoleManager())
                    ClickOnMenu(SubMenu.RoleManager);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new RoleManagerPage(Navigator, newRoleManagerPage);
        }


        public LogicSearchPage NavigateToLogicSearch()
        {
            var newLogicSearchPage = Navigator.Navigate<LogicSearchPageObjects>(() =>
            {
                if (Mouseover.IsLogicSearch())
                    ClickOnMenu(SubMenu.LogicSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

                WaitForPageToLoadWithSideBarPanel();
            });

            return new LogicSearchPage(Navigator, newLogicSearchPage);
        }
        public MicrostrategyMaintenancePage NavigateToMicrostrategyMaintenance()
        {
            var newMicrostratergyPage = Navigator.Navigate<MicrostrategyMaintenancePageObjects>(() =>
              {
                  if (Mouseover.IsMicrostrategyMaintainacePresent())
                      ClickOnMenu(SubMenu.MicrostrategyMaintenance);
                  //SiteDriver.WaitForCondition(JavaScriptExecutors.ExecuteJqueryStatus);
                  SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
                  SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

              }
            );
            return new MicrostrategyMaintenancePage(Navigator, newMicrostratergyPage);

        }


        /// <summary>
        /// PreAuthorization => Pre-Auth Creator Page
        /// </summary>
        /// <returns>Pre-Auth Creator Page</returns>
        public PreAuthCreatorPage NavigateToPreAuthCreatorPage()
        {
            var newPreAuthCreatorPage = Navigator.Navigate<PreAuthCreatorPageObjects>(() =>
            {
                if (Mouseover.IsPreAuthCreatorPresent())
                    ClickOnMenu(SubMenu.PreAuthCreator);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);

            });

            return new PreAuthCreatorPage(Navigator, newPreAuthCreatorPage);
        }




        /// <summary>
        /// Claim => CV Coders Claim
        /// </summary>
        /// <returns>CV Coders Claims</returns>
        public ClaimActionPage NavigateToPciCodersClaim(bool handlePopup = false, bool isFromRetroPage = false)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (
                    Mouseover.
                        IsPciCodersClaim())
                    ClickOnMenu(SubMenu.PciCodersClaims);
                Console.WriteLine("First Wait");
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                Console.WriteLine("Second Wait");
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
                Console.WriteLine("Third Wait");
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
                EnvironmentManager.Instance.CaptureScreenShot("pcicoder");

            });
            return new ClaimActionPage(Navigator, newClaimActionPage);
        }

        public QuickLaunchPage SwitchTabAndNavigateToQuickLaunchPage(bool changeUserToAdmin = false, int tabno = 1)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();


            QuickLaunchPageObjects redirectPage = Navigator.Navigate<QuickLaunchPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[tabno]);
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                if (changeUserToAdmin)
                {

                    SiteDriver.Open(browserOptions.ApplicationUrl);
                    new LoginPage(Navigator, new LoginPageObjects()).LoginAsHciAdminUser();

                }
                else
                {
                    SiteDriver.Open(browserOptions.ApplicationUrl + PageUrlEnum.QuickLaunch.GetStringValue());
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector), 10000);
                }

            });

            return new QuickLaunchPage(Navigator, redirectPage);

        }

        public UserProfileSearchPage SearchByUserIdToNavigateToUserSettingsForm(string userId)
        {
            NavigateToNewUserProfileSearch();
            _getSideBarPanelSearch.OpenSidebarPanel();
            _getSideBarPanelSearch.SetInputFieldByLabel("User ID", userId);
            _getSideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
            _getGridViewSection.ClickOnGridRowByRow();
            return new UserProfileSearchPage(Navigator, new UserProfileSearchPageObjects());
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
                if (Mouseover.IsPreAuthSearchPresent())
                    ClickOnMenu(SubMenu.PreAuthSearch);
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                WaitForPageToLoadWithSideBarPanel();
            });

            return new PreAuthSearchPage(Navigator, preAuthSearchPage);
        }

        public EditSettingsManagerPage NavigateToEditSettingsManager(bool mouseOver = true, bool isInactive = false)
        {
            var editSettingsManagerPage = Navigator.Navigate<EditSettingsManagerPageObject>(() =>
            {
                if (mouseOver)
                {
                    if (Mouseover.IsNewClientSearch())
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

            return new EditSettingsManagerPage(Navigator, editSettingsManagerPage);
        }

        public ProviderSearchPage SwitchTabAndNavigateToProviderSearchPage(string url, bool changeUserToAdmin = false)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();

            ProviderSearchPageObjects redirectPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                IBrowserOptions browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
                if (changeUserToAdmin)
                {

                    SiteDriver.Open(url);
                    new LoginPage(Navigator, new LoginPageObjects()).LoginAsHciAdminUser();

                }
                else
                {
                    SiteDriver.Open(url);
                    SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProviderSearchPageObjects.PageHeaderCssLocator, How.CssSelector));
                }

            });

            return new ProviderSearchPage(Navigator, redirectPage);

        }

        public ClientSearchPage ClickOnReturnToClientSearch()
        {
            SiteDriver.FindElement<Section>(EditSettingsManagerPageObject.ReturnToClientSearchCssSelector,
                How.CssSelector).Click();
            return new ClientSearchPage(Navigator, new ClientSearchPageObjects());
        }

        public bool IsUserNameGreetingMenuPresent()
        {
            MouseOverOnUserMenu();
            return SiteDriver.IsElementPresent(DefaultPageObjects.UserMenuId, How.Id);
        }
        public bool IsManagerMenuPresent()
        {
            return JavaScriptExecutor.IsElementPresent(DefaultPageObjects.ManagerTabByCss);

        }

        public List<string> GetManagerSubMenu()
        {
            MouseOverManagerMenu();
            return JavaScriptExecutor.FindElements(DefaultPageObjects.ManagerTabSubMenu);
        }

        public string GetUserNameGreetingMenuDropdownLabel()
        {
            return
                SiteDriver.FindElement<TextLabel>(DefaultPageObjects.UserGreetingMenuLabel, How.CssSelector)
                    .Text;
        }

        public List<string> GetUserNameGreetingMenuDropdownOptions()
        {
            MouseOverOnUserMenu();
            var list = JavaScriptExecutor.FindElements(DefaultPageObjects.UserGreetingMenuList, "Text");
            return list;

        }

        public List<string> GetQuickLinksMenuOptions()
        {
            MouseOverQuickLinks();
            var list =
                JavaScriptExecutor.FindElements(DefaultPageObjects.QuickLinksOptionsCssSelector, "Text");
            return list;
        }

        public List<string> GetCotivitiLinksMenuOptions()
        {
            MouseOverCotivitiLinks();
            var list = JavaScriptExecutor.FindElements(DefaultPageObjects.CotivitiLinksOptionsCssSelector, "Text");
            return list;
        }

        public List<string> GetTopNavigationMenuOptions()
        {
            var list = JavaScriptExecutor.FindElements(DefaultPageObjects.TopNavMenuOptions, "Text");
            return list;
        }


        public ProviderSearchPage ClickOnProviderSearchLinkAndNavigateToProviderSearchPage(string linkName)
        {

            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.FindElement<Link>(String.Format(DefaultPageObjects.CotivitiLinksByLinkNameXPath, linkName), How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    !SiteDriver.IsElementPresent(ProviderSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector));
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage);
        }



        public void ClickOnCotivitiLinksByLinkName(string linkName) =>
            SiteDriver.FindElement<Link>(string.Format(DefaultPageObjects.CotivitiLinksByLinkNameXPath, linkName),
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




        #endregion



        #region PRIVATE/PROTECTED METHODS

        private static void ClickOnMenu(String menuOption)
        {
            var menuToClick = HeaderMenu.GetElementLocatorTemplate(menuOption);
            JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            Console.WriteLine("Navigated to {0}", menuOption);
            try
            {
                ClickOnPageFooter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Page Header is overlapped by the menu dropdown \n" + e.Message);
            }

            if (SiteDriver.IsElementPresent(menuToClick, How.XPath))
                JavaScriptExecutor.ExecuteMouseOut(menuToClick, How.XPath);
        }

        private static void ClickOnNormalizedMenu(String menuOption)
        {
            var menuToClick = HeaderMenu.GetNormalizedElementLocatorTemplate(menuOption);
            JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            Console.WriteLine("Navigated to {0}", menuOption);
            if (SiteDriver.IsElementPresent(menuToClick, How.XPath))
                JavaScriptExecutor.ExecuteMouseOut(menuToClick, How.XPath);
        }

        public void ClickOnDashboardIcon()
        {
            MouseOverOnUserMenu();
            SiteDriver.FindElement<ImageButton>(DefaultPageObjects.DashboardIconXPath, How.XPath).Click();
            SiteDriver.WaitForPageToLoad();
            ClickOnPageFooter();
            Console.WriteLine("Clicked on Dashboard icon.");
        }

        public MicrostrategyReportPage ClickOnDashboardIconToNavigateToMicrostrategyHome()
        {
            var microstrategyPage = Navigator.Navigate<MicrostrategyReportPageObjects>(() =>
            {
                ClickOnDashboardIcon();
                Console.WriteLine("Clicked on Dashboard icon.");
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicrostrategyReportPageObjects.DashboardLabelCssSelector, How.CssSelector));
                SiteDriver.WaitToLoadNew(2000);
                SiteDriver.WaitForPageToLoad();
                //SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
                //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.SwitchBackToMainFrame();

            });

            return new MicrostrategyReportPage(Navigator, microstrategyPage);

        }

        public MicrostrategyPage ClickOnDashboardIconToNavigateToMicrostrategy()
        {
            var microstrategyPage = Navigator.Navigate<MicroStrategyPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.DashboardIconByCss, How.CssSelector);
                Console.WriteLine("Clicked on Dashboard icon.");
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
                SiteDriver.WaitToLoadNew(2000);
                SiteDriver.WaitForPageToLoad();
                //SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
                //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.SwitchBackToMainFrame();
            });

            return new MicrostrategyPage(Navigator, microstrategyPage);
        }

        protected int GetElementHeight(BaseElement webElement)
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
            SiteDriver.FindElement<Section>(DefaultPageObjects.EditSettingsIcon, How.CssSelector).Click();
            WaitForPageToLoad();
        }

        #endregion

        #region Invalid Input

        private const string InvalidInputByLabelXPathSelector =
            "//label[text()='{0}']/..//input[contains(@class,'invalid')]";

        public bool IsInvalidInputPresentByLabel(string label)
        {
            bool firstCondition = SiteDriver.IsElementPresent(Format(DefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                How.XPath);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement<Section>(
                Format(DefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                How.XPath).GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public string GetInvalidInputToolTipByLabel(string label)
        {
            return SiteDriver.FindElement<Section>(
                Format(DefaultPageObjects.InvalidInputByLabelXPathTemplate, label),
                How.XPath).GetAttribute("title");
        }

        public string GetInvalidTooltipMessageOnNote()
        {
            return
                SiteDriver.FindElement<TextLabel>(DefaultPageObjects.NoteDivCssLocator,
                    How.CssSelector).GetAttribute("title");
        }

        public bool IsInvalidInputPresentOnNote()
        {
            WaitForStaticTime(1000);
            bool firstCondition = SiteDriver.IsElementPresent(DefaultPageObjects.NoteDivCssLocator,
                How.CssSelector);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement<Section>(DefaultPageObjects.NoteDivCssLocator, How.CssSelector).GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public string GetInvalidTooltipMessageOnNoteByLabel(string label)
        {
            return
                SiteDriver.FindElement<TextLabel>(Format(DefaultPageObjects.NoteDivByLabelXPathTemplate, label),
                    How.XPath).GetAttribute("title");
        }

        public bool IsInvalidInputPresentOnNoteByLabel(string label)
        {
            bool firstCondition = SiteDriver.IsElementPresent(Format(DefaultPageObjects.NoteDivByLabelXPathTemplate, label),
                How.XPath);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement<Section>(Format(DefaultPageObjects.NoteDivByLabelXPathTemplate, label), How.XPath).GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        // Check for Invalid highlight for Transfer Components (eg. click and assign roles)
        public bool IsInvalidInputPresentOnTransferComponentByLabel(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(Format(DefaultPageObjects.TransferComponentByLabelCssSelector, label));
            var secondCondition = false;

            if (firstCondition)
                secondCondition = JavaScriptExecutor
                    .FindElement(Format(DefaultPageObjects.TransferComponentByLabelCssSelector, label))
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public bool IsValidInputPresentOnTransferComponentByLabel(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(Format(DefaultPageObjects.DisplayedTransferComponentByLabelCssSelector, label));

            return firstCondition;
        }


        public bool IsInvalidDropdownInputFieldPresent(string label)
        {
            bool firstCondition =
                JavaScriptExecutor.IsElementPresent(Format(DefaultPageObjects.InvalidDropdownInputFieldCssSelector, label));
            var secondCondition = false;

            if (firstCondition)
                secondCondition = JavaScriptExecutor
                    .FindElement(Format(DefaultPageObjects.InvalidDropdownInputFieldCssSelector, label))
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)"); ;
            return firstCondition && secondCondition;
        }

        public int GetCountOfInvalidRed()
        {
            return SiteDriver.FindElementsCount(DefaultPageObjects.AllInvalidCssLocator, How.CssSelector);
        }

        #endregion


        #region Available/Assigned 

        public bool IsRoleAssigned<T>(List<string> userId, string roleName, bool isSearchedByUserId = true, bool isAssigned = true, bool isDefaultPageUserProfile = false)
        {
            var target = typeof(T);

            if (isDefaultPageUserProfile || !IsPageHeaderPresent() || GetPageHeader() != PageHeaderEnum.UserProfileSearch.GetStringValue())
                _newUserProfileSearch = NavigateToNewUserProfileSearch();
            else
            {
                _newUserProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                _newUserProfileSearch.SideBarPanelSearch.ClickOnClearLink();
            }

            _newUserProfileSearch.SearchUserByNameOrId(userId, isSearchedByUserId, !isSearchedByUserId);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(isSearchedByUserId ? userId[0] : userId[0] + " " + userId[1]);

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

            }
            if (isAvailableAssigned)
                SiteDriver.FindElement<TextLabel>(Format(DefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Available {headerLabel}", value), How.XPath).Click();
            else
                SiteDriver.FindElement<TextLabel>(Format(DefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Assigned {headerLabel}", value), How.XPath).Click();

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

            }

            if (isAvailableAssigned)
            {
                var elements = SiteDriver.FindElements(Format(DefaultPageObjects.AllAvailableListCssLocator, $"Available {headerLabel}"), How.XPath);
                ClickAllElements(elements);
            }



            else
            {
                var elements = SiteDriver.FindElements(Format(DefaultPageObjects.AllAssignedListXPath, $"Assigned {headerLabel}"), How.XPath);
                ClickAllElements(elements);
            }



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
                    ? Format(DefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Available {headerLabel}", value)
                    : Format(DefaultPageObjects.AvailableAssignedRowXPathTemplte, $"Assigned {headerLabel}", value),
                How.XPath);
        }

        public List<string> GetAvailableAssignedList(int? headerType = null, bool isAvailable = true)
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
                //    (DefaultPageObjects.AvailableAssignedListCssLocator, "left_list", $"Available {headerLabel}"), "Text");
                return JavaScriptExecutor.FindElements(Format
                    (DefaultPageObjects.AvailableAssignedListCssLocator, $"Available {headerLabel}"), "Text");
            }
            //return JavaScriptExecutors.FindElements(Format
            //    (DefaultPageObjects.AvailableAssignedListCssLocator, "right_list", $"Assigned {headerLabel}"), "Text");
            return JavaScriptExecutor.FindElements(Format
                (DefaultPageObjects.AvailableAssignedListCssLocator, $"Assigned {headerLabel}"), "Text");
        }

        public void ClickAllElements(IEnumerable<IWebElement> elements)
        {
            foreach (var ele in elements)
                ele.Click();
        }

        public bool IsAvailableAssignedClientsComponentPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(DefaultPageObjects.DisplayedTransferComponentByLabelCssSelector,
                label));

        public bool IsAvailableAssignedClientsDisabled(string header, string client) => JavaScriptExecutor.FindElement(Format(DefaultPageObjects.AvailableAssignedClientsDivCssSelectorTemplate, header, client)).GetAttribute("class").Contains("is_disabled");

        #endregion

        public ChromeDownLoadPage NavigateToChromeDownLoadPage()
        {
            var chromeDownLoad = Navigator.Navigate<ChromeDownLoadPageObjects>(() => SiteDriver.Open("Chrome://Downloads/"));
            return new ChromeDownLoadPage(Navigator, chromeDownLoad);
        }

        public string CalculateAndGetAppealDueDateFromDatabase(string turnaroundTime, string type = "Business")
        {
            if (type == "Business")
            {
                return _executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDate, turnaroundTime));
            }

            else
            {
                return _executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDateForCalendarType, turnaroundTime));

            }
        }

        public string GetAppealLevel(string claseq,ClientEnum client)
        {
            var claimSeq = claseq.Split('-'); 
                return _executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealLevel, claimSeq[0], claimSeq[1],client));

            
        }
    }
}
