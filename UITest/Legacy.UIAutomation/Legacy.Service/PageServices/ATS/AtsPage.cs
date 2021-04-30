using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.ATS;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.ATS
{
  public class AtsPage : DefaultPage
  {
    private readonly AtsPageObjects _atsPage;

    public AtsPage(INavigator navigator, AtsPageObjects atsPage)
      : base(navigator, atsPage)
    {
      // Just for performance!
      _atsPage = (AtsPageObjects)PageObject;
    }

    public override IPageService GoBack()
    {
      var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _atsPage.BackBtn.Click());
      return new WelcomePage(Navigator, welcomePage);
    }

    public AtsSearchPage GoToAtsSearch()
    {
      var atsSearchPage = Navigator.Navigate<AtsSearchPageObjects>(() => _atsPage.SearchBtn.Click());
      return new AtsSearchPage(Navigator, atsSearchPage);
    }
  }
}
