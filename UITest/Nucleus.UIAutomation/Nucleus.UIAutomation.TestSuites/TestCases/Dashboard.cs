using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class Dashboard : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private String day = DateTime.Now.ToString("ddd");
        private ProfileManagerPage _profileManager;
        private QuickLaunchPage _quickLaunch;
        #endregion

        #region OVERRIDE METHODS

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _dashboard = QuickLaunch.NavigateToDashboard();
                try
                {
                    _isDashboardOpened = _dashboard.IsDashboardPageOpened();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                }

            }
            catch (Exception ex)
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
                _dashboard.Logout().LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient).NavigateToDashboard();
            }
        }

        protected override void ClassCleanUp()
        {
            try
            {
                _dashboard.CloseConnection();

            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] // CAR-3204 [CAR-3243]
        [Author("Shyam Bhattarai")]
        public void Verify_appeal_overdue_icon_should_not_be_shown_when_no_appeals_are_overdue()
        {
            _dashboard = _dashboard.Logout().LoginAsPciTest5User().NavigateToDashboard();
            _dashboard.IsAppealOverdueIconPresent().ShouldBeFalse("Appeal Overdue icon should not be present when none of the appeals are over due");
        }

        [Test, Category("SmokeTest")]//US28489 + TE-595
        public void Verify_Claims_Overview_section_is_present_with_PCI_in_right_corner_and_last_updated_time_in_lower_right_corner()
        {

            _dashboard.IsContainerHeaderClaimsOverviewPresent().ShouldBeTrue("Container header Claims Overview present");
            _dashboard.IsContainerHeaderClaimsOverviewPCIPresent().ShouldBeTrue("Container header has CV in right corner");
            _dashboard.IsNextRefreshTimePresent().ShouldBeTrue("Last updated time appears in upper right corner");
            //_dashboard.ClickOnRefreshIconAndWait();
            //_dashboard.GetLastUpdatedTimeInLowerRightCorner().ShouldBeEqual($"Next Refresh : {_dashboard.GetNextRefreshForPCIDashboard(EnvironmentManager.Username)}", "Next Refresh");
        }

        [Test, Category("SmokeTestDeployment")]//US31608
        public void Verify_PCI_Logic_Request_Overview_container_and_its_highcharts()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string testedBrowser = EnvironmentManager.Browser;
            _dashboard.IsLogicRequestOverviewDivPresent().ShouldBeTrue("Logic Request Overview section present?");
            _dashboard.GetLogicRequestOverviewDivHeaderText().ShouldBeEqual("Logic Request Overview", "Header Text");
            _dashboard.IsLogicRequestOverdueGraphPresent().ShouldBeTrue("Logic Request Overdue chart present?");
            _dashboard.IsLogicRequestDueIn30MinutesGraphPresent().ShouldBeTrue("Logic Request Due In 30 Mins chart present?");
            _dashboard.IsLogicRequestCurrentlyOpenGraphPresent().ShouldBeTrue("Logic Request Currently Open chart present?");

            var ieRange = "050";
            var chromeRange = "0\r\n50";
            var openvalue = Convert.ToInt32(_dashboard.GetLogicRequestCurrentlyOpenGraphValue());
            if (openvalue > 50 && openvalue <= 150)
            {
                ieRange = "0100";
                chromeRange = "0\r\n100";
            }
            else if (openvalue > 150 && openvalue <= 200)
            {
                ieRange = "0200";
                chromeRange = "0\r\n200";
            }
            else if (openvalue > 200 && openvalue <= 350)
            {
                ieRange = "0250";
                chromeRange = "0\r\n250";
            }

            _dashboard.GetLogicRequestOverdueGraphYAxisText()
                .Trim()
                .ShouldBeEqual(
                   testedBrowser.Equals("IE", StringComparison.InvariantCulture)
                        ? ieRange
                        : chromeRange, "Y-axis Range of Overdue graph");
            _dashboard.GetLogicRequestDueIn30MinsGraphYAxisText()
                .Trim()
                .ShouldBeEqual(
                   testedBrowser.Equals("IE", StringComparison.InvariantCulture)
                        ? ieRange
                        : chromeRange, "Y-axis Range of DueIn30Mins graph");
            _dashboard.GetLogicRequestCurrentlyOpenGraphYAxisText()
               .Trim()
               .ShouldBeEqual(
                  testedBrowser.Equals("IE", StringComparison.InvariantCulture)
                       ? ieRange
                       : chromeRange, "Y-axis Range of CurrentlyOpen graph");

            _dashboard.GetLogicRequestOverdueGraphYAxisLabel().ShouldBeEqual("Overdue", "1st graph label");
            _dashboard.GetLogicRequestDueIn30MinsGraphYAxisLabel().ShouldBeEqual("Due in 30 Minutes", "2nd  graph label");
            _dashboard.GetLogicRequestCurrentlyOpenGraphYAxisLabel().ShouldBeEqual("Currently Open", "3rd  graph label");
            Assert.GreaterOrEqual(Convert.ToInt32(_dashboard.GetLogicRequestOverdueGraphValue()), 0,
                "Logic Request Overdue graph value is greater or equal to zero");
            Assert.GreaterOrEqual(Convert.ToInt32(_dashboard.GetLogicRequestDueIn30MinsGraphValue()), 0,
               "Logic Request Due in 30 mins graph value is greater or equal to zero");
            Assert.GreaterOrEqual(Convert.ToInt32(_dashboard.GetLogicRequestCurrentlyOpenGraphValue()), 0,
               "Logic Request Currently Open graph value is greater or equal to zero");
        }


        [Test, Category("SmokeTest")]//US31632
        public void Verify_Unapproved_Claims_widget_is_displayed_below_claims_overview_widget()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
            _dashboard.GetUnapprovedClaimsDivHeaderText().ShouldBeEqual("Unapproved Claims Overview", "Div header");
            _dashboard.GetUnapprovedClaimsProductText().ShouldBeEqual("CV", "Product label in div");


            _dashboard.GetUnapprovedClaimsClientCodeList().ShouldCollectionBeEqual(_dashboard.GetPCIExpectedUnapprovedClaimsClientCodeList(EnvironmentManager.HciAdminUsername),
                "Client Code List should equal to list from Database and should be displayed in ascending order.");
            _dashboard.GetUnapprovedClaimsList().ShouldCollectionBeEqual(_dashboard.GetPCIExpectedUnapprovedClaimsList(EnvironmentManager.HciAdminUsername),
                "Claim Sequence List should equal to list from Database and should be displayed in ascending order.");
            _dashboard.GetTotalUnapprovedClaimsCount().ShouldBeEqual(_dashboard.GetPCIExpectedFFPUnapprovedClaimsCount(EnvironmentManager.HciAdminUsername),
                "Total Unapproved Claims should be equal to database value.");

            _dashboard.IsExpandIconPresentInUnapprovedClaimsDiv().ShouldBeFalse("Expand Icon Present?");
            //_dashboard.ClickOnUnapprovedClaimsRefreshIcon();
            //_dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
        }

        [Test, Category("SmokeTestDeployment"), Category("ExcludeDailyTest")]//US31632
        public void Verify_Unapproved_Claims_widget_is_displayed_below_claims_overview_widget_SmokeTest()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
            _dashboard.GetUnapprovedClaimsDivHeaderText().ShouldBeEqual("Unapproved Claims Overview", "Div header");
            _dashboard.GetUnapprovedClaimsProductText().ShouldBeEqual("CV", "Product label in div");


            _dashboard.GetUnapprovedClaimsClientCodeList().ShouldCollectionBeNotEmpty(
                "Client Code List should not empty.");
            _dashboard.GetUnapprovedClaimsList().ShouldCollectionBeNotEmpty(
                "Claim Sequence List should not empty");
            Convert.ToInt32(_dashboard.GetTotalUnapprovedClaimsCount())
                .ShouldBeGreaterOrEqual(0, "Total Unapproved Claims should be greater than zero.");

            _dashboard.IsExpandIconPresentInUnapprovedClaimsDiv().ShouldBeFalse("Expand Icon Present?");
            //_dashboard.ClickOnUnapprovedClaimsRefreshIcon();
            //_dashboard.IsUnapprovedClaimsWidgetPresent().ShouldBeTrue("Unapproved Claims Widget present?");
        }

        [Test]//US28489 + US63944
        public void Verify_the_data_labels_in_Claims_Overveiw_and_on_clicking_refresh_icon_data_is_loaded_again()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IList<string> expectedDataLablesInClaimsOverviewList = DataHelper.GetMappingData(FullyQualifiedClassName, "data_labels_in_claims_overview").Values.ToList();
            //expectedDataLablesInClaimsOverviewList.ShouldCollectionBeEqual(_dashboard.GetClaimsOverveiwDataLabels(), "Claims Overveiw data lables are equal:");
            Verify_datalabels_and_tooltip_for_Claims_Overview_widget();
            Verify_claimscount_in_the_widget_with_database(EnvironmentManager.HciAdminUsername);
            _dashboard.IsClaimsOverveiwDataWidgetPresent().ShouldBeTrue("Claims Overview data widget present");
            //_dashboard.ClickOnRefreshIconAndWait();
            //_dashboard.IsClaimsOverveiwDataWidgetPresent().ShouldBeTrue("Claims Overview data widget present");
        }

        [Test, Category("SmokeTestDeployment"), Category("ExcludeDailyTest")]//US28489 + US63944
        public void Verify_the_data_labels_in_Claims_Overveiw_and_on_clicking_refresh_icon_data_is_loaded_again_SmokeTest()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            Verify_datalabels_and_tooltip_for_Claims_Overview_widget();
            _dashboard.ClaimsOverviewGridViewClaimCount().ShouldCollectionBeNotEmpty(
                "Grid view Claim count values should not empty");
            _dashboard.GetRealTimeClaimsInWidget().ShouldCollectionBeNotEmpty(
                "Real time Claim count values should not empty");
            _dashboard.GetUnreviewedClaimsInWidget().ShouldCollectionBeNotEmpty(
                "Unreviewed Claim count values should not empty");
            _dashboard.IsClaimsOverveiwDataWidgetPresent().ShouldBeTrue("Claims Overview data widget present");
            //_dashboard.ClickOnRefreshIconAndWait();
            //_dashboard.IsClaimsOverveiwDataWidgetPresent().ShouldBeTrue("Claims Overview data widget present");
        }

        [Test, Category("SmokeTestDeployment")]//US28490+CAR-3124(CAR-3065)
        public void Verify_container_header_Appeals_Overview_is_present_with_PCI_in_right_corner_and_verify_the_data_labels()
        {
            _dashboard.GetAppealsOverviewDivHeader().ShouldBeEqual("Appeals Overview");
            _dashboard.IsContainerHeaderAppealsOverviewPCIPresent().ShouldBeTrue("Container header has CV in right corner");
            StringFormatter.PrintMessageTitle("Verifying Appeals Overview data labels");
            _dashboard.GetAppealsOverviewDataLabel(1).ShouldBeEqual("Total Appeals", "Total Appeals data label");
            _dashboard.GetAppealsOverviewDataLabel(2).ShouldBeEqual("Urgent Appeals", "Urgent Appeals data label");
            _dashboard.GetAppealsOverviewDataLabel(3).ShouldBeEqual("Record Reviews", "Record Reviews data label");
            _dashboard.GetAppealsOverviewDataLabel(4).ShouldBeEqual("Medical Record Reviews", "Medical Record Reviews data label");
            _dashboard.GetAppealsOverviewDataLabel(5).ShouldBeEqual("Restricted Appeals", "Restricted Appeals data label");
        }

        [Test] //CAR-3002(CAR-2939)+CAR-3124(CAR-3065)
        public void Verify_container_header_Appeals_Overview__data_values()
        {
            _dashboard.GetAppealsOverviewDataValuesByLabel("Total Appeals").
                ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Total Appeals", EnvironmentManager.Username), "Value Should Match");
            _dashboard.GetAppealsOverviewDataValuesByLabel("Urgent Appeals").
                ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Urgent Appeals", EnvironmentManager.Username), "Value Should Match");
            _dashboard.GetAppealsOverviewDataValuesByLabel("Record Reviews").
                ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Record Reviews", EnvironmentManager.Username), "Value Should Match");
            _dashboard.GetAppealsOverviewDataValuesByLabel("Medical Record Reviews").
                ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Medical Record Reviews", EnvironmentManager.Username), "Value Should Match");
            _dashboard.GetAppealsOverviewDataValuesByLabel("Restricted Appeals").
                ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Restricted Appeals", EnvironmentManager.Username), "Value Should Match");

        }

        [Test, Category("SmokeTestDeployment")]//US28490
        public void Verify_column_header_and_contents_for_data_label_Total_Appeals_by_Day_for_the_Next_5_days_in_Appeals_Overview()
        {
            StringFormatter.PrintMessageTitle("Verifying cloumn header");
            var dates = new List<DateTime>();
            var holidayList = _dashboard.GetHolidays();
            var dt = DateTime.Now.AddDays(1);
            while (true)
            {
                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (holidayList != null && !holidayList.Contains(dt.ToString("MM/dd/yyyy")))
                        dates.Add(dt);
                    else if (holidayList == null)
                        dates.Add(dt);
                }

                if (dates.Count == 5)
                    break;
                dt = dt.AddDays(1);
            }
            for (var i = 1; i <= 5; i++)
            {
                _dashboard.GetDaysInRow(i).ShouldBeEqual(dates[i - 1].ToString("ddd MM/d"), "Date in column " + i);
            }


            //switch (day)
            //{
            //    case "Sun":
            //        for (int i = 1; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i), "Date in column " + i);
            //        break;
            //    case "Mon":
            //        for (int i = 1; i <= 4; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i), "Date in column " + i);
            //        for (int i = 5; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 2), "Date in column " + i);
            //        break;
            //    case "Tue":
            //        for (int i = 1; i <= 3; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i), "Date in column " + i);
            //        for (int i = 4; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 2), "Date in column " + i);
            //        break;
            //    case "Wed":
            //        for (int i = 1; i <= 2; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i), "Date in column " + i);
            //        for (int i = 3; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 2), "Date in column " + i);
            //        break;
            //    case "Thu":
            //        for (int i = 1; i <= 1; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i), "Date in column " + i);
            //        for (int i = 2; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 2), "Date in column " + i);
            //        break;
            //    case "Fri":
            //        for (int i = 1; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 2), "Date in column " + i);
            //        break;
            //    case "Sat":
            //        for (int i = 1; i <= 5; i++)
            //            _dashboard.GetDaysInColumn(i).ShouldBeEqual(_dashboard.GetActualDaysForColumn(i + 1), "Date in column " + i);
            //        break;
            //    default:
            //        Assert.Fail("Error in date format");
            //        break;
            //}
            StringFormatter.PrintMessageTitle("Verifying contents for data label Total Appeals by Day for the Next 5 days in Appeals Overview");
            _dashboard.GetNext5DaysColumnHeader().ShouldCollectionBeEqual(new List<string> { "Total", "Urgent", "Reviews" }, "Verify Next 5 days in Appeals Overview Header");
            _dashboard.GetNext5DaysValue().ForEach(x =>
                Convert.ToInt32(x).ShouldBeGreaterOrEqual(0, "Value on each day should be zero or greater than zero"));
        }

        [Test, Category("SmokeTestDeployment")]//US28490
        public void Verify_Appeals_Assigned_to_Each_Analyst_for_the_Next_5_days_appears_with_User_full_name_userid_in_parentheses()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IDictionary<string, string> paramLists = DataHelper.GetMappingData(FullyQualifiedClassName, "users_list");
            _dashboard.IsAppealsAssignedToEachAnalystPresent().ShouldBeTrue("Appeals Assigned to Each Analyst for the Next 5 days present");
            StringFormatter.PrintMessageTitle("Verifying Appeals Assigned appears with full name and user id");
            for (int i = 1; i <= _dashboard.AppealsAssignedRowCount(); i++)
            {
                String analyst = _dashboard.AppealsAssignedToAnalyst(i);
                analyst.DoesNameContainsNameWithUserName()
                    .ShouldBeTrue("Appeals Assigned to Analyst Full Name with userid in parentheses");

            }

        }

        [Test, Category("Working")] //US69710
        public void Verify_dashboard_authority_check_for_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var productDashboardViewList = testData["ProductDashboard"].Split(';');
            var myDashboardView = testData["MyDashboard"];
            var userFirstNameList = testData["FirstName"].Split(';');
            var userLastNameList = testData["LastName"].Split(';');
            var filterList = testData["FilterList"].Split(';');
            CurrentPage = _profileManager = _dashboard.Logout().LoginAsHciAdminUser1().NavigateToProfileManager();
            if (_profileManager.IsRespectiveAuthorityAssigned("Product Dashboards", "Read-Write"))
            {
                _profileManager.IsAuthorityAvailable("My Dashboard")
                    .ShouldBeTrue(string.Format("My Dashboard authority is not assigned for current user: {0}", EnvironmentManager.Username));
                _profileManager.ClickOnProfile();
                List<string> defaultDashboadViewList = _profileManager.GetDashboardViewProductType();
                defaultDashboadViewList.Contains(myDashboardView).ShouldBeFalse(string.Format("Default Dashboard View List should not contain: {0} for user not having My Dashboard authority.", myDashboardView));
                foreach (string prodDashboard in productDashboardViewList)
                {
                    defaultDashboadViewList.Contains(prodDashboard).ShouldBeTrue(
                        string.Format(
                            "Default Dashboard View List should contain: {0} for user having Product Dashboard authority.",
                            prodDashboard));
                }
                // _profileManager.SelectDefaultDashboard(productDashboardViewList[0]);

                CurrentPage = _dashboard = _profileManager.NavigateToDashboard();
                _dashboard.GetProductFilterList().ShouldBeEqual(filterList, "Filter List Options should have CV and FFP");
                _dashboard.IsCorrectDashboard("CV").ShouldBeTrue("Default dashboard should be CV for current user when no default dashboard is explicity selected.");
            }
            UserProfileSearchPage newuserProfile = QuickLaunch.NavigateToNewUserProfileSearch();
            _profileManager = newuserProfile.ClickonUserNameToNavigateProfileManagerPage(userFirstNameList[0], userLastNameList[0]);
            _profileManager.IsRespectiveAuthorityAssigned("Product Dashboards", "Read-Only").ShouldBeTrue(
                string.Format("Product Dashboards r/o authority should be present for current user: {0} {1}",
                    userFirstNameList[0], userLastNameList[0]));
            _profileManager.IsRespectiveAuthorityAssigned("My Dashboard", "Read-Write").ShouldBeTrue(
                string.Format("My Dashboard r/w authority should be present for current user: {0} {1}",
                    userFirstNameList[0], userLastNameList[0]));
            _profileManager.SelectDefaultDashboard(productDashboardViewList[1]);

            // CurrentPage = QuickLaunch.NavigateToNewUserProfileSearch();
            _profileManager = newuserProfile.ClickonUserNameToNavigateProfileManagerPage(userFirstNameList[1], userLastNameList[1]);
            _profileManager.IsRespectiveAuthorityAssigned("My Dashboard", "Read-Only")
                .ShouldBeTrue(string.Format("My Dashboard r/o authority is present for current user:{0} {1}",
                userFirstNameList[2], userLastNameList[2]));
            _profileManager.SelectDefaultDashboard(myDashboardView);

            //   CurrentPage = QuickLaunch.NavigateToNewUserProfileSearch();

            _profileManager = newuserProfile.ClickonUserNameToNavigateProfileManagerPage(userFirstNameList[2], userLastNameList[2]);
            _profileManager.ClickOnPrivileges();
            _profileManager.IsAuthorityAvailable("My Dashboard")
                .ShouldBeFalse(string.Format("My Dashboard authority is not present for client user: {0} {1}",
                userFirstNameList[2], userLastNameList[2]));

            CurrentPage = QuickLaunch.NavigateToNewUserProfileSearch();

            _profileManager = newuserProfile.ClickonUserNameToNavigateProfileManagerPage(userFirstNameList[3], userLastNameList[3]);
            _profileManager.ClickOnPrivileges();
            _profileManager.IsRespectiveAuthorityAssigned("Product Dashboards")
                .ShouldBeFalse(string.Format("Product Dashboards authority is not present for  user: {0} {1}",
                    userFirstNameList[2], userLastNameList[2]));

            CurrentPage = _quickLaunch = _profileManager.Logout().LoginAsClientUser4();
            _quickLaunch.IsDashboardIconPresent().ShouldBeFalse("Dashboard icon should not be visible for users with no dashboard authority.");
            _quickLaunch.Logout().LoginAsHciUserWithMyDashboardAuthorityAndReadOnlyProductDashboardAuthority();

            CurrentPage = _dashboard = _quickLaunch.NavigateToDashboard();
            _dashboard.IsMydashboardOptionPresentInDashboardMenu().ShouldBeTrue("My dashboard option should be present in dashboard menu when  it's authority is present for user");
            _dashboard.IsCorrectDashboard("FFP").ShouldBeTrue("Default dashboard should be FFP for current user.");
            _dashboard.Logout().LoginAsHciUserWithReadOnlyMyDashboardAndReadWriteProductDashboardAuthority().NavigateToDashboard();

            _dashboard.IsCorrectDashboard("My Dashboard").ShouldBeTrue("Default dashboard should be My Dashboard for current user.");
            _dashboard.Logout().LoginAsHciUserWithNoProductDashboardAuthority().NavigateToDashboard();

            _dashboard.GetProductFilterList().ShouldNotContain(productDashboardViewList, " Filter List Options should not contain CV and FFP");
        }

       
        #endregion

        #region PRIVATE METHODS

        private void Verify_datalabels_and_tooltip_for_Claims_Overview_widget()
        {
            string[] mappingOptionName =
            {
                "data_labels_in_claims_overview", "data_labels_in_tooltip_claims_overview", "data_lables_in_grid_view",
                "data_lables_tooltip_in_grid_view"
            };

            for (int i = 0; i <= mappingOptionName.Length - 1; i++)
            {
                List<string> expectedList = new List<string>(DataHelper.GetMappingData(FullyQualifiedClassName, mappingOptionName[i]).Values.ToList());

                switch (i)
                {
                    case 0:
                        _dashboard.GetClaimsOverveiwDataLabels().ShouldCollectionBeEqual(expectedList, "Claims Overview data labels should match");
                        break;
                    case 1:
                        _dashboard.GetClaimsOverveiwDataLabelsToolTip().ShouldCollectionBeEqual(expectedList, "Claims Overview data labels tooltip should match");
                        break;
                    case 2:
                        _dashboard.GetClaimsOverveiwGridViewDataLabels().ShouldCollectionBeEqual(expectedList, "Claims Overview gridview data labels should match");
                        break;
                    case 3:
                        _dashboard.ClaimsOverveiwGridViewDataLabelsToolTip().ShouldCollectionBeEqual(expectedList, "Claims Overview gridview data labels tooltip should match");
                        break;
                }

            }

        }

        private void Verify_claimscount_in_the_widget_with_database(string userSeq)
        {
            _dashboard.ClaimsOverviewGridViewClaimCount().ShouldCollectionBeEqual(
                _dashboard.GetClaimsCountInClaimsOverviewWidgetFromDatabase(userSeq),
                "Grid view Claim count values should match with the database");
            _dashboard.GetRealTimeClaimsInWidget().ShouldCollectionBeEqual(
                _dashboard.GetClaimsCountForRealTimeClient(userSeq),
                "Real time Claim count values should match with the database");
            _dashboard.GetUnreviewedClaimsInWidget().ShouldCollectionBeEqual(
                _dashboard.GetUnreviewedClaimsCountInWidget(userSeq),
                "Unreviewed Claim count values should match with the database");


        }

        #endregion
    }
}
