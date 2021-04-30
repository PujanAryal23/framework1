using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageObjects.Default;
using System.IO;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using static System.String;
namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealManagerPage : NewDefaultPage
    {
        private readonly string _originalWindow;
        private readonly AppealManagerPageObjects _appealManagerPage;
        private readonly CalenderPage _calenderPage;
        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly FileUploadPage _fileUpload;
        private readonly CommonSQLObjects _commonSqlObjects;

        #region CONSTRUCTOR

        public AppealManagerPage(INewNavigator navigator, AppealManagerPageObjects appealManagerPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealManagerPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealManagerPage = appealManagerPage;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _calenderPage = new CalenderPage(SiteDriver);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _fileUpload = new FileUploadPage(SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _commonSqlObjects = new CommonSQLObjects(Executor);
        }



        #endregion

        #region PUBLIC METHODS

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public FileUploadPage GetFileUploadPage
        {
            get { return _fileUpload; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        #region database interaction

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public List<string> GetAssociatedPlansForClient(string client)
        {
            var planList =
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.ClientWisePlanList, client));
            Console.WriteLine("The count of plans associated with client: {0} is {1}", client, planList[0]);
            return planList;
        }

        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetAssignedClientList, userId));
        }

        public List<string> GetPrimaryReviewerAssignedToList()
        {
            var userList =
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.UserListHavingAppealCanBeAssignedAuthority));
            userList = userList.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList();
            userList.Sort();
            Console.WriteLine("The list of users with 'Appeals can be assigned to user' authority ");
            return userList;
        }

        public string GetCountOfDeletedAppealByAppealSequenceList(List<string> appealSeqList)
        {
            var appealSequences = string.Join(",", appealSeqList);
            var result =
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetCountOfDeletedAppealByAppealSequenceList, appealSequences));
            Executor.CloseConnection();
            if (result == null)
                return "0";
            return result[0];
        }



        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }
        #endregion

        #region 3 column Appeal Manager Page

        public bool IsSelectAllCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.SelectAllCheckBoxCssLocator, How.CssSelector);
        }

        public void ClickOnSelectAllCheckBox(bool check = true)
        {
            var element = SiteDriver.FindElement(AppealManagerPageObjects.SelectAllCheckBoxCssLocator,
                How.CssSelector);
            if (!element.GetAttribute("class").Contains("selected") && check)
                element.Click();
            else if (element.GetAttribute("class").Contains("selected") && !check)
                element.Click();

            Console.WriteLine("Select All CheckBox Clicked");
        }
        #endregion


        #region Appeal Details

        public string GetAppealDetailsValueByLabel(string label)
        {
            return JavaScriptExecutor.FindElement(string.Format(AppealManagerPageObjects.AppealDetailsValueByLabelCssLocator, label)).Text;
        }

        public string GetFlagInAppealDetails()
        {
            return JavaScriptExecutor.FindElement(AppealManagerPageObjects.FlagDetailsInAppealDetailsCssLocator).Text;
        }

        #endregion

        #region AppealSearchResult

        public bool IsEditIconDisplayed()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.EditIconCssLocator, How.CssSelector);
        }

        public bool IsDeleteIconDisplayed()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.DeleteIconCssLocator, How.CssSelector);
        }
        
        public AppealManagerPage SwitchBackToAppealManagerPage()
        {
            var appealManager = Navigator.Navigate<AppealManagerPageObjects>(() => SiteDriver.SwitchWindowByUrl("appeal_manager"));
            return new AppealManagerPage(Navigator, appealManager, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAppealLevelBadgeValuePresentForMRRAppealType()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.AppealLevelBadgeValueForMRRAppealType, How.XPath);
        }

        public AppealActionPage SwitchBackToAppealActionPage()
        {
            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
            return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealManagerPage ClosePopupWindowAndSwitchBackToAppealManagerPage()
        {
            if (SiteDriver.Url.Contains("appeal_manager"))
                SiteDriver.SwitchWindowByUrl("appeals");
            var appealManager = Navigator.Navigate<AppealManagerPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByUrl("appeal_manager");
            });
            return new AppealManagerPage(Navigator, appealManager, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public AppealManagerPage CloseAllPopupWindowAndSwitchBackToAppealManagerPage()
        {
            if (SiteDriver.Url.Contains("appeal_manager"))
                SiteDriver.SwitchWindowByUrl("appeals");
            var appealManager = Navigator.Navigate<AppealManagerPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByUrl("appeals");
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByUrl("appeal_manager");
            });
            return new AppealManagerPage(Navigator, appealManager, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public AppealActionPage ClickOnAppealSequenceByStatusOrAppealToGoAppealAction(string status = "New")
        {
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(AppealSearchPageObjects.AppealSequenceByStatusOrAppealXPathTemplate, status), How.XPath).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealActionPage(Navigator, newAppealActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealActionPage ClickOnAppealSequenceSwitchToAppealAction(string appealSequence)
        {
            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                JavaScriptExecutor.ClickJQuery(
                    string.Format(
                        AppealManagerPageObjects.AppealSequenceInAppealSearchResultByAppealSequenceCssTemplate,
                        appealSequence));
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new AppealActionPage(Navigator, appealAction,SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
        }

        public AppealSummaryPage ClickOnAppealSequenceSwitchToAppealSummary(string appealSequence)
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                JavaScriptExecutor.ClickJQuery(
                    string.Format(
                        AppealManagerPageObjects.AppealSequenceInAppealSearchResultByAppealSequenceCssTemplate,
                        appealSequence));
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_summary"));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealLetterPage ClickOnAppealLetter(int row)
        {
            var appealLetter = Navigator.Navigate<AppealLetterPageObjects>(() =>
            {
                SiteDriver.FindElement(string.Format(AppealSearchPageObjects.AppealLetterIconCssTemplate, row), How.CssSelector).Click();
                Console.WriteLine("Clicked on Appeal Letter");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            });
            return new AppealLetterPage(Navigator, appealLetter, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public bool IsAppealLetterIconPrsentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                string.Format(AppealSearchPageObjects.AppealLetterIconCssTemplate, row), How.CssSelector);
        }

        public string GetAdjustValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSearchPageObjects.AdjustCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetPayValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSearchPageObjects.PayCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetDenyValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSearchPageObjects.DenyCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetNoDocsValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealSearchPageObjects.NoDocsCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public bool IsBoldRedColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.BoldRedColorDueDateCssTemplate,
                How.CssSelector, 200);
        }

        public bool IsNonBoldOnlyRedColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.NonBoldOnlyRedColorDueDateCssTemplate,
                How.CssSelector, 200);
        }

        public bool IsOrangeColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.OrangeColorDueDateCssTemplate,
                How.CssSelector, 200);
        }

        public bool IsBlackColorDueDatePresent(int row = 1)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(AppealManagerPageObjects.BlackColorDueDateByAppealSequenceCssTemplate, row));
        }

        public bool IsAppealLockIconPresent(string appealSeq)
        {
            return JavaScriptExecutor.IsElementPresent(
                   string.Format(AppealManagerPageObjects.LockIconByAppealSequenceCssTemplate, appealSeq));
        }

        public string GetAppealLockIconTooltip(string appealSeq)
        {
            return JavaScriptExecutor.FindElement(
                   string.Format(AppealManagerPageObjects.LockIconByAppealSequenceCssTemplate, appealSeq)).GetAttribute("title");
        }

        public bool IsUrgentIconPresent(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealSearchPageObjects.UrgerIconCssTemplate, row),
                How.CssSelector);
        }

        public int GetAppealSearchResultRowCount()
        {
            return SiteDriver.FindElementsCount(AppealManagerPageObjects.AppealSearchResultCssLocator, How.CssSelector);
        }

        public List<bool> GetUrgentList()
        {
            ClickOnFilterOption();
            var list = SiteDriver.FindElementsAndGetAttributeByClass("field_error",
                AppealManagerPageObjects.UrgentListCssLocator, How.CssSelector);
            ClickOnFilterOption();
            return list;

        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(AppealManagerPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;


        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public List<String> GetSearchResultListByCol(int col, bool removeNull = true)
        {

            var list = JavaScriptExecutor.FindElements(
                  string.Format(AppealManagerPageObjects.AppealSearchResultListCssTemplate, col), How.CssSelector, "Text");
            if (removeNull)
                list.RemoveAll(String.IsNullOrEmpty);
            return list;
        }

        public String GetSearchResultByColRow(int col, int row = 1)
        {
            var t =
                SiteDriver.FindElement(
                    string.Format(AppealManagerPageObjects.AppealSearchResultByRowColCssTemplate, col, row),
                    How.CssSelector).Text;
            return t;
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInAscendingOrder();
        }

        public bool IsListDateSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder();
        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder();
        }

        #endregion

        public string GetNoMatchingRecordFoundMessage()
        {
            return _sideBarPanelSearch.GetNoMatchingRecordFoundMessage();
        }

        public void SetDateFieldFrom(string dateName, string date)
        {
            _sideBarPanelSearch.SetDateFieldFrom(dateName, date);
        }
        public string GetDateFieldPlaceholder(string label, int col)
        {
            return
                _sideBarPanelSearch.GetDateFieldPlaceholder(label, col);
        }

        public string GetDateFieldFrom(string dateName)
        {
            return _sideBarPanelSearch.GetDateFieldFrom(dateName);
        }

        public string GetDateFieldTo(string dateName)
        {
            return _sideBarPanelSearch.GetDateFieldTo(dateName);
        }

        public void SetDateFieldTo(string dateName, string date)
        {
            _sideBarPanelSearch.SetDateFieldTo(dateName, date);
        }

        public string GetFieldErrorIconTooltipMessage(string label)
        {
            return _sideBarPanelSearch.GetFieldErrorIconTooltipMessage(label);
        }
        public bool IsAdvancedSearchFilterIconDispalyed()
        {
            return _sideBarPanelSearch.IsAdvancedSearchFilterIconDispalyed();
        }
        public void ClickOnAdvancedSearchFilterIcon(bool click)
        {
            _sideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(click);
        }

        public void WaitToOpenCloseWorkList(bool condition = true)
        {
            if (condition)
                _sideBarPanelSearch.WaitSideBarPanelClosed();
            else
                _sideBarPanelSearch.WaitSideBarPanelOpen();
        }

        public bool IsSideBarPanelOpen()
        {
            return _sideBarPanelSearch.IsSideBarPanelOpen();
        }

        public bool IsSearchInputFieldDispalyedByLabel(string label)
        {
            return _sideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(label);
        }

        public int FilterCountByLabel()
        {
            return _sideBarPanelSearch.FilterCountByLabel();
        }

        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public bool IsSideBarIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
        }

        public bool IsHeaderEditIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.HeaderEditIconCssLocator, How.CssSelector);
        }

        public List<string> GetUnSelectedListIn3ColumnPage()
        {
            return JavaScriptExecutor.FindElements(
                AppealManagerPageObjects.UnSelectedAppealListXPath, How.XPath,
                "Text");
        }

        public List<string> GetSelectedListIn3ColumnPage(string columnName)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(AppealManagerPageObjects.SelectedAppealListXPathTemplate, columnName), How.XPath,
                "Text");
        }

        public void ClickOnSelectAppealListIn3ColumnPageByRow(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealManagerPageObjects.UnSelectedAppealListXPath + "[{0}]",
                    row), How.XPath);
        }

        public void ClickOnSelectedAppealListIn3ColumnPageByRow(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealManagerPageObjects.SelectedAppealListXPathTemplate + "[{1}]", "Selected Appeals",
                    row), How.XPath);
        }

        public void ClickOnSelectAppealListIn3ColumnPageByAppeal(string appealSeq)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealManagerPageObjects.SearchListByAppealSeqSelectorTemplate, appealSeq), How.XPath);
        }

        public void ClickOnSelectedAppealListIn3ColumnPageByAppeal(string appealSeq) =>
            JavaScriptExecutor.ClickJQuery(Format(AppealManagerPageObjects.SelectAppealInSelectedAppealsColumnCssLocator, appealSeq));
        
        public string GetPrimaryReviewerIn3colTemplate(string appealSeq)
        {
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.GetPrimaryReviewerOrAssignedToIn3colTemplate,
                 appealSeq, "Primary Reviewer"), How.XPath).Text;
        }


        public List<string> GetAppealsUsingAnalysts(string analyst1, string analyst2, string userseq)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetAppealsUsingAnalysts, analyst1, analyst2,
                userseq));
        }




        public string GetAssignedToIn3colTemplate(string appealSeq)
        {
            var catchThis = SiteDriver.FindElement(string.Format(AppealManagerPageObjects.GetPrimaryReviewerOrAssignedToIn3colTemplate,
                appealSeq, "Assigned To"), How.XPath).Text;
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.GetPrimaryReviewerOrAssignedToIn3colTemplate,
                appealSeq, "Assigned To"), How.XPath).Text;
        }

        public void WaitToReturnAppealManagerSearchPage()
        {
            SiteDriver.WaitForCondition(IsHeaderEditIconPresent);
            _sideBarPanelSearch.WaitSideBarPanelOpen();
        }

        public void ClickOnHeaderEditIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.HeaderEditIconCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(IsSelectAllCheckBoxPresent);
            Console.WriteLine("Clicked on Header Edit Icon");
        }

        public bool IsNoMatchingRecordFoundMessagePresent()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.NoMatchingRecordFoundCssSelector, How.CssSelector);
        }

        public void SetInputFieldByInputLabel(string label, string value)
        {
            _sideBarPanelSearch.SetInputFieldByLabel(label, value);
        }

        public void SelectDropDownListbyInputLabel(string label, string value, bool directSelect = true)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel(label, value, directSelect);
        }

        public void SelectDropDownListbyInputLabel3Column(string label, string value)
        {
            SiteDriver.WaitToLoadNew(300);
            
                JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.InputFieldByLabelXpathTemplate, label), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(
                    string.Format(AppealManagerPageObjects.InputFieldByLabelXpathTemplate, label), How.XPath)
                .SendKeys(value);
                if (!SiteDriver.IsElementPresent(string.Format(AppealManagerPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath))
                    JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.InputFieldByLabelXpathTemplate, label), How.XPath);
                JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        //public string GetInputValueByLabel(string label)
        //{
        //    return _sideBarPanelSearch.GetInputValueByLabel(label);
        //}
        public void SelectSearchDropDownListForMultipleSelectValue(string label, string value)
        {
            _sideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, value);
        }

        public List<string> GetAvailableDropDownList(string label, bool singleSelect = true)
        {
            if (singleSelect)
                return _sideBarPanelSearch.GetAvailableDropDownList(label);
            return _sideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
        }


        public List<string> GetMultiSelectListedDropDownList(string label)
        {
            return _sideBarPanelSearch.GetMultiSelectListedDropDownList(label);

        }

        public List<string> GetAppealSeqListSortedByCreateDate(int col)
        {
            var list = GetSearchResultListByCol(4);
            return list;
        }

        public string GetToolTipOfSearchResultByColRow(int col, int row = 1)
        {
            var t =
                SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealSearchResultByRowColCssTemplate, col, row),
                    How.CssSelector).GetAttribute("title");
            return t;
        }


        #region EditApeal

        public void SelectDropDownOnEditSection(string label, string value)
        {
            JavaScriptExecutor.ExecuteClick("//label[text()='Appeal Type']/..//input", How.XPath);
            JavaScriptExecutor.ExecuteClick("//label[text()='Appeal Type']/..//section/ul/li[text()='Record Review']", How.XPath);

        }

        public bool IsCursorInDueDate()
        {
            return
                SiteDriver.ReturnActiveElement()
                    .Location.Equals(
                        JavaScriptExecutor.FindElement(
                            string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, "Due Date")
                                ).Location);
        }

        public void SendTabKeyInDueDate()
        {
            JavaScriptExecutor.FindElement(
                            string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, "Due Date")).SendKeys(Keys.Tab);
        }

        public List<string> GetAvailableDropDownListInEditAppealByLabel(string label)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, label));
            Console.WriteLine("Looking for <{0}> List", label);
            SiteDriver.WaitToLoadNew(200);
            var list = JavaScriptExecutor.FindElements(string.Format(AppealManagerPageObjects.DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ClickJQuery(string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, label));
            Console.WriteLine("<{0}> Drop down list count is {1} ", label, list.Count);
            return list;
        }

        public string GetInputValueInDropDownListInEditAppealByLabel(string label)
        {
            return JavaScriptExecutor.GetText(string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, label));
        }

        public void MouseOutFromInputField(string label)
        {
            JavaScriptExecutor.ExecuteMouseOut(string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, label));
        }

        #endregion


        public List<string> SelectedDropDownOptionsByLabel(string label)
        {
            return _sideBarPanelSearch.SelectedDropDownOptionsByLabel(label);
        }
        public void SetDueDateInEditAppealForm(string date, bool closePopup = true)
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.DueDateInEditAppealFormSectionXpath, How.XPath);
            SiteDriver.WaitToLoadNew(500);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(500);
            if (IsPageErrorPopupModalPresent() && closePopup)
                ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Due Date Selected:<{0}>", date);
        }


        public void SendEnterKeysOnTextFieldByLabel(string label = "Client")
        {
            _sideBarPanelSearch.SendEnterKeysOnTextFieldByLabel(label);
        }
        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.ButtonXPathTemplate, "Find"),
                How.XPath);
            Console.WriteLine("Find Button Clicked");
        }

        public void ClickOnSaveButton()
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.ButtonXPathTemplate, "Save"),
                How.XPath);
            Console.WriteLine("Save Button Clicked");
        }



        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.ClearCancelXPathTemplate, "Cancel"),
                How.XPath);
            Console.WriteLine("Cancel Link Clicked");
        }

        public void ClickOnClearLink()
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.ClearCancelXPathTemplate, "Clear"),
                How.XPath);
            Console.WriteLine("Clear Link Clicked");
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

        public bool IsInputFieldForRespectiveLabelDisabled(string label)
        {
            return _sideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled(label);
        }

        public string GetInputValueByLabel(string label)
        {
            return
                _sideBarPanelSearch.GetInputValueByLabel(label);
        }
        public string GetInputAttributeValueByLabel(string label, string attribute)
        {
            return
                _sideBarPanelSearch.GetInputAttributeValueByLabel(label, attribute);
        }
        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public void ClickOnEditIcon(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.EditIconByRowCssLocator, row), How.CssSelector);
            SiteDriver.WaitForCondition(IsEditFormDisplayed);
            Console.WriteLine("Click On Small Edit Icon");
        }

        public bool IsEditIconDisabledByAppealSequence(string appeal)
        {
            return
                JavaScriptExecutor.IsElementPresent(string.Format(AppealManagerPageObjects.DisabledEditIconCssTemplate,
                    appeal));
        }

        public string GetToolTipOfDisabledEditIconByAppealSequence(string appeal)
        {
            return JavaScriptExecutor.FindElement(
                string.Format(AppealManagerPageObjects.DisabledEditIconCssTemplate,
                    appeal)).GetAttribute("title");
        }

        public void ClickOnEditIconByAppealRowSelector(string appeal)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(AppealManagerPageObjects.AppealListRowEditIconCssTemplate, appeal));
            SiteDriver.WaitForCondition(IsEditFormDisplayed);
            Console.WriteLine("Clicked On Small Edit Icon");
        }
        public void ClickOnDeleteIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.DeleteIconCssLocator, How.CssSelector);
            Console.WriteLine("Click On Small Delete Icon");

        }


        public void ClickOnDeleteIconByRowSelector(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.DeleteIconTemplateCssLocator, row), How.CssSelector);
            Console.WriteLine("Clicked On Small Delete Icon for " + row);

        }

        public bool IsEditFormDisplayed()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.EditFormCssLocator, How.CssSelector);
        }

        public int GetSearchResultCount()
        {
            return SiteDriver.FindElementsCount(AppealManagerPageObjects.SearchResultListCountCssSelector,
                How.CssSelector);
        }
        public void SearchByAppealSequence(string appealSeq)
        {
            SelectAllAppeals();
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            SetInputFieldByInputLabel("Appeal Sequence", appealSeq);
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            ClickOnFindButton();
            WaitForWorking();
        }

        public void SearchByClaimSequence(string claimSeq, string client = null)
        {
            SelectAllAppeals();
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            if (client == null)
                SelectSMTST();
            else
                SelectDropDownListbyInputLabel("Client", client);
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            SetInputFieldByInputLabel("Claim Sequence", claimSeq);
            ClickOnFindButton();
            WaitForWorking();
        }
        public void SearchOutstandingAppealByClaimSequence(string claimSeq, string client = null)
        {
            SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            if (client == null)
                SelectSMTST();
            else
                SelectDropDownListbyInputLabel("Client", client);
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            SetInputFieldByInputLabel("Claim Sequence", claimSeq);
            ClickOnFindButton();
            WaitForWorking();
        }

        public void SearchByClaimNo(string claimNo)
        {
            SelectAllAppeals();
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            SelectSMTST();
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            SetInputFieldByInputLabel("Claim No", claimNo);
            ClickOnFindButton();
            WaitForWorking();
        }

        public void SetAppealSequence(string appealSeq)
        {
            SetInputFieldByInputLabel("Appeal Sequence", appealSeq);
        }

        public void SetClaimSequence(string appealSeq)
        {
            SetInputFieldByInputLabel("Claim Sequence", appealSeq);
        }

        public void SetClaimNo(string appealSeq)
        {
            SetInputFieldByInputLabel("Claim No", appealSeq);
        }

        public void SelectAllAppeals()
        {
            SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
        }

        public void SelectAppealsDueToday()
        {
            SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue());
        }

        public void SelectOutstandingAppeals()
        {
            SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
        }

        public void SelectSMTST()
        {
            SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
        }

        public void CloseAppeal(string claimseq = null, string appealSeq = null)
        {
            if (!String.IsNullOrEmpty(appealSeq)) SearchByAppealSequence(appealSeq);
            if (!String.IsNullOrEmpty(claimseq)) SearchByClaimSequence(claimseq);

            ClickOnEditIcon();
            SelectDropDownListbyInputLabel("Status", "Closed");
            SetNote("UINOTE");
            ClickOnSaveButton();
            WaitForWorking();
        }

        public void ChangeStatusOfAppeal(string claimseq = null, string appealSeq = null, string newStatus = "New")
        {
            if (!String.IsNullOrEmpty(appealSeq)) SearchByAppealSequence(appealSeq);
            if (!String.IsNullOrEmpty(claimseq)) SearchByClaimSequence(claimseq);

            ClickOnEditIcon();
            SelectDropDownListbyInputLabel("Status", newStatus);
            SetNote("UINOTE");
            if (String.IsNullOrEmpty(GetInputValueByLabel("Due Date"))) SetDueDateInEditAppealForm(DateTime.Now.ToString("MM/d/yyyy"));
            ClickOnSaveButton();
            WaitForWorking();
        }
        public void ChangeStatusOfAppealByRowSelection(int row = 1, string newStatus = "New")
        {
            ClickOnEditIcon(row);
            var oldStatus = GetInputValueByLabel("Status");
            SelectDropDownListbyInputLabel("Status", newStatus);
            SetNote("UINOTE");
            if (String.IsNullOrEmpty(GetInputValueByLabel("Due Date")) && (oldStatus != "Complete" || oldStatus != "Closed"))
                SetDueDateInEditAppealForm(DateTime.Now.ToString("MM/d/yyyy"));
            ClickOnSaveButton();
            WaitForWorking();
        }
        public void ChangeAppealStatus(string newStatus)
        {
            SelectDropDownListbyInputLabel("Status", newStatus);
            SetNote("UINOTE");
            if (String.IsNullOrEmpty(GetInputValueByLabel("Due Date"))) SetDueDateInEditAppealForm(DateTime.Now.ToString("MM/d/yyyy"));
            ClickOnSaveButton();
            WaitForWorking();

        }
        public void ChangeDueDateOfAppeal(string appealSeq, string dueDate = null)
        {
            ClickOnEditIconByAppealRowSelector(appealSeq);
            SetDueDateInEditAppealForm(dueDate ?? DateTime.Now.ToString("MM/d/yyyy"));
            SetNote("UINOTE");
            ClickOnSaveButton();
            WaitForWorking();
        }

        public void ChangeDueDateOfAppealByRow(int row = 1, string dueDate = null)
        {
            ClickOnEditIcon(row);
            SetDueDateInEditAppealForm(dueDate ?? DateTime.Now.ToString("MM/d/yyyy"));
            SetNote("UINOTE");
            ClickOnSaveButton();
            WaitForWorking();
        }
        public void ClearDueDate()
        {
            JavaScriptExecutor.FindElement(string.Format(AppealManagerPageObjects.InputFieldInEditAppealByLabelCssTemplate, "Due Date")).ClearElementField();
            SendTabKeyInDueDate();
            ClosePageError();
        }

        public void ChangeUrgentTypeOfAppeal(string urgent = null, string claimseq = null, string appealSeq = null)
        {
            if (!String.IsNullOrEmpty(appealSeq)) SearchByAppealSequence(appealSeq);
            if (!String.IsNullOrEmpty(claimseq)) SearchByClaimSequence(claimseq);

            ClickOnEditIcon();
            SelectDropDownListbyInputLabel("Urgent", urgent ?? PriorityEnum.Normal.GetStringValue());
            SetNote("UINOTE");
            ClickOnSaveButton();
            WaitForWorking();
        }

        public void ChangeStatusOfListedAppeal(string newStatus, string appealSeq)
        {
            ClickOnEditIconByAppealRowSelector(appealSeq);
            SelectDropDownListbyInputLabel("Status", newStatus);
            SetNote("UINOTE");
            if (String.IsNullOrEmpty(GetInputValueByLabel("Due Date"))) SetDueDateInEditAppealForm(DateTime.Now.ToString("MM/d/yyyy"));
            ClickOnSaveButton();
            WaitForWorking();
        }

        public void DeleteAppealsAssociatedWithClaim(string claimSeq)
        {
            SearchByClaimSequence(claimSeq);
            if (!IsNoMatchingRecordFoundMessagePresent())
            {
                var listCount = GetSearchResultCount();

                for (var i = 0; i < listCount; i++)
                {
                    //to delete appeals created and undeleted when test fails
                    ClickOnDeleteIconByRowSelector(1);
                    ClickOkCancelOnConfirmationModal(true);
                    Console.WriteLine("Deleting the appeal hence created for the test");
                    ClickOnFindButton();
                    WaitForWorking();
                }

            }
        }

        public void DeleteAppealsAssociatedWithClaim(List<string> claimSeq)
        {
            SelectAllAppeals();
            SelectSMTST();
            for (var i = 0; i < claimSeq.Count; i++)
            {
                SetInputFieldByInputLabel("Claim Sequence", claimSeq[i]);
                ClickOnFindButton();
                WaitForWorking();
                if (!IsNoMatchingRecordFoundMessagePresent())
                {
                    ClickOnDeleteIconByRowSelector(1);
                    ClickOkCancelOnConfirmationModal(true);
                    Console.WriteLine("Deleting the appeal hence created for the test");
                    ClickOnFindButton();
                    WaitForWorking();
                }
            }


        }

        public bool DeleteAppealByAppealSeq(string appealSeq)
        {
            SearchByAppealSequence(appealSeq);
            ClickOnDeleteIcon();
            ClickOkCancelOnConfirmationModal(true);

            SearchByAppealSequence(appealSeq);
            return IsNoMatchingRecordFoundMessagePresent();
        }

        public void DeleteLastCreatedAppeal(string claimSeq)
        {
            SearchByClaimSequence(claimSeq);
            if (!IsNoMatchingRecordFoundMessagePresent())
            {
                var resultCount = GetAppealSearchResultRowCount();
                while (resultCount != 0)
                {
                    ClickOnDeleteIconByRowSelector(1);
                    ClickOkCancelOnConfirmationModal(true);
                    resultCount--;
                }
            }
            //ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Delete Last Created Appeal");
            WaitForWorking();
        }
        public void ClickOnSearchListRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.SearchListRowSelectorTemplate, row), How.CssSelector);
            WaitForWorkingAjaxMessage();
        }
        public void ClickOnSearchListRowByAppealSequence(string appealSequence)
        {
            Console.WriteLine("Clicked on appeal result list with appeal sequence :{0}", appealSequence);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.SearchListByAppealSeqSelectorTemplate, appealSequence), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public string GetSearchResultByAppealSeq(string appealSeq, int col)
        {
            Console.WriteLine("Get value of appeal details from search list for appeal:{0}, and column {1}", appealSeq, col);
            return
                SiteDriver.FindElement(
                    string.Format(AppealManagerPageObjects.SearchResultByAppealSeqXpathTemplate, appealSeq, col), How.XPath)
                    .Text;
        }
        #region Appeal Details section

        public bool IsVerticalScrollBarPresentInClientNoteSection()
        {
            const string select = AppealManagerPageObjects.ClientNotesDivCssLocator;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public int GetScrollHeight(string select)
        {
            return JavaScriptExecutor.ScrollHeight(select);
        }

        public int GetClientHeight(string select)
        {
            return JavaScriptExecutor.ClientHeight(select);
        }


        public bool IsAppealDetailSectionOpen()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.AppealDetailsHeaderXpath, How.XPath);
        }

        public string GetAppealDetailsLabel(int row, int col)
        {
            Console.WriteLine("Get value of appeal details from details section for row:{0}, and column {1}", row, col);
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealDetailContentLabelXpathTemplate, row, col), How.XPath).Text;
        }
        public string GetAppealDetailsContentValue(int row, int col)
        {
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealDetailContentValueXpathTemplate, row, col), How.XPath).Text;
        }
        public string GetAppealDetailsContentValue(string label)
        {
            Console.WriteLine("Get value of appeal details from details section {0}", label);
            return JavaScriptExecutor.FindElement(string.Format(AppealManagerPageObjects.AppealDetailsValueByLabelCssLocator, label)).Text;
        }
        public bool IsNoDataMessagePresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.NoDataMessageCssSelector, How.CssSelector);
        }
        public bool IsNoDataMessagePresentInLeftSection()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.NoDataMessageLeftComponentCssSelector, How.CssSelector);
        }
        public string GetNoDataMessageText()
        {
            return SiteDriver.FindElement(AppealManagerPageObjects.NoDataMessageCssSelector, How.CssSelector).Text;
        }

        #region Upload Section
        public bool IsAppealDocumentUploadEnabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.AppealDocumentUploadEnabledXPath, How.XPath);
        }
        public bool IsAppealDocumentUploadDisabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.AppealDocumentUploadDisabledXPath, How.XPath);
        }

        public void ClickOnAppealDocumentUploadIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.AppealDocumentUploadEnabledXPath, How.XPath);
            Console.WriteLine("Clicked Appeal Document Upload Icon");
        }
        public bool IsAppealDocumentUploadSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.AppealDocumentUploadSectionCssLocator, How.CssSelector);
        }

        public bool IsAddFileButtonDisabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.DisabledAddDocumentButtonCssLocator, How.CssSelector);
        }


        public void PassFilePathForDocumentUpload()
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/Test.txt");
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(AppealManagerPageObjects.AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
        }


        public string GetAppealSummaryUploaderFieldLabel(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealManagerPageObjects.AppealManagerUploaderFieldLabelCssLocator, row), How.CssSelector).Text;
        }
        public void SetAppealSummaryUploaderFieldValue(string description, int row)
        {
            SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealManagerUploaderFieldValueCssLocator, row), How.CssSelector)
                .SendKeys(description);
        }
        public string GetAppealSummaryUploaderFieldValue(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealManagerPageObjects.AppealManagerUploaderFieldValueCssLocator, row), How.CssSelector).GetAttribute("value");
        }

        ///<summary>
        /// Click on add file button
        /// </summary>
        public void ClickOnAddFileBtn()
        {
            SiteDriver.FindElement(AppealManagerPageObjects.AddDocumentButtonCssLocator, How.CssSelector).Click();
            Console.WriteLine("Add document in  Appeal Document Upload");
        }

        public int DeleteIconCountOfFileList()
        {
            return SiteDriver.FindElementsCount(AppealManagerPageObjects.DeleteIconForFileListCssLocator, How.CssSelector);
        }
        public bool IsAppealDocumentDeleteDisabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.DeleteIconDisabledForFileListCssLocator, How.CssSelector);
        }
        /// <summary>
        /// Click on delete file button
        /// </summary>
        public void ClickOnDeleteFileBtn(int row)
        {
            SiteDriver.FindElement(string.Format(AppealManagerPageObjects.DeleteFileDocumentInAppealSummaryCssLocator, row), How.CssSelector).Click();

            Console.WriteLine("Delete document in  Appeal Document ");
        }

        public bool IsFileToUploadPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.FileToUploadSection, How.XPath);
        }
        public string FileToUploadDocumentValue(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(AppealManagerPageObjects.FileToUploadDetailsCssLocator, row, col), How.CssSelector).Text;
        }


        public List<string> GetAvailableFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            return JavaScriptExecutor.FindElements(AppealManagerPageObjects.FileTypeValueListCssLocator, How.CssSelector, "Text");

        }
        public string GetPlaceHolderValue()
        {
            return SiteDriver.FindElement(AppealManagerPageObjects.FileTypeCssLocator, How.CssSelector)
                .GetAttribute("placeholder");
        }
        public void SetFileTypeListVlaue(string fileType)
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            SiteDriver.FindElement(AppealManagerPageObjects.FileTypeCssLocator, How.CssSelector).SendKeys(fileType);
            JavaScriptExecutor.ExecuteClick(string.Format(AppealManagerPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(string.Format(AppealManagerPageObjects.FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
            Console.WriteLine("File Type Selected: <{0}>", fileType);
        }

        public List<string> GetSelectedFileTypeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.FileTypeToggleIconCssLocator, How.CssSelector);
            return JavaScriptExecutor.FindElements(AppealManagerPageObjects.FileTypeSelectedValueListCssLocator, How.CssSelector, "Text");

        }
        public List<DateTime> GetAddedAppealDocumentList()
        {
            return JavaScriptExecutor.FindElements(AppealManagerPageObjects.ListsofAppealDocumentsCssLocator, How.CssSelector, "Text").Select(DateTime.Parse).ToList();

        }
        public string AppealDocumentsListAttributeValue(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).Text;
        }
        public string GetAppealDocumentAttributeToolTip(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            return SiteDriver.FindElement(string.Format(AppealManagerPageObjects.AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).GetAttribute("title");
        }


        #endregion

        #endregion
        /// <summary>
        /// Click Clear All Button
        /// </summary>
        /// <returns></returns>
        public AppealManagerPage ClearAll()
        {
            JavaScriptExecutor.ExecuteClick(AppealManagerPageObjects.ClearFilterClass, How.CssSelector);
            Console.WriteLine("Clicked Clear Filter.");
            return new AppealManagerPage(Navigator, _appealManagerPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
            WaitForWorkingAjaxMessage();
        }

        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }


        /// <summary>
        /// Check whether values are in ascending order or not only for category
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool IsListInAscendingOrder(List<String> searchListValue)
        {
            var sorted = new List<String>(searchListValue);
            sorted.Sort(String.CompareOrdinal);
            for (var i = 0; i < sorted.Count; i++)
            {
                if (searchListValue[i].Equals(sorted[i])) continue;
                Console.WriteLine("{0} and {1}", searchListValue[i], sorted[i]);
                return false;
            }
            return true;
        }
        public bool IsGreyAppealLevelBadgeIconPresent()
        {
            var cssHexCode = SiteDriver.FindElement(AppealManagerPageObjects.GreyAppealLevelBadgeCssTemplate, How.CssSelector)
                .GetCssValue("background");
            Console.WriteLine("Vlaue of badge icon background color is {0}", cssHexCode);
            return cssHexCode.Contains("rgb(194, 194, 194)");
        }
        public bool IsAppealLevelBadgeIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.AppealLevelBadgeCssTemplate,
                How.CssSelector);
        }

        public List<string> GetAppealLevelBadgeValue()
        {
            return JavaScriptExecutor.FindElements(AppealManagerPageObjects.AppealLevelBadgeCssTemplate,
                How.CssSelector, "Text");
        }

        public bool IsMoveAppealsToQaDisabled()
        {
            return SiteDriver.FindElementAndGetAttribute(AppealManagerPageObjects.MoveAppealsToQACssLocator, How.CssSelector, "class").Contains("is_disabled");
        }

        public bool IsMoveAppealsToQaPresent() =>
            SiteDriver.IsElementPresent(AppealManagerPageObjects.MoveAppealsToQACssLocator, How.CssSelector);

        public string GetTooltipMoveAppealsToQA()
        {
            return SiteDriver.FindElementAndGetAttribute(AppealManagerPageObjects.MoveAppealsToQACssLocator,
                How.CssSelector, "title");
        }

        public void MoveAppealsToQa()
        {
            SiteDriver.FindElement(AppealManagerPageObjects.MoveAppealsToQACssLocator, How.CssSelector).Click();
        }

       
        //Don't hard delete appeals
        /*public void DeleteAppealsOnClaim(string claseq)
        {
            var temp = string.Format(AppealSqlScriptObjects.DeleteAppealsOnAClaim, claseq);
            Executor.ExecuteQuery(temp);
        }*/

        public List<string> GetUrgentAppealsList(string user)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetAllurgentAppealsForInternalUser,
                user));
        }

        public List<string> GetRecordReviewsList(string user)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetRecordReviewAppealsForInternalUser,
                user));
        }

        public List<string> GetAppealsDueTodayList(string user)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetAppealsDueToday,
                user));
        }
        public bool IsModifyAppealsIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.EnabledEditAppealsConditionCssSelector, How.CssSelector);
        }

        public bool IsModifyAppealsIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealManagerPageObjects.DisabledEditAppealsConditionCssSelector, How.CssSelector);
        }

        public void ResetAppealToDocumentWaiting(string claimSeq)
        {
            var claSeq = claimSeq.Split('-').ToList();
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.ResetAppealToDocumentWaiting, claSeq[0], claSeq[1]));
        }

        public void DeleteExistingDocumentsForAppeal()
        {
            var countOfDoc = DeleteIconCountOfFileList();
            for (int i = 0; i < countOfDoc; i++)
            {
                _fileUpload.ClickOnDeleteUploadedDocumentIcon(1);
                ClickOkCancelOnConfirmationModal(true);
            }
        }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(AppealManagerPageObjects.FindButtonCssLocator, How.CssSelector);
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(AppealManagerPageObjects.FindButtonCssLocator,
                                 AppealManagerPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public void DeleteAppealLockByClaimSeq(string claimSeq)
        {
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.DeleteAppealLockByClaimSeq,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));
        }

        public void UpdateAppealStatusToNew(string claimSeq)
        {
            var claSeq = claimSeq.Split('-').ToList();
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.UpdateAppealStatusByAppealSeq, claSeq[0], claSeq[1]));
        }

        public List<string> GetOutstandingDCIAppealsFromDb(string userId)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetOutstandingDCIAppeals,userId));
        }

        public List<string> GetLineOfBusinessFromDb()
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetLineOfBusiness));
        }

        public void DeleteAppealForQAReviewByAppealSeqFromDb(string appealseq)
        {
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.DeleteAppealForQAReviewByAppealSeq, appealseq));
        }

        public List<string> GetMedicalRecordReviewAppealsFromDatabase()
        {
            return Executor.GetTableSingleColumn(AppealSqlScriptObjects.GetMedicalRecordReviewAppealsFromDb);
        }
        #endregion
    }
}
