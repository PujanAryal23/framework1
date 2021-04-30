using System;
using System.Text;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    ///  Provides a convenience method for maniputlating selections of multiple options in an HTML MultiSelectField element. The MultiSelectField is a dropdown field, in which multiple options can be selected.
    /// </summary>
    public class MultiSelectField : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> MultiSelectField.
        /// </summary>
        public MultiSelectField(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> MultiSelectField.
        /// </summary>
        public MultiSelectField(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {

        }

        /// <summary>
        /// Gets option from MultiSelectField
        /// </summary>
        public MultiSelectField Option
        {
            get
            {
                var div1 = SiteDriver.FindElement<MultiSelectField>("div", How.TagName, this);
                var div2 = SiteDriver.FindElement<MultiSelectField>("div", How.TagName, div1);
                return SiteDriver.FindElement<MultiSelectField>("ul", How.TagName, div2);
            }
        }

        /// <summary>
        /// Selects a value by its name. 
        /// </summary>
        /// <param name="text">A value to select.</param>
        /// <returns>This class.</returns>
        public MultiSelectField SelectByText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text", "text cannot be null");
            }
            var builder = new StringBuilder(string.Format(".//li"));
            builder.Append("[");
            builder.AppendFormat("span[text()='{0}']", text);
            builder.Append(" or ");
            builder.AppendFormat("a[text()='{0}']", text);
            builder.Append("]");
            SetSelected(SiteDriver.FindElement(builder.ToString(), How.XPath, ProxyWebElement));
            return this;
        }

        /// <summary>
        /// Sets the selected option.
        /// </summary>
        /// <param name="option">The object of <see cref="MultiSelectField"/>.</param>
        private static void SetSelected(IWebElement option)
        {
            if (!option.Selected)
            {
                option.Click();
            }
        }
    }
}
