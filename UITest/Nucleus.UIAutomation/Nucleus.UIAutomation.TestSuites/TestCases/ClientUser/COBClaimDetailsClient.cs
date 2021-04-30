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
    public class COBClaimDetailsClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private COBClaimsDetailPage _COBdashboard;
        private bool _isDashboardOpened = true;
        private string _errorMessage = string.Empty;
        private String day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;
        private MyProfilePage _myProfile;
        private COBAppealsDetailPage _cobAppealsDetail;

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
                _COBdashboard = _dashboard.ClickOnCOBClaimsDetailExpandIcon();
                try
                {
                    //_isDashboardOpened = _COBdashboard.IsDashboardPageOpened();
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
                _COBdashboard = CurrentPage.Logout()
                        .LoginAsClientUser().NavigateToCobClaimsDetailPage();
            }

            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.DashboardClaimsDetail.GetStringValue()))
            {
                _COBdashboard = QuickLaunch.NavigateToCobClaimsDetailPage();
            }
            _COBdashboard.CloseConnection();
        }

        protected override void ClassCleanUp()
        {
            base.ClassCleanUp();

        }

        #region TestSuites

        [Test] //CAR-2935(CAR-2879)
        public void Verify_Totals_By_Client_widget_In_Claims_Detail_Page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var fileName = "";
            var sheetname = paramList["SheetName"];
            var columnHeaders = paramList["ColumnNames"].Split(',').ToList();
            var totalsByClientCountFromDatabase = _COBdashboard.GetTotalsByClientDataFromDbClientUser(EnvironmentManager.Username);


            try
            {
                StringFormatter.PrintMessage("Verify Icons present in Totals By batch Widget");
                _COBdashboard.GetCOBClaimsDetailPageHeader().ShouldBeEqual(
                    PageTitleEnum.COBClaimsDetail.GetStringValue(),
                    "User should be navigated to COB Claims Detail page.");
                _COBdashboard.IsWidgetPresentInDashboard(COBClaimsDetailEnum.TotalsByClient.GetStringValue()).ShouldBeTrue("Totals by client widget present?");
                _COBdashboard.IsCollapseIconPresentBywidget(COBClaimsDetailEnum.TotalsByClient.GetStringValue())
                    .ShouldBeTrue("Collapse Icon Present in Total by client Widget");
                _COBdashboard.IsExportIconPresentBywidget(COBClaimsDetailEnum.TotalsByClient.GetStringValue())
                    .ShouldBeTrue("Export Icon Present in Total by client Widget");
                _COBdashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(COBClaimsDetailEnum.TotalsByClient.GetStringValue()).
                    ShouldBeTrue($"Is COB label present in the {COBClaimsDetailEnum.TotalsByClient.GetStringValue()}?");


                StringFormatter.PrintMessage("Verify Column headers");
                _COBdashboard.GetColumnNamesByWidgetName(COBClaimsDetailEnum.TotalsByClient.GetStringValue()).ShouldCollectionBeEqual(columnHeaders, "Column Names should match");

                StringFormatter.PrintMessage("Verify Unreviewed Claims Count and Client List");
                for (int i = 0; i < totalsByClientCountFromDatabase.Count - 1; i++)
                {
                    _COBdashboard
                        .GetClientNameByRow(COBClaimsDetailEnum.TotalsByClient.GetStringValue(), i + 1)
                        .ShouldBeEqual(totalsByClientCountFromDatabase[i][0], "Client Name should match");
                    _COBdashboard
                        .GetUnreviewedClaimsCountForEachClient(COBClaimsDetailEnum.TotalsByClient.GetStringValue(), i + 1)
                        .ShouldBeEqual(totalsByClientCountFromDatabase[i][1], "Claims count should match");
                }

                StringFormatter.PrintMessage("Verify Excel Export and Excel sheet content");
                _COBdashboard.ClickOnExportIconByWidget(COBClaimsDetailEnum.TotalsByClient.GetStringValue());
                fileName = _COBdashboard.GoToDownloadPageAndGetFileName();
                ExcelReader.ReadExcelSheetValueWithMergedCell(fileName, sheetname, 0, 0,
                    out List<string> mergedCellHeader, out List<List<string>> excelExportList);
                mergedCellHeader.ShouldCollectionBeEqual(columnHeaders, "Header list should match");
                for (int i = 2; i < excelExportList.Count; i++)
                {
                    excelExportList[i][0].ShouldBeEqual(totalsByClientCountFromDatabase[i - 2][0],
                        "Correct Client Code values should be exported");
                    excelExportList[i][1].ShouldBeEqual(totalsByClientCountFromDatabase[i - 2][1],
                        "Correct Unreviewed Claim Count value should be exported");

                }

                _COBdashboard.IsNextRefreshTimePresent().ShouldBeTrue("Is Next Refresh Time present?");

                StringFormatter.PrintMessage("Verify Collapse icon navigates back to Dashboard page");
                _dashboard = _COBdashboard.NavigateToCOBDashboard(() => _COBdashboard.ClickCollapseIconByWidgetName(COBClaimsDetailEnum.TotalsByClient.GetStringValue()));
                _dashboard.PageTitle.ShouldBeEqual(PageTitleEnum.Dashboard.GetStringValue());

            }
            finally
            {
                ExcelReader.DeleteFileIfAlreadyExists(fileName);
            }
        }

        #endregion
    }
}
