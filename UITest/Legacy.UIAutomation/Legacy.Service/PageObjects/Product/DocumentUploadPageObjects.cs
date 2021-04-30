using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class DocumentUploadPageObjects : PageBase
    {
        #region PRIVATE PROPERTIES

        private const string CloseButtonXPath = "//img[contains(@src, '_Images/Btn_ClosePop_Gray.jpg')]";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = CloseButtonXPath)] public ImageButton CloseButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.DocumentUpload.GetStringValue(); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public DocumentUploadPageObjects()
            : base(ProductPageUrlEnum.DocumentUpload.GetStringValue())
        {

        }

        #endregion
    }
}
