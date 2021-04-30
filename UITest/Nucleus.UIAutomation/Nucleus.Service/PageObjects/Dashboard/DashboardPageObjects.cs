using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.PageObjects.Dashboard
{
    public class DashboardPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string AppealOverdueIconCssSelector = "span.warning_icon[title = \"Contains at least 1 overdue appeal\"]";
        public const string DashboardLabelCssSelector = "label.page_title";
        public const string DashboardIconByCss = ".dashboard";
        public const string ClaimsOverviewXPath = "//span[text()='Claims Overview']";
        public const string UnapprovedClaimsOverviewWidgetXPath = "//div[@id='column_1']/ul[2]/li";//"//div[@id='column_1']/ul/li[2]";
        public const string LogicRequestOverviewDivCss = "li.analytics_widget.logics.analytics_dashboard_widget";
        public const string FilterOptionsListCssLocator = "div.option_button_selector >div";
        public const string DashboardTitleXPathLocator = "//span[text()='{0}']";
        public const string FilterOptionsIconCssLocator = "li.dashboard_product_filter";
        //public const string FFPProductCssLocator = "ul.option_list>li:nth-of-type(2)>span.span_link";
        public const string FFPProductCssLocator = "div.option_button_selector >div:contains(FFP)";
        public const string COBProductCssLocator = "div.option_button_selector > div:contains(COB)";
        public const string PCIProductCssLocator = "div.option_button_selector >div:contains(Coding Validation)";
        public const string MyDashboardCssLocator = "div.option_button_selector >div:contains(My Dashboard)";
        public const string MicrostrategyDashboardCssLocator = "div.option_button_selector >div:contains(Microstrategy)";

        public const string LogicRequestOverviewDivHeaderCss =
            "li.ember-view.analytics_widget.logics.analytics_dashboard_widget > header >div >span.component_title.widget_title";
        public const string UnapprovedClaimsDivHeaderCss =
            "li.ember-view.analytics_widget.basic.analytics_dashboard_widget > header > div > span.component_title.widget_title";

        public const string UnapprovedClaimsGridDataCss = "ul.ember-view.component_item_list.unapproved_claim > li";
        public const string UnapprovedClaimsGridDataLoadMoreCss = "ul.ember-view.component_item_list.unapproved_claim > li>span";
        public const string UnapprovedClaimsGridClmSequenceListCss = "//label[text()='Claim Seq:']/../span";
        public const string UnapprovedClaimsGridAltClaimNumberListCss = "//label[text()='Client Code:']/../../li[2]/span";
        public const string UnapprovedClaimsGridClientCodeListCss = "//label[text()='Client Code:']/../span";
        public const string UnapprovedClaimsProductLabelCss =
            "li.ember-view.analytics_widget.basic.analytics_dashboard_widget > header > div.component_header_right > ul > div.widget_data_type >span";
        public const string UnapprovedClaimsExpandIconCss =
           "li.ember-view.analytics_widget.basic.analytics_dashboard_widget> header > div.component_header_right > ul > li >span.icon.toolbar_icon.expand_icon";
        public const string UnapprovedClaimsRefreshIconCss =
          "li.ember-view.analytics_widget.basic.analytics_dashboard_widget> header > div.component_header_right >ul >li >span.icon.toolbar_icon.reload_icon";

        public const string OverViewWidgetExpandIconTemplate = "li.analytics_widget div:contains({0})~div.component_header_right span.expand";
        public const string waitForWidgetLoadingCSSLocator = "li.analytics_widget div.medium_loading";
        public const string CollapseIconCssSelector = "span.icon.collapse";
        public const string ExportIconByCss = @"div.widget_column:has(span:contains({0})) span.download";
      



        //*[@id="ember2076"]/span


        public const string LogicRequestHighChart1Css = "g.highcharts-series.highcharts-tracker > path";
        public const string LogicRequestHighChart2Css = "div[data-highcharts-chart=\"1\"] svg";
        public const string LogicRequestHighChart3Css = "div[data-highcharts-chart=\"2\"]  svg";
        public const string LogicRequestHighchartYAxisTitleCss = "div[data-highcharts-chart=\"0\"]  svg > g.highcharts-axis-labels.highcharts-yaxis-labels";
        public const string LogicRequestHighchartYAxisLabelCss = "div[data-highcharts-chart=\"0\"]  svg > g.highcharts-axis > text";
        public const string LogicRequestHighchartValueCss = "div[data-highcharts-chart=\"0\"] div.highcharts-data-labels > div > span";
        public const string LogicRequestHighchart2YAxisTitleCss = "div[data-highcharts-chart=\"1\"] svg > g.highcharts-axis-labels.highcharts-yaxis-labels";
        public const string LogicRequestHighchart2YAxisLabelCss = "div[data-highcharts-chart=\"1\"]  svg > g.highcharts-axis > text";
        public const string LogicRequestHighchart2ValueCss = "div[data-highcharts-chart=\"1\"] div.highcharts-data-labels > div > span";
        public const string LogicRequestHighchart3YAxisTitleCss = "div[data-highcharts-chart=\"2\"] svg > g.highcharts-axis-labels.highcharts-yaxis-labels";
        public const string LogicRequestHighchart3YAxisLabelCss = "div[data-highcharts-chart=\"2\"]  svg  g.highcharts-axis > text";
        public const string LogicRequestHighchart3ValueCss = "div[data-highcharts-chart=\"2\"]  > div.highcharts-container > div.highcharts-data-labels > div > span";
        public const string AppealsOVerviewDivHeaderXPath = "//div[@id='analytics_content']/div[2]/ul/li[1]/header/div/span[@class='component_title widget_title']";

        public const string MainLabelBySectionHeaderXPathTemplate =
            "//li[.//span[text()='{0}']]//li[contains(@class,'main_label')]";
        public const string SubLabelBySectionHeaderXPathTemplate =
            "//li[.//span[text()='{0}']]//li[contains(@class,'main_label')]/span";

        public const string SecondaryValueBySectionHeaderXPathTemplate =
            "//li[.//span[text()='{0}']]//li[contains(@class,'secondary_value')]";

        public const string ContainerHeaderClaimsOverviewPHIXPath = "//div[@id='column_1']/ul[1]/li[1]//div[@class='widget_data_type']/span[text()='CV']";
        public const string ContainerHeaderClaimsOverviewFFPXPath = "//div[@id='column_1']/ul[1]/li[1]//div[@class='widget_data_type']/span[text()='FFP']";
        public const string ContainerHeaderWidgetOverviewCOBTemplate = "li.analytics_widget.cob.analytics_dashboard_widget.ember-view div.component_header_left:has(span:contains({0}))+div.component_header_right div:has(span:contains(COB)),li.analytics_widget.cob.analytics_detail_widget.ember-view div.component_header_left:has(span:contains({0}))~div.component_header_right div:has(span:contains(COB))";
        public const string ContainerHeaderAppealsOverviewPHIXPath = "//div[@id='column_2']/ul[1]/li[1]//div[@class='widget_data_type']/span[text()='CV']";
        public const string LastUpdatedTimeXPath = "//div[@id='column_1']/ul[1]/li[1]//div/span[@class='data_value refresh_date']";
        public const string ClaimsOverviewDataLableCountXPath = "//ul[@class='widget_data column_47']/li/ul/li[2]";
        public const string ClaimsOverviewDataLableXPath = "//ul[@class='widget_data column_47']/li[{0}]/ul/li[2]";
        public const string ClaimsOverviewSubDataLableCountXPath = "//ul[@class='widget_data column_47']/li/ul/li[4]";
        public const string ClaimsOverviewSubDataLableXPath = "//ul[@class='widget_data column_47']/li[{0}]/ul/li[4]";
        public const string ClaimsOverviewWidgetGridViewDataLabelCount = "div.claims_overview_center>ul";
        public const string ClaimsOverviewWidgetGridViewDataLabelCssSelectorTemplate =
            "div.claims_overview_center>ul:nth-of-type({0})>ul>li:nth-of-type({1})>label";
        public const string ClaimsOverviewWidgetGridViewTotalClaimCountCssSelectorTemplate =
            "div.claims_overview_center>ul:nth-of-type({0})>ul>li:nth-of-type({1})>span";

        public const string ClaimsOverViewWidgetClaimCountXpathSelectorTemplate =
            "//li[li[text()='{0}']]/span";
        public const string ClaimsOverViewWidgetTotalClaimCountXpathSelectorTemplate =
            "//ul[.//li[text()='{0}']]/li[contains(@class,'secondary_value')]";
        public const string ClaimsOverviewWidgetDataXPath = "//div[@id='column_1']/ul[1]/li[1]/div/ul";
        public const string ClaimsOverviewRefreshIconXPath = "//div[@id='column_1']/ul[1]/li[1]//li/span[text()='Refresh Widget']";
        public const string AppealsOverviewDataLableCountXPath = "//ul[@class='widget_data widget_25_col']/li/ul/li[2]";
        public const string AppealsOverviewDataLableCssLocator = "li.appeals.analytics_dashboard_widget>div>div >ul>li:nth-of-type({0})>ul>li>li.main_label";
        public const string AppealsOverviewDataValueXpath = "//li[text()='{0}']/following-sibling::span[1]";
        public const string AppealsOverviewSubDataLabelCssLocator = "li.appeals.analytics_dashboard_widget>div>div >ul>li:nth-of-type({0})>ul>li.secondary_value";
        public const string AppealsOverviewSubDataLableCountXPath = "//ul[@class='widget_data widget_25_col']/li/ul/li[4]";
        public const string DaysInColumnsCountXPath = "//ul[@class='calendar_items widget_cols widget_100_col']/li/ul/li[1]";
        public const string DaysInRowCssTemplate = "li.appeals div.inner_widget:nth-of-type(1) tbody>tr:nth-of-type({0})>td:nth-of-type(1)";
        public const string Next5DaysColumnHeaderCssLocator = "li.appeals div.inner_widget:nth-of-type(1) thead td";

        public const string Next5DaysValueCssLocator =
            "li.appeals div.inner_widget:nth-of-type(1) tbody>tr>td:nth-of-type(1) ~ td";
        public const string AppealsAssignedToEachAnalystXPath = "//span[text()='Appeals Assigned to Each Analyst for the Next 5 Days']";
        public const string UserFullNameInAppealsAssignedCssTemplate = "li.appeals div.inner_widget:nth-of-type(2) tbody>tr:nth-of-type({0})>td:nth-of-type(1)";
        public const string CountInAppealsAssignedCssTemplate = "ul.listed_calendar_items >li:nth-of-type({0})>span:nth-of-type(2)";
        public const string AppealsAssignedRowCountCssLocator = "li.appeals div.inner_widget:nth-of-type(2) tbody>tr>td:nth-of-type(1)";

        public const string ClaimsDetailExpandIconCssLocator = "li.claim span.expand";
        public const string COBClaimsOverViewExpandIconCssSelector="div.component_header_left:has(span:contains(Claims Overview))~div ul .expand";
        public const string AppealsDetailExpandIconCSS = "li.appeals span.expand";
        public const string LogicRequestsDetailExpandIconCssSelector = "li.logics span.expand";


        public const string AppealValueOnDayColumnCssLocator =
            "ul.calendar_items li:nth-of-type({0})>ul>li:nth-of-type({1})>div.item_value";
        public const string AppealCreatorAnalystCssLocator = "li.appeals div.inner_widget:nth-of-type(2) tbody>tr>td:nth-of-type(1)";
        public const string AppealCreatorAppealValueCssLocator = "li.appeals div.inner_widget:nth-of-type(2) tbody>tr>td:nth-of-type(2) ";

        public const string FFPClaimsOverviewDivMainDataCssTemplate = "li.claim.analytics_dashboard_widget li.main_value li:contains({0})+span";
        public const string COBWidgetOverviewDivMainDataCssTemplate = "li.cob.analytics_dashboard_widget li.main_value li:contains({0})+span";
        public const string FFPClaimsOverviewDivSecondaryDataCssTemplate = "li.claim.analytics_dashboard_widget li.large_analytics_data_point:nth-of-type({0}) li.secondary_value";
        public const string UnapprovedClaimsLoadMoreXpath = "//span[contains(normalize-space(),'(Load More)')]";
        public const string UnapprovedClaimsLoadMoreCssSelector = "ul.unapproved_claim span.load_more_data";

        //client dashboard 


        public const string ClaimOverviewMainValueCssTemplate = "li.claim ul.widget_data >li:nth-of-type({0}) >ul>li.main_value";
        public const string ClaimOverviewSecondaryValueCssTemplate = "li.claim ul.widget_data >li:nth-of-type({0}) >ul>li.secondary_value";

        public const string LogicRequestDetailClientExpandIconCssSelector =
            "li.logics span.expand";

        public const string MyDashboardWidgetCssLocatorTemplate = "span.widget_title:contains({0})";
        public const string MyDashboardOnRightWidgetCssLocatorTemplate = ".analytics_widget:has( span.widget_title:contains({0})) div.widget_data_type>span:contains(My Dashboard)";

        public const string MyDashboardTotalThisWeekCssTemplate =
            ".analytics_widget:has( span.widget_title:contains({0})) span.focus_value";

        public const string MyDashboardAvgperHourCssTemplate =
            ".analytics_widget:has( span.widget_title:contains({0})) li.secondary_value";
        public const string MyDashboardClaimsOrAppealThisWeekCssTemplate =
            ".analytics_widget span.main_label:contains({0})";
        public const string MyDashboardWeightedClaimsOrAppealThisWeekCssTemplate =
            ".analytics_widget li.main_label:contains({0})+span.focus_value";
        public const string MyDashboardSecondaryWidgetAvgPerHourCssTemplate =
            ".analytics_widget:has( span.widget_title:contains({0}))  li:nth-of-type({1}) span.secondary_label";
        public const string MyDashboardWidgetRefreshDataCssTemplate =
            ".analytics_widget:has( span.widget_title:contains({0})) span.refresh_date";
        public const string NextRefreshTimeCssSelector =
            "div.toolbar_text>span";

        public const string COBDashboardWidgetHeaderTemplate = "li.cob >header:contains({0})~div li.analytics_data_point li>li";
        public const string CurrentDashboardView = "div.option_button_selector div.is_selected";
        public const string DownloadIconCssSelector = "span.download";
        public const string COBWidgetDetailColumnsCssSelector = "div.component_content>table>thead>tr>td";
        public const string COBAppealsDetailColumnValuesCssSelectorTemplate = "div.component_content>table>tbody>tr:not(.total_row)>td:nth-of-type({0})";

        public const string COBAppealDetailsTotalValueCssSelectorTemplate =
            "div.component_content>table>tbody>tr.total_row>td:nth-of-type({0})";
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.Dashboard.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public DashboardPageObjects()
            : base(PageUrlEnum.Dashboard.GetStringValue())
        {
        }

        public DashboardPageObjects(string url)
            : base(url)
        {
        }

        #endregion

    }
}
