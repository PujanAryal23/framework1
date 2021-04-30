using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using static System.String;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.MicroStrategy;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Logic;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.Support.Environment;
using static System.Console;

namespace Nucleus.Service.PageServices.Claim
{
    public class LogicSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly NewPagination _pagination;

        #endregion

        #region CONSTRUCTOR

        public LogicSearchPage(INewNavigator navigator, LogicSearchPageObjects logicsearchpage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager,
            IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, logicsearchpage, siteDriver, javaScriptExecutors, environmentManager, browserOptions,
                    executor)
        {
            logicsearchpage = (LogicSearchPageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);


        }

        #endregion

        #region PROPERTIES

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public NewPagination GetPagination
        {
            get { return _pagination; }
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(LogicSqlScriptObjects.GetAssignedClientList, userId));
        }

        #endregion

        #region public methods
        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public bool IsLockIconPresentForClaimSequence(string claseq)
        {
            return SiteDriver.IsElementPresent(
                string.Format(LogicSearchPageObjects.LogicSearchResultListLockForClaimXpathTemplate, claseq),
                How.XPath);
        }

        public bool IsOverDueIconPresent()
        {
            return SiteDriver.IsElementPresent(LogicSearchPageObjects.OverDueIcon, How.CssSelector);
        }

        public ClaimActionPage SearchByClaimSequenceToNavigateToNewClaimActionPage(string claimSeq, bool handlePopup = false)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                WaitForStaticTime(500);
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                WaitForStaticTime(500);
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                WaitForStaticTime(500);
                _sideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claimSeq);
                _sideBarPanelSearch.ClickOnFindButton();
                WaitForWorking();
                GetGridViewSection.ClickOnGridByRowCol();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            ,handlePopup);

        }

        public ClaimActionPage ClickonclaimseqToNavigateClaimActionPage(string claimSeq, bool handlePopup = false)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SearchByClaimSequence(claimSeq);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                GetGridViewSection.ClickOnGridByRowCol();
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
                , handlePopup);
        }

        public PreAuthActionPage ClickOnAuthSeqToNavigatePreAuthActionPage(string authSeq, bool handlePopup = false)
        {
            var preAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(LogicSearchPageObjects.PreAuthInLogicSearchXpathLocator, authSeq),
                    How.XPath).Click();
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(PreAuthActionPageObjects.PageHeaderCssLocator,
                        How.CssSelector));

            });
            return new PreAuthActionPage(Navigator, preAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
                , handlePopup);
        }

        public void SearchByClaimSequence(string claseq)
        {

            if (!_sideBarPanelSearch.IsSideBarPanelOpen()) ClickOnSidebarIcon();

            _sideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claseq);
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
        }

        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public List<string> getSecondaryDataFromDataBase(string claseq, string clasub)
        {
            var secondaryData = Executor.GetCompleteTable(string.Format(LogicSqlScriptObjects.GetSecondaryDataForLogicSearch, claseq, clasub));
            var infoList = new List<List<string>>();
            foreach (DataRow row in secondaryData)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];


        }

        public List<string> getPrimaryDataFromDataBase()
        {
            var infoList = new List<List<string>>();
            var PrimaryData = Executor.GetCompleteTable(LogicSqlScriptObjects.GetPrimaryDataForLogicSearch);
            foreach (DataRow row in PrimaryData)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];

        }

        public bool IsmodifiedDateExceedTwoHours(string claseq, string clasub)
        {
            var modifiedDateFromDatabase =
                Executor.GetSingleStringValue(
                    string.Format(LogicSqlScriptObjects.ModifiedDateForLogic, claseq, clasub));
            DateTime newdate = Convert.ToDateTime(modifiedDateFromDatabase);
            TimeSpan difSpan = DateTime.Now.Subtract(newdate);

            if ((int)difSpan.TotalHours > 2)
                return true;
            else
                return false;
        }

        public string GetTooltipMessageForOverDueIcon()
        {
            return SiteDriver.FindElement(LogicSearchPageObjects.OverDueIcon, How.CssSelector).GetAttribute("title");
        }

        public string GetToolTipMessageForLockIcon()
        {
            return SiteDriver.FindElement(LogicSearchPageObjects.LogicSearchResultListLockForClaimXpathTemplate, How.XPath).GetAttribute("title");
        }

        public void GetLockedByUserNameFromDatabase()
        {

        }

        public string GetLogicDetailHeader()
        {

            return SiteDriver
                .FindElement(LogicSearchPageObjects.LogicDetailheaderByCSS, How.CssSelector).Text;
        }

        public string GetSecondaryDetailsByLabel(string label)
        {
            return SiteDriver
                .FindElement(
                    string.Format(LogicSearchPageObjects.LogicSecondaryDetailsByCss, label), How.XPath)
                .Text;
        }

        #endregion

        public IList<String> GetActiveProductListForClient(string client)
        {
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(string.Format(LogicSqlScriptObjects.ActiveProductList, client));
            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            return newList;
        }

        public List<string> GetPciOpenLogicSearchFromDatabase(string user = "H")
        {
            return Executor.GetTableSingleColumn(String.Format(LogicSqlScriptObjects.GetprimarydataForPciOpenLogic, user));
        }

        public List<string> GetDCiOpenLogicSearchFromDatabase(string user = "H")
        {
            return Executor.GetTableSingleColumn(String.Format(LogicSqlScriptObjects.GetprimarydataForDciOpenLogic, user));
        }

        public List<string> GetFfpOpenLogicSearchFromDatabase(string user = "H")
        {
            return Executor.GetTableSingleColumn(String.Format(LogicSqlScriptObjects.GetprimarydataForFfpOpenLogic, user));
        }

        public List<String> GetlogicResultInAscendingOrder(string assignedTo)
        {
            return Executor.GetTableSingleColumn(string.Format(LogicSqlScriptObjects.LogicResultOrderedInAscending, assignedTo));
        }

        public List<String> GetlogicResultInDecscendingOrder(string assignedTo)
        {
            return Executor.GetTableSingleColumn(string.Format(LogicSqlScriptObjects.LogicResultOrderedInDecscending, assignedTo));
        }

        public List<string> GetDefaultDataForOpenLogicsFromDatabase(string user)
        {
            var infoList = new List<string>();
            var list = Executor.GetCompleteTable(
                string.Format(LogicSqlScriptObjects.GetPrimarydataForOpenLogics, user));
            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList()[0];
                infoList.Add(t);
            }

            return infoList;

        }

        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(LogicSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public string GetFilterOptionTooltip()
        {
            return SiteDriver.FindElement(LogicSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector).GetAttribute("title");
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(LogicSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(LogicSearchPageObjects.FilterOptionListByCss, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;
        }

        public List<String> GetSecondaryDetailsLabels()
        {
            return SiteDriver.FindElementsAndGetAttribute("title", LogicSearchPageObjects.DetailsLabelByCss, How.CssSelector
                );
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(String.Format(LogicSearchPageObjects.FilterOptionValueByCss, row)
                 , How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public void clickOnClearSort()
        {
            ClickOnFilterOptionListRow(6);
            Console.WriteLine("Clicked on Clear sort option.");
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            return _gridViewSection.GetGridListValueByCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder();
        }

        public List<List<string>> GetValueForAllLogicsFromDatabase(string claseq, string clasub)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(string.Format(LogicSqlScriptObjects.GetPrimaryDataForLogicSearch, claseq, clasub));
            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList;
        }

        public void DeleteClaimLock(string claimseq, string clasub)
        {
            Executor.ExecuteQuery(string.Format(LogicSqlScriptObjects.deleteclaimLockFromDatabase, claimseq, clasub));
        }

        public void DeleteLogicNoteFromDatabase(string claimSeq)
        {
            Executor.ExecuteQuery(Format(LogicSqlScriptObjects.DeleteClaimLogicInFlagOfLogicManager,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));
        }

        public LoginPage ClickOkOnConfirmationModal()
        {
            var loginPage = Navigator.Navigate<LoginPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
                WaitForWorking();
                Console.WriteLine("Ok Button is Clicked");
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(LoginPageObjects.UserIdBoxId, How.Id));
            });
            return new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public List<List<string>> GetExcelDataListFromDB(string user = "H")
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(String.Format(LogicSqlScriptObjects.GetExportlogicData, user));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;
        }
        public bool IsExportIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.ClaimsearchExportByCss, How.CssSelector);
        }



        public void ClickOnExportIcon()
        {
            JavaScriptExecutor.FindElement(LogicSearchPageObjects.ClaimsearchExportByCss).Click();
            Write("Clicked on Export option");
            WaitForStaticTime(100);
        }

        public bool IsExportIconDisabled()
        {
            return SiteDriver.IsElementPresent(LogicSearchPageObjects.DisabledExportIconCss, How.CssSelector);
        }

        public bool IsExportIconEnabled()
        {
            return SiteDriver.IsElementPresent(LogicSearchPageObjects.EnabledExportIconCss, How.CssSelector);
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<LogicSearchPage>();
            return fileName;

        }


        public List<string> GetLogicExportAuditListFromDB(string username)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExcelDownloadAudit, username));
        }

    }

}
