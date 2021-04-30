using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.PreAuthorization
{
    public class PreAuthCreatorPageObjects: NewDefaultPageObjects

    {
        #region PRIVATE FIELDS

        

        #endregion

        #region CONSTRUCTOR
        public PreAuthCreatorPageObjects()
            : base(PageUrlEnum.PreAuthorization.GetStringValue())
        { }
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.PreAuthorization.GetStringValue(); }
        }

        #endregion

        #region CSS/XpathLocators

        public static string SelectOptionTabButtonCssTemplate = "div.option_button_selector>div:nth-of-type({0})";
        public static string InputFieldByLabelXPathTemplate = "//section[contains(@class,'form_component')]/div[not(@hidden)]//div//label[contains(normalize-space(),'{0}')]/following-sibling::input " +
                                                              "| " +
                                                              "//section[contains(@class,'form_component')]/ul //div//label[contains(normalize-space(),'{0}')]/following-sibling::input";

        public static string GridRowByHeaderLabelXPathTemplate =
            "//li[.//label[text()='{0}']]/following-sibling::ul //ul[contains(@class,'component_item_row')]";

        public static string ValueOnLeftWindowGridByHeaderLabelXPathTemplate =
            "//li[.//label[text()='{0}']]/following-sibling::ul //ul[contains(@class,'component_item_row')]/li[{1}] //span";

        public static string ValueOnRightWindowGridByHeaderLabelXPathTemplate =
            "//li[.//label[text()='{0}']] //ul[contains(@class,'component_item_row')]/li[{1}] //span";

        public static string LabelOnLeftWindowGridByHeaderLabelXPathTemplate =
            "//li[.//label[text()='{0}']]/following-sibling::ul //ul[contains(@class,'component_item_row')]/li[{1}] //label";

        public static string LabelOnRightWindowGridByHeaderLabelXPathTemplate =
            "//li[.//label[text()='{0}']] //ul[contains(@class,'component_item_row')]/li[{1}] //label";

        public static string ValueByInputLabelXPathTemplate =
                    "//section[contains(@class,'form_component')]/ul //label[contains(text(),'{0}')]/following-sibling::span";
        //"//label[contains(text(),'{0}')]/following-sibling::span";

        public static string ButtonCssLocator =
            "section.form_component >div:not([hidden]) button";

        public static string ClearLinkCssLocator =
            "section.form_component >div:not([hidden]) span.span_link";

        public static string NoResultFoundPTagXPathTemplate =
            "//li[.//label[text()='{0}']]/following-sibling::p";

        public static string CancelLinkXPath = "//button[text()='Submit for Review']/following-sibling:: span/span";

        public static string SubmitButtonXpath = "//button[text()='Submit for Review']";

        public static string LabelsOfInputFieldOnLeftSideOfAddLineXPath = "(//section[contains(@class,'component_content')])[1]/section/div[4]/ul//label";
        
        public static string CalendarSelectorXPathTemplate = "(//div[contains(@class,'pika-lendar')]//td/button[contains(@class,'pika-button')])[{0}]";

        public static string AddLineScenarioDropdownListCssSelector = "section.select_wrap>ul>li";

        public static string ScenarioDropDownCssSelector = "section.select_wrap span.select_toggle";

        public static string AddLineScenarioInputFieldXPath = "//section[contains(@class,'form_component')]/div[not(contains(@style,'none'))] //label[contains(normalize-space(),'Scenario')]/following-sibling::section//input";

        public static string AddLineScenarioOptionSelectorXPathTemplate = "//section[contains(@class,'select_wrap')]/ul/li[{0}]";

        public static string InputFieldByLabelXpath = "//label[text()='{0}']/../section/div/input";

        public static string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section/ul/li[text()='{1}']";

        public static string ValueByLabelXPath = "//section[contains(@class,'form_component')]/ul//li//label[text()='{0}']/../span";

        public static string InputFieldByLabelXPath = "//label[text()='{0}']/../input";

        public static string CreatePreAuthSectionCssSelector = "section.form_component:has(label):contains(\"Create Pre-Authorization\")";

        public static string IsLabelAllowedToBeEditedCssSelector = "section.form_component:has(label):contains(\"Create Pre-Authorization\") label:contains(\"{0}\").is_enabled";

        public static string IsLabelRequiredFieldXpath =
            "//label[text()='{0}']/span[contains(@class,'required_asterisk') and not(@hidden)]";

        public static string IsLabelPresentCssSelectorTemplate =
            "ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})>label";

        public static string AddLineDeleteIconCssSelectorBYRow =
            "ul.results_panel>ul>div:nth-of-type({0})>ul>li.small_delete_icon";


       

        #region Upload Document
        public const string UpperRightQuadrantTitleCssLocator = "#top_section section.component_right>section>div>label";

        public const string UploadDocumentFormHeader = "span.form_header_left>label";

        public const string UploadDocumentFormCssSelector = "section.form_component div.uploader";
        public const string documentBadgeCount = ".has_documents span";

        public const string AddIconCssSelector = "span.form_header_right span.add_icon";
        public const string preauthIconCssSelector = ".preauth_icon ";

        public const string CancelLinkCssSelector = "ul.document_list span.span_link";

        public const string PreAuthStatusCssSelector = "label[title='Status']+div";

        public const string PreAuthDocumentDeleteIconCssLocator =
            "div.listed_document:nth-of-type(1) >ul:nth-of-type(1)>li:not([style*='none'])  span.small_delete_icon";

        public const string PreAuthDocumentNameCssTemplate = "div.listed_document ul>li:nth-of-type({0})>span";
        public const string DocumentUploadIcon = ".add_documents ";

        #endregion

        #endregion
    }

}

