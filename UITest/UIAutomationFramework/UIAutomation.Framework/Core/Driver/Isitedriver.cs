using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace UIAutomation.Framework.Core.Driver
{
    public interface ISiteDriver
    {
        #region properties

        IJavaScriptExecutor JsExecutor { get; }
        string BaseUrl
        {
            get;
            set;
        }
        WebDriverWait waitPageLoad { get; }

        string Url { get; }
        List<string> WindowHandles { get; }
        string Title { get; }

        ICookieJar GetCookies { get; }

        IWebDriver WebDriver { get; }

        string CurrentWindowHandle { get; }
         WebDriverWait WaitTime { get; }

         void WebdriverSwitchTo(IWebElement element);
       
        #endregion

        // void WaitForCondition();
        object InitializePageElement(object page);
        void Start();
        string  FindElementAndGetAttribute(string select, How selector, int index);
        string FindElementAndGetAttribute(String select, How selector, string attribute = "id");
        void SwitchFrameByClass(string className);

        void SwitchFrameById(string frameName);
        void CaptureScreenShot(string name);
        void Open(string url);
        void Init(IBrowserOptions browserOptions);
        void MaximizeWindow();
        void CloseWindow();
        void CloseWindow(string windowName);
        string GetAlertBoxText();
        void AcceptAlertBox();
        void DismissAlert();
        void Stop();
        void SwitchFrame(string frameName);
        void CloseFrame(string frameName);

        void Reload();
        void Back();
        void WaitForPageToLoad();
        void WaitForAjaxLoad(string library);
        T FindElement<T>(string select, How selector, bool cache = false) where T:class,IElement;
        void WaitForCondition(Func<bool> f, int milliSec = 0);
        bool IsAlertBoxPresent();
        string GetScreenshotFilename();
        void SwitchWindow(string windowName);
        void LocalFileDetect();
        bool IsUrlValid(string url);
        IWebElement ReturnActiveElement();
        Point GetElementIndex(string select, How selector);
        bool IsElementPresent(string select, How selector, IWebElement context);
        bool IsElementPresent(string select, How selector, int timeOut =0);

      //  List<string> FindElements(string select, How selector, string selectSelector, bool isNull = false);
        bool IsElementEnabled(string select, How selector, int timeOut = 0);

        void FindElementAndSetText(string select, How selector, string reasonCode);
        void Refresh();

        string GetElementText(string select, How selector, int timeOut = 0);

        void WaitToLoadNew(int millisecondsTimeout);

        

        void MouseOver(string select, How selector, int timeout = 2000,bool release = false);

        bool SwitchWindowByTitle(string windowTitle);

        void WaitForIe(int millisecondsTimeout);

        void SwitchFrameByCssLocator(string cssLocator);

        void SwitchBackToMainFrame();

        bool SwitchWindowByUrl(string windowUrl);
        List<string> FindElementAndGetNotNullIdAttribute(string select, How selector);
        

        void SwitchForwardTab();

        void SwitchToLastWindow();
        //void SwitchFrameByJQuery(string cssLocator);
        bool CloseWindowAndSwitchTo(string windowName);

        int FindElementsCount(string select, How selector);

        IEnumerable<IWebElement> FindElements(string select, How selector);

        void FindElementAndClickOnCheckBox(string select, How selector);

        List<string> FindElementsAndGetAttribute(string attribute, string select, How selector);

        void WaitForCorrectPageToLoad(string pageTitle);

        void ElementToBeClickable(IWebElement element, int elementTimeOut = 3000);

        IWebElement FindElement(string select, How selector, ISearchContext context, int elementTimeOut = 2000);

        IWebElement FindElement(string select, How selector);

        void WaitTillClear(IWebElement element, int elementTimeOut = 500);
        List<string> FindDisplayedElementsText(string select, How selector);


        List<bool> FindElementsAndGetAttributeByTwoClass(string className1, string className2, string select,
            How selector);

        List<string> FindElementAndGetClassAttribute(string select, How selector);

        void SwitchFrameByXPath(string xpath);


        List<bool> FindElementAndGetActiveAttribute(string select, How selector);



        List<bool> FindElementsAndGetAttributeByClass(string className, string select, How selector);


        string GetHandleByTitle(string title);

        void SwitchBackwardTab();

        bool IsElementDisplayed(string select, How selector, int timeOut = 0);

        bool SwitchDynamicWindowByTitle(string windowTitle);

        void WaitForAjaxToLoad(string ajaxLibrary);

        List<string> FindElementAndGetAttributeByAttributeName(string select, How selector, string attributeName);

        List<bool> FindElementAndGetCheckedAttribute(string select, How selector);
        Size GetWindowSize();

        IEnumerable<IWebElement> GetWebElementList(string select, How selector);
    }
}
