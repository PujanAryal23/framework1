using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for maniputlating submission of an HTML InputButton element to the web server.
    /// </summary>
    public class InputButton : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> InputButton.
        /// </summary>
        public InputButton(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> InputButton.
        /// </summary>
        public InputButton(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        /// <returns>A new instance of InputButton</returns>
        public new InputButton Submit()
        {
            ProxyWebElement.Submit();
            return this;
        }
    }
}