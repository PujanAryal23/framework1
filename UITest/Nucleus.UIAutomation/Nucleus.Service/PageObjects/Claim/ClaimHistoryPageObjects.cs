using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;


namespace Nucleus.Service.PageObjects.Claim
{
    public class ClaimHistoryPageObjects : NewDefaultPageObjects
    {
        #region PUBLIC FIELDS

        public const string ClaimHistoryIconByName = "span.icon[title=\"{0}\"]";
        public const string PreAuthId = "pre_auth";
        public const string AuthSeqLink = "//span[normalize-space()='Auth Seq']/following-sibling::span/a";
        //public const string WorkingAjaxMessageCssLocator = "div.small_loading:not([style*='none'])";
        public const string SelectedQuadrantJQuerySelector = "g:has(g.selected)";
        public const string DentalHistoryDataCssSelector = "tbody>tr.dynamic_row:nth-of-type({0})>td:nth-of-type({1})";
        public const string ProviderHistoryTabCssSelector = "span#history_provider";
        public const string PatientDxTabCssSelector = "span#patient_DX";
        public const string DentalTabCssSelector = "span#patient_DCI";
        public const string SameDayTextLabelCssSelector = "div#history_same";
        public const string SixtyDaysTextLabelCssSelector = "div#history_60";
        public const string TwelveMonthsTextLabelCssSelector = "div#history_12";
        public const string AllHistoryTextLabelCssSelector = "div#all_history";
        public const string AltClaimNoXPathTemplate = "//td[div[3]/span[2]/a[text()='{0}']]/div[2]/span[2]";
        public const string ProviderHistoryTextLabelId = "div#history_provider";//"ctl00_MainContentPlaceHolder_history_provider";
        public const string ProviderSequenceTextLabelId = "ctl00_MainContentPlaceHolder_ProviderSequence";
        public const string TinHistoryTextLabelId = "tin_history";//"ctl00_MainContentPlaceHolder_lbl_tin_history";
        public const string TinHistoryRadioButtonInExtendedPageId = "ctl00_MainContentPlaceHolder_TinHistoryRbtn";
        public const string PatientDxRadioButtonId = "ctl00_MainContentPlaceHolder_Label7";//"patient_DX";//"ctl00_MainContentPlaceHolder_Label7";
        public const string PatientDxRadioButtonInExtendedPageId = "ctl00_MainContentPlaceHolder_PatientDxRbtn";
        public const string RadAjaxLoading = "div.rtLoading";
        public const string DxCodeWithIcdXPathTemplate =
            "//div[@id='view_port']/div/table[{0}]/tbody/tr[{1}]/td[5]/span";
        public const string DxCodeInPopupXPath =
            "//div[@class='info_tool_tip']/div[@class='tip_content']/div/table/tbody/tr[{0}]";
        public const string ExtendedClaimHistoryId = "claimHistoryLink";
        public const string DxCodeWithIcdExtendedClaimHistoryXPath =
            "//tr[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00__{0}']/td[7]/span";
        public const string IcdDxCodeExtendedClaimHistoryXPathTemplates =
            "//table[@id='ctl00_MainContentPlaceHolder_ctl02_grViewDX_ctl00']/tbody/tr[{0}]";
        public const string TableRowFieldCssTemplate =
            "table.nucleus_table:nth-of-type({0}) >tbody>tr:nth-of-type({1})>td:nth-of-type({2})>span";
        public const string TableRowLinkFieldCssTemplate =
            "table.nucleus_table:nth-of-type({0}) >tbody>tr:nth-of-type({1})>td:nth-of-type({2}) a";
        public const string RevCodeXpathTemplateByRevCode = "//a[text()='{0}']";
        public const string ToolTipHeaderCssLocator = "div.info_tool_tip>div.tip_header>span";
        public const string FlagLinkOnToolTipHeaderCssLocator = "div.info_tool_tip>div.tip_header>a";
        public const string ToolTipContentCssTemplate = "div.info_tool_tip>div.tip_content>div>li:nth-of-type({0})";
        public const string ToolTipTableContentHeaderCssTemplate =
            "div.info_tool_tip>div.tip_content>div>table>thead>tr>th:nth-of-type({0})";
        public const string LoadingAjaxMessageCssLocator = "div.tooltipster-content";
        public const string ProviderToolTipCssLocator =
            "table.nucleus_table:nth-of-type({0}) >tbody>tr.summary_row>td>div:nth-of-type(1)>span";
        public const string ClaimSequenceXPathTemplate = "//a[text()='{0}']";
        public const string AppealIconByClaimSeqXPathTemplate =
            "//table[contains(@class,'highlight')][//a[text()='{0}']]/tbody/tr[1]/td[@class='flags']/span[contains(@class,'flag_icon o')]";
        public const string PatientClaimHistoryDentalDataPointsRowXPathTemplate = "(//table[contains(@class,'nucleus_table')]/thead/tr)[{0}]";
        public const string PatientClaimHistoryDentalDataPointsCssSelector = "table.dental>thead>tr>th";
        public const string PatientClaimHistoryPreAuthLabelCssSelector = "table.nucleus_table>thead>tr>th";
        public const string DentalDOSXPath = "//table[contains(@class,'nucleus_table')]//td[2]";
        public const string DentalDosCssSelector = "table.nucleus_table tbody tr.dynamic_row td:nth-of-type(2)";
        public const string DentalDataPointValues = "(//table[contains(@class,'dental')]//tr)[2]//td";
        public const string ProcDescTooltipCssSelector = "div.tooltipster-default>div>div>div>li";
        public const string DentalDataPointsPointerXPathTemplate = "//table[contains(@class,'dental')]//td[{0}]";
        public const string HistoryTableCssSelector = "table.nucleus_table";
        public const string PatClaimHistoryDentalIconCssSelector = "ul.SubNavPopupRight>li>span.dental_button";
        public const string DentalChartCssSelectorTemplate = "div.{0}";
        // public const string DentalChartSelectorButtonTemplate = "div#tooth_chart_selector>div>div:nth-of-type({0})";
        public const string DentalChartSelectorButtonTemplate = "//div[text()='{0}']";
        public const string QuadrantSelectionCssSelectorTemplate = "div.{0}>svg>g#{1}>g";
        public const string QuadrantClassSelectionCssSelector = "g#{0}.selected";
        public const string SelectedQuadrantCssSelector = "g.quadrant.selected";
        public const string IndividualURToothSelectorTemplate = "div.{0}>svg>g#{1}>g:nth-of-type({2})";
        public const string ToothNumberTextCssSelector = "svg.{0}>g#{1}>g.tooth_group.selected>text";
        public const string ToothNumberSelectorCssSelector = "g.tooth_group.selected";
        public const string DentalResetButtonCssSelector = "div.dental_reset";
        public const string EmptyHistoryDetailCssSelector = "tr:not([style*='none']).empty_row>td";
        

        public const string DentalDataTableCssSelector = "table.nucleus_table>tbody>tr:nth-of-type(1):not([style*='none'])>td";
        public const string ToothListByQuadrant = "svg.{0}>g#{1}>g";

        public const string HistoryTabsByNameCssLocator = "div.header_selector_option:contains({0})";
        public const string HistoryOptionListCssLocator = "div#inner_content_header>div.header_selector>div";
        public const string PatientHistoryHeaderXpathTemplate = "//label[@id='{0}']";
        public const string EmptymessageCssSelector = "p.empty_message";
        public const string RedBadgeCssSelector = "span.preauth_button>span.icon_badge";
        public const string EightMonthsHistoryCssSelector = ".header_selector_option:contains(8 Months)";
        public const string AllDosFromPatientClaimHxCssSelector = "span[edos]";

        public const string HugeDataLoadErrorMessageCssSelector = "div.error_message";
        public const string CloseHugeDataLoadMessageSectionCssSelector = "div.error.close_icon";

        public const string ClaimHistoryClaimSequenceCssSelector =
            "div.patient_claim_history_content>table tbody>tr.summary_row>td>div>span:has(span:contains(Claim))+ span.bold>a";

        public const string ClaimHistoryDosXPathTemplate =
            "//div[contains(@class,'patient_claim_history_content')]/table//tbody//tr[contains(@class,'summary_row')]//a[@title='{0}']/../../../../..//tr[{1}]/td[2]/span";

        public const string ColumnNameByRowXPathTemplate =
            "//div[contains(@class,'patient_claim_history_content')]//table[{1}]//th[contains(text(),'{0}')]";

        public const string PatientPreAuthHistoryRowAndColumnTemplate =
            "//div[contains(@class,'patient_claim_history_content')]//table[{0}]//tbody//tr[1]//td[{1}]";

        public const string PatientPreAuthHistoryDiv = "div.patient_claim_history_content";

        public const string PatientPreAuthHistoryTooltipCssSelector = "div.tooltipster-default div.tip_content";


        #endregion

        #region NewFilterLinesPatientClaimHistory
        public const string DxCodeInputCssSelector = "div.header_filters input.dxcode";
        public const string ProcCodeInputCssSelector = "div.header_filters input.proccode";
        public const string DOSInputCssSelector = "div.header_filters input.dos";
        public const string FilterButtonCssSelector = "div.header_filters button.filter_button";
        public const string ClearButtonCssSelector = "div.header_filters button.clear_button";
        public const string TablePresentCssSelector = "div.patient_claim_history_content table.nucleus_table";
        public const string AllDxCodesFromPatientClaimHxCssSelector = "table:not([style*='none']) tr:not([class*='filtered']) td.dxcode";
        public const string AllProcCodesFromPatientClaimHxCssSelector = "table:not([style*='none']) tr:not([class*='filtered']) td.proccode";
        public const string AllDOSActiveFromPatientClaimHxCssSelector = "table:not([style*='none']) tr:not([class*='filtered']) td.dos";
        public const string TableHeaderCssSelector = "table:not([style*='none']) thead";
        public const string TableFooterCssSelector = "table:not([style*='none']) tr.summary_row";
        #endregion


        #region PROTECTED PROPERTIES

        //public override string PageTitle => AssignPageTitle;
        public static string AssignPageTitle= PageTitleEnum.ExtendedPageClaimHistory.GetStringValue();

        public override string PageTitle
        {
            get { return AssignPageTitle; }
        }

      
        #endregion

        #region CONSTRUCTOR

        public ClaimHistoryPageObjects()
            : base(PageUrlEnum.ClaimPatientHistory.GetStringValue())
        {
        }

        public ClaimHistoryPageObjects(string url):base(url)
        {
        }

        #endregion
    }
}
