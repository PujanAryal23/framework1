using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nucleus.Service.Data;
using NUnit.Framework;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nucleus.Service.Support.Menu;
using Nucleus.Service;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class ClaimAction1 : NewAutomatedBase
    {
        #region PRIVATE FIELDS


        private ClaimActionPage _claimAction;
        private NewPopupCodePage _newpopupCodePage;
        private QuickLaunchPage _quickLaunchPage;
        private string _claimSequence = string.Empty;
        private ClaimSearchPage _claimSearch;
        private FlagPopupPage _flagPopup;

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
                _claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");

                _claimAction = QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
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
            if (_claimAction.IsPageErrorPopupModalPresent())
                _claimAction.ClosePageError();

            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _claimAction = _claimAction.Logout()
                    .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient).NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                SearchByClaimSeqFromWorkList(_claimSequence);
            }

            if (!_claimAction.GetClaimSequence().Equals(_claimSequence))
            {
                SearchByClaimSeqFromWorkList(_claimSequence);
            }

        }

        protected override void ClassCleanUp()
        {
            try
            {
                if (_claimAction.IsClaimLocked())
                    Console.WriteLine("Claim is Locked!");
                _claimAction.CloseDatabaseConnection();
                //if (CurrentPage.IsQuickLaunchIconPresent())
                //    _claimAction.GoToQuickLaunch();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        #endregion

        #region TEST SUITES



        [Test]//US9473
        public void Verify_that_if_user_selects_flag_that_has_editor_requires_history_line_equals_to_F_then_Trigger_Claim_Line_box_is_disabled()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            try
            {
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.SelectAddInFlagAdd("FRP");
                (_claimAction.IsTriggerClaimLineDisabled()).ShouldBeTrue("Trigger Claim Line is disabled.");
            }
            finally
            {
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }

        [Test]//US9473
        public void Verify_that_if_user_selects_flag_that_has_editor_requires_history_line_equals_to_T_then_Trigger_Claim_Line_box_is_enabled()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            try
            {
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.SelectAddInFlagAdd("DUP");
                (!_claimAction.IsTriggerClaimLineDisabled()).ShouldBeTrue("Trigger Claim Line is enabled.");
            }
            finally
            {
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }

        [Test]
        public void Verify_that_while_adding_a_flag_save_button_is_disabled_until_flag_is_selected()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            try
            {
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.IsSaveButtonEnabled().ShouldBeFalse("Save button is enabled.");
                _claimAction.SelectAddInFlagAdd("AADD");
                _claimAction.IsSaveButtonEnabled().ShouldBeTrue("Save button is enabled.");
            }
            finally
            {
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }

        [Test]//US8586
        public void Verify_that_while_adding_a_flag_users_receives_modal_when_trying_to_save_a_flag_that_requires_an_edit_source()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            bool modalpresent = false;
            try
            {
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.SelectAddInFlagAdd("AADD");
                _claimAction.ClickOnSaveEditButton();
                modalpresent = _claimAction.IsPageErrorPopupModalPresent();
                modalpresent.ShouldBeTrue("Modal is present.");
                _claimAction.GetPageErrorMessage().ShouldBeEqual("Flag Source is required before the flag can be saved", "Modal Error Message");
            }
            finally
            {
                if (modalpresent)
                    _claimAction.ClosePageError();
                _claimAction.ClickOnFlagLevelCancelLink();
                _claimAction.Wait();
            }
        }

        [Test, Category("SmokeTest")]//US8584
        public void Verify_that_add_flag_panel_appears_when_user_clicks_on_add_icon()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            try
            {
                _claimAction.IsAddFlagPanelPresent().ShouldBeFalse("Add Flag Panel is present.");
                _claimAction.ClickAddIconButton();
                _claimAction.IsAddFlagPanelPresent().ShouldBeTrue("Add Flag Panel is present.");
            }
            finally
            {
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }

        [Test]
        public void Verify_that_entering_non_numbers_in_sug_Units_gives_page_error_popup_saying_Only_numbers_allowed()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string flag = paramLists["Flag"];
            StringFormatter.PrintLineBreak();
            var modalPresent = false;
            try
            {
                _claimAction
                    .ClickOnEditForGivenFlag(flag);
                _claimAction.EnterSugUnits("a");
                (modalPresent = _claimAction.IsPageErrorPopupModalPresent()).ShouldBeTrue("Page Error Modal popup present ?");
                _claimAction.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                                                                       "Message Content");
            }
            finally
            {
                if (modalPresent)
                    _claimAction.ClosePageError();
                //_claimAction.ClickOnCancelLink();
                StringFormatter.PrintLineBreak();
            }
        }

        [Test]
        public void Verify_that_entering_units_greater_than_the_units_on_the_line_gives_validation_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string flag = paramLists["Flag"];
            StringFormatter.PrintLineBreak();
            var modalPresent = false;
            try
            {
                _claimAction.ClickOnEditForGivenFlag(flag);
                int sugUnit = Convert.ToInt32(_claimAction
                                                  .GetClaimLevelUnitValue(flag));
                Console.WriteLine("Unit on line: " + sugUnit);
                sugUnit += 1;
                Console.WriteLine(string.Format("Entering sug unit value '{0}'", sugUnit));
                _claimAction.EnterSugUnits(sugUnit.ToString(CultureInfo.InvariantCulture));
                (modalPresent = _claimAction.IsPageErrorPopupModalPresent()).ShouldBeTrue("Page Error Modal popup present ?");
                _claimAction.GetPageErrorMessage().ShouldBeEqual("Suggested Units cannot be greater than the units billed on the line.",
                                                                       "Message Content");
            }
            finally
            {
                if (modalPresent)
                    _claimAction.ClosePageError();
                // _claimAction.ClickOnCancelLink();
                StringFormatter.PrintLineBreak();
            }
        }

        [Test]
        public void Verify_click_on_the_lines_icon_in_the_Claim_Dollar_Details_view_loads_the_Claim_Lines_View()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnDollarIcon();
            _claimAction.ClickOnLinesIcon();
            _claimAction.GetValueOfLowerRightQuadrant()
                .ShouldBeEqual("Claim Lines", "Label of Lower Right Quadrant");
        }

        [Test]
        public void Verify_click_on_the_lines_icon_in_the_Lines_Details_view_loads_the_Claim_Lines_View()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnLineDetailsSection();
            _claimAction.ClickOnLinesIcon();
            _claimAction.GetValueOfLowerRightQuadrant().ShouldBeEqual("Claim Lines", "Label of Lower Right Quadrant");
        }

        [Test]
        public void Verify_click_on_the_Dollar_symbol_in_the_Line_Details_view_loads_the_Claim_Dollar_Details_view()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnLineDetailsSection();
            _claimAction.ClickOnDollarIcon();
            string labelValue = _claimAction.GetValueOfLowerRightQuadrantClaimDollar();
            _claimAction.ClickOnLinesIcon();
            labelValue.ShouldBeEqual("Claim Dollar Details", "Label of Lower Right Quadrant");
        }

        [Test]
        public void Verify_that_click_on_line_and_verify_that_Line_Details_appear_in_lower_right_quadrant()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnLineDetailsSection();
            string labelValue = _claimAction.GetValueOfLowerRightQuadrant();
            _claimAction.ClickOnLinesIcon();
            labelValue.ShouldBeEqual("Line Details", "Label of Lower Right Quadrant");
        }

        [Test]
        public void Verify_that_multiple_click_on_same_proc_code_reuses_the_window()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string popupName = "";
            try
            {
                const int count = 4;
                for (int i = 0; i <= count && i != count; i++)
                {
                    _newpopupCodePage = _claimAction.ClickOnClaimLinesProcCode(out popupName);
                    _claimAction = _newpopupCodePage.SwitchBackToNewClaimActionPage();
                }
                _newpopupCodePage.CountWindowHandles(_newpopupCodePage.PageTitle).ShouldBeEqual(1, string.Format("Click on Same proc code for {0} times and number of popup is", count));
            }
            finally
            {
                _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(popupName);
            }
        }

        [Test]
        public void Verify_that_new_window_is_opened_when_clicked_on_different_proc_code()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string procCode1 = paramLists["Proc1"];
            string procCode2 = paramLists["Proc2"];
            IList<string> popups = new List<string>();
            int countWindowHandles = 0;
            try
            {
                _newpopupCodePage = _claimAction.ClickOnClaimLineProcCode("CPT Code", procCode1);
                popups.Add(procCode1);
                countWindowHandles += _newpopupCodePage.CountWindowHandles(_newpopupCodePage.PageTitle);
                _claimAction = _newpopupCodePage.SwitchBackToNewClaimActionPage();
                _newpopupCodePage = _claimAction.ClickOnClaimLineProcCode("CPT Code", procCode2);
                popups.Add(procCode2);
                countWindowHandles += _newpopupCodePage.CountWindowHandles(_newpopupCodePage.PageTitle);
                countWindowHandles.ShouldBeGreater(1, string.Format("{0} windows opened when clicked on {0} different proc codes", countWindowHandles));
            }
            finally
            {
                foreach (var popup in popups)
                {
                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(popup);
                }
            }
        }



        [Test]
        public void Verify_that_click_on_proc_code_opens_proc_code_description_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string popupName = "";
            try
            {
                _newpopupCodePage = _claimAction.ClickOnClaimLinesProcCode(out popupName);
                _newpopupCodePage.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Popup Header Text");
                _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", popupName), "Procedure Code");
                Console.WriteLine("popup code text {0}", _newpopupCodePage.GetTextValueinLiTag(1));
            }
            finally
            {
                _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(popupName);
            }
        }

        [Test]
        public void Verify_that_click_on_trigger_code_opens_trigger_code_description_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string popupName = "";
            try
            {
                _newpopupCodePage = _claimAction.ClickOnTriggerCodeToOpenTriggerDescriptionPopup(out popupName);
                _newpopupCodePage.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Popup Header Text");
                _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", popupName), "Trigger Code");
                Console.WriteLine("popup code text {0}", _newpopupCodePage.GetTextValueinLiTag(1));
                _claimAction = _newpopupCodePage.SwitchBackToNewClaimActionPage();
            }
            finally
            {
                _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(popupName);
            }
        }

        [Test]
        public void Verify_that_click_on_revenue_code_opens_revenue_code_description_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string popupName = "";
            try
            {
                _newpopupCodePage = _claimAction.ClickOnRevenueCodeToOpenRevCodeDescriptionPopup(out popupName);
                _newpopupCodePage.GetPopupHeaderText().ShouldBeEqual("Revenue Code", "Popup Header Text");
                _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", popupName), "Revenue Code");
                Console.WriteLine("popup code text {0}", _newpopupCodePage.GetTextValueinLiTag(1));

            }
            finally
            {
                _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(popupName);
            }

        }

        [Test]
        public void Verify_when_user_click_on_Trigger_Claim_opens_Claim_Action_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string originalHandle = string.Empty;
            string triggerClaim = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName)["TriggerClaim"];
            try
            {
                ClaimActionPage newClaimAction = _claimAction.ClickOnTriggerClaimLink(triggerClaim, out originalHandle);
                newClaimAction.CurrentPageTitle.ShouldBeEqual(newClaimAction.PageTitle, "Page Title");
                newClaimAction.NewClaimActionPageIsWindowPopup(triggerClaim.Substring(0, triggerClaim.IndexOf('-'))).ShouldBeTrue("Nucleus: Claim Action is popup");
            }
            finally
            {
                _claimAction.CloseAnyPopupIfExist(originalHandle);
            }
        }



        [Test]
        public void Verify_that_click_on_Dollar_symbol_loads_Claim_Dollar_Details_view()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnDollarIcon();
            string labelValue = _claimAction.GetValueOfLowerRightQuadrantClaimDollar();
            _claimAction.ClickOnLinesIcon();
            labelValue.ShouldBeEqual("Claim Dollar Details", "Label of Lower Right Quadrant");
        }



        [Test]
        public void Verify_that_if_we_do_not_select_reason_code_when_deleting_a_flag_validation_appears()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            try
            {
                _claimAction.ClickOnEditAllFlagsIcon();
                _claimAction.ClickDeleteButtonOfEditAllFlagsSection();
                _claimAction.ClickOnSaveButton();
                _claimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("ReasonCode Popup modal is present");
                _claimAction.ClosePageError();
            }
            finally
            {
                _claimAction.ClickOnCancelLink();
            }
        }

        [Test]
        public void Verify_that_clicking_on_the_pencil_icon_opens_flag_editing_box_and_clicking_on_it_again_closes_the_flag_edit()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnEditAllFlagsIcon();
            _claimAction.IsEditFlagAreaPresent().ShouldBeTrue("Flag Edit Area");
            //clicking on the pencil icon again closes the edit flag area
            _claimAction.ClickOnCancelLink();
            _claimAction.IsEditFlagAreaPresent().ShouldBeFalse("Flag Edit Area");

        }



        [Test, Category("SmokeTest")]
        public void Verify_that_edit_icon_is_enabled_for_user_having_manage_edit_authority()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            if(_claimAction.IsClaimLocked())
                _claimAction.RemoveLock();
            _claimAction.IsEditFlagsIconEnabled().ShouldBeTrue("Edit Icon Enabled?");
        }

        [Test]
        public void Verify_when_clicked_on_delete_pencil_icon_approve_next_approve_and_next_buttons_get_disabled()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            if (!_claimAction.IsDeleteAllFlagsPresent())
            {
                _claimAction.ClickOnEditAllFlagsIcon();
                _claimAction.SelectReasonCode("ACC 1 - Accept Response To Logic Request");
                _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                _claimAction.ClickOnSaveButton();
                Console.WriteLine("Restored flags.");
            }
            _claimAction.ClickOnEditAllFlagsIcon();
            _claimAction.IsApproveButtonDisabled().ShouldBeTrue("Approve button disabled?");
            _claimAction.IsNextButtonDisabled().ShouldBeTrue("Next button disabled?");
            StringFormatter.PrintLineBreak();

        }




        [Test]
        public void Verify_that_entering_sug_paid_greater_than_the_adj_paid_gives_validation_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string flag = paramLists["Flag"];
            var modalPresent = false;
            StringFormatter.PrintLineBreak();
            try
            {
                decimal adjPaid = _claimAction.GetLineAdjustedSavings(flag);
                _claimAction.ClickOnEditForGivenFlag(flag);
                _claimAction.SelectReasonCode("ADD - Edit Added After Appeal");
                _claimAction.EnterSugPaid((adjPaid + 2).ToString(CultureInfo.InvariantCulture));
                _claimAction.SaveFlag();
                (modalPresent = _claimAction.IsPageErrorPopupModalPresent()).ShouldBeTrue("Page Error Modal popup present ?");
                _claimAction.GetPageErrorMessage().ShouldBeEqual((string.Format("Suggested Paid amount of ${0} cannot exceed the line Adjusted Paid amount of ${1}.", adjPaid + 2, adjPaid)), "Message Content");
            }
            finally
            {
                if (modalPresent)
                    _claimAction.ClosePageError();
                //  _claimAction.ClickOnCancelLink();
                StringFormatter.PrintLineBreak();
            }

        }


        [Test, Category("SmokeTest")]
        public void Verify_that_version_exists_in_lower_right_corner_of_new_claim_action()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.GetVersionText().Contains("Version:").ShouldBeTrue("Version: text is present ?");
        }



        [Test]
        public void Clicking_on_tranfer_option_brings_up_Transfer_Claims_widget_and_if_saved_without_status_error_message_comes_cancel_should_close_the_widget()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnTransferButton();
            _claimAction.IsTransferClaimsWidgetDisplayed().ShouldBeTrue("Transfer Widget Displayed?");
            _claimAction.GetTransferWidgetHeader().ShouldBeEqual("Transfer Claim", "Transfer widget header message");
            _claimAction.ClickOnSaveButton();
            _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error popup present?");
            _claimAction.GetPageErrorMessage().ShouldBeEqual("Status is required before the record can be saved.", "Page error modal popup message");
            _claimAction.ClosePageError();
            _claimAction.ClickOnCancelLink();
        }

        [Test]
        public void Clicking_on_tranfer_approve_option_brings_up_Transfer_Approve_Claims_widget_and_if_saved_without_status_error_message_comes_cancel_should_close_the_widget()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction.ClickOnTransferApproveButton();
            _claimAction.IsTransferClaimsWidgetDisplayed().ShouldBeTrue("Transfer/Approve Claims Widget Displayed?");
            _claimAction.GetTransferWidgetHeader().ShouldBeEqual("Transfer/Approve Claim", "Transfer/Approve Claim widget header message");
            _claimAction.ClickOnSaveButton();
            _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error popup present?");
            _claimAction.GetPageErrorMessage().ShouldBeEqual("Status is required before the record can be saved.", "Page error modal popup message");
            _claimAction.ClosePageError();
            _claimAction.ClickOnCancelLink();
        }


        [Test]
        public void Ellipsis_appears_for_different_fields()
        {
            _claimAction.IsEllipsisPresentForFlaggedLinesFlagDescription().ShouldBeTrue("Ellipsis will appear over limit for FLagged LInes >> FLagDescription");
            _claimAction.IsEllipsisPresentForFlaggedLinesProcDescription().ShouldBeTrue("Ellipsis will appear over limit for  FLagged LInes >> ProcDescription");
            _claimAction.IsEllipsisPresentForSpecialty().ShouldBeTrue("Ellipsis will appear over limit for Specialty");
            _claimAction.IsEllipsisPresentForProviderName().ShouldBeTrue("Ellipsis will appear over limit for PROVIDERNAME");
            _claimAction.IsEllipsisPresentForHciRun().ShouldBeTrue("Ellipsis will appear over limit for HCIRUN");
            _claimAction.IsEllipsisPresentForHciVoid().ShouldBeTrue("Ellipsis will appear over limit for HCIVOID");
            _claimAction.IsEllipsisPresentForHdrPayorName().ShouldBeTrue("Ellipsis will appear over limit for HDR_PAYORNAME");
            _claimAction.IsEllipsisPresentForHdrPhone().ShouldBeTrue("Ellipsis will appear over limit for HDR_PHONE");


            _claimAction.IsEllipsisPresentForClaimLinesProcDescription().ShouldBeTrue("Ellipsis will appear over limit for Claim Lines >> ProcDescription");
            _claimAction.ClickOnLineDetailsSection();
            _claimAction.ClickOnDollarIcon();
            _claimAction.IsEllipsisPresentForClaimDollarDetailsProcDescription().ShouldBeTrue("Ellipsis will appear over limit for Claim Lines >> ProcDescription");

        }

        [Test, Category("SmokeTest")]
        public void Verify_menu_options_in_clinical_ops()
        {
            IDictionary<string, string> menuOptions = DataHelper.GetMappingData(FullyQualifiedClassName, "menu_options_in_clinical_ops");
            StringFormatter.PrintMessageTitle("Menu Options in Clinical Ops");
            foreach (var menuOption in menuOptions)
            {
                _claimAction.GetMenuOptionsForClinicalOps(menuOption.Key).ShouldBeEqual(menuOption.Value, string.Format("Menu Option {0}", menuOption.Key));
            }
            StringFormatter.PrintLineBreak();
        }

        [Test]
        //public void Verify_that_PCI_QA_Audit_worklist_menu_is_shown_for_user_with_PCI_QA_Authority()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

        //    SubMenu.IsSecondarySubMenuOptionPresent(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.PciQaWorkList)
        //        .ShouldBeTrue("Is CV QA SubMenu Present");


        //}

        public void Verify_that_CV_QC_Audit_worklist_menu_is_shown_for_user_with_CV_QC_Authority()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            _claimAction.IsSecondarySubMenuOptionPresent(HeaderMenu.Claim, SubMenu.ClaimWorkList, SubMenu.CVQCWorkList)
                .ShouldBeTrue("Is CV QC SubMenu Present");


        }

        [Test]
        public void Verify_that_if_no_logic_exist_on_flag_then_Lplus_icon_appears()
        {
            _claimAction.IsLogicPlusIconDisplayed(1).ShouldBeTrue("Lplus Icon should be displayed");
            _claimAction.ClickOnAddLogicIconByRow(1);
            _claimAction.IsLogicFormText("Create Logic").ShouldBeTrue("Create Logic form should be displayed");
            _claimAction.ClickOnCancelLink();
        }


        //[Test]
        //public void Verify_that_logic_icon_is_disabled_if_the_claim_is_locked_by_another_user()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    try
        //    {
        //        StringFormatter.PrintMessageTitle("Logic Icon is disabled if the claim is locked by another user");
        //        var startNucleus = new StartNucleus();
        //        var quickLaunch = startNucleus.StartNucleusApplication().LoginAsHciUser();
        //        CheckTestClientAndSwitch();
        //        ClaimActionPage newClaimAction = quickLaunch.NavigateToClaimSearch()
        //            .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);

        //        newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        //        bool isPresent = newClaimAction.IsDisabledLogicIconPresent();
        //        isPresent.ShouldBeTrue("Disabled Logic Icon Present");
        //    }
        //    finally
        //    {
        //        StringFormatter.PrintLineBreak();
        //    }
        //}

        [Test]
        public void Verify_that_Dx_Codes_section_appears_on_page_load_not_appeals()
        {
            _claimAction.GetTopRightComponentTitle().ShouldBeEqual("Dx Codes", "Top Right component title");
        }


        [Test]
        public void Verify_that_Dx_popups_match_code_on_the_page_and_long_description_appears_for_that_code()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> expectedDiagnosisCodes = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "Dx");
            int i = 1;
            foreach (var expectedDxCode in expectedDiagnosisCodes)
            {
                IDictionary<string, string> dxCode = _claimAction.ClickOnDxCodeAndGetDescription(i);
                dxCode.Single().Key.ShouldBeEqual(expectedDxCode.Key, "Dx Code:");
                dxCode.Single().Value.ShouldBeEqual(expectedDxCode.Value, "Dx Description:");
                i++;
                StringFormatter.PrintLineBreak();
            }
        }


        //  [Test]
        public void Verify_that_if_work_list_exists_then_user_is_directed_to_next_claim_after_clicking_on_Approve_Transfer_TransferApprove()
        {
            StringFormatter.PrintMessageTitle("Test");
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimAction
                .ClickWorkListIcon()
                .ToggleWorkListToPci()
                .SelectClaimStatus("Unreviewed")
                .ClickOnCreateButton();
            HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            string claimSequence = _claimAction.GetClaimSequence();
            //Check for worklist claim sequence TODO:
            string nextClaimSequence = _claimAction.GetNextClaimOfQueue();
            if (!_claimAction.IsApproveButtonDisabled())
            {
                _claimAction.ClickOnApproveButton();
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                (!_claimAction.IsBlankPagePresent()).ShouldBeTrue("New Claim Action page is not blank.", true);
                nextClaimSequence.ShouldBeEqual(_claimAction.GetClaimSequence(),
                                              "New Claim Action Page with Claim Sequence");
                _claimAction.ScrollToViewWorkListQuery();
                _claimAction.ClickOnReturnToClaim(claimSequence);
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            }

            if (!_claimAction.IsApproveButtonDisabled())
            {
                nextClaimSequence = _claimAction.GetNextClaimOfQueue();
                _claimAction.ClickOnTransferApproveButton();
                _claimAction.SelectStatusCode("Documentation Requested");
                _claimAction.ClickOnSaveButton();
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                (!_claimAction.IsBlankPagePresent()).ShouldBeTrue("New Claim Action page is not blank.", true);
                nextClaimSequence.ShouldBeEqual(_claimAction.GetClaimSequence(),
                                              "New Claim Action Page with Claim Sequence");
                _claimAction.ScrollToViewWorkListQuery();
                _claimAction.ClickOnReturnToClaim(claimSequence);
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            }

            nextClaimSequence = _claimAction.GetNextClaimOfQueue();
            _claimAction.ClickOnTransferButton();
            _claimAction.SelectStatusCode("Documentation Requested");
            _claimAction.ClickOnSaveButton();
            HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            (!_claimAction.IsBlankPagePresent()).ShouldBeTrue("New Claim Action page is not blank.", true);
            nextClaimSequence.ShouldBeEqual(_claimAction.GetClaimSequence(),
                                              "New Claim Action Page with Claim Sequence");
            _claimAction.ScrollToViewWorkListQuery();
            _claimAction.ClickOnReturnToClaim(claimSequence);
            HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            _claimAction.ClickWorkListIcon();
        }




        [Test]//US9415
        public void Verify_when_user_having_Enable_ClaimAction_Tooltip_setting_ON_can_see_the_tooltips_and_the_user_who_has_this_setting_OFF_cannot_see_the_tooltips()
        {
            //default test automation user has Enable_ClaimAction_Tooltipsetting ON
            StringFormatter.PrintMessageTitle("User with Enable_ClaimAction_Tooltipsetting ON");
            _claimAction.GetDeleteAllIconTooltip().ShouldBeEqual("Delete all Flags on the Claim", "Delete All tooltip");
            _claimAction.GetApproveIconTooltip().ShouldBeEqual("Approve", "Approve  tooltip");
            _claimAction.GetEditIconTooltip().ShouldBeEqual("Edit all Flags on the Claim", "Edit All  tooltip");
            _claimAction.GetAddIconTooltip().ShouldBeEqual("Add flag", "Add  tooltip");
            _claimAction.GetTransferIconTooltip().ShouldBeEqual("Transfer", "Transfer tooltip");
            StringFormatter.PrintMessageTitle("User with Enable_ClaimAction_Tooltipsetting OFF");
            //logging in with  user has Enable_ClaimAction_Tooltipsetting OFF
            _claimAction.Logout().LoginAsHciUser().NavigateToClaimSearch()
                .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);

            HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            _claimAction.GetDeleteAllIconTooltip().ShouldBeEqual("", "Delete All tooltip");
            _claimAction.GetApproveIconTooltip().ShouldBeEqual("", "Approve  tooltip");
            _claimAction.GetEditIconTooltip().ShouldBeEqual("", "Edit All  tooltip");
            _claimAction.GetAddIconTooltip().ShouldBeEqual("", "Add  tooltip");
            _claimAction.GetTransferIconTooltip().ShouldBeEqual("", "Transfer tooltip");

        }

        [Test]//US9606
        public void Verify_when_user_adds_an_MPR_flag_a_page_error_popup_appears_if_a_trigger_claim_is_not_selected()
        {
            bool modalpresent = false;
            try
            {
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.SelectAddInFlagAdd("MPR");
                _claimAction.SelectFlagSourceInAddFlag("CL - Client Policy");
                _claimAction.ClickOnSaveEditButton();
                modalpresent = _claimAction.IsPageErrorPopupModalPresent();
                modalpresent.ShouldBeTrue("Page error popup present?");
                _claimAction.GetPageErrorMessage().ShouldBeEqual("Trigger Claim/Line is required before the flag can be saved", "Page Error Message");
            }
            finally
            {
                if (modalpresent)
                    _claimAction.ClosePageError();
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }

        [Test] //CAR-2957(CAR-2928)
        public void Verify_flag_description_in_flag_Description_pop_up()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string flag = paramLists["Flag"];
            string popupHeader = paramLists["PopupHeader"];
            string flagType = paramLists["FlagType"];
            string cotivitiAutoReviewValue = paramLists["CotivitiAutoReviewValue"];
            string clientAutoReviewValue = paramLists["ClientAutoReviewValue"];
            string flagDescriptionValue = paramLists["FlagDescriptionValue"];

            var flagShortDescription = _claimAction.GetEOBMessageFromDatabase(flag, "S");
            var flagLongDescription = Regex.Replace(_claimAction.GetEOBMessageFromDatabase(flag, "G"), @"\s+", " ");

            try
            {
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    Extensions.GetStringValue(ClaimQuickSearchTypeEnum.FlaggedClaims));
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flag);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.ClickOnGridByRowCol(col: 2);
                _claimAction.WaitForPageToLoad();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _flagPopup = _claimAction.ClickOnFlagandSwitch($"Flag Information - {flag}", flag);
                _flagPopup.GetPopupHeaderText().ShouldBeEqual(popupHeader, "Popup header should match");
                _flagPopup.GetTextValueinLiTag(1)
                    .ShouldBeEqual(flagShortDescription, "Short flag description should match");
                _flagPopup.GetTextValueWithInTag(tag: "").Replace("\r\n", " ")
                    .ShouldBeEqual(flagType, "Flag Type should match");
                _flagPopup.GetTextValueWithinSpanTag(4).ShouldBeEqual(cotivitiAutoReviewValue, "Cotiviti Auto review value should match");
                _flagPopup.GetTextValueWithinSpanTag(5).ShouldBeEqual(clientAutoReviewValue, "Client Auto Review value should match");
                _flagPopup.GetTextValueWithInTag(6, ">span")
                    .ShouldBeEqual(flagDescriptionValue, "Flag description label should be present");
                _flagPopup.GetTextValueWithInTag(6, "").Replace($"{flagDescriptionValue}\r\n", "")
                    .ShouldBeEqual(flagLongDescription, "Long description should match");
                _claimAction = _flagPopup.ClosePopupOnNewClaimActionPage(flag);
            }

            finally
            {
                _claimAction.CloseAnyPopupIfExist();
                _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", Extensions.GetStringValue(ClaimQuickSearchTypeEnum.AllClaims));
                _claimAction.ClickWorkListIcon();
            }

        }

        #endregion


        #region PRIVATE METHODS

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
            //_claimAction.ClickWorkListIcon();
            _claimAction.RemoveLock();
        }

        void HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(object obj)
        {
            if (obj is ClaimActionPage)
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }
        }

        void DeleteOrRestoreLineFlag(string reasonCode)
        {
            try
            {
                bool action = _claimAction.IsFlagDeletable();
                string workItem = !action ? "Delete" : "Restore";

                int expectedDeletedFlags = _claimAction.GetCountOfDeletedFlags();
                int expectedAllFlags = _claimAction.GetCountOfAllFlags();

                StringFormatter.PrintMessageTitle(workItem + " Single Flag");
                string flag = _claimAction.ClickOnFirstEditFlag(action);
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.ClickOnWorkButton(action);
                _claimAction.ClickOnSaveButton();

                if (action)
                {
                    (_claimAction.GetCountOfAllFlags() - expectedAllFlags).ShouldBeEqual(1, "Count of Restored Flags");
                }
                else
                {
                    (_claimAction.GetCountOfDeletedFlags() - expectedDeletedFlags).ShouldBeEqual(1,
                                                                                                  "Count of Deleted Flags");
                }

                StringFormatter.PrintMessage(workItem + " the single flag : " + flag + ".");
                StringFormatter.PrintLineBreak();
            }
            finally
            {
                bool isErrorModalPresent = _claimAction.IsPageErrorPopupModalPresent();
                if (isErrorModalPresent)
                {
                    _claimAction.ClosePageError();
                    this.AssertFail("Error page popup is present");
                }
            }
        }



        #endregion
    }
}