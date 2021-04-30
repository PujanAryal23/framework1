using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.Batch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Batch;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Batch
{
    public class BatchSearchPage : NewDefaultPage
    {

        #region PRIVATE FIELDS

        private readonly BatchSearchPageObjects _batchSearchPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;
        private readonly SubMenu _subMenu;


        #endregion

        #region CONSTRUCTOR


        public BatchSearchPage(INewNavigator navigator, BatchSearchPageObjects _newBatchSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor) : base(navigator,
            _newBatchSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _newBatchSearchPage = new BatchSearchPageObjects();
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _subMenu = new SubMenu(SiteDriver, JavaScriptExecutor);

        }

        #endregion

        #region PUBLIC METHODS

        public void SelectSmtstClient()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public SubMenu GetSubMenu => _subMenu;

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }
        public bool IsBatchSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.BatchSearch), How.XPath);
        }

        public string GetBatchReleaseIconToolTip()
        {
            return SiteDriver.FindElementAndGetAttribute(BatchSearchPageObjects.BatchReleaseIconToolTipCssSelector,
                How.CssSelector, "title");
        }
        public int GetBatchReleaseIconCount()
        {
            return SiteDriver.FindElementsCount(BatchSearchPageObjects.BatchReleaseIconCssSelector, How.CssSelector);
        }

        public void ClickOnBatchReleaseIcon()
        {
            SiteDriver.FindElement(BatchSearchPageObjects.BatchReleaseIconCssSelector, How.CssSelector).Click();
            WaitForWorking();
        }

        public List<string> GetFirstFiftyActiveBatchesFromDB()
        {
            return Executor.GetTableSingleColumn(BatchSqlScriptObjects.GetFirstFiftyActiveBatches);
        }

        public string GetTotalClaimsCountInSecondaryDetails(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.TotalClaimsInBatch, batchId));
        }

        public string GetRelesedUserDateFromDatabase(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.ReleaseUserReleaseDate, batchId));
        }

        public string GetBatchCompleteDateFromDatabase(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.BatchCompleteDate, batchId));
        }

        public string GetBatchDateFromDatabase(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.BatchDate, batchId));
        }

        public string GetTotalUnreviewedClaimsCountFromDatabase(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.UnreviewedClaims, batchId));
        }

        public string GetTotalUnreviewedClaimsCountByClientFromDatabase(string batchId)
        {
            return Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.UnreviewedClaimsByClient, batchId));
        }

        public List<string> GetBatchesReceivedThisWeekFromDatabase()
        {
            return Executor.GetTableSingleColumn(BatchSqlScriptObjects.BatchesReceivedThisWeek);
        }


        public List<string> GetIncompleteBatchesListromDatabase()
        {
            return Executor.GetTableSingleColumn(BatchSqlScriptObjects.IncompleteBatches);
        }

        public List<string> GetCotivitiAndClientCreateDateFromDatabase(string batchId)
        {
            List<string> createDate = new List<string>();
            var cotivitiClientCreateDate = Executor
                .GetCompleteTable(string.Format(BatchSqlScriptObjects.CotivitiANDClientCreateDate, batchId));

            foreach (DataRow row in cotivitiClientCreateDate)
            {
                createDate.Add(row[0].ToString());
                createDate.Add(row[1].ToString());
            }
            return createDate;

        }

        public string GetUnreviewedClaimsCountByProductFromDatabase(string batchId, ProductEnum p)
        {
            //List<string> claimCount = new List<string>();
            return Executor
                .GetSingleStringValue(string.Format(BatchSqlScriptObjects.ProductWiseUnreviewedClaims, batchId,
                    p.GetStringDisplayValue()));

        }

        public string GetUnreviewedClaimsCountByProductByClientFromDatabase(string batchId, ProductEnum p)
        {
            //List<string> claimCount = new List<string>();
            //var productClaimCount = Executor
            //    .GetCompleteTable(string.Format(BatchSqlScriptObjects.ProductWiseUnreviewedClaimsByClient, batchId));
            //var dataRows = productClaimCount as DataRow[] ?? productClaimCount.ToArray();
            //return dataRows[0].ItemArray.Select(x => x.ToString()).ToList();
            return Executor
                .GetSingleStringValue(string.Format(BatchSqlScriptObjects.ProductWiseUnreviewedClaimsByClient, batchId,
                    p.GetStringDisplayValue()));
        }

        public void RevertBatch(string claseq1, string claseq2, string claseq3, string claseq4, string batchSeq)
        {
            Executor.ExecuteQuery(string.Format(BatchSqlScriptObjects.BatchReleaseRevert, claseq1, claseq2, claseq3, claseq4, batchSeq));
        }

        public string GetRelToClientFromDatabase(string batchID)
        {
            var reltoclientvalue = Executor.GetSingleStringValue(string.Format(BatchSqlScriptObjects.RELTOCLIENT, batchID));
            return reltoclientvalue;
        }

        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
        }

        public List<string> GetProductLabels()
        {
            int i = 3;
            List<string> list = new List<string>();
            while (!GetGridViewSection.GetLabelInGridByColRow(i).ToLower().Contains("released"))
            {
                var text = GetGridViewSection.GetLabelInGridByColRow(i);
                text = Regex.Match(text, @"\w+").ToString();
                list.Add(text);
                i = i + 2;
            }
            return list;
        }

        public List<String> GetActiveProductListForClientDB()
        {
            var orderofProducts = new[] { "CV", "FFP", "FCI", "DCA", "COB" };
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.ActiveProductList));

            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }

            var finalListProducts = orderofProducts.Zip(newList, (order, list) => list == "T" ? order : "-1").ToList();
            finalListProducts.RemoveAll(x => x.Equals("-1"));
            return finalListProducts;
        }

        public bool IsCheckMarkPresent(string label)
        {

            return SiteDriver.IsElementPresent(string.Format(BatchSearchPageObjects.CheckMarkIconCssSelectorByLabel, label), How.XPath);


        }



        public string GetBatchDetailsHeader()
        {
            return SiteDriver
                .FindElement(BatchSearchPageObjects.BatchDetailsHeaderCssSelector, How.CssSelector).Text;
        }
        public string GetBatchDetailsSecondaryViewValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(
                    string.Format(BatchSearchPageObjects.BatchDetailsValueByLabelXpathTemplate, label), How.XPath)
                .Text;
        }

        public BatchSummaryPage ClickOnBatchIdAndNavigateToBatchSummaryPage(string batchID)
        {
            var newBatchSummaryPage = Navigator.Navigate<BatchSummaryPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(
                    String.Format(BatchSearchPageObjects.BatchIDXPathTemplate, batchID), How.XPath));
                SiteDriver.FindElement(String.Format(BatchSearchPageObjects.BatchIDXPathTemplate, batchID),
                    How.XPath).Click();
                Console.WriteLine("Clicked on batch id to open batch summary for batch id '{0}'", batchID);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(BatchSummaryPageObjects.BackIconCssTemplate, How.CssSelector));

            });
            return new BatchSummaryPage(Navigator, newBatchSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(BatchSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(BatchSearchPageObjects.FindButtonCssLocator,
                How.CssSelector);
            Console.WriteLine("Find Button Clicked");
            WaitForWorkingAjaxMessage();
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(BatchSearchPageObjects.FindButtonCssLocator,
                                 BatchSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public BatchSummaryPage ClickOnBatchByRowCol(int row, int col)
        {
            var newBatchSummaryPage = Navigator.Navigate<BatchSummaryPageObjects>(() =>
            {
                _sideBarPanelSearch.SetInputFieldByLabel("Quick Search", "Incomplete Batches", false, 0, true);
                _sideBarPanelSearch.ClickOnFindButton();
                _gridViewSection.ClickOnGridByRowCol(row, col);

            });
            return new BatchSummaryPage(Navigator, newBatchSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(BatchSearchPageObjects.FilterOptionListByCss, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(BatchSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(string.Format(BatchSearchPageObjects.FilterOptionValueByCss, row), How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public void ClickOnClearSort()
        {
            ClickOnFilterOptionListRow(3);
            Console.WriteLine("Clicked on clear filter option");
        }
        #endregion

    }

}
