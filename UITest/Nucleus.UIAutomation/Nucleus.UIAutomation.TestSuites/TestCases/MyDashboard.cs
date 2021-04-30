 using System;
using System.Collections.Generic;
 using System.Diagnostics;
 using System.Globalization;
 using System.Linq;
 using Nucleus.Service.PageServices.ChromeDownLoad;
 using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
 using UIAutomation.Framework.Utils;

 namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class MyDashboard : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private ProfileManagerPage _profileManager;
        private DashboardPage.UserKpi _myDashboardForUser;
        


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
                UserLoginIndex = 5;
                base.ClassInit();
                _dashboard = QuickLaunch.NavigateToMyDashboard();
                try
                {
                    _isDashboardOpened = _dashboard.IsDashboardPageOpened();
                    GetMyDashboardMvUserKpiDetails();
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
            if (string.Compare(UserType.CurrentUserType, UserType.AdminWithManageEditAuthority, StringComparison.OrdinalIgnoreCase) != 0)
            {
                //_dashboard.ClickOnQuickLaunch();
                //QuickLaunch =
                //    QuickLaunch.Logout()
                //        .LoginAsHciUserWithMyDashboardAuthorityAndReadOnlyProductDashboardAuthority()
                //        .ClickOnSwitchClient()
                //        .SwitchClientTo(EnvironmentManager.Instance.TestClient);               
                _dashboard = _dashboard.Logout().LoginAsHciUserWithMyDashboardAuthorityAndReadOnlyProductDashboardAuthority().NavigateToMyDashboard();

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

        private void GetMyDashboardMvUserKpiDetails()
        {
            _myDashboardForUser = _dashboard.GetMyDashboardDetailsForUserFromMvUserKpiMaterializedView(EnvironmentManager.HciUserWithManageEditAuthority);
        }


        #region TEST SUITES
      
        //[Test]//US69711
        //public void Verify_my_dashboard_option_in_dashboard_menu()
        //{
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.IsRespectiveAuthorityAssigned("My Dashboard", "Read-Write").ShouldBeTrue("My Dashboard r/w authority is present for current user:" + EnvironmentManager.Username);
        //    _dashboard = _profileManager.NavigateToDashboard();
        //    _dashboard.IsMydashboardOptionPresentInDashboardMenu().ShouldBeTrue("My dashboard option should be present in dashboard menu when  it's authority is present for user");

        //}

        [Test]//US69712
        public void Verify_claim_performance_widget_in_my_dashboard()
        {
            _dashboard.IsWidgetPresentInMydashboard().ShouldBeTrue("Claim Performance widget should be present in my dashboard page.");
            _dashboard.IsMydashboardShownInRightCornerForWidget().ShouldBeTrue("My Dashboard should be showin in right corner of Claim Performance widget.");
            _dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek()
                .ShouldBeEqual(_myDashboardForUser.WeightedClaimsCompleted, "Expected Total Claims this week should be equal to " + _myDashboardForUser.WeightedClaimsCompleted);
            _dashboard.GetValueOfAvgPerHourThisWeek()
                .ShouldBeEqual(_myDashboardForUser.TotalAvgClaimsPerHour, "Expected Average Per Hour of total claims this week should be equal to " + _myDashboardForUser.TotalAvgClaimsPerHour);
            _dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Claims This Week")
                .ShouldBeEqual(_myDashboardForUser.NucleusClaimsReviewed, "Expected Nucleus Claims This Week should be equal to " + _myDashboardForUser.NucleusClaimsReviewed);
            _dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Claims This Week")
                .ShouldBeEqual(_myDashboardForUser.LegacyClaimsReviewed, "Expected Legacy Claims This Week should be equal to " + _myDashboardForUser.LegacyClaimsReviewed);
            _dashboard.GetValueOfSecondaryWidgetAvgPerHour()
                .ShouldBeEqual(_myDashboardForUser.NucleusAvgClaimsPerHour, "Expected Average Per Hour of nucleus claims this week should be equal to " + _myDashboardForUser.NucleusAvgClaimsPerHour);
            _dashboard.GetValueOfSecondaryWidgetAvgPerHour(2)
                .ShouldBeEqual(_myDashboardForUser.LegacyAvgClaimsPerHour, "Expected Average Per Hour of legacy claims this week should be equal to " + _myDashboardForUser.LegacyAvgClaimsPerHour);
        }

        [Test, Category("SmokeTestDeployment"),Category("ExcludeDailyTest")]//US69712
        public void Verify_claim_performance_widget_in_my_dashboard_SmokeTest()
        {
            _dashboard.IsWidgetPresentInMydashboard().ShouldBeTrue("Claim Performance widget should be present in my dashboard page.");
            _dashboard.IsMydashboardShownInRightCornerForWidget().ShouldBeTrue("My Dashboard should be showin in right corner of Claim Performance widget.");
            Convert.ToInt32(_dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek())
                .ShouldBeGreaterOrEqual(0, "Expected Total Claims this week should not empty");
            Convert.ToDouble(_dashboard.GetValueOfAvgPerHourThisWeek())
                .ShouldBeGreaterOrEqual(0, "Expected Average Per Hour of total claims this week should not empty");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Claims This Week"))
                .ShouldBeGreaterOrEqual(0, "Expected Nucleus Claims This Week should not empty");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Claims This Week"))
                .ShouldBeGreaterOrEqual(0, "Expected Legacy Claims This Week should not empty");
            Convert.ToDouble(_dashboard.GetValueOfSecondaryWidgetAvgPerHour())
                .ShouldBeGreaterOrEqual(0, "Expected Average Per Hour of nucleus claims this week should not empty");
            Convert.ToDouble(_dashboard.GetValueOfSecondaryWidgetAvgPerHour(2))
                .ShouldBeGreaterOrEqual(0, "Expected Average Per Hour of legacy claims this week should not empty");
        }


        [Test]//US69713 + CAR-3166(CAR-3045)
        public void Verify_appeal_performance_widget_in_my_dashboard()
        {
            _dashboard.IsWidgetPresentInMydashboard(false).ShouldBeTrue("Appeal Performance widget should be present in my dashboard page.");
            _dashboard.IsMydashboardShownInRightCornerForWidget(false).ShouldBeTrue("My Dashboard should be shown in right corner of Claim Performance widget.");
            
            _dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek(false)
                .ShouldBeEqual(_myDashboardForUser.WeightedAppealsCompleted,
                    "Expected Total Weighted Appeals this week should be equal to " + _myDashboardForUser.WeightedAppealsCompleted);
            
            _dashboard.GetValueOfAvgPerHourThisWeek(false)
                .ShouldBeEqual(_myDashboardForUser.AvgAppealsPerHour, 
                    "Expected Average Per hour of total appeals this week should be equal to " + _myDashboardForUser.AvgAppealsPerHour);
            
            _dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Appeals This Week")
                .ShouldBeEqual(_myDashboardForUser.NucleusAppealsReviewed, 
                    "Expected Nucleus Appeals This Week should be equal to " + _myDashboardForUser.NucleusAppealsReviewed);
            
            _dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Appeals This Week")
                .ShouldBeEqual(_myDashboardForUser.LegacyAppealsReviewed, 
                    "Expected Legacy Appeals This Week should be equal to " + _myDashboardForUser.LegacyAppealsReviewed);
            
            #region CAR-3166(CAR-3045) 
            Convert.ToDouble(_dashboard.GetValueOfWeightedClaimsOrAppealThisWeek("Total Weighted Claims This Week"))
                .ShouldBeGreaterOrEqual(0, "Total Weighted Claims Completed This Week should not empty");
            #endregion
        }

        [Test, Category("SmokeTestDeployment"),Category("ExcludeDailyTest")]//US69713 + CAR-3166(CAR-3045)
        public void Verify_appeal_performance_widget_in_my_dashboard_SmokeTest()
        {
            _dashboard.IsWidgetPresentInMydashboard(false).ShouldBeTrue("Appeal Performance widget should be present in my dashboard page.");
            _dashboard.IsMydashboardShownInRightCornerForWidget(false).ShouldBeTrue("My Dashboard should be shown in right corner of Claim Performance widget.");
            Convert.ToInt32(_dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek(false))
                .ShouldBeGreaterOrEqual(0, "Expected Total Appeals this week should not empty");
            Convert.ToDouble(_dashboard.GetValueOfAvgPerHourThisWeek(false))
                .ShouldBeGreaterOrEqual(0, "Expected Average Per hour of total appeals this week should not empty");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Appeals This Week"))
                .ShouldBeGreaterOrEqual(0, "Expected Nucleus Appeals This Week should not empty");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Appeals This Week"))
                .ShouldBeGreaterOrEqual(0, "Expected Legacy Appeals This Week should not empty");

            #region CAR-3166(CAR-3045) 
            Convert.ToDouble(_dashboard.GetValueOfWeightedClaimsOrAppealThisWeek("Weighted Appeals This Week"))
                .ShouldBeGreaterOrEqual(0, "Weighted Appeals This Week should not empty");
            #endregion
        }



        [Test] //US69713 + US69712 + CAR-3166(CAR-3045)
        public void Verify_last_updated_in_my_dashboard_for_different_udpated_time_scenario()
        {
            var setDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time");
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDate.ToString("MM/dd/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture));
            var expectedMessage = (TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time") - setDate).TotalSeconds < 60 ? "a few seconds ago" : "in a minute";
            ValidateLastUpdated(expectedMessage);
            var setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddSeconds(60).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("in a minute");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddMinutes(-2).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated((TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time") - DateTime.Parse(setDateAsString)).Minutes + " minutes ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddDays(-1).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("a day ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddDays(-2).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("2 days ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddMonths(-1).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("a month ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddMonths(-2).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("2 months ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddYears(-1).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("a year ago");
            setDateAsString = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Mountain Standard Time").AddYears(-3).ToString("MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            _dashboard.UpdateLastUpdpatedDateAndResfreshMvUuserKpi(setDateAsString);
            ValidateLastUpdated("3 years ago");
        }

        [Test] //CAR-3191(CAR-3176)
        public void Verify_Excel_Export_In_My_Dashboard()
        {
            var TestName = new StackFrame(true).GetMethod().Name;
            var parameterList = DataHelper.GetTestData(FullyQualifiedClassName, TestName);
            var sheetName = parameterList["sheetName"];
            var expectedHeaders = parameterList["headers"].Split(',').ToList();
            List<string>widgetName= new List<string>() { "Claim Performance", "Appeal Performance" };
            var loggedInUser = _dashboard.GetLoggedInUserFullName();
            foreach (var widget in widgetName)
            {
                _dashboard.ClickonExportIconByWidget(widget);
                var mountainTime =
                    TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Mountain Standard Time").ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                var fileName = _dashboard.GoToDownloadPageAndGetFileName();
                ExcelReader.ReadExcelSheetValue(fileName, sheetName, 2, 1, out List<string> headerList,
                    out List<List<string>> excelExportList, out string name, false, username: true);
                name.Replace("\n", "").ShouldBeEqual($"My Dashboard Metrics - {loggedInUser} {mountainTime}", "username and date correct?");
                expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                _dashboard = QuickLaunch.NavigateToMyDashboard();
                VerifyMyDasboardExcelExport(excelExportList);
                ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);
            }

        }


        

        #region private methods

        public void VerifyMyDasboardExcelExport(List<List<string>> excelExportList)
        {
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Claims This Week"))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][0]), "Expected Nucleus claims This Week should match");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Claims This Week"))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][1]), "Expected Legacy claims This Week should match");
            Convert.ToInt32(_dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek(true))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][2]), "Expected Total Claims this week should match");
            Convert.ToDouble(_dashboard.GetValueOfAvgPerHourThisWeek(true))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][3]), "Expected Average Per hour of total claims this week should match");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Nucleus Appeals This Week"))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][4]), "Expected Nucleus appeals This Week should match");
            Convert.ToInt32(_dashboard.GetValueOfClaimsOrAppealThisWeek("Legacy Appeals This Week"))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][5]), "Expected Legacy appeals This Week should match");
            Convert.ToInt32(_dashboard.GetValueOfTotalWeightedClaimsOrAppealThisWeek(false))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][6]), "Expected Total Appeals this week should match");
            Convert.ToDouble(_dashboard.GetValueOfAvgPerHourThisWeek(false))
                .ShouldBeEqual(Convert.ToInt32(excelExportList[0][7]), "Expected Average Per hour of total appeals this week should match");
        }

        public void ValidateLastUpdated(string expectedMessage)
        {
            CurrentPage.RefreshPage(false);
            _dashboard.SelectMyDashboardFromFilterOptions();
            _dashboard.GetValueOfLasUpdatedDataInSecondaryWidget()
                .ShouldBeEqual(string.Format("Last updated {0}", expectedMessage), "Expected last updated message in claim performance widget should be: " + expectedMessage);
            _dashboard.GetValueOfLasUpdatedDataInSecondaryWidget(false)
                .ShouldBeEqual(string.Format("Last updated {0}", expectedMessage), "Expected last updated message in claim performance widget should be: " + expectedMessage);
        }
        #endregion
    }
}
#endregion