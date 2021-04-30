using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Dashboard
{
    public class ClaimsDetailPageObjects : NewDefaultPageObjects
    {
        #region FIELDS
        public const string ClaimsDetailHeaderXPath = "//div[@id='analytics_content']/li//span[@class='component_title widget_title']";
        public const string DashboardIconXPath = "//a[text()='Dashboard']";
        public const string FirstRowColumnHeaderCountXPath = "//span[text()='Claims by Client']/../../..//ul[contains(@class,'widget_data')]/li/ul/li[1]";
        public const string SecondRowColumnHeaderCountXPath = "//span[text()='Claims by Client']/../../..//ul[contains(@class,'widget_data')]/li/ul/li[3]";
        public const string FirstRowColumnHeaderXPath = "//span[text()='Claims by Client']/../../..//ul[contains(@class,'widget_data')]/li[{0}]/ul/li[1]";
        public const string SecondRowColumnHeaderXPath = "//span[text()='Claims by Client']/../../..//ul[contains(@class,'widget_data')]/li[{0}]/ul/li[3]";
        public const string ClientValuesRowCountXPath = "//span[text()='Claims by Client']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[2]";
        public const string ClientValuesXPath = "//span[text()='{2}']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li[{0}]/div[2]/span[{1}]";
        public const string ActiveClientsCountXPath = "//span[text()='{0}']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[1]/span";
        public const string ActiveClientXPath = "//div[text()='{0}']/../../../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[1]/span[contains(@class, 'client_name_detail')]";
        public const string ActiveClientClaimsByClientXPath = "//span[text()='{0}']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[1]/span[contains(@class, 'client_name_detail')]";
        public const string ColumnValuesXPath = "//span[text()='{1}']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[2]/span[{0}]";
        public const string ClaimsByClientRowCountXPath = "//span[text()='Claims by Client']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li";
        //public const string BoxWithArrowIconXPath = "//span[text()='Claims by Client']/../../..//span[text()='Print Details']";
        public const string DownloadPDFLinkXPath = "//header[.//span[text()='Claims by Client']]//span[contains(@class,'download')]";
        public const string SpinnerWrapperXPath = "//div[contains(@class,'spinner_wrap large_spineer')]";

        public const string GetClientListFromByWidgetLabelXPath =
            "//span[text()='{0}']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li/div[1]";
        //client dashboard claim detail page
        public const string UnreviewedClaimswDivMainLabelCssSelector =
            "div[class^='analytics_detail_header'] > ul > li > ul > li";
        public const string UnreviewedClaimswDivSecondaryLabelCssSelector =
            "div[class^='analytics_detail_header'] > ul > li > ul > li.secondary_value";
        public const string PendedClaimswDivMainLabelCssSelector =
              "div[class^='analytics_detail_header'] > ul > li:nth-of-type(2) > ul > li";
        public const string PendedClaimswDivSecondaryLabelCssSelector=
             "div[class^='analytics_detail_header'] > ul > li:nth-of-type(2) > ul > li.secondary_value";

        public const string UnapprovedClaimswDivMainLabelCssSelector =
            "div[class^='analytics_detail_header'] > ul > li:nth-of-type(3) > ul > li";
        public const string UnapprovedClaimswDivSecondaryLabelCssSelector =
            "div[class^='analytics_detail_header'] > ul > li:nth-of-type(3) > ul > li.secondary_value";

        public const string ClaimByClientDetailRowCountCssSelector =
             "div[class*='claim_summary_analytics_details'] > ul > li > div:nth-of-type(1) >span";
        public const string ClaimByClientDetailRowValueCssSelector =
           "div[class*='claim_summary_analytics_details'] > ul > li:nth-of-type({0}) > div >span";
        public const string ClaimByClientDetailAllColumnDataCssSelectorTemplate=
             "div[class*='claim_summary_analytics_details'] > ul > li:nth-of-type({0}) > div:nth-of-type(2) >span:nth-of-type({1})";

        public const string ClaimsDetailContainerHeadersXPath = "//div[@id='analytics_content']/ul/li//div[contains(@class,'option_button is_selected')]";
        public const string RealTimeClaimsColumnHeaderCountXPath = "//span[text()='Real Time Claims']/../../..//ul[contains(@class,'widget_data')]/li/ul/li[1]";
        public const string RealTimeClaimsColumnHeaderXPath = "//div[text()='Real Time Claims']/../../../../..//ul[contains(@class,'widget_data')]/li";
        public const string RealTimeColumnValuesXPath = "//div[text()='Real Time Claims']/../../..//div[contains(@class,'claim_summary_analytics_details')]/ul/li[{0}]/div[2]/span[{1}]";
        public const string RealTimeClaimsDownloadPDFLinkXPath = "//header[.//div[text()='Real Time Claims']]//span[contains(@class,'download')]";
        public const string RealTimeClaimsBoxWithArrowIconXPath = "//div[text()='Real Time Claims']/../../..//span[text()='Print Details']";
        public const string LessThan2HrXpath = "//h6[text()=' < 2 Hr']";
        public const string RealTimeWidgetXPathTemplate = "//div[text()='{0}']";

        public const string OverallUnreviewedClaimsForRealTimeClientCssSelector =
            "li.analytics_widget:has(div.option_button:contains(Real Time Claims)) div.analytics_details:has(.client_name_detail:contains({0})) li:has(span.client_name_detail:contains({0})).component_item_row>div:nth-of-type(2) span";
        public const string CaretSignForRealTimeClientByClientNameCssSelector =
            "li.analytics_widget:has(div.option_button:contains(Real Time Claims)) div.analytics_details li:has(.client_name_detail:contains({0})) .small_caret_right_icon";
        public const string ClaimsByRestrictionInRealTimeClientByClientNameCssSelector =
            "li.analytics_widget:has(div.option_button:contains(Real Time Claims)) div.analytics_details:has(.client_name_detail:contains({0})) li:has(span.client_name_detail:contains({0})) div.component_sub_row span";

        public const string ClaimRestrictionsForRealTimeClientCssSelector =
            "li.analytics_widget:has(div.option_button:contains(Real Time Claims)) div.analytics_details:has(.client_name_detail:contains({0})) li:has(span.client_name_detail:contains({0})) div.component_sub_row .restriction_name";

        public const string ClaimsDetailRealTimeClaimsDataHeaderCssSelector = "div.component_content.single_column>ul>li div.analytics_detail_header li";

        #endregion

        #region PAGEOBJECT PROPERTIES

        #region ID

        #endregion

        #region XPATH





        [FindsBy(How = How.XPath, Using = DownloadPDFLinkXPath)]
        public Link DownloadPDFLink;

        

        [FindsBy(How = How.XPath, Using = RealTimeClaimsDownloadPDFLinkXPath)]
        public Link RealTimeClaimDownloadPDFLink;

        #endregion

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            //get { return PageTitleEnum.DashboardClaimsDetail.GetStringValue(); }
            get { return AssignPageTitle; }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        public static string AssignPageTitle;

        #endregion

        #region CONSTRUCTOR

        public ClaimsDetailPageObjects()
            : base(PageUrlEnum.ClaimsDetail.GetStringValue())
        {
        }

        public ClaimsDetailPageObjects(string url)
            : base(url)
        {
        }

        #endregion
    }
}
