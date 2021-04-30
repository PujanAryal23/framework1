using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class SearchProductPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string SearchBtnId = "Imagebutton2";
        private const string ClearBtnXPath = ".//img[@title='Clear Form']";
        private const string PatientDobCalXPath = "//table/tbody/tr[8]/td[5]/img";
        private const string BeginDosCalXPath = "//table/tbody/tr[16]/td[3]/img";
        private const string EndDosCalXPath = "//table/tbody/tr[16]/td[5]/img";
        private const string PatientDobId = "txtPatDOB";
        private const string BeginDosId = "txtBeginDOS";
        private const string EndDosId = "txtEndDOS";
        
        #endregion  

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = SearchBtnId)]
        public ImageButton SearchBtn;

        [FindsBy(How = How.XPath, Using = ClearBtnXPath)]
        public ImageButton ClearBtn;

        [FindsBy(How = How.XPath, Using = PatientDobCalXPath)]
        public Link PatientDobCalLnk;

        [FindsBy(How = How.XPath, Using = BeginDosCalXPath)]
        public Link BeginDosCalLnk;

        [FindsBy(How = How.XPath, Using = EndDosCalXPath)]
        public Link EndDosCallLnk;

        [FindsBy(How = How.Id, Using = PatientDobId)]
        public TextField PatientDobTxt;

        [FindsBy(How = How.Id, Using = BeginDosId)]
        public TextField BeginDosTxt;

        [FindsBy(How = How.Name, Using = EndDosId)]
        public TextField EndDosTxt;


        #endregion  

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get {return string.Format(PageTitleEnum.SearchProduct.GetStringValue(),StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public SearchProductPageObjects()
            : base(ProductPageUrlEnum.SearchProduct.GetStringValue())
        {
        }
        
        #endregion
    }
}
