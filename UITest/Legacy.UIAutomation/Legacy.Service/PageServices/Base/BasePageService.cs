using System;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Base
{
    public abstract class BasePageService : IPageService
    {
        private readonly INavigator _navigator;

        protected PageBase PageObject;
        protected string ProductTitle;

        protected BasePageService(INavigator navigator, PageBase page)
        {
            _navigator = navigator;
            ProductTitle = StartLegacy.Product.GetStringValue();
            PageObject = (PageBase)SiteDriver.InitializePageElement(page);
        }

        protected INavigator Navigator
        {
            get { return _navigator; }
        }

        public string BaseUrl
        {
            get { return SiteDriver.BaseUrl; }
        }

        public string CurrentPageUrl
        {
            get { return SiteDriver.Url; }
        }

        public string CurrentPageTitle
        {
            get { return SiteDriver.Title; }
        }

        public string PageUrl
        {
            get { return PageObject.PageUrl; }
        }

        public string PageTitle
        {
            get { return PageObject.PageTitle; }
        }

        protected T GetCurrentWindowHandle<T>(object pageObject, out string handle)
        {
            handle = SiteDriver.CurrentWindowHandle;
            return (T)Activator.CreateInstance(typeof(T), _navigator, pageObject);
        }

        /// <summary>
        /// Get current title from sitedriver
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use CurrentPageTitle property instead of this method.")]
        public string GetPageTitle()
        {
            return SiteDriver.Title;
        }

        [Obsolete("Use CurrentPageUrl property instead of this method.")]
        public string GetCurrentPageUrl()
        {
            return SiteDriver.Url;
        }


        public virtual IPageService GoBack()
        {
            throw new InvalidOperationException("Method 'GoBack' should have been called on this page!");
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return string.Equals(PageObject.ToString(), obj.ToString());
        }

        public bool IsPopupPresentWithHandleName(string handle)
        {
            return SiteDriver.IsHandlePresent(handle);
        }

        /// <summary>
        /// Close child popup handle 
        /// </summary>
        /// <param name="currentWindow"></param>
        /// <param name="originalWindow"></param>
        /// <returns></returns>
        public BasePageService ClosePopupAndSwitchToOriginalHandle(string currentWindow, string originalWindow = null)
        {
            SiteDriver.CloseWindow(currentWindow);
            SiteDriver.SwitchWindow(originalWindow ?? SiteDriver.WindowHandles[0]);
            return this;
        }
    }
}