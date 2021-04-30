using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.QA;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.QA
{
    public class QaAppealSearchPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private readonly QaAppealSearchPageObjects _qaAppealSearchPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly CalenderPage _calenderPage;
        private readonly AppealSearchPage _appealSearch;
        private AppealActionPage _appealAction;
        private readonly SubMenu _subMenu;

        #endregion

        #region CONSTRUCTOR

        public QaAppealSearchPage(INewNavigator navigator, QaAppealSearchPageObjects qaAppealSearchPageObject, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, qaAppealSearchPageObject, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _qaAppealSearchPage = (QaAppealSearchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _calenderPage = new CalenderPage(SiteDriver);
            _subMenu = new SubMenu(SiteDriver, JavaScriptExecutor);
        }

        #endregion

        public SubMenu GetSubMenu => _subMenu;
        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }
        public void SelectQuickSearch(string quickSearch)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearch);
        }

        public void SelectOutstandingQaAppeals()
        {
            SelectQuickSearch(QaAppealQuickSearchTypeEnum.OutstandingQaAppeals.GetStringValue());
        }

        public void ClickOnClearButton()
        {
            _sideBarPanelSearch.ClickOnClearLink();
        }

        public void ClickOnFindButton(bool wait = true)
        {
            JavaScriptExecutor.ExecuteClick(QaAppealSearchPageObjects.FindButtonCssLocator,
                How.CssSelector);
            Console.WriteLine("Find Button Clicked");
            if (wait)
                WaitForWorkingAjaxMessage();
            WaitForStaticTime(1000);
        }

        public bool IsQaAppealSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.QaAppealSearch), How.XPath);
        }

        public String GetPageInsideTitle()
        {
            return SiteDriver
                .FindElement(QaAppealSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector).Text;
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public void SelectAllQaAppeals()
        {
            SelectQuickSearch(QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
        }

        public void SelectClient(string clientName)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", clientName);
            //SelectQuickSearch(clientName);
            //_sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
        }

        public void ClickonFindButton()
        {
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();

        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        //public void ClickFilterIcon()
        //{
        //    _qaAppealSearchPage.SortFilterIcon.Click();s
        //}


        public void SetCompletedDateRange(string from, string to)
        {
            SiteDriver.FindElement(QaAppealSearchPageObjects.CompletedDateFrom, How.XPath).Click();
            _calenderPage.SetDate(from);
            SiteDriver.FindElement(QaAppealSearchPageObjects.CompletedDateTo, How.XPath).Click();
            _calenderPage.SetDate(to);
        }

        public bool IsSortIconPresent()
        {
            return SiteDriver.IsElementPresent(QaAppealSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }


        public void ClearFilterSearch()
        {
            _sideBarPanelSearch.ClickOnClearLink();
        }


        public AppealSummaryPage ClickOnAppealSequenceAndNavigateToAppealSummaryPage(string appealSeq = null, bool fromGrid = false, int row = 1)
        {
            var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                if (!fromGrid)
                {
                    SiteDriver.FindElement(
                        string.Format(GridViewSection.AppealSequenceXPathTemplate, appealSeq), How.XPath).Click();
                }
                else
                {
                    SiteDriver.FindElement(
                        string.Format(GridViewSection.ValueInGridByRowColumnCssTemplate, row, 3), How.CssSelector).Click();
                }
                Console.WriteLine("Clicked on Appeal Sequence, navigating to Appeal Summary");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealSummaryPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public List<string> GetAnalystList()
        {
            var analystList = Executor.GetTableSingleColumn(QASqlScriptObjects.AnalystList);
            analystList.Sort();
            return analystList;
        }

        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.GetAssignedClientList, userId));
        }

        public int GetDateRangeInputFieldsCount(string dateFieldName)
        {
            return SiteDriver.FindElementsCount(
                string.Format(QaClaimSearchPageObjects.InputFieldXPathTemplate, dateFieldName),
                How.XPath);
        }


        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(QaAppealSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(QaAppealSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector,
                "Text");
            ClickOnFilterOption();
            return list;
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(
                QaAppealSearchPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public List<String> GetSearchResultListByCol(int col, bool removeNull = true)
        {

            var list = JavaScriptExecutor.FindElements(
                string.Format(QaAppealSearchPageObjects.QAAppealSearchResultListCssTemplate, col), How.CssSelector,
                "Text");
            if (removeNull)
                list.RemoveAll(String.IsNullOrEmpty);
            return list;
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            if (col == 5) return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder();
            return GetSearchResultListByCol(col).IsInAscendingOrder();
        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public void SetAppealStatusToNewAndDeleteFromQAAppeal(string appealSeq, string userName)
        {
            var query = string.Format(QASqlScriptObjects.ChangeAppealStatusToNew, appealSeq);
            var query2 = string.Format(QASqlScriptObjects.DeleteFromQAAppeal, appealSeq);
            var query3 = string.Format(QASqlScriptObjects.ResetQAppealCounter, userName);
            Executor.ExecuteQuery(query);
            Executor.ExecuteQuery(query2);
            Executor.ExecuteQuery(query3);

        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
            WaitForWorkingAjaxMessage();
        }
        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }

        public bool IsVerticalScrollBarPresentInResultList()
        {
            const string select = QaAppealSearchPageObjects.QaAppealResutGridCheckForVerticalScrollBar;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public int GetScrollHeight(string select)
        {
            return JavaScriptExecutor.ScrollHeight(select);
        }

        public int GetClientHeight(string select)
        {
            return JavaScriptExecutor.ClientHeight(select);
        }

        public void SearchByAnalyst(string analyst, string client = null)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analyst);
            if (client == null)
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
            else
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", client);
            ClickOnFindButton();
        }

        public void RevertQAReviewComplete(string appealSeq, string userName)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.UpdateQAReviewToNULL, appealSeq, userName));
        }

        public void UpdateQAReviewToNULLAndUpdateCompletedDateToday(string appealSeq)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.UpdateQAReviewToNULLAndUpdateCompletedDateToday, appealSeq));
        }
        public void UpdateQAReviewToReviewed(string appealSeq)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.UpdateQAReviewToReviewed, appealSeq));
        }


        public void UpdateQAReviewToNULLForSingleAppeal(string appealSeq, string userName)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.UpdateQAReviewToNULLForSingleAppeal, appealSeq, userName));
        }

        public List<string> GetSearchResultsForAllRestrictionFilter(string userId)
        {
            return
            Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AllRestrictionSearchResult, userId));
        }

        public List<string> GetSearchResultsForAllRestrictionFilteForOutstandingQA(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AllRestrictionSearchResultForOutstandingQAAppeals, userId));
        }

        public List<string> GetSearchResultsForNoRestrictionFilter(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.NoRestrictionSearchResult, userId));
        }

        public List<string> GetSearchResultsForNoRestrictionFilterForOutstandingQA(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.NoRestrictionSearchResultForOutstandingQAAppeals, userId));
        }

        public List<List<string>> GetSearchResultsForAssignedRestriction(string userId)
        {
            var restrictionlist =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
            var list = new List<List<string>>();

            foreach (var restriction in restrictionlist)
            {
                var searchresult =
                    Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionSearchResult,
                        userId, restriction));
                list.Add(searchresult);
            }

            return list;
        }

        public List<List<string>> GetSearchResultsForAssignedRestrictionForOutstandingQA(string userId)
        {
            var restrictionlist = Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
            var list = new List<List<string>>();

            foreach (var restriction in restrictionlist)
            {
                var searchresult = Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionSearchResultForOutstandingQAAppeals, userId, restriction));
                list.Add(searchresult);
            }

            return list;
        }
        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(QaAppealSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(QaAppealSearchPageObjects.FindButtonCssLocator,
                                 QaAppealSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public List<string> GetAllActiveCategory()
        {
            var data = Executor.GetTableSingleColumn(QASqlScriptObjects.AppealCategoryCodes);
            return data;
        }

        public List<string> GetAllQAAppealByCategory(string code)
        {
            return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AllAppealSearchResultByCategory,code));
        }

        public List<string> GetOutStandingQAAppealByCategory(string code)
        {
            return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.OutStandingAppealSearchResultByCategory, code));
        }

        public List<List<string>> GetOutstandingQaAppealsGridResultFromDB(string conditionText = "")
        {
            if (conditionText.Length == 0)
                conditionText = "1=1";

            List<List<string>> collectedList = new List<List<string>>();
            List<DataRow> list = new List<DataRow>();
            
            list = Executor.GetCompleteTable(Format(QASqlScriptObjects.OutstandingQaAppealsGridResult, conditionText)).ToList();

            foreach (var dataRow in list)
            {
                collectedList.Add(dataRow.ItemArray.Select(x => x.ToString()).ToList());
            }

            return collectedList;
        }

        public string GetSysDateFromDB() =>
            GetCommonSql.GetSystemDateFromDB();

        public void ClickOnExportIcon() =>
            JavaScriptExecutor.ClickJQuery(QaAppealSearchPageObjects.ExportIconCssLocator);

        public string GetFileName(string url = null)
        {
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
            Action action;
            if (url != null)
                action = () => SiteDriver.Open(url);
            else
                action = SiteDriver.Back;
            Navigator.Navigate<QaAppealSearchPageObjects>(action);
            WaitForPageToLoad();
            WaitForWorkingAjaxMessage();
            WaitForStaticTime(3000);
            return fileName;
        }

        public int GetAuditSeqForExportFromDB(string userId, string dateInMMDDYYYY )
        {
            return (int) Executor.GetSingleValue(Format(QASqlScriptObjects.GetAuditSeqFromDBAfterExport, userId, dateInMMDDYYYY));
        }

        public string GetAppealCompletedDateFromDb(string appealSeq)
        {
            return Executor.GetSingleStringValue(Format(AppealSqlScriptObjects.GetAppealCompletedDateFromDb,appealSeq));
        }

        public List<string> GetOutStandingQAAppealByAppealResult(string appealResult)
        {
            if(appealResult=="A")
                return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AdjustAppealResultSearchDataForOutstandingQAAppeals, appealResult));

            return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AppealResultSearchDataForOutstandingQAAppeals, appealResult));
        }

        public List<string> GetAllQAAppealsByAppealResult(string appealResult)
        {
            if (appealResult == "A")
                return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AdjustAppealResultSearchDataForAllQAAppeals, appealResult));

            return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AppealResultSearchDataForAllAppeals, appealResult));
        }

        public List<string> GetAppealResultTypeInGridViewSection() =>
            JavaScriptExecutor.FindElements(QaAppealSearchPageObjects.AppealResultGridViewSectionCSSSelector, "Text");

        public List<string> GetQaAppealsForAllCenteneClient(string userId, bool allQaAppeals = true)
        {
            var sqlCondition = allQaAppeals ? "" : "and qra.reviewed = 'F' ";

            return Executor.GetTableSingleColumn(Format(QASqlScriptObjects.QaAppealForAllCenteneClientSqlScript, sqlCondition, userId));
        }
            

    }


}

