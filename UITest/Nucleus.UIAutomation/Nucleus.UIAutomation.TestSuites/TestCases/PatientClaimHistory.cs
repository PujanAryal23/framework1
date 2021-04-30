using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PatientClaimHistory
    {
        //#region PRIVATE FIELDS

        //private ClaimActionPage _claimAction;
        //private ClaimSearchPage _claimSearch;
        //private ClientSearchPage _clientSearch;

        //private string _claimSequence = string.Empty;
        //private ClaimHistoryPage _claimHistory;
        //private NewPopupCodePage _newPopupCode;
        //private FlagPopupPage _flagPoup;

        //#endregion

        //#region PROTECTED PROPERTIES

        //protected string GetType().FullName
        //{
        //    get
        //    {
        //        return GetType().FullName;
        //    }
        //}
        //#endregion

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
        //        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        //        _claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, "ClassInit",
        //                                               "ClaimSequence", "Value");
        //        SearchByClaimSeqFromWorkList(_claimSequence);
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}

        //protected override void TestCleanUp()
        //{
        //    _claimAction.CloseAnyTabIfExist();
        //    if (_claimAction.IsPageErrorPopupModalPresent())
        //        _claimAction.ClosePageError();

        //    base.TestCleanUp();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _claimAction = _claimAction.Logout()
        //            .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient).NavigateToCVClaimsWorkList();
        //        HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
        //    }
        //    SearchByClaimSeqFromWorkList(_claimSequence);


        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        if (_claimAction.IsClaimLocked())
        //            Console.WriteLine("Claim is Locked!");
        //        if (_claimHistory != null && !_claimAction.IsProviderHistoryClose())
        //        {
        //            _claimHistory.SwitchToNewClaimActionPage(true);
        //        }


        //        //if (CurrentPage.IsQuickLaunchIconPresent())
        //        //    _claimAction.GoToQuickLaunch();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}

        //#endregion

        #region TEST SUITES

        [Test] // CAR-3043(CAR-3014)
        public void Verify_large_data_load_in_patient_claim_history()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claim = paramList["Claim"];
                var patSeq = paramList["PatSeq"];
                var prvseq = paramList["PrvSeq"];

                _claimAction = automatedBase.CurrentPage.SwitchClientTo(ClientEnum.LOADT).NavigateToCVClaimsWorkList(true);

                SearchByClaimSeqFromWorkList(claim);

                var claimsFromDb = _claimHistory.GetClaimSeqListForHugeDataLoadFromDb(patSeq, prvseq);
                var claimCount = claimsFromDb.Count;
                claimCount.ShouldBeGreater(1000, "Is count greater than 1000?");
                var newerClaSeqListFromDb = claimsFromDb.Take(claimsFromDb.IndexOf(claim)).ToList();
                claimsFromDb.RemoveRange(0, claimsFromDb.IndexOf(claim) + 1);

                try
                {
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
                    var newerClaseqList = claimSeqList.Take(claimSeqList.IndexOf(claim)).ToList();
                    newerClaseqList.ShouldCollectionBeEquivalent(newerClaSeqListFromDb, "Newer claims should be shown");

                    StringFormatter.PrintMessage("Verification of DOS of immediate newer and immediate older claims");
                    _claimHistory
                        .GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claim) - 1])
                        .ShouldBeGreaterOrEqual(
                            _claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(
                                claimSeqList[claimSeqList.IndexOf(claim)]),
                            "Is Dos of newer claim less than current claim?");
                    _claimHistory
                        .GetDosByClaimSeqAndRowFromClaimHistory(claimSeqList[claimSeqList.IndexOf(claim) + 1])
                        .ShouldBeLessOrEqual(
                            _claimHistory.GetDosByClaimSeqAndRowFromClaimHistory(
                                claimSeqList[claimSeqList.IndexOf(claim)]),
                            "Is Dos of older claim less than current claim?");

                    StringFormatter.PrintMessage(
                        "Verification that max of 1000 claims will be shown before current claim");
                    claimSeqList.RemoveRange(0, claimSeqList.IndexOf(claim) + 1);
                    claimSeqList.Count.ShouldBeEqual(1000,
                        "A max of 1000 claims will be shown before the current claim");
                    claimSeqList.ShouldCollectionBeEquivalent(claimsFromDb.Take(1000).ToList(),
                        "Max of 1000 older claims should be shown before current claim");
                }

                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }

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
                    _claimAction.RemoveLock();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

                }

            }
        }

        [Test] //CAR-2950 [CAR-3035]
        public void Verify_8_month_filter_option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;
                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claims = paramList["Claims"].Split(';').ToList();
                StringFormatter.PrintMessageTitle(
                    "Verifying the '8 Months' filter for claims with one, two and three claim lines");
                foreach (var claimSeq in claims)
                {
                    SearchByClaimSeqFromWorkList(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
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
                        .ShouldBeTrue(
                            "History records will be shown for claims whose DOS is between 4 months prior to the earliest DOS and 4 months after the oldest EDOS on the current claim");

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

                #endregion
            }
        }

        [Test]
        public void Verify_tooltip_of_grid_row()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                var TestName = new StackFrame(true).GetMethod().Name;
                string _claimSequence = string.Empty;
                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, "ClassInit",
                    "ClaimSequence", "Value");
                SearchByClaimSeqFromWorkList(_claimSequence);
                _claimHistory.MouseOverOnTableRowFieldCss(1);
                _claimHistory.GetTooltipHeaderValue().ShouldBeEqual("Line Detail", "Header Tooltip of Line Details");
                _claimHistory.GetTooltipContentValue(1)
                    .AssertIsContained("Provider Seq", "Content Tooltip of Line Details");
                _claimHistory.GetTooltipContentValue(2).AssertIsContained("Tos", "Content Tooltip of Line Details");
                _claimHistory.GetTooltipContentValue(3)
                    .AssertIsContained("Adj Status", "Content Tooltip of Line Details");
                _claimHistory.GetTooltipContentValue(4)
                    .AssertIsContained("Proc Type", "Content Tooltip of Line Details");
                _claimHistory.MouseOverOnTableRowFieldCss(3);
                _claimHistory.GetTooltipHeaderValue()
                    .ShouldBeEqual("Place of Service Code", "Header Tooltip of Place of Service Code");
                _claimHistory.GetTooltipContentValue(1)
                    .AssertIsContained("Code", "Content Tooltip of Place of Service Code");
                _claimHistory.GetTooltipContentValue(2)
                    .AssertIsContained("Type", "Content Tooltip of Place of Service Code");
                _claimHistory.GetTooltipContentValue(3)
                    .AssertIsContained("Description", "Content Tooltip of Place of Service Code");
                _claimHistory.MouseOverOnTableRowFieldCss(4);
                _claimHistory.GetTooltipHeaderValue()
                    .ShouldBeEqual("Specialty Code", "Header Tooltip of Specialty Code");
                _claimHistory.GetTooltipContentValue(1).AssertIsContained("Code", "Content Tooltip of Specialty Code");
                _claimHistory.GetTooltipContentValue(2).AssertIsContained("Type", "Content Tooltip of Specialty Code");
                _claimHistory.GetTooltipContentValue(3)
                    .AssertIsContained("Description", "Content Tooltip of Specialty Code");
                _claimHistory.MouseOverOnTableRowFieldCss(5);
                _claimHistory.GetTooltipHeaderValue().ShouldBeEqual("Dx Codes", "Header Tooltip of Dx Codes");
                _claimHistory.GetToolTipTableContentHeader(1)
                    .ShouldBeEqual("Code", "Content of Table Header Tooltip of Dx Codes");
                _claimHistory.GetToolTipTableContentHeader(2)
                    .ShouldBeEqual("V", "Content of Table Header Tooltip of Dx Codes");
                _claimHistory.GetToolTipTableContentHeader(3)
                    .ShouldBeEqual("Description", "Content of Table Header Tooltip of Dx Codes");
                _claimHistory.MouseOverOnTableRowFieldCss(8);
                _claimHistory.GetTooltipHeaderValue().ShouldBeEqual("Modifier Codes", "Header Tooltip of Dx Codes");
                _claimHistory.GetToolTipTableContentHeader(1)
                    .ShouldBeEqual("Code", "Content of Table Header Tooltip of Modifier Codes");
                _claimHistory.GetToolTipTableContentHeader(2).ShouldBeEqual("Description",
                    "Content of Table Header Tooltip of Modifier Codes");
                _claimHistory.MouseOverOnTableRowFieldCss(16);
                _claimHistory.GetTooltipHeaderValue().AssertIsContained("Flag", "Header Tooltip of Flag");
                _claimHistory.GetTooltipContentValue(3).AssertIsContained("Flag Type", "Content Tooltip of Flag");
                _claimHistory.GetTooltipContentValue(4)
                    .AssertIsContained("Cotiviti Auto Review", "Content Tooltip of Flag");
                _claimHistory.GetTooltipContentValue(5)
                    .AssertIsContained("Client Auto Review", "Content Tooltip of Flag");
                _claimHistory.GetTooltipContentValue(6)
                    .AssertIsContained("Flag Description", "Content Tooltip of Flag");
                _claimHistory.MouseOverOnProviderCss();
                _claimHistory.GetTooltipHeaderValue().ShouldBeEqual("Provider", "Header Tooltip of Dx Codes");
                _claimHistory.GetToolTipTableContentHeader(1).ShouldBeEqual("Prov Seq", "Content of Table Header Tooltip ofProvider");
                _claimHistory.GetToolTipTableContentHeader(2).ShouldBeEqual("Name", "Content of Table Header Tooltip of Provider");
                _claimHistory.GetToolTipTableContentHeader(3).ShouldBeEqual("TIN", "Content of Table Header Tooltip of Provider");
                _claimAction.CloseAnyPopupIfExist();

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

            }
        }

        [Test]
        public void Verify_header_text_content_of_revenue_code_and_proc_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;
                NewPopupCodePage _newPopupCode;

                var TestName = new StackFrame(true).GetMethod().Name;

                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, "ClassInit",
                    "ClaimSequence", "Value");

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                SearchByClaimSeqFromWorkList(_claimSequence);
                var revenueCode = _claimHistory.GetRenenueCodeValue();
                _newPopupCode = _claimHistory.ClickOnRevenueCodeandSwitch("Revenue Code - " + revenueCode);
                _newPopupCode.GetPopupHeaderText().ShouldBeEqual("Revenue Code", "Header Text");
                _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                _newPopupCode.GetTextContent(1, 2)
                    .ShouldBeEqual(revenueCode,
                        "Revenue Code on Revenue Code page should equal to Patient Claim History Page");
                _newPopupCode.ClosePopup("Revenue Code - " + revenueCode);
                var procedureCode = _claimHistory.GetProcedureCodeValue();
                _newPopupCode = _claimHistory.ClickOnProcedureCodeandSwitch("CPT Code - " + procedureCode);
                _newPopupCode.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Header Text");
                _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                _newPopupCode.GetTextContent(1, 2)
                    .ShouldBeEqual(procedureCode,
                        "Procedure Code on Procedure Code page should equal to Patient Claim History Page");
                _newPopupCode.ClosePopup("CPT Code - " + procedureCode);
                _claimAction.CloseAnyPopupIfExist();

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
            }
        }

        [Test] //+CAR-2957(CAR-2928)
        public void Verify_tooltip_details_of_flag_in_flag_hover_and_popup_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;
                FlagPopupPage _flagPoup;

                var TestName = new StackFrame(true).GetMethod().Name;

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, "ClassInit",
                    "ClaimSequence", "Value");

                SearchByClaimSeqFromWorkList(_claimSequence);
                var flagCode = _claimHistory.GetFlagCodeValue();
                _claimHistory.MouseOverOnTableRowFieldCss(16);
                var header = _claimHistory.GetTooltipHeaderValue();
                var shorDescription = _claimHistory.GetTooltipContentValue(1);
                var flagType = _claimHistory.GetTooltipContentValue(3).Replace("\r\n", " ");
                var CotivitiReview = _claimHistory.GetTooltipContentValue(4);
                var clientReview = _claimHistory.GetTooltipContentValue(5);
                var description = _claimHistory.GetTooltipContentValue(6).Replace("\r\n", " ");
                var shortDescriptionFromDb = _claimAction.GetEOBMessageFromDatabase(flagCode, "S");
                var longDescriptionFromDb =
                    $"Flag Description {Regex.Replace(_claimAction.GetEOBMessageFromDatabase(flagCode, "G"), @"\s+", " ")}";
                try
                {
                    _flagPoup = _claimHistory.ClickOnFlagandSwitch($"Flag Information - {flagCode}");
                    _flagPoup.GetPopupHeaderText().ShouldBeEqual(header, "Header Text");
                    _flagPoup.GetTextValueinLiTag(1)
                        .ShouldBeEqual(shorDescription, "Flag short description should match");
                    _flagPoup.GetTextValueinLiTag(1).ShouldBeEqual(shortDescriptionFromDb,
                        "Flag short description should match with database");
                    _flagPoup.GetTextValueinLiTag(3).Replace("\r\n", " ").ShouldBeEqual(flagType, "Flag Text");

                    #region CAR-2957

                    _flagPoup.GetTextValueinLiTag(3).Replace("\r\n", " ")
                        .ShouldBeEqual("Flag Type V - Validated", "Flag Text");

                    #endregion

                    _flagPoup.GetTextValueinLiTag(4).ShouldBeEqual(CotivitiReview, "Cotiviti Review Text");
                    _flagPoup.GetTextValueinLiTag(5).ShouldBeEqual(clientReview, "Client Review Text");
                    _flagPoup.GetTextValueinLiTag(6).Replace("\r\n", " ")
                        .ShouldBeEqual(description, "Description Text");
                    _flagPoup.GetTextValueinLiTag(6).Replace("\r\n", " ").ShouldBeEqual(longDescriptionFromDb,
                        "Description Text should match to that with database");
                    _flagPoup.ClosePopup("Flag Information - " + flagCode);
                    HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                    _claimAction.Logout().LoginAsClientUser().NavigateToCVClaimsWorkList().ClickWorkListIcon()
                        .ClickSearchIcon().SearchByClaimSequence(_claimSequence);
                    _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.MouseOverOnTableRowFieldCss(16);
                    _flagPoup = _claimHistory.ClickOnFlagandSwitch("Flag Information - " + flagCode);
                    _flagPoup.GetPopupHeaderText().ShouldBeEqual(header, "Header Text");
                    _flagPoup.GetTextValueinLiTag(1)
                        .ShouldBeEqual(shorDescription, "Flag short description should match");
                    _flagPoup.GetTextValueinLiTag(1)
                        .ShouldBeEqual(shortDescriptionFromDb, "Flag short description should match");
                    _flagPoup.GetTextValueinLiTag(3).Replace("\r\n", " ").ShouldBeEqual(flagType, "Flag Text");

                    #region CAR-2957

                    _flagPoup.GetTextValueinLiTag(3).Replace("\r\n", " ")
                        .ShouldBeEqual("Flag Type V - Validated", "Flag Text");

                    #endregion

                    _flagPoup.GetTextValueinLiTag(4).ShouldBeEqual(clientReview, "Client Review Text");
                    _flagPoup.GetTextValueinLiTag(5).Replace("\r\n", " ")
                        .ShouldBeEqual(description, "Description Text");
                    _flagPoup.GetTextValueinLiTag(5).Replace("\r\n", " ").ShouldBeEqual(longDescriptionFromDb,
                        "Description Text should match to that with database");
                    _flagPoup.ClosePopup("Flag Information - " + flagCode);
                    HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                }
                finally
                {
                    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN,
                        StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        _claimAction = _claimAction.Logout()
                            .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient)
                            .NavigateToCVClaimsWorkList();
                        HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                        SearchByClaimSeqFromWorkList(_claimSequence);
                    }
                    _claimAction.CloseAnyPopupIfExist();
                }

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
                    _claimAction.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //CAR-527
        public void Verify_the_functionality_of_show_Interactive_tooth_chart()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var title = paramLists["Title"].Split(',');
                var category = paramLists["Category"].Split(',').ToList();
                var quadrant = paramLists["Quadrant"].Split(',').ToList();
                var permanentUpperRightToothNumber = paramLists["URToothNumber"].Split(',').ToList();
                var permanentUpperLeftToothNumber = paramLists["ULToothNumber"].Split(',').ToList();
                var permanentLowerLeftToothNumber = paramLists["LLToothNumber"].Split(',').ToList();
                var permanentLowerRightToothNumber = paramLists["LRToothNumber"].Split(',').ToList();
                var deciduousupperRightToothNumber = paramLists["DeciduousURToothNumber"].Split(',').ToList();
                var deciduousupperLeftToothNumber = paramLists["DeciduousULToothNumber"].Split(',').ToList();
                var deciduouslowerLeftToothNumber = paramLists["DeciduousLLToothNumber"].Split(',').ToList();
                var deciduouslowerRightToothNumber = paramLists["DeciduousLRToothNumber"].Split(',').ToList();

                SearchByClaimSeqFromWorkList(claimSeq);
                _claimHistory.ClickDentalIcon();
                _claimHistory.IsDentalChartSelectorPresent(1, "Decidious buttom should be present.");
                _claimHistory.IsDentalChartSelectorPresent(2, "Permanent button should be present.");
                _claimHistory.IsChartSelectorSelected(category[1])
                    .ShouldBeTrue("Permanent tooth chart selection should be true.");
                _claimHistory.IsDentalChartPresent("permanent")
                    .ShouldBeTrue("Permanent dental chart should be shown on default.");
                ClickQuadrantAndIndividualTooth(category[0], quadrant[0], deciduousupperRightToothNumber);
                ClickQuadrantAndIndividualTooth(category[1], quadrant[0], permanentUpperRightToothNumber);
                ClickQuadrantAndIndividualTooth(category[0], quadrant[1], deciduousupperLeftToothNumber);
                ClickQuadrantAndIndividualTooth(category[1], quadrant[1], permanentUpperLeftToothNumber);
                ClickQuadrantAndIndividualTooth(category[0], quadrant[2], deciduouslowerLeftToothNumber);
                ClickQuadrantAndIndividualTooth(category[1], quadrant[2], permanentLowerLeftToothNumber);
                ClickQuadrantAndIndividualTooth(category[0], quadrant[3], deciduouslowerRightToothNumber);
                ClickQuadrantAndIndividualTooth(category[1], quadrant[3], permanentLowerRightToothNumber);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var random = new Random();
                VerifySelectedQuadrantByOralCavity("UR", permanentUpperRightToothNumber, deciduousupperRightToothNumber,
                    random.Next(1, 8).ToString("00"));
                VerifySelectedQuadrantByOralCavity("UL", permanentUpperLeftToothNumber, deciduousupperLeftToothNumber,
                    random.Next(10, 16).ToString());
                VerifySelectedQuadrantByOralCavity("LL", permanentLowerLeftToothNumber, deciduouslowerLeftToothNumber,
                    random.Next(17, 24).ToString());
                VerifySelectedQuadrantByOralCavity("LR", permanentLowerRightToothNumber, deciduouslowerRightToothNumber,
                    random.Next(25, 32).ToString());


                var claimlineTNValue = _claimAction.GetDentalData("claim_line", title[0]).TrimStart('0');
                var claimLineOCValue = _claimAction.GetDentalData("claim_line", title[1]);
                _claimAction.ClickDentalValues(title[0]);
                _claimHistory.WaitForWorkingAjaxMessageOnly();
                _claimHistory.GetSelectedDentalNumber(category[1], claimLineOCValue)[0]
                    .ShouldBeEqual(claimlineTNValue, "TN values must match");

                _claimHistory.IsDentalDataPresent()
                    .ShouldBeTrue("Dental Data must be present for the selected TN value");
                _claimHistory.ClickResetButton();
                var toothNumber = random.Next(2, 9);

                _claimHistory.ClickIndividualTooth(category[1], "UR", toothNumber);
                _claimHistory.GetEmptyHistoryText()
                    .ShouldBeEqual("No Data Found.", "Is Empty Data Found display?");
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);

                void SearchByClaimSeqFromWorkList(string claSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claSeq);
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

                void VerifySelectedQuadrantByOralCavity(string quad, List<string> permanentTN,
                    List<string> deciduousTN, string TNValue)
                {
                    _claimAction.ClickClaimLineDentalEditIcon();
                    _claimAction
                        .InputEditDentalDataLength("TN", TNValue);
                    _claimAction
                        .InputEditDentalDataLength("OC", quad);

                    _claimAction.ClickSaveButton();
                    _claimAction.ClickDentalValues("OC");
                    _claimHistory.WaitForWorkingAjaxMessageOnly();
                    _claimHistory.GetQuadrantName().ShouldBeEqual(quad, "OC values must match");
                    _claimHistory.GetSelectedDentalNumber("permanent", quad)
                        .ShouldCollectionBeEquivalent(permanentTN,
                            string.Format("All tooth number of {0} should be selected", quad));
                    _claimHistory.ClickOnChartSelector("Deciduous");
                    _claimHistory.GetSelectedDentalNumber("deciduous", quad)
                        .ShouldBeEqual(deciduousTN, "");
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                }

                void ClickQuadrantAndIndividualTooth(string cat, string quad, List<string> toothNo)
                {
                    _claimHistory.ClickOnChartSelector(cat);
                    _claimHistory.ClickDentalQuadrant(cat, quad);
                    _claimHistory.IsDentalQuadrantSelected(quad).ShouldBeTrue("Quadrant must be selected");


                    var selectedList = _claimHistory.GetSelectedDentalNumber(cat, quad);
                    if (quad == _claimHistory.GetPatientDentalHistoryData("1", "6"))
                        _claimHistory.IsDentalDataPresent().ShouldBeTrue("Dental data should be shown?");
                    else
                        _claimHistory.IsNoDataFoundPresent().ShouldBeTrue("No Data Found?");

                    selectedList.ShouldCollectionBeEquivalent(toothNo,
                        string.Format("{0} should be selected", toothNo));
                    _claimHistory.ClickDentalQuadrant(cat, quad);
                    _claimHistory.IsDentalQuadrantSelected(quad)
                        .ShouldBeFalse("Quadrant selection should be false");
                    _claimHistory.ClickIndividualTooth(cat, quad, 5);
                    _claimHistory.IsToothNumberSelected(cat, quad, 5)
                        .ShouldBeTrue("The Tooth number must be selected");
                    _claimHistory.ClickResetButton();

                }
            }
        }

        [Test] //CAR-806
        public void Verify_Patient_Tooth_History_Popup_And_Data()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var dos = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "DOS", "Value").Split(';');
                var proc = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Proc", "Value").Split(';');
                var procDescription = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ProcDescription", "Value").Split(';');
                var tn = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "TN", "Value").Split(';');
                var ts = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "TS", "Value").Split(';');
                var oc = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "OC", "Value").Split(';');
                var allowed = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Allowed", "Value").Split(';');
                var flag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Flag", "Value").Split(';');

                SearchByClaimSeqFromWorkList(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                _claimHistory.IsProviderHistoryTabPresent()
                    .ShouldBeTrue("Claim History tab should be present consisting filter options.");
                _claimHistory.IsPatientDxHistoryTabDisplayed()
                    .ShouldBeTrue("Patient Dx History tab should be present.");
                _claimHistory.IsDentalHistoryTabDisplayed()
                    .ShouldBeTrue("Patient Tooth History tab should be present.");
                _claimHistory.IsProviderHistoryTabSelected()
                    .ShouldBeTrue("ClaimHistory tab should be selected by default.");
                _claimHistory.IsProviderHistoryFilterPresent()
                    .ShouldBeTrue("Provider History Filter tab should be available.");
                _claimHistory.IsProviderHistorySelected()
                    .ShouldBeTrue("Provider History Filter should be selected under HX tab");
                _claimHistory.IsTinHistoryFilterPresent().ShouldBeTrue("TIN History Filter tab should be available.");
                _claimHistory.IsSameDayFilterPresent().ShouldBeTrue("Same Day Filter tab should be available.");
                _claimHistory.IsSixtyDaysFilterPresent().ShouldBeTrue("Sixty Days Filter tab should be available.");
                _claimHistory.IsTwelveMonthsFilterPresent()
                    .ShouldBeTrue("Twelve Months Filter tab should be available.");
                _claimHistory.IsAllHistoryFilterPresent().ShouldBeTrue("All History Filter tab should be available.");
                _claimHistory.ClickPatientDxTab();
                _claimHistory.IsPatientDxHistoryTabSelected()
                    .ShouldBeTrue("Patient Dx History tab should be selected.");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnDentalLinkAndSwitchToPatientClaimHistoryPage("TN:");
                _claimHistory.IsDentalHistoryTabSelected()
                    .ShouldBeTrue("Patient Tooth History tab should be selected.");
                int i;
                for (i = 1; i < 3; i++)
                {
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "1")
                        .ShouldBeEqual(dos[i - 1], "First Column should consist the value for DOS.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "2").ShouldBeEqual(proc[i - 1],
                        "Second Column should consist the value for Proc.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "3").ShouldBeEqual(procDescription[i - 1],
                        "Third Column should consist the value for Proc Description.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "4")
                        .ShouldBeEqual(tn[i - 1], "Fourth Column should consist the value for TN.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "5")
                        .ShouldBeEqual(ts[i - 1], "Fifth Column should consist the value for TS.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "6")
                        .ShouldBeEqual(oc[i - 1], "Sixth Column should consist the value for OC.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "7").ShouldBeEqual(allowed[i - 1],
                        "Seventh Column should consist the value for Allowed.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "8").ShouldBeEqual(flag[i - 1],
                        "Eight Column should consist the value for Flag.");
                }

                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                void SearchByClaimSeqFromWorkList(string claSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.RemoveLock();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

                }

            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-1113 (CAR-995)
        public void Verify_visibility_of_patient_tooth_hx_for_non_dental_clients()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {

                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;
                ClientSearchPage _clientSearch;
                ClaimSearchPage _claimSearch;
                
                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, "ClassInit",
                    "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList(); 
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    StringFormatter.PrintMessageTitle("Disabling DCA product for SMTST");
                    automatedBase.CurrentPage = _clientSearch = _claimAction.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.IsRadioButtonOnOffByLabel(ProductEnum.DCA.ToString()).ShouldBeTrue(
                        string.Format("Dental Claim Accuracy should be active for: {0}", ClientEnum.SMTST.ToString()));
                    _clientSearch.ClickOnRadioButtonByLabel(ProductEnum.DCA.ToString(), false);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    //StringFormatter.PrintMessage("Switching the client to RPE");
                    //QuickLaunch.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);

                    StringFormatter.PrintMessageTitle("Verifying whether the tooth icon is visible for the RPE client");
                    _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorkingAjaxMessage();
                    _claimSearch.GetGridViewSection.ClickOnGridByRowCol(1, 2);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.IsDentalHistoryTabPresent().ShouldBeFalse(
                        "Tooth History Icon should not be present in case of clients with inactive DCA Product.");
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                }

                finally
                {
                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickOnRadioButtonByLabel(ProductEnum.DCA.ToString());
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    
                }

            }
        }

        [Test] //CAR-1799(CAR-1098)
        public void Verify_Pre_Auth_Action_Popup_version()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                try
                {
                    SearchByClaimSeqFromWorkList(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Display?");
                    var openedWindowCountBefore = _claimHistory.GetWindowHandlesCount();
                    var patientPreAuthHx = _claimHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    patientPreAuthHx.PageTitle.ShouldBeEqual(PageTitleEnum.PatientPreAuthHistory.GetStringValue(),
                        $"Page Should redirect to {PageHeaderEnum.PatientPreAuthHistory.GetStringValue()}");
                    patientPreAuthHx.GetPopupPageTitle().ShouldBeEqual(
                        PageHeaderEnum.PatientPreAuthHistory.GetStringValue(),
                        $"Popup page should be {PageHeaderEnum.PatientPreAuthHistory.GetStringValue()}");
                    var preAuthSeq = patientPreAuthHx.GetAuthSeq();
                    var preAuthActionPopup = patientPreAuthHx.ClickOnAuthSeqLinkAndNavigateToPreAuthActionPopup();
                    preAuthActionPopup.GetWindowHandlesCount().ShouldBeEqual(openedWindowCountBefore,
                        "Pre-Auth Action page should replace the Patient Claim History window");

                    StringFormatter.PrintMessageTitle("Verification of Pre Auth Action Popup Version");
                    preAuthActionPopup.GetUpperLeftQuadrantValueByLabel("Auth Seq")
                        .ShouldBeEqual(preAuthSeq, "Auth Seq should be displayed correctly");
                    preAuthActionPopup.IsNucleusHeaderPresent().ShouldBeFalse("Nucleus Header Should not display");
                    preAuthActionPopup.IsReturnToSearchIconEnabled()
                        .ShouldBeFalse("Return To Search Icon should disabled");
                    preAuthActionPopup.IsApproveIconEnabled().ShouldBeFalse("Approve Icon should disabled");
                    preAuthActionPopup.IsAddIconEnabled().ShouldBeFalse("Add Flag Icon should disabled");
                    preAuthActionPopup.IsTransferIconDisabled().ShouldBeTrue("Transfer Icon should disabled");
                    preAuthActionPopup.IsNextIconDisabled().ShouldBeTrue("Next Icon should disabled");
                    preAuthActionPopup.IsAllEditIconDisabled()
                        .ShouldBeTrue("All Edit Flag Icon in flagged lines should be disabled");
                    //preAuthActionPopup.IsEditDentalRecordIconDisabled().ShouldBeTrue("Edit Line Item Icon should disabled");

                }
                finally
                {
                    _claimAction.CloseAnyTabIfExist();
                }


                void SearchByClaimSeqFromWorkList(string claSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.RemoveLock();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

                }
            }
        }

        [Test] // CAR-3037([CAR-2946])
        public void Verify_Line_Filters_In_Patient_Claim_History_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;

                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
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
                _claimHistory.GetDxCodeInputFieldPlaceHolder()
                    .ShouldBeEqual(placeHolders[0], "Place Holder Should Match");
                var maxCharacter = new string('a', 10) + new string('.', 1) + new string('1', 20);
                _claimHistory.SetDxCodeValue(maxCharacter);
                //_claimHistory.GetDxCodeValue()
                //    .Length.ShouldBeEqual(15, "Accept only max 15 alphanumeric characters");

                StringFormatter.PrintMessage("Verify Proc Code Filter");
                _claimHistory.IsProcCodeInputFieldPresent().ShouldBeTrue("Proc Code Input Field Should Be Present");
                _claimHistory.GetProcCodeInputFieldPlaceHolder()
                    .ShouldBeEqual(placeHolders[1], "Place Holder Should Match");
                maxCharacter = new string('a', 10) + new string('1', 20);
                _claimHistory.SetProcCodeValue(maxCharacter);
                //_claimHistory.GetDxCodeValue()
                //    .Length.ShouldBeEqual(10, "Accept only max 10 alphanumeric characters");

                StringFormatter.PrintMessage("Verify DOS Filter");
                _claimHistory.IsDOSInputFieldPresent().ShouldBeTrue("DOS filter Should Be Present");
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
                    StringFormatter.PrintMessage(
                        $"Verify Correct Results Are Displayed When Correct Filters Applied for {tab} tab");
                    _claimHistory.ClickOnHistoryTabs(tab);
                    _claimHistory.SetDxCodeValue(dxCode);
                    _claimHistory.SetProcCodeValue(procCode);
                    _claimHistory.SetDOSValue(DOS);
                    _claimHistory.ClickOnFilterButton();

                    _claimHistory.GetAllDosActiveFromPatientClaimHx().Distinct().Count()
                        .ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of DOS");
                    _claimHistory.GetAllDosActiveFromPatientClaimHx().Distinct().ToList()[0]
                        .ShouldBeEqual(DOS, "DOS should be correct");

                    _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().Count()
                        .ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of Dx Code");
                    _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().ToList()[0]
                        .ShouldBeEqual(dxCode, "DOS should be correct");

                    _claimHistory.GetAllProcCodesFromPatientClaimHx().Distinct().Count()
                        .ShouldBeEqual(1, "Data Should Be Filtered On The Basis Of Proc Code");
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
                    _claimHistory.GetAllDosFromPatientClaimHx().Distinct().Count()
                        .ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");
                    _claimHistory.GetAllDxCodesFromPatientClaimHx().Distinct().Count()
                        .ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");
                    _claimHistory.GetAllProcCodesFromPatientClaimHx().Distinct().Count()
                        .ShouldBeGreaterOrEqual(1, "Data Should Be UnFiltered");


                }

                _claimAction.CloseAnyPopupIfExist();

                void SearchByClaimSeqFromWorkList(string claSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.RemoveLock();
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();

                }

            }
        }

        #endregion


    }
}
