using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Batch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Batch;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Batch
{
    public class BatchSummaryPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly BatchSummaryPageObjects _batchSummaryPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;
        
        #endregion

        #region CONSTRUCTOR


        public BatchSummaryPage(INewNavigator navigator, BatchSummaryPageObjects _newBatchSummaryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor) : base(
            navigator,
            _newBatchSummaryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _newBatchSummaryPage = new BatchSummaryPageObjects();
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            
        }

        #endregion

        #region PUBLIC METHODS
        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public void SelectAllBatches()
        {
            SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
        }

        public string GetQuadrantTitle(int i = 0)
        {
            var value = JavaScriptExecutor.FindElements(BatchSummaryPageObjects.QuadrantTitlesCssTemplate, How.CssSelector,
                "Text");
            return value[i];
        }

        public string GetBatchIdValue()
        {
            return SiteDriver.FindElement(BatchSummaryPageObjects.BatchIDValueXpath, How.XPath).Text;

        }

        public List<string> GetValuesByLabelListInQ1Qudrant(string[] labelList)
        {
            var data = "";
            foreach (var item in labelList)
            {
                if (labelList.LastOrDefault().Equals(item))
                {
                    data += string.Format("text()='{0}:'", item);
                }
                else
                {
                    data +=string.Format( "text()='{0}:' or ",item);
                }
            }

            return JavaScriptExecutor.FindElements(
                string.Format(BatchSummaryPageObjects.BatchInformationTopDivValueXPathTemplate, data),
                How.XPath, "Text");
        }

        public List<string> GetBatchInformationRightDivValue()
        {
            return JavaScriptExecutor.FindElements(BatchSummaryPageObjects.BatchInformationRightDivValueCssLocator,
                How.XPath, "Text");
        }

        public string GetValueByLabelInQ1Qudrant(string label)
        {
            return SiteDriver.FindElement(string.Format(BatchSummaryPageObjects.ValueByLabelXpathTemplate,label), How.XPath).Text;
            
        }

        public List<string> GetProcessingHistoryListByColumn(int col = 1)
        {
            var t = JavaScriptExecutor.FindElements(string.Format(BatchSummaryPageObjects.ProcessingHistoryValueListByColumnCssTemplate, col), "Text");
            return t;
        }

        public List<string> GetBatchFilesListByColumn(int col = 1)
        {
            var t = JavaScriptExecutor.FindElements(string.Format(BatchSummaryPageObjects.ProcessingHistoryValueListByColumnCssTemplate, col), "Text");
            return t;
        }

        public bool IsBackIconPresent()
        {
            return SiteDriver.IsElementPresent(BatchSummaryPageObjects.BackIconCssTemplate, How.CssSelector);
        }
        
        public BatchSearchPage ClickOnBackButtonAndNavigateBackToBatchSearchPage()
        {
            var newBatchSearchPage = Navigator.Navigate<BatchSearchPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(BatchSummaryPageObjects.BackIconCssTemplate, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForPageToLoadWithSideBarPanel();
            });

            return new BatchSearchPage(Navigator, newBatchSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
          
        }
        
        public List<string> GetFirstRowHeaderLabel()
        {
            return JavaScriptExecutor.FindElements(BatchSummaryPageObjects.FirstRowHeaderXpathTempate, How.XPath, "Text").Skip(1).Take(3).ToList();
            
        }

        public List<string> GetColumnHeaderLabel()
        {
            return JavaScriptExecutor.FindElements(BatchSummaryPageObjects.FirstColumnXpathTemplate, How.XPath, "Text");
        }

        public List<string> GetRowValueByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(string.Format(BatchSummaryPageObjects.RowValueByLabelXpath,label), How.XPath, "Text");
        }

        public string GetLabelInProcessingHistoryByRow(int row = 1,int col=1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(BatchSummaryPageObjects.ProcessingHistoryLabelInGridByRowColumnCssTemplate, row,col),
                    How.CssSelector).Text;
        }

        public void ClickOnDataValueIcon()
        {
            var element = SiteDriver.FindElement(BatchSummaryPageObjects.DataIconCssTemplate, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Data Value icon");
        }

        public void ClickOnDollarValueIcon()
        {
            var element = SiteDriver.FindElement(BatchSummaryPageObjects.DollarIconCssTemplate, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Dollar Value icon");
        }

        public bool IsDollarValueIconPresent()
        {
           return SiteDriver.IsElementPresent(BatchSummaryPageObjects.DollarIconCssTemplate, How.CssSelector);
           
        }

        public bool IsDataValueIconSelected()
        {
            return SiteDriver.IsElementPresent(BatchSummaryPageObjects.SelectedDataValueIconXpath, How.XPath);
        }

        public bool IsDollarValueIconSelected()
        {
            return SiteDriver.IsElementPresent(BatchSummaryPageObjects.SelectedDollarIconCssSelector, How.CssSelector);
        }
        public bool IsCountValueIconPresent()
        {
            return SiteDriver.IsElementPresent(BatchSummaryPageObjects.DataIconCssTemplate, How.CssSelector);

        }

        public bool IsEmptyMessagePresentInReturnFiles()
        {
            return SiteDriver.IsElementPresent(BatchSummaryPageObjects.EmptyMessageXpathTemplate, How.XPath);
        }

        public string GetEmptyMessagePresentInReturnFiles()
        {
            return SiteDriver
                .FindElement(BatchSummaryPageObjects.EmptyMessageXpathTemplate, How.XPath).Text;

        }

        public List<string> GetStatsByProductList(string product)
        {
            var value = JavaScriptExecutor.FindElements(
                string.Format(BatchSummaryPageObjects.DataValuesByProductCssTemplate, product), How.XPath, "Text");
            return value;
        }

        public List<string> GetActiveProductList()
        {
            var list= JavaScriptExecutor.FindElements(BatchSummaryPageObjects.ActiveProductListCssTemplate, How.CssSelector,
                "Text");
            return list;
        }

        public List<string> GetStatsByProductFirstColumnLabel()
        {
            return JavaScriptExecutor.FindElements(BatchSummaryPageObjects.FirstContainerFirstColumnLabelsXpathTemplate, How.XPath,
                "Text");
        }

        public List<string> GetStatsByProductLabelByRow(int row=1)
        {
            return JavaScriptExecutor.FindElements(string.Format(BatchSummaryPageObjects.FirstContainerLabelsByRowXpathTemplate,row+1), How.XPath,
                "Text");
        }
        #region SQL

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }
        public List<List<string>> GetBatchInformationInQaQuadrant(string batchId)
        {
            var infoList = new List<List<string>>();
            var batchInfo = Executor
                .GetCompleteTable(string.Format(BatchSqlScriptObjects.BatchInformationInQ1,
                    batchId));
            foreach (DataRow row in batchInfo)
            {
                infoList.Add(new List<string>
                {
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString(),
                    row[3].ToString(),
                    row[4].ToString(),
                    row[5].ToString()
                });
                infoList.Add(new List<string>
                {
                    row[6].ToString(),
                    row[7].ToString(),
                    Math.Round(Convert.ToDouble(row[8]), 2, MidpointRounding.AwayFromZero).ToString("C2"),
                    row[9].ToString(),
                    row[10].ToString(),
                    Math.Round(Convert.ToDouble(row[11]), 2, MidpointRounding.AwayFromZero).ToString("C2")
                });
                
               



            }
            return infoList;
        }

        public List<List<string>> GetBatchReleaseListFromDatabase(string batchId)
        {
            var infoList =new List<List<string>>();
            var batchInfo = Executor
                .GetCompleteTable(string.Format(BatchSqlScriptObjects.BatchReleaseUserDateTimeList,
                    batchId));
            foreach (DataRow row in batchInfo)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList;
        }

        public int GetReturnFilesCount(string batchSeq)
        {
            return Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.ReturnFiles, batchSeq)).Count();

        }

        public List<List<String>> GetDataValueListByActiveProduct(string batchID)
        { 
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(BatchSqlScriptObjects.NewProductList);
                 
            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            newList.RemoveAll(x => x.Equals("-1"));
            var newDataValueList = new List<List<string>>();
            foreach (string product in newList)
            {
                switch (product)
                {
                    case "CV":
                        var list = Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.PCIDataValues,batchID));
                        newDataValueList.AddRange(list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()));
                        break;
                    case "FFP":
                        newDataValueList.AddRange(Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.FFPDataValues, batchID)).Select(row => row.ItemArray.Select(x => x.ToString()).ToList()));
                        break;
                    case "FCI":
                        newDataValueList.AddRange(Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.FCIDataValues, batchID)).Select(row => row.ItemArray.Select(x => x.ToString()).ToList()));
                        break;
                    case "DCA":
                        newDataValueList.AddRange(Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.DCIDataValues, batchID)).Select(row => row.ItemArray.Select(x => x.ToString()).ToList()));
                        break;
                    case "COB":
                        newDataValueList.AddRange(Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.COBDataValues, batchID)).Select(row => row.ItemArray.Select(x => x.ToString()).ToList()));
                        break;

                }
            }
            return newDataValueList;
        }

        public List<List<String>> GetDollarValueListByActiveProduct(string batchID)
        {
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.NewProductList));

            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            newList.RemoveAll(x => x.Equals("-1"));
            var newDataValueList = new List<List<string>>();
            foreach (string product in newList)
            {
                switch (product)
                {
                    case "CV":
                        var list = Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.PCIDollarValues, batchID));
                        newDataValueList.AddRange(list.Select(row => row.ItemArray.Select(x =>
                                Math.Round(Convert.ToDouble(x), 2, MidpointRounding.AwayFromZero).ToString("C2"))
                            .ToList()));
                        break;
                    case "FFP":
                        newDataValueList.AddRange(Executor
                            .GetCompleteTable(string.Format(BatchSqlScriptObjects.FFPDollarValues, batchID))
                            .Select(row => row.ItemArray.Select(x =>
                                    Math.Round(Convert.ToDouble(x), 2, MidpointRounding.AwayFromZero).ToString("C2"))
                                .ToList()));
                        break;
                    case "FCI":
                        newDataValueList.AddRange(Executor
                            .GetCompleteTable(string.Format(BatchSqlScriptObjects.FCIDollarValues, batchID))
                            .Select(row => row.ItemArray.Select(x =>
                                    Math.Round(Convert.ToDouble(x), 2, MidpointRounding.AwayFromZero).ToString("C2"))
                                .ToList()));
                        break;
                    case "DCA":
                        newDataValueList.AddRange(Executor
                            .GetCompleteTable(string.Format(BatchSqlScriptObjects.DCIDollarValues, batchID))
                            .Select(row => row.ItemArray.Select(x =>
                                    Math.Round(Convert.ToDouble(x), 2, MidpointRounding.AwayFromZero).ToString("C2"))
                                .ToList()));
                        break;
                    case "COB":
                        newDataValueList.AddRange(Executor
                            .GetCompleteTable(string.Format(BatchSqlScriptObjects.COBDollarValues, batchID))
                            .Select(row => row.ItemArray.Select(x =>
                                    Math.Round(Convert.ToDouble(x), 2, MidpointRounding.AwayFromZero).ToString("C2"))
                                .ToList()));

                        break;

                }
            }
            return newDataValueList;
        }
        public List<String> GetActiveProductListForClientDB()
        {
            var newList = new List<String>();

            var productList =
                Executor.GetCompleteTable(string.Format(BatchSqlScriptObjects.NewProductList));

            foreach (var row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            newList.RemoveAll(x => x.Equals("-1"));
            return newList;
        }

        public string GetValueInProcessingHistoryByRow(int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(BatchSummaryPageObjects.ProcessingHistoryValueInGridByRowColumnCssTemplate, row, col),
                    How.CssSelector).Text;
        }
        #endregion

        #endregion

    }
}
