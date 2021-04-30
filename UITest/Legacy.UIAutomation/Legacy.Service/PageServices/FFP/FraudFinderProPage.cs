using Legacy.Service.PageObjects.FFP;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageServices.Default;

namespace Legacy.Service.PageServices.FFP
{
    public class FraudFinderProPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private readonly FraudFinderProPageObjects _ffpPage;
        
        #endregion

        #region CONSTURCTORS

        public FraudFinderProPage(INavigator navigator, FraudFinderProPageObjects pciPage)
            : base(navigator, pciPage)
        {
            _ffpPage = (FraudFinderProPageObjects)PageObject;
        } 

        #endregion

        #region PUBLIC METHODS

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _ffpPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }

        public BatchListPage NavigateToBatchListPage()
        {
            var batchList = Navigator.Navigate<BatchListPageObjects>(() =>
            {
                _ffpPage.BatchListBtn.Click();
                
            });
            return new BatchListPage(Navigator, batchList);
        }
        #endregion
    }
}
