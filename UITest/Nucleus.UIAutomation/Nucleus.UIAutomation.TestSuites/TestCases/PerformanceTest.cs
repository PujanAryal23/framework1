using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class PerformanceTest : NewAutomatedBase
    {

        #region PRIVATE FIELDS

        private LoginPage _login;
        #endregion

        #region OVERRIDE METHODS
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region TEST SUITES

        [Test, Category("PerformanceTest")]
        public void Test_page_load_time_for_login()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //QuickLaunch = _login.LoginAsHciAdminUser();
            var pageLoad = SiteDriver.ReturnPageLoadTime();
            StringFormatter.PrintMessage(pageLoad + " time taken for loading the page completly");
            Dictionary<string, object> webTimings = SiteDriver.WebTimings("loginPage", 1);
            var connectionTime = (Convert.ToInt64(webTimings["loadEventStart"]) - Convert.ToInt64(webTimings["navigationStart"])) / 1000;
            StringFormatter.PrintMessage(" Time taken from page navigation to page loading is :" + connectionTime);
            StringFormatter.PrintMessage("Login Successful !");
            _login = QuickLaunch.Logout();
            QuickLaunch = _login.LoginAsHciAdminUser();
            StringFormatter.PrintMessage(" Calculating time taken for loading the page completly after page caching");
            SiteDriver.WebTimings("loginPage", 2);
            QuickLaunch.Logout();
        }



        #endregion
    }
}
