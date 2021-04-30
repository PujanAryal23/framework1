using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.PageObjects.Provider
{
    public class SuspectProvidersPageObjects : NewDefaultPageObjects
    {

        #region PRIVATE/PUBLIC FIELDS
        public const string FindButtonCssLocator = "button.work_button";
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string FilterLabelCssLocator = "label.form_label";
        public const string PreviousViewedProviderLabelCssLocator = "span.item_label";
        public const string PreviousViewedProviderLinkCssLocator = "span.item_link";
        public const string RightSideSubHeaderCssLocator = "div.component_sub_header";
        public const string ProviderProfileCssLocator = "div.profile_indicator";
        public const string ProviderEyeIconCssLocator = "div.toolbar_icon";
        public const string RightHeaderCssLocator = "div.provider_search_detail>div:nth-of-type({0})>div>label.component_title";
        public const string DollarIconCssLocator = "div.currency";
        public const string PersonIconCssLocator = "div.user";
        public const string GraphIconCssLocator = "div.analytic";
        public const string ProviderExposureValueCssLocator = "p.value";
        public const string ProviderExposureAverageValueCssLocator = "p.average";
        public const string FluctuationPercentageCssLocator = "p.average>span";
        #endregion

        public const string ExposureTitleLabelXPathTemplate = "//p[text() ='{0}']";

        #region Provider Decision Quadrant Graphic

        public const string RiskQuadrantTextCssTemplate = "div.component_item g:nth-of-type({0})>text:nth-of-type({1})";
        public const string SelectedQuadrantRectCssTemplate = "div.component_item g:nth-of-type({0})>rect.selected";

        #endregion

        public const string SearchFilterPanelTextXPathTemplate = "//label[text()='{0}']/../input";

        public const string FindButtonXPathTemplate = "//button[text()='Find']";

        public const string ClearButtonXPathTemplate = "//span[text()='Clear']";

        public const string NoDataAvailableXPathTemplate = "//p[text()='No Data Available']";

        public const string DropdownListXPathTemplate =
            "//label[text()='{0}']/..//ul/section[@class='available_options']/li";
        public const string LockIconByProviderSequenceXPathTemplate =
            "//ul[contains(@class,'component_item_list')]//ul[li/span[text()='{0}']]//li[contains(@class,'lock')]";

        public const string ProviderSequenceValueXPathTemplate =
            "//span[text()='{0}']";

        public const string ProviderScoreByHighScoreLevelXPath =
            "//ul[li[contains(@class,'small_risk_indicator high_risk')]]/li[3]/span";

        public const string ProviderScoreByElevatedScoreLevelXPath =
            "//ul[li[contains(@class,'small_risk_indicator elevated_risk')]]/li[3]/span";

        public const string ProviderScoreByModrateScoreLevelXPath =
            "//ul[li[contains(@class,'small_risk_indicator moderate_risk')]]/li[3]/span";

        public const string ProviderScoreByLowScoreLevelXPath =
            "//ul[li[contains(@class,'small_risk_indicator low_risk')]]/li[3]/span";

        public const string ProviderScoreGridResultListByCssSelector =
            "ul.component_item_list>.component_item>ul>li:nth-of-type(1) span.data_point_value";

        public const string SuspectProviderRiskXAxisLabel = "svg.pdq>text:nth-of-type({0})";

        public const string RiskProviderQuadrantInFindPanelCssTemplate = "svg.small_pdq>rect:nth-of-type({0})";
        public const string SuspectProviderProviderScoreLabel = "//label[contains(text(),'Provider Scorecard')]";

        public const string SuspectProviderConditionScore = 
            "div[class=\"component_item component_score column_100 is_selectable ember-view\"]:has(p.title:contains({0})) p.score";

        public const string SuspectProviderTotalScoreFromSideBar = "div.component_header_right div.score";

        public const string SuspectProviderAverageScoreFromSidebar =
            "div[class=\"component_item component_score column_100 is_selectable ember-view\"]:has(p.title:contains({0})) " +
            "div.score_visuals p.secondary, div[class=\"component_item component_score column_100 is_selectable ember-view\"]:has(p.title:contains({0})) " +
            "div.score_visuals p.secondary";

        public SuspectProvidersPageObjects()
            : base(PageUrlEnum.SuspectProviders.GetStringValue())
        {            
        }
        
    }
}