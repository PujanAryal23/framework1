using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Dashboard
{
    public class AppealsDetailPageObjects : NewPageBase
    {
        #region FIELDS
        public const string DashboardIconXPath = "//a[text()='Dashboard']";
        public const string AppealsDetailPageheaderXPath = "//div[@id='analytics_header']/div/label";
        public const string AppealsDetailContainerHeadersCSS = "li.appeal:nth-of-type({0}) div.component_header_left span.component_title";
        public const string FirstRowColumnHeaderCountXPath = "//ul[@class='widget_data widget_cols column_60']/li/ul/li[1]";
        public const string SecondRowColumnHeaderCountXPath = "//ul[@class='widget_data widget_cols column_60']/li/ul/li[3]";
        public const string FirstRowColumnHeaderCSS = "ul.widget_data.widget_cols.column_80 li:nth-of-type({0}) ul li:nth-of-type(1)";
        public const string SecondRowColumnHeaderCSS = "ul.widget_data.widget_cols.column_80 li:nth-of-type({0}) ul li:nth-of-type(3)";
        public const string ActiveClientsCountXPath = "//div[@id='analytics_content']/ul/li[1]//div[@class='component_content']/div[2]/ul/li/div[1]";
        public const string ActiveClientXPath = "//div[@id='analytics_content']/ul/li[1]//div[@class='component_content']/div[2]/ul/li[{0}]/div[1]";
        public const string ColumnValuesXPath = "//div[@class='analytics_details claim_summary_analytics_details widget_cols widget_100_col']/ul/li[{0}]/div[2]/span[{1}]";
        public const string ClaimsByClientRowCountXPath = "//div[@class='analytics_details claim_summary_analytics_details widget_cols widget_100_col']/ul/li";
        public const string BoxWithArrowIconInAppealsByClientXPath = "//span[text()='Appeals by Client']/../../..//span[text()='Print Details']";
        public const string DownloadPDFLinkInAppealsByClientXPath = "//header[.//span[text()='Appeals by Client']]//span[contains(@class,'download')]";
        public const string BoxWithArrowIconInAppealsByAnalystXPath = "//span[text()='Appeals by Analyst']/../../..//span[text()='Print Details']";
        public const string DownloadPDFLinkInAppealsByAnalystXPath = "//header[.//span[text()='Appeals by Analyst']]//span[contains(@class,'download')]";
        public const string ClientValuesXPath = "//div[@id='analytics_content']/ul/li[1]//ul[@class='widget_scroller widget_cols column_100 listed_items']/li[{0}]/div[2]/span[{1}]";
        public const string ClientValuesRowCountXPath = "//div[@id='analytics_content']/ul/li[1]//ul[@class='widget_scroller widget_cols column_100 listed_items']/li/div[2]";
        public const string ClientValuesColumnCountCss = "div#analytics_content li.analytics_widget.appeal.analytics_detail_widget:nth-of-type(1) div.analytics_detail_header>ul>li";
        public const string CotivitiUserCountWithAppealsAssignAuthXPath = "//div[@id='analytics_content']/ul/li[2]//ul[@class='widget_scroller widget_cols column_100 listed_items']/li/div[1]";
        public const string CotivitiUserWithAppealsAssignAuthXPathTemplate = "//div[@id='analytics_content']/ul/li[2]//ul[@class='widget_scroller widget_cols column_100 listed_items']/li[{0}]/div[1]/span[1]";
        public const string CotivitiUserWithAppealsAssignAuthCssLocator = "span.analyst_name_detail";
        public const string OverdueAppealYellowIconXPath = "//div[@id='analytics_content']/ul/li[2]//ul[@class='widget_scroller widget_cols column_100 listed_items']/li[{0}]/div[1]/span[2]";
        public const string FirstRowColumnHeaderAppealsByAnalystXPath = "//div[@id='analytics_content']/ul/li[2]//div[contains(@class,'analytics_detail_header')]/ul/li[{0}]/ul/li[1]";
        public const string SecondRowColumnHeaderAppealsByAnalystXPath = "//div[@id='analytics_content']/ul/li[2]//div[@class='analytics_detail_header widget_cols column_100 widget_data']/ul/li[{0}]/ul/li[3]";
        public const string SecondRowColumnHeaderCountAppealsByAnalystXPath = "//div[@id='analytics_content']/ul/li[2]//div[@class='analytics_detail_header widget_cols column_100 widget_data']/ul/li/ul/li[3]";
        public const string TotalAppealsCountInAppealsCountByAnalystXpath = "//div[@id='analytics_content']/ul/li[2]//div[contains(@class,'analytics_detail_header')]/ul/li[2]/ul/li[3]";

        public const string YellowIconUserListJQuery = "div:has(>span.warning_icon)>span.analyst_name_detail";
        //appeal over page details
        public const string FirstRowColumnHeaderValueAppealByClientCssTemplate =
            "div#analytics_content>ul>li:nth-of-type(1)  div.analytics_detail_header>ul>li:nth-of-type({0})>ul>li:nth-of-type(1)";

        public const string SecondRowColumnHeaderYesterdayValueAppealByClientCssTemplate =
            "div#analytics_content>ul>li:nth-of-type(1)  div.analytics_detail_header>ul>li:nth-of-type({0})>ul>li:nth-of-type(3)";

        public const string FirstRowColumnHeaderAppealByClientCssLocator =
            "div#analytics_content>ul>li:nth-of-type(1)  div.analytics_detail_header>ul>li>ul>li:nth-of-type(1)";

        public const string AppealByClientValueCssTemplate =
            "div#analytics_content>ul>li:nth-of-type(1)  div.analytics_details>ul>li:nth-of-type({0}) >div:nth-of-type(2) > span:nth-of-type({1})";

        public const string AppealByClientRowCssLocator =
            "div#analytics_content>ul>li:nth-of-type(1)  div.analytics_details>ul>li";

        public const string AppealByAnalystNameCssLocator =
            "div#analytics_content>ul>li:nth-of-type(2) ul.listed_items>li>div:nth-of-type(1)>span:nth-of-type(2)";

        public const string AppealByAnalystRowCssLocator = "div#analytics_content>ul>li:nth-of-type(2) ul.listed_items>li";

        public const string AppealByAnalystAppealValueCssTemplate =
            "div#analytics_content>ul>li:nth-of-type(2) ul.listed_items>li:nth-of-type({0})>div:nth-of-type(2)>span:nth-of-type({1})";

        public const string AppealByAnalystColumnCssLocator =
            "div#analytics_content>ul>li:nth-of-type(2) div.analytics_detail_header>ul>li";

        public const string SecondRowHeaderValueAppealByAnalystCssTemplate =
            "div#analytics_content>ul>li:nth-of-type(2) div.analytics_detail_header>ul>li:nth-of-type({0})>ul>li.secondary_value";

        public const string AppealByAnalystSectionCssLocator = "div#analytics_content>ul>li:nth-of-type(2)";
        


        #endregion
        
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.DashboardAppealsDetail.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region CONSTRUCTOR

        public AppealsDetailPageObjects()
            : base(PageUrlEnum.ClaimsDetail.GetStringValue())
        {
        }

        public AppealsDetailPageObjects(string url)
            : base(url)
        {
        }

        #endregion
    }
}
