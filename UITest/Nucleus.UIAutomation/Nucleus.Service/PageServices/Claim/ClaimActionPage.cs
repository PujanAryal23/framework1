using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.QA;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using CheckBox = UIAutomation.Framework.Elements.CheckBox;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using Keys = OpenQA.Selenium.Keys;



namespace Nucleus.Service.PageServices.Claim
{
    public class ClaimActionPage : NewDefaultPage

    {
        #region PRIVATE FIELDS

        private ClaimActionPageObjects _claimActionPage;
        private readonly string _originalWindow;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;
        private readonly FileUploadPage _fileUploadPage;
        private readonly Mouseover _mouseOver;
        private readonly HeaderMenu _headerMenu;

        #endregion

        #region PUBLIC FIELDS

        public static int FlagCount;
        private SideWindow _sideWindow;

        public enum ActionItems
        {
            Edit = 1,
            DeleteOrRestore = 2,
            Approve = 3,

            ApproveNext = 4,
            Next = 5,
            TransferDropDown = 6,
            AddViewDocuments = 7
        }

        public string OriginalWindowHandle
        {
            get { return _originalWindow; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }


        #endregion

        #region CONSTRUCTOR



        public ClaimActionPage(INewNavigator navigator, ClaimActionPageObjects claimActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor,
            bool handlePopup = false)
            : base(navigator, claimActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)

        {
            _claimActionPage = claimActionPage;
            //_claimActionPage = (ClaimActionPageObjects) PageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _commonSqlObjects = new CommonSQLObjects(Executor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _fileUploadPage = new FileUploadPage(SiteDriver,JavaScriptExecutor);
            _mouseOver = new Mouseover(SiteDriver, JavaScriptExecutor);
            _headerMenu = new HeaderMenu(SiteDriver,JavaScriptExecutor);
            if (handlePopup)

                HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }



        #endregion

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public FileUploadPage GetFileUploadPage
        {
            get { return _fileUploadPage; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        
        public List<List<String>> GetAllCotivitiExpectedLists()
        {
            var list = new List<List<String>>();
            var editFlagList = Executor.GetTableSingleColumn(ClaimSqlScriptObjects.EditFlagList);
            editFlagList.Insert(0, "All");
            list.Add(editFlagList);
            return list;
        }

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        #region PUBLIC METHODS

        /// <summary>
        /// QA => QA Claim search
        /// </summary>
        /// <returns>QA Claim search</returns>
        public QaClaimSearchPage NavigateToQaClaimSearch()

        {
            var qaClaimSearchPage = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                if (GetMouseOver.IsQAClaimSearch())
                    ClickOnMenu(SubMenu.QaClaimSearch);
                JavaScriptExecutor.WaitForJqueryStatusCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(QaClaimSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector));
            });

            return new QaClaimSearchPage(Navigator, qaClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void WaitToLoadPatientClaimHistoryPopUp(int waitTime = 500)
        {
            SiteDriver.WaitForCondition(IsPatientClaimHistoryPopupPresent, waitTime);
        }
        public bool IsPatientClaimHistoryPopupPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.PopUpCssSelector, How.CssSelector), 500);
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.PopUpCssSelector, How.CssSelector, 200);
        }

        #region ClaimQA

        public bool IsRecordQaErrorSectionPresent(int row = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.RecordQaErrorSectionCssTemplate, row), How.CssSelector);
        }

        public string GetClaimQaRowValueByCol(int col = 1, int row = 1)
        {
            return SiteDriver.FindElement(

                    Format(ClaimActionPageObjects.ClaimQaRowValueByColCssTemplate, row, col), How.CssSelector)

                .Text;
        }

        public List<string> GetClaimQaRowValueListByCol(int col = 1)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.ClaimQaRowValueListCssTemplate, col), How.CssSelector, "Text");
        }

        public bool IsNoteIconPresentInClaimQaByRow(int row = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.ClaimQaNoteIconByRowCssTemplate, row), How.CssSelector);
        }

        public bool IsEditIconPresentInClaimQaByRow(int row = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.ClaimQaEditFlagCssTemplate, row), How.CssSelector);

        }

        #endregion

        public void ClickCancelButtonOnRecordQaError(int row = 1)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.CancelButtonOnRecordQaErrorCssTemplate, row),
                    How.CssSelector)
                .Click();
        }

        public void ClickClaimActionQaPass()
        {

            SiteDriver.FindElement(Format(ClaimActionPageObjects.ClaimQaPassCssSelector),
                How.CssSelector).Click();
        }

        public ClaimActionPage ClickClaimQaPassAndWaitForNextClaim()
        {
            var newClaimPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickClaimActionQaPass();
                Console.WriteLine("Clicked on QA Pass button");
            });

            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
            HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return new ClaimActionPage(Navigator, newClaimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public QaClaimSearchPage ClickClaimQaPassAndNavigateToQaClaimSearchPage()
        {
            var qaClaimSearchPage = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                ClickClaimActionQaPass();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on QA Pass button");
            });

            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            return new QaClaimSearchPage(Navigator, qaClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickClaimActionQaFail()
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.ClaimQaFailCssSelector),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
        }

        public void ClickClaimQaCancelLink()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimQaCancelLinkCssSelector, How.CssSelector)
                .Click();

        }

        public List<string[]> GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(string client, string hcidoneOrCliDone ,string claseq, int linNo, string flagName)
        {
            var resultList = Executor.GetCompleteTable(
                string.Format(ClaimSqlScriptObjects.HciDoneAndCliDoneValuesByAuthSeqLinNoFlagName, hcidoneOrCliDone, client ,claseq, linNo, flagName));

            var resultsHciDoneCliDone =
                resultList.Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
            return resultsHciDoneCliDone;
        }

        public string GetProviderSpecFromDB(string claseq)
        {
           return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.ProviderSpecialty, claseq));
        }
        public string GetSpecialtyFromDB(string prvseq)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.providerSpec, prvseq));
        }

        public string GetProvierDetailSpecialtyFromDB(string prvseq)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.ProviderDetailSpec, prvseq));
        }

        public bool CheckQaDoneStatus()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.QaDoneCssSelector, How.CssSelector);
        }

        public void ClickonOnEditOnClaimQaByRow(int row = 1)
        {
            SiteDriver.FindElement(Format(ClaimActionPageObjects.ClaimQaEditFlagCssTemplate, row),
                How.CssSelector).Click();
        }

        public void SelectRecordQaErrorDropdown(string label, string value)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),

                How.XPath);
            JavaScriptExecutor.ExecuteClick(

                Format(ClaimActionPageObjects.RecordQaErrorDropdownListValueXPath, label, value), How.XPath);
        }



        public bool IsTypeAheadForRecordQaErrorDropdown(string label, string value)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),
                How.XPath);

            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label), How.XPath).ClearElementField();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label), How.XPath).SendKeys(value);
            var isEqual = JavaScriptExecutor.FindElements(
                    Format(ClaimActionPageObjects.RecordQaErrorDropdownListXPath, label), How.XPath,
                    "Text")[0]
                .Equals(value);
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),

                How.XPath);
            return isEqual;
        }

        public void SetRecordQaErrorDropdown(string label, string value)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),
                How.XPath);
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label), How.XPath).SendKeys(value);
        }

        public List<string> GetRecordQaErrorDropdownList(string label)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),
                How.XPath);
            var list = JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.RecordQaErrorDropdownListXPath, label), How.XPath, "Text");
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label),
                How.XPath);
            list.RemoveAt(0);
            return list;
        }

        public string GetSelectedRecordQaErrorDropDownValue(string label)
        {
            return

                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.RecordQaErrorDropdownXPath, label), How.XPath)
                    .GetAttribute("value");
        }

        public List<string> GetExpectedErrorDescriptionList()
        {
            return new List<string>(new[]
            {

                "Informational", "Should have left edit", "Should have removed edit", "Switched", "Not Switched",
                "Manual Edit Added", "Manual Edit not added"
            });
        }

        public ClaimActionPage SaveRecordQAErrorAndNavigateToNextClaim()
        {
            var newClaimPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickSaveRecordQAError();
                Console.WriteLine("Clicked on Save button on QA Fail Section");

            });
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
            HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return new ClaimActionPage(Navigator, newClaimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickSaveRecordQAError()

        {
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimQaSaveButtonCssSelector, How.CssSelector)
                .Click();

        }

        public QaClaimSearchPage SaveRecordQaErrorAndNavigateToQaClaimSearchPage()
        {
            var qaClaimSearch = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.ClaimQaSaveButtonCssSelector, How.CssSelector)
                    .Click();
                WaitForWorkingAjaxMessage();

                Console.WriteLine("Clicked on Save button on QA Fail Section");
            });
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            return new QaClaimSearchPage(Navigator, qaClaimSearch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void Sleep(int milliSec = 0)

        {
            SiteDriver.WaitToLoadNew(milliSec);
        }

        #region sql

        public List<List<string>> GetClaimFlagNotesFromDb(string claseq,bool internluser=true)
        {
            var dataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(internluser?ClaimSqlScriptObjects.GetClaimFlagNotes: ClaimSqlScriptObjects.GetClaimFlagNotesClientUser, claseq));
            dataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return dataList;

        }


        public bool IsGetClaimStatusOfWorkListPended(string claseq)
        {
            var pendedClaims = Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetStatusOfClaimSeq,
                claseq));
            return pendedClaims == "P";
        }

        public void RevertScriptForSwitchFunctionalityCoreFlag(string claseq1, string claseq2, string claseq3,
            string status)
        {

            var claimseq1 = claseq1.Split('-').ToList();

            var claimseq2 = claseq2.Split('-').ToList();
            var claimseq3 = claseq3.Split('-').ToList();
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.RevertScriptForSwitchFunctionalityCoreFlag,
                claimseq1[0], claimseq1[1], claimseq2[0], claimseq2[1], claimseq3[0], claimseq3[1], status));
            Console.WriteLine("Status and Deleted Flag Reverted");
        }

        public void DeleteAllDeletedFlagsByClaSeq(string claSeq)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteAllDeletedFlag,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine("Delete All Deleted flag from database if exists for claseq<{0}>  ", claSeq);

        }

        public List<string> GetAssociatedBatchList(string clientCode)
        {
            var list = Executor
                .GetTableSingleColumn(Format(ClaimSqlScriptObjects.BatchIDListByClient, clientCode)).ToList();
            return list;
        }

        public List<int> GetInternalAndClientSugUnitsByClaseqAndFlag(string claimSeq,string flag)
        {
            var dataPointsValues = Executor
                .GetCompleteTable(Format(ClaimSqlScriptObjects.GetInternalAndClientSugUnitsByClaseqAndFlag, claimSeq.Split('-')[0], claimSeq.Split('-')[1], flag))
                .ToList();
            List<int> listDataPoints = new List<int>();
            for (int i = 0; i < dataPointsValues[0].Table.Columns.Count; i++)
            {
                listDataPoints.Add(Convert.ToInt32(dataPointsValues[0][i]));
            }
            return listDataPoints;
        }

        public List<string> GetExpectedQaErrorReasonCodeList()
        {
            return Executor.GetTableSingleColumn(ClaimSqlScriptObjects.QaErrorReasonCode);
        }

        public void DeleteClaimAuditRecordFromDatabase(string flagSeq, string claSeq, string lineNo, string clientName = "SMTST")
        {
            //Executor = new OracleStatementExecutor();

            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecord, flagSeq,
                claSeq.Split('-')[0], claSeq.Split('-')[1], lineNo, clientName));
            Console.WriteLine("Delete Audit Record from database if exists for claseq<{0}> flagseq<{1} lineNo<{2}>",
                claSeq, flagSeq, lineNo);

        }


        public void DeleteClaimAuditRecordFromDatabaseByClaseq(string claSeq, string lineNo, string clientName = "SMTST")
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecordByClaseq, claSeq.Split('-')[0], claSeq.Split('-')[1], lineNo, clientName));
            Console.WriteLine("Delete Audit Record from database if exists for claseq<{0}> lineNo<{1}>", claSeq,  lineNo);
        }

        public void UpdateAuditCompletedInClaimQaAuditFromDatabase(string clientName, string value, string claSeq)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateAuditCompletedInClaimQaAudit, clientName, value, claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine($"Update Audit Completed value to {value} from database for {claSeq} for {clientName}");
        }

        public void RestoreAllFlagByClaimSequence(string claSeq)
        {
            //Executor = new OracleStatementExecutor();

            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.RestoreDeletedFlagsByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine("Restore All Flag from database if exists for claseq<{0}> ",
                claSeq);

        }

        public void RestoreParticularFlagsByClaimSequence(string claSeq)
        {
            //Executor = new OracleStatementExecutor();

            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.RestoreParticularFlagsByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine("Restore All Flag from database if exists for claseq<{0}> ",
                claSeq);

        }


        public void DeleteClaimAuditRecordExceptAdd(string claSeq)
        {
            //Executor = new OracleStatementExecutor();
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecordExceptAdd,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine("Delete Audit Record from database  for claseq<{0}>", claSeq);

        }

        public void DeleteClaimAuditRecordExceptAddAndApprove(string claSeq)

        {
            //Executor = new OracleStatementExecutor();
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecordExceptApproveAndAdd,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            Console.WriteLine("Delete Audit Record from database  for claseq<{0}>", claSeq);

        }

        public void DeleteFlagLineFromDatabaseByClaimSeqAndFlag(string claSeq, string flag)
        {
            //Executor = new OracleStatementExecutor();
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteFlagLine, claSeq.Split('-')[0],
                claSeq.Split('-')[1], flag));

            Console.WriteLine("Delete Flag from database if exists for claseq<{0}>s flag<{1} ", claSeq, flag);

        }

        public void DeleteClaimAuditOnly(string claSeq, string date)
        {
            //Executor = new OracleStatementExecutor();
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditOnly, claSeq.Split('-')[0],
                claSeq.Split('-')[1], date));
            Console.WriteLine("Delete Audit from database if exists for claseq<{0}> date greater than <{1}> ", claSeq,
                date);
        }

        public void DeleteLogicNoteFromDatabase(string claimSeq)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimLogicInFlagOfLogicManager,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));

        }

        public void DeleteLineFlagAuditByClaimSequence(string claSeq, string clientCode)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteLineFlagAuditByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1], clientCode));
            Console.WriteLine("Delete Line Flag Audit from database if exists for claseq<{0}>", claSeq);
        }

        public void RestoreClaimQAAuditByClaimSequence(string claSeq)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateClaimQAAudit, claSeq.Split('-')[0],
                claSeq.Split('-')[1]));
            Console.WriteLine("Update ClaimQAAudit from database if exists for claseq<{0}>", claSeq);

        }

        public void UpdateStatusAndRestoreFlags(string claSeq, string client)

        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateStatusToUnreviewedByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1], client));
            Console.WriteLine("Update ClaimQAAudit from database if exists for claseq<{0}>", claSeq);

        }

        public void DeleteClaimAuditOnlyByClient(string claSeq, string date, string client)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditOnly, claSeq.Split('-')[0],

                claSeq.Split('-')[1], date, client));
            Console.WriteLine("Delete Audit from database if exists for claseq<{0}> date greater than <{1}> ", claSeq,
                date);
        }

        public void DeleteClaimAuditRecordFromDatabaseByCleint(string flagSeq, string claSeq, string lineNo,
            string client)

        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecordByClient, flagSeq,
                claSeq.Split('-')[0], claSeq.Split('-')[1], lineNo, client));
            Console.WriteLine("Delete Audit Record from database if exists for claseq<{0}> flagseq<{1} lineNo<{2}>",
                claSeq, flagSeq, lineNo, client);

        }


        public string GetRuleNoteForGivenClaimSeqFromDB(string claimSeq, string flagCode)
        {
            string fetchedRuleNote = Executor.GetSingleStringValue(Format(
                ClaimSqlScriptObjects.FindRuleNoteForGivenClaimSeq,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1], flagCode));
            Console.WriteLine("Fetched the rule note for the provided claimSeq : " + claimSeq);
            return fetchedRuleNote;
        }


        public bool IsRuleNoteFieldPresent()
        {

            return JavaScriptExecutor.IsElementPresent(Format(ClaimActionPageObjects.FlagDetailsTextByLabel,
                "Rule Note"));
        }

        public string GetSourceOfFlaggedLine(int row, string flag)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.FlagLineEditFlagSourceXpathTemplate, row, flag), How.XPath)
                .Text;
        }

        public int GetFlagLineManaulSourceCount(string flag)
        {
            return SiteDriver.FindElementsCount(
                Format(ClaimActionPageObjects.FlagLineManualSourceXPathTemplate, flag), How.XPath);
        }

        public string GetFlagCodeFromFlaggedLines(int row)

        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.GetFlagCodeFromFlaggedLines, row),
                How.XPath).Text;
        }

        public int GetCountOfTotalFlagsAssociatedToAClaim(string claimSeq)
        {
            int fetchedCount = Int32.Parse(Executor.GetSingleStringValue(Format(
                ClaimSqlScriptObjects.CountOfTotalFlagsAssociatedToAClaim,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1])));
            return fetchedCount;

        }

       #endregion



        #region Claim Processing Page

        public ClaimProcessingHistoryPage ClickOnClaimProcessingHistoryAndSwitch()
        {
            var claimProcessingHistory = Navigator.Navigate<ClaimProcessingHistoryPageObjects>(() =>
            {
                
                ClickMoreOption();
                SiteDriver.FindElement(ClaimActionPageObjects.ClaimProcessingHistoryLinkXpath, How.XPath)
                    .Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimProcessingHistoryPage.GetStringValue()));

                SiteDriver.WaitForCondition(() =>

                    SiteDriver.IsElementPresent(ClaimProcessingHistoryPageObjects.ProcessingHistoryLabeByCss,
                        How.CssSelector));
                Console.WriteLine("Switch To Claim Processing History Page");
                SiteDriver.WaitToLoadNew(1000);
            });
            return new ClaimProcessingHistoryPage(Navigator, claimProcessingHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #endregion

        public void WaitforReviewTime()
        {
            SiteDriver.WaitToLoadNew(12000);
        }

        #region OriginalClaimData

        public OriginalClaimDataPage ClickOnOriginalClaimDataAndSwitch()

        {
            var originalClaimData = Navigator.Navigate<OriginalClaimDataPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.OriginalClaimDataLinkXpath, How.XPath).Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.OriginalClaimDataPage.GetStringValue()));
                
                if (CurrentPageUrl.Contains("Error"))
                {
                    CaptureScreenShot("OriginalClaimDataPage");

                    Console.WriteLine("Erro Page opend");

                    while (SiteDriver.WindowHandles.Count != 1)
                    {
                        SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                        if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                        {
                            SiteDriver.CloseWindow();
                            SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                        }
                    }
                    SiteDriver.FindElement(ClaimActionPageObjects.OriginalClaimDataLinkXpath, How.XPath).Click();
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.SwitchWindowByTitle(PageTitleEnum.OriginalClaimDataPage.GetStringValue()));
                }
                WaitForPageToLoad();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(OriginalClaimDataPageObjects.PopupTitleCssLocator, How.CssSelector));
                Console.WriteLine("Switch To Original Claim Data Page");
            });
            return new OriginalClaimDataPage(Navigator, originalClaimData, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetMenuOptionsForClinicalOps(string key)
        {
            return _headerMenu.GetMenuOptionsForClinicalOps(key);
        }

        #endregion

        #region Invoice Data

        public void ClickOnInvoiceDataLink()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.InvoiceLinkXpath, How.XPath).Click();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.SwitchDynamicWindowByTitle(PageTitleEnum.InvoiceData.GetStringValue()));
            Console.WriteLine("Switch To Invoice Data Page");

        }

        public InvoiceDataPage ClickOnInvoiceData()
        {
            var invoiceData = Navigator.Navigate<InvoiceDataPageObjects>(() =>
            {

                ClickMoreOption();
                SiteDriver.FindElement(ClaimActionPageObjects.InvoiceLinkXpath, How.XPath)
                    .Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchDynamicWindowByTitle(PageTitleEnum.InvoiceData.GetStringValue()));
                Console.WriteLine("Switch To Invoice Data Page");
                SiteDriver.WaitToLoadNew(1000);
            });
            return new InvoiceDataPage(Navigator, invoiceData, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

      

        public bool IsInvoiceDataLinkOpened()
        {
            return SiteDriver.Title.Contains(PageTitleEnum.InvoiceData.GetStringValue());
        }

        #endregion

        #region SidebarPannel

        public bool IsEmptyMessageOnAdditionalLockedClaimsPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EmptyMessageOnAdditionalLockedClaims,
                How.XPath);
        }

        #endregion

        #region claim flag notes

        public string GetFlagAndLineNoFromFlagNotesSection(int row,int col)
        {
           return SiteDriver.FindElement(Format(ClaimActionPageObjects.GetFlagValueandLineFlagNotesSection, row,col),How.CssSelector).Text;
        }
        public void ClickOnClaimFlagNotesIcon()
        {
            JavaScriptExecutor.ClickJQuery(ClaimActionPageObjects.ClaimFlagNotesIconCssLocator);
        }

        public bool IsClaimFlagNoteDisplayed()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimActionPageObjects.ClaimFlagNotesIconCssLocator);
        }

        public bool IsClaimLineNotesEnabled()
        {
           return  SiteDriver.IsElementPresent(ClaimActionPageObjects.EnabledClaimFlagNotesIconCssLocator,How.CssSelector);
        }

        public string GetClaimFlagNotesIconTooltip()
        {
            return
                JavaScriptExecutor.FindElement(
                    ClaimActionPageObjects.ClaimFlagNotesIconCssLocator).GetAttribute("title");
        }

        public string GetNoflagMessage(string flag)
        {
           // return SiteDriver.FindElement(Format(ClaimActionPageObjects.NOFlagMessageByFlag,flag),How.CssSelector).Text;

            return JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.NOFlagMessageByFlag, flag)).Text;
        }

        public string GetClaimFlagNoteDetailListByFlagAndLabel(string flag, string label)

        {
            return JavaScriptExecutor.FindElement(

                Format(ClaimActionPageObjects.FlagNotesDetailsByFlagandLabel, flag,
                    label)).Text;
        }

        public List<string> GetFlagdetailabel(string flag)
        {
            return JavaScriptExecutor          
                .FindElements(Format(ClaimActionPageObjects.FlagDetailLabel, flag));
        }

        #endregion

        #region claim flag audit history

        public int GetClaimFlagAuditHistoryFlagCount(string flag)
        {
            return SiteDriver.FindElementsCount(
                Format(ClaimActionPageObjects.ClaimFlagAuditHistoryFlagSelectorXPathTemplate, flag),
                How.XPath);
        }


        public void ClickOnClaimFlagAuditHistoryIcon()
        {
            JavaScriptExecutor.ClickJQuery(ClaimActionPageObjects.ClaimFlagAuditHistoryIconCssLocator);
        }


        public string GetClaimFlagAuditHistoryIconTooltip()
        {
            return
                JavaScriptExecutor.FindElement(
                    ClaimActionPageObjects.ClaimFlagAuditHistoryIconCssLocator).GetAttribute("title");
        }

        public string GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(string flag, string label,

            string lineNo, int auditRow = 1)
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDivUsingJquery(Format(ClaimActionPageObjects.ClaimFlagAuditHistoryDetailCssTemplate, flag, auditRow,
               label, lineNo));
            return
                JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.ClaimFlagAuditHistoryDetailCssTemplate, flag, auditRow,
                        label, lineNo)).Text;

        }

        public string GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(string flag, string label)
        {
            return
                JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.ClaimFlagAuditHistoryHeaderDetailCssTemplate, flag,
                        label)).Text;
        }


        public string GetFlagOnClaimFlagAuditHistoryByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.FlagOnClaimFlagAuditHistoryCssTemplate, row),
                    How.CssSelector).Text;
        }

        public string GetLineNoOnClaimFlagAuditHistoryByRow(int row)
        {
            return
                SiteDriver.FindElement(

                    Format(ClaimActionPageObjects.LineNoOnClaimFlagAuditHistoryCssTemplate, row),

                    How.CssSelector).Text;
        }

        public List<string> GetClaimFlagAuditHistorysDetailListByFlagAndLabel(string flag, string label)

        {
            return JavaScriptExecutor.FindElements(

                Format(ClaimActionPageObjects.ClaimFlagAuditHistoryDetailListCssTemplate, flag,
                    label), "Text");
        }

        public bool IsClaimAuditHistoryDivPresentByLineNoAndFlag(string flag, string lineNo)
        {
            return
                JavaScriptExecutor.IsElementPresent(
                    Format(ClaimActionPageObjects.ClaimFlagAuditHistoryRowDivCsLocator, flag,
                        lineNo));

        }



        #endregion

        #region FlagDetail

        public NewPopupCodePage ClickOnSugCodeLinkOnFlagDetails(string codeType, string sugCode)
        {
            NewPopupCodePage.AssignPageTitle(Format("{0} - {1}", codeType, sugCode));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                JavaScriptExecutor.ClickJQuery(

                    Format(ClaimActionPageObjects.SugCodeLinkOnFlagDetailsCssTemplate, sugCode));

                Console.Out.WriteLine("Clicked on Sug Code<{0}> popup link", sugCode);

                SiteDriver.SwitchWindowByTitle(sugCode);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #endregion

        #region FlagDetails


        public void ClickOnFlagLineByFlag(string flag)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.FlagLineByFlagCssTemplate, flag));
            Console.WriteLine("Click on Flag Line having Flag<{0}>", flag);
        }

        #endregion


        public List<string> GetHeaderList()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.HeaderListCssLocator, How.CssSelector,
                "Text");

        }


        public int GetWindowHandlesCount()
        {
            return SiteDriver.WindowHandles.Count;

        }

        public ClaimActionPage SwitchBackToNewClaimActionPage()
        {

            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>

            {
                SiteDriver.SwitchWindow(_originalWindow);
                StringFormatter.PrintMessage("Switch back to New Claim Action Page.");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public void SwitchToSecondHandlePage()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count == 2)
            {
                SiteDriver.SwitchWindow(handles[1]);

                StringFormatter.PrintMessage("Switch to second handle page");


            }

        }

        public ClaimActionPage SwitchClientAndNavigateClaimAction(string clientToSwitch, string preValue,
            string currentValue, string previousclient = "SMTST")

        {
            var chromeDownLoad =
                Navigator.Navigate<ClaimActionPageObjects>(
                    () =>
                        SiteDriver.Open(CurrentPageUrl.Replace(previousclient, clientToSwitch)
                            .Replace(preValue, currentValue)));
            WaitForWorkingAjaxMessage();
            return new ClaimActionPage(Navigator, chromeDownLoad, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        #region FlagAudit


        public List<string> GetFlagAuditList()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.FlagAuditListCssLocator, How.CssSelector, "Text");
        }

        public string GetFlagAuditDetailByRowCol(int row, int col)
        {
            return
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.FlagAuditDetailsCssTemplate, row, col), How.CssSelector)
                    .Text;
        }

        public bool IsFlagDeletedInFlagAuditDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DeletedFlagRowInFlagAudtiDetailCssLocator,
                How.CssSelector);
        }

        public string GetFlagDetailDefenseRationale()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.FlagDetailsDefenseRationaleXpath, How.XPath)
                .Text;
        }

        public bool IsCVPFlagContainerPresent()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.FlagDetailsContainersCssLocator,
                       How.CssSelector) > 1;
        }

        public string GetFlagDetailLineNumberForCvpFlag()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagDetailsCVPFlagLineNumberCssLocator,
                How.CssSelector).Text;

        }

        public string GetFlagDetailEditForCvpFlag()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagDetailsCVPFlagEditCssLocator,
                How.CssSelector).Text;

        }

        public string GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(string claseq)
        {
            return Executor.GetSingleStringValue(Format(
                ClaimSqlScriptObjects.GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS,
                claseq.Split('-')[0], claseq.Split('-')[1]));
        }


        public bool IsDefenseCodePresent(string flagseq)
        {
            return !String.IsNullOrEmpty(Executor.GetSingleStringValue(Format(
                ClaimSqlScriptObjects.DefenseCodeByFlagSeq,
                flagseq)));

        }

        public string GetLineNoOfSelectedFlaggedLine()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.LineNoOfSelectedFlaggedLineXpath, How.XPath)
                .Text;
        }

        public string GetEditOfSelectedFlaggedLine()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.EditOfSelectedFlaggedLineCssLocator,
                    How.CssSelector)
                .Text;
        }

        public void ClickOnFlaggedLinesLineByLineNumberAndRow(int lineNumber, int row)
        {
            SiteDriver.FindElement(Format(ClaimActionPageObjects.FlaggedLineLineCssSelector, lineNumber, row), How.CssSelector).Click();
        }

        public string GetLabelByDivNumAndRowAndCol(int divNum, int row, int col) =>
            JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.LineDetailLabelByDivNumAndRowAndColCssSelector, divNum, row, col)).Text;


        public bool IsFlagDetailLabelPresentByLabelName(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(ClaimActionPageObjects.FlagDetailLabelCssLocatorTemplate,
                label));

        public string GetLabelByDivNumAndPreceedingSiblingOfLineDetail(int divNum, string preceedingSibling)
        {
            return JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.LineDetailsSugModifierByPreceedingSiblingCssSelector, divNum,
                preceedingSibling)).Text;
        }

        public string GetValueOfLineDetailsDataPointByDivNumAndTitle(int divNum, string title) =>

            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.LineDetailsDataPointValuesCssSelectorByDivNumAndTitle, divNum, title), How.CssSelector).GetAttribute("innerHTML");

        public string GetLabelOfLineDetailsByDivNumRowAndCol(int divNum, int row, int col) =>
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.DataPointsLabelForLineDetailsCssSelector, divNum, row, col),
                How.CssSelector).Text;

        public bool IsDataPointLabelOfLineDetailByLabelPresent(string label) =>
            JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.LineDetailDataPointLabelCssSelectorByLabel, label));

        public string GetDataPointValueOfLineDetailsByLabel(string label) => 
         JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.LineDetailDataPointValueCssSelectorByLabel,
                label)).Text;

        public string GetFlagDetailLabelByPreceedingLabel(string preceedingLabel) =>
            JavaScriptExecutor.FindElement(
                Format(ClaimActionPageObjects.FlagDetailLabelCssLocatorByPreceedingLabel, preceedingLabel)).Text;

        public void ClickDeleteButtonOfEditAllFlagsOnLineSection()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.DeleteButtonOnEditFormLineLevel,
                How.CssSelector).Click();
            Console.WriteLine("Click on Delete Button");
        }

        public void ClickRestoreButtonOfEditAllFlagsOnLineSection()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.RestoreButtonOnEditFormLineLevel,
                How.CssSelector).Click();
            Console.WriteLine("Click on Delete Button");
        }

        public string GetFlagDetailsDataPointValueByUlAndLiNumber(int ulNumber, int liNumber) =>
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagDetailDataPointValueCssLocator, ulNumber, liNumber),
                How.CssSelector).Text;

        public bool IsRuleIdLabelPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.RuleIdCssLocator, How.CssSelector);
        }

        public string GetFlagDetailsLabel(int col)
        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagDetailsLabelCssSelectorTemplate, col),
                How.CssSelector).Text;
        }

        public string GetFlagDetailsValueSecondRow(int col)
        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagDetails2ndRowValueCssSelecorTemplate, col),
                How.CssSelector).Text;
        }

        public string GetFlagDetailsValueFirstRow(int col)
        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagDetails1stRowValueCssSelecorTemplate, col),
                How.CssSelector).Text;
        }

        public bool IsFlagAuditHistoryEditNoteIconPresent(string username)
        {
            return
                SiteDriver.IsElementPresent(
                    Format(ClaimActionPageObjects.FlagAuditHistoryNoteEditIconXPath, username), How.XPath);
        }

        public void EditFlagNoteAndClientDisplay(string clientDisplay, string note, string username, bool Save)
        {
          SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagAuditHistoryNoteEditIconXPath, username), How.XPath).Click();
          if (clientDisplay == "Yes")
            {
                if (!SiteDriver.FindElement(ClaimActionPageObjects.VisibleToClientCheckBoxCssSelector,
                    How.CssSelector).GetAttribute("class").Contains("active"))
                {
                    SiteDriver.FindElement(ClaimActionPageObjects.VisibleToClientCheckBoxCssSelector,
                        How.CssSelector).Click();
                }
            }
          else
          {
              if (SiteDriver.FindElement(ClaimActionPageObjects.VisibleToClientCheckBoxCssSelector,
                  How.CssSelector).GetAttribute("class").Contains("active"))
              {
                  SiteDriver.FindElement(ClaimActionPageObjects.VisibleToClientCheckBoxCssSelector,
                      How.CssSelector).Click();
              }
            }
            ClearIFrame("Notes");
            FillIFrame("Notes", note);
            if (Save)
            {
                SiteDriver.FindElement(ClaimActionPageObjects.SaveClaimFlagAuditHistoryXPath, How.XPath)
                    .Click();
                WaitForWorkingAjaxMessage();
            }
            else
            {
                SiteDriver.FindElement(ClaimActionPageObjects.CancelClaimFlagAuditHistoryXPath, How.XPath)
                    .Click();
                SiteDriver.IsElementDisplayed(ClaimActionPageObjects.EditFlagNoteXPath, How.XPath).Equals(false);
            }
        }

        public void ClearIFrame(string label)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.IFrameNoteXPathTemplateByLabel, label), How.XPath));
            SiteDriver.SwitchFrameByXPath(Format(ClaimActionPageObjects.IFrameNoteXPathTemplateByLabel, label));
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(Keys.Backspace);
            SiteDriver.SwitchBackToMainFrame();
        }

        public void FillIFrame(string label, string text, int maxlength = 0)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.IFrameNoteXPathTemplateByLabel, label), How.XPath));
            SiteDriver.SwitchFrameByXPath(Format(ClaimActionPageObjects.IFrameNoteXPathTemplateByLabel, label));
            SendValuesOnTextArea(label, text);
        }
        public string GetClaimFlagAuditHistoryValueByLabel(string username,string label)
        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.ClaimFlagAuditHistoryValueByLabelXPath,username, label), How.XPath).Text;
        }
        #endregion

        public string GetFlagVlaueAddValueSection()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath,
                How.XPath).Text;
        }

        public bool IsFlagDropDownDisabledInAddFlag()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath,
                       How.XPath).GetAttribute("disabled") != null;
        }

        public int GetActiveClaimLineCount()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.ClaimLinesCssSelector, How.CssSelector);
        }

        //public bool IsClaimLineActive()
        //{
        //   return SiteDriver.FindElementsCount(NewClaimActionPageObjects.ClaimLinesCssSelector,How.CssSelector).Contains("is_active");
        //    return claimLines.Count()>0
        //            ? true
        //            : false;
        //}

        public string GetTriggerCode()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.TriggerCodeXPath, How.XPath).Text;
        }


        public string GetTrigDOS()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.TrigDOSXPath, How.XPath).Text;
        }

        public string GetClaimLineDetailsValueByLineNo(int lineNo, int row, int col)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.ClaimLineDetailsValueByLineNoXPathTemplate, lineNo, row,
                        col),
                    How.XPath).Text;

        }

        public string GetSourceByFlag(string flag)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SourceByFlagXPath, flag),
                    How.XPath).Text;

        }

        public void ClickOnClearOnWorkListPanel()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClearCssLocator, How.CssSelector);

        }

        public string GetPageHeader()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector).Text;
        }

        public string GetPageTitle()
        {
            return SiteDriver.Title;
        }

        public int GetFlagLineValue(int row)
        {
            var units =
                SiteDriver.FindElement(Format(ClaimActionPageObjects.FlagLineCssTemplate, row),
                    How.CssSelector).Text;
            return Int32.Parse(Regex.Replace(units, "[^0-9]+", string.Empty));
        }

        public double GetAllowedValueByLine(int line)
        {
            return
                Double.Parse(SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AllowedOnClaimDetailsByLineBadgeXPathTemplate, line),
                        How.XPath)
                    .Text.Replace('$', ' ')
                    .Trim());

        }

        public double GetBilledValueByLine(int line)
        {
            return
                Double.Parse(SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.BilledOnClaimDetailsByLineBadgeXPathTemplate, line),
                        How.XPath)
                    .Text.Replace('$', ' ')
                    .Trim());

        }

        public void ClickOnClaimDollarDetailsIcon()
        {
            JavaScriptExecutor.ClickJQuery(ClaimActionPageObjects.ClaimDollarDetailsIconCssLocator);
        }

        public double GetAdjPaidValueByLine(int line)
        {
            return
                Double.Parse(SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AdjPaidOnClaimsDetailsByLineBadgeXPathTemplate, line),
                        How.XPath)
                    .Text.Replace('$', ' ')
                    .Trim());

        }

        public string GetClaimDollarDetailsByLineWithLabel(string label, int line = 1)
        {
            return
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.ClaimDollarDetailsByLineWithLabelXPathTemplate, line,
                            label), How.XPath)
                    .Text;

        }

        public int GetUnitValueOnFlag(int row)
        {
            var units =
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.UnitsOnFlagLineCssTemplate, row), How.CssSelector).Text;
            return Int32.Parse(Regex.Replace(units, "[^0-9]+", string.Empty));
        }

        public Double GetLineSavingsValue(string flag)
        {
            return
                Double.Parse(
                    SiteDriver.FindElement(Format(ClaimActionPageObjects.SavingsXPath, flag),
                        How.XPath).Text.Replace('$', ' ').Trim());
        }

        public Double GetLineSugPaidValue(string flag)
        {
            return
                Double.Parse(
                    SiteDriver.FindElement(Format(ClaimActionPageObjects.SugPaidAmountXPath, flag),
                        How.XPath).Text.Replace('$', ' ').Trim());
        }

        public Double GetSugPaid()
        {
            return Double.Parse(
                SiteDriver.FindElement(ClaimActionPageObjects.SugPaidInputFieldCssLocator,
                    How.CssSelector).GetAttribute("value").Replace('$', ' ').Trim());
        }

        /// <summary>
        /// Navigate to appeal creator
        /// </summary>
        /// <returns></returns>
        public AppealCreatorPage ClickOnCreateAppealIcon()
        {
            var appealCreatorPage = Navigator.Navigate<AppealCreatorPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.AddAppealIconCssSelector, How.CssSelector).Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on 1st enabled + appeal icon");
            });
            return new AppealCreatorPage(Navigator, appealCreatorPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void RemoveLock()
        {
            if (!IsClaimLocked()) return;
            Console.WriteLine("Claim is Locked!");
            var lockTitle = GetLockIConTooltip();
            Console.WriteLine(lockTitle);
            GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(GetClaimSequence(), EnvironmentManager.Username);
            ClickWorkListIcon();
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FindXPath, How.XPath); //.ProxyWebElement
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
            ClickWorkListIcon();
            if (!IsClaimLocked()) return;
            lockTitle = GetLockIConTooltip();
            Console.WriteLine(lockTitle);
            Refresh();
            HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        public void ClickAddIconButton()
        {
            RemoveLock();
            SiteDriver.FindElement(ClaimActionPageObjects.AddIconCssLocator, How.CssSelector).Click();
            ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Clicked on Add Icon Button");
        }

        public bool IsAddFlagIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddIconCssLocator, How.CssSelector);
        }

        public void SelectClaimLineToAddFlag(string index = "1")
        {
            var element = SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FirstClaimLineToAddFlagCssSelectorTemplate, index),
                How.CssSelector);
            if (!element.GetAttribute("class").Contains("is_active"))
                element.Click();
            Console.WriteLine("Clicked Claim Line {0} To Add Flag.", index);
        }

        public string GetValueOfClaimLineByRowAndLabel(int row, string label)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.ClaimLineValueRowAndLabelXPathTemplate, row, label),
                    How.XPath).Text;
        }

        public NewPopupCodePage ClickOnRevCodeOfClaimLineByRow(int row, string revCode)
        {
            NewPopupCodePage.AssignPageTitle(Format("Revenue Code - {0}", revCode));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.ClaimLineValueRowAndLabelXPathTemplate, row, "R:"),
                    How.XPath).Click();
                Console.Out.WriteLine("Clicked on Rev Code<{0}> for {1} ", row, revCode);

                SiteDriver.SwitchWindowByTitle(revCode);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void SelectTriggerClaimLineOnAddFlag(string claSeq, int lineNo = 1)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.TriggerClaimLineOnAddFlagCssTemplate,
                claSeq, lineNo));
            Console.WriteLine("Select Trigger Claim Line having claimseq<{0}> and line no <{1}>", claSeq, lineNo);
        }

        public void ClickCheckBox()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.CheckBoxCssLocator, How.CssSelector);
            Console.WriteLine("Check box checked");
        }

        public bool IsFlagLinePresentByFlag(string flag)
        {
            return
                JavaScriptExecutor.IsElementPresent(Format(ClaimActionPageObjects.FlagLineDivCssTemplate,
                    flag));
        }

        public bool IsAddFlagPanelPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddFlagDivId, How.CssSelector);
        }

        public bool IsFlagDetailDivDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.FlagDetailsDivCssLocator, How.CssSelector);
        }

        public ClaimActionPage ClickOnApproveButton()
        {
            RemoveLock();
            var newClaimPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.ApproveCssLocator, How.CssSelector).Click();
                WaitForWorkingAjaxMessageForBothDisplayAndHide();
                SiteDriver.WaitToLoadNew(4000);
                Console.WriteLine("Clicked on Approve button");
            });
            if (!IsPageErrorPopupModalPresent()) return new ClaimActionPage(Navigator, newClaimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            SiteDriver.CaptureScreenShot("Error popup when approving claim");
            
            ClosePageError();
            GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(GetClaimSequence(), EnvironmentManager.Username);
            Refresh();

            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
            HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            newClaimPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.ApproveCssLocator,How.CssSelector).Click();
                WaitForWorkingAjaxMessageForBothDisplayAndHide();
                Console.WriteLine("Clicked on Approve button");
            });

            return new ClaimActionPage(Navigator, newClaimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public ClaimSearchPage ClickOnApproveButtonAndNavigateToNewClaimSearchPage(string claseq)
        {
            RemoveLock();
            var newClaimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.ApproveCssLocator, How.CssSelector).Click();
                WaitForWorkingAjaxMessage();
                if (IsPageErrorPopupModalPresent())
                {
                    ClosePageError();
                    ClickWorkListIcon();
                    ClickSearchIcon();
                    SearchByClaimSequence(claseq);
                    ClickWorkListIcon();
                    SiteDriver.FindElement(ClaimActionPageObjects.ApproveCssLocator, How.CssSelector).Click();
                    WaitForWorkingAjaxMessage();


                }

                SiteDriver.WaitForCondition(IsBlankPagePresent);
                Console.WriteLine("Clicked on Approve button");
            });

            return new ClaimSearchPage(Navigator, newClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public ClaimProcessingHistoryPage ClickOnClaimHistoryButtonAndSwitchToClaimProcessingHistoryPage()
        {
            ClickMoreOption();

            try
            {
                SiteDriver.FindElement(ClaimActionPageObjects.ClaimProcessingHistoryLinkXpath, How.XPath)
                    .Click();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimProcessingHistoryPage.GetStringValue()));
                SiteDriver.IsElementPresent(ClaimProcessingHistoryPageObjects.ProcessingHistoryLabeByCss,
                    How.CssSelector);
                Console.WriteLine("Switch To Claim Processing History Page");
                SiteDriver.WaitToLoadNew(1000);
                return new ClaimProcessingHistoryPage(Navigator, new ClaimProcessingHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return null;
        }


        public ClaimActionPage ClickOnBrowserBack()
        {

            var newClaimPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.Back();
                Console.WriteLine("Clicked on Browser Back button");
            });
            return new ClaimActionPage(Navigator, newClaimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsSwitchIconNotDisplayedByLineNumberAndRowNum(int lineNum = 1, int rowNum = 1)
        {
            var isSwtichIconDisplayed = SiteDriver.FindElementAndGetAttribute(
                Format(ClaimActionPageObjects.SwitchIconByLineNumberAndRowNum, lineNum, rowNum), How.CssSelector,"style").Contains("display: none");
            return isSwtichIconDisplayed;
        }

        public bool IsSwitchIconDisabledByLineNumAndRowNum(int lineNum = 1, int rowNum = 1) => SiteDriver.FindElementAndGetAttribute(
            Format(ClaimActionPageObjects.SwitchIconByLineNumberAndRowNum, lineNum, rowNum), How.CssSelector, "class").Contains("is_disabled");

        public bool IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(
            string currentLineNo, string triggerClaseq, string triggerLineNo)
        {
            return SiteDriver.IsElementPresent(
                Format(
                    ClaimActionPageObjects.SwitchIconByCurrentLineNoAndTriggerClaimAndTriggerLineNoXPathLocator,
                    currentLineNo, triggerClaseq, triggerLineNo), How.XPath);
        }

        public bool IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(
            string currentLineNo, string triggerClaseq, string triggerLineNo)
        {
            return SiteDriver.IsElementPresent(
                Format(
                    ClaimActionPageObjects
                        .DisabledSwitchIconByCurrentLineNoAndTriggerClaimAndTriggerLineNoXPathLocator,
                    currentLineNo, triggerClaseq, triggerLineNo), How.XPath);
        }


        public bool IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(string currentLineNo,
            string flag, string triggerLineNo)
        {
            return SiteDriver.IsElementPresent(
                Format(
                    ClaimActionPageObjects.SwitchIconByCurrentLineNoAndFlagAndTriggerLineNoXPathLocator,
                    currentLineNo, flag, triggerLineNo), How.XPath);
        }

        public bool IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndFlagAndTriggerLineNo(
            string currentLineNo, string flag, string triggerLineNo)
        {
            return SiteDriver.IsElementPresent(
                Format(
                    ClaimActionPageObjects.DisabledSwitchIconByCurrentLineNoAndFlagAndTriggerLineNoXPathLocator,
                    currentLineNo, flag, triggerLineNo), How.XPath);
        }

        public void ClickOnSwitchIconByCurrentLineNoAndTriggerClaSeqAndTriggerLineNo(string currentLineNo,
            string triggerClaseq, string triggerLineNo)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(
                    ClaimActionPageObjects.SwitchIconByCurrentLineNoAndTriggerClaimAndTriggerLineNoXPathLocator,
                    currentLineNo, triggerClaseq, triggerLineNo), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnSwitchIconByCurrentLineNoAndFlagAndTriggerLineNo(string currentLineNo, string flag,
            string triggerLineNo)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(
                    ClaimActionPageObjects.SwitchIconByCurrentLineNoAndFlagAndTriggerLineNoXPathLocator,
                    currentLineNo, flag, triggerLineNo), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public bool IsDeleteIconPresentOnFlaggedLinesRowsByRow(int row = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DeleteIconOnFlagLinesByRowCssLocator, row), How.CssSelector);
        }

        public void ClickOnDeleteIconOnFlaggedLinesRowsByRow(string claseq, int row = 1,
            bool isClickOnQaFailIcon = false)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.DeleteIconOnFlagLinesByRowCssLocator, row), How.CssSelector);
            WaitForWorkingAjaxMessage();
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                ClickWorkListIcon();
                ClickSearchIcon();
                GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(claseq, EnvironmentManager.Username);
                SearchByClaimSequence(claseq);
                ClickWorkListIcon();
                if (isClickOnQaFailIcon)
                    ClickClaimActionQaFail();
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.DeleteIconOnFlagLinesByRowCssLocator, row),
                    How.CssSelector);
                WaitForWorkingAjaxMessage();

            }

            SiteDriver.WaitToLoadNew(1000);
            if (IsClaimLocked())
            {
                Console.WriteLine("Claim is Locked!");
                var lockTitle = GetLockIConTooltip();
                Console.WriteLine(lockTitle);
            }

            CaptureScreenShot("Need_To_Find_Failure_Issue");

        }

        public void ClickOnDeleteIconOnFlagByLineNoAndRow(string claseq, int flaggedLineRow = 1, int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.DeleteIconOnFlagByLineNoAndRowCssTemplate, flaggedLineRow, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            if (!IsPageErrorPopupModalPresent()) return;
            ClosePageError();
            GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(GetClaimSequence(), EnvironmentManager.Username);
            if (IsClaimLocked())
            {
                ClickWorkListIcon();
                ClickSearchIcon();
                SearchByClaimSequence(claseq);
                ClickWorkListIcon();
            }
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.DeleteIconOnFlagByLineNoAndRowCssTemplate, flaggedLineRow, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitToLoadNew(1500);
        }

        public void ClickOnRestoreIconOnFlagByLineNoAndRow(string claseq, int flaggedLineRow = 1, int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.RestoreIconOnFlagLineNoAndRowCssTemplate,flaggedLineRow, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            if (!IsPageErrorPopupModalPresent()) return;
            ClosePageError();
            GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(GetClaimSequence(), EnvironmentManager.Username);
            if (IsClaimLocked())
            {
                ClickWorkListIcon();
                ClickSearchIcon();
                SearchByClaimSequence(claseq);
                ClickWorkListIcon();
            }
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.RestoreIconOnFlagLineNoAndRowCssTemplate, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitToLoadNew(1000);

        }

        public void ClickOnRestoreIconOnFlaggedLinesRowsByRow(string claseq, int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.RestoreIconOnFlaggedLinesByRowCssLocator, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            if (!IsPageErrorPopupModalPresent()) return;
            ClosePageError();
            GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(GetClaimSequence(), EnvironmentManager.Username);
            if (IsClaimLocked())
            {
                ClickWorkListIcon();
                ClickSearchIcon();
                SearchByClaimSequence(claseq);
                ClickWorkListIcon();
            }
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.RestoreIconOnFlaggedLinesByRowCssLocator, row),
                How.CssSelector);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitToLoadNew(1000);

        }

        /// <summary>
        /// Return true if proc desc is present in flagged lines first div.
        /// </summary>
        /// <returns></returns>
        public bool IsProcDescPresentInFlaggedLines()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProcDescFlaggedLinesCssLocator,
                How.CssSelector);
        }

        public string GetEmptyFlagLineMessage()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.EmptyFlagMessageCssLocator, How.CssSelector).Text;
        }

        public bool IsEmptyFlagLineMessageAvailable()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EmptyFlagMessageCssLocator, How.CssSelector);
        }

        public string GetProcDescriptionTextOfFlaggedLines()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProcDescFlaggedLinesCssLocator, How.CssSelector).Text;
        }

        public string GetClaimStatus()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimStatusDivInClaimActionCssLocator, How.CssSelector).Text;
        }

        public string GetBatchIdInClaimDetails()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.BatchIdDivInClaimActionXPath, How.XPath).Text;
        }

        public string GetProviderSpecialty()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderSpecialtyValueCssSelector, How.CssSelector).Text;
        }

        public string GetProviderName()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderNameValueCssSelector, How.CssSelector).Text;
        }

        public string GetProviderZip()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderZipValueCssSelector, How.CssSelector).Text;
        }

        public string GetProviderTin()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderTinValueCssSelector, How.CssSelector).Text;
        }

        /// <summary>
        /// Get title attribute value of Pos of flagged lines.
        /// </summary>
        /// <returns></returns>
        public string GetTitleAttributeValueOfPosOfFlaggedLines()
        {
            return SiteDriver
                .FindElementAndGetAttribute(ClaimActionPageObjects.PosFlaggedLinesCssLocator, How.CssSelector, "title");
        }

        public bool IsSpecialtyOnFlaggedLinesByValuePresent(string specialty)
        {
            var val = specialty.Split('-').Select(x => x.Trim()).ToList();
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.SpecialtyOnFlaggedLinesByValueXPathLocator, val[0], val[1]),
                How.XPath);
        }

        public bool IsTriggerClaimPresentInFirstFlagLine()
        {
            return !string.IsNullOrEmpty(SiteDriver
                .FindElement(ClaimActionPageObjects.TriggerClaimCssSelector, How.CssSelector).Text);
        }

        public bool IsTriggerClaimsPresentInFirstLineDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.TriggerClaimsXPath, How.CssSelector);
        }

        public void ClickOnFirstFlagLineOfFlaggedLinesDiv()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.FirstFlagDetailsCssSelector, How.CssSelector).Click();
            Console.WriteLine("Flag line is clicked");
        }

        public void ClickOnFlagDivInFlaggedLinesByRow(int row)
        {
            var catchElementCSS =
                Format(ClaimActionPageObjects.SelectFlagCodeDivInFlaggedLinesCssTemplate, row);
            SiteDriver.FindElementAndClickOnCheckBox(
                Format(ClaimActionPageObjects.SelectFlagCodeDivInFlaggedLinesCssTemplate, row),
                How.XPath);
        }

        public void ClickOnFirstDxCodeT()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.FirstDxCodeCssLocator, How.CssSelector).Click();
            Console.WriteLine("Clicked on first dx code {0}", SiteDriver
                .FindElement(ClaimActionPageObjects.FirstDxCodeCssLocator, How.CssSelector).Text);
        }

        /// <summary>
        /// Click on DxCode and get Description
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> ClickOnDxCodeAndGetDescription(int row)
        {
            IDictionary<string, string> dxCode = new Dictionary<string, string>();
            var element =
                SiteDriver.FindElement(Format(ClaimActionPageObjects.DxCodeByRowCssTemplate, row),
                    How.CssSelector);
            string title = element.Text;
            string originalWindowHandles = SiteDriver.CurrentWindowHandle;
            element.Click();
            //element.Escape();
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(string.Concat("Dx Code - ", title)));
            NewPopupCodePage.AssignPageTitle(string.Concat("Dx Code - ", title));
            var daignosisCode = new NewPopupCodePage(Navigator, new NewPopupCodePageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            dxCode.Add(daignosisCode.GetTextValueinLiTag(1),
                Regex.Split(daignosisCode.GetTextValueinLiTag(3), "\r\n")[1]);
            SiteDriver.CloseWindowAndSwitchTo(originalWindowHandles);
            return dxCode;
        }

        public string GetPatientHistoryPatientSeq()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PatientHistoryPatientSeqCssLocator, How.CssSelector).Text;
        }

        public string GetTitleAttributeValueOfPosOfFlagDetails()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PosFlagDetailsCssLocator, How.CssSelector).Text;
        }

        public string GetTitleAttributeValueOfPosOfLineDetails()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PosLineDetailsCssLocator, How.CssSelector).Text;
        }


        public bool IsPosPresentInFlagDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.PosFlagDetailsCssLocator, How.CssSelector);
        }

        public bool IsPosPresentInLineDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.PosLineDetailsCssLocator, How.CssSelector);
        }

        /// <summary>
        /// Get title attribute  value of Pos of claim lines
        /// </summary>
        /// <returns></returns>
        public string GetTitleAttributeValueOfPosOfClaimLines()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PosClaimLinesCssSelector, How.CssSelector).GetAttribute("title");
        }

        /// <summary>
        /// Return true if proc desc is present in claim lines first div.
        /// </summary>
        /// <returns></returns>
        public bool IsProcDescPresentInClaimLinesDiv()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProcDescClaimLinesDivCssLocator,
                How.CssSelector);
        }

        public string GetProcDescriptionTextOfClaimLinesDiv()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProcDescClaimLinesDivCssLocator, How.CssSelector)
                .Text;
        }

        /// <summary>
        /// Return true if proc desc is present in claim dollar details first div.
        /// </summary>
        /// <returns></returns>
        public bool IsProcDescPresentInClaimDollarDetailsDiv()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProcDescClaimDollarDetailsDivCssLocator,
                How.CssSelector);
        }

        public string GetProcDescriptionTextOfClaimDollarDetailsDiv()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ProcDescClaimDollarDetailsDivCssLocator,
                How.CssSelector).Text;
        }

        /// <summary>
        /// Return true if proc desc is present in claim lines first div.
        /// </summary>
        /// <returns></returns>
        public bool IsFlagDescPresentInFlaggedLinesDiv()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProcDescFlaggedLineOfFlaggedLinesDivXPath,
                How.XPath);
        }

        public string GetFlagDescriptionText()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProcDescFlaggedLineOfFlaggedLinesDivXPath, How.XPath)
                .Text;
        }

        public ClaimActionPage SwitchToOriginalWindow()
        {
            _claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                StringFormatter.PrintMessage("Switch back to New Claim Action Page");
            });
            return new ClaimActionPage(Navigator, _claimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealLetterPage ClickOnAppealLetter(string appealSequence)
        {
            var appealLetter = Navigator.Navigate<AppealLetterPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealLetterXPathTemplate, appealSequence), How.XPath)
                    .Click();
                Console.WriteLine("Clicked on Appeal Letter");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            });
            return new AppealLetterPage(Navigator, appealLetter, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool AppealLetterIsPresent(string appealSequence)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.AppealLetterXPathTemplate, appealSequence), How.XPath);
        }

        public List<string> GetListOfAppealSequenceInRows()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.AppealSequenceListXpath, How.XPath, "Text");
        }
        public List<string> GetListOfAppealStatusInRows()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.AppealStatusListXpath, How.XPath, "Text");
        }

        public string GetAppealStatus(string appealSequence)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.AppealSequenceStatusXPathTemplate, appealSequence),
                    How.XPath).Text;
        }

        public string GetAppealStatusByRow(int row)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.AppealStatusByRowOfAppealXPathTemplate, row), How.XPath)
                .Text;
        }

        public string GetAppealTypeByRow(int row=1)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.AppealTypeByRowOfAppealXPathTemplate, row), How.XPath)
                .Text;
        }

        public void WaitForWorkListControlToLoad()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver
                    .FindElement(ClaimActionPageObjects.WorkListControlCssLocator, How.CssSelector).GetAttribute("class")
                    .Equals("open", StringComparison.OrdinalIgnoreCase));
        }

        public string GetClaimSequence()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSeqCssLocator, How.XPath).Text;
        }

        public int GetAppealCountShownInAppealButton()
        {
            return Convert.ToInt32(SiteDriver
                .FindElement(ClaimActionPageObjects.ViewAppealIconWithBadgeCssLocator, How.CssSelector).Text);
        }

        public int GetAppealSequenceCount()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.AppealSequencesXPath, How.XPath);
        }

        public AppealActionPage ClickOnAppealSequence(string appealSequence)
        {
            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceCssTemplate, appealSequence),
                        How.CssSelector)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
        }
        public AppealSummaryPage ClickOnAppealSequenceToSwitchToNewTab(string appealSequence)
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceCssTemplate, appealSequence),
                        How.CssSelector)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeals/"+appealSequence.Split('-')[0]));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
               
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealActionPage ClickOnAppealSequenceAndOpenAppealActionInNewTab(string appealSequence)
        {
            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceCssTemplate, appealSequence),
                        How.CssSelector)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeals/" + appealSequence.Split('-')[0]));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });
            return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealSummaryPage ClickOnAppealSequenceToOpenAppealSummarypopup(string appealSequence)
        {

            var appealSummaryPopup = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceCssTemplate, appealSequence),
                        How.CssSelector)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + appealSequence);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindowByUrl("appeal_summary");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealSummaryPage(Navigator, appealSummaryPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealSummaryPage ClickOnAppealSequenceToOpenAppealSummarypopupByAppealType(int row=1 )
        {

            var appealSummaryPopup = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceXpathByRow, row),
                        How.XPath)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + GetAppealSequenceByAppealType(row));
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindowByUrl($"{GetAppealSequenceByAppealType(row)}/appeal_summary");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealSummaryPage(Navigator, appealSummaryPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public AppealActionPage ClickOnAppealSequenceByRow(int row=1)
        {
            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.AppealSequenceXpathByRow, row),
                        How.XPath)
                    .Click();
                Console.WriteLine("Clicked on Appeal Sequence: " + GetAppealSequenceByAppealType(row));
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindowByUrl("appeal_action");
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
        }
        public string GetAppealSequence()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.AppealSequenceXPath, How.XPath).Text;
        }

        public string GetAppealSequenceByAppealType(int row =1)
        {
            return SiteDriver.FindElement(Format(ClaimActionPageObjects.AppealSequenceXpathByRow,row), How.XPath).Text;
        }

        /// <summary>
        /// Get no dx code present mssage
        /// </summary>
        /// <returns></returns>
        public string GetNoDxCodePresentMesssage()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.ViewDxCodesIconCssLocator, How.CssSelector).Click();
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DxCodeNotPresentDivCssLocator, How.CssSelector).Text;
        }

        public ClaimActionPage ClickOnTriggerClaimLink(string trigClaim, out string newClaimActionPageWindowHandle)
        {
            newClaimActionPageWindowHandle = _originalWindow;
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.TrigClaimLinkXPathTemplate, trigClaim), How.XPath).Click();
                Console.WriteLine("Clicked on Trigger Claim Sequence: " + trigClaim);
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool NewClaimActionPageIsWindowPopup(string claimSeq)
        {
            return SiteDriver.Url.Contains(claimSeq);
        }

        public void CloseAnyPopupIfExist(string windowHandle)
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!windowHandle.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                }
            }
        }

        public NewPopupCodePage ClickOnClaimLinesProcCode(out string popupname)
        {
            string windowName = SiteDriver
                .FindElement(ClaimActionPageObjects.ProcCodePopupLinkCssLocator, How.CssSelector).Text;
            popupname = windowName;
            NewPopupCodePage.AssignPageTitle(Format("CPT Code - {0}", windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ProcCodePopupLinkCssLocator, How.CssSelector).Click();
                Console.Out.WriteLine("Clicked on Proc Code popup link");

                SiteDriver.SwitchWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage ClickOnClaimLineProcCode(string codeType, string procode)
        {
            string windowName = procode;
            NewPopupCodePage.AssignPageTitle(Format("{0} - {1}", codeType, windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.ClaimLineProcCodePopupLinkXPathTemaplate, procode),
                    How.XPath);
                //SiteDriver.SwitchWindow(windowName);
                SiteDriver.SwitchWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public IList<string> GetListOfProCodes()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ProcCodePopupLinkCssLocator, How.CssSelector, "Text");
        }

        public string GetProcCodeForRow(int row)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.ProcCodeFlaggedLinesCssLocator, row),
                    How.CssSelector).Text;
        }

        public IList<string> GetClaimTypeOptions()
        {
            var element = SiteDriver.FindElement(ClaimActionPageObjects.ClaimTypeDropDownXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimTypeListOptionsXPath, How.XPath, "Text");
        }

        public void Wait()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void WaitForIe(int milliseconds)
        {
            SiteDriver.WaitForIe(milliseconds);
        }

        public IList<string> GetFLagOptions()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FlagDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(ClaimActionPageObjects.FlagDropDownXPath, How.XPath);
            IList<string> flagOptions =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.FLagListOptionsXPath, How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(ClaimActionPageObjects.FlagDropDownXPath, How.XPath);
            return flagOptions;
        }

        public IList<string> GetBatchOptions()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.BatchIdDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(ClaimActionPageObjects.BatchIdDropDownXPath, How.XPath);
            IList<string> batchOptions =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.BatchListOptionsXPath, How.XPath, "Text");
            // JavaScriptExecutor.ExecuteMouseOut(NewClaimActionPageObjects.BatchIdDropDownXPath, How.XPath);
            return batchOptions;
        }

        public IList<String> GetActiveProductListForClient(string client)
        {
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.ActiveProductList, client));
            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            return newList;
        }

        public IList<string> GetEditReasonCodeOptions()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.EditReasonCodeDropDownCssLocator,
                How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(ClaimActionPageObjects.EditReasonCodeDropDownCssLocator,
                How.CssSelector);
            IList<string> reasonCodeOptions =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.EditReasonCodeOptionsCssLocator, How.CssSelector,
                    "Text");
            JavaScriptExecutor.ExecuteMouseOut(ClaimActionPageObjects.EditReasonCodeDropDownCssLocator, How.CssSelector);
            return reasonCodeOptions;
        }

        public NewPopupCodePage ClickOnTriggerCodeToOpenTriggerDescriptionPopup(out string popupname)
        {
            string windowName = SiteDriver
                .FindElement(ClaimActionPageObjects.TriggerCodeXPath, How.XPath).Text;
            popupname = windowName;
            NewPopupCodePage.AssignPageTitle(Format("CPT Code - {0}", windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.TriggerCodeXPath, How.XPath).Click();

                SiteDriver.SwitchWindowByTitle(windowName);

            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage ClickOnRevenueCodeToOpenRevCodeDescriptionPopup(out string popupname)
        {
            string windowName = SiteDriver
                .FindElement(ClaimActionPageObjects.RevenueCodeCssLocator, How.CssSelector).Text;
            popupname = windowName;
            NewPopupCodePage.AssignPageTitle(Format("Revenue Code - {0}", windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.RevenueCodeCssLocator, How.CssSelector).Click();

                SiteDriver.SwitchWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Return true if edit flags area is present
        /// </summary>
        /// <returns></returns>
        public bool IsEditFlagAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditAllFlagClaimSectionCssLocator,
                How.CssSelector);
        }

        public bool IsFlagLevelEditFlagAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditFlagClaimSectionCssLocator,
                How.CssSelector);
        }

        /// <summary>
        /// Returns if delete all flag icon is disabled or enabled
        /// </summary>
        /// <returns></returns>
        public bool IsEditFlagsIconEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditFlagsIconCssLocator, How.CssSelector);
        }

        public bool IsDeleteAllFlagsIconDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DeleteAllFlagsCssLocatorDisabled, How.CssSelector);
        }

        public bool IsEditFlagNoteTextAreaEnabled()
        {
            bool isEditable = false;
            SiteDriver.SwitchFrameByXPath(ClaimActionPageObjects.EditFlagNoteIframeXPath);
            if (SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable") == "true")
            {
                isEditable = true;
            }

            SiteDriver.SwitchBackToMainFrame();
            return isEditable;

        }

        public void SetNoteInEditFlagNoteField(string note)
        {
            SiteDriver.SwitchFrameByXPath(ClaimActionPageObjects.EditFlagNoteIframeXPath);
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
        }
        /// <summary>
        /// Returns if approve/next button is disabled
        /// </summary>
        /// <returns></returns>
        public bool IsApproveNextButtonDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledApproveNextCssLocator,
                How.CssSelector);
        }

        /// <summary>
        /// Returns if approve button is disabled
        /// </summary>
        /// <returns></returns>
        public bool IsApproveButtonDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledApproveCssLocator, How.CssSelector);
        }

        /// <summary>
        /// Returns if approve button is enabled
        /// </summary>
        /// <returns></returns>
        public bool IsApproveButtonEnabled()
        {
            return SiteDriver.IsElementEnabled(ClaimActionPageObjects.ApproveClaimIconCssLocator, How.CssSelector);
        }

        public bool IsTransferApproveButtonEnabled()
        {
            return SiteDriver.IsElementEnabled(ClaimActionPageObjects.TransferApproveCssSelector, How.CssSelector);
        }

        /// <summary>
        /// Returns if claim has lock icon at the top
        /// </summary>
        /// <returns></returns>
        public bool IsClaimLocked()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.LockIconCssSelector, How.CssSelector);
        }

        /// <summary>
        /// Returns if next button is disabled
        /// </summary>
        /// <returns></returns>
        public bool IsNextButtonDisabled()
        {

            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledNextCssLocator, How.CssSelector);

        }

        public bool IsRestoreButtonPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.RestoreFlagsIconCssLocator, How.CssSelector);
        }

       

        public void WaitForInvalidSugCodeModalPopup()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.PageErrorModalPopupId, How.Id));
        }

        public bool IsProviderSearchNotesIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProviderSearchNotesIconCssSelector,
                How.CssSelector);
        }

        

        /// <summary>
        /// Return if title attribute is present in the tag that controls the ellipsis appearance
        /// </summary>
        /// <returns></returns>

        public bool IsEllipsisPresentForHciRun()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciRunFieldWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForHciVoid()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciVoidFieldWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForHdrPayorName()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HdrPayornameFieldWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForHdrPhone()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HdrPhoneFieldWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForHdrProvType()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HdrProvTypeFieldWithEllipsisXPath, How.XPath);
        }

        public bool IsEllipsisPresentForHdrSpecialty()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HdrSpecFieldWithEllipsisXPath, How.XPath);
        }

        public bool IsClaimAuditAddedForDocumentUpload(string claseq)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimAuditDocumentUploaded,
                       claseq.Split('-')[0], claseq.Split('-')[1])).Count() > 0
                ? true
                : false;
        }

        public bool IsClaimAuditAddedForDocumentDelete(string claseq)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimAuditDocumentDeleted,
                       claseq.Split('-')[0], claseq.Split('-')[1])).Count() > 0
                ? true
                : false;
        }

        public bool IsClaimAuditAddedForDocumentDownload(string claseq)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimAuditDocumentDownload,
                       claseq.Split('-')[0], claseq.Split('-')[1])).Count() > 0
                ? true
                : false;
        }

        public bool IsEllipsisPresentForFlaggedLinesFlagDescription()
        {
            return SiteDriver.IsElementPresent(
                ClaimActionPageObjects.FlaggedLinesFlagDescriptionWithEllipsisCssSelector, How.CssSelector);
        }

        public bool IsEllipsisPresentForFlaggedLinesProcDescription()
        {
            return SiteDriver.IsElementPresent(
                ClaimActionPageObjects.FlaggedLinesProcDescriptionWithEllipsisCssSelector, How.CssSelector);
        }

        public bool IsEllipsisPresentForSpecialty()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SpecialtyWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForProviderName()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProviderNameWithEllipsisCssSelector,
                How.CssSelector);
        }

        public bool IsEllipsisPresentForClaimLinesProcDescription()
        {
            return SiteDriver.IsElementPresent(
                ClaimActionPageObjects.ClaimLinesProcDEscriptionWithEllipsisCssSelector, How.CssSelector);
        }

        public bool IsEllipsisPresentForClaimDollarDetailsProcDescription()
        {
            return SiteDriver.IsElementPresent(
                ClaimActionPageObjects.ClaimDollarDetailsProcDescriptionWithEllipsisCssLocator, How.CssSelector);
        }

        

        /// <summary>
        /// Return line adjusted savings amount
        /// </summary>
        /// <returns></returns>
        public decimal GetLineAdjustedSavings(string flag)
        {
            decimal lineAdjustedPaidAmount =
                Convert.ToDecimal(
                    SiteDriver.FindElement(Format(ClaimActionPageObjects.SavingsXPath, flag),
                        How.XPath).Text.Replace('$', ' ').Trim()) +
                Convert.ToDecimal(
                    SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.SugPaidAmountXPath, flag), How.XPath).Text.Replace(
                        '$', ' ').Trim());
            return lineAdjustedPaidAmount;
        }

        #region MPR / PS EDIT

        /// <summary>
        /// Get trigger line # of MPR edit is added
        /// </summary>
        /// <returns></returns>
        public string GetTriggerLineForEditMpr()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.TriggerLineForEditMprXPath, How.XPath)
                .Text;
        }

        public string GetSwitchedFromTriggerLine(string flagName)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.SwitchedFromTriggerLineXPath, flagName),
                    How.XPath).Text;
        }

        public string GetSwitchedFromDifferentClaimTriggerLine(string flagName, string claimSeq)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.SwitchedFromDifferentClaimTriggerLineXPath, flagName,
                        claimSeq), How.XPath).Text;
        }

        public string GetSwitchedToTriggerLine(string flagName)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.SwitchedToTriggerLineXPath, flagName),
                    How.XPath).Text;
        }

        public string GetSwitchedToClaimSequence(string claimSeq)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.SwitchedToClaimSequenceXPath, claimSeq),
                    How.XPath).Text;
        }

        public string GetSwitchedFromClaimSequence(string claimSeq)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.SwitchedFromClaimSequenceXPath, claimSeq), How.XPath).Text;
        }

        public string GetEditPsColor(string lineNumber)
        {
            return
                SiteDriver.FindElement(Format(ClaimActionPageObjects.PsEditXPathTemplate, lineNumber),
                    How.XPath).GetCssValue("color");

        }

        public IList<string> GetStatusOfLineAction(string lineNumber)
        {
            return
                SiteDriver.FindElementAndGetClassAttribute(
                    Format(ClaimActionPageObjects.LineActionXPathTemplate, lineNumber), How.XPath);
        }


        public bool IsMprEditDeleted()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagLineWithMPrEditXPath, How.XPath)
                .GetAttribute("class").Contains("deleted");
        }

        public bool IsPsEditDeleted(string lineNumber)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.FlagLineWithPsEditXPathTemplate, lineNumber), How.XPath)
                .GetAttribute("class").Contains("deleted");
        }

        public void ClickMprEditDelOrRestoreAction()
        {
            RemoveLock();
            SiteDriver.FindElement(ClaimActionPageObjects.MprEditDelOrRestoreActionXPath, How.XPath).Click();
            WaitForWorkingAjaxMessage();
        }

        #endregion

        /// <summary>
        /// Return true if successful else return false
        /// </summary>
        /// <returns></returns>
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
        

        /// <summary>
        /// Return true if successful else return false
        /// </summary>
        /// <returns></returns>
        //public bool HandleAutomaticallyOpenedLogicManagerPopup()
        //{

        //    bool isHandled = false;
        //    try
        //    {
        //        SiteDriver.WaitForCondition(() =>
        //        {
        //            if (SiteDriver.WindowHandles.Count > 1)
        //                isHandled = SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue());
        //            return isHandled;
        //        });
        //        var logicManager = new LogicManagementPage(Navigator, new LogicManagementPageObjects());
        //        if (SiteDriver.CloseWindowAndSwitchTo(_originalWindow))
        //            Console.WriteLine("Automatically opened Logic Manager page closed");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Catch exception and close window" + ex.Message);
        //        SiteDriver.SwitchWindow(_originalWindow);
        //    }

        //    return isHandled;

        //}
        
        /// <summary>
        /// Return true if successful else return false
        /// </summary>
        /// <returns></returns>
        public bool HandleAutomaticallyOpenedPatientBillHistoryPopup()
        {
            SiteDriver.WaitToLoadNew(3000);
            bool isHandled = false;
            try
            {
                SiteDriver.WaitForCondition(() =>
                {
                    if (SiteDriver.WindowHandles.Count > 1)
                        isHandled = SiteDriver.SwitchWindowByTitle(
                            PageTitleEnum.ExtendedPageBillHistory.GetStringValue());
                    return isHandled;
                });

                if (SiteDriver.CloseWindowAndSwitchTo(_originalWindow))
                    Console.WriteLine("Automatically opened Patient Bill History page closed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch exception and close window" + ex.Message);
                SiteDriver.SwitchWindow(_originalWindow);
            }

            return isHandled;

        }

        
        public ClaimHistoryPage SwitchToPatientClaimHistory()
        {
            var patientHistory = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                Console.WriteLine("Switch To Patient Claim History Page");
            });
            return new ClaimHistoryPage(Navigator, patientHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsProviderHistoryPopUpPresent()
        {
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                var patientHistory = new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                var isProviderHistoryPopUp = patientHistory.IsProviderHistoryTextLabelSelected();
                SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                Console.WriteLine("Patient Claim History page closed");

                return isProviderHistoryPopUp;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }

            return false;
        }
        
        public bool IsProviderHistoryPopUpOpen()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue());

        }

        public bool IsLogicMangerPopUpPresent()
        {
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue()));
                var LogicMaanger = new LogicManagementPage(Navigator, new LogicManagementPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                var isProviderHistoryPopUp = LogicMaanger.IsLogicManagerPageTitlePresent();
                SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                Console.WriteLine("Logic Manager page closed");

                return isProviderHistoryPopUp;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }

            return false;
        }

        public bool IsProviderHistoryClose()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.ClaimAction.GetStringValue()) ||
                   SiteDriver.Title.Equals(PageTitleEnum.BillAction2.GetStringValue());
        }

        public bool IsClaimProcessingHistoryOpen()
        {
            return SiteDriver.Title.Contains(PageTitleEnum.ClaimProcessingHistoryPage.GetStringValue());
        }

        public bool IsOriginalClaimDataOpen()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.OriginalClaimDataPage.GetStringValue());
        }


        public bool IsFlagWithXIsEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ViewSystemDeletedFlagIconCssLocator,
                How.CssSelector);
        }

        public bool IsCurrentClaimLocked(string userName,string clientName="SMTST")
        {
            var lockedClaimList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientName, userName));
            return lockedClaimList != null && lockedClaimList.Contains(GetCurrentClaimSequence());
        }

      
        public bool IsClaimSearchResultsLocked(string userSeq, List<string> claimSearchList, string clientName = "SMTST")
        {
            var lockedClaimList = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientName, userSeq));
            if (lockedClaimList == null)
                return false;
            return claimSearchList.All(x => lockedClaimList.Contains(x));

        }

        public bool IsClaimLocked(string clientname, string claseq, string clasub, string userSeq)
        {
            WaitForStaticTime(1000);
            var lockedClaimList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientname, userSeq));
            return lockedClaimList.Any(x => x.Contains($"{claseq}-{clasub}"));
        }


        public bool IsCoderReviewByClaimSequenceNull(string claimSequence)
        {
            var claSeq = claimSequence.Split('-');
            var result =
                Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetCoderReviewByClaimSequence,
                    claSeq[0], claSeq[1]));
            return string.IsNullOrEmpty(result);
        }

        public bool IsCoderReviewByClaimSequenceTrue(string claimSequence)
        {
            var claSeq = claimSequence.Split('-');
            var result =
                Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetCoderReviewByClaimSequence,
                    claSeq[0], claSeq[1]));
            if (string.IsNullOrEmpty(result)) return false; //handle for null or empty condition as well
            return result[0].Equals('T');
        }

        public bool IsFlagPciProduct(string claimSequence)
        {
            var claSeq = claimSequence.Split('-');
            var productList = Executor.GetTableSingleColumn(
                Format(ClaimSqlScriptObjects.GetEditFlagProductByClaimSequence, claSeq[0], claSeq[1]));
            if (productList.Count < 0) return false; //handle for null or empty condition as well

            var pciProd = productList.FirstOrDefault(s => s.Contains("F"));
            return !string.IsNullOrEmpty(pciProd);
        }

        public ClaimActionPage ClickOnCreateButton()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.CreateCssLocator, How.CssSelector); // ProxyWebElement);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Create Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickonWorkListToggler()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver
                    .FindElement(ClaimActionPageObjects.WorkListOptionSelectorDropDownCssLocator, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on WorkList Toggler");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsWorkListOptionPresent()
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "CV Work List"),
                How.XPath);
        }

        public ClaimActionPage ToggleWorkListToPci()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickonWorkListToggler();
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.WorkListOptionsXPathTemplate, "CV Work List"), How.XPath);
                ClickonWorkListToggler();
                Console.WriteLine("Selected CV work list from dropdown");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ToggleWorkListToQa()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickonWorkListToggler();
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.WorkListOptionsXPathTemplate, "CV QC Work List"),
                    How.XPath);
                ClickonWorkListToggler();
                Console.WriteLine("Selected CV QC Work List from dropdown");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ToggleWorkListToFci()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickonWorkListToggler();
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.WorkListOptionsXPathTemplate, "FCI Work List"), How.XPath);
                ClickonWorkListToggler();
                Console.WriteLine("Selected Fci work list from dropdown");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ToggleWorkListToPciRN()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickonWorkListToggler();
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.WorkListOptionsXPathTemplate, "CV RN Work List"),
                    How.XPath);
                ClickonWorkListToggler();
                Console.WriteLine("Selected Fci work list from dropdown");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickonFindOption()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.WaitForIe(2000);
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FindIconCssLocator, How.CssSelector);

                Console.WriteLine("Clicked on Find Option");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickonProviderDetailsIcon()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                WaitForStaticTime(2000);
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ProviderDetailsIconCssSelector, How.CssSelector).Click();
                Console.WriteLine("Clicked on Provider Details Icon");
                WaitForWorkingAjaxMessage();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage clickOnBadgeIconInContainerViewVerticalRow(int row)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.BadgeIconInContainerViewVerticalRowXPath, row),
                        How.XPath)
                    .Click();
                Console.WriteLine("Clicked on Badge Icon in Container View, vertical row {0} .", row);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickOnFlagLineSwitchedFromAnotherLine(string flagName, string switchFromLineNumber)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagLineSwitchedFromAnotherLineXPathLocator, flagName,
                    switchFromLineNumber), How.XPath).Click();
            Console.WriteLine("Clicked on flag line switched from another line on the same claim sequence.");
        }

        public void ClickOnFlagLineSwitchedFromAnotherLineFromDifferentClaim(string flagName, string claimSeq,
            string switchFromLineNumber)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagLineSwitchedFromAnotherLineDifferentClaimXPathLocator,
                    flagName, claimSeq, switchFromLineNumber), How.XPath).Click();
            Console.WriteLine("Clicked on flag line switched from another line from diffrent claim sequence.");
        }

        public void ClickOnFlagLineSwitchedToAnotherLine(string flagName, string switchToLineNumber)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.FlagLineSwitchedToAnotherLineXPathLocator, flagName,
                    switchToLineNumber), How.XPath).Click();
            Console.WriteLine("Clicked on flag line switched to another line.");
        }

        public ClaimActionPage ClickSwitchedToClaimSequence(string claimSeq)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.SwitchedToClaimSequenceXPath, claimSeq), How.XPath)
                    .Click();
                Console.WriteLine("Clicked on claim sequence in which a flag line is switched to another line.");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimAction.GetStringValue() + "/" +
                                                       claimSeq.Split('-')[0] + "/" + claimSeq.Split('-')[1]));
                SiteDriver.WaitForCondition(
                    () =>
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss,
                            How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void CloseSwitchedToClaimSequenceWindow()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
        }

        public void ClickWorkListIconWithinWorkList()
        {
            var element = SiteDriver.FindElement(ClaimActionPageObjects.WorkListIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Worklist icon");
        }

        public bool IsHciRunLabelPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciRunLabelCssLocator, How.CssSelector);
        }

        public void ClickMoreOption()
        {
            var element = SiteDriver.FindElement(ClaimActionPageObjects.MoreOptionCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsOriginalClaimDataLinkPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.OriginalClaimDataLinkXpath, How.XPath);
        }

        public bool IsHciRunValuePresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciRunValueCssLocator, How.CssSelector);
        }

        public bool IsWorkListIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.WorkListCssLocator, How.CssSelector);
        }

        public ClaimActionPage ClickWorkListIcon()
        {

            SiteDriver.WaitForCondition(IsWorkListIconPresent);
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.WorkListCssLocator, How.CssSelector);
                Console.WriteLine("Clicked on WorkList Icon");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsClaimSearchIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimSearchIconCssLocator, How.CssSelector);
        }

        public ClaimSearchPage ClickClaimSearchIcon()
        {
            SiteDriver.WaitForCondition(IsClaimSearchIconPresent);
            var newClaimSearch = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimSearchIconCssLocator, How.CssSelector);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Claim Search Icon");

            });
            return new ClaimSearchPage(Navigator, newClaimSearch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public QaClaimSearchPage ClickClaimSearchIconAndNavigateToQaClaimSearchPage()
        {
            SiteDriver.WaitForCondition(IsClaimSearchIconPresent);
            var qaClaimSearch = Navigator.Navigate<QaClaimSearchPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimSearchIconCssLocator, How.CssSelector);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Claim Search Icon");

            });
            return new QaClaimSearchPage(Navigator, qaClaimSearch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public LogicSearchPage ClickClaimSearchIconAndNavigateToLogicSearchPage()
        {
            SiteDriver.WaitForCondition(IsClaimSearchIconPresent);
            var logicSearch = Navigator.Navigate<LogicSearchPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimSearchIconCssLocator, How.CssSelector);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Claim Search Icon");
            });
            return new LogicSearchPage(Navigator, logicSearch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickSwitchEditIcon()
        {
            RemoveLock();
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.MouseOver(ClaimActionPageObjects.FirstEnabledSwitchButtonCssLocator, How.CssSelector);
                SiteDriver.FindElement(ClaimActionPageObjects.FirstEnabledSwitchButtonCssLocator,
                    How.CssSelector).Click();
                ClickOkCancelOnConfirmationModal(true);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Switch Edit Icon");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsQaAuditIconPresent()
        {
            return SiteDriver.IsElementPresent(
                Format("{0},{1}", ClaimActionPageObjects.QaAuditGreenIconCssSelector,
                    ClaimActionPageObjects.QaAuditRedIconCssSelector), How.CssSelector);
        }

        public bool IsQaAuditIconInReviewMode()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.QaAuditGreenIconCssSelector, How.CssSelector);
        }

        public bool IsQaAuditIconInLockedMode()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.QaAuditRedIconCssSelector, How.CssSelector);
        }

        public bool IsQaPassIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.QaPassIconCssLocator, How.CssSelector);
        }

        public int GetTotalDxCodesCountFromUI()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.DxCodesInClaimLineDetailsCssSelector,
                How.CssSelector);
        }

        public bool IsQaFailIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.QaFailIconCssLocator, How.CssSelector);
        }

        public string GetDxCodeValuesByRowColumn(int row, int column)
        {
            return SiteDriver.FindElement(Format(ClaimActionPageObjects.DxCodesDescriptionValuesInClaimLineDetailsByRowColumnCssTemplate, row,column),
                How.CssSelector).Text;
        }

        public bool IsQaPassFailIconDisabled()
        {
            return (
                SiteDriver.FindElementAndGetClassAttribute(ClaimActionPageObjects.QaPassIconCssLocator,How.CssSelector).Any(x => x.Contains("is_disabled"))
                &&
                SiteDriver.FindElementAndGetClassAttribute(ClaimActionPageObjects.QaFailIconCssLocator, How.CssSelector).Any(x => x.Contains("disabled")));
        }

        public string GetQaPassIconTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClaimActionPageObjects.QaPassIconCssLocator, How.CssSelector);
            return SiteDriver
                .FindElement(ClaimActionPageObjects.QaPassIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetQaFailIconTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClaimActionPageObjects.QaFailIconCssLocator, How.CssSelector);
            return SiteDriver
                .FindElement(ClaimActionPageObjects.QaFailIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetCurrentClaimSequence()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceValueInClaimActionCssLocator,
                    How.CssSelector).Text;
        }

        public string GetQaAuditIconTooltip()
        {
            return SiteDriver.FindElement(Format("{0},{1}",
                ClaimActionPageObjects.QaAuditGreenIconCssSelector,
                ClaimActionPageObjects.QaAuditRedIconCssSelector), How.CssSelector).GetAttribute("title");
        }

        public string GetTopRightComponentTitle()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.RightComponentHeaderSectionCssLocator, How.CssSelector).Text;
        }

        public string GetEmptyAppealSectionMessage()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.NoAppealsMessageSectionCssLocator, How.CssSelector).Text;
        }

        public string GetNoteTextInAuditTrial()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.NoteTextInAuditTrialCssLocator, How.CssSelector).Text;
        }

        public string GetModifiedDateTextInAuditTrial()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ModifiedDateTextInAuditTrialCssLocator,
                How.CssSelector).Text;
        }

        public string GetActionTextInAuditTrial()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ActionTextInAuditTrialCssLocator, How.CssSelector)
                .Text;
        }

        public string GetActionTypeInAuditTrial()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ActionTypeInAuditTrialCssLocator, How.CssSelector)
                .Text;
        }

        public string GetCodeInAuditTrial()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.CodeInAuditTrialCssLocator, How.CssSelector).Text;
        }

        public string GetModifiedByInAuditTrial()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ModifiedByInAuditTrialCssLocator, How.CssSelector)
                .Text;
        }

        public ClaimActionPage ClickOnViewAppealIcon()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                WaitForStaticTime(2000);
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ViewAppealIconCssLocator, How.CssSelector).Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on View Appeal Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetDxLvlValueFromDatabaseByDxCode(string dxcode, string claimSeq,string lineNo,string client = "RPE")
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetDxLvlValueFromDatabase,dxcode,claimSeq,lineNo,client));
        }

        public ClaimActionPage ClickOnViewDxCodesIcon()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ViewDxCodesIconCssLocator, How.CssSelector).Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on View DX Codes Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetLeftHeaderOfQ2Quadrant()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.Q2LeftHeaderCssLocator, How.CssSelector)
                .Text;
        }


        public int GetLockedClaimCountByUser(string username,string clientName="SMTST")
        {
            return (int) Executor.GetSingleValue(string.Format(ClaimSqlScriptObjects.LockedClaimCountByUser,clientName,username));
        }

       

        public void ClickOnlyCreateAppealIcon()
        {
        SiteDriver
            .FindElement(ClaimActionPageObjects.AddAppealIconCssSelector, How.CssSelector).Click();
            Console.WriteLine("Clicked on Create Appeal Button");
        }

        public void ClickOnlyDashboardIcon()
        {
            SiteDriver.MouseOver(DefaultPageObjects.UserMenuId, How.Id);
            //JavaScriptExecutor.ExecuteMouseOver(DefaultPageObjects.UserMenuDivCssLocator, How.CssSelector);
            JavaScriptExecutor.ExecuteClick(DefaultPageObjects.DashboardIconXPath, How.XPath);
            SiteDriver.WaitForPageToLoad();
            JavaScriptExecutor.ExecuteMouseOut(DefaultPageObjects.UserMenuDivCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Dashboard icon.");
        }

        public void ClickOnlyQuickLaunchIcon()
        {
            SiteDriver.FindElement(NewDefaultPageObjects.QuickLaunchButtonXPath, How.XPath).Click();
            Console.WriteLine("Clicked on Quick Launch Button");
        }

        public void ClickOnlyHelpIcon()
        {
            SiteDriver.MouseOver(DefaultPageObjects.UserMenuId, How.Id);
            //JavaScriptExecutor.ExecuteMouseOver(DefaultPageObjects.UserMenuDivCssLocator, How.CssSelector);
            var element = SiteDriver.FindElement(DefaultPageObjects.HelpCenterXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public void ClickOnlyAppealSearch()
        {
            SiteDriver.FindElement(".//ul/li/a[contains(text(),'Appeal Search')]", How.XPath).Click();
        }

        public ClaimActionPage ClickSearchIcon()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.WaitForIe(2000);
                var element = SiteDriver
                    .FindElement(ClaimActionPageObjects.SearchIconCssLocator, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Search Icon");

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetSearchIcontooltip()
        {
             return SiteDriver.FindElement(ClaimActionPageObjects.ClaimSearchIconCssLocator, How.CssSelector)
                .GetAttribute("title");

        }

        public ClaimActionPage SearchByClaimSequence(string claimSeq)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(ClaimActionPageObjects.ClaimSequenceXPath, How.XPath);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(claimSeq);
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FindXPath, How.XPath);//.ProxyWebElement);
                WaitForWorkingAjaxMessage();
                WaitForStaticTime(1000);
                Console.WriteLine("Searched Claim Sequence: " + claimSeq);
                if (IsClaimLocked() && GetLockIConTooltip().Contains("unavailable"))
                {
                    RefreshPage();
                }
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SearchByClaimNoAndStayOnSearch(string claimNo)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ClaimNoXPath, How.XPath).ClearElementField();
                SiteDriver
                    .FindElement(ClaimActionPageObjects.ClaimNoXPath, How.XPath).SendKeys(claimNo);
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FindXPath, How.XPath);//ProxyWebElement);
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Searched Claim No: " + claimNo);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnClaimSequenceOfSearchResult(int row)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                        Format(AppealCreatorPageObjects.ClaimActionLinkOnSearchResultTemplate, row),
                        How.CssSelector)
                    .Click();
                Console.WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickSystemDeletedFlagIcon()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.ViewSystemDeletedFlagIconCssLocator, How.CssSelector).Click();
                    Console.WriteLine("Clicked on System Deleted Flag Icon");
                    WaitForStaticTime(500);

        }

        public bool IsSystemDeletedFlagPresentInFlagDetailSection()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditFlagDetailSectionCssLocator,
                How.CssSelector);
        }

        public bool IsBadgeIconPresentInAppeal()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ViewAppealIconWithBadgeCssLocator,
                How.CssSelector);
        }

        public bool IsProviderDetailsIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProviderDetailsIconCssSelector,
                How.CssSelector);
        }

        public bool IsProviderDetailsViewDivDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProviderDetailsViewDivXPath, How.XPath);
        }

        public bool IsBadgePresentInProviderDetailIcon()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ProviderDetailsIconBadgeCssLocator,
                How.CssSelector);
        }

        public bool IsDataPointsContainsValue()
        {
            return IsDataPointsValuepresent(1) || IsDataPointsValuepresent(2) || IsDataPointsValuepresent(3) ||
                   IsDataPointsValuepresent(4);
        }

        public bool IsDataPointsValuepresent(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DataPointsOnContainerViewXPath, row), How.XPath);
        }

        public bool IsFlagAuditTrailInfoPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.FlagAuditTrailInfoCssLocator, How.CssSelector);
        }

        public bool IsSystemDeletedFlagPresentInFlagLinesSection()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SystemDeletedFlagLineCssLocator,
                How.CssSelector);
        }

        public bool IsViewSystemDeletedFlagIconEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ViewSystemDeletedFlagIconCssLocator,
                How.CssSelector);
        }

        public bool IsCustomizationPopulatedValueVisible()
        {
            return
                SiteDriver.IsElementPresent(ClaimActionPageObjects.CustomizationValueCssLocator, How.CssSelector);
        }

        public string GetCustomizationPopulatedField()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.CustomizationValueCssLocator, How.CssSelector).Text;
        }

        public string GetCustomizationPopulatedLabel()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.CustomizationLabelCssLocator, How.CssSelector).Text;
        }

        public int GetBadgeValue()
        {
            return Convert.ToInt32(SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderDetailsIconBadgeCssLocator, How.CssSelector)
                .Text);
        }

        public int GetTotalDataPointsOnContainerView()
        {
            return Convert.ToInt32(GetDataPoint(1)) + Convert.ToInt32(GetDataPoint(2)) +
                   Convert.ToInt32(GetDataPoint(3)) + Convert.ToInt32(GetDataPoint(4));
        }

        public string GetDataPoint(int row)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.DataPointsOnContainerViewXPath, row),
                    How.XPath).Text;
        }

        public List<String> GetContainerViewVerticalRows()
        {
            var containerList =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.ContainerViewVerticalRowLabelListXPath, How.XPath,
                    "Text");
            return containerList.Select(c => c.Trim(':')).ToList();
        }

        public string GetContainerViewVerticalRowLabel(int row)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.ContainerViewVerticalRowLabelXPath, row), How.XPath).Text
                .Trim(':');
        }

        public string GetContainerViewVerticalRowValue(int row)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.ContainerViewVerticalRowValueXPath, row), How.XPath).Text
                .Trim(':');
        }

        public string GetContainerViewVerticalRowSecondValue(int row)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.ContainerViewVerticalRowSecondValueXPath, row), How.XPath)
                .Text.Trim(':');
        }

        public int GetVerticalRowCount()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.ContainerViewVerticalRowCountXPath,
                How.XPath);
        }

        public int GetVerticalRowCountWithBatch()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.ContainerViewVerticalRowCountWithBatchXPath,
                How.XPath);
        }

        public int GetDataPointListCountForVerticalRow(int row)
        {
            if (row == 2)
            {
                return SiteDriver.FindElementsCount(
                    Format(ClaimActionPageObjects.DataPointListCountForSecondVerticalRowXPath, row),
                    How.XPath);
            }
            return SiteDriver.FindElementsCount(
                Format(ClaimActionPageObjects.DataPointListCountForVerticalRowXPath, row), How.XPath);
        }

        public int GetBadgeValueDisplayedInContainerViewFirstVerticalRow()
        {
            return Convert.ToInt32(SiteDriver
                .FindElement(ClaimActionPageObjects.BadgeIconInContainerViewFirstVerticalRowCountXPath,
                    How.XPath).Text);
        }

        public int GetBadgeValueDisplayedInContainerViewVerticalRow(int row)
        {
            return Convert.ToInt32(SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.BadgeIconInContainerViewVerticalRowXPath, row), How.XPath)
                .Text);
        }

        public bool IsBadgeIconDisplayedInContainerViewFirstVerticalRow()
        {
            return SiteDriver.IsElementPresent(
                ClaimActionPageObjects.BadgeIconInContainerViewFirstVerticalRowCountXPath, How.XPath);
        }

        public bool IsBadgeIconDisplayedInContainerViewVerticalRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.BadgeIconInContainerViewVerticalRowXPath, row), How.XPath);
        }

        public bool AreAllRowsCollapsedInProviderDetailsContainerView()
        {
            return (!IsContainerViewVerticalRowVisible(1) && !IsContainerViewVerticalRowVisible(2) &&
                    !IsContainerViewVerticalRowVisible(3) && !IsContainerViewVerticalRowVisible(4));
        }

        public bool IsContainerViewVerticalRowVisible(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.CollapsedContainerViewVerticalRowXPath, row), How.XPath);
        }

        public ClaimHistoryPage ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.PatientClaimHistoryIconCssLocator, How.CssSelector).Click();
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector));
                ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.ExtendedPageClaimHistory.GetStringValue();
                return new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return null;
        }

        public ClaimHistoryPage ClickOnHistoryButtonAndSwitchToPatientBillHistoryPage()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.PatientClaimHistoryIconCssLocator, How.CssSelector).Click();
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector));
                ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.PatientBillHistory.GetStringValue();
                return new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return null;
        }

        public ClaimHistoryPage ClickOnDentalLinkAndSwitchToPatientClaimHistoryPage(string DentalLabel)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.DentalHistoryLinkXPathTemplate, DentalLabel), How.XPath)
                .Click();
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector));
                return new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return null;
        }

        public int GetDeletedAppealCountByClaseq(string claseq)
        {
            return Convert.ToInt32(Executor.GetTableSingleColumn(Format(
                AppealSqlScriptObjects.GetDeletedAppealCount,
                claseq.Split('-')[0], claseq.Split('-')[1]))[0]);
        }

        public ClaimHistoryPage SwitchToProviderHistoryPopUp()
        {
            try
            {
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ExtendedPageClaimHistory.GetStringValue()));
                return new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return null;
        }

        public ClaimActionPage ClickOnBackButton()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.BackButtonXPath, How.XPath);
            return new ClaimActionPage(Navigator, new ClaimActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimSearchPage ClickOnBrowserBackButtonToClaimSearch()
        {
            SiteDriver.Back();
            return new ClaimSearchPage(Navigator, new ClaimSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetProviderSequenceValue()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ProviderSequenceTextLabelCss, How.CssSelector).Text;
        }

        public string GetPlanOptionSelectorText()
        {
            return SiteDriver.FindElementAndGetAttribute(ClaimActionPageObjects.PlanSelectOptionXPath, How.XPath, "placeholder");

        }

        public string GetClaimStatusOptionSelectorText()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimStatusSelectOptionXPath, How.XPath).GetAttribute("value");
        }

        public string GetClaimSubStatusText()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ClaimSubStatusTextXPath, How.XPath)
                .GetAttribute("placeholder");
        }

        public void ClickOnLinesIcon()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimLinesCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Lines Icon");
        }

        public bool IsClaimLinesPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimLinesCssLocator, How.CssSelector);
        }

        public ClaimActionPage ClickOnNextButton()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => !IsNextButtonDisabled());
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.NextButtonCss, How.CssSelector);
                Console.WriteLine("Next Button is clicked.");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool ClickOnNextButtonWithoutWaitAndIsNextButtonDisabled()
        {
            var isDisabled = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.NextButtonCss,
                                 ClaimActionPageObjects.DisabledNextCssLocator) != null;
            //SiteDriver.FindElement(NewClaimActionPageObjects.NextButtonCss, How.CssSelector).Click();
            //
            Console.WriteLine("Next Button is clicked.");
            return isDisabled;
        }

        public void ClickOnApproveButtonWithoutWait()
        {
            //JavaScriptExecutor.ExecuteClick(NewClaimActionPageObjects.NextButtonCss, How.CssSelector);
            SiteDriver.FindElement(ClaimActionPageObjects.ApproveClaimIconCssLocator, How.CssSelector)
                .Click();
            Console.WriteLine("Approve Button is clicked.");
        }


        public void ClickOnLineDetailsSection()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.LineDetailsSectionCss, How.CssSelector).Click();
                    Console.WriteLine("Clicked on Line Details");
        }

        public void ClickOnFlagDetailsSection()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FlagDetailsSectionCss, How.CssSelector); //ProxyWebElement);
            Console.WriteLine("Clicked on Flag Line");
            SiteDriver.WaitForIe(3000);
        }

        public void ClickOnSystemDeletedFlagLine()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.SystemDeletedFlagLineCssLocator, How.CssSelector);//.ProxyWebElement);
            
            Console.WriteLine("Clicked on Flag Line");
        }

        public void ClickOnDollarIcon()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.DollarIconCssLocator, How.CssSelector).Click();
            SiteDriver.WaitToLoadNew(500);
            Console.WriteLine("Clicked on Dollar Icon");
        }

        public void ClickOnEditAllFlagsIcon()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.EditFlagsIconCssLocator, How.CssSelector).Click();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.EditAllFlagClaimSectionCssLocator,
                    How.CssSelector));
            Console.WriteLine("Clicked on Edit All Flags Icon");
        }

        public bool IsVisibleToclientPresentInAllFlagsSection()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.VisibleToClientAllFLagsCssSelector,
                How.CssSelector);
        }
        public bool IsVisibleToclientPresentInFlagLineSection()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.VisibleToClientFlagLineCssSelector,
                How.CssSelector);
        }
        public bool IsVisibleToclientPresentInEditAllFlagLineSection()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.VisibleToClientLineFlagCssSelector,
                How.CssSelector);
        }

        public bool IsVisibleToclientSelectedInAllFlagsSection()
        {
            return SiteDriver.FindElementAndGetClassAttribute(ClaimActionPageObjects.VisibleToClientAllFLagsCssSelector,
                How.CssSelector).Any(x => x.Contains("selected"));
            
        }
        public bool IsVisibleToclientSelectedInFlagLineSection()
        {
            return SiteDriver.FindElementAndGetClassAttribute(ClaimActionPageObjects.VisibleToClientFlagLineCssSelector,
                How.CssSelector).Any(x => x.Contains("selected"));
         
        }
        public bool IsVisibleToclientSelectedInEditAllFlagLineSection()
        {
            return SiteDriver.FindElementAndGetClassAttribute(ClaimActionPageObjects.VisibleToClientLineFlagCssSelector,
                How.CssSelector).Any(x => x.Contains("selected"));
         
        }


        public void ClickOnDeleteAllFlagsIcon(bool handleIfPopup=true, bool waitForRestore = true)
        {
            RemoveLock();
            var element = SiteDriver
                .FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Delete All Flags Icon");
            if (IsPageErrorPopupModalPresent() && handleIfPopup)
            {
                ClosePageError();
                RemoveLock();
                SiteDriver
                    .FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator, How.CssSelector).Click();
            }
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            if(waitForRestore)
                SiteDriver.WaitForCondition(() => IsRestoreButtonPresent());
        }

        public void ClickOnDeleteAllFlagsIconOnFlagLine()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.DeleteLineIconCssLocator, How.CssSelector).Click();
            Console.WriteLine("Clicked on Delete All Flags Icon");
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RemoveLock();
                SiteDriver.FindElement(ClaimActionPageObjects.DeleteLineIconCssLocator, How.CssSelector).Click();
            }
            SiteDriver.WaitForCondition(()=>IsRestoreLineIconPresent());
        }
        public bool ClickOnDeleteAllFlagsIconAndNextIconIsDisabled()
        {
            RemoveLock();
            var isDisabled = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                                 ClaimActionPageObjects.DisabledNextCssLocator, true) != null;
            Console.WriteLine("Clicked on Delete All Flags Icon");
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                isDisabled = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                                 ClaimActionPageObjects.DisabledNextCssLocator, true) != null;
            }
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            return isDisabled;
        }


        public void ClickOnDeleteAllFlagsIconNoWait()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator, How.CssSelector).Click();
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RemoveLock();
                SiteDriver
                    .FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator, How.CssSelector).Click();
            }
            Console.WriteLine("Clicked on Delete All Flags Icon");
        }

        public bool ClickOnDeleteAllFlagsIconAndIsWorkingIconPresent()
        {
            RemoveLock();
            var displayed = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                                ClaimActionPageObjects.WorkingAjaxMessageCssLocator, true) != null;
            Console.WriteLine("Clicked on Delete All Flags Icon");
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RemoveLock();
                displayed = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                                ClaimActionPageObjects.WorkingAjaxMessageCssLocator, true) != null;
            }
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            return displayed;
        }

        public bool ClickOnDeleteAllFlagsIconAndIsApprovedIconDisabled()
        {
            RemoveLock();
            var disabled = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                               ClaimActionPageObjects.DisabledApproveCssLocator, true) != null;
            Console.WriteLine("Clicked on Delete All Flags Icon");
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RemoveLock();
                disabled = JavaScriptExecutor.ClickAndGet(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                               ClaimActionPageObjects.DisabledApproveCssLocator, true) != null;
            }
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            return disabled;
        }

        public void ClickOnRestoreAllFlagsIconNoWait()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.RestoreFlagsIconCssLocator, How.CssSelector).Click();
            Console.WriteLine("Clicked on Restore All Flags Icon");
        }

        public string ClickOnFirstEditFlag(bool isDeleted = false)
        {
            RemoveLock();
            string flag = string.Empty;
            string flaggedLines = isDeleted
                ? ClaimActionPageObjects.DeletedFlaggedLinesCssLocator
                : ClaimActionPageObjects.FlaggedLinesCssLocator;
            Task newClaimAction = Task.Factory.StartNew(() =>
            {
                Navigator.Navigate<ClaimActionPageObjects>(() =>
                    SiteDriver.FindElement(string.Concat(flaggedLines, " span.small_edit_icon.is_active"),
                        How.CssSelector).Click());
                flag = SiteDriver.FindElement(string.Concat(flaggedLines, " > li:nth-of-type(3) > span"),
                    How.CssSelector).Text;
            });
            newClaimAction.ContinueWith(taskPrev => StringFormatter.PrintMessage("Click on Edit Flag Icon")).Wait();
            return flag;
        }

        public void ClickOnSelectedClaimLinesOnSelectedLinesByRow(int row)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.SelectedLineSelectedClaimLinesCssSelectorTemplate, row),
                How.CssSelector).Click();
        }

        public void ClickOnFlagLineByLineNoWithFlag(int lineNo, string flag, int row = 3)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.SpecificLineAndEditXPathTemplate, lineNo, flag, row), How.XPath);
            WaitForWorking();
        }

        public void ClickOnEditForGivenFlag(string flag)
        {
            RemoveLock();
            JavaScriptExecutor.ExecuteClick(

                Format(ClaimActionPageObjects.FlagLevelEditIconXPathTemplate, flag), How.XPath);
            Console.WriteLine(Format("Clicked on Delete Icon for Flag {0}", flag));

        }

        public string GetFlagLevelUnitValue(string flag)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.FlagLevelUnitXPathTemplate, flag), How.XPath).Text;
        }

        public string GetClaimLevelUnitValue(string flag)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.ClaimLevelUnitXPathTemplate, flag), How.XPath).Text;
        }

        public Boolean IsLocked()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.LockIconCssSelector, How.CssSelector);
        }

        public Boolean IsDisabled(ActionItems actionItems)
        {
            bool check = false;
            switch ((int) actionItems)
            {
                case 1:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledEditAllCssLocator,
                        How.CssSelector);
                    break;
                case 2:
                    check =
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledDeleteAllCssLocator,
                            How.CssSelector) ||
                        SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledRestoreAllCssLocator,
                            How.CssSelector);
                    break;
                case 3:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledApproveCssLocator,
                        How.CssSelector);
                    break;
                case 4:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledApproveNextCssLocator,
                        How.CssSelector);
                    break;
                case 5:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledNextCssLocator,
                        How.CssSelector);
                    break;
                case 6:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledTransferCssLocator,
                        How.CssSelector);
                    break;
                case 7:
                    check = SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledAddDocumensCssLocator,
                        How.CssSelector);
                    break;
            }
            return check;
        }

        public bool IsAddIconDisabledInFlaggedLine()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledAddCssLocator, How.CssSelector);
        }

        #region EDIT/DELETEALLFLAGSECTION

        public void ClickOnSaveButton()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.SaveButtonCssLocator, How.CssSelector);//ProxyWebElement);
            Console.WriteLine("Clicked on Save Button");

            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();



        }

        public void ClickOnSaveButtonForTransferClaim()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.SaveButtonCssLocator, How.CssSelector).Click();
            Console.WriteLine("Clicked on Save Button");
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(IsBlankPagePresent);
        }

        public void ClickOnSaveEditButton()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.SaveEditButtonCssLocator, How.CssSelector).Click();
            Console.WriteLine("Clicked on Save Button");
            WaitForWorkingAjaxMessage();

        }

        public bool IsSaveButtonEnabled()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.SaveEditButtonCssLocator, How.CssSelector).Enabled;
        }

        public void SelectReasonCode(string reasonCode)
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.EditReasonCodeCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(200);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.EditReasonCodeSelectorXPath, reasonCode), How.XPath);
            Console.WriteLine("ReasonCode Selected: <{0}>", reasonCode);
        }

        public void SelectReasonCodeInEditAllFlag(string reasonCode)
        {
            //here findelement doesn't work for ie browser, hence using javascript
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.EditReasonCodeInEditAllFlagCssLocator,
                How.CssSelector);

            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.EditReasonCodeSelectorInEditAllFlagXPath, reasonCode),
                How.XPath);
            Console.WriteLine("ReasonCode Selected: <{0}>", reasonCode);
        }

        public bool IsReasonCodeEnabled() => SiteDriver
            .FindElementAndGetClassAttribute(ClaimActionPageObjects.EditReasonCodeLabelCssSelector, How.CssSelector)[0]
            .Contains("is_enable");
        

        public void SelectStatusCode(string statusCode)
        {
            JavaScriptExecutor.FindElement(ClaimActionPageObjects.StatusComboBoxCssSelector).Click();
            JavaScriptExecutor.FindElement(
                Format(ClaimActionPageObjects.StatusCodeCssTemplate, statusCode)).Click();
            Console.WriteLine("StatusCode Selected: <{0}>", statusCode);
        }

        public List<string> GetStatusCodeList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.StatusComboBoxCssSelector, How.CssSelector);
            var statusList =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.StatusOptionListCssSelector, How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.StatusComboBoxCssSelector, How.CssSelector);
            statusList.RemoveAt(0);
            return statusList;
        }

        public void InsertStatusCode(string statusCode)
        {
            SiteDriver.FindElement(
                "//div[@class='ember-view custom_select_component field transfer_selects']/span/div[@class='selected_option pointer']/span",
                How.XPath).Click();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.StatusCodeCssTemplate, statusCode), How.CssSelector).Click();
            Console.WriteLine("ReasonCode Selected: <{0}>", statusCode);
        }

        public void SelectClaimType(string claimType)
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimTypeDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.ClaimTypeListOptionsSelectorXPathTemplate, claimType),
                How.XPath);
            Console.WriteLine("ClaimType Selected: <{0}>", claimType);
        }

        public string GetClaimType()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ClaimTypeXPath, How.XPath)
                .GetAttribute("value");
        }

        public void SelectFlag(string flag)
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FlagDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.FLagListOptionsSelectorXPathTemplate, flag), How.XPath);
            Console.WriteLine("Flag Selected: <{0}>", flag);
        }

        public List<decimal> GetAdjustedPaidFromTriggerClaimLineSection()
        {
            return
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.TriggerClaimLineAdjustedPaidXPath, How.XPath, "Text")
                    .Select(x => decimal.Parse(x.Substring(1, x.Length - 1))).ToList();
        }

        public void SelectFlagInAddFlag(string flag)
        {
            SiteDriver.FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath,
                How.XPath).Click();

            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.FlagListOptionsAddFlagSectionXPathTemplate, flag),
                    How.XPath)
                .Click();


            Console.WriteLine("Flag Selected: <{0}>", flag);
        }

        public void SelectAddInFlag(string flag)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath, How.XPath).SendKeys(flag);
            SiteDriver.FindElement<ImageButton>(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath,
                How.XPath).SendKeys(Keys.Enter);


            Console.WriteLine("Flag Selected: <{0}>", flag);
        }

        public List<string> GetFlagListFromFlagDropdownInAddFlagForm()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath,
                How.XPath).Click();

            return SiteDriver.FindElementsAndGetAttribute("innerText", ClaimActionPageObjects.FlagListOptionsInFlagDropdownInAddFlagForm, How.XPath);
        }

        public void SelectAddInFlagAdd(string flag)
        {
            var element = SiteDriver
                .FindElement(ClaimActionPageObjects.FlagDropDownAddFlagSectionXPath, How.XPath);
            SiteDriver.WaitToLoadNew(300);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            element.SendKeys(flag);
            element.SendKeys(Keys.ArrowDown);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.FlagListOptionsAddFlagSectionXPathTemplate, flag), How.XPath);
           WaitForWorkingAjaxMessage();

            Console.WriteLine("Flag Selected: <{0}>", flag);
        }

        public void SelectFlagSourceInAddFlag(string flag)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.FlagSrcDropDownAddFlagSectionXPath, How.XPath).ClearElementField();
            SiteDriver
                .FindElement(ClaimActionPageObjects.FlagSrcDropDownAddFlagSectionXPath, How.XPath).SendKeys(flag);
            SiteDriver
                .FindElement(ClaimActionPageObjects.FlagSrcDropDownAddFlagSectionXPath, How.XPath).SendKeys(Keys.ArrowDown);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.FlagSrcDropDownAddFlagSectionXPathTemplate, flag), How.XPath);
            
            Console.WriteLine("Flag Source Selected: <{0}>", flag);
        }

        public bool IsClaimSequenceWithLineAndDosExist(string div, string claimSeq, string line, string dosValue,
            ref string claimSequenceLineDos)
        {
            if (!SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.TriggerClaimLineWithDosXPathTemplate, div, claimSeq, line,
                    dosValue),
                How.XPath))
            {
                List<string> claimSeqLineDos = JavaScriptExecutor.FindElements(
                    Format(ClaimActionPageObjects.TriggerClaimLineDosValueXPathTemplate, div, line),
                    How.XPath,
                    "Text");
                claimSequenceLineDos = claimSeqLineDos[0] + " " + claimSeqLineDos[1] + " " + claimSeqLineDos[2];
                return false;
            }

            return true;
        }

        public List<string> GetTriggerClaimSequenceList()
        {
            return JavaScriptExecutor.FindElements("//section[@class='add_trigger bottom_section']/div/section/div[1]/span/a",
                How.XPath, "Text");
        }

        public List<string> GetProviderNameListFromTriggerClaimLine()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ProviderNameTriggerClaimLineXPath, How.XPath,
                "Text");
        }

        public bool IsTriggerClaimLineDisabled()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TriggerClaimLineInputXPath, How.XPath).GetAttribute("disabled") != null;
        }

        public string GetFlag()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.FlagXPath, How.XPath).GetAttribute("value");
        }

        public ClaimActionPage SelectBatchId(string batchId, bool otherPage = false)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (otherPage)
                {
                    JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.BatchIdDropDownXPathOtherPage, How.XPath);
                    SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.BatchListOptionsSelectorXPathTemplateOtherPage,
                            batchId),
                        How.XPath).Click();

                    Console.WriteLine("BatchId Selected: <{0}>", batchId);
                }
                else
                {
                    JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.BatchIdDropDownXPath, How.XPath);
                    SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.BatchListOptionsSelectorXPathTemplate, batchId),
                        How.XPath).Click();

                    Console.WriteLine("BatchId Selected: <{0}>", batchId);
                }
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetBatchId()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.BatchIdXPath, How.XPath)
                .GetAttribute("value");
        }

        public ClaimActionPage SelectPlan(string plan)
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.PlanDropDownXPath, How.XPath);

            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.PlanOptionXPathTemplate, plan),
                How.XPath);

            JavaScriptExecutor.ExecuteMouseOut(ClaimActionPageObjects.PlanDropDownXPath, How.XPath);
            SiteDriver.FindElement("//label[text()='Flagged Lines']", How.XPath).Click();
            Console.WriteLine("Plan Selected: <{0}>", plan);
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.PlanDropDownXPath, How.XPath);
                JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.PlanOptionXPathTemplate, plan),
                    How.XPath);

                JavaScriptExecutor.ExecuteMouseOut(ClaimActionPageObjects.PlanDropDownXPath, How.XPath);
                SiteDriver.FindElement("//label[text()='Flagged Lines']", How.XPath).Click();
                Console.WriteLine("Plan Selected: <{0}>", plan);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #region ModifierExplanationFlagDetails

        public bool IsModifierExplanationValuePresentInFirstFlagRowOfFirstDiv()
        {
            return !string.IsNullOrEmpty(SiteDriver
                .FindElement(ClaimActionPageObjects.ModifierExplanationOfFirstFlagRowCssLocator, How.CssSelector).Text);
        }

        public bool IsModifierExplanationPresentInFlagDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ModifierNumberFlagDetailsXPath, How.XPath) &&
                   SiteDriver.IsElementPresent(ClaimActionPageObjects.ModifierDescriptionFlagDetailsXPath,
                       How.XPath);
        }

        public string GetModifierExplanationValueOfFlagDetails()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ModifierNumberFlagDetailsXPath,
                       How.XPath).Text +
                   " " + SiteDriver
                       .FindElement(ClaimActionPageObjects.ModifierDescriptionFlagDetailsXPath, How.XPath)
                       .Text;
        }

        #endregion

        #region ModifierExplanationLineDetails

        public bool IsModifierExplanationValuePresentInFlaggedLinesOfFirstDiv()
        {
            return !string.IsNullOrEmpty(SiteDriver
                .FindElement(ClaimActionPageObjects.ModifierExplanationFlaggedLinesCssLocator, How.CssSelector).Text);
        }

        public bool IsModifierExplanationPresentInLinesDetails()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ModifierNumberLinesDetailsXPath, How.XPath) &&
                   SiteDriver.IsElementPresent(ClaimActionPageObjects.ModifierDescriptionLinesDetailsXPath,
                       How.XPath);
        }

        public string GetModifierExplanationValue()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ModifierNumberLinesDetailsXPath,
                       How.XPath).Text +
                   " " + SiteDriver
                       .FindElement(ClaimActionPageObjects.ModifierDescriptionLinesDetailsXPath,
                           How.XPath).Text;
        }

        #endregion

        public ClaimActionPage SelectClaimStatus(string claimStatus)
        {


            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {

                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimStatusDropDownXPath, How.XPath);
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.ClaimStatusOptionXPathTemplate, claimStatus), How.XPath);

                Console.WriteLine("ClaimStatus Selected: <{0}>", claimStatus);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SelectFCIClaimStatus(string claimStatus)
        {

            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {

                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimStatusFCIDropDownXPath, How.XPath);
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.ClaimStatusFCIOptionXPathTemplate, claimStatus), How.XPath);

                Console.WriteLine("ClaimStatus Selected: <{0}>", claimStatus);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetWorkListClaimStatus()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ClaimStatusXPath, How.XPath)
                .GetAttribute("value");
        }

        public void SelectClaimSubStatus(string claimSubStatus)
        {
            SiteDriver.WaitForIe(1000);

            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimSubStatusDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.ClaimSubStatusSelectOptionXPathTemplate, claimSubStatus),
                How.XPath);
            SiteDriver.FindElement("//label[text()='Flagged Lines']", How.XPath).Click();

            Console.WriteLine("ClaimSubStatus Selected: <{0}>", claimSubStatus);
        }

        public void ClickRestoreButtonOfEditAllFlagsSection()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.RestoreButtonOfEditAllFlagSectionCssLocator, How.CssSelector).Click();
            Console.WriteLine("Click on Restore Button");
        }

        public void ClickDeleteButtonOfEditAllFlagsSection()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.DeleteButtonOfEditAllFlagSectionCssLocator, How.CssSelector).Click();
            Console.WriteLine("Click on Delete Button");
        }


        public void ClickEditAllFlagsOnLineButton()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.EditAllFlagsOnLineCssLocator, How.CssSelector).Click();
            Console.WriteLine("Click on Edit All Flags on Line Icon");
        }


        public List<string> GetExpectedReasonCodesForAllFlagsOnClaimOption(string action,bool internalUser = true)
        {
            List<string> list = new List<string>();
            if (internalUser)
            {
                if (action == "No Action")
                        list = Executor.GetTableSingleColumn(Format(
                            ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnClaimOptionNoAction,"HCI", "H")).ToList();
                    else
                        list = Executor.GetTableSingleColumn(Format(
                            ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnClaimOption,
                            action, "HCI", "H")).ToList();
            }

            else
            {
                if (action == "No Action")
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnClaimOptionNoAction,
                       "CLT", "C")).ToList();
                else
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnClaimOption,
                        action, "CLT", "C")).ToList();
            }

            return list;
        }

        public List<string> GetExpectedReasonCodesForAParticularFlag(string product, string action, bool internalUser = true)
        {
            List<string> list = new List<string>();
            if (internalUser)
            {
                if (action == "No Action")
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAFlagNoAction,
                        product, "HCI", "H")).ToList();
                else
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAFlag,
                        product, action, "HCI", "H")).ToList();
            }
            else
            {
                if (action == "No Action")
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAFlagNoAction,
                        product, "CLT", "C")).ToList();
                else
                    list = Executor.GetTableSingleColumn(Format(
                        ClaimSqlScriptObjects.GetExpectedReasonCodesForAFlag,
                        product, action, "CLT", "C")).ToList();
            }

            return list;
        }


        public List<string> GetExpectedReasonCodesForAllFlagsOnTheLine(string action, bool internalUser = true)
        {
            List<string> list = new List<string>();
            if (internalUser)
                {
                    if (action == "No Action")
                        list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnTheLineNoAction,"HCI", "H")).ToList();
                    else
                        list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnTheLine,action, "HCI", "H")).ToList();
                }

                else
                {
                    if (action == "No Action")
                        list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnTheLineNoAction,"CLT", "C")).ToList();
                    else
                        list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExpectedReasonCodesForAllFlagsOnTheLine,action, "CLT", "C")).ToList();
                }
            return list;
        }

        public void ClickOnWorkButton(bool delete = false)
        {
            string workButton;
            string work;
            if (delete)
            {
                work = "Restore";
                workButton = ClaimActionPageObjects.LineRestoreCssLocator;

            }
            else
            {
                work = "Delete";
                workButton = ClaimActionPageObjects.LineDeleteCssLocator;

            }

            Task clickWorkButton = Task.Factory.StartNew(() =>
                SiteDriver.FindElement(workButton, How.CssSelector).Click());
            clickWorkButton.ContinueWith(tskPrev => StringFormatter.PrintMessage("Click on Line " + work + " button"))
                .Wait();
        }

        /// <summary>
        /// Determines if delete option is enabled or restore
        /// </summary>
        /// <returns></returns>
        public bool IsFlagDeletable()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.FlaggedLineCssLocator, How.CssSelector).GetAttribute("Class").Contains("deleted");
        }

        public bool IsReasonCodeErrorFieldElementPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.FieldErrorXPath, How.XPath);
        }

       
        #endregion

        /// <summary>
        /// Return true if delete all flags present
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteAllFlagsPresent(bool checkEnabled = false)
        {
            if (SiteDriver.IsElementPresent(ClaimActionPageObjects.DeleteAllFlagsCssLocator, How.CssSelector))
            {
                if (checkEnabled)
                    return
                        SiteDriver.FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                            How.CssSelector).Enabled;
                return
                    SiteDriver.FindElement(ClaimActionPageObjects.DeleteAllFlagsCssLocator,
                        How.CssSelector).Displayed;
            }
            return false;
        }

        /// <summary>
        /// Return true if delete all flags present
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteFlagsIconPresent()
        {
            if (SiteDriver.IsElementPresent(ClaimActionPageObjects.DeleteFlagIconCssLocator, How.CssSelector))
                return
                    SiteDriver.FindElement(ClaimActionPageObjects.DeleteFlagIconCssLocator,
                        How.CssSelector).Displayed;
            return false;
        }

        public void ClickOnDeleteFlagIcon()
        {
            RemoveLock();
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.DeleteFlagIconCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("First Flag Deleted");
        }

        public bool IsRestoreFlagIconEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.RestoreFlagIconCssLocator, How.CssSelector);

        }

        public void ClickOnRestoreFlagIcon()
        {
            RemoveLock();
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.RestoreFlagIconCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("First Flag Restored");
        }

        /// <summary>
        /// Return true if delete all flags present
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteLineIconPresent()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DeleteLineIconCssLocator, How.CssSelector)
                .Displayed;
        }

        public bool IsRestoreLineIconPresent()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.RestoreLineIconCssLocator, How.CssSelector)
                .Displayed;
        }

        public int GetCountOfAllFlags()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.FlagRowCssLocator, How.CssSelector);
        }

        public int GetCountOfRestoreFlags()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.FlaggedLinesCssLocator, How.CssSelector);
        }

        public int GetCountOfDeletedFlags()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.DeletedFlagRowCssSelector, How.CssSelector);
        }

        public void ClickOnRestoreAllFlagsIcon()
        {
            RemoveLock();
            var element = SiteDriver.FindElement(ClaimActionPageObjects.RestoreFlagsIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            if (IsPageErrorPopupModalPresent())
            {
                ClosePageError();
                RemoveLock();
                SiteDriver
                    .FindElement(ClaimActionPageObjects.RestoreFlagsIconCssLocator, How.CssSelector).Click();
            }
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
            Console.WriteLine("Clicked on Restore All Flags Icon");

        }

        public bool IsRestoreSpecificLineEditFlagByClientPresent(string line, string edit, int row = 3,
            bool internalUser = false)
        {
            if (internalUser)
                    return SiteDriver.IsElementPresent(
                        Format(ClaimActionPageObjects.RestoreDeletedFlagForSpecificLineAndEditXPathTemplate,
                            line,
                            edit, row), How.XPath);
                else
                {
                    return SiteDriver.IsElementPresent(
                        Format(ClaimActionPageObjects.RestoreDeletedFlagForSpecificLineAndEditClientXPathTemplate,
                            line,
                            edit), How.XPath);
                }
           
        }
    

        public void RestoreSpecificLineEditFlagByClient(string line, string edit, int row = 3, bool internalUser = false)
        {
            if (IsFlagDeletedForLineEditClient(line, edit,row, internalUser))
            {
                if (internalUser)
                    SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.RestoreDeletedFlagForSpecificLineAndEditXPathTemplate,
                            line,
                            edit, row), How.XPath).Click();
                else
                {
                    SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.RestoreDeletedFlagForSpecificLineAndEditClientXPathTemplate,
                            line,
                            edit), How.XPath).Click();
                }
                Console.WriteLine("Restored Flag at line: " + line + ", Edit: " + edit);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => IsDeleteFlagPresentForLineEdit(line, edit));
            }

            Console.WriteLine("No deleted flag found at line: " + line + ", Edit: " + edit);
        }

        public void CloseWorkList()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.CloseWorkListBtnCssLocator, How.CssSelector).Click();
            _sideBarPanelSearch.WaitSideBarPanelClosed();
        }

        public string GetValueOfLowerRightQuadrant()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.RightQuadrantLabelXPath, How.XPath).Text;
        }

        public void ClickOnEditRecord()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimLineEditIconXpath,How.XPath).Click();
        }

        public bool IsEditAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditClaimLineRegionCssSelector,
                How.CssSelector);
        }

        public bool IsEditNoteAreaPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimLineEditNotesRegionCssSelector,
                How.CssSelector);
        }

        public void ClickOnEditNotesIcon()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClaimLineEditNotesIconXPath, How.XPath);
        }

        public string GetValueOfLowerRightQuadrantClaimDollar()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.RightQuadrantLabelClaimDollarCss, How.CssSelector)
                .Text;
        }

        public string GetDosTooltipValue()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DosTextLabelCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetTextColorOfDosValue()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DosTextLabelCssLocator, How.CssSelector).GetCssValue("color");
        }

        public string GetLockIConTooltip()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.LockIconCssSelector, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetDeleteAllIconTooltip()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DeleteAllFlagsIconCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetApproveIconTooltip()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ApproveClaimIconCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetNextIconTooltip()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.NextIconXPath, How.XPath)
                .GetAttribute("title");
        }

        public string GetEditIconTooltip()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.EditIconCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetAddIconTooltip()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FlagAddIconCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetTransferIconTooltip()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TransferIconCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public string GetProviderSearchNotesIconTooltip()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ProviderSearchNotesIconCssSelector,
                    How.CssSelector)
                .GetAttribute("title");
        }

        public string GetTransferIconTooltipMovedToLeft()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TransferOptionMovedToLeftXPath, How.XPath)
                .GetAttribute("class");
        }

        public string GetLogicIconMovedToLeft()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.ClientLogicIconCssLocator, How.CssSelector)
                .GetAttribute("class");
        }

        public string GetWorkListHeaderText()
        {
            SiteDriver.WaitForCondition(IsWorkListControlDisplayed);
            return SiteDriver
                .FindElement(ClaimActionPageObjects.WorkListHeaderCssLocator, How.CssSelector).Text;
        }

        public bool IsCvQcClaimsPresentInWorkListPanel()//IsPciQaWorkListPresentInWorkListPanel()
        {
            return SiteDriver.IsElementPresent("//span[text()='CV QC Claims']", How.XPath);
        }

        public string GetDosValue()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DosTextLabelCssLocator, How.CssSelector).Text;
        }

        public List<string> GetAllDosValues()
        {
            var claimLineCount = GetCountOfClaimLines();
            List<string> allDos = new List<string>();

            for(int i = 1 ; i <= claimLineCount ; i++)
            {
                allDos.Add(GetClaimLineDetailsValueByLineNo(i, 1, 3));
            }

            return allDos;
        }
            

        public bool IsSugUnitTextFieldEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SugUnitInputFieldCssLocator, How.CssSelector);
        }

        public void EnterSugUnits(string value)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugUnitInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugUnitInputFieldCssLocator, How.CssSelector).SendKeys(value);
            SiteDriver.WaitToLoadNew(100);
            Console.WriteLine("Entered the Sug units: " + value);
        }

        public string GetSugUnits()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.SugUnitInputFieldCssLocator,
                        How.CssSelector)
                    .GetAttribute("value");
        }

        public bool IsSugPaidTextFieldEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SugPaidInputFieldCssLocator, How.CssSelector);
        }

        public bool IsSugCodeTextFieldEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SugCodeInputFieldCssLocator, How.CssSelector);
        }

        public bool IsAddAppealIconEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddAppealIconCssSelector, How.CssSelector);
        }
        
        public bool IsAddAppealIconDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledAddAppealIconCssSelector,
                How.CssSelector);
        }

        public string GetToolTipMessageDisabledCreateAppealIcon()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.DisabledAddAppealIconCssSelector,
                    How.CssSelector).GetAttribute("title");
        }

        public bool IsNextClaimsInWorklistSectionDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.NextClaimsInWorklistXPath, How.XPath).Displayed;
        }

        public bool IsListOfNextClaimsInWorklistSectionDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimListInNextClaimsInWorklistXPath,
                How.XPath);
        }

        public List<string> GetListOfNextClaimsInWorklistSectionDisplayed()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimListInNextClaimsInWorklistXPath, How.XPath,
                "Text");
        }

        public bool IsNoDataTextInNextClaimsInWorklistSectionDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.NoDataTextInNextClaimsInWorklistXPath,
                How.XPath);
        }

        public string GetNoDataTextInNextClaimsInWorklistSectionDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.NoDataTextInNextClaimsInWorklistXPath, How.XPath)
                .Text;
        }

        public bool IsAdditionalLockedClaimsSectionDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.AdditionalLockedClaimsSectionXPath, How.XPath).Displayed;
        }

        public bool IsPreviouslyViewedClaimsSectionDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PreviouslyViewedClaimsSectionXPath, How.XPath).Displayed;
        }

        public string GetVersionText()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.VersionTextId, How.Id).Text.Trim();
        }

        public string GetNextClaimsInWorklistSectionHeader()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.NextClaimsInWorklistSectionHeaderXPath, How.XPath).Text;
        }

        public string GetAdditionalLockedClaimsSectionHeader()
        {
            var element = SiteDriver
                .FindElement(ClaimActionPageObjects.AdditionalLockedClaimsSectionHeaderXPath, How.XPath);
            return element.Text;
        }

        public string GetPrevioulsyViewedClaimsSectionHeader()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.PreviouslyViewedClaimsSectionHeaderXPath, How.XPath).Text;
        }

        /// <summary>
        /// Return true if worklist control is present
        /// </summary>
        /// <returns></returns>
        public bool IsWorkListControlDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.WorkListControlCssLocator, How.CssSelector);
        }

        /// <summary>
        /// Return string value of the closed status of the worklist control
        /// </summary>
        public string IsWorkListControlClosed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.WorkListControlCssLocator, How.CssSelector).GetAttribute("class");
        }

        public bool IsFlaggedLineDeleted()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.FlaggedLineDeletedRowXPath, How.XPath);
        }

        public bool IsFlaggedLineNotDeleted()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.FlaggedLineRowXPath, How.XPath);
        }

        public bool IsSwitchIconDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DisabledSwitchButtonXPath, How.XPath);
        }

        public bool IsSwitchIconDisplayed()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.SwitchButtonXPath, How.XPath)
                .Displayed;
        }

        public void ClickOnFirstFlaggedLineDeleteButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.DeleteButtonXPath, How.XPath).Click();
        }

        public void ClickOnFirstFlaggedLineRestoreButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.RestoreButtonXPath, How.XPath).Click();
        }

        public void ClickOnSecondFlaggedLineRestoreButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.SecondRestoreButtonXPath, How.XPath).Click();
        }

        public void ClickOnDeleteRestoreAllFlagsOnLineButton(string line)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.DeleteAllFlagsOnLineXPathTemplate, line), How.XPath).Click();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        }

        /// <summary>
        /// Return true if page is blank.
        /// </summary>
        /// <returns></returns>
        public bool IsBlankPagePresent()
        {
            return !SiteDriver.IsElementPresent(ClaimActionPageObjects.NewClaimActionDataHolderDivCssLocator,
                How.CssSelector);
        }

        public void WaitForBlankPage()
        {
            SiteDriver.WaitForCondition(IsBlankPagePresent);
        }

        public bool IsSearchIconDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.FindIconCssLocator, How.CssSelector).Displayed;
        }

        public bool IsClaimSequenceLabelDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceLabelCssLocator, How.CssSelector).Displayed;
        }

        public bool IsClaimSequenceInputFieldDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceXPath, How.XPath).Displayed;
        }

        public bool IsClaimNoLabelDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimNoLabelCssLocator, How.CssSelector).Displayed;
        }

        public bool IsClaimNoInputFieldDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimNoInputXPath, How.XPath).Displayed;
        }

        public bool IsTransferOptionPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.TransferOptionXPath, How.XPath);
        }

        public bool IsTransferApproveOptionPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.TransferApproveOptionXPath, How.XPath);
        }

        public bool IsTrasnferApproveIconDisabled()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TransferApproveCssSelector, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsAddNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddNotesIndicatorCssSelector, How.CssSelector);
        }

        public bool IsProviderDetailIconWithNoBadgePresent()
        {
            return IsProviderDetailsIconPresent() && !IsBadgePresentInProviderDetailIcon();
        }

        #region notes

        public bool IsViewNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ViewNotesIndicatorCssSelector,
                How.CssSelector);
        }

        public bool IsRedBadgeNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.RedBadgeNotesIndicatorCssSelector,
                How.CssSelector);
        }

        public string NoOfClaimNotes()
        {
            return !IsRedBadgeNoteIndicatorPresent()
                ? "0"
                : JavaScriptExecutor.FindElement(ClaimActionPageObjects.RedBadgeNotesIndicatorCssSelector).Text;
        }

        public List<string> GetAvailableDropDownListInNoteType(string label)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            Console.WriteLine("Looking for <{0}> List", label);
            SiteDriver.WaitToLoadNew(200);
            var list = JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.NoteTypeDropDownInputListByLabelXPathTemplate, label),
                How.XPath, "Text");
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            Console.WriteLine("<{0}> Drop down list count is {1} ", label, list.Count);
            return list;
        }

        public string GetDefaultValueOfNoteTypeOnHeader(string label)
        {
            return JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label))
                .GetAttribute("value");
        }

        public String GetNoteRecordByRowColumn(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.NoteRecordsByRowColCssTemplate, col, row),
                    How.CssSelector).Text;
        }

        public List<String> GetNoteRecordListByColumn()
        {

            return
                JavaScriptExecutor.FindElements(
                    Format(ClaimActionPageObjects.NoteRecordsListByColCssTemplate, 2),
                    How.CssSelector, "Text");

        }

        public bool IsPencilIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.NotePencilIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsPencilIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.NotePencilIconByNameCssTemplate, name));
        }


        public bool IsCarrotIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.NoteCarrotIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsCarrotIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.NoteCarrotIconByNameCssTemplate, name));
        }

        public bool IsVisibletoClientIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.VisibleToClientIconInNoteContainerByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsVisibletoClientIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.VisibleToClientIconInNoteRowByNameCssTemplate, name));
        }

        public bool IsNoteContainerPresent()
        {
            return SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.NoteContainerCssLocator),
                How.CssSelector);
        }

        public void SelectNoteTypeInHeader(string label, string value)
        {
            var element =
                JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            if (
                !SiteDriver.IsElementPresent(
                    Format(ClaimActionPageObjects.NoteTypeDropDownInputListByLabelXPathTemplate, label,
                        value), How.XPath))
                JavaScriptExecutor.ClickJQuery(
                    Format(ClaimActionPageObjects.InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.NoteTypeDropDownInputListByLabelXPathTemplate, label, value),
                How.XPath);
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);

        }

        public int GetNoteEditFormCount()
        {
            return SiteDriver.FindElementsCount(
                ClaimActionPageObjects.NotesEditFormCssLocator, How.CssSelector);
        }

        public int GetNoteListCount()
        {
            return SiteDriver.FindElementsCount(
                ClaimActionPageObjects.NotesListCssLocator, How.CssSelector);

        }


        public bool IsVerticalScrollBarPresentInNoteSection()
        {
            const string select = ClaimActionPageObjects.NotesSectionWithScrollBarCssLocator;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("clientName Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public bool IsVerticalScrollBarPresentInNoteAreaByRow(int row)
        {
            string select = Format(ClaimActionPageObjects.NotesTextAreaByRowBarCssLocator, row);
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("clientName Height:" + GetClientHeight(select));
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

        public string GetVisibleToClientTooltipInNotesList()
        {
            return JavaScriptExecutor.FindElement(ClaimActionPageObjects.VisibleToClientIconInNoteRowCss)
                .GetAttribute("title");
        }

        public void ClickOnEditIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NotePencilIconByRowCssTemplate,
                row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Edit Icon");
        }

        public void ClickOnEditIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NotePencilIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(() => IsNoteEditFormDisplayedByName(name));
            Console.WriteLine("Clicked On Small Edit Icon");
        }

        public void ClickOnExpandIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NoteCarrotIconByRowCssTemplate,
                row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Exapnd Icon");
        }

        public void ClickOnExpandIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NoteCarrotIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(() => IsNoteEditFormDisplayedByName(name));
            Console.WriteLine("Clicked On Small Exapnd Icon");
        }

        public void ClickOnCollapseIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NoteCarrotDownIconByRowCssTemplate,
                row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Collaspe Icon");
        }

        public void ClickOnCollapseIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimActionPageObjects.NoteCarrotDownIconByNameCssTemplate,
                name));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            Console.WriteLine("Clicked On Small Collaspe Icon");
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

        public bool IsNoteEditFormDisplayedByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.NotesEditFormByRowXpath, row),
                How.XPath);

        }

        public bool IsNoteEditFormDisplayedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.NotesEditFormByNameCssSelector, name));

        }

        public void ClickOnSaveButtonInNoteEditorByRow(int row)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.NotesEditFormSaveButtonCssLocator, row), How.CssSelector)
                .Click();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void ClickOnSaveButtonInNoteEditorByName(string name)
        {
            SiteDriver.WaitToLoadNew(2000);
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.NotesEditFormSaveButtonByNameCssLocator, name));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void ClickOnCancelButtonInNoteEditorByRow(int row)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.NotesEditFormCancelButtonCssLocator, row), How.CssSelector)
                .Click();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void ClickOnCancelButtonInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.NotesEditFormCancelButtonByNameCssLocator, name));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void CheckVisibleToClientInNoteEditorByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.VisibleToClientCheckBoxInNoteEditorByRowCssLocator, row));
        }

        public void ClickVisibleToClientCheckboxInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(
                Format(ClaimActionPageObjects.VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));
        }

        public bool IsVisibleToClientInNoteEditorSelectedByRow(int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator,
                    row), How.CssSelector);

        }

        public bool IsVisibleToClientInNoteEditorSelectedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(
                ClaimActionPageObjects.SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }

        public bool IsVisibleToClientCheckboxPresentInNoteEditorByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClaimActionPageObjects.VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }

        public bool IsNoNotesMessageDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.NoNotesMessageCssLocator, How.CssSelector);

        }

        public string GetNoNotesMessage()
        {
            return JavaScriptExecutor.FindElement(ClaimActionPageObjects.NoNotesMessageCssLocator).Text;
        }

        public void ClickonAddNoteIcon()
        {

            var element = SiteDriver.FindElement(ClaimActionPageObjects.NotesAddIconCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Add Icon");
        }

        public bool IsAddNoteIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.NotesAddIconCssSelector, How.CssSelector);
        }

        public void ClickonAddNoteSaveButton()
        {

            var element = SiteDriver.FindElement(ClaimActionPageObjects.AddNoteSaveButtonCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Save Button");
        }

        public void ClickOnAddNoteCancelLink()
        {
            var element = SiteDriver.FindElement(ClaimActionPageObjects.AddNoteCancelLinkCssSelector, How.CssSelector);
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


       
        public bool IsAddNoteFormPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddNoteFormCssSelector, How.CssSelector);
        }

        public bool IsAddIconDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddIconDisabledCssLocator, How.CssSelector);
        }

        public bool IsVisibleToClientChecked()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.CheckedVisibleToClientXpath, How.XPath);
        }

        public void ClickVisibleToClient()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.VisibleToClientCssLocator, How.CssSelector)
                .Click();
            WaitForStaticTime(1000);
        }

        public bool IsVisibleToClientCheckboxPresentInAddNoteForm()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.VisibleToClientCssLocator, How.CssSelector);
        }

        public bool IsSubTypeDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.SubTypeDisabledXPath, How.XPath);
        }

        //public bool IsNoteIndicatorpresent()
        //{
        //    return SiteDriver.IsElementPresent(NewClaimActionPageObjects.AddNoteExclamationXpath, How.XPath);
        //}

        public void DeleteClaimNotesofNoteTypeClaimOnly(string claSeq, string userName)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimNotesofNoteTypeClaimOnly,
                claSeq.Split('-')[0], claSeq.Split('-')[1], userName));
            Console.WriteLine("Delete Note of type claim from d if exists for claseq<{0}>  ", claSeq, userName);
        }

        public string TotalCountOfNotes(string claSeq, string prvSeq, string patSeq)
        {
            List<string> claseq = new List<string>() {claSeq.Split('-')[0], claSeq.Split('-')[1]};

            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfNotes, claseq[0],
                prvSeq, patSeq));
            Console.WriteLine("Total count of Claim and Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofClaimAndPatientNotes(string claSeq, string patSeq)
        {
            List<string> claseq = new List<string>() { claSeq.Split('-')[0], claSeq.Split('-')[1] };
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfClaimAndPatientNotes,
                claseq[0], patSeq));
            Console.WriteLine("Total count of Claim and Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofClaimNotes(string claSeq)
        {
            List<string> claseq = new List<string>() { claSeq.Split('-')[0], claSeq.Split('-')[1] };
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfClaimNotes,
                claseq[0], claseq[1]));
            Console.WriteLine("Total count of Claim and Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofClaimDocsFromDatabase(string claSeq)
        {
            var count = Executor.GetSingleValue(Format(ClaimSqlScriptObjects.TotalCountOfClaimDocuments,
                claSeq.Split('-')[0], claSeq.Split('-')[1])).ToString();
            Console.WriteLine("Total count of Provider notes is <{0}>", count);
            return count;
        }

        public string TotalCountofProviderNotes(string prvSeq)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfProviderNotes,
                prvSeq));
            Console.WriteLine("Total count of Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();
        }


        

        public void SelectNoteType(string noteType)
        {
            SiteDriver.FindElement(ClaimActionPageObjects.TypeInputXPath, How.XPath).Click();
            SiteDriver.WaitToLoadNew(500);
            if (!SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.TypeListValueXPathTemplate, noteType), How.XPath))
                SiteDriver.FindElement(ClaimActionPageObjects.TypeInputXPath, How.XPath).Click();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.TypeListValueXPathTemplate, noteType), How.XPath).Click();
            Console.WriteLine("NoteType Selected: <{0}>", noteType);
        }

        public void SelectNoteSubType(string noteSubType)
        {

            SiteDriver.FindElement(ClaimActionPageObjects.SubTypeInputXPath, How.XPath).Click();
            SiteDriver.WaitToLoadNew(500);
            if (!SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.SubTypeListValueXPathTemplate, noteSubType), How.XPath))
                SiteDriver.FindElement(ClaimActionPageObjects.SubTypeInputXPath, How.XPath).Click();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.SubTypeListValueXPathTemplate, noteSubType), How.XPath).Click();
            Console.WriteLine("NoteSubType Selected: <{0}>", noteSubType);

        }

        public string GetNameLabel()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.NameLabelXPath, How.XPath).Text;
        }

        #endregion

        public bool IsTransferClaimsWidgetDisplayed()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.TransferClaimWidgetCssLocator,
                    How.CssSelector).Displayed;
        }

        public string GetTransferWidgetHeader()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.TransferClaimWidgetHeaderCssLocator,
                How.CssSelector).Text;
        }

        public void EnterSugCode(string sugCode)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugCodeInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugCodeInputFieldCssLocator, How.CssSelector).SendKeys(sugCode);
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugCodeInputFieldCssLocator, How.CssSelector).SendKeys(Keys.Tab);
            Console.WriteLine("Entered the sug code: " + sugCode);
        }

        public void EnterSugPaid(string sugPaid)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugPaidInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver
                .FindElement(ClaimActionPageObjects.SugPaidInputFieldCssLocator, How.CssSelector).SendKeys(sugPaid);
            Console.WriteLine("Enter the sug paid: " + sugPaid);
        }

        public ClaimActionPage ClickOnTransferDropdown()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(ClaimActionPageObjects.TransferDropDownBtnCssLocator,
                    How.CssSelector).Click();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.TransferDropDownOptionsDivCssLocator,
                        How.CssSelector));
                Console.WriteLine("Clicked on Transfer Dropdown");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickOnTransferButton()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.TransferCssSelector, How.CssSelector).Click();
            Console.WriteLine("Clicked on Transfer Button.");
        }

        public void ClickOnTransferApproveButton()
        {
            RemoveLock();
            SiteDriver
                .FindElement(ClaimActionPageObjects.TransferApproveCssSelector, How.CssSelector).Click();
            Console.WriteLine("Clicked on TransferApprove Button.");
        }

        public void ScrollToViewWorkListQuery()
        {
            JavaScriptExecutor.ExecuteToScrollToView("worklist_section worklist_previous");
        }

        public void ScrollToQaClaimListByRow(int row)
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDivUsingJquery($"div.claim_line:nth-of-type({row}) span.small_edit_icon");
        }

        public string GetNextClaimOfQueue()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.NextClaimInWorkListQueueXPath, How.XPath)
                .Text;
        }

        public ClaimActionPage ClickOnReturnToClaim(string claimSeq)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.ReturnToClaimXPathTemplate, claimSeq), How.XPath).Click();
                Console.WriteLine("Clicked on return to claim sequence: " + claimSeq);
                WaitForWorkingAjaxMessage();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SelectTransferOption()
        {
            return SelectTransferOption(ClaimActionPageObjects.TransferOptionXPath);
        }

        public ClaimActionPage SelectTransferApproveOption()
        {
            return SelectTransferOption(ClaimActionPageObjects.TransferApproveOptionXPath);
        }

        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.CancelWidgetCssSelector, How.CssSelector);
        }

        public void ClickOnFlagLevelCancelLink()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.FlagLevelCancelLinkCssLocator, How.CssSelector)
                .Click();
        }

        public void ClickOnCancelEditFlagLink()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.CancelEditFlagLinkCssLocator, How.CssSelector);
        }

        public ClaimActionPage SaveFlag()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.SaveSugButtonCssLocator, How.CssSelector).Click();
                Console.WriteLine("Clicked the Save Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SaveFlag(string reasonCode, string claseq = null)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.SaveSugButtonCssLocator, How.CssSelector).Click();
                WaitForWorking();
                if (IsPageErrorPopupModalPresent())
                {
                    ClosePageError();
                    SelectReasonCode(reasonCode);
                    if (claseq != null)
                    {
                        ClickWorkListIcon();
                        ClickSearchIcon();
                        SearchByClaimSequence(claseq);
                        ClickWorkListIcon();
                    }
                    SiteDriver
                        .FindElement(ClaimActionPageObjects.SaveSugButtonCssLocator, How.CssSelector).Click();
                }
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked the Save Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SaveFlagAndClickFlagAudiHistory(string reasonCode, int lineNo, string flag, string claseq)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.SaveSugButtonCssLocator, How.CssSelector).Click();
                WaitForWorking();
                if (IsPageErrorPopupModalPresent())
                {
                    ClosePageError();
                    SelectReasonCode(reasonCode);
                    if (claseq != null)
                    {
                        GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(claseq, EnvironmentManager.Username);
                        ClickWorkListIcon();
                        ClickSearchIcon();
                        SearchByClaimSequence(claseq);
                        ClickWorkListIcon();
                        ClickOnClaimFlagAuditHistoryIcon();
                    }
                    SiteDriver
                        .FindElement(ClaimActionPageObjects.SaveSugButtonCssLocator, How.CssSelector).Click();
                }
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked the Save Button");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #region LogicManagement

        public void ClickOnAddLogicIconByRow(int row)
        {
            var element = SiteDriver.FindElement(Format(ClaimActionPageObjects.AddLogicIconCssSelectorByRow, row),
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
            SiteDriver.WaitForCondition(() => IsLogicWindowDisplay());
            WaitForStaticTime(500);
        }

        public void ClickOnLogicIconWithLogicByRow(int row,bool close=false)
        {
            SiteDriver.FindElement(Format(ClaimActionPageObjects.LogicIconWithLogicByRowCssTemplate, row), How.CssSelector).Click();
            WaitForWorking();
            if(!close)
                SiteDriver.WaitForCondition(() => IsLogicWindowDisplayByRowPresent(row));
            else
                SiteDriver.WaitForCondition(() => !IsLogicWindowDisplayByRowPresent(row));
        }

        public bool IsLogicFormText(string text) => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.LogicFormXpathTemplate, text), How.XPath);

        public bool IsLogicWindowDisplay() => SiteDriver.IsElementPresent(ClaimActionPageObjects.LogicFormCssLocator, How.CssSelector);

        public bool IsLogicWindowDisplayByRowPresent(int row) => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.LogicFormByRowCssTemplate, row), How.CssSelector);

        public bool IsClientLogicIconByRowPresent(int row) => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.ClientLogicIconByRowCssTemplate, row), How.CssSelector);

        public bool IsInternalLogicIconByRowPresent(int row) => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.InternalLogicIconByRowCssTemplate, row), How.CssSelector);

        public void  ClickOnLogicIcon(int row, string userType = "")
        {
           
            switch (userType)
                {
                    case UserType.HCIUSER:
                        SiteDriver.FindElement(ClaimActionPageObjects.CotivitiLogicIconCssLocator, How.CssSelector).Click();
                        break;
                    case UserType.CLIENT:
                        SiteDriver.FindElement(Format(ClaimActionPageObjects.ClientLogicIconCssSelectorByRow, row), How.CssSelector).Click(); 
                        break;
                    default:
                    SiteDriver.FindElement(Format(ClaimActionPageObjects.AddLogicIconCssSelectorByRow, row), How.CssSelector).Click();
                    break;
                }

            WaitForSpinner();
            
        }

        public void AddLogicMessageTextarea(string message)
        {
            SiteDriver.FindElement(ClaimActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(ClaimActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).SendKeys(message);
            WaitForStaticTime(500);
        }

        public bool VerifyMaxSizeOfLogicMessageTextArea(int maxLength = 500)
        {
            string message = RandomString(maxLength + 2);
            SiteDriver.FindElement(ClaimActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(ClaimActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).SendKeys(message);
            bool result = GetLogicMessageTextarea().Length == maxLength ? true : false;
            return result;
        }

        public string GetLogicMessageTextarea()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.LogicMessageTextareaCssLocator, How.CssSelector).GetAttribute("value");            
        }

        public string GetRecentLeftLogicMessage()
        {
            return JavaScriptExecutor
                .FindElements(ClaimActionPageObjects.LeftMessaageSectionCssLocator, How.CssSelector, "Text")
                .LastOrDefault();
        }

        public string GetRecentRightLogicMessage()
        {
            return JavaScriptExecutor
                .FindElements(ClaimActionPageObjects.RightMessaageSectionCssLocator, How.CssSelector, "Text")
                .LastOrDefault();
        }
      
        public bool IsLogicPlusIconDisplayed(int row) => SiteDriver.IsElementPresent(Format(ClaimActionPageObjects.AddLogicIconCssSelectorByRow, row), How.CssSelector);

        public LogicManagementPage ClickOnLogicIcon(string userType = "")
        {
            var logicManagement = Navigator.Navigate<LogicManagementPageObjects>(() =>
            {
                switch (userType)
                {
                    case UserType.HCIUSER:
                        if(SiteDriver
                            .IsElementPresent(ClaimActionPageObjects.CotivitiLogicIconCssLocator, How.CssSelector))
                            SiteDriver.FindElement(ClaimActionPageObjects.CotivitiLogicIconCssLocator, How.CssSelector).Click();
                            SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue());
                        break;
                    case UserType.CLIENT:
                        if (SiteDriver
                            .IsElementPresent(ClaimActionPageObjects.ClientLogicIconCssLocator, How.CssSelector))
                            SiteDriver.FindElement(ClaimActionPageObjects.ClientLogicIconCssLocator, How.CssSelector).Click();
                            SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue());
                        break;
                    default:
                        if (SiteDriver
                            .IsElementPresent(ClaimActionPageObjects.AddLogicIconCssLocator, How.CssSelector))
                            SiteDriver.FindElement(ClaimActionPageObjects.AddLogicIconCssLocator, How.CssSelector).Click();
                            SiteDriver.SwitchWindowByTitle(PageTitleEnum.LogicManagementPage.GetStringValue());
                        break;
                }
            });
            return new LogicManagementPage(Navigator, logicManagement, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor); 
        }

        public string GetLogicAssignedToValue()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.AssignedToValueCssSelector, How.CssSelector)
                .Text;
        }

        #endregion
        public Boolean IsDisabledLogicIconPresent()
        {
                    return (SiteDriver.IsElementPresent(ClaimActionPageObjects.LockedAddLogicIconCssLocator, How.CssSelector)
                            || SiteDriver.IsElementPresent(ClaimActionPageObjects.LockedCotivitiLogicIconCssLocator,
                                How.CssSelector)
                            || SiteDriver.IsElementPresent(ClaimActionPageObjects.LockedClientLogicIconCssLocator,
                                How.CssSelector))
                           &&
                           !(SiteDriver.IsElementPresent(ClaimActionPageObjects.AddLogicIconCssLocator, How.CssSelector)
                             || SiteDriver.IsElementPresent(ClaimActionPageObjects.CotivitiLogicIconCssLocator,
                                 How.CssSelector)
                             || SiteDriver.IsElementPresent(ClaimActionPageObjects.CotivitiLogicIconCssLocator,
                                 How.CssSelector));
        }

        public string GetTriggerClaimData()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TriggerClaimDataCssLocator, How.CssSelector).Text.Trim();
        }

        public string[] GetDxLineData()
        {
            var coll = new string[4];
            coll[0] = SiteDriver
                .FindElement(ClaimActionPageObjects.DxCodeLabelCssLocator, How.CssSelector).Text;
            coll[1] = SiteDriver
                .FindElement(ClaimActionPageObjects.DxCodeDataCssLocator, How.CssSelector).Text;
            coll[2] = SiteDriver
                .FindElement(ClaimActionPageObjects.DxVersionDataCssLoctor, How.CssSelector).Text;
            coll[3] = SiteDriver
                .FindElement(ClaimActionPageObjects.DxDescriptionDataCssLocator, How.CssSelector).Text;
            return coll;
        }

        public ClaimActionPage ClickOnFlagLineWithTriggerClaim(string claimSeq)
        {
            _claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(
                    Format(
                        ClaimActionPageObjects
                            .FlagLineWithTriggerClaimXPath,
                        claimSeq), How.XPath).Click();
                Console.Out.WriteLine(
                    "Clicked flag line with trigger claim : {0}",
                    claimSeq);
            });
            return new ClaimActionPage(Navigator, _claimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Click on notes link
        /// </summary>
        
        public NotesPopupPage ClickOnClaimNotes(String clmSequence)
        {
            String clmSeq = clmSequence.Remove(clmSequence.IndexOf('-'));
            var isAdddNoteIconPresent = SiteDriver
                .FindElement(ClaimActionPageObjects.NotesLinkXPath, How.XPath).GetAttribute("class").Contains("add_notes");
            var claimNotePage = Navigator.Navigate(() =>
            {
                SiteDriver
                    .FindElement(ClaimActionPageObjects.NotesLinkXPath, How.XPath).Click();
                Console.WriteLine("Clicked Notes Link");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimNotes.GetStringValue()));
                if (isAdddNoteIconPresent)
                    SiteDriver.WaitForCondition(
                        () =>
                            SiteDriver.IsElementPresent(NotesPopupPageObjects.CreateNoteSectionCssSelector,
                                How.CssSelector));
                else
                {
                    SiteDriver.WaitForCondition(
                        () =>
                            SiteDriver.IsElementPresent(NotesPopupPageObjects.NotesGrid,
                                How.XPath));
                    SiteDriver.WaitForCondition(
                        () =>
                            !SiteDriver.IsElementPresent(NotesPopupPageObjects.LoadingImageIconCssLocator,
                                How.CssSelector));
                }
            }, () => new NotesPopupPageObjects("retro/Notes/Claim/" + clmSeq + "?"));
            return new NotesPopupPage(Navigator, claimNotePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public void ClickOnClaimNotes()
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.NotesLinkXPath, How.XPath).Click();
            WaitForWorkingAjaxMessage();
        }

public NotesPopupPage ClickOnProviderSearchNotesIcon(String provSequence)
{
    var providerNotePage = Navigator.Navigate(() =>
    {
        SiteDriver
            .FindElement(ClaimActionPageObjects.ProviderSearchNotesIconCssSelector, How.CssSelector).Click();
        Console.WriteLine("Clicked Provider Search Notes Link");
        SiteDriver.WaitForCondition(
            () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.NotesPage.GetStringValue()));

    }, () => new NotesPopupPageObjects("retro/Notes/Provider/" + provSequence + "?"));
    return new NotesPopupPage(Navigator, providerNotePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
}

/// <summary>
/// Click on Menu Option
/// </summary>
/// <param name="menuOption"></param>
public void ClickOnMenuSpan(String menuOption)
        {
            var menuToClick = HeaderMenu.GetElementLocatorTemplateSpan(menuOption);
            JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            Console.WriteLine("Navigated to {0}", menuOption);
        }

        /// <summary>
        /// Click on Menu Option
        /// </summary>
        /// <param name="menuOption"></param>
        public void ClickOnMenu(String menuOption)
        {
            var menuToClick = HeaderMenu.GetElementLocatorTemplate(menuOption);
            JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
            Console.WriteLine("Navigated to {0}", menuOption);
        }

        /// <summary>
        /// Get text from Alert Box
        /// </summary>
        /// <returns></returns>
        public string GetAlertBoxText()
        {
            return Navigator.GetAlertBoxText();
        }

        /// <summary>
        /// Checks if Alert is present
        /// </summary>
        /// <returns></returns>
        public bool IsAlertBoxPresent()
        {
            return Navigator.IsAlertBoxPresent();
        }

        /// <summary>
        /// Accepts the alert
        /// </summary>
        public void AcceptAlertBoxIfPresent()
        {
            if (IsAlertBoxPresent())
            {
                Navigator.AcceptAlertBox();
                Console.WriteLine("Alert Box Accepted");
            }
            else
            {
                Console.WriteLine("Alert Box wasn't found.");
            }
        }

        /// <summary>
        /// Dismisses the alert
        /// </summary>
        public void DismissAlertBoxIfPresent()
        {
            if (IsAlertBoxPresent())
            {
                Navigator.DismissAlertBox();
                Console.WriteLine("Alert Box Dismissed");
            }
            else
            {
                Console.WriteLine("Alert Box wasn't found.");
            }
        }

       
        public void WaitForAjaxToLoad(string ajaxLibrary)
        {
            try
            {
                SiteDriver.WaitForAjaxToLoad(ajaxLibrary);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception caught while waiting: " + ex.Message);
            }
        }

        public bool IsExClamationIconPresent()
        {

            return SiteDriver.IsElementPresent(ClaimActionPageObjects.IsExClamationIconPresentCssSelector,
                How.CssSelector);
        }

        #region WORKLIST COMPONENET

        public IList<string> GetWorkListFiltersLabel()
        {
            IList<string> filters = JavaScriptExecutor.FindElements(ClaimActionPageObjects.FiltersLabelCssLocator,
                How.CssSelector, "Text");
            foreach (var filter in filters)
            {
                if (filter.Contains("\r"))
                {
                    filter.Replace(filter, filter.Split('\r')[0]);
                }
            }

            return filters;
        }

        public IList<string> GetWorkListFiltersSelectedOption()
        {
            return SiteDriver.FindDisplayedElementsText(ClaimActionPageObjects.FiltersSelectedOptionCssLocator,
                How.CssSelector);
        }

        public string GetValueByEntering(char text)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.InputReviewGroupXPath, How.XPath).ClearElementField();

            SiteDriver
                .FindElement(ClaimActionPageObjects.InputReviewGroupXPath, How.XPath).Click();
            SiteDriver
                .FindElement(ClaimActionPageObjects.InputReviewGroupXPath, How.XPath).SendKeys(text.ToString());
                    string reviewGroup = SiteDriver
                        .FindElement(ClaimActionPageObjects.ReviewGroupXPath, How.XPath).Text;
            SiteDriver
                .FindElement(ClaimActionPageObjects.InputReviewGroupXPath, How.XPath).ClearElementField();
            return reviewGroup;
        }

        #endregion

        #region SWITCH EDIT OPERTATION

        public string GetCurrentLineNumToSwitch()
        {
            try
            {
                return SiteDriver
                    .FindElement(ClaimActionPageObjects.CurrentLineNumForSwitchXPath, How.XPath).Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetCurrentEditToSwitch()
        {
            try
            {
                return SiteDriver.FindElement(ClaimActionPageObjects.SwitchEditXPath, How.XPath).Text;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetTriggerLine()
        {
            try
            {
                return SiteDriver.FindElement(ClaimActionPageObjects.TriggerLineValueXPath, How.XPath)
                    .Text;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public int GetDeletedLineCountWithSwitchOperation(string flaggedLine, string edit)
        {
            try
            {
                return
                    SiteDriver.FindElementsCount(
                        Format(ClaimActionPageObjects.DeletedLineForSpecificLineAndEditXPathTemplate,
                            flaggedLine,
                            edit), How.XPath);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetDeletedLineFlagCountForClient(string flaggedLine, string edit)
        {
            try
            {
                return
                    SiteDriver.FindElementsCount(
                        Format(ClaimActionPageObjects.DeletedLineForClientSpecificLineAndEditXPathTemplate,
                            flaggedLine,
                            edit), How.XPath);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool IsFlagDeletedForLineEditClient(string flagLine, string edit, int row = 3, bool internalUser = false)
        {
            if (internalUser)
                return SiteDriver.FindElement(Format(
                    ClaimActionPageObjects.SpecificLineAndEditXPathTemplate, flagLine,
                    edit, row), How.XPath).GetAttribute("class").Contains("is_deleted");
            else
                return SiteDriver.FindElement(Format(
                ClaimActionPageObjects.SpecificLineAndEditClientXPathTemplate, flagLine,
                edit), How.XPath).GetAttribute("class").Contains("is_deleted");
        }

        public bool IsFlagDeletedForLineEdit(string flagLine, string edit, int row = 3)
        {
            return SiteDriver.FindElement(Format(
                ClaimActionPageObjects.SpecificLineAndEditXPathTemplate, flagLine,
                edit, row), How.XPath).GetAttribute("class").Contains("is_deleted");
        }

        public bool IsDeleteFlagDisabledForLineEdit(string flagLine, string edit)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.DeleteButtonOfGivenLineEditFlagClientTemplate, flagLine,
                        edit),
                    How.XPath).GetAttribute("class").Contains("is_disabled");
        }


        public bool IsDeleteFlagPresentForLineEdit(string flagLine, string edit)
        {
            return
                SiteDriver.IsElementPresent(
                    Format(ClaimActionPageObjects.DeleteButtonOfGivenLineEditFlagClientTemplate, flagLine,
                        edit),
                    How.XPath);
        }

        public void ClickOnDeleteIconFlagLevelForLineEdit(string flagLine, string edit)
        {
            /*SiteDriver.FindElement(
                Format(ClaimActionPageObjects.DeleteButtonOfGivenLineEditFlagClientTemplate, flagLine, edit), How.XPath).Click();*/

            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.DeleteButtonOfGivenLineEditFlagClientTemplate, flagLine, edit), 
                How.XPath);
            Console.WriteLine("Delete icon clicked for Line: " + flagLine + ", Edit: " + edit);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(()=> IsRestoreSpecificLineEditFlagByClientPresent(flagLine, edit));
        }

        public void ClickOnEditIconFlagLevelForLineEdit(int flagLine, string flag)
        {
            RemoveLock();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.EditButtonOfGivenLineEditFlagClientTemplate, flagLine, flag),
                How.XPath).Click();
            Console.WriteLine("Edit icon clicked for Line: " + flagLine + ", Flag: " + flag);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        }

        public int GetEditedLineCountAtTriggerLineNumber(string trigLine, string edit)
        {
            try
            {
                return SiteDriver.FindElementsCount(
                    Format(ClaimActionPageObjects.EditCountTriggeredByOtherLineXPathTemplate, trigLine, edit),
                    How.XPath);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool IsSwitchDeleted(string flaggedLine)
        {
            string disabledSwitch =
                Format(ClaimActionPageObjects.DisabledSwitchIconXPathTemplate, flaggedLine);
            try
            {
                return SiteDriver.FindElement(disabledSwitch, How.XPath) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetTriggerLineByFlag(string flag)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.TriggerLineValueByFlagXPathTemplate, flag), How.XPath).Text;
        }

        public string GetUnitByLineFlag(string line, string flag)
        {
            return
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.UnitsValueByLineFlagXPathTemplate, line, flag),
                        How.XPath)
                    .Text;
        }

        public string GetUnitByFlag(string flag)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.UnitsValueByFlagXPathTemplate, flag), How.XPath).Text;
        }

        public ClaimActionPage ClickSwitchEditIconByFlag(string flag, bool handlepopup = true)
        {
            RemoveLock();
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.MouseOver(Format(ClaimActionPageObjects.SwitchIconByFlagXPathTemplate, flag),
                    How.XPath);
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SwitchIconByFlagXPathTemplate, flag), How.XPath).Click();
                if (handlepopup)
                    ClickOkCancelOnConfirmationModal(true);
                WaitForWorkingAjaxMessage();

                Console.WriteLine("Clicked on Switch Edit Icon");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsSwitchEditIconByFlagEnabled(string flag)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.SwitchIconByFlagXPathTemplate, flag), How.XPath);
        }

        public string GetLogicClassNameByFlag(string flag)
        {
            return SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.LogicIconByFlagXPathTemplate, flag), How.XPath)
                    .GetAttribute("class").Split(' ')[0];
        }

        /// <summary>
        /// Click on  notes link on header section of container view
        /// </summary>
        public void ClickOnLogicIconByFlag(string flag)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.LogicIconByFlagXPathTemplate, flag), How.XPath).Click();
                Console.WriteLine("Clicked  Logic Icon{0} on flag<{0}>", flag);
                SiteDriver.WaitForCondition(() => IsLogicWindowDisplay());

        }

        public bool IsLogicMessagePresent()
        {
            bool messagePresent = SiteDriver.IsElementPresent(ClaimActionPageObjects.LogicMessageCssSelector, How.CssSelector);
            return messagePresent;
        }

        public string GetLogicMessageFormLabel()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.LogicMessageFormLabelCssSelector, How.CssSelector)
                .Text;
        }

        public void Refresh()
        {
            WaitForStaticTime(1000);
            SiteDriver.Reload();
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(ClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
        }

        public string GetHciRunValue()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciRunValueCssLocator, How.CssSelector)
                ? SiteDriver.FindElement(ClaimActionPageObjects.HciRunValueCssLocator, How.CssSelector)
                    .Text
                : null;
        }

        public string GetHciVoidValue()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciVoidValueCssLocator, How.CssSelector)
                ? SiteDriver.FindElement(ClaimActionPageObjects.HciVoidValueCssLocator, How.CssSelector)
                    .Text
                : null;
        }

        public string GetProvSpecValue()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciProvSpecValueCssLocator, How.CssSelector)
                ? SiteDriver
                    .FindElement(ClaimActionPageObjects.HciProvSpecValueCssLocator, How.CssSelector).Text
                : null;
        }

        public string GetProvNameValue()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.HciProvNameValueCssLocator, How.CssSelector)
                ? SiteDriver
                    .FindElement(ClaimActionPageObjects.HciProvNameValueCssLocator, How.CssSelector).Text
                : null;
        }

        #endregion

        #region DXCODES

        /// <summary>
        /// Get first original dx date
        /// </summary>
        /// <returns>FirstOriginalDxDate</returns>
        public string GetFirstOrignialDxDate()
        {
            try
            {
                return SiteDriver.FindElement(ClaimActionPageObjects.FirstOriginalDxDateCssLocator,
                    How.CssSelector).Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        #region CLAIM LINES

        public List<string> GetClaimLineNoByLabelAndValueCssLocator(string claimLineLabel, string value)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.ClaimLineNoByLabelAndValueCssLocator, claimLineLabel, value),
                "Text");
        }

        public bool IsSelectAllLinesIconPresentInClaimLine()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.SelectAllLinesToggleIconCssSelector,
                       How.CssSelector) != null;
        }

        public void ClickSelectAllLinesIcon()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.SelectAllLinesToggleIconCssSelector,
                How.CssSelector).Click();
        }

        public List<string> GetClaimLinesIndexValue()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimLineIndexCssSelector,
                How.CssSelector, "Text");

        }

        public List<string> GetSelectedClaimLineIndexValue()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.SelectedClaimLineCssSelector,
                How.CssSelector, "Text");
        }

        public string GetTopFlagByRow(int row = 1)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.TopFlagOnClaimLineCssTemplate, row),
                    How.CssSelector).Text;
        }

        public List<string> GetTopFlagList()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.TopFlagListOnClaimLineCssTemplate,
                How.CssSelector, "Text");
        }

        public int GetCountOfClaimLines()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.ClaimLineCount,
                How.CssSelector);
        }

        public string GetProcCodeOnClaimLine(int row = 1)
        {
            return SiteDriver
                .FindElement(Format(ClaimActionPageObjects.ProcCodeClaimLineCssLocator, row),
                    How.CssSelector).Text;
        }

        public string GetProcDescriptionOnClaimLine(int row = 1)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.ProcDescriptionClaimLineCssLocator, row),
                    How.CssSelector).Text;
        }

        /// <summary>
        /// Get first end date of service on claim line
        /// </summary>
        /// <returns>End Dos</returns>
        public string GetFirstEndDosOnClaimLine()
        {
            try
            {
                return SiteDriver.FindElement(ClaimActionPageObjects.FirstEndDosOnClaimLineCssLocator,
                    How.CssSelector).Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public List<string> GetFlagListForClaimLine(int col)
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimActionPageObjects.FlagListForLineCssLocator, col), How.CssSelector), 10000);
            return JavaScriptExecutor.FindElements(Format(ClaimActionPageObjects.FlagListForLineCssLocator, col),
                How.CssSelector, "Text");
        }

        public List<string> GetFlagListWithDeletedFlagForLine(int col)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.FlagListWithDeletedFlagForLineCssLocator, col),
                How.CssSelector, "Text");
        }

        #endregion

        public List<string> GetIndexOfSelectedLinesInFlaggedLine()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.FlagLineSelectedLinesIndexCssSelector,
                How.CssSelector, "Text");
        }

        public bool IsClaimSearchPanelPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimSearchPanelSectionCssLocator,
                How.CssSelector);
        }

        public bool IsClaimSearchPanelHidden()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimSearchPanelDisabledSectionCssLocator,
                How.CssSelector);
        }

        //public string GetSearchListMessageOnLeftGridForEmptyData()
        //{
        //    return
        //        SiteDriver.FindElement(NewClaimActionPageObjects.EmptySearchListMessageSectionCssLocator,
        //            How.CssSelector).Text;
        //}

        public string GetEmptyMessage()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.EmptyMessageCssLocator, How.CssSelector)
                    .Text;
        }

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.FindXPath, How.XPath); //ProxyWebElement);
        }

        public void ClickOnClearLinkOnFindSection()
        {
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.ClearLinkOnFindClaimSectionCssLocator,
                How.CssSelector);
        }

        public string GetAlternateClaimNoLabelTitleInFindPanel()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.AlternateClaimNoInFindPanelLabelCssLocator,
                    How.CssSelector
                ).Text;
        }

        public string GetClaimSequenceInFindClaimSection()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceXPath, How.XPath).GetAttribute("value");
        }

        public void SetClaimSequenceInFindClaimSection(string claimSeq)
        {
            SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceXPath, How.XPath).ClearElementField();
            SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimSequenceXPath, How.XPath).SendKeys(claimSeq);
            Console.WriteLine("Claim sequence set to " + claimSeq);
        }

        public string GetClaimNoInFindClaimSection()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.ClaimNoXPath, How.XPath).GetAttribute("value");
        }

        public void SetClaimNoInFindClaimSection(string claimNo)
        {
            if (!IsWorkListControlDisplayed()) ClickWorkListIcon();
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimNoXPath, How.XPath).ClearElementField();
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimNoXPath, How.XPath).SendKeys(claimNo);
            Console.WriteLine("Claim no set to " + claimNo);
        }

        public string GetSearchlistComponentItemLabel(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SearchlistComponentItemLabelTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetSearchlistComponentItemValue(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SearchlistComponentItemValueTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetSearchlistComponentTooltipValue(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SearchlistComponentItemValueTemplate, row, col),
                    How.CssSelector).GetAttribute("title");
        }

        public string GetAppealLevelBadgeOnSearchResult(int row)
        {
            return
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.AppealLevelBadgeOnSearchResultTemplate, row),
                    How.CssSelector).Text;
        }

        public List<string> GetClaimSeqListOnSearchResult()
        {
            return
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimActionsListOnSearchResult,
                    How.CssSelector, "Text");
        }

        public void GenericSelectSearchDropDownListValue(string fieldName, string value)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath)
                .ClearElementField();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath).SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimActionPageObjects.SearchInputListValueXPathTemplateGeneric, fieldName, value),
                How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(
                Format(ClaimActionPageObjects.SearchInputListValueXPathTemplateGeneric, fieldName, value),
                How.XPath);
            Console.WriteLine("<{0}> <{1}> Selected", value, fieldName);
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

        public void SetNoteToVisbleTextarea(String note)
        {
            SiteDriver.SwitchFrameByCssLocator("section:not([style*='none'])>ul iframe.cke_wysiwyg_frame.cke_reset");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
        }

        public void SetLengthyNoteToVisbleTextarea(string header,string text, bool isNoteForAddFlagSection = false)
        {
            SiteDriver.SwitchFrameByCssLocator(!isNoteForAddFlagSection
                ? "section:not([style*='none'])>ul iframe.cke_wysiwyg_frame.cke_reset"
                : "section.add_flag_form ul .cke_wysiwyg_frame");
            SendValuesOnTextArea(header, text);
        }

        public void SetNoteToCiuReferral(String note)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(
                "div:has(>label:contains(Identified Pattern)) iframe.cke_wysiwyg_frame.cke_reset");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
        }

        public void SetLengthyNoteToCiuReferral(string header,string text,bool handlePopup=true)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(
                "div:has(>label:contains(Identified Pattern)) iframe.cke_wysiwyg_frame.cke_reset");
            SendValuesOnTextArea(header, text,handlePopup);
        }

        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
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

        public string GetNoteOfCiuReferral()
        {
            JavaScriptExecutor.SwitchFrameByJQuery(
                "div:has(>label:contains(Identified Pattern)) iframe.cke_wysiwyg_frame.cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

       

        public void SetOverrideNoteInNoteByRow(string note, int row = 1)
        {
            if (note.Length == 2501)
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row), How.CssSelector)
                    .ClearElementField();
                JavaScriptExecutor.SendKeys(note.Substring(0, 2490),
                    Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row),
                    How.CssSelector); //selenium couldnot perform sendkeys for long text which causes hung issue so javascript implemented
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row),
                        How.CssSelector)
                    .SendKeys(note.Substring(2490, 11));
            }
            else
            {
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row), How.CssSelector)
                    .ClearElementField();
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row), How.CssSelector)
                    .SendKeys(note);
            }
        }

        public string GetOverrideNoteInNoteByRow(int row = 1)
        {
            return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.OverrideNotesCssTemplate, row),
                How.CssSelector).GetAttribute("value");
        }

        public void SetNoteInNoteEditorByName(string text, string name, bool lengthy)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(Format("div.note_row:has(span:contains({0})) .cke_wysiwyg_frame",
                name));
            SendValuesOnTextArea(name, text);
        }

        public string GetNoteInNoteEditorByName(string name)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(Format("div.note_row:has(span:contains({0})) .cke_wysiwyg_frame",
                name));
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

       public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(()=>IsWorkingAjaxMessagePresent(), 2000);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void ClickOnDeleteReferralBtn(int row)
        {
            SiteDriver.FindElement(

                string.Format(ClaimActionPageObjects.DeleteReferralButtonCssTemplate, row), How.CssSelector).Click();

            Console.WriteLine("Delete referral  in  new claim action ");
        }

        
        public List<string> GetListOfDxCodeList()
        {
            var list = JavaScriptExecutor.FindElements(ClaimActionPageObjects.DxCodeListXpathLocator, How.XPath, "Text");
            list.RemoveAll(String.IsNullOrEmpty);
            return list;
        }

        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                }
            }
        }

        public void CloseAllExistingPopupIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                }
            }
        }

        public string GetFlagLineDetailsRowColWise(int row, int col)
        {
            var t = SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.FlagLineDescRowColWiseCssLocator, row, col),
                    How.CssSelector).Text;
            return t;
        }

        public bool IsTriggerAltClaimOnFlagLinesClickable(int row)
        {
            return
                SiteDriver.IsElementPresent(
                    Format(ClaimActionPageObjects.TriggerAltclaimNoOnFlagLinesCssLocator, 1, row),
                    How.CssSelector);
        }

        public string GetClaimSequenceFromClaimActionPopupOnClickTriggerAltClaimNo(string claseq, int row)
        {
            var claimAction = Navigator.Navigate(() =>
            {
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.TriggerAltclaimNoOnFlagLinesCssLocator, 1, row),
                    How.CssSelector).Click();
                Console.WriteLine("Clicked on Trigger claim Alt Claim No.");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl(claseq.Split('-')[0]));
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            }, () => new ClaimActionPageObjects());
            var claimSeq = GetCurrentClaimSequence();
            CloseAllExistingPopupIfExist();
            return claimSeq;

        }

        public string GetEOBMessage(string flag)
        {
            return JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.EOBMessageCssLocator, flag))
                .Text;
        }

        public string GetTriggerClaimLineResultValueByRowCol(int row, int col)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.SelectTriggerClaimLineXpathByRowAndColTemplate, row, col),
                    How.XPath).Text;
        }

        public void ClickOnTriggerClaimLineResultProcCodeByRow(int row)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.SelectTriggerClaimLineXpathByRowAndColTemplate, row, 3),
                How.XPath).Click();
        }

        public NewPopupCodePage ClickOnTriggerClaimLineResultForUnknownProcCodeByRow(int row)
        {
            var proccode = GetTriggerClaimLineResultValueByRowCol(1, 3);
            NewPopupCodePage.AssignPageTitle(Format("- Unknown"));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.SelectTriggerClaimLineXpathByRowAndColTemplate, row, 3),
                    How.XPath).Click();
                Console.Out.WriteLine("Clicked on Trigger claim/line {0} for {1} ", row, proccode);
                SiteDriver.SwitchWindowByTitle("Unknown");
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAddDocumentIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.AddDocumentIconCssSelector, How.CssSelector);
        }

        public bool IsViewDocumentIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ViewDocumentIconCssSelector, How.CssSelector);
        }

        public bool IsRedBadgeDocumentIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.RedBadgeDocumentIconCssSelector,
                How.CssSelector);
        }

        public void ClickOnClaimDocuments()
        {
            WaitForStaticTime(2000);
            SiteDriver
                .FindElement(ClaimActionPageObjects.DocumentsLinkCssLocator, How.CssSelector).Click();
            WaitForStaticTime(2000);
            WaitForWorkingAjaxMessage();
            if (GetLeftHeaderOfQ2Quadrant() != "Claim Documents")
            {
                Console.WriteLine("First Click is not working so click once again in claim document");
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.DocumentsLinkCssLocator, How.CssSelector);
                WaitForWorkingAjaxMessage();
            }
        }

        public string GetClaimDocumentIconToolTip()
        {
            return SiteDriver
                .FindElement(ClaimActionPageObjects.DocumentsLinkCssLocator, How.CssSelector)
                .GetAttribute("title");
        }

        public bool IsUploadDocumentContainerPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.UploadDocumentCssSelector, How.CssSelector);
        }

        public bool IsUploadNewDocumentFormPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.UploadDocumentFormCssSelector,
                How.CssSelector);
        }

        public string GetClaimDocsCountInBadge()
        {
            return !IsRedBadgeDocumentIconPresent()
                ? "0"
                : JavaScriptExecutor.FindElement(ClaimActionPageObjects.RedBadgeDocumentIconCssSelector).Text;
        }

        public int GetClaimDocListCount()
        {
            return SiteDriver.FindElementsCount(
                ClaimActionPageObjects.NotesListCssLocator, How.CssSelector);
        }

        public void ClickonUploadDocumentSaveButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.UploadDocumentSaveButtonCssSelector,
                How.CssSelector).Click();
            Console.WriteLine("Clicked on Save Button");
        }

        public void ClickOnUploadDocumentCancelBtn()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.UploadDocumentCancelButtonCssSelector,
                How.CssSelector).Click();
            Console.WriteLine("Clicked on Cancel Link");
        }

        public void DeleteClaimDocumentRecord(string claSeq, string audit_date)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimDocumentRecord, claSeq.Split('-')[0],
                claSeq.Split('-')[1], audit_date));
            Console.WriteLine("Delete Claim Document Record from database  for claseq<{0}>", claSeq);
        }

        public void ClickOnDocumentToViewAndStayOnPage(int docrow)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.ClaimDocumentNameCssTemplate, docrow),
                    How.CssSelector)
                .Click();
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count - 1]);
        }

        public string GetOpenedDocumentText()
        {
            return SiteDriver.FindElement("body>pre", How.CssSelector).Text;
        }

        public ClaimActionPage CloseDocumentTabPageAndBackToNewClaimAction()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsClaimDocumentDeleteIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDocumentDeleteIconCssLocator,
                How.CssSelector);
        }

        public bool IsVerticalScrollBarPresentInClaimDocumentSection()
        {
            const string select = ClaimActionPageObjects.ClaimDocumentSectionWithScrollBarCssLocator;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("clientName Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public void ClickOnSwitchIconInEditFlagSectionInClaimLine()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.SwitchIconInEditFlagCssSelector,
                How.CssSelector).Click();
        }

        public bool IsFlaggedLineDeletedByLine(int linno, int flaggedRow)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.FlaggedLineDeletedByLineNoAndFlaggedRow, linno, flaggedRow),
                How.CssSelector);
        }

        public bool IsFlaggedLineNotDeletedByLine(int linno, int flaggedRow)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.FlaggedLineNotDeletedByLineNoAndFlaggedRow, linno, flaggedRow),
                How.CssSelector);
        }

        public bool IsEditFlagDeleteRestoreButtonDisabled(bool restore = false)
        {
            if (restore)
                return SiteDriver
                    .FindElement(ClaimActionPageObjects.LineRestoreCssLocator, How.CssSelector)
                    .GetAttribute("class").Contains("is_disabled");
            return SiteDriver.FindElement(ClaimActionPageObjects.LineDeleteCssLocator, How.CssSelector)
                .GetAttribute("class").Contains("is_disabled");
        }

        public bool IsEditIconEnabledByLinenoAndRow(int linno, int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.EnabledEditIconByLineNoAndRowCssSelector, linno, row),
                How.CssSelector);
        }

        public bool IsEditIconDisabledByLinenoAndRow(int linno, int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DisabledEditIconByLineNoAndRowCssSelector, linno, row),
                How.CssSelector);
        }

        public bool IsEditIconDisabledByLineno(int linno)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DisabledEditIconByLineNumber, linno),
                How.CssSelector);
        }

        public bool IsDeleteIconEnabledByLinenoAndRow(int linno, int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.EnabledDeleteIconByLineNoAndRowCssSelector, linno, row),
                How.CssSelector);
        }

        public bool IsDeleteIconDisabledByLinenoAndRow(int linno, int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DisabledDeleteIconByLineNoAndRowCssSelector, linno, row),
                How.CssSelector);
        }

        public bool IsDeleteIconDisabledByLineNumber(int linno = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DisabledDeleteIconbyLineNumber, linno),
                How.CssSelector);
        }

        public bool IsDeleteIconRowByRowDisabled(int row)
        {
            return SiteDriver.FindElement(Format(
                    ClaimActionPageObjects.DeleteIconOnFlagLinesByRowCssLocator, row), How.CssSelector)
                .GetAttribute("class").Contains("is_disabled");
        }

        public bool IsDeleteRestoreIconRowByRowDisabled(int row)
        {
            return SiteDriver.FindElement(Format(
                    ClaimActionPageObjects.DeleteRestoreIconOnFlagLinesByRowCssLocator, row), How.CssSelector)
                .GetAttribute("class").Contains("is_disabled");
        }

        public bool IsDeleteRestoreAllFlagsIconCssLocator()
        {
            return SiteDriver.FindElement(Format(
                    ClaimActionPageObjects.DeleteAllFlagsIconCssLocator), How.CssSelector).GetAttribute("class")
                .Contains("is_disabled");
        }

        public List<string> GetTriggerClaimsList(bool single = false)
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.FlagDetailsTriggerClaimsList, How.CssSelector,
                "Text");
        }

        public bool IsRestoreIconDisabledByLinenoAndRow(int linno, int row)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DisabledRestoreIconByLineNoAndRowCssSelector, linno, row),
                How.CssSelector);
        }

        public bool IsIconEnabledByLineNoFlagNameAndIconName(int lineNo, string flag, string iconName)
        {
               return SiteDriver.FindElementAndGetClassAttribute(Format(
                    ClaimActionPageObjects.IconSelectorByLineNumFlagAndIconNameXPathTemplate,
                    lineNo, flag, iconName), How.XPath)[0].Contains("is_active");
          
        }

        public string ClickOnTriggerClaimOnFlagDetailsAndGetClaSeq(int row = 1)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(
                    Format(ClaimActionPageObjects.FlagDeatailsTrigerClaimsTemplate, 1), How.CssSelector);
                Console.WriteLine("Clicked on " + row + "th Trigger Claim Sequence");
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
            });
            SwitchToSecondHandlePage();
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).GetClaimSequence();
        }

        #endregion

        #region PRIVATE METHODS

        private string GetToolTipHelp(string webElement)
        {
            SiteDriver.MouseOver(webElement, How.XPath, 1000);
            return SiteDriver
                .FindElement(ClaimActionPageObjects.TooltipId, How.Id).Text.Substring(9);
        }

        private ClaimActionPage SelectTransferOption(string transferOptionXPath)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(transferOptionXPath, How.XPath).Click();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #endregion

        public void ClickPatientHistoryIcon()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.PatientClaimHistoryIconCssLocator,
                How.CssSelector).Click();
            Console.WriteLine("Clicked on Patient Claim History Icon");
        }

        //public NewPopupCodePage ClickOnPatientHxAndSwitch(string title, string revCode)
        //{
        //    JavaScriptExecutor.ExecuteClick(Format(ClaimHistoryPageObjects.RevCodeXpathTemplateByRevCode, revCode), How.XPath);
        //    return SwitchToPopupCodePage(title);
        //}

        public void ClickOnErrorMessageOkButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.ErrorMessageOkButtonCssSelector,
                    How.CssSelector)
                .Click();
            WaitForWorkingAjaxMessage();
        }

        #region CIU Referrals

        //to check no ciu icon is present next to prov name
        public string GetLastRowLastColLabelForClaimDetails()
        {
            return
                SiteDriver.FindElement(ClaimActionPageObjects.ClaimDetailsLastRowCssLocator,
                    How.CssSelector).Text;
        }

        public int GetCiuReferralRecordRowCount()
        {
            return SiteDriver.FindElementsCount(ClaimActionPageObjects.CiuReferralRecordRowCssSelector,
                How.CssSelector);
        }

        public void ClickonAddCiuReferralIcon()
        {

            SiteDriver
                .FindElement(ClaimActionPageObjects.AddCiuButtonCssLocator, How.CssSelector).Click();
            SiteDriver.WaitForCondition(IsCreateCiuReferralSectionDisplayed);
            Console.WriteLine("Clicked on add Ciu Referral Icon");

        }

        public void SetSearchInputFieldForCiu(string fieldName, string value)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldName), How.XPath)
                .ClearElementField();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldName), How.XPath).SendKeys(value);
            Console.WriteLine("<{0}: <{1}>", fieldName, value);
        }


        public string GetSearchInputFieldForCiu(string fieldName)
        {
            return SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldName), How.XPath)
                .GetAttribute("value");
        }

        public string GetCiuReferralFormInputValueGeneric(string fieldname)
        {
            return
                SiteDriver.FindElement(
                        Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                        How.XPath)
                    .GetAttribute("value");
        }

        public void SetCiuReferralFormInputValueGeneric(string value, string fieldname)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                How.XPath).ClearElementField();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                How.XPath).SendKeys(value);
        }

        public void SetLengthyValueInCiuReferralFormInputValueGeneric(string value, string fieldname, int length)
        {
            JavaScriptExecutor.SendKeys(value.Substring(0, length - 11),
                Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                How.XPath); //selenium couldnot perform sendkeys for long text which causes hung issue so javascript implemented

            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                    How.XPath)
                .SendKeys(value.Substring(0, 11));
        }

        public string GetNoCiuReferralMessage()
        {
            return JavaScriptExecutor.FindElement(ClaimActionPageObjects.NoCiuReferralCssSelector).Text;
        }

        public bool IsNoCiuReferralMessagePresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimActionPageObjects.NoCiuReferralCssSelector);
        }

        public void InsertIdentifiedPattern(string text)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(text);
            Console.WriteLine("Decision Rationale set to {0}", text);
            SiteDriver.SwitchBackToMainFrame();
        }

        public bool IsCreateCiuReferralSectionDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.CreateCiuReferralFormXPath, How.XPath);
        }


        public void ClickOnCancelCiuReferral()
        {
            SiteDriver.FindElementAndClickOnCheckBox(ClaimActionPageObjects.CancelCiuReferralLinkCssSelector,
                How.XPath);
            Console.WriteLine("Click on Cancel Link");
        }

        public void ClickonSaveCiuReferralIcon()
        {

            SiteDriver
                .FindElement(ClaimActionPageObjects.SaveCiuButtonXpath, How.XPath).Click();
            Console.WriteLine("Saved Ciu Referral Icon");

            WaitForWorkingAjaxMessage();
        }

        public void SetInputFieldOnCreateCiuReferralByLabel(string label, string text)
        {
            //JavaScriptExecutor.SetText(Format(
            //    NewClaimActionPageObjects.InputCiuReferralCssTemplate, label),text);
            JavaScriptExecutor.FindElement(Format(
                ClaimActionPageObjects.InputCiuReferralCssTemplate, label)).SendKeys(text);
            Console.WriteLine("Set {0}:<{1}>", label, text);
        }

        public void SelectPatternCategory(string pattern)
        {
            JavaScriptExecutor.FindElement(ClaimActionPageObjects.PatternCategoryInputCssSelector).Click();

            SetInputFieldOnCreateCiuReferralByLabel("Pattern", pattern);

            JavaScriptExecutor
                .FindElement(Format(ClaimActionPageObjects.PatternCategoryValueCssSelector, pattern)).Click();
            JavaScriptExecutor.ExecuteMouseOut(ClaimActionPageObjects.PatternCategoryInputCssSelector);
            Console.WriteLine("Select Pattern :<{0}>", pattern);
        }

        public List<string> GetCiuCreatedDateList()
        {
            return
                JavaScriptExecutor.FindElements(
                    ClaimActionPageObjects.CiuReferralCreatedDeteCssTemplate,
                    How.CssSelector, "Text");
        }

        public string GetCiuReferralDetailsByRowLabel(int ciuRecordRow, string label)
        {

            return JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralDetailByRowLabelCssTemplate, ciuRecordRow,
                        label))
                .Text.Trim();
        }

        public string GetCiuReferralToolTipDayByRowLabel(int ciuRecordRow, string label)
        {

            return JavaScriptExecutor.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralDetailByRowLabelCssTemplate, ciuRecordRow,
                        label))
                .GetAttribute("title")
                .Trim();
        }

        public bool IsVerticalScrollBarPresentInProviderDetailSection()
        {
            const string select = ClaimActionPageObjects.ProviderDetailSectionCssSelector;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("clientName Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public void ClickOnDeleteCiuReferralIconByRecordRow(int ciuRecordRow)
        {
            SiteDriver.FindElementAndClickOnCheckBox(
                Format(ClaimActionPageObjects.DeleteCiuReferralIconCssTemplate, ciuRecordRow),
                How.CssSelector);
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent);
        }

    

        public void SendTabKeyOnPhoneNumberToFocusExt()
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.CiuReferralInputFieldXpathTempalte, "Phone Number"),
                    How.XPath)
                .SendKeys(Keys.Tab);
        }

        public string GetFlagDetailsTextByLabel(string label)
        {
            return JavaScriptExecutor
                .FindElement(Format(ClaimActionPageObjects.FlagDetailsTextByLabel, label) + " span").Text;
        }

        #endregion

        public List<string> GetTriggerClaimsFromDB(string claimSequenceWithFotFlag)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetTriggerClaimDetails,
                claimSequenceWithFotFlag));
        }

        public List<string> GetRestrictionsListFromDB()
        {
            var userId = EnvironmentManager.Username;
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetAvailabeRestrictionsList,
                userId));
        }

        public bool DoesClaimNotHaveRestriction(ClaimActionPage _claimAction)
        {
            //Restrictions
            // 0 = No restriction assigned
            // 1 = Offshore,
            // 2 = A,
            // 3 = B
            List<string> restrictionList = new List<string>(new string[] {"1", "2", "3"});
            var claimSequence = _claimAction.GetClaimSequence().Split('-');
            var restrictionFromDB = Executor
                .GetSingleValue(Format(ClaimSqlScriptObjects.GetRestrictionForClaimseq, claimSequence[0],
                    claimSequence[1])).ToString();
            if (restrictionFromDB == "0")
                return true;
            return restrictionList.Contains(Executor
                .GetSingleValue(Format(ClaimSqlScriptObjects.GetRestrictionForClaimseq,
                    _claimAction.GetClaimSequence().Split('-')[0], _claimAction.GetClaimSequence().Split('-')[1]))
                .ToString());
        }

        public bool DoesClaimHaveProperRestrictionForRestrictedUser(ClaimActionPage _claimAction)
        {
            //Restrictions
            // 0 = No restriction assigned
            // 3 = B
            List<string> restrictionList = new List<string>(new string[] {"0", "3"});
            return restrictionList.Contains(Executor
                .GetSingleValue(Format(ClaimSqlScriptObjects.GetRestrictionForClaimseq,
                    _claimAction.GetClaimSequence().Split('-')[0], _claimAction.GetClaimSequence().Split('-')[1]))
                .ToString());
        }

        public bool DoesClaimContainRespectiveRestriction(ClaimActionPage _claimAction)
        {
            var claseq = _claimAction.GetClaimSequence().Split('-');
            string restriction = Executor.GetSingleValue(Format(ClaimSqlScriptObjects.GetRestrictionFromDec,
                _claimAction.GetSideBarPanelSearch.GetInputValueByLabel("Claim Restrictions"))).ToString();
            return (Executor
                .GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetRestrictionForClaimseq, claseq[0],
                    claseq[1])).Contains(restriction));
        }

        public bool DoesClaimMatchNoRestriction(ClaimActionPage _claimAction)
        {
            return (Executor.GetSingleValue(Format(ClaimSqlScriptObjects.GetRestrictionForClaimseq,
                        _claimAction.GetClaimSequence().Split('-')[0],
                        _claimAction.GetClaimSequence().Split('-')[1])).ToString() == "0");
        }

        public bool IsClaimRestrictionIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimRestrictionIconToolbarCssLocator,
                How.CssSelector);
        }

        public string GetClaimRestrictionTooltip()
        {
            return
                JavaScriptExecutor.FindElement(
                    ClaimActionPageObjects.ClaimRestrictionIconToolbarCssLocator).GetAttribute("title");
        }

        public bool IsClaimRestrictionIndicatorTooltipPresent(string workListType)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.ClaimRestrictionIndicatorIconToolbarCssLocator, workListType),
                How.CssSelector);
        }

        public string GetPatientSeq()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.PatientSeqXPath, How.XPath).Text;
        }

        public void SwitchBack()
        {
            SiteDriver.Back();
        }

        public bool IsDentalElementPresent(string classname, string title)
        {
            if (classname == "flagged_line")
            {
                return SiteDriver
                    .IsElementPresent(
                        Format(ClaimActionPageObjects.DataPointsLabelForFlaggedLineCssSelectorTemplate,
                            title),
                        How.CssSelector);
            }
            return SiteDriver
                .IsElementPresent(
                    Format(ClaimActionPageObjects.DentalDataPointLabelForClaimLineCssTemplate,
                        title),
                    How.CssSelector);
        }

        public string GetDentalData(string classname, string title, int row = 1)
        {
            if (classname == "flagged_line")
            {
                return SiteDriver
                    .FindElement(
                        Format(ClaimActionPageObjects.DataPointsValueForFlaggedLineCssSelectorTemplate, row,
                            title),
                        How.CssSelector).Text;
            }
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.DentalDataPointValueForClaimLineCssTemplate, row, title),
                    How.CssSelector).Text;
        }

        public List<string> GetDentalDataList(List<string> labelList, string classname)
        {
            var data = "";
            foreach (var item in labelList)
            {
                if (labelList.LastOrDefault().Equals(item))
                {
                    data += Format("contains(normalize-space(),'{0}')", item);
                }
                else
                {
                    data += Format("contains(normalize-space(),'{0}') or ", item);
                }
            }
            return JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.DentalDataValueListXPathTemplate, classname, data),
                How.XPath, "Text");
        }

        public List<string> GetDentalInputDataList(List<string> labelList)
        {
            var data = "";
            foreach (var item in labelList)
            {
                if (labelList.LastOrDefault().Equals(item))
                {
                    data += Format("contains(normalize-space(),'{0}')", item);
                }
                else
                {
                    data += Format("contains(normalize-space(),'{0}') or ", item);
                }
            }
            return JavaScriptExecutor.FindElements(
                Format(ClaimActionPageObjects.DentalDataInputValueListXPathTemplate, data),
                How.XPath, "Attribute:value");
        }

        public string GetDentalDataToolTipText(string classname, string title, int row = 1)
        {
            if (classname == "flagged_line")
            {
                return SiteDriver
                    .FindElement(
                        Format(ClaimActionPageObjects.DataPointsValueForFlaggedLineCssSelectorTemplate, row,
                            title), How.CssSelector).GetAttribute("title");
            }
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.DentalDataPointValueForClaimLineCssTemplate, row,
                        title), How.CssSelector).GetAttribute("title");
        }

        public int GetDentalDataLength(string classname, string title, int row = 1)
        {
            if (classname == "flagged_line")
            {
                return GetDentalData(classname, title, row).Length;
            }
            return GetDentalData(classname, title, row).Length;
        }

        public bool IsDigitOfLengthTwo(string classname, string title, int row = 1)
        {
            if (classname == "flagged_line")
            {
                var value = GetDentalData(classname, title, row);
                return Regex.IsMatch(value, @"^[0-9]{2}$");
            }
            var data = GetDentalData(classname, title, row);
            return Regex.IsMatch(data, @"^[0-9]{2}$");
        }

        public List<string> GetDentalDataPointsValuesFromDb(string claseq)
        {
            var dataPointsValues = Executor
                .GetCompleteTable(Format(ClaimSqlScriptObjects.GetDentalDataPointsValuesFromDb, claseq))
                .ToList();
            List<string> listDataPoints = new List<string>();
            for (int i = 0; i < dataPointsValues[0].Table.Columns.Count; i++)
            {
                listDataPoints.Add(dataPointsValues[0][i].ToString());
            }
            return listDataPoints;
        }

        public string GetLongDescOfToothNoFromDb(string toothno)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetLongDescOfToothNoFromDb,
                toothno));
        }

        public string GetLongDescOfOralCavityFromDb(string oralcavity)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetLongDescOfOralCavityFromDb,
                oralcavity));
        }

        public void ClickOnClaimLineByTitle(string title)
        {
            SiteDriver.FindElement(Format(ClaimActionPageObjects.ClaimLineViewLinkCssTemplate, title),
                How.CssSelector).Click();
        }

        public List<string> GetClaimLinesDataPointsTitle()
        {
            ClickOnClaimLineByTitle("Claim Lines");
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimLineDataPointsTitleCssSelector,
                How.CssSelector, "Text");
        }

        public bool DoesClaimLineContainAdjPaidandMod()
        {
            return GetClaimLinesDataPointsTitle().Contains("M:") &&
                   GetClaimLinesDataPointsTitle().Contains("Adj Paid:");
        }

        public void UpdateProductTypeToInactiveByClientCodeFromDb(string clientCode)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateProductTypeToInactiveByClientCode,
                clientCode));
            Console.WriteLine("Update Product type to inactive ", clientCode);
        }

        public void UpdateProductTypeToActiveByClientCodeFromDb(string clientCode)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateProductTypeToActiveByClientCode,
                clientCode));
            Console.WriteLine("Update Product type to inactive ", clientCode);
        }

        public bool GetProductStatusForClient(string product, string client)
        {
            if (product == "DCA")
                product = "DCI";
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.ActiveProduct, product, client)).Equals("T");
        }

        public void UpdateProductStatusForClient(string product, string client,bool active=false)
        {
            if (product == "DCA")
                product = "DCI";
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.ActiveOrDisableProductByClient, product, client,
                active ? 'T' : 'F'));
            Console.WriteLine($"Update {product} product status to  {active} for client : {client}");

        }
        public void EnableOnlyDCIForClient( string client )
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.EnableOnlyDCIForClient,  client));
            Console.WriteLine($"Enable only DCA product for client : {client}");
        }

        public void UpdateHideAppeal(bool hide,string clientcode)
        {
           string hideappeal = hide ? "T" : "F"; 
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateHideAppeal, hideappeal, clientcode));
            Console.WriteLine($"Hide Appeal Function setting updated to {hide} for client : {clientcode}");
            
        }

        public void RestoreProductForClients(string client)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.RestoreProductsForClients, client));

        }

        public void ClickClaimLineDentalEditIcon()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimLineDentalEditIconCssSelector, How.XPath)
                .Click();
        }

        public bool IsEditClaimLineFieldPresent()
        {
            var editClaimLineComponent =
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.EditClaimLineComponentsXPath, How.XPath, "Text")
                    .Count;
            if (editClaimLineComponent == 3)
            {
                return SiteDriver.IsElementPresent(ClaimActionPageObjects.EditClaimLineRegionCssSelector,
                    How.CssSelector);
            }
            return false;
        }

        public string GetEditClaimDentalValue(string title)
        {
            return SiteDriver
                .FindElement(
                    Format(ClaimActionPageObjects.EditClaimLineInputFieldXPathTemplate, title), How.XPath)
                .GetAttribute("value");
        }

        public int InputEditDentalDataLength(string title, string value)
        {
            SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.EditClaimLineInputFieldXPathTemplate, title), How.XPath)
                .ClearElementField();
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.EditClaimLineInputFieldXPathTemplate, title), How.XPath).SendKeys(value);
            return SiteDriver.FindElement(
                    Format(ClaimActionPageObjects.EditClaimLineInputFieldXPathTemplate, title), How.XPath)
                .GetAttribute("value").Length;
        }

        public void ClickSaveButton()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.EditDentalFieldSaveButtonCssLocator,
                How.CssSelector).Click();
            WaitForWorkingAjaxMessage();
        }

        public void ClickCancelLink()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.EditDentalFieldCancelLinkCssLocator, How.CssSelector)
                .Click();
        }

        public string GetPageErrorMessage()
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 5000);
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.PageErrorMessageId, How.Id).Text;
        }

        public void ClickDentalValues(string title)
        {
            SiteDriver.FindElement(
                Format(ClaimActionPageObjects.DentalDataPointValueForClaimLineCssTemplate, 1, title),
                How.CssSelector).Click();
            SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ExtendedPageClaimHistory));
        }

        public bool IsDCIProductFlagPresentInDCIWorklist()
        {
            var dciFlagFromDb = _commonSqlObjects.GetFlagListBasedOnProductDb("D");
            var flagList = JavaScriptExecutor.FindElements(ClaimActionPageObjects.DciWorklistFlagsCssSelector,
                How.CssSelector, "Text");
            return flagList.Any(x => dciFlagFromDb.Contains(x));

        }

        public bool IsDCIWorkListPresentAtTheBottom()
        {
            var worklistOptionList = _sideBarPanelSearch.GetOptionListOnArrowDownIcon();
            return worklistOptionList[worklistOptionList.Count - 1] == "DCA Work List";
        }

        public List<string> GetDciWorklistFilters()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.DciWorkListFiltersCssSelector, How.CssSelector,
                "Text");
        }

        public List<string> GetDciClaimStatus()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.DciClaimStatusCssSelector, How.CssSelector,
                "Text");
        }

        public List<string> MoreOptionList()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.MoreOptionListCssLocator, "Text");
        }

        public List<string> GetFlagDetailsList(string clientcode, string claseq)
        {
            var newList = new List<string>();
            var flagDetailsDataRows = Executor
                .GetCompleteTable(Format(ClaimSqlScriptObjects.GetFlagDetailInformation, clientcode,
                    claseq.Split('-')[0], claseq.Split('-')[1]));
            foreach (DataRow row in flagDetailsDataRows)
            {
                newList.Add(row[0].ToString());
                newList.Add(row[1].ToString());
                newList.Add(row[2].ToString());
                newList.Add(row[3].ToString());
            }
            return newList;
        }

        public string GetEditTextInformation(string clientcode, string claseq, string flag)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetEditTextlInformation,
                clientcode, claseq.Split('-')[0], claseq.Split('-')[1], flag));
        }

        public string GetEOBMessageFromDatabase(string flag, string type = "C")
        {
            var eobmsgDB =
                Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetEOBMessageForFlag, flag, type));
            eobmsgDB = eobmsgDB.Replace("{LineNo}", "{0}").Replace("{LineProcCode}", "{1}")
                .Replace("{LineProcCodeDesc}", "{2}");
            return eobmsgDB;
        }

        public string GetLongProcCodeDescription(string code, string codeType )
        {
            return Executor.GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetLongProcDescFromDb, code,
                    codeType));
        }

        public bool IsTNValueNull(string classname, string title, int row)
        {
            return GetDentalData(classname, title, row) == "";
        }

        public bool IsDxCodeHeaderDisplayed()
        {
            //return SiteDriver.FindElement(NewClaimActionPageObjects.Q2LeftHeaderCssLocator, How.CssSelector).Text ==
            //    "Dx Codes";
            return true;
        }

        public FlagPopupPage ClickOnFlagandSwitch(string title,string flag)
        {
            JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.FlagLinkByFlagCssTemplate,flag)).Click();
            var popupCode = Navigator.Navigate<FlagPopupPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(title));
                FlagPopupPageObjects.AssignPageTitle = title;
                Console.WriteLine("Switch To " + title + " Page");
            });
            return new FlagPopupPage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        
        public void ClickOnFlagLink()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.FirstFlagLinkCssLocator, How.CssSelector).Click();
        }

        public string GetFirstFlag()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.FirstFlagLinkCssLocator, How.CssSelector)
                .Text;
        }

        public bool IsFlagPopupDisplayed(string flag)
        {
            return SiteDriver.Title.Equals("Flag Information - " + flag);
        }

        public List<string> GetActiveFlagsForFCIFromDatabase()
        {
           return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.FlagList,'U'));           
        }



        public int GetFlagCountByClaimSeqAndProductFromDatabase(string claSeq, string productEnum)
        {
            var claimSeq = claSeq.Split('-').ToList();
            return Convert.ToInt32(Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetFlagCountByClaimSeqAndProduct, claimSeq[0], claimSeq[1], productEnum)));          
        }

        public List<string> GetAllFlagsFromWorklist()
        {
            var flagList = JavaScriptExecutor.FindElements(ClaimActionPageObjects.WorklistFlagsCssSelector,
                How.CssSelector, "Text");
            return flagList;
        }

        public List<string> GetActiveFlagsForFFPFromDatabase()
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.FlagList, 'R'));
        }

        public void ClickAndSaveOnEditAllFlagOnTheLineIcon(string reasonCode= "ACC 1 - Accept Response To Logic Request", bool emptyReasonCode = false)
        {
            RemoveLock();
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.EditAllFlagOnTheLineIconCssSelector,How.CssSelector);
            SiteDriver.WaitForCondition(()=>SiteDriver.IsElementPresent(ClaimActionPageObjects.EditAllFlagOnTheLineSectionCssSelector,How.CssSelector));            
            if(IsDeleteAllFlagIconPresentOnLine())
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.DeleteIconOnEditAllFlagOnTheLineCssSelector, How.CssSelector);
            else
                JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.RestoreIconOnEditAllFlagOnTheLineCssSelector, How.CssSelector);
            SelectReasonCode(reasonCode);
            SetNoteToVisbleTextarea("TEST");
            ClickOnSaveButton();
            //JavaScriptExecutor.ExecuteClick(NewClaimActionPageObjects.SaveButtonCssLocator,How.CssSelector);
            WaitForWorkingAjaxMessage();
            CaptureScreenShot("PopupIssueInEditFlags");
        }

        public void ClickEditAllFlagOnLineByLineNo(int lineNo = 1) =>
            JavaScriptExecutor.ExecuteClick(Format(ClaimActionPageObjects.EditAllFlagOnTheLineIconByFlagLineNoCssSelector, lineNo.ToString()), How.CssSelector);


        public bool IsDeleteAllFlagIconPresentOnLine()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DeleteIconOnEditAllFlagOnTheLineCssSelector,How.CssSelector);
        }

        public ClaimActionPage CreatePciWorklistWithReviewGroups(List<string> reviewGroups)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickOnPciWorkList();
                _sideBarPanelSearch.ClickOnClearLink();
                _sideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group", reviewGroups);
                _sideBarPanelSearch.ClickOnButtonByButtonName("Create");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void CreateFFPWorklistWithReviewGroups(List<string> reviewGroups)
        {
            ClickOnFFPWorkList();
            _sideBarPanelSearch.ClickOnClearLink();
            _sideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group", reviewGroups);
            _sideBarPanelSearch.ClickOnButtonByButtonName("Create");
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        }

        public void ClickOnPciWorkList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Worklist filter carot icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "CV Work List"),
                    How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "CV Work List"), How.XPath).Click();
            Console.WriteLine("Clicked on CV Work List Filter.");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "CV Work List"), How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "CV Work List"), How.XPath).Click();
        }

        public void ClickOnFFPWorkList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Worklist filter carot icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FFP Work List"),
                    How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FFP Work List"), How.XPath).Click();
            Console.WriteLine("Clicked on FFP Work List Filter.");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FFP Work List"), How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FFP Work List"), How.XPath).Click();
        }

        public List<string> GetReviewGroup(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ReviewGroupList, client));
        }

        public void ClickOnFlaggedLines(int n = 0)
        {
            SiteDriver.FindElement(Format(ClaimActionPageObjects.FlagLineDivCssSelector, n), How.CssSelector ).Click();
        }

        public string GetFlagDetailsByLabel(string label)
        {
        
             return SiteDriver.FindElement(
                Format(ClaimActionPageObjects.GetFlagDetailsByLabelXPath, label), How.XPath).Text;
        }

        public List<string> GetCustomFieldsLabel()
        {
            var list = SiteDriver.FindDisplayedElementsText(ClaimActionPageObjects.CustomFieldsLabelCssSelector,
                How.CssSelector);
            return list;
        }

       
       public string GetTooltipMessageForDentalIcon()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.DentalProfilerIconCssSelector, How.CssSelector)
                .GetAttribute("title");
        }

        public void WaitForAdditionalLockedClaims()
        {
            SiteDriver.WaitForCondition(() => !IsEmptyMessageOnAdditionalLockedClaimsPresent());
        }
        
        public string GetFlaggedLineDetails(int lineNum = 1, int rowNum= 1, int colNum = 1) =>
            SiteDriver.FindElementAndGetAttribute(
                string.Format(ClaimActionPageObjects.FlaggedLineDetailsCssSelectorTemplate, lineNum, rowNum, colNum),
                How.CssSelector, "innerHTML");


        public List<string> GetAssociatedClaimSubStatusForInternalUser(string client)
        {
            var subStatusList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.InternalUserClaimSubStatusList, client));
            return subStatusList;
        }

        public List<string> GetAssociatedClaimSubStatusForInternalUserWithDCIInactive(string client)
        {
            var subStatusList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.InternalUserClaimSubStatusListForDCIInactiveClient, client));
            return subStatusList;
        }

        public void UpdateClaimStatus(string claSeq, string client,string status,char reltoclient = 'F')

        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateStatusByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1], client,status,reltoclient));
            Console.WriteLine("Update ClaimQAAudit from database if exists for claseq<{0}>", claSeq);

        }

        public bool IsImageIdFieldPresent() =>
            JavaScriptExecutor.IsElementPresent(ClaimActionPageObjects.ImageIdSectionCssSelector);

        public string GetImageId() =>
            JavaScriptExecutor.FindElement(ClaimActionPageObjects.ImageIdSectionCssSelector + " .data_point_value").Text;

        public int GetPreAuthCountFromDatabase(string patSeq)
        {
            return (int)Executor.GetSingleValue(Format(ClaimSqlScriptObjects.PreAuthCountByPatSequence, patSeq));
        }

        public List<string> GetAvailableFlagList()
        {
            return JavaScriptExecutor.FindElements(ClaimActionPageObjects.AvailableFlagList, How.CssSelector, "Text");

        }

        public string GetConfidenceScoreForFlage(string claimseq, string flag)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetConfidenceScoreForClaimAndFlag, claimseq,
                flag));
        }

        public bool IsDentalProfilerIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.DentalProfilerIconCssSelector,
                How.CssSelector);
        }
        public String GetDefaultSecondaryView()
        {
            return SiteDriver.FindElement(ClaimActionPageObjects.DefaultSecondaryView,How.CssSelector).Text;
        }


        #region Database
        public string GetDentalProfilerReview(string claseq, string clasub)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetDentalProfileReview, claseq, clasub));
        }
        public string GetEditTypeOfSystemDeletedFlagByClaseqDb(string claseq) =>
            Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.GetEditTypeOfSystemDeletedFlagByClaimSeq, claseq));

        public string GetCountOfSystemDeletedFlagByEditClaseqPrepepDb(string editflg, string claseq, string prepep) =>
            Executor.GetSingleStringValue(Format(
                ClaimSqlScriptObjects.GetCountOfSystemDeletedFlagsByEditFlgClaseqPrepep, editflg, claseq, prepep));
        public string GetProductOfSystemDeletedFlagByClaseqDb(string claseq) =>
            Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetProductOfSystemDeletedFlagByClaseq, claseq));

        public List<String> GetSugModifiersByClaimSeqAndEditFlagFromDatabase(string claimseq, string editFlag) 
        {
            var resultList =
            Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetSugModifierValueByClaimSeqAndEditFlag, claimseq,
                editFlag));
            var sugModifierList = new List<string>();

            foreach (DataRow row in resultList)
            {
                sugModifierList = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return sugModifierList;
        }

        public void UpdateRWROAccessToAuthorities(int attribute, string userId, string product)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateRWROAccessForAuthorities, attribute, userId, product));
            WaitForStaticTime(3000);
        }

        public void UpdateRWRORoleForAuthorities(string userId, string oldRole, string newRole,bool isInternalUser=true)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateRWRORoleForUser,  userId, oldRole,newRole,isInternalUser?2:8));
            WaitForStaticTime(3000);
        }

        public void AddRoleForAuthorities(string userId,string role, bool isInternalUser = true)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetRoleForUser, userId, role, isInternalUser ? 2 : 8));
            if (list==null)
            {
                Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.InsertRoleForUser, userId, role,
                    isInternalUser ? 2 : 8));
                WaitForStaticTime(3000);
            }
        }

        public void RemoveRoleForAuthorities(string userId, string role, bool isInternalUser = true)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteRoleForUser, userId, role, isInternalUser ? 2 : 8));
            WaitForStaticTime(3000);
        }

        public bool IsGetClientReviewFlagTrueOrFalse(string claseq, bool clientReviewFlagTrue = true)
        {
            var clientReviewFlagList = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetClientReviewFlag,
                claseq));
            if (clientReviewFlagTrue)
            {
                bool isClientReviewTrue = clientReviewFlagList.TrueForAll(x => x.Equals("Y"));
                return isClientReviewTrue;
            }
            else
            {
                bool isClientReviewTrue = clientReviewFlagList.TrueForAll(x => x.Equals("N"));
                return isClientReviewTrue;
            }
        }

        public bool IsFlagDental(string flag)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetEditFlagForClaim, flag)).Equals("D");
        }

        public void RestoreDeletedFlagsFromDB(string claseq1,string claseq2)
        {
             Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.RestoreDeletedFlagsFromDatabase, claseq1,claseq2));
        }

        public List<String> GetClaimsByClaimNoforClientUserFromDatabase(string claimNo)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimByClaimNoForClientUser, claimNo));
        }

        public List<String> GetClaimsByClaimNoFromDatabase(string claimNo)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimByClaimNo, claimNo));
        }

        public string GetSystemDateFromDatabase()
        {
            return Executor.GetSingleStringValue(ClaimSqlScriptObjects.GetSystemDateFromDatabase);
        }

        public bool IsClaimLockedByClaimSequence(string clientName,string claseq,string clasub,string userName)
        {
            var lockedClaimList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientName, userName));
            return lockedClaimList != null && lockedClaimList.Contains($"{claseq}-{clasub}");
        }

        public int GetRecordCountOfRealTimeClaimQueueAuditFromDb(string claseq) => Convert.ToInt16(Executor
            .GetSingleStringValue(Format(ClaimSqlScriptObjects.GetCountOfClaimQueueRecord, claseq.Split('-')[0], claseq.Split('-')[1])));

        public void DeleteRealTimeClaimQueueRecordFromDb(string claseq) =>
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteRealTimeClaimQueueRecord, claseq.Split('-')[0], claseq.Split('-')[1]));

        public void DeleteRealTimeClaimQueueRecordAuditFromDb(string claseq) =>
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteRealTimeClaimQueueAuditRecord, claseq.Split('-')[0], claseq.Split('-')[1]));

        public void DeleteClaimAuditRecordByClientAndClaseqFromDb(string client, string claseq) =>
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditRecordByClientAndClaimSeq, client,
                claseq.Split('-')[0], claseq.Split('-')[1]));

        public void UpdateAuditCompletedAndCompletedDateFromDb(string client, string auditCompleted,
            string completeDate, string claseq) => Executor.ExecuteQuery(Format(
            ClaimSqlScriptObjects.UpdateAuditCompletedAndCompletedDate, client, auditCompleted, completeDate,
            claseq.Split('-')[0], claseq.Split('-')[1]));

        public List<string> GetRecReasonCodeAndDpKeyFromDb(string claseq)
        {
            var list = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetRecReasonCodeAndDpKeyFromDb, claseq));
            var recReasonCodeAndDpKey = new List<string>();
            foreach (DataRow row in list)
            {
                recReasonCodeAndDpKey = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return recReasonCodeAndDpKey;
        }
        #endregion

        public string GetNDCValueByClaimSeqFromDb(string claseq, string clasub) =>
            Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetNDCValueByClaimSeq, claseq, clasub));
        public List<List<string>> GetDxCodeValuesAndDescriptionsFromDb(string claseq, string clasub, string linNo)
        {
            var list = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetDxCodesValuesDetailsinLineDetails, claseq, clasub,
                linNo));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public string GetDxCodeLabelByRow(int row)
        {
           return JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.DxCodesLabelInClaimLineDetailsByRowCssTemplate, row)).Text;
        }

        public bool IsDxCodePresentByLabel(string dxCode)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimActionPageObjects.DxCodesInClaimLineDetailsByLabelCssTemplate, dxCode),How.CssSelector);
        }

        public void ClickClaimLineIconInClaimLines()
        {
            SiteDriver.FindElement(ClaimActionPageObjects.ClaimLineIconInClaimLinesCssSelector, How.CssSelector).Click();
        }

        public void ScrollToLastPosition()
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDiv(Format(ClaimActionPageObjects.DxCodesLabelInClaimLineDetailsByRowCssTemplate,1));
        }

        public void MaximizeWindow()
        {
            SiteDriver.WebDriver.Manage().Window.Maximize();
        }

        public List<string> GetStatusListFromDropdown()
        {
            var element = JavaScriptExecutor.FindElement(Format(ClaimActionPageObjects.InputFieldByLabelCssTemplate, "Status"));
            element.Click();
            return JavaScriptExecutor.FindElements(string.Format(ClaimActionPageObjects.DropDownInputListByLabelXPathTemplate, "Status"),"Text",selector:How.XPath);
        }

        public bool IsSourcePresentForFlag(string flag)
        {
            return JavaScriptExecutor.IsElementPresent(Format(ClaimActionPageObjects.sourcevalueByFlag,flag));
        }

        public List<string> GetListOfAppealTypesInRows() =>
             JavaScriptExecutor.FindElements(ClaimActionPageObjects.AppealTypeListXpath, How.XPath, "Text");
        
    }
}



