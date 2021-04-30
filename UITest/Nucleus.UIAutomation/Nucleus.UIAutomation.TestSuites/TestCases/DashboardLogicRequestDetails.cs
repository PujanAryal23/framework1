using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class DashboardLogicRequestDetail : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private readonly String _day = DateTime.Now.ToString("ddd");
        private DashboardLogicRequestsDetailsPage _logicRequestDetailsPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _expectedLogicByClientList;


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
                _logicRequestDetailsPage = _dashboard.ClickOnLogicRequestsDetailExpandIcon();
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
                _logicRequestDetailsPage = _dashboard.ClickOnLogicRequestsDetailExpandIcon();

            }
        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _logicRequestDetailsPage.GetAllCotivitiExpectedLists(EnvironmentManager.HciAdminUsername);
            _expectedLogicByClientList = _allCotivitiExpectedLists[0];
        }

        #endregion

        #region TEST SUITES

     
        //US31609 contains all test
        [Test]
        public void Verify_clicking_on_logicrequest_expand_icon_opens_logicrequestdetail_page()
        {
            _logicRequestDetailsPage.PageUrl.ShouldBeEqual(_logicRequestDetailsPage.CurrentPageUrl,
                                                                "Navigated to Dashboard- CV Logic Requests Detail");
            _logicRequestDetailsPage.GetLogicRequestsDetailHeader().ShouldBeEqual("Dashboard - Logic Requests Detail","Logic Requests Detail page header");
            _logicRequestDetailsPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Last updated time appears in upper right corner");


        }

        [Test]
        public void Verify_Logic_Requests_By_Client_grid_header()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _logicRequestDetailsPage.GetLogicRequestsGridHeader().ShouldBeEqual("Logic Requests By Client");

        }

        [Test, Category("SmokeTest")]//US69761
        public void Verify_Logic_Requests_By_Client_grid_contains_list_of_all_PCI_active_assigned_clients_in_alphabetical_order()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            //var clientsList = paramLists["ClientList"];
            
            //List<string> actualClientsList = _logicRequestDetailsPage.GetClients();
            //Assert.That(actualClientsList,Is.Ordered);
            _logicRequestDetailsPage.GetClients().ShouldCollectionBeEqual(_expectedLogicByClientList, "Logic Reuests by Active and only " +
                "assigned clients with sorted ordered");
        }

        [Test]
        public void Verify_grid_column_headers_in_dashboard_logic_requests_detail_and_verify_client_values_sum_to_top_row_total_value()
        {
             TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _logicRequestDetailsPage.GetOverdueLogicRequestsColumnHeaderText().ShouldBeEqual("Overdue Requests" ,"1st column header");
            _logicRequestDetailsPage.GetDueIn30MinsLogicRequestsColumnHeaderText().ShouldBeEqual("Due In 30 Minutes", "2nd column header");
            _logicRequestDetailsPage.GetOpenLogicRequestsColumnHeaderText().ShouldBeEqual("Open Requests", "3rd column header");
             StringFormatter.PrintMessageTitle("Verifying client values sum to top row total values");
            _logicRequestDetailsPage.GetTotalOverdueLogicRequestsCount().ShouldBeEqual(_logicRequestDetailsPage.GetSumofOverdueRequestsForAllClient(), "Total overdue client requests equals to top value in column ");
            _logicRequestDetailsPage.GetTotalDueIn30MinsLogicRequestsCount().ShouldBeEqual(_logicRequestDetailsPage.GetSumofDueIn30MinsRequestsForAllClient(), "Total due in 30 mins  client requests equals to top value in column ");
            _logicRequestDetailsPage.GetTotalOpenLogicRequestsCount().ShouldBeEqual(_logicRequestDetailsPage.GetSumofOpenRequestsForAllClient(), "Total open client requests equals to top value in column ");
      }

        [Test, Category("SmokeTest"), Category("IENonCompatible")]
        public void Verify_pdf_download_button_downloads_pdf_document()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var currentTime = DateTime.Now;
            string fileName = string.Format(@"DASHBOARD_LOGICSSUMMARY_{0}{1}{2}{3}{4}.pdf", currentTime.Month, currentTime.Day, currentTime.Year, currentTime.Hour, currentTime.Minute);
            string testserver = ConfigurationManager.AppSettings["TestServer"],
                testserver2 = ConfigurationManager.AppSettings["TestNode"];
            bool isRemoteTestServer = !string.IsNullOrEmpty(testserver);
            string userName = isRemoteTestServer ? ConfigurationManager.AppSettings["ProxyUserName"] : System.Environment.UserName;

            string filelocation;

            if (isRemoteTestServer)
            {
                testserver = @"\\" + testserver;
                testserver2 = @"\\" + testserver2;
            }
            //_logicRequestDetailsPage.ClickOnBoxWithArrowIcon();
            _logicRequestDetailsPage.IsSubMenuDownloadPDFPresent().ShouldBeTrue("Download to PDF submenu present");
            var url = _logicRequestDetailsPage.CurrentPageUrl;
            _logicRequestDetailsPage.ClickOnDownloadPDF();
            if (EnvironmentManager.Browser.Equals("IE", StringComparison.InvariantCulture))
            {
                fileName = string.Format(@"C:\Users\{0}\Downloads\{1}", userName, fileName);
                IEDownloadHelper.ClickSaveButtonToDownloadFile(isRemoteTestServer, testserver2, fileName);
                filelocation = isRemoteTestServer ? fileName.Substring(2) : fileName;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
            }
            else
            {
                ChromeDownLoadPage chromeDownLoad = _logicRequestDetailsPage.NavigateToChromeDownLoadPage();
                filelocation = chromeDownLoad.GetFileLocation();
                filelocation = filelocation.Replace("%20", " ");
                filelocation = isRemoteTestServer ? filelocation.Substring(2).Replace("/", @"\") : filelocation;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                _logicRequestDetailsPage = chromeDownLoad.ClickBrowserBackButton<DashboardLogicRequestsDetailsPage>(url);
            }
        }

        


       
        #endregion
    }
}
