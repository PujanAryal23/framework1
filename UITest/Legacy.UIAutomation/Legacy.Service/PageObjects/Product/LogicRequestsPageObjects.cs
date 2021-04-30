using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;

namespace Legacy.Service.PageObjects.Product
{
    public class LogicRequestsPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string SearchBtnXPath = ".//img[@title='Search']";
        private const string ClearBtnXPath = ".//img[@title='Clear Form']";
        private const string NotifyClientBtnXPath = ".//img[@title='Notify Client by E-mail']";
        private const string ClientReceiveDateBeginXPath = "//table/tbody/tr[3]/td[2]/img";
        private const string ClientReceiveDateToXPath = "//table/tbody/tr[3]/td[4]/img";
        private const string LogicDateBeginXPath = "//table/tbody/tr[4]/td[2]/img";
        private const string LogicDateToXPath = "//table/tbody/tr[4]/td[4]/img";
        private const string ClientBeginDateName ="CliBegDate";
        private const string ClientEndDateName = "CliEndDate";
        private const string LogicBeginDateName ="LogBegDate";
        private const string LogicEndDateName = "LogEndDate";
        
        #endregion  

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = SearchBtnXPath)]
        public ImageButton SearchBtn;

        [FindsBy(How = How.XPath, Using = ClearBtnXPath)]
        public ImageButton ClearBtn;

        [FindsBy(How = How.XPath, Using = NotifyClientBtnXPath)]
        public ImageButton NotifyClientBtn;

        [FindsBy(How = How.XPath, Using = ClientReceiveDateBeginXPath)]
        public Link ClientReceiveDateBeginCalLnk;

        [FindsBy(How = How.XPath, Using = ClientReceiveDateToXPath)]
        public Link ClientReceiveDateToCalLnk;

        [FindsBy(How = How.XPath, Using = LogicDateBeginXPath)]
        public Link LogicDateBeginCalLnk;

        [FindsBy(How = How.XPath, Using = LogicDateToXPath)]
        public Link LogicDateToCalLnk;

        [FindsBy(How = How.Name, Using = ClientBeginDateName)]
        public TextField ClientBeginDateTxt;

        [FindsBy(How = How.Name, Using = ClientEndDateName)]
        public TextField ClientEndDateTxt;

        [FindsBy(How = How.Name, Using = LogicBeginDateName)]
        public TextField LogicBeginDateTxt;

        [FindsBy(How = How.Name, Using = LogicEndDateName)]
        public TextField LogicEndDateTxt;


        #endregion  

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get {return string.Format(PageTitleEnum.LogicRequests.GetStringValue(),StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public LogicRequestsPageObjects()
            : base(ProductPageUrlEnum.LogicRequests.GetStringValue())
        {
        }
        
        #endregion
    }
}
