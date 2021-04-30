using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Negotiation;
using Legacy.Service.PageServices.Default;

namespace Legacy.Service.PageServices.Negotiation
{
  public class ClientMonitorPage : DefaultPage
  {
    private readonly ClientMonitorPageObjects _clientMonitorPage;

    public ClientMonitorPage(INavigator navigator, ClientMonitorPageObjects clientMonitorPage)
      : base(navigator, clientMonitorPage)
    {
      // Just for performance!
      _clientMonitorPage = (ClientMonitorPageObjects)PageObject;
    }

    public override IPageService GoBack()
    {
        var welcomePage = Navigator.Navigate<WelcomePageObjects>(() =>  _clientMonitorPage.BackBtn.Click());
      return new WelcomePage(Navigator, welcomePage);
    }
  }
}
