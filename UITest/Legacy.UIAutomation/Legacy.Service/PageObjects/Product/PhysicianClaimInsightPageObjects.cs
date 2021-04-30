
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Product
{
    public class PhysicianClaimInsightPageObjects : ProductPageObjects
    {
        #region CONSTRUCTOR
        public PhysicianClaimInsightPageObjects()
            : base(ProductPageUrlEnum.Product.GetStringValue())
        {
        }
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.PhysicianClaimInsight.GetStringValue(); }
        }

        #endregion
    }
}

