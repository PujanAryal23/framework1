using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for maniputlating click event in an HTML CustomButton element.
    /// </summary>
    public class CustomButton : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of CustomButton.
        /// </summary>
        public CustomButton(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of CustomButton.
        /// </summary>
        public CustomButton(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }
    }
}