using OpenQA.Selenium;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for manipulating check into an HTML CheckBox element.
    /// </summary>
    public class CheckBox : BaseElement
    {

        /// <summary>
        /// Initializes a new instance of CheckBox.
        /// </summary>
        public CheckBox(bool cache, How select, string selector)
            : this(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of CheckBox.
        /// </summary>
        public CheckBox(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Gets or sets the CheckBox check/uncheck.
        /// </summary>
        public bool Checked
        {
            get { return (base.GetAttribute("checked") == "true"); }

            set
            {
                if (value)
                {
                    this.Check();
                }
                else
                {
                    this.Uncheck();
                }
            }
        }

        /// <summary>
        /// Click on CheckBox to make check.
        /// </summary>
        private void Check()
        {
            if (ProxyWebElement.GetAttribute("checked") != "true")
            {
                ProxyWebElement.Click();
            }
        }

        /// <summary>
        /// Click on CheckBox to make uncheck.
        /// </summary>
        private void Uncheck()
        {
            if (ProxyWebElement.GetAttribute("checked") == "true")
            {
                ProxyWebElement.Click();
            }
        }

    }
}