using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class DashboardAppealsDetail : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private readonly String _day = DateTime.Now.ToString("ddd");
        private AppealsDetailPage _appealsDetailPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _allActiveClientsList;
        private List<String> _allCotivitiUserWithAssignAppealsAuthList;
        private List<String> _allCotivitiUserWithDueAppeal;
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
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch.Logout().LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);
                _dashboard = QuickLaunch.NavigateToDashboard();
                _appealsDetailPage = _dashboard.ClickOnAppealsDetailExpandIcon();

            }
        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _appealsDetailPage.GetAllCotivitiExpectedLists(EnvironmentManager.HciAdminUsername);
            _allActiveClientsList = _allCotivitiExpectedLists[0];
            _allCotivitiUserWithAssignAppealsAuthList = _allCotivitiExpectedLists[1];
            _allCotivitiUserWithDueAppeal = _allCotivitiExpectedLists[2];
        }

        #endregion

        #region TEST SUITES


        //US29056 contains all test
        [Test]
        public void Verify_Appeals_Detail_page_header_and_first_and_second_container_header_in_Dashboard_Appeals_Detail()
        {

            _appealsDetailPage.CurrentPageTitle.ShouldBeEqual("Dashboard - Appeals Detail", "Appeal detail page opened.");
            _appealsDetailPage.GetAppealsDetailPageHeader().ShouldBeEqual("Dashboard - Appeals Detail", "Appeals detail page header");
            _appealsDetailPage.GetAppealsDetailContainerHeader(1).ShouldBeEqual("Appeals by Client Due Today", "First Container Header");
            _appealsDetailPage.GetAppealsDetailContainerHeader(2).ShouldBeEqual("Appeals by Analyst", "Second Container Header");
            _appealsDetailPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Last updated time appears in upper right corner");


        }

        [Test]//US69761
        public void Verify_that_assigned_clients_appears_and_clients_the_user_does_not_have_access_to_donot_appear_in_Appeals_by_Client_list()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name; 
            //IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            //string allActiveClients = paramLists["ActiveClients"];
            //string allInActiveClients = paramLists["InActiveClients"];

            //var allInActiveClientsList = allInActiveClients.Split(';').ToList();
            StringFormatter.PrintMessageTitle("Verifying active clients appear only in the client list.");
            _appealsDetailPage.GetActiveClients().ShouldCollectionBeEqual(_allActiveClientsList, "Appeals By Active Client with ascending order");
            //foreach (var activeClient in activeClientsList)
            //    allActiveClientsList.Contains(activeClient).ShouldBeTrue("Active Client " + activeClient + " Appears in list?");

            //foreach (var allInactiveClient in allInActiveClientsList)
            //    activeClientsList.Contains(allInactiveClient).ShouldBeFalse("InActive client " + allInactiveClient + " Appears in list?");
        }

        [Test] //CAR-2938 + CAR-3002(CAR-2939) + CAR-3010(CAR-2938)+CAR-3124(CAR-3065)
        public void Verify_the_column_headers_and_verify_client_values_sum_to_top_row_total_values_of_Appeals_by_Client()
        {
            var unrestrictedTotalAppealsCount =_appealsDetailPage.GetUnrestrictedTotalAppealsCount(EnvironmentManager.Username);
            var restrictedTotalAppealCount = _appealsDetailPage.GetRestrictedTotalAppealsCount(EnvironmentManager.Username);
            var unrestrictedTotalAppealsCountRecordReviewType = _appealsDetailPage.GetUnrestrictedTotalAppealsCountRecordReview(EnvironmentManager.Username);
            var restrictedTotalAppealCountRecordReviewType = _appealsDetailPage.GetRestrictedTotalAppealsCountRecordReview(EnvironmentManager.Username);
            var unrestrictedTotalAppealsCountUrgentPriority = _appealsDetailPage.GetUnrestrictedTotalAppealsCountUrgentPriority(EnvironmentManager.Username); 
            var restrictedTotalAppealCountUrgentPriority = _appealsDetailPage.GetRestrictedTotalAppealsCountUrgentPriority(EnvironmentManager.Username);
            var unrestrictedTotalAppealsCountMedicalRecordReviewType = _appealsDetailPage.GetUnrestrictedTotalAppealsCountMedicalRecordReview(EnvironmentManager.Username);
            var restrictedTotalAppealCountMedicalRecordReviewType = _appealsDetailPage.GetRestrictedTotalAppealsCountMedicalRecordReview(EnvironmentManager.Username);
            
            StringFormatter.PrintMessageTitle("Verifying column headers.");
            _appealsDetailPage.GetFirstRowColumnHeader(1).ShouldBeEqual("Total Appeals", "First row column header");
            _appealsDetailPage.GetFirstRowColumnHeader(2)
                .ShouldBeEqual("Urgent Appeals", "First row column header");
            _appealsDetailPage.GetFirstRowColumnHeader(3)
                .ShouldBeEqual("Record Reviews", "First row column header");
            _appealsDetailPage.GetFirstRowColumnHeader(4)
                .ShouldBeEqual("Medical Record Reviews", "First row column header");
            _appealsDetailPage.GetFirstRowColumnHeader(5)
                .ShouldBeEqual("Restricted Appeals", "First row column header");
            StringFormatter.PrintMessageTitle(
                "Verifying client values sum to top row total values and total count from database");
            for (int i = 1; i <= _appealsDetailPage.GetColumnHeadersCount(); i++)
            {
                _appealsDetailPage.GetHeaderValue(i).ShouldBeEqual(_appealsDetailPage.GetEachColumnSumValue(i),
                    "Total sum equals to top value in column " + i);
                _appealsDetailPage.GetHeaderValue(i).ShouldBeEqual(
                    _dashboard.GetTotalAppealCountsFromDatabaseByLabel
                        (_appealsDetailPage.GetFirstRowColumnHeader(i), EnvironmentManager.Username),
                    "Values Should Match Against Database");
            }

            var activeClientList = _appealsDetailPage.GetActiveClients();
            StringFormatter.PrintMessage("Verify Count For Each Client");
            for (int i = 1; i <= activeClientList.Count; i++)
            {
                _appealsDetailPage.GetEachGridValue(i, 1).ShouldBeEqual(
                    _appealsDetailPage.GetTotalAppealsCountByClient(activeClientList[i - 1]),
                    $"Total Appeals Value for {activeClientList[i - 1]} should match Against database");
                _appealsDetailPage.GetEachGridValue(i, 2).ShouldBeEqual(
                    _appealsDetailPage.GetUrgentAppealsCountByClient(activeClientList[i - 1]),
                    $"Urgent Appeals Value for {activeClientList[i - 1]} should match Against database");
                _appealsDetailPage.GetEachGridValue(i, 3).ShouldBeEqual(
                    _appealsDetailPage.GetRecordReviewsCountByClient(activeClientList[i - 1]),
                    $"Record Reviews Value for {activeClientList[i - 1]} should match Against database");
                _appealsDetailPage.GetEachGridValue(i, 4).ShouldBeEqual(
                    _appealsDetailPage.GetMedicalRecordReviewsCountByClient(activeClientList[i - 1]),
                    $"Medical Record Reviews Value for {activeClientList[i - 1]} should match Against database");
                _appealsDetailPage.GetEachGridValue(i, 5).ShouldBeEqual(
                    _appealsDetailPage.GetRestrictedAppealsCountByClient(activeClientList[i - 1]),
                    $"Restricted Appeals Value for {activeClientList[i - 1]} should match Against database");
            }

            #region CAR-3010

            StringFormatter.PrintMessage("Subtotal for Restricted Appeals will be part of the total Appeals count");
            _appealsDetailPage.GetHeaderValue(1).ShouldBeEqual(
                unrestrictedTotalAppealsCount + restrictedTotalAppealCount,
                "Total count should include restricted appeal count");

            StringFormatter.PrintMessage(
                "Subtotal for Restricted Appeals may be part of the total Record Review count");
            _appealsDetailPage.GetHeaderValue(3).ShouldBeEqual(
                unrestrictedTotalAppealsCountRecordReviewType + restrictedTotalAppealCountRecordReviewType,
                "Total record review count might include restricted appeal count");

            StringFormatter.PrintMessage(
                "Subtotal for Restricted Appeals will be part of the Urgent Appeals count");
            _appealsDetailPage.GetHeaderValue(2).ShouldBeEqual(
                unrestrictedTotalAppealsCountUrgentPriority + restrictedTotalAppealCountUrgentPriority,
                "Total urgent appeal count might include restricted appeal count");

            #endregion

            StringFormatter.PrintMessage(
                "Subtotal for Restricted Appeals may be part of the total Medical Record Review count");
            _appealsDetailPage.GetHeaderValue(4).ShouldBeEqual(
                unrestrictedTotalAppealsCountMedicalRecordReviewType + restrictedTotalAppealCountMedicalRecordReviewType,
                "Total record review count might include restricted appeal count");


            StringFormatter.PrintMessage(
                "Subtotal for Restricted Appeals will be part of the All Type of Appeals count");
            _appealsDetailPage.GetHeaderValue(5).ShouldBeEqual(
                restrictedTotalAppealCount,
                "Total Restricted appeals count might include restricted appeal count");
        }

        [Test, Category("IENonCompatible"), Category("BambooServerNonCompatible")]
        public void Verify_clicking_on_Download_PDF_button_of_Appeals_by_Client_downloads_pdf_document_and_verify_the_name_and_header_of_downloaded_document()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var currentTime = DateTime.Now;
            string fileName = string.Format(@"DASHBOARD_CLIENTAPPEALSUMMARY_{0}{1}{2}{3}{4}.xls", currentTime.Month.ToString("d2"), currentTime.Day.ToString("d2"), currentTime.Year, currentTime.ToString("hh"), currentTime.Minute.ToString("d2"));
            string testserver = ConfigurationManager.AppSettings["TestServer"],
                testserver2 = ConfigurationManager.AppSettings["TestNode"];
            bool isRemoteTestServer = !string.IsNullOrEmpty(testserver);
            string userName = isRemoteTestServer ? ConfigurationManager.AppSettings["ProxyUserName"] : System.Environment.UserName;

            char chartext = @"\".ToCharArray()[0];

            if (isRemoteTestServer)
            {
                testserver = @"\\" + testserver;
                testserver2 = @"\\" + testserver2;
            }
            
            _appealsDetailPage.ClickOnBoxWithArrowIconInAppealsByClient();
            _appealsDetailPage.IsSubMenuDownloadAppealsDetailPresentInAppealsbyClient().ShouldBeTrue("Download Appeals Detail submenu present");
            _appealsDetailPage.ClickOnDownloadAppealsDetailInAppealsByClient();
            const string workSheet = "ClientAppealsSummary";
            string downloadedHeader;
            string filelocation;
            string downloadedFileName;
            if (EnvironmentManager.Browser.Equals("IE", StringComparison.InvariantCulture))
            {
                fileName = string.Format(@"C:\Users\{0}\Downloads\{1}", userName, fileName);
                IEDownloadHelper.ClickSaveButtonToDownloadFile(isRemoteTestServer, testserver2, fileName);
                filelocation = isRemoteTestServer ? fileName.Substring(2) : fileName;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                fileName = fileName.Remove(0, fileName.LastIndexOf(chartext) + 1);
                downloadedFileName = filelocation.Remove(0, filelocation.LastIndexOf(chartext) + 1);
            }
            else
            {
                ChromeDownLoadPage chromeDownLoad = _appealsDetailPage.NavigateToChromeDownLoadPage();
                filelocation = chromeDownLoad.GetFileLocation();
                filelocation = filelocation.Replace("%20", " ");
                filelocation = isRemoteTestServer ? filelocation.Substring(2).Replace("/", @"\") : filelocation;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                downloadedFileName = isRemoteTestServer ? filelocation.Remove(0, filelocation.LastIndexOf(chartext) + 1) : filelocation.Remove(0, filelocation.LastIndexOf('/') + 1);
                _appealsDetailPage = chromeDownLoad.ClickBrowserBackButton<AppealsDetailPage>();
            }
            downloadedFileName.ShouldBeEqual(fileName, "Downloaded File Name");

            if (ExcelReader.GetDownloadedFileHeader(testserver + filelocation, testserver, workSheet, out downloadedHeader)
           || ExcelReader.GetDownloadedFileHeader(testserver2 + filelocation, testserver2, workSheet, out downloadedHeader))
            {
                downloadedHeader.ShouldBeEqual("\nDashboard - Appeals by Client", "Downloaded File Header");
            }
            else
            {
                _appealsDetailPage.AssertFail("File Not Found!");
            }

        }

        [Test, Category("IENonCompatible"), Category("BambooServerNonCompatible")]
        public void Verify_clicking_on_Download_PDF_button_of_Appeals_by_Analyst_downloads_pdf_document_and_verify_the_name_and_header_of_downloaded_document()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var currentTime = DateTime.Now;
            string fileName = string.Format(@"DASHBOARD_ANALYSTAPPEALSUMMARY_{0}{1}{2}{3}{4}.xls", currentTime.Month.ToString("d2"), currentTime.Day.ToString("d2"), currentTime.Year, currentTime.ToString("hh"), currentTime.Minute.ToString("d2"));
            string testserver = ConfigurationManager.AppSettings["TestServer"],
                testserver2 = ConfigurationManager.AppSettings["TestNode"];
            bool isRemoteTestServer = !string.IsNullOrEmpty(testserver);
            string userName = isRemoteTestServer ? ConfigurationManager.AppSettings["ProxyUserName"] : System.Environment.UserName;

            char chartext = @"\".ToCharArray()[0];

            if (isRemoteTestServer)
            {
                testserver = @"\\" + testserver;
                testserver2 = @"\\" + testserver2;
            }

            _appealsDetailPage.ClickOnBoxWithArrowIconInAppealsByAnalyst();
            _appealsDetailPage.IsSubMenuDownloadAppealsDetailPresentInAppealsbyAnalyst().ShouldBeTrue("Download Appeals Detail submenu present");
            _appealsDetailPage.ClickOnDownloadAppealsDetailInAppealsByAnalyst();
            string downloadedHeader;
            string filelocation;
            string downloadedFileName;
            const string workSheet = "AnalystAppealsSummary";
            if (EnvironmentManager.Browser.Equals("IE", StringComparison.InvariantCulture))
            {
                fileName = string.Format(@"C:\Users\{0}\Downloads\{1}", userName, fileName);
                IEDownloadHelper.ClickSaveButtonToDownloadFile(isRemoteTestServer, testserver2, fileName);
                filelocation = isRemoteTestServer ? fileName.Substring(2) : fileName;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                fileName = fileName.Remove(0, fileName.LastIndexOf(chartext) + 1);
                downloadedFileName = filelocation.Remove(0, filelocation.LastIndexOf(chartext) + 1);
            }
            else
            {
                ChromeDownLoadPage chromeDownLoad = _appealsDetailPage.NavigateToChromeDownLoadPage();
                filelocation = chromeDownLoad.GetFileLocation();
                filelocation = filelocation.Replace("%20", " ");
                filelocation = isRemoteTestServer ? filelocation.Substring(2).Replace("/", @"\") : filelocation;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                downloadedFileName = isRemoteTestServer ? filelocation.Remove(0, filelocation.LastIndexOf(chartext) + 1) : filelocation.Remove(0, filelocation.LastIndexOf('/') + 1);
                _appealsDetailPage = chromeDownLoad.ClickBrowserBackButton<AppealsDetailPage>();
            }

            downloadedFileName.ShouldBeEqual(fileName, "Downloaded File Name");

            if (ExcelReader.GetDownloadedFileHeader(testserver + filelocation, testserver, workSheet, out downloadedHeader)
            || ExcelReader.GetDownloadedFileHeader(testserver2 + filelocation, testserver2, workSheet, out downloadedHeader))
            {
                downloadedHeader.ShouldBeEqual("Dashboard - Appeals by Analyst", "Downloaded File Header");
            }
            else
            {
                _appealsDetailPage.AssertFail("File Not Found!");
            }
        }


        [Test, Category("SmokeTest")]//TE-984
        public void Verify_list_of_analyst_contains_Cotiviti_user_who_have_Appeals_can_be_assigned_to_user_authority_only_and_sorted()
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //var allCotivitiUserWithAssignAppealsAuthList = DataHelper.GetMappingData(FullyQualifiedClassName, "Cotiviti_user_with_Appeals_can_be_assigned_to_user_authority").Values.ToList();

            _allCotivitiUserWithAssignAppealsAuthList.Sort();
            //var allCotivitiUserWithoutAssignAppealsAuthList = DataHelper.GetMappingData(FullyQualifiedClassName, "Cotiviti_user_without_Appeals_can_be_assigned_to_user_authority").Values.ToList();
            //var allClientUsersList = DataHelper.GetMappingData(FullyQualifiedClassName, "Client_users").Values.ToList();
            var analystList = _appealsDetailPage.GetAnalysts();
            //_appealsDetailPage.IsListContainsCotivitiUserWithAssignAppelsAuth(analystList,allCotivitiUserWithAssignAppealsAuthList).ShouldBeTrue("List contains analyst who has Appeals can be assigned to user authority", true);
            analystList.ShouldCollectionBeEqual(_allCotivitiUserWithAssignAppealsAuthList,
                "List contains only Cotiviti user not client user  who has Appeals can be assigned to user authority and should be sorted");
            //_appealsDetailPage.IsListDoesNotContainsCotivitiUserWithoutAssignAppelsAuth(analystList,allCotivitiUserWithoutAssignAppealsAuthList).ShouldBeTrue("List does not contains analyst who does not have Appeals can be assigned to user authority", true);
            //_appealsDetailPage.IsListDoesNotContainsClientUsers(analystList,allClientUsersList).ShouldBeTrue("List doesnot contains client users", true);

        }

        //[Test]
        //public void Verify_users_appear_in_alphabetical_order_by_user_first_name()
        //{
        //    var displayed = new List<string>();
        //    var sorted = new List<string>();
        //    var analysts = _appealsDetailPage.GetAnalysts();
        //    foreach (string analyst in analysts)
        //    {
        //        displayed.Add(analyst);
        //        sorted.Add(analyst);
        //    }

        //    sorted.Sort();
        //    displayed.ShouldCollectionEqual(sorted, "Users appears in alpbhabetical order by user first name");
        //}

        [Test] //TE-984
        public void Verify_users_with_atleast_one_overdue_appeal_have_yellow_icon_next_to_their_name()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            _appealsDetailPage.GetAllYellowIconUserList().ShouldCollectionBeEqual(_allCotivitiUserWithDueAppeal,"User with atleast one overdue appeal");
        }

        [Test]
        public void Verify_column_headers_in_Appeals_by_Analyst()
        {
            StringFormatter.PrintMessageTitle("Verifying 1st row cloumn headers");
            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(1).ShouldBeEqual("Yesterday", "Header in Column 1");
            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(2).ShouldBeEqual("Today", "Header in Column 2");

            #region CAR-3002(CAR-2939)
            _appealsDetailPage.GetTotalAppealsCountForTodayInAppealsByAnalyst().ShouldBeEqual(_dashboard.GetTotalAppealCountsFromDatabaseByLabel("Total Appeals", EnvironmentManager.Username));
            #endregion

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
            for (var i = 3; i <= 7; i++)
            {
                _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(dates[i - 3].ToString("ddd MM/d"), "Header in column " + i);
            }

            //switch (_day)
            //{
            //    case "Sun":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        break;
            //    case "Mon":
            //        for (int i = 3; i <= 6; i++) _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 7; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Tue":
            //        for (int i = 3; i <= 5; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 6; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Wed":
            //        for (int i = 3; i <= 4; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 5; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Thu":
            //        for (int i = 3; i <= 3; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i), "Header in column " + i);
            //        for (int i = 4; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Fri":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 2), "Header in column " + i);
            //        break;
            //    case "Sat":
            //        for (int i = 3; i <= 7; i++)
            //            _appealsDetailPage.GetFirstRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(_appealsDetailPage.GetActualDaysForColumn(i + 1), "Header in column " + i);
            //        break;
            //    default:
            //        Assert.Fail("Error in date format");
            //        break;
            //}

            StringFormatter.PrintMessageTitle("Verifying 2nd row cloumn headers");
            for (int i = 1; i <= _appealsDetailPage.GetSecondRowColumnHeaderCountAppealsByAnalyst(); i++)
            {
                _appealsDetailPage.GetSecondRowColumnHeaderAppealsByAnalyst(i).ShouldBeEqual(i == 1 ? "Completed Appeals" : "Total Appeals", "Header in column " + i);
            }
        }

        #endregion
    }
}
