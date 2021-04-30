using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Reference;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class Repository : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private RepositoryPage _repository;
        private ProfileManagerPage _userProfileManager;
        private CommonValidations _commonValidations;


        #endregion

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _repository = QuickLaunch.NavigateToRepository();
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
            CurrentPage = _repository;
        }

        protected override void TestCleanUp()
        {
            if (_repository.IsPageErrorPopupModalPresent())
                _repository.ClosePageError();

            if(_repository.IsClearLinkPresent())
                _repository.GetSideBarPanelSearch().ClickOnClearLink();

            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _repository = _repository.ClickOnQuickLaunch()
                    .Logout()
                    .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient)
                    .NavigateToRepository();

            }
            
            

        }

      

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

        #region TEST SUITES
        [Test, Category("SmokeTest")] //TANT-204
        public void Verify_red_highlight_for_invalid_input()
        {
            StringFormatter.PrintMessageTitle("Verify whether red highlight is present on proc code begin field when left empty");
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");
            SearchAddOn("", "");
            if (_repository.IsPageErrorPopupModalPresent())
                _repository.ClosePageError();
            _repository.IsInvalidInputPresentByLabel("Proc Code Range").ShouldBeTrue("'Proc Code Range' begin field should be highlighted red" +
                                                                                     "if searched with this left empty");
        }

        [Test, Category("SmokeTestDeployment")] //TANT-100 
        public void Verify_Repository_Page()
        {
            StringFormatter.PrintMessage("Verify Repository Page is presented");
            _repository.GetPageHeader().ShouldBeEqual(PageHeaderEnum.Repository.GetStringValue(), "Repository Page Should Be Presented");

            StringFormatter.PrintMessage("Verify Data is returned");
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");

            SearchAddOn("00000", "11111");
            _repository.GetGridViewSection().GetGridRowCount().ShouldBeGreater(0, "Search Result should be returned.");
        }

        [Test] //US69676
        public void Security_and_navigation()
        {
            _commonValidations.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Reference,
                new List<string> { SubMenu.Repository },
                RoleEnum.ClaimsProcessor.GetStringValue(),

                new List<string> { PageHeaderEnum.Repository.GetStringValue() },
                Login.LoginAsUserHavingNoAnyAuthority, new[] { "Test4", "Automation4" });


            ////TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //List<string> referenceSubMenuItems = new List<string>(new string[] { "Repository", "Reference Manager" });
            //try
            //{
            //    _repository.GetPageHeader().ShouldBeEqual("Repository");
            //    StringFormatter.PrintMessageTitle(
            //        "Verifying whether the user has privileges to view the Repository tab");
            //    CurrentPage = _userProfileManager = QuickLaunch.NavigateToProfileManager();
            //    _userProfileManager.ClickOnPrivileges();
            //    _userProfileManager.IsReadWriteAssigned("Repository")
            //        .ShouldBeTrue("User is assiged with Read/Write privilege for 'Repository'");
            //    StringFormatter.PrintMessageTitle("Verifying if Reference tab has Repository as its sub menu");
            //    _repository.GetReferenceSubMenuList()
            //        .ShouldCollectionBeEqual(referenceSubMenuItems, "Repository is a sub menu of Reference");
            //    _userProfileManager.ClickOnQuickLaunch();
            //    StringFormatter.PrintMessageTitle("Verifying the 'Reference' tab is between Batch and Invoice tabs");
            //    var mainNavMenuItems = QuickLaunch.GetMainNavigationMenuTabList();
            //    var referenceIndex = mainNavMenuItems.FindIndex("Reference".Equals);
            //    mainNavMenuItems[referenceIndex - 1].ShouldBeEqual("Batch");
            //    mainNavMenuItems[referenceIndex + 1].ShouldBeEqual("Reports");
            //    StringFormatter.PrintMessageTitle(
            //        "Verifying User who has read-only privilege to Repository cannot view the Reference tab");
            //    _repository.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities();
            //    CurrentPage = _userProfileManager = QuickLaunch.NavigateToProfileManager();
            //    _userProfileManager.ClickOnPrivileges();
            //    _userProfileManager.IsReadOnlyAssgined("Repository")
            //        .ShouldBeTrue("User is assiged with Read-Only privilege for 'Repository'");
            //    QuickLaunch.IsRepositorySubMenuPresent()
            //        .ShouldBeFalse("Read-Only user is not able to view the Reference tab");
            //}
            //finally
            //{
            //    CurrentPage = _repository = _userProfileManager.Logout().LoginAsHciAdminUser().NavigateToRepository();
            //}
        }

        [Test] //US69674 + CAR-3146[CAR-3060]
        public void Validation_of_Repository_Search_Filters_and_Result_Section()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            #region CAR-3146[CAR-3060]
            var primaryProcCode = testData["PrimaryProcCode"];
            var invalidPrimaryProcCode = testData["InvalidPrimaryProcCode"];
            var alphanumericCharacter = testData["AlphanumericCharacter"];
            var specialCharacter = testData["SpecialCharacter"];
            var expectedDataForPrimaryProcCode = _repository.GetExpectedDataForPrimaryProcCodeFromDb(primaryProcCode);
            #endregion

            StringFormatter.PrintMessage("Verifying default state of page");
            _repository.IsNoDataAvailablePresentInitially().ShouldBeTrue("Search should not be executed initially");
            _repository.IsFindReferencePanelPresent().ShouldBeTrue("Find Reference panel should be visible");
            _repository.IsFindReferencePanelControllable()
                .ShouldBeTrue("Find Reference panel should be controllable via icon at the header");
            _repository.GetSideBarPanelSearch().GetAvailableDropDownList("Reference Table").Contains("Add-On Code")
                .ShouldBeTrue("Reference Table Drop Down should contain Add-On_Code");
            _repository.GetSideBarPanelSearch().GetInputValueByLabelPlaceholder("Reference Table").
                ShouldBeEqual("Please Select", "Default value for Reference Table should be Please Select");
            _repository.GetSideBarPanelSearch().IsSearchInputFieldDisplayedByLabel("Proc Code Range").ShouldBeFalse("Proc code filter options should not be shown untill add on code is not selected in reference table option");
            
            StringFormatter.PrintMessage("Verification of options after selecting Add-On Code");
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");
            _repository.GetSideBarPanelSearch().IsSearchInputFieldDisplayedByLabel("Proc Code Range").ShouldBeTrue("Proc code filter options should  be shown when add on code is selected in reference table option");
            _repository.GetSideBarPanelSearch().IsSearchInputFieldDisplayedByLabel("Primary Proc Code").ShouldBeTrue("Primary Proc code filter option should be shown when add on code is selected in reference table option");
            
            #region CAR-3146[CAR-3060]
            StringFormatter.PrintMessage("Verifying Primary Proc Code accepts only alphanumeric characters");
            _repository.GetSideBarPanelSearch().SetInputFieldByLabel("Primary Proc Code", alphanumericCharacter);
            _repository.GetSideBarPanelSearch().GetLengthOfTheInputFieldByLabel("Primary Proc Code")
                .ShouldBeEqual(5,
                    "Field should allow only 5 alpha numeric characters");
            _repository.GetSideBarPanelSearch().ClickOnClearLink();
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");
            _repository.GetSideBarPanelSearch().SetInputFieldByLabel("Primary Proc Code", specialCharacter);
            _repository.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Error popup displayed?");
            _repository.GetPageErrorMessage().ShouldBeEqual("Only alphanumerics allowed.");
            _repository.ClosePageError();

            #endregion

            StringFormatter.PrintMessage("Verifying No Data Found Messages for invalid proc code range");
            _repository.FillSideBarOptionByLabel("Proc Code Range", "12346", true);
            _repository.FillSideBarOptionByLabel("Proc Code Range", "12345", true, 1, true);
            _repository.WaitForWorkingAjaxMessage();
            _repository.GetSideBarPanelSearch().IsNoMatchingRecordFoundMessagePresent()
                .ShouldBeTrue("No matching records were found message must be displayed when there are no results");

            #region CAR-3146[CAR-3060]
            StringFormatter.PrintMessage("Verifying No Data Found Messages for invalid primary proc code");
            _repository.GetSideBarPanelSearch().ClickOnClearLink();
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");
            _repository.GetSideBarPanelSearch().SetInputFieldByLabel("Primary Proc Code", invalidPrimaryProcCode);
            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.WaitForWorkingAjaxMessage();
            _repository.GetSideBarPanelSearch().IsNoMatchingRecordFoundMessagePresent()
                .ShouldBeTrue("No matching records were found message must be displayed when there are no results");
            #endregion

            StringFormatter.PrintMessage("Verifying Validation popup for missing Proc code Range and Primary Proc Code");
            _repository.GetSideBarPanelSearch().ClickOnClearLink();
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");
            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Error popup displayed?");
            _repository.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated without any criteria entered.");
            _repository.ClosePageError();

            #region CAR-3146[CAR-3060]
            StringFormatter.PrintMessage("Verify Search Result for Primary Proc Code");
            _repository.GetSideBarPanelSearch().SetInputFieldByLabel("Primary Proc Code", primaryProcCode);
            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.IsPageErrorPopupModalPresent().ShouldBeFalse("Search should be initiated with primary proc code value");
            _repository.GetGridViewSection().GetGridRowCount().ShouldBeEqual(expectedDataForPrimaryProcCode.Count,"Search Result Should Match to that of the database");
            for (int i= 0; i< expectedDataForPrimaryProcCode.Count;i++)
            {
                _repository.GetGridViewSection().GetValueInGridByColRow(1, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][2], "First Column should display dataset detail.");
                _repository.GetGridViewSection().GetValueInGridByColRow(row: i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][0], "Second Column should have the code detail.");
                _repository.GetGridViewSection().GetValueInGridByColRow(3, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][1], "Third Column should have the primary code value");
                _repository.GetGridViewSection().GetValueInGridByColRow(4, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][10], "Fourth column should be showing the details of state.");
                _repository.GetGridViewSection().GetValueInGridByColRow(5, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][7], "Fifth column should be showing the details of CMS.");
                _repository.GetGridViewSection().GetValueInGridByColRow(6, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][8], "Sixth column should have the CPT details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(7, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][9], "Seventh column should be showing the details of HCI.");
                _repository.GetGridViewSection().GetValueInGridByColRow(8, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][6], "Eighth column should have the Src details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(9, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][4], "Ninth column should have the details of review.");
                _repository.GetGridViewSection().GetValueInGridByColRow(10, i + 1).ShouldBeEqual(expectedDataForPrimaryProcCode[i][3], "Tenth column should have the apply details.");
            }
           
            #endregion
        }

        /// <summary>
        /// Awaiting resolve of validation 
        /// </summary>
        [Test, Category("Working")] //US69674
        public void Validation_of_Repository_Search_Filters_for_proc_code()
        {
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");

            StringFormatter.PrintMessage("Verifying if Proc Code Range accepts only alphanumeric characters");
            _repository.FillSideBarOptionByLabel("Proc Code Range","12$32", true);
            _repository.FillSideBarOptionByLabel("Proc Code Range", "12#92", true,1,true);

             _repository.GetPageErrorMessage()
              .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                    "Only Alpha-Numeric Allowed");

             StringFormatter.PrintMessage("Verifying only proc code from field is required");
             _repository.GetSideBarPanelSearch().ClickOnClearLink();
             _repository.FillSideBarOptionByLabel("Proc Code Range", "99210", true, 0, true);
            _repository.IsPageErrorPopupModalPresent().ShouldBeFalse("User should be able to execute search with only specifying value for from field of proc code.");
            _repository.GetGridViewSection().GetGridRowCount().ShouldBeGreater(0, "Search should be executed with proc code from value only");

            StringFormatter.PrintMessage("Verifying only giving proc code to field doesnot execute search");
            _repository.GetSideBarPanelSearch().ClickOnClearLink();
            _repository.FillSideBarOptionByLabel("Proc Code Range", "99231", true, 1, true);
            _repository.IsPageErrorPopupModalPresent().ShouldBeTrue("User should not be able to execute search with only specifying value for to field of proc code.");
            _repository.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated without any criteria entered.");
            _repository.ClosePageError();
            _repository.GetSideBarPanelSearch()
                .GetFieldErrorIconTooltipMessage("Proc Code Range")
                .ShouldBeEqual("Begin proc code is required.",
                    "Expected tool tip for field error message should be present");
            _repository.GetGridViewSection().GetGridRowCount().ShouldBeEqual(0, "Search should not be executed with proc code to value only");


        }


        [Test] //US69675 + CAR-927 + CAR-3146[CAR-3060]
        public void Verify_search_result_data_points_for_addon_code_reference_table()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var procCodeFrom = testData["ProcCodeFrom"];
            var procCodeTo = testData["ProcCodeTo"];
            var expectedDataSet = testData["ExpectedDataSet"].Split(';').ToList();

            var expectedData = _repository.GetAddonSearchList(procCodeFrom, procCodeTo);
            var searchResultCode = expectedData[0][0];
            var searchResultPrimaryCodeValue = expectedData[0][1];
            var searchResultClientCode = expectedData[0][2];
            var searchResultCpt = expectedData[0][8];
            var searchResultSrc = expectedData[0][6];

            var searchResultState = expectedData[0][10];
            var searchResultCms = expectedData[0][7];

            var searchResultHci = expectedData[0][9];
            var searchResultApply = expectedData[0][3];
            var searchResultReview = expectedData[0][4];
            var searchResultModDate = expectedData[0][13];
            var modDate = DateTime.Parse(searchResultModDate).Date.ToString("MM/dd/yyyy");
            var searchResultModUser = expectedData[0][12];
            var searchResultNote = expectedData[0][14];
            var searchResultRuleSeq = expectedData[0][15];

            _repository.RefreshPage();

            StringFormatter.PrintMessage("Selecting Add-On Code option");            
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "Add-On Code");

            #region CAR-3146(CAR-3060)
            StringFormatter.PrintMessage("Verification of search result with Proc Code range");
            SearchAddOn(procCodeFrom, procCodeTo);
            #endregion

            StringFormatter.PrintMessageTitle("Verifying the search grid columns and labels");
            _repository.GetGridViewSection().GetValueInGridByColRow(1).ShouldBeEqual(searchResultClientCode, "First Column should have the client details.");
            _repository.GetGridViewSection().GetValueInGridByColRow().ShouldBeEqual(searchResultCode, "Second Column should have the code details.");
            _repository.GetGridViewSection().GetValueInGridByColRow(3).ShouldBeEqual(searchResultPrimaryCodeValue, "Third Column should have the primary code value");
            _repository.GetGridViewSection().GetValueInGridByColRow(6).ShouldBeEqual(searchResultCpt, "Fifth column should have the CPT details.");
            _repository.GetGridViewSection().GetValueInGridByColRow(8).ShouldBeEqual(searchResultSrc, "Seventh column should have the Src details.");
            _repository.GetGridViewSection().GetValueInGridByColRow(9).ShouldBeEqual(searchResultReview, "Eight column should have the details of review.");
            _repository.GetGridViewSection().GetValueInGridByColRow(10).ShouldBeEqual(searchResultApply, "Ninth column should have the apply details.");
            _repository.GetGridViewSection().GetValueInGridByColRow(7).ShouldBeEqual(searchResultHci, "Sixth column should be showing the details of HCI.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(4).ShouldBeEqual("State:", "Third column should be showing the details of State.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(5).ShouldBeEqual("CMS:", "Fourth column should be showing the details of CMS.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(6).ShouldBeEqual("CPT:", "Fifth column should be showing the details of CPT.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(7).ShouldBeEqual("HCI:", "Sixth column should be showing the details of HCI.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(8).ShouldBeEqual("Src:", "Seventh column should be showing the details of Src.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(9).ShouldBeEqual("Review:", "Eight column should be showing the details of Review.");
            _repository.GetGridViewSection().GetLabelInGridByColRow(10).ShouldBeEqual("Apply:", "Ninth column should be showing the details of Apply.");
            
            var dataSetVal = _repository.GetGridViewSection().GetGridListValueByCol(1)
                 .Distinct().ToList();
            dataSetVal.Count.ShouldBeLessOrEqual(2, "Only CMS and CPT data set should be displayed");
            expectedDataSet.Sort(); dataSetVal.Sort();
            dataSetVal.CollectionShouldBeSubsetOf(expectedDataSet, "Search Result should be subset of CMS and CPT data set");

            StringFormatter.PrintMessageTitle("Verifying the values in repository details section");
            _repository.ClickOnSearchResultsRow();
            _repository.GetDetailsLabel().ShouldBeEqual("Mod Date:", "Add on Details should contain the details of Mod Date.");
            _repository.GetDetailsValue().ShouldBeEqual(modDate, "Modified Date details should be in the first row first column of details section.");
            _repository.GetDetailsLabel(1, 2).ShouldBeEqual("Mod User:", "Add on Details should contain the details of Mod User.");
            _repository.GetDetailsValue(1, 2).ShouldBeEqual(searchResultModUser, "Modified User details should be in the first row second column of details section.");
            _repository.GetDetailsLabel(1, 3).ShouldBeEqual("Rule Seq:", "Add on Details should contain the details of Rule Seq.");
            _repository.GetDetailsValue(1, 3).ShouldBeEqual(searchResultRuleSeq, "Rule Sequence details should be in the first row third column of details section.");
            _repository.GetDetailsValue(2).ShouldBeEqual(searchResultNote, "Note details should be in the second row of details section.");

            _repository.GetGridViewSection().GetValueInGridByColRow(4).ShouldBeEqual(searchResultState, "Fourth column should be showing the details of state.");
            _repository.GetGridViewSection().GetValueInGridByColRow(5).ShouldBeEqual(searchResultCms, "Fifth column should be showing the details of CMS.");
        }

        [Test] //CAR-520
        public void Validation_of_Repository_Search_Result_for_UNB()
        {
            const string dataSet = "MCRCCI_SET";
            var expectedData = _repository.GetUnbSearchList(dataSet);
            _repository.SelectUnbInReferenceTable();
            SearchRange(expectedData[1], expectedData[1]);
            const string trigLabel = "Trigger Code Range";
            SearchRange(expectedData[0], expectedData[0], trigLabel);
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Dataset", dataSet);
            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.WaitForWorkingAjaxMessage();
            
            var searchResultDataSet = expectedData[2];
            var searchResultProcCode = expectedData[1];
            var searchResultTrigCode = expectedData[0];
            var searchResultSrc = expectedData[6];
            var searchResultCciMod = expectedData[9];
            var searchResultApply = expectedData[3];
            var searchResultReview = expectedData[4];
            var searchResultEffDate = string.IsNullOrEmpty(expectedData[15]) ? "" : DateTime.Parse(expectedData[15]).ToString("MM/dd/yyyy");
            var searchResultTermDate = string.IsNullOrEmpty(expectedData[16]) ? "" : DateTime.Parse(expectedData[16]).ToString("MM/dd/yyyy");
            var searchResulModDate = string.IsNullOrEmpty(expectedData[17]) ? "" : DateTime.Parse(expectedData[17]).Date.ToString("MM/dd/yyyy");
            var searchResultModUser = expectedData[18];
            var searchResultNote = expectedData[19];
            var searchResultRuleSeq = expectedData[20];

            try
            {
                _repository.GetSearchResultHeader()
                    .ShouldBeEqual("NCCI UNB Rules", "Search Result Header should be correct");
                _repository.GetGridViewSection().GetValueInGridByColRow(1).ShouldBeEqual(searchResultDataSet,
                    "First Column should have Dataset source to which the rule applies");
                _repository.GetGridViewSection().GetValueInGridByColRow().ShouldBeEqual(searchResultProcCode,
                    "Second Column should have the proc code details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(3).ShouldBeEqual(searchResultTrigCode,
                    "Third column should be showing the details of trig code.");
                _repository.GetGridViewSection().GetValueInGridByColRow(4)
                    .ShouldBeEqual(searchResultSrc, "Fourth column should have the Src details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(5).ShouldBeEqual(searchResultCciMod,
                    "Fifth column should have the CCI Mod details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(6).ShouldBeEqual(searchResultReview,
                    "Sixth column should have the details of whether HCI review is required or not.");
                _repository.GetGridViewSection().GetValueInGridByColRow(7).ShouldBeEqual(searchResultApply,
                    "Seventh column should have the apply details.");
                _repository.GetGridViewSection().GetValueInGridByColRow(8).ShouldBeEqual(searchResultEffDate,
                    "Eigth column should be showing the details of Effective Date.");
                _repository.GetGridViewSection().GetValueInGridByColRow(9).ShouldBeEqual(searchResultTermDate,
                    "Ninth column should be showing the details of Term Date.");
                _repository.GetGridViewSection().GetLabelInGridByColRow()
                    .ShouldBeEqual("Proc:", "Second column should be showing the details of Proc.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(3)
                    .ShouldBeEqual("Trig:", "Fourth column should be showing the details of Trig.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(4)
                    .ShouldBeEqual("Src:", "Fifth column should be showing the details of source.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(5).ShouldBeEqual("CCI Mod:",
                    "Sixth column should be showing the details of CCi mod.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(6).ShouldBeEqual("Review:",
                    "Eight column should be showing the details of Review.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(7)
                    .ShouldBeEqual("Apply:", "Ninth column should be showing the details of Apply.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(8).ShouldBeEqual("Eff:",
                    "Seventh column should be showing the details of effective date.");
                _repository.GetGridViewSection().GetLabelInGridByColRow(9)
                    .ShouldBeEqual("Term:", "Seventh column should be showing the details of term.");
                _repository.ClickOnSearchResultsRow();
                _repository.GetDetailsLabel()
                    .ShouldBeEqual("Mod Date:", "Add on Details should contain the details of Mod Date.");
                _repository.GetDetailsValue().ShouldBeEqual(searchResulModDate,
                    "Modified Date details should be in the first row first column of details section.");
                _repository.GetDetailsLabel(1, 2)
                    .ShouldBeEqual("Mod User:", "Add on Details should contain the details of Mod User.");
                _repository.GetDetailsValue(1, 2).ShouldBeEqual(searchResultModUser,
                    "Modified User details should be in the first row second column of details section.");
                _repository.GetDetailsLabel(1, 3)
                    .ShouldBeEqual("Rule Seq:", "Add on Details should contain the details of Rule Seq.");
                _repository.GetDetailsValue(1, 3).ShouldBeEqual(searchResultRuleSeq,
                    "Rule Sequence details should be in the first row third column of details section.");
                _repository.GetDetailsValue(2).ShouldBeEqual(searchResultNote,
                    "Note details should be in the second row of details section.");
            }
            finally
            {
                CurrentPage.ClickOnQuickLaunch();
                CurrentPage.NavigateToRepository();
            }

            

        }
        

        [Test] //CAR-726
        public void Verification_of_Repository_Search_Filters_for_Adding_NCCIUNB_in_ReferenceTable_Filter()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var procCodeFrom = testData["ProcCodeFrom"].Split(';').ToList();
            var procCodeTo = testData["ProcCodeTo"].Split(';').ToList();
            var trigCodeFrom = testData["TrigCodeFrom"].Split(';').ToList();
            var trigCodeTo = testData["TrigCodeTo"].Split(';').ToList();
            var dataSet = DataHelper.GetMappingData(FullyQualifiedClassName, "dataset_values").Values.ToList();
            _repository.IsFindReferencePanelPresent().ShouldBeTrue("Find Reference table should be present by default.");
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "NCCI UNB Rules");
            _repository.GetSideBarValueByLabel("Proc Code Range", 1).ShouldBeEqual("", "Proc Code From should be empty.");
            _repository.GetSideBarValueByLabel("Proc Code Range", 2).ShouldBeEqual("", "Proc Code To should be empty.");
            _repository.GetSideBarValueByLabel("Trigger Code Range", 1).ShouldBeEqual("", "Trigger Code From should be empty.");
            _repository.GetSideBarValueByLabel("Trigger Code Range", 2).ShouldBeEqual("", "Trigger Code To should be empty.");
            _repository.GetSideBarPanelSearch().GetAvailableDropDownList("Dataset")
                .ShouldCollectionBeEqual(dataSet, "Dataset dropdown should consist the values.");

            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.IsPageErrorPopupModalPresent().ShouldBeTrue("User should be able to execute search with only specifying value for from field.");
            _repository.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated without any criteria entered.");
            _repository.ClosePageError();

            
            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Dataset", dataSet[1]);
            _repository.FillSideBarOptionByLabel("Proc Code Range", procCodeFrom[0], true);
            _repository.GetSideBarValueByLabel("Proc Code Range", 1).ShouldBeEqual(procCodeFrom[0].Remove(procCodeFrom[0].Length - 1, 1), "Proc Code From should take an alphanumeric input of 5 characters max.");
            _repository.GetSideBarValueByLabel("Proc Code Range", 2).ShouldBeEqual(procCodeFrom[0].Remove(procCodeFrom[0].Length - 1, 1), "Proc Code To should have the default value as Proc Code From.");
            _repository.GetSideBarPanelSearch().ClickOnFindButton();
            _repository.IsPageErrorPopupModalPresent().ShouldBeTrue("User should be able to execute search with only specifying value for from field.");
            _repository.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated without any criteria entered.");
            _repository.ClosePageError();
            _repository.FillSideBarOptionByLabel("Proc Code Range", procCodeTo[0], true, 1, false);
            _repository.GetSideBarValueByLabel("Proc Code Range", 2).ShouldBeEqual(procCodeTo[0].Remove(procCodeTo[0].Length - 1, 1), "Proc Code To should take an alphanumeric input of 5 characters max.");
            _repository.FillSideBarOptionByLabel("Trigger Code Range", trigCodeFrom[0], true);
            _repository.GetSideBarValueByLabel("Trigger Code Range", 1).ShouldBeEqual(trigCodeFrom[0].Remove(trigCodeFrom[0].Length - 1, 1), "Trigger Code From should take an alphanumeric input of 5 characters max.");
            _repository.GetSideBarValueByLabel("Trigger Code Range", 2).ShouldBeEqual(trigCodeFrom[0].Remove(trigCodeFrom[0].Length - 1, 1), "Trigger Code To should have the default value as Proc Code From.");
            _repository.FillSideBarOptionByLabel("Trigger Code Range", trigCodeTo[0], true, 1, false);
            _repository.GetSideBarValueByLabel("Trigger Code Range", 2).ShouldBeEqual(trigCodeTo[0].Remove(trigCodeTo[0].Length - 1, 1), "Trigger Code To should take an alphanumeric input of 5 characters max.");
            _repository.GetSideBarPanelSearch().ClickOnClearLink();
            _repository.IsFindReferencePanelPresent().ShouldBeTrue("Find Reference table should be present by default.");

            _repository.GetSideBarPanelSearch().SelectDropDownListValueByLabel("Reference Table", "NCCI UNB Rules");
            SearchNCCIUNBRules(procCodeFrom[2], procCodeTo[2], trigCodeFrom[2], trigCodeTo[2]);
            _repository.GetSideBarPanelSearch().IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue("No matching records were found message must be displayed when there are no results");
            SearchNCCIUNBRules(procCodeFrom[1], procCodeTo[1], trigCodeFrom[1], trigCodeTo[1]);
            var dataSetValue = _repository.GetGridViewSection().GetGridListValueByCol(1);
            foreach (string set in dataSetValue)
            {
                (set == "MCRCCI_SET" || set == "MCDCCI_SET").ShouldBeTrue("Only MCRCCI_SET and MCDCCI_SET rows should be shown in the results");
            }
        }
        #endregion

        private void SearchNCCIUNBRules(string procCodeFrom, string procCodeTo, string trigCodeFrom, string trigCodeTo)
        {
            _repository.FillSideBarOptionByLabel("Trigger Code Range", trigCodeFrom, true);
            _repository.FillSideBarOptionByLabel("Trigger Code Range", trigCodeTo, true, 1, false);
            SearchAddOn(procCodeFrom, procCodeTo);
        }



        private void SearchAddOn(string procCodeFrom, string procCodeTo, string label = "Proc Code Range")
        {
            _repository.FillSideBarOptionByLabel(label, procCodeFrom, true);
            _repository.FillSideBarOptionByLabel(label, procCodeTo, true, 1,true);
            _repository.WaitForWorkingAjaxMessage();
            _repository.WaitForStaticTime(1000);
        }

        private void SearchRange(string procCodeFrom, string procCodeTo, string label = "Proc Code Range")
        {
            _repository.FillSideBarOptionByLabel(label, procCodeFrom, true);
            _repository.FillSideBarOptionByLabel(label, procCodeTo, true, 1);
        }
    }
}
