using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class PreAuthSearch: NewAutomatedBase
    {
        #region PRIVATE PROPERTIES

        private PreAuthSearchPage _preAuthSearch;
        private CommonValidations _commonValidation;
        private QuickLaunchPage _quickLaunch;
        private readonly string _dciPreAuthorizationPrivilege = RoleEnum.DCIAnalyst.GetStringValue();
        private PreAuthActionPage _preAuthAction;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
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
                CurrentPage = _preAuthSearch = QuickLaunch.NavigateToPreAuthSearch();
                _commonValidation = new CommonValidations(CurrentPage);

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
            CurrentPage = _preAuthSearch;
        }

        protected override void ClassCleanUp()
        {
            try
            {
                _preAuthSearch.CloseDbConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _preAuthSearch = _preAuthSearch.Logout().LoginAsHciAdminUser().NavigateToPreAuthSearch();
            }

            if (CurrentPage.GetPageHeader() != PageHeaderEnum.PreAuthSearch.GetStringValue())
            {
                CurrentPage.NavigateToPreAuthSearch();

            }

            _preAuthSearch.ClickOnSidebarIcon();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test] //CAR-1390(CAR-265) + TE-1185
        public void Validate_and_verify_pre_auth_panel()
        {
            var quickSearchList =
                DataHelper.GetMappingData(FullyQualifiedClassName, "Quick_Search_List").Values.ToList();
            var statusList =
                DataHelper.GetMappingData(FullyQualifiedClassName, "status_for_all_pre_auths").Values.ToList();
            var filterOptionList =
                DataHelper.GetMappingData(FullyQualifiedClassName, "input_filter_for_all_pre_auths").Values.ToList();
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var firstName = paramLists["FirstName"];
            var lastName = paramLists["LastName"];
            var preAuthSequence = paramLists["PreAuthSequence"];

            //_preAuthSearch.ClickOnQuickLaunch().NavigateToPreAuthSearch();

            _preAuthSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                .ShouldCollectionBeEqual(quickSearchList, "Verify Quick Search Dropdown list");

            _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue(), "Verify Default Option of Quick Search");

            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
            StringFormatter.PrintMessageTitle("Verify Label for All Pre-Auths");
            _preAuthSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(filterOptionList, "Search Filters", true);
           
           

            StringFormatter.PrintMessageTitle("Verify Search cannot be initiated only by firstname or lastname");
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", firstName);
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.GetPageErrorMessage()
                .ShouldBeEqual("First and last name are required when searching by name.",
                    "Verify Search can not be initiated with First Name Only");
            _preAuthSearch.ClosePageError();
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", "");
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name", lastName);
            _preAuthSearch.WaitForStaticTime(500);
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.GetPageErrorMessage()
                .ShouldBeEqual("First and last name are required when searching by name.",
                    "Verify Search can not be initiated with Last Name Only");
            _preAuthSearch.ClosePageError();
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", firstName);
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.WaitForWorkingAjaxMessage();
            _preAuthSearch.GetGridViewSection.GetGridRowCount()
                .ShouldBeGreater(0, "Verify Search Result");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();

            StringFormatter.PrintMessageTitle("Validation of Input Field");
            
            ValidateNumericOnlyField("Auth Sequence", "Only numbers allowed.", "0!2a", paramLists["NumberOnly"]);
            ValidateDatePickerField("Create Date", "Please enter date range that spans less than 6 months.",  "Please enter a valid date range.");
            ValidateAlphaNumericField("Pre-Auth ID", paramLists["AlphanumericCharacter"]);
            ValidateNumericOnlyField("Patient Sequence", "Only numbers allowed.", "0!2a", paramLists["NumberOnly"]);
            ValidateAlphaNumericField("Member ID", paramLists["AlphanumericCharacter"]);
            ValidateNumericOnlyField("Provider Sequence", "Only numbers allowed.", "0!2a", paramLists["NumberOnly"]);
            ValidateAlphaNumericField("Provider No", paramLists["AlphanumericCharacter"]);
            ValidateNumericOnlyField("TIN", "Only numbers allowed.", "0!2a", paramLists["NumberOnly"]);

            StringFormatter.PrintMessageTitle("Verify Status dropdown and no result found");
            var actualStatusList = _preAuthSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Status");
            actualStatusList.RemoveAt(0);
            actualStatusList.ShouldCollectionBeEqual(statusList, "Verify Status Dropdown list");

            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", "1");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.WaitForWorkingAjaxMessage();
            _preAuthSearch.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                .ShouldBeEqual("No matching records were found.", "Verify No matching record");

            StringFormatter.PrintMessageTitle("Verify Clear Link and Empty Search validation");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.GetPageErrorMessage()
                .ShouldBeEqual("Search cannot be initiated without any criteria entered.",
                    "Verify Search can not be initiated without criteria");
            _preAuthSearch.ClosePageError();

            StringFormatter.PrintMessageTitle("Verify Input field when clicked on clear link");
            foreach (var filterInputLabel in filterOptionList)
            {
                if (filterInputLabel == "Quick Search")
                    continue;
                _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel(filterInputLabel)
                    .ShouldBeNullorEmpty(String.Format("Is {0} Input Field Empty?", filterInputLabel));
            }

            StringFormatter.PrintMessageTitle("Verify Searched Result with out error");
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", preAuthSequence);
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.WaitForWorkingAjaxMessage();
            _preAuthSearch.GetGridViewSection.GetValueInGridByColRow()
                .ShouldBeEqual(preAuthSequence, "Verify Search Result");

            StringFormatter.PrintMessageTitle("Verify Search can not be executed only status=CLOSED ");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                StatusEnum.Closed.GetStringDisplayValue());
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.GetPageErrorMessage()
                .ShouldBeEqual(
                    "Search cannot be initiated with Closed status only. Additional search criteria is required.",
                    "Verify Search can not be initiated with Closed Status");
            _preAuthSearch.ClosePageError();
            _preAuthSearch.GetInvalidInputToolTipByLabel("Status").ShouldBeEqual("Search cannot be initiated with Closed status only. Additional search criteria is required.",
                "Verify tooltip of exclamation icon when searched initiated with Closed Status");

            StringFormatter.PrintMessageTitle("Verify only one value can be selected in 'Status' dropdown");
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                StatusEnum.CotivitiUnreviewed.GetStringDisplayValue());
            _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel("Status")
                .ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue());


        }

        [Test] //CAR-200
        public void Validate_Security_And_Navigation_Of_PreAuthorization_Search_Page()
        {
            _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.PreAuthorization, new List<string> { SubMenu.PreAuthSearch },
                _dciPreAuthorizationPrivilege, new List<string> { PageHeaderEnum.PreAuthSearch.GetStringValue()}, Login.LoginAsUserHavingNoAnyAuthority, new[] { "Test4", "Automation4" });
        }

        [Test]//CAR-266 (CAR-1389)
        public void Verify_pre_auth_Search_list_sorting_for_internal_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var prvSeq = paramLists["PrvSeq"];
            var patSeq = paramLists["PatSeq"];
            var filterOptions =
                DataHelper.GetMappingData(FullyQualifiedClassName, "preauth_sorting_option_list").Values.ToList();

            try
            {
                _preAuthSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                _preAuthSearch.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence", prvSeq);
                _preAuthSearch.ClickOnFindButton();
                _preAuthSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search Result must be listed.");
                var expectedDescendingSortedAuthSequence =
                    _preAuthSearch.GetAuthSequenceByPrvSeqInDescendingOrder(prvSeq);
                var expectedAscendingSortedAuthSequence =
                    _preAuthSearch.GetAuthSequenceByPrvSeqInAscendingOrder(prvSeq);
                _preAuthSearch.GetAuthSeqListOnSearchResult().ShouldCollectionBeEqual(
                    expectedDescendingSortedAuthSequence,
                    "Default sorting of pre auth search result should be by Auth sequence in descending order.");

                StringFormatter.PrintMessageTitle("Validation of Sorting by Auth Sequence");
                _preAuthSearch.ClickOnFilterOptionListRow(1);
                _preAuthSearch.GetAuthSeqListOnSearchResult().ShouldCollectionBeEqual(
                    expectedAscendingSortedAuthSequence,
                    "Auth Sequence should be sorted in Ascending Order");
                _preAuthSearch.ClickOnFilterOptionListRow(1);
                _preAuthSearch.GetAuthSeqListOnSearchResult().ShouldCollectionBeEqual(
                    expectedDescendingSortedAuthSequence,
                    "Auth Sequence should be sorted in Descending Order");


                _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
                _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _preAuthSearch.WaitForWorkingAjaxMessage();

                //ValidatePreAuthSearchRowSorted(3, 2, "Review Type"); no data
                ValidatePreAuthSearchRowSorted(8, 4, "Status");
                ValidatePreAuthSearchRowSorted(6, 6, "Received Date");


                ValidatePreAuthSearchRowSorted(5, 3, "Provider Seq");

                _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Patient Sequence", patSeq);
                _preAuthSearch.ClickOnFindButton();

                ValidatePreAuthSearchRowSorted(7, 5, "State");

                _preAuthSearch.ClickOnFilterOptionListRow(7);
                StringFormatter.PrintMessage("Clicked on Clear Sort.");
                _preAuthSearch.IsListIntSortedInDescendingOrder(2).ShouldBeTrue(
                    "All sorting is cleared and search list is sorted by Auth Sequence which is default sort.");
            }
          
            finally
            {
                _preAuthSearch.ClickOnFilterOptionListRow(7);
                StringFormatter.PrintMessage("Clicked on Clear Sort.");
            }
        }


        [Test] //CAR-1391 (CAR-264) + TE-1185
        public void Verify_pre_auth_search_and_worklist_for_internal_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var date = paramLists["Createdate"].Split(',').ToList();
            var preAuthDetails = _preAuthSearch.GetPreAuthSearchValues(paramLists["PreAuthId"], date[0],date[1]);
            try
            {
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                     paramLists["QuickSearch"]);
                _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Pre-Auth ID", paramLists["PreAuthId"]);
                _preAuthSearch.GetSideBarPanelSearch.SetDateField("Create Date", date[0],1);
                _preAuthSearch.GetSideBarPanelSearch.SetDateField("Create Date", date[1], 2);
                _preAuthSearch.ClickOnFindButton();
                _preAuthSearch.GetGridViewSection.GetGridListValueByCol().IsInDescendingOrder();
                var rowCount = _preAuthSearch.GetGridViewSection.GetGridRowCount();
                if (rowCount == 25)
                {
                    _preAuthSearch.GetGridViewSection.IsLoadMorePresent().ShouldBeTrue("Load More should be present when the search result exceeds 25.");
                    _preAuthSearch.GetGridViewSection.ClickLoadMore();
                    _preAuthSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual(25,
                        "All the search results should be shown and should be equal or greater than 25.");
                }

                while(_preAuthSearch.GetGridViewSection.IsLoadMorePresent())
                {
                    _preAuthSearch.GetGridViewSection.ClickLoadMore();
                }
                _preAuthSearch.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEquivalent(preAuthDetails.Select(x => x[0]).ToList(),"Preauth sequence value correct in result grid?");
                _preAuthSearch.GetGridViewSection.IsLabelPresentByCol().ShouldBeFalse("Auth Sequence Label should not be present.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow().ShouldBeEqual(preAuthDetails[0][0], string.Format("Auth Sequence value should be equal to '{0}'.", preAuthDetails[0][0]));
                _preAuthSearch.GetGridViewSection.IsLabelPresentByCol(3).ShouldBeFalse("Review Type Label should not be present.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(3, 1).ShouldBeEqual(preAuthDetails[0][1], string.Format("Review type value should be '{0}.'", preAuthDetails[0][1]));
                _preAuthSearch.GetGridViewSection.GetLabelInGridByColRow(4, 1).ShouldBeEqual("Pat Seq:", "Label should be Patient Sequence.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(4, 1).ShouldBeEqual(preAuthDetails[0][2], string.Format("Patient Sequence value should be equal to '{0}'.", preAuthDetails[0][2]));
                _preAuthSearch.GetGridViewSection.GetLabelInGridByColRow(5, 1).ShouldBeEqual("Prv Seq:", "Label should be Provider Sequence.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(5, 1).ShouldBeEqual(preAuthDetails[0][3], string.Format("Provider Sequence value should be equal to '{0}'.", preAuthDetails[0][3]));
                _preAuthSearch.GetGridViewSection.IsLabelPresentByCol(6).ShouldBeFalse("Received Date Label should not be present.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(6, 1).ShouldBeEqual(Convert.ToDateTime(preAuthDetails[0][4]).ToString("MM/dd/yyyy"), string.Format("Received Date should be equal to {0}.", preAuthDetails[0][4]));
                _preAuthSearch.GetGridViewSection.IsLabelPresentByCol(7).ShouldBeFalse("State Label should not be present.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(7, 1).Length.ShouldBeEqual(2, "State value should be equal to 2 characters.");
                _preAuthSearch.GetGridViewSection.IsLabelPresentByCol(8).ShouldBeFalse("Status Label should not be present.");
                _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8, 1).ShouldBeEqual(preAuthDetails[0][5], string.Format("Status should be equal to '{0}'.", preAuthDetails[0][5]));
                _preAuthSearch.GetGridViewSection.ClickOnGridRowByRow();
                _preAuthSearch.GetPreAuthDetailsLabel("Pre-Auth ID")
                    .ShouldBeEqual("Pre-Auth ID", "Pre-Auth ID label should be present in the Pre-Auth Details section.");
                _preAuthSearch.GetPreAuthDetailsLabel("Patient Name")
                    .ShouldBeEqual("Patient Name", "Patient Name label should be present in the Pre-Auth Details section.");
                _preAuthSearch.GetPreAuthDetailsLabel("Provider Name")
                    .ShouldBeEqual("Provider Name", "Provider Name label should be present in the Pre-Auth Details section.");
                _preAuthSearch.GetPreAuthDetailsValue("Pre-Auth ID").ShouldBeEqual(preAuthDetails[0][6], "Pre-Auth ID value should be equal to 1.");
                _preAuthSearch.GetPreAuthDetailsValue("Patient Name").ShouldBeEqual(preAuthDetails[0][7], "Patient Name value should be equal to RRR, QQQ.");
                _preAuthSearch.GetPreAuthDetailsValue("Provider Name").ShouldBeEqual(preAuthDetails[0][8], "Provider Name value should be equal to Dr. Seuss.");
            }
            finally
            {
                _preAuthSearch.ClickOnFilterOptionListRow(7);
                StringFormatter.PrintMessage("Clicked on Clear Sort.");
            }
        }

        [Test] //CAR-1433 (CAR-1003)
        public void Verify_Find_Pre_Auths_Panel_And_Other_Quick_Search_Options()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            var filterInputFields = DataHelper.GetMappingData(FullyQualifiedClassName, "input_filter_for_all_pre_auths").Values.ToList();
            var expectedStatusListWhenOutstandingPreAuthIsSelected = new List<string>
            {
                StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(),
                StatusEnum.StateConsultantComplete.GetStringDisplayValue(),
                StatusEnum.CotivitiConsultantComplete.GetStringDisplayValue(),
                StatusEnum.DocumentReview.GetStringDisplayValue()
            } ;

            var expectedStatusListWhenConsultantReviewIsSelected = new List<string>
            {
                StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue(),
                StatusEnum.StateConsultantRequired.GetStringDisplayValue()
            } ;

            StringFormatter.PrintMessageTitle("Verifying the 'Outstanding Pre-Auths' Quick Search option");
            _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
            _preAuthSearch.ClickOnFindButton();

            StringFormatter.PrintMessage("Verifying the status options when 'Outstanding Pre-Auths' Quick Search option is selected");
            var statusListWhenOutstandingPreAuthIsSelected = _preAuthSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Status");
            statusListWhenOutstandingPreAuthIsSelected = statusListWhenOutstandingPreAuthIsSelected.Where(s => !string.IsNullOrEmpty(s)).ToList<string>();
            statusListWhenOutstandingPreAuthIsSelected.ShouldCollectionBeEqual(expectedStatusListWhenOutstandingPreAuthIsSelected,
                "Status List is as expected when 'Outstanding Pre-Auths' is selected in the Quick Search dropdown");

            filterInputFields.Remove("Quick Search");
            filterInputFields.Remove("Status");

            StringFormatter.PrintMessage("Verifying whether correct input fields are being shown for 'Outstanding Pre-Auths' Quick Search");
            foreach (var filter in filterInputFields)
            {
                _preAuthSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(filter)
                    .ShouldBeFalse(string.Format("Is Filter option '{0}' displayed? ", filter));
            }

            StringFormatter.PrintMessage("Verifying the correct count of results is being shown for 'Outstanding Pre-Auths' Quick Search");
            
            while (_preAuthSearch.GetGridViewSection.IsLoadMorePresent())
                _preAuthSearch.GetGridViewSection.ClickLoadMore();

            var countOfResultInGrid = _preAuthSearch.GetGridViewSection.GetGridRowCount();
            _preAuthSearch.GetCountOfOutstandingPreAuthsForInternalUser().ShouldBeEqual(countOfResultInGrid);

            StringFormatter.PrintMessageTitle("Verifying the 'Consultant Review' Quick Search option");
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", PreAuthQuickSearchEnum.ConsultantReview.GetStringValue());
            _preAuthSearch.ClickOnFindButton();

            StringFormatter.PrintMessage("Verifying the status options when 'Consultant Review' Quick Search option is selected");
            var statusListWhenConsultantReviewIsSelected = _preAuthSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Status");
            statusListWhenConsultantReviewIsSelected = statusListWhenConsultantReviewIsSelected.Where(s => !string.IsNullOrEmpty(s)).ToList<string>();
            statusListWhenConsultantReviewIsSelected.ShouldCollectionBeEqual(expectedStatusListWhenConsultantReviewIsSelected,
                "Status List is as expected when 'Consultant Review' is selected in the Quick Search dropdown");

            StringFormatter.PrintMessage("Verifying whether correct input fields are being shown for 'Consultant Review' Quick Search");
            foreach (var filter in filterInputFields)
            {
                _preAuthSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(filter)
                    .ShouldBeFalse(string.Format("Is Filter option '{0}' displayed? ", filter));
            }

            StringFormatter.PrintMessage("Verifying the correct count of results is being shown for 'Consultant Review' Quick Search");
            countOfResultInGrid = _preAuthSearch.GetGridViewSection.GetGridRowCount();
            while(_preAuthSearch.GetGridViewSection.IsLoadMorePresent())
                _preAuthSearch.GetGridViewSection.ClickLoadMore();
            _preAuthSearch.GetCountOfConsultantReviewPreAuthsForInternalUser().ShouldBeEqual(countOfResultInGrid);

        }

        [Test] //TE-494
        public void Verify_Presence_Of_Scroll_Bar_When_Navigating_Back_From_Preauth_Action_Page()
        {
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
            _preAuthSearch.ClickOnFindButton();

            if (_preAuthSearch.GetGridViewSection.IsVerticalScrollBarPresentInGridSection())
            {
                _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(1, 2);
                _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthSearch.WaitForStaticTime(500);
                _preAuthSearch.GetGridViewSection.IsVerticalScrollBarPresentInGridSection()
                    .ShouldBeTrue(
                        "Vertical Scroll Bar should be present when navigating back from pre auth action page");
            }

        }

        #endregion

        #region PRIVATE METHODS

        private void ValidatePreAuthSearchRowSorted(int col, int sortOptionRow, string colName)
        {
            _preAuthSearch.ClickOnFilterOptionListRow(sortOptionRow);
           switch (colName)
            {
                case "Auth Seq":
                    _preAuthSearch.IsListIntSortedInAscendingOrder(col)
                   .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _preAuthSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _preAuthSearch.IsListIntSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;

                case "Provider Seq":
                    _preAuthSearch.IsListIntSortedInAscendingOrder(col)
                   .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _preAuthSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _preAuthSearch.IsListIntSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;

                case "Received Date": 
                    _preAuthSearch.IsListDateSortedInAscendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _preAuthSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _preAuthSearch.IsListDateSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;

                default:
                    _preAuthSearch.IsListStringSortedInAscendingOrder(col)
                   .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _preAuthSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _preAuthSearch.IsListStringSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;


            }
        }


        private void ValidateNumericOnlyField(string label, string message, string illegalCharacter, string value = null)
        {
            _preAuthSearch.SetInputFieldByInputLabel(label, illegalCharacter);

            _preAuthSearch.GetPageErrorMessage()
                .ShouldBeEqual(message,
                    "Popup Error message is shown when unexpected " + illegalCharacter + "is passed to " + label);
            _preAuthSearch.ClosePageError();

            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, value);
            if (label == "TIN")
                _preAuthSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel(label)
                    .ShouldBeEqual(9, string.Format("{0} Field should allow only 9 digits", label));
            else
                _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel(label)
                    .ShouldBeEqual(value,
                        string.Format("Verify {0} accept numeric values only", label));
        }
        private void ValidateAlphaNumericField(string label, string value)
        {
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, value);
            _preAuthSearch.GetSideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(value, "Verify Pre-Auth ID accept alphanumeric values");

        }

        public void ValidateDatePickerField( string label, string message1,  string message2)
        {
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();

            _preAuthSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _preAuthSearch.GetSideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message2,
                    "Field Error Tooltip Message When Date From is empty");
            _preAuthSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue($"{label} should be highlighted red for invalid input");
            _preAuthSearch.GetCountOfInvalidRed().ShouldBeEqual(2,
                $"Both to and from fields of {label} should be highlighted red for invalid input");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
          
            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, DateTime.Now.AddDays(2).ToString("MM/d/yyyy"), sendTabKey: true); //check numeric value can be typed
            _preAuthSearch.GetSideBarPanelSearch.GetDateFieldFrom(label).ShouldBeEqual(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"), label + " Checks numeric value is accepted");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.SetDateField(label, DateTime.Now.ToString("MM/d/yyyy"),1);
            _preAuthSearch.GetSideBarPanelSearch.GetDateFieldTo(label).
                ShouldBeEqual(_preAuthSearch.GetSideBarPanelSearch.GetDateFieldFrom(label), label + " From value populated in To field as well.");

            _preAuthSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.Subtract(new TimeSpan(24, 0, 0)).ToString("MM/d/yyyy"));
            _preAuthSearch.GetPageErrorMessage().ShouldBeEqual("Please enter a valid date range.");
            _preAuthSearch.ClosePageError();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnHeader();


            _preAuthSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddMonths(6).AddDays(3).ToString("MM/d/yyyy"));
            _preAuthSearch.GetSideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message1,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 6 months", label));
            _preAuthSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue($"{label} should be highlighted red for invalid input");
            _preAuthSearch.GetCountOfInvalidRed().ShouldBeEqual(2,
                $"Both to and from fields of {label} should be highlighted red for invalid input");
            _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _preAuthSearch.GetPageErrorMessage().ShouldBeEqual(message1, "Verification of popup message for invalid date");
            _preAuthSearch.ClosePageError();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnHeader();

        }


        #endregion
    }
}
