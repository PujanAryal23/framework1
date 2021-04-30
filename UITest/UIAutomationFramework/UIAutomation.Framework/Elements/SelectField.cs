using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for manipulating selections of options in an HTML select element.
    /// </summary>
    public class SelectField : BaseElement
    {
        private SelectElement _selm;
        /// <summary>
        /// Gets a value indicating whether the parent element supports multiple selections.
        /// </summary>
        public bool IsMultiple
        {
            get
            {
                return Selm.IsMultiple;
            }
        }

        /// <summary>
        /// Gets the list of options for the select element.
        /// </summary>
        public IList<IElement> Options
        {
            get
            {
                return Selm.Options as IList<IElement>;
            }
        }

        /// <summary>
        /// Gets the selected item within the select element.
        /// </summary>
        /// <remarks>If more than one item is selected this will return the first item.</remarks>
        /// <exception cref="NoSuchElementException">Thrown if no option is selected.</exception>
        public IElement SelectedOption
        {
            get
            {
                return Selm.SelectedOption as IElement;
            }
        }

        /// <summary>
        /// Gets all of the selected options within the select element.
        /// </summary>
        public IList<IElement> AllSelectedOptions
        {
            get
            {
                return Selm.AllSelectedOptions as IList<IElement>;
            }
        }

        private SelectElement Selm {
            get
            {
                return  new SelectElement(ProxyWebElement);
            }
        }

        /// <summary>
        /// Initializes a new instance of the SelectField class.
        /// </summary>
        public SelectField(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the SelectField class.
        /// </summary>
        public SelectField(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
           
        }

        /// <summary>
        /// Select all options by the text displayed.
        /// </summary>
        /// <param name="text">The text of the option to be selected. If an exact match is not found,
        /// this method will perform a substring match.</param>
        /// <remarks>When given "Bar" this method would select an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        /// <exception cref="NoSuchElementException">Thrown if there is no element with the given text present.</exception>
        public SelectField SelectByText(string text)
        {
            Selm.SelectByText(text);
            return this;
        }

        /// <summary>
        /// Select the option by the index, as determined by the "index" attribute of the element.
        /// </summary>
        /// <param name="index">The value of the index attribute of the option to be selected.</param>
        /// <exception cref="NoSuchElementException">Thrown when no element exists with the specified index attribute.</exception>
        public SelectField SelectByIndex(int index)
        {
            Selm.SelectByIndex(index);
            return this;
        }


        /// <summary>
        /// Select an option by the value.
        /// </summary>
        /// <param name="value">The value of the option to be selected.</param>
        /// <remarks>When given "foo" this method will select an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        /// <exception cref="NoSuchElementException">Thrown when no element with the specified value is found.</exception>
        public SelectField SelectByValue(string value)
        {
            Selm.SelectByValue(value);
            return this;
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        /// <param name="selection">The text to type into the element.</param>
        /// <remarks>The text to be typed may include special characters like arrow keys,
        /// backspaces, function keys, and so on. Valid special keys are defined in 
        /// <see cref="Keys"/>.</remarks>
        /// <seealso cref="Keys"/>
        /// <exception cref="InvalidElementStateException">Thrown when the target element is not enabled.</exception>
        /// <exception cref="ElementNotVisibleException">Thrown when the target element is not visible.</exception>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public new SelectField SendKeys(string selection)
        {
            ProxyWebElement.SendKeys(selection);
            return this;
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        /// <remarks>If this element is a text entry element, the <see cref="Clear"/>
        /// method will clear the value. It has no effect on other elements. Text entry elements 
        /// are defined as elements with INPUT or TEXTAREA tags.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public new SelectField Clear()
        {
            ProxyWebElement.ClearElementField();
            SiteDriver.WaitTillClear(ProxyWebElement);
            return this;
        }
    }
}