using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class SearchUnreviewedPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string SearchBtnId = "btnSearch";
        private const string FromClientRecvdDateCalXPath = "//table/tbody/tr[3]/td[2]/img";
        private const string ToClientRecvdDateCalXPath = "//table/tbody/tr[3]/td[4]/img";
        private const string FromClientRecvdDateId = "txtFromClientRecvdDate";
        private const string ToClientRecvdDateId = "txtToClientRecvdDate";

        private const string SearchResultsTblId = "dgResults";

        public const string NoMatchingRecords = "lblNoRecords";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = SearchBtnId)]
        public ImageButton SearchBtn;

        [FindsBy(How = How.XPath, Using = FromClientRecvdDateCalXPath)]
        public Link FromClientRecvdDateCalLnk;

        [FindsBy(How = How.XPath, Using = ToClientRecvdDateCalXPath)]
        public Link ToClientRecvdDateCalLnk;

        [FindsBy(How = How.Id, Using = FromClientRecvdDateId)]
        public TextField FromClientRecvdDateTxt;

        [FindsBy(How = How.Id, Using = ToClientRecvdDateId)]
        public TextField ToClientRecvdDateTxt;

        [FindsBy(How = How.Id, Using = SearchResultsTblId)]
        public Table SearchResultsTbl;


        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                return StartLegacy.Product.Equals(ProductEnum.DCI) ? string.Format(PageTitleEnum.SearchUnreviewed.GetStringValue(), "DentalClaim Insight") : string.Format(PageTitleEnum.SearchUnreviewed.GetStringValue(), StartLegacy.Product.GetStringValue());
            }
        }

        #endregion

        #region CONSTRUCTOR

        public SearchUnreviewedPageObjects()
            : base(ProductPageUrlEnum.SearchUnreviewed.GetStringValue())
        {
        }

        #endregion
    }
}
