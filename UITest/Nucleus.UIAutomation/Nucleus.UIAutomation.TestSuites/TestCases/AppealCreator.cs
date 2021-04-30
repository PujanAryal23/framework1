using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;
using System.IO;
using System.Text;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealCreator
    {
        //#region PRIVATE FIELDS

        //private ClientSearchPage _clientSearch;
        //private AppealCreatorPage _appealCreator;
        //private AppealSummaryPage _appealSummary;
        //private NewPopupCodePage _newPopupCode;
        //private ClaimSearchPage _claimSearch;
        //private ClaimActionPage _claimAction;
        //private AppealProcessingHistoryPage _appealProcessingHistory;
        //private AppealNotePage _appealNotePage;
        //private AppealManagerPage _appealManager;
        //private AppealSearchPage _appealSearch;
        //private AppealActionPage _appealAction;
        //private FileUploadPage FileUploadPage;
        //private List<string> _activeProductListForClient;
        //private List<string> _allProductListFromDB;
        //private const string col1 = "Select Appeal Lines";
        //private const string col2 = "Selected Lines";
        //#endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
        //        FileUploadPage = _appealCreator.GetFileUploadPage;
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


        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        //if (automatedBase.CurrentPage.IsQuickLaunchIconPresent())
        //        //    _appealCreator.GoToQuickLaunch().Logout();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }

        //}

        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    automatedBase.CurrentPage = _appealCreator;
        //}

        //protected override void TestCleanUp()
        //{
        //    base.TestCleanUp();
        //    if (_appealCreator.IsOkButtonPresent())
        //        _appealCreator.ClickOnOkButtonOnConfirmationModal();
        //    if (_appealCreator.IsPageErrorPopupModalPresent())
        //        _appealCreator.ClosePageError();

        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _appealCreator = _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealCreator();

        //    }

        //    if (_appealCreator.GetPageHeader() != PageHeaderEnum.ClaimSearch.GetStringValue())
        //        _appealCreator.NavigateToAppealCreator();

        //    if (!_appealCreator.CurrentPageUrl.Contains("appealClaims"))
        //        _appealCreator.NavigateToAppealCreator();


        //}
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

        //#region DBinteraction methods
        //private void RetrieveListFromDatabase()
        //{
        //    _activeProductListForClient = (List<string>)_appealCreator.GetCommonSql.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
        //    _allProductListFromDB = _appealCreator.GetCommonSql.GetAllProductList();
        //}


        //#endregion

        #region TEST SUITES

        [Test] //CAR-3120(CAR-3064)
        public void Verify_creation_of_MRR_Appeal_type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                AppealCategoryManagerPage _appealCategoryManager;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claSeq = testData["ClaimSequences"].Split(',').ToList();
                var categoryId = testData["CategoryId"];
                var proc = testData["Proc"];
                var analyst = testData["Analyst"].Split(',').ToList();
                var appealType = "Medical Record Review";
                var userId = testData["UserId"];
                Random rnd = new Random();
                Dictionary<string, string> products = new Dictionary<string, string>
                {
                    ["Coding Validation"] = "F",
                    ["FacilityClaim Insight"] = "U",
                    ["FraudFinder Pro"] = "R"
                };
                var product = products.ElementAt(rnd.Next(products.Count));
                
                string[] newAppealData =
                {
                    categoryId, proc, proc, "", product.Key, "", "", analyst[0],
                    "For MRR appeal creation", ""
                };
                try
                {
                    StringFormatter.PrintMessage("Setting 'enable_medical_record_reviews' to T");
                    if (automatedBase.CurrentPage.GetCommonSql.GetSpecificClientDataFromDb("enable_medical_record_reviews",
                        ClientEnum.SMTST.ToString()) == "F")
                    {
                        automatedBase.CurrentPage.GetCommonSql.UpdateSpecificClientDataInDB("enable_medical_record_reviews='T'", ClientEnum.SMTST.ToString());
                        automatedBase.CurrentPage.RefreshPage(false);
                    }

                    StringFormatter.PrintMessage("Creating Appeal Category");
                    _appealCategoryManager = automatedBase.CurrentPage.NavigateToAppealCategoryManager();
                    _appealCategoryManager.CreateAppealCategory(newAppealData, categoryOrder: false);

                    StringFormatter.PrintMessageTitle($"Navigating to Claim Action page via {claSeq}");
                    _claimSearch = _appealCategoryManager.NavigateToClaimSearch();

                    StringFormatter.PrintMessage("Deleting Analyst's PTO from Db");
                    _claimSearch.GetCommonSql.DeletePTOByUserIdFromDb(userId);

                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0], true);
                    
                    StringFormatter.PrintMessage($"Navigating to Appeal Creator page via {claSeq} claim sequence");
                    _appealCreator = _claimAction.ClickOnCreateAppealIcon();
                    var expectedDueDate = _appealCreator.CalculateAndGetAppealDueDateFromDatabase("0");

                    StringFormatter.PrintMessage(
                            "Verifying if MRR appeal type is present when enable medical record reviews is T");
                    _appealCreator.GetCommonSql.GetSpecificClientDataFromDb("enable_medical_record_reviews",
                            ClientEnum.SMTST.ToString()).ShouldBeEqual("T", "Enable Medical Record Review should be T");
                    _appealCreator.IsMedicalRecordReviewButtonPresent().ShouldBeTrue(
                            "For Enable Medical Record Review = T, Medical record review button should be present");
                    _appealCreator.GetMedicalRecordReviewToolTip().ShouldBeEqual("Medical Record Review",
                            "Tooltip should match for MRR appeal type");
                    _appealCreator.ClickMedicalRecordReviewButton();
                    _appealCreator.GetCreateAppealColumnHeaderText().ShouldBeEqual("Create Medical Record Review",
                            "Form header of Medical Record Review Appeal type should match");
                    _appealCreator.IsUrgentCheckboxDisabled()
                            .ShouldBeTrue("Urgent checkbox should be disabled when appeal type 'M' is selected ");

                    StringFormatter.PrintMessage("Verifying creation of MRR appeal");
                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppeal(product.Key, "DocID", "", "M");

                    StringFormatter.PrintMessage("Verification after creating MRR appeal");
                    _claimAction.IsAddAppealIconDisabled().ShouldBeTrue(
                            "Add appeal icon should be disabled after Appeal has been created");
                    VerifyAppealDataAppealAfterSave(analyst, categoryId, expectedDueDate, _claimAction,
                            automatedBase,
                            product.Key, appealType, product.Value, true);
                        
                    StringFormatter.PrintMessage("Verifying if MRR appeal type is present when enable medical record reviews is F");
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("enable_medical_record_reviews='F'",
                            ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage.NavigateToAppealCreator().SearchByClaimSequence(claSeq[1]);
                    _appealCreator.RefreshPage(false);
                    _appealCreator.GetCommonSql.GetSpecificClientDataFromDb("enable_medical_record_reviews",
                            ClientEnum.SMTST.ToString()).ShouldBeEqual("F", "Enable Medical Record Review should be F");
                    _appealCreator.IsMedicalRecordReviewButtonPresent()
                            .ShouldBeFalse("Is Medical Record Review Button present?");
                }
                finally
                {
                    StringFormatter.PrintMessage("Clearing up the appeal created");
                    DeletePreviousAppeals(claSeq, automatedBase);
                    _appealCategoryManager = automatedBase.CurrentPage.NavigateToAppealCategoryManager();

                    StringFormatter.PrintMessage("Deleting the Appeal Category created");
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    if (_appealCategoryManager.GetSideBarPanelSearch.GetAvailableDropDownList("Category ID").Contains(categoryId))
                    {
                        _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID", categoryId);
                        _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _appealCategoryManager.WaitForWorking();
                        _appealCategoryManager.GetGridViewSection.ClickEditDeleteIconByValueAndIcon(categoryId, "Delete Record");
                        _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);
                    }

                    StringFormatter.PrintMessage("Enabling Medical Record Type for SMTST");
                    _appealCategoryManager.GetCommonSql.UpdateSpecificClientDataInDB("enable_medical_record_reviews='T'", ClientEnum.SMTST.ToString());
                }
            }
        }

        [Test, Category("AppealCreator1"), Category("OnDemand")] //CAR-2694(CAR-2489) + CAR-2898(CAR-2850)
        [NonParallelizable]

        public void Verify_create_a_COB_product_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage  _clientSearch = null;
                AppealCreatorPage _appealCreator;
                AppealManagerPage _appealManager;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequence", "Value")
                    .Split(',').ToList();
                var AppealDueDateCalculationType = new List<string> {"Business", "Calendar"};
                Random rnd = new Random();
                var type = AppealDueDateCalculationType[rnd.Next(AppealDueDateCalculationType.Count)];
                var expectedDueDate = new List<string>();
                try
                {
                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.SelectCalculationType(type);
                    _clientSearch.GetSideWindow.Save();
                    StringFormatter.PrintMessage($"updated Appeal calculation type to {type}");
                    var offsetValue = _clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.COB.ToString());
                    automatedBase.CurrentPage.NavigateToAppealCreator();
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq[0]);
                    if (_appealCreator.IsClaimLocked() &&
                        _appealCreator.GetPageHeader() == PageHeaderEnum.ClaimSearch.GetStringValue())
                    {
                        DeletePreviousAppeals(claimSeq, automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq[0]);
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        StringFormatter.PrintMessageTitle(
                            "Verification of Product drop down list when claim line is selected:if top flag is COB of a line then COB should selected by default when claim line selected");
                        _appealCreator.ClickOnClaimLine(i == 0 ? 3 : 1);
                        _appealCreator.GetSideWindow.GetInputValueByLabel("Product").ShouldBeEqual(
                            ProductEnum.COB.GetStringValue(),
                            $"Is {ProductEnum.COB.GetStringValue()} select by default?");
                        if (i == 0)
                            _appealCreator.SelectRecordReviewRecordType();
                        else
                            _appealCreator.SelectAppealRecordType();
                        if (i == 1)
                            _appealCreator.SetAppealPriorityUrgent();
                        _appealCreator.SetDocumentId("1234");
                        _appealCreator.GetSideWindow.Save(waitForWorkingMessage: true);
                        expectedDueDate.Add(
                            _appealCreator.CalculateAndGetAppealDueDateFromDatabase(offsetValue[i], type));
                        if (i < 2)
                        {
                            _appealCreator.GetSideBarPanelSearch.OpenSidebarPanel();
                            _appealCreator.SearchByClaimSequence(claimSeq[i + 1]);
                        }
                    }

                    _appealManager = _appealCreator.NavigateToAppealManager();
                    _appealManager.SelectAllAppeals();
                    _appealManager.SelectSMTST();

                    for (var i = 0; i < 3; i++)
                    {
                        _appealManager.GetSideBarPanelSearch.OpenSidebarPanel();
                        _appealManager.SetClaimSequence(claimSeq[i]);
                        _appealManager.ClickOnFindButton();
                        _appealManager.WaitForWorkingAjaxMessage();


                        _appealManager.GetGridViewSection.GetValueInGridByColRow(7)
                            .ShouldBeNullorEmpty("Is Category Id Null?");
                        _appealManager.GetGridViewSection.GetValueInGridByColRow(8)
                            .ShouldBeNullorEmpty("Is Primary Reviewer Null?");
                        _appealManager.GetGridViewSection.GetValueInGridByColRow(9)
                            .ShouldBeNullorEmpty("Is DenyCode Null?");
                        _appealManager.GetGridViewSection.GetValueInGridByColRow(10)
                            .ShouldBeNullorEmpty("Is PayCode Null?");
                        _appealManager.GetGridViewSection.GetValueInGridByColRow(13)
                            .ShouldBeEqual(ProductEnum.COB.GetStringDisplayValue(),
                                $"Is {ProductEnum.COB.GetStringValue()}  Equal?");
                        _appealManager.GetGridViewSection.ClickOnGridRowByRow();
                        _appealManager.WaitForWorking();
                        _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                            .ShouldBeNullorEmpty("Is Assigned To Null?");
                        var appealType = i == 0 ? "Record Review" : i == 1 ? "Urgent" : "Appeal";

                        _appealManager.GetGridViewSection.GetValueInGridByColRow(3)
                            .ShouldBeEqual(expectedDueDate[i], $"Is Due Date Equal for {appealType}");

                    }



                    _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    _appealSearch.SelectSMTST();
                    _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq[2]);
                    _appealAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue(),
                        "COB Appeal can be searchable from Appeal Search for Appeal Action Page");
                    _appealAction.GetPrimaryReviewer().ShouldBeNullorEmpty("Is Primary Reviewer Null?");
                    _appealAction.GetAssignedTo().ShouldBeNullorEmpty("Is Assigned To Null?");
                    _appealAction.GetAppealCategory().ShouldBeNullorEmpty("Is Category Id Null?");
                    _appealAction.GetDueDate().ShouldBeEqual(expectedDueDate[2], "Is Due Date Equals?");


                    StringFormatter.PrintMessageTitle(
                        "Verification of COB Appeal can be viewable and other field should be null");
                    _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage().Logout()
                        .LoginAsClientUser()
                        .NavigateToAppealSearch();

                    _appealSearch.SelectAllAppeals();
                    var _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq[2]);
                    _appealSummary.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue(),
                        "COB Appeal can be searchable from Appeal Search for Appeal Summary Page");

                    _appealSummary.GetAppealDetails(1, 3).ShouldBeEqual(expectedDueDate[2], "Is Due Date Equals?");
                    _appealSummary.GetAppealDetails(2, 4)
                        .ShouldBeEqual(ProductEnum.COB.GetStringValue(), "Is Product Equals?");

                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    expectedDueDate.Clear();
                    automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();

                }
                finally
                {
                    automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.SelectCalculationType("Business");
                    _clientSearch.GetSideWindow.Save();
                    StringFormatter.PrintMessage("updated Appeal calculation type to Business");
                }
            }
        }

        [Test,Category("OnDemand")] //CAR-2998(CAR-2925)
        [NonParallelizable]
        public void Verify_Appeal_Due_Date_Calculation_Settings_For_Calendar_Holiday_Options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch = null;
                AppealCreatorPage _appealCreator;
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequence", "Value")
                    .Split(',').ToList();
                var excludeHolidayOptions = new List<string> {"Cotiviti", "Client", "Both"};
                Random rnd = new Random();
                var option = excludeHolidayOptions[rnd.Next(excludeHolidayOptions.Count)];
                var expectedDueDate = new List<string>();
                try
                {
                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch
                        .SelectCalculationType("Business"); // To Reset Exclude Holidays After Each Iteration
                    _clientSearch.SelectCalculationType("Calendar");
                    if (option == "Both")
                    {
                        _clientSearch.SelectHolidayOption(excludeHolidayOptions[0]);
                        _clientSearch.SelectHolidayOption(excludeHolidayOptions[1]);
                    }
                    else
                    {
                        _clientSearch.SelectHolidayOption(option);
                    }

                    _clientSearch.GetSideWindow.Save();
                    StringFormatter.PrintMessage($"Updated Holiday option type to {option}");
                    _clientSearch.IsHolidayOptionSetToTrueInDatabase(option)
                        .ShouldBeTrue($"Holiday Option Should Be Set To True In Database for {option}");
                    var offsetValue = _clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.COB.ToString());
                    automatedBase.CurrentPage.NavigateToAppealCreator();
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq[0]);
                    if (_appealCreator.IsClaimLocked() &&
                        _appealCreator.GetPageHeader() == PageHeaderEnum.ClaimSearch.GetStringValue())
                    {
                        DeletePreviousAppeals(claimSeq, automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq[0]);
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        StringFormatter.PrintMessageTitle(
                            "Verification of Product drop down list when claim line is selected:if top flag is COB of a line then COB should selected by default when claim line selected");
                        _appealCreator.ClickOnClaimLine(i == 0 ? 3 : 1);
                        _appealCreator.GetSideWindow.GetInputValueByLabel("Product").ShouldBeEqual(
                            ProductEnum.COB.GetStringValue(),
                            $"Is {ProductEnum.COB.GetStringValue()} select by default?");
                        if (i == 0)
                            _appealCreator.SelectRecordReviewRecordType();
                        else
                            _appealCreator.SelectAppealRecordType();
                        if (i == 1)
                            _appealCreator.SetAppealPriorityUrgent();
                        _appealCreator.SetDocumentId("1234");
                        _appealCreator.GetSideWindow.Save(waitForWorkingMessage: true);
                        expectedDueDate.Add(
                            _appealCreator.CalculateAndGetAppealDueDateFromDatabase(offsetValue[i], "Calendar"));
                        if (i < 2)
                        {
                            _appealCreator.GetSideBarPanelSearch.OpenSidebarPanel();
                            _appealCreator.SearchByClaimSequence(claimSeq[i + 1]);
                        }
                    }

                    _appealManager = _appealCreator.NavigateToAppealManager();
                    _appealManager.SelectAllAppeals();
                    _appealManager.SelectSMTST();
                    for (var i = 0; i < 3; i++)
                    {
                        if (!_appealManager.IsSideBarPanelOpen())
                            _appealManager.GetSideBarPanelSearch.OpenSidebarPanel();
                        _appealManager.SetClaimSequence(claimSeq[i]);
                        _appealManager.ClickOnFindButton();
                        _appealManager.WaitForWorkingAjaxMessage();
                        _appealManager.GetGridViewSection.ClickOnGridRowByRow();
                        _appealManager.WaitForWorking();
                        _appealManager.GetGridViewSection.GetValueInGridByColRow(3)
                            .ShouldBeEqual(expectedDueDate[i], $"Is Due Date Correct ?");
                    }

                    expectedDueDate.Clear();
                }
                finally
                {
                    automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.SelectCalculationType("Business");
                    _clientSearch.GetSideWindow.Save();
                    StringFormatter.PrintMessage("updated Appeal calculation type to Business");
                }
            }
        }

        [Test,Category("AppealCreator1")]//US50646 bug story
        [Order(2)]
        public void Verify_presence_of_sub_status_of_pended_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                automatedBase.CurrentPage = _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq);
                _appealCreator.GetSearchlistComponentItemValue(1, 7)
                    .ShouldBeEqual("Pending Agreement", "Sub Status Should present for pended claims");
            }
        }

        [Test, Category("AppealDependent"), Category("AppealCreator1")]//US41373 +CAR-2967
        [Order(3)]
        public void Verify_upload_document_and_view_appeal_documents()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                AppealProcessingHistoryPage _appealProcessingHistory = null;
                AppealNotePage _appealNotePage = null;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction = null;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                var modifiedBy =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ModifiedBy",
                        "Value");
                var action =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Action", "Value");
                var status =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Status", "Value");
                var line = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Line",
                    "Value");
                var priority =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Priority", "Value");
                var product =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Product", "Value");
                var note = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Note",
                    "Value");
                try
                {
                    _appealCreator.NavigateToAppealCreatorUsingDifferentUser(claimSeq);

                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq);
                    _appealCreator.IsClaimLocked().ShouldBeTrue("Claim Should be Locked");
                    _appealCreator.IsCreateAppealDisabled().ShouldBeTrue("Create Appeal Icon Should be disabled");

                    _appealCreator.CloseWindowThatLocksClaim();


                    _appealProcessingHistory = _appealCreator.OpenAppealProcessingHistoryPage(claimSeq);
                    var row = _appealProcessingHistory.GetResultGridRowCount();
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 2)
                        .ShouldBeEqual(modifiedBy, "Modified By:"); //modifiedBy
                    var actionValue = _appealProcessingHistory.GetAppealAuditGridTableDataValue(row - 1, 3);
                    if (actionValue != "UploadDocs")
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(row - 1, 3)
                            .ShouldBeEqual(action, "Action :"); //Action
                    else
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(row - 2, 3)
                            .ShouldBeEqual(action, "Action :"); //Action
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 6)
                        .ShouldBeEqual(status, "Status of Appeal"); //Status
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(row, 7)
                        .ShouldBeEqual(claimSeq, "Appeal created to claim Seq:"); //Claim Seq
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(row, 8)
                        .ShouldBeEqual(line, "Line:"); //Line #
                    var assignedTo = _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 9);
                    Regex.IsMatch(assignedTo, @"^(\S+ )+\S+ +\(+\S+\)+$")
                        .ShouldBeTrue("Assigned To '" + assignedTo + "' is in format XXX XXX (XXX)");
                    var primaryReviewer = _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 10);
                    Regex.IsMatch(primaryReviewer, @"^(\S+ )+\S+ +\(+\S+\)+$")
                        .ShouldBeTrue("Primary Reviewer '" + primaryReviewer + "' is in format XXX XXX (XXX)");
                    string date = _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 12);
                    Regex.IsMatch(date, @"^([1-9]|1[0-2])\/([1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$")
                        .ShouldBeTrue("The Due  Date'" + date + "' is in format MM/DD/YYYY");
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 13)
                        .ShouldBeEqual(priority, "Priority:"); //Priority
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 14)
                        .ShouldBeEqual(product, "Product :"); //Product
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 15)
                        .ShouldBeEqual(note, "Note :"); //Comments
                    _appealAction = _appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();
                    _appealAction.GetClientNotes().ShouldBeEqual(note, "Note Value");
                    //_appealNotePage = _appealAction.ClickOnViewNoteIcon();
                    //_appealNotePage.GetnoteTypeValue().ShouldBeEqual("Appeal", "Note Type Value");
                    //_appealNotePage.GetNoteValue().ShouldBeEqual(note, "Note Value");
                    //_appealAction = _appealNotePage.CloseNotePopUpPageSwitchToAppealAction();

                    automatedBase.CurrentPage =
                        _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                    _claimSearch.RefreshPage();
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);

                    _claimAction.ClickOnViewAppealIcon();
                    _claimAction.GetAppealSequenceCount()
                        .ShouldBeGreater(0,
                            "Appeal Should Present after appeal created in claim sequence<" + claimSeq + ">");

                    automatedBase.CurrentPage = _appealCreator = _claimAction.NavigateToAppealCreator();
                }
                finally
                {
                    if (_appealProcessingHistory != null && _appealAction.IsAppealProcessingHistory())
                    {
                        _appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();
                        automatedBase.CurrentPage =
                            _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    if (_appealNotePage != null && _appealAction.IsAppealNotePagePresent())
                    {
                        _appealNotePage.CloseNotePopUpPageSwitchToAppealAction();
                        automatedBase.CurrentPage =
                            _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }

        }



        [Test, Category("AppealDependent"), Category("AppealCreator1")]//US41371
        [Order(4)]
        public void Verify_search_by_claim_sequence_without_sub_and_search_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var lockClaimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "LockClaimSequence", "Value");
                try
                {

                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);
                    _appealCreator.ClickOnSearchIcon();
                    _appealCreator.SearchByClaimSequence(claimSeq.Split('-')[0]);
                    _appealCreator.ClickCloseIconOnFindClaimSection();
                    _appealCreator.GetClaimRowSectionListInAddClaimSection()
                        .ShouldBeGreater(1, "Claim Row should be Greater than 1 when search by claim seq without sub");
                    _appealCreator.GetClaimSequenceListInAddClaimSection()
                        .IsInDescendingOrder()
                        .ShouldBeTrue("Claim Sequence should be in descending order");
                    _appealCreator.GetClaimSequenceHavingLockInAddClaimSection()
                        .ShouldBeEqual(lockClaimSeq, "Locked Claim Sequence match?");
                    _appealCreator.GetClaimSequenceWithSubList(col1)
                        .Contains(lockClaimSeq)
                        .ShouldBeFalse(string.Format("Claim Sequence <{0}> should not be in Select Claim Lines",
                            lockClaimSeq));

                    _appealCreator.ClickOnUnlockClaimSequenceInAddClaimSection()
                        .ShouldBeEqual("Claim Action",
                            "Claim Action Popup Page Header Equal when click on unlocked claim sequence");
                    _appealCreator.ClickOnLockClaimSequenceInAddClaimSection()
                        .ShouldBeEqual("Claim Action",
                            "Claim Action Popup Page Header Equal when click on locked claim sequence");
                    _appealCreator.ClickOnClaimLineHavingLockInAddClaimSection();
                    _appealCreator.IsFlagIconPresentInAddSection().ShouldBeTrue("Flag Icon should Present");
                    _appealCreator.ClickCloseIconOnFindClaimSection();

                    _appealCreator.GetClaimNoInAddClaimSection()
                        .ShouldBeEqual(_appealCreator.GetClaimNoValueSelectedClaimSection(),
                            "Claim No is not hyperlink and should be specific");
                    _appealCreator.IsClearOnAddClaimSectionPresent().ShouldBeTrue("Clear Link is present?");

                    _appealCreator.ClickOnClearOnAddClaimSection();
                }
                finally
                {

                    if (_appealCreator.IsCreateAppealHeaderPresent())
                        _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();


                }
            }

        }

        //US41371
        [Test, Category("AppealCreator1")]
        [Order(6)]
        public void Verify_proper_validation_on_search_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                var claimSeqNotMatch = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSeqNotMatch", "Value");
                var claimNoNotMatch = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimNoNotMatch", "Value");

                try
                {
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);

                    _appealCreator.IsSearchIconPresentInAppealCreatorPage()
                        .ShouldBeTrue("Search Icon Displayed in Appeal Cretor Page");
                    _appealCreator.ClickOnSearchIcon();
                    _appealCreator.IsFindClaimSectionPresent().ShouldBeTrue("Find Claim Section Displayed");
                    _appealCreator.GetClaimSequenceInFindClaimSection().ShouldBeEqual(claimSeq,
                        "Claim Sequence Text Box should be previous searched claim sequence");
                    string.IsNullOrEmpty(_appealCreator.GetClaimNoInFindClaimSection())
                        .ShouldBeTrue("Claim No Text Box should be blank by default");

                    _appealCreator.SetClaimSequenceInFindClaimSection("asdf");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Only numbers allowed.");
                    _appealCreator.ClosePageError();
                    _appealCreator.SetClaimNoInFindClaimSection("a1");
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("No error pop up open when claim no is alphanumeric. Is error present?");
                    _appealCreator.SearchByClaimSequence(claimSeqNotMatch);
                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Present");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Claims added to an appeal must be for the same patient and provider. Please enter a different claim.");
                    _appealCreator.ClosePageError();

                    _appealCreator.ClickOnClearLinkOnFindSection();

                    _appealCreator.SearchByClaimNo(claimNoNotMatch);

                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Present");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Claims added to an appeal must be for the same patient and provider. Please enter a different claim.",
                            "Popup Message when claim is trying to add having different patient and provider by Claim Number");
                    _appealCreator.ClosePageError();

                    _appealCreator.ClickOnClearLinkOnFindSection();

                    _appealCreator.ClickOnFindButton();

                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Present");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Search cannot be initiated without any criteria entered.",
                            "Poup Message with no values in Claim Seq and Claim No");
                    _appealCreator.ClosePageError();

                    _appealCreator.SearchByClaimSequence("1");
                    _appealCreator.GetEmptyMessage()
                        .ShouldBeEqual("No matching records were found.", "No Match Record Message");
                    _appealCreator.ClickOnClearLinkOnFindSection();
                    string.IsNullOrEmpty(_appealCreator.GetClaimSequenceInFindClaimSection())
                        .ShouldBeTrue("Claim Sequence Text Box should be blank when click on clear button");
                    string.IsNullOrEmpty(_appealCreator.GetClaimNoInFindClaimSection())
                        .ShouldBeTrue("Claim Sequence Text Box should be blank when click on clear button");
                }
                finally
                {
                    if (_appealCreator.IsCreateAppealHeaderPresent())
                        _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();

                }
            }

        }

        [Test, Category("AppealCreator1")]//US41371
        [Order(7)]
        public void Verify_ability_to_search_and_add_new_claim_to_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                var claimSeq1 =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence1",
                        "Value");
                try
                {
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);
                    if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        DeletePreviousAppeals(claimSeq,automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    _appealCreator.ClickOnSearchIcon();
                    _appealCreator.SearchByClaimSequence(claimSeq1);
                    _appealCreator.ClickOnAddClaimRowSection();
                    _appealCreator.ClickCloseIconOnFindClaimSection();
                    _appealCreator.ClickOnClearOnAddClaimSection();
                    var claimSeqList = _appealCreator.GetClaimSequenceWithSubList(col1);
                    claimSeqList[0].ShouldBeEqual(claimSeq,
                        "Original Claim Should Be on Top after new claim added");
                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "All Selected Claim Line Present in Selected Lines and Highlighted when checked on Select All CheckBox?");
                    _appealCreator.GetClaimLineListHavingZeroBillSection(col1)
                        .ShouldCollectionBeEmpty(
                            "Claim Lines Having Zero bill should not Highlighted in Select Appeal Lines");
                    _appealCreator.GetClaimLineListHavingZeroBillSection(col2)
                        .ShouldCollectionBeEmpty(
                            "Claim Lines Having Zero bill should not Highlighted in Selected Lines");
                    foreach (var claimSequence in claimSeqList)
                    {
                        _appealCreator.GetLineNoList(col1, claimSequence).IsInAscendingOrder().ShouldBeTrue(
                            "Is  Line No  of Claim line in ascending order By Claim Seq" + claimSequence + "in " +
                            col1);
                        _appealCreator.GetLineNoList(col2, claimSequence).IsInAscendingOrder().ShouldBeTrue(
                            "Is  Line No  of Claim line in ascending order By Claim Seq" + claimSequence + "in " +
                            col2);
                    }

                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1).ShouldCollectionBeEmpty(
                        "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line when unchecked on Select All CheckBox");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2).ShouldCollectionBeEmpty(
                        "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line when unchecked on Select All CheckBox");
                    _appealCreator.ClickOnClaimLevelSection(col1);
                    _appealCreator.ClickOnClaimLevelSection(col1, 2);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "All Selected Claim Line Present in Selected Lines and Highlighted when click on claim level?");
                    _appealCreator.ClickOnClaimLevelSection(col2, 2);
                    _appealCreator.ClickOnClaimLevelSection(col2);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1).ShouldCollectionBeEmpty(
                        "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2).ShouldCollectionBeEmpty(
                        "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col1);
                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col1, 2);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1).ShouldCollectionBeEqual(
                        _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                        "All Selected Claim Line Present in Selected Lines and Highlighted when clicked on every claim lines?");
                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col2, 2, true);
                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col2, 1, true);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1).ShouldCollectionBeEmpty(
                        "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2).ShouldCollectionBeEmpty(
                        "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.ClickOnCliamLineByClaimSeqRow(col1);
                    _appealCreator.ClickOnCliamLineByClaimSeqRow(col1, 2);
                    _appealCreator.SelectProduct("Coding Validation");
                    _appealCreator.SelectAppealRecordType();
                    _appealCreator.ClickOnSaveBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                    {
                        _appealCreator.ClosePageError();
                        _appealCreator.SelectProduct(ProductEnum.CV.GetStringValue());
                        _appealCreator.ClickOnSaveBtn();
                    }

                    _appealCreator.WaitForWorking();
                    _claimSearch = _appealCreator.NavigateToClaimSearch();
                    _claimSearch.RefreshPage();
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage("1458012", true);
                    foreach (var claimSequence in claimSeqList)
                    {
                        _claimAction.ClickWorkListIcon();
                        _claimAction.ClickSearchIcon();
                        _claimAction.SearchByClaimSequence(claimSequence);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickOnViewAppealIcon();
                        _claimAction.GetAppealSequenceCount()
                            .ShouldBeGreater(0,
                                "Appeal Should Present after appeal created in claim sequence<" + claimSequence + ">");
                    }

                    automatedBase.CurrentPage = _appealCreator = _claimAction.NavigateToAppealCreator();
                }
                finally
                {
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }
        }


        [Test, Category("AppealCreator1"),Category("OnDemand")]//US41367
        [NonParallelizable]
        public void Verify_Document_ID_display_or_not_when_setting_is_altered_also_validate_Document_Type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                List<string> expectedFileTypeList = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "File_Type_List").Values.ToList();
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                List<string> expectedSelectedFileTypeList = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "SelectedFileList", "Value").Split(',')
                    .ToList();
                var documentId = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "DocumentId",
                    "Value");
                var description = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "Description", "Value");
                var displayExtDocIdFlag = false;
                try
                {
                    for (var i = 0; i < 2; i++)
                    {
                        automatedBase.CurrentPage =
                            _clientSearch =
                                automatedBase.CurrentPage.NavigateToClientSearch();

                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                            ClientSettingsTabEnum.Product.GetStringValue());
                        _clientSearch.ClickOnRadioButtonByLabel(
                            ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(), i != 0);

                        if (i == 0)
                            _clientSearch
                                .IsDivByLabelPresent(ProductAppealsEnum.CotivitiUploadsAppealDocuments.GetStringValue())
                                .ShouldBeFalse($"Is Cotiviti Uploads Appeal Documents Appeal Settings Display?");

                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        displayExtDocIdFlag = i != 0;
                        automatedBase.CurrentPage = _clientSearch.NavigateToAppealCreator();

                        automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);
                        if (i != 0)
                            _appealCreator.IsDocumentIDPresent()
                                .ShouldBeTrue(
                                    $"{ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()} should present");
                        else
                            _appealCreator.IsDocumentIDPresent().ShouldBeFalse(
                                $"{ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()} should not present");

                    }

                    _appealCreator.SetDocumentId(documentId);
                    _appealCreator.GetDocumentId().Length.ShouldBeEqual(50,
                        "Character length should not exceed more than 50 in Document ID");
                    _appealCreator.SetDescription(description);
                    _appealCreator.GetDescription().Length.ShouldBeEqual(100,
                        "Character length should not exceed more than 100 in Description");
                    _appealCreator.GetAvailableFileTypeList()
                        .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                    _appealCreator.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                    _appealCreator.GetPlaceHolderValue().ShouldBeEqual("Provider Letter", "File Type Text");
                    _appealCreator.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                    _appealCreator.GetPlaceHolderValue().ShouldBeEqual("Multiple values selected",
                        "File Type Text when multiple value selected");
                    _appealCreator.GetSelectedFileTypeList()
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Selected File List Equal");
                    _appealCreator.ClickOnCancelBtn();

                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }
                finally
                {
                    if (!displayExtDocIdFlag)
                    {
                        automatedBase.CurrentPage =
                            _clientSearch =
                                automatedBase.CurrentPage.NavigateToClientSearch();

                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                            ClientSettingsTabEnum.Product.GetStringValue());
                        _clientSearch.ClickOnRadioButtonByLabel(
                            ProductAppealsEnum.DisplayExtDocIDField.GetStringValue());
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }
                }
            }
        }


        [Test, Category("AppealCreator1"),Category("OnDemand")]//US41366
        [NonParallelizable]
        public void Verify_select_lines_for_appeal_and_proper_validation_on_selecting_claim_lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";
                var subClaimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "SubClaimSequence", "Value");
                var procCodeHavingDeletedFlag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName, "ProcCodeHavingDeletedFlag", "Value");
                var procCodeHavingNeverFlag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName, "ProcCodeHavingNeverFlag", "Value");
                var subClaimSeqList = subClaimSeq.Split(',').ToList();
                var allowBillableFlag = false;
                try
                {
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='N'", ClientEnum.SMTST.ToString());
                    _appealCreator.RefreshPage();
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);
                    _appealCreator.ClickOnClaimLinesNotHavingFlag(col1);
                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Popup Appeared");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "This client does not allow billable appeals. Lines that do not have Cotiviti flags cannot be selected for an appeal.",
                            "Validate Popup Message when click on claim line not  having flag when allow billable setting set to 'NO'");
                    _appealCreator.ClosePageError();
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='Y'", ClientEnum.SMTST.ToString());
                    _appealCreator.RefreshPage();
                    allowBillableFlag = true;
                    _appealCreator.ClickOnSearchIcon();
                    automatedBase.CurrentPage = _appealCreator.SearchByClaimSequence(claimSeq);
                    _appealCreator.ClickOnCliamLinesSectionByProCode(col1, procCodeHavingDeletedFlag);

                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "Claim lines having deleted flag with Proc Code<" + procCodeHavingDeletedFlag +
                            ">     Present in Selected Lines and Highlighted after allow billable setting set to 'YES' ?");
                    _appealCreator.ClickOnCliamLinesSectionByProCode(col2, procCodeHavingDeletedFlag);

                    _appealCreator.ClickOnCliamLinesSectionByProCode(col1, procCodeHavingNeverFlag);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "Claim lines having never flagged with  Proc Code<" + procCodeHavingNeverFlag +
                            ">   Present in Selected Lines and Highlighted after allow billable setting set to 'YES' ?");
                    _appealCreator.ClickOnCliamLinesSectionByProCode(col2, procCodeHavingNeverFlag);

                    _appealCreator.ClickOnClaimLineHavingZeroBillSection(col1);
                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Popup Appeared");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual("Lines with billed amount of 0 cannot be selected for an appeal.",
                            "Validate Popup Message when click on claim line having bill amount zero");
                    _appealCreator.ClosePageError();

                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "All Selected Claim Line Present in Selected Lines and Highlighted when checked on Select All CheckBox?");
                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line when unchecked on Select All CheckBox");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line when unchecked on Select All CheckBox");

                    _appealCreator.ClickOnClaimLevelSection(col1);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "All Selected Claim Line Present in Selected Lines and Highlighted when click on claim level?");
                    _appealCreator.ClickOnClaimLevelSection(col2);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line");

                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col1);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEqual(_appealCreator.GetClaimLineListNotHavingZeroBillSection(col2),
                            "All Selected Claim Line Present in Selected Lines and Highlighted when clicked on every claim lines?");
                    _appealCreator.ClickOnClaimLineNotHavingZeroBillSection(col2, 1, true);
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col1)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Select Appeal Lines Column Should Be un-highlighted after deselect claim line");
                    _appealCreator.GetClaimLineListNotHavingZeroBillSection(col2)
                        .ShouldCollectionBeEmpty(
                            "All Selected Claim in Selected Lines Column Should Be un-highlighted after deselect claim line");

                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetLineNoList(col2, claimSeq).IsInAscendingOrder()
                        .ShouldBeTrue("Is  Line No  of Claim line in ascending order");
                    _appealCreator.ClickOnSelectAllCheckBox();

                    foreach (var cliamSequence in subClaimSeqList)
                    {
                        _appealCreator.SearchByClaimSequence(cliamSequence);
                        _appealCreator.ClickOnAddClaimRowSection();
                    }

                    _appealCreator.ClickCloseIconOnFindClaimSection();


                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.GetClaimSequenceList(col2).IsInAscendingOrder()
                        .ShouldBeTrue("Is Claim Seq in Selected Lines Column in ascending order");
                    _appealCreator.ClickOnSelectAllCheckBox();

                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }
                finally
                {
                    if (!allowBillableFlag)
                    {
                        _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='Y'", ClientEnum.SMTST.ToString());
                    }


                }
            }


        }

        [Test, Category("AppealCreator1")]//us41365
        [Order(9)]
        public void Verify_presence_of_column_container_in_appeal_creator()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                _appealCreator.SearchByClaimSequence(claimSeq);

                _appealCreator.IsAppealCreatorColumnContainerHeaderPresent(1).ShouldBeTrue("Is Column one present?");
                _appealCreator.GetColumnHeaderText(1).AssertIsContained("Select Appeal Lines",
                    "Checking if first column header contains select appeal lines");
                _appealCreator.IsAppealCreatorColumnContainerHeaderPresent(2).ShouldBeTrue("Is Column two present?");
                _appealCreator.GetColumnHeaderText(2).ShouldBeEqual("Selected Lines", "Checking second column header");
                _appealCreator.IsCreateAppealHeaderPresent().ShouldBeTrue("Is third Column present?");
                var appealcolumnCreateFormHeader = new List<string>
                {
                    "Create Appeal", "Create Record Review", "Create Dental Review"
                };
                _appealCreator.GetCreateAppealColumnHeaderText().AssertIsContainedInList(appealcolumnCreateFormHeader,
                    "Release date and user should be updated with the current logged in User and current system date and time");
                //_appealCreator.GetCreateAppealColumnHeaderText().ShouldBeEqual("Create Appeal", "Checking third column header");
                _appealCreator.ClickOnCancelBtn();
            }
        }

        [Test, Category("AppealCreator1")] //us41365
        public void Verify_presence_of_provider_name_patience_sequence()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                _appealCreator.SearchByClaimSequence(claimSeq);

                _appealCreator.GetProviderLabelValue().ShouldBeEqual("Provider:", "Checking label value for provider");
                _appealCreator.GetPatientSequenceLabelValue()
                    .ShouldBeEqual("Patient:", "Checking label value for patient");
                _appealCreator.IsProviderNamePresent().ShouldBeTrue("Provider Name is present in header");
                _appealCreator.IsPatientSequenceValuePresent().ShouldBeTrue("Patient Sequence is present in header");
                _appealCreator.ClickOnCancelBtn();
            }
        }

        [Test, Category("AppealDependent"), Category("AppealCreator1")] //US41365
        public void Verify_select_appeal_lines_column_population_for_each_row()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                NewPopupCodePage _newPopupCode;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "ClaimSequence",
                        "Value");
                var claimSequenceForRevCode = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName,
                    "ClaimSequenceForRevCode", "Value");
                var claimSequenceForFlags = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName,
                    "ClaimSequenceForFlags", "Value");
                var appealLevel = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AppealLevel", "Value");
                var unreviewedFlag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "UnreviewedFlag", "Value");
                var reviewedFlag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ReviewedFlag", "Value");
                var autoApprovedFlag = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AutoApprovedFlag", "Value");

                _appealCreator.SearchByClaimSequence(claimSeq);
                try
                {
                    Console.WriteLine("Checking grouping by Claim Sequence");
                    _appealCreator.GetClaimSequenceLabel()
                        .ShouldBeEqual("Claim Sequence:", "Checking label value for Claim Seq");
                    _appealCreator.IsClaimSequenceValuePresent().ShouldBeTrue("Claim Sequence is present ");
                    _appealCreator.GetClaimNoLabel().ShouldBeEqual("Claim No:", "Checking label value for Claim No");
                    _appealCreator.IsClaimNoValueElementPresent().ShouldBeTrue("Claim No is present");
                    var claimLinesCount = _appealCreator.GetClaimLinesCount();
                    var originalHandle = string.Empty;
                    _appealCreator.GetLineNoList("Select Appeal Lines", claimSeq).IsInAscendingOrder()
                        .ShouldBeTrue("Is Claim Line No in Select Appeal Lines Column in ascending order");

                    Console.WriteLine("Checking if claim lines are sorted");
                    for (var row = 1; row <= claimLinesCount; row++)
                    {
                        Console.WriteLine("Checking columns for row no " + row);
                        _appealCreator.IsLineNoForClaimLinesPresent(row).ShouldBeTrue("Line no is present");
                        _appealCreator.IsProcCodePresent(row).ShouldBeTrue("ProcCode is present");
                        _appealCreator.IsProcDescriptionToolTipPresent(row)
                            .ShouldBeTrue("Proc Description is displayed for row<" + row + ">");
                        (!String.IsNullOrEmpty(_appealCreator.GeProcDescriptionToolTip(row))).ShouldBeTrue(
                            "Proc description is present and tool tip displayed on hover");
                        _appealCreator.IsAppealLevelPresent(row).ShouldBeTrue("Appeal Level is present");
                        if (row > 1)
                            _appealCreator.IsAppealLevelIconPresent(row)
                                .ShouldBeFalse("Appeal Level Should not displayed");
                        else
                            _appealCreator.GetAppealLevelIconValue(1)
                                .ShouldBeEqual(appealLevel,
                                    "Appeal Level value for first row of " + claimSeq + " should equal to " +
                                    appealLevel);

                        _appealCreator.IsBilledAmountPresent(row).ShouldBeTrue("Bill Amount Present?");
                        _appealCreator.IsFlagDivPresent(row).ShouldBeTrue("Flag div is present");

                    }

                    _appealCreator.GetFlagValue(2).Trim().ShouldBeEqual(unreviewedFlag,
                        "Unreviewd flag value of row no  " + 2 + "  of " + claimSeq + " should equal to " +
                        unreviewedFlag);
                    _appealCreator.AreUnreviewedFlagsPresent().ShouldBeTrue("Flags unreviewed are colored bold red");


                    var procCode = _appealCreator.GetProcCodeValue(1);
                    _newPopupCode = _appealCreator.ClickOnProcedureCodeandSwitch("CPT Code - " + procCode, 1);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Header Text");
                    _appealCreator.CloseAnyPopupIfExist();




                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();

                    Console.WriteLine("Claim sequence with revenue code value being used for test ");
                    _appealCreator.SearchByClaimSequence(claimSequenceForRevCode);

                    var revenueCode = _appealCreator.GetRevCodeValue(1);

                    _newPopupCode = _appealCreator.ClickOnRevenueCodeandSwitch("Revenue Code - " + revenueCode, 1);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("Revenue Code", "Header Text");
                    _appealCreator.CloseAnyPopupIfExist();

                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();

                    Console.WriteLine("Claim sequence with reviewd and auto approved flags being used for test ");
                    _appealCreator.SearchByClaimSequence(claimSequenceForFlags);

                    _appealCreator.AreReviewedFlagsPresent()
                        .ShouldBeTrue("Flags reviewed are colored red in appeal line for row=1 ");
                    _appealCreator.GetFlagValue(1).Trim().ShouldBeEqual(reviewedFlag,
                        "Reviewed flag value of 1st row of " + claimSeq + " should equal to " + reviewedFlag);
                    var flagPresentList = reviewedFlag.Split(',').ToList();
                    _appealCreator.AreAutoApprovedFlagsPresent()
                        .ShouldBeTrue("Flags Auto Approved are colored gray in appeal line for row=2 ");

                    _appealCreator.GetFlagValue(2).Trim().ShouldBeEqual(autoApprovedFlag,
                        "Auto Approved flag value of 2nd row of " + claimSeq + " should equal to " + autoApprovedFlag);
                    Console.WriteLine("Flags Auto Approved are colored gray");
                    try
                    {

                        ClaimActionPage newClaimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                        var uniqueFlagList = newClaimAction.GetFlagListForClaimLine(1).Distinct().ToList();
                        ; //get only unique flag list for first claim line of claimAction
                        flagPresentList.ShouldCollectionBeEqual(uniqueFlagList,
                            "Unique flags are present per line and flag order matches that of Claim Action");

                    }
                    finally
                    {
                        _appealCreator.CloseAnyPopupIfExist();
                    }

                    _appealCreator.ClickOnCancelBtn();
                }
                finally
                {
                    if (_appealCreator.IsCreateAppealHeaderPresent())
                        _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();

                }
            }

        }

        [Test, Category("AppealCreator1")] //US41370 part 1 (broken down to US43143) working successfully  part
        public void Verify_presenece_of_required_columns_in_create_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeqWithAllFlags = paramLists["ClaimSequenceWithAllFlags"];
                var claimSeqWithNoFlags = paramLists["ClaimSequenceWithNoFlags"];
                var claimSeqWithSomeFlags = paramLists["ClaimSequenceWithSomeFlags"];
                //var activeProductsList = paramLists["ActiveProductsList"];
                var flaggedLineDefaultProduct = paramLists["FlaggedLineDefaultProduct"];
                var name = paramLists["Name"];
                var emailaddress = paramLists["EmailAddress"];
                var phoneno = paramLists["PhoneNo"];
                var appealNo = paramLists["AppealNo"];
                var popUpMessage = paramLists["PopUpMessage"];
                var expectedProductTypeList = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "Product_Type_List").Values.ToList();
                _appealCreator.SearchByClaimSequence(claimSeqWithAllFlags);
                try
                {
                    _appealCreator.IsAppealPriorityStatusSetToUrgent()
                        .ShouldBeFalse("Appeal Priority Status as Urgent should be false by default");
                    _appealCreator.SelectRecordReviewRecordType().GetCreateAppealColumnHeaderText()
                        .ShouldBeEqual("Create Record Review", "Create appeal form header title check");
                    _appealCreator.IsUrgentCheckboxDisabled().ShouldBeTrue("Urgent should be disabled");
                    _appealCreator.SelectAppealRecordType().GetCreateAppealColumnHeaderText()
                        .ShouldBeEqual("Create Appeal", "Create appeal form header title check");
                    _appealCreator.IsUrgentCheckboxDisabled()
                        .ShouldBeFalse(
                            "Urgent chekcbox field should be enabled if user selectes 'Appeal' radio button");

                    _appealCreator.GetListOfActiveProducts()
                        .ShouldCollectionBeEqual(expectedProductTypeList,
                            "List of active Products are displayed and present");

                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.GetSelectedProductValue()
                        .ShouldBeEqual(flaggedLineDefaultProduct, "Product value is based on top flag line");
                    const string appealNoVal = "1120";
                    _appealCreator.SetAppealCreatorFieldValue("Appeal #", appealNoVal);
                    _appealCreator.ClickOnCancelBtn();
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue(
                            "Pop up error should display when user clicks cancel after selecting a claim line");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(popUpMessage,
                            "Confirmation message should be as expected when  cancel button is clicked after selection of claim line");
                    _appealCreator.ClickOnCancelOnConfirmationModal();

                    _appealCreator.GetCreateAppealColumnHeaderText().ShouldBeEqual("Create Appeal",
                        "Create appeal form header title check. Appeal selected by default");
                    _appealCreator.GetCreateAppealFormInputValueGeneric("Appeal #")
                        .ShouldBeEqual(appealNoVal,
                            "On clicking cancel, no changes have been made, and value of appeal # should be equal to previously set value");
                    _appealCreator.ClickOnCancelBtn();
                    _appealCreator.ClickOnOkButtonOnConfirmationModal();

                    _appealCreator.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                        "If a user clicks on cancel in the 3 column Appeal Creator page, the page should redirected back to the search result");

                    Console.WriteLine("Claim with no flag lines being selected ");
                    _appealCreator.SearchByClaimSequence(claimSeqWithNoFlags);
                    VerifyValidationMessageForNoLineSelected("Validation message displayed when no line is selected ",_appealCreator);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnCancelOnConfirmationModal(); //cancel returns to same page with no changes
                    _appealCreator.GetSelectedProductValue().ShouldBeNullorEmpty("Product value needs to be selected.");
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                    Console.WriteLine("Claim with some flag lines being selected ");
                    _appealCreator.SearchByClaimSequence(claimSeqWithSomeFlags);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.SelectAppealRecordType();
                    _appealCreator.GetSelectedProductValue()
                        .ShouldBeEqual(flaggedLineDefaultProduct, "Product value needs to be selected.");
                    _appealCreator.SelectProduct(ProductEnum.DCA.GetStringValue());
                    _appealCreator.GetSelectedProductValue()
                        .ShouldBeEqual("Dental Claim Accuracy", "Product value can be assigned manually.");
                    var n = _appealCreator.GetCreateAppealFormLabelValue(4) == "External Document ID" ? 6 : 5;
                    _appealCreator.GetCreateAppealFormLabelValue(n)
                        .ShouldBeEqual("Name *",
                            "Checking label value for name"); //this step is being performed so as to confirm column 5 corresponds to name and so on for the rest
                    _appealCreator.GetCreateAppealFormInputValue(n)
                        .ShouldBeEqual(name, "Checking field is populated by names of user");
                    _appealCreator.GetCreateAppealFormLabelValue(n + 1)
                        .ShouldBeEqual("Email Address *", "Checking label value for email");
                    _appealCreator.GetCreateAppealFormInputValue(n + 1).ShouldBeEqual(emailaddress,
                        "Checking field is populated by email of user");
                    _appealCreator.GetCreateAppealFormPhoneExtLabelValue(n + 2, 1)
                        .ShouldBeEqual("Phone Number *", "Checking label value for phone");
                    _appealCreator.GetCreateAppealFormPhoneExtInputValue(n + 2, 1)
                        .ShouldBeEqual(phoneno, "Checking field is populated by phone no of user ");
                    _appealCreator.SetCreateAppealFormInputValue("", n); //name
                    VerifyValidationMessage("Validation message displayed when name is missing ",_appealCreator);
                    _appealCreator.SetCreateAppealFormInputValue(name, n); //set name to default
                    _appealCreator.SetCreateAppealFormInputValue("", n + 1); //email
                    VerifyValidationMessage("Validation message displayed when email is missing ",_appealCreator);
                    _appealCreator.SetCreateAppealFormInputValue(emailaddress, n + 1); //set email to default
                    _appealCreator.SetCreateAppealFormPhoneExtInputValue("", n + 2, 1); //phone no
                    VerifyValidationMessage("Validation message displayed when phone no is missing ",_appealCreator);
                    _appealCreator.SetCreateAppealFormPhoneExtInputValue(phoneno, n + 2, 1); //phone no
                    _appealCreator.GetCreateAppealFormLabelValue(n + 3)
                        .ShouldBeEqual("Appeal #", "Checking label value for Appeal Number");
                    _appealCreator.SetCreateAppealFormInputValue(appealNo,
                        n + 3); //26 characters,field is user editable
                    _appealCreator.GetCreateAppealFormInputValue(n + 3).Length.ShouldBeEqual(25,
                        "Character length should not exceed more than 25 in Appeal no");
                    _appealCreator.SetCreateAppealFormInputValue("as!", n + 3); //alpha numeric
                    VerifyValidationMessageForAppealNoAlphanumeric(
                        "Validation message displayed when appeal no is not alphanumeric. ",_appealCreator);

                    _appealCreator.ClearProduct();
                    VerifyValidationMessage("Validation message displayed when product is missing ",_appealCreator);
                    _appealCreator.SelectProduct(flaggedLineDefaultProduct); //set product to default
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }
                finally
                {
                    if (_appealCreator.IsCreateAppealHeaderPresent())
                        _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }
            }
        }
         
        [Test]//us41370 part 2 +CAR-2967
        public void Verify_appeal_create_behaviour_after_appeal_creation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                AppealSummaryPage _appealSummary = null;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                AppealProcessingHistoryPage _appealProcessingHistory;
                AppealSearchPage _appealSearch;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var modifiedBy = paramLists["Name"];
                var appealNo = paramLists["AppealNo"];
                var maxnote = paramLists["MaxNote"];
                try
                {
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    if ((_appealCreator.IsClaimLocked() || _appealCreator.IsAddAppealInClaimListDisabled(1)) &&
                        _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        DeletePreviousAppeals(claimSeq,automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    var n = _appealCreator.GetCreateAppealFormLabelValue(4) == "External Document ID" ? 6 : 5;
                    _appealCreator.SetNote(maxnote);
                    _appealCreator.SetCreateAppealFormInputValue(appealNo,
                        n + 3); //26 characters,field is user editable
                    _appealCreator.ClickOnClaimLine(1);
                    if (_appealCreator.IsExternalDocIdDisplayed())
                        _appealCreator.SetAppealCreatorFieldValue("External Document ID", "");
                    var createdDate = DateTime.Now.Date.ToString("M/d/yyyy");
                    _appealCreator.SelectAppealRecordType();
                    _appealCreator.ClickOnSaveBtn();
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("No page error displayed when external document id is empty.");
                    _appealCreator.WaitForWorking();
                    if (_appealCreator.GetPageHeader() == "Claim Search")
                    {
                        _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                        _claimSearch.RefreshPage();
                        _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                        _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.IsClaimLocked().ShouldBeTrue("Claim is Locked!");
                        _claimAction.GetLockIConTooltip().ShouldBeEqual(
                            "Claim is locked. You cannot modify claims linked to an appeal. Adjustments must be made using Appeal Action.");
                        _appealProcessingHistory = _appealCreator.OpenAppealProcessingHistoryPage(claimSeq);
                        var rows = _appealProcessingHistory.GetResultGridRowCount();
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 2, 1).Split(' ')[0]
                            .ShouldBeEqual(createdDate, "Modified Date :"); //modifiedDate
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 2, 2)
                            .ShouldBeEqual(modifiedBy, "Modified By:"); //modifiedBy
                        if (_appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 1, 3) == "AddLine")
                            _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 3)
                                .ShouldBeEqual("Create", "Action :"); //Action
                        else
                            _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 1, 3)
                                .ShouldBeEqual("Create", "Action :"); //Action
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 2, 6)
                            .ShouldBeEqual("Open", "Status :"); //Status
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 7)
                            .ShouldBeEqual(claimSeq, "Claim Sequence"); //Claim Seq
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 8)
                            .ShouldBeEqual("1", "Line No :"); //Line #
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 9)
                            .ShouldBeNullorEmpty("Assigned To Should be Empty"); //Assigned to
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 10)
                            .ShouldBeNullorEmpty("Primary Reviewer should be Empty"); //PR 
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows, 12)
                            .ShouldBeNullorEmpty("DueDate Should be Empty"); //Duedate # 
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 1, 13)
                            .ShouldBeEqual("Normal", "Priority :"); //Priority
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 1, 14)
                            .ShouldBeEqual("F", "Product :"); //Product
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(rows - 1, 15)
                            .ShouldBeEqual(maxnote, "Comments :"); //Comments
                        automatedBase.CurrentPage = _appealSummary =
                            _appealProcessingHistory.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                        _appealSummary.GetAppealDetailFieldPresentByLabel("Client Notes")
                            .ShouldBeEqual("UITEST", "Note Type Value");
                        //var appealNotePage = _appealAction.ClickOnViewNoteIcon();
                        //appealNotePage.GetnoteTypeValue().ShouldBeEqual("Appeal", "Note Type Value");
                        //_appealAction = appealNotePage.CloseNotePopUpPageSwitchToAppealAction();
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                }
                finally //to delete appeal created
                {
                    _appealCreator.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.GetPageHeader() == "Appeal Action")
                        automatedBase.CurrentPage =
                            _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                }
            }
        }


     

        [Test] //US41372
        public void Verify_appeal_creator_claim_search_check_for_menu_option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                _appealCreator.Logout().LoginAsUserHavingNoManageAppealAuthority();
                _appealCreator.IsAppealMenuPresent()
                    .ShouldBeFalse("Appeal Creator not visible to user with no authority of manage appeals");
                _appealCreator.IsAppealCreatorQuicklaunchTilePresent()
                    .ShouldBeFalse("Appeal Creator Tile is absent  to user with no authority of manage appeals");
                _appealCreator.Logout().LoginAsUserHavingManageAppealsReadOnlyAuthority();
                _appealCreator.IsAppealMenuPresent()
                    .ShouldBeTrue("Appeal Creator should visible to user with read only authority of manage appeals");
            }

        }




        [Test, Category("AppealDependent")] //US41372 //passing so far
        public void Verify_appeal_creator_claim_search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var noDataMessage = paramLists["NoDataMessage"];
                var claimNo = paramLists["ClaimNo"];
                var appealLevelCs = paramLists["AppealLevelCS"];
                var multiAppealCs = paramLists["MultipleAppealCS"];
                var claseq = paramLists["NonLockedAppealCS"];
                var lockedAppealMessage = paramLists["LockedAppealMessage"];
                var multiCompletedAppealCs = paramLists["CompletedMultiAppealCS"];
                var claimNoForMultiCompletedAppeal = paramLists["ClaimNoForMultiCompletedAppeal"];
                var claimNoForFlagVerfication = paramLists["ClaimNoForFlagVerfication"];
                var csForFlagVerfication = paramLists["CSForFlagVerfication"];

                automatedBase.CurrentPage.NavigateToAppealCreator();
                _appealCreator.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                        "Appeal creator sub menu opens Appeal Creator Claim search page.");
                _appealCreator.IsClaimSearchPanelPresent().ShouldBeTrue("Claim Search Panel is present");
                _appealCreator.GetSearchListMessageOnLeftGridForEmptyData()
                    .ShouldBeEqual(noDataMessage, "No data avaialble displayed on left portion");
                _appealCreator.ToggleSearchButtonForClaimSearch();
                _appealCreator.IsClaimSearchPanelHidden().ShouldBeTrue("Search icon controls claim search panel");
                _appealCreator.ToggleSearchButtonForClaimSearch(); //re  open search panel
                _appealCreator.GetClaimSequenceInFindClaimSection()
                    .ShouldBeNullorEmpty("Claim Sequence Text Box should be blank by default");
                _appealCreator.GetClaimNoInFindClaimSection()
                    .ShouldBeNullorEmpty("Claim No Text Box should be blank by default");
                _appealCreator.GetAlternateClaimNoLabelTitleInFindPanel()
                    .ShouldBeEqual("Claim No", "title of alternate claim # equals claim no ");
                try
                {
                    _appealCreator.SetClaimSequenceInFindClaimSection("TEST");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Only numbers allowed.", "Is Correct Message Display in Popup?");
                    _appealCreator.ClosePageError();
                    _appealCreator.SetClaimSequenceInFindClaimSection("123");
                    _appealCreator.SetClaimNoInFindClaimSection("a1");
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("No error pop up open when claim no is alphanumeric. Is error present?");
                    _appealCreator.GetClaimSequenceInFindClaimSection()
                        .ShouldBeNullorEmpty(
                            "Claim Sequence Text Box should be automatically cleared when claim no is entered");
                    _appealCreator.SetClaimSequenceInFindClaimSection("1");
                    _appealCreator.GetClaimNoInFindClaimSection()
                        .ShouldBeNullorEmpty(
                            "Claim No Text Box should be automatically empty when claim sequence is filled");

                    _appealCreator.ClickOnClearLinkOnFindSection();
                    _appealCreator.ClickOnFindButton();
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page Error Present when searched by without any crieria entered");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Search cannot be initiated without any criteria entered.",
                            "Is Popup Message display Correctly with no values in Claim Seq and Claim No");
                    _appealCreator.ClosePageError();

                    _appealCreator.SearchByClaimSequence("1");
                    _appealCreator.GetEmptyMessage().ShouldBeEqual("No matching records were found.",
                        "Is Correct Message display for Invalid Claims Sequence");
                    _appealCreator.IsClaimSearchPanelPresent()
                        .ShouldBeTrue("Claim Search Panel should open when search executed");
                    _appealCreator.ClickOnClearLinkOnFindSection();
                    _appealCreator.GetClaimSequenceInFindClaimSection()
                        .ShouldBeNullorEmpty("Claim Sequence Text Box should be blank when click on clear button");
                    _appealCreator.GetClaimNoInFindClaimSection()
                        .ShouldBeNullorEmpty("Claim No Text Box should be blank when click on clear button");

                    var recentlyAddedAppeal = _appealCreator.GetRecentlyAddedApealList();
                    recentlyAddedAppeal.Count.ShouldBeEqual(5, "Is Count of Recently Added Appeals Equal?");
                    recentlyAddedAppeal.ShouldCollectionBeSorted(true, "Most recent appeals are on top");

                    _appealCreator.SearchByClaimSequence(appealLevelCs);
                    _appealCreator.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                            "Appeal creator  for creating new appeal page opened.");
                    _appealCreator.ClickOnCancelBtn();

                    Console.WriteLine("Appeal creator automatically loaded for a single unlocked claim.");

                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);
                    _appealCreator.IsAddAppealInClaimListEnabled(1).ShouldBeTrue("Add new appeal is enabled for claim");
                    _appealCreator.ClickOnCreateAppealIcon(1);
                    _appealCreator.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                            "Page should Navigate when click on Create Appeal Icon");
                    _appealCreator.ClickOnCancelBtn();
                    _appealCreator.IsAddAppealInClaimListEnabled(3)
                        .ShouldBeTrue("Add new appeal should enabled for claim having appeal that is closed");
                    _appealCreator.SearchByClaimSequence(multiAppealCs);
                    _appealCreator.GetAppealLevelBadgeOnSearchResult(1).ShouldBeGreater(1,
                        "Appeal result should have corresponding badge present for claims with more than  one appeals");
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(lockedAppealMessage,
                        "Checking Appeal message for claims with exsiting appeals not yet closed or complete");
                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNoForMultiCompletedAppeal);
                    _appealCreator.GetAppealLevelBadgeOnSearchResult(multiCompletedAppealCs).ShouldBeGreater(1,
                        "Appeal result should have corresponding badge present for claims with more than one appeals");
                    _appealCreator.ClickOnCreateAppealIcon(multiCompletedAppealCs);
                    _appealCreator.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                            "Appeal creator for creating new appeal page opened for claim sequence with multiple appeals, all closed or completed.");
                    _appealCreator.ClickOnCancelBtn();

                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNoForFlagVerfication);
                    var newClaimAction =
                        _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(csForFlagVerfication);
                    var gettopFlagList = newClaimAction.GetTopFlagList().Distinct();
                    var topFlagList = string.Join(", ", gettopFlagList.OrderBy(s => s));
                    _appealCreator.CloseAnyPopupIfExist();
                    _appealCreator.GetSearchlistComponentTooltipValue(1, 5)
                        .ShouldBeEqual(topFlagList, "Top Flag displayed");
                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);
                    _appealCreator.GetSearchlistComponentItemValue(1, 3)
                        .ShouldBeEqual(claimNo, "Client's claim no label displayed");
                    _appealCreator.GetSearchlistComponentItemLabel(1, 4)
                        .ShouldBeEqual("Provider:", "Provider label present");
                    _appealCreator.GetSearchlistComponentItemValue(1, 4)
                        .ShouldBeEqual("DOCTOR", "Provider Name is displayed");
                    _appealCreator.GetSearchlistComponentItemValue(1, 5).ShouldBeEqual("FRE", "Flag displayed");
                    _appealCreator.GetSearchlistComponentItemLabel(1, 7)
                        .ShouldBeEqual("S:", "Claim Status label present");
                    _appealCreator.GetSearchlistComponentItemValue(1, 7)
                        .ShouldBeEqual("Client Reviewed", "Claim Status displayed");
                    _appealCreator.GetSearchlistComponentItemLabel(1, 8)
                        .ShouldBeEqual("Adj:", "Adjustment status label present");
                    _appealCreator.GetSearchlistComponentItemValue(1, 8)
                        .ShouldBeEqual("O", "Adjustment Status displayed");
                    _appealCreator.GetClaimSeqListOnSearchResult()
                        .ShouldCollectionBeSorted(true, "Iterations are list with most recent claim at top");
                    _appealCreator.SearchByClaimSequence(claseq);
                    var claimLineCount = _appealCreator.GetClaimLinesCount();
                    newClaimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                    claimLineCount.ShouldBeEqual(newClaimAction.GetCountOfClaimLines(),
                        "Claim line of Claim should be loaded to appeal creator and count should be equal");



                }
                finally
                {
                    _appealCreator.CloseAnyPopupIfExist();
                }
            }
        }




        [Test, Category("AppealDependent"), Category("OnDemand")] //us41372
        [NonParallelizable]
        public void Verify_appeal_creator_lock_messages()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);

                var lockedAppealCs = paramLists["LockedAppealCS"];
                var lockedClaimMessageForCotiviti = paramLists["LockedClaimMessageForCotiviti"];
                var lockedAppealMessage = paramLists["LockedAppealMessage"];
                var clientUnreleasedCs = paramLists["ClientUnreleasedCS"];
                var allLinesZeroBilledCs = paramLists["AllLinesZeroBilledCS"];
                var clientUnreleasedAppealLockMessage = paramLists["ClientUnreleasedAppealLockMessage"];
                var clientUnreleasedAppealLockMessageForClient =
                    paramLists["ClientUnreleasedAppealLockMessageForClientUser"];
                var clientUnreleasedClaimLockMessage = paramLists["ClientUnreleasedLockMessage"];
                var billedZeroLockMessage = paramLists["BilledZeroLockMessage"];
                var noFlagCs = paramLists["NoFlagCS"];
                var lockedNoFlagMessage = paramLists["LockedNoFlagMessage"];
                var allowBillableFlag = false;
                try
                {
                    _appealCreator.SearchByClaimSequence(lockedAppealCs);
                    _appealCreator.GetClaimActionLockToolTip().ShouldBeEqual(lockedClaimMessageForCotiviti,
                        "Checking Claim Lock message for locked claim.");
                    _appealCreator.IsAddAppealInClaimListDisabled(1)
                        .ShouldBeTrue("Add new appeal is disabled for claim");
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(lockedAppealMessage,
                        "Checking Appeal message for locked claim/disabled appeal");
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='N'", ClientEnum.SMTST.ToString());
                    _appealCreator.RefreshPage();
                    _appealCreator.SearchByClaimSequence(noFlagCs);
                    //lock messsage should be in lock icon, currently story on develop so checking it for appeal since message showing in appeal icon
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(lockedNoFlagMessage,
                        "Checking Claim Lock message for claim with no flags.");
                    //_appealCreator.SearchByClaimSequence(clientUnreleasedCs);
                    //_appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(clientUnreleasedAppealLockMessage, "Checking Appeal Lock message for claims not yet released to client.");
                    _appealCreator.SearchByClaimSequence(allLinesZeroBilledCs);
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(billedZeroLockMessage,
                        "Checking Appeal Lock message for claim with all lines having billed amount zero.");
                    _appealCreator.IsLineUnselectableForLockedClaimInSearchGrid()
                        .ShouldBeTrue("Line should be disabled for a locked claim");

                    automatedBase.CurrentPage = _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(noFlagCs);
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(lockedNoFlagMessage,
                        "Checking Claim Lock message for claim with no flags."); //verify lock message for appeal not comlete or closed
                    _appealCreator.SearchByClaimSequence(clientUnreleasedCs);
                    _appealCreator.GetClaimActionLockToolTip().ShouldBeEqual(clientUnreleasedClaimLockMessage,
                        "Checking Claim Lock message for claim not yet released to client."); //verify lock message for appeal not complete or closed
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(clientUnreleasedAppealLockMessageForClient,
                        "Checking Appeal Lock message for claims not yet released to client.");
                    _appealCreator.SearchByClaimSequence(allLinesZeroBilledCs);
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(billedZeroLockMessage,
                        "Checking Appeal Lock message for claim with all lines having billed amount zero.");

                    _appealCreator
                        .Logout()
                        .LoginAsHciAdminUser();

                    //lock messsage should be in lock icon, currently story on develop so checking it for appeal since message showing in appeal icon
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='Y'", ClientEnum.SMTST.ToString());
                    allowBillableFlag = true;
                }
                finally
                {
                    _appealCreator.CloseAnyPopupIfExist();
                    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN,
                        StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        _appealCreator.Logout().LoginAsHciAdminUser();
                    }

                    if (!allowBillableFlag)
                    {
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("ALLOW_BILLABLE_APPEALS='Y'", ClientEnum.SMTST.ToString());
                    }
                }
            }
        }

        [Test]//us41372 
        public void Validate_appeal_lock_is_present_when_appeal_is_viewed_by_other_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                automatedBase.CurrentPage =
                    _appealCreator =
                        _appealCreator.Logout().LoginAsHciAdminUser4().NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var appealCreatorUrl = automatedBase.CurrentPage.CurrentPageUrl;
                var newClaimSearch = _appealCreator.NavigateToClaimSearch();
                newClaimSearch.RefreshPage();
                var newclaimAction = newClaimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                newclaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();


                string.Format("Open appeal creator page for claim sequence: {0} with from another user ", claimSeq);
                _appealCreator = _appealCreator.SwitchToOpenAppealCreatorByUrlForAdmin(appealCreatorUrl);
                _appealCreator.SearchByClaimSequence(claimSeq);
                _appealCreator.IsClaimLocked()
                    .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                _appealCreator.GetClaimActionLockToolTip()
                    .ShouldBeEqual(
                        "This claim has been opened in view mode. It is currently locked by ui automation_4 (uiautomation_4).");

                _appealCreator.GetAddAppealockToolTip()
                    .ShouldBeEqual(
                        "Claim is currently locked by ui automation_4 (uiautomation_4). An appeal cannot be opened while the claim is being edited by another user.");
            }
        }



        [Test] //US41400 + CAR2304(CAR-2135)
        public void Verify_claim_search_returns_results_on_coming_back_from_appeal_creator()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                List<string> _allProductListFromDB = _appealCreator.GetCommonSql.GetAllProductList();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var nextclaimSeq = paramLists["SecondClaimSeq"];
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_allProductListFromDB,_appealCreator).Select(s =>
                    s.Replace("FraudFinder Pro", ProductEnum.FFP.GetStringValue())).ToList();
                _appealCreator.SearchByClaimSequence(claimSeq);
                _appealCreator.GetPageHeader().ShouldBeEqual("Appeal Creator", "Appeal creator page opened for SMTST.");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList,_appealCreator);
                _appealCreator.NavigateToAppealCreatorFromCreateAppeal();
                _appealCreator.SearchByClaimSequence(nextclaimSeq);
                _appealCreator.GetPageHeader().ShouldBeEqual("Appeal Creator", "Appeal creator page opened for SMTST.");
                _appealCreator.ClickOnCancelBtn();
            }

        }



        [Test] //US41389
        public void Verify_deselected_lines_from_multiple_claims_arent_added_to_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var associatedClaimSeq = paramLists["AssociatedClaimSeq"];
                var associatedClaimSeqList = associatedClaimSeq.Split(',').ToList();

                _appealCreator.SearchByClaimSequence(claimSeq);
                if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                {
                    DeletePreviousAppeals(claimSeq,automatedBase); //to delete appeal created
                    _appealCreator.SearchByClaimSequence(claimSeq);
                }

                try
                {

                    _appealCreator.ClickOnSearchIcon();
                    foreach (var claimSequence in associatedClaimSeqList)
                    {
                        _appealCreator.SearchByClaimSequence(claimSequence);
                        _appealCreator.ClickOnAddClaimRowSection();
                        _appealCreator.WaitForWorking();
                    }

                    _appealCreator.ClickCloseIconOnFindClaimSection();
                    _appealCreator.ClickOnSelectAllCheckBox(); //select all
                    _appealCreator.ClickOnClaimLevelSection(col2, 3); //deselct 3rd row 
                    _appealCreator.ClickOnClaimLevelSection(col2, 2); //deselct 2nrd row  

                    _appealCreator.CreateAppealForClaimSequence("Coding Validation", false);
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                    {
                        _appealCreator.ClosePageError();
                        _appealCreator.SelectProduct("Coding Validation");
                        _appealCreator.ClickOnSaveBtn();
                    }

                    _appealCreator.WaitForWorking();
                    automatedBase.CurrentPage = _appealManager = _appealCreator.NavigateToAppealManager();

                    foreach (var claimSequence in associatedClaimSeqList)
                    {
                        _appealManager.SearchByClaimSequence(claimSequence);
                        _appealManager.IsNoMatchingRecordFoundMessagePresent()
                            .ShouldBeTrue("Appeal not saved for deselected lines of claim sequence " + claimSequence);
                    }

                    _appealManager.SearchByClaimSequence(claimSeq);
                    _appealManager.GetSearchResultCount().ShouldBeEqual(1,
                        "Appeal  saved for selected lines of claim sequence " + claimSeq);

                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }

        }

        [Test] //US41395
        public void Verify_unabbreviated_status_values_is_displayed_in_tooltip_for_appeal_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimNoForOrc = paramLists["ClaimNoForORC"];
                var claimNoForV = paramLists["ClaimNoForV"];
                var claimNoForCur = paramLists["ClaimNoForCUR"];
                var ClaimNoForVr = paramLists["ClaimNoForVR"];
                var ClaimNoForVur = paramLists["ClaimNoForVUr"];

                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNoForOrc);
                _appealCreator.GetSearchlistComponentItemLabel(1, 8).ShouldBeEqual("Adj:", "Adj Status label present");
                _appealCreator.GetSearchlistComponentTooltipValue(1, 8)
                    .ShouldBeEqual("Reissue", "Adj Status displayed and equals to Reissue");
                _appealCreator.GetSearchlistComponentTooltipValue(2, 8)
                    .ShouldBeEqual("Original", "Adj Status displayed and equals to Original");
                _appealCreator.GetSearchlistComponentTooltipValue(3, 8)
                    .ShouldBeEqual("Corrected", "Adj Status displayed and equals to corrected");
                _appealCreator.GetSearchlistComponentItemLabel(1, 7).ShouldBeEqual("S:", " Claim Status label present");
                _appealCreator.GetSearchlistComponentItemValue(1, 7).ShouldBeEqual("Client Reviewed",
                    "Claim Status displayed and equals to Client Reviewed");
                _appealCreator.GetSearchlistComponentTooltipValue(1, 7).ShouldBeEqual("Client Reviewed",
                    "Claim Status displayed and equals to Client Reviewed");
                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNoForV);
                _appealCreator.GetSearchlistComponentTooltipValue(1, 8)
                    .ShouldBeEqual("Voided", "Adj Status displayed and equals to Voided");
                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNoForCur);
                _appealCreator.GetSearchlistComponentTooltipValue(1, 7).ShouldBeEqual("Client Unreviewed",
                    "Claim Status displayed and equals to Client Unreviewed");
                _appealCreator.SearchByClaimNoAndStayOnSearch(ClaimNoForVr);
                _appealCreator.GetSearchlistComponentTooltipValue(2, 7).ShouldBeEqual("Cotiviti Reviewed",
                    "Claim Status displayed and equals to Cotiviti Reviewed");
                _appealCreator.SearchByClaimNoAndStayOnSearch(ClaimNoForVur);
                _appealCreator.GetSearchlistComponentTooltipValue(2, 7).ShouldBeEqual("Cotiviti Unreviewed",
                    "Claim Status displayed and equals to Cotiviti Unreviewed");

                _appealCreator.ClickOnClearLinkOnFindSection();
            }

        }


        [Test] //US44851
        public void Verify_claim_search_refreshed_after_appeal_creation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                const string lockAppealMessage =
                    "Appeals cannot be opened for claims that have an existing appeal in process.";
                try
                {
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        DeletePreviousAppeals(claimSeq,automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), note: "UITEST");
                    _appealCreator.GetAddAppealockToolTip(1).ShouldBeEqual(lockAppealMessage);
                    _appealCreator.GetSearchlistComponentItemValue(1, 2).ShouldBeEqual(claimSeq,
                        "Original search result displayed for " + claimSeq);
                    _appealCreator.IsAddAppealInClaimListDisabled(1)
                        .ShouldBeTrue("Add new appeal is disabled for claim");
                    _appealCreator.IsLineUnselectableForLockedClaimInSearchGrid()
                        .ShouldBeTrue("Line should be disabled for a locked claim");

                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //US45710 + us61129
        public void Verify_maximum_request_length_for_documents_upload()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var fileType = paramLists["FileType"].Split(';');
                var fileName = new List<string> {"TEST1.txt", "TEST2.txt", "TEST3.txt", "TEST4.txt", "TEST5.txt"};
                try
                {
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        DeletePreviousAppeals(claimSeq,automatedBase); //to delete appeal created
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    StringFormatter.PrintMessageTitle("Get the count of documents associated with claim's appeal.");
                    var docCount = Convert.ToInt32(_appealCreator.GetAssociatedDocumentCount(claimSeq)[0]);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.SelectProduct("Coding Validation");
                    _appealCreator.SelectAppealRecordType();
                    for (int i = 0; i < fileName.Count; i++)
                    {
                        string localDoc = Path.GetFullPath(string.Format("Documents/{0}", fileName[i]));
                        if (File.Exists(localDoc))
                        {
                            File.Delete(localDoc);
                        }

                        using (FileStream fs = File.Create(localDoc))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file." + (i + 1));
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);
                        }

                        AddFileForUpload(fileType[i], i + 1, fileName[i],_appealCreator);
                    }

                    //AddFileForUpload(fileType, 1, fileName);
                    //AddFileForUpload(fileType, 2, paramLists["FileName2"]);
                    //AddFileForUpload(fileType, 3, paramLists["FileName3"]);
                    //AddFileForUpload(fileType, 4, paramLists["FileName4"]);
                    //AddFileForUpload(fileType, 5, paramLists["FileName5"]);
                    _appealCreator.ClickOnSaveBtn();
                    _appealCreator.WaitForWorking();
                    _appealCreator.GetPageHeader().ShouldBeEqual("Claim Search",
                        "Appeal Created and navigated back to claim search for appeal creator");
                    Convert.ToInt32(_appealCreator.GetAssociatedDocumentCount(claimSeq)[0])
                        .ShouldBeGreater(docCount, "Document audit record has been added in database");
                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }
        }

        [Test, Category("AppealDependent")] //US50615
        public void Verify_tooltip_with_appeal_creator_icon_is_enabled_or_disabled_for_different_claim_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claimSeq = testData["ClaimSeq"].Split(',');
                var claimNo = testData["ClaimNo"];
                var lockTooltip = testData["ToolTipForClientUnreleasedCotiviti"];
                try
                {
                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is enabled for Client Unreviewed claims");
                    _appealCreator.GetSearchlistComponentItemValueByClaimseq(claimSeq[0], 7)
                        .ShouldBeEqual("Client Unreviewed", "Status of claim is client unreviewed.");
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[0])
                        .ShouldBeFalse("Ceate Appeal Icon is enabled?");
                    Console.WriteLine(
                        "Verifying that all previous appeals has status closed or complete for Client Unreviewed");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[0], true,_appealCreator);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is enabled for Client Reviewed claims");
                    _appealCreator.GetSearchlistComponentItemValueByClaimseq(claimSeq[1], 7)
                        .ShouldBeEqual("Client Reviewed", "Status of claim is client reviewed.");
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[0])
                        .ShouldBeFalse("Create Appeal Icon is enabled?");
                    Console.WriteLine("Verifying that all previous appeals has status closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[1], true,_appealCreator);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is disabled for Client Reviewed claims with previous appeals not closed or complete");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[2]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[2])
                        .ShouldBeTrue("Create Appeal Icon is disabled?");
                    _appealCreator.GetAppealDisabledToolTipValue(claimSeq[2])
                        .ShouldBeEqual(testData["ToolTipForClaimWithAppealProcess"],
                            "Tooltip of Create Appeal Icon for Claim with having appeals not closed or completed");
                    Console.WriteLine("Verifying that all previous appeals are not in status of closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[2], false,_appealCreator);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is disabled for Client Unreviewed claims with previous appeals not closed or complete");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[3]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[3])
                        .ShouldBeTrue("Create Appeal Icon is disabled?");
                    _appealCreator.GetAppealDisabledToolTipValue(claimSeq[3])
                        .ShouldBeEqual(testData["ToolTipForClaimWithAppealProcess"],
                            "Tooltip of Create Appeal Icon for Claim with having appeals not closed or completed");
                    Console.WriteLine("Verifying that all previous appeals has status closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[3], false,_appealCreator);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator for those claims that are not Client Reviewed or Client Unreviewed");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[4]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[4])
                        .ShouldBeFalse("Create Appeal Icon is disabled?");

                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }
        }


        //[Test] //US50616
        public void Verify_status_of_claim_when_creating_RR_or_Appeal_type_appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimActionPage _claimAction;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claimSeq = testData["ClaimSeq"].Split(',');
                var disabledTooltip = testData["ToolTipForCUrDisabledAppealType"];
                _appealCreator.SearchByClaimSequence(claimSeq[0]);
                try
                {
                    _claimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                    _claimAction.GetClaimStatus().ShouldBeEqual("Client Unreviewed",
                        "Status of claim is as required: client unreviewed.");
                    _appealCreator.CloseAnyPopupIfExist();
                    _appealCreator.IsRespectiveAppealTypeSelected("R")
                        .ShouldBeTrue("Record Review selected by default for Client unreviewed claims");
                    _appealCreator.IsAppealTypeDisabled("A")
                        .ShouldBeTrue("Appeal typpe as Appeal is disabled for client unreviewed claims");
                    _appealCreator.GetToolTipForDisabledAppealType("A")
                        .ShouldBeEqual(disabledTooltip,
                            "Tool tip message for client unreviewed claims record type selection");
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                    _appealCreator.SearchByClaimSequence(claimSeq[1]);
                    _claimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                    _claimAction.GetClaimStatus().ShouldBeEqual("Client Reviewed",
                        "Status of claim is as required: client Reviewed.");
                    _appealCreator.CloseAnyPopupIfExist();
                    _appealCreator.IsRespectiveAppealTypeSelected("A")
                        .ShouldBeTrue("Appeal type record selected by default for Client unreviewed claims");
                    _appealCreator.IsAppealTypeDisabled("A")
                        .ShouldBeFalse("Appeal type as Appeal should not be disabled for client reviewed claims");
                    _appealCreator.SelectAppealRecordType();
                    _appealCreator.IsRespectiveAppealTypeSelected("A")
                        .ShouldBeTrue("Appeal record type should be selectable for Client reviewed claims");
                    _appealCreator.SelectRecordReviewRecordType();
                    _appealCreator.IsRespectiveAppealTypeSelected("R")
                        .ShouldBeTrue("Record Review type should be selectable for Client reviewed claims");
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }

                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }
        }

        [Test, Category("Upload_Document")]//US41367 story broken to us61129
        public void Verify_uploading_documents_functionality_in_appeal_creator()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                FileUploadPage FileUploadPage;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                FileUploadPage = _appealCreator.GetFileUploadPage;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var fileType = paramLists["FileType"];
                var fileToUpload = paramLists["FileToUpload"];
                var maxNote = paramLists["MaxNote"];
                var product = paramLists["Product"];

                try
                {
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.SelectProduct(product);
                    _appealCreator.SetNote(maxNote);
                    FileUploadPage.SetFileUploaderFieldValue("Description", "Test Description");
                    FileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeTrue(
                            "Add file button should be disabled, unless atleast a file has been uploaded, must be true?");
                    FileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    FileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeFalse(
                            "Add file button should be enabled, when atleast a file has been uploaded, must be false?");

                    FileUploadPage.ClickOnAddFileBtn();
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                    _appealCreator.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "At least one Document Type selection is required before the files can be added.",
                            "Expected error message on zero file type selection");
                    _appealCreator.ClosePageError();

                    FileUploadPage.SetFileUploaderFieldValue("Description", " ");
                    FileUploadPage.SetFileTypeListVlaue(fileType.Split(';')[0]);
                    FileUploadPage.GetPlaceHolderValue().ShouldBeEqual("Provider Letter", "File Type Text");
                    FileUploadPage.ClickOnAddFileBtn();
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Page error pop up should not be present if no description is set as its not a required field");
                    FileUploadPage.FileToUploadDocumentValue(1, 4)
                        .ShouldBeEqual(fileType.Split(';')[0], "Document type value is correct and present");
                    FileUploadPage.ClickOnDeleteIconInFilesToUpload(1);
                    FileUploadPage.SetFileUploaderFieldValue("Description", "Test Description");
                    FileUploadPage.SetFileTypeListVlaue(fileType.Split(';')[0]);
                    FileUploadPage.SetFileTypeListVlaue(fileType.Split(';')[1]);
                    //FileUploadPage.AddFileForUpload(fileToUpload.Split(';'));
                    //FileUploadPage.GetSelectedFilesValue()
                    //.ShouldCollectionBeEqual(fileToUpload.Split(';').ToList(), "Expected multiples files list is present");

                    /* --------- add single file only since multiple file upload with selenium is successfull locally only -----*/
                    FileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    FileUploadPage.ClickOnAddFileBtn();
                    FileUploadPage.FileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileToUpload.Split(';')[0], "Document file is correct and present");
                    FileUploadPage.FileToUploadDocumentValue(1, 3)
                        .ShouldBeEqual("Test Description", "Document description is correct and present");
                    FileUploadPage.FileToUploadDocumentValue(1, 4)
                        .ShouldBeEqual("Multiple File Types",
                            "Document type value a message for multiple file types selection");
                    _appealCreator.ClickOnCancelBtn();

                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();

                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }

        }

        [Test] //US69376
        public void Validate_note_character_limit_in_appeal_creator()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                _appealCreator.SearchByClaimSequence(claimSeq);
                _appealCreator.GetPageHeader().ShouldBeEqual("Appeal Creator", "Appeal creator page opened for SMTST.");
                var notes = new string('a', 4494);
                _appealCreator.SetLengthyNote("note",notes);
                _appealCreator.GetNote().Length.ShouldBeEqual(4493,
                    "Character limit should be 4494 or less, where 7 characters are separated for <p></p> tag.");
                _appealCreator.ClickOnCancelBtn();
            }
        }

        [Test] //US69378
        public void Search_for_claim_by_alt_claim_no()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                var altClaimNoMultipleResult = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName,
                    "AltClaimNoMultipleResult", "Value");
                var altClaimNoSingleResult = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName,
                    "AltClaimNoSingleResult", "Value");

                _appealCreator.GetSideBarPanelSearch.SetInputFieldByLabel("Other Claim Number",
                    altClaimNoMultipleResult);
                _appealCreator.ClickOnFindButton();
                _appealCreator.GetPageHeader().ShouldBeEqual("Claim Search", "For multiple search results, " +
                                                                             "the results should be displayed in the same page ");
                _appealCreator.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0,
                    "For multiple search results, all claims associated with alternate claim no should be " +
                    "shown in the search page");

                _appealCreator.SearchByOtherClaimNoAndNavigateToAppealCreator3ColumnPage(altClaimNoSingleResult);
                _appealCreator.GetPageHeader().ShouldBeEqual("Appeal Creator", "For single search results, " +
                                                                               "the user should be directed to Appeal Creator page");
                _appealCreator.ClickOnCancelBtn();
                if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClickOnOkButtonOnConfirmationModal();
                _appealCreator.GetPageHeader().ShouldBeEqual("Claim Search",
                    "If the user clicks cancel in the 3 column Appeal Creator user is redirected back to the search result");
            }
        }

        [Test,Category("OnDemand")] // CAR-204
        [NonParallelizable]
        [Retrying(Times = 3)]
        public void Verify_Client_Can_Appeal_Core_Flags_Setting_Functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var settingClient = true;
                try
                {
                    var claseqWithCoreFlagAtTop = paramLists["claseqWithCoreFlagAtTop"];
                    var claseqWithCoreFlagOnlyFlag = paramLists["claseqWithCoreFlagOnlyFlag"];

                    StringFormatter.PrintMessageTitle("Enabling Client Can Appeal Core Flags Option For Client");

                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());

                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealCoreFlags.GetStringValue(), false);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    settingClient = false;
                    StringFormatter.PrintMessageTitle(
                        "Verify Line is Not selecteable when the core flag is the top flag");
                    _clientSearch.NavigateToAppealCreator();
                    _appealCreator.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",
                        claseqWithCoreFlagAtTop);
                    _appealCreator.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCreator.WaitForWorkingAjaxMessage();
                    _appealCreator.ClickOnCreateAppealIcon(1);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.GetPageErrorMessage().ShouldBeEqual(
                        "Lines with CORE flags cannot be added to an appeal.",
                        "Error message should be displayed when attempting to selecting line where core flag is top flag");
                    _appealCreator.ClosePageError();

                    StringFormatter.PrintMessageTitle(
                        "Verify Line is Not selecteable when the core flag is the only flag in the line");
                    _appealCreator.ClickOnCancelBtn();
                    _appealCreator.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCreator.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",
                        claseqWithCoreFlagOnlyFlag);
                    _appealCreator.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCreator.WaitForWorkingAjaxMessage();
                    _appealCreator.ClickOnCreateAppealIcon(1);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.GetPageErrorMessage().ShouldBeEqual(
                        "Lines with CORE flags cannot be added to an appeal.",
                        "Error message should be displayed when attempting to selecting line where core flag is the only flag in the line");
                    _appealCreator.ClosePageError();

                    StringFormatter.PrintMessageTitle(
                        "Verify Line is selecteable when the core flag is not the top flag in the line");

                    _appealCreator.ClickOnClaimLine(2);
                    _appealCreator.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "Error message should not be displayed when attempting to selecting line where core flag is not the top flag in the line");

                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());

                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealCoreFlags.GetStringValue());
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    settingClient = true;
                    _clientSearch.NavigateToAppealCreator();
                    _appealCreator.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",
                        claseqWithCoreFlagOnlyFlag);
                    _appealCreator.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCreator.WaitForWorkingAjaxMessage();
                    _appealCreator.ClickOnCreateAppealIcon(1);

                    StringFormatter.PrintMessageTitle(
                        "Verify Line is selecteable when the core flag is the only flag in the line after enabling Client can appeal core flag");

                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Error message should not be displayed when attempting to selecting line where core flag is the only flag in the line");

                    StringFormatter.PrintMessageTitle(
                        "Verify Line is selecteable when the core flag is the top flag after enabling Client can appeal core flag");
                    _appealCreator.NavigateToAppealCreator();
                    _appealCreator.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCreator.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",
                        claseqWithCoreFlagAtTop);
                    _appealCreator.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCreator.WaitForWorkingAjaxMessage();
                    _appealCreator.ClickOnCreateAppealIcon(1);
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Error message should not be displayed when attempting to selecting line where core flag is top flag");

                }
                finally
                {
                    if (!settingClient)
                    {
                        _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                            ClientSettingsTabEnum.Product.GetStringValue());

                        _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealCoreFlags.GetStringValue());
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _clientSearch.NavigateToAppealCreator();
                    }

                }
            }

        }

        [Test,Category("OnDemand")] //CAR-832 (CAR-524)
        [NonParallelizable]
        [Retrying(Times=3)]
        public void Verify_Allow_Internal_User_To_Create_Dental_Review_Type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;

                var paramsList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var reviewedClaimSeq = paramsList["ClientReviewedClaimSeq"];
                var pendedClaimSeq = paramsList["PendedClaimSeq"];
                var analyst = paramsList["Analyst"];
                var expectedCategoryId = "";
                var expectedAnalyst = new List<string>();
                var expectedDueDate = _appealCreator.CalculateAndGetAppealDueDateFromDatabase("1");

                try
                {
                    StringFormatter.PrintMessage("Clearing up prior appeals if any");
                    DeletePreviousAppeals(pendedClaimSeq,automatedBase);

                    StringFormatter.PrintMessage("Clearing up prior appeals if any");
                    var _appealCategoryManager = CreateNewCategoryIfAppealCategoryDoesNotExist(analyst,_appealCreator);
                    expectedCategoryId = _appealCategoryManager.GetCategoryIdByProductProcCodeTrigCodePresent("DCA", "NA", "NA");
                    expectedAnalyst = _appealCategoryManager.GetAnalystListOnAnalystAssignment();

                    StringFormatter.PrintMessageTitle(" Verifying the Appeal Creator Form for Internal User ");
                    _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                    _claimSearch.RefreshPage();
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(pendedClaimSeq, true);
                    _claimAction.IsAddAppealIconEnabled().ShouldBeTrue(
                        "The 'Add Appeal icon in New Claim Action Page should be enabled even for claims outside status Client Reviewed and Client Unreviewed'");


                    _appealCreator = _claimAction.ClickOnCreateAppealIcon();

                    StringFormatter.PrintMessageTitle(" Verifying Appeal Creator 3 column Form for internal user ");
                    _appealCreator.GetCreateAppealColumnHeaderText().ShouldBeEqual("Create Dental Review");
                    _appealCreator.IsRespectiveAppealTypeSelected(AppealType.DentalReview.GetStringValue())
                        .ShouldBeTrue("Appeal type 'D' should be selected by default in the Appeal Creator form");
                    _appealCreator.IsUrgentCheckboxDisabled()
                        .ShouldBeTrue("Urgent checkbox should be disabled when appeal type 'D' is selected ");
                    _appealCreator.SelectClaimLine();

                    _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "",
                        AppealType.DentalReview.GetStringValue());
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    StringFormatter.PrintMessageTitle("Verifying Dental Appeal Data after saved");
                    VerifyAppealDataAppealAfterSave(expectedAnalyst, expectedCategoryId, expectedDueDate,_claimAction,automatedBase);

                    StringFormatter.PrintMessage(
                        " Verifying that the 'Create Appeal' icon should not be enabled for clients which do not have Dental product active ");
                    DeletePreviousAppeals(pendedClaimSeq,automatedBase);

                    _appealCreator.ChangeDCIProductForClientFromDB(ClientEnum.SMTST.ToString(), false);

                    _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                    _claimSearch.RefreshPage();
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(pendedClaimSeq);
                    _claimAction.IsAddAppealIconDisabled().ShouldBeTrue(
                        "Analysts should not be able to add appeals to Claims not in 'Client reviewed' or 'unreviewed' status for clients with Dental product inactive");

                    StringFormatter.PrintMessageTitle("Verifying appeal creator behavior for client users");
                    _appealCreator = automatedBase.CurrentPage.Logout().LoginAsClientUser().NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(pendedClaimSeq);
                    _appealCreator.IsCreateAppealDisabled()
                        .ShouldBeTrue(
                            "Client user should not be able to create appeals on claims which are not in 'Client Reviewed' or 'Client Unreviewed' status");
                    _appealCreator.SearchByClaimSequence(reviewedClaimSeq);
                    _appealCreator.IsAppealTypeRadioButtonPresent("D")
                        .ShouldBeFalse("Appeal Type 'D' should not be present for client users");

                    _appealCreator.Logout().LoginAsHciAdminUser();
                }

                finally
                {
                    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN,
                        StringComparison.OrdinalIgnoreCase) != 0)
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                    _appealCreator.ChangeDCIProductForClientFromDB(ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage.NavigateToAppealCreator();
                    DeletePreviousAppeals(pendedClaimSeq,automatedBase);
                }
            }
        }


        [Test, Category("SmokeTestDeployment")]//TANT-94
        public void Verify_Appeal_Creator_Page_on_Popup()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimNo = paramLists["ClaimNumber"];
                var fileType = paramLists["FileType"];
                var fileName = paramLists["FileName"];

                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);
                var claimSequence = _appealCreator.GetEnableClaimSequence();
                var claimActionPopup =
                    _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claimSequence);
                var appealCreatorPopup = claimActionPopup.ClickOnCreateAppealIcon();
                appealCreatorPopup.ClickOnClaimLine(1);
                var list = appealCreatorPopup.GetSelectedClaimLinesByHeader(col1);
                list.Count.ShouldBeEqual(1, "Is Claim Line Selected?");
                appealCreatorPopup.GetSelectedClaimLinesByHeader(col2).ShouldCollectionBeEqual(list,
                    "Is claim Line Equal on Select Appeal Lines and Selected Lines");
                AddFileForUpload(fileType, 1, fileName,_appealCreator);
                var claimActionPopUp = appealCreatorPopup.ClickOnClaimSequenceAndSwitchWindow();
                claimActionPopUp.GetPageHeader().ShouldBeEqual("Claim Action");
                claimActionPopUp.SwitchBack();
                appealCreatorPopup.ClickOnCancelBtn();
                if (appealCreatorPopup.IsPageErrorPopupModalPresent())
                    appealCreatorPopup.ClickOnOkButtonOnConfirmationModal();
                _appealCreator.CloseAnyTabIfExist();
            }
        }

        

        [Test] //TE-915 + CAR-3133 [CAR-3262]
        public void Verify_Creation_Of_Appeal_With_External_DocumentId_And_File_Type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimActionPage _claimAction;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claseq = paramLists["ClaimSequence"];
                var fileType = DentalAppealDocTypeEnum.ChartNotes.GetStringValue();
                var docId = paramLists["DocID"];
                var reasonCode = paramLists["ReasonCode"];

                List<string> expectedFileTypeList = new List<string>();
                foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                {
                    var preAuthFileType = fileTypeEnum.GetStringValue();
                    expectedFileTypeList.Add(preAuthFileType);
                }

                try
                {
                    _appealCreator.DeleteAppealDocument(claseq.Replace("-", ""));
                    var _appealManager = _appealCreator.NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(claseq.Split('-')[0]);
                    _appealManager.NavigateToAppealCreator();

                    _appealCreator.SearchByClaimSequence(claseq);
                    _appealCreator.SelectClaimLine();

                    //CAR-3133 [CAR-3262]
                    _appealCreator.SelectProduct(ProductEnum.DCA.GetStringValue());
                    _appealCreator.GetAvailableFileTypeList().ShouldCollectionBeEqual(expectedFileTypeList, "File Types list is correctly displayed for DCA product");

                    _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), docId, appealType: "D", fileType: fileType);

                    _appealCreator.GetAppealDocumentType(claseq.Replace("-", ""))[0]
                        .ShouldBeEqual((int) DentalAppealDocTypeEnum.ChartNotes, "Value correctly stored?");
                    _claimAction = _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claseq);
                    _claimAction.ClickOnViewAppealIcon();
                    var appealSeq = _claimAction.GetAppealSequence();
                    _appealAction = _claimAction.ClickOnAppealSequence(appealSeq);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.GetDocTypeInAppealLetterBody()[0].ShouldBeEqual(fileType, "File type correct?");
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.CompleteAppeals(reasonCode: reasonCode, rationale: "rationale");
                    _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    var _appealSummary = _appealSearch.SearchByAppealSequenceAndNavigateToAppealSummaryPage(appealSeq.Split('-')[0]);
                    var appealLetter = _appealSummary.ClickAppealLetter();
                    automatedBase.CurrentPage.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue());
                    appealLetter.GetAppealLetterFullDetail().AssertIsContained(fileType, "File type available?");
                    _appealSearch = appealLetter.CloseLetterPopUpPageAndBackToAppealSearch();
                    _appealSearch.CloseAnyPopupIfExist();
                }

                finally
                {
                    automatedBase.CurrentPage.NavigateToAppealManager().DeleteAppealsAssociatedWithClaim(claseq.Split('-')[0]);
                    _appealCreator.DeleteAppealDocument(claseq.Replace("-", ""));
                }
            }
        }

        [Test] //TE-915
        [NonParallelizable]
        public void Verify_Creation_Of_Appeal_With_External_DocumentId_And_Multiple_File_Type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                ClaimActionPage _claimAction;
                AppealActionPage _appealAction;
                AppealSearchPage _appealSearch;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claseq = paramLists["ClaimSequence"];
                var docId = paramLists["DocID"];
                var reasonCode = paramLists["ReasonCode"];

                List<string> appealDocType = new List<string>();
                appealDocType.Add(DentalAppealDocTypeEnum.ClaimImage.GetStringValue());
                appealDocType.Add(DentalAppealDocTypeEnum.ChartNotes.GetStringValue());

                _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                _appealCreator.DeleteAppealDocument(claseq.Replace("-", ""));
                _appealCreator.SearchByClaimSequence(claseq);
                _appealCreator.SelectClaimLine();
                _appealCreator.CreateAppealWithMultipleDocType(ProductEnum.DCA.GetStringValue(), docId, appealType: "D",
                    fileType: appealDocType);
                _appealCreator.GetAppealDocumentType(claseq.Replace("-", ""))
                    .ShouldCollectionBeEquivalent(
                        new List<int> {(int)DentalAppealDocTypeEnum.ClaimImage, (int)DentalAppealDocTypeEnum.ChartNotes },
                        "Value correctly stored?");
                _claimAction = _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claseq);
                _claimAction.ClickOnViewAppealIcon();
                var appealSeq = _claimAction.GetAppealSequence();
                _appealAction = _claimAction.ClickOnAppealSequence(appealSeq);
                _appealAction.ClickOnAppealLetter();
                _appealAction.GetDocTypeInAppealLetterBody()
                    .ShouldCollectionBeEquivalent(appealDocType, "File type correct?");
                _appealAction.ClickOnAppealLetter();
                _appealAction.CompleteAppeals(reasonCode: reasonCode, rationale: "rationale");
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                var _appealSummary =
                    _appealSearch.SearchByAppealSequenceAndNavigateToAppealSummaryPage(appealSeq.Split('-')[0]);
                var appealLetter = _appealSummary.ClickAppealLetter();
                appealLetter.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue());
                appealLetter.GetDocTypeInAppealLetterBody(true)
                    .ShouldCollectionBeEquivalent(appealDocType, "File type available?");
                _appealSearch = appealLetter.CloseLetterPopUpPageAndBackToAppealSearch();
                _appealSearch.CloseAnyPopupIfExist();
            }


        }

        [Test] //CAR-3027(CAR-3013)
        public void Verify_Appeal_Category_Assignment_While_Creating_Appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCreatorPage _appealCreator;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage = _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claimseq = paramLists["ClaimSequence"].Split(',').ToList();
                StringFormatter.PrintMessage("Verify Correct category code is set in appeal based on Top Flag value and lower category order");
                foreach (var claseq in claimseq)
                {
                    _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                    _appealCreator.SearchByClaimSequence(claseq);
                    _appealCreator.WaitForWorking();
                    _appealCreator.ClickOnSelectAllCheckBox();
                    _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "DocID");
                    var category = _appealCreator.GetAppealCategoryforAppealWithClaseq(claseq.Replace("-", ""));
                    _appealSearch = _appealCreator.NavigateToAppealSearch();
                    _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "SMTST");
                    _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claseq);
                    _appealAction.GetAppealCategory().ShouldBeEqual(category,
                        "Category value correct based on Top Flag and lower category order among competing flags ?");
                    _appealCreator = _appealAction.NavigateToAppealCreator();
                }
            }

           

        }



        #endregion

        #region Private Methods

        private AppealCategoryManagerPage CreateNewCategoryIfAppealCategoryDoesNotExist(string analyst,AppealCreatorPage _appealCreator)
        {
            // Creating Appeal Category for DCA if not already present
            var _appealCategoryManager = _appealCreator.NavigateToAppealCategoryManager();
            if (!_appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent("DCA", "NA", "NA"))
                _appealCategoryManager.CreateAppealCategoryForMultipleAnalystForDci(new List<string>() { analyst });

            _appealCategoryManager.ClickOnAppealRowCategoryByProductProcCodeTrigCodePresent("DCA", "NA", "NA");

            return _appealCategoryManager;
        }

        private void VerifyAppealDataAppealAfterSave(List<string> expectedAnalyst, string expectedCategoryId, string expectedDueDate,ClaimActionPage _claimAction,NewAutomatedBaseParallelRun automatedBase, string product = "Dental", string appealType = "Dental Review", string productAbbreviation = "D", bool mrrAppeal = false)
        {
            AppealSearchPage _appealSearch;
            AppealActionPage _appealAction;
            AppealProcessingHistoryPage _appealProcessingHistory;
            StringFormatter.PrintMessage($"Verifying the {product} appeal data from New Claim Action after it is saved ");
            _claimAction.ClickOnViewAppealIcon();
            var appealSeq = _claimAction.GetAppealSequence().Split('-')[0];
            _claimAction.GetAppealStatusByRow(1).ShouldBeEqual("New", "Appeal Status is showing correctly as 'New' in Claim Action Page");
            _claimAction.GetAppealTypeByRow(1).ShouldBeEqual(appealType, $"Appeal Type is showing up as '{appealType}' in Claim Action Page");

            StringFormatter.PrintMessageTitle(" Performing appeal search by selecting 'Type' as one of the search filters ");
            _appealSearch = _claimAction.NavigateToAppealSearch();
            _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
            _appealSearch.SelectClientSmtst(true);
            _appealSearch.SetInputFieldByInputLabel("Status", "New");
            _appealSearch.SetInputFieldByInputLabel("Type", appealType);
            _appealAction = _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeq);
            _appealAction.GetPageHeader().ShouldBeEqual("Appeal Action", $"The {product} appeal can be searched using the Appeal Type filter");

            StringFormatter.PrintMessage($"Verifying the {product} appeal data from Appeal Action after it is saved ");
            if (mrrAppeal)
            {
                _appealAction.IsAppealActionHeaderForMRRAppealTypePresent()
                    .ShouldBeTrue("Is MRR Appeal type header present?");
            }

            _appealAction.GetPrimaryReviewer().ShouldBeEqual(expectedAnalyst[0].Split('(')[0].Trim(),
                "Primary Reviewer should be assigned per Appeal Category Assignment");
            _appealAction.GetAssignedTo().ShouldBeEqual(expectedAnalyst[0].Split('(')[0].Trim(),
                "Assigned To should be assigned per Appeal Category Assignment");
            _appealAction.GetAppealCategory().ShouldBeEqual(expectedCategoryId,
                "Category ID should be assigned per Appeal Category Assignment");
            _appealAction.GetDueDate().ShouldBeEqual(expectedDueDate,
                "Due Date should be the next business day, excluding company holidays");
            _appealAction.GetStatus().ShouldBeEqual("New", "Appeal Status should be 'New' ");

            StringFormatter.PrintMessage(" Verifying the {product} appeal priority and product from Appeal HX after it is saved ");
            _appealProcessingHistory = _appealAction.ClickAppealProcessingHx();
            _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 13)
                .ShouldBeEqual("Normal", "Appeal priority should be 'Normal' ");
            _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 14)
                .ShouldBeEqual(productAbbreviation, $"'{product} product should be saved for the appeal'");
            automatedBase.CurrentPage = _appealAction =
                _appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();
        }

        private void VerifyValidationMessage(string title,AppealCreatorPage _appealCreator)
        {

            _appealCreator.ClickOnSaveBtn();
            _appealCreator.GetPageErrorMessage().ShouldBeEqual(
                "Invalid or missing data must be entered before record can be saved.", title);
            _appealCreator.ClosePageError();
        }
        private void VerifyValidationMessageForNoLineSelected(string title, AppealCreatorPage _appealCreator)
        {
            _appealCreator.ClickOnSaveBtn();
            _appealCreator.GetPageErrorMessage().ShouldBeEqual(
                "At least one claim line must be selected before the appeal record can be saved.", title);
            _appealCreator.ClosePageError();
        }
        private void VerifyValidationMessageForAppealNoAlphanumeric(string title, AppealCreatorPage _appealCreator)
        {
            _appealCreator.GetPageErrorMessage().ShouldBeEqual(
                "Only alphanumerics allowed.", title);
            _appealCreator.ClosePageError();
        }
        


        private void AddFileForUpload(string fileType, int row, string fileName,AppealCreatorPage _appealCreator)
        {
            _appealCreator.PassGivenFileNameFilePathForDocumentUpload(fileName);
            _appealCreator.SetFileTypeListVlaue(fileType);
            _appealCreator.SetAppealCreatorFieldValue("Description", "test appeal doc");
            _appealCreator.ClickOnAddFileBtn();
            _appealCreator.WaitForStaticTime(1500);
            _appealCreator.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
            _appealCreator.FileToUploadDocumentValue(row, 2)
                .ShouldBeEqual(fileName, "Document correct and present");

        }


        private void CheckClaimSequenceStatusOfAppeal(string claimSeq, bool checkTrueFalse,AppealCreatorPage _appealCreator)
        {
            ClaimActionPage _claimAction;
            _claimAction = _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claimSeq);
            _claimAction.ClickOnViewAppealIcon();
            var countOfCloseCompletedAppeals = 0;
            var appealList = _claimAction.GetListOfAppealSequenceInRows();
            foreach (var appealSeq in appealList)
            {
                var status = _claimAction.GetAppealStatus(appealSeq);
                if (checkTrueFalse)
                    (status.Equals("Closed") || status.Equals("Complete")).ShouldBeTrue(string.Format("All previous appeal <{0}> are in status of closed or complete", appealSeq));
                else if (status.Equals("Closed") || status.Equals("Complete"))
                {
                    Console.WriteLine("status of " + appealSeq + "<{0}>", status);
                    countOfCloseCompletedAppeals++;
                }

            }
            countOfCloseCompletedAppeals.ShouldBeLess(appealList.Count, "All Previous appeals are not closed or completed.");
            _appealCreator.CloseAnyPopupIfExist();
        }

        //private void DeleteLastCreatedAppeal(string claimSeq)
        //{
        //    if (automatedBase.CurrentPage.GetPageHeader() != "Appeal Manager")
        //    {
        //        automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
        //    }
        //    _appealManager.DeleteLastCreatedAppeal(claimSeq);
        //    automatedBase.CurrentPage =
        //        _appealCreator = _appealManager.NavigateToAppealCreator();
        //}
        private void DeletePreviousAppeals(string claimSeq,NewAutomatedBaseParallelRun automatedBase)
        {
            AppealManagerPage _appealManager = null;
            AppealCreatorPage _appealCreator;
            if (automatedBase.CurrentPage.GetPageHeader() != "Appeal Manager")
            {
                automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
            }
            _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
            automatedBase.CurrentPage =
                     _appealCreator = _appealManager.NavigateToAppealCreator();
        }
        private void DeletePreviousAppeals(List<string> claimSeq,NewAutomatedBaseParallelRun automatedBase)
        {
            AppealCreatorPage _appealCreator;
            AppealManagerPage _appealManager = null;
            if (automatedBase.CurrentPage.GetPageHeader() != "Appeal Manager")
            {
                automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
            }
            _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
            automatedBase.CurrentPage =
                     _appealCreator = _appealManager.NavigateToAppealCreator();
        }

        private List<String> GetExpectedProductListFromDatabase(List<string> allProductList,AppealCreatorPage _appealCreator)
        {
            List<string> expectedProductTypeList = new List<string>();
            List<string> _activeProductListForClient = (List<string>)_appealCreator.GetCommonSql.GetActiveProductListForClient(ClientEnum.SMTST.ToString());

            for (int i = 0; i < _activeProductListForClient.Count; i++)
            {
                if (_activeProductListForClient[i] == "T")
                {
                    expectedProductTypeList.Add(allProductList[i]);
                }
            }

            expectedProductTypeList.Insert(0, "");
            return expectedProductTypeList;
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual,AppealCreatorPage _appealCreator)
        {
            var actualDropDownList = _appealCreator.GetSideWindow.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEquivalent(collectionToEqual, label + " List As Expected");
            
            _appealCreator.GetSideWindow.SelectDropDownValue(label, actualDropDownList[1], false); //check for type ahead functionality
            _appealCreator.GetSideWindow.SelectDropDownValue(label, actualDropDownList[2]);
            _appealCreator.GetSideWindow.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
            
        }

        #endregion
    }
}
