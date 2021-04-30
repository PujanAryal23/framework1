using System;
using Legacy.Service.PageObjects.Common;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Pre_Authorizations
{
    public class LogicRequestsPage : DefaultPage
    {
         #region PRIVATE FIELDS

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
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _logicRequestsPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        public NotifyClientPage NavigateToNotifyClient()
        {
            var notifyClient =
                Navigator.Navigate(() =>
                {
                    _logicRequestsPage.NotifyClientBtn.Click();
                    Console.Out.WriteLine("Click on Notify Client Button.");
                    SiteDriver.WaitForCondition(
                        () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.NotifyClient.GetStringValue()));
                }, () => new NotifyClientPageObjects(MedicalPreAuthPageUrlEnum.NotifyClient.GetStringValue()));
            return new NotifyClientPage(Navigator, notifyClient);
        }



        /// <summary>
        /// Get client recv date
        /// </summary>
        /// <param name="clientRecvDate"></param>
        /// <param name="iSFrom"></param>
        /// <returns></returns>
        public LogicRequestsPage GetClienRecvDate(out DateTime clientRecvDate, bool iSFrom = true)
        {
            TextField clientRecvDateTxt = iSFrom
                                              ? _logicRequestsPage.FromClientRecvdDateTxt
                                              : _logicRequestsPage.ToClientRecvdDateTxt;
            return GetDate<LogicRequestsPage>(_logicRequestsPage, clientRecvDateTxt, out clientRecvDate);
        }

        /// <summary>
        /// Get logic date
        /// </summary>
        /// <param name="logicDate"></param>
        /// <param name="iSFrom"></param>
        /// <returns></returns>
        public LogicRequestsPage GetLogicDate(out DateTime logicDate, bool iSFrom = true)
        {
            TextField logicDateTxt = iSFrom ? _logicRequestsPage.FromLogicDateNameTxt : _logicRequestsPage.ToLogicDateNameTxt;
            return GetDate<LogicRequestsPage>(_logicRequestsPage, logicDateTxt, out logicDate);
        }

        #endregion
    }
}
