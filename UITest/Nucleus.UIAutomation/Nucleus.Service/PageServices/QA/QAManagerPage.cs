using System.IO;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Utils;
using System;
using UIAutomation.Framework.Database;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nucleus.Service.PageObjects.Login;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.SqlScriptObjects.QA;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using static System.String;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.QA
{
    public class QaManagerPage : NewDefaultPage
    {

        #region PRIVATE/PUBLIC FIELDS
        private readonly QaManagerPageObjects _qaManagerPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private CalenderPage _calenderPage;

        private SearchFilter _searchFilter;
        private readonly QuickSearch _quickSearch;
        private readonly StandardGridFunctionalityService _standardGridFunctionality;
        private readonly SubMenu _subMenu;
        #endregion


        #region CONSTRUCTOR
        public QaManagerPage(INewNavigator navigator, QaManagerPageObjects qaManagerPageObject, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, qaManagerPageObject, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _qaManagerPage = (QaManagerPageObjects)PageObject;
            _calenderPage = new CalenderPage(SiteDriver);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _subMenu = new SubMenu(SiteDriver, JavaScriptExecutor);
        }
        #endregion

        public SubMenu GetSubMenu => _subMenu;
        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        #region database

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public List<string> GetActiveQAManagerListFromDB()
        {
            var qaManagerList = Executor.GetTableSingleColumn(QASqlScriptObjects.ActiveQAManagerList);
            return qaManagerList;
        }

        public List<string> GetInactiveQAManagerListFromDB()
        {
            var qaManagerList = Executor.GetTableSingleColumn(QASqlScriptObjects.GetInactiveQAManagerList);
            return qaManagerList;
        }

        public List<string> GetActiveInactiveList()
        {
            var userList =
                Executor.GetTableSingleColumn(QASqlScriptObjects.ActiveInactiveUserList);
            userList.Sort();
            return userList;
        }

        public int GetExpectedTotalFlaggedClaimsCount(string userId, string date)
        {
            var list =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.TotalFlaggedClaimsCount, userId, date));
            return Convert.ToInt32(list[0]);
        }

        public int GetExpectedTotalPassedClaimsCount(string userId, string date)
        {
            var list =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.TotalPassedClaimsCount, userId, date));
            return list == null ? 0 : Convert.ToInt32(list[0]);
        }

        public List<List<string>> GetQaReviewClaimByUser(string userId, string date)
        {
            var list =
                Executor.GetCompleteTable(string.Format(QASqlScriptObjects.QaReviewClaimByUserAndCreatedDate, userId, date));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public bool IsCreateDateEqualsToCorrespondsClaimApproveDate(string claseq, string clientCode, string auditDate)
        {
            var list =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.ClaimAuditByClaimAndAuditDate, clientCode,
                    claseq.Split('-')[0], claseq.Split('-')[1], auditDate));
            return list.Count >= 1;
        }
        public void DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(string claimSequence, string date, string clientCode = "SMTST")
        {
            Executor.ExecuteQuery(string.Format(
                QASqlScriptObjects.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus, claimSequence.Split('-')[0],
                claimSequence.Split('-')[1], date, clientCode));
            Console.WriteLine("Deleted Qa Audit Record Of Claim Approve And Changed Claim Status in database if exists for claimseq<{0}>", claimSequence);

        }

        public void DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatusForQCReview(string claimSequence, string date)
        {
            Executor.ExecuteQuery(string.Format(
                QASqlScriptObjects.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatusForQCReview, claimSequence.Split('-')[0],
                claimSequence.Split('-')[1], date));
            Console.WriteLine("Deleted Qa Audit Record Of Claim Approve And Changed Claim Status in database if exists for claimseq<{0}>", claimSequence);

        }

        public void ResetUserQaCounterToDefault(string userid)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.ResetUserQaCounterToDefault, userid));
            Console.WriteLine("Updated Qa counter for user <{0}>", userid);

        }
        public void DeleteAuditRecordOfClaimApproveAndChangeClaimStatus(string claimSeq, string date)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.DeleteAuditRecordOfClaimApproveAndChangeClaimStatus, claimSeq, date));
            Console.WriteLine("Deleted Qa Audit Record Of Claim Approve And Changed Claim Status in database if exists for claimseq<{0}>", claimSeq);

        }
        public List<string> GetQaTargetResultHistoryList(string userId)
        {
            var userList =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.QaTargetResultsHistoryforUserSequence, userId));
            //userList.Sort();
            return userList;
        }
        public List<string> GetAssignedToList()
        {
            return
                Executor.GetTableSingleColumn(QASqlScriptObjects.AnalystList);
        }

        #endregion

        /// <summary>
        /// logout
        /// </summary>
        //public LoginPage Logout()
        //{
        //    if (SiteDriver.IsElementPresent(QaManagerPageObjects.LogOutBtnId, How.Id))
        //        SiteDriver.FindElement(QaManagerPageObjects.LogOutBtnId, How.Id).Click();
        //    else
        //    {
        //        SiteDriver.FindElement(QaManagerPageObjects.LogOutCssLocator, How.CssSelector)
        //            .Click();
        //    }
        //    return new LoginPage(Navigator, new LoginPageObjects(),SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        //}

            public void ClickOnPTOIcon()
        {
            JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.AddPTOIconCssLocator, How.CssSelector);
            WaitForWorking();
        }

        public void ClickOnModifyPlannedTimeOff()
        {
            JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.ModifyPlannedTimeOffForm,How.CssSelector);
        }
        public bool IsPlannedTimeOffFormDisabled()
        {
           return JavaScriptExecutor.IsElementPresent(QaManagerPageObjects.PlannedTimeOffForm);
        }

        public bool NoentriesSectionPresent()
        {
            return JavaScriptExecutor.IsElementPresent(QaManagerPageObjects.NoentriesMessage);
        }

        public void DeleteAllPTO()
        {
            var totalPTOCount = SiteDriver.FindElementsCount(QaManagerPageObjects.DeleteIconListCssLocator, How.CssSelector);
            for (int i = 1; i <= totalPTOCount; i++)
            {
                JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.DeleteIconByRowCssTemplate,2), How.CssSelector);
            }
        }

        public bool IsDaterangePresent()
        {
            return JavaScriptExecutor.IsElementPresent(QaManagerPageObjects.TimeOffDateRange);
        }

        public int GetTotalAnalystPTO()
        {
            return JavaScriptExecutor.FindElementsCount(QaManagerPageObjects.TimeOffDateRange);
        }

        public List<string> GetAnalystPTOBeginOrStartDate(bool startDate=true)
        {
            return SiteDriver.FindElementsAndGetAttribute("value", Format(QaManagerPageObjects.AnalystOTPStartOrEndDate, startDate?1:2),
                How.CssSelector);
        }

        public bool IsAnalystPTOSortedInAscendingOrder()
        {
            return GetAnalystPTOBeginOrStartDate().Select(DateTime.Parse).ToList().IsInAscendingOrder();
        }
        public void DeleteDateRange(int row)
        {
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.DeleteIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public void SetDateFromOnAddPTO(string date,int row=1,bool isBegin=true)
        {
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.DateInputByRowColCssTemplate, row + 1,isBegin?1:2), How.CssSelector);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine($"<Date> {date} Selected at row:<{row}>");
        }

        public void SetPTODateWithoutUsingCalendar(string date, int row = 1, bool isBegin = true)
        {
            var element = JavaScriptExecutor.FindElement(Format(QaManagerPageObjects.DateInputByRowColCssTemplate, row + 1, isBegin ? 1 : 2));
            element.ClearElementField();
            element.SendKeys(date);
            Console.WriteLine("<Begin Date> Selected:<{0}>", date);
            SiteDriver.WaitToLoadNew(500);
            element.SendKeys(Keys.Tab);
            SiteDriver.WaitToLoadNew(500);
        }

        public bool IsAnalystManagerFormReadOnly() => SiteDriver
            .FindElement(QaManagerPageObjects.AnalystManagerFormCssSelector, How.CssSelector).GetAttribute("class")
            .Contains("read_only");
        public string GetAnalystManagerFormTitle() =>
            SiteDriver.FindElement(QaManagerPageObjects.AnalystManagerFormTitleCssSelector, How.CssSelector).Text;

        public string GetSelectedIconName()
        {
            var iconName = JavaScriptExecutor.FindElement(QaManagerPageObjects.GetNameOfSelectedIconCssTemplate).GetAttribute("Title");
            return iconName;
        }

        public bool IsSectionWithHeaderNamePresent(string header)
        {
            return JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.SectionHeaderPresenceCssTemplate, header));
        }

        public bool IsQaManagerSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.QaManager), How.XPath);
        }
        
        public string GetPageInsideTitle()
        {
            return SiteDriver.FindElement(QaManagerPageObjects.PageInsideTitleCssLocator, How.CssSelector).Text;
        }

        public bool IsFilterPanelPresent()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.FilterPanelCssLocator, How.CssSelector);
        }

        public void SelectAnalayst(string analyst)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analyst);
        }

        public void SelectQaOption(string qaOption)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("QA", qaOption);
        }

        public string GetSelectedQaAppealOption()
        {
            var selectedQaAppealOption = SiteDriver.FindElement(
                "//label[text()='Appeal QA Settings']/../../following-sibling::section//div[contains(@class,'select_component')]//li[contains(@class,'is_active')]"
                , How.XPath).GetAttribute("textContent");
            
            return selectedQaAppealOption;
        }

        public void SearchByAnalyst(string analyst)
        {
            ClickOnSidebarIcon();

            SelectAnalayst(analyst);
            ClickOnFindButton();
        }


        #region Grid

        public List<string> GetGridColumnLabelList()
        {
            var columnLabel = JavaScriptExecutor.FindElements(QaManagerPageObjects.GridColumnLabelCssLocator, "Text");
            columnLabel.Remove("");
            return columnLabel;
        }

        public bool IsSideBarIconPresent()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
        }

        public void SetInputFieldByLabel(string label, string value)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(QaManagerPageObjects.InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            Console.WriteLine("{0} set in {1}", value, label);
        }

        public List<string> GetGridAllList(string gridHeader = "Analyst Manager")
        {
            return JavaScriptExecutor.FindElements(Format(QaManagerPageObjects.GridListCssSelector, gridHeader));
        }

        public int GetGridListCount(string gridHeader = "Analyst Manager")
        {
            return JavaScriptExecutor.FindElementsCount(Format(QaManagerPageObjects.GridListCssSelector, gridHeader));
        }

        public void SetBeginDate(string date, string dateFieldName = "QA Date Range")
        {
            if (dateFieldName.Equals("QA Date Range"))
                JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.BeginDateInEditSectionCssLocator, How.CssSelector);
            else
                JavaScriptExecutor.ExecuteClick(string.Format(QaManagerPageObjects.InputFieldXPathTemplateForQaResultDate, "1"), How.XPath);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine("<Begin Date> Selected:<{0}>", date);
        }

        public void SetBeginDateWithoutUsingCalender(string date)
        {
            var element = JavaScriptExecutor.FindElement(QaManagerPageObjects.BeginDateInEditSectionCssLocator);
            element.ClearElementField();
            element.SendKeys(date);
            Console.WriteLine("<Begin Date> Selected:<{0}>", date);
            SiteDriver.WaitToLoadNew(500);
            element.SendKeys(Keys.Tab);
            SiteDriver.WaitToLoadNew(500);
        }

        public void SetDateWithoutUsingCalender(string dateFieldHeaderName, string date, bool beginDate)
        {
            var elem = JavaScriptExecutor.FindElement(Format(QaManagerPageObjects.BeginOrEndDateByHeaderNameCssLocator
                , dateFieldHeaderName, beginDate ? 1 : 2));
            elem.ClearElementField();
            elem.SendKeys(date);
            Console.WriteLine($"<{(beginDate ? "Begin " : "End ")} Date> Selected:<{date}>");
            SiteDriver.WaitToLoadNew(500);
            elem.SendKeys(Keys.Tab);
            SiteDriver.WaitToLoadNew(500);
        }

        public void SetEndDate(string date, string dateFieldName = "QA Date Range")
        {
            if (dateFieldName.Equals("QA Date Range"))
                JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.EndDateInEditSectionCssLocator, How.CssSelector);
            else
                JavaScriptExecutor.ExecuteClick(string.Format(QaManagerPageObjects.InputFieldXPathTemplateForQaResultDate, "2"), How.XPath);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine("<End Date> Selected:<{0}>", date);
        }

        public void SetStartOrEndDateByDateHeaderName(string date, string dateFieldHeaderName, bool beginDate = true)
        {
            JavaScriptExecutor.ClickJQuery(Format(QaManagerPageObjects.BeginOrEndDateByHeaderNameCssLocator, 
                dateFieldHeaderName, (beginDate) ? 1 : 2));
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine($"<{(beginDate ? "Begin " : "End ")} Date> Selected:<{date}>");
        }

        public void SetEndDateWithoutUsingCalender(string date)
        {
            var element = JavaScriptExecutor.FindElement(QaManagerPageObjects.EndDateInEditSectionCssLocator);
            element.ClearElementField();
            element.SendKeys(date);
            Console.WriteLine("<End Date> Selected:<{0}>", date);
            element.SendKeys(Keys.Tab);
        }

        public void ClearBeginDate()
        {
            SiteDriver.FindElement(QaManagerPageObjects.BeginDateInEditSectionCssLocator, How.CssSelector)
                .ClearElementField();
        }

        public void ClearEndDate()
        {
            SiteDriver.FindElement(QaManagerPageObjects.EndDateInEditSectionCssLocator, How.CssSelector)
                .ClearElementField();
        }

        public void ClearDate(bool beginDate)
        {
            SiteDriver.FindElement(string.Format(QaManagerPageObjects.InputFieldXPathTemplateForQaResultDate, beginDate ? 1 : 2), How.XPath)
                .ClearElementField();
        }

        public string GetBeginDate()
        {
            return
                 JavaScriptExecutor.FindElement(
                    QaManagerPageObjects.BeginDateInEditSectionCssLocator)
                     .GetAttribute("value");
        }

        public string GetEndDate()
        {
            return
                 JavaScriptExecutor.FindElement(
                    QaManagerPageObjects.EndDateInEditSectionCssLocator)
                     .GetAttribute("value");
        }

        public string GetBeginOrEndDate(bool isBeginDate)
        {
            return
                  SiteDriver.FindElement(string.Format(QaManagerPageObjects.InputFieldXPathTemplateForQaResultDate, isBeginDate ? 1 : 2), How.XPath)
                     .GetAttribute("value");
        }

        public string GetBeginOrEndDateByHeaderName(string dateFieldHeaderName, bool isBeginDate)
        {
            return JavaScriptExecutor.FindElement(Format(QaManagerPageObjects.BeginOrEndDateByHeaderNameCssLocator, dateFieldHeaderName, isBeginDate ? 1 : 2)).GetAttribute("value");
        }

        public string GetGridValueByRowCol(int row = 1, int col = 2)
        {
            var element =
                SiteDriver.FindElement(string.Format(QaManagerPageObjects.ValueInGridByRowColumnCssTemplate, row, col),
                    How.CssSelector);
            return element.Text;
        }

        public List<string> GetGridListValueByCol(string gridHeader = "Analyst Manager", int col = 2)
        {
            return JavaScriptExecutor.FindElements(string.Format(QaManagerPageObjects.ListValueInGridByColumnCssTemplate, gridHeader, col),
                "Text");
        }

        public string GetGridLabelByRowCol(int row = 1, int col = 3)
        {
            return SiteDriver.FindElement(
                string.Format(QaManagerPageObjects.LabelInGridByRowColumnCssTemplate, row, col), How.CssSelector).Text;
        }

        public void DeleteClientInEditSection(int row = 1)
        {
            ClickOnSidebarIcon(true);
            GetSideBarPanelSearch.WaitSideBarPanelClosed();

            SiteDriver.FindElement(Format(QaManagerPageObjects.DeleteClientIneditSectionCssLocator, row), How.CssSelector).Click();
        }
        public bool IsDeleteClientInEditSectionPresent(int row = 1)
        {
            return SiteDriver.IsElementPresent(Format(QaManagerPageObjects.DeleteClientIneditSectionCssLocator, row), How.CssSelector);
        }



        public string GetClient()
        {
            return SiteDriver
                .FindElement(QaManagerPageObjects.ClientInEditSectionCssLocator, How.CssSelector).Text;
        }

        public int GetSelectedClientCount()
        {
            return SiteDriver
                .FindElementsCount(QaManagerPageObjects.ClientInEditSectionCssLocator, How.CssSelector);
        }


        public string GetPercentOfClaims()
        {
            return SiteDriver
                .FindElement(QaManagerPageObjects.PercentClaimsInEditSectionCssLocator, How.CssSelector)
                .Text;
        }

        #endregion

        #region SideBarPanel



        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(QaManagerPageObjects.SideBarIconCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public void WaitToOpenCloseWorkList(bool condition = true)
        {
            if (condition)
                SiteDriver.WaitForCondition(() => !IsFindAnalystppealSectionPresent());
            else
                SiteDriver.WaitForCondition(IsFindAnalystppealSectionPresent);
        }

        public void ClickOnFindButton()
        {
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
        }

        public void ClickOnClearButton()
        {
            _sideBarPanelSearch.ClickOnClearLink();
        }

        public bool IsFindAnalystppealSectionPresent()
        {
            return _sideBarPanelSearch.IsSideBarPanelOpen();
        }

        public List<string> GetValuesOfSideBarLabel()
        {
            return _sideBarPanelSearch.GetSearchFiltersList();

        }
        public List<string> GetAvailableDropDownList(string label)
        {
            return _sideBarPanelSearch.GetAvailableDropDownList(label);
        }


        public string GetAnalystInputField()
        {
            return SiteDriver.FindElement(QaManagerPageObjects.AnalystInputFieldCssLocator, How.CssSelector)
                 .GetAttribute("value");
        }
        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.ExecuteClick(string.Format(QaManagerPageObjects.LabelXPathTemplate, "Cancel"),
                How.XPath);
            Console.WriteLine("Cancel Link Clicked");
        }

        public bool IsCancelLinkPresent()
        {
           return  SiteDriver.IsElementPresent(string.Format(QaManagerPageObjects.LabelXPathTemplate, "Cancel"),How.XPath);
        }
        public void SetAppealQaDetails(int col, string appeals)
        {
            var element = SiteDriver.FindElement(
                string.Format(QaManagerPageObjects.AppealQaDetailsInputFieldCssLocator, col),
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(appeals);
        }

        public void SetClaimQaDetails(int n, string claimno)
        {
            var element = SiteDriver.FindElement(
                string.Format(QaManagerPageObjects.ClaimQaDetailsInputFieldCssLocator, n),
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(claimno);
        }

        public string GetToolTipForAppealQaDetailsRadioButtonLabel(string label)
        {
            return
                SiteDriver.FindElement(
                    string.Format(QaManagerPageObjects.QaRadioButtonLabelXPathTemplate, label), How.XPath).GetAttribute("title");
        }

        public void SelectAppealQaOptionFromDropdown(string option)
        {
            JavaScriptExecutor.ClickJQuery(QaManagerPageObjects.AppealQaDropdownCssSelector);
            JavaScriptExecutor.ClickJQuery(Format(QaManagerPageObjects.QaOptionInAppealQASettingsCssSelector, option));
        }

        public bool IsCheckBoxOptionDisplayedInAppealQaSettings(string checkboxOption)
        {
            return SiteDriver.IsElementPresent(Format(QaManagerPageObjects.CheckBoxOptionsInAppealQaSettingsXPath, checkboxOption), How.XPath);
        }


        public void ClickOnQaRadioButton(int n)
        {
            JavaScriptExecutor
                .ExecuteClick(string.Format(QaManagerPageObjects.QaRadioButtonXPathTemplate, n),
                    How.XPath);
            Console.WriteLine("Radio Button Clicked.");
        }
        public bool IsAppealQaSettingsHeaderPresent()
        {
            return SiteDriver.IsElementPresent(
                string.Format(QaManagerPageObjects.FormHeaderLabelXPathTemplate, "Appeal QA Settings"),
                How.XPath);
        }

        public bool IsAppealQaSettingsFormSectionPresent() => 
            SiteDriver.IsElementPresent(QaManagerPageObjects.AppealQASettingsFormSectionCssSelector, How.CssSelector);

        public bool IsInactiveRowPresent()
        {
            return SiteDriver.FindElementsCount(QaManagerPageObjects.InactiveGridCssLocator, How.CssSelector).Equals(3);
        }

        public bool IsRadioButtonSelected(int n)
        {
            return SiteDriver
                .FindElement(string.Format(QaManagerPageObjects.QaRadioButtonXPathTemplate, n), How.XPath)
                .GetAttribute("class").Contains("is_active");
        }

        public string GetTextValueInAppealsCompleted()
        {
            return SiteDriver.FindElement(string.Format(QaManagerPageObjects.AppealsCompletedXPathTemplate), How.XPath).Text;
        }

        public void SetClaimsCount(string claimsCount, int row = 1)
        {
            var element = SiteDriver.FindElement(Format(QaManagerPageObjects.ClaimsCountInEditSectionCssLocator, row), How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);  
            element.SendKeys(claimsCount);
        }

        public string GetMaxClaimsCount(int row = 1)
        {
            return SiteDriver.FindElement(Format(QaManagerPageObjects.MaxClaimsCountInEditSectionCssLocator, row),
                How.CssSelector).GetAttribute("value");

        }

        public string GetClaimsCount(int row = 1)
        {
            return SiteDriver.FindElement(Format(QaManagerPageObjects.ClaimsCountInEditSectionCssLocator, row),
                How.CssSelector).GetAttribute("value");

        }

        public void SetClient(string client)
        {
            CaptureScreenShot("QaManagerPage");
            SiteDriver.FindElement(QaManagerPageObjects.ClientDropDownCssSelector, How.CssSelector).Click();
            WaitForStaticTime(2000);
            SiteDriver.WaitForCondition(() => JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.ClientDropDownValueCssLocator, client)));
            JavaScriptExecutor.ClickJQuery(Format(QaManagerPageObjects.ClientDropDownValueCssLocator, client));
        }
        public void SetMaxClaimsCount(string maxClaimsCount, int row = 1)
        {
            var element =
                SiteDriver.FindElement(
                    Format(QaManagerPageObjects.MaxClaimsCountInEditSectionCssLocator, row), How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(maxClaimsCount);

        }

        public void ClearClaimsCount(int row = 1)
        {
            SiteDriver.FindElement(Format(QaManagerPageObjects.ClaimsCountInEditSectionCssLocator, row), How.CssSelector)
                .ClearElementField();
        }

        public void ClearMaxClaimsCount(int row = 1)
        {
            SiteDriver.FindElement(Format(QaManagerPageObjects.MaxClaimsCountInEditSectionCssLocator, row), How.CssSelector)
                .ClearElementField();
        }

        public void ClickOnGridRowToOpenSideWindow(int row = 1)
        {
            GetGridViewSection.ClickOnGridRowByRow(row);
            Console.WriteLine("Click on Grid row <{0}> ", row);
        }

        public void ClickOnSaveButton()
        {
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.ButtonXPathTemplate, "Save"), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnAddNewEntryButton()
        {
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.ButtonXPathTemplate,"Add New Entry"), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public bool IsSaveButtonPresent()
        {
            return SiteDriver.IsElementPresent(Format(QaManagerPageObjects.ButtonXPathTemplate, "Add New Entry"),How.XPath);
        }

        public bool IsAddNewEntryButtonPresent()
        {
          return  SiteDriver.IsElementPresent(Format(QaManagerPageObjects.ButtonXPathTemplate, "Add New Entry"),How.XPath);
        }

        public void UpdateQAAppealDetails(string appealCompleted,string maxAppeal)
        {
            ClickOnGridRowToOpenSideWindow();
            SetAppealQaDetails(1, appealCompleted);
            SetAppealQaDetails(2, maxAppeal);
            ClickOnSaveButton();
        }

        public bool IsEditIconDisplayed(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(QaManagerPageObjects.EditIconByRowCssLocator, row), How.CssSelector);
        }

        public bool IsEditIconDisplayedInEachRow()
        {
            return SiteDriver.FindElementsCount(QaManagerPageObjects.EditIconListCssSelector, How.CssSelector)
                .Equals(SiteDriver.FindElementsCount(QaManagerPageObjects.GridListCssSelector, How.CssSelector));
        }

        public bool IsEditIconDisabled() => SiteDriver
            .FindElement(QaManagerPageObjects.EditIconCssLocator, How.CssSelector).GetAttribute("class")
            .Contains("is_disabled ");

        public bool IsEditIconPresent() => SiteDriver.IsElementPresent(QaManagerPageObjects.EditIconCssLocator, How.CssSelector);
        #endregion


        public bool IsEditFormDisplayed()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.EditFormCssLocator, How.CssSelector);
        }

        public bool AreQaDateFieldsPresent()
        {
            if (SiteDriver.IsElementPresent(
                string.Format(QaManagerPageObjects.InputFieldXPathTemplate, "Date Range"), How.XPath))
                return
                    SiteDriver.FindElementsCount(
                        string.Format(QaManagerPageObjects.InputFieldXPathTemplate, "Date Range"), How.XPath)
                        .Equals(2);
            return false;
        }

        public void AddMultipleAnalystPTO(int number=1)
        {
            
                GetSideWindow.ClickOnEditIcon();
                
            for (int i = 1; i <=number; i++)
            {
                 ClickOnAddNewEntryButton(); 
                SetPTODateWithoutUsingCalendar(DateTime.Now.AddDays(i).ToString("MM/dd/yyyy"), i);
                SetPTODateWithoutUsingCalendar(DateTime.Now.AddDays(i).ToString("MM/dd/yyyy"), i, isBegin: false);
                
            }
            ClickOnSaveButton();
        }

        public string GetInputValueByLabel(string label)
        {
            return
                 JavaScriptExecutor.FindElement(
                     string.Format(QaManagerPageObjects.InputFieldByLabelCssTemplate, label))
                     .GetAttribute("value");
        }

        public string GetClaimPercentageValue()
        {
            return SiteDriver.FindElement(QaManagerPageObjects.ClaimPercentageCssLocator, How.CssSelector).Text;
        }

        public void ClickOnSearchListRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(QaManagerPageObjects.SearchListRowSelectorTemplate, row), How.CssSelector);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnSearchListRow(string text)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(QaManagerPageObjects.SearchListByTextInRowSelectorTemplate, text), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public void ActivateAnalystForGivenDateRangeAndTarget(string beginDate, string endDate, string claimCount, string maxClaim, string analyst,bool qc = false)
        {

            SearchByAnalyst(analyst);
            GetGridViewSection.ClickOnGridRowByRow();
            //ClickOnGridRowToOpenSideWindow();
            SetEndDateWithoutUsingCalender(endDate);
            SetBeginDateWithoutUsingCalender(beginDate);
            if (IsPageErrorPopupModalPresent()) ClosePageError();
            if (GetEndDate() != endDate)
                SetEndDateWithoutUsingCalender(endDate);
            SetClaimsCount(claimCount);
            SetMaxClaimsCount(maxClaim);
            if (qc)
                SelectRadioButtonOptionByLabel("QC");
            else
                SelectRadioButtonOptionByLabel("QA");
            ClickOnSaveButton();
        }

        public void SelectQAQCOption(bool qc = false)
        {
            var label = (qc) ? "QC" : "QA";
            SelectRadioButtonOptionByLabel(label);
        }

        #region Appeal Details section
        public bool IsQaTargetHistoryDetailSectionOpen()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.QaTargetHistoryDetailsHeaderXpath, How.XPath);
        }

        public bool IsQaResultsSectionOpen()
        {
            return JavaScriptExecutor.IsElementPresent(QaManagerPageObjects.QaResultsFieldCssLocator);
        }

        public List<string> GetQaResultsDetailsListValueByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                    string.Format(QaManagerPageObjects.QaResultsDetailsListValueByLabelJQueryTemplate, label), "Text")
                ;
        }

        public string GetQaTargetHistoryDetailsContentValueByLabelAndRow(string label, int row = 1)
        {
            Console.WriteLine("Get value of Qa Target Results History from details section for row {1} and {0} ", row, label);
            //return JavaScriptExecutor.FindElement(string.Format(QaManagerPageObjects.QaTargetHistoryDetailsValueByLabelCssTemplate, row, label)).Text;
            return JavaScriptExecutor.FindElement(string.Format(QaManagerPageObjects.QaResultsDetailsValueByLabelCssTemplate, row, label)).Text;
        }


        public string GetQaTargetHistoryDetailsLabelValueByRowAndLabel(int row, int col)
        {
            Console.WriteLine("Get value of Qa Target Results History from details section row:{0} and col: {1}", row, col);
            return SiteDriver.FindElement(string.Format(QaManagerPageObjects.QaTargetHistoryDetailsValueByLabelCssTemplate, row, col), How.CssSelector).Text;
        }

        public string GetQaTargetHistoryDetailsContentValueByRowAndLabel(int row, int col)
        {
            Console.WriteLine("Get value of label of Qa Target Results History from details section row:{0} and col: {1}", row, col);
            return SiteDriver.FindElement(string.Format(QaManagerPageObjects.QaTargetHistoryDetailsLabelByRowColCssTemplate, row, col), How.CssSelector).Text;
        }

        public bool IsNoDataMessagePresent()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.NoDataMessageLeftComponentCssSelector, How.CssSelector);
        }
        public string GetNoDataMessageText()
        {
            return SiteDriver.FindElement(QaManagerPageObjects.NoDataMessageLeftComponentCssSelector, How.CssSelector).Text;
        }
        public void SetDateForQaFilterFieldValue(string date, int row)
        {
            SiteDriver.FindElement(string.Format(QaManagerPageObjects.QaTargetHistoryFilterFieldValueCssLocator, row), How.CssSelector)
                .SendKeys(date);
        }
        public string GetDateForQaFilterFieldValue(int row)
        {
            return SiteDriver.FindElement(
                string.Format(QaManagerPageObjects.QaTargetHistoryFilterFieldValueCssLocator, row), How.CssSelector).GetAttribute("value");
        }
        public int GetQaResultsHistoryCount()
        {
            /*return SiteDriver.FindElementsCount(QaManagerPageObjects.QaResultsHistoryCountCssSelector,
                How.CssSelector);*/

            return JavaScriptExecutor.FindElementsCount(QaManagerPageObjects.QaResultsHistoryCountCssSelector);
        }

        public void ClickOnFindQaResultButton()
        {
            JavaScriptExecutor.ClickJQuery(QaManagerPageObjects.FindQaResultButtonCssSelector);
            Console.WriteLine("Find Qa result Button Clicked");
        }

        public void ClickOnCancelQaResultLink()
        {
            //SiteDriver.FindElement(QaManagerPageObjects.CancelQaResultButtonCssSelector , How.CssSelector).Click();
            JavaScriptExecutor.FindElement(QaManagerPageObjects.CancelQaResultButtonCssSelector).Click();
            //JavaScriptExecutor.ClickJQueryByText(QaManagerPageObjects.ClearQaResultButton, "Cancel");
            Console.WriteLine("Cancel Qa result Link in 'QA Results' Clicked");
        }
        public List<String> GetQaResultListbyCol(int col)
        {
            //return SiteDriver.FindElements(string.Format(QaManagerPageObjects.QaTargetHistoryDetailsResultListCssTemplate,col), How.CssSelector, "Text");

            return JavaScriptExecutor.FindElements(Format(QaManagerPageObjects.QaResultsDetailsListCssTemplate, col));

        }
        public bool IsCreatedDateSortedInDescendingOrder()
        {
            return GetQaResultsDetailsListValueByLabel("Date:").Select(DateTime.Parse).ToList()
                .IsInDescendingOrder();
        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetQaResultListbyCol(col).IsInDescendingOrder();
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            return GetQaResultListbyCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder();
        }
        public int GetDateRangeInputFieldsCount(string header)
        {
            return SiteDriver.FindElementsCount(Format(QaManagerPageObjects.InputFieldForQaResultDateXPath, header), How.XPath);
        }

        public void ScrollToFindQAResult()
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDiv(Format(QaManagerPageObjects.FindQaResultButtonCssSelector));
        }

        #endregion

        #region Analyst Manager

        public bool IsAnalystManagerFormPresentByFormName(string formName) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.AnalystManagerFormLocatorCssTemplate,
                formName));

        public void ClickAnalystManagerIconByColNum(int col)
        {
            var element = SiteDriver
                .FindElement(Format(QaManagerPageObjects.AnalystManagerIconCssSelectorTemplate, col), How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
        } 

        public string GetTitleOfAnalystManagerIconByColNum(int col) => SiteDriver.FindElementAndGetAttribute(
            Format(QaManagerPageObjects.AnalystManagerIconCssSelectorTemplate, col), How.CssSelector, "title");

        public bool IsAnalystManagerIconPresentByColNum(int col) => SiteDriver
            .IsElementPresent(Format(QaManagerPageObjects.AnalystManagerIconCssSelectorTemplate, col), How.CssSelector);

        public bool IsAnalystManagerFormIconPresentByTitle(string title) =>
            SiteDriver.IsElementPresent(Format(QaManagerPageObjects.AnalystManagerFormIconsCssSelectorTemplate, title),How.CssSelector);

        public bool IsAnalystManagerTitlePresentByTitleName(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.AnalystManagerLabelCssSelectorTemplate,
                label));

        public bool IsAppealFormPresent() =>
            SiteDriver.IsElementPresent(QaManagerPageObjects.AppealFormCssLocator, How.CssSelector);

        public bool IsDownCaretIconPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.DownCaretIconCssSelectorTemplate, label));

        public bool IsRightCaretIconPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.RightCaretIconCssSelectorTemplate, label));
        public void ClickDownCaretIconByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(QaManagerPageObjects.DownCaretIconCssSelectorTemplate, label)).Click();

        public void ClickRightCaretIconByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(QaManagerPageObjects.RightCaretIconCssSelectorTemplate, label)).Click();

        public bool IsDropdownPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.DropdownCssSelectorTemplate, label));
        public string GetClaimsAssignmentsNoRecordsByLabel(int row) =>
            SiteDriver.FindElementAndGetAttribute(Format(QaManagerPageObjects.ClaimsAssignmentsNoRecordsCssSelectorTemplate, row), How.CssSelector, "innerHTML");

        public List<List<string>> GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalystsFromDb(string label, string analyst)
        {
            var list =
                Executor.GetCompleteTable(string.Format(QASqlScriptObjects.GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalysts, label, analyst));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public string GetAnalystPTO(string userid)
        {
            return Executor.GetSingleStringValue(Format(QASqlScriptObjects.AnalystPTOByUserId, userid));
        }

        public void InsertAnalystPTO(string userseq)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.InsertAnalystPTOUser,userseq));
            
        }

        public void DeleteAnalystPTO(string userid)
        {
             Executor.ExecuteQuery(Format(QASqlScriptObjects.DeleteAnalystPto, userid));
        }
        public List<string> GetClaimsAssignmentsListByLabelAndRow(string label, int row) =>
            JavaScriptExecutor.FindElements(Format(QaManagerPageObjects.ClaimsAssignmentsValueListCssTemplate, label, row),
                "innerHTML");

        public int GetCountOfClaimsAssignmentsRecordRowsByLabel(string label) =>
            JavaScriptExecutor.FindElementsCount(Format(QaManagerPageObjects.ClaimsAssignmentsRecordsRowCssTemplate,
                label));

        public List<string> GetClaimsAssignmentsRecordLabelByLabelLineNoRowCol(string label, int lineNo, int row) =>
            JavaScriptExecutor.FindElements(
                Format(QaManagerPageObjects.ClaimsAssignmentRecordLabelSelectorTemplate, label, lineNo, row),
                "innerHTML");

        public List<string> GetListOfClaimsAssignmentParticularValueByTitleRowAndCol(string title, int row, int col) =>
            JavaScriptExecutor.FindElements(
                Format(QaManagerPageObjects.ClaimsAssignmentParticularRecordValueListSelectorTemplate, title, row, col),
                "innerHTML");

        public bool IsDeleteIconPresentByTitleAndLineNumber(string title, int lineNo) =>
            JavaScriptExecutor.IsElementPresent(Format(QaManagerPageObjects.DeleteIconSelectorTemplate, title, lineNo));

        public List<string> GetValuesInCategoryIdDropdown()
        {
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.DropDownToggleIconCssTemplate), How.CssSelector);
            return JavaScriptExecutor.FindElements(Format(QaManagerPageObjects.DropDownToggleValueCssTemplate), How.CssSelector, "Text");

        }

        public List<string> GetCategoryIdsFromDatabase()
        {
            return Executor.GetTableSingleColumn(QASqlScriptObjects.AppealCategoryCodes);
        }

        public List<List<string>> GetCategoryDetailsByCategoryCode(string code)
        {
            var data = Executor.GetCompleteTable(String.Format(QASqlScriptObjects.AppealCategoryDetails, code));
            return data.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public bool IsCategoryDetailsSectionPresent()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.CategoryCodeDetailsCssSelector, How.CssSelector);
        }

        public bool IsCategoryDetailsSectionDisabled()
        {
            return JavaScriptExecutor.FindElement(QaManagerPageObjects.CategoryCodeDetailsCssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public string GetCategoryDetailsLabel(int row = 1, int column = 1,int lineNo = 1)
        {
            return JavaScriptExecutor.FindElement(String.Format(QaManagerPageObjects.CategoryCodeDetailsLabelByRowColumnCssTemplate,row,column,lineNo)).Text;
        }

        public string GetCategoryDetailsValueByRowColumn(int row = 1, int column = 1,int lineNo = 1)
        {
            return JavaScriptExecutor.FindElement(String.Format(QaManagerPageObjects.CategoryCodeDetailsValueByRowColumnCssTemplate, row, column,lineNo)).Text;
        }

        public void ClickOnCategoryDetailsByLineNo(int lineNo)
        {
            //JavaScriptExecutor.ExecuteClick(String.Format(QaManagerPageObjects.CategoryCodeDetailsByLineNoCssTemplate, lineNo),How.CssSelector);
            SiteDriver.FindElement(String.Format(QaManagerPageObjects.CategoryCodeDetailsByLineNoCssTemplate, lineNo), How.CssSelector).Click();
        }

        public string GetMessageForUsersWithClaimAccessRestrictionAssigned()
        {
            return JavaScriptExecutor.FindElement(QaManagerPageObjects.MessageForUsersWithRestrictionAssigned)
                .Text;
        }

        public List<string> GetAssignedRestrictionForUser(string userId)
        {
            return Executor.GetTableSingleColumn(String.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
        }

        public List<string> GetRestrictionOptions()
        {
            return JavaScriptExecutor.FindElements(Format(QaManagerPageObjects.RestrictionOptionsByCssSelector), How.CssSelector, "Text");
        }

        public bool IsRestrictionOptionsPresent()
        {
            return SiteDriver.IsElementPresent(QaManagerPageObjects.RestrictionOptionsByCssSelector, How.CssSelector);
        }

        public void SelectRadioButtonOptionByLabel(string option)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(QaManagerPageObjects.RadioButtonByLabelXPathTemplate, option),
                How.XPath);
        }

        public bool IsCategoryPresentInRespectiveClaimsAssignment(string category, string assignment)
        {
            int i = (assignment == "Current Non-Restricted Claims Assignments") ? 1 : 2;
            return SiteDriver.IsElementPresent(String.Format(QaManagerPageObjects.CategoryAssignmentByCategoryXPathTemplate, assignment, category,i), How.XPath);
        }

        public void ClickOnDeleteIconByOptionandLineNo(string assignmentCategory, int lineNo)
        {
            int i = (assignmentCategory == "Current Non-Restricted Claims Assignments") ? 1 : 2;
            JavaScriptExecutor.ExecuteClick(Format(QaManagerPageObjects.DeleteIconByAssignementAndLineNo,lineNo,i), How.CssSelector);
        }

        public AppealCategoryManagerPage OpenAppealCategoryManagerInNewTab()
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();
            var appealCategoryManagerPage = Navigator.Navigate<AppealCategoryManagerPageObjects>(() =>
            {

                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(SiteDriver.BaseUrl + PageUrlEnum.AppealCategoryManager.GetStringValue());
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new AppealCategoryManagerPage(Navigator,appealCategoryManagerPage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public List<string> GetAssignedAnalystForACategoryFromDb(string restrictionOption,string category)
        {
            var column = (restrictionOption == "Restricted") ? "unrestricted_assigned_analysts" : "assigned_analysts";
            return Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedAnalystForACategory,
                column, category));
        }

        public List<string> GetAssignedRestrictionDescriptionForUser(string userId)
        {
            return Executor.GetTableSingleColumn(String.Format(QASqlScriptObjects.AssignedRestrictionDescriptionForAUser, userId));
        }

        public void DeleteAppealCategoryAuditFromDatabase(string category1, string category2, string category3, string userId)
        {
            Executor.ExecuteQuery(string.Format(QASqlScriptObjects.DeleteAppealCategoryAuditHistory,category1,category2,category3,userId));
        }

        public void UpdateRoleForUser(string userId, string oldRole, string newRole)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.UpdateRoleForUser, userId, oldRole, newRole));
            WaitForStaticTime(3000);
        }

        public void AddRoleForAuthorities(string userId, string role)
        {
            
                Executor.ExecuteQuery(Format(QASqlScriptObjects.InsertRoleForUser, userId, role
                    ));
                WaitForStaticTime(3000);
            
        }

        public void RemoveRoleForAuthorities(string userId, string role)
        {
            Executor.ExecuteQuery(Format(QASqlScriptObjects.DeleteRoleForUser, userId, role));
            WaitForStaticTime(3000);
        }

        public void RefreshPage()
        {
            SiteDriver.Refresh();
            WaitForSpinner();
            WaitForPageToLoad();
        }

        public bool IsQaQcRadioButtonsPresentByLabel(string label) =>
            SiteDriver.IsElementPresent(Format(QaManagerPageObjects.ClaimQaDetailQaQcRadioButtonsXPathTemplate,label), How.XPath);

        public bool IsQaQcRadioButtonSelectedByLabel(string label) => SiteDriver
            .FindElement(Format(QaManagerPageObjects.ClaimQaDetailQaQcRadioButtonsXPathTemplate, label), How.XPath)
            .GetAttribute("class").Contains("is_active");

        public string GetToolTipInfoOfQaQcButtonsByLabel(string label) =>
            SiteDriver.FindElementAndGetAttribute(
                Format(QaManagerPageObjects.ClaimsQaDetailQaQcLabelXPathTemplate, label), How.XPath, "title");

        public bool IsQcBusinessDaysOnlyOptionPresent() => 
            SiteDriver.IsElementPresent(QaManagerPageObjects.QCBusinessDaysOnlyXpath, How.XPath);

        public string GetQcBusinessDaysOnlyTooltip() =>
            SiteDriver.FindElement(QaManagerPageObjects.QCBusinessDaysOnlyTooltipXpath, How.XPath).GetAttribute("title");

        public bool IsQaBusinessDaysOnlyOptionSelectedByDefault() => SiteDriver
            .FindElement(QaManagerPageObjects.QCBusinessDaysOnlyTooltipXpath, How.XPath)
            .GetAttribute("class").Contains("active");

        public List<string> GetDayAndHourFromApprovedDate(string claSeq)
        {
            var infoList = new List<List<string>>();
            var batchInfo = Executor
                .GetCompleteTable(string.Format(QASqlScriptObjects.GetDayHourApproveDate,
                    claSeq.Split('-')[0],claSeq.Split('-')[1]));
            foreach (DataRow row in batchInfo)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList[0];
        }

        public bool IsClaimSeqAlreadySelectedForDailyQc(string claseq)
        {
            return Convert.ToInt32(
                Executor.GetTableSingleColumn(Format(QASqlScriptObjects.CheckIfClaimIsSelectedForDailyQcByCount,
                        claseq.Split('-')[0], claseq.Split('-')[1]))
                    [0]) > 0;
        }

        public bool IsApprovedDateHoliday(string claseq)
        {
            return Convert.ToInt32(
                Executor.GetTableSingleColumn(Format(QASqlScriptObjects.IsApprovedDateHoliday,
                        claseq.Split('-')[0], claseq.Split('-')[1]))
                    [0]) > 0;
        }


        #endregion

    }
}
