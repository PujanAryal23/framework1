using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.Invoice;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Menu;

namespace Nucleus.Service.PageServices.Invoice
{
    public class InvoiceSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private InvoiceSearchPageObjects _invoiceSearchPage;
        private SideBarPanelSearch _sideBarPanelSearch;
        private GridViewSection _GridViewSection;

        #endregion

        #region CONSTRUCTOR

        public InvoiceSearchPage(INewNavigator navigator, InvoiceSearchPageObjects invoiceSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, invoiceSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            invoiceSearchPage = (InvoiceSearchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _GridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _GridViewSection; }
        }

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public List<string> GetInvDateListFromDB()
        {
            return Executor.GetTableSingleColumn(InvoiceSearchSqlScripts.InvoiceDateList).ToList();
        }

        public void SetInputFieldByInputLabel(string label, string value)
        {
            _sideBarPanelSearch.SetInputFieldByLabel(label, value);
        }

        public void ClickOnSearchResultByInvoiceNumber(string invoiceNumber)
        {
            Console.WriteLine("Clicked on invocie result list with invoice number :{0}", invoiceNumber);
            JavaScriptExecutor.ExecuteClick(
                string.Format(InvoiceSearchPageObjects.SearchListByInvocieNumberSelectorTemplate, invoiceNumber),
                How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public double GetInvoiceCountByClaimNumberFromDatabase(string invoiceNumber)
        {
            return Executor.GetSingleValue(string.Format(InvoiceSearchSqlScripts.InvoiceCountByClaimNumber,
                invoiceNumber));
        }

        public long GetInvoiceCountByInvDate(string invDate)
        {
            return Executor.GetSingleValue(string.Format(InvoiceSearchSqlScripts.InvoiceCountByDate,
                invDate));
        }

        public string GetInvoiceDetailHeader()
        {
            return SiteDriver.FindElement(InvoiceSearchPageObjects.InvoiceDetailsHeaderCssSector,
                How.CssSelector).Text;
        }

        public string GetInvoiceDetailsValueByLabel(string label)
        {
            Console.WriteLine("Get value of invoice details from details section {0}", label);
            return JavaScriptExecutor
                .FindElement(string.Format(InvoiceSearchPageObjects.InvoiceDetailsValueByLabelCssLocator, label))
                .Text;
        }

        public double GetTotalSavingsByInvoiceNumberFromDB(string invNumber)
        {
            return Convert.ToDouble(Executor.GetSingleStringValue(string.Format(
                InvoiceSearchSqlScripts.TotalSavingsByInvoiceNumber,
                invNumber)));
        }

        public double GetTotalInvAmountByInvoiceNumberFromDB(string invNumber)
        {
            return Convert.ToDouble(Executor.GetSingleStringValue(string.Format(
                InvoiceSearchSqlScripts.TotalInvoiceAmountByInvoiceNumber,
                invNumber)));
        }

        public string GetTotalDebitCountByInvoiceNumber(string invNumber)
        {
            return Executor.GetSingleStringValue(string.Format(InvoiceSearchSqlScripts.DebitCountByInvoiceNumber,
                invNumber));
        }

        public string GetTotalCreditCountByInvoiceNumber(string invNumber)
        {
            return Executor.GetSingleStringValue(string.Format(InvoiceSearchSqlScripts.CreditCountByInvoiceNumber,
                invNumber));
        }

        public bool IsValueInRedColorByRowCol(int col = 2, int row = 1)
        {

            return SiteDriver.FindElement(
                    string.Format(GridViewSection.ValueInGridByRowColumnCssTemplate, row, col), How.CssSelector)
                .GetCssValue("color").Contains("rgba(255, 0, 0, 1)");
        }



        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
        }

        public bool IsExportIconDisabled()
        {
            return SiteDriver.FindElementAndGetAttribute(InvoiceSearchPageObjects.ExportIconTitleXpathSelector, How.XPath, "class")
                .Contains("is_disabled");
        }

        public string GetRemitContactHeader()
        {
            return SiteDriver
                .FindElement(InvoiceSearchPageObjects.RemitContactHeaderCssSector, How.CssSelector).Text;
        }

        public string GetClaimQuestionsInfo()
        {
            var text = SiteDriver
                .FindElement(InvoiceSearchPageObjects.ClaimQuestionsXpath, How.XPath).Text
                .Replace(System.Environment.NewLine, " ");
            return text;
        }

        public string GetInvoicingQuestionsInfo()
        {
            var text = SiteDriver
                .FindElement(InvoiceSearchPageObjects.InvoicingQuestionsXpath, How.XPath).Text
                .Replace(System.Environment.NewLine, " ");
            return text;
        }

        public string GetCotivitiTechInfo()
        {
            var text = SiteDriver
                .FindElement(InvoiceSearchPageObjects.VersendTechXpath, How.XPath).Text
                .Replace(System.Environment.NewLine, " ");
            return text;
        }

        public void ClickOnExportIcon()
        {
            JavaScriptExecutor.ExecuteClick(InvoiceSearchPageObjects.ExportIconXpathSelector, How.XPath);
        }

        public List<string> GetExportOptionList()
        {
            ClickOnExportIcon();
            var list = JavaScriptExecutor.FindElements(InvoiceSearchPageObjects.ExportOptionListXpath, How.XPath, "Text");
            ClickOnExportIcon();
            return list;

        }

        public void ClickOnExportOptionListRow(int row)
        {
            ClickOnExportIcon();
            JavaScriptExecutor.ExecuteClick(string.Format(InvoiceSearchPageObjects.ExportOptionXpathTemplate, row), How.XPath);
            Console.WriteLine("Click on {0} filter option", row);

            SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count == 1, 10000);

            ClickOnExportIcon();
        }

        #endregion

        public List<string> GetInvoiceNumberAndDatefromDB(string claseq)
        {
            var DataList =
                  Executor.GetCompleteTable(string.Format(InvoiceSearchSqlScripts.GetInvoiceNumberandDate, claseq));
            var newList = new List<String>();
            foreach (var row in DataList)
            {
                newList = row.ItemArray.Select(x => x.ToString()).ToList();
            }
            return newList;
        }
        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(InvoiceSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(InvoiceSearchPageObjects.FindButtonCssLocator,
                How.CssSelector);
            Console.WriteLine("Find Button Clicked");
            WaitForWorkingAjaxMessage();
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(InvoiceSearchPageObjects.FindButtonCssLocator,
                                 InvoiceSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }
    }
}
