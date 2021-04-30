using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
  public class ClientMonitorPageAction : BasePageAction
  {
    private readonly ClientMonitorPage _clientMonitorPage;

    public ClientMonitorPageAction(INavigator navigator, ClientMonitorPage clientMonitorPage)
      : base(navigator, clientMonitorPage)
    {
      // Just for performance!
      _clientMonitorPage = (ClientMonitorPage)_pageObject;
    }

    public override IPageAction GoBack()
    {
      var welcomePage = Navigator.Navigate<WelcomePage>(() => { _clientMonitorPage.BackBtn.Click(); });
      return new WelcomePageAction(Navigator, welcomePage);
    }
  }
}
