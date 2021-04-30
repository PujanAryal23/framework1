using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Elements;
using OpenQA.Selenium.Remote;
using System.IO;
using System.Configuration;
using UIAutomation.Framework.Utils;
using System.Text;
using OpenQA.Selenium.Edge;


namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Defines the class through which the user controls the browser.
    /// </summary>
    public class NewSiteDriver : ISiteDriver
    {
        #region PRIVATE FIELDS

        /// <summary>
        /// An <see cref="IBrowserOptions"/> instance of IBrowserOptions.
        /// </summary>
        private IBrowserOptions _browserOptions;

        /// <summary>
        /// An <see cref="WebDriverWait"/> instance of WebDriverWait for page load.
        /// </summary>
        public WebDriverWait _waitPageLoad;
        public WebDriverWait waitPageLoad
        {
            get
            {
              return   _waitPageLoad;
            }
        }
       

        /// <summary>
        /// An <see cref="WebDriverWait"/> instance of WebDriverWait for ajax load.
        /// </summary>
        private WebDriverWait _waitAjaxLoad;

        /// <summary>
        /// An <see cref="IJavaScriptExecutor"/> instance of IJavaScriptExecutors through which user can execute JavaScript.
        /// </summary>
        private IJavaScriptExecutor _je;

        /// <summary>
        /// An <see cref="IWebDriver"/> instance of IWebDriver through which user can controls the browser.
        /// </summary>
        /// <remarks>
        /// This is the primary webdriver in which user can performs all the tasks as in default webdriver.
        /// </remarks>
        private IWebDriver _primaryWebDriver;

        /// <summary>
        /// An <see cref="IWebDriver"/> instance of IWebDriver through which user can controls the browser.
        /// </summary>
        /// <remarks>
        /// This is the secondary and optional webdriver in which user can performs their tasks in multiple browser.
        /// </remarks>
        private IWebDriver _webdriver;

        public IWebDriver Webdriver
        {
            get { return _webdriver; }
        }


        /// <summary>
        /// An <see cref="DriverService"/> instance of DriverService.
        /// </summary>
        /// <remarks>
        /// Exposes the service provided by a native WebDriver server executable.
        /// </remarks>
        private DriverService _driverService;

        private DriverService _primaryDriverService;

        /// <summary>
        /// An <see cref="DesiredCapabilities"/> instance of DesiredCapabilities.
        /// </summary>
        /// <remarks>
        /// Class to Create the capabilities of the browser you require for <see cref="IWebDriver"/>. 
        /// If you wish to use default values use the  methods.
        /// </remarks>
        private DesiredCapabilities _capabilities;

        /// <summary>
        /// An <see cref="ScreenshotRemoteWebDriver"/> instance of ScreenshotRemoteWebDriver.
        /// </summary>
        private ScreenshotRemoteWebDriver _screenshotRemoteWebDriver;

        /// <summary>
        /// An <see cref="Uri"/> instance of Uri.
        /// </summary>
        /// <remarks>
        /// Provides an object representation of a uniform resource identifier (URI) and easy access to the part of URI.
        /// </remarks>
        private Uri _remoteAddress;

        /// <summary>
        /// Gets or sets the boolean value to check whether the <see cref="IBrowserOptions"/> IBrowserOptions is initialized or not. 
        /// </summary>
        /// <remarks>
        /// If IBrowserOptions is initilized the value will set to true else false. This handles the TypeInitializationException and 
        /// InvalidOperationException in the start of automation.
        /// </remarks>
        private bool IsInitalized { get; set; }

        /// <summary>
        /// Defines default location of executbles like chromedriver.exe and IEDriverServer.exe
        /// </summary>
        private const string PATH = "Executables";

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this window 
        /// that uniquely identifies it within this driver instance.
        /// </summary>
        public string CurrentWindowHandle
        {
            get { return _webdriver.CurrentWindowHandle; }
        }

        /// <summary>
        /// Get previous page title
        /// </summary>
        public string PreviousPageTitle { get; set; }

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        public string PageSource
        {
            get { return _webdriver.PageSource; }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        public string Title
        {
            get { return _webdriver.Title; }
        }

        /// <summary>
        /// Gets the URL the browser is currently displaying. 
        /// </summary>
        public string Url
        {
            get { return _webdriver.Url; }

        }

        public ICookieJar GetCookies
        {
            get { return _webdriver.Manage().Cookies; }

        }


        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        public List<string> WindowHandles
        {
            get { return _webdriver.WindowHandles.ToList(); }
        }

        /// <summary>
        /// Gets the JavaScriptExecutors
        /// </summary>
        public IJavaScriptExecutor JsExecutor
        {
            get { return _webdriver as IJavaScriptExecutor; }
        }

        public IWebDriver WebDriver
        {
            get { return _webdriver as IWebDriver; }
        }
        /// <summary>
        /// Gets the <see cref="ICapabilities"/> capabilities of the browser that you are going to use.
        /// </summary>
        public ICapabilities Capabilites
        {
            get { return _capabilities; }
        }

        /// <summary>
        /// Gets or sets base url of a page.
        /// </summary>
        public string BaseUrl
        {
            get;
            set;
        }

        public int SleepInterval { get { return _browserOptions.SleepInterval; } }

        public int ElementTimeOut { get { return _browserOptions.ElementLoadTimeout; } }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Initialize SiteDriver
        /// </summary>
        /// <param name="browserOptions"></param>
        public void Init(IBrowserOptions browserOptions)
        {
            _browserOptions = browserOptions;
            IsInitalized = true;
        }

        /// <summary>
        /// Starts the brower.
        /// </summary>
        public void Start()
        {
            if (!IsInitalized)
                throw new TypeInitializationException(typeof(SiteDriver).FullName,
                  new InvalidOperationException(string.Format("{0} not initialized.",
                    typeof(SiteDriver).FullName)));
            if (_webdriver == null)
            {
                _webdriver = LaunchDriver();
                return;
            }

            if (_primaryWebDriver == null && _webdriver.WindowHandles.Count > 0)
            {
                _primaryWebDriver = _webdriver;
            }
            _webdriver = LaunchDriver();
        }

        /// <summary>
        /// Opens a new web page in the current browser window.
        /// </summary>
        /// <param name="url"></param>
        public void Open(string url)
        {
            _webdriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Refresh the current brower window
        /// </summary>
        public void Reload()
        {
            _webdriver.Navigate().Refresh();
        }

        /// <summary>
        /// Move back a single entry in the browser's history.
        /// </summary>
        public void Back()
        {
            _webdriver.Navigate().Back();
        }

        /// <summary>
        /// Move forward in the brower's history.
        /// </summary>
        public void Forward()
        {
            _webdriver.Navigate().Forward();
        }

        /// <summary>
        /// Retrieves the message of a JavaScript alert.
        /// </summary>
        /// <returns>A value from alert box.</returns>
        public string GetAlertBoxText()
        {
            return _webdriver.SwitchTo().Alert().Text;
        }

        /// <summary>
        /// Checks if alert box is present
        /// </summary>
        /// <returns>A boolean value depends on the display of alert box.</returns>
        public bool IsAlertBoxPresent()
        {
            try
            {
                _webdriver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Accepts the alert. 
        /// </summary>
        public void AcceptAlertBox()
        {
            _webdriver.SwitchTo().Alert().Accept();
        }

        ///<summary>
        /// Dismiss Alert if present
        /// </summary>
        public void DismissAlert()
        {
            try
            {
                IAlert modalDialog = _webdriver.SwitchTo().Alert();
                modalDialog.Dismiss();
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Clicks this element
        /// </summary>
        public void ClickAndHold(string select, How selector)
        {
            var action = new Actions(_webdriver);
            var webElement = FindElement(select, selector, _webdriver);
            action.MoveToElement(webElement).Click();

            action.MoveToElement(webElement).ClickAndHold().Build().Perform();

        }

        /// <summary>
        /// Refreshes the current the page.
        /// </summary>
        public void Refresh()
        {
            _webdriver.Navigate().Refresh();
        }

        public string GetElementText(string select, How selector, int timeOut = 0)
        {
            try
            {
                return FindElement(select, selector, _webdriver, timeOut).Text;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return null;
            }
        }

        public bool IsElementDisplayed(string select, How selector, int timeOut = 0)
        {
            try
            {
                return FindElement(select, selector, _webdriver, timeOut).Displayed;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Verifies that the specified element is somewhere on the page.
        /// </summary>
        /// <param name="select">An lookup value</param>
        /// <param name="selector">An <see cref="How"/>object of lookup methods.</param>
        /// <returns>A boolean value to represent the value is present or not.</returns>
        public bool IsElementPresent(string select, How selector, int timeOut = 0)
        {
            try
            {
                FindElement(select, selector, _webdriver, timeOut);
                return true;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Checks whether the element is present or not.
        /// </summary>
        /// <typeparam name="T">A <see cref="Type"/> generic type.</typeparam>
        /// <param name="select">An lookup value</param>
        /// <param name="selector">An <see cref="How"/> object of lookup methods.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsElementPresent(string select, How selector, IElement context)
        {
            try
            {
                FindElement(select, selector, (IWebElement)context.BaseObject, 0);
                return true;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// A generic method, that finds the first <see cref="IWebElement"/>  in the page that matches with in a document.
        /// </summary>
        /// <param name="select">The element to be found.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public T FindElement<T>(string select, How selector, bool cache = false) where T : class, IElement
        {
            //WaitElementExists(select, selector, _webdriver);
            return (T)Activator.CreateInstance(typeof(T), cache, selector, select);
        }

        /// <summary>
        /// A generic method, that Finds the first <see cref="IWebElement"/>  in the page that matches with in a document.
        /// </summary>
        /// <param name="select">The element to be found.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="context">The base element through which its corresponding elements can be found.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public T FindElement<T>(string select, How selector, IElement context, bool cache = false) where T : class, IElement
        {
            //WaitElementExists(select, selector, _webdriver);
            return (T)Activator.CreateInstance(typeof(T), cache, selector, select, (IWebElement)context.BaseObject);
        }

        /// <summary>
        /// Waits for a new page to load.
        /// </summary>
        public void WaitForPageToLoad()
        {
            const string jScript = "return document.readyState;";
            _waitPageLoad.Until(d =>
            {
                try
                {
                    return (string)_je.ExecuteScript(jScript) == "complete";
                }
                catch (UnhandledAlertException)
                {
                    DismissAlert();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                    return false;
                }
            });
        }

        /// <summary>
        /// Waits for the correct page to load.
        /// </summary>
        /// <param name="pageTitle">A title of the page.</param>
        public void WaitForCorrectPageToLoad(string pageTitle)
        {
            string readyState = string.Empty;
            try
            {
                _waitPageLoad.Until((d) =>
                {
                    try
                    {
                        readyState =
                            _je.ExecuteScript(
                                "if (document.readyState) return document.readyState;").
                                ToString();
                        if (string.Compare(readyState, "complete",
                                           StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(Title, pageTitle, StringComparison.OrdinalIgnoreCase) ==
                            0)
                        {
                            if (string.Compare(PreviousPageTitle, Title, StringComparison.Ordinal) != 0)
                            {
                                Console.WriteLine("{0} page opened", Title);
                                PreviousPageTitle = Title;
                            }
                            return true;
                        }
                        return false;
                    }
                    catch (UnhandledAlertException)
                    {
                        DismissAlert();
                        return false;
                    }
                    catch (InvalidOperationException e)
                    {
                        //Window is no longer available
                        return e.Message.ToLower().Contains("unable to get browser");
                    }
                    catch (WebDriverException e)
                    {
                        //Browser is no longer available
                        return e.Message.ToLower().Contains("unable to connect");
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine(
                            "Expected to find a page title of <{0}>, but found <{1}>.",
                            pageTitle, Title);
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                if (string.Compare(readyState, "complete", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Console.Out.WriteLine("Expected to find a page title of <{0}>, but found <{1}>.",
                                                    pageTitle, Title);
                }
            }
        }

        /// <summary>
        /// Maximizes the window
        /// IE retains the last opened state
        /// Chrome uses ChromeOptions
        /// </summary>
        public void MaximizeWindow()
        {

            //Chrome throws error
            if (!_capabilities.BrowserName.Equals(BrowserConstants.Chrome, StringComparison.OrdinalIgnoreCase))
                _webdriver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Checks ajax state according to the library script used.
        /// </summary>
        /// <param name="library"></param>
        public void WaitForAjaxLoad(string library)
        {
            string jScript = "";
            switch (library)
            {
                case ScriptConstants.Jquery:
                    jScript = "return Boolean($.active);";
                    break;
                case ScriptConstants.Extjs:
                    jScript = "return Ext.Ajax.isLoading();";
                    break;
                case ScriptConstants.Telerik:
                    jScript = "return $telerik.radControls[0]._manager._isRequestInProgress;";
                    break;
            }

            _waitAjaxLoad.Until(d =>
            {
                try
                {
                    return (bool)_je.ExecuteScript(jScript) == false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Waits until a condition is true or times out.
        /// </summary>
        /// <param name="f"></param>
        public void WaitForCondition(Func<bool> f, int milliSec = 0)
        {

            milliSec = (int)((milliSec == 0) ? _browserOptions.PageLoadTimeout * 1000 : milliSec);
            var wait = new WebDriverWait(_webdriver, TimeSpan.FromMilliseconds(milliSec));
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        return f();
                    }
                    catch (Exception)
                    {
                        DismissAlert();
                        return false;
                    }
                });
            }
            catch (UnhandledAlertException ex)
            {
                Console.Out.WriteLine("unhandled exception" + ex.Message);
            }

            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

      

        /// <summary>
        /// Waits until a condition is true or times out.
        /// </summary>
      
        /// <summary>
        /// Instructs the driver to send future commands to a different alert window
        /// </summary>
        /// <param name="alertName">Alertname</param>
        public void AcceptAlert(string alertName)
        {
            _webdriver.SwitchTo().Alert().Accept();
        }

        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window. 
        /// </summary>
        /// <param name="windowName">A window name.</param>
        public void SwitchWindow(string windowName)
        {
            _webdriver.SwitchTo().Window(windowName);
        }

        /// <summary>
        /// Switch window by  window title
        /// </summary>
        /// <param name="windowTitle">Title of the window</param>
        /// <returns>Return true if switchHandle</returns>
        public bool SwitchWindowByTitle(string windowTitle)
        {
            foreach (string handle in _webdriver.WindowHandles)
            {
                if (windowTitle.Equals(_webdriver.SwitchTo().Window(handle).Title))
                    return true;
            }
            return false;
        }

        public bool SwitchDynamicWindowByTitle(string windowTitle)
        {
            foreach (string handle in _webdriver.WindowHandles)
            {
                if (_webdriver.SwitchTo().Window(handle).Title.Contains(windowTitle))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Switch window by  window Url
        /// </summary>
        /// <returns>Return true if switchHandle</returns>
        public bool SwitchWindowByUrl(string windowUrl)
        {
            foreach (string handle in _webdriver.WindowHandles)
            {

                if (_webdriver.SwitchTo().Window(handle).Url.Contains(windowUrl))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window. 
        /// </summary>
        public void SwitchToActiveElement()
        {
            _webdriver.SwitchTo().ActiveElement();
        }
        /// <summary>
        /// returns the currently focused webelement
        /// </summary>
        /// <returns></returns>
        public IWebElement ReturnActiveElement()
        {
            return _webdriver.SwitchTo().ActiveElement();
        }
        /// <summary>
        /// Instructs the driver to close the selected window
        /// </summary>
        /// <param name="windowName"></param>
        public void CloseWindow(string windowName)
        {
            //Ensure to switch to the closing window and close it.
            _webdriver.SwitchTo().Window(windowName).Close();
        }

        /// <summary>
        /// Instructs the driver to close the selected window
        /// </summary>
        public void CloseWindow()
        {
            //Ensure to switch to the closing window and close it.
            _webdriver.Close();
        }

        /// <summary>
        /// Instructs the driver to close the selected window and switch to original window
        /// </summary>
        /// <param name="windowName"></param>
        public bool CloseWindowAndSwitchTo(string windowName)
        {
            if (!_webdriver.CurrentWindowHandle.Equals(windowName))
            {
                _webdriver.Close();
                _webdriver.SwitchTo().Window(windowName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Switches to a frame.
        /// </summary>
        /// <param name="frameName">A frame name of the page.</param>
        public void SwitchFrame(string frameName)
        {
            _webdriver.SwitchTo().Frame(frameName);
        }

        public void SwitchFrameById(string frameName)
        {
            IWebElement targetFrame = WaitandReturnElementExists(By.Id(frameName), _webdriver, 5000);
            WaitToLoadNew(200);
            _webdriver.SwitchTo().Frame(targetFrame);
        }

        public void SwitchFrameByClass(string className)
        {
            IWebElement targetFrame = WaitandReturnElementExists(By.XPath("//iframe[@class='" + className + "']"), _webdriver, 5000);// _webdriver.FindElement(By.XPath("//iframe[@class='" + className + "']"));
            WaitToLoadNew(200);
            _webdriver.SwitchTo().Frame(targetFrame);
        }

        public void SwitchFrameByCssLocator(string cssLocator)
        {
            IWebElement targetFrame = WaitandReturnElementExists(By.CssSelector(cssLocator), _webdriver, 5000);// _webdriver.FindElement(By.CssSelector(cssLocator));
            WaitToLoadNew(200);
            _webdriver.SwitchTo().Frame(targetFrame);
        }

        public void SwitchFrameByXPath(string xpath)
        {
            IWebElement targetFrame = WaitandReturnElementExists(By.XPath(xpath), _webdriver, 5000);// _webdriver.FindElement(By.XPath(xpath));
            WaitToLoadNew(200);
            _webdriver.SwitchTo().Frame(targetFrame);
        }
        //public void SwitchFrameByJQuery(string cssLocator)
        //{
        //    var wait = new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval));
        //    IWebElement webElement = null;
        //    wait.Until(driver =>
        //    {
        //        try
        //        {
        //            webElement = JavaScriptExecutor.FindElement(cssLocator);
        //            return webElement != null;

        //        }
        //        catch (Exception ex)
        //        {
        //            //Console.Out.WriteLine("unhandled exception" + ex.Message);
        //            return false;
        //        }
        //    });
        //    WaitToLoadNew(200);
        //    _webdriver.SwitchTo().Frame(webElement);
        //}

        public WebDriverWait WaitTime
        {
            get { return new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval)); }
        }

        public void WebdriverSwitchTo(IWebElement element)
        {
            _webdriver.SwitchTo().Frame(element);
        }

        public void SwitchBackToMainFrame()
        {
            _webdriver.SwitchTo().DefaultContent();
        }
        /// <summary>
        /// Switches to an active element and keys.excape to close the modal popup
        /// </summary>
        public void CloseModalPopup()
        {
            _webdriver.SwitchTo().ActiveElement().SendKeys(Keys.Escape);
            //JsExecutor.ExecuteScript("window.confirm = function(msg) { return true; }");
        }

        /// <summary>
        /// Instructs the driver to close the selected frame and switch to another frame
        /// </summary>
        /// <param name="frameName"></param>
        public void CloseFrameAndSwitchTo(String frameName)
        {
            _webdriver.SwitchTo().DefaultContent().SwitchTo().Frame(frameName);
        }

        /// <summary>
        /// Closes current page frame.
        /// </summary>
        /// <param name="frameName">A frame of the page.</param>
        public void CloseFrame(string frameName)
        {
            JsExecutor.ExecuteScript("window.close();");
            //JavaScriptExecutor.WindowClose();
            //_webdriver.SwitchTo().Frame(frameName).Close();
        }

        /// <summary>
        /// Quits the web driver instance.
        /// </summary>
        public void Stop()
        {
            if (_webdriver != null)
            {
                _webdriver.Quit();
                _webdriver = _primaryWebDriver;
                if (_driverService != null)
                {
                    _driverService.Dispose();
                    _driverService = null;
                }
            }

            //if (_primaryWebDriver != null)
            //{
            //    _primaryWebDriver.Quit();
            //    _primaryWebDriver = null;
            //    Console.WriteLine("PRimary");
            //    return;
            //}
        }

        /// <summary>
        /// Executes mover over.
        /// </summary>
        /// <param name="select">The locating element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="timeout">Waits for specific time.</param>
        public void MouseOver(string select, How selector, int timeout = 2000, bool release = false)
        {
            var action = new Actions(_webdriver);
            var webElement = FindElement(select, selector, _webdriver);
            if (release)
                action.MoveToElement(webElement).ClickAndHold().Release().Build().Perform();
            else
                action.MoveToElement(webElement).ClickAndHold().Build().Perform();

            //WaitToLoadNew(timeout);
        }

        /// <summary>
        /// Executes mouse right click.
        /// </summary>
        /// <param name="select">The locating element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public void RightClick(string select, How selector)
        {
            var action = new Actions(_webdriver);
            action.ContextClick(FindElement(select, selector, _webdriver)).Build().Perform();
        }

        /// <summary>
        /// Initialize the page elements.
        /// </summary>
        /// <param name="page">The page object to initialize.</param>
        /// <returns>The page object i.e initialized.</returns>
        public object InitializePageElement(object page)
        {
            return PageFactory.InitElements(page);
        }

        /// <summary>
        /// Waits until ajax request completed.
        /// </summary>
        /// <param name="ajaxLibrary">A <see cref="ScriptConstants"/>.</param>
        public void WaitForAjaxToLoad(string ajaxLibrary)
        {
            string script = string.Format("return {0};", ajaxLibrary);
            int count = 0;
            _waitAjaxLoad.Until((d) =>
            {
                try
                {

                    if (count > 1)
                        WaitToLoadNew(1000);
                    count++;
                    var result = ((bool)_je.ExecuteScript(script) == false);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Suspends the current thread for specified time.
        /// </summary>
        /// <param name="millisecondsTimeout">A specific time in milli seconds.</param>
        public void WaitToLoadNew(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        public void WaitForIe(int millisecondsTimeout)
        {
            if (string.Compare(BrowserConstants.Iexplorer, ConfigurationManager.AppSettings["TestBrowser"].ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) == 0)
                WaitToLoadNew(0);
        }

        /// <summary>
        /// Click and wait more time on exception
        /// </summary>
        /// <param name="select">The locating element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="waitTime"> wait time</param>
        public void ClickAndWaitMoreOnException(string select, How selector, int waitTime)
        {
            try
            {
                FindElement(select, selector, _webdriver).Click();
            }
            catch (WebDriverException ex)
            {
                WaitToLoadNew(waitTime);
            }
        }




        /// <summary>
        /// Captures an object representing the image of the page on the screen.
        /// </summary>
        /// <param name="name">A name to reproduce a screenshot filename.</param>
        public void CaptureScreenShot(string name)
        {
            _screenshotRemoteWebDriver.CaptureScreenShot(name);
        }

        /// <summary>
        /// Captures an object representing the image of the page on the screen.
        /// </summary>
        /// <param name="dirPath">A path of a directory in which a screenshot file stored.</param>
        /// <param name="name">A name to reproduce a screenshot filename.</param>
        public void CaptureScreenShot(string dirPath, string name)
        {
            _screenshotRemoteWebDriver.CaptureScreenShot(dirPath, name);
        }

        /// <summary>
        /// Gets a screenshot filename.
        /// </summary>
        /// <returns>A screenshot file name.</returns>
        public string GetScreenshotFilename()
        {
            return ScreenshotRemoteWebDriver.FullQualifiedFileName;
        }

        /// <summary>
        /// Gets a handle by its page title.
        /// </summary>
        /// <param name="title">A title of the current page.</param>
        /// <returns>A handles to be used.</returns>
        public string GetHandleByTitle(string title)
        {
            string currentHandle = _webdriver.CurrentWindowHandle;
            foreach (string handle in _webdriver.WindowHandles)
            {
                if (title.Equals(_webdriver.SwitchTo().Window(handle).Title))
                {
                    return handle;
                }
            }
            return currentHandle;
        }

        /// <summary>
        /// Return true if handle is present for title
        /// </summary>
        /// <param name="argHandle"></param>
        /// <returns></returns>
        public bool IsHandlePresent(string argHandle)
        {
            return _webdriver.WindowHandles.Contains(argHandle);
        }

        /// <summary>
        /// Is url valid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsUrlValid(string url)
        {
            int statusCode = Convert.ToInt16(GetHttpStatusCode(url));
            Console.WriteLine("Http status Code for url {0} : {1}", url, statusCode);
            return (statusCode == 200);
        }



        /// <summary>
        /// Ask agent to detect local file , for file upload
        /// </summary>
        public void LocalFileDetect()
        {
            IAllowsFileDetection allowsDetection = _webdriver as IAllowsFileDetection;
            if (allowsDetection != null)
            {
                allowsDetection.FileDetector = new LocalFileDetector();
            }
        }
        #endregion

        #region FINDELEMENTS

        //TODO: Need to figure out another way around
        /// <summary>
        /// A generic method, that finds the multiple <see cref="IWebElement"/>  in the page that matches with in a document.
        /// </summary>
        ////public List<string> FindElements(string select, How selector, string selectSelector, bool isNull = false)
        //{
        //    if (FindElementsCount(select, selector) > 15)
        //        return JavaScriptExecutor.FindElements(select, selectSelector, isNull, selector);
        //    var data = new List<string>();
        //    if (isNull)
        //        return JavaScriptExecutor.FindElements(select, selector, _webdriver).Select(element => element.Text).ToList();
        //    if (selectSelector.Equals("Text"))
        //        return FindElements(select, selector, _webdriver).Where(x => x.Displayed).Select(element => element.Text).ToList();

        //    if (selectSelector.StartsWith("Attribute"))
        //        return
        //            FindElements(select, selector, _webdriver)
        //                .Where(x => x.Displayed)
        //                .Select(element => element.GetAttribute(selectSelector.Replace("Attribute:", "")))
        //                .ToList();
        //data.Add(FindElements(select, selector, _webdriver).Where(x => x.Displayed).First().Text.Trim());
        //    return data;


        //}

    public IEnumerable<IWebElement> FindElements(string select, How selector)
        {
            return FindElements(select, selector, _webdriver);
        }

        public void FindElementAndChecked(string select, How selector)
        {
            foreach (var checkBox in FindElements(select, selector, _webdriver).Where(x => x.Enabled && x.GetAttribute("checked") != "true"))
            {
                checkBox.Click();
            }
        }

        public void FindElementAndSetText(string select, How selector, string reasonCode)
        {
            foreach (var dropDownField in FindElements(select, selector, _webdriver).Where(x => x.Enabled))
            {
                dropDownField.SendKeys(reasonCode);
            }
        }

        public List<string> FindDisplayedElementsText(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Where(e => e.Displayed).Select(e => e.Text).ToList();
        }

        public string FindElementAndGetAttribute(string select, How selector, int index)
        {
            return FindElements(select, selector, _webdriver).ToList()[index].GetAttribute("id");
        }

        public string FindElementAndGetAttribute(String select, How selector, string attribute = "id")
        {
            return FindElements(select, selector, _webdriver).Last().GetAttribute(attribute);
        }

        public List<string> FindElementAndGetNotNullIdAttribute(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Select(x => x.GetAttribute("id")).Where(
                x => x != string.Empty).ToList();
        }

        public List<bool> FindElementAndGetCondition(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Select(conditionActionCheckState => conditionActionCheckState.Selected).ToList();
        }

        public bool FindElementAndCheckCondition(string select, How selector, out int index)
        {
            index = 0;
            var appealLines = FindElements(select, selector, _webdriver).ToList();
            foreach (var appealLine in appealLines)
            {
                if (!appealLine.Selected)
                {
                    index = appealLines.IndexOf(appealLine) + 1;
                    return false;
                }
            }
            return true;
        }

        public List<string> FindElementAndGetIdAttribute(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Select(x => x.GetAttribute("id")).ToList();
        }
        public List<string> FindElementAndGetAttributeByAttributeName(string select, How selector, string attributeName)
        {
            return FindElements(select, selector, _webdriver).Select(x => x.GetAttribute(attributeName)).ToList();
        }

        public int FindElementsCount(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Count();
        }

        public List<string> FindElementAndGetClassAttribute(string select, How selector)
        {
            return FindElements(select, selector, _webdriver).Select(x => x.GetAttribute("class")).ToList();
        }


        public List<bool> FindElementAndGetCheckedAttribute(string select, How selector)
        {
            List<bool> result = new List<bool>();
            foreach (var check in FindElements(select, selector, _webdriver).Where(x => x.Enabled))
            {
                result.Add(check.GetAttribute("class").Contains("active"));
            }
            return result;
        }

        public List<bool> FindElementAndGetActiveAttribute(string select, How selector)
        {
            List<bool> result = new List<bool>();
            foreach (var check in FindElements(select, selector, _webdriver).Where(x => x.Enabled))
            {
                result.Add(check.GetAttribute("class").Contains("is_active"));
            }
            return result;
        }

        public List<bool> FindElementsAndGetAttributeByClass(string className, string select, How selector)
        {
            List<bool> result = new List<bool>();
            foreach (var element in FindElements(select, selector, _webdriver))
            {
                result.Add(element.GetAttribute("class").Contains(className));
            }
            return result;
        }

        public List<bool> FindElementsAndGetAttributeByTwoClass(string className1, string className2, string select, How selector)
        {
            List<bool> result = new List<bool>();
            foreach (var element in FindElements(select, selector, _webdriver))
            {
                result.Add(element.GetAttribute("class").Contains(className1) && element.GetAttribute("class").Contains(className2));
            }
            return result;
        }

        public List<string> FindElementsAndGetAttribute(string attribute, string select, How selector)
        {
            List<string> result = new List<string>();
            foreach (var element in FindElements(select, selector, _webdriver))
            {
                result.Add(element.GetAttribute(attribute));
            }
            return result;
        }

        public void FindElementAndClickOnCheckBox(string select, How selector)  //for checbox
        {
            foreach (var checkBox in FindElements(select, selector, _webdriver).Where(x => x.Enabled))
            {
                checkBox.Click();
            }
        }

        public void SwitchToLastWindow()
        {
            _webdriver.SwitchTo().Window(_webdriver.WindowHandles.Last());
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Get http status code
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpStatusCode GetHttpStatusCode(string url)
        {
            string address = ConfigurationManager.AppSettings["ProxyAddress"];
            string port = ConfigurationManager.AppSettings["ProxyPort"];
            string userName = ConfigurationManager.AppSettings["ProxyUserName"];
            string password = ConfigurationManager.AppSettings["ProxyPassword"];
            string domain = ConfigurationManager.AppSettings["ProxyDomain"];

            var request = (HttpWebRequest)WebRequest.Create(url);
            if (userName != null && password != null)
            {
                request.Proxy = new WebProxy(address + ":" + port, true)
                {
                    Credentials = new NetworkCredential(userName, password, domain),
                };
            }
            request.AllowAutoRedirect = true;
            request.Method = "HEAD";
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null) return response.StatusCode;
                    return HttpStatusCode.NotFound;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return HttpStatusCode.NotFound;
            }
        }

        /// <summary>
        /// Instantiates a <see cref="ScreenshotRemoteWebDriver"/>.
        /// </summary>
        /// <returns>A <see cref="IWebDriver"/></returns>
        private IWebDriver LaunchDriver()
        {
            string testServer = ConfigurationManager.AppSettings["TestServer"];
            string testPort = ConfigurationManager.AppSettings["TestPort"];
            string address = ConfigurationManager.AppSettings["ProxyAddress"];
            string port = ConfigurationManager.AppSettings["ProxyPort"];
            string userName = ConfigurationManager.AppSettings["ProxyUserName"];
            string password = ConfigurationManager.AppSettings["ProxyPassword"];
            string domain = ConfigurationManager.AppSettings["ProxyDomain"];
            string incognito = ConfigurationManager.AppSettings["InCognito"];

            var strBuilder = new StringBuilder();
            string requestUri = !string.IsNullOrEmpty(testServer) && !string.IsNullOrEmpty(testPort)
                                    ? strBuilder.Append("http://")
                                          .Append(testServer)
                                          .Append(":")
                                          .Append(testPort)
                                          .Append("/").ToString()
                                    : string.Empty;

            _remoteAddress = !string.IsNullOrEmpty(requestUri) ? new Uri(strBuilder
            .Append("wd")
            .Append("/")
            .Append("hub").ToString()) : null;

            if (!string.IsNullOrEmpty(requestUri) && !string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(domain))
            {
                try
                {
                    var webProxy = new WebProxy(address + ":" + port, true)
                    {
                        Credentials = new NetworkCredential(userName, password, domain),
                    };
                    var webRequest = WebRequest.Create(new Uri(requestUri));
                    webRequest.Proxy = webProxy;
                    webRequest.PreAuthenticate = true;

                    using (var httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        if (httpWebResponse != null)
                        {
                            httpWebResponse.GetResponseStream();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("No response from server.", ex.Message);
                }
            }

            switch (_browserOptions.Browser)
            {
                case BrowserConstants.Firefox:
                    _webdriver = LaunchFirefox();
                    break;
                case BrowserConstants.Iexplorer:
                    _webdriver = LaunchInternetExplorer();
                    break;
                case BrowserConstants.Chrome:
                    _webdriver = LaunchChrome(incognito);
                    break;
                case BrowserConstants.Edge:
                    _webdriver = LaunchEdge();
                    break;
                default:
                    throw new NotSupportedException(String.Format("Browser {0} is not supported.", _browserOptions.Browser));
            }

            _waitPageLoad = new WebDriverWait(_webdriver, TimeSpan.FromSeconds(_browserOptions.PageLoadTimeout));
            _waitAjaxLoad = new WebDriverWait(_webdriver, TimeSpan.FromSeconds(_browserOptions.AjaxLoadTimeout));

            _je = (IJavaScriptExecutor)_webdriver;

            return _webdriver;
        }

        /// <summary>
        /// Instantiates a <see cref="FirefoxDriver"/>.
        /// </summary>
        /// <returns>A <see cref="IWebDriver"/></returns>
        private IWebDriver LaunchFirefox()
        {
            var fp = new FirefoxProfile
            {
                AcceptUntrustedCertificates = true
            };
            fp.SetPreference("browser.download.folderList", 2);
            fp.SetPreference("browser.download.useDownloadDir", true);
           // fp.SetPreference("browser.download.manager.showWhenStarting", false);
            fp.SetPreference("browser.download.dir", System.IO.Directory.GetCurrentDirectory());
            fp.SetPreference("browser.helperApps.neverAsk.saveToDisk",
                             "text/html,application/xml,application/pdf,application/octet-stream");

            var options = new FirefoxOptions();

            _capabilities = options.ToCapabilities() as DesiredCapabilities;
            _capabilities.SetCapability("firefox_profile", fp.ToBase64String());
            _capabilities.SetCapability("enableVNC", true);
            _capabilities.AcceptInsecureCerts = true;
            if (null == _remoteAddress)
            {
                _driverService = FirefoxDriverService.CreateDefaultService(Path.GetFullPath(PATH), "geckodriver.exe");
                _driverService.Start();
                _remoteAddress = _driverService.ServiceUrl;

            }
            _screenshotRemoteWebDriver = new ScreenshotRemoteWebDriver(_remoteAddress, _capabilities);
            return _screenshotRemoteWebDriver;

        }

        /// <summary>
        /// Instantiates a <see cref="InternetExplorerDriver"/>.
        /// </summary>
        /// <returns>A <see cref="IWebDriver"/></returns>
        private IWebDriver LaunchInternetExplorer()
        {
            string ieVersion = ConfigurationManager.AppSettings["IeVersion"];
            if (null == _remoteAddress)
            {
                _driverService = InternetExplorerDriverService.CreateDefaultService(PATH);
                _driverService.Start();
                _remoteAddress = _driverService.ServiceUrl;
            }

            var options = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            };
            if (!string.IsNullOrEmpty(ieVersion))
                options.AddAdditionalCapability(CapabilityType.Version, ieVersion);
            options.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true);
            options.AddAdditionalCapability(CapabilityType.HandlesAlerts, true);
            options.AddAdditionalCapability(CapabilityType.SupportsFindingByCss, true);
            options.EnablePersistentHover = false;
            //options.IgnoreZoomLevel = true;
            options.RequireWindowFocus = false;

            _capabilities = (DesiredCapabilities)options.ToCapabilities();

            var remoteWebDriver = new ScreenshotRemoteWebDriver(_remoteAddress, _capabilities);

            return remoteWebDriver;
        }

        /// <summary>
        /// Instantiates a <see cref="ChromeDriver"/>.
        /// </summary>
        /// <returns>A <see cref="IWebDriver"/></returns>
        private IWebDriver LaunchChrome(string incognito = "0")
        {
            var switches = new[] { "--start-maximized", "--disable-popup-blocking", "--ignore-certificate-errors", "--multi-profiles", "--profiling-flush", "--disable-extensions", "--no-sandbox" };
            if (incognito == "1") switches[switches.Length - 1] = "--incognito";
            var options = new ChromeOptions();
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            options.AddArguments(switches);
            if (_browserOptions.BrowserArguments != null)
                options.AddArguments(_browserOptions.BrowserArguments);
            _capabilities = options.ToCapabilities() as DesiredCapabilities;
            if (null == _remoteAddress)
            {
                _driverService = ChromeDriverService.CreateDefaultService(Path.GetFullPath(PATH));
                _driverService.Start();
                _remoteAddress = _driverService.ServiceUrl;

            }
            _screenshotRemoteWebDriver = new ScreenshotRemoteWebDriver(_remoteAddress, _capabilities);
            return _screenshotRemoteWebDriver;
        }

        private IWebDriver LaunchEdge()
        {
            var switches = new[] { "--start-maximized", "--disable-popup-blocking", "--ignore-certificate-errors", "--multi-profiles", "--profiling-flush", "--disable-extensions", "--no-sandbox" };
            //if (incognito == "1") switches[switches.Length - 1] = "--incognito";
            var options = new EdgeOptions();
            //options.AddUserProfilePreference("credentials_enable_service", false);
            //options.AddUserProfilePreference("profile.password_manager_enabled", false);
            //options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            options.AddAdditionalCapability("acceptInsecureCerts",true);
            //if (_browserOptions.BrowserArguments != null)
            //    options.AddArguments(_browserOptions.BrowserArguments);
            _capabilities = options.ToCapabilities() as DesiredCapabilities;
            if (null == _remoteAddress)
            {
                _driverService = Microsoft.Edge.SeleniumTools.EdgeDriverService.CreateChromiumService(Path.GetFullPath(PATH));
                _driverService.Start();
                _remoteAddress = _driverService.ServiceUrl;

            }
            _screenshotRemoteWebDriver = new ScreenshotRemoteWebDriver(_remoteAddress, _capabilities);
            return _screenshotRemoteWebDriver;
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Checks whether the element is present or not.
        /// </summary>
        /// <param name="select">An lookup value</param>
        /// <param name="selector">An <see cref="How"/> object of lookup methods.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsElementPresent(string select, How selector, IWebElement context)
        {
            try
            {
                FindElement(select, selector, context, 0);
                return true;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Finds the first <see cref="IWebElement"/>  using the given method. 
        /// </summary>
        /// <param name="select">The element to be found.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IWebElement FindElement(string select, How selector)
        {
            return FindElement(select, selector, _webdriver);
        }

        /// <summary>
        /// Finds the first <see cref="IWebElement"/>  in the page that matches with in a document.
        /// </summary>
        /// <param name="select">The element to be found.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="context">The base element through which its corresponding elements can be found.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IWebElement FindElement(string select, How selector, ISearchContext context, int elementTimeOut = 2000)
        {
            switch (selector)
            {
                case How.ClassName:
                    return WaitandReturnElementExists(By.ClassName(select), context, elementTimeOut);
                case How.CssSelector:
                    return WaitandReturnElementExists(By.CssSelector(select), context, elementTimeOut);
                case How.Id:
                    return WaitandReturnElementExists(By.Id(select), context, elementTimeOut);
                case How.LinkText:
                    return WaitandReturnElementExists(By.LinkText(select), context, elementTimeOut);
                case How.Name:
                    return WaitandReturnElementExists(By.Name(select), context, elementTimeOut);
                case How.PartialLinkText:
                    return WaitandReturnElementExists(By.PartialLinkText(select), context, elementTimeOut);
                case How.TagName:
                    return WaitandReturnElementExists(By.TagName(select), context, elementTimeOut);
                case How.XPath:
                    return WaitandReturnElementExists(By.XPath(select), context, elementTimeOut);
            }
            throw new NotSupportedException(string.Format("Selector \"{0}\" is not supported.", selector));
        }

        /// <summary>
        /// Finds the multiple <see cref="IWebElement"/>  using the given method.
        /// </summary>
        /// <param name="select">The element to be found.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="context">The base element through which its corresponding elements can be found.</param>
        ///<returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IEnumerable<IWebElement> FindElements(string select, How selector, ISearchContext context, int elementTimeOut = 2000)
        {
            switch (selector)
            {
                case How.ClassName:
                    return WaitandReturnElementsExists(By.ClassName(select), context, elementTimeOut);
                case How.CssSelector:
                    return WaitandReturnElementsExists(By.CssSelector(select), context, elementTimeOut);
                case How.Id:
                    return WaitandReturnElementsExists(By.Id(select), context, elementTimeOut);
                case How.LinkText:
                    return WaitandReturnElementsExists(By.LinkText(select), context, elementTimeOut);
                case How.Name:
                    return WaitandReturnElementsExists(By.Name(select), context, elementTimeOut);
                case How.PartialLinkText:
                    return WaitandReturnElementsExists(By.PartialLinkText(select), context, elementTimeOut);
                case How.TagName:
                    return WaitandReturnElementsExists(By.TagName(select), context, elementTimeOut);
                case How.XPath:
                    return WaitandReturnElementsExists(By.XPath(select), context, elementTimeOut);
            }
            throw new NotSupportedException(string.Format("Selector \"{0}\" is not supported.", selector));
        }

        public Size GetWindowSize()
        {
            return _webdriver.Manage().Window.Size;
        }

        public double ReturnPageLoadTime()
        {
            //for test that is not refreshed using ajax, as dom in ajax refresh is not re loaded
            IWait<IWebDriver> wait = new WebDriverWait(_webdriver, TimeSpan.FromSeconds(30.00));
            var start = TimeSpan.TicksPerMillisecond;
            wait.Until(driver1 => ((IJavaScriptExecutor)_webdriver).ExecuteScript("return document.readyState").Equals("complete"));
            var end = TimeSpan.TicksPerMillisecond;
            return end - start;
        }

        public Dictionary<string, object> WebTimings(string pagename, int runNo)
        {
            const string scriptToExecute =
                "var performance = window.performance || window.mozPerformance || window.msPerformance || window.webkitPerformance || {}; var timings = performance.timing || {}; return timings;";

            var webTiming = (Dictionary<string, object>)((IJavaScriptExecutor)_webdriver)
                .ExecuteScript(scriptToExecute);

            // Calculate & Save the load times to an xml file
            Performance.CalculateAndSaveLoadTimes(webTiming, pagename, runNo);

            return webTiming;
        }


        /// <summary>
        /// Clicks this element
        /// </summary>
        public void ClickLinkInNewTab(string select, How selector)
        {
            var link = FindElement(select, selector, _webdriver);
            Actions newTab = new Actions(_webdriver);
            newTab.KeyDown(Keys.Control).KeyDown(Keys.Shift).Click(link).KeyUp(Keys.Control).KeyUp(Keys.Shift).Build().Perform();
        }

        /// <summary>
        ///Double Clicks this element
        /// </summary>
        public void DoubleClick(string select, How selector)
        {
            var link = FindElement(select, selector, _webdriver);
            Actions action = new Actions(_webdriver);
            action.MoveToElement(link).DoubleClick().Perform();
        }
        public void SwitchForwardTab()
        {
            Actions newTab = new Actions(_webdriver);
            newTab.KeyDown(Keys.Control).SendKeys(Keys.Tab).KeyUp(Keys.Control).Build().Perform();
        }

        public void SwitchBackwardTab()
        {
            Actions newTab = new Actions(_webdriver);
            newTab.KeyDown(Keys.Control).KeyDown(Keys.Shift).SendKeys(Keys.Tab).KeyUp(Keys.Shift).KeyUp(Keys.Control).Build().Perform();
        }


        public IWebElement WaitandReturnElementExists(By locator, ISearchContext context, int elementTimeOut = 2000)
        {
            try
            {
                if (elementTimeOut == 0)
                    return context.FindElement(locator);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to find element for locator {locator} in page {_webdriver.Url}");
            }
            
            var wait = new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval));
            IWebElement webElement = null;
            try
            {
                wait.Until(driver =>
                {
                    try
                    {
                        webElement = context.FindElement(locator);
                        return webElement != null;

                    }
                    catch (Exception ex)
                    {
                        //Console.Out.WriteLine($"Searching for locator {locator} in page  \nException Message\n" + ex.Message);
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                if (webElement == null)
                    throw new Exception($"Unable to find element for locator {locator} in page {_webdriver.Url}");
            }
            
            return webElement;
        }

        public IEnumerable<IWebElement> WaitandReturnElementsExists(By locator, ISearchContext context, int elementTimeOut = 2000)
        {
            if (elementTimeOut == 0)
                return context.FindElements(locator);

            var wait = new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval));
            IEnumerable<IWebElement> webElement = null;
            wait.Until(driver =>
            {
                try
                {
                    webElement = context.FindElements(locator);
                    return webElement != null;

                }
                catch (Exception ex)
                {
                    //Console.Out.WriteLine("unhandled exception" + ex.Message);
                    return false;
                }
            });
            return webElement;
        }

        public bool IsElementEnabled(string select, How selector, int timeOut = 0)
        {
            try
            {
                return FindElement(select, selector, _webdriver, timeOut).Enabled;
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }
        public void ElementToBeClickable(IWebElement element, int elementTimeOut = 3000)
        {
            //elementTimeOut = _browserOptions.ElementLoadTimeout;
            var wait = new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval));

            try
            {
                wait.Until(driver =>
                {
                    try
                    {
                        return element != null && element.Displayed && element.Enabled && !new[] { "is_hidden", "none", "hidden" }.Any(c => element.GetAttribute("class").Contains(c));
                    }

                    catch (Exception ex)
                    {
                        //Console.Out.WriteLine("unhandled exception clickable" + ex.Message);
                        return false;
                    }

                });
            }
            catch (Exception)
            {


            }
        }

        public void WaitTillClear(IWebElement element, int elementTimeOut = 500)
        {
            // elementTimeOut = _browserOptions.ElementLoadTimeout;
            var wait = new WebDriverWait(new SystemClock(), _webdriver, TimeSpan.FromMilliseconds(ElementTimeOut), TimeSpan.FromMilliseconds(SleepInterval));
            try
            {
                wait.Until(driver =>
                {
                    try
                    {
                        return element.Displayed && element.Enabled && element.Text.Length == 0 && element.GetAttribute("value") == "";
                    }

                    catch (Exception ex)
                    {
                        //Console.Out.WriteLine("unhandled exception clear" + ex.Message);
                        return false;
                    }

                });
            }
            catch (Exception)
            {


            }
        }

        public Point GetElementIndex(string select, How selector)
        {
            return FindElement(select, selector, _webdriver, 0).Location;
        }

        public IEnumerable<IWebElement> GetWebElementList(string select, How selector)
        {
            return FindElements(select, selector, _webdriver);
        }

        #endregion
    }
}