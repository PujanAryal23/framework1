using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;

namespace Legacy.Service.PageObjects.Common
{
    public class CalendarPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC FIELDS/PROPERTIES

        private const string PreviousMonthBtnXPath = ".//img[@alt='previous month']";
        private const string NextMonthBtnXPath = ".//img[@alt='next month']";
        
        public const string MontYearXPath = ".//font[@color='#ffffff']";
        public const string DayPickerXPathTemplate = ".//a/font[@color='#000000' and contains(text(),'{0}')]";

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = PreviousMonthBtnXPath)]
        public ImageButton PreviousMonthBtn;

        [FindsBy(How = How.XPath, Using = NextMonthBtnXPath)]
        public ImageButton NextMonthBtn;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                return PageTitleEnum.Calendar.GetStringValue();
            }
        }

        #endregion

        #region CONSTRUCTOR

        public CalendarPageObjects()
        {
        }

        public CalendarPageObjects(string pageUrl)
            : base(pageUrl)
        {

        }

        #endregion
    }
}
