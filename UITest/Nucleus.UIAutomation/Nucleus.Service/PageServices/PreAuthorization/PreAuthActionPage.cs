using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.String;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.SqlScriptObjects.Pre_Auth;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;

namespace Nucleus.Service.PageServices.PreAuthorization
{
    public class PreAuthActionPage : NewDefaultPage
    {

        #region PRIVATE FIELDS

        private PreAuthActionPageObjects _preAuthorizationActionPage;
        private readonly SideWindow _sideWindow;
        private GridViewSection _gridViewSection;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly string _originalWindow;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly FileUploadPage FileUploadPage;

        #endregion

        #region CONSTRUCTOR

        public PreAuthActionPage(INewNavigator navigator, PreAuthActionPageObjects preAuthorizationActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor, bool handlePopup = false)
            : base(navigator, preAuthorizationActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _preAuthorizationActionPage = (PreAuthActionPageObjects) PageObject;
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _commonSqlObjects = new CommonSQLObjects(Executor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            FileUploadPage = new FileUploadPage(SiteDriver,JavaScriptExecutor);
            WaitForStaticTime(500);
            WaitForWorkingAjaxMessage();
            if (handlePopup)
                HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        #endregion


        #region PUBLIC METHODS

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public FileUploadPage GetFileUploadPage
        {
            get { return FileUploadPage; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public List<string> GetUpperLeftQuadrantValuesFromDB(string preAuthSeq)
        {
            var resultList =
                Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.PreAuthActionUpperLeftQuadrantValues,
                    preAuthSeq));
            var upperQuadrantValueList = new List<string>();

            foreach (DataRow row in resultList)
            {
                upperQuadrantValueList = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return upperQuadrantValueList;
        }

        public string GetUpperLeftQuadrantValueByLabel(string label)
        {
            return JavaScriptExecutor
                .FindElement(string.Format(PreAuthActionPageObjects.UpperLeftQuadrantValueByLabel, label))
                .GetAttribute("title");
        }

        //public string GetLineItemsValueByLabel(string label)
        //{
        //    return
        //        SiteDriver.FindElement(
        //                string.Format(PreAuthActionPageObjects.ValueByLabelXpath, label), How.XPath)
        //            .Text;
        //}

        public List<string> GetLineItemsValueByLabel(string label)
        {
            return
                JavaScriptExecutor.FindElements(
                    string.Format(PreAuthActionPageObjects.ValueByLabelXpath, label), How.XPath, "Text");
        }

        public List<string> GetProcCodeNDateNFlag(string label)
        {
            if (label == "ProcCode")
            {
                return
                    JavaScriptExecutor.FindElements(
                        string.Format(PreAuthActionPageObjects.ProcCodenDatenFlagCssLocator, "column_10"),
                        How.CssSelector, "Text");
            }
            else if (label == "Date")
            {
                return
                    JavaScriptExecutor.FindElements(
                        string.Format(PreAuthActionPageObjects.ProcCodenDatenFlagCssLocator, "column_15"),
                        How.CssSelector, "Text");
            }
            else if (label == "Flag")
            {
                return JavaScriptExecutor.FindElements(
                    string.Format(PreAuthActionPageObjects.ProcCodenDatenFlagCssLocator, "column_9"), How.CssSelector,
                    "Text");
            }

            return null;
        }
        public string GetLineItemValueByDivRowColumn(int div = 1, int row = 1, int col = 1)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.LineItemValueByDivRowColumnXPath, div, row, col), How.XPath
                ).Text;
        }

        //public List<string> GetLineItemsValueListByRowColumn(int row = 1, int col = 2)
        //{
        //    return SiteDriver.FindElements(
        //        string.Format(PreAuthActionPageObjects.LineItemValueListByRowColumnXPath, row, col), How.XPath,
        //        "Text");
        //}

        public List<string> GetLineItemsValueListByRowColumn(int row = 1, int col = 2)
        {
            ScrollToLastDiv("LineItems");
            return JavaScriptExecutor.FindElements(string.Format(PreAuthActionPageObjects.LineItemValueListByRowColumnXPath, row, col), How.XPath, "Text");
        }

        public List<string> GetQuadrantHeaderTitleList()
        {
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.QuadrantHeaderTitleCssLocator, How.CssSelector,
                "Text");
        }

        public bool IsReturnToSearchIconEnabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.ReturnToPreAuthSearchCssLocator, How.CssSelector)
                .GetAttribute("class").Contains("is_active");
        }

        public bool IsApproveIconEnabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.ApproveIconStateCssLocator, How.CssSelector)
                .GetAttribute("class").Contains("is_active");
        }

        public bool IsApproveIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.ApproveIconCssLocator, How.CssSelector);
        }

        public bool IsAddFlagIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.AddFlagIconCssLocator, How.CssSelector);
        }

        public bool IsAddIconEnabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.AddIconStateCssLocator, How.CssSelector)
                .GetAttribute("class").Contains("is_active");

        }

        public void ClickOnAddFlagIcon()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.AddFlagIconCssLocator, How.CssSelector);
            Console.WriteLine("Click on Add Flag Icon");
        }

        public string GetMessageBelowSelectedLines()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.MessageBelowSelectedLinesXPath, How.XPath)
                .Text;
        }



        public bool IsAllLinesSelected(bool isSelected = true)
        {
            var list = SiteDriver.FindElementsCount(PreAuthActionPageObjects.LinesListOnSelectLinesXPath, How.XPath);
            var activeList =
                SiteDriver.FindElementAndGetActiveAttribute(PreAuthActionPageObjects.LinesListOnSelectLinesXPath,
                    How.XPath);
            return (list == activeList.Count && activeList.Distinct().Count() == 1 &&
                    activeList.Distinct().First() == isSelected);
        }

        public string GetValuesOnLinesOnSelectLines(int col = 1, int row = 1)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.ValuesListOnLinesOnSelectLinesXPathTemplate + "{2}", col,
                    "[" + row + "]", "/span"), How.XPath).Text;
        }

        public List<string> GetAllLinesListOnSelectedLines()
        {
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.LinesListOnSelectedLinesXPath, How.XPath, "Text");
        }

        public List<string> GetValuesListOnLinesOnSelectLines(int col = 1, bool isActiveList = false)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.ValuesListOnLinesOnSelectLinesXPathTemplate, col,
                    isActiveList ? "[contains(@class,'is_active')]" : ""), How.XPath, "Text");
        }

        public List<string> GetValuesListOnLinesOnSelectedLines(int col = 1)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.ValuesListOnLinesOnSelectedLinesXPathTemplate, col), How.XPath,
                "Text");
        }

        public void ClickOnLinesOnSelectLines(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthActionPageObjects.LinesListOnSelectLinesXPath + "[{0}]", row), How.XPath);
            WaitForStaticTime(200);
        }


        public bool IsAddFlagSectionPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.AddFlagSectionCssLocator, How.CssSelector);
        }

        public bool IsHistoryIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.HistoryIconCssLocator, How.CssSelector);
        }

        public string GetHistoryIconToolTip()
        {
            return SiteDriver.FindElementAndGetAttribute(PreAuthActionPageObjects.HistoryIconCssLocator,
                How.CssSelector, "title");
        }

        public void ClickOnHistoryIconToNavigateToPatientClaimHistoryPage(bool notable = false)
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.HistoryIconCssLocator).Click();
            SwitchToPatientClaimHistory(notable);
        }

        public bool IsNextIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NextIconCssLocator, How.CssSelector);
        }
        public bool IsNextIconDisabled()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NextIconDisabledCssLocator, How.CssSelector);
        }
        public void ClickOnNextIcon()
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.NextIconCssLocator).Click();
            WaitForWorking();

        }

        public bool ClickOnNextIconAndCheckIfNextIconIsDisabled(bool checkDoubleClick = true)
        {
            var isDisabled = JavaScriptExecutor.ClickAndGetWithJquery(PreAuthActionPageObjects.NextIconCssLocator,
                                 PreAuthActionPageObjects.NextIconDisabledCssLocator) != null;

            return isDisabled;
        }

        public bool IsSearchIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.ReturnToPreAuthSearchCssLocator,
                How.CssSelector);
        }

        public bool IsMoreOptionsIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.MoreOptionsIconCssSelector, How.CssSelector);
        }

        public bool IsFlagRowEditIconPresent(int flagRow = 1, int flagLineRow = 1)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlagRowEditIconXPathTemplate, flagRow, flagLineRow),
                How.XPath);
        }

        public bool IsFlagRowLogicIconPresent(int flagLineDiv = 1, int row = 1)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlagRowLogicIconXPathTemplate, flagLineDiv, row),
                How.XPath);
        }

        public bool IsFlagRowAddLogicIconPresent(int flagLineDiv = 1, int row = 1)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlagRowAddLogicIconXPathTemplate, flagLineDiv, row),
                How.XPath);
        }

        public bool IsFlaggedLineRowDivPresentByLineNo(int lineNo)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlaggedLineRowDivByLineNoXPathTemplate, lineNo),
                How.XPath);
        }

        public bool IsFlagRowDeletedPresent(int flagLineDiv = 1, int row = 1)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.FlagRowXPathTemplate, flagLineDiv, row),
                How.XPath).GetAttribute("class").Contains("is_deleted");
        }

        public string GetSearchIconToolTip()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.ReturnToPreAuthSearchCssLocator,
                    How.CssSelector)
                .GetAttribute("title");
        }

        public string GetMoreOptionIconToolTip()
        {
            return SiteDriver
                .FindElement(PreAuthActionPageObjects.MoreOptionsIconCssSelector, How.CssSelector)
                .GetAttribute("title");
        }

        //public List<string> GetFlagLineNumberList()
        //{
        //    return SiteDriver.FindElements(PreAuthActionPageObjects.FlagLineNumberXPath, How.XPath,
        //        "Text");
        //}

        public List<string> GetFlagLineNumberList()
        {
            ScrollToLastDiv("FlaggedLines");
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.FlagLineNumberXPath, How.XPath, "Text");
        }

        public List<string> GetFlagListByDivList(int div = 1, bool isInternalUser = true)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.FlagListByDivXPathTemplate, div, isInternalUser ? 3 : 2),
                How.XPath,
                "Text");
        }

        public List<string> GetBoldOrNormalColorFlagList(bool isBoldColor = true)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.FlagBoldOrNormalColorXPathTemplate,
                    isBoldColor ? "line_flag_must_be_worked " : "line_flag_can_be_worked "), How.XPath, "Text");
        }

        public string GetFlagLineLevelHeaderDetailValue(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthActionPageObjects.FlagLineLevelHeaderDetailValueXPathTemplate, flagLineDiv,
                        row, col),
                    How.XPath).Text;
        }

        public string GetFlagDetailValueBylabelandFlag(string flag,string label)
        {
            return JavaScriptExecutor.FindElement(string
                .Format(PreAuthActionPageObjects.FlagDetailValueByLabelAndFlag, flag, label)).Text;

        }

        public bool IsStrikeThroughPresentInFlagDetail(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            var cssValue = SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.FlagRowDetailValueXPathTemplate, flagLineDiv, row, col),
                How.XPath).GetCssValue("text-decoration");

            return cssValue.Contains("line-through");
        }

        public string GetFlagTextColor(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            var cssValue = SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.FlagRowDetailValueXPathTemplate, flagLineDiv, row, col),
                How.XPath).GetCssValue("color");

            return cssValue;
        }

        public string GetFlagLineLevelHeaderDetailLabel(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthActionPageObjects.FlagLineLevelHeaderDetailLabelXPathTemplate, flagLineDiv,
                        row, col),
                    How.XPath).Text;
        }

        public string GetFlagRowDetailValue(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthActionPageObjects.FlagRowDetailValueXPathTemplate, flagLineDiv, row, col),
                    How.XPath).Text;
        }

        public string GetClientDataSourceValue(int flagLineDiv = 1, int row = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthActionPageObjects.ClientDataSourceValueXPathTemplate, flagLineDiv, row),
                    How.XPath).Text;
        }

        public string GetFlagRowDetailLabel(int flagLineDiv = 1, int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthActionPageObjects.FlagRowDetailLabelXPathTemplate, flagLineDiv, row, col),
                    How.XPath).Text;
        }

        public void ClickOnFlag(int flagLineDiv = 1, int row = 1, bool isInternalUser = true)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthActionPageObjects.FlagXPathTemplate, flagLineDiv, row, isInternalUser ? 3 : 2),
                How.XPath);
        }

        public void ClickOnProcCode(int flagLineDiv = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(PreAuthActionPageObjects.ProcCodeXPathTemplate, flagLineDiv),
                How.XPath);
        }

        
        public PreAuthSearchPage ClickOnReturnToPreAuthSearch()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.ReturnToPreAuthSearchCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            return new PreAuthSearchPage(Navigator, new PreAuthSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public FlagPopupPage ClickOnFlagandSwitch(string title, int flagLineDiv = 1, int row = 1,
            bool isInternalUser = true)
        {
            var popupCode = Navigator.Navigate<FlagPopupPageObjects>(() =>
            {
                ClickOnFlag(flagLineDiv, row, isInternalUser);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(title));
                FlagPopupPageObjects.AssignPageTitle = title;

                Console.WriteLine("Switch To " + title + " Page");
            });
            return new FlagPopupPage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage ClickOnProcCodeAndSwitch(string codeType, string procCode, int flagLineDiv = 1)
        {
            string windowName = procCode;
            NewPopupCodePage.AssignPageTitle(string.Format("{0} - {1}", codeType, windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                ClickOnProcCode(flagLineDiv);
                SiteDriver.SwitchDynamicWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }



        public void SelectReasonCode(int reasoncode)
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.FlaggedLineReasonCodeToggleCssSelector,
                How.CssSelector);
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthActionPageObjects.FlaggedLineReasonCodesXPathTemplate, reasoncode),
                How.XPath);
        }

        public void ClickOnDeleteOrRestoreIcon(string title)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthActionPageObjects.FlaggedLineDeleteRestoreIconXPathTemplate, title),
                How.XPath);
        }

        #region database

        public List<string> GetFlagListByHcidone(string preauthSeq, bool isHciDone = false)
        {
            return Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.FlagListByHcidone, preauthSeq,
                isHciDone ? 'T' : 'F'));
        }

        public List<string> GetFlagListByClientdone(string preauthSeq, bool isClientDone = false)
        {
            return Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.FlagListByClientdone,
                preauthSeq, isClientDone ? 'T' : 'F'));
        }

        public string GetProcDesc(string procCode, string code_type)
        {
            var description = Executor
                .GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetProcDescFromDb, procCode, code_type))
                .ToString();
            return description;
        }

        public List<string> GetFlagOrderSequenceListByFlag(string flags)
        {
            return Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.FlagOrderSequenceList, flags));
        }

        public List<string> GetAllFlagList(string preAuthSeq, int linno = 1)
        {
            return Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.AllFlag, preAuthSeq, linno));
        }

        public List<string> GetClientFlagList(string preAuthSeq, int linno = 1)
        {
            return Executor.GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.ClientFlag, preAuthSeq, linno));
        }

        public List<string[]> GetLineDetailsValuesFromDB(string preAuthSeq)
        {
            var resultList =
                Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.PreAuthLineValues, preAuthSeq));

            //var d = resultList["LINNO"];
            //foreach (var dataRow in resultList)
            //{

            //}
            List<string[]> results = resultList.Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
            return results;
        }

        public string GetProcCodeDescriptionFromDB(string code, string code_type = "DEN")
        {
            var result = Executor
                .GetSingleStringValue(
                    string.Format(PreAuthSQLScriptObjects.PreAuthProcCodeDescription, code, code_type)).ToString()
                .Replace("  ", " ");
            return result;
        }

        public string GetShortProcCodeDescriptionFromDB(string code, string code_type = "DEN")
        {
            var result = Executor
                .GetSingleStringValue(
                    string.Format(PreAuthSQLScriptObjects.PreAuthShortProcCodeDescription, code, code_type)).ToString()
                .Replace("  ", " ");
            return result;
        }
        #endregion

        public List<string> GetLineNumberValues()
        {
            ScrollToLastDiv("LineItems");
            List<string> linenolist =
                JavaScriptExecutor.FindElements(PreAuthActionPageObjects.LineNoCssSelector, How.CssSelector, "Text");
            return linenolist;
        }

        public List<string> GetProcCodeDescription()
        {
            List<string> procCodeDescriptionlist =
                JavaScriptExecutor.FindElements(PreAuthActionPageObjects.ProcCodeDescriptionCssSelector, How.CssSelector,
                    "Text");
            return procCodeDescriptionlist;
        }



        public NewPopupCodePage ClickOnLineDetailsProcCodeAndSwitch(string codeType, string procCode, int LineDiv = 1)
        {
            string windowName = procCode;
            NewPopupCodePage.AssignPageTitle(string.Format("{0} - {1}", codeType, windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(
                    string.Format(PreAuthActionPageObjects.LineDetailsProcCodeXPath, LineDiv),
                    How.XPath);
                SiteDriver.SwitchDynamicWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsPatientClaimHistoryPresent()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                return SiteDriver.Title.Equals(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue());

            }
            return false;
        }


        public bool IsLineItemOrFlagAuditHistoryIconPresent(string title)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.LineItemsOrFlagAuditHistoryCssSelectorByTitle, title),
                How.CssSelector);
        }


        public int GetCountOfEnabledEditFlagIcons()
        {
            return JavaScriptExecutor.FindElementsCount(PreAuthActionPageObjects.EnabledEditFlagIconsCssLocator);
        }

        public void ClickOnEditFlagIconByLineNoAndRow(string lineNo = "1", string row = "1")
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthActionPageObjects.EditFlagIconByFlagLineNoAndRowXPathTemplate, lineNo, row),
                How.XPath);
            SiteDriver.WaitForCondition(() => !GetSideWindow.IsIFrameDisabled("Note"));
        }

        public bool IsEditFlagFormSectionPresent()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthActionPageObjects.EditFlagFormSectionCssLocator);
        }

        public bool IsEditFlagIconDisabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.EditFlagIconCssLocator, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsAllEditIconDisabled()
        {
            return SiteDriver.FindElementsAndGetAttributeByClass("is_disabled",
                PreAuthActionPageObjects.EditFlagIconCssLocator,
                How.CssSelector).All(x => x.Equals(true));

        }
        public bool IsEditDentalRecordIconDisabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.EditDentalRecordCssLocator, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public void ClickOnLineItemOrFlagAuditHistoryIconByTitle(string title)
        {
            SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.LineItemsOrFlagAuditHistoryCssSelectorByTitle, title),
                How.CssSelector).Click();
            WaitForWorking();
        }

        public bool IsFlagAuditsPresent()
        {
            bool flagAudits =
                SiteDriver.IsElementPresent(PreAuthActionPageObjects.FlagAuditRecordsCssSelector, How.CssSelector);
            if (flagAudits)
            {
                return true;
            }

            return false;
        }

        public string GetNoFlagAuditRecordMessage()
        {
            return SiteDriver
                .FindElement(PreAuthActionPageObjects.FlagAuditHistoryNoAuditRecordMessageCssLocator,
                    How.CssSelector).Text;
        }

        public int GetFlagAuditRecordCount()
        {
            return SiteDriver.FindElementsCount(PreAuthActionPageObjects.FlagAuditRecordsCssSelector, How.CssSelector);
        }

        public List<string> GetLinenumberOrFlagFromFlagAuditRecord(int col, string header)
        {
            ScrollToLastDiv(header);
            return JavaScriptExecutor.FindElements(string.Format(PreAuthActionPageObjects.FlagAuditRecordLineNumberFlagSelectionCssSelectorTemplate, col), How.CssSelector, "Text");

            //return JavaScriptExecutor.FindElements(
            //    string.Format(PreAuthActionPageObjects.FlagAuditRecordLineNumberFlagSelectionCssSelectorTemplate, col),
            //     "Text");
        }

        public bool IsLineNumberOfFlagAuditHistorySorted(string header, int col = 1)
        {
            return GetLinenumberOrFlagFromFlagAuditRecord(col, header).IsInAscendingOrder();
        }

        public bool IsFlagAuditHistoryModDateSorted(int row)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryModDateCssSelectorTemplate, row),
                How.CssSelector, "Text").IsInDescendingOrder();
        }

        public int GetFlagAuditHistoryActionCount(int lineNumber)
        {
            return SiteDriver.FindElementsCount(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryActionsCssSelectorTemplate, lineNumber),
                How.CssSelector);
        }

        public List<string> GetFlagAuditHistoryRecordLabels(int lineNum, int row)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryRecordLabelCssSelectorTemplate, lineNum, row),
                How.CssSelector, "Text");
        }

        public List<string> GetFlagAuditHistoryRecordsData(int row, int col)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryRecordValuesListCssSelectorTemplate, row, col),
                How.CssSelector, "Text");
        }

        /// <summary> GetFlagAuditHistoryValue()</summary>
        /// <param name="lineNum">Takes the Flag line number for which the detail is to be retrieved from the Flag History Audit pane</param>
        /// 
        /// <param name="auditRecordRow">Selects the individual history row containing the following values :
        /// Mod Date, By, User Type, Action, Reason Code, Client Display, Notes
        /// auditRecordRow = 2 retrieves the latest record for the supplied lineNum
        /// </param>
        /// 
        /// <param name="row">Used along with col.
        /// Retrieves the value for each label inside the supplied auditRecordRow (Eg : value of Mod Date, By, User Type, etc.)</param>
        /// 
        /// <param name="col">Used along with row.
        /// Retrieves the value for each label inside the supplied auditRecordRow (Eg : value of Mod Date, By, User Type, etc.)</param>
        /// 
        /// <returns>Returns the string value contained in the Flag Audit History pane</returns>
        public string GetFlagAuditHistoryValue(int lineNum, int auditRecordRow, int row, int col)
        {
            var text = JavaScriptExecutor.FindElement(string.Format(
                PreAuthActionPageObjects.FlagAuditHistoryRecordValueCssSelectorTemplate, lineNum,
                auditRecordRow, row, col)).Text;

            return text;
        }
        public string GetFlagAuditHistoryByLinNoFlagLabel(int lineNum, string flag, string label)
        {
            return SiteDriver.FindElement(string.Format(
                PreAuthActionPageObjects.FlagAuditHistoryByLinNoFlagLabelXPathTemplate, lineNum,
                flag, label), How.XPath).Text;
        }

        public bool IsModDateFormatCorrect(int lineNum, int innerLineNum, int row, int col)
        {
            return JavaScriptExecutor.FindElement(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryRecordValueCssSelectorTemplate, lineNum,
                    innerLineNum, row, col)).Text.IsDateTimeInFormat();
        }

        public bool IsFirstNameLastNameInUserName(int lineNum, int innerLineNum, int row, int col)
        {
            var username = SiteDriver.FindElement(string.Format(
                PreAuthActionPageObjects.FlagAuditHistoryRecordValueCssSelectorTemplate, lineNum,
                innerLineNum, row, col), How.CssSelector).Text.Split(' ').ToList().Count;
            return username.Equals(2);
        }

        public void ClickSaveButton()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.SaveButtonByCss, How.CssSelector);
            WaitForWorkingAjaxMessage();
        }
        public void ClickSaveButtonLineLevel()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.SaveButtomonlineByXpath, How.XPath);
        }


        public void ClickCancel()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.CancelLinkXPath, How.CssSelector);
        }

        public string GetEditFlagToolTipByFlagNo(string flagNo = "1")
        {
            return JavaScriptExecutor
                .FindElement(string.Format(PreAuthActionPageObjects.EditFlagToopTipByFlagNoCssLocator, flagNo)).Text;
        }

        public bool IsIconInFlaggedLinesSectionActive(string iconName)
        {
            return JavaScriptExecutor.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlaggedLinesSectionActiveIconsByIconName, iconName));
        }

        public string GetEditFlaggedLinesFormSectionHeader()
        {
            return JavaScriptExecutor.FindElement(PreAuthActionPageObjects.EditFlagFormSectionHeaderCssLocator).Text;
        }

        public List<string> GetReasonCodeListFromDB(bool isInternalUser = true)
        {
            var sqlString = (isInternalUser)
                ? PreAuthSQLScriptObjects.PreAuthReasonCodeListForInternalUser
                : PreAuthSQLScriptObjects.PreAuthReasonCodeListForClientUser;

            var resultList = Executor.GetTableSingleColumn(sqlString);
            return resultList;
        }

        public int GetCountOfDeletedFlags()
        {
            return JavaScriptExecutor.FindElementsCount(PreAuthActionPageObjects.GetCountOfDeletedFlagsCssLocator);
        }

        public bool IsVisibleToClientIconPresent()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthActionPageObjects.VisibleToClientCheckboxCssLocator);
        }

        public bool IsVisibleToClientIconChecked()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthActionPageObjects.VisibleToClientCheckboxCssLocator +
                                                       ".active");
        }

        public void ClickVisibleToClientIcon(bool check = true)
        {
            if (!IsVisibleToClientIconPresent())
                return;

            if (IsVisibleToClientIconChecked() && !check)
                JavaScriptExecutor.FindElement(PreAuthActionPageObjects.VisibleToClientCheckboxCssLocator).Click();

            if (!IsVisibleToClientIconChecked() && check)
                JavaScriptExecutor.FindElement(PreAuthActionPageObjects.VisibleToClientCheckboxCssLocator).Click();
        }

        public void DeleteRestoreFlagByLineNoAndFlagName(int lineNo, string flagName, bool delete = true)
        {
            string option = delete ? "Delete" : "Restore";
            string note = delete ? "Delete Note" : "Restore Note";
            JavaScriptExecutor.FindElement(string.Format(PreAuthActionPageObjects.EditIconByLineNoAndFlagNameCssLocator,
                lineNo, flagName)).Click();
            ClickOnDeleteOrRestoreIcon(option);
            SelectReasonCode(2);
            InputNotes(note);
            WaitForStaticTime(1500);
            ClickSaveButton();
            WaitForWorkingAjaxMessage();
        }

        public List<string[]> GetHciDoneAndCliDoneValuesByAuthSeqLinNoFlagName(string authSeq, int linNo,
            string flagName)
        {
            var resultList = Executor.GetCompleteTable(
                string.Format(PreAuthSQLScriptObjects.HciDoneAndCliDoneValuesByAuthSeqLinNoFlagName, authSeq, linNo,
                    flagName));

            var resultsHciDoneCliDone =
                resultList.Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
            return resultsHciDoneCliDone;
        }

        public void InputNotes(string notes)
        {
            SiteDriver.SwitchFrameByCssLocator("iframe.cke_wysiwyg_frame");
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.ClearElementField();
            element.SendKeys(notes);
            SiteDriver.SwitchBackToMainFrame();
        }




        public void ClearNotes()
        {
            SiteDriver.SwitchFrameByCssLocator("iframe.cke_wysiwyg_frame");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("p", How.CssSelector).ClearElementField();
            SiteDriver.SwitchBackToMainFrame();
        }

        public string GetInputValueOfNotes()
        {
            try
            {
                JavaScriptExecutor.SwitchFrameByJQuery("iframe.cke_wysiwyg_frame");
                JavaScriptExecutor.ExecuteClick("body",How.CssSelector);
                bool notePresence = SiteDriver.IsElementPresent("body", How.CssSelector);
                if (!notePresence)
                {
                    return "";
                }

                var element = SiteDriver.FindElement("body", How.CssSelector);
                return element.Text;
            }

            finally
            {
                SiteDriver.SwitchBackToMainFrame();
            }

        }

        public string GetHeaderOfBottomRightSection()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.HeaderOfBottomRightSectionCssLocator,
                How.CssSelector).Text;
        }

        public bool DoesLineNumberStartsWith1(int col, string header)
        {
            return GetLinenumberOrFlagFromFlagAuditRecord(col, header)[0].Equals("1");
        }

        public bool IsDeleteRestoreIconDisabled(string title)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.FlaggedLineDeleteRestoreIconXPathTemplate, title),
                How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        public string GetPreAuthDetailValueByTitle(string title)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthActionPageObjects.PreAuthDetailValueByTitleCssSelector, title),
                How.CssSelector).Text;
        }

        public bool IsNotePresent(int lineNum, int row)
        {
            bool note = SiteDriver.IsElementPresent(
                string.Format(PreAuthActionPageObjects.FlagAuditHistoryNotesLabelCssSelectorByLineNumAndRow, lineNum,
                    row), How.CssSelector);

            if (!note)
            {
                return false;
            }

            return true;
        }

        public void ClickOnApproveIcon()
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.ApproveIconCssLocator, How.CssSelector).Click();
            WaitForWorking();
        }

        public PreAuthSearchPage ClickOnApproveIconAndNavigateToPreAuthSearchPage()
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.ApproveIconCssLocator, How.CssSelector).Click();
            WaitForWorkingAjaxMessage();
            return new PreAuthSearchPage(Navigator, new PreAuthSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool ClickApproveAndCheckIfApproveIconIsDisabled()
        {

            var isDisabled = JavaScriptExecutor.ClickAndGetWithJquery(PreAuthActionPageObjects.ApproveIconCssLocator,
                                 PreAuthActionPageObjects.DisabledApproveIconCssSelector) != null;
            WaitForWorking();
            return isDisabled;

        }

        public void ClickMoreOption()
        {
            var element = SiteDriver.FindElement(PreAuthActionPageObjects.MoreOptionCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.PreAuthorization.GetStringValue());
                }
            }
        }

        public void CloseLastWindow()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                SiteDriver.SwitchToLastWindow();
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.PreAuthorization.GetStringValue());
            }

        }

        public bool HandleAutomaticallyOpenedPatientClaimHistoryPopup(int numberofCurrentlyOpenTabs = 1)
        {
            bool isHandled = false;
            try
            {
                SiteDriver.WaitForCondition(() =>
                {
                    if (SiteDriver.WindowHandles.Count > numberofCurrentlyOpenTabs)
                        isHandled = SiteDriver.SwitchWindowByTitle(
                            PageTitleEnum.ExtendedPageClaimHistory.GetStringValue());
                    return isHandled;
                }, 7000);

                if (SiteDriver.CloseWindowAndSwitchTo(_originalWindow))
                    Console.WriteLine("Automatically opened Patient Claim History page closed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch exception and close window" + ex.Message);
                SiteDriver.SwitchWindow(_originalWindow);
            }

            return isHandled;
        }

        public void ScrollToLastDiv(string header)
        {
            switch (header)
            {
                case "LineItems":
                    JavaScriptExecutor.ExecuteToScrollToSpecificDiv(PreAuthActionPageObjects.LastDivOfLineItemsCssSelector);
                    break;
                case "FlaggedLines":
                    JavaScriptExecutor.ExecuteToScrollToSpecificDiv(PreAuthActionPageObjects.LastDivOfFlaggedLinesCssSelector);
                    break;
                case "FlagAuditHistory":
                    JavaScriptExecutor.ExecuteToScrollToSpecificDiv(PreAuthActionPageObjects.LastDivOfFlagAuditHistoryCssSelector);
                    break;
            }

        }

        public bool IsLockIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.LockIconCssSelector, How.CssSelector);
        }

        public bool IsPreAuthLocked(string clientname, string authseq, string userID)
        {
            var lockedAppealList =
                Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreauthLock, clientname, userID));
            return lockedAppealList.Any(x => x.Contains(authseq));

        }

        public string GetToolTipOfLockIcon()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.LockIconCssSelector, How.CssSelector).GetAttribute("title");
        }


        public PreAuthActionPage SwitchBackToPreAuthActionPage()
        {

            var newPreAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>

            {
                SiteDriver.SwitchWindow(_originalWindow);
                StringFormatter.PrintMessage("Switch back to New Claim Action Page.");
            });
            return new PreAuthActionPage(Navigator, newPreAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public ClaimHistoryPage SwitchToPatientClaimHistory(bool notable = false)
        {
            var patientHistory = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                if (notable)
                    SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.EmptymessageCssSelector, How.CssSelector));
                else
                    SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector));
                Console.WriteLine("Switch To Patient Claim History Page");
            });
            return new ClaimHistoryPage(Navigator, patientHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }


        public void CloseAnyPopupIfExist(string windowHandle)
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!windowHandle.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.PreAuthorization.GetStringValue());
                }
            }
        }

        public List<string> GetPatientProviderDetails(List<string> patprovTitle)
        {
            List<string> patprovdetaiList = new List<string>();
            for (int i = 0; i < patprovTitle.Count; i++)
            {
                patprovdetaiList.Add(SiteDriver.FindElement(
                    string.Format(PreAuthActionPageObjects.PreAuthDetailValueByTitleCssSelector, patprovTitle[i]),
                    How.CssSelector).Text);
            }

            return patprovdetaiList;
        }

        #region Upload Pre Auth Document
        public string GetTitleOfUpperRightQuadrant()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.UpperRightQuadrantTitleCssLocator, How.CssSelector).Text;
        }

        public string GetDocumentUploadFormTitle()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.UploadDocumentFormHeader, How.CssSelector).Text;
        }

       
        public List<string> GetAddedDocumentList(int row=1)
        {
            return SiteDriver.FindElements(Format(PreAuthActionPageObjects.addedDocumentList, row), How.CssSelector).Select(x => x.Text).ToList().ToList();
        }

        public bool IsUploadNewDocumentFormPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.UploadDocumentFormCssSelector,
                How.CssSelector);
        }


        public bool IsAddIconDisabledInDocumentUploadForm()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.AddIconCssSelector, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public void ClickOnCancelLinkOnUploadDocumentForm()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.CancelLinkCssSelector, How.CssSelector);
        }

        public void ClickOnAddIconDocumentUploadForm()
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.AddIconCssSelector, How.CssSelector).Click();
        }

        public bool IsPreAuthAuditAddedForDocumentUpload(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentUploaded,
                       authseq)).Count() > 0
                ? true
                : false;
        }

        public string GetPreAuthStatus()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.PreAuthStatusCssSelector, How.CssSelector).Text;
        }

        public bool IsPreAuthDocumentDeleteIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.PreAuthDocumentDeleteIconCssLocator,
                How.CssSelector);
        }

        public void ClickOnDocumentToViewAndStayOnPage(int docrow)
        {
            SiteDriver.FindElement(
                    Format(PreAuthActionPageObjects.PreAuthDocumentNameCssTemplate, docrow),
                    How.CssSelector)
                .Click();
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count - 1]);
        }

        public string GetOpenedDocumentText()
        {
            return SiteDriver.FindElement("body>pre", How.CssSelector).Text;
        }

        public PreAuthActionPage CloseDocumentTabPageAndBackToPreAuthAction()
        {
            var preAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
            });
            return new PreAuthActionPage(Navigator, preAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAuditAddedForDocumentDownload(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentDownload,
                       authseq)).Count() > 0
                ? true
                : false;
        }

        public bool IsAuditAddedForDocumentDelete(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentDeleted,
                       authseq)).Count() > 0
                ? true
                : false;
        }


        public bool IsDocumentDeletedInDB(string authseq)
        {
            return Executor
                .GetSingleStringValue(Format(PreAuthSQLScriptObjects.GetDeletedDocumentsStatusFromDB, authseq))
                .Equals("T");
        }

        public List<List<string>> GetDocumentUploadInformationFromDb(string authseq)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetDocumentUploadValuesFromDb, authseq));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }

        public string UploadedDocumentValueByRowColumn(int docrow, int ulrow, int col)
        {
            return FileUploadPage.AppealDocumentsListAttributeValue(docrow, ulrow, col);
        }

        public bool IsPreAuthDocumentPresent(string fileName)
        {
            return FileUploadPage.IsFollowingAppealDocumentPresent(fileName);
        }

        #endregion

        #region TRANSFER PRE-AUTH

        public bool IsTransferIconDisabled()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.TransferIconStatusCssSelector, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsTransferIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.TransferIconCssLocator, How.CssSelector);
        }

        public bool IsTransferIconEnabled()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.TransferIconCssLocator, How.CssSelector);
        }

        public void ClickOnTransferIcon()
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.TransferIconCssLocator, How.CssSelector).Click();
        }

        public bool IsTransferPreAuthFormPresent()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthActionPageObjects.TransferPreAuthFormCssSelector);
        }

        public void SetNoteInTransferPreAuth(string text)
        {
            var noteTextArea = JavaScriptExecutor.FindElement(PreAuthActionPageObjects.TransferPreAuthNoteCssSelector);
            noteTextArea.Click();
            noteTextArea.ClearElementField();
            SiteDriver.WaitToLoadNew(1000);

            noteTextArea.SendKeys(text);
        }

        public string GetNoteFromTransferPreAuth()
        {
            var noteInTextArea = JavaScriptExecutor.GetText(PreAuthActionPageObjects.TransferPreAuthNoteCssSelector);
            return noteInTextArea;
        }

        public List<string> GetStatusToolTipInPreAuthTransferForm()
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.StatusDropdownIconInTransferFormCssSelector).Click();


            var tooltipList = SiteDriver.FindElementsAndGetAttribute("Title",
                PreAuthActionPageObjects.AllStatusInStatusDropdownInPreAuthTransferForm, How.CssSelector);

            return tooltipList;
        }

        public void SelectStatusDropDownValue(string statusValue, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = JavaScriptExecutor.FindElement(PreAuthActionPageObjects.StatusDropdownInputFieldInTransferFormCssSelector);

            JavaScriptExecutor.ExecuteClick(element);
            SiteDriver.WaitToLoadNew(300);

            if (directSelect) element.SendKeys(statusValue);

            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.AllStatusInStatusDropdownInPreAuthTransferForm, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);

            Console.WriteLine("<{0}> Selected in <{1}> ", statusValue, "Status");
        }

        #endregion

        #region Claim Processing Page

        public PreAuthProcessingHistoryPage ClickOnPreAuthProcessingHistoryAndSwitch()
        {
            var preAuthProcessingHistory = Navigator.Navigate<PreAuthProcessingHistoryPageObjects>(() =>
           {
               ClickMoreOption();
               JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.PreAuthProcessingHistoryLinkXpath, How.XPath);
               SiteDriver.WaitForCondition(() =>
                   SiteDriver.SwitchWindowByUrl(PageUrlEnum.PreAuthProcessingHistory.GetStringValue()));

               SiteDriver.WaitForCondition(() =>

                   SiteDriver.IsElementPresent(PreAuthProcessingHistoryPageObjects.PreAuthHistoryRowCountCssLocator,
                       How.CssSelector));
               Console.WriteLine("Switch To Pre-Auth Processing History Page");
               SiteDriver.WaitToLoadNew(1000);
           });
            return new PreAuthProcessingHistoryPage(Navigator, preAuthProcessingHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsPreAuthProcessingHistoryOpen()
        {
            return SiteDriver.Title.Contains(PageTitleEnum.PreAuthProcessingHistory.GetStringValue());
        }


        public string GetUsername()
        {
            return SiteDriver.FindElementAndGetAttribute(PreAuthActionPageObjects.UsernameCssSelector, How.CssSelector,
                "title");
        }

        public List<string> GetAuthSeqListAfterSearchByColRow(int col)
        {
            return _gridViewSection.GetGridListValueByCol();
        }

        public void CloseCurrentWindowAndSwitchToOriginal(int window)
        {
            var handles = SiteDriver.WindowHandles.ToList();
            SiteDriver.SwitchToLastWindow();
            SiteDriver.CloseWindowAndSwitchTo(handles[window]);
        }

        public ClaimHistoryPage ClickOnPreAuthClaimProcessingHistoryAndSwitch()
        {
            var claimHistory = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                SiteDriver.FindElement(PreAuthActionPageObjects.HistoryIconCssLocator, How.CssSelector)
                    .Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimPatientHistory.GetStringValue()));
                //SiteDriver.WaitForCondition(() =>
                //SiteDriver.IsElementPresent(ClaimHistoryPageObjects.ProviderHistoryTabCssSelector,
                //        How.CssSelector));
                Console.WriteLine("Switch To Claim Patient History Page");
                SiteDriver.WaitToLoadNew(1000);
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimHistoryPage(Navigator, claimHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsTnHyperlinkPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.TNHyperlinkXpath, How.XPath);
        }

        public bool IsSaveButtonDisabled()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.DisabledSaveButtonByCss, How.CssSelector);
        }
        public bool IsCancelButtonEnabled()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.QuadrantTwoCancelButtonByCss, How.CssSelector);
        }

        public ClaimHistoryPage ClickOnTnLinkByRowToNavigateToPatientClaimHistoryPage(int row = 1)
        {

            var patientHistory = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(Format(PreAuthActionPageObjects.TNHyperlinkByXpath, row), How.XPath);
                ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.PatientToothHistory.GetStringValue();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientToothHistory.GetStringValue()));
                SiteDriver.WaitForPageToLoad();

            });
            var claimHistoryPage = new ClaimHistoryPage(Navigator, patientHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.ExtendedPageClaimHistory.GetStringValue();
            return claimHistoryPage;

            //SiteDriver.FindElement(Format(PreAuthActionPageObjects.TNHyperlinkByXpath, row), How.XPath).Click();
            //SwitchToPatientClaimHistory(false);
        }
        #endregion

        #region DatabaseInteraction

        public List<List<string>> PreAuthProcessingHistoryList(string authSeq)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.PreAuthProcessingHistoryListByAuthSeq,
                authSeq));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }


        public bool IsAllHCIDONETrue(string authSeq)
        {
            var list = Executor
                .GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.GetHciDoneStatusOfPreAuthSeq, authSeq));
            bool result = list.TrueForAll(y => y.Equals("T"));
            return result;
        }

        public bool IsAllCliDONETrue(string authSeq)
        {
            var list = Executor
                .GetTableSingleColumn(string.Format(PreAuthSQLScriptObjects.GetCliDoneStatusOfPreAuthSeq, authSeq));
            bool result = list.TrueForAll(y => y.Equals("T"));
            return result;
        }

        public List<List<string>> PatientPreAuthHistoryList(string preauthSeq)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetPatientPreAuthHistoryData,
                preauthSeq));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }

        public List<List<string>> PatientPreAuthHistoryTotalList(string preauthSeq)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetPatientPreAuthHistoryTotalData,
                preauthSeq));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }

        public string GetSystemDateFromDatabase()
        {
            return Executor.GetSingleStringValue(PreAuthSQLScriptObjects.GetSystemDateFromDatabase);
        }

        public int GetLockedPreauthCountByUser(string username, string clientName = "SMTST")
        {
            return (int)Executor.GetSingleValue(Format(PreAuthSQLScriptObjects.PreAuthLockCountByUser, clientName, username));
        }

        public List<List<string>> GetToothRecordsForTNValue(string patSeq, string toothNumber)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetToothRecordsByToothNumber,
                patSeq, toothNumber));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }

        public void UpdateProductStatusForClient(string product, string client, bool active = false)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.ActiveOrDisableProductByClient, product, client,
                active ? 'T' : 'F'));
        }


        public int GetPreAuthCountFromDatabase(string patSeq)
        {
            return (int)Executor.GetSingleValue(Format(PreAuthSQLScriptObjects.PreAuthCountByPatSequence, patSeq));
        }

        public string GetEOBMessageFromDatabase(string flag, string type = "C")
        {
            var eobmsgDB = Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetEOBMessageForFlag, flag, type));

            if (eobmsgDB.Contains("TriggerLineNo"))
                eobmsgDB = eobmsgDB.Replace("{LineNo}", "{0}")
                    .Replace("{LineProcCode}", "{1}")
                .Replace("{LineProcCodeDesc}", "{2}")
                    .Replace("{TriggerLineProcCode}", "{3}")
                    .Replace("{TriggerLineProcCodeDesc}", "{4}")
                    .Replace("{TriggerLineNo}", "{5}");
            else
                eobmsgDB = eobmsgDB.Replace("{LineNo}", "{0}")
                    .Replace("{LineProcCode}", "{1}")
                    .Replace("{LineProcCodeDesc}", "{2}")
                .Replace("{EditSugProcCode}", "{3}")
                .Replace("{EditSugProcCodeDesc}", "{4}");

            return eobmsgDB;
        }

        public string GetProcDescriptionTextOfByLineNo(int lineNo = 1)
        {
            return SiteDriver
                .FindElement(Format(PreAuthActionPageObjects.ProcCodeDescriptionByLineNumberCssSelector, lineNo), How.CssSelector).Text;
        }

        public string GetProcDescriptionByLineNo(int lineNo = 1)
        {
            return SiteDriver
                .FindElement(Format(PreAuthActionPageObjects.ProcCodeByLineNumberCssSelector, lineNo), How.CssSelector).Text;
        }

        public string GetEobMessageByLineNumber(int lineNo = 1)
        {
            return SiteDriver
                .FindElement(Format(PreAuthActionPageObjects.EobMessageByLineNoCssSelector, lineNo), How.CssSelector).Text;
        }

        #endregion

        #region LogicManager

        public void ClickOnAddLogicIconByRow(int lineNo, int row)
        {
            SiteDriver.FindElement(Format(PreAuthActionPageObjects.AddLogicIconCssSelectorByLineNumberAndRow, lineNo, row), How.CssSelector).Click();
            WaitForWorking();
            SiteDriver.WaitForCondition(() => IsLogicWindowDisplay());
        }

        public void ClickOnLogicIconWithLogicByRow(int lineNo, int row, bool close = false)
        {
            SiteDriver.FindElement(Format(PreAuthActionPageObjects.LogicIconWithLogicByLineNumAndRowCssTemplate, lineNo, row), How.CssSelector).Click();
            WaitForWorking();
            if (!close)
                SiteDriver.WaitForCondition(() => IsLogicWindowDisplayByRowPresent(lineNo, row));
            else
                SiteDriver.WaitForCondition(() => !IsLogicWindowDisplayByRowPresent(lineNo, row));
        }

        public bool IsLogicFormTextPresent(string text) => SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.LogicFormCssSelector, text), How.CssSelector);

        public bool IsLogicWindowDisplay() => SiteDriver.IsElementPresent(PreAuthActionPageObjects.LogicFormCssSelector, How.CssSelector);

        public bool IsLogicWindowDisplayByRowPresent(int lineNo, int row) => SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.LogicFormByLineNumAndByRowCssTemplate, lineNo, row), How.CssSelector);

        public bool IsClientLogicIconByRowPresent(int lineNo, int row) => SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.ClientLogicIconCssSelectorByLineNumAndRow, lineNo, row), How.CssSelector);

        public string GetPrimaryButtonText()
        {
            // return SiteDriver.FindElement()
            //       .Text;
            return JavaScriptExecutor.FindElement(PreAuthActionPageObjects.ReplyButtonByCss).Text;
        }

        public bool IsInternalLogicIconByRowPresent(int lineNo, int row) => SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.InternalLogicIconByRowCssTemplate, lineNo, row), How.CssSelector);

        public void ClickOnLogicIcon(int lineNo, int row, string userType = "")
        {

            switch (userType)
            {
                case UserType.HCIUSER:
                    SiteDriver.FindElement(PreAuthActionPageObjects.CotivitiLogicIconCssLocator, How.CssSelector).Click();
                    break;
                case UserType.CLIENT:
                    SiteDriver.FindElement(Format(PreAuthActionPageObjects.ClientLogicIconCssSelectorByLineNumAndRow, lineNo, row), How.CssSelector).Click();
                    break;
                default:
                    SiteDriver.FindElement(Format(PreAuthActionPageObjects.AddLogicIconCssSelectorByLineNumberAndRow, lineNo, row), How.CssSelector).Click();
                    break;
            }

            WaitForSpinner();

        }

        public void AddLogicMessageTextarea(string message)
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(PreAuthActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).SendKeys(message);
        }

        public bool VerifyMaxSizeOfLogicMessageTextArea(int maxLength = 500)
        {
            string message = RandomString(maxLength + 2);
            SiteDriver.FindElement(PreAuthActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(PreAuthActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).SendKeys(message);
            bool result = GetLogicMessageTextarea().Length == maxLength ? true : false;
            return result;
        }

        public string GetLogicMessageTextarea()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).GetAttribute("value");
        }

        public string GetRecentLeftLogicMessage()
        {
            return JavaScriptExecutor
                .FindElements(PreAuthActionPageObjects.LeftMessaageSectionCssLocator, How.CssSelector, "Text")
                .LastOrDefault();
        }

        public string GetRecentRightLogicMessageByRow(int row)
        {
            return SiteDriver
                .FindElement(Format(PreAuthActionPageObjects.RightMessaageSectionCssLocatorByRow, row), How.CssSelector).Text;

        }

        public bool IsLogicPlusIconDisplayed(int lineNo, int row) => SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.AddLogicIconCssSelectorByLineNumberAndRow, lineNo, row), How.CssSelector);

        public LogicManagementPage ClickOnLogicIcon(string userType = "")
        {
            var logicManagement = Navigator.Navigate<LogicManagementPageObjects>(() =>
            {
                IWebElement logicIcon;
                switch (userType)
                {
                    case UserType.HCIUSER:
                        logicIcon = SiteDriver.FindElement(PreAuthActionPageObjects.CotivitiLogicIconCssLocator,
                            How.CssSelector);
                        break;
                    case UserType.CLIENT:
                        logicIcon = SiteDriver.FindElement(PreAuthActionPageObjects.ClientLogicIconCssLocator,
                            How.CssSelector);
                        break;
                    default:
                        logicIcon = SiteDriver.FindElement(PreAuthActionPageObjects.AddLogicIconCssLocator,
                            How.CssSelector);
                        break;
                }
                if (logicIcon != null)
                {
                    logicIcon.Click();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue());
                }
            });
            return new LogicManagementPage(Navigator, logicManagement, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetLogicAssignedToValue()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.AssignedToValueCssSelector, How.CssSelector)
                .Text;
        }
        #endregion

        public LogicSearchPage ClickOnSearchIconToReturnToLogicSearchPage()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthActionPageObjects.ReturnToPreAuthSearchCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            return new LogicSearchPage(Navigator, new LogicSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        #endregion

        #region Notes 
        public bool IsAddNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.AddNotesIndicatorCssSelector, How.CssSelector);
        }

        public void ClickOnClaimNotes()
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.NotesLinkXPath,
                How.XPath).Click();
            WaitForWorkingAjaxMessage();
        }

        public String GetNoteRecordByRowColumn(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    Format(PreAuthActionPageObjects.NoteRecordsByRowColCssTemplate, col, row),
                    How.CssSelector).Text;
        }

        public bool IsPencilIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(PreAuthActionPageObjects.NotePencilIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsPencilIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(PreAuthActionPageObjects.NotePencilIconByNameCssTemplate, name));
        }


        public bool IsCarrotIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(PreAuthActionPageObjects.NoteCarrotIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsCarrotIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(PreAuthActionPageObjects.NoteCarrotIconByNameCssTemplate, name));
        }

        public bool IsVisibletoClientIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(PreAuthActionPageObjects.VisibleToClientIconInNoteContainerByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsVisibletoClientIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(PreAuthActionPageObjects.VisibleToClientIconInNoteRowByNameCssTemplate, name));
        }

        public bool IsNoteContainerPresent()
        {
            return SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.NoteContainerCssLocator),
                How.CssSelector);
        }

        public void SelectNoteTypeInHeader(string label, string value)
        {
            var element =
                JavaScriptExecutor.FindElement(
                    Format(PreAuthActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ClickJQuery(
                Format(PreAuthActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            if (
                !SiteDriver.IsElementPresent(
                    Format(PreAuthActionPageObjects.NoteTypeDropDownInputListByLabelXPathTemplate, label,
                        value), How.XPath))
                JavaScriptExecutor.ClickJQuery(
                    Format(PreAuthActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ExecuteClick(
                Format(PreAuthActionPageObjects.NoteTypeDropDownInputListByLabelXPathTemplate, label, value),
                How.XPath);
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);

        }

        public int GetNoteEditFormCount()
        {
            return SiteDriver.FindElementsCount(
                PreAuthActionPageObjects.NotesEditFormCssLocator, How.CssSelector);
        }

        public int GetNoteListCount()
        {
            return SiteDriver.FindElementsCount(
                PreAuthActionPageObjects.NotesListCssLocator, How.CssSelector);

        }

        public string GetVisibleToClientTooltipInNotesList()
        {
            return JavaScriptExecutor.FindElement(PreAuthActionPageObjects.VisibleToClientIconInNoteRowCss)
                .GetAttribute("title");
        }

        public void ClickOnEditIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(PreAuthActionPageObjects.NotePencilIconByRowCssTemplate,
                row));
            SiteDriver.WaitForCondition(IsFramePresent);
            //SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Edit Icon");
        }

        public void ClickOnEditIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(PreAuthActionPageObjects.NotePencilIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(() => IsNoteEditFormDisplayedByName(name));
            Console.WriteLine("Clicked On Small Edit Icon");
        }

        public void ClickOnExpandIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(PreAuthActionPageObjects.NoteCarrotIconByRowCssTemplate,
                row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Exapnd Icon");
        }

        public void ClickOnExpandIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(PreAuthActionPageObjects.NoteCarrotIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(() => IsNoteEditFormDisplayedByName(name));
            Console.WriteLine("Clicked On Small Exapnd Icon");
        }

        public bool IsNoteFormEditableByName(string name)
        {
            return (IsVisibleToClientCheckboxPresentInNoteEditorByName(name) && IsNoteTextAreaEditable(name));
        }

        public bool IsNoteTextAreaEditable(string name)
        {
            bool isEditable = false;
            JavaScriptExecutor.SwitchFrameByJQuery(
                Format("div.note_row:has(span:contains({0})) iframe.cke_wysiwyg_frame", name));
            if (SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable") == "true")
            {
                isEditable = true;
            }

            SiteDriver.SwitchBackToMainFrame();
            return isEditable;
        }

        public void ClickOnCollapseIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(PreAuthActionPageObjects.NoteCarrotDownIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Collaspe Icon");
        }

        public bool IsNoteEditFormDisplayedByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.NotesEditFormByRowXpath, row),
                How.XPath);

        }

        public bool IsNoteEditFormDisplayedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(PreAuthActionPageObjects.NotesEditFormByNameCssSelector, name));

        }

        public void ClickOnSaveButtonInNoteEditorByRow(int row)
        {
            var element = SiteDriver.FindElement(
                Format(PreAuthActionPageObjects.NotesEditFormSaveButtonCssLocator, row), How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void ClickOnSaveButtonInNoteEditorByName(string name)
        {
            SiteDriver.WaitToLoadNew(2000);
            JavaScriptExecutor.ClickJQuery(
                Format(PreAuthActionPageObjects.NotesEditFormSaveButtonByNameCssLocator, name));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void ClickOnCancelButtonInNoteEditorByRow(int row)
        {
            var element = SiteDriver.FindElement(
                Format(PreAuthActionPageObjects.NotesEditFormCancelButtonCssLocator, row), How.CssSelector);

            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void ClickOnCancelButtonInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(PreAuthActionPageObjects.NotesEditFormCancelButtonByNameCssLocator, name));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void CheckVisibleToClientInNoteEditorByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(PreAuthActionPageObjects.VisibleToClientCheckBoxInNoteEditorByRowCssLocator, row));
        }

        public void ClickVisibleToClientCheckboxInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(PreAuthActionPageObjects.VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));
        }

        public bool IsVisibleToClientInNoteEditorSelectedByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(PreAuthActionPageObjects.SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator,
                    row), How.CssSelector);

        }

        public bool IsVisibleToClientInNoteEditorSelectedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(
                PreAuthActionPageObjects.SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }

        public bool IsVisibleToClientCheckboxPresentInNoteEditorByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(PreAuthActionPageObjects.VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }

        public bool IsNoNotesMessageDisplayed()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NoNotesMessageCssLocator, How.CssSelector);

        }

        public string GetNoNotesMessage()
        {
            return JavaScriptExecutor.FindElement(PreAuthActionPageObjects.NoNotesMessageCssLocator).Text;
        }

        public void ClickonAddNoteIcon()
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.NotesAddIconCssSelector).Click();
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Clicked on Add Icon");

        }

        public void ClickNotesIcon()
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.NOtesIconByCss).Click();
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Clicked on NOtes Icon");
        }
        public void clickPreauthNoteSectionAddNoteIcon()
        {
            JavaScriptExecutor.FindElement(PreAuthActionPageObjects.PreauthNoteSectionAddNoteByCss).Click();
            Console.WriteLine("Clicked on Add Icon");

        }

        public bool IsAddNoteIconPresent()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthActionPageObjects.NotesAddIconCssSelector);
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NotesAddIconCssSelector, How.CssSelector);
        }

        public bool IsAddNoteIconDisabled()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NotesIconDisabled, How.CssSelector);
        }

        public bool IsRedBadgeNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.RedBadgeNotesIndicatorCssSelector,
                How.CssSelector);
        }

        public string NoOfPreAauthNotes()
        {
            return !IsRedBadgeNoteIndicatorPresent()
                ? "0"
                : JavaScriptExecutor.FindElement(PreAuthActionPageObjects.RedBadgeNotesIndicatorCssSelector).Text;
        }
        public void ClickonAddNoteSaveButton()
        {

            var element =
                SiteDriver.FindElement(PreAuthActionPageObjects.AddNoteSaveButtonCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Save Button");
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnAddNoteCancelLink()
        {
            var element =
                SiteDriver.FindElement(PreAuthActionPageObjects.AddNoteCancelLinkCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Cancel Link");
        }

        public void SetAddNote(String note)
        {
            SiteDriver.SwitchFrameByCssLocator(".note_component .cke_wysiwyg_frame");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();

            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
        }
        public void ClickOnCollapseIconOnNotesByRow(int row)
        {
            SiteDriver.FindElement(Format(PreAuthActionPageObjects.NoteCarrotDownIconByRowCssTemplate,
                row), How.CssSelector).Click();
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Collaspe Icon");
        }
        public void SelectNoteType(string noteType)
        {
            SiteDriver.FindElement(PreAuthActionPageObjects.TypeInputXPath, How.XPath).Click();
            SiteDriver.WaitToLoadNew(500);
            if (!SiteDriver.IsElementPresent(
                Format(PreAuthActionPageObjects.TypeListValueXPathTemplate, noteType), How.XPath))
                SiteDriver.FindElement(PreAuthActionPageObjects.TypeInputXPath, How.XPath).Click();
            SiteDriver.FindElement(
                Format(PreAuthActionPageObjects.TypeListValueXPathTemplate, noteType), How.XPath).Click();
            Console.WriteLine("NoteType Selected: <{0}>", noteType);
        }

        public string GetGridNoteDetailByRowandCol(int row, int col)
        {
            return SiteDriver.FindElement(Format(PreAuthActionPageObjects.SavedNotesDetailsByCss, row, col), How.CssSelector).Text;
        }

        public string GetPreAuthNoteType()
        {
            return JavaScriptExecutor.FindElement(PreAuthActionPageObjects.GetPreAuthNoteType).Text;
        }

        public string GetPreAuthNoteUserName()
        {
            return SiteDriver.FindElement(PreAuthActionPageObjects.GetPreauthNoteUserName, How.XPath).Text.
                Split(new[] { Environment.NewLine }, StringSplitOptions.None)[1];
        }
        public void SetLengthyNoteToVisbleTextarea(string header,string text, bool isNoteForAddFlagSection = false,bool handlePopup=true)
        {
            SiteDriver.SwitchFrameByCssLocator(!isNoteForAddFlagSection
                ? "section:not([style*='none'])>ul iframe.cke_wysiwyg_frame.cke_reset"
                : "section.add_flag_form ul .cke_wysiwyg_frame");
            SendValuesOnTextArea(header, text, handlePopup);
        }

        public string GetNoteOfVisibleTextarea(bool isNoteForAddFlagSection = false)
        {
            SiteDriver.SwitchFrameByCssLocator(!isNoteForAddFlagSection
                ? "section:not([style*='none'])>ul iframe.cke_wysiwyg_frame.cke_reset"
                : "section.add_flag_form ul .cke_wysiwyg_frame");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }


        public string GetGridNoteDetailByCol(int row, int col)
        {
            return JavaScriptExecutor.FindElement(Format(PreAuthActionPageObjects.SavedNotesDetailsByCol, row, col)).Text;
        }
        public List<List<string>> GetNotesDetailFromDb(string authSeq)
        {
            var dataList = Executor.GetCompleteTable(Format(PreAuthSQLScriptObjects.GetNoteDetails, authSeq));
            return dataList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public bool IsDeletedFlagPresentByFlagName(string flagName)
        {
            return JavaScriptExecutor.IsElementPresent(Format(PreAuthActionPageObjects.DeletedFlagByCssSelector, flagName));
        }

        public bool IsAuditPresentForDeletedFlag(string deletedflag)
        {
            var flags = SiteDriver.FindElementsAndGetAttribute("title", Format(
               PreAuthActionPageObjects.AllFlagsInFlagAuditHistory), How.CssSelector).Select(t => Regex.Replace(t, @"\s+", "")).ToList();
            return flags.Contains(deletedflag);
        }


        public bool IsFramePresent() => SiteDriver.IsElementPresent(".cke_contents.cke_reset", How.CssSelector);

        #endregion

        public bool IsNoDataAvailableMessagePresentInFlaggedLine()
        {
            return SiteDriver.IsElementPresent(PreAuthActionPageObjects.NoDataAvailableMessageXpath, How.XPath);
        }

        public bool IsFlagAuditPresentByInternalUserTypeAndAction(string action,string flag)
        {
            return SiteDriver.IsElementPresent(Format(PreAuthActionPageObjects.FlagAuditByActionAndUserTypeXpath,action," "+flag+" "),How.XPath);
        }

        public void ClickQuickDeleteIcon()
        {
            SiteDriver
                .FindElement(PreAuthActionPageObjects.QuickDeleteIconCssSelector, How.CssSelector).Click();
            WaitForWorking();
        }

        public void ClickQuickRestoreIcon()
        {
            SiteDriver
                .FindElement(PreAuthActionPageObjects.QuickRestoreIconCssSelector, How.CssSelector).Click();
            WaitForWorking();
        }

        public bool IsQuickDeleteIconPresent() =>
            SiteDriver.IsElementPresent(PreAuthActionPageObjects.QuickDeleteIconCssSelector, How.CssSelector);

        public bool IsQuickRestoreIconPresent() =>
            SiteDriver.IsElementPresent(PreAuthActionPageObjects.QuickRestoreIconCssSelector, How.CssSelector);

        public bool AreAllFlagsDeleted()
        {
            List<bool> isDeleted = SiteDriver.FindElementsAndGetAttributeByClass("is_deleted",
                PreAuthActionPageObjects.FlagLinesCssSelector,
                How.CssSelector);
            return isDeleted.TrueForAll(c => c);
        }

        public bool AreFlagsStruckThrough()
        {
            List<bool> isStruck = new List<bool>();
            var list = SiteDriver.FindElements(
                (PreAuthActionPageObjects.FlagsOfFlagLineCssSelector),
                How.CssSelector);
            foreach (var flag in list)
            {
                isStruck.Add(flag.GetCssValue("text-decoration").Contains("line-through"));
                
            }

            return isStruck.TrueForAll(c => c);
        }

        public void UploadPreAuthDocument(string fileType, string description, string fileName)
        {
            GetFileUploadPage.SetFileTypeListVlaue(fileType);
            GetFileUploadPage.SetFileUploaderFieldValue("Description", description);
            GetFileUploadPage.AddFileForUpload(fileName);
            GetFileUploadPage.ClickOnAddFileBtn();
        }

        public string GetTriggerProcCodeFromDatabaseByLinNoAndFlag(string preAuthSeq, string linNo, string flag)
        {
            return Executor.GetSingleStringValue(Format(PreAuthSQLScriptObjects.GetTriggerProcCodeFromDb, preAuthSeq,
                linNo, flag));
        }

        public void ClickOnEditIconOnLine(int lineno = 1)
        {
            var ele=JavaScriptExecutor.FindElement(String.Format(PreAuthActionPageObjects.EditIconInLineItemsQuadrantByLineNo, lineno));
            ele.Click();
        }

        public bool IsEditIconPresentInClaimLine(int lineno=1)
        {
            return JavaScriptExecutor.IsElementPresent(
                String.Format(PreAuthActionPageObjects.EditIconInLineItemsQuadrantByLineNo, lineno));
        }

        public void ClickOnSaveEditIconByLineNo(int lineno = 1)
        {
            JavaScriptExecutor.FindElement(string.Format(PreAuthActionPageObjects.SaveButtoninEditLineSectionInLineItemsByLineNo, lineno)).Click();

        }

        public void ClickOnCancelEditIconByLineNo(int lineno = 1)
        {
            JavaScriptExecutor.FindElement(string.Format(PreAuthActionPageObjects.CancelButtoninEditLineSectionInLineItemsByLineNo, lineno)).Click();

        }

        public void SetToothInformationInLineBynoandLabel(string label, string value, int lineno = 1)
        {
            var element = JavaScriptExecutor.FindElement(
                String.Format(PreAuthActionPageObjects.EditLineSectionInLineItemsQuadrantByLineNo, lineno, label));
            element.ClearElementField();
            element.Click();
            element.SendKeys(value);

        }

        public string GetToothInformationInLineBynoandLabel(string label, int lineno = 1)
        {
            var element = JavaScriptExecutor.FindElement(
                String.Format(PreAuthActionPageObjects.EditLineSectionInLineItemsQuadrantByLineNo, lineno, label));
            return element.GetAttribute("value");


        }

        public void DeleteToothInfoForPreAuth(string sequence)
        {
            Executor.ExecuteQuery(Format(PreAuthSQLScriptObjects.DeleteToothInfoFromDB,sequence));
        }

        public List<string> GetToothInfoForPreAuth(string sequence)
        {
            var list=Executor.GetCompleteTable(Format(PreAuthSQLScriptObjects.GetToothInfoForPreauthFromDB, sequence));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList()[0];

        }
    }
}
