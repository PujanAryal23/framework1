using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class FlaggedClaimPageObjects :  DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS
        public const string ClaimNoXPath = "//table[@id='dgResults']//tr[{0}]//td[3]";
        public const string ClaimSeqXPath = "//tr//td[2]//a";
        public const string ClaimStatusXPathTemplate = "//table[@id='dgResults']//tr[{0}]//td[{1}]//a";
        public const string ClaimSeqXPathTemplate = "//tr//td[2]//a[text()='{0}']";
        private const string ClaimNoAscendingArrowXPath = "//table[@id='dgResults']//tr[1]//td[3]//tr[1]//td[1]//input";
        public const string ClaimNoDescendingArrowXPath = "//table[@id='dgResults']//tr[1]//td[3]//tr[1]//td[2]//input";

        #endregion


        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = ClaimNoAscendingArrowXPath)] 
        public ImageButton ClaimNoAscendingArrow;

        [FindsBy(How = How.XPath, Using = ClaimNoDescendingArrowXPath)]
        public ImageButton ClaimNoDescendingArrow;

        #endregion

        #region CONSTRUCTOR

        public FlaggedClaimPageObjects()
            : base(ProductPageUrlEnum.FlaggedClaims.GetStringValue())
        {

        }

        #endregion
    }
}
