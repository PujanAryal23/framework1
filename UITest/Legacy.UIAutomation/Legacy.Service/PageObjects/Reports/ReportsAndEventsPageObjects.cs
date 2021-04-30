using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Reports
{
    public class ReportsAndEventsPageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        public ReportsAndEventsPageObjects()
            : base("Reports/Reports.aspx")
        {
        }

    }
}
