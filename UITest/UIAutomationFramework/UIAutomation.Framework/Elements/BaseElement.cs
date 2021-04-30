using OpenQA.Selenium;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using System;
using UIAutomation.Framework.Utils;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Defines a class for base element.
    /// </summary>
    /// <remarks>
    /// This implements <see cref="IElement"/> IElement interface.
    /// </remarks>
    public abstract class BaseElement : IElement, IProxyWebElement
    {
        private bool _cached = false;
        private How _selector;
        private string _select;
        private IWebElement _cachedElement;
        private ISearchContext _context;

        /// <summary>
        /// Initializes a new instance of the BaseElement class.
        /// </summary>
        /// <param name="cached"></param>
        /// <param name="selector"></param>
        /// <param name="select"></param>
        /// <param name="context"></param>
        protected BaseElement(bool cached, How selector, string select, ISearchContext context)
        {
            _cached = cached;
            if (_cached)
                Console.WriteLine("Cache set here");
            _selector = selector;
            _select = select;
            _context = context;
        }

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets the innerText of this element, without any leading or trailing whitespace, 
        /// and with other whitespace collapsed.
        /// </summary>
        public string Text
        {
            get { return ProxyWebElement.Text; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return ProxyWebElement.Enabled; }
        }

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        public string TagName
        {
            get { return ProxyWebElement.TagName; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        public bool Displayed
        {
            get { return ProxyWebElement.Displayed; }
        }

        /// <summary>
        /// Gets the base element.
        /// </summary>
        public object BaseObject
        {
            get { return ProxyWebElement; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        public bool Selected
        {
            get { return ProxyWebElement.Selected; }
        }

        /// <summary>
        /// Get a location of element.
        /// </summary>
        public Point Location
        {
            get { return ProxyWebElement.Location; }
        }

        /// <summary>
        /// Gets height and width of element.
        /// </summary>
        public Size Size
        {
            get { return ProxyWebElement.Size; }
        }

        /// <summary>
        /// Clear the value from element.
        /// </summary>
        public void Clear()
        {
            SiteDriver.WaitToLoadNew(300);
            ProxyWebElement.ClearElementField();
            SiteDriver.WaitTillClear(ProxyWebElement);
        }

        /// <summary>
        /// Send text in element.
        /// </summary>
        /// <param name="text"></param>
        public void SendKeys(string text)
        {
            ProxyWebElement.SendKeys(text);
        }

        /// <summary>
        /// Submit a form.
        /// </summary>
        public void Submit()
        {
            ProxyWebElement.Submit();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Gets the value of the specified attribute for this element
        /// </summary>
        /// <param name="attribute"></param>
        public string GetAttribute(string attribute)
        {
            return ProxyWebElement.GetAttribute(attribute);
        }

        /// <summary>
        /// Gets the value of a CSS property of this element
        /// </summary>
        /// <param name="property"></param>
        public string GetCssValue(string property)
        {
            return ProxyWebElement.GetCssValue(property);
        }

        /// <summary>
        /// Clicks this element
        /// </summary>
        public void Click()
        {
            SiteDriver.ElementToBeClickable(ProxyWebElement);
            SiteDriver.WaitToLoadNew(500);
            ProxyWebElement.Click();
           
            //SiteDriver.WaitToLoadNew(500);
        }

        /// <summary>
        /// Clicks this element
        /// </summary>
        public void ClickWithOutWait()
        {
            
            SiteDriver.WaitToLoadNew(500);
            ProxyWebElement.Click();
            //SiteDriver.WaitToLoadNew(500);
        }



        /// <summary>
        /// Press Enter key
        /// </summary>
        public void Enter()
        {
            ProxyWebElement.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Press Tab Key
        /// </summary>
        public void Tab()
        {
            ProxyWebElement.SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Press Esc key
        /// </summary>
        public void Escape()
        {
            ProxyWebElement.SendKeys(Keys.Escape);
        }

        public void Backspace()
        {
            ProxyWebElement.SendKeys(Keys.Backspace);
        }

        /// <summary>
        /// Press Esc key
        /// </summary>
        public void PageDown()
        {
            ProxyWebElement.SendKeys(Keys.PageDown);
        }

        #endregion

        public IWebElement ProxyWebElement
        {
            get
            {
                if (_cached && _cachedElement != null)
                {
                    Console.WriteLine("IWebElement cache captured");
                    return _cachedElement;
                }
                _cachedElement = _context != null
                                     ? SiteDriver.FindElement(_select, _selector, _context)
                                     : SiteDriver.FindElement(_select, _selector);
                return _cachedElement;
            }
        }


        public void ClickAndWait(Action wait)
        {
            SiteDriver.ElementToBeClickable(ProxyWebElement);
            SiteDriver.WaitToLoadNew(500);
            ProxyWebElement.Click();
            wait.Invoke();
        }

        public void ClickJS()
        {
            SiteDriver.ElementToBeClickable(ProxyWebElement);
            SiteDriver.WaitToLoadNew(500);
            JavaScriptExecutor.ExecuteClick(ProxyWebElement);
        }

        public void ArrowDown()
        {
            ProxyWebElement.SendKeys(Keys.ArrowDown);
        }
    }
}