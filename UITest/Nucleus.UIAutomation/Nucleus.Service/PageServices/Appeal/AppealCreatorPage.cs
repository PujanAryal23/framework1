using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Appeal;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Core.Driver;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Elements;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Menu;
using System.IO;
using Nucleus.Service.Data;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealCreatorPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly AppealCreatorPageObjects _appealCreatorPage;
        private readonly ClaimActionPageObjects _claimActionPage;
        private AppealSearchPage _appealSearch;
        private AppealActionPage _appealAction;
        private AppealProcessingHistoryPage _appealProcessingHistory;
        private readonly string _originalWindow;
        //private readonly SideBarPanelSearch _sideBarPanelSearch;
       // private readonly GridViewSection _gridViewSection;
       //private readonly SideWindow _sideWindow;
        private readonly FileUploadPage _fileUploadPage;

        #endregion

        #region CONSTRUCTOR

        public AppealCreatorPage(INewNavigator navigator, AppealCreatorPageObjects appealCreatorPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealCreatorPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealCreatorPage = appealCreatorPage;//(AppealCreatorPageObjects) PageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            //_sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            //_gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            //_sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _fileUploadPage = new FileUploadPage(SiteDriver,JavaScriptExecutor);
        }
        //public SideWindow GetSideWindow
        //{
        //    get { return _sideWindow; }
        //}

        public FileUploadPage GetFileUploadPage
        {
            get { return _fileUploadPage; }
        }

        #endregion
        #region PUBLIC METHODS
        #region sql

        public List<string> GetAssociatedDocumentCount(string claimseq)
        {
            var countOfDoc =
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.AppealDocumentUploadedAuditRecord, claimseq));
            Executor.CloseConnection();
            Console.WriteLine("The count of documents associated with claim's appeal is {0}",countOfDoc[0]);
            return countOfDoc;
        }
        #endregion

        public AppealProcessingHistoryPage OpenAppealProcessingHistoryPage(string claimSeq)
        {
            _appealSearch = NavigateToAppealSearch();
            _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeq);
            _appealAction.ClickMoreOption();
            return _appealProcessingHistory=_appealAction.ClickAppealProcessingHx();
        }

        public AppealCreatorPage CloseAppealProcessingHistoryAndBackToAppealCreator()
        {
            _appealAction=_appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();
            _appealSearch=_appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
            return _appealSearch.NavigateToAppealCreator();
        }


        public string OriginalWindowHandle
        {
            get { return _originalWindow; }
        }

        public bool IsFlagIconPresentInAddSection()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.flagIocnAddSectionCssLocator, How.CssSelector);
        }

      

        public string GetClaimNoValueSelectedClaimSection()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoValueCssLocator, How.CssSelector).Text;
        }

        public string GetClaimNoInAddClaimSection()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoInAddClaimSectionCssLocator, How.CssSelector).Text;
        }

        public string ClickOnUnlockClaimSequenceInAddClaimSection()
        {
            try
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceInAddClaimSectionCssLocator, How.CssSelector).Click();
                Console.WriteLine("Clicked on unlocked claim sequence in Add Section , navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                var newClaimAction = new ClaimActionPage(Navigator, new ClaimActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
                var pageHeader = newClaimAction.GetPageHeader();
                SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                Console.WriteLine("New Claim Action Popup page closed");
                
                return pageHeader;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return "";
        }

        public string ClickOnLockClaimSequenceInAddClaimSection()
        {
            try
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceHavingLockInAddClaimSectionXPath, How.XPath).Click();
                Console.WriteLine("Clicked on unlocked claim sequence in Add Section , navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                var newClaimAction = new ClaimActionPage(Navigator, new ClaimActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
                var pageHeader = newClaimAction.GetPageHeader();
                SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                Console.WriteLine("New Claim Action Popup page closed");
                
                return pageHeader;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return "";
            
        }

       

        public void ClickOnClaimLineHavingLockInAddClaimSection()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ClaimLineHavingLockInAddClaimSectionXPath,How.XPath);
            
        }
        public List<int> GetClaimSequenceListInAddClaimSection()
        {
           return
               JavaScriptExecutor.FindElements(AppealCreatorPageObjects.ClaimSequenceInAddClaimSectionCssLocator,
                    How.CssSelector,"Text").Select(s => s.Substring(s.IndexOf('-')+1)).ToList().Select(int.Parse).ToList();
        }
        public string GetClaimSequenceHavingLockInAddClaimSection()
        {
            return
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceHavingLockInAddClaimSectionXPath, How.XPath).Text;

        }
        public string GetEmptyMessage()
        {
            return
                SiteDriver.FindElement(AppealCreatorPageObjects.EmptyMessageCssLocator, How.CssSelector).Text;

        }
        public void ClickOnFindButton()
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath).Click();
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnClearLinkOnFindSection()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ClearLinkOnFindClaimSectionCssLocator,How.CssSelector);
            
        }

        public string GetAlternateClaimNoLabelTitleInFindPanel()
        {
            return
                SiteDriver.FindElement(AppealCreatorPageObjects.AlternateClaimNoInFindPanelLabelCssLocator,
                    How.CssSelector
                    ).Text;
        }

        public string GetClaimSequenceInFindClaimSection()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath).GetAttribute("value");

        }
        public void SetClaimSequenceInFindClaimSection(string claimSeq)
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath).ClearElementField();
            SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath).SendKeys(claimSeq);
            Console.WriteLine("Claim sequence set to "+claimSeq);
        }
                
        public string GetClaimNoInFindClaimSection()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).GetAttribute("value");
        }
        public void SetClaimNoInFindClaimSection(string claimNo)
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).ClearElementField();
            SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).SendKeys(claimNo);
            Console.WriteLine("Claim no set to " + claimNo);

        }
         
        public bool IsFindClaimSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.FindClaimSectionCssLocator, How.CssSelector);
        }
        public bool IsSearchIconPresentInAppealCreatorPage()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.SearchIconCssLocator, How.CssSelector);
        }
        public bool IsDocumentIDPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.DocumentIDCssLocator, How.CssSelector);
        }

        public void SetDocumentId(string documentId)
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.DocumentIDCssLocator, How.CssSelector)
                .ClearElementField();
            SiteDriver.FindElement(AppealCreatorPageObjects.DocumentIDCssLocator, How.CssSelector)
                .SendKeys(documentId);
        }

        public string GetDocumentId()
        {
          return  SiteDriver.FindElement(AppealCreatorPageObjects.DocumentIDCssLocator, How.CssSelector).GetAttribute("value");
                
        }

       public void SetDescription(string description)
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.DescriptionCssLocator, How.CssSelector)
                .SendKeys(description);
        }

        public string GetDescription()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.DescriptionCssLocator, How.CssSelector).GetAttribute("value");
                
        }

        public string GetPlaceHolderValue()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.FileTypeCssLocator, How.CssSelector)
                .GetAttribute("placeholder");
        }

        public void SetFileTypeListVlaue(string fileType)
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            SiteDriver.FindElement(AppealCreatorPageObjects.FileTypeCssLocator, How.CssSelector).SendKeys(fileType);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            Console.WriteLine("File Type Selected: <{0}>", fileType);
        }
        public void SetMultipleFileTypeListValue(List<string> fileTypesList)
        {

            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            var element =
                SiteDriver.FindElement(AppealCreatorPageObjects.FileTypeCssLocator, How.CssSelector);
            foreach (var fileType in fileTypesList)
            {
                element.SendKeys(fileType);
                JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
                JavaScriptExecutor.ExecuteMouseOut(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
                Console.WriteLine("File Type Selected: <{0}>", fileType);
            }
           
        }

        public List<string> GetAppealDocType()
        {
            return Enum.GetNames(typeof(AppealDocTypeEnum)).ToList();
        }
        public List<string> GetAvailableFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            return JavaScriptExecutor.FindElements(AppealCreatorPageObjects.FileTypeValueListCssLocator);
            
        }

        public List<string> GetSelectedFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(()=>SiteDriver.IsElementPresent(AppealCreatorPageObjects.FileTypeSelectedValueListCssLocator, How.CssSelector,5000));
            return JavaScriptExecutor.FindElements(AppealCreatorPageObjects.FileTypeSelectedValueListCssLocator, "Text");
            
        }
        public void ClickOnCliamLinesSectionByProCode(string columnName,string procCode)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.CliamLinesSectionByProCodeXPathTemplate, columnName, procCode), How.XPath);
            Console.WriteLine("Clicked on Claim Line Section having proc code<{0}> on <{1}> column",procCode,columnName);
        }

        public void ClickOnClaimLinesNotHavingFlag(string columnName,int row=1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLinesNotHavingFlagXPathTemplate, columnName,row),How.XPath);
        }

        public List<int> GetLineNoList(string columnName,string claimSeq)
        {
            return
                JavaScriptExecutor.FindElements(string.Format(AppealCreatorPageObjects.LinesNoXPathTemplate, columnName, claimSeq),
                    How.XPath,"Text").Select(int.Parse).ToList();
            
        }

        public List<int> GetClaimSequenceList(string columnName)
        {
            return
                JavaScriptExecutor.FindElements(string.Format(AppealCreatorPageObjects.ClaimSequenceXPathTemplate, columnName),
                    How.XPath,"Text").Select(s => s.Substring(0, s.LastIndexOf('-'))).ToList().Select(int.Parse).ToList();
            
        }
        public List<string> GetClaimSequenceWithSubList(string columnName)
        {
            return
                JavaScriptExecutor.FindElements(string.Format(AppealCreatorPageObjects.ClaimSequenceXPathTemplate, columnName),
                    How.XPath, "Text");

        }
        public void ClickOnSearchIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.SearchIconCssLocator,How.CssSelector);
            SiteDriver.WaitForCondition(IsFindClaimSectionPresent);
        }

        public void ClickCloseIconOnFindClaimSection()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.CloseIconOnFineClaimSectionCssSelector, How.CssSelector);
            
        }
        public void ClickOnClearOnAddClaimSection()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ClearLinkOnAddClaimSectionXPath, How.XPath);
            Console.WriteLine("Clear Link is Clicked in in Add Claim Section");
        }

        public bool IsClearOnAddClaimSectionPresent()
        {
           return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ClearLinkOnAddClaimSectionXPath, How.XPath);
        }

        public void ClickOnAddClaimRowSection()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.AddClaimRowSectionCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();

        }

        public int GetClaimRowSectionListInAddClaimSection()
        {
            return SiteDriver.FindElementsCount(AppealCreatorPageObjects.AddClaimRowSectionCssLocator, How.CssSelector);
        }

        public void ClickOnCancelOnConfirmationModal()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.CancelConfirmationCssSelector, How.CssSelector);
            Console.WriteLine("Cancel Button is Clicked");
        }

        public void ClickOnOkButtonOnConfirmationModal()
        {
             SiteDriver.FindElement(AppealCreatorPageObjects.OkConfirmationCssSelector, How.CssSelector).Click();
             Console.WriteLine("Ok Button is Clicked");
        }

        public void ClickOnClaimLineHavingZeroBillSection(string columnName)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLineHavingZeroBillSectionXPathTemplate,columnName,""),How.XPath);
        }
        public void ClickOnClaimLevelSection(string columnName,int row=1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLevelXPathTemplate, columnName,row), How.XPath);
            Console.WriteLine("Clicked on Claim Level Section");
        }
        public void ClickOnSelectAllCheckBox()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.SelectAllCheckBoxXPath,How.XPath);
            Console.WriteLine("Select All Check Box clicked");
        }

        public bool IsSelectAllCheckBoxDisabled() =>
            SiteDriver.FindElement(AppealCreatorPageObjects.SelectAllCheckBoxXPath, How.XPath).GetAttribute("class").Contains("is_disabled");
        public void ClickOnClaimLineNotHavingZeroBillSection(string columnName, int row = 1, bool select = false)
        {
            var ulCss = "";
            if (select)
                ulCss = "[contains(@class,'is_active')]";
            var count = SiteDriver.FindElementsCount(string.Format(AppealCreatorPageObjects.ClaimLineNotHavingZeroBillSectionCountXPathTemplate,columnName,ulCss,row),
                How.XPath);
           
            for(var i=count;i>= count;i--)
            {
                JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLineNotHavingZeroBillSectionXPathTemplate, columnName, i, ulCss, row), How.XPath);
                Console.WriteLine("Clicked on Claim line Section<{0}>",i);
            }
        }

        public void ClickOnCliamLineByClaimSeqRow(string columnName,int claimSeqDiv = 1,int row=1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLineNotHavingZeroBillSectionXPathTemplate, columnName, row, "", claimSeqDiv), How.XPath);
            Console.WriteLine("Clicked on Claim line Section<{0}>",row);
        }

        public void ClickOnClaimLine(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ClaimLineCssLocator, row), How.CssSelector);
            SiteDriver.WaitToLoadNew(2000);
            Console.WriteLine("Claim line "+row+" has been selected");
        }

        public List<string> GetSelectedClaimLinesByHeader(string columnName)
        {
                return JavaScriptExecutor.FindElements(string.Format(AppealCreatorPageObjects.SelectedClaimLinesByHeaderXpathTemplate, columnName),
               How.XPath, "Text");
        }


        public List<string> GetClaimLineListNotHavingZeroBillSection(string columnName)
        {
            return JavaScriptExecutor.FindElements(string.Format(AppealCreatorPageObjects.ClaimLineNotHavingZeroBillAllSectionCountXPathTemplate, columnName, "[contains(@class,'is_active')]"),
               How.XPath, "Text");
        }
        public List<string> GetClaimLineListHavingZeroBillSection(string columnName)
        {
            return
                JavaScriptExecutor.FindElements(
                    string.Format(AppealCreatorPageObjects.ClaimLineHavingZeroBillSectionXPathTemplate, columnName,
                        "[contains(@class,'is_active')]"), How.XPath, "Text");
        }
        public bool IsAppealMenuPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.AppealMenu, How.XPath);
        }
        public bool IsAppealCreatorQuicklaunchTilePresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.AppealCreatorQuicklaunchTile, How.Id);
        }

       
        public List<string> GetRecentlyAddedApealList()
        {
            return JavaScriptExecutor.FindElements(AppealCreatorPageObjects.RecentlyAddedAppealListCssLocator, How.CssSelector, "Text");
        }
        public string CflickOnAppealToOpenAppealSummaryAndSwithc( string appealseq)
        {

            return JavaScriptExecutor.FindElement(
                string.Format(AppealCreatorPageObjects.RecentlyAddedAppealJsSelectorTemplate, appealseq)).Text.Trim();
        }
        public AppealSummaryPage ClickOnAppealToOpenAppealSummaryAndSwitch(string appealSeq)
        {
            var appealSummaryPopup = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick((
                string.Format(AppealCreatorPageObjects.RecentlyAddedAppealXPathTemplate, appealSeq)), How.XPath);
                Console.WriteLine("Clicked on appeal sequence, navigating to appeal summary pop up");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealSummaryPage(Navigator, appealSummaryPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetRecentlyCreatedAppealSequence()
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.RecentlyAddedAppealCssSelectorTemplate, 1), How.CssSelector)
                    .Text;
        }

       

        public AppealCreatorPage NavigateToAppealCreatorFromCreateAppeal()
        {
            var appealCreatorPage = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                GetMouseOver.IsAppealCreator();
                var menuToClick = HeaderMenu.GetElementLocatorTemplateSpan(SubMenu.AppealCreator);
                JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
                Console.WriteLine("Navigated to {0}", SubMenu.AppealCreator);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealCreatorPageObjects.PageHeaderCssLocator, How.CssSelector));
            });

            return new AppealCreatorPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        

        /// <summary>
        /// Get provider and patient lable
        /// </summary>
        /// <returns></returns>
        public string GetProviderAndPatientLableValue()
        {
            return null;// _appealCreatorPage.ProviderAndPatient.Text;
        }

        /// <summary>
        /// Get provider label
        /// </summary>
        /// <returns></returns>
        public string GetProviderLabelValue()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ProviderNameLabelCssLocator, How.CssSelector).Text;
        }
        /// <summary>
        /// is provider name value present
        /// summary>
        /// <returns></returns>
        public bool IsProviderNamePresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ProviderNameValueCssLocator, How.CssSelector);
        }

        public string GetProviderNameValue() 
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ProviderNameValueCssLocator, How.CssSelector).Text;

        }

        /// <summary>
        /// Get patience sequence label
        /// </summary>
        /// <returns></returns>
        public string GetPatientSequenceLabelValue()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.PatientSequenceLabelCssLocator, How.CssSelector).Text;
        }
        /// <summary>
        /// is patience sequence value present
        /// </summary>
        /// <returns></returns>
        public bool IsPatientSequenceValuePresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.PatientSequenceValueCssLocator, How.CssSelector);
        }

        /// <summary>
        /// Get first selectaqble claim line
        /// </summary>
        /// <returns></returns>
        public string GetFirstSelectableClaimLine()
        {
            return null;// _appealCreatorPage.ClaimsGrid.GetRowData("td[2]/input and not(td/span[@disabled='disabled'])", 3);
        }

        /// <summary>
        /// Create appeal for selected claim
        /// </summary>
        /// <param name="claimLineNo"></param>
        /// <param name="appealNo"></param>
        /// <param name="product"></param>
        /// <param name="appealUrgentOrNot"></param>
        /// <param name="documentId"></param>
        public AppealCreatorPage CreateAppealForClaimSequence( string product, bool appealUrgentOrNot, string documentId = null)
        {
            var appealCreatorPage = Navigator.Navigate<AppealCreatorPageObjects>(() =>
                                                                             {
                                                                                 try
                                                                                 {
                                                                                     SelectAppealRecordType();
                                                                                     if (appealUrgentOrNot)
                                                                                        SetAppealPriorityUrgent();
                                                                                     SelectClaimLine();
                                                                                     if (ProductDropInputEnabled())
                                                                                         SelectProduct(product);
                                                                                     if(documentId!=null)
                                                                                         SetDocumentId(documentId);
                                                                                     ClickOnSaveBtn();
                                                                                 }
                                                                                 catch (InvalidOperationException exception)
                                                                                 {
                                                                                    throw new InvalidOperationException("Unable to create appeal.",exception);
                                                                                 }
                                                                             });
            return new AppealCreatorPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Creating appeal in appeal creator page
        /// </summary>
        /// <param name="product">product type</param>
        /// <param name="docId">external doc id</param>
        /// <param name="note">note text</param>
        /// <param name="appealType">appeal type "R" or "A"</param>
        /// <param name="urgent">is appeal urgent?</param>
        public void CreateAppeal(string product, string docId=null, string note =null, string appealType="A", string fileType=null, bool urgent = false)
        {
            SelectProduct(product);
            if (docId != null)
                    if(IsDocumentIDPresent())
                        SetDocumentId(docId);
            if (note != null)
                SetNote(note);
            switch (appealType)
            {
                case "A":
                    SelectAppealRecordType();
                    break;
                case "D":
                    SelectDentalAppealType();
                    break;
                case "M":
                    ClickMedicalRecordReviewButton();
                    break;
                default:
                    SelectRecordReviewRecordType();
                    break;
            }

            if (fileType != null)
            {
                SetFileTypeListVlaue(fileType);
            }
            if (urgent)
                SetAppealPriorityUrgent();

            ClickOnSaveBtn();
        }

        public void CreateAppealWithMultipleDocType(string product, string docId = null, string note = null, string appealType = "A", List<string> fileType = null, bool urgent = false)
        {
            SelectProduct(product);
            if (docId != null)
                if (IsDocumentIDPresent())
                    SetDocumentId(docId);
            if (note != null)
                SetNote(note);
            switch (appealType)
            {
                case "A":
                    SelectAppealRecordType();
                    break;
                case "D":
                    SelectDentalAppealType();
                    break;
                default:
                    SelectRecordReviewRecordType();
                    break;
            }

            if (fileType != null)
            {
                SetMultipleFileTypeListValue(fileType);
            }
            if (urgent)
                SetAppealPriorityUrgent();

            ClickOnSaveBtn();
        }


        public void DeleteAppealsOnClaim(string claseq)
        {
            NavigateToAppealManager().DeleteAppealsAssociatedWithClaim(claseq);
            NavigateToAppealCreator();
        }

        public void DeleteAppealDocument(string claseq)
        {
            var temp = string.Format(AppealSqlScriptObjects.DeleteAppealDocumentByClaseq, claseq);
            Executor.ExecuteQuery(temp);
        }


        public List<int> GetAppealDocumentType(string claseq)
        {
            var temp = string.Format(AppealSqlScriptObjects.GetAppealDocumentType, claseq);
            return Executor.GetTableSingleColumn(temp).ConvertAll(int.Parse);
            
        }

      


        /// <summary>
        /// Select product
        /// </summary>
        /// <param name="product"></param>
        public AppealCreatorPage SelectProduct(string product, bool isDirectSelect = false)
        {


            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputToggleCssSelector,
                How.CssSelector);
            if (!isDirectSelect)
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ProductComboInputCssSelector,
                    How.CssSelector).SendKeys(product);
            }

            SiteDriver.WaitToLoadNew(200);
            if (!SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.ProductComboInputValueXPathTemplate, product), How.XPath))
                JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputToggleCssSelector,
                    How.CssSelector);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ProductComboInputValueXPathTemplate, product), How.XPath);


            Console.WriteLine("Selected product = {0}", product);



            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetSelectedProductValue()
        {
            //JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            //JavaScriptExecutor.ExecuteMouseOver(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            return
                SiteDriver.FindElement(AppealCreatorPageObjects.ProductComboInputCssSelector,
                    How.CssSelector).GetAttribute("value");
        }

        public List<string> GetListOfActiveProducts()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            var productList =
                JavaScriptExecutor.FindElements(AppealCreatorPageObjects.ProductComboListCssSelector,
                    How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteMouseOut(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            productList.RemoveAt(0);
            return productList;
        }
        public bool ProductDropInputEnabled()
        {
            return SiteDriver.IsElementEnabled(AppealCreatorPageObjects.ProductComboInputDisabledCssSelector, How.CssSelector);
        }

        public void ClearProduct()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            //_appealCreatorPage.ProductComboInput.ClearElementField();
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ProductComboListSelectCssSelector,1), How.CssSelector);
            Console.WriteLine("Cleared Product");
        }

        public void SelectProductByUsingRow(int row)
        {

            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(AppealCreatorPageObjects.ProductComboInputCssSelector, How.CssSelector);
            //_appealCreatorPage.ProductComboInput.ClearElementField();
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ProductComboListSelectCssSelector, row), How.CssSelector);
            Console.WriteLine("Selected Product for row no "+ row);
        }

        /// <summary>
        /// return whether appeal type is disabled or not
        /// </summary>
        /// <param name="appealType">appeal type should be either "A" or "R"</param>
        /// <returns>status of radio button for appeal type</returns>
        public bool IsAppealTypeDisabled(string appealType)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealTypeBoxDisabledXPathTemplate, appealType), How.XPath);
               
        }
        /// <summary>
        /// return tool tip message for reason of appeal type disabled
        /// </summary>
        /// <param name="appealType">takes "A" or "R" as parameter</param>
        /// <returns>tool tip message</returns>
        public string GetToolTipForDisabledAppealType(string appealType)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.AppealTypeBoxDisabledXPathTemplate, appealType), How.XPath).GetAttribute("title");
        }

        /// <summary>
        /// Select appeal record type
        /// </summary>
        public AppealCreatorPage SelectAppealRecordType()
        {
            var element = SiteDriver.FindElement(AppealCreatorPageObjects.AppealRadioBoxInputXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Selected Record Type = Appeal");
            return new AppealCreatorPage(Navigator,_appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        } 
        /// <summary>
        /// Select record review record type
        /// </summary>
        public AppealCreatorPage SelectRecordReviewRecordType()
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.RecordReviewRadioBoxInputXPath, How.XPath).Click();
            Console.WriteLine("Selected Record Type = Record review");
            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsRecordReviewRadioButtonDisabled()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.RecordReviewRadioBoxInputXPath+ "/..", How.XPath)
                .GetAttribute("class").Contains("is_disabled");
        }

        /// <summary>
        /// Select record review record type
        /// </summary>
        public AppealCreatorPage SelectDentalAppealType()
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.DentalAppealTypeRadioBoxInputXpath, How.XPath).Click();
            Console.WriteLine("Selected Record Type = Record review");
            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public bool IsRespectiveAppealTypeSelected(string appealType)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealTypeBoxSelectionStatusXPath, appealType), How.XPath);
        }

        public bool IsAppealTypeRadioButtonPresent(string appealType)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealTypeBoxPresentXpath, appealType), How.XPath);
        }
        public bool  IsAppealPriorityStatusSetToUrgent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.UrgentCheckBoxSelectionStatusXPath, How.XPath);
        }
        public bool IsUrgentCheckboxDisabled()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.UrgentCheckBoxDisabledsXPath, How.XPath);
        }

        #region MedicalRecordReview
        public bool IsMedicalRecordReviewButtonPresent() =>
            SiteDriver.IsElementPresent(AppealCreatorPageObjects.MedicalRecordReviewAppealTypeXPath, How.XPath);

        public string GetMedicalRecordReviewToolTip() =>
            SiteDriver.FindElementAndGetAttribute(AppealCreatorPageObjects.MedicalRecordReviewAppealTypeXPath,
                How.XPath, "title");

        public void ClickMedicalRecordReviewButton() => SiteDriver.FindElement(
            AppealCreatorPageObjects.MedicalRecordReviewAppealTypeXPath,
            How.XPath).Click();

        #endregion
        /// <summary>
        /// Set appeal priority to urgent
        /// </summary>
        public AppealCreatorPage SetAppealPriorityUrgent()
        {
            SiteDriver.FindElement(AppealCreatorPageObjects.UrgentCheckBoxInpuXPath, How.XPath).Click();
            Console.WriteLine("Check Appeal Priority = Urgent");
            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// to check if passed row corresponds to desired form input by checking label value
        /// </summary>
        /// <param name="row"></param>
        /// <returns>returns the label value for row passed</returns>
        public string GetCreateAppealFormLabelValue(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.CreateAppealFormLabelCssSelector, row), How.CssSelector).Text;
        }
        public string GetCreateAppealFormInputValue(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.CreateAppealFormInputCssSelector, row), How.CssSelector).GetAttribute("value");
        }

        public string GetCreateAppealFormInputValueGeneric(string fieldname)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.CreateAppealFormInputXpathTempalte, fieldname), How.XPath).GetAttribute("value");
        }
        public void SetCreateAppealFormInputValue(string value, int row)
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormInputCssSelector, row),
                How.CssSelector).ClearElementField();
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormInputCssSelector, row),
                How.CssSelector).SendKeys(value);
        }
        public string GetAppealCategoryforAppealWithClaseq(string claseq)
        {
            return Executor.GetSingleStringValue(String.Format(AppealSqlScriptObjects.GetAppealCategoryForAppeals,
                claseq));
        }
        public void SetCreateAppealFormInputValueGeneric(string value, string fieldname)
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormInputXpathTempalte, fieldname),
                How.XPath).ClearElementField();
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormInputXpathTempalte, fieldname),
                How.XPath).SendKeys(value);
        }
        public string GetCreateAppealFormPhoneExtLabelValue(int row,int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.CreateAppealFormPhoneExtLabelCssSelector, row, col), How.CssSelector)
                    .Text;
        }
        public string GetCreateAppealFormPhoneExtInputValue(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCreatorPageObjects.CreateAppealFormPhoneExtInputCssSelector, row, col), How.CssSelector)
                    .GetAttribute("value");
        }


        public void SetCreateAppealFormPhoneExtInputValue(string value, int row, int col)
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormPhoneExtInputCssSelector, row, col),
                How.CssSelector).ClearElementField();
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.CreateAppealFormPhoneExtInputCssSelector, row, col),
                How.CssSelector).SendKeys(value);
        }
        public void SetNote(String note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();

        }

        public void SetLengthyNote(String header, string text)
        {
            JavaScriptExecutor.SwitchFrameByJQuery("iframe.cke_wysiwyg_frame.cke_reset");
            SendValuesOnTextArea(header, text);


        }

        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }
        public string GetExclamationIconTooltipMessage(int row)
        {
            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.ExclamationIconCssTemplate, row),
                    How.CssSelector).GetAttribute("title");
        }

        /// <summary>
        /// Select claim line
        /// </summary>
        /// <param name="claimLine"></param>
        public AppealCreatorPage SelectClaimLine()
        {
            JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.ClaimSequenceTemplate, How.CssSelector);
            Console.WriteLine("Claim Line Selected" );
            
            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        /// <summary>
        /// Click on save button
        /// </summary>
        public AppealCreatorPage ClickOnSaveBtn()
        {
            var element = SiteDriver.FindElement(AppealCreatorPageObjects.SaveBtnXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Clicked on Appeal save Button.");
            return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        /// <summary>
        /// Click on cancel button
        /// </summary>
        public void ClickOnCancelBtn()
        {
            var element = SiteDriver.FindElement(AppealCreatorPageObjects.CancelAppealCreateButtonCssLocator,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Cancel Appeal Creator");
           // return new AppealCreatorPage(Navigator, _appealCrelistatorPage);
        }

        public AppealCreatorPage SearchByClaimSequence(string claimSeq)
        {
            
            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(claimSeq);
                Console.WriteLine("Claim sequence set to " + claimSeq);
                element = SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForWorkingAjaxMessage();
                SiteDriver.WaitForCondition(()=> GetPageHeader() ==PageHeaderEnum.AppealCreator.GetStringValue(),5000);
                SiteDriver.WaitToLoadNew(3000);
                Console.WriteLine("Searched Claim Sequence: " + claimSeq);
                if (GetPageHeader() == PageHeaderEnum.ClaimSearch.GetStringValue() || GetPageHeader() == "Bill Search")
                {
                    ClickOnCreateAppealIcon(1);
                }
            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealCreatorPage SearchByClaimSequenceForLockedClaim(string claimSeq)
        {

            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath).ClearElementField();
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSequenceXPath, How.XPath).SendKeys(claimSeq);
                Console.WriteLine("Claim sequence set to " + claimSeq);
                SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                
                Console.WriteLine("Searched Claim Sequence: " + claimSeq);
                
            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsClaimLocked()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.LockIconCssLocator, How.CssSelector);
        }

        public bool IsCreateAppealDisabled()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.DisableCreateAppealIconCssLocator, How.CssSelector);
        }

        public bool IsExternalDocIdDisplayed()
        {
            return GetCreateAppealFormLabelValue(4) == "External Document ID";
        }

        public AppealCreatorPage SearchByClaimNo(string claimNo)
        {

            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).ClearElementField();
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).SendKeys(claimNo);
                SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                
                Console.WriteLine("Searched Claim No: " + claimNo);
                if (GetPageHeader() == "Claim Search")
                {
                    ClickOnCreateAppealIcon(1);
                }
            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealCreatorPage SearchByOtherClaimNoAndNavigateToAppealCreator3ColumnPage(string otherClaimNo)
        {

            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                GetSideBarPanelSearch.SetInputFieldByLabel("Other Claim Number", otherClaimNo);
                SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => GetPageHeader() == PageHeaderEnum.AppealCreator.GetStringValue(), 5000);
                SiteDriver.WaitToLoadNew(3000);
            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealCreatorPage SearchByClaimNoAndStayOnSearch(string claimNo)
        {

            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoXPath, How.XPath).SendKeys(claimNo);
                element = SiteDriver.FindElement(AppealCreatorPageObjects.FindXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                
                Console.WriteLine("Searched Claim No: " + claimNo);
               
            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        
        public bool IsAppealCreatorColumnContainerHeaderPresent(int col)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealCreatorColumnHeaderTemplate, col), How.CssSelector);
        }
        public string GetColumnHeaderText(int row)
        {
            return SiteDriver.FindElement(String.Format(AppealCreatorPageObjects.AppealCreatorColumnHeaderTemplate, row), How.CssSelector).Text;
        }
        public bool IsCreateAppealHeaderPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.AppealCreateFormHeaderCssLocator, How.CssSelector);
        }
        public string GetCreateAppealColumnHeaderText( )
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.AppealCreateFormHeaderCssLocator, How.CssSelector).Text;
        }
        public bool IsLineUnselectableForLockedClaimInSearchGrid()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.SearchlistComponentRowDisabled, How.CssSelector);

        }
        public bool IsAddAppealInClaimListPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AddAppealOnClaimSearchTemplate,row), How.CssSelector);
        }
        public bool IsAddAppealInClaimListPresent(string claseq)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(AppealCreatorPageObjects.AddAppealOnClaimByClaimSequenceTemplate, claseq));
        }

        public bool IsAddAppealInClaimListDisabled(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AddAppealDisabledOnClaimSearchTemplate, row), How.CssSelector);
        }

        public bool IsAddAppealInClaimListEnabled(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AddAppealEnabledOnClaimSearchTemplate, row), How.CssSelector);
        }

        public string GetAddAppealockToolTip(int row =1)
        {
            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.AddAppealOnClaimSearchTemplate, row),
                    How.CssSelector).GetAttribute("title");
        }
        public string GetSearchlistComponentItemLabel(int row, int col)
        {
            var element = SiteDriver.FindElement(
                string.Format(AppealCreatorPageObjects.SearchlistComponentItemLabelTemplate, row, col),
                How.CssSelector);
            return element.Text;
        }
        public string GetSearchlistComponentItemValue(int row,int col)
        {
            
            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.SearchlistComponentItemValueTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetSearchlistComponentItemValueByClaimseq(string claimseq, int col)
        {

            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.SearchlistComponentItemValueByClaimSeqTemplate, claimseq, col),
                    How.XPath).Text;
        }
        public string GetSearchlistComponentTooltipValue(int row, int col)
        {

            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.SearchlistComponentItemValueTemplate, row, col),
                    How.CssSelector).GetAttribute("title");
        }
        public string GetSearchlistComponentTooltipValueByClaimseq(string claimseq, int col)
        {

            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.SearchlistComponentItemValueByClaimSeqTemplate, claimseq, col),
                    How.CssSelector).GetAttribute("title");
        }
        public double GetAppealLevelBadgeOnSearchResult(int row)
        {

            var badgeNo   =  SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.AppealLevelBadgeOnSearchResultTemplate, row),
                    How.CssSelector).Text;
            return double.Parse(badgeNo);
        }

        public double GetAppealLevelBadgeOnSearchResult(string claseq)
        {

            var badgeNo = JavaScriptExecutor.FindElement(string.Format(AppealCreatorPageObjects.AppealLevelBadgeByClaimSequenceOnSearchResultTemplate, claseq)).Text;
            return double.Parse(badgeNo);
        }
        public List<string> GetClaimSeqListOnSearchResult()
        {

            return
                JavaScriptExecutor.FindElements(AppealCreatorPageObjects.ClaimActionsListOnSearchResult, 
                    How.CssSelector ,"Text");
        }

        public bool IsAppealCreateIconDisabledlookByClaimSeq(string appeal)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.DisableCreateAppealIconTemplate, appeal), How.XPath);
        }

        public string GetAppealDisabledToolTipValue(string claimseq)
        {
            return SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.DisableCreateAppealIconTemplate, claimseq),
                    How.XPath).GetAttribute("title");
        }
        /// <summary>
        /// Navigate to appeal creator
        /// </summary>
        /// <returns></returns>
        public void ClickOnCreateAppealIcon(int row)
        {
            SiteDriver.WaitForCondition(() => GetPageHeader().Equals(PageHeaderEnum.AppealCreator.GetStringValue()),
                5000);
            if (IsAddAppealInClaimListPresent(row))
            {
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.AddAppealOnClaimSearchTemplate, row), How.CssSelector).Click();
                Console.WriteLine("Clicked on "+row+"th  appeal icon from the list");
                WaitForWorkingAjaxMessage();
            }
            else { Console.WriteLine("No claim list found"); }
        }

        public AppealCreatorPage ClickOnFirstEnableCreateAppealIcon()
        {
            var appealCreator = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(AppealCreatorPageObjects.EnableCreateAppealIconCssLocator,How.CssSelector);
                WaitForWorkingAjaxMessage();

            });
            return new AppealCreatorPage(Navigator, appealCreator, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Navigate to appeal creator
        /// </summary>
        /// <returns></returns>
        public void ClickOnCreateAppealIcon(string claseq)
        {
            if (IsAddAppealInClaimListPresent(claseq))
            {
                JavaScriptExecutor.ClickJQuery(string.Format(AppealCreatorPageObjects.AddAppealOnClaimByClaimSequenceTemplate, claseq));
                Console.WriteLine("Clicked on claim sequence: " + claseq + " icon from the list");
                WaitForWorkingAjaxMessage();
            }
            else { Console.WriteLine("No claim list found"); }
        }
        public bool IsClaimActionLockToolTipPresent(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.LockTooltipOnClaimSearchPageOfAppealCreator,row), How.CssSelector);
        }


        public string GetClaimActionLockToolTip(int row = 1)
        {
            return
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.LockTooltipOnClaimSearchPageOfAppealCreator, row),
                    How.CssSelector).GetAttribute("title");
        }

        public string GetClaimSequenceLabel()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSeqLabelCssLocator, How.CssSelector).Text;
        }
        public bool IsClaimSequenceValuePresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ClaimSeqValueCssLocator, How.CssSelector);
        }

        public string GetEnableClaimSequence()
        {
            return SiteDriver
                .FindElement(AppealCreatorPageObjects.EnableClaimSequenceCssLocator, How.CssSelector).Text;
        }

        /// <summary>
        /// Navigate to claim action page
        /// </summary>
        /// <returns>claim action page</returns>
        public ClaimActionPage ClickOnClaimSequenceAndSwitchWindow()
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(AppealCreatorPageObjects.ClaimSeqValueCssLocator, How.CssSelector).Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnClaimSequenceOfAppealCreatorSearchAndSwitchWindow(int row)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.ClaimActionLinkOnSearchResultTemplate, row), How.CssSelector).Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public ClaimActionPage ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(string claimseq)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.SearchlistClaimSeqTemplate, claimseq), How.XPath).Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public string GetClaimNoLabel()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoLabelCssLocator, How.CssSelector).Text;
        }
        public bool IsClaimNoValueElementPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ClaimNoValueCssLocator, How.CssSelector);
        }
        public string GetClaimNoValue()
        {
            return SiteDriver.FindElement(AppealCreatorPageObjects.ClaimNoValueCssLocator, How.CssSelector).Text;
        }
        public int GetClaimLinesCount()
        {
            return SiteDriver.FindElementsCount(AppealCreatorPageObjects.ClaimLinesCountCssLocator, How.CssSelector);
        }
        public bool IsLineNoForClaimLinesPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.LineNoIconCssLocator, row), How.CssSelector);
        }
        public string GetLineNoForClaimLines(int row)
        {
            return SiteDriver.FindElement(String.Format(AppealCreatorPageObjects.LineNoIconCssLocator, row), How.CssSelector).Text;  
        }
        public bool IsDateOfServicePresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.DateofServiceCssLocator, row), How.CssSelector);
        }
        public bool IsProcCodePresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.ProcCodeCssLocator, row), How.CssSelector);
        }
        public string GetProcCodeValue(int row)
        {
            return SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.ProcCodeCssLocator, row), How.CssSelector).Text;
        }

        public AppealCreatorPage ClickOnProcCode(int row)
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.ProcCodeCssLocator, row), How.CssSelector).Click();
            Console.WriteLine("Clicked on Proc Code");
             return new AppealCreatorPage(Navigator, _appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage ClickOnProcedureCodeandSwitch(string title, int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.ProcCodeCssLocator, row), How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public bool IsProcDescriptionToolTipPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.ProcDescriptionCssLocator,row), How.CssSelector);
        }

        //public string GetProcDescription()
        //{
        //    return SiteDriver.FindElement(AppealCreatorPageObjects.ProcDescriptionCssLocator, How.CssSelector).Text;
        //}
        public string GeProcDescriptionToolTip(int row)
        {
            return 
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.ProcDescriptionCssLocator, row),
                    How.CssSelector).GetAttribute("title");
        }
        public void SwitchToPopUpWindow()
        {
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
        }
        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealCreator.GetStringValue());
                }
            }
        }
        public void CloseAllExistingPopupIfExist()
        {
            while(SiteDriver.WindowHandles.Count !=1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealCreator.GetStringValue());
                }
            }
        }
        public bool IsAppealLevelPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealLevelCssLocator, row), How.CssSelector);
        }
        public bool IsAppealLevelIconPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.AppealLevelIconCssLocator, row), How.CssSelector);
        }
        public string GetAppealLevelIconValue(int row)
        {
            if (IsAppealLevelPresent(row))
            {
                return
                    SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.AppealLevelIconCssLocator, row), How.CssSelector)
                        .Text;
            }
            else return null;
        }
        public bool IsBilledAmountPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.BilledAmountCssLocator, row), How.CssSelector);
        }
        public string GetRevCodeValue(int row)
        {
            return SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.RevCodeCssLocator, row), How.CssSelector).Text;
        }
        public void ClickOnRevCode(int row)
        {
            if (GetRevCodeValue(row) != "")
            {
                SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.RevCodeCssLocator, row), How.CssSelector).Click();
                Console.WriteLine("Clicked on Rev Code");
            }
        }
        public NewPopupCodePage ClickOnRevenueCodeandSwitch(string title, int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealCreatorPageObjects.RevCodeCssLocator, row), How.CssSelector);
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
        public bool IsFlagDivPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.FlagDivCssLocator, row), How.CssSelector);
        }

        public bool AreFlagsPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCreatorPageObjects.FlagCssLocator, row), How.CssSelector);
        }

        public bool AreUnreviewedFlagsPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.BoldRedFlagCssLocator, How.CssSelector);
        }
        public bool AreReviewedFlagsPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.RedFlagCssLocator, How.CssSelector);
        }
        public bool AreAutoApprovedFlagsPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.GrayFlagCssLocator,  How.CssSelector);
        }
        public string GetFlagValue(int row)
        {
            if (AreFlagsPresent(row))
            {
                return
                    SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.FlagCssLocator, row), How.CssSelector)
                        .GetAttribute("title");
            }
            else return null;
        }



        public bool IsClaimSearchPanelPresent()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ClaimSearchPanelSectionCssLocator, How.CssSelector);
        }

        public bool IsClaimSearchPanelHidden()
        {
            return SiteDriver.IsElementPresent(AppealCreatorPageObjects.ClaimSearchPanelDisabledSectionCssLocator, How.CssSelector);
        }

        public string GetSearchListMessageOnLeftGridForEmptyData()
        {
            return
                SiteDriver.FindElement(AppealCreatorPageObjects.EmptySearchListMessageSectionCssLocator,
                    How.CssSelector).Text;
        }

        public void ToggleSearchButtonForClaimSearch()
        {
            var element = SiteDriver.FindElement(AppealCreatorPageObjects.SearchIconButtonInAppealCreatorLeftComponent,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitToLoadNew(1000);
        }
        public void PassFilePathForDocumentUpload()
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/Test.txt");
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(AppealCreatorPageObjects.AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
        }
        public void PassGivenFileNameFilePathForDocumentUpload(string filename)
        {
            //SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/"+ filename);
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(AppealCreatorPageObjects.AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);

        }
        public void SetAppealCreatorUploaderFieldValue(string description, int row)
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.AppealCreatorUploaderFieldValueCssLocator, row), How.CssSelector)
                .SendKeys(description);
        }
        public void SetAppealCreatorFieldValue(string fieldname, string description  )
        {
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.FieldInputXpath, fieldname), How.XPath)
                .ClearElementField();
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.FieldInputXpath, fieldname), How.XPath).Click();
            SiteDriver.FindElement(string.Format(AppealCreatorPageObjects.FieldInputXpath, fieldname), How.XPath).SendKeys(description);
        }
        public string GetAppealCreatorUploaderFieldLabel(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealCreatorPageObjects.AppealCreatorUploaderFieldLabelCssLocator, row), How.CssSelector).Text;
        }
        public string GetAppealCreatorUploaderFieldValue(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealCreatorPageObjects.AppealCreatorUploaderFieldValueCssLocator, row), How.CssSelector).GetAttribute("value");
        }


       /// Click on add file button
       /// </summary>
       public void ClickOnAddFileBtn()
       {
           SiteDriver.FindElement(AppealCreatorPageObjects.AddDocumentButtonXpath, How.XPath).Click();
           Console.WriteLine("Add document in  Appeal Document Upload");
       }

       public bool IsFileToUploadPresent()
       {
           return SiteDriver.IsElementPresent(AppealCreatorPageObjects.FileToUploadSectionXpath, How.XPath);
       }
       public string FileToUploadDocumentValue(int row,  int col)
       {
           return SiteDriver.FindElement(
               string.Format(AppealCreatorPageObjects.FileToUploadDetailsXpath, row,col), How.XPath).Text;
       }
       public bool AddFileForUpload(string fileType, int row, string fileName)
       {
           PassGivenFileNameFilePathForDocumentUpload(fileName);
           SetFileTypeListVlaue(fileType);
           SetAppealCreatorFieldValue("Description", "test appeal doc");
           ClickOnAddFileBtn();
           if (!IsFileToUploadPresent())
           {

               Console.WriteLine("File not present for upload"); 
               return false;
           }
            Console.WriteLine("File is present for upload"); 
            if (!FileToUploadDocumentValue(row, 1).Equals(fileName))
            {
                Console.WriteLine("Added document doesn't correspond to uploaded doc details");
                return false;
            }
            Console.WriteLine("Added document  correspond to uploaded doc details");
            
            return true;
       }

        //public SideBarPanelSearch GetSideBarPanelSearch
        //{
        //    get { return _sideBarPanelSearch; }
        //}

        //public GridViewSection GetGridViewSection
        //{
        //    get { return _gridViewSection; }
        //}


        #endregion

        public void NavigateToAppealCreatorUsingDifferentUser(string claimSeq)
        {

            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });
            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).LoginAsHciAdminUser1().NavigateToAppealCreator().SearchByClaimSequence(claimSeq); 
            SiteDriver.SwitchWindow(handles[0]);
        }

        public void CloseWindowThatLocksClaim()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            SiteDriver.SwitchWindow(handles[1]);
            Logout().LoginAsHciAdminUser();
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindow(handles[0]);
            

        }

        public AppealCreatorPage SwitchToOpenAppealCreatorByUrlForAdmin(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).LoginAsHciAdminUser();
            var appealCreatorpage = Navigator.Navigate<AppealCreatorPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealCreatorPageObjects.PageHeaderCssLocator, How.CssSelector));
            var appealCreator = new AppealCreatorPage(Navigator, appealCreatorpage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            return appealCreator;
        }

        public string CalculateAndGetAppealDueDateFromDatabase(string turnaroundTime,string type="Business")
        {
            if (type == "Business")
            {
                return Executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDate, turnaroundTime));
            }
            
            else
            {
                return Executor.GetSingleStringValue(
                    string.Format(AppealSqlScriptObjects.GetAppealDueDateForCalendarType, turnaroundTime));
                
            }
        }

        public string GetAppealCreatorHeaderValues(int col)
        {
            return SiteDriver
                .FindElement(string.Format(AppealCreatorPageObjects.AppealCreatorHeaderLocatorTemplate, col),
                    How.CssSelector).Text;
        }

        public void ChangeDCIProductForClientFromDB(string client,bool active = true)
        {
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.EnableDisableDCIForClient, client,(active)?'T':'F'));
            Console.WriteLine($"Set DCA product for client : {client} to {active}");
        }
    }
}
