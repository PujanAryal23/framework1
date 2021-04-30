using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderScoreCardPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private readonly ProviderScoreCardPageObjects _providerScoreCard;

        #endregion

        #region CONSTRUCTOR

        public ProviderScoreCardPage(INewNavigator navigator, ProviderScoreCardPageObjects providerScoreCard, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager,
                IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            :  base(navigator, providerScoreCard, siteDriver, javaScriptExecutors, environmentManager, browserOptions,executor)
        { 
            _providerScoreCard = (ProviderScoreCardPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get score from score card
        /// </summary>
        /// <returns></returns>
        public string GetScoreFromScoreCardForSelectedProvider()
        {
            return SiteDriver.FindElement(ProviderScoreCardPageObjects.ScoreCardId, How.Id).Text;
        }

       

        ///// <summary>
        ///// Close provider score card and return to provider search page
        ///// </summary>
        ///// <param name="pageTitleEnum"></param>
        ///// <returns></returns>
        //public ProviderSearchPage CloseProviderScoreCardAndReturnToProviderSearch(PageTitleEnum pageTitleEnum)
        //{
        //    var providerSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
        //    {
        //        SiteDriver.CloseWindow();
        //        SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
        //    });
        //    return new ProviderSearchPage(Navigator, providerSearchPage);
        //}
        
        public ProviderActionPage CloseProviderScoreCardAndReturnToProviderAction(PageTitleEnum pageTitleEnum)
        {
            var newProviderActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
            });
            return new ProviderActionPage(Navigator, newProviderActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            );
        }
        
        public string GetProviderScorecardPageHeader()
        {
            return SiteDriver.FindElement(ProviderScoreCardPageObjects.pageHeaderCSSLocator, How.CssSelector).Text;
        }

        #endregion
    }
}
