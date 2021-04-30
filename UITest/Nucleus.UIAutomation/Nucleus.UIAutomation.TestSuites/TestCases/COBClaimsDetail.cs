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

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class COBClaimsDetail :NewAutomatedBase
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
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _COBdashboard = CurrentPage.Logout()
                        .LoginAsHciAdminUser().NavigateToCobClaimsDetailPage();
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

        #region TestsSuites

        [Test]//CAR-2934(CAR-2889)
        public void Verify_Totals_By_Batch_widget_In_Claims_Detail_Page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            
            var sheetname = paramList["SheetName"];
            List<string> lastFiveDays = Enumerable.Range(0, 5)
                .Select(i => DateTime.Now.Date.AddDays(-i).ToString("ddd MM/d"))
                .ToList();
            lastFiveDays[0]="Today";
            var assignedClientList = _COBdashboard.GetCOBClientsFromDb(EnvironmentManager.Username);
            var ClaimsDataFromDB = _COBdashboard.GetDashboardDataFromDb(EnvironmentManager.Username);
            var fileName = "";

            try
            {
                StringFormatter.PrintMessage("Verify Icons present in Totals By batch Widget");
                _COBdashboard.IsWidgetPresentInDashboard(COBClaimsDetailEnum.TotalsByBatch.GetStringValue()).ShouldBeTrue("Totals by widget present?");
                _COBdashboard.IsCollapseIconPresentBywidget(COBClaimsDetailEnum.TotalsByBatch.GetStringValue())
                    .ShouldBeTrue("Collapse Icon Present in Total by batch Widget");
                _COBdashboard.IsExportIconPresentBywidget(COBClaimsDetailEnum.TotalsByBatch.GetStringValue())
                    .ShouldBeTrue("Export Icon Present in Total by Batch Widget");
                _COBdashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(COBClaimsDetailEnum.TotalsByBatch.GetStringValue()).ShouldBeTrue($"Is COB present in the {COBClaimsDetailEnum.TotalsByBatch.GetStringValue()}?");

                StringFormatter.PrintMessage("Verify correct Data Displayed in the Widget");
                _COBdashboard.GetLastFiveDays(COBClaimsDetailEnum.TotalsByBatch.GetStringValue()).ShouldCollectionBeEqual(lastFiveDays,"Days Displayed Correct?");
                _COBdashboard.GetCOBAssignedClientList(COBClaimsDetailEnum.TotalsByBatch.GetStringValue())
                    .ShouldCollectionBeEquivalent(assignedClientList, "Displayed clients");


                for (int i = 1; i <= lastFiveDays.Count; i++)
                {
                    _COBdashboard.GetClaimsCountByDay(COBClaimsDetailEnum.TotalsByBatch.GetStringValue(), i + 1)
                        .ShouldCollectionBeEqual(ClaimsDataFromDB.Select(x => x[i-1]), "Data correct?");
                }

                StringFormatter.PrintMessage("Verify Excel Export and Excel sheet content");
                _COBdashboard.ClickOnExportIconByWidget(COBClaimsDetailEnum.TotalsByBatch.GetStringValue());
                fileName = _COBdashboard.GoToDownloadPageAndGetFileName();

                ExcelReader.ReadExcelSheetValueWithMergedCell(fileName, sheetname, 0, 0,
                    out List<string> mergedCellHeader, out List<List<string>> excelExportList);
                mergedCellHeader[0].ShouldBeEqual("Client", "Client Header correct?");
                var lastFiveDaysLabelInExcel = excelExportList[1].Skip(1);
                var excelClientList = excelExportList.Select(x => x[0]).Skip(2);
                var excelClaimData = excelExportList.Skip(2);

                lastFiveDaysLabelInExcel.ShouldCollectionBeEqual(lastFiveDays, "Label correct in Excel For Last working days");
                excelClientList.ShouldCollectionBeEqual(assignedClientList, "Correct Clients Exported  in Excel? ");
                for (int i = 1; i < lastFiveDaysLabelInExcel.Count(); i++)
                {
                    excelClaimData.Select(x => x[i]).ShouldCollectionBeEqual(ClaimsDataFromDB.Select(x => x[i - 1]),
                        "Claims count correct?");
                }

                StringFormatter.PrintMessage("Verify Collapse icon navigates back to Dashboard page");
                _dashboard=_COBdashboard.NavigateToCOBDashboard(()=>_COBdashboard.ClickCollapseIconByWidgetName(COBClaimsDetailEnum.TotalsByBatch.GetStringValue()));
                _dashboard.PageTitle.ShouldBeEqual(PageTitleEnum.Dashboard.GetStringValue());

            }

            finally
            {
                ExcelReader.DeleteFileIfAlreadyExists(fileName);
            }
            

        }

        [Test] //CAR-2935(CAR-2879)
        public void Verify_Totals_By_Client_widget_In_Claims_Detail_Page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var fileName = "";
            var sheetname = paramList["SheetName"];
            var columnHeaders = paramList["ColumnNames"].Split(',').ToList();
            var totalsByClientCountFromDatabase = _COBdashboard.GetTotalsByClientDataFromDb(EnvironmentManager.Username);
           
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
                _COBdashboard.GetColumnNamesByWidgetName(COBClaimsDetailEnum.TotalsByClient.GetStringValue()).ShouldCollectionBeEqual(columnHeaders,"Column Names should match");

                StringFormatter.PrintMessage("Verify Unreviewed Claims Count and Client List");
                for (int i = 0; i < totalsByClientCountFromDatabase.Count-1; i++)
                {
                    _COBdashboard
                        .GetClientNameByRow(COBClaimsDetailEnum.TotalsByClient.GetStringValue(), i + 1)
                        .ShouldBeEqual(totalsByClientCountFromDatabase[i][0], "Client Name should match");
                    _COBdashboard
                        .GetUnreviewedClaimsCountForEachClient(COBClaimsDetailEnum.TotalsByClient.GetStringValue(),i+1)
                        .ShouldBeEqual(totalsByClientCountFromDatabase[i][1],"Claims count should match");
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

        [Test] //CAR-3296(CAR-3251)
        public void Verify_total_paid_value_in_totals_by_flag()
        {
            var userId = EnvironmentManager.HciAdminUsername;
            var clientsList = _COBdashboard.GetCOBClientsFromDb(userId);
            
            _COBdashboard.GetColumnNamesByWidgetName(COBClaimsDetailEnum.TotalsByFlag.GetStringValue())[3]
                .ShouldBeEqual("Total Paid", "Total Paid column should be present");

            StringFormatter.PrintMessage("Verifying total paid per client");
            _COBdashboard.GetTotalPaidClaimsDetail().ShouldCollectionBeEqual(_COBdashboard.GetTotalPaidForCOBDashboardTotalsByFlagDb(userId), "Total Paid should match with db");

            StringFormatter.PrintMessage("Verifying if there is at least one non-deleted COB flag.");
            foreach (var client in clientsList)
            {
                _COBdashboard.IsAtLeastOneNonDeletedCOBFlagPresent(client).ShouldBeTrue($"Is at least one non-deleted flag present for {client}");
            }

            StringFormatter.PrintMessage("Verifying total paid per client where there is at least one non-deleted COB flag");
            for (int i = 0; i < clientsList.Count; i++)
            {
                _COBdashboard.GetTotalOfTotalPaidClaimsDetail()[i].ShouldBeEqual(
                    _COBdashboard.GetTotalPaidPerClientDb(clientsList[i],userId),
                    $"Total of Total paid for {clientsList[i]} should match.");
            }
        }

        #endregion

    }
}
