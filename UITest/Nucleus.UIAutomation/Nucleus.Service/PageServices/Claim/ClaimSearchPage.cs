using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.QuickLaunch;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageServices.Base.Default;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;
using Nucleus.Service.SqlScriptObjects.Claim;
using UIAutomation.Framework.Database;
using System.Data;
using System.Drawing;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.QA;
using Nucleus.Service.Support.Environment;
using static System.String;
using static System.Console;

namespace Nucleus.Service.PageServices.Claim
{
    public class ClaimSearchPage : NewDefaultPage 
    {
        #region PUBLIC FIELDS

        public readonly ClaimSearchPageObjects _newClaimSearchPage;


        #endregion

        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly NewPagination _pagination;
        private readonly GridViewSection _gridViewSection;
        private readonly SwitchClientSection _switchClientSection;

        #region CONSTRUCTOR

        public ClaimSearchPage(INewNavigator navigator, ClaimSearchPageObjects newClaimSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, newClaimSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            //_newClaimSearchPage = (ClaimSearchPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _switchClientSection = new SwitchClientSection(SiteDriver, JavaScriptExecutor);
        }

        #endregion
        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }
        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public NewPagination GetPagination
        {
            get { return _pagination; }
        }

        public SwitchClientSection SwitchClientSection
        {
            get { return _switchClientSection; }
        }

        public void MouseOutAppealMenu() => GetMouseOver.MouseOutAppealMenu();


        #region database methods

        public List<List<string>> GetStatusReltoClientByClaseqDb(string claseq)
        {
            var list = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetStatusRelToClientByClaseq, claseq));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }
        public List<string> GetAssociatedClaimSubStatusForClient(string client)
        {
            var subStatusList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClientWiseClaimSubStatusList, client));
             WriteLine("The count of sub status associated with internal user: {0} is {1}", client, subStatusList[0]);
            return subStatusList;
        }

        public List<string> GetAssociatedClaimSubStatusForInternalUser(string client)
        {
            var subStatusList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.InternalUserClaimSubStatusList, client));
             WriteLine("The count of sub status associated with internal user: {0} is {1}", client, subStatusList[0]);
            return subStatusList;
        }

        public List<string> GetAssociatedClaimSubStatusForInternalUserWithDCIInactive(string client)
        {
            var subStatusList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.InternalUserClaimSubStatusListForDCIInactiveClient, client));
            return subStatusList;
        }

        public List<string> GetAssociatedPlansForClient(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClientWisePlanList, client));
        }

        public List<string> GetAssignedToList(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.AssignedToList, client));
        }

        public List<string> GetClientAssignedToList(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClientUserAssignedToList, client));
        }

        public List<string> GetUnreviewedClaimListForInternalUser(string client,string user)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfUnreviewedClaimsForInternalUser, client,user));
        }

        public List<string> GetUnreviewedClaimListForClientUser(string client,string user)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfUnreviewedClaimsForClientUser, client, user));
        }

        public List<string> GetPendedClaimsCount(string client, string user)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfPendedClaims, client, user));
        }

        public List<string> GetPendedClaimsCountforClient(string client, string user)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfPendedClaimsClient, client, user));
        }

        public List<string> GetReviewGroup(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ReviewGroupList, client));
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

        public bool IsClaimSearchResultsLocked(string userId, List<string> claimSearchList,string clientName="SMTST")
        {
            var lockedClaimList = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientName, userId));
            if (lockedClaimList == null)
                return false;
            return claimSearchList.Any(x => lockedClaimList.Contains(x));

        }

        public bool IsClaimLocked(string clientname, string claseq, string clasub, string userSeq)
        {
            var lockedClaimList =
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.LockedClaimListByUser, clientname, userSeq));
            if (lockedClaimList !=null)
                return lockedClaimList.Any(x => x.Contains($"{claseq}-{clasub}"));
            else
            {
                return false;
            }
        }

        public int GetCountOfClaseqFromBatchId(string batchId)
        {
            int claimCount = Int32.Parse(Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetCountOfClaseqFromBatchId,
                batchId)));
            return claimCount;

        }
        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public void UpdateClaimStatusToUnreviewedFromDatabase(string claimNo)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateStatusToUnreviewed, claimNo));
             WriteLine("Update Claim Status from database claim nos ", claimNo);
        }

        public List<String> GetClaimsByClaimNoforClientUserFromDatabase(string claimNo)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimByClaimNoForClientUser, claimNo));
        }
        public void DeleteClaimAuditOnlyByClaimNo(string claimno, string date)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimAuditOnlyByClaimNo, claimno, date));
             WriteLine("Delete Audit from database if exists for claimno<{0}> date greater than <{1}> ", claimno, date);

        }

        public List<string[]> GetListOfClaimsForPciWorklist(List<string> claimSeqList)
        {
            //return Executor.GetTableSingleColumn(string.Format(ClaimSqlScriptObjects.ListOfClaimsForPciWorklist, String.Join(",", claimSeqList.Select(x => string.Format("'{0}'", x)))));
            var list =
                Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.ListOfClaimsForPciWorklist, Join(",", claimSeqList.Select(x => Format("'{0}'", x)))));
            var results =
                                        list.Select(dr =>
                                                dr.ItemArray
                                                    .Select(x => x.ToString())
                                                    .ToArray())
                                            .ToList();
            return results;

        }

        public List<string> GetAssociatedBatchList(string clientCode)
        {
            var list= Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.BatchIDListByClient, clientCode)).ToList();
            return list;
        }

        public List<string> GetAppealCountByClaseqAppealStatus(string claseqList,string status)
        {
            return  Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetAppealCountByClaseqAppealStatus, claseqList,status));
        }

        public List<string> GetDataWithAppealCountByCol(int col) =>
            JavaScriptExecutor.FindElements(Format(ClaimSearchPageObjects.GridDataWithAppealCountCssSelectorTemplate,
                col));
        //public string GetAppealCountByClaimnoAppealStatus(string status, string claimnoList)
        //{
        //    var appealCount = Executor.GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetAppealCountByClaimnoAppealStatus, status, claimnoList));
        //    return appealCount;
        //}

        public string GetCountOfFlaggedClaimsByFlagName(string clientCode, string username, string edits)
        {
            return Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.CountOfSpecificFlaggedClaims, clientCode, username, edits));
        }
        
        public string GetCountOfUnreviewedClaimsForSpecificFlag(string clientCode, string username, string edits)
        {
            return Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.GetCountOfUnreviewedClaimsForSpecificFlag, clientCode, username, edits));
        }

        public string GetCountOfFlaggedClaimsByFlaggedNameForClient(string clientCode, string username, string edits)
        {
            return Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.GetCountOfSpecificFlaggedClaimsForClient, clientCode, username, edits));
        }
        
        public string GetCountOfUnreviewedClaimsForSpecificFlagForClient(string clientCode, string username, string edits)
        {
            return Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.GetCountOfUnreviewedClaimsForSpecificFlagForClient, clientCode, username, edits));
        }

        public List<string> GetListOfQAReadyClaimsFromDatabase()
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetClaimsWithQAAuditRecordFromDatabase,EnvironmentManager.Username));
        }

        public string GetCountOfUnreviewedClaimsForNoRestrictionDb(string userName)
        {
            return Executor.GetSingleStringValue(
                Format(ClaimSqlScriptObjects.CountOfUnreviewedClaimsResultForNoRestriction, userName));
        }

        public List<List<string>> GetResultsForSpecificRestrictionFilterForUnreviewedClaimsDb(string userId)
        {
            var restrictionlist =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
            var list = new List<List<string>>();

            foreach (var restriction in restrictionlist)
            {
                var searchresult =
                    Executor.GetTableSingleColumn(string.Format(
                        ClaimSqlScriptObjects.UnreviewedClaimsResultForSpecificRestriction,
                        userId, restriction));
                list.Add(searchresult);
            }

            return list;
        }

        public List<List<string>> GetResultsForSpecificRestrictionFilterForOutstandingQADb(string userId)
        {
            var restrictionlist =
                Executor.GetTableSingleColumn(string.Format(QASqlScriptObjects.AssignedRestrictionForUser, userId));
            var list = new List<List<string>>();

            foreach (var restriction in restrictionlist)
            {
                var searchresult =
                    Executor.GetTableSingleColumn(string.Format(
                        ClaimSqlScriptObjects.OutstandingClaimsQAResultForSpecificRestriction,
                        userId, restriction));
                list.Add(searchresult);
            }

            return list;
        }
        
        public List<string> GetResultsForAllRestrictionFilterForOutstandingQADb(string userId)
        {
            return
                Executor.GetTableSingleColumn(
                    string.Format(ClaimSqlScriptObjects.OutstandingClaimsQAResultForAllRestriction, userId));
        }

        public List<string> GetResultsForNoRestrictionFilterForOutstandingQADb(string userId)
        {
            return
                Executor.GetTableSingleColumn(
                    string.Format(ClaimSqlScriptObjects.OutstandingClaimsQAResultForNoRestriction, userId));
        }

        #endregion

        #region PUBLIC METHODS
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
                 WriteLine("Alert Box Accepted");
            }
            else
            {
                 WriteLine("Alert Box wasn't found.");
            }
        }
       
        public void SetClaimSequenceInFindClaimSection(string claimSeq)
        {
            if (!_sideBarPanelSearch.IsSideBarPanelOpen()) ClickOnSidebarIcon();
            SiteDriver.FindElement(ClaimSearchPageObjects.ClaimSequenceInputXPath, How.XPath).ClearElementField();
            SiteDriver.FindElement(ClaimSearchPageObjects.ClaimSequenceInputXPath, How.XPath).SendKeys(claimSeq);
             WriteLine("Claim sequence set to " + claimSeq);
        }

        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public void ClickonClaimSequenceInFindClaimSectionTextField()
        {
            var element = SiteDriver.FindElement(ClaimSearchPageObjects.ClaimSequenceInputXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();

        }

        public void SetClaimNoInFindClaimSection(string claimNo)
        {
            if (!_sideBarPanelSearch.IsSideBarPanelOpen()) ClickOnSidebarIcon();
            var element = SiteDriver.FindElement(ClaimSearchPageObjects.ClaimNoInputXPath, How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(claimNo);
            WriteLine("Claim no set to " + claimNo);
        }

        public void SetOtherClaimNoInFindClaimSection(string otherClaimNo)
        {
            var element = SiteDriver.FindElement(ClaimSearchPageObjects.OtherClaimNoInputXPath, How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(otherClaimNo);
            WriteLine("Other Claim no set to " + otherClaimNo);
        }

        public string GetOtherClaimNoInFindClaimSection()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.OtherClaimNoInputXPath, How.XPath).GetAttribute("value");

        }

        public bool IsOtherClaimNumberFieldDisplayed()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.OtherClaimNoInputXPath, How.XPath).Displayed;
        }

        public string GetNoMatchingRecordFoundMessage()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.NoMatchingRecordFoundCssSelector,
                How.CssSelector).Text;
        }

        public void ClickOnClearLinkOnFindSection()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.ClearLinkOnFindClaimSectionCssLocator, How.CssSelector);
            
        }

        public int GetSearchResultRowCount()
        {
            return SiteDriver.FindElementsCount(ClaimSearchPageObjects.SearchResultRowCssLocator, How.CssSelector);
        }
        public ClaimActionPage ClickOnClaimSequenceOfSearchResult(int row)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(AppealCreatorPageObjects.ClaimActionLinkOnSearchResultTemplate, row), How.CssSelector).Click();
                 WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnFindButtonAndNavigateToClaimAction()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(()=>
            {
                SiteDriver.FindElement(ClaimSearchPageObjects.FindXPath, How.XPath).Click();
                 WriteLine("Search for single claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public List<string> GetClaimSeqListOnSearchResult()
        {
            var list=
                JavaScriptExecutor.FindElements(ClaimActionPageObjects.ClaimActionsListOnSearchResult,
                    How.CssSelector, "Text");
            return list;
        }

        public int GetSearchResultCount()
        {

            return
                SiteDriver.FindElementsCount(ClaimActionPageObjects.ClaimActionsListOnSearchResult,
                    How.CssSelector);
        }

        

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.FindXPath, How.XPath);
            WaitForWorking();
        }

        public void SearchByOtherClaimNmber(string otherClaimNumber)
        {
            SetOtherClaimNoInFindClaimSection(otherClaimNumber);
            ClickOnFindButton();
            WaitForWorking();
        }

        public void SearchByClaimNmber(string claimNumber)
        {
            SetClaimNoInFindClaimSection(claimNumber);
            ClickOnFindButton();
            WaitForWorking();
        }
        public void SearchByClaimSequence(string claimSeq)
        {
            SetClaimSequenceInFindClaimSection(claimSeq);
            ClickOnFindButton();
            WaitForWorking();
        }

        public bool IsWorkListControlDisplayed()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.WorkListControlCssLocator, How.CssSelector);
        }
        public bool IsWorkListIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimActionPageObjects.WorkListCssLocator, How.CssSelector);
        }

        public void ClickWorkListIcon()
        {
            SiteDriver.WaitForCondition(IsWorkListIconPresent);
            JavaScriptExecutor.ExecuteClick(ClaimActionPageObjects.WorkListCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on WorkList Icon");
        }
        public ClaimActionPage ClickSearchIcon()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.WaitForIe(2000);
                SiteDriver.FindElement(ClaimSearchPageObjects.SearchIconCssLocator,How.CssSelector).Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Clicked on Search Icon");

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SearchByClaimSequenceToNavigateToClaimActionPage(string claimSeq,bool handlePopup=false)
        {
            if (!IsWorkListControlDisplayed())
                    ClickWorkListIcon();
            if (SiteDriver.IsElementPresent(ClaimSearchPageObjects.SearchIconCssLocator, How.CssSelector))
                ClickSearchIcon();
            var claimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SearchByClaimSequence(claimSeq);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
                if(handlePopup)
                    CloseAnyPopupIfExist();
            });
            return new ClaimActionPage(Navigator, claimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);

        }
        

        public ClaimActionPage SearchByClaimSequenceToNavigateToClaimActionForClientSwitch(string claimSeq, bool handlePopup = false)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SearchByClaimSequence(claimSeq);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("/claims"));
                string element = "section.claim_action";
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(element, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);

        }
        public ClaimActionPage ClickOnClaimSequenceByClaimSequence(string claimSeq)
        {
            
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(ClaimSearchPageObjects.ClaimSequenceXPathTemplate,claimSeq), How.XPath).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                 WriteLine("Clicked on Claim Seq<{0}>",claimSeq);
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        
        }

        public void ResetFlagStatusOfClaim(string claseq,bool hcidone=false,bool clidone=false,bool reltoclient=false)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.ResetFlagStatus,hcidone==true?"T":"F",clidone == true ? "T" : "F", claseq.Split('-')[0], reltoclient == true ? "T" : "F"));
        }

        public string GetFlagStatusForInternalUser(string claseq,string clasub)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetFlagstatusForInternalUser, claseq,
                clasub));
        }
        public string GetFlagStatusForClientUser(string claseq, string clasub)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetFlagstatusForClientUser, claseq,
                clasub));
        }
        public string GetClaimDetailsValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(Format(ClaimSearchPageObjects.ClaimDetailsXPathTemplate, label),
                    How.XPath).Text;
        }

        public string GetClaimTypeofH()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClaimTypeHXpath, How.XPath).Text;
        }

        public string GetClaimTypeofU()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClaimTypeUXpath, How.XPath).Text;
        }

        public void ClickSearchResultRow(int row=1)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimSearchPageObjects.SearchResultRowCssTemplate,row), How.CssSelector);
        }

        public void ClickSearchResultRowByValue(string claseq)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimSearchPageObjects.SearchResultRowByValueXpathTemplate, claseq), How.XPath);
        }
        public void ClickOnClaimSearchResultByRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(Format(ClaimSearchPageObjects.ClaimSearchResultByRowXPath, row), How.CssSelector);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnClaimSearchResultByClaseq(string claseq)
        {
            JavaScriptExecutor.FindElement(Format(ClaimSearchPageObjects.claimsearchResultWithClaseqByCss, claseq)).Click();
            WaitForWorkingAjaxMessage();
        }

        public string GetProviderNameInProviderdetailsByRowCol(int row = 1, int col = 1)
        {
            return JavaScriptExecutor.FindElement(Format(ClaimSearchPageObjects.GetValueFromClaimDetailsJsCssSelector, row, col)).Text;
        }
        public string GetProviderSeq()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ProvXpath, How.XPath).GetAttribute("title");
        }

        public string GetMemberId()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.MemberIdByXpath, How.XPath).GetAttribute("title");
        }

        public string GetLineOfBusinessfromClaimDetails()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.LOBXpath, How.XPath).Text;
        }

        public string GetBatchId()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.BatchIDXpath, How.XPath).Text;
        }

        public List<string> GetClaimSequenceByMemberIdFromDB(string patnum)
        {
            return  Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetClaimSequenceInClaimSearchByMemberId, patnum));
        }

        public string GetClaimNo()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClaimNoXpath, How.XPath).Text;
        }

        public string GetClientBatchId()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientBatchIDCssSelector, How.CssSelector).Text;
        }

        public bool IsAppealCountPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.AppealCountByRowXPathTemplate, row),
                How.XPath);
        }
        public string GetClaimSubStatus()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClaimSubStatusCssLocator, How.CssSelector).Text;
        }

        public string GetClientUserClaimSubStatus()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientUserClaimSubStatusCssLocator, How.CssSelector).Text;
        }

        public string GetPlanName()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.PlanXpath, How.XPath).Text;
        }

        public string GetAssignedTo()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.AssignedToXpath, How.XPath).Text;
        }

        public string GetInternalUserClaimSeq()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.InternalUserClaimSeqCssLocator, How.CssSelector).Text;
        }

        public string GetInternalUserClaimSeqFlaggedClaims()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.FlaggedClaimCssSelector, How.CssSelector).Text;
        }

        public string GetClientUserClaimNumber(int row = 1)
        {
            return SiteDriver.FindElement(Format(ClaimSearchPageObjects.ClientUserClaimNumberCssLocator, row), How.CssSelector).Text;
        }

        public string GetClientPlanName()
        {                    
                return SiteDriver.FindElement(ClaimSearchPageObjects.ClientPlanNameXpath, How.CssSelector).Text;            
        }

        public string GetClientLOBName()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientLOBNameXpath, How.XPath).Text;
        }

        public bool IsClientLOBPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.ClientLOBNameXpath, How.XPath);
        }

        public string GetClientReviewGroup()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientReviewGroupXpath, How.XPath).Text;
        }

        public string GetClientFormType()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientFormTypeXpath, How.XPath).Text;
        }

        public string GetClientProvSeq()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientProvSeqXpath, How.XPath).Text;
        }

        public string GetClientTIN()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientTINXpath, How.XPath).Text;
        }

        public string GetClientMemberId()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientMemberIdXpath, How.XPath).Text;
        }

        public string GetClientClaimSeq()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientClaimSeqXpath, How.XPath).Text;
        }

        public string GetClientBatchID()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClientBatchIDXpath, How.XPath).Text;
        }

        public bool GetClientReceievedDate()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.ClientReceivedDateXpath, How.XPath);
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


        public List<string> GetClaimStatus()
        {
             var list = JavaScriptExecutor.FindElements(
                  Format(ClaimSearchPageObjects.ClaimStatusXPath), How.XPath, "Text"); 
            return list; 
        }


        public List<string> GetClaimStatusforClient()
        {
            var list = JavaScriptExecutor.FindElements(
                 Format(ClaimSearchPageObjects.ClientClaimStatusXpath), How.XPath, "Text");
            return list;
        }
        

        public void ClickOnFindClaimIcon()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.FindClaimsIconCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "Find Claim"), How.XPath));
        }

        public void ClickOnPciWorkList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
             WriteLine("Clicked on Worklist filter carot icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "CV Work List"),
                    How.XPath), 10000);
            JavaScriptExecutor.ExecuteClick(
                Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "CV Work List"), How.XPath);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "CV Work List"), How.XPath),10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "CV Work List"), How.XPath).Click();
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
            WriteLine("Clicked on Worklist filter carot icon.");
        }

        public void ClickOnFciWorkList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
             WriteLine("Clicked on Worklist filter carot icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FCI Work List"),
                    How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FCI Work List"), How.XPath).Click();
             WriteLine("Clicked on FCI Work List Filter.");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FCI Work List"), How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FCI Work List"), How.XPath).Click();
        }


        public void ClickOnFFPWorkList()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.SideBarWorklistTypeCarotCssLocator, How.CssSelector);
             WriteLine("Clicked on Worklist filter carot icon.");
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(
                    Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FFP Work List"),
                    How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.WorkListTypeSelectorXpathTemplate, "FFP Work List"), How.XPath).Click();
             WriteLine("Clicked on FFP Work List Filter.");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FFP Work List"), How.XPath), 10000);
            SiteDriver.FindElement(Format(ClaimSearchPageObjects.SidebarHeaderXpathTemplate, "FFP Work List"), How.XPath).Click();
        }

        public ClaimActionPage CreatePciWorklistForUnreviewedClaim()
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickOnPciWorkList();
                _sideBarPanelSearch.ClickOnClearLink();
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status", "Unreviewed");
                _sideBarPanelSearch.ClickOnButtonByButtonName("Create");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public ClaimActionPage CreatePciWorklistWithReviewGroups(string reviewGroups)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                ClickOnPciWorkList();
                _sideBarPanelSearch.ClickOnClearLink();
                _sideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group", reviewGroups.Split(',').ToList());
                _sideBarPanelSearch.ClickOnButtonByButtonName("Create");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,
                        How.CssSelector));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void CreateFFPWorklistWithReviewGroups(string reviewGroups)
        {
           
                ClickOnFFPWorkList();
                _sideBarPanelSearch.ClickOnClearLink();
                _sideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group", reviewGroups.Split(',').ToList());
                _sideBarPanelSearch.ClickOnButtonByButtonName("Create");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                
        }
        public bool IsListInAscendingOrder<T>(List<T> list)
        {
            return list.IsInAscendingOrder();
        }
        
        
        public List<string> GetFlaggedClaimList(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfFlaggedClaims, client,
                    EnvironmentManager.Username));
        }

        public List<string> GetFlaggedClaimListClient(string client)
        {
            return
                Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.CountOfFlaggedClaimsClient, client,
                    EnvironmentManager.Username));
        }

        public List<List<string>> GetExcelDataList(string username)
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(String.Format(ClaimSqlScriptObjects.GetExcelvalueForFlaggedClaims, username));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;
        }

        public List<string> GetClaimExportAuditListFromDB(string username)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetExcelDownloadAudit, username));
        }


        public List<List<string>> GetExcelDataListForClient(string username)
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(String.Format(ClaimSqlScriptObjects.GetExcelValueFOrFlaggedClaimsClient, username));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;
        }

        public string GetClaimSearchResultsDetails(int row)
        {
            return SiteDriver
                .FindElement(Format(ClaimSearchPageObjects.ClaimSearchResultListCssTemplate, row),
                    How.CssSelector).Text;
        }

        public bool IsClaimLockPresentForClaimSequence(string claseq)
        {
            return SiteDriver.IsElementPresent(Format(ClaimSearchPageObjects.ClaimSearchResultListLockForClaimXpathTemplate, claseq), How.XPath);
        }

        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }

        public void ClickOnSidebarIcon()
        {
             SiteDriver.FindElement(ClaimSearchPageObjects.SideBarIconCssLocator, How.CssSelector).Click();
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(ClaimSearchPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
        }
        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(ClaimSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;


        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(
                ClaimSearchPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
             WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public List<String> GetSearchResultListByCol(int col)
        {

            var list = JavaScriptExecutor.FindElements(
                  Format(ClaimSearchPageObjects.ClaimSearchResultListCssTemplate, col), How.CssSelector, "Text");
            list.RemoveAll(IsNullOrEmpty);
            return list;
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInAscendingOrder();
            
        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public bool IsListClaimSeqSortedInDescendingOrder(int col)
        {
            return GetClaimSequenceListInSearchResult(col).ToList().IsInDescendingOrder();
        }

        public bool IsListClaimSeqSortedInAscendingOrder(int col)
        {
            return GetClaimSequenceListInSearchResult(col).ToList().IsInAscendingOrder();
        }

        public bool IsProvSeqinAscendingOrder()
        {
            return JavaScriptExecutor
                .FindElements(ClaimSearchPageObjects.ProvSeqListInClaimSearchPageCssSelector, How.CssSelector,"Text")
                .IsInAscendingOrder();
        }

        public bool IsProvSeqinDescendingOrder()
        {
            return JavaScriptExecutor
                .FindElements(ClaimSearchPageObjects.ProvSeqListInClaimSearchPageCssSelector, How.CssSelector, "Text")
                .IsInDescendingOrder();
        }

        public List<long> GetClaimSequenceListInSearchResult(int col)
        {

            var list = JavaScriptExecutor.FindElements(
                  Format(ClaimSearchPageObjects.ClaimSearchResultListCssTemplate, col), How.CssSelector, "Text").Select(s => s.Split('-')[0]).ToList().Select(Int64.Parse).ToList();           
            return list;
        }
        public List<double> GetSavingsListinSearchResult(int col)
        {
            var list = JavaScriptExecutor.FindElements(
                     Format(ClaimSearchPageObjects.ClaimSearchResultListFollowingProvSeqXPath, col), How.XPath, "Text").Select(s => double.Parse(s.Split('$')[1])).ToList();
            return list;

        }

        public bool IsListInAscendingOrderBySavings(int col=1)
        {
            return GetSavingsListinSearchResult(col).IsInAscendingOrder();
        }

        public bool IsListInADecendingOrderBySavings(int col=1)
        {
            return GetSavingsListinSearchResult(col).IsInDescendingOrder();
        }

        public bool IsListDateSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder();
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder();
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

        public bool IsClaimSearchListInReferenceToProvSeqInAcseningOrder(int col=1, bool preceding = true)
        {
            if (!preceding)
            {
                return JavaScriptExecutor
                    .FindElements(Format(ClaimSearchPageObjects.ClaimSearchResultListFollowingProvSeqXPath, col),
                        How.XPath, "Text").IsInAscendingOrder();
            }

            return JavaScriptExecutor
                .FindElements(Format(ClaimSearchPageObjects.ClaimSearchResultListPrecedingProvSeqXPath, col),
                    How.XPath, "Text").IsInAscendingOrder();
        }
        public List<string> IsClaimSearchResultListForMemIdInAcseningOrder()
        {
            return JavaScriptExecutor
                .FindElements(ClaimSearchPageObjects.ClaimSearchResultListForMemIdXPath, How.XPath, "Text");
        }

        public bool IsClaimSearchListInReferenceToProvSeqInDescendingOrder(string col = "1", bool preceding = true)
        {
            if (!preceding)
            {
                return JavaScriptExecutor
                    .FindElements(Format(ClaimSearchPageObjects.ClaimSearchResultListFollowingProvSeqXPath, col),
                        How.XPath, "Text").IsInDescendingOrder();

            }

            return JavaScriptExecutor
                .FindElements(Format(ClaimSearchPageObjects.ClaimSearchResultListPrecedingProvSeqXPath, col),
                    How.XPath, "Text").IsInDescendingOrder();

        }

        public List<string> GetClaimSequenceByClaimNoInDescendingOrder(string claimNo)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimSequenceByClaimNoInDescendingOrder, claimNo));
            return list;
        }

        public List<string> GetClaimSequenceByClaimNoInAscendingOrder(string claimNo)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.ClaimSequenceByClaimNoInAscendingOrder, claimNo));
            return list;
        }

        public string GetClaimSequenceInDetailSection()
        {
            return SiteDriver.FindElement(ClaimSearchPageObjects.ClaimSeqInDetailSectionXpath, How.XPath).Text;
        }

        public ClaimActionPage ClickOnPreviouslyViewedClaimSequenceLinkByRow(int row)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(ClaimSearchPageObjects.PreviouslyViewedClaimsSequenceLinkXPath, row), How.XPath).Click();
                 WriteLine("Clicked on Previous claim Sequence of row {0}", row);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnPreviouslyViewedClaimSequenceLinkByClaseq(string claseq)
        {
            var newClaimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(ClaimSearchPageObjects.PreviouslyViewedClaimsSequenceLinkByClaseqXPath, claseq), How.XPath).Click();
                WriteLine("Clicked on Previous claim Sequence {0}", claseq);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
            });
            return new ClaimActionPage(Navigator, newClaimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public List<string> GetPreviouslyViewedClaimList()
        {
            return JavaScriptExecutor.FindElements(ClaimSearchPageObjects.PreviouslyViewedClaimsSequenceListXPath, How.XPath, "Text");
        }

        public List<string> GetPendedClaimForReviewGroupClient()
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetPenedClaimsFromReviewGroup,
                EnvironmentManager.ClientUserName));
        }

        public List<string> GetUnreviewedClaimForReviewGroupClient(string procCode)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetUnreviewedClaimsFromReviewGroup,
                EnvironmentManager.ClientUserName,procCode));
        }

        public List<string> GetUnreviewedClaimForReviewGroupInternaList(string procCode)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetUnreviewedClaimsFromReviewGroupForInternal,
                EnvironmentManager.ClientUserName,procCode));
        }
        public List<string> GetFlaggedClaimForReviewGroupClient(string procCode)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetFlaggedClaimForReviewGroup,
                EnvironmentManager.ClientUserName,procCode));
        }

        public List<string> GetFlaggedClaimForReviewGroupInternaList(string procCode)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetFlaggedClaimForReviewGroupForInternal,
                EnvironmentManager.ClientUserName,procCode));
        }

        public bool IsExportIconPresent()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.ClaimsearchExportByCss, How.CssSelector);
        }

     

        public void ClickOnExportIcon()
        {
            JavaScriptExecutor.FindElement(ClaimSearchPageObjects.ClaimsearchExportByCss).Click();
             Write("Clicked on Export option");
            WaitForStaticTime(100);
        }

        public bool IsExportIconDisabled()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.DisabledExportIconCss, How.CssSelector);
        }

        public bool IsExportIconEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimSearchPageObjects.EnabledExportIconCss, How.CssSelector);
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<ClaimSearchPage>();
            return fileName;

        }

        public bool IsFlagLabelPresent()
        {
            return _sideBarPanelSearch.GetSearchFiltersList().Contains("Flag")&&
                _sideBarPanelSearch.GetSearchFiltersList()[_sideBarPanelSearch.GetSearchFiltersList().IndexOf("Flag")+1].Contains("Batch");
        }

        public bool IsFlagDropDownListAscending()
        {
            ScrollToLastLiOfDropDownOption();
            return _sideBarPanelSearch.GetAvailableDropDownList("Flag").Skip(2).ToList().IsInAscendingOrder();
            //return _sideBarPanelSearch.GetAvailableDropDownList("Flag").Skip(1).ToList().IsInAscendingOrder();
        }

        public void ScrollToLastLiOfDropDownOption()
        {
            JavaScriptExecutor.ExecuteToScrollToLastLi(ClaimSearchPageObjects.LastLiOfDropDownListXPathByLabel);
        }

        public void ScrollToLastLiOfResultSet()
        {
            JavaScriptExecutor.ExecuteToScrollToLastLi(ClaimSearchPageObjects.LastLiOfResultSetCssSelector);
        }

        public int GetResultSetCount()
        {
            ScrollToLastLiOfResultSet();
            return JavaScriptExecutor.FindElementsCount(ClaimSearchPageObjects.LastLiOfResultSetCssSelector);
        }
        public bool IsClientProcessingTypeRealtime(string clientcode)
        {
            if (Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetClientProcessingType, clientcode)) ==
                "R")
                return true;
            return false;
        }

        public void DeleteClaimAuditOnly(string clientcode, string claSeq, string date)
        {
            Executor.ExecuteQueryAsync(Format(ClaimSqlScriptObjects.DeleteClaimAuditOnly,
                claSeq.Split('-')[0], claSeq.Split('-')[1], date));
            WriteLine("Delete Audit from database if exists for claseq<{0}> date greater than <{1}> ", claSeq,
                date);
        }

        public bool IsPopupOrTab()
        {
            var i = JavaScriptExecutor.IsWindowOpenedAsTab();
            return i;
        }

        public string GetPatientRestrictionDescriptionByPatNum(string patNum)
        {
            return Executor.GetSingleStringValue(Format(ClaimSqlScriptObjects.GetPatientRestrictionDescriptionByPatNum,
                patNum));
        }

        public List<string> GetPendedClaimsFromSpecificProcCodeAndTIN(string procCode, string tin)
        {
            return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetPendedClaimsFromProcCodeandTin,
                EnvironmentManager.ClientUserName, procCode,tin));
        }

        public ClaimActionPage SearchByTINAndProCodeToNavigateToNewClaimActionPage(string tin, string proccode, bool handlePopup = false)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                GetSideBarPanelSearch.SetInputFieldByLabel("TIN",tin);
                GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", proccode);
                GetSideBarPanelSearch.ClickOnFindButton();
                WaitForWorking();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue()));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);

        }

        public void DeleteLogicNoteFromDatabase(string claimSeq)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimLogicInFlagOfLogicManager,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));

        }

        public void UpdateClaimStatus(string claSeq, string client, string status, char reltoclient = 'F')

        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateStatusByClaimSequence,
                claSeq.Split('-')[0], claSeq.Split('-')[1], client, status, reltoclient));
            Console.WriteLine("Update ClaimQAAudit from database if exists for claseq<{0}>", claSeq);

        }

        public void UpdateStatusByClaimSequence(string claimSequence, string client, char status)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.UpdateStatusToUnreviewedByClaimSequence, claimSequence.Split('-')[0], claimSequence.Split('-')[1], client, status));
            WriteLine("Update Claim Status from database for claim sequence", claimSequence);
        }

        public List<string> GetActiveFlagsForProductFromDatabase(string product)
        {
            var flaglist = new List<string>();
            flaglist = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.FlagList, product));
            if(flaglist == null)
            {
                return new List<string>();
            }
            return flaglist;
        }

        public List<string> GetAllFlagsFromDatabase()
        {
            return Executor.GetTableSingleColumn(ClaimSqlScriptObjects.AllFlagList);
        }

        public bool IsRestrictionIconPresent(int linNo = 1)
        {
            return SiteDriver.IsElementPresent(
                Format(ClaimSearchPageObjects.ClaimViewRestrictionIconCssSelectorByLineNum, linNo), How.CssSelector);
        }

        public string GetRestrictionIconToolTip(int linNo = 1)
        {
            return SiteDriver.FindElement(
                    Format(ClaimSearchPageObjects.ClaimViewRestrictionIconCssSelectorByLineNum, linNo),
                    How.CssSelector)
                .GetAttribute("title");
        }

        public List<string> GetUnreviewedClaimsByBatchDate(string date, string userId)
        {
             return Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.UnreviewedClaimByBatchDate, date, userId));
        }

        #endregion
    }

    }
