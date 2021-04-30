using System;
using System.Windows.Forms.VisualStyles;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public sealed class SearchFilter
    {
        #region PRIVATE FIELDS

        private const string RadcontrolDivForDropdown = "//div[@class='rcbScroll rcbWidth']";
        private readonly InputButton _searchBtn;
        private readonly INewNavigator _navigator;
        private const string AllComboOptionCssSelector = "a.ui-multiselect-all";
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #endregion

        #region CONSTRUCTOR

        public SearchFilter(INewNavigator navigator, InputButton searchBtn, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            _searchBtn = searchBtn;
            _navigator = navigator;
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Search grid result
        /// </summary>
        /// <param name="searchfilterElement">A web element</param>
        /// <param name="value">value</param>
        /// <param name="iSsearch"></param>
        public void SearchBy(TextField searchfilterElement, string value, bool iSsearch = true)
        {
            searchfilterElement.Click();
            searchfilterElement.Clear().SetText(value);
            if (iSsearch)
                ClickOnSearchButton(searchfilterElement);
        }

        /// <summary>
        /// Select search option and search
        /// </summary>

        public void SelectOption(InputButton searchField, SelectComboBox dropDownComboBox, string selectItem, bool iSsearch = true)
        {
            searchField.Click();
            _navigator.Sleep(500);
            dropDownComboBox.SelectByText(selectItem);
            if (iSsearch)
                ClickOnSearchButton(searchField);
        }

        /// <summary>
        /// Select option and search
        /// </summary>
        /// <param name="searchField"></param>
        /// <param name="selectItem"></param>
        /// <param name="iSsearch"></param>
        public void SelectOption(TextField searchField, string selectItem, bool iSsearch)
        {
            searchField.Click();
            searchField.Clear().SetText(selectItem).Escape();
            
            if (iSsearch)
                ClickOnSearchButton(searchField);
        }

        

       

        /// <summary>
        /// Select multi-state field
        /// </summary>
        public bool SelectMultiSelectField(ImageButton multiSelectButton, MultiSelectField selectField, string selectItem, bool iSsearch = true)
        {
            multiSelectButton.Click();
            SiteDriver.WaitToLoadNew(500);
            selectField.SelectByText(selectItem);
            //TODO: Correct it further
            SiteDriver.FindElement("label", How.TagName).Click();
            
            if (iSsearch)
                ClickOnSearchButton();
            return iSsearch;
        }


        /// <summary>
        /// Select All option from multi select
        /// </summary>

        public void SelectAllOption(MultiSelectField selectField)
        {
            selectField.Click();
            
            SiteDriver.FindElement("//div[@class='ui-multiselect-menu ui-widget ui-widget-content ui-corner-all']/div/ul/li/a", How.XPath).Click();
            selectField.Click();
            
            ClickOnSearchButton();
        }

        /// <summary>

        /// </summary>
        public void ClickOnSearchButton(IElement element = null)
        {
            try
            {
                SiteDriver.WaitToLoadNew(500);
               _searchBtn.Click();
                Console.WriteLine("Clicked Search Button.");
            }
            catch (Exception)
            {
                if (element != null) element.Enter();
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void WaitForChrome(int milliSecondsTimeout = 000)
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser, StringComparison.OrdinalIgnoreCase) == 0)
                SiteDriver.WaitToLoadNew(milliSecondsTimeout);
        }

        #endregion
    }
}
