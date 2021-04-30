using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ProviderSearch
    {
        #region PRIVATE FIELDS
        //private  ProviderSearchPage _providerSearch;
        //private NewProviderScoreCardPage _newProviderScoreCard;
        //private ProfileManagerPage _profileManager;
        //private List<string> _specialtyDropdownList;
        //private List<string> _stateDropdownList;
        //private IDictionary<string, string> _riskScoreBand;
        //private ProviderActionPage _providerAction;
        //private Int16 _suspectProviderCount;
        //private Int16 _cotivitiFlaggedProviderCount;
        //private Int16 _clientFlaggedProviderCount;
        //private List<string> _conditionIDList;

        //private List<string> _clientActionList;
        //private DashboardPage dashboardPage;
        //private CommonValidations _commonValidation;
        #endregion

        #region PROTECTED PROPERTIES
        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();               
        //        CurrentPage = _providerSearch = QuickLaunch.NavigateToProviderSearch();
        //        _riskScoreBand = DataHelper.GetMappingData(FullyQualifiedClassName, "RiskScoreBand");
        //        _clientActionList = DataHelper.GetMappingData(FullyQualifiedClassName, "client_action_list").Values.ToList();
        //        _commonValidation = new CommonValidations(CurrentPage);

        //        try
        //        {
        //            RetrieveListFromDatabase();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
                
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}

        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    CurrentPage = _providerSearch;
        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _providerSearch.CloseDbConnection();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}

        //protected override void TestCleanUp()
        //{
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _providerSearch = _providerSearch.Logout().LoginAsHciAdminUser().NavigateToProviderSearch();
        //    }

        //    if (_providerSearch.GetPageHeader() != PageHeaderEnum.ProviderSearch.GetStringValue())
        //    {

        //        _providerSearch = CurrentPage.NavigateToProviderSearch();
        //    }
        //    //CurrentPage.NavigateToProviderSearch();
        //    _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
        //    _providerSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();
        //    //CurrentPage.ClickOnQuickLaunch().NavigateToProviderSearch();
        //    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderEnum.AllProviders.GetStringValue());
        //    base.TestCleanUp();
        //}
        //#endregion

        //#region DBinteraction methods
        //private void RetrieveListFromDatabase()
        //{
        //    _specialtyDropdownList = _providerSearch.GetSpecialtyList();
        //    _stateDropdownList = _providerSearch.GetStateList();
        //    _conditionIDList = _providerSearch.GetConditionIDList();

        //}
        //#endregion

       #region TEST SUITES

        [Test]//US69120
        [Retry(3)]
        public void Verify_security_and_navigation_of_the_Provider_search_page() 
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage); 
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Provider,
                    new List<string> { SubMenu.ProviderSearch },
                    RoleEnum.FFPAnalyst.GetStringValue(), new List<string> { PageHeaderEnum.ProviderSearch.GetStringValue() },
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] { "Test4", "Automation4" });
            }
        }

        [Test, Category("SmokeTestDeployment")] // TANT-90
        public void Verify_Sidebar_Panel_and_Navigation_To_Provider_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "provider_sorting_option_list_internal").Values
                        .ToList();
                StringFormatter.PrintMessage(
                    "Verify Open or Close of Providers Panel and navigation to provider action page");
                _providerSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Side Bar panel should be opened by default");
                _providerSearch.IsSideBarIconPresent().ShouldBeTrue("Sideabar icon should be present");
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.SuspectProviders.GetStringValue());
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
                _providerSearch = _providerAction.NavigateToProviderSearch();

                _providerSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _providerSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("Sidebar Panel on sidebar is hidden when toggle button is clicked.");

                StringFormatter.PrintMessage("Verify Sorting options displayed ");
                _providerSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                _providerSearch.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Provider Results", "Correct tooltip is displayed");

                StringFormatter.PrintMessage("Verification of Filter options");
                _providerSearch.GetFilterOptionList().ShouldCollectionBeEqual(filterOptions,
                    "Sort Provider Results icon: When selected, will expand list of Sort options");
            }
        }


        [Test]//US69133+US69435(Removed the Last Name and First Name filters and verified Provider Full Name filter
        public void Verify_All_Providers_Quick_search_option_and_filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                List<string> _stateDropdownList = _providerSearch.GetStateList();
                List<string> _specialtyDropdownList = _providerSearch.GetSpecialtyList(); 
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                Verify_correct_search_filter_options_displayed_for("all_providers",
                    ProviderEnum.AllProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ProviderEnum.AllProviders.GetStringValue(),
                        "Quick Search option defaults to All Providers");

                _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty")
                    .ShouldBeFalse("Is Specialty  visible when State value is not selected?");


                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", paramLists["NumberOnly"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Provider Seq")
                    .ShouldBeEqual(15, "Field should allow only 15 digits");
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateNumericOnlyField("Provider Seq", "Only numbers allowed.", "0!2a",_providerSearch);


                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", paramLists["NumberOnly"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("TIN")
                    .ShouldBeEqual(10, "Field should allow only 10 digits");
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateNumericOnlyField("TIN", "Only numbers allowed.", "0!2a",_providerSearch);


                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("NPI",
                    paramLists["AlphanumericSpecialcharacter"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("NPI")
                    .ShouldBeEqual(50, "Field should allow only 50 alpha numeric digits as well as special characters");


                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider #",
                    paramLists["AlphanumericSpecialcharacter"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Provider #")
                    .ShouldBeEqual(100,
                        "Field should allow only 100 alpha numeric digits as well as special characters");

                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    paramLists["AlphanumericSpecialcharacter"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Provider First Name")
                    .ShouldBeEqual(100, "Field should allow only 100 characters");


                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    paramLists["AlphanumericSpecialcharacter"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Provider Last Name")
                    .ShouldBeEqual(100, "Field should allow only 100 characters");

                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name",
                    paramLists["AlphanumericSpecialcharacter"]);
                _providerSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Provider Full Name")
                    .ShouldBeEqual(100,
                        "Field should allow only 100 alpha numeric digits as well as special characters");

                _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty")
                    .ShouldBeFalse("Specialty should only be visible when State value is selected");



                StringFormatter.PrintMessageTitle("State Dropdown verification");
                ValidateFieldSupportingMultipleValues("State", _stateDropdownList,_providerSearch);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                    paramLists["State"]);

                _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty")
                    .ShouldBeTrue("Specialty should only be visible when State value is selected");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder")
                    .ShouldBeEqual("Select one or more", "Specialty");

                StringFormatter.PrintMessageTitle("Specialty Dropdown verification");
                var _specialtyDropdownListWithSpace = _specialtyDropdownList
                    .Select(s => s.Remove(s.IndexOf("-"), 1).Insert(s.IndexOf("-"), " - ")).ToList();
                ValidateFieldSupportingMultipleValues("Specialty", _specialtyDropdownListWithSpace,_providerSearch);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                    paramLists["Specialty"]);
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error message should be present");
                _providerSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Search cannot be initiated without any criteria entered.");
                _providerSearch.ClosePageError();
            }

        }

        [Test]//US69506
        public void Verify_additional_Quick_search_filters_for_various_Quick_Search_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                List<string> _conditionIDList = _providerSearch.GetConditionIDList();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ProviderEnum.AllProviders.GetStringValue(),
                        "Quick Search option defaults to All Providers");

                Verify_correct_search_filter_options_displayed_for("all_providers",
                    ProviderEnum.AllProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);
                Verify_correct_search_filter_options_displayed_for("quick_search_options",
                    ProviderEnum.SuspectProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);
                //_newProviderSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder").ShouldBeEqual("Select one or more", "Specialty");

                Verify_correct_search_filter_options_displayed_for("client_flagged_filters",
                    ProviderEnum.ClientFlaggedProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);

                Verify_correct_search_filter_options_displayed_for("quick_search_options",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);

                StringFormatter.PrintMessageTitle("Condition Dropdown verification");
                ValidateFieldSupportingMultipleValues("Condition", _conditionIDList,_providerSearch);


                StringFormatter.PrintMessageTitle("Triggered Data validation");
                ValidateDateRangePickerBehaviour("Triggered Date",_providerSearch);
                ValidateFieldErrorMessageForDateRange("Triggered Date",
                    "Search cannot be initiated for date ranges greater than 3 months.",_providerSearch);
            }

        }

        [Test] //TE-377
        public void Verify_First_Name_And_Last_Name_Search_Filters_Result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var firstNames = paramLists["First_Name"].Split(',');
                var lastNames = paramLists["Last_Name"].Split(',');

                StringFormatter.PrintMessageTitle(
                    "Verify Correct Results Are Displayed For All Providers Quick Search Option");
                var resultsFromDatabase =
                    _providerSearch.GetAllProvidersNameFromDataBase(firstNames[0], lastNames[0]);
                Verify_Correct_Results_Displayed_For(ProviderEnum.AllProviders.GetStringValue(), firstNames[0],
                    lastNames[0], resultsFromDatabase,_providerSearch);

                StringFormatter.PrintMessageTitle("Verify Results For Cotiviti Flagged Providers");

                var resultsFromDatabaseForCotivitiFlaggedProviders =
                    _providerSearch.GetCotivitiFlaggedProvidersNameFromDataBase(firstNames[0], lastNames[0]);
                Verify_Correct_Results_Displayed_For(ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),
                    firstNames[0], lastNames[0], resultsFromDatabaseForCotivitiFlaggedProviders,_providerSearch);

                StringFormatter.PrintMessageTitle("Verify Results For Suspect Providers");
                var resultsFromDatabaseForSuspectProviders =
                    _providerSearch.GetSuspectProvidersNameFromDataBase(firstNames[1], lastNames[1]);
                Verify_Correct_Results_Displayed_For(ProviderEnum.SuspectProviders.GetStringValue(), firstNames[1],
                    lastNames[1], resultsFromDatabaseForSuspectProviders,_providerSearch);

                StringFormatter.PrintMessageTitle("Verify Results For Client Flagged Providers");
                var resultFromDataBaseForClientFlaggedProviders =
                    _providerSearch.GetClientFlaggedProvidersNameFromDataBase(firstNames[0], lastNames[0]);
                Verify_Correct_Results_Displayed_For(ProviderEnum.ClientFlaggedProviders.GetStringValue(),
                    firstNames[0], lastNames[0], resultFromDataBaseForClientFlaggedProviders,_providerSearch);

                StringFormatter.PrintMessageTitle("Verify No Records Are Found When Incorrect Names Entered");
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", firstNames[2]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name", lastNames[2]);
                _providerSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerSearch.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No matching record found Message");
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }

        }

        [Test] //US69506
        public void Verify_that_default_value_and_the_clear_filters_clears_all_filters_suspect_providers_quick_search_option() 
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                //Check for the default values when in the Provider Search page
                //_newProviderSearch = CurrentPage.ClickOnQuickLaunch().NavigateToProviderSearch();
                _providerSearch = automatedBase.CurrentPage.NavigateToProviderSearch();

                StringFormatter.PrintMessageTitle(
                    "Verify default input value for Suspect Providers Quick Search option");
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.SuspectProviders.GetStringValue(), false);
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq")
                    .ShouldBeEqual("", "Provider Seq Should empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN Should empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("NPI").ShouldBeEqual("", "NPI should empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider #")
                    .ShouldBeEqual("", "Provider # should empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name")
                    .ShouldBeEqual("", "Provider First Name should Empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name")
                    .ShouldBeEqual("", "Provider Last Name should Empty");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name")
                    .ShouldBeEqual("", "Provider Full Name should empty");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder")
                    .ShouldBeEqual("Select one or more", "State");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder")
                    .ShouldBeEqual("Select one or more", "Specialty");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Condition", "placeholder")
                    .ShouldBeEqual("Select one or more", "Condition");
                _providerSearch.GetDateFieldFrom("Triggered Date").ShouldBeEqual("", "Triggered Date");

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears all values in the search criteria except for Quick Search");
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", paramLists["ProviderSeq"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", paramLists["TIN"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("NPI", paramLists["NPI"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider #", paramLists["Provider#"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name",
                    paramLists["ProviderFirstName"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name",
                    paramLists["ProviderLastName"]);
                _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name",
                    paramLists["ProviderFullName"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                    paramLists["State1"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State",
                    paramLists["State2"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                    paramLists["Specialty1"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                    paramLists["Specialty2"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Condition",
                    paramLists["Condition1"]);
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Condition",
                    paramLists["Condition2"]);
                _providerSearch.SetDateFieldFrom("Triggered Date", DateTime.Now.ToString("MM/d/yyyy"));
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();


                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search value in the dropdown");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ProviderEnum.SuspectProviders.GetStringValue(), "Quick Search");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq")
                    .ShouldBeEqual("", "Provider Seq");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("NPI").ShouldBeEqual("", "NPI");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider #")
                    .ShouldBeEqual("", "Provider #");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name")
                    .ShouldBeEqual("", "Provider First Name");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name")
                    .ShouldBeEqual("", "Provider Last Name");
                _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name")
                    .ShouldBeEqual("", "Provider Full Name");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder")
                    .ShouldBeEqual("Select one or more", "State");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder")
                    .ShouldBeEqual("Select one or more", "Specialty");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Condition", "placeholder")
                    .ShouldBeEqual("Select one or more", "Condition");
                _providerSearch.GetDateFieldFrom("Triggered Date").ShouldBeEqual("", "Triggered Date");
                _providerSearch.GetDateFieldTo("Triggered Date").ShouldBeEqual("", "Triggered Date");
            }

        }

        [Test,Category("SmokeTestDeployment")]//TANT-90
        public void Verify_search_results_are_correct_for_other_Quick_Search_options_except_All_Providers_smokeTest()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                string[] ProvidersStringValue =
                {
                    ProviderEnum.SuspectProviders.GetStringValue(),
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),
                    ProviderEnum.ClientFlaggedProviders.GetStringValue()
                };

                StringFormatter.PrintMessage(
                    "Verifying the suspect providers, cotiviti flagged providers and client flagged providers");

                for (int j = 0; j < 3; j++)
                {
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ProvidersStringValue[j], false);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorking();
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                        paramLists["Specialty" + (j + 1)]);
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Condition",
                        paramLists["Condition" + (j + 1)]);
                    _providerSearch.GetSideBarPanelSearch.SetDateField("Triggered Date",
                        paramLists["TriggeredDate" + (j + 1)], 1);
                    _providerSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "Search results displayed for" + ProvidersStringValue[j]);
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }
            }


        }


        [Test]//US69506
        public void Verify_search_results_are_correct_for_other_Quick_Search_options_except_All_Providers()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                Int16 _suspectProviderCount;
                Int16 _cotivitiFlaggedProviderCount;
                Int16 _clientFlaggedProviderCount;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                _suspectProviderCount = Convert.ToInt16(_providerSearch.TotalCountOfSuspectProvidersFromDatabase());
                _cotivitiFlaggedProviderCount =
                    Convert.ToInt16(_providerSearch.TotalCountOfCotivitiFlaggedProvidersFromDatabase());
                _clientFlaggedProviderCount =
                    Convert.ToInt16(_providerSearch.TotalCountOfClientFlaggedProvidersFromDatabase());
                int[] countValuesFromDB =
                    {_suspectProviderCount, _cotivitiFlaggedProviderCount, _clientFlaggedProviderCount};
                string[] ProvidersStringValue =
                {
                    ProviderEnum.SuspectProviders.GetStringValue(),
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),
                    ProviderEnum.ClientFlaggedProviders.GetStringValue()
                };

                StringFormatter.PrintMessage(
                    "Verifying the suspect providers, cotiviti flagged providers and client flagged providers from the database");

                for (int j = 0; j < countValuesFromDB.Length; j++)
                {
                    Verify_the_Provider_count_with_database(ProvidersStringValue[j], countValuesFromDB[j],_providerSearch);
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                        paramLists["Specialty" + (j + 1)]);
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Condition",
                        paramLists["Condition" + (j + 1)]);
                    _providerSearch.GetSideBarPanelSearch.SetDateField("Triggered Date",
                        paramLists["TriggeredDate" + (j + 1)], 1);
                    _providerSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "Search results displayed for" + ProvidersStringValue[j]);
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }
            }


        }

        [Test] //TE-644 + //TE-669
        public void Verify_Both_Active_And_Inactive_Results_Are_Shown_When_Condition_And_Triggered_Date_Filters_Are_Applied()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var inactiveProviderSequence = paramLists["InactiveProviderSequence"].Split(',').ToList();
                string[] ProvidersStringValue =
                {
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),
                    ProviderEnum.ClientFlaggedProviders.GetStringValue()
                };
                for (int j = 1; j <= ProvidersStringValue.Length; j++)
                {
                    try
                    {
                        StringFormatter.PrintMessageTitle(
                            "Verify Search Result Yields both Active and Inactive condition results when Condition and Triggered Date Filters Are Applied");
                        _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                            ProvidersStringValue[j - 1], false);
                        _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                            "Condition", paramLists["Condition" + j]);
                        _providerSearch.GetSideBarPanelSearch.SetDateField("Triggered Date",
                            paramLists["TriggeredDate" + j], 1);
                        if (j == 1)
                            _providerSearch.UpdateActiveStatus('N');
                        else
                            _providerSearch.UpdateActiveStatus('N', true);
                        _providerSearch.ClickOnFindButton();
                        _providerSearch.WaitForWorkingAjaxMessage();
                        _providerSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(
                            (j == 1)
                                ? _providerSearch.TotalCountOfCotivitiFlaggedProvidersByTriggeredDateFromDatabase()
                                : _providerSearch.TotalCountOfClientFlaggedProvidersByTriggeredDateFromDatabase(),
                            "Search Result should yield both Active and Inactive condition results");

                        StringFormatter.PrintMessageTitle(
                            "Verify Search Result Does Not Show Inactive Provider Results ");
                        _providerSearch.ClickOnClearLink();
                        _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq",
                            inactiveProviderSequence[j - 1]);
                        _providerSearch.ClickOnFindButton();
                        _providerSearch.WaitForWorkingAjaxMessage();
                        _providerSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                            .ShouldBeTrue("No Matching Record Found Message Should Be Present");

                    }
                    finally
                    {
                        if (j == 1)
                            _providerSearch.UpdateActiveStatus();
                        else
                            _providerSearch.UpdateActiveStatus('Y', true);
                        _providerSearch.ClickOnClearLink();
                    }
                }
            }
        }
    

        [Test]
        public void Verify_that_default_value_and_the_clear_filters_clears_all_filters_all_providers_quick_search_option() //US69133
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            //Check for the default values when in the Provider Search page
            _providerSearch = automatedBase.CurrentPage.ClickOnQuickLaunch().NavigateToProviderSearch();

            StringFormatter.PrintMessageTitle("Verify default input value for All Providers Quick Search option");
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderEnum.AllProviders.GetStringValue(), false);//check for type ahead functionality
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq").ShouldBeEqual("", "Provider Seq Should empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN Should empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("NPI").ShouldBeEqual("", "NPI should empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider #").ShouldBeEqual("", "Provider # should empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name").ShouldBeEqual("", "Provider First Name should Empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name").ShouldBeEqual("", "Provider Last Name should Empty");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name").ShouldBeEqual("", "Provider Full Name should empty");            
            _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder").ShouldBeEqual("Select one or more", "State");

            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", paramLists["ProviderSeq"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN",paramLists["TIN"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("NPI", paramLists["NPI"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider #", paramLists["Provider#"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", paramLists["ProviderFirstName"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name", paramLists["ProviderLastName"]);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", paramLists["ProviderFullName"]);            
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State", paramLists["State1"]);
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State", paramLists["State2"]);            
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty", paramLists["Specialty1"]);
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty", paramLists["Specialty2"]);
            _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();


            StringFormatter.PrintMessageTitle("Verify Clear Filter clears does not clear Quick Search value in the dropdown");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").
                ShouldBeEqual(ProviderEnum.AllProviders.GetStringValue(), "Quick Search");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Seq").ShouldBeEqual("", "Provider Seq");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("TIN").ShouldBeEqual("", "TIN");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("NPI").ShouldBeEqual("", "NPI");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider #").ShouldBeEqual("", "Provider #");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider First Name").ShouldBeEqual("", "Provider First Name");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Last Name").ShouldBeEqual("", "Provider Last Name");
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Full Name").ShouldBeEqual("", "Provider Full Name");            
            _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder").ShouldBeEqual("Select one or more", "State");
            _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty").
                ShouldBeFalse("Specialty should only be visible when State value is selected");
            }

        }
        
        [Test]//US69263 + //US69458 + //CAR-21
        public void Verify_search_result_are_correct()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                IDictionary<string, string> _riskScoreBand = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "RiskScoreBand"); ;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var lockBy = paramLists["LockedBy"];
                var prvSeqOnReview = paramLists["PrvSeqOnReview"];
                var prvSeqWithLowModerateElevatedHighScoreList =
                    new[]
                    {
                        paramLists["PrvSeqWithLowScore"], paramLists["PrvSeqWithModerateScore"],
                        paramLists["PrvSeqWithElevatedScore"], paramLists["PrvSeqWithHighScore"]
                    };
                var scoreBandEnumList =
                    new[] {ScoreBandEnum.LOW, ScoreBandEnum.MODERATE, ScoreBandEnum.ELEVATED, ScoreBandEnum.HIGH};
                var riskClassList = new[] {"low_risk", "moderate_risk", "elevated_risk", "high_risk"};
                var vAction = paramLists["vAction"];
                var cAction = paramLists["cAction"];
                var prvseq = paramLists["Prvseq"];
                var npi = paramLists["NPI"];
                var tin = paramLists["TIN"];
                var providerID = paramLists["ProviderID"];
                var providerName = paramLists["ProviderName"];
                var specialty = paramLists["Specialty"];
                var state = paramLists["State"];
                try
                {
                     _providerSearch.DeleteProviderLockByProviderSeq(prvSeqOnReview);
                    automatedBase.CurrentPage.Logout().LoginAsHciAdminUser1().NavigateToProviderSearch();
                    StringFormatter.PrintMessageTitle(
                        "Verify No Search executed while landing on Provider Search page.");
                    _providerSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(0,
                        "When landing on the Provider Search page, no result must be executed.");
                    _providerSearch.GetGridViewSection.IsNoDataMessagePresentInLeftSection()
                        .ShouldBeTrue("Is No data message displayed?");
                    StringFormatter.PrintMessageTitle("Validation of default sorting");
                    _providerSearch.GetSideBarPanelSearch
                        .SelectSearchDropDownListForMultipleSelectValue("State", "All");
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.WaitForStaticTime(500);
                    _providerSearch.GetProviderScoreList().ShouldCollectionBeSorted(true,
                        "Search result must be sorted by Provider Score in descending order by default.");
                    StringFormatter.PrintMessageTitle("Validation of Provider On Review");
                    _providerSearch.SearchByProviderSequence(prvSeqOnReview);
                    _providerSearch.IsProviderLockIconPresent(prvSeqOnReview)
                        .ShouldBeFalse("Is lock icon present initially?");
                    _providerSearch.IsReviewIconByRowPresent().ShouldBeTrue("Is Review icon/eye-ball icon displayed?");
                    _providerSearch.GetReviewIconTooltip().ShouldBeEqual("Provider currently on review.",
                        "Correct On Review tooltip is displayed.");
                    automatedBase.CurrentPage = _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqOnReview);
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    IsProviderOnReview(vAction, cAction,_providerAction).ShouldBeTrue(
                        "The provider has at least one condition where Cotiviti Action= Review or Client Action= Review or Monitor.");

                    StringFormatter.PrintMessageTitle("Verify Provider lock functionality");
                    _providerSearch.IsLockIconPresentAndGetLockIconToolTipMessage(prvSeqOnReview, pageUrl)
                        .ShouldBeEqual(string.Format("The provider is currently being viewed by {0}", lockBy),
                            "Provider Icon is locked and Tooltip Message");
                    automatedBase.CurrentPage = _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeqOnReview);
                    _providerAction.IsProviderOpenedInViewMode().ShouldBeTrue("Is provider opened in view mode");
                    // //CAR-21
                    StringFormatter.PrintMessageTitle(
                        "Verify that Search Icon at header should return to provider search");
                    _providerAction.ClickOnSearchIconAtHeader();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                        "Clicking on Search Icon on header should return to provider search");
                    _providerSearch.CloseNewTabIfExists();
                    StringFormatter.PrintMessageTitle("Verify Provider Score for different score band");
                    for (var j = 0; j < scoreBandEnumList.Length; j++)
                    {
                        _providerSearch.SearchByProviderSequence(prvSeqWithLowModerateElevatedHighScoreList[j]);
                        _providerSearch.IsAppropriateScoreIconPresent(riskClassList[j])
                            .ShouldBeTrue(
                                "Is appropriate score icon displayed for score between" + scoreBandEnumList[j]);
                        VerifyScore(scoreBandEnumList[j],_riskScoreBand,_providerSearch);
                    }

                    StringFormatter.PrintMessageTitle("Verify search results");
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvseq);
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("NPI", npi);
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("TIN", tin);
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider #", providerID);
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", providerName);
                    _providerSearch.GetSideBarPanelSearch
                        .SelectSearchDropDownListForMultipleSelectValue("State", state);
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                        specialty);
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(3)
                        .ShouldBeEqual(prvseq, "Provider Sequence");
                    _providerSearch.GetGridViewSection.GetLabelInGridByColRow(3)
                        .ShouldBeEqual("Prov Seq:", "Correct Provider Sequence Label is displayed.");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(4).ShouldBeEqual(npi, "NPI");
                    _providerSearch.GetGridViewSection.GetLabelInGridByColRow(4).ShouldBeEqual("NPI:");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(5).ShouldBeEqual(tin, "TIN");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(5).Length
                        .ShouldBeEqual(9, "9 digit Provider TIN value is displayed");
                    _providerSearch.GetGridViewSection.GetLabelInGridByColRow(5).ShouldBeEqual("TIN:");
                    _providerSearch.GetGridViewSection.GetLabelInGridByColRow(6).ShouldBeEqual("Sp:");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(6)
                        .ShouldBeEqual("20", "Specialty");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(7).ShouldBeEqual("OH", "State");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(7).Length
                        .ShouldBeEqual(2, "2 digit state value is displayed");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow(6).Length
                        .ShouldBeEqual(2, "2 digit specialty is displayed");
                    _providerSearch.GetGridViewSection.GetValueInGridByColRow()
                        .ShouldBeEqual(providerName, "Provider Name");
                    StringFormatter.PrintMessageTitle("Verify search can be initiated with any search criteria");
                    SearchBySingleValue("TIN", tin, _providerSearch,true);
                    SearchBySingleValue("NPI", npi, _providerSearch,true);
                    SearchBySingleValue("Provider #", providerID, _providerSearch,true);
                    SearchBySingleValue("Provider Full Name", providerName, _providerSearch,true);
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch
                        .SelectSearchDropDownListForMultipleSelectValue("State", "All");
                    _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Specialty",
                        "07 - Dermatology");
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "Search by State and Specialty only");
                    _providerSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Does Error Pop up appear on searching by State and Specialty");

                    StringFormatter.PrintMessageTitle("Verify search result matching partial Provider Full Name");
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name",
                        providerName.Substring(0, 2));
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.WaitForStaticTime(500);
                    _providerSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0,
                        "Result displayed for seawrch with even partial matches to Provider Full Name");
                    _providerSearch.GetProviderNameList().ConvertAll(str => str.ToLower())
                        .All(x => x.Contains(providerName.Substring(0, 2).ToLower()))
                        .ShouldBeTrue("Is Expected Provider Display?");

                }
                finally
                {
                    _providerSearch.CloseNewTabIfExists();
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }
            }
        }
      

        [Test]//US69380 + //US69702 +US69796
        public void Verify_Provider_details_values()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var prvSeq = paramLists["Prvseq"];
                var prvID = paramLists["ProviderID"];
                var groupName = paramLists["GroupName"];
                var groupTIN = paramLists["GroupTIN"];
                try
                {
                    _providerSearch.SearchByProviderSequence(prvSeq);
                    _providerSearch.IsSelectedProviderRowHighlighted(prvSeq)
                        .ShouldBeFalse("Initailly row Should not be hightlighted");
                    _providerSearch.ClickOnSearchListRowByProviderSequence(prvSeq);
                    _providerSearch.IsSelectedProviderRowHighlighted(prvSeq)
                        .ShouldBeTrue("The selected provider row should be highlighted");
                    _providerSearch.GetProviderDetailHeader()
                        .ShouldBeEqual("Provider Details", "Is Header of Detail Section Matches?");
                    _providerSearch.GetProviderDetailsValueByLabel("Prov #")
                        .ShouldBeEqual(prvID, "Is Provider# detail Equals?");
                    _providerSearch.GetProviderDetailsValueByLabel("Group")
                        .ShouldBeEqual(groupName, "Is Group Name Equals");
                    _providerSearch.GetProviderDetailsValueByLabel("Group TIN")
                        .ShouldBeEqual(groupTIN, "Is Group TIN Equals");

                    StringFormatter.PrintMessage("Verify selected is record highlighted");
                    _providerSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _providerSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ProviderEnum.SuspectProviders.GetStringValue());
                    _providerSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    var prvSeq1 = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3);
                    _providerSearch.ClickOnSearchListRowByProviderSequence(prvSeq1);
                    _providerSearch.IsSelectedProviderRowHighlighted(prvSeq1)
                        .ShouldBeTrue("The selected provider row 1 should be highlighted");
                    var prvSeq2 = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3, 2);
                    _providerSearch.ClickOnSearchListRowByProviderSequence(prvSeq2);
                    _providerSearch.IsSelectedProviderRowHighlighted(prvSeq2)
                        .ShouldBeTrue("The selected provider row 2 should be highlighted");
                    _providerSearch.IsSelectedProviderRowHighlighted(prvSeq1)
                        .ShouldBeFalse("Row 1 should not be highlighted when another row is selected.");

                    _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ProviderEnum.AllProviders.GetStringValue());
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("State", "All");
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _providerSearch.WaitForStaticTime(500);
                    var prvNo1 = _providerSearch.GetProviderDetailsValueByLabel("Prov #");
                    _providerSearch.GetGridViewSection.ClickOnGridRowByRow(2);
                    _providerSearch.WaitForStaticTime(500);
                    var prvNo2 = _providerSearch.GetProviderDetailsValueByLabel("Prov #");
                    prvNo1.ShouldNotBeEqual(prvNo2, "Clicking on grid row must display corresponding data");


                    //StringFormatter.PrintMessage("Validation of Provider History popup");
                    //ProviderClaimHistoryPage _providerClaimHistory = ValidateProviderClaimHistoryByRow(1,_providerSearch);
                    //_providerSearch = _providerClaimHistory.CloseProviderClaimHistoryAndSwitchToProviderSearch();

                    //_providerClaimHistory = ValidateProviderClaimHistoryByRow(2,_providerSearch);
                    //_providerSearch = _providerClaimHistory.CloseProviderClaimHistoryAndSwitchToProviderSearch();


                }
                finally
                {
                    _providerSearch.CloseAllExistingPopupIfExist();
                }
            }
        }
      

        [Test]//US69623
        public void Verify_sorting_of_provider_search_result_for_different_sort_options()
         {
             using (var automatedBase = new NewAutomatedBaseParallelRun())
             {
                 ProviderSearchPage _providerSearch;
                 automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;


                 try
                 {

                     _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                         ProviderEnum.ClientFlaggedProviders.GetStringValue());
                     _providerSearch.ClickOnFindButton();
                     _providerSearch.WaitForWorkingAjaxMessage();
                     _providerSearch.GetGridViewSection.GetGridRowCount()
                         .ShouldBeGreater(0, "Search Result must be listed.");
                     _providerSearch.GetProviderScoreList().ShouldCollectionBeSorted(true,
                         "Search result must be sorted by Provider Score in descending order by default.");

                     StringFormatter.PrintMessageTitle("Validation of Sorting by Provider Score");
                     _providerSearch.ClickOnFilterOptionListRow(1);
                     _providerSearch.GetProviderScoreList().ShouldCollectionBeSorted(false,
                         "Search result must be sorted by Provider Score in ascending");
                     _providerSearch.ClickOnFilterOptionListRow(1);
                     _providerSearch.GetProviderScoreList().ShouldCollectionBeSorted(true,
                         "Search result must be sorted by Provider Score in descending");

                     StringFormatter.PrintMessageTitle("Validation of Sorting by Provider on Review");
                     _providerSearch.ClickOnFilterOptionListRow(2);
                     _providerSearch.GetProviderOnReviewList()
                         .IsInDescendingOrder()
                         .ShouldBeTrue("Provider Search Row should be sorted by presence of Review icon ");
                     _providerSearch.ClickOnFilterOptionListRow(2);
                     _providerSearch.GetProviderOnReviewList()
                         .IsInAscendingOrder()
                         .ShouldBeTrue("Provider Search Row should sorted by absence of Review icon ");
                     ValidateProviderSearchRowSorted(2, 3, "Provider Name",_providerSearch);
                     ValidateProviderSearchRowSorted(3, 4, "Provider Sequence",_providerSearch);
                     ValidateProviderSearchRowSorted(4, 5, "NPI",_providerSearch);
                     ValidateProviderSearchRowSorted(5, 6, "TIN",_providerSearch);
                     ValidateProviderSearchRowSorted(6, 7, "Specialty",_providerSearch);
                     _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                         ProviderEnum.AllProviders.GetStringValue());
                     _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("State", "All");
                     _providerSearch.ClickOnFindButton();
                     _providerSearch.WaitForWorkingAjaxMessage();
                     ValidateProviderSearchRowSorted(7, 8, "State",_providerSearch);

                     _providerSearch.ClickOnFilterOptionListRow(9);
                     StringFormatter.PrintMessage("Clicked on Clear Sort.");
                     _providerSearch.GetProviderScoreList().ShouldCollectionBeSorted(true,
                         "All sorting is cleared and search list is sorted by Provider Score which is default sort.");


                 }
                 finally
                 {
                     _providerSearch.ClickOnFilterOptionListRow(9);
                     StringFormatter.PrintMessage("Clicked on Clear Sort.");
                 }
             }

         }

        [Test,Category("SmokeTestDeployment")]//US69705
        public void Verify_provider_scorecard_popup_upon_clicking_the_provider_score_in_the_search_results_grid()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();
                Verify_the_scorecard_popup_opens_for_the_correct_providerseq(1,automatedBase.CurrentPage,_providerSearch);
                Verify_the_scorecard_popup_opens_for_the_correct_providerseq(2,automatedBase.CurrentPage,_providerSearch);
            }
        }

        [Test]//US69725
        public void Verify_provider_notes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeqNoNotes = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ProviderSequence", "Value");
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.CotivitiFlaggedProviders.GetStringValue());
                _providerSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();

                var prvseq = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3);
                var totalProviderNoteCount = Convert.ToInt32(_providerSearch.TotalCountofProviderNotes(prvseq));
                _providerSearch.GetGridViewSection.ClickOnGridRowByRow(1);
                _providerSearch.IsProviderNotesIconPresent().ShouldBeTrue("Provider note icon should be present");
                _providerSearch.GetProviderNotesIconTooltip().ShouldBeEqual("Provider Notes");
                _providerSearch.ClickOnProviderNotesIcon();
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerSearch.IsProviderNoteContainerPresent()
                    .ShouldBeTrue("Provider Notes container should be visible upon clicking the provider notes icon");
                _providerSearch.GetNoteListCount().ShouldBeEqual(totalProviderNoteCount,
                    "Notes list should display only provider notes");
                _providerSearch.IsCarrotIconPresentInAllProviderNotes()
                    .ShouldBeTrue("Carrot Icon should be present in all Provider notes");
                _providerSearch.GetNoteAttributeValueListInTheNoteContainer(2)
                    .ShouldNotContain("Claim", "Only Provider Note type should be visible");
                _providerSearch.GetNoteAttributeValueListInTheNoteContainer(5).Select(DateTime.Parse).ToList()
                    .IsInDescendingOrder().ShouldBeTrue("The order of note list should be in descending order");
                _providerSearch.ClickOnCarrotIconOnProviderNotes();
                _providerSearch.ClickOnExpandCarrotIconOnProviderNotes();
                _providerSearch.ClickOnProviderNotesIcon();
                _providerSearch.GetProviderDetailHeader().ShouldBeEqual("Provider Details",
                    "Provider Notes section should toggel to Provider details");
                StringFormatter.PrintMessage("Verify No notes available message");
                _providerSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ProviderEnum.AllProviders.GetStringValue());
                _providerSearch.SetInputFieldByInputLabel("Provider Seq", prvSeqNoNotes);
                _providerSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();

                _providerSearch.GetGridViewSection.ClickOnGridRowByRow(1);
                _providerSearch.ClickOnProviderNotesIcon();
                _providerSearch.GetNoNotesMessage().ShouldBeEqual("There are no notes available.",
                    "No Note message should be displayed");
                _providerSearch.ClickOnProviderNotesIcon();
            }
        }


        [Test, Category("OnDemand")] //US69728
        public void Verify_Provider_Flagging_Search_filter_and_results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var prvSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ProviderSequence", "Value");
                var expectedProviderFlaggingList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Provider_Flagging_List").Values.ToList();
                string label = "";
                string providerAlertIndicatorOptionLabel =
                    ConfigurationSettingsEnum.ProviderAlertIndicatorOption.GetStringValue();

                var _newClientSearch = _providerSearch.NavigateToClientSearch();

                try
                {
                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());

                    label = _newClientSearch.GetInputTextBoxValueByLabel(ConfigurationSettingsEnum.ProviderAlertLabel
                        .GetStringValue());

                    if (_newClientSearch.IsRadioButtonOnOffByLabel(providerAlertIndicatorOptionLabel))
                    {
                        _newClientSearch.ClickOnRadioButtonByLabel(providerAlertIndicatorOptionLabel, false);
                        _newClientSearch.GetSideWindow.Save();
                        _newClientSearch.IsRadioButtonOnOffByLabel(providerAlertIndicatorOptionLabel)
                            .ShouldBeFalse($"Is {providerAlertIndicatorOptionLabel} Selected?");
                    }

                    automatedBase.CurrentPage = _providerSearch = _newClientSearch.NavigateToProviderSearch();
                    Verify_Medaware_dropdown_for_quick_search_options(label, false,_providerSearch);
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ProviderEnum.AllProviders.GetStringValue());
                    SearchBySingleValue("State", "All", _providerSearch,true, true);
                    _providerSearch.IsMedawareIconPresent().ShouldBeFalse(
                        $"Provider profile alert icon should not be shown in search result when {providerAlertIndicatorOptionLabel} client setting is disabled.");

                    _newClientSearch = _providerSearch.NavigateToClientSearch();
                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());

                    _newClientSearch.ClickOnRadioButtonByLabel(providerAlertIndicatorOptionLabel);
                    _newClientSearch.GetSideWindow.Save();
                    _newClientSearch.IsRadioButtonOnOffByLabel(providerAlertIndicatorOptionLabel)
                        .ShouldBeTrue($"Is {providerAlertIndicatorOptionLabel} Selected?");

                    automatedBase.CurrentPage = _providerSearch = _newClientSearch.NavigateToProviderSearch();

                    Verify_Medaware_dropdown_for_quick_search_options(label, true,_providerSearch);

                    var actualDropDownList = _providerSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.ShouldCollectionBeEqual(expectedProviderFlaggingList,
                        label + " List As Expected");
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel(label)
                        .ShouldBeEqual("All", label + " value defaults to All");

                    _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ProviderEnum.AllProviders.GetStringValue());
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "Yes");
                    _providerSearch.ClickOnFindButton();

                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.GetMedwareIconTooltip().ShouldBeEqual(label, "Correct tooltip is displayed");
                    _providerSearch.IsMedawareIconPresent()
                        .ShouldBeTrue("All Provider profile icon should contain alert icon.");

                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "No");
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Full Name", "Doctor");
                    _providerSearch.ClickOnFindButton();

                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.IsMedawareIconPresent()
                        .ShouldBeFalse(
                            "When the provider is not flagged, provider profile alert icon must not be shown.");

                    StringFormatter.PrintMessage("Verify alert icon after toggling provider flagging");
                    _providerSearch.SearchByProviderSequence(prvSeq);
                    var isAlert = _providerSearch.IsMedawareIconPresent();
                    automatedBase.CurrentPage = _providerAction =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvSeq);
                    _providerAction.ClickOnCheckBoxProviderFlagging();
                    _providerAction.IsConfirmationPopupModalPresent().ShouldBeTrue("Modal is present.");
                    _providerAction.ClickOkCancelOnConfirmationModal(true); //ok button clicked
                    automatedBase.CurrentPage = _providerSearch = _providerAction.NavigateToProviderSearch();
                    if (isAlert)
                        _providerSearch.IsMedawareIconPresent()
                            .ShouldBeFalse("Is Provider Profile Alert icon present?");
                    else
                        _providerSearch.IsMedawareIconPresent()
                            .ShouldBeTrue("Is Provider Profile Alert  icon present?");
                }
                finally
                {
                    automatedBase.CurrentPage.NavigateToClientSearch().SearchByClientCodeToNavigateToClientProfileViewPage(
                        ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());

                    if (!_newClientSearch.IsRadioButtonOnOffByLabel(providerAlertIndicatorOptionLabel))
                    {
                        _newClientSearch.ClickOnRadioButtonByLabel(providerAlertIndicatorOptionLabel);
                        _newClientSearch.GetSideWindow.Save();
                    }

                    automatedBase.CurrentPage = _providerSearch = _newClientSearch.NavigateToProviderSearch();
                }
            }
        }

        [Test] //US69805
        public void Verify_Client_Action_search_filter_and_results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                List<string> _clientActionList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "client_action_list").Values.ToList(); ;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                Verify_correct_search_filter_options_displayed_for("client_flagged_filters",
                    ProviderEnum.ClientFlaggedProviders.GetStringValue(),_providerSearch,automatedBase.DataHelper);

                _providerSearch.ClickOnClearLink();
                // _newProviderSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderEnum.ClientFlaggedProviders.GetStringValue());

                ValidateSingleDropDownForDefaultValueAndExpectedList("Client Action", _clientActionList,_providerSearch);

                foreach (Enum action in Enum.GetValues(typeof(ClientActionEnum)))
                {
                    _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client Action",
                        action.GetStringValue());
                    _providerSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    Verify_the_Provider_count_with_database(ProviderEnum.ClientFlaggedProviders.GetStringValue(),
                        action.ToString().Equals("All")
                            ? Convert.ToInt16(_providerSearch.TotalCountOfClientFlaggedProvidersFromDatabase())
                            : Convert.ToInt16(
                                _providerSearch
                                    .TotalCountOfClientFlaggedProvidersByActionFromDatabase(action.ToString())),_providerSearch,false);

                    _providerSearch.ClickOnClearLink();
                    _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel("Client Action").ShouldBeEqual("All",
                        "Clicking on clear link clears the current value in CLient Action dropdown and displays default value");
                }

            }
        }

        [Test] //CAR-40
        [Retrying(Times = 3)]
        public void Verify_Provider_WorkList_On_Provider_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerAction;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                const int rowValuetoPress = 6;
                const int rowToLockfromDb = 2;
                string prvseqtoLock = null;
                try
                {
                    _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                    _providerSearch.GetSideBarPanelSearch
                        .SelectSearchDropDownListForMultipleSelectValue("State", "All");
                    _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();

                    var providerSeqListFromGrid = _providerSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .Skip(rowValuetoPress - 1).ToList();

                    _providerAction = _providerSearch.NavigateToProviderAction(() =>
                        _providerSearch.GetGridViewSection.ClickOnGridByRowCol(rowValuetoPress));


                    VerifyConditionOnProviderWorkList(_providerAction.IsWorkListProviderSequenceConsistent,
                        "Does WorkList Order Retain the Filter Panel Result Order and Provider Sequence Change on pressing Next",
                        verificiationList: providerSeqListFromGrid, CurrentPage:automatedBase.CurrentPage,_providerAction:_providerAction);

                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                        "WorkList should return to Provider Search Page when the user reaches the end of the list");

                    StringFormatter.PrintMessageTitle(
                        "Verifying if a provider is locked, the provider is opened in read only ");
                    prvseqtoLock = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3, rowToLockfromDb);

                    _providerAction.LockProviderFromDB(prvseqtoLock, "uiautomation_2");
                    _providerSearch.GetGridViewSection.ClickOnGridByRowCol(rowToLockfromDb);

                    _providerAction.GetProviderLockIconToolTip().Contains("This provider has been opened in view mode.")
                        .ShouldBeTrue("Provider should be opened in View Mode when Locked");

                    _providerAction.IsNextIconDisabled()
                        .ShouldBeFalse("Next Icon should not be disabled when provider is locked");


                }
                finally
                {
                    _providerSearch.DeleteProviderLockByProviderSeq(prvseqtoLock);
                }
            }
        }

        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    "Suspect Providers");
                _providerSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _providerSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] //TE-609
        public void Verify_Excel_Export_For_Provider_Search_Results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                automatedBase.CurrentPage = _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "provider_export").Values
                    .ToList();
                var parameterList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var sheetName = parameterList["sheetName"];
                var prvSeq = parameterList["Prvseq"];
                var expectedDataList = _providerSearch.GetExcelDataList(prvSeq);
                var fileName = "";
                try
                {
                    _providerSearch.RefreshPage();
                    _providerSearch.IsExportIconPresent().ShouldBeTrue("Export Icon Present?");
                    _providerSearch.IsExportIconDisabled().ShouldBeTrue("Is Export Icon disabled?");
                    _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
                    _providerSearch.ClickOnFindButton();
                    _providerSearch.WaitForWorkingAjaxMessage();
                    _providerSearch.IsExportIconEnabled().ShouldBeTrue("Is Export Icon enabled?");

                    _providerSearch.ClickOnExportIcon();
                    _providerSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Confirmation Model Displayed after clicking on export?");

                    StringFormatter.PrintMessage("verify on clicking cancel in confirmation model , nothing happens");
                    _providerSearch.ClickOkCancelOnConfirmationModal(false);
                    _providerSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Is Confirmation model displayed after clicking cancel?");

                    StringFormatter.PrintMessage("verify export of provider search");
                    _providerSearch.ClickOnExportIcon();
                    _providerSearch.ClickOkCancelOnConfirmationModal(true);
                    _providerSearch.WaitForStaticTime(3000);

                    fileName = _providerSearch.GoToDownloadPageAndGetFileName();

                    ExcelReader.ReadExcelSheetValue(fileName, sheetName, 3, 3, out List<string> headerList,
                        out List<List<string>> excelExportList, out string clientName, true);

                    StringFormatter.PrintMessage("verify client name and header values");
                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                    clientName.Trim().ShouldBeEqual(ClientEnum.SMTST.GetStringValue());

                    StringFormatter.PrintMessage("verify values correct?");
                    excelExportList[0].ShouldCollectionBeEqual(expectedDataList[0], "values equal?");
                    _providerSearch.GetProviderExportAuditListFromDB(automatedBase.EnvironmentManager.Username).ShouldContain(
                        "/api/clients/SMTST/ProviderSearchResults/DownloadProviderXLS/",
                        "provider search download audit present?");
                }
                finally
                {
                    _providerSearch.CloseAnyPopupIfExist();
                    ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);
                }
            }
        }

        


      

        #endregion

        #region PRIVATE METHODS
        private void VerifyConditionOnProviderWorkList(Func<ProviderActionPage, string, bool> requiredCondition, string message,NewDefaultPage CurrentPage, ProviderActionPage _providerAction, int numberOfNext = 5, List<string> verificiationList = null)
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


        private void Verify_Medaware_dropdown_for_quick_search_options(string label, bool present,ProviderSearchPage _providerSearch)
        {
            _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
            foreach (ProviderEnum option in Enum.GetValues(typeof(ProviderEnum)))
            {
                //_newProviderSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", option.GetStringValue());
                if (present)
                    _providerSearch.IsProviderFlaggingDropdownPresent(label).
                        ShouldBeTrue(label + " dropdown should  be present when Enable Provider Flagging client setting is selected for Quick Search option : "+ option.GetStringValue());
                else
                    _providerSearch.IsProviderFlaggingDropdownPresent(label).
                        ShouldBeFalse(label + " dropdown should not be present when Enable Provider Flagging client setting is not selected for Quick Search option : " + option.GetStringValue());
            }


        }
        private void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue, ProviderSearchPage _providerSearch,IDataHelper DataHelper)
        {

            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
            _providerSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(
                        DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                        true);
            var allprovd = quickSearchValue.Equals(ProviderEnum.AllProviders.GetStringValue()) ;
            if (allprovd)
            {
                StringFormatter.PrintMessageTitle(
                    "Verify Specialty is displayed without State selection for Quick Search option other than All Providers");
                _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty").ShouldBeFalse("Specialty should not be displayed when " + ProviderEnum.AllProviders.GetStringValue() +
                                  " is selected.");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder")
                    .ShouldBeEqual("Select one or more", "State");
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("If "+quickSearchValue+ "is selected without any other input criteria entered, State must be selected for search");
                _providerSearch.ClosePageError();
                _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("State", "Alaska");
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("For" + quickSearchValue + " if State has been selected, search should be executed without error pop up");
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
            else
            {
                
                StringFormatter.PrintMessageTitle("Verify Specialty is displayed without State selection for Quick Search option other than All Providers");
                _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("State", "placeholder").ShouldBeEqual("Select one or more", "State");
                _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Specialty").ShouldBeTrue("Specialty should be displayed when " + quickSearchValue + "is selected.");
                _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Specialty", "placeholder").ShouldBeEqual("Select one or more", "Specialty");
                
                if(quickSearchValue.Equals(ProviderEnum.ClientFlaggedProviders.GetStringValue()))                                   
                    _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Client Action").ShouldBeTrue("Client Action should be displayed when " + quickSearchValue + "is selected.");                
                else
                    _providerSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Client Action").ShouldBeFalse("Client Action should not be displayed when " + quickSearchValue + "is selected.");

                //_newProviderSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.WaitForWorkingAjaxMessage();
                _providerSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("For" + quickSearchValue + ", State should not be selected for executing search and there should not be any pop up.");
                
            }

        }

        private void ValidateNumericOnlyField(string label, string message, string character, ProviderSearchPage _providerSearch)
        {
            _providerSearch.SetInputFieldByInputLabel(label, character);

            _providerSearch.GetPageErrorMessage()
                .ShouldBeEqual(message,
                    "Popup Error message is shown when unexpected " + character + "is passed to " + label);
            _providerSearch.ClosePageError();
        }

        private void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, ProviderSearchPage _providerSearch)
        {
            var listedOptionsList = _providerSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
            listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            _providerSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                .ShouldBeTrue(
                    "A value of all displayed at the top of the list");

            listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected , followed by options sorted alphabetically.");

            listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
            
            
        }
        private void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList, ProviderSearchPage _providerSearch)
        {
            ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList,_providerSearch);
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
            _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
            _providerSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
            _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, ProviderSearchPage _providerSearch, bool order = true)
        {
            var actualDropDownList = _providerSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            //if (order)
            //{
            //    actualDropDownList.Remove("All");
            //    actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            //}
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            //_newProviderSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _providerSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
        }
        private void ValidateDateRangePickerBehaviour(string label, ProviderSearchPage _providerSearch)
        {
            _providerSearch.GetDateFieldPlaceholder(label, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (from) default value");
            _providerSearch.GetDateFieldPlaceholder(label, 2).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (to) default value");
            _providerSearch.SetInputFieldByInputLabel(label, DateTime.Now.ToString("MM/d/yyyy")); //check numeric value can be typed
            _providerSearch.GetDateFieldFrom(label).ShouldBeEqual(DateTime.Now.ToString("MM/d/yyyy"), label + " Checks numeric value is accepted");
            _providerSearch.SetInputFieldByInputLabel(label, "");
            _providerSearch.SetDateFieldFrom(label, DateTime.Now.ToString("MM/d/yyyy"));
            _providerSearch.GetDateFieldTo(label).
                ShouldBeEqual(_providerSearch.GetDateFieldFrom(label), label + " From value populated in To field as well.");

             _providerSearch.SetDateFieldTo(label, DateTime.Now.Subtract(new TimeSpan(24, 0, 0)). ToString("MM/d/yyyy"));
            _providerSearch.GetPageErrorMessage().ShouldBeEqual("Please enter a valid date range.");
            _providerSearch.ClosePageError();
           
        }

        private void ValidateFieldErrorMessageForDateRange(string fieldName, string message, ProviderSearchPage _providerSearch)
        {
            _providerSearch.SetDateFieldFrom(fieldName, DateTime.Now.ToString("MM/d/yyyy"));
            _providerSearch.SetDateFieldTo(fieldName, DateTime.Now.AddMonths(3).AddDays(1).ToString("MM/d/yyyy"));
            _providerSearch.GetFieldErrorIconTooltipMessage(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", fieldName));
            _providerSearch.ClickOnFindButton();
            _providerSearch.GetPageErrorMessage();                   
            _providerSearch.ClosePageError();
            _providerSearch.ClearAll();
        }
        

        private void VerifyScore(ScoreBandEnum riskScoreBandEnum, IDictionary<string, string> _riskScoreBand, ProviderSearchPage _providerSearch)
        {
            string[] score;
            int scoreLowerLimit = 900;
            int scoreUpperLimit = 1000;

            switch (riskScoreBandEnum)
            {
                case ScoreBandEnum.HIGH:
                    {
                        score = _riskScoreBand[Enum.GetName(typeof(ScoreBandEnum), ScoreBandEnum.HIGH)].Split('-');
                        int.TryParse(score[0], out scoreLowerLimit);
                        int.TryParse(score[1], out scoreUpperLimit);
                    }
                    break;
                case ScoreBandEnum.ELEVATED:
                    {
                        score = _riskScoreBand[Enum.GetName(typeof(ScoreBandEnum), ScoreBandEnum.ELEVATED)].Split('-');
                        int.TryParse(score[0], out scoreLowerLimit);
                        int.TryParse(score[1], out scoreUpperLimit);
                    }
                    break;
                case ScoreBandEnum.MODERATE:
                    {
                        score = _riskScoreBand[Enum.GetName(typeof(ScoreBandEnum), ScoreBandEnum.MODERATE)].Split('-');
                        int.TryParse(score[0], out scoreLowerLimit);
                        int.TryParse(score[1], out scoreUpperLimit);
                    }
                    break;
                case ScoreBandEnum.LOW:
                    {
                        score = _riskScoreBand[Enum.GetName(typeof(ScoreBandEnum), ScoreBandEnum.LOW)].Split('-');
                        int.TryParse(score[0], out scoreLowerLimit);
                        int.TryParse(score[1], out scoreUpperLimit);
                    }
                    break;
            }

            int providerScoreFromGrid = Convert.ToInt16(_providerSearch.GetProviderScoreByRow(1));
            Console.WriteLine("Expected Score Range : {0} - {1}  Actual Score : {2}", scoreLowerLimit, scoreUpperLimit, providerScoreFromGrid);
            if ((providerScoreFromGrid < scoreLowerLimit) || (providerScoreFromGrid > scoreUpperLimit))
                Assert.Fail("Provider Score do not match the search criteria");

        }

        private bool IsProviderOnReview(string vAction, string cAction, ProviderActionPage _providerAction)
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

        private void SearchBySingleValue(string label, string value, ProviderSearchPage _providerSearch, bool verifyPopUp = false, bool multiselect = false)
        {
            _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
            if (multiselect) _providerSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, value);
            else _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, value);
            _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerSearch.WaitForWorkingAjaxMessage();
            _providerSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, string.Format("Search by {0} only with value - {1}", label, value));
            if (verifyPopUp)
                _providerSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("Does Pop Up Appear on Searching with criteria?");
        }

        private void Verify_the_Provider_count_with_database(string quickSearchMappingOption, int providerCount, ProviderSearchPage _providerSearch,bool search = true)
        {
            if (search)
            {
                _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    quickSearchMappingOption, false);
                _providerSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerSearch.WaitForWorking();
            }
            if (providerCount > 25)
            {
                
                _providerSearch.ClickOnLoadMore();
                var loadMoreValue = _providerSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                numbers[1].ShouldBeEqual(providerCount, quickSearchMappingOption + "provider count match");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (providerCount,
                        "For " + quickSearchMappingOption +
                        "count less than 50, clicking on Load more should equal the " + quickSearchMappingOption +
                        " count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }
            }
            else
            {
                _providerSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(providerCount, "Provider Counts Should Match");
            }

        }

        private void ValidateProviderSearchRowSorted(int col, int sortOptionRow, string colName, ProviderSearchPage _providerSearch)
        {
            _providerSearch.ClickOnFilterOptionListRow(sortOptionRow);
            switch (colName)
            {
                case "Provider Sequence":
                    _providerSearch.IsListIntSortedInAscendingOrder(col)
                   .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _providerSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _providerSearch.IsListIntSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;                

                default:
                    _providerSearch.IsListStringSortedInAscendingOrder(col)
                   .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _providerSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _providerSearch.IsListStringSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                    break;

            }
        }

        private ProviderClaimHistoryPage ValidateProviderClaimHistoryByRow(int row, ProviderSearchPage _providerSearch)
        {
            var prvSeq = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3, row);
            _providerSearch.GetGridViewSection.ClickOnGridRowByRow(row);
           
            if (_providerSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
            {
                _providerSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            }

            _providerSearch.GetProviderHistoryIconTooltip().ShouldBeEqual("View Provider Claim History", "Correct toottip over History icon is displayed");
            _providerSearch.IsProviderHistoryIconPresent().ShouldBeTrue("Is Provider History icon present?");

            var _providerClaimHistory = _providerSearch.ClickOnProviderHistoryIcon(prvSeq);
            _providerClaimHistory.CurrentPageTitle.ShouldBeEqual(PageTitleEnum.ClaimHistoryPopup.GetStringValue());
            _providerClaimHistory.IsTwevleMonthButtonSelected().ShouldBeTrue("12 Months radio button should be selected by default");

            var prvSeqInHistory = _providerClaimHistory.GetProviderSequenceFromProviderClaimHistoryPage().Split(',')[0];
            prvSeqInHistory.ShouldBeEqual(prvSeq, "Provider History popup of corresponding Provider Sequence: " + prvSeq + " is displayed");
            _providerClaimHistory.GetTwevleMonthsProviderHistory(prvSeqInHistory).
                ShouldBeEqual(_providerClaimHistory.GetTotalDataCount(), "Provider Claim History count is verified against database.");

            return _providerClaimHistory;
        }

        private void Verify_the_scorecard_popup_opens_for_the_correct_providerseq(int rowValue,NewDefaultPage CurrentPage, ProviderSearchPage _providerSearch)
        {
            NewProviderScoreCardPage _newProviderScoreCard;
            string prvSeq;

            if (rowValue == 1)
            { prvSeq = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3); }
            else
            { prvSeq = _providerSearch.GetGridViewSection.GetValueInGridByColRow(3, 2); }

            _newProviderScoreCard = _providerSearch.ClickOnGridScoreToOpenScoreCard(rowValue);
            var scoreCardURL = _newProviderScoreCard.GetScorecardPopUpCurrentUrl();
            scoreCardURL.AssertIsContained(PageUrlEnum.NewProviderScoreCard.GetStringValue() + prvSeq, "url should match.");
            
            CurrentPage = _newProviderScoreCard.CloseProviderScoreCardAndReturnToProviderSearch(PageTitleEnum.ProviderSearch);

        }

        private void Verify_Correct_Results_Displayed_For(string quickSearchOption,string first_name,string last_name,List<string> resultFromDb, ProviderSearchPage _providerSearch)
        {
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchOption);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider First Name", first_name);
            _providerSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Last Name", last_name);
            _providerSearch.ClickOnFindButton();
            _providerSearch.WaitForWorkingAjaxMessage();
            _providerSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search results displayed");
            _providerSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEquivalent(resultFromDb, "Verify Results");
            _providerSearch.GetSideBarPanelSearch.ClickOnClearLink();
        }
        #endregion

    }
}
