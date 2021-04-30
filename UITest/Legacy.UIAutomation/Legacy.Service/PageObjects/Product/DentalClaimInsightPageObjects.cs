
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Product
{
    public class DentalClaimInsightPageObjects : ProductPageObjects
    {
        public DentalClaimInsightPageObjects()
            : base(ProductPageUrlEnum.Product.GetStringValue())
        {
        }
    }
}

