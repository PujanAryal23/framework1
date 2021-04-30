using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Report;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Environment;
using NUnit.Framework.Internal;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClientSearch
    {
        #region Private
        /*
                private ClientSearchPage _clientSearch;
                private CommonValidations _commonValidation;
                private readonly string _clientMaintenance = RoleEnum.NucleusAdmin.GetStringValue();

                private List<string> _allClientList;
                private List<string> _activeProductListForClientDB;
                private LogicSearchPage _logicSearch;
                private ReportCenterPage _reportCenter;
                private FileUploadPage FileUploadPage;
                private ClaimActionPage _claimAction;
                private QuickLaunchPage quickLaunch;
                private AppealSearchPage _appeasearchpage;
                private  AppealActionPage _appealaction;
                private AppealCreatorPage _appealCreator;
                private string claimSeq;*/

        List<string> timeOptionsList = new List<string>
        {
            "12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM",
            "6:00 AM", "7:00 AM", "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM",
            "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM", "4:00 PM", "5:00 PM",
            "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"
        };

        private List<string> _allClientList;
        private List<string> _activeProductListForClientDB;

        #endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region DBinteraction methods

        private void RetrieveListFromDatabase(ClientSearchPage _clientSearch)
        {
            _allClientList = _clientSearch.GetAllClientList();
            _allClientList.Insert(0, "All");
            _activeProductListForClientDB = _clientSearch.GetActiveProductListForClientDB();
        }



        #endregion

        #region OVERRIDE 


        /*protected override void ClassInit()
        {
            try
            {

                base.ClassInit();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                claimSeq = paramLists["ClaimSequence"];
                CurrentPage = _clientSearch = CurrentPage.NavigateToClientSearch();
                FileUploadPage = _clientSearch.GetFileUploadPage;
                _commonValidation = new CommonValidations(CurrentPage);
                RetrieveListFromDatabase();

            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }*/


        /*protected override void TestInit()
        {

            base.TestInit();

        }*/

        /*protected override void ClassCleanUp()
        {
            try
            {
                _clientSearch.CloseDatabaseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
            
            base.ClassCleanUp();
        }*/

        /*protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _clientSearch = _clientSearch.Logout().LoginAsHciAdminUser().NavigateToClientSearch();
            }

            if (!CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
            {
                CheckTestClientAndSwitch();
                CurrentPage.NavigateToClientSearch();
            }

            if (_clientSearch.GetPageHeader() != PageHeaderEnum.ClientSearch.GetStringValue())
            {
                _clientSearch.ClickOnQuickLaunch().NavigateToClientSearch();
            }
            _clientSearch.SideBarPanelSearch.OpenSidebarPanel();
            _clientSearch.SideBarPanelSearch.ClickOnClearLink();
            
        }*/


        #endregion

        #region TestSuites

        #region On-Demand Tests

        [NonParallelizable]
        [Test] //CAR-3289(CAR-3257)
        [Author("Shreya Shrestha")]
        public void Verify_MRR_record_request_expiration()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());
                string updatedValue = "10";
                try
                {
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.EnableMedicalRecordsReviews
                        .GetStringValue());

                    StringFormatter.PrintMessage("Default values verification");
                    _clientSearch.IsMRRRecordReviewExpirationLabelPresent("MRR Record Request Expiration")
                        .ShouldBeTrue(
                            "Is MRR Record Request Expiration present when Medical Record Review is enabled?");
                    _clientSearch.GetMRRRecordReviewExpirationInputValue()
                        .ShouldBeEqual("30", "Default value should equal to 30");
                    _clientSearch.IsMRRRecordReviewExpirationLabelPresent("Business Days")
                        .ShouldBeTrue("Is Business Days label present?");

                    StringFormatter.PrintMessage("Validation of input field");
                    ValidateInputField("101", "Value cannot be greater than 100.");
                    ValidateInputField("a", "Only numbers allowed.");
                    ValidateInputField("!", "Only numbers allowed.");

                    StringFormatter.PrintMessage("Verification of Save");
                    _clientSearch.SetMRRRecordReviewExpirationInputValue(updatedValue);
                    _clientSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Error message should not be present while entering value less than 100");
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetMRRRecordReviewExpirationInputValue()
                        .ShouldBeEqual(updatedValue, "Updated value should match");
                    _clientSearch.GetCommonSql.GetClientSettingsByColNames("mrr_expiration_days", ClientEnum.SMTST.ToString())[0]
                        .ShouldBeEqual(updatedValue, "Updated value must be reflected in database");

                    StringFormatter.PrintMessage("Verification when MRR is disabled");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.EnableMedicalRecordsReviews
                        .GetStringValue(),false);
                    _clientSearch.IsMRRRecordReviewExpirationLabelPresent("MRR Record Request Expiration")
                        .ShouldBeFalse(
                            "Is MRR Record Request Expiration present when Medical Record Review is disabled?");
                    _clientSearch.GetSideWindow.Cancel();
                }

                finally
                {
                    StringFormatter.PrintMessage("Setting values to default");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.EnableMedicalRecordsReviews
                        .GetStringValue());
                    _clientSearch.SetMRRRecordReviewExpirationInputValue("30");
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                }

                void ValidateInputField(string value, string validationMessage)
                {
                    _clientSearch.SetMRRRecordReviewExpirationInputValue(value);
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual(validationMessage,
                        "Verify error message");
                    _clientSearch.ClosePageError();
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-3161(CAR-3210)
        [Author("Shyam Bhattarai")]
        public void Verify_functionality_of_disable_appeals_at_flag_level_client_setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                var settingLabel = "Disable Appeals at Flag level";
                var client = ClientEnum.SMTST.ToString();
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseqWithOneValidTopFlag = paramList["ClaseqWithOneValidTopFlag"];
                var claseqWithoutValidTopFlags = paramList["ClaseqWithoutValidTopFlags"];
                var restrictedFlags = paramList["RestrictedFlags"].Split(';').ToList();
                var unrestrictedFlag = paramList["UnrestrictedFlags"];
                var appealLineDetails = paramList["AppealLineDetails"].Replace("\\r\\n", "\r\n");

                try
                {
                    StringFormatter.PrintMessageTitle($"Verifying the client appeal setting {settingLabel}");
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(client, ClientSettingsTabEnum.Product.GetStringValue());

                    _clientSearch.IsRadioButtonOnOffByLabel(settingLabel).ShouldBeTrue($"{settingLabel} should be YES");

                    _clientSearch.ScrollNonAppealFlagsIntoView();
                    _clientSearch.GenericGetAvailableAssignedList("Selected Flags")
                        .ShouldCollectionBeEqual(restrictedFlags, "Restricted flags should be correct");

                    //todo : move this somewhere
                    /*StringFormatter.PrintMessage("Verifying whether the data is stored correctly in the DB");
                    _clientSearch.GetCommonSql
                        .GetSpecificClientDataFromDb("disable_appeal_flag_level", client)
                        .ShouldBeEqual("T", $"{settingLabel} status stored correctly in DB");
                    _clientSearch.GetCommonSql
                        .GetSpecificClientDataFromDb("appeal_non_selected_flags", client)
                        .ShouldBeEqual(string.Join(",",restrictedFlags), $"Selected flags for {settingLabel} stored correctly in DB");*/

                    var _appealCreator = _clientSearch.NavigateToAppealCreator();

                    StringFormatter.PrintMessage("Verifying creating an appeal from the Appeal Creator Page");
                    _appealCreator.SearchByClaimSequence(claseqWithoutValidTopFlags);

                    StringFormatter.PrintMessage("Verification of appeal lines by clicking on the lines individually");
                    for (var appealLineNo = 1; appealLineNo <= 2; appealLineNo++)
                    {
                        _appealCreator.ClickOnClaimLine(appealLineNo);
                        _appealCreator.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue(
                                "Error message pops up when trying to select an appeal line with top flag which is restricted for appeal creation");
                        _appealCreator.GetPageErrorMessage()
                            .ShouldBeEqual("Per contractual agreements, this flag does not permit appeal requests.");
                        _appealCreator.ClosePageError();
                        _appealCreator.GetSelectedClaimLinesByHeader("Selected Lines")
                            .ShouldCollectionBeAllNullOrEmpty("Lines should not move to the Selected Lines column");
                    }

                    StringFormatter.PrintMessage("Verification of 'Select All Lines' checkbox");
                    _appealCreator.IsSelectAllCheckBoxDisabled()
                        .ShouldBeTrue("'Select All Lines' options should be disabled when none of the appeal lines have a top flag for which appeal can be created");
                    
                    StringFormatter.PrintMessage("Verifying the create appeal icon in Claim Action page");
                    var _claimAction = _appealCreator.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithoutValidTopFlags, true);
                    _claimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue("Disable appeal creator button when there are no claim lines where there is a top flag that is permitted to create an appeal");
                    _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                        .ShouldBeEqual("There are no top flags on the claim which permit an appeal.");

                    var appealCreationMethods = new List<string>
                    {
                        "From Claim Action",
                        "From Appeal Creator"
                    };

                    var rand = new Random();
                    var randomlyChosenAppealCreationMethod = appealCreationMethods[rand.Next(appealCreationMethods.Count)];

                    if (randomlyChosenAppealCreationMethod == "From Appeal Creator")
                    {
                        StringFormatter.PrintMessageTitle("Verifying whether appeal creation is allowed by selecting claim lines for which the top flag is unrestricted");
                        var _claimSearch = _claimAction.ClickClaimSearchIcon();
                        _claimSearch.NavigateToAppealCreator();
                        _appealCreator.SearchByClaimSequence(claseqWithOneValidTopFlag);
                        VerifyAppealCreatorPage();
                    }

                    else
                    {
                        StringFormatter.PrintMessageTitle("Verifying whether appeal creation is allowed by selecting claim lines for which the top flag is unrestricted");
                        var _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                        _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithOneValidTopFlag, true);

                        _appealCreator = _claimAction.ClickOnCreateAppealIcon();
                        VerifyAppealCreatorPage();
                    }
                    

                    #region LOCAL METHOD
                    void VerifyAppealCreatorPage()
                    {
                        StringFormatter.PrintMessage("Verification of appeal lines by clicking on the lines individually");
                        for (var appealLineNo = 2; appealLineNo <= 3; appealLineNo++)
                        {
                            _appealCreator.ClickOnClaimLine(appealLineNo);
                            _appealCreator.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue(
                                    "Error message pops up when trying to select an appeal line with top flag which is restricted for appeal creation");
                            _appealCreator.GetPageErrorMessage()
                                .ShouldBeEqual("Per contractual agreements, this flag does not permit appeal requests.");
                            _appealCreator.ClosePageError();
                            _appealCreator.GetSelectedClaimLinesByHeader("Selected Lines")
                                .ShouldCollectionBeAllNullOrEmpty("Lines should not move to the Selected Lines column");
                        }

                        _appealCreator.IsSelectAllCheckBoxDisabled()
                            .ShouldBeFalse("'Select All Lines' options should be enabled when at least one of the appeal lines has a top flag for which appeal can be created");

                        _appealCreator.ClickOnClaimLine(1);

                        _appealCreator.GetSelectedClaimLinesByHeader("Select Appeal Lines").Count
                            .ShouldBeEqual(1, $"Only the line with allowed flag {unrestrictedFlag} should be selected");

                        
                        _appealCreator.GetSelectedClaimLinesByHeader("Selected Lines")[0]
                            .ShouldBeEqual(appealLineDetails, "Analyst can click on the appeal line list to select it for appeal creation");

                        _appealCreator.GetSideWindow.CheckOrUncheckByLabel("Select All Lines", false);
                        _appealCreator.GetSideWindow.CheckOrUncheckByLabel("Select All Lines");

                        var selectedLines = _appealCreator.GetSelectedClaimLinesByHeader("Selected Lines");
                        selectedLines.Count
                            .ShouldBeEqual(1, "Only the appeal line with the allowed flag should be selected when 'Select All Lines' checkbox is clicked");

                        selectedLines[0]
                            .ShouldBeEqual(appealLineDetails, "Correct appeal line with the allowed flag should be selected when 'Select All Lines' checkbox is clicked");

                        _appealCreator.SelectAppealRecordType();
                        _appealCreator.GetSideWindow.SetInputInInputFieldByLabel("External Document ID", "test");
                        _appealCreator.ClickOnSaveBtn();

                        StringFormatter.PrintMessage("Verifying the newly created appeal");
                        var _appealSearch = _appealCreator.NavigateToAppealSearch();
                        _appealSearch.SelectSMTST();
                        var _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claseqWithOneValidTopFlag);
                        var appealSeq = _appealAction.GetAppealSequence();

                        _appealAction.GetAppealLineCount().ShouldBeEqual(1);
                        _appealAction.GetFlagList().Count.ShouldBeEqual(1);
                        _appealAction.GetFlagList()[0].ShouldBeEqual(unrestrictedFlag);

                        var _appealManager = _appealAction.NavigateToAppealManager();
                        _appealManager.DeleteAppealByAppealSeq(appealSeq);
                        _appealManager.RefreshPage();
                    }
                    #endregion
                }

                finally
                {
                    //_clientSearch.GetCommonSql.UpdateSpecificClientDataInDB("disable_appeal_flag_level='F', appeal_non_selected_flags = null", ClientEnum.SMTST.ToString());
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2999(CAR-3017)
        public void Verify_Disable_Record_Reviews_Functionality_In_ProductAppeals_Tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var clientReviewedClaim = paramList["clientReviewedClaim"];
                var clientUnreviewedClaim = paramList["clientUnreviewedClaim"];

                try
                {
                    StringFormatter.PrintMessage("Deleting any associated appeals to the claims");
                    var _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(new List<string>() { clientReviewedClaim, clientUnreviewedClaim });

                    _clientSearch = _appealManager.NavigateToClientSearch();

                    StringFormatter.PrintMessage($"Setting the {ProductAppealsEnum.DisableRecordReviews.GetStringValue()} to true");
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());

                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisableRecordReviews.GetStringValue());

                    StringFormatter.PrintMessage("Turning the DCA product off for SMTST");
                    _clientSearch.ClickOnRadioButtonByLabel(ProductEnum.DCA.ToString(), false);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    StringFormatter.PrintMessage($"Verifying Appeal Creator section for a Client Reviewed claim after {ProductAppealsEnum.DisableRecordReviews.GetStringValue()} is set to true");
                    var _appealCreator = _clientSearch.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(clientReviewedClaim);
                    _appealCreator.IsRecordReviewRadioButtonDisabled()
                        .ShouldBeTrue("'Record Review' radio button should be disabled for Client Reviewed claim");
                    _appealCreator.IsRespectiveAppealTypeSelected(AppealType.Appeal.GetStringValue())
                        .ShouldBeTrue("Appeal type 'A' should be selected by default for Client Reviewed claim");

                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue());

                    StringFormatter.PrintMessage($"Verifying Appeal Creator section for a Client Unreviewed claim after {ProductAppealsEnum.DisableRecordReviews.GetStringValue()} is set to true");
                    _appealCreator.SearchByClaimSequence(clientUnreviewedClaim);
                    _appealCreator.IsRecordReviewRadioButtonDisabled()
                        .ShouldBeTrue("'Record Review' radio button should be disabled for Client Unreviewed claim");
                    _appealCreator.IsRespectiveAppealTypeSelected(AppealType.Appeal.GetStringValue())
                        .ShouldBeTrue("Appeal type 'A' should be selected by default for Client Reviewed claim and not Record Review");
                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue());

                    StringFormatter.PrintMessage("Verifying that the created appeals are of type 'Appeal'");
                    var _appealSearch = _appealCreator.NavigateToAppealSearch();
                    var _appealSummary = _appealSearch.FindByClaimSequenceToNavigateAppealummary(clientReviewedClaim);
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Appeal Type")
                        .ShouldBeEqual(AppealType.Appeal.GetStringDisplayValue(),
                            "The newly created appeal for a Reviewed Claim should be of type Appeal");

                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.FindByClaimSequenceToNavigateAppealummary(clientUnreviewedClaim);
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Appeal Type")
                        .ShouldBeEqual(AppealType.Appeal.GetStringDisplayValue(),
                            "The newly created appeal for an Unreviewed Claim should be of type Appeal");
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Running the Finally block");

                    StringFormatter.PrintMessage("Deleting any associated appeals to the claims");
                    var _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(new List<string> { clientReviewedClaim, clientUnreviewedClaim });

                    StringFormatter.PrintMessage("Reverting the product DCA to True");
                    _appealManager.GetCommonSql.UpdateSingleProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(), true);

                    StringFormatter.PrintMessage($"Reverting {ProductAppealsEnum.DisableRecordReviews.GetStringValue()} to False");
                    automatedBase.CurrentPage.GetCommonSql.UpdateSpecificAppealDataInDB(new List<string> { ProductAppealsEnum.DisableRecordReviews.GetStringValue() },
                        new List<string> { "F" }, ClientEnum.SMTST.ToString());
                }
            }
               
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] // CAR-3009(CAR-2995)
        public void Verify_Interop_Tab_for_COB_Product()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                Random rnd = new Random();
                var processingTypes = new List<string>
                {
                    "B",
                    "C",
                    "PB",
                    "PR",
                    "R"
                };
                var processingType = processingTypes[rnd.Next(processingTypes.Count)];
                
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString());
                _clientSearch.ClickOnClientSettingTabByTabName(
                    ClientSettingsTabEnum.Product.GetStringValue());
                if (!_clientSearch.IsRadioButtonOnOffByLabel(ProductEnum.COB.ToString()))
                    TurnProductOnOff(ProductEnum.COB.ToString(), true);

                try
                {
                    StringFormatter.PrintMessageTitle($"Verifying for '{processingType}' processing type");
                    _clientSearch.GetCommonSql.UpdateProcessingTypeOfClient(processingType, ClientEnum.RPE.ToString());
                    if (processingType.Equals("B"))
                    {
                            _clientSearch.IsClientSettingsTabDisabled(ClientSettingsTabEnum.Interop.GetStringValue())
                                .ShouldBeTrue(
                                    $"Is <{ClientSettingsTabEnum.Interop.GetStringValue()}> Tab disabled for processing Type<{processingType}>");

                    }

                    else
                    {
                            _clientSearch.ClickOnClientSettingTabByTabName(
                                ClientSettingsTabEnum.Interop.GetStringValue());
                            _clientSearch.GetSideWindow.ClickOnEditIcon();
                            _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.RealTimeClaims.GetStringValue())
                                .ShouldBeTrue(
                                    $"Is {InteropTabEnum.RealTimeCobClaims.GetStringValue()} Display for processing type<{processingType}> ");
                            _clientSearch.GetTimeUnitByLabel(InteropTabEnum.RealTimeCobClaims.GetStringValue())
                                .ShouldBeEqual("hours",
                                    $"Unit of turnaround time for '{InteropTabEnum.RealTimeCobClaims.GetStringValue()}' should be in hours");
                            _clientSearch
                                .GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeCobClaims
                                    .GetStringValue())
                                .ShouldBeEqual("24", "Default turnaround time should be 24 hours");
                            VerifyInputField(InteropTabEnum.RealTimeCobClaims.GetStringValue(), "1000",
                                "Value cannot be greater than 999.", _clientSearch);
                            VerifyInputField(InteropTabEnum.RealTimeCobClaims.GetStringValue(), "a",
                                "Only numbers allowed.", _clientSearch);
                            VerifyInputField(InteropTabEnum.RealTimeCobClaims.GetStringValue(), "!",
                                "Only numbers allowed.", _clientSearch);
                            _clientSearch.GetSideWindow.Cancel();

                            StringFormatter.PrintMessage("Verification when COB product is inactive");
                            TurnProductOnOff(ProductEnum.COB.ToString(), false);
                            _clientSearch.ClickOnClientSettingTabByTabName(
                                ClientSettingsTabEnum.Interop.GetStringValue());
                            _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.RealTimeCobClaims.GetStringValue())
                                .ShouldBeFalse(
                                    $"Is {InteropTabEnum.RealTimeCobClaims.GetStringValue()} Display for processing type<{processingType}> when COB is unassigned");
                            TurnProductOnOff(ProductEnum.COB.ToString(), true);
                    }
                }
                finally
                {
                    _clientSearch.GetCommonSql.UpdateProcessingTypeOfClient(processingTypes[4], ClientEnum.RPE.ToString());
                    TurnProductOnOff(ProductEnum.COB.ToString(), true);

                }
                void TurnProductOnOff(string product, bool check)
                {
                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.Product.GetStringValue());
                    if (_clientSearch.IsOkButtonPresent())
                        _clientSearch.ClickOkCancelOnConfirmationModal(true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonToTurnOnOrOff(product, check);
                }
            }

            

        }


        [NonParallelizable]
        [Test,Category("OnDemand")]//CAR-3290(CAR-3245)
        [Author("Shreya Pradhan")]
        public void Verify_Fraud_Medical_Record_Request_Expiration()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                var defaultValue = "30";
                var updateValue = "10";

                try
                {
                    StringFormatter.PrintMessage("Verify FWAV Record Request Expiration input field is not present when FFP product is inactive");
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString());

                    StringFormatter.PrintMessage("Turning off FFP product");
                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FFP.ToString(), false);
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
                    _clientSearch.IsInputFieldByLabelPresent("FWAV Record Request Expiration")
                        .ShouldBeFalse("Fraud medical record review field present?");


                    StringFormatter.PrintMessage(
                        "Verify FWAV Record Request Expiration input field is  present when FFP product is active");
                    StringFormatter.PrintMessage("Turning on FFP product");
                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FFP.ToString(), true);
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
                    _clientSearch.IsInputFieldByLabelPresent("FWAV Record Request Expiration")
                        .ShouldBeTrue("Fraud medical record review field present?");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.GetFWAVRecordReviewExpirationInputValue().ShouldBeEqual(defaultValue, "Default value correct?");
                    _clientSearch.SetFWAVRecordReviewExpirationInputValue(updateValue);
                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.GetCommonSql.GetClientSettingsByColNames("frr_expiration_days",
                            ClientEnum.RPE.ToString())[0]
                        .ShouldBeEqual(updateValue, "Updated value must be reflected in database");

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    ValidateInputField("101", "Value cannot be greater than 100.");
                    ValidateInputField("a", "Only numbers allowed.");
                    ValidateInputField("!", "Only numbers allowed.");
                    _clientSearch.GetSideWindow.Cancel(false);
                }

                finally
                {
                    StringFormatter.PrintMessage("Setting values to default");
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.SetFWAVRecordReviewExpirationInputValue(defaultValue);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                }


                void ValidateInputField(string value, string validationMessage)
                {
                    _clientSearch.SetFWAVRecordReviewExpirationInputValue(value);
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual(validationMessage,
                        "Verify error message");
                    _clientSearch.ClosePageError();
                }


            }
        }

        [NonParallelizable]
        [Test, Category("Working")] //CAR-2354 (CAR-1807) + CAR-3009(CAR-2995)
        public void Verify_Interop_setting_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                Random rnd = new Random();
                var expectedProcessingType = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ProcessingType").Values.ToList();
                var processingType = expectedProcessingType[rnd.Next(expectedProcessingType.Count)];

                bool processingTypeSetting = true;
                bool productTypeSetting = true;

                try
                {
                    StringFormatter.PrintMessage("Switching client to RPE");
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString());

                    StringFormatter.PrintMessage("Turning off FCI product");
                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), false);
                    productTypeSetting = false;

                    StringFormatter.PrintMessage("Verification of FCI Claims should not display when FCI product is OFF");
                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.General.GetStringValue());
                    if (_clientSearch.IsOkButtonPresent())
                        _clientSearch.ClickOkCancelOnConfirmationModal(true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", processingType);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    
                    processingTypeSetting = false;
                    if (processingType == "Batch")
                    {
                        _clientSearch.IsClientSettingsTabDisabled(ClientSettingsTabEnum.Interop.GetStringValue())
                            .ShouldBeTrue(
                                $"Is <{ClientSettingsTabEnum.Interop.GetStringValue()}> Tab disabled for processing Type<{processingType}>");

                    }

                    else
                    {
                        _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
                        _clientSearch.GetSideWindow.ClickOnEditIcon();

                        _clientSearch
                            .IsRadioButtonDisabledInInteropTab(InteropTabEnum.IncludeClaimReceivedDate.GetStringValue())
                            .ShouldBeTrue("YES/NO button should disabled");

                        _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.FCIClaims.GetStringValue())
                            .ShouldBeFalse(
                                $"Is {InteropTabEnum.FCIClaims.GetStringValue()} Display for processing type<{processingType}> ");


                        if (processingType == "PCA Real-time" || processingType == "Real-time")
                            _clientSearch.IsInputFieldByLabelDisabled(InteropTabEnum.RealTimeClaims.GetStringValue())
                                .ShouldBeFalse(
                                    $"{InteropTabEnum.RealTimeClaims.GetStringValue()} should enabled for processing type<{processingType}>");
                        else
                            _clientSearch.IsInputFieldByLabelDisabled(InteropTabEnum.RealTimeClaims.GetStringValue())
                                .ShouldBeTrue(
                                    $"{InteropTabEnum.RealTimeClaims.GetStringValue()} should disabled for processing type<{processingType}>");
                    }

                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.Product.GetStringValue());

                    StringFormatter.PrintMessage("Turning FCI product on");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), true);

                    productTypeSetting = true;
                    expectedProcessingType.Remove("Batch");
                    processingType = expectedProcessingType[rnd.Next(expectedProcessingType.Count)];
                    
                    StringFormatter.PrintMessageTitle(
                        "Verification of FCI Claims should not UpdateProcessingTypeOfClient when FCI product is ON");
                    //var iteration = 1;
                    //var iteration1 = 1;
                    //var iteration2 = 1;
                    var random = new Random();
                    int index = random.Next(timeOptionsList.Count);
                    string newTime = timeOptionsList[index];
                    var newTextValue = random.Next(0, 999).ToString();

                    var oldIncludeClaimReceivedDate = false;

                    var oldValueFCIClaimsTime = "";
                    var oldValueStartMF = "";
                    var oldValueEndMF = "";
                    var oldValueStartSat = "";
                    var oldValueEndSat = "";
                    var oldValueStartSun = "";
                    var oldValueEndSun = "";


                    var oldValueRetrySoft = "";
                    var oldValueFailThrough = "";
                    var oldValueFCIClaims = "";
                    var oldValueCVPBatchDue = "";
                    var oldValueRealTimeClaims = "";

                    var columnName = new List<string>
                    {
                        "RETRY_SOFT_MATCH_AFTER", "FAIL_DLQ_AFTER", "PROCESSING_TURNAROUND_MINUTES",
                        "FCI_TURNAROUND_DAYS",
                        "FCI_TURNAROUND_TIME", "FCI_INCLUDE_RECEIVED_DATE",
                        "CBHRS_MF_START_TIME", "CBHRS_MF_END_TIME",
                        "CBHRS_SAT_END_TIME",
                        "CBHRS_SAT_START_TIME", "CBHRS_SUN_END_TIME", "CBHRS_SUN_START_TIME"
                    };

                    var columnNameValues = "'" + String.Join("','", columnName) + "'";

                    _clientSearch.ClickOnClientSettingTabByTabName(
                        ClientSettingsTabEnum.General.GetStringValue());
                    if (_clientSearch.IsOkButtonPresent())
                        _clientSearch.ClickOkCancelOnConfirmationModal(true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", processingType);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch
                            .IsRadioButtonDisabledInInteropTab(InteropTabEnum.IncludeClaimReceivedDate
                                .GetStringValue())
                            .ShouldBeFalse("YES/NO button should disabled");

                    VerifyInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), "1000",
                        "Value cannot be greater than 999.", _clientSearch);
                    VerifyInputField(InteropTabEnum.FailDLQ.GetStringValue(), "1000",
                        "Value cannot be greater than 999.", _clientSearch);

                    _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.FCIClaims.GetStringValue())
                        .ShouldBeTrue(
                            $"Is {InteropTabEnum.FCIClaims.GetStringValue()} Display for processing type<{processingType}> ");

                    VerifyInputField(InteropTabEnum.FCIClaims.GetStringValue(), "1000",
                        "Value cannot be greater than 999.", _clientSearch);

                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.FCIClaims.GetStringValue())
                        .ShouldCollectionBeEqual(timeOptionsList,
                            $"Verification of Time List on {InteropTabEnum.FCIClaims.GetStringValue()}");

                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.MF.GetStringValue())
                        .ShouldCollectionBeEqual(timeOptionsList,
                            $"Verification of Time List on {InteropTabEnum.MF.GetStringValue()} start time");
                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.MF.GetStringValue(), 2)
                        .ShouldCollectionBeEqual(timeOptionsList,
                            $"Verification of Time List on {InteropTabEnum.MF.GetStringValue()} end time");

                    var timeOptionWithNull = new List<string>(timeOptionsList);
                    timeOptionWithNull.Insert(0, "");
                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sat.GetStringValue())
                        .ShouldCollectionBeEqual(timeOptionWithNull,
                            $"Verification of Time List on {InteropTabEnum.Sat.GetStringValue()} start time");
                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sat.GetStringValue(), 2)
                        .ShouldCollectionBeEqual(timeOptionWithNull,
                            $"Verification of Time List on {InteropTabEnum.Sat.GetStringValue()} end time");
                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sun.GetStringValue())
                        .ShouldCollectionBeEqual(timeOptionWithNull,
                            $"Verification of Time List on {InteropTabEnum.Sun.GetStringValue()} start time");
                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sun.GetStringValue(), 2)
                        .ShouldCollectionBeEqual(timeOptionWithNull,
                            $"Verification of Time List on {InteropTabEnum.Sun.GetStringValue()} end time");

                    oldIncludeClaimReceivedDate =
                        _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true);

                    oldValueFCIClaimsTime =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.FCIClaims.GetStringValue());
                    oldValueStartMF = _clientSearch.GetTimePickerInputValue(InteropTabEnum.MF.GetStringValue());
                    oldValueEndMF =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.MF.GetStringValue(), 2);
                    oldValueStartSat =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sat.GetStringValue());
                    oldValueEndSat =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sat.GetStringValue(), 2);
                    oldValueStartSun =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sun.GetStringValue());
                    oldValueEndSun =
                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sun.GetStringValue(), 2);


                    oldValueRetrySoft =
                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(
                            InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
                    oldValueFailThrough =
                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.FailDLQ
                            .GetStringValue());
                    oldValueFCIClaims =
                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.FCIClaims
                            .GetStringValue());

                    if (oldValueFCIClaims == newTextValue || oldValueFCIClaimsTime == newTime)
                    {
                        while (oldValueFCIClaims == newTextValue)
                        {
                            newTextValue = random.Next(0, 999).ToString();
                        }

                        while (oldValueFCIClaimsTime == newTime)
                        {
                            index = random.Next(timeOptionsList.Count);
                            newTime = timeOptionsList[index];
                        }
                    }

                    StringFormatter.PrintMessageTitle("Verification of Cancel button");

                    SetInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
                    SetInputField(InteropTabEnum.FailDLQ.GetStringValue());
                    SetInputField(InteropTabEnum.FCIClaims.GetStringValue());


                    SelectTimeField(InteropTabEnum.FCIClaims.GetStringValue());
                    SelectTimeField(InteropTabEnum.MF.GetStringValue());
                    SelectTimeField(InteropTabEnum.MF.GetStringValue(), 2);
                    SelectTimeField(InteropTabEnum.Sat.GetStringValue());
                    SelectTimeField(InteropTabEnum.Sat.GetStringValue(), 2);
                    SelectTimeField(InteropTabEnum.Sun.GetStringValue());
                    SelectTimeField(InteropTabEnum.Sun.GetStringValue(), 2);

                    _clientSearch.ClickOnRadioButtonInInteropTab(
                        InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), !oldIncludeClaimReceivedDate);

                    _clientSearch.GetSideWindow.Cancel();
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    VerifyInputTextBox(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), oldValueRetrySoft);
                    VerifyInputTextBox(InteropTabEnum.FailDLQ.GetStringValue(), oldValueFailThrough);
                    VerifyInputTextBox(InteropTabEnum.FCIClaims.GetStringValue(), oldValueFCIClaims);


                    VerifySelectTimeField(InteropTabEnum.FCIClaims.GetStringValue(), oldValueFCIClaimsTime);
                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), oldValueStartMF);
                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), oldValueEndMF, 2);
                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), oldValueStartSat);
                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), oldValueEndSat, 2);
                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), oldValueStartSun);
                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), oldValueEndSun, 2);

                    _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true)
                        .ShouldBeEqual(oldIncludeClaimReceivedDate,
                            $"Is {InteropTabEnum.IncludeClaimReceivedDate.GetStringValue()} button checked value equals");


                    StringFormatter.PrintMessageTitle("Verification of Save button");

                    SetInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
                    SetInputField(InteropTabEnum.FailDLQ.GetStringValue());
                    SetInputField(InteropTabEnum.FCIClaims.GetStringValue());


                    SelectTimeField(InteropTabEnum.FCIClaims.GetStringValue());
                    SelectTimeField(InteropTabEnum.MF.GetStringValue());
                    SelectTimeField(InteropTabEnum.MF.GetStringValue(), 2);
                    SelectTimeField(InteropTabEnum.Sat.GetStringValue());
                    SelectTimeField(InteropTabEnum.Sat.GetStringValue(), 2);
                    SelectTimeField(InteropTabEnum.Sun.GetStringValue());
                    SelectTimeField(InteropTabEnum.Sun.GetStringValue(), 2);

                    _clientSearch.ClickOnRadioButtonInInteropTab(
                        InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), !oldIncludeClaimReceivedDate);

                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    VerifyInputTextBox(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), newTextValue);
                    VerifyInputTextBox(InteropTabEnum.FailDLQ.GetStringValue(), newTextValue);
                    VerifyInputTextBox(InteropTabEnum.FCIClaims.GetStringValue(), newTextValue);


                    VerifySelectTimeField(InteropTabEnum.FCIClaims.GetStringValue(), newTime);
                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), newTime);
                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), newTime, 2);
                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), newTime);
                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), newTime, 2);
                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), newTime);
                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), newTime, 2);

                    _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true)
                        .ShouldBeEqual(!oldIncludeClaimReceivedDate,
                            $"Is {InteropTabEnum.IncludeClaimReceivedDate.GetStringValue()} button checked value equals");

                    _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(
                        $"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");

                    void SetInputField(string label)
                    {
                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(label, newTextValue);
                    }

                    void SelectTimeField(string label, int inputIndex = 1)
                    {
                        _clientSearch.SelectTimePickerInputFieldList(label, newTime, inputIndex);
                    }

                    void VerifyInputTextBox(string label, string value)
                    {
                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(label)
                            .ShouldBeEqual(value, $"Is {label} value equals?");
                    }

                    void VerifySelectTimeField(string label, string value, int inputIndex = 1)
                    {
                        _clientSearch.GetTimePickerInputValue(label, inputIndex)
                            .ShouldBeEqual(value, $"Is {label}  value equals?");
                    }


                    if (processingType == "PCA Real-Time" || processingType == "Real-Time")
                    {
                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
                        InteropTabEnum.RealTimeClaims.GetStringValue(), "1000");
                        _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be greater than 999.",
                            $"Verify validation Message when entering value greater than 999 in {InteropTabEnum.RealTimeClaims.GetStringValue()} Input Field");
                        _clientSearch.ClosePageError();

                        oldValueRealTimeClaims =
                                _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
                                    .GetStringValue());
                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
                                InteropTabEnum.RealTimeClaims.GetStringValue(), newTextValue);
                        _clientSearch.GetSideWindow.Cancel();
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch
                            .GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
                                .GetStringValue())
                            .ShouldBeEqual(oldValueRealTimeClaims,
                                $"{InteropTabEnum.RealTimeClaims.GetStringValue()} value should retain when cancel button clicked");

                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
                            InteropTabEnum.RealTimeClaims.GetStringValue(), newTextValue);
                        _clientSearch.GetSideWindow.Save();
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch
                            .GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
                                .GetStringValue())
                            .ShouldBeEqual(newTextValue,
                                $"{InteropTabEnum.RealTimeClaims.GetStringValue()} value should changed when cancel button clicked");

                    }

                    var dbValue =
                    _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.RPE.ToString(), columnNameValues,
                        12);
                    var oldValueList = new List<string>
                        {
                        oldValueRetrySoft, oldValueFailThrough, oldValueRealTimeClaims, oldValueFCIClaims,
                        oldValueFCIClaimsTime, oldIncludeClaimReceivedDate ? "True" : "False",
                        oldValueStartMF, oldValueEndMF, oldValueStartSat, oldValueEndSat,
                        oldValueStartSun, oldValueEndSun

                        };

                    var NewValueList = new List<string>
                        {
                        newTextValue, newTextValue, newTextValue, newTextValue,
                        newTime, !oldIncludeClaimReceivedDate ? "True" : "False",
                        newTime, newTime, newTime, newTime,
                        newTime, newTime

                        };

                    var finalList = new List<List<string>>();
                    for (int i = 0; i < columnName.Count; i++)
                    {
                        finalList.Add(new List<string> { columnName[i], oldValueList[i], NewValueList[i] });

                    }

                    finalList.ShouldCollectionBeEquivalent(dbValue, "Comparision of db Value");

                }

                finally
                {
                    if (!processingTypeSetting)
                    {
                        _clientSearch.ClickOnClientSettingTabByTabName(
                            ClientSettingsTabEnum.General.GetStringValue());
                        if (_clientSearch.IsOkButtonPresent())
                            _clientSearch.ClickOkCancelOnConfirmationModal(true);
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", "Real-Time");
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }

                    if (!productTypeSetting)
                    {
                        _clientSearch.ClickOnClientSettingTabByTabName(
                            ClientSettingsTabEnum.Product.GetStringValue());

                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), true);
                    }
                }
            }
        }

        //public void Verify_Interop_setting_tab()
        //{
        //    using (var automatedBase = new NewAutomatedBaseParallelRun())
        //    {
        //        ClientSearchPage _clientSearch;
        //        automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
        //        TestExtensions.TestName = new StackFrame().GetMethod().Name;
        //        var expectedProcessingType =
        //            automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ProcessingType").Values.ToList();
        //        bool processingTypeSetting = true;
        //        bool productTypeSetting = true;

        //        try
        //        {
        //            _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString());

        //            _clientSearch.ClickOnClientSettingTabByTabName(
        //                ClientSettingsTabEnum.Product.GetStringValue());

        //            _clientSearch.GetSideWindow.ClickOnEditIcon();
        //            _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), false);
        //            productTypeSetting = false;

        //            StringFormatter.PrintMessageTitle(
        //                "Verification of FCI Claims should not display when FCI product is OFF");
        //            foreach (var processingType in expectedProcessingType)
        //            {
        //                _clientSearch.ClickOnClientSettingTabByTabName(
        //                    ClientSettingsTabEnum.General.GetStringValue());
        //                if (_clientSearch.IsOkButtonPresent())
        //                    _clientSearch.ClickOkCancelOnConfirmationModal(true);
        //                _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", processingType);
        //                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
        //                processingTypeSetting = false;
        //                if (processingType == "Batch")
        //                {
        //                    _clientSearch.IsClientSettingsTabDisabled(ClientSettingsTabEnum.Interop.GetStringValue())
        //                        .ShouldBeTrue(
        //                            $"Is <{ClientSettingsTabEnum.Interop.GetStringValue()}> Tab disabled for processing Type<{processingType}>");
        //                    continue;
        //                }

        //                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
        //                _clientSearch.GetSideWindow.ClickOnEditIcon();

        //                _clientSearch
        //                    .IsRadioButtonDisabledInInteropTab(InteropTabEnum.IncludeClaimReceivedDate.GetStringValue())
        //                    .ShouldBeTrue("YES/NO button should disabled");

        //                _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.FCIClaims.GetStringValue())
        //                    .ShouldBeFalse(
        //                        $"Is {InteropTabEnum.FCIClaims.GetStringValue()} Display for processing type<{processingType}> ");


        //                if (processingType == "PCA Real-Time" || processingType == "Real-Time")
        //                    _clientSearch.IsInputFieldByLabelDisabled(InteropTabEnum.RealTimeClaims.GetStringValue())
        //                        .ShouldBeFalse(
        //                            $"{InteropTabEnum.RealTimeClaims.GetStringValue()} should enabled for processing type<{processingType}>");
        //                else
        //                    _clientSearch.IsInputFieldByLabelDisabled(InteropTabEnum.RealTimeClaims.GetStringValue())
        //                        .ShouldBeTrue(
        //                            $"{InteropTabEnum.RealTimeClaims.GetStringValue()} should disabled for processing type<{processingType}>");
        //            }

        //            _clientSearch.ClickOnClientSettingTabByTabName(
        //                ClientSettingsTabEnum.Product.GetStringValue());

        //            _clientSearch.GetSideWindow.ClickOnEditIcon();
        //            _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), true);

        //            productTypeSetting = true;
        //            expectedProcessingType.Remove("Batch");

        //            StringFormatter.PrintMessageTitle(
        //                "Verification of FCI Claims should not display when FCI product is ON");
        //            var iteration = 1;
        //            var iteration1 = 1;
        //            var iteration2 = 1;
        //            var random = new Random();
        //            int index = random.Next(timeOptionsList.Count);
        //            string newTime = timeOptionsList[index];
        //            var newTextValue = random.Next(0, 999).ToString();

        //            var oldIncludeClaimReceivedDate = false;

        //            var oldValueFCIClaimsTime = "";
        //            var oldValueStartMF = "";
        //            var oldValueEndMF = "";
        //            var oldValueStartSat = "";
        //            var oldValueEndSat = "";
        //            var oldValueStartSun = "";
        //            var oldValueEndSun = "";


        //            var oldValueRetrySoft = "";
        //            var oldValueFailThrough = "";
        //            var oldValueFCIClaims = "";
        //            var oldValueCVPBatchDue = "";
        //            var oldValueRealTimeClaims = "";

        //            var columnName = new List<string>
        //            {
        //                "RETRY_SOFT_MATCH_AFTER", "FAIL_DLQ_AFTER", "PROCESSING_TURNAROUND_MINUTES",
        //                "FCI_TURNAROUND_DAYS",
        //                "FCI_TURNAROUND_TIME", "FCI_INCLUDE_RECEIVED_DATE",
        //                "CBHRS_MF_START_TIME", "CBHRS_MF_END_TIME",
        //                "CBHRS_SAT_END_TIME",
        //                "CBHRS_SAT_START_TIME", "CBHRS_SUN_END_TIME", "CBHRS_SUN_START_TIME"
        //            };

        //            var columnNameValues = "'" + String.Join("','", columnName) + "'";

        //            foreach (var processingType in expectedProcessingType)
        //            {
        //                _clientSearch.ClickOnClientSettingTabByTabName(
        //                    ClientSettingsTabEnum.General.GetStringValue());
        //                if (_clientSearch.IsOkButtonPresent())
        //                    _clientSearch.ClickOkCancelOnConfirmationModal(true);
        //                _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", processingType);
        //                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

        //                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Interop.GetStringValue());
        //                _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                if (iteration == 1)
        //                {
        //                    void VerifyInputField(string label, string value, string validation_message)
        //                    {
        //                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(label, value);
        //                        _clientSearch.GetPageErrorMessage().ShouldBeEqual(validation_message,
        //                            $"Verify validation Message when entering value greater than 999 in {label} Input Field");
        //                        _clientSearch.ClosePageError();
        //                    }

        //                    _clientSearch
        //                        .IsRadioButtonDisabledInInteropTab(InteropTabEnum.IncludeClaimReceivedDate
        //                            .GetStringValue())
        //                        .ShouldBeFalse("YES/NO button should disabled");

        //                    VerifyInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), "1000",
        //                        "Value cannot be greater than 999.");
        //                    VerifyInputField(InteropTabEnum.FailDLQ.GetStringValue(), "1000",
        //                        "Value cannot be greater than 999.");

        //                    _clientSearch.IsInputFieldByLabelPresent(InteropTabEnum.FCIClaims.GetStringValue())
        //                        .ShouldBeTrue(
        //                            $"Is {InteropTabEnum.FCIClaims.GetStringValue()} Display for processing type<{processingType}> ");

        //                    VerifyInputField(InteropTabEnum.FCIClaims.GetStringValue(), "1000",
        //                        "Value cannot be greater than 999.");

        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.FCIClaims.GetStringValue())
        //                        .ShouldCollectionBeEqual(timeOptionsList,
        //                            $"Verification of Time List on {InteropTabEnum.FCIClaims.GetStringValue()}");

        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.MF.GetStringValue())
        //                        .ShouldCollectionBeEqual(timeOptionsList,
        //                            $"Verification of Time List on {InteropTabEnum.MF.GetStringValue()} start time");
        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.MF.GetStringValue(), 2)
        //                        .ShouldCollectionBeEqual(timeOptionsList,
        //                            $"Verification of Time List on {InteropTabEnum.MF.GetStringValue()} end time");

        //                    var timeOptionWithNull = new List<string>(timeOptionsList);
        //                    timeOptionWithNull.Insert(0, "");
        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sat.GetStringValue())
        //                        .ShouldCollectionBeEqual(timeOptionWithNull,
        //                            $"Verification of Time List on {InteropTabEnum.Sat.GetStringValue()} start time");
        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sat.GetStringValue(), 2)
        //                        .ShouldCollectionBeEqual(timeOptionWithNull,
        //                            $"Verification of Time List on {InteropTabEnum.Sat.GetStringValue()} end time");
        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sun.GetStringValue())
        //                        .ShouldCollectionBeEqual(timeOptionWithNull,
        //                            $"Verification of Time List on {InteropTabEnum.Sun.GetStringValue()} start time");
        //                    _clientSearch.GetTimePickerInputFieldList(InteropTabEnum.Sun.GetStringValue(), 2)
        //                        .ShouldCollectionBeEqual(timeOptionWithNull,
        //                            $"Verification of Time List on {InteropTabEnum.Sun.GetStringValue()} end time");

        //                    oldIncludeClaimReceivedDate =
        //                        _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
        //                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true);

        //                    oldValueFCIClaimsTime =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.FCIClaims.GetStringValue());
        //                    oldValueStartMF = _clientSearch.GetTimePickerInputValue(InteropTabEnum.MF.GetStringValue());
        //                    oldValueEndMF =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.MF.GetStringValue(), 2);
        //                    oldValueStartSat =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sat.GetStringValue());
        //                    oldValueEndSat =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sat.GetStringValue(), 2);
        //                    oldValueStartSun =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sun.GetStringValue());
        //                    oldValueEndSun =
        //                        _clientSearch.GetTimePickerInputValue(InteropTabEnum.Sun.GetStringValue(), 2);


        //                    oldValueRetrySoft =
        //                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(
        //                            InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
        //                    oldValueFailThrough =
        //                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.FailDLQ
        //                            .GetStringValue());
        //                    oldValueFCIClaims =
        //                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.FCIClaims
        //                            .GetStringValue());

        //                    if (oldValueFCIClaims == newTextValue || oldValueFCIClaimsTime == newTime)
        //                    {
        //                        while (oldValueFCIClaims == newTextValue)
        //                        {
        //                            newTextValue = random.Next(0, 999).ToString();
        //                        }

        //                        while (oldValueFCIClaimsTime == newTime)
        //                        {
        //                            index = random.Next(timeOptionsList.Count);
        //                            newTime = timeOptionsList[index];
        //                        }
        //                    }

        //                    StringFormatter.PrintMessageTitle("Verification of Cancel button");

        //                    SetInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
        //                    SetInputField(InteropTabEnum.FailDLQ.GetStringValue());
        //                    SetInputField(InteropTabEnum.FCIClaims.GetStringValue());


        //                    SelectTimeField(InteropTabEnum.FCIClaims.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.MF.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.MF.GetStringValue(), 2);
        //                    SelectTimeField(InteropTabEnum.Sat.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.Sat.GetStringValue(), 2);
        //                    SelectTimeField(InteropTabEnum.Sun.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.Sun.GetStringValue(), 2);

        //                    _clientSearch.ClickOnRadioButtonInInteropTab(
        //                        InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), !oldIncludeClaimReceivedDate);

        //                    _clientSearch.GetSideWindow.Cancel();
        //                    _clientSearch.GetSideWindow.ClickOnEditIcon();

        //                    VerifyInputTextBox(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), oldValueRetrySoft);
        //                    VerifyInputTextBox(InteropTabEnum.FailDLQ.GetStringValue(), oldValueFailThrough);
        //                    VerifyInputTextBox(InteropTabEnum.FCIClaims.GetStringValue(), oldValueFCIClaims);


        //                    VerifySelectTimeField(InteropTabEnum.FCIClaims.GetStringValue(), oldValueFCIClaimsTime);
        //                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), oldValueStartMF);
        //                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), oldValueEndMF, 2);
        //                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), oldValueStartSat);
        //                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), oldValueEndSat, 2);
        //                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), oldValueStartSun);
        //                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), oldValueEndSun, 2);

        //                    _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
        //                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true)
        //                        .ShouldBeEqual(oldIncludeClaimReceivedDate,
        //                            $"Is {InteropTabEnum.IncludeClaimReceivedDate.GetStringValue()} button checked value equals");


        //                    StringFormatter.PrintMessageTitle("Verification of Save button");

        //                    SetInputField(InteropTabEnum.RetrySoftMatchAfter.GetStringValue());
        //                    SetInputField(InteropTabEnum.FailDLQ.GetStringValue());
        //                    SetInputField(InteropTabEnum.FCIClaims.GetStringValue());


        //                    SelectTimeField(InteropTabEnum.FCIClaims.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.MF.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.MF.GetStringValue(), 2);
        //                    SelectTimeField(InteropTabEnum.Sat.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.Sat.GetStringValue(), 2);
        //                    SelectTimeField(InteropTabEnum.Sun.GetStringValue());
        //                    SelectTimeField(InteropTabEnum.Sun.GetStringValue(), 2);

        //                    _clientSearch.ClickOnRadioButtonInInteropTab(
        //                        InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), !oldIncludeClaimReceivedDate);

        //                    _clientSearch.GetSideWindow.Save();
        //                    _clientSearch.GetSideWindow.ClickOnEditIcon();

        //                    VerifyInputTextBox(InteropTabEnum.RetrySoftMatchAfter.GetStringValue(), newTextValue);
        //                    VerifyInputTextBox(InteropTabEnum.FailDLQ.GetStringValue(), newTextValue);
        //                    VerifyInputTextBox(InteropTabEnum.FCIClaims.GetStringValue(), newTextValue);


        //                    VerifySelectTimeField(InteropTabEnum.FCIClaims.GetStringValue(), newTime);
        //                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), newTime);
        //                    VerifySelectTimeField(InteropTabEnum.MF.GetStringValue(), newTime, 2);
        //                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), newTime);
        //                    VerifySelectTimeField(InteropTabEnum.Sat.GetStringValue(), newTime, 2);
        //                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), newTime);
        //                    VerifySelectTimeField(InteropTabEnum.Sun.GetStringValue(), newTime, 2);

        //                    _clientSearch.GetRadioButtonCheckedValueOnInInteropTab(
        //                            InteropTabEnum.IncludeClaimReceivedDate.GetStringValue(), true)
        //                        .ShouldBeEqual(!oldIncludeClaimReceivedDate,
        //                            $"Is {InteropTabEnum.IncludeClaimReceivedDate.GetStringValue()} button checked value equals");

        //                    _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(
        //                        $"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");

        //                    void SetInputField(string label)
        //                    {
        //                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(label, newTextValue);
        //                    }

        //                    void SelectTimeField(string label, int inputIndex = 1)
        //                    {
        //                        _clientSearch.SelectTimePickerInputFieldList(label, newTime, inputIndex);
        //                    }

        //                    void VerifyInputTextBox(string label, string value)
        //                    {
        //                        _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(label)
        //                            .ShouldBeEqual(value, $"Is {label} value equals?");
        //                    }

        //                    void VerifySelectTimeField(string label, string value, int inputIndex = 1)
        //                    {
        //                        _clientSearch.GetTimePickerInputValue(label, inputIndex)
        //                            .ShouldBeEqual(value, $"Is {label}  value equals?");
        //                    }

        //                    iteration++;
        //                }

        //                if (processingType == "PCA Real-Time" || processingType == "Real-Time")
        //                {
        //                    _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
        //                        InteropTabEnum.RealTimeClaims.GetStringValue(), "1000");
        //                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be greater than 999.",
        //                        $"Verify validation Message when entering value greater than 999 in {InteropTabEnum.RealTimeClaims.GetStringValue()} Input Field");
        //                    _clientSearch.ClosePageError();

        //                    if (iteration2 == 1)
        //                    {
        //                        oldValueRealTimeClaims =
        //                            _clientSearch.GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
        //                                .GetStringValue());
        //                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
        //                            InteropTabEnum.RealTimeClaims.GetStringValue(), newTextValue);
        //                        _clientSearch.GetSideWindow.Cancel();
        //                        _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                        _clientSearch
        //                            .GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
        //                                .GetStringValue())
        //                            .ShouldBeEqual(oldValueRealTimeClaims,
        //                                $"{InteropTabEnum.RealTimeClaims.GetStringValue()} value should retain when cancel button clicked");

        //                        _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(
        //                            InteropTabEnum.RealTimeClaims.GetStringValue(), newTextValue);
        //                        _clientSearch.GetSideWindow.Save();
        //                        _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                        _clientSearch
        //                            .GetInputTextBoxValueByLabelInWorkflowTab(InteropTabEnum.RealTimeClaims
        //                                .GetStringValue())
        //                            .ShouldBeEqual(newTextValue,
        //                                $"{InteropTabEnum.RealTimeClaims.GetStringValue()} value should changed when cancel button clicked");
        //                        iteration2++;
        //                    }
        //                }
        //            }

        //            var dbValue =
        //                _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.RPE.ToString(), columnNameValues,
        //                    12);
        //            var oldValueList = new List<string>
        //            {
        //                oldValueRetrySoft, oldValueFailThrough, oldValueRealTimeClaims, oldValueFCIClaims,
        //                oldValueFCIClaimsTime, oldIncludeClaimReceivedDate ? "True" : "False",
        //                oldValueStartMF, oldValueEndMF, oldValueStartSat, oldValueEndSat,
        //                oldValueStartSun, oldValueEndSun
        //            };

        //            var NewValueList = new List<string>
        //            {
        //                newTextValue, newTextValue, newTextValue, newTextValue,
        //                newTime, !oldIncludeClaimReceivedDate ? "True" : "False",
        //                newTime, newTime, newTime, newTime,
        //                newTime, newTime
        //            };

        //            var finalList = new List<List<string>>();
        //            for (int i = 0; i < columnName.Count; i++)
        //            {
        //                finalList.Add(new List<string> {columnName[i], oldValueList[i], NewValueList[i]});
        //            }

        //            finalList.ShouldCollectionBeEquivalent(dbValue, "Comparision of db Value");
        //        }

        //        finally
        //        {
        //            if (!processingTypeSetting)
        //            {
        //                _clientSearch.ClickOnClientSettingTabByTabName(
        //                    ClientSettingsTabEnum.General.GetStringValue());
        //                if (_clientSearch.IsOkButtonPresent())
        //                    _clientSearch.ClickOkCancelOnConfirmationModal(true);
        //                _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type", "Real-Time");
        //                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
        //            }

        //            if (!productTypeSetting)
        //            {
        //                _clientSearch.ClickOnClientSettingTabByTabName(
        //                    ClientSettingsTabEnum.Product.GetStringValue());

        //                _clientSearch.GetSideWindow.ClickOnEditIcon();
        //                _clientSearch.ClickOnRadioButtonToTurnOnOrOff(ProductEnum.FCI.ToString(), true);
        //            }
        //        }
        //    }
        //}

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-1757 (CAR-2352)
        public void Verify_save_and_update_in_workflow_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                var testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                try
                {
                    StringFormatter.PrintMessageTitle("Reverting Batch Complete Status for active products");
                    _clientSearch.UpdateRevertBatchCompleteStatus(ClientEnum.RPE.ToString());

                    _clientSearch.RefreshPage();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString());
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.WorkFlow.GetStringValue());

                    #region VERIFICATION OF SAVE

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether data is getting saved when 'Save' button is clicked");

                    _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    var valuesToBeSaved = testData["valuesToBeSaved"].Split(';').ToList();
                    var listOfProducts = _clientSearch.GetProductListInWorkflowTab();

                    _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());
                    valuesToBeSaved.Insert(0, "YES");

                    // Sets 'NO' if product is currently 'YES' and vice versa
                    foreach (var product in listOfProducts)
                        valuesToBeSaved.Add(_clientSearch.IsRadioButtonOnOffByLabel(product) ? "NO" : "YES");

                    EnterValuesInTheForm(valuesToBeSaved);
                    _clientSearch.GetSideWindow.Save();

                    #region Verification with DB

                    StringFormatter.PrintMessage("Verifying whether DB values are set correctly for the active products" +
                                                 "after setting the products to 'YES' in 'Complete Batch with Pended Claims'");
                    var productBatchCompleteStatusAfterSave =
                        _clientSearch.GetBatchCompleteStatusForProductsByClient(ClientEnum.RPE.ToString(),
                            listOfProducts);

                    productBatchCompleteStatusAfterSave.Count.ShouldBeEqual(listOfProducts.Count,
                        "The batch complete status should be updated for all active products once set to 'NO' in 'Complete Batch with Pended Claims'");
                    productBatchCompleteStatusAfterSave.Distinct().ToList().ShouldCollectionBeEqual(new List<string> { "P" },
                        "The batch complete status for all active products should be 'P' in DB once they are set to 'NO' in 'Complete Batch with Pended Claims'");

                    #endregion

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    VerifyDataInWorkflowTab(valuesToBeSaved, listOfProducts, _clientSearch);

                    #endregion

                    #region VERIFICATION OF UPDATE

                    StringFormatter.PrintMessageTitle("Verifying whether data is updated when changes are saved");

                    var valuesToBeUpdated = testData["valuesToBeUpdated"].Split(';').ToList();
                    _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());
                    valuesToBeUpdated.Insert(0, "YES");

                    // Sets 'NO' if product is currently 'YES' and vice versa 
                    foreach (var product in listOfProducts)
                        valuesToBeUpdated.Add(_clientSearch.IsRadioButtonOnOffByLabel(product) ? "NO" : "YES");

                    EnterValuesInTheForm(valuesToBeUpdated, true);
                    _clientSearch.GetSideWindow.Save();

                    #region Verification with DB

                    StringFormatter.PrintMessage("Verifying whether DB values are set correctly for the active products " +
                                                 "after setting the products to 'NO' in 'Complete Batch with Pended Claims'");
                    var productBatchCompleteStatusAfterUpdate =
                        _clientSearch.GetBatchCompleteStatusForProductsByClient(ClientEnum.RPE.ToString(),
                            listOfProducts);

                    productBatchCompleteStatusAfterUpdate.Count.ShouldBeEqual(listOfProducts.Count,
                        "Values should be updated for all active products once set to 'NO' in 'Complete Batch with Pended Claims'");
                    productBatchCompleteStatusAfterUpdate.Distinct().ToList().ShouldCollectionBeEqual(
                        new List<string> { "0" },
                        "Values for all active products should be set to NULL once set to 'NO' in 'Complete Batch with Pended Claims'");

                    #endregion

                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    VerifyDataInWorkflowTab(valuesToBeUpdated, listOfProducts,_clientSearch);

                    #endregion

                    #region LOCAL METHODS

                    void EnterValuesInTheForm(List<string> values, bool setProductsToOff = false)
                    {
                        _clientSearch.
                            SelectDropDownItemByLabelInWorkflowTab(WorkflowTabEnum.BeginClaimRelease.GetStringValue(), values[1]);
                        _clientSearch.
                            SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(), values[2]);
                        _clientSearch.
                            SelectDropDownItemByLabelInWorkflowTab(WorkflowTabEnum.ReturnFileFrequency.GetStringValue(), values[3]);
                        _clientSearch.
                            SelectDropDownItemByLabelInWorkflowTab(
                            WorkflowTabEnum.FailureAlert.GetStringValue(), values[4]);

                        foreach (var product in listOfProducts)
                        {
                            if (!setProductsToOff)
                                _clientSearch.ClickOnRadioButtonByLabel(product);
                            else
                                _clientSearch.ClickOnRadioButtonByLabel(product, false);
                        }
                    }

                    #endregion
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Reverting Batch Complete Status for active products");
                    _clientSearch.UpdateRevertBatchCompleteStatus(ClientEnum.RPE.ToString());
                }
            }

            
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-1804(CAR-2206) + TE-995
        public void Verify_update_Security_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                var auditDataList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "SecurityTabAuditData").Values.ToList();
                var auditDataListLogoutURI = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "SecurityTabAuditDataLogoutURI").Values.ToList();
                var invalidUrlList = new List<string>()
            {
                "",
                "abc.com",
                "www.abc.com",
                "https://wwww.abc"
            };

                var validUrlList = new List<string>()
            {
                "https://www.abc.com",
                "http://www.abc.com"
            };

                try
                {
                    _clientSearch.RefreshPage();

                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.RPE.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.ClickOnClientSettingTabByTabName("Security");

                    StringFormatter.PrintMessageTitle("Verification of Save functionality");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "56");
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    StringFormatter.PrintMessage("Verifying whether saved values are being reflected correctly in the form");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.GetInputTextBoxValueByLabel("Password Duration").ShouldBeEqual("56", "'Password Duration' is saved correctly");
                    _clientSearch.GetInputTextBoxValueByLabel("Client User PHI/PII Access").ShouldBeEqual("Requires User Authority", "'Client User PHI/PII Access' is saved correctly");

                    StringFormatter.PrintMessage("Verifying audit information from the database");
                    var auditDataFromDB = _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.RPE.ToString(), 1);
                    var lastModifiedDate = Convert.ToDateTime(auditDataFromDB[0][2]).ToString("MM/dd/yyyy hh:mm tt").Trim();

                    var sysdate = DateTime.ParseExact(_clientSearch.GetCommonSql.GetSystemDateFromDB(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                    //DateTime.ParseExact(lastModifiedDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AssertDateRange(sysdate.AddMinutes(-1),
                    //    sysdate.AddMinutes(1), "Last modified date should match");
                    auditDataFromDB[0].RemoveAt(2);
                    auditDataFromDB[0].ShouldCollectionBeEqual(auditDataList, "Audit data should match");


                    #region TE-995
                    StringFormatter.PrintMessage("Verify federated user logout uri in client settings page");
                    _clientSearch.IsExternalIDPLogoutURLSectionPresent().ShouldBeTrue("External IDP Logout URL present ?");
                    _clientSearch
                        .IsRadioButtonPresentByLabel("Is Federated")
                        .ShouldBeTrue("Is Federated radio button should display");
                    _clientSearch.IsRadioButtonOnOffByLabel("Is Federated")
                        .ShouldBeFalse("By Default Is Federated should be set to no");
                    _clientSearch.ClickOnRadioButtonByLabel("Is Federated");
                    _clientSearch.IsInputFieldByLabelPresent("Logout URL").ShouldBeTrue(
                        "Logout URL input field should be present when is federated is set to true");
                    _clientSearch.GetInfoHelpTooltipByLabel("Logout URL").
                        ShouldBeEqual("The user will be redirected to this URL once they log out of the Nucleus application. Enter the URL in the format http://www.cotiviti.com/home or https://www.cotiviti.com.");

                    StringFormatter.PrintMessage("Verify error message when URL is not set in correct format");
                    foreach (var url in invalidUrlList)
                    {
                        _clientSearch.SetInputTextBoxValueByLabel("Logout URL", url);
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error Message Should Be Present");
                        _clientSearch.GetPageErrorMessage().ShouldBeEqual("Enter the URL in the format http://www.cotiviti.com/home or https://www.cotiviti.com before the record can be saved.");
                        _clientSearch.ClosePageError();
                    }

                    StringFormatter.PrintMessage("Verify valid URL is saved");
                    foreach (var url in validUrlList)
                    {
                        _clientSearch.SetInputTextBoxValueByLabel("Logout URL", url);
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch.GetInputTextBoxValueByLabel("Logout URL").ShouldBeEqual(url, "URL should be saved correctly");
                        _clientSearch.GetRedirectURLForClient(ClientEnum.RPE.ToString()).ShouldBeEqual(url);
                    }

                    StringFormatter.PrintMessage("Verifying audit information from the database");
                    auditDataFromDB = _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.RPE.ToString(), 1);
                    lastModifiedDate = Convert.ToDateTime(auditDataFromDB[0][2]).ToString("MM/dd/yyyy hh:mm tt").Trim();

                    sysdate = DateTime.ParseExact(_clientSearch.GetCommonSql.GetSystemDateFromDB(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                    //DateTime.ParseExact(lastModifiedDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AssertDateRange(sysdate.AddMinutes(-1),
                    //    sysdate.AddMinutes(1), "Last modified date should match");
                    auditDataFromDB[0].RemoveAt(2);
                    auditDataFromDB[0].ShouldCollectionBeEqual(auditDataListLogoutURI, "Audit data should match");
                    #endregion
                }

                finally
                {
                    _clientSearch.ClickOnRadioButtonByLabel("Is Federated", false);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    StringFormatter.PrintMessageTitle("Setting the Password Duration to original state for 'RPE' in Security Tab");
                    _clientSearch.GetCommonSql.UpdateSetPasswordDurationInSecurityTab("60", ClientEnum.RPE.ToString());
                }
            }



        }

        [NonParallelizable]
        [Test, Category("OnDemand")]//CAR-2075(CAR-1931) + CAR-1801(CAR-2083)
        public void Verify_General_Client_Information_Settings_view_and_update()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var expectedClientType = paramLists["ClientType"].Split(',').ToList();
                var expectedProcessingType = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ProcessingType").Values.ToList();
                var expectedClientSettingTab =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Client_Settings_Tab").Values.ToList();
                var loggedInUserFullName = _clientSearch.GetLoggedInUserFullName();
                

                try
                {
                    #region Restore Configuration

                    _clientSearch.UpdateDefaultSchemaValue();

                    #endregion

                    #region SearchClient and Verify client settings tab list

                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                        ClientEnum.SMTST.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.GetClientSettingsTabList().ShouldCollectionBeEqual(expectedClientSettingTab,
                        "Verification of Client Settings Tab ");

                    #endregion

                    #region Verification of Client Information

                    StringFormatter.PrintMessageTitle("Verification of Client Information");
                    _clientSearch.GetFormHeaderLabel().ShouldBeEqual("Client Information",
                        "Is Client Information Tab Selected and Display?");
                    _clientSearch.GetClientSettingsSidePaneHeaderText().ShouldBeEqual("Client Settings", "Is header correct for Client Settings side view?");
                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    ValidateGeneralTabFormEnableDisabled(true, _clientSearch);
                    _clientSearch.GetSideWindow.GetPrimaryButtonName().ShouldBeEqual("Save", "Is Button Name Updated?");
                    _clientSearch.GetSideWindow.GetValueByLabel("Client Code")
                        .ShouldBeEqual(ClientEnum.SMTST.ToString(), "Is Client Code display correctly?");
                    _clientSearch.GetStatusRadioButtonLabel().ShouldBeEqual("Active", "Is Active Button Display?");
                    _clientSearch.GetStatusRadioButtonLabel(false)
                        .ShouldBeEqual("Inactive", "Is Active Button Display?");

                    _clientSearch.GetSideWindow.GetDropDownList("Client Type")
                        .ShouldCollectionBeEqual(expectedClientType, "Verification of Client Type dropdown list");
                    _clientSearch.GetSideWindow.GetDropDownList("Processing Type")
                        .ShouldCollectionBeEqual(expectedProcessingType, "Verification of Processing Type dropdown list");

                    #endregion

                    #region Validation of Input and Drop down list and save and cancel button

                    var statusBefore = _clientSearch.IsActiveInactiveStatusRadioButtonClicked();
                    var clientNameBefore = _clientSearch.GetSideWindow.GetInputFieldText("Client Name");
                    var clientTypeBefore = _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Client Type");
                    var processingTypeBefore =
                        _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Processing Type");

                    const bool statusAfter = false;
                    const string clientNameAfter = "Selenium Test Client1";
                    var clientTypeAfter = expectedClientType[1];
                    var processingTypeAfter = expectedProcessingType[4];

                    const string validateString =
                        "This is Test Client for verification of Client Name Selenium Test Client not more than hundred character";

                    _clientSearch.GetSideWindow.FillInputBox("Client Name", "");
                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Invalid or missing data must be resolved before the record can be saved.",
                        "Is Confirmation Message Equals?");
                    _clientSearch.ClosePageError();

                    FillGeneralTabForm(statusAfter, validateString, clientTypeAfter, processingTypeAfter, _clientSearch);
                    //ValidateGeneralTabForm(statusAfter, validateString.Substring(0, 100), clientTypeAfter,
                    //    processingTypeAfter);

                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Configuration.GetStringValue());
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Any unsaved changes will be lost.",
                        "Is Confirmation Message Equals?");
                    _clientSearch.ClickOkCancelOnConfirmationModal(false);

                    StringFormatter.PrintMessageTitle("Validate ClientName should allow only 100 character");
                    ValidateGeneralTabForm(statusAfter, validateString.Substring(0, 100), clientTypeAfter,
                        processingTypeAfter, _clientSearch);
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Configuration.GetStringValue());
                    _clientSearch.ClickOkCancelOnConfirmationModal(true);
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.General.GetStringValue());

                    StringFormatter.PrintMessageTitle(
                        "Verification of Form Data should disabled and discard the changes after switching from tab to tab");
                    ValidateGeneralTabForm(statusBefore, clientNameBefore, clientTypeBefore, processingTypeBefore, _clientSearch);
                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);


                    StringFormatter.PrintMessageTitle(
                        "Verification of Form Data should disabled and discard the changes after switching from tab to tab");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    FillGeneralTabForm(statusAfter, clientNameAfter, clientTypeAfter, processingTypeAfter, _clientSearch);
                    _clientSearch.GetSideWindow.Cancel();

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    ValidateGeneralTabForm(statusBefore, clientNameBefore, clientTypeBefore, processingTypeBefore, _clientSearch);

                    _clientSearch.GetSideWindow.Cancel();
                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);

                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    FillGeneralTabForm(statusAfter, clientNameAfter, clientTypeAfter, processingTypeAfter, _clientSearch);
                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    var auditData = _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.SMTST.ToString());

                    #endregion

                    #region Validation of Audit Record

                    foreach (var row in auditData)
                    {
                        switch (row[3])
                        {
                            case "CLIENTNAME":
                                row[4].ShouldBeEqual(clientNameBefore, "Verification of Old Value of CLIENTNAME");
                                row[5].ShouldBeEqual(clientNameAfter, "Verification of New Value of CLIENTNAME");
                                break;
                            case "PROCESSING_TYPE":
                                row[4].ShouldBeEqual("Batch", "Verification of Old Value of PROCESSING_TYPE");
                                row[5].ShouldBeEqual("RealTime", "Verification of New Value of PROCESSING_TYPE");
                                break;
                            case "CLIENT_TYPE":
                                row[4].ShouldBeEqual("GroupHealth", "Verification of Old Value of CLIENT_TYPE");
                                row[5].ShouldBeEqual("PropertyAndCasualty", "Verification of New Value of CLIENT_TYPE");
                                break;
                            case "ACTIVE":
                                row[4].ShouldBeEqual("True", "Verification of Old Value of ACTIVE");
                                row[5].ShouldBeEqual("False", "Verification of New Value of ACTIVE");
                                break;

                        }

                    }

                    #endregion

                    #region Verification of changes after  modifying data

                    StringFormatter.PrintMessageTitle(
                        "Verification of Form Data should disabled and save the changes");
                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);
                    ValidateGeneralTabForm(statusAfter, clientNameAfter, clientTypeAfter,
                        processingTypeAfter, _clientSearch);

                    _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(
                        $"{loggedInUserFullName} {DateTime.Now:MM/dd/yyyy}");

                    #endregion


                }
                finally
                {
                    #region Restore Configuration

                    _clientSearch.UpdateDefaultSchemaValue();

                    #endregion
                    //_clientSearch.ClickOnautomatedBase.QuickLaunch().NavigateToClientSearch();
                }
            }

            
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2204(CAR-1806)+ CAR-2307(CAR-1709)+ CAR-2898(CAR-2850) + CAR-2998(CAR-2925) + CAR-3111(CAR-3066)
        public void Verify_update_of_Product_Appeal_Settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                #region Expected Data from XML

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var revertProductStatus = paramLists["RevertProductStatus"].Split(';').ToList();
                var revertAppealData = paramLists["RevertAppealData"].Split(';').ToList();
                var productStatusLabel =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Product_Status").Values.ToList();

                #endregion

                #region Revert All Settings
                ////revert script
                _clientSearch.GetCommonSql.UpdateRevertProductStatus(new List<string>(revertProductStatus)
                {ClientEnum.RPE.ToString()});
                _clientSearch.GetCommonSql.UpdateRevertAppealData(new List<string>(revertAppealData)
                {ClientEnum.RPE.ToString()});
                _clientSearch.RefreshPage();
                #endregion

                #region Search and click on Product Tab

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                    ClientEnum.RPE.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());

                #endregion

                try
                {
                    #region Expected Data From Database

                    var expectedAppealSettings =
                        _clientSearch.GetCommonSql.GetAppealSettingsBoolValueFromDatabase(ClientEnum.RPE.ToString());
                    var allProductValueList = _clientSearch.GetCommonSql.GetAllProductValueList(ClientEnum.RPE.ToString());

                    Dictionary<string, bool> MappingOfAppealSettingsLabelWithDefaultValue = new Dictionary<string, bool>();

                    Enum.GetValues(typeof(ProductAppealsEnum)).Length.ShouldBeEqual(expectedAppealSettings.Count,
                        "Appeal Settings and its respective values count in DB should match. " +
                        "If the counts are not equal, then some Appeal settings label might be missing/extra.");

                    int count = 0;
                    foreach (ProductAppealsEnum appealSettingsLabel in Enum.GetValues(typeof(ProductAppealsEnum)))
                    {
                        MappingOfAppealSettingsLabelWithDefaultValue.Add(appealSettingsLabel.GetStringValue(),
                            expectedAppealSettings[count]);
                        count++;
                    }

                    #endregion

                    #region Verification of update of Appeal Due Date Calculation

                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    //CAR-2850 + CAR-3111(CAR-3066)
                    StringFormatter.PrintMessage("Validate appeal due date calculation settings");

                    VerifyAppealDueDateCalculationSettings(_clientSearch);

                    _clientSearch.SetAppealDueDatesAllInputTextBox("99");
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    var appealDueDateList = _clientSearch.GetAppealDueDatesAllInputTextBox(true).Distinct().ToList();
                    appealDueDateList.Count.ShouldBeEqual(1, "Is All Value Should Updated?");
                    appealDueDateList[0].ShouldBeEqual("99", "Is All Value Should Updated correctly?");

                    VerificationOfDataSavedInDB(ProductAppealsSectionHeadersEnum.AppealDueDateCalculation.GetStringValue(),
                        productStatusLabel, ClientEnum.RPE.ToString(), _clientSearch);

                    _clientSearch.ClearAppealDueDatesAllInputTextBox();
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    appealDueDateList = _clientSearch.GetAppealDueDatesAllInputTextBox(true).Distinct().ToList();
                    appealDueDateList.Count.ShouldBeEqual(1, "Is All Value Should Updated?");
                    appealDueDateList[0].ShouldBeEqual("0", "Is All Value Should Updated correctly?");

                    #endregion

                    #region Verification of update of Product setting and appeals active settings

                    for (int i = 0; i < productStatusLabel.Count; i++)
                    {
                        _clientSearch.ClickOnRadioButtonByLabel(productStatusLabel[i], !allProductValueList[i]);
                    }

                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealsActive.GetStringValue(), false);

                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    for (int i = 0; i < productStatusLabel.Count; i++)
                    {
                        _clientSearch.IsRadioButtonOnOffByLabel(productStatusLabel[i], !allProductValueList[i])
                            .ShouldBeTrue(
                                $"Is Radio button {(!allProductValueList[i] ? "ON" : "OFF")} for {productStatusLabel[i]}");
                    }

                    #endregion

                    #region Verification of update of Display EXT Doc and Send Appeal Email

                    _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.AppealsActive.GetStringValue(), false)
                        .ShouldBeTrue($"Is Radio button OFF for {ProductAppealsEnum.AppealsActive.GetStringValue()}");

                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealsActive.GetStringValue());
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(),
                        false);
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(), false);

                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(), false)
                        .ShouldBeTrue(
                            $"Is Radio button OFF for {ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()}");
                    _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(), false)
                        .ShouldBeTrue($"Is Radio button OFF for {ProductAppealsEnum.SendAppealEmail.GetStringValue()}");

                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue());
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue());

                    #endregion

                    #region Verification of update of remaining appeal settings

                    foreach (KeyValuePair<string, bool> appealLabelAndSetting in MappingOfAppealSettingsLabelWithDefaultValue)
                    {
                        if (appealLabelAndSetting.Key == ProductAppealsEnum.AppealsActive.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.DisableRecordReviews.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.DisplayExtDocIDField.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.SendAppealEmail.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.AppealEmailCC.GetStringValue())
                            continue;

                        _clientSearch.ClickOnRadioButtonByLabel(appealLabelAndSetting.Key, !appealLabelAndSetting.Value);
                    }

                    _clientSearch.SetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue(),
                        "Test@test.com;Test1@test.com");

                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    foreach (KeyValuePair<string, bool> appealLabelAndSetting in MappingOfAppealSettingsLabelWithDefaultValue)
                    {
                        if (appealLabelAndSetting.Key == ProductAppealsEnum.AppealsActive.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.DisableRecordReviews.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.DisplayExtDocIDField.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.SendAppealEmail.GetStringValue() ||
                            appealLabelAndSetting.Key == ProductAppealsEnum.AppealEmailCC.GetStringValue())
                            continue;

                        _clientSearch.IsRadioButtonOnOffByLabel(appealLabelAndSetting.Key, !appealLabelAndSetting.Value)
                            .ShouldBeTrue(
                                $"Is Radio button {(!appealLabelAndSetting.Value ? "ON" : "OFF")} for {appealLabelAndSetting.Key}");
                    }

                    _clientSearch.GetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue())
                        .ShouldBeEqual("Test@test.com;Test1@test.com",
                            "Appeal Email CC should be semi comma separated");

                    StringFormatter.PrintMessageTitle("Verifying whether all the data are stored correctly in the DB");
                    VerificationOfDataSavedInDB(ProductAppealsSectionHeadersEnum.ProductStatus.GetStringValue(),
                        productStatusLabel, ClientEnum.RPE.ToString(), _clientSearch);

                    VerificationOfDataSavedInDB(ProductAppealsSectionHeadersEnum.AppealSettings.GetStringValue(),
                        productStatusLabel, ClientEnum.RPE.ToString(), _clientSearch);

                    VerificationOfDataSavedInDB(ProductAppealsSectionHeadersEnum.AppealDueDateCalculation.GetStringValue(),
                        productStatusLabel, ClientEnum.RPE.ToString(), _clientSearch);

                    #endregion
                }

                finally
                {
                    #region Revert All Settings
                    ////revert script
                    _clientSearch.GetCommonSql.UpdateRevertProductStatus(new List<string>(revertProductStatus)
                    {ClientEnum.RPE.ToString()});
                    _clientSearch.GetCommonSql.UpdateRevertAppealData(new List<string>(revertAppealData)
                    {ClientEnum.RPE.ToString()});
                    _clientSearch.ClickOnQuickLaunch().NavigateToClientSearch();
                    #endregion
                }

            }

            
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2207 (CAR-1802) + CAR-2352 (CAR-1757) -- Configuration Tab
        public void Verify_save_and_update_in_configuration_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;

                var listOfClientConfigurationOptions = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Configuration_Setting_Options").Values.ToList();
                var originalConfigSettingsList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "configuration_setting_default_values_for_rpe").Values.ToList();
                var testData = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "UpdatedValues", "Value").Split(';').ToList();

                StringFormatter.PrintMessage("Setting the configuration settings to original state for 'RPE'");
                _clientSearch.GetCommonSql.UpdateRevertConfigSettings(originalConfigSettingsList, ClientEnum.RPE.ToString());
                _clientSearch.RefreshPage();
                try
                {
                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.RPE.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();

                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Configuration.GetStringValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    StringFormatter.PrintMessage("Updating all radio buttons to 'NO' if currently 'YES' and vice-versa");
                    foreach (var optionLabel in listOfClientConfigurationOptions)
                    {
                        if (_clientSearch.IsRadioButtonPresentByLabel(optionLabel))
                        {
                            if (_clientSearch.IsRadioButtonOnOffByLabel(optionLabel) && optionLabel != "Provider Alert Indicator option")
                                _clientSearch.ClickOnRadioButtonByLabel(optionLabel, false);
                            else
                                _clientSearch.ClickOnRadioButtonByLabel(optionLabel);
                        }
                    }

                    StringFormatter.PrintMessage("Updating the non-radio fields");
                    _clientSearch.SetInputTextBoxValueByLabel("Provider Alert Label", testData[0]);
                    _clientSearch.SetInputTextBoxValueByLabel("Paid Exposure Threshold", testData[1]);
                    _clientSearch.SetInputTextBoxValueByLabel("Provider Score Threshold", testData[2]);
                    _clientSearch.SetInputTextBoxValueByLabel("Claim Number Title", testData[3]);
                    _clientSearch.SelectDropDownListForClientSettingsByLabel("Allow Switch of non-Reverse Flags", "YES");

                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    StringFormatter.PrintMessageTitle("Verifying whether the updated data is retained in the form");
                    for (int i = 0; i < originalConfigSettingsList.Count; i++)
                    {
                        var optionLabel = listOfClientConfigurationOptions[i];
                        if (_clientSearch.IsRadioButtonPresentByLabel(optionLabel) && optionLabel != "Provider Alert Indicator option")
                        {
                            var radioOriginalStatus = !_clientSearch.IsRadioButtonOnOffByLabel(optionLabel) ? "T" : "F";
                            originalConfigSettingsList[i].ShouldBeEqual(radioOriginalStatus);
                        }
                    }

                    _clientSearch.GetInputTextBoxValueByLabel("Provider Alert Label")
                        .ShouldBeEqual(testData[0], "Data is saved correctly for 'Provider Alert Label'");
                    _clientSearch.GetInputTextBoxValueByLabel("Paid Exposure Threshold")
                        .ShouldBeEqual(testData[1], "Data is saved correctly for 'Paid Exposure Threshold'");
                    _clientSearch.GetInputTextBoxValueByLabel("Provider Score Threshold")
                        .ShouldBeEqual(testData[2], "Data is saved correctly for 'Provider Score Threshold'");
                    _clientSearch.GetInputTextBoxValueByLabel("Claim Number Title")
                        .ShouldBeEqual(testData[3], "Data is saved correctly for 'Claim Number Title'");
                    _clientSearch.GetInputTextBoxValueByLabel("Allow Switch of non-Reverse Flags")
                        .ShouldBeEqual("YES", "Data is saved correctly for 'Allow Switch of non-Reverse Flags'");

                    _clientSearch.ClickOnRadioButtonByLabel("Provider Alert Indicator option", false);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.IsRadioButtonOnOffByLabel("Provider Alert Indicator option").ShouldBeFalse("Is Provider Alert Indicator option");
                    _clientSearch.IsDivByLabelPresent("Provider Alert Label").ShouldBeFalse("'Provider Alert Label' should be hidden once 'Provider Alert Indicator option'" +
                                                                                               "is set to NO");

                    StringFormatter.PrintMessageTitle("Verification whether the audit information is being saved and displayed beneath the form");
                    _clientSearch.GetCommonSql.GetLastModifiedUsernameAndDate(ClientEnum.RPE.ToString())
                        .ShouldBeEqual(_clientSearch.GetLastModifiedByWithDateFromForm());

                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Setting the configuration settings to original state for 'RPE'");
                    _clientSearch.GetCommonSql.UpdateRevertConfigSettings(originalConfigSettingsList, ClientEnum.RPE.ToString());
                }
            }


            
        }

        [NonParallelizable]
        [Test, Category("OnDemand")]//CAR-2205(CAR-1805)
        public void Verify_edit_of_Custom_fields()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var expectedLabelInCustomTab = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Custom_Field")
                    .Values.ToList();
                int maxValue = 6;
                int minValue = 1;

                _clientSearch.DeleteCustomFields(ClientEnum.TTREE.GetStringDisplayValue());
                _clientSearch.RefreshPage();
                try
                {
                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                        ClientEnum.TTREE.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Custom.GetStringValue());
                    _clientSearch.WaitForWorking();
                    _clientSearch.GetNoFieldSelectedMessage().ShouldBeEqual("No Fields Selected");
                    _clientSearch.IsAvailableFieldsDropdownPresent()
                        .ShouldBeFalse("Available Fields dropdown should not be present");

                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.IsAllTextBoxPresent().ShouldBeTrue("No fields should be present");
                    _clientSearch.GetHeaderLabelInCustomField().ShouldBeEqual(expectedLabelInCustomTab);
                    _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Available Fields")
                        .ShouldBeFalse("Available Fields dropdown should be enabled");
                    _clientSearch.IsAllTextBoxDisabled().ShouldBeFalse("All Text box area should be enabled");
                    _clientSearch.IsPHISelectorCheckboxDisabled()
                        .ShouldBeFalse("PHI Selector Checkbox should be enabled.");
                    SelectCustomLabelFromAvailableFields(maxValue, _clientSearch);

                    _clientSearch.GetAvailableListCount().ShouldBeEqual(maxValue, "Is Selected Available Fields display?");
                    _clientSearch.ClickOnPHISelectorCheckbox(5);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _clientSearch.GetNumericalLabelInOrderColumn().IsInAscendingOrder()
                        .ShouldBeTrue("Order label should be in ascending order");
                    _clientSearch.IsPHISelectorChecked().ShouldBeEqual(minValue, "Is PHI selected?");
                    VerifyOrderControlArrowIcons(_clientSearch);
                    _clientSearch.DeleteCustomLabelByRow();
                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error should be present");
                    _clientSearch.GetPageErrorMessage()
                        .ShouldBeEqual("Invalid or missing data must be resolved before the record can be saved.");
                    _clientSearch.ClosePageError();
                    _clientSearch.ClickOnDeleteIcon();
                    _clientSearch.GetAvailableListCount().ShouldBeEqual(maxValue - 1, "Verify Delete Icon functionalilty");
                }
                finally
                {
                    _clientSearch.DeleteCustomFields(ClientEnum.TTREE.GetStringDisplayValue());
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")]//TE-714
        public void Verify_No_Error_Message_Displayed_For_Send_Appeals_Email()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var testData = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "Email", "Value");
                try
                {
                    StringFormatter.PrintMessage(
                        "verify no error message is displayed when Send Appeal Email if off and email address is not provided");
                    _clientSearch.SetAppealEmailInDB(ClientEnum.RPE.ToString());
                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.RPE.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.WaitForWorking();
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.ClickOnRadioButtonByLabel("Send Appeal Email");
                    _clientSearch.SetTextAreaValueByLabel("Appeal Email CC", "");
                    _clientSearch.GetTextAreaValueByLabel("Appeal Email CC").ShouldBeNullorEmpty("email value empty?");
                    _clientSearch.ClickOnRadioButtonByLabel("Send Appeal Email", false);
                    _clientSearch.GetSideWindow.Save();
                    _clientSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("error message displayed when  Send Appeal email setting switched off successfully?");
                }
                finally
                {
                    _clientSearch.SetAppealEmailInDB(ClientEnum.RPE.ToString(), testData);
                    _clientSearch.SwitchSendAppealEmailvalue(ClientEnum.RPE.ToString());
                }
            }
        }

        #endregion


        [Test] //CAR-3160(CAR-3209)
        [Author("Shyam Bhattarai")]
        public void Verify_form_validation_for_disable_appeals_at_flag_level_client_setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                var settingLabel = "Disable Appeals at Flag level";

                var flagListFromDB = _clientSearch.GetCommonSql.GetAllFlagsForAllActiveProductsByClient(ClientEnum.RPE.ToString());

                StringFormatter.PrintMessageTitle($"Verify the presence of {settingLabel}");
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.RPE.ToString(),
                    ClientSettingsTabEnum.Product.GetStringValue());
                _clientSearch.IsRadioButtonPresentByLabel(settingLabel)
                    .ShouldBeTrue($"{settingLabel} should be present in the Product/Appeals tab");

                _clientSearch.IsRadioButtonOnOffByLabel(settingLabel)
                    .ShouldBeFalse($"Default value of {settingLabel} should be No");
                _clientSearch.GetInfoHelpTooltipByLabel(settingLabel).ShouldBeEqual(
                    "Allows the selection of edit flags for which appeals cannot be opened.",
                    "Tooltip message should be correctly shown");

                StringFormatter.PrintMessageTitle(
                    $"Validating the {settingLabel} field and the transfer component");
                _clientSearch.ClickOnRadioButtonByLabel(settingLabel);
                _clientSearch.WaitForPageToLoad();

                _clientSearch.ScrollNonAppealFlagsIntoView();

                _clientSearch.IsNonAppealedFlagsSectionPresent()
                    .ShouldBeTrue($"Non Appealed Flags section should be present when {settingLabel} is switched to Yes");

                var allAvailableFlagsInTransferComponent =
                    _clientSearch.GenericGetAvailableAssignedList("Available Flags");

                allAvailableFlagsInTransferComponent.ShouldCollectionBeEqual(flagListFromDB,
                    "All the flags respective to the active products are getting displayed in the 'Non-Appealed Flags' section");

                _clientSearch.ClickSelectAllOrDeselectAll();
                _clientSearch.GenericGetAvailableAssignedList("Available Flags")
                    .ShouldCollectionBeEmpty(
                        "Available Flags column should be empty when all the flags are selected");

                _clientSearch.GenericGetAvailableAssignedList("Selected Flags")
                    .ShouldCollectionBeEqual(allAvailableFlagsInTransferComponent,
                        "All of the flags should be transferred to the selected flags column");

                StringFormatter.PrintMessage("Verifying Cancel Functionality");
                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.RefreshPage();
                if (_clientSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _clientSearch.GetSideBarPanelSearch.CloseSidebarPanel();
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());
                _clientSearch.GetSideWindow.ClickOnEditIcon();

                _clientSearch.IsRadioButtonOnOffByLabel(settingLabel)
                    .ShouldBeFalse($"{settingLabel} should be No since the changes were not saved");
                _clientSearch.IsNonAppealedFlagsSectionPresent()
                    .ShouldBeFalse($"Non Appealed Flags section should not be present when {settingLabel} is switched to No");
            }
        }

        [Test] //CAR-2207 (CAR-1802) + CAR-2352 (CAR-1757) -- Configuration Tab
        public void Verify_form_fields_of_configuration_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;

                var listOfClientConfigurationOptions = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Configuration_Setting_Options").Values.ToList();
                var configSettingsFromDatabase = _clientSearch.GetCommonSql.GetConfigSettingsFromDatabase(ClientEnum.SMTST.ToString());

                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.SMTST.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.ClickOnGridRowByRow();

                _clientSearch.GetClientDetailHeader()
                    .ShouldBeEqual("Client Settings", "Correct Header displayed for secondary Details");

                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Configuration.GetStringValue());

                StringFormatter.PrintMessageTitle("Verification whether the form fields are disabled");
                _clientSearch.IsAllRadioButtonDisabled().ShouldBeTrue("Is All Radio Button disabled");
                _clientSearch.IsAllTextBoxDisabled().ShouldBeTrue("Is All Text Box disabled");

                StringFormatter.PrintMessageTitle("Verification of tooltip messages and input fields");
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.IsAllRadioButtonDisabled().ShouldBeFalse("Is All Radio Button enabled after clicking edit icon");
                _clientSearch.IsAllTextBoxDisabled().ShouldBeFalse("Is All Text Box enabled after clicking edit icon");

                foreach (var option in listOfClientConfigurationOptions)
                {
                    VerifyInfoIconTooltipAndValidation(option, testData[option]);
                }

                StringFormatter.PrintMessageTitle("Verification of data not saved when navigating to other tab without saving the data first");
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.General.GetStringValue());
                _clientSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("A popup message should show up saying the unsaved data will be lost");
                _clientSearch.ClickOkCancelOnConfirmationModal(true);

                _clientSearch.GetCommonSql.GetConfigSettingsFromDatabase(ClientEnum.SMTST.ToString())
                    .ShouldCollectionBeEqual(configSettingsFromDatabase,
                        "Configuration settings should not be saved when navigating to another tab without saving the data");

                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Configuration.GetStringValue());
                _clientSearch.GetSideWindow.ClickOnEditIcon();

                VerifyChangesAreNotSavedInConfigurationSettingsTab(listOfClientConfigurationOptions,
                    configSettingsFromDatabase, _clientSearch);

                StringFormatter.PrintMessageTitle("Verification of data not saved when Cancel button is clicked");
                foreach (var optionLabel in listOfClientConfigurationOptions)
                {
                    if (optionLabel == "Allow Switch of non-Reverse Flags")
                        _clientSearch.SelectDropDownListForClientSettingsByLabel(
                            "Allow Switch of non-Reverse Flags", "YES");
                    else if (!_clientSearch.IsRadioButtonPresentByLabel(optionLabel))
                        _clientSearch.SetInputTextBoxValueByLabel(optionLabel, "123");

                    else if (_clientSearch.IsRadioButtonOnOffByLabel(optionLabel) &&
                             optionLabel != "Provider Alert Indicator option")
                        _clientSearch.ClickOnRadioButtonByLabel(optionLabel, false);
                    else
                        _clientSearch.ClickOnRadioButtonByLabel(optionLabel);
                }

                StringFormatter.PrintMessageTitle("Verification whether the form fields are disabled when Cancel is clicked");
                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.IsAllRadioButtonDisabled().ShouldBeTrue("Is All Radio Button disabled");
                _clientSearch.IsAllTextBoxDisabled().ShouldBeTrue("Is All Text Box disabled");

                _clientSearch.GetSideWindow.ClickOnEditIcon();

                VerifyChangesAreNotSavedInConfigurationSettingsTab(listOfClientConfigurationOptions,
                    configSettingsFromDatabase, _clientSearch);

                #region LOCAL METHODS

                void VerifyInfoIconTooltipAndValidation(string label, string expectedTooltipText)
                {
                    _clientSearch.GetInfoHelpTooltipByLabel(label).ShouldBeEqual(expectedTooltipText,
                        $"Tooltip message for '{label}' should be correct");

                    StringFormatter.PrintMessage("Validation of input fields");
                    if (!_clientSearch.IsRadioButtonPresentByLabel(label))
                        ValidateFormFieldsForNonRadioFields(label);

                    else
                    {
                        _clientSearch.IsRadioButtonPresentByLabel(label)
                            .ShouldBeTrue($"'{label}' should have a radio button");

                        if (_clientSearch.IsRadioButtonOnOffByLabel(label) && label != "Provider Alert Indicator option")
                            _clientSearch.ClickOnRadioButtonByLabel(label, false);

                        else
                            _clientSearch.ClickOnRadioButtonByLabel(label);
                    }
                }

                void ValidateFormFieldsForNonRadioFields(string label)
                {
                    switch (label)
                    {
                        case "Provider Alert Label":
                            _clientSearch.ClickOnRadioButtonByLabel("Provider Alert Indicator option", false);
                            _clientSearch.IsDivByLabelPresent(label)
                                .ShouldBeFalse(
                                    $"'{label}' should only be shown when Provider Alert Indicator option is set to YES");
                            _clientSearch.ClickOnRadioButtonByLabel("Provider Alert Indicator option");
                            _clientSearch.IsDivByLabelPresent(label)
                                .ShouldBeTrue($"'{label}' is shown when Provider Alert Indicator option is set to YES");

                            _clientSearch.SetInputTextBoxValueByLabel(label, new string('a', 26));
                            _clientSearch.GetInputTextBoxValueByLabel(label).Length.ShouldBeEqual(25,
                                $"The max length of character allowed in {label} is 25");
                            break;

                        case "Paid Exposure Threshold":
                            _clientSearch.SetInputTextBoxValueByLabel(label, "-1");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when {label} value is less than 0");
                            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be less than 0.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, "1000000000");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeFalse($"Max limit for {label} should be 1000000000");

                            _clientSearch.SetInputTextBoxValueByLabel(label, "1000000001");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when {label} value is greater than 1000000000");
                            _clientSearch.GetPageErrorMessage()
                                .ShouldBeEqual("Value cannot be greater than 1000000000.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, "1.0");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when decimal value is entered in {label} ");
                            _clientSearch.GetPageErrorMessage()
                                .ShouldBeEqual("Only numbers allowed.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, "a");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when decimal value is entered in {label} ");
                            _clientSearch.GetPageErrorMessage()
                                .ShouldBeEqual("Only numbers allowed.");
                            _clientSearch.ClosePageError();
                            break;

                        case "Provider Score Threshold":
                            _clientSearch.SetInputTextBoxValueByLabel(label, "-1");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when {label} value is less than 0");
                            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be less than 0.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, "1000");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeFalse($"Max limit for {label} should be 1000");

                            _clientSearch.SetInputTextBoxValueByLabel(label, "1001");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when {label} value is greater than 1000");
                            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be greater than 1000.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, ".");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when decimal value is entered in {label} ");
                            _clientSearch.GetPageErrorMessage()
                                .ShouldBeEqual("Only numbers allowed.");
                            _clientSearch.ClosePageError();

                            _clientSearch.SetInputTextBoxValueByLabel(label, "a");
                            _clientSearch.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue($"Error pops up when decimal value is entered in {label} ");
                            _clientSearch.GetPageErrorMessage()
                                .ShouldBeEqual("Only numbers allowed.");
                            _clientSearch.ClosePageError();
                            break;

                        case "Claim Number Title":
                            _clientSearch.SetInputTextBoxValueByLabel(label, new string('a', 51));
                            _clientSearch.GetInputTextBoxValueByLabel(label).Length.ShouldBeEqual(50,
                                $"The max length of character allowed in {label} is 50");
                            break;

                        case "Allow Switch of non-Reverse Flags":
                            var expectedListOfAllowSwitchOfReverseFlagsOptions =
                                testData["AllowSwitchOfReverseFlagsListItems"].Split(';').ToList();
                            _clientSearch.GetDropDownListForClientSettingsByLabel(label)
                                .ShouldCollectionBeEqual(expectedListOfAllowSwitchOfReverseFlagsOptions,
                                    $"The dropdown values for {label} should be correct");
                            break;
                    }
                }

                #endregion
            }



        }

        [Test]//CAR-2204(CAR-1806) + CAR-2307(CAR-1709) + CAR-2999(CAR-3017) + CAR-3111(CAR-3066)
        public void Verify_view_and_cancel_Product_Appeal_Settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                try
                {
                    #region Expected Data from XML Files

                    TestExtensions.TestName = new StackFrame().GetMethod().Name;
                    IDictionary<string, string> paramLists =
                        automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                    var productStatusLabel =
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Product_Status").Values.ToList();
                    var appealsSettingsIconValueList =
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Appeals_Settings_Help_Icon").Values.ToList();
                    var expectedAppealDueDatesTableColumnName =
                        paramLists["AppealDueDatesTableColumnName"].Split(';').ToList();

                    #endregion

                    _clientSearch.RefreshPage();

                    #region Search and click on Product Tab

                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                        ClientEnum.SMTST.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Product.GetStringValue());

                    #endregion

                    #region Verification of default view and editable view

                    StringFormatter.PrintMessageTitle("Verification of default view and editable view");

                    _clientSearch.IsAllRadioButtonDisabled().ShouldBeTrue("Is All Radio Button disabled");
                    _clientSearch.IsAllTextAreaDisabled().ShouldBeTrue("Is All Text Area disabled");
                    _clientSearch.IsAllTextBoxDisabled().ShouldBeTrue("Is All Text Box disabled");

                    _clientSearch.GetSideWindow.ClickOnEditIcon();

                    _clientSearch.IsAllRadioButtonDisabled().ShouldBeFalse("Is All Radio Button disabled");
                    _clientSearch.IsAllTextAreaDisabled().ShouldBeFalse("Is All Text Area disabled");
                    _clientSearch.IsAllTextBoxDisabled().ShouldBeFalse("Is All Text Box disabled");

                    #endregion

                    #region Verification of Product Status Label and value

                    StringFormatter.PrintMessageTitle("Verification of Product Status Label and value");

                    _clientSearch.GetActualProductList()
                        .ShouldCollectionBeEquivalent(_clientSearch.GetCommonSql.GetAllProductList(true),
                            "Is Product List Equivalent?");

                    var productsOnOrOffList = _clientSearch.GetCommonSql.GetAllProductValueList(ClientEnum.SMTST.ToString());

                    for (int productIndex = 0; productIndex < productStatusLabel.Count; productIndex++)
                    {
                        _clientSearch.IsRadioButtonOnOffByLabel(productStatusLabel[productIndex], productsOnOrOffList[productIndex])
                            .ShouldBeTrue($"Is Radio button {(productsOnOrOffList[productIndex] ? "ON" : "OFF")} for {productStatusLabel[productIndex]}");
                    }

                    #endregion

                    #region Verification of dependency of Appeal Due Date Calculation Input field to Product ON/OFF and values

                    var expectedAppealDueDatesList = _clientSearch.GetCommonSql.GetAppealDueDatesValueFromDatabase(ClientEnum.SMTST.ToString());

                    for (int i = 0; i < productStatusLabel.Count; i++)
                    {
                        if (productsOnOrOffList[i])
                        {
                            var inputValueList =
                                _clientSearch.GetAppealDueDatesInputFieldsByLabel(productStatusLabel[i], false);
                            inputValueList.ShouldCollectionBeEqual(expectedAppealDueDatesList.GetRange(i * 4, 4),
                                $"Turn around days match with DB for {productStatusLabel[i]}");
                        }
                        else
                        {
                            _clientSearch.IsAppealDueDatesInputFieldsByLabelPresent(productStatusLabel[i])
                                .ShouldBeFalse(
                                    $"Is {productStatusLabel[i]} product display in Appeal Due Date Calculation?");
                        }
                    }

                    #endregion

                    StringFormatter.PrintMessageTitle("Verification of Appeal Settings ON/OFF and Help Icon tooltip value");
                    VerificationOfAppealSettingsOnOffAndHelpIconValue(appealsSettingsIconValueList, out var expectedAppealcc, out var MappingOfAppealSettingsLabelWithDefaultValue,_clientSearch);

                    #region Verification of Help Icon and Column Names in Appeal Due Date Calculation section

                    StringFormatter.PrintMessage("Verification of Help Icon and Column Names in Appeal Due Date Calculation section");

                    _clientSearch.GetHeaderHelpIcon("Appeal Due Date Calculation").ShouldBeEqual(
                        "Defines the max # of days in which an appeal can be completed based on the create date and the client appeal SLA.",
                        "Is tooltip of help icon of Appeal Due Date Calculation equals?");
                    _clientSearch.GetAppealDueDatesTableColumnName()
                        .ShouldCollectionBeEqual(expectedAppealDueDatesTableColumnName, "Is Column Name Equals?");

                    #endregion

                    StringFormatter.PrintMessageTitle("Verification of dependency behaviour of Appeal Settings");
                    VerifyDependencyBehaviorOfAppealSettings(MappingOfAppealSettingsLabelWithDefaultValue, _clientSearch);

                    StringFormatter.PrintMessageTitle("Verification of Cancel button for product and Appeals Active");
                    VerificationOfCancelButton(productStatusLabel, productsOnOrOffList, MappingOfAppealSettingsLabelWithDefaultValue, expectedAppealcc, _clientSearch);
                }

                finally
                {
                    if (_clientSearch.IsPageErrorPopupModalPresent())
                        _clientSearch.ClosePageError();
                }
            }
        }

        [Test] //CAR-1757 (CAR-2352)
        public void Verify_form_fields_of_workflow_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                var testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                const string completeBatchWithPendedClaimsHeaderLabel = "Complete Batch with Pended Claims";
                var listOfActiveProducts =
                    _clientSearch.GetCommonSql.GetAllActiveProductsAbbrvForClient(ClientEnum.SMTST.ToString());

                _clientSearch.RefreshPage();
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.WorkFlow.GetStringValue());

                #region FORM VALIDATION

                StringFormatter.PrintMessageTitle("Verifying whether the form fields are disabled when edit icon is not clicked");
                _clientSearch.IsAllRadioButtonDisabled().ShouldBeTrue("All radio buttons in the form should be disabled when edit icon is not clicked");
                _clientSearch.IsClientSettingsFormReadOnly().ShouldBeTrue("The form should be readonly before the edit icon is clicked");

                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.IsAllRadioButtonDisabled().ShouldBeFalse("The form should be enabled for edit once the edit icon is clicked");
                _clientSearch.IsClientSettingsFormReadOnly().ShouldBeFalse("The form should be editable once the edit icon is clicked");

                StringFormatter.PrintMessageTitle("Verification of tooltip messages");
                _clientSearch.GetInfoHelpTooltipByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue())
                    .ShouldBeEqual(testData["AutomatedBatchReleaseToolTip"],
                        $"Tooltip message should be correct for {WorkflowTabEnum.AutomatedBatchRelease.GetStringValue()}");

                _clientSearch
                    .GetToolTipTextForCompleteBatchWithPendedClaims(completeBatchWithPendedClaimsHeaderLabel)
                    .ShouldBeEqual(testData["CompleteBatchWithPendedClaimsTooltip"],
                        $"Tooltip message should be correct for {completeBatchWithPendedClaimsHeaderLabel}");

                StringFormatter.PrintMessageTitle("Verifying whether only active products are getting displayed");
                _clientSearch.GetProductListInWorkflowTab().ShouldCollectionBeEqual(listOfActiveProducts,
                    "Only active products for the client should be displayed");

                StringFormatter.PrintMessageTitle($"Verification of the '{WorkflowTabEnum.AutomatedBatchRelease.GetStringValue()}' section");
                _clientSearch.IsRadioButtonOnOffByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue())
                    .ShouldBeFalse($"The '{WorkflowTabEnum.AutomatedBatchRelease.GetStringValue()}' should be set to NO by default");

                AreHiddenLabelsVisible().ShouldBeFalse("Hidden input fields should not if 'Automated Batch Release' is set to NO");
                _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());
                AreHiddenLabelsVisible().ShouldBeTrue("Hidden input fields should be visible once 'Automated Batch Release' is set to YES");

                StringFormatter.PrintMessage("Validation of the form input fields");
                var returnFileFrequencyValues = testData["ReturnFileFrequencyValues"].Split(';').ToList();
                ValidateAutomatedBatchReleaseFormFields(returnFileFrequencyValues, _clientSearch);

                _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(), "");
                _clientSearch.GetSideWindow.Save();
                _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue($"{WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()} is a required field");
                _clientSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before the record can be saved.");
                _clientSearch.ClosePageError();

                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());

                #endregion

                #region CANCEL BUTTON VALIDATION

                StringFormatter.PrintMessageTitle("Verify data is not saved when cancel button is clicked");

                var originalData = _clientSearch.GetListOfAllDataInWorkflowTab(listOfActiveProducts);

                EnterValuesInTheForm();
                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.GetSideWindow.ClickOnEditIcon();

                VerifyDataInWorkflowTab(originalData, listOfActiveProducts, _clientSearch);

                StringFormatter.PrintMessageTitle("Verify data is not saved when another tab is clicked before saving");

                EnterValuesInTheForm();
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.General.GetStringValue());
                _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("A warning popup should appear saying 'Any unsaved changes will be lost'");
                _clientSearch.GetPageErrorMessage().ShouldBeEqual("Any unsaved changes will be lost.");
                _clientSearch.ClickOkCancelOnConfirmationModal(true);
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.WorkFlow.GetStringValue());
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                VerifyDataInWorkflowTab(originalData, listOfActiveProducts, _clientSearch);

                #endregion

                #region LOCAL METHODS
                bool AreHiddenLabelsVisible() =>
                    _clientSearch.IsRowDisplayedByLabel(WorkflowTabEnum.BeginClaimRelease.GetStringValue()) &&
                    _clientSearch.IsRowDisplayedByLabel(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()) &&
                    _clientSearch.IsRowDisplayedByLabel(WorkflowTabEnum.ReturnFileFrequency.GetStringValue()) &&
                    _clientSearch.IsRowDisplayedByLabel(WorkflowTabEnum.FailureAlert.GetStringValue());

                void EnterValuesInTheForm()
                {
                    var newValues = testData["EditedValues"].Split(';').ToList();
                    _clientSearch.SelectDropDownItemByLabelInWorkflowTab(WorkflowTabEnum.BeginClaimRelease.GetStringValue(), newValues[0]);

                    _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(),
                        newValues[1]);
                    _clientSearch.SelectDropDownItemByLabelInWorkflowTab(WorkflowTabEnum.ReturnFileFrequency.GetStringValue(),
                        newValues[2]);
                    _clientSearch.SelectDropDownItemByLabelInWorkflowTab(WorkflowTabEnum.FailureAlert.GetStringValue(),
                        newValues[3]);

                    foreach (var product in listOfActiveProducts)
                    {
                        _clientSearch.IsRadioButtonPresentByLabel(product).ShouldBeTrue($"'{product}' is present as a radio button");
                        _clientSearch.ClickOnRadioButtonByLabel(product);
                    }
                }
                #endregion
            }
        }

        [Test] //CAR-1804(CAR-2206)
        public void Verify_view_security_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                var clientUserAccessList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ClientUserAccessOptions").Values.ToList();
                var expectedPasswordToolTipMessage = "Number of days between required password resets. Value cannot be greater than 60.";
                var expectedClientUerAccessToolTipMessage = "Determines level of accessibility of PHI to client users. Options: All, None, or Requires User Authority.";
                var expectedWhiteListingToolTipMessage = "Allows restriction of client users to access Nucleus from the following list of IP addresses.  Enter IP and CIDR in comma separated list (e.g. 127.0.0.1,127.0.0.2/32).";

                _clientSearch.RefreshPage();

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.SMTST.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                _clientSearch.ClickOnClientSettingTabByTabName("Security");

                StringFormatter.PrintMessage("Verification of form labels being displayed correctly");
                _clientSearch.IsClientSettingsFormReadOnly().ShouldBeTrue("Is Security form read-only?");
                _clientSearch.AreSecurityTabSpecificationsPresent("Password Duration", "Client User PHI/PII Access", "Limit Access by client IP address")
                    .ShouldBeTrue("Form Labels should be correctly displayed");

                StringFormatter.PrintMessage("Verification of Password Duration before editing");
                _clientSearch.GetInfoHelpTooltipByLabel("Password Duration")
                    .ShouldBeEqual(expectedPasswordToolTipMessage, "Tooltip message for Password Duration should match");
                _clientSearch.GetInputTextBoxValueByLabel("Password Duration").ShouldBeEqual("60", "Default value of 'Password Duration' should be 60");

                StringFormatter.PrintMessage("Verification of Client user access before editing");
                _clientSearch.GetInfoHelpTooltipByLabel("Client User PHI/PII Access").ShouldBeEqual(
                    expectedClientUerAccessToolTipMessage, "Tooltip message for Client user access should match");
                _clientSearch.GetDefaultValueForClientUserAccess().ShouldBeEqual("Requires User Authority", "Default text message should match");

                StringFormatter.PrintMessage("Verification of White-listing options before editing");
                _clientSearch.IsRadioButtonPresentByLabel("Limit Access by client IP address").ShouldBeTrue("Are Yes/No radio buttons present for " +
                                                                                                               "'Limit Access by client IP address'?");
                //_clientSearch.AreYesNoRadioButtonsPresent().ShouldBeTrue("Are Yes/No radio buttons present?");
                _clientSearch.GetInfoHelpTooltipByLabel("Limit Access by client IP address").ShouldBeEqual(
                    expectedWhiteListingToolTipMessage, "Tool tip message for limit access by client IP should match");

                StringFormatter.PrintMessage("Verification of Save without any changes");
                var lastModifiesValue = _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by");
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.IsClientSettingsFormReadOnly().ShouldBeFalse("Is client settings form read only?");
                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(lastModifiesValue,
                    "Last modified should remain unchanged when saving without any changes");

                StringFormatter.PrintMessage("Validation after clicking Edit icon for password field");
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "A");
                HandleErrorPopUp("Only numbers allowed.", "Only numeric value should be allowed", _clientSearch);

                _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "61");
                HandleErrorPopUp("Value cannot be greater than 60.", "Maximum range of password is 60", _clientSearch);

                _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "0");
                HandleErrorPopUp("Value cannot be less than 1.", "Minimum range of password is 1", _clientSearch);

                _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "");
                _clientSearch.GetSideWindow.Save();
                HandleErrorPopUp("Invalid or missing data must be resolved before the record can be saved.", "Empty password reset field is not allowed", _clientSearch);

                StringFormatter.PrintMessage("Verification of Client user access PHI/PII access");
                _clientSearch.GetDropDownListForClientSettingsByLabel("Client User PHI/PII Access")
                    .ShouldBeEqual(clientUserAccessList, "Option list should match");

                StringFormatter.PrintMessage("Verification on clicking YES for Limit Access by client IP address");
                _clientSearch.ClickOnRadioButtonByLabel("Limit Access by client IP address");
                _clientSearch.IsCotivitiAccessByIPAddressPresent().ShouldBeTrue("Is Cotiviti by IP address present?");
                _clientSearch.GetInfoHelpTooltipByLabel("Cotiviti access by IP address").ShouldBeEqual("Requires Cotiviti users to access this client from the following list of IP addresses.", "Tooltip message should match");
                _clientSearch.IsRadioButtonPresentByLabel("Cotiviti access by IP address").ShouldBeTrue("Are Yes/No radio buttons present?");
                _clientSearch.IsTextAreaPresentByLabel("Cotiviti access by IP address").ShouldBeTrue("TextArea should be present");
                _clientSearch.GetValueOfTextBox("Cotiviti access by IP address").Length
                    .ShouldBeGreater(0, "List of current known Cotiviti IP addresses should be displayed");
                _clientSearch.IsTextAreaByLabelDisabled("Cotiviti access by IP address").ShouldBeTrue("Cotiviti access by IP address textarea is not editable");

                _clientSearch.ClickOnRadioButtonByLabel("Cotiviti access by IP address", false);
                _clientSearch.IsTextAreaPresentByLabel("Cotiviti access by IP address")
                    .ShouldBeFalse("TextArea should not be present if 'NO' is selected in 'Cotiviti access by IP address'");
                _clientSearch.ClickOnRadioButtonByLabel("Cotiviti access by IP address");

                StringFormatter.PrintMessage("Validation of invalid IP");
                _clientSearch.SetIp("abc");
                _clientSearch.GetSideWindow.Save();
                HandleErrorPopUp("Comma separated valid IP and CIDR (e.g. 127.0.0.1,127.0.0.2/32) is required before the record can be saved.", "Validation message should match", _clientSearch);

                _clientSearch.SetIp("123.0.0");
                _clientSearch.GetSideWindow.Save();
                HandleErrorPopUp("Comma separated valid IP and CIDR (e.g. 127.0.0.1,127.0.0.2/32) is required before the record can be saved.", "Validation message should match", _clientSearch);

                _clientSearch.ClickOnRadioButtonByLabel("Limit Access by client IP address", false);

                StringFormatter.PrintMessage("Verification of cancel functionality");
                _clientSearch.SetInputTextBoxValueByLabel("Password Duration", "10");
                _clientSearch.SelectDropDownListForClientSettingsByLabel("Client User PHI/PII Access", "All");
                _clientSearch.ClickOnRadioButtonByLabel("Limit Access by client IP address");

                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.IsClientSettingsFormReadOnly().ShouldBeTrue("Is client settings form read only?");

                _clientSearch.GetInputTextBoxValueByLabel("Password Duration")
                    .ShouldBeEqual("60", "Value should not be saved for 'Password Duration' when Cancel is clicked");
                _clientSearch.GetInputTextBoxValueByLabel("Client User PHI/PII Access")
                    .ShouldBeEqual("Requires User Authority", "'Client User PHI/PII Access' should not be saved when Cancel is clicked");
                _clientSearch.IsRadioButtonOnOffByLabel("Limit Access by client IP address")
                    .ShouldBeFalse("'Limit Access by client IP addres' should not be saved when Cancel is clicked");
            }
        }

        [Test] //CAR-1913(CAR-2350)+CAR-2755(CAR-2688)
        public void Validate_Edit_settings_manager_icon_enabled_and_navigation_from_client_settings_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                var _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();

                StringFormatter.PrintMessage("Listing Assigned clients for the user");
                _newUserProfileSearch.SearchUserByNameOrId(new List<string> {automatedBase.EnvironmentManager.Username}, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager.Username);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                var assignedClientList = _newUserProfileSearch.GetAvailableAssignedList(2, false);
                var assignedClientsCount = assignedClientList.Count;
                _clientSearch = _newUserProfileSearch.NavigateToClientSearch();
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                var clientsList = _clientSearch.GetGridViewSection.GetGridListValueByCol();
                var unassignedClientList = clientsList.Except(assignedClientList).ToList();

                StringFormatter.PrintMessage("Verification that Edit settings icon is enabled for assigned clients");
                foreach (var clients in assignedClientList)
                {
                    _clientSearch.ClickOnGridRowByClientName(clients);
                    _clientSearch.WaitForWorking();
                    _clientSearch.IsEditSettingsIconEnabled().ShouldBeTrue("Is edit settings manager icon enabled?");
                }

                StringFormatter.PrintMessage("Verification of tooltip");
                _clientSearch.GetEditSettingsToolTip().ShouldBeEqual(
                    PageHeaderEnum.EditSettingsManager.GetStringValue(),
                    "Tooltip value should match");

                StringFormatter.PrintMessage(
                    "Verification that user will be navigated to the new Edit Settings Manager page for the client that was selected");
                _clientSearch.ClickOnEditSettingsIcon();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                    PageHeaderEnum.EditSettingsManager.GetStringValue(),
                    "Page should be navigated to Edit Settings Manager page");
                //automatedBase.CurrentPage.GetClientCode().ShouldBeEqual(assignedClientList[assignedClientsCount-1]);

                StringFormatter.PrintMessage(
                    "Verification that Edit Settings icon should not disabled for unassigned clients");
                automatedBase.CurrentPage.NavigateToClientSearch();
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _newUserProfileSearch.WaitForWorkingAjaxMessage();
                foreach (var clients in unassignedClientList)
                {
                    _clientSearch.ClickOnGridRowByClientName(clients);
                    _clientSearch.WaitForWorking();
                    _clientSearch.IsEditSettingsIconEnabled().ShouldBeTrue("Is edit settings manager icon enabled?");
                }

                _newUserProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "Inactive");
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _newUserProfileSearch.WaitForWorkingAjaxMessage();
                var inActiveClientCount = _clientSearch.GetGridViewSection.GetGridListValueByCol();
                foreach (var clients in inActiveClientCount)
                {
                    _clientSearch.ClickOnGridRowByClientName(clients);
                    _clientSearch.WaitForWorking();
                    _clientSearch.IsEditSettingsIconEnabled()
                        .ShouldBeTrue("Is edit settings manager icon enabled for Inactive Client?");
                }
            }
        }

        [Test]//TE-389+CAR-2075(CAR-1931)
        public void Verify_security_and_navigation_of_the_new_Client_search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                CommonValidations _commonValidation;
                _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                LoginPage _login;
                _login = automatedBase.Login;

                string _clientMaintenance = RoleEnum.NucleusAdmin.GetStringValue();

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                var expectedClientSettingTab =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Client_Settings_Tab").Values.ToList();

                StringFormatter.PrintMessage("Verify Find Clients Panel Open By Default");
                _clientSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" By default Find Clients panel displayed ?");
                StringFormatter.PrintMessage("Verify Sort and Find Clients Panel Control Icon");
                _clientSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _clientSearch.GetGridViewSection.IsSideBarIconPresent()
                    .ShouldBeTrue("Is Side Bar Icon Present ?");
                _clientSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _clientSearch.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Sidebar open after toggle?");
                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Settings,
                    new List<string> { SubMenu.ClientSearch },
                    _clientMaintenance,
                    new List<string> { PageHeaderEnum.ClientSearch.GetStringValue() },
                    _login.LoginAsUserHavingNoAnyAuthority, new[] { "Test4", "Automation4" });
                
                #region CAR-2075(CAR-1931) ,CAR-2204 
                StringFormatter.PrintMessageTitle("Verification of ReadOnly User does not have authority to edit client settings");
                _clientSearch = automatedBase.CurrentPage.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities()
                    .NavigateToClientSearch();
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                    ClientEnum.SMTST.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                foreach (var tab in expectedClientSettingTab)
                {
                    _clientSearch.ClickOnClientSettingTabByTabName(tab);
                    _clientSearch.GetSideWindow.IsEditDisabled().ShouldBeTrue("Edit Icon should be disabled.");
                }
                #endregion
            }
        }

        [Test]//TE-444
        public void Verify_New_Client_search_Filters() 
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                _allClientList = _clientSearch.GetAllClientList();
                _allClientList.Insert(0, "All");

                var expectedClientStatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Client_Status")
                    .Values.ToList();
                var expectedProductTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "product").Values.ToList();
                StringFormatter.PrintMessage("Verify Default filter List");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client Status", expectedClientStatusList, _clientSearch);
                ValidateSingleDropDownForDefaultValueAndExpectedList("Client Code", _allClientList, _clientSearch);
                _clientSearch.GetdefaultValueInFilterList("Client Name").ShouldBeNullorEmpty("client name should not have a default value");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList, _clientSearch, false);

                StringFormatter.PrintMessage("Verify maximum allowed value for Client name");
                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Name", string.Concat(Enumerable.Repeat("a1@_A", 21)));
                _clientSearch.SideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Client Name").ShouldBeEqual(100, "Verification of Maximum length of Client name");

                StringFormatter.PrintMessage("Verify clear Filters");
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", expectedClientStatusList[0]);
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.SMTST.ToString());

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Product", expectedProductTypeList[1]);
                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Name", ClientEnum.SMTST.GetStringValue());
                _clientSearch.SideBarPanelSearch.ClickOnClearLink();
                ValidateDefaultFilterValue(_clientSearch);
            }

            
        }

        [Test] //TE-445 + CAR-2307(CAR-1709)
        public void Verify_Correct_Primary_Data_Displayed_For_Client_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                _activeProductListForClientDB = _clientSearch.GetActiveProductListForClientDB();

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var expectedProductTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "product").Values.ToList();
                expectedProductTypeList.RemoveAt(0);
                var expectedList = _clientSearch.GetPrimaryDataResultFromDatabase(ClientEnum.SMTST.GetStringValue(), ClientEnum.SMTST.ToString());
                var expectedPartialMatchCount =
                    _clientSearch.GetResultFromDatabaseForPartialMatchOnClientName(paramLists["PartialName"]);

                _clientSearch.RefreshPage();
                StringFormatter.PrintMessageTitle("Verify No Search executed while landing on Client Search page.");
                _clientSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(0,
                     "When landing on the Client Search page, no result must be executed.");
                _clientSearch.GetGridViewSection.IsNoDataMessagePresentInLeftSection()
                    .ShouldBeTrue("Is No data message displayed?");
                _clientSearch.GetGridViewSection.GetNoDataMessageText().ShouldBeEqual("No Data Available");

                StringFormatter.PrintMessage("Verify search result for default search filter values and default sorting");
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual(0, "Search Result Should Be Populated");
                _clientSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeSorted(false,
                    "Search result must be sorted by Client Code in ascending order by default.");

                StringFormatter.PrintMessage("Verify search results");
                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Name", ClientEnum.SMTST.GetStringValue());
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", ClientEnum.SMTST.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();

                _clientSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(1, "Result Should Be Displayed");
                _clientSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "Client Code Values Equal ?");
                _clientSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "Client Name Values Equal ?");
                _clientSearch.GetGridViewSection.GetLabelInGridByColRow(12).ShouldBeEqual("COB:");


                StringFormatter.PrintMessageTitle("Verifying product labels and check mark icon for product labels that are active");
                _clientSearch.GetProductLabels()
                    .ShouldCollectionBeEqual(expectedProductTypeList, "Both the list should match");

                for (int i = 0; i < expectedProductTypeList.Count; i++)
                {
                    if (_activeProductListForClientDB.Contains(expectedProductTypeList[i]))
                    {
                        _clientSearch.IsCheckMarkPresent(expectedProductTypeList[i])
                            .ShouldBeTrue("Check Mark Icon should be present for products that are active for the client");
                    }
                    else
                    {
                        _clientSearch.IsCheckMarkPresent(expectedProductTypeList[i])
                            .ShouldBeFalse("Check Mark Icon should not be present for products that are not active for the client");
                    }
                }
                _clientSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Results will yield partial matches on the Client Name");

                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Name", paramLists["PartialName"]);
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(expectedPartialMatchCount, "Results should match");
                _clientSearch.SideBarPanelSearch.ClickOnClearLink();
                
                StringFormatter.PrintMessage("verify no matching records found message");
                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Name", paramLists["IncorrectName"]);
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("Verify Message visible when results not available");
                _clientSearch.SideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Matching records found?");
            }
        }

        [Test] //TE-445
        public void Verify_Search_Result_For_Client_Status_And_Product_Search_Filter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                var expectedActiveClientList = _clientSearch.GetCommonSql.GetClientFromDatabaseByClientStatus("T");
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "Active");
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                if (_clientSearch.GetGridViewSection.IsLoadMorePresent())
                    _clientSearch.GetGridViewSection.ClickLoadMore();
                _clientSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(expectedActiveClientList, "Lists Should Match");
                _clientSearch.SideBarPanelSearch.ClickOnClearLink();

                var expectedInactiveClientList = _clientSearch.GetCommonSql.GetClientFromDatabaseByClientStatus("F");
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "Inactive");
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                if (!_clientSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
                {
                    if (_clientSearch.GetGridViewSection.IsLoadMorePresent())
                        _clientSearch.GetGridViewSection.ClickLoadMore();
                    var rowCount = _clientSearch.GetGridViewSection.GetGridRowCount();
                    _clientSearch.GetGridViewSection.GetGridListValueByCol()
                        .ShouldCollectionBeEqual(expectedInactiveClientList, "Both List Should Match");
                    if (rowCount > 0)
                    {
                        _clientSearch.GetInactiveIconCount().ShouldBeEqual(rowCount, "Is Inactive Icon Present ?");

                    }
                }
                else
                {
                    expectedInactiveClientList.ShouldBeNull("There is no Inactive client list.");
                }

                _clientSearch.SideBarPanelSearch.ClickOnClearLink();
                var expectedClientListByProduct = _clientSearch.GetClientFromDatabaseByProduct();
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "All");
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Product", ProductEnum.CV.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEqual(expectedClientListByProduct, "Both List Should Match");
                _clientSearch.SideBarPanelSearch.ClickOnClearLink();
            }

            
        }

        [Test] //TE-445
        public void Verify_Navigation_To_Client_Profile_Page_From_New_Client_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                //TODO : Can remove this test.
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());
                _clientSearch.GetClientSettingsSidePaneHeaderText().ShouldBeEqual("Client Settings",
                    "Is header correct for Client Settings side view?");
            }
        }

        [Test]//TE-444
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "All");
                _clientSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _clientSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] //TE-447
        public void Verify_Sorting_Options_In_New_Client_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Client_Sorting_options").Values
                        .ToList();
                try
                {
                    _clientSearch.GetGridViewSection.IsFilterOptionIconPresent()
                        .ShouldBeTrue("Filter Icon Option Should Be Present");
                    _clientSearch.GetGridViewSection.GetFilterOptionTooltip()
                        .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                    _clientSearch.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();

                    StringFormatter.PrintMessage("Verify sort using client code");
                    _clientSearch.ClickOnFilterOptionListRow(1);
                    _clientSearch.GetGridViewSection.GetGridListValueByCol().IsInDescendingOrder()
                        .ShouldBeTrue("Search result must be sorted by client code in Descending");
                    _clientSearch.ClickOnFilterOptionListRow(1);
                    _clientSearch.GetGridViewSection.GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("Search result must be sorted by client code in Descending");

                    StringFormatter.PrintMessage("Verify other sort options");
                    VerifySortingOptionsInClientSearch(3, 2, filterOptions[1], _clientSearch);
                    VerifySortingOptionsInClientSearch(12, 3, filterOptions[2], _clientSearch);
                }
                finally
                {
                    _clientSearch.ClickOnClearSort();
                    _clientSearch.GetGridViewSection.GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("Is default sort applied after clear sort?");
                }
            }
        }

        [Test] //TE-493
        public void Verify_Navigation_To_Other_Pages_From_New_Client_Search_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                LogicSearchPage _logicSearch;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                try
                {
                    StringFormatter.PrintMessage("Verify Navigation To Core Page From New Client Search Page");
                    _logicSearch = _clientSearch.NavigateToLogicSearch();
                    _logicSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue(),
                        "User should be navigated to logic search core page");
                    _clientSearch = _logicSearch.NavigateToClientSearch();

                }
                finally
                {
                    automatedBase.CurrentPage.NavigateToClientSearch();
                }
            }
        }

        [Test] //CAR-2205(CAR-1805)
        public void Verify_view_of_Custom_Fields_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                ClaimActionPage _claimAction;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                #region Expected Data from XML
                IDictionary<string, string> paramListsFromClassInit = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, "ClassInit");

                TestExtensions.TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var expectedLabelInCustomTab = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Custom_Field")
                    .Values.ToList();
                var expectedList = _clientSearch.GetCustomFieldListFromDatabase(ClientEnum.SMTST.GetStringValue());
                int maxValue = 6;
                int minValue = 1;

                #endregion

                _clientSearch.RefreshPage();

                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                    ClientEnum.SMTST.ToString());
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                _clientSearch.ClickOnClientSettingTabByTabName(ClientSettingsTabEnum.Custom.GetStringValue());
                _clientSearch.WaitForWorking();

                var customLabeList = _clientSearch.GetCustomFieldList();
                _clientSearch.IsCustomFieldsLabelPresent().ShouldBeTrue("Custom Field label should be present");
                _clientSearch.IsAvailableFieldsDropdownPresent()
                    .ShouldBeFalse("Available Fields dropdown should not be present");
                _clientSearch.GetHeaderLabelInCustomField()
                    .ShouldCollectionBeEqual(expectedLabelInCustomTab, "Is Header label Equals?");
                _clientSearch.IsAllTextBoxDisabled().ShouldBeTrue("All Text box area should be disabled.");
                _clientSearch.IsPHISelectorCheckboxDisabled().ShouldBeTrue("PHI Selector Checkbox should be disabled.");

                _clientSearch.GetSideWindow.ClickOnEditIcon();
                _clientSearch.IsAvailableFieldsDropdownPresent()
                    .ShouldBeTrue("Available Fields dropdown should be present");
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Available Fields")
                    .ShouldBeFalse("Available Fields dropdown should be enabled");
                _clientSearch.IsAllTextBoxDisabled().ShouldBeFalse("All Text box area should be enabled");
                _clientSearch.IsPHISelectorCheckboxDisabled().ShouldBeFalse("PHI Selector Checkbox should be enabled.");
                _clientSearch.GetAvailableListCount()
                    .ShouldBeLessOrEqual(maxValue, "Available List should be less than 6");
                _clientSearch.GetNumericalLabelInOrderColumn().IsInAscendingOrder()
                    .ShouldBeTrue("Order label should be in ascending order");
                _clientSearch.IsPHISelectorChecked().ShouldBeEqual(1, "Is PHI data selected");

                _clientSearch.GetCustomFieldDataListByColumn()
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "Custom Field Values Equal ?");
                _clientSearch.GetCustomFieldLabelInputList()
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(),
                        "Custom Field Label Values Equal ?");
                _clientSearch.GetPHISelectorCheckboxList()
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[2]).ToList(), "Client Type Values Equal ?");
                _clientSearch.GetTotalDeleteIcon()
                    .ShouldBeEqual(customLabeList.Count, "Is delete icon present in every custom field");


                SelectCustomLabelFromAvailableFields(maxValue, _clientSearch);
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Available Fields")
                    .ShouldBeTrue("Available Fields dropdown should be disabled");
                _clientSearch.GetAvailableListCount().ShouldBeEqual(maxValue, "Available List should be less than 6");
                _clientSearch.GetSideWindow.Cancel();
                _clientSearch.WaitForWorking();
                _clientSearch.GetAvailableListCount().ShouldBeLessOrEqual(maxValue,
                    "Changes should not be saved once Cancel button is clicked");
                _clientSearch.IsPHISelectorChecked().ShouldBeEqual(minValue, "Is PHI Data Present?");
                _claimAction = _clientSearch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(paramListsFromClassInit["ClaimSequence"], true);
                _claimAction.GetCustomFieldsLabel()
                    .ShouldCollectionBeEqual(customLabeList, "Is Custom Filed Label List Equals?");
            }
        }

        [Test] // TE-721
        public void Verify_Client_LOGO_Upload_Functionality_In_Client_Setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                var FileUploadPage = _clientSearch.GetFileUploadPage;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var auditDataListForClientLogo = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "AuditDataForClientLogo").Values.ToList();
                var invalidFileType = testData["fileWithInvalidType"];
                var fileName = testData["validFileName"];
                var invalidFileName = testData["invalidFileName"];
                var clientCode = ClientEnum.TTREE.ToString();
                var lastModifiedName = _clientSearch.GetLogoModifiedByUser(clientCode);
                var lastModifiedDateBeforeUpload = _clientSearch.GetLogoModifiedDate(clientCode);

                try
                {
                    _clientSearch = automatedBase.CurrentPage.ClickOnSwitchClient()
                        .SwitchClientTo(ClientEnum.TTREE, true).NavigateToClientSearch();

                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                        clientCode);
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.IsClientLogoUploadSectionPresent()
                        .ShouldBeTrue("client logo upload section present?");
                    _clientSearch.GetLastModifiedInformationForClientLogo().ShouldBeEqual(
                        $"{lastModifiedName} {lastModifiedDateBeforeUpload.Split(' ')[0]}",
                        "Last Updated Name Should Be Correct");
                    _clientSearch.IsChooseFileButtonDisabledForClientLogoUploadSection()
                        .ShouldBeTrue("File Upload Button Should Be Disabled Before Clicking On Edit Icon");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.IsChooseFileButtonDisabledForClientLogoUploadSection()
                        .ShouldBeFalse("File Upload Button Should Be enabled after Clicking On Edit Icon");

                    StringFormatter.PrintMessage("verify validation message when invalid filename is uploaded");
                    FileUploadPage.UploadMultipleClientLogo(invalidFileName);
                    HandleErrorPopUp($"LOGO file name must match logo_{clientCode} or logo_{clientCode}_black.",
                        "Correct Error Message Displayed?", _clientSearch);


                    StringFormatter.PrintMessage("Verify Validation message when invalid file type is uploaded");
                    FileUploadPage.UploadMultipleClientLogo(invalidFileType);
                    HandleErrorPopUp("Only .png files are allowed.", "Correct Error Message Displayed?", _clientSearch);

                    StringFormatter.PrintMessageTitle("Verify Clicking On Cancel link logo Is Not Saved");
                    FileUploadPage.UploadMultipleClientLogo(fileName);
                    _clientSearch.GetSideWindow.Cancel();
                    _clientSearch.GetLogoModifiedDate(clientCode).ShouldBeEqual(lastModifiedDateBeforeUpload,
                        "Last Modified Date Should Not Change");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();


                    StringFormatter.PrintMessageTitle("Verify error message when only one logo is uploaded");
                    FileUploadPage.UploadMultipleClientLogo(fileName.Split(',')[0], false);
                    HandleErrorPopUp($"There must be two logos: logo_{clientCode}.png and logo_{clientCode}_black.png",
                        "Correct Error Message Displayed?", _clientSearch);



                    StringFormatter.PrintMessage("Verify logo upload is successful with correct filename and filetype");
                    FileUploadPage.UploadMultipleClientLogo(fileName);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    var lastModifiedDateAfterUpload = _clientSearch.GetLogoModifiedDate(clientCode);
                    _clientSearch.GetLastModifiedInformationForClientLogo()
                        .ShouldBeEqual($"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");
                    _clientSearch.GetLogoModifiedByUser(clientCode).ShouldBeEqual(
                        _clientSearch.GetLoggedInUserFullName(),
                        "Last Modified Name Should Be Updated In Database");
                    lastModifiedDateAfterUpload.ShouldNotBeTheSameAs(lastModifiedDateBeforeUpload,
                        "When saved, previous file should be deleted and new file should be added and last modified date should be updated");

                    StringFormatter.PrintMessageTitle("Verify Changes Are Saved In Client Settings Audit Table");
                    var auditDataForLogoUploadFromDb = _clientSearch.GetClientSettingAuditFromDatabase(clientCode, 1);
                    var lastModifiedDate = Convert.ToDateTime(auditDataForLogoUploadFromDb[0][2])
                        .ToString("MM/dd/yyyy hh:mm tt").Trim();
                    var sysdate = DateTime.ParseExact(_clientSearch.GetCommonSql.GetSystemDateFromDB(),
                        "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    DateTime.ParseExact(lastModifiedDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture)
                        .AssertDateRange(sysdate.AddMinutes(-2),
                            sysdate.AddMinutes(1), "Last modified date should match");

                    auditDataForLogoUploadFromDb[0].RemoveAt(2);
                    auditDataForLogoUploadFromDb[0].GetRange(1, auditDataForLogoUploadFromDb[0].Count - 1)
                        .ShouldCollectionBeEquivalent(auditDataListForClientLogo, "Audit Data should Match");
                    auditDataForLogoUploadFromDb[0][0].ShouldBeEqual(
                        _clientSearch.GetUserSequenceForUser(auditDataListForClientLogo[0]),
                        "usersequence correct in the audit?");


                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);

                    _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(
                        $"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");
                    _clientSearch.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(),
                        "client logo should be displayed after upload");
                    var QuickLaunch = _clientSearch.ClickOnQuickLaunch();
                    automatedBase.QuickLaunch.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(),
                        "client logo should be displayed after upload");
                }
                finally
                {
                    _clientSearch.CloseAnyPopupIfExist();
                    _clientSearch = automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST)
                        .NavigateToClientSearch();
                }
            }
        }

        [Test] //TE-645 //TE-670
        public void Verify_Client_CIW_Document_Upload_Functionality_In_Client_Setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                var FileUploadPage = _clientSearch.GetFileUploadPage;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var auditDataListForCIWDocument = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "CIWAuditDataForCiwDocument").Values.ToList();
                var fileWithInvalidType = testData["fileWithInvalidType"];
                var validFileName = testData["validFileName"];
                var invalidFileName = testData["invalidFileName"];
                var lastModifiedName = _clientSearch.GetCIWLastUpdatedNameFromDb();
                var lastModifiedDateBeforeUpload = _clientSearch.GetCIWLastUpdatedDateFromDb();

                try
                {
                    _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                        ClientEnum.SMTST.ToString());
                    _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                    _clientSearch.WaitForWorkingAjaxMessage();
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearch.IsCIWUploadSectionPresent()
                        .ShouldBeTrue("CIW Upload Section Should Be Present In the Client Settings General Tab");
                    _clientSearch.GetLastModifiedInformation().ShouldBeEqual(
                        $"{lastModifiedName} {lastModifiedDateBeforeUpload.Split(' ')[0]}",
                        "Last Updated Name Should Be Correct");
                    _clientSearch.IsChooseFileButtonDisabledInCIWFileUploadSection()
                        .ShouldBeTrue("File Upload Button Should Be Disabled Before Clicking On Edit Icon");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.IsChooseFileButtonDisabledInCIWFileUploadSection()
                        .ShouldBeFalse("File Upload Button Should Be enabled after Clicking On Edit Icon");

                    StringFormatter.PrintMessageTitle("Verify Only File with xls File Type Can Be Uploaded");
                    FileUploadPage.UploadCIWDocument(fileWithInvalidType);
                    _clientSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page Error popUp model should be present");
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Only an Excel file can be added.");
                    _clientSearch.ClosePageError();

                    StringFormatter.PrintMessageTitle("Verify Invalid Document Name");
                    FileUploadPage.UploadCIWDocument(invalidFileName);
                    _clientSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page Error popUp model should be present");
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("CIW file name must match SMTST");
                    _clientSearch.ClosePageError();

                    StringFormatter.PrintMessageTitle("Verify Clicking On Cancel Link CIW File Is Not Saved");
                    FileUploadPage.UploadCIWDocument(validFileName);
                    _clientSearch.GetSideWindow.Cancel();
                    _clientSearch.GetCIWLastUpdatedDateFromDb().ShouldBeEqual(lastModifiedDateBeforeUpload,
                        "Last Modified Date Should Not Change");

                    StringFormatter.PrintMessageTitle("Verify CIW File Is Saved In Database");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    FileUploadPage.UploadCIWDocument(validFileName);
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    var lastModifiedDateAfterUpload = _clientSearch.GetCIWLastUpdatedDateFromDb();
                    _clientSearch.GetLastModifiedInformation()
                        .ShouldBeEqual($"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");
                    _clientSearch.GetCIWLastUpdatedNameFromDb().ShouldBeEqual(_clientSearch.GetLoggedInUserFullName(),
                        "Last Modified Name Should Be Updated In Database");
                    lastModifiedDateAfterUpload.ShouldNotBeTheSameAs(lastModifiedDateBeforeUpload,
                        "When saved, previous file should be deleted and new file should be added and last modified date should be updated");

                    //TE-670
                    StringFormatter.PrintMessageTitle("Verify Changes Are Saved In Client Settings Audit Table");
                    var auditDataForCIWUploadFromDb =
                        _clientSearch.GetClientSettingAuditFromDatabase(ClientEnum.SMTST.ToString(), 1);
                    var lastModifiedDate = Convert.ToDateTime(auditDataForCIWUploadFromDb[0][2])
                        .ToString("MM/dd/yyyy hh:mm tt").Trim();
                    var sysdate = DateTime.ParseExact(_clientSearch.GetCommonSql.GetSystemDateFromDB(),
                        "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    DateTime.ParseExact(lastModifiedDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture)
                        .AssertDateRange(sysdate.AddMinutes(-2),
                            sysdate.AddMinutes(1), "Last modified date should match");
                    auditDataListForCIWDocument[0] =
                        _clientSearch.GetCommonSql.GetUserSeqForCurrentlyLoggedInUser(automatedBase.EnvironmentManager.Username);

                    auditDataForCIWUploadFromDb[0].RemoveAt(2);
                    auditDataForCIWUploadFromDb[0]
                        .ShouldCollectionBeEqual(auditDataListForCIWDocument, "Audit data should match");

                    ValidateGeneralTabFormEnableDisabled(false, _clientSearch);
                    _clientSearch.GetSideWindow.GetValueByLabel("Last Modified by").ShouldBeEqual(
                        $"{_clientSearch.GetLoggedInUserFullName()} {DateTime.Now:MM/dd/yyyy}");

                    StringFormatter.PrintMessageTitle(
                        "Verify New File Is Available To Be Downloaded Using The CIW Icon. ");
                    var fileName = _clientSearch.ClickOnCIWIconAndGetFileName();
                    var actualfileName = Regex.Replace(fileName, @"\d", "").Trim();
                    actualfileName = actualfileName.Replace("()", "").Replace(" ", "");
                    actualfileName.ShouldBeEqual(validFileName, "Is Download Filename Correct?");
                    _clientSearch.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + fileName);
                    File.Exists(@"C:/Users/uiautomation/Downloads/" + fileName)
                        .ShouldBeTrue("CIW File should be downloaded");
                    _clientSearch.ClickOnBrowserBackButton();
                }
                finally
                {
                    _clientSearch.CloseAnyPopupIfExist();
                    ExcelReader.DeleteExcelFileIfAlreadyExists(validFileName);
                }
            }
        }

        [Test] //TE-708
        public void Verify_Processing_Type_Is_Unchanged_When_Modifying_Other_General_Settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;

                automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                var FileUploadPage = _clientSearch.GetFileUploadPage;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var ciwFileName = testData["ciwFileName"];

                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorkingAjaxMessage();
                _clientSearch.ClickOnGridRowByClientName(ClientEnum.CVTY.GetStringValue());

                StringFormatter.PrintMessage("Client Settings Before Uploading CIW File");
                _clientSearch.ClickOnGridRowByClientName(ClientEnum.SMTST.GetStringValue());
                var statusBefore = _clientSearch.IsActiveInactiveStatusRadioButtonClicked();
                var clientNameBefore = _clientSearch.GetSideWindow.GetInputFieldText("Client Name");
                var clientTypeBefore = _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Client Type");
                var processingTypeBefore = _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Processing Type");

                StringFormatter.PrintMessage("Upload CIW File");
                _clientSearch.GetSideWindow.ClickOnEditIcon();
                FileUploadPage.UploadCIWDocument(ciwFileName);
                _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                _clientSearch.RefreshPage();

                StringFormatter.PrintMessage("Verify Other Settings Are Not Changed");
                ValidateGeneralTabForm(statusBefore, clientNameBefore, clientTypeBefore, processingTypeBefore, _clientSearch);

                StringFormatter.PrintMessage("Verify Settings In Database");
                _clientSearch.GetClientGeneralSettingFromDatabase().ShouldCollectionBeEqual(
                    new List<string>
                        {(statusBefore) ? "T" : "F", clientNameBefore, clientTypeBefore, processingTypeBefore},
                    "Values Should Not Be Changed In Database");
            }
        }

        #endregion

        



        #region PRIVATE METHODS

        private void VerificationOfCancelButton(List<string> productStatusLabel, IList<bool> productsOnOrOffList,
           Dictionary<string, Tuple<bool, string>> MappingOfAppealSettingsLabelWithDefaultValue, string expectedAppealcc, ClientSearchPage _clientSearch)
        {
            #region Verification of Cancel button for product and Appeals Active

            for (int i = 0; i < productStatusLabel.Count; i++)
            {
                _clientSearch.ClickOnRadioButtonByLabel(productStatusLabel[i], !productsOnOrOffList[i]);
            }

            _clientSearch.GetSideWindow.Cancel();
            _clientSearch.GetSideWindow.ClickOnEditIcon();

            for (int i = 0; i < productStatusLabel.Count; i++)
            {
                _clientSearch.IsRadioButtonOnOffByLabel(productStatusLabel[i], productsOnOrOffList[i])
                    .ShouldBeTrue(
                        $"Is Radio button {(productsOnOrOffList[i] ? "ON" : "OFF")} for {productStatusLabel[i]}");
            }

            _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.AppealsActive.GetStringValue(),
                    MappingOfAppealSettingsLabelWithDefaultValue[ProductAppealsEnum.AppealsActive.GetStringValue()]
                        .Item1)
                .ShouldBeTrue($"Is Radio button " +
                              $"{(MappingOfAppealSettingsLabelWithDefaultValue[ProductAppealsEnum.AppealsActive.GetStringValue()].Item1 ? "ON" : "OFF")} for {ProductAppealsEnum.AppealsActive.GetStringValue()}");

            #endregion

            #region Verification of Cancel button for Display Ext Doc and Send Appeal Email

            StringFormatter.PrintMessageTitle(
                "Verification of Cancel button for Display Ext Doc and Send Appeal Email");

            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(),
                false);
            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(), false);

            _clientSearch.GetSideWindow.Cancel();
            _clientSearch.GetSideWindow.ClickOnEditIcon();

            _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(),
                    MappingOfAppealSettingsLabelWithDefaultValue[
                        ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()].Item1)
                .ShouldBeTrue(
                    $"Is Radio button {(MappingOfAppealSettingsLabelWithDefaultValue[ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()].Item1 ? "ON" : "OFF")} for {ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()}");
            _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(),
                    MappingOfAppealSettingsLabelWithDefaultValue[
                        ProductAppealsEnum.SendAppealEmail.GetStringValue()].Item1)
                .ShouldBeTrue(
                    $"Is Radio button {(MappingOfAppealSettingsLabelWithDefaultValue[ProductAppealsEnum.SendAppealEmail.GetStringValue()].Item1 ? "ON" : "OFF")} for {ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()}");

            #endregion

            #region Verification of Cancel button for remaing Appeals Settings and Appeal Due Date Calculation

            StringFormatter.PrintMessageTitle(
                "Verification of Cancel button for remaing Appeals Settings and Appeal Due Date Calculation");
            foreach (KeyValuePair<string, Tuple<bool, string>> appealSettingLabelWithExpectedSetting in
                MappingOfAppealSettingsLabelWithDefaultValue)
            {
                var label = appealSettingLabelWithExpectedSetting.Key;
                var expectedSetting = appealSettingLabelWithExpectedSetting.Value.Item1;

                if (label == ProductAppealsEnum.AppealsActive.GetStringValue() ||
                    label == ProductAppealsEnum.DisableRecordReviews.GetStringValue() ||
                    label == ProductAppealsEnum.DisplayExtDocIDField.GetStringValue() ||
                    label == ProductAppealsEnum.SendAppealEmail.GetStringValue() ||
                    label == ProductAppealsEnum.AppealEmailCC.GetStringValue())
                    continue;

                _clientSearch.ClickOnRadioButtonByLabel(label, !expectedSetting);
            }

            _clientSearch.SetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue(),
                "Test@test.com");

            var expectedAppealDueDatesInputList = _clientSearch.GetAppealDueDatesAllInputTextBox();

            StringFormatter.PrintMessageTitle("Changes of all input field of Appeal Due Date Calculation");
            _clientSearch.SetAppealDueDatesAllInputTextBox("99");

            _clientSearch.GetSideWindow.Cancel();
            _clientSearch.GetSideWindow.ClickOnEditIcon();

            foreach (KeyValuePair<string, Tuple<bool, string>> appealSettingLabelWithExpectedSetting in
                MappingOfAppealSettingsLabelWithDefaultValue)
            {
                var label = appealSettingLabelWithExpectedSetting.Key;
                var expectedSetting = appealSettingLabelWithExpectedSetting.Value.Item1;

                if (label == ProductAppealsEnum.AppealsActive.GetStringValue() ||
                    label == ProductAppealsEnum.AppealEmailCC.GetStringValue())
                    continue;

                _clientSearch.IsRadioButtonOnOffByLabel(label, expectedSetting)
                    .ShouldBeTrue($"Is Radio button {(expectedSetting ? "ON" : "OFF")} for {label}");
            }

            _clientSearch.GetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue())
                .ShouldBeEqual(expectedAppealcc, "Verification of Appeal Email CC");

            _clientSearch.GetAppealDueDatesAllInputTextBox().ShouldCollectionBeEqual(
                expectedAppealDueDatesInputList,
                "Does Input filed of Appeal Due Date Calculation retained value after cancellation?");

            StringFormatter.PrintMessage("Verifying Cancel for 'Disable Record Reviews'");
            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisableRecordReviews.GetStringValue());
            _clientSearch.GetSideWindow.Cancel();
            _clientSearch.GetSideWindow.ClickOnEditIcon();

            _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.DisableRecordReviews.GetStringValue(), false)
                .ShouldBeTrue("Changes made to Disable Record Reviews should not be saved when canceled");
            _clientSearch.IsRecordReviewDisabledInAppealDueDateCalculation(false)
                .ShouldBeTrue(
                    "Record Review fields should be enabled again when cancel is clicked after setting Disable Record Review to yes");

            #endregion
        }

        private void VerificationOfDataSavedInDB(string clientSettingsOption, List<string> productList, string client, ClientSearchPage _clientSearch)
        {
            switch (clientSettingsOption)
            {
                case "Product Status":
                    var allActiveProductsFromDB = _clientSearch.GetCommonSql.GetAllActiveProductsAbbrvForClient(client);
                    var allActiveProductsFromUI = _clientSearch.GetAllActiveProductsFromProductAppealsTab(client, productList);

                    allActiveProductsFromUI.ShouldCollectionBeEquivalent(allActiveProductsFromDB,
                        "Product ON/OFF Status should match in UI and DB");
                    break;

                case "Appeal Settings":
                    int count = 0;
                    List<string> appealSettingLabels = new List<string>();

                    foreach (ProductAppealsEnum appealSettingsLabel in Enum.GetValues(typeof(ProductAppealsEnum)))
                    {
                        appealSettingLabels.Add(appealSettingsLabel.GetStringValue());
                        count++;
                    }

                    var appealSettingsValuesFromDB = _clientSearch.GetCommonSql
                        .GetAppealSettingsStatusByLabelInDB(appealSettingLabels, client);

                    foreach (KeyValuePair<string, string> appealSettingWithBoolValue in appealSettingsValuesFromDB)
                    {
                        var label = appealSettingWithBoolValue.Key;
                        var valueFromDB = appealSettingWithBoolValue.Value;

                        if (bool.TryParse(valueFromDB, out bool isBoolValue) && isBoolValue && label != ProductAppealsEnum.AppealEmailCC.GetStringValue())
                            _clientSearch.IsRadioButtonOnOffByLabel(label)
                                .ShouldBeTrue($"Appeal Settings value for {label} is ON and is saved correctly in DB");

                        else
                        {
                            if (label != ProductAppealsEnum.AppealEmailCC.GetStringValue())
                                _clientSearch.IsRadioButtonOnOffByLabel(label)
                                    .ShouldBeFalse($"Appeal Settings value for {label} is OFF and is saved correctly in DB");
                            else
                                _clientSearch.GetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue())
                                    .ShouldBeEqual(valueFromDB, "Appeal Email CC is correctly saved in DB");
                        }
                    }
                    break;

                case "Appeal Due Date Calculation":
                    var activeProducts = _clientSearch.GetAllActiveProductsFromProductAppealsTab(ClientEnum.RPE.ToString(), productList);
                    var appealDueDatesForActiveProductsFromDB =
                        _clientSearch.GetCommonSql.GetAppealDueDatesForProductsFromDB(activeProducts,
                            ClientEnum.RPE.ToString());

                    foreach (KeyValuePair<string, List<string>> productWithDueDate in appealDueDatesForActiveProductsFromDB)
                    {
                        var product = productWithDueDate.Key;
                        var dueDateValuesForProduct = productWithDueDate.Value;

                        _clientSearch.GetAppealDueDatesInputFieldsByLabel(product, false)
                            .ShouldCollectionBeEqual(dueDateValuesForProduct, "Appeal Due Date values are stored correctly in the DB");
                    }

                    break;
            }
        }

        private void VerifyDependencyBehaviorOfAppealSettings(Dictionary<string, Tuple<bool, string>> MappingOfAppealSettingsLabelWithDefaultValue, 
            ClientSearchPage _clientSearch)
        {
            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisplayExtDocIDField.GetStringValue(),
                false);
            _clientSearch.IsDivByLabelPresent(ProductAppealsEnum.CotivitiUploadsAppealDocuments.GetStringValue())
                .ShouldBeFalse(
                    $"Is {ProductAppealsEnum.CotivitiUploadsAppealDocuments.GetStringValue()} Appeal Settings Display?");
            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(), false);
            _clientSearch.IsDivByLabelPresent(ProductAppealsEnum.AppealEmailCC.GetStringValue())
                .ShouldBeFalse($"Is {ProductAppealsEnum.AppealEmailCC.GetStringValue()} Appeal Settings Display?");

            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealsActive.GetStringValue(), false);

            foreach (KeyValuePair<string, Tuple<bool, string>> appealSettingLabelWithExpectedSetting in
                MappingOfAppealSettingsLabelWithDefaultValue)
            {
                var label = appealSettingLabelWithExpectedSetting.Key;

                if (label == ProductAppealsEnum.AppealsActive.GetStringValue()) continue;

                _clientSearch.IsDivByLabelPresent(label).ShouldBeFalse($"Is {label} Appeal Settings Display?");
            }

            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.AppealsActive.GetStringValue());

            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.DisableRecordReviews.GetStringValue());
            _clientSearch.GetAppealDueDatesAllInputTextBox().Where((x, j) => j % 4 == 0)
                .All(y => y == "NA")
                .ShouldBeTrue(
                    $"Setting the '{ProductAppealsEnum.DisableRecordReviews.GetStringValue()}' to true will set all the Record Review values to NA");

            _clientSearch.IsRecordReviewDisabledInAppealDueDateCalculation(true)
                .ShouldBeTrue
                    ($"Setting the '{ProductAppealsEnum.DisableRecordReviews.GetStringValue()}' to true will disable the Record Review section in Appeal Due Date Calculation");

            _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.EnableMedicalRecordsReviews.GetStringValue(),false);
            _clientSearch.GetAppealDueDatesAllInputTextBox().Where((x, j) => (j-1) % 4 == 0).Distinct()
                .ShouldCollectionBeEquivalent(new List<string>(){"0","NA"},
                    $"Setting the '{ProductAppealsEnum.EnableMedicalRecordsReviews.GetStringValue()}' to false will set all the Medical Record Review values to either 0 or NA");
            _clientSearch.IsMedicalRecordReviewDisabledInAppealDueDateCalculation(true)
                .ShouldBeTrue
                    ($"Setting the '{ProductAppealsEnum.EnableMedicalRecordsReviews.GetStringValue()}' to false will disable the Medical Record Review section in Appeal Due Date Calculation");
        }

        private void VerificationOfAppealSettingsOnOffAndHelpIconValue(List<string> appealsSettingsIconValueList,
            out string expectedAppealcc, out Dictionary<string, Tuple<bool, string>> MappingOfAppealSettingsLabelWithDefaultValue, ClientSearchPage _clientSearch)
        {
            var expectedAppealSettings =
                _clientSearch.GetCommonSql.GetAppealSettingsBoolValueFromDatabase(ClientEnum.SMTST.ToString());
            expectedAppealcc = _clientSearch.GetCommonSql.GetAppealSettingsFromDatabase(ClientEnum.SMTST.ToString())[9];

            MappingOfAppealSettingsLabelWithDefaultValue = new Dictionary<string, Tuple<bool, string>>();

            // Verifying whether the Appeal Settings are in sync with our test data. If not, we need to check if some settings are added/removed
            Enum.GetValues(typeof(ProductAppealsEnum)).Length.ShouldBeEqual(expectedAppealSettings.Count,
                "Appeal Settings and its respective values count in DB should match. " +
                "If the counts are not equal, then some Appeal settings label might be missing/extra.");

            // Storing the Appeal Setting label as key and its ON/OFF boolean value and Help Icon message in a Dictionary
            int appealSettingCount = 0;
            foreach (ProductAppealsEnum appealSettingsLabel in Enum.GetValues(typeof(ProductAppealsEnum)))
            {
                MappingOfAppealSettingsLabelWithDefaultValue.Add(appealSettingsLabel.GetStringValue(),
                    Tuple.Create(expectedAppealSettings[appealSettingCount],
                        appealsSettingsIconValueList[appealSettingCount]));
                appealSettingCount++;
            }

            StringFormatter.PrintMessage("Verification of ON/OFF values in Appeal Setting fields and their Help Icon messages");

            foreach (KeyValuePair<string, Tuple<bool, string>> appealSettingLabelWithExpectedSetting in
                MappingOfAppealSettingsLabelWithDefaultValue)
            {
                var label = appealSettingLabelWithExpectedSetting.Key;
                var settingTrueOrFalse = appealSettingLabelWithExpectedSetting.Value.Item1;
                var helpIconMsg = appealSettingLabelWithExpectedSetting.Value.Item2;

                if (appealSettingLabelWithExpectedSetting.Key != ProductAppealsEnum.AppealEmailCC.GetStringValue())
                {
                    _clientSearch.IsRadioButtonOnOffByLabel(label, settingTrueOrFalse)
                        .ShouldBeTrue($"Is Radio button {(settingTrueOrFalse ? "ON" : "OFF")} for {label}");
                }

                _clientSearch.GetInfoHelpTooltipByLabel(label)
                    .ShouldBeEqual(helpIconMsg, $"Is tooltip of help icon of {label} equals?");
            }

            StringFormatter.PrintMessageTitle("Verification of Appeal Email CC");
            _clientSearch.GetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue())
                .ShouldBeEqual(expectedAppealcc, "Verification of Appeal Email CC");

            _clientSearch.ClearTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue());
            _clientSearch.GetSideWindow.Save();
            _clientSearch.GetPageErrorMessage()
                .ShouldBeEqual("Valid email addresses separated by a semi-colon are required.",
                    "Verify Popup message for empty email");
            _clientSearch.ClosePageError();

            _clientSearch.SetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue(), "Test.com");
            _clientSearch.GetSideWindow.Save();
            _clientSearch.GetPageErrorMessage()
                .ShouldBeEqual("Valid email addresses separated by a semi-colon are required.",
                    "Verify Popup message for invalid email");
            _clientSearch.ClosePageError();
            _clientSearch.SetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue(),
                "Test@test.com;");
            _clientSearch.GetSideWindow.Save();
            _clientSearch.GetPageErrorMessage()
                .ShouldBeEqual("Valid email addresses separated by a semi-colon are required.",
                    "Verify Popup message for invalid email");
            _clientSearch.ClosePageError();

            StringFormatter.PrintMessageTitle("Verification of max input value in Appeal Due Date Fields");
            for (var appealTypeColumn = 1; appealTypeColumn < 5; appealTypeColumn++)
            {
                Dictionary<string, int> mapLabelWithColumnNo = new Dictionary<string, int>()
                {
                    ["Record Review"] = 1,
                    ["Med Record Review"] = 2,
                    ["Urgent"] = 3,
                    ["Appeal"] = 4
                };

                string appealType = "";

                switch (appealTypeColumn)
                {
                    case 1:
                        appealType = mapLabelWithColumnNo.Keys.ToList()[0];
                        break;
                    case 2:
                        appealType = mapLabelWithColumnNo.Keys.ToList()[1];
                        _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.EnableMedicalRecordsReviews
                            .GetStringValue());
                        break;
                    case 3:
                        appealType = mapLabelWithColumnNo.Keys.ToList()[2];
                        break;
                    case 4:
                        appealType = mapLabelWithColumnNo.Keys.ToList()[3];
                        break;
                }

                //Greater than 99 validation
                _clientSearch.SetAppealDueDatesInputByLabelAndColumn(ProductEnum.CV.ToString(),
                    mapLabelWithColumnNo[appealType], "100");

                if (appealType != "Record Review" && appealType != "Med Record Review")
                {
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be greater than 99.",
                        $"Validate {appealType} for greater than 99.");
                    _clientSearch.ClosePageError();
                    _clientSearch.SetAppealDueDatesInputByLabelAndColumn(ProductEnum.CV.ToString(),
                        mapLabelWithColumnNo[appealType], "a");
                    _clientSearch.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                        $"Validate {appealType} for non integer value");
                    _clientSearch.ClosePageError();
                }

                else
                {
                    #region CAR-2999 + CAR-3066

                    _clientSearch
                        .GetAppealDueDatesInputByLabelAndColumn(ProductEnum.CV.ToString(), appealTypeColumn)
                        .ShouldBeEqual("10", $"More than 2 digits should not be allowed in {appealType}");
                    #endregion
                }
            }
        }

        private void VerifyInputField(string label, string value, string validation_message, ClientSearchPage _clientSearch)
        {
            _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(label, value);
            _clientSearch.GetPageErrorMessage().ShouldBeEqual(validation_message,
                $"Verify validation Message when entering value greater than 999 in {label} Input Field");
            _clientSearch.ClosePageError();
        }

        private void VerifyAppealDueDateCalculationSettings(ClientSearchPage _clientSearch)
        {
            List<string> calculationType = new List<string> { "Business", "Calendar" };
            List<string> calendarExcludeHolidaysType = new List<string> {"Cotiviti","Client"};

            StringFormatter.PrintMessage("Verify Only One of the options can be checked at a time");
            _clientSearch.IsAppealDueDateCheckBoxChecked(calculationType[0]).ShouldBeTrue("Business should be selected by default.");
            _clientSearch.IsAppealDueDateCheckBoxChecked(calculationType[1]).ShouldBeFalse("Only one option should be selected at a time.");

            _clientSearch.GetAppealDueDateSettingsTooltip().ShouldBeEqual(
                "Defines the max # of days in which an appeal can be completed based on the create date and the client appeal SLA.");
            var count = _clientSearch.GetAppealDueDateProductRowCount();

            foreach (var type in calculationType)
            {
                StringFormatter.PrintMessage($"Verify {type} present?");
                _clientSearch.IsAppealDueDateCheckBoxPresent(type).ShouldBeTrue($"appeal radio button for {type} present");
                if (type == "Business")
                {
                    _clientSearch.GetAppealDueDateSettingsTypeTooltip(type).ShouldBeEqual
                    ("Calculation will skip weekends and Cotiviti holidays.");
                }
                else
                {
                    #region CAR-2998(CAR-2925)
                    _clientSearch.SelectCalculationType(type);
                    _clientSearch.IsExcludeHolidaysLabelPresent().ShouldBeTrue($"Exclude Holidays label present on clicking {type} option");
                    _clientSearch.GetExcludeHolidaysOptions().ShouldCollectionBeEqual(calendarExcludeHolidaysType, "Options should match");
                    _clientSearch.IsExcludeHolidaysOptionCheckBoxChecked(calendarExcludeHolidaysType[0]).ShouldBeFalse($"By default {calendarExcludeHolidaysType[0]} option should not be selected");
                    _clientSearch.IsExcludeHolidaysOptionCheckBoxChecked(calendarExcludeHolidaysType[1]).ShouldBeFalse($"By default {calendarExcludeHolidaysType[1]} option should not be selected");
                    _clientSearch.GetAppealDueDateSettingsTypeTooltip(type).ShouldBeEqual("Calculation will not skip weekends or holidays.");
                    _clientSearch.SelectCalculationType("Business");
                    #endregion
                }
            }
        }
        
        private void ValidateAutomatedBatchReleaseFormFields(List<string> returnFileFrequencyValues, ClientSearchPage _clientSearch)
        {
            StringFormatter.PrintMessage($"Verification of the '{WorkflowTabEnum.BeginClaimRelease.GetStringValue()} option'");

            _clientSearch.GetSideWindow.GetDropDownList(WorkflowTabEnum.BeginClaimRelease.GetStringValue())
                .ShouldCollectionBeEqual(timeOptionsList, $"The time picker field {WorkflowTabEnum.BeginClaimRelease.GetStringValue()} should show the correct time options");

            StringFormatter.PrintMessage($"Verification of the '{WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()}' option");

            _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(), "0");
            _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error should popup if value entered is not within the valid range");
            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be less than 1.",
                $"Correct error message should be populated when 0 is entered in {WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()}");
            _clientSearch.ClosePageError();

            _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(), "1000");
            _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error should popup if value entered not within the valid range");
            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Value cannot be greater than 999.",
                $"Correct error message should be populated when 1000 is entered in {WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()}");
            _clientSearch.ClosePageError();

            _clientSearch.SetInputTextBoxValueByLabelInWorkflowInteropTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue(), "1abc");
            _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error should popup if non-numeric value is entered");
            _clientSearch.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                $"Correct error message should be populated when non-numeric value is entered in {WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()}");
            _clientSearch.ClosePageError();

            StringFormatter.PrintMessage($"Verification of the '{WorkflowTabEnum.ReturnFileFrequency.GetStringValue()}' option");

            _clientSearch.GetSideWindow.GetDropDownList(WorkflowTabEnum.ReturnFileFrequency.GetStringValue())
                .ShouldCollectionBeEqual(returnFileFrequencyValues, $"Dropdown values are correct for ${WorkflowTabEnum.ReturnFileFrequency.GetStringValue()}");

            StringFormatter.PrintMessage($"Verification of the '{WorkflowTabEnum.FailureAlert.GetStringValue()}' option");
            _clientSearch.GetSideWindow.GetDropDownList(WorkflowTabEnum.FailureAlert.GetStringValue())
                .ShouldCollectionBeEqual(timeOptionsList, $"The time picker field {WorkflowTabEnum.FailureAlert.GetStringValue()} should show the correct time options");
        }

        private void VerifyDataInWorkflowTab(List<string> originalValues, List<string> productList, ClientSearchPage _clientSearch)
        {
            _clientSearch.ClickOnRadioButtonByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue());
            var actualValues = _clientSearch.GetListOfAllDataInWorkflowTab(productList);

            actualValues.ShouldCollectionBeEqual(originalValues, "Data should not be saved in the form when Cancel is clicked");
        }

        private void SelectRandomCustomFieldsFromAvailableDropDownList(ClientSearchPage _clientSearch)
        {
            var list = _clientSearch.GetSideWindow.GetDropDownList("Available Fields", true);
            var r = new Random();
            var customFieldIndex = r.Next(0, list.Count - 1);
            _clientSearch.GetSideWindow.SelectDropDownValue("Available Fields",list[customFieldIndex]);

        }

        private void SelectCustomLabelFromAvailableFields(int maxValue, ClientSearchPage _clientSearch)
        {
            
            while (_clientSearch.GetAvailableListCount() < maxValue)
            {
                SelectRandomCustomFieldsFromAvailableDropDownList(_clientSearch);
            }

        }

        private void VerifyOrderControlArrowIcons(ClientSearchPage _clientSearch)
        {
            var customList = _clientSearch.GetCustomFieldList();
            var initialValue = customList[0];
            _clientSearch.GetSideWindow.ClickOnEditIcon();
            _clientSearch.ClickOnMoveDownCaretIcon();
            var newcustomList = _clientSearch.GetCustomFieldList();
            newcustomList[1].ShouldBeEqual(initialValue,"Verify Up Arrow functionality");
            _clientSearch.ClickOnMoveUpCaretIcon(2);
            customList = _clientSearch.GetCustomFieldList();
            customList[0].ShouldBeEqual(initialValue, "Verify Down Arrow functionality");
        }
        private void VerifyChangesAreNotSavedInConfigurationSettingsTab(List<string> configurationOptions, List<string> originalConfigSettings, ClientSearchPage _clientSearch)
        {
            for (int optionIndex = 0; optionIndex < configurationOptions.Count; optionIndex++)
            {
                var optionLabel = configurationOptions[optionIndex];

                if (_clientSearch.IsRadioButtonPresentByLabel(optionLabel))
                {
                    var radioButtonValueFromForm = (_clientSearch.IsRadioButtonOnOffByLabel(optionLabel)) ? "T" : "F";
                    radioButtonValueFromForm.ShouldBeEqual(originalConfigSettings[optionIndex],
                        $"'{optionLabel}' should have the original value when the data is not saved");
                }

                else if (optionLabel == "Allow Switch of non-Reverse Flags")
                {
                    switch (_clientSearch.GetInputTextBoxValueByLabel(optionLabel))
                    {
                        case "NO":
                            "D".ShouldBeEqual(originalConfigSettings[optionIndex]);
                            break;

                        case "YES":
                            "A".ShouldBeEqual(originalConfigSettings[optionIndex]);
                            break;

                        case "YES - only to client unreviewed claims":
                            "C".ShouldBeEqual(originalConfigSettings[optionIndex]);
                            break;
                    }
                }

                else
                {
                    _clientSearch.GetInputTextBoxValueByLabel(optionLabel)
                        .ShouldBeEqual(originalConfigSettings[optionIndex]);
                }
            }
        }

        private void ValidateDefaultFilterValue(ClientSearchPage _clientSearch)
        {
            _clientSearch.GetdefaultValueInFilterList("Client Status").ShouldBeEqual("Active");
            _clientSearch.GetdefaultValueInFilterList("Client Code").ShouldBeEqual("All");
            _clientSearch.GetdefaultValueInFilterList("Client Name").ShouldBeNullorEmpty("client name should not have a default value");
            _clientSearch.GetdefaultValueInFilterList("Product").ShouldBeEqual("All");
        }
        
        private void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, ClientSearchPage _clientSearch, bool order = true)
        {
            var actualDropDownList = _clientSearch.SideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                
            }
            if(label=="Client Status")
               _clientSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("Active", label + " value defaults to Active");
            else
                _clientSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");

            _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _clientSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
            if (label == "Client Status")
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, "Active");
            else
                _clientSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");

        }

        private void VerifySortingOptionsInClientSearch(int col, int option, string sortoption, ClientSearchPage _clientSearch)
        {
            StringFormatter.PrintMessageTitle("Validation of Sorting by Client");
            _clientSearch.ClickOnFilterOptionListRow(option);
            _clientSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                .ShouldBeTrue("Search result must be sorted by" + sortoption + " in Ascending");
            _clientSearch.ClickOnFilterOptionListRow(option);
            _clientSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                .ShouldBeTrue("Search result must be sorted by" + sortoption + " in Descending");

        }

        private void FillGeneralTabForm(bool status, string clientName, string clientType, string processingType, ClientSearchPage _clientSearch)
        {
            _clientSearch.ClickOnStatusRadioButton(status);
            _clientSearch.GetSideWindow.FillInputBox("Client Name", clientName);
            _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Client Type", clientType);
            _clientSearch.GetSideWindow.SelectDropDownListValueByLabel("Processing Type",
                processingType);
        }
        private void ValidateGeneralTabForm(bool status, string clientName, string clientType, string processingType, ClientSearchPage _clientSearch)
        {
            _clientSearch.IsActiveInactiveStatusRadioButtonClicked(status)
                .ShouldBeTrue($"{(status ? "Active" : "Inactive")} Radio button should selected");
            _clientSearch.GetSideWindow.GetInputFieldText("Client Name").ShouldBeEqual(clientName, "Client Name should be Equal");
            _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Client Type")
                .ShouldBeEqual(clientType, "Verification of Client Type dropdown selected list");
            _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel("Processing Type")
                .ShouldBeEqual(processingType, "Verification of Processing Type dropdown selected list");
        }
        private void HandleErrorPopUp(string expectedErrorMessage, string message, ClientSearchPage _clientSearch)
        {
            _clientSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Is error pop up present?");
            _clientSearch.GetPageErrorMessage()
                .ShouldBeEqual(expectedErrorMessage, message);
            _clientSearch.ClosePageError();
        }

        private void ValidateGeneralTabFormEnableDisabled(bool enable, ClientSearchPage _clientSearch)
        {
            if (enable)
            {
                _clientSearch.IsActiveInactiveStatusRadioButtonDisabled()
                    .ShouldBeFalse("Active Radio Button should enabled");
                _clientSearch.IsActiveInactiveStatusRadioButtonDisabled(false)
                    .ShouldBeFalse("Inactive Radio Button should enabled");
                _clientSearch.GetSideWindow.IsTextBoxDisabled("Client Name")
                    .ShouldBeFalse("Client Name should enabled");
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Client Type")
                    .ShouldBeFalse("Client Type should enabled");
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Processing Type")
                    .ShouldBeFalse("Processing Type should enabled");
            }
            else
            {
                _clientSearch.IsActiveInactiveStatusRadioButtonDisabled()
                    .ShouldBeTrue("Active Radio Button should disabled");
                _clientSearch.IsActiveInactiveStatusRadioButtonDisabled(false)
                    .ShouldBeTrue("Inactive Radio Button should disabled");
                _clientSearch.GetSideWindow.IsTextBoxDisabled("Client Name")
                    .ShouldBeTrue("Client Name should disabled");
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Client Type")
                    .ShouldBeTrue("Client Type should disabled");
                _clientSearch.GetSideWindow.IsDropDownBoxDisabled("Processing Type")
                    .ShouldBeTrue("Processing Type should disabled");
            }
        }

        #endregion

    }
}
