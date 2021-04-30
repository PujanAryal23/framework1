using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for manipulating selection of a single option in an HTML RadioButton element.
    /// </summary>
    public class RadioButton : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> RadioButton.
        /// </summary>
        public RadioButton(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> RadioButton.
        /// </summary>
        public RadioButton(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Gets if RadioButton is check/uncheck.
        /// </summary>
        public bool IsChecked()
        {
            return (ProxyWebElement.GetAttribute("checked") == "true");
        }
    }
}