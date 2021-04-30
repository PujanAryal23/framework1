using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Default;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public  class SideBarPanelSearch
    {
        private readonly CalenderPage _calenderPage;
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #region CONSTRUCTOR

        public SideBarPanelSearch(CalenderPage calenderPage,ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            _calenderPage = calenderPage;
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region PRIVATE FIELDS
        private const string FilterIconCssLocator = "span.icon.filter_icon";
        private const string SearchFiltersLabelCss = "section.component_sidebar.is_slider:not(.is_hidden) section.component_sidebar_panel label.form_label";
        public const string InputFieldByLabelCssTemplate = "section.form_component div:has(>label:contains({0})) input ,ul div:has(>label:contains({0})) input,section.is_slider:not(.is_hidden) div:has(>label:contains({0})) input";
        public const string InputFieldByLabelXpathTemplate = "//section[not(contains(@class,'is_hidden'))]//section[contains(@class,'component_header')]/..//label[text()='{0}']//..//input";
        public const string MultipleInputFieldByLabelXpathTemplate = "//label[text()='{0}']/../div[{1}]/input";
        public const string InputFieldDisbaledCheckByLabelCssTemplate = "div:has(>label:contains({0})) input:disabled";
        private const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";
        private const string DropDownInputListValueByLabelAndIndexXPathTemplate = "//label[text()='{0}']/../section//ul//li[{1}]";
        private const string DropDownInputListByLabelXPathTemplate = "//label[text()='{0}']/../section//ul//li";
        
        public const string MultiDropDownToggleIconXpathTemplate = "//label[text()='{0}']/../section/span[contains(@class,'select_toggle')]";
        public const string MultiSelectListedDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'list_options')]/li";
        public const string MultiSelectListedDropDownToggleValueForClaimSearchXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'available_options')]/li";
        public const string MultiSelectAvailableDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'available_options')]/li";

        private const string DropDownToggleIconCssTemplate = "div:has(>label:contains({0})) span.select_toggle";
        ////label[contains(text(),'Claim Status')]/following-sibling::section//span/span
        public const string SideBarPanelSectionCssLocator = "section.component_sidebar.is_slider:not(.is_hidden)";
        public const string HiddenSideBarPanelSectionCssLocator = "section.component_sidebar.is_slider.is_hidden";

        public const string SelectedDropDownOptionsXpathtemplate = "//div[label[text()='{0}']]//ul/li[contains(@class,'is_active')]";

        public const string AdvancedSearchIconCssLocator = "span.advanced_filter_icon ";
        public const string AdvancedSearchIconSelectedCssLocator = "li.is_selected:has(span.advanced_filter_icon)";

        public const string SearchInputFieldCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) input";
        //public const string FieldErrorIconXPathTemplate = "//label[text()='{0}']/span[contains(@class,'field_error')]";

        public const string ButtonCssTemplate = "form button:contains({0}) ,section.component_sidebar:not(.is_hidden) button:contains({0})";//JQuery


        public const string ClearCancelJQueryTemplate = "span:contains({0})";
        public const string ClearCssLocator = "section.component_sidebar.is_slider:not(.is_hidden) span.span_link:contains(Clear)";//jquery
        public const string LabelCountOfAllFilters = "section.component_sidebar_panel label";
        public const string LabelValueOfAFilter = "section.component_sidebar_panel label:nth-of-type({0})";

        public const string WorkListIconCssLocator = "span.list_icon.icon";
        public const string SectionHeaderCssLocator = "div.current_viewing_searchlist>header";//
        public const string TopHeaderCssLocator = "section:not(.is_hidden).component_sidebar div.component_header_left>label";
        public const string SwitchWorkListIconCssLocator = "span.chevrons";
        public const string ToggleFindAnalystPanelCssLocator = "span.sidebar_icon.toolbar_icon";
        private const string OptionListOnArrowDownIconJquery = "section.component_sidebar li:has(>span.options) span.span_link";
        private const string OptionSelectorDropDownCssLocator = "section.component_sidebar span.options.toolbar_icon";

        public const string NextClaimsWorkListXpath =
            "//header[contains(text(),'Next Claims in Work List')]/parent::span/ul/li";

        public const string AdditionalLockedClaimsXpath =
            "//header[contains(text(),'Additional Locked Claims')]/parent::span/ul/li/span[1]";

        public const string RemoveLockXpath =
            "//header[contains(text(),'Additional Locked Claims')]/parent::span/ul/li/span[contains(text(),'{0}')]/following-sibling::span";
        //public const string RedRequiredExclamationPointXpath = "//label[text()='{0}']/span[contains(@class,'field_error')]";
        private const string SideBarHeaderCssLocator = "section:not(.is_hidden).component_sidebar label.component_title";

        

        #endregion

        #region PUBLIC PROPERTIES

        public void ClickOnSideBarHeader()
        {
            SiteDriver.FindElement(SideBarHeaderCssLocator, How.CssSelector).Click();
        }

        //public string GetRedExclamationDescription(string label)
        //{
        //    return SiteDriver.FindElementAndGetAttribute(string.Format(RedRequiredExclamationPointXpath, label),
        //        How.XPath, "Title");
        //}

        public List<string> GetNextClaimsInWorkList()
        {
            return JavaScriptExecutor.FindElements(NextClaimsWorkListXpath, How.XPath, "Text");
        }

        public List<string> GetAddtionallockedClaims()
        {
            return JavaScriptExecutor.FindElements(AdditionalLockedClaimsXpath, How.XPath, "Text");
        }

        public void ClickOnRemoveLock(string claseq)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(RemoveLockXpath,claseq), How.XPath);
            SiteDriver.WaitToLoadNew(500);

        }

        public void ClickOnOptionSelectorDropDownIcon()
        {
            JavaScriptExecutor.ExecuteClick(OptionSelectorDropDownCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(500);
        }

        public List<string> GetOptionListOnArrowDownIcon()
        {
            ClickOnOptionSelectorDropDownIcon();
            var list=  JavaScriptExecutor.FindElements(OptionListOnArrowDownIconJquery, "Text");
            ClickOnOptionSelectorDropDownIcon();
            return list;
        }

        public void ClickOnFilterIcon()
        {
            JavaScriptExecutor.ExecuteClick(FilterIconCssLocator, How.CssSelector);
        }
        public void ClickOnButtonByButtonName(string label)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(ButtonCssTemplate, label));
            Console.WriteLine(label+" Button Clicked");
        }

        public void ClickOnFindButton()
        {
            SiteDriver.WaitToLoadNew(200);
            ClickOnButtonByButtonName("Find");
            SiteDriver.WaitToLoadNew(500);
        }

        public void ClickOnClearLink()
        {
            JavaScriptExecutor.ClickJQueryByText(string.Format(ClearCancelJQueryTemplate, "Clear"), "Clear");
            SiteDriver.WaitToLoadNew(1000);
            Console.WriteLine("Clear Link Clicked");
        }

        public void ClickOnClearLinkCssSelector()
        {
            JavaScriptExecutor.ClickJQuery(ClearCssLocator);
            SiteDriver.WaitToLoadNew(1000);
            Console.WriteLine("Clear Link Clicked");
        }

        public void SetDateFieldFrom(string dateName,string date)
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDivUsingJquery(string.Format(ButtonCssTemplate, "Find"));
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(1)",
                    dateName));
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }

        public void SetDateField( string dateName, string date,int col)
        {
            var element = JavaScriptExecutor.FindElement(
                string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(" + col + ")",
                    dateName));
            element.ClearElementField();
                 element.SendKeys(date);
                 //JavaScriptExecutors.ExecuteMouseOut(string.Format(InputFieldByLabelCssTemplate, dateName));
                 Console.WriteLine("<{0}> Input value:<{1}>", dateName, date);
                 SiteDriver.WaitToLoadNew(500);
                 element.SendKeys(Keys.Tab);
                 SiteDriver.WaitToLoadNew(500);
        }

        public string GetDateFieldFrom(string dateName)
        {
            return
                JavaScriptExecutor.FindElement(
                    string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(1)", dateName))
                    .GetAttribute("value");
        }

        public string GetDateFieldPlaceholder(string dateName, int col)
        {
            return
               JavaScriptExecutor.FindElement(
                    string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(" + col + ")", dateName)
                    ).GetAttribute("placeholder");
        }

        public void SetDateFieldTo(string dateName, string date)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(2)", dateName));
            _calenderPage.SetDate(date);
            Console.WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }

        public string GetDateFieldTo(string dateName)
        {
            return
                 JavaScriptExecutor.FindElement(
                    string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(2)", dateName)
                    ).GetAttribute("value");
        }

        public string GetNoMatchingRecordFoundMessage()
        {
            return SiteDriver.FindElement(AppealSearchPageObjects.NoMatchingRecordFoundCssSelector,
                How.CssSelector).Text;
        }

        public bool IsNoMatchingRecordFoundMessagePresent()
        {
            return SiteDriver.IsElementPresent(AppealSearchPageObjects.NoMatchingRecordFoundCssSelector, How.CssSelector);
        }

        public string GetFieldErrorIconTooltipMessage(string label)
        {
            return SiteDriver.FindElement(string.Format(DefaultPageObjects.InvalidInputByLabelXPathTemplate,label), How.XPath)
                .GetAttribute("title");
        }

        public bool IsAdvancedSearchFilterIconDispalyed()
        {
            return SiteDriver.IsElementPresent(
                AdvancedSearchIconCssLocator, How.CssSelector);
        }

        public bool IsAdvancedSearchFilterIconSelected()
        {
            return JavaScriptExecutor.IsElementPresent(AdvancedSearchIconSelectedCssLocator);
        }

        public void ClickOnAdvancedSearchFilterIcon(bool click)
        {
            var extedFieldDispaly = IsSearchInputFieldDispalyed(17);
            if (!extedFieldDispaly && click)
            {
                JavaScriptExecutor.ExecuteClick(AdvancedSearchIconCssLocator, How.CssSelector);
                SiteDriver.WaitForCondition(() => IsSearchInputFieldDispalyed(17));
            }
            else if (extedFieldDispaly && !click)
            {
                JavaScriptExecutor.ExecuteClick(AdvancedSearchIconCssLocator, How.CssSelector);
                SiteDriver.WaitForCondition(() => !IsSearchInputFieldDispalyed(17));
            }
        }

        public bool IsSearchInputFieldDispalyed(int row)
        {
            return SiteDriver.IsElementPresent(
                string.Format(SearchInputFieldCssTemplate, row), How.CssSelector);
        }

        public bool IsSearchInputFieldDisplayedByLabel(string label)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(InputFieldByLabelCssTemplate, label));
        }

        public void SendEnterKeysOnTextFieldByLabel(string label="Client")
        {
            JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label)).SendKeys(Keys.Enter);
        }

        public void SelectDropDownListValueByLabel(string label, string value, bool directSelect =true)
        {       
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(string.Format(InputFieldByLabelXpathTemplate, label),How.XPath); 
            if (!GetInputValueByLabel(label).Equals(value))
            {
                JavaScriptExecutor.ExecuteClick(string.Format(InputFieldByLabelXpathTemplate, label),How.XPath);
                SiteDriver.WaitToLoadNew(300);
                try
                {
                    SiteDriver.WaitToLoadNew(300);
                    element.ClearElementField();
                    SiteDriver.WaitTillClear(element);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if (!directSelect) element.SendKeys(value);
                if (!SiteDriver.IsElementPresent(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath)) JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            }
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public void SelectDropDownListByIndex(string label,int index)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(string.Format(InputFieldByLabelXpathTemplate, label), How.XPath);
            JavaScriptExecutor.ExecuteClick(string.Format(InputFieldByLabelXpathTemplate, label), How.XPath);
            SiteDriver.WaitToLoadNew(300);
            try
            {
                element.ClearElementField();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
            if (!SiteDriver.IsElementPresent(string.Format(DropDownInputListValueByLabelAndIndexXPathTemplate, label,index), How.XPath))
                JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndIndexXPathTemplate, label, index), How.XPath);
            SiteDriver.WaitToLoadNew(300);
            Console.WriteLine("Index <{0}> Selected in <{1}> ", index, label);
        }

        public List<string> GetAvailableDropDownList(string label)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.Click();
            Console.WriteLine("Looking for <{0}> List",  label);
            SiteDriver.WaitToLoadNew(500);
            var list = JavaScriptExecutor.FindElements(string.Format(DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            if (list.Count == 0)
            {
                JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
                SiteDriver.WaitToLoadNew(500);
                list = JavaScriptExecutor.FindElements(string.Format(DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            }
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
            Console.WriteLine("<{0}> Drop down list count is {1} ",  label, list.Count);
            return list;
        }

        public List<string> SelectedDropDownOptionsByLabel(string label)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListByLabelXPathTemplate, label), How.XPath);
            SiteDriver.WaitToLoadNew(200);
            return JavaScriptExecutor.FindElements(string.Format(SelectedDropDownOptionsXpathtemplate, label), How.XPath, "Text");
        }

        public void SetInputFieldByLabel(string label, string value,bool multipleInputValues=false,int index=0,bool sendTabKey=false)
        {
            if (!multipleInputValues)
            {
                var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
                element.ClearElementField();
                SiteDriver.WaitToLoadNew(300);
                element.SendKeys(value);
                if (sendTabKey)
                    element.SendKeys(Keys.Tab);
                Console.WriteLine("{0} set in {1}", value, label);
            }
            else
            {                            
                var element = SiteDriver.GetWebElementList(string.Format(InputFieldByLabelXpathTemplate, label),How.XPath).ElementAt(index);
                element.ClearElementField();
                SiteDriver.WaitToLoadNew(300);
                element.SendKeys(value);
                Console.WriteLine("{0} set in {1}", value, label);
            }
            SiteDriver.WaitToLoadNew(300);
        }

        public Int32 GetLengthOfTheInputFieldByLabel(string label)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label)).GetAttribute("value");
            var lengthOfInputField =  element.Length;
            return lengthOfInputField;
        }

        public void SelectSearchDropDownListForMultipleSelectValue(string label, string value)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
            Console.WriteLine("<{0}> set in <{1}>", label,value);
        }

        public void SelectMultipleValuesInMultiSelectDropdownList(string label, List<string> multipleValue)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            foreach (var val in multipleValue)
            {
                element.SendKeys(val);
                JavaScriptExecutor.ExecuteClick(
                    string.Format(
                        DropDownInputListValueByLabelAndValueXPathTemplate, label,
                        val), How.XPath);
                JavaScriptExecutor.ExecuteMouseOut(
                    string.Format(
                        DropDownInputListValueByLabelAndValueXPathTemplate, label,
                        val), How.XPath);
            }
        }

        public List<string> GetMultiSelectListedDropDownList(string label)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            var list= JavaScriptExecutor.FindElements(string.Format(MultiSelectListedDropDownToggleValueXpathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            return list;
        }

        public List<string> GetMultiSelectListedDropDownListForCLaimSearch(string label)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            var list = JavaScriptExecutor.FindElements(string.Format(MultiSelectListedDropDownToggleValueXpathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            return list;
        }

        public List<string> GetMultiSelectAvailableDropDownList(string label)
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDivUsingJquery(string.Format(InputFieldByLabelCssTemplate, label));
            var spanElem = SiteDriver.FindElement(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            spanElem.Click();
            SiteDriver.WaitToLoadNew(2000);
            if (
               !SiteDriver.IsElementPresent(
                    string.Format(MultiSelectAvailableDropDownToggleValueXpathTemplate, label), How.XPath))
                spanElem.Click();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(MultiSelectAvailableDropDownToggleValueXpathTemplate, label), How.XPath));

            var actualList = JavaScriptExecutor.FindElements(string.Format(MultiSelectAvailableDropDownToggleValueXpathTemplate, label), How.XPath, "Text");

            JavaScriptExecutor.ExecuteClick(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            if (
               SiteDriver.IsElementPresent(
                    string.Format(MultiSelectAvailableDropDownToggleValueXpathTemplate, label), How.XPath))
                JavaScriptExecutor.ExecuteClick(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            Console.WriteLine("<{0}> Drop down list count is {1} ", label, actualList.Count);
            JavaScriptExecutor.ExecuteMouseOut(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            return actualList;
        }

        public void ClickOnToggleIcon(string label)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(DropDownToggleIconCssTemplate, label));
        }

        public bool IsSideBarPanelOpen()
        {
            return SiteDriver.IsElementPresent(SideBarPanelSectionCssLocator, How.CssSelector);
        }

        public void OpenSidebarPanel()
        {
            var open = IsSideBarPanelOpen();
            if (open)
                return;
            
            JavaScriptExecutor.ExecuteClick(ToggleFindAnalystPanelCssLocator, How.CssSelector);
            WaitSideBarPanelOpen();
            if (!IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(ToggleFindAnalystPanelCssLocator, How.CssSelector);
            WaitSideBarPanelOpen();

        }

        public void CloseSidebarPanel()
        {
            var open = IsSideBarPanelOpen();
            if (!open)
                return;

            JavaScriptExecutor.ExecuteClick(ToggleFindAnalystPanelCssLocator, How.CssSelector);
            WaitSideBarPanelClosed();
        }

        public void ClickOnSideBarPanelIcon()
        {
            JavaScriptExecutor.ExecuteClick(ToggleFindAnalystPanelCssLocator, How.CssSelector);
        }

        public void ClickOnToggleSidebarPanelButton()
        {
            var open = IsSideBarPanelOpen();
            JavaScriptExecutor.ExecuteClick(ToggleFindAnalystPanelCssLocator, How.CssSelector);
            if (open)
                WaitSideBarPanelClosed();
            else
                WaitSideBarPanelOpen();
        }
        public void WaitSideBarPanelOpen()
        {
            SiteDriver.WaitForCondition(IsSideBarPanelOpen,5000);
        }

        public void WaitSideBarPanelClosed()
        {
            SiteDriver.WaitForCondition(()=>!IsSideBarPanelOpen(),5000);
        }

        public List<string> GetSearchFiltersList()
        {
            var list = SiteDriver.FindDisplayedElementsText(SearchFiltersLabelCss, How.CssSelector);
            return list;
        }

        public string GetInputValueByLabel(string label)
        {
            return
                 JavaScriptExecutor.FindElement(
                     string.Format(InputFieldByLabelCssTemplate , label))
                     .GetAttribute("value"); 
        }

        public string GetMultipleInputValueByLabel(string label, int index)
        {
            return
                SiteDriver.FindElement(string.Format(MultipleInputFieldByLabelXpathTemplate, label, index), How.XPath)
                    .GetAttribute("value");
        }

        public string GetInputValueByLabelPlaceholder(string label)
        {
            return
               JavaScriptExecutor.FindElement(
                    string.Format(InputFieldByLabelCssTemplate, label)
                    ).GetAttribute("placeholder");
        }

        public string GetInputAttributeValueByLabel(string label, string attribute)
        {
            return
                 JavaScriptExecutor.FindElement(
                     string.Format(InputFieldByLabelCssTemplate, label))
                     .GetAttribute(attribute);
        }

        public bool IsInputFieldForRespectiveLabelDisabled(string label)
          {
              return JavaScriptExecutor.IsElementPresent(
                  string.Format(InputFieldDisbaledCheckByLabelCssTemplate, label));
          }
       
        public int FilterCountByLabel()
          {
             return JavaScriptExecutor.FindElements(LabelCountOfAllFilters, How.CssSelector, "Text").Count;
          }

        public void ClickOnDropdownListByLabel(string label)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate,label));
        }

        public string GetValueOfLabel(int row=1)
        {
            return SiteDriver.FindElement(string.Format(LabelValueOfAFilter, row), How.CssSelector).Text;
        }

        public bool IsWorkListIconPresent()
        {
            return SiteDriver.IsElementPresent(WorkListIconCssLocator, How.CssSelector);
        }

        public string GetTopHeaderName()
        {
            return SiteDriver.FindElement(TopHeaderCssLocator, How.CssSelector).Text;
        }
        public string GetSectionHeaderName()
        {
            return SiteDriver.FindElement(SectionHeaderCssLocator, How.CssSelector).Text;
        }
        
         public bool IsSwitchWorkListIconDisplayed()
        {
            return SiteDriver.IsElementPresent(SwitchWorkListIconCssLocator, How.CssSelector);
        }

        public void ClickOnHeader()
        {
            JavaScriptExecutor.ExecuteClick(TopHeaderCssLocator,How.CssSelector);
        }

        public void ClickOnDropDownInputList(string label)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListByLabelXPathTemplate,label), How.XPath);
        }

        #endregion
    }
}
