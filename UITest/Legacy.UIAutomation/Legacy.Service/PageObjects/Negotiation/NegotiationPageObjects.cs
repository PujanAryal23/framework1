using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Negotiation
{
    public class NegotiationPageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        public NegotiationPageObjects()
            : base("Negotiation/Negotiation.aspx")
        {
        }
    }
}
