using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for maniputlating click event in an HTML Link element.
    /// </summary>
    public class Link : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> Link.
        /// </summary>
        public Link(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> Link.
        /// </summary>
        public Link(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }
    }
}
