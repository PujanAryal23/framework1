using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SuspectProvidersClient
    {
        

        /*private SuspectProvidersPage _suspectProviders;
        private CommonValidations _commonValidation;
        private ProviderActionPage _providerAction;
        private readonly string _suspectProviderPrivilige = RoleEnum.FFPAnalyst.GetStringValue();
        #endregion

        #region OVERRIDE METHODS

        /*  protected override void ClassInit()
          {
              try
              {
                  base.ClassInit();
                  _suspectProviders = QuickLaunch.NavigateToSuspectProviders();
                  _commonValidation=new CommonValidations(CurrentPage);
              }
              catch (Exception)
              {
                  if (StartFlow != null)
                      StartFlow.Dispose();
                  throw;
              }
          }

          protected override void TestInit()
          {
              base.TestInit();
              CurrentPage = _suspectProviders;
          }

          protected override void TestCleanUp()
          {
              base.TestCleanUp();

              if (Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
              {
                  _suspectProviders = _suspectProviders.Logout().LoginAsClientUser().NavigateToSuspectProviders();
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

        protected String FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }
        #endregion

        #region TEST SUITES

        [Test] //CAR-131(CAR-404) + TANT-189
        public void Verify_a_provider_risk_filter_option_in_find_provider_panel()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();

                var clientProfile = _suspectProviders.SwitchTabAndNavigateToQuickLaunchPage(true)
                    .NavigateToClientSearch();
                clientProfile.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Configuration.GetStringValue());

                var paidExposureThreshold =
                    Convert.ToDouble(
                        clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum.PaidExposureThreshold
                            .GetStringValue()));
                var providerScoreThreshold =
                    Convert.ToInt32(clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                        .ProviderScoreThreshold.GetStringValue()));
                automatedBase.CurrentPage.Logout().LoginAsClientUser();
                _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle, true);

                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a >= providerScoreThreshold,
                    a => a < paidExposureThreshold, "High Risk,Low Dollars", 1);
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a >= providerScoreThreshold,
                    a => a >= paidExposureThreshold, "High Risk,High Dollars", 2);
                VerifySearchResultForProviderRiskQuadrantSelection(_suspectProviders,a => a < providerScoreThreshold,
                    a => a < paidExposureThreshold, "Low Risk,Low Dollars", 3);
                VerifySearchResultForProviderRiskQuadrantSelection( _suspectProviders,a => a < providerScoreThreshold,
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

        [Test, Category("SmokeTestDeployment")]//TANT-91
        public void Verify_Suspect_Provider_sideBar_Pannel_and_sorting_list()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "provider_sorting_option_list_Client").Values
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
                _providerAction = _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(provseq);
                _suspectProviders = _providerAction.NavigateToSuspectProviders();

                StringFormatter.PrintMessage("Verify Load More Option");
                //var expectedRowCount = _suspectProviders.TotalCountOfClientSuspectProvidersFromDatabase();
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

        [Test] //CAR-407, CAR-375 + TANT-189
        public void Verify_provider_decision_quadrant_graphic()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                try
                {
                    var clientProfile = _suspectProviders.SwitchTabAndNavigateToQuickLaunchPage(true)
                        .NavigateToClientSearch();
                    clientProfile.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());
                    var defaultPaidExposure = 100000;
                    var defaultProviderScore = 500;

                    var paidExposureThreshold =
                        Convert.ToDouble(clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                            .PaidExposureThreshold.GetStringValue()));
                    var providerScoreThreshold =
                        Convert.ToInt32(clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                            .ProviderScoreThreshold.GetStringValue()));
                    automatedBase.CurrentPage.Logout().LoginAsClientUser();
                    _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);

                    for (var loop = 0; loop < 2; loop++)
                    {

                        _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                        _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                            "Condition",
                            "SCAC");
                        _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                            "Condition",
                            "SANT");
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

                        var score = _suspectProviders.GetSearchResultListByCol(1);
                        var prvSeq = _suspectProviders.GetSearchResultListByCol(3);
                        var paid = _suspectProviders.GetSearchResultListByCol(7)
                            .Select(x => double.Parse(x, System.Globalization.NumberStyles.Currency).ToString())
                            .ToList();



                        for (var j = 0; _suspectProviders.GetPagination.IsLoadMoreLinkable() && j < 5; j++)
                        {
                            _suspectProviders.GetPagination.ClickOnLoadMore();

                        }

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
                        _suspectProviders.WaitForWorkingAjaxMessage();
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(1)
                            .ShouldBeTrue("Is High Risk Low Dollor Rectangle Selected");
                        _suspectProviders.WaitForWorkingAjaxMessage();
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(highRiskHighDollar[0].prvSeq);
                        _suspectProviders.WaitForWorkingAjaxMessage();
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(2)
                            .ShouldBeTrue("Is High Risk High Dollor Rectangle Selected");
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(lowRiskLowDollar[0].prvSeq);
                        _suspectProviders.WaitForWorkingAjaxMessage();
                        _suspectProviders.IsQuadrantRectSelectedByQuadrant(3)
                            .ShouldBeTrue("Is Low Risk Low Dollor Rectangle Selected");
                        _suspectProviders.GetGridViewSection.ClickOnGridRowByValue(lowRiskHighDollar[0].prvSeq);
                        _suspectProviders.WaitForWorkingAjaxMessage();
                        _suspectProviders.WaitForWorkingAjaxMessage();
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

                        if (loop == 0)
                        {
                            _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToClientSearch();
                            clientProfile.SearchByClientCodeToNavigateToClientProfileViewPage(
                                ClientEnum.SMTST.ToString(), ClientSettingsTabEnum.Configuration.GetStringValue());
                            paidExposureThreshold =
                                Convert.ToDouble(clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                                    .PaidExposureThreshold.GetStringValue())) ==
                                defaultPaidExposure
                                    ? 50000
                                    : defaultPaidExposure;
                            providerScoreThreshold =
                                Convert.ToInt32(clientProfile.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum
                                    .ProviderScoreThreshold.GetStringValue())) ==
                                defaultProviderScore
                                    ? 400
                                    : defaultProviderScore;

                            clientProfile.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.PaidExposureThreshold.GetStringValue(),
                                paidExposureThreshold.ToString());
                            clientProfile.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.ProviderScoreThreshold.GetStringValue(),
                                providerScoreThreshold.ToString());
                            clientProfile.GetSideWindow.Save();
                            automatedBase.CurrentPage.Logout().LoginAsClientUser();
                            _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
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
                            _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToClientSearch();
                            clientProfile.SearchByClientCodeToNavigateToClientProfileViewPage(
                                ClientEnum.SMTST.ToString(), ClientSettingsTabEnum.Configuration.GetStringValue());
                            clientProfile.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.PaidExposureThreshold.GetStringValue(),
                                defaultPaidExposure.ToString());
                            clientProfile.SetInputTextBoxValueByLabel(
                                ConfigurationSettingsEnum.ProviderScoreThreshold.GetStringValue(),
                                defaultProviderScore.ToString());
                            clientProfile.GetSideWindow.Save();
                            automatedBase.CurrentPage.Logout().LoginAsClientUser();
                            _suspectProviders.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle, true);
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

        [Test]//CAR-350
        public void Verify_sorting_of_provider_search_result_for_different_sort_options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper
                        .GetMappingData(FullyQualifiedClassName, "provider_sorting_option_list_internal").Values
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
                    _suspectProviders.IsListStringSortedInDescendingOrder(1)
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


        [Test] //CAR-276
        [Retrying(Times = 3)]
        public void Validate_Security_And_Navigation_Of_Suspect_Provider_Page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                CommonValidations _commonValidation=new CommonValidations(automatedBase.CurrentPage);

                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Provider,
                    new List<string> { SubMenu.SuspectProviders, SubMenu.SuspectProvidersWorkList },
                    RoleEnum.FFPAnalyst.GetStringValue(),
                    new List<string>
                    {
                        PageHeaderEnum.SuspectProviders.GetStringValue(), PageHeaderEnum.ProviderAction.GetStringValue()
                    }, automatedBase.Login.LoginAsClientUserWithNoAnyAuthorityAndRedirectToQuickLaunch,
                    new[] { "uiautomation", "noauthority" }, automatedBase.Login.LoginAsClientUser);
            }
        }

        /// <summary>
        /// X=Required,R=Review,D=Deny,Q=Monitor,O=Record Review
        /// </summary>
        [Test] //CAR-313 and CAR-319 + TE-563
        public void Verify_display_provider_result_set()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var lockBy = paramLists["LockBy"];
                var prvSeq = paramLists["PrvSeq"];

                try
                {
                    _suspectProviders.DeleteProviderLockByProviderSeq(prvSeq);
                    _suspectProviders.Logout().LoginAsAppealClientUser().NavigateToSuspectProviders();

                    _suspectProviders.GetGridListValueforScore().ShouldCollectionBeSorted(true,
                        "Suspect Provider results should be sorted by provider score in descending order");
                    StringFormatter.PrintMessageTitle("Verify Score Level for different range");
                    Enumerable.Range(900, 1000)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.HIGH))
                        .ShouldBeTrue("High Score Level should be in range of 900-100 and should be RED");
                    Enumerable.Range(700, 899)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.ELEVATED))
                        .ShouldBeTrue("Elevated Score Level should be in range of 900-100 and should be ORANGE");
                    Enumerable.Range(500, 699)
                        .Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.MODERATE))
                        .ShouldBeTrue("Moderate Score Level should be in range of 900-100 MODERATE");
                    Enumerable.Range(0, 499).Contains(_suspectProviders.GetProviderScoreByScoreLevel(ScoreBandEnum.LOW))
                        .ShouldBeTrue("Low Score Level should be in range of 900-100 GREEN");

                    StringFormatter.PrintMessageTitle(
                        "Verify Search is executed while landing on Provider Search page.");

                    var expectedRowCount = _suspectProviders.TotalCountOfClientSuspectProvidersFromDatabase();
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
                    _suspectProviders.SearchByProviderSequence(prvSeq);
                    _suspectProviders.IsProviderLockIconPresent(prvSeq)
                        .ShouldBeFalse("Is lock icon present initially?");




                    automatedBase.CurrentPage = _providerAction =
                        _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                    var providerName = _providerAction.GetProviderName();
                    prvSeq = _providerAction.GetProviderSequence();
                    var suspectConditionList = _providerAction.GetConditionIdListInProviderConditions()
                        .Select(x => x.Split('-')[0].Trim()).ToList();
                    var paid = _providerAction.GetProviderExposureCountValue(4, 1).Replace(",", "");
                    var state = _providerAction.GetProviderDetailByLabel("State");
                    var specialty = _providerAction.GetProviderDetailByLabel("Specialty").Split('-')[0].Trim();
                    var triggeredDate = _providerAction.GetTriggeredDaeInProviderConditions();
                    var tin = _providerAction.GetProviderDetailByLabel("TIN");
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;


                    StringFormatter.PrintMessageTitle("Verify Provider lock functionality");
                    _suspectProviders.IsLockIconPresentAndGetLockIconToolTipMessage(prvSeq, pageUrl, false)
                        .ShouldBeEqual($"The provider is currently being viewed by {lockBy}",
                            "Provider Icon is locked and Tooltip Message");
                    automatedBase.CurrentPage = _providerAction =
                        _suspectProviders.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                    _providerAction.IsProviderOpenedInViewMode().ShouldBeTrue("Is provider opened in view mode");
                    _providerAction.ClickOnSearchIconAtHeader();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                        PageHeaderEnum.SuspectProviders.GetStringValue(),
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
                        .ShouldBeEqual(specialty, "Is Specialty Equal?");
                    specialty.Length.ShouldBeEqual(2, "Value of specialty should equal to 2 digit only");
                    _suspectProviders.GetGridViewSection.GetValueInGridByColRow(10)
                        .ShouldBeEqual(triggeredDate, "Is Triggered Date Equals?");
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



                    var list = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(6).Split(',')
                        .Select(x => x.Trim()).ToList();
                    list.Count.ShouldBeGreater(1, "Suspect Condition should be in comma separated");
                    list.IsInAscendingOrder()
                        .ShouldBeTrue("Suspect Condition Should be in ascending order");


                    StringFormatter.PrintMessage("Verification of NPI Primary Data Point");
                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
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

        [Test] //CAR-40
        public void Verify_Suspect_Provider_WorkList()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                const int rowValuetoPress = 3;
                const int rowToLockfromDb = 2;
                string prvseqtoLock = null;
                try
                {
                    _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                    _suspectProviders.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                        "All");
                    _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
                    _suspectProviders.WaitForWorkingAjaxMessage();

                    var providerSeqListFromGrid = _suspectProviders.GetGridViewSection.GetGridListValueByCol(3)
                        .Skip(rowValuetoPress - 1).ToList();

                    _providerAction = _suspectProviders.NavigateToProviderAction(() =>
                        _suspectProviders.GetGridViewSection.ClickOnGridByRowCol(rowValuetoPress));

                    VerifyConditionOnProviderWorkList(_providerAction,automatedBase.CurrentPage, _providerAction.IsWorkListProviderSequenceConsistent,
                        "Does WorkList Order Retain the Filter Panel Result Order and Provider Sequence Change on pressing Next",
                        verificiationList: providerSeqListFromGrid);

                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                        PageHeaderEnum.SuspectProviders.GetStringValue(),
                        "WorkList should return to Suspect Provider Page when the user reaches the end of the list");

                    StringFormatter.PrintMessageTitle(
                        "Verifying if a provider is locked, the provider is opened in read only ");
                    prvseqtoLock = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3, rowToLockfromDb);
                    _providerAction.LockProviderFromDB(prvseqtoLock, "uiautomation_2");
                    _suspectProviders.GetGridViewSection.ClickOnGridByRowCol(rowToLockfromDb);

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

        [Test] //CAR-501
        public void Verify_Provider_Score_Card()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var conditionScoreLabel = ProviderScoreEnum.ConditionScore.GetStringValue();
                var ruleEngineScoreLabel = ProviderScoreEnum.RuleEngineScore.GetStringValue();
                var billingActivityScoreLabel = ProviderScoreEnum.BillingScore.GetStringValue();
                var specialtyScoreLabel = ProviderScoreEnum.SpecialtyScore.GetStringValue();
                var geographicScoreLabel = ProviderScoreEnum.GeographicScore.GetStringValue();
                var selectedProviderSeq = _suspectProviders.GetGridViewSection.GetValueInGridByColRow(3);
                List<string> providerScoreListFromDB =
                    _suspectProviders.GetSuspectProviderScoresFromDB(selectedProviderSeq, ClientEnum.SMTST.ToString());
                string providerSpecialtyFromDB = _suspectProviders.GetProviderExposureDetails(selectedProviderSeq)[1];
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
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
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


        [Test]
        public void Verify_Suspect_Provider_Name_Filters_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var providerfirstname = testData["firstname"];
                var providerlastname = testData["lastname"];
                _suspectProviders.GetSideBarPanelSearch.OpenSidebarPanel();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    Concat(Enumerable.Repeat("a1@_B", 21)));
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    Concat(Enumerable.Repeat("a1@_B", 21)));
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name").Length
                    .ShouldBeEqual(100, "Provider Full Name should allow upto 100 characters.");
                _suspectProviders.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name").Length
                    .ShouldBeEqual(100, "Provider Full Name should allow upto 100 characters.");
                _suspectProviders.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("verify message displayed for no matching record");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", providerfirstname);
                _suspectProviders.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEquivalent(
                    _suspectProviders.FullOrFacilityNameForClient(providerfirstname), "verfiy facility name equal?");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name", providerlastname);
                _suspectProviders.ClickOnFindButton();
                _suspectProviders.WaitForWorkingAjaxMessage();
                _suspectProviders.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEquivalent(
                    _suspectProviders.FullOrFacilityNameForClient(providerlastname, "lastname"),
                    "verfiy full or facility name");
                _suspectProviders.GetSideBarPanelSearch.ClickOnClearLink();
                _suspectProviders.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();

            }
        }

        [NonParallelizable]
        [Test] //TE-615
        public void Verify_Excel_Export_For_Suspect_Provider_Search_Results()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                SuspectProvidersPage _suspectProviders;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _suspectProviders = automatedBase.QuickLaunch.NavigateToSuspectProviders();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "suspect_provider_export").Values
                    .ToList();
                var parameterList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var sheetName = parameterList["sheetName"];
                var expectedDataList = _suspectProviders.GetExcelDataListForSuspectProvidersForClientUser();

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

                    var fileName = _suspectProviders.GoToDownloadPageAndGetFileName();


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

                }

            }
        }

        #endregion
        #region PRIVATE METHODS
        private void VerifySearchResultForProviderRiskQuadrantSelection(SuspectProvidersPage _suspectProviders,Func<int, bool> providerScoreThresholdCondition, Func<double, bool> paidExposureThresholdCndition, string quadrantName, int quadrant = 1)
        {
            _suspectProviders.ClickOnRiskProviderQuadrantInFindPanel(quadrant);
            _suspectProviders.IsQuadrantRectSelectedByQuadrantInFindPanel(quadrant)
                .ShouldBeTrue(Format("Is {0} Quadrant Selected?", quadrantName));
            _suspectProviders.GetSideBarPanelSearch.ClickOnFindButton();
            _suspectProviders.WaitForWorkingAjaxMessage();
            _suspectProviders.WaitForStaticTime(2000);
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


        private void VerifyConditionOnProviderWorkList(ProviderActionPage _providerAction,NewDefaultPage CurrentPage, Func<ProviderActionPage, string, bool> requiredCondition, string message, int numberOfNext = 5, List<string> verificiationList = null)
        {
            int count = 0;
            if (verificiationList != null)
                numberOfNext = verificiationList.Count;
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

        private void VerifyProviderDecisionQuadrantLogic(SuspectProvidersPage _suspectProviders,string quadrantName, string quadrantText, int quadrantProviderCount, int totalProviderCount, int quadrant = 1)
        {
            _suspectProviders.GetRiskQuadrantText(quadrant, 1)
                .ShouldBeEqual(quadrantText, "Verification of Quadrants Label of " + quadrantName);
            _suspectProviders.GetRiskQuadrantText(quadrant, 2)
                .ShouldBeEqual(Math.Round(quadrantProviderCount * 100.0 / totalProviderCount, 0, MidpointRounding.AwayFromZero) + "%", "Verification of Percentage Label of " + quadrantName);
            _suspectProviders.GetRiskQuadrantText(quadrant, 3)
                .ShouldBeEqual(quadrantProviderCount + " Providers", "Verification of Total Provider of " + quadrantName);
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

        #endregion
        
    }

}
