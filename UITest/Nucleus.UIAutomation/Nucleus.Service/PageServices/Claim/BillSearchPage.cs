
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Claim
{
    public class BillSearchPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        #endregion

        #region CONSTRUCTOR

        public BillSearchPage(INewNavigator navigator, BillSearchPageObjects billSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, billSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            
        }
        #endregion

    
    }
}
