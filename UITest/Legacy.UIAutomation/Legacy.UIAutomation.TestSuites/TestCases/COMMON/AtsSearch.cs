using Legacy.Service.Data;
using Legacy.Service.PageServices.ATS;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.COMMON
{
    public class AtsSearch : AutomatedBase
    {
        private AtsSearchPage _atsSearchFlow;
        private BasePageService _lastFlow;

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
          //_atsSearchFlow = LoginFlow.Login().GoToAts().GoToAtsSearch();
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
            _atsSearchFlow = (AtsSearchPage)_lastFlow.GoBack();
        }

        //[Test]
        public void SearchFor_RPET_StatusOpen()
        {
          var data = DataHelper.GetTestData("Legacy.UIAutomation.TestSuites.TestCases.AtsSearchTest", "SearchFor_RPET_StatusOpen");
          _atsSearchFlow.SelectClient(data["Client"]);
          _atsSearchFlow.SelectAppealStatus(data["Status"]);
          _lastFlow = _atsSearchFlow.StartSearch();

          var actualData = (_lastFlow as AtsSearchPage).GetResultDataAt(0);
          Assert.AreEqual(data["Client"], actualData[3]);
          Assert.AreEqual(data["Status"], actualData[8]);
        }
    }
}
