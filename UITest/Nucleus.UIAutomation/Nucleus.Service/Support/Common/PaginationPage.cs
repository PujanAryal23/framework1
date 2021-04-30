using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Base;

namespace Nucleus.Service.Support.Common
{
    public class PaginationPage : PageBase
    {

        #region CONSTANTS

        public const string WaitForGridToLoadMethod = "WaitForGridToLoad";

        #endregion

        #region ID

       
        
        #endregion

        #region TEMPLATES

        public const string NumPartTemplate = ".//div[contains(@class,'rgWrap rgNumPart')]/a[span[contains(text(),'{0}')]]";

        #endregion

        #region XPATH

        public const string NumPartXPath = ".//div[contains(@class,'rgWrap rgNumPart')]/a/span";
        public const string CurrentPageXPath = ".//a[contains(@class,'rgCurrentPage')]/span";
        public const string RgAdvPart = ".//div[@class='rgWrap rgAdvPart']/div/table/tbody/tr//td/input";
        public const string RadAjaxCssLocator = "div.RadAjax_Nucleus:not([style*='none'])>div.raDiv";

        #endregion


        #region ClassName

        public const string PageFirstXPath = ".//input[@class='rgPageFirst']";
        public const string PagePreviousXPath = ".//input[@class='rgPagePrev']";
        public const string PageNextXPath = ".//input[@class='rgPageNext']";
        public const string PageLastXPath = ".//input[@class='rgPageLast']";
        public const string RgInfoPartXPath = ".//div[@class='rgWrap rgInfoPart']";

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = CurrentPageXPath)]
        public Link PageCurrent;

        [FindsBy(How = How.XPath, Using = PageFirstXPath)]
        public Link PageFirst;

        [FindsBy(How = How.XPath, Using = PagePreviousXPath)]
        public Link PagePrevious;

        [FindsBy(How = How.XPath, Using = PageNextXPath)]
        public Link PageNext;

        [FindsBy(How = How.XPath, Using = PageLastXPath)]
        public Link PageLast;

        [FindsBy(How = How.XPath, Using = RgInfoPartXPath)]
        public TextLabel RgInfoPart;

        [FindsBy(How = How.XPath, Using = RgAdvPart)]
        public Link PageSize;

        #endregion
    }
}
