using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class COBClaimsDetailPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private COBClaimsDetailpageObject _cobClaimsDetailPage;


        #endregion

        #region CONSTRUCTOR

        public COBClaimsDetailPage(INewNavigator navigator, COBClaimsDetailpageObject cobClaimsDetailPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, cobClaimsDetailPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _cobClaimsDetailPage = (COBClaimsDetailpageObject) PageObject;
        }

        #endregion

        #region PUBLIC METHODS
        public string GetCOBClaimsDetailPageHeader()
        {
            return SiteDriver.FindElement(COBClaimsDetailpageObject.COBClaimsDetailPageheaderXPath, How.XPath).Text;
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<COBClaimsDetailPage>();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(COBClaimsDetailpageObject.COBClaimsDetailPageheaderXPath, How.XPath));
            SiteDriver.WaitForPageToLoad();
            return fileName;

        }

        public void CloseConnection()
        {
            Executor.CloseConnection();
        }

        public bool IsWidgetPresentInDashboard(string widgetname)
        {
            return JavaScriptExecutor.IsElementPresent(Format(COBClaimsDetailpageObject.DashboardWidgetByTitle,
                widgetname));

        }

        public void ClickOnExportIconByWidget(string widgetName)
        {
            JavaScriptExecutor.FindElement(Format(COBClaimsDetailpageObject.ExportIconInDashboardWidget, widgetName))
                .Click();
        }

        public bool IsExportIconPresentBywidget(string widgetName)
        {
            return JavaScriptExecutor.IsElementPresent(Format(COBClaimsDetailpageObject.ExportIconInDashboardWidget,
                widgetName));
        }

        public bool IsCollapseIconPresentBywidget(string widgetName)
        {
            return JavaScriptExecutor.IsElementPresent(Format(COBClaimsDetailpageObject.CollapseIconDashboardWidget,
                widgetName));
        }

        public DashboardPage NavigateToCOBDashboard(Action action)
        {

            var dashboardPage = Navigator.Navigate<DashboardPageObjects>(action);
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() =>
                !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new DashboardPage(Navigator, dashboardPage, SiteDriver, JavaScriptExecutor, EnvironmentManager,
                BrowserOptions, Executor);
        }



        public void ClickCollapseIconByWidgetName(string widgetName)
        {
            JavaScriptExecutor.FindElement(Format(COBClaimsDetailpageObject.CollapseIconDashboardWidget, widgetName))
                .Click();
            SiteDriver.WaitForPageToLoad();
        }


        public string IsDashboardNamePresentInWidget(string WidgetName)
        {
            return JavaScriptExecutor.FindElement(COBClaimsDetailpageObject.DashboardTypeInWidgetByName).Text;
        }

        public List<String> GetCOBAssignedClientList(string widgetName)
        {
            return JavaScriptExecutor.FindElements(Format(COBClaimsDetailpageObject.AssignedClientsInDashboard,
                widgetName));
        }

        public List<string> GetCOBClientsFromDb(string user)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.COBClientsForUser, user));
        }


        public List<List<string>> GetDashboardDataFromDb(string username)
        {
            var list = Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.COBBatchData, username));

            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();

        }

        public List<List<string>> GetTotalsByClientDataFromDb(string userId)
        {
            var list = Executor.GetCompleteTable(
                Format(DashboardSqlScriptObjects.COBUnreviewedClaimsCountByClientForInternalUser, userId));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public List<List<string>> GetTotalsByClientDataFromDbClientUser(string userId)
        {
            var list = Executor.GetCompleteTable(
                Format(DashboardSqlScriptObjects.COBUnreviewdClaimsCountByClientForClientUser, userId));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public List<string> GetTotalPaidForCOBDashboardTotalsByFlagDb(string userId)=>
            Executor.GetTableSingleColumn(Format(
                DashboardSqlScriptObjects.GetTotalPaidForCOBDashboardTotalsByFlag,userId));

       public bool IsAtLeastOneNonDeletedCOBFlagPresent(string client)
        {
            var list = Executor.GetTableSingleColumn(
                Format(DashboardSqlScriptObjects.GetStatusOfEdits, client));
            return list.Any(val => val.Equals("A"));
        }

        public string GetTotalPaidPerClientDb(string client, string userID) =>
            Executor.GetSingleStringValue(
                Format(DashboardSqlScriptObjects.GetTotalPaidPerClient, client, userID));
       

        public List<string> GetLastFiveDays(string WidgetName)
        {
            return JavaScriptExecutor.FindElements(Format(COBClaimsDetailpageObject.LastFiveDaysByCSS, WidgetName));
        }

        public List<string> GetClaimsCountByDay(string WidgetName, int i)
        {
            return JavaScriptExecutor.FindElements(Format(COBClaimsDetailpageObject.ClaimsCountByDay, WidgetName, i));
        }



        public bool IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(string title) =>
            JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.ContainerHeaderWidgetOverviewCOBTemplate,
                title));

        #endregion

        public List<string> GetColumnNamesByWidgetName(string widgetName)
        {
            return JavaScriptExecutor.FindElements(Format(COBClaimsDetailpageObject.ColumnNamesByCssSelector,
                widgetName));
        }

        public string GetUnreviewedClaimsCountForEachClient(string widgetName, int row)
        {
            return JavaScriptExecutor
                .FindElement(Format(COBClaimsDetailpageObject.UnreviewedClaimsCountByCssSelector, widgetName, row))
                .Text;
        }

        public string GetClientNameByRow(string widgetName, int row)
        {
            return JavaScriptExecutor
                .FindElement(Format(COBClaimsDetailpageObject.ClientNameByRowCssSelector, widgetName, row)).Text;
        }

        public bool IsNextRefreshTimePresent()
        {
            return SiteDriver.FindElement(DashboardPageObjects.NextRefreshTimeCssSelector, How.CssSelector).Text
                .Contains("Next Refresh :");
        }

        public bool IsTotalClaimsAndMemberCountValidByWidgetNameAndCol(string widget, int col)
        {
            var counts = JavaScriptExecutor.FindElements(
                    Format(COBClaimsDetailpageObject.CobClaimsDetailDataByWidgetAndColCssSelectorTemplate, widget, col))
                .Select(x => int.Parse(x)).ToList();
            var condition = counts.TrueForAll(x => x >= 0);
            return condition;
        }

        public bool IsTotalClaimCountByWidgetNameAndClientGreaterOrEqualsZero(string widget, string client)
        {
            var data = JavaScriptExecutor.FindElements(
                    Format(COBClaimsDetailpageObject.TotalClaimByClientCssSelectorTemplate, widget, client)).Skip(1).ToList();
            var counts = data.Select(x => int.Parse(x)).Skip(1).ToList();
            var condition = counts.TrueForAll(x => x >= 0);
            return condition;
        }

        public List<string> GetCobClaimsDetailDataByWidgetAndCol(string widget, int col) =>
            JavaScriptExecutor.FindElements(
                Format(COBClaimsDetailpageObject.CobClaimsDetailDataByWidgetAndColCssSelectorTemplate, widget, col));

        public List<string> GetTotalPaidClaimsDetail() =>
            JavaScriptExecutor.FindElements(COBClaimsDetailpageObject.TotalPaidClaimsDetailCssSelector, "innerText");

        public List<string> GetTotalOfTotalPaidClaimsDetail() =>
            JavaScriptExecutor.FindElements(COBClaimsDetailpageObject.TotalOfTotalPaidClaimsDetailCssSelector, "innerText");

    }

}