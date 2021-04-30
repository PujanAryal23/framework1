using System;
using System.Threading;
using NUnit.Framework;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace UIAutomation.Framework.Core.Navigation
{
    public class Navigator : INavigator
    {
        #region CONSTRUCTOR

        public Navigator(IBrowserOptions options)
        {
            _browserOptions = options;
        }


        #endregion

        #region PROPERTIES

        public string CurrentUrl
        {
            get { return SiteDriver.Url; }
        }

        #endregion

        #region PUBLIC METHODS

        public void Start(string siteUrl)
        {
            _siteUrl = siteUrl;
            SiteDriver.Init(_browserOptions);
            SiteDriver.BaseUrl = _browserOptions.ApplicationUrl;
            SiteDriver.Start();
            SiteDriver.MaximizeWindow();
        }

        public T Open<T>() where T : PageBase, new()
        {
            SiteDriver.Open(_siteUrl);

            WaitForPageToLoad();
            // AssertErrorPage();
            if (SiteDriver.Title.Contains("ORA") ||
                SiteDriver.Url.Contains("Connection request timed out"))
            {
                Console.WriteLine("Oracle Issue URL=" + SiteDriver.Url);
                Console.WriteLine("Oracle Issue Title=" + SiteDriver.Title);
                SiteDriver.Open(_siteUrl);
                WaitForPageToLoad();
            }
            var target = new T();

            AssertCorrectPageLoaded(target);

            return target;
        }

        public T Navigate<T>(Action action) where T : PageBase, new()
        {
            if (action != null)
                action();

            WaitForPageToLoad();
            // AssertErrorPage();

            var target = new T();
            AssertCorrectPageLoaded(target);

            return target;
        }

        public T Navigate<T>(Action action, Func<T> pageBase) where T : PageBase, new()
        {
            if (action != null)
                action();
            WaitForPageToLoad();
            var target = pageBase();
            AssertCorrectPageLoaded(target);
            return target;
        }

        public void Sleep(int t)
        {
            Thread.Sleep(t);
        }

        public void WaitForAjaxToLoad(Action action, string library)
        {
            action();
            SiteDriver.WaitForAjaxLoad(library);
        }

        public void WaitForCondition(Func<bool> f)
        {
            SiteDriver.WaitForCondition(f);
        }

        public bool IsAlertBoxPresent()
        {
            return SiteDriver.IsAlertBoxPresent();
        }

        //public T SwitchWindow<T>(string windowName) where T : PageBase, new()
        //{
        //    SiteDriver.SwitchWindow(windowName);
        //    WaitForPageToLoad();
        //    //AssertErrorPage();

        //    var target = new T();

        //    AssertCorrectPageLoaded(target);
        //    return target;
        //}

        public void CloseWindow(string windowName)
        {
            SiteDriver.CloseWindow(windowName);
        }

        public string GetWindow(int index)
        {
            return SiteDriver.WindowHandles[index];
        }

        public string GetAlertBoxText()
        {
            return SiteDriver.GetAlertBoxText();
        }

        public void AcceptAlertBox()
        {
            SiteDriver.AcceptAlertBox();
        }

        public void DismissAlertBox()
        {
            SiteDriver.DismissAlert();
        }

        public void Dispose()
        {
            SiteDriver.Stop();
        }

        public void SwitchFrame(string frameName)
        {
            SiteDriver.SwitchFrame(frameName);
        }

        public void CloseFrame(string frameName)
        {
            SiteDriver.CloseFrame(frameName);
        }

        public void Refresh()
        {
            SiteDriver.Reload();
        }

        public void Back()
        {
            SiteDriver.Back();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Waits for a new page to load.
        /// </summary>
        private void WaitForPageToLoad()
        {
            SiteDriver.WaitForPageToLoad();
        }

        /// <summary>
        /// Checks if there is any IIS error.
        /// </summary>
        private void AssertErrorPage()
        {
            var bodyText = SiteDriver.FindElement("body", How.TagName).Text;
            if (bodyText.Contains("Server Error in"))
            {
                Assert.Fail("Server error while navigating to \"{0}\"\r\n\r\n{1}.", SiteDriver.Url, bodyText);
            }
        }

        /// <summary>
        /// Asserts the correct page is loaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        private void AssertCorrectPageLoaded<T>(T target) where T : PageBase, new()
        {
            var currentUri = new Uri(SiteDriver.Url);
            var pageUri = new Uri(target.PageUrl);
            var currentLocation = String.Format("{0}{1}", currentUri.Host, currentUri.PathAndQuery);
            var pageLocation = String.Format("{0}{1}", pageUri.Host, pageUri.PathAndQuery);
            if (!currentLocation.StartsWith(pageLocation, StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail("Expected URL {0} but was {1}", pageLocation, currentLocation);
            }
        }

        public T SwitchWindow<T>(string windowName) where T : NewPageBase, new()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PRIVATE MEMBERS

        private readonly IBrowserOptions _browserOptions;
        private string _siteUrl;

        #endregion
    }
}