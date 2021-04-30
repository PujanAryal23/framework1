using System;
using System.Collections.Generic;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    class DashboardClientAppealsDetail : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private readonly String _day = DateTime.Now.ToString("ddd");
        private AppealsDetailPage _appealsDetailPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _allActiveClientsList; 


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
                _appealsDetailPage = _dashboard.ClickOnAppealsDetailExpandIcon();
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
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) == 0)
            {
                QuickLaunch.Logout().LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);
                _dashboard = QuickLaunch.NavigateToDashboard();
                _appealsDetailPage = _dashboard.ClickOnAppealsDetailExpandIcon();

            }
        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _appealsDetailPage.GetAllClientExpectedLists(EnvironmentManager.ClientUserName);
            _allActiveClientsList = _allCotivitiExpectedLists[0];
        }

        #endregion

        #region TEST SUITES

        [Test]//US37255
        public void Verify_Appeals_Detail_page_header_and_first_and_second_container_header_in_Dashboard_Appeals_Detail()
        {
            _appealsDetailPage.CurrentPageTitle.ShouldBeEqual("Dashboard - Appeals Detail", "Appeal detail page opened.");
            _appealsDetailPage.GetAppealsDetailPageHeader().ShouldBeEqual("Dashboard - Appeals Detail", "Appeals detail page header");
            _appealsDetailPage.GetAppealsDetailContainerHeader(1).ShouldBeEqual("Appeals Outstanding by Client", "First Container Header");
            _appealsDetailPage.GetAppealsDetailContainerHeader(2).ShouldBeEqual("Appeals Outstanding by Creator and Due Date", "Second Container Header");
            _appealsDetailPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Last updated time appears in upper right corner");


        }

        [Test]//US37255
        public void Verify_that_clients_the_user_has_access_to_appears_in_Appeals_by_Client_list_and_in_alphabetical_order()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            //var allActiveClients = paramLists["Clients"];
            
            StringFormatter.PrintMessageTitle("Verifying active clients appear  in the client list.");
            _appealsDetailPage.GetActiveClients().ShouldCollectionBeEqual(_allActiveClientsList, "Appeals By Active and authorized Client with ascending order");
            //foreach (var activeClient in activeClientsList)
            //    allActiveClientsList.Contains(activeClient).ShouldBeTrue("Client " + activeClient + " appears in list?");
            //StringFormatter.PrintMessageTitle("Client List is in alphabetical order");
            //allActiveClientsList.IsInAscendingOrder().ShouldBeTrue("Client List is in alphabetical order");
        }

         [Test]//US37255
        public void Verify_the_Appeal_Detail_has_no_null_appeal_Value_and_verify_client_values_sum_to_top_row_total_values_of_Appeals_by_Client()
        {
            _appealsDetailPage.IsAppealDetailsHeaderValueByClientNull().ShouldBeFalse("Appeal Detail By Client Header values have null column?");
            _appealsDetailPage.IsAppealDetailsYesterdayHeaderValueByClientNull().ShouldBeFalse("Appeal Detail By Client Yesterday Header values have null column?");
            _appealsDetailPage.IsAppealDetailsValueApppealByClientNull().ShouldBeFalse("Appeal Detail By Client Values have null data?");
            StringFormatter.PrintMessageTitle("Verifying client values sum to top row total values");
            for (int i = 1; i <= _appealsDetailPage.GetFirstRowColumnHeaderAppealByClientCount(); i++)
                _appealsDetailPage.GetFirstRowColumnHeaderValueAppealByClient(i).ShouldBeEqual(_appealsDetailPage.GetEachColumnSumValueAppealByClient(i), "Total sum equals to top value in column " + i);

        }

         [Test]//US37257
        public void Verify_column_headers_in_Appeals_by_Analyst_and_Appeals_Details_by_Analyst_and_verify_Analyst_is_in_alphabetical_order_by_firstname()
        {
            _appealsDetailPage.IsAppealByAnalystSectionPresent().ShouldBeTrue("<Appeals by Analyst section> is displayed below the <Appeals by Client section>");
            StringFormatter.PrintMessageTitle("Verifying Appeal Detials By Analyst");
            _appealsDetailPage.IsAppealByAnalystSecondHeaderValueNull().ShouldBeFalse("Appeal Detail by Analyst Header values have null data?");
            _appealsDetailPage.IsAppealDetailsValueApppealByAnalystNull().ShouldBeFalse("Appeal Detail by Analyst values have null data?");


             StringFormatter.PrintMessageTitle("Verifying 1st row cloumn headers");
            
            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(1).ShouldBeEqual("Today", "Header in Column 2");
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
            for (var i = 2; i <= 6; i++)
            {
                _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(dates[i - 2].ToString("ddd MM/d"), "Header in column " + i);
            }

            //switch (_day)
            //{
            //    case "Sun":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        break;
            //    case "Mon":
            //        for (int i = 3; i <= 6; i++) _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 7; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Tue":
            //        for (int i = 3; i <= 5; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 6; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Wed":
            //        for (int i = 3; i <= 4; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 5; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Thu":
            //        for (int i = 3; i <= 3; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 4; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Fri":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Sat":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i-1).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 1), "Header in column " + i);
            //        break;
            //    default:
            //        Assert.Fail("Error in date format");
            //        break;
            //}

            StringFormatter.PrintMessageTitle("Verifying Analyst is in alphabetical order");
            _appealsDetailPage.IsAppealByAnalystAscendingOrder().ShouldBeTrue("Appeal By Analyst is in Alphabetical Order");
        }

       
        #endregion
    }
}
