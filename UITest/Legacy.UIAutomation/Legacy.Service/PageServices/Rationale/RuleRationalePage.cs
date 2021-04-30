
using Legacy.Service.PageObjects.Password;
using Legacy.Service.PageObjects.Rationale;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Rationale
{
    public class RuleRationalePage : DefaultPage
    {

        private readonly RationalePageObjects _rationalePage;

        public RuleRationalePage(INavigator navigator, RationalePageObjects rationalePage)
            : base(navigator, rationalePage)
        {
            // Just for performance!
            _rationalePage = (RationalePageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _rationalePage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}
