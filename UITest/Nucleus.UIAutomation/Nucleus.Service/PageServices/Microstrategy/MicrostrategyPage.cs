using Nucleus.Service.PageObjects.MicroStrategy;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.MicroStrategy;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageObjects.Microstrategy;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.SqlScriptObjects.SwitchClient;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Microstrategy
{
    public class MicrostrategyPage : NewDefaultPage
    {
        #region PRIVATE FIELDS
        private readonly MicroStrategyPageObjects _microStrategyPage;
        private readonly GridViewSection _gridViewSection;
        private readonly SwitchClientSection _switchClientSection;
        #endregion


        #region CONSTRUCTOR

        public MicrostrategyPage(INewNavigator navigator, MicroStrategyPageObjects microstrategyPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, microstrategyPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor
            )
        {
            _microStrategyPage = (MicroStrategyPageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _switchClientSection = new SwitchClientSection(SiteDriver,JavaScriptExecutor);
        }



        #endregion

        #region SQL



        public List<string> GetReportNameToUserInfo(string userid)
        {
            return Executor.GetTableSingleColumn(String.Format(MicrostrategySqlScriptObjects.ReportNamesForUser, userid));

        }

        public int GetCountOfReportsAssigendtoUser(string userid)
        {
            return (int)Executor.GetSingleValue(String.Format(MicrostrategySqlScriptObjects.NumberOfReportsAssigned, userid));
        }

        public List<List<string>> GetReportInfoFromDatabase(string userid)
        {
            var infoList = new List<List<string>>();
            var ReprotInfo = Executor
                .GetCompleteTable(string.Format(MicrostrategySqlScriptObjects.ReportInfoForUser,
                    userid));
            foreach (DataRow row in ReprotInfo)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList;
        }

        public bool checkRolesToReportsMappingfromDatabase(string provaiderrelationrole, string qualityassurancerole)
        {
            return ((int)(Executor.GetSingleValue(string.Format(MicrostrategySqlScriptObjects.CountOfRolesmapped,
                         provaiderrelationrole, qualityassurancerole))) > 0);
        }

        public void UpdateReportToRoleMapping(string RoleValue1, string RoleValue2, char isActive = 'T')
        {
            Executor.ExecuteQuery(string.Format(MicrostrategySqlScriptObjects.UpdateToRemoveRoleMappedForReport, RoleValue1, RoleValue2, isActive));

        }

        #endregion
        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public SwitchClientSection SwitchClientSection
        {
            get { return _switchClientSection; }
        }

        public List<string> GetMstrHeadrsLabel()
        {
            return JavaScriptExecutor.FindElements(MicroStrategyPageObjects.PageHeadersByCssLocator, How.CssSelector, "Text");

        }
        public bool IsMstrNoDataMessageAvailable()
        {
            return SiteDriver.IsElementPresent(MicroStrategyPageObjects.MicrostratergyNoDataavailableByCss, How.CssSelector);
        }

        public string GetTitle()
        {
            return SiteDriver.Title;
        }
        public List<string> GetReportnameDisplayerPerUser()
        {
            return JavaScriptExecutor.FindElements(String.Format(MicroStrategyPageObjects.ReportValuesLocatorCssLocator, 2), How.CssSelector, "Text");

        }
        public string GetPageHeaderForMstrpage(int col)
        {
            return JavaScriptExecutor.FindElement(string.Format(MicroStrategyPageObjects.pageheaderlocatorBycss, col)).Text;


        }




        public MicrostrategyReportPage ClickOnViewReportsAndNavigatetohome()
        {
            var microstratergyHome = Navigator.Navigate<MicrostrategyReportPageObjects>(clickonReportName);
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DashboardPageObjects.DashboardLabelCssSelector, How.CssSelector));
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            return new MicrostrategyReportPage(Navigator, microstratergyHome, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public void clickonViewReport()
        {
            JavaScriptExecutor.ExecuteClick(MicroStrategyPageObjects.ViewReportsByXpath, How.XPath);

        }

        public void clickonReportName()
        {
            JavaScriptExecutor.ExecuteClick(MicroStrategyPageObjects.ReportLinkByCss, How.CssSelector);

        }

        public bool IsMicrostrategyOptionPresentInDashboardMenu()
        {

            return SiteDriver.IsElementPresent(MicroStrategyPageObjects.MicrostrategyDashboardXpath, How.XPath);
        }




    }
}
