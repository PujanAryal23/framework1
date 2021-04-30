using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.PageObjects.Settings
{
    public class MicrostrategyMaintenancePageObjects : NewDefaultPageObjects
    {

        #region PRIVATE
        public const string QuadrantTitlesCssTemplate = "label.component_title";
        public const string AddIconXpath =
            "//label[text()='{0}']/../..//span[contains(@class,'add_icon')]";

        public const string AddIconByQuadrantNameXpath =
            "//label[text()='{0}']/../..//li/span[contains(@class,'add_icon')]";
        public const string AddUserGroupIconByXpath = "//span[contains(@title,'{0}')]";
        public const string GridRowByQuadrantHeaderXpath= "//label[text()='{0}']/../../.. //ul[contains(@class,'component_item_list')]/div[contains(@class,'component_item')]";

        public const string ListValueInGridByQuadrantAndColumnXpath =
                "//label[text()='{0}']/../../../section[2]//div[contains(@class,'component_item')]/ul/li[{1}] //span[contains(@class,'data_point_value')]";
        public const string LabelInGridByQuadrantRowColumnXpath = "//label[text()='{0}']/../../../section[2]//ul[contains(@class,'component_item_row')][{1}]/li[{2}]/label";
        public const string ValueInGridByQuadrantRowColumnXpath = "//label[text()='{0}']/../../../section[2]//ul[contains(@class,'component_item_list')]/div[contains(@class,'component_item')][{1}]/ul/li[{2}]/span";
        public const string EditIconByQuadrantXpath = "//label[text()='{0}']/../../../section[2]//span[contains(@class,'small_edit_icon')]";
        public const string DeleteIconByQuadrantXpath = "//label[text()='{0}']/../../../section[2]//span[contains(@class,'small_delete_icon')]";
        public const string EditRecordIconByQuadrantRowXpath =
            "//label[text()='{0}']/../../..//div[contains(@class,'component_item')][{1}]//span[contains(@class,'small_edit_icon')]";
        public const string DeleteRecordIconByQuadrantRow =
                "//label[text()='{0}']/../../..//div[contains(@class,'component_item')][{1}]//span[contains(@class,'small_delete_icon')]";

        public const string EditIconByValueInRowCssLocator =
            "ul.component_item_list>div.component_item:has(span:contains({0}))>ul>li span.small_edit_icon";

        public const string DeleteIconByValueInRowCssLocator =
                "ul.component_item_list>div.component_item:has(span:contains({0}))>ul>li span.small_delete_icon"
            ;

        public const string AddMicrostrategyReportsFormXpath =
            "//label[text()='Microstrategy Reports']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Add Microstrategy Report']";

        public const string AddReportIconXpath = "//li[contains(@title,'Add Microstrategy Report')]";
        public const string InputFieldByLabelXpath = "div:has(>label:contains({0})) input";

        public const string SaveButtonByQuadrantHeaderXpath =
            "//label[text()='{0}']/../../../section[2] //button[contains(@class,'work_button')]";

        public const string RoleDropDownListInAddReportFormByXpath =
            "//label[text()='Microstrategy Reports']/../../..//label[text()='Role']/../section//ul//li";
        public const string EditMicrostrategyReportsFormXpath =
            "//label[text()='Microstrategy Reports']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Edit Microstrategy Report']";
        public const string ReportFormHeaderXpath= "//label[text()='Microstrategy Reports']/../../..//section[contains(@class,'form_component')]//span[contains(@class,'form_header_left')]/label";
        public const string ActiveAddReportIconXpath =
            "//li[contains(@title,'Add Microstrategy Report') and contains(@class,'is_active')]";
        public const string DisabledAddReportIconXpath =
            "//li[contains(@title,'Add Microstrategy Report') and contains(@class,'is_disabled')]";

        public const string CancelButtonXpath = "//span[contains(@class,'span_link') and text()='Cancel']";
        public const string GridRowByValue = "ul.component_item_list>div.component_item:has(span:contains({0}))>ul.component_item_row";
        public const string SaveAndCloseButtonXpath =
            "//button[contains(@class,'work_button') and text()='Save and Close']";

        public const string GridColValueByUniqueRowValue =
            "ul.component_item_list>div.component_item:has(span:contains({0}))>ul>li:nth-of-type({1})>span";

        public const string EmptyMessageCssSelector = "p.empty_message";
        #region firstQuadrant

        public const string AddMicrostrategyuserGroupFormByXpath = "//label[text()='User Group']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Add Microstrategy User-Group']";
        public const string EditMicrostrategyuserGroupFormByXpath = "//label[text()='User Group']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Edit Microstrategy User-Group']";

        public const string AddGroupButtonByCss = "//button[text()='Add Group']";
        public const string SaveButtonByXpath = "//label[text()='User Group']/../../../section[2] //button[text()='Save']";

        public const string SaveandCloseButtonByXpath =
            "//label[text()='User Group']/../../../section[2] //button[text()='Save and Close']";
        public const string RemoveAddedUserByCss= "section ul:nth-of-type(3)>ul>li>ul>li:nth-of-type(1)";
        public const string DetailsOfAddedUserByCss = "section ul:nth-of-type(3)>ul>li>ul>li:nth-of-type({0})";
        public const string NoOfExisitingUserGroupByCss = "ul.component_group >ul>div";
        public const string DetailsOfExisitingUserGroupByCss = "ul.component_group >ul>div>ul>li:nth-of-type({0})>span";// list 2: groupNames;3:usernames
        public const string EditOrRemoveExistingUserGroupByCss = "ul.component_group >ul>div:nth-of-type({0})>ul>li>ul>li:nth-ot-type({1})";
        public const string AddUsergrpupIconXpath = "//li[contains(@title,'Add Microstrategy Group')]";

        public const string AddMicrostrategyUserGroupFromXpath =
            "//label[text()='User Group']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Add Microstrategy User-Group']";

        public const string UsergroupFormHeaderByXpath = "//label[text()='User Group']/../../..//section[contains(@class,'form_component')] //header[contains(@class,'form_header')] //label[text()='Add Microstrategy User-Group']";
        public const string UserGroupLabelByCss = "ul.component_group >ul>div:nth-of-type(1)>ul>li:nth-of-type(2)>label";//// extract text 2:groupname label;3: username label

        public const string GridvalueForUserGroupRowByXpath =
            "//label[text()='User Group']/../../../section[2]//div[{0}][contains(@class,'component_item')]/ul/li[{1}] //span[contains(@class,'data_point_value')]";

        public const string UpdatePasswordCheeckBoxBycss = "div .component_checkbox span";

        public const string GridLabelListByRowXpath =
                "//label[text()='{0}']/../../../section[2]//div[contains(@class,'component_item')][{1}] //label"
            ;
        public const string GridValueListByRowXpath =
                "//label[text()='{0}']/../../../section[2]//div[contains(@class,'component_item')][{1}] //span[contains(@class,'data_point_value')]"
            ;
        #endregion

        #region second quadrant

        public const string GridRowByQuadrantNameXpathTemplate =
            // "//label[text()='{0}']/../../.. //div[contains(@class,'component_item')]/ul";
            "section:has(>section label:contains(\"{0}\")) div:nth-of-type({1}) ul.component_item_row";

        public const string GridValueByColumnXpathTemplate =
            "//label[text()='{0}']/../../.. //div[contains(@class,'component_item')]/ul/li[{1}]/span";

        public const string AddFormCssSelector = "section.form_component";


        public const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";
        public const string DropDownInputListByLabelXPathTemplate = "//label[text()='{0}']/../section//ul//li";
        public const string MultiDropDownToggleIconXpathTemplate = "//label[text()='{0}']/../section/span[contains(@class,'select_toggle')]";
        public const string MultiSelectListedDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'list_options')]/li";
        public const string MultiSelectAvailableDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'available_options')]/li";

        public const string DeleteIconByFullNameXpathTemplate =
            "//ul[..//span[contains(@title,'{0}')] and contains(@class,'component_item_row')]//span[contains(@class,'small_delete_icon')]";

        public const string AddIconDisabledXpath =
            "//label[text()='{0}']/../..//li[contains(@class,'is_disabled')]/span[contains(@class,'add_icon')]";

        public const string FullNameLinkXpath = "//ul[contains(@class,'component_item_list')]/div/ul[li/span[text()='{0}']]/li[contains(@class,'action_link')]/span";
        #endregion

        #endregion

        #region CONSTRUCTOR
        public MicrostrategyMaintenancePageObjects()
            : base(Support.Enum.PageUrlEnum.MicrostrategyMaintenance.GetStringValue())
        {
        }

        #endregion

        #region PROTECTED
        public override string PageTitle
        {
            get { return PageTitleEnum.Microstrategymaintainance.GetStringValue(); }
        }
        #endregion

    }
}
