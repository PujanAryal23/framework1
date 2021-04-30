using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Utils
{
    /// <summary>
    /// Handles JavaScript operations compatible with both Microsoft Internet Explorer and Netscape browsers. This class cannot be inherited.
    /// </summary>
    public sealed class JavaScriptExecutor
    {
        private const string JqueryActive = "return $.active;";

        /// <summary>
        /// A JavaScript template executes mouse over element. 
        /// </summary>
        private const string JavaScriptExecutorMouseOverTemplate = @"
                if(navigator.appName == 'Netscape') {
                var dispatchMouseEvent = function(target, var_args) { 
                              var e = document.createEvent('MouseEvents');
                              e.initEvent.apply(e, Array.prototype.slice.call(arguments, 1));
                              target.dispatchEvent(e);}
                                dispatchMouseEvent(arguments[0], 'mouseover', true, true);}
                                else {
                                arguments[0].fireEvent('onmouseover');}";

        /// <summary>
        /// A JavaScript template executes mouse out of element.
        /// </summary>
        private const string MouseOutTemplate = @"
                if(navigator.appName == 'Netscape') {
                var dispatchMouseEvent = function(target, var_args) { 
                              var e = document.createEvent('MouseEvents');
                              e.initEvent.apply(e, Array.prototype.slice.call(arguments, 1));
                              target.dispatchEvent(e);}
                                dispatchMouseEvent(arguments[0], 'mouseout', true, true);}
                                else {
                                arguments[0].fireEvent('onmouseout');}";

        /// <summary>
        /// A JavaScript template executes mouse click on element.
        /// </summary>
        private const string JavaScriptExecutorClickTemplate = @"
                if(navigator.appName == 'Netscape') {
                var dispatchMouseEvent = function(target, var_args) { 
                              var e = document.createEvent('MouseEvents');
                              e.initEvent.apply(e, Array.prototype.slice.call(arguments, 1));
                              target.dispatchEvent(e);}
                                dispatchMouseEvent(arguments[0], 'click', true, true);}
                                else {
                                arguments[0].click();}";

        private const string JavaScriptExecutorContextClickTemplate = @"
                if(navigator.appName == 'Netscape') {
                                var dispatchMouseEvent = function(target, var_args) { 
                                              var e = document.createEvent('MouseEvents');
                                              e.initEvent.apply(e, Array.prototype.slice.call(arguments, 1));
                                              target.dispatchEvent(e);}
                                                dispatchMouseEvent(arguments[0], 'contextmenu', true, true);}
                                                else {
                                                arguments[0].fireEvent('contextmenu');}";
        

        private const string ElementFindByXpath =
            @"return document.evaluate({0}, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue";


        /// <summary>
        /// A JavaScript closes window.
        /// </summary>
        private const string JsWindowClose = "window.close();";

        private const string JsScrollToLastLi = @"$(""html,body"").animate({scrollTop: $('ul#cart-items li:last')});";

        private const string JsScrollToView = @"document.getElementById('{0}').rows[0].cells[{1}].scrollIntoView(true);";

        private const string JsScrollToViewOfClassName = @"document.getElementsByClassName('{0}')[0].scrollIntoView(true);";

        private const string text = @"document.getElementsByClassName('{0}')[0].scrollIntoView(true);";

        private const string JsScrollToSpecificDiv = @"document.querySelector('{0}').scrollIntoView('{1}');";

        private const string JsScrollHeightTemplate = @"return document.querySelector('{0}').scrollHeight;";
        private const string JsScrollWidthTemplate = @"return document.querySelector('{0}').scrollWidth;";
        private const string JsClientHeightTemplate = @"return document.querySelector('{0}').clientHeight;";
        private const string JsClientWidthTemplate = @"return document.querySelector('{0}').clientWidth;";

        private const string JSDownloadedFilename =
            @"return document.querySelector('downloads-manager').shadowRoot.querySelectorAll('downloads-item')[{0}].shadowRoot.querySelector('div div#title-area>a');";

        private const string JSDownloadedFilenameListCount =
            @"return document.querySelector('downloads-manager').shadowRoot.querySelectorAll('downloads-item').length;";

        private const string JQueryScript = "return $('{0}').get(0)";
        private const string JQueryScriptScrollIntoView = "$('{0}').get(0).scrollIntoView('{1}');";
        private const string JQueryScriptMultipleResultValues = @"var inp = $('{0}'); 
                                                                function returnFilesList( nameArray )
                                                                    {{
                                                                    for (var i = 0; i < inp.length; ++i){{
                                                                        if('{1}'=='Attribute')
                                                                        {{
                                                                            nameArray.push(inp[i].getAttribute('{2}').trim());
                                                                        }}
                                                                        else
                                                                        {{
                                                                         nameArray.push(inp[i].innerText.trim());
                                                                        }}
                                                                    }}
                                                                    return nameArray;
                                                                    }}
                                                                var nameArray = []; 
                                                                return returnFilesList(nameArray);";
        private const string JQueryScriptMultipleResultValuesForXPath = @"
function _x(STR_XPATH) {{
    var xresult = document.evaluate(STR_XPATH, document, null, XPathResult.ANY_TYPE, null);
    var xnodes = [];
    var xres;
    while (xres = xresult.iterateNext()) {{
        xnodes.push(xres);
    }}

    return xnodes;
}}
var inp = _x(""{0}""); 
                                                                function returnFilesList( nameArray )
                                                                    {{
                                                                    for (var i = 0; i < inp.length; ++i){{
                                                                        if('{1}'=='Attribute')
                                                                        {{
                                                                            nameArray.push(inp[i].getAttribute('{2}').trim());
                                                                        }}
                                                                        else
                                                                        {{
                                                                         nameArray.push(inp[i].innerText.trim());
                                                                        }}
                                                                    }}
                                                                    return nameArray;
                                                                    }}
                                                                var nameArray = []; 
                                                                return returnFilesList(nameArray);";
        private const string JQueryScriptMultipleResult = "return $('{0}')";
        private const string JQueryScriptSetText = "$('{0}').val({1})";
        private const string JQueryScriptGetText = "return $('{0}').val()";
        private const string JQueryScriptClickAndGet = "$('{0}').click();var elem=$('{1}'); return elem.get(0);";
        private const string JQueryScriptClick = "$('{0}').click();";

        private const string JQueryScriptSetProcCode = "[{0}].map( function(item) {{$('{1}').val(item);$('{1}').blur();$('{2}').click();}})";

        private const string JsGetUploadFileListObject = @"var inp = document.getElementsByClassName('{0}'); 
                                                                function returnFilesList( nameArray )
                                                                    {{
                                                                    for (var i = 0; i < inp[0].files.length; ++i){{ 
                                                                            nameArray.push(inp[0].files.item(i).name); }}
                                                                    return nameArray;
                                                                    }}
                                                                var nameArray = []; 
                                                                return returnFilesList(nameArray);";

        private const string JQuerySelectByTextInList = @"return $('{0}').filter(function(index)
                                                            {{
                                                                return $(this).text() === '{1}';
                                                            }}).get(0);";

        private const string JQueryScriptClickByText = @"return $('{0}').filter(function(index)
                                                            {{
                                                                return $(this).text() === '{1}';
                                                            }}).get(0).click();";

        private const string JQueryScriptToWaitUntilElementExists = @"var callback = arguments[arguments.length - 1]; var checkExist = setInterval(function() 
                                                                {{
                                                                   if ($('{0}').length) 
	                                                                {{
                                                                        callback($('{0}').get(0));
                                                                    }}
                                                                }}, {1});";

        private const string JSToFindWindowsTab = @"return window.toolbar.visible;";

        public static object GetUploadFileListObject(string select)
        {
            return Execute(string.Format(JsGetUploadFileListObject, select));
        }

        /// <summary>
        /// return scrollWidth of Div
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public static int ScrollWidth(string select)
        {
            return Convert.ToInt32(Execute(string.Format(JsScrollWidthTemplate, select)));
        }

        /// <summary>
        /// return ClientHeight of Div
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public static int ClientHeight(string select)
        {
            return Convert.ToInt32(Execute(string.Format(JsClientHeightTemplate, select)));
        }

        /// <summary>
        /// return ClientWidth of Div
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public static int ClientWidth(string select)
        {
            return Convert.ToInt32(Execute(string.Format(JsClientWidthTemplate, select)));
        }

        /// <summary>
        /// return scrollHeight of Div
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public static int ScrollHeight(string select)
        {
            return Convert.ToInt32(Execute(string.Format(JsScrollHeightTemplate, select)));
        }

        /// <summary>
        /// Executes a javascript mouse over element.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void ExecuteMouseOver(string select, How selector)
        {
            ExecuteScript(JavaScriptExecutorMouseOverTemplate, SiteDriver.FindElement(select, selector));
        }

        public static IWebElement GetDownloadedFilename(int index=0)
        {
           //return  (string)Execute(JSDownloadedFilename);
            return (IWebElement)Execute(string.Format(JSDownloadedFilename,index));
        }
        public static int GetDownloadedFilenameList()
        {
           //return  (string)Execute(JSDownloadedFilename);
            return Convert.ToInt32(Execute(JSDownloadedFilenameListCount));
        }

        /// <summary>
        /// Execute jQuery status script
        /// </summary>
        /// <returns></returns>
        public static bool ExecuteJqueryStatus()
        {
            return Execute(JqueryActive).ToString() == "0";
        }

        /// <summary>
        /// Execute javascript to find whether opened window is popup or tab
        /// </summary>
        /// <returns></returns>
        public static bool IsWindowOpenedAsTab()
        {
            return bool.Parse(Execute(JSToFindWindowsTab).ToString());
        }
        /// <summary>
        /// Executes a javascript mouseclick at element.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void ExecuteClick(string select, How selector)
        {
            SiteDriver.WaitToLoadNew(300);
            ExecuteScript(JavaScriptExecutorClickTemplate, SiteDriver.FindElement(select, selector));
        }

        /// <summary>
        /// Executes a javascritp mouseclick at element
        /// </summary>
        /// <param name="element"></param>
        public static void ExecuteClick(IWebElement element)
        {
            SiteDriver.WaitToLoadNew(300);
            ExecuteScript(JavaScriptExecutorClickTemplate, element);
        }

        /// <summary>
        /// Executes a javascript mouse right click at element.
        /// </summary>
        /// <param name="select">An element</param>
        /// <param name="selector">The locating mechanism to use</param>
        public static void ExecuteContextClick(string select, How selector)
        {
            ExecuteScript(JavaScriptExecutorContextClickTemplate, SiteDriver.FindElement(select, selector));
        }

        /// <summary>
        /// Executes a javascript to scroll to view element
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="colIndex"></param>
        public static void  ExecuteToScrollToView(string elementId, string colIndex)
        {
            SiteDriver.JsExecutor.ExecuteScript(string.Format(JsScrollToView, elementId, colIndex));
        }

        public static void ExecuteToScrollToLastLi(string element, bool bottom = true)
        {
            SiteDriver.JsExecutor.ExecuteScript(JsScrollToLastLi, element, bottom);
        }

        public static void  ExecuteToScrollToSpecificDiv(string element,bool bottom=true)
        {
            SiteDriver.JsExecutor.ExecuteScript(string.Format(JsScrollToSpecificDiv, element,bottom));
            SiteDriver.WaitToLoadNew(200);
        }

        public static void ExecuteToScrollToSpecificDivUsingJquery(string element, bool bottom = true)
        {
            SiteDriver.JsExecutor.ExecuteScript(string.Format(JQueryScriptScrollIntoView, element, bottom));
            SiteDriver.WaitToLoadNew(200);
        }

        /// <summary>
        /// Executes a javascript to scroll to view element
        /// </summary>
        /// <param name="elementClassName"></param>
        public static void ExecuteToScrollToView(string elementClassName)
        {
            SiteDriver.JsExecutor.ExecuteScript(string.Format(JsScrollToViewOfClassName, elementClassName));
        }

        public static void ExecuteScrollToMostRight(string elementClassName)
        {
            ExecuteScript("arguments[0].scrollLeft = arguments[0].offsetWidth", SiteDriver.FindElement(elementClassName,How.ClassName));
        }

       

        /// <summary>
        /// Executes a javascript mouseout at element.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void ExecuteMouseOut(string select, How selector)
        {
            ExecuteScript(MouseOutTemplate, SiteDriver.FindElement(select, selector));
        }

        /// <summary>
        /// Send keys using javascript.
        /// </summary>
        /// <param name="strKey">A value to send.</param>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void SendKeys(string strKey,string select,How selector)
        {
            ExecuteScript("arguments[0].value ='" + strKey + "';", Click(select,selector));
        }

        public static void SendKeysToInnerHTML(string strKey, string select, How selector)
        {
            ExecuteScript("arguments[0].innerHTML ='" + strKey + "';", Click(select, selector));
        }
        /// <summary>
        /// Clear using javascript.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void Clear(string select,How selector)
        {
            var element = Click(select, selector);
            ExecuteScript("arguments[0].value ='';", element);
            element.SendKeys(Keys.Escape);
        }

        /// <summary>
        /// Clear using javascript.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        public static void Clear(IWebElement element)
        {
            
            ExecuteScript("arguments[0].value ='';", element);
            element.SendKeys(Keys.Escape);
        }

        /// <summary>
        /// Executes a javascript.
        /// </summary>
        /// <param name="script">A JavaScript to execute.</param>
        /// <param name="args">Array of object arguments.</param>
        public static void ExecuteScript(string script, params object[] args)
        {
            SiteDriver.JsExecutor.ExecuteScript(script, args);
        }

        public static object Execute(string script)
        {
           return SiteDriver.JsExecutor.ExecuteScript(script);
        }

        /// <summary>
        /// Executes a javascript window close.
        /// </summary>
        public static void WindowClose()
        {
            SiteDriver.JsExecutor.ExecuteScript(JsWindowClose);
        }

        /// <summary>
        /// Click on an element.
        /// </summary>
        /// <param name="select">An element.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <returns></returns>
        private static IWebElement Click(string select,How selector)
        {
            var element = SiteDriver.FindElement(select, selector);
            element.Click();
            return element;
        }

        public static IWebElement FindElement(string select)
        {
            return (IWebElement)Execute(string.Format(JQueryScript, select));
        }

        public static IWebElement FindElementByText(string select, string value)
        {
            return (IWebElement)Execute(string.Format(JQuerySelectByTextInList, select, value));
        }

        public static string GetText(string select)
        {
            return (string)Execute(string.Format(JQueryScriptGetText,select));
        }

        public static void SetText(string select, string value)
        {
             //Execute(string.Format(JQueryScriptSetText, select,value));
            FindElement(select).ClearElementField();
            FindElement(select).SendKeys(value);
        }

        public static IWebElement ClickAndGet(string select1, string select2,bool mulitpleScript=false)
        {
            if(mulitpleScript)
            {
                Execute(string.Format(JQueryScriptClick, select1));
                object element = null;
                SiteDriver.WaitForCondition(() =>
                    {
                        element = Execute(string.Format(JQueryScript, select2));
                        return element != null;
                    }, 3000
                );
                return (IWebElement)element;
            }
            var ele=Execute(string.Format(JQueryScriptClickAndGet, select1,select2));
            return (IWebElement) ele;
            
        }

        public static IWebElement ClickAndGetWithJquery(string select1, string select2, int interval = 100 )
        {
            object element = null;

            Execute(string.Format(JQueryScriptClick, select1));
            
            SiteDriver.WaitForCondition(() =>
                {
                    element = SiteDriver.JsExecutor.ExecuteAsyncScript(string.Format(JQueryScriptToWaitUntilElementExists, select2, interval ));
                    return element != null;
                }, 3000
                );
         
            return (IWebElement)element;
        }

        /// <summary>
        /// Executes a javascript mouseout at element.
        /// </summary>
        /// <param name="select">An element get by jquery</param>

        public static void ExecuteMouseOut(string select)
        {
            ExecuteScript(MouseOutTemplate, FindElement(select));
        }

        public static List<string> FindElements(string select, string selectSelector = "Text", bool isNull = false, How selector=How.CssSelector)
        {

            if (isNull)
            {
                var obj = (IList<IWebElement>)Execute(string.Format(JQueryScriptMultipleResult, select));
                return obj.Select(element => element.GetAttribute(selectSelector)).ToList();
            }
            var attb = selectSelector.Split(':').Select(x => x.Trim()).ToList();
            if (selector == How.CssSelector)
            {
                var obj = Execute(string.Format(JQueryScriptMultipleResultValues.Replace("'","\""), select,
                    selectSelector.StartsWith("Attribute") ? attb[0] : "", selectSelector.StartsWith("Attribute") ? attb[1] : ""));
                return ((IEnumerable) obj).Cast<string>().ToList();
            }
            var obj1 = Execute(string.Format(JQueryScriptMultipleResultValuesForXPath.Replace("'", "\""), select,
                selectSelector.StartsWith("Attribute") ? attb[0] : "", selectSelector.StartsWith("Attribute") ? attb[1] : ""));
            return ((IEnumerable)obj1).Cast<string>().ToList();


            //"return $('div.appeal-line-flag:has(span:contains(FRE)) li>span')"));
            //String[] allText = new String[all.Count];
            //int i = 0;
            //foreach (IWebElement element in all)
            //{
            //    allText[i++] = element.Text;
            //}
            //return elements;
        }

        public static int FindElementsCount(string select)
        {
            return FindElements(select,"").Count();
        }

        public static bool IsElementPresent(string select)
        {
            try
            {
                return (FindElement(select)!=null);
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        public static bool IsDownloadedFilePresent()
        {
            try
            {
                return (GetDownloadedFilename() != null);
            }
            catch (Exception ex)
            {
                // Don't handle NotSupportedException
                if (ex is NotSupportedException)
                    throw;
                return false;
            }
        }

        public static void ClickJQuery(string select)
        {
             Execute(string.Format(JQueryScriptClick, select));
        }

        public static void ClickJQueryByText(string select,string text)
        {
            Execute(string.Format(JQueryScriptClickByText, select,text));
        }

        public static IWebElement ExecuteFindByXpathClick(string select)
        {
            return (IWebElement)Execute(string.Format(ElementFindByXpath, "\"" + select + "\""));
        }

        public static void SetProcCode(string procCodeArrary, string input, string searchIcon)
        {
            Execute(string.Format(JQueryScriptSetProcCode, procCodeArrary, input, searchIcon));
        }

    }
}
