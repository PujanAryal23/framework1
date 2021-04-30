
using Legacy.Service.PageObjects.Password;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Password
{
    public class ChangePasswordPage : DefaultPage
    {

        private readonly ChangePasswordPageObjects _changePasswordPage ;

        public ChangePasswordPage(INavigator navigator, ChangePasswordPageObjects changePasswordPage)
            : base(navigator, changePasswordPage)
        {
            // Just for performance!
            _changePasswordPage = (ChangePasswordPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _changePasswordPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}
