using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class COBDashboardClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private String day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;
        private MyProfilePage _myProfile;
        private UserProfileSearchPage _userProfileSearch;
        private COBAppealsDetailPage _cobAppealsDetail;
        private COBClaimsDetailPage _cobClaimsDetail;

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
                _dashboard = QuickLaunch.NavigateToCOBDashboard();
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
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _dashboard = CurrentPage.Logout()
                        .LoginAsClientUser().NavigateToCOBDashboard();
            }

            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.Dashboard.GetStringValue()))
            {
                CurrentPage =
                    _dashboard = QuickLaunch.NavigateToCOBDashboard();
            }
            _dashboard.CloseConnection();
        }

        protected override void ClassCleanUp()
        {
            base.ClassCleanUp();

        }

        #region TestSuites
        [Test] //CAR-2918(CAR-2881)
        public void Verify_Claims_Overview_section_is_present_and_last_updated_time_is_Displayed()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

            _dashboard.IsContainerHeaderClaimsOverviewPresent().ShouldBeTrue("Container header Claims Overview present");
            _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue()).ShouldBeTrue("Container header has COB in right corner");
            _dashboard.IsNextRefreshTimePresent().ShouldBeTrue("Last updated time appears in upper right corner of the page");


        }
        [Test] //CAR-2918(CAR-2881)
        public void Verify_Claims_count_In_OverView_Widget()
        {
            var expectedLabelsList =
                DataHelper.GetMappingData(FullyQualifiedClassName, "1st_row_column_headers").Values.ToList();
            var expectedToolTipList =
                DataHelper.GetMappingData(FullyQualifiedClassName, "1st_row_column_ToolTip").Values.ToList();

            _dashboard.GetClaimOverViewWidgetLabelForCOBByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue()).ShouldCollectionBeEquivalent(expectedLabelsList, "Labels displayed correct?");
            _dashboard.GetClaimOverViewWidgetToolTipForCOBByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue()).ShouldCollectionBeEquivalent(expectedToolTipList, "Tooltips displayed?");
            _dashboard.GetCOBWidgetCountData("Unreviewed Claims").ShouldBeEqual(_dashboard.GetCOBUnreviewedClaimsDataForClientFromDB(EnvironmentManager.Username),
                "unreviewed claim count correct?");
            _dashboard.GetCOBWidgetCountData("Unreviewed Members").ShouldBeEqual(_dashboard.GetCOBUnreviewedClaimsDataForClientFromDB(EnvironmentManager.Username),
                "unreviewed members count correct?");

            _dashboard.IsExpandElementPresentInOverViewWidgetByWidgetTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue()).ShouldBeTrue("Expand icon present?");

        }

        [Test, Category("SmokeTestDeployment")] //TANT-263
        public void Verify_data_points_of_COB_Claims_Overview_Widget_Client()
        {
            var unreviewedClaims = Convert.ToInt32(_dashboard.GetCOBWidgetCountData("Unreviewed Claims"));
            var unreviewedMembers = Convert.ToInt32(_dashboard.GetCOBWidgetCountData("Unreviewed Members"));

            StringFormatter.PrintMessage("Verify Unreviewed Claims Count");
            unreviewedClaims.ShouldBeGreaterOrEqual(0, "Total COB Unreviewed Claims count should not be empty");

            StringFormatter.PrintMessage("Verification of Unreviewed Members Count");
            unreviewedMembers.ShouldBeGreaterOrEqual(0, "Total COB Unreviewed Members count should not be empty");

            StringFormatter.PrintMessageTitle("Verification of expanded COB Dashboard - Claims Detail");
            _cobClaimsDetail = _dashboard.ClickOnCOBClaimsDetailExpandIcon();
            var claimDetailHeaders =
                _cobClaimsDetail.GetColumnNamesByWidgetName(COBClaimsDetailEnum.TotalsByFlag.GetStringValue());
            claimDetailHeaders.RemoveAt(0);
            claimDetailHeaders.RemoveAt(2);

            StringFormatter.PrintMessage("Verification of Totals by Flag section");
            _cobClaimsDetail.IsTotalClaimsAndMemberCountValidByWidgetNameAndCol(COBClaimsDetailEnum.TotalsByFlag.GetStringValue(), 2)
                .ShouldBeTrue("Unreviewed Claims column in Totals by flag should show a value of either 0 or higher");

            _cobClaimsDetail.IsTotalClaimsAndMemberCountValidByWidgetNameAndCol(COBClaimsDetailEnum.TotalsByFlag.GetStringValue(), 3)
                .ShouldBeTrue("Unreviewed Members column in Totals by flag should show a value of either 0 or higher");

            StringFormatter.PrintMessage("Verification of Totals by Batch section");
            var clients =
                _cobClaimsDetail.GetCobClaimsDetailDataByWidgetAndCol(
                    COBClaimsDetailEnum.TotalsByBatch.GetStringValue(), 1);
            foreach (var client in clients)
            {
                _cobClaimsDetail.IsTotalClaimCountByWidgetNameAndClientGreaterOrEqualsZero(COBClaimsDetailEnum.TotalsByBatch.GetStringValue(), client)
                    .ShouldBeTrue("Unreviewed Claims column in Totals by Client should show a value of either 0 or higher");
            }

            StringFormatter.PrintMessage("Verification of Totals by Client section");
            _cobClaimsDetail.IsTotalClaimsAndMemberCountValidByWidgetNameAndCol(COBClaimsDetailEnum.TotalsByClient.GetStringValue(), 2)
                .ShouldBeTrue("Unreviewed Claims column in Totals by Client should show a value of either 0 or higher");

            _dashboard.ClickCollapseIcon();

            _dashboard.GetClaimOverViewWidgetLabelForCOBByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview
                    .GetStringValue()).ShouldBeEqual(claimDetailHeaders, "Headers should match after collapsing the detail view");
        }

        [Test, Category("SmokeTestDeployment")] //TANT-264
        public void Verify_data_points_of_COB_Appeals_Overview_Widget_Client()
        {
            StringFormatter.PrintMessage("Verify Outstanding Appeals Count");
            _dashboard.GetCOBWidgetCountData(COBAppealsDetailEnum.OutstandingAppeals.GetStringValue())
                .ShouldBeGreaterOrEqual(0, "Total COB Unreviewed Claims count should not be empty");

            StringFormatter.PrintMessageTitle("Verification of expanded COB Appeals Detail section");
            _dashboard.ClickOnAppealsOverviewExpandIcon();

            var appealsDetailHeaders = _dashboard.GetColumnHeaderListFromCOBWidgetDetailPage();
            appealsDetailHeaders.RemoveAt(0);

            int.Parse(_dashboard.GetAppealsDetailTotalValueByCol(2))
                .ShouldBeGreaterOrEqual(0, "Total Outstanding Appeals cannot be empty");
    

            StringFormatter.PrintMessage("Verifying collapsing the details COB Appeals Detail section");
            _dashboard.ClickCollapseIcon();
            _dashboard.GetClaimOverViewWidgetLabelForCOBByComponentTitle(DashboardOverviewTitlesEnum.AppealsOverview
                .GetStringValue()).ShouldBeEqual(appealsDetailHeaders, "Headers should match after collapsing");
        }

        [Test, Category("OnDemand")] //CAR-2918(CAR-2881)
        public void Verify_User_Behaviour_when_Default_Dashboard_Is_COB()
        {
            try
            {
                StringFormatter.PrintMessage("Verify user is should be navigated to COb Dashboard when default Dashboard is set to COB");
                _myProfile = _dashboard.NavigateToMyProfilePage();
                _myProfile.SelectDropDownListValueByLabelOption(UserPreferencesEnum.DefaultDashboard.GetStringValue(), ProductEnum.COB.GetStringValue());
                _myProfile.ClickOnSaveButton();
                _myProfile.ClickOnDashboardIcon();
                _dashboard.GetDisplayedProductDashboard()
                    .ShouldBeEqual(ProductEnum.COB.ToString(), "COB Dashboard Displayed?")
                    ;
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue()).ShouldBeTrue("Container header has COB in right corner");
            }
            finally
            {
                _myProfile = _dashboard.NavigateToMyProfilePage();
                _myProfile.SelectDropDownListValueByLabelOption(UserPreferencesEnum.DefaultDashboard.GetStringValue(), ProductEnum.CV.GetStringValue());
                _myProfile.ClickOnSaveButton();
                _myProfile.NavigateToCOBDashboard();

            }

        }

        [Test] //CAR-2936 (CAR-2920)
        public void Verify_COB_Appeals_overview_and_detail_view_client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var tableName = testData["TableName"];
            var columnName = testData["ColumnName"];
            var appealsDetailContainerHeaderTitle = testData["AppealsDetailContainerHeaderTitle"];
            var expectedAppealOverviewList = COBAppealsDetailEnum.OutstandingAppeals.GetStringValue();
            var expectedToolTipList = testData["ExpectedToolTipList"];
            var appealDetailsColumn = testData["AppealDetailsColumns"].Split(',').ToList();
            var sheetname = testData["SheetName"];
            var fileName = "";
            var expectedDataList = _dashboard.GetExcelDataListForClientUsers(EnvironmentManager.Username);
            

            try
            {
                StringFormatter.PrintMessage("Verification of labels");
                _dashboard.IsAppealsOverviewHeaderPresent().ShouldBeTrue("Is Appeal Overview present?");
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum.AppealsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB shown to the right of the widget header?");
                _dashboard.GetClaimOverViewWidgetLabelForCOBByComponentTitle(DashboardOverviewTitlesEnum.AppealsOverview
                        .GetStringValue())[0].ShouldBeEqual(expectedAppealOverviewList,
                        "Is Outstanding Appeals label displayed?");

                StringFormatter.PrintMessage("Tooltip verification");
                _dashboard.GetClaimOverViewWidgetToolTipForCOBByComponentTitle(DashboardOverviewTitlesEnum.AppealsOverview
                        .GetStringValue())[0].ShouldBeEqual(expectedToolTipList, "Does tooltip match?");

                StringFormatter.PrintMessage("Verification of Appeals count");
                _dashboard.GetCOBWidgetCountData(COBAppealsDetailEnum.OutstandingAppeals.GetStringValue())
                    .ShouldBeEqual(_dashboard.GetCOBWidgetCountDataFromDB(columnName, tableName,
                        EnvironmentManager.Username));

                StringFormatter.PrintMessage("Verification of expand icon");
                _dashboard
                    .IsExpandElementPresentInOverViewWidgetByWidgetTitle(DashboardOverviewTitlesEnum.AppealsOverview
                        .GetStringValue()).ShouldBeTrue("Is expand icon present in Appeals Overview widget?");

                StringFormatter.PrintMessage("Verification of COB Appeal detail page");
                _cobAppealsDetail = _dashboard.ClickOnAppealsOverviewExpandIcon();
                _cobAppealsDetail.GetCOBAppealsDetailPageHeader().ShouldBeEqual(
                    PageTitleEnum.COBAppealsDetail.GetStringValue(),
                    "User should be navigated to COB Appeals Detail page.");
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(appealsDetailContainerHeaderTitle)
                    .ShouldBeTrue("Is COB present in Appeals by Client widget");
                _dashboard.GetColumnHeaderListFromCOBWidgetDetailPage()
                    .ShouldCollectionBeEqual(appealDetailsColumn, "Appeals Detail column should match");
                _dashboard.IsDownloadIconPresentInWidget()
                    .ShouldBeTrue("Is Download icon present in Appeals Detail widget?");
                _dashboard.IsCollapseIconPresent().ShouldBeTrue("Is Collapse icon present?");
                _dashboard.ClickDownloadIconInWidget();

                StringFormatter.PrintMessage("Verification of Excel data");
                fileName = _cobAppealsDetail.GoToDownloadPageAndGetFileName();
                _dashboard.WaitForWorking();
                _dashboard.WaitForSpinner();

                ExcelReader.ReadExcelSheetValueWithMergedCell(fileName, sheetname, 0, 0,
                    out List<string> mergedCellHeader, out List<List<string>> excelExportList);
                mergedCellHeader.ShouldCollectionBeEqual(appealDetailsColumn, "Header list should match");
                for (int i = 2; i < excelExportList.Count-1; i++)
                {
                    excelExportList[i][0].ShouldBeEqual(expectedDataList[i - 2][0],
                        "Correct Claim Sequence values should be exported");
                    excelExportList[i][1].ShouldBeEqual(expectedDataList[i - 2][1],
                        "Correct Claim Number values should be exported");
                }

                excelExportList[excelExportList.Count - 1][1].ShouldBeEqual(
                    _dashboard.GetAppealsDetailTotalValueByCol(2), "Total count should match");

                StringFormatter.PrintMessage("Verification of COB Appeals detail UI data against Db");
                for (int i = 0; i < expectedDataList.Count-1; i++)
                {
                    _dashboard.GetAppealsDetailValueListByCol(i + 1)
                        .ShouldCollectionBeEqual(_dashboard.GetAppealDetailValueForClientUserFromDbByCol(EnvironmentManager.Username, i), "Appeal detail values- Clients list and Outstanding Appeals should match");
                }

                Convert.ToInt16(_dashboard.GetAppealsDetailTotalValueByCol(2)).ShouldBeEqual(_dashboard.GetCOBWidgetCountDataFromDB(
                        columnName, tableName, EnvironmentManager.Username), "Total Outstanding Appeals count should match");
                _dashboard.IsNextRefreshTimePresent().ShouldBeTrue("Is Next Refresh Time present?");
            }

            finally
            {
                _dashboard.ClickCollapseIcon();
                _dashboard.GetPageHeader().ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(),
                    "Clicking collapse icon should navigate the user to Dashboard page");
                ExcelReader.DeleteFileIfAlreadyExists(fileName);
            }


        }
        #endregion

    }
}
