using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class PatientClaimHistoryClient: AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private ClaimActionPage _claimAction;
        private ClaimSearchPage _claimSearch;
        private ClientSearchPage _clientSearch;

        private string _claimSequence = string.Empty;
        private ClaimHistoryPage _claimHistory;
        private NewPopupCodePage _newPopupCode;
        private FlagPopupPage _flagPoup;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                SearchByClaimSeqFromWorkList(_claimSequence);
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
            _claimAction.CloseAnyTabIfExist();
            if (_claimAction.IsPageErrorPopupModalPresent())
                _claimAction.ClosePageError();

            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _claimAction = _claimAction.Logout()
                    .LoginAsClientUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient).NavigateToCVClaimsWorkList();
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
            }
            SearchByClaimSeqFromWorkList(_claimSequence);
        }

        protected override void ClassCleanUp()
        {
            try
            {
                if (_claimAction.IsClaimLocked())
                    Console.WriteLine("Claim is Locked!");
                if (_claimHistory != null && !_claimAction.IsProviderHistoryClose())
                {
                    _claimHistory.SwitchToNewClaimActionPage(true);
                }
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        #endregion

        #region TEST SUITES

        [Test] // CAR-3043(CAR-3014)
        public void Verify_large_data_load_in_patient_claim_history_client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var claims = paramList["Claims"].Split(',').ToList();
            var patSeq = paramList["PatSeq"];
            var prvseq = paramList["PrvSeq"];
            var claimsFromDb = _claimHistory.GetClaimSeqListForHugeDataLoadFromDb(patSeq, prvseq);
            var claimCount = claimsFromDb.Count;
            claimCount.ShouldBeGreater(1000, "Is count greater than 1000?");
            var newerClaSeqListFromDb = claimsFromDb.Take(claimsFromDb.IndexOf(claims[1])).ToList();
            claimsFromDb.RemoveRange(0, claimsFromDb.IndexOf(claims[1]) + 1);
           
            try
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.LOADT.ToString(), claims[0].Split('-')[0],
                    claims[1].Split('-')[0]);
                _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

                StringFormatter.PrintMessage("Verification of huge data load section");
                _claimHistory.IsHugeDataMessageSectionPresent().ShouldBeTrue("Is huge data load message present?");
                _claimHistory.GetHugeDataLoadMessage().ShouldBeEqual(
                    $"Patient history has {claimCount} claims. This view has been limited to improve performance.",
                    "Huge data load message should match");
                _claimHistory.CloseHugeDataLoadSection();
                _claimHistory.IsHugeDataMessageSectionPresent()
                    .ShouldBeFalse("Is huge data load message section present?");

                StringFormatter.PrintMessage("Verification newer claims will be shown in patient history");
                var claimSeqList = _claimHistory.GetDisplayedClaimSeqListFromClaimHistory();
                var newerClaseqList = claimSeqList.Take(claimSeqList.IndexOf(claims[1])).ToList();
                newerClaseqList.ShouldCollectionBeEquivalent(newerClaSeqListFromDb, "Newer claims should be shown");

                StringFormatter.PrintMessage("Verification of DOS of immediate newer and immediate older claims");
                _claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claims[1]) - 1]).
                    ShouldBeGreaterOrEqual(_claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claims[1])]),
                        "Is Dos of newer claim less than current claim?");
                _claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claims[1]) + 1]).
                    ShouldBeLessOrEqual(_claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claims[1])]),
                        "Is Dos of older claim less than current claim?");

                StringFormatter.PrintMessage("Verification that max of 1000 claims will be shown before current claim");
                claimSeqList.RemoveRange(0, claimSeqList.IndexOf(claims[1]) + 1);
                claimSeqList.Count.ShouldBeEqual(1000, "A max of 1000 claims will be shown before the current claim");
                claimSeqList.ShouldCollectionBeEquivalent(claimsFromDb.Take(1000).ToList(),
                    "Max of 1000 older claims should be shown before current claim");
            }

            finally
            {
                _claimAction.CloseAllExistingPopupIfExist();
                _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.SMTST.ToString(), claims[1].Split('-')[0],
                    claims[0].Split('-')[0], ClientEnum.LOADT.ToString());
            }

        }

        [Test] //CAR-2950 [CAR-3035]
        public void Verify_8_month_filter_option_for_client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var claims = paramList["Claims"].Split(';').ToList();

            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

            StringFormatter.PrintMessageTitle("Verifying the '8 Months' filter for claims with one, two and three claim lines");
            foreach (var claimSeq in claims)
            {
                SearchByClaimSeqFromWorkList(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickClaimLineIconInClaimLines();

                var allDos = _claimAction.GetAllDosValues().Select(d => DateTime.Parse(d)).ToList();
                var minAndMaxDos = allDos.Count == 1 ? allDos : GetMinMaxDateOfService(allDos);

                StringFormatter.PrintMessageTitle("Verifying the Patient Claim Hx for '8 Months' filter");
                _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                _claimHistory.Is8MonthsFilterPresent().ShouldBeTrue("'8 Months' filter option should be present");
                _claimHistory.Click8MonthsFilter();

                var allDosFromPatientHx = _claimHistory.GetAllDosFromPatientClaimHx().Distinct();
                bool isDosWithinRange = true;
                var startDate = minAndMaxDos[0].AddMonths(-4);
                var endDate = allDos.Count == 1 ? minAndMaxDos[0].AddMonths(4) : minAndMaxDos[1].AddMonths(4);

                foreach (var dosFromPatientHx in allDosFromPatientHx)
                {
                    isDosWithinRange = isDosWithinRange
                                       &&
                                       startDate <= DateTime.Parse(dosFromPatientHx)
                                       &&
                                       endDate >= DateTime.Parse(dosFromPatientHx);
                }

                isDosWithinRange
                    .ShouldBeTrue("History records will be shown for claims whose DOS is between 4 months prior to the earliest DOS and 4 months after the oldest EDOS on the current claim");

                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }

            #region Local Method

            List<DateTime> GetMinMaxDateOfService(List<DateTime> dateStrings)
            {
                DateTime minDate = new DateTime(9999, 12, 31);
                DateTime maxDate = new DateTime(1000, 01, 01);

                foreach (var dateInString in dateStrings)
                {
                    var dateInDatetime = dateInString;
                    if (dateInDatetime < minDate)
                        minDate = dateInDatetime;

                    if (dateInDatetime > maxDate)
                        maxDate = dateInDatetime;
                }

                return new List<DateTime> { minDate, maxDate };
            }

            #endregion
        }

        [Test] // CAR-3037([CAR-2946])
        public void Verify_Line_Filters_In_Patient_Claim_History_Page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var claimSeq = paramLists["ClaimSequence"];
            var placeHolders = paramLists["PlaceHolders"].Split(';').ToList();
            var DOS = paramLists["DOS"];
            var dxCode = paramLists["DxCode"];
            var procCode = paramLists["ProcCode"];
            var patClaimHistoryTabs = paramLists["Tabs"].Split(';').ToList();
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            SearchByClaimSeqFromWorkList(claimSeq);

            StringFormatter.PrintMessage("Verify Dx Code Filter");
            _claimHistory.IsDxCodeInputFieldPresent().ShouldBeTrue("Dx Code Input Field Should Be Present");
            _claimHistory.GetDxCodeInputFieldPlaceHolder().ShouldBeEqual(placeHolders[0], "Place Holder Should Match");
            var maxCharacter = new string('a', 10) + new string('.', 1) + new string('1', 20);
            _claimHistory.SetDxCodeValue(maxCharacter);
            //_claimHistory.GetDxCodeValue()
            //    .Length.ShouldBeEqual(15, "Accept only max 15 alphanumeric characters");

            StringFormatter.PrintMessage("Verify Proc Code Filter");
            _claimHistory.IsProcCodeInputFieldPresent().ShouldBeTrue("Proc Code Input Field Should Be Present");
            _claimHistory.GetProcCodeInputFieldPlaceHolder().ShouldBeEqual(placeHolders[1], "Place Holder Should Match");
            maxCharacter = new string('a', 10) + new string('1', 20);
            _claimHistory.SetProcCodeValue(maxCharacter);
            //_claimHistory.GetDxCodeValue()
            //    .Length.ShouldBeEqual(10, "Accept only max 10 alphanumeric characters");

            StringFormatter.PrintMessage("Verify DOS Filter");
            _claimHistory.IsDOSInputFieldPresent().ShouldBeTrue("DOS Should Be Present");
            _claimHistory.GetDOSInputFieldPlaceHolder().ShouldBeEqual(placeHolders[2], "Place Holder Should Match");
            _claimHistory.ClickOnClearButton();

            StringFormatter.PrintMessage("Verify View Is Blank When There Are No Results For Applied Filters");
            _claimHistory.SetDxCodeValue("randomValue");
            _claimHistory.SetProcCodeValue("randomValue");
            _claimHistory.SetDOSValue("07/13/2020");
            _claimHistory.ClickOnFilterButton();
            _claimHistory.IsViewEmpty().ShouldBeTrue("View Should Be Empty");
            _claimHistory.ClickOnClearButton();


            foreach (var tab in patClaimHistoryTabs)
            {
                StringFormatter.PrintMessage($"Verify Correct Results Are Displayed When Correct Filters Applied for {tab} tab");
                _claimHistory.ClickOnHistoryTabs(tab);
                _claimHistory.SetDxCodeValue(dxCode);
                _claimHistory.SetProcCodeValue(procCode);
                _claimHistory.SetDOSValue(DOS);
                _claimHistory.ClickOnFilterButton();

                _claimHistory.GetAllDosActiveFromPatientClaimHx().Distinct().Count().
                    ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of DOS");
                _claimHistory.GetAllDosActiveFromPatientClaimHx().Distinct().ToList()[0]
                    .ShouldBeEqual(DOS, "DOS should be correct");

                _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().Count().
                    ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of Dx Code");
                _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().ToList()[0]
                    .ShouldBeEqual(dxCode, "DOS should be correct");

                _claimHistory.GetAllProcCodesFromPatientClaimHx().Distinct().Count().
                    ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of Proc Code");
                _claimHistory.GetAllProcCodesFromPatientClaimHx().Distinct().ToList()[0]
                    .ShouldBeEqual(procCode, "DOS should be correct");

                _claimHistory.IsHeaderRowPresent().ShouldBeTrue("Header Row Should Be Present");
                _claimHistory.IsFooterRowPresent().ShouldBeTrue("Footer Row Should Be Present");

                StringFormatter.PrintMessage("Verify Clear Functionality");
                _claimHistory.IsClearButtonPresent().ShouldBeTrue("Is Clear Button Present ?");
                _claimHistory.ClickOnClearButton();
                _claimHistory.GetDOSValue().ShouldBeNullorEmpty("Value should be cleared");
                _claimHistory.GetProcCodeValue().ShouldBeNullorEmpty("Value Should Be Null");
                _claimHistory.GetDOSValue().ShouldBeNullorEmpty("Value Should Be Null");
                _claimHistory.GetAllDosFromPatientClaimHx().Distinct().Count().
                    ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");
                _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().Count().
                    ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");
                _claimHistory.GetAllProcCodesFromPatientClaimHx().Distinct().Count().
                    ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");


            }

            _claimAction.CloseAnyPopupIfExist();


        }

        #endregion

        

        #region PRIVATE METHODS

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
            _claimAction.RemoveLock();
            _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

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
