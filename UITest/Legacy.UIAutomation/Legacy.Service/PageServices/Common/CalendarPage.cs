using Legacy.Service.PageObjects.Common;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using Pre_AuthorizationPageObjects = Legacy.Service.PageObjects.Pre_Authorizations;
using System;
using Legacy.Service.PageObjects.Default;

namespace Legacy.Service.PageServices.Common
{
    public class CalendarPage : BasePageService
    {
        #region PRIVATE/PUBLIC FIELDS

        private CalendarPageObjects _calendarPage;

        #endregion

        #region CONSTRUCTOR

        public CalendarPage(INavigator navigator, CalendarPageObjects calendarPage)
            : base(navigator, calendarPage)
        {
            _calendarPage = (CalendarPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get calendar window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public CalendarPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<CalendarPage>(_calendarPage,out handle);
        }

        /// <summary>
        /// Select a date.
        /// </summary>
        /// <param name="originalWindowHandle"></param>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public T SelectADate<T>(string originalWindowHandle, DateTime inputDate)
        {
            var target = typeof(T);
            if (typeof(Pre_Authorizations.PCI.ClosedPage).Equals(target))
                PageObject = Navigator.Navigate<Pre_AuthorizationPageObjects.PCI.ClosedPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(ModifiedEditsPage).Equals(target))
                PageObject = Navigator.Navigate<ModifiedEditsPageObjects>(()=> ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(LogicRequestsPage).Equals(target))
                PageObject = Navigator.Navigate<LogicRequestsPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(Pre_Authorizations.LogicRequestsPage).Equals(target))
                PageObject = Navigator.Navigate<Pre_AuthorizationPageObjects.LogicRequestsPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(SearchProductPage).Equals(target))
                PageObject = Navigator.Navigate<SearchProductPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(SearchUnreviewedPage).Equals(target))
                PageObject = Navigator.Navigate<SearchUnreviewedPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            if (typeof(SearchPendedPage).Equals(target))
                PageObject = Navigator.Navigate<SearchPendedPageObjects>(() => ClickOnADateAndSwitchToOriginalHandle(inputDate, originalWindowHandle));

            return (T)Activator.CreateInstance(target, Navigator, PageObject);
        }

        /// <summary>
        /// Close calendar popup and switch to original window handle.
        /// </summary>
        /// <param name="originalHandle">Name of original handle</param>
        /// <returns></returns>
        public T CloseCalendarPopup<T>(string originalHandle)
        {
            var target = typeof(T);
            if (typeof(ModifiedEditsPage).Equals(target))
                PageObject = Navigator.Navigate<ModifiedEditsPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(LogicRequestsPage).Equals(target))
              PageObject =   Navigator.Navigate<LogicRequestsPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(Pre_Authorizations.LogicRequestsPage).Equals(target))
                PageObject = Navigator.Navigate<Pre_AuthorizationPageObjects.LogicRequestsPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(Pre_Authorizations.PCI.ClosedPage).Equals(target))
                PageObject = Navigator.Navigate<Pre_AuthorizationPageObjects.PCI.ClosedPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(SearchProductPage).Equals(target))
                PageObject = Navigator.Navigate<SearchProductPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(SearchUnreviewedPage).Equals(target))
                PageObject = Navigator.Navigate<SearchUnreviewedPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            if (typeof(SearchPendedPage).Equals(target))
                PageObject = Navigator.Navigate<SearchPendedPageObjects>(() => CloseCalendarAndSwitchTo(originalHandle));

            return (T)Activator.CreateInstance(typeof(T),Navigator,PageObject);
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Click on a date and switch to original handle
        /// </summary>
        /// <param name="inputDate"></param>
        /// <param name="windowHandle"></param>
        private void ClickOnADateAndSwitchToOriginalHandle(DateTime inputDate, string windowHandle)
        {
            SiteDriver.WaitToLoad(3000);
            MoveToExpectedDate(inputDate);
            SiteDriver.FindElement<Link>(string.Format(CalendarPageObjects.DayPickerXPathTemplate, inputDate.Day), How.XPath).Click();
            SiteDriver.WaitToLoad(5000);
            SiteDriver.SwitchWindow(windowHandle);
        }

        /// <summary>
        /// Click on Previous/Next button to reach expected date.
        /// </summary>
        /// <param name="inputDate"></param>
        private void MoveToExpectedDate(DateTime inputDate)
        {
            DateTime takenDate = DateTime.Parse(SiteDriver.FindElement<TextLabel>(CalendarPageObjects.MontYearXPath, How.XPath).Text.Trim());
            int result = ((inputDate.Year - takenDate.Year) * 12) + inputDate.Month - takenDate.Month;
            if (result == 0) return;
            for (int i = 0; i < Math.Abs(result); i++)
            {
                ImageButton prevOrNextBtn = (result > 0) ? _calendarPage.NextMonthBtn : _calendarPage.PreviousMonthBtn;
                ClickOnPrevOrNextButton(prevOrNextBtn);
            }

        }

        /// <summary>
        /// Click Previous/Next button.
        /// </summary>
        /// <param name="prevOrNextBtn"></param>
        /// <returns></returns>
        private CalendarPage ClickOnPrevOrNextButton(ImageButton prevOrNextBtn)
        {
            _calendarPage = Navigator.Navigate<CalendarPageObjects>(() =>
                                                                                {
                                                                                    prevOrNextBtn.Click();
                                                                                    SiteDriver.SwitchWindow("Calendar");
                                                                                });
            return new CalendarPage(Navigator, _calendarPage);
        }

        private void CloseCalendarAndSwitchTo(string originalWindowHandle)
        {
            ClosePopupAndSwitchToOriginalHandle("Calendar", originalWindowHandle);
        }


        #endregion
    }
}
