using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.Data;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class InvoiceDetail : AutomatedBase
    {
        #region PRIVATE FIELDS

        private InvoiceSearchPage _invoiceSearch;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
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

        #endregion

        #region TEST SUITES

        [Test]//US14406
        public void Verify_that_InvHash_is_present_where_previously_PrevInvHash_was_present()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                                                                            TestExtensions.TestName);
            string invoiceNo = paramLists["InvoiceNumber"];
            _invoiceSearch.SearchByInvoiceNumber(invoiceNo);
            InvoiceDetailPage invoiceDetail = null;
            try
            {
                invoiceDetail = _invoiceSearch.ClickOnInvoiceNumberToOpenInvoiceDetail(invoiceNo);
                invoiceDetail.GetHeaderLabelAtIndex(paramLists["InvoiceNoIndex"]).ShouldBeEqual("Inv #", "Changed name of 'Prev Inv #' at index 25");
            }
            finally
            {
                if(invoiceDetail != null)
                {
                   _invoiceSearch = invoiceDetail.ClickOnBackButton();
                   _invoiceSearch.ClickClear();
                }
            }
        }

        [Test]//US14406
        public void Verify_that_Inv_Date_is_present_at_left_of_InvNo()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                                                                            TestExtensions.TestName);
            string invoiceNo = paramLists["InvoiceNumber"];
            _invoiceSearch.SearchByInvoiceNumber(invoiceNo);
            InvoiceDetailPage invoiceDetail = null;
            try
            {
                invoiceDetail = _invoiceSearch.ClickOnInvoiceNumberToOpenInvoiceDetail(invoiceNo);
               // invoiceDetail.GridColumnReset();
                invoiceDetail.GetHeaderLabelAtIndex((Int32.Parse(paramLists["InvoiceNoIndex"]) - 1).ToString()).ShouldBeEqual("Inv Date", "Grid Header Present at left of Inv # (25th Index)");
            }
            finally
            {
                if (invoiceDetail != null)
                {
                    _invoiceSearch = invoiceDetail.ClickOnBackButton();
                    _invoiceSearch.ClickClear();
                }
            }
        }

        [Test, Category("SmokeTest")]//US14406
        public void Verify_that_if_previous_invoice_exists_then_invoice_date_column_will_display_date_of_previous_invoice()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string invoiceNo = paramLists["InvoiceNumber"];
            string invoiceRowIndex = paramLists["DifferentInvoiceRowIndex"];
            _invoiceSearch.SearchByInvoiceNumber(invoiceNo);
            InvoiceDetailPage invoiceDetail = null;
            try
            {
                invoiceDetail = _invoiceSearch.ClickOnInvoiceNumberToOpenInvoiceDetail(invoiceNo);
                invoiceDetail.ScrollToLastOfHeader("26");
                string invoiceDate = invoiceDetail.GetValueOfInvoiceDateForRow(invoiceRowIndex);
                InvoiceDetailPage invoiceDetailPopup = null;
                try
                {
                    invoiceDetailPopup = invoiceDetail.ClickOnInvoiceNumber(invoiceRowIndex);
                    invoiceDetailPopup.GetInvoiceDateLabelText().ShouldBeEqual(invoiceDate, "Invoice Date From Invoice Date Column and Invoice Date Label");
                }
                finally
                {
                    if(invoiceDetailPopup != null)
                    {
                        invoiceDetailPopup.CloseInvoiceDetailPopup();
                    }
                }
            }
            finally
            {
                if (invoiceDetail != null)
                {
                    _invoiceSearch = invoiceDetail.ClickOnBackButton();
                    _invoiceSearch.ClickClear();
                }
            }
        }

        [Test, Category("SmokeTest")]//US14406
        public void Verify_that_when_page_is_loaded_the_grid_result_will_be_sorted_by_claim_sequenece_ascending_and_when_multiple_claim_sequence_are_present_they_are_sorted_by_invoice_date()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            IDictionary<string, string> monthNum = DataHelper.GetMappingData(FullyQualifiedClassName, "MonthsNumber");
            string invoiceNo = paramLists["InvoiceNumber"];
            _invoiceSearch.SearchByInvoiceNumber(invoiceNo);
            InvoiceDetailPage invoiceDetail = null;
            try
            {
                invoiceDetail = _invoiceSearch.ClickOnInvoiceNumberToOpenInvoiceDetail(invoiceNo);
                List<string> claimSequences = invoiceDetail.GetAllClaimSequenceFromDataGrid();
                int limit = claimSequences.Count - 1;
                for (int count = 0; count < limit; count++)
                {
                    int countPlus = count + 1;
                    string[] first = claimSequences[count].Split('-');
                    string[] second = claimSequences[countPlus].Split('-');
                    string[] firstDate = invoiceDetail.GetValueOfInvoiceDateForRow(countPlus.ToString()).Split(' ');
                    string[] secondDate = invoiceDetail.GetValueOfInvoiceDateForRow((countPlus + 1).ToString()).Split(' ');
                    int compareValue = string.Compare(first[0], second[0], StringComparison.InvariantCultureIgnoreCase);

                    if( compareValue < 0)
                    {
                        Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                        if(limit - count == 1)
                        {
                            Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                        }
                    }
                    else if(compareValue == 0)
                    {
                        int compareSubValue = string.Compare(first[1], second[1], StringComparison.InvariantCultureIgnoreCase);
                        if(compareSubValue < 0)
                        {
                            Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                            if (limit - count == 1)
                            {
                                Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                            }
                        }
                        else if(compareSubValue == 0)
                        {
                            //Check for invoice date
                            int compareYearDate = string.Compare(firstDate[1], secondDate[1], StringComparison.InvariantCultureIgnoreCase);
                            if (compareYearDate < 0)
                            {
                                Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                                if (limit - count == 1)
                                {
                                    Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                                }
                            }
                            else if(compareYearDate == 0)
                            {
                                int compareMonthDate = string.Compare(monthNum[firstDate[0]], monthNum[secondDate[0]], StringComparison.InvariantCultureIgnoreCase);
                                if(compareMonthDate < 1)
                                {
                                    Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                                    if (limit - count == 1)
                                    {
                                        Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                                    Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                                    this.AssertFail("Claim Sequence With Descending Order of Invoice Date.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                            Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                            this.AssertFail("Claim Sequence With Descending Order.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus, claimSequences[count], firstDate[0], firstDate[1]);
                        Console.WriteLine("{0}: Claim Sequence {1} with Invoice Date {2} {3}.", countPlus + 1, claimSequences[countPlus], secondDate[0], secondDate[1]);
                        this.AssertFail("Claim Sequence With Descending Order.");
                    }
                }
                if(limit == 1)
                {
                    Console.WriteLine("Single row of Claim Sequence Present.");
                }
                else if(limit == 0)
                {
                    this.AssertFail("No Data");
                }
            }
            finally
            {
                if (invoiceDetail != null){
                    _invoiceSearch = invoiceDetail.ClickOnBackButton();
                    _invoiceSearch.ClickClear();
                }
            }
        }

        [Test]//US14404
        public void Verify_if_the_user_invoices_by_client_then_the_user_will_not_see_Group_ID_label_or_value_in_invoice_detail_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                TestExtensions.TestName);

            string invoiceNumber = paramLists["InvoiceNumber"];
            _invoiceSearch.SearchByInvoiceNumber(invoiceNumber);

            InvoiceDetailPage invoiceDetail = null;
            try
            {
                invoiceDetail = _invoiceSearch.ClickOnInvoiceNumberToOpenInvoiceDetail(invoiceNumber);
                invoiceDetail.GroupIDLabelOrValueIsPresent()
                    .ShouldBeFalse("Group Id label or value is present");
            }
            finally
            {
                if (invoiceDetail != null)
                {
                    _invoiceSearch = invoiceDetail.ClickOnBackButton();
                    _invoiceSearch.ClickClear();
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        #endregion

    }
}
