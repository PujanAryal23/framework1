using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Pre_Auth;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.PreAuthorization
{
    public class PreAuthSearchPage : NewDefaultPage
    {

        #region PRIVATE/PUBLIC FIELDS

        private PreAuthSearchPageObjects _preAuthSearchPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly string _originalWindow;



        #endregion

        #region CONSTRUCTOR

        public PreAuthSearchPage(INewNavigator navigator, PreAuthSearchPageObjects preAuthSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, preAuthSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _preAuthSearchPage = (PreAuthSearchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;

        }

        #endregion

        #region PUBLIC METHODS
        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }
        public PreAuthActionPage SearchByAuthSequenceAndNavigateToAuthAction(string authSequence, bool isClient = true, bool popup = true)
        {

            if (!isClient)
                GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());

            Console.Out.WriteLine("Searching for Grid Result on Auth Sequence : {0}", authSequence);
            SetAuthSequence(authSequence);
            ClickOnFindButton();
            return ClickOnAuthSeqToNavigateToPreAuthActionPage(authSequence, popup);


        }

        //public PreAuthActionPage SearchByAuthSequenceAndNavigateToAuthAction(string authSequence, bool isClient = true)
        //{
        //    if (!isClient)
        //        GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());

        //    Console.Out.WriteLine("Searching for Grid Result on Auth Sequence : {0}", authSequence);
        //    SetAuthSequence(authSequence);
        //    ClickOnFindButton();
        //    return ClickOnAuthSeqToNavigateToPreAuthActionPageAndHandlepopup(authSequence);
        //}

        public PreAuthActionPage SearchByAuthSequenceAndNavigateToAuthActionAndHandlePopup(string authSequence, bool isClient = true, bool handlePopup = true)
        {
            string pageTitle = "label.page_title";
            string authSeqLocator =
                $"div.component_item:has(li.component_data_point) span:contains(\"{authSequence}\")";
            var newPreAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                if (!isClient)
                    GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());

                Console.Out.WriteLine("Searching for Grid Result on Auth Sequence : {0}", authSequence);
                SetAuthSequence(authSequence);
                ClickOnFindButton();
                JavaScriptExecutor.ClickJQuery(authSeqLocator);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("pre_auths"));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(pageTitle, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });

            return new PreAuthActionPage(Navigator, newPreAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, true);
        }

        public PreAuthActionPage SearchByProviderSeqAndNavigateToAuthAction(string providerSeq, int row, int col, bool isClient = true)
        {
            if (!isClient)
                GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
            _sideBarPanelSearch.ClickOnClearLink();
            Console.Out.WriteLine("Searching for Grid Result by Provider Seq : {0} ", providerSeq);
            SetProviderSeq(providerSeq);
            ClickOnFindButton();
            _gridViewSection.ClickOnGridByRowCol(row, col);
            WaitForWorkingAjaxMessage();
            CloseAnyTabIfExist();
            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public PreAuthActionPage NaviateToPreAuthActionPage(int row, int col, bool clickFindButton = true, bool popUp = true)
        {
            if (clickFindButton)
            {
                _sideBarPanelSearch.ClickOnFindButton();
                WaitForWorkingAjaxMessage();
            }

            _gridViewSection.ClickOnGridByRowCol(row, col);
            WaitForWorkingAjaxMessage();
            if (popUp)
            {
                CloseAnyPopupIfExist();
            }

            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public PreAuthActionPage NaviateToPreAuthActionPageClient(int row, int col)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchClientEnum.DocumentsNeeded.GetStringValue());
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorkingAjaxMessage();
            _gridViewSection.ClickOnGridByRowCol(row, col);
            WaitForWorkingAjaxMessage();

            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public PreAuthActionPage ClickOnGridByRowColToPreAuthActionPage(int row, int col = 2, bool closeTab = true)
        {
            _gridViewSection.ClickOnGridByRowCol(row, col);
            WaitForWorkingAjaxMessage();
            if (closeTab)
                CloseAnyTabIfExist();
            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsLockIconPresentByAuthSeq(string authSeq)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(PreAuthSearchPageObjects.LockIconByAuthSeqCssLocator,
                authSeq));
        }

        public PreAuthActionPage ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage()
        {
            var countOfRecords = _gridViewSection.GetRecordRowsCountFromPage();

            for (int row = 1; row <= countOfRecords; row++)
            {
                var preAuthSeq = GetGridViewSection.GetValueInGridByColRow(2, row);
                if (!IsLockIconPresentByAuthSeq(preAuthSeq))
                {
                    _gridViewSection.ClickOnGridByRowCol(row, 2);
                    WaitForWorkingAjaxMessage();
                    return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                }
            }
            return null;
        }


        public void SetAuthSequence(string authSeq)
        {
            _sideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", authSeq);
        }

        public void SetProviderSeq(string providerSeq)
        {
            _sideBarPanelSearch.SetInputFieldByLabel("Provider Sequence", providerSeq);
        }

        public void SetPatientSeq(string patientSeq)
        {
            _sideBarPanelSearch.SetInputFieldByLabel("Patient Sequence", patientSeq);
        }

        public void ClickOnFindButton(bool wait = true)
        {
            SiteDriver.FindElement(PreAuthSearchPageObjects.FindXPath,How.XPath).Click();
            if (wait)
                WaitForWorkingAjaxMessage();
        }

        public PreAuthActionPage ClickOnAuthSeqToNavigateToPreAuthActionPage(string authSeq, bool popup = true)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(PreAuthActionPageObjects.SelectSearchResultByAuthSeqCssLocator, authSeq));
            WaitForWorkingAjaxMessage();
            if (popup)
                CloseAnyTabIfExist();

            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public void SetInputFieldByInputLabel(string label, string value)
        {
            _sideBarPanelSearch.SetInputFieldByLabel(label, value);
        }

        public List<string> GetAuthSeqListOnSearchResult()
        {
            var list =
                JavaScriptExecutor.FindElements(PreAuthSearchPageObjects.AuthListOnSearchResult,
                    How.CssSelector, "Text");
            return list;
        }
        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(PreAuthSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;

        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(
                PreAuthSearchPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public bool IsListIntSortedInDescendingOrder(int col)
        {
            var list = GetSearchResultListByCol(col).ToList().Select(int.Parse).ToList();
            return list.IsInDescendingOrder();
        }

        public bool IsListIntSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(int.Parse).ToList().IsInAscendingOrder();
        }
        public List<String> GetSearchResultListByCol(int col)
        {

            var list = JavaScriptExecutor.FindElements(
                string.Format(PreAuthSearchPageObjects.PreAuthSearchResultListCssTemplate, col), How.CssSelector, "Text");
            list.RemoveAll(String.IsNullOrEmpty);
            return list;
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInAscendingOrder();

        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder();
        }

        public bool IsListDateSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder();
        }
        public List<string> GetAuthSequenceByPrvSeqInDescendingOrder(string prvSeq)
        {
            var list = Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.AuthSequenceByPrvSeqInDescendingOrder, prvSeq));
            return list;
        }

        public List<string> GetAuthSequenceByPrvSeqInAscendingOrder(string prvSeq)
        {
            var list = Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.AuthSequenceByPrvSeqInAscendingOrder, prvSeq));
            return list;
        }

        public string GetPreAuthDetailsLabel(string label)
        {
            return SiteDriver.FindElement(
                String.Format(PreAuthSearchPageObjects.PreAuthDetailsLabelXPath, label),
                How.XPath).Text.Replace(":", "");
        }

        public string GetPreAuthDetailsValue(string label)
        {
            return SiteDriver.FindElement(String.Format(PreAuthSearchPageObjects.PreAuthDetailsValueXPath, label),
                    How.XPath).Text;
        }

        public List<string[]> GetPreAuthSearchValues(string preAuthId, string begindate,string enddate)
        {
            var resultList = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.PreAuthSearchValues, preAuthId, begindate, enddate));
            List<string[]> results = resultList.Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();

            return results;
        }

        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(PreAuthSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(PreAuthSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public string GetProvStateValue(string prvNum)
        {
            var list = Executor.GetSingleStringValue(string.Format(PreAuthSQLScriptObjects.ProvStateValue, prvNum));
            return list;
        }

        public int GetCountOfOutstandingPreAuthsForInternalUser()
        {
            return Executor.GetTableSingleColumn(PreAuthSQLScriptObjects.OutstandingPreAuthsCountForInternalUser).Count;
        }

        public int GetCountOfConsultantReviewPreAuthsForInternalUser()
        {
            return Executor.GetTableSingleColumn(PreAuthSQLScriptObjects.ConsultantReviewPreAuthsCountForInternalUser).Count;
        }

        public int GetCountOfDocumentsNeededPreAuthsForClientUser()
        {
            return Executor.GetTableSingleColumn(PreAuthSQLScriptObjects.DocumentsNeededPreAuthsCountForClientUser).Count;
        }

        public string IsLockIconPresentAndGetLockIconToolTipMessage(string authSeq, string pageUrl, bool isClient = true)
        {
            JavaScriptExecutor.ExecuteScript("window.open()");

            var handles = SiteDriver.WindowHandles.ToList();

            var loginPage = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            var login = isClient == false ? new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            ).LoginAsHciAdminUser() : new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            ).LoginAsClientUser();
            login.NavigateToPreAuthSearch();
            GetSideBarPanelSearch.SelectDropDownListByIndex("Quick Search", 1);
            GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", authSeq);
            ClickOnFindButton();
            SiteDriver.WaitForCondition(() => IsLockIconPresentByAuthSeq(authSeq), 3000);
            var isLockIconPresent = IsLockIconPresentByAuthSeq(authSeq);
            var authLockToolTipMessage = "";
            if (isLockIconPresent)
                authLockToolTipMessage = JavaScriptExecutor
                    .FindElement(string.Format(PreAuthSearchPageObjects.LockIconByAuthSeqCssLocator, authSeq)).GetAttribute("title");
            return authLockToolTipMessage;
        }

        public int GetCountOfLockedAuthSequences()
        {
            return SiteDriver.FindElementsCount(PreAuthSearchPageObjects.LockIconCssSelector, How.CssSelector);
        }

        public void ClickLastUnlockedAuthSeq(int row)
        {
            SiteDriver.FindElement(PreAuthSearchPageObjects.LastUnlockedAuthSeqXPath, How.XPath).Click();
            WaitForWorking();
            CloseCurrentWindowAndSwitchToOriginal(row);
        }

        public void CloseCurrentWindowAndSwitchToOriginal(int window)
        {
            var handles = SiteDriver.WindowHandles.ToList();
            SiteDriver.SwitchToLastWindow();
            SiteDriver.CloseWindowAndSwitchTo(handles[window]);

        }

        
        public void CloseLastWindow()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                SiteDriver.SwitchToLastWindow();
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.PreAuthorization.GetStringValue());
            }
        }
        #endregion

        #region Database Interactions

        public void DeleteLockByPreAuthSeq(string authSeq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteLockByAuthSeq, authSeq));
        }

        public void DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(string preAuthSeq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteFlagAuditHistory, preAuthSeq));

            Console.WriteLine("Delete Flag audit history from database if exists for preAuthSeq<{0}>", preAuthSeq);

        }

        public void UnDeleteAnyFlagByPreAuthSeq(string preAuthSeq)
        {
            Executor.ExecuteQuery(String.Format(PreAuthSQLScriptObjects.UnDeleteAnyFlagByPreAuthSeq, preAuthSeq));
        }

        /// <summary>
        /// Returns a Dictionary with 'Pre-Auth Seq' as the Key and it's search row details as it's Value
        /// </summary>
        /// <param name="totalRows"></param>
        /// <param name="excludeLockedPreAuths"></param>
        /// <returns>A Dictionary with 'Pre-Auth Seq' as the Key and it's details as it's Value</returns>
        public Dictionary<string, List<string>> GetSearchResultUptoRow(int totalRows, bool excludeLockedPreAuths = false)
        {
            int currentRow = 0;
            int column = 0;
            List<string> valuesList = new List<string>();
            Dictionary<string, List<string>> preAuthSearchResultDetails = new Dictionary<string, List<string>>();

            for (currentRow = 1; currentRow <= totalRows; currentRow++)
            {
                var preAuthID = GetGridViewSection.GetValueInGridByColRow(2, currentRow);

                if (excludeLockedPreAuths)
                {
                    if (IsLockIconPresentByAuthSeq(preAuthID))
                    {
                        totalRows++;
                        continue;
                    }
                }

                valuesList = new List<string>();

                for (column = 3; column < 9; column++)
                {
                    valuesList.Add(GetGridViewSection.GetValueInGridByColRow(column, currentRow));
                }

                preAuthSearchResultDetails.Add(preAuthID, valuesList);
            }

            return preAuthSearchResultDetails;
        }

        public void UpdateStatusOfAuthSeqFromDatabase(string status, string authSeq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.UpdateStatusOfPreAuthSeq, status, authSeq));

            Console.WriteLine("Update status for authsequences <{0}", authSeq);
        }


        public void DeleteHistoryAndUpdateStatusByAuthSeq(string authSeq, string date, string status)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteHistoryAndUpdateStatus, authSeq, date, status));
            Console.WriteLine("Delete history and update status for authsequence <{0}>", authSeq);
        }

        public void UpdateHciDoneOrClientDoneToFalseFromDatabase(string hcidoneOrClientDone, string authSeq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.UpdateHciDoneOrClientDoneToFalse, hcidoneOrClientDone, authSeq));

            Console.WriteLine("Update {0} for authseq <{1}", hcidoneOrClientDone, authSeq);
        }

        public void DeleteFlagAndAuditByAuthSeqFlagsLinNo(string authSeq, string flags, string linNo)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteFlagAndAuditByAuthSeqFlagsLinNo, authSeq, flags, linNo));
            Console.WriteLine("Delete Flags and Audit History for authsequence <{0}>", authSeq);
        }
        public void DeleteFlagAuditByAuthSeqFlagsLinNo(string authSeq, string flags, string linNo)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteFlagAuditByAuthSeqFlagsLinNo, authSeq, flags, linNo));
            Console.WriteLine("Delete Flags and Audit History for authsequence <{0}>", authSeq);
        }

        public void DeletePreAuthNote(string authseq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.deletePreAuthNotesFromDB, authseq));
        }


        public void DeleteLogicFromDatabase(string authseq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteLogic, authseq));
        }

        public void DeletePreAuthDocumentRecord(string authSeq, string audit_date)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeletePreAuthDocumentRecord, authSeq, audit_date));
            Console.WriteLine("Delete Pre Auth Document Record from database  for authSeq<{0}>", authSeq);
        }


        public void UpdateProductStatusForClient(string product, string client, bool active = false)
        {
            if (product == "DCA")
                product = "DCI";
            Executor.ExecuteQuery(string.Format(ClaimSqlScriptObjects.ActiveOrDisableProductByClient, product, client,
                active ? 'T' : 'F'));
        }

        public void DeleteRestorePreAuthFlagByInternalUser(string flagseq, bool delete = true)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeleteOrRestorePreAuthFlag, (delete) ? 'H' : 'N', (delete) ? 'T' : 'F', flagseq));
        }

        #endregion
    }
}
