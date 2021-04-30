using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using static System.String;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    class LogicSearchClient 
    {
       
        #region Private

        //private LogicSearchPage _logicSearch;
        //private List<string> _assignedClientList;
        //private ClaimActionPage _claimAction;
        //private ClaimSearchPage _claimSearch;

        #endregion

        #region PROTECTED PROPERTIES

        protected  string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion


        #region DBinteraction methods

       /* private void RetrieveListFromDatabase()
        {

            _assignedClientList = _logicSearch.GetAssignedClientList(EnvironmentManager.ClientUserName);
            _assignedClientList.Insert(0, "All");

        }*/

        #endregion

        #region OVERRIDE 

        /*
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _logicSearch = QuickLaunch.NavigateToLogicSearch();
                RetrieveListFromDatabase();
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
            CurrentPage = _logicSearch;

        }
        
        protected override void ClassCleanUp()
        {
            _logicSearch.CloseDatabaseConnection();
            base.ClassCleanUp();
        }

        protected override void TestCleanUp()
        {

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _logicSearch = _logicSearch.Logout().LoginAsClientUser().NavigateToLogicSearch();
            }

            if (_logicSearch.GetPageHeader() != Extensions.GetStringValue(PageHeaderEnum.LogicSearch))
            {
                _logicSearch.ClickOnQuickLaunch().NavigateToLogicSearch();
            }
            _logicSearch.SideBarPanelSearch.OpenSidebarPanel();
            _logicSearch.SideBarPanelSearch.ClickOnClearLink();
            _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue());

        }*/

        #endregion

        #region TestSuites

        [Test] //TE-365,TE-509
        public void Verify_Search_Filters_For_All_Logics_Quick_Search_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> messageLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "Date_Validation_Messages");
                var paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claimSequence = paramLists["ClaimSequence"].Split(',').ToList();
                var expectedQuickSearchOptions = Enum.GetValues(typeof(LogicQuickSearchTypeEnum))
                    .Cast<LogicQuickSearchTypeEnum>().Select(x => x.GetStringValue()).ToList();
                var expectedProductTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "product_list_options").Values.ToList();
                var expectedAssignedToList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "client_list_options")
                    .Values.ToList();
                var expectedStatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Logic_status_options")
                    .Values.ToList();
                var _assignedClientList = _logicSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");


                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                        "Quick Search option defaults to All Open Logics");

                _logicSearch.SideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                    .ShouldCollectionBeEqual(expectedQuickSearchOptions, "Search Filters", true);

                StringFormatter.PrintMessageTitle("Verify Filter Options For All Logics");
                Verify_correct_search_filter_options_displayed_for("all_logics",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue(), _logicSearch, automatedBase);

                StringFormatter.PrintMessageTitle("Client Field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _assignedClientList, _logicSearch, true);

                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim Seq")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Create Date Range")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");

                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim Seq")
                    .ShouldBeFalse("Claim Sequence Should Be Enabled After Client Selected");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("Claim Sequence Should Be Enabled After Client Selected");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Create Date Range")
                    .ShouldBeFalse("Claim Sequence Should Be Enabled After Client Selected");

                StringFormatter.PrintMessageTitle("Create Date Range Validation");
                ValidateDatePickerField("Create Date Range", messageLists["Message1"], messageLists["Message2"],
                    messageLists["Message3"], _logicSearch);
                _logicSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle("Product field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Product")
                    .ShouldBeEqual("All", "Product should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList, _logicSearch, false);


                StringFormatter.PrintMessageTitle("Verify Assigned To Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To").ShouldBeEqual("Client",
                    "Is Default Value Set to Internal For Internal User ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Assigned To", expectedAssignedToList, _logicSearch, false);

                StringFormatter.PrintMessageTitle("Verify Status Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Status").ShouldBeEqual("All",
                    "Is Default Value Set to Open ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Status", expectedStatusList, _logicSearch, false);
                _logicSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Search cannot be completed without CLient .");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorkingAjaxMessage();
                _logicSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("User should not be able to search by Quick Search only");
                _logicSearch.GetPageErrorMessage().ShouldBeEqual(
                    "Search cannot be initiated without any criteria entered.",
                    "Verify the popup message when search by all logics only");
                _logicSearch.ClosePageError();
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("User should be able to search by All Logics with any other criteria selected.");
                _logicSearch.SideBarPanelSearch.ClickOnClearLink();



            }
        }

        [Test] //TE-365
        public void Verify_Search_Filters_For_All_Open_Logics_Quick_Search_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "Date_Validation_Messages");

                var expectedQuickSearchOptions = Enum.GetValues(typeof(LogicQuickSearchTypeEnum))
                    .Cast<LogicQuickSearchTypeEnum>().Select(x => x.GetStringValue()).ToList();

                var expectedClientList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "client_list_options")
                    .Values.ToList();
                var expectedProductTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "product_list_options").Values.ToList();
                var _assignedClientList = _logicSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");
                //Checking for default value in Quick Search Options
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                        "Quick Search option defaults to All Open Logics");

                //Checking Quick Search Contains Expected Options
                _logicSearch.SideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                    .ShouldCollectionBeEqual(expectedQuickSearchOptions, "Search Filters", true);

                StringFormatter.PrintMessageTitle("Verify Filter Options For All Open Logics");
                Verify_correct_search_filter_options_displayed_for("all_open_logics",
                    LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(), _logicSearch, automatedBase);

                StringFormatter.PrintMessageTitle("Client Field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _assignedClientList, _logicSearch);

                StringFormatter.PrintMessageTitle("Product field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Product")
                    .ShouldBeEqual("All", "Product should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList, _logicSearch, false);

                StringFormatter.PrintMessageTitle("Verify Assigned To Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To").ShouldBeEqual("Client",
                    "Is Default Value Set to Client For Client User ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Assigned To", expectedClientList, _logicSearch, false);

                StringFormatter.PrintMessageTitle("Create Date Range Validation");
                ValidateDatePickerField("Create Date Range", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"], _logicSearch);
            }
        }

        [Test] //TE-365
        public void Verify_Search_Filters_For_PCI_Open_Logics_Quick_Search_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var expectedClientList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "client_list_options")
                    .Values.ToList();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "Date_Validation_Messages");
                var expectedQuickSearchOptions = Enum.GetValues(typeof(LogicQuickSearchTypeEnum))
                    .Cast<LogicQuickSearchTypeEnum>().Select(x => x.GetStringValue()).ToList();
                var _assignedClientList = _logicSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                        "Quick Search option defaults to All Open Logics");
                _logicSearch.SideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                    .ShouldCollectionBeEqual(expectedQuickSearchOptions, "Search Filters", true);

                StringFormatter.PrintMessageTitle("Verify Filter Options For CV Open Logics");
                Verify_correct_search_filter_options_displayed_for("pci_open_logics",
                    LogicQuickSearchTypeEnum.PCIOpenLogics.GetStringValue(), _logicSearch, automatedBase);

                StringFormatter.PrintMessageTitle("Client Field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _assignedClientList, _logicSearch);

                StringFormatter.PrintMessageTitle("Verify Assigned To Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To").ShouldBeEqual("Client",
                    "Is Default Value Set to Client For Client User ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Assigned To", expectedClientList, _logicSearch, false);

                StringFormatter.PrintMessageTitle("Create Date Range Validation");
                ValidateDatePickerField("Create Date Range", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"], _logicSearch);

            }
        }

        [Test] //TE-365
        public void Verify_Search_Filters_For_FFP_Open_Logics_Quick_Search_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var expectedClientList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "client_list_options")
                    .Values.ToList();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "Date_Validation_Messages");
                var expectedQuickSearchOptions = Enum.GetValues(typeof(LogicQuickSearchTypeEnum))
                    .Cast<LogicQuickSearchTypeEnum>().Select(x => x.GetStringValue()).ToList();
                var _assignedClientList = _logicSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                        "Quick Search option defaults to All Open Logics");

                _logicSearch.SideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                    .ShouldCollectionBeEqual(expectedQuickSearchOptions, "Search Filters", true);

                StringFormatter.PrintMessageTitle("Verify Filter Options For FFP open Logics");
                Verify_correct_search_filter_options_displayed_for("ffp_open_logics",
                    LogicQuickSearchTypeEnum.FFPOpenLogics.GetStringValue(), _logicSearch, automatedBase);

                StringFormatter.PrintMessageTitle("Client Field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _assignedClientList, _logicSearch);

                StringFormatter.PrintMessageTitle("Verify Assigned To Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To").ShouldBeEqual("Client",
                    "Is Default Value Set to Client For Client User ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Assigned To", expectedClientList, _logicSearch, false);

                StringFormatter.PrintMessageTitle("Create Date Range Validation");
                ValidateDatePickerField("Create Date Range", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"], _logicSearch);

            }
        }

        [Test] //TE-365
        public void Verify_Search_Filters_For_DCI_Open_Logics_Quick_Search_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var expectedClientList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "client_list_options")
                    .Values.ToList();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "Date_Validation_Messages");
                var expectedQuickSearchOptions = Enum.GetValues(typeof(LogicQuickSearchTypeEnum))
                    .Cast<LogicQuickSearchTypeEnum>().Select(x => x.GetStringValue()).ToList();
                var _assignedClientList = _logicSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");
                //Checking for default value in Quick Search Options
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                        "Quick Search option defaults to All Open Logics");

                //Checking Quick Search Contains Expected Options
                _logicSearch.SideBarPanelSearch.GetAvailableDropDownList("Quick Search")
                    .ShouldCollectionBeEqual(expectedQuickSearchOptions, "Search Filters", true);

                StringFormatter.PrintMessageTitle("Verify Filter Options For DCA Open Logics");
                Verify_correct_search_filter_options_displayed_for("dci_open_logics",
                    LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue(), _logicSearch, automatedBase);

                StringFormatter.PrintMessageTitle("Client Field Validation");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client should default to All");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _assignedClientList, _logicSearch);

                StringFormatter.PrintMessageTitle("Verify Assigned To Field");

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To").ShouldBeEqual("Client",
                    "Is Default Value Set to Client For Client User ?");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Assigned To", expectedClientList, _logicSearch,false);

                StringFormatter.PrintMessageTitle("Create Date Range Validation");
                ValidateDatePickerField("Create Date Range", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"], _logicSearch);
            }
        }

        [Test] //TE-365,TE-509
        public void Verify_Clicking_Clear_Link_Will_Reset_All_Filters_Except_Quick_Search()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                StringFormatter.PrintMessageTitle("Check For All Logics Quick Search Option");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", paramLists["ClaimSequence"]);
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim No", paramLists["ClaimNo"]);
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Product", ProductEnum.CV.GetStringValue());
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Assigned To", "Client");
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Status", "Open");
                _logicSearch.SideBarPanelSearch.SetDateFieldFrom("Create Date Range",
                    DateTime.Now.ToString("MM/d/yyyy"));


                _logicSearch.SideBarPanelSearch.ClickOnClearLink();
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllLogics.GetStringValue(), "Quick Search");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Claim Seq")
                    .ShouldBeEqual("", "Claim Sequence");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim Seq")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Claim No").ShouldBeEqual("", "Claim No");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Product")
                    .ShouldBeEqual("All", "Product");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("Client", "Assigned To");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Status")
                    .ShouldBeEqual("All", "Status");
                _logicSearch.SideBarPanelSearch.GetDateFieldFrom("Create Date Range")
                    .ShouldBeEqual("", "Date From");
                _logicSearch.SideBarPanelSearch.GetDateFieldTo("Create Date Range")
                    .ShouldBeEqual("", "Date To");
                _logicSearch.SideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Create Date Range")
                    .ShouldBeTrue("Claim Sequence Should Be Disabled Before Client Selected");


                StringFormatter.PrintMessageTitle("Check For All Open Logics Quick Search Option");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());

                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                    ProductEnum.DCA.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", "Client");
                _logicSearch.SideBarPanelSearch.SetDateFieldFrom("Create Date Range",
                    DateTime.Now.ToString("MM/d/yyyy"));

                _logicSearch.SideBarPanelSearch.ClickOnClearLink();

                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(), "Quick Search");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "Client");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Product")
                    .ShouldBeEqual("All", "Product");
                _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("Client", "Assigned To");
                _logicSearch.SideBarPanelSearch.GetDateFieldFrom("Create Date Range")
                    .ShouldBeEqual("", "Date From");
                _logicSearch.SideBarPanelSearch.GetDateFieldTo("Create Date Range")
                    .ShouldBeEqual("", "Date To");


                StringFormatter.PrintMessageTitle("Check For CV Open Logics Quick Search Option");
                VerifyClearLinkForOtherQuickSearchOptions(LogicQuickSearchTypeEnum.PCIOpenLogics.GetStringValue(), _logicSearch);


                StringFormatter.PrintMessageTitle("Check For FFP Open Logics Quick Search Option");
                VerifyClearLinkForOtherQuickSearchOptions(LogicQuickSearchTypeEnum.FFPOpenLogics.GetStringValue(), _logicSearch);


                StringFormatter.PrintMessageTitle("Check For DCA Open Logics Quick Search Option");
                VerifyClearLinkForOtherQuickSearchOptions(LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue(), _logicSearch);
            }
        }

        [Test] //TE-364,TE-512
        public void Verify_Navigation_To_Claim_Action_From_logic_Search_page_For_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction;
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var claimseq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                        "claseq", "Value");
                _claimAction = _logicSearch.SearchByClaimSequenceToNavigateToNewClaimActionPage(claimseq);
                _claimAction.CloseAllExistingPopupIfExist();
                _claimAction.GetClaimSequence().ShouldBeEqual(claimseq);
                _logicSearch = _claimAction.ClickClaimSearchIconAndNavigateToLogicSearchPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue());
                _logicSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Default filter Results displayed?");
                _logicSearch.SideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test] //TE-364,TE-509,TE-571
        public void Verify_Correct_Primary_Data_Displayed_for_logic_Search_For_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramLists["claseq"];
                var expectedList = _logicSearch.GetValueForAllLogicsFromDatabase(paramLists["claseq"].Split('-')[0],
                    paramLists["claseq"].Split('-')[1]);
                StringFormatter.PrintMessage("Verify load more options");

                var rowCount = _logicSearch.GetGridViewSection.GetGridRowCount();
                if (_logicSearch.GetGridViewSection.IsLoadMorePresent())
                    _logicSearch.GetGridViewSection.ClickLoadMore();
                _logicSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(rowCount, "Row count increased?");

                StringFormatter.PrintMessage("verify search results");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq);
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim No", paramLists["claimno"]);
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                    ProductEnum.CV.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", "Cotiviti");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Status", "Open");
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range",
                    paramLists["createdate"].Split(',')[0], 1);
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range",
                    paramLists["createdate"].Split(',')[1], 2);
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorkingAjaxMessage();


                _logicSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "ClaimSequence value verified");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "Flag Value verified");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(5)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[2]).ToList(), "Assigned To value verified");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(6)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[3]).ToList(), "status value verified");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(7)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[6]).ToList(), " Created By Verified");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(8)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[4]).ToList(), " Modified date verified");

                StringFormatter.PrintMessage("Verify Label");
                _logicSearch.GetGridViewSection.GetLabelInGridByColRow(7)
                    .ShouldBeEqual("Created by:", "Label Should Match");
                _logicSearch.GetGridViewSection.GetLabelInGridByColRow(8)
                    .ShouldBeEqual("Last Response:", "Label Should Match");



                StringFormatter.PrintMessage("verify not matching records found message");
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", "1");
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorkingAjaxMessage();
                _logicSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("verify Message visible when results not available");
                _logicSearch.SideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Matching records found?");

            }
        }

        [Test] //TE-364,TE-509
        public void Verify_Correct_Secondary_Data_Displayed_for_logic_Search_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var expectedList = _logicSearch.getSecondaryDataFromDataBase(paramLists["claseq"].Split('-')[0],
                    paramLists["claseq"].Split('-')[1]);
                try
                {


                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", paramLists["claseq"]);
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessage();
                    _logicSearch.GetGridViewSection.ClickOnGridByRowCol(1, 4);
                    _logicSearch.GetLogicDetailHeader()
                        .ShouldBeEqual("Logic Details", "Correct Header displayed for secondary Details");



                    _logicSearch.GetSecondaryDetailsByLabel("Claim No")
                        .ShouldBeEqual(expectedList[0], "Claim No");
                    _logicSearch.GetSecondaryDetailsByLabel("Product")
                        .ShouldBeEqual(expectedList[1], "Product");
                    _logicSearch.GetSecondaryDetailsByLabel("Created")
                        .ShouldBeEqual(expectedList[2], "Created");
                    _logicSearch.GetSecondaryDetailsByLabel("Group")
                        .ShouldBeEqual(expectedList[3], "Group");
                }
                finally
                {
                    _logicSearch.WaitForStaticTime(2000);
                    _logicSearch.SideBarPanelSearch.OpenSidebarPanel();
                    _logicSearch.WaitForStaticTime(2000);
                }

            }
        }

        [Test] //TE-366
        public void Verify_Sorting_options_in_logic_search_For_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Sorting_options").Values.ToList();
                var createdateFrom = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "createdateFrom", "Value");
                var createdateTo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "createdateTo", "Value");
                try
                {

                    _logicSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "All open logic should get displayed");

                    StringFormatter.PrintMessage("Verify sort button present");
                    _logicSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                    _logicSearch.GetFilterOptionTooltip()
                        .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                    _logicSearch.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                    StringFormatter.PrintMessage("verify default sorting");
                    _logicSearch.IsListDateSortedInDescendingOrder(8)
                        .ShouldBeTrue("By Default result should be sorted by modified date");
                    StringFormatter.PrintMessage("Verify sort by client");
                    VerifySortingOptionsInLogicSearch(2, 1, filterOptions[0], _logicSearch);

                    VerifySortingOptionsInLogicSearch(4, 3, filterOptions[2], _logicSearch);
                    _logicSearch.clickOnClearSort();
                    _logicSearch.IsListDateSortedInDescendingOrder(8)
                        .ShouldBeTrue("By Default result should be sorted by modified date");


                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", createdateFrom, 1);
                    _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", createdateTo, 2);
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessage();
                    StringFormatter.PrintMessage("verify sorting of claim sequence");
                    _logicSearch.ClickOnFilterOptionListRow(2);
                    _logicSearch.GetlogicResultInAscendingOrder("C")
                        .ShouldCollectionBeEqual(_logicSearch.GetGridViewSection.GetGridListValueByCol(3),
                            "Ascending order maintained??");
                    _logicSearch.ClickOnFilterOptionListRow(2);
                    _logicSearch.GetlogicResultInDecscendingOrder("C")
                        .ShouldCollectionBeEqual(_logicSearch.GetGridViewSection.GetGridListValueByCol(3),
                            "claim seq sorted in Decscending order?");
                    VerifySortingOptionsInLogicSearch(5, 4, filterOptions[3], _logicSearch);
                    VerifySortingOptionsInLogicSearch(6, 5, filterOptions[4], _logicSearch);


                }
                finally
                {
                    _logicSearch.clickOnClearSort();

                }
            }
        }

        [Test] //TE-364
        public void Verify_Lock_Icon_Displayed_In_Logic_search_For_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {

                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "claseq", "Value");

                _logicSearch.DeleteClaimLock(claimSeq.Split('-')[0], claimSeq.Split('-')[1]);
                automatedBase.QuickLaunch = _logicSearch.Logout().LoginAsHciAdminUser5();

                _logicSearch = automatedBase.QuickLaunch.NavigateToLogicSearch();
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", "Client");
                var newClaimAction =
                    _logicSearch.ClickonclaimseqToNavigateClaimActionPage(claimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                automatedBase.CurrentPage = newClaimAction;
                StringFormatter.PrintMessage(
                    string.Format("Open claim action page for claim sequence: {0} with direct URL ",
                        claimSeq));
                var newClaimActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                newClaimAction =
                    _logicSearch.SwitchToOpenNewClaimActionByUrlForClientUserFromLogicSearchClient(newClaimActionUrl);
                newClaimAction.IsClaimLocked()
                    .ShouldBeTrue("Claim should be locked when it is in view mode by another user");


                _logicSearch = newClaimAction.NavigateToLogicSearch();
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.AllLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _logicSearch.SearchByClaimSequence(claimSeq);
                _logicSearch.IsLockIconPresentForClaimSequence(claimSeq).ShouldBeTrue("Lock icon present?");
                _logicSearch.GetGridViewSection.GetTitleOfListIconPresentInGridForClassName("lock")
                    .ShouldBeEqual(
                        "The claim is currently being locked by Test Automation5",
                        "Is Lock Message Equal?");




            }
        }

        [Test] //TE-364
        public void Verify_Search_Result_For_Different_Quick_Search_Option_For_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var openLogicList =
                    _logicSearch.GetDefaultDataForOpenLogicsFromDatabase("C");
                var OpenDciLogicList = _logicSearch.GetDCiOpenLogicSearchFromDatabase("C");
                var OpenPciLogicList = _logicSearch.GetPciOpenLogicSearchFromDatabase("C");
                var OpenFfpLogicList = _logicSearch.GetFfpOpenLogicSearchFromDatabase("C");
                string[] QuicksearchOptions =
                {
                    LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue(),
                    LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue(),
                    LogicQuickSearchTypeEnum.PCIOpenLogics.GetStringValue(),
                    LogicQuickSearchTypeEnum.FFPOpenLogics.GetStringValue()
                };
                List<string>[] ValuesFromDB = {openLogicList, OpenDciLogicList, OpenPciLogicList, OpenFfpLogicList};
                for (int j = 0; j < ValuesFromDB.Length; j++)
                {
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QuicksearchOptions[j]);
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", "Client");
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessage();
                    while (_logicSearch.GetGridViewSection.IsLoadMorePresent())
                        _logicSearch.GetGridViewSection.ClickLoadMore();
                    _logicSearch.IsListDateSortedInDescendingOrder(8)
                        .ShouldBeTrue("Results sorted in ascending order of the Modified date?");
                    _logicSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEqual(ValuesFromDB[j],
                        "verify values correct for" + QuicksearchOptions[j]);
                    _logicSearch.SideBarPanelSearch.ClickOnClearLink();
                }
            }
        }

        [Test] //CAR-1406 + CAR-1797
        public void Verify_New_Logic_Creation_on_Claim_Action_Flag_Line_For_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var message = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Message", "Value");
                var client = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Client", "Value");
                var createUser = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "CreateUser", "Value");
                _logicSearch.DeleteLogicNoteFromDatabase(claSeq);
                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verifying the tested claim sequence does not already have an open logic");
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Logics");
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", client);
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To",
                        CompanyEnum.companyName.GetStringValue());
                    _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq);
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessageForBothDisplayAndHide();
                    _logicSearch.SideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.",
                            "No matching records message should be shown for the search with no results.");

                    StringFormatter.PrintMessageTitle("Verifying creating a new logic");
                    _claimSearch = _logicSearch.NavigateToClaimSearch();

                    if (!_claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _claimSearch.ClickOnSidebarIcon();
                    }

                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    string flag = _claimAction.GetFirstFlag();
                    _claimAction.IsLogicPlusIconDisplayed(1).ShouldBeTrue("Create Logic Icon should be displayed.");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.IsLogicWindowDisplay().ShouldBeTrue("Create Logic Form Window should be displayed.");
                    _claimAction.IsLogicFormText("Create Logic").ShouldBeTrue("Create Logic form should be displayed");

                    StringFormatter.PrintMessage("Verifying the note section is a required field");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("A message should be given to create a Logic Request.");
                    _claimAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.");
                    _claimAction.ClosePageError();

                    StringFormatter.PrintMessage("Verifying the note text area allows only 500 characters");
                    _claimAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("TEST ", 101)));
                    _claimAction.GetLogicMessageTextarea().Length
                        .ShouldBeEqual(500,
                            "User should be able to write upto 500 characters in the message text area.");

                    StringFormatter.PrintMessage("Verifying clicking on Cancel button should clear the note text area");
                    _claimAction.GetSideWindow.Cancel();
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.GetLogicMessageTextarea()
                        .ShouldBeNullorEmpty("Clicking on Cancel button should clear the note text area.");
                    _claimAction.AddLogicMessageTextarea(message);
                    _claimAction.GetSideWindow.Save(waitForWorkingMessage: true);
                    if (_claimAction.IsPageErrorPopupModalPresent())
                    {
                        _claimAction.ClosePageError();
                        _claimAction.GetSideWindow.Cancel();
                        _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                        _claimAction.SearchByClaimSequence(claSeq);
                        if (!_claimAction.IsInternalLogicIconByRowPresent(1))
                        {
                            _claimAction.ClickOnAddLogicIconByRow(1);
                            _claimAction.AddLogicMessageTextarea(message);
                            _claimAction.GetSideWindow.Save(waitForWorkingMessage: true);
                        }

                    }

                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    _claimAction.SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnLogicIconWithLogicByRow(1);
                    var logicDate = Regex.Split(_claimAction.GetRecentRightLogicMessage(), "\r\n").ToList()[1];

                    _logicSearch = _claimAction.NavigateToLogicSearch();

                    if (!_logicSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _logicSearch.ClickOnSidebarIcon();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether the newly added logic can be searched from the Logic Search Page");
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Logics");
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client", client);
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To",
                        CompanyEnum.companyName.GetStringValue());
                    _logicSearch.SideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq);
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessageForBothDisplayAndHide();

                    var searchLogicData = new List<string>()
                    {
                        client, claSeq, flag, "Cotiviti", LogicStatusEnum.Open.GetStringValue(), "Created by:",
                        createUser, "Last Response:", logicDate
                    };

                    var logicSearchDetails = Regex.Split(_logicSearch.GetGridViewSection.GetGridAllRowData()[0], "\r\n")
                        .ToList();

                    for (int i = 0; i < logicSearchDetails.Count; i++)
                    {
                        if (i == logicSearchDetails.Count - 1)
                        {
                            var dateTimeWithoutSeconds = DateTime.ParseExact(logicSearchDetails[i],
                                "MM/dd/yyyy hh:mm:ss tt",
                                System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy hh:mm tt");

                            dateTimeWithoutSeconds.ShouldBeEqual(logicDate,
                                "Displayed 'Last Response' date in the Logic Search should be the date and time of last response in the logic conversation ");
                            continue;
                        }

                        logicSearchDetails[i].ShouldBeEqual(searchLogicData[i],
                            $"Is the Logic Search detail being shown correctly as per the newly created logic ?");
                    }
                }
                finally
                {
                    _logicSearch.SideBarPanelSearch.ClickOnClearLink();
                }
            }
        }

        [Test] //TE-873
        public void Validate_export_icon_and_exported_Excel_Value()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {

                LogicSearchPage _logicSearch;
                automatedBase.CurrentPage = _logicSearch = automatedBase.CurrentPage.NavigateToLogicSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Logic_export_headers").Values
                    .ToList();
                var parameterList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var sheetname = parameterList["sheetName"];
                var expectedDataList = _logicSearch.GetExcelDataListFromDB("C");
                var fileName = "";

                try
                {

                    _logicSearch.IsExportIconPresent().ShouldBeTrue("Export Icon Present?");
                    _logicSearch.IsExportIconEnabled().ShouldBeTrue("Is Export Icon enabled?");

                    _logicSearch.ClickOnExportIcon();
                    _logicSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Confirmation Model Displayed after clicking on export?");
                    _logicSearch.GetPageErrorMessage()
                        .ShouldBeEqual("Logic Search results will be exported to Excel. Do you wish to continue?");

                    StringFormatter.PrintMessage("verify on clicking cancel in confirmation model , nothing happens");
                    _logicSearch.ClickOkCancelOnConfirmationModal(false);
                    _logicSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Is Confirmation model displayed after clicking cancel?");

                    StringFormatter.PrintMessage("verify export of claim search");
                    _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                    _logicSearch.WaitForWorkingAjaxMessage();
                    _logicSearch.ClickOnExportIcon();
                    _logicSearch.ClickOkCancelOnConfirmationModal(true);
                    _logicSearch.WaitForStaticTime(3000);
                    

                    fileName = _logicSearch.GoToDownloadPageAndGetFileName();


                    ExcelReader.ReadExcelSheetValue(fileName, sheetname, 3, 3, out List<string> headerList,
                        out List<List<string>> excelExportList, out string clientname, true);

                    StringFormatter.PrintMessage("verify client name and header values");
                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                    clientname.Split('-')[0].ShouldBeEqual(parameterList["Header"], "Header name correct?");
                    StringFormatter.PrintMessage("verify values correct?");


                    for (int i = 0; i < expectedDataList.Count - 1; i++)
                    {
                        excelExportList[i][1].ShouldBeEqual(expectedDataList[i][0],
                            "Correct Claim Sequence/pre auth sequence values should be exported");
                        excelExportList[i][2].ShouldBeEqual(expectedDataList[i][1],
                            "Correct Flag values should be exported");
                        excelExportList[i][3].ShouldBeEqual(expectedDataList[i][2],
                            "Correct Product values should be exported");
                        excelExportList[i][4].ShouldBeEqual(expectedDataList[i][3],
                            "Correct Assigned to values should be exported");
                        excelExportList[i][5].ShouldBeEqual(expectedDataList[i][4],
                            "Correct status values should be exported");
                        excelExportList[i][6].ShouldBeEqual(expectedDataList[i][5],
                            "Correct last response values should be exported");
                    }


                    _logicSearch.GetLogicExportAuditListFromDB(automatedBase.EnvironmentManager.Username).ShouldContain(
                        "/api/clients/SMTST/logicSearchResults/DownloadXLS/", "Claim search download audit present?");
                }
                finally
                {
                    _logicSearch.CloseAnyPopupIfExist();
                    if (!IsNullOrEmpty(fileName))
                        ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);

                }
            }
        }

        #endregion

        #region Private Methods

        private void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue, LogicSearchPage _logicSearch, AutomatedBaseClientParallel automatedBase)
        {
            _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
            _logicSearch.SideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                    true);
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, LogicSearchPage _logicSearch,bool order = true)
        {
            var actualDropDownList = _logicSearch.SideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
        }

        private void ValidateDatePickerField(string label, string message1, string message2, string message3,LogicSearchPage _logicSearch)
        {
            _logicSearch.SideBarPanelSearch.GetDateFieldPlaceholder(label, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (from) default value");
            _logicSearch.SideBarPanelSearch.GetDateFieldPlaceholder(label, 2).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (to) default value");
            _logicSearch.SideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _logicSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date From is empty");
            _logicSearch.SideBarPanelSearch.SetDateField(label,
                DateTime.Now.AddDays(1).ToString("MM/d/yyyy"),1);
            _logicSearch.SideBarPanelSearch.SetDateField(label, "", 2);
            _logicSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date To is empty");
            _logicSearch.SideBarPanelSearch.SetInputFieldByLabel(label,
                DateTime.Now.AddDays(2).ToString("MM/d/yyyy"), sendTabKey: true); //check numeric value can be typed
            _logicSearch.SideBarPanelSearch.GetDateFieldFrom(label).ShouldBeEqual(
                DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"), label + " Checks numeric value is accepted");
            _logicSearch.SideBarPanelSearch.SetDateField(label, DateTime.Now.ToString("MM/d/yyyy"),1);
            _logicSearch.SideBarPanelSearch.GetDateFieldTo(label).ShouldBeEqual(
                _logicSearch.SideBarPanelSearch.GetDateFieldFrom(label),
                label + " From value populated in To field as well.");
            _logicSearch.SideBarPanelSearch.SetDateField(label,
                DateTime.Now.Subtract(new TimeSpan(24, 0, 0)).ToString("MM/d/yyyy"),2);
            _logicSearch.GetPageErrorMessage().ShouldBeEqual("Please enter a valid date range.");
            _logicSearch.ClosePageError();
            _logicSearch.SideBarPanelSearch.ClickOnHeader();
            _logicSearch.SideBarPanelSearch.SetDateField(label,
                DateTime.Now.AddMonths(3).AddDays(3).ToString("MM/d/yyyy"),2);
            _logicSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message1,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", label));
            _logicSearch.SideBarPanelSearch.ClickOnFindButton();
            _logicSearch.GetPageErrorMessage()
                .ShouldBeEqual(message2, "Verification of popup message for invalid date");
            _logicSearch.ClosePageError();
            _logicSearch.SideBarPanelSearch.ClickOnHeader();
        }

        private void VerifyClearLinkForOtherQuickSearchOptions(string quickSearchOption, LogicSearchPage _logicSearch)
        {
            _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                quickSearchOption);
            _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                ClientEnum.SMTST.ToString());
            _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", "Cotiviti");
            _logicSearch.SideBarPanelSearch.SetDateFieldFrom("Create Date Range",
                DateTime.Now.ToString("MM/d/yyyy"));
            _logicSearch.SideBarPanelSearch.ClickOnClearLink();

            _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search")


                .ShouldBeEqual(quickSearchOption, "Quick Search");
            _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Client")
                .ShouldBeEqual("All", "Client");
            _logicSearch.SideBarPanelSearch.GetInputValueByLabel("Assigned To")
                .ShouldBeEqual("Client", "Assigned To");
            _logicSearch.SideBarPanelSearch.GetDateFieldFrom("Create Date Range")
                .ShouldBeEqual("", "Date From");
            _logicSearch.SideBarPanelSearch.GetDateFieldTo("Create Date Range")
                .ShouldBeEqual("", "Date To");
        }

        private void VerifySortingOptionsInLogicSearch(int col, int option, string sortoption, LogicSearchPage _logicSearch)
        {
            StringFormatter.PrintMessageTitle("Validation of Sorting by "+ sortoption);
            _logicSearch.ClickOnFilterOptionListRow(option);
            _logicSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                .ShouldBeTrue("Search result must be sorted by" + sortoption + " in Ascending");
            _logicSearch.ClickOnFilterOptionListRow(option);
            _logicSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                .ShouldBeTrue("Search result must be sorted by" + sortoption + " in Descending");

        }

        #endregion


    }
}
