using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations.PCI
{
    public class ClosedPageObjects : DefaultPageObjects
    {
        #region PRIVATE PROPERTIES

        private const string SearchId = "searchImageButton";
        private const string ReceivedDateFromCalendarXPath = ".//a[contains(@href, 'javascript:cal1.popup();')]";
        private const string ReceivedDateToCalendarXPath = ".//a[contains(@href, 'javascript:cal2.popup();')]";
        private const string ReceiveFromId = "receiveFromTextBox";
        private const string ReceiveToId = "receiveToTextBox";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = SearchId)]
        public ImageButton SearchButton;

        [FindsBy(How = How.XPath, Using = ReceivedDateFromCalendarXPath)] 
        public Link ReceivedDateFromCalendarLink;

        [FindsBy(How = How.XPath, Using = ReceivedDateToCalendarXPath)] 
        public Link ReceivedDateToCalendarLink;

        [FindsBy(How = How.Id, Using = ReceiveFromId)] 
        public TextField ReceivedFromTxt;

        [FindsBy(How = How.Id, Using = ReceiveToId)] 
        public TextField ReceivedToTxt;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.Closed.GetStringValue(), StartLegacy.PreAuthorizationProduct); }
        }

        #endregion

        #region CONSTRUCTOR

        public ClosedPageObjects()
            : base(string.Format(PageUrlEnum.Closed.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}
