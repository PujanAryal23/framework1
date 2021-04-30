using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
  public class AtsPageAction : BasePageAction
  {
    private readonly AtsPage _atsPage;

    public AtsPageAction(INavigator navigator, AtsPage atsPage)
      : base(navigator, atsPage)
    {
      // Just for performance!
      _atsPage = (AtsPage)_pageObject;
    }

    public override IPageAction GoBack()
    {
      var welcomePage = Navigator.Navigate<WelcomePage>(() => _atsPage.BackBtn.Click());
      return new WelcomePageAction(Navigator, welcomePage);
    }

    public AtsSearchPageAction GoToAtsSearch()
    {
      var atsSearchPage = Navigator.Navigate<AtsSearchPage>(() => _atsPage.SearchBtn.Click());
      return new AtsSearchPageAction(Navigator, atsSearchPage);
    }
  }
}
