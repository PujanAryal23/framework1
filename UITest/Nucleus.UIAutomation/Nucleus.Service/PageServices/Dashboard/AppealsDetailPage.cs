using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class AppealsDetailPage : NewBasePageService 
    {
        #region PRIVATE FIELDS

        private AppealsDetailPageObjects _appealsDetailPage;
        
        #endregion

        #region CONSTRUCTOR

        public AppealsDetailPage(INewNavigator navigator, AppealsDetailPageObjects appealsDetailPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealsDetailPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealsDetailPage = (AppealsDetailPageObjects)PageObject; 
            
        }

        #endregion

        #region PUBLIC METHODS


        public List<List<String>> GetAllCotivitiExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var appealsByClientActiveClientList =
               Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.AppealsByClientForInternalUser, userSeq));
            list.Add(appealsByClientActiveClientList);
            var allCotivitiUserWithAssignAppealsAuthrityList =
                Executor.GetTableSingleColumn(DashboardSqlScriptObjects.CotivitiUserWithAssignAppealsAuthorityList,1);
            list.Add(allCotivitiUserWithAssignAppealsAuthrityList);

            var allCotivitiUserWithDueAppeal =
                Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.CotivitiUserWithYelloIconList, userSeq));
            allCotivitiUserWithDueAppeal.Sort();
            list.Add(allCotivitiUserWithDueAppeal);
            Executor.CloseConnection();
            return list;
        }

        public List<List<String>> GetAllClientExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
           
            var appealsByClientActiveClientList =
               Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.AppealsByClientForClientUser, userSeq));
            list.Add(appealsByClientActiveClientList);
            Executor.CloseConnection();
            return list;

        }

        /// <summary>
        /// Navigate to download page for chrome
        /// </summary>
        /// <returns></returns>
        public ChromeDownLoadPage NavigateToChromeDownLoadPage()
        {
            var url = "Chrome://Downloads/";
            if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"], "edge", StringComparison.OrdinalIgnoreCase) == 0)
                url = "edge://downloads/all";
            var chromeDownLoad = Navigator.Navigate<ChromeDownLoadPageObjects>(() => SiteDriver.Open(url));
            return new ChromeDownLoadPage(Navigator, chromeDownLoad, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetAppealsDetailPageHeader()
        {
            return SiteDriver.FindElement(AppealsDetailPageObjects.AppealsDetailPageheaderXPath, How.XPath).Text;
        }

        public string GetAppealsDetailContainerHeader(int container)
        {
            return SiteDriver.FindElement(String.Format(AppealsDetailPageObjects.AppealsDetailContainerHeadersCSS, container), How.CssSelector).Text;
        }

        

       

        public List<String> GetActiveClients()
        {
            var activeClients = new List<String>();
            for (int i = 1; i <= GetActiveClientsCount(); i++)
            {
                activeClients.Add(GetActiveClient(i));
            }
            return activeClients;

        }

        

        public bool IsListContainsCotivitiUserWithAssignAppelsAuth(IEnumerable<string> analystsList,List<String> allCotivitiUserWithAssignAppealsAuthrityList)
        {
            foreach (var analyst in analystsList.Where(analyst => !allCotivitiUserWithAssignAppealsAuthrityList.Contains(analyst)))
            {
                Console.WriteLine("{0} is Cotiviti User with Appeals Assign authority", analyst);
                return false;
            }

            return true;
        }

        public bool IsListDoesNotContainsCotivitiUserWithoutAssignAppelsAuth(IEnumerable<string> analystsList,List<String> allCotivitiUserWithoutAssignAppealsAuthrityList)
        {
            foreach (var analyst in analystsList.Where(allCotivitiUserWithoutAssignAppealsAuthrityList.Contains))
            {
                Console.WriteLine("{0} is Cotiviti User without Appeals Assign authority", analyst);
                return false;
            }

            return true;
        }

        public bool IsListDoesNotContainsClientUsers(IEnumerable<string> analystsList,List<String> clientUsersList)
        {
            foreach (var analyst in analystsList.Where(clientUsersList.Contains))
            {
                Console.WriteLine("{0} is client user.", analyst);
                return false;
            }

            return true;
        }

        public List<string> GetAnalysts()
        {
            //var analysts = new List<String>();
            //for (int i = 1; i <= CotivitiUserCountWithAppealsAssignAuth(); i++)
            //{
            //    analysts.Add(GetCotivitiUserWithAppealsAssignAuth(i));
            //}
            //return analysts;
            return JavaScriptExecutor.FindElements(AppealsDetailPageObjects.CotivitiUserWithAppealsAssignAuthCssLocator, How.CssSelector, "Text");
        }

        public int CotivitiUserCountWithAppealsAssignAuth()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.CotivitiUserCountWithAppealsAssignAuthXPath, How.XPath);
        }

        public string GetCotivitiUserWithAppealsAssignAuth(int row)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.CotivitiUserWithAppealsAssignAuthXPathTemplate, row), How.XPath).Text;
        }

        public int GetActiveClientsCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.ActiveClientsCountXPath, How.XPath);
        }

        public string GetActiveClient(int row)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.ActiveClientXPath, row), How.XPath).Text;
        }

        public string GetFirstRowColumnHeader(int column)
        {
            string firstRowColumnHeader = SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.FirstRowColumnHeaderCSS, column), How.CssSelector).Text;
            return Regex.Replace(firstRowColumnHeader, @"[\d-]", string.Empty).Trim();
        }

        public string GetSecondRowColumnHeader(int column)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.SecondRowColumnHeaderCSS, column), How.CssSelector).Text.TrimEnd().Substring(0, 9);
        }

        public int GetColumnHeadersCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.ClientValuesColumnCountCss, How.CssSelector);
        }

        public int GetHeaderValue(int column)
        {
            string headerValue = SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.FirstRowColumnHeaderCSS, column), How.CssSelector).Text;
            return Convert.ToInt32(Regex.Replace(headerValue, "[^0-9]+", string.Empty));
        }

        public int GetEachColumnSumValue(int col)
        {
            int sum = 0;
            for (int i = 1; i <= GetClientValuesRowCount(); i++)
            {
                sum += Convert.ToInt32(GetEachGridValue(i, col));
            }
            return sum;
        }

        public string GetTotalAppealsCountByClient(string client)
        {
            return Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayCountByClientFromDb, client));
        }

        public string GetUrgentAppealsCountByClient(string client)
        {
            return Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayUrgentCountByClientFromDb, client));
        }

        public string GetRecordReviewsCountByClient(string client)
        {
            return Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayRecordReviewCountByClientFromDb, client));
        }

        public string GetMedicalRecordReviewsCountByClient(string client)
        {
            return Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayMedicalRecordReviewCountByClientFromDb, client));
        }

        public string GetRestrictedAppealsCountByClient(string client)
        {
            return Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayRestrictedAppealsCountByClientFromDb, client));
        }

        public int GetUnrestrictedTotalAppealsCount(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalUnRestrictedAppealsCountFromDb,user)));
        }

        public int GetUnrestrictedTotalAppealsCountRecordReview(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithRecordReviewType,user)));
        }

        public int GetUnrestrictedTotalAppealsCountMedicalRecordReview(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithMedicalRecordReviewType, user)));
        }

        public int GetUnrestrictedTotalAppealsCountUrgentPriority(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithUrgentPriority,user)));
        }

        public int GetRestrictedTotalAppealsCount(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalRestrictedAppealsCountFromDb,user)));
        }

        public int GetRestrictedTotalAppealsCountRecordReview(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalRestrictedAppealsCountFromDbWithRecordReviewType,user)));
        }

        public int GetRestrictedTotalAppealsCountMedicalRecordReview(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalRestrictedAppealsCountFromDbWithMedicalRecordReviewType, user)));
        }

        public int GetRestrictedTotalAppealsCountUrgentPriority(string user)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(
                string.Format(DashboardSqlScriptObjects.GetCVAppealsTotalRestrictedAppealsCountFromDbWithUrgentPriority,user)));
        }


        public int GetClientValuesRowCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.ClientValuesRowCountXPath, How.XPath);
        }

        public string GetEachGridValue(int row, int column)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.ClientValuesXPath, row, column), How.XPath).Text;
        }

        public bool IsSubMenuDownloadAppealsDetailPresentInAppealsbyClient()
        {
            return SiteDriver.IsElementPresent(AppealsDetailPageObjects.DownloadPDFLinkInAppealsByClientXPath, How.XPath);
        }

        public bool IsSubMenuDownloadAppealsDetailPresentInAppealsbyAnalyst()
        {
            return SiteDriver.IsElementPresent(AppealsDetailPageObjects.DownloadPDFLinkInAppealsByAnalystXPath, How.XPath);
        }

        public bool IsUsersWithOverdueAppealHasYellowIcon(int rowCount)
        {
            if (GetOverdueAppealYellowIcon(rowCount).Equals("Warning"))
                return true;

            return false;
        }

        public List<string> GetAllYellowIconUserList()
        {
            return JavaScriptExecutor.FindElements(AppealsDetailPageObjects.YellowIconUserListJQuery,"Text");
;        }

        public string GetOverdueAppealYellowIcon(int row)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.OverdueAppealYellowIconXPath, row), How.XPath).Text;
        }

        public string GetFirstRowColumnHeaderAppealsByAnalyst(int column)
        {
            return SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.FirstRowColumnHeaderAppealsByAnalystXPath, column), How.XPath).Text;
        }

        public int GetTotalAppealsCountForTodayInAppealsByAnalyst()
        {
            var text = SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.TotalAppealsCountInAppealsCountByAnalystXpath),How.XPath).Text;
            text = text.Split(' ')[2];
            return Convert.ToInt32(text.Trim());
        }

        public string GetSecondRowColumnHeaderAppealsByAnalyst(int column)
        {
            string columnHeader = SiteDriver.FindElement(string.Format(AppealsDetailPageObjects.SecondRowColumnHeaderAppealsByAnalystXPath, column), How.XPath).Text;
            return columnHeader.Substring(0, columnHeader.LastIndexOf(":"));
        }

        public int GetSecondRowColumnHeaderCountAppealsByAnalyst()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.SecondRowColumnHeaderCountAppealsByAnalystXPath, How.XPath);
        }

        public string GetActualDaysForColumn(int dayCount)
        {
            return DateTime.Now.AddDays(dayCount - 2).ToString("ddd MM/d");
        }

        public void ClickOnBoxWithArrowIconInAppealsByClient()
        {
            SiteDriver.FindElement(AppealsDetailPageObjects.BoxWithArrowIconInAppealsByClientXPath, How.XPath).Click();
        }

        public void ClickOnDownloadAppealsDetailInAppealsByClient()
        {
            SiteDriver.FindElement(AppealsDetailPageObjects.DownloadPDFLinkInAppealsByClientXPath, How.XPath).Click();
            Console.WriteLine("Clicked on Download Appeals Detail");
        }

        public void ClickOnBoxWithArrowIconInAppealsByAnalyst()
        {
            SiteDriver.FindElement(AppealsDetailPageObjects.BoxWithArrowIconInAppealsByAnalystXPath, How.XPath).Click();
        }

        public void ClickOnDownloadAppealsDetailInAppealsByAnalyst()
        {
            SiteDriver.FindElement(AppealsDetailPageObjects.DownloadPDFLinkInAppealsByAnalystXPath, How.XPath).Click();
            Console.WriteLine("Clicked on Download Appeals Detail");
        }

        public void ClickOnDashboardIcon()
        {
            SiteDriver.FindElement(AppealsDetailPageObjects.DashboardIconXPath, How.XPath).Click();
        }

        //Appeal Details OverView Page
        public int GetFirstRowColumnHeaderAppealByClientCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.FirstRowColumnHeaderAppealByClientCssLocator,
                How.CssSelector);
        }

        public string GetFirstRowColumnHeaderValueAppealByClient(int col)
        {
            var headerValue =
                SiteDriver.FindElement(
                    String.Format(AppealsDetailPageObjects.FirstRowColumnHeaderValueAppealByClientCssTemplate, col),
                    How.CssSelector).Text;
            return Regex.Replace(headerValue, "[^0-9]+", string.Empty);
        }

        public bool IsAppealDetailsHeaderValueByClientNull()
        {
            for (int j = 1; j <= GetFirstRowColumnHeaderAppealByClientCount(); j++)
            {
                if (string.IsNullOrEmpty(GetFirstRowColumnHeaderValueAppealByClient(j)))
                    return true;
            }
            return false;
        }

        public string GetFirstRowColumnHeaderYesterdayValueAppealByClient(int col)
        {
            var headerValue =
                SiteDriver.FindElement(
                    String.Format(AppealsDetailPageObjects.SecondRowColumnHeaderYesterdayValueAppealByClientCssTemplate, col),
                    How.CssSelector).Text;
            return Regex.Replace(headerValue, "[^0-9]+", string.Empty);
        }

        public bool IsAppealDetailsYesterdayHeaderValueByClientNull()
        {
            for (int j = 1; j <= GetFirstRowColumnHeaderAppealByClientCount(); j++)
            {
                if (string.IsNullOrEmpty(GetFirstRowColumnHeaderYesterdayValueAppealByClient(j)))
                    return true;
            }
            return false;
        }

        public int GetAppealByClientRowCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.AppealByClientRowCssLocator, How.CssSelector);
        }

        public string GetAppealByClientRowValue(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealsDetailPageObjects.AppealByClientValueCssTemplate, row, col), How.CssSelector)
                    .Text;
        }

        public bool IsAppealDetailsValueApppealByClientNull()
        {
            for (int i = 1; i <= GetAppealByClientRowCount(); i++)
            {
                for (int j = 1; j <= GetFirstRowColumnHeaderAppealByClientCount(); j++)
                {
                    if (string.IsNullOrEmpty(GetAppealByClientRowValue(i, j)))
                        return true;
                }
            }
            return false;
        }

        public string GetEachColumnSumValueAppealByClient(int col)
        {
            int sum = 0;
            for (int i = 1; i <= GetAppealByClientRowCount(); i++)
            {
                sum += Convert.ToInt32(GetAppealByClientRowValue(i, col));
            }
            return sum.ToString();
        }

       

        public bool IsAppealByAnalystAscendingOrder()
        {
            var listValue = JavaScriptExecutor.FindElements(AppealsDetailPageObjects.AppealByAnalystNameCssLocator, How.CssSelector, "Text");
            var searchListValues = listValue.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList();
            var sorted = new List<string>(searchListValues);
            sorted.Sort();
            for (var i = 0; i < sorted.Count; i++)
            {
                if (searchListValues[i].Equals(sorted[i])) continue;
                Console.WriteLine("{0} and {1}", searchListValues[i], sorted[i]);
                return false;
            }
            return true;
        }

        public int GetAppealByAnalystRowCount()
        {
           return SiteDriver.FindElementsCount(AppealsDetailPageObjects.AppealByAnalystRowCssLocator, How.CssSelector);
        }

        public int GetAppealByAnalystColumnCount()
        {
            return SiteDriver.FindElementsCount(AppealsDetailPageObjects.AppealByAnalystColumnCssLocator,
                How.CssSelector);
        }

        public bool IsAppealDetailsValueApppealByAnalystNull()
        {
            for (int i = 1; i <= GetAppealByAnalystRowCount(); i++)
            {
                for (int j = 1; j <= GetAppealByAnalystColumnCount(); j++)
                {
                    if (string.IsNullOrEmpty(GetAppealByAnalystRowValue(i, j)))
                        return true;
                }
            }
            return false;
        }

        public string GetAppealByAnalystRowValue(int row, int col)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealsDetailPageObjects.AppealByAnalystAppealValueCssTemplate, row, col), How.CssSelector)
                    .Text;
        }

        public int GetEachColumnSumValueAppealByAnalyst(int col)
        {
            int sum = 0;
            for (int i = 1; i <= GetAppealByAnalystRowCount(); i++)
            {
                sum += Convert.ToInt32(GetAppealByAnalystRowValue(i, col));
            }
            return sum;
        }

        public string GetSecondRowHeaderValueAppealByAnalyst(int col)
        {
            var headerValue =
                SiteDriver.FindElement(
                    String.Format(AppealsDetailPageObjects.SecondRowHeaderValueAppealByAnalystCssTemplate, col),
                    How.CssSelector).Text;
            return  Regex.Replace(headerValue, "[^0-9]+", string.Empty);
        }

        public bool IsAppealByAnalystSecondHeaderValueNull()
        {
            for (int i = 1; i <= GetAppealByAnalystColumnCount(); i++)
            {
                if (string.IsNullOrEmpty(GetSecondRowHeaderValueAppealByAnalyst(i)))
                    return true;
            }
            return false;
        }

        public bool IsAppealByAnalystSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealsDetailPageObjects.AppealByAnalystSectionCssLocator,
                How.CssSelector);
        }




        public bool DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage()
        {
            return SiteDriver.FindElement(DashboardPageObjects.NextRefreshTimeCssSelector, How.CssSelector).Text.Contains("Next Refresh :");
        }

        #endregion
    }
}
