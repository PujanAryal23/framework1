using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.FFP
{
    public class FlaggedClaimPageObjects :  DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS
        public const string ClaimNoXPath = "//table[@id='dgResults']//tr[{0}]//td[3]";
        public const string ClaimSeqXPath = "//tr//td[2]//a";
        public const string ClaimStatusXPathTemplate = "//table[@id='dgResults']//tr[{0}]//td[{1}]//a";
        public const string ClaimSeqXPathTemplate = "//tr//td[2]//a[text()='{0}']";
        private const string ClaimNoAscendingArrowXPath = "//table[@id='dgResults']//tr[1]//td[3]//tr[1]//td[1]//input";
        private const string ClaimNoDescendingArrowXPath = "//table[@id='dgResults']//tr[1]//td[3]//tr[1]//td[2]//input";
        private const string SearchButtonXPath = "//img[contains(@src, '_Images/Btn_Search.jpg')]";
         

        #endregion

        [FindsBy(How = How.XPath, Using = ClaimNoAscendingArrowXPath)] public ImageButton ClaimNoAscendingArrow;

        [FindsBy(How = How.XPath, Using = ClaimNoDescendingArrowXPath)] public ImageButton ClaimNoDescendingArrow;

        [FindsBy(How = How.XPath, Using = SearchButtonXPath)]
        public ImageButton SearchButton;

        #region PAGEOBJECTS PROPERTIES

        public override string PageTitle
        {
            get
            {
                return string.Format(PageTitleEnum.FlaggedClaims.GetStringValue(),StartLegacy.Product.GetStringValue());
            }
        }

        #endregion

        #region CONSTRUCTOR

        public FlaggedClaimPageObjects()
            : base(FraudFinderProPageUrlEnum.FlaggedClaims.GetStringValue())
        {

        }

        #endregion
    }
}
