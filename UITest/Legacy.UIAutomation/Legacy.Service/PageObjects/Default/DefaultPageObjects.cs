using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Default
{
    public class DefaultPageObjects : PageBase
    {
        #region PRIVATE FILEDS

        public  const string BackButtonXPath = "//img[contains(@src, '_Images/Btn_Back.jpg')]";
        public const string WelcomePageButtonXPath = "//img[contains(@src, '_Images/Btn_MainMenu.jpg')]";
        public const string LogoutButtonXPath = "//img[contains(@src, '_Images/Btn_Logout.jpg')]";
        
        public const string MonthCssLocator = "span.ui-datepicker-month";
        public const string YearCssLocator = "span.ui-datepicker-year";
        public const string PreviousMonthCssLocator = "a.ui-datepicker-prev";
        public const string NextMonthCssLocator = "a.ui-datepicker-next";
        public const string DateXPathTemplate ="//table[contains(@class,'ui-datepicker-calendar')]/tbody/tr/td/a[text()='{0}']";

        

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = BackButtonXPath)]
        public ImageButton BackButton;

        [FindsBy(How = How.XPath, Using = WelcomePageButtonXPath)]
        public ImageButton WelcomePageButton;

        [FindsBy(How = How.XPath, Using = LogoutButtonXPath)]
        public ImageButton LogoutButton;

            

        #endregion
        #region CONSTRUCTOR

        public DefaultPageObjects(string pageUrl)
            : base(pageUrl)
        {
        }

        public DefaultPageObjects(string baseUrl, string pageUrl)
            : base(baseUrl, pageUrl)
        {

        }

        #endregion
    }
}
