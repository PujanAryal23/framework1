using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class ModifiedEditsPageObjects : DefaultPageObjects
    {
        #region PRIVATE FIELDS

        private const string ModBegDateCalendarXPath = "//table/tbody/tr[5]/td[2]/img";
        private const string ModEndDateCalendarXPath = "//table/tbody/tr[5]/td[4]/img";
        private const string ModBegDateTextFieldName = "ModBegDate";
        private const string ModEndDateTextFieldName = "ModEndDate";
        private const string SearchButtonXPath = "//img[contains(@title, 'Search')]";
        private const string SearchResultsTblXPath = ".//form//table[2]//tr[1]//td[1]//table";

        public const string SearchResultsTblXPathTemplate = ".//form//table[2]//tr[1]//td[1]//table//tbody//tr[{0}]//td[{1}]";

        public const string NoMatchingRecordsXPath = ".//table//tr[3]//td[1]//span";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = ModBegDateCalendarXPath)] 
        public Link ModBegDateCalendarLink;

        [FindsBy(How = How.XPath, Using = ModEndDateCalendarXPath)] 
        public Link ModEndDateCalendarLink;

        [FindsBy(How = How.Name, Using = ModBegDateTextFieldName)]
        public TextField ModBegDateTextField;

        [FindsBy(How = How.Name, Using = ModEndDateTextFieldName)]
        public TextField ModEndDateTextField;

        [FindsBy(How = How.XPath, Using = SearchButtonXPath)]
        public ImageButton SearchButton;

        [FindsBy(How = How.XPath, Using = SearchResultsTblXPath)] 
        public Table SearchResultsTbl;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.ModifiedEdits.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public ModifiedEditsPageObjects()
            : base(ProductPageUrlEnum.ModifiedEdits.GetStringValue())
        {
        }
        
        #endregion
    }
}
