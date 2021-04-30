using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using Legacy.Service.PageObjects.Default;

namespace Legacy.Service.PageObjects.Product
{
    public class OncologyClaimInsightPageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;
        public OncologyClaimInsightPageObjects()
            : base("Product/Product.aspx?Prod=O")
        {
        }

    }
}

