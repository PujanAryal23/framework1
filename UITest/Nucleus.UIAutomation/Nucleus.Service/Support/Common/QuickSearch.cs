using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Nucleus.Service.Support.Common
{
    public sealed class QuickSearch
    {
        #region PRIVATE FIELDS

        private const string QuickSearchDropDownArrow = "//input[@class='rcbInput']";
        private const string QuickSearchListOptionsXPath = "//div[@id='ctl00_MainContentPlaceHolder_SearchCombo_DropDown' or @id='ctl00_MainContentPlaceHolder_QuickSearchCombo_DropDown' or @id='ctl00_MainContentPlaceHolder_SearchForCombo_DropDown']/div/ul/li";

        private readonly string _comboLocatorTemplateId;

        private readonly SelectComboBox _dropDown;

        private readonly InputButton _searchComboInput;
        private readonly INewNavigator _navigator;


        #endregion

        #region PUBIC PROPERTIES

        #endregion

        #region CONSTRUCTOR

        public QuickSearch(INewNavigator navigator, InputButton searchCombo, SelectComboBox dropDown)
        {
            _navigator = navigator;
            _searchComboInput = searchCombo;
            _dropDown = dropDown;
        }

        #endregion

        #region PUBLIC METHODS

        public void ClickQuickSearchComboBox()
        {
            SiteDriver.FindElement(QuickSearchDropDownArrow, How.XPath).Click();
        }

        

        public void SelectQuickSearchOptions(String quickSearchOption)
        {
            ClickQuickSearchCombo();
            _dropDown.SelectByText(quickSearchOption);
            
        }

        public IList<string> SelectAndGetQuickSearchOptions()
        {
            ClickQuickSearchCombo();
            return _dropDown.Options;
        }

        #endregion

        #region PRIVATE METHODS

        private void ClickQuickSearchCombo()
        {
            _searchComboInput.Click();
            SiteDriver.WaitToLoadNew(200);
            
        }

        #endregion
    }
}
