using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealSummaryClient
    {
        //#region PRIVATE FIELDS


        //private AppealSearchPage _appealSearch;
        //private AppealSummaryPage _appealSummary;
        //private NewPopupCodePage _newPopupCode;
        //private AppealProcessingHistoryPage _appealProcessingHx;
        //private AppealManagerPage _appealManagerPage;
        //private AppealManagerPage _appealManager;
        //private AppealCreatorPage _appealCreator;




        //#endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        #endregion

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        automatedBase.CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    } 
        //}

        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    automatedBase.CurrentPage = _appealSearch;
        //}

        //protected override void TestCleanUp()
        //{
        //    _appealSearch.CloseAnyTabIfExist();
        //    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        automatedBase.CurrentPage = QuickLaunch = automatedBase.CurrentPage.Logout().LoginAsClientUser();
        //       _appealSearch= automatedBase.CurrentPage.NavigateToAppealSearch();
        //    }

        //    if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
        //    {
        //        _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
        //    }
        //    if (_appealSummary != null && _appealSummary.GetPageHeader() == "Appeal Summary")
        //    {
        //        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
        //    }

        //    if (!_appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
        //        _appealSearch.ClickOnSearchButton();

        //    _appealSearch.ClearAll();
        //    _appealSearch.SelectAllAppeals();
        //    base.TestCleanUp();

        //}
        //#endregion

        #region TEST SUITES

        [Test] //US67260
        [Category("EdgeNonCompatible")]
        public void Verify_Appeal_letter_download_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.Complete.GetStringDisplayValue());
                _appealSearch.ClickOnFindButtonAndWait();
                _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                var appealLetter = _appealSummary.ClickAppealLetter();

                var fileName = _appealSummary.ClickOnDownloadPDFAndGetFileName(appealLetter);
                File.Exists(@"C:/Users/uiautomation/Downloads/" + fileName)
                    .ShouldBeTrue("Appeal Letter should be downloaded");

                _appealSummary.CloseAnyPopupIfExist();

                _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }

        [Test]//US58183
        public void Verify_page_should_redirect_to_appeal_search_when_closed_the_appeal_by_entering_appeal_summary_via_direct_url()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManagerPage;
                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AppealSequence", "Value");

                _appealSearch.SelectAllAppeals();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);

                try
                {
                    var url = automatedBase.CurrentPage.CurrentPageUrl;
                    var status = _appealSummary.GetAppealDetails(1, 4);
                    if (status != "Complete")
                    {
                        automatedBase.CurrentPage = _appealManagerPage =
                            _appealSummary.Logout()
                                .LoginAsHciAdminUser()
                                .NavigateToAppealManager();
                        _appealManagerPage.SearchByAppealSequence(appealSeq);
                        _appealManagerPage.ClickOnEditIcon();
                        _appealManagerPage.SelectDropDownListbyInputLabel("Status", "Complete");
                        _appealManagerPage.ClickOnSaveButton();
                        _appealManagerPage.WaitForWorking();

                        automatedBase.CurrentPage =
                            _appealSearch =
                                _appealManagerPage
                                    .Logout()
                                    .LoginAsClientUser()
                                    .NavigateToAppealSearch();


                    }
                    else
                        automatedBase.CurrentPage =
                            _appealSearch =
                                _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SwitchToNavigateAppealSummaryViaUrl(url);
                    automatedBase.CurrentPage = _appealSearch =
                        _appealSummary.ClickOnApproveIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage.GetPageHeader()
                        .ShouldBeEqual("Appeal Search", "Page Should Redirect to Appeal Search");
                    automatedBase.CurrentPage.PageUrl.ShouldBeEqual(
                        automatedBase.EnvironmentManager.ApplicationUrl + PageUrlEnum.AppealSearch.GetStringValue(),
                        "Page Url Should be Appeal Search");
                    automatedBase.CurrentPage = _appealSearch;


                    _appealSearch.SelectAllAppeals();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);
                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual("Closed", "Appeal Should Closed when appeal is approved by client user");

                }
                finally
                {

                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                }
            }
        }

        [Test]//US53194
        public void Verify_dollar_values_in_appeal_line()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManagerPage;
                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);
                
                try
                {
                    
                    ClaimActionPage claimAction = _appealSummary.ClickOnClaimNoAndSwitchWindow();
                    claimAction.ClickOnDollarIcon();
                    var allowed = claimAction.GetClaimDollarDetailsByLineWithLabel("Allowed");
                    var adjPaid = claimAction.GetClaimDollarDetailsByLineWithLabel("Adj Paid");
                    var sugPaid = claimAction.GetClaimDollarDetailsByLineWithLabel("Sug Paid");
                    _appealSummary.CloseAnyPopupIfExist();
                    var firstLineValue = _appealSummary.GetFirstLineOfAppealLineDataByAppealLineRow();
                    new[] { "Allowed", "Adj Paid", "Sug Paid" }.All(firstLineValue.Contains)
                        .ShouldBeTrue(
                            "Allowed,Adj Paid and Sug Paid Should be on the first line of the appeal line data");
                    new[] { allowed, adjPaid, sugPaid }.All(firstLineValue.Contains)
                        .ShouldBeTrue(
                            "Allowed,Adj Paid and Sug Paid value  Should be consistent with values shown on Claim Action");
                    allowed.Contains('$').ShouldBeTrue("Allowed is in Currency Format");
                    adjPaid.Contains('$').ShouldBeTrue("AdjPaid is in Currency Format");
                    sugPaid.Contains('$').ShouldBeTrue("sugPaid is in Currency Format");

                }
                finally
                {
                    _appealSummary.CloseAnyPopupIfExist();
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
            }
        }

        [Test]//US50604, TE-580
        public void Verify_when_appeal_is_closed_for_complete_status_without_any_error()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManagerPage;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AppealSequence", "Value");
                _appealSearch.SelectAllAppeals();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);

                try
                {
                    var status = _appealSummary.GetAppealDetails(1, 4);
                    if (status != "Complete")
                    {
                        _appealManagerPage =
                            _appealSummary.Logout()
                                .LoginAsHciAdminUser()
                                .NavigateToAppealManager();
                        _appealManagerPage.SearchByAppealSequence(appealSeq);
                        _appealManagerPage.ClickOnEditIcon();
                        _appealManagerPage.SelectDropDownListbyInputLabel("Status", "Complete");
                        _appealManagerPage.ClickOnSaveButton();
                        _appealManagerPage.WaitForWorking();
                        automatedBase.CurrentPage =
                            _appealSearch = _appealManagerPage.Logout().LoginAsClientUser().NavigateToAppealSearch();
                        _appealSearch.SelectAllAppeals();
                        automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);
                    }

                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.ClickOnFilterOptionListRow(1); //TE-580
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                    automatedBase.CurrentPage = _appealSearch =
                        _appealSummary.ClickOnApproveIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);
                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual("Closed", "Appeal Should Closed when appeal is approved by client user");

                    _appealSummary.ClickMoreOption();
                    AppealProcessingHistoryPage _appealProcessingHistory = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(2, 3)
                        .ShouldBeEqual("Close", "Closed Appeal Record should added in Appeal Processing Hisory Page");
                    _appealProcessingHistory.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                }
                finally
                {

                    _appealSummary.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    automatedBase.CurrentPage = _appealManagerPage =
                        _appealSearch.Logout()
                            .LoginAsHciAdminUser()
                            .NavigateToAppealManager();
                    _appealManagerPage.SearchByAppealSequence(appealSeq);
                    _appealManagerPage.ClickOnEditIcon();
                    _appealManagerPage.SelectDropDownListbyInputLabel("Status", "Complete");
                    _appealManagerPage.ClickOnSaveButton();
                    _appealManagerPage.WaitForWorking();
                    automatedBase.CurrentPage =
                        _appealSearch = _appealManagerPage.Logout().LoginAsClientUser().NavigateToAppealSearch();

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41380 + US57587 story added that create date only dispaly for client user////US53193 story added that state display for client user only +CAR-2967
        public void Verify_that_navigation_with_basic_appeal_information_and_presence_of_header_icons()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var claimSeqWithState = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithState", "Value");
                var appealNumber = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AppealNumber", "Value");
                var externalDocId = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ExternalDocId", "Value");
                var product = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "Product", "Value");

                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                var appeal = _appealSearch.GetAppealSequenceByStatus();
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage();
                try
                {
                    StringFormatter.PrintMessageTitle("Verification of Appeal Summary Page Opened");
                    _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Verification of Page");
                    _appealSummary.GetAppealSequenceOnHeader()
                        .ShouldBeEqual(appeal, "Appeal Seq Should should be displayed");

                    StringFormatter.PrintMessageTitle("Verification of Appeal Summary Details Information");

                    _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual("Appeal", "Appeal Type");
                    _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual("Urgent", "Priority");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");

                    _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual("Record Review", "Appeal Type");
                    _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual("Normal", "Priority");
                    VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealDetails(1, 3), " Due ");
                    _appealSummary.GetAppealDetails(1, 4).ShouldBeEqual("Complete", "Status");
                    _appealSummary.GetAppealDetails(1, 5).ShouldBeNullorEmpty("State is empty if no data available");
                    _appealSummary.GetAppealDetails(2, 1).ShouldBeEqual(appealNumber, "Appeal Number");
                    VerifyThatNameIsInCorrectFormat(_appealSummary.GetAppealDetails(2, 2), "Created By");
                    _appealSummary.GetAppealDetails(2, 4).ShouldBeEqual(product, "Product");
                    _appealSummary.GetAppealDetails(2, 5).ShouldBeEqual(externalDocId, "External Doc Id");

                    _appealSummary.IsAppealDetailFieldPresentByLabel("Created Date")
                        .ShouldBeTrue("Created Date display?");
                    VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealDetailFieldPresentByLabel("Created Date"),
                        "Created Date");

                    StringFormatter.PrintMessageTitle("Verification of Presence of Appeal Summary Header Icons");
                    _appealSummary.IsClosedAppealIconEnabled().ShouldBeTrue("Close Appeal Icon Present");
                    //_appealSummary.IsAppealNoteEnabled().ShouldBeTrue("Appeal Note Present");
                    _appealSummary.IsSearchIconPresent().ShouldBeTrue("Search Icon Present");
                    _appealSummary.ClickMoreOption();
                    _appealSummary.IsAppealProcessingHxLinkPresent().ShouldBeTrue("Appeal Processing History Present");
                    _appealSummary.ClickMoreOption();
                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SetClaimSequence(claimSeqWithState);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual("OH", "State");

                }
                finally
                {

                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41386
        public void Verify_proper_display_on_edit_appeal_information_and_should_display_changed_value_after_updated_without_refresh()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Closed");
                try
                {
                    _appealSummary.IsEditIconDisabled().ShouldBeTrue("Edit Icon Should be Disabled for Closed Status");
                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                    _appealSummary.IsEditIconDisabled()
                        .ShouldBeTrue("Edit Icon Should be Disabled for Complete Status");
                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Open");
                    var appealNo = _appealSummary.GetAppealDetails(2, 1);
                    var externalDocId = "";
                    if (_appealSummary.IsExternalDocIdPresentInAppealDetails())
                        externalDocId = _appealSummary.GetAppealDetails(2, 5);
                    var status = _appealSummary.GetAppealDetails(1, 4);

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.GetEditAppealInputValue(2).ShouldBeEqual(appealNo, "Appeal No Should be same");
                    _appealSummary.GetEditAppealInputValue(3)
                        .ShouldBeEqual(externalDocId, "External Doc Id Should be same");

                    _appealSummary.SetEditAppealInputValue(2, "");
                    _appealSummary.SetEditAppealInputValue(3, "");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    string.IsNullOrEmpty(_appealSummary.GetAppealDetails(2, 1))
                        .ShouldBeTrue("Appeal No Should allow Empty Value");
                    if (!_appealSummary.IsExternalDocIdPresentInAppealDetails())
                        Console.WriteLine("External Doc Id Should not be dispalyed for empty value");
                    else _appealSummary.GetAppealDetails(2, 5).ShouldBeNullorEmpty("External Doc Id Should be null");
                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual(status, "Status should not be changed when External Doc Id is empty");

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.SetEditAppealInputValue(2, "1111");
                    _appealSummary.SetEditAppealInputValue(3, "2222");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.GetAppealDetails(2, 1).ShouldBeEqual("1111", "Appeal No Should updated");
                    _appealSummary.GetAppealDetails(2, 5).ShouldBeEqual("2222", "External Doc Id Should updated");
                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual(status, "Status should not be changed when External Doc Id added");

                    _appealSummary.ClickMoreOption();

                    AppealProcessingHistoryPage _appealProcessingHistory = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("Edit", "Action Should be Edit"); //Status
                    _appealProcessingHistory.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                }
                finally
                {
                    _appealSummary.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41385
        public void Verify_that_appeal_information_with_proper_display_of_claim_line()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                NewPopupCodePage _newPopupCode;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var clainNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimNo", "Value");
                var provider = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "Provider", "Value");
                var specialty = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "Specialty", "Value");
                var altClaim = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AltClaim", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary = _appealSearch.ClickOnAppealSequence(2);

                try
                {
                    _appealSummary.IsAppealLineSectionPresent().ShouldBeTrue("Appeal Lines Section displayed");
                    _appealSummary.GetClaimSectionRowValueByLabel("Claim Sequence")
                        .ShouldBeEqual(claimSeq, "Claim Sequence dispalyed and Equal");
                    _appealSummary.GetClaimSectionRowValueByLabel("Claim No")
                        .ShouldBeEqual(clainNo, "Claim No dispalyed and Equal");
                    _appealSummary.GetClaimSectionRowValueByLabel("Provider")
                        .ShouldBeEqual(provider, "Provider dispalyed and Equal");
                    _appealSummary.GetClaimSectionRowValueByLabel("S:")
                        .ShouldBeEqual(specialty, "Specialty dispalyed and Equal");
                    _appealSummary.GetClainNoList().IsInAscendingOrder()
                        .ShouldBeTrue("Clain Line should be in Ascending Order");
                    _appealSummary.GetClaimSectionRowLabeleByLabel("Claim No")
                        .ShouldBeEqual(altClaim, "Alternate Claim Title displayed and equal");

                    _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 2, 1)
                        .ShouldBeEqual("A", "Adjust displayed and equal");
                    _appealSummary.GetClaimLineSectionRowValueByRowCol(2, 2, 1)
                        .ShouldBeEqual("D", "Deny displayed and equal");
                    _appealSummary.GetClaimLineSectionRowValueByRowCol(3, 2, 1)
                        .ShouldBeEqual("N", "No Docs displayed and equal");
                    _appealSummary.GetClaimLineSectionRowValueByRowCol(4, 2, 1)
                        .ShouldBeEqual("P", "Pay displayed and equal");
                    _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 1, 1).ShouldBeEqual("1", "Line No displayed");
                    _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 1, 3)
                        .ShouldNotBeEmpty("Date of Service displayed");
                    var procCode = _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 1, 4);

                    _newPopupCode = _appealSummary.ClickOnProcCodeandSwitch("CPT Code - " + procCode, 1, 1, 4);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Header Text");
                    _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                    _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                    _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                    _newPopupCode.ClosePopupOnBackAppealSummaryPage("CPT Code - " + procCode);

                    _appealSummary.GetProcDescriptionToolTipByRowCol(1, 1)
                        .ShouldBeEqual(_appealSummary.GetClaimLineSectionRowValueByRowCol(1, 1, 5),
                            "Tooltip should match text for Proc Description");


                    var revCode = _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 1, 6);
                    _newPopupCode = _appealSummary.ClickOnRevenueCodeandSwitch("Revenue Code - " + revCode, 1, 1, 6);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("Revenue Code", "Header Text");
                    _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                    _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                    _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                    _newPopupCode.ClosePopupOnBackAppealSummaryPage("Revenue Code - " + revCode);

                    _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 2, 3)
                        .ShouldNotBeEmpty("Bill amount displayed");

                    ClaimActionPage
                        newClaimAction =
                            _appealSummary
                                .ClickOnClaimNoAndSwitchWindow(); //story modifed according to  US52204,now claim no will popup claim action for client only
                    var uniqueFlagList = newClaimAction.GetFlagListForClaimLine(2).Distinct().ToList();
                    ; //get only unique flag list for first claim line of claimAction
                    _appealSummary.CloseAnyPopupIfExist();
                    _appealSummary.GetFlagList().ShouldCollectionBeEqual(uniqueFlagList,
                        "Unique flags are present per line and flag order matches that of Claim Action");

                    _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 2, 4)
                        .ShouldBeEqual("2", "Appeal Level Should Equal");
                    _appealSummary.IsGreyAppealIconPresent(2).ShouldBeTrue("Grey Icon Present");
                    _appealSummary.ClickOnClaimLineDiv(3);
                    _appealSummary.GetNoPreviousAppealsMessage()
                        .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");
                    _appealSummary.ClickOnClaimLineDiv(1);
                    _appealSummary.GetPreviousAppealList().Count.ShouldBeGreater(0, "Previous Appeal Should Present ");
                    _appealSummary.GetAppealLevelToolTipByRowCol(1, 2)
                        .ShouldBeEqual("2 Appeals", "Title should equal");
                }
                finally
                {

                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41391
        public void Verify_proper_validation_message_and_tooltip_for_close_appeal_document()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                AppealProcessingHistoryPage appealProcessingHx = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary = _appealSearch.ClickOnAppealSequence(2);

                try
                {
                    _appealSummary.IsClosedAppealIconEnabled()
                        .ShouldBeTrue("Is Closed Appeal Icon Present and Enabled for <Complete> Status");
                    _appealSummary.GetClosedAppealEnabledIconToolTip()
                        .ShouldBeEqual("Select to close appeal",
                            "ToolTip Mesage for Closed Appeal Icon for <Complete> status");
                    var status = _appealSummary.GetStatusValue();
                    _appealSummary.ClickOnClosedAppealIcon();
                    _appealSummary.IsPageErrorPopupModalPresent().ShouldBeTrue("Confirmation Popup Present");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("This appeal will be Closed. Would you like to continue?");
                    _appealSummary.ClickOkCancelOnConfirmationModal(false);
                    _appealSummary.IsAppealLetterEnabled()
                        .ShouldBeTrue("Is Closed Appeal Icon Present and Enabled after cancel button click");
                    _appealSummary.GetStatusValue()
                        .ShouldBeEqual(status, "Status Should be same after cancel button click");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);

                    _appealSummary.IsClosedAppealIconDisabled()
                        .ShouldBeTrue("Is Closed Appeal Icon Present and Disabled for <Closed> Status");
                    _appealSummary.GetClosedAppealDisabledIconToolTip()
                        .ShouldBeEqual("This appeal has been closed",
                            "ToolTip Mesage for Closed Appeal Icon for <Closed> status");
                    status = _appealSummary.GetStatusValue();
                    _appealSummary.ClickMoreOption();
                    appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    var row = appealProcessingHx.GetResultGridRowCount();
                    appealProcessingHx.GetAppealAuditGridTableDataValue(row - 17, 3)
                        .ShouldBeEqual("Close", "Action :"); //Action
                    appealProcessingHx.GetAppealAuditGridTableDataValue(row - 17, 6)
                        .ShouldBeEqual(status, "Status of Appeal"); //Status
                    appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                }
                finally
                {
                    if (appealProcessingHx != null && _appealSummary.IsAppealProcessingHistory())
                    {
                        appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                    }

                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        if (_appealSummary.IsPageErrorPopupModalPresent())
                            _appealSummary.ClickOkCancelOnConfirmationModal(false);
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41387
        public void Verify_view_appeal_letter_enabled_only_for_closed_or_complete_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                AppealLetterPage appealLetter = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary = _appealSearch.ClickOnAppealSequence(1);

                try
                {
                    _appealSummary.IsAppealLetterEnabled().ShouldBeTrue("Appeal Letter Should be Enabled for Closed");
                    try
                    {
                        appealLetter = _appealSummary.ClickAppealLetter();
                        appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle,
                            "Appeal Letter Popup", true);
                    }
                    finally
                    {
                        if (appealLetter != null)
                            appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                    }

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(2);
                    _appealSummary.IsAppealLetterDisabled().ShouldBeTrue("Appeal Letter Should be Disabled for Open");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(3);
                    _appealSummary.IsAppealLetterEnabled().ShouldBeTrue("Appeal Letter Should be Enabled for Complete");

                    appealLetter = null;
                    try
                    {
                        appealLetter = _appealSummary.ClickAppealLetter();
                        appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle,
                            "Appeal Letter Popup", true);
                    }
                    finally
                    {
                        if (appealLetter != null)
                            appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                    }

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    if (appealLetter != null)
                        Console.WriteLine("appeal letter is not null");
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US41390
        public void Verify_appeal_line_details_values_and_previous_appeals_should_be_sorted_in_order_of_creation()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var claimSequenceHavingMultipleLine = automatedBase.DataHelper.GetSingleTestData(
                    FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceHavingMultipleLine", "Value");
                var notesVisibleToClient = "Client Visible";
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verification of appeal line details when status is not closed or complete");
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                    _appealSummary.GetAppealLineDetailsEmptyMessage()
                        .ShouldBeEqual(
                            "This appeal is in process with Cotiviti. Please refer to the appeal due date for approximate date of completion.",
                            "Appeal Line Details Message for status that is not completed or closed");

                    var previousAppeal = _appealSummary.GetPreviousAppealList();
                    previousAppeal.IsInDescendingOrder().ShouldBeTrue("Appeals Seq should be order of creation");
                    _appealSummary.ClickOnAppealSeqAndSwitch(previousAppeal[0]);
                    _appealSummary.GetAppealSequenceOnHeader()
                        .ShouldBeEqual(previousAppeal[0],
                            string.Format("Appeal Summary of <{0}> should dispaly", previousAppeal[1]));

                    _appealSummary.CloseAnyPopupIfExist();

                    _appealSummary.ClickOnAppealLineRow(2);
                    _appealSummary.GetNoPreviousAppealsMessage()
                        .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");


                    for (var i = 0; i <= 1; i++)
                    {
                        StringFormatter.PrintMessageTitle(string.Format(
                            "Verification of appeal line details when status is {0}", i == 0 ? "Closed" : "Complete"));
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                        automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(i + 2);
                        VerifyThatNameIsInCorrectWithUserNameFormat(
                            _appealSummary.GetAppealLineDetailsValueByRowCol(1, 1),
                            "Completed By");
                        _appealSummary.GetAppealLineDetailsValueByRowCol(1, 2)
                            .Length.ShouldBeGreater(1, "Result Should display");

                        VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealLineDetailsValueByRowCol(1, 3),
                            " Completed ");
                        previousAppeal = _appealSummary.GetPreviousAppealList();
                        previousAppeal.IsInDescendingOrder().ShouldBeTrue("Appeals Seq should be order of creation");
                        _appealSummary.ClickOnAppealSeqAndSwitch(previousAppeal[0]);
                        _appealSummary.GetAppealSequenceOnHeader()
                            .ShouldBeEqual(previousAppeal[0],
                                string.Format("Appeal Summary of <{0}> should dispaly", previousAppeal[0]));
                        _appealSummary.CloseAnyPopupIfExist();
                        _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes")
                            .ShouldBeEqual(notesVisibleToClient, "Notes displayed for given appeal.");

                    }

                    StringFormatter.PrintMessageTitle(
                        "Verification of No previous appeal exist for complete status");
                    _appealSummary.ClickOnAppealSeqAndSwitch(previousAppeal[1]);
                    _appealSummary.GetNoPreviousAppealsMessage()
                        .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");
                    _appealSummary.CloseAnyPopupIfExist();


                    for (var i = 0; i <= 1; i++)
                    {
                        StringFormatter.PrintMessageTitle(string.Format(
                            "Verification of appeal line details when status is {0}", i == 0 ? "Closed" : "Complete"));
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                        automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(i + 4);
                        _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes")
                            .ShouldBeNullorEmpty("Notes displayed for given appeal.");

                    }

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    StringFormatter.PrintMessageTitle(
                        "Verification of Selecting different lines will change the viw to correspond with information for the selected line");
                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.SearchByClaimSequence(claimSequenceHavingMultipleLine);
                    var resultLine1 = _appealSummary.GetAppealLineDetailsAuditValueByLabel("Result");
                    var notesLine1 = _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes");
                    _appealSummary.ClickOnAppealLineRow(2);
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Result")
                        .ShouldNotBeEqual(resultLine1, "Result");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes")
                        .ShouldNotBeEqual(notesLine1, "Notes");
                    StringFormatter.PrintMessageTitle(
                        "Verification of No previous appeal exist for closed status");
                    _appealSummary.GetNoPreviousAppealsMessage()
                        .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");
                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealSearch();
                    }

                    _appealSearch.ClearAll();
                }

                #region Private Method
                void VerifyThatDateIsInCorrectFormat(string date, string dateName)
                {
                    Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The " + dateName + " Date'" + date + "' is in format MM/DD/YYYY");
                }
                #endregion
            }
        }


        //[Test]//us41393
        //public void Verify_behaviour_of_appeal_notes()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");
        //    AppealNotePage appealNote = null;
        //    AppealProcessingHistoryPage appealProcessingHx = null;
        //    int countWindowHandles = 0;
        //    _appealSearch.SelectAllAppeals();
        //    automatedBase.CurrentPage= _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);

        //    //_appealSummary.IsAppealNoteEnabled().ShouldBeTrue("Appeal Note Should be Enabled and present");

        //    _appealSummary.ClickMoreOption();
        //    try
        //    {
        //        _appealSummary.IsAppealProcessingHxLinkPresent().ShouldBeTrue("Appeal Processing History Link Present");
        //        appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
        //        appealProcessingHx.GetPageHeader().ShouldBeEqual("Appeal Processing History", "Appeal Processing Page Should Open");
        //        countWindowHandles += appealProcessingHx.CountWindowHandles(appealProcessingHx.PageTitle);
        //        appealProcessingHx.SwitchBackToAppealSummaryPage();

        //        appealNote = _appealSummary.ClickAppealNote();
        //        appealNote.PageTitle.ShouldBeEqual(appealNote.automatedBase.CurrentPageTitle, "Appeal Note Popup");
        //        appealNote.GetnoteTypeValueForNewNote().ShouldBeEqual("Appeal", "Note Type Value");
        //        countWindowHandles += appealNote.CountWindowHandles(appealNote.PageTitle);
        //        appealNote.SwitchBackToAppealSummaryPage();
        //        countWindowHandles.ShouldBeGreater(1, string.Format("{0} windows opened when clicked on Notes and Appeal Processing History Page", countWindowHandles));


        //        appealNote.CloseNotePopUpPageAndBackToAppealSummary();
        //        appealProcessingHx.CloseAppealProcessingHistoryAndBackToAppealSummary();
        //    }
        //    finally
        //    {
        //        _appealSummary.CloseAnyPopupIfExist();
        //        if (appealProcessingHx != null && _appealSummary.IsAppealProcessingHistory())
        //        {
        //            appealProcessingHx.CloseAppealProcessingHistoryAndBackToAppealSummary();
        //            automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
        //        }

        //        if (appealNote != null && _appealSummary.IsAppealNotePagePresent())
        //        {
        //            appealNote.CloseNotePopUpPageAndBackToAppealSummary();
        //            automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
        //        }
        //        if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
        //        {
        //            automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
        //        }
        //        else if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
        //        {
        //            automatedBase.CurrentPage.NavigateToAppealSearch();
        //        }
        //        _appealSearch.ClearAll();
        //    }

        //}


        [Test, Category("Upload_Document")] //us41378 part 1
        public void Verify_appeal_document_is_listed_with_info_in_appeal_summary()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var fileName = paramLists["FileName"];
                var fileType = paramLists["FileType"];
                var date = paramLists["Date"];
                var docDescription = paramLists["DocDescription"];
                _appealSearch.SelectAllAppeals();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);
                try
                {
                    _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary",
                        "Appeal Summary page opened from appeal action.");
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeTrue("Document List are present");


                    _appealSummary.GetAddedAppealDocumentList()
                        .IsInAscendingOrder()
                        .ShouldBeTrue("Most recenet uploaded files are on top");
                    _appealSummary.AppealDocumentsListAttributeValue(1, 1, 2)
                        .ShouldBeEqual(fileName, "Document filename is displayed");
                    _appealSummary.AppealDocumentsListAttributeValue(1, 1, 3)
                        .ShouldBeEqual(fileType, "Document fileType is displayed");
                    _appealSummary.GetAppealDocumentAttributeToolTip(1, 1, 3)
                        .ShouldBeEqual(fileType, "Document fileType tooltip is displayed");
                    _appealSummary.AppealDocumentsListAttributeValue(1, 1, 4)
                        .ShouldBeEqual(date, "Document date is displayed");
                    _appealSummary.AppealDocumentsListAttributeValue(1, 2, 2)
                        .ShouldBeEqual(docDescription, "Document Description is displayed");
                    _appealSummary.ClickOnDocumentToViewAndStayOnPage(1); //window opens to view appeal document 
                    _appealSummary.CloseDocumentTabPageAndBackToAppealSummary();

                }
                finally
                {
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                    else if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealSearch();
                    }

                    _appealSearch.ClearAll();

                }
            }
        }

        [Test, Category("Upload_Document")] //us41378 part 2
        public void Verify_appeal_document_upload_behaviour()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                List<string> expectedFileTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "File_Type_List").Values.ToList();
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var description = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "Description", "Value");
                List<string> expectedSelectedFileTypeList = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "SelectedFileList", "Value")
                    .Split(',').ToList();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary = _appealSearch.ClickOnAppealSequence(1);

                try
                {
                    _appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Disabled for Closed");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(3);
                    _appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Disabled for Complete");
                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(2);
                    _appealSummary.IsAppealDocumentUploadEnabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Enabled for non complete or unclosed");
                    _appealSummary.ClickOnAppealDocumentUploadIcon();
                    _appealSummary.IsAppealDocumentUploadSectionPresent()
                        .ShouldBeTrue("Appeal Document Uploader Section is displayed");
                    _appealSummary.GetAppealSummaryUploaderFieldLabel(3).ShouldBeEqual("Description");
                    _appealSummary.SetAppealSummaryUploaderFieldValue(description, 3);
                    _appealSummary.GetAppealSummaryUploaderFieldValue(3).Length.ShouldBeEqual(100,
                        "Character length should not exceed more than 100 in Description");
                    _appealSummary.GetAvailableFileTypeList()
                        .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                    _appealSummary.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                    _appealSummary.GetPlaceHolderValue().ShouldBeEqual("Provider Letter", "File Type Text");
                    _appealSummary.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                    _appealSummary.GetPlaceHolderValue().ShouldBeEqual("Multiple values selected",
                        "File Type Text when multiple value selected");
                    _appealSummary.GetSelectedFileTypeList()
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Selected File List Equal");
                    _appealSummary.ClickOnCancelBtn();
                }
                finally
                {
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }


        [Test, Category("Upload_Document")] //us44380 and US41392 client
        public void Verify_appeal_document_upload_in_local_and_remote()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManager;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var fileType = paramLists["FileType"];
                var fileName = paramLists["FileName"];
                var maxnote = paramLists["MaxNote"];
                AppealCreatorPage appealCreator = _appealSearch.NavigateToAppealCreator(); 


                try
                {
                    appealCreator.SearchByClaimSequence(claimSeq);
                    appealCreator.SetNote(maxnote);
                    appealCreator.ClickOnClaimLine(1);
                    appealCreator.SelectAppealRecordType();
                    appealCreator.ClickOnSaveBtn();
                    appealCreator.WaitForWorking();
                    automatedBase.CurrentPage = appealCreator.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    if (_appealSearch.IsPageErrorPopupModalPresent())
                    {
                        _appealSearch.ClosePageError();
                    }

                    _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);
                    if (_appealSearch.IsPageErrorPopupModalPresent())
                    {
                        _appealSearch.ClosePageError();
                    }

                    _appealSummary.GetPageHeader()
                        .ShouldBeEqual("Appeal Summary", "Appeal Summary page opened from appeal search.");

                    var dueDate = _appealSummary.GetAppealDetails(1, 3);
                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual("Open", "Status of the newly created appeal is  Open status?");

                    _appealSummary.IsAppealDocumentUploadEnabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Enabled for non complete or unclosed");
                    _appealSummary.ClickOnAppealDocumentUploadIcon();
                    _appealSummary.ClickOnSaveUploadBtn();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Error popup for no file selection is present?");

                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("At least one file must be uploaded before the changes can be saved.");
                    _appealSearch.ClosePageError();
                    _appealSummary.IsAddFileButtonDisabled()
                        .ShouldBeTrue("Add File Button is disabled when no file selected?");
                    _appealSummary.PassFilePathForDocumentUpload();
                    _appealSummary.IsAddFileButtonDisabled()
                        .ShouldBeFalse("Add File Button disabled should be false? ");
                    _appealSummary.ClickOnAddFileBtn();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Error pop up for no file type selection is present?");

                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "At least one Document Type selection is required before the files can be added.");
                    _appealSearch.ClosePageError();
                    _appealSummary.SetAppealSummaryUploaderFieldValue("test appeal doc", 3);
                    _appealSummary.SetFileTypeListVlaue("Provider Letter");
                    _appealSummary.ClickOnAddFileBtn();
                    _appealSummary.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
                    _appealSummary.FileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileName, "Document correct and present");
                    _appealSummary.ClickOnSaveUploadBtn();
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeTrue("Document List are present");
                    var updatedDueDate = _appealSummary.GetAppealDetails(1, 3);
                    updatedDueDate.ShouldNotBeEqual(dueDate,
                        "Due date " + updatedDueDate +
                        " should be based on document upload date and updated due date equal to old " + dueDate +
                        " should be false.");
                    _appealSummary.GetAppealDetails(1, 4).ShouldBeEqual("New", "Status should be udpated to new");

                    //test for  status update for appeal with New status
                    _appealSummary.ClickOnAppealDocumentUploadIcon();
                    _appealSummary.PassFilePathForDocumentUpload();
                    _appealSummary.SetFileTypeListVlaue(fileType);
                    _appealSummary.ClickOnAddFileBtn();
                    _appealSummary.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
                    _appealSummary.FileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileName, "Document correct and present");
                    _appealSummary.ClickOnSaveUploadBtn();
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeTrue("Document List are present");

                    _appealSummary.AppealDocumentCountOfFileList().ShouldBeEqual(2, "All the two documents are listed");

                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual("New",
                            "Status should not be changed and should be new for file upload with status other than new");


                    _appealSummary.DeleteIconCountOfFileList()
                        .ShouldBeEqual(2, "Delete icon present in all listed files");
                    _appealSummary.ClickOnDeleteFileBtn(1);

                    if (_appealSummary.IsPageErrorPopupModalPresent())
                        _appealSummary.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    _appealSummary.ClickOkCancelOnConfirmationModal(false);

                    _appealSummary.AppealDocumentCountOfFileList()
                        .ShouldBeEqual(2, "All the two documents are still entact and listed");
                    _appealSummary.ClickOnDeleteFileBtn(1);
                    _appealSummary.ClickOkCancelOnConfirmationModal(true);

                    _appealSummary.GetAppealDetails(1, 4)
                        .ShouldBeEqual("New", "Status should not be changed and should be new after deleting file");


                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeTrue("Document List are present");
                    _appealSummary.ClickOnDeleteFileBtn(1);
                    _appealSummary.ClickOkCancelOnConfirmationModal(true);
                    _appealSummary.WaitForWorkingAjaxMessage();
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeFalse("Document List present should be false.");


                    _appealSummary.ClickMoreOption();

                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("DeleteDoc", "Action Should be Delete"); //Status
                    _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                }
                finally //to delete appeal created
                {
                    automatedBase.CurrentPage.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
                    {
                        if (_appealSearch.IsPageErrorPopupModalPresent())
                        {
                            _appealSearch.ClosePageError();
                        }
                    }
                    DeleteLastCreatedAppeal(claimSeq);
                }

                void DeleteLastCreatedAppeal(string claSeq)
                {
                    automatedBase.CurrentPage =
                        _appealManager = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    _appealManager.DeleteLastCreatedAppeal(claSeq);
                    automatedBase.CurrentPage =
                        _appealSearch = _appealManager.Logout().LoginAsClientUser().NavigateToAppealSearch();
                }
            }
        }

        [Test, Category("Upload_Document")]//us41392 story broken to US61344
        public void Verify_appeal_document_delete_upload_disabled_for_complete_closed_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Closed");
                try
                {
                    _appealSummary.IsAppealDocumentUploadEnabled()
                        .ShouldBeFalse("Appeal Document Upload Should be Disabled for  complete or unclosed");

                    _appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("Appeal Document Delete Should be Disabled for  complete or unclosed");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                    _appealSummary.IsAppealDocumentUploadEnabled()
                        .ShouldBeFalse("Appeal Document Upload Should be Disabled for  complete or unclosed");

                    _appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("Appeal Document Delete Should be Disabled for  complete or unclosed");
                }
                finally
                {

                    _appealSummary.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//US50609
        public void Verify_blank_or_lenghty_client_notes_displayed_and_editable()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var claimSeqForLongNote = testData["ClaimSequenceForLongNote"];
                var clientNotes = new string('a', 140);

                _appealSearch.SelectAllAppeals();
                _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);
                try
                {
                    _appealSummary.GetAppealDetailsToolTip(3, 1)
                        .ShouldBeNullorEmpty("Client Notes field tooltip should blank if no notes are entered");
                    _appealSummary.ClickOnEditIcon();

                    StringFormatter.PrintMessageTitle("Verification of field value default to current value");
                    _appealSummary.GetNote().ShouldBeNullorEmpty("ClientNotes Should empty");
                    StringFormatter.PrintMessageTitle("Verifying cleint note as not a required field.");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.WaitForWorkingAjaxMessage();
                    _appealSummary.GetAppealDetails(3, 1).ShouldBeNullorEmpty("ClientNotes");

                    automatedBase.CurrentPage =
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    automatedBase.CurrentPage =
                        _appealSummary = _appealSearch.SearchByClaimSequence(claimSeqForLongNote);
                    _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Verification of Page");
                    if (!_appealSummary.GetAppealDetailsToolTip(3, 1).Equals(clientNotes))
                    {
                        _appealSummary.ClickOnEditIcon();
                        StringFormatter.PrintMessageTitle(
                            "validating data as needed. in case of change due to test failure previously");
                        _appealSummary.SetNote(clientNotes);
                        _appealSummary.ClickOnSaveButtonOnEditAppeal();
                        _appealSummary.WaitForWorkingAjaxMessage();
                    }

                    _appealSummary.GetClientNotesEllipsis()
                        .ShouldBeEqual("ellipsis", "Client Note for lengthy values truncated.");
                    _appealSummary.GetAppealDetailsToolTip(3, 1).ShouldBeEqual(clientNotes,
                        "Verification of client notes tooltip for lengthy note");

                    var appealNo = _appealSummary.GetAppealDetails(2, 1);
                    var externalDocId = "";
                    if (_appealSummary.IsExternalDocIdPresentInAppealDetails())
                        externalDocId = _appealSummary.GetAppealDetails(2, 4);

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.WaitForStaticTime(1500);

                    StringFormatter.PrintMessageTitle("Verification of editing and updates without refresh.");
                    _appealSummary.SetNote(clientNotes + "aaa");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.WaitForWorkingAjaxMessage();
                    _appealSummary.GetAppealDetailsToolTip(3, 1).ShouldBeEqual(clientNotes + "aaa", "ClientNotes");
                    _appealSummary.GetAppealDetails(2, 1).ShouldBeEqual(appealNo, "Appeal no should not changed.");
                    if (_appealSummary.IsExternalDocIdPresentInAppealDetails())
                        _appealSummary.GetAppealDetails(2, 4)
                            .ShouldBeEqual(externalDocId, "External Doc Id should not changed.");
                    _appealSummary.ClickMoreOption();
                    AppealProcessingHistoryPage appealProcessingHistory = _appealSummary.ClickAppealProcessingHx();
                    if (appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 3) == "View")
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(2, 3)
                            .ShouldBeEqual("Edit", "Action Should be Edit");
                    else
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 3)
                            .ShouldBeEqual("Edit", "Action Should be Edit");
                    automatedBase.CurrentPage = _appealSummary =
                        appealProcessingHistory.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.SetNote(clientNotes);
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.WaitForWorkingAjaxMessage();
                }

                finally
                {
                    _appealSummary.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClearAll();
                }
            }
        }

        [Test]//CAR-354
        public void Validate_appeal_status_updates_when_docs_uploaded_to_docs_waiting_appeal_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManager;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var fileType = testData["FileType"];
                var fileName = testData["FileName"];

                StringFormatter.PrintMessage(
                    "Reset appeal to default i.e default status is Docs awaiting with no documents");
                _appealSearch.ResetAppealToDocumentWaiting(claimSeq);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectSMTST();
                _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);

                try
                {
                   _appealSummary.GetStatusValue().ShouldBeEqual(
                        AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(),
                        "Current status of appeal should be document awaiting");


                    var dueDate = _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date");

                    StringFormatter.PrintMessage("Upload document to appeal");
                    _appealSummary.UploadDocumentFromAppealSummary(fileType, fileName,
                        "new file test doc", 1);
                    _appealSummary.WaitForWorkingAjaxMessage();

                    StringFormatter.PrintMessage("Validate appeal status is updated to New");
                    _appealSummary.GetStatusValue().ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(),
                        "Current status of appeal should be updated to new after document is uploaded.");


                    _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date")
                        .ShouldNotBeEqual(dueDate, "Due Date Should Updated");

                    StringFormatter.PrintMessage("Validate Appeal Audit record");
                    _appealSummary.ClickMoreOption();
                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 6)
                        .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(), "Status audit");
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 9).ShouldNotBeEmpty("Assigned To audit");
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 10)
                        .ShouldNotBeEmpty("Primary Reviewer audit");
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 12)
                        .ShouldNotBeEmpty("Due date must be assigned");

                    _appealSummary.CloseAnyPopupIfExist();
                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealManager =
                        _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claimSeq, ClientEnum.SMTST.ToString());
                    _appealManager.ClickOnSearchListRow(1);
                    _appealManager.GetSearchResultByColRow(11)
                        .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(),
                            "Current status of appeal should be updated to new after document is uploaded.");
                    _appealManager.GetSearchResultByColRow(3).ShouldNotBeEqual(dueDate, "Due Date should be updated");
                    _appealManager.GetSearchResultByColRow(7).ShouldNotBeEmpty("Category Should be assigned");
                    _appealManager.GetSearchResultByColRow(8).ShouldNotBeEmpty("Primary Reviewer should be assigned");
                    _appealManager.GetAppealDetailsContentValue(2, 1)
                        .ShouldNotBeEmpty("Assigned To Should be assigned");
                    automatedBase.CurrentPage = _appealSearch =
                        _appealSearch.Logout().LoginAsClientUser().NavigateToAppealSearch();
                }
                finally
                {
                    _appealSummary.CloseAnyPopupIfExist();
                }
            }
        }

        #endregion

        #region Private Methods

        private void VerifyThatNameIsInCorrectFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
        }

        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
        }


        private void VerifyThatDateIsInCorrectFormat(string date, string dateName)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The " + dateName + " Date'" + date + "' is in format MM/DD/YYYY");
        }


        //private void DeleteLastCreatedAppeal(string claimSeq)
        //{
        //    automatedBase.CurrentPage =
        //        _appealManager = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
        //    _appealManager.DeleteLastCreatedAppeal(claimSeq);
        //    automatedBase.CurrentPage =
        //        _appealSearch = _appealManager.Logout().LoginAsClientUser().NavigateToAppealSearch();
        //}

        //private void CreateAppeal()
        //{
        //    _appealCreator.ClickOnClaimLine(1);
        //    _appealCreator.SelectProduct(ProductEnum.CV.GetStringValue());
        //    _appealCreator.SelectAppealRecordType();
        //    _appealCreator.ClickOnSaveBtn();
        //    _appealCreator.WaitForWorking();
        //}
        #endregion
    }
}

