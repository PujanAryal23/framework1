using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Settings
{
    public class MaintenanceNoticesPageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR

        public MaintenanceNoticesPageObjects()
            : base(PageUrlEnum.MaintenanceNotices.GetStringValue())
        {
        }

        #endregion


        #region calender

        public const string MonthCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-lendar select.pika-select-month";

        public const string MonthValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// select[contains(@class,'pika-select-month')]/option[text()='{0}']";

        public const string YearCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-lendar select.pika-select-year";

        public const string YearValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// select[contains(@class,'pika-select-year')]/option[text()='{0}']";

        public const string DayValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// table[contains(@class,'pika-table')]//td/button[text()='{0}']";

        public const string HourCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-timepicker select.pika-select-hours";
        public const string HourValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-timepicker')]/select[contains(@class,'pika-select-hours')]/option[text()='{0}']";
        
        public const string MinuteCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-timepicker select.pika-select-minutes";
        public const string MinutesValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-timepicker')]/select[contains(@class,'pika-select-minutes')]/option[text()='{0}']";


        #endregion calender

        #region PRIVATE/PUBLIC FIELDS

        public const string ScheduleMaintenanceNoticeSectionXPath =
            "//section[form/header[.//text()='Schedule Maintenance Notice']]";
        public const string PreviewMessageCssLocator = "div.data_point_area ";
        public const string PreviewMessageDateTimeCssLocator = "div.data_point_area >b";
        public const string GridRowCssLocator = "section.search_list ul.component_item_list > li.component_item";
        public const string NotificationRowCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul";
        public const string RightsidePanelCssTempalte = "section.search_list ul.component_item_maintenance_notice";
        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        public const string ActiveNotificationCountCssLocator= "section.search_list ul.component_item_list > li > ul > span.active_badge >span";
        public const string PopupModalContentDivId = "nucleus_modal_content";
        public const string PopupModalCloseId = "nucleus_modal_close";
        public const string PopupModalId = "nucleus_modal_wrap";
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string AddNoticeCssSelector = "a.toolbar_icon.add_icon";
        public const string DisplayDateRangeFromCssSelector ="section.component_left form>ul:nth-of-type(2)>div:nth-of-type(1)>input";
        public const string DisplayDateRangeToCssSelector = "section.component_left form>ul:nth-of-type(2)>div:nth-of-type(3)>input";
        public const string MaintenanceDateRangeFromCssSelector = "section.component_left form>ul:nth-of-type(4)>div:nth-of-type(1)>input";
        public const string MaintenanceDateRangeToCssSelector = "section.component_left form>ul:nth-of-type(4)>div:nth-of-type(3)>input";
        public const string ScheduleButtonCssSelector = "button.work_button";
        public const string CancelScheduleCssSelector = "div.form_buttons >span>span";
        public const string AddScheduleHeaderLabelCssSelector = "header.form_header ";
        public const string EditDefaultMessageCssLocator = "form li.different_dates.action_link>span";
       

        public const string ThreeBusinessDayLabelCssSelector = "section.component_left form>ul:nth-of-type(1) label";

        public const string DiplayDateRangelabelCssSelector =
            "section.component_left form>label:nth-of-type(1) >span.data_point_label";
        

        public const string EditFormCssLocator = "section.component_nested_form";

       /* public const string DisplayDateRangeToolTipCssLocator =
           "section.component_nested_form>form>label:nth-of-type(1)>span:nth-of-type(3)";*/
       /* public const string MaintenanceDateRangeToolTipCssLocator =
           "section.component_nested_form>form>label:nth-of-type(1)>span:nth-of-type(3)";*/
        //public const string ExclamationIconNoteInEditSectionCssLocator =
        //    "section.component_nested_form>form>ul>div>label:nth-of-type(2)>span.small_icon.field_error";
        public const string DisplayDateRangeFromOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(1) input";

        public const string DisplayDateRangeToOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(3) input";

        public const string MaintenanceDateRangeFromOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(4)> div:nth-of-type(1) input";
        public const string MaintenanceDateRangeToOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(4)> div:nth-of-type(3) input";
        public const string EditMessageOnEditSectionFieldCssLocator =
           "section.component_left form>ul:nth-of-type(3)>div>li:nth-of-type(1)>span";
        public const string DefaultMessageOnEditSectionFieldCssLocator =
           "section.component_left form>ul:nth-of-type(3)>div>li:nth-of-type(2)>span";

        public const string SaveButtonCssLocator = "form>ul:nth-of-type(6) button.work_button";
        public const string CancelButtonCssLocator = "form>ul:nth-of-type(6) span.span_link";

        #region ID

        public const string MaintenancenoticeContainerCssLocator = "div#notice-container > div.maintenance_notice";

        public const string MaintenanceContainerNoticeDescriptionCssLocator =
            "div#notice-container > div.maintenance_notice  ";//not(:last-child)";
        public const string MaintenanceContainerTimeFrameCssLocator =
            "div#notice-container > div.maintenance_notice>p>b";
        #endregion

        #region values
        public const string DisplayValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(2) > span"; //date range
        public const string MaintenanceValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(3) > span";
        public const string LastModifiedUserValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(4) > span";
        public const string LastModifiedDateValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(5) > span";
        public const string PageInsideTitleCssLocator = "label.page_title";

        public const string MaintenanceNoticesRowColValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul>li:nth-of-type({1}) span";
        public const string DisplayValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(2) > span";
        public const string MaintenanceValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(3) > span";
        public const string LastModifiedUserValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(4) > span";
        public const string LastModifiedDateValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(5) > span";
        public const string NotificationDateRangePanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(1).component_data_point>.data_point_value";
        public const string MaintenanceDateRangePanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(2).component_data_point>.data_point_value";
        public const string LastModifiedUserPanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(3).component_data_point>.data_point_value";
        public const string LastModifiedDatePanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(4).component_data_point>.data_point_value";
        public const string NotificationStatusPanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(5).component_data_point>.data_point_value";
        public const string NotificationMessagePanelValueCssTemplate =
            "section.search_list ul.component_item_maintenance_notice> div.data_point_area";

        #endregion

        #region templates

        //public const string ExclamationIconCssTemplate =
        //    "section.form_component >form>label:nth-of-type({0})>span.small_icon.field_error";
        //public const string ExclamationIconInEditSectionCssTemplate =
        //    "section.component_nested_form>form>label:nth-of-type({0})>span.small_icon.field_error";

        public const string MaintenanceNoticesRowCssLocator = "section.search_list ul.component_item_list > li > ul";
        public const string MaintenanceNoticesRowCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul";
        public const string EditIconEnabledCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_edit_icon.is_active";
        public const string EditIconEnabledCssLocator = "span.small_edit_icon.is_active";
        public const string DeleteIconEnabledCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_delete_icon.is_active";
        public const string EditIconDisabledCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_edit_icon.is_disabled";
        public const string DeleteIconDisabledCssLocator = "span.small_delete_icon.is_disabled";
        public const string DisplayLabelCsssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(2) > label";
        public const string MaintenanceLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(3) > label";
        public const string LastModifiedUserLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(4) > label";
        public const string LastModifiedDateLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(5) > label";
        public const string NotificationStatusCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > span.value_badge >span";
        public const string ActiveNotificationStatusCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > span.active_badge >span";
        public const string CancelledNotificationStatusCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > span.cancelled_badge >span";
        public const string InactiveNotificationStatusCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > span.inactive_badge >span";

        public const string EditIconCssTemplate = "li:nth-of-type(1) >ul>li>ul> li>span.small_edit_icon";
        public const string DeleteIconCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_delete_icon";

        public const string NotificationDateRangePanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(1).component_data_point>.data_point_label";
        public const string MaintenanceDateRangePanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(2).component_data_point>.data_point_label";
        public const string LastModifiedUserPanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(3).component_data_point>.data_point_label";
        public const string LastModifiedDatePanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(4).component_data_point>.data_point_label";
        public const string NotificationStatusPanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(5).component_data_point>.data_point_label";
        public const string NotificationMessagePanelCssTemplate =
            "section.search_list ul.component_item_maintenance_notice li:nth-of-type(6).component_data_point>.data_point_label";

        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES


        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.MaintenanceNotices.GetStringValue(); }
        }

        //[FindsBy(How = How.Id, Using = PopupModalCloseId)]
        //public ImageButton PopupModalCloseButton;

        //[FindsBy(How = How.Id, Using = PopupModalCloseId)]
        //public ImageButton PageErrorCloseButton;

        //[FindsBy(How = How.CssSelector, Using = OkConfirmationCssSelector)]
        //public Link OkConfirmationLink;

        //[FindsBy(How = How.CssSelector, Using = CancelConfirmationCssSelector)]
        //public Link CancelConfirmationLink;

      

        

        #endregion

    }
}
