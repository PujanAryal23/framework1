using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.SqlScriptObjects.QA;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using static System.String;

namespace Nucleus.Service.PageServices.QA
{
    public class QaClaimSearchPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private readonly QaClaimSearchPageObjects _qaClaimSearchPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly NewPagination _pagination;
        private readonly SideWindow _sideWindow;

        #endregion

        #region CONSTRUCTOR

        public QaClaimSearchPage(INewNavigator navigator, QaClaimSearchPageObjects qaClaimSearchPageObject, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, qaClaimSearchPageObject, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _qaClaimSearchPage = (QaClaimSearchPageObjects) PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _pagination = new NewPagination(SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public NewPagination GetPagination
        {
            get { return _pagination; }
        }

        #endregion

        #region database

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public void UpdateDeleteQaClaimReviewStatusForClaim(string claseq)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.UpdateDeleteQaClaimReviewStatusForClaim, claseq));
            Console.WriteLine(
                "Updated Qa Review Claim Status And Delete Claim Audit Record in database if exists for claimseq<{0}>",
                claseq);
        }

        public void UpdateQAClaimStatus(string claseq)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.UpdateQAStatusofClaim, claseq));
        }

        public bool IsClaimAuditRowPresentForQaPassByClaseq(string claseq)
        {
            return Convert.ToInt32(
                       Executor.GetTableSingleColumn(Format(QASqlScriptObjects.GetCountofQaPassAudit, claseq))
                           [0]) > 0;
        }

        public int GetTotalQaReviewClaimCount()
        {
            return Convert.ToInt32(
                Executor.GetTableSingleColumn(Format(QASqlScriptObjects.GetTotalQaReviewClaimCount,
                    EnvironmentManager.HciAdminUsername))[0]);
        }

        public bool IsClaimAuditRowPresentForQaFailByClaseq(string claseq)
        {
            return Convert.ToInt32(
                       Executor.GetTableSingleColumn(Format(QASqlScriptObjects.GetCountofQaFailAudit, claseq))
                           [0]) > 0;
        }

        public bool IsClaimAuditRowPresentForNoQaResultByClaseq(string claseq)
        {
            return Convert.ToInt32(
                       Executor.GetTableSingleColumn(Format(QASqlScriptObjects.GetCountofNoQaResultAudit, claseq))
                           [0]) > 0;
        }

        public List<string> GetAnalystList()
        {
            Executor.GetTableSingleColumn(QASqlScriptObjects.AnalystList);
            var analystList = Executor.GetTableSingleColumn(QASqlScriptObjects.AnalystList);
            analystList.Sort();
            return analystList;
        }

        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn(Format(QASqlScriptObjects.GetAssignedClientList, userId));
        }

        public List<string> GetOutstandingQaClaimsList()
        {

            var outstandingQaClaimList =
                Executor.GetTableSingleColumn(Format(QASqlScriptObjects.OutstandingQaClaimsList,
                    EnvironmentManager.HciAdminUsername));
            outstandingQaClaimList.Sort();
            return outstandingQaClaimList;
        }

        public List<string> GetQaClaimsListForGivenFilterForOutstandingStatus(string filterLabel, string filterValue)
        {
            var qaClaimList = new List<string>();

            switch (filterLabel)
            {
                case "Analyst":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForAnalystWithStatus,
                            filterValue, "A"));
                    break;
                case "Client":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForClientWithStatus,
                            filterValue, "A"));
                    break;

            }

            qaClaimList.Sort();
            return qaClaimList;
        }

        public List<string> GetQaClaimsListForGivenFilterForAllStatus(string filterLabel, string filterValue,
            string client = null)
        {
            var qaClaimList = new List<string>();

            switch (filterLabel)
            {
                case "Analyst":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForAnalyst, filterValue));
                    break;
                case "Client":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForClient, filterValue));
                    break;
                case "Approve Date":
                    qaClaimList =
                        Executor.GetTableSingleColumn(
                            Format(QASqlScriptObjects.QaClaimsListForApproveDate, filterValue));
                    break;
                case "Review Date":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForReviewDate,
                            filterValue));
                    break;
                case "QA Reviewer":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForQaReviewer,
                            filterValue));
                    break;
                case "Status":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForStatus, filterValue));
                    break;
                case "ClientAndStatus":
                    qaClaimList =
                        Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaClaimsListForClientWithStatus,
                            filterValue, client));
                    break;

            }

            qaClaimList.Sort();
            return qaClaimList;
        }

        public bool IsClaimSeqAlreadySelectedForDailyQc(string claseq)
        {
            return Convert.ToInt32(
                       Executor.GetTableSingleColumn(Format(QASqlScriptObjects.CheckIfClaimIsSelectedForDailyQcByCount,
                               claseq.Split('-')[0], claseq.Split('-')[1]))
                           [0]) > 0;
        }

        public void SideBarIconClick()
        {
            SiteDriver.FindElement(QaClaimSearchPageObjects.SearchAnalystCssLocator, How.CssSelector).Click();
        }

        #endregion

        #region GRID

        public bool IsCurrentClaimSequenceLastClaimSequence(string claseq)
        {
            return SiteDriver.FindElement(QaClaimSearchPageObjects.LastClaimSequenceOfGridXPath, How.XPath).Text
                .Trim().Equals(claseq);
        }

        public string GetQaResultStatusByClaimSequence(string claseq)
        {
            return
                SiteDriver.FindElement(Format(QaClaimSearchPageObjects.QaResultIconByClaseqXPathTemplate, claseq),
                        How.XPath)
                    .GetAttribute("title");
        }

        public bool IsSideBarIconPresent()
        {
            return SiteDriver.IsElementPresent(QaClaimSearchPageObjects.FilterPanelCssLocator, How.CssSelector);
        }

        public List<string> GetListValueOfGrid(int col)
        {
            return _gridViewSection.GetGridListValueByCol(col);
        }

        public string GetClaimSequenceForNoQaResultIcon()
        {
            return JavaScriptExecutor.FindElement(QaClaimSearchPageObjects.ClaseqByNoQaResultIconJQuery).Text;
        }

        public string GetClaimSequenceForQaPassResultIcon()
        {
            return JavaScriptExecutor.FindElement(QaClaimSearchPageObjects.ClaseqByQaPassResultIconJQuery).Text;
        }

        public string GetClaimSequenceForQaFailResultIcon()
        {
            return JavaScriptExecutor.FindElement(QaClaimSearchPageObjects.ClaseqByQaFailResultIconJQuery).Text;
        }

        #endregion

        public bool IsListInAscendingOrder(List<string> expectedList)
        {
            return expectedList.IsInAscendingOrder();
        }

        public bool IsListInDescendingOrder(List<string> expectedList)
        {
            return expectedList.IsInDescendingOrder();
        }

        public void SelectQuickSearch(string quickSearch)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearch);
        }

        public void SelectOutstandingQaClaims()
        {
            SelectQuickSearch(QaClaimQuickSearchTypeEnum.OutstandingQaClaims.GetStringValue());
        }

        public void SelectAllQaClaims()
        {
            SelectQuickSearch(QaClaimQuickSearchTypeEnum.AllQaClaims.GetStringValue());
        }
        
        public void SelectClient(string client)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", client);
        }

        public void SelectSmtstClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
        }

        public void SelectCvtyClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.CVTY.GetStringDisplayValue());
        }

        public void SelectPehpClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.PEHP.ToString());
        }

        public void SelectRpeClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.RPE.ToString());
        }

        public void SelectTtreeClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.TTREE.ToString());
        }

        public void SelectAnalyst(string analyst)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analyst);
        }

        public void ClickOnFilterOption()
        {
            var element = SiteDriver.FindElement(QaClaimSearchPageObjects.FilterCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var filterOptionList = JavaScriptExecutor.FindElements(QaClaimSearchPageObjects.FilterOptionListCssLocator,
                How.CssSelector, "Text");
            return filterOptionList;
        }

        public void ClickOnFilterOptionList(string listValue)
        {
            JavaScriptExecutor.ExecuteClick(Format(QaClaimSearchPageObjects.FilterListValueCssTemplate, listValue), How.XPath);
            SiteDriver.WaitForPageToLoad();
        }

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(QaClaimSearchPageObjects.FindButtonCssLocator,
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Find Button Clicked");
        }

        public void SearchByAnalyst(string analyst)
        {
            SelectAnalyst(analyst);
            ClickOnFindButton();
        }

        public void SelectClientAndFind(string client)
        {
            SelectClient(client);
            ClickOnFindButton();
        }

        public void ClickOnClearButton()
        {
            _sideBarPanelSearch.ClickOnClearLink();
        }

        public ClaimActionPage ClickOnClaimSequenceAndNavigateToNewClaimActionPage(string claseq)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(Format(GridViewSection.ClaimSequenceXPathTemplate, claseq),
                    How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsClaimSequencePresentInGridPanel(string claimSeq)
        {
            return SiteDriver.IsElementPresent(
                Format(QaClaimSearchPageObjects.GridForClaimSequenceXPathTemplate, claimSeq), How.XPath);
        }

        public bool IsNoDataAvailableMessagPresentInSearchList()
        {
            return SiteDriver.IsElementPresent(QaClaimSearchPageObjects.EmptySearchResultCssTemplate, How.CssSelector);
        }

        public bool IsQaClaimSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.QaClaimSearch), How.XPath);
        }

        public bool IsQaAppealSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.QaAppealSearch), How.XPath);
        }

        public string GetPageInsideTitle()
        {
            return SiteDriver.FindElement(QaClaimSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector).Text;
        }

        public void ClickOnToggleFindAnalystPanelButton()
        {
            JavaScriptExecutor.ExecuteClick(QaClaimSearchPageObjects.ToggleFindAnalystPanelCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(1000);
        }

        public int GetDateRangeInputFieldsCount(string dateFieldName)
        {
            return SiteDriver.FindElementsCount(Format(QaClaimSearchPageObjects.InputFieldXPathTemplate, dateFieldName),
                How.XPath);
        }

        public bool IsSortIconPresent()
        {
            return SiteDriver.IsElementPresent(QaClaimSearchPageObjects.FilterCssSelector, How.CssSelector);
        }

        public bool IsAwaitingReviewListWithNoIconPresentInGrid()
        {
            return SiteDriver.IsElementPresent(QaClaimSearchPageObjects.AwaitingReviewListIconPresentInGridCssTemplate,
                How.CssSelector);
        }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(QaClaimSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }
        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            // ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(QaClaimSearchPageObjects.FindButtonCssLocator,
                                 QaClaimSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public void DeleteClaimLock(string claimSeq)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.DeleteClaimLock,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));
        }
        public bool IsClaimSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.QaClaimSearch), How.XPath);
        }


        public List<List<string>> GetSearchResultsForAssignedRestriction(string userId)
        {
            var restrictionlist =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
            var list = new List<List<string>>();

            foreach (var restriction in restrictionlist)
            {
                var searchresult =
                    Executor.GetTableSingleColumn(string.Format(
                        QASqlScriptObjects.QAClaimSearchAssignedRestrictionSearchResult,
                        userId, restriction));
                list.Add(searchresult);
            }

            return list;
        }

        public List<string> GetQAClaimSearchResultsForAllRestrictionFilterForOutstandingQADb(string userId)
        {
            return
                Executor.GetTableSingleColumn(
                    string.Format(QASqlScriptObjects.QAClaimSearchResultForAllRestrctionForOutStandingQA, userId));
        }

        public List<string> GetQAClaimSearchResultsForNoRestrictionFilterForOutstandingQADb(string userId)
        {
            return
                Executor.GetTableSingleColumn(
                    string.Format(QASqlScriptObjects.QAClaimsSearchResultForNoRestrctionForOutstandingQA, userId));
        }

        public List<string> GetQAClaimsAllClaimsAllRestrcitionSearchResultDb(string userId)
        {
            return
                Executor.GetTableSingleColumn(
                    string.Format(QASqlScriptObjects.QAClaimsAllClaimsAllRestrcitionSearchResult, userId));
        }

        public List<string> GetQAClaimsAllClaimNoRestrcitionSearchResultDb(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.QAAllClaimsNoRestrictionSearchResult,
                    userId));
        }

        public bool IsRestrictionIconPresentInSearchResult()
        {
            return
                SiteDriver.IsElementPresent(QaClaimSearchPageObjects.RestrictionIconCssSelector, How.CssSelector);
        }

        public int GetRestrictionIconCountInSearchResult()
        {
            return
                SiteDriver.FindElements(QaClaimSearchPageObjects.RestrictionIconCssSelector, How.CssSelector).Count();
        }

        public string GetRestrictionTooltip() =>
            SiteDriver.FindElementAndGetAttribute(QaClaimSearchPageObjects.RestrictionIconCssSelector, How.CssSelector,
                "title");

    }
}
