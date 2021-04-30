using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class SearchClearedPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string BatchIdDropDownId = "ddlBatches";
        public const string BatchIdDropDownXPathTemplate ="//table//select[@id='ddlBatches']/option[text()='{0}']";
        private const string SearchButtonId = "btnSearch";

        private const string ReleaseDateAscendingArrowXPath = "//table[contains(@id, 'dgResults')]//tr[1]//td[6]//tr[1]//td[1]//input";
        private const string FirstRowReleaseDateXPath = "//table[contains(@id, 'dgResults')]//tr[2]//td[6]";
        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = FirstRowReleaseDateXPath)] 
        public TextLabel FirstRowReleaseDate;

        [FindsBy(How = How.Id, Using = BatchIdDropDownId)]
        public TextField BatchIdDropDown;

        [FindsBy(How = How.Id, Using = SearchButtonId)] 
        public ImageButton SearchButton;

        [FindsBy(How = How.XPath, Using = ReleaseDateAscendingArrowXPath)]
        public ImageButton ReleaseDateAscendingArrow;
        #endregion

        #region CONSTRUCTOR

        public SearchClearedPageObjects()
            : base(ProductPageUrlEnum.SearchCleared.GetStringValue())
        {
            
        }

        #endregion
    }
}
