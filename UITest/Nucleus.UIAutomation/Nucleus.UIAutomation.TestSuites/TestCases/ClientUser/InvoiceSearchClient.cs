using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageServices.Invoice;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Diagnostics;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.ChromeDownLoad;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class InvoiceSearchClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private InvoiceSearchPage _invoiceSearch;
        private ChromeDownLoadPage chromeDownLoad;

        #endregion

        #region OVERRIDE METHODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _invoiceSearch = QuickLaunch.NavigateToInvoiceSearch();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _invoiceSearch;
        }

        protected override void ClassCleanUp()
        {
            try
            {
                _invoiceSearch.CloseDbConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _invoiceSearch = _invoiceSearch.Logout().LoginAsClientUser().NavigateToInvoiceSearch();
            }

            if (_invoiceSearch.GetPageHeader() != PageHeaderEnum.InvoiceSearch.GetStringValue())
            {
                _invoiceSearch.ClickOnQuickLaunch().NavigateToInvoiceSearch();
            }
            _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
            if (!_invoiceSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                _invoiceSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            base.TestCleanUp();
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }


        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] //TANT-103
        public void Verify_Invoice_Search_Page()
        {
            StringFormatter.PrintMessageTitle("Verify Invoice Search Page is presented.");
            _invoiceSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.InvoiceSearch.GetStringValue(),
                "Invoice Search Page Should Be Presented");
            StringFormatter.PrintMessageTitle("Verify Data is presented.");
            var countOfInvoiceDateOptions =
                _invoiceSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Invoice Date").Count;
            var i = 2;
            var gridRowCount = _invoiceSearch.GetGridViewSection.GetGridRowCount();
            while (!(gridRowCount > 0) && i <= countOfInvoiceDateOptions)
            {
                _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListByIndex("Invoice Date", i);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();
                gridRowCount = _invoiceSearch.GetGridViewSection.GetGridRowCount();
                i++;
            }
            gridRowCount.ShouldBeGreater(0, "Result Should Be Presented");
        }

        [Test] //TANT 48
        public void Verify_landing_page_and_search_filters_and_their_default_values_in_the_NewInvoice_search_page()
        {
            try
            {
                _invoiceSearch.ClickOnQuickLaunch().NavigateToInvoiceSearch();
                //_newInvoiceSearch.NavigateToInvoiceSearch();
                StringFormatter.PrintMessageTitle(
                    "Verifying Search Bar Present by default and toggle button functionality");
                _invoiceSearch.GetSideBarPanelSearch.IsSideBarPanelOpen().ShouldBeTrue
                    ("Side bar panel should be opened when the user lands on the Invoice search page");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _invoiceSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("Find Invoice Search Panel on sidebar should hidden when toggle button is clicked.");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _invoiceSearch.IsExportIconDisabled().ShouldBeTrue("Export Icon disabled?");

                StringFormatter.PrintMessageTitle("Verifying filter options in the Invoice search page");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim #")
                    .ShouldBeNullorEmpty("Claim # field should be empty by default");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Seq")
                    .ShouldBeNullorEmpty("Claim Seq field should be empty by default");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Invoice #")
                    .ShouldBeNullorEmpty("Invoice # field should be empty by default");
                var list = _invoiceSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Invoice Date");
                list.RemoveAt(0);
                list.ShouldCollectionBeEqual(_invoiceSearch.GetInvDateListFromDB(),
                    "Available Invoice date list should match and be most recent date should be shown on top");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page Error pops up when any of the search criteria is not entered");
                _invoiceSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Search cannot be initiated without any criteria entered.");
                _invoiceSearch.ClosePageError();
                _invoiceSearch.GetSideBarPanelSearch.GetFieldErrorIconTooltipMessage("Invoice Date")
                    .ShouldBeEqual("Select valid Invoice Date.");
            }
            finally
            {
                _invoiceSearch.CloseAnyPopupIfExist();
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test]//TANT 48
        public void Verify_max_length_of_search_filters_and_upon_clicking_clear_link_clears_all_the_filters_in_the_invoice_search_page()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _invoiceSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim #", paramLists["Alphanumeric"]);
                _invoiceSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Claim #").ToString().ShouldBeEqual("50");

                _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", paramLists["NumberOnly"]);
                _invoiceSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Claim Seq").ToString()
                    .ShouldBeEqual("14");


                _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Invoice #", paramLists["NumberOnly"]);
                _invoiceSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Claim Seq").ToString()
                    .ShouldBeEqual("14");

                var invoiceDateOptions =
                    _invoiceSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Invoice Date");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Invoice Date")
                    .ShouldBeEqual(invoiceDateOptions[0], "Invoice date dropdown value defaults to All");
                _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Invoice Date", invoiceDateOptions[1]);

                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim #")
                    .ShouldBeNullorEmpty("Claim # field should clear");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Seq")
                    .ShouldBeNullorEmpty("Claim Seq field should clear");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Invoice #")
                    .ShouldBeNullorEmpty("Invoice # field should clear");
                _invoiceSearch.GetSideBarPanelSearch.GetInputValueByLabel("Invoice Date")
                    .ShouldBeEqual("All", "Invoice date dropdown value defaults to All");

                ValidateNumericOnlyField("Claim Seq", "Only numbers allowed.", paramLists["Alphanumeric"]);
                ValidateNumericOnlyField("Invoice #", "Only numbers allowed.", paramLists["Alphanumeric"]);
            }
            finally
            {
                _invoiceSearch.CloseAnyPopupIfExist();
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }

        }


        [Test] //TANT 48
        public void Verify_search_results_are_correct()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
               
                var claimNumber = paramLists["ClaimNumber"];
                var claSeq = paramLists["ClaimSequence"];
                var invoiceNumber = paramLists["InvoiceNumber"];
                var invoiceDate = paramLists["InvoiceDate"].Split(';').ToList();
                var invNumWithNegativedata = paramLists["InvNumWithNegativedata"];
                var invNumWithInvByClient = paramLists["InvNumWithInvByClient"];

                var expectedValues = paramLists["ExpectedData"].Split(';').ToList();
                _invoiceSearch.RefreshPage();
                _invoiceSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(0,
                    "When landing on the Invoice Search page, no result must be executed.");
                _invoiceSearch.GetGridViewSection.IsNoDataMessagePresentInLeftSection()
                    .ShouldBeTrue("Is No Data Available message displayed?");

                StringFormatter.PrintMessageTitle("Validation of default sorting");
                _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Invoice Date", invoiceDate[0]);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();
                _invoiceSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeSorted(true,
                    "Default sorting of Result set must be  in descending by Invoice Number.");

                StringFormatter.PrintMessage("Verify selected record is highlighted");


                _invoiceSearch.GetGridViewSection.ClickOnGridRowByRow();
                _invoiceSearch.GetGridViewSection.IsRowHighlighted(1)
                    .ShouldBeTrue("The selected  invoice row 1 should be highlighted");
                _invoiceSearch.GetGridViewSection.ClickOnGridRowByRow(2);

                _invoiceSearch.GetGridViewSection.IsRowHighlighted(2)
                    .ShouldBeTrue("The selected invoice row 2 should be highlighted");
                _invoiceSearch.GetGridViewSection.IsRowHighlighted(1)
                    .ShouldBeFalse("Row 1 should not be highlighted when another row is selected.");

                StringFormatter.PrintMessageTitle("Verify search results");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();

                _invoiceSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(1,
                    "When searched by Claim sequence, invoice record should be displayed");

                _invoiceSearch.GetGridViewSection.GetLabelInGridByColRow(1).ShouldBeEqual("Group ID:");
                _invoiceSearch.GetGridViewSection.GetLabelInGridByColRow(2).ShouldBeEqual("Invoice #:");
                _invoiceSearch.GetGridViewSection.GetLabelInGridByColRow(3).ShouldBeEqual("Invoice Date:");
                _invoiceSearch.GetGridViewSection.GetLabelInGridByColRow(4).ShouldBeEqual("Total Savings:");
                _invoiceSearch.GetGridViewSection.GetLabelInGridByColRow(5).ShouldBeEqual("Total Inv Amount:");

                var InvoiceNumberAndDate = _invoiceSearch.GetInvoiceNumberAndDatefromDB(claSeq);
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(1)
                    .ShouldBeEqual(expectedValues[0], "Group ID");
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(2)
                    .ShouldBeEqual(InvoiceNumberAndDate[0], "Invoice #");
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(InvoiceNumberAndDate[1], "Invoice Date");
                var exp = ConvertToDollar(_invoiceSearch.GetTotalSavingsByInvoiceNumberFromDB(expectedValues[1]));
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(4)
                    .ShouldBeEqual(exp, "Total Savings should equal the value obtained from database.");
                var toinamt =
                    ConvertToDollar(_invoiceSearch.GetTotalInvAmountByInvoiceNumberFromDB(expectedValues[1]));
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(5).ShouldBeEqual(toinamt,
                    "Total Inv Amount should equal the value obtained from database.");

                StringFormatter.PrintMessageTitle("Verify search result count by claim number");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim #", claimNumber);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();
                _invoiceSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(_invoiceSearch.GetInvoiceCountByClaimNumberFromDatabase(claimNumber),
                        "Invoice count when searched by Claim Number should equal to value obtained from database ");

                StringFormatter.PrintMessageTitle("Verify search results for multiple same invoice numbers");
                SearchByInvoiceNumber(invoiceNumber);
                _invoiceSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(1,
                    "When searched by Invoice number, invoice record with distinct invoice number should be displayed");
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(4)
                    .ShouldBeEqual(
                        ConvertToDollar(_invoiceSearch.GetTotalSavingsByInvoiceNumberFromDB(invoiceNumber)),
                        "Total Savings should equal the value obtained from database.");
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(5).ShouldBeEqual(
                    ConvertToDollar(_invoiceSearch.GetTotalInvAmountByInvoiceNumberFromDB(invoiceNumber)),
                    "Total Inv Amount should equal the value obtained from database.");

                StringFormatter.PrintMessageTitle("Verify negative values");

                SearchByInvoiceNumber(invNumWithNegativedata);
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(4)
                    .ShouldBeEqual(
                        ConvertToDollar(_invoiceSearch.GetTotalSavingsByInvoiceNumberFromDB(invNumWithNegativedata)),
                        "Total Savings should equal the value obtained from database.");
                _invoiceSearch.IsValueInRedColorByRowCol(4)
                    .ShouldBeTrue("Negative values should display in red color");
                _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(5)
                    .ShouldBeEqual(
                        ConvertToDollar(
                            _invoiceSearch.GetTotalInvAmountByInvoiceNumberFromDB(invNumWithNegativedata)),
                        "Total Inv Amount should equal the value obtained from database.");
                _invoiceSearch.IsValueInRedColorByRowCol(5)
                    .ShouldBeTrue("Negative values should display in red color");

                StringFormatter.PrintMessageTitle("Verify grid load more");
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Invoice Date", invoiceDate[1]);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();
                var loadMoreValue = _invoiceSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                var invoiceCountByDate = _invoiceSearch.GetInvoiceCountByInvDate("JUL-13");
                numbers[1].ShouldBeEqual(invoiceCountByDate,
                    "Invoice count should equal to value obtained from database.");
                _invoiceSearch.ClickOnLoadMore();
                _invoiceSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(numbers[0] + 25, "Clicking load more must display 25 more data");

                StringFormatter.PrintMessageTitle("Verify Group ID for Invoice By Client");
                SearchByInvoiceNumber(invNumWithInvByClient);
                if (!_invoiceSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
                    _invoiceSearch.GetGridViewSection.GetValueInGridByColRow(1)
                        .ShouldBeNullorEmpty("Group ID should be null when Invoice By is Client");
            }
            finally
            {
                
                _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }



        [Test]//TANT 48
        public void Verify_invoice_result_secondary_details()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var invNumber = paramLists["InvoiceNumber"]; //5382
                var invBy = paramLists["InvoiceBy"];
                var invNumWithCreditType = paramLists["InvoiceNumberWithCreditType"];
                _invoiceSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                SearchByInvoiceNumber(invNumber);
                if (!_invoiceSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
                {
                    _invoiceSearch.ClickOnSearchResultByInvoiceNumber(invNumber);
                    _invoiceSearch.GetInvoiceDetailHeader().ShouldBeEqual("Invoice Details");
                    _invoiceSearch.GetInvoiceDetailsValueByLabel("Inv By").ShouldBeEqual(invBy);
                    _invoiceSearch.GetInvoiceDetailsValueByLabel("Net Inv %")
                        .ShouldBeEqual(CalculateNetInvoicePercentage(invNumber));
                    _invoiceSearch.GetInvoiceDetailsValueByLabel("Debit Count").ShouldBeEqual(
                        _invoiceSearch.GetTotalDebitCountByInvoiceNumber(invNumber),
                        "Debit Amount should equal to database value");
                    _invoiceSearch.GetInvoiceDetailsValueByLabel("Credit Count")
                        .ShouldBeEqual(_invoiceSearch.GetTotalCreditCountByInvoiceNumber(invNumber));


                    _invoiceSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Invoice #", invNumWithCreditType);
                    _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _invoiceSearch.WaitForWorkingAjaxMessage();
                    _invoiceSearch.ClickOnSearchResultByInvoiceNumber(invNumWithCreditType);
                    _invoiceSearch.GetInvoiceDetailsValueByLabel("Credit Count")
                        .ShouldBeEqual(_invoiceSearch.GetTotalCreditCountByInvoiceNumber(invNumWithCreditType),
                            "Invoice Record Type with value C must have credit ammount and must be equal to database value.");
                }
            }
            finally
            {
                _invoiceSearch.RefreshPage();
            }

        }

        [Test, Category("IENonCompatible")]
        public void Verify_download_of_different_invoice_reports()
        {
            var url = "";
            try
            {

                var expectedFileNameList = new List<string>{ "InvoiceSearchResultsDetails.xlsx",
                    string.Format("{0}_INVOICEDETAILS_.pdf", ClientEnum.SMTST), string.Format("{0}_INVOICESUMMARY_.pdf", ClientEnum.SMTST)};
                const string invoiceDate = "Feb 2013";
                _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Invoice Date", invoiceDate);
                _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _invoiceSearch.WaitForWorkingAjaxMessage();
                _invoiceSearch.ClickOnExportIcon();
                _invoiceSearch.ClickOnExportOptionListRow(1);
                _invoiceSearch.ClickOnExportOptionListRow(2);
                _invoiceSearch.ClickOnExportOptionListRow(3);
                url = _invoiceSearch.CurrentPageUrl;
                chromeDownLoad = _invoiceSearch.NavigateToChromeDownLoadPage();
                var fileNameList = chromeDownLoad.GetFileNameList();
                fileNameList = fileNameList.Select(x => Regex.Replace(x, @"\d", "").Trim()).ToList();
                fileNameList = fileNameList.Select(x => x.Replace("()", "").Replace(" ", "")).ToList();
                fileNameList.ShouldCollectionBeEqual(expectedFileNameList, "List should match");

            }
            finally
            {
                if (chromeDownLoad != null)
                    _invoiceSearch = chromeDownLoad.ClickBrowserBackButton<InvoiceSearchPage>(url);
            }
        }
        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            _invoiceSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            _invoiceSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Invoice Date", "Jul 2013");
            _invoiceSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                .ShouldBeTrue("Find Button Should be disabled while the search is active.");
            _invoiceSearch.WaitForWorkingAjaxMessage();
            _invoiceSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search results should be displayed");
            _invoiceSearch.CheckIfFindButtonIsEnabled()
                .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
        }
        #endregion



        #region PRIVATE METHODS


        private void SearchByInvoiceNumber(string invNum)
        {
            _invoiceSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _invoiceSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Invoice #", invNum);
            _invoiceSearch.GetSideBarPanelSearch.ClickOnFindButton();
            StringFormatter.PrintMessage(string.Format("Search By Invoice Number : {0}", invNum));
            _invoiceSearch.WaitForWorkingAjaxMessage();
        }

        private String CalculateNetInvoicePercentage(string invNum)
        {
            var totInvAmt = _invoiceSearch.GetTotalInvAmountByInvoiceNumberFromDB(invNum);
            var totSavingAmt = _invoiceSearch.GetTotalSavingsByInvoiceNumberFromDB(invNum);
            var netInvPer = Math.Round((totInvAmt / totSavingAmt) * 100, 0);


            return netInvPer + "%";

        }

        private String ConvertToDollar(double amount)
        {
            var val = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            return val.ToString("C2");
        }

        private void ValidateNumericOnlyField(string label, string message, string character)
        {
            _invoiceSearch.SetInputFieldByInputLabel(label, character);

            _invoiceSearch.GetPageErrorMessage()
                .ShouldBeEqual(message,
                    "Popup Error message is shown when unexpected " + character + "is passed to " + label);
            _invoiceSearch.ClosePageError();
        }
        #endregion
    }
}
