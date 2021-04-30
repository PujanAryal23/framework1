using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Common;

namespace Nucleus.Service.PageObjects.Claim
{
    public class LogicManagementPageObjects : NewPageBase
    {
        #region PUBLIC FIELDS

        public const string LogicMessageTextAreaXPath = ".//div[@class='reponse_form toggle_section open']/form/div/textarea";
        public const string SaveBtnXPath = ".//div[@class='reponse_form toggle_section open']/form/div/input";

        public const string AssignedToXPath = "//ul[li[contains(@class,'grid_col1 col_tool toggle active')]]/li[@class='grid_col10']";

        public const string LogicRequestListCssLocator = "div.logic_responses.toggle_section.open>span";

        public const string PageTitleCssLocator = "label.PopupPageTitle";

        public const string CreateLogicFormXpathTemplateForFlagSelected =
            "//span[text()='{0}']/ancestor::div[contains(@class,'grid_row')]//div[contains(@class,'open') and contains(@class,'reponse_form')]";
        public const string LogicFormXpathTemplateForFlagAndRow =
            "(//span[text()='{0}']/ancestor::div[contains(@class,'grid_row')]//div[contains(@class,'reponse_form')])[{1}]";

        public const string AddLogicXpathTemplateForFlagAndRow =
            "(//span[text()='{0}']/ancestor::div//li[contains(@class,'logic')]/a)[{1}]";

        public const string SelectDropDownCssSelector = "div.status_select_wrap>select";        

        public const string DropDownListForStatusCssTemplate =
            "div.status_select_wrap>select>option:nth-of-type({0})";
        
        public const string DropdownListDefaultSelectedValueXpath =
            "//div[@class='reponse_form toggle_section open']/form/div/div/label[text()='{0}']/../span/following-sibling::span[1]";

        public const string SaveUpdateButtonForGivenFlagRowXpathTemplate =
            "(//span[text()='{0}']/ancestor::div[contains(@class,'grid_row')]//input[contains(@class,'work_button') and @value='{1}']])[{2}]"; //flag, button value and row
        public const string CancelButtonXpathTemplateForFlagAndRow =
            "(//span[text()='{0}']/ancestor::div//a[contains(@class,'CancelButton close_edit')])[{1}]";
        public const string ReplyIconCssSelector = "div.grid_row:has(span.line_flag_can_be_worked:contains({0})) div.logic_responses:has( span:contains({1})) li.reply_icon a";
        public const string EditIconCssSelector = "div.grid_row:has(span.line_flag_can_be_worked:contains({0})) div.logic_responses:has( span:contains({1})) li.edit_icon>a";
        public const string EditMessageTextAreaCssSelector = "textarea.edit_message";
        public const string UpdateLogicNoteButtonXpath = "//div[contains(@class,'logic_responses toggle_section open')]/span/div/form/div/div/input";
        public const string UpdatedLogicNoteXpath = "//div[contains(@class,'logic_responses toggle_section open')]/span/div/div/p";
        public const string StatusDropdownClassSelector = "status_combo select";
        #endregion

       

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.LogicManagementPage.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region CONSTRUCTOR

        public LogicManagementPageObjects()
            : base(PageUrlEnum.LogicManagement.GetStringValue())
        {
        }

        #endregion
    }
}
