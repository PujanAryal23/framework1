using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class FFPDashboardClaimsDetail : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private readonly String _day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _expectedClaimByClientList;


        #endregion

        #region OVERRIDE METHODS

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _dashboard = QuickLaunch.NavigateToFFPDashboard();
                _claimsDetailPage = _dashboard.ClickOnFFPClaimsDetailExpandIcon();
                AssignedExpectedList();

            }

            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                
                    QuickLaunch.Logout()
                        .LoginAsHciAdminUser()
                        .ClickOnSwitchClient()
                        .SwitchClientTo(EnvironmentManager.TestClient);
                _dashboard = QuickLaunch.NavigateToFFPDashboard();
                _claimsDetailPage = _dashboard.ClickOnFFPClaimsDetailExpandIcon();

            }
        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _claimsDetailPage.GetAllFfpCotivitiExpectedLists(EnvironmentManager.HciAdminUsername);
            _expectedClaimByClientList = _allCotivitiExpectedLists[0];
        }

        #endregion

        #region TEST SUITES

      
        //US65700 contains all story
        [Test]
        public void Verify_header_and_column_headers_in_ffp_dashboard_claims_detail_and_verify_client_values_sum_to_top_row_total_value
        ()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedFirstRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "1st_row_column_headers").Values.ToList();
            IList<string> expectedSecondRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "2nd_row_column_headers").Values.ToList();
            _claimsDetailPage.GetClaimsDetailHeader().ShouldBeEqual("Claims by Client", "Claims Detail Header");
            _claimsDetailPage.GetFirstRowColumnHeaders()
            .ShouldCollectionBeEqual(expectedFirstRowColumnHeadersList, "1st row column headers are equal:");
            _claimsDetailPage.GetSecondRowColumnHeaders()
            .ShouldCollectionBeEqual(expectedSecondRowColumnHeadersList, "2nd row column headers are equal:");
            _claimsDetailPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Does Last Updated Time Appears In Upper Right Corner ?");
            
            StringFormatter.PrintMessageTitle("Verifying client values sum to top row total values");
            for (var i = 1; i <= _claimsDetailPage.GetFirstRowColumnHeaderCount(); i++)
                _claimsDetailPage.GetHeaderValue(i).ShouldBeEqual(_claimsDetailPage.GetEachColumnSumValue(i, _claimsDetailPage.GetClaimsDetailHeader()), "Total sum equals to top value in column " + i);
        }

        [Test]
        public void Verify_data_points_of_FFP_Claims_Details_for_Internal_user()
        {
            string widget = _claimsDetailPage.GetClaimsDetailHeader();
            for (var i = 1; i <= _expectedClaimByClientList.Count; i++)
            {
                var clientCode = _expectedClaimByClientList[i - 1];
                StringFormatter.PrintMessageTitle("Verify data points for " + clientCode + " Client");
                _claimsDetailPage.GetEachGridValue(i, 1, widget)
                    .ShouldBeEqual(_claimsDetailPage.GetExpectedUnreviewedFFPClaimsCountByClient(clientCode),
                        "Unreviewed Claims Count should equal to database value ." + clientCode);
                _claimsDetailPage.GetEachGridValue(i, 2, widget)
                    .ShouldBeEqual(_claimsDetailPage.GetExpectedPendedFFPClaimsCountByClient(clientCode),
                        "Pended Claim Count should equal to database value for client - " + clientCode);
                _claimsDetailPage.GetEachGridValue(i, 3, widget)
                    .ShouldBeEqual(_claimsDetailPage.GetExpectedUnreleasedFFPClaimsCountByClient(clientCode),
                        "Unreleased Claim Count should equal to database value for client  -" + clientCode);
            }

        }

        [Test]
        public void Verify_active_FFP_clients_appear_and_inactive_clients_do_not_appear_in_list_and_no_column_has_null_value_in_Claims_by_Client()
        {
            StringFormatter.PrintMessageTitle("Verifying active clients appear and inactive clients donot appear in the list.");
            string widget = _claimsDetailPage.GetClaimsDetailHeader();
            _claimsDetailPage.GetActiveClientList(widget).ShouldCollectionBeEqual(_expectedClaimByClientList, "Claims by Active Client List and should be sorted");

            StringFormatter.PrintMessageTitle("Verifying values in column");
            _claimsDetailPage.GetAllColumnsValues(widget).ShouldBeTrue("All Column value should not null");


        }
        #endregion
    }

}
