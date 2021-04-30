using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Common.Constants;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases
{
    public class ClaimSummaryTest : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private ClaimSummaryPage _claimSummary;

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

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, "ClaimSummaryTestMethod");
            _claimSummary = LoginPage.Login().GoToPhysicianClaimInsight().NavigateToBatchList().ClickOnBatchIdLink(
                    param["BatchId"]).ClickOnClaimSequence(param["ClaimSequence"]);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _claimSummary;
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_Negotiation_section_is_displayed_when_S_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();

            _claimSummary
                .ClickOnSButton()
            .GetNegotiationTitle()
            .ShouldEqual("Negotiation", "Negotiation Section Title:", true);

            _claimSummary
                .GetNegotiationNoteLabel()
                .ShouldEqual("Notes:", "Label");

            _claimSummary
                .IsNegotiationTextAreaPresent()
                .ShouldBeTrue("Negotiation Text Area Present");

            _claimSummary
                .IsSaveButtonPresent()
                .ShouldBeTrue("Save Button Present");

            _claimSummary
                .IsCloseButtonPresent()
                .ShouldBeTrue("Close Button Present");

            _claimSummary
                .ClickCloseButton();
        }

        [Test]
        public void Verify_Add_Edit_section_is_displayed_when_A_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedLabelValue = DataHelper.GetMappingData(FullyQualifiedClassName, "Add Edit Section Labels").Values.ToList();
            StringFormatter.PrintLineBreak();

            _claimSummary
                .ClickOnAButton()
            .GetAddEditTitle()
            .ShouldEqual("Add Edit", "Add Edit Section Title:", true);

            _claimSummary
                .GetTopLabelCollection()
                .ShouldCollectionEqual(expectedLabelValue, "Labels:");
            _claimSummary.IsAddLineComboBoxPresent().ShouldBeTrue("Add Line ComboxBox Present");

            _claimSummary
                .IsAddEditComboBoxPresent()
                .ShouldBeTrue("Add Edit ComboBox Present");

            _claimSummary
                .IsAddSugPaidTextAreaPresent()
                .ShouldBeTrue("Add-Sug Paid Text Area Present");

            _claimSummary
                .IsAddSugProcTextAreaPresent()
                .ShouldBeTrue("Add-Sug Proc Text Area Present");

            _claimSummary
                .GetEditSrcLabel()
                .ShouldEqual("EditSrc:", "Label:");

            _claimSummary
                .IsEditSrcComboBoxPresent()
                .ShouldBeTrue("Edit Src ComboxBox Present");

            _claimSummary
                .GetNotesLabelAddEditsSection()
                .ShouldEqual("Notes:", "Label:");

            _claimSummary
                .IsNotesTextAreaPresentInAddEditSection()
                .ShouldBeTrue("Notes Text Area Present");

            _claimSummary
                .IsSaveButtonPresent()
                .ShouldBeTrue("Save Button Present");

            _claimSummary
                .IsCloseButtonPresent()
                .ShouldBeTrue("Close Button Present");

            _claimSummary
                .ClickCloseButton();
        }

        [Test]
        public void Verify_Modify_section_is_dispalyed_when_M_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();

            _claimSummary
                .ClickOnMButton()
                .GetModifyTitle()
                .ShouldEqual("Modify All", "Modify All Section Title", true);

            _claimSummary
                .GetReasonLabelTitle()
                .ShouldEqual("Reason:", "Label");

            _claimSummary
                .IsReasonDropDownPresent()
                .ShouldBeTrue("Reason Drop Down Present");

            _claimSummary
                .GetDeleteLabelTitle()
                .ShouldEqual("Delete:", "Label");

            _claimSummary
                .IsDeleteCheckBoxPresent()
                .ShouldBeTrue("Delete Check Box Present");

            _claimSummary
                .GetRestoreLabelTitle()
                .ShouldEqual("Restore:", "Label");

            _claimSummary
                .IsRestoreCheckBoxPresent()
                .ShouldBeTrue("Restore Check Box Present");

            _claimSummary
                .GetNotesLabelTitle()
                .ShouldEqual("Notes:", "Label");

            _claimSummary
                .IsNotesTextAreaPresent()
                .ShouldBeTrue("Modify All Section Text Area Present");

            _claimSummary
                .IsSaveButtonPresent()
                .ShouldBeTrue("Save Button Present");

            _claimSummary
                .IsCloseButtonPresent()
                .ShouldBeTrue("Close Button Present");

            _claimSummary
                .ClickCloseButton();
        }

        [Test]
        public void Verify_Logic_section_is_displayed_when_L_button_is_clicked()
        {
            StringFormatter.PrintLineBreak();
            int logicSectionDivId;
            _claimSummary.PageDown();
            _claimSummary
                .ClickOnLButtonAndGetDivId(out logicSectionDivId)
                .IsLogicSectionPresent(logicSectionDivId)
                .ShouldBeTrue("Logic Section Present");

            _claimSummary
                .GetLogicSectionTitle(logicSectionDivId)
                .ShouldEqual("Logic", "Logic Section Title");

            _claimSummary
                .ClickCloseButton();
        }

        [Test]
        public void Verify_click_on_Dx_code_navigate_to_CodeDesc_page_and_verify_diag_description_is_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> diagnosisCodes = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            for (int i = 2; i < (Convert.ToInt32(diagnosisCodes["NumberOfDxCode"]) + 2); i++)
            {
                string elementText;
                string descValue;
                _claimSummary.ClickOnDxCodeAndGetDescription(i, out elementText, out descValue);
                string dxCodeDescription;
                diagnosisCodes.TryGetValue(elementText, out dxCodeDescription);
                descValue.Trim().ToUpper().ShouldEqual(dxCodeDescription.Trim().ToUpper(), string.Concat("Dx Code - ", elementText));
            }
        }

        [Test]
        public void Verify_click_on_Proc_Code_navigate_to_CodeDesc_page_and_verify_proc_description_is_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> procedureCodes = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            for (int i = 1; i <= Convert.ToInt32(procedureCodes["NumberOfProcCode"]); i++)
            {
                string elementText;
                string descValue;
                _claimSummary.ClickOnProcCodeAndGetDescription(i, out elementText, out descValue);
                string dxCodeDescription;
                procedureCodes.TryGetValue(elementText, out dxCodeDescription);
                descValue.Trim().ToUpper().ShouldEqual(dxCodeDescription.Trim().ToUpper(), string.Concat("Proc Code - ", elementText));
            }
        }

        [Test]
        public void Verify_click_on_Edit_link_navigate_to_edit_popup_and_verify_edit_description_is_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> editDescription = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            _claimSummary.PageDown();
            for (int i = 0; i < _claimSummary.GetEditLinksCount(); i++)
            {
                string elementText;
                string descValue;

                _claimSummary.ClickOnEditLinkAndGetDescription(i, out elementText, out descValue);
                string editDescriptionParam;
                editDescription.TryGetValue(elementText, out editDescriptionParam);
                descValue.Trim().ToUpper().ShouldEqual(editDescriptionParam.Trim().ToUpper(),
                                                           string.Concat("Edit Link: ", elementText));
            }
        }

        [Test]
        public void Verify_HCI_invoice_table_is_displayed_when_Invoice_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedList = DataHelper.GetMappingData(FullyQualifiedClassName, "HCI Invoice Dark Text Labels").Values.ToList();
            IList<string> expectedListLightTextLabels = DataHelper.GetMappingData(FullyQualifiedClassName, "HCI Invoice Light Text Labels").Values.ToList();

            _claimSummary.SwitchFrameToMenu();
            _claimSummary.ClickOnInvoiceButton();
            _claimSummary.SwitchFrameToInvoice();

            StringFormatter.PrintLineBreak();

            _claimSummary
                .GetInvoiceTableTitle()
                .ShouldEqual("HCI Invoice", "Invoice Table Title");

            _claimSummary
                .GetInvoiceHeader()
                .ShouldEqual("Group:", "Invoice Table Header");

            _claimSummary
                .GetInvoiceTableLabels()
                .ShouldCollectionEqual(expectedList, "Invoice Table Labels");

            _claimSummary
                .GetInvoiceTableLightTextLabels()
                .ShouldCollectionEqual(expectedListLightTextLabels, "Invoice Table Light Text Labels");

            _claimSummary
                .IsInvoiceCloseButtonPresent()
                .ShouldBeTrue("Invoice Close Button Present");

            _claimSummary
                .ClickInvoiceCloseButton();
        }

        [Test]
        public void Verify_Patient_History_with_12_Months_as_duration_label_Month_popup_appeared_when_12_Month_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string patientHistoryHandle = string.Empty;
            bool isPatientHistoryPresent = true;
            PatientHistoryPage patientHistory = null;
            try
            {
                string durationLabel;
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnTwelveMonthButton();
                patientHistory
                    .GetWindowHandle(out patientHistoryHandle)
                    .GetDurationLabel(out durationLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PatientHistory.GetStringValue(), "Page Title");
                durationLabel.ShouldEqual("[12 Months]", "Duration Label", true);
                _claimSummary = patientHistory.ClickCloseButton();
                isPatientHistoryPresent = _claimSummary.IsPopupPresentWithHandleName(patientHistoryHandle);
                isPatientHistoryPresent.ShouldBeFalse("Patient History Popup Present");
            }
            finally
            {
                if (patientHistory != null && isPatientHistoryPresent)
                {
                    _claimSummary.CloseChildPopupHandle(patientHistoryHandle);
                }
            }
        }

        [Test]
        public void Verify_Patient_History_with_All_as_duration_label_popup_appeared_when_All_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string patientHistoryHandle = string.Empty;
            bool isPatientHistoryPresent = true;
            PatientHistoryPage patientHistory = null;
            try
            {
                string durationLabel;
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnAllButton();
                patientHistory
                    .GetWindowHandle(out patientHistoryHandle)
                    .GetDurationLabel(out durationLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PatientHistory.GetStringValue(), "Page Title");
                durationLabel.ShouldEqual("[All]", "Duration Label", true);
                _claimSummary = patientHistory.ClickCloseButton();
                isPatientHistoryPresent = _claimSummary.IsPopupPresentWithHandleName(patientHistoryHandle);
                isPatientHistoryPresent.ShouldBeFalse("Patient History Popup Present");
            }
            finally
            {
                if (patientHistory != null && isPatientHistoryPresent)
                {
                    _claimSummary.CloseChildPopupHandle(patientHistoryHandle);
                }
            }
        }

        [Test]
        public void Verify_Patient_History_with_Same_Day_as_duration_label_popup_appeared_when_Same_Day_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string patientHistoryHandle = string.Empty;
            bool isPatientHistoryPresent = true;
            PatientHistoryPage patientHistory = null;
            try
            {
                string durationLabel;
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnSameDayButton();
                patientHistory
                    .GetWindowHandle(out patientHistoryHandle)
                    .GetDurationLabel(out durationLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PatientHistory.GetStringValue(), "Page Title");
                durationLabel.ShouldEqual("[Same Day]", "Duration Label", true);
                _claimSummary = patientHistory.ClickCloseButton();
                isPatientHistoryPresent = _claimSummary.IsPopupPresentWithHandleName(patientHistoryHandle);
                isPatientHistoryPresent.ShouldBeFalse("Patient History Popup Present");
            }
            finally
            {
                if (patientHistory != null && isPatientHistoryPresent)
                {
                    _claimSummary.CloseChildPopupHandle(patientHistoryHandle);
                }
            }
        }

        [Test]
        public void Verify_Patient_History_with_60_Day_as_duration_label_popup_appeared_when_60_Days_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string patientHistoryHandle = string.Empty;
            bool isPatientHistoryPresent = true;
            PatientHistoryPage patientHistory = null;
            try
            {
                string durationLabel;
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnSixtyDaysButton();
                patientHistory
                    .GetWindowHandle(out patientHistoryHandle)
                    .GetDurationLabel(out durationLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PatientHistory.GetStringValue(), "Page Title");
                durationLabel.ShouldEqual("[60 Day]", "Duration Label", true);
                _claimSummary = patientHistory.ClickCloseButton();
                isPatientHistoryPresent = _claimSummary.IsPopupPresentWithHandleName(patientHistoryHandle);
                isPatientHistoryPresent.ShouldBeFalse("Patient History Popup Present");
            }
            finally
            {
                if (patientHistory != null && isPatientHistoryPresent)
                {
                    _claimSummary.CloseChildPopupHandle(patientHistoryHandle);
                }
            }
        }

        [Test]
        public void Verify_Patient_History_with_Provider_as_identifier_popup_appeared_when_Provider_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string patientHistoryHandle = string.Empty;
            bool isPatientHistoryPresent = true;
            PatientHistoryPage patientHistory = null;
            try
            {
                string durationLabel;
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnProviderButton();
                patientHistory
                    .GetWindowHandle(out patientHistoryHandle)
                    .GetDurationLabel(out durationLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PatientHistory.GetStringValue(), "Page Title");
                durationLabel.ShouldEqual("[Provider]", "Duration Label", true);
                _claimSummary = patientHistory.ClickCloseButton();
                isPatientHistoryPresent = _claimSummary.IsPopupPresentWithHandleName(patientHistoryHandle);
                isPatientHistoryPresent.ShouldBeFalse("Patient History Popup Present");
            }
            finally
            {
                if (patientHistory != null && isPatientHistoryPresent)
                {
                    _claimSummary.CloseChildPopupHandle(patientHistoryHandle);
                }
            }
        }

        [Test]
        public void Verify_Document_Upload_popup_appeared_when_Documents_button_is_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string documentUploadHandle = string.Empty;
            bool isDocumentUploadPresent = true;
            DocumentUploadPage documentUpload = null;
            try
            {
                _claimSummary.SwitchFrameToMenu();
                documentUpload = _claimSummary.ClickOnDocumentsButton();
                documentUpload
                    .GetWindowHandle(out documentUploadHandle)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.DocumentUpload.GetStringValue(), "Page Title");
                _claimSummary = documentUpload.ClickCloseButton();
                isDocumentUploadPresent = _claimSummary.IsPopupPresentWithHandleName(documentUploadHandle);
                isDocumentUploadPresent.ShouldBeFalse("Document Upload Popup Present");
            }
            finally
            {
                if (documentUpload != null && isDocumentUploadPresent)
                {
                    _claimSummary.CloseChildPopupHandle(documentUploadHandle);
                }
            }
        }

        [Test]
        public void Verify_clicking_on_Appeal_Button_opens_provider_appeal_Popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimSummary.SwitchFrameToMenu();
            ProviderAppealPage providerAppeal = null;
            try
            {
                providerAppeal = _claimSummary.ClickOnAppealButton();
                providerAppeal.GetPageTitle().ShouldEqual(PageTitleEnum.ProviderAppeal.GetStringValue(), "PageTitle",
                                                        "Page Title Mismatch Error");
            }
            finally
            {
                if (providerAppeal != null)
                {
                    _claimSummary = providerAppeal.CloseProviderAppealPopup(PageTitleEnum.ClaimSummary);
                }
            }

        }


        [Test]
        public void Verify_Medical_PreAuthorization_search_popup_appears_when_search_link_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            bool ispreauthorizationSearchPopupPresent = true;
            string preauthorizationSearchHandle = null;
            PreAuthorizationSearchPage preauthorizationSearch = null;
            try
            {
                _claimSummary.SwitchFrameToView();
                preauthorizationSearch = _claimSummary.ClickOnSearchLink();
                string titleLabel;
                preauthorizationSearch
                    .GetWindowHandle(out preauthorizationSearchHandle)
                    .GetTitleLabel(out titleLabel)
                    .GetPageTitle().ShouldEqual(PageTitleEnum.PreAuthorizationSearch.GetStringValue(), "Page Title");

                _claimSummary = preauthorizationSearch.ClickCloseButton();
                ispreauthorizationSearchPopupPresent = _claimSummary.IsPopupPresentWithHandleName(preauthorizationSearchHandle);
                ispreauthorizationSearchPopupPresent.ShouldBeFalse("PreAuthorization Popup Present");
            }
            finally
            {
                if (preauthorizationSearch != null && ispreauthorizationSearchPopupPresent)
                {
                    _claimSummary.CloseChildPopupHandle(preauthorizationSearchHandle);
                }
            }
        }


        [Test]
        public void Verify_Processing_History_popup_appears_when_processing_button_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            
            _claimSummary.SwitchFrameToMenu();
            _claimSummary.ClickOnProcessingButton();
            _claimSummary.SwitchFrameToView();
            StringFormatter.PrintLineBreak();
            _claimSummary
               .IsProcessingSectionDisplayed()
               .ShouldBeTrue("Claim History Table Displayed");
            _claimSummary.SwitchFrameToProcessingHistory();
                
            _claimSummary
                .GetSystemModificationsTableTitle()
                .ShouldEqual("System Modifications", "System Modifications Table Title");
            _claimSummary
               .GetLineModificationsTableTitle()
               .ShouldEqual("Line Modifications", "Line Modifications Table Title");
            _claimSummary
              .GetProcessingHistoryTableTitle()
              .ShouldEqual("Processing History", "Processing History Table Title");
            _claimSummary.CloseClaimProcessing();

            _claimSummary
                 .IsProcessingSectionDisplayed()
                 .ShouldBeFalse("Claim History Table Displayed");
            StringFormatter.PrintLineBreak();
        }


        #endregion

    }
}
