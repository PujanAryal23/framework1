using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Data;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class WorklistClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        //private ProfileManagerPage _profileManager;
        private Dictionary<string, string> _workList;
        //private string _defaultUser;
        private ClaimActionPage _claimAction;
        private ClaimSearchPage _claimSearch;

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
            base.ClassInit();
            _workList = DataHelper.GetTestData(FullyQualifiedClassName, "WorkListValue");
        }

        protected override void TestCleanUp()
        {
            if (!CurrentPage.CurrentPageTitle.Equals(PageTitleEnum.QuickLaunch.GetStringValue()))
            {
                CurrentPage = QuickLaunch = _claimAction.ClickOnQuickLaunch();
                Console.Out.WriteLine("Back to QuickLaunch page");
            }
            base.TestCleanUp();

        }

        #endregion

        #region TEST SUITES

     
        

        [Test] //CAR-425(CAR-496)
        public void Verification_of_Claim_View_Restriction_Filter()
        {
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
            var restrictionList =
                _claimAction.GetSideBarPanelSearch.GetAvailableDropDownList("Claim Restrictions");
            var restrictionListDB = _claimAction.GetRestrictionsListFromDB();

            StringFormatter.PrintMessage("Verifying Restriction Values from Database");
            _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions")
                .ShouldBeEqual("All",
                    "Claim Restrictions default value should be All");
            restrictionListDB.Insert(0, "No Restriction");
            restrictionListDB.Insert(0, "All");
            restrictionList.ShouldCollectionBeEquivalent(restrictionListDB,
                "Restriction Values present in Database should be populated in filter option");

        }

        [Test]
        public void Verify_client_user_PCI_worklist_includes_filters_ClaimStatus_Plan_ReviewGroup_Claim_Type_Batch_ID()
        {
             TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            _claimAction.ClickWorkListIcon();
            _claimAction.GetWorkListFiltersLabel().ShouldCollectionBeEqual(DataHelper.GetMappingData(FullyQualifiedClassName, "work_list_filters_for_client_user").Values.ToList(), "WorkList Filters ");
            _claimAction.ClickOnQuickLaunch();
        }

       // [Test]
        public void Veriy_Review_Group_filter_allows_users_to_enter_special_characters()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var specialCharacter = new[] { '/', ',', '.', '*', '-' };
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            _claimAction.ClickWorkListIcon();
            StringFormatter.PrintMessageTitle("Review Group filter allows users to enter special characters and display the results");
            foreach (char text in specialCharacter)
            {
                string actualResult = _claimAction.GetValueByEntering(text);
                if (!actualResult.Contains(text))
                {
                    actualResult.ShouldBeEqual("No results found", "Results for " + text);
                }
                else
                    actualResult.AssertIsContained(text, "Results for :" + text);
            }
            StringFormatter.PrintLineBreak();
            _claimAction.ClickOnQuickLaunch();
        }


        [Test, Category("SmokeTest")]
        public void Verify_that_claims_returned_In_Worklist_shows_correct_status()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimStatus1 = paramLists["ClaimStatus1"];
            string claimStatus2 = paramLists["ClaimStatus2"];
            string claimSubStatus = paramLists["ClaimStatus2SubStatus"];
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            _claimAction.ClickWorkListIcon();
            _claimAction.SelectClaimStatus("Unreviewed");
            _claimAction.ClickOnCreateButton();
            _claimAction.GetClaimStatus().ShouldBeEqual(claimStatus1, "Claim Status");
            _claimAction.ClickWorkListIcon();
            _claimAction.SelectClaimStatus(claimStatus2);
            _claimAction.SelectClaimSubStatus(claimSubStatus);
            _claimAction.ClickOnCreateButton();
            _claimAction.GetClaimStatus().ShouldBeEqual(claimSubStatus, "Claim Status");
            }
 



        //[Test]//US13853
        //public void Verify_that_for_PCI_flag_only_Cotiviti_user_approved_flag_will_appear_in_the_client_pci_workList()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
        //    string expectedClaimSequence = paramLists["CotivitiUserApprovedClaim"];
        //    _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
        //    IList<string> pciworkList = paramLists["PCIWorkListClient"].Split(';');
        //    expectedClaimSequence.AssertIsContainedInList(pciworkList, string.Format("ClaimSequence {0} is contained in the pci worklist", expectedClaimSequence));

        //}

        //[Test]//US13852
        //public void Verify_that_for_PCI_flag_only_Cotiviti_user_deleted_flag_will_NOT_appear_in_the_client_pci_workList()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
        //    string CotivitiuserDeletedClaim = paramLists["CotivitiUserDeletedClaim"];
        //    _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
        //    IList<string> pciworkList = paramLists["PCIWorkListClient"].Split(';');
        //    pciworkList.Contains(CotivitiuserDeletedClaim)
        //       .ShouldBeFalse(string.Format("The ClaimSequence {0} appears in the pci worklist", CotivitiuserDeletedClaim));

        //}

        //[Test]//US13854
        //public void Verify_that_system_deleted_pci_flag_does_NOT_appear_in_the_client_pci_worklist()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
        //    string claimSequenceSystemDeleted = paramLists["ClaimSequenceSystemDeleted"];
        //    _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
        //    IList<string> pciworkList = paramLists["PCIWorkListClient"].Split(';');
        //    pciworkList.Contains(claimSequenceSystemDeleted)
        //         .ShouldBeFalse(string.Format("The ClaimSequence {0} appears in the pci worklist", claimSequenceSystemDeleted));

        //}

        [Test]//TE-451
        public void Verify_ReviewGroup_Filter_For_PCI_and_FFP_WorkList()
        { 
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            var expectedReviewGroupList = _claimAction.GetReviewGroup(ClientEnum.SMTST.ToString());
            _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status","Pended");
            _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder").ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
            StringFormatter.PrintMessage("Verify Review Group ");
            ValidateFieldSupportingMultipleValues("Review Group", expectedReviewGroupList);
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status","Unreviewed");
            _claimAction.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Review Group")
                .ShouldBeTrue("Is Review Group dropdown displayed for Unreviewed Claim?");
            _claimAction = _claimAction.CreatePciWorklistWithReviewGroups(expectedReviewGroupList);
            _claimAction.WaitForStaticTime(2000);
            _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
            _claimAction.ClickOnFFPWorkList();
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", "Unreviewed");
            _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder").ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
            StringFormatter.PrintMessage("Verify Review Group ");
            ValidateFieldSupportingMultipleValues("Review Group", expectedReviewGroupList);
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", "Pended");
            _claimAction.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Review Group")
                .ShouldBeTrue("Is Review Group dropdown displayed for Pended Claim");
            _claimAction.GetSideBarPanelSearch.ClickOnClearLink();
            _claimAction.CreateFFPWorklistWithReviewGroups(expectedReviewGroupList);
            _claimAction.GetSideBarPanelSearch.ClickOnClearLink();

        }

        #endregion
        #region PRIVATE METHODS

        private void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label,
            IList<string> collectionToEqual)
        {
            var listedOptionsList = _claimAction.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
            listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            _claimAction.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                .ShouldBeTrue(
                    "A value of all displayed at the top of the list");
        }

        private void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList)
        {
            ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList);
            _claimAction.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
            _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
            _claimAction.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
            _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
            _claimAction.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");
        }
#endregion
    }
}
