using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UIAutomation.Framework.Core.Driver;
using Org.BouncyCastle.Crypto.Tls;
using UIAutomation.Framework.Utils;
using static System.String;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class EditSettingsManager
    {
        
        #region PRIVATE FIELDS

            //private EditSettingsManagerPage _editSettingsManager;
            //private ClientSearchPage _clientSearchPage;
            //private CommonValidations _commonValidation;
            //private readonly string _productMaintenancePrivileges = RoleEnum.ProductAdmin.GetStringValue();
            // private List<string> _activeProductListForClientDB;
            //private SideBarPanelSearch _sideBarPanelSearch ;
            //Dictionary<string, string> editsWithRespectivePrimaryEditor = new Dictionary<string, string>
            //{
            //    ["M25R"] = "M25R / R25R",
            //    ["NCD"] = "NCD / NCDN",
            //    ["REBD"] = "REBD / RBPD",
            //    ["UPR"] = "UPR / RUPR",
            //    ["ASM"] = "ASM / RASM",
            //    ["BIL"] = "BIL / MAX",
            //    ["FUD"] = "FUD / GPA",
            //    ["MPR"] = "MPR / PS",
            //    ["NPT"] = "NPT / NPR",
            //    ["REB"] = "REB / RBP"
            //};

            //Dictionary<string, string> listOfAddFlags = new Dictionary<string, string>()
            //{
            //    ["FOT ADD"] = "R", // R for reporting
            //    ["UNB ADD"] = "T"
            //};

            #endregion

            #region DBInteraction Method

            //private void RetrieveListFromDatabase()
            //{

            //    _activeProductListForClientDB = _editSettingsManager.GetActiveProductListForClientDB();
            //    _activeProductListForClientDB.Insert(0, "All");

            //}


            #endregion

            //#region OVERRIDE METHOD

            //protected override void ClassInit()
            //{
            //    try
            //    {
            //        base.ClassInit();
            //        _editSettingsManager = QuickLaunch.NavigateToEditSettingsManager();
            //        _commonValidation = new CommonValidations(CurrentPage);
            //        RetrieveListFromDatabase();

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
            //    base.TestCleanUp();
            //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            //    {
            //        _editSettingsManager = _editSettingsManager.Logout().LoginAsHciAdminUser().NavigateToEditSettingsManager();
            //    }
            //    else if (CurrentPage.GetPageHeader() != PageHeaderEnum.EditSettingsManager.GetStringValue())
            //    {
            //        _editSettingsManager = CurrentPage.NavigateToEditSettingsManager();
            //    }
            //    _editSettingsManager.GetSideBarPanelSearch.OpenSidebarPanel();
            //    _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
            //    _editSettingsManager.GetSideBarPanelSearch.OpenSidebarPanel();
            //    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status", EditStatusEnum.On.GetStringValue());
            //}

            //#endregion

            #region PROTECTED PROPERTIES

            protected string FullyQualifiedClassName
            {
                get { return GetType().FullName; }
            }

        #endregion

        #region TESTSUITES

        [Test]//CAR-3295(CAR-3287)
        [Author("SurajR")]
        public void Verify_and_validation_of_MEUK_edit()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                EditSettingsManagerPage _editSettingsManager =
                    automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                const string edit = "MEUK";
                var label = paramLists["Label"]; 
                var tooltipMessage = paramLists["TooltipMessage"];
                var defaultValue = paramLists["DefaultValue"];
                var random = new Random();


                _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", "All");
                _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                    EditStatusEnum.All.GetStringValue());
                _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit", edit);
                _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                _editSettingsManager.WaitForWorkingAjaxMessage();
                try
                {

                    _editSettingsManager.ClickOnEditIconByFlag(edit);
                    StringFormatter.PrintMessage($"Verify Tooltip Icon For {label} of {edit} edit");
                    _editSettingsManager.GetInfoIconByEditAndInputLabel(edit, label)
                        .ShouldBeEqual(tooltipMessage,
                            $"Is Tooltip of <{label}> of <{edit}> Equal?");


                    StringFormatter.PrintMessage($"Verify Default Value for {edit} edit");
                    _editSettingsManager.GetCOBInputByEditAndInputLabel(edit, label)
                        .ShouldBeEqual(defaultValue, "Is Default Value displayed?");

                    StringFormatter.PrintMessage($"Verify Maximum Allowed Values For {label} of {edit} edit");
                    _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label, "1000000000");
                    _editSettingsManager.GetPageErrorMessage()
                        .ShouldBeEqual("Value cannot be greater than 99999999.",
                            "Is Popup Message Equals?");
                    _editSettingsManager.ClosePageError();

                    StringFormatter.PrintMessage($"Verify Text Field Allows Only Numeric For {label} of {edit} edit");
                    _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                        "a!.-+=");
                    _editSettingsManager.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                        "Is Popup Message Equals for non numeric character?");
                    _editSettingsManager.ClosePageError();

                    StringFormatter.PrintMessage($"Verify Values Are Saved For {label} of {edit} edit");
                    var value = random.Next(0, 99999999).ToString();
                    _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                        value);
                    _editSettingsManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Popup Message Should not display for valid data");
                    _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                    _editSettingsManager.ClickOnEditIconByFlag(edit);
                    _editSettingsManager.GetCOBInputByEditAndInputLabel(edit, label)
                        .ShouldBeEqual(value, "Is Saved Value displayed?");
                    _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label, defaultValue);
                    _editSettingsManager.ClickSaveInModifyEditSettingsForm();

                }
                finally
                {
                    if (_editSettingsManager.IsPageErrorPopupModalPresent())
                        _editSettingsManager.ClosePageError();
                }




            }
        }




        [Test] //CAR-2955(CAR-2872),CAR-2954(CAR-2926),CAR-3294(CAR-3231),CAR-3293(CAR-3230)+CAR-3292(CAR-3229),CAR-3291(CAR-3228)
        [NonParallelizable]
        [Author("SurajR + PujanA")]
        public void Verify_and_validation_of_COB_edit_member_look_back_and_min_paid()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                EditSettingsManagerPage _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var cobEditList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "cob_edit_list").Values
                    .ToList();
                var random = new Random();
                var labelList = paramLists["LabelList"].Split(',').ToList();
                var defaultValueList = paramLists["DefaultValueList"].Split(',').ToList();

                _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                    ProductEnum.COB.ToString());
                _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                    EditStatusEnum.All.GetStringValue());
                _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                _editSettingsManager.WaitForWorkingAjaxMessage();
                try
                {
                    foreach (var edit in cobEditList)
                    {
                        var i = 0;
                        var toolTipList = _editSettingsManager.GetTooltipLabelsForASpecificEdit(edit);
                        labelList[1] = (edit == "CMPU" || edit == "CMPK") ? "Member Look-Back Commercial" : "Member Look-Back Medicare";

                        _editSettingsManager.ClickOnEditIconByFlag(edit);
                        _editSettingsManager.GetAllLabelListFromUI(edit).ShouldCollectionBeEqual(labelList, "Order of settings in the UI should be in the order of Claim, Member and Batch");

                        foreach (var label in labelList)
                        {
                            _editSettingsManager.ClickOnEditIconByFlag(edit);
                            StringFormatter.PrintMessage($"Verify Tooltip Icon For {label} of {edit} edit");
                            _editSettingsManager.GetInfoIconByEditAndInputLabel(edit, label)
                                .ShouldBeEqual(toolTipList[i],
                                    $"Is Tooltip of <{label}> of <{edit}> Equal?");

                            StringFormatter.PrintMessage($"Verify Default Value For {label} of {edit} edit");
                            _editSettingsManager.GetCOBInputByEditAndInputLabel(edit, label)
                                .ShouldBeEqual((label == "Member Look-Back Commercial") ? "24" : defaultValueList[i], "Is Default Value displayed?");

                            StringFormatter.PrintMessage($"Verify Maximum Allowed Values For {label} of {edit} edit");
                            _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                                (label == "Member Look-Back Commercial"||label== "Member Look-Back Medicare") ? "100" : "1000000000");
                            _editSettingsManager.GetPageErrorMessage()
                                .ShouldBeEqual((label == "Member Look-Back Commercial" || label == "Member Look-Back Medicare") 
                                        ? "Value cannot be greater than 99." 
                                        : "Value cannot be greater than 999999999.", 
                                    "Is Popup Message Equals?");
                            _editSettingsManager.ClosePageError();

                            StringFormatter.PrintMessage($"Verify Text Field Allows Only Numeric For {label} of {edit} edit");
                            _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                                "a!.-+=");
                            _editSettingsManager.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.",
                                "Is Popup Message Equals for non numeric character?");
                            _editSettingsManager.ClosePageError();

                            StringFormatter.PrintMessage($"Verify Values Are Saved For {label} of {edit} edit");
                            var value = (label == "Member Look-Back Commercial" || label == "Member Look-Back Medicare") ? random.Next(0, 99).ToString() : random.Next(0, 999999999).ToString();
                            _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                                value);
                            _editSettingsManager.IsPageErrorPopupModalPresent()
                                .ShouldBeFalse("Popup Message Should not display for valid data");
                            _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                            _editSettingsManager.ClickOnEditIconByFlag(edit);
                            _editSettingsManager.GetCOBInputByEditAndInputLabel(edit, label)
                                .ShouldBeEqual(value, "Is Saved Value displayed?");
                            _editSettingsManager.SetCOBInputByEditAndInputLabel(edit, label,
                                (label == "Member Look-Back Commercial") ? "24" : defaultValueList[i]);
                            _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                            i++;
                        }
                    }
                }

                finally
                {
                    if (_editSettingsManager.IsPageErrorPopupModalPresent())
                        _editSettingsManager.ClosePageError();
                }

               

                
            }

        }

        [Test] //CAR-2834[CAR-2665]
            public void Verify_FOT_ADD_and_UNB_ADD_Edits()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    IDictionary<string, string> paramLists =
                        automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                    var edits = paramLists["Edits"].Split(',').ToList();
                    var reportingSettings = _editSettingsManager.GetReportingSettingsFromDB();
                    const string label = "Edit";
                    foreach (var edit in edits)
                    {
                        try
                        {
                            StringFormatter.PrintMessage(
                                $"Verification that {edit} is included in the list and searchable");
                            _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList(label)
                                .ShouldContain(edit, $"{edit} should be included in the list");
                            SearchByEdit(edit);

                            StringFormatter.PrintMessage($"Verification that {edit} is editable");
                            var statusValue = _editSettingsManager.GetStatusValueForFirstEditInGridFromDB();
                            _editSettingsManager.GetGridViewSection().ClickOnEditIcon();
                            _editSettingsManager.GetSideWindow().GetHeaderText()
                                .ShouldBeEqual("Modify Edit Settings", "Is Edit Form Header Text Equals?");

                            StringFormatter.PrintMessageTitle(
                                "Verify Radio buttons and Review statuses are in desired selection state");
                            _editSettingsManager.IsStatusRadioButtonPresent(1)
                                .ShouldBeTrue("On radio button should be present");
                            _editSettingsManager.IsStatusRadioButtonPresent(2)
                                .ShouldBeTrue("Off radio button should be present");
                            _editSettingsManager.IsStatusRadioButtonPresent()
                                .ShouldBeTrue("Reporting radio button should be present");
                            VerifyStatusRadioButton(new List<string> {reportingSettings, statusValue});
                            _editSettingsManager.GetReviewStatusLabel()
                                .ShouldBeEqual(EditReviewStatus.AutoApprove.GetStringValue(),
                                    $"{EditReviewStatus.AutoApprove.GetStringValue()} review status should be present");
                            _editSettingsManager.GetReviewStatusLabel(2)
                                .ShouldBeEqual(EditReviewStatus.ClientReview.GetStringValue(),
                                    $"{EditReviewStatus.ClientReview.GetStringValue()} review status should be present");
                            _editSettingsManager.GetReviewStatusLabel(3)
                                .ShouldBeEqual(EditReviewStatus.InternalReview.GetStringValue(),
                                    $"{EditReviewStatus.InternalReview.GetStringValue()} review status should be present");
                            _editSettingsManager.IsReviewStatusCheckBoxChecked()
                                .ShouldBeFalse("Is Auto approve check box checked?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled()
                                .ShouldBeFalse("Is Auto approve checkbox disabled?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                                .ShouldBeTrue("Is Client Review checkbox disabled?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                                .ShouldBeTrue("Is Internal Review checkbox disabled?");

                            StringFormatter.PrintMessage($"Verify Cancel functionality");
                            _editSettingsManager.ClickOnReviewStatusCheckBox();
                            ClickReviewStatusCheckBoxAndValidate();
                            _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                            _editSettingsManager.GetGridViewSection().ClickOnEditIcon();
                            StringFormatter.PrintMessage("Changes should not be saved on clicking Cancel Button");
                            ClickReviewStatusCheckBoxAndValidate(false);

                            StringFormatter.PrintMessage("Verify save functionality");
                            _editSettingsManager.ClickOnReviewStatusCheckBox();
                            _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                            _editSettingsManager.GetGridViewSection().ClickOnEditIcon();
                            ClickReviewStatusCheckBoxAndValidate();

                            StringFormatter.PrintMessage("Resetting to previous state");
                            _editSettingsManager.ClickOnReviewStatusCheckBox();
                            _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        }

                        finally
                        {
                            _editSettingsManager.GetGridViewSection().ClickOnEditIcon();
                            if (_editSettingsManager.IsReviewStatusCheckBoxChecked())
                            {
                                _editSettingsManager.ClickOnReviewStatusCheckBox();
                                _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                            }

                        }

                        void VerifyStatusRadioButton(List<string> check)
                        {
                            if (check[0] == "T")
                            {
                                _editSettingsManager.IsStatusRadioButtonPresent()
                                    .ShouldBeTrue(
                                        $"Is {EditStatusEnum.Reporting.GetStringValue()} Radio button displayed");
                                if (check[1] == "Reporting")
                                    _editSettingsManager.IsStatusRadioButtonChecked(3)
                                        .ShouldBeTrue(
                                            $"{EditStatusEnum.Reporting.GetStringValue()} radio button should be checked");
                            }
                            else
                                _editSettingsManager.IsStatusRadioButtonPresent()
                                    .ShouldBeFalse(
                                        $"Is {EditStatusEnum.Reporting.GetStringValue()} Radio button displayed");

                            if (check[1] == "On")
                                _editSettingsManager.IsStatusRadioButtonChecked()
                                    .ShouldBeTrue(
                                        $"Is {EditStatusEnum.On.GetStringValue()} radio button should be checked");
                            else if (check[1] == "Off")
                                _editSettingsManager.IsStatusRadioButtonChecked(2)
                                    .ShouldBeTrue(
                                        $"Is {EditStatusEnum.Off.GetStringValue()} radio button should be checked");

                        }

                        void SearchByEdit(string primaryEditor)
                        {
                            _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", "All");
                            _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit",
                                primaryEditor);
                            _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                                "All");
                            _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                            _editSettingsManager.WaitForWorking();
                        }
                    }


                    void ClickReviewStatusCheckBoxAndValidate(bool reviewStatus = true)
                    {
                        if (reviewStatus)
                        {
                            _editSettingsManager.IsReviewStatusCheckBoxChecked()
                                .ShouldBeTrue("Is Auto approve check box checked?");
                            _editSettingsManager.IsReviewStatusCheckBoxChecked(1)
                                .ShouldBeTrue("Is Client Review check box checked?");
                            _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                                .ShouldBeTrue("Is Internal Review check box checked?");
                        }
                        else
                        {
                            _editSettingsManager.IsReviewStatusCheckBoxChecked()
                                .ShouldBeFalse("Is Auto approve check box checked?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled()
                                .ShouldBeFalse("Is Auto approve checkbox disabled?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                                .ShouldBeTrue("Is Client Review checkbox disabled?");
                            _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                                .ShouldBeTrue("Is Internal Review checkbox disabled?");
                        }
                    }
                }
            }

            [Test]//TE-1105
            public void Verify_Ancillary_Settings_For_DUP()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                    var ancillarySettings =
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ancillary_settings_DUP").Values.ToList();
                    var commonAncillaySettings = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ancillary_settings_DUP_CPD").Values.ToList();
                    var flag = "DUP";
                    _editSettingsManager.SearchForEdit("Edit", flag);
                    try
                    {
                        StringFormatter.PrintMessage("Verify ancillary settings for DUP");
                        _editSettingsManager.ClickOnEditIconByFlag(flag);
                        _editSettingsManager.GetToolTipByLabelForDUP("DUP Match Options")
                            .ShouldBeEqual("Select value by which duplicate will be determined.", "tooltip correct?");
                        ancillarySettings.ShouldCollectionBeEqual(
                            _editSettingsManager.GetAncillarySettingsDropDown("DUP Match Options"),
                            "Ancillary Settings Dropdown Options Correct ?");
                        _editSettingsManager.SelectAncillaryDropDownSetting("DUP Match Options", ancillarySettings[1]);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("Confirmation pop up present?");
                        _editSettingsManager.GetAncillarySettingsForDUP()
                            .ShouldBeEqual(ancillarySettings[1], "DUP Match Options correct against database ?");

                        StringFormatter.PrintMessage("Verify SPEC Ancillary Settings");
                        _editSettingsManager.ClickOnEditIconByFlag(flag);
                        _editSettingsManager.GetToolTipByLabelForDUP("SPEC Override")
                            .ShouldBeEqual("Determines if specialty will override this edit.", "tooltip correct?");
                        commonAncillaySettings.ShouldCollectionBeEqual(
                            _editSettingsManager.GetAncillarySettingsDropDown("SPEC Override"),
                            "Ancillary Settings Dropdown Options Correct ?");
                        _editSettingsManager.SelectAncillaryDropDownSetting("SPEC Override", commonAncillaySettings[1]);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Confirmation message should be displayed");
                        _editSettingsManager.GetPageErrorMessage().ShouldBeEqual("Update to SPEC Override setting will effect both the DUP and CPD edits. Do you wish to continue?");
                        _editSettingsManager.ClickOkCancelOnConfirmationModal(true);
                        _editSettingsManager.GetAncillarySettingsForDUP(false)
                            .ShouldBeEqual(commonAncillaySettings[1], "SPEC Override settings correct against database ?");
                    }
                    finally
                    {
                        _editSettingsManager.ClickOnEditIconByFlag(flag);
                        _editSettingsManager.SelectAncillaryDropDownSetting("DUP Match Options", ancillarySettings[0]);
                        _editSettingsManager.SelectAncillaryDropDownSetting("SPEC Override", commonAncillaySettings[0]);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.ClickOkCancelOnConfirmationModal(true);
                    }

                }
        }

            [Test] //CAR-2550 (CAR-2640)
            public void Verify_that_COB_product_flags_are_getting_displayed_in_search()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var activeProductsForClient = _editSettingsManager.GetActiveProductListForClientDB();
                    var COBFlagDetailsFromDB = _editSettingsManager.GetCOBEditFlagDetailsByFlag();

                    try
                    {
                        activeProductsForClient.ShouldContain(ProductEnum.COB.ToString(),
                            $"{ProductEnum.COB.ToString()} is active for {ClientEnum.SMTST.ToString()}");

                        StringFormatter.PrintMessageTitle(
                            $"Verifying whether {ProductEnum.COB.ToString()} flags are being displayed in search result");
                        var productList =
                            _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList("Product");
                        productList.Contains(ProductEnum.COB.ToString()).ShouldBeTrue(
                            $"{ProductEnum.COB.ToString()} option should be present " +
                            $"in 'Product' dropdown");
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                            ProductEnum.COB.ToString());
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.All.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();
                        var productValueInGrid = _editSettingsManager.GetGridViewSection().GetGridListValueByCol(5);
                        productValueInGrid.All(product => product == "COB")
                            .ShouldBeTrue($"Only {ProductEnum.COB.ToString()} edits should be shown in result");
                        productValueInGrid.Count.ShouldBeEqual(COBFlagDetailsFromDB.Count, "Search count should match");


                        StringFormatter.PrintMessage("Verifying details of each COB edit flag");
                        VerifyCOBFlagDetails();
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.Prototype.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();
                        (_editSettingsManager.GetGridViewSection().GetGridListValueByCol(5)
                             .Any(product => product == "COB")
                         &&
                         _editSettingsManager.GetGridViewSection().GetGridListValueByCol(5)
                             .Any(product => product != "COB"))
                            .ShouldBeTrue(
                                $"{ProductEnum.COB.ToString()} flags should also be included as part of performing regular search");

                        void VerifyCOBFlagDetails()
                        {
                            foreach (var COBFlag in COBFlagDetailsFromDB)
                            {
                                _editSettingsManager.ClickOnEditIconByFlag(COBFlag[0]);
                                var expectedStatus = COBFlag[4];
                                bool expectedAutoApprove = COBFlag[1].Equals("T");
                                bool expectedInternalReview = COBFlag[2].Equals("T");
                                bool expectedClientReview = COBFlag[3].Equals("T");

                                switch (expectedStatus)
                                {
                                    case "On":
                                        _editSettingsManager.IsStatusRadioButtonChecked()
                                            .ShouldBeTrue("Edit Status is 'On'");
                                        break;

                                    case "Off":
                                        _editSettingsManager.IsStatusRadioButtonChecked(2)
                                            .ShouldBeTrue("Edit Status is 'Off'");
                                        break;

                                    case "Reporting":
                                        _editSettingsManager.IsStatusRadioButtonChecked(3)
                                            .ShouldBeTrue("Edit Status is 'Reporting'");
                                        break;

                                    case "Prototype":
                                        _editSettingsManager.IsStatusRadioButtonChecked(4)
                                            .ShouldBeTrue("Edit Status is 'Prototype'");
                                        break;
                                }

                                if (!expectedAutoApprove)
                                {
                                    _editSettingsManager.IsReviewStatusCheckBoxChecked(1)
                                        .ShouldBeFalse("'Auto Approved' status should not be checked");
                                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                                        .ShouldBeTrue(
                                            "'Client Review' should be disabled when 'Auto Approve' is unchecked");
                                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                                        .ShouldBeTrue(
                                            "'Internal Review' should be disabled when 'Auto Approve' is unchecked");
                                }
                                else
                                {
                                    _editSettingsManager.IsReviewStatusCheckBoxChecked(1)
                                        .ShouldBeTrue("'Auto Approved' status should be checked");
                                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                                        .ShouldBeFalse(
                                            "'Client Review' should not be disabled when 'Auto Approve' is checked");
                                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                                        .ShouldBeFalse(
                                            "'Internal Review' should not be disabled when 'Auto Approve' is checked");

                                    if (expectedClientReview)
                                        _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                                            .ShouldBeTrue("'Client Review' should be checked");
                                    else
                                        _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                                            .ShouldBeFalse("'Client Review' should not be checked");

                                    if (expectedInternalReview)
                                        _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                                            .ShouldBeTrue("'Client Review' should be checked");
                                    else
                                        _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                                            .ShouldBeFalse("'Client Review' should not be checked");
                                }

                                _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                            }
                        }

                    }

                    finally
                    {
                        if (_editSettingsManager.IsPageErrorPopupModalPresent())
                            _editSettingsManager.ClosePageError();

                        _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                    }
                }
            }

            [Test] //CAR-2309(CAR-2351)
            [Retrying(Times = 3)]
            public void Verification_of_Prototype_Status()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    StringFormatter.PrintMessage(
                        "Verification that Prototype will be shown as an Edit Status drop down option");

                    var prototypeOption = EditStatusEnum.Prototype.GetStringValue();
                    var TestName = new StackFrame(true).GetMethod().Name;
                    var editFlag =
                        automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "Edit", "Value");

                    _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList("Edit Status")
                        .ShouldContain(prototypeOption, $"{prototypeOption} should be present in edit status list");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                        prototypeOption);
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                        ProductEnum.CV.ToString());
                    _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();

                    //var editFlag = _editSettingsManager.GetGridViewSection().GetValueInGridByColRow();
                    var editStatus = _editSettingsManager.GetStatusValueForFirstEditInGridFromDB();

                    StringFormatter.PrintMessage(
                        $"Verification that {prototypeOption} will be shown as the status in the primary data display for the edit");
                    _editSettingsManager.GetGridViewSection().GetValueInGridByColRow(3)
                        .ShouldBeEqual(editStatus, $"Status should be {prototypeOption}");
                    _editSettingsManager.ClickOnEditIconByFlag(editFlag);

                    StringFormatter.PrintMessage("Verification of radio buttons and checkboxes");

                    _editSettingsManager.IsStatusRadioButtonDisabled(1)
                        .ShouldBeTrue("On radio button should be disabled");
                    _editSettingsManager.IsStatusRadioButtonDisabled(2)
                        .ShouldBeTrue("Off radio button should be disabled");
                    _editSettingsManager.IsStatusRadioButtonDisabled(3)
                        .ShouldBeTrue("Reporting radio button should be disabled");
                    _editSettingsManager.IsStatusRadioButtonDisabled(4)
                        .ShouldBeTrue($"{prototypeOption} radio button should be disabled");
                    _editSettingsManager.IsStatusRadioButtonChecked(4)
                        .ShouldBeTrue($"{prototypeOption} radio button should be checked");
                    _editSettingsManager.IsAncillaryRadioButtonEnabled(2)
                        .ShouldBeTrue("No radio button should be selected");
                    _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                        .ShouldBeFalse("Internal review should not be checked");

                    StringFormatter.PrintMessage(
                        "Verifying whether user will be able to modify Fall BackOrder settings and ancillary settings and save it");

                    var allDataSets = _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm();
                    var originallyUsedDataSets = _editSettingsManager.GetAllActiveDataSetsFromTheForm();
                    var originallyUnusedDataSets = allDataSets.Except(originallyUsedDataSets).ToList();

                    try
                    {
                        foreach (var dataSet in originallyUsedDataSets)
                            _editSettingsManager.ClickOnCheckboxByDataSetName(dataSet);

                        for (int count = 0; count < originallyUnusedDataSets.Count; count++)
                        {
                            _editSettingsManager.ClickOnCheckboxByDataSetName(
                                originallyUnusedDataSets.ElementAt(count));
                            _editSettingsManager.SetFallBackorderForDataSet(originallyUnusedDataSets.ElementAt(count),
                                (count + 1).ToString());
                        }

                        _editSettingsManager.ClickAncillarySettingsRadioButtons(1);
                        _editSettingsManager.ClickOnReviewStatusCheckBox(3);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.ClickOnEditIconByFlag(editFlag);
                        _editSettingsManager.GetAllActiveDataSetsFromTheForm().ShouldCollectionBeEqual(
                            originallyUnusedDataSets,
                            "Updated edit dataset should be saved");
                        _editSettingsManager.IsAncillaryRadioButtonEnabled(1)
                            .ShouldBeTrue("Edits to the Ancillary Settings should be saved");
                        _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                            .ShouldBeTrue("Internal Review checkBox should be checked");
                    }

                    finally
                    {
                        StringFormatter.PrintMessageTitle(
                            "Reverting the ancillary settings and review status changes made in the form");
                        if (_editSettingsManager.IsReviewStatusCheckBoxChecked(3))
                            _editSettingsManager.ClickOnReviewStatusCheckBox(3);

                        if (_editSettingsManager.IsAncillaryRadioButtonEnabled(1))
                            _editSettingsManager.ClickAncillarySettingsRadioButtons(2);

                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                    }
                }
            }

            [Test] //CAR-2353(CAR-1868)
            public void Verify_secondary_edit_details()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    IDictionary<string, string> paramLists =
                        automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                    var labelList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "secondary_detail_label_list")
                        .Values
                        .ToList();
                    var labelTooltipList = automatedBase.DataHelper
                        .GetMappingData(FullyQualifiedClassName, "secondary_detail_label_tooltip_list").Values.ToList();
                    var singleFlag = paramLists["SingleFlag"];
                    var multipleFlag = paramLists["MultipleFlag"];
                    var flagList = paramLists["FlagList"];

                    var expectedEditDetails = _editSettingsManager.GetEditDetailsByFlag(flagList);
                    _editSettingsManager.GetGridViewSection().ClickOnGridRowByValue(multipleFlag);

                    StringFormatter.PrintMessageTitle("Verification of Tooltip and value for multiple edit flag");
                    for (var i = 0; i < labelList.Count; i++)
                    {
                        _editSettingsManager.GetToolTipByLabel(labelList[i])
                            .ShouldBeEqual(labelTooltipList[i], $"Is {labelList[i]} Tooltip for Equals?");

                        _editSettingsManager.GetValueByLabel(labelList[i])
                            .ShouldBeEqual(expectedEditDetails[0][i], $"Is Value for <{labelList[i]}>  Equals?");
                    }

                    _editSettingsManager.GetGridViewSection().ClickOnGridRowByValue(singleFlag);
                    StringFormatter.PrintMessageTitle("Verification of value for single edit flag");
                    for (var i = 0; i < labelList.Count; i++)
                    {
                        if (i == 0)
                            expectedEditDetails[1][i] = Regex.Replace(expectedEditDetails[1][i], "<.*?>", String.Empty)
                                .Replace("  ", " ");
                        _editSettingsManager.GetValueByLabel(labelList[i])
                            .ShouldBeEqual(expectedEditDetails[1][i], $"Is Value for <{labelList[i]}>  Equals?");

                    }
                }

            }

            [Test] //CAR-1732(CAR-2003)
            [Retrying(Times = 3)]
            public void Verify_edit_functionality()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    IDictionary<string, string> paramLists =
                        automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                    var flag = paramLists["Flag"];
                    var autoApproveTooltip = paramLists["AutoApproveTooltip"];
                    var clientReviewTooltip = paramLists["ClientReviewTooltip"];
                    var internalReviewTooltip = paramLists["InternalReviewTooltip"];
                    var reportingSettings = _editSettingsManager.GetReportingSettingsFromDB();
                    var statusValue = _editSettingsManager.GetStatusValueForFirstEditInGridFromDB();
                    StringFormatter.PrintMessageTitle("Verify Header Text for Edit Settings Manager Edit Form");
                    EditAndWaitForWorkingMessage();
                    _editSettingsManager.GetSideWindow().GetHeaderText()
                        .ShouldBeEqual("Modify Edit Settings", "Is Edit Form Header Text Equals?");

                    StringFormatter.PrintMessageTitle("Verify Radio button for different status");
                    VerifyStatusRadioButton(new List<string> {reportingSettings, statusValue});

                    SearchByStatus(EditStatusEnum.Off);
                    EditAndWaitForWorkingMessage();
                    reportingSettings = _editSettingsManager.GetReportingSettingsFromDB();
                    statusValue = _editSettingsManager.GetStatusValueForFirstEditInGridFromDB();
                    VerifyStatusRadioButton(new List<string> {reportingSettings, statusValue});

                    SearchByStatus(EditStatusEnum.Reporting);
                    EditAndWaitForWorkingMessage();
                    reportingSettings = _editSettingsManager.GetReportingSettingsFromDB();
                    statusValue = _editSettingsManager.GetStatusValueForFirstEditInGridFromDB();
                    VerifyStatusRadioButton(new List<string> {reportingSettings, statusValue});

                    StringFormatter.PrintMessageTitle("Verify Tooltip different status");
                    _editSettingsManager.GetStatusOnOffLabel()
                        .ShouldBeEqual(EditStatusEnum.On.GetStringValue(), "Status On Label");
                    _editSettingsManager.GetStatusOnOffLabel(2)
                        .ShouldBeEqual(EditStatusEnum.Off.GetStringValue(), "Status On Label");
                    _editSettingsManager.GetStatusOnOffLabel(3)
                        .ShouldBeEqual(EditStatusEnum.Reporting.GetStringValue(), "Status On Label");


                    _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                    SearchByStatus(EditStatusEnum.All);


                    EditAndWaitForWorkingMessage(flag);
                    var statusIndex = _editSettingsManager.IsStatusRadioButtonChecked() ? 2 : 1;
                    var reviewIndex = _editSettingsManager.IsReviewStatusCheckBoxChecked();

                    if (_editSettingsManager.IsReviewStatusCheckBoxChecked())
                        _editSettingsManager.ClickOnReviewStatusCheckBox();

                    StringFormatter.PrintMessageTitle("Verify Auto Approve checkbox functionality");

                    _editSettingsManager.ClickOnReviewStatusCheckBox();
                    _editSettingsManager.IsReviewStatusCheckBoxChecked()
                        .ShouldBeTrue($"{EditReviewStatus.AutoApprove.GetStringValue()} should be checked");

                    _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                        .ShouldBeTrue($"Is {EditReviewStatus.ClientReview.GetStringValue()} checkbox checked");
                    _editSettingsManager.ClickOnReviewStatusCheckBox(2);
                    _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                        .ShouldBeFalse($"Is {EditReviewStatus.ClientReview.GetStringValue()} checkbox checked");
                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                        .ShouldBeFalse($"Is {EditReviewStatus.ClientReview.GetStringValue()} checkbox disabled?");

                    _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                        .ShouldBeTrue($"Is {EditReviewStatus.InternalReview.GetStringValue()} checkbox checked");
                    _editSettingsManager.ClickOnReviewStatusCheckBox(3);
                    _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                        .ShouldBeFalse($"Is {EditReviewStatus.InternalReview.GetStringValue()} checkbox checked");
                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                        .ShouldBeFalse($"Is {EditReviewStatus.InternalReview.GetStringValue()} checkbox disabled?");

                    _editSettingsManager.ClickOnReviewStatusCheckBox();
                    _editSettingsManager.IsReviewStatusCheckBoxChecked()
                        .ShouldBeFalse($"Is {EditReviewStatus.AutoApprove.GetStringValue()} checkbox checked");

                    _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                        .ShouldBeFalse($"Is {EditReviewStatus.ClientReview.GetStringValue()} checkbox checked");
                    _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                        .ShouldBeFalse($"Is {EditReviewStatus.InternalReview.GetStringValue()} checkbox checked");

                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(2)
                        .ShouldBeTrue($"Is {EditReviewStatus.ClientReview.GetStringValue()} checkbox disabled?");
                    _editSettingsManager.IsReviewStatusCheckBoxDisabled(3)
                        .ShouldBeTrue($"Is {EditReviewStatus.InternalReview.GetStringValue()} checkbox disabled?");

                    StringFormatter.PrintMessageTitle("Verify Tooltip of Review Status Checkbox ");
                    _editSettingsManager.GetReviewStatusCheckBoxTitle()
                        .ShouldBeEqual(autoApproveTooltip, "Tooltip for Auto approve checkbox");
                    _editSettingsManager.GetReviewStatusCheckBoxTitle(2)
                        .ShouldBeEqual(clientReviewTooltip, "Tooltip for Client Review checkbox");
                    _editSettingsManager.GetReviewStatusCheckBoxTitle(3)
                        .ShouldBeEqual(internalReviewTooltip, "Tooltip for Internal Review checkbox");

                    StringFormatter.PrintMessageTitle("Verify Audit Record for cancel and save");
                    var countBeforeSave = _editSettingsManager.GetTotalEditSettingAuditCount(flag);

                    StringFormatter.PrintMessageTitle("Verify Save and Cancel functionality");

                    _editSettingsManager.ClickOnStatusRadioButton(statusIndex);
                    _editSettingsManager.ClickOnReviewStatusCheckBox();
                    _editSettingsManager.GetSideWindow().Cancel();
                    _editSettingsManager.WaitForWorking();
                    _editSettingsManager.GetSideWindow().IsSideWindowBlockPresent()
                        .ShouldBeFalse("Form Should Closed when Cancel is selected");
                    EditAndWaitForWorkingMessage(flag);

                    _editSettingsManager.IsStatusRadioButtonChecked(statusIndex)
                        .ShouldBeFalse("Status Should not Updated");
                    if (reviewIndex)
                        _editSettingsManager.IsReviewStatusCheckBoxChecked()
                            .ShouldBeTrue("Review Status Should not Updated");
                    else
                        _editSettingsManager.IsReviewStatusCheckBoxChecked()
                            .ShouldBeFalse("Review Status Should not Updated");

                    _editSettingsManager.GetTotalEditSettingAuditCount(flag)
                        .ShouldBeEqual(countBeforeSave, "Audit Record Should not saved");

                    _editSettingsManager.ClickOnStatusRadioButton(statusIndex);
                    _editSettingsManager.ClickOnReviewStatusCheckBox();
                    _editSettingsManager.GetSideWindow().Save();
                    _editSettingsManager.WaitForWorking();
                    _editSettingsManager.GetSideWindow().IsSideWindowBlockPresent()
                        .ShouldBeFalse("Form Should Closed when Save is clicked");
                    EditAndWaitForWorkingMessage(flag);
                    _editSettingsManager.IsStatusRadioButtonChecked(statusIndex).ShouldBeTrue("Status Should Updated");
                    if (reviewIndex)
                    {
                        _editSettingsManager.IsReviewStatusCheckBoxChecked()
                            .ShouldBeFalse("Review Status Should Updated");
                        _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                            .ShouldBeFalse("Review Status Should Updated");
                        _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                            .ShouldBeFalse("Review Status Should Updated");
                    }
                    else
                    {
                        _editSettingsManager.IsReviewStatusCheckBoxChecked()
                            .ShouldBeTrue("Review Status Should Updated");
                        _editSettingsManager.IsReviewStatusCheckBoxChecked(2)
                            .ShouldBeTrue("Review Status Should Updated");
                        _editSettingsManager.IsReviewStatusCheckBoxChecked(3)
                            .ShouldBeTrue("Review Status Should Updated");
                    }

                    _editSettingsManager.GetTotalEditSettingAuditCount(flag)
                        .ShouldBeEqual(countBeforeSave + 1, "Audit Record Should Saved");

                    void VerifyStatusRadioButton(List<string> check)
                    {
                        if (check[0] == "T")
                        {
                            _editSettingsManager.IsStatusRadioButtonPresent()
                                .ShouldBeTrue($"Is {EditStatusEnum.Reporting.GetStringValue()} Radio button displayed");
                            if (check[1] == "Reporting")
                                _editSettingsManager.IsStatusRadioButtonChecked(3)
                                    .ShouldBeTrue(
                                        $"{EditStatusEnum.Reporting.GetStringValue()} radio button should be checked");
                        }
                        else
                            _editSettingsManager.IsStatusRadioButtonPresent()
                                .ShouldBeFalse(
                                    $"Is {EditStatusEnum.Reporting.GetStringValue()} Radio button displayed");

                        if (check[1] == "On")
                            _editSettingsManager.IsStatusRadioButtonChecked()
                                .ShouldBeTrue(
                                    $"Is {EditStatusEnum.On.GetStringValue()} radio button should be checked");
                        else if (check[1] == "Off")
                            _editSettingsManager.IsStatusRadioButtonChecked(2)
                                .ShouldBeTrue(
                                    $"Is {EditStatusEnum.Off.GetStringValue()} radio button should be checked");

                    }

                    void SearchByStatus(EditStatusEnum status)
                    {
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            status.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();
                    }

                    void EditAndWaitForWorkingMessage(string flg = null)
                    {
                        if (flg == null)
                            _editSettingsManager.GetGridViewSection().ClickOnEditIcon();
                        else
                            _editSettingsManager.ClickOnEditIconByFlag(flg);
                        _editSettingsManager.WaitForWorking();
                    }
                }
            }

            [Test] //CAR-1730 (CAR-1968)
            public void Verify_find_edits_filter_options()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    Dictionary<string, string> listOfAddFlags = new Dictionary<string, string>()
                    {
                        ["FOT ADD"] = "R", // R for reporting
                        ["UNB ADD"] = "T"
                    };
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                    List<string> _activeProductListForClientDB = _editSettingsManager.GetActiveProductListForClientDB();
                    _activeProductListForClientDB.Insert(0, "All");
                    var TestName = new StackFrame(true).GetMethod().Name;
                    _editSettingsManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                        .ShouldBeTrue("Side bar panel should be opened by default");
                    _editSettingsManager.GetSideBarPanelSearch.GetSearchFiltersList().ShouldCollectionBeEqual(
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "search_filter_list").Values.ToList(),
                        "Search Filter List should match");
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabel("Product")
                        .ShouldBeEqual("All", "Product dropdown should default to All");
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Edit")
                        .ShouldBeEqual("Type to filter", "Default value for Edit dropdown");
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabel("Edit Status")
                        .ShouldBeEqual("On", "Default value for Edit Status dropdown");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Product", _activeProductListForClientDB,
                        false);

                    ValidateSingleDropDownForDefaultValueAndExpectedList("Edit Status",
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "edit_status_filter").Values.ToList(),
                        false);

                    Verify_editflag_list_per_product_type("Product");

                    void ValidateSingleDropDownForDefaultValueAndExpectedList(string label,
                        IList<string> collectionToEqual, bool order = true)
                    {
                        var actualDropDownList =
                            _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                        actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                        if (collectionToEqual != null)
                            actualDropDownList.ShouldCollectionBeEquivalent(collectionToEqual,
                                label + " List As Expected");
                        if (order)
                        {
                            actualDropDownList.Remove("All");
                            actualDropDownList.IsInAscendingOrder()
                                .ShouldBeTrue(label + " should be sorted in alphabetical order.");
                        }

                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label,
                            actualDropDownList[0], false); //check for type ahead functionality
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label,
                            actualDropDownList[1]);
                        _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabel(label)
                            .ShouldBeEqual(actualDropDownList[1], $"User can select only a single option for {label} ");

                        _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                    }

                    void Verify_editflag_list_per_product_type(string label)
                    {
                        var gridValuesFromDb = _editSettingsManager.GetAllResultsInEditSettingsManagerGrid();


                        var productDropDownList =
                            _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                        for (int i = 0; i < productDropDownList.Count; i++)
                        {
                            _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label,
                                productDropDownList[i]);

                            var editDropdownList = new List<string>();

                            switch (productDropDownList[i])
                            {
                                case "CV":
                                    editDropdownList = gridValuesFromDb.Where(x =>
                                            (string) x.ItemArray[4] == ProductEnum.CV.GetStringDisplayValue())
                                        .Select(x => x.ItemArray[0].ToString()).ToList();
                                    foreach (var flag in listOfAddFlags)
                                    {
                                        editDropdownList.Add(flag.Key);
                                    }

                                    editDropdownList.Sort();
                                    break;

                                case "DCA":
                                    editDropdownList = gridValuesFromDb.Where(x =>
                                            (string) x.ItemArray[4] == ProductEnum.DCA.GetStringDisplayValue())
                                        .Select(x => x.ItemArray[0].ToString()).ToList();
                                    break;

                                case "FFP":
                                    editDropdownList = gridValuesFromDb.Where(x =>
                                            (string) x.ItemArray[4] == ProductEnum.FFP.GetStringDisplayValue())
                                        .Select(x => x.ItemArray[0].ToString()).ToList();
                                    break;

                                case "FCI":
                                    editDropdownList = gridValuesFromDb.Where(x =>
                                            (string) x.ItemArray[4] == ProductEnum.FCI.GetStringDisplayValue())
                                        .Select(x => x.ItemArray[0].ToString()).ToList();
                                    break;

                                case "NEG":
                                    editDropdownList = gridValuesFromDb.Where(x =>
                                            (string) x.ItemArray[4] == ProductEnum.NEG.GetStringDisplayValue())
                                        .Select(x => x.ItemArray[0].ToString()).ToList();
                                    break;

                                case "COB":
                                    editDropdownList =
                                        gridValuesFromDb.Where(x =>
                                                (string) x.ItemArray[4] == ProductEnum.COB.GetStringDisplayValue())
                                            .Select(x => x.ItemArray[0].ToString()).ToList();
                                    break;
                            }

                            if (i == 0)
                                continue;
                            editDropdownList.Insert(0, "");
                            if (editDropdownList != null)
                                editDropdownList.ShouldCollectionBeEqual(
                                    _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList("Edit"),
                                    "List should match");
                            else
                                _editSettingsManager.GetSideBarPanelSearch.GetAvailableDropDownList("Edit")
                                    .Where(s => s != "").ToList().ShouldCollectionBeEmpty("List should match");
                        }
                    }
                }

            }

            [Test] //CAR-1730 (CAR-1968)
            public void
                Verify_search_results_as_per_selected_criteria_and_selecting_cancel_will_result_filters_to_their_default_state()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    StringFormatter.PrintMessage(
                        "Verification clicking on clear button reverts the input fields back to the original state");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", "CV");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit", "ACW");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                        "Reporting");
                    _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabel("Product")
                        .ShouldBeEqual("All", "Product dropdown should default to All");
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Edit")
                        .ShouldBeEqual("Type to filter", "Default value for Edit dropdown");
                    _editSettingsManager.GetSideBarPanelSearch.GetInputValueByLabel("Edit Status")
                        .ShouldBeEqual("On", "Default value for Edit Status dropdown");

                    StringFormatter.PrintMessageTitle(
                        "Verification of performing a search by entering the search criteria");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", "CV");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit", "ACW");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status", "On");
                    _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _editSettingsManager.WaitForWorking();

                    _editSettingsManager.GetGridViewSection().GetGridRowCount()
                        .ShouldBeEqual(1, "Search result should be displayed in the grid");
                    _editSettingsManager.GetGridViewSection().GetValueInGridByColRow()
                        .ShouldBeEqual("ACW", "The filtered Edit Flag should be displayed");
                    _editSettingsManager.GetGridViewSection().GetValueInGridByColRow(3)
                        .ShouldBeEqual("On", "The status of the filtered Edit Flag should be 'On'");
                    _editSettingsManager.GetGridViewSection().GetValueInGridByColRow(5)
                        .ShouldBeEqual("CV", "The product of the filtered Edit Flag should be 'CV'");
                }

            }

            [Test] //CAR-1733, CAR-1969 + CAR-1913(CAR-2350)
            public void Validate_Security_And_Navigation_Of_Edit_Settings_Manager_Page()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                    EditSettingsManagerPage _editSettingsManager;
                    ClientSearchPage _clientSearchPage;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                    string _productMaintenancePrivileges = RoleEnum.ProductAdmin.GetStringValue();

                    try
                    {
                        StringFormatter.PrintMessage("Validation of Page specifications");
                        _editSettingsManager.GetPageHeader().ShouldBeEqual(
                            PageHeaderEnum.EditSettingsManager.GetStringValue(),
                            "Page should be directed to Edit Settings Manager");
                        _editSettingsManager.GetClientCodeValue().ShouldBeEqual(ClientEnum.SMTST.ToString(),
                            $"Client Code displayed should be {ClientEnum.SMTST.ToString()}");
                        _editSettingsManager.GetLoggedInUserFullName().ShouldBeEqual("Test Automation",
                            "Validation that Test Automation user is logged in");

                        StringFormatter.PrintMessage("Validation of user with read/write authority");
                        _commonValidation.ValidateSecurityOfPage(_productMaintenancePrivileges, true,
                            firstLastNames: new[] {"Test", "Automation"});




                        StringFormatter.PrintMessage("Validation of side panel");
                        _editSettingsManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                            .ShouldBeTrue("Is side bar panel open?");
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                        _editSettingsManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                            .ShouldBeFalse("Is side bar panel open?");

                        StringFormatter.PrintMessage("Verification of return to Client Search functionality");
                        _clientSearchPage = _editSettingsManager.ClickOnReturnToClientSearch();
                        _clientSearchPage.GetPageHeader()
                            .ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue(),
                                "Page should be navigated to Client Search");

                        StringFormatter.PrintMessage("Verification of authority for user with readonly privilege");
                        _editSettingsManager.IsRoleAssigned<UserProfileSearchPage>(
                            new List<string> {"Test", "Automation6"},
                            RoleEnum.ProductAdminReadOnly.GetStringValue(), false).ShouldBeTrue(
                            $"Is Product Maintanence present for current user <Test Automation6 >");

                        StringFormatter.PrintMessage("Verification of authority for user with no privilege");
                        _editSettingsManager
                            .IsRoleAssigned<UserProfileSearchPage>(new List<string> {"Test4", "Automation4"},
                                RoleEnum.ProductAdminReadOnly.GetStringValue(), false).ShouldBeFalse(
                                $"Is Product Maintanence present for current user <Test4 Automation4 >");

                        StringFormatter.PrintMessage("Verification of authority for Client User");
                        _editSettingsManager
                            .IsRoleAssigned<UserProfileSearchPage>(new List<string> {"uiautomation_client", "client"},
                                RoleEnum.ProductAdminReadOnly.GetStringValue(), false).ShouldBeFalse(
                                $"Is Product Maintanence present for current user <uiautomation_client client >");

                        StringFormatter.PrintMessage("Verification of page navigation by User with readonly access");
                        _editSettingsManager = automatedBase.CurrentPage.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities()
                            .NavigateToEditSettingsManager();
                        _editSettingsManager.GetPageHeader().ShouldBeEqual(
                            PageHeaderEnum.EditSettingsManager.GetStringValue(),
                            "Page should be directed to Edit Settings Manager for user with readonly 'Product Admin Read Only' role");
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnClearLink();
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.All.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();
                        _editSettingsManager.GetGridViewSection().IsEnabledEditIconPresent()
                            .ShouldBeFalse("Edit Icon Should disabled");

                        StringFormatter.PrintMessage("Verification of page navigation by User with No Authority");
                        var currentUrlForUserWithNoAuthority = _editSettingsManager.Logout()
                            .LoginAsUserHavingNoAnyAuthority()
                            .VisitAndGetUrlByUrlForUnAuthorizedPage(PageUrlEnum.EditSettingsManager.GetStringValue());
                        automatedBase.VerifyIfUrlIsUnauthorized(currentUrlForUserWithNoAuthority);

                        StringFormatter.PrintMessage("Verification of page navigation by Client User");
                        var currentUrlForClientUser = _editSettingsManager.Logout().LoginAsClientUser()
                            .VisitAndGetUrlByUrlForUnAuthorizedPage(PageUrlEnum.EditSettingsManager.GetStringValue());
                        automatedBase.VerifyIfUrlIsUnauthorized(currentUrlForClientUser);
                    }

                    finally
                    {
                        if (_editSettingsManager.GetWindowHandlesCount() > 1)
                            _editSettingsManager.CloseAnyTabIfExist();

                        _editSettingsManager.ClickOnQuickLaunch();
                    }
                }
            }

            [Test,Category("Working")] //CAR-1731 (CAR-1967) + CAR-1732 + CAR-2664(CAR-2696) +CV-8952(CV-8846)
        public void Verify_edit_settings_manager_display_pci_edit_records()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    Dictionary<string, string> editsWithRespectivePrimaryEditor = new Dictionary<string, string>
                    {
                        ["M25R"] = "M25R / R25R",
                        ["NCD"] = "NCD / NCDN",
                        ["REBD"] = "REBD / RBPD",
                        ["UPR"] = "UPR / RUPR",
                        ["ASM"] = "ASM / RASM",
                        ["BIL"] = "BIL / MAX",
                        ["FUD"] = "FUD / GPA",
                        ["MPR"] = "MPR / PS",
                        ["NPT"] = "NPT / NPR",
                        ["REB"] = "REB / RBP",
                        ["NNPT"]= "NNPT / NNPR"
                    };
                    Dictionary<string, string> listOfAddFlags = new Dictionary<string, string>()
                    {
                        ["FOT ADD"] = "R", // R for reporting
                        ["UNB ADD"] = "T"
                    };
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();
                    List<string> _activeProductListForClientDB = _editSettingsManager.GetActiveProductListForClientDB();
                    _activeProductListForClientDB.Insert(0, "All");
                    StringFormatter.PrintMessageTitle("The active edits should be shown in the page");
                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                        EditStatusEnum.Reporting.GetStringValue());
                    _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _editSettingsManager.WaitForWorking();


                    var listOfReportingEditFlags = _editSettingsManager.GetReportingFlagList();

                    foreach (var flag in listOfAddFlags)
                    {
                        if (flag.Value == "R")
                        {
                            listOfReportingEditFlags.Add(flag.Key);
                        }

                        listOfReportingEditFlags.Sort();

                    }

                    foreach (var primaryEditor in editsWithRespectivePrimaryEditor.Keys)
                    {
                        if (listOfReportingEditFlags.Contains(primaryEditor))
                            listOfReportingEditFlags[listOfReportingEditFlags.IndexOf(primaryEditor)] =
                                editsWithRespectivePrimaryEditor[primaryEditor];
                    }

                    _editSettingsManager.GetGridViewSection().GetGridListValueByCol()
                        .ShouldCollectionBeEqual(listOfReportingEditFlags,
                            "All the active Reporting edits are listed in the Edit Settings Manager grid");

                    _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status", "All");
                    _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _editSettingsManager.WaitForWorking();

                    var gridValuesFromDb = _editSettingsManager.GetAllResultsInEditSettingsManagerGrid();

                    var listOfEditFlags = gridValuesFromDb.Select(x => x.ItemArray[0]).ToList();
                    foreach (var flag in listOfAddFlags)
                    {
                        listOfEditFlags.Add(flag.Key);
                    }

                    listOfEditFlags.Sort();

                    foreach (var primaryEditor in editsWithRespectivePrimaryEditor.Keys)
                    {
                        if (listOfEditFlags.Contains(primaryEditor))
                            listOfEditFlags[listOfEditFlags.IndexOf(primaryEditor)] =
                                editsWithRespectivePrimaryEditor[primaryEditor];
                    }

                    _editSettingsManager.GetGridViewSection().GetEnabledEditIconListCount()
                        .ShouldBeEqual(listOfEditFlags.Count, "All Edit icons should enabled");

                    _editSettingsManager.GetGridViewSection().GetGridListValueByCol()
                        .ShouldCollectionBeEqual(listOfEditFlags,
                            "All the active edits are listed in the Edit Settings Manager grid");

                    StringFormatter.PrintMessageTitle(
                        "Edit Records should be shown in the default order of edit names ascending");
                    _editSettingsManager.GetGridViewSection().GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("Records are sorted in ascending order of Edit name");

                    StringFormatter.PrintMessageTitle("Verification of Data Points");
                    _editSettingsManager.GetGridViewSection().IsPencilIconPresentInEachRecord()
                        .ShouldBeTrue("The Pencil icon should be present");

                    _editSettingsManager.GetGridViewSection().GetLabelInGridByColRow(3)
                        .ShouldBeEqual("Status:", "The label for the status field should be 'Status'");
                    var statusList = _editSettingsManager.GetGridViewSection().GetGridListValueByCol(3).Distinct()
                        .ToList();
                    statusList.Where(x => Regex.Match(x, @"^(On|Off|Reporting|Prototype)").Success).ToList()
                        .ShouldCollectionBeEqual(statusList, "Status should be either 'On/Off/Reporting'");

                    _editSettingsManager.GetGridViewSection().GetLabelInGridByColRow(4).ShouldBeEqual("Auto Approve:",
                        "The label for the status field should be 'Status'");
                    var autoApproveList = _editSettingsManager.GetGridViewSection().GetGridListValueByCol(4).Distinct()
                        .ToList();
                    autoApproveList.Where(x => Regex.Match(x, @"^(On|Off)").Success).ToList()
                        .ShouldCollectionBeEqual(autoApproveList, "'Auto Approve' should be either 'On/Off'");

                    var productList = _editSettingsManager.GetGridViewSection().GetGridListValueByCol(5);

                    productList.Distinct().ToList().CollectionShouldBeSubsetOf(_activeProductListForClientDB, "");
                }
            }

            [Test] //CAR-2044 (CAR-1976) + CAR-2664(CAR-2696)
            public void Verify_update_labels_for_edits_using_primary_editor()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    Dictionary<string, string> editsWithRespectivePrimaryEditor = new Dictionary<string, string>
                    {
                        ["M25R"] = "M25R / R25R",
                        ["NCD"] = "NCD / NCDN",
                        ["REBD"] = "REBD / RBPD",
                        ["UPR"] = "UPR / RUPR",
                        ["ASM"] = "ASM / RASM",
                        ["BIL"] = "BIL / MAX",
                        ["FUD"] = "FUD / GPA",
                        ["MPR"] = "MPR / PS",
                        ["NPT"] = "NPT / NPR",
                        ["REB"] = "REB / RBP"
                    };

                    Dictionary<string, string> listOfAddFlags = new Dictionary<string, string>()
                    {
                        ["FOT ADD"] = "R", // R for reporting
                        ["UNB ADD"] = "T"
                    };
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    var secondaryEditorList = automatedBase.DataHelper
                        .GetSingleTestData(FullyQualifiedClassName, TestName, "Editor", "Value")
                        .Split(';')
                        .ToList();
                    List<string> editorValues = editsWithRespectivePrimaryEditor.Values.ToList();
                    int i = 0;

                    foreach (var primaryEditor in editsWithRespectivePrimaryEditor.Keys)
                    {
                        StringFormatter.PrintMessageTitle(
                            $"Verification of performing a search by primary Editor: {primaryEditor}");
                        SearchByEdit(primaryEditor);
                        if (primaryEditor != "UPR")
                            _editSettingsManager.GetGridViewSection().GetValueInGridByColRow().ShouldBeEqual(
                                editorValues[i],
                                "Search Using Primary editor should show both the editors  divided by slash.");
                        i++;
                    }

                    void SearchByEdit(string primaryEditor)
                    {
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", "All");
                        _editSettingsManager.GetSideBarPanelSearch
                            .SelectDropDownListValueByLabel("Edit", primaryEditor);
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status", "All");
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();
                    }

                    //foreach (var secondaryEditor in secondaryEditorList)
                    //{
                    //    StringFormatter.PrintMessageTitle(
                    //        $"Verification of performing a search by secondary Editor: {secondaryEditor}");
                    //    SearchByEdit(secondaryEditor);
                    //    _editSettingsManager.GetGridViewSection().IsNoDataMessagePresent()
                    //        .ShouldBeTrue("No data should be seen while searching using secondary editor.");
                    //}
                }
            }

            [Test, Category("SchemaDependent")] //CAR-1783 (CAR-2046) + CAR-3034(CAR-3005)
            public void Verify_fall_back_order_functionality()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                    var dataSetsForEditWhichIsNotPciNonCoveredService =
                        paramList["DataSetsForEditWhichIsNotPciNonCoveredService"].Split(';').ToList();
                    var dataSetsForEditWhichIsPciUnbundle =
                        paramList["DataSetsForEditWhichIsPciUnbundle"].Split(';').ToList();
                    var dataSetsForEditWhichIsPciNonCoveredService =
                        paramList["DataSetsForEditWhichIsPciNonCoveredService"].Split(';').ToList();

                    var AOM = paramList["EditWhichIsNotPciNonCoveredService"];
                    var UNB = paramList["EditWhichIsPciUnbundle"];
                    var NCS = paramList["EditWhichIsPciNonCoveredService"];

                    var editList = new List<string>()
                    {
                        AOM,
                        UNB,
                        NCS
                    };

                    // There is no client with LOB - Medicaid, so the verification of dataset with LOB- Medicaid has been omitted as per communication with Amy
                    /* Datasets for an edit from logic
                    var defaultDataSetsForAOM = paramList["DataSetsForEditWhichIsNotPciNonCoveredService"].Split(';').ToList();
                    var defaultDataSetsForUNB = paramList["DataSetsForEditWhichIsPciUnbundle"].Split(';').ToList();
                    var defaultDataSetsForNCS = paramList["DataSetsForEditWhichIsPciNonCoveredService"].Split(';').ToList();
        
                    // Merging the Datasets from DB to get the complete default dataset for the edit
                    defaultDataSetsForAOM.AddRange(_editSettingsManager.GetDefaultOrderListFromDB("AOM"));
                    defaultDataSetsForUNB.AddRange(_editSettingsManager.GetDefaultOrderListFromDB("UNB"));
                    defaultDataSetsForNCS.AddRange(_editSettingsManager.GetDefaultOrderListFromDB("NCS"));
        
                    var editWithDefaultDataSets = new Dictionary<string, List<string>>()
                    {
                        [AOM] = defaultDataSetsForAOM,
                        [UNB] = defaultDataSetsForUNB,
                        [NCS] = defaultDataSetsForNCS
                    };*/

                    try
                    {
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.All.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();

                        var editsForWhichFallBackShouldDisplay =
                            _editSettingsManager.GetListOfEditsRequiringFallBackOrderFromDB();

                        var editWhichIsNotPciNonCoveredService = paramList["EditWhichIsNotPciNonCoveredService"];

                        StringFormatter.PrintMessageTitle(
                            "Verifying whether fallback order settings is not displayed for edit not requiring fallback");
                        _editSettingsManager.ClickOnEditIconByFlag("ACW");
                        _editSettingsManager.IsFallBackOrderSettingsLabelPresent()
                            .ShouldBeFalse(
                                $"Fallback Order Settings should be shown for {editWhichIsNotPciNonCoveredService}");
                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();


                        #region CAR-3034

                        StringFormatter.PrintMessage(
                            "Verifying all data sets for edit which is NotPciNonCoveredService");
                        _editSettingsManager.ClickOnEditIconByFlag(editList[0]);
                        _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm().ShouldCollectionBeEquivalent(
                            dataSetsForEditWhichIsNotPciNonCoveredService,
                            "Data sets for edits which is not pci non-covered service should match");
                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();

                        StringFormatter.PrintMessage("Verifying all data sets for edit which is PciUnbundle");
                        _editSettingsManager.ClickOnEditIconByFlag(editList[1]);
                        _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm().ShouldCollectionBeEquivalent(
                            dataSetsForEditWhichIsPciUnbundle,
                            "Data sets for edits which is pci unbundle service should match");
                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();

                        StringFormatter.PrintMessage("Verifying all data sets for edit which is PciNonCoveredService");
                        _editSettingsManager.ClickOnEditIconByFlag(editList[2]);
                        _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm().ShouldCollectionBeEquivalent(
                            dataSetsForEditWhichIsPciNonCoveredService,
                            "Data sets for edits which is pci non-covered service should match");
                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();

                        #endregion

                        //    StringFormatter.PrintMessageTitle("Comparing the currently used datasets with the Database");
                        //    foreach (var edit in editList)
                        //    {
                        //        editsForWhichFallBackShouldDisplay.ShouldContain(edit,$"{edit} should have Fall back order enabled");
                        //        _editSettingsManager.ClickOnEditIconByFlag(edit);

                        //        _editSettingsManager.IsFallBackOrderSettingsLabelPresent()
                        //            .ShouldBeTrue($"Fallback order section should be shown for correct edit : {edit}");

                        //        var countOfActiveFallBackOrders = _editSettingsManager.GetCountOfActiveFallBackOrdersFromForm();
                        //        var dataSetWithfallBackOrderFromDB =
                        //            _editSettingsManager.GetDataSetsWithFallBackOrderFromDB(edit);
                        //        var activeDataSets = _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm()
                        //            .GetRange(0, countOfActiveFallBackOrders);

                        //        activeDataSets.ShouldCollectionBeEqual(
                        //            dataSetWithfallBackOrderFromDB.Select(dataSetWithFallBack => dataSetWithFallBack.Key.ToList()),
                        //            "The active data sets should match with the database");

                        //        StringFormatter.PrintMessage("Validation of the default datasets");
                        //        _editSettingsManager.ClickUseDefaultOrCurrentButton();
                        //        _editSettingsManager.GetButtonTextOfUseDefaultOrCurrentBtn().ShouldBeEqual("Use Current");

                        //        _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                        //    }

                        //    StringFormatter.PrintMessageTitle("Verification of updating the order settings and saving the changes");
                        //    _editSettingsManager.ClickOnEditIconByFlag(AOM);
                        //    var allDataSets = _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm();
                        //    var originallyUsedDataSets = _editSettingsManager.GetAllActiveDataSetsFromTheForm();
                        //    var originallyUnusedDataSets = allDataSets.Except(originallyUsedDataSets).ToList();
                        //    int offset = 1;

                        //    foreach (var dataSet in originallyUsedDataSets)
                        //        _editSettingsManager.ClickOnCheckboxByDataSetName(dataSet);

                        //    for (int count = 0; count < originallyUnusedDataSets.Count(); count++)
                        //    {
                        //        _editSettingsManager.ClickOnCheckboxByDataSetName(originallyUnusedDataSets.ElementAt(count));
                        //        _editSettingsManager.SetFallBackorderForDataSet(originallyUnusedDataSets.ElementAt(count),
                        //            (count + offset).ToString());
                        //        offset++;
                        //    }

                        //    _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        //    _editSettingsManager.ClickOnEditIconByFlag(AOM);

                        //    var sysdate = DateTime.ParseExact(_editSettingsManager.GetSystemDateFromDatabase(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                        //    var auditInformation = _editSettingsManager.GetLatestFallbackOrderSettingsAuditForDataSets(originallyUnusedDataSets, "uiautomation");

                        //    _editSettingsManager.GetAllActiveDataSetsFromTheForm().ShouldCollectionBeEqual(originallyUnusedDataSets,
                        //        "Updated edit datasets should be saved");

                        //    StringFormatter.PrintMessage("Verification of the saved values as well as audit table");
                        //    for (int i = 0; i < originallyUnusedDataSets.Count(); i++)
                        //    {
                        //        var dataSet = originallyUnusedDataSets.ElementAt(i);
                        //        _editSettingsManager.GetFallBackOrderForDataSet(dataSet)
                        //            .ShouldBeEqual((i + 1).ToString(), "The order should be saved correctly");

                        //        _editSettingsManager.ClickOnCheckboxByDataSetName(dataSet);

                        //        DateTime.ParseExact(auditInformation[dataSet], "M/d/yyyy h:m:s tt", CultureInfo.InvariantCulture).AssertDateRange(sysdate.AddMinutes(-1),
                        //            sysdate.AddMinutes(1), "Audit information should be saved in the table once changes are saved");
                        //    }

                        //    _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                        //    _editSettingsManager.ClickOnEditIconByFlag(AOM);
                        //    VerifyIfCancelDoesNotSaveChanges(originallyUnusedDataSets);
                    }

                    finally
                    {
                        if (_editSettingsManager.IsPageErrorPopupModalPresent())
                            _editSettingsManager.ClosePageError();

                        if (_editSettingsManager.IsFallBackOrderSettingsLabelPresent())
                            _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                    }
                }
            }

            [Test] //CAR-1783 (CAR-2046)
            public void Verify_fall_back_order_form_input_validation()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var flag = "AOM";

                    try
                    {
                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.All.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();

                        _editSettingsManager.ClickOnEditIconByFlag(flag);

                        var countOfActiveFallBackOrders = _editSettingsManager.GetCountOfActiveFallBackOrdersFromForm();

                        StringFormatter.PrintMessageTitle("Verifying the initial Fall Back Order for the edit flag");
                        _editSettingsManager.IsFallBackOrderSettingsLabelPresent()
                            .ShouldBeTrue("The form should show fall back order settings section");

                        countOfActiveFallBackOrders
                            .ShouldBeGreater(0, "At least one fall back order should be selected by default");

                        StringFormatter.PrintMessageTitle("Validation of the fallback order input field");
                        var dataSetWithfallBackOrderFromDB =
                            _editSettingsManager.GetDataSetsWithFallBackOrderFromDB(flag);

                        foreach (KeyValuePair<string, string> entry in dataSetWithfallBackOrderFromDB)
                        {
                            var dataSetName = entry.Key;
                            var dataSetFallBackOrder = entry.Value;

                            _editSettingsManager.IsDataSetInputFieldEnabled(dataSetName)
                                .ShouldBeTrue(
                                    $"The 'Set order' should be enabled for the active dataset {dataSetName}");
                            _editSettingsManager.GetFallBackOrderForDataSet(dataSetName)
                                .ShouldBeEqual(dataSetFallBackOrder,
                                    "The fallback order should match with the database value");

                            _editSettingsManager.SetFallBackorderForDataSet(dataSetName, "0");
                            _editSettingsManager.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue("Error message should pop up when value entered is less than 1");
                            _editSettingsManager.GetPageErrorMessage().ShouldBeEqual("Value cannot be less than 1.");
                            _editSettingsManager.ClosePageError();

                            _editSettingsManager.SetFallBackorderForDataSet(dataSetName, "100");
                            _editSettingsManager.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue("Error message should pop up when value entered is greater than 99");
                            _editSettingsManager.GetPageErrorMessage()
                                .ShouldBeEqual("Value cannot be greater than 99.");
                            _editSettingsManager.ClosePageError();

                            _editSettingsManager.SetFallBackorderForDataSet(dataSetName, "a");
                            _editSettingsManager.IsPageErrorPopupModalPresent()
                                .ShouldBeTrue("Error message should pop up when value entered non-numeric");
                            _editSettingsManager.GetPageErrorMessage().ShouldBeEqual("Only numbers allowed.");
                            _editSettingsManager.ClosePageError();

                            _editSettingsManager.ClickOnCheckboxByDataSetName(dataSetName);
                            _editSettingsManager.IsDataSetInputFieldEnabled(dataSetName)
                                .ShouldBeFalse("Unselecting the checkbox should set the 'Set order' field as disabled");
                            _editSettingsManager.GetFallBackOrderForDataSet(dataSetName)
                                .ShouldBeNullorEmpty(
                                    "The 'Set order' will be cleared out when the dataset is deselected");
                        }

                        StringFormatter.PrintMessage("At least one dataset must be selected for updates to be saved");
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("An error message should pop up if all the datasets are deselected");
                        _editSettingsManager.GetPageErrorMessage()
                            .ShouldBeEqual("A minimum of one dataset must be selected for use.");
                        _editSettingsManager.ClosePageError();

                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();

                        StringFormatter.PrintMessageTitle("Order number cannot be repeated for two datasets");
                        _editSettingsManager.ClickOnEditIconByFlag(flag);
                        var dataSetsForFallBack = _editSettingsManager.GetAllDataSetsFromModifyEditSettingsForm();
                        var firstFallbackOrder =
                            _editSettingsManager.GetFallBackOrderForDataSet(dataSetsForFallBack[0]);
                        _editSettingsManager.SetFallBackorderForDataSet(dataSetsForFallBack[1], firstFallbackOrder);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("Error message should pop up for duplicate order values");
                        _editSettingsManager.GetPageErrorMessage()
                            .ShouldBeEqual("Duplicate values cannot be selected in Set Order. Please revise.");
                        _editSettingsManager.ClosePageError();

                        StringFormatter.PrintMessageTitle("All the enabled datasets should have an order value");
                        _editSettingsManager.SetFallBackorderForDataSet(dataSetsForFallBack[0], Empty);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                        _editSettingsManager.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("All the enabled datasets should have an order value");
                        _editSettingsManager.GetPageErrorMessage()
                            .ShouldBeEqual(
                                "Set Order value is required for all datasets in use. Please enter a value.");
                        _editSettingsManager.ClosePageError();
                    }
                    finally
                    {
                        if (_editSettingsManager.IsPageErrorPopupModalPresent())
                            _editSettingsManager.ClosePageError();

                        if (_editSettingsManager.IsFallBackOrderSettingsLabelPresent())
                            _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                    }
                }
            }

            [Test] //CAR-1789 (CAR-2081)
            public void Verify_FRE_and_FOT_ancillary_settings()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    var expectedValuesInDropdown = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                        TestName, "ExpectedValuesInDropdown", "Value").Split(';').ToList();

                    StringFormatter.PrintMessageTitle("Verification of the ancillary settings for FOT");
                    VerifyAncillarySettings("FOT");

                    StringFormatter.PrintMessageTitle("Verification of the ancillary settings for FRE");
                    VerifyAncillarySettings("FRE");

                    void VerifyAncillarySettings(string edit)
                    {
                        var label = edit == "FOT" ? "FOT Val" : "FRE Val";

                        _editSettingsManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status",
                            EditStatusEnum.All.GetStringValue());
                        _editSettingsManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _editSettingsManager.WaitForWorking();

                        StringFormatter.PrintMessage($"Verification of label and dropdown values for {label}");
                        _editSettingsManager.ClickOnEditIconByFlag(edit);
                        _editSettingsManager.IsLabelCorrectForAncillarySetting(label)
                            .ShouldBeTrue($"Label should be correctly shown for {label}");

                        var initiallySelectedOption =
                            _editSettingsManager.GetSideWindow().GetDropDownInputFieldByLabel(label);

                        initiallySelectedOption.ShouldBeEqual(
                            _editSettingsManager.AncillarySettingValuesForFOTandFREfromDB(edit),
                            $"Initial {label} value should default to previously saved value");
                        _editSettingsManager.GetSideWindow().GetDropDownList(label).ShouldCollectionBeEqual(
                            expectedValuesInDropdown,
                            $"Drop down list for {label} contains the correct list");

                        StringFormatter.PrintMessage(
                            $"Verification of updating and saving the {label} value for {edit}");
                        var newList = expectedValuesInDropdown.Where(s => s != initiallySelectedOption).ToList();
                        var random = new Random();
                        _editSettingsManager.GetSideWindow()
                            .SelectDropDownValue(label, newList[random.Next(newList.Count)]);

                        var newlySelectedOption =
                            _editSettingsManager.GetSideWindow().GetDropDownInputFieldByLabel(label);
                        _editSettingsManager.ClickSaveInModifyEditSettingsForm();

                        newlySelectedOption.ShouldBeEqual(
                            _editSettingsManager.AncillarySettingValuesForFOTandFREfromDB(edit),
                            $"Updated {label} value should be successfully saved to the database");

                        StringFormatter.PrintMessage(
                            $"Verifying whether changes are not saved when cancel button is clicked in {label}");
                        _editSettingsManager.ClickOnEditIconByFlag(edit);
                        _editSettingsManager.GetSideWindow()
                            .SelectDropDownListValueByLabel(label, initiallySelectedOption);
                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();

                        _editSettingsManager.ClickOnEditIconByFlag(edit);
                        newlySelectedOption.ShouldBeEqual(
                            _editSettingsManager.GetSideWindow().GetDropDownInputFieldByLabel(label),
                            "Updated values should not be saved when Cancel button is clicked");

                        _editSettingsManager.ClickCancelInModifyEditSettingsForm();
                    }
                }
            }

            [Test] //CAR-2736(CAR-2813)
            public void Verify_retain_search_criteria_and_results()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    ClientSearchPage _clientSearchPage;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    _clientSearchPage = _editSettingsManager.ClickOnReturnToClientSearch();
                    _clientSearchPage.WaitForWorkingAjaxMessage();
                    _clientSearchPage.SideBarPanelSearch.SetInputFieldByLabel("Client Name",
                        ClientEnum.SMTST.GetStringValue());
                    _clientSearchPage.SideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                        ProductEnum.CV.ToString());
                    _clientSearchPage.SideBarPanelSearch.ClickOnFindButton();
                    var previousSearchResultList = _clientSearchPage.GetGridViewSection.GetGridAllRowData();
                    var clientCode = _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Client Code");
                    var clientStatus = _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Client Status");
                    var product = _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Product");
                    _clientSearchPage.GetGridViewSection.ClickOnGridRowByRow();
                    _clientSearchPage.ClickOnEditSettingsIcon();

                    _clientSearchPage = _editSettingsManager.ClickOnReturnToClientSearch();
                    _clientSearchPage.WaitForWorkingAjaxMessage();

                    StringFormatter.PrintMessage("Verify search criteria is retained");
                    _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Client Status").ShouldBeEqual(
                        clientStatus,
                        "Client Status value should remain intact once return from edit settings manager page.");
                    _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Client Code").ShouldBeEqual(
                        clientCode,
                        "Client Code value should remain intact once return from edit settings manager page.");
                    _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual(
                        product,
                        "Product value should remain intact once return from edit settings manager page.");
                    _clientSearchPage.SideBarPanelSearch.GetInputValueByLabel("Client Name").ShouldBeEqual(
                        ClientEnum.SMTST.GetStringValue(),
                        "Client Name value should remain intact once return from edit settings manager page.");

                    StringFormatter.PrintMessage("Verify Search result is retained");
                    _clientSearchPage.GetGridViewSection.GetGridAllRowData()
                        .ShouldCollectionBeEqual(previousSearchResultList, "Search Result is retained");
                }
            }

            [Test] //TE-919
            public void Verify_Ancillary_Settings_For_INVD_Edit()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun())
                {
                    EditSettingsManagerPage _editSettingsManager;
                    automatedBase.CurrentPage =
                        _editSettingsManager = automatedBase.QuickLaunch.NavigateToEditSettingsManager();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    const string flag = "INVD";
                    var labelList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ancillary_settings_INVD").Values
                        .ToList();
                    _editSettingsManager.UpdateAncillarySettingsStatus();
                    _editSettingsManager.SearchForEdit("Edit", flag);
                    _editSettingsManager.ClickOnEditIconByFlag(flag);
                    _editSettingsManager.GetAncillarySettingsLabelForINVD()
                        .ShouldCollectionBeEqual(labelList, "label displayed correct?");
                    _editSettingsManager.ClickAncillarySettingsRadioButtons(2);
                    for (int i = 2; i <= 4; i++)
                    {
                        _editSettingsManager.IsAncillaryRadioButtonEnabled(2, i)
                            .ShouldBeFalse("Radio button should be independent");
                    }

                    StringFormatter.PrintMessage("verify correct data stored in database for Ancillary settings");
                    for (int i = 2; i <= 4; i++)
                    {
                        _editSettingsManager.ClickAncillarySettingsRadioButtons(2, i);
                    }

                    _editSettingsManager.ClickSaveInModifyEditSettingsForm();
                    var ancillarySettingsStatus = _editSettingsManager.GetAncillarySettingsStatusFromDb();
                    ancillarySettingsStatus.All(x => x.Equals("F")).ShouldBeTrue("correct data stored?");
                }
            }

        [Test,Category("Acceptance"),Category("OnDemand")] //CV-8862(CV-7469)
        [Author("Shreya P")]
        public void Verify_Fallback_Settings_Based_On_LOB()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                EditSettingsManagerPage _editSettingsManager;
                _editSettingsManager = automatedBase.CurrentPage.NavigateToEditSettingsManager();

                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string flag = paramList["Flag"];
               
                var LOBList = paramList["LOB"].Split(',').ToList();
                var MedicaidSettingsList = paramList["MedicaidFallBackSetting"].Split(',').ToList();
                var MedicareSettingsList = paramList["MedicareFallbackSetting"].Split(',').ToList();
                var labelList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ancillary_settings_INVD").Values
                    .ToList();
                try
                {
                    foreach (var lob in LOBList)
                    {
                        _editSettingsManager.UpdateLOBvalueForClient(lob, ClientEnum.SMTST.ToString());
                        _editSettingsManager.RefreshPage();
                        _editSettingsManager.SearchForEdit("Edit", flag);
                        _editSettingsManager.ClickOnEditIconByFlag(flag);
                        var fallBackSettings = _editSettingsManager.GetFallbackSettingsLabel();
                        if (lob == "MEDICAID")
                        {
                            MedicaidSettingsList.ShouldCollectionBeEquivalent(fallBackSettings,
                                $"fall back setting correct for {lob}?");
                        }
                        else
                        {
                            MedicareSettingsList.ShouldCollectionBeEquivalent(fallBackSettings,
                                $"fall back setting correct for {lob}?");
                        }
                    }


                }
                finally
                {
                    _editSettingsManager.UpdateLOBvalueForClient(LOBList[0], ClientEnum.SMTST.ToString());
                }
                
            }
        }

            #endregion
        

    }
}
