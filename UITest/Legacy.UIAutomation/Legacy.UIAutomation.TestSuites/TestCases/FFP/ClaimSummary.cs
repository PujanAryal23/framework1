using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Environment;
using UIAutomation.Framework.Common.Constants;
using FFPPage = Legacy.Service.PageServices.FFP;
using Legacy.Service.Support.Common.Constants;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Support.Enum;
using Legacy.Service.Data;

namespace Legacy.UIAutomation.TestSuites.TestCases.FFP
{
    [Category("FFP")]
    public class ClaimSummary : AutomatedBase
    {
        #region PRIVATE FIELDS

        private ClaimSummaryPage _claimSummary;
        private string _originalWindowHandle = null;
        private bool _isPostBack;
        private string _originalClaimSequence;
        private string _originalBatchId;
        private string _originalPageNo;

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
                return ProductEnum.FFP;
            }

        }

        #endregion


        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, "ClaimSummaryTestMethod");
            _originalClaimSequence = param["ClaimSequence"];
            _originalBatchId = param["BatchId"];
            _originalPageNo = param["PageNumber"];
            _claimSummary = LoginPage
                    .Login()
                    .GoToFraudFinderPro()
                    .NavigateToBatchListPage()
                    .ClickOnPageLink(_originalPageNo,out _isPostBack)
                    .ClickOnBatchIdLink(_originalBatchId).ClickOnClaimSequence(_originalClaimSequence);
            _originalWindowHandle = _claimSummary.GetCurrentHandle();
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _claimSummary;
        }

        protected override void TestCleanUp()
        {
            string currentHandle = _claimSummary.GetCurrentHandle();
            if (!currentHandle.Equals(_originalWindowHandle))
            {
                _claimSummary.CloseChildPopupHandle(currentHandle);
            }
           
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_Click_on_Edit_link_navigate_to_View_Rationale_apsx()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            _claimSummary = ((FFPPage.BatchListPage)((FFPPage.FlaggedClaimPage)_claimSummary.GoBack()).GoBack()).ClickOnPageLink(param["PageNumber"], out _isPostBack).ClickOnBatchIdLink(param["BatchId"]).ClickOnClaimSequence(param["ClaimSequence"]);
            if (string.Compare(EnvironmentManager.Instance.Browser, BrowserConstants.Iexplorer, StringComparison.OrdinalIgnoreCase) == 0)
           _claimSummary.PageDown();
            int count = 1;
            int editLinksCount = _claimSummary.GetEditLinksCount();
            for (int i = 1; i <= editLinksCount; i++)
            {
                if (count > 1)
                    break;
                StringFormatter.PrintMessageTitle("Edit Links");
                string windowHandle;
                var viewRationale = _claimSummary.ClickOnEditsLink(param["FRE"], out windowHandle);
                viewRationale.CurrentPageTitle.ShouldEqual(viewRationale.PageTitle, "Page Title");
                viewRationale.CurrentPageUrl.Contains(viewRationale.PageUrl)
                    .ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", viewRationale.CurrentPageUrl, viewRationale.PageUrl));
                StringFormatter.PrintLineBreak();
                _claimSummary = viewRationale.CloseViewRationalePage(windowHandle);
                count++;
            }
            _claimSummary = ((FFPPage.BatchListPage)((FFPPage.FlaggedClaimPage)_claimSummary.GoBack()).GoBack()).ClickOnPageLink(_originalPageNo, out _isPostBack).ClickOnBatchIdLink(_originalBatchId).ClickOnClaimSequence(_originalClaimSequence);
        }

        [Test]
        public void Verify_Click_on_Rev_code_navigate_to_RevDesc_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            int revCodes = Convert.ToInt32(DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName)["NumberOfRevCode"]);
             StringFormatter.PrintMessageTitle("Rev Code");
                string windowHandle;
                var revDesc = _claimSummary.ClickOnRevCode(1, out windowHandle);
                revDesc.CurrentPageTitle.ShouldEqual(revDesc.PageTitle, "Page Title");
                revDesc.CurrentPageUrl.Contains(revDesc.PageUrl)
                    .ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", revDesc.CurrentPageUrl, revDesc.PageUrl));
                StringFormatter.PrintLineBreak();
                _claimSummary = revDesc.CloseRevDesc(windowHandle);
           
        }

        [Test]
        public void Verify_Click_on_Proc_Code_navigate_to_CodeDesc_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> procCodes = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            int procCodesNumber = Convert.ToInt32(procCodes["NumberOfProcCode"]);

            for (int i = 1; i <= procCodesNumber; i++)
            {
                string windowHandle;
                var procCode = _claimSummary.ClickOnProcCode(i, out windowHandle);
                procCode.CurrentPageTitle.ShouldEqual(procCode.PageTitle, "Page Title");
                procCode.CurrentPageUrl.Contains(procCode.PageUrl)
                    .ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", procCode.CurrentPageUrl, procCode.PageUrl));
                StringFormatter.PrintLineBreak();
                _claimSummary = procCode.CloseCodeDesc(windowHandle);
            }
         
        }

        [Test]
        public void Verify_Click_on_Dx_code_navigate_to_CodeDesc_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            int diagnosisCodes = Convert.ToInt32(DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName)["NumberOfDxCode"]);
            for (int i = 2; i < (diagnosisCodes + 2); i++)
            {
                string windowHandle;
                var dxCode = _claimSummary.ClickOnDxCode(i, out windowHandle);
                dxCode.CurrentPageTitle.ShouldEqual(dxCode.PageTitle, "Page Title");
                dxCode.CurrentPageUrl.Contains(dxCode.PageUrl)
                    .ShouldBeTrue(string.Format("Page Url '{0}' contains '{1}'", dxCode.CurrentPageUrl, dxCode.PageUrl));
                StringFormatter.PrintLineBreak();
                _claimSummary = dxCode.CloseCodeDesc(windowHandle);
            }
        }
        
        [Test]
        public void Verify_Negotiation_section_is_displayed_when_S_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string claimSeq = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName)["ClaimSequence"];
            string originalClaSeq = _claimSummary.GetClaimSequence();
            _claimSummary = ((FFPPage.FlaggedClaimPage)_claimSummary.GoBack()).ClickOnClaimSequence(claimSeq);
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
            _claimSummary = ((FFPPage.FlaggedClaimPage)_claimSummary.GoBack()).ClickOnClaimSequence(originalClaSeq);
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
        public void Verify_Clicking_on_Appeal_Button_opens_provider_appeal_Popup()
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

        #endregion
    }
}
