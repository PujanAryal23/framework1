using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Utils
{
    public interface IJavaScriptExecutors
    {
        void Init(ISiteDriver driver);

        bool IsWindowOpenedAsTab();

        void ExecuteClick(string select, How selector);

        bool ExecuteJqueryStatus();

        List<string> FindElements(string select, string selectSelector = "Text", bool isNull = false,
            How selector = How.CssSelector);

        List<IWebElement> FindWebElements(string select);

        void ExecuteScript(string script, params object[] args);

        void ClickJQuery(string select);

        void ExecuteMouseOver(string select, How selector);

        IWebElement FindElement(string select);

        void ExecuteToScrollToView(string elementId, string colIndex);
        void ExecuteToScrollToView(string elementClassName);
        IWebElement ClickAndGetWithJquery(string select1, string select2, int interval = 100);
        IWebElement ClickAndGet(string select1, string select2, bool mulitpleScript = false);
        int FindElementsCount(string select);
        void ExecuteClick(IWebElement element);

        string GetText(string select);

        bool IsElementPresent(string select);

        void ExecuteMouseOut(string select, How selector);

        void SendKeys(string strKey, string select, How selector);

        void Clear(string select, How selector);

        void Clear(IWebElement element);

        bool IsDownloadedFilePresent();

        IWebElement GetDownloadedFilename(int index = 0);

        int GetDownloadedFilenameList();
        int ScrollHeight(string select);

        int ClientHeight(string select);

        object Execute(string script);

        void SetProcCode(string procCodeArrary, string input, string searchIcon);

        void SendKeysToInnerHTML(string strKey, string select, How selector);

        void ExecuteMouseOut(string select);
        void ExecuteScrollToMostRight(string elementClassName);

        void ExecuteToScrollToSpecificDiv(string element, bool bottom = true);

        object GetUploadFileListObject(string select);
        void ClickJQueryByText(string select, string text);
        void ExecuteToScrollToSpecificDivUsingJquery(string element, bool bottom = true);

        void ExecuteToScrollToLastLi(string element, bool bottom = true);
        void WaitForJqueryStatusCondition();
        void CloseFrame(string frameName);
        void SwitchFrameByJQuery(string cssLocator);
        List<string> FindElements(string select, How selector, string selectSelector, bool isNull = false);
    }
}
