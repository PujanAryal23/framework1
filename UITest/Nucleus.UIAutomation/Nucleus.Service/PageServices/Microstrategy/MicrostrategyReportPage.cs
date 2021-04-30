using Nucleus.Service.PageObjects.Microstrategy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.MicroStrategy;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.PageServices.Microstrategy
{
    public class MicrostrategyReportPage : NewDefaultPage
    {

        #region PRIVATE
        private readonly MicrostrategyReportPageObjects _microstrategyReport;
        #endregion



        #region constructor
        public MicrostrategyReportPage(INewNavigator navigator, MicrostrategyReportPageObjects microreport, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor) : 
            base(navigator, microreport, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _microstrategyReport = (MicrostrategyReportPageObjects)PageObject;
        }

        #endregion


        #region PUBLIC

        public bool IsHomeButtonPresent()
        {
            return SiteDriver.IsElementPresent(MicrostrategyReportPageObjects.HomeButtonCssLocator, How.CssSelector);
        }

        public void SwitchToIframe()
        {
            SiteDriver.SwitchFrameById(MicrostrategyReportPageObjects.IframeId);
            Console.WriteLine(DateTime.Now);
            SiteDriver.WaitForCondition(IsHomeButtonPresent);
            Console.WriteLine(DateTime.Now);
            SiteDriver.WaitForCondition(IsHomeButtonPresent);
            Console.WriteLine(DateTime.Now);
            SiteDriver.WaitForCondition(IsHomeButtonPresent);
            Console.WriteLine(DateTime.Now);
            var text = SiteDriver
                .FindElement(MicrostrategyReportPageObjects.HomeButtonCssLocator, How.CssSelector).Text;
            Console.WriteLine(text);
            SiteDriver.SwitchBackToMainFrame();
        }

        public List<string> GetMSTRheadersLabel()
        {
            return JavaScriptExecutor.FindElements(MicrostrategyReportPageObjects.PageHeadersByCssLocator, How.CssSelector, "Text");
        }
        public string GetTitle()
        {
            return SiteDriver.Title;
        }

        public bool IsSelectProductOptionpresent()
        {

            // return SiteDriver.IsElementPresent(MicrostrategyReportPageObjects.ReportHomeBycss, UIAutomation.Framework.Common.How.CssSelector);

            return SiteDriver.IsElementPresent(MicrostrategyReportPageObjects.SelectProductBycss, How.CssSelector);

        }

        public MicrostrategyPage ClickOnMicrostrategyDashboardTONavigateToMicrostratergyPage()
        {
            var microstratergypage = Navigator.Navigate<MicroStrategyPageObjects>(() =>

            {

                JavaScriptExecutor.ExecuteClick(MicrostrategyReportPageObjects.MicrostrategyProduct, How.CssSelector);
                Console.WriteLine("Microstrategy");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DashboardPageObjects.DashboardLabelCssSelector, How.CssSelector));
            });
            return new MicrostrategyPage(Navigator, microstratergypage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }
        public bool IsMicrostrategyOptionPresentInDashboardMenu()
        {
            return JavaScriptExecutor.IsElementPresent(MicrostrategyReportPageObjects.MicrostrategyDashboardCssLocator);
        }




        #endregion

    }
}
