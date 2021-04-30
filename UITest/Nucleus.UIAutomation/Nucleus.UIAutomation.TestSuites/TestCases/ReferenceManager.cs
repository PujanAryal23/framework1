using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Reference;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ReferenceManager
    {
        #region PRIVATE FIELDS

        //private ReferenceManagerPage _referenceManager;
        //private ProfileManagerPage _profileManager;
        //private List<string> _assignedClientList;
        //private List<string> _flagList;

        // private CommonValidations _commonValidations;

        #endregion

        /* #region OVERRIDE METHODS

         /// <summary>
         /// Override ClassInit to add additional code.
         /// </summary>
         protected override void ClassInit()
         {
             try
             {
                 base.ClassInit();
                 _referenceManager = QuickLaunch.NavigateToReferenceManager();
                 _commonValidations = new CommonValidations(CurrentPage);

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
             CurrentPage = _referenceManager;
         }

         protected override void TestCleanUp()
         {
             base.TestCleanUp();
             if (_referenceManager.IsPageErrorPopupModalPresent())
                 _referenceManager.ClosePageError();
             if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
             {
                 _referenceManager = _referenceManager.Logout().LoginAsHciAdminUser().NavigateToReferenceManager();

             }
             _referenceManager.GetSideBarPanelSearch.OpenSidebarPanel();
             _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();

         }

         protected override void ClassCleanUp()
         {
             try
             {
                 _referenceManager.CloseDbConnection();
             }

             finally
             {
                 base.ClassCleanUp();
             }
         }

         #endregion
        */

        #region PROTECTED PROPERTIES

        protected  string FullyQualifiedClassName
     {
         get { return GetType().FullName; }
     }

        #endregion


        #region TEST SUITES




        [Test] //US69878
        [Retry(3)]
        public void Verify_security_and_navigation_of_the_reference_manager_page()
        {

            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                var TestName = new StackFrame(true).GetMethod().Name;
                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Manager,
                    new List<string> {SubMenu.ReferenceManager},
                    RoleEnum.OperationsManager.GetStringValue(),

                    new List<string> {PageHeaderEnum.ReferenceManager.GetStringValue()},
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});

                automatedBase.CurrentPage.Logout().LoginAsClientUser();
                automatedBase.CurrentPage.IsReferenceManagerSubMenuPresent()
                    .ShouldBeFalse("Reference Manager submenu should not be present for client user");
            }
        }

        [Test] //US69881
        public void Verify_reference_manager_filter_panel_and_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var referenceStatusOptions = paramsList["ReferenceStatusDropDownList"].Split(',').ToList();
                var alphaNumericCode = paramsList["AlphaNumericCode"];
                var longCode = paramsList["LongCode"];
                var noMatchingRecordsMsg = paramsList["NoMatchingRecordMsg"];

                var initialGridCount = _referenceManager.GetGridViewSection.GetGridRowCount();
                var searchList = _referenceManager.SearchRowHavingTricProc();

                StringFormatter.PrintMessageTitle(
                    "Verifying Search Bar Present by default and toggle button functionality");
                _referenceManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(
                        "Find References Search Panel on sidebar should open by default when user lands on page.");
                _referenceManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _referenceManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse(
                        "Find References Search Panel on sidebar should hidden when toggle button is clicked.");
                _referenceManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();

                StringFormatter.PrintMessageTitle("Verifying Reference Status field");
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Reference Status")
                    .ShouldBeEqual("Active", "Reference Status value is 'Active' by default");
                _referenceManager.GetSideBarPanelSearch.GetAvailableDropDownList("Reference Status")
                    .ShouldCollectionBeEqual(referenceStatusOptions,
                        "Drop Down List for Reference Status shows all status");

                StringFormatter.PrintMessageTitle("Verifying Client field");

                var assignedClientList =
                    _referenceManager.GetCommonSql.GetAssignedClientListForUser(automatedBase.EnvironmentManager.Username);
                assignedClientList.Insert(0, "ALL");

                ValidateDropDownForDefaultValueAndExpectedList("Client", assignedClientList, _referenceManager);

                StringFormatter.PrintMessageTitle("Verifying Flag field");
                var assignedFlagList = _referenceManager.GetCommonSql.GetFlagLists();
                assignedFlagList.Insert(0, "ALL");
                ValidateDropDownForDefaultValueAndExpectedList("Flag", assignedFlagList, _referenceManager, true);

                StringFormatter.PrintMessageTitle("Verifying Proc Code field");
                ValidateProcAndTrigCodeFields(alphaNumericCode, longCode, _referenceManager);

                StringFormatter.PrintMessage("Verifying search result are within range for Proc Code");
                ValidateSearchResultCodeIsWithinRange(searchList[1].Split('-')[0], _referenceManager);

                StringFormatter.PrintMessageTitle("Verifying Trigger Code field");
                ValidateProcAndTrigCodeFields(alphaNumericCode, longCode, _referenceManager, false);

                StringFormatter.PrintMessage("Verifying search result are within range for Trigger Code");
                ValidateSearchResultCodeIsWithinRange(searchList[2].Split('-')[0], _referenceManager,false);


                StringFormatter.PrintMessageTitle(
                    "Verifying Find button, no matching records message and Clear link button");
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Reference Status", "All");
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "ALL");
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", "ALL");
                _referenceManager.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", alphaNumericCode);
                _referenceManager.GetSideBarPanelSearch.SetInputFieldByLabel("Trigger Code", longCode);
                _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                _referenceManager.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage().ShouldBeEqual(
                    noMatchingRecordsMsg,
                    "'No matching records were found.' message is getting displayed ");

                _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Reference Status")
                    .ShouldBeEqual("Active", "Reference Status Should Equal");
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeNullorEmpty("Client Drop down Should be Empty");
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Flag")
                    .ShouldBeNullorEmpty("Flag Drop down Should be Empty");
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code")
                    .ShouldBeNullorEmpty("Proc Code Field Should be Empty");
                _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel("Trigger Code")
                    .ShouldBeNullorEmpty("Trigger Code Field Should be Empty");
                _referenceManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(initialGridCount, "Grid Result should be default view");

            }
        }

        [Test] //US69880
        public void Validate_display_of_existing_reference_records()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                _referenceManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(_referenceManager.GetActiveReferenceCount(), "Active Reference count should match");
                _referenceManager.GetFlagLists()
                    .ShouldCollectionBeSorted(false, "Result should be sorted by flag ascending");
                _referenceManager.GetGridViewSection.IsPencilIconPresentInRecordForRow()
                    .ShouldBeTrue("Pencil Icon should be visible in a rationale record");
                _referenceManager.GetGridViewSection.IsDeleteIconPresentInRecordForRow()
                    .ShouldBeTrue("Deactivate Icon should be visible in a reference record");
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Flag:")
                    .ShouldNotBeEmpty("Flag: label should be correct and value should not be null");
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Proc Code:")
                    .ShouldNotBeEmpty("Proc: label should be correct and value should not be null");
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Trig Code:").Length
                    .ShouldBeGreaterOrEqual(0, "Trig: label should be correct and value should not be null");
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Client:")
                    .ShouldNotBeEmpty("Client: label should be correct and value should not be null");

                StringFormatter.PrintMessage("Checking Flags:");
                _referenceManager.GetGridViewSection.GetGridListValueByCol()
                    .ShouldNotContain("", "Flag value should not be empty");
                StringFormatter.PrintMessage("Checking Proc codes:");

                _referenceManager.GetGridViewSection.GetGridListValueByCol(3)
                    .All(proc => proc.IsProcCodeInCorrectFormatForRange())
                    .ShouldBeTrue("Proc codes should match the expected format <XXXXX-XXXXX>");

                StringFormatter.PrintMessage("Checking Trig codes");
                _referenceManager.GetGridViewSection.GetGridListValueByCol(4).Where(trig => !string.IsNullOrEmpty(trig))
                    .All(trig => trig.IsTrigCodeEitherNullOrInCorrectFormat())
                    .ShouldBeTrue("Trig codes should match the expected format <XXXXX-XXXXX> or <XXXXX> or null");
                StringFormatter.PrintMessage("Checking Clients");
                _referenceManager.GetSideBarPanelSearch.OpenSidebarPanel();
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "ALL");
                _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                _referenceManager.GetGridViewSection.GetGridListValueByCol(5).Any(client => client == "ALL")
                    .ShouldBeTrue("At least one client should be 'ALL");
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Client:")
                    .ShouldBeEqual(ClientEnum.SMTST.ToString(), "Flag value should equal flag selected for references");
                _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", "ALL");
                _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                _referenceManager.GetGridViewSection.GetGridListValueByCol().Any(flag => flag == "ALL")
                    .ShouldBeTrue("At least one flag should be 'ALL'");
                _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();
                _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", "A30D");
                _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                _referenceManager.GetGridViewSection.GetValueInGridBylabelAndRow("Flag:")
                    .ShouldBeEqual("A30D", "Flag value should equal flag selected for references");

            }
        }

        [Test]//US69879
        public void Verify_create_new_Claim_reference()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                try
                {
                    // _referenceManager.ClickOnDeleteReferenceManager(paramList["NewClaimReferenceData"].Split(';'));
                    _referenceManager.IsAddIconPresent()
                        .ShouldBeTrue("A new + icon should be present at the header level of the page");
                    _referenceManager.ClickOnAddIcon();
                    _referenceManager.GetSideWindow.IsSideWindowBlockPresent()
                        .ShouldBeTrue("Claim reference form should open");
                    VerifyDropdownValuesInCreateReferenceForm("Client",
                        _referenceManager.GetActiveClientList(automatedBase.EnvironmentManager.HciAdminUsername), _referenceManager);
                    VerifyDropdownValuesInCreateReferenceForm("Flag", _referenceManager.GetFlagLists(), _referenceManager);
                    VerifyRequiredFieldsCreateNewClaimReferenceForm(paramList["NewClaimReferenceData"].Split(';'), _referenceManager);
                    VerifyProcTrigCodeValues(paramList["NewClaimReferenceData"].Split(';'), _referenceManager);
                    CreateNewandAddDuplicateVerifyCancelClaimReference(paramList["NewClaimReferenceData"].Split(';'), _referenceManager);
                }
                finally
                {
                    _referenceManager.DeleteClaimReferenceFromDB(paramList["NewClaimReferenceData"].Split(';'));
                }
            }
        }

        [Test, Category("CommonTableDependent")] //TANT-34 --US69115
        public void Verify_view_and_edit_existing_reference_record()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                _referenceManager.DeleteClaimReferenceFromDB(paramList["NewClaimReferenceData"].Split(';'));
                _referenceManager.DeleteClaimReferenceFromDB(paramList["EditClaimReferenceData"].Split(';'));
                _referenceManager.DeleteClaimReferenceFromDB(paramList["EditClaimReferenceData"].Split(';'), false);
                _referenceManager.RefreshPage();
                try
                {
                    if (!_referenceManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _referenceManager.ToggleSideBarMenu();
                    _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "SMTST");
                    _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                    // 'duplicateValues[]' stores values of the already existing reference data for SMTST client.
                    string[] duplicateValues =
                    {
                        "SMTST",
                        _referenceManager.GetGridViewSection.GetValueInGridByColRow(row: 1, col: 2), // Flag
                        _referenceManager.GetGridViewSection.GetValueInGridByColRow(row: 1, col: 3)
                            .Split('-')[0], // Proc Code From
                        _referenceManager.GetGridViewSection.GetValueInGridByColRow(row: 1, col: 3)
                            .Split('-')[1], // Proc Code To
                        _referenceManager.GetGridViewSection.GetValueInGridByColRow(row: 1, col: 4)
                            .Split('-')[0], // Trig Code From
                        _referenceManager.GetGridViewSection.GetValueInGridByColRow(row: 1, col: 4)
                            .Split('-')[1], // Trig Code To
                        "Test review guidelines"
                    };
                    _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();
                    _referenceManager.ToggleSideBarMenu();
                    _referenceManager.AddDataforClaimReference(paramList["NewClaimReferenceData"].Split(';'));
                    _referenceManager.ClickOnEditClaimReferenceIcon(paramList["NewClaimReferenceData"].Split(';'));
                    _referenceManager.GetSideWindow.IsSideWindowBlockPresent()
                        .ShouldBeTrue("Form opens up to show the fields to update the reference");
                    StringFormatter.PrintMessageTitle(
                        "Verifying the edit form has the previously stored values prefilled in the respective fields");
                    VerifyEditReferenceFormData(paramList["NewClaimReferenceData"].Split(';'), _referenceManager);
                    VerifyRequiredFieldsCreateNewClaimReferenceForm(paramList["NewClaimReferenceData"].Split(';'), _referenceManager,
                        isEditClaimReference: true);
                    StringFormatter.PrintMessageTitle("Verifying the Flag dropdown list");
                    VerifyDropdownValuesInCreateReferenceForm("Flag", _referenceManager.GetFlagLists(), _referenceManager,
                        isEditClaimReference: true);
                    StringFormatter.PrintMessage("Verifying type ahead functionality in Flag Dropdown list");
                    _referenceManager.GetSideWindow.SelectDropDownValue("Flag", "AADD",
                        false); //to validate type-ahead functionality
                    VerifyProcTrigCodeValues(paramList["EditClaimReferenceData"].Split(';'), _referenceManager,
                        isEditClaimReference: true);
                    StringFormatter.PrintMessageTitle("Validation of Cancel button");
                    _referenceManager.GetSideWindow.Cancel();
                    _referenceManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Message popup is shown to the user when 'Cancel' is clicked");
                    _referenceManager.GetPageErrorMessage()
                        .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to continue?");
                    _referenceManager.ClickCancelButtonOnEditConfirmWindow();
                    _referenceManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeTrue(
                        "The form is still present once the user clicks cancel in the confirmation" +
                        "popup");
                    VerifyRecordSavedAfterEdit(paramList["EditClaimReferenceData"].Split(';'), _referenceManager);
                    //Adding a new claim reference record to verify the addition of duplicate edited records 
                    StringFormatter.PrintMessageTitle("Verification of duplicated edited record");
                    _referenceManager.ClickOnEditClaimReferenceIcon(paramList["EditClaimReferenceData"].Split(';'));
                    _referenceManager.AddDataforClaimReference(duplicateValues, true);
                    _referenceManager.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "Error message pops up when trying to edit a reference to already existing reference");
                    _referenceManager.GetPageErrorMessage()
                        .ShouldBeEqual("A record with this criteria already exists.");
                    _referenceManager.ClosePageError();
                    _referenceManager.GetSideWindow.Cancel();
                    _referenceManager.ClickOkCancelOnConfirmationModal(true);
                    StringFormatter.PrintMessageTitle(
                        "Verifying a record with same data can be added, if previous record is inactive");
                    _referenceManager.ClickOnDeleteReferenceManager(paramList["EditClaimReferenceData"].Split(';'));
                    _referenceManager.AddDataforClaimReference(paramList["EditClaimReferenceData"].Split(';'));
                    _referenceManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Reference Status",
                        "Active");
                    _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _referenceManager.WaitForWorkingAjaxMessage();
                    _referenceManager
                        .IsDeleteButtonPresentForReferenceRecord(paramList["EditClaimReferenceData"].Split(';'))
                        .ShouldBeTrue("Record has been created, and should have a delete button");
                }
                finally
                {
                    _referenceManager.DeleteClaimReferenceFromDB(paramList["NewClaimReferenceData"].Split(';'));
                    _referenceManager.DeleteClaimReferenceFromDB(paramList["EditClaimReferenceData"].Split(';'));
                    _referenceManager.DeleteClaimReferenceFromDB(paramList["EditClaimReferenceData"].Split(';'), false);
                    _referenceManager.RefreshPage();
                }
            }
        }

        [Test, Category("CommonTableDependent")] //TANT-37+TANT-36
        public void Valdiate_deactivation_and_Audit_Trails_of_claim_reference()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string[] actionList = {"Deactivate", "Edit", "Edit", "Create"};
                var referenceRecordData = paramList["NewClaimReferenceData"].Split(';');
                _referenceManager.DeleteClaimReferenceFromDB(referenceRecordData);
                _referenceManager.DeleteClaimReferenceFromDB(referenceRecordData, false);
                _referenceManager.RefreshPage();
                try
                {
                    _referenceManager.GetGridViewSection.IsDeleteIconPresentInEachRecord()
                        .ShouldBeTrue("A Delete Icon should be present on each active reference record.");
                    _referenceManager.AddDataforClaimReference(referenceRecordData);
                    _referenceManager.IsDeleteButtonPresentForReferenceRecord(referenceRecordData)
                        .ShouldBeTrue("Record has been created, and should have a delete button");

                    StringFormatter.PrintMessageTitle("Verification of Create Action on Audit Window");

                    _referenceManager.ShowReferenceAudit(new[]
                    {
                        referenceRecordData[1], referenceRecordData[2], referenceRecordData[3], referenceRecordData[4],
                        referenceRecordData[5], referenceRecordData[0]
                    });
                    _referenceManager.IsReferenceManagerAuditWindowPresent()
                        .ShouldBeTrue("Audit Window should appear when the reference record is selected");
                    VerifyCreateEditDeleteOnAuditLogs("Create", DateTime.Now.ToString("MM/dd/yyyy"),
                        _referenceManager.GetLoggedInUserFullName(), _referenceManager);

                    StringFormatter.PrintMessageTitle("Verification of Edit Action on Audit Window");

                    _referenceManager.ClickOnEditClaimReferenceIcon(referenceRecordData);
                    _referenceManager.GetSideWindow.FillTextAreaBox("Review Guideline",
                        referenceRecordData[6] + "(edited)");
                    _referenceManager.GetSideWindow.Save();
                    _referenceManager.WaitForWorkingAjaxMessage();
                    _referenceManager.ShowReferenceAudit(
                        new[]
                        {
                            referenceRecordData[1], referenceRecordData[2], referenceRecordData[3],
                            referenceRecordData[4],
                            referenceRecordData[5], referenceRecordData[0]
                        }, true);
                    VerifyCreateEditDeleteOnAuditLogs("Edit", DateTime.Now.ToString("MM/dd/yyyy"),
                        _referenceManager.GetLoggedInUserFullName(), _referenceManager);

                    _referenceManager.ClickOnEditClaimReferenceIcon(referenceRecordData);
                    _referenceManager.GetSideWindow.FillTextAreaBox("Review Guideline", referenceRecordData[6]);
                    _referenceManager.GetSideWindow.Save();
                    _referenceManager.WaitForWorkingAjaxMessage();

                    StringFormatter.PrintMessageTitle("Verifying X icon and Ok/Cancel on Pop Up");
                    _referenceManager.ClickOnDeleteReferenceManager(referenceRecordData, true)
                        .ShouldBeTrue("Is X Icon Present?");
                    _referenceManager.GetPageErrorMessage()
                        .ShouldBeEqual("The Claim Reference will no longer be active. Do you wish to proceed?");
                    StringFormatter.PrintMessageTitle("Verification of Ok and Cancel button on confirmation popup");
                    _referenceManager.ClickOkCancelOnConfirmationModal(false);
                    _referenceManager.IsDeleteButtonPresentForReferenceRecord(referenceRecordData)
                        .ShouldBeTrue("Record should not be deletetd, and is still active");
                    _referenceManager.ClickOnDeleteReferenceManager(referenceRecordData);

                    StringFormatter.PrintMessageTitle("Verification of Delete Action on Audit Window");

                    _referenceManager.ShowReferenceAudit(
                        new[]
                        {
                            referenceRecordData[1], referenceRecordData[2], referenceRecordData[3],
                            referenceRecordData[4],
                            referenceRecordData[5], referenceRecordData[0]
                        }, true);
                    _referenceManager.IsReferenceManagerAuditWindowPresent()
                        .ShouldBeTrue("Audit Window should appear when the reference record is selected");
                    VerifyCreateEditDeleteOnAuditLogs("Deactivate", DateTime.Now.ToString("MM/dd/yyyy"),
                        _referenceManager.GetLoggedInUserFullName(), _referenceManager);

                    StringFormatter.PrintMessageTitle(
                        "Verification of All Actions on Audit Window and sorting by modified date");

                    for (var j = 0; j < 3; j++)
                    {
                        VerifyCreateEditDeleteOnAuditLogs(actionList[j], DateTime.Now.ToString("MM/dd/yyyy"),
                            _referenceManager.GetLoggedInUserFullName(), _referenceManager, j + 1);
                    }

                    _referenceManager.GetAuditHistoryListByCol(2).Select(DateTime.Parse).ToList().IsInDescendingOrder()
                        .ShouldBeTrue("Is Audit History in Descending Order By Date?");

                    _referenceManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Reference Status",
                        "Inactive");
                    _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _referenceManager.ShowAppealRationaleAudit(referenceRecordData);
                    _referenceManager.GetDeletedIconToolTipText(referenceRecordData)
                        .ShouldBeEqual("Inactive", "Is tooltip on delete icon Inactive?");
                    _referenceManager.ClickOnEditClaimReferenceIcon(referenceRecordData);

                    string[] fieldList = {"Flag", "Proc Code From", "Proc Code To", "Trig Proc From", "Trig Proc To"};
                    foreach (var eachField in fieldList)
                    {
                        _referenceManager.IsTextBoxDisabled(eachField)
                            .ShouldBeTrue("Is " + eachField + " Field Not Editable?");
                    }

                    _referenceManager.IsReviewGuidelineDisabled()
                        .ShouldBeTrue("Is Review Guideline Field Not Editable?");
                    StringFormatter.PrintMessage(
                        "Check if save and cancel button is disabled  by calling Save function and passing true for flag :checkIfDisabled");
                    _referenceManager.GetSideWindow.Save(true).ShouldBeTrue("Is Save Button Disabled?");
                    _referenceManager.GetSideWindow.Cancel(true).ShouldBeFalse("Is Cancel Button Disabled?");
                    _referenceManager.GetSideWindow.Cancel();
                    StringFormatter.PrintMessageTitle(
                        "Verifying a record with same data can be added, if previous record is inactive");
                    _referenceManager.AddDataforClaimReference(referenceRecordData);
                    _referenceManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Reference Status",
                        "Active");
                    _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _referenceManager.WaitForWorkingAjaxMessage();
                    _referenceManager.IsDeleteButtonPresentForReferenceRecord(referenceRecordData)
                        .ShouldBeTrue("Record has been created, and should have a delete button");
                }
                finally
                {
                    _referenceManager.DeleteClaimReferenceFromDB(referenceRecordData);
                    _referenceManager.DeleteClaimReferenceFromDB(referenceRecordData, false);
                    _referenceManager.RefreshPage();
                }
            }
        }

        [Test] //CAR-1212
        public void Verify_creation_of_client_specific_references_when_a_duplicate_all_refernce_is_present()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ReferenceManagerPage _referenceManager;
                automatedBase.CurrentPage = _referenceManager = automatedBase.QuickLaunch.NavigateToReferenceManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var oldClaimReferenceData = paramsList["OldClaimReferenceData"].Split(';').ToArray();
                var newClaimReferenceData = paramsList["NewClaimReferenceData"].Split(';').ToArray();
                var claimSeq = paramsList["ClaimSequence"];

                _referenceManager.DeleteClaimReferenceFromDB(newClaimReferenceData);
                try
                {
                    _referenceManager.RefreshPage();
                    if (!_referenceManager.ClickOnDeleteReferenceManager(oldClaimReferenceData, true))
                        _referenceManager.AddDataforClaimReference(oldClaimReferenceData);
                    else
                        _referenceManager.ClickOkCancelOnConfirmationModal(false);

                    _referenceManager.AddDataforClaimReference(newClaimReferenceData);
                    _referenceManager.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "User should not be able to  create a client specific claim reference when there is an existing claim reference.");
                    _referenceManager.ClickOnDeleteReferenceManager(newClaimReferenceData, true)
                        .ShouldBeTrue("Is Newly Added Reference Manager with different Review Guide Line.");
                    _referenceManager.ClickOkCancelOnConfirmationModal(false);
                    var _claimAction = _referenceManager.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);

                    _claimAction.ClickOnFlaggedLines(1);
                    _claimAction.GetFlagDetailsByLabel("Review Guidelines").ShouldBeEqual(newClaimReferenceData[6],
                        string.Format("Review Guidelines should be equal to {0}", newClaimReferenceData[6]));
                    _referenceManager.DeleteClaimReferenceFromDB(newClaimReferenceData);
                    _claimAction.ClickWorkListIcon();
                    _claimAction.SearchByClaimSequence(claimSeq);
                    _claimAction.ClickOnFlaggedLines(1);
                    _claimAction.GetFlagDetailsByLabel("Review Guidelines").ShouldBeEqual(oldClaimReferenceData[6],
                        string.Format("Review Guidelines should be equal to {0}", oldClaimReferenceData[6]));

                }
                finally
                {
                    automatedBase.CurrentPage.ClickOnQuickLaunch().NavigateToReferenceManager();
                }
            }
        }

        #endregion

        #region PRIVATE METHODS
        private void ValidateDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, ReferenceManagerPage _referenceManager, bool isSorted = false)
        
        {


            var actualDropDownList = _referenceManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
            {
                actualDropDownList.RemoveAll(string.IsNullOrEmpty);
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + "Is Collection of List Expected and Equal?");
            }
            if (!isSorted)
            {
                actualDropDownList.Remove("ALL");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _referenceManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
        }

        private void ValidateProcAndTrigCodeFields(string alphaNumericCode, string longCode, ReferenceManagerPage _referenceManager, bool isProcCode=true, bool isEditClaimReference = false)
        {
            var label = isProcCode ? "Proc Code" : "Trigger Code";
            _referenceManager.GetSideBarPanelSearch.SetInputFieldByLabel(label, alphaNumericCode);
            _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
            _referenceManager.IsPageErrorPopupModalPresent().ShouldBeFalse("There should not be an error message when performing a search by" +
                                                                           "alphanumeric "+label);
            _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(alphaNumericCode);
            _referenceManager.GetSideBarPanelSearch.SetInputFieldByLabel(label, longCode);
            _referenceManager.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(longCode.Remove(5), "The "+ label +" field" +
                                                                                                                        "takes only 5 digit value");
            _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();

        }

        private void ValidateSearchResultCodeIsWithinRange(string codeToSearch, ReferenceManagerPage _referenceManager, bool isProcCode = true)
        {
            var label = isProcCode ? "Proc Code" : "Trigger Code";
            _referenceManager.GetSideBarPanelSearch.SetInputFieldByLabel(label, codeToSearch);
           _referenceManager.GetSideBarPanelSearch.ClickOnFindButton();
            _referenceManager.WaitForWorkingAjaxMessage();

            var listOfProcCodesInSearchResult = _referenceManager.GetListByLabelInSearchGrid(label);

            foreach (var code in listOfProcCodesInSearchResult)
            {
                if (Regex.IsMatch(code, @"^[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}$"))
                {

                    var codeFrom = (code.Split('-')[0]);
                    var codeTo = (code.Split('-')[1]);
                    (String.Compare(codeTo, codeToSearch) * String.Compare(codeToSearch, codeFrom) >= 0)
                        .ShouldBeTrue("The searched " + label + " is within range");
                }
            
                else
                {
                    code.ShouldBeEqual(codeToSearch).ShouldBeEqual("The " + label + " in search result and the searched procCode are equal");
                }
            }

            _referenceManager.GetSideBarPanelSearch.ClickOnClearLink();

        }
        private void VerifyRequiredFieldsCreateNewClaimReferenceForm(string[] newClaimReference, ReferenceManagerPage _referenceManager, bool isEditClaimReference = false)
        {
            StringFormatter.PrintMessage("Verify the required fields in the Add Claim Reference Form");
            _referenceManager.GetSideWindow.IsAsertiskPresent("Proc Code From").ShouldBeTrue("Is Asterisk Present on Proc Code From? ");
            _referenceManager.GetSideWindow.IsAsertiskPresent("Proc Code To").ShouldBeTrue("Is Asterik present in the Proc Code To ?");
            _referenceManager.GetSideWindow.IsAsertiskPresent(("Review Guideline")).ShouldBeTrue(("Is Asterik present in Review Guideline?"));

            if (isEditClaimReference)
            {
                _referenceManager.GetSideWindow.GetInputBox("Proc Code From", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Proc Code To", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Trig Proc From", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Trig Proc To", isEditClaimReference: isEditClaimReference).ClearElementField();
            }
            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage()
                .ShouldBeEqual("Proc codes are required before record can be saved.");
            _referenceManager.ClosePageError();
            _referenceManager.GetSideWindow.FillInputBox("Proc Code To",newClaimReference[3], isEditReference: isEditClaimReference);
            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage()
                .ShouldBeEqual("Proc codes are required before record can be saved.");
            _referenceManager.ClosePageError();
            if (!isEditClaimReference)
            {
                _referenceManager.GetSideWindow.SelectDropDownValue("Client", "ALL");
                _referenceManager.GetSideWindow.GetTextAreaData("Review Guideline")
                    .ShouldBeEqual("Per ALL client review guidelines");
                _referenceManager.GetSideWindow.SelectDropDownValue("Client", newClaimReference[0]);
            }
            _referenceManager.GetSideWindow.GetTextAreaData("Review Guideline")
                .ShouldBeEqual(newClaimReference[6]);
            _referenceManager.GetSideWindow.FillInputBox("Proc Code From", newClaimReference[2], isEditReference: isEditClaimReference);
            _referenceManager.GetSideWindow.ClearTextArea("Review Guideline");
            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage()
                .ShouldBeEqual("Valid Review Guideline must be entered in the Review Guideline fields before the record can be saved.");
            _referenceManager.ClosePageError();
            if(!isEditClaimReference)
                _referenceManager.GetSideWindow.SelectDropDownValue("Client", "ALL");
            _referenceManager.GetSideWindow.FillTextAreaBox("Review Guideline", newClaimReference[6]);
        }

        private void VerifyProcTrigCodeValues(string[] newClaimReference, ReferenceManagerPage _referenceManager, bool isEditClaimReference = false)
        {
            if (isEditClaimReference)
            {
                _referenceManager.GetSideWindow.GetInputBox("Proc Code From", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Proc Code To", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Trig Proc From", isEditClaimReference: isEditClaimReference).ClearElementField();
                _referenceManager.GetSideWindow.GetInputBox("Trig Proc To", isEditClaimReference: isEditClaimReference).ClearElementField();
            }

            _referenceManager.GetSideWindow.GetInputBox("Proc Code From" , isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.GetInputBox("Proc Code To", isEditClaimReference: isEditClaimReference).ClearElementField();

            StringFormatter.PrintMessage("Verify the max length for Proc code and Trig Code");
            _referenceManager.GetSideWindow.Check_Maxlength_by_Label("Proc Code From", "5", isEditClaimReference: isEditClaimReference);
            _referenceManager.GetSideWindow.Check_Maxlength_by_Label("Proc Code To", "5", isEditClaimReference: isEditClaimReference);
            _referenceManager.GetSideWindow.Check_Maxlength_by_Label("Trig Proc From", "5", isEditClaimReference: isEditClaimReference);
            _referenceManager.GetSideWindow.Check_Maxlength_by_Label("Trig Proc To", "5", isEditClaimReference: isEditClaimReference);

            StringFormatter.PrintMessage("Verify validation message for invalid Proc code and Trig code values");
            _referenceManager.GetSideWindow.FillInputBox("Proc Code From", "12345", true, isEditReference: isEditClaimReference);

            _referenceManager.GetSideWindow.GetInputFieldText("Proc Code To" , isReferenceManager: isEditClaimReference).ShouldBeEqual("12345",
                "Is auto populated Proc Code To value equal to Proc Code From on TAB?");

            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage().ShouldBeEqual(!isEditClaimReference
                ? "Procedure codes must be valid to create a claim reference."
                : "Procedure codes must be valid for updating a claim reference.");
            _referenceManager.ClosePageError();
            _referenceManager.GetSideWindow.GetInputBox("Proc Code From", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.FillInputBox("Proc Code From", newClaimReference[2], isEditReference: isEditClaimReference);
            _referenceManager.GetSideWindow.FillInputBox("Proc Code To", newClaimReference[2], isEditReference: isEditClaimReference);

            _referenceManager.GetSideWindow.FillInputBox("Trig Proc To", "12345", isEditReference: isEditClaimReference);
            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage().ShouldBeEqual("Trigger codes are required before record can be saved.");
            _referenceManager.ClosePageError();
            _referenceManager.GetSideWindow.GetInputBox("Trig Proc To", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.FillInputBox("Trig Proc From", "12345", true, isEditReference: isEditClaimReference);
            _referenceManager.GetSideWindow.GetInputFieldText("Trig Proc To", isReferenceManager: isEditClaimReference).ShouldBeEqual("12345",
                "Is auto populated Trig Code To value equal to Proc Code From on TAB?");
            _referenceManager.GetSideWindow.Save();
            _referenceManager.WaitToLoadPageErrorPopupModal(1000);
            _referenceManager.GetPageErrorMessage().ShouldBeEqual(!isEditClaimReference
                ? "Trigger codes must be valid to create a claim reference."
                : "Trigger codes must be valid for updating a claim reference.");
            _referenceManager.ClosePageError();

            StringFormatter.PrintMessage("Verify the character limit for Review Guideline field");
            var newdescp = new string('a', 4494);
            _referenceManager.GetSideWindow.FillTextAreaBox("Review Guideline", newdescp, 4494);
            _referenceManager.GetSideWindow.GetTextAreaData("Review Guideline").Length.Equals(4493);
           
            _referenceManager.GetSideWindow.GetInputBox("Proc Code From", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.GetInputBox("Proc Code To", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.GetInputBox("Trig Proc From", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.GetInputBox("Trig Proc To", isEditClaimReference: isEditClaimReference).ClearElementField();
            _referenceManager.GetSideWindow.FillTextAreaBox("Review Guideline", newClaimReference[6]);

        }

        private void VerifyDropdownValuesInCreateReferenceForm(string label, IList<string> collectionToEqual, ReferenceManagerPage _referenceManager, bool isEditClaimReference = false)
        {
            var list = _referenceManager.GetSideWindow.GetDropDownList(label);
            list.RemoveAt(0);
            list.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order."); ;
            list.ShouldCollectionBeEqual(collectionToEqual, label + " List is As Expected");
           
            if (!isEditClaimReference)
            _referenceManager.GetSideWindow.GetDropDownListDefaultValue(label).ShouldBeEqual("ALL", string.Format("Is {0}'s default value All?", label));
            if (label == "Client")
            {
                for (int j = 1; j <= 2; j++)
                {
                    var randm = new Random();
                    var i = randm.Next(0, list.Count - 2);
                    _referenceManager.GetSideWindow.SelectDropDownValue("Client", list[i]);
                    _referenceManager.GetSideWindow.GetTextAreaData("Review Guideline")
                        .ShouldBeEqual("Per " + list[i] + " client review guidelines");
                    
                }
                // code to include during UAT run
                //_referenceManager.GetSideWindow.SelectDropDownValue("Client", "ALL Centene");
                //_referenceManager.GetSideWindow.GetTextAreaData("Review Guideline")
                //    .ShouldBeEqual("Per ALL Centene client review guidelines");
            }
            
        }

        private void CreateNewandAddDuplicateVerifyCancelClaimReference(string[] newClaimReferenceData, ReferenceManagerPage _referenceManager)
        {
            var resultCount = _referenceManager.GetGridViewSection.GetGridRowCount();
            _referenceManager.AddDataforClaimReference(newClaimReferenceData);
            _referenceManager.GetGridViewSection.GetGridRowCount().ShouldBeGreater(resultCount, "A record should be saved and displayed on result list.");
            _referenceManager.AddDataforClaimReference(newClaimReferenceData, waitToLoad:false);
            _referenceManager.WaitToLoadPageErrorPopupModal();
            _referenceManager.GetPageErrorMessage()
                .ShouldBeEqual("A record with this criteria already exists.");
            _referenceManager.ClosePageError();
            _referenceManager.GetSideWindow.Cancel();
            _referenceManager.WaitToLoadPageErrorPopupModal();
            _referenceManager.GetPageErrorMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to continue?");
            _referenceManager.ClickOkCancelOnConfirmationModal(false);
            _referenceManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeTrue("Add Claim Reference form present after selecting Cancel?");
            _referenceManager.GetSideWindow.Cancel();
            _referenceManager.WaitToLoadPageErrorPopupModal();
            _referenceManager.ClickOkCancelOnConfirmationModal(true);
            _referenceManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeFalse("Add Claim Reference form not present after selecting Ok on pop up?");
             
        }

        private void VerifyEditReferenceFormData(string[] claimReferenceData, ReferenceManagerPage _referenceManager)
        {
            _referenceManager.GetSideWindow.GetDropDownListDefaultValue("Flag").ShouldBeEqual(claimReferenceData[1]);
            _referenceManager.GetSideWindow.GetInputFieldText("Proc Code From", true).ShouldBeEqual(claimReferenceData[2]);
            _referenceManager.GetSideWindow.GetInputFieldText("Proc Code To", true)
                .ShouldBeEqual(claimReferenceData[3]);
            _referenceManager.GetSideWindow.GetInputFieldText("Trig Proc From", true)
                .ShouldBeEqual(claimReferenceData[4]);
            _referenceManager.GetSideWindow.GetInputFieldText("Trig Proc To", true)
                .ShouldBeEqual(claimReferenceData[5]);
            _referenceManager.GetSideWindow.GetTextAreaData("Review Guideline").ShouldBeEqual(claimReferenceData[6]);


        }

        private void VerifyRecordSavedAfterEdit(string[] claimReferenceData, ReferenceManagerPage _referenceManager)
        {
            StringFormatter.PrintMessageTitle("Verification if record is saved with valid data");
            _referenceManager.AddDataforClaimReference(claimReferenceData, true);
            //_referenceManager.GetSideWindow.Save();

            _referenceManager.ClickOnEditClaimReferenceIcon(claimReferenceData);

            StringFormatter.PrintMessage("Verifying whether the edited data is saved correctly");
            VerifyEditReferenceFormData(claimReferenceData, _referenceManager);
            _referenceManager.GetSideWindow.Cancel();
            _referenceManager.ClickOkButtonOnEditConfirmWindow();
        }
        #endregion

        public void VerifyCreateEditDeleteOnAuditLogs(string value,string modifiedDate,string modifiedBy, ReferenceManagerPage _referenceManager,int row=1)
        {
            _referenceManager.GetReferenceManagerAuditHistoryActionByRowCol(row, 1).ShouldBeEqual(value, "Is Action field valid?");
            _referenceManager.GetReferenceManagerAuditHistoryActionByRowCol(row, 2)
                 .ShouldBeEqual(modifiedDate, "Is Modified Date field valid?");
            _referenceManager.GetReferenceManagerAuditHistoryActionByRowCol(row, 3).ShouldBeEqual(modifiedBy, "Is Modified By field valid?");

        }
        
}
}
