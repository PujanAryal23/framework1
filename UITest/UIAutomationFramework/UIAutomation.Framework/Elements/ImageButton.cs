using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for maniputlating submission of an HTML ImageButton element to the web server.
    /// </summary>
    public class ImageButton : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> ImageButton.
        /// </summary>
        public ImageButton(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> ImageButton.
        /// </summary>
        public ImageButton(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        /// <returns>A new instance of ImageButton</returns>
        public new ImageButton Submit()
        {
            ProxyWebElement.Submit();
            return this;
        }
    }
}
