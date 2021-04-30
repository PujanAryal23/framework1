using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Common.Constants;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Diagnostics;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class ClaimSummary : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private ClaimSummaryPage _claimSummary;
        private string _originalWindowHandle = null;

        #endregion


        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override ProductEnum TestProduct
        {
            get
            {
                return ProductEnum.DCI;
            }

        }

        #endregion


        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, "ClaimSummaryTestMethod");
            _claimSummary = LoginPage
                            .Login()
                            .GoToProductPage(ProductEnum.DCI)
                            .NavigateToBatchListPage()
                            .ClickOnPageLink(param["PageNumber"])
                            .ClickOnBatchIdLink(param["BatchId"])
                            .ClickOnClaimSequence(param["ClaimSequence"]);
           _originalWindowHandle =  _claimSummary.GetCurrentHandle();
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _claimSummary;
        }

        protected override void TestCleanUp()
        {
            string currentHandle = _claimSummary.GetCurrentHandle();
            if(!currentHandle.Equals(_originalWindowHandle))
            {
                _claimSummary.CloseChildPopupHandle(currentHandle);
            }
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_ClaSum_aspx_page_opened_when_clicked_on_ClaSeq_Link()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintMessageTitle("Claim Summary");
            _claimSummary
                .CurrentPageTitle.ShouldEqual(_claimSummary.PageTitle, "Page Title");
            string pageUrl = string.Concat(_claimSummary.BaseUrl, ProductPageUrlEnum.ClaimSummaryInfo.GetStringValue());
            _claimSummary.CurrentPageUrl.Contains(pageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", pageUrl));
            StringFormatter.PrintLineBreak();
        }

        [Test]
        public void Verify_Negotiation_section_is_displayed_when_S_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            _claimSummary
                .ClickOnSButton();
            _claimSummary
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
            IList<string> expectedLabelValue =
                DataHelper.GetMappingData(FullyQualifiedClassName, "Add Edit Section Labels").Values.ToList();
            StringFormatter.PrintLineBreak();

            _claimSummary
                .ClickOnAButton();
            _claimSummary
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
                .ClickOnMButton();
            _claimSummary
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
            _claimSummary
                .ClickOnLButton();
            _claimSummary
                .IsLogicSectionPresent()
                .ShouldBeTrue("Logic Section Present");

            _claimSummary
                .ClickCloseButton();
        }

        [Test]
        public void Verify_Pend_section_is_displayed_when_P_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            _claimSummary
                .ClickOnPButton();
            _claimSummary
                .IsPendSectionPresent()
                .ShouldBeTrue("Pend Section Present");

            _claimSummary
                .GetPendSectionTitle()
                .ShouldEqual("Pend", "Pend Section Title");

            _claimSummary.ClickCloseButton();
        }

        [Test]
        public void Verify_click_on_Proc_Code_navigate_to_CodeDesc_page_and_verify_proc_description_is_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> procedureCodes = DataHelper.GetTestData(FullyQualifiedClassName,
                                                                                TestExtensions.TestName);
            for (int i = 1; i <= Convert.ToInt32(procedureCodes["NumberOfProcCode"]); i++)
            {
                string elementText;
                string descValue;
                _claimSummary.ClickOnProcCodeAndGetDescription(i, out elementText, out descValue);
                string dxCodeDescription;
                procedureCodes.TryGetValue(elementText, out dxCodeDescription);
                descValue.Trim().ToUpper().ShouldEqual(dxCodeDescription.Trim().ToUpper(),
                                                       string.Concat("Proc Code - ", elementText));
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
                providerAppeal.CurrentPageTitle.ShouldEqual(providerAppeal.PageTitle, "PageTitle",
                                                        "Page Title Mismatch Error");
            }
            finally
            {
                if (providerAppeal != null)
                {
                    _claimSummary = providerAppeal.CloseProviderAppealPopup(string.Format(PageTitleEnum.ClaimSummary.GetStringValue(), ProductEnum.DCI.GetStringValue()));
                }
            }

        }

        [Test]
        public void Verify_Original_Data_popup_appears_when_data_button_clicked_and_closed_when_X_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            bool isoriginalDataPopupPresent = true;
            string originalDataPopupHandle = null;
            OriginalDataPage originalData = null;
            try
            {
                _claimSummary.SwitchFrameToMenu();
                originalData = _claimSummary.ClickOnDataButton();
                originalData
                    .GetWindowHandle(out originalDataPopupHandle)
                    .CurrentPageTitle.ShouldEqual(originalData.PageTitle, "Page Title");

                originalData
                    .CurrentPageUrl.Contains(originalData.PageUrl).ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", originalData.CurrentPageUrl, originalData.PageUrl));

                originalData.SwitchFrameToHeader();
                _claimSummary = originalData.ClickCloseButton();
                isoriginalDataPopupPresent = _claimSummary.IsPopupPresentWithHandleName(originalDataPopupHandle);
                isoriginalDataPopupPresent.ShouldBeFalse("Original Data Popup Present");
            }
            finally
            {
                if (originalData != null && isoriginalDataPopupPresent)
                {
                    _claimSummary.CloseChildPopupHandle(originalDataPopupHandle);
                }
                StringFormatter.PrintLineBreak();
            }
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
                
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnTwelveMonthButton();
                patientHistory.GetWindowHandle(out patientHistoryHandle);
                string durationLabel = patientHistory.GetDurationLabel();
                patientHistory.CurrentPageTitle.ShouldEqual(patientHistory.PageTitle, "Page Title");
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
                
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnAllButton();
                patientHistory.GetWindowHandle(out patientHistoryHandle);
                string durationLabel = patientHistory.GetDurationLabel();
                patientHistory.CurrentPageTitle.ShouldEqual(patientHistory.PageTitle, "Page Title");
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
                
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnSameDayButton();
                patientHistory.GetWindowHandle(out patientHistoryHandle);
                string durationLabel = patientHistory.GetDurationLabel();
                patientHistory.CurrentPageTitle.ShouldEqual(patientHistory.PageTitle, "Page Title");
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
                
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnSixtyDaysButton();
                patientHistory.GetWindowHandle(out patientHistoryHandle);
                string durationLabel = patientHistory.GetDurationLabel();
                patientHistory.CurrentPageTitle.ShouldEqual(patientHistory.PageTitle, "Page Title");
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
                
                _claimSummary.SwitchFrameToMenu();
                patientHistory = _claimSummary.ClickOnProviderButton();
                patientHistory.GetWindowHandle(out patientHistoryHandle);
                string durationLabel = patientHistory.GetDurationLabel();
                patientHistory.CurrentPageTitle.ShouldEqual(patientHistory.PageTitle, "Page Title");
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
        public void Verify_Processing_History_section_and_claim_tables_displayed_when_processing_button_clicked_and_closed_when_close_btn_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            _claimSummary.SwitchFrameToMenu();
            _claimSummary.ClickOnProcessingButton();
            _claimSummary.SwitchFrameToView();
            _claimSummary
               .IsProcessingSectionDisplayed()
               .ShouldBeTrue("Processing History section displayed");
            _claimSummary.SwitchFrameToProcessingHistory();
            _claimSummary
                .GetClaimHistoryTableTitle().ShouldEqual("Claim History", "Claim History Table Title");
            _claimSummary
              .GetSystemModificationsTableTitle().ShouldEqual("System Modifications", "System Modifications Table Title");
            _claimSummary
              .GetLineModificationsTableTitle().ShouldEqual("Line Modifications", "Line Modifications Table Title");
            _claimSummary
             .GetProcessingHistoryTableTitle().ShouldEqual("Processing History", "Processing History Table Title");
            _claimSummary.CloseClaimProcessing();
            _claimSummary
               .IsProcessingSectionDisplayed()
               .ShouldBeFalse("Processing History section displayed");
            StringFormatter.PrintLineBreak();
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
                    .CurrentPageTitle.ShouldEqual(documentUpload.PageTitle, "Page Title");
                documentUpload
                    .CurrentPageUrl.Contains(documentUpload.PageUrl).ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", documentUpload.CurrentPageUrl, documentUpload.PageUrl));
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
        public void Verify_Medical_PreAuthorization_search_popup_appears_when_search_link_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintLineBreak();
            bool ispreauthorizationSearchPopupPresent = true;
            string preauthorizationSearchHandle = null;
            PreAuthorizationSearchPage preauthorizationSearch = null;
            try
            {
                preauthorizationSearch = _claimSummary.ClickOnSearchLink();
                preauthorizationSearch
                    .GetWindowHandle(out preauthorizationSearchHandle)
                    .CurrentPageTitle.ShouldEqual(preauthorizationSearch.PageTitle, "Page Title");

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
                StringFormatter.PrintLineBreak();
            }
        }
        #endregion
    }
}
