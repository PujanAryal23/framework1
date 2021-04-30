using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.Service.PageServices.Claim
{
    public class ClaimProcessingHistoryPage : NewBasePageService
    {

        private readonly ClaimProcessingHistoryPageObjects _claimProcessingHistoryPage;
      
        public ClaimProcessingHistoryPage(INewNavigator navigator,
            ClaimProcessingHistoryPageObjects claimProcessingHistoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimProcessingHistoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _claimProcessingHistoryPage = (ClaimProcessingHistoryPageObjects) PageObject;
        }

        public ClaimActionPage CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage()
        {

            var claimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                if (!SiteDriver.CurrentWindowHandle.Equals(Extensions.GetStringValue(PageTitleEnum.ClaimAction)))
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
            });
            return new ClaimActionPage(Navigator, claimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public ClaimSearchPage CloseClaimProcessingHistoryPageAndSwitchToClaimSearchPage()
        {

            var claimPage = Navigator.Navigate<ClaimSearchPageObjects>(() =>
            {
                if (!SiteDriver.CurrentWindowHandle.Equals(Extensions.GetStringValue(PageTitleEnum.ClaimAction)))
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.NewClaimSearch));
            });
            return new ClaimSearchPage(Navigator, claimPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public List<string> GetProcessingHistoryHeader()
        {
            return JavaScriptExecutor.FindElements(ClaimProcessingHistoryPageObjects.processinghistoryHeader,
                "Text");

        }

        public string GetReviewTime(string status)
        { return JavaScriptExecutor.FindElements(Format(ClaimProcessingHistoryPageObjects.ReviewTimeBycss))[1];
        }

        public List<string> GetClaimHistoryHeader()
        {
            var list= JavaScriptExecutor.FindElements(ClaimProcessingHistoryPageObjects.ClaimHistoryHeader,
                "Text");
            list.RemoveAt(list.Count-1);
            return list;
        }

        public List<string> GetProcessingHistoryData()
        {
            return JavaScriptExecutor.FindElements(ClaimProcessingHistoryPageObjects.ProcessingHistoryDataByCSS,
                "Text");
        }

        public List<string> GetClaimHistoryData(int col=1)
        {
            return JavaScriptExecutor.FindElements(
                Format(ClaimProcessingHistoryPageObjects.ClaimHistoryDataByCss, col), "Text");
        }

        public int GetClaimHistoryRowCount()
        {
        //public const string ClaimHistoryRowCountCssLocator = "table#ctl00_MainContentPlaceHolder_ClaimHistoryGrid_ctl00>tbody>tr";
            return SiteDriver.FindElementsCount(ClaimProcessingHistoryPageObjects.ClaimHistoryRowCountCssLocator,
                How.CssSelector);
        }

        public void  waitforReviewTime()
        {
            SiteDriver.WaitToLoadNew(180000);
        }

        

        #region DBinteraction



        public List<string> GetProcessingDataForClaim(string claseq, string clasub)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetProcessingDataForClaim, claseq,
                clasub));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];
        }

        public List<List<string>> GetClaimHistoryDataForClaim(string clientcode,string claseq, string clasub, bool isClientUser=false)
        {
            var infoList = new List<List<string>>();
            var condition = string.Empty;
            if (isClientUser)
                condition = "where u.user_type in (8) or (u.user_type = 128 and ca.status = 'X')";
            var list = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.GetClaimHistory, clientcode,claseq, clasub,condition));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList;
        }

        public string GetClaimHistoryTableDataValueByRowCol(int row, int col)
        {
            var t =
                SiteDriver.FindElement(
                    string.Format(ClaimProcessingHistoryPageObjects.ClaimHistoryTableCssTemplate, row, col),
                    How.CssSelector).Text;
            return t;
        }

        public string GetClaimHistoryLabel()
        {
            // return JavaScriptExecutor.FindElement(ClaimProcessingHistoryPageObjects.ProcessingHistoryLabeByCss).Text;
            return SiteDriver.FindElement(ClaimProcessingHistoryPageObjects.ClaimHistorylabelByCss,
                How.CssSelector).Text;

        }

        public string GetProcessingHistoryLabel()
        {
            return SiteDriver.FindElement(ClaimProcessingHistoryPageObjects.ProcessingHistoryLabeByCss,
                How.CssSelector).Text;

        }

        public bool IsProcessingHistorySectionPresent()
        {
            return SiteDriver.IsElementPresent(ClaimProcessingHistoryPageObjects.ProcessingHistoryLabeByCss,
                How.CssSelector);
        }

        public bool IsClaimHistorySectionPresent()
        {
            return SiteDriver.IsElementPresent(ClaimProcessingHistoryPageObjects.ClaimHistorylabelByCss,
                How.CssSelector);
        }

#endregion



    }

}
