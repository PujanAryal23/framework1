using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderBillHistoryPage : BasePageService
    {
        #region PRIVATE FIELDS

        private ProviderBillHistoryPageObjects _providerBillHistory;

        #endregion

        #region CONSTRUCTOR

        public ProviderBillHistoryPage(INavigator navigator, ProviderBillHistoryPageObjects providerBillHistory)
            : base(navigator, providerBillHistory)
        {
            _providerBillHistory = (ProviderBillHistoryPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public void ScrollToLastColumn()
        {
            UIAutomation.Framework.Utils.JavaScriptExecutor.ExecuteToScrollToView(ProviderBillHistoryPageObjects.GridHeaderId, "42");
        }

        public bool IsExtraClaimNoColumnDisplayed()
        {
            return SiteDriver.FindElement(ProviderBillHistoryPageObjects.ExtraClaimNoColumnXPath, How.XPath).Displayed;
        }

        ///// <summary>
        ///// Close provider bill history
        ///// </summary>
        ///// <returns></returns>
        //public ProviderSearchPage CloseProviderBillHistoryAndSwitchToProviderSearch()
        //{
        //    var providerSearch = Navigator.Navigate<ProviderSearchPageObjects>(() =>
        //    {
        //        SiteDriver.CloseWindow();
        //        SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderSearch.GetStringValue());
        //    });
        //    return new ProviderSearchPage(Navigator, providerSearch);
        //}

     




        #endregion
    }
}
