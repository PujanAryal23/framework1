using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.QuickLaunch;
using UIAutomation.Framework.Core.Driver;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimSearchClient
    {
        #region Old Sequential Code Commented Out
        /*#region PRIVATE FIELDS

        private ClaimSearchPage _claimSearch;       

        private List<string> _smtstPlanList;
        private List<string> _smtstSubStatusList;
        private List<string> _claimsAssignedToUserList;
        private List<string> _lineOfBusinessList;
        private List<string> _smtstReviewGroup;
        private List<string> _activeProductListForClient;
        private List<string> _allProductList;
        private Int16 _unreviewedClaimsCountInternalUser;
        private Int16 _unreviewedClaimsCountClientUser;
        private Int16 _pendedClaimsCount;
        private Int16 _flaggedClaimsCountInternalUser;
        private ClaimActionPage _claimAction;

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

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                automatedBase.CurrentPage = _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();

                try
                {
                    RetrieveListFromDatabase();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }


        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if(_claimSearch.IsPageErrorPopupModalPresent())
                _claimSearch.ClosePageError();

            if ( Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _claimSearch = automatedBase.CurrentPage
                    .Logout()
                    .LoginAsClientUser().ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient).NavigateToClaimSearch();
            }
            else
            {
                if (!automatedBase.CurrentPage.IsDefaultTestClientForEmberPage(automatedBase.EnvironmentManager.TestClient))
                {
                    automatedBase.CurrentPage.SwitchClientTo(automatedBase.EnvironmentManager.TestClient)
                        .NavigateToClaimSearch();
                }

                if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.ClaimSearch.GetStringValue())
                {
                    Console.Out.WriteLine("Back to automatedBase.QuickLaunch page");
                    _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                    Console.Out.WriteLine("Finally Navigated to New Claim Search Page");
                }

                else
                {
                    _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                }
            }

            if (!_claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();


            _claimSearch.ClickOnFilterOptionListRow(13);
        }

        protected override void ClassCleanUp()
        {
            try
            {
                _claimSearch.CloseDbConnection();
                if (automatedBase.CurrentPage.IsautomatedBase.QuickLaunchIconPresent())
                    _claimSearch.ClickOnautomatedBase.QuickLaunch();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }
        

        #endregion
        */

        #endregion
        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        /*
        #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {

            _smtstSubStatusList = _claimSearch.GetAssociatedClaimSubStatusForClient(ClientEnum.SMTST.ToString());
            _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
            _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
            _allProductList = _claimSearch.GetCommonSql.GetAllProductList();
            _claimsAssignedToUserList = _claimSearch.GetClientAssignedToList(ClientEnum.SMTST.ToString());

            _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
            _smtstReviewGroup =_claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
            _unreviewedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetUnreviewedClaimListForInternalUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.ClientUserName)[0]);
            _unreviewedClaimsCountClientUser = Convert.ToInt16(_claimSearch.GetUnreviewedClaimListForClientUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.ClientUserName)[0]);
            _pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCountforClient(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.ClientUserName)[0]);
            _flaggedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetFlaggedClaimListClient(ClientEnum.SMTST.ToString())[0]);
        }



        #endregion*/

        #region TEST SUITES

        [Test] //CAR-3157(CAR-3143)
        public void Verify_restrict_access_to_claims_not_released_to_clients()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var cotivitiUnreviewedReviewedClaimSequence =
                    paramLists["CotivitiUnreviewedReviewedClaimSequences"].Split(',').ToList();
                var cotivitiUnreviewedReviewedClaimNumbers =
                    paramLists["CotivitiUnreviewedReviewedClaimNumbers"].Split(',').ToList();

                StringFormatter.PrintMessage("Updating status of claseqs");
                _claimSearch.UpdateClaimStatus(cotivitiUnreviewedReviewedClaimSequence[0], ClientEnum.SMTST.ToString(),
                    "U");
                _claimSearch.UpdateClaimStatus(cotivitiUnreviewedReviewedClaimSequence[1], ClientEnum.SMTST.ToString(),
                    "R");

                StringFormatter.PrintMessage("Verification that client users do not have access to Cotiviti Unreviewed and Cotiviti Reviewed Claims when searching via claseq");
                foreach (var claseq in cotivitiUnreviewedReviewedClaimSequence)
                {
                    var result = _claimSearch.GetStatusReltoClientByClaseqDb(
                        $"{cotivitiUnreviewedReviewedClaimSequence[0]},{cotivitiUnreviewedReviewedClaimSequence[1]}");
                    if (claseq == cotivitiUnreviewedReviewedClaimSequence[0])
                    {
                        StringFormatter.PrintMessage("Verification for Cotiviti Unreviewed claseq");
                        result[1][0].ShouldBeEqual("U", "Status should be U");
                        result[1][1].ShouldBeEqual("F", "RelToClient should be F");

                    }
                    else
                    {
                        StringFormatter.PrintMessage("Verification for Cotiviti Reviewed claseq");
                        result[0][0].ShouldBeEqual("R", "Status should be U");
                        result[0][1].ShouldBeEqual("F", "RelToClient should be F");
                    }

                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence", claseq);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForStaticTime(2000);
                    _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                        .ShouldBeTrue("No result should be displayed");
                }

                StringFormatter.PrintMessage(
                    "Verification that client users do not have access to Cotiviti Unreviewed and Cotiviti Reviewed Claims when searching via claim ID");
                foreach (var claimId in cotivitiUnreviewedReviewedClaimNumbers)
                {
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No", claimId);
                    _claimSearch.ClickOnFindButton();
                    _claimSearch.WaitForStaticTime(2000);
                    _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                        .ShouldBeTrue($"No result should be displayed for claim ID : {claimId}");
                }
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("ExcludeDailyTest")] //TANT-79
        public void Verify_Claim_Search()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                var listBeforeSearch = _claimSearch.GetGridViewSection.GetGridAllRowData();
                var claimstatusDistinctValue = _claimSearch.GetClaimStatusforClient().Distinct().ToList();
                claimstatusDistinctValue.Count.ShouldBeEqual(1,
                    "Distinct Claim Status Should be one when search by Unreviewed");
                claimstatusDistinctValue[0].ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                    "Claims with status Client Unreviewed should display in the Search results");
                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                    "Client Action Page Should Be Opened");
                automatedBase.CurrentPage = _claimSearch = newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue(),
                    "Quick search option should be unreviewed claims.");
                _claimSearch.GetGridViewSection.GetGridAllRowData()
                    .ShouldCollectionBeEqual(listBeforeSearch, "Previous list should be retained");
            }
        }


        [Test] //US50648
        public void Verify_other_claim_number_field_with_searched_validation()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();


                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var otherClaimNumber = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "OtherClaimNumber", "Value");

                _claimSearch.IsOtherClaimNumberFieldDisplayed().ShouldBeTrue("Other Claim Number Should displayed");
                var maxCharacter = new string('a', 25) + new string('1', 26);
                _claimSearch.SetOtherClaimNoInFindClaimSection(maxCharacter);
                _claimSearch.GetOtherClaimNoInFindClaimSection()
                    .Length.ShouldBeEqual(50, "Accept only max 50 alphanumeric characters");
                _claimSearch.ClickonClaimSequenceInFindClaimSectionTextField();
                _claimSearch.GetOtherClaimNoInFindClaimSection()
                    .ShouldBeNullorEmpty("Other Claim Number Should clear when click away from that field");
                _claimSearch.SearchByOtherClaimNmber("Test123");
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No Result Message");
                _claimSearch.SearchByOtherClaimNmber(otherClaimNumber);
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(1,
                        "Claim Search Result Row Should displayed when searched by Other Claim Number");
            }
        }

        [Test] //US69155
        public void Verify_Assigned_To_search_filter_when_All_Claims_Option_is_selected_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var assignedTo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AssignedTo", "Value");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error message should be present");
                _claimSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Search cannot be initiated without any criteria entered.",
                        "Is error Mesage equal?");
                _claimSearch.ClosePageError();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", assignedTo);
                _claimSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(1, "Claim Search Result Row Should be displayed when searched by Assigned to");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetAssignedTo().ShouldBeEqual(assignedTo, "Assigned To value should match");
            }
        }

        [Test]
        public void
            Verify_that_default_value_and_the_clear_filters_clears_all_filters_claim_search_client_user() //US65789+US66263+TE-801 + CAR-3143
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                //page should be initial statte to verify default value
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                StringFormatter.PrintMessageTitle("Verify default input value for Unreviwed and Pended Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue(),
                    false); //check for type ahead functionality

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type")
                    .ShouldBeEqual("All", "Claim Type");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("All", "Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");

                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Claim Sub Status", "placeholder")
                    .ShouldBeEqual("Select one or more", "Claim Sub Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");

                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Unreviewed Claims");

                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                //_claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", paramLists["UnreviewedClaimStatus"], false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Type", paramLists["ClaimType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID", paramLists["BatchID"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence",
                    paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Claim Status when Unreviewed Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue(), "Quick Search");
                //_claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status").ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(), "Claim Status");

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears all the rest of the search filters - Unreviewed Claims ");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type")
                    .ShouldBeEqual("All", "Claim Type");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("All", "Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");


                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Pended Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Claim Sub Status",
                    paramLists["PendedClaimSubStatus1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Claim Sub Status",
                    paramLists["PendedClaimSubStatus2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To",
                    paramLists["AssignedTo"], false);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence",
                    paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", paramLists["TIN"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", paramLists["ProcCode"]);

                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Claim Sub Status when Pended Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue(), "Quick Search");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Claim Sub Status", "placeholder")
                    .ShouldBeEqual("Select one or more", "Claim Sub Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code").ShouldBeEqual("", "Proc Code");

                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Flagged Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status",
                    ClaimStatusTypeEnum.ClientReviewed.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID",
                    paramLists["BatchIDFlaggedClaim"]);
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Batch ID when Flagged Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status")
                    .ShouldBeEqual("All", "Claim Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID")
                    .ShouldBeEqual(paramLists["BatchID"], "Most recent Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test]
        [Order(1)]
        public void
            Verify_basic_search_filters_for_unreviewed_quick_Search_option_client_user() //US66263 + CAR-1408 + TE-835 + CAR-3157(CAR-3143) + CAR-3052
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                var _allProductList = _claimSearch.GetCommonSql.GetAllProductList();
                var _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                var _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
                var _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                var _unreviewedClaimsCountClientUser = Convert.ToInt16(_claimSearch.GetUnreviewedClaimListForClientUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.ClientUserName)[0]);

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_claimSearch, _allProductList);
                var appealStatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status")
                    .Values.ToList();
                var flagList = _claimSearch.GetAllFlagsFromDatabase();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                Verify_correct_search_filter_options_displayed_for(automatedBase, _claimSearch, "unreviewed_claims",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());

                //Commented as part of CAR-3157(CAR-3143)
                //StringFormatter.PrintMessageTitle("Claim Status field Validation");
                //ValidateClaimStatusforUnreviewedClaims("Claim Status");

                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList(_claimSearch, "Product", expectedProductTypeList);

                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Plan", _smtstPlanList);


                StringFormatter.PrintMessageTitle("Line of Business Field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Line Of Business", _lineOfBusinessList);

                StringFormatter.PrintMessageTitle("Claim Type Field Validation");
                ValidateDropdownListWithMappingData(automatedBase, _claimSearch, "Claim Type", "claim_type");

                #region CAR-3052

                StringFormatter.PrintMessageTitle("Flag Field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Flag", flagList);

                #endregion

                StringFormatter.PrintMessageTitle("Batch ID Validation");
                ValidateBatchIdListWithDatabase(_claimSearch, "Batch ID");


                StringFormatter.PrintMessageTitle("Review Group Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Review Group", _smtstReviewGroup);


                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence should be empty");

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN")
                    .ShouldBeNullorEmpty("TIN should be null by default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", Concat(Enumerable.Repeat("33333", 5)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("TIN")
                    .ShouldBeEqual(9, "Verification of Maximum length of TIN");

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID").ShouldBeNullorEmpty("Member ID");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("a1234", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Verification of Maximum length of TIN");

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc")
                    .ShouldBeNullorEmpty("Proc Should Be Null By Default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", Concat(Enumerable.Repeat("a1234", 5)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc")
                    .ShouldBeEqual(5, "Verification of Maximum length of Proc");


                StringFormatter.PrintMessageTitle("Appeal Status Validation");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateDropdownListWithMappingData(automatedBase, _claimSearch, "Appeal Status", "appeal_status");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _unreviewedClaimsCountClientUser = Convert.ToInt16(
                    _claimSearch.GetUnreviewedClaimListForClientUser(ClientEnum.SMTST.ToString(),
                        automatedBase.EnvironmentManager.ClientUserName)[0]);
                _claimSearch.ClickOnLoadMore();

                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                numbers[1].ShouldBeEqual(_unreviewedClaimsCountClientUser,
                    "Unreviewed Claims count should match with the value from database");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_unreviewedClaimsCountClientUser,
                        "For claim count less than 50, clicking on Load more should equal the Unreviewed Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50,
                        "For claim count less than 50, clicking on Load more should equal the Unreviewed Claims count with the database");
                }

                var appealStatusAbb = new List<string> {"O', 'N', 'T', 'C','M", "O", "N", "T", "C"};
                if (_claimSearch.CurrentPageUrl.Contains("dev"))
                {
                    for (var j = 0; j < appealStatusList.Count; j++)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Status",
                            appealStatusList[j]);
                        _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _claimSearch.WaitForWorking();

                        if (automatedBase.CurrentPage.GetPageHeader() == PageHeaderEnum.ClaimAction.GetStringValue())
                            automatedBase.CurrentPage.ClickOnBrowserBackButton();

                        var claseqList = _claimSearch.GetDataWithAppealCountByCol(3);
                        var claseqlist = claseqList.Select(x => x.Split('-')[0]).ToList();
                        var classublist = claseqList.Select(x => x.Split('-')[1]).ToList();
                        var newList = claseqlist.Zip(classublist, (s, i) => new {sv = "(" + s, iv = i + ")"}).ToList();
                        var newList1 = newList.Select(x => x.sv + "," + x.iv).ToList();
                        var finallist = Join(",", newList1.Select(x => x));
                        var appealCountList = _claimSearch.GetGridViewSection.GetAppealCountList();
                        _claimSearch.GetAppealCountByClaseqAppealStatus(finallist, appealStatusAbb[j])
                            .ShouldCollectionBeEquivalent(appealCountList, "Appeal Count Should be equal ");
                    }
                }
            }
        }

        [Test]
        [Order(3)]
        public void
            Verify_basic_search_filters_for_pended_quick_Search_option_client_user() //US65789 +TE-451+ TE-452 + TE-801
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                var _allProductList = _claimSearch.GetCommonSql.GetAllProductList();
                var _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                var _smtstSubStatusList = _claimSearch.GetAssociatedClaimSubStatusForClient(ClientEnum.SMTST.ToString());
                var _pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCountforClient(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.ClientUserName)[0]);
                var _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_claimSearch, _allProductList);

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                Verify_correct_search_filter_options_displayed_for(automatedBase, _claimSearch, "pended_claims",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());

                StringFormatter.PrintMessageTitle("Claim Sub Status field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Claim Sub Status", _smtstSubStatusList);


                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList(_claimSearch, "Product", expectedProductTypeList);

                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Plan", _smtstPlanList);


                StringFormatter.PrintMessageTitle("Assigned To Field Validation");
                ValidateAssignedTo(_claimSearch, "Assigned To");
                //TE-453
                StringFormatter.PrintMessage("Verify Review Group ");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Review Group", _smtstReviewGroup);

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence should be empty");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnLoadMore();

                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                numbers[1].ShouldBeEqual(_pendedClaimsCount,
                    "Pended Claims count should match with the value from database");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_pendedClaimsCount,
                        "For claim count less than 50, clicking on Load more should equal the Pended Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50,
                        "For claim count less than 50, clicking on Load more should equal the Pended Claims count with the database");
                }

                var claimsubstatusList = _claimSearch.GetClaimStatusforClient().Distinct().ToList();

                _smtstSubStatusList.Add("CV QA Audit 2");
                claimsubstatusList.CollectionShouldBeSubsetOf(_smtstSubStatusList,
                    "Pended Claims Sub Status in the Search List should match with the list from the database");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                //TE-453
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("a1234", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Verification of Maximum length of Member Id");

                //TE-801
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", Concat(Enumerable.Repeat("12345", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("TIN")
                    .ShouldBeEqual(9, "Verification of Maximum length of TIN");

                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code",
                    Concat(Enumerable.Repeat("a1234", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc Code")
                    .ShouldBeEqual(5, "Verification of Maximum length of Proc Code");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }

        [Test] //TE-556 + TE-451 + CAR-3052 [CAR-3179]
        public void
            Verify_search_results_with_extra_search_filters_are_correct_unreviewed_claims_client_user() //US66263 +TE-453+ TE-451+ TE-452 + TE-556 + CAR-1716

        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var clientName = paramLists["ClientName"];
                var flags = paramLists["Flag"].Split(';').ToList();
                var username = paramLists["Uname"];
                try
                {
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                        ("Quick Search", ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                        ("Line Of Business", paramLists["LineofBusiness"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnSideBarHeader();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                        ("Claim Type", paramLists["FormTypeH"], false);
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                        ("Batch ID", paramLists["BatchID"]);
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                        ("Review Group", paramLists["ReviewGroup"].Split(',').ToList()[0]);

                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel
                        ("Provider Sequence", paramLists["ProviderSequence"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.GetClientPlanName().ShouldBeEqual("Default Plan", "Plan");
                    _claimSearch.GetClientReviewGroup().ShouldBeEqual(paramLists["ReviewGroup"].Split(',').ToList()[0]);
                    _claimSearch.ClickSearchResultRow();
                    _claimSearch.GetClientLOBName().ShouldBeEqual(paramLists["LineofBusiness"], "Is LOB equal?");
                    _claimSearch.GetClientBatchID().ShouldBeEqual(paramLists["BatchID"], "Is Batch ID equal?");
                    _claimSearch.GetClientFormType().ShouldBeEqual(paramLists["FormTypeH"], "Is Form Type of H equal?");
                    _claimSearch.GetClientProvSeq()
                        .ShouldBeEqual(paramLists["ProviderSequence"], "Is Provider Sequence equal?");
                    _claimSearch.GetClaimSearchResultsDetails(3)
                        .ShouldBeEqual(paramLists["ClaimSeq"], "Is Claim Sequence equal?");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                        ("Claim Type", paramLists["FormTypeU"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.ClickSearchResultRow();
                    _claimSearch.GetClientFormType().ShouldBeEqual(paramLists["FormTypeU"], "Is Form Type of U equal?");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                        ("Product", paramLists["ProductType"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", paramLists["TIN"]);
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
                    {
                        automatedBase.CurrentPage.ClickOnBrowserBackButton();
                        automatedBase.CurrentPage.WaitForPageToLoad();
                    }

                    _claimSearch.ClickSearchResultRow();
                    _claimSearch.GetClientTIN().ShouldBeEqual(paramLists["TIN"], "Is TIN equal?");
                    _claimSearch.GetGridViewSection.GetValueInGridBylabelAndRow("Mem ID:")
                        .ShouldBeEqual(paramLists["MemberId"], "Is Member ID equal?");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    //TE-451 + TE-556
                    StringFormatter.PrintMessage("Verify Results when multiple Review Groups and Proc is selected");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                        paramLists["ReviewGroup"].Split(',').ToList());
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["ValidProc"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEquivalent(
                        _claimSearch.GetUnreviewedClaimForReviewGroupClient(paramLists["ValidProc"]), "Results equal?");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    StringFormatter.PrintMessage("Verify Search Cannot Be Instantiated With Invalid Proc Code");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["InvalidProc"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue(
                            "Search Should Not Be Initiated with invalid proc code");
                    _claimSearch.GetPageErrorMessage().ShouldBeEqual(
                        "Selected Proc Code is invalid. Please search again.",
                        "Verify the popup message");
                    _claimSearch.ClosePageError();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    StringFormatter.PrintMessage(
                        "Verify No Matching Records Found message when no results found for selected proc code");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["Proc"]);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    #region CAR-1716 + CAR-3052 [CAR-3179]

                    _claimSearch.IsFlagLabelPresent().ShouldBeTrue("Is Flag Label present above Batch ID?");
                    _claimSearch.GetSideBarPanelSearch.ClickOnToggleIcon("Flag");
                    _claimSearch.IsFlagDropDownListAscending().ShouldBeTrue("Is Flag dropdown is ascending?");
                    _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Flag", flags);
                    //_claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flag, false);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.CloseAnyPopupIfExist();
                    if (_claimSearch.GetGridViewSection.IsLoadMorePresent())
                    {
                        var loadMoreValue = _claimSearch.GetLoadMoreText();
                        var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                            .Select(m => int.Parse(m.Trim())).ToList();
                        numbers[1].ToString().ShouldBeEqual(
                            _claimSearch.GetCountOfUnreviewedClaimsForSpecificFlagForClient(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                            "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                    }
                    else
                    {
                        _claimSearch.GetGridViewSection.GetRecordRowsCountFromPage().ShouldBeEqual(
                            _claimSearch.GetCountOfUnreviewedClaimsForSpecificFlagForClient(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                            "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                    }
                    #endregion
                }
                finally
                {
                    _claimSearch.CaptureScreenShot("Failed reason for Failing ClaimSearchClient Test");
                }
            }
        }

        [Test]
        public void
            Verify_search_results_with_extra_search_filters_are_correct_pended_claims_client_user() //US65789 +TE-451+ TE-452 + TE-801
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                    ("Claim Sub Status", paramLists["ClaimSubStatus"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Assigned To", paramLists["AssignedTo"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel
                    ("Provider Sequence", paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetClientUserClaimNumber().ShouldBeEqual(paramLists["ClaimNumber"], "Claim Number");
                if (_claimSearch.IsAppealCountPresentByRow(1))
                    _claimSearch.GetClientUserClaimSubStatus()
                        .ShouldBeEqual(paramLists["ClaimSubStatus"], "Claim Sub Status");
                else
                    _claimSearch.GetClaimSubStatus().ShouldBeEqual(paramLists["ClaimSubStatus"], "Claim Sub Status");

                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetAssignedTo().ShouldBeEqual(paramLists["AssignedTo"], "Assigned To");
                _claimSearch.GetClientProvSeq().ShouldBeEqual(paramLists["ProviderSequence"], "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Product", paramLists["ProductType"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify search Results For Member Id field");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetGridViewSection.GetValueInGridBylabelAndRow("Mem ID:")
                    .ShouldBeEqual(paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Results for Review Group");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("Data Present for SMTST?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();


                //TE-801
                StringFormatter.PrintMessage("Verify Results For TIN and Proc Code");
                _claimAction =
                    _claimSearch.SearchByTINAndProCodeToNavigateToNewClaimActionPage(paramLists["TIN"],
                        paramLists["ProcCode"], true);
                _claimAction.GetProviderTin().ShouldBeEqual(paramLists["TIN"], "TIN value should match");
                _claimAction.GetProcCodeForRow(1).ShouldBeEqual(paramLists["ProcCode"], "Proc Code value should match");
                var newClaimSearch = _claimAction.ClickClaimSearchIcon();
                newClaimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEquivalent(
                    _claimSearch.GetPendedClaimsFromSpecificProcCodeAndTIN(paramLists["ProcCode"], paramLists["TIN"]),
                    "Results equal?");
                newClaimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test] //US66943
        public void Verify_client_users_are_allowed_to_work_through_the_list_of_claim_results_in_Claim_Search()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(4))
            {
                ClaimSearchPage _claimSearch;
                LoginPage Login;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimNo = paramLists["ClaimNo"];
                var auditDate = paramLists["AuditDate"];

                /*Login = _claimSearch.Logout();
                automatedBase.QuickLaunch = Login.LoginAsClientUser1();*/
                automatedBase.CheckTestClientAndSwitch();
                var claimList = _claimSearch.GetClaimsByClaimNoforClientUserFromDatabase(claimNo);
                var newClaimList = new List<string>();
                _claimSearch.UpdateClaimStatusToUnreviewedFromDatabase(claimNo);
                _claimSearch.DeleteClaimAuditOnlyByClaimNo(claimNo, auditDate);

                //_claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();

                _claimSearch.SetClaimNoInFindClaimSection(claimNo);
                _claimSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                var searchListCount = _claimSearch.GetSearchResultCount();
                for (var i = 1; i <= searchListCount; i++)
                {
                    newClaimList.Add(_claimSearch.GetGridViewSection.GetValueInGridByColRow(3, i));
                }


                searchListCount.ShouldBeEqual(claimList.Count(),
                    "Claim Search result count should match with database count");
                _claimSearch.IsClaimSearchResultsLocked(automatedBase.EnvironmentManager.ClientUserName1, claimList)
                    .ShouldBeFalse("Is any claims in claim search result is locked?");

                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(newClaimList[0],
                    "Page should advance to next claim of the claim search list.");
                newClaimAction.IsCurrentClaimLocked(automatedBase.EnvironmentManager.ClientUserName1)
                    .ShouldBeTrue("Current Claim should be locked.");
                newClaimAction.ClickOnApproveButton();
                newClaimAction.WaitForWorkingAjaxMessage();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after approving claim.");
                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(newClaimList[1],
                    "Page should advance to next claim of the claim search list.");
                newClaimAction.ClickOnTransferApproveButton();
                newClaimAction.SelectStatusCode("Documentation Requested");
                newClaimAction.ClickOnSaveButton();
                newClaimAction.WaitForWorkingAjaxMessage();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after clicking Approve/Transfer claim.");

                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(newClaimList[2],
                    "Page should advance to next claim of the claim search list.");
                newClaimAction.ClickOnTransferButton();
                newClaimAction.SelectStatusCode("Documentation Requested");
                newClaimAction.ClickOnSaveButton();
                newClaimAction.WaitForWorkingAjaxMessage();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after clicking transfer claim.");
                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(newClaimList[3],
                    "Page should advance to next claim of the claim search list.");

                for (var i = 4; i <= searchListCount; i++)
                {

                    newClaimAction.ClickOnNextButton();
                    newClaimAction.WaitForWorkingAjaxMessage();
                    if (i != searchListCount)
                        newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(newClaimList[i],
                            "Page should advance to next claim of the claim search list.");

                }

                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page after all claims in claim search result are iterated.");

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim No").ShouldBeEqual(claimNo,
                    "Search criteria should retained after navigating back to Claim Search page.");
                searchListCount.ShouldBeEqual(claimList.Count, "Claim Search result should retain");

                newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.WaitForStaticTime(2000);

                newClaimAction.SetClaimNoInFindClaimSection(claimNo);
                newClaimAction.ClickOnFindButton();
                newClaimAction.WaitForWorkingAjaxMessage();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page when claim is searched with records.");

                newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.WaitForStaticTime(2000);

                newClaimAction.SetClaimNoInFindClaimSection("Test123");
                newClaimAction.ClickOnFindButton();
                newClaimAction.WaitForWorkingAjaxMessage();
                newClaimAction.GetEmptyMessage()
                    .ShouldBeEqual("No matching records were found.", "No Match Record Message");
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page when claim is searched without records");
            }
        }

        [Test] //US68072 + CAR-1408 +TE-451+ TE-452 + TE-835 + CAR-3052
        [Order(4)]
        public void Verify_basic_search_filters_for_Flagged_Claims_in_Quick_Search_option_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                var _allProductList = _claimSearch.GetCommonSql.GetAllProductList();
                var _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                var _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
                var _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                var _flaggedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetFlaggedClaimListClient(ClientEnum.SMTST.ToString())[0]);

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_claimSearch, _allProductList);
                var claimstatusList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "flagged_claims_status").Values.ToList();
                var appealStatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status")
                    .Values.ToList();
                var flagList = _claimSearch.GetAllFlagsFromDatabase();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                Verify_correct_search_filter_options_displayed_for(automatedBase, _claimSearch, "flagged_claims",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());

                StringFormatter.PrintMessageTitle("Claim Status field Validation");
                ValidateClaimStatusforFlaggedClaims(automatedBase, _claimSearch, "Claim Status");

                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList(_claimSearch, "Product", expectedProductTypeList);

                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Plan", _smtstPlanList);

                StringFormatter.PrintMessageTitle("Line of Business Field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Line Of Business", _lineOfBusinessList);

                #region CAR-3052

                StringFormatter.PrintMessageTitle("Flag Field Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Flag", flagList);

                #endregion

                StringFormatter.PrintMessageTitle("Batch ID Validation");
                ValidateBatchIDforFlaggedClaims(_claimSearch, "Batch ID");

                StringFormatter.PrintMessageTitle("Review Group Validation");
                ValidateFieldSupportingMultipleValues(_claimSearch, "Review Group", _smtstReviewGroup);


                StringFormatter.PrintMessageTitle("Appeal Status Validation");
                ValidateDropdownListWithMappingData(automatedBase, _claimSearch, "Appeal Status", "appeal_status");

                StringFormatter.PrintMessage("Verify Member ID");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("@1234", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Maximum allowed value 100?");

                StringFormatter.PrintMessage("Verify Proc Code");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", Concat(Enumerable.Repeat("A123", 5)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc")
                    .ShouldBeEqual(5, "Maximum allowed value 5 ?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnLoadMore();

                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                numbers[1].ShouldBeEqual(_flaggedClaimsCountInternalUser,
                    "Unreviewed Claims count should match with the value from database");

                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_flaggedClaimsCountInternalUser,
                        "For claim count less than 50, clicking on Load more should equal the Flagged Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }

                var listOfClaimStatus = _claimSearch.GetClaimStatusforClient().Distinct().ToList();
                claimstatusList.Add("Revised Claims");
                listOfClaimStatus.CollectionShouldBeSubsetOf
                (claimstatusList,
                    "The Claim status of the search results should match with values from Claim Status dropdown ");
                _claimSearch.ClickOnFilterOptionListRow(12);
                _claimSearch.ClickOnFilterOptionListRow(12);
                var appealStatusAbb = new List<string> {"O', 'N', 'T', 'C", "O", "N", "T", "C"};
                if (_claimSearch.CurrentPageUrl.Contains("dev"))
                {
                    for (var j = 0; j < appealStatusList.Count; j++)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Status",
                            appealStatusList[j]);
                        _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _claimSearch.WaitForWorking();
                        _claimSearch.GetGridViewSection.ClickOnGridRowByRowInClaimSearch();
                        var claimseq = _claimSearch.GetGridViewSection.GetValueInGridByColRow(3, 1);

                        string claseq = "(" + claimseq.Replace('-', ',') + ")";
                        var appealCountList = _claimSearch.GetGridViewSection.GetAppealCountList().Take(1).ToList();

                        _claimSearch.GetAppealCountByClaseqAppealStatus(claseq, appealStatusAbb[j])
                            .ShouldCollectionBeEqual(appealCountList, "Appeal Count Should be equal ");


                        _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();


                    }

                }
            }
        }

        [Test] //US68072 +TE-451+ TE-452 +TE-556 + CAR-1716 + CAR-3052 [CAR-3179]
        public void Verify_search_results_with_extra_search_filters_are_correct_flagged_claims_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var clientName = paramLists["ClientName"];
                var flags = paramLists["Flag"].Split(';').ToList();
                var username = paramLists["Uname"];
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Batch ID", paramLists["BatchID"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetClientUserClaimNumber(2).ShouldBeEqual(paramLists["ClaimNumber"], "Claim Number");
                _claimSearch.ClickSearchResultRow(2);
                _claimSearch.ClickSearchResultRowByValue(paramLists["ClaimNumber"]);
                _claimSearch.GetGridViewSection.GetValueInGridByColRow(3, 2).ShouldBeEqual(paramLists["ClaimSeq"]);
                _claimSearch.GetClientBatchId().ShouldBeEqual(paramLists["BatchID"], "Batch ID");
                _claimSearch.GetLineOfBusinessfromClaimDetails()
                    .ShouldBeEqual(paramLists["LineofBusiness"], "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnClaimSearchResultByRow(1);
                _claimSearch.GetGridViewSection.GetValueInGridBylabelAndRow("Mem ID:")
                    .ShouldBeEqual(paramLists["MemberId"], "Member ID Value Correct?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();


                //TE-451 + TE-556
                StringFormatter.PrintMessage("Verify Results when multiple Review Groups and Proc is selected");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["ValidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEquivalent(
                    _claimSearch.GetFlaggedClaimForReviewGroupClient(paramLists["ValidProc"]), "Results equal?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Search Cannot Be Instantiated With Invalid Proc Code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["InvalidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "Search Should Not Be Initiated with invalid proc code");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual("Selected Proc Code is invalid. Please search again.",
                    "Verify the popup message");
                _claimSearch.ClosePageError();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage(
                    "Verify No Matching Records Found message when no results found for selected proc code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["Proc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                #region CAR-1716 + CAR-3052

                _claimSearch.IsFlagLabelPresent().ShouldBeTrue("Is Flag Label present above Batch ID?");
                _claimSearch.GetSideBarPanelSearch.ClickOnToggleIcon("Flag");
                _claimSearch.IsFlagDropDownListAscending().ShouldBeTrue("Is Flag dropdown is ascending?");
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Flag", flags);
                //_claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flags, false);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.CloseAnyPopupIfExist();

                if (_claimSearch.GetGridViewSection.IsLoadMorePresent())
                {
                    var loadMoreValue = _claimSearch.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                        .Select(m => int.Parse(m.Trim())).ToList();
                    numbers[1].ToString().ShouldBeEqual(
                        _claimSearch.GetCountOfFlaggedClaimsByFlaggedNameForClient(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }
                else
                {
                    _claimSearch.GetGridViewSection.GetRecordRowsCountFromPage().ShouldBeEqual(
                        _claimSearch.GetCountOfFlaggedClaimsByFlaggedNameForClient(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }

                #endregion

            }
        }

        [Test] //US68186
        public void Verify_client_received_date_in_the_Claim_search_result_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSearchResultDetails = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSearchResultDetails",
                        "Value").Split(';');
                var newClaimAction =
                    _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(paramLists["ClaimSequence"]);
                newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetClaimSearchResultsDetails(2)
                    .ShouldBeEqual(claimSearchResultDetails[0], "Claim Number should match");
                _claimSearch.GetClaimSearchResultsDetails(4)
                    .ShouldBeEqual(claimSearchResultDetails[1], "Flags should match");
                _claimSearch.GetClaimSearchResultsDetails(5)
                    .ShouldBeEqual(claimSearchResultDetails[2], "Plan should match");
                _claimSearch.GetClaimSearchResultsDetails(6)
                    .ShouldBeEqual(claimSearchResultDetails[3], "Provider Name should match");
                _claimSearch.GetClaimSearchResultsDetails(8)
                    .ShouldBeEqual(claimSearchResultDetails[4], "Review Group should match");
                _claimSearch.GetClaimSearchResultsDetails(9)
                    .ShouldBeEqual(claimSearchResultDetails[5], "Savings should match");
                _claimSearch.GetClaimSearchResultsDetails(10)
                    .ShouldBeEqual(claimSearchResultDetails[6], "Member Id should match");
                _claimSearch.GetClaimSearchResultsDetails(11)
                    .ShouldBeEqual(claimSearchResultDetails[7], "Received Date should match");
                _claimSearch.GetClaimSearchResultsDetails(12)
                    .ShouldBeEqual(claimSearchResultDetails[8], "Claim Status should match");
                _claimSearch.GetClaimSearchResultsDetails(3)
                    .ShouldBeEqual(paramLists["ClaimSequence"], "ClaimSeq should match");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.WaitForStaticTime(1000);
                _claimSearch.IsClientLOBPresent()
                    .ShouldBeTrue("LOB should not be present in the Search Results row");

                newClaimAction =
                    _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(
                        paramLists["ClaimSequenceWithState"]);
                newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetClaimSearchResultsDetails(7)
                    .ShouldBeEqual(claimSearchResultDetails[9], "State should match");

            }
        }

        [Test]//US66665
        public void Validate_nucleus_worklist_is_ordered_by_claim_due_date_client_received_date_claim_seq_ascendingly()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                        "Claseq", "Value");

                var newClaimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                var claimSeqList = new List<string> {newClaimAction.GetClaimSequence()};
                newClaimAction.ClickClaimSearchIcon();
                claimSeqList.AddRange(newClaimAction.GetListOfNextClaimsInWorklistSectionDisplayed());
                var dbClaSeqList = _claimSearch.GetListOfClaimsForPciWorklist(claimSeqList).ToList();



                List<DateTime?> dueDateList = dbClaSeqList
                    .Select(list => list[1] != "" ? DateTime.Parse(list[1]) : (DateTime?) null).ToList();
                List<DateTime?> clientRecvDate = dbClaSeqList
                    .Select(list => list[1] != "" ? DateTime.Parse(list[2]) : (DateTime?) null).ToList();
                var clasSeqList = dbClaSeqList.Select(row => row[0]).ToList();
                if (dueDateList.Any(same => same != dueDateList[0]))
                {
                    _claimSearch.IsListInAscendingOrder(dueDateList)
                        .ShouldBeTrue("List should be sorted in ascending order of due date.");
                }
                else if (clientRecvDate.Any(same => same != clientRecvDate[0]))
                {
                    _claimSearch.IsListInAscendingOrder(clientRecvDate)
                        .ShouldBeTrue("List should be soreted in ascending order of client received date.");
                }
                else
                {
                    _claimSearch.IsListInAscendingOrder(clasSeqList)
                        .ShouldBeTrue("List should be soreted in ascending order of claim sequence value.");
                }

                clasSeqList.ShouldCollectionBeEqual(claimSeqList,
                    "The worklist order should be sorted in the basis of due date, client received date and claseq in  ascending order.");

                if (!_claimSearch.GetSideBarPanelSearch.GetTopHeaderName().Equals("Find Claim"))
                    _claimSearch.ClickOnFindClaimIcon();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }

        [Test] //US68301 + CAR-1395
        public void Verify_Search_list_sorting_for_client_Users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "claim_sorting_option_list_client")
                        .Values.ToList();
                var claimNoForAppealCount = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimNoForAppealCount", "Value");
                var claimNumForMultipleClaSub = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimNumForMultipleClaSub", "Value");

                _claimSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                _claimSearch.GetFilterOptionList().ShouldCollectionBeEqual(filterOptions,
                    "Filter Options Lists Collection Should Equal");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.IsListStringSortedInDescendingOrder(2)
                    .ShouldBeTrue("Is Claim Result sorted by Claim Number in Descending Order (Default Sort)?");

                ValidateClaimSearchRowSorted(_claimSearch, 2, 1, "Claim Number");
                ValidateClaimSearchRowSorted(_claimSearch, 3, 2, "Claim Sequence");
                ValidateClaimSearchRowSorted(_claimSearch, 4, 3, "Flag");
                ValidateClaimSearchRowSorted(_claimSearch, 5, 4, "Plan");
                ValidateClaimSearchRowSorted(_claimSearch, 6, 5, "Provider Name");
                ValidateClaimSearchRowSorted(_claimSearch, 7, 6, "State");
                ValidateClaimSearchRowSorted(_claimSearch, 8, 7, "Review Group");
                ValidateClaimSearchRowSorted(_claimSearch, 9, 8, "Savings");
                ValidateClaimSearchRowSorted(_claimSearch, 10, 9, "MemberId");
                ValidateClaimSearchRowSorted(_claimSearch, 11, 10, "Client Received Date");
                ValidateClaimSearchRowSorted(_claimSearch, 12, 11, "Claim Status");

                StringFormatter.PrintMessageTitle("Verification of Clear Sort");
                //_claimSearch.ClickOnFilterOptionListRow(2);
                _claimSearch.ClickOnFilterOptionListRow(13);
                StringFormatter.PrintMessage("Clicked on Clear Sort.");

                _claimSearch.IsListStringSortedInDescendingOrder(2).ShouldBeTrue(
                    "Is Claim Result sorted by Claim Number in Descending Order (Default Sort)?");

                StringFormatter.PrintMessageTitle("Verification of Sorting after Clear Sort");
                _claimSearch.ClickOnFilterOptionListRow(11);
                _claimSearch.IsListStringSortedInAscendingOrder(12)
                    .ShouldBeTrue("Claim Status Should sorted in Ascending Order");
                _claimSearch.ClickOnFilterOptionListRow(1);
                _claimSearch.IsListStringSortedInAscendingOrder(2)
                    .ShouldBeTrue("Claim Number Should sorted in Ascending Order");

                #region CAR-1395

                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                _claimSearch.SearchByClaimNmber(claimNoForAppealCount);
                _claimSearch.ClickOnFilterOptionListRow(12);
                _claimSearch.GetSearchResultListByCol(13).IsInAscendingOrder()
                    .ShouldBeTrue("Is Appeal Count in ascending order?");
                _claimSearch.ClickOnFilterOptionListRow(12);
                _claimSearch.GetSearchResultListByCol(13).IsInDescendingOrder()
                    .ShouldBeTrue("Is Appeal Count in descending order?");


                StringFormatter.PrintMessageTitle("Validation of Sorting by Claim Sequence");
                _claimSearch.SearchByClaimNmber(claimNumForMultipleClaSub);

                var expectedDescendingSortedClaimSequence =
                    _claimSearch.GetClaimSequenceByClaimNoInDescendingOrder(claimNumForMultipleClaSub);
                var expectedAscendingSortedClaimSequence =
                    _claimSearch.GetClaimSequenceByClaimNoInAscendingOrder(claimNumForMultipleClaSub);


                _claimSearch.ClickOnFilterOptionListRow(2);
                _claimSearch.GetSearchResultListByCol(3).ShouldCollectionBeEqual(
                    expectedAscendingSortedClaimSequence,
                    "Claim Sequence and Claim sub should be sorted in Ascending Order");

                _claimSearch.ClickOnFilterOptionListRow(2);
                _claimSearch.GetSearchResultListByCol(3).ShouldCollectionBeEqual(
                    expectedDescendingSortedClaimSequence,
                    "Claim Sequence and Claim sub should be sorted in descending order.");

                #endregion
                
            }
        }

        [Test] //US68396
        public void Verify_Sort_order_retains_when_return_to_claim_search()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimNum = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimNo", "Value");
                _claimSearch.SearchByClaimNmber(claimNum.Split(',')[0]);
                StringFormatter.PrintMessage("Sort by Claim  Status");

                _claimSearch.ClickOnFilterOptionListRow(7);
                _claimSearch.IsListInAscendingOrderBySavings()
                    .ShouldBeTrue("Is Search Result Sorted in ascending Order by Saving?");
                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                    "Page Should Navigate to Claim Action");
                newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                    "Page Should navigate back to Claim Search page.");
                _claimSearch.IsListInAscendingOrderBySavings()
                    .ShouldBeTrue("Is Saving sorting retained after return back to Claim Search page?");
                _claimSearch.SearchByClaimNmber(claimNum.Split(',')[1]);
                _claimSearch.IsListInAscendingOrderBySavings()
                    .ShouldBeTrue("Is Saving sorting retained after next search?");
            }
        }



        [Test] //US69154
        public void Verify_previously_viewed_claims_are_listed_in_find_claim_section_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value").Split(';');
                StringFormatter.PrintMessage("Previously Viewed Claims Verification");
                foreach (var claseq in claimSeq)
                {
                    var newClaimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                    _claimSearch = newClaimAction.ClickClaimSearchIcon();
                }

                var claseqList = claimSeq.Reverse().ToList();
                claseqList.RemoveAt(3);
                var previouslyViewedClaseq = _claimSearch.GetPreviouslyViewedClaimList();
                previouslyViewedClaseq.ShouldNotContain(claimSeq[0], "Only 3 recently viewed claims should be listed");
                previouslyViewedClaseq.ShouldCollectionBeEqual(claseqList,
                    "3 most recently viewed claims, with most recent on top should be listed in this section");
                var _claimAction = _claimSearch.ClickOnPreviouslyViewedClaimSequenceLinkByRow(3);
                _claimAction.GetCurrentClaimSequence()
                    .ShouldBeEqual(claseqList[2],
                        "When Return to claim link for Claseq: " + claseqList[2] +
                        " is clicked , page should redirect to claim action page for that claim: " + claseqList[2]);
                _claimSearch = _claimAction.ClickClaimSearchIcon();
            }
        }


        [Test] //US69158
        public void Validate_claim_lock_is_present_when_claim_is_viewed_by_other_user_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimNo", "Value");
                
                automatedBase.QuickLaunch =
                    _claimSearch.Logout().LoginAsClientUserHavingFfpEditOfPciFlagsAuthority();
                automatedBase.CheckTestClientAndSwitch();
                automatedBase.CurrentPage = _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var newClaimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                automatedBase.CurrentPage = newClaimAction;
                StringFormatter.PrintMessage(
                    Format("Open claim aciton page for claim sequence: {0} with direct URL ", claimSeq));
                var newClaimActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                newClaimAction =
                    _claimSearch.SwitchToOpenNewClaimActionByUrl(newClaimActionUrl,
                        automatedBase.EnvironmentManager.HciAdminUsername1);
                newClaimAction.IsClaimLocked()
                    .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                newClaimAction.GetLockIConTooltip()
                    .ShouldBeEqual(
                        "This claim has been opened in view mode. It is currently locked by uiautomation_cl2 client2 (uiautomation_cl2).",
                        "Is Lock Message Equal?");
                newClaimAction.ClickClaimSearchIcon();

                StringFormatter.PrintMessage(
                    Format("Validate lock on claim sequence: {0} when returninig to new claim search page ",
                        claimSeq));
                if (!_claimSearch.GetSideBarPanelSearch.GetTopHeaderName().Equals("Find Claim"))
                    _claimSearch.ClickOnFindClaimIcon();
                StringFormatter.PrintMessage(
                    Format("Validate lock on claim sequence: {0} by searching from new claim search page ",
                        claimSeq));
                _claimSearch.SearchByClaimSequence(claimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                newClaimAction.IsClaimLocked()
                    .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                newClaimAction.GetLockIConTooltip()
                    .ShouldBeEqual(
                        "This claim has been opened in view mode. It is currently locked by uiautomation_cl2 client2 (uiautomation_cl2).",
                        "Is Lock Message Equal?");
                newClaimAction.ClickClaimSearchIcon();

                StringFormatter.PrintMessage(
                    Format("Validate lock on claim sequence: {0} when returninig to new claim search page ",
                        claimSeq));
                _claimSearch.GetGridViewSection.GetTitleOfListIconPresentInGridForClassName("lock")
                    .ShouldBeEqual(
                        "This claim has been opened in view mode. It is currently locked by uiautomation_cl2 client2 (uiautomation_cl2).",
                        "Is Lock Message Equal?");
                _claimSearch.SearchByClaimNmber(claimNo);
                _claimSearch.IsClaimLockPresentForClaimSequence(claimSeq)
                    .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
            }
        }

        [Test, Category("ClaimViewRestriction")] //CAR-506,CAR-495
        public void Verify_claim_should_display_for_restricted_claim_for_all_user_type()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                automatedBase.CurrentPage = _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                _claimSearch.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(claimSequence,
                        automatedBase.EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(Format("{0} is in restricted group", claimSequence));
                var url = Format("app/#/clients/{0}/claims/{1}/{2}/lines/claim_lines?claimSearchType=findClaims",
                    ClientEnum.SMTST, claimSequence.Split('-')[0], claimSequence.Split('-')[1]);

                var currentPage =
                    _claimSearch.VisitAndReturnPageByUrlFoAuthorizedPage<ClaimActionPage>(url);
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence, "Is Claim Action Page Open?");
                currentPage.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                currentPage.ClickClaimSearchIcon();
                currentPage.ClickSearchIcon();
                _claimSearch.SearchByClaimSequence(claimSequence);
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence);

            }
        }

        [Test]//TE264
        public void Verify_Auto_trim_Off_spaces_for_fields_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                StringFormatter.PrintMessage("Verify extra space is removed from fields");

                /*
                 out of scope
                  _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",  Concat(paramLists["ClaimSequence"]," "));
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Sequence").Length.ShouldBeEqual(paramLists["Claim Sequence"].Length, "extra spaces removed?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();*/

                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No",
                    Concat(paramLists["ClaimNo"], "      "));
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(0, "Claim Search Result Row  for search with claim number");
                _claimSearch.GetClientUserClaimNumber().ShouldBeEqual(paramLists["ClaimNo"], "Claim Number");

                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Other Claim Number",
                    Concat(paramLists["OtherClaimNumber"], "  "));
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(0, "Claim Search Result Row for search with other claim number ");
                _claimSearch.GetClientUserClaimNumber()
                    .ShouldBeEqual(paramLists["OtherClaimNumber"], "Other Claim Number");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }


        [Test] //TE692
        public void Verify_MemberId_search_filter_when_All_claims_option_selected_For_Clients()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var validMemberId = paramLists["MemberId"].Split(',')[0];
                var restrictedMemberId = paramLists["MemberId"].Split(',')[1];
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                    "Default filter for quick search is All claims");
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Member ID")
                    .ShouldBeTrue("member ID field should be displayed");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID")
                    .ShouldBeNullorEmpty("Default value for BatchId ");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", validMemberId);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount().ShouldBeGreater(0, "Claim Search Result Row ");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetGridViewSection.GetValueInGridBylabelAndRow("Mem ID:")
                    .ShouldBeEqual(validMemberId, "Member id correct?");

                StringFormatter.PrintMessage("verify character limit");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("a1@_A", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Verification of Maximum length of BatchId");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessage("verifying invalid member id");
                _claimSearch.GetNoMatchingRecordFoundMessage().ShouldBeEqual("No matching records were found.",
                    "Verify Empty Record Message on Sidebar Panel");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("verify Claim Restriction Message for restricted patient ");
                _claimSearch = automatedBase.CurrentPage.Logout().LoginAsClientUserWithClaimViewRestriction()
                    .NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", restrictedMemberId);
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error Message displayed when searching restricted patient");
                _claimSearch.GetPageErrorMessage()
                    .ShouldBeEqual(
                        $"This claim has {_claimSearch.GetPatientRestrictionDescriptionByPatNum(restrictedMemberId)} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.");
                _claimSearch.ClosePageError();

            }

        }

        [Test] //CAR-766
        public void Verify_full_facility_name_displays_on_search_result()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                var prvName =
                    _claimSearch.GetCommonSql.GetFullPrvNameForGivenClaimNo(
                        _claimSearch.GetGridViewSection.GetValueInGridByColRow());
                _claimSearch.GetGridViewSection.GetValueInGridByColRow(6).ShouldBeEqual(prvName,
                    "Full Facility Name/ Provider name for given claim seq is present and should be equal to " +
                    prvName);
                /* sorting covered in US68301 */
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test, Category("AppealDependent")] //CAR-1395(CAR-836)
        public void Verify_appeal_badge_icon_in_primary_and_appeal_status_in_secondary_detail()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimNo", "Value");

                _claimSearch.SearchByClaimNmber(claimNo);
                _claimSearch.ClickOnFilterOptionListRow(12);
                Convert.ToInt32(_claimSearch.GetGridViewSection.GetValueInGridByColRow(13, 4))
                    .ShouldBeEqual(1, "Verify Appeal Count for having only one appeal");
                var appealCount = Convert.ToInt32(_claimSearch.GetGridViewSection.GetValueInGridByColRow(13, 5));
                appealCount.ShouldBeGreater(1, "Is Appeal Badge Icon Present for having appeal greater than one?");

                _claimSearch.GetGridViewSection.GetToolTipInGridByColRow(13, 5)
                    .ShouldBeEqual(Format("This claim has {0} appeal(s).", appealCount),
                        "Verify Tooltip message of appeal badge icon");

                _claimSearch.GetGridViewSection.GetColorInGridByColRow(13, 5)
                    .AssertIsContained("rgb(194, 194, 194)", "Is Appeal Badge Icon in Grey Color");

                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(5);
                newClaimAction.ClickOnViewAppealIcon();
                var status = newClaimAction.GetListOfAppealStatusInRows();
                _claimSearch = newClaimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnClaimSearchResultByRow(5);
                _claimSearch.GetClaimDetailsValueByLabel("Appeal Status:")
                    .ShouldBeEqual(status[0], "Verify Appeal Status should be recent one for multiple Appeals");
                _claimSearch.ClickOnClaimSearchResultByRow(2);
                _claimSearch.GetClaimDetailsValueByLabel("Appeal Status:")
                    .ShouldBeEqual("None", "Verify Appeal Status for not having appeal");

            }
        }

        [Test] //TE-527 +  TE-672 +TE-795
        public void Validate_export_icon_and_exported_Excel_Value_For_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Claim_export_headers").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var sheetname = parameterList["sheetName"];
                string lob = parameterList["LoB"];
                var expectedDataList =
                    _claimSearch.GetExcelDataListForClient(automatedBase.EnvironmentManager.Username);
                var fileName = "";
                try
                {

                    _claimSearch.IsExportIconPresent().ShouldBeTrue("Export Icon Present?");
                    _claimSearch.IsExportIconDisabled().ShouldBeTrue("Is Export Icon disabled?");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "Flagged Claims");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                        "Line Of Business",
                        lob);
                    _claimSearch.GetSideBarPanelSearch.ClickOnHeader();
                    _claimSearch.ClickOnFindButton();
                    _claimSearch.IsExportIconEnabled().ShouldBeTrue("Is Export Icon enabled?");

                    _claimSearch.ClickOnExportIcon();
                    _claimSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Confirmation Model Displayed after clicking on export?");

                    StringFormatter.PrintMessage("verify on clicking cancel in confirmation model , nothing happens");
                    _claimSearch.ClickOkCancelOnConfirmationModal(false);
                    _claimSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Is Confirmation model displayed after clicking cancel?");

                    StringFormatter.PrintMessage("verify export of claim search");
                    _claimSearch.ClickOnExportIcon();
                    _claimSearch.ClickOkCancelOnConfirmationModal(true);
                    _claimSearch.WaitForStaticTime(3000);

                    fileName = _claimSearch.GoToDownloadPageAndGetFileName();

                    ExcelReader.ReadExcelSheetValue(fileName, sheetname, 3, 3, out List<string> headerList,
                        out List<List<string>> excelExportList, out string clientname, true);

                    StringFormatter.PrintMessage("verify client name and header values");

                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                    clientname.Trim().ShouldBeEqual(Extensions.GetStringValue(ClientEnum.SMTST));
                    StringFormatter.PrintMessage("verify values correct?");
                    for (int i = 0; i < expectedDataList.Count - 1; i++)
                    {
                        excelExportList[i][0].ShouldBeEqual(expectedDataList[i][1],
                            "Correct Claim Sequence values should be exported");
                        excelExportList[i][1].ShouldBeEqual(expectedDataList[i][2],
                            "Correct Claim Number values should be exported");
                        excelExportList[i][2].ShouldBeEqual(expectedDataList[i][3],
                            "Correct Received Date values should be exported");
                        excelExportList[i][3].ShouldBeEqual(expectedDataList[i][4],
                            "Correct Form Type values should be exported");
                        excelExportList[i][4].ShouldBeEqual(expectedDataList[i][5],
                            "Correct Provider Name values should be exported");
                        excelExportList[i][5].ShouldBeEqual(expectedDataList[i][6],
                            "Correct Provider Sequence values should be exported");
                        excelExportList[i][6].ShouldBeEqual(expectedDataList[i][7],
                            "CorrectMember Id values should be exported");
                        excelExportList[i][7].ShouldBeEqual(expectedDataList[i][8],
                            "Correct Flag values should be exported");
                        excelExportList[i][7].IsFlagInCorrectFormat().ShouldBeTrue("Flags in correct format?");
                        excelExportList[i][8].ShouldBeEqual(double.Parse(expectedDataList[i][9]).ToString(),
                            "Correct Savings values should be exported");
                        excelExportList[i][9].ShouldBeEqual(expectedDataList[i][10],
                            "Correct Batch ID values should be exported");
                        excelExportList[i][10].ShouldBeEqual(expectedDataList[i][11],
                            "Correct Line Of Business values should be exported");
                        excelExportList[i][11].ShouldBeEqual(expectedDataList[i][12],
                            "Correct Claim Status values should be exported");



                    }

                    _claimSearch.GetClaimExportAuditListFromDB(automatedBase.EnvironmentManager.Username)
                        .ShouldContain("/api/clients/SMTST/ClaimSearchResults/DownloadXLS/",
                            "Claim search download audit present?");
                }
                finally
                {
                    _claimSearch.CloseAnyPopupIfExist();
                    if (!IsNullOrEmpty(fileName))
                        ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);
                }
            }
        }

        [Test, Category("Working")] //TE-820
        public void Verify_Validation_Message_When_Searched_Using_Only_Clasub()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _claimSearch.SetClaimSequenceInFindClaimSection(paramLists["Clasub"]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("error pop up present?");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated with ClaSub only.");
                _claimSearch.ClosePageError();

            }
        }

        [Test] //TE-986
        [Order(2)]
        public void Verify_Flag_status_In_Secondary_Details()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseq = paramLists["claseq"];
                var noFlagClaseq = paramLists["NoFlagClaseq"];
                var claimNumber = paramLists["ClaimNumber"];
                var noFlagClaseqClaimNo = paramLists["NoFlagClaimNo"];
                _claimSearch.GetCommonSql.GetAllActiveProductsAbbrvForClient(ClientEnum.SMTST.ToString())
                    .ShouldContain(ProductEnum.COB.ToString(), "Is COB product active for the client ?");
                _claimSearch.ResetFlagStatusOfClaim(claseq, true, false, true);
                _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                    ProductEnum.COB.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnClaimSearchResultByClaseq(claimNumber);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForClientUser(claseq.Split('-')[0], claseq.Split('-')[1]),
                    "correct status displayed when COB Flags are Unreviewed");
                var claimAction = _claimSearch.ClickOnClaimSequenceByClaimSequence(claimNumber);
                claimAction.ClickOnApproveButton();
                if(!_claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    claimAction.ClickWorkListIcon();
                claimAction.ClickSearchIcon();
                claimAction.GetSideBarPanelSearch.ClickOnClearLink();
                claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                claimAction.SearchByClaimSequence(claseq);
                claimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnClaimSearchResultByClaseq(claimNumber);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForClientUser(claseq.Split('-')[0], claseq.Split('-')[1]),
                    "correct status displayed when COB Flags are reviewed");

                _claimSearch.SearchByClaimSequence(noFlagClaseq);
                claimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnClaimSearchResultByClaseq(noFlagClaseqClaimNo);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForClientUser(noFlagClaseq.Split('-')[0], noFlagClaseq.Split('-')[1]),
                    "correct status displayed when COB Flags are reviewed");

            }
        }


        #endregion

        #region PRIVATE METHODS

        private void ValidateClaimSearchRowSorted(ClaimSearchPage _claimSearch, int col, int sortOptionRow, string colName)
        {
            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
            switch (colName)
            {
                case "Client Received Date":
                    _claimSearch.IsListDateSortedInAscendingOrder(col)
                   .ShouldBeTrue( Format("{0} Should sorted in Ascending Order", colName));
                    _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _claimSearch.IsListDateSortedInDescendingOrder(col)
                        .ShouldBeTrue( Format("{0} Should sorted in Descending Order", colName));
                    break;

                case "Savings":
                    _claimSearch.IsListInAscendingOrderBySavings()
                   .ShouldBeTrue( Format("{0} Should sorted in Ascending Order", colName));
                    _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _claimSearch.IsListInADecendingOrderBySavings()
                        .ShouldBeTrue( Format("{0} Should sorted in Descending Order", colName));
                    break;
                case "Claim Sequence":
                    _claimSearch.IsListClaimSeqSortedInAscendingOrder(col)
                        .ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                    _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _claimSearch.IsListClaimSeqSortedInDescendingOrder(col)
                        .ShouldBeTrue(Format("{0} Should sorted in Descending Order", colName));
                    break;
                default:
                    _claimSearch.IsListStringSortedInAscendingOrder(col)
                   .ShouldBeTrue( Format("{0} Should sorted in Ascending Order", colName));
                    _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _claimSearch.IsListStringSortedInDescendingOrder(col)
                        .ShouldBeTrue( Format("{0} Should sorted in Descending Order", colName));
                    break;


            }            
        }
        private void Verify_correct_search_filter_options_displayed_for(AutomatedBaseClientParallel automatedBase,
            ClaimSearchPage _claimSearch,
            string mappingQuickSearchOptionName, string quickSearchValue)
        {

            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
            _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                    true);

        }

        private void ValidateClaimStatusforUnreviewedClaims(AutomatedBaseClientParallel automatedBase, ClaimSearchPage _claimSearch, string label)
        {
            var expectedClaimStatusforUnreviewedClaims = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "unreviewed_claims_status").Values.ToList();
            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");

            actualDropDownList.ShouldCollectionBeEqual(expectedClaimStatusforUnreviewedClaims, "Claim Status for Unreviewed Claims match");
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status")
                .ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(), "Claim Status");

        }

        private void ValidateDropdownListWithMappingData(AutomatedBaseClientParallel automatedBase,
            ClaimSearchPage _claimSearch, string label,
            string mappingOptionName)
        {
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            var expectedMappingList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingOptionName).Values.ToList();
            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            actualDropDownList.ShouldCollectionBeEqual(expectedMappingList, (label + " List match with" + mappingOptionName));
        }

        private void ValidateFlagForFlaggedAndUnreviewedClaims(ClaimSearchPage _claimSearch, string label)
        {
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");

            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);

            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
            VerifyFlagListBasedOnProduct(_claimSearch);
        }

        private void ValidateBatchIdListWithDatabase(ClaimSearchPage _claimSearch, string label)
        {
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            var expectedList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
            expectedList.Insert(0, "All");
            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.ShouldCollectionBeEqual(expectedList, ("Batch Id List should equal to database value"));
        }

        private void ValidateMultipleDropDownForDefaultValueAndExpectedList(ClaimSearchPage _claimSearch, string label,
            IList<string> collectionToEqual)
        {
            var listedOptionsList = _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
            listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            _claimSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
              .ShouldBeTrue(
               "A value of all displayed at the top of the list");
           


            listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected , followed by options sorted alphabetically.");

            listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
        }
        private void ValidateSingleDropDownForDefaultValueAndExpectedList(ClaimSearchPage _claimSearch, string label,
            IList<string> collectionToEqual, bool order = true)
        {
            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
        }

        private void ValidateAssignedTo(ClaimSearchPage _claimSearch, string label)
        {
            var _claimsAssignedToUserList = _claimSearch.GetClientAssignedToList(ClientEnum.SMTST.ToString());
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            var reqDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            
            reqDropDownList.Remove("All");
            _claimsAssignedToUserList.Sort();
            reqDropDownList.ShouldCollectionBeEqual(_claimsAssignedToUserList, "Validate Assigned To List with the database");
            reqDropDownList[0].DoesNameContainsOnlyFirstWithLastname()
                .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");

        }

        private void ValidateFieldSupportingMultipleValues(ClaimSearchPage _claimSearch, string label,
            IList<string> expectedDropDownList)
        {
            ValidateMultipleDropDownForDefaultValueAndExpectedList(_claimSearch, label, expectedDropDownList);
            _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
            _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
            _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
            _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
            _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");
        }

        private List<string> GetExpectedProductListFromDatabase(ClaimSearchPage _claimSearch,
            List<string> allProductList)
        {
            List<string> expectedProductTypeList = new List<string>();
            var _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());

            for (int i = 0; i < _activeProductListForClient.Count; i++)
            {
                if (_activeProductListForClient[i] == "T")
                {
                    expectedProductTypeList.Add(allProductList[i]);
                }
            }
            //if (_activeProductListForClient[0] == "T")
            //{
            //    expectedProductTypeList.Add("Coordination of Benefits");
            //}
            //if (_activeProductListForClient[1] == "T")
            //{
            //    expectedProductTypeList.Add("Dental Claim Accuracy");
            //}

            //if (_activeProductListForClient[2] == "T")
            //{
            //    expectedProductTypeList.Add("FacilityClaim Insight");
            //}

            //if (_activeProductListForClient[3] == "T")
            //{
            //    expectedProductTypeList.Add("FraudFinder Pro");
            //}

            //if (_activeProductListForClient[4] == "T")
            //{
            //    expectedProductTypeList.Add("Negotiation");
            //}

            //if (_activeProductListForClient[5] == "T")
            //{
            //    expectedProductTypeList.Add("PharmaClaim Insight");
            //}

            //if (_activeProductListForClient[6] == "T")
            //{
            //    expectedProductTypeList.Add("Coding Validation");
            //}

            expectedProductTypeList.Insert(0, "All");
            return expectedProductTypeList;
        }

        private void VerifyFlagListBasedOnProduct(ClaimSearchPage _claimSearch)
        {
            var productListAbbreviation = GetExpectedProductListFromDatabase(_claimSearch, _claimSearch.GetCommonSql.GetAllProductList(true));
            var _allProductList = _claimSearch.GetCommonSql.GetAllProductList();
            var productList = GetExpectedProductListFromDatabase(_claimSearch, _allProductList);
            var i = 0;
            foreach (var product in productListAbbreviation)
            {
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", productList[i]);
                var flagList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Flag");
                flagList.RemoveAt(0);
                if (product=="All")
                {
                    flagList.ShouldCollectionBeEqual(_claimSearch.GetAllFlagsFromDatabase(),"Flag List Should Match when All is selected");
                }
                else
                {
                    Enum.TryParse(product, out ProductEnum productName);
                    flagList.ShouldCollectionBeEqual(_claimSearch.GetActiveFlagsForProductFromDatabase(productName.GetStringDisplayValue()), "Flag List Should Match");
                }
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                i++;
            }
        }

        private void ValidateClaimStatusforFlaggedClaims(AutomatedBaseClientParallel automatedBase,
            ClaimSearchPage _claimSearch, string label)
        {
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            var expectedClaimStatusforFlaggedClaims = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "flagged_claims_status").Values.ToList();
            var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);

            actualDropDownList.ShouldCollectionBeEqual(expectedClaimStatusforFlaggedClaims, "Claim Status for Unreviewed Claims match");
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");        

        }

        private void ValidateBatchIDforFlaggedClaims(ClaimSearchPage _claimSearch, string label)
        {
            var expecteBatchIDList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
            var actualBatchIDList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualBatchIDList.ShouldCollectionBeEqual(expecteBatchIDList, "Batch Id List Shoul Equal");
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID")
                .ShouldBeEqual(expecteBatchIDList[0], ("The default " + label + "should be" + expecteBatchIDList[0]));
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualBatchIDList[1], false); //check for type ahead functionality
            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualBatchIDList[0]);
            _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualBatchIDList[0], "User can select only a single option");
        }
        #endregion
    }
}
