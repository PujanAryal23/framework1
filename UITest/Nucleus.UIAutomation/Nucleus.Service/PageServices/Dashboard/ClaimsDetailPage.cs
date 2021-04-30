using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class ClaimsDetailPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private ClaimsDetailPageObjects _claimsDetailPage;
        private CommonSQLObjects _commonSQLObjects;
        private JsonParser _jsonParser;
        
        #endregion

        #region CONSTRUCTOR

        public ClaimsDetailPage(INewNavigator navigator, ClaimsDetailPageObjects claimsDetailPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimsDetailPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _claimsDetailPage = (ClaimsDetailPageObjects)PageObject;
            _commonSQLObjects=new CommonSQLObjects(Executor);
            _jsonParser=new JsonParser(SiteDriver);
        }

        #endregion

        #region PUBLIC METHODS

        public void CloseConnection()
        {
            Executor.CloseConnection();
        }
        

        public List<List<String>> GetAllCotivitiExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var claimByClilentList =
                 Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.ClaimByClilentForInternalUser, userSeq));
            list.Add(claimByClilentList);
            Executor.CloseConnection();
            return list;
        }

        public List<List<String>> GetAllClientExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var claimByClilentList =
                      Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.ClaimByClilentForClientUser, userSeq));
            list.Add(claimByClilentList);
            Executor.CloseConnection();
            return list;

        }
        public List<List<String>> GetAllFfpCotivitiExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var ffpClaimByClientList =
                 Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.FFPClaimByClientForInternalUser, userSeq));
            list.Add(ffpClaimByClientList);
            Executor.CloseConnection();
            return list;
        }


        public string GetClaimsDetailHeader()
        {
            return SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.ClaimsDetailHeaderXPath), How.XPath).Text;
        }
       

        public List<String> GetFirstRowColumnHeaders()
        {
            var firstRowColumnHeaders = new List<String>();
            
            for (int i = 1; i <= GetFirstRowColumnHeaderCount(); i++)
            {
                firstRowColumnHeaders.Add(GetFirstRowColumnHeader(i));
            }

            return firstRowColumnHeaders;
        }

      
        public List<String> GetSecondRowColumnHeaders()
        {
            var secondRowColumnHeaders = new List<String>();
            for (int i = 1; i <= GetSecondRowColumnHeaderCount(); i++)
            {
                secondRowColumnHeaders.Add(GetSecondRowColumnHeader(i));
                    
            }

            return secondRowColumnHeaders;
        }

       

        

        public bool IsListInAscedingOrder(List<string> list)
        {
            return list.IsInAscendingOrder();
        }

        public bool GetAllColumnsValues(string widget)
        {
            
                for (int j = 1; j <= GetClaimsByClientColumnCount(); j++)
                {
                    if (GetColumnValue( j, widget).Contains(""))
                    {
                        return false;
                    }
                }
            
            return true;
        }

        public int GetFirstRowColumnHeaderCount()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.FirstRowColumnHeaderCountXPath, How.XPath);
        }
       

        public int GetSecondRowColumnHeaderCount()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.SecondRowColumnHeaderCountXPath, How.XPath);
        }

        public int GetActiveClientsCount(string widget)
        {
            return SiteDriver.FindElementsCount(string.Format(ClaimsDetailPageObjects.ActiveClientsCountXPath,widget), How.XPath);
        }

        public string GetFirstRowColumnHeader(int column)
        {
            string firstRowColumnHeader = SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.FirstRowColumnHeaderXPath, column), How.XPath).Text;
            return Regex.Replace(firstRowColumnHeader, @"[\d-]", string.Empty).Trim();
        }

       
        public string GetSecondRowColumnHeader(int column)
        {
            string secondRowColumnHeader = SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.SecondRowColumnHeaderXPath, column), How.XPath).Text;
            return secondRowColumnHeader.Split(':')[0].Trim();
        }
        

        public List<string> GetActiveClientList(string widget)
        {
            if(widget== "Claims by Client")
                return JavaScriptExecutor.FindElements(string.Format(ClaimsDetailPageObjects.ActiveClientClaimsByClientXPath, widget), How.XPath, "Text");
            return JavaScriptExecutor.FindElements(string.Format(ClaimsDetailPageObjects.ActiveClientXPath, widget), How.XPath,"Text");
        }

        public List<string> GetColumnValue(int column, string widget)
        {
            return JavaScriptExecutor.FindElements(string.Format(ClaimsDetailPageObjects.ColumnValuesXPath, column, widget), How.XPath,"Text");
        }

        public int GetClaimsByClientRowCount()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.ClaimsByClientRowCountXPath, How.XPath);
        }

        public List<string> GetClientListByWidget(string widgetName)
        {

            return JavaScriptExecutor.FindElements(string.Format(ClaimsDetailPageObjects.GetClientListFromByWidgetLabelXPath,widgetName), How.XPath,"Text");
        }
        public int GetClaimsByClientColumnCount()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.FirstRowColumnHeaderCountXPath, How.XPath);
        }

        public int GetHeaderValue(int column)
        {
            string headerValue = column == 2 ? SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.SecondRowColumnHeaderXPath, column), How.XPath).Text : SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.FirstRowColumnHeaderXPath, column), How.XPath).Text;
            return Convert.ToInt32(Regex.Replace(headerValue, "[^0-9]+", string.Empty));
        }

        public int GetHeaderValueForFirstRow(int column)
        {
            string headerValue = SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.FirstRowColumnHeaderXPath, column), How.XPath).Text;
            return Convert.ToInt32(Regex.Replace(headerValue, "[^0-9]+", string.Empty));
        }
        public int GetHeaderValueForClientDashboard(int column)
        {
            string headerValue = SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.FirstRowColumnHeaderXPath, column), How.XPath).Text;
            return Convert.ToInt32(Regex.Replace(headerValue, "[^0-9]+", string.Empty));
        }

        public int GetEachColumnSumValue(int col, string widget)
        {
            int sum = 0;
            Console.WriteLine(GetClientValuesRowCount());
            for (int i = 1; i <= GetClientValuesRowCount(); i++)
            {
                sum += Convert.ToInt32(GetEachGridValue(i, col, widget));
            }
            return sum;
        }

        public int GetClientValuesRowCount()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.ClientValuesRowCountXPath, How.XPath);
        }

        public string GetEachGridValue(int row, int column,string widget)
        {
            var str = SiteDriver
                .FindElement(string.Format(ClaimsDetailPageObjects.ClientValuesXPath, row, column, widget),
                    How.XPath).Text;
            return str;
        }

        public bool IsSubMenuDownloadPDFPresent()
        {
            return SiteDriver.IsElementPresent(ClaimsDetailPageObjects.DownloadPDFLinkXPath, How.XPath);
        }

        public bool IsSpinnerWrapperPresent()
        {
            return SiteDriver.IsElementPresent(ClaimsDetailPageObjects.SpinnerWrapperXPath, How.XPath);
        }

        public void ClickOnDashboardIcon()
        {
            SiteDriver.MouseOver(DefaultPageObjects.UserMenuId, How.Id);
            //JavaScriptExecutors.ExecuteMouseOver(DefaultPageObjects.UserMenuDivCssLocator, How.CssSelector);
            var element = SiteDriver.FindElement(DefaultPageObjects.DashboardIconXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForPageToLoad();
            JavaScriptExecutor.ExecuteMouseOut(DefaultPageObjects.UserMenuDivCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Dashboard icon.");
        }

       

        public void ClickOnDownloadPDF()
        {
            SiteDriver.FindElement(ClaimsDetailPageObjects.DownloadPDFLinkXPath,How.XPath).Click();
            SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count == 1, 10000);
            Console.WriteLine("Clicked on Download PDF");
        }


        public string GetUnreviewedClaimsLabelTooltipClientDashboardDetail()
        {
            return SiteDriver.FindElement(ClaimsDetailPageObjects.UnreviewedClaimswDivMainLabelCssSelector, How.CssSelector).GetAttribute("title");
            
        }

        public string Get10DaysOldClaimsLabelTooltipClientDashboardDetail()
        {
           return SiteDriver.FindElement(ClaimsDetailPageObjects.UnreviewedClaimswDivSecondaryLabelCssSelector, How.CssSelector).GetAttribute("title");
        
        }

        public string GetPendedClaimLabelTooltipClientDashboardDetail()
        {
            return SiteDriver.FindElement(ClaimsDetailPageObjects.PendedClaimswDivMainLabelCssSelector, How.CssSelector).GetAttribute("title");
        }

        public string Get10DaysOldPendedClaimsLabelTooltipClientDashboardDetail()
        {
            return SiteDriver.FindElement(ClaimsDetailPageObjects.PendedClaimswDivSecondaryLabelCssSelector, How.CssSelector).GetAttribute("title");
        }

        public string GetUnapprovedClaimsLabelTooltipClientDashboardDetail()
        {
            return SiteDriver.FindElement(ClaimsDetailPageObjects.UnapprovedClaimswDivMainLabelCssSelector, How.CssSelector).GetAttribute("title");
        }
        public string GetApprovedYesterdayClaimsLabelTooltipClientDashboardDetail()
        {
            return SiteDriver.FindElement(ClaimsDetailPageObjects.UnapprovedClaimswDivSecondaryLabelCssSelector, How.CssSelector).GetAttribute("title");
        }


        public List<String> GetClaimByClientsList()
        {
            var claimClients = new List<String>();
            for (int i = 1; i <= GetClaimByClientsCount(); i++)
            {
                claimClients.Add(GetClaimByClientData(i));
            }
            return claimClients;

        }


        public int GetClaimByClientsCount()
        {
            return  SiteDriver.FindElementsCount(ClaimsDetailPageObjects.ClaimByClientDetailRowCountCssSelector, How.CssSelector);
        
           
        }

        public string GetClaimByClientData(int row)
        {
           return SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.ClaimByClientDetailRowValueCssSelector, row), How.CssSelector).Text;
           
          
        }
        
        public bool IsClaimDetailsValueNull()
        {
            for (int i = 1; i <= GetClaimByClientsCount(); i++)
            {
                for (int j = 1; j <= GetClaimsByClientColumnCount(); j++)
                {
                    if (string.IsNullOrEmpty(GetClientColumnValue(i, j) ))
                        return true;
                }
            }
            return false;
        }


        public string GetClientColumnValue(int row, int column)
        {
            return SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.ClaimByClientDetailAllColumnDataCssSelectorTemplate, row, column), How.CssSelector).Text;
          
        }

        public bool IsClaimDetailsHeaderValueNull()
        {
            for (int j = 1; j <= GetClaimsByClientColumnCount(); j++)
            {
                if (string.IsNullOrEmpty(GetHeaderClientDetailValue(j)))
                    return true;
            }
            return false;
        }

        
        public string GetHeaderClientDetailValue(int column)
        {
            string headerValue = SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.FirstRowColumnHeaderXPath, column), How.XPath).Text;
            return Regex.Replace(headerValue, "[^0-9]+", string.Empty);
        }

        public string GetExpectedUnreviewedFFPClaimsCountByClient(string client)
        {           
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.FFPUnreviewedClaimForCotivitiUserByClient, client)).ToString();
        }

        public string GetExpectedPendedFFPClaimsCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.FFPPendedClaimForInternalUserByClient, client)).ToString();
           
        }

        public string GetExpectedUnreleasedFFPClaimsCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.FFPUnreleasedClaimForInternalUserByClient, client)).ToString();

        }

        public string GetClaimsDetailContainerHeader(int i = 1)
        {
            return SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.ClaimsDetailContainerHeadersXPath,i), How.XPath).Text;
        }
        public List<String> GetRealTimeClaimsColumnHeaders()
        {
            List<string> columns= JavaScriptExecutor.FindElements(ClaimsDetailPageObjects.RealTimeClaimsColumnHeaderXPath, How.XPath, "Text");
            return columns;
        }
       

        public int GetRealTimeColumnHeader()
        {
            return SiteDriver.FindElementsCount(ClaimsDetailPageObjects.RealTimeClaimsColumnHeaderXPath, How.XPath);
        }

        public List<List<String>> GetAllRealTimeExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var claimByClilentList =
                Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.RealTimeClientsForInternalUser, userSeq));
            list.Add(claimByClilentList);
            Executor.CloseConnection();
            return list;

        }

        public bool GetAllRealTimeColumnsValues(string widget)
        {
            
                for (int j = 1; j <= GetRealTimeColumnHeader(); j++)
                {
                    if (GetColumnValue( j, widget) .Contains(""))
                    {
                        return false;
                    }
                }
            
            return true;
        }

        public string GetRealTimeColumnValue(int row, int column)
        {
            return SiteDriver.FindElement(string.Format(ClaimsDetailPageObjects.RealTimeColumnValuesXPath, row, column), How.XPath).Text;
        }




        public void ClickOnRealTimeClaimsDownloadPDF()
        {
            SiteDriver.FindElement(ClaimsDetailPageObjects.RealTimeClaimsDownloadPDFLinkXPath, How.XPath).Click();
            SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count == 1, 10000);
            Console.WriteLine("Clicked on Download PDF");

        }

        public bool IsRealTimeSubMenuDownloadPDFPresent()
        {
            return SiteDriver.IsElementPresent(ClaimsDetailPageObjects.RealTimeClaimsDownloadPDFLinkXPath, How.XPath);
        }

        public bool IsClaimRestrictionSectionPresentForRealTimeClient(string client)
        {
            return JavaScriptExecutor.IsElementPresent(Format(ClaimsDetailPageObjects
                .ClaimsByRestrictionInRealTimeClientByClientNameCssSelector, client));
        }

        public void ClickOnCaretSignByRealTimeClient(string client)
        {
            JavaScriptExecutor.ClickJQuery(Format(ClaimsDetailPageObjects.CaretSignForRealTimeClientByClientNameCssSelector, client));
        }

        public List<string> GetClaimRestrictionNamesByRealTimeClient(string client)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimsDetailPageObjects.ClaimRestrictionsForRealTimeClientCssSelector, client));
        }

        public List<string> GetUnreviewedClaimDataForRealTimeClientFromDB(string client)
        {
            var result =
                Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.GetTotalUnreviewedDataForRealTimeClients,
                    client));

            var resultList =  result.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();

            return resultList[0];
        }

        public List<List<string>> GetRestrictedClaimDataForRealTimeClientFromDB(string client)
        {
            var result =
                Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.DataByClaimRestrictionForRealTimeClients,
                    client));

            return result.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public void SwitchWidgetByTabName(string widget)
        {
            SiteDriver.FindElement(Format(ClaimsDetailPageObjects.RealTimeWidgetXPathTemplate,widget), How.XPath).Click();
        }

        public List<string> GetClaimCountsByRestrictionInRealTimeClientByClientName(string client)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimsDetailPageObjects.ClaimsByRestrictionInRealTimeClientByClientNameCssSelector, client));
        }

        public List<string> GetClaimsCountForEveryDueTimeForRealTimeClientCssSelector(string client)
        {
            return JavaScriptExecutor.FindElements(Format(
                ClaimsDetailPageObjects.OverallUnreviewedClaimsForRealTimeClientCssSelector, client));
        }

        public string GetExpectedOverdueUnreviewedPCIClaimsCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.OverdueUnreviewedClaimsCountByClient, client)).ToString();
        }

        public string GetExpectedUnreviewedPCIClaimsLessThan30MinsCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.UnreviewedClaimsWithDuedateLessThan30MinsByClient, client)).ToString();
        }

        public string GetExpectedUnreviewedPCIClaimsLessThan1HrCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.UnreviewedClaimsWithDuedateLessThan1HrByClient, client)).ToString();
        }

        public string GetExpectedUnreviewedPCIClaimsLessThan2HrCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.UnreviewedClaimsWithDuedateLessThan2HrByClient, client)).ToString();
        }

        public string GetExpectedUnreviewedPCIClaimsLessThan4HrCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.UnreviewedClaimsWithDuedateLessThan4HrByClient, client)).ToString();
        }

        public string GetExpectedUnreviewedPCIClaimsLessThan6HrCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.UnreviewedClaimsWithDuedateLessThan6HrByClient, client)).ToString();
        }

        public string GetExpectedPCIUnreviewedClaimsCountByClient(string client)
        {
            return Executor.GetSingleValue(string.Format(DashboardSqlScriptObjects.PCIUnreviewedClaimsCountByClient, client)).ToString();
        }
        #endregion

        public int GetAwaitingQAClaimsForAPIByClient(string client)
        {
            _commonSQLObjects.DeleteLockedClaimsByClient(client);
            return _jsonParser.GetClaseqListFromAPI(client, "100",
                SiteDriver.BaseUrl + PageUrlEnum.PCIQAWorkListResponse.GetStringValue()).Count+1;


        }

        public bool IsQuickLaunchIconPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.QuickLaunchButtonXPath, How.XPath);
        }


        public bool IsSwitchClientIconPresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.SwitchClientCssSelector, How.CssSelector);
        }

        public bool DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage()
        {
            return SiteDriver.FindElement(DashboardPageObjects.NextRefreshTimeCssSelector, How.CssSelector).Text.Contains("Next Refresh :");
        }

        public List<string> GetClaimsDetailRealTimeClaimsDataHeader() =>
            JavaScriptExecutor.FindElements(ClaimsDetailPageObjects.ClaimsDetailRealTimeClaimsDataHeaderCssSelector,
                How.CssSelector, "Text".TrimStart());
        public List<string> GetOverallClaimCountsWaitingForQCReviewFromDB(string client)
        {
            var result =
                Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.GetClientSpecificRealTimeQCClaimsCount,
                    client));

            var resultList = result.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();

            return resultList[0];
        }

        public bool IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(string title) => JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.ContainerHeaderWidgetOverviewCOBTemplate, title));
    }
}
