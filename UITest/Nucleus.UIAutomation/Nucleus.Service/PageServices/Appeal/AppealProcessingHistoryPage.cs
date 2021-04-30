using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Appeal
{
   public class AppealProcessingHistoryPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private AppealProcessingHistoryPageObjects _appealProcessingHistoryPage;

        #endregion

        #region CONSTRUCTOR

        public AppealProcessingHistoryPage(INewNavigator navigator, AppealProcessingHistoryPageObjects appealProcessingHistoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealProcessingHistoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealProcessingHistoryPage =
               (AppealProcessingHistoryPageObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS

        public int CountWindowHandles(string title)
        {
            return SiteDriver.WindowHandles.Count(handle => SiteDriver.GetHandleByTitle(title).Equals(handle));
        }

        public AppealSummaryPage SwitchBackToAppealSummaryPage()
        {
            var newClaimActionPageObject = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
                StringFormatter.PrintMessage("Switch back to Appeal Summary Page.");
            });
            return new AppealSummaryPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public string GetPageHeader()
        {
            return SiteDriver.FindElement(AppealProcessingHistoryPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

       public string GetAppealAuditGridTableDataValue(int row, int col)
       {
           if(col==10)
               JavaScriptExecutor.ExecuteScrollToMostRight(AppealProcessingHistoryPageObjects.TblGridHeaderId);
           return
               SiteDriver.FindElement(
                   string.Format(AppealProcessingHistoryPageObjects.AppealAuditGridTableCssTemplate, row, col),
                   How.CssSelector).Text;
       }

        public List<string> GetAppealAuditGridTableDataValuesByColumn(int col)
        {
            return
                JavaScriptExecutor.FindElements(String.Format(AppealProcessingHistoryPageObjects.AppealAuditGridTableColumnXPath , col) ,
                    How.XPath, "Text");
        }




        public int GetResultGridRowCount()
       {
           return SiteDriver.FindElementsCount(AppealProcessingHistoryPageObjects.AppealAuditGridCountCssLocator, How.CssSelector);
       }

       public AppealActionPage CloseAppealProcessingHistoryPageBackToAppealActionPage()
       {

           var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
           {
               if (!SiteDriver.CurrentWindowHandle.Equals(Extensions.GetStringValue(PageTitleEnum.AppealAction)))
                   SiteDriver.CloseWindow();
               SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealAction));
           });
           return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
       }

       public AppealSummaryPage CloseAppealProcessingHistoryPageToAppealSummaryPage()
       {

           var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               if (!SiteDriver.CurrentWindowHandle.Equals(Extensions.GetStringValue(PageTitleEnum.AppealSummary)))
                   SiteDriver.CloseWindow();
               SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealSummary));
           });
           return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

       /// <summary>
       /// Close note popup page and back to appeal summary page
       /// </summary>
       public AppealSummaryPage CloseAppealProcessingHistoryAndBackToAppealSummary()
       {
           var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
           {
               SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealNotes.GetStringValue());
               SiteDriver.CloseWindow();
               SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
           });
           return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
       }

        public string GetInitialQADoneUser()
        {
            return SiteDriver
                .FindElement(AppealProcessingHistoryPageObjects.ModifiedByValueByAction, How.XPath).Text;
        }
        #endregion
    }
}
