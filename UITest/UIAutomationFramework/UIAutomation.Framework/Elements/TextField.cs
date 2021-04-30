using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for manipulating text into an HTML TextBox element.
    /// </summary>
    public class TextField : BaseElement
    {
        public TextField(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        public TextField(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Gets a value from a TextBox.
        /// </summary>
        public string Value
        {
            get { return ProxyWebElement.GetAttribute("value"); }
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        public TextField SetText(string text)
        {
            ProxyWebElement.SendKeys(text);
            return this;
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        public TextField Clear(bool javascript = false)
        {
            //SiteDriver.WaitToLoadNew(300);
            ProxyWebElement.ClearElementField();
            if (javascript)
                JavaScriptExecutor.Clear(ProxyWebElement);
            SiteDriver.WaitTillClear(ProxyWebElement);
            SiteDriver.WaitToLoadNew(200);
            return this;
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        public TextField ClearWithoutWait()
        {
            //SiteDriver.WaitToLoadNew(300);
            ProxyWebElement.ClearElementField();
            return this;
        }
    }
}