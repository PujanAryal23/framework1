using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Provider
{
    public class NewProviderScoreCardPage : NewBasePageService
    {

        #region PRIVATE FIELDS

        private readonly NewProviderScoreCardPageObjects _providerScoreCard;

        #endregion

        #region CONSTRUCTOR

        public NewProviderScoreCardPage(INewNavigator navigator, NewProviderScoreCardPageObjects providerScoreCard, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerScoreCard, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerScoreCard = (NewProviderScoreCardPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get score from score card
        /// </summary>
        /// <returns></returns>
        //public string GetScoreFromScoreCardForSelectedProvider()
        //{
        //    return _providerScoreCard.ScoreCard.Text;
        //}



        /// <summary>
        /// Close provider score card and return to provider search page
        /// </summary>
        /// <param name="pageTitleEnum"></param>
        /// <returns></returns>
        ///
        
        public ProviderSearchPage CloseProviderScoreCardAndReturnToProviderSearch(PageTitleEnum pageTitleEnum)
        {
            var newProviderSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
            });
            return new ProviderSearchPage(Navigator, newProviderSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        /// <summary>
        /// Close provider score card current URL
        /// </summary>
        /// <param name="pageTitleEnum"></param>
        /// <returns></returns>
        public string GetScorecardPopUpCurrentUrl()
        {
            return SiteDriver.Url;
        }

        #endregion
    }
}

