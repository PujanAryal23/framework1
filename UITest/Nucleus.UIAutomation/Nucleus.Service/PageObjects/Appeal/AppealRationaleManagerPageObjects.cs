
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Appeal
{
   
    public class AppealRationaleManagerPageObjects : NewDefaultPageObjects

    {
        //save button to scroll
        public const string SaveButtonCSSSelector = "section.form_component div.form_buttons button.work_button";

        public const string AppealRationaleAuditSideWindowXpathSelector = "//label[text()='Appeal Rationale Audit History']/ancestor::section[contains(@class,'component_right')]";
        public const string ActiveXpathSelector = "//li[@title='Deactivate']";
        public const string InActiveXpathSelector = "//li[@title='Inactive']";
        public const string UploadedFileNameXpath = "//label[contains(@title,\"File Name\")]/following-sibling::span";

        #region CSS

        public const string DocumentsIconInAppealRationaleAuditHx = "a[Title=Documents]";
        public const string AddButtonCssLocator = "div li>a.icon";
        public const string SearchIconCssLocator = "span.sidebar_icon";
        public const string RationaleRowsCssLocator = "section.search_list > section.component_content > ul > li";
        public const string EditWindowCSSLocator = "section.form_component.column_100.component_nested_form.ember-view";
        public const string AppealRationaleAuditSideWindowTitle = "section.component_right>section.component_header label";
        #endregion

        #region TEMPLATE
        public const string AppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate =
           "//ul[contains(@class,'component_item_list')]/li [ul/li[2]/span[text()='{0}']] [ul/li[3]/span[text()='{1}']] [ul/li[4]/span[text()='{2}']] [ul/li[6]/span[text()='{3}']] [ul/li[7]/span[text()='{4}']] [ul/li[8]/span[text()='{5}']] ";
      


        public const string AppealRationaleRowCategoryByFlagSourcerProcClientTemplate =
            "//ul[contains(@class,'component_item_list')]/li [ul/li[2]/span[text()='{0}']] [ul/li[3]/span[text()='{1}']]  [ul/li[6]/span[text()='{2}']]  [ul/li[8]/span[text()='{3}']]";

        public const string AppealRationaleAuditHistoryTemplate =AppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate + "/ul/li[2]";

        

        public const string AppealRationaleAuditValueListByCol =
            "//label[text()='Appeal Rationale Audit History']/ancestor::section[contains(@class,'component_right')]//ul[contains(@class, 'component_item_row')]/li[{0}]/span";

        public const string EditButtonofAppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate =
           AppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate + "/ul/li[1]//li[1]";

        public const string EditButtonOfAppealRationaleRowbyFlagSourceModifierProcTrigClientCssTemplate =
            "li.component_item>ul.component_item_row:has( span:contains({0})):has( span:contains({1})):has( span:contains({2})):has( span:contains({3})):has( span:contains({4})):has( span:contains({5})) span.small_edit_icon";

        public const string DeleteButtonofAppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate =
           AppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate+ "/ul/li[1]//li[2]";

        public const string DeleteButtonofAppealRationaleRowCategoryByFlagSourcProcClientTemplate =
            AppealRationaleRowCategoryByFlagSourcerProcClientTemplate + "/ul/li[1]//li[2]";

        public const string FormTypeValueofRationaleRowCategoryByFlagSourcProcClientTemplate =
            "("+ AppealRationaleRowCategoryByFlagSourcerProcClientTemplate + ")/ul/li[5]//span";

        public const string GetGridViewItembyColCss = "ul.component_item_list>li>ul>li:nth-of-type({0})";
        public const string RationaleSearchResultListCssTemplate =
            "ul.component_item_list>li>ul>li:nth-of-type({0}) span";
        public const string RationaleResultByRowColCssTemplate =
           "ul.component_item_list>li:nth-of-type({0})>ul>li:nth-of-type({1}) span";


        public const string AuditHistoryValueTemplateByRowXpathSelector = "(//label[text()='Appeal Rationale Audit History']/ancestor::section[contains(@class,'component_right')]//ul[contains(@class, 'component_item_row')])[{0}]";

        public const string AuditHistoryTemplateFieldsByRowColXpathSelector = AuditHistoryValueTemplateByRowXpathSelector + "/li[{1}]/span";

        public const string ValueInGridByRowColumnCssTemplate =
            "section.search_list ul.component_item_list>li:nth-of-type{0}) li.component_data_point:nth-of-type({1}) span.data_point_value";

        public const string ValueInGridByLabelXpathTemplate =
            "(//label[text()= '{0}']/../span)[{1}]";

        public const string EditRecordIconCssTemplate =
                "section.search_list > section.component_content > ul > li:nth-of-type({0})  > ul > li > ul > li > span.small_edit_icon";

        public const string EditIconCssLocator =
            "section.search_list > section.component_content  span.small_edit_icon";

        public const string DeactivateIconCssTemplate =
            "section.search_list > section.component_content > ul > li:nth-of-type({0})  > ul > li > ul > li > span.small_delete_icon";

        public const string HelpfulAppealHintsTextBoxXpath = "//label[text()='Helpful Appeal Hints']/../div";
        #endregion



        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealSearch.GetStringValue(); }
        }


        #endregion

        #region CONSTRUCTOR

        public AppealRationaleManagerPageObjects()
            : base(PageUrlEnum.AppealSearch.GetStringValue())
        {
        }

        #endregion


        #region PAGEOBJECTS PROPERTIES


        #endregion

    }
}
