using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Search;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Search
{
    public class SearchPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private SearchPageObjects _searchPage;

        #endregion

        #region CONSTRUCTOR

        public SearchPage(INavigator navigator, SearchPageObjects searchProductPageObjects)
            : base(navigator, searchProductPageObjects)
        {
            _searchPage = (SearchPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get search product window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SearchPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<SearchPage>(_searchPage, out handle);
        }




        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _searchPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }

       

        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
