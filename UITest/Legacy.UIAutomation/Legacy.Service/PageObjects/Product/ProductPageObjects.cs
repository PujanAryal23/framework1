using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class ProductPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        private const string WelcomePageButtonXPath = ".//img[contains(@src, '_Images/Btn_MainMenu.jpg')]";
        private const string BatchListXPath = ".//img[contains(@src, '_Images/Btn_BatchList.jpg')]";
        private const string BackXPath = ".//img[contains(@src, '_Images/Btn_Back.jpg')]";
        private const string LogicXPath = ".//img[contains(@src, '_Images/Btn_Logic.jpg')]";
        private const string SearchProductXPath = ".//img[contains(@src, '_Images/Btn_Search.jpg')]";
        private const string UnreviewedClaimsXPath = ".//img[contains(@src, '_Images/Btn_UnreviewedClaims.jpg')]";
        private const string ModifiedEditsXPath = ".//img[contains(@src, '_Images/Btn_ModifiedEdits.jpg')]";
        private const string PendedClaimsXPath = ".//img[contains(@src, '_Images/Btn_Pended.jpg')]";
        private const string DocsRequiredXPath = ".//img[contains(@src, '_Images/Btn_Docs_Required.jpg')]";
        private const string DocsRequestedXPath = ".//img[contains(@src, '_Images/Btn_Docs_Requested.jpg')]";
        private const string DocsReceivedXPath = ".//img[contains(@src, '_Images/Btn_Docs_Received.jpg')]";
        private const string PreAuthorizationXPath = ".//img[contains(@src, '_Images/Btn_PreAuth.jpg')]";
        private const string SearchClearedXPath = ".//img[contains(@src, '_Images/Btn_Cleared.jpg')]";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = WelcomePageButtonXPath)]
        public ImageButton WelcomePageBtn;

        [FindsBy(How = How.XPath, Using = PreAuthorizationXPath)]
        public Link PreAuthorizationLink;

        [FindsBy(How = How.XPath, Using = SearchClearedXPath)]
        public Link SearchClearedLink;

        [FindsBy(How = How.XPath, Using = BatchListXPath)]
        public ImageButton BatchListBtn;

        [FindsBy(How = How.XPath, Using = BackXPath)]
        public ImageButton BackBtn;

        [FindsBy(How = How.XPath, Using = LogicXPath)]
        public ImageButton LogicBtn;

        [FindsBy(How = How.XPath, Using = SearchProductXPath)]
        public ImageButton SearchProductBtn;

        [FindsBy(How = How.XPath, Using = UnreviewedClaimsXPath)]
        public ImageButton UnreviewedClaimsBtn;

        [FindsBy(How = How.XPath, Using = ModifiedEditsXPath)]
        public ImageButton ModifiedEditsBtn;
        
        [FindsBy(How = How.XPath, Using = PendedClaimsXPath)]
        public ImageButton PendedClaimsBtn;

        [FindsBy(How = How.XPath, Using = DocsRequiredXPath)]
        public ImageButton DocsRequiredBtn;

        [FindsBy(How = How.XPath, Using = DocsRequestedXPath)]
        public ImageButton DocsRequestedBtn;

        [FindsBy(How = How.XPath, Using = DocsReceivedXPath)]
        public ImageButton DocsReceivedBtn;


        #endregion

        #region CONSTRUCTOR

        public ProductPageObjects(string pageUrl)
            : base(pageUrl)
        {
        }

        #endregion
    }
}
