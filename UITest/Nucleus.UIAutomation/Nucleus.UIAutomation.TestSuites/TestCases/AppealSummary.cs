using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealSummary 
    {
        #region PRIVATE FIELDS

/*

        private AppealSearchPage _appealSearch;
        private AppealSummaryPage _appealSummary;
        private NewPopupCodePage _newPopupCode;
        private AppealProcessingHistoryPage _appealProcessingHx;
        private AppealManagerPage _appealManager;
        private ClientSearchPage _clientSearch;*/
        #endregion

        #region OVERRIDE METHODS
        /*protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();
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
            CurrentPage = _appealSearch;
        }

        protected override void TestCleanUp()
        {
            _appealSearch.CloseAnyTabIfExist();

            if (!CurrentPage.IsDefaultTestClientForEmberPage(EnvironmentManager.TestClient))
            {
                //CurrentPage.ClickOnQuickLaunch();
                CurrentPage.ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);
            }

            
            if (CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealSummary.GetStringValue()))
            {
                CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
            }
            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealSearch.GetStringValue()))
            {
                CurrentPage = _appealSearch = CurrentPage.NavigateToAppealSearch();
            }

            _appealSearch.WaitForPageToLoadWithSideBarPanel();

            if (!_appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                _appealSearch.ClickOnSearchButton();

            _appealSearch.ClearAll();
            _appealSearch.SelectAllAppeals();
            

            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _appealSearch = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
            }
            base.TestCleanUp();

        }*/

        #endregion

        //#region PROTECTED PROPERTIES

        //protected string GetType().FullName
        //{
        //    get
        //    {
        //        return GetType().FullName;
        //    }
        //}
        //#endregion
        

        #region TEST SUITES


        [Test] //US67260
        [Category("EdgeNonCompatible")]
        public void Verify_Appeal_letter_download_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                try
                {
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

                    //_appealSummary = appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                    //_appealSummary.CheckIfPdfDownloadPageIsOpenAndCloseAndReturnToFirstWindow()
                    //    .ShouldBeTrue("Is PDF download Page open in new tab? Also verify content-type of page is pdf type");
                    _appealSummary.CloseAnyPopupIfExist();
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                }
            }
        }

        [Test]//US50610
        public void Verify_edit_icon_enabled_only_to_user_with_having_Appeal_Manager_authority_otherwise_disabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;

                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);
                
                _appealSummary.IsEditIconEnabled()
                        .ShouldBeTrue("Edit Icon Enabled for having Appeal Manager Authority");
                automatedBase.CurrentPage = _appealSearch = _appealSummary.Logout()
                    .LoginAsUserHavingNoManageEditAuthority()
                    .NavigateToAppealSearch();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();

                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);
                _appealSummary.IsEditIconDisabled()
                    .ShouldBeTrue("Edit Icon Disabled for not having Appeal Manager Authority");
                automatedBase.CurrentPage = _appealSearch = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
            }
        }

        [Test]//US50610
        public void Verify_proper_validation_for_edit_appeal_information_with_changes_made_should_display_in_Q1_without_refreshing_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealProcessingHistoryPage _appealProcessingHx;
                
                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AppealSequence", "Value");
                var appealTypeList = automatedBase.DataHelper.GetMappingData(GetType().FullName, "Appeal_type").Values.ToList();
                var appealPriorityList = automatedBase.DataHelper.GetMappingData(GetType().FullName, "Appeal_priority").Values
                    .ToList();
                var statusList = automatedBase.DataHelper.GetMappingData(GetType().FullName, "Appeal_status").Values.ToList();
                var productList = automatedBase.DataHelper.GetMappingData(GetType().FullName, "Product_Type_List").Values
                    .ToList();
                _appealSearch.SelectAllAppeals();
                const string actualAppealType = "Appeal";
                const string actualPriority = "Urgent";
                const string actualStatus = "Pending Other";
                const string actualProduct = "Coding Validation";
                const string actualPrimaryReviewer = "abc9214 def9214";
                const string actualAssignedTo = "abc9214 def9214";

                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);
                
                try
                {
                    var appealType = _appealSummary.GetAppealDetails(1, 1);

                    if (appealType != "Appeal")
                    {
                        _appealSummary.ClickOnEditIcon();
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Type", "Appeal");
                        _appealSummary.ClickOnSaveButtonOnEditAppeal();

                    }

                    appealType = _appealSummary.GetAppealDetails(1, 1);
                    var priority = _appealSummary.GetAppealDetails(1, 2);
                    var dueDate = _appealSummary.GetAppealDetails(1, 3);
                    var status = _appealSummary.GetAppealDetails(1, 5);
                    var product = _appealSummary.GetAppealDetails(2, 3);
                    var primaryReviewer = _appealSummary.GetAppealDetails(2, 4);
                    var assignedTo = _appealSummary.GetAppealDetails(2, 5);

                    _appealSummary.ClickOnEditIcon();

                    StringFormatter.PrintMessageTitle("Verification of value at textfield at Edit appeal information");
                    _appealSummary.GetEditAppealInputValueByLabel("Appeal Type")
                        .ShouldBeEqual(appealType, "Appel Type  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Due Date")
                        .ShouldBeEqual(dueDate, "Due Date  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Primary Reviewer")
                        .ShouldBeEqual(primaryReviewer, "Primary Reviwer  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Assigned To")
                        .ShouldBeEqual(assignedTo, "Assigned To  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Appeal Priority")
                        .ShouldBeEqual(priority, "Appeal Priority  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Status")
                        .ShouldBeEqual(status, "Appeal Status  shows correct value");
                    _appealSummary.GetEditAppealInputValueByLabel("Product")
                        .ShouldBeEqual(product, "Product  shows correct value");

                    Console.WriteLine("Verification of Drop Down List Value");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Type", "Record Review");
                    _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Appeal Priority")
                        .ShouldBeTrue("Appeal Priority Should Disabled when Appeal Type is Record Review");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Type", "Appeal");

                    _appealSummary.GetEditAppealInputListByLabel("Appeal Type")
                        .ShouldCollectionBeEqual(appealTypeList, "Appeal Type List");
                    _appealSummary.GetEditAppealInputListByLabel("Appeal Priority")
                        .ShouldCollectionBeEquivalent(appealPriorityList, "Appeal Priority List");
                    _appealSummary.GetEditAppealInputListByLabel("Status")
                        .ShouldCollectionBeEqual(statusList, "Appeal Status List");
                    _appealSummary.GetEditAppealInputListByLabel("Product")
                        .ShouldCollectionBeEqual(productList, "Product Type List");
                    _appealSummary.GetEditAppealInputListByLabel("Primary Reviewer").Count
                        .ShouldBeGreater(1, "Primary Reviewer List Should Greater than one");
                    _appealSummary.GetEditAppealInputListByLabel("Assigned To").Count
                        .ShouldBeGreater(1, "Assigned To List Should Greater than one");

                    Console.WriteLine("Verification of Input field when set to empty");
                    _appealSummary.SetEditAppealInputValueByLabel("Due Date", "");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Validate Should popup when trying to save without due date");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Appeal Due Date is required.", "Validation Message when due date is empty");
                    _appealSummary.ClosePageError();
                    _appealSummary.IsInvalidInputPresentByLabel("Due Date").ShouldBeTrue(
                        "'Due Date' field should be highlighted red when " +
                        "empty value is entered");
                    _appealSummary.SetDueDate(Convert.ToDateTime(dueDate).ToString("MM/d/yyyy"));
                    _appealSummary.GetEditAppealInputValueByLabel("Due Date")
                        .ShouldBeEqual(dueDate, "Due Date shows correct value after selecting from date picker");

                    _appealSummary.ClearEditAppealInfoDropDownField("Primary Reviewer");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Validate Should popup when trying to save without due date");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Appeal Primary Reviewer is required",
                            "Validation Message when due date is empty");
                    _appealSummary.ClosePageError();
                    _appealSummary.IsInvalidInputPresentByLabel("Primary Reviewer").ShouldBeTrue(
                        "'Primary Reviewer' field should be highlighted red when " +
                        "empty value is entered");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", primaryReviewer);

                    _appealSummary.ClearEditAppealInfoDropDownField("Assigned To");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Validate Should popup when trying to save without due date");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Appeal Assigned To is required", "Validation Message when due date is empty");
                    _appealSummary.ClosePageError();
                    _appealSummary.IsInvalidInputPresentByLabel("Assigned To").ShouldBeTrue(
                        "'Assigned To' field should be highlighted red when " +
                        "empty value is entered");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Assigned To", assignedTo);

                    _appealSummary.ClearEditAppealInfoDropDownField("Product");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();

                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Validate Should popup when trying to save without due date");

                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Appeal Product is required", "Validation Message when due date is empty");
                    _appealSummary.ClosePageError();
                    _appealSummary.IsInvalidInputPresentByLabel("Product").ShouldBeTrue(
                        "'Product' field should be highlighted red when " +
                        "empty value is entered");

                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Product", product);

                    _appealSummary.SetEditAppealInputValueByLabel("Due Date", "");
                    _appealSummary.ClearEditAppealInfoDropDownField("Assigned To");
                    _appealSummary.ClearEditAppealInfoDropDownField("Primary Reviewer");
                    _appealSummary.ClearEditAppealInfoDropDownField("Product");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Validate Should popup when trying to save without data");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Appeal Due Date is required.\r\nAppeal Primary Reviewer is required\r\nAppeal Assigned To is required\r\nAppeal Product is required",
                            "Validation Message when due date is empty");
                    _appealSummary.ClosePageError();

                    _appealSummary.ClickOnCancelLinkOnEditAppeal();

                    Console.WriteLine("Verification of Appeal Information when click on cancel link");
                    _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual(appealType, "Appeal Type");
                    _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual(priority, "Priority");
                    _appealSummary.GetAppealDetails(1, 3).ShouldBeEqual(dueDate, " Due Date");
                    _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual(status, "Status");
                    _appealSummary.GetAppealDetails(2, 3).ShouldBeEqual(product, "Product");
                    _appealSummary.GetAppealDetails(2, 4).ShouldBeEqual(primaryReviewer, "Primary reviewer");
                    _appealSummary.GetAppealDetails(2, 5).ShouldBeEqual(assignedTo, "Assigned To");

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.SetDueDate(Convert.ToDateTime(dueDate).AddDays(-1).ToString("MM/d/yyyy"));
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Assigned To", "ui automation_4");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", "ui automation_4");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Product", "Dental Claim Accuracy");

                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Type", "Record Review");
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status", "Documents Waiting");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();

                    Console.WriteLine("Verification of Appeal Information when changes made without refreshing page");
                    _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual("Record Review", "Appeal Type");
                    _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual("Normal", "Priority");
                    _appealSummary.GetAppealDetails(1, 3)
                        .ShouldBeEqual(Convert.ToDateTime(dueDate).AddDays(-1).ToString("MM/dd/yyyy"), " Due Date");
                    _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual("Documents Waiting", "Status");
                    _appealSummary.GetAppealDetails(2, 3).ShouldBeEqual("Dental Claim Accuracy", "Product");
                    _appealSummary.GetAppealDetails(2, 4).ShouldBeEqual("ui automation_4", "Primary reviewer");
                    _appealSummary.GetAppealDetails(2, 5).ShouldBeEqual("ui automation_4", "Assigned To");


                    _appealSummary.ClickMoreOption();
                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("Edit", "Action Should be Edit");
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 6)
                        .ShouldBeEqual("Documents Waiting", "Status :"); //Status
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 9)
                        .ShouldBeEqual("ui automation_4 (uiautomation_4)", "Assigned To :"); //Status
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 10)
                        .ShouldBeEqual("ui automation_4 (uiautomation_4)", "Primary Reviewer"); //PR 
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 13)
                        .ShouldBeEqual("Normal", "Priority :"); //Priority
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 14)
                        .ShouldBeEqual("D", "Product :"); //Product

                    _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                }
                finally
                {
                    if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)))
                    {
                        if (!_appealSummary.IsEditAppealFormDisplayed())
                            _appealSummary.ClickOnEditIcon();

                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Assigned To", actualAssignedTo);
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer",
                            actualPrimaryReviewer);
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Product", actualProduct);
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Type", actualAppealType);
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status", actualStatus);
                        _appealSummary.SelectEditAppealFieldDropDownListByLabel("Appeal Priority", actualPriority);
                        _appealSummary.SetDueDate(DateTime.Now.ToString("MM/d/yyyy"));
                        _appealSummary.ClickOnSaveButtonOnEditAppeal();
                        automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                    else if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealSearch();
                    }
                }
            }
        }

        [Test]//US50610 + US61344(break down of US41392 )
        public void Verify_edit_appeal_information_field_should_disabled_for_status_closed_or_complete()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AppealSequence", "Value");
                _appealSearch.SelectAllAppeals();

                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealSeq);
                var status = _appealSummary.GetAppealDetails(1, 5);
                StringFormatter.PrintMessageTitle(
                    string.Format("Verification of Edit appeal information field should disabled for {0}", status));
                _appealSummary.IsAppealDocumentUploadEnabled()
                    .ShouldBeFalse("Appeal Document Upload Should be Disabled for  complete or unclosed");

                _appealSummary.IsAppealDocumentDeleteDisabled()
                    .ShouldBeTrue("Appeal Document Delete Should be Disabled for  complete or unclosed");
                _appealSummary.ClickOnEditIcon();
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Appeal Type")
                    .ShouldBeTrue("Appeal Type Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Due Date")
                    .ShouldBeTrue("Due Date Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Primary Reviewer")
                    .ShouldBeTrue("Primary Reviewer Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Assigned To")
                    .ShouldBeTrue("Assigned To Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Appeal Priority")
                    .ShouldBeTrue("Appeal Priority Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Product")
                    .ShouldBeTrue("Product Should disabled");

                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status",
                    status == "Complete" ? "Closed" : "Complete");
                _appealSummary.ClickOnSaveButtonOnEditAppeal();
                status = _appealSummary.GetAppealDetails(1, 5);
                StringFormatter.PrintMessageTitle(
                    string.Format("Verification of Edit appeal information field should disabled for {0}", status));
                _appealSummary.IsAppealDocumentUploadEnabled()
                    .ShouldBeFalse("Appeal Document Upload Should be Disabled for  complete or unclosed");
                _appealSummary.IsAppealDocumentDeleteDisabled()
                    .ShouldBeTrue("Appeal Document Delete Should be Disabled for  complete or unclosed");
                _appealSummary.ClickOnEditIcon();
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Appeal Type")
                    .ShouldBeTrue("Appeal Type Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Due Date")
                    .ShouldBeTrue("Due Date Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Primary Reviewer")
                    .ShouldBeTrue("Primary Reviewer Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Assigned To")
                    .ShouldBeTrue("Assigned To Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Appeal Priority")
                    .ShouldBeTrue("Appeal Priority Should disabled");
                _appealSummary.IsEditAppealInfoFieldDisabledByLabel("Product")
                    .ShouldBeTrue("Product Should disabled");
            }
        }

        [Test]//DE6049
        public void Verify_CompletedDate_and_CompletedBy_is_updated_after_changing_appeal_status_to_complete()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var appealseq = paramLists["AppealSequence"];
                var completedBy = paramLists["CompletedBy"];
                _appealSearch.SelectAllAppeals();
                
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByAppealSequence(appealseq);
                _appealSummary.ClickOnEditIcon();
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status",
                    AppealStatusEnum.Complete.GetStringDisplayValue());
                _appealSummary.ClickOnSaveButtonOnEditAppeal();
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed By").ShouldBeEqual(completedBy,
                    "CompletedBy must be updated in Appeal Line details");
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed")
                    .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                        "Completed Date must be updated in Appeal Line details");
                _appealSummary.ClickOnEditIcon();
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status",
                    AppealStatusEnum.Open.GetStringDisplayValue());
                _appealSummary.ClickOnSaveButtonOnEditAppeal();
            }
        }

        [Test]//us41380 + CAR-2967
        public void Verify_that_presence_of_header_icons()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                
                StringFormatter.PrintMessageTitle("Verification of Presence of Appeal Summary Header Icons");
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                _appealSummary.IsClosedAppealIconEnabled().ShouldBeFalse("Close Appeal Icon Present?");
                //_appealSummary.IsAppealNoteEnabled().ShouldBeTrue("Appeal Note Present");
                _appealSummary.IsSearchIconPresent().ShouldBeTrue("Search Icon Present");
                _appealSummary.ClickMoreOption();
                _appealSummary.IsAppealProcessingHxLinkPresent().ShouldBeTrue("Appeal Processing History Present");
                _appealSummary.ClickMoreOption();
            }
        }

        [Test] //US41398 part 2 for Cotiviti user
        [Retrying(Times = 2)]
        public void Verify_appeal_processiong_hx_from_appeal_summary()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                AppealProcessingHistoryPage appealProcessingHx = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);
                
                _appealSummary.IsAppealProcessingHxLinkPresent()
                    .ShouldBeTrue("Appeal Processing History Link Present");
                _appealSummary.ClickMoreOption();
                appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                appealProcessingHx.GetPageHeader().ShouldBeEqual("Appeal Processing History",
                    "Appeal Processing Page Should Open");
                _appealSummary.CloseAnyPopupIfExist();
            }
        }

        [Test]//US41387
        public void Verify_view_appeal_letter_enabled_only_for_closed_or_complete_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                AppealLetterPage appealLetter = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                try
                {
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                    _appealSummary.IsAppealLetterEnabled().ShouldBeTrue("Appeal Letter Should be Enabled for Closed");
                    try
                    {
                        appealLetter = _appealSummary.ClickAppealLetter();
                        appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Appeal Letter Popup",
                            true);
                    }
                    finally
                    {
                        if (appealLetter != null)
                            appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                    }

                    automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(2);
                    _appealSummary.IsAppealLetterDisabled().ShouldBeTrue("Appeal Letter Should be Disabled for Open");

                    automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(3);
                    _appealSummary.IsAppealLetterEnabled().ShouldBeTrue("Appeal Letter Should be Enabled for Complete");

                    appealLetter = null;
                    try
                    {
                        appealLetter = _appealSummary.ClickAppealLetter();
                        appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Appeal Letter Popup",
                            true);
                    }
                    finally
                    {
                        if (appealLetter != null)
                            appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                    }

                    automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    if (appealLetter != null)
                        Console.WriteLine("appeal letter is not null");

                }
            }
        }

        [Test]//US41385
        public void Verify_that_appeal_information_with_proper_display_of_claim_line()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                NewPopupCodePage _newPopupCode;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var provider = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Provider", "Value");
                var specialty = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Specialty", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                
                automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(2);
                _appealSummary.IsAppealLineSectionPresent().ShouldBeTrue("Appeal Lines Section displayed");
                _appealSummary.GetClaimSectionRowValueByLabel("Claim Sequence")
                    .ShouldBeEqual(claimSeq, "Claim Sequence dispalyed and Equal");
                _appealSummary.GetClaimSectionRowValueByLabel("Provider")
                    .ShouldBeEqual(provider, "Provider dispalyed and Equal");
                _appealSummary.GetClaimSectionRowValueByLabel("S:")
                    .ShouldBeEqual(specialty, "Specialty dispalyed and Equal");
                _appealSummary.GetClainNoList().IsInAscendingOrder()
                    .ShouldBeTrue("Clain Line should be in Ascending Order");

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

                ClaimActionPage claimAction = _appealSummary.ClickOnClaimSequenceAndSwitchWindow();
                var uniqueFlagList = claimAction.GetFlagListForClaimLine(2).Distinct().ToList();
                ; //get only unique flag list for first claim line of claimAction
                _appealSummary.CloseAnyPopupIfExist();
                _appealSummary.GetFlagList().ShouldCollectionBeEqual(uniqueFlagList,
                    "Unique flags are present per line and flag order matches that of Claim Action");

                _appealSummary.GetClaimLineSectionRowValueByRowCol(1, 2, 5)
                    .ShouldBeEqual("2", "Appeal Level Should Equal");
                _appealSummary.IsGreyAppealIconPresent(2, 5).ShouldBeTrue("Grey Icon Present");
                _appealSummary.ClickOnClaimLineDiv(3);
                _appealSummary.GetNoPreviousAppealsMessage()
                    .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");
                _appealSummary.ClickOnClaimLineDiv(1);
                _appealSummary.GetPreviousAppealList().Count.ShouldBeGreater(0, "Previous Appeal Should Present ");
                _appealSummary.GetAppealLevelToolTipByRowCol(1, 2, 5)
                    .ShouldBeEqual("2 Appeals", "Title should equal");
            }
        }

        [Test,Category("Upload_Document")] // US41392 Cotiviti
        public void Verify_appeal_document_delete_for_Cotiviti_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealProcessingHistoryPage _appealProcessingHx;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var product = paramLists["Product"];
                var fileType = paramLists["FileType"];
                var fileName = paramLists["FileName"];
                var maxnote = paramLists["MaxNote"];
                AppealCreatorPage appealCreator = null;
                try
                {
                    automatedBase.CurrentPage = appealCreator = _appealSearch.NavigateToAppealCreator();
                    appealCreator.SearchByClaimSequence(claimSeq);
                    appealCreator.SelectProduct(product);
                    appealCreator.SetNote(maxnote);
                    appealCreator.ClickOnClaimLine(2);
                    appealCreator.SelectAppealRecordType();
                    appealCreator.ClickOnSaveBtn();
                    appealCreator.WaitForWorking();
                    automatedBase.CurrentPage = appealCreator.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);

                    _appealSummary.GetPageHeader()
                        .ShouldBeEqual("Appeal Summary", "Appeal Summary page opened from appeal search.");

                    var status = _appealSummary.GetAppealDetails(1, 5);

                    status.ShouldBeEqual("Open", "Status of the newly created appeal is is Open status?");

                    _appealSummary.IsAppealDocumentUploadEnabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Enabled for non complete or unclosed");
                    _appealSummary.ClickOnAppealDocumentUploadIcon();

                    _appealSummary.PassFilePathForDocumentUpload();
                    _appealSummary.SetAppealSummaryUploaderFieldValue("test appeal doc", 3);
                    _appealSummary.SetFileTypeListVlaue(fileType);
                    _appealSummary.ClickOnAddFileBtn();
                    _appealSummary.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
                    _appealSummary.FileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileName, "Document correct and present");
                    _appealSummary.ClickOnSaveUploadBtn();
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeTrue("Document List are present");

                    var filesCount = _appealSummary.FileUploadPage.GetAddedDocumentList();
                    _appealSummary.FileUploadPage.ClickOnDeleteFileBtn(1);
                    //_appealSummary.ClickOnDeleteFileBtn(1);

                    if (_appealSummary.IsPageErrorPopupModalPresent())
                        _appealSummary.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    _appealSummary.ClickOkCancelOnConfirmationModal(false);
                    _appealSummary.IsAppealDocumentDivPresent()
                        .ShouldBeTrue("All the two documents are still entact and listed.");
                    _appealSummary.FileUploadPage.GetAddedDocumentList().ShouldBeEqual(filesCount,
                        "All the two documents are still entact and listed. ");
                    _appealSummary.FileUploadPage.ClickOnDeleteFileBtn(1);
                    //_appealSummary.ClickOnDeleteFileBtn(1);
                    _appealSummary.ClickOkCancelOnConfirmationModal(true);
                    _appealSummary.WaitForWorkingAjaxMessage();
                    _appealSummary.IsAppealDocumentDivPresent().ShouldBeFalse("Document List present should be false.");
                    _appealSummary.ClickMoreOption();

                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("DeleteDoc", "Action Should be Delete"); //Status
                    _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                    //CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally //to delete appeal created
                {
                    DeleteLastCreatedAppeal(claimSeq, _appealSearch);
                }
            }
        }

       
        [Test]//US48458 (part of story covered in us41385)
        public void Verify_appeal_lines_details_of_modifier_unit_sugpaid()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var speciality = paramLists["Speciality"];
                var modifier = paramLists["Modifier"];
                var units = paramLists["Units"];
                var sugPaid = paramLists["SugPaid"];
                var source = paramLists["Source"];
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                
                automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                _appealSummary.IsAppealLineSectionPresent().ShouldBeTrue("Appeal Lines Section displayed");
                _appealSummary.GetClaimSectionRowValueTooltipByLabel("S:")
                    .ShouldBeEqual(speciality, "Specialty described in tool tip");

                _appealSummary.GetAppealLineSectionRowValueByLabel("M:").ShouldBeEqual(modifier,
                    "Modifier displayed and listed by comma separation");
                _appealSummary.GetAppealLineSectionRowToolTipByLabel("M:")
                    .ShouldBeEqual(modifier, "Modifier tool tip displays entire list.");
                _appealSummary.GetAppealLineSectionRowValueByLabel("U:").ShouldBeEqual(units,
                    "Reported units displayed and equal for the flagged proc code");
                _appealSummary.GetAppealLineSectionRowValueByLabel("Sug Paid").ShouldBeEqual(sugPaid,
                    "Sug paid associated with the claim line displayed and equal");
                _appealSummary.GetAppealLineSectionRowValueByLabel("Source:").ShouldBeEqual(source,
                    "Source associated with top flag displayed and equal ");
                _appealSummary.GetAppealLineSectionRowToolTipByLabel("Source:")
                    .ShouldBeEqual(source, "Tool tip with full description of Source displayed and equal");
            }
        }

        [Test] //US50606+//US57587 story added that create date only dispaly for client user+//US53193 story added that state display for client user only
        public void Verify_that_appeal_information_displayed_for_Cotiviti_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = testData["ClaimSequence"];
                var category = testData["Category"];
                var appealNo = testData["AppealNo"];
                var product = testData["Product"];
                var externalDocId = testData["ExternalDocId"];
                var clientNotes = testData["ClientNotes"];

                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                
                var appeal = _appealSearch.GetAppealSequenceByStatus("Complete");
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                StringFormatter.PrintMessageTitle("Verification of Appeal Summary Page Opened");
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Verification of Page");
                _appealSummary.GetAppealSequenceOnHeader()
                    .ShouldBeEqual(appeal, "Appeal Seq Should should be displayed");

                StringFormatter.PrintMessageTitle("Verification of Appeal Summary Details Information");

                _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual("Appeal", "Appeal Type");
                _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual("Urgent", "Priority");
                VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealDetails(1, 3), " Due Date");
                _appealSummary.GetAppealDetails(1, 4).ShouldBeEqual(category, "Category");
                _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual("Complete", "Status");
                _appealSummary.GetAppealDetails(2, 1).ShouldBeEqual(appealNo, "Appeal Number");
                VerifyThatNameIsInCorrectFormat(_appealSummary.GetAppealDetails(2, 2), "Created By");
                _appealSummary.GetAppealDetails(2, 3).ShouldBeEqual(product, "Product");
                VerifyThatNameIsInCorrectFormat(_appealSummary.GetAppealDetails(2, 4), "Primary reviewer");
                VerifyThatNameIsInCorrectFormat(_appealSummary.GetAppealDetails(2, 5), "Assigned To");
                _appealSummary.GetAppealDetails(2, 6).ShouldBeEqual(externalDocId, "External Doc Id");
                _appealSummary.GetClientNotesEllipsis()
                    .ShouldBeEqual("ellipsis", "Client Note for lengthy values truncated.");
                _appealSummary.GetAppealDetailsToolTip(3, 1).ShouldBeEqual(clientNotes, "ClientNotes");

                _appealSummary.IsAppealDetailFieldPresentByLabel("Created Date")
                    .ShouldBeFalse("Created Date display for Cotiviti user?");
                _appealSummary.IsAppealDetailFieldPresentByLabel("State")
                    .ShouldBeFalse("State display for Cotiviti user?");

                automatedBase.CurrentPage =
                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Documents Waiting");

                _appealSummary.GetAppealDetails(1, 1).ShouldBeEqual("Record Review", "Appeal Type");
                _appealSummary.GetAppealDetails(1, 2).ShouldBeEqual("Normal", "Priority");
                _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual("Documents Waiting", "Status");
                _appealSummary.GetAppealDetails(3, 1).ShouldBeNullorEmpty("ClientNotes");
            }
        }


        [Test]//US50607
        public void Verify_display_of_necessary_info_on_appeal_lines_details_container()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var appealSeq = paramLists["AppealSequence"];
                var reason = paramLists["Reason"];
                var result = paramLists["Result"];
                var notes = paramLists["Notes"];
                var rationale = paramLists["Rationale"];
                var summary = paramLists["Summary"];
                var notesVisibleToClient = paramLists["NotesVisibleToClient"];
                IList<string> prevAppealList = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "PrevAppealList", "Value")
                    .Split(',').ToList();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage(appealSeq);
                _appealSummary.IsAppealLineSectionPresent().ShouldBeTrue("Appeal Lines Section displayed");
                _appealSummary.IsFirstAppealLineActiveElementPresent()
                    .ShouldBeTrue("First Appeal Lines selected and active");
                _appealSummary.IsFirstAppealLineActiveElementPresent()
                    .ShouldBeTrue("First Appeal Lines selected and active");
                VerifyThatNameIsInCorrectFormat(
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Last Modified User"),
                    "Last Modified User for given appeal.");
                VerifyThatNameIsInCorrectFormat(
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed By"),
                    "Completed By displayed for given appeal");
                VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed"),
                    "Completed  displayed for given appeal.");
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Reason Code")
                    .ShouldBeEqual(reason, "Reason displayed for given appeal.");
                VerifyThatReasonCodeIsInCorrectFormat(reason, "reason Code is properly formatted");
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Result")
                    .ShouldBeEqual(result, "Result displayed for given appeal.");
                _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes:")
                    .ShouldBeEqual(notes, "Notes displayed for given appeal.");

                _appealSummary.ClickOnDiplayRationaleLink("Rationale");
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Rationale")
                    .ShouldBeEqual(rationale, "Rationale displayed for given appeal.");
                _appealSummary.GetAppealLineDetailsAuditValueByLabel("Summary")
                    .ShouldBeEqual(summary, "Summary displayed for given appeal.");
                _appealSummary.GetPreviousAppealListSpanValue()
                    .ShouldCollectionBeEqual(prevAppealList, "Previous Appeals collection listed.");
                _appealSummary.GetPreviousAppealListSpanValue()
                    .ShouldCollectionBeSorted(true, "Previous Appeals collection sorted.");

                _appealSummary.ClickOnAppealSeqAndSwitch(prevAppealList[1]);
                _appealSummary.CloseAnyPopupIfExist();
                _appealSummary.ClickOnAppealLineRow(2);
                _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes {Visible to Client}:")
                    .ShouldBeEqual(notesVisibleToClient, "Notes displayed for given appeal.");
                _appealSummary.GetNoPreviousAppealsMessage()
                    .ShouldBeEqual("No previous appeals exist", "No Previous appeals Message");

            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")]//US68890
        public void Verify_completed_appeal_is_set_to_Closed_status_automatically_when_Close_Client_Appeals_client_setting_is_set_true()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                ClientSearchPage _clientSearch;
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequence", "Value");

                try
                {
                    automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickAutoCloseAppealRadioButton(true);

                    automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claimSequence, ClientEnum.SMTST.ToString());
                    _appealManager.ChangeStatusOfAppealByRowSelection(1,
                        "Documents Waiting"); //changed status to document waiting

                    automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSequence);

                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status", "Complete");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    _appealSummary.WaitForWorkingAjaxMessage();

                    _appealSummary.GetStatusValue().ShouldBeEqual(AppealStatusEnum.Closed.GetStringDisplayValue(),
                        "Completed appeal should Closed automatically");
                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.WaitForWorking();
                    _appealSearch.GetSearchResultByColRow(11).ShouldBeEqual(
                        AppealStatusEnum.Closed.GetStringDisplayValue(),
                        "Completed appeal should Closed automatically");
                }
                finally
                {
                    automatedBase.CurrentPage = _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickAutoCloseAppealRadioButton(false);
                }
            }
        }


        [Test] //US69833 
        [Order(1)]
        [Retrying(Times = 3)]
        public void Validate_appeal_lock_is_present_when_appeal_is_viewed_by_other_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                ClientSearchPage _clientSearch;
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                automatedBase.CurrentPage =
                    _appealSearch =
                        _appealSearch.Logout().LoginAsHciAdminUser4().NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                var appealSeq = _appealSummary.GetAppealSequenceOnHeader();
                StringFormatter.PrintMessage(
                    string.Format("Open appeal Summary page for appeal sequence: {0} with direct URL ", appealSeq));
                var newClaimActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                _appealSummary =
                    _appealSearch.SwitchToOpenAppealSummaryByUrlForAdmin1(newClaimActionUrl);

                _appealSummary.IsAppealLocked()
                    .ShouldBeTrue("Appeal should be locked when it is in view mode by another user");
                _appealSummary.GetLockIConTooltip()
                    .ShouldBeEqual(
                        "This appeal is currently locked for editing by ui automation_4",
                        "Is Lock Message Equal?");

                _appealSearch = _appealSummary.NavigateToAppealSearch();
                if (!_appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();

                StringFormatter.PrintMessage(
                    string.Format(
                        "Validate lock on appeal for claim  sequence sequence: {0} by searching from Appeal search page ",
                        claimSeq));
                _appealSearch.SelectAllAppeals();

                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                _appealSummary.IsAppealLocked()
                    .ShouldBeTrue("Appeal should be locked when it is in view mode by another user");
                _appealSummary.GetLockIConTooltip()
                    .ShouldBeEqual(
                        "This appeal is currently locked for editing by ui automation_4",
                        "Is Lock Message Equal?");
                _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                StringFormatter.PrintMessage(
                    string.Format("Validate lock on claim sequence: {0} when returning to appeal search page ",
                        claimSeq));
                _appealSearch.GetGridViewSection.GetTitleOfListIconPresentInGridForClassName("lock")
                    .ShouldBeEqual(
                        "This appeal is locked by ui automation_4 (uiautomation_4)",
                        "Is Lock Message Equal?");

            }
        }


        [Test] //CAR-354
        public void Validate_appeal_status_updates_when_docs_uploaded_to_docs_waiting_appeal_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealProcessingHistoryPage _appealProcessingHx;

                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claimSeq = testData["ClaimSequence"];
                var fileType = testData["FileType"];
                var fileName = testData["FileName"];
                
                StringFormatter.PrintMessage("Reset appeal to default i.e default status is Docs awaiting with no documents");
                _appealSearch.ResetAppealToDocumentWaiting(claimSeq);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);

                try
                {
                    _appealSummary.GetStatusValue().ShouldBeEqual(
                        AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(),
                        "Current status of appeal should be document awaiting");
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Category")
                        .ShouldBeNullorEmpty("Category Should Be Empty");
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Primary Reviewer")
                        .ShouldBeNullorEmpty("Category Should Be Empty");
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Assigned To")
                        .ShouldBeNullorEmpty("Category Should Be Empty");
                    var dueDate = _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date");

                    StringFormatter.PrintMessage("Upload document to appeal");
                    _appealSummary.UploadDocumentFromAppealSummary(fileType, fileName,
                        "new file test doc", 1);
                    _appealSummary.WaitForWorkingAjaxMessage();
                    StringFormatter.PrintMessage("Validate appeal status is updated to New");
                    _appealSummary.GetAppealDetails(1, 5).ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(),
                        "Current status of appeal should be updated to new after document is uploaded.");

                    _appealSummary.GetAppealDetailFieldPresentByLabel("Category")
                        .ShouldNotBeEmpty("Category Should Be Empty");
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Primary Reviewer")
                        .ShouldNotBeEmpty("Category Should Be Empty");
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Assigned To")
                        .ShouldNotBeEmpty("Category Should Be Empty");
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
                }
                finally
                {
                    _appealSummary.CloseAnyPopupIfExist();
                }
            }
        }
        
        #endregion

        #region PRIVATE METHODS

        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
        }
        private void VerifyThatNameIsInCorrectFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
        }

        private void VerifyThatDateIsInCorrectFormat(string date, string dateName)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The " + dateName + " Date'" + date + "' is in format MM/DD/YYYY");
        }

        private void VerifyThatReasonCodeIsInCorrectFormat(string reasonCode, string message)
        {
            Regex.IsMatch(reasonCode, @"^([a-zA-Z0-9\s]+)+ (-)+([a-zA-Z0-9\s]+)").ShouldBeTrue(message + " '" + reasonCode + "' is in format XXX - XXX ");
        }

        private void DeleteLastCreatedAppeal(string claimSeq, AppealSearchPage _appealSearch)
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();

                _appealManager.DeleteLastCreatedAppeal(claimSeq);
                automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
            }
        }

        #endregion

    }
}
