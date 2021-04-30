using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class PreAuthPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string UnreviewedXPath = ".//img[contains(@src, '_Images/Btn_UnreviewedClaims.jpg')]";
        private const string DocumentReviewXPath = ".//img[contains(@src, '_Images/Btn_Doc_Review.jpg')]";
        private const string PendedXPath = ".//img[contains(@src, '_Images/Btn_Pended.jpg')]";
        private const string SearchXPath = ".//img[contains(@src, '_Images/Btn_Search.jpg')]";
        private const string LogicRequestsXPath = ".//img[contains(@src, '_Images/Btn_Logic_Req.jpg')]";
        private const string ClosedXPath = ".//img[contains(@src, '_Images/Btn_Closed.jpg')]";

        #region DCI
        private const string ConsultRequiredXPath = ".//img[contains(@src, '_Images/Btn_Consult_Required.jpg')]";
        private const string ConsultCompleteXPath = ".//img[contains(@src, '_Images/Btn_Consult_Complete.jpg')]";
        private const string HciConsultRequiredXPath = ".//input[contains(@value, 'Internal Consult Reqd')]";
        private const string HciConsultCompleteXPath = ".//input[contains(@value, 'Internal Consult Cmplt')]";
        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = HciConsultRequiredXPath)]
        public ImageButton HciConsultRequiredButton;

        [FindsBy(How = How.XPath, Using = HciConsultCompleteXPath)]
        public ImageButton HciConsultCompleteButton;

        [FindsBy(How = How.XPath, Using = ConsultCompleteXPath)]
        public ImageButton ConsultCompleteButton;

        [FindsBy(How = How.XPath, Using = ConsultRequiredXPath)]
        public ImageButton ConsultRequiredButton;

        [FindsBy(How = How.XPath, Using = UnreviewedXPath)]
        public ImageButton UnreviewedButton;

        [FindsBy(How = How.XPath, Using = PendedXPath)]
        public ImageButton PendedButton;

        [FindsBy(How = How.XPath, Using = LogicRequestsXPath)]
        public ImageButton LogicRequestsButton;

        [FindsBy(How = How.XPath, Using = DocumentReviewXPath)] 
        public ImageButton DocumentReviewButton;

        [FindsBy(How = How.XPath, Using = SearchXPath)] 
        public ImageButton SearchButton;

        [FindsBy(How = How.XPath, Using = ClosedXPath)]
        public ImageButton ClosedButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.PreAuthorizations.GetStringValue(), StartLegacy.PreAuthorizationProduct); }
        }

        #endregion

        #region CONSTRUCTOR

        public PreAuthPageObjects()
            : base(string.Format(PageUrlEnum.PreAuth.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}
