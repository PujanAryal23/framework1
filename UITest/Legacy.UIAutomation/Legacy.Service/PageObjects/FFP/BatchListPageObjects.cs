using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;
using Legacy.Service.PageObjects.Default;

namespace Legacy.Service.PageObjects.FFP
{
    public class BatchListPageObjects : DefaultPageObjects
    {
        #region PRIVATE FIELDS

        public const string StatsLinkXPath = "//tr[contains(@tagname, 'tr_{0}')]//td[3]//a";
        public const string BatchIdLinkXPath = "//a[text()='{0}']";

        public const string PageLnkListXPath = ".//th/a/font";
        private const string CurrentPageLnkXPath = ".//th/span/font";
        public static string PageLnkToClickTemplate = @".//th/a[contains(@onclick,""javascript:return NewPage('{0}')"")]";
        
        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = PageLnkListXPath)]
        public Link PageLnkList;

        [FindsBy(How = How.XPath, Using = CurrentPageLnkXPath)]
        public Link CurrentPageLnk;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                return string.Format(PageTitleEnum.BatchList.GetStringValue(),StartLegacy.Product.GetStringValue());
            }
        }
        #endregion

        #region CONSTRUCTOR

        public BatchListPageObjects()
            : base(FraudFinderProPageUrlEnum.Batchmenu.GetStringValue())
        {
        }

        public BatchListPageObjects(string pageUrl)
            : base(pageUrl)
        {
        }

        #endregion
    }
}
