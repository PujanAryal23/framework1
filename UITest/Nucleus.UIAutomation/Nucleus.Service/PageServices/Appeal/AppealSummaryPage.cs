using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Appeal;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common.Constants;
using CheckBox = UIAutomation.Framework.Elements.CheckBox;
using System.Drawing;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageObjects.QuickLaunch;
using System.IO;
using System.Security.Policy;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.Appeal;
using UIAutomation.Framework.Database;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealSummaryPage : NewDefaultPage
    {
        #region PRIVATE PROPERTIES

        private AppealSummaryPageObjects _appealSummaryPage;
        private AppealSearchPage _appealSearch;
        private CalenderPage _calenderPage;
        private readonly FileUploadPage _fileUpload;
        private readonly string _originalWindow;        

        #endregion

        #region CONSTRUCTOR

        public AppealSummaryPage(INewNavigator navigator, AppealSummaryPageObjects appealSummaryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealSummaryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealSummaryPage = (AppealSummaryPageObjects) PageObject;
            _calenderPage = new CalenderPage(SiteDriver);
            _fileUpload = new FileUploadPage(SiteDriver,JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
        }

        #endregion

        #region PUBLIC METHODS

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public FileUploadPage FileUploadPage
        {
            get { return _fileUpload; }
        }

        public string ClickOnDownloadPDFAndGetFileName(AppealLetterPage appealLetter)
        {
            appealLetter.ClickOnDownloadPdf();
            var downloadPage = NavigateToChromeDownLoadPage();
            var fileName = downloadPage.GetFileName();

            SiteDriver.WaitForCondition(() =>
            {
                if (fileName == "")
                {
                    fileName = downloadPage.GetFileName();
                    return false;
                }
                else
                    return true;
            }, 5000);
            return fileName;
        }

        #region EditAppealDetail

        public void ClearEditAppealInfoDropDownField(string label)
        {
            SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
                How.XPath).ClearElementField();
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate,label),
               How.XPath);
            SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
                How.XPath).SendKeys(Keys.ArrowDown);
            JavaScriptExecutor.ExecuteClick(
               string.Format(AppealSummaryPageObjects.EditAppealDropDownListByLabelXpathTemplate, label)+"[1]", How.XPath);
            Console.WriteLine("{0} cleared", label);
        }

        public void SelectEditAppealFieldDropDownListByLabel(string label, string value)
        {
            var element = SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
                How.XPath);
               
            
            SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath)
                .SendKeys(value);
            if(!SiteDriver.IsElementPresent(string.Format(AppealSummaryPageObjects.EditAppealDropDownListValueByLabelXpathTemplate, label, value), How.XPath))
                JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
                    How.XPath);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealSummaryPageObjects.EditAppealDropDownListValueByLabelXpathTemplate, label, value), How.XPath);
            Console.WriteLine("{0} <{1}> selected", label, value);
        }

        public void SetDueDate(string date)
        {
            SiteDriver.FindElement(
                  string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, "Due Date"),
                  How.XPath).ClearElementField();
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, "Due Date"),
                How.XPath);
            
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(300);//wait to load whether confirmation popup appears or not
            if(IsPageErrorPopupModalPresent())
                ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Due Date Selected:<{0}>",  date);
        }

        public bool IsEditAppealInfoFieldDisabledByLabel(string label)
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(AppealSummaryPageObjects.DisabledEditAppealInputFieldByLabelXPathTemplate, label),
                    How.XPath);
        }

        public string GetEditAppealInputValue(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInformationValueCssTemplate, row),
                How.CssSelector).GetAttribute("value");
        }

        public string GetDisabledEditAppealInputValueByLabel(string label)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.DisabledEditAppealInputFieldByLabelXPathTemplate, label),
                How.XPath).GetAttribute("value");
        }

        public string GetEditAppealInputValueByLabel(string label)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
                How.XPath).GetAttribute("value");
        }


        public string GetLetterBodyForDCIAppealLetter()
        {
            var body =
                SiteDriver.FindElement(AppealSummaryPageObjects.LetterBodyForDCICssLocator, How.CssSelector).Text;
            return body;
        }
        public void SetEditAppealInputValueByLabel(string label, string value, bool checkForerror=false)

        {
            SiteDriver.FindElement(
               string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
               How.XPath).ClearElementField();
            SiteDriver.FindElement(
              string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label),
              How.XPath).SendKeys(value);
            if(!checkForerror) SiteDriver.FindElementAndClickOnCheckBox(string.Format(AppealSummaryPageObjects.LabelXPath, label),
              How.XPath);
            Console.WriteLine("<{0}> is selected to <{1}>", label, value);
        }
        public List<String> GetEditAppealInputListByLabel(string label)
        {
            JavaScriptExecutor.ExecuteClick(
                 string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            var list = JavaScriptExecutor.FindElements(
                string.Format(AppealSummaryPageObjects.EditAppealDropDownListByLabelXpathTemplate, label), How.XPath,
                "Text");
            JavaScriptExecutor.ExecuteMouseOut(
               string.Format(AppealSummaryPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            
            list.RemoveAll(string.IsNullOrEmpty);
            return list;
        }

        public void SetEditAppealInputValue(int row, string value)
        {
            SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.EditAppealInformationValueCssTemplate, row),
                How.CssSelector).ClearElementField();
            SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.EditAppealInformationValueCssTemplate, row),
                How.CssSelector).SendKeys(value);
        }

        public void ClickOnSaveButtonOnEditAppeal()
        {
            
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.SaveButtonEditAppeal, How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            
        }

        public void ClickOnCancelLinkOnEditAppeal()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.CancelLinkEditAppeal, How.CssSelector);
        }
        #endregion

        #region AppealEmail
        public void ClickonEmailIcon()
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.EmailIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(IsAppealEmailFormPresent);
        }
        
        public bool IsAppealEmailFormPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealEmailFormCssLocator, How.CssSelector);
        }

        public void SetToEmailInfo(string to)
        {
            SiteDriver.FindElement(AppealSummaryPageObjects.ToInputFieldCssLocator,How.CssSelector).ClearElementField();
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(AppealSummaryPageObjects.ToInputFieldCssLocator, How.CssSelector).SendKeys(to);
        }

        public void SetAdditionalCCEmailInfo(string to)
        {
            SiteDriver.FindElement(AppealSummaryPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealSummaryPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector).SendKeys(to);
        }

        public string GetToEmailInfo()
        {
           return  SiteDriver.FindElement(AppealSummaryPageObjects.ToInputFieldCssLocator, How.CssSelector)
                .GetAttribute("value");
        }

        public string GetAdditionalCCEmailInfo()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector)
                 .GetAttribute("value");
        }

        public string GetEmailValue()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.EmailValueXPath, How.XPath).Text;
        }

        public string GetClientCCValue()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.ClientCCValueXPath, How.XPath).Text;
        }

        public string GetTextMessage()
        {
            return
                SiteDriver.FindElement(AppealSummaryPageObjects.TextMessageCssLocator, How.CssSelector).Text.Replace("\r\n", "");
        }

        public void SetNote(string note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            WaitForStaticTime(2000);

        }
        public void SetNoteInRecordQaResult(string note)
        {
            SiteDriver.FindElement(AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector).ClearElementField();
            SiteDriver.WaitToLoadNew(200);
            SiteDriver.FindElement(AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector).SendKeys(note);
        }

        public void SetLengthyNoteInRecordQaResults(string note)
        {

            SiteDriver.FindElement(AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector).ClearElementField();
            SiteDriver.WaitToLoadNew(200);
            JavaScriptExecutor.SendKeys(note.Substring(0, note.Length - 4), AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector);
            SiteDriver.FindElement(
                    AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector)
                .SendKeys(note.Substring(0, 1));
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(
                    AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector)
                .SendKeys(note.Substring(0, 1));
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(
                    AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector)
                .SendKeys(note.Substring(0, 1));
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(
                    AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector)
                .SendKeys(note.Substring(0, 1));
            SiteDriver.WaitToLoadNew(500); //wait for removing last character which will take few ms
            Console.WriteLine("Note set to {0}", note);
        }
        //public void SetNoteToEmpty()
        //{
        //    SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
        //    SiteDriver.FindElement("body", How.CssSelector).Click();
        //    
        //    SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
        //    SiteDriver.FindElement("body", How.CssSelector).SetText("");
        //    JavaScriptExecutor.SendKeys(" ",
        //        "body", How.CssSelector);
        //    //JavaScriptExecutor.ExecuteScript("document.body.innerHTML = 'c'");          
        //    Console.WriteLine("Note set to empty");
        //    SiteDriver.SwitchBackToMainFrame();
        //    SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.EditAppealInformationValueCssTemplate, 2),
        //        How.CssSelector)
        //        .Click();
        //}
        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public string GetNoteInRecordQaResult()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.RecordQaResultNoteCssSelector, How.CssSelector).GetAttribute("value"); 
        }


        public void ClickOnCancelLinkOnAppealEmail()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.CancelAppealEmailLinkCssLocator,How.CssSelector);
            
        }
        public void ClickOnSendEmail()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.SendEmailButtonXPath, How.XPath);
            SiteDriver.WaitForCondition(()=>!IsWorkingAjaxMessagePresent());
            
        }

        public bool IsAppealEmailEnabled()
        {
            return
                !SiteDriver.FindElement(AppealSummaryPageObjects.EmailIconCssLocator, How.CssSelector)
                    .GetAttribute("class")
                    .Contains("disabled");
        }

        public string GetAppealEmailTitle()
        {
            return
                SiteDriver.FindElement(AppealSummaryPageObjects.EmailIconCssLocator, How.CssSelector)
                    .GetAttribute("title");
        }

        public bool IsManageAppealEmailIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EmailIconCssLocator, How.CssSelector);
        }
        #endregion

        public bool IsAppealDetailFieldPresentByLabel(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealSummaryPageObjects.AppealDetailValueXPthTemplate,label), How.XPath);
        }

        public string GetAppealDetailFieldPresentByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.AppealDetailValueXPthTemplate, label), How.XPath).Text;
        }

        public bool IsSearchIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.SearchIconCssLocator, How.CssSelector);
        }

        public string GetAppealDetails(int row, int col)
        {
           return  SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealDetailsCssTemplate,row,col),
                How.CssSelector).Text;
        }

        public string GetAppealDetailsToolTip(int row, int col)
        {
            return SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealDetailsCssTemplate, row, col),
                 How.CssSelector).GetAttribute("title").Trim();
        }


        public string GetClientNotesEllipsis()
        {
            return SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealDetailsCssTemplate, 3, 1),
                 How.CssSelector).GetCssValue("text-overflow");
        }
        public bool IsExternalDocIdPresentInAppealDetails()
        {
            return
                SiteDriver.IsElementPresent(string.Format(AppealSummaryPageObjects.AppealDetailsCssTemplate, 2,5),
                    How.CssSelector);
        }

        public bool IsEditIconDisabled()
        {
            return
                SiteDriver.IsElementPresent(AppealSummaryPageObjects.EditDisabledIconXPath,
                    How.XPath);
        }

        public bool IsEditIconEnabled()
        {
            return
                SiteDriver.IsElementPresent(AppealSummaryPageObjects.EditEnabledIconXPath,
                    How.XPath);
        }

        public void ClickOnEditIcon()
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.EditIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsEditAppealFormDisplayed()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.EditFormCssLocator, How.CssSelector);
        }

        public void WaitToCloseEditAppealForm()
        {
            SiteDriver.WaitForCondition(() => !IsEditAppealFormDisplayed());
        }

       

        public bool IsGreyAppealIconPresent(int row,int col=4)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealSummaryPageObjects.GreyAppealLevelCssTemplate, row,col),
                How.CssSelector);
        }

        public void ClickOnClaimLineDiv(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.ClaimLineDivCssTemplate,row),How.CssSelector);
            
        }

        /// <summary>
        /// Navigate to claim action page
        /// </summary>
        /// <returns>claim action page</returns>
        public ClaimActionPage ClickOnClaimSequenceAndSwitchWindow()
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element =
                    SiteDriver.FindElement(
                        string.Format(AppealSummaryPageObjects.ClaimGroupRowValueXPathTemplate, "Claim Sequence"),
                        How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnClaimSequenceSwitchToClaimAction(int row = 2)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element =
                    SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.ClaimSequenceValueCssTemplate, row),
                        How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on Claim Sequence: ");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("claim_lines"));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        /// <summary>
        /// Navigate to claim action page
        /// </summary>
        /// <returns>claim action page</returns>
        public ClaimActionPage ClickOnClaimNoAndSwitchWindow()
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.ClaimGroupRowValueXPathTemplate, "Claim No"), How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on claim no, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage ClickOnProcCodeandSwitch(string title, int claimRow, int row, int col)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnValueCssTemplate, claimRow, row, col),
                How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public NewPopupCodePage ClickOnRevenueCodeandSwitch(string title, int claimRow, int row, int col)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnValueCssTemplate, claimRow, row, col),
                How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public NewPopupCodePage SwitchToPopupCodePage(string title)
        {
            var popupCode = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(title));
                NewPopupCodePage.AssignPageTitle(title);

                Console.WriteLine("Switch To " + title + " Page");
            });
            return new NewPopupCodePage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAppealLineSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealLinesSectionCssLocator, How.CssSelector);
        }

        public string GetClaimSectionRowValueByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimGroupRowValueXPathTemplate, label), How.XPath).Text;
        }
        public string GetClaimSectionRowValueTooltipByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimGroupRowValueXPathTemplate, label), How.XPath)
                    .GetAttribute("title");
        }
        public string GetClaimSectionRowLabeleByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimGroupRowLabelXPathTemplate, label), How.XPath).Text;
        }

        public string GetClaimLineSectionRowValueByRowCol(int claimRow,int row,int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnValueCssTemplate, claimRow, row, col),
                    How.CssSelector).Text;
        }
        public string GetClaimLineSectionTooltipByRowCol(int claimRow, int row, int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnValueCssTemplate, claimRow, row, col),
                    How.CssSelector).GetAttribute("title");
        }

        public string GetAppealLineSectionRowValueByLabel(string field)
        {
             return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.AppealLineValueForLabelXpathTempalte, field),
                    How.XPath).Text;
        }

        public string GetAppealLineSectionRowToolTipByLabel(string field)
        {
            return
               SiteDriver.FindElement(
                   string.Format(AppealSummaryPageObjects.AppealLineValueForLabelXpathTempalte, field),
                   How.XPath).GetAttribute("title");
        }

        public string GetFirstLineOfAppealLineDataByAppealLineRow(int row=1)
        {
            return SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.FirstLineOfAppealLineDataCssLocator,row),How.CssSelector).Text;
        }

        public string GetProcDescriptionToolTipByRowCol(int claimRow, int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnValueCssTemplate, claimRow, row, 5),
                    How.CssSelector).GetAttribute("title");
        }

        public string GetAppealLevelToolTipByRowCol(int claimRow, int row,int col=4)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimLinesRowColumnCssTemplate, claimRow, row, col),
                    How.CssSelector).GetAttribute("title");
        }

        public List<string> GetFlagList( int claimRow = 2)
        {
            var list =
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.ClaimLineFlagListValueCssTemplate,  claimRow),
                    How.CssSelector).Text.Replace("\r\n", "").Replace(" ", "");
            return
               new List<string>(list.Split(','));
        }

        public List<string> GetClainNoList()
        {
            return
                JavaScriptExecutor.FindElements(AppealSummaryPageObjects.ClainNoListCssLcoator, How.CssSelector, "text");

        }
        public string GetStatusValue()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.StatusValueXPath, How.XPath).Text;
        }

        public string GetAppealType()=> SiteDriver.FindElement(AppealSummaryPageObjects.AppealTypeXPath, How.XPath).Text;

        public void ClickOkCancelOnConfirmationModal(bool confirmation)
        {

            if (confirmation)
            {
                var element =
                    SiteDriver.FindElement(AppealSummaryPageObjects.OkConfirmationCssSelector, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForPopupClosed();
                Console.WriteLine("Ok Button is Clicked");

            }
            else
            {
                var element = SiteDriver.FindElement(AppealSummaryPageObjects.CancelConfirmationCssSelector,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForPopupClosed();
                Console.WriteLine("Cancel Button is Clicked");

            }

        }

        public void WaitForPopupClosed()
        {
            SiteDriver.WaitForCondition(() => !IsPageErrorPopupModalPresent());
        }
        public bool IsClosedAppealIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.ClosedAppealDisabledIconCssLocator, How.CssSelector);
        }

        public bool IsClosedAppealIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.ClosedAppealEnabledIconCssLocator, How.CssSelector);
        }

        public void ClickOnClosedAppealIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.ClosedAppealEnabledIconCssLocator,How.CssSelector);
            
        }

        public void ClosedAppeal()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.ClosedAppealEnabledIconCssLocator, How.CssSelector);
            
            ClickOkCancelOnConfirmationModal(true);
            if(IsPageErrorPopupModalPresent())
                ClosePageError();
            
        }

        public string GetClosedAppealEnabledIconToolTip()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.ClosedAppealEnabledIconCssLocator,
                How.CssSelector).GetAttribute("title");
        }

        public string GetClosedAppealDisabledIconToolTip()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.ClosedAppealDisabledIconCssLocator,
                How.CssSelector).GetAttribute("title");
        }

        public void ClickOnAppealLineRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.AppealLineRowCssTemplate,row),How.CssSelector);
            
        }

        public string GetAppealSummaryValueByRowCol(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.AppealSummaryValueCssTemplate, row, col), How.CssSelector).Text;
        }

        public string GetNoPreviousAppealsMessage()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.NoPreviousAppealXPath,
                How.XPath).Text;
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public AppealSummaryPage ClickOnPreviousAppealSequenceByRow(int row)
        {
            var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                var element =
                    SiteDriver.FindElement(
                        string.Format(AppealSummaryPageObjects.PreviousAppealValueXPath + "[{0}]/span", row),
                        How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on Previous Appeal Sequence of row {0}", row);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
            });
            return new AppealSummaryPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetAppealSequenceOnHeader()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.AppealSequenceValueHeaderSectionCssLocator,
                How.CssSelector).Text;
        }

        public List<string> GetPreviousAppealList()
        {
            return JavaScriptExecutor.FindElements(AppealSummaryPageObjects.PreviousAppealValueXPath, How.XPath, "Text");
        }
        public List<string> GetPreviousAppealListSpanValue()
        {
            return JavaScriptExecutor.FindElements(AppealSummaryPageObjects.PreviousAppealListXPath, How.XPath, "Text");
        }

        public void ClickOnAppealSeqAndSwitch(string appealSeq)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealSummaryPageObjects.PreviousAppealSelectionXPath, appealSeq),
                How.XPath);
             SwitchToPopUpWindow();
             SiteDriver.WaitForCondition(() =>
             {
                 SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                
                 if (
                     string.Compare(SiteDriver.Title, PageTitleEnum.AppealSummary.GetStringValue(),
                         StringComparison.OrdinalIgnoreCase) == 0)
                 {
                    AppealSummaryPage appealSummary= new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                    var appealSequence = appealSummary.GetAppealSequenceOnHeader();
                     return true;
                 }
                 return false;
             });
            SiteDriver.WaitForPageToLoad();
        }
        public string GetPageHeader()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public bool IsPageLoaded()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector);
        }

        public string GetAppealLineDetailsValueByRowCol(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.AppealLineDetailsValueCssTemplate, row, col), How.CssSelector).Text;
        }

        public string GetAppealLineDetailsEmptyMessage()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.AppealLineDetailsEmptyMessageCssLocator,
                How.CssSelector).Text;
        }


        public bool IsAppealLineDetailsSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealLineDetailsSectionCssLocator, How.CssSelector);
        }
        public AppealSearchPage ClickOnSearchIconToNavigateAppealSearchPage()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.SearchIconCssLocator,How.CssSelector);
            SiteDriver.WaitForCondition(() => GetPageHeader().Equals(PageHeaderEnum.AppealSearch.GetStringValue()));
            GetSideBarPanelSearch.OpenSidebarPanel();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSearchPageObjects.FindAppealSectionCssLocator, How.CssSelector));
            return new AppealSearchPage(Navigator, new AppealSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        
        public QaAppealSearchPage ClickOnSearchIconToNavigateQAAppealSearchPage()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.SearchIconCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            return new QaAppealSearchPage(Navigator, new QaAppealSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsAppealLetterEnabled()
        {
           return  SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealLetterEnabledXPath, How.XPath);
        }

        public bool IsAppealLetterDisabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealLetterDisabledXPath, How.XPath);
        }

        /// <summary>
        /// CLick on appeal letter link
        /// </summary>
        /// <returns></returns>
        public AppealLetterPage ClickAppealLetter()
        {
            var appealLetter = Navigator.Navigate<AppealLetterPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.AppealLetterEnabledXPath, How.XPath);
                Console.WriteLine("Clicked Appeal Letter Link");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            });
            return new AppealLetterPage(Navigator, appealLetter, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Close note popup page
        /// </summary>
        public AppealSummaryPage CloseLetterPopUpPage()
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        //public bool IsAppealNoteEnabled()
        //{
        //    return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealNoteEnabledXPath, How.XPath);
        //}


        /// <summary>
        /// CLick on appeal note link
        /// </summary>
        /// <returns></returns>
        //public AppealNotePage ClickAppealNote()
        //{
        //    var appealNote = Navigator.Navigate<AppealNotePageObjects>(() =>
        //    {
        //        JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.AppealNoteEnabledXPath, How.XPath);
        //        Console.WriteLine("Clicked Appeal Note Link");
        //        SiteDriver.WaitForCondition(
        //            () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealNotes.GetStringValue()));
        //    });
        //    return new AppealNotePage(Navigator, appealNote, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        //}

        /// <summary>
        /// Close note popup page
        /// </summary>
        public AppealSummaryPage CloseNotePopUpPage()
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public void ClickMoreOption()
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.MoreOptionCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }
        public bool IsAppealProcessingHxLinkPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealProcessingHxLinkXpath, How.XPath);
        }

        /// <summary>
        /// CLick on appeal Processing Hx
        /// </summary>
        /// <returns></returns>
        public AppealProcessingHistoryPage ClickAppealProcessingHx()
        {
            var appealprocessingHx = Navigator.Navigate<AppealProcessingHistoryPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.AppealProcessingHxLinkXpath, How.XPath);
                Console.WriteLine("Clicked Appeal Proccessing Link");
                if(SiteDriver.IsElementPresent("footer",How.Id))
                    SiteDriver.MouseOver("footer", How.Id, release: true);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealProcessingHistory.GetStringValue()));
            });
            return new AppealProcessingHistoryPage(Navigator, appealprocessingHx, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public bool IsAppealProcessingHistory()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.AppealProcessingHistory.GetStringValue());
        }
        public bool IsAppealNotePagePresent()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.AppealNotes.GetStringValue());
        }


        public bool IsAppealDocumentDivPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealDocumentsDivCssLocator, How.CssSelector);
        }
        public int AppealDocumentCountOfFileList()
        {
            return SiteDriver.FindElementsCount(AppealSummaryPageObjects.AppealDocumentsDivCssLocator, How.CssSelector);
        }
        public List<DateTime> GetAddedAppealDocumentList()
        {
            return JavaScriptExecutor.FindElements(AppealSummaryPageObjects.ListsofAppealDocumentsCssLocator, How.CssSelector, "Text").Select(DateTime.Parse).ToList();
            
        }
        public string AppealDocumentsListAttributeValue(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            return SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).Text;
        }
        public string GetAppealDocumentAttributeToolTip(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            return SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).GetAttribute("title");
        }

        public void ClickOnDocumentToViewAndStayOnPage(int docrow)
        {
            var element =
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.AppealDocumentsListAttributeValueCssTemplate, docrow, 1, 2),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();

            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
        }
        /// <summary>
        /// Close note popup page and back to appeal summary page
        /// </summary>
        public AppealSummaryPage CloseDocumentTabPageAndBackToAppealSummary()
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public bool IsAppealDocumentUploadEnabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealDocumentUploadEnabledXPath, How.XPath);
        }
        public bool IsAppealDocumentUploadDisabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealDocumentUploadDisabledXPath, How.XPath);
        }

        public void ClickOnAppealDocumentUploadIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.AppealDocumentUploadEnabledXPath, How.XPath);
            Console.WriteLine("Clicked Appeal Document Upload Icon");
        }
        public bool IsAppealDocumentUploadSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealDocumentUploadSectionCssLocator, How.CssSelector);
        }

        public bool IsAddFileButtonDisabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.DisabledAddDocumentButtonCssLocator, How.CssSelector);
        }
      

        public void PassFilePathForDocumentUpload()
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/Test.txt");
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(AppealSummaryPageObjects.AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
        }


        public string GetAppealSummaryUploaderFieldLabel(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.AppealSummaryUploaderFieldLabelCssLocator, row), How.CssSelector).Text;
        }
        public void SetAppealSummaryUploaderFieldValue(string description, int row)
        {
            SiteDriver.FindElement(string.Format(AppealSummaryPageObjects.AppealSummaryUploaderFieldValueCssLocator, row), How.CssSelector)
                .SendKeys(description);
        }
        public string GetAppealSummaryUploaderFieldValue(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.AppealSummaryUploaderFieldValueCssLocator, row), How.CssSelector).GetAttribute("value");
        }
        public int GetAppealLockCountByUser(string username)
        {
            return (int)Executor.GetSingleValue(string.Format(AppealSqlScriptObjects.AppealCountByUser, username));
        }

        /// Click on add file button
        /// </summary>
        public void ClickOnAddFileBtn()
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.AddDocumentButtonCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitToLoadNew(500);
            Console.WriteLine("Add document in  Appeal Document Upload");
        }

        public int DeleteIconCountOfFileList()
        {
            return SiteDriver.FindElementsCount(AppealSummaryPageObjects.DeleteIconForFileListCssLocator, How.CssSelector);
        }
        public bool IsAppealDocumentDeleteDisabled()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.DeleteIconDisabledForFileListCssLocator, How.CssSelector);
        }
        /// <summary>
        /// Click on delete file button
        /// </summary>
        public void ClickOnDeleteFileBtn(int row)
        {
            var element =
                SiteDriver.FindElement(
                    string.Format(AppealSummaryPageObjects.DeleteFileDocumentInAppealSummaryCssLocator, row),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();

            Console.WriteLine("Delete document in  Appeal Document ");
        }

        public bool IsFileToUploadPresent()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.FileToUploadSection, How.XPath);
        }
        public string FileToUploadDocumentValue(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(AppealSummaryPageObjects.FileToUploadDetailsCssLocator, row, col), How.CssSelector).Text;
        }


        public List<string> GetAvailableFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            return JavaScriptExecutor.FindElements(AppealSummaryPageObjects.FileTypeValueListCssLocator, How.CssSelector, "Text");

        }
        public string GetPlaceHolderValue()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.FileTypeCssLocator, How.CssSelector)
                .GetAttribute("placeholder");
        }
        public void SetFileTypeListVlaue(string fileType)
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            SiteDriver.FindElement(AppealSummaryPageObjects.FileTypeCssLocator, How.CssSelector).SendKeys(fileType);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealSummaryPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(string.Format(AppealSummaryPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
            Console.WriteLine("File Type Selected: <{0}>", fileType);
        }

        public List<string> GetSelectedFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            return JavaScriptExecutor.FindElements(AppealSummaryPageObjects.FileTypeSelectedValueListCssLocator, How.CssSelector, "Text");

        }


        /// <summary>
        /// providing the label name, get the value for respective label
        /// </summary>
        /// <param name="field">title for the label</param>
        /// <returns>value corresponding to the label</returns>
        public string GetAppealLineDetailsAuditValueByLabel(string field)
        {
            return
                SiteDriver.FindElement(
                   string.Format(AppealSummaryPageObjects.AppealLineDetailsAuditByLabelXpathTemplate, field),
                   How.XPath).Text;
        }

            
        /// <summary>
        /// providing the label name, get the value for respective label, displayed under a div
        /// </summary>
        /// <param name="field">title for the label</param>
        /// <returns>value corresponding to the label</returns>
        public string GetAppealLineDetailsAuditValueByLabelInDiv(string field)
        {
            return
               SiteDriver.FindElement(
                   string.Format(AppealSummaryPageObjects.AppealLineDetailsAuditByLabelForDivXpathTemplate, field),
                   How.XPath).Text;
        }
        /// <summary>
        /// Click on display rationale link
        /// </summary>
        public void ClickOnDiplayRationaleLink(string label)
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.DiplayRationaleLinkXpath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Display Rationale and Summary link.");
            SiteDriver.WaitForCondition(() => IsProvidedLabelElementPresent(label));
        }

        public bool IsProvidedLabelElementPresent(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealSummaryPageObjects.LabelElementPresentXpathTemplate,label), How.XPath);
        }

        public ClaimActionPage CloseAppealSummaryWindow()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimAction.GetStringValue());
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }


        public bool IsFirstAppealLineActiveElementPresent( )
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.ActiveFirstAppealLineXpath,  How.XPath);
        }




        /// <summary>
        /// Click on cancel button
        /// </summary>
        public void ClickOnCancelBtn()
        {
            var element = SiteDriver.FindElement(AppealSummaryPageObjects.CancelAppealUploadButtonCssLocator,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Cancel Appeal Document Upload");
        }
        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
            
        }
        /// <summary>
        /// Click on save button
        /// </summary>
        public void ClickOnSaveUploadBtn()
        {
            JavaScriptExecutor.ExecuteClick(AppealSummaryPageObjects.SaveAppealUploadButtonCssLocator,How.CssSelector);
            Console.WriteLine("Save Appeal Document Upload");
            
            WaitForWorkingAjaxMessage();
        } 
        public void SwitchToPopUpWindow()
        {
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]); 
        }

        public int WindowCount()
        {
            return SiteDriver.CurrentWindowHandle.Count();
        }
        

        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
                }
            }
        }
        
        public AppealSearchPage ClickOnApproveIconToNavigateAppealSearchPage()
        {
            var appealSearchPage = Navigator.Navigate<AppealSearchPageObjects>(() =>
            {
                ClickOnClosedAppealIcon();
                ClickOkCancelOnConfirmationModal(true);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            });
            
            return new AppealSearchPage(Navigator, appealSearchPage, SiteDriver, JavaScriptExecutor,
                EnvironmentManager, BrowserOptions, Executor);
        }
        public void PassGivenFileNameFilePathForDocumentUpload(string filename)
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/" + filename);
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(AppealSummaryPageObjects.AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
        }
           public bool UploadDocumentFromAppealSummary(string fileType, string fileName, string descp, int row)
        {
              if(!IsAppealDocumentUploadEnabled())
            {
                Console.WriteLine("Uploader not enabled");
                return false;
            }
            ClickOnAppealDocumentUploadIcon();

            PassGivenFileNameFilePathForDocumentUpload(fileName);
            SetAppealSummaryUploaderFieldValue(descp, 3);
            SetFileTypeListVlaue(fileType);
            ClickOnAddFileBtn();
            if (!IsFileToUploadPresent())
            {
                Console.WriteLine("File not added for upload.");
                return false;
            }
            if (!FileToUploadDocumentValue(row, 2).Equals(fileName))
            {
                Console.WriteLine("Added document doesn't correspond to uploaded doc details");
                return false;
            }
            ClickOnSaveUploadBtn();
            StringFormatter.PrintMessage("Uploaded document to appeal");
               return true;
        }

        public bool IsQaCompleteIconInAppealSummaryEnabled()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.QaCompleteInAppealSummaryEnabledIconXpathSelector, How.XPath)
                .GetAttribute("class").Contains("is_active");
        }

        public bool IsQaCompleteIconInAppealSummaryDisabled()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.QaCompleteInAppealSummaryEnabledIconXpathSelector, How.XPath)
                .GetAttribute("class").Contains("is_disabled");
        }

        public void ClickOnCompleteQAIcon(bool isClickOnly=false)
        {
            if (isClickOnly)
            {
                var element = SiteDriver.FindElement(AppealSummaryPageObjects.QaCompleteInAppealSummaryIconCssSelector,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                return;
            }

            var element1 = SiteDriver.FindElement(AppealSummaryPageObjects.QaCompleteInAppealSummaryIconCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element1);
            SiteDriver.WaitToLoadNew(500);
            element1.Click();
            SetEditAppealInputValue(2, "0");
            ClickOnSaveButtonOnEditAppeal();
            WaitForWorkingAjaxMessage();
        }
      

        public AppealSummaryPage ClickOnAppealQaPassIconAndWaitForNextAppeal()
        {
            var newAppealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                ClickOnCompleteQAIcon();
                Console.WriteLine("Clicked on QA Complete button");
            });
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSummaryPageObjects.AppealSummaryPageTitleCss, How.CssSelector));            ;
            return new AppealSummaryPage(Navigator, newAppealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        /// <summary>
        /// Returns if claim has lock icon at the top
        /// </summary>
        /// <returns></returns>
        public bool IsAppealLocked()
        {
            return SiteDriver.IsElementPresent(AppealSummaryPageObjects.LockIconCssSelector, How.CssSelector);
        }


        public string GetLockIConTooltip()
        {
            return SiteDriver.FindElement(AppealSummaryPageObjects.LockIconCssSelector, How.CssSelector)
                  .GetAttribute("title");
        }

       

        
        #endregion

        #region PRIVATE METHODS



        #endregion
    }
}
