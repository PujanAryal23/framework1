using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class SearchPendedPageObjects : DefaultPageObjects
    {
         #region PRIVATE/PUBLIC PROPERTIES

        private const string SearchBtnId = "Imagebutton2";
        private const string ClearBtnXPath = ".//img[@title='Clear Form']";
        private const string ClientRecvdDateBeginCalXPath = "//table/tbody/tr[4]/td[2]/img";
        private const string ClientRecvdDateEndCalXPath = "//table/tbody/tr[4]/td[4]/img";
        private const string PendDateBeginCalXPath = "//table/tbody/tr[6]/td[2]/img";
        private const string PendDateEndCalXPath = "//table/tbody/tr[6]/td[4]/img";
        private const string ClientRecvdDateBeginId = "CliBegDate";
        private const string ClientRecvdDateEndId = "CliEndDate";
        private const string PendDateBeginId = "PendBegDate";
        private const string PendDateEndId = "PendEndDate";

        private const string SearchResultsTblId = "dgResults";

        public const string NoMatchingRecords = "lblNoRecords";
        
        #endregion  

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = SearchBtnId)]
        public ImageButton SearchBtn;

        [FindsBy(How = How.XPath, Using = ClearBtnXPath)]
        public ImageButton ClearBtn;

        [FindsBy(How = How.XPath, Using = ClientRecvdDateBeginCalXPath)]
        public Link ClientRecvdDateBeginCalLnk;

        [FindsBy(How = How.XPath, Using = ClientRecvdDateEndCalXPath)]
        public Link ClientRecvdDateEndCalLnk;

        [FindsBy(How = How.XPath, Using = PendDateBeginCalXPath)]
        public Link PendDateBeginCalLnk;

        [FindsBy(How = How.XPath, Using = PendDateEndCalXPath)]
        public Link PendDateEndCalLnk;

        [FindsBy(How = How.Id, Using = ClientRecvdDateBeginId)]
        public TextField ClientRecvdDateBeginTxt;

        [FindsBy(How = How.Id, Using = ClientRecvdDateEndId)]
        public TextField ClientRecvdDateEndTxt;

        [FindsBy(How = How.Id, Using = PendDateBeginId)]
        public TextField PendDateBeginTxt;

        [FindsBy(How = How.Id, Using = PendDateEndId)]
        public TextField PendDateEndTxt;

        [FindsBy(How = How.Id, Using = SearchResultsTblId)]
        public Table SearchResultsTbl;



        #endregion  

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.SearchPended.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public SearchPendedPageObjects()
            : base(ProductPageUrlEnum.SearchPended.GetStringValue())
        {
        }
        
        #endregion
    }
}
