using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Utils;
using static System.String;


namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderSearchPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS
        private readonly ProviderSearchPageObjects _providerSearchPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private ProviderActionPage _providerAction;
        private readonly string _originalWindow;
        
        #endregion

        #region CONSTRUCTOR

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public ProviderSearchPage(INewNavigator navigator, ProviderSearchPageObjects providerSearchPageObject, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerSearchPageObject, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerSearchPage = (ProviderSearchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
        }
        #endregion

        #region database
        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public List<string> GetSpecialtyList()
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.SpecialtyList));
        }

        public List<string> GetStateList()
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.StateList));
        }

        public List<string> GetConditionIDList()
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.ConditionIDList));
        }

        public void DeleteProviderLockByProviderSeq(string prvseq)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteProviderLockByProviderSeq, prvseq));
        }

        public List<List<string>> GetExport12MonthsHistoryExcelData(string prvseq, string claseq, string lineno)
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.Export12MonthsHistoryExcelData, prvseq, claseq, lineno));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            var age = GetAge(newdataList[0][13], newdataList[0][8]);
            //newdataList[0].RemoveAt(13);
            newdataList[0][13] = age;
            return newdataList;
        }

        public List<List<string>> GetExport3YearsHistoryExcelData(string prvseq)
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.Export3yearsHistoryExcelData, prvseq));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();

            for (int i = 0; i < newdataList.Count; i++)
            {
                var age = GetAge(newdataList[i][13], newdataList[i][8]);
                newdataList[i][13] = age;
            }

            //newdataList[0].RemoveAt(13);
            
            return newdataList;
        }

        public string GetExport12MonthsHistoryExcelFileNameFromDatabase(string prvseq, string claseq, string lineno)
        {
           var fileName = Executor.GetSingleStringValue(Format(ProviderSqlScriptObjects.GetExport12MonthsHistoryExcelFileName, prvseq, claseq, lineno));
            return fileName;
        }

        public string GetExport3YearsHistoryExcelFileNameFromDatabase(string prvseq)
        {
            var fileName = Executor.GetSingleStringValue(Format(ProviderSqlScriptObjects.Get3YearsHistoryExcelFileName, prvseq));
            return fileName;
        }

        public void DeleteDocumentAuditRecordFromDb(string prvseq, string userId)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteDocumentAuditAction, prvseq, userId));
        }

       

        #endregion

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public bool IsNewProviderSearchSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.ProviderSearch), How.XPath);
        }
        public void ClickOnDashboardIcon()
        {
            SiteDriver.FindElement(ProviderSearchPageObjects.DashboardIconXPath,How.XPath).Click();
        }


        public string GetAge(string PatDob, string BeginDos)
        {
           
            string ageString = Empty;
            int years = Convert.ToDateTime(BeginDos).Year - Convert.ToDateTime(PatDob).Year;
            int months = Convert.ToDateTime(BeginDos).Month - Convert.ToDateTime(PatDob).Month;

            if (months < 0)
            {
                years--;
                months = 12 + months;
            }
            switch (years)
            {
                case 0:
                    break;
                case 1:
                    ageString = Format("{0} yr", years);
                    break;
                default:
                    ageString = Format("{0} yrs", years);
                    break;
            }


            switch (months)
            {
                case 0:
                    break;
                case 1:
                    ageString += Format(" {0} mo", months);
                    break;
                default:
                    ageString += Format(" {0} mos", months);
                    break;
            }

            return ageString;
        }

        public string GetPageInsideTitle()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.PageInsideTitleCssLocator, How.CssSelector).Text;
        }

        public bool IsSideBarIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
        }

        public void SetInputFieldByInputLabel(string label, string value)
        {
            _sideBarPanelSearch.SetInputFieldByLabel(label, value);
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

        public void SetDateFieldFrom(string dateName, string date)
        {
            _sideBarPanelSearch.SetDateFieldFrom(dateName, date);
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

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(Format(ProviderSearchPageObjects.ButtonXPathTemplate, "Find"),
                How.XPath);
            Console.WriteLine("Find Button Clicked");
            WaitForWorkingAjaxMessage();
        }

        public void UpdateEdosDosIfOlderThanYearForProvSeqProcAndClaseq(string provseq, string proc, string claseq)
        {
            var temp = Format(ProviderSqlScriptObjects.UpdateEdosDosIfOlderThanYearForProvSeqProcClaseq, provseq, proc, claseq);
            Executor.ExecuteQuery(temp);
        }

        /// <summary>
        /// Click Clear All Button
        /// </summary>
        /// <returns></returns>
        public ProviderSearchPage ClearAll()
        {
            JavaScriptExecutor.ExecuteClick(ProviderSearchPageObjects.ClearFilterClass, How.CssSelector);
            Console.WriteLine("Clicked Clear Filter.");
            return new ProviderSearchPage(Navigator, _providerSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public void DeleteConditionActionAuditRecordFromDatabaseForCotiviti(string prvseq, string date)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteConditionActionForProviderSequenceAndDate, prvseq, date));
            Console.WriteLine("Delete Condition action audit record from database if exists for prvseq<{0}>", prvseq);

        }

        public void UpdateTriggeredConditionForCotiviti(string prvseq)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.UpdateTriggeredCondition, prvseq));
            Console.WriteLine("Delete Condition action audit record from database if exists for prvseq<{0}>", prvseq);
        }

        
        public ProviderActionPage ClickOnProviderSequenceAndNavigateToProviderActionPage(string providerName)
        {

            var providerAction = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(Format(ProviderSearchPageObjects.ProviderSequenceValueXPath), How.XPath);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                Console.WriteLine("Clicked on Provider Name<{0}>", providerName);
            });
            return new ProviderActionPage(Navigator, providerAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }



        public string IsLockIconPresentAndGetLockIconToolTipMessage(string prvSeq, string pageUrl,bool isInternal=true)
        {

            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();

            var loginPage = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            var login =isInternal? new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).LoginAsHciAdminUser(): 
                new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).LoginAsClientUser();
            login.NavigateToProviderSearch();
            SearchByProviderSequence(prvSeq);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => IsProviderLockIconPresent(prvSeq), 3000);
            var isProviderLockIconPresent = IsProviderLockIconPresent(prvSeq);
            var prvLockToolTipMessage = "";
            if (isProviderLockIconPresent)
                prvLockToolTipMessage =
                    SiteDriver.FindElement(
                        Format(ProviderSearchPageObjects.LockIconByProviderSequenceXPathTemplate, prvSeq),
                        How.XPath).GetAttribute("title");
            return prvLockToolTipMessage;
        }

        //public ProviderActionPage ClickOnProviderNameToOpenNewProviderActionForProvSeq(string provSeq)
        //{
        //    var newProviderActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
        //    {
        //        SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(
        //            Format(ProviderSearchPageObjects.ProviderNameTemplate, provSeq), How.XPath));
        //        SiteDriver.FindElement(Format(ProviderSearchPageObjects.ProviderNameTemplate, provSeq), How.XPath).Click();
        //        Console.WriteLine("Clicked on provider name to open provider action for provider sequence '{0}'", provSeq);
        //        WaitForSpinner();
        //        SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProviderActionPageObjects.BasicProviderDetailsDivCssSelector, How.CssSelector));
        //        WaitForWorkingAjaxMessageForBothDisplayAndHide();
        //    });
        //    return new ProviderActionPage(Navigator, newProviderActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        //}

        public ProviderActionPage ClickOnFirstProviderSeqToNavigateToProviderActionPage()
        {
            var providerActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                GetGridViewSection.ClickOnGridByRowCol(1,3);
                WaitForWorkingAjaxMessage();
            });
            return new ProviderActionPage(Navigator, providerActionPage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

    
       

        public bool IsProviderLockIconPresent(string prvSeq)
        {
            return SiteDriver.IsElementPresent(
                Format(ProviderSearchPageObjects.LockIconByProviderSequenceXPathTemplate, prvSeq), How.XPath);
        }
        public int GetLockedProviderCountByUser(string username,string clientname="SMTST")
        {
            return (int)Executor.GetSingleValue(Format(ProviderSqlScriptObjects.GetProviderCountByuser, clientname, username));
        }


        public void CloseNewTabIfExists()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            if (handles.Count != 2) return;
            SiteDriver.SwitchWindow(handles[0]);
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
        }

        public void SearchByProviderSequence(string prvSeq)
        {
            GetSideBarPanelSearch.ClickOnClearLink();
            GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
            GetSideBarPanelSearch.ClickOnFindButton();
            Console.WriteLine("Searched by PrvSeq<{0}>", prvSeq);
            WaitForWorkingAjaxMessage();
            // SiteDriver.WaitForCondition(() => IsProviderSequenceDisplayed(prvSeq));
            SiteDriver.WaitToLoadNew(500);

        }

        public bool IsProviderSequenceDisplayed(string prvseq)
        {
            return (GetGridViewSection.GetValueInGridByColRow(3) == prvseq);

        }

        public bool IsReviewIconByRowPresent(int i = 1)
        {
            return SiteDriver
                .IsElementPresent(Format(ProviderSearchPageObjects.ReviewIconByRowCssSelector, i),
                    How.CssSelector);
        }

        public string GetReviewIconTooltip()
        {
            return SiteDriver
                .FindElement(Format(ProviderSearchPageObjects.ReviewIconByRowCssSelector, 1),
                    How.CssSelector).GetAttribute("title");
        }


        public bool IsAppropriateScoreIconPresent(String risk, int i = 1)
        {
            return SiteDriver.FindElement(Format(ProviderSearchPageObjects.ScoreIconByRowXpath, i), How.XPath)
                .GetAttribute("class").Contains(risk);
        }




        public string GetProviderScoreByRow(int i = 1)
        {
            return SiteDriver
                .FindElement(Format(ProviderSearchPageObjects.SearchResultScoreCssSelector, i),
                    How.CssSelector).Text;
        }

        public List<string> GetProviderScoreList()
        {
            return JavaScriptExecutor.FindElements(ProviderSearchPageObjects.ProviderScoreListCssSelector, How.CssSelector, "Text");

        }

        public string GetProviderDetailsValueByLabel(string label)
        {
            Console.WriteLine("Get value of provider details from details section {0}", label);
            return JavaScriptExecutor.FindElement(Format(ProviderSearchPageObjects.ProviderDetailsValueByLabelCssLocator, label)).Text;
        }

        public void ClickOnSearchListRowByProviderSequence(string prvSequence)
        {
            Console.WriteLine("Clicked on provider result list with provider sequence :{0}", prvSequence);
            JavaScriptExecutor.ExecuteClick(Format(ProviderSearchPageObjects.SearchListByProviderSeqSelectorTemplate, prvSequence), How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public bool IsSelectedProviderRowHighlighted(string prvSequence)
        {
            return SiteDriver.FindElement(Format(ProviderSearchPageObjects.SelectedProviderSearchResultRowXpath, prvSequence), How.XPath).
                GetAttribute("class").Contains("is_active");
        }
        public string GetProviderDetailHeader()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.ProviderDetailsHeaderCssSector,
                How.CssSelector).Text;
        }

        public List<string> GetProviderNameList()
        {
            return JavaScriptExecutor.FindElements(ProviderSearchPageObjects.ProviderNameListCssSector, How.CssSelector, "Text").ToList();
        }

        public string TotalCountOfSuspectProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.SuspectProviderCount)[0];

        }

        public string TotalCountOfClientSuspectProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.ClientSuspectProviderCount)[0];

        }
        public string TotalCountOfCotivitiFlaggedProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.CotivitiFlaggedProviderCount)[0];

        }

        public int TotalCountOfCotivitiFlaggedProvidersByTriggeredDateFromDatabase()
        {
            return Convert.ToInt32(Executor.GetTableSingleColumn(ProviderSqlScriptObjects.GetCotivitiFlaggedProvidersByTriggeredDateCount)[0]);

        }

        public int TotalCountOfClientFlaggedProvidersByTriggeredDateFromDatabase()
        {
            return Convert.ToInt32(Executor.GetTableSingleColumn(ProviderSqlScriptObjects.GetClientFlaggedProvidersByTriggeredDateCount)[0]);

        }

        public string TotalCountOfClientFlaggedProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.ClientFlaggedProviderCount)[0];

        }

        public List<string> GetAllProvidersNameFromDataBase(string first_name,string last_name)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetProvidersByFirstNameAndLastName,first_name,last_name));
        }

        public List<string> GetCotivitiFlaggedProvidersNameFromDataBase(string first_name, string last_name)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetCotivitiFlaggedProvidersByFirstNameAndLastName, first_name, last_name));

        }


        public List<string> GetSuspectProvidersNameFromDataBase(string first_name, string last_name)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetSuspectProvidersByFirstNameAndLastName, first_name, last_name));
        }

        public List<string> GetClientFlaggedProvidersNameFromDataBase(string first_name, string last_name)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetClientFlaggedProvidersByFirstNameAndLastName, first_name, last_name));
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

        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public string GetFilterOptionTooltip()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(ProviderSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }
        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(ProviderSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;


        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(
                ProviderSearchPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public List<String> GetSearchResultListByCol(int col)
        {

            var list = GetGridViewSection.GetGridListValueByCol(col);
            list.RemoveAll(IsNullOrEmpty);
            return list;
        }
        public bool IsListIntSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(int.Parse).ToList().IsInAscendingOrder();
        }

        public bool IsListIntSortedInDescendingOrder(int col)
        {
            var list = GetSearchResultListByCol(col).ToList().Select(int.Parse).ToList();
            return list.IsInDescendingOrder();
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInAscendingOrder();

        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public List<bool> GetProviderOnReviewList()
        {
            ClickOnFilterOption();
            var list = SiteDriver.FindElementsAndGetAttributeByClass("eyeball",
                ProviderSearchPageObjects.ReviewIconListCssTemplate, How.CssSelector);
            ClickOnFilterOption();
            return list;

        }
        public bool IsProviderHistoryIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.ProviderHistoryIconCssLocator, How.CssSelector);
        }

        public bool IsProviderNotesIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.NotesIconCssLocator, How.CssSelector);
        }
        

        public string GetProviderHistoryIconTooltip()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.ProviderHistoryIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetProviderNotesIconTooltip()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.NotesIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public void ClickOnProviderNotesIcon()
        {
            SiteDriver.FindElement(ProviderSearchPageObjects.NotesIconCssLocator, How.CssSelector).Click();
        }
        
        public bool IsProviderNoteContainerPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.NotesContainerXpathLocator, How.XPath);
        }

        public List<string> GetNoteAttributeValueListInTheNoteContainer(int col)
        {
            return JavaScriptExecutor.FindElements(Format(ProviderSearchPageObjects.ProviderNoteRowAttributeCssLocatorTemplate, col), How.CssSelector, "Text");
        }

        public int GetNoteListCount()
        {
            return SiteDriver.FindElementsCount(ProviderSearchPageObjects.NoteListCssLocator, How.CssSelector);
        }

        public int GetVisitAllFromDatabase(string prvseq)
        {
            return int.Parse(Executor.GetSingleValue(Format(ProviderSqlScriptObjects.GetVisitAllDetail, prvseq)).ToString());
        }

        

        public string TotalCountofProviderNotes(string prvSeq)
        {
            var list = Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.TotalCountOfProviderNotes, prvSeq));
            Console.WriteLine("Total count of Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();
        }

        public List<string> GetProviderTriggeredConditions(string prvSeq)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.ProviderTriggeredConditions, prvSeq));
        }

        public string CountOfConditionNotesAssociatedToTheConditionFromDatabase(string prvseq, string subType)
        {
            var list = Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.CountOfConditionNotesAssociatedToTheCondition, prvseq, subType));
            return list[0];

        }

        public bool IsCarrotIconPresentInAllProviderNotes()
        {           

            List<bool> list = SiteDriver.FindElementsAndGetAttributeByClass("small_caret_right_icon",
                 ProviderSearchPageObjects.CarrotIconCssSelector, How.CssSelector);
            if (list.Count == 0)
                return false;
            return list.All(c => c);
        }

        public void ClickOnCarrotIconOnProviderNotes(int row = 1)
        {
            SiteDriver.FindElement(Format(ProviderSearchPageObjects.CarrotIconRowwiseCssLocatorTemplate, row), How.CssSelector).Click();
        }

        public void ClickOnExpandCarrotIconOnProviderNotes()
        {
            SiteDriver.FindElement(ProviderSearchPageObjects.CarrotDownIconCssSelector, How.CssSelector).Click();
        }
        public void DeleteProviderNotesOnly(string prvSeq, string userName)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteProviderNotesOnly, prvSeq, userName));
            Console.WriteLine("Delete Note of type Provider from database if exists for prvseq<{0}>  ", prvSeq);
        }
        public void UpdateTriggeredConditionForClient(string prvseq)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.UpdateTriggeredConditionForClient, prvseq));
            Console.WriteLine("Delete Condition action audit record from database if exists for prvseq<{0}>", prvseq);
        }
        public void DeletePRVQueueData(string conditionId, string prvSeq, string clientCode)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeletePRVQueueData, conditionId, prvSeq, clientCode));

        }
        public void UpdateProviderActionAndOpenCtiCase(string prvSeq)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.UpdateProviderActionAndOpenCTICase, prvSeq));
        }

        public string GetJobFlagValueFromDatabase(string client)
        {
            return Executor.GetSingleStringValue(Format(ProviderSqlScriptObjects.GetJobFlag, client));
        }

        public void DeleteConditionIdByProviderSequence(string prvseq)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteConditionId, prvseq));
        }
        public void DeleteConditionActionAuditRecordFromDatabaseForClient(string prvseq, string date)
        {
            Executor.ExecuteQuery(
                Format(ProviderSqlScriptObjects.DeleteConditionActionForProviderSequenceForClient, prvseq,
                    EnvironmentManager.ClientUserName, date));
            Console.WriteLine("Delete Condition action audit record from database if exists for prvseq<{0}>", prvseq);

        }

        public string GetNoNotesMessage()
        {
            return SiteDriver.FindElement(ProviderSearchPageObjects.ProviderNoNotesCssSelector, How.CssSelector).Text;
        }
        
        public ProviderClaimHistoryPage ClickOnProviderHistoryIcon(string prvSeq)
        {            
            var providerClaimHistory = Navigator.Navigate<ProviderClaimHistoryPageObjects>(() =>
            {
                SiteDriver.FindElement(ProviderSearchPageObjects.ProviderHistoryIconCssLocator, How.CssSelector).Click();
                Console.WriteLine("Clicked on History Icon of Provider Sequence : {0}",prvSeq);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimHistoryPopup.GetStringValue()));
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderClaimHistoryPage(Navigator, providerClaimHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            );
        }

        public ProviderClaimSearchPage SearchByProviderSequenceAndNavigateToProviderClaimSearchPage(string providerSeq)
        {
            SearchByProviderSequence(providerSeq);
            _providerAction = ClickOnFirstProviderSeqToNavigateToProviderActionPage();
            return _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();
        }

        public ProviderActionPage SearchByProviderSequenceAndNavigateToProviderActionPage(string providerSeq)
        {
            SearchByProviderSequence(providerSeq);
            return ClickOnFirstProviderSeqToNavigateToProviderActionPage();
        }

        public void CloseAllExistingPopupIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderSearch.GetStringValue());
                }
            }
        }

        public bool IsProviderFlaggingDropdownPresent(string label)
        {
            return GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(label);
        }

        public bool IsMedawareIconPresent()
        {
            List<bool> list = SiteDriver.FindElementsAndGetAttributeByClass("alerted",
                 ProviderSearchPageObjects.ProviderProfileIconListCssTemplate, How.CssSelector);
            if (list.Count == 0)
                return false;
            return list.All(c => c);
          
        }
        public string GetMedwareIconTooltip(int row=1)
        {
            return SiteDriver.FindElementAndGetAttribute(Format(ProviderSearchPageObjects.MedawareIconCssTemplate,row), How.CssSelector,"title");
        }

        public void ClickOnClearLink()
        {
            SiteDriver.WaitToLoadNew(1000);
            JavaScriptExecutor.ExecuteClick(ProviderSearchPageObjects.ClearFilterClass, How.CssSelector);
            SiteDriver.WaitToLoadNew(2000);
            Console.WriteLine("Clear Link Clicked");
        }

         public string TotalCountOfClientFlaggedProvidersByActionFromDatabase(string action)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.ClientFlaggedProviderCountByClientAction,action))[0];

        }

         
        #region NAVIGATETOSCORECARD

        public NewProviderScoreCardPage ClickOnGridScoreToOpenScoreCard(int rowId=1)
        {
            var providerScoreCard = Navigator.Navigate<NewProviderScoreCardPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(ProviderSearchPageObjects.GridScoreLocatorXpathTemplate, rowId), How.XPath).Click();
                Console.WriteLine("Clicked on View ScoreCard for row id = " + rowId);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderScoreCard.GetStringValue()));
            });
            return new NewProviderScoreCardPage(Navigator, providerScoreCard, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

       
        #endregion

        public ProviderActionPage NavigateToProviderAction(Action function)
        {
            var _newProviderActionPage = Navigator.Navigate<ProviderActionPageObjects>(function);
            return new ProviderActionPage(Navigator, _newProviderActionPage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }
        
        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(ProviderSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(ProviderSearchPageObjects.FindButtonCssLocator,
                                 ProviderSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public void SelectDropDownValueByLabel(string label, string value, bool directSelect = true)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel(label, value, directSelect);
        }

        public void ClickOnProvSeqByRowCol(int row, int col)
        {
            _gridViewSection.ClickOnGridByRowCol(row, col);
        }

        public bool IsExclamationPresentInProviderProfile(int row = 1)
        {
            return SiteDriver.IsElementPresent(Format(ProviderSearchPageObjects.MedawareIconCssTemplate, row),How.CssSelector);
        }

        public bool IsExportIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.ProviderSearchExportIconCssSelector, How.CssSelector);
        }



        public void ClickOnExportIcon()
        {
            JavaScriptExecutor.FindElement(ProviderSearchPageObjects.ProviderSearchExportIconCssSelector).Click();
            Console.WriteLine("Clicked on Export option");
            WaitForStaticTime(100);
        }

        public bool IsExportIconDisabled()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.DisabledExportIconCss, How.CssSelector);
        }

        public bool IsExportIconEnabled()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.EnabledExportIconCss, How.CssSelector);
        }


        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<ProviderSearchPage>();
            return fileName;

        }

        public List<List<string>> GetExcelDataList(string prvSeq)
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.GetExcelExport,prvSeq));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;
            
        }

        public List<string> GetProviderExportAuditListFromDB(string username)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetExcelDownloadAudit, username));
        }

        public void UpdateActiveStatus(char status = 'Y', bool isClientFlagged = false)
        {
            var action = (isClientFlagged) ? "client_action" : "action";
            Executor.ExecuteQuery(string.Format(ProviderSqlScriptObjects.UpdateFlagStatus, status, action));
        }
    }
}
