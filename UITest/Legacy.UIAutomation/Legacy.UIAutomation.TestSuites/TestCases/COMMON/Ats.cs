using Legacy.Service.PageServices.ATS;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.COMMON
{
    public class Ats : AutomatedBase
    {
        private AtsPage _atsFlow;
        private DefaultPage _lastFlow;
        

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
            //_atsFlow = LoginFlow.Login().GoToAts();
        }

        [SetUp]
        public void StartUp()
        {
            _lastFlow = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (_lastFlow != null)
            {
                // Handle GoBack for pages that do a postback to themselves
                var pageAction = _lastFlow.GoBack();
                while (!(pageAction is AtsPage))
                {
                    pageAction = _lastFlow.GoBack();
                }
                _atsFlow = (AtsPage)pageAction;
            }
        }
    }
}
