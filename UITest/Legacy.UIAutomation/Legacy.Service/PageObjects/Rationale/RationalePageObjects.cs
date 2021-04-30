using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Rationale
{
    public class RationalePageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        public RationalePageObjects()
            : base("Rationale/Rule_Rationale.aspx")
        {
        }

    }
}
