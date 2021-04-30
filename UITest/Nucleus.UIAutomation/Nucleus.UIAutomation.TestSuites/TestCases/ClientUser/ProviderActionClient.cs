using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Support.Utils;
using NUnit.Framework;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using System.Text.RegularExpressions;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.PageServices.Settings;
using UIAutomation.Framework.Core.Driver;
using ExcelReader = UIAutomation.Framework.Utils.ExcelReader;


namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]

    public class ProviderActionClient
    {
        /*#region PRIVATE PROPERTIES

        //private ProviderSearchPage _providerSearch;
        private ProviderActionPage _providerAction;
        private ClientSearchPage _clientSearch;
        
        private SuspectProvidersPage _suspectProviders;
        private ProviderSearchPage _providerSearch;
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
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
            automatedBase.CurrentPage = _providerSearch;
            _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();

            if (automatedBase.CurrentPage.GetPageHeader()!=PageHeaderEnum.ProviderSearch.GetStringValue())
            {
                _providerSearch = _providerAction.NavigateToProviderSearch();
            }

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                //TODO: Check client and switch
                _providerSearch = _providerSearch.Logout().LoginAsClientUser().NavigateToProviderSearch();
            }
            _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            _providerSearch.ClearAll();
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                ProviderEnum.AllProviders.GetStringValue());
        }



        #endregion*/

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] //TANT-282
        public void Verify_google_icon_in_provider_action_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                var prvSeq = "55100";

                _providerAction = _providerSearch.SearchByProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.IsGoogleSearchIconPresent().ShouldBeTrue("Is Google Search icon present?");
                _providerAction.CloseAnyPopupIfExist();
                _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
            }
        }

        [Test] //CAR-3056(CAR-2989)
        public void Verification_of_Google_quick_search_button_to_Provider_name_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var prvSeqs = testData["ProvSeq"].Split(',').ToList();

                StringFormatter.PrintMessage(
                    "Verification with provseqs with complete provname, specialty and state, then without specialty desc and state " +
                    "and finally without provname and specialty desc and state");
                foreach (var prvSeq in prvSeqs)
                {
                    _providerAction =
                        _providerSearch.SearchByProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                    var googleSearchInputText = _providerAction.GetSearchInputOfGoogleDb(prvSeq);
                    _providerAction.IsGoogleSearchIconPresent().ShouldBeTrue("Is Google Search icon present?");
                    _providerAction.ClickGoogleSearchIcon();
                    _providerAction.WaitForStaticTime(2000);
                    _providerAction.SwitchToPopUpWindow();
                    _providerAction.GetGooglePopupPageTitle()
                        .ShouldBeEqual($"{googleSearchInputText} - Google Search", "Page title should match");
                    _providerAction.GetGooglePopupPageTitle()
                        .AssertIsContained("Google Search", "Is page navigated to Google?");

                    _providerAction.GetGoogleSearchInputFieldValue()
                        .ShouldBeEqual(googleSearchInputText,
                            "Search input field should be auto populated by Provider Name, Specialty and State");
                    _providerAction.CloseAnyPopupIfExist();
                    _providerSearch = _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                }
            }
        }

        [Test] //CAR-1752 (CAR-2002) + CAR-2051 + CAR-2103
        public void Validate_User_Specified_Condition_Form_Options_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var provSeq = testData["ProviderSeq"];
                var procCodes = testData["ProcCodes"].Split(',');
                var userSpecifiedCondition = testData["UserSpecifiedConditions"].Split(',');


                try
                {
                    _providerSearch.DeleteConditionIdByProviderSequence(provSeq);
                    _providerAction = _providerSearch.NavigateToProviderAction(() =>
                    {
                        _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", provSeq);
                        _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                        _providerSearch.WaitForPageToLoad();
                    });

                    StringFormatter.PrintMessage(
                        "Verification of dropdown options and no search initiation without any search criteria");
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.AreUserSpecifiedConditionsPresent()
                        .ShouldBeTrue("Are user specified condition options present?");
                    _providerAction.ClickOnSelectConditionFromDropdownIcon();
                    _providerAction.GetUserSpecifiedConditionList().ShouldCollectionBeEqual(
                        _providerAction.GetUserSpecifiedConditonEditList(),
                        "User specified conditions drop dowm values should match");
                    _providerAction.ClickOnSelectConditionFromDropdownIcon();
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent(_providerAction, "Search cannot be initiated without criteria entered.");


                    StringFormatter.PrintMessage(
                        "Verification of Search without Search Condition, invalid proc code and  end proc code being auto populated with begin code value ");
                    _providerAction.SetBeginCodeAndEndCode("ABCDE", isTabRequired: true);
                    _providerAction.GetValueofInputConditionCode(false)
                        .ShouldBeEqual("ABCDE", "End Proc should be auto populated");
                    _providerAction.GetTextIfAddOrSearchButton().ShouldBeEqual("Add");
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent(_providerAction, "Cannot add codes without a condition selected.");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent(_providerAction, "Search is invalid. Please enter new search criteria.");

                    StringFormatter.PrintMessage("Verification that begin code and end code should be sequential");
                    _providerAction.SetBeginCodeAndEndCode(procCodes[3], isTabRequired: true);
                    _providerAction.SetEndCode(procCodes[2]);
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    IsErrorPopUpPresent(_providerAction, "Selected codes are not a valid range.");

                    StringFormatter.PrintMessage("Verification of Add functionality with single proc code");
                    _providerAction.SetBeginCodeAndEndCode(procCodes[0], isTabRequired: true);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    _providerAction.WaitForStaticTime(1000);
                    _providerAction.IsSearchResultsPresent().ShouldBeFalse("Is search results present?");
                    _providerAction.IsMactchingConditionsPresent().ShouldBeFalse("Is Matching Conditions present?");
                    _providerAction.IsSelectedConditionsPresentInActionConditions()
                        .ShouldBeTrue("Is Selected Conditions present in Action Conditions?");
                    _providerAction.AreSelectedUserSpecifiedConditionsPresent()
                        .ShouldBeTrue("Are selected user specified condition options present?");

                    StringFormatter.PrintMessage("Verification of user being able to deselect the code");
                    _providerAction.SelectProcCodeUserSpecifiedCondition();
                    _providerAction.IsSelectedConditionsPresentInActionConditions()
                        .ShouldBeFalse("Is Selected Conditions present in Action Conditions?");
                    _providerAction.AreSelectedUserSpecifiedConditionsPresent()
                        .ShouldBeFalse("Are selected user specified condition options present?");

                    StringFormatter.PrintMessage("Verification of cancel functionality");
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    _providerAction.ClickOnCancelUserAddedAction();
                    _providerAction.IsUserSpcifiedConditionFormPresent().ShouldBeFalse(
                        "Selecting cancel after selections have been made will close the User Specified Conditions form");
                    _providerAction.IsSelectedUserSpecifiedConditionComponentPresent().ShouldBeTrue(
                        "Selecting cancel after selections have been made will still retain the selected condition");
                    _providerAction.IsActionConditionFormPresent().ShouldBeTrue("Is form open?");
                    _providerAction.IsSelectedConditionsPresentInActionConditions()
                        .ShouldBeTrue("Are Selected condition present?");

                    StringFormatter.PrintMessage("Verification of Add functionality with range of codes");
                    _providerAction.ClickSearchForConditionsByCode();
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[0]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[1], procCodes[2]);
                    _providerAction.IsAddButtonPresentInUserSpecifiedCondition()
                        .ShouldBeTrue("Add Button should be displayed When a range of Proc Codes is entered");
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessage(
                        "Verification of same range not allowed to be added to another user specified condition");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[1], procCodes[2]);
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    IsErrorPopUpPresent(_providerAction, "The selected code(s) has already been added to another user specified condition.");
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    _providerAction.WaitForStaticTime(500);

                    StringFormatter.PrintMessage("Verification of Audit record after saving the range");
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.WaitForWorking();
                    _providerAction.SelectProviderConditionByConditionId(userSpecifiedCondition[0]);
                    _providerAction.ClickOnConditionDetailsIcon();
                    var customCodes = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    customCodes.Replace("\r\n", string.Empty)
                        .Contains(String.Format("Codes of Concern:{0}", "Custom Codes Applied9921399214"))
                        .ShouldBeTrue("Checking Custom codes applied is present");

                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickSearchForConditionsByCode();

                    #region CAR-2103

                    StringFormatter.PrintMessage(
                        "Verification that provider can only be on review for single proc code regardless of user specified condition selected");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[1], isTabRequired: true);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    Set_action_conditions(_providerAction);
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");

                    StringFormatter.PrintMessage(
                        "Verification that same range is not allowed to add for different Specified conditions"); //CAR-2103
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[1], procCodes[2]);
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    Set_action_conditions(_providerAction);
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");
                    _providerAction.WaitForStaticTime(100);

                    #endregion

                    StringFormatter.PrintMessage(
                        "Verification that only one instance of each user specified condition will be created");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[0]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[6], isTabRequired: true);
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    IsErrorPopUpPresent(_providerAction, "Search is invalid. Please enter new search criteria.");

                    #region 2103

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag all codes, but other active manual condition(s) already exist that are flagging any codes.");
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    Set_User_specified_conditions(_providerAction, userSpecifiedCondition[1], "", "", isFlagAllCodes: true);
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");

                    #endregion

                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "Verification if the total result set exceeds 100 codes, a message will be shown to the user");
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickSearchForConditionsByCode();
                    Verify_if_result_set_exceeds_100_codes_warning_message_will_be_displayed(_providerAction, procCodes[1],
                        procCodes[7]);
                    _providerAction.SelectProcCodeUserSpecifiedCondition();
                    _providerAction.GetResultSetProcCodeCount().ShouldBeLess(100,
                        "User should be able to deselect the code in this view to exclude them from flagging. ");
                    _providerAction.CountOfCheckboxNextToTheCode()
                        .ShouldBeEqual(_providerAction.GetResultSetProcCodeCount());
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessage(
                        "Verification of addition of new container showing newly added condition and selected codes");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.SetBeginCodeAndEndCode(procCodes[4], procCodes[5]);
                    _providerAction.IsMactchingConditionsPresent().ShouldBeFalse("Is Matching Conditions present?");
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    _providerAction.GetResultSetContainerCount().ShouldBeEqual(2,
                        "A new result set container should be added for the new search criteria");
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    _providerSearch.DeleteConditionIdByProviderSequence(provSeq);

                    StringFormatter.PrintMessageTitle("Verify Flag All Codes message");
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickSearchForConditionsByCode();
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[0]);
                    _providerAction.ClickOnFlagAllCodes();
                    _providerAction.AreRangeSearchFieldsEnabled().ShouldBeFalse("Are range search fields disabled?");
                    _providerAction.IsAddButtonPresentInUserSpecifiedCondition()
                        .ShouldBeTrue("Add Button should be displayed When a range of Proc Codes is entered");
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    var msg = _providerAction.GetSelectedUserSpecifiedCondition();
                    string[] splitlist = msg.Split('-');
                    string selectedUserSpecifiedCondition = splitlist[0].Trim();
                    _providerAction.GetMessageWhenFlagAllCodesIsSelected()
                        .ShouldBeEqual("Flag all services reported by this provider.");
                    selectedUserSpecifiedCondition.ShouldBeEqual(_providerAction
                        .GetSelectedUserSpecifiedConditionFromDropdown());
                    _providerAction.WaitForStaticTime(100);


                    StringFormatter.PrintMessage(
                        "If the user attempts to flag all codes when another condition is already flagging all codes");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[1]);
                    _providerAction.WaitForStaticTime(1000);
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    IsErrorPopUpPresent(_providerAction, "A user specified condition is already flagging all codes for this provider.");


                    StringFormatter.PrintMessage("Attempt to save without filling action condition form");
                    _providerAction.ClickOnSaveActionCondition();
                    IsErrorPopUpPresent(_providerAction, "You have not selected an action.You have not selected a reason code.A Note is required before the record can be saved.");
                    Set_action_conditions(_providerAction, false);

                    StringFormatter.PrintMessageTitle("Verify audit message");
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.WaitForWorking();
                    _providerAction.ClickOnConditionDetailsIcon();
                    var codeOfConcernRow = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    codeOfConcernRow.Contains(String.Format("Codes of Concern:\r\n{0}", "Flag All Codes"));

                    #region CAR-2103

                    StringFormatter.PrintMessage(
                        "If the user attempts to flag all codes when another condition is already flagging all codes after generating decision rationale");
                    Set_User_specified_conditions(_providerAction, userSpecifiedCondition[1], "", "", isFlagAllCodes: true);
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");
                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag a specified code when another active condition exists that is flagging ALL codes");

                    Set_User_specified_conditions(_providerAction, userSpecifiedCondition[1], procCodes[1], procCodes[1]);
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");
                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag codes when another active condition exists that is flagging ALL codes");
                    Set_User_specified_conditions(_providerAction, userSpecifiedCondition[1], procCodes[1], procCodes[2]);
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");
                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag all codes, but other active manual condition(s) already exist that are flagging any codes.");
                    Set_User_specified_conditions(_providerAction, userSpecifiedCondition[1], "", "", isFlagAllCodes: true);
                    Set_action_conditions(_providerAction, false);
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(_providerAction, "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).");

                    #endregion
                }

                finally
                {
                    _providerSearch.DeleteConditionIdByProviderSequence(provSeq);
                }
            }
        }

        

        [Test] //CAR-1664 (CAR-1854)

        public void Verify_Export_12_month_History_to_Excel_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName,
                        "Client12MonthsClaimHistoryExportedExcelFileHeaders").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var prvSequencesWithData = parameterList["PrvSequenceWithData"];
                var prvSequencesWithoutData = parameterList["PrvSequenceWithoutData"];
                var claimSeq = parameterList["ClaimSeq"];
                var lineNo = parameterList["LineNo"];
                var lineNoWithProcDesc = parameterList["LineNoWithProcDesc"];
                var label = parameterList["Label"];
                var sheetname = parameterList["SheetName"];
                var expectedDataList =
                    _providerSearch.GetExport12MonthsHistoryExcelData(prvSequencesWithData, claimSeq, lineNo);
                var expectedDataListWithProcDesc =
                    _providerSearch.GetExport12MonthsHistoryExcelData(prvSequencesWithData, claimSeq,
                        lineNoWithProcDesc);
                var fileNameFromDb =
                    _providerSearch.GetExport12MonthsHistoryExcelFileNameFromDatabase(prvSequencesWithData, claimSeq,
                        lineNo) + "_12Months";

                _providerSearch.DeleteDocumentAuditRecordFromDb(
                    string.Concat(prvSequencesWithoutData, ",", prvSequencesWithData),
                    automatedBase.EnvironmentManager.ClientUserName);

                _providerAction = ClickOnExportHistoryIcon(_providerSearch,  label, prvSequencesWithData);

                try
                {
                    var fileName = _providerAction.GetFileName();
                    fileName.Split('.')[0].Trim().Split('(')[0].Trim().ShouldBeEqual(fileNameFromDb,
                        "File name should contain <Provider Name>_<ProvSeq>_<Specialty>_12Months");
                    ExcelReader.ReadExcelSheetValue(fileName, sheetname, 0, 0, out var headerList,
                        out var excelExportList, out _);

                    StringFormatter.PrintMessage("verify and header values");

                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");

                    StringFormatter.PrintMessage("Verify data values");

                    excelExportList[0].ShouldCollectionBeEqual(expectedDataList[0], headerList,
                        "Data should match");
                    excelExportList[1].ShouldCollectionBeEqual(expectedDataListWithProcDesc[0], headerList,
                        "Data should match");
                    _providerAction
                        .IsDocumentAuditActionPresent(prvSequencesWithData,
                            automatedBase.EnvironmentManager.ClientUserName)
                        .ShouldBeTrue("Is Audit Action present?");
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    ClickOnExportHistoryIcon(_providerSearch,  label, prvSequencesWithoutData);
                    _providerAction.GetWindowHandlesCount().ShouldBeEqual(2, "New Blank tab should open");
                    _providerAction.CloseAnyTabIfExist();
                    _providerAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue());
                }

                finally
                {
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileNameFromDb);
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    _providerSearch.DeleteDocumentAuditRecordFromDb(
                        string.Concat(prvSequencesWithoutData, ",", prvSequencesWithData),
                        automatedBase.EnvironmentManager.ClientUserName);

                }
            }
        }

        [Test] //CAR-3055([CAR-2961])

        public void Verify_Export_3_years_History_to_Excel()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Client12MonthsClaimHistoryExportedExcelFileHeaders")
                    .Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var prvSequencesWithData = parameterList["PrvSequenceWithData"];
                var label = parameterList["Label"];
                var sheetname = parameterList["SheetName"];
                var expectedDataList =
                    _providerSearch.GetExport3YearsHistoryExcelData(prvSequencesWithData);
                var fileNameFromDb =
                    _providerSearch.GetExport3YearsHistoryExcelFileNameFromDatabase(prvSequencesWithData) + "_3Years";
                
                _providerAction = ClickOnExportHistoryIcon(_providerSearch,  label, prvSequencesWithData, false);
                
                try
                {
                    automatedBase.CurrentPage.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Message should be shown to the users");
                    automatedBase.CurrentPage.GetPageErrorMessage()
                        .ShouldBeEqual("This export contains PHI. Do you wish to continue?");

                    StringFormatter.PrintMessage("Verify clicking cancel warning is closed");
                    automatedBase.CurrentPage.ClickOkCancelOnConfirmationModal(false);
                    automatedBase.CurrentPage.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Message should not be shown to the users");

                    automatedBase.CurrentPage.NavigateToProviderSearch();
                    ClickOnExportHistoryIcon(_providerSearch,  label, prvSequencesWithData, false);
                    automatedBase.CurrentPage.ClickOkCancelOnConfirmationModal(true);

                    var fileName = _providerAction.GetFileName();
                    fileName.Split('.')[0].Trim().Split('(')[0].Trim().ShouldBeEqual(fileNameFromDb,
                        "File name should contain <Provider Name>_<ProvSeq>_<Specialty>_3Years");
                    ExcelReader.ReadExcelSheetValue(fileName, sheetname, 0, 0, out var headerList,
                        out var excelExportList, out _);

                    StringFormatter.PrintMessage("verify and header values");

                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");

                    StringFormatter.PrintMessage("Verify data values");
                    for (int i = 0; i < excelExportList.Count; i++)
                    {
                        excelExportList[i].ShouldCollectionBeEquivalent(expectedDataList[i],
                            "Data should match");
                    }

                    _providerAction
                        .IsDocumentAuditActionPresent(prvSequencesWithData,
                            automatedBase.EnvironmentManager.ClientUserName)
                        .ShouldBeTrue("Is Audit Action present?");
                }

                finally
                {
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileNameFromDb);
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    _providerSearch.DeleteDocumentAuditRecordFromDb(prvSequencesWithData,
                        automatedBase.EnvironmentManager.ClientUserName);

                }
            }
        }

        //suspect providers work list 
        [Test, Category("Working")] //CAR-39(CAR-484) story already reviewed awating bux fix
        public void Verify_Suspect_Provider_Work_List_Menu_Option()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                SuspectProvidersPage _suspectProviders;

                _suspectProviders = automatedBase.CurrentPage.NavigateToSuspectProviders();
                var providerSeqList = _suspectProviders.GetGridViewSection.GetGridListValueByCol(3);
                automatedBase.CurrentPage = automatedBase.QuickLaunch =
                    automatedBase.CurrentPage.ClickOnQuickLaunch();
                automatedBase.CurrentPage =
                    _providerAction = automatedBase.CurrentPage.NavigateToSuspectProvidersWorkList();

                for (var i = 0; i < 5; i++)
                {
                    _providerAction.GetProviderSequence()
                        .ShouldBeEqual(providerSeqList[i], "Provider Sequence Should Match");
                    _providerAction.ClickOnNextOnProviderAction();
                }

                _suspectProviders = _providerAction.ClickOnSearchIconAtHeaderReturnToSuspectProvidersPage();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEqual(providerSeqList, "Is List Retain?");
            }
        }

        //[Test, Category("NewProviderActionClient1")] //US67260
        public void Verify_provider_action_popup_when_click_on_points_in_provider_results_chart()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ProviderSequence", "Value");
            //_providerSearch.SelectAllProviders();
            //_providerSearch.SearchByProviderSequence(prvSeq);
            //var newProviderAction = _providerSearch.ClickOnChartPointToOpenNewProviderActionPopup();
            //newProviderAction.GetProviderSequence().ShouldBeEqual(prvSeq, "Does Correct Provider Action popup open?");
            //newProviderAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue(),
            //    "Does Provider Action popup display properly");
            //newProviderAction.CloseNewProviderActionPopupAndSwitchToProviderSearchPage();
        }

        [Test, Category("NewProviderActionClient1")] //US51986
        public void Verify_behaviour_cancel_action_on_decision_rationale()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("SBRD");
                _providerAction.SelectReasonCode("BEW4 - BEW Test 4");
                if (_providerAction.IsPageErrorPopupModalPresent())
                    _providerAction.ClickOkCancelOnConfirmationModal(true);
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Warning Message");
                _providerAction.GetConfirmationMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?");
                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when click on cancel on popup window");
                _providerAction.ClickOkCancelOnConfirmationModal(false);
                _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                    .ShouldBeEqual("BEW4 - BEW Test 4", "Reason Code should retain");
                _providerAction.InsertDecisionRationale("UI Test Note");
                _providerAction.ClearReasonCode();
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confirmation Warning Message");
                _providerAction.GetConfirmationMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?");
                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when clicked on cancel on popup window");
                _providerAction.ClickOkCancelOnConfirmationModal(false);
                _providerAction.GetDecisionRationaleText()
                    .ShouldBeEqual("UI Test Note", "Decision Rationale should retain");
                _providerAction.ClickOnCancelActionCondition();
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("SBRD");
                _providerAction.SelectAction("No Action");
                _providerAction.ClearReasonCode();
                if (_providerAction.IsPageErrorPopupModalPresent())
                    _providerAction.ClickOkCancelOnConfirmationModal(true);
                _providerAction.InsertDecisionRationale("");
                if (_providerAction.IsPageErrorPopupModalPresent())
                    _providerAction.ClickOkCancelOnConfirmationModal(true);
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confirmation Warning Message");
                _providerAction.GetConfirmationMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?");
                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when click on cancel on popup window");
                _providerAction.ClickOkCancelOnConfirmationModal(false);
                _providerAction.GetActionCondtionInputFieldValue("Action")
                    .ShouldBeEqual("No Action", "Action should retain");
                _providerAction.SelectAction("Deny");
                _providerAction.SelectAction("No Action");
                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when clicked on ok on popup window");
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.ClickOkCancelOnConfirmationModal(true);
                string[] expectedComponentTitlesInActionPage =
                {
                    "Provider Details", "Provider Conditions",
                    "Condition Notes"
                };
                _providerAction.GetComponentTitlesFromPage()
                    .ShouldCollectionBeEqual(expectedComponentTitlesInActionPage,
                        "Provider Action returned to default state");
                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when action that condition after cancelled");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction.IsRetriggerPeriodSelected()
                    .ShouldBeFalse("Retrigger value should be cleared and deselected");
                _providerAction.IsActionSelectedInComboBox()
                    .ShouldBeFalse("Previous Selected Action should not be selected");
                _providerAction.GetReasonCodeDropDrownValue()
                    .ShouldBeEqual("Select Reason Code", "Previously Selected Reason Code should not be selected");
                _providerAction.GetDecisionRationaleText()
                    .ShouldBeNullorEmpty("Previously set Decision Rationale Test should not be displayed");
                _providerAction.ClickOnCancelActionCondition();
                
            }
        }


        [Test, Category("NewProviderActionClient1")]//US48448
        public void Verify_scrollbar_should_not_display_in_provider_action_exposure_section()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.IsVerticalScrollBarPresentInProviderExposureSection()
                    .ShouldBeFalse("Scroll Bar should not displayed");
            }
        }

        [Test, Category("NewProviderActionClient1")]//US51991
        public void Verify_lengthy_message_with_list_of_selected_code_of_concern_in_Manage_Codes_of_Concern_column_for_a_condition_that_has_already_been_actioned()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("AAGG");
                _providerAction.GetConditionDescription()
                    .ShouldBeEqual(
                        "The list of procedure codes associated with this condition is too lengthy to display.");
                _providerAction.ClickOnSelectedConditionByCode("AAGG");
                _providerAction.SelectProviderConditionByConditionId("SCAC");
                _providerAction.GetAllCodesofConcernListCount()
                    .ShouldBeLessOrEqual(100, "Codes will be listed here for having 100 or less codes of cncern");
                _providerAction.GetColorOfSelectedCodeOfConcern()
                    .AssertIsContained("rgba(49, 0, 111, 1)", "Blue Color of Selected Code of Concern");
                _providerAction.ClickOnCancelActionCondition();
            }
        }


      
        [Test, Category("NewProviderActionClient1"),Category("Acceptance")] //TE-532 + CAR-3244 (CAR-3315)
        public void Verify_that_client_users_see_same_data_points_as_Cotiviti_user_in_three_sections()

        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                string provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");

                _providerSearch.Logout().LoginAsHciAdminUser().NavigateToProviderSearch();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;
                String basicProviderDetailsData;
                String providerExposureData;
                String providerDetailsData;
                List<String> basicProviderDetailsDataFromDatabase;

                try
                {
                    basicProviderDetailsData = _providerAction.GetBasicProviderDetailsData();
                    providerExposureData = _providerAction.GetProviderExposureData();
                    providerDetailsData = _providerAction.GetProviderDetailsData();
                    basicProviderDetailsDataFromDatabase = _providerAction.GetBasicProviderInformation(provSeq);
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();
                }

                _providerSearch.Logout().LoginAsClientUser().NavigateToProviderSearch();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                _providerAction.GetBasicProviderInformationLabels()
                    .ShouldCollectionBeEqual(paramLists["Labels"].Split(',').ToList(), "Lists Should Match");
                _providerAction.GetBasicProviderInformationValues()
                    .ShouldCollectionBeEqual(basicProviderDetailsDataFromDatabase,
                        "Data Should Match With That From Database");

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    try
                    {
                        _providerAction.GetBasicProviderDetailsData().AssertIsContained(basicProviderDetailsData,
                            "Basic Provider Details match");
                        _providerAction.GetProviderExposureData()
                            .ShouldBeEqual(providerExposureData, "Provider Exposure Data match");
                        _providerAction.GetProviderDetailsData()
                            .AssertIsContained(providerDetailsData, "Provider Details Data match");
                    }
                    finally
                    {
                        automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();
                    }
                }
            }
        }

        [Test, Category("NewProviderActionClient1")]
        public void Verify_that_client_users_can_see_proper_Provider_Conditions()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                string provSeq = testData["ProviderSequence"];

                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                _providerAction.IsQuickNoActionPresent().ShouldBeFalse("Quick No Action is present");

                _providerAction.SelectFilterConditions(1);
                var expectedConditionsRequireAction = new List<string>(new String[] {"Required"});
                _providerAction.GetConditionsClientAction()
                    .ShouldCollectionBeEqual(expectedConditionsRequireAction,
                        "Only conditions that require client action present");

                _providerAction.SelectFilterConditions(2);
                var expectedConditionsNotRequireAction = new List<string>(new String[] {"Review"});
                _providerAction.GetConditionsClientAction()
                    .ShouldCollectionBeEqual(expectedConditionsNotRequireAction,
                        "Only conditions that do not require client action present");

                _providerAction.SelectFilterConditions(3);
                var expectedConditionsAll = new List<string>(new String[] {"Review", "Required"});
                _providerAction.GetConditionsClientAction()
                    .ShouldCollectionBeEqual(expectedConditionsAll, "All conditions ");
            }
        }

        [Test, Category("NewProviderActionClient1"), Category("SmokeTestDeployment")] //TANT-89
        public void Verify_that_client_users_can_see_proper_Condition_Details_audit_trail()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var actionDate = testData["ActionDate"];
                var action = testData["Action"];
                var userType = testData["User Type"];
                var actionedBy = testData["Actioned By"];

                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3); //Select 'All Conditions'
                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.SelectProviderConditionByConditionId("SUSC");
                _providerAction.IsProviderConditionSelectedActive("SUSC")
                    .ShouldBeTrue("Selected condition outlined in blue");

                var conditionDetailsRow = _providerAction.GetConditionDetailsRow(3);
                conditionDetailsRow.Contains(String.Format("Action Date:\r\n{0}", actionDate))
                    .ShouldBeTrue("Action Date is present");
                conditionDetailsRow.Contains(String.Format("Action:\r\n{0}", action))
                    .ShouldBeTrue("Action is present");
                conditionDetailsRow.Contains(String.Format("User Type:\r\n{0}", userType))
                    .ShouldBeTrue("User Type is present");
                conditionDetailsRow.Contains(String.Format("Actioned By:\r\n{0}", actionedBy))
                    .ShouldBeTrue("Actioned By is present");

                var firstRowDate =
                    Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(1));
                var secondRowDate =
                    Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(2));
                (firstRowDate > secondRowDate).ShouldBeTrue("Records are in descending order of Action Date");
            }
        }

        [Test, Category("NewProviderActionClient1")]
        public void Verify_message_modal_and_exclamation_icon_appear_disappear_when_provider_flagging_is_checked_unchecked()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> param = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var proSeq = param["ProviderSequence"];
                var providerFlag = param["IsProviderFlagEnabled"];
                var claimSeq = param["ClaimSequence"];
                var providerFlaggingText = param["ProviderFlaggingText"];

                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.SearchByProviderSequence(proSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(proSeq);

                try
                {
                    if (providerFlag == "Y")
                    {
                        if (_providerAction.IsExclamationProfileIconPresent())
                        {
                            _providerAction.ClickOnCheckBoxProviderFlagging();
                            _providerAction.ClickOkCancelOnConfirmationModal(true);
                        }

                        _providerAction.IsCheckBoxPresent().ShouldBeTrue("Checkbox is present ");
                        _providerAction.GetProviderFlaggingLabel()
                            .ShouldBeEqual(providerFlaggingText, "Enable Provider FLagging Client Specific Text");
                        _providerAction.ClickOnCheckBoxProviderFlagging();

                        _providerAction.IsConfirmationPopupModalPresent().ShouldBeTrue("Modal is present.");

                        _providerAction.GetConfirmationMessage()
                            .ShouldBeEqual(
                                "You are issuing a " + providerFlaggingText + " on this provider. Do you wish to save?",
                                "Confirmation Update Message");
                        _providerAction.ClickOkCancelOnConfirmationModal(false); //cancel button clicked
                        _providerAction.IsConfirmationPopupModalPresent()
                            .ShouldBeFalse("Cancel button Clicked, Modal is not present.");
                        _providerAction.ClickOnCheckBoxProviderFlagging();
                        _providerAction.ClickOkCancelOnConfirmationModal(true); //ok button clicked
                        //_providerAction.IsExclamationProfileIconPresent()
                        //    .ShouldBeTrue("Exclamation Profile Icon is Present in Provider Action Page");
                        //ProviderProfilePage providerprofile =
                        //    _providerAction.ClickOnWidgetProviderIconToOpenProviderProfile();
                        //providerprofile.IsExClamationIconPresent()
                        //    .ShouldBeTrue("Exclamation Profile Icon is Present in Provider Profile Page");
                        //_providerAction = providerprofile.CloseProviderProfileAndSwitchToProviderAction();
                        _providerSearch = _providerAction.NavigateToProviderSearch();
                        _providerSearch.IsExclamationPresentInProviderProfile()
                            .ShouldBeTrue("Exclamation Profile Icon is Present in Provider Seach Page");
                        /*ProviderProfilePage providerProfile =
                         _providerSearch.ClickOnProviderIconInGridToOpenProviderProfile(1);
                     providerProfile.IsExClamationIconPresent()
                         .ShouldBeTrue("Exclamation Profile Icon is Present in Provider Profile Page");
                     providerProfile.CloseProviderProfileAndSwitchToProviderSearch();*/

                        ClaimSearchPage claimSearchPage = automatedBase.QuickLaunch.NavigateToClaimSearch();
                        ClaimActionPage newClaimAction =
                            claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                        newClaimAction.IsExClamationIconPresent()
                            .ShouldBeTrue("Exclamation Icon is Present in Claim Action Page");

                        automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();
                        _providerSearch.SearchByProviderSequence(proSeq);

                        _providerAction =
                            _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(proSeq);
                        automatedBase.CurrentPage = _providerAction;
                        _providerAction.ClickOnCheckBoxProviderFlagging();

                        _providerAction.GetConfirmationMessage()
                            .ShouldBeEqual(
                                "You are removing the " + providerFlaggingText +
                                " on this provider. Do you wish to save?", "Confirmation Update Message");
                        _providerAction.ClickOkCancelOnConfirmationModal(false);
                        _providerAction.IsConfirmationPopupModalPresent()
                            .ShouldBeFalse("Cancel button clicked, Modal is not  present.");
                        _providerAction.ClickOnCheckBoxProviderFlagging();
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                        //_providerAction.IsExclamationProfileIconPresent()
                        //    .ShouldBeFalse("Exclamation Profile Icon is no longer present in Provider Action Page");
                        //providerprofile = _providerAction.ClickOnWidgetProviderIconToOpenProviderProfile();
                        //providerprofile.IsExClamationIconPresent()
                        //    .ShouldBeFalse("Exclamation Profile Icon is Present in Provider Profile Page");
                        //_providerAction = providerprofile.CloseProviderProfileAndSwitchToProviderAction();
                        _providerSearch = _providerAction.NavigateToProviderSearch();
                        automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();
                        _providerSearch.IsExclamationPresentInProviderProfile()
                            .ShouldBeFalse("Exclamation Profile Icon is no longer present in Provider Seach Page");


                        claimSearchPage = automatedBase.QuickLaunch.NavigateToClaimSearch();
                        newClaimAction = claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                        newClaimAction.IsExClamationIconPresent()
                            .ShouldBeFalse("Exclamation Icon is no longer present in Claim Action Page");
                    }
                    else
                    {
                        Console.WriteLine("Provider Flaggging is Disabled");
                    }
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();

                    _providerSearch.SearchByProviderSequence(proSeq);
                    _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(proSeq);
                    automatedBase.CurrentPage = _providerAction;
                    if (_providerAction.IsCheckBoxPresent() &&
                        _providerAction.IsCheckBoxAdjecentToProviderFlaggingChecked())
                    {
                        _providerAction.ClickOnCheckBoxProviderFlagging();
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();
                }
            }
        }



        [Test, Category("NewProviderActionClient1")]
        public void Viewing_codes_of_concern_and_edit_codes_of_concern()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                Console.WriteLine("Provider Search Page Loaded : Classinit Override method");
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                //var action = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Action", "Value");
                var reTriggerPeriod = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ReTriggerPeriod", "Value");
                var decisionRationaleNote = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "DecisionRationaleNote", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ReasonCode", "Value");


                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                // _providerAction = _providerSearch.ClickOnProviderNameToOpenNewProviderActionForProvSeq(provSeq);


                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.IsManageCodesofConcernColumnPresent()
                    .ShouldBeTrue("Manage Codes of Concern Column present");
                _providerAction.SelectProviderConditionByConditionId("SSWP");
                _providerAction.IsSelectAllCodesofConcernCheckBoxPresent()
                    .ShouldBeTrue("Select all codes of concern check box present");
                _providerAction.IsCodeofConcernSorted().ShouldBeTrue("Codes of concern sorted");
                _providerAction.IsCheckBoxForEachCodeofConcernPresent()
                    .ShouldBeTrue("Check box for individual codes of concern present.");

                _providerAction.IsSelectAllCodesofConcernCheckBoxChecked()
                    .ShouldBeTrue("Select All checkbox is selected by default in Manage Codes of Concern");
                _providerAction.ToggleSelectAllCodesofConcern()
                    .ShouldBeTrue("Toggle select all cofdes of concern successful");
                _providerAction.IsIndividualCodesofConcernSelected()
                    .ShouldBeTrue("Individual Codes of concern selected.");
                _providerAction.ToggleCodesofConcernCheckBox()
                    .ShouldBeTrue("Toggle of individual code of concern successful");

                List<bool> allCodesofConcernCheckBoxValuesBeforeSave =
                    _providerAction.GetAllCodesofConcernCheckBoxValues();
                List<string> allCodeofConcernBeforeSave = _providerAction.GetAllCodesofConcernList();
                _providerAction.ClickOnSelectAllCheckBoxManageCodeofConcern();
                var currAction = _providerAction.GetProviderConditionDetailForFieldAndRow("Client Action", 2);
                var action = currAction.Equals("Review") ? "Deny" : "Review";
                _providerAction.SelectAction(action);
                _providerAction.SelectReasonCode(reasonCode);
                //_providerAction.SelectReTriggerPeriod(reTriggerPeriod);
                _providerAction.InsertDecisionRationale(decisionRationaleNote);
                _providerAction.ClickOnSaveActionCondition();

                _providerAction.IsValidationNoticeModalPopupPresent()
                    .ShouldBeTrue("Page Error Modal popup present ?");
                _providerAction.GetValidationNoticePopupMessage()
                    .ShouldBeEqual("All conditions must have at least one associated code of concern selected.");
                _providerAction.CloseValidationNoticePopup();
                _providerAction.ClickOnSelectAllCheckBoxManageCodeofConcern();
                _providerAction.SaveActionConditions();
                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("SSWP");

                List<bool> allCodesofConcernCheckBoxValueAfterSave =
                    _providerAction.GetAllCodesofConcernCheckBoxValues();
                List<string> allCodeofConcernAfterSave = _providerAction.GetAllCodesofConcernList();

                allCodesofConcernCheckBoxValuesBeforeSave.SequenceEqual(allCodesofConcernCheckBoxValueAfterSave)
                    .ShouldBeTrue(
                        "After Modified Conditions, Checkbox Value of All Code of Concern Before modified Equals to After");
                allCodeofConcernBeforeSave.SequenceEqual(allCodeofConcernAfterSave).ShouldBeTrue(
                    "After Modified Conditions, Checkbox Value of All Code of Concern Before modified Equals to After");

                _providerAction.SelectProviderConditionByConditionId("SCMC");
                _providerAction.GetConditionDescription()
                    .ShouldBeEqual("See referral note for codes associated with this condition.");
                _providerAction.ClickOnSelectedConditionByCode("SCMC");

                _providerAction.SelectProviderConditionByConditionId("SUSC");
                _providerAction.ClickOnActionAllConditionsCheckBox();

                _providerAction.IsTriggerDatePresent().ShouldBeTrue("Trigger date is present");
                _providerAction.IsConditionTitlePresent().ShouldBeTrue("Condition Title is present");
                _providerAction.IsConditionDescriptionPresent().ShouldBeTrue("Condition description is present");
                _providerAction.IsScrollBarPresentinManageCodeOfConcern()
                    .ShouldBeTrue("Vertical Scrollbar is present");
                _providerAction.ClickOnActionAllConditionsCheckBox();
                _providerAction.SelectProviderConditionByConditionId("A30D");

                //_providerAction.GetCodeofConcernDisplayedColumn().ShouldEqual(5, "Code is displayed in five columns");
                _providerAction.IsScrollBarPresentinManageCodeOfConcern()
                    .ShouldBeFalse("Vertical Scrollbar is present");
                _providerAction.GetConditionDescription()
                    .ShouldBeEqual(
                        "The list of procedure codes associated with this condition is too lengthy to display.");
                _providerAction.ClickOnSelectedConditionByCode("A30D");
                
            }
        }

        //[Test]
        public void Add_Note_icon_will_display_if_no_Notes_and_two_state_Notes_icon_is_present_if_atleast_one_notes_in_container_view_and_header_section()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string[] proSeq = new string[2];
            proSeq[0] = param["ProviderSequenceWithoutNote"];
            proSeq[1] = param["ProviderSequenceWithNote"];

            ProviderNotesPage providerNotesPage = null;

            for (int i = 0; i < proSeq.Count(); i++)
            {
                _providerSearch.SearchByProviderSequence(proSeq[i]);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(proSeq[i]);

                automatedBase.CurrentPage = _providerAction;

                //click on  note adjacent to provider flagging 
                providerNotesPage = _providerAction.ClickOnSmallProviderNotes();
                int rowCount = providerNotesPage.GetGridRecordCountBySubTyped("Alert");
                if (rowCount > 0)
                {
                    var firstRowDate = Convert.ToDateTime(providerNotesPage.GetCreatedDateOnGrid("Alert", 1));
                    var secondRowDate = Convert.ToDateTime(providerNotesPage.GetCreatedDateOnGrid("Alert", 2));
                    (firstRowDate >= secondRowDate).ShouldBeTrue(
                        "Records with the most recent note record displayed at the top");
                    _providerAction.ClosePopupNoteAndSwitchToNewProviderActionPage();
                    _providerAction.IsSmallViewNoteIconPresent()
                        .ShouldBeTrue("Notes Icon Present");
                }
                else
                {
                    providerNotesPage.IsCreateNotesSectionDisplayed()
                        .ShouldBeTrue("Create Notes Section Displayed");
                    providerNotesPage.GetTypeValueFromNotesPage()
                        .ShouldBeEqual("Provider", "Provider is Present in  Type Field ");
                    providerNotesPage.GetSubTypeValueFromNotesPage()
                        .ShouldBeEqual("Alert", "Alert is Present in Sub Type Field ");
                    _providerAction.ClosePopupNoteAndSwitchToNewProviderActionPage();
                    _providerAction.IsSmallAddNoteIconPresent().ShouldBeTrue("Add Notes Icon Present");
                }

                //click on header note 
                providerNotesPage = _providerAction.ClickOnHeaderProviderNotes();
                rowCount = providerNotesPage.GetGridRecordCountBySubTyped("Alert");
                if (rowCount > 0)
                {
                    var firstRowDate = Convert.ToDateTime(providerNotesPage.GetCreatedDateOnGrid("Alert", 1));
                    var secondRowDate = Convert.ToDateTime(providerNotesPage.GetCreatedDateOnGrid("Alert", 2));
                    (firstRowDate >= secondRowDate).ShouldBeTrue(
                        "Records with the most recent note record displayed at the top");
                    _providerAction.ClosePopupNoteAndSwitchToNewProviderActionPage();
                    _providerAction.IsViewNoteIconPresent()
                        .ShouldBeTrue("Notes Icon Present");
                }
                else
                {
                    providerNotesPage.IsCreateNotesSectionDisplayed()
                        .ShouldBeTrue("Create Notes Section Displayed");
                    providerNotesPage.GetTypeValueFromNotesPage()
                        .ShouldBeEqual("Provider", "Provider is Present in  Type Field ");
                    providerNotesPage.GetSubTypeValueFromNotesPage()
                        .ShouldBeNullorEmpty("Sub Type Field should null for header note icon");
                    _providerAction.ClosePopupNoteAndSwitchToNewProviderActionPage();
                    _providerAction.IsAddNoteIconPresent().ShouldBeTrue("Add Notes Icon Present");
                }
                }
            }

        }



        [Test, Category("Working")]
        public void Verify_that_client_user_during_action_conditioning_process_works_properly()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                StringFormatter.PrintMessageTitle("New Provider Action");
                var conditionIdClientActionDictionary = new Dictionary<string, string>();
                _providerAction.SelectFilterConditions(3);
                var numProviderConditions = _providerAction.GetProviderConditionsCount();
                var clientActionList = _providerAction.GetConditionsClientAction();
                numProviderConditions.ShouldBeGreater(0, "Provider Condtions count should greater than zero");
                for (int i = 1; i <= numProviderConditions; i++)
                {
                    var conditionId =
                        _providerAction.GetConditionIdFromProviderConditions(i);

                    conditionIdClientActionDictionary.Add(conditionId, clientActionList[i - 1]);
                }

                _providerAction.ClickOnEditActionCondition();

                StringFormatter.PrintMessageTitle("Action Condition Components");
                string[] expectedComponentTitlesInPage =
                {
                    "Provider Details", "Select Conditions for Actioning", "Manage Codes of Concern",
                    "Action Conditions"
                };
                _providerAction.GetComponentActionProviderConditionCount()
                    .ShouldBeEqual(3, "Three column container view is displayed");
                _providerAction.GetComponentTitlesFromPage().ShouldCollectionBeEqual(expectedComponentTitlesInPage,
                    "All required componenets are shown");

                StringFormatter.PrintMessageTitle("Selecting Conditions");
                _providerAction.SelectCodeOfConcernForActioning(1);
                var firstCodeId = _providerAction.GetConditionIdFromLeftColumn(1);
                _providerAction.GetSelectedConditionsCount()
                    .ShouldBeEqual(1, "Conditions displayed in the 'Selected Conditions' section");
                _providerAction.ClickOnSelectedConditionByCode(firstCodeId);
                _providerAction.GetSelectedConditionsCount()
                    .ShouldBeEqual(0, "Conditions displayed in the 'Selected Conditinos' section");

                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction.IsActionSelectedInComboBox().ShouldBeFalse("Action selected by default");
                _providerAction.IsReasonCodeSelectedInComboBox().ShouldBeFalse("Reason code seleced by default");

                StringFormatter.PrintMessageTitle("Action Conditions");
                _providerAction.SelectAction("No Action");
                var retriggerTimePeriodList = _providerAction.GetRetriggerTimePeriodList();
                string[] expectedRetriggerTimePeriodList = {"1-89 Days", "90 Days", "6 Months", "1 Year"};
                retriggerTimePeriodList.ShouldCollectionBeEqual(expectedRetriggerTimePeriodList,
                    "Retrigger Time Periods displayed correctly.");

                _providerAction.ClickOnActionAllConditionsCheckBox();

                string[] actionList = {"No Action", "Review", "Deny", "Monitor", "Remove Review"};
                string[] invalidActionsForNoActionList = {"Review", "Deny", "Monitor", "Remove Review"};
                string[] invalidActionsForRemoveReviewList = {"No Action", "Required"};
                foreach (var selectAction in actionList)
                {
                    _providerAction.SelectAction(selectAction);
                    for (int i = 1; i <= numProviderConditions; i++)
                    {
                        var conditionIdFromLeftColumn = _providerAction.GetConditionIdFromLeftColumn(i);
                        var clientActionForCondition = conditionIdClientActionDictionary[conditionIdFromLeftColumn];
                        var savedConditionText =
                            _providerAction.GetTextFromSelectedConditionsUsingConditionId(
                                conditionIdFromLeftColumn);
                        var actionIsInvalid = IsActionInvalid(savedConditionText);

                        //action is always invalid if it is same as the previous action, and some more cases for "No Action" and "Remove Review"
                        if ((selectAction == clientActionForCondition) ||
                            ((selectAction == "No Action") &&
                             invalidActionsForNoActionList.Contains(clientActionForCondition)) ||
                            ((selectAction == "Remove Review") &&
                             invalidActionsForRemoveReviewList.Contains(clientActionForCondition)))
                        {
                            actionIsInvalid.ShouldBeTrue(
                                String.Format(
                                    "Previous Action: {0}, Current Action: {1}, The selected action is not valid for this condition",
                                    clientActionForCondition, selectAction));
                        }
                        else
                        {
                            actionIsInvalid.ShouldBeFalse(
                                String.Format(
                                    "Previous Action: {0}, Current Action: {1}, The selected action is not valid for this condition",
                                    clientActionForCondition, selectAction));
                        }
                    }
                }

                _providerAction.ClickOnCancelActionCondition();
                StringFormatter.PrintMessageTitle("Back to default state");
                string[] expectedComponentTitlesInActionPage =
                    {"Provider Details", "Provider Conditions", "Condition Exposure"};
                _providerAction.GetComponentTitlesFromPage().ShouldCollectionBeEqual(
                    expectedComponentTitlesInActionPage, "Provider Action returned to default state");
                
            }
        }

        [Test, Category("NewProviderActionClient2")]
        public void Verify_that_proper_options_and_validation_message_are_shown_in_action_conditioning()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var provSeq = testData["ProviderSequence"];
                _providerSearch
                    .SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                var expectedResponseCodeList = _providerAction.GetExpectedResonCodeList(false);
                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();

                _providerAction.SelectCodeOfConcernForActioning(1);

                StringFormatter.PrintMessageTitle("Validation Message");
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.IsValidationNoticeModalPopupPresent()
                    .ShouldBeTrue("Validation Notice Modal Popup is shown");
                _providerAction.GetValidationNoticePopupMessage()
                    .AssertIsContained("You have not selected an action.",
                        "Proper message is shown when Action is not selected.");
                _providerAction.CloseValidationNoticePopup();

                _providerAction.SelectAction("Review");
                _providerAction.ClearReasonCode();
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.IsValidationNoticeModalPopupPresent()
                    .ShouldBeTrue("Validation Notice Modal Popup is shown");
                _providerAction.GetValidationNoticePopupMessage()
                    .AssertIsContained("You have not selected a reason code.",
                        "Proper message is shown when Reason Code is not selected.");
                _providerAction.CloseValidationNoticePopup();

                _providerAction.SelectFirstReasonCode();
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.IsValidationNoticeModalPopupPresent()
                    .ShouldBeTrue("Validation Notice Modal Popup is shown");
                _providerAction.GetValidationNoticePopupMessage()
                    .AssertIsContained("A Note is required before the record can be saved",
                        "Proper message is shown when Note is not provided.");
                _providerAction.CloseValidationNoticePopup();

                StringFormatter.PrintMessageTitle("Reason Codes");

                expectedResponseCodeList.Insert(0, "");
                IList<string> responseCodeList = _providerAction.GetReasonCodeOptions();
                responseCodeList.ShouldCollectionBeEqual(expectedResponseCodeList,
                    "All are Fraud specific Reason Codes:");

                StringFormatter.PrintMessageTitle("Decision Rationale Box");
                string[] expectedDecisionRationaleFormattingOptions =
                {
                    "Bold", "Italic", "Underline", "Strike Through", "Align Left", "Center",
                    "Insert/Remove Bulleted List", "Insert/Remove Numbered List"
                };
                var decisionRationaleFormattingOptions = _providerAction.GetDecisionRationaleFormattingOptions();
                decisionRationaleFormattingOptions.ShouldCollectionBeEqual(
                    expectedDecisionRationaleFormattingOptions,
                    "All required formatting options shown in Decision Rationale");
                _providerAction.IsVisibleToClientCheckBoxShown()
                    .ShouldBeFalse("Visible to Client checkbox is shown");

                _providerAction.InsertDecisionRationale("uitest");
                _providerAction.GetDecisionRationaleText()
                    .ShouldBeEqual("uitest", "Entered text present in 'Decision Rationale' text box");
                _providerAction.ClickOnCancelActionCondition();
            }
        }

        [Test, Category("NewProviderActionClient2")] //US39871
        public void Verify_that_client_users_has_ability_to_select_deselect_user_specified_codes()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                //checking if condition is a type that requires client action
                var expectedConditionsAll = new List<string>(new String[] {"Required"});
                _providerAction.GetConditionsClientAction().ShouldCollectionBeEqual(expectedConditionsAll,
                    "Provider Condition is of type requrijng client action ");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction
                    .ClickOnCodeOfConcernCheckBox(); //by default for this test the checbox is checked so on re checking the checkbox is inactive
                _providerAction.IsCodeOfConcernCheckBoxChecked()
                    .ShouldBeFalse("Code of concern is editable. A checked box is unchecked.");

                _providerAction.ClickOnCodeOfConcernCheckBox();
                _providerAction.IsCodeOfConcernCheckBoxChecked()
                    .ShouldBeTrue("Code of concern is editable. An unchecked box is checked.");
                _providerAction.ClickOnCancelActionCondition();
            }
        }

        [Test, Category("NewProviderActionClient2")] //US45704
        public void Verify_presence_of_stpx_procedure_code_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;
                
                var stpxProcedureCode = _providerAction.GetConditionFromProviderConditions(1);
                VerifyFormatOfStpxCode(stpxProcedureCode)
                    .ShouldBeTrue("Proc desc code correctly formatted as STPX -  Procedure Combination XXXXX.");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction.GetConditionFromEditProvideActionColumns(1, 1)
                    .ShouldBeEqual(stpxProcedureCode,
                        "STPX procedure code is present in Select condition section.");
                _providerAction.GetConditionFromEditProvideActionColumns(2, 1)
                    .ShouldBeEqual(stpxProcedureCode,
                        "STPX procedure code is present in manage Codes of Concern section.");
                _providerAction.GetConditionFromEditProvideActionColumns(3, 1)
                    .ShouldBeEqual(stpxProcedureCode,
                        "STPX procedure code is present in Action condition section.");
            }
        }

        [Test, Category("NewProviderActionClient2")] //US48440
        public void Verify_codes_of_concern_are_displayed_in_condition_details_audit_trail()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var conditionCode = testData["ConditionCode"];
                var reasonCode = testData["ReasonCode"];
                var reasonDescription = testData["ReasonDescription"];
                var customCod = testData["CustomCodes"];

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.SelectProviderConditionByConditionId("PFOT");

                var conditionDetailsRow = _providerAction.GetConditionDetailsRowSelector(1, 1);
                conditionDetailsRow = conditionDetailsRow + _providerAction.GetConditionDetailsRowSelector(1, 2);
                conditionDetailsRow.Contains(String.Format("{0}", conditionCode))
                    .ShouldBeTrue("4 digit condition code is present");
                conditionDetailsRow.Contains(String.Format("Reason:\r\n{0} - {1}", reasonCode, reasonDescription))
                    .ShouldBeTrue("Reason code and description is present");
                var firstRowDate =
                    Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(1));
                var rowCount = _providerAction.GetConditionDetailsRowCount();
                for (var i = 0; i < rowCount; i++)
                {
                    if (i != rowCount - 1)
                    {
                        var secondRowDate =
                            Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(i + 2));
                        (firstRowDate > secondRowDate).ShouldBeTrue(
                            "Records are in descending order of Action Date");
                        firstRowDate = secondRowDate;
                    }

                    _providerAction.GetConditionDetailsRowSelector(i + 1, 2)
                        .Contains("User Type:\r\nCotiviti")
                        .ShouldBeFalse("Condition Details  Actioned by Cotiviti User Should not displayed");

                }

                var masterCodesRow = _providerAction.GetConditionDetailsCodeOfConcernRow(2);
                masterCodesRow.Contains(String.Format("Codes of Concern:\r\n{0}", "Master Codes Applied"))
                    .ShouldBeTrue("checking master codes applied is present");

                _providerAction.SelectProviderConditionByConditionId("SCAC");
                var customCodesList = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                customCodesList.Contains(String.Format("Codes of Concern:\r\n{0}", "Custom Codes Applied"))
                    .ShouldBeTrue("checking Custom codes applied is present");
                customCodesList.Contains("...")
                    .ShouldBeTrue("checking Custom codes exceeding 25 has an ellipsis applied is present");
                string[] codeList =
                    BetweenStringSplitter(customCodesList, "Applied\r\n", "\r\n...")
                        .Split(new string[] {"\r\n"}, StringSplitOptions.None);
                codeList.ShouldCollectionBeSorted(false, "Custom codes are sorted in ascending order");
                _providerAction.SelectProviderConditionByConditionId("SBRD");
                var customCodes = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                customCodes.Replace("\r\n", string.Empty)
                    .Contains(String.Format("Codes of Concern:{0}", customCod))
                    .ShouldBeTrue("checking Custom codes applied is present");
                _providerAction.SelectProviderConditionByConditionId("SUSC");
                var noProc = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                noProc.Contains(String.Format("Codes of Concern:\r\n{0}", testData["NoProcCode"]))
                    .ShouldBeTrue("checking no proc associated message is present");

                _providerAction.SelectProviderConditionByConditionId("SFJT");
                var exceedProc = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                exceedProc.Contains(String.Format("Codes of Concern:\r\n{0}", testData["ExceedProcCode"]))
                    .ShouldBeTrue("checking message for exceeding proc code is present");

                _providerAction.SelectProviderConditionByConditionId("SEMH");
                _providerAction.IsConditionDetailAuditRowPresent()
                    .ShouldBeFalse("Condition  Details Audit Row Should Blank");
                
            }
        }

        [Test, Category("NewProviderActionClient2")]
        public void Verify_decision_rationale_text_box_should_enabled_for_any_action_selected()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);

                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("SUSC");
                var actionList = _providerAction.GetActionOptions();
                foreach (var action in actionList)
                {
                    _providerAction.SelectAction(action);
                    _providerAction.InsertDecisionRationale(string.Format("UINOTE for action-{0}", action));
                    _providerAction.GetDecisionRationaleText()
                        .ShouldBeEqual(string.Format("UINOTE for action-{0}", action),
                            string.Format("Decision Rationale Text Box is editable for action {0}", action));
                }

                _providerAction.ClickOnCancelActionCondition();
            }
        }
        [Test, Category("NewProviderActionClient2")]//US50626
        public void Verify_clear_on_codes_of_concern_does_not_return_error()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var conditionCode = testData["ProviderConditionCode"];

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                if (!_providerAction.IsUserAddedConditionSectionPresent())
                    _providerAction.ClickOnSearchCondition();
                _providerAction.SearchConditionByCodeAndClickOnFirstResult(beginCode: conditionCode,
                    isClient: true);
                _providerAction.ClickOnClearButtonInUserSpecifiedCondition();
                _providerAction.GetValueofInputConditionCode()
                    .ShouldBeNullorEmpty("Clicking on Clear button clears search box contents");
                _providerAction.GetValueofInputConditionCode(false)
                    .ShouldBeNullorEmpty("Clicking on Clear button clears search box contents");
                _providerAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("Clearing user added condition should not generate page error.");
            }
        }

        [Test, Category("NewProviderActionClient1")] //US50624
        public void Verify_default_reason_code_selected_based_on_selected_action()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var reasonCodes = testData["Reasoncodes"].Split(';');

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();

                _providerAction.SelectCodeOfConcernForActioning(1);
                var actions = _providerAction.GetActionOptions();
                var i = 0;
                foreach (var action in actions)
                {
                    StringFormatter.PrintMessageTitle(string.Format("Verfiying behavour for action type {0}",
                        action));

                    _providerAction.SelectAction(action);
                    if (_providerAction.IsConfirmationPopupModalPresent())
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                    _providerAction.IsReasonCodeSelectedInComboBox().ShouldBeTrue("Reason code seleced by default");
                    _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                        .ShouldBeEqual(reasonCodes[i],
                            string.Format("Reason code field default to a pre determined value for action {0} ",
                                action));
                    Console.Write(
                        "User can change reason code despite auto selection of default value. Also checking if reason code is required.");
                    _providerAction.ClearReasonCode();
                    if (_providerAction.IsConfirmationPopupModalPresent())
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                    _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                        .ShouldBeEqual(" ", "Reason code filed value cleared and empty");
                    _providerAction.ClickOnSaveActionWaitForPopup();
                    _providerAction.IsValidationNoticeModalPopupPresent()
                        .ShouldBeTrue("Validation Notice Modal Popup is shown");
                    _providerAction.GetValidationNoticePopupMessage()
                        .AssertIsContained("You have not selected a reason code.",
                            "Proper message is shown when Reason Code is not selected.");
                    _providerAction.CloseValidationNoticePopup();

                    _providerAction.SelectFirstReasonCode();
                    if (_providerAction.IsConfirmationPopupModalPresent())
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                    _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                        .ShouldNotBeEqual(" ", "Reason code field can be changed.");
                    i++;
                }
            }
        }

        [Test, Category("NewProviderActionClient2")]//US50623 
        public void Verify_that_fraud_specific_reason_codes_are_sorted()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                IList<string> expectedResponseCodeList = _providerAction.GetExpectedResonCodeList(false);
                expectedResponseCodeList.Insert(0, "");
                IList<string> responseCodeList = _providerAction.GetReasonCodeOptions();
                responseCodeList.ShouldCollectionBeEqual(expectedResponseCodeList,
                    "All are Fraud specific Reason Codes:");
                responseCodeList.ShouldCollectionBeSorted(false,
                    "Fraud reason codes are displayed in an alphabetical order.");
            }
        }

        [Test, Category("NewProviderActionClient2")]//US51987
        public void Verify_decision_rationale_change_behaviour_when_action_changes()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var actions = testData["Actions"].Split(',');
                var reasonCode = testData["Reasoncodes"];
                var decisionRationale = testData["DecisionRationale"].Split(';');

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();

                _providerAction.SelectCodeOfConcernForActioning(1);
                for (int r = 0; r < actions.Count(); r++)
                {
                    if (!_providerAction.GetActionCondtionInputFieldValue("Action").Equals("No Action") || r == 0)
                    {
                        _providerAction.SelectActionOnly("No Action");
                        _providerAction.IsReasonCodeSelectedInComboBox()
                            .ShouldBeTrue("Reason code seleced by default");
                        _providerAction.GetDecisionRationaleText().ShouldBeEqual(reasonCode,
                            "Default reason code is copied to Decison rationale for no action.");

                    }

                    _providerAction.GetActionCondtionInputFieldValue("Reason Code").ShouldBeEqual(reasonCode,
                        "Reason code field default to a pre determined value " + reasonCode);


                    Console.Write("Verfiying behaviour of rationale when action type changes to {0}", actions[r]);
                    _providerAction.SelectActionOnly(actions[r]);
                    var reasonCodeForcurrecnt = _providerAction.GetActionCondtionInputFieldValue("Reason Code");
                    _providerAction.IsConfirmationPopupModalPresent()
                        .ShouldBeFalse("No warning when action changes from no action to " + actions[r]);
                    _providerAction.GetDecisionRationaleText()
                        .ShouldNotBeEqual(reasonCode, "Decisional rationale of no action cleared");
                    _providerAction.GetDecisionRationaleText()
                        .ShouldBeNullorEmpty("Decision rationale  empty when action selected to :" + actions[r]);
                    _providerAction.InsertDecisionRationale(decisionRationale[1]);
                    _providerAction.SelectActionOnly("No Action");

                    _providerAction.IsConfirmationPopupModalPresent().ShouldBeTrue("Warning is present.");
                    _providerAction.GetConfirmationMessage()
                        .ShouldBeEqual(
                            "Rationale text will be discarded.\r\nDo you wish to continue?",
                            "Confirmation on Message discard");
                    _providerAction.ClickOkCancelOnConfirmationModal(false); //cancel button clicked
                    _providerAction.GetActionCondtionInputFieldValue("Action").ShouldBeEqual(actions[r],
                        "No changes, action reverted to previously selected option");
                    _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                        .ShouldBeEqual(reasonCodeForcurrecnt, "Reason code field hasnot changed  ");
                    _providerAction.GetDecisionRationaleText().ShouldBeEqual(decisionRationale[1],
                        "Decision rationale  hasnt changed");
                    Console.Write(
                        "Verifying when action are switched between review, deny, monitor, and remove review no warning is displayed.");

                    foreach (var action in actions)
                    {
                        if (action != actions[r])
                        {
                            _providerAction.SelectActionOnly(action);
                            _providerAction.IsConfirmationPopupModalPresent()
                                .ShouldBeFalse(" Warning is not present.");
                            _providerAction.GetActionCondtionInputFieldValue("Action")
                                .ShouldBeEqual(action, "Action selected as " + action);
                            _providerAction.GetDecisionRationaleText().ShouldBeEqual(decisionRationale[1],
                                "Decision rationale  hasnt changed");
                        }
                    }

                    _providerAction.SelectActionOnly(actions[r]);

                    _providerAction.SelectActionOnly("No Action");
                    _providerAction.IsConfirmationPopupModalPresent().ShouldBeTrue("Warning is present.");

                    _providerAction.GetConfirmationMessage()
                        .ShouldBeEqual(
                            "Rationale text will be discarded.\r\nDo you wish to continue?",
                            "Confirmation on Message discard");
                    _providerAction.ClickOkCancelOnConfirmationModal(true); //confirm button clicked
                    _providerAction.GetActionCondtionInputFieldValue("Reason Code").ShouldBeEqual(reasonCode,
                        "Reason code field has changed to no action default");
                    _providerAction.GetDecisionRationaleText().ShouldBeEqual(reasonCode,
                        "Decision rationale  has changed to no action reason code");
                    _providerAction.GetActionCondtionInputFieldValue("Action")
                        .ShouldBeEqual("No Action", "Action selected confirmed to No action");

                }
            }
        }


        [Test, Category("NewProviderActionClient2")]//US51988
        public void Verify_presence_of_icon_for_condition_requiring_action()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];

                var cAction = testData["cAction"];

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                _providerAction.SelectFilterConditions(3);
                for (var i = 1; i <= _providerAction.GetProviderConditionsCount(); i++)
                {
                    var actionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(cAction, i);
                    if (actionSelected.Equals("Required"))
                        _providerAction.IsProviderConditionRowHaveActionRequiredBadge(i).ShouldBeTrue(
                            "Action required badge icon present should be true for action: " + actionSelected);
                    else
                        _providerAction.IsProviderConditionRowHaveActionRequiredBadge(i).ShouldBeFalse(
                            "Action required badge icon present should be false for action: " + actionSelected);

                }

                automatedBase.CurrentPage = automatedBase.QuickLaunch.NavigateToProviderSearch();

            }
        }

        [Test, Category("NewProviderActionClient2")] //US39872+US51989 added new story 
        public void Verify_that_condition_description_popup_should_be_in_proper_format_with_display_correct_description()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var provSeq = testData["ProviderSequence"];
                var conditionDescription = testData["ConditionDescription"];

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                string originalHandle = string.Empty;
                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionConditionCode(1);
                try
                {
                    var conditionId = _providerAction.GetConditionIdFromProviderConditions(1).Split('-')[0];
                    _providerAction.SwitchToPopUpWindow();
                    _providerAction.GetProviderConditionIdPopupContent().ShouldBeEqual("Condition: " + conditionId,
                        "Header Information is Correct");
                    _providerAction.IsLabelBoldInProviderConditionPopup()
                        .ShouldBeTrue("Condition Label Should be in Bold");
                    _providerAction.IsLabelBoldInProviderConditionPopup(3)
                        .ShouldBeTrue("Condition Description Label Should be in Bold");
                    _providerAction.GetProviderConditionDescription().ShouldBeEqual(
                        "Condition Description:\r\n" + conditionDescription, "Condition Desctiption is accurate");
                }
                finally
                {
                    _providerAction.CloseAnyPopupIfExist(originalHandle);
                }
            }
        }


        [Test, Category("NewProviderActionClient1")] //US57578
        public void Verify_proc_codes_associated_with_SUSC_is_listed_under_condition_details()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var prvSeq = testData["ProviderSequence"];
                var procedureCode = testData["ProcedureCode"];
                var conditionId = testData["ConditionId"];
                var date = testData["Date"];

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForClient(prvSeq, date);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.SelectFilterConditions(3);
                var currAction = _providerAction.GetProviderConditionDetailForFieldAndRow("Client Action", 1);
                var newAction = currAction.Equals("Review") ? "Deny" : "Review";
                StringFormatter.PrintMessageTitle(
                    "Verify All associated Procedure codes are listed under Code of Concern in condition details section");
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.ClickOnConditionDetailsIcon();
                var conditionDetailRow = _providerAction.GetConditionDetailsRowCount();
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                ActionCondition(_providerAction, reasonCode: "", action: newAction);
                _providerAction.SelectFilterConditions(3);
                StringFormatter.PrintMessageTitle(
                    "Verify All associated Procedure codes are listed under Code of Concern in condition details section");
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.GetListOfAssociatedProcCodeCodeOfConcernRow(conditionDetailRow + 1)
                    .ShouldCollectionBeEqual(procedureCode.Split(';'), "Associated procedure code has been listed");

                _providerAction.GetConditionDetailsRowCount()
                    .ShouldBeEqual(conditionDetailRow + 1, "Audit Record for the new action has been added");
            }
        }

        [Test, Category("NewProviderActionClient1")]//US68666
        public void Verify_that_Visible_to_client_option_is_not_available_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var prvsSeq = paramLists["ProviderSequence"];
                var userName = paramLists["UserName"];
                _providerSearch.DeleteProviderNotesOnly(prvsSeq, automatedBase.EnvironmentManager.ClientUserName);

                _providerSearch.SearchByProviderSequence(prvsSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvsSeq);
                
                try
                {
                    _providerAction.ClickonProviderNotes();
                    _providerAction.NotePage.IsNoteContainerPresent()
                        .ShouldBeTrue("Note Container must display after clicking note icon.");
                    _providerAction.NotePage.IsVisibleToClientCheckboxPresentInAddNoteForm()
                        .ShouldBeFalse("Visible To Client option not available to client user in Add Note form");
                    _providerAction.NotePage.SelectNoteSubType("Alert");
                    _providerAction.NotePage.SetAddNote("note");
                    _providerAction.NotePage.ClickonAddNoteSaveButton();
                    _providerAction.WaitForWorkingAjaxMessage();
                    _providerAction.NotePage.IsVisibletoClientIconPresentByName(userName)
                        .ShouldBeFalse("Visible to Client icon(Tick mark) must not be present for client user.");
                    _providerAction.NotePage.ClickOnEditIconOnNotesByName(userName);
                    _providerAction.NotePage.IsVisibleToClientCheckboxPresentInNoteEditorByName(userName)
                        .ShouldBeFalse("Visible to Client checkbox not available to client user in Edit Note form. ");
                    _providerAction.NotePage.ClickonAddNoteIcon();
                    _providerAction.NotePage.IsVisibleToClientCheckboxPresentInAddNoteForm()
                        .ShouldBeFalse("Visible To Client option not available to client user in Add Note form");
                    _providerAction.Logout().LoginAsHciAdminUser2();
                    _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                    _providerSearch.SearchByProviderSequence(prvsSeq);
                    automatedBase.CurrentPage = _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvsSeq);
                    _providerAction.ClickonProviderNotes();
                    _providerAction.NotePage.IsVisibletoClientIconPresentByName(userName).ShouldBeTrue(
                        "Visible to Client icon(Tick mark) for note created by client user must be present for internal user, .");
                }
                finally
                {
                    automatedBase.CurrentPage = _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();
                }
            }
        }


        [Test, Category("NewProviderActionClient1")]//US68666
        public void Verify_that_client_user_cannot_view_Notes_with_visible_to_client_false_in_Provider_Action_Page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var prvsSeq = paramLists["ProviderSequence"];
                var internalUser = paramLists["InternalUser"];

                _providerSearch.Logout().LoginAsHciAdminUser2();
                _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                _providerSearch.SearchByProviderSequence(prvsSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvsSeq);

                _providerAction.ClickonProviderNotes();
                if (_providerAction.NotePage.IsVisibletoClientIconPresentByName(internalUser))
                {
                    _providerAction.NotePage.ClickOnEditIconOnNotesByName(internalUser);
                    _providerAction.NotePage
                        .ClickVisibleToClientCheckboxInNoteEditorByName(internalUser); //set visible to client false
                    _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(internalUser);
                    _providerAction.WaitForWorkingAjaxMessage();
                }

                var notesCountForInternalUser = _providerAction.NotePage.GetNoteListCount();

                _providerAction.Logout().LoginAsClientUser();
                _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                _providerSearch.SearchByProviderSequence(prvsSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvsSeq);
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.GetNoteListCount().ShouldBeEqual(notesCountForInternalUser - 1,
                    "Client can view only those notes with visible to client option set to true");
            }
        }

        [Test, Category("NewProviderActionClient1")]//US68651 + US68999
        public void Verify_note_edit_and_view_functionality_provider_action_page_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                var updatedNoteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UpdatedNoteText", "Value");
                var userNameList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "UserNameList", "Value")
                        .Split(';')
                        .ToList();
                var currentUser = userNameList[0];
                var nonCurrentUser = userNameList[1];


                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.ClickonProviderNotes();
                var noteCount = _providerAction.NotePage.GetNoteListCount();

                StringFormatter.PrintMessageTitle("Verification of Edit note");
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(currentUser)
                    .ShouldBeTrue("Notes Edit form displayed");
                _providerAction.NotePage.IsNoteFormEditableByName(currentUser)
                    .ShouldBeTrue("Notes can be edited by current user only ");


                _providerAction.NotePage.SetNoteInNoteEditorByName(string.Empty, currentUser, false); //set empty note
                _providerAction.NotePage.IsEmptyNoteWarning().ShouldBeTrue("Empty Note warning displayed");
                _providerAction.NotePage.GetEmptyNoteTooltipInEditNoteForm()
                    .ShouldBeEqual("Note is required before the record can be saved.", "Empty Note warning tooltip.");
                _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessage("Validation popup displayed");
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Pop Up model is present");
                _providerAction.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
                _providerAction.ClosePageError();

                var noteText = new string('a', 19994);
                _providerAction.NotePage.SetNoteInNoteEditorByName(noteText, currentUser, false);
                _providerAction.GetPageErrorMessage()
                    .ShouldBeEqual("Note value is too long.", "Long note message should equal");
                _providerAction.ClosePageError();
                _providerAction.NotePage.GetNoteInNoteEditorByName(currentUser).Length
                    .ShouldBeEqual(19993, "Note Text max length");

                _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessageTitle(
                    "A scroll bar should be present in the notes container at this point of time but the verification in the notes " +
                    "container is out of scope of Selenium");

                StringFormatter.PrintMessage("Updating Note");
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.SetNoteInNoteEditorByName(updatedNoteText, currentUser, false);
                _providerAction.WaitForStaticTime(1000);
                _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.NotePage.IsEmptyNoteWarning()
                    .ShouldBeFalse("Empty Note warning should remove after enter text in text area.");

                StringFormatter.PrintMessageTitle("Mod Date should show the latest date");
                int totalRow = _providerAction.NotePage.GetNoteListCount();
                _providerAction.NotePage.GetNoteRecordByRowColumn(5, totalRow)
                    .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"), "Modified date in Note list must be updated.");

                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.GetNoteInNoteEditorByName(currentUser)
                    .ShouldBeEqual(updatedNoteText, "Updated Note text displayed in notes form");

                StringFormatter.PrintMessageTitle("Verification of Cancel note");
                _providerAction.NotePage.SetNoteInNoteEditorByName("Cancel Note Text", currentUser, false);
                _providerAction.NotePage.ClickOnCancelButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(currentUser)
                    .ShouldBeFalse("Notes Edit form must be collasped");
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.GetNoteInNoteEditorByName(currentUser).ShouldBeEqual(updatedNoteText,
                    "Notes must not be updated when Cancel button is clicked.");
                _providerAction.NotePage.ClickOnCancelButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();

                StringFormatter.PrintMessageTitle("Verification of Carrot Icon on each note record.");
                _providerAction.NotePage.ClickOnExpandIconOnNotesByName(nonCurrentUser);
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(nonCurrentUser)
                    .ShouldBeTrue("Notes form must be expanded.");
                _providerAction.NotePage.IsNoteFormEditableByName(nonCurrentUser)
                    .ShouldBeFalse("Notes form must not be editable");
                _providerAction.NotePage.ClickOnCollapseIconOnNotesByName(nonCurrentUser);
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(nonCurrentUser)
                    .ShouldBeFalse("Clicking on Expand icon again must close note form.");

                StringFormatter.PrintMessageTitle("User should able to expand multiple forms.");
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.ClickOnExpandIconOnNotesByName(nonCurrentUser);
                _providerAction.NotePage.GetOpenedNoteFormListCount()
                    .ShouldBeEqual(noteCount, "User should able to view multiple/all note");
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser, false);
                _providerAction.NotePage.ClickOnCollapseIconOnNotesByName(nonCurrentUser);
            }
        }

        [Test, Category("NewProviderActionClient1")]//US68677
        public void Verify_note_saved_in_Decision_Rationale_when_condition_is_actioned_the_note_is_visible_in_the_Provider_action_note_section()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                var currentUserFullName = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UserFullName", "Value");
                var noteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "NoteText", "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Date", "Value");
                //var updatedNoteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "UpdatedNoteText", "Value");

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForClient(prvSeq, date);
                _providerSearch.UpdateTriggeredConditionForClient(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction.SelectAction("Review");
                _providerAction.SelectFirstReasonCode();
                _providerAction.InsertDecisionRationale(noteText);
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUserFullName);
                _providerAction.NotePage.GetNoteInNoteEditorByName(currentUserFullName)
                    .ShouldBeEqual(noteText, "Note text should match");
                _providerAction.NotePage.ClickOnCancelButtonInNoteEditorByName(currentUserFullName);
                _providerAction.ClickonProviderNotes();
                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId("SIJF");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                _providerAction.GetNoteInConditionNotesByRow(1).ShouldBeEqual(noteText,
                    "Note created in the generate rationale should be present in the Condition Exposure Note section");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);
            }
        }

        [Test, Category("NewProviderActionClient2")] //US68677 
        public void Verify_users_can_view_condition_rationales_in_Provider_action_at_the_condition_level_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                var subType = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "SubType", "Value");
                var currentUserFullName = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UserFullName", "Value");
                var noteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "NoteText", "Value");
                var updatedNoteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UpdatedNoteText", "Value");
                //var totalProviderConditionNoteCount = Convert.ToInt32(_providerSearch.CountOfConditionNotesAssociatedToTheConditionFromDatabase(prvSeq, subType));

                _providerSearch.DeleteProviderNotesOnly(prvSeq,
                    automatedBase.EnvironmentManager.ClientUserWithReadWriteManageAppeals);


                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.IsConditionExposureButtonSelected()
                    .ShouldBeFalse("Condition exposure Button must be selected for the Default view");

                _providerAction.ClickonProviderNotesinConditionExposureSection();
                _providerAction.GetTooltipOfNotesIconInConditionExposureSection().ShouldBeEqual("View Condition Notes");
                _providerAction.SelectProviderConditionByConditionId(subType);
                _providerAction.GetEmptyNoteMessage().ShouldBeEqual("There are no notes available.",
                    "Note message should be displayed when notes are not availabe");

                _providerAction.ClickonProviderNotes();
                _providerAction.WaitForWorking();
                _providerAction.NotePage.SelectNoteSubType(subType);
                _providerAction.NotePage.SetAddNote(noteText);
                _providerAction.NotePage.ClickonAddNoteSaveButton();
                _providerAction.WaitForWorking();

                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.ClickonProviderNotes();

                _providerAction.WaitForStaticTime(2000);
                StringFormatter.PrintMessage("Click on a Condition to view the notes associated with that condition");
                _providerAction.SelectProviderConditionByConditionId(subType);
                _providerAction.GetConditionNotesAssociatedToTheCondition().ShouldBeEqual
                    (1, "Both count should match");

                StringFormatter.PrintMessageTitle(
                    "Verify the labels upon selecting the condition ID and expand to view the notes");
                _providerAction.IsCaretIconPresentByRowInConditionNotes(1)
                    .ShouldBeTrue("Carrot Icon should be present");
                _providerAction.GetConditionNoteRecordByColumnRow(2)
                    .ShouldBeEqual("Provider", "Correct Note Type should be displayed");
                _providerAction.GetConditionNoteRecordByColumnRow(3)
                    .ShouldBeEqual(subType, "Correct Sub  Type should be displayed");
                _providerAction.GetConditionNoteRecordByColumnRow(4)
                    .ShouldBeEqual(currentUserFullName, "Correct User Name should be displayed.");
                _providerAction.GetConditionNoteRecordByColumnRow(5).ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                    "Correct Date should be displayed");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                _providerAction.GetNoteInConditionNotesByRow(1).ShouldBeEqual(noteText,
                    "Note Text before the note is modified should be as expecteed");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);
                _providerAction.SelectProviderConditionByConditionId("SMID");

                StringFormatter.PrintMessageTitle("Update the Note text");
                _providerAction.ClickonProviderNotes();
                _providerAction.WaitForWorking();
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUserFullName);
                _providerAction.NotePage.SetNoteInNoteEditorByName(updatedNoteText, currentUserFullName, false);
                _providerAction.WaitForStaticTime(1000);
                _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(currentUserFullName);
                _providerAction.ClickonProviderNotes();

                StringFormatter.PrintMessageTitle(
                    "Verify the labels upon selecting the condition ID and expand to view the notes after the note text is updated");
                _providerAction.SelectProviderConditionByConditionId(subType);
                _providerAction.IsCaretIconPresentByRowInConditionNotes(1)
                    .ShouldBeTrue("Carrot Icon should be present");
                _providerAction.GetConditionNoteRecordByColumnRow(2)
                    .ShouldBeEqual("Provider", "Correct Note Type should be displayed");
                _providerAction.GetConditionNoteRecordByColumnRow(3)
                    .ShouldBeEqual(subType, "Correct Sub  Type should be displayed");
                _providerAction.GetConditionNoteRecordByColumnRow(4)
                    .ShouldBeEqual(currentUserFullName, "Correct User Name should be displayed.");
                _providerAction.GetConditionNoteRecordByColumnRow(5).ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                    "Correct Date should be displayed");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                _providerAction.GetNoteInConditionNotesByRow(1)
                    .ShouldBeEqual(updatedNoteText, "Note Text should be modified");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);

            }
        }

        [Test, Category("NewProviderActionClient2")]//US69533 + TE-379 + TANT-189
        public void Verify_open_scout_case_functionality()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                ClientSearchPage _clientSearch;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Date", "Value");
                var noteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "NoteText", "Value");
                var conditionId = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ConditionId", "Value");
                var action = "Review";
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForClient(prvSeq, date);
                _providerSearch.DeletePRVQueueData(conditionId, prvSeq, ClientEnum.SMTST.ToString());
                _providerSearch.UpdateProviderActionAndOpenCtiCase(prvSeq);
                automatedBase.CurrentPage =
                    _clientSearch = _providerSearch.Logout().LoginAsHciAdminUser().NavigateToClientSearch();
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Configuration.GetStringValue());
                _clientSearch.ClickOnRadioButtonByLabel(ConfigurationSettingsEnum.ScoutCaseTracker.GetStringValue());
                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                _clientSearch.IsRadioButtonOnOffByLabel(ConfigurationSettingsEnum.ScoutCaseTracker.GetStringValue())
                    .ShouldBeTrue("Is Scout Case Tracker selected?");
                _providerSearch.GetJobFlagValueFromDatabase(ClientEnum.SMTST.ToString()).ShouldBeEqual("T",
                    "CTI_JOB_FLAG column in HCIUSER.HCICLIENT_NUCLEUS table is set True.");
                _providerSearch = _clientSearch.Logout().LoginAsClientUser().NavigateToProviderSearch();
                _providerSearch.SearchByProviderSequence(prvSeq);

                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.SelectFilterConditions(3);

                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.IsOpenScoutCaseSelected()
                    .ShouldBeFalse("Open Scout Case checkbox should not be selected by default");

                var actionList = _providerAction.GetActionOptions();
                foreach (var _action in actionList)
                {
                    _providerAction.SelectAction(_action);
                    if (_action == "Review")
                    {
                        _providerAction.IsOpenScoutCaseEnabled().ShouldBeTrue(
                            string.Format("Open Scout Case checkbox should be enabled when {0} option selected",
                                _action));
                    }
                    else
                    {
                        _providerAction.IsOpenScoutCaseDisabled().ShouldBeTrue(
                            string.Format("Open Scout Case checkbox should be disabled when {0} option selected",
                                _action));
                    }
                }

                _providerAction.SelectAction(action);

                _providerAction.SelectOpenScoutCase();
                _providerAction.IsOpenScoutCaseSelected().ShouldBeTrue("Is Open Scout Case checkbox selected?");
                _providerAction.SelectFirstReasonCode();
                _providerAction.InsertDecisionRationale(noteText);

                StringFormatter.PrintMessage("Cancel Action Verification  for Scout Case Tracker");
                _providerAction.ClickOnCancelActionCondition();
                _providerAction.GetRealTimeProviderQueueCount(prvSeq, conditionId).ShouldBeEqual(0,
                    "A new record must not be created in the real time provider table when cancel button is clicked ");

                StringFormatter.PrintMessage(
                    "apply Review action on Provider when Scout case tracker not selected");
                ActionProvider(_providerAction, conditionId, action, noteText, false);
                _providerAction.GetRealTimeProviderQueueCount(prvSeq, conditionId).ShouldBeEqual(0,
                    "A new record must not be created in the real time provider table. ");
                _providerAction.IsOpenCTICaseSetTrueInDatabase(prvSeq)
                    .ShouldBeFalse("Is Open CTI Case column set 'T'?");

                StringFormatter.PrintMessage("Apply Deny action on Provider ");
                ActionProvider(_providerAction, conditionId, "Deny", noteText, false);
                _providerAction.SelectFilterConditions(3);
                _providerAction.GetProviderConditionDetailForFieldAndRow("Client Action", 1)
                    .ShouldBeEqual("Deny", "Client Action is successful");
                _providerAction.GetRealTimeProviderQueueCount(prvSeq, conditionId).ShouldBeEqual(0,
                    "A new record must not be created in the real time provider table. ");
                _providerAction.IsOpenCTICaseSetTrueInDatabase(prvSeq)
                    .ShouldBeFalse("Is Open CTI Case column set 'T'");

                StringFormatter.PrintMessage(
                    "Apply Review action on Provider when Open Scout case tracker selected ");
                ActionProvider(_providerAction, conditionId, action, noteText, true);
                _providerAction.SelectFilterConditions(3);
                _providerAction.GetProviderConditionDetailForFieldAndRow("Client Action", 1)
                    .ShouldBeEqual(action, "Client Action is successful");
                _providerAction.GetRealTimeProviderQueueCount(prvSeq, conditionId).ShouldBeEqual(1,
                    "A new record must be created in the real time provider table. ");
                _providerAction.IsOpenCTICaseSetTrueInDatabase(prvSeq)
                    .ShouldBeTrue("Is Open CTI Case column set 'T'?");


                StringFormatter.PrintMessageTitle("Verify Scout Option unchecked ");
                automatedBase.CurrentPage = _clientSearch =
                    _providerSearch.Logout().LoginAsHciAdminUser().NavigateToClientSearch();
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Configuration.GetStringValue());
                _clientSearch.ClickOnRadioButtonByLabel(ConfigurationSettingsEnum.ScoutCaseTracker.GetStringValue(),
                    false);
                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                _clientSearch.IsRadioButtonOnOffByLabel(ConfigurationSettingsEnum.ScoutCaseTracker.GetStringValue())
                    .ShouldBeFalse("Is Scout Case Tracker selected?");
                _providerSearch.GetJobFlagValueFromDatabase(ClientEnum.SMTST.ToString()).ShouldBeEqual("F",
                    "CTI_JOB_FLAG column in HCIUSER.HCICLIENT_NUCLEUS table is set True.");
                _providerSearch = _clientSearch.Logout().LoginAsClientUser().NavigateToProviderSearch();

                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.SelectFilterConditions(3);

                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.IsOpenScountCaseVisible()
                    .ShouldBeFalse("Is Open Scout Case Visible if Client Setting is unchecked?");
                _providerAction.ClickOnCancelActionCondition();
            }
        }

        [Test, Category("NewProviderActionClient1")] //CAR-711
        public void Verify_that_no_action_note_from_client_action_is_visbile_to_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var prvSeq = testData["ProviderSequence"];
                var reason = testData["Note"];
                var action = testData["Action"];
                var date = testData["Date"];
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForClient(prvSeq, date);
                _providerSearch.UpdateTriggeredConditionForClient(prvSeq);
                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                automatedBase.CurrentPage = _providerAction;
                
                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectCodeOfConcernForActioning(1);
                _providerAction.SelectAction(action);
                _providerAction.InsertDecisionRationale(reason);
                var utcTime = DateTime.UtcNow;
                var serverZone = TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
                var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone).ToString("MM/dd/yyyy");
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId("SANW");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);

                _providerAction.IsNoteInConditionNotesPresentByDate(currentDateTime)
                    .ShouldBeTrue("Notes saved from no action is visible to client should be true");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);
                _providerAction.GetVisibileToForProvSeqAndDate(prvSeq, date).ShouldBeEqual("1",
                    "Visible to client should be equal to 1, meaning both client and internal user can see the note");
                
            }
        }

        [Test, Category("SmokeTestDeployment"),Category("Working")] //TANT-89
        public void Verify_quadrant_1_of_provider_action_page_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 1"

                StringFormatter.PrintMessageTitle(
                    "Verifying the Provider Scorecard Popup from Provider Action page");
                var providerScoreCard = _providerAction.ClickOnProviderScorecard();
                providerScoreCard.GetProviderScorecardPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.ProviderScoreCard.GetStringValue());
                providerScoreCard.CloseProviderScoreCardAndReturnToProviderAction(PageTitleEnum.ProviderAction);

                //StringFormatter.PrintMessageTitle("Verifying Provider Profile popup when Provider Profile Icon is clicked");
                //var providerProfile = _providerAction.ClickOnWidgetProviderIconToOpenProviderProfile();
                //providerProfile.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderProfile.GetStringValue());
                //providerProfile.CloseProviderProfileAndSwitchToProviderAction();

                /*StringFormatter.PrintMessageTitle("Verifying Provider Popup pops up when 'Provider under Review' icon is clicked");
                providerProfile = _providerAction.ClickOnProviderReviewIconToOpenProviderProfile();
                providerProfile.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderProfile.GetStringValue());
                providerProfile.CloseProviderProfileAndSwitchToProviderAction();*/

                StringFormatter.PrintMessageTitle("Verifying clicking 'Manage Notes icon' presents the Notes form");
                _providerAction.ClickNoteIcon();
                _providerAction.IsAddNoteFormSectionPresent()
                    .ShouldBeTrue("Add Note Section is present after clicking the 'View or Manager Notes' icon");
                _providerAction.ClickNoteIcon();
                _providerAction.IsAddNoteFormSectionPresent().ShouldBeFalse("Add Note Section toggles off");

                StringFormatter.PrintMessageTitle("Verifying 'Claim History pops up when 'Hx' icon is clicked'");
                var providerClaimHistory = _providerAction.ClickProviderClaimHistoryToOpenProviderHistory(3);
                providerClaimHistory.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimHistory.GetStringValue());
                automatedBase.CurrentPage.CloseAnyPopupIfExist();

                StringFormatter.PrintMessageTitle("Verifying Provider Exposure widget presents the data");
                _providerAction.ProviderExposureDataItemsCountPresent()
                    .ShouldBeGreater(0, "Provider Exposure Section should show the data");

                StringFormatter.PrintMessageTitle(
                    "Verification of Provider Search icon: When selected, Provider Search page will be presented");
                _providerSearch = _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                _providerSearch.GetPageHeader().ShouldBeEqual("Provider Search");

                #endregion
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_google_maps_of_provider_action_page_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);
                _providerAction.WaitForStaticTime(5000);

                #region "Quadrant 2"

                StringFormatter.PrintMessage(
                    "Verifying Address row: Select Address hyperlink, will present Google maps with the address highlighted");
                _providerAction.ClickAddressHyperlinkInProviderDetailSection();
                _providerAction.WaitForGoogleMapToLoad();
                _providerAction.IsGoogleMapPopupPresent()
                    .ShouldBeTrue("The Google Map popup is present showing the address", true);
                _providerAction.CloseGoogleMapPopup();

                #endregion
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_2_of_provider_action_page_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 2"


                StringFormatter.PrintMessageTitle("Verify Top 10 Proc Codes icon");
                _providerAction.ClickOnTop10ProcCodes(1);
                _providerAction.GetTop10ProcCodeCount()
                    .ShouldBeGreater(0, "Data is being displayed for 'Top 10 Proc Codes By Count'");
                _providerAction.ClickOnTop10ProcCodes(2);
                _providerAction.GetTop10ProcCodeCount()
                    .ShouldBeGreater(0, "Data is being displayed for 'Top 10 Proc Codes By Billed'");
                _providerAction.ClickOnTop10ProcCodes(3);
                _providerAction.GetTop10ProcCodeCount()
                    .ShouldBeGreater(0, "Data is being displayed for 'Top 10 Proc Codes By Paid'");

                StringFormatter.PrintMessageTitle("Verifying Provider Details Icon");
                _providerAction.ClickOnProviderDetailsIcon();
                _providerAction.IsProviderDetailsSectionPresent()
                    .ShouldBeTrue("Provider Details section is expanded with additional data points");



                StringFormatter.PrintMessage(
                    "Verifying whether Provider Details rows are expanded for data rows having Red Badge");
                _providerAction.ClickOnProviderDetailByRow(1);
                _providerAction.IsProviderDetailExpandedSectionPresent(1)
                    .ShouldBeTrue("'Specialty' row should expand since it has a Red Badge");

                _providerAction.ClickOnProviderDetailByRow(2);
                _providerAction.IsProviderDetailExpandedSectionPresent(2)
                    .ShouldBeTrue("'Address' row should expand since it has a Red Badge");

                _providerAction.ClickOnProviderDetailByRow(3);
                _providerAction.IsProviderDetailExpandedSectionPresent(3)
                    .ShouldBeTrue("'Name' row should expand since it has a Red Badge");

                _providerAction.ClickOnProviderDetailByRow(4);
                _providerAction.IsProviderDetailExpandedSectionPresent(4)
                    .ShouldBeTrue("'TIN' row should expand since it has a Red Badge");

                /*StringFormatter.PrintMessage("Verifying CIU Referral: Will be listed if there are associated CIU records");
                _providerAction.GetCIUReferralRecordRowCount().ShouldBeGreater(0, "Associated CIU Referrals are being shown under the 'CIU Referral' section");
                _providerAction.ClickOnAddCIUReferralRecordCssSelector();
                _providerAction.IsCreateCIUReferralSectionDisplayed().ShouldBeTrue("Add CIU Referral form is being displayed");*/

                #endregion
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_3_of_provider_action_page_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 3"

                /*StringFormatter.PrintMessageTitle("Verification of 'Quick No Action All Conditions' icon");
                var providerCondition = _providerAction.GetConditionFromProviderConditions(1);
                _providerAction.ClickOnQuickNoActionXIcon();
                _providerAction.GetActionCondtionInputFieldValue("Action").ShouldBeEqual("No Action");
                _providerAction.GetConditionFromEditProvideActionColumns(1, 1).ShouldBeEqual(providerCondition);
                _providerAction.ClickOnCancelActionCondition();*/

                StringFormatter.PrintMessageTitle(
                    "Verification of Triggered Conditions options under More Options");
                int totalProviderConditions = 0;

                for (int i = 1; i < 3; i++)
                {
                    _providerAction.SelectFilterConditions(i);
                    totalProviderConditions += _providerAction.GetProviderConditionsCount();
                }

                _providerAction.SelectFilterConditions(3);
                _providerAction.GetProviderConditionsCount().ShouldBeEqual(totalProviderConditions);

                StringFormatter.PrintMessageTitle(
                    "Verifying When 'Action Condition' icon is  selected, Provider Action page is presented with ALL Conditions requiring action displayed in the 1st column of the page");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.GetProviderConditionsCount().ShouldBeEqual(totalProviderConditions);
                _providerAction.SelectFirstProviderConditionClientAction();
                _providerAction.ClickOnCancelActionCondition();

                /*StringFormatter.PrintMessageTitle("When 'Quick No Action icon' is clicked, will set the result to No Action for the specific condition requiring review");
                var listOfProviderConditions = _providerAction.GetConditionIdListInProviderConditions();
                _providerAction.ClickOnQuickNoActionAllConditions();
                _providerAction.GetConditionIdListInProviderConditions().ShouldCollectionBeEqual(listOfProviderConditions, "All condition that going to be taken No Action should match.");
                _providerAction.ClickOnCancelActionCondition();*/

                StringFormatter.PrintMessageTitle("Verification of 'Next' button in Provider Action page");
                Verify_Next_Button_In_Provider_Action(_providerSearch);

                #endregion

                # region LOCAL METHOD

                void Verify_Next_Button_In_Provider_Action(ProviderSearchPage providerSearch)
                {
                    providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();
                    _providerAction = providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
                    _providerAction.ClickOnNextOnProviderAction();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual("Provider Search");
                    providerSearch.ClickOnClearLink();
                    providerSearch.SelectDropDownValueByLabel("Quick Search", ProviderEnum.SuspectProviders.GetStringValue());
                    providerSearch.ClickOnFindButton();

                    var listOfProviderSeqFromSearchResult = providerSearch.GetSearchResultListByCol(3);
                    providerSearch.ClickOnProvSeqByRowCol(1, 3);

                    for (int i = 0; i < 3; i++)
                    {
                        _providerAction.ClickOnNextOnProviderAction();
                        _providerAction.GetProviderSequence().ShouldBeEqual(listOfProviderSeqFromSearchResult[i + 1]);
                    }
                }
                #endregion
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_4_of_provider_action_page_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var providerSequenceWithConditionInformation =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                        "ProviderSequence", "Value");

                #region "Quadrant 4"

                    StringFormatter.PrintMessageTitle(
                        "Verification of 'Condition Notes', 'Condition Exposure' and 'Condition Details' icons");
                    _providerSearch.SearchByProviderSequence(providerSequenceWithConditionInformation);
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(
                            providerSequenceWithConditionInformation);

                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickonProviderNotesinConditionExposureSection();
                    _providerAction.GetConditionNotesAssociatedToTheCondition()
                        .ShouldBeGreater(0, "Condition Notes are present for the selected condition");

                    _providerAction.ClickOnConditionExposure();
                    _providerAction.IsConditionExposureDetailsSectionPresent()
                        .ShouldBeTrue("Condition Exposure section is present");

                    #endregion
            }
        }

        [Test] //TE-636
        public void Verify_Action_Condition_Form_And_Plus_Icon()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var prvSeq = testData["ProviderSequence"];
                var condition = testData["Condition"];
                var code = testData["Code"];

                _providerAction = _providerSearch.NavigateToProviderAction(() =>
                {
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                    _providerSearch.WaitForPageToLoad();
                });

                _providerAction.ClickOnEditActionCondition();
                StringFormatter.PrintMessageTitle("Verify Action Conditions Form Is In Disabled State");
                _providerAction.IsActionConditionsFormDisabled(true)
                    .ShouldBeTrue("Action Condition Form Should Be Disabled By Default");
                _providerAction.SelectFirstProviderConditionClientAction();
                _providerAction.IsActionConditionsFormDisabled(true)
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());
                
                StringFormatter.PrintMessageTitle("Verify Plus Icon In Manage Codes Of Concern");
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeFalse("User Specified Condition Form Should Not Be Present");
                _providerAction.GetToolTipForPlusIconInSecondColumn().ShouldBeEqual("Create User Specified Condition");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeTrue("User Specified Condition Form Should Be Present");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeFalse("User Specified Condition Form Should Be Collapsed on second selection");

                StringFormatter.PrintMessageTitle(
                    "Verify Action Condition Form Is Enabled When User Specified Condition Is Selected");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.SelectConditionFromDropdownList(condition);
                _providerAction.ClickOnFlagAllCodes();
                _providerAction.ClickAddButtonInUserSpecifiedCondition();
                _providerAction.IsActionConditionsFormDisabled(true)
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());

                _providerAction.ClickOnFlagAllCodes();
                _providerAction.SetBeginCodeAndEndCode(code, isTabRequired: true);
                _providerAction.ClickAddButtonInUserSpecifiedCondition();
                _providerAction.IsActionConditionsFormDisabled(true)
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());
                _providerAction.ClickOnPlusIconInSecondColumn();

                StringFormatter.PrintMessageTitle("Verify Clicking Cancel Will Navigate User Out Of 3 Column View");
                _providerAction.IsThreeColumnViewPresent()
                    .ShouldBeTrue("User Should Be In 3 Column View Prior To Clicking Cancel");
                _providerAction.ClickOnCancelActionCondition();
                _providerAction.IsThreeColumnViewPresent()
                    .ShouldBeFalse("User Should Be Navigated Out Of 3 Column View to provider action page");

                StringFormatter.PrintMessage(
                    "Verify Clicking On Small Edit icon in condition row, Action Condition Form Is Enabled");
                _providerAction.ClickOnFirstConditionEditIcon();
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Quick edit icon Is Selected");
                _providerAction.ClickOnCancelActionCondition();
            }
        }

        [Test] //TE-817
        public void Verify_Existing_Text_Is_Overwritten_In_Decision_Rationale_For_Newly_Selected_Reason_Code()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderActionPage _providerAction;
                var _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var prvSeq = testData["ProviderSequence"];
                var defaultReasonCode = testData["DefaultReasonCode"];
                var updatedReasonCode = testData["UpdatedReasonCode"];

                _providerAction = _providerSearch.NavigateToProviderAction(() =>
                {
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                    _providerSearch.WaitForPageToLoad();
                });

                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectFirstProviderConditionClientAction();
                _providerAction.SelectAction("No Action");
                _providerAction.GetDecisionRationaleText().ShouldBeEqual(defaultReasonCode);
                _providerAction.SelectReasonCode(updatedReasonCode);
                _providerAction.IsConfirmationPopupModalPresent()
                    .ShouldBeTrue("Confirmation Popup modal should be present");
                _providerAction.ClickOkCancelOnConfirmationModal(true);
                _providerAction.GetDecisionRationaleText().ShouldBeEqual(updatedReasonCode,
                    "Decision Rationale Text Should Be Updated With Selected Reason Code");

            }
        }


        #endregion

        #region PRIVATE METHODS

        private void ActionCondition(ProviderActionPage _providerAction, string reasonCode = "", string action = "Deny",
            string note = "UITEST", List<string> procCodeList = null, bool save = true)
        {
            _providerAction.SelectAction(action);
            _providerAction.SelectReasonCode(reasonCode == "" ? "BEW4 - BEW Test 4" : reasonCode);
            if (action != "No Action")
                _providerAction.InsertDecisionRationale(note);
            if (procCodeList != null)
                for (int i = 1; i < procCodeList.Count; i++)
                {
                    _providerAction.ClickOnCodeOfConcernByCode(procCodeList[i]);
                }
            if (save)
            {
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.WaitForWorkingAjaxMessage();
            }
        }
        private bool IsActionInvalid(string conditionText)
        {
            return conditionText.Contains("The selected action is not valid for this condition");
        }

        private bool VerifyFormatOfStpxCode(string stpxCode)
        {
            if (stpxCode.StartsWith("STPX - Procedure Combination "))
            {
                stpxCode = stpxCode.Substring(stpxCode.LastIndexOf(" ") + 1);
                return Regex.IsMatch(stpxCode, @"^[a-zA-Z0-9\s,]*$");
            }
            return false;
        }

        /// <summary>
        /// Returns splitted output string by splitting throught consecutive string delimiters
        /// Returns a string array that contains the substrings in this string that are delimited by elements of a firstDelimiter string array and seconDelimiter string array.
        /// </summary>
        /// <param name="inputString">string to be splitted for desired output</param>
        /// <param name="firstDelimiter">first array of strings that delimit the substrings in input string</param>
        /// <param name="secondDelimiter">second array of strings that delimit the substrings in output of first split string</param>
        /// <returns></returns>
        private string BetweenStringSplitter(string inputString, string firstDelimiter, string secondDelimiter)
        {
            var outputString = inputString.Split(new[] { firstDelimiter }, StringSplitOptions.None)[1];
            outputString = outputString.Split(new[] { secondDelimiter }, StringSplitOptions.None)[0];
            return outputString;
        }
        private void ActionProvider(ProviderActionPage _providerAction, string conditionId, string action,
            string noteText, bool selectScout)
        {
            _providerAction.SelectFilterConditions(3);
            _providerAction.ClickOnEditActionCondition();
            _providerAction.SelectProviderConditionByConditionId(conditionId);
            _providerAction.SelectAction(action);
            _providerAction.SelectFirstReasonCode();
            _providerAction.InsertDecisionRationale(noteText);
            SiteDriver.WaitToLoadNew(200);
            if (selectScout)
            {
                _providerAction.SelectOpenScoutCase();
            }

            _providerAction.ClickOnSaveActionCondition();
            _providerAction.WaitForWorkingAjaxMessage();
        }
        
        public ProviderActionPage ClickOnExportHistoryIcon(ProviderSearchPage _providerSearch, string label, string prvseq, bool twelve_months = true)
        {
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, prvseq);
            _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerSearch.WaitForPageToLoadWithSideBarPanel();
            var _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvseq);
            _providerAction.ClickOnProviderClaimHistoryIcon();
            if (twelve_months)
                _providerAction.ClickOnExport12MonthsClaimHistory();
            else
                _providerAction.ClickOnExport3YearsClaimHistory();

            return _providerAction;
        }

        private void Generate_Rationale(ProviderActionPage _providerAction, string[] requiredFields,
            string[] inputValue, string beginCode, string endCode, string condition)
        {
            _providerAction.SelectConditionFromDropdownList(condition);
            _providerAction.SetBeginCodeAndEndCode(beginCode, endCode);
            _providerAction.ClickAddButtonInUserSpecifiedCondition();
            _providerAction.SelectAction("Review");
            _providerAction.SelectFirstReasonCode();
            _providerAction.ClickOnGenerateRationaleButton();
            for (int i = 0; i < requiredFields.Count(); i++)
            {
                _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
            }

            _providerAction.ClickOnFinishButton();
        }

        private void Verify_if_result_set_exceeds_100_codes_warning_message_will_be_displayed(ProviderActionPage _providerAction, string begincode, string endcode)
        {
            _providerAction.SelectConditionFromDropdownList("SUSC");
            _providerAction.SetBeginCodeAndEndCode(begincode, endcode);
            _providerAction.ClickAddButtonInUserSpecifiedCondition();
            _providerAction.IsMactchingConditionsPresent().ShouldBeFalse("Is Matching Conditions present?");
            _providerAction.IsResultSetContainerPresentForRangeOfCodes().ShouldBeTrue("Resut set container should be present");
            _providerAction.GetResultSetProcCodeCount().ShouldBeLessOrEqual(100, "Result set should be less than  100");
            _providerAction.IsCheckboxPresentNextToTheCodeInUserSpecifiedCondition().ShouldBeTrue("Checkbox should be present next to the code");
            _providerAction.CountOfCheckboxNextToTheCode().ShouldBeEqual(_providerAction.GetResultSetProcCodeCount());

            while (_providerAction.GetResultSetProcCodeCount() < 100)
            {
                endcode = (int.Parse(endcode) + 1).ToString();
                _providerAction.SetEndCode(endcode);
                _providerAction.ClickAddButtonInUserSpecifiedCondition();
                _providerAction.GetResultSetProcCodeCount().ShouldBeLessOrEqual(100, "Result set should be less than 100");
                _providerAction.IsCheckboxPresentNextToTheCodeInUserSpecifiedCondition()
                    .ShouldBeTrue("Checkbox should be present next to the code");
                _providerAction.CountOfCheckboxNextToTheCode()
                    .ShouldBeEqual(_providerAction.GetResultSetProcCodeCount());
            }

            _providerAction.GetResultSetProcCodeCount().ShouldBeEqual(100, "Result set should be equal to 100");
            endcode = (int.Parse(endcode) + 1).ToString();
            _providerAction.SetEndCode(endcode);
            _providerAction.ClickAddButtonInUserSpecifiedCondition();
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Error message should be displayed");
            _providerAction.GetPageErrorMessage().ShouldBeEqual("The result set found exceeds 100 codes. Please enter new search criteria.");
            _providerAction.CountOfCheckboxNextToTheCode().ShouldBeEqual(_providerAction.GetResultSetProcCodeCount());
            _providerAction.ClosePageError();


        }

        private void Set_action_conditions(ProviderActionPage _providerAction, bool popUp = true)
        {
            _providerAction.SelectAction("Review");
            _providerAction.SelectFirstReasonCode();
            _providerAction.WaitForStaticTime(500);
            _providerAction.InsertDecisionRationale("UI Test Note");
            _providerAction.WaitForStaticTime(500);
            if(popUp)
                _providerAction.ClickOnSaveActionWaitForPopup();
            else
                _providerAction.ClickOnSaveActionCondition();
            _providerAction.WaitForWorkingAjaxMessage();
        }

        private void IsErrorPopUpPresent(ProviderActionPage _providerAction, string errorMessage)
        {
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Is page error pop up present?");
            _providerAction.GetPageErrorMessage().Replace("\r\n", string.Empty).ShouldBeEqual(errorMessage,"Verify Popup Message");
            _providerAction.ClosePageError();
            _providerAction.WaitForStaticTime(1000);
        }

        private void Set_User_specified_conditions(ProviderActionPage _providerAction, string userSpecifiedCondition,
            string beginCode, string endCode, bool isClient = true, bool isFlagAllCodes = false)
        {
            _providerAction.ClickOnEditActionCondition();
            _providerAction.ClickOnSearchCondition();
            _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition);
            if(isFlagAllCodes)
                _providerAction.ClickOnFlagAllCodes();
            else
            {
                _providerAction.SetBeginCodeAndEndCode(beginCode, endCode);
            }

            _providerAction.ClickSearchButtonInUserSpecifiedCondition(isClient);
        }

        #endregion
    }
}

