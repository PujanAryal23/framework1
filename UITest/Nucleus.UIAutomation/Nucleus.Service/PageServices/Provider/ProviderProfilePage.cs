using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Provider;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using System;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderProfilePage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private ProviderProfilePageObjects _providerProfilePage;

        #endregion

        #region CONSTRUCTOR

        public ProviderProfilePage(INewNavigator navigator, ProviderProfilePageObjects providerProfilePage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager,
            IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerProfilePage, siteDriver, javaScriptExecutors, environmentManager, browserOptions,executor)
        {
            _providerProfilePage = (ProviderProfilePageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public int GetProviderExposureVisitCountValue()
        {
            return int.Parse(SiteDriver.FindElement(
               ProviderProfilePageObjects.ProviderExposureVisitCountValueCssLocator,
                How.CssSelector).Text);
        }

        public string GetProviderExposureAvgValue()
        {
            return SiteDriver.FindElement(
                ProviderProfilePageObjects.ProviderExposureVisitAvgValueCssLocator,
                How.CssSelector).Text;
        }

        public string GetProviderNameFromProviderProfileForSelectedProvider()
        {
            return SiteDriver.FindElement(ProviderProfilePageObjects.ProviderNameId, How.Id).Text;
        }

        public string GetScoreFromProviderProfileForSelectedProvider()
        {
            return SiteDriver.FindElement(ProviderProfilePageObjects.ScoreId, How.Id).Text;
        }

        public string[] GetStatsInfo(string tickerName)
        {
            string[] statsInfo = { "", "", "" };
            string tickerCode = null;
            switch (tickerName)
            {
                case ("patients"):
                    tickerCode = "patient_stat";
                    break;
                case ("visits"):
                    tickerCode = "visit_stat";
                    break;
                case ("proccodes"):
                    tickerCode = "proc_code_stat";
                    break;
                case ("icd9"):
                    tickerCode = "icd_9_stat";
                    break;
                case ("claims"):
                    tickerCode = "claim_stat";
                    break;
                case ("billed"):
                    tickerCode = "billed_stat";
                    break;
                case ("paid"):
                    tickerCode = "paid_stat";
                    break;
                case ("lines"):
                    tickerCode = "line_stat";
                    break;
                case ("savings"):
                    tickerCode = "savings_stat";
                    break;
            }
            statsInfo[0] = SiteDriver.FindElement(string.Format(ProviderProfilePageObjects.StatsInfoTemplate, tickerCode), How.XPath).Text;
            statsInfo[1] = SiteDriver.FindElement(string.Format(ProviderProfilePageObjects.StatScoreValueTemplate, tickerCode), How.XPath).Text;
            statsInfo[2] = SiteDriver.FindElement(string.Format(ProviderProfilePageObjects.StatsAverageTemplate, tickerCode), How.XPath).Text;
            return statsInfo;
        }

       

        //public ProviderSearchPage CloseProviderProfileAndSwitchToProviderSearch()
        //{
        //    var providerSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
        //    {
        //        SiteDriver.CloseWindow();
        //        SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderSearch.GetStringValue());
        //    });
        //    return new ProviderSearchPage(Navigator, providerSearchPage);
        //}
        
        public ProviderActionPage CloseProviderProfileAndSwitchToProviderAction()
        {
            var newProviderActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
            });
            return new ProviderActionPage(Navigator, newProviderActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public bool IsExClamationIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderProfilePageObjects.ExclamationProfileIndicatorCssSelector, How.CssSelector);

        }

        public string GetProviderProfileIconTooltip(int rowNo)
        {
            
            return SiteDriver.FindElement(string.Format(ProviderProfilePageObjects.ProfileIconOnlyXPathTemplate, rowNo),
                    How.XPath).GetAttribute("data-tooltiptitle");

        }
        public string GetProviderProfileReviewTooltip(int rowNo)
        {

            
            return SiteDriver.FindElement(string.Format(ProviderProfilePageObjects.ProfileIconReviewXPathTemplate, rowNo),
                    How.XPath).GetAttribute("data-tooltiptitle");

        }
        public string GetWidgetProviderProfileIconTooltip()
        {

            
            return SiteDriver.FindElement(ProviderProfilePageObjects.WidgetProfileIconCssLocator,
                    How.CssSelector).GetAttribute("data-tooltiptitle");

        }
        public string GetWidgetProviderProfileReviewTooltip()
        {
            

            return SiteDriver.FindElement(ProviderProfilePageObjects.WidgetProfileIconReviewCssLocator,
                    How.CssSelector).GetAttribute("data-tooltiptitle");

        }
        public string GetProfileIndicatorTitle()
        {
            
            return SiteDriver.FindElement(ProviderProfilePageObjects.ProfileIndicatorCssSelector,
                How.CssSelector).GetAttribute("title");
        }

        public string GetProfileReviewIndicatorTitle()
        {
            
            return SiteDriver.FindElement(ProviderProfilePageObjects.ProfileReviewIndicatorCssSelector,
                How.CssSelector).GetAttribute("title");
        }


        public ProviderClaimHistoryPage ClickOnHistoryIconToOpenHistoryPopup()
        {
            var providerClaimHistory = Navigator.Navigate<ProviderClaimHistoryPageObjects>(() =>
            {
                SiteDriver.FindElement(ProviderProfilePageObjects.HistoryIconLocatorXPathTemplate, How.XPath).Click();
                Console.WriteLine("Clicked on History Icon");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimHistoryPopup.GetStringValue()));
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderClaimHistoryPage(Navigator, providerClaimHistory,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public string GetPageHeader()
        {
            return SiteDriver.FindElement(ProviderProfilePageObjects.ProviderProfilePageHeaderXPath, How.XPath).Text;
        }
        #endregion

    }
}
