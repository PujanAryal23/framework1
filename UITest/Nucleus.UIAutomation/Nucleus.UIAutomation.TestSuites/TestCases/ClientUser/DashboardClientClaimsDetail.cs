using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Configuration;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.PageServices.ChromeDownLoad;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    class DashboardClientClaimsDetail:AutomatedBaseClient
    {


        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        //private String day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _expectedClaimByClientList;

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
               _claimsDetailPage= _dashboard.ClickOnClaimsDetailExpandIcon();
                try
                {
                    _isDashboardOpened = _dashboard.IsDashboardPageOpened();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                }
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
            if (!CurrentPage.CurrentPageTitle.Equals(PageTitleEnum.DashboardClaimsDetail.GetStringValue()))
            {
                CurrentPage =
                    _dashboard = QuickLaunch.NavigateToDashboard();
            }
            base.TestCleanUp();

        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _claimsDetailPage.GetAllClientExpectedLists(EnvironmentManager.ClientUserName);
            _expectedClaimByClientList = _allCotivitiExpectedLists[0];
        }

        #endregion

        #region TEST SUITES
        //US33630 contains all test

        [Test]
        public void Verify_when_the_user_hovers_over_data_points_the_corresponding_text_is_displayed_in_a_browser_tooltip_in_claim_detail_page()
        {
            _claimsDetailPage.GetUnreviewedClaimsLabelTooltipClientDashboardDetail().ShouldBeEqual("Total number of CV claims in Client Unreviewed status.", "Unreviewed Claims Tooltip");
            _claimsDetailPage.Get10DaysOldClaimsLabelTooltipClientDashboardDetail()
                .ShouldBeEqual("Total number of Client Unreviewed CV claims that were received more than 10 business days ago.",
                    ">10 Days Unreviewed Claims Label Tooltip");
            _claimsDetailPage.GetPendedClaimLabelTooltipClientDashboardDetail()
                  .ShouldBeEqual("Total number of pended CV claims.", "Pended CLaims Tooltip");
            _claimsDetailPage.Get10DaysOldPendedClaimsLabelTooltipClientDashboardDetail().
                ShouldBeEqual("Total number of pended CV claims received more than 10 business days ago.", ">10 Days Pended Claims Label Tooltip");
            _claimsDetailPage.GetUnapprovedClaimsLabelTooltipClientDashboardDetail().
                ShouldBeEqual("Total number of CV claims that have been reviewed but are still in Client Unreviewed status", "Unapproved Claims Tooltip");
            _claimsDetailPage.GetApprovedYesterdayClaimsLabelTooltipClientDashboardDetail().
                ShouldBeEqual("Total number of CV claims approved on the previous business day.", "Approved Yesterday Claims Label Tooltip");
            _claimsDetailPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Last updated time appears in upper right corner");


        }




        [Test]
        public void Verify_clicking_on_Download_PDF_button_downloads_pdf_document()
        {
           TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var currentTime = DateTime.Now;
            string fileName = string.Format(@"DASHBOARD_CLAIMSSUMMARY_{0}{1}{2}{3}{4}.pdf", currentTime.Month, currentTime.Day, currentTime.Year, currentTime.Hour, currentTime.Minute);
            string testserver = ConfigurationManager.AppSettings["TestServer"],
                testserver2 = ConfigurationManager.AppSettings["TestNode"];
            bool isRemoteTestServer = !string.IsNullOrEmpty(testserver);
            string userName = isRemoteTestServer ? ConfigurationManager.AppSettings["ProxyUserName"] : System.Environment.UserName;

            string filelocation;
            if (isRemoteTestServer)
            {
                //testserver = @"\\" + testserver;
                testserver2 = @"\\" + testserver2;
            }
            
            _claimsDetailPage.IsSubMenuDownloadPDFPresent().ShouldBeTrue("Download to PDF submenu present");
            var url = _claimsDetailPage.CurrentPageUrl;
            _claimsDetailPage.ClickOnDownloadPDF();
            if (EnvironmentManager.Browser.Equals("IE", StringComparison.InvariantCulture))
            {
                fileName = string.Format(@"C:\Users\{0}\Downloads\{1}", userName, fileName);
                IEDownloadHelper.ClickSaveButtonToDownloadFile(isRemoteTestServer, testserver2, fileName);
                filelocation = isRemoteTestServer ? fileName.Substring(2) : fileName;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
            }
            else
            {
                ChromeDownLoadPage chromeDownLoad = _claimsDetailPage.NavigateToChromeDownLoadPage();
                filelocation = chromeDownLoad.GetFileLocation();
                filelocation = filelocation.Replace("%20", " ");
                filelocation = isRemoteTestServer ? filelocation.Substring(2).Replace("/", @"\") : filelocation;
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
                _claimsDetailPage = chromeDownLoad.ClickBrowserBackButton<ClaimsDetailPage>(url);
            }
        }



        [Test]
        public void Verify_User_can_see_authorized_clients()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
            //    TestExtensions.TestName);
            //string expectedClaimByClients = paramLists["Clients"];
           
            _claimsDetailPage.GetClaimByClientsList()
                .ShouldCollectionBeEqual(_expectedClaimByClientList, "Authorized Claims by Active Client with sorted order");

            //foreach (var actualClientList in actualClaimByClientList)
            //{
            //    expectedClaimByClientList.Contains(actualClientList).ShouldBeTrue("Authorized Client " + actualClientList + " Appears in list?");
            //}

        }



        [Test]
        public void Verify_that_Claim_Detail_has_no_null_claim_Value()
        {
           _claimsDetailPage.IsClaimDetailsValueNull().ShouldBeFalse("Claims Detail values have null column?");
           _claimsDetailPage.IsClaimDetailsHeaderValueNull().ShouldBeFalse("Claims Detail Header values have null column?");
        }

        [Test]
        public void Verify_header_and_column_headers_in_dashboard_claims_detail_and_verify_client_values_sum_to_top_row_total_value()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedFirstRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "1st_row_column_headers").Values.ToList();
            IList<string> expectedSecondRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "2nd_row_column_headers").Values.ToList();
            _claimsDetailPage.GetClaimsDetailHeader().ShouldBeEqual("Claims by Client", "Claims Detail Header");

            _claimsDetailPage.GetFirstRowColumnHeaders().ShouldCollectionBeEqual(expectedFirstRowColumnHeadersList, "1st row column headers are equal:");
            _claimsDetailPage.GetSecondRowColumnHeaders().ShouldCollectionBeEqual(expectedSecondRowColumnHeadersList, "2nd row column headers are equal:");

            StringFormatter.PrintMessageTitle("Verifying client values sum to top row total values");
            for (int i = 1; i <= _claimsDetailPage.GetFirstRowColumnHeaderCount(); i++)
                _claimsDetailPage.GetHeaderValueForClientDashboard(i).ShouldBeEqual(_claimsDetailPage.GetEachColumnSumValue(i, _claimsDetailPage.GetClaimsDetailHeader()), "Total sum equals to top value in column " + i);
        }

        #endregion
    }
}
