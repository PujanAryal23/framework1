using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageObjects.Welcome
{
    public class WelcomePageObjects : PageBase
    {
        #region PAGEOBJECTS PROPERTIES

        public const string NegotiationButtonXPath = "//img[contains(@title,'Negotiation')]";

        [FindsBy(How = How.XPath, Using = ".//img[contains(@title,'PhysicianClaim Insight')]")]
        public ImageButton PhysicianClaimInsight;

        [FindsBy(How = How.XPath, Using = ".//img[contains(@title,'FacilityClaim Insight')]")]
        public ImageButton FacilityClaimInsight;

        [FindsBy(How = How.XPath, Using = ".//img[contains(@title,'DentalClaim Insight')]")]
        public ImageButton DentalClaimInsight;

        [FindsBy(How = How.XPath, Using = ".//img[contains(@src,'_Graphics/icon_gray_OCI.gif')]")]
        public ImageButton OncologyClaimInsight;

        [FindsBy(How = How.XPath, Using = ".//img[contains(@src,'_Graphics/button_Logout.gif')]")]
        public ImageButton LogOutBtn;

        [FindsBy(How = How.XPath, Using = ".//img[contains(@src,'_Graphics/Button_FraudFinder.gif')]")]
        public ImageButton FraudFinderPro;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_Search.gif')]")]
        public ImageButton SearchBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/Button_Negotiation.gif')]")]
        public ImageButton NegotiationBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_Invoicing.gif')]")]
        public ImageButton InvoicingBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_Reports.gif')]")]
        public ImageButton ReportsAndEventsBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_ATS_Green.gif')]")]
        public ImageButton AtsBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_Rationale.gif')]")]
        public ImageButton RationaleBtn;

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Graphics/button_Password.gif')]")]
        public ImageButton PasswordBtn;

        #endregion

        #region CONSTRUCTOR

        public WelcomePageObjects()
            : base("")
        {

        }

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                return PageTitleEnum.WelcomePage.GetStringValue();
            }
        }

        #endregion
    }
}
