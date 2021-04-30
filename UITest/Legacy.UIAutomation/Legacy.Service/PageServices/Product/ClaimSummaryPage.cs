using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Rationale;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Rationale;
using FFPPage = Legacy.Service.PageServices.FFP;
using FFPPageObject = Legacy.Service.PageObjects.FFP;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class ClaimSummaryPage : DefaultPage
    {
        private ClaimSummaryPageObjects _claimSummaryPage;

        private IList<Link> _editLinks;

        #region CONSTRUCTOR

        public ClaimSummaryPage(INavigator navigator, ClaimSummaryPageObjects claimSummaryPage)
            : base(navigator, claimSummaryPage)
        {
            _claimSummaryPage = (ClaimSummaryPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go back to previous page
        /// </summary>
        /// <returns></returns>
        public override IPageService GoBack()
        {
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(2000);
            if (StartLegacy.Product.Equals(ProductEnum.FFP))
            {
                var ffpFlaggedClaimPage = Navigator.Navigate<FFPPageObject.FlaggedClaimPageObjects>(() => _claimSummaryPage.BackButton.Click());
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitToLoadNew(2000);
                return new FFPPage.FlaggedClaimPage(Navigator, ffpFlaggedClaimPage);
            }
            var flaggedClaimPage = Navigator.Navigate<FlaggedClaimPageObjects>(() => _claimSummaryPage.BackButton.Click());
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(2000);
            return new FlaggedClaimPage(Navigator, flaggedClaimPage);
        }

        public DocClaimListPage GoBackToDocClaimList()
        {
            Console.WriteLine("Clicked Back Button");
            var docClaimList = Navigator.Navigate<DocClaimListPageObjects>(()=> _claimSummaryPage.BackButton.Click());
            return new DocClaimListPage(Navigator, docClaimList);
        }

        #region VIEW FRAME BUTTON

        /// <summary>
        /// Click on S button
        /// </summary>
        public void ClickOnSButton()
        {
            _claimSummaryPage.SButton.Click();
            Console.WriteLine("Clicked S Button");
        }

        /// <summary>
        /// Click on A button
        /// </summary>
        /// <returns></returns>
        public void ClickOnAButton()
        {
            _claimSummaryPage.AButton.Click();
            Console.WriteLine("Clicked A Button");
        }

        /// <summary>
        /// Click M Button
        /// </summary>
        /// <returns></returns>
        public void ClickOnMButton()
        {
            _claimSummaryPage.MButton.Click();
            Console.WriteLine("Clicked M Button");
        }

        /// <summary>
        /// Click P Button
        /// </summary>
        /// <returns></returns>
        public void ClickOnPButton()
        {
            _claimSummaryPage.PButton.Click();
            Console.WriteLine("Clicked P Button");
        }

        /// <summary>
        /// Click L Button in Dci
        /// </summary>
        /// <returns></returns>
        public void ClickOnLButton()
        {
            _claimSummaryPage.LButton.Click();
            Console.WriteLine("Clicked L Button");
        }

        /// <summary>
        /// Return true if SaveButton is present
        /// </summary>
        /// <returns></returns>
        public bool IsSaveButtonPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.SaveButtonXPath, How.XPath);
        }

        /// <summary>
        /// Return true if CloseButton is present
        /// </summary>
        /// <returns></returns>
        public bool IsCloseButtonPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.CloseButtonXPath, How.XPath);
        }

        /// <summary>
        /// Click Close Button
        /// </summary>
        /// <returns></returns>
        public void ClickCloseButton()
        {
            _claimSummaryPage.CloseButton.Click();
            Console.WriteLine("Clicked Close Button");
        }

        #endregion

        #region MENU BUTTONS

        /// <summary>
        /// Click Invoice Button
        /// </summary>
        public void ClickOnInvoiceButton()
        {
            _claimSummaryPage.InvoiceButton.Click();
            Console.WriteLine("Clicked Invoice Button");
        }

        /// <summary>
        /// Click Appeal Button
        /// </summary>
        public ProviderAppealPage ClickOnAppealButton()
        {
            var providerAppealPage = Navigator.Navigate<ProviderAppealPageObjects>(() =>
                                        {
                                            _claimSummaryPage.AppealButton.Click();
                                            Console.WriteLine("Clicked Appeal Button");
                                            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAppeal.GetStringValue()));
                                        });
            return new ProviderAppealPage(Navigator, providerAppealPage);
        }

        /// <summary>
        /// Click 12 Month Button
        /// </summary>
        /// <returns></returns>
        public PatientHistoryPage ClickOnTwelveMonthButton()
        {
            var patientHistoryPage = Navigator.Navigate<PatientHistoryPageObjects>(() =>
            {
                _claimSummaryPage.TwelveMonthButton.Click();
                Console.WriteLine("Clicked 12 Month Button");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientHistory.GetStringValue()));
            });
            return new PatientHistoryPage(Navigator, patientHistoryPage);
        }

        /// <summary>
        /// Click All Button
        /// </summary>
        /// <returns></returns>
        public PatientHistoryPage ClickOnAllButton()
        {
            var patientHistoryPage = Navigator.Navigate<PatientHistoryPageObjects>(() =>
            {
                _claimSummaryPage.AllButton.Click();
                Console.WriteLine("Clicked All Button");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientHistory.GetStringValue()));
            });
            return new PatientHistoryPage(Navigator, patientHistoryPage);
        }

        /// <summary>
        /// Click Same Day Button
        /// </summary>
        /// <returns></returns>
        public PatientHistoryPage ClickOnSameDayButton()
        {
            var patientHistoryPage = Navigator.Navigate<PatientHistoryPageObjects>(() =>
            {
                
                _claimSummaryPage.SameDayButton.Click();
                Console.WriteLine("Clicked Same Day Button");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientHistory.GetStringValue()));
            });
            return new PatientHistoryPage(Navigator, patientHistoryPage);
        }

        /// <summary>
        /// Click 60 Days Button
        /// </summary>
        /// <returns></returns>
        public PatientHistoryPage ClickOnSixtyDaysButton()
        {
            var patientHistoryPage = Navigator.Navigate<PatientHistoryPageObjects>(() =>
            {
                
                _claimSummaryPage.SixtyDaysButton.Click();
                Console.WriteLine("Clicked 60 Days Button");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientHistory.GetStringValue()));
            });
            return new PatientHistoryPage(Navigator, patientHistoryPage);
        }

        /// <summary>
        /// Click Provider Button
        /// </summary>
        /// <returns></returns>
        public PatientHistoryPage ClickOnProviderButton()
        {
            var patientHistoryPage = Navigator.Navigate<PatientHistoryPageObjects>(() =>
            {
                _claimSummaryPage.ProviderButton.Click();
                Console.WriteLine("Clicked Provider Button");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientHistory.GetStringValue()));
            });
            return new PatientHistoryPage(Navigator, patientHistoryPage);
        }

        /// <summary>
        /// Click Documents Button
        /// </summary>
        /// <returns></returns>
        public DocumentUploadPage ClickOnDocumentsButton()
        {
            var documentUploadPage = Navigator.Navigate<DocumentUploadPageObjects>(() =>
            {
                _claimSummaryPage.DocumentButton.Click();
                Console.WriteLine("Clicked Documents Button");
                SiteDriver.WaitForIe(2000);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DocumentUpload.GetStringValue()));
            });
            return new DocumentUploadPage(Navigator, documentUploadPage);
        }
        #endregion

        #region NEGOTIATION SECTION

        /// <summary>
        /// Return true if Negotiation test area is present
        /// </summary>
        /// <returns></returns>
        public bool IsNegotiationTextAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.NegotiationTextAreaId, How.Id);
        }

        /// <summary>
        /// Get Negotiation Note Label
        /// </summary>
        /// <returns></returns>
        public string GetNegotiationNoteLabel()
        {
            return _claimSummaryPage.NotesTxtFld.Text.Trim();
        }

        /// <summary>
        /// Get negotiaton title
        /// </summary>
        /// <returns></returns>
        public string GetNegotiationTitle()
        {
            return  _claimSummaryPage.NegotiationTitleTxtLbl.Text;
        }

        #endregion

        #region LOGIC SECTION

        

        /// <summary>
        /// Is logic section present
        /// </summary>
        /// <returns></returns>
        public bool IsLogicSectionPresent()
        {
            return (SiteDriver.IsElementPresent("//td[@class='lightTextL']", How.XPath));
        }

        /// <summary>
        /// Get logic section title
        /// </summary>
        /// <param name="divId"></param>
        /// <returns></returns>
        public string GetLogicSectionTitle(int divId)
        {
            return
                SiteDriver.FindElement<TextLabel>(
                    string.Format(ClaimSummaryPageObjects.LogicSectionTitleXPathTemplate, divId), How.XPath).Text;
        }


        #endregion

        #region PEND SECTION

        /// <summary>
        /// Return true if Pend section is present
        /// </summary>
        /// <returns></returns>
        public bool IsPendSectionPresent()
        {
            return (SiteDriver.IsElementPresent(ClaimSummaryPageObjects.PendSectionXPath, How.XPath));

        }

        /// <summary>
        /// Get pend section title
        /// </summary>
        /// <returns></returns>
        public string GetPendSectionTitle()
        {
            return _claimSummaryPage.PendSectionTitleTxtLbl.Text;
        }

        #endregion

        #region MODIFY ALL SECTION

        /// <summary>
        /// Get title of Modify all
        /// </summary>
        /// <returns></returns>
        public string GetModifyTitle()
        {
            return _claimSummaryPage.ModifyAllTitleTxtLbl.Text;
        }

        /// <summary>
        /// Get reason label title
        /// </summary>
        /// <returns></returns>
        public string GetReasonLabelTitle()
        {
            return _claimSummaryPage.ReasonTxtLbl.Text.Trim();
        }

        /// <summary>
        /// Get notes label
        /// </summary>
        /// <returns></returns>
        public string GetNotesLabelTitle()
        {
            return _claimSummaryPage.NotesTxtLbl.Text.Trim();
        }

        /// <summary>
        /// Get delete label title
        /// </summary>
        /// <returns></returns>
        public string GetDeleteLabelTitle()
        {
            return _claimSummaryPage.DeleteTxtLbl.Text.Trim();
        }

        /// <summary>
        /// Get restore label title
        /// </summary>
        /// <returns></returns>
        public string GetRestoreLabelTitle()
        {
            return _claimSummaryPage.RestoreTxtLbl.Text.Trim();
        }

        /// <summary>
        /// Return true if reason drop down present
        /// </summary>
        /// <returns></returns>
        public bool IsReasonDropDownPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.ReasonComboBoxId, How.Id);
        }

        /// <summary>
        /// Return true if notes text area present
        /// </summary>
        /// <returns></returns>
        public bool IsNotesTextAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.NotesTextAreaId, How.Id);
        }

        /// <summary>
        /// Return true if Delete check box is present
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.DeleteCheckBoxId, How.Id);
        }

        /// <summary>
        /// Return true if restore check box is present
        /// </summary>
        /// <returns></returns>
        public bool IsRestoreCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.RestoreCheckBoxId, How.Id);
        }

        #endregion

        #region ADD EDITS SECTION

        /// <summary>
        /// Get add edit title
        /// </summary>
        /// <returns></returns>
        public string GetAddEditTitle()
        {
            return _claimSummaryPage.AddEditsTitleTxtLbl.Text;
        }

        /// <summary>
        /// Get all top label 
        /// </summary>
        /// <returns></returns>
        public IList<string> GetTopLabelCollection()
        {
            return SiteDriver.FindElements(ClaimSummaryPageObjects.LabelXPath, How.XPath, "Text").Select(x => x.Trim()).ToList();
        }

        /// <summary>
        /// Return true if add line combobox is present
        /// </summary>
        /// <returns></returns>
        public bool IsAddLineComboBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.AddLineComboBoxId, How.Id);
        }

        /// <summary>
        /// Return true if add edit combobox is present
        /// </summary>
        /// <returns></returns>
        public bool IsAddEditComboBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.AddEditComboBoxId, How.Id);
        }

        /// <summary>
        /// Return true if add sug paid text area is present
        /// </summary>
        /// <returns></returns>
        public bool IsAddSugPaidTextAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.AddSugPaidTextAreaId, How.Id);
        }

        /// <summary>
        /// Return true if add sug proc text area is present
        /// </summary>
        /// <returns></returns>
        public bool IsAddSugProcTextAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.AddSugProcTextAreaId, How.Id);
        }

        /// <summary>
        /// Get edit src label
        /// </summary>
        /// <returns></returns>
        public string GetEditSrcLabel()
        {
            return _claimSummaryPage.EditsSrcTxtLbl.Text.Trim();
        }

        /// <summary>
        /// Return true if edit src combobox is present
        /// </summary>
        /// <returns></returns>
        public bool IsEditSrcComboBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.EditSrcComboBoxId, How.Id);
        }

        /// <summary>
        /// Get notes label of add edit section
        /// </summary>
        /// <returns></returns>
        public string GetNotesLabelAddEditsSection()
        {
            return _claimSummaryPage.NotesLabelAddEditsSection.Text.Trim();
        }

        /// <summary>
        /// Return true if notes text area is present
        /// </summary>
        /// <returns></returns>
        public bool IsNotesTextAreaPresentInAddEditSection()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.NotesTextAreaAddEditSectionId, How.Id);
        }

        #endregion

        #region DXCODE/PROCCODE/EDITLINK/REVCODE

        /// <summary>
        /// Click on dxcode and get description
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void ClickOnDxCodeAndGetDescription(int index, out string key, out string value)
        {
            key = string.Empty;
            value = string.Empty;
            string xpathValue = string.Format(ClaimSummaryPageObjects.DxCodeXPathTemplate, index.ToString(CultureInfo.InvariantCulture));
            if (SiteDriver.IsElementPresent(xpathValue, How.XPath))
            {
                var webElement = SiteDriver.FindElement<Link>(xpathValue, How.XPath);
                key = webElement.Text;
                value = GetDescriptionFromCodeDesc(webElement, key);
            }
        }

        /// <summary>
        /// Click on proc code and get description
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void ClickOnProcCodeAndGetDescription(int index, out string key, out string value)
        {
            key = string.Empty;
            value = string.Empty;
            string xpathValue = string.Format(ClaimSummaryPageObjects.ProcCodeXPathTemplate, (index * 2).ToString(CultureInfo.InvariantCulture));
            if (SiteDriver.IsElementPresent(xpathValue, How.XPath))
            {
                var webElement = SiteDriver.FindElement<Link>(xpathValue, How.XPath);
                key = webElement.Text;
                value = GetDescriptionFromCodeDesc(webElement, key);
            }
        }

        /// <summary>
        /// Click on dxcode
        /// </summary>
        /// <param name="index"></param>
        /// <param name="originalWindowHandle"></param>
        public CodeDescriptionPage ClickOnDxCode(int index, out string originalWindowHandle)
        {
            originalWindowHandle = SiteDriver.CurrentWindowHandle;
            string xpathValue = string.Format(ClaimSummaryPageObjects.DxCodeXPathTemplate, index.ToString(CultureInfo.InvariantCulture));
            if (SiteDriver.IsElementPresent(xpathValue, How.XPath))
            {
                var element = SiteDriver.FindElement<Link>(xpathValue, How.XPath);

                string requiredHandle = string.Empty;
                element.Click();
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitToLoadNew(1000);
                Console.WriteLine("Clicked on " + element.Text);
                foreach (var currentWindowHandle in SiteDriver.WindowHandles)
                {
                    if (currentWindowHandle != originalWindowHandle)
                    {
                        requiredHandle = currentWindowHandle;
                        break;
                    }
                }
                SiteDriver.SwitchWindow(requiredHandle);
                return new CodeDescriptionPage(Navigator, new CodeDescriptionPageObjects());
            }
            return null;
        }

        /// <summary>
        /// Click on proc code
        /// </summary>
        /// <param name="index"></param>
        /// <param name="originalWindowHandle"></param>
        public CodeDescriptionPage ClickOnProcCode(int index, out string originalWindowHandle)
        {
            originalWindowHandle = SiteDriver.CurrentWindowHandle;
            string xpathValue = string.Format(ClaimSummaryPageObjects.ProcCodeXPathTemplate, (index * 2).ToString(CultureInfo.InvariantCulture));
            if (SiteDriver.IsElementPresent(xpathValue, How.XPath))
            {
                var element = SiteDriver.FindElement<Link>(xpathValue, How.XPath);

                string requiredHandle = string.Empty;
                element.Click();
                Console.WriteLine("Clicked on " + element.Text);
                foreach (var currentWindowHandle in SiteDriver.WindowHandles)
                {
                    if (currentWindowHandle != originalWindowHandle)
                    {
                        requiredHandle = currentWindowHandle;
                        break;
                    }
                }
                SiteDriver.SwitchWindow(requiredHandle);
                return new CodeDescriptionPage(Navigator, new CodeDescriptionPageObjects());
            }
            return null;
        }

        /// <summary>
        /// Click on rev code
        /// </summary>
        /// <param name="index"></param>
        /// <param name="originalWindowHandle"></param>
        public RevDescriptionPage ClickOnRevCode(int index, out string originalWindowHandle)
        {
            originalWindowHandle = SiteDriver.CurrentWindowHandle;
            string xpathValue = string.Format(ClaimSummaryPageObjects.RevCodeXPathTemplate, (index * 2).ToString(CultureInfo.InvariantCulture));
            if (SiteDriver.IsElementPresent(xpathValue, How.XPath))
            {
                var element = SiteDriver.FindElement<Link>(xpathValue, How.XPath);
               
                string requiredHandle = string.Empty;
                element.Click();
                
                Console.WriteLine("Clicked on " + element.Text);
                foreach (var currentWindowHandle in SiteDriver.WindowHandles)
                {
                    if (currentWindowHandle != originalWindowHandle)
                    {
                        requiredHandle = currentWindowHandle;
                        break;
                    }
                }
                SiteDriver.SwitchWindow(requiredHandle);
                
                return new RevDescriptionPage(Navigator, new RevDescriptionPageObjects());
            }
            return null;
        }

        /// <summary>
        /// Get description
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementText"></param>
        private string GetDescriptionFromCodeDesc(Link element, string elementText)
        {
            string originalWindowHandle = SiteDriver.CurrentWindowHandle;
            string requiredHandle = string.Empty;
            element.Click();
            Console.WriteLine("Clicked on " + elementText);
            SiteDriver.WaitForIe(2000);
            foreach (var currentWindowHandle in SiteDriver.WindowHandles)
            {
                if (currentWindowHandle != originalWindowHandle)
                {
                    requiredHandle = currentWindowHandle;
                    break;
                }
            }
            SiteDriver.SwitchWindow(requiredHandle);
            var codeDescription = new CodeDescriptionPage(Navigator, new CodeDescriptionPageObjects());
            string descValue = codeDescription.GetDescriptionValue();
            SiteDriver.CloseWindowAndSwitchTo(originalWindowHandle);
            SiteDriver.SwitchFrame("View");
            return descValue;
        }

        /// <summary>
        /// Click on edit link and get description
        /// </summary>
        /// <returns></returns>
        public void ClickOnEditLinkAndGetDescription(string index, out string key, out string value)
        {
            var editLink = SiteDriver.FindElement<Link>(string.Format(ClaimSummaryPageObjects.EditLinkXPathTemplate, index), How.XPath);
            key = editLink.Text;
            value = GetDescriptionFromEditDesc(editLink, key);
        }

        /// <summary>
        /// Click on edit link
        /// </summary>
        /// <param name="index"></param>
        /// /// <param name="originalWindowHandle"></param>
        /// <returns></returns>
        public ViewRationalePage ClickOnEditsLink(string index, out string originalWindowHandle)
        {
            originalWindowHandle = SiteDriver.CurrentWindowHandle;
            SiteDriver.FindElement<Link>(string.Format(ClaimSummaryPageObjects.EditLinkXPathTemplate, index), How.XPath).Click();
            Console.WriteLine("Clicked on Edit Link");
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ViewRationale.GetStringValue()));
            return new ViewRationalePage(Navigator, new ViewRationalePageObjects());
        }

        /// <summary>
        /// Get Edit links
        /// </summary>
        /// <returns></returns>
        public int GetEditLinksCount()
        {
            return SiteDriver.FindElementsCount(ClaimSummaryPageObjects.EditLinkXPath, How.XPath);
        }

        public void PageDown()
        {
            SiteDriver.FindElement<TextLabel>("//span[@class='LargeTitle']", How.XPath).PageDown();
        }

        /// <summary>
        /// Get description of edit link
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementText"></param>
        private string GetDescriptionFromEditDesc(Link element, string elementText)
        {
            string originalWindowHandle = SiteDriver.CurrentWindowHandle;
            element.Click();
            Console.WriteLine("Clicked on " + elementText);
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ViewRationale.GetStringValue()));
            var codeDescription = new ViewRationalePage(Navigator, new ViewRationalePageObjects());
            string elementDesc = codeDescription.GetDescriptionValue();
            SiteDriver.CloseWindowAndSwitchTo(originalWindowHandle);
            SiteDriver.SwitchFrame("View");
            return elementDesc;
        }

        #endregion

        #region INVOICESECTION

        /// <summary>
        /// Get invoice table title
        /// </summary>
        /// <returns></returns>
        public string GetInvoiceTableTitle()
        {
            return _claimSummaryPage.InvoiceTitleTxtLbl.Text;
        }

        /// <summary>
        /// Get invoice header
        /// </summary>
        /// <returns></returns>
        public string GetInvoiceHeader()
        {
            return  _claimSummaryPage.InvoiceHeaderTxtLbl.Text.Contains("Group:")? "Group:": " ";
        }

        /// <summary>
        /// Get invoice table label
        /// </summary>
        /// <returns></returns>
        public IList<string> GetInvoiceTableLabels()
        {
            return SiteDriver.FindElements(ClaimSummaryPageObjects.InvoiceLabelsXPath, How.XPath, "Text");
        }

        /// <summary>
        /// Get invoice table white labels
        /// </summary>
        /// <returns></returns>
        public IList<string> GetInvoiceTableLightTextLabels()
        {
            return SiteDriver.FindElements(ClaimSummaryPageObjects.InvoiceLightTextLabelsXPath, How.XPath, "Text").Where(x => x != " ").Select(
                    x => x.Replace("\r\n", " ")).ToList();
        }

        /// <summary>
        /// Return true is invoice button is present
        /// </summary>
        /// <returns></returns>
        public bool IsInvoiceCloseButtonPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSummaryPageObjects.InvoiceCloseButtonXPath, How.XPath);
        }

        /// <summary>
        /// Click close button
        /// </summary>
        public void ClickInvoiceCloseButton()
        {
            _claimSummaryPage.InvoiceCloseButton.Click();
            SwitchFrameToView();
        }

        #endregion

        #region PROCESSING HISTORY

        /// <summary>
        /// Click Processing Button
        /// </summary>
        public void ClickOnProcessingButton()
        {
            _claimSummaryPage.ProcessingLink.Click();
            Console.WriteLine("Clicked Processing Button");
        }

        /// <summary>
        /// Get claim history table title
        /// </summary>
        /// <returns></returns>
        public string GetClaimHistoryTableTitle()
        {
            return _claimSummaryPage.ClaimHistoryTableTitle.Text;
        }

        /// <summary>
        /// Get system modifications table title
        /// </summary>
        /// <returns></returns>
        public string GetSystemModificationsTableTitle()
        {
            return _claimSummaryPage.SystemModificationsTableTitle.Text;
        }

        /// <summary>
        /// Get processing history table title
        /// </summary>
        /// <returns></returns>
        public string GetProcessingHistoryTableTitle()
        {
            return _claimSummaryPage.ProcessingHistoryTableTitle.Text;
        }

        /// <summary>
        /// Get line modifications table title
        /// </summary>
        /// <returns></returns>
        public string GetLineModificationsTableTitle()
        {
            return _claimSummaryPage.LineModificationsTableTitle.Text;
        }

        public bool IsProcessingSectionDisplayed()
        {
            return (Convert.ToInt32(_claimSummaryPage.ProcessingSectionFrame.GetAttribute("height")) != 0);
        }

        /// <summary>
        /// Close processing section
        /// </summary>
        public void CloseClaimProcessing()
        {
            _claimSummaryPage.ProcessingCloseButton.Click();
            SwitchFrameToView();
        }

        #endregion

        #region GENERAL METHODS

        /// <summary>
        /// Get claim sequence
        /// </summary>
        /// <returns></returns>
        public string GetClaimSequence()
        {
            return _claimSummaryPage.ClaimSequenceTextLabel.Text.Substring(7);
        }

        public string GetBatchId()
        {
            return _claimSummaryPage.BatchIdTextLabel.Text.Substring(6);
        }

       

        /// <summary>
        /// Close child popup handle 
        /// </summary>
        /// <param name="popuphandle"></param>
        /// <returns></returns>
        public void CloseChildPopupHandle(string popuphandle)
        {
            Navigator.Navigate<ClaimSummaryPageObjects>(() => ClosePopupAndSwitchToOriginalHandle(popuphandle));
            SiteDriver.SwitchFrame("View");
        }

        /// <summary>
        /// Switch frame to menu
        /// </summary>
        public void SwitchFrameToMenu()
        {
            
            SiteDriver.CloseFrameAndSwitchTo("Menu");
            
        }

        /// <summary>
        /// Switch frame to invoice
        /// </summary>
        public void SwitchFrameToInvoice()
        {
            SwitchFrameToView();
            SiteDriver.SwitchFrame("invoice");
        }

        /// <summary>
        /// Switch frame to processing
        /// </summary>
        public void SwitchFrameToProcessingHistory()
        {
            SwitchFrameToView();
            SiteDriver.SwitchFrame("history");
            Func<bool> f= delegate()
                {
                    if (SiteDriver.IsElementPresent(ClaimSummaryPageObjects.SystemModificationsTableTitleXPath,
                        How.XPath)) return true;
                    SwitchFrameToView();
                    SiteDriver.SwitchFrame("history");
                    return false;
                };
            
           SiteDriver.WaitForCondition(f);
        }

        /// <summary>
        /// Switch to view frame
        /// </summary>
        public void SwitchFrameToView()
        {
            SiteDriver.CloseFrameAndSwitchTo("View");
        }

        public string GetCurrentHandle()
        {
            return SiteDriver.CurrentWindowHandle;
        }

        #endregion

        /// <summary>
        /// Click Search link
        /// </summary>
        /// <returns></returns>
        public PreAuthorizationSearchPage ClickOnSearchLink()
        {
            var preauthorizationSearchPage = Navigator.Navigate<PreAuthorizationSearchPageObjects>(() =>
             {
                 _claimSummaryPage.SearchLink.Click();
                 Console.WriteLine("Clicked on Search Link");
                 SiteDriver.CloseModalPopup();
                 SiteDriver.SwitchWindow(ClaimSummaryPageObjects.PreAuthorizationSearchWindowName);
             });
            return new PreAuthorizationSearchPage(Navigator, preauthorizationSearchPage);
        }

        /// <summary>
        /// Click Data Btn
        /// </summary>
        /// <returns></returns>
        public OriginalDataPage ClickOnDataButton()
        {
            var originalDataPage = Navigator.Navigate<OriginalDataPageObjects>(() =>
            {
                _claimSummaryPage.DataButton.Click();
                Console.WriteLine("Clicked on Data Btn");
                SiteDriver.WaitForIe(2000);
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.OriginalData.GetStringValue());
            });
            return new OriginalDataPage(Navigator, originalDataPage);
        }
        #endregion

    }
}
