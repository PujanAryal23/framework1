using System.Linq;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
   // [Parallelizable(ParallelScope.All)]
    public class AppealRationaleManager
    {
        #region PRIVATE PROPERTIES

        //private AppealRationaleManagerPage _appealRationaleManager;
        //private AppealActionPage _appealAction;
        //private AppealManagerPage _appealManager;
        //private AppealCreatorPage _appealCreator;
        //private AppealSearchPage _appealSearch;



        #endregion

        #region PROTECTED PROPERTIES

        //protected string FullyQualifiedClassName
        //{
        //    get { return GetType().FullName; }
        //}

        #endregion

        #region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        _appealRationaleManager = QuickLaunch.NavigateToAppealRationaleManager();
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}


        //protected override void TestCleanUp()
        //{
        //    base.TestCleanUp();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _appealRationaleManager = _appealRationaleManager.Logout().LoginAsHciAdminUser().NavigateToAppealRationaleManager();
        //    }
        //    else
        //    {
        //        if (!_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen()) _appealRationaleManager.ToggleSideBarMenu();
        //        if (_appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent())
        //        {
        //            _appealRationaleManager.GetSideWindow.Cancel();
        //            _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);
        //        }
        //        _appealRationaleManager.GetSideBarPanelSearch.ClickOnClearLink();
        //        _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "Active");
        //        _appealRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
        //    }

        //}

        #endregion

        #region TEST SUITES

        [Test,Category("SmokeTestDeployment")] //TANT-96
        public void Verify_Appeal_Rationale_Manager_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var testName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var filterOptions = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "AppealRationaleManager_Sort_Option").Values.ToList();

                StringFormatter.PrintMessage("Verify Navigation From Sub Menu");
                _appealRationaleManager.GetPageHeader().ShouldBeEqual(
                    PageHeaderEnum.AppealRationaleManager.GetStringValue(),
                    "Appeal Rationale Manager Page Should Be Presented");
                _appealRationaleManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");

                StringFormatter.PrintMessage("Verify open/close of Find Rationale Panel");
                _appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Find Rationale Panel on sidebar is open by default when user lands on page.");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("Find Rationale Panel on sidebar is hidden when toggle button is clicked.");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Find Rationale Panel on sidebar should be opened.");

                StringFormatter.PrintMessage("Verify Sort Option List");
                _appealRationaleManager.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _appealRationaleManager.GetGridViewSection.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Results", "Correct tooltip is displayed for sort icon");
                _appealRationaleManager.GetGridViewSection.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Be Equal");

                StringFormatter.PrintMessage("Verify Add Appeal Rationale Icon");
                _appealRationaleManager.ClickOnAddButton();
                _appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeTrue("Is SideWindow present?");
                _appealRationaleManager.GetSideWindow.Cancel();
                _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);

                StringFormatter.PrintMessage("Verify Edit Appeal Rationale Icon");
                _appealRationaleManager.GetGridViewSection.ClickOnEditIcon();

                _appealRationaleManager.ScrollToLastPosition();
                _appealRationaleManager.IsEditWindowPresent().ShouldBeTrue("Is Edit Window Present?");
                _appealRationaleManager.GetSideWindow.Cancel();

                StringFormatter.PrintMessage("Verify Deactivate Icon");
                int activeAppealRationaleCount = _appealRationaleManager.GetRationaleRowsCountFromPage();
                _appealRationaleManager.GetGridViewSection.ClickOnDeleteIcon();
                _appealRationaleManager.GetPageErrorMessage()
                    .ShouldBeEqual("The rationale will no longer be active. Do you wish to proceed?");
                StringFormatter.PrintMessageTitle("Verification of Cancel button on confirmation popup");
                _appealRationaleManager.ClickOkCancelOnConfirmationModal(false);
                _appealRationaleManager.GetRationaleRowsCountFromPage().ShouldBeEqual(activeAppealRationaleCount,
                    "Is Rationale Present after pressing cancel on confirmation pop up?");
            }
        }

       

        [Test, Category("SchemaDependent")] //US67968 //US67969 //US67971 
        public void Verify_Audit_Records_And_Inactive_Rationales_For_Appeal_Rationale()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var createdAppeal = paramList["AppealRationaleData"].Split(';');
                var existingAppealRationale = paramList["ExistingAppealRationaleData"].Split(';');

                _appealRationaleManager.HardDeleteRationale(createdAppeal);

                _appealRationaleManager.ClickOnAddButton();
                if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _appealRationaleManager.ToggleSideBarMenu();
                _appealRationaleManager.CreateAppealRationale(createdAppeal);
                _appealRationaleManager.ShowAppealRationaleAudit(createdAppeal);

                StringFormatter.PrintMessageTitle("Verifying if Appeal Rationale Audit Window is Present");
                _appealRationaleManager.IsAppealRatioanleAuditWindowPresent()
                    .ShouldBeTrue("Is Appeal Rationale Audit Window Present?");
                _appealRationaleManager.GetAppealAuditHistoryTitle().ShouldBeEqual("Appeal Rationale Audit History",
                    "Is Appeal Audit History Title Consistent?");


                StringFormatter.PrintMessageTitle("Verifying Action, Modification Date and  User on Appeal Audits");
                VerifyCreateEditDeleteOnAuditLogs("Create",_appealRationaleManager);

                _appealRationaleManager.ClickOnEditAppealRationale(createdAppeal);
                _appealRationaleManager.GetSideWindow.FillIFrame("Pay Summary", "Test Pay Summary (edited)");
                _appealRationaleManager.GetSideWindow.Save();
                _appealRationaleManager.ShowAppealRationaleAudit(createdAppeal, true);

                VerifyCreateEditDeleteOnAuditLogs("Edit",_appealRationaleManager);

                _appealRationaleManager.ClickOnDeleteAppealRationale(createdAppeal);


                //US67971
                _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "Inactive");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealRationaleManager.ShowAppealRationaleAudit(createdAppeal);
                _appealRationaleManager.GetDeletedIconToolTipText(createdAppeal)
                    .ShouldBeEqual("Inactive", "Is tooltip on delete icon Inactive?");
                _appealRationaleManager.ClickOnEditAppealRationale(createdAppeal);

                string[] fieldList = {"Rationale", "Pay Summary", "Deny Summary", "Adjust Summary"};
                foreach (var eachField in fieldList)
                {
                    _appealRationaleManager.GetSideWindow.IsIFrameDisabled(eachField)
                        .ShouldBeTrue("Is " + eachField + " Field Not Editable?");
                }

                StringFormatter.PrintMessage(
                    "Check if save and cancel button is disabled  by calling Save function and passing true for flag :checkIfDisabled");
                _appealRationaleManager.GetSideWindow.Save(true).ShouldBeTrue("Is Save Button Disabled?");
                _appealRationaleManager.GetSideWindow.Cancel(true).ShouldBeFalse("Is Cancel Button Disabled?");

                _appealRationaleManager.RefreshPage();

                if (!_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _appealRationaleManager.ToggleSideBarMenu();
                _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "Active");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealRationaleManager.ShowAppealRationaleAudit(existingAppealRationale, false, true);

                _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "Active");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealRationaleManager.ShowAppealRationaleAudit(existingAppealRationale, false, true);

                _appealRationaleManager.GetAuditHistoryListByCol(2).Select(DateTime.Parse).ToList()
                    .IsInDescendingOrder()
                    .ShouldBeTrue("Is Audit History in Descending Order By Date?");

                //US67971

                _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "All");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();

                _appealRationaleManager.IsActiveAndInactiveCountNonZero()
                    .ShouldBeTrue("Are both active and inactive rationales present on search?");

            }

        }


        [Test] //US67964 + US69384  + US69383 + CAR-3093(CAR-3212) + CV-3444(CV-3632)
        [NonParallelizable]
        public void Verify_Add_Rationale_Field_Values()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                _appealRationaleManager.ClickOnAddButton();
                if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _appealRationaleManager.ToggleSideBarMenu();
                VerifyIfRequiredFieldsArePresent(_appealRationaleManager); //US69384
                DoFieldsAllowOnlyNCharacters(_appealRationaleManager);

                CheckCAGApprovalDateFormat(paramList["CagDate"], _appealRationaleManager);
                CheckDuplicatesOnSave(paramList["DuplicateData"].Split(';'), paramList["OverlappingData"].Split(';'), _appealRationaleManager);
                VerifyProcTrigandModifierValues(paramList["DuplicateData"].Split(';'), _appealRationaleManager); //US69383
                VerifyAddingOfRationaleandCancelButton(paramList["AppealRationaleData"].Split(';'),_appealRationaleManager);
                CheckClientFlagSourceValueandOverrideFieldsSortingandDefaultValue(_appealRationaleManager);

                CheckMaxSizeforTextBoxandFrameBody(_appealRationaleManager);
            }

        }


        [NonParallelizable]
        [Test] //US67966
        public void Validate_filter_panel_in_find_rationale_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var defaultRowCount = _appealRationaleManager.GetGridViewSection.GetGridRowCount();
                var clientList = _appealRationaleManager.GetActiveClientList();
                clientList.Insert(0,"All Centene");

                StringFormatter.PrintMessageTitle(
                    "Verifying Default value behaviour for Filter input panel fields");
                ValidateDefaultValueForSideBarPanelFilters(_appealRationaleManager);

                StringFormatter.PrintMessageTitle("Verification of drop down fields under find panel");
                ValidateDropDownFoDefaultValueAndExpectedList("Status", paramList["StatusList"].Split(';'), _appealRationaleManager,false);
                ValidateDropDownFoDefaultValueAndExpectedList("Client",
                        clientList, _appealRationaleManager);
                StringFormatter.PrintMessageTitle("The following code runs only in UAT");
                ValidateDropDownFoDefaultValueAndExpectedList("Flag", _appealRationaleManager.GetFlagLists(),_appealRationaleManager);
                ValidateDropDownFoDefaultValueAndExpectedList("Source Value",
                        _appealRationaleManager.GetSourceListsAndSort(),_appealRationaleManager);

                var text = "as.12";
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", text + "3");
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Trigger Code", text + "3");
                _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code")
                    .Length.ShouldBeEqual(5, "Field accepts 5 digit value only.");
                _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code")
                    .ShouldBeEqual(text,
                        "Field is numeric but its upto user to enter correct values and no validation is being checked.");
                _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Trigger Code")
                    .ShouldBeEqual(text,
                        "Field is numeric but its upto user to enter correct values and no validation is being checked.");
                _appealRationaleManager.ClickFindButtonAndWait();
                _appealRationaleManager.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.");

                StringFormatter.PrintMessageTitle("Verification of Clear Link");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateDefaultValueForSideBarPanelFilters(_appealRationaleManager);

                var filterTestData = paramList["FilterTestDataFirst"].Split(';');
                InputValuesUnderFindPanelWithGivenParameter(filterTestData,_appealRationaleManager);
                _appealRationaleManager.ClickFindButtonAndWait();
                _appealRationaleManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search should execute and show for a combination of filter values.");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Client:").ShouldBeEqual(filterTestData[1]);
                _appealRationaleManager.GetValueInGridBylabelAndRow("Flag:").ShouldBeEqual(filterTestData[2]);
                _appealRationaleManager.GetValueInGridBylabelAndRow("Source:").ShouldBeEqual(filterTestData[3]);
                _appealRationaleManager.GetValueInGridBylabelAndRow("M:").ShouldBeEqual(filterTestData[4]);

                var procRange = _appealRationaleManager.GetValueInGridBylabelAndRow("Proc:").Split('-');
                var trigCodeRange = _appealRationaleManager.GetValueInGridBylabelAndRow("Trig:").Split('-');
                Enumerable.Range(int.Parse(procRange[0]), int.Parse(procRange[1]))
                    .Contains(int.Parse(filterTestData[5]))
                    .ShouldBeTrue("Search result should yield rationale with proc code falling under the range.");
                Enumerable.Range(int.Parse(trigCodeRange[0]), int.Parse(trigCodeRange[1]))
                    .Contains(int.Parse(filterTestData[6]))
                    .ShouldBeTrue(
                        "Search result should yield rationale with trigger code falling under the range.");
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", "123");
                _appealRationaleManager.ClickFindButtonAndWait();

                StringFormatter.PrintMessageTitle("Validate For range value of proc code and trigger code.");
                _appealRationaleManager.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.");
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", filterTestData[5]);
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Trigger Code",
                    filterTestData[6]);
                _appealRationaleManager.ClickFindButtonAndWait();

                StringFormatter.PrintMessageTitle(
                    "Search for correct result so as to validate for  non match of non matching trigger code.");
                _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Trigger Code", "112");
                _appealRationaleManager.ClickFindButtonAndWait();
                _appealRationaleManager.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.");
                _appealRationaleManager.GetSideBarPanelSearch.ClickOnClearLink();
                _appealRationaleManager.GetGridViewSection.GetGridRowCount().ShouldBeEqual(defaultRowCount,
                    "Selecting Clear will reset results to default view.");
            }
        }

        [NonParallelizable]
        [Test] //US67965
        public void Verify_list_of_appeal_rationales()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var loadMoreValue = _appealRationaleManager.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
                var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                for (var i = 0; i < count; i++)
                {
                    _appealRationaleManager.ClickOnLoadMore();
                }
                _appealRationaleManager.GetRationaleRowsCountFromPage()
                    .ShouldBeEqual(_appealRationaleManager.GetActiveRationaleCount(),
                        "Active Rationales count should match");
                _appealRationaleManager.GetFlagLists()
                    .ShouldCollectionBeSorted(false, "Result should be sorted by flag ascending");
                _appealRationaleManager.IsPencilIconPresentInRationaleRecord(1)
                    .ShouldBeTrue("Pencil Icon should be visible in a rationale record");
                _appealRationaleManager.IsDeactivateIconPresentInRationaleRecord(1)
                    .ShouldBeTrue("Deactivate Icon should be visible in a rationale record");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Flag:")
                    .ShouldNotBeNull("Flag: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Source:")
                    .ShouldNotBeNull("Source: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("M:")
                    .ShouldNotBeNull("M: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Proc:")
                    .ShouldNotBeNull("Proc: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Trig:")
                    .ShouldNotBeNull("Trig: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Client:")
                    .ShouldNotBeNull("Client: label should be correct and value should not be null");
                _appealRationaleManager.GetValueInGridBylabelAndRow("Eff DOS:")
                    .ShouldNotBeNull("Eff DOS: label should be correct and value should not be null");

                StringFormatter.PrintMessage("Checking Flags:");
                _appealRationaleManager.GetGridListValueByCol(2).ShouldNotContain("", "Flag value should not be empty");
                StringFormatter.PrintMessage("Checking Sources:");
                _appealRationaleManager.GetGridListValueByCol(3).Any(source => source.Contains(","))
                    .ShouldBeTrue("At least one source should be shown as <Source value>,<modifier override>");
                StringFormatter.PrintMessage("Checking Modifiers:");
                var modifierFormat = new Regex(@"^[a-zA-Z0-9]{2}$");

                _appealRationaleManager.GetGridListValueByCol(4)
                    .Where(modifier => modifier.Replace("M:", "").Trim() != "")
                    .All(modifier => modifierFormat.IsMatch(modifier.Replace("M:", "").Trim()))
                    .ShouldBeTrue("Modifier values should be either empty or two digits");
                StringFormatter.PrintMessage("Checking Proc codes:");
                var procTrigFormat =
                    new Regex(@"^[a-zA-Z0-9]+-[a-zA-Z0-9]+$|^[a-zA-Z0-9]+$"); //<XXXXX-XXXXX> or <XXXXX>
                _appealRationaleManager.GetGridListValueByCol(6).Where(proc => proc.Replace("Proc:", "").Trim() != "")
                    .All(proc => procTrigFormat.IsMatch(proc.Replace("Proc:", "").Trim()))
                    .ShouldBeTrue("Proc codes should match the expected format");
                StringFormatter.PrintMessage("Checking Trig codes");
                _appealRationaleManager.GetGridListValueByCol(7).Where(trig => trig.Replace("Trig:", "").Trim() != "")
                    .All(trig => procTrigFormat.IsMatch(trig.Replace("Trig:", "").Trim()))
                    .ShouldBeTrue("Trig codes should match the expected format");
                StringFormatter.PrintMessage("Checking Clients");
                _appealRationaleManager.GetGridListValueByCol(8)
                    .Any(client => client.Replace("Client:", "").Trim() == "All")
                    .ShouldBeTrue("At least one client should be 'All");


                #region CAR-803

                var effDOS = _appealRationaleManager.GetGridListValueByCol(9);
                var effDosRegex =
                    new Regex(
                        @"(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[0-1])\/(19|20)\d{2}-(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[0-1])\/(19|20)\d{2}$");
                effDOS.Count(x => effDosRegex.IsMatch(x))
                    .ShouldBeGreater(0, "Is Eff DOS is in MM/DD/YYYY-DD/MM/YYYY");
                effDosRegex =
                    new Regex(
                        @"(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[0-1])\/(19|20)\d{2} -current$");
                effDOS.Count(x => effDosRegex.IsMatch(x))
                    .ShouldBeGreater(0, "Is Eff DOS is in MM/DD/YYYY-current");

                #endregion
            }
        }

        [Test] //US67967 + CAR-3096(CAR-3202) + CAR-3188(CAR-3203) + CAR-3093(CAR-3212)
        [Author("Shyam")]
        public void Verify_and_validate_edit_rationale_properties()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var appealRationaleDetails = paramList["AppealRationaleData"].Split(';');

                var oldIframeData = new List<string>
                {
                    appealRationaleDetails[3],
                    appealRationaleDetails[4],
                    appealRationaleDetails[5],
                    appealRationaleDetails[6],
                    appealRationaleDetails[10]
                };

                var newIframeData = new List<string>
                {
                    "Test Rationale New",
                    "Test Pay Summary New",
                    "Test Deny Summary New",
                    "Test Adjust Summary New",
                    "Test Helpful Appeal Hints New"
                };

                var errorMessage = paramList["ErrorMessageOnFileUpload"];

                try
                {
                    StringFormatter.PrintMessageTitle("Create new appeal rationale for editing");
                    if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealRationaleManager.GetSideBarPanelSearch.CloseSidebarPanel();
                    _appealRationaleManager.ClickOnAddButton();
                    _appealRationaleManager.CreateAppealRationale(appealRationaleDetails, paramList["CagDate"], clickSave: false);

                    #region CAR-3096

                    StringFormatter.PrintMessageTitle("Verifying upload document functionality in Add Appeal Rationale form");
                    ValidateUploadFunctionality("Test.txt");

                    #endregion
                    _appealRationaleManager.IsPencilIconPresentInEachRationaleRecord()
                        .ShouldBeTrue("Pencil Icon should be present in each record");
                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    _appealRationaleManager.ScrollToLastPosition();
                    _appealRationaleManager.GetSideWindow.GetIFrameData("Helpful Appeal Hints")
                        .ShouldBeEqual(oldIframeData[4], "Old Helpful Appeal Hints Text should be saved.");

                    #region CAR-803

                    _appealRationaleManager.GetSideWindow.GetDateFieldFrom("Effective DOS")
                        .ShouldBeEqual(DateTime.Now.Date.ToString("MM/dd/yyyy"), "Effective Date To Should Empty");

                    _appealRationaleManager.GetSideWindow.GetDateFieldTo("Effective DOS")
                        .ShouldBeNullorEmpty("Effective Date To Should Empty");

                    _appealRationaleManager.GetSideWindow.SetDateFieldTo("Effective DOS",
                        DateTime.Now.ToString("MM/d/yyyy"));

                    _appealRationaleManager.GetSideWindow.SetDateFieldFrom("Effective DOS",
                        DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
                    _appealRationaleManager.WaitToLoadPageErrorPopupModal();
                    _appealRationaleManager.GetPageErrorMessage()
                        .ShouldBeEqual("Please enter a valid date range.");
                    _appealRationaleManager.ClosePageError();
                    _appealRationaleManager.GetSideWindow.SetDateFieldFrom("Effective DOS",
                        DateTime.Now.ToString("MM/d/yyyy"));
                    _appealRationaleManager.GetSideWindow.SetAndTypeDateField("Effective DOS", "", 1);
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.GetPageErrorMessage()
                        .ShouldBeEqual("Effective start date must be entered before the record can be saved.");
                    _appealRationaleManager.GetSideWindow.Cancel();
                    _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);

                    #endregion
                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitToLoadPageErrorPopupModal();
                    _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("No changes have been made.",
                        "Is Popup Message Equals if trying to save without any changes");
                    _appealRationaleManager.ClosePageError();
                    CheckMaxSizeforTextBoxandFrameBody(_appealRationaleManager, false);

                    _appealRationaleManager.GetSideWindow.ClearIFrame("Rationale");
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitToLoadPageErrorPopupModal();
                    _appealRationaleManager.GetPageErrorMessage()
                        .ShouldBeEqual("Rationale text is required before record can be saved.",
                            "Validation for empty Rationale text");
                    _appealRationaleManager.ClosePageError();

                    StringFormatter.PrintMessageTitle("Verification of Edit Icon and Save Button");
                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    FillAllIframeText(newIframeData, _appealRationaleManager);
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitForWorkingAjaxMessage();
                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    VerifyAllIframeText(newIframeData, "updated", _appealRationaleManager);

                    _appealRationaleManager.GetSideWindow.IsTextBoxDisabled("CAG Approval")
                        .ShouldBeTrue("CAG Approval should disabled");
                    _appealRationaleManager.GetSideWindow.IsTextBoxDisabled("Rationale Source")
                        .ShouldBeTrue("Rationale Source should disabled");
                    _appealRationaleManager.GetSideWindow.GetInputFieldText("CAG Approval")
                        .ShouldBeEqual(Convert.ToDateTime(paramList["CagDate"]).ToString("MM/dd/yyyy"),
                            "Is CAG Approval consistent with old record?");

                    FillAllIframeText(oldIframeData, _appealRationaleManager);

                    _appealRationaleManager.GetSideWindow.Cancel();
                    _appealRationaleManager.GetPageErrorMessage()
                        .ShouldBeEqual("Any unsaved data will be discarded. Do you wish to continue?");

                    StringFormatter.PrintMessageTitle("Verification of Cancel button on confirmation popup");
                    _appealRationaleManager.ClickOkCancelOnConfirmationModal(false);
                    _appealRationaleManager.IsEditWindowPresent()
                        .ShouldBeTrue("Is Edit Window Present after pressing cancel on Cancel?");
                    VerifyAllIframeText(oldIframeData, "retain", _appealRationaleManager);

                    StringFormatter.PrintMessageTitle("Verification of Ok button on confirmation popup");
                    _appealRationaleManager.GetSideWindow.Cancel();
                    _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);
                    _appealRationaleManager.IsEditWindowPresent()
                        .ShouldBeFalse("Is Edit Window Present after pressing okay on Cancel?");

                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    VerifyAllIframeText(newIframeData, "not updated", _appealRationaleManager);

                    #region CAR-3188

                    StringFormatter.PrintMessageTitle("Verifying upload document functionality in edit form");
                    ValidateUploadFunctionality("Test 2.txt");

                    #endregion
                    
                    #region LOCAL METHODS

                    void ValidateUploadFunctionality(string fileName)
                    {
                        _appealRationaleManager.GetFileUploadPage.IsUploadInputSectionPresent()
                            .ShouldBeTrue("Upload file section should be present");

                        StringFormatter.PrintMessage("Verifying uploading file with an incompatible file format");
                        _appealRationaleManager.GetFileUploadPage.AddFileForUpload("performance stylesheet.xsl");
                        _appealRationaleManager.GetFileUploadPage.GetSelectedFilesValue()[0]
                            .ShouldBeEqual("performance stylesheet.xsl", "Name of the file to be uploaded is correctly displayed");
                        _appealRationaleManager.GetSideWindow.Save(waitForWorkingMessage: true);
                        _appealRationaleManager.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("Is error popup present when an incompatible file type is uploaded");

                        _appealRationaleManager.GetPageErrorMessage().Replace("\r\n", " ")
                            .ShouldBeEqual(errorMessage, "The error message should be correctly displayed");
                        _appealRationaleManager.ClosePageError();

                        StringFormatter.PrintMessage("Verifying uploading file with a compatible file format");
                        _appealRationaleManager.GetFileUploadPage.AddFileForUpload(fileName);
                        _appealRationaleManager.GetFileUploadPage.GetSelectedFilesValue().Count
                            .ShouldBeEqual(1, "Only one document can be uploaded");
                        _appealRationaleManager.GetFileUploadPage.GetSelectedFilesValue()[0]
                            .ShouldBeEqual(fileName,
                                "When a different file is uploaded, then the name of the prior file is replaced by the new one");

                        _appealRationaleManager.GetSideWindow.Save(waitForWorkingMessage: true);
                        _appealRationaleManager.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("Error popup should not be present when a compatible file type is uploaded");

                        StringFormatter.PrintMessage("Verifying document upload audit record");
                        var docInfoFromDb = _appealRationaleManager
                            .GetUploadedDocInfoFromDB(appealRationaleDetails[0], appealRationaleDetails[1], appealRationaleDetails[2], appealRationaleDetails[8], appealRationaleDetails[9]);

                        docInfoFromDb[0].ShouldBeEqual(fileName, "DB should record the uploaded file name correctly");
                        docInfoFromDb[1].ShouldBeEqual(DateTime.Now.Date.ToString("MM/dd/yyyy"), "DB should record the uploaded date correctly");

                        StringFormatter.PrintMessage("Verifying whether uploaded document can be accessed from the Documents tab");
                        _appealRationaleManager.ShowAppealRationaleAudit(appealRationaleDetails);
                        if(_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                            _appealRationaleManager.GetSideBarPanelSearch.CloseSidebarPanel();
                        _appealRationaleManager.IsDocumentIconPresentInAppealRationaleAuditHx()
                            .ShouldBeTrue("Documents icon is present in Appeal Rationale Audit History Page");

                        _appealRationaleManager.ClickDocumentIconInAppealRationaleAuditHistoryPage();
                        _appealRationaleManager.GetAppealAuditHistoryTitle()
                            .ShouldBeEqual("Documents",
                                "Users can toggle between 'Appeal Rationale Audit History' and 'Documents' tabs by clicking the respective icons");

                        _appealRationaleManager.GetUploadedFileNameFromDocumentsTab()
                            .ShouldBeEqual(fileName, "Uploaded filename must be correctly displayed in Documents tab");

                        _appealRationaleManager.GetUploadedDateFromDocumentsTab()
                            .ShouldBeEqual(DateTime.Now.Date.ToString("MM/dd/yyyy"));

                        _appealRationaleManager.ClickOnFileToOpenInNewTab();

                        _appealRationaleManager.GetWindowHandlesCount()
                            .ShouldBeEqual(2, "When the uploaded file is clicked, it should open in a new tab");

                        _appealRationaleManager.CloseAnyTabIfExist();
                    }

                    #endregion

                }

                finally
                {
                    // Delete created appeal
                    var loadMoreValue = _appealRationaleManager.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
                    var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                    for (var i = 0; i < count; i++)
                    {
                        _appealRationaleManager.ClickOnLoadMore();
                    }
                    _appealRationaleManager.ClickOnDeleteAppealRationale(paramList["AppealRationaleData"].Split(';'));
                    if (_appealRationaleManager.IsEditWindowPresent())
                    {
                        _appealRationaleManager.GetSideWindow.Cancel();
                        _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);
                    }
                }
            }
        }

        [Test, Category("SchemaDependent")]//US67972
        public void Verify_sorting_of_appeal_rationale_search_results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "AppealRationaleManager_Sort_Option").Values
                        .ToList();
                var columnNos = new[] {8, 6, 3, 2};
                //var status = new[] { "Inactive", "Active" };
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var createdAppealRationale = paramList["AppealRationaleData"].Split(';');

                _appealRationaleManager.HardDeleteRationale(createdAppealRationale);
                _appealRationaleManager.ClickOnQuickLaunch().NavigateToAppealRationaleManager();
                try
                {
                    _appealRationaleManager.IsListStringSortedInAscendingOrder(2)
                        .ShouldBeTrue("Result grid should be sorted in Ascending Order of flag by default");
                    _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[0]);

                    int j;
                    //for (var k = 0; k < 2; k++)  // k=2 checks sort for both active and inactive rationale. Set k=1 for testing for only active rationales.
                    //{
                    j = filterOptions.Count - 1;
                    foreach (var column in columnNos) //--reverse oder deu to default sort option is flag 
                    {
                        ValidateAppealRationaleRowSorted(column, filterOptions[--j],_appealRationaleManager);
                    }
                    //_newRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", status[k]);
                    //_newRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
                    //}

                    _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[4]);
                    _appealRationaleManager.IsListStringSortedInAscendingOrder(2)
                        .ShouldBeTrue("Clearing sort should sort results in ascending order of Flag");

                    //StringFormatter.PrintMessage("Clear find rationale filter and go bacak to default result");
                    //_newRationaleManager.GetSideBarPanelSearch.ClickOnClearLink();
                    //_newRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[0]);
                    if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _appealRationaleManager.ToggleSideBarMenu();
                    }

                    _appealRationaleManager.ClickOnAddButton();
                    _appealRationaleManager.CreateAppealRationale(createdAppealRationale);
                    _appealRationaleManager.IsListStringSortedInAscendingOrder(2)
                        .ShouldBeTrue("Saving rationale should sort results in ascending order of Flag");

                    StringFormatter.PrintMessage("Editing rationale should retain search results");
                    j = filterOptions.Count - 1;
                    foreach (var column in columnNos)
                    {
                        ValidateAppealRationaleRowSortisRetainedAfterEdit(column, filterOptions[--j],
                            createdAppealRationale,_appealRationaleManager);
                    }

                }
                finally
                {
                    if (_appealRationaleManager.IsPageErrorPopupModalPresent())
                        _appealRationaleManager.ClosePageError();
                    _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[4]);
                    if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _appealRationaleManager.ToggleSideBarMenu();
                    }
                }
            }

        }

        [Test, Category("AppealDependent")] //CAR-803,CAR-827+CAR-3095(CAR-3213)
        public void Verify_rationale_with_summary_detail_on_appeal_action_based_on_appeal_rationale_with_their_effective_dates_of_service()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                AppealRationaleManagerPage _appealRationaleManager;
                AppealActionPage _appealAction = null;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSequence = paramList["ClaimSequence"];
                var claimSequenceNoData = paramList["ClaimSequenceNoData"];
                var fileName = paramList["FileName"];
                var appealRationaleDetails = paramList["AppealRationaleData"].Split(';');
                var duplicateData = new[]
                {
                    appealRationaleDetails[0],
                    appealRationaleDetails[1],
                    appealRationaleDetails[2],
                    appealRationaleDetails[8],
                    appealRationaleDetails[9],
                    null,
                    null,
                    appealRationaleDetails[3],
                    appealRationaleDetails[7],
                };
                var validEffectiveDos = "01/01/2010";
                var totalCreatedRow = 0;
                try
                {
                    _appealRationaleManager.RefreshPage();
                    StringFormatter.PrintMessageTitle(
                        "Verify Rational should apply when tha appeal line DOS falls on the effective date");
                    if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealRationaleManager.GetSideBarPanelSearch.CloseSidebarPanel();
                    _appealRationaleManager.ClickOnAddButton();
                    _appealRationaleManager.CreateAppealRationale(appealRationaleDetails, effDos: validEffectiveDos,uploadFile:true,fileName:fileName);
                    totalCreatedRow++;
                    _appealAction = _appealRationaleManager.NavigateToAppealSearch()
                        .FindByClaimSequenceToNavigateAppealAction(claimSequence);
                    _appealAction.ClickOnPayIcon();
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleDetails[3],
                            "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(appealRationaleDetails[4],
                            "Pay Summary Text Areas should automatically populated when the appeal line DOS falls on the effective date");

                    _appealAction.ClickOnDenyIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true, 5000);
                    _appealAction.WaitForStaticTime(1000);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleDetails[3],
                            "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(appealRationaleDetails[5],
                            "Deny Summary Text Areas should automatically populated when tha appeal line DOS falls on the effective date");

                    _appealAction.ClickOnAdjustIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true, 5000);
                    _appealAction.WaitForStaticTime(1000);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleDetails[3],
                            "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(appealRationaleDetails[6],
                            "Adjust Summary Text Areas should automatically populated when tha appeal line DOS falls on the effective date");


                    #region CAR 3095
                    StringFormatter.PrintMessage("Verify Appeal Helpful Hints text and documents is shown for the line");
                    _appealAction.ClickOnHelpIcon();
                    _appealAction.IsAppealHelpSectionPresentToTheRight().ShouldBeTrue("Appeal Help section should be shown to the right on the Appeal Action page");
                    _appealAction.IsAppealHelpfulHintsTextPresentInAppealHelpSection()
                        .ShouldBeTrue("Appeal Helpful Hints text should be present");
                    _appealAction.GetAppealHelpfulHintsTextInAppealHelpSection()
                        .ShouldBeEqual(appealRationaleDetails[10],
                            "Text entered on the Appeal Rationale manager for the corresponding appeal rationale selected for the line should be shown");

                    _appealAction.IsAppealRationaleDocumentSectionPresentInAppealHelpSection()
                        .ShouldBeTrue("Appeal Rationale Documents section should be present");
                    _appealAction.GetAppealRationaleDocumentNameInAppealHelpSection()
                        .ShouldBeEqual(fileName,
                            "Documents that have been added to the associated Appeal Rationale for the line should be shown.");
                    
                    StringFormatter.PrintMessage("Verify user should be able to select document to open in a separate browser window.");
                    _appealAction.ClickOnDocumentNameToOpenInNewTab();
                    _appealAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "When the uploaded file is clicked, it should open in a new tab");
                    _appealAction.CloseAnyTabIfExist();


                    StringFormatter.PrintMessage("Verify No Data Found is shown for the line when appeal helpful hints text and appeal rationale document is not present");
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage().FindByClaimSequenceToNavigateAppealAction(claimSequenceNoData);
                    _appealAction.ClickOnHelpIcon();
                    _appealAction.GetAppealHelpfulHintsTextInAppealHelpSection(true)
                        .ShouldBeEqual("No data found.",
                            "No data found should be shown if text doesnt exist.");
                    _appealAction.GetAppealRationaleDocumentNameInAppealHelpSection(true)
                        .ShouldBeEqual("No data found.",
                            "No data found should be shown if appeal rationale document doesn't exist.");

                    #endregion

                    StringFormatter.PrintMessageTitle(
                        "Verify Rational should not apply when the appeal line DOS  does not falls on the effective date");
                    _appealRationaleManager = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage()
                        .NavigateToAppealRationaleManager();

                    validEffectiveDos = "01/02/2010";
                    _appealRationaleManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    _appealRationaleManager.ClickOnEditAppealRationale(appealRationaleDetails);
                    _appealRationaleManager.ScrollToLastPosition();

                    #region CAR-827

                    _appealRationaleManager.GetSideWindow.SetAndTypeDateField("Effective DOS", validEffectiveDos, 1);
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitForWorkingAjaxMessage();
                    _appealRationaleManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("No Error Message display when user input date in datepicker and save");

                    #endregion

                    _appealAction = _appealRationaleManager.NavigateToAppealSearch()
                        .FindByClaimSequenceToNavigateAppealAction(claimSequence);
                    _appealAction.ClickOnPayIcon();
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeNullorEmpty(
                        "Rationale Text Areas should empty when appeal line DOS does not falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary").ShouldBeNullorEmpty(
                        "Pay Summary Text Areas should empty when appeal line DOS does not falls on the effective date");

                    _appealAction.ClickOnDenyIcon();
                    _appealAction.WaitForStaticTime(1000);
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeNullorEmpty(
                        "Rationale Text Areas should empty when appeal line DOS does not falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary").ShouldBeNullorEmpty(
                        "Deny Summary Text Areas should empty when appeal line DOS does not falls on the effective date");

                    _appealAction.ClickOnAdjustIcon();
                    _appealAction.WaitForStaticTime(1000);
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeNullorEmpty(
                        "Rationale Text Areas should empty when appeal line DOS does not falls on the effective date");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary").ShouldBeNullorEmpty(
                        "Adjust Summary Text Areas should empty when appeal line DOS does not falls on the effective date");


                    StringFormatter.PrintMessageTitle(
                        "Verify Rational should not be overlapping in date range for rationales that have matching criteria.");

                    _appealRationaleManager = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage()
                        .NavigateToAppealRationaleManager();

                    _appealRationaleManager.ClickOnAddButton();
                    _appealRationaleManager.FillFormData(duplicateData, true,
                        effectiveDos: new[] {"01/01/2009", "01/02/2010"});
                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitToLoadPageErrorPopupModal();
                    _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual(
                        "A rationale already exists for the criteria selected. Duplicate rationales cannot be created.");
                    _appealRationaleManager.ClosePageError();

                    #region CAR-827

                    _appealRationaleManager.GetSideWindow.SetAndTypeDateField("Effective DOS",
                        Convert.ToDateTime("01/01/2010").ToString("MM/d/yyyy"), 2);

                    #endregion

                    _appealRationaleManager.GetSideWindow.Save();
                    _appealRationaleManager.WaitForWorkingAjaxMessage();
                    _appealRationaleManager.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "There should be no overlap in date range for rationales that have matching criteria");
                    _appealRationaleManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    _appealRationaleManager.CountOfAppealRationaleByRationaleData(appealRationaleDetails)
                        .ShouldBeEqual(2, "New Appeal Rationale Created?");
                    totalCreatedRow++;
                }
                finally
                {
                    _appealRationaleManager =
                        _appealRationaleManager.GetPageHeader() == PageHeaderEnum.AppealAction.GetStringValue()
                            ? _appealAction.ClickOnSearchIconToNavigateAppealSearchPage()
                                .NavigateToAppealRationaleManager()
                            : automatedBase.CurrentPage.NavigateToAppealRationaleManager();

                    if(_appealRationaleManager.GetGridViewSection.IsLoadMorePresent())
                        _appealRationaleManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    // Delete created appeal
                    for (var i = 0; i < totalCreatedRow; i++)
                        _appealRationaleManager.ClickOnDeleteAppealRationale(
                            paramList["AppealRationaleData"].Split(';'));
                }
            }
        }

        [Test] //CAR-328 (CAR-858)
        public void Verify_Appeal_Rationales_Should_Be_Applying_Regardless_Of_Presence_Of_Comma_In_Source_Value()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                
                AppealRationaleManagerPage _appealRationaleManager;
                AppealManagerPage _appealManager;
                AppealCreatorPage _appealCreator;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction = null;
                _appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claseq = paramsList["claseqForSpaceSeparatedEditSrc"];
                var claseqForCommaSeparatedEditSrc = paramsList["claseqForCommaSeparatedEditSrc"];
                var appealRationaleDetails = paramsList["appealRationaleDetails"].Split(';');
                var appealRationaleDetailsToDelete = paramsList["appealRationaleDetailsToDelete"].Split(';');
                var effDosValue = paramsList["effDos"];
                var overrideVal = paramsList["overrideVal"];
                const string sourceWithoutComma = "IUST 1";
                const string sourceWithComma = "IUST,1";

                string[] claseqArray =
                {
                    claseq,
                    claseqForCommaSeparatedEditSrc
                };

                var flagSeqList = paramsList["flagSeqs"].Split(';');

                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verifying whether the Claims in the test data have the expected format for Source Value");
                    _appealRationaleManager.GetEditSrcForClaSeq(claseq, flagSeqList[0])
                        .ShouldBeEqual(sourceWithoutComma,
                            string.Format("Claseq : {0} has source value as : '{1}'", claseq, sourceWithoutComma));
                    _appealRationaleManager.GetEditSrcForClaSeq(claseqForCommaSeparatedEditSrc, flagSeqList[1])
                        .ShouldBeEqual(sourceWithComma,
                            string.Format("Claseq : {0} has source value as : '{1}'", claseqForCommaSeparatedEditSrc,
                                sourceWithComma));

                    StringFormatter.PrintMessage(
                        "Deleting any prior appeal rationale and creating a new one to be linked to the appeal");
                    if (_appealRationaleManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealRationaleManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _appealRationaleManager.ClickOnDeleteAppealRationale(appealRationaleDetailsToDelete);
                    _appealRationaleManager.ClickOnAddButton();
                    _appealRationaleManager.CreateAppealRationale(appealRationaleDetails, effDos: effDosValue,
                        overrideValue: overrideVal);

                    StringFormatter.PrintMessage(
                        "Create an Appeal if there is not present");
                    _appealManager = _appealRationaleManager.NavigateToAppealManager();
                    foreach (var claimSequence in claseqArray)
                    {
                        _appealManager.SearchByClaimSequence(claimSequence);
                        if (_appealManager.GetSearchResultCount() == 0 || _appealManager.IsNoDataMessagePresent())
                            continue;
                        automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                        _appealManager.DeleteAppealsAssociatedWithClaim(claimSequence); //to delete appeal created
                    }

                    StringFormatter.PrintMessageTitle("Creating appeals on the claims");
                    _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();
                    foreach (var claimSequence in claseqArray)
                    {
                        _appealCreator.SearchByClaimSequence(claimSequence);
                        _appealCreator.SelectClaimLine();
                        _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "DocID", "");
                    }

                    StringFormatter.PrintMessageTitle("Verifying the assignment of the appeal rationale to the appeal");
                    //  The first iteration validates the test for claseq with Space Separated EditSrc. 
                    //  The second iteration valides the test for claseq with Comma Separated EditSrc.
                    _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    foreach (var claimSequence in claseqArray)
                    {
                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSequence);
                        _appealAction.ClickOnDenyIcon();
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                            .ShouldBeEqual(appealRationaleDetails[3]);
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                            .ShouldBeEqual(appealRationaleDetails[5],
                                "Newly created Appeal Rationale is being assigned to the appeal");
                        _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                }
                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
                    {
                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealRationaleManager = automatedBase.CurrentPage.NavigateToAppealRationaleManager();
                    _appealRationaleManager.ClickOnDeleteAppealRationale(appealRationaleDetailsToDelete);
                }
            }
        }

        [Test] //CAR-3195(CAR-3242)
        [Author("Pujan Aryal")]
        public void Verify_Pagination_And_Load_More_Icon_Functionality_In_Appeal_Rationale_Manager_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealRationaleManagerPage appealRationaleManager;
                automatedBase.CurrentPage =
                    appealRationaleManager = automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var rowCount = appealRationaleManager.GetGridViewSection.GetGridRowCount();
                rowCount.ShouldBeEqual(25, "Maximum 25 results displayed initially?");
                appealRationaleManager.GetGridViewSection.IsLoadMorePresent().ShouldBeTrue("Is load more icon present?");

                var loadMoreValue = appealRationaleManager.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                var scrollCount = numbers[1] / 25;

                for (int i = 1; i < scrollCount; i++)
                {
                    appealRationaleManager.GetGridViewSection.ClickLoadMore();
                    rowCount += 25;
                    appealRationaleManager.GetGridViewSection.GetGridRowCount()
                        .ShouldBeEqual(rowCount, "25 more results added?");
                }

                if (appealRationaleManager.IsLoadMoreLinkable())
                {
                    appealRationaleManager.GetGridViewSection.ClickLoadMore();
                    appealRationaleManager.GetGridViewSection.GetGridRowCount().ShouldBeEqual(numbers[1]);
                }
            }
        }

        /* TODO : Needs to be uncommented after merged with UAT

         [Test, Category("Acceptance1")]//CAR 783, CAR 284
         public void Verify_All_Centene_option_and_when_an_appeal_rationale_is_created_with_All_Centene_option_the_rationale_will_be_applied_only_to_Centene_clients()
         {
             TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
             IDictionary<string, string> paramList =
                 automatedBase.DataHelper.GetTestData(GetType().FullName, TestExtensions.TestName);
             var claimSequenceCTNSC = paramList["ClaimSequenceCTNSC"];
             var claimSequenceSMTST = paramList["ClaimSequenceSMTST"];
             var appealRationaleDetailsCTNSC = paramList["AppealRationaleDataCTNSC"].Split(';');
             var appealRationaleDetailsSMTST = paramList["AppealRationaleDataSMTST"].Split(';');
             var validEffectiveDos =  paramList["EffectiveDos"];
             var validEffectiveDosCTNSC = paramList["EffectiveDosCTNSC"];
             var totalCreatedRow = 0;

             _newRationaleManager.ClickOnDeleteAppealRationale(appealRationaleDetailsSMTST);


             try
             {
                 StringFormatter.PrintMessageTitle(
                     "Verify Rational should apply when tha appeal line DOS falls on the effective date");
                 _newRationaleManager.ClickOnAddButton();
                 Verify_All_Centene_option_in_the_dropdown("Client", true, false);
                 _newRationaleManager.CreateAppealRationale(appealRationaleDetailsCTNSC, effDos: validEffectiveDosCTNSC);
                 totalCreatedRow++;
                 Verify_All_Centene_option_in_the_dropdown("Client", false);
                 _newRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "All Centene");
                 _newRationaleManager.GetSideBarPanelSearch.ClickOnFindButton();
                 _newRationaleManager.GetGridListValueByCol(7).
                     Any(client => client.Replace("Client:", "").Trim() == "All Centene").ShouldBeTrue("Client should be 'All Centene");

                 _newRationaleManager.ClickFindButtonAndWait();
                 _appealAction = _newRationaleManager.NavigateToAppealSearch()
                     .FindByClaimSequenceToNavigateAppealAction(claimSequenceCTNSC, true, false);
                 _appealAction.ClickOnPayIcon();
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[4],
                         "Pay Summary Text Areas should automatically populated when the appeal line DOS falls on the effective date");
                 _appealAction.ClickOnAdjustIcon();
                 _appealAction.ClickOkCancelOnConfirmationModal(true);
                 _appealAction.WaitForStaticTime(1000);
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[6],
                         "Adjust Summary Text Areas should automatically populated when tha appeal line DOS falls on the effective date");



                 StringFormatter.PrintMessageTitle("Verify for other clients the rationale should not apply");
                 _appealAction.ClickOnSearchIconToNavigateAppealSearchPage()
                     .FindByClaimSequenceToNavigateAppealAction(claimSequenceSMTST);
                 _appealAction.ClickOnPayIcon();
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeNullorEmpty("Rationale details should be null");

                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeNullorEmpty("Rationale details should be null");

                 _appealAction.ClickOnAdjustIcon();
                 _appealAction.ClickOkCancelOnConfirmationModal(true);
                 _appealAction.WaitForStaticTime(1000);
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeNullorEmpty("Rationale details should be null");

                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeNullorEmpty("Rationale details should be null");


                 StringFormatter.PrintMessageTitle(
                     "Create a new rationale for SMTST client and verify the rationale should apply to only SMTST client");
                 _appealAction.ClickOnSearchIconToNavigateAppealSearchPage().NavigateToNewAppealRationaleManager();
                 _newRationaleManager.ClickOnAddButton();
                 _newRationaleManager.CreateAppealRationale(appealRationaleDetailsSMTST, effDos: validEffectiveDos);
                 totalCreatedRow++;
                 _appealAction = _newRationaleManager.NavigateToAppealSearch()
                     .FindByClaimSequenceToNavigateAppealAction(claimSequenceSMTST);
                 _appealAction.ClickOnPayIcon();
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsSMTST[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsSMTST[4],
                         "Pay Summary Text Areas should automatically populated when the appeal line DOS falls on the effective date");
                 _appealAction.ClickOnAdjustIcon();
                 _appealAction.ClickOkCancelOnConfirmationModal(true);
                 _appealAction.WaitForStaticTime(1000);
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsSMTST[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsSMTST[6],
                         "Adjust Summary Text Areas should automatically populated when tha appeal line DOS falls on the effective date");

                 StringFormatter.PrintMessage(
                     "Delete the rationale for SMTST client and Verify the rationale for ALL CENTENE option");
                 _appealAction.ClickOnSearchIconToNavigateAppealSearchPage().NavigateToNewAppealRationaleManager();
                 _newRationaleManager.ClickOnDeleteAppealRationale(appealRationaleDetailsSMTST);
                 _appealAction = _newRationaleManager.NavigateToAppealSearch()
                     .FindByClaimSequenceToNavigateAppealAction(claimSequenceCTNSC, true, false);
                 _appealAction.ClickOnPayIcon();
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[4],
                         "Pay Summary Text Areas should automatically populated when the appeal line DOS falls on the effective date");
                 _appealAction.ClickOnAdjustIcon();
                 _appealAction.ClickOkCancelOnConfirmationModal(true);
                 _appealAction.WaitForStaticTime(1000);
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[3],
                         "Rationale Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                     .ShouldBeEqual(appealRationaleDetailsCTNSC[6],
                         "Adjust Summary Text Areas should automatically populated when tha appeal line DOS falls on the effective date");
             }

             finally
             {
                 _newRationaleManager =
                     _newRationaleManager.GetPageHeader() == PageHeaderEnum.AppealAction.GetStringValue()
                         ? _appealAction.ClickOnSearchIconToNavigateAppealSearchPage()
                             .NavigateToNewAppealRationaleManager()
                         : CurrentPage.NavigateToNewAppealRationaleManager();

             }

         }
         */

        [Test] //CV-3444(CV-3632)
        [Author("Pujan Aryal")]
        public void Verify_Appeal_Rationales_Are_Applied_Based_On_Form_Type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealRationaleManagerPage appealRationaleManager = 
                    automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.
                    GetTestData(GetType().FullName, TestName);
                var claimSequence = paramsList["ClaimSequence"];
                var appealRationaleText = paramsList["AppealRationaleText"];
                var appealRationaleData = paramsList["AppealRationaleData"].Split(';');
                appealRationaleManager.UpdateAppealFormTypeForARationaleFromDb(appealRationaleData, "All");
                appealRationaleManager.UpdateClaimFormTypeFromDb(claimSequence, "H");

                try
                {
                    StringFormatter.PrintMessage("Verify rationale is applied when form type is All");
                    appealRationaleManager.GetAppealFormTypeForARationaleFromDb(appealRationaleData)
                        .ShouldBeEqual(string.Empty, "Form Type Should Be All");
                    appealRationaleManager.GetClaimFormTypeFromDb(claimSequence)
                        .ShouldBeEqual("H", "Claim Form Type Should Be 'H'");
                    AppealActionPage appealAction = appealRationaleManager.NavigateToAppealSearch()
                        .FindByClaimSequenceToNavigateAppealAction(claimSequence);
                    appealAction.ClickOnPayIcon();
                    appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleText,
                            "Rationale is applied when form type is All");

                    StringFormatter.PrintMessage("Changing Appeal Rationale Form Type to 'H'");
                    appealRationaleManager.UpdateAppealFormTypeForARationaleFromDb(appealRationaleData, "H");

                    StringFormatter.PrintMessage(
                        "Verify rationale should be applied when appeal rationale form type is H " +
                        "and claim form type is also H");
                    appealRationaleManager.GetAppealFormTypeForARationaleFromDb(appealRationaleData)
                        .ShouldBeEqual("H", "Form Type Should Be H");
                    appealRationaleManager.GetClaimFormTypeFromDb(claimSequence)
                        .ShouldBeEqual("H", "Claim Form Type Should Be 'H'");
                    appealAction.RefreshPage(false);
                    appealAction.HandleAutomaticallyOpenedActionPopup();
                    appealAction.ClickOnPayIcon();
                    appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleText,
                            "Rationale should be applied when appeal rationale form type is H and claim form type is also H");

                    StringFormatter.PrintMessage("Changing Appeal Rationale Form Type to 'U'");
                    appealRationaleManager.UpdateAppealFormTypeForARationaleFromDb(appealRationaleData, "U");

                    StringFormatter.PrintMessage(
                        "Verify rationale should not be applied when appeal rationale form type is U " +
                        "and claim form type is H");
                    appealRationaleManager.GetAppealFormTypeForARationaleFromDb(appealRationaleData)
                        .ShouldBeEqual("U", "Form Type Should Be U");
                    appealRationaleManager.GetClaimFormTypeFromDb(claimSequence)
                        .ShouldBeEqual("H", "Claim Form Type Should Be 'H'");
                    appealAction.RefreshPage(false);
                    appealAction.HandleAutomaticallyOpenedActionPopup();
                    appealAction.ClickOnPayIcon();
                    appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual("",
                            "Rationale should not be applied when appeal rationale form type is U and claim form type is H");

                    StringFormatter.PrintMessage("Changing Claim Form Type to 'U'");
                    appealRationaleManager.UpdateClaimFormTypeFromDb(claimSequence, "U");

                    StringFormatter.PrintMessage(
                        "Verify rationale should be applied when appeal rationale form type is U " +
                        "and claim form type is also U");
                    appealRationaleManager.GetAppealFormTypeForARationaleFromDb(appealRationaleData)
                        .ShouldBeEqual("U", "Form Type Should Be U");
                    appealRationaleManager.GetClaimFormTypeFromDb(claimSequence)
                        .ShouldBeEqual("U", "Claim Form Type Should Be 'U'");
                    appealAction.RefreshPage(false);
                    appealAction.HandleAutomaticallyOpenedActionPopup();
                    appealAction.ClickOnPayIcon();
                    appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(appealRationaleText,
                            "Rationale should be applied when appeal rationale form type is U and claim form type is also U");

                    StringFormatter.PrintMessage("Changing Appeal Rationale Form Type to 'H'");
                    appealRationaleManager.UpdateAppealFormTypeForARationaleFromDb(appealRationaleData, "H");

                    StringFormatter.PrintMessage(
                        "Verify rationale should not be applied when appeal rationale form type is H " +
                        "and claim form type is U");
                    appealRationaleManager.GetAppealFormTypeForARationaleFromDb(appealRationaleData)
                        .ShouldBeEqual("H", "Form Type Should Be H");
                    appealRationaleManager.GetClaimFormTypeFromDb(claimSequence)
                        .ShouldBeEqual("U", "Claim Form Type Should Be 'U'");
                    appealAction.RefreshPage(false);
                    appealAction.HandleAutomaticallyOpenedActionPopup();
                    appealAction.ClickOnPayIcon();
                    appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual("",
                            "Rationale should not be applied when appeal rationale form type is H and claim form type is U");

                }
                finally
                {
                    StringFormatter.PrintMessage("Reverting back to initial setting");
                    appealRationaleManager.UpdateAppealFormTypeForARationaleFromDb(appealRationaleData, "All");
                    appealRationaleManager.UpdateClaimFormTypeFromDb(claimSequence, "H");
                }

            }
        }

        #endregion

        #region Private methods

        private void VerifyAllIframeText(IList<string> iframText, string message,AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.GetSideWindow.GetIFrameData("Rationale")
                .ShouldBeEqual(iframText[0], "Rationale Text should " + message);
            _appealRationaleManager.GetSideWindow.GetIFrameData("Pay Summary")
                .ShouldBeEqual(iframText[1], "Pay Summary Text should " + message);
            _appealRationaleManager.GetSideWindow.GetIFrameData("Deny Summary")
                .ShouldBeEqual(iframText[2], "Deny Summary Text should " + message);
            _appealRationaleManager.GetSideWindow.GetIFrameData("Adjust Summary")
                .ShouldBeEqual(iframText[3], "Adjust Summary Text should " + message);
            _appealRationaleManager.GetSideWindow.GetIFrameData("Helpful Appeal Hints")
                .ShouldBeEqual(iframText[4], "Helpful Appeal Hints Text should " + message);
        }

        private void FillAllIframeText(IList<string> iframText,AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.GetSideWindow.FillIFrame("Rationale", iframText[0]);
            _appealRationaleManager.GetSideWindow.FillIFrame("Pay Summary", iframText[1]);
            _appealRationaleManager.GetSideWindow.FillIFrame("Deny Summary", iframText[2]);
            _appealRationaleManager.GetSideWindow.FillIFrame("Adjust Summary", iframText[3]);
            _appealRationaleManager.GetSideWindow.FillIFrame("Helpful Appeal Hints", iframText[4]);
        }

        private void ValidateDefaultValueForSideBarPanelFilters(AppealRationaleManagerPage _appealRationaleManager)
        {
            StringFormatter.PrintMessageTitle("Validate For default value of Find Rationale Filter  panel fields");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Status")
                .ShouldBeEqual("Active", "Default value is as expected for Filter type Status");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Client")
               .ShouldBeNullorEmpty("Client value defaults to blank ");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Flag")
                .ShouldBeNullorEmpty("Flag value defaults to blank ");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Source Value")
                .ShouldBeNullorEmpty("Source Value defaults to blank ");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Modifier")
                .ShouldBeNullorEmpty("Modifier value defaults to blank ");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code")
                .ShouldBeNullorEmpty("Proc Code field value defaults to blank ");
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel("Trigger Code")
                .ShouldBeNullorEmpty("Trigger Code field value defaults to blank ");
        }
        private void ValidateDropDownFoDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, AppealRationaleManagerPage _appealRationaleManager, bool noAll = true)
        {
            StringFormatter.PrintMessageTitle("Verification of " + label + " drop down field");
            var actualDropDownList = _appealRationaleManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
            {
                actualDropDownList.RemoveAll(item => item == "");
                if (noAll) actualDropDownList.Remove("All");
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List is As Expected");
            }
            actualDropDownList.Remove("All");
            //actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _appealRationaleManager.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
        }
        private void InputValuesUnderFindPanelWithGivenParameter(IList<string> paramList,AppealRationaleManagerPage _appealRationaleManager)//status, client, flag, source, modifier, proc, trigger code
        {
            StringFormatter.PrintMessageTitle("Validate Search Result For Given Filter Parameter");
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", paramList[0]);
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramList[1]);
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", paramList[2]);
            _appealRationaleManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Source Value", paramList[3]);
            _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Modifier", paramList[4]);
            _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", paramList[5]);
            _appealRationaleManager.GetSideBarPanelSearch.SetInputFieldByLabel("Trigger Code", paramList[6]);

        }


        private void CheckClientFlagSourceValueandOverrideFieldsSortingandDefaultValue(AppealRationaleManagerPage _appealRationaleManager)
        {
            if (!_appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent()) _appealRationaleManager.ClickOnAddButton();


            //Check for Client Sort and Default Values          
            CheckAlphabeticalOrderByLabel("Client", _appealRationaleManager.GetActiveClientList(),_appealRationaleManager);

            //Check for Flag Sort and Default Values     
            CheckAlphabeticalOrderByLabel("Flag", _appealRationaleManager.GetFlagLists(),_appealRationaleManager);

            //Check for Source Value Sort and Default Values 
            var listdb = _appealRationaleManager.GetSourceLists();
            listdb.Sort();
            CheckAlphabeticalOrderByLabel("Source Value", listdb,_appealRationaleManager);


            //Check for Source Value Sort and Default Values 
            var list = _appealRationaleManager.GetSideWindow.GetDropDownList("Override ");
            list.RemoveAt(0);
            list.ShouldBeEqual(new List<string>(new String[] { "0", "1" }), "Does list contain 0,1");
        }

        public void CheckAlphabeticalOrderByLabel(string label, IList<string> collectionToEqual, AppealRationaleManagerPage _appealRationaleManager)
        {
            var list = _appealRationaleManager.GetSideWindow.GetDropDownList(label);
            if (label == "Client") list.RemoveRange(0, 2);
            else list.RemoveAt(0);

            // list.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order."); ;
            list.ShouldCollectionBeEqual(collectionToEqual, label + " List is As Expected");
            if (label == "Client")
                _appealRationaleManager.GetSideWindow.GetDropDownListDefaultValue(label).ShouldBeEqual("All", string.Format("Is {0}'s default value All?", label));


        }

        private void DoFieldsAllowOnlyNCharacters(AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.Check_Maxlength_by_Label("Proc Code From", "5").ShouldBeTrue("Is Proc Code From 5 characters long?");
            _appealRationaleManager.Check_Maxlength_by_Label("Proc Code To", "5").ShouldBeTrue("Is Proc Code To 5 characters long?");
            _appealRationaleManager.Check_Maxlength_by_Label("Trig Proc From", "5").ShouldBeTrue("Is Trig Code From 5 characters long?");
            _appealRationaleManager.Check_Maxlength_by_Label("Trig Proc To", "5").ShouldBeTrue("Is Trig Code To 5 characters long?");
            _appealRationaleManager.Check_Maxlength_by_Label("Modifier", "2").ShouldBeTrue("Is Modifier 2 characters long?");
            _appealRationaleManager.Check_Maxlength_by_Label("Rationale Source", "200").ShouldBeTrue("Is Modifier 200 characters long?");
        }


        private void CheckMaxSizeforTextBoxandFrameBody(AppealRationaleManagerPage _appealRationaleManager,bool addRationale = true)
        {
            var labels = new[] { "Rationale", "Pay Summary", "Deny Summary", "Adjust Summary","Helpful Appeal Hints" };
            if (addRationale)
            {
                if (!_appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent()) _appealRationaleManager.ClickOnAddButton();
            }

            var descp = new string('a', 3994);
            for (var i = 0; i < labels.Length; i++)
            {
                if (i == 0||i==4)
                {
                    _appealRationaleManager.GetSideWindow.FillIFrame(labels[i], descp, 3994);
                    _appealRationaleManager.GetSideWindow.GetIFrameData(labels[i]).Length
                        .ShouldBeEqual(3993,
                            string.Format("Character length for {0} should not exceed more than 4000", labels[i]));
                }
                else
                {
                    var newdescp = new string('a', 4494);
                    _appealRationaleManager.GetSideWindow.FillIFrame(labels[i], newdescp, 4494);
                    _appealRationaleManager.GetSideWindow.GetIFrameData(labels[i]).Length
                        .ShouldBeEqual(4493,
                            string.Format("Character length for {0} should not exceed more than 4500", labels[i]));
                }
            }
            if (addRationale)
                _appealRationaleManager.GetSideWindow.GetInputBox("Rationale Source").GetAttribute("maxlength").ShouldBeEqual("200");
        }


        private void CheckCAGApprovalDateFormat(string date,AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.GetSideWindow.GetInputBox("CAG Approval").Click();

            _appealRationaleManager.GetCalendarPage.SetDate(date);
            _appealRationaleManager.GetSideWindow.GetInputFieldText("CAG Approval").IsDateInFormat().ShouldBeTrue("Begin Date should show as MM/DD/YYYY");


        }

        public void VerifyCreateEditDeleteOnAuditLogs(string value,AppealRationaleManagerPage _appealRationaleManager)
        {

            _appealRationaleManager.GetAppealAuditHistoryActionByRowCol(1, 1).ShouldBeEqual(value, "Is Action field valid?");
            _appealRationaleManager.GetAppealAuditHistoryActionByRowCol(1, 2)
                .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"), "Is Modified Date field valid?");
            _appealRationaleManager.GetAppealAuditHistoryActionByRowCol(1, 3).ShouldBeEqual("Test Automation", "Is Modified By field valid?");

        }


        private void CheckDuplicatesOnSave(string[] duplicateData, string[] overlappingData, AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.FillDuplicateData(duplicateData);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("A rationale already exists for the criteria selected. Duplicate rationales cannot be created.");
            _appealRationaleManager.ClosePageError();

            _appealRationaleManager.FillOverlappingData(overlappingData);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("A rationale already exists for the criteria selected. Duplicate rationales cannot be created.");
            _appealRationaleManager.ClosePageError();


            _appealRationaleManager.FillOverlappingData(overlappingData, false);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("A rationale already exists for the criteria selected. Duplicate rationales cannot be created.");
            _appealRationaleManager.ClosePageError();

            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc From").ClearElementField();
            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
        }


        private void VerifyIfRequiredFieldsArePresent(AppealRationaleManagerPage _appealRationaleManager)
        {
            StringFormatter.PrintMessage("Verification for presence of required fields on Adding Appeal Rationale"); //US68643

            _appealRationaleManager.GetSideWindow.IsAsertiskPresent("Flag")
                .ShouldBeTrue("Is Asterisk Present on Flag?");

            _appealRationaleManager.GetSideWindow.IsAsertiskPresent("Source Value")
                .ShouldBeTrue("Is Asterisk Present on Source Value?");


            _appealRationaleManager.GetSideWindow.GetPlaceHolderText("Flag")
                .ShouldBeEqual("Select Flag", "Is Flag's default text Select Flag?");

            _appealRationaleManager.GetSideWindow.GetPlaceHolderText("Source Value")
                .ShouldBeEqual("Select Source", "Is Source Value's default text Select Source?");

            #region CAR-803
            _appealRationaleManager.GetSideWindow.IsAsertiskPresent("Effective DOS")
                .ShouldBeTrue("Is Asterisk Present on Effective DOS?");
            
            _appealRationaleManager.GetSideWindow.GetPlaceHolderTextForDate("Effective DOS")
                .ShouldBeEqual("MM/DD/YYYY", "Is Placeholder of Effective DOS From equal?");
            _appealRationaleManager.GetSideWindow.GetPlaceHolderTextForDate("Effective DOS",2)
                .ShouldBeEqual("MM/DD/YYYY", "Is Placeholder of Effective DOS To equal?");
            #endregion
            

            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            SiteDriver.WaitToLoadNew(2000);
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Flag must be selected before the record can be saved."); //US69384
            _appealRationaleManager.ClosePageError();

            _appealRationaleManager.GetSideWindow.SelectDropDownValue("Flag", "ADD");


            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Source Value must be selected before the record can be saved."); //US69384
            _appealRationaleManager.ClosePageError();

            _appealRationaleManager.GetSideWindow.SelectDropDownValue("Source Value", "ACS");

            #region CAR-803
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Effective start date must be entered before the record can be saved.");
            _appealRationaleManager.ClosePageError();
            _appealRationaleManager.GetSideWindow.SetDateFieldTo("Effective DOS", DateTime.Now.ToString("MM/d/yyyy"));

            _appealRationaleManager.GetSideWindow.SetDateFieldFrom("Effective DOS", DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Please enter a valid date range.");
            _appealRationaleManager.ClosePageError();
            _appealRationaleManager.GetSideWindow.SetDateFieldFrom("Effective DOS", DateTime.Now.ToString("MM/d/yyyy"));
            #endregion

            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            SiteDriver.WaitToLoadNew(2000);
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Rationale must be entered before the record can be saved.");
            _appealRationaleManager.ClosePageError();
            _appealRationaleManager.GetSideWindow.GetInputBox("Rationale Source").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillIFrame("Rationale", "test");
            SiteDriver.WaitToLoadNew(2000);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage()
                .ShouldBeEqual("Rationale Source must be entered before the record can be saved.");
            _appealRationaleManager.ClosePageError();

        }

        private void VerifyProcTrigandModifierValues(string[] duplicateData,AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.FillDuplicateData(duplicateData);
            /* _newRationaleManager.GetSideWindow.FillInputBox("Proc Code From", "12%23");
             _newRationaleManager.WaitToLoadPageErrorPopupModal();
             _newRationaleManager.GetPageErrorMessage().ShouldBeEqual("Only alphanumerics allowed.");
             _newRationaleManager.ClosePageError();
             _newRationaleManager.GetSideWindow.FillInputBox("Proc Code To", "12%23");
             _newRationaleManager.GetPageErrorMessage().ShouldBeEqual("Only alphanumerics allowed.");
             _newRationaleManager.ClosePageError();
             _newRationaleManager.GetSideWindow.GetInputBox("Proc Code From").ClearElementField();


             _newRationaleManager.GetSideWindow.FillInputBox("Trig Proc From", "12%23");
             _newRationaleManager.WaitToLoadPageErrorPopupModal();
             _newRationaleManager.GetPageErrorMessage().ShouldBeEqual("Only alphanumerics allowed.");
             _newRationaleManager.ClosePageError();
             _newRationaleManager.GetSideWindow.FillInputBox("Trig Proc To", "12%23");
             _newRationaleManager.GetPageErrorMessage().ShouldBeEqual("Only alphanumerics allowed.");
             _newRationaleManager.ClosePageError();
             _newRationaleManager.GetSideWindow.GetInputBox("Trig Proc From").ClearElementField();*/

            _appealRationaleManager.GetSideWindow.FillInputBox("Proc Code From", "12423"); //US69383
            _appealRationaleManager.GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.");
            _appealRationaleManager.ClosePageError();
            _appealRationaleManager.GetSideWindow.FillInputBox("Proc Code To", "12426");


            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillInputBox("Trig Proc From", "12423"); //US69383

            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.WaitToLoadPageErrorPopupModal();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("Valid procedure codes must be entered in the Trig Code From, Trig Code To fields before the record can be saved.");
            _appealRationaleManager.ClosePageError();


            /*  _newRationaleManager.GetSideWindow.FillInputBox("Modifier", "AB"); // Not a valid Modifier
              _newRationaleManager.GetSideWindow.Save();
              _newRationaleManager.WaitToLoadPageErrorPopupModal();
              _newRationaleManager.GetPageErrorMessage().ShouldBeEqual("Modifier value must be valid to create a rationale.");
              _newRationaleManager.ClosePageError();*/

            StringFormatter.PrintMessage("Verifying if proc/trig code TO values are autopopulated on TAB or Click Away"); //US69384


            _appealRationaleManager.GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillInputBox("Proc Code From", "12345");

            _appealRationaleManager.GetSideWindow.FillInputBox("Modifier", "");
            _appealRationaleManager.GetSideWindow.GetInputFieldText("Proc Code To").ShouldBeEqual("12345",
                "Is auto populated Proc Code To value equal to Proc Code From on click-away?");

            _appealRationaleManager.GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            _appealRationaleManager.GetSideWindow.GetInputBox("Proc Code From").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillInputBox("Proc Code From", "12345", true);
            _appealRationaleManager.GetSideWindow.GetInputFieldText("Proc Code To").ShouldBeEqual("12345",
                "Is auto populated Proc Code To value equal to Proc Code From on TAB?");

            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillInputBox("Trig Proc From", "12345");
            _appealRationaleManager.GetSideWindow.FillInputBox("Modifier", "");
            _appealRationaleManager.GetSideWindow.GetInputFieldText("Trig Proc To").ShouldBeEqual("12345",
                "Is auto populated Trig Proc To value equal to Trig Proc From on click-away?");

            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
            _appealRationaleManager.GetSideWindow.GetInputBox("Trig Proc From").ClearElementField();
            _appealRationaleManager.GetSideWindow.FillInputBox("Trig Proc From", "12345", true);
            _appealRationaleManager.GetSideWindow.GetInputFieldText("Trig Proc To").ShouldBeEqual("12345",
                "Is auto populated Trig Proc To value equal to Trig Proc From on TAB?");



        }


        private void VerifyAddingOfRationaleandCancelButton(string[] AppealRationaleData, AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.IsHelpfulAppealHintsTextBoxPresent().ShouldBeTrue("Is Helpful Appeal Hints Textbox present ?");
            
            #region CV-3444
            _appealRationaleManager.GetSideWindow.IsDropDownPresentByLabel("Form Type ").ShouldBeTrue("Form Type field should be added");
            _appealRationaleManager.GetSideWindow.GetDropDownInputFieldByLabel("Form Type ").ShouldBeEqual("All","By default all should be selected");
            _appealRationaleManager.GetSideWindow.GetAvailableDropDownList("Form Type ").
                ShouldCollectionBeEqual(new List<string>{"All","U","H"},"Dropdown Option Should Match");
            #endregion

            _appealRationaleManager.CreateAppealRationale(AppealRationaleData,formType:true);
            Regex.Replace(_appealRationaleManager.GetAppealHelpfulHintsForARationaleFromDb(AppealRationaleData), "<.*?>",String.Empty).ShouldBeEqual(AppealRationaleData[10],"Appeal Helpful Hints should be saved in db");
            
            #region CV-3444
            _appealRationaleManager.GetAppealFormTypeForARationaleFromDb(new string[] { AppealRationaleData[0],AppealRationaleData[1], AppealRationaleData[2], AppealRationaleData[8], AppealRationaleData[9] }).
                ShouldBeEqual(AppealRationaleData[11], "Appeal Form Type should be saved in db");

            StringFormatter.PrintMessage("Verify form type on primary details display for the added rationale record");
            _appealRationaleManager.GetFormTypeForAppealRationale(AppealRationaleData).
                ShouldBeEqual(AppealRationaleData[11], "Form Type value on primary details display should match");
            #endregion

            _appealRationaleManager.ClickOnDeleteAppealRationale(AppealRationaleData).ShouldBeTrue("Is Rationale Created?");

            // Verify clicking ok after cancel
            _appealRationaleManager.ClickOnAddButton();
            _appealRationaleManager.GetSideWindow.Cancel();
            _appealRationaleManager.GetPageErrorMessage().ShouldBeEqual("Unsaved changes will be discarded. Do you wish to continue?");
            _appealRationaleManager.ClickOkCancelOnConfirmationModal(true);
            _appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeFalse("Does pressing OK disregard data?");

            // Verify clicking Cancel after cancel
            _appealRationaleManager.ClickOnAddButton();
            _appealRationaleManager.GetSideWindow.Cancel();
            _appealRationaleManager.ClickOkCancelOnConfirmationModal(false);
            _appealRationaleManager.GetSideWindow.IsSideWindowBlockPresent().ShouldBeTrue("Does pressing cancel disregard data?");

        }


        private void ValidateAppealRationaleRowSorted(int col, string filterBy, AppealRationaleManagerPage _appealRationaleManager)
        {
            _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterBy);
            _appealRationaleManager.IsListStringSortedInAscendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Ascending Order", filterBy));
            _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterBy);
            _appealRationaleManager.IsListStringSortedInDescendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Descending Order", filterBy));
        }


        private void ValidateAppealRationaleRowSortisRetainedAfterEdit(int col, string filterBy, string[] createdAppeal, AppealRationaleManagerPage _appealRationaleManager)
        {
            StringFormatter.PrintMessage("Generating a random value to make edit to rationale, as application wont save edit unless there is new change");
            var randVal = new Random();
            var editChange = randVal.Next(0, 10);
            var editChangePlus = randVal.Next(10, 20);
            _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterBy);
            var loadMoreValue = _appealRationaleManager.GetLoadMoreText();
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
            var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
            for (var i = 0; i < count; i++)
            {
                _appealRationaleManager.ClickOnLoadMore();
            }
            _appealRationaleManager.ClickOnEditAppealRationale(createdAppeal);
            _appealRationaleManager.GetSideWindow.FillIFrame("Pay Summary", "Test Pay Summary (edited for sort result test )" + editChange + editChangePlus);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.IsListStringSortedInAscendingOrder(col)
                .ShouldBeTrue(string.Format("Sort should be retained once user saves an edited record, result should be sorted in ascending order of {0}", filterBy));
            _appealRationaleManager.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterBy);
            loadMoreValue = _appealRationaleManager.GetLoadMoreText();
            numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
            count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
            for (var i = 0; i < count; i++)
            {
                _appealRationaleManager.ClickOnLoadMore();
            }
            _appealRationaleManager.ClickOnEditAppealRationale(createdAppeal);
            _appealRationaleManager.GetSideWindow.FillIFrame("Pay Summary", "Test Pay Summary (edited for sort result test)" + editChange + editChangePlus);
            _appealRationaleManager.GetSideWindow.Save();
            _appealRationaleManager.IsListStringSortedInDescendingOrder(col)
                .ShouldBeTrue(string.Format("Sort should be retained once user saves an edited record, result should be sorted in descending order of {0}", filterBy));
        }

        private void Verify_All_Centene_option_in_the_dropdown(string label, AppealRationaleManagerPage _appealRationaleManager, NewEnvironmentManager EnvironmentManager, bool addform = true,  bool sidebarpanel = true)
        {
            List<string> _assignedClientList = _appealRationaleManager.GetAssignedClientList(EnvironmentManager.HciAdminUsername);

            if (addform )
            {
                var list = _appealRationaleManager.GetSideWindow.GetDropDownList(label);
                if (_assignedClientList != null)
                {
                    list.ShouldContain("All Centene", "All Centene option should be present in the list");
                    list.IndexOf("All Centene", 2);
                }
                else
                {
                    list.ShouldNotContain("All Centene", "All Centene option should not be present");
                }
            }

            if (sidebarpanel)
            {
                var actualDropDownList = _appealRationaleManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                if (_assignedClientList != null)
                {
                    actualDropDownList.ShouldContain("All Centene", "All Centene option should be present in the list");
                    actualDropDownList.IndexOf("All Centene", 2);
                }
                else
                {
                    actualDropDownList.ShouldNotContain("All Centene", "All Centene option should not be present");
                }
            }

        }

        #endregion
    }
}
