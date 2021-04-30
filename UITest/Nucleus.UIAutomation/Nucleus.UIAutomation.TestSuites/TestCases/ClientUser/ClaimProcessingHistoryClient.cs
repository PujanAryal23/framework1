using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
   public  class ClaimProcessingHistoryClient: AutomatedBaseClient
    {
        private ClaimActionPage _claimAction;
        private ClaimProcessingHistoryPage _claimProcessingHistory;
        private ClaimSearchPage _claimSearchPage;
        private ClientSearchPage _clientSearch;

        #region Override

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _claimSearchPage = QuickLaunch.NavigateToClaimSearch();


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
                _claimSearchPage = _claimAction
                    .Logout()
                    .LoginAsClientUser().NavigateToClaimSearch();

            }
            else if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
            {
                CheckTestClientAndSwitch();
                _claimSearchPage = QuickLaunch.NavigateToClaimSearch();
            }

            else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimSearch.GetStringValue()))
            {
                CurrentPage = _claimSearchPage = CurrentPage.NavigateToClaimSearch();


            }

            else if (_claimSearchPage.IsPageErrorPopupModalPresent())
                CurrentPage.ClosePageError();


        }

        protected override void ClassCleanUp()
        {
            try
            {
                if (_claimProcessingHistory != null && _claimAction.IsClaimProcessingHistoryOpen())
                {
                    _claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimSearchPage();
                }
            }

            finally
            {
                base.ClassCleanUp();
            }
        }


        #endregion

        #region TestCase

        [Test]//TE-642
        public void Verify_Correct_Data_Displayed_In_Claim_Processing_History_Client()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var processingTypeheader = DataHelper.GetMappingData(FullyQualifiedClassName, "Processing_History")
                    .Values.ToList();
                var claimHistoryHeader =
                    DataHelper.GetMappingData(FullyQualifiedClassName, "Claim_History").Values.ToList();
                _claimSearchPage.DeleteClaimAuditOnly(ClientEnum.SMTST.ToString(), claimSeq, "24-MAy-2019");
                _claimAction =
                    _claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                _claimAction.Refresh();
                _claimProcessingHistory = _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                _claimProcessingHistory.GetClaimHistoryLabel()
                    .ShouldBeEqual("Claim History", "claim history label correct?");
                _claimProcessingHistory.GetProcessingHistoryLabel()
                    .ShouldBeEqual("Processing History", "Processing History label present?");
                _claimProcessingHistory.GetProcessingHistoryHeader()
                    .ShouldCollectionBeEqual(processingTypeheader, "processing history header equal?");
                _claimProcessingHistory.GetClaimHistoryHeader()
                    .ShouldCollectionBeEqual(claimHistoryHeader, "Claim History header equal?");

                StringFormatter.PrintMessage("Verify processing history data present");
                _claimProcessingHistory
                    .GetProcessingDataForClaim(claimSeq.Split('-')[0], claimSeq.Split('-')[1])
                    .ShouldCollectionBeEqual(_claimProcessingHistory.GetProcessingHistoryData(),
                        "Processing History data equal?");

                StringFormatter.PrintMessage("Verify Claim History data present");
                var resultfromDB = _claimProcessingHistory
                    .GetClaimHistoryDataForClaim(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],true);
                _claimProcessingHistory.GetClaimHistoryRowCount()
                    .ShouldBeEqual(resultfromDB.Count, "Count should be equal.");
                var count = _claimProcessingHistory.GetClaimHistoryHeader().Count;
                for (int i = 0; i < count; i++)
                {
                    _claimProcessingHistory.GetClaimHistoryData(i + 1).ShouldCollectionBeEqual(
                        resultfromDB.Select(x => x[i]).ToList(), $"Is {claimHistoryHeader[i]} audit equal?");
                }
            }
            finally
            {
                CurrentPage.CloseAnyPopupIfExist();
                if (CurrentPage.IsPageErrorPopupModalPresent())
                    CurrentPage.ClosePageError();
                _claimSearchPage = CurrentPage.NavigateToClaimSearch();

            }
        }

        [Test, Category("OnDemand")]//TE-642
        public void Verify_Processing_History_Not_Displayed_For_Realtime_Client_Client_User()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var _claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence", "Value");
            var claimHistoryHeader =
                DataHelper.GetMappingData(FullyQualifiedClassName, "Claim_History").Values.ToList();
            try
            {
                _clientSearch=QuickLaunch.Logout().LoginAsHciAdminUser1().NavigateToClientSearch();
                _clientSearch.ChangeProcessingTypeOfClient(ClientEnum.SMTST.ToString(),"Real-Time");
                _claimSearchPage = _clientSearch.Logout().LoginAsClientUser().NavigateToClaimSearch();
                _claimSearchPage.DeleteClaimAuditOnly(ClientEnum.SMTST.ToString(),_claimSequence, "28-MAY-2019");
                _claimAction =
                    _claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                if (_claimAction.IsPageErrorPopupModalPresent())
                    _claimAction.ClosePageError();
                _claimProcessingHistory =
                    _claimAction.ClickOnClaimHistoryButtonAndSwitchToClaimProcessingHistoryPage();
                _claimProcessingHistory.IsProcessingHistorySectionPresent()
                    .ShouldBeFalse("processing type section should not be displayed for Real time clients");
                _claimProcessingHistory.IsClaimHistorySectionPresent()
                    .ShouldBeTrue("only claim history should be present");
                var resultfromDB = _claimProcessingHistory
                    .GetClaimHistoryDataForClaim(ClientEnum.SMTST.ToString(), _claimSequence.Split('-')[0],
                        _claimSequence.Split('-')[1],true);
                _claimProcessingHistory.GetClaimHistoryRowCount()
                    .ShouldBeEqual(resultfromDB.Count, "Count should be equal.");
                var count = _claimProcessingHistory.GetClaimHistoryHeader().Count;
                for (int i = 0; i < count; i++)
                {
                    _claimProcessingHistory.GetClaimHistoryData(i + 1).ShouldCollectionBeEqual(
                        resultfromDB.Select(x => x[i]).ToList(), $"Is {claimHistoryHeader[i]} audit equal?");
                }

            }
            finally
            {
                CurrentPage.CloseAnyPopupIfExist();
                if (CurrentPage.IsPageErrorPopupModalPresent())
                    CurrentPage.ClosePageError();
                _clientSearch = CurrentPage.Logout().LoginAsHciAdminUser1().NavigateToClientSearch();
                _clientSearch.ChangeProcessingTypeOfClient(ClientEnum.SMTST.ToString(),"Batch");
                _claimSearchPage = CurrentPage.Logout().LoginAsClientUser().NavigateToClaimSearch();

            }



        }

        [Test]//CAR-3259(CAR-3192)
        public void Verify_Review_Time_In_Claim_Processing_History()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(GetType().FullName, TestExtensions.TestName);
            var _claimSequence = paramLists["ClaimSequence"].Split(',').ToList();
            var status = paramLists["status"];

            try
            {
                StringFormatter.PrintMessageTitle("Opening Claim Action page to perform Approve/Transfer and verifying the review time");
                for (int i = 0; i < _claimSequence.Count; i++)
                {
                    _claimSearchPage.GetSideBarPanelSearch.OpenSidebarPanel();

                    _claimAction =
                        _claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence[i], false);
                    _claimAction.WaitforReviewTime();

                    if (i == 0)
                        _claimAction.ClickOnApproveButton();

                    else
                    {
                        _claimAction.ClickOnTransferButton();
                        _claimAction.SelectStatusCode(status);
                        _claimAction.ClickOnSaveButton();
                    }

                    _claimAction =
                        _claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence[i], false);
                    _claimProcessingHistory = _claimAction.ClickOnClaimProcessingHistoryAndSwitch();

                    TimeSpan interval;
                    TimeSpan.TryParseExact(_claimProcessingHistory.GetReviewTime(status), @"mm\:ss", null,
                        out interval);

                    interval.TotalSeconds.ShouldBeGreaterOrEqual(5, "review time recorded?");
                    _claimAction = _claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();
                }

                _claimSearchPage = _claimAction.ClickClaimSearchIcon();
                _claimSearchPage.GetSideBarPanelSearch.ClickOnFilterIcon();
                _claimSearchPage.GetSideBarPanelSearch.ClickOnClearLink();
            }
            finally
            {
                StringFormatter.PrintMessageTitle("Reverting the claim status to the original state");
                _claimSequence.ForEach(claSeq => _claimSearchPage.UpdateClaimStatus(claSeq, "SMTST", "U", 'T'));
                _claimSequence.ForEach(claSeq => _claimSearchPage.DeleteClaimAuditOnly("SMTST", claSeq, "2-FEB-2021"));
            }
        }




        #endregion

        #region Private Methods
        void SearchByClaimSeqFromWorkList(string claimSeq)
        {
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.ClickSearchIcon()
                .SearchByClaimSequence(claimSeq);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //_claimAction.ClickWorkListIcon();
            _claimProcessingHistory = _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
        }

        void HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(object obj)
        {
            if (obj is ClaimActionPage)
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }
        }

#endregion

    }
}
