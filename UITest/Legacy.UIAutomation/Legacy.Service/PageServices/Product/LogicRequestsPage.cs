using System;
using Legacy.Service.PageObjects.Common;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class LogicRequestsPage : DefaultPage
    {
        #region PRIVATE/PUBILC FIELDS

        private readonly LogicRequestsPageObjects _logicRequestsPage;

        #endregion

        #region CONSTRUCTOR

        public LogicRequestsPage(INavigator navigator, LogicRequestsPageObjects logicRequestsPage)
            : base(navigator, logicRequestsPage)
        {
            _logicRequestsPage = (LogicRequestsPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get logic request window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public LogicRequestsPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<LogicRequestsPage>(_logicRequestsPage, out handle);
        }

        public override IPageService GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new DentalClaimInsightPage(Navigator, new DentalClaimInsightPageObjects());
        }

        public NotifyClientPage NavigateToNotifyClient()
        {
            var notifyClient =
                Navigator.Navigate(() =>
                                                                {
                                                                    _logicRequestsPage.NotifyClientBtn.Click();
                                                                    Console.Out.WriteLine("Click on Notify Client Button.");
                                                                    SiteDriver.WaitForIe(2000);
                                                                    SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.NotifyClient.GetStringValue()));
                                                                }, () => new NotifyClientPageObjects(ProductPageUrlEnum.NotifyClient.GetStringValue()));
            return new NotifyClientPage(Navigator, notifyClient);
        }

        public void ClickSearchButon()
        {
            _logicRequestsPage.SearchBtn.Click();
            Console.Out.WriteLine("Click Search button.");
        }

        public void ClickClearButon()
        {
            _logicRequestsPage.ClearBtn.Click();
            Console.Out.WriteLine("Click Clear button.");
        }

        public void ClickOnClientReceiveDate(bool isBeginDate = true)
        {
            var clientReceiveDate = isBeginDate ? _logicRequestsPage.ClientReceiveDateBeginCalLnk : _logicRequestsPage.ClientReceiveDateToCalLnk;
            ClickToOpenACalendar(clientReceiveDate);
        }

        public void ClickOnLogicDate(bool isBeginDate = true)
        {
            var logicDate = isBeginDate ? _logicRequestsPage.LogicDateBeginCalLnk : _logicRequestsPage.LogicDateToCalLnk;
             ClickToOpenACalendar(logicDate);
        }

        public LogicRequestsPage GetClientReceiveDate(out DateTime clientReceiveDate, bool isBeginField = true)
        {
            TextField clientDateTxt = isBeginField ? _logicRequestsPage.ClientBeginDateTxt : _logicRequestsPage.ClientEndDateTxt;
            return GetDate<LogicRequestsPage>(_logicRequestsPage, clientDateTxt, out clientReceiveDate);
        }

        public LogicRequestsPage GetLogicDate(out DateTime logicDate, bool isBeginField = true)
        {
            TextField logicDateTxt = isBeginField ? _logicRequestsPage.LogicBeginDateTxt : _logicRequestsPage.LogicEndDateTxt;
            return GetDate<LogicRequestsPage>(_logicRequestsPage, logicDateTxt, out logicDate);
        }
        
        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
