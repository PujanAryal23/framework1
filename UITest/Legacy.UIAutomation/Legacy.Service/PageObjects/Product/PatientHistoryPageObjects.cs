using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class PatientHistoryPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string PatientHistoryPageLabelXPath = "//td[contains(@class, 'masthead')]//table//tr//td[2]/span[2]";
        private const string PatientHistoryCloseButtonXPath = "//img[contains(@src, '_Images/Btn_ClosePop_Gray.jpg')]";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = PatientHistoryPageLabelXPath)]
        public TextLabel PatientHistoryPageLabel;

        [FindsBy(How = How.XPath, Using = PatientHistoryCloseButtonXPath)]
        public ImageButton PatientHistoryCloseButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.PatientHistory.GetStringValue(); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public PatientHistoryPageObjects()
            : base(ProductPageUrlEnum.PatientHistory.GetStringValue())
        {

        }

        #endregion
    }
}
