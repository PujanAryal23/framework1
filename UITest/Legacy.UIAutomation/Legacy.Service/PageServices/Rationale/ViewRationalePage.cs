using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Rationale;
using Legacy.Service.PageServices.Base;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using Legacy.Service.PageServices.Product;

namespace Legacy.Service.PageServices.Rationale
{
    public class ViewRationalePage : BasePageService
    {
        private ViewRationalePageObjects _viewRationalePage;

        public ViewRationalePage(INavigator navigator, ViewRationalePageObjects viewRationalePage)
            : base(navigator, viewRationalePage)
        {
            _viewRationalePage = (ViewRationalePageObjects)PageObject;
        }

        /// <summary>
        /// Get description value
        /// </summary>
        /// <returns></returns>
        public string GetDescriptionValue()
        {
            return SiteDriver.FindElement<TextLabel>(ViewRationalePageObjects.RuleDescId, How.Id).Text;
        }

        public ClaimSummaryPage CloseViewRationalePage(string windowHandle)
        {
            
            SiteDriver.CloseWindowAndSwitchTo(windowHandle);
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, new ClaimSummaryPageObjects());
        }
    }
}
