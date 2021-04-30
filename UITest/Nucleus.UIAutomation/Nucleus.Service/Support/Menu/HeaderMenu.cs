using System;
using System.Collections.Generic;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Menu
{
    public class HeaderMenu
    {
        #region Private VAriables
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;
        #endregion


        #region CONSTRUCTOR

        public HeaderMenu(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region HeaderMenu

        public const string Claim = "Claim";
        public const string Bill = "Bill";
        public const string Provider = "Provider";
        public const string Patient = "Patient";
        public const string Alliance = "Alliance";
        public const string Appeal = "Appeal";
        public const string Batch = "Batch";
        public const string Reports = "Reports";
        public const string Invoice = "Invoice";
        public const string Settings = "Settings";
        public const string Manager = "Manager";


        public const string Reference = "Reference";
        public const string Qa = "QA";
        public const string PreAuthorization = "Pre-Authorization";

        #endregion

        #region Locator Template

        public const string HeaderMenuLocatorTemplate = ".//div[@id='master_navigation']/ul/li/header[text()='{0}']";
        public const string HeaderMenuXPathTemplate = ".//div[@id='master_navigation']/ul/li[{0}]/header";

        public const string MenuOptionsClinicalOpsTemplate = ".//ul[@id='main_nav']/li[{0}]/header";
        public const string MenuLocatorTemplate = ".//ul/li/a[contains(text(),'{0}')] | .//ul/li/span[contains(text(),'{0}')]";
        public const string MenuLocatorSpanTemplate = ".//ul/li/span[contains(text(),'{0}')]";
        public const string NormalizedMenuLocatorTemplate = ".//ul/li/a[contains(normalize-space(),'{0}')] | .//ul/li/span[contains(normalize-space(),'{0}')]";
        #endregion

        /// <summary>
        /// Get header menu locator
        /// </summary>
        /// <param name="strHeaderMenu"></param>
        /// <returns>HeaderMenuLocatorTemplate</returns>
        public static string GetHeaderMenuLocator(String strHeaderMenu)
        {
            return String.Format(HeaderMenuLocatorTemplate, strHeaderMenu);
        }

        public static string GetElementLocatorTemplate(String menuOption)
        {
            return String.Format(MenuLocatorTemplate, menuOption);
        }

        public static string GetNormalizedElementLocatorTemplate(String menuOption)
        {
            return String.Format(NormalizedMenuLocatorTemplate, menuOption);
        }

        public static string GetElementLocatorTemplateSpan(String menuOption)
        {
            return String.Format(MenuLocatorSpanTemplate, menuOption);
        }

        public string GetMenuOptionsForClinicalOps(string position)
        {
            return SiteDriver.FindElement(string.Format(MenuOptionsClinicalOpsTemplate, position), How.XPath).Text;
        }

        public string GetMenuOption(int position)
        {
           return SiteDriver.FindElement(string.Format(HeaderMenuXPathTemplate, position), How.XPath).Text;
        }


    }
}