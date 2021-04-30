using System;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
  public class WelcomePageAction : BasePageAction//, IDisposable
  {
    private readonly WelcomePage _welcomePage;

    public WelcomePageAction(INavigator navigator, WelcomePage welcomePage)
      : base(navigator, welcomePage)
     {
      // Just for performance!
      _welcomePage = (WelcomePage)_pageObject;
     }

    public PhysicianClaimInsightPageAction GoToPhysicianClaimInsight()
      {
          var pciPage = Navigator.Navigate<PhysicianClaimInsightPage>
         (() => _welcomePage.PhysicianClaimInsight.Click());
          return new PhysicianClaimInsightPageAction(Navigator, pciPage);
      }

      public FacilityClaimInsightPageAction GoToFacilityClaimInsight()
      {
          var fciPage = Navigator.Navigate<FacilityClaimInsightPage>
          (() => _welcomePage.FacilityClaimInsight.Click());
          return new FacilityClaimInsightPageAction(Navigator, fciPage);
      }

      public DentalClaimInsightPageAction GoToDentalClaimInsight()
      {
          var dciPage = Navigator.Navigate<DentalClaimInsightPage>
          (() => _welcomePage.DentalClaimInsight.Click());
          return new DentalClaimInsightPageAction(Navigator, dciPage);
      }
      
  }
}
