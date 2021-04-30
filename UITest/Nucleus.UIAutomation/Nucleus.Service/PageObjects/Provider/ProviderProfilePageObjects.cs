using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderProfilePageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        public const string ProviderNameId = "ctl00_MainContentPlaceHolder_ProviderNameAndScoreUc_lnkProviderAction";
        public const string ScoreId = "ctl00_MainContentPlaceHolder_ProviderNameAndScoreUc_lnkProviderScore";
        public const string StatsInfoTemplate = ".//div[@id='{0}']/div/span[@class='title']";
        public const string StatScoreValueTemplate = ".//div[@id='{0}']/div/div/div/span[@class='stat_score_value']";
        public const string StatsAverageTemplate = ".//div[@id='{0}']/div/span[@class='avg']";

        public const string ExclamationProfileIndicatorCssSelector =
          "div#ProviderInfoSection  a[class$='alert']";

        public const string ProviderExposureCountValueCssLocator =
           "div#visit_stat span.stat_score_value";
        public const string ProviderExposureAvgValueCssLocator =
            "div#visit_stat>div>span:nth-of-type(2)";
        public const string ProfileIconXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[{0}]/td[2]/a";
        public const string ProfileIconOnlyXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[{0}]/td[2]/a[1]";
        public const string ProfileIconReviewXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[{0}]/td[2]/a[2]";
        public const string WidgetProfileIconCssLocator = "a.profile_link";
        public const string WidgetProfileIconReviewCssLocator = "a.review_action.eyeball";
        public const string ProfileIndicatorCssSelector = "div.icon.profile_indicator";
        public const string ProfileReviewIndicatorCssSelector = "div.icon.toolbar_icon.eyeball ";
        public const string HistoryIconLocatorXPathTemplate = "//a[text()='Claim History']";
        public const string ProviderProfilePageHeaderXPath = "//label[@class='PopupPageTitle']";

        public const string ProviderExposureVisitCountValueCssLocator =
            "div#visit_stat span.stat_score_value";
        public const string ProviderExposureVisitAvgValueCssLocator =
            "div#visit_stat>div>span:nth-of-type(2)";
        #endregion

        #region PAGEOBJECTS

        //[FindsBy(How = How.Id, Using = ProviderNameId)]
        //public TextLabel ProviderName;

        //[FindsBy(How = How.Id, Using = ScoreId)] 
        //public TextLabel ScoreTextLabel;

        //[FindsBy(How = How.XPath, Using = ProviderProfilePageHeaderXPath)]
        //public TextLabel PageHeader;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderProfilePage.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }
       
        #endregion

        #region CONSTRUCTOR

        public ProviderProfilePageObjects()
            : base(PageUrlEnum.ProviderProfile.GetStringValue())
        {
        }

        #endregion
    }
}
