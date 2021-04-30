using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Product
{
    public class FacilityClaimInsightPageObjects : ProductPageObjects
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.FacilityClaimInsight.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public FacilityClaimInsightPageObjects()
            : base(ProductPageUrlEnum.Product.GetStringValue())
        {
        }

        #endregion


    }
}

