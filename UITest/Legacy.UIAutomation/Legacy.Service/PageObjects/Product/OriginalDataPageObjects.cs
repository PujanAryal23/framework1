using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;


namespace Legacy.Service.PageObjects.Product
{
    public class OriginalDataPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string TitleLabelId = "Title";
        private const string CloseButtonXPath = "//img[contains(@src, '_Images/Btn_ClosePop_Gray.jpg')]";

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
            get { return string.Format(PageTitleEnum.OriginalData.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public OriginalDataPageObjects()
            : base(ProductPageUrlEnum.OriginalData.GetStringValue())
        {

        }

        #endregion



    }
}
