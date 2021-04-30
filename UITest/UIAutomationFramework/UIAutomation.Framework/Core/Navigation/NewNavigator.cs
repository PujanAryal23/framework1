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
    public class NewNavigator : INewNavigator
    {
        #region PRIVATE MEMBERS

        
        private string _siteUrl;
        private ISiteDriver _siteDriver;

        #endregion

        #region CONSTRUCTOR

        public NewNavigator(ISiteDriver siteDriver)
        {
            _siteDriver = siteDriver;
        }

        #endregion

        #region PROPERTIES
        public string CurrentUrl
        {
            get { return _siteDriver.Url; }
        }

        public ISiteDriver SiteDriver
        {
            get { return _siteDriver; }
        }

        #endregion

        #region PUBLIC METHODS

        public void Start(IBrowserOptions browserOptions)
        {
            _siteUrl = browserOptions.ApplicationUrl;
            _siteDriver.Init(browserOptions);
            _siteDriver.BaseUrl = browserOptions.ApplicationUrl;
            _siteDriver.Start();
            _siteDriver.MaximizeWindow();
        }

        public T Open<T>() where T : NewPageBase, new()
        {
            _siteDriver.Open(_siteUrl);

            WaitForPageToLoad();
            // AssertErrorPage();
            if (_siteDriver.Title.Contains("ORA") ||
                _siteDriver.Url.Contains("Connection request timed out"))
            {
                Console.WriteLine("Oracle Issue URL=" + _siteDriver.Url);
                Console.WriteLine("Oracle Issue Title=" + _siteDriver.Title);
                _siteDriver.Open(_siteUrl);
                WaitForPageToLoad();
            }
            var target = new T();
            target.SiteDriver = _siteDriver;
            target.SetPageBase();
            AssertCorrectPageLoaded(target);

            return target;
        }

        public T Navigate<T>(Action action) where T : NewPageBase, new()
        {
            if (action != null)
                action();
          
            WaitForPageToLoad();
           // AssertErrorPage();

            var target = new T();
            target.SiteDriver = _siteDriver;
            target.SetPageBase();
            AssertCorrectPageLoaded(target);

            return target;
        }

        public T Navigate<T>(Action action, Func<T> NewPageBase) where T : NewPageBase, new()
        {
            if (action != null)
                action();
            WaitForPageToLoad();
            var target = NewPageBase();
            target.SiteDriver = _siteDriver;
            target.SetPageBase();
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
            _siteDriver.WaitForAjaxLoad(library);
        }

        public void WaitForCondition(Func<bool> f)
        {
            _siteDriver.WaitForCondition(f);
        }

        public bool IsAlertBoxPresent()
        {
            return _siteDriver.IsAlertBoxPresent();
        }

        public T SwitchWindow<T>(string windowName) where T : NewPageBase, new()
        {
            _siteDriver.SwitchWindow(windowName);
            WaitForPageToLoad();
            //AssertErrorPage();

            var target = new T();

            AssertCorrectPageLoaded(target);
            return target;
        }

        public void CloseWindow(string windowName)
        {
            _siteDriver.CloseWindow(windowName);
        }

        public string GetWindow(int index)
        {
            return _siteDriver.WindowHandles[index];
        }

        public string GetAlertBoxText()
        {
            return _siteDriver.GetAlertBoxText();
        }

        public void AcceptAlertBox()
        {
            _siteDriver.AcceptAlertBox();
        }

        public void DismissAlertBox()
        {
            _siteDriver.DismissAlert();
        }

        public void Dispose()
        {
            _siteDriver.Stop();
        }

        public void SwitchFrame(string frameName)
        {
            _siteDriver.SwitchFrame(frameName);
        }

        public void CloseFrame(string frameName)
        {
            _siteDriver.CloseFrame(frameName);
        }

        public void Refresh()
        {
            _siteDriver.Reload();
        }

        public void Back()
        {
            _siteDriver.Back();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Waits for a new page to load.
        /// </summary>
        private void WaitForPageToLoad()
        {
            _siteDriver.WaitForPageToLoad();
        }

        /// <summary>
        /// Checks if there is any IIS error.
        /// </summary>
        private void AssertErrorPage()
        {
            var bodyText = _siteDriver.FindElement("body", How.TagName).Text;
            if (bodyText.Contains("Server Error in"))
            {
                Assert.Fail("Server error while navigating to \"{0}\"\r\n\r\n{1}.", _siteDriver.Url, bodyText);
            }
        }

        /// <summary>
        /// Asserts the correct page is loaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        private void AssertCorrectPageLoaded<T>(T target) where T : NewPageBase, new()
        {
            var currentUri = new Uri(_siteDriver.Url);
            var pageUri = new Uri(target.PageUrl);
            var currentLocation = String.Format("{0}{1}", currentUri.Host, currentUri.PathAndQuery);
            var pageLocation = String.Format("{0}{1}", pageUri.Host, pageUri.PathAndQuery);
            if (!currentLocation.StartsWith(pageLocation,StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail("Expected URL {0} but was {1}", pageLocation, currentLocation);
            }
        }

        #endregion

    }
}