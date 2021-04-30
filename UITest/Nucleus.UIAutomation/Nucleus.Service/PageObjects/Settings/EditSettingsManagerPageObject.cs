using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Settings
{
    public class EditSettingsManagerPageObject : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        #region cob

        public const string InfoIconXPathTemplate =
            "//ul[*/span[text()='{0}']]/following-sibling::div//label[span[text()='{1}']]/span[contains(@class,'info_icon')]";

        public const string COBInputXPathTemplate =
            "//ul[*/span[text()='{0}']]/following-sibling::div//label[span[text()='{1}']]/following-sibling::input";

        public const string GetAllLabelsXpathTemplate =
            "//ul[*/span[text()='{0}']]/following-sibling::div//form/div//span[contains(@class,'position_left')]";

        #endregion


        #region edit details secondary details

        public const string ValueByLabelXpathTemplate =
                "//label[text()='{0}:']/following-sibling::span";

            public const string ToolTipByLabelXpathTemplate =
                "//label[text()='{0}:']";


        #endregion

        public const string EditIconByFlagXPathTemplate =
            "//ul[li/span[text()='{0}']]//span[contains(@class,'small_edit_icon ')]";

        public const string DUPAncillarySettings = @"label:has(span:contains({0}))~section ul li";
        public const string DUPAncillarySettingslabel = @"label:has(span:contains({0}))~section div input";
        public const string DUPAnicllarySettingsTooltip = @"label:has(span:contains({0})) span.small_icon";

        public const string AncillarysettingsvalueDropdown =
            @"label:has(span:contains({0}))~section ul li:contains({1})";

        public const string ReturnToClientSearchCssSelector = "span[title = 'Return to Client Search']";
        public const string SideBarIconCssSelector = "span.sidebar_icon";
        public const string ClientCodeValueCssSelector = "label[title='Client code']+span";

        #region Modify Edit Settings

        public const string StatusOnOffLabelXPathTemplate =
            "//div[label[text()='Status:']]/div/label[{0}]";

        public const string StatusRadioButtonXPathTemplate =
            "//div[label[text()='Status:']]/div/span[{0}]";

        public const string ReviewStatusCheckBoxXPathTemplate = "//div[label[text()='Review Status:']]/div[{0}]/span";
        public const string ReviewStatusCheckBoxTextXPathTemplate = "//div[label[text()='Review Status:']]/div[{0}]/div";


        #region Fall Back Order Settings

        public const string FallBackOrderSettingsLabelXPathTemplate = "//label[contains(@class, 'fall_back_order_label')]";
        public const string FallBackOrderLaberByCss = "ul.component_item_row div.toggle_select~li.component_data_point";
        public const string InputFieldForEachDataSetXpathTemplate = 
            "//section[contains(@class,'form_component')]//span[text()='{0}']/../..//following-sibling::input";
        public const string GetAllDataSetsFromFormXPathTemplate = 
            "//label/span[text()='Fall Back Order Settings']/../..//li[contains(@class,'component_data_point')]/span";
        public const string UseDefaultOrCurrentButtonXPathTemplate = "//label[contains(@class, 'fall_back_order_label')]/../button[contains(@class, 'work_button')]";
        public const string ActiveDataSetsXPathTemplate = "//div//span[text()='Fall Back Order Settings']/../..//span[contains(@class,'active')]/../..//li[contains(@class,'component_data_point')]";

        public const string GetActiveFallBackOrdersCssSelector = "div:has(>label>span:contains(Fall Back Order Settings)) span.icon.checkbox.active";
        public const string CheckBoxByDataSetCssSelector = "div:has(>label>span:contains(Fall Back Order Settings)) div:has(span:contains({0})) span.icon.checkbox";

        public const string CheckBoxByIndexCssSelector =
            "div:has(>label>span:contains(Fall Back Order Settings)) .component_item:nth-of-type({0}) span.icon.checkbox.active";

        public const string CancelInEditSettingsFormXPath = "//span[text()=\"Cancel\"]";







        #endregion

        #region Ancillary Settings

        public const string AncillaryLabelCssSelector = ".edit_ancillary_setting label:contains({0})";

        public const string AncillaryLabelTooltipCssSelector = ".edit_ancillary_setting>label>span.small_icon";

        public const string AncillarySettingsCssSelector = ".edit_ancillary_setting";

        public const string AncillarySettingsForINVDCssSelector = ".edit_ancillary_setting>label>span:nth-of-type(1)";

        public const string AncillaryRadioButtons = "div.component_item >div:nth-of-type({1}) >ul.edit_ancillary_setting div.radio_button_group>span:nth-of-type({0})";

        public const string AncillaryRadioButtonsByLabel = "div.component_item >div:contains({1}) >ul.edit_ancillary_setting div.radio_button_group>span:nth-of-type({0})";
        #endregion


        #endregion
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.EditSettingsManager.GetStringValue(); }
        }


        #endregion

        #region CONSTRUCTER

        public EditSettingsManagerPageObject()
            : base(PageUrlEnum.EditSettingsManager.GetStringValue())
        {
        }

        #endregion
      
        #region PAGEOBJECT PROPERTIES



        #endregion

    }
}
