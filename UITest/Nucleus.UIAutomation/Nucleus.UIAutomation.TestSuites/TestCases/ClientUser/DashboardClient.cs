using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    class DashboardClient : AutomatedBaseClient
    {

        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private String day = DateTime.Now.ToString("ddd");

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
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestCleanUp()
        {
            if (!CurrentPage.CurrentPageTitle.Equals(PageTitleEnum.Dashboard.GetStringValue()))
            {
                CurrentPage =
                    _dashboard = QuickLaunch.NavigateToDashboard();
            }
            base.TestCleanUp();

        }



        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] // CAR-3204 [CAR-3243]
        [Author("Shyam Bhattarai")]
        public void Verify_appeal_overdue_icon_should_not_be_shown_when_no_appeals_are_overdue_for_client_user()
        {
            _dashboard = _dashboard.Logout().LoginAsPciTest5ClientUser().NavigateToDashboard();
            _dashboard.IsAppealOverdueIconPresent().ShouldBeFalse("Appeal Overdue icon should not be present when none of the appeals are over due");
        }

        [Test]//US33627 + TE-595
        public void Verify_Claims_Overview_section_is_present_with_PCI_in_right_corner_and_last_updated_time_in_lower_right_corner()
        {
            _dashboard.IsContainerHeaderClaimsOverviewPresent().ShouldBeTrue("Container header Claims Overview present");
            _dashboard.IsContainerHeaderClaimsOverviewPCIPresent().ShouldBeTrue("Container header has CV in right corner");
            _dashboard.IsNextRefreshTimePresent().ShouldBeTrue("Last updated time appears in upper right corner");
            //_dashboard.ClickOnRefreshIconAndWait();
            //_dashboard.GetLastUpdatedTimeInLowerRightCorner().ShouldBeEqual($"Next Refresh : {_dashboard.GetNextRefreshForPCIDashboardClient(EnvironmentManager.Username)}", "Next Refresh");
        }



        [Test, Category("SmokeTestDeployment")]//US33627
        public void Verify_the_data_labels_in_Claims_Overview_and_Validate_when_the_user_hovers_over_data_points_the_corresponding_text_is_displayed_in_a_browser_tooltip()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            _dashboard.IsClaimsOverveiwDataWidgetPresent().ShouldBeTrue("Claims Overview data widget present");

            IList<string> expectedDataLablesInClaimsOverviewList = DataHelper.GetMappingData(FullyQualifiedClassName, "numerator_denominator_data_labels_in_claims_overview").Values.ToList();
            _dashboard.GetClaimsOverveiwDataLabels().ShouldCollectionBeEqual(expectedDataLablesInClaimsOverviewList, "Claims Overview data lables");


            var mainToolTipList = new List<string>
            {
                "Total number of CV claims in Client Unreviewed status.", "Total number of pended CV claims.",
                "Total number of CV claims that have been reviewed but are still in Client Unreviewed status"

            };
            var secondaryToolTipList = new List<string>
            {
                "Total number of Client Unreviewed CV claims that were received more than 10 business days ago.",
                "Total number of pended CV claims received more than 10 business days ago.",
                "Total number of CV claims approved on the previous business day."
            };

            var messageList = new List<string>
            {
                "Unreviewed Claims Tooltip",
                "Pended Claims Tooltip",
                "Unapproved Claims Tooltip"
            };

            for (int i = 0; i < 3; i++)
            {
                _dashboard.GetClaimOverviewMainValueToolTip(i + 1).ShouldBeEqual(mainToolTipList[i], messageList[i]);
                _dashboard.GetClaimOverviewSecondaryValueToolTip(i + 1).ShouldBeEqual(secondaryToolTipList[i], messageList[i]);
            }


        }


        [Test, Category("SmokeTestDeployment")]//US33629
        public void Verify_container_header_Appeals_Overview_is_present_with_PCI_in_right_corner_and_verify_the_data_labels()
        {
            _dashboard.GetAppealsOverviewDivHeader().ShouldBeEqual("Appeals Overview", "Appeals Div Header");
            _dashboard.IsContainerHeaderAppealsOverviewPCIPresent().ShouldBeTrue("Container header has CV in right corner");
            StringFormatter.PrintMessageTitle("Verifying Appeals Overview data labels");
            _dashboard.GetAppealsOverviewDataLabel(1).ShouldBeEqual("Total Appeals", "Total Appeals data label");
            _dashboard.GetAppealsOverviewDataLabel(2).ShouldBeEqual("Urgent Appeals", "Urgent Appeals data label");
            _dashboard.GetAppealsOverviewDataLabel(3).ShouldBeEqual("Record Reviews", "Record Reviews data label");
        }

        [Test, Category("SmokeTestDeployment")]//US37252
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

            //StringFormatter.PrintMessageTitle("Verifying cloumn header");
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
            StringFormatter.PrintMessageTitle("Veryfing contents for data label Total Appeals by Day for the Next 5 days in Appeals Overview");
            _dashboard.GetNext5DaysColumnHeader().ShouldCollectionBeEqual(new List<string> { "Total", "Urgent", "Reviews" }, "Verify Next 5 days in Appeals Overview Header");
            _dashboard.GetNext5DaysValue().ForEach(x =>
                Convert.ToInt32(x).ShouldBeGreaterOrEqual(0, "Value on each day should be zero or greater than zero"));
        }

        [Test, Category("SmokeTestDeployment")]//US37261
        public void Verify_that_appeal_record_for_next_5_days_and_Appeal_Creator_display_format()
        {
            _dashboard.GetNext5DaysValue().ForEach(x =>
                Convert.ToInt32(x).ShouldBeGreaterOrEqual(0, "Value on each day should be zero or greater than zero"));
            if (_dashboard.GetNoAppealMessage() == "No appeals are due in the next 5 days")
            {
                Console.WriteLine("No appeals are due in the next 5 days");
            }
            else
            {
                _dashboard.GetAppealCreatorCount()
                    .ShouldBeLessOrEqual(5, "Appeal Creator should not more than 5 Cotiviti Client Users");

                _dashboard.IsAppealCreatorAppealValueZero().ShouldBeFalse("Appeals have zero value?");
                _dashboard.IsAppealCreatorAppealValuesListInDescendingOrder()
                    .ShouldBeTrue("Appeal Creator Appeals Value Should be in descending order");
                foreach (var analyst in _dashboard.GetAppealCreatorAnalystList())
                {
                    if (analyst.Contains("System")) continue;
                    VerifyThatNameIsInCorrectFormat(analyst);
                }
            }

        }

        [Test, Category("SmokeTestDeployment")]//US37259
        public void Verify_PCI_Logic_Request_Overview_container_and_its_highcharts()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string testedBrowser = EnvironmentManager.Browser;
            _dashboard.IsLogicRequestOverviewDivPresent().ShouldBeTrue("Logic Request Overview section present?");
            _dashboard.GetLogicRequestOverviewDivHeaderText().ShouldBeEqual("Logic Request Overview", "Header Text");
            _dashboard.IsLogicRequestOverdueGraphPresent().ShouldBeTrue("Ready for Client Review chart present?");
            _dashboard.IsLogicRequestDueIn30MinutesGraphPresent().ShouldBeTrue("Awaiting Response chart present?");

            var readyForClientReviewRange = _dashboard.GetLogicRequestOverdueGraphYAxisText().Trim();

            readyForClientReviewRange.StartsWith("0").ShouldBeTrue("Ready for Client Review graph Range Should Start from Zero");
            (testedBrowser.Equals("IE", StringComparison.InvariantCulture)
                  ? Convert.ToInt32(readyForClientReviewRange)
                  : Convert.ToInt32(readyForClientReviewRange.Replace("\r\n", ""))).ShouldBeGreater(0, "Y-axis Range of Ready for Client Review graph");
            var awaitingCotivitiReviewRange = _dashboard.GetLogicRequestDueIn30MinsGraphYAxisText().Trim();

            readyForClientReviewRange.StartsWith("0").ShouldBeTrue("Awaiting Response graph Range Should Start from Zero");
            (testedBrowser.Equals("IE", StringComparison.InvariantCulture)
                      ? Convert.ToInt32(awaitingCotivitiReviewRange)
                      : Convert.ToInt32(awaitingCotivitiReviewRange.Replace("\r\n", ""))).ShouldBeGreater(0, "Y-axis Range of Awaiting Response graph");


            _dashboard.GetLogicRequestOverdueGraphYAxisLabel().ShouldBeEqual("Ready for Client Review", "1st  graph label");
            _dashboard.GetLogicRequestDueIn30MinsGraphYAxisLabel().ShouldBeEqual("Awaiting Response", "2nd  graph label");
            Convert.ToInt32(_dashboard.GetLogicRequestOverdueGraphValue()).ShouldBeGreaterOrEqual(0, "Logic Request Ready for Clinet Review value is greater or equal to zero");
            Convert.ToInt32(_dashboard.GetLogicRequestDueIn30MinsGraphValue()).ShouldBeGreaterOrEqual(0, "Logic Request Awaiting Response value is greater or equal to zero");
        }





        #endregion

        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectFormat(string name)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue("The Name '" + name + "' is in format XXX XXX (XXX)");
        }
    }
}
