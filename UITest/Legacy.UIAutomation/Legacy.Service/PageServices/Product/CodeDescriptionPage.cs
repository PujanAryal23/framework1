using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class CodeDescriptionPage : BasePageService
    {
        private CodeDescriptionPageObjects _codeDescriptionPage;

        public CodeDescriptionPage(INavigator navigator, CodeDescriptionPageObjects claimSummaryPage)
            : base(navigator, claimSummaryPage)
        {
            _codeDescriptionPage = (CodeDescriptionPageObjects)PageObject;
        }

        public string GetDescriptionValue()
        {
            return _codeDescriptionPage.CodeDescLabel.Text;
        }

        public ClaimSummaryPage CloseCodeDesc(string windowHandle)
        {
            SiteDriver.CloseWindowAndSwitchTo(windowHandle);
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, new ClaimSummaryPageObjects());
        }
    }
}
