using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for manipulating text into an HTML TextArea element.
    /// </summary>
    public class TextArea : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of TextArea class.
        /// </summary>
        public TextArea(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of TextArea class.
        /// </summary>
        public TextArea(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        public TextArea SetText(string text)
        {
            ProxyWebElement.SendKeys(text);
            return this;
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        public new TextArea Clear()
        {
            ProxyWebElement.Clear();
            return this;
        }

        public new TextArea Escape()
        {
            ProxyWebElement.SendKeys(Keys.Escape);
            return this;
        }

    }
}
