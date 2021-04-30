
using System;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class PatientHistoryPage : BasePageService
    {
        private readonly PatientHistoryPageObjects _patientHistoryPage;

        public PatientHistoryPage(INavigator navigator, PatientHistoryPageObjects patientHistoryPage)
            : base(navigator, patientHistoryPage)
        {
            _patientHistoryPage = (PatientHistoryPageObjects)PageObject;
        }

        /// <summary>
        /// Get duration label
        /// </summary>
        /// <returns></returns>
        public string GetDurationLabel()
        {
            return _patientHistoryPage.PatientHistoryPageLabel.Text;
        }

        /// <summary>
        /// Get patient history window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public PatientHistoryPage GetWindowHandle(out string handle)
        {
           return GetCurrentWindowHandle<PatientHistoryPage>(_patientHistoryPage,out handle);
        }

        /// <summary>
        /// Click close button of patient history page
        /// </summary>
        /// <returns></returns>
        public ClaimSummaryPage ClickCloseButton()
        {
            var claimSummary = Navigator.Navigate<ClaimSummaryPageObjects>(()=>
                                                {
                                                    _patientHistoryPage.PatientHistoryCloseButton.Click();
                                                    Console.WriteLine("Clicked Close Button of Patient History Page.");
                                                    SiteDriver.SwitchWindowByTitle(string.Format(PageTitleEnum.ClaimSummary.GetStringValue(),ProductTitle));
                                                });
            SiteDriver.CloseFrameAndSwitchTo("View");
            return new ClaimSummaryPage(Navigator, claimSummary);
        }

    }
}
