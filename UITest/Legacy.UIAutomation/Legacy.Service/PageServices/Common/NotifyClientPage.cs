using System;
using Legacy.Service.PageObjects.Common;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Common
{
    public class NotifyClientPage : BasePageService
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private readonly NotifyClientPageObjects _notifyClientPage; 

        #endregion

        #region CONSTRUCTOR

        public NotifyClientPage(INavigator navigator, NotifyClientPageObjects notifyClientPage)
            : base(navigator, notifyClientPage)
        {
            _notifyClientPage = (NotifyClientPageObjects)PageObject;
        } 

        #endregion

        #region PUBLIC METHODS
        
        /// <summary>
        /// Get notifyclient window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public NotifyClientPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<NotifyClientPage>(_notifyClientPage, out handle);
        }

        /// <summary>
        /// Click close button of notify client page
        /// </summary>
        /// <returns></returns>
        public T ClickCloseButton<T>()
        {
            var target = typeof (T);
            object logicRequests = null;
           
            if(target.Equals(typeof(Product.LogicRequestsPage)))
                logicRequests =  Navigator.Navigate<PageObjects.Product.LogicRequestsPageObjects>(ClickCloseButtonAndSwitchToWindow);

            if (target.Equals(typeof(Pre_Authorizations.LogicRequestsPage)))
                logicRequests =Navigator.Navigate<PageObjects.Pre_Authorizations.LogicRequestsPageObjects>(ClickCloseButtonAndSwitchToWindow);

            return (T) Activator.CreateInstance(typeof (T), Navigator,logicRequests);
        }

        /// <summary>
        /// Click close button and switch to window by title.
        /// </summary>
        private void ClickCloseButtonAndSwitchToWindow()
        {
            _notifyClientPage.CloseButton.Click();
            Console.WriteLine("Clicked Close Button of Notify Client Page.");
            SiteDriver.SwitchWindowByTitle(string.Format(PageTitleEnum.LogicRequests.GetStringValue(),StartLegacy.Product.GetStringValue()));
        }

        #endregion
    }
}
