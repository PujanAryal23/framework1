using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Reports;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Reports
{
    public class ReportsAndEventsPage: DefaultPage
    {

        private readonly ReportsAndEventsPageObjects _reportsAndEvents ;

        public ReportsAndEventsPage(INavigator navigator, ReportsAndEventsPageObjects changePasswordPage)
            : base(navigator, changePasswordPage)
        {
            // Just for performance!
            _reportsAndEvents = (ReportsAndEventsPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _reportsAndEvents.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}
