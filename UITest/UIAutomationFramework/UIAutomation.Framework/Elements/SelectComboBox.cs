using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Elements
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Provides a convenience method for maniputlating selections of option in an HTML SelectComboBox element. The SelectComboBox is a dropdown field, in which a single option is selected.
    /// </summary>
    public class SelectComboBox : BaseElement
    {
        /// <summary>
        /// A list of options containing SelectCombox.
        /// </summary>
        private IList<IWebElement> _options;

        public event ChangedEventHandler Changed;

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> SelectComboBox.
        /// </summary>
        public SelectComboBox(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IWebElement"/> SelectComboBox.
        /// </summary>
        public SelectComboBox(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {

        }

        /// <summary>
        /// Gets a list values SelectComboBox.
        /// </summary>
        public IList<string> Options
        {
            get
            {
                SetOptions();
                return _options.Select(option => option.Text).ToList();
            }
        }

        /// <summary>
        /// Selects a value by its name.
        /// </summary>
        /// <param name="text">A value to select.</param>
        public void SelectByText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text", "text cannot be null");
            }
            SiteDriver.WaitForCondition(() => 
            {
                SetOptions();
                foreach (var option in _options.Where(option => string.Compare(text, option.Text, StringComparison.OrdinalIgnoreCase) == 0))
                    option.Click();
                return this.Text != text;
            });
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Sets SelectComboxBox in a list.
        /// </summary>
        private void SetOptions()
        {
            var div = SiteDriver.FindElement("div", How.XPath, ProxyWebElement);
            var select = SiteDriver.FindElement("ul", How.TagName, div);
            _options = SiteDriver.FindElements("li", How.TagName, select).ToList();
        }
    }
}
