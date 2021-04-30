using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for getting text from an HTML TextLabel element.
    /// </summary>
    public class Div : BaseElement
    {
        /// <summary>
        /// Initializes a new instances of TextLabel.
        /// </summary>
        public Div(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instances of TextLabel.
        /// </summary>
        public Div(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }
    }
}