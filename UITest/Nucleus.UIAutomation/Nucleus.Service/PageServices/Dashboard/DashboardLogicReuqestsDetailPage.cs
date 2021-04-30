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
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class DashboardLogicRequestsDetailsPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private DashboardLogicRequestsDetailPageObjects _logicReuqestsDetailPage;

        #endregion

        #region CONSTRUCTOR

        public DashboardLogicRequestsDetailsPage(INewNavigator navigator, DashboardLogicRequestsDetailPageObjects claimsDetailPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimsDetailPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _logicReuqestsDetailPage = (DashboardLogicRequestsDetailPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public List<List<String>> GetAllCotivitiExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var logicRequestByClilentList =
                 Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.LogicRequestByClilentForInternalUser, userSeq));
            list.Add(logicRequestByClilentList);
            Executor.CloseConnection();
            return list;
        }

        public List<List<String>> GetAllClientExpectedLists(string userSeq)
        {
            var list = new List<List<String>>();
            var logicRequestByClilentList =
                      Executor.GetTableSingleColumn(string.Format(DashboardSqlScriptObjects.LogicRequestByClilentForClientUser, userSeq));
            list.Add(logicRequestByClilentList);
            Executor.CloseConnection();
            return list;

        }


        public ChromeDownLoadPage NavigateToChromeDownLoadPage()
        {
            var url = "Chrome://Downloads/";
            if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"], "edge", StringComparison.OrdinalIgnoreCase) == 0)
                url = "edge://downloads/all";
            var chromeDownLoad = Navigator.Navigate<ChromeDownLoadPageObjects>(() => SiteDriver.Open(url));
            SiteDriver.WaitForPageToLoad();
            return new ChromeDownLoadPage(Navigator, chromeDownLoad, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetLogicRequestsDetailHeader()
        {
            return SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.LogicRequestsDetailPageHeaderCss), How.CssSelector).Text;
        }

        public string GetLogicRequestsGridHeader()
        {
            return SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.LogicRequestDetailGridHeaderCss), How.CssSelector).Text;
        }

        public string GetReadyForClientReviewClientUserColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OverdueRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Ready", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Ready", StringComparison.Ordinal)));
        }


        public string GetOverdueLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OverdueRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Overdue", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Overdue", StringComparison.Ordinal)));
        }

        public int GetTotalReadyForClientReviewClientUserLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OverdueRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Ready", StringComparison.Ordinal) - 1));
        }

        public int GetTotalOverdueLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OverdueRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Overdue", StringComparison.Ordinal) - 1));
        }


        public string GetDueIn30MinsLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Due", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Due", StringComparison.Ordinal)));
        }

        public string GetAwaitingCotivitiRReviewClientUserLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Awaiting", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Awaiting", StringComparison.Ordinal)));
        }

        public string GetReadyForClientReviewLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Ready", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Ready", StringComparison.Ordinal)));
        }

        public int GetTotalDueIn30MinsLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Due", StringComparison.Ordinal) - 1));
        }

        public int GetTotalAwaitingForClientReviewClientUserLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Awaiting", StringComparison.Ordinal) - 1));
        }

        public int GetTotalReadyForClientReviewLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.DueIn30MinsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Ready", StringComparison.Ordinal) - 1));
        }

        public string GetOpenLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OpenRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Open", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Open", StringComparison.Ordinal)));
        }

        public string GetAwaitingCotivitiReviewLogicRequestsColumnHeaderText()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OpenRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return (wholeText.Substring(wholeText.IndexOf("Awaiting", StringComparison.Ordinal), wholeText.Length - wholeText.IndexOf("Awaiting", StringComparison.Ordinal)));
        }

        public int GetTotalOpenLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OpenRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Open", StringComparison.Ordinal) - 1));
        }

        public int GetTotalAwaitingCotivitiReviewLogicRequestsCount()
        {
            string wholeText = SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.OpenRequestsHeaderCss), How.CssSelector).Text.Trim(
                );
            return Convert.ToInt32(wholeText.Substring(0, wholeText.IndexOf("Awaiting", StringComparison.Ordinal) - 1));
        }


        public List<String> GetClients()
        {
            return JavaScriptExecutor.FindElements(DashboardLogicRequestsDetailPageObjects.ActiveClientCssLocator, How.CssSelector, "Text");
        }

        public int GetClientsCount()
        {
            return SiteDriver.FindElementsCount(DashboardLogicRequestsDetailPageObjects.ClientsCountCssLocator, How.CssSelector);
        }


        public string GetEachGridRequestCount(int row, int column)
        {

            return SiteDriver.FindElement(string.Format(DashboardLogicRequestsDetailPageObjects.GridLogicRequestCountValueTemplateCssTemplate, row, column), How.CssSelector).Text;

        }

        public int GetSumofOverdueRequestsForAllClient()
        {
            int sum = 0;
            for (int i = 1; i <= GetClientsCount(); i++)
            {
                sum += Convert.ToInt32(GetEachGridRequestCount(i, 2));
            }
            return sum;
        }

        public int GetSumofDueIn30MinsRequestsForAllClient()
        {
            int sum = 0;
            for (int i = 1; i <= GetClientsCount(); i++)
            {
                sum += Convert.ToInt32(GetEachGridRequestCount(i, 3));
            }
            return sum;
        }

        public int GetSumofOpenRequestsForAllClient()
        {
            int sum = 0;
            for (int i = 1; i <= GetClientsCount(); i++)
            {
                sum += Convert.ToInt32(GetEachGridRequestCount(i, 4));
            }
            return sum;
        }

        public bool IsSubMenuDownloadPDFPresent()
        {
            return SiteDriver.IsElementPresent(DashboardLogicRequestsDetailPageObjects.DownloadPDFLinkXPath, How.XPath);
        }
        public bool IsSpinnerWrapperPresent()
        {
            return SiteDriver.IsElementPresent(DashboardLogicRequestsDetailPageObjects.SpinnerWrapperCssLocator, How.CssSelector);
        }


        public void ClickOnDownloadPDF()
        {
            JavaScriptExecutor.ExecuteClick(DashboardLogicRequestsDetailPageObjects.DownloadPDFLinkXPath, How.XPath);
            SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count == 1);
            Console.WriteLine("Clicked on Download PDF");

        }


        public bool DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage()
        {
            return SiteDriver.FindElement(DashboardPageObjects.NextRefreshTimeCssSelector, How.CssSelector).Text.Contains("Next Refresh :");
        }
        #endregion
    }
}
