using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class LogicRequestsPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string NotifyClientBtnXPath = ".//img[@title='Notify Client by E-mail']";
        private const string ClientRecvdDateBeginCalXPath = ".//a[contains(@href, 'javascript:cal1.popup();')]";
        private const string ClientRecvdDateEndCalXPath = ".//a[contains(@href, 'javascript:cal2.popup();')]";
        private const string LogicDateBeginCalXPath = ".//a[contains(@href, 'javascript:cal3.popup();')]";
        private const string LogicDateEndCalXPath = ".//a[contains(@href, 'javascript:cal4.popup();')]";
        private const string FromClientRecvdDateName = "CliBegDate";
        private const string ToClientRecvdDateName = "CliEndDate";
        private const string FromLogicDateName = "LogBegDate";
        private const string ToLogicDateName = "LogEndDate";
        #endregion

        #region PAGEOBJECT PRPERTIES

        [FindsBy(How = How.XPath, Using = NotifyClientBtnXPath)]
        public ImageButton NotifyClientBtn;

        [FindsBy(How = How.XPath, Using = ClientRecvdDateBeginCalXPath)]
        public Link ClientRecvdDateBeginCalLink;

        [FindsBy(How = How.XPath, Using = ClientRecvdDateEndCalXPath)]
        public Link ClientRecvdDateEndCalLink;

        [FindsBy(How = How.XPath, Using = LogicDateBeginCalXPath)]
        public Link LogicDateBeginCalLink;

        [FindsBy(How = How.XPath, Using = LogicDateEndCalXPath)]
        public Link LogicDateEndCalLink;

        [FindsBy(How = How.Name, Using = FromClientRecvdDateName)]
        public TextField FromClientRecvdDateTxt;

        [FindsBy(How = How.Name, Using = ToClientRecvdDateName)] 
        public TextField ToClientRecvdDateTxt;

        [FindsBy(How = How.Name, Using = FromLogicDateName)]
        public TextField FromLogicDateNameTxt;

        [FindsBy(How = How.Name, Using = ToLogicDateName)]
        public TextField ToLogicDateNameTxt;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.LogicRequests.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public LogicRequestsPageObjects()
            : base(MedicalPreAuthPageUrlEnum.LogicRequests.GetStringValue())
        {

        }

        #endregion
    }
}
