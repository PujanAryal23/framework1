using System;
using Legacy.Service.PageServices.Login;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.COMMON
{
    public class Login : AutomatedBase
    {
        private WelcomePage _welcomePage;
        private LoginPage _loginPage;

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override ProductEnum TestProduct
        {
            get
            {
                return ProductEnum.COMMON;
            }

        }



        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
        }



        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!BasePage.Equals(typeof(LoginPage)))
            {
                BasePage = _loginPage = _welcomePage.Logout();
            }
        }

        [Test]
        public void VerifyValidUserCanLogin()
        {
            BasePage = _welcomePage = LoginPage.Login();
            _welcomePage.CurrentPageTitle.ShouldEqual(PageTitleEnum.WelcomePage.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void VerifyValidateUnSupportedMessage()
        {
            _loginPage = LoginPage;
            if (_loginPage.IsCurrentBrowserVersionIE9())
            {
                _loginPage.GetUnSupportedBrowserMessage()
                    .ShouldEqual(
                        "Verisk Health supports Chrome and the most current version of Internet Explorer, plus the last two versions (Internet Explorer 10 and above). Updating or changing your browser is not required, but is strongly recommended to improve your user experience. If you have any questions please contact Client Services at (801)285-5910.",
                        "UnSupported Browser Message");
            }
            BasePage = _welcomePage = LoginPage.Login();
        }

        


    }
}
