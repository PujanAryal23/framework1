
using System;
using System.Collections.Generic;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.Console;
using static System.String;

namespace Nucleus.Service.Support.Common
{
    public class SideWindow
    {
        private readonly CalenderPage _calenderPage;
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #region CONSTRUCTOR

        public SideWindow(CalenderPage calenderPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            _calenderPage = calenderPage;
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }
        #endregion
        #region SideWindowComponents
       
        private const string SideWindowBlockCSS = "section.component_nested_form";
        private const string HeaderCssLocator = "section.form_component >form>ul header label";
        private const string DropDownLabelXpathTemplate = "//*/section[contains(@class, 'form_component')]//*[label[text()='{0}']]//input";
        private const string AsteriskXpathTemplateByLabel= "//*/section[contains(@class, 'form_component')]//label[text()='{0}']/span";
        private const string DropDownListCSS = "ul.is_visible li";
        private const string DropDownListValueXpathTemplate = "//ul[contains(@class,'is_visible')]/li[text()='{0}']";
        private const string DropDownListDefaultValueXpathTemplateByLabel = "//*/section[contains(@class, 'form_component')]//label[text()='{0}']/../section/ul/li[1]";
        private const string InputBoxXpathTemplatebyLabel = "//section[contains(@class,'form_component')]//label[text()='{0}']/following-sibling::input";
        private const string InputBoxForRefManagerXpathTemplatebyLabel = "//*/ul[contains(@class, 'component_item_row')]//..//label[text()='{0}']/../input";
        private const string IFrameXpathTemplayebyLabel = "//section[contains(@class,'form_component')]//label[text()='{0}']/..//iframe";
        private const string TextareaTemplatebyLabel =
            "//section[contains(@class,'form_component')]//label[text()='{0}']/..//textarea";
        private const string SaveButtonCSSSelector = "section.form_component:not([style*='none']) div.form_buttons button.work_button";
        private const string SecondaryButtonCSSSelector = "section.form_component:not([style*='none']) div.form_buttons button.secondary_button";
        private const string CancelButtonCSSSelector = "section.form_component:not([style*='none']) div.form_buttons>span>span";
        private const string InputFieldByLabelCssTemplate = "div.radio_button_group:has(span:contains({0})),li.form_item:has(label:contains({0})) input,div.component_item:has(label:contains({0})) input";
        public const string MultiDropDownToggleIconXpathTemplate = "//label[text()='{0}']/../section/span[contains(@class,'select_toggle')]";
        public const string MultiSelectListedDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'list_options')]/li";
        public const string MultiSelectAvailableDropDownToggleValueXpathTemplate = "//label[text()='{0}']/..//section[contains(@class,'available_options')]/li";
        private const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";
        private const string DropDownInputListByLabelXPathTemplate = "//*/section[contains(@class, 'form_component')]//label[text()='{0}']/../section//ul//li";
        public const string SelectedDropDownOptionsXpathtemplate = "//div[label[text()='{0}']]//ul/li[contains(@class,'is_active')]";
        public const string InputFieldByLabelXpathTemplate = "//section[contains(@class,'component_header')]/..//label[text()='{0}']//..//input";
        private const string SelectedInputFieldByLabelCssTemplate = "div.select_component:has(label:contains({0})) input";
        private const string ValueByLabelXpathTemplate =
            "//label[contains(text(),'{0}')]/following-sibling::span";
        public const string CheckBoxXPathByLabelTemplate = "//div[div[text()='{0}']]/span";
        public const string EditIconCssSelector = "span.small_edit_icon";
        public const string DropDownInputListValueByLabelAndIndexXPathTemplate = "//label[text()='{0}']/..//section//ul//li[{1}]";

        public const string DropDownLabelCssSelector =
            "div.component_item:has(label:contains({0})) div.select_component input";
        #endregion

        #region  PUBLIC METHOD

        public void SetInputInInputFieldByLabel(string label, string value, bool pressTab = false)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            if(pressTab)
                element.SendKeys(Keys.Tab);
        }

        public void SelectDropDownListByIndex(string label, int index)
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

            if (!SiteDriver.IsElementPresent(string.Format(DropDownInputListValueByLabelAndIndexXPathTemplate, label, index), How.XPath))
                JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndIndexXPathTemplate, label, index), How.XPath);
            SiteDriver.WaitToLoadNew(300);
            Console.WriteLine("Index <{0}> Selected in <{1}> ", index, label);
        }

        public string GetHeaderText()
        {
            return SiteDriver.FindElement(HeaderCssLocator, How.CssSelector).Text;
        }

        public bool IsCheckboxDisabledByLabel(string label)
        {
            return SiteDriver.FindElement(string.Format(CheckBoxXPathByLabelTemplate, label),How.XPath).GetAttribute("Class").Contains("is_disabled");
        }

        public bool IsCheckboxCheckedByLabel(string label)
        {
            return SiteDriver.FindElement(string.Format(CheckBoxXPathByLabelTemplate, label), How.XPath).GetAttribute("Class").Contains("active");
        }

        public void ClickOnCheckBoxByLabel(string label)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(CheckBoxXPathByLabelTemplate, label), How.XPath);
            SiteDriver.WaitToLoadNew(200);
        }

        public void CheckOrUncheckByLabel(string label, bool check = true)
        {
            var isOptionChecked = IsCheckboxCheckedByLabel(label);

            if (isOptionChecked)
            {
                if (!check)
                    ClickOnCheckBoxByLabel(label);
            }
            else
            {
                if(check)
                    ClickOnCheckBoxByLabel(label);
            }
        }

        public string GetValueByLabel(string label)
        {
            return SiteDriver.FindElement(string.Format(ValueByLabelXpathTemplate, label), How.XPath).Text;
        }

        public bool IsSideWindowBlockPresent()
        {
            return SiteDriver.IsElementPresent(SideWindowBlockCSS, How.CssSelector);
        }

        public void SelectDropDownValue(string label, string text, bool directSelect = true) // directSelect = false to check type-ahead
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            if (directSelect)
            {
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                SiteDriver.WaitToLoadNew(300);
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownListValueXpathTemplate, text), How.XPath);
            }
            else
            {
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                element.ClearElementField();
                element.SendKeys(text);
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownListValueXpathTemplate, text), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            }
            Console.WriteLine("<{0}> Selected in <{1}>", text, label);
            
        

        }

        public bool IsAsertiskPresent(string label)
        {
            //return  SiteDriver.FindElement(string.Format(AsteriskXpathTemplateByLabel,"Flag"),How.XPath).Text.Equals("*");
            return SiteDriver.FindElement(string.Format(AsteriskXpathTemplateByLabel, label), How.XPath).Text
                .Equals("*");

        }

        public string GetPlaceHolderText(string label)
        {
            return SiteDriver.FindElementAndGetAttribute(string.Format(DropDownLabelXpathTemplate, label), How.XPath,
                "placeholder");

        }

        public string GetPlaceHolderTextForDate(string label, int dateIndex = 1)
        {
            return SiteDriver.FindElementAndGetAttribute(string.Format(DropDownLabelXpathTemplate + "[{1}]", label, dateIndex), How.XPath,
                "placeholder");

        }

        public void FillInputBox(string label,string text,bool pressTab=false, bool isEditReference=false)
        {
            if (!isEditReference)
            {
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath));
                var element = SiteDriver.FindElement(string.Format(InputBoxXpathTemplatebyLabel, label),
                    How.XPath);
                element.ClearElementField();
                element.SendKeys(text);
                if (pressTab)
                    element.SendKeys(Keys.Tab);
                //SiteDriver.FindElementAndSetText(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath, text);
                SiteDriver.WaitToLoadNew(1000);
            }
            else
            {
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(InputBoxForRefManagerXpathTemplatebyLabel, label), How.XPath));
                var element = SiteDriver.FindElement(string.Format(InputBoxForRefManagerXpathTemplatebyLabel, label),
                    How.XPath);
                element.ClearElementField();
                element.SendKeys(text);
                if (pressTab)
                    element.SendKeys(Keys.Tab);
                //SiteDriver.FindElementAndSetText(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath, text);
                SiteDriver.WaitToLoadNew(1000);
            }
            

        }

        public bool IsInputFieldPresentByLabel(string label)
        {
            return JavaScriptExecutor.IsElementPresent(String.Format(InputFieldByLabelCssTemplate, label));
        }

        public void FillTextAreaBox(string label, string text, int maxlength = 0)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(TextareaTemplatebyLabel, label), How.XPath));
            var element = SiteDriver.FindElement(string.Format(TextareaTemplatebyLabel, label),
                How.XPath);
            element.ClearElementField();
            JavaScriptExecutor.SendKeys(text,string.Format(TextareaTemplatebyLabel, label), How.XPath);
            SiteDriver.WaitToLoadNew(1000);

           

        }

        public IWebElement GetInputBox(string label, bool isEditClaimReference = false)
        {
            if (!isEditClaimReference)
                return SiteDriver.FindElement(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath);
            else
                return SiteDriver.FindElement(string.Format(InputBoxForRefManagerXpathTemplatebyLabel, label),
                    How.XPath);
        }

        public string GetTextAreaData(string label)
        {
            return SiteDriver.FindElement(string.Format(TextareaTemplatebyLabel, label), How.XPath).GetAttribute("value");

        }

        public void ClearTextArea(string label){
            
            SiteDriver.FindElement(string.Format(TextareaTemplatebyLabel, label), How.XPath)
                .ClearElementField();
            SiteDriver.FindElement(string.Format(TextareaTemplatebyLabel, label), How.XPath)
                .SendKeys(Keys.Backspace);
        }

        public string GetInputFieldText(string label, bool isReferenceManager = false)
        {
            if (!isReferenceManager)
            {
                var element =
                    SiteDriver.FindElement(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath);
                if (element.Text == "")
                    return element.GetAttribute("value");
                return element.Text;
            }
            else
            {
                var element = SiteDriver.FindElement(string.Format(InputBoxForRefManagerXpathTemplatebyLabel, label), How.XPath);
                if (element.Text == "")
                    return element.GetAttribute("value");
                return element.Text;
            }

        }

        public bool IsTextBoxDisabled(string label)
        {
            
            return !SiteDriver.FindElement(string.Format(InputBoxXpathTemplatebyLabel, label), How.XPath).Enabled;
        }

        public bool IsDropDownBoxDisabled(string label)
        {

            return !SiteDriver.FindElement(string.Format(DropDownLabelXpathTemplate, label), How.XPath).Enabled;
        }

        public bool Save(bool checkIsDisabled=false,bool waitForWorkingMessage=false)
        {
            SiteDriver.WaitToLoadNew(3000);
            if (checkIsDisabled)
                return !SiteDriver.FindElement(SaveButtonCSSSelector, How.CssSelector).Enabled;
            JavaScriptExecutor.ExecuteClick(SaveButtonCSSSelector, How.CssSelector);
            SiteDriver.WaitToLoadNew(5000);
            if(waitForWorkingMessage)
                WaitForWorking();
            return true;
        }

        public bool SaveWithoutJs(bool checkIsDisabled = false, bool waitForWorkingMessage = false)
        {
            SiteDriver.WaitToLoadNew(3000);
            if (checkIsDisabled)
                return !SiteDriver.FindElement(SaveButtonCSSSelector, How.CssSelector).Enabled;
            SiteDriver.FindElement(SaveButtonCSSSelector, How.CssSelector).Click();
            SiteDriver.WaitToLoadNew(3000);
            if (waitForWorkingMessage)
                WaitForWorking();
            return true;
        }

        public void WaitForWorking()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitToLoadNew(300);
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public bool IsEditDisabled()
        {
            SiteDriver.WaitToLoadNew(3000);
            return SiteDriver.FindElement(EditIconCssSelector, How.CssSelector).Enabled;
        }
        
        public void ClickOnSecondaryButton()
        {
            SiteDriver.WaitToLoadNew(3000);
            JavaScriptExecutor.ExecuteClick(SecondaryButtonCSSSelector, How.CssSelector);
            SiteDriver.WaitToLoadNew(3000);
        }

        public string GetPrimaryButtonName()
        {
            return SiteDriver.FindElement(SaveButtonCSSSelector, How.CssSelector).Text;
        }

        public void ClickOnEditIcon()=>
            SiteDriver.FindElement(EditIconCssSelector, How.CssSelector).Click();

        public string GetSecondaryButtonName()
        {
            return SiteDriver.FindElement(SecondaryButtonCSSSelector, How.CssSelector).Text;
        }

        public bool IsSaveButtonPresent()
        {
            return SiteDriver.IsElementPresent(SaveButtonCSSSelector, How.CssSelector);
        }

        public bool Cancel(bool checkIsDisabled = false)
        {
            if (checkIsDisabled)
                return !SiteDriver.FindElement(CancelButtonCSSSelector, How.CssSelector).Enabled;
            JavaScriptExecutor.ExecuteClick(CancelButtonCSSSelector, How.CssSelector);
            SiteDriver.WaitToLoadNew(3000);
            return true;
        }

        public bool IsIFrameDisabled(string label)
        {
            
            SiteDriver.SwitchFrameByXPath(string.Format(IFrameXpathTemplayebyLabel, label));
            var text = SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable");
            SiteDriver.SwitchBackToMainFrame();
            return text.Equals("false");


        }
        public string GetIFrameData(string label)
        {
           
            SiteDriver.SwitchFrameByXPath(string.Format(IFrameXpathTemplayebyLabel, label));
            var text = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return text;
        }
        public void ClearIFrame(string label)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(IFrameXpathTemplayebyLabel, label), How.XPath));
            SiteDriver.SwitchFrameByXPath(string.Format(IFrameXpathTemplayebyLabel, label));
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(Keys.Backspace);
            SiteDriver.WaitToLoadNew(300);
            SiteDriver.SwitchBackToMainFrame();
        }
        public void FillIFrame(string label, string text, int maxlength=0)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(string.Format(IFrameXpathTemplayebyLabel, label), How.XPath));
            SiteDriver.SwitchFrameByXPath(string.Format(IFrameXpathTemplayebyLabel, label));
            SendValuesOnTextArea(label, text);

        }


        public List<string> GetDropDownList(string label, bool removeNull = false)
        {
            if (removeNull)
            {
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                var list = JavaScriptExecutor.FindElements(string.Format(DropDownListCSS, label), How.CssSelector, "Text");
                list.RemoveAt(0);
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                return list;
            }
            else
            {

                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                var list = JavaScriptExecutor.FindElements(string.Format(DropDownListCSS, label), How.CssSelector, "Text");
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownLabelXpathTemplate, label), How.XPath);
                return list;
            }
        }


        public string GetDropDownListDefaultValue(string label)
        {
            var element = SiteDriver.FindElementAndGetAttribute(string.Format(DropDownListDefaultValueXpathTemplateByLabel, label), How.XPath, "innerHTML");
            return element;

        }

        public bool Check_Maxlength_by_Label(string label, string length, bool isEditClaimReference = false)
        {
            var a = GetInputBox(label, isEditClaimReference)
                .GetAttribute("maxlength");
            return a.Contains(length);
        }

        public string GetInputValueByLabel(string label)
        {
            return
                JavaScriptExecutor.FindElement(
                        string.Format(InputFieldByLabelCssTemplate, label))
                    .GetAttribute("value");
        }

        public void SetDateFieldFrom(string dateName, string date)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(1)",
                dateName));
            _calenderPage.SetDate(Convert.ToDateTime(date).ToString("MM/d/yyyy"));
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }

        public void SetDateFieldTo(string dateName, string date)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(2)", dateName));
            _calenderPage.SetDate(Convert.ToDateTime(date).ToString("MM/d/yyyy"));
            SiteDriver.WaitToLoadNew(200);
            Console.WriteLine("<{0}> Selected:<{1}>", dateName, date);
        }

        public string GetDateFieldFrom(string dateName)
        {
            return
                JavaScriptExecutor.FindElement(
                        string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(1)", dateName))
                    .GetAttribute("value");
        }

        public string GetDateFieldTo(string dateName)
        {
            return
                JavaScriptExecutor.FindElement(
                    string.Format(InputFieldByLabelCssTemplate + ":nth-of-type(2)", dateName)
                ).GetAttribute("value");
        }

        public void SetAndTypeDateField(string dateName, string date, int col)
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

        public void ScrollIntoView(string cssLocator)
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDivUsingJquery(cssLocator);
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

        public void SelectSearchDropDownListForMultipleSelectValue(string label, string value)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
            Console.WriteLine("<{0}> set in <{1}>", label, value);
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
            //JavaScriptExecutors.ExecuteClick(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            SiteDriver.FindElement(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath).Click();
            var list = JavaScriptExecutor.FindElements(string.Format(MultiSelectListedDropDownToggleValueXpathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(string.Format(MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            return list;
        }

        public string GetInputAttributeValueByLabel(string label, string attribute)
        {
            return
                JavaScriptExecutor.FindElement(
                        string.Format(InputFieldByLabelCssTemplate, label))
                    .GetAttribute(attribute);
        }

        public List<string> GetAvailableDropDownList(string label)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(InputFieldByLabelCssTemplate, label));
            element.Click();
            Console.WriteLine("Looking for <{0}> List", label);
            SiteDriver.WaitToLoadNew(500);
            var list = JavaScriptExecutor.FindElements(string.Format(DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            if (list.Count == 0)
            {
                JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
                SiteDriver.WaitToLoadNew(500);
                list = JavaScriptExecutor.FindElements(string.Format(DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            }
            JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
            Console.WriteLine("<{0}> Drop down list count is {1} ", label, list.Count);
            return list;
        }

        public string SelectedDropDownOptionsByLabel(string label)
        {
            
            var value= JavaScriptExecutor.FindElement(string.Format(SelectedInputFieldByLabelCssTemplate, label)).GetAttribute("value");
           

            return value;
        }

        public void SelectDropDownListValueByLabel(string label, string value, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(string.Format(InputFieldByLabelXpathTemplate, label), How.XPath);
            if (!GetDropDownInputFieldByLabel(label).Equals(value))
            {
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
                if (!directSelect) element.SendKeys(value);
                if (!SiteDriver.IsElementPresent(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath)) JavaScriptExecutor.ClickJQuery(string.Format(InputFieldByLabelCssTemplate, label));
                JavaScriptExecutor.ExecuteClick(string.Format(DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            }
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public void MouseOutFromDropdown(string label)
        {
            JavaScriptExecutor.ExecuteMouseOut(string.Format(InputFieldByLabelXpathTemplate, label), How.XPath);

        }

        public string GetDropDownInputFieldByLabel(string label)
        {
            return SiteDriver.FindElement(string.Format(InputFieldByLabelXpathTemplate, label), How.XPath)
                .GetAttribute("value");

        }

        public bool IsEditIconPresent()
        {
            return SiteDriver.IsElementPresent(EditIconCssSelector, How.CssSelector);
        }

        public bool IsEditIconDisabled()
        {
            return SiteDriver.FindElement(EditIconCssSelector, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsDropDownPresentByLabel(string label)
        { 
            bool result = JavaScriptExecutor.IsElementPresent(string.Format(DropDownLabelCssSelector, label));
            return result;
        }

        public void SendValuesOnTextArea(string note, string text, bool handlePopup = true)
        {
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField(true);
            SiteDriver.WaitToLoadNew(1000);
            if (IsNullOrEmpty(text))
            {
                SiteDriver.FindElement("body", How.CssSelector).SendKeys(Keys.Backspace);

            }

            else
            {
                if (text.Length >= 1990)
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(0, text.Length - 4), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);//wait for removing last character which will take few ms
                    WriteLine("Note set to {0}", text);
                }
                else
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(1, text.Length - 1), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement("body", How.CssSelector).SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                }
            }
            WriteLine("{0} set to {1}", note, text);
            SiteDriver.SwitchBackToMainFrame();
            if (handlePopup && IsPageErrorPopupModalPresent())
                ClosePageError();
        }
        public bool IsPageErrorPopupModalPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id), 500);
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id, 200);
        }

        public void ClosePageError()
        {
            JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.PageErrorCloseId, How.Id);
            WriteLine("Closed the modal popup");
        }


        #endregion


    }
}