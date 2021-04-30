using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;


namespace Legacy.Service.PageObjects.Product
{
    public class PreAuthorizationSearchPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string TitleLabelId = "Title";
        private const string CloseButtonXPath = ".//img[contains(@src, '_Images/Button_Close.jpg')]";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = TitleLabelId)]
        public TextLabel TitleLabel;

        [FindsBy(How = How.XPath, Using = CloseButtonXPath)]
        public ImageButton CloseButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.PreAuthorizationSearch.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public PreAuthorizationSearchPageObjects()
            : base(string.Format(PageUrlEnum.PreAuthorizationSearch.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion



    }
}
