using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.FFP
{
    public class FraudFinderProPageObjects : DefaultPageObjects
    {

        #region PRIVATE/PUBLIC FIELDS

        private const string BatchListXPath = ".//img[contains(@src, '_Images/Btn_BatchList.jpg')]";
        private const string BackXPath = ".//img[contains(@src, '_Images/Btn_Back.jpg')]";

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = BatchListXPath)]
        public ImageButton BatchListBtn;

        [FindsBy(How = How.XPath, Using = BackXPath)]
        public ImageButton BackBtn;

        #endregion
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.FraudFinderPro.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public FraudFinderProPageObjects()
            : base(FraudFinderProPageUrlEnum.FraudFinderPro.GetStringValue())
        {
        }
        
        #endregion
    }
}
