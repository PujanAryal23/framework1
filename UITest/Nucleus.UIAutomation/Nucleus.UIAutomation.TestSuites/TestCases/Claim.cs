using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Data;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Extensions = UIAutomation.Framework.Utils.Extensions;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class WorkList : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private ProfileManagerPage _profileManager;
        private Dictionary<string, string> _workList;
        private string _defaultUser;
        private string _claimSequence;
        private ClaimActionPage _claimAction;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _expectedFLagList;
        private FlagPopupPage flagPopup;
        private ClaimSearchPage _claimSearch;
        private UserProfileSearchPage _newUserProfileSearch;
       
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
                
                UserLoginIndex = 7;
                base.ClassInit();
                _defaultUser = EnvironmentManager.HciAdminUsernameClaim5;
                _workList = DataHelper.GetTestData(FullyQualifiedClassName, "WorkListValue");
                _claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                _claimSearch = QuickLaunch.NavigateToClaimSearch();
                _claimAction= _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();

                try
                {
                    AssignedExpectedList();
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
            string defaultUser = EnvironmentManager.HciAdminUsernameClaim5;
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMINCLAIM5, StringComparison.OrdinalIgnoreCase) != 0)
            {
                Login = CurrentPage.Logout();
                CurrentPage = QuickLaunch = Login.LoginAsHciAdminUserClaim5();
                _claimAction = QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
                _defaultUser = defaultUser;
            }

            if (CurrentPage.GetPageHeader().Equals(Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch)))
            {
                _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
            }
            else if (!CurrentPage.GetPageHeader().Equals(Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction)))
            {

                _claimAction.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
                Console.Out.WriteLine("Back to New Claim Action page");
            }
            else if (_claimAction.IsWorkingAjaxMessagePresent())
            {
                _claimAction.Refresh();
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

            }
            if (!_claimAction.IsWorkListControlDisplayed())
                _claimAction.ClickWorkListIcon();
            if (_claimAction.GetWorkListHeaderText() != "CV Work List")
                _claimAction.ToggleWorkListToPci();
            
            _claimAction.ClickOnClearOnWorkListPanel();
            base.TestCleanUp();

        }
        protected override void ClassCleanUp()
        {
            try
            {
                _claimAction.CloseDatabaseConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        #endregion

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _claimAction.GetAllCotivitiExpectedLists();
            _expectedFLagList = _allCotivitiExpectedLists[0];
        }


        #region TEST SUITES

       
        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_Navigation_to_appeal_creator_page_from_worklist()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string claimSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSeq", "Value");
                SearchByClaimSeqFromWorkList(claimSeq);
                _claimAction.IsAddAppealIconEnabled().ShouldBeTrue("Is create Appeal icon enabled?");
                var appealCreator = _claimAction.ClickOnCreateAppealIcon();
                appealCreator.GetPageHeader().ShouldBeEqual(Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.AppealCreator),
                    "Redirected to the 'Appeal Creator' page when 'Create Appeal' icon is clicked ");

                appealCreator.SelectClaimLine();

                appealCreator.ClearProduct();
                appealCreator.SetCreateAppealFormInputValueGeneric("1", "Phone Number");
                appealCreator.SetCreateAppealFormInputValueGeneric("","Name");
                appealCreator.SetCreateAppealFormInputValueGeneric("","Email Address");

                appealCreator.ClickOnSaveBtn();
                appealCreator.ClosePageError();

                appealCreator.IsInvalidInputPresentByLabel("Product").ShouldBeTrue("");
                appealCreator.IsInvalidInputPresentByLabel("Name").ShouldBeTrue("");
                appealCreator.IsInvalidInputPresentByLabel("Phone Number").ShouldBeTrue("");
                appealCreator.IsInvalidInputPresentByLabel("Email Address").ShouldBeTrue("");

                appealCreator.ClickOnCancelBtn();
                if(appealCreator.IsPageErrorPopupModalPresent())
                    appealCreator.ClickOkCancelOnConfirmationModal(true);

                CurrentPage.GetPageHeader().ShouldBeEqual(Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction),
                    "Page header text equals Claim Action");
            }
            finally
            {
                _claimAction.ClickClaimSearchIcon().RefreshPage(true);
                    _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_note_indicator_from_worklist_and_search_icon()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSeq = paramLists["ClaimSeq"];
            SearchByClaimSeqFromWorkList(claimSeq);
            _claimAction.IsAddNoteIndicatorPresent().ShouldBeTrue("Is Note Icon present?");
            _claimAction.ClickOnClaimNotes();
            _claimAction.IsAddNoteFormPresent().ShouldBeTrue("Is Note form present?");
            _claimAction.ClickOnClaimNotes();
            _claimAction.IsAddNoteFormPresent().ShouldBeFalse("Is Note form present?");

            _claimAction.ClickClaimSearchIcon();
            CurrentPage.GetPageHeader().ShouldBeEqual(Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch));

        }

        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_upper_right_quadrant_icons_from_worklist()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSeq = paramLists["ClaimSeq"];
            SearchByClaimSeqFromWorkList(claimSeq);
            
            _claimAction.ClickOnViewDxCodesIcon();
            _claimAction.GetLeftHeaderOfQ2Quadrant().ShouldBeEqual("Dx Codes", "DX Codes are displayed");
            _claimAction.ClickOnViewAppealIcon();
            _claimAction.GetLeftHeaderOfQ2Quadrant().ShouldBeEqual("Appeals", "Appeals are displayed");

            if (_claimAction.IsLocked() && !_claimAction.GetLockIConTooltip().Contains("locked by"))
            {
                var lockMessage = _claimAction.GetLockIConTooltip();
                StringFormatter.PrintMessage(
                    $"Claim is locked. Locking the claim by navigating back to the claim action page again.\nLock tooltip message is : \n {lockMessage}");
                var _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                
                _claimAction.ClickOnClaimDocuments();
                _claimAction.GetLeftHeaderOfQ2Quadrant()
                    .ShouldBeEqual("Claim Documents", "Claim Documents are displayed", true);
                _claimAction.IsUploadDocumentContainerPresent()
                    .ShouldBeTrue("Is 'UpUpload Claim Document' form present?");

                _claimAction.ClickonProviderDetailsIcon();
                _claimAction.GetLeftHeaderOfQ2Quadrant()
                    .ShouldBeEqual("Provider Details", "Provider Details are displayed");
                _claimAction.GetCiuReferralRecordRowCount()
                    .ShouldBeGreater(0, "Are associated CIU Referrals being shown?");
                _claimAction.ClickonAddCiuReferralIcon();
                _claimAction.IsCreateCiuReferralSectionDisplayed()
                    .ShouldBeTrue("Is Create CIU referral form displayed?");
            }

            else
            {
                StringFormatter.PrintMessageTitle("Could not proceed with the test since the claim was locked by another user");
            }
        }

       
        [Test,Category("SmokeTestDeployment")]
        public void Verify_lower_right_quadrant_icons_from_worklist() //TANT-82
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSeq = paramLists["ClaimSeq"];
            SearchByClaimSeqFromWorkList(claimSeq);
            _claimAction.ClickOnDollarIcon();
            _claimAction.GetValueOfLowerRightQuadrant().ShouldBeEqual("Claim Dollar Details","Claim Dollar Details Are Displayed");

            _claimAction.ClickOnClaimFlagAuditHistoryIcon();
            _claimAction.GetValueOfLowerRightQuadrant()
                .ShouldBeEqual("Claim Flag Audit History", "Claim Flag Audit History Are Displayed");
            _claimAction.ClickOnEditNotesIcon();
            _claimAction.IsEditNoteAreaPresent().ShouldBeTrue("Is Edit form expanded ?");

            _claimAction.ClickOnLinesIcon();
            _claimAction.GetValueOfLowerRightQuadrant()
                .ShouldBeEqual("Claim Lines", "Claim Lines Are Displayed");
            _claimAction.ClickOnEditRecord();
            _claimAction.IsEditAreaPresent().ShouldBeTrue("Is Edit form expanded ?");

            
            _claimAction.ClickOnFlagDetailsSection();
            _claimAction.GetValueOfLowerRightQuadrant().ShouldBeEqual("Flag Details", "Label of Lower Right Quadrant");

            _claimAction.ClickOnLineDetailsSection();
            _claimAction.GetValueOfLowerRightQuadrant().ShouldBeEqual("Line Details", "Label of Lower Right Quadrant");



        }


        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_more_option_list_and_different_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSeq = paramLists["ClaimSeq"];
            var moreOptionList = DataHelper.GetMappingData(FullyQualifiedClassName, "More_Option_List").Values.ToList();

            SearchByClaimSeqFromWorkList(claimSeq);
            _claimAction.ClickMoreOption();
            _claimAction.MoreOptionList()
                .ShouldCollectionBeEqual(moreOptionList, "More Option list should be equal");

            StringFormatter.PrintMessage("Verify Original Claim Data");
            _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeTrue("Is 'Original Claim Data' link present?");
            _claimAction.ClickOnOriginalClaimDataAndSwitch();
            _claimAction.IsOriginalClaimDataOpen().ShouldBeTrue("Is Original Claim Data popup displayed");
            _claimAction.CloseAllExistingPopupIfExist();

            StringFormatter.PrintMessage("Verify Claim Processing History");
            //_claimAction.ClickMoreOption();
            _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
            _claimAction.IsClaimProcessingHistoryOpen().ShouldBeTrue("Is Claim Processing History popup displayed?");
            _claimAction.CloseAllExistingPopupIfExist();

            StringFormatter.PrintMessage("Verify Invoice Data Popup");
            _claimAction.ClickMoreOption();
            _claimAction.ClickOnInvoiceDataLink();
            _claimAction.IsInvoiceDataLinkOpened().ShouldBeTrue("Is Invoice Data popup displayed?");
            _claimAction.CloseAllExistingPopupIfExist();
        }

        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_that_new_window_is_opened_when_clicked_on_different_flag()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSeq = paramLists["ClaimSeq"];
            var flagList = paramLists["Flags"].Split(';').ToList();
            var flag1 = flagList[0];
            var flag2 = flagList[1];
            IList<string> popups = new List<string>();
            int countWindowHandles = 0;
            SearchByClaimSeqFromWorkList(claimSeq);
            try
            {
                flagPopup = _claimAction.ClickOnFlagandSwitch("Flag Information - "+ flag1, flag1);
                popups.Add(flagList[0]);
                countWindowHandles += flagPopup.CountWindowHandles(flagPopup.PageTitle);
                _claimAction = flagPopup.SwitchBackToNewClaimActionPage();
                flagPopup = _claimAction.ClickOnFlagandSwitch("Flag Information - " + flag2, flag2);
                popups.Add(flagList[1]);
                countWindowHandles += flagPopup.CountWindowHandles(flagPopup.PageTitle);
                countWindowHandles.ShouldBeGreater(1, string.Format("{0} windows opened when clicked on {0} different flags", countWindowHandles));
            }
            finally
            {
                foreach (var popup in popups)
                {
                    _claimAction = flagPopup.ClosePopupOnNewClaimActionPage(popup);
                }
            }
        }
        [Test, Category("SmokeTestDeployment")]//TANT-82
        public void Verify_Patient_Claim_History_changes_with_periods()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var patientClaimHistoryOptionList = DataHelper.GetMappingData(FullyQualifiedClassName, "patient_claim_history_option_list").Values.ToList();
            
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            var claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
            claimHistory.GetHistoryOptionList().ShouldCollectionBeEqual(patientClaimHistoryOptionList,"Provider History options should be equal.");
            _claimAction.IsProviderHistoryPopUpOpen().ShouldBeTrue("Is Patient Claim history popup opened?");
            foreach (var tab in patientClaimHistoryOptionList)
            {
                claimHistory.ClickOnHistoryTabs(tab);
                claimHistory.IsHistoryTabSelectedByTabName(tab).ShouldBeTrue(string.Format("Is {0} tab selected?",tab));
            }
            
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }




        [Test, Category("SmokeTest"), Category("IENonCompatible")]
        public void Verify_that_user_with_CV_QC_authority_can_view_CV_QC_audit_worklist_in_work_list_panel_And_contains_filters_plan_claim_type_flag_batch_id()
        {
            try
            {
                CurrentPage =  _claimAction.NavigateToCvQcWorkList();
                bool isPatientClaimHistoryOpened = _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                if (isPatientClaimHistoryOpened)
                    _claimAction.ClickWorkListIcon();

                _claimAction.GetWorkListHeaderText().ShouldBeEqual("CV QC Work List", "WorkList Header");
                _claimAction.ClickonWorkListToggler()
                    .IsCvQcClaimsPresentInWorkListPanel()
                    .ShouldBeTrue("CV QC WorkList is present");
                _claimAction.ClickonWorkListToggler();
                _claimAction.GetWorkListFiltersLabel()
                    .ShouldCollectionBeEqual(
                        DataHelper.GetMappingData(FullyQualifiedClassName, "work_list_filters_for_cvqc")
                            .Values.ToList(), "WorkList Filters ");
               
            }
            finally
            {
                _claimAction.ClickClaimSearchIcon();
                if (_claimAction.IsWorkListOptionPresent())
                    _claimAction.ClickonWorkListToggler();
                _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test]
        public void Verify_that_filter_retains_value_when_user_switches_from_a_work_list_to_Find_Claim_and_back_to_original_Work_List()
        {
            try
            {
                _claimAction.ClickClaimSearchIcon().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
                _claimAction.SelectClaimStatus("Pended");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID", "LoadTestBatch");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Type", "H");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", "AADD");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Specialty", "02 - General Surgery");
                _claimAction.ClickSearchIcon();
                _claimAction.ClickWorkListIconWithinWorkList();
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status").ShouldBeEqual("Pended", "Claim Status");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type").ShouldBeEqual("H", "Claim Type");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("LoadTestBatch", "Batch ID");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Flag").ShouldBeEqual("AADD", "Flag");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Specialty")
                    .ShouldBeEqual("02 - General Surgery", "Specialty");
            }
            finally
            {
                _claimAction.ClickOnClearOnWorkListPanel();
            }
        }

        [Test, Category("SmokeTest"), Category("WorkList1")]
        public void Verify_that_filters_are_reset_when_user_switches_to_another_work_list_and_then_back_to_original_Work_List()
        {
            try
            {
                _claimAction.SelectClaimStatus("Pended");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID" ,"LoadTestBatch");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Type", "H");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", "AADD");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Specialty", "02 - General Surgery");
                _claimAction.ToggleWorkListToFci();
                _claimAction.ToggleWorkListToPci();
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status").ShouldBeEqual("Unreviewed", "Claim Status"); ;
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type").ShouldBeEqual("All", "Claim Type");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("All", "Batch Id"); 
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Flag").ShouldBeEqual("All", "Flag"); 
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Specialty")
                    .ShouldBeEqual("All", "Specialty");
            }
            finally
            {
                if (_claimAction.GetWorkListHeaderText() != "CV Work List")
                    _claimAction.ToggleWorkListToPci();
                _claimAction.ClickOnClearOnWorkListPanel();
            }
        }

        [Test, Category("SmokeTest"), Category("WorkList2")]
        public void Verify_that_user_must_have_read_write_or_read_only_authority_to_PCI_AND_FCI_Worklist_in_order_to_see_the_PCI_AND_FCI_Claims_Work_List_option_under_Claim_menu()
        {
            VerificationForFciAndPciClaimsWorkListOptionUnderClaimMenu(RoleEnum.ClinicalValidationAnalyst, RoleEnum.FCIAnalyst);
        }

        [Test]
        public void Verify_that_user_does_not_assign_with_PCI_AND_FCI_Work_List_authority_should_not_see_the_PCI_AND_FCI_Claims_Work_List_option_under_Claim_menu()
        {
            LoginAsUserHavingNotWorkListOptionUnderClaimMenu();
            var list=CurrentPage.GetAllPrimaryAndSecondarySubMenuListByHeaderMenu(HeaderMenu.Claim);
            list.Contains(SubMenu.CVClaimsWorkList).ShouldBeFalse("Is CV Claim Work List Present?");
            list.Contains(SubMenu.FciClaimsWorkList).ShouldBeFalse("Is FCI Claim Work List Present?");
        }

        //[Test]
        //public void Verify_that_user_must_have_read_write_or_read_only_authority_to_FCI_Worklist_in_order_to_see_the_FCI_Claims_Work_List_option_under_Claim_menu()
        //{
        //    VerificationForFciAndPciClaimsWorkListOptionUnderClaimMenu(RoleEnum.FCIAnalyst);
        //}

        //[Test]
        //public void Verify_that_user_does_not_assign_with_FCI_Work_List_authority_should_not_see_the_FCI_Claims_Work_List_option_under_Claim_menu()
        //{
        //    LoginAsUserHavingNotWorkListOptionUnderClaimMenu();
        //    Mouseover.IsFciClaimsWorkList().ShouldBeFalse("Is FCI Work List Present for not having CV Work List Authority?");
        //}

       

        [Test,Category("SmokeTestDeployment")]
        public void Verify_that_claim_action_button_for_the_worklist_shows_the_worklist_options_and_X_closes_the_worklist()
        {
            try
            {
                _claimAction.IsWorkListControlDisplayed().ShouldBeTrue("Work list control present ?");
                _claimAction.GetWorkListHeaderText().ShouldBeEqual("CV Work List", "Work List Div Header");
                _claimAction.CloseWorkList();
                _claimAction.IsWorkListControlDisplayed().ShouldBeFalse("Work list control closed ?");
            }
            finally
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
                
            }
        }


        [Test, Category("SmokeTest"), Category("WorkList2")]
        public void Verify_that_worklist_can_be_toggled_from_FCI_to_PCI_from_worklist_from_dropdown_menu()
        {
            try
            {

                _claimAction.ToggleWorkListToFci()
                    .GetWorkListHeaderText()
                    .ShouldBeEqual("FCI Work List", "Work List Div Header");
                _claimAction.ToggleWorkListToPci()
                    .GetWorkListHeaderText()
                    .ShouldBeEqual("CV Work List", "Work List Div Header");
            }
            finally
            {
                if (_claimAction.GetWorkListHeaderText() != "CV Work List")
                    _claimAction.ToggleWorkListToPci();
            }

        }

       

        [Test]
        public void Verify_that_find_option_contains_claim_sequence_and_claim_no_input_fields()
        {
            try
            {
                _claimAction.ClickonFindOption();
                _claimAction.IsClaimSequenceLabelDisplayed().ShouldBeTrue("ClaimSequence label present ?");
                _claimAction.IsClaimSequenceInputFieldDisplayed().ShouldBeTrue("ClaimSequence Input Field present ?");
                _claimAction.IsClaimNoLabelDisplayed().ShouldBeTrue("ClaimNo label present ?");
                _claimAction.IsClaimNoInputFieldDisplayed().ShouldBeTrue("ClaimNo Input Field present ?");
            }
            finally
            {
                _claimAction.ClickWorkListIconWithinWorkList();
            }
        }

        [Test, Category("SmokeTest"), Category("WorkList2")]
        public void Verify_work_list_contains_section_called_Next_Claims_in_worklist_Additional_Locked_Claims_and_Previously_viewed_claims()
        {

            _claimAction.IsNextClaimsInWorklistSectionDisplayed().ShouldBeTrue("Next Claims In Worklist section present ?");
            _claimAction.IsAdditionalLockedClaimsSectionDisplayed().ShouldBeTrue("Additional Locked claims section present ?");
            _claimAction.IsPreviouslyViewedClaimsSectionDisplayed().ShouldBeTrue("Previously viewed claims section present ?");
            _claimAction.GetNextClaimsInWorklistSectionHeader().ShouldBeEqual("Next Claims in Work List", "Next Claims section header");
            _claimAction.GetAdditionalLockedClaimsSectionHeader().ShouldBeEqual("Additional Locked Claims", "Additional Locked Claims section header");
            _claimAction.GetPrevioulsyViewedClaimsSectionHeader().ShouldBeEqual("Previously Viewed Claims", "Previously Viewed Claims section header");

        }

        [Test]
        public void Verify_that_if_no_values_are_entered_for_search_page_error_appears()
        {
            try
            {
                _claimAction.IsSearchIconDisplayed().ShouldBeTrue("Find Option present ?");
                _claimAction.ClickonFindOption().SearchByClaimSequence("");
                _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("page error popup message present?");
                _claimAction.GetPageErrorMessage()
                    .ShouldBeEqual("Search cannot be initiated without any criteria entered.", "Error message");
                _claimAction.ClosePageError();
            }
            finally
            {
                if (_claimAction.IsPageErrorPopupModalPresent())
                    _claimAction.ClosePageError();
                _claimAction.ClickWorkListIconWithinWorkList();
                _claimAction.ClickOnClearOnWorkListPanel();
            }

        }

        [Test]
        public void Verify_for_plan_filter_Select_One_or_more_is_default_text_if_user_selects_single_value_then_filter_displays_selected_value()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string plan = paramLists["Plan"];

            try
            {


                //verify default text 
                _claimAction.GetPlanOptionSelectorText()
                    .ShouldBeEqual("Select one or more", "Default plan selector text");
                _claimAction.SelectPlan(plan);
                _claimAction.GetPlanOptionSelectorText().ShouldBeEqual(plan, "Plan selector text");
            }
            finally
            {
                _claimAction.ClickOnClearOnWorkListPanel();
            }
        }


        [Test, Category("SmokeTest"),Category("WorkList1")]
        public void Verify_for_plan_filter_if_user_selects_multiple_values_then_Multiple_values_selected_is_displayed()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string plan1 = paramLists["Plan1"];
            string plan2 = paramLists["Plan2"];
            StringFormatter.PrintLineBreak();
            try
            {
                _claimAction.SelectPlan(plan1).SelectPlan(plan2);
                _claimAction.GetPlanOptionSelectorText().ShouldBeEqual("Multiple values selected", "Plan selector text");
                StringFormatter.PrintLineBreak();

            }
            finally
            {
                _claimAction.ClickOnClearOnWorkListPanel();
            }
        }

        [Test, Category("SmokeTest"), Category("WorkList1")]
        public void Verify_that_filter_criteria_are_retained_if_user_closes_the_work_list_panel()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string plan = paramLists["Plan"];
            string claimStatus = paramLists["ClaimStatus"];
            try
            {
                _claimAction.SelectClaimStatus(claimStatus).CloseWorkList();
                _claimAction.ClickWorkListIcon();
                _claimAction.GetClaimStatusOptionSelectorText().ShouldBeEqual(claimStatus, "Claim Status selector text");
            }
            finally
            {
                _claimAction.ClickOnClearOnWorkListPanel();
            }

        }

        [Test]
        public void Verify_that_for_Cotiviti_user_PCI_worklist_verify_that_editflag_claimtype_and_batchid_should_contain_appropriate_values()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            IDictionary<string, string> expectedClaimTypeOptions = DataHelper.GetMappingData(FullyQualifiedClassName,
                                                                         "claim_type_options_list");
            //IDictionary<string, string> expectedBatchOptions = DataHelper.GetMappingData(FullyQualifiedClassName,
            //                                                             "batch_options_list");
            var expectedBatchOptions=_claimAction.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
            expectedBatchOptions.Insert(0,"All");
            //var expectedFlagOptions = _claimAction.GetExpectedFLagOptions();// DataHelper.GetMappingData(FullyQualifiedClassName,
                                                                        // "flag_options_list");
            _claimAction.GetClaimTypeOptions().ShouldCollectionBeEqual(expectedClaimTypeOptions.Values.ToList(), "CLaim Type Options");
            _claimAction.Wait();
            _claimAction.GetBatchOptions().ShouldCollectionBeEqual(expectedBatchOptions, "Batch Id Options");
            _claimAction.Wait();
            _claimAction.GetFLagOptions().ShouldCollectionBeEqual(_expectedFLagList.ToList(), "Edit Flag options");
        }


        [Test, Category("SmokeTest"), Category("WorkList2")]
        public void Verify_that_if_user_with_CV_QC_Audit_was_not_the_user_who_originally_approved_the_claim_the_green_QA_Review_icon_appear_with_appropriate_tooltip()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];
            try
            {

                //CurrentPage = QuickLaunch = _claimAction.GoToQuickLaunch();
                //_claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
                //_claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                //_claimAction.ClickWorkListIcon();
                _claimAction.ClickonFindOption().SearchByClaimSequence(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsQaAuditIconInReviewMode().ShouldBeTrue("QA Audit Icon green present?");
                _claimAction.GetQaAuditIconTooltip()
                    .ShouldBeEqual("Claim requires CV QC audit.", "Audit Icon tooltip");
            }
            finally
            {
                _claimAction.ClickWorkListIcon();
                _claimAction.ClickWorkListIconWithinWorkList();
                _claimAction.ClickOnClearOnWorkListPanel();
            }

        }


        [Test]
        public void Verify_that_if_user_with_PCI_QA_Audit_was_the_user_who_originally_approved_the_claim_the_red_QA_Review_icon_appear_with_appropriate_tooltip()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];
            try
            {
                _claimAction.ClickonFindOption().SearchByClaimSequence(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsQaAuditIconInLockedMode().ShouldBeTrue("QA Audit Icon red present?");
                _claimAction.GetQaAuditIconTooltip().ShouldBeEqual("Claim requires CV QC audit.  Audit cannot be completed by the same user who approved the claim.", "Audit Icon tooltip");
            }
            finally
            {
                _claimAction.ClickWorkListIcon();
                _claimAction.ClickWorkListIconWithinWorkList();
                _claimAction.ClickOnClearOnWorkListPanel();
            }

        }

        [Test, Category("SmokeTest"), Category("WorkList1")]
        public void Verify_that_claims_returned_In_Worklist_shows_correct_status()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimStatus1 = paramLists["ClaimStatus1"];
            string claimStatus2 = paramLists["ClaimStatus2"];
            string claimSubStatus = paramLists["ClaimStatus2SubStatus"];
            try
            {
                _claimAction.SelectClaimStatus("Unreviewed");
                _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetClaimStatus().ShouldBeEqual(claimStatus1, "Claim Status");
                _claimAction.SelectClaimStatus(claimStatus2);
                _claimAction.ClickWorkListIcon();
                _claimAction.SelectClaimSubStatus(claimSubStatus);
                _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetClaimStatus().ShouldBeEqual(claimSubStatus, "Claim Status");
            }
            finally
            {
                _claimAction.ClickOnClearOnWorkListPanel();
            }
        }


        [Test, Category("SmokeTest"), Category("WorkList1")]//US8582
        public void Verify_that_claims_returned_In_FCI_Worklist_shows_correct_status()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];
            try
            {
                _claimAction=_claimAction.NavigateToFciClaimsWorkList(true);
                SearchByClaimSeqFromWorkList(claimSequence);
                _claimAction.ClickWorkListIcon();
                _claimAction.ClickWorkListIconWithinWorkList();
                _claimAction.SelectFCIClaimStatus("Unreviewed");
                _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetClaimStatus().ShouldBeEqual("Cotiviti Unreviewed", "Claim Status");
            }
            finally
            {
                _claimAction.ClickClaimSearchIcon().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }

        }

        [Test, Category("SmokeTest"), Category("WorkList2")]//US10900
        public void Verify_that_for_Cotiviti_user_if_the_claim_is_in_Cotiviti_Unreviewed_and_the_HCIDONE_value_for_the_PCI_flag_is_F_then_the_claim_will_appear_on_the_list()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequenceHcidoneF = paramLists["ClaimSequenceHCIDONE_F"];

            IList<string> pciworkList = paramLists["PCIWorkList_Cotiviti"].Split(';');
            claimSequenceHcidoneF.AssertIsContainedInList(pciworkList, string.Format("ClaimSequence {0} is contained in the pci worklist", claimSequenceHcidoneF));

        }

        [Test]//US10900
        public void Verify_that_for_Cotiviti_user_if_the_claim_is_in_Cotiviti_Unreviewed_and_the_HCIDONE_value_for_the_PCI_flag_is_T_then_the_claim_will_not_appear_on_the_list()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequenceHcidoneT = paramLists["ClaimSequenceHCIDONE_T"];

            IList<string> pciworkList = paramLists["PCIWorkList_Cotiviti"].Split(';');
            pciworkList.Contains(claimSequenceHcidoneT)
                .ShouldBeFalse(string.Format("The ClaimSequence {0} appears in the pci worklist", claimSequenceHcidoneT));

        }

        [Test]
        public void Verify_that_system_deleted_pci_flag_does_NOT_appear_in_the_Cotiviti_pci_worklist()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequenceSystemDeleted = paramLists["ClaimSequenceSystemDeleted"];

            IList<string> pciworkList = paramLists["PCIWorkList_Cotiviti"].Split(';');
            pciworkList.Contains(claimSequenceSystemDeleted)
                 .ShouldBeFalse(string.Format("The ClaimSequence {0} appears in the pci worklist", claimSequenceSystemDeleted));

        }

        [Test]//TANT-35
        public void Verify_specialty_field_and_created_worklist()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var specialtyWithRecord = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "SpecialtyWithRecord", "Value");
            var specialtyForNoRecord = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "SpecialtyForNoRecord", "Value");
            var expectedList = _claimAction.GetCommonSql.GetSpecialtyList();
            expectedList.Insert(0, "All");
            _claimAction.GetSideBarPanelSearch.GetAvailableDropDownList("Specialty")
                .ShouldCollectionBeEqual(expectedList, "Specialty List should be Equal and Order by ");
            _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Specialty")
                .ShouldBeEqual(expectedList[0], "Default value of Specialty should be All");
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Specialty",expectedList[10]);
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Specialty", specialtyForNoRecord, false);
            _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Specialty")
                .ShouldBeEqual(specialtyForNoRecord, "Specialty Dropdown should be single select type");
            _claimAction.ClickOnCreateButton();
            _claimAction.WaitForWorkingAjaxMessage();
            _claimAction.GetEmptyMessage()
                .ShouldBeEqual("No matching records were found", "No Match Record Message");
            _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Specialty", specialtyWithRecord, false);
            _claimAction.ClickOnCreateButton();
            _claimAction.WaitForWorkingAjaxMessage();
            var i = 1;
            while (true)
            {
                if(_claimAction.IsBlankPagePresent())
                    break;
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetProviderSpecialty()
                    .ShouldBeEqual(specialtyWithRecord,
                        string.Format("Is Correct Specialty Display for iteration<{0}>?", i));
                _claimAction.IsSpecialtyOnFlaggedLinesByValuePresent(specialtyWithRecord)
                    .ShouldBeTrue("At Lease one specialty Present in Flagged Lines?");
                if (i >= 8)
                    break;
                _claimAction.ClickOnNextButton();
                _claimAction.WaitForWorkingAjaxMessage();
                i++;
                
            }
            _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
            _claimAction.GetSideBarPanelSearch.ClickOnClearLink();
            _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Specialty")
                .ShouldBeEqual(expectedList[0],
                    "Specialty should be default value when click on clear link");

        }



        [Test] //CAR-13(CAR-141)
        public void Verification_of_Claim_View_Restriction_Filter()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            try
            {
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                var restrictionList =
                    _claimAction.GetSideBarPanelSearch.GetAvailableDropDownList("Claim Restrictions");
                var restrictionListDB = _claimAction.GetRestrictionsListFromDB();
                var restrictionValueForRestrictedUser = paramLists["RestrictionValueForRestrictedUser"];

                StringFormatter.PrintMessage("Verifying Restriction Values from Database");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions")
                    .ShouldBeEqual("All",
                        "Claim Restrictions default value should be All");
                restrictionListDB.Insert(0, "No Restriction");
                restrictionListDB.Insert(0, "All");
                restrictionListDB.ShouldCollectionBeEquivalent(restrictionList,
                    "Restriction Values present in Database should be populated in filter option");
                restrictionList.RemoveRange(0, 2);

                StringFormatter.PrintMessage("Verifying Restriction Values For Individual Restriction Option");

                foreach (var restriction in restrictionList)
                {
                    _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                        restriction);
                    _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                    VerifyConditionsForWorkListClaims(2, _claimAction.DoesClaimContainRespectiveRestriction,
                        String.Format("Claim should not contain {0}  restriction",restriction));
                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                }

                StringFormatter.PrintMessage(
                    "Verifying if No Restrictions is selected, claims that don't have any restrictions are created in worklist");
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                    "No Restriction");
                _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                VerifyConditionsForWorkListClaims(3, _claimAction.DoesClaimNotHaveRestriction,
                    "Claim should not contain any restriction");

                StringFormatter.PrintMessage(
                    "Verifying Users that have a restriction assigned will only see restriction values that are not assigned to them");
                _claimAction.Logout().LoginAsClaimViewRestrictionUser().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions").ShouldNotContain(
                    restrictionValueForRestrictedUser, string.Format(
                        "Restriction {0} shouldn't be visible in filter option for current user",
                        restrictionValueForRestrictedUser));
                _claimAction.GetSideBarPanelSearch.GetAvailableDropDownList("Claim Restrictions").ShouldNotContain(
                    restrictionValueForRestrictedUser,
                    string.Format("Restriction {0} shouldn't be visible in filter option for current user", restrictionValueForRestrictedUser));

            }

            finally
            {
                
                _claimAction.ToggleWorkListToPci();
            }


        }

        [Test] //CAR-109(CAR-255) + CAR-493(CAR-415)
        public void Verification_of_Claim_View_Restriction_Filter_PCI_QA_Work_List()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            try
            {
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.ToggleWorkListToQa();
                var restrictionList =
                    _claimAction.GetSideBarPanelSearch.GetAvailableDropDownList("Claim Restrictions");
                var restrictionListDB = _claimAction.GetRestrictionsListFromDB();
                var restrictionValueForRestrictedUser = paramLists["RestrictionValueForRestrictedUser"].Split(';').ToList();

                StringFormatter.PrintMessage("Verifying Restriction Values from Database");
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions")
                    .ShouldBeEqual(restrictionList[0],
                        "Claim Restrictions default value should be All");
                restrictionList.RemoveRange(0, 2);
                restrictionListDB.ShouldCollectionBeEquivalent(restrictionList,
                    "Restriction Values present in Database should be populated in filter option");


                StringFormatter.PrintMessage("Verifying Restriction Values For Individual Restriction Option");
                foreach (var restriction in restrictionList)
                {
                    _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                        restriction);
                    _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.WaitForWorkingAjaxMessage();
                    var pageUrl = VerifyConditionsForWorkListClaims(2,
                        _claimAction.DoesClaimContainRespectiveRestriction,
                        string.Format("Claim should not contain {0}  restriction", restriction));
                    CurrentPage = _claimAction.SwitchTabAndOpenNewClaimActionByUrlByClaimViewRestrictedUser(pageUrl);
                    _claimAction.GetUnAuthorizedMessage().ShouldBeEqual(UnAuthorizedMessage, string.Format(
                        "User with no restriction access assigned " +
                        "is not able to view the claim with {0} restricted access", restriction));
                    _claimAction.SwitchTab(_claimAction.CurrentWindowHandle, true);

                    CurrentPage.Logout().LoginAsHciAdminUserClaim5();
                    _claimAction = QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                    _claimAction.ClickWorkListIcon();
                    _claimAction.ToggleWorkListToQa();
                }

                StringFormatter.PrintMessage(
                    "Verifying if 'All' is selected, claims that don't have any restrictions are created in worklist");

                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                    "All");
                _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.WaitForWorkingAjaxMessage();
                VerifyConditionsForWorkListClaims(3, _claimAction.DoesClaimNotHaveRestriction,
                    "All the Claims which are not restricted for another internal user are displayed");

                StringFormatter.PrintMessage(
                    "Verifying 'No Restriction' filter in unrestricted users will display all the unrestricted claims");
                if (!_claimAction.IsWorkListControlDisplayed()) _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions", "No Restriction");
                _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.WaitForWorkingAjaxMessage();
                VerifyConditionsForWorkListClaims(3, _claimAction.DoesClaimMatchNoRestriction,
                    "Claims should not contain any restriction");

                StringFormatter.PrintMessage(
                    "Verifying restricted users will only be able to view internally-unrestricted claims and client-restricted claims");
                if (_claimAction.IsPageErrorPopupModalPresent())
                    _claimAction.ClosePageError();
                _claimAction.Logout().LoginAsClaimViewRestrictionUser().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToQa();

                restrictionValueForRestrictedUser.ForEach(delegate(string restriction)
                {
                    _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions").ShouldNotContain(
                        restriction, string.Format(
                            "Restriction {0} shouldn't be visible in filter option for current user",
                            restrictionValueForRestrictedUser));
                });

                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                    "All");
                _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.WaitForWorkingAjaxMessage();
                VerifyConditionsForWorkListClaims(3, _claimAction.DoesClaimHaveProperRestrictionForRestrictedUser,
                    "Claim should not contain any restriction");

                StringFormatter.PrintMessage(
                    "Verifying 'No Restriction' filter in restricted users will display all the unrestricted claims");
                if (!_claimAction.IsWorkListControlDisplayed()) _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions", "No Restriction");
                _claimAction.GetSideBarPanelSearch.ClickOnButtonByButtonName("Create");
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.WaitForWorkingAjaxMessage();
                VerifyConditionsForWorkListClaims(3, _claimAction.DoesClaimMatchNoRestriction,
                    "Claims should not contain any restriction");


            }
            
            finally
            {
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.ToggleWorkListToPci();
            }
        }

        
        [Test, Category("SmokeTestDeployment"), Category("SmokeWorkList1")] //TANT-83
        public void Verify_CV_RN_Claims_Worklist()
        {
            try
            {
                _claimAction = _claimAction.NavigateToCVRnWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                
                for (var i = 0; i < 2; i++)
                {
                    if(CurrentPage.GetPageHeader()==Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadRNWorklist();
                }
            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("SmokeWorkList1")] //TANT-84
        public void Verify_CV_Coder_Claims_Worklist()
        {
            try
            {
                _claimAction.NavigateToCVCodersClaim(isFromRetroPage:true);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadCoderWorkList();
                }
            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Retrying(Times = 2)]
        [Test, Category("SmokeTestDeployment"), Category("SmokeWorkList1")] //TANT-85
        public void Verify_CV_QC_Claims_Worklist()
        {
            try
            {
                _claimAction.NavigateToCvQcWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadPCIQaClaimsWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTestDeployment"),Category("SmokeWorkList1")] // TANT-87
        public void Verify_DCI_Claims_Worklist_smokeTest()
        {
            try
            {
                _claimAction.NavigateToDciClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadNextClaimsFromWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("SmokeWorkList1")] // TANT-86
        public void Verify_FCI_Claims_Worklist_smokeTest()
        {
            try
            {
                _claimAction.NavigateToFciClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadNextClaimsFromWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("SmokeWorkList1")] // TANT-112
        public void Verify_FFP_Claims_Worklist_smokeTest()
        {
            try
            {
                _claimAction.NavigateToFfpClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadNextClaimsFromWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTest")] // TANT-87
        public void Verify_DCI_Claims_Worklist()
        {
            try
            {
                _claimAction.NavigateToDciClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadDCIClaimsWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTest")] // TANT-86
        public void Verify_FCI_Claims_Worklist()
        {
            try
            {
                _claimAction.NavigateToFciClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadFCIClaimsWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }

        [Test, Category("SmokeTest")] // TANT-112
        public void Verify_FFP_Claims_Worklist()
        {
            try
            {
                _claimAction.NavigateToFfpClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                for (var i = 0; i < 4; i++)
                {
                    if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimSearch))
                        break;
                    LoadFFPClaimsWorkList();
                }

            }
            finally
            {
                if (CurrentPage.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
                    _claimAction.ClickClaimSearchIcon()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                else
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
            }
        }
        [Test]//TE-451
        public void verify_ReviewGroup_Filter_For_PCI_and_FFP_WorkList()
        {
            try
            {
                _claimAction = QuickLaunch.NavigateToFfpClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var expectedReviewGroupList = _claimAction.GetReviewGroup(ClientEnum.SMTST.ToString());
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", "Unreviewed");
                _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                StringFormatter.PrintMessage("Verify Review Group ");
                ValidateFieldSupportingMultipleValues("Review Group", expectedReviewGroupList);
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", "Pended");
                _claimAction.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Review Group")
                    .ShouldBeTrue("Is Review Group dropdown displayed for Pended Claim");
                _claimAction.CreateFFPWorklistWithReviewGroups(expectedReviewGroupList);
            }
            finally
            {
                CurrentPage.NavigateToCVClaimsWorkList(true);
            }
        }

        [Test,Category("OnDemand")] //TE-769
        public void Verify_Additional_Pended_Substatus_List_For_DCI_Active_Clients_In_WorkList_Search_Filter()
        {
            var statusListForDCIActiveFromDb = _claimAction.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.SMTST.ToString());
            var statusListForDCIInactiveFromDb = _claimAction.GetAssociatedClaimSubStatusForInternalUserWithDCIInactive(ClientEnum.SMTST.ToString());
            _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(), true);
            try
            {
                _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", ClaimStatusTypeEnum.Pended.ToString());
                _claimAction.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Claim Sub Status").
                    ShouldBeEqual(statusListForDCIActiveFromDb, "Sub status list should match");

                _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString());
                _claimAction.Refresh();
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", ClaimStatusTypeEnum.Pended.ToString());
                _claimAction.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Claim Sub Status").
                    ShouldBeEqual(statusListForDCIInactiveFromDb, "Sub status list should match");
            }
            finally
            {
                _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(), true);
            }
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
        void SearchByClaimSeqFromWorkList(string claimSeq)
        {
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.ClickSearchIcon()
                .SearchByClaimSequence(claimSeq);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        }
        private bool VerifyAllFlagsDeleted()
        {
            List<bool> isAllFlagDeleted = new List<bool>();
            for (int i = 1; i < 2; i++)
            {
                for (int j = 1; j < 2; j++)
                {
                    isAllFlagDeleted.Add(_claimAction.IsFlaggedLineDeletedByLine(i, j));
                }
            }

            return isAllFlagDeleted.Any(x => x == false);
        }

        private bool VerifyAllFlagsRestored()
        {
            List<bool> isAllFlagRestored = new List<bool>();
            for (int i = 1; i < 2; i++)
            {
                for (int j = 1; j < 2; j++)
                {
                    isAllFlagRestored.Add(_claimAction.IsFlaggedLineNotDeletedByLine(i, j));
                }
            }

            return isAllFlagRestored.Any(x => x == false);
        }
        private string VerifyConditionsForWorkListClaims(int numberofNext, Func<ClaimActionPage, bool> RequiredCondition, string message = "")
        {
            int count = 0;
            string pageUrl = null;
            _claimAction.WaitForWorking();
            if (_claimAction.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
            {
                _claimAction.AssertFail("No Record Found");
            }

            while (count++ < numberofNext && _claimAction.GetPageHeader() == Service.Support.Utils.Extensions.GetStringValue(PageHeaderEnum.ClaimAction))
            {
                _claimAction.WaitForWorking();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                RequiredCondition(_claimAction).ShouldBeTrue(message);
                pageUrl = _claimAction.CurrentPageUrl;
                _claimAction.ClickOnNextButton();

            }

            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return pageUrl;
        }
        //void VerificationForFciAndPciClaimsWorkListOptionUnderClaimMenu(ProductEnum product)
        //{
        //    CurrentPage = _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnPrivileges();
        //    string workListValue = _profileManager.GetWorkListValue(product);
        //    string message = string.Format("Does User have Read/Write or Read-only authority to {0} Claims Work List & appear {0} Claims Work List under Claim menu ?", product);
        //    StringFormatter.PrintMessageTitle(string.Format("{0} Claims Work List", product));
        //    switch (product)
        //    {
        //        case ProductEnum.CV:
        //            Mouseover.IsCVClaimsWorkList().ShouldBeEqual(_workList.ContainsValue(workListValue), message);
        //            break;

        //        case ProductEnum.FCI:
        //            Mouseover.IsFciClaimsWorkList().ShouldBeEqual(_workList.ContainsValue(workListValue), message);
        //            break;
        //    }
        //    StringFormatter.PrintLineBreak();
        //}

        void VerificationForFciAndPciClaimsWorkListOptionUnderClaimMenu(RoleEnum pciRole,RoleEnum fciRole)
        {
            CurrentPage = _newUserProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
            _newUserProfileSearch.SearchUserByNameOrId(new List<string> { EnvironmentManager.Username }, true);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.Username);
            _newUserProfileSearch.ClickOnUserSettingTabByTabName(Service.Support.Utils.Extensions.GetStringValue(UserSettingsTabEnum.RolesAuthorities));
            var isPciRoleAssigned = _newUserProfileSearch.IsAvailableAssignedRowPresent(1, Service.Support.Utils.Extensions.GetStringValue(pciRole));

            var isFciRoleAssigned = _newUserProfileSearch.IsAvailableAssignedRowPresent(1, Service.Support.Utils.Extensions.GetStringValue(fciRole));

            var message = string.Format("Does User have {0} and {1} role for CV and FCI Claims Work List appears under Claims Work List under Claim menu ?", pciRole, fciRole);
            StringFormatter.PrintMessageTitle(message);

            _newUserProfileSearch.IsPCI_FCIClaimsWorkList().ShouldBeEqual(isPciRoleAssigned && isFciRoleAssigned, $"Is {ProductEnum.CV} and {ProductEnum.FCI} Work List Present");

            CurrentPage.RefreshPage();
        }


        void LoginAsUserHavingNotWorkListOptionUnderClaimMenu()
        {
            Login = CurrentPage.Logout();
            CurrentPage = QuickLaunch = Login.LoginAsUserHavingNoAnyAuthority();
            _defaultUser = EnvironmentManager.HciUserWithNoManageAppealAuthority;
            StringFormatter.PrintMessage("Login with user that does not have FCI & CV Claims Work List under Claim menu.");
        }

     
        private void LoadRNWorklist()
        {
            _claimAction.IsClaimRestrictionIndicatorTooltipPresent("rn").ShouldBeTrue("Only claims with RN designation should be presented in this worklist.");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        private void LoadCoderWorkList()
        {
            _claimAction.IsClaimRestrictionIndicatorTooltipPresent("coder").ShouldBeTrue("Only claims with CPC designation should be presented in this worklist.");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        private void LoadPCIQaClaimsWorkList()
        {
            _claimAction.GetQaAuditIconTooltip().AssertIsContained("Claim requires CV QC audit.  Audit cannot be completed by the same user who approved the claim.", "Audit Icon tooltip");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        private void LoadNextClaimsFromWorkList()
        {
            _claimAction.GetAllFlagsFromWorklist().Count.ShouldBeGreaterOrEqual(1, "claims with at least one flag should be presented in work list");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        private void LoadDCIClaimsWorkList()
        {
            var claseq = _claimAction.GetClaimSequence();
            _claimAction.GetFlagCountByClaimSeqAndProductFromDatabase(claseq, "D").ShouldBeGreaterOrEqual(1, "claims with at least one active FFP flag should be presented in work list");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        public void LoadFCIClaimsWorkList()
        {
            var claseq = _claimAction.GetClaimSequence();
            _claimAction.GetFlagCountByClaimSeqAndProductFromDatabase(claseq,"U").ShouldBeGreaterOrEqual(1, "claims with at least one active FCI flag should be presented in work list");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        public void LoadFFPClaimsWorkList()
        {
            var claseq = _claimAction.GetClaimSequence();
            _claimAction.GetFlagCountByClaimSeqAndProductFromDatabase(claseq, "R").ShouldBeGreaterOrEqual(1, "claims with at least one active FFP flag should be presented in work list");
            _claimAction.ClickOnNextButton();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        #endregion
    }
}
