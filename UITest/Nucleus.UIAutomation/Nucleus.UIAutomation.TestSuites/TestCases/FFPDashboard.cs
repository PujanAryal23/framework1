using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class FFPDashboard : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private String day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;

        #endregion

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
                try
                {
                    _isDashboardOpened = _dashboard.IsDashboardPageOpened();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                }

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
                _dashboard = CurrentPage.Logout()
                        .LoginAsHciAdminUser().NavigateToFFPDashboard();
            }

            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.Dashboard.GetStringValue()))
            {
                CurrentPage =
                    _dashboard = QuickLaunch.NavigateToDashboard();
            }
            _dashboard.CloseConnection();
        }

        protected override void ClassCleanUp()
        {
            try
            {

            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        #region TEST SUITES

        [Test]//US65701 + TE-595

        public void Verify_Claims_Overview_section_is_present_with_FFP_in_right_corner_and_last_updated_time_in_lower_right_corner()
        {
            _dashboard.IsContainerHeaderClaimsOverviewPresent().ShouldBeTrue("Container header Claims Overview present");
            _dashboard.IsContainerHeaderClaimsOverviewFFPPresent().ShouldBeTrue("Container header has FFP in right corner");
            //_dashboard.IsContainerHeaderContainsRefreshIcon().ShouldBeTrue("Container header has refresh icon");
            _dashboard.IsNextRefreshTimePresent().ShouldBeTrue("Last updated time appears in upper right corner of the page");
            //_dashboard.ClickOnRefreshIconAndWait();
            //_dashboard.GetLastUpdatedTimeInLowerRightCorner().ShouldBeEqual($"Next Refresh : {_dashboard.GetNextRefreshForFFPDashboard(EnvironmentManager.Username)}", "Next Refresh");
        }

        [Test]
        public void Verify_data_points_in_FFP_Claims_Overview_widget_for_internal_user()
        {
            //_dashboard.ClickOnRefreshIconAndWait();
            var expectedValues = _dashboard.GetExpectedFFPClaimsCount(EnvironmentManager.HciAdminUsername);
            var expectedFfpUnreveiwedClaimCount = expectedValues[0];
            var expectedFfpPendedClaimCount = expectedValues[1];
            var expectedFfpUnreleasedClaimCount = expectedValues[2];
            var expectedFfpAllPendedClaimCount = expectedValues[3];
            var expectedFfpReviewedYesterdayClaimCount = expectedValues[4];
            var expectedFfpReleasedYetserdayClaimCount = expectedValues[5];


            StringFormatter.PrintMessageTitle("Verify Unreviewed Claim Count");
            _dashboard.GetTotalFFPUnreveiwedClaimsCount().ShouldBeEqual(expectedFfpUnreveiwedClaimCount.ToString(),
                "Total FFP Unreviewed Claims count should equal to database value.");

            StringFormatter.PrintMessageTitle("Verify Unreleased Claim Count");
            _dashboard.GetTotalFFPUnreleasedClaimsCount().ShouldBeEqual(expectedFfpUnreleasedClaimCount.ToString(),
                "Total FFP Unreleased Claims count should equal to database value.");

            StringFormatter.PrintMessageTitle("Verify Pended Claim Count");
            _dashboard.GetTotalFFPPendedClaimsCount().ShouldBeEqual(expectedFfpPendedClaimCount.ToString(),
                "Total FFP Pended Claims count should equal to database value.");

            StringFormatter.PrintMessageTitle("Verify All pended Claim Count");
            _dashboard.GetAllPendedClaimsCount().ShouldBeEqual(expectedFfpAllPendedClaimCount.ToString(),
                "Total FFP All pended Claims count should equal to database value.");

            StringFormatter.PrintMessageTitle("Verify Reviewed Yesteday Claim Count");
            _dashboard.GetReviewedYesterdayClaimsCount().ShouldBeEqual(expectedFfpReviewedYesterdayClaimCount.ToString(),
                "Total FFP Reviewed Yesterday Claims count should equal to database value.");

            StringFormatter.PrintMessageTitle("Verify Released Yesterday Claim Count");
            _dashboard.GetReleasedYesterdayClaimsCount().ShouldBeEqual(expectedFfpReleasedYetserdayClaimCount.ToString(),
                "Total FFP Released Yesterday Claims count should equal to database value.");

            _claimsDetailPage = _dashboard.ClickOnFFPClaimsDetailExpandIcon();
            List<int> claimdetailHeaders = new List<int>();

            for (var i = 1; i <= 3; i++)
            {
                claimdetailHeaders.Add(_claimsDetailPage.GetHeaderValueForFirstRow(i));
            }


            _claimsDetailPage.ClickOnDashboardIcon();
            _dashboard.WaitForWorking();
            List<int> widgetClaimHeaders = new List<int>();
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPUnreveiwedClaimsCount()));
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPPendedClaimsCount()));
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPUnreleasedClaimsCount()));

            for (int i = 0; i <= 2; i++)
            {
                claimdetailHeaders[i].ShouldBeEqual(widgetClaimHeaders[i]);
            }

        }

        [Test, Category("SmokeTestDeployment"), Category("ExcludeDailyTest")]
        public void Verify_data_points_in_FFP_Claims_Overview_widget_for_internal_user_SmokeTest()
        {

            StringFormatter.PrintMessageTitle("Verify Unreviewed Claim Count");
            Convert.ToInt32(_dashboard.GetTotalFFPUnreveiwedClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP Unreviewed Claims count should not empty.");

            StringFormatter.PrintMessageTitle("Verify Unreleased Claim Count");
            Convert.ToInt32(_dashboard.GetTotalFFPUnreleasedClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP Unreleased Claims count should not empty.");

            StringFormatter.PrintMessageTitle("Verify Pended Claim Count");
            Convert.ToInt32(_dashboard.GetTotalFFPPendedClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP Pended Claims count should not empty.");

            StringFormatter.PrintMessageTitle("Verify All pended Claim Count");
            Convert.ToInt32(_dashboard.GetAllPendedClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP All pended Claims count should  not empty.");

            StringFormatter.PrintMessageTitle("Verify Reviewed Yesteday Claim Count");
            Convert.ToInt32(_dashboard.GetReviewedYesterdayClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP Reviewed Yesterday Claims count should not empty.");

            StringFormatter.PrintMessageTitle("Verify Released Yesterday Claim Count");
            Convert.ToInt32(_dashboard.GetReleasedYesterdayClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total FFP Released Yesterday Claims count should not empty.");

            _claimsDetailPage = _dashboard.ClickOnFFPClaimsDetailExpandIcon();
            List<int> claimdetailHeaders = new List<int>();

            for (var i = 1; i <= 3; i++)
            {
                claimdetailHeaders.Add(_claimsDetailPage.GetHeaderValueForFirstRow(i));
            }


            _claimsDetailPage.ClickOnDashboardIcon();
            _dashboard.WaitForWorking();
            List<int> widgetClaimHeaders = new List<int>();
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPUnreveiwedClaimsCount()));
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPPendedClaimsCount()));
            widgetClaimHeaders.Add(Convert.ToInt32(_dashboard.GetTotalFFPUnreleasedClaimsCount()));

            for (int i = 0; i <= 2; i++)
            {
                claimdetailHeaders[i].ShouldBeEqual(widgetClaimHeaders[i]);
            }

        }


        [Test]//US65741
        public void Verify_Unapproved_Claims_widget_is_displayed_below_claims_overview_widget()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
            _dashboard.GetUnapprovedClaimsDivHeaderText().ShouldBeEqual("Unapproved Claims Overview", "Div header");
            _dashboard.GetUnapprovedClaimsProductText().ShouldBeEqual("FFP", "Product label in div");

            _dashboard.GetUnapprovedClaimsClientCodeList().ShouldCollectionBeEqual(_dashboard.GetExpectedUnapprovedClaimsClientCodeList(EnvironmentManager.HciAdminUsername),
                "Client Code List should equal to list from Database and should be displayed in ascending order.");
            _dashboard.GetUnapprovedClaimsList().ShouldCollectionBeEqual(_dashboard.GetExpectedUnapprovedClaimsList(EnvironmentManager.HciAdminUsername),
                "Claim Sequence List should equal to list from Database and should be displayed in ascending order.");
            _dashboard.GetTotalUnapprovedClaimsCount().ShouldBeEqual(_dashboard.GetExpectedFFPUnapprovedClaimsCount(EnvironmentManager.HciAdminUsername),
                "Total Unapproved Claims should be equal to database value.");
            var totalUnapprovedClaimsDisplayed = _dashboard.GetTotalDisplayedUnapprovedClaimsCount();
            var isLoadMorePresent = _dashboard.IsLoadMorePresent();
            if (isLoadMorePresent)
            {
                _dashboard.ClickOnLoadMoreFFPUnapprovedClaims();
                _dashboard.GetTotalDisplayedUnapprovedClaimsCount().ShouldBeGreater(totalUnapprovedClaimsDisplayed,
                    "Clicking on Load More must display 50 more or all data.");
            }
            else
                _dashboard.GetTotalDisplayedUnapprovedClaimsCount().ShouldBeEqual(totalUnapprovedClaimsDisplayed,
                    "Is Total Unapproved Claims Count Equal?");

            _dashboard.IsExpandIconPresentInUnapprovedClaimsDiv().ShouldBeFalse("Expand Icon should not be Present.");
            // _dashboard.ClickOnUnapprovedClaimsRefreshIcon();
            // _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
        }

        [Test, Category("SmokeTestDeployment"), Category("ExcludeDailyTest")]//US65741
        public void Verify_Unapproved_Claims_widget_is_displayed_below_claims_overview_widget_SmokeTest()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
            _dashboard.GetUnapprovedClaimsDivHeaderText().ShouldBeEqual("Unapproved Claims Overview", "Div header");
            _dashboard.GetUnapprovedClaimsProductText().ShouldBeEqual("FFP", "Product label in div");

            _dashboard.GetUnapprovedClaimsClientCodeList().ShouldCollectionBeNotEmpty(
                "Client Code List should not empty.");
            _dashboard.GetUnapprovedClaimsList().ShouldCollectionBeNotEmpty(
                "Claim Sequence List should not empty.");
            Convert.ToInt32(_dashboard.GetTotalUnapprovedClaimsCount()).ShouldBeGreaterOrEqual(0,
                "Total Unapproved Claims should not empty.");
            var totalUnapprovedClaimsDisplayed = _dashboard.GetTotalDisplayedUnapprovedClaimsCount();
            var isLoadMorePresent = _dashboard.IsLoadMorePresent();
            if (isLoadMorePresent)
            {
                _dashboard.ClickOnLoadMoreFFPUnapprovedClaims();
                _dashboard.GetTotalDisplayedUnapprovedClaimsCount().ShouldBeGreater(totalUnapprovedClaimsDisplayed,
                    "Clicking on Load More must display 50 more or all data.");
            }
            else
                _dashboard.GetTotalDisplayedUnapprovedClaimsCount().ShouldBeEqual(totalUnapprovedClaimsDisplayed,
                    "Is Total Unapproved Claims Count Equal?");

            _dashboard.IsExpandIconPresentInUnapprovedClaimsDiv().ShouldBeFalse("Expand Icon should not be Present.");
            // _dashboard.ClickOnUnapprovedClaimsRefreshIcon();
            // _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
        }
        #endregion

        #region private methods


        #endregion
    }
}
