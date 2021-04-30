using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.SwitchClient;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public class SwitchClientSection
    {
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #region CONSTRUCTOR

        public SwitchClientSection(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }
        

        #endregion

        #region PRIVATE FIELDS
        private const string MostRecentHeaderXpath = "//div[@class='switch_client_header']/label[contains(text(),'Most Recent')]";

        private const string AllClientsHeaderCssXpath = "//div[@class='switch_client_header']/label[contains(text(),'All Clients')]";

        private const string MostRecentClientsCodesCss = "section.component_sidebar>section.sidebar_content>div:nth-of-type(1) li:nth-of-type(1)>label";

        private const string MostRecentClientsNamesCss = "section.component_sidebar>section.sidebar_content>div:nth-of-type(1) li:nth-of-type(2) span";

        private const string AllClientsCodesCss = "section.component_sidebar>section.sidebar_content>div:nth-of-type(2) li:nth-of-type(1)>span";

        private const string AllClientsNamesCss = "section.component_sidebar>section.sidebar_content>div:nth-of-type(2) li:nth-of-type(2) span";

        private const string SwitchClientWindowCssLocator = "section.component_sidebar.is_slider.is_clientSwitch:not(.is_hidden)";

        private const string CloseSideWindowButtonCssLocator = "span[title='Close Client Switch']";

        public const string SwitchClientBtnCssLocator = "a.switch_client";

        public const string SwitchClientTemplate = "//span[contains(text(),'{0}')]/../following-sibling::li[1]//span[contains(text(),'{1}')]";
            //"section.component_sidebar>section.sidebar_content>div:nth-of-type(2) li:nth-of-type(2)>span:contains({0})";
        #endregion

        #region PUBLIC PROPERTIES

        public void SwitchClientTo(ClientEnum clientName)
        {
            var oldHeaderName =
                SiteDriver.FindElement(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
            var element = SiteDriver.FindElement(string.Format(SwitchClientTemplate, clientName.ToString(), clientName.GetStringValue()),How.XPath);
            element.Click();
            StringFormatter.PrintMessage(string.Format("Switch client to : {0}", clientName)); 
            SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.FindElement(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text!= oldHeaderName);
            
           // JavaScriptExecutors.ClickJQueryByText(string.Format(SwitchClientTemplate,clientName.ToString(), clientName.GetStringValue()), clientName.GetStringValue());
        }

        public void ClickOnSwitchClient()
        {
            SiteDriver.FindElement(SwitchClientBtnCssLocator, How.CssSelector).Click();
        }
        
        public bool IsSwitchClientSideWindowPresent()
        {
            return SiteDriver.IsElementPresent(SwitchClientWindowCssLocator, How.CssSelector);
        }

        public bool IsMostRecentPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(MostRecentHeaderXpath, How.XPath);
        }

        public bool IsAllClientsPageHeaderPresent()
        {
            return SiteDriver.IsElementPresent(AllClientsHeaderCssXpath, How.XPath);
        }

        public void ClickOnCloseButton()
        {
            SiteDriver.FindElement(CloseSideWindowButtonCssLocator, How.CssSelector).Click();
            SiteDriver.WaitForCondition(() => !IsSwitchClientSideWindowPresent(),5000);
        }


        public List<string> GetMostRecentClientCodes()
        {
            var list = JavaScriptExecutor.FindElements(MostRecentClientsCodesCss, "Text");
            return list;
        }

        public List<string> GetMostRecentClientNames()
        {
            var list = JavaScriptExecutor.FindElements(MostRecentClientsNamesCss, "Text");
            return list;
        }

        public List<string> GetAllClientCodes()
        {
            var list = JavaScriptExecutor.FindElements(AllClientsCodesCss,"Text");
            return list;
        }

        public List<string> GetAllClientNames()
        {
            var list = JavaScriptExecutor.FindElements(AllClientsNamesCss,"Text");
            return list;
        }

        #endregion
    }
}
