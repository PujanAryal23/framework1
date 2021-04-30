using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using InputValidation = UIAutomation.Framework.Utils.InputValidation;
using static System.String;
using Nucleus.Service.Support.Environment;
using NUnit.Framework.Internal;



namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SuspectProviders
    {

        #region PRIVATE FIELDS

        /*private SuspectProvidersPage _suspectProviders;
        private CommonValidations _commonValidation;
        private ProviderActionPage _providerAction;
        private readonly string _suspectProviderPrivilige = RoleEnum.FFPAnalyst.GetStringValue();
        private List<string> _specialtyDropdownList;
        private List<string> _stateDropdownList;
        private List<string> _conditionIDList;
        private ProviderProfilePage _providerProfile;
        private string _originalWindow;
        #endregion
        #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {
            _specialtyDropdownList = _suspectProviders.GetSpecialtyList();
            _stateDropdownList = _suspectProviders.GetStateList();
            _conditionIDList = _suspectProviders.GetConditionIdList();

        }
        #endregion
        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _suspectProviders = QuickLaunch.NavigateToSuspectProviders();
                _commonValidation = new CommonValidations(CurrentPage);
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

            if (Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _suspectProviders = _suspectProviders.Logout().LoginAsHciAdminUser().NavigateToSuspectProviders();
            }
            if (CurrentPage.GetPageHeader() != PageHeaderEnum.SuspectProviders.GetStringValue())
            {
                CurrentPage.NavigateToSuspectProviders();
            }
            _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
            _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
            _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
            _suspectProviders.WaitForWorking();
            _suspectProviders.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
        }
        #endregion
    */

       
        #region PROTECTED PROPERTIES

        //protected string GetType().FullName
        //{
        //    get { return GetType().FullName; }
        //}
        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")]//TANT-91
        public void Verify_Suspect_Provider_sideBar_Pannel_and_sorting_list()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "provider_sorting_option_list_internal").Values
                        .ToList();
                //_suspectProviders.ClickOnQuickLaunch().NavigateToSuspectProviders();

                StringFormatter.PrintMessage("Verify open/close of Find Provider Pannel");
                _suspectProviders.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("By default Find Provider pannel displayed?");
                _suspectProviders.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _suspectProviders.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" Find Provider pannel displayed?");
                _suspectProviders.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _suspectProviders.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse(" Find Provider pannel displayed?");

                StringFormatter.PrintMessage("Verify Data displayed gets updated on selecting different row");
                var provseq = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3);

                _suspectProviders.GetGridViewSection.ClickOnGridRowByRow();
                _suspectProviders.GetRightComponentHeader()
                    .ShouldBeEqual(_suspectProviders.GetGridViewSection.GetValueInGridByColRow(),
                        "Is Provider Name match in secondary Details?");
                _suspectProviders.GetGridViewSection.ClickOnGridRowByRow(2);
                _suspectProviders.GetRightComponentHeader()
                    .ShouldBeEqual(_suspectProviders.GetGridViewSection.GetValueInGridByColRow(2, 2),
                        "Is Provider Name updated in secondary Details?");

                StringFormatter.PrintMessage("Verify profile pop up ");
                //_suspectProviders.CheckProfileIconOpensPopUpAndClose()
                //    .ShouldBeTrue("Clicking on provider profile icon should open  its  pop up page");

                StringFormatter.PrintMessage("Verify Navigation to Provider Action");
                ProviderActionPage _providerAction = _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(provseq);
                _suspectProviders = _providerAction.NavigateToSuspectProviders();

                StringFormatter.PrintMessage("Verify Load More Option");
                //var expectedRowCount = _suspectProviders.TotalCountOfSuspectProvidersFromDatabase();
                var loadMoreValue = _suspectProviders.GetPagination.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                if (numbers[1] > 25)
                {
                    numbers[1].ShouldBeGreaterOrEqual(Convert.ToInt16(25),
                        "When landing on the Provider Search page, Executed search count should be equal to suspect provider count for active batches");
                    var rowcount = _suspectProviders.GetGridViewSection.GetGridRowCount();
                    _suspectProviders.GetPagination.ClickOnLoadMore();
                    _suspectProviders.GetGridViewSection.GetGridRowCount().ShouldBeGreater(rowcount,
                        "Row Count Equal after loading more suspect providers?");
                }
                else
                {
                    _suspectProviders.GetGridViewSection.GetGridRowCount().ShouldBeLessOrEqual(25,
                        "When landing on the Provider Search page, Executed search count should be equal to suspect provider count for active batches");
                }


                StringFormatter.PrintMessage("Verify Sort Option List");
                _suspectProviders.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present?");
                _suspectProviders.GetGridViewSection.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Provider Results", "Correct tooltip is displayed");
                _suspectProviders.GetGridViewSection.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Be Equal");

            }
        }


        [Test, Category("SmokeTestDeployment")] //CAR-131(CAR-404),TANT-91

        public void Verify_a_provider_risk_filter_option_in_find_provider_panel()

        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                automatedBase.CurrentPage.RefreshPage(false);

                var _newClientSearch =
                    _suspectProviders.SwitchTabAndNavigateToQuickLaunchPage().NavigateToClientSearch();

                _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Configuration.GetStringValue());

                var paidExposureThreshold =
                    Convert.ToDouble(
                        _newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum.PaidExposureThreshold
                            .GetStringValue()));
                var providerScoreThreshold =
                    Convert.ToInt32(_newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                        .ProviderScoreThreshold.GetStringValue()));
                _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle(), true);

                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a >= providerScoreThreshold,
                    a => a < paidExposureThreshold, "High Risk,Low Dollars", 1);
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a >= providerScoreThreshold,
                    a => a >= paidExposureThreshold, "High Risk,High Dollars", 2);
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a < providerScoreThreshold,
                    a => a < paidExposureThreshold, "Low Risk,Low Dollars", 3);
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a < providerScoreThreshold,
                    a => a >= paidExposureThreshold, "Low Risk,High Dollars", 4);

                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(1)
                    .ShouldBeFalse("Is High Risk,Low Dollars Quadrant Selected?");
                _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(2)
                    .ShouldBeFalse("Is High Risk,High Dollars Quadrant Selected?");
                _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(3)
                    .ShouldBeFalse("Is Low Risk,Low Dollars Quadrant Selected?");
                _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(4)
                    .ShouldBeFalse("Is Low Risk,High Dollars Quadrant Selected?");


            }
        }




        [Test] //CAR-407, CAR-375
        [Order(3)]
        public void Verify_provider_decision_quadrant_graphic()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;

                try
                {
                    var _newClientSearch = _suspectProviders.SwitchTabAndNavigateToQuickLaunchPage()
                        .NavigateToClientSearch();

                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());

                    var paidExposureThreshold =
                        Convert.ToDouble(_newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                            .PaidExposureThreshold.GetStringValue()));
                    var providerScoreThreshold =
                        Convert.ToInt32(_newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                            .ProviderScoreThreshold.GetStringValue()));
                    _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle());

                    for (var loop = 0; loop < 2; loop++)
                    {
                        _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                        _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                            "Condition",
                            "SDBS");
                        _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                            "Condition",
                            "STPX");
                        _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                        _suspectProviders.WaitForWorking();

                        _suspectProviders.GetProviderRiskXAxisLabel().ShouldBeEqual("12 months of paid-claim exposure",
                            "X axis label should be consistent");
                        _suspectProviders.GetProviderRiskYAxisLabel()
                            .ShouldBeEqual("fraud likelihood", "Y axis label should be consistent");
                        _suspectProviders.GetXAxisMidPointValue().Replace("$", "").Replace(",", "").ShouldBeEqual(
                            paidExposureThreshold.ToString(),
                            "Mid Point value for  Paid Exposure should be consistent withPaid Exposure Threshold setting ");
                        _suspectProviders.GetYAxisMidPointValue().Replace("$", "").Replace(",", "").ShouldBeEqual(
                            providerScoreThreshold.ToString(),
                            "Mid Point value for Fraud Likelihood should be consistent withPaid Provider Score Threshold setting ");

                        for (var j = 0; _suspectProviders.GetPagination.IsLoadMoreLinkable() && j < 5; j++)
                        {
                            _suspectProviders.GetPagination.ClickOnLoadMore();
                        }

                        var score = _suspectProviders.GetSearchResultListByCol(1);
                        var prvSeq = _suspectProviders.GetSearchResultListByCol(3);
                        var paid = _suspectProviders.GetSearchResultListByCol(7)
                            .Select(x => double.Parse(x, System.Globalization.NumberStyles.Currency).ToString())
                            .ToList();

                        var combineList = prvSeq.Zip(score, (n, p) => new { n, p })
                            .Zip(paid, (t, c) => new { prvSeq = t.n, score = t.p, paid = c }).ToList();

                        var highRiskLowDollar = combineList.Select(x => x).Where(x =>
                                Convert.ToDouble(x.paid) < paidExposureThreshold &&
                                Convert.ToInt16(x.score) >= providerScoreThreshold)
                            .ToList();

                        var highRiskHighDollar = combineList.Select(x => x).Where(x =>
                                Convert.ToDouble(x.paid) >= paidExposureThreshold &&
                                Convert.ToInt16(x.score) >= providerScoreThreshold)
                            .ToList();

                        var lowRiskLowDollar = combineList.Select(x => x).Where(x =>
                                Convert.ToDouble(x.paid) < paidExposureThreshold &&
                                Convert.ToInt16(x.score) < providerScoreThreshold)
                            .ToList();

                        var lowRiskHighDollar = combineList.Select(x => x).Where(x =>
                                Convert.ToDouble(x.paid) >= paidExposureThreshold &&
                                Convert.ToInt16(x.score) < providerScoreThreshold)
                            .ToList();

                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(highRiskLowDollar[0].prvSeq);
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(1)
                            .ShouldBeTrue("Is High Risk Low Dollor Rectangle Selected");
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(highRiskHighDollar[0].prvSeq);
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(2)
                            .ShouldBeTrue("Is High Risk High Dollor Rectangle Selected");
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(lowRiskLowDollar[0].prvSeq);
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(3)
                            .ShouldBeTrue("Is Low Risk Low Dollor Rectangle Selected");
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(lowRiskHighDollar[0].prvSeq);
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(4)
                            .ShouldBeTrue("Is Low Risk High Dollor Rectangle Selected");

                        _suspectProviders.GetRightSideSubHeader()[0]
                            .ShouldBeEqual("Provider Risk", "Verify Header of Quadrant");

                        VerifyProviderDecisionQuadrantLogic(_suspectProviders,"Upper left", "High risk, low dollars",
                            highRiskLowDollar.Count,
                            combineList.Count);
                        VerifyProviderDecisionQuadrantLogic(_suspectProviders,"Upper right", "High risk, high dollars",
                            highRiskHighDollar.Count,
                            combineList.Count, 2);
                        VerifyProviderDecisionQuadrantLogic(_suspectProviders,"Lower Left", "Low risk, low dollars",
                            lowRiskLowDollar.Count,
                            combineList.Count, 3);
                        VerifyProviderDecisionQuadrantLogic(_suspectProviders,"Lower right", "Low risk, high dollars",
                            lowRiskHighDollar.Count,
                            combineList.Count, 4);

                        var defaultPaidExposure = 100000;
                        var defaultProviderScore = 500;

                        if (loop == 0)
                        {
                            _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle());

                            paidExposureThreshold =
                                Convert.ToDouble(_newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                                    .PaidExposureThreshold.GetStringValue())) ==
                                defaultPaidExposure
                                    ? 50000
                                    : defaultPaidExposure;
                            providerScoreThreshold =
                                Convert.ToInt32(_newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                                    .ProviderScoreThreshold.GetStringValue())) ==
                                defaultProviderScore
                                    ? 400
                                    : defaultProviderScore;

                            _newClientSearch.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.PaidExposureThreshold.GetStringValue(),
                                paidExposureThreshold.ToString());
                            _newClientSearch.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.ProviderScoreThreshold.GetStringValue(),
                                providerScoreThreshold.ToString());
                            _newClientSearch.GetSideWindow.Save();

                            _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle());
                            _suspectProviders.RefreshPage(false);
                            _suspectProviders.RefreshPage(false);
                            _suspectProviders.WaitForSpinner();
                            _suspectProviders.WaitForGridToLoad();
                            _suspectProviders.WaitForWorkingAjaxMessage();
                            _suspectProviders.WaitForStaticTime(3000);
                            StringFormatter.PrintMessageTitle(
                                "Verify Quadrant after Paid Exposure and Provider Score changed");
                        }
                        else
                        {
                            _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle());

                            if (_newClientSearch.IsClientSettingsFormReadOnly())
                                _newClientSearch.GetSideWindow.ClickOnEditIcon();
                            _newClientSearch.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.PaidExposureThreshold.GetStringValue(),
                                defaultPaidExposure.ToString());
                            _newClientSearch.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.ProviderScoreThreshold.GetStringValue(),
                                defaultProviderScore.ToString());
                            _newClientSearch.GetSideWindow.Save();
                            _suspectProviders.SwitchTab(_suspectProviders.GetCurrentWindowHandle(), true);
                            StringFormatter.PrintMessageTitle(
                                "Reset to Default Value");
                        }
                    }
                }
                finally
                {
                    _suspectProviders.CloseAnyTabIfExist();
                    _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                }
            }
        }

        private void VerifyProviderDecisionQuadrantLogic(SuspectProvidersPage _suspectProviders, string quadrantName, string quadrantText, int quadrantProviderCount, int totalProviderCount, int quadrant = 1)
        {
            _suspectProviders.GetRiskQuadrantText(quadrant, 1)
                .ShouldBeEqual(quadrantText, "Verification of Quadrants Label of " + quadrantName);
            _suspectProviders.GetRiskQuadrantText(quadrant, 2)
                .ShouldBeEqual(Math.Round(quadrantProviderCount * 100.0 / totalProviderCount, 0, MidpointRounding.AwayFromZero) + "%", "Verification of Percentage Label of " + quadrantName);
            _suspectProviders.GetRiskQuadrantText(quadrant, 3)
                .ShouldBeEqual(quadrantProviderCount + " Providers", "Verification of Total Provider of " + quadrantName);
        }


        [Test] //CAR-35
        [Order(4)]
        public void Verify_sorting_of_provider_search_result_for_different_sort_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper
                        .GetMappingData(GetType().FullName, "provider_sorting_option_list_internal").Values
                        .ToList();

                var colIndex = new List<int> { 10, 9, 8, 7, 6, 5, 4, 2, 1 };
                try
                {
                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                        "All");
                    _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                    _suspectProviders.WaitForWorkingAjaxMessage();
                    _suspectProviders.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "Search Result must be listed.");

                    _suspectProviders.GetGridViewSection.IsListInRequiredOrder(1, true)
                        .ShouldBeTrue(
                            "All sorting is cleared and search list is sorted by Provider Score which is default sort.");

                    StringFormatter.PrintMessageTitle("Validation of Sorting by Provider Score");
                    filterOptions.Reverse();
                    for (var i = 1; i < filterOptions.Count; i++)
                    {
                        ValidateProviderSearchRowSorted(_suspectProviders,colIndex[i - 1], filterOptions[i]);

                    }

                    _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[0]);
                    StringFormatter.PrintMessage("Clicked on Clear Sort.");
                    _suspectProviders.IsListStringSortedInDescendingOrder(1)
                        .ShouldBeTrue(
                            "All sorting is cleared and search list is sorted by Provider Score which is default sort.");

                }
                finally
                {
                    _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(filterOptions[0]);
                    StringFormatter.PrintMessage("Clicked on Clear Sort.");
                }


            }
        }

        /// <summary>
        /// X=Required,R=Review,D=Deny,Q=Monitor,O=Record Review
        /// </summary>
        [Test] //CAR-313 and CAR-319 + TE-563
        [Retry(3)]
        public void Verify_display_provider_result_set()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var lockBy = paramLists["LockBy"];
                var prvSeqReview = paramLists["PrvSeqReview"];
                var prvSeqNoReview = paramLists["PrvSeqNoReview"];
                var prvSeqForMultipleSuspectCondtion = paramLists["PrvSeqForMultipleSuspectCondtion"];
                var cActionEyeBallList = new List<char> { 'X', 'R', 'D', 'Q' };
                var cActionNoEyeBallList = new List<char> { 'O', 'N' };

                try
                {
                    _suspectProviders.DeleteProviderLockByProviderSeq(prvSeqNoReview);
                    _suspectProviders.DeleteProviderLockByProviderSeq(prvSeqReview);
                    _suspectProviders.Logout().LoginAsHciAdminUser2().NavigateToSuspectProviders();

                    StringFormatter.PrintMessageTitle("Verify Score Level for different range");
                    _suspectProviders.GetGridListValueforScore().ShouldCollectionBeSorted(true,
                        "Suscpect Provider results should be sorted by provider score in descending order");
                    Enumerable.Range(900, 1000)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.HIGH))
                        .ShouldBeTrue("High Score Level should be in range of 900-100 and should be RED");
                    Enumerable.Range(700, 899)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.ELEVATED))
                        .ShouldBeTrue("Elevated Score Level should be in range of 900-100 and should be ORANGE");
                    Enumerable.Range(500, 699)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.MODERATE))
                        .ShouldBeTrue("Moderate Score Level should be in range of 900-100 should be YELLOW");
                    Enumerable.Range(0, 499).Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.LOW))
                        .ShouldBeTrue("Low Score Level should be in range of 900-100 should be GREEN");

                    StringFormatter.PrintMessageTitle(
                        "Verify Search is executed while landing on Provider Search page.");

                    var expectedRowCount = _suspectProviders.TotalCountOfSuspectProvidersFromDatabase();
                    var loadMoreValue = _suspectProviders.GetPagination.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                        .Select(m => int.Parse(m.Trim())).ToList();
                    if (numbers[1] > 25)
                    {

                        numbers[1].ShouldBeEqual(Convert.ToInt16(expectedRowCount),
                            "When landing on the Provider Search page, Executed search count should be equal to suspect provider count for active batches");
                        var rowcount = _suspectProviders.GetGridViewSection.GetGridRowCount();
                        _suspectProviders.GetPagination.ClickOnLoadMore();
                        _suspectProviders.GetGridViewSection.GetGridRowCount().ShouldBeGreater(rowcount,
                            "Row Count Equal after loading more suspect providers?");
                    }
                    else
                    {
                        _suspectProviders.GetGridViewSection.GetGridRowCount().ShouldBeEqual(expectedRowCount,
                            "When landing on the Provider Search page, Executed search count should be equal to suspect provider count for active batches");

                    }


                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.SearchByProviderSequence(prvSeqNoReview);
                    _suspectProviders.IsProviderLockIconPresent(prvSeqNoReview)
                        .ShouldBeFalse("Is lock icon present initially?");

                    _suspectProviders.IsReviewIconByRowPresent()
                        .ShouldBeFalse("Is Review icon/eye-ball icon displayed for Cotiviti Action=<No Action>");

                    StringFormatter.PrintMessageTitle("Verify Eyeball icon for different situation");
                    var vAction = 'R';
                    for (var i = 0; i < 2; i++)
                    {
                        foreach (var cAction in cActionNoEyeBallList)
                        {
                            _suspectProviders.UpdateVactionCactionInTriggeredCondition(prvSeqReview, "SELE", vAction,
                                cAction);
                            _suspectProviders.SearchByProviderSequence(prvSeqReview);
                            _suspectProviders.IsReviewIconByRowPresent()
                                .ShouldBeFalse(Format(
                                    "Is Review icon/eye-ball icon displayed for Cotiviti Action=<{0}> and Client Action=<{1}>?",
                                    vAction, cAction));
                        }

                        foreach (var cAction in cActionEyeBallList)
                        {
                            _suspectProviders.UpdateVactionCactionInTriggeredCondition(prvSeqReview, "SELE", vAction,
                                cAction);
                            _suspectProviders.SearchByProviderSequence(prvSeqReview);
                            _suspectProviders.IsReviewIconByRowPresent()
                                .ShouldBeTrue(Format(
                                    "Is Review icon/eye-ball icon displayed for Cotiviti Action=<{0}> and Client Action=<{1}>?",
                                    vAction, cAction));
                            _suspectProviders.GetReviewIconTooltip().ShouldBeEqual("Provider currently on review.");
                        }

                        vAction = 'D';
                    }

                    ProviderActionPage _providerAction;

                    automatedBase.CurrentPage = _providerAction =
                        _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqReview);
                    var providerName = _providerAction.GetProviderName();
                    var prvSeq = _providerAction.GetProviderSequence();
                    var suspectConditionList = _providerAction.GetConditionIdListInProviderConditions()
                        .Select(x => x.Split('-')[0].Trim()).ToList();
                    var paid = _providerAction.GetProviderExposureCountValue(4, 1).Replace(",", "");
                    var state = _providerAction.GetProviderDetailByLabel("State");
                    var specialty = _providerAction.GetProviderDetailByLabel("Specialty").Split('-')[0].Trim();
                    var triggeredDate = _providerAction.GetTriggeredDaeInProviderConditions();
                    var tin = _providerAction.GetProviderDetailByLabel("TIN");
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;



                    StringFormatter.PrintMessageTitle("Verify Provider lock functionality");
                    _suspectProviders.IsLockIconPresentAndGetLockIconToolTipMessage(prvSeqReview, pageUrl)
                        .ShouldBeEqual(Format("The provider is currently being viewed by {0}", lockBy),
                            "Provider Icon is locked and Tooltip Message");
                    automatedBase.CurrentPage = _providerAction =
                        _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqReview);
                    _providerAction.IsProviderOpenedInViewMode().ShouldBeTrue("Is provider opened in view mode");
                    _providerAction.ClickOnSearchIconAtHeader();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                        Extensions.GetStringValue(PageHeaderEnum.SuspectProviders),
                        "Clicking on Search Icon on header should return to Suspect Provider search Page");
                    _suspectProviders.CloseNewTabIfExists(PageTitleEnum.SuspectProviders);

                    StringFormatter.PrintMessageTitle("Verify Grid value and label");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow()
                        .ShouldBeEqual(providerName, "Is Provider Name Equals");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3)
                        .ShouldBeEqual(prvSeq, "Is Provider Sequence Equal?");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(6).Split(',')
                        .Select(x => x.Split('-')[0].Trim()).ToList()
                        .ShouldCollectionBeEquivalent(suspectConditionList, "Is Suspect Condition in Equals");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(4)
                        .ShouldBeEqual(tin, "Is Value of Tin Equal");
                    Convert.ToInt64(double.Parse(_suspectProviders.GetGridViewSection.GetValueInGridByColRow(7),
                            System.Globalization.NumberStyles.Currency)).ToString()
                        .ShouldBeEqual(paid, "Is Value of Paid Equal?");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(8)
                        .ShouldBeEqual(state, "Is State Equal?");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(9)
                        .ShouldBeEqual(specialty, "Is Speciality Equal?");
                    specialty.Length.ShouldBeEqual(2, "Value of specialty should equal to 2 digit only");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(10)
                        .ShouldBeEqual(triggeredDate, "Is Triggred Date Equals?");
                    triggeredDate.IsDateInFormat().ShouldBeTrue("Triggered Date should be in MM/DD/YYY format?");

                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol()
                        .ShouldBeFalse("Is Label display for Provider Name?");
                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol(3)
                        .ShouldBeFalse("Is Label display for Provider Sequence?");
                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol(6)
                        .ShouldBeFalse("Is Label display for Suspect Conditions ?");
                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol(8)
                        .ShouldBeFalse("Is Label display for State?");
                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol(9)
                        .ShouldBeFalse("Is Label display for Specialty?");
                    _suspectProviders.GetGridViewSection.IsLabelPresentByCol(10)
                        .ShouldBeFalse("Is Label display for Triggered Date?");

                    _suspectProviders.GetGridViewSection.GetLabelInGridByColRow(4).ShouldBeEqual("TIN:");
                    _suspectProviders.GetGridViewSection.GetLabelInGridByColRow(5).ShouldBeEqual("NPI:");
                    _suspectProviders.GetGridViewSection.GetLabelInGridByColRow(7).ShouldBeEqual("Paid:");

                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.SearchByProviderSequence(prvSeqForMultipleSuspectCondtion);

                    var list = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(6).Split(',')
                        .Select(x => x.Trim()).ToList();
                    list.Count.ShouldBeGreater(1, "Suspect Condition should be in comma separated");
                    list.IsInAscendingOrder()
                        .ShouldBeTrue("Suspect Condition Should be in ascending order");

                    StringFormatter.PrintMessage("Verification of NPI Primary Data Point");
                    var npi = _suspectProviders.GetProviderNpiFromDatabase(paramLists["PrvSeqWithNpi"]);
                    _suspectProviders.SearchByProviderSequence(paramLists["PrvSeqWithNpi"]);
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(5)
                        .ShouldBeEqual(npi, "Is Value of Npi Equal");


                }
                finally
                {
                    _suspectProviders.CloseNewTabIfExists(PageTitleEnum.SuspectProviders);
                }

            }
        }

        [Test] //CAR-276
        [NonParallelizable]
        public void Validate_Security_And_Navigation_Of_Suspect_Provider_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();

                var TestName = new StackFrame(true).GetMethod().Name;
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);

                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Provider,
                    new List<string> {SubMenu.SuspectProviders, SubMenu.SuspectProvidersWorkList},
                    RoleEnum.FFPAnalyst.GetStringValue(),
                    new List<string>
                    {
                        PageHeaderEnum.SuspectProviders.GetStringValue(), PageHeaderEnum.ProviderAction.GetStringValue()
                    }, automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});

            }
        }

        [Test] //CAR-316 //TE-378
        [Retry(3)]
        public void Verify_Filter_Suspect_Provider_List()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage =
                    _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var prvseq = testData["ProviderSeq"];
                var tin = testData["TIN"];
                var npi = testData["NPI"];
                var prvfn = testData["ProviderFullName"];
                var condition = testData["Condition"];
                var trgDate = testData["TriggeredDate"].Split(';').ToList();
                var trgDateFrom = Convert.ToDateTime(trgDate[0]);
                var trgDateTo = Convert.ToDateTime(trgDate[1]);
                var state = testData["State"];
                var specialty = testData["Specialty"];
                var pseq1 = new string('1', 16);
                var tin1 = new string('1', 11);
                var npi1 = new string('#', 51);
                var pfn = new string('@', 101);
                var providerFirstName = testData["firstname"];
                var providerLastName = testData["lastname"];
                


        var previouslyViewedProviderSequenceList = new List<string>
                    {
                        prvseq
                    };

                //_suspectProviders = CurrentPage.ClickOnQuickLaunch().NavigateToSuspectProviders();
                List<string> _conditionIDList = _suspectProviders.GetConditionIdList();
                List<string> _stateDropdownList = _suspectProviders.GetStateList();
                List<string> _specialtyDropdownList = _suspectProviders.GetSpecialtyList();


                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.GetSideBarPanelSearch.GetTopHeaderName().ShouldBeEqual("Find Providers",
                    "The filter panel should have title Find Providers");
                //var defaultProviderList = _suspectProviders.GetSearchResultListByCol(2);
                var defaultProviderScoreList = _suspectProviders.GetGridListValueforScore();
                var defaultTotalResults = _suspectProviders.GetGridViewSection.GetTotalInLoadMore();
                var prvSeqList = _suspectProviders.GetSearchResultListByCol(3);
                CheckFilterValues(_suspectProviders);
                StringFormatter.PrintMessageTitle("Verify Clear Filter clears all values in the search criteria.");
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", "a");
                _suspectProviders.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                    "Provider Seq field should allow only numeric values.");
                _suspectProviders.ClosePageError();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", "a");
                _suspectProviders.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                    "TIN field should allow only numeric values.");
                _suspectProviders.ClosePageError();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", pseq1);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", tin1);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("NPI", npi1);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", pfn);

                var conditionList = _suspectProviders.GetDropdownValueList("Condition");
                var list = conditionList.Select(x => x.Length).Distinct().ToList();
                list.Count.ShouldBeEqual(1, "Verify Condition Id should be in 4 digit");
                list[0].ShouldBeEqual(4, "Verify Condition Id should be in 4 digit");


                ValidateDropDownForDefaultValueAndExpectedList(_suspectProviders,"Condition", _conditionIDList, true);
                ValidateDropDownForDefaultValueAndExpectedList(_suspectProviders,"State", _stateDropdownList, true);
                var _specialtyDropdownListWithSpace = _specialtyDropdownList
                    .Select(s => s.Remove(s.IndexOf("-"), 1).Insert(s.IndexOf("-"), " - ")).ToList();
                ValidateDropDownForDefaultValueAndExpectedList(_suspectProviders,"Specialty", _specialtyDropdownListWithSpace, true);
                _suspectProviders.SetDateFieldFrom("Triggered Date", DateTime.Now.ToString("MM/d/yyyy"));

                _suspectProviders.GetDateFieldTo("Triggered Date").ShouldBeEqual(
                    DateTime.Now.ToString("MM/dd/yyyy"),
                    "End date field should automatically populate with the beginning date.");
                _suspectProviders.SetDateFieldTo("Triggered Date", DateTime.Now.AddMonths(4).ToString("MM/d/yyyy"));

                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq").Length
                    .ShouldBeEqual(15, "Provider Seq should allow upto 15 numeric charater");
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("TIN").Length
                    .ShouldBeEqual(10, "TIN Should allow upto 10 numeric characters");
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("NPI").Length
                    .ShouldBeEqual(50, "NPI should allow upto 50");
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name").Length
                    .ShouldBeEqual(100, "Provider Full Name should allow upto 100 characters.");

                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    Concat(Enumerable.Repeat("a1@_B", 21)));
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    Concat(Enumerable.Repeat("a1@_B", 21)));
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name").Length
                    .ShouldBeEqual(100, "Provider Full Name should allow upto 100 characters.");
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name").Length
                    .ShouldBeEqual(100, "Provider Full Name should allow upto 100 characters.");


                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.GetPageErrorMessage().ShouldBeEqual(
                    "Triggered date range must be three months or less.",
                    "User should be able to execute search for a date range of 3 months or less.");
                _suspectProviders.ClosePageError();
                _suspectProviders.SetDateFieldTo("Triggered Date",
                    DateTime.Now.AddMonths(-5).ToString("MM/d/yyyy"));
                _suspectProviders.GetPageErrorMessage().ShouldBeEqual("Please enter a valid date range.",
                    "Beginning Date should be less than End date.");
                _suspectProviders.ClosePageError();
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                CheckFilterValues(_suspectProviders);
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorking();
                _suspectProviders.GetGridListValueforScore().ShouldCollectionBeEqual(defaultProviderScoreList,
                    "Search with no search criteria should show all suspect providers.");

                _suspectProviders.GetGridViewSection.GetTotalInLoadMore().ShouldBeEqual(defaultTotalResults,
                    "Total count of search results should remain the same when no search criteria is entered");

                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", "111");
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorking();
                _suspectProviders.IsNoDataAvailablePresent()
                    .ShouldBeTrue("No Results Found message should be shown when no results are found.");

                StringFormatter.PrintMessageTitle("Verify Search Result and Previously Viewed Providers list");
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvseq);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", tin);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("NPI", npi);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", prvfn);
                _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Condition",
                    condition);
                _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                    state);
                _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                    specialty);
                _suspectProviders.SetDateFieldFrom("Triggered Date", trgDateFrom.ToString("MM/d/yyyy"));
                _suspectProviders.SetDateFieldTo("Triggered Date", trgDateTo.ToString("MM/d/yyyy"));
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    providerFirstName.Split(',')[1]);
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    providerLastName.Split(',')[1]);
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorking();

                _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(prvseq, "Is Search Result Match?");
                automatedBase.CurrentPage = _providerAction =
                    _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvseq);
                _providerAction.ClickOnSearchIconAtHeader();
                _suspectProviders.WaitForWorking();
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", "Thomp");
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorking();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol()[0].AssertIsContained("Thomp",
                    "Provider Full Name should show the search result even for partial match of full name.");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorking();
                previouslyViewedProviderSequenceList.Add(prvSeqList[0]);
                automatedBase.CurrentPage = _providerAction =
                    _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqList[0]);
                _providerAction.ClickOnSearchIconAtHeader();
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                previouslyViewedProviderSequenceList.Add(prvSeqList[1]);
                previouslyViewedProviderSequenceList.Reverse();
                automatedBase.CurrentPage = _providerAction =
                    _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqList[1]);
                _providerAction.ClickOnSearchIconAtHeader();
                _suspectProviders.WaitForWorking();
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.GetPreviouslyViewedProviderSeqList()
                    .ShouldBeEqual(previouslyViewedProviderSequenceList);
                automatedBase.CurrentPage = _providerAction =
                    _suspectProviders.ClickOnPreviouslyViewedProviderSequenceAndNavigateToProviderActionPage();
                _providerAction.GetProviderSequence().ShouldBeEqual(previouslyViewedProviderSequenceList[0]);
                automatedBase.CurrentPage = _suspectProviders =
                    _providerAction.ClickOnSearchIconAtHeaderReturnToSuspectProvidersPage();

                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();

                // TE_378
                StringFormatter.PrintMessage("Verify filter search results using first and last name");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();

                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    providerFirstName.Split(',')[0]);
                _suspectProviders.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEquivalent(
                    _suspectProviders.FullOrFacilityNameFromDatabase(providerFirstName.Split(',')[0]),
                    "Verify full or facility name");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    providerLastName.Split(',')[0]);
                _suspectProviders.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEquivalent(
                    _suspectProviders.FullOrFacilityNameFromDatabase(providerLastName.Split(',')[0], "lastname"),
                    "Provider Sequence value equal?");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }


        [Test, Category("SmokeTestDeployment")]//CAR-40+TANT-92

        public void Verify_Suspect_Provider_WorkList()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                const int rowValuetoPress = 3;
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State", "All");
                _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();

                var providerSeqListFromGrid = _suspectProviders.GetGridViewSection.GetGridListValueByCol(3)
                    .Skip(rowValuetoPress - 1).ToList();

                _providerAction = _suspectProviders.NavigateToProviderAction(() =>
                    _suspectProviders.GetGridViewSection.ClickOnGridByRowCol(rowValuetoPress));

                VerifyConditionOnProviderWorkList(_providerAction,automatedBase.CurrentPage,_providerAction.IsWorkListProviderSequenceConsistent,
                    "Does WorkList Order Retain the Filter Panel Result Order and Provider Sequence Change on pressing Next",
                    verificiationList: providerSeqListFromGrid);

                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),
                    "WorkList should return to Suspect Provider Page when the user reaches the end of the list");
            }
        }


        [Test] //CAR-40
        [NonParallelizable]

        public void Verify_provider_lock_and_next_button_should_disabled_when_read_only()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                const int rowToLockfromDb = 2;
                string prvseqtoLock = null;
                try
                {
                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                        "All");
                    _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                    _suspectProviders.WaitForWorkingAjaxMessage();


                    StringFormatter.PrintMessageTitle(
                        "Verifying if a provider is locked, the provider is opened in read only ");
                    prvseqtoLock = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3, rowToLockfromDb);

                    _suspectProviders.LockProviderFromDB(prvseqtoLock, "uiautomation_2");
                    _providerAction = _suspectProviders.NavigateToProviderAction(() =>
                        _suspectProviders.GetGridViewSection.ClickOnGridByRowCol(rowToLockfromDb));

                    _providerAction.GetProviderLockIconToolTip().Contains("This provider has been opened in view mode.")
                        .ShouldBeTrue("Provider should be opened in View Mode when Locked");

                    _providerAction.IsNextIconDisabled()
                        .ShouldBeFalse("Next Icon should not be disabled when provider is locked");


                }
                finally
                {
                    _suspectProviders.DeleteProviderLockByProviderSeq(prvseqtoLock);
                }
            }
        }

        private void VerifyConditionOnProviderWorkList(ProviderActionPage _providerAction, NewDefaultPage CurrentPage,
        Func<ProviderActionPage, string, bool> requiredCondition, string message, int numberOfNext = 5, List<string> verificiationList = null)
        {
            int count = 0;
            if (verificiationList != null)
                numberOfNext = verificiationList.Count;

            //Keep pressing NEXT on worklist
            while (count < numberOfNext &&
                   CurrentPage.GetPageHeader() == PageHeaderEnum.ProviderAction.GetStringValue())
            {
                if (verificiationList == null)
                    requiredCondition(_providerAction, null).ShouldBeTrue(message);
                else
                    requiredCondition(_providerAction, verificiationList[count])
                        .ShouldBeTrue(message);

                _providerAction.ClickOnNextOnProviderAction();
                count++;
            }

        }

        [Test] //CAR-499

        public void Verify_Provider_Exposure_Data()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                string _originalWindow = _suspectProviders.GetCurrentWindowHandle();
                var prvSeq = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3);
                var providerExposureDetailsList =
                    _suspectProviders.GetProviderExposureDetails(prvSeq);
                List<int> providerdata = new List<int>();
                for (var i = 0; i < providerExposureDetailsList.Count; i++)
                {
                    var val = providerExposureDetailsList[i];
                    if (val.Contains('.'))
                        providerdata.Insert(i, Convert.ToInt32(Math.Round(Convert.ToDecimal(val))));
                    else providerdata.Insert(i, Convert.ToInt32(val));
                }

                var specialty = providerExposureDetailsList[1];
                var billed = providerdata[8];
                var paid = providerdata[9];
                var savings = providerdata[11];
                var patients = providerdata[3];
                var visits = providerdata[7];
                var claims = providerdata[4];
                var lines = providerdata[5];
                var procCodes = providerdata[14];
                var icdCodes = providerdata[15];
                List<string> providerRecord = new List<string>();
                providerRecord.AddRange(new List<string>
                {
                    billed.ToString(), paid.ToString(), savings.ToString(), patients.ToString(), visits.ToString(),
                    claims.ToString(), lines.ToString(), procCodes.ToString(), icdCodes.ToString()
                });
                List<string> providerSpecialtyAverageList = _suspectProviders.GetProviderSpecialtyAverage(specialty);
                List<int> averageProviderdata = new List<int>();
                for (var j = 0; j < providerSpecialtyAverageList.Count; j++)
                {
                    var val = Convert.ToDecimal(providerSpecialtyAverageList[j]);
                    if (val > 1000)
                        averageProviderdata.Insert(j, Convert.ToInt32(Math.Round(Convert.ToDecimal(val) / 1000)));
                    else averageProviderdata.Insert(j, Convert.ToInt32(val));
                }

                var averageBilled = averageProviderdata[7];
                var averagePaid = averageProviderdata[8];
                var averageSavings = averageProviderdata[9];
                var averagePatients = averageProviderdata[1];
                var averageVisits = averageProviderdata[2];
                var averageClaims = averageProviderdata[3];
                var averageLines = averageProviderdata[4];
                var averageProcCodes = averageProviderdata[5];
                var averageIcdCodes = averageProviderdata[6];
                var flucPercentBilled = Convert
                    .ToInt64(Math.Round((billed - Convert.ToDecimal(providerSpecialtyAverageList[7])) /
                                        Convert.ToDecimal(providerSpecialtyAverageList[7]) * 100)).ToString();
                var flucPercentPaid = Convert
                    .ToInt64(Math.Round((paid - Convert.ToDecimal(providerSpecialtyAverageList[8])) /
                                        Convert.ToDecimal(providerSpecialtyAverageList[8]) * 100)).ToString();
                var flucPercentSavings = Convert
                    .ToInt64(Math.Round((savings - Convert.ToDecimal(providerSpecialtyAverageList[9])) /
                                        Convert.ToDecimal(providerSpecialtyAverageList[9]) * 100)).ToString();
                var flucPercentPatients =
                    Math.Round(Convert.ToDecimal(patients - Convert.ToInt64(providerSpecialtyAverageList[1])) /
                               Convert.ToInt64(providerSpecialtyAverageList[1]) * 100).ToString();
                var flucPercentVisits =
                    Math.Round(Convert.ToDecimal(visits - Convert.ToInt64(providerSpecialtyAverageList[2])) /
                               Convert.ToInt64(providerSpecialtyAverageList[2]) * 100).ToString();
                var flucPercentClaims =
                    Math.Round(Convert.ToDecimal(claims - Convert.ToInt64(providerSpecialtyAverageList[3])) /
                               Convert.ToInt64(providerSpecialtyAverageList[3]) * 100).ToString();
                var flucPercentLines =
                    Math.Round(Convert.ToDecimal(lines - Convert.ToInt64(providerSpecialtyAverageList[4])) /
                               Convert.ToInt64(providerSpecialtyAverageList[4]) * 100).ToString();
                var flucPercentProcCodes =
                    Math.Round(Convert.ToDecimal(procCodes - Convert.ToInt64(providerSpecialtyAverageList[5])) /
                               Convert.ToInt64(providerSpecialtyAverageList[5]) * 100).ToString();
                var flucPercentIcdCodes =
                    Math.Round(Convert.ToDecimal(icdCodes - Convert.ToInt64(providerSpecialtyAverageList[6])) /
                               Convert.ToInt64(providerSpecialtyAverageList[6]) * 100).ToString();
                List<string> flucRecord = new List<string>();
                flucRecord.AddRange(new List<string>
                {
                    flucPercentBilled, flucPercentPaid, flucPercentSavings, flucPercentPatients, flucPercentVisits,
                    flucPercentClaims, flucPercentLines, flucPercentProcCodes, flucPercentIcdCodes
                });
                var providerName = _suspectProviders.GetGridViewSection.GetValueInGridByColRow();
                _suspectProviders.GetGridViewSection.ClickOnGridRowByRow();
                _suspectProviders.GetRightComponentHeader()
                    .ShouldBeEqual(providerName, "The header should consist of the provider name.");
                CheckForLabelInProviderRecord(_suspectProviders);
                var providerExposureValueList = _suspectProviders.GetProviderExposureValueList()
                    .Select(s => s.Replace(",", Empty)).ToList();
                var providerExposureAverageList = _suspectProviders.GetProviderExposureAverageList();
                var fluctuationPercentageList = _suspectProviders.GetFluctuationValueList();
                CheckProviderRecordValue(_suspectProviders,providerExposureValueList, providerRecord);
                VallidateExposureAverageValue(_suspectProviders,averageBilled, averagePaid, averageSavings, averagePatients,
                    averageVisits, averageClaims, averageLines, averageProcCodes, averageIcdCodes,
                    providerExposureAverageList);
                CheckProviderFluctuationValue(fluctuationPercentageList, flucRecord);
                _suspectProviders.GetGridViewSection.ClickOnGridRowWithReviewProvider();
                _suspectProviders.IsProviderEyeIconPresent()
                    .ShouldBeTrue(
                        "Eye Icon should be shown in the exposure header for the provider which is on review.");
            }
        }



        [Test] //CAR-501

        public void Verify_Provider_Score_Card()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;
                var conditionScoreLabel = ProviderScoreEnum.ConditionScore.GetStringValue();
                var ruleEngineScoreLabel = ProviderScoreEnum.RuleEngineScore.GetStringValue();
                var billingActivityScoreLabel = ProviderScoreEnum.BillingScore.GetStringValue();
                var specialtyScoreLabel = ProviderScoreEnum.SpecialtyScore.GetStringValue();
                var geographicScoreLabel = ProviderScoreEnum.GeographicScore.GetStringValue();
                var selectedProviderSeq = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3);
                List<string> providerScoreListFromDB =
                    _suspectProviders.GetSuspectProviderScoresFromDB(selectedProviderSeq, ClientEnum.SMTST.ToString());
                string providerSpecialtyFromDB =
                    _suspectProviders.GetProviderExposureDetails(selectedProviderSeq)[1];
                StringFormatter.PrintMessageTitle(
                    "Verify whether Provider Score Card is getting displayed once the provider row is clicked.");
                var totalScoreFromDB = providerScoreListFromDB[0];
                var conditionScoreFromDB = providerScoreListFromDB[1];
                var ruleEngineScoreFromDB = providerScoreListFromDB[2];
                var billingActivityScoreFromDB = providerScoreListFromDB[3];
                var specialtyComparisonScoreFromDB = providerScoreListFromDB[4];
                var geographicScoreFromDB = providerScoreListFromDB[5];
                _suspectProviders.GetGridViewSection.ClickOnGridRowByRow();
                _suspectProviders.IsProviderScoreDisplayedInSideBar()
                    .ShouldBeTrue("Provider Score Card is displayed once the provider is selected");
                StringFormatter.PrintMessageTitle("Verify whether displayed Scores match with that from the database");
                _suspectProviders.GetScoreFromSideBar().ShouldBeEqual(totalScoreFromDB);
                _suspectProviders.GetScoreFromSideBar(conditionScoreLabel).ShouldBeEqual(conditionScoreFromDB);
                _suspectProviders.GetScoreFromSideBar(ruleEngineScoreLabel).ShouldBeEqual(ruleEngineScoreFromDB);
                _suspectProviders.GetScoreFromSideBar(billingActivityScoreLabel)
                    .ShouldBeEqual(billingActivityScoreFromDB);
                _suspectProviders.GetScoreFromSideBar(specialtyScoreLabel)
                    .ShouldBeEqual(specialtyComparisonScoreFromDB);
                _suspectProviders.GetScoreFromSideBar(geographicScoreLabel).ShouldBeEqual(geographicScoreFromDB);
                StringFormatter.PrintMessageTitle(
                    "Verify whether average score displayed in the sidebar matches with the database values");
                List<string> averageScoresListFromDB =
                    _suspectProviders.GetSuspectProviderAverageForEachScore(providerSpecialtyFromDB);
                var averageConditionScoreFromDB = averageScoresListFromDB[0];
                var averageRuleEngineScoreFromDB = averageScoresListFromDB[1];
                var averageBillingActivityScoreFromDB = averageScoresListFromDB[2];
                var averageSpecialtyComparisonScoreFromDB = averageScoresListFromDB[3];
                var averageGeographicScoreFromDB = averageScoresListFromDB[4];
                var regexForAverageScore = @"\d+[^\(\)\+\-\d%)]";
                Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(conditionScoreLabel),
                    regexForAverageScore).ToString().Trim().ShouldBeEqual(averageConditionScoreFromDB);
                Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ruleEngineScoreLabel),
                        regexForAverageScore).ToString().Trim()
                    .ShouldBeEqual(averageRuleEngineScoreFromDB);
                Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(billingActivityScoreLabel),
                        regexForAverageScore).ToString().Trim()
                    .ShouldBeEqual(averageBillingActivityScoreFromDB);
                Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(specialtyScoreLabel),
                        regexForAverageScore).ToString().Trim()
                    .ShouldBeEqual(averageSpecialtyComparisonScoreFromDB);
                Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(geographicScoreLabel),
                        regexForAverageScore).ToString().Trim()
                    .ShouldBeEqual(averageGeographicScoreFromDB);
                VerifyFluctuationPercentageForProviderScore(_suspectProviders,providerScoreListFromDB, averageScoresListFromDB);
            }
        }

        [Test] //CAR-728
        [Order(2)]
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _suspectProviders.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] //TE-615
        [Order(1)]
        [Retry(3)]

        public void Verify_Excel_Export_For_Suspect_Provider_Search_Results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "suspect_provider_export").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var sheetName = parameterList["sheetName"];
                var expectedDataList = _suspectProviders.GetExcelDataListForSuspectProviders();
                var fileName = "";
                try
                {

                    _suspectProviders.IsExportIconPresent().ShouldBeTrue("Export Icon Present?");
                    _suspectProviders.IsExportIconDisabled().ShouldBeFalse("Is Export Icon enabled?");
                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                        "All");
                    _suspectProviders.ClickOnFindButton();
                    _suspectProviders.WaitForWorkingAjaxMessage();
                    _suspectProviders.IsExportIconEnabled().ShouldBeTrue("Is Export Icon enabled?");

                    _suspectProviders.ClickOnExportIcon();
                    _suspectProviders.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Confirmation Model Displayed after clicking on export?");
                    _suspectProviders.GetPageErrorMessage()
                        .ShouldBeEqual("Suspect Provider results will be exported to Excel. Do you wish to continue?",
                            "Export message correct?");

                    StringFormatter.PrintMessage("verify on clicking cancel in confirmation model , nothing happens");
                    _suspectProviders.ClickOkCancelOnConfirmationModal(false);
                    _suspectProviders.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Is Confirmation model displayed after clicking cancel?");

                    StringFormatter.PrintMessage("verify export of provider search");
                    _suspectProviders.ClickOnExportIcon();
                    _suspectProviders.ClickOkCancelOnConfirmationModal(true);
                    _suspectProviders.WaitForStaticTime(3000);

                    fileName = _suspectProviders.GoToDownloadPageAndGetFileName();

                    ExcelReader.ReadExcelSheetValue(fileName, sheetName, 3, 3, out List<string> headerList,
                        out List<List<string>> excelExportList, out string clientName, true);

                    StringFormatter.PrintMessage("verify client name and header values");
                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                    clientName.Trim().ShouldBeEqual(ClientEnum.SMTST.GetStringValue());


                    StringFormatter.PrintMessage("verify values correct?");
                    for (int i = 0; i < expectedDataList.Count - 1; i++)
                    {
                        excelExportList[i][0].ShouldBeEqual(expectedDataList[i][0],
                            "Correct Provider Score values should be exported");
                        excelExportList[i][1].ShouldBeEqual(expectedDataList[i][1],
                            "Correct Provider name values should be exported");
                        excelExportList[i][2].ShouldBeEqual(expectedDataList[i][2],
                            "Correct Provider Sequence values should be exported");
                        excelExportList[i][3].ShouldBeEqual(expectedDataList[i][3],
                            "Correct Provider Number values should be exported");
                        excelExportList[i][4].ShouldBeEqual(expectedDataList[i][4],
                            "Correct Condition (s) values should be exported");
                        excelExportList[i][5].ShouldBeEqual(expectedDataList[i][5],
                            "Correct NPI values should be exported");
                        excelExportList[i][6].ShouldBeEqual(expectedDataList[i][6],
                            "Correct TIN values should be exported");
                        excelExportList[i][7].ShouldBeEqual(expectedDataList[i][7],
                            "Correct Specialty values should be exported");
                        excelExportList[i][8].ShouldBeEqual(expectedDataList[i][8],
                            "Correct State values should be exported");
                        excelExportList[i][9].ShouldBeEqual(expectedDataList[i][9],
                            "Correct Group Name values should be exported");
                        excelExportList[i][10].ShouldBeEqual(expectedDataList[i][10],
                            "Correct Group TIN values should be exported");

                        excelExportList[i][11].ShouldBeEqual(expectedDataList[i][11],
                            "Correct Paid values should be exported");
                        excelExportList[i][12].ShouldBeEqual(expectedDataList[i][12],
                            "Correct Trigger Date values should be exported");



                    }

                    _suspectProviders.GetProviderExportAuditListFromDB(automatedBase.EnvironmentManager.Username).ShouldContain(
                        "/api/clients/SMTST/ProviderSearchResults/DownloadSuspectProviderXLS/",
                        " Suspect provider search download audit present?");
                }
                finally
                {
                    _suspectProviders.CloseAnyPopupIfExist();
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);
                }

            }
        }

        #endregion

        #region PRIVATE METHODS

        private bool IsProviderOnReview(ProviderActionPage _providerAction,string vAction, string cAction)
        {
            bool isOnReview = false;
            _providerAction.SelectFilterConditions(3);
            for (var i = 1; i <= _providerAction.GetProviderConditionsCount(); i++)
            {
                var vactionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(vAction, i);
                var cactionSelected = _providerAction.GetProviderConditionDetailForFieldAndRow(cAction, i);
                if (vactionSelected.Equals("Review") || cactionSelected.Equals("Review") ||
                    cactionSelected.Equals("Monitor"))
                    isOnReview = true;
            }
            return isOnReview;
        }

        private void CheckFilterValues(SuspectProvidersPage _suspectProviders)
        {
            _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq").ShouldBeEqual("", "Provider Seq Should empty");
            _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN Should empty");
            _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("NPI").ShouldBeEqual("", "NPI should empty");
            _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name").ShouldBeEqual("", "Provider Full Name should empty");
            _suspectProviders.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder").ShouldBeEqual("Select one or more", "State");
            _suspectProviders.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder").ShouldBeEqual("Select one or more", "Specialty");
            _suspectProviders.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Condition", "placeholder").ShouldBeEqual("Select one or more", "Condition");
            _suspectProviders.GetDateFieldFrom("Triggered Date").ShouldBeEqual("", "Triggered Date");
        }

        private void ValidateDropDownForDefaultValueAndExpectedList(SuspectProvidersPage _suspectProviders,string label, IList<string> collectionToEqual, bool isSorted = false)
        {
            var actualDropDownList = _suspectProviders.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
            {
                actualDropDownList.RemoveAll(IsNullOrEmpty);
                actualDropDownList.Remove("All");
                actualDropDownList.Remove("Clear");
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + "Is Collection of List Expected and Equal?");
            }
            if (!isSorted)
            {

                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, actualDropDownList[0]);
            _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, actualDropDownList[1]);
            _suspectProviders.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "multiple value selected");

        }

        private void ValidateProviderSearchRowSorted(SuspectProvidersPage _suspectProviders,int col, string sortOptionName)
        {
            _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(sortOptionName);

            switch (sortOptionName)
            {
                case "Trigger Date":
                    _suspectProviders.IsListDateSortedInAscendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Ascending Order");
                    _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(sortOptionName);
                    _suspectProviders.IsListDateSortedInDescendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Descending Order");
                    break;
                case "Billed":
                case "Paid":
                case "Provider Score":
                case "Specialty":
                    _suspectProviders.IsListIntSortedInAscendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Ascending Order");
                    _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(sortOptionName);
                    _suspectProviders.IsListIntSortedInDescendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Descending Order");
                    break;
                default:
                    _suspectProviders.IsListStringSortedInAscendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Ascending Order");
                    _suspectProviders.GetGridViewSection.ClickOnFilterOptionListByFilterName(sortOptionName);
                    _suspectProviders.IsListStringSortedInDescendingOrder(col)
                        .ShouldBeTrue($"{sortOptionName} Should sorted in Descending Order");
                    break;
            }



        }

        private void VerifySearchResultForProviderRiskQuadrantSelection(SuspectProvidersPage _suspectProviders,Func<int, bool> providerScoreThresholdCondition, Func<double, bool> paidExposureThresholdCndition, string quadrantName, int quadrant = 1)
        {
            _suspectProviders.ClickOnRiskProviderQuadrantInFindPanel(quadrant);
            _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(quadrant)
                .ShouldBeTrue(Format("Is {0} Quadrant Selected?", quadrantName));
            _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
            _suspectProviders.WaitForWorkingAjaxMessage();
            _suspectProviders.WaitForStaticTime(3000);
            _suspectProviders.GetSearchResultListByCol(1).Select(int.Parse).ToList()
                .All(providerScoreThresholdCondition).ShouldBeTrue("All Value should be in " + quadrantName.Split(',')[0]);

            _suspectProviders.GetSearchResultListByCol(7)
                .Select(x => double.Parse(x, System.Globalization.NumberStyles.Currency))
                .All(paidExposureThresholdCndition).ShouldBeTrue("All Value should be in " + quadrantName.Split(',')[1]);

            var loadMoreValue = _suspectProviders.GetPagination.GetLoadMoreText();
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                .Select(m => int.Parse(m.Trim())).ToList();
            _suspectProviders.GetRiskQuadrantText(quadrant, 2)
                .ShouldBeEqual("100%", "Percentage Should be 100%");
            _suspectProviders.GetRiskQuadrantText(quadrant, 3)
                .ShouldBeEqual(numbers[1] + " Providers", "Verification of Total Provider of " + quadrantName);
        }

        public void VerifyFluctuationPercentageForProviderScore(SuspectProvidersPage _suspectProviders,List<string> providerScoreListFromDB, List<string> averageScoresListFromDB)
        {
            StringFormatter.PrintMessageTitle("Verify whether Fluctuation Percentage of scores is calculated correctly");

            var regexForScorePercentage = @"(\+|\-)\d+%";
            var calculatedPercentageConditionScore =
                _suspectProviders.CalculateFluctuationPercentage(Convert.ToInt64(providerScoreListFromDB[1]),
                    Convert.ToInt64(averageScoresListFromDB[0]));
            var calculatedRuleEngineScore =
                _suspectProviders.CalculateFluctuationPercentage(Convert.ToInt64(providerScoreListFromDB[2]),
                    Convert.ToInt64(averageScoresListFromDB[1]));
            var calculatedBillingActivityScore =
                _suspectProviders.CalculateFluctuationPercentage(Convert.ToInt64(providerScoreListFromDB[3]),
                    Convert.ToInt64(averageScoresListFromDB[2]));
            var calculatedSpecialtyScore =
                _suspectProviders.CalculateFluctuationPercentage(Convert.ToInt64(providerScoreListFromDB[4]),
                    Convert.ToInt64(averageScoresListFromDB[3]));
            var calculatedGeographicScore =
                _suspectProviders.CalculateFluctuationPercentage(Convert.ToInt64(providerScoreListFromDB[5]),
                    Convert.ToInt64(averageScoresListFromDB[4]));

            Convert.ToInt64(Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ProviderScoreEnum.ConditionScore.GetStringValue()),
                regexForScorePercentage).ToString().Replace("%", "").Trim()).Equals(calculatedPercentageConditionScore)
                .ShouldBeTrue("Calculation for Condition Score Fluctuation is correct");

            Convert.ToInt64(Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ProviderScoreEnum.RuleEngineScore
                .GetStringValue()), regexForScorePercentage).ToString().Replace("%", "").Trim()).Equals(calculatedRuleEngineScore)
                .ShouldBeTrue("Calculation for Rule Engine Score Fluctuation is correct");

            Convert.ToInt64(Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ProviderScoreEnum.BillingScore.GetStringValue()),
                regexForScorePercentage).ToString().Replace("%", "").Trim()).Equals(calculatedBillingActivityScore)
                .ShouldBeTrue("Calculation for Billing Activity Score Fluctuation is correct");

            Convert.ToInt64(Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ProviderScoreEnum.SpecialtyScore.GetStringValue()),
                regexForScorePercentage).ToString().Replace("%", "").Trim()).Equals(calculatedSpecialtyScore)
                .ShouldBeTrue("Calculation for Specialty Score Fluctuation is correct");

            Convert.ToInt64(Regex.Match(_suspectProviders.GetAverageProviderScoreFromSidebar(ProviderScoreEnum.GeographicScore.GetStringValue()),
                regexForScorePercentage).ToString().Replace("%", "").Trim()).Equals(calculatedGeographicScore)
                .ShouldBeTrue("Calculation for Geographic Score Fluctuation is correct");
        }
        private void VallidateExposureAverageValue(SuspectProvidersPage _suspectProviders,int averageBilled, int averagePaid, int averageSavings, int averagePatients, int averageVisits, int averageClaims, int averageLines, int averageProcCodes, int averageIcdCodes, List<string> providerExposureAverageList)
        {
            providerExposureAverageList[0].Contains(_suspectProviders.GetAverageValue(averageBilled)).ShouldBeTrue("First Section should contain the average per month of total billed for the last 12 months.");
            providerExposureAverageList[1].Contains(_suspectProviders.GetAverageValue(averagePaid)).ShouldBeTrue("First Section should contain the average per month of total paid for the last 12 months.");
            providerExposureAverageList[2].Contains(_suspectProviders.GetAverageValue(averageSavings)).ShouldBeTrue("First Section should contain the average per month of total savings for the last 12 months.");
            providerExposureAverageList[3].Contains(_suspectProviders.GetAverageValue(averagePatients)).ShouldBeTrue("Second Section should contain the average count of patients with at least one claim for this provider in the last 12 months.");
            providerExposureAverageList[4].Contains(_suspectProviders.GetAverageValue(averageVisits)).ShouldBeTrue("Second Section should contain the average count of total unique dates of service reported for all patients in the last 12 months.");
            providerExposureAverageList[5].Contains(_suspectProviders.GetAverageValue(averageClaims)).ShouldBeTrue("Third Section should contain the average count of total claims for this provider for all patients in the last 12 months.");
            providerExposureAverageList[6].Contains(_suspectProviders.GetAverageValue(averageLines)).ShouldBeTrue("Third Section should contain the average count of total lines reported for all patients in the last 12 months.");
            providerExposureAverageList[7].Contains(_suspectProviders.GetAverageValue(averageProcCodes)).ShouldBeTrue("Third Section should contain the average count of total unique Proc Codes reported in the last 12 months.");
            providerExposureAverageList[8].Contains(_suspectProviders.GetAverageValue(averageIcdCodes)).ShouldBeTrue("Third Section should contain the average count of total unique ICD-10 in the last 12 months.");
        }

        private void CheckProviderRecordValue(SuspectProvidersPage _suspectProviders,List<string> provRecord, List<string> recordValueInDb)
        {
            for (int i = 0; i < provRecord.Count; i++)
            {
                provRecord[i].ShouldBeEqual(recordValueInDb[i], Format("In the {0}th item Total Value for the last 12 months should be equal to {1}.", i, recordValueInDb[i]));
            }
        }
        private void CheckProviderFluctuationValue(List<string> actualRecord, List<string> recordValueInDb)
        {
            for (int i = 0; i < actualRecord.Count; i++)
            {
                actualRecord[i].Contains(recordValueInDb[i]).ShouldBeTrue(Format("In the {0}th item the fluctuation percentage for last 12 months should be equal to {1}.", i, recordValueInDb[i]));
            }
        }

        private void CheckForLabelInProviderRecord(SuspectProvidersPage _suspectProviders)
        {
            _suspectProviders.IsDollarIconPresent().ShouldBeTrue("Dollar Icon should be present in the first section.");
            _suspectProviders.IsPersonIconPresent().ShouldBeTrue("Person Icon should be present in the second section.");
            _suspectProviders.IsGraphIconPresent().ShouldBeTrue("Graph Icon should be present in the third section.");
            _suspectProviders.IsLabelPresent("Billed").ShouldBeTrue("First Section should contain the Billed Label.");
            _suspectProviders.IsLabelPresent("Paid").ShouldBeTrue("First Section should contain the Paid Label.");
            _suspectProviders.IsLabelPresent("Savings").ShouldBeTrue("First Section should contain the Savings Label.");
            _suspectProviders.IsLabelPresent("Patients").ShouldBeTrue("Second Section should contain the Patients Label.");
            _suspectProviders.IsLabelPresent("Visits").ShouldBeTrue("Second Section should contain the Visits Label.");
            _suspectProviders.IsLabelPresent("Claims").ShouldBeTrue("Third Section should contain the Claims Label.");
            _suspectProviders.IsLabelPresent("Lines").ShouldBeTrue("Third Section should contain the Lines Label.");
            _suspectProviders.IsLabelPresent("Proc Codes").ShouldBeTrue("Third Section should contain the Proc Codes Label.");
            _suspectProviders.IsLabelPresent("ICD Codes").ShouldBeTrue("Third Section should contain the ICD Codes Label.");
            //_suspectProviders.IsProviderIconPresent().ShouldBeTrue("Provider icon should be shown in the exposure header.");
            _suspectProviders.IsProviderEyeIconPresent().ShouldBeFalse("Eye Icon should not be shown in the exposure header for the provider which is not on review.");
        }

        #endregion
        #endregion

    }
}