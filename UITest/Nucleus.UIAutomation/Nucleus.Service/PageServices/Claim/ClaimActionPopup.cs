using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Claim
{
    public class ClaimActionPopup : NewBasePageService
    {
        #region PRIVATE FIELDS

        private ClaimActionPopupObjects _claimActionPopup;
        private string _windowHandle;

        #endregion

        #region CONSTRUCTOR

        public ClaimActionPopup(INewNavigator navigator, ClaimActionPopupObjects claimActionPopup, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimActionPopup, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            //AssignUrl(claimSeq);
            _claimActionPopup = (ClaimActionPopupObjects)PageObject;
            _windowHandle = SiteDriver.CurrentWindowHandle;
        }
        #endregion

        #region PUBLIC METHODS

       

        public string GetClaimSequenceData()
        {
            return SiteDriver.FindElement(ClaimActionPopupObjects.ClaimSequenceDataId, How.Id).Text;
        }

        public ClaimActionPage CloseWindowAndSwitchToNewClaimAction(string windowHandle)
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(()=>
                                                {
                                                    SiteDriver.CloseWindow(_windowHandle);
                                                    SiteDriver.SwitchWindow(windowHandle);
                                                });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #endregion
    }
}
