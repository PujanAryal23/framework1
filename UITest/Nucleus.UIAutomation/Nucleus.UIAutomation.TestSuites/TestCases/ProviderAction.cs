using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.UIAutomation.TestSuites.TestCases.ClientUser;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ProviderAction
    {
        #region PRIVATE PROPERTIES

        /*private ProviderSearchPage _providerSearch;
        //private ProviderSearchPage _providerSearch;
        private ProviderActionPage _providerAction;
        private ClaimActionPage _claimAction;
        private SuspectProvidersPage _suspectProviders;*/

        #endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE METHODS

        /*protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _providerSearch = QuickLaunch.NavigateToProviderSearch();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }*/

        /*protected override void ClassCleanUp()
        {
            try
            {
                _providerAction.CloseDBConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }*/

        /*protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _providerSearch;
        }*/

        /*protected override void TestCleanUp()
        {
            base.TestCleanUp();

            if (CurrentPage.GetPageHeader() != PageHeaderEnum.ProviderSearch.GetStringValue())
            {
                _providerSearch = _providerAction.NavigateToProviderSearch();
            }

            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                //TODO: Check client and switch
                _providerSearch = _providerSearch.Logout().LoginAsHciAdminUser().NavigateToProviderSearch();
            }
            _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            _providerSearch.ClearAll();
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                ProviderEnum.AllProviders.GetStringValue());
        }*/

        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] //TANT-282
        public void Verify_google_icon_in_provider_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var prvSeq = "55100";

                _providerAction = _providerSearch.SearchByProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.IsGoogleSearchIconPresent().ShouldBeTrue("Is Google Search icon present?");
                _providerAction.CloseAnyPopupIfExist();
                _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
            }
        }


        [Test] //CAR-3056(CAR-2989)
        public void Verification_of_Google_quick_search_button_to_Provider_name()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
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

        [Test] //CAR-2103 [CAR-2391]
        public void Verification_of_server_side_verification_for_user_specified_condition()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var provSeq = testData["ProviderSequence"];
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

                    _providerAction.ClickOnEditActionCondition();
                    Generate_Rationale(requiredFields, inputValue, procCodes[0], procCodes[0],
                        userSpecifiedCondition[0], _providerAction,
                        isClient: false, isSearchResult: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    _providerAction.ClickOnEditActionCondition();

                    StringFormatter.PrintMessage("If any duplicate codes are found on other already active SUSCs.");
                    Generate_Rationale(requiredFields, inputValue, procCodes[0], procCodes[0],
                        userSpecifiedCondition[1], _providerAction,
                        isClient: false, isSearchResult: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    _providerAction.ClickOnEditActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag all codes, but other active manual condition(s) already exist that are flagging any codes.");
                    Generate_Rationale(requiredFields, inputValue, "", "", userSpecifiedCondition[0], _providerAction,
                        isFlagAllCodes: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    _providerSearch.DeleteConditionIdByProviderSequence(provSeq);

                    _providerAction.ClickOnEditActionCondition();
                    Generate_Rationale(requiredFields, inputValue, "", "", userSpecifiedCondition[0], _providerAction,
                        isFlagAllCodes: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();

                    StringFormatter.PrintMessage(
                        "If the user attempts add a condition to flag all codes when another active condition is already flagging all codes");
                    _providerAction.ClickOnEditActionCondition();
                    Generate_Rationale(requiredFields, inputValue, "", "", userSpecifiedCondition[1], _providerAction,
                        isFlagAllCodes: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag a specified code when another active condition exists that is flagging ALL codes");
                    _providerAction.ClickOnEditActionCondition();
                    Generate_Rationale(requiredFields, inputValue, procCodes[0], procCodes[0],
                        userSpecifiedCondition[1], _providerAction,
                        isClient: false, isSearchResult: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();

                    StringFormatter.PrintMessage(
                        "If the user attempts to add a manual condition to flag codes when another active condition exists that is flagging ALL codes");
                    _providerAction.ClickOnEditActionCondition();
                    Generate_Rationale(requiredFields, inputValue, procCodes[0], procCodes[1],
                        userSpecifiedCondition[1], _providerAction,
                        isClient: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);

                }
                finally
                {
                    _providerSearch.DeleteConditionIdByProviderSequence(provSeq);
                    if (automatedBase.CurrentPage.IsPageErrorPopupModalPresent())
                        automatedBase.CurrentPage.ClosePageError();
                }
            }
        }

        [Test,Category("NewProviderAction2")] //CAR-1752 (CAR-2002)
        [NonParallelizable]
        public void Validate_User_Specified_Condition_Form_Options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var provSeq = testData["ProviderSeq"];
                var procCodes = testData["ProcCodes"].Split(',');
                var userSpecifiedCondition = testData["UserSpecifiedConditions"].Split(',');

                _providerAction = _providerSearch.NavigateToProviderAction(() =>
                {
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", provSeq);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                    _providerSearch.WaitForPageToLoad();
                });
                _providerAction.ClickOnEditActionCondition();
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeTrue(
                        "Are user specified condition options present?"); //95936-proc code 98796, 98808- prvseq 
                _providerAction.ClickOnSelectConditionFromDropdownIcon();
                _providerAction.GetUserSpecifiedConditionList().ShouldCollectionBeEqual(
                    _providerAction.GetUserSpecifiedConditonEditList(),
                    "User specified conditions drop dowm values should match");
                _providerAction.ClickOnSelectConditionFromDropdownIcon();
                _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                IsErrorPopUpPresent("Search cannot be initiated without criteria entered.", _providerAction);
                _providerAction.SetBeginCodeAndEndCode("ABCDE", isTabRequired: true);
                _providerAction.GetValueofInputConditionCode(false)
                    .ShouldBeEqual("ABCDE", "End Proc should be auto populated");
                _providerAction.GetTextIfAddOrSearchButton().ShouldBeEqual("Search");
                _providerAction.ClickSearchButtonInUserSpecifiedCondition();
                IsErrorPopUpPresent("Search is invalid. Please enter new search criteria.", _providerAction);


                StringFormatter.PrintMessage("Verification that begin code and end code should be sequential");
                _providerAction.SetBeginCodeAndEndCode("99218", isTabRequired: true);
                _providerAction.SetEndCode("99214");
                _providerAction.ClickAddButtonInUserSpecifiedCondition();
                IsErrorPopUpPresent("Selected codes are not a valid range.", _providerAction);


                StringFormatter.PrintMessage("Verification of Flag all codes functionality");
                _providerAction.ClickOnFlagAllCodes();
                _providerAction.AreRangeSearchFieldsEnabled().ShouldBeFalse("Are range search fields disabled?");
                _providerAction.ClickOnFlagAllCodes();
                _providerAction.SetBeginCodeAndEndCode(procCodes[0], isTabRequired: true);
                _providerAction.GetValueofInputConditionCode(false)
                    .ShouldBeEqual(procCodes[0], "End Proc should be auto populated");
                _providerAction.ClickSearchButtonInUserSpecifiedCondition();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeFalse("Is page error pop up present?");
                _providerAction.IsSearchResultsPresent().ShouldBeTrue("Is search results present?");
                _providerAction.GetValueOfUserSpecifiedSearchResults().ShouldBeEqual(
                    _providerAction.GetSearchResultForSingleProcCodeInUserSpecifiedConditionFromDb(procCodes[0],
                        "HCPCS"),
                    "Code and code description match?");
                _providerAction.IsMactchingConditionsPresent().ShouldBeTrue("Is Matching Conditions present?");
                _providerAction.GetMatchingConditionsRecords().Count
                    .ShouldBeGreater(0, "Matching Conditions should be present");
                _providerAction.ClickOnUserSepcifiedSearchResults();
                IsErrorPopUpPresent("Cannot add codes without a condition selected.", _providerAction);
                _providerAction.SelectConditionFromDropdownList(userSpecifiedCondition[0]);
                _providerAction.ClickOnUserSepcifiedSearchResults();
                _providerAction.IsSelectedConditionsPresentInActionConditions()
                    .ShouldBeTrue("Is Selected Conditions present in Action Conditions?");
                _providerAction.AreSelectedUserSpecifiedConditionsPresent()
                    .ShouldBeTrue("Are selected user specified condition options present?");

                //click proc code checkbox
                _providerAction.SelectProcCodeUserSpecifiedCondition();
                _providerAction.IsSelectedConditionsPresentInActionConditions()
                    .ShouldBeFalse("Is Selected Conditions present in Action Conditions?");
                _providerAction.AreSelectedUserSpecifiedConditionsPresent()
                    .ShouldBeFalse("Are selected user specified condition options present?");

                //cancel 
                Generate_Rationale(requiredFields, inputValue, procCodes[1], procCodes[2],
                    userSpecifiedCondition[1], _providerAction);
                _providerAction.ClickOnCancelUserAddedAction();
                _providerAction.IsUserSpcifiedConditionFormPresent().ShouldBeFalse(
                    "Selecting cancel after selections have been made will close the User Specified Conditions form");
                _providerAction.IsSelectedUserSpecifiedConditionComponentPresent().ShouldBeTrue(
                    "Selecting cancel after selections have been made will still retain the selected condition");
                _providerAction.IsActionConditionFormPresent().ShouldBeTrue("Is form open?");
                _providerAction.IsSelectedConditionsPresentInActionConditions()
                    .ShouldBeTrue("Are Selected condition present?");
                }
        }


      
        [Test]//CAR-1752 (CAR-2002) + CAR-2051 
        public void Verify_Addition_of_Manage_Codes_of_Concern()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var prvseq = testData["ProviderSequence"].Split(',');
                var userSpecifiedConditions = testData["UserSpecifiedConditions"].Split(',');

                try
                {
                    _providerSearch.DeleteConditionIdByProviderSequence(prvseq[0]);
                    _providerAction = _providerSearch.NavigateToProviderAction(() =>
                    {
                        _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvseq[0]);
                        _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                    });
                    _providerAction.ClickOnEditActionCondition();

                    StringFormatter.PrintMessageTitle("Verify Add button when a range of Proc code is entered");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[0]);
                    _providerAction.SetBeginCodeAndEndCode(prvseq[1], null, true);
                    _providerAction.IsAddButtonPresentInUserSpecifiedCondition()
                        .ShouldBeFalse("Add Button should not be present");
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessageTitle(
                        "Verify error message when an invalid range of Proc code is entered");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[0]);
                    _providerAction.SetBeginCodeAndEndCode("12345", "12346");
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent("Search is invalid. Please enter new search criteria.", _providerAction);
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessageTitle("Verify Flag All Codes message");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[1]);
                    _providerAction.ClickOnFlagAllCodes();
                    _providerAction.IsAddButtonPresentInUserSpecifiedCondition()
                        .ShouldBeTrue("Add Button should be displayed When a range of Proc Codes is entered");
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);

                    var msg = _providerAction.GetSelectedUserSpecifiedCondition();
                    string[] splitlist = msg.Split('-');
                    string selectedUserSpecifiedCondition = splitlist[0].Trim();
                    selectedUserSpecifiedCondition.ShouldBeEqual(_providerAction
                        .GetSelectedUserSpecifiedConditionFromDropdown());
                    _providerAction.GetMessageWhenFlagAllCodesIsSelected()
                        .ShouldBeEqual("Flag all services reported by this provider.");
                    _providerAction.WaitForStaticTime(500);


                    StringFormatter.PrintMessage(
                        "If the user attempts to flag all codes when another condition is already flagging all codes");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[0]);
                    _providerAction.WaitForStaticTime(500);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent("A user specified condition is already flagging all codes for this provider.", _providerAction);


                    _providerAction.SelectAction("Review");
                    _providerAction.SelectFirstReasonCode();
                    _providerAction.ClickOnGenerateRationaleButton();
                    for (int i = 0; i < requiredFields.Count(); i++)
                    {
                        _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
                    }

                    _providerAction.ClickOnFinishButton();
                    _providerAction.ClickOnSaveActionCondition();


                    StringFormatter.PrintMessageTitle("Verify audit message");
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.WaitForWorking();
                    _providerAction.ClickOnConditionDetailsIcon();
                    var codeOfConcernRow = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    codeOfConcernRow.Contains(String.Format("Codes of Concern:\r\n{0}", "Flag All Codes"));


                    StringFormatter.PrintMessage("Add button should be displayed");
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickSearchForConditionsByCode();
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[0]);
                    _providerAction.SetBeginCodeAndEndCode(prvseq[1], prvseq[2]);
                    _providerAction.IsAddButtonPresentInUserSpecifiedCondition()
                        .ShouldBeTrue("Add Button should be displayed When a range of Proc Codes is entered");
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessage(
                        "Verification that same range cannot be added to multiple user specified conditions");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[1]);
                    _providerAction.SetBeginCodeAndEndCode(prvseq[1], prvseq[2]);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition(true);
                    IsErrorPopUpPresent(
                        "The selected code(s) has already been added to another user specified condition.", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    _providerSearch.DeleteConditionIdByProviderSequence(prvseq[0]);

                    StringFormatter.PrintMessage(
                        "Verification that only one instance of user specified condition can be created. Single proc code");
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickSearchForConditionsByCode();
                    Generate_Rationale(requiredFields, inputValue, prvseq[1], prvseq[1],
                        userSpecifiedConditions[0], _providerAction, isClient: false, isSearchResult: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();


                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[0]);
                    _providerAction.SetBeginCodeAndEndCode(prvseq[3], isTabRequired: true);
                    _providerAction.ClickSearchButtonInUserSpecifiedCondition();
                    IsErrorPopUpPresent("Search is invalid. Please enter new search criteria.", _providerAction);

                    StringFormatter.PrintMessage(
                        "Verification that provider can only be on review for single proc code regardless of user specified condition selected");
                    Generate_Rationale(requiredFields, inputValue, prvseq[1], prvseq[1], userSpecifiedConditions[1], _providerAction,
                        isClient: false, isSearchResult: true);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorking();
                    IsErrorPopUpPresent(
                        "Condition action has not been saved. A user specified condition was not added as one is already active for the selected procedure code(s).", _providerAction);
                    _providerAction.ClickOnCancelLinkInActionCondition();
                    _providerAction.ClickOnEditActionCondition();


                    StringFormatter.PrintMessage(
                        "Verification of error message after exceeding 100 codes and user will be able to deselect the code to exclude them from flagging");
                    Verify_if_result_set_exceeds_100_codes_warning_message_will_be_displayed(prvseq[1], prvseq[6], _providerAction);
                    _providerAction.SelectProcCodeUserSpecifiedCondition();
                    _providerAction.GetResultSetProcCodeCount().ShouldBeLess(100,
                        "User should be able to deselect the code in this view to exclude them from flagging. ");
                    _providerAction.CountOfCheckboxNextToTheCode()
                        .ShouldBeEqual(_providerAction.GetResultSetProcCodeCount());
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();

                    StringFormatter.PrintMessage(
                        "Verification that new container should be added for the new search criteria");
                    _providerAction.SelectConditionFromDropdownList(userSpecifiedConditions[1]);
                    _providerAction.SetBeginCodeAndEndCode(prvseq[7], prvseq[8]);
                    _providerAction.IsMactchingConditionsPresent().ShouldBeFalse("Is Matching Conditions present?");
                    _providerAction.ClickAddButtonInUserSpecifiedCondition();
                    _providerAction.GetResultSetContainerCount().ShouldBeEqual(2,
                        "A new result set container should be added for the new search criteria");
                    _providerAction.ClickOnCancelLinkInActionCondition();

                }

                finally
                {
                    _providerSearch.DeleteConditionIdByProviderSequence(prvseq[0]);
                }
            }
        }


        [Test, Category("NewProviderAction2")] //CAR-1664 (CAR-1854) + TE-750

        public void Verify_Export_12_month_History_to_Excel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "12MonthsClaimHistoryExportedExcelFileHeaders").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
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
                    automatedBase.EnvironmentManager.HciAdminUsername);

                try
                {
                    _providerAction = ClickOnExportHistoryIcon(label, prvSequencesWithData, _providerSearch);

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
                            automatedBase.EnvironmentManager.HciAdminUsername)
                        .ShouldBeTrue("Is Audit Action present?");
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    _providerAction = ClickOnExportHistoryIcon(label, prvSequencesWithoutData, _providerSearch);
                    _providerAction.GetWindowHandlesCount().ShouldBeEqual(2, "New Blank tab should open");
                    _providerAction.CloseAnyTabIfExist();
                    _providerAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue(),
                        "Page Header Should be Provider Action");
                }

                finally
                {
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileNameFromDb);
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    _providerSearch.DeleteDocumentAuditRecordFromDb(
                        string.Concat(prvSequencesWithoutData, ",", prvSequencesWithData),
                        automatedBase.EnvironmentManager.HciAdminUsername);

                }
            }
        }

        [Test, Category("NewProviderAction2")] //CAR-3055([CAR-2961])

        public void Verify_Export_3_years_History_to_Excel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "12MonthsClaimHistoryExportedExcelFileHeaders").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var prvSequencesWithData = parameterList["PrvSequenceWithData"];
                var label = parameterList["Label"];
                var sheetname = parameterList["SheetName"];
                var expectedDataList =
                    _providerSearch.GetExport3YearsHistoryExcelData(prvSequencesWithData);
                var fileNameFromDb =
                    _providerSearch.GetExport3YearsHistoryExcelFileNameFromDatabase(prvSequencesWithData) + "_3Years";

                try
                {
                    _providerAction = ClickOnExportHistoryIcon(label, prvSequencesWithData, _providerSearch,  false);
                    automatedBase.CurrentPage.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Message should be shown to the users");
                    automatedBase.CurrentPage.GetPageErrorMessage()
                        .ShouldBeEqual("This export contains PHI. Do you wish to continue?");

                    StringFormatter.PrintMessage("Verify clicking cancel warning is closed");
                    automatedBase.CurrentPage.ClickOkCancelOnConfirmationModal(false);
                    automatedBase.CurrentPage.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Message should not be shown to the users");

                    automatedBase.CurrentPage.NavigateToProviderSearch();
                    ClickOnExportHistoryIcon(label, prvSequencesWithData, _providerSearch,  false);
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
                            automatedBase.EnvironmentManager.HciAdminUsername)
                        .ShouldBeTrue("Is Audit Action present?");
                }

                finally
                {
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileNameFromDb);
                    _providerAction.ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
                    _providerSearch.DeleteDocumentAuditRecordFromDb(prvSequencesWithData,
                        automatedBase.EnvironmentManager.HciAdminUsername);
                }
            }
        }

        //suspect providers work list
        [Test, Category("Working")] //CAR-39(CAR-484)  story already reviewed awating bux fix
        public void Verify_Suspect_Provider_Work_List_Menu_Option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;
                SuspectProvidersPage _suspectProviders;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                _suspectProviders = automatedBase.CurrentPage.NavigateToSuspectProviders();
                var providerSeqList = _suspectProviders.GetGridViewSection.GetGridListValueByCol(3);
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

        [Test, Category("NewProviderAction1")] //US50622 +US58229
        public void Verify_code_of_concern_modification_for_conditions_that_previously_defined_as_too_many_to_display()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Date", "Value");
                var procCodeList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                            "ProcCodeList", "Value")
                        .Split(',')
                        .ToList();
                var action = "Review";

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForCotiviti(prvSeq, date);

                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnConditionDetailsIcon();
                var preAction = _providerAction.GetProviderConditionDetailForFieldAndRow("Cotiviti Action", 1);
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("A30D");
                if (!_providerAction.IsLengthyMessageLabelPresent())
                {
                    action = preAction.Equals(action) ? "Deny" : "Review";
                    ActionCondition(_providerAction, reasonCode: "", action: action, note: "UITEST", procCodeList: procCodeList);
                    _providerAction.SelectFilterConditions(3); //select all triggered condition
                    _providerAction.ClickOnConditionDetailsIcon();
                }
                else
                {
                    _providerAction.ClickOnCancelActionCondition();
                }

                preAction = _providerAction.GetProviderConditionDetailForFieldAndRow("Cotiviti Action", 1);
                action = preAction.Equals(action) ? "Deny" : "Review";
                var conditionDetailRow = _providerAction.GetConditionDetailsRowCount();
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("A30D");
                _providerAction.GetConditionDescription()
                    .ShouldBeEqual(
                        "The list of procedure codes associated with this condition is too lengthy to display.");
                _providerAction.GetCodeSelectionLabel().ShouldBeEqual("Code Selection", "Code Selection Label:");
                _providerAction.IsCodeSelectionSearchBoxPresent()
                    .ShouldBeTrue("Procedure Code Search Box should displayed");

                _providerAction.searchByProcedureCodeOnCodeSelection("");
                _providerAction.GetValidationNoticePopupMessage()
                    .ShouldBeEqual("No search criteria has been entered. Please enter a valid CPT or HCPCS code.",
                        "Popup validation message for empty procedure code");
                _providerAction.ClosePageError();

                _providerAction.searchByProcedureCodeOnCodeSelection("Test123");
                _providerAction.GetValidationNoticePopupMessage()
                    .ShouldBeEqual(string.Format(
                            "The procedure code '{0}' is either invalid or not associated with the selected condition. Please enter a new value.",
                            "Test123"),
                        "Popup Message for invalid procedure code");
                _providerAction.ClosePageError();
                _providerAction.SearchByProcedureCodeListOnCodeSelection(procCodeList);
                _providerAction.IsLengthyMessageLabelPresent().ShouldBeFalse("Lengthy Message Label displayed");
                _providerAction.IsCodeOfConcernLabelPresent().ShouldBeTrue("Code of Concern Label displayed");
                _providerAction.IsCodeSelectionSearchBoxEnabled()
                    .ShouldBeFalse("Code Selection Search Box Enabled?");
                _providerAction.IsCodeSelectionSearchIconDisabled()
                    .ShouldBeTrue("Code Selection Search Icon Disabled?");
                _providerAction.GetTooltipOfSearchBoxCodeSelection()
                    .ShouldBeEqual("A maximum of 100 codes of concern can be selected for this condition.",
                        "Tooltip of Disabled Search Box next to Code Selection");

                //_providerAction.Get5ColumnCount().ShouldEqual(5, "Codes of Concern should diplay in 5 columns");
                _providerAction.IsCodeofConcernSorted()
                    .ShouldBeTrue("Codes of concern sorted"); //us58229 story added
                _providerAction.ClickOnCodeOfConcernByCode(procCodeList[0]);
                ActionCondition(_providerAction, reasonCode: "", action: action);
                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.GetConditionDetailsCodeOfConcernRow(1)
                    .Contains("Custom Codes Applied")
                    .ShouldBeTrue("The condition will no longer use the master code list");
                _providerAction.GetConditionDetailEllipseTitleByRow()
                    .Split(',')
                    .Select(val => val.Trim())
                    .Contains(procCodeList[0])
                    .ShouldBeFalse("Deselected Code should not present");
                _providerAction.GetConditionDetailsRowCount()
                    .ShouldBeEqual(conditionDetailRow + 1, "Audit Record should added");

                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.IsCodeofConcernByCodePresent(procCodeList[0])
                    .ShouldBeFalse("Deselected code should not present");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("A30D");
                action = action == "Deny" ? "Review" : "Deny";
                ActionCondition(_providerAction, reasonCode: "", action: action, note: "UITEST", procCodeList: procCodeList);

                _providerAction.IsCodeofConcernByCodePresent(procCodeList[0])
                    .ShouldBeFalse("Deselected Code Should not dispalyed");
                
            }
        }

        [Test, Category("NewProviderAction1")] //US51986
        public void Verify_behaviour_cancel_action_on_decision_rationale()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                //_providerAction = _newProviderSearch.ClickOnProviderNameToOpenNewProviderActionForProvSeq(prvSeq);
                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("SIJF");

                _providerAction.SelectReasonCode("BEW4 - BEW Test 4");
                // _providerAction.InsertDecisionRationale("UI Test Note");
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Warning Message");
                _providerAction.GetConfirmationMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?");

                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when click on cancel on popup window");
                _providerAction.ClickOkCancelOnConfirmationModal(false);
                _providerAction.GetActionCondtionInputFieldValue("Reason Code")
                    .ShouldBeEqual("BEW4 - BEW Test 4", "Reason Code should retained");

                _providerAction.ClearReasonCode();

                _providerAction.SelectAction("No Action");
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Warning Message");
                _providerAction.GetConfirmationMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?");

                StringFormatter.PrintMessageTitle(
                    "Verify Action Condtion field when click on cancel on popup window");
                _providerAction.ClickOkCancelOnConfirmationModal(false);
                _providerAction.GetActionCondtionInputFieldValue("Action")
                    .ShouldBeEqual("No Action", "Action should retained");

                _providerAction.SelectReasonCode("BEW4 - BEW Test 4");

                StringFormatter.PrintMessageTitle("Verify Action Condtion field when click on ok on popup window");
                _providerAction.ClickOnCancelActionConditionWithoutConfirmation();
                _providerAction.ClickOkCancelOnConfirmationModal(true);
                string[] expectedComponentTitlesInActionPage =
                {
                    "Provider Details", "Provider Conditions",
                    "Condition Exposure"
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
                    .ShouldBeFalse("Previous Selected Action should not selected");
                _providerAction.GetReasonCodeDropDrownValue()
                    .ShouldBeEqual("Select Reason Code", "Previously Selected Reason Code should not selected");
                _providerAction.GetDecisionRationaleTextForCotivitiUser()
                    .ShouldBeNullorEmpty("Previously set Decision Rationale Test should not displayed");
                _providerAction.ClickOnCancelActionCondition();
            }
        }

        [Test, Category("NewProviderAction1")] //US50620
        public void Verify_CIU_referrals_details_Records()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var claSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "ClaimSequence",
                    "Value");
                const string phoneNo = "231-456-7890";
                const string procCode = "123456789101112131415";
                const string category = "IC,RD,RE,RP,UP";
                const string pattern = "Test Identified Pattern\r\nTest Identified Pattern\r\nTest Identified Pattern";
                
                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                StringFormatter.PrintMessageTitle("Verification of CIU Record Detail Value");
                var createdDateList = _providerAction.GetCIUCreatedDateList();
                createdDateList.IsInDescendingOrder().ShouldBeTrue("Created Date Should be in Descending Order");
                VerifyThatDateIsInCorrectFormat(createdDateList[0], "Record Created Date");
                VerifyThatNameIsInCorrectWithUserNameFormat(
                    _providerAction.GetCIUReferralDetailsByRowLabel(1, "By:"), "Referral Created By");
                _providerAction.GetCIUReferralDetailsByRowLabel(2, "Claim Seq:")
                    .ShouldBeEqual(claSeq,
                        "CIU Referral Created from Claim Action should display corresponding Claim Sequence");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Claim Seq:")
                    .ShouldBeEqual("All",
                        "CIU Referral Createdy from New Provider Action should dispaly <All> in claim Sequence");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Ph:")
                    .ShouldBeEqual(phoneNo, "Phone No Should dispalyed according to Created By");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Type:")
                    .ShouldBeEqual("Research", "Type Should Equal");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Proc Code:")
                    .ShouldBeEqual(procCode, "Type Should Equal");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Category:")
                    .ShouldBeEqual(category, "Category Abbreviation should Equal and separated by comma");
                _providerAction.GetCIUReferralDetailsByRowLabel(1, "Pattern")
                    .ShouldBeEqual(pattern, "Pattern should dispaly entire text");
                _providerAction.GetCIUReferralToolTipDayByRowLabel(1, "Pattern")
                    .ShouldBeEqual(Regex.Replace(pattern, "\r\n", ""),
                        "Pattern Tooltip should equal and should not contains next line");
                _providerAction.IsVerticalScrollBarPresentInProviderDetailSection()
                    .ShouldBeTrue("Scrollbar Should display in Provider Detail Section");
            }
        }


        [Test, Category("NewProviderAction1")] //US50620
        public void Verify_Delete_CIU_referrals_details_Record()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction = null;
                ClaimActionPage _claimAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var claimSequenceList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ClaimSequenceList", "Value");

                try
                {
                    _providerSearch.SearchByProviderSequence(prvSeq);
                    automatedBase.CurrentPage =
                        _providerAction =
                            _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                    StringFormatter.PrintMessageTitle("Verification of Delete CIU Referral Records");

                    _providerAction.IsVerticalScrollBarPresentInProviderDetailSection()
                        .ShouldBeFalse("Scrollbar Should not display in Provider Detail Section");
                    if (_providerAction.GetCIUReferralRecordRowCount() == 0)
                        CreateCIUReferral(_providerAction);
                    _providerAction.ClickOnDeleteCIUReferralIconByRecordRow(1);
                    _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confirmation popup should display");
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual("The selected CIU referral will be deleted. Click Ok to proceed or Cancel.",
                            "Verification of  CIU Referral Delete popup message");
                    _providerAction.ClickOkCancelOnConfirmationModal(false);
                    _providerAction.GetCIUReferralRecordRowCount().ShouldBeEqual(1, "Record will not be deleted");
                    _providerAction.ClickOnDeleteCIUReferralIconByRecordRow(1);
                    _providerAction.ClickOkCancelOnConfirmationModal(true);
                    _providerAction.WaitForWorkingAjaxMessage();
                    _providerAction.GetCIUReferralRecordRowCount().ShouldBeEqual(0, "Record should deleted");
                    _providerAction.GetNoCIUReferralMessage()
                        .ShouldBeEqual("There are no CIU referral details available.",
                            "Verification of No CIU Referral Message");
                    //change
                    _claimAction = _providerAction.NavigateToCVClaimsWorkList();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    foreach (var claimSequence in claimSequenceList.Split(';'))
                    {
                        _claimAction.ClickWorkListIcon();
                        _claimAction.ClickSearchIcon();
                        _claimAction.SearchByClaimSequence(claimSequence)
                            .HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickonProviderDetailsIcon();
                        _claimAction.GetCiuReferralRecordRowCount()
                            .ShouldBeEqual(0,
                                "CIU Referral should empty when CIU Referral deleted");
                    }

                    //change
                    automatedBase.CurrentPage = _providerSearch = _claimAction.NavigateToProviderSearch();
                }
                finally
                {
                    if (_providerAction.GetCIUReferralRecordRowCount() == 1)
                    {
                        _providerAction.ClickOnDeleteCIUReferralIconByRecordRow(1);
                        _providerAction.ClickOkCancelOnConfirmationModal(true);
                        _providerAction.WaitForWorkingAjaxMessage();
                    }

                    automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }

        [Test, Category("NewProviderAction1")] //US48448
        public void Verify_scrollbar_should_not_display_in_provider_action_exposure_section()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                _providerAction.IsVerticalScrollBarPresentInProviderExposureSection()
                    .ShouldBeFalse("Scroll Bar should not displayed");
                
            }
        }

        [Test, Category("NewProviderAction1")] //US51991
        public void Verify_lengthy_message_with_list_of_selected_code_of_concern_in_Manage_Codes_of_Concern_column_for_a_condition_that_has_already_been_actioned ()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId("A30D");
                _providerAction.GetConditionDescription()
                    .ShouldBeEqual(
                        "The list of procedure codes associated with this condition is too lengthy to display.");
                _providerAction.ClickOnSelectedConditionByCode("A30D");
                _providerAction.SelectProviderConditionByConditionId("SCAC");
                _providerAction.GetAllCodesofConcernListCount()
                    .ShouldBeLessOrEqual(100, "Codes will be listed here for having 100 or less codes of cncern");
                _providerAction.GetColorOfSelectedCodeOfConcern()
                    .AssertIsContained("rgba(49, 0, 111, 1)", "Blue Color of Selected Code of Concern");
                _providerAction.ClickOnCancelActionCondition();
                
            }
        }

        [Test, Category("NewProviderAction1")]
        public void Verify_action_condition_form_when_click_on_quick_no_action_icon_X()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var reTriggerPeriod = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ReTriggerPeriod", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ReasonCode",
                    "Value");
                var action = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "Action",
                    "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                _providerAction.SelectFilterConditions(3);
                _providerAction.ClickOnQuickNoActionXIcon();
                _providerAction.GetActionDropDrownValue()
                    .ShouldBeEqual(action, string.Format("{0} Should be selected by default ", action));
                _providerAction.GetReasonCodeDropDrownValue()
                    .ShouldBeEqual("Select Reason Code", "User Required to Select Reason Code");
                _providerAction.GetActiveRetriggerPeriodValue()
                    .ShouldBeEqual(reTriggerPeriod,
                        string.Format("{0} Should Selected by default", reTriggerPeriod));
                _providerAction.SelectReasonCode(reasonCode);
                _providerAction.GetDecisionRationaleText()
                    .ShouldBeEqual(reasonCode, "Reason Code Should Stored after Reason Code is selected");
                _providerAction.IsVisibleToClientCheckBoxChecked()
                    .ShouldBeFalse("Visible to Client should default unchecked");
                _providerAction.ClickOnCancelActionCondition();
                string[] expectedComponentTitlesInActionPage =
                {
                    "Provider Details", "Provider Conditions",
                    "Condition Exposure"
                };
                _providerAction.GetComponentTitlesFromPage()
                    .ShouldCollectionBeEqual(expectedComponentTitlesInActionPage,
                        "Provider Action returned to default state");
            }
        }

        [Test, Category("NewProviderAction1")]
        public void Verify_visit_count_is_accurate_formatted_and_display_correctly()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                ProviderProfilePage providerProfile = null;

                var visitall = _providerSearch.GetVisitAllFromDatabase(provSeq);
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                Convert.ToInt16(_providerAction.GetProviderExposureCountValue(2, 1))
                    .ShouldBeEqual(visitall, "Verify Visit All data");
                var visitAvg = _providerAction.GetProviderExposureAvgValue(2, 1);
                visitAvg.Contains("Avg:").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                visitAvg.Contains("(").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                visitAvg.Contains("%)").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                _providerAction.GetProviderExposureVisitAvgValueOnly(2, 1)
                    .ShouldBeGreater(0,
                        "Avg Value Should Greater than Zero");

                //providerProfile = _providerAction.ClickOnWidgetProviderIconToOpenProviderProfile();
                //providerProfile.GetProviderExposureVisitCountValue()
                //    .ShouldBeEqual(visitall, "Visit Count Should Equal to visit count in provider search");
                //providerProfile.GetProviderExposureAvgValue().Replace("+", "")
                //    .ShouldBeEqual(visitAvg, "Avg Value Should Equal to avg value in provider search");
                //providerProfile.CloseProviderProfileAndSwitchToProviderAction();

                /* _providerSearch.SearchByProviderSequence(provSeq);
                 var visitCount = _providerSearch.GetProviderExposureVisitCountValue();
                 var visitAvg = _providerSearch.GetProviderExposureVisitAvgValue();
                 visitCount.ShouldBeGreater(0, "Visit Count Should Greate than Zero");
                 visitAvg.Contains("Avg:").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                 visitAvg.Contains("(").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                 visitAvg.Contains("%)").ShouldBeTrue("Avg Value Should be formatted and displayed correctly");
                 var vistAvgIntegerValue = _providerSearch.GetProviderExposureVisitAvgValueOnly();
                 vistAvgIntegerValue.ShouldBeGreater(0, "Average Value Shoul Greate than Zero");
 
                 
 
                 automatedBase.CurrentPage =
                     _providerAction = _providerSearch.ClickOnProviderNameToOpenNewProviderActionForProvSeq(provSeq);
 
                 Convert.ToInt16(_providerAction.GetProviderExposureCountValue(2, 1)).ShouldBeEqual(visitCount,
                     "Visit Count Should Equal to visit count in provider search");
                 var vistAvgValueInProviderAction = _providerAction.GetProviderExposureAvgValue(2, 1);
                 vistAvgValueInProviderAction.Contains("Avg:")
                     .ShouldBeTrue("Avg Value Should be formatted and displayed correctly in New Provider Action Page");
                 vistAvgValueInProviderAction.Contains("(")
                     .ShouldBeTrue("Avg Value Should be formatted and displayed correctly in New Provider Action Page");
                 vistAvgValueInProviderAction.Contains("%)")
                     .ShouldBeTrue("Avg Value Should be formatted and displayed correctly in New Provider Action Page");
                 _providerAction.GetProviderExposureVisitAvgValueOnly(2, 1)
                     .ShouldBeGreater(0,
                         "Avg Value Should Greater than Zero");*/
                
            }
        }

        [Test, Category("NewProviderAction1")] //US50623 test modified for slight change requirement of US50623
        public void Verify_that_Cotiviti_users_see_Fraud_specific_reason_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.SelectFilterConditions(3); //select all triggered condition
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    IList<string> expectedResponseCodeList = _providerAction.GetExpectedResonCodeList();
                    expectedResponseCodeList.Insert(0, "");
                    IList<string> responseCodeList = _providerAction.GetReasonCodeOptions();
                    responseCodeList.ShouldCollectionBeEqual(expectedResponseCodeList,
                        "All are Fraud specific Reason Codes:");
                    responseCodeList.ShouldCollectionBeSorted(false,
                        "Fraud reason codes are displayed in an alphabetical order.");
                }
            }
        }

        [Test, Category("NewProviderAction1")]
        public void Verify_that_Visible_to_Client_is_auto_checked_when_user_selects_Review_or_Deny()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("Review");
                    _providerAction.IsVisibleToClientCheckBoxChecked().ShouldBeTrue("Visible To Client checked?");
                    _providerAction.SelectAction("Deny");
                    _providerAction.IsVisibleToClientCheckBoxChecked().ShouldBeTrue("Visible To Client checked?");

                    _providerAction.ClickOnCancelActionCondition();
                }
            }
        }

        [Test, Category("NewProviderAction1")]
        public void Verify_that_conditions_cannot_be_saved_with_no_code_of_concern_selected()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var validationMsg = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "MessageText", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectProviderConditionByConditionId("SSWP");
                    _providerAction.SelectAction("Review");
                    _providerAction.SelectFirstReasonCode();

                    _providerAction.InsertDecisionRationaleUsingGenerateRationale("uitest");

                    if (!_providerAction.IsSelectAllCodesofConcernCheckBoxChecked())
                    {
                        _providerAction.ClickOnSelectAllCheckBoxManageCodeofConcern();

                    }

                    _providerAction.ClickOnSelectAllCheckBoxManageCodeofConcern();
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.IsValidationNoticeModalPopupPresent()
                        .ShouldBeTrue("Validation Notice Modal Popup is shown");
                    _providerAction.GetValidationNoticePopupMessage()
                        .Contains(validationMsg)
                        .ShouldBeTrue("Proper message is shown.");
                    _providerAction.CloseValidationNoticePopup();
                    _providerAction.ClickOnCancelActionCondition();
                }
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_that_user_can_manually_add_a_condition_to_non_suspect_provider()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var procCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProcCode", "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.ClickOnSearchCondition();
                    _providerAction.SearchConditionByCodeAndClickOnFirstResult("SUSC", procCode, procCode);
                    _providerAction.GetFirstSelectedConditionText()
                        .ShouldBeEqual("SUSC - User Specified Condition", "First Selected Condition");
                    _providerAction.ClickOnCancelActionCondition();
                }
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_1_of_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "ProviderSequence", "Value");


                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 1"

                StringFormatter.PrintMessageTitle("Verifying the Provider Scorecard Popup from Provider Action page");
                var providerScoreCard = _providerAction.ClickOnProviderScorecard();
                providerScoreCard.GetProviderScorecardPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.ProviderScoreCard.GetStringValue());
                providerScoreCard.CloseProviderScoreCardAndReturnToProviderAction(PageTitleEnum.ProviderAction);

                //StringFormatter.PrintMessageTitle("Verifying Provider Profile popup when Provider Profile Icon is clicked");
                //var providerProfile = _providerAction.ClickOnWidgetProviderIconToOpenProviderProfile();
                //providerProfile.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderProfile.GetStringValue());
                //providerProfile.CloseProviderProfileAndSwitchToProviderAction();

                //StringFormatter.PrintMessageTitle("Verifying Provider Popup pops up when 'Provider under Review' icon is clicked");
                //providerProfile = _providerAction.ClickOnProviderReviewIconToOpenProviderProfile();
                //providerProfile.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderProfile.GetStringValue());
                //providerProfile.CloseProviderProfileAndSwitchToProviderAction();

                StringFormatter.PrintMessageTitle("Verifying clicking 'Manage Notes icon' presents the Notes form");
                _providerAction.ClickNoteIcon();
                _providerAction.IsAddNoteFormSectionPresent()
                    .ShouldBeTrue("Add Note Section is present after clicking the 'View or Manager Notes' icon");
                _providerAction.ClickNoteIcon();
                _providerAction.IsAddNoteFormSectionPresent().ShouldBeFalse("Add Note Section toggles off");

                //StringFormatter.PrintMessageTitle("Verifying 'Claim History pops up when 'Hx' icon is clicked'");
                //var providerClaimHistory = _providerAction.ClickProviderClaimHistoryToOpenProviderHistory(3);
                //providerClaimHistory.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimHistory.GetStringValue());
                //automatedBase.CurrentPage.CloseAnyPopupIfExist();

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
        public void Verify_google_maps_of_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
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
        public void Verify_quadrant_2_of_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 2"


                StringFormatter.PrintMessageTitle("Verify Top 10 Proc Codes icon");
                _providerAction.ClickOnTop10ProcCodes(1);
                _providerAction.GetTop10ProcCodeCount()
                    .ShouldBeGreater(0, "Data is being displayed for 'Top 10 Proc Codes By Count'");
                _providerAction.ClickOnTop10ProcCodes(2);
                _providerAction.GetTop10ProcCodeCount().ShouldBeGreater(0,
                    "Data is being displayed for 'Top 10 Proc Codes By Billed'");
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

                StringFormatter.PrintMessage(
                    "Verifying CIU Referral: Will be listed if there are associated CIU records");
                _providerAction.GetCIUReferralRecordRowCount().ShouldBeGreater(0,
                    "Associated CIU Referrals are being shown under the 'CIU Referral' section");
                _providerAction.ClickOnAddCIUReferralRecordCssSelector();
                _providerAction.IsCreateCIUReferralSectionDisplayed()
                    .ShouldBeTrue("Add CIU Referral form is being displayed");

                #endregion
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_3_of_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var providerSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "ProviderSequence", "Value");

                StringFormatter.PrintMessage("Searching for the Provider and navigating to Provider Action Page");
                _providerSearch.SearchByProviderSequence(providerSeq);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);

                #region "Quadrant 3"

                StringFormatter.PrintMessageTitle("Verification of 'Quick No Action All Conditions' icon");
                var providerCondition = _providerAction.GetConditionFromProviderConditions(1);
                _providerAction.ClickOnQuickNoActionXIcon();
                _providerAction.GetActionCondtionInputFieldValue("Action").ShouldBeEqual("No Action");
                _providerAction.GetConditionFromEditProvideActionColumns(1, 1).ShouldBeEqual(providerCondition);
                _providerAction.ClickOnCancelActionCondition();

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
                _providerAction.SaveActionConditions();
                if (_providerAction.IsPageErrorPopupModalPresent())
                    _providerAction.ClosePageError();

                _providerAction.IsInvalidInputPresentByLabel("Action")
                    .ShouldBeTrue("'Action' field should be highlighted with red if left empty");
                _providerAction.IsInvalidInputPresentByLabel("Reason Code")
                    .ShouldBeTrue("'Reason Code' field should be highlighted with red if left empty");
                _providerAction.IsInvalidDecisionRationalePresent()
                    .ShouldBeTrue("'Decision Rationale' should be highlighted with red if left empty");

                _providerAction.ClickOnCancelActionCondition();

                StringFormatter.PrintMessageTitle(
                    "When 'Quick No Action icon' is clicked, will set the result to No Action for the specific condition requiring review");
                var listOfProviderConditions = _providerAction.GetConditionIdListInProviderConditions();
                _providerAction.ClickOnQuickNoActionAllConditions();
                _providerAction.GetConditionIdListInProviderConditions().ShouldCollectionBeEqual(
                    listOfProviderConditions, "All condition that going to be taken No Action should match.");
                _providerAction.ClickOnCancelActionCondition();

                StringFormatter.PrintMessageTitle("Verification of 'Next' button in Provider Action page");
                Verify_Next_Button_In_Provider_Action(_providerSearch, automatedBase, _providerAction);

                #endregion

            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-89
        public void Verify_quadrant_4_of_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var providerSequenceWithConditionInformation = automatedBase.DataHelper.GetSingleTestData(
                    FullyQualifiedClassName, TestName, "ProviderSequenceWithConditionInformation",
                    "Value");

                #region "Quadrant 4"

                StringFormatter.PrintMessageTitle(
                    "Verification of 'Condition Notes', 'Condition Exposure' and 'Condition Details' icons");
                _providerSearch.SearchByProviderSequence(providerSequenceWithConditionInformation);
                _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(
                        providerSequenceWithConditionInformation);

                _providerAction.SelectFilterConditions(3);
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeTrue("Condition Exposure section is present");

                _providerAction.ClickonProviderNotesinConditionExposureSection();
                _providerAction.GetConditionNotesAssociatedToTheCondition()
                    .ShouldBeGreater(0, "Condition Notes are present for the selected condition");

                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.GetConditionDetailsRowCount().ShouldBeGreater(0,
                    "The Condition Details are present for the selected condition ");

                #endregion
                
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_that_top_10_procedure_codes_can_be_viewed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);

                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.ClickOnTop10ProcCodes(1);

                _providerAction.GetTop10ProcCodeCount().ShouldBeEqual(10, "Proc Codes displayed");
                _providerAction.GetTop10ProcCodeProcedureValueByRow(1)
                    .ShouldNotBeTheSameAs("", "Procedure code is not empty");
                _providerAction.GetTop10ProcCodeShortDescValueByRow(1)
                    .ShouldNotBeTheSameAs("", "Short Description is not empty");
                _providerAction.GetTop10ProcCodeCountLabelByRow(1).ShouldBeEqual("Count:", "Count label is correct");
                IsCountInCorrectFormat(_providerAction.GetTop10ProcCodeCountValueByRow(1), _providerAction)
                    .ShouldBeTrue("Count value is in correct format");
                _providerAction.GetTop10ProcCodeBilledLabelByRow(1).ShouldBeEqual("Billed:", "Billed label is correct");
                IsDollarAmountInCorrectFormat(_providerAction.GetTop10ProcCodeBilledValueByRow(1), _providerAction)
                    .ShouldBeTrue("Billed value is in correct format");
                _providerAction.GetTop10ProcCodePaidLabelByRow(1).ShouldBeEqual("Paid:", "Paid label is correct");
                IsDollarAmountInCorrectFormat(_providerAction.GetTop10ProcCodePaidValueByRow(1), _providerAction)
                    .ShouldBeTrue("Paid value is in correct format");

                var actualCountValueList = _providerAction.GetTop10ProcCodeCountValueList();
                VerifyThatListOfStringIsSortedDescByValue(actualCountValueList, _providerAction);

                _providerAction.ClickOnTop10ProcCodes(2);
                var actualBilledValueList = _providerAction.GetTop10ProcCodeBilledValueList();
                VerifyThatListOfStringIsSortedDescByValue(actualBilledValueList, _providerAction);

                _providerAction.ClickOnTop10ProcCodes(3);
                var actualPaidValueList = _providerAction.GetTop10ProcCodePaidValueList();
                VerifyThatListOfStringIsSortedDescByValue(actualPaidValueList, _providerAction);

                _providerAction.ClickOnProviderDetailsIcon();
                _providerAction.GetTop10ProcCodeCount().ShouldBeEqual(0, "Proc Codes displayed");
            }
        }

        [Test, Category("NewProviderAction1")] //US37256
        public void Verify_that_condition_specific_exposure_data_can_be_viewed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                const string proc = "99214";
                const string claseq = "1336874";
                _providerSearch.UpdateEdosDosIfOlderThanYearForProvSeqProcAndClaseq(provSeq, proc, claseq);
                _providerSearch.SearchByProviderSequence(provSeq);

                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderCondition(2);
                _providerAction.GetConditionExposureEmptyMessage()
                    .ShouldBeEqual("No matching exposure records were found", "Empty message");

                _providerAction.SelectProviderCondition(1);
                _providerAction.GetConditionExposureLabelByColRow(1, 1).ShouldBeEqual("Patients", "Label");
                _providerAction.GetConditionExposureLabelByColRow(1, 2).ShouldBeEqual("Visits", "Label");
                _providerAction.GetConditionExposureLabelByColRow(1, 3).ShouldBeEqual("Billed", "Label");
                _providerAction.GetConditionExposureLabelByColRow(1, 4).ShouldBeEqual("Paid", "Label");
                _providerAction.GetConditionExposureLabelByColRow(2, 1).ShouldBeEqual("Claims", "Label");
                _providerAction.GetConditionExposureLabelByColRow(2, 2).ShouldBeEqual("Lines", "Label");
                _providerAction.GetConditionExposureLabelByColRow(2, 3).ShouldBeEqual("Proc Codes", "Label");

                _providerAction.GetConditionExposureValueByColRow(1, 1)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(1, 2)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(1, 3)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(1, 4)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(2, 1)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(2, 2)
                    .ShouldBeGreater(0, "Value should not be empty");
                _providerAction.GetConditionExposureValueByColRow(2, 3)
                    .ShouldBeGreater(0, "Value should not be empty");

                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.GetComponentTitlesFromPage()[2].ShouldBeEqual("Condition Details",
                    "Switched to Condition Details");
            }
        }


        [Test, Category("NewProviderAction1")] //US39878
        public void Verify_toolbar_buttons_present_if_provname_too_long()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;
                var currWinSize = _providerAction.ReturnWindowSize();
                if (_providerAction.GetProviderNameLengthInProviderAction() > 44 && currWinSize.Width > 1280)
                {
                    _providerAction.AreToolBarButtonsVisible()
                        .ShouldBeTrue("Tool bar Buttons visibility when provider name is too long should be true");
                }
            }
        }

        [Test, Category("NewProviderAction1")] //US39874
        public void Verify_that_Cotiviti_users_see_Fraud_reason_code_in_condition_detail()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var reviewFirstRow = testData["ReviewInFirstRow"];
                var reviewSecondRow = testData["ReviewInSecondRow"];

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnConditionDetailsIcon();
                    var conditionDetailsRow = _providerAction.GetConditionDetailsRow(1);
                    _providerAction.IsProviderConditionReasonLabelPresent(1).ShouldBeTrue("Reason Label is present");
                    conditionDetailsRow.Contains(String.Format("Reason:\r\n{0}", reviewFirstRow))
                        .ShouldBeTrue("Reason Code is present");
                    _providerAction.SelectProviderConditionByConditionId("SM59");
                    _providerAction.IsProviderConditionReasonLabelPresent(1).ShouldBeTrue("Reason Label is present");
                    _providerAction.IsProviderConditionSelectedActive("SM59")
                        .ShouldBeTrue("Selected condition outlined in blue");
                    conditionDetailsRow = _providerAction.GetConditionDetailsRow(1);
                    conditionDetailsRow.Contains(String.Format("Reason:\r\n{0}", reviewSecondRow))
                        .ShouldBeTrue(
                            "Reason Code when unavailable 'No reason available' is present as a reason value.");
                    
                }
            }
        }

        [Test, Category("NewProviderAction1")] //US39872+US51989 added new story 
        public void
            Verify_that_condition_description_popup_should_be_in_proper_format_with_display_correct_description()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var conditionDescription = testData["ConditionDescription"];
                _providerSearch.SearchByProviderSequence(provSeq);
                
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    string originalHandle = string.Empty;
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.SelectProviderConditionConditionCode(1);
                    try
                    {
                        var conditionId = _providerAction.GetConditionIdFromProviderConditions(1).Split('-')[0];
                        _providerAction.SwitchToPopUpWindow();
                        _providerAction.GetProviderConditionIdPopupContent()
                            .ShouldBeEqual("Condition: " + conditionId, "Header Information is Correct");
                        _providerAction.IsLabelBoldInProviderConditionPopup()
                            .ShouldBeTrue("Condition Label Should be in Bold");
                        _providerAction.IsLabelBoldInProviderConditionPopup(3)
                            .ShouldBeTrue("Condition Description Label Should be in Bold");
                        _providerAction.GetProviderConditionDescription().ShouldBeEqual(
                            "Condition Description:\r\n" + conditionDescription,
                            "Condition Desctiption is accurate");
                    }
                    finally
                    {
                        _providerAction.CloseAnyPopupIfExist(originalHandle);
                    }
                }
            }
        }

        [Test, Category("NewProviderAction1")] //US39877
        public void Verifty_that_condition_exposure_when_no_provider_conditions_selected()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                const string proc = "83655";
                const string claseq = "1329207";
                const string proc1 = "99214";
                const string claseq1 = "1327769";

                _providerSearch.UpdateEdosDosIfOlderThanYearForProvSeqProcAndClaseq(provSeq, proc, claseq);
                _providerSearch.UpdateEdosDosIfOlderThanYearForProvSeqProcAndClaseq(provSeq, proc1, claseq1);

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeFalse(
                        "Condition Exposure Should not be Present in Triggered Conditions That Require Action");
                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId("SCMC");
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeTrue("Condition Exposure Should be Present in All Triggered Conditions");

                _providerAction.SelectFilterConditions(1);
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeFalse(
                        "Condition Exposure Should not be Present in Triggered Conditions That Require Action");

                _providerAction.SelectFilterConditions(2);
                _providerAction.SelectProviderConditionByConditionId("SCMC");
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeTrue(
                        "Condition Exposure Should be Present in Triggered Conditions That Have Already Been Actioned");

                _providerAction.SelectFilterConditions(1);
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeFalse(
                        "Condition Exposure Should not be Present Triggered Conditions That Require Action");

                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId("SCAC");
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeTrue("Condition Exposure Should be Present for All Triggered Conditions");

                _providerAction.SelectFilterConditions(2);
                _providerAction.SelectProviderConditionByConditionId("SLAB");
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeFalse(
                        "Condition Exposure Should not be Present for Triggered Conditions That Have Already Been Actioned");
                _providerAction.SelectProviderCondition(3);
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeTrue(
                        "Condition Exposure Should Present in Triggered Conditions That Have Already Been Actioned");

                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId("SLAB");
                _providerAction.IsConditionExposureDetailsSectionPresent()
                    .ShouldBeFalse("Condition Exposure Should not Present All Triggered Conditions");
                
            }
        }


        [Test, Category("NewProviderAction1")]
        //US45031--test modified after generate rationale texture and reresh button removed and decision rationale is forever disabled for Cotiviti user
        public void
            Verify_that_proper_validation_Generate_Rationale_Popup_form_and_Generate_rationale_button_should_disabled_other_than_review_or_deny
            ()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var billingSummary = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "BillingSummary", "Value");
                var license = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "License",
                    "Value");
                var providerWebsite = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderWebsite", "Value");
                var billed = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "Billed",
                    "Value");
                var paid = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Paid", "Value");
                var summary = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "Summary",
                    "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ReasonCode",
                    "Value");

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    _providerAction.SelectFilterConditions(2);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("Review");

                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button should not be disabled for Review?");
                    _providerAction.SelectAction("No Action");

                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeTrue("Generate Rationale button should be disabled for No Action?");
                    _providerAction.SelectReasonCode(reasonCode);
                    _providerAction.GetDecisionRationaleText()
                        .ShouldBeEqual(reasonCode, "Decision Rationale Box Text should equal to Reason Code");
                    _providerAction.SelectAction("Deny");

                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button should not be disabled for Deny?");

                    _providerAction.ClickOnGenerateRationaleButton();
                    _providerAction.IsGenerateRationaleInputFieldPresentByLabel("Billed Referral Exposure")
                        .ShouldBeTrue("Billed Referral Expouse input field present?");
                    _providerAction.IsGenerateRationaleInputFieldPresentByLabel("Paid Referral Exposure")
                        .ShouldBeTrue("Paid Referral Exposuree input field present?");
                    _providerAction.InsertDecisionRationale(summary);
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Billing Summary", billingSummary);
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("License #", license);
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Provider Website", providerWebsite);


                    _providerAction.ClickOnFinishButton();
                    _providerAction.IsValidationNoticeModalPopupPresent()
                        .ShouldBeTrue("Is Validation Modal popup present?");
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual(
                            "Billed Referral Exposure should be greater than $0.00.\r\nPaid Referral Exposure should be greater than $0.00.",
                            "Validation Message for Billed and Paid are empty");
                    _providerAction.CloseValidationNoticePopup();
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure", billed);
                    _providerAction.ClickOnFinishButton();
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual("Paid Referral Exposure should be greater than $0.00.",
                            "Paid Empty Validation");
                    _providerAction.CloseValidationNoticePopup();

                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure", "");
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure", paid);
                    _providerAction.ClickOnFinishButton();
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual("Billed Referral Exposure should be greater than $0.00.",
                            "Billed Empty Validation");
                    _providerAction.CloseValidationNoticePopup();

                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure",
                        billed + "abc!@#");
                    //_providerAction.ClickOnFinishButton();
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual("Only numbers allowed.", "Billed Non-numeric Validation");
                    _providerAction.CloseValidationNoticePopup();
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure", billed);

                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure",
                        paid + "abc!@#");
                    //_providerAction.ClickOnFinishButton();
                    _providerAction.GetValidationNoticePopupMessage()
                        .ShouldBeEqual("Only numbers allowed.", "Paid Non-numeric Validation");
                    _providerAction.CloseValidationNoticePopup();
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure", paid);


                    _providerAction.ClickOnFinishButton();


                    _providerAction.ClickOnCancelActionCondition();
                    
                }
            }
        }


        [Test, Category("NewProviderAction1")] //US44852
        public void
            Verify_that_codes_of_concern_should_display_for_SLAB_condition_if_MASTER_RULE_is_true_in_table_suspect_provider_rules
            ()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var procCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProcedureCode", "Value");
                var conditionId = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ConditionId", "Value");
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SearchConditionByCodeAndClickOnFirstResult(beginCode: procCode);
                    _providerAction.ClickOnMatchingConditionByConditionId(conditionId);
                    _providerAction.ClickOnCancelActionCondition();
                    _providerAction.GetCodeofCodesofConcernCount()
                        .ShouldBeGreater(0, "Codes of Concern List Displayed for SLAB Condition Id");
                    _providerAction.ClickOnCancelActionCondition();
                    
                }
            }
        }

        [Test, Category("NewProviderAction1")] //US45704
        public void Verify_presence_of_stpx_procedure_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;
                    try
                    {
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
                    finally
                    {
                        automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }
            }
        }


        [Test, Category("NewProviderAction2")] //US48436
        public void Verify_presence_of_condition_note_in_condition_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var emptyMessage = testData["EmptyMessage"];
                var sysGeneratedMessage = testData["SysGeneratedMessage"];

                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    try
                    {
                        _providerAction.SelectFilterConditions(3);
                        _providerAction.ClickOnConditionDetailsIcon();
                        _providerAction.SelectProviderCondition(1);
                        _providerAction.GetEmptyConditionNote()
                            .ShouldBeEqual(emptyMessage,
                                "If there is no system generated condition note, 'No data available' message is displayed");
                        _providerAction.SelectProviderCondition(2);
                        _providerAction.GetConditionNoteInConditionDetail()
                            .ShouldBeEqual(sysGeneratedMessage,
                                "System generated message is displayed");
                    }
                    finally
                    {
                        automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }
            }
        }


        [Test, Category("NewProviderAction2")] //US48439
        public void Verify_retrigger_deselects_disables_on_review_deny_as_action_and_clears_on_cancel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var reTriggerPeriod = testData["ReTriggerPeriod"];
                var reasonCode = testData["ReasonCode"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("No Action");
                    _providerAction.GetActiveRetriggerPeriodValue()
                        .ShouldBeEqual(reTriggerPeriod,
                            string.Format("{0} Should Selected by default", reTriggerPeriod));
                    _providerAction.SelectAction("Review");
                    _providerAction.IsRetriggerTimePeriodDisbaled()
                        .ShouldBeTrue("Retrigger is disabled when Review is selected as the action ?");
                    _providerAction.IsRetriggerPeriodSelected()
                        .ShouldBeFalse("Retrigger value should be cleared and deselected");
                    _providerAction.SelectAction("Deny");
                    _providerAction.IsRetriggerTimePeriodDisbaled()
                        .ShouldBeTrue("Retrigger is disabled when Deny is selected as the action ?");
                    _providerAction.IsRetriggerPeriodSelected()
                        .ShouldBeFalse("Retrigger value should be cleared and deselected");
                    _providerAction.SelectAction("No Action");
                    _providerAction.SelectReTriggerPeriod("7");

                    _providerAction.SelectReasonCode(reasonCode);
                    _providerAction.GetDecisionRationaleText()
                        .ShouldBeEqual(reasonCode, "Reason Code Should Stored after Reason Code is selected");
                    _providerAction.ClickOnCancelActionCondition();
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.IsRetriggerPeriodSelected()
                        .ShouldBeFalse("Retrigger value should be cleared and deselected");
                    _providerAction.IsActionSelectedInComboBox().ShouldBeFalse("Action selected by default");
                    _providerAction.GetReasonCodeDropDrownValue()
                        .ShouldBeEqual("Select Reason Code", "User Required to Select Reason Code");
                    _providerAction.GetDecisionRationaleTextForCotivitiUser()
                        .ShouldBeNullorEmpty("Decision rationale value cleared on cancel");
                }
                finally
                {
                    automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }


        [Test, Category("NewProviderAction2"), Category("Working")] //us48441, us48441
        public void Verify_respective_icon_displayed_with_provider_profile_icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeqMedReview = testData["ProviderSequenceMedReview"];
                var provSeqMed = testData["ProviderSequenceMed"];
                var provSeqReview = testData["ProviderSequenceReview"];
                var provSeq = testData["ProviderSequence"];
                var medAlert = testData["MedAlertToolTip"];
                var profileView = testData["ProfileView"];
                //var onReview = testData["OnReview"];

                StringFormatter.PrintMessageTitle("Provider with Red Exclamation Point");
                CheckforProfileTooltipTitle(provSeqMed, medAlert, true, _providerSearch, automatedBase);
                StringFormatter.PrintMessageTitle("Provider with Red Exclamation and Eyeball");
                CheckforProfileTooltipTitle(provSeqMedReview, medAlert, true,  _providerSearch, automatedBase, true);
                StringFormatter.PrintMessageTitle("Provider with Only Eyeball");
                CheckforProfileTooltipTitle(provSeqReview, profileView, false,  _providerSearch, automatedBase, true);
                StringFormatter.PrintMessageTitle("Provider withouth Red Exclamation and Eyeball");
                CheckforProfileTooltipTitle(provSeq, profileView, false,  _providerSearch, automatedBase);
            }
        }

        [Test, Category("NewProviderAction2")] //US48446
        public void Verify_triggered_trail_action_is_unique_to_batchseq()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnConditionDetailsIcon();
                    _providerAction.SelectProviderCondition(2);
                    var conditionDetailsEmtpy = _providerAction.GetConditionDetailsEmptyRecordMessage();
                    _providerAction.SelectProviderCondition(4);
                    conditionDetailsEmtpy.ShouldNotBeEqual(_providerAction.GetConditionDetailsRow(1),
                        "Audit information unique to batch sequence");
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }


        [Test, Category("NewProviderAction2")] //US48440 + TANT -3
        public void Verify_codes_of_concern_are_displayed_in_condition_details_audit_trail()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var actionDate = testData["ActionDate"];
                var action = testData["Action"];
                var userType = testData["User Type"];
                var actionedBy = testData["Actioned By"];
                var conditionCode = testData["ConditionCode"];
                var reasonCode = testData["ReasonCode"];
                var reasonDescription = testData["ReasonDescription"];
                var customCod = testData["CustomCodes"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                automatedBase.CurrentPage = _providerAction;

                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnConditionDetailsIcon();
                    _providerAction.SelectProviderConditionByConditionId("SEMH");
                    _providerAction.IsProviderConditionSelectedActive("SEMH")
                        .ShouldBeTrue("Selected condition outlined in blue");

                    var conditionDetailsRow = _providerAction.GetConditionDetailsRowSelector(1, 1);
                    conditionDetailsRow = conditionDetailsRow + _providerAction.GetConditionDetailsRowSelector(1, 2);
                    conditionDetailsRow.Contains(String.Format("{0}", conditionCode))
                        .ShouldBeTrue("4 digit condition code is present");
                    conditionDetailsRow.Contains(String.Format("Action Date:\r\n{0}", actionDate))
                        .ShouldBeTrue("Action Date is present");
                    conditionDetailsRow.Contains(String.Format("Action:\r\n{0}", action))
                        .ShouldBeTrue("Action is present");
                    conditionDetailsRow.Contains(String.Format("User Type:\r\n{0}", userType))
                        .ShouldBeTrue("User Type is present");
                    conditionDetailsRow.Contains(String.Format("Actioned By:\r\n{0}", actionedBy))
                        .ShouldBeTrue("Actioned By is present");
                    conditionDetailsRow.Contains(String.Format("Reason:\r\n{0} - {1}", reasonCode, reasonDescription))
                        .ShouldBeTrue("Reason code and description is present");

                    var firstRowDate =
                        Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(1));
                    var secondRowDate =
                        Convert.ToDateTime(_providerAction.GetActionDateStringFromConditionDetailsRow(2));
                    (firstRowDate > secondRowDate).ShouldBeTrue("Records are in descending order of Action Date");
                    var masterCodesRow = _providerAction.GetConditionDetailsCodeOfConcernRow(2);
                    masterCodesRow.Contains(String.Format("Codes of Concern:\r\n{0}", "Master Codes Applied"))
                        .ShouldBeTrue("Checking master codes applied is present");

                    _providerAction.SelectProviderConditionByConditionId("SCAC");
                    var customCodesList = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    customCodesList.Contains(String.Format("Codes of Concern:\r\n{0}", "Custom Codes Applied"))
                        .ShouldBeTrue("Checking Custom codes applied is present");
                    customCodesList.Contains("...")
                        .ShouldBeTrue("Checking Custom codes exceeding 25 has an ellipsis applied is present");
                    string[] codeList =
                        BetweenStringSplitter(customCodesList, "Applied\r\n", "\r\n...")
                            .Split(new string[] {"\r\n"}, StringSplitOptions.None);
                    codeList.ShouldCollectionBeSorted(false, "Custom codes are sorted in ascending order");
                    _providerAction.SelectProviderConditionByConditionId("SBRD");
                    var customCodes = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    customCodes.Replace("\r\n", string.Empty)
                        .Contains(String.Format("Codes of Concern:{0}", customCod))
                        .ShouldBeTrue("Checking Custom codes applied is present");
                    _providerAction.SelectProviderConditionByConditionId("SIJF");
                    var noProc = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    _providerAction.CheckForProviderConditionInSuspectProviderRulesTable("SIJF")
                        .ShouldBeTrue(
                            "Condition should not exist in table for the message 'See referral notes'  to display");
                    noProc.Contains(String.Format("Codes of Concern:\r\n{0}", testData["NoProcCode"]))
                        .ShouldBeTrue("Checking no proc associated message is present");

                    _providerAction.SelectProviderConditionByConditionId("SFJT");
                    var exceedProc = _providerAction.GetConditionDetailsCodeOfConcernRow(1);
                    exceedProc.Contains(String.Format("Codes of Concern:\r\n{0}", testData["ExceedProcCode"]))
                        .ShouldBeTrue("checking message for exceeding proc code is present");

                    _providerAction.SelectProviderConditionByConditionId("SMID");
                    _providerAction.IsConditionDetailAuditRowPresent()
                        .ShouldBeFalse("Condition  Details Audit Row Should Blank");
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }

        [Test, Category("NewProviderAction2")] //US50626
        public void Verify_clear_on_codes_of_concern_does_not_return_error()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var conditionCode = testData["ProviderConditionCode"];
                var beginCode = testData["BeginCode"];
                var endCode = testData["EndCode"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                automatedBase.CurrentPage = _providerAction;

                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    if (!_providerAction.IsUserAddedConditionSectionPresent())
                        _providerAction.ClickOnSearchCondition();
                    _providerAction.SearchConditionByCodeAndClickOnFirstResult(conditionCode, beginCode, endCode);
                    _providerAction.ClickOnClearButtonInUserSpecifiedCondition();
                    _providerAction.GetValueofInputConditionCode()
                        .ShouldBeNullorEmpty("Clicking on Clear button clears search box contents");
                    _providerAction.GetValueofInputConditionCode(false)
                        .ShouldBeNullorEmpty("Clicking on Clear button clears search box contents");
                    _providerAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Clearing user added condition should not generate page error.");
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }

        [Test, Category("NewProviderAction2")] //US51981
        public void Verify_generate_rationale_additional_field_are_present_and_required()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var reasonCode = testData["ReasonCode"];
                var addedFields = testData["GenerateRationaleFields"].Split(';');
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    try
                    {
                        _providerAction.SelectFilterConditions(3);
                        _providerAction.ClickOnEditActionCondition();
                        _providerAction.SelectCodeOfConcernForActioning(1);
                        _providerAction.SelectAction("Review");
                        _providerAction.SelectReasonCode(reasonCode);
                        _providerAction.IsGenerateRationaleButtonDisabled()
                            .ShouldBeFalse("Generate Rationale button disabled for Review should be false. Is false?");
                        _providerAction.ClickOnGenerateRationaleButton();
                        var i = 1;
                        for (; i < requiredFields.Count(); i++)
                        {
                            _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
                        }

                        i = 0;
                        foreach (var field in addedFields)
                        {
                            _providerAction.IsGenerateRationaleInputFieldPresentByLabel(field)
                                .ShouldBeTrue(field + " input field present?");
                            if (requiredFields.Contains(field))
                            {
                                _providerAction.ClearGenerateRationaleFormInputFieldByLabel(field);
                                _providerAction.ClickOnFinishButton();
                                _providerAction.IsValidationNoticeModalPopupPresent()
                                    .ShouldBeTrue("Is Validation Modal popup present?");
                                if (field != "Billed Referral Exposure" && field != "Paid Referral Exposure")
                                    _providerAction.GetValidationNoticePopupMessage()
                                        .ShouldBeEqual(
                                            field + " is required.",
                                            "Validation Message for empty value in required field");
                                else
                                {
                                    _providerAction.GetValidationNoticePopupMessage()
                                        .ShouldBeEqual(
                                            field + " should be greater than $0.00.",
                                            "Validation Message for empty value in required field");
                                }

                                _providerAction.CloseValidationNoticePopup();
                                _providerAction.SetGenerateRationaleFormInputFieldByLabel(field, inputValue[i]);
                                i++;
                            }
                        }

                        _providerAction.ClickOnCancelOnGenerateRationale(true);
                        _providerAction.ClickOnCancelActionCondition();
                    }
                    finally
                    {
                        automatedBase.CurrentPage =
                            automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }
            }
        }

        [Test, Category("NewProviderAction2")] //US51983
        public void Verify_validation_unnecessary_for_FRA17_in_referral_exposure()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var actions = testData["Action"].Split(';');
                var reasonCode = testData["ReasonCode"];
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var allReasonCode =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "FraudReasonCodes").Values.ToList();
                allReasonCode.Remove(reasonCode);
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;

                    try
                    {
                        _providerAction.SelectFilterConditions(3);
                        _providerAction.ClickOnEditActionCondition();
                        _providerAction.SelectCodeOfConcernForActioning(1);
                        /*---------------------------Select action option 1 for defined reason code-----------------------------------*/
                        _providerAction.SelectAction(actions[0]);
                        _providerAction.SelectReasonCode(reasonCode);

                        _providerAction.ClickOnGenerateRationaleButton();
                        var i = 0;
                        for (; i < requiredFields.Count(); i++)
                        {
                            _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
                        }

                        _providerAction.GetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure")
                            .ShouldBeEqual("0", "Verifying billed referral Should zero value");
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure")
                            .ShouldBeEqual("0", "Verifying billed referral Should zero value");
                        _providerAction.ClickOnFinishButton();
                        _providerAction.IsValidationNoticeModalPopupPresent()
                            .ShouldBeFalse("Is Validation Modal popup present when referral is empty?");
                        /*---------------------------Select action option 2 for defined reason code-----------------------------------*/
                        _providerAction.SelectAction(actions[1]);


                        _providerAction.ClickOnGenerateRationaleButton();
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure")
                            .ShouldBeEqual("0", "Verifying billed referral Should have zero value");
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure")
                            .ShouldBeEqual("0", "Verifying billed referral Should have zero value");
                        _providerAction.ClickOnFinishButton();
                        _providerAction.IsValidationNoticeModalPopupPresent()
                            .ShouldBeFalse("Is Validation Modal popup present when referral is empty?");
                        foreach (var action in actions)
                        {
                            ForAllReasonCodeReferralValidation(allReasonCode, action, _providerAction);
                        }

                        _providerAction.ClickOnCancelActionCondition();
                    }
                    finally
                    {
                        automatedBase.CurrentPage =
                            automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }
            }
        }

        [Test, Category("NewProviderAction2")] //US53168
        public void Verify_generate_rationale_allow_tabbing_through_fields_without_deleting_values()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var reasonCode = testData["ReasonCode"];
                var addedFields = testData["GenerateRationaleFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);

                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("Review");
                    _providerAction.SelectReasonCode(reasonCode);
                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button disabled for Review should be false. Is false?");
                    _providerAction.ClickOnGenerateRationaleButton();
                    var i = 1;
                    for (; i < addedFields.Count(); i++)
                    {
                        switch (addedFields[i])
                        {
                            case "License Status":
                                _providerAction.SetLicenseStatus(inputValue[i]);
                                break;
                            case "User Rationale Summary":
                                _providerAction.SetUserRationaleSummary(inputValue[i]);
                                break;
                            default:
                                _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields[i],
                                    inputValue[i]);
                                break;
                        }
                    }

                    /* --- set focus to first input element ----*/
                    _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields[0], inputValue[0]);
                    for (i = 0;
                        i < addedFields.Count() - 2;
                        i++) /*exclude user rationale summary as element location issue with it.*/
                    {
                        Console.WriteLine("Checking if tab through {0} field from {1} is successful",
                            addedFields[i + 1],
                            addedFields[i]);
                        _providerAction.ClickOnTabKey(addedFields[i]);
                        _providerAction.CheckIfFocusIsInAnElement(addedFields[i + 1])
                            .ShouldBeTrue("Tab to " + addedFields[i + 1] + " field from " + addedFields[i] +
                                          " is successful and complete, must be true");
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[i + 1])
                            .ShouldBeEqual(inputValue[i + 1], "Tab key should not clear text field");

                    }

                    _providerAction.ClickOnCancelOnGenerateRationale(true);
                    automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();

                }
                finally
                {
                    if (!automatedBase.CurrentPage.Equals(typeof(ProviderSearchPage)))
                    {
                        automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }

            }
        }


        [Test, Category("NewProviderAction2")] //US51988
        public void Verify_presence_of_icon_for_condition_requiring_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var provSeq = testData["ProviderSequence"];

                var vAction = testData["vAction"];
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                _providerAction.SelectFilterConditions(3);
                for (var i = 1; i <= _providerAction.GetProviderConditionsCount(); i++)
                {
                    var actionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(vAction, i);
                    if (actionSelected.Equals("Required"))
                        _providerAction.IsProviderConditionRowHaveActionRequiredBadge(i)
                            .ShouldBeTrue("Action required badge icon present should be true for action: " +
                                          actionSelected);
                    else
                        _providerAction.IsProviderConditionRowHaveActionRequiredBadge(i)
                            .ShouldBeFalse("Action required badge icon present should be false for action: " +
                                           actionSelected);

                }

                automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
            }
        }

        [Test, Category("NewProviderAction2")] //us57575 , //us53167
        public void Verify_input_text_validation_of_Provider_info_for_new_added_fields_of_generate_rationale()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                //new added fields=Proider Tin, Taxonomy Code
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testValue = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testValue["ProviderSequence"];
                var reasonCode = testValue["ReasonCode"];
                var defaultValue = testValue["DefaultValue"].Split(';').ToList();
                var validationInputValue = testValue["ValidationInputValue"].Split(';');
                var addedFields = testValue["GenerateRationaleFields"].Split(';');
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("Review");
                    _providerAction.SelectReasonCode(reasonCode);
                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button disabled for Review should be false.");
                    _providerAction.ClickOnGenerateRationaleButton();
                    _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[0])
                        .ShouldBeEqual(defaultValue[0], "Provider name default value is populated");
                    _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[1])
                        .ShouldBeEqual(provSeq, "Provider seq default value has been populated");

                    _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[2])
                        .ShouldBeNullorEmpty("Other Website has no value populated as expected");
                    _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[3])
                        .ShouldBeEqual(defaultValue[1], "Provider tin default value populated");
                    _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[4])
                        .ShouldBeNullorEmpty("Taxonomy Code has no value populated as expected");

                    StringFormatter.PrintMessageTitle("Verify Provider Name and Provider Sequence and Other website");
                    for (var i = 0; i < addedFields.Count() - 2; i++)
                    {
                        ValidationForNcharactersAndAlphanumeric(addedFields[i], validationInputValue[0], 100, _providerAction);
                    }

                    StringFormatter.PrintMessageTitle("Verify Provider Tin and Taxonomy Code");
                    ValidationForNcharactersAndAlphanumeric(addedFields[3], validationInputValue[0], 9, _providerAction); //prov tin
                    ValidationForNcharactersAndAlphanumeric(addedFields[4], validationInputValue[0],
                        50, _providerAction); //taxonomy code
                    _providerAction.ClickOnCancelOnGenerateRationale(true);
                    _providerAction.ClickOnCancelActionCondition();
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                }
            }
        }

        [Test, Category("NewProviderAction1")] //us51984+US57585
        public void Verify_validation_of_fields_of_generate_rationale()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var provSeq = testData["ProviderSequence"];
                var reasonCode = testData["ReasonCode"];
                var addedFields = testData["GenerateRationaleFields"].Split(';');
                var labelForDefaultValue = testData["LabelForDefaultValue"].Split(';');
                var labelForDollarValue = testData["LabelForDollarValue"].Split(';');
                var labelFor50AlphanumericCharacter = testData["LabelFor50AlphanumericCharacter"].Split(';');
                var defaultValue = testData["DefaultValue"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var licenseStatus = testData["LicenseStatus"].Split(';');
                const string validationInputValue = "text12!@#";
                var multiValdiation = testData["MultipleValidation"];
                _providerSearch.SearchByProviderSequence(provSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;
                    try
                    {
                        _providerAction.SelectFilterConditions(3);
                        _providerAction.ClickOnEditActionCondition();
                        _providerAction.SelectCodeOfConcernForActioning(1);
                        _providerAction.SelectAction("Review");
                        _providerAction.SelectReasonCode(reasonCode);
                        _providerAction.ClickOnGenerateRationaleButton();
                        _providerAction.ClickOnFinishButton();
                        _providerAction.IsValidationNoticeModalPopupPresent()
                            .ShouldBeTrue("Valdiation in case of empty required fields is present");
                        _providerAction.GetValidationNoticePopupMessage().Replace("\r\n", " ")
                            .ShouldBeEqual(multiValdiation,
                                "Validation Message for multiple empty value in required field is as expected");
                        _providerAction.ClosePageError();
                        string billsummary = new string('a', 4001);

                        _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields[0],
                            billsummary, true);
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[0])
                            .Length.ShouldBeEqual(4000, "Billing Summary accepts up to 4000 characters");

                        _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields[0], validationInputValue);
                        _providerAction.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse(
                                "The billing summary is a text field. Error generated when input is any character should be false.");

                        Console.WriteLine("Verify Dollar form input field");
                        foreach (var label in labelForDollarValue)
                        {
                            Console.WriteLine("Verify dollar format for <{0}>", label);
                            IsDollarAmountInCorrectFormat(
                                _providerAction.GetGenerateRationaleFormInputFieldByLabel(label), _providerAction);

                            SetInputGetExpectedError(label, "c", label + " is not a number and value is editable too.", _providerAction);
                        }

                        var i = 0;
                        for (; i < labelForDefaultValue.Length; i++)
                        {
                            _providerAction.GetGenerateRationaleFormInputFieldByLabel(labelForDefaultValue[i])
                                .ShouldBeEqual(defaultValue[i],
                                    string.Format("{0} is populated with default value.", labelForDefaultValue[i]));
                        }

                        foreach (var label in labelFor50AlphanumericCharacter)
                        {
                            ValidationForNcharactersAndAlphanumeric(label, validationInputValue, 50, _providerAction);
                        }


                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[10])
                            .ShouldBeEqual(licenseStatus[4], "Default license status is NA");

                        _providerAction.GetLicenseStatusOptions()
                            .ShouldCollectionBeEqual(licenseStatus, "Expected license options is discovered");


                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[13])
                            .IsDateInFormat()
                            .ShouldBeTrue("Review from Date is in mm/dd/yyy format");
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[14])
                            .IsDateInFormat()
                            .ShouldBeTrue("Review to Date is in mm/dd/yyy format");

                        //TE-469
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[13])
                            .ShouldBeEqual(DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy"),
                                "Date in the Review From Field Should be populated to exactly one year before");
                        _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[14])
                            .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                                "Date in the Review To Field Should be populated to today's date");

                        _providerAction.SetDateByCalendraPicker(addedFields[13],
                            DateTime.Now.ToString("MM/d/yyyy")); //check for date picker 
                        _providerAction.SetDateByCalendraPicker(addedFields[14], DateTime.Now.ToString("MM/d/yyyy"));

                        for (i = 15; i < 18; i++)
                        {
                            //occurence,total patients, patients involved
                            SetInputGetExpectedError(addedFields[i], "c",
                                addedFields[i] + " is not a number and value is editable too.", _providerAction);
                        }

                        for (i = 0; i < addedFields.Count(); i++)
                        {
                            switch (addedFields[i])
                            {
                                case "License Status":
                                    _providerAction.SetLicenseStatus(inputValue[i]);
                                    break;
                                case "User Rationale Summary":
                                    _providerAction.SetUserRationaleSummary(inputValue[i]);
                                    break;
                                default:
                                    _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields[i],
                                        inputValue[i]);
                                    break;
                            }
                        }

                        _providerAction.ClickOnCancelOnGenerateRationaleOnly();
                        _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Warning Message");
                        _providerAction.GetConfirmationMessage()
                            .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to continue?");
                        _providerAction.ClickOkCancelOnConfirmationModal(false);

                        StringFormatter.PrintMessageTitle("Changes in inpupt field should retain");
                        for (i = 0; i < addedFields.Length; i++)
                        {
                            _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields[i])
                                .ShouldBeEqual(inputValue[i]);
                        }

                        _providerAction.ClickOnCancelOnGenerateRationale(true);


                    }
                    finally
                    {
                        _providerAction.CloseAnyPopupIfExist(string.Empty);
                        automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                    }
                }
            }
        }


        [Test, Category("NewProviderAction2")] //us53169 (story broken to US58908 awaiting fix for proc codes of concern)
        // [Test,Category("Acceptance")]//US68677 [modified test as per the user story]
        public void Test_the_final_generate_rationale_in_action_condition_for_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var prvSeq = testData["ProviderSequence"];
                var reasonCode = testData["ReasonCode"];
                var addedFields = testData["GenerateRationaleFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var requiredFields = testData["RequiredFields"].Split(';');
                var reqValue = testData["ReqValue"].Split(';');
                var inputEmptyValue = testData["InputEmptyValue"].Split(';');
                var finalRationaleReqFields = testData["FinalRationaleReqFields"].Split(';');
                var finalReqFieldValue = testData["FinalReqFieldValue"].Split(';');
                var finalRationaleNonReqFields = testData["FinalRationaleNonReqFields"].Split(';').ToList();
                var finalNonReqFieldValue = testData["FinalNonReqFieldValue"].Split(';');
                var currentUserFullName = testData["UserFullName"];
                var date = testData["Date"];
                //var updatedNoteText = testData["UpdatedNoteText"];
                const string staticText = "There are no license sanctions.";

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForCotiviti(prvSeq, date);
                _providerSearch.UpdateTriggeredConditionForCotiviti(prvSeq);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                if (_providerAction != null)
                {
                    automatedBase.CurrentPage = _providerAction;
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);
                    _providerAction.SelectAction("Review");
                    _providerAction.SelectReasonCode(reasonCode);
                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button disabled for Review should be false. Is false?");
                    _providerAction.IsNoteTextAreaEditable().ShouldBeFalse("Note text area should be disabled");
                    _providerAction.ClickOnGenerateRationaleButton();
                    /*--------------- Check for required fields first and test for final finished rationale ----*/
                    SetValueForGenerateRationale(requiredFields, reqValue, _providerAction);
                    SetValueForGenerateRationale(addedFields, inputEmptyValue, _providerAction);
                    _providerAction.ClickOnFinishButton();
                    _providerAction.WaitForWorkingAjaxMessage();
                    _providerAction.WaitForStaticTime(4000);
                    _providerAction.IsNoteTextAreaEditable().ShouldBeTrue("Note text area should be editable");
                    var finalDecisionRationale =
                        _providerAction.GetDecisionRationaleText().Replace("\r\n", " ");

                    finalDecisionRationale.Contains(staticText)
                        .ShouldBeTrue("Static text is as present.");
                    var i = 0;
                    /*------checking if  req fields are  present in final rationale -----*/
                    for (; i < finalRationaleReqFields.Count(); i++)
                    {

                        finalDecisionRationale.Contains(finalRationaleReqFields[i] + ": " + finalReqFieldValue[i])
                            .ShouldBeTrue("Final decision rationale generated for " + finalRationaleReqFields[i] +
                                          ": " +
                                          finalReqFieldValue[i] +
                                          " is as expected should be true.");
                        if (finalRationaleReqFields[i].Contains("Provider triggered"))
                            finalReqFieldValue[i].IsConditionCodeInCorrectFormat()
                                .ShouldBeTrue("Condition code value should be displayed as <4char> - <descp> format");
                        switch (finalRationaleReqFields[i])
                        {

                            case "Provider specialty":
                                finalReqFieldValue[i].IsProviderSpecialtyInCorrectFormat()
                                    .ShouldBeTrue(
                                        "Provider Specialty value should be displayed in <2 digit specialty code> - <specialty short description> format");
                                break;
                            case "Provider website":
                                _providerAction.IsExpectedHyperLinkPresent(finalReqFieldValue[i])
                                    .ShouldBeTrue("Provider Website value should be displayed as a hyperlink");
                                break;

                        }
                    }

                    /*------checking if non req fields are not present in final rationale when those fields are empty-----*/
                    for (i = 0; i < finalRationaleNonReqFields.Count(); i++)
                    {
                        finalDecisionRationale.Contains(finalRationaleNonReqFields[i] + ":")
                            .ShouldBeFalse("In final rationale, Non required empty field " +
                                           finalRationaleNonReqFields[i] +
                                           " is not left off should be false.");
                    }

                    _providerAction.ClickOnGenerateRationaleButton();
                    _providerAction.WaitForWorkingAjaxMessage();
                    /*----Setting values for all fields ----*/
                    SetValueForGenerateRationale(addedFields, inputValue, _providerAction);
                    _providerAction.ClickOnFinishButton();
                    _providerAction.WaitForWorkingAjaxMessage();
                    _providerAction.WaitForStaticTime(4000);
                    _providerAction.IsNoteTextAreaEditable().ShouldBeTrue("Note text area should be editable");
                    finalDecisionRationale =
                        _providerAction.GetDecisionRationaleText().Replace("\r\n", " ");
                    /*------checking if non req fields are present in final rationale when those fields have value-----*/
                    for (i = 0; i < finalRationaleNonReqFields.Count(); i++)
                    {
                        finalDecisionRationale.Contains(finalRationaleNonReqFields[i] + ": " + finalNonReqFieldValue[i])
                            .ShouldBeTrue("Final decision rationale generated as " + finalRationaleNonReqFields[i] +
                                          ": " +
                                          finalNonReqFieldValue[i] +
                                          " is as expected should be true.");
                        switch (finalRationaleNonReqFields[i])
                        {
                            case "Board website":
                                _providerAction.IsExpectedHyperLinkPresent(finalNonReqFieldValue[i])
                                    .ShouldBeTrue("Board Website value should be displayed as a hyperlink");
                                break;
                            case "Other websites":
                                _providerAction.IsExpectedHyperLinkPresent(finalNonReqFieldValue[i])
                                    .ShouldBeTrue("Other Website value should be displayed as a hyperlink");
                                break;
                            case "Review Period":
                                finalNonReqFieldValue[i].IsDatePeriodInFormat()
                                    .ShouldBeTrue(
                                        "Review Period value should be displayed in <MM/DD/YYYY - MM/DD/YYYY> format");
                                break;
                            case "Percentage of patients involved":
                                int indexA = finalRationaleNonReqFields.FindIndex(a => a.StartsWith("Total patients"));
                                int indexB =
                                    finalRationaleNonReqFields.FindIndex(a => a.StartsWith("Number of patients"));
                                double totalPat = Int32.Parse(finalNonReqFieldValue[indexA]);
                                double patInv = Int32.Parse(finalNonReqFieldValue[indexB]);
                                var percCount = Convert.ToInt32(Math.Round(patInv / totalPat * 100,
                                    MidpointRounding.AwayFromZero));
                                (percCount + "%").ShouldBeEqual(finalNonReqFieldValue[i],
                                    "Percentage of patiends involved is as expected");
                                break;

                        }
                        //  if(new[] { "Board Website" , "Provider Website"}.Contains(finalRationaleNonReqFields[i])) _providerAction.IsExpectedHyperLinkPresent(finalNonReqFieldValue[i]).ShouldBeTrue("Wesite value should be displayed as a hyperlink");

                    }

                    const string testDecisionRationale = "test decision rationale for editable access ";
                    _providerAction.FillDecisionRationaleNote(testDecisionRationale);
                    _providerAction.IsNoteTextAreaEditable().ShouldBeTrue("Note text area shpould be editable");
                    finalDecisionRationale = _providerAction.GetDecisionRationaleText().Replace("\r\n", " ");
                    finalDecisionRationale.Contains(testDecisionRationale);
                    _providerAction.ClickOnSaveActionCondition();
                    _providerAction.WaitForWorkingAjaxMessage();

                    try
                    {
                        _providerAction.ClickonProviderNotes();
                        _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUserFullName);
                        _providerAction.WaitForStaticTime(2000);
                        var providerNotes = _providerAction.NotePage.GetNoteInNoteEditorByName(currentUserFullName)
                            .Replace("\r\n", " ");
                        providerNotes.ShouldBeEqual(finalDecisionRationale,
                            "Saved Generate rationale is populated in provider notes");
                        _providerAction.ClickonProviderNotes();
                        _providerAction.SelectFilterConditions(3);
                        _providerAction.SelectProviderConditionByConditionId("SMID");
                        _providerAction.ClickonProviderNotesinConditionExposureSection();
                        _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                        var providerNotesInConditionNotes =
                            _providerAction.GetNoteInConditionNotesByRow(1).Replace("\r\n", " ");
                        providerNotesInConditionNotes.ShouldBeEqual(finalDecisionRationale,
                            "Generate rationale note should be present in Condition Notes");
                        _providerAction.ClickOnCollapseIconOnNotesByRow(1);

                    }
                    finally
                    {
                        _providerAction.CloseAnyPopupIfExist(string.Empty);
                        automatedBase.CurrentPage =
                            automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();

                    }
                }
            }
        }

        [Test, Category("NewProviderAction2")] //US57577 + TE-673
        public void Validate_cancelling_action_to_generate_rationale_performs_cancel_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var prvSeq = testData["ProviderSequence"];
                var requiredFields = testData["RequiredFields"].Split(';');
                var inputValue = testData["InputValue"].Split(';');
                var updateText = testData["UpdateText"];
                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                try
                {
                    _providerAction.SelectFilterConditions(3);
                    _providerAction.ClickOnEditActionCondition();
                    _providerAction.SelectCodeOfConcernForActioning(1);

                    _providerAction.SelectAction("Review");
                    _providerAction.IsGenerateRationaleButtonDisabled()
                        .ShouldBeFalse("Generate Rationale button disabled for Review should be false. Is false?");
                    _providerAction.IsNoteTextAreaEditable()
                        .ShouldBeFalse("Decision rationale text box should not be editable");
                    _providerAction.ClickOnGenerateRationaleButton();

                    _providerAction.WaitForCondition(() => _providerAction.IsNoteTextAreaEditable());
                    _providerAction.WaitToLoadUserRationaleSummary();
                    _providerAction.ClickOnCancelOnGenerateRationale();
                    _providerAction.IsNoteTextAreaEditable()
                        .ShouldBeFalse("Decision rationale text box should not be editable");

                    _providerAction.ClickOnGenerateRationaleButton();
                    for (int i = 0; i < requiredFields.Count(); i++)
                    {
                        _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
                    }

                    _providerAction.ClickOnCancelOnGenerateRationale();
                    _providerAction.GetDecisionRationaleTextForCotivitiUser()
                        .ShouldBeNullorEmpty("Decision rationale should be empty so no unintended changes are made.");
                    _providerAction.IsNoteTextAreaEditable()
                        .ShouldBeFalse("Decision rationale text box should not be editable");

                    _providerAction.ClickOnGenerateRationaleButton();
                    _providerAction.WaitToLoadUserRationaleSummary();
                    _providerAction.ClickOnFinishButton();
                    _providerAction.IsNoteTextAreaEditable()
                        .ShouldBeTrue("Decision rationale text box should be editable");
                    _providerAction.GetDecisionRationaleText()
                        .Contains(inputValue[1])
                        .ShouldBeTrue("Most recent changes reflected in decision rationale.");

                    _providerAction.FillDecisionRationaleNote(updateText);
                    _providerAction.GetDecisionRationaleText()
                        .Contains(updateText)
                        .ShouldBeTrue("Most recent changes reflected in decision rationale.");

                    _providerAction.ClickOnCancelActionCondition();
                }
                finally
                {
                    automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();

                }
            }
        }

        [Test, Category("NewProviderAction2")] //US58219
        public void Verify_whether_action_condition_icon_is_enabeld_or_disabeld_for_respective_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var provSeq = testData["ProviderSequence"];

                var actionUser = testData["ActionUser"].Split(';');
                var action = testData["action"].Split(';');
                _providerSearch.SearchByProviderSequence(provSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
                for (var i = 1; i <= _providerAction.GetProviderConditionsCount(); i++)
                {
                    var vActionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(actionUser[0], i);
                    var cActionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(actionUser[1], i);
                    if (vActionSelected.Equals(action[3]))
                        cActionSelected.Equals(action[4])
                            .ShouldBeTrue(
                                "For conidtions where Cotiviti action is NoAction , Client action will equal to Not required should be true.");

                    Console.WriteLine("Checking for action condition icon status");
                    if (vActionSelected.Equals(action[0]) && cActionSelected.Equals(action[0]))
                        _providerAction.IsProvCondListEditActionConditionEnabled(i)
                            .ShouldBeTrue(
                                "When both actions are required, action codition icon's enabled status should be true.");
                    if ((vActionSelected.Equals(action[1]) || vActionSelected.Equals(action[2])) &&
                        cActionSelected.Equals(action[0]))
                        _providerAction.IsProvCondListEditActionConditionEnabled(i)
                            .ShouldBeTrue(
                                "When cotiviti actions is Review/deny and client action equals required, action codition icon's enabled status should be true.");
                    if (vActionSelected.Equals(action[3]) && cActionSelected.Equals(action[4]))
                        _providerAction.IsProvCondListEditActionConditionDisbaled(i)
                            .ShouldBeTrue(
                                "When cotiviti actions equal no Action and client action equals not required, action codition icon's disabled status should be true.");


                }

                automatedBase.CurrentPage = automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();
                ;


            }
        }



        [Test, Category("NewProviderAction1")] //US58219
        public void Verify_code_of_concern_for_STPX_condition()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var prvSeq = testData["ProviderSequence"];
                var procedureCode = testData["ProcedureCode"];
                var conditionId = string.Format(testData["ConditionId"], procedureCode);
                const string action = "Deny";

                _providerSearch.SearchByProviderSequence(prvSeq);

                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.SelectFilterConditions(3);
                StringFormatter.PrintMessageTitle("verify Code of concern in condition detail");
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.ClickOnConditionDetailsIcon();
                _providerAction.GetConditionDetailsCodeOfConcernRow(1)
                    .AssertIsContained(conditionId,
                        "code of concern should be in STPX-Procedure Combination<code> format");
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.IsLengthyMessageLabelPresent()
                    .ShouldBeFalse("message with gray container should not display");
                StringFormatter.PrintMessageTitle("verify Code of concern in Rationale Textarea");
                ActionCondition(_providerAction, reasonCode: "", action: action, note: "UITEST", procCodeList: null, save: false);
                Regex.Replace(_providerAction.GetDecisionRationaleText()
                        .Replace("\r\n", " "), @"\s{2,}", " ")
                    .AssertIsContained(string.Format("Procedure Codes of Concern: {0}", procedureCode),
                        "proced code associated with the STPX should display in the Procedure codes");
                _providerAction.ClickOnCancelActionCondition();
                _providerAction.WaitForWorkingAjaxMessage();
            }
        }

        [Test, Category("NewProviderAction2")] //US57578
        public void Verify_proc_codes_associated_with_SUSC_is_listed_under_condition_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var prvSeq = testData["ProviderSequence"];
                var procedureCode = testData["ProcedureCode"];
                var conditionId = testData["ConditionId"];
                var date = testData["Date"];

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForCotiviti(prvSeq, date);
                _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.SelectFilterConditions(3);
                var currAction = _providerAction.GetProviderConditionDetailForFieldAndRow("Cotiviti Action", 1);
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

        [Test, Category("NewProviderAction2")] //US68634
        public void
            Verify_that_the_add_notes_icon_will_be_displayed_on_Provider_Action_Page_if_notes_are_not_present_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var prvSeq = paramLists["ProviderSequence"];
                var prvSeqWithClaimNotesOnly = paramLists["ProviderSequenceWithClaimNotes"];

                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                _providerAction.NotePage.IsAddNoteIndicatorPresent().ShouldBeTrue("Is Add note indicator present");

                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.IsAddNoteFormPresent().ShouldBeTrue("Add Note Form should be present");
                _providerAction.NotePage.IsAddIconDisabled().ShouldBeTrue("Add Note Indicator should be disabled.");
                var noOfNotes = _providerAction.NotePage.GetNoteListCount();
                (noOfNotes > 0).ShouldBeFalse("The given provider sequence has notes should be false.");
                _providerAction.NotePage.ClickOnAddNoteCancelLink();
                _providerAction.NotePage.IsAddNoteFormPresent()
                    .ShouldBeFalse("Add Note Form should not be present after clicking the Cancel button");
                _providerAction.NotePage.IsNoteContainerPresent()
                    .ShouldBeFalse("Note Container should not be present");

                StringFormatter.PrintMessageTitle("Verify Add note for providers with claim notes only");
                automatedBase.CurrentPage = _providerSearch = _providerAction.NavigateToProviderSearch();

                _providerSearch.SearchByProviderSequence(prvSeqWithClaimNotesOnly);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch
                        .ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqWithClaimNotesOnly);
                _providerAction.NotePage.IsAddNoteIndicatorPresent().ShouldBeTrue("Is Add note indicator present");
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.GetNoteListCount()
                    .ShouldBeEqual(0,
                        "Provider should not contain Provider notes"); //default selection of Note Type is Provider
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "Claim");
                _providerAction.NotePage.GetNoteListCount()
                    .ShouldBeGreater(0, "Provider should contain Claim Notes ");
                
            }
        }

        [Test, Category("NewProviderAction2")] //US68634
        public void Verify_existing_provider_note_history_are_displayed_upon_note_icon_click()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                var prvsSeq = paramLists["ProviderSequence"];
                var prvSeqWithOnlyPrvNote = paramLists["ProviderSequenceWithOnlyProviderNote"];
                var expectedNoteTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Note_type").Values.ToList();
                var userNameList = paramLists["UserNameList"]
                    .Split(';')
                    .ToList();
                var currentUser = userNameList[0];
                var nonCurrentUser1 = userNameList[1];
                var nonCurrentUser2 = userNameList[2];
                var expectedOutputList = paramLists["ExpectedOutputList"]
                    .Split(';')
                    .ToList();
                var claim = expectedOutputList[0];
                var provider = expectedOutputList[1];
                var subType = expectedOutputList[2];
                var subTypeTrigCond = expectedOutputList[3];

                _providerSearch.SearchByProviderSequence(prvsSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvsSeq);

                var totalClaimNoteCount =
                    Convert.ToInt32(_providerAction.NotePage.TotalCountofProviderClaimNotesInternalUsers(prvsSeq));
                var totalProviderNoteCount =
                    Convert.ToInt32(_providerAction.NotePage.TotalCountofProviderNotes(prvsSeq));
                var totalNoteCount =
                    Convert.ToInt32(
                        _providerAction.NotePage.TotalCountofProviderAndProviderClaimNotesInternalUsers(prvsSeq));

                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.IsNoteContainerPresent()
                    .ShouldBeTrue("Note Container must display after clicking note icon.");

                StringFormatter.PrintMessageTitle(
                    "Verification of Note Type in Notes Container Header");
                _providerAction.NotePage.GetAvailableDropDownListInNoteType("Type")
                    .ShouldCollectionBeEqual(expectedNoteTypeList, "Note Type List");
                _providerAction.NotePage.GetDefaultValueOfNoteTypeOnHeader("Type")
                    .ShouldBeEqual("Provider", "Default selection of Note Type. ");
                _providerAction.NotePage.GetNoteListCount().ShouldBeEqual(totalProviderNoteCount,
                    "Notes list should display only provider notes");

                StringFormatter.PrintMessageTitle(
                    "Verification of Notes Record and its values when All Note Type selected");
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "All");
                _providerAction.NotePage.GetNoteListCount().ShouldBeEqual(totalNoteCount,
                    "Notes list should display both claim and only provider notes");
                var totalRow = _providerAction.NotePage.GetNoteListCount();
                _providerAction.NotePage.GetNoteRecordByRowColumn(2, totalRow - 3)
                    .ShouldBeEqual(claim, "Claim Note Present");
                _providerAction.NotePage.GetNoteRecordByRowColumn(2, totalRow - 2)
                    .ShouldBeEqual(provider, "Provider Note Present");
                _providerAction.NotePage.GetNoteRecordByRowColumn(3, totalRow - 2)
                    .ShouldBeEqual(subType, "Provider Note with sub type Referral Detail");
                _providerAction.NotePage.GetNoteRecordByRowColumn(3, totalRow - 1).ShouldBeEqual(subTypeTrigCond,
                    "Provider Note with sub type equal to condition for which that provider has triggered. ");
                _providerAction.NotePage.GetNoteRecordByRowColumn(4, totalRow - 2)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Created By Should be in <First Name><Last Name>");

                _providerAction.NotePage.IsPencilIconPresentByName(currentUser)
                    .ShouldBeTrue("Pencil Icon must be Present when creator of Note is user itself. ");
                _providerAction.NotePage.IsCarrotIconPresentByName(currentUser)
                    .ShouldBeFalse(
                        "Carrot Icon must not be Present when the note was created by the current user.");

                _providerAction.NotePage.IsCarrotIconPresentByName(nonCurrentUser1).ShouldBeTrue(
                    "Carrot Icon must be Present when the note was created by a user other than the current user.");
                _providerAction.NotePage.IsPencilIconPresentByName(nonCurrentUser1)
                    .ShouldBeFalse("Pencil Icon must not be Present when creator of Note is not user itself. ");
                _providerAction.NotePage.GetVisibleToClientTooltipInNotesList()
                    .ShouldBeEqual("Visible to Client", "Is Tooltip Equal?");

                StringFormatter.PrintMessageTitle(
                    "Verification of Search By Claim Notes");
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "Claim");
                var list = _providerAction.NotePage.GetNoteRecordListByColumn();
                var distinctList = list.Distinct().ToList();
                distinctList[0].ShouldBeEqual("Claim", "Only claim Notes should be displayed.");
                distinctList.Count.ShouldBeEqual(1, "Distinct List Length");
                list.Count.ShouldBeEqual(totalClaimNoteCount,
                    "Claim note type count equal to that in the database");

                StringFormatter.PrintMessageTitle(
                    "Verification of Search By Provider Notes");
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "Provider");
                list = _providerAction.NotePage.GetNoteRecordListByColumn();
                distinctList = list.Distinct().ToList();
                distinctList[0].ShouldBeEqual("Provider", "Only provider Notes should be displayed.");
                distinctList.Count.ShouldBeEqual(1, "Distinct List Length");
                list.Count.ShouldBeEqual(totalProviderNoteCount,
                    "Provider note type count equal to that in the database");

                StringFormatter.PrintMessageTitle("Verification Scrollbar");
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "All");
                _providerAction.NotePage.ClickOnExpandIconOnNotesByName(nonCurrentUser1);
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(nonCurrentUser1)
                    .ShouldBeTrue("Notes form must be expanded.");
                _providerAction.NotePage.ClickOnExpandIconOnNotesByName(nonCurrentUser2);
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(nonCurrentUser2)
                    .ShouldBeTrue("Notes form must be expanded.");
                _providerAction.NotePage.IsNoteEditFormDisplayedByName(nonCurrentUser1)
                    .ShouldBeTrue("Notes form in row 2 is still present.");
                _providerAction.NotePage.GetNoteEditFormCount()
                    .ShouldBeGreater(1, "User is able to view multiple forms");
                _providerAction.NotePage.IsVerticalScrollBarPresentInNoteSection()
                    .ShouldBeTrue(
                        "Scrollbar Should display in Notes Section when the list of note records extends out of the view");

                StringFormatter.PrintMessageTitle("Verification of Empty notes message.");
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                _providerSearch.SearchByProviderSequence(prvSeqWithOnlyPrvNote);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqWithOnlyPrvNote);
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.SelectNoteTypeInHeader("Type", "Claim");
                _providerAction.NotePage.GetEmptyNoteMessage().ShouldBeEqual("There are no notes available");

                StringFormatter.PrintMessageTitle(
                    "Click Notes Icon again to collapse Notes container");
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.IsNoteContainerPresent()
                    .ShouldBeFalse("Note Container must collapse after clicking note icon again.");

                _providerAction.IsSmallAddNoteIconPresent()
                    .ShouldBeFalse("Is Add a Note Icon next to Pre-pay present?");
                _providerAction.IsSmallViewNoteIconPresent()
                    .ShouldBeFalse("Is Add a Note Icon next to Pre-pay present?");
                
            }
        }

        [Test, Category("NewProviderAction2")] //US68651
        public void Verify_note_edit_and_view_functionality_provider_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;

                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var updatedNoteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "UpdatedNoteText", "Value");
                var userNameList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
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


                _providerAction.NotePage.SetNoteInNoteEditorByName(string.Empty, currentUser); //set empty note
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
                _providerAction.NotePage.SetNoteInNoteEditorByName(noteText, currentUser,false);
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
                _providerAction.NotePage.SetNoteInNoteEditorByName(updatedNoteText, currentUser);
                _providerAction.NotePage.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.NotePage.IsEmptyNoteWarning()
                    .ShouldBeFalse("Empty Note warning should remove after enter text in text area.");

                StringFormatter.PrintMessageTitle("Mod Date should show the latest date");
                int totalRow = _providerAction.NotePage.GetNoteListCount();
                _providerAction.NotePage.GetNoteRecordByRowColumn(5, totalRow - 3)
                    .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"), "Modified date in Note list must be updated.");

                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUser);
                _providerAction.NotePage.GetNoteInNoteEditorByName(currentUser)
                    .ShouldBeEqual(updatedNoteText, "Updated Note text displayed in notes form");

                StringFormatter.PrintMessageTitle("Verification of Cancel note");
                _providerAction.NotePage.SetNoteInNoteEditorByName("Cancel Note Text", currentUser);
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

        [Test, Category("NewProviderAction2")] //US68666 + US68999
        public void Verify_create_new_Provider_note()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var prvSeq = paramLists["ProviderSequence"];
                var currentUserFullName = paramLists["UserFullName"];
                var expectedNoteSubTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Note_SubType").Values.ToList();

                var conditions = _providerSearch.GetProviderTriggeredConditions(prvSeq);
                expectedNoteSubTypeList.AddRange(conditions);
                _providerSearch.DeleteProviderNotesOnly(prvSeq, automatedBase.EnvironmentManager.HciAdminUsername);

                 _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.ClickonProviderNotes();
                var noOfNotes = Convert.ToInt32(_providerAction.NotePage.GetNoteListCount());

                _providerAction.NotePage.IsAddNoteIndicatorPresent().ShouldBeTrue("Add Note Icon Present");
                _providerAction.NotePage.IsAddNoteFormPresent()
                    .ShouldBeTrue("Add Note Form must display after clicking note icon.");
                _providerAction.NotePage.IsAddIconDisabled().ShouldBeTrue("Add Icon Should be disabled");
                _providerAction.NotePage.IsVisibleToClientChecked().ShouldBeTrue("Visible To Client should be checked");
                _providerAction.NotePage.ClickVisibleToClient();
                _providerAction.NotePage.IsVisibleToClientChecked()
                    .ShouldBeFalse("Visible To Client should be unchecked");
                _providerAction.NotePage.ClickVisibleToClient();

                _providerAction.NotePage.GetNoteSubTypeList().ShouldCollectionBeEqual(expectedNoteSubTypeList,
                    "Correct Sub type list should be displayed.");
                _providerAction.NotePage.GetNoteType().ShouldBeEqual("Provider", "Type of Note should be <Provider>");
                _providerAction.NotePage.GetNameLabel()
                    .AssertIsContained(currentUserFullName, "Name of the current User should equal");
                _providerAction.NotePage.SelectNoteSubType("Alert");
                _providerAction.NotePage.ClickonAddNoteSaveButton();

                _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Pop up message should be present");
                _providerAction.ClosePageError();
                _providerAction.NotePage.IsEmptyNoteWarning().ShouldBeTrue("Note Indicator should be present");
                _providerAction.NotePage.GetEmptyNoteTooltipInEditNoteForm()
                    .ShouldBeEqual("Note is required before the record can be saved.", "Note tooltip warning message");

                var noteText = new string('a', 19994);
                _providerAction.NotePage.SetAddNote(noteText,false);
                _providerAction.GetPageErrorMessage()
                    .ShouldBeEqual("Note value is too long.", "Long note message should equal");
                _providerAction.ClosePageError();
                var l = _providerAction.NotePage.GetAddNote().Length;
                _providerAction.NotePage.GetAddNote().Length.ShouldBeEqual(19993, "Note Text max length");

                _providerAction.NotePage.SetAddNote("note");
                _providerAction.NotePage.ClickonAddNoteSaveButton();
                _providerAction.WaitForWorkingAjaxMessage();

                _providerAction.NotePage.IsAddNoteFormPresent().ShouldBeFalse("Is Add Note form opened?");
                _providerAction.IsAddNoteIconPresent().ShouldBeFalse("Is Add Note icon present after adding note?");
                _providerAction.IsViewNoteIconPresent().ShouldBeTrue("Is View Note icon present after adding note?");
                noOfNotes = noOfNotes + 1;
                _providerAction.NotePage.GetNoteListCount()
                    .ShouldBeEqual(noOfNotes, "Number of Notes present in Claim");
                _providerAction.NotePage.IsPencilIconPresentByRow(1).ShouldBeTrue("Is Edit Icon Present?");
                _providerAction.NotePage.GetNoteRecordByRowColumn(2, 1)
                    .ShouldBeEqual("Provider", "Correct Note Type is displayed");
                _providerAction.NotePage.GetNoteRecordByRowColumn(3, 1)
                    .ShouldBeEqual("Alert", "Correct Sub  Type is displayed");
                _providerAction.NotePage.GetNoteRecordByRowColumn(4, 1)
                    .ShouldBeEqual(currentUserFullName, "Correct User Name is displayed.");
                _providerAction.NotePage.GetNoteRecordByRowColumn(5, 1)
                    .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"), "Correct Date is displayed");
                _providerAction.NotePage.IsVisibletoClientIconPresentByRow(1)
                    .ShouldBeTrue("Is Visible to Client icon present?");

                StringFormatter.PrintMessageTitle("Add Condition Note");
                _providerAction.NotePage.ClickonAddNoteIcon();
                _providerAction.NotePage.SelectNoteSubType(conditions[0]);
                _providerAction.NotePage.SetAddNote("UI Note");
                if (_providerAction.NotePage.IsVisibleToClientChecked())
                    _providerAction.NotePage.ClickVisibleToClient();
                _providerAction.NotePage.ClickonAddNoteSaveButton();
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.NotePage.GetNoteListCount()
                    .ShouldBeEqual(noOfNotes + 1, "Number of Notes present in Claim");
                _providerAction.NotePage.IsVisibletoClientIconPresentByRow(1)
                    .ShouldBeFalse("Is Visible to  Client Icon Present?");

                StringFormatter.PrintMessageTitle("Add Without SubType Note");
                _providerAction.NotePage.ClickonAddNoteIcon();
                _providerAction.NotePage.SetAddNote("UI Note");
                _providerAction.NotePage.ClickonAddNoteSaveButton();
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.NotePage.GetNoteListCount()
                    .ShouldBeEqual(noOfNotes + 2, "Number of Notes present in Claim");
                _providerAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("There should not any popup message for emtyp subtype note");

                StringFormatter.PrintMessageTitle("Verification of Cancel Note");
                _providerAction.NotePage.ClickonAddNoteIcon();
                _providerAction.NotePage.SetAddNote("Cancel Note");
                _providerAction.NotePage.ClickOnAddNoteCancelLink();
                _providerAction.NotePage.IsAddNoteFormPresent().ShouldBeFalse("Is Add Note Form present?");
                _providerAction.NotePage.GetNoteListCount().ShouldBeEqual(noOfNotes + 2,
                    "Note count is same as before and hence note not added.");

                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.IsNoteContainerPresent()
                    .ShouldBeFalse("Note Container must collapse after clicking note icon again.");

            }
        }

        [Test, Category("NewProviderAction1")] //US68677 
        public void Verify_users_can_view_condition_rationales_in_Provider_action_at_the_condition_level()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ProviderSequence", "Value");
                var subType =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                        "SubType", "Value");
                var currentUserFullName = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "UserFullName", "Value");
                var noteText =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                        "NoteText", "Value");
                var updatedNoteText = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "UpdatedNoteText", "Value");

                _providerSearch.DeleteProviderNotesOnly(prvSeq, automatedBase.EnvironmentManager.HciAdminUsername);

                var totalProviderConditionNoteCount =
                    Convert.ToInt32(
                        _providerSearch.CountOfConditionNotesAssociatedToTheConditionFromDatabase(prvSeq, subType));


                _providerSearch.SearchByProviderSequence(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);

                _providerAction.IsConditionExposureButtonSelected()
                    .ShouldBeTrue("Condition exposure Button must be selected for the Default view");

                _providerAction.ClickonProviderNotesinConditionExposureSection();
                _providerAction.GetTooltipOfNotesIconInConditionExposureSection().ShouldBeEqual("View Condition Notes");
                _providerAction.SelectProviderConditionByConditionId(subType);
                _providerAction.GetEmptyNoteMessage().ShouldBeEqual("There are no notes available.",
                    "Note message should be displayed when notes are not availabe");

                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.SelectNoteSubType(subType);
                _providerAction.NotePage.SetAddNote(noteText);
                _providerAction.NotePage.ClickonAddNoteSaveButton();
                _providerAction.WaitForWorkingAjaxMessage();
                _providerAction.ClickonProviderNotes();

                StringFormatter.PrintMessage("Click on a Condition to view the notes associated with that condition");
                _providerAction.SelectProviderConditionByConditionId(subType);
                _providerAction.GetConditionNotesAssociatedToTheCondition().ShouldBeEqual
                    (totalProviderConditionNoteCount + 1, "Both count should match");

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
                _providerAction.IsVisibletoClientIconPresentByRowInConditionNotes(1)
                    .ShouldBeTrue("Is Visible to Client icon present?");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                _providerAction.GetNoteInConditionNotesByRow(1).ShouldBeEqual(noteText,
                    "Note Text before the note is modified should be as expected.");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);
                _providerAction.SelectProviderConditionByConditionId("SQ6L");

                StringFormatter.PrintMessageTitle("Update the Note text");
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUserFullName);
                _providerAction.NotePage.SetNoteInNoteEditorByName(updatedNoteText, currentUserFullName);
                _providerAction.NotePage.ClickVisibleToClient();
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
                    .ShouldBeEqual("SQ6P", "Correct Sub  Type should be displayed");
                _providerAction.GetConditionNoteRecordByColumnRow(4)
                    .ShouldBeEqual(currentUserFullName, "Correct User Name should be displayed.");
                _providerAction.GetConditionNoteRecordByColumnRow(5).ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                    "Correct Date should be displayed");
                _providerAction.IsVisibletoClientIconPresentByRowInConditionNotes(1)
                    .ShouldBeFalse("Is Visible to Client icon present?");
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                _providerAction.GetNoteInConditionNotesByRow(1)
                    .ShouldBeEqual(updatedNoteText, "Note Text should be modified");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);
            }
        }

        [Test, Category("NewProviderAction1")] //US50619
        public void Verify_Create_CIU_Referral()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                Dictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var providerSeq = testData["ProviderSequence"];
                var shortPhoneNumber = testData["ShortPhoneNumber"];
                var longPhoneNumber = testData["LongPhoneNumber"];
                var alphaNumericNumber = testData["AlphanumericNumber"];
                var specialCharactersNumber = testData["AlphanumericSpecialcharacter"];
                var patternCategoryAvailableOptions = testData["PatternCategoryOptions"].Split(',').ToList();


                _providerSearch.SearchByProviderSequence(providerSeq);
                automatedBase.CurrentPage =
                    _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(providerSeq);
                DeleteExistingCIUReferral(_providerAction);

                StringFormatter.PrintMessageTitle(
                    "Verify 'Add CIU Referral' icon is adjacent to the 'CIU Referral' Label");
                _providerAction.IsAddCIUIconAdjacentToLabel()
                    .ShouldBeTrue("'Add CIU Referral' icon is adjacent to the 'CIU Referral' Label");

                StringFormatter.PrintMessageTitle(
                    "Verify clicking on 'Add CIU Referrals' icon toggles 'Create CIU Referral' section");
                StringFormatter.PrintMessage("Clicking on 'Add CIU Referral Icon'");
                _providerAction.ClickOnAddCIUReferralRecordCssSelector();
                _providerAction.IsProviderDetailsSectionPresent().ShouldBeFalse("Provider Section is removed");
                _providerAction.IsCreateCIUReferralSectionDisplayed()
                    .ShouldBeTrue("Provider Section is replaced by 'Create CIU Referral'");

                CheckDefaultValuesInCIUReferral(automatedBase, _providerAction);

                StringFormatter.PrintMessageTitle("Verifying saving empty values produces the error message");
                _providerAction.ClickOnSaveCIUReferral();
                _providerAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error pops up when trying to save without any values");
                _providerAction.ClosePageError();
                _providerAction.IsInvalidInputPresentOnNoteByLabel("Identified Pattern").ShouldBeTrue(
                    "The 'Identified Pattern' field should" +
                    "have red highlight for empty input");

                StringFormatter.PrintMessageTitle("Verify Phone Number Field");
                VerifyPhoneNumberCIUReferral(shortPhoneNumber, longPhoneNumber, alphaNumericNumber, specialCharactersNumber, _providerAction);

                ClearAllFieldInCIUReferral(_providerAction);

                StringFormatter.PrintMessageTitle("Verify Ext. is an editable non-required field");
                _providerAction.IsRequiredField("Ext.")
                    .ShouldBeFalse("Ext. label does not have an asterisk indicating a required field");

                StringFormatter.PrintMessageTitle("Validation for Pattern Category field");
                VerifyPatternCategoryField(patternCategoryAvailableOptions, _providerAction);

                StringFormatter.PrintMessageTitle("Validation for the Proc Code Field");
                VerifyProcCode(_providerAction, _providerSearch);

                StringFormatter.PrintMessageTitle("Validation for the Identified Pattern field");
                VerifyIdentifiedPattern(_providerAction);

                StringFormatter.PrintMessageTitle("Verify Save button saved the CIU Referral");
                VerifySaveBtn(_providerAction);

            }
        }


        [Test, Category("NewProviderAction2")] // TE-636
        public void Verify_Action_Condition_Form_And_Plus_Icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var prvSeq = testData["ProviderSequence"];
                var condition = testData["Condition"];
                var code = testData["Code"];

                _providerAction = _providerSearch.NavigateToProviderAction(() =>
                {
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorking();
                    _providerSearch.ClickOnProvSeqByRowCol(1, 3);
                    _providerSearch.WaitForPageToLoad();
                });

                _providerAction.ClickOnEditActionCondition();
                StringFormatter.PrintMessageTitle("Verify Action Conditions Form Is In Disabled State");
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeTrue("Action Condition Form Should Be Disabled By Default");
                _providerAction.SelectFirstProviderConditionClientAction();
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeTrue("Action Condition Form Should Be Disabled By Default");


                StringFormatter.PrintMessageTitle(
                    "Verify Create User Specified Condition Icon In Manage Codes Of Concern");
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeFalse("User Specified Condition Form Should Not Be Present");
                _providerAction.GetToolTipForPlusIconInSecondColumn().ShouldBeEqual("Create User Specified Condition");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.AreUserSpecifiedConditionsPresent()
                    .ShouldBeTrue("User Specified Condition Form Should Be Present");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.AreUserSpecifiedConditionsPresent().ShouldBeFalse(
                    "User Specified Condition Form Should Be collapse on selecting icon for second time");

                StringFormatter.PrintMessageTitle(
                    "Verify Action Condition Form Is Enabled When User Specified Condition Is Selected");
                _providerAction.ClickOnPlusIconInSecondColumn();
                _providerAction.SelectConditionFromDropdownList(condition);
                _providerAction.ClickOnFlagAllCodes();
                _providerAction.ClickAddButtonInUserSpecifiedCondition();
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());
                _providerAction.ClickOnFlagAllCodes();
                _providerAction.SetBeginCodeAndEndCode(code, isTabRequired: true);
                _providerAction.ClickSearchButtonInUserSpecifiedCondition();
                _providerAction.ClickOnUserSepcifiedSearchResults();
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnSelectedConditionByCode(
                    _providerAction.GetFirstSelectedConditionText().Split('-')[0].Trim());

                _providerAction.ClickOnMatchingConditionByConditionId("A30D");
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Condition Is Selected For Action");
                _providerAction.ClickOnPlusIconInSecondColumn();

                StringFormatter.PrintMessageTitle("Verify Clicking Cancel Will Navigate User Out Of 3 Column View");
                _providerAction.IsThreeColumnViewPresent().ShouldBeTrue("Is three column view present ?");
                _providerAction.ClickOnCancelActionCondition();
                _providerAction.IsThreeColumnViewPresent()
                    .ShouldBeFalse("User Should Be Navigated Out Of 3 Column View to provider action page");

                StringFormatter.PrintMessage(
                    "Verify Clicking On Quick No Action Icon, Action Condition Form Is Enabled");
                _providerAction.ClickOnQuickNoActionAllConditions();
                _providerAction.IsActionConditionsFormDisabled().ShouldBeFalse(
                    "Action Condition Form Should Be Enabled When Quick No Action Is Selected For Action");
                _providerAction.ClickOnCancelActionCondition();

                StringFormatter.PrintMessage(
                    "Verify Clicking On Small Edit icon in condition row, Action Condition Form Is Enabled");
                _providerAction.ClickOnFirstConditionEditIcon();
                _providerAction.IsActionConditionsFormDisabled()
                    .ShouldBeFalse("Action Condition Form Should Be Enabled When Quick edit icon Is Selected");
                _providerAction.ClickOnCancelActionCondition();

            }
        }


        [Test]//TE-673
        public void Verify_Decision_rationale_form_Is_editable()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;

                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var reasoncode = paramList["ReasonCode"];
                var rationaleNote = paramList["InputValue"];
                //var expectedDecisionRationale = rationaleNote+reasoncode;
                var conditionId = paramList["ConditionId"];
                var prvSeq = paramList["ProviderSeq"];
                var date = paramList["Date"];

                _providerSearch.SearchByProviderSequence(prvSeq);
                _providerSearch.DeleteConditionActionAuditRecordFromDatabaseForCotiviti(prvSeq, date);
                _providerSearch.UpdateTriggeredConditionForCotiviti(prvSeq);
                automatedBase.CurrentPage = _providerAction =
                    _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(paramList["ProviderSeq"]);
                string currentUserFullName = _providerAction.GetLoggedInUserFullName();

                _providerAction.SelectFilterConditions(3); //select all triggered condition
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerAction.ClickOnEditActionCondition();
                _providerAction.SelectProviderConditionByConditionId(conditionId);

                _providerAction.SelectAction("No Action");
                _providerAction.SelectReasonCode(reasoncode);
                //_providerAction.ClickOnSaveActionCondition();
                _providerAction.GetDecisionRationaleText()
                    .ShouldBeEqual(paramList["ReasonCode"], "Reason code displayed?");
                _providerAction.IsNoteTextAreaEditable()
                    .ShouldBeTrue("Decision rationale text box should be enabled after adding reason code and action");

                _providerAction.FillDecisionRationaleNote(rationaleNote, true);
                var finalDecisionRationale = _providerAction.GetDecisionRationaleText();
                finalDecisionRationale.ShouldBeEqual(rationaleNote, "User entered value should be displayed");
                _providerAction.ClickOnSaveActionCondition();
                _providerAction.ClickonProviderNotes();
                _providerAction.NotePage.ClickOnEditIconOnNotesByName(currentUserFullName);
                //_providerAction.WaitForStaticTime(2000);
                var providerNotes = _providerAction.NotePage.GetNoteInNoteEditorByName(currentUserFullName)
                    .Replace("\r\n", " ");
                providerNotes.ShouldBeEqual(finalDecisionRationale,
                    "Saved Generate rationale is populated in provider notes");
                _providerAction.ClickonProviderNotes();
                _providerAction.SelectFilterConditions(3);
                _providerAction.SelectProviderConditionByConditionId(conditionId);
                _providerAction.ClickonProviderNotesinConditionExposureSection();
                _providerAction.ClickOnCaretIconOnConditionNotesByRow(1);
                var providerNotesInConditionNotes =
                    _providerAction.GetNoteInConditionNotesByRow(1).Replace("\r\n", " ");
                providerNotesInConditionNotes.ShouldBeEqual(finalDecisionRationale,
                    "Generate rationale note should be present in Condition Notes");
                _providerAction.ClickOnCollapseIconOnNotesByRow(1);

            }
        }

       
        #endregion

        #region PRIVATE METHODS

        private void ActionCondition(ProviderActionPage _providerAction, string reasonCode = "", string action = "Deny",
            string note = "UITEST", List<string> procCodeList = null, bool save = true)
        {
            _providerAction.SelectAction(action);
            if (reasonCode == "")
                _providerAction.SelectReasonCode("BEW4 - BEW Test 4");
            else
                _providerAction.SelectReasonCode(reasonCode);
            if (action != "No Action")
                _providerAction.InsertDecisionRationaleUsingGenerateRationale(note);
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
        private void CreateCIUReferral(ProviderActionPage _providerAction, string phoneNo = "231-456-7890", string ext = null,
            string PatternCategory = "All", string IdentifiedPattern = "Test Pattern", string ProcCode = "1234")
        {
            _providerAction.ClickOnAddCIUReferralRecordCssSelector();
            if (ext != null)
                _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Ext", ext);
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Phone", phoneNo);
            _providerAction.InsertIdentifiedPattern(IdentifiedPattern);
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Proc", ProcCode);
            _providerAction.SelectPatternCategory(PatternCategory);
            _providerAction.ClickOnSaveCIUReferral();
            _providerAction.WaitForWorkingAjaxMessage();
        }

        private static bool IsCountInCorrectFormat(string value, ProviderActionPage _providerAction)
        {
            Console.WriteLine("Checking format of Count: " + value);
            return Regex.IsMatch(value, @"^[0-9]+ \([0-9]+%\)$");
        }

        private static bool IsDollarAmountInCorrectFormat(string value, ProviderActionPage _providerAction)
        {
            Console.WriteLine("Checking format of Amount: " + value);
            return Regex.IsMatch(value, @"^\$[0-9.,]+ \([0-9]+%\)$");
        }

        private static void VerifyThatListOfStringIsSortedDescByValue(IEnumerable<string> actualList, ProviderActionPage _providerAction)
        {
            var actualListFloat =
                actualList.Select(
                    s => float.Parse(s.Remove(s.IndexOf(" ", System.StringComparison.Ordinal) + 1).Replace("$", "")))
                    .ToList();
            actualListFloat.IsInDescendingOrder();
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


        private ProviderActionPage CheckforProfileTooltipTitle(string provSeq, string profileMessage, bool isMedAlert,
             ProviderSearchPage _providerSearch,
            NewAutomatedBaseParallelRun automatedBase, bool isReview = false)
        {
            const string onReview = "Provider is currently on review";
            const string medAlert = "Medaware Alert Issued tst";
            string providerActionMessage = (isMedAlert) ? medAlert : "View provider profile";
            _providerSearch.SearchByProviderSequence(provSeq);
            /*_providerSearch.SelectAllProviders();
            _providerSearch.SearchByProviderSequence(provSeq);
            if (_providerSearch.IsPageErrorPopupPresent()) _providerSearch.CloseErrorPopup();
            _providerSearch.GetProviderProfileIconTooltip(1)
                .ShouldBeEqual(profileMessage,
                    string.Format("Tooltip with message {0} for flagged provider", profileMessage));
            _providerSearch.GetWidgetProviderProfileIconTooltip()
                .ShouldBeEqual("View Provider Profile", "Tooltip message to view profile in widget present");
            if (isReview)
            {
                _providerSearch.GetProviderProfileReviewTooltip(1)
                    .ShouldBeEqual("Provider currently on review", "Tooltip for on review message in grid result");
                _providerSearch.GetWidgetProviderProfileReviewTooltip()
                    .ShouldBeEqual(onReview, "Tooltip for on review message in widget present");
            }

            _providerAction = _providerSearch.ClickOnProviderNameToOpenNewProviderActionForProvSeq(provSeq);*/
            var _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(provSeq);
            automatedBase.CurrentPage = _providerAction;
            _providerAction.GetProfileIndicatorTitle()
                .ShouldBeEqual(providerActionMessage, "Tooltip in provider action header as " + providerActionMessage);
            if (isReview)
            {
                _providerAction.GetProfileReviewIndicatorTitle()
                    .ShouldBeEqual("Provider is currently under review",
                        "Tooltip for on review message in provider action header");
            }
            automatedBase.CurrentPage = _providerAction.NavigateToProviderSearch();

            return _providerAction;
        }


        private void ForAllReasonCodeReferralValidation(List<string> allReasonCode, string action, ProviderActionPage _providerAction)
        {
            _providerAction.SelectAction(action);
            Random rand = new Random();
            // foreach (var reason in allReasonCode)
            for (var i = 0; i < 5; i++)
            {
                int index = rand.Next(0, 18);
                _providerAction.ClearReasonCode();
                _providerAction.SelectReasonCode(allReasonCode[index]);
                _providerAction.ClickOnGenerateRationaleButton();
                _providerAction.ClickOnFinishButton();
                _providerAction.IsValidationNoticeModalPopupPresent()
                    .ShouldBeTrue("Is Validation Modal popup present when referral is empty for ?" + allReasonCode[index]);
                _providerAction.GetValidationNoticePopupMessage()
                    .ShouldBeEqual(
                        "Billed Referral Exposure should be greater than $0.00.\r\nPaid Referral Exposure should be greater than $0.00.");
                _providerAction.CloseValidationNoticePopup();
                _providerAction.ClickOnCancelOnGenerateRationale();
            }
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

        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
        {
            name.DoesNameContainsOnlyFirstWithLastname().ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
        }


        private void VerifyThatDateIsInCorrectFormat(string date, string dateName)
        {
            date.IsDateInFormat().ShouldBeTrue("The " + dateName + " Date'" + date + "' is in format MM/DD/YYYY");
        }
        private void SetValueForGenerateRationale(IList<string> requiredFields, IList<string> reqValue, ProviderActionPage _providerAction)
        {
            var i = 0;
            for (; i < requiredFields.Count(); i++)
            {
                switch (requiredFields[i])
                {
                    case "License Status":
                        _providerAction.SetLicenseStatus(reqValue[i]);
                        break;
                    case "User Rationale Summary":
                        _providerAction.SetUserRationaleSummary(reqValue[i]);
                        break;
                    default:
                        _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i],
                            reqValue[i]);
                        break;
                }
            }

        }

        private void ValidationForNcharactersAndAlphanumeric(string addedFields, string validationInputValue,
            int charcNo, ProviderActionPage _providerAction)
        {
            var textlimit = new string('a', charcNo + 1);
            _providerAction.SetGenerateRationaleFormInputFieldByLabel(addedFields,
                validationInputValue + textlimit);
            _providerAction.IsPageErrorPopupModalPresent()
                .ShouldBeFalse(
                    addedFields + " is an alphanumeric field. Error not generated when input is alphanumeric.");
            _providerAction.GetGenerateRationaleFormInputFieldByLabel(addedFields)
                .Length.ShouldBeEqual(charcNo, addedFields + " accepts up to " + charcNo + " alphanumeric characters");
        }

        private void SetInputGetExpectedError(string inputField, string value, string expectedType, ProviderActionPage _providerAction)
        {
            _providerAction.SetGenerateRationaleFormInputFieldByLabel(inputField, value);
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Error generated when " + expectedType);
            _providerAction.ClosePageError();
            _providerAction.WaitForStaticTime(1000);
        }

        private void VerifyPhoneNumberCIUReferral(string shortPhone, string longPhone, string alphaNumericPhone, string specialCharacters, ProviderActionPage _providerAction)
        {

            const string patternCategory = "All";
            const string procCode = "99213";
            const string identifiedPattern = "Test";
            const string extNumber = "256";

            StringFormatter.PrintMessageTitle("Validate error is thrown on entering phone number less than 10 digits");
            SetAllCIUDetailsAndSave(shortPhone, patternCategory, procCode, identifiedPattern, _providerAction);
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Field is editable and error is thrown when phone number is less than 10 digits");
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
            _providerAction.ClosePageError();
            _providerAction.IsInvalidInputPresentByLabel("Phone Number").ShouldBeTrue("The red highlight should be present when the Phone Number" +
                                                                                         "is invalid");
            _providerAction.GetInvalidInputToolTipByLabel("Phone Number")
                .ShouldBeEqual("10 digit Phone number is required before the record can be saved.",
                    "Is Tooltip message of Red Exclamation Icon");


            StringFormatter.PrintMessageTitle("Validate entering phone number greater than 10 digits");
            _providerAction.ClearInputField("Phone Number");
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Phone Number", longPhone);
            _providerAction.ClearInputField("Ext.");
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Ext.", extNumber);
            _providerAction.GetCIUInputValueByLabel("Phone Number").Replace("-", "").ShouldBeEqual(longPhone.Substring(0, 10));
            _providerAction.GetCIUInputValueByLabel("Ext.").ShouldBeEqual(extNumber, "The Ext. field is editable");



            StringFormatter.PrintMessageTitle("Validate entering an alphanumeric phone number");
            _providerAction.ClearInputField("Phone Number");
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Phone Number", alphaNumericPhone, true);
            _providerAction.ClickOnSaveCIUReferral();
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Error is thrown when alphanumeric phone number is entered");
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
            _providerAction.ClosePageError();
            _providerAction.IsInvalidInputPresentByLabel("Phone Number").ShouldBeTrue("The red highlight should be present when the Phone Number" +
                                                                                         "is invalid");
            _providerAction.GetInvalidInputToolTipByLabel("Phone Number")
                .ShouldBeEqual("10 digit Phone number is required before the record can be saved.",
                    "Is Tooltip message of Red Exclamation Icon");

            StringFormatter.PrintLineBreak();
            StringFormatter.PrintMessage("Validate entering an alphanumeric phone number containing special characters");
            _providerAction.ClearInputField("Phone Number");
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Phone Number", specialCharacters, true);
            _providerAction.ClickOnSaveCIUReferral();
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
            _providerAction.ClosePageError();
            _providerAction.IsInvalidInputPresentByLabel("Phone Number").ShouldBeTrue("The red highlight should be present when the Phone Number" +
                                                                                         "is invalid");
            _providerAction.GetInvalidInputToolTipByLabel("Phone Number")
                .ShouldBeEqual("10 digit Phone number is required before the record can be saved.",
                    "Is Tooltip message of Red Exclamation Icon");
        }

        private void VerifyPatternCategoryField(List<String> availablePatternCategoryOptions, ProviderActionPage _providerAction)
        {
            _providerAction.ClickOnSaveCIUReferral();
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record " +
                               "can be saved.", "At least one pattern needs to be selected to save the CIU Referral");
            _providerAction.ClosePageError();
            _providerAction.IsInvalidInputPresentByLabel("Pattern Category").ShouldBeTrue("Red highlight should be around 'Pattern Category'" +
                                                                                             "for invalid input");

            foreach (var patternCategory in availablePatternCategoryOptions)
            {
                _providerAction.SelectPatternCategory(patternCategory);
            }

            _providerAction.GetSelectedPatternCategories().ShouldCollectionBeEqual(availablePatternCategoryOptions, "All of the Pattern Category " +
                                                                                                    "options are selected and displayed correctly");
            _providerAction.GetPatternCategoryPlaceholderValue("Pattern Category").ShouldBeEqual("Multiple values selected", "Filter watermark " +
                                                                                                    "displays 'Multiple values selected'");
        }


        private void VerifyProcCode(ProviderActionPage _providerAction, ProviderSearchPage _providerSearch)
        {
            const string patternCategory = "All";
            const string identifiedPattern = "Test";
            const string phoneNumber = "1234567890";
            SetAllCIUDetailsAndSave(phoneNumber, patternCategory, "", identifiedPattern, _providerAction);
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Error Message pops up if tried to save without a Proc Code value");
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
            _providerAction.ClosePageError();
            _providerAction.IsInvalidInputPresentByLabel("Proc Code").ShouldBeTrue("'Proc Code' field should be highlighted red on invalid input");
            _providerAction.GetInvalidInputToolTipByLabel("Proc Code")
                .ShouldBeEqual("Proc Code value is required before the record can be saved.");
            var procCode = _providerSearch.RandomString(1001);
            StringFormatter.PrintMessageTitle("Validate if the Proc Code field takes 1000 characters input");
            _providerAction.SetLengthyValueInCiuReferralFormInputValueGeneric(procCode, "Proc Code", 1000);
            _providerAction.GetCIUInputValueByLabel("Proc Code").ShouldBeEqual(procCode.Substring(0, 1000),
                "Proc Code Field Allow Only 1000 character");
        }

        private void CheckDefaultValuesInCIUReferral(NewAutomatedBaseParallelRun automatedBase, ProviderActionPage _providerAction)
        {
            var phone = GetPhoneNumber(automatedBase.EnvironmentManager.Username, _providerAction);

            StringFormatter.PrintMessage("Validate if current user's phone number is displayed by default in the phone number field");
            _providerAction.GetCIUInputValueByLabel("Phone").Replace("-", "")
                .ShouldBeEqual(phone, "Phone no.field should display current user's phone number");

            StringFormatter.PrintMessage("Validate if none of the categories are selected by default in Selected Pattern Categories");
            _providerAction.GetSelectedPatternCategories().Count.ShouldBeEqual(0, "None of the Pattern Category options are selected by default");
            _providerAction.GetPatternCategoryPlaceholderValue("Pattern Category").ShouldBeEqual("Select one or more", "User is notified to select at least one value for Pattern Category");

            StringFormatter.PrintMessage("Validate default Value for Proc Code");
            _providerAction.GetCIUInputValueByLabel("Proc Code").ShouldBeNullorEmpty("The default value of Proc Code field should null by default");

        }

        public void VerifyIdentifiedPattern(ProviderActionPage _providerAction)
        {
            var noteString = new string('a', 2010);
            _providerAction.SetLengthyNoteToCiuReferral(noteString,false);
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("When Identified Pattern is long error message should pop up.");
            _providerAction.GetConfirmationMessage()
                .ShouldBeEqual("Identified Pattern value is too long.",
                    "Error pop up message should be present when Identified Pattern is long");
            _providerAction.ClosePageError();
            _providerAction.GetNoteOfCiuReferral()
                .Length.ShouldBeLessOrEqual(1893, "Identified Pattern shuold be less or equal to 2000 characters");
        }

        private string GetPhoneNumber(string username, ProviderActionPage _providerAction)
        {
            return _providerAction.GetPhoneNumberForSelectedUser(username);
        }

        private void SetAllCIUDetailsAndSave(string phoneNum, string patternCategory, string procCode,
            string identifiedPattern, ProviderActionPage _providerAction, string ext = "")
        {
            _providerAction.ClearInputField("Phone Number");
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Phone Number", phoneNum);
            _providerAction.SelectPatternCategory(patternCategory);
            _providerAction.ClearInputField("Proc Code");
            _providerAction.InsertIdentifiedPattern(identifiedPattern);
            _providerAction.SetInputFieldOnCreateCIUReferralByLabel("Proc Code", procCode);
            _providerAction.WaitForStaticTime(300);
            _providerAction.ClickOnSaveCIUReferral();
        }

        private void VerifySaveBtn(ProviderActionPage _providerAction, string patternCategoryAll = "All", string phoneNumber = "2314567890", string procCode = "99213", string noteString = "Test")
        {
            SetAllCIUDetailsAndSave(phoneNumber, patternCategoryAll, procCode, noteString, _providerAction);
            _providerAction.GetCIUReferralRecordRowCount()
                .ShouldBeEqual(1, "New record is sucessfully added when clicked on save with valid entries");
        }

        private void DeleteExistingCIUReferral(ProviderActionPage _providerAction)
        {
            if (_providerAction.IsNoCiuReferralMessagePresent()) return;
            var ciuCount = _providerAction.GetCiuReferralRecordRowCount();
            for (var i = 0; i < ciuCount; i++)
            {
                _providerAction.ClickOnDeleteCIUReferralIconByRecordRow(1);
                _providerAction.ClickOkCancelOnConfirmationModal(true);
            }
        }

        private void ClearAllFieldInCIUReferral(ProviderActionPage _providerAction)
        {
            _providerAction.ClearInputField("Phone Number");
            _providerAction.ClearInputField("Ext.");
            _providerAction.ClearInputField("Proc Code");
            _providerAction.InsertIdentifiedPattern("");
            _providerAction.SelectPatternCategory("Clear");
        }


        public void Verify_Next_Button_In_Provider_Action(ProviderSearchPage providerSearch, NewAutomatedBaseParallelRun automatedBase, ProviderActionPage _providerAction)
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


        public ProviderActionPage ClickOnExportHistoryIcon(string label, string prvseq, ProviderSearchPage _providerSearch, bool twelve_months = true)
        {
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, prvseq);
            _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerSearch.WaitForPageToLoadWithSideBarPanel();
            var _providerAction = _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvseq);
            _providerAction.ClickOnProviderClaimHistoryIcon();
            if(twelve_months)
                _providerAction.ClickOnExport12MonthsClaimHistory();
            else
                _providerAction.ClickOnExport3YearsClaimHistory();

            return _providerAction;
        }

        private void Verify_if_result_set_exceeds_100_codes_warning_message_will_be_displayed(string begincode, string endcode, ProviderActionPage _providerAction)
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

        private void Generate_Rationale(string[] requiredFields, string[] inputValue, string beginCode, string endCode,
            string condition, ProviderActionPage _providerAction, bool isClient = true, bool isSearchResult = false,
            bool isFlagAllCodes = false)
        {
            _providerAction.SelectConditionFromDropdownList(condition);
            if (isFlagAllCodes)
            {
                _providerAction.ClickOnFlagAllCodes();
            }
            else
            {
                _providerAction.SetBeginCodeAndEndCode(beginCode, endCode);
            }

            _providerAction.ClickSearchButtonInUserSpecifiedCondition(isClient);
            if(isSearchResult)
                _providerAction.ClickOnUserSepcifiedSearchResults();
            _providerAction.SelectAction("Review");
            _providerAction.SelectFirstReasonCode();
            _providerAction.ClickOnGenerateRationaleButton();
            for (int i = 0; i < requiredFields.Count(); i++)
            {
                _providerAction.SetGenerateRationaleFormInputFieldByLabel(requiredFields[i], inputValue[i]);
            }

            _providerAction.ClickOnFinishButton();
        }

        private void IsErrorPopUpPresent(string errorMessage, ProviderActionPage _providerAction)
        {
            _providerAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Is page error pop up present?");
            _providerAction.GetPageErrorMessage().Replace("\r\n", string.Empty).ShouldBeEqual(errorMessage);
            _providerAction.ClosePageError();
            _providerAction.WaitForStaticTime(1000);
        }
    }

    #endregion
}

