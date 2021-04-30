using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class ClaimSearchPageObjects : NewDefaultPageObjects
    {

        #region PRIVATE/PUBLIC FIELDS
        public const string SearchIconCssLocator = "span.icon.filter_icon";

        public const string ClaimSequenceInputXPath = "//input[@placeholder='Claim Sequence' or @placeholder='Bill Sequence']";
        public const string ClaimNoInputXPath = "//input[@placeholder='Claim No']";
        public const string OtherClaimNoInputXPath = "//input[@placeholder='Other Claim Number']";
        public const string FindXPath = "//button[text()='Find']";

        public const string NoMatchingRecordFoundCssSelector = "p.empty_search_results_message";
        public const string ClearLinkOnFindClaimSectionCssLocator = "div.current_viewing_searchlist span.span_link";
        public const string SearchResultRowCssLocator = "ul.component_item_list>li";
        public const string SideBarPannelCssLocator = "section.component_sidebar:not(.is_hidden)";

        public const string ClaimSequenceXPathTemplate = "//span[text()='{0}']";
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string ClaimSearchResultListCssTemplate = "ul.component_item_list>li>ul>li:nth-of-type({0})>span";

        public const string AppealCountByRowXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/li[{0}]//li[contains(@title,'This claim has')]/span";
        public const string ClaimSearchResultListLockForClaimXpathTemplate =
            "//li/span[text()='{0}' and @title='View in Claim Action']/ancestor::li//li[contains(@class,'lock')]";

        public const string SearchResultRowCssTemplate = " ul.component_item_list li:nth-of-type({0}) ul.component_item_row";
        public const string SearchResultRowByValueXpathTemplate = "//span[text()='{0}']/../../../ul[contains(@class,'component_item_row ')]";
        public const string PlanXpath= "//span[text()='Default Plan']";
        public const string ClaimTypeHXpath = "//span[text()='H']";
        public const string ClaimTypeUXpath = "//span[text()='U']";
        public const string MemberIdByXpath = "//label[text()= 'Mem ID:']/../span";
        public const string ProvXpath = "//label[text()= 'Prov:']/../span";
        public const string BatchIDXpath = "//label[text()='Batch:']/../span";
        public const string ClaimNoXpath = "//label[text()='Claim No:']/../span";
        public const string ClientBatchIDCssSelector = "section.component_content>div>ul>li>span";
        public const string LOBXpath = "//label[text()='LOB:']/../span";
        public const string ClaimSubStatusCssLocator = ".component_item_list ul.component_item_row> li:last-child>span";
        public const string AssignedToXpath = "//label[text()='Assigned To:']/../span";
        public const string ClaimStatusXPath = "//li[label[@title='Prov']]/following-sibling::li[3]/span";
        
        public const string ClientClaimStatusXpath = "//label[@title='Received']/../following-sibling::li[1]";
        public const string ClientPlanNameXpath = "ul.component_item_row>li:nth-of-type(5)";
        public const string ClientLOBNameXpath = "//label[text()= 'LOB:']/../span";
        public const string ClientReviewGroupXpath = "//span[text()='Permapend']";
        public const string ClientFormTypeXpath = "//label[text()= 'Form Type:']/../span";
        public const string ClientProvSeqXpath = "//label[text()= 'ProvSeq:']/../span";
        public const string ClientTINXpath = "//label[text()= 'TIN:']/../span";
        public const string ClientMemberIdXpath = "//label[text()= 'Member ID:']/../span";
        public const string ClientClaimSeqXpath = "//label[text()= 'Claim Seq:']/../span";
        public const string ClientBatchIDXpath = "//span[text()='LoadTestBatch']";
        public const string InternalUserClaimSeqCssLocator = ".component_item_list ul.component_item_row>li:nth-of-type(2)>span";
        public const string FlaggedClaimCssSelector = "ul.component_item_list >li:nth-of-type(1)>ul>li:nth-of-type(2)>span";
        public const string ClientUserClaimNumberCssLocator = "li.component_item:nth-of-type({0}) li.action_link span";
        public const string ClientUserClaimSubStatusCssLocator = ".component_item_list ul.component_item_row>li:nth-last-child(2)>span";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string ClientReceivedDateXpath = "//label[@title='Received']/../span";
        public const string FindClaimsIconCssLocator = "span.filter_icon";

        public const string SideBarWorklistTypeCarotCssLocator =
             ".component_header_right >ul>li.is_active span.options";
        public const string WorkListTypeSelectorXpathTemplate =
            "//ul[not(contains(@class,'is_hidden'))]/li/span[text()='{0}']";
        public const string SidebarHeaderXpathTemplate =
            "//label[text()='{0}']";

        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options >ul>li";
        public const string FilterOptionsIconCssLocator = "li.appeal_search_filter_options";
        public const string ClaimSearchResultByRowXPath = "ul.component_item_list>li:nth-of-type({0})>ul";

        public const string claimsearchResultWithClaseqByCss =
            "ul.component_item_list>li :has(li.component_data_point span:contains({0}))";
        public const string ClaimSeqInDetailSectionXpath = "//label[text()='Claim Seq:']/../span";
        public const string GetValueFromClaimDetailsJsCssSelector =
            "section.search_list:has( label:contains(Claim Details)) section.component_content ul:nth-of-type({0})>li:nth-of-type({1})";
        public const string PreviouslyViewedClaimsSequenceListXPath =
            "//header[text()='Previously Viewed Claims']/../ul/li/span[1]";
        public const string PreviouslyViewedClaimsSequenceLinkXPath =
            "(//header[text()='Previously Viewed Claims']/../ul/li/span)[{0}]";

        public const string PreviouslyViewedClaimsSequenceLinkByClaseqXPath =
            "//header[text()='Previously Viewed Claims']/../ul/li/span[text()='{0}']";

        public const string ClaimDetailsXPathTemplate = "//label[text()='{0}']/../span";
        public const string ClaimsearchExportByCss = ".print_icon";
        public const string DisabledExportIconCss = "li.is_disabled>span.print_icon";
        public const string EnabledExportIconCss = "li.is_active>span.print_icon";
        public const string ClaimSearchResultListPrecedingProvSeqXPath = "(//label[@title='Prov']/..)/preceding-sibling::li[{0}]/span";
        public const string ClaimSearchResultListFollowingProvSeqXPath = "(//label[@title='Prov']/..)/following-sibling::li[{0}]/span";
        public const string ClaimSearchResultListForMemIdXPath = "//label[@title='Mem ID']/following-sibling::span";
        public const string ProvSeqListInClaimSearchPageCssSelector = "label[title='Prov']+span";
        //public const string LastLiOfDropDownListXPathByLabel = "//label[text()='Flag']/../section//ul//li[last()]";
        public const string LastLiOfDropDownListXPathByLabel = "label:contains(Flag) section ul li:last-of-type";
        public const string LastLiOfResultSetCssSelector = "section.component_left  section.component_content li";
        public const string ClaimViewRestrictionIconCssSelectorByLineNum = "section.component_left ul.component_item_list>li:nth-of-type({0}) li.claim_restrictions_small";

        public const string GridDataWithAppealCountCssSelectorTemplate =
            "ul.component_item_list>.component_item>ul:has(li.secondary_badge)>li:nth-of-type({0})";
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.NewClaimSearch.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ClaimSearchPageObjects()
            : base(PageUrlEnum.NewClaimSearch.GetStringValue())
        {
        }
        #endregion
    }
}
