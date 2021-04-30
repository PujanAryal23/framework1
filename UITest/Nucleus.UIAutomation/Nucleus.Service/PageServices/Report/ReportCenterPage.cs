using Nucleus.Service.PageObjects.Report;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Report
{
    public class ReportCenterPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private ReportCenterPageObjects _reportCenterPage;

        #endregion

        #region PUBLIC PROPERTIES

        public ReportCenterPageObjects ReportCenter
        {
            get { return _reportCenterPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public ReportCenterPage(INewNavigator navigator, ReportCenterPageObjects reportCenterPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, reportCenterPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _reportCenterPage = (ReportCenterPageObjects)PageObject;
        }

        #endregion
    }
}
