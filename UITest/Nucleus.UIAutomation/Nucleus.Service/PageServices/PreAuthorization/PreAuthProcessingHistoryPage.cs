using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;


namespace Nucleus.Service.PageServices.PreAuthorization
{
    public class PreAuthProcessingHistoryPage : NewBasePageService
    {
        #region PRIVATE FIELDS 

        private readonly PreAuthProcessingHistoryPageObjects _preAuthProcessingHistoryPage;

        #endregion

        #region PUBLIC METHODS
        public string GetPageHeader()
        {
            return SiteDriver.FindElement(PreAuthProcessingHistoryPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public List<string> GetColumnHeaderList()
        {
            return JavaScriptExecutor.FindElements(PreAuthProcessingHistoryPageObjects.PreAuthHistoryColumnHeadersCssLocator,
                How.CssSelector, "Text");
        }

        public int GetRowCount()
        {
            return SiteDriver.FindElementsCount(PreAuthProcessingHistoryPageObjects.PreAuthHistoryRowCountCssLocator,
                How.CssSelector);
        }

        public string GetGridValueByRowCol(int row = 1, int col = 1)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthProcessingHistoryPageObjects.PreAuthHistoryGridValueByRowColCssLocator, row, col),
                How.CssSelector).Text;
        }

        public List<string> GetGridValueListByCol(int col = 1)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(PreAuthProcessingHistoryPageObjects.ListValueInGridByColumnCssTemplate, col),
                How.CssSelector,"Text");
        }

        public PreAuthActionPage CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage()
        {

            var preAuthActionPage = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                if (!SiteDriver.CurrentWindowHandle.Equals(Extensions.GetStringValue(PageTitleEnum.PreAuthorization)))
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.PreAuthorization));
            });
            return new PreAuthActionPage(Navigator, preAuthActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            );
        }

        #endregion

        #region CONSTRUCTOR

        public PreAuthProcessingHistoryPage(INewNavigator navigator,
            PreAuthProcessingHistoryPageObjects preAuthProcessingHistoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor) :
            base(navigator, preAuthProcessingHistoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor
            )
        {
            _preAuthProcessingHistoryPage = (PreAuthProcessingHistoryPageObjects) PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public List<string> GetPreAuthProcessingHistoryHeader()
        {
            return SiteDriver.FindElementsAndGetAttribute(
                "innerHTML", PreAuthProcessingHistoryPageObjects.PreAuthHistoryColumnHeadersCssLocator, How.CssSelector);
        }

        public string GetPreAuthProcessingHistoryModifiedDate(int row, int col)
        {
            var dateTime = SiteDriver
                .FindElement(
                    string.Format(PreAuthProcessingHistoryPageObjects.PreAuthHistoryGridValueByRowColCssLocator, row,
                        col), How.CssSelector).Text;
            DateTime date = Convert.ToDateTime(dateTime);
            return date.Date.ToString("MM/dd/yyyy");

        }

        public string GetPreAuthProcessingHistoryGridValueByRowCol(int row, int col)
        {
            return SiteDriver
                .FindElement(
                    string.Format(PreAuthProcessingHistoryPageObjects.PreAuthHistoryGridValueByRowColCssLocator, row,
                        col), How.CssSelector).Text;
        }

        public string GetPreAuthHistoryAuthSeq()
        {
            return SiteDriver
                .FindElement(PreAuthProcessingHistoryPageObjects.PreAuthHistoryAuthSeqCssLocator,
                    How.CssSelector).Text;
        }

        #endregion
    }
}
