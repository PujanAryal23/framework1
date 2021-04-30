using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.SqlScriptObjects.Appeal;
using UIAutomation.Framework.Database;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Environment;
using static System.Console;
using static System.String;
using Keys = OpenQA.Selenium.Keys;
using System.Collections;
using Nucleus.Service.SqlScriptObjects.Settings;

namespace Nucleus.Service.PageServices.Appeal
{
   public class AppealSearchPage:NewDefaultPage
    {
        #region PRIVATE PROPERTIES

        private AppealSearchPageObjects _appealSearchPage;
        private CalenderPage _calenderPage;
        private readonly string _originalWindow;
        private AppealActionPage _appealAction;
        private QuickLaunchPage _quickLaunch;
        private AppealManagerPage _appealManager;
        private AppealSummaryPage _appealSummary;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly CommonSQLObjects _commonSqlObjects;

        #endregion

        #region CONSTRUCTOR

        public AppealSearchPage(INewNavigator navigator, AppealSearchPageObjects newAppealSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, newAppealSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealSearchPage = newAppealSearchPage;//(AppealSearchPageObjects)PageObject;
            _calenderPage =new CalenderPage(SiteDriver);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _commonSqlObjects = new CommonSQLObjects(Executor);
        }

        #endregion

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

       

        public SideBarPanelSearch GetSideBarPanelSearch
       {
           get { return _sideBarPanelSearch; }

       }
       public GridViewSection GetGridViewSection
       {
           get { return _gridViewSection; }
       }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        #region database interaction
        public int DoesClaimRequireStateReview(string claseq, string clasub = "0")
        {
            string query =  Format(AppealSqlScriptObjects.DoesClaimRequireStateReview, claseq, clasub);
            return Convert.ToInt32(Executor.GetSingleValue(query));
        }

        public void UpdateAppealConsultantAndStatusInAppeal(string appealseq)
        {

            Executor.ExecuteQuery( Format(AppealSqlScriptObjects.UpdateAppealConsultantAndStatus,
                appealseq));
             WriteLine("Update consultant and status for appeal sequence <{0}>", appealseq);
        }

        public void UpdateAppealStatusToNew(string claimSeq)
        {
            var claSeq = claimSeq.Split('-').ToList();
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.UpdateAppealStatusByAppealSeq, claSeq[0], claSeq[1]));
        }

        public void UpdateAppealType(string type,string appealseq)
        {
            
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.UpdateAppealType, type,appealseq ));
            WriteLine($"Updated Appeal Type To {type}");
        }




        public void DeleteAppealAuditByClaseq(string claseq, string date)
        {
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.DeleteAppealAuditByClaseq, claseq.Split('-')[0],
                claseq.Split('-')[1], date));
        }

        public string GetAppealProductByClaSeqFromDb(string claseq)
        {
            return Executor.GetSingleStringValue(Format(AppealSqlScriptObjects.GetAppealProductFromClaimSequence, claseq.Split('-')[0], claseq.Split('-')[1]));
        }

        public void UpdateAppealStatusToIncomplete(string appealSeq)
        {
            //var appealSequence = appealSeq;
            Executor.ExecuteQuery( Format(AppealSqlScriptObjects.UpdateAppealStatusToIncomplete, appealSeq));
        }

        public void UpdateAppealStatusToComplete(string appealSeq)
        {
            
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.UpdateAppealStatusToComplete, appealSeq));
        }
        public void ResetAppealToDocumentWaiting(string claimSeq)
       {
           var claSeq = claimSeq.Split('-').ToList();
           Executor.ExecuteQuery( Format(AppealSqlScriptObjects.ResetAppealToDocumentWaiting, claSeq[0], claSeq[1]));
       }

        public void DeleteAppealLockByAppealSeq(string appealSeq)
        {
            Executor.ExecuteQuery( Format(AppealSqlScriptObjects.DeleteAppealLockByAppealSeq, appealSeq));
        }

        public void DeleteAppealLockByClaimSeq(string claimSeq)
        {
            Executor.ExecuteQuery( Format(AppealSqlScriptObjects.DeleteAppealLockByClaimSeq,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));
        }

        public void DeleteClaimLock(string claimSeq)
        {
            Executor.ExecuteQuery(Format(AppealSqlScriptObjects.DeleteClaimLock,
                claimSeq.Split('-')[0], claimSeq.Split('-')[1]));
        }

        //Don't hard delete appeals!
        /*public void DeleteAppealsOnClaim(string claseq)
        {
            var temp =  Format(AppealSqlScriptObjects.DeleteAppealsOnAClaim, claseq);
            Executor.ExecuteQuery(temp);
        }*/
        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn( Format(AppealSqlScriptObjects.GetAssignedClientList, userId));
        }
        public List<string> GetAssociatedPlansForClient(string client)
        {
            var planList =
                Executor.GetTableSingleColumn( Format(AppealSqlScriptObjects.ClientWisePlanList, client));
             WriteLine("The count of plans associated with client: {0} is {1}", client, planList[0]);
            return planList;
        }

        public List<String> GetPrimaryReviewerAssignedToList()
        {
            var userList =
                Executor.GetTableSingleColumn( Format(AppealSqlScriptObjects.UserListHavingAppealCanBeAssignedAuthority));
            userList = userList.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList();
            userList.Sort();
             WriteLine("The list of users with 'Appeals can be assigned to user' authority ");
            return userList;
        }

        public List<List<string>> GetTopFlagAndProductForClaimSequence(string claseq)
        {
            var temp =  Format(AppealSqlScriptObjects.GetTopFlagAndProductForClaimSeq, claseq);
            var list =
            Executor.GetCompleteTable(temp);
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }
        #endregion

        #region PUBLIC METHODS

        public List<string> GetAppealLevelBadgeValue()
        {
            return JavaScriptExecutor.FindElements(AppealSearchPageObjects.AppealLevelBadgeCssTemplate,
                How.CssSelector, "Text");
        }

        public void SelectDropDownListbyInputLabel(string label, string value, bool directSelect =true)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel(label, value, directSelect);
        }
        public AppealActionPage SwitchToNavigateAppealActionViaUrl(string url)
        { 
            var newAppealAction =
                Navigator.Navigate<AppealActionPageObjects>(
                    () =>
                        SiteDriver.Open(BrowserOptions.ApplicationUrl+url));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            SiteDriver.WaitForPageToLoad();
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealSummaryPage SwitchToNavigateAppealSummaryViaUrl(string url)
        {
            var newAppealAction =
                Navigator.Navigate<AppealSummaryPageObjects>(
                    () =>
                        SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_summary"));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector));
            SiteDriver.WaitForPageToLoad();
            return new AppealSummaryPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


       public void SendEnterKeysOnClientTextField()
        {
            SiteDriver.FindElement(AppealSearchPageObjects.ClientSelectCssLocator, How.CssSelector).SendKeys(Keys.Enter);
        }

       public int GetAppealSearchResultRowCount()
       {
           return SiteDriver.FindElementsCount(AppealSearchPageObjects.AppealSearchResultCssLocator, How.CssSelector);
       }

        public List<bool> GetUrgentList()
        {
            ClickOnFilterOption();
            var list = SiteDriver.FindElementsAndGetAttributeByClass("field_error",
                AppealSearchPageObjects.UrgentListCssLocator, How.CssSelector);
            ClickOnFilterOption();
            return list;

        }

        public void ClickOnFilterOption()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.FilterOptionsIconCssLocator,How.CssSelector);
       }

       public List<string> GetFilterOptionList()
       {
           ClickOnFilterOption();
           var list = JavaScriptExecutor.FindElements(AppealSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
           ClickOnFilterOption();
           return list;


       }

       public void ClickOnFilterOptionListRow(int row)
       {
           ClickOnFilterOption();
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.FilterOptionsListCssLocator + ":nth-of-type(" + row + ")", How.CssSelector);
            WriteLine("Click on {0} filter option",row);
           SiteDriver.WaitForPageToLoad();
           ClickOnFilterOption();
       }

       public List<String> GetSearchResultListByCol(int col)
       {
           
             var list= JavaScriptExecutor.FindElements(
                    Format(AppealSearchPageObjects.AppealSearchResultListCssTemplate,  col), How.CssSelector,"Text");
           list.RemoveAll( IsNullOrEmpty);
           return list;
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

        public bool IsListIntSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(int.Parse).ToList().IsInAscendingOrder();
        }

        public bool IsListIntSortedInDescendingOrder(int col)
        {
            var list = GetSearchResultListByCol(col).ToList().Select(int.Parse).ToList();
            return list.IsInDescendingOrder();
        }
        public bool IsBoldRedColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.BoldRedColorDueDateCssTemplate,
                How.CssSelector,200);
        }

        public bool IsNonBoldOnlyRedColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.NonBoldOnlyRedColorDueDateCssTemplate,
                How.CssSelector, 200);
        }

        public bool IsOrangeColorDueDatePresent()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.OrangeColorDueDateCssTemplate,
                How.CssSelector, 200);
        }

        public bool IsBlackColorDueDatePresent(int row)
        {
            return SiteDriver.IsElementPresent( Format(AppealSearchPageObjects.BlackColorDueDateXPathTemplate, row),
                How.XPath, 200);
        }

        /// <summary>
        /// click on appeal sequence to navigate appeal action page
        /// </summary>
        /// <returns>AppealActionPage</returns>
        public AppealActionPage ClickOnAppealSequenceToNavigateAppealActionPageByRow(int row)
        {
            var appealSummaryPage = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                var element =
                    SiteDriver.FindElement(Format(AppealSearchPageObjects.AppealSequenceValueCssTemplate, row),
                        How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WriteLine("Clicked on Appeal Sequence of row {0}", row);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
                SiteDriver.WaitForCondition(()=>SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator,How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealActionPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// click on appeal sequence navigate to appeal action page
        /// </summary>
        /// <param name="appealSeq"></param>
        /// <returns></returns>
        public AppealActionPage ClickOnAppealSequenceToNavigateAppealActionPageByAppealSequence(string appealSeq)
        {
            var appealSummaryPage = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealSequenceByStatusOrAppealXPathTemplate, appealSeq), How.XPath).Click();
                 WriteLine("Clicked on Appeal Sequence<{0}>", appealSeq);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealActionPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealLetterPage ClickOnAppealLetter(int row)
        {
            var appealLetter = Navigator.Navigate<AppealLetterPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(Format(AppealSearchPageObjects.AppealLetterIconCssTemplate, row),
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WriteLine("Clicked on Appeal Letter");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            });
            return new AppealLetterPage(Navigator, appealLetter, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public AppealLetterPage ClickOnApealLetterIconByAppealLevel(int appealLevel)
        {
            var appealLetter = Navigator.Navigate<AppealLetterPageObjects>(() =>
            {
                if (appealLevel > 1)
                {
                    SiteDriver.FindElement(
                         Format(AppealSearchPageObjects.ApealLetterIconByAppealLevelXPathTemplate,
                            appealLevel),
                        How.XPath).Click();
                     WriteLine( Format("Clicked on Appeal Letter having appeal level<{0}>", appealLevel));
                }
                else
                {
                    SiteDriver.FindElement(AppealSearchPageObjects.ApealLetterIconForEmptyAppealLevelXPathTemplate,
                        How.XPath).Click();
                     WriteLine("Clicked on Appeal Letter For Empty appeal level");
                }

                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            });
            return new AppealLetterPage(Navigator, appealLetter, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public bool IsAppealLetterIconPrsentByRow(int row)
       {
           return SiteDriver.IsElementPresent(
                Format(AppealSearchPageObjects.AppealLetterIconCssTemplate, row), How.CssSelector);
       }

        public string GetAdjustValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealSearchPageObjects.AdjustCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetPayValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealSearchPageObjects.PayCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetDenyValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealSearchPageObjects.DenyCssTemplate, row), How.CssSelector)
                    .Text;
        }

        public string GetNoDocsValueByRow(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealSearchPageObjects.NoDocsCssTemplate, row), How.CssSelector)
                    .Text;
        }

        

        public void OpenAppealManagerInNewTab(string appealSeq)
       {
           JavaScriptExecutor.ExecuteScript("window.open()");
           var handles = SiteDriver.WindowHandles.ToList();
           var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() =>
           {
               
               SiteDriver.SwitchWindow(handles[1]);
               SiteDriver.Open(SiteDriver.BaseUrl+PageUrlEnum.QuickLaunch.GetStringValue());
               //SiteDriver.SwitchForwardTab();
               SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
           });
           _quickLaunch = new QuickLaunchPage(Navigator, quickLaunch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            _appealManager= _quickLaunch.NavigateToAppealManager();
            _appealManager.SearchByAppealSequence(appealSeq);
            
            SiteDriver.SwitchForwardTab();
            SiteDriver.SwitchWindow(handles[0]);
       }

        public string IsAppealLockIconPresentAndGetLockIconToolTipMessage(string appealSeq, string pageUrl)
       {
           
           JavaScriptExecutor.ExecuteScript("window.open()");
           var handles = SiteDriver.WindowHandles.ToList();
           var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
           {
               
               SiteDriver.SwitchWindow(handles[1]);
               SiteDriver.Open(pageUrl);
               
               //SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
               SiteDriver.WaitForCondition(()=>SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator,How.CssSelector));
           });



           _appealAction = new AppealActionPage (Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
           SiteDriver.SwitchForwardTab();
           SiteDriver.SwitchWindow(handles[0]);
           ClickOnFindButton();
           WaitForWorkingAjaxMessage();
           SiteDriver.WaitForCondition(()=>IsAppealLockIconPresent(appealSeq),3000);
           var isAppealLockIconPresent = IsAppealLockIconPresent(appealSeq);
           var appealLockToolTipMessage = "";
           if (isAppealLockIconPresent)
               appealLockToolTipMessage =
                   SiteDriver.FindElement(
                        Format(AppealSearchPageObjects.LockIconByAppealSequenceXPathTemplate, appealSeq),
                       How.XPath).GetAttribute("title");
           

           return appealLockToolTipMessage;
       }

       

       public void SwitchToNewTabAndChangeStatus(string status)
       {
           SiteDriver.SwitchForwardTab();
           SiteDriver.SwitchWindowByUrl("appeal_manager");
           _appealManager.ClickOnEditIcon();
           _appealManager.SelectDropDownListbyInputLabel("Status",status);
           _appealManager.ClickOnSaveButton();
           _appealManager.WaitForWorking();
          
           SiteDriver.SwitchBackwardTab();
           SiteDriver.SwitchWindowByUrl("appeals");
           ClickOnFindButton();
           WaitForWorkingAjaxMessage();
           SiteDriver.WaitToLoadNew(300);
       }

       public void SwitchToNewTabAndChangeDueDate(string date)
       {
           SiteDriver.SwitchForwardTab();
           SiteDriver.SwitchWindowByUrl("appeal_manager");
           _appealManager.ClickOnEditIcon();
           _appealManager.SetDueDateInEditAppealForm(date);
           _appealManager.ClickOnSaveButton();
           _appealManager.WaitForWorking();
           //_appealAction.SetDueDateAndSave(date);
           SiteDriver.SwitchBackwardTab();
           SiteDriver.SwitchWindowByUrl("appeals");
           ClickOnFindButton();
           WaitForWorkingAjaxMessage();
           
       }
      
       public void CloseAppealActionTabIfExists()
       {
           if (SiteDriver.WindowHandles.Count != 2) return;
           if (GetPageHeader()=="Appeal Search")
           {
               SiteDriver.SwitchForwardTab();
               SiteDriver.SwitchWindowByUrl("appeal_action");
               _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
               ClickOnQuickLaunch();
           }
           else
           {
               if (GetPageHeader() == "Appeal Action")
               {
                   _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                   _appealAction.ClickOnQuickLaunch();
               }
                  
           }
           SiteDriver.CloseWindow();
           SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSearch.GetStringValue());
       }

       public void CloseAppealManagerTabIfExists()
       {
           if (SiteDriver.WindowHandles.Count != 2) return;
           if (GetPageHeader() == "Appeal Search")
           {
               SiteDriver.SwitchForwardTab();
               SiteDriver.SwitchWindowByUrl("Appeal Manager");
               ClickOnQuickLaunch();
           }
           ClickOnQuickLaunch();
           SiteDriver.CloseWindow();
           SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSearch.GetStringValue());
       }
       public void CloseAnyTabOfAppealSummaryIfExists()
       {
           if (SiteDriver.WindowHandles.Count != 2) return;
           if (GetPageHeader() == "Appeal Search")
           {
               SiteDriver.SwitchForwardTab();
               SiteDriver.SwitchWindowByUrl("appeal_summary");
               _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
               
           }
           else
           {
               if (GetPageHeader() == "Appeal Summary")
               {
                   _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                   
               }

           }
           SiteDriver.CloseWindow();
           SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSearch.GetStringValue());
       }

       public bool IsAppealLockIconPresent(string appealSeq)
       {
           return SiteDriver.IsElementPresent(
                   Format(AppealSearchPageObjects.LockIconByAppealSequenceXPathTemplate, appealSeq), How.XPath);
       }

       public bool IsUrgentIconPresent(int row)
       {
           return SiteDriver.IsElementPresent( Format(AppealSearchPageObjects.UrgerIconCssTemplate, row),
               How.CssSelector);
       }

       public bool IsUrgentIconPresentByAppealSeq(string appealSeq)
       {
           return JavaScriptExecutor.IsElementPresent(Format(AppealSearchPageObjects.UrgentIconByAppealSeqCssTemplate, appealSeq));
       }

        public string GetSearchResultByRowCol(int row, int col)
       {
           return
               SiteDriver.FindElement(
                    Format(AppealSearchPageObjects.AppealSearchResultCssTemplate, row, col), How.CssSelector)
                   .Text;
       }
       public void ScrollToRow(int row = 1)
       {
           JavaScriptExecutor.ExecuteToScrollToSpecificDiv( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row));

       }

        public void SetDateFieldFrom(int row,string date,string dateName)
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row), How.CssSelector);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
             WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }
        public void SetDateField(int row,int col, string date, string dateName)
        {
            SiteDriver.FindElement(
                  Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(" + col + ")", row), How.CssSelector).Click();
            SiteDriver.FindElement(
                Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(" + col + ")", row),
                How.CssSelector).ClearElementField();
            SiteDriver.FindElement(
                Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(" + col + ")", row), How.CssSelector).SendKeys(date);
            JavaScriptExecutor.ExecuteMouseOut( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(" + col + ")", row), How.CssSelector);
             WriteLine("<{0}> Input value:<{1}>", dateName, date);
        }

       public string GetDateFieldFrom(int row)
       {
           return
               SiteDriver.FindElement(
                    Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(1)", row),
                   How.CssSelector).GetAttribute("value");
       }
       public string GetDateFieldPlaceholder(int row,int  col)
       {
           return
               SiteDriver.FindElement(
                    Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type("+col+")", row),
                   How.CssSelector).GetAttribute("placeholder");
       }

        public void SetDateFieldTo(int row, string date, string dateName)
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate+":nth-of-type(2)", row), How.CssSelector);
            _calenderPage.SetDate(date);
             WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }

        public string GetDateFieldTo(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealSearchPageObjects.SearchInputFieldCssTemplate + ":nth-of-type(2)", row),
                    How.CssSelector).GetAttribute("value");
        }

       public string GetNoMatchingRecordFoundMessage()
       {
           return SiteDriver.FindElement(AppealSearchPageObjects.NoMatchingRecordFoundCssSelector,
               How.CssSelector).Text;
       }

       public bool IsNoMatchingRecordFoundMessagePresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.NoMatchingRecordFoundCssSelector,How.CssSelector);
       }

     

        public void ClickOnAdvancedSearchFilterIcon(bool click)
       {
           var extedFieldDispaly = IsSearchInputFieldDispalyed(17);
           if (!extedFieldDispaly && click)
           {
               JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.AdvancedSearchIconCssLocator, How.CssSelector);
               SiteDriver.WaitForCondition(() => IsSearchInputFieldDispalyed(17));
           }
           else if (extedFieldDispaly && !click)
           {
               JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.AdvancedSearchIconCssLocator, How.CssSelector);
               SiteDriver.WaitForCondition(() => !IsSearchInputFieldDispalyed(17));
           }
       }

       public void ClickOnAdvancedSearchFilterIconForMyAppeal()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.AdvancedSearchIconCssLocator, How.CssSelector);
       }

       public bool IsSearchInputFieldDispalyed(int row)
       {
           return SiteDriver.IsElementPresent(
                Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row), How.CssSelector);
       }
       public bool IsSearchInputLabelDispalyed( string value)
       {
           return SiteDriver.IsElementPresent( Format(AppealSearchPageObjects.SearchInputLabelXpathTemplate,  value), How.XPath);
       }

       public bool IsSearchInputLabelForDropDownFieldDispalyed(string value)
       {
           return SiteDriver.IsElementPresent( Format(AppealSearchPageObjects.DropDownSearchInputLabelXpathTemplate, value), How.XPath);
       }
        public void SetSearchInputField(string fieldName,string value,int row)
        {
            var element = SiteDriver.FindElement(
                Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row), How.CssSelector);
           element.ClearElementField();
           SiteDriver.WaitTillClear(element);
           SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            WriteLine("<{0}: <{1}>",fieldName, value);
        }

        public string GetSearchInputLabel(int row)
        {
            return SiteDriver.FindElement(
                  Format(AppealSearchPageObjects.SearchLabelCssTemplate, row), How.CssSelector)
                 .Text;
        }
        public string GetAlternateClaimNoLabel(int row)
        {
            return SiteDriver.FindElement(
                  Format(AppealSearchPageObjects.SearchLabelCssTemplate, row), How.CssSelector).Text;
        }


       public string GetSearchInputField(int row)
        {
           return  SiteDriver.FindElement(
                 Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row), How.CssSelector)
                .GetAttribute("value");
        }
       public string GetSearchInputFieldPlaceholder(int row)
       {
           return SiteDriver.FindElement(
                 Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row), How.CssSelector)
                .GetAttribute("placeholder");
       }
       public bool IsSearchInputFieldDisabled(int row)
       {
           return SiteDriver.IsElementPresent(
                 Format(AppealSearchPageObjects.DisabledSearchInputFieldCssTemplate, row), How.CssSelector);
       }

        public void  SelectSearchDropDownListValue(string fieldName,string value,int row)
        {
            if(row>10)
                JavaScriptExecutor.ExecuteToScrollToSpecificDiv( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row));
            var element = SiteDriver.FindElement(Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row),
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            if (!SiteDriver.IsElementPresent( Format(AppealSearchPageObjects.SearchInputListValueXPathTemplate, row, value), How.XPath))
                SiteDriver.FindElement( Format(AppealSearchPageObjects.SearchInputFieldCssTemplate,row), How.CssSelector).Click();
            JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.SearchInputListValueXPathTemplate,row, value), How.XPath);
             WriteLine("<{0}> <{1}> Selected",value,fieldName);
        }
        public void SelectSearchDropDownListValue(string fieldName, string value)
        {
            SiteDriver.FindElement(Format(AppealSearchPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath)
                .ClearElementField();
            SiteDriver.WaitTillClear(SiteDriver.FindElement(Format(AppealSearchPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath));
            SiteDriver.WaitToLoadNew(200);
            SiteDriver.FindElement(Format(AppealSearchPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath).SendKeys(value);
            if (
                !SiteDriver.IsElementPresent(
                     Format(AppealSearchPageObjects.SearchInputListValueXPathTemplateGeneric, fieldName, value),
                    How.XPath))
                JavaScriptExecutor.ExecuteClick(
                     Format(AppealSearchPageObjects.SearchInputFieldXpathTemplate, fieldName), How.XPath);
            JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.SearchInputListValueXPathTemplateGeneric, fieldName, value), How.XPath);
             WriteLine("<{0}> <{1}> Selected", value, fieldName);
        }

        public void SelectSearchDropDownListForMultipleSelectValue(string fieldName, string value, int row)
        {
            var element = SiteDriver.FindElement(Format(AppealSearchPageObjects.SearchInputFieldCssTemplate, row),
                How.CssSelector);
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.SearchInputListValueXPathTemplate, row, value), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut( Format(AppealSearchPageObjects.SearchInputListValueXPathTemplate, row, value), How.XPath);
             WriteLine("<{0}> <{1}> Selected", value, fieldName);
        }

       public string GetAppealSequenceByStatus(string status="New")
       {
           return
               SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealSequenceByStatusOrAppealXPathTemplate,status), How.XPath).Text;
       }

       public string GetAssignedToByAppealSequence(string appealSequence)
       {
           return
               SiteDriver.FindElement(
                    Format(AppealSearchPageObjects.AssignedToByAppealSequenceXPathTemplate, appealSequence),
                   How.XPath).Text;
       }
       public bool IsAppealSequenceListPresent()
       {
           return
              SiteDriver.IsElementPresent(AppealSearchPageObjects.AppealSequenceListCssSelector, How.CssSelector);
       }
       public List<string> GetAppealSequenceList()
       {
           return
               JavaScriptExecutor.FindElements(AppealSearchPageObjects.AppealSequenceListCssSelector, How.CssSelector,"Text");
       }

       public List<string> GetUnlockAppealSequenceListWithStatusNew()
       {
           return
               JavaScriptExecutor.FindElements(AppealSearchPageObjects.UnlockAppealSequenceListWithStatusNew, "Text");
       }

        /// <summary>
        /// click on appeal sequence to navigate to new appeal action page
        /// </summary>
        /// <returns>NewAppealAction</returns>
       public AppealActionPage ClickOnAppealSequenceByStatusOrAppealToGoAppealAction(string status = "New")
        {
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealSequenceByStatusOrAppealXPathTemplate, status), How.XPath).Click();
                 WriteLine("Clicked on Appeal Sequence having status New");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
                SiteDriver.WaitForPageToLoad();
            });
            return new AppealActionPage(Navigator, newAppealActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

       /// <summary>
       /// click on appeal sequence to navigate appeal summary page
       /// </summary>
       /// <returns>AppealSummaryPage</returns>
       public AppealSummaryPage ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage(string status = "New")
       {
            var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                var element =
                    SiteDriver.FindElement(
                        Format(AppealSearchPageObjects.AppealSequenceByStatusOrAppealXPathTemplate, status), How.XPath);
               SiteDriver.ElementToBeClickable(element);
               SiteDriver.WaitToLoadNew(500);
               element.Click();
               WriteLine("Clicked on Appeal Sequence by Status {0}", status);
               SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
               SiteDriver.WaitForPageToLoad();

           });
            return new AppealSummaryPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
         
       }

       /// <summary>
       /// click on appeal sequence to navigate appeal summary page
       /// </summary>
       /// <returns>AppealSummaryPage</returns>
       public AppealSummaryPage ClickOnReturnToAppealToGoAppealSummaryPage()
       {
           var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               SiteDriver.FindElement(AppealSearchPageObjects.ReturnToAppealCssLocator, How.CssSelector).Click();
                WriteLine("Clicked on Return To Appeal ");
               SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
               SiteDriver.WaitForPageToLoad();

           });
           return new AppealSummaryPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

       }

       /// <summary>
       /// click on appeal sequence to navigate appeal summary page
       /// </summary>
       /// <returns>AppealSummaryPage</returns>
       public AppealActionPage ClickOnReturnToAppealToGoAppealActionPage()
       {
            var newAppealActionPage = Navigator.Navigate<AppealActionPageObjects>(() =>
           {
               SiteDriver.FindElement(AppealSearchPageObjects.ReturnToAppealCssLocator, How.CssSelector).Click();
                WriteLine("Clicked on Return To Appeal ");
               SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
               SiteDriver.WaitForPageToLoad();

           });
           return new AppealActionPage(Navigator, newAppealActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

          

       }
        public void SetAppealSequence(string value)
        {
            SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.WaitTillClear(SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator, How.CssSelector));
            SiteDriver.WaitToLoadNew(200);
            SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator, How.CssSelector).SendKeys(value); 
             WriteLine("Appeal Sequence: <{0}>", value);
        }

        public List<string> GetMedicalRecordReviewAppealsFromDatabase()
        {
            return Executor.GetTableSingleColumn(AppealSqlScriptObjects.GetMedicalRecordReviewAppealsFromDb);
        }

        public void SelectAllAppeals()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());

        }

        public void SelectMyAppeals()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.MyAppeals.GetStringValue());
        }

        public void SelectOutstandingAppeals()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
        }

        public void SelectLast30Days()
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.Last30Days.GetStringValue());
        }
      

       public bool IsCreateDatePresent()
       {
           return _sideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Create Date");
       }

       public string GetQuickSearchFilterValue()
       {
           return
              SiteDriver.FindElement(AppealSearchPageObjects.QuickSearchCssLocator, How.CssSelector).GetAttribute("value");
       }
        public void SelectClientSmtst(bool smtst = true)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                smtst ? ClientEnum.SMTST.ToString() : ClientEnum.CTNSC.ToString());
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


        public string GetClientFilterValue()
        {

            return
               SiteDriver.FindElement(AppealSearchPageObjects.ClientSelectCssLocator, How.CssSelector).GetAttribute("value");
        }

        public void SetClaimSequence(string value)
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.FindElement(AppealSearchPageObjects.ClaimSequenceInputFieldCssLocator,
                    How.CssSelector).Enabled, 5000);
            var element = SiteDriver.FindElement(AppealSearchPageObjects.ClaimSequenceInputFieldCssLocator,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
             WriteLine("Claim Sequence: <{0}>", value);
        }
        public void SetClaimSequenceForCotivitiUser(string value)
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.FindElement(AppealSearchPageObjects.ClaimSequenceInputFieldInCotivitiUserCssLocator,
                    How.CssSelector).Enabled, 5000);
            SiteDriver.FindElement(AppealSearchPageObjects.ClaimSequenceInputFieldInCotivitiUserCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealSearchPageObjects.ClaimSequenceInputFieldInCotivitiUserCssLocator, How.CssSelector).SendKeys(value);
             WriteLine("Claim Sequence: <{0}>", value);
        }

        public void SetAppealSequenceForCotivitiUser(string value)
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator,
                    How.CssSelector).Enabled, 5000);
            SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealSearchPageObjects.AppealSequenceInputFieldCssLocator, How.CssSelector).SendKeys(value);
             WriteLine("Appeal Sequence: <{0}>", value);
        }
        /// <summary>
        /// click on appeal sequence to navigate appeal summary page
        /// </summary>
        /// <returns>AppealSummaryPage</returns>
        public AppealSummaryPage ClickOnAppealSequence(int row)
       {
           var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealSequenceValueCssTemplate, row), How.CssSelector).Click();
                WriteLine("Clicked on Appeal Sequence of row {0}", row);
               SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
               SiteDriver.WaitForPageToLoad();
           });
           return new AppealSummaryPage(Navigator,appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

       public void ClickOnFindButton()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.FindButtonCssLocator,How.CssSelector);
       }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(AppealSearchPageObjects.EnabledFindButtonCssLocator,
                       How.CssSelector);
        }

        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(AppealSearchPageObjects.FindButtonCssLocator,
                                 AppealSearchPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public void ClickOnFindButtonAndWait()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.FindButtonCssLocator, How.CssSelector);
           WaitForWorkingAjaxMessage();
       }
       public void ClickOnSearchButton()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.SearchIconCssLocator, How.CssSelector);
       }

       public void WaitToOpenCloseWorkList(bool condition = true)
       {
           if (condition)
               SiteDriver.WaitForCondition(() => !IsFindAppealSectionPresent());
           else
               SiteDriver.WaitForCondition(IsFindAppealSectionPresent);
       }

       
       /// <summary>
       /// Search by claim sequence
       /// </summary>
       /// <param name="claimSequence"></param>
       /// <returns></returns>
       public AppealSummaryPage SearchByClaimSequence(string claimSequence)
       {
            Out.WriteLine("Searching for Grid Result on Claim Sequence : {0}", claimSequence);
           SetClaimSequence(claimSequence);
           ClickOnFindButton();
           if (IsPageErrorPopupModalPresent())
           {
               ClosePageError();
               ClickOnFindButton();
           }
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
           SiteDriver.WaitForPageToLoad();
           return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }
       /// <summary>
       /// search by appeal sequence 
       /// </summary>
       /// <param name="appealSequence"></param>
       /// <returns></returns>
       public AppealSummaryPage SearchByAppealSequence(string appealSequence)
       {
            Out.WriteLine("Searching for Grid Result on Appeal Sequence : {0}", appealSequence);
           ClearAll();
           SetAppealSequence(appealSequence);
           ClickOnFindButton();
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           if (GetPageHeader() == "Appeal Search")
           {
               ClickOnAppealSequence(1);
           }
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
           SiteDriver.WaitForPageToLoad();
           return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

       public void DeleteAppealDocument(string claseq)
       {
           var temp = string.Format(AppealSqlScriptObjects.DeleteAppealDocument, claseq);
           Executor.ExecuteQuery(temp);
       }


        public AppealSummaryPage SearchByAppealSequenceAndNavigateToAppealSummaryPage(string appealSequence, bool clearAll = true)
       {
           Out.WriteLine("Searching for Grid Result on Appeal Sequence : {0}", appealSequence);
           
           if(clearAll)
            ClearAll();
           
           SetAppealSequence(appealSequence);
           ClickOnFindButton();
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           if (GetPageHeader() == "Appeal Search")
           {
               ClickOnAppealSequence(1);
           }
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForPageToLoad();
           return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

        public AppealSearchPage SearchByAppealSequenceForRestrictedAppeal(string appealSequence)
        {
            SelectAllAppeals();
            SelectDropDownListbyInputLabel("Client", "All");
             Out.WriteLine("Searching for Restricted Appeal Sequence : {0}", appealSequence);
            ClearAll();
            SetAppealSequence(appealSequence);
            ClickOnFindButton();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            if (GetPageHeader() == "Appeal Search" && IsNoMatchingRecordFoundMessagePresent())
            {
                return this;
            }
            return null;

        }

        /// <summary>
        /// search by appeal sequence 
        /// </summary>
        /// <param name="appealSequence"></param>
        /// <returns></returns>
       public AppealActionPage SearchByAppealSequenceNavigateToAppealAction(string appealSequence)
        {
             Out.WriteLine("Searching for Grid Result on Appeal Sequence : {0}", appealSequence);
            SetAppealSequence(appealSequence);
            ClickOnFindButton();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));
            SiteDriver.WaitForPageToLoad();
            return new AppealActionPage(Navigator, new AppealActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealActionPage SearchByAppealSequenceToNavigateToAppealActionForClientSwitch(string appealSeq, bool handlePopup = false)
        {
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SetAppealSequence(appealSeq);
                ClickOnFindButton();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
                string element = "label.page_title";
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(element, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);

        }

        public AppealActionPage SearchByAppealSequenceToNavigateToAppealSummaryForClientSwitch(string appealSeq, bool handlePopup = false)
        {
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SetAppealSequence(appealSeq);
                ClickOnFindButton();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_summary"));
                string element = "label.page_title";
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(element, How.CssSelector));
                SiteDriver.WaitForPageToLoad();

            });
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);

        }
        public AppealSummaryPage SearchByClaimSequenceInCotivitiUser(string claimSequence)
       {
            Out.WriteLine("Searching for Grid Result on Claim Sequence In Cotiviti User: {0}", claimSequence);
           SetClaimSequenceForCotivitiUser(claimSequence);
           ClickOnFindButton();
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
           SiteDriver.WaitForPageToLoad();
           return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

       public AppealActionPage SearchByClaimSequenceToGoAppealAction(string claimSequence,bool handlePopup=false)
       {
            Out.WriteLine("Searching for Grid Result on Claim Sequence In Cotiviti User To Navigate New Appeal Action : {0}", claimSequence);
           SetClaimSequenceForCotivitiUser(claimSequence);
           ClickOnFindButton();
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_action"));
           SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(NewDefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
           SiteDriver.WaitForPageToLoad();
           if(handlePopup)
               CloseAnyPopupIfExist();
           return new AppealActionPage(Navigator, new AppealActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);
       }


       public AppealActionPage FindByClaimSequenceToNavigateAppealAction(string claimSequence, bool handlePopup = true, bool selectclientSMTST = true)
       {
           SelectAllAppeals();
           SelectClientSmtst(selectclientSMTST);
            Out.WriteLine("Searching for Grid Result on Claim Sequence In Verisk User To Navigate New Appeal Action : {0}", claimSequence);
           SetClaimSequenceForCotivitiUser(claimSequence);
           ClickOnFindButton();
           SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
           SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
           SiteDriver.WaitForPageToLoad();
           return new AppealActionPage(Navigator, new AppealActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);
       }

        public AppealActionPage FindByAppealSequenceToNavigateAppealAction(string appealSequence, bool handlePopup = true, bool selectclientSMTST = true)
        {
            //SelectAllAppeals();
            SelectClientSmtst(selectclientSMTST);
             Out.WriteLine("Searching for Grid Result on Claim Sequence In Verisk User To Navigate New Appeal Action : {0}", appealSequence);
            SetAppealSequenceForCotivitiUser(appealSequence);
            ClickOnFindButton();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue()));
            SiteDriver.WaitForPageToLoad();
            return new AppealActionPage(Navigator, new AppealActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, handlePopup);
        }


        public AppealSummaryPage FindByClaimSequenceToNavigateAppealummary(string claimSequence, bool selectclientSMTST = true)
        {
            SelectAllAppeals();
            SelectClientSmtst(selectclientSMTST);
             Out.WriteLine("Searching for Grid Result on Claim Sequence In Verisk User To Navigate Appeal Summary for : {0}", claimSequence);
            SetClaimSequenceForCotivitiUser(claimSequence);
            ClickOnFindButton();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
            SiteDriver.WaitForPageToLoad();
            return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Click Clear All Button
        /// </summary>
        /// <returns></returns>
        public AppealSearchPage ClearAll()
       {
           JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.ClearFilterClass, How.CssSelector);
           //JavaScriptExecutor.ExecuteClick(NewAppealSearchPageObjects.ClearFilterClass, How.CssSelector);
           SiteDriver.WaitToLoadNew(500);
            WriteLine("Clicked Clear Filter.");
           return new AppealSearchPage(Navigator, _appealSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

       public bool IsFindAppealSectionPresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.FindAppealSectionCssLocator, How.CssSelector);
       }
       public bool IsSearchListSectionPresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.SearchListSectionCssLocator, How.CssSelector);
       }

       public bool IsDefaultEmptySearchResultPresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.SearchResultEmptyMessageCssLocator, How.CssSelector);
       }

       public List<string> GetAppealSearchResult()
       {

           return
               JavaScriptExecutor.FindElements(AppealSearchPageObjects.AppealSearchResultCssLocator,
                   How.CssSelector, "Text");
       }

       //1 quick search
       public List<string> GetDropDownList(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.DropDownToggleIconCssTemplate, row), How.CssSelector);
           return JavaScriptExecutor.FindElements( Format(AppealSearchPageObjects.DropDownToggleValueCssTemplate, row), How.CssSelector, "Text");

       }
       public List<string> GetMultiSelectListedDropDownList(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           return JavaScriptExecutor.FindElements( Format(AppealSearchPageObjects.MultiSelectListedDropDownToggleValueCssTemplate, row), How.CssSelector, "Text");

       }
       public List<string> GetMultiSelectAvailableDropDownList(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           //return SiteDriver.FindElements( Format(NewAppealSearchPageObjects.MultiSelectAvailableDropDownToggleValueCssTemplate, row), How.CssSelector, "Text");

           return JavaScriptExecutor.FindElements(Format(AppealSearchPageObjects.MultiSelectAvailableDropDownToggleValueCssTemplate, row), "Text");

       }
       public string GetPreviouslyViewedAppealSeq()
       {

           return
               SiteDriver.FindElement(AppealSearchPageObjects.PreviouslyViewedAppealSequenceCssLocator,
                   How.CssSelector).Text;
       }

       public bool IsReturnToAppealSectionPresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.ReturnToAppealCssLocator, How.CssSelector);
       }

       public AppealSummaryPage ReturnToPreviouslyViewedAppeal()
       {
           var appealSummaryPage = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               SiteDriver.FindElement(AppealSearchPageObjects.PreviouslyViewedAppealLinkCssLocator, How.CssSelector).Click();
                WriteLine("Clicked on Return To Previously Viewed Appeal Sequence");
               SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
               SiteDriver.WaitForPageToLoad();

           });
           return new AppealSummaryPage(Navigator, appealSummaryPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor); 
       }

       public bool IsDropDownSorted(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.DropDownToggleIconCssTemplate, row), How.CssSelector);
           JavaScriptExecutor.ExecuteMouseOver( Format(AppealSearchPageObjects.DropDownToggleIconCssTemplate, row), How.CssSelector);
           var dropDownList = JavaScriptExecutor.FindElements( Format(AppealSearchPageObjects.DropDownToggleValueCssTemplate, row), How.CssSelector, "Text");
           JavaScriptExecutor.ExecuteMouseOut( Format(AppealSearchPageObjects.DropDownToggleIconCssTemplate, row), How.CssSelector);
           return dropDownList.IsInAscendingOrder();
       }
       public bool IsMultiSelectAvailableDropDownSorted(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           JavaScriptExecutor.ExecuteMouseOver( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           var dropDownList = JavaScriptExecutor.FindElements( Format(AppealSearchPageObjects.MultiSelectAvailableDropDownToggleValueCssTemplate, row), How.CssSelector, "Text");
           JavaScriptExecutor.ExecuteMouseOut( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           return dropDownList.IsInAscendingOrder();
       }

       public bool IsMultiSelectListedDropDownSorted(int row)
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           JavaScriptExecutor.ExecuteMouseOver( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           var dropDownList = JavaScriptExecutor.FindElements( Format(AppealSearchPageObjects.MultiSelectListedDropDownToggleValueCssTemplate, row), How.CssSelector, "Text");
           JavaScriptExecutor.ExecuteMouseOut( Format(AppealSearchPageObjects.MultiDropDownToggleIconCssTemplate, row), How.CssSelector);
           return dropDownList.IsInAscendingOrder();
       }

       public void ClickOnSearchListRow(int row)
       {

           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.AppealSearchListRowSelectorTemplate, row), How.CssSelector);
           WaitForWorkingAjaxMessage();
       }

       public bool IsAppealDetailSectionOpen()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.AppealDetailsHeaderXpath, How.XPath);
       }

       public string GetAppealDetailsLabel(int row, int col)
       {
           return SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealDetailContentLabelXpathTemplate, row, col), How.XPath).Text;
       }
       public string GetAppealDetailsContentValue(int row, int col)
       {
           return SiteDriver.FindElement( Format(AppealSearchPageObjects.AppealDetailContentValueXpathTemplate, row, col), How.XPath).Text;
       }


       public string IsAppealLockIconPresentForAppealSummaryViewAndGetLockIconToolTipMessage(string claimSeq,string appealSeq, string pageUrl)
       {
           JavaScriptExecutor.ExecuteScript("window.open()");

           var newAppealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               List<string> handles = SiteDriver.WindowHandles.ToList<string>();
               SiteDriver.SwitchWindow(handles[1]);

               // SiteDriver.WaitForCondition( () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue()));
               SiteDriver.Open(pageUrl);
               SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("appeal_summary"));
               SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector));
               SiteDriver.WaitForPageToLoad();
           });

           
          
           _appealSummary = new AppealSummaryPage(Navigator, newAppealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
           SiteDriver.SwitchBackwardTab();
           SiteDriver.SwitchWindowByUrl("appeals");
           SelectAllAppeals();
           SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
           SetClaimSequence(claimSeq);
           ClickOnFindButton();
           WaitForWorkingAjaxMessage();

           var isAppealLockIconPresent = IsAppealLockIconPresent(appealSeq);
           

           if (!isAppealLockIconPresent)
           {
               SetClaimSequence(claimSeq);
               ClickOnFindButton();
               WaitForWorkingAjaxMessage();
           }
            var   appealLockToolTipMessage =
                   SiteDriver.FindElement(
                        Format(AppealSearchPageObjects.LockIconByAppealSequenceXPathTemplate, appealSeq),
                       How.XPath).GetAttribute("title");
           

           return appealLockToolTipMessage;
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

        public List<string> GetAvailableDropDownList(string label, bool singleSelect = true)
       {
           if (singleSelect)
               return _sideBarPanelSearch.GetAvailableDropDownList(label);
           return _sideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
       }

       /// <summary>
       /// Check whether values are in ascending order or not only for category
       /// </summary>
       /// <param name="values"></param>
       /// <returns></returns>
       public bool IsListInAscendingOrder(List<String> searchListValue)
       {
           var sorted = new List<string>(searchListValue);
           sorted.Sort( CompareOrdinal);
           for (var i = 0; i < sorted.Count; i++)
           {
               if (searchListValue[i].Equals(sorted[i])) continue;
                WriteLine("{0} and {1}", searchListValue[i], sorted[i]);
               return false;
           }
           return true;
       }

       public bool IsAppealSearchQuickLaunchTilePresent()
       {
           return SiteDriver.IsElementPresent(AppealSearchPageObjects.AppealSearchQuickLaunchTile,
               How.CssSelector);
       }

       public int FilterCountByLabel()
       {
           return _sideBarPanelSearch.FilterCountByLabel();
       }

       public string GetInputValueByLabel(string label)
       {
           return
               _sideBarPanelSearch.GetInputValueByLabel(label);
       }
       public bool IsInputFieldForRespectiveLabelDisabled(string label)
       {
           return _sideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled(label);
       }

       public void SetInputFieldByInputLabel(string label, string value)
       {
           _sideBarPanelSearch.SetInputFieldByLabel(label, value);
       }
       public void ClickOnClearLink()
       {
           JavaScriptExecutor.ExecuteClick( Format(AppealSearchPageObjects.ClearCancelXPathTemplate, "Clear"),
               How.XPath);
            WriteLine("Clear Link Clicked");
       }
       public String GetSearchResultByColRow(int col, int row = 1)
       {
           return
               SiteDriver.FindElement(
                    Format(AppealSearchPageObjects.AppealSearchResultByRowColCssTemplate, col, row),
                   How.CssSelector).Text;
       }

       public void SelectSMTST()
       {
           SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
       }

       public bool IsSearchInputFieldDispalyedByLabel(string label)
       {
           return _sideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(label);
       }
       public bool IsAdvancedSearchFilterIconDispalyed()
       {
           return _sideBarPanelSearch.IsAdvancedSearchFilterIconDispalyed();
       }

        public bool IsAdvancedSearchFilterIconSelected()
        {
            return _sideBarPanelSearch.IsAdvancedSearchFilterIconSelected();
        }

        public void CompleteAppeals(string[] createdAppealSeq)
        {
            foreach (var appealSeq in createdAppealSeq)
            {
                var newAppealAction = SearchByAppealSequenceNavigateToAppealAction(appealSeq);
                newAppealAction.CompleteAppeals(handlePopUp: true, isCompleted: true, isAction: false);
            }
        }

        public AppealSummaryPage SwitchToOpenAppealSummaryByUrlForAdmin1(string url)
        {
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor).LoginAsHciAdminUser();
            var appealSummarypage = Navigator.Navigate<AppealSummaryPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSummaryPageObjects.PageHeaderCssLocator, How.CssSelector));
            var appealSummary = new AppealSummaryPage(Navigator, appealSummarypage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            return appealSummary;
        }

        public AppealSummaryPage NavigateToAppealSummaryPageBySideBarPannel(bool advancedFilters = false)
        {
            if (advancedFilters)
            {
                _sideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                WaitForWorking();
            }

            SelectAllAppeals();
            SelectClientSmtst();
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Status",AppealStatusEnum.Complete.GetStringDisplayValue());
            WaitForStaticTime(500);
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(()=>_gridViewSection.GetGridRowCount()>0,10000);
            CaptureScreenShot("appeal_search_issue");
            _gridViewSection.ClickOnGridByRowCol(1,3);
            return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            
        }

        //public List<string> GetAppealSearchResultListByCol(int col)
        //{
        //    return SiteDriver
        //        .FindElements(
        //             Format(NewAppealSearchPageObjects.AppealSearchResultByColCssTemplate, col), How.CssSelector, "text");
        //}

        #endregion

        #region PRIVATE METHODS



        #endregion


        public List<string> GetAppealsOnClaimsDb(string claimSeqList)
        {
            return Executor.GetTableSingleColumn( Format(AppealSqlScriptObjects.GetAppealsOnClaimSeq, claimSeqList));
        }

        public void RevertSaveAppealDraft(string claseq)
        {
            var claimSequence = claseq.Split('-');
            Executor.ExecuteQuery( Format(AppealSqlScriptObjects.UpdateAppealLineSaveDraft, claimSequence[0],
                claimSequence[1]));
            WriteLine($"Set result,reason code, rationale and summary null for claseq : {claseq}");
        }

        public List<string> GetOutstandingDCIAppealsFromDb(string userId)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetOutstandingDCIAppeals,userId));
        }

        public List<string> GetAppealsUsingAnalysts(string analyst1, string analyst2, string userseq)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetAppealsUsingAnalysts, analyst1, analyst2,
                userseq));
        }
        public List<string> GetMyAppealsForInternalUserFromDB(string user)
        {
            return Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.MyAppealsForInternalUser, user));
        }

        public List<string> GetMyAppealsForClientUserFromDB(string user)
        {
            return Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.MyAppealsForClientUser, user));
        }

        




        public List<string> GetLineOfBusinessFromDb()
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetLineOfBusiness));
        }

        public bool IsAppealLevelBadgeValuePresentForMRRAppealType()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.AppealLevelBadgeValueForMRRAppealType, How.XPath);
        }

        public void updateClientProcessingType(string type,string clientcode)
        {
            Executor.ExecuteQuery(string.Format(SettingsSqlScriptObject.UpdateProcessingTypeOfClient, type, clientcode));
        }



        public void DeleteAppealsFromRealTimeQueue(string claseq)
        {
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.DeleteFromRealTimeQueue,claseq));
        }
    }
}
