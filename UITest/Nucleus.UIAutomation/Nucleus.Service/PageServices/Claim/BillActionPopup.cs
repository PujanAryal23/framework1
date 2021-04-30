
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Claim
{
    public class BillActionPopup : NewBasePageService
    { 
        #region PRIVATE FIELDS

        private BillActionPopupObjects _billActionPopup;

        #endregion

        #region CONSTRUCTOR

        public BillActionPopup(INewNavigator navigator, BillActionPopupObjects billActionPopup, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor
        )
            : base(navigator, billActionPopup, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _billActionPopup = (BillActionPopupObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS

        public string GetBillSequenceData()
        {
            return SiteDriver.FindElement(BillActionPopupObjects.BillSequenceDataId, How.Id).Text;
        }

        #endregion
    }
}
