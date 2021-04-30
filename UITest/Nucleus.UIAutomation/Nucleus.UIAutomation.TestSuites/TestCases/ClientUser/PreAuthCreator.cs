using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;


namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class PreAuthCreator : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private PreAuthCreatorPage _preAuthCreator;
        private OldUserProfileSearchPage _userProfileSearch;
        private ProfileManagerPage _profileManager;
        private QuickLaunchPage _quickLaunch;
        private PreAuthSearchPage _preAuthSearch;
        private PreAuthActionPage _preAuthAction;
        private CommonValidations _commonValidation;
        private readonly string _dciPreAuthorization = RoleEnum.DCIAnalyst.GetStringValue();
        #endregion


        #region PROTECTED PROPERTIES
        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }


        #endregion

        #region OVERRIDE
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _preAuthCreator = QuickLaunch.NavigateToPreAuthCreatorPage();
                _commonValidation = new CommonValidations(CurrentPage);
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }
        protected override void ClassCleanUp()
        {
            try
            {

            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _preAuthCreator = _preAuthCreator.Logout().LoginAsClientUser().NavigateToPreAuthCreatorPage();
            }

            if (_preAuthCreator.GetPageHeader() != PageHeaderEnum.PreAuthCreator.GetStringValue())
            {
                _preAuthCreator.ClickOnQuickLaunch().NavigateToPreAuthCreatorPage();
            }
            _preAuthCreator.ClickOnClearLink();
            _preAuthCreator.ClickOnCancelLink();
            _preAuthCreator.ClickOkCancelOnConfirmationModal(true);
            _preAuthCreator.ClickOnSelectOptionTabButton("Select Patient");
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES
        
        [Test]//CAR-1287(CAR-919)
        public void Verify_find_and_select_provider_functionality()
        {
            try
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var provSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ProviderSequence", "Value");
                var list = _preAuthCreator.GetBasicProviderInfo(provSeq);
                var providerNumber = list[0];
                var firstName = list[1];
                var lastName = list[2];
                var tin = list[3];
                var providerName = list[4];

                StringFormatter.PrintMessageTitle("Verification of Popup Message for Empty and other Criteria");
                _preAuthCreator.ClickOnSelectOptionTabButton("Select Provider");
                _preAuthCreator.ClickOnClearLink();
                _preAuthCreator.ClickOnButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "Search cannot be initiated without any criteria entered.",
                    "Verification of Popup Message when No Value Entered in any Input Fields");
                _preAuthCreator.ClosePageError();

                _preAuthCreator.SetInputFieldByLabel("First Name", "Test");
                _preAuthCreator.WaitForStaticTime(2000);
                _preAuthCreator.ClickOnButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "First and last name are required when searching by name.",
                    "Verification of Popup Message when Searched by First Name Only");
                _preAuthCreator.ClosePageError();
                _preAuthCreator.ClickOnClearLink();

                _preAuthCreator.SetInputFieldByLabel("Last Name", "Test");
                _preAuthCreator.WaitForStaticTime(500);
                _preAuthCreator.ClickOnButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "First and last name are required when searching by name.",
                    "Verification of Popup Message when Searched by First Name Only");
                _preAuthCreator.ClosePageError();
                _preAuthCreator.ClickOnClearLink();

                StringFormatter.PrintMessageTitle("Verification of No Result Found");
                _preAuthCreator.SetInputFieldByLabel("First Name", "TEST");
                _preAuthCreator.SetInputFieldByLabel("Last Name", "TEST");
                _preAuthCreator.WaitForStaticTime(500);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.GetNoResultFoundMessageByHeaderlabel("Provider Results")
                    .ShouldBeEqual("No results found", "Verify No Result Found Message");
                _preAuthCreator.ClickOnClearLink();

                StringFormatter.PrintMessageTitle("Verification of Search Result when search by each input field");

                VerifyProviderSearchResult("Provider Number", providerNumber);
                VerifyProviderSearchResult("TIN", tin);
                VerifyProviderSearchResult("Provider Sequence", provSeq);


                _preAuthCreator.SetInputFieldByLabel("First Name", firstName);
                _preAuthCreator.SetInputFieldByLabel("Last Name", lastName);
                _preAuthCreator.WaitForStaticTime(500);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.IsGridRowByHeaderLabelPresent("Provider Results")
                    .ShouldBeTrue("Is Search Result Found when search by First Name and Last Name?");
                if (_preAuthCreator.IsPageErrorPopupModalPresent()) _preAuthCreator.ClosePageError();
                StringFormatter.PrintMessageTitle("Verify the Search Result on Left Grid");

                _preAuthCreator.GetLabelOnLeftWindowGridByHeaderLabel("Provider Results", 1)
                    .ShouldBeEqual("Prov Num:", "Is Provider Number Label match?");
                _preAuthCreator.IsLabelOnLeftWindowGridByHeaderLabelPresent("Provider Results", 2)
                    .ShouldBeFalse("Is Label Present for Provider Name?");
                _preAuthCreator.GetLabelOnLeftWindowGridByHeaderLabel("Provider Results", 3)
                    .ShouldBeEqual("TIN:", "Is Provider Number Label match?");
                _preAuthCreator.GetLabelOnLeftWindowGridByHeaderLabel("Provider Results", 4)
                    .ShouldBeEqual("Provider Seq:", "Is Provider Number Label match?");

                StringFormatter.PrintMessageTitle("Verify the Display Provider Result on Create Pre-Authorization Window");

                _preAuthCreator.ClickOnGridRowByHeaderLabel("Provider Results");
                _preAuthCreator.GetSideWindow.GetValueByLabel("Provider Number").ShouldBeEqual(providerNumber,
                    "Is Correct Provider Number Display in Create Pre-Authorization side window?");
                _preAuthCreator.GetSideWindow.GetValueByLabel("Provider Name").ShouldBeEqual(providerName,
                    "Is Correct Provider Name Display in Create Pre-Authorization side window?");
                _preAuthCreator.GetSideWindow.GetValueByLabel("TIN").ShouldBeEqual(tin,
                    "Is Correct TIN Display in Create Pre-Authorization side window?");
                _preAuthCreator.GetSideWindow.GetValueByLabel("Provider Sequence").ShouldBeEqual(provSeq,
                    "Is Correct Provider Sequence Display in Create Pre-Authorization side window?");

            }
            finally
            {
                _preAuthCreator.ClickOnClearLink();
                _preAuthCreator.ClickOnCancelLink();
                _preAuthCreator.ClickOkCancelOnConfirmationModal(true);
            }
        }

        private void VerifyProviderSearchResult(string label, string value)
        {
            _preAuthCreator.SetInputFieldByLabel(label, value);
            _preAuthCreator.ClickOnButton();
            _preAuthCreator.WaitForWorkingAjaxMessage();

            _preAuthCreator.IsGridRowByHeaderLabelPresent("Provider Results")
                .ShouldBeTrue(string.Format("Is Search Result Found when searched by {0}?", label));

            _preAuthCreator.WaitForStaticTime(2000);
            _preAuthCreator.ClickOnClearLink();
        }

        [Test] //CAR-1217[CAR-263]
        public void Verify_security_and_navigation_to_PreAuthCreator_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            
            _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.PreAuthorization, new List<string> { SubMenu.PreAuthCreator, SubMenu.PreAuthSearch },
                _dciPreAuthorization, new List<string> { PageHeaderEnum.PreAuthCreator.GetStringValue(), PageHeaderEnum.PreAuthSearch.GetStringValue() },
                Login.LoginAsClientUserWithNoAnyAuthorityAndRedirectToQuickLaunch, new[] { "uiautomation", "noauthority" }, Login.LoginAsClientUser);

        }

       
        [Test] //CAR-982 + CAR-3165(CAR-3132)
        public void Verify_create_and_add_pre_auth_line_items_functionality()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            const string addLines = "Add Lines";
            var label = paramLists["Labels"].Split(',').ToList();
            var scenarioList = paramLists["Scenario"].Split(',').ToList();
            var lineNumberOrderBeforeLineDeletion = paramLists["LineOrderBeforeLineDeletion"].Split(',').ToList();
            var lineNumberOrderAfterLineDeletion = paramLists["LineOrderAfterLineDeletion"].Split(',').ToList();

            try
            { 
                _preAuthCreator.ClickOnSelectOptionTabButton(addLines);

                var DOS = _preAuthCreator.GetInputFieldValueByLabel("DOS");

                _preAuthCreator.GetLabelsOfInputFieldOnLeftSideOfAddLine()
                    .ShouldCollectionBeEqual(label, "Labels should be present");
                VerifyDefaultvalues(DOS); //Verification of default values

                StringFormatter.PrintMessage("Set value and verification of validity of the inputs allowed");

                _preAuthCreator.SelectDateInAddLine(1);
                SetInputInAddLineWithErrorPopUp("Proc Code", "!", "Only alphanumerics allowed.", "ABCD12", "ABCD1",
                    "Only 5 digit alphanumeric value is allowed");
                SetInputInAddLineAndVerify("TN", "Z11", "Z1", "Only 2 digit alphanumeric value is allowed");
                SetInputInAddLineAndVerify("TS", "ABC123", "ABC12", "Only 5 digit alphanumeric value is allowed");
                SetInputInAddLineAndVerify("OC", "ABC", "AB", "Only 2 characters are allowed");
                SetInputInAddLineWithErrorPopUp("Billed", "A", "Only numbers allowed.", "9999999999999",
                    "$999,999,999,999.00",
                    "Only 12 digit alphanumeric value is allowed");
                SetInputInAddLineWithErrorPopUp("Paid", "A", "Only numbers allowed.", "9999999999999",
                    "$999,999,999,999.00",
                    "Only 12 digit alphanumeric value is allowed");

                StringFormatter.PrintMessage("Verification of Scenario input field");
                #region CAR-3165(CAR-3132)

                _preAuthCreator.GetDefaultValueOfScenario()
                    .ShouldBeEqual("Please select", "Default value should be 'Please Select'");
                _preAuthCreator.ClickOnScenarioDropDown();
                _preAuthCreator.GetScenarioListInAddLine() 
                    .ShouldCollectionBeEqual(scenarioList, "The options should match");
                _preAuthCreator.SelectDropDownListValueByLabel("Scenario",scenarioList[1]);
                #endregion 

                StringFormatter.PrintMessage("Verification of add button functionality and verification if valid value is being added");

                ClickAddVerifyErrorMessageAndSetInput("ABCD1 is not a valid proc code.", "Proc Code", "D1201");
                ClickAddVerifyErrorMessageAndSetInput("Z1 is not a valid TN.", "TN", "ZZ");
                ClickAddVerifyErrorMessageAndSetInput("ZZ is not a valid TN.", "TN", "AS");
                ClickAddVerifyErrorMessageAndSetInput("AB is not a valid OC.", "OC", "UR");
                

                StringFormatter.PrintMessage("Verification of Clear functionality");
                _preAuthCreator.ClickOnClearLink();
                _preAuthCreator.WaitForStaticTime(1000);
                VerifyDefaultvalues(DOS);


                StringFormatter.PrintMessage("Verification of DOS, TN, TS, OC, Paid, Scenario values are not necessary");

                _preAuthCreator.IsLabelRequiredField("DOS").ShouldBeFalse("DOS is not required");
                _preAuthCreator.IsLabelRequiredField("Proc Code").ShouldBeTrue("Proc Code is required");
                _preAuthCreator.IsLabelRequiredField("TN").ShouldBeFalse("TN is not required");
                _preAuthCreator.IsLabelRequiredField("TS").ShouldBeFalse("TS is not required");
                _preAuthCreator.IsLabelRequiredField("OC").ShouldBeFalse("OC is not required");
                _preAuthCreator.IsLabelRequiredField("Billed").ShouldBeTrue("Billed is required");
                _preAuthCreator.IsLabelRequiredField("Paid").ShouldBeTrue("Paid is required");
                _preAuthCreator.IsLabelRequiredField("Scenario").ShouldBeFalse("Scenario is not required");
                

                StringFormatter.PrintMessage("Verification of DOS, TN, TS, OC, Paid, Scenario values are not necessary by setting input");

                ClickAddVerifyErrorMessageAndSetInput("Proc code is required.", "Proc Code", "D1201");
                ClickAddVerifyErrorMessageAndSetInput("Billed amount should be greater than $0.00.", "Billed",
                    "9999999999999");
                ClickAddVerifyErrorMessageAndSetInput("Paid amount should be greater than $0.00.", "Paid",
                    "9999999999999");

                
                var line1ProcCode = _preAuthCreator.GetInputFieldValueByLabel("Proc Code");
                var tn = _preAuthCreator.GetInputFieldValueByLabel("TN");
                var ts = _preAuthCreator.GetInputFieldValueByLabel("TS");
                var oc = _preAuthCreator.GetInputFieldValueByLabel("OC");
                var billedAmount = _preAuthCreator.GetInputFieldValueByLabel("Billed");
                var paidAmount = _preAuthCreator.GetInputFieldValueByLabel("Paid");
                var scenario = _preAuthCreator.GetScenarioInputFieldValue();

                _preAuthCreator.ClickOnButton();
                _preAuthCreator.WaitForWorking();
                VerifyDefaultvalues(DOS); //Verification if the value is set to default after a line has been added

                StringFormatter.PrintMessage("Verification if the entered value in the add line form matches with the value displayed after line has been added in the added line list");

                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 3).ShouldBeEqual(DOS, "DOS values should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 4).ShouldBeEqual(line1ProcCode,
                    "Proc Codes should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 5)
                    .ShouldBeEqual(_preAuthCreator.GetProcCodeDescription(line1ProcCode,excludeExpire:true),
                        "Proc Code short description should be correct");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 6).ShouldBeEqual(tn, "TN values should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 7).ShouldBeEqual(ts, "TS values should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 8).ShouldBeEqual(oc, "OC values should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 9).ShouldBeEqual(billedAmount,
                    "Billed amount values should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 10).ShouldBeEqual(paidAmount,
                    "Paid amount should match");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 11).ShouldBeEqual(scenario,
                    "Scenario values should match");

                AddLines("D0417", "02", "12", "UL", "1", "1", "B",true); //addition of another line


                var line2Proccode = _preAuthCreator.GetValueOfAddedLineInRightWindow(2, 4);

                StringFormatter.PrintMessage("Verification of the required fields of the added line in right side and the criterias they should meet");

                _preAuthCreator.IsDeleteIconPresentInAddedLine(1).ShouldBeTrue("Delete line icon must be present");
                _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 2).ShouldBeEqual("1", "Line number should be present denoting the number of line");
                LabelVerificationInAddLine(); // Verification if labels are present 
                _preAuthCreator.IsValueCurrency(2, 9, 1).ShouldBeTrue("Value should be currency");
                _preAuthCreator.IsValueCurrency(2, 10, 1).ShouldBeTrue("Value should be currency");


                AddLines("D0120", "12", "ABC1", "UR", "90", "76", "C",true); //addition of another line


                StringFormatter.PrintMessage("Verification of the line order of added line, delete funtionality and rearrangement of line number after deletion");

                _preAuthCreator.GetGridViewSection.GetGridListValueByCol(2)
                    .ShouldBeEqual(lineNumberOrderBeforeLineDeletion,
                        "Line number should be ordered on the basis of line number starting from 1");
                _preAuthCreator.IsAddedLineNumberSorted().ShouldBeTrue("Line number should be in ascending order starting from 1");
                _preAuthCreator.GetRowCountOfAddedLine(2).ShouldBeEqual(3, "number of added lines should be 3");
                _preAuthCreator.DeleteLineNumber(2, 1);
                _preAuthCreator.WaitForStaticTime(1000);
                line2Proccode.ShouldNotBeEqual(_preAuthCreator.GetValueOfAddedLineInRightWindow(2, 4),
                    "Proc Codes should not match"); // to verify the line has been deleted
                _preAuthCreator.GetGridViewSection.GetGridListValueByCol(2)
                    .ShouldBeEqual(lineNumberOrderAfterLineDeletion, "Line number should be rearranged");
                _preAuthCreator.IsAddedLineNumberSorted().ShouldBeTrue("Line number should be in ascending order starting from 1");
                _preAuthCreator.GetRowCountOfAddedLine(2).ShouldBeEqual(2, "Line number should decrease to 2 after deletion of a line");
            }

            finally
            {
                _preAuthCreator.ClickOnCancelLink();
                _preAuthCreator.ClickOkCancelOnConfirmationModal(true);
            }

        }


        [Test] //CAR-1169 (CAR-1285)
        public void Verify_Pre_Auth_Creator_Submit_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var patientMemberId = paramLists["PatientMemberId"];
            var providerNumber = paramLists["ProviderNumber"];
            var procCode = paramLists["ProcCode"];
            var tn = paramLists["TN"];
            var ts = paramLists["TS"];
            var oc = paramLists["OC"];
            var billed = paramLists["Billed"];
            var paid = paramLists["Paid"];
            var scenario = paramLists["Scenario"];
            var group = paramLists["Group"];
            var preAuthId = paramLists["PreAuthID"];
            var imageId = paramLists["ImageId"];
            try
            {
                StringFormatter.PrintMessageTitle("Validating the message popup with incomplete information");
                _preAuthCreator.ClickOnSubmitButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "A patient must be selected before a pre-auth can be created.",
                    "Verification of Popup Message when No Value Entered in Patient Information Fields");
                _preAuthCreator.ClosePageError();
                SetPatientInformation(patientMemberId, group);
                _preAuthCreator.ClickOnSubmitButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "A provider must be selected before a pre-auth can be created.",
                    "Verification of Popup Message when No Value Entered in Provider Information Fields");
                _preAuthCreator.ClosePageError();
                SetProviderInformation(providerNumber, preAuthId, imageId);
                _preAuthCreator.ClickOnSubmitButton();
                _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(
                    "At least one line item must be added before a pre-auth can be created.",
                    "Verification of Popup Message when No Lines are added.");
                _preAuthCreator.ClosePageError();
                AddLines(procCode, tn, ts, oc, billed, paid, scenario);
                StringFormatter.PrintMessageTitle("Verifying the 'Cancel' button functionality");
                var listOfInputValuesBeforeClickingCancel = _preAuthCreator.GetListOfAllValuesInTheRightWindowForm();
                _preAuthCreator.ClickOnCancelLink();
                _preAuthCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("A message will be shown to the user.");
                _preAuthCreator.GetPageErrorMessage()
                    .ShouldBeEqual("Selected values will be cleared from this form. Do you wish to continue?");
                _preAuthCreator.ClickOkCancelOnConfirmationModal(false);
                _preAuthCreator.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("Message popup will be closed");
                _preAuthCreator.GetListOfAllValuesInTheRightWindowForm().ShouldCollectionBeEqual(listOfInputValuesBeforeClickingCancel, "No changes should be made when message popup is canceled ");
                _preAuthCreator.ClickOnCancelLink();
                _preAuthCreator.ClickOkCancelOnConfirmationModal(true);
                var listOfNonEmptyValuesOnRightWindowForm = _preAuthCreator.GetListOfAllValuesInTheRightWindowForm().Where(s => s.Value != "").ToList();
                listOfNonEmptyValuesOnRightWindowForm.Count.ShouldBeEqual(0, "Input Fields should be reset to empty once OK is clicked on the Cancel message popup");
                StringFormatter.PrintMessage("Creating a valid Pre-Auth");
                SetPatientInformation(patientMemberId, group);
                SetProviderInformation(providerNumber, preAuthId, imageId);
                AddLines(procCode, tn, ts, oc, billed, paid, scenario);
                _preAuthCreator.ClickOnButton();
                var listOfInputValuesInRightWindowForm = _preAuthCreator.GetListOfAllValuesInTheRightWindowForm();
                var billedValueInAddedLine = _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 9);
                var paidValueInAddedLine = _preAuthCreator.GetValueOfAddedLineInRightWindow(1, 10);
                var tin = _preAuthCreator.GetValueByLabel("TIN:");
                _preAuthCreator.WaitForWorkingAjaxMessageForBothDisplayAndHide();
                _preAuthCreator.ClickOnSubmitButton();
                _preAuthCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("A message will be shown to the user.");
                var msg = _preAuthCreator.GetPageErrorMessage();
                _preAuthCreator.ClosePageError();
                Regex.Match(msg, "Pre-Auth [\\d]+ has been submitted for review.").Success.ShouldBeTrue("Does message popup show correct information after pre-auth is created?");
                var authSeq = Regex.Match(msg, @"\d+").Value;
                _preAuthCreator.GetCreateAuditNote(authSeq).ShouldBeEqual("Newly created Pre-Authorization.", "Audit record should be created.");               
                VerifyPreAuthActionValues(listOfInputValuesInRightWindowForm, authSeq, tin, group, imageId, preAuthId, tn, ts, oc, procCode, billedValueInAddedLine, paidValueInAddedLine, scenario);               
            }
            finally
            {
                _preAuthCreator = _preAuthAction.ClickOnQuickLaunch().NavigateToPreAuthCreatorPage();
            }
        }

        [Test] //CAR-920 (CAR-1216) // 'SSN' and 'Insured SSN' fields are removed as per CAR-1169
        public void Verify_Pre_Auth_Creator_Input_Fields()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var firstProvSeq = paramLists["FirstProviderSeq"];
            var firstMemberID = paramLists["FirstMemberID"];
            var secondProvSeq = paramLists["SecondProviderSeq"];
            var secondMemberID = paramLists["SecondMemberID"];

            var initialPatientInfoList = _preAuthCreator.GetPatientInformationByFromDBByMemberID(firstMemberID);
            var initialProviderInfoList = _preAuthCreator.GetProviderInformationByFromDBByMemberID(firstProvSeq);
            var secondPatientInfoList = _preAuthCreator.GetPatientInformationByFromDBByMemberID(secondMemberID);
            var secondProviderInfoList = _preAuthCreator.GetProviderInformationByFromDBByMemberID(secondProvSeq);

            try
            {
                _preAuthCreator.IsCreatePreAuthSectionDisplayed().ShouldBeTrue("Create Pre-Authorization section is displayed");

                StringFormatter.PrintMessage("Searching by Patient and Provider information and selecting the result");
                _preAuthCreator.SetInputFieldByLabel("Member ID", firstMemberID);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.ClickOnGridRowByHeaderLabel("Patient Results");

                _preAuthCreator.ClickOnSelectOptionTabButton("Select Provider");
                _preAuthCreator.SetInputFieldByLabel("Provider Sequence", firstProvSeq);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.ClickOnGridRowByHeaderLabel("Provider Results");

                StringFormatter.PrintMessageTitle("Verification of displayed values in the form once Patient and Provider is selected");
                VerifyInputFieldsShowCorrectValues(initialPatientInfoList, initialProviderInfoList);

                StringFormatter.PrintMessageTitle("Verification of the input fields which can be manually entered");
                _preAuthCreator.GetInputFieldValueByLabel("Group").ShouldBeNullorEmpty("Initial value of 'Group' field is empty");
                _preAuthCreator.SetInputFieldByLabel("Group", "Test Group");
                _preAuthCreator.GetInputFieldValueByLabel("Group").ShouldBeEqual("Test Group");
                _preAuthCreator.IsLabelRequiredField("Group").ShouldBeFalse("'Group' field is not a required field");

                _preAuthCreator.GetInputFieldValueByLabel("Pre-Auth ID").ShouldBeNullorEmpty("Initial value of 'Pre-Auth ID' field is empty");
                _preAuthCreator.SetInputFieldByLabel("Pre-Auth ID", "Test Pre-Auth ID");
                _preAuthCreator.GetInputFieldValueByLabel("Pre-Auth ID").ShouldBeEqual("Test Pre-Auth ID");
                _preAuthCreator.IsLabelRequiredField("Pre-Auth ID").ShouldBeFalse("'Pre-Auth ID' field is not a required field");

                _preAuthCreator.GetInputFieldValueByLabel("Doc Ref Num").ShouldBeNullorEmpty("Initial value of 'Doc Ref Num' field is empty");
                _preAuthCreator.SetInputFieldByLabel("Doc Ref Num", "Test Doc Ref Num");
                _preAuthCreator.GetInputFieldValueByLabel("Doc Ref Num").ShouldBeEqual("Test Doc Ref Num");
                _preAuthCreator.IsLabelRequiredField("Doc Ref Num").ShouldBeFalse("'Doc Ref Num' field is not a required field");

                StringFormatter.PrintMessageTitle("Verifying whether values are updated if a new provider and patient are selected");
                _preAuthCreator.ClickOnSelectOptionTabButton("Select Patient");

                _preAuthCreator.SetInputFieldByLabel("Member ID", secondMemberID);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.ClickOnGridRowByHeaderLabel("Patient Results");

                _preAuthCreator.ClickOnSelectOptionTabButton("Select Provider");
                _preAuthCreator.SetInputFieldByLabel("Provider Sequence", secondProvSeq);
                _preAuthCreator.ClickOnButton(true);
                _preAuthCreator.ClickOnGridRowByHeaderLabel("Provider Results");

                VerifyInputFieldsShowCorrectValues(secondPatientInfoList, secondProviderInfoList);

            }

            finally
            {
                _preAuthCreator.ClickOnClearLink();
                _preAuthCreator.ClickOnCancelLink();
                _preAuthCreator.ClickOkCancelOnConfirmationModal(true);
            }
        }


        [Test] //CAR-1100(CAR-1611)
        public void Verify_review_type_for_pre_auths_when_created_manually()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var patientMemberId = paramLists["PatientMemberId"];
            var providerNumber = paramLists["ProviderNumber"];
            var procCode1 = paramLists["ProcCode1"];
            var procCode2 = paramLists["ProcCode2"];
            var tn = paramLists["TN"];
            var ts = paramLists["TS"];
            var oc = paramLists["OC"];
            var billed = paramLists["Billed"];
            var paid = paramLists["Paid"];
            var preAuthId = paramLists["PreAuthID"];
            var docRefNum = paramLists["DocRefNum"];
            string[] reviewTypes = {"Professional", "Standard"};
            
            foreach (var reviewType in reviewTypes)
            {
                var expectedStatus = StatusEnum.Closed.GetStringDisplayValue();
                StringFormatter.PrintMessage(string.Format("Verification of {0} Review Type",reviewType));
                SetPatientInformation(patientMemberId);
                SetProviderInformation(providerNumber, preAuthId, docRefNum);
                if (reviewType == "Professional")//for professional and flagging logic we used common proc code and there is no link between review type and flagging logic.
                {
                    _preAuthCreator.IsReviewTypeProfessional(procCode1).ShouldBeTrue(
                        "If even one proc code on the pre-auth exists on this table and the client code is the current client and Apply =Y, the Review Type must be Professional ");
                    AddLines(procCode1, tn, "", "", billed, paid,"A",true);
                    expectedStatus = StatusEnum.CotivitiUnreviewed.GetStringDisplayValue();
                }
                else
                    _preAuthCreator.IsReviewTypeProfessional(procCode2).ShouldBeFalse(
                        "If there are no proc codes on the pre-auth that also exist in this table, the Review Type must be Standard");
                AddLines(procCode2, tn, ts, oc, billed, paid,"", save:true);
                _preAuthCreator.ClickOnSubmitButton();

                _preAuthCreator.IsPageErrorPopupModalPresent().ShouldBeTrue("A message will be shown to the user.");
                var msg = _preAuthCreator.GetPageErrorMessage();
                _preAuthCreator.ClosePageError();
                Regex.Match(msg, "Pre-Auth [\\d]+ has been submitted for review.").Success
                    .ShouldBeTrue("Does message popup show correct information after pre-auth is created?");
                var authSeq = Regex.Match(msg, @"\d+").Value;
              
                _preAuthSearch = _preAuthCreator.NavigateToPreAuthSearch();
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);

                #region CAR-1393(CAR-1515)

                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual(expectedStatus,
                    $"Status Should be {expectedStatus} if any flag is applied");
                if (expectedStatus == StatusEnum.CotivitiUnreviewed.GetStringDisplayValue())
                {
                    _preAuthAction.GetFlagListByDivList(isInternalUser:false).Count
                        .ShouldBeGreater(0, "Flag Should present for matching flagging logic");
                    _preAuthAction.GetClientDataSourceValue().ShouldNotBeEmpty("Source value should present");
                    
                }
                else
                    _preAuthAction.GetFlagListByDivList().Count.ShouldBeEqual(0, "Flag Should not present for not matching flagging logic");

                #endregion
                
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Review Type")
                    .ShouldBeEqual(reviewType, string.Format("Review Type should be {0} in action page", reviewType));
                _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthAction.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(reviewType, string.Format("Review Type should be {0} in search page", reviewType));
                _preAuthAction.NavigateToPreAuthCreatorPage();
            }
        }

        [Test] //TE-1184
        [Author("Shreya Pradhan")] 
        public void Verify_Document_Upload_Functionality()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string preautseq = null;
            var fileToUpload = testData["FileToUpload"];
            var expectedSelectedFileTypeList = testData["SelectedFileList"].Split(',').ToList();
            var providerNumber=testData["ProviderNumber"];
            var authhId = testData["PreAuthID"];
            var docRefNum = testData["DocRefNum"];
            var proccode= testData["ProcCode1"];
            var tin = testData["TN"];
            var ts=testData["TS"];
            var oc = testData["OC"];
            var billed = testData["Billed"];
            var paid = testData["Paid"];
            var patientseq = testData["PatientSeq"];
            try
            {
                _preAuthCreator.IsDocumentUploadIconPresent().ShouldBeTrue("icon present?");
                _preAuthCreator.ClickOnDocumentUploader();
                _preAuthCreator.IsUploadNewDocumentFormPresent()
                    .ShouldBeTrue("Upload Document Form Should Be Open ");
                _preAuthCreator.IsAddIconDisabledInDocumentUploadForm().ShouldBeTrue("Add Icon Should Be Disabled");

                StringFormatter.PrintMessage("Verify cancel link behaviour");
                _preAuthCreator.ClickOnCancelLinkOnUploadDocumentForm();
                _preAuthCreator.IsUploadNewDocumentFormPresent().ShouldBeFalse("Upload Document Form Should Be Closed");
                _preAuthCreator.IsAddIconDisabledInDocumentUploadForm().ShouldBeFalse("Add Icon Should Be Enabled");
                _preAuthCreator.ClickonAddIconInDocumentUploadForm();

                StringFormatter.PrintMessage("Verify Maximum character in description");
                var descp = new string('b', 105);
                _preAuthCreator.GetFileUploadPage.SetFileUploaderFieldValue("Description", descp);
                _preAuthCreator.GetFileUploadPage.GetFileUploaderFieldValue(2)
                    .Length.ShouldBeEqual(100, "Character length should not exceed more than 100 in Description");
                _preAuthCreator.GetFileUploadPage.IsAddFileButtonDisabled()
                    .ShouldBeTrue(
                        "Add file button should be disabled (unless atleast a file has been uploaded) must be true?");
                _preAuthCreator.GetFileUploadPage.ClickOnSaveUploadBtn();
                _preAuthCreator.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _preAuthCreator.GetPageErrorMessage()
                    .ShouldBeEqual("At least one file must be uploaded before the changes can be saved.",
                        "Expected error message on zero file type selection");
                _preAuthCreator.ClosePageError();

                StringFormatter.PrintMessage("verify  field validation");
                _preAuthCreator.GetFileUploadPage.GetAvailableFileTypeList()
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "File Type List Equal");
                _preAuthCreator.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                _preAuthCreator.GetFileUploadPage.GetPlaceHolderValue()
                    .ShouldBeEqual(expectedSelectedFileTypeList[0], "File Type Text");
                _preAuthCreator.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                _preAuthCreator.GetFileUploadPage.GetPlaceHolderValue()
                    .ShouldBeEqual("Multiple values selected", "File Type Text when multiple value selected");
                _preAuthCreator.GetFileUploadPage.GetSelectedFileTypeList()
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList.Take(2), "Selected File List Equal");
                _preAuthCreator.GetFileUploadPage.SetFileUploaderFieldValue("Description", "Test Description");
                _preAuthCreator.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthCreator.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthCreator.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse(
                        "Page error pop up should not be present if no description is set as its not a required field");

                StringFormatter.PrintMessage("Verify details of added file correct");
                _preAuthCreator.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                    .ShouldBeEqual(fileToUpload.Split(';')[0],
                        "Document Name  correct and present under files to upload");
                _preAuthCreator.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 3)
                    .ShouldBeEqual("Test Description",
                        "Document Description is correct and present under files to upload.");
                _preAuthCreator.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 4)
                    .ShouldBeEqual("Multiple File Types", "Document Type text when multiple File Types is selected.");
               
                var filesToUploadCount = _preAuthCreator.GetFileUploadPage.GetFilesToUploadCount();
                StringFormatter.PrintMessage("Verify Duplicate Files Can Not Be Added");
                _preAuthCreator.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                _preAuthCreator.GetFileUploadPage.SetFileUploaderFieldValue("Description", descp);
                _preAuthCreator.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthCreator.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthCreator.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _preAuthCreator.GetPageErrorMessage()
                    .ShouldBeEqual("Duplicate file is not added in the list.");
                _preAuthCreator.ClosePageError();
                _preAuthCreator.GetFileUploadPage.GetFilesToUploadCount()
                    .ShouldBeEqual(filesToUploadCount, "count should be same");
                _preAuthCreator.GetFileUploadPage.ClickOnDeleteIconInFilesToUpload(1);
                _preAuthCreator.GetFileUploadPage.GetFilesToUploadCount()
                    .ShouldBeEqual(0, "count should be same");

                StringFormatter.PrintMessage("verify Document addition in preauth");
                _preAuthCreator.GetFileUploadPage.ClickOnCancelBtn();
                _preAuthCreator.ClickonAddIconInDocumentUploadForm();
                _preAuthCreator.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                _preAuthCreator.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthCreator.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthCreator.GetFileUploadPage.ClickOnSaveUploadBtn();
                _preAuthCreator.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                _preAuthCreator.IsUploadNewDocumentFormPresent()
                    .ShouldBeFalse("Upload New document form should be closed after uploading document.");
                Convert.ToInt32(_preAuthCreator.GetdocumentCountFromBadge()).ShouldBeEqual(1, "badge added to the document id?");
                _preAuthCreator.IsPreAuthDocumentPresent(fileToUpload.Split(';')[0])
                    .ShouldBeTrue("Uploaded Document is listed and document Name correct");
                DateTime.Parse(_preAuthCreator.UploadedDocumentValueByRowColumn(1, 1, 3)).ToString("MM/dd/yyy").ShouldBeEqual(_preAuthCreator.CurrentDateTimeInMst(DateTime.UtcNow).ToString("MM/dd/yyyy"));

                StringFormatter.PrintMessage("Verify preauth creation with added document");
                _preAuthCreator.clickOnPreauthIcon();
                SetPatientInformation(patientseq);
                SetProviderInformation(providerNumber, authhId, docRefNum);
                AddLines(proccode, tin, ts, oc, billed,paid, "A", true);
                _preAuthCreator.ClickOnSubmitButton();
                _preAuthCreator.IsPageErrorPopupModalPresent();
                var message = _preAuthCreator.GetPageErrorMessage();
                _preAuthCreator.ClosePageError();
                preautseq = Regex.Match(message, @"\d+").ToString();

                StringFormatter.PrintMessage("Verify correct document displayed in preauth action page");
                _preAuthAction = _preAuthCreator.NavigateToPreAuthSearch()
                    .SearchByAuthSequenceAndNavigateToAuthAction(preautseq);
                var docUploadDataFromDb = _preAuthAction.GetDocumentUploadInformationFromDb(preautseq);
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(docUploadDataFromDb.Count, " Uploaded Document Count Should Match");
                _preAuthAction.UploadedDocumentValueByRowColumn(1, 1, 1)
                    .ShouldBeEqual(fileToUpload.Split(';')[0], "File Name Should Match");
                _preAuthAction.UploadedDocumentValueByRowColumn(1, 1, 2)
                     .ShouldBeEqual(expectedSelectedFileTypeList[0], "Dcoument type Should Match");
                DateTime.Parse(_preAuthAction.UploadedDocumentValueByRowColumn(1, 1, 3)).ToString("MM/dd/yyy").ShouldBeEqual(_preAuthCreator.CurrentDateTimeInMst(DateTime.UtcNow).ToString("MM/dd/yyyy"));

                StringFormatter.PrintMessage("Verify Preauth audit for document upload");
                var preauthHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                preauthHistory.GetGridValueListByCol(4).ShouldContain("Document Uploaded", "document upload audit set?");
                _preAuthAction = preauthHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();
                _preAuthCreator = _preAuthAction.NavigateToPreAuthCreatorPage();
            }
            finally
            {
                QuickLaunch.CloseAnyPopupIfExist();
                if(preautseq!= null)
                _preAuthCreator.DeletePreAuthDocumentRecord(preautseq, DateTime.Now.ToString("dd-MMM-yyyy"));
                _preAuthCreator.DeletePreauthFromDb(preautseq);
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void VerifyInputFieldsShowCorrectValues(List<string> initialPatientInfoList, List<string> initialProviderInfoList)
        {
            StringFormatter.PrintMessageTitle("Verification of the input fields of 'Patient Information' section");
            var initialMemberID = _preAuthCreator.GetValueByInputLabel("Member ID");
            initialMemberID.ShouldBeEqual(initialPatientInfoList[0]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Member ID").ShouldBeFalse("'Member ID' field should not be editable");

            var initialFirstName = _preAuthCreator.GetValueByInputLabel("First Name");
            initialFirstName.ShouldBeEqual(initialPatientInfoList[1]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("First Name").ShouldBeFalse("'First Name' field should not be editable");

            var initialLastName = _preAuthCreator.GetValueByInputLabel("Last Name");
            initialLastName.ShouldBeEqual(initialPatientInfoList[2]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Last Name").ShouldBeFalse("'Last Name' field should not be editable");

            var initialPatSeq = _preAuthCreator.GetValueByInputLabel("Pat Seq");
            initialPatSeq.ShouldBeEqual(initialPatientInfoList[3]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Pat Seq").ShouldBeFalse("'Pat Seq' field should not be editable");

            var initialDOB = _preAuthCreator.GetValueByInputLabel("DOB");
            initialDOB.ShouldBeEqual(initialPatientInfoList[4]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("DOB").ShouldBeFalse("'Member ID' field should not be editable");

            StringFormatter.PrintMessageTitle("Verification of the input fields of 'Provider Information' section");
            var initialProvNumber = _preAuthCreator.GetValueByInputLabel("Provider Number");
            initialProvNumber.ShouldBeEqual(initialProviderInfoList[0]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Provider Number").ShouldBeFalse("'Provider Number' field should not be editable");

            var initialProvName = _preAuthCreator.GetValueByInputLabel("Provider Name");
            initialProvName.ShouldBeEqual(initialProviderInfoList[1]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Provider Name").ShouldBeFalse("'Provider Name' field should not be editable");

            var initialTIN = _preAuthCreator.GetValueByInputLabel("TIN");
            initialTIN.ShouldBeEqual(initialProviderInfoList[2]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("TIN").ShouldBeFalse("'TIN' field should not be editable");

            var initialProvSeq = _preAuthCreator.GetValueByInputLabel("Provider Sequence");
            initialProvSeq.ShouldBeEqual(initialProviderInfoList[3]);
            _preAuthCreator.IsLabelAllowedToBeEditedByLabel("Provider Sequence").ShouldBeFalse("'Provider Sequence' field should not be editable");
        }

        //private void VerifyValueByLabel(string label)
        //{
        //    _preAuthCreator.GetValueByLabel(label)
        //        .ShouldBeNullorEmpty("All Values should be cleared after cancelling.");
        //}

        //private void VerifyInputValueByLabel(string label)
        //{
        //    _preAuthCreator.GetInputFieldCreateFormByLabel(label)
        //        .ShouldBeEqual("", "All Values should be cleared after cancelling.");
        //}

        private void SetPatientInformation(string patientMemberId, string group = "")
        {
            _preAuthCreator.ClickOnSelectOptionTabButton("Select Patient");
            _preAuthCreator.SetInputFieldByLabel("Member ID", patientMemberId);
            _preAuthCreator.ClickOnButton();
            _preAuthCreator.ClickOnGridRowByHeaderLabel("Patient Results");
            if (!string.IsNullOrEmpty(group))
                _preAuthCreator.SetInputFieldCreateFormByLabel("Group", group);
        }

        private void SetProviderInformation(string providerNumber, string preAuthId, string docRefNum)
        {
            _preAuthCreator.ClickOnSelectOptionTabButton("Select Provider");
            _preAuthCreator.SetInputFieldByLabel("Provider Number", providerNumber);
            _preAuthCreator.ClickOnButton();
            _preAuthCreator.ClickOnGridRowByHeaderLabel("Provider Results");
            _preAuthCreator.SetInputFieldCreateFormByLabel("Pre-Auth ID", preAuthId);
            _preAuthCreator.SetInputFieldCreateFormByLabel("Doc Ref Num", docRefNum);
        }

        private void AddLines(string procCode, string tn, string ts, string oc, string billed, string paid, string scenario = "A" ,bool save=false)
        {
            _preAuthCreator.ClickOnSelectOptionTabButton("Add Lines");
            _preAuthCreator.SetInputFieldByLabel("Billed", billed);
            _preAuthCreator.SetInputFieldByLabel("Paid", paid);
            _preAuthCreator.SetInputFieldByLabel("Proc Code", procCode);
            _preAuthCreator.SetInputFieldByLabel("TN", tn);
            _preAuthCreator.SetInputFieldByLabel("TS", ts);
            _preAuthCreator.SetInputFieldByLabel("OC", oc);
            _preAuthCreator.SelectDropDownListValueByLabel("Scenario", scenario);
            if (save)
            {
                _preAuthCreator.ClickOnButton();
                _preAuthCreator.WaitForWorking();
            }
        }

        private void VerifyDefaultvalues(string DOS)
        {
            var currentDate = DateTime.Today.ToString("MM/dd/yyyy");
            DOS.ShouldBeEqual(currentDate, "Today's date should be displayed");
            _preAuthCreator.SetInputFieldByLabel("DOS",currentDate, true);
            _preAuthCreator.GetInputFieldValueByLabel("Proc Code")
                .ShouldBeNullorEmpty("No value should be displayed");
            _preAuthCreator.GetInputFieldValueByLabel("TN")
                .ShouldBeNullorEmpty("No value should be displayed");
            _preAuthCreator.GetInputFieldValueByLabel("TS").ShouldBeNullorEmpty("No value should be displayed");
            _preAuthCreator.GetInputFieldValueByLabel("OC").ShouldBeNullorEmpty("No value should be displayed");
            _preAuthCreator.GetInputFieldValueByLabel("Billed")
                .ShouldBeEqual("$0.00", "Default value should be $0.00");
            _preAuthCreator.GetInputFieldValueByLabel("Paid")
                .ShouldBeEqual("$0.00", "Default value should be $0.00");
            _preAuthCreator.GetScenarioInputFieldValue()
                .ShouldBeEqual("", "Default value should be empty");
            
        }

        private void SetInputInAddLineAndVerify(string label, string inputValue, string actualValue, string message)
        {
            _preAuthCreator.SetInputFieldByLabel(label, inputValue);
            _preAuthCreator.GetInputFieldValueByLabel(label)
                .ShouldBeEqual(actualValue, message);
        }

        private void SetInputInAddLineWithErrorPopUp(string label, string testInput, string errorMessage, string validInput, string actualValue, string message)
        {
            _preAuthCreator.SetInputFieldByLabel(label, testInput);
            _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(errorMessage, "error message should match");
            _preAuthCreator.ClosePageError();
            _preAuthCreator.SetInputFieldByLabel(label, validInput, true);
            _preAuthCreator.GetInputFieldValueByLabel(label)
                .ShouldBeEqual(actualValue, message);
        }

        private void ClickAddVerifyErrorMessageAndSetInput(string errorMessage, string label, string inputValue)
        {
            _preAuthCreator.ClickOnButton();
            _preAuthCreator.WaitForWorking();
            _preAuthCreator.GetPageErrorMessage().ShouldBeEqual(errorMessage,
                "Error message should match");
            _preAuthCreator.ClosePageError();
            _preAuthCreator.WaitForStaticTime(1000);
            _preAuthCreator.SetInputFieldByLabel(label, inputValue,true);
           
        }

        private void LabelVerificationInAddLine()
        {
            _preAuthCreator.IsLabelPresentInAddedLineInRightPaneOfAddLine(1, 3)
                .ShouldBeFalse("DOS does not have label");
            _preAuthCreator.IsLabelPresentInAddedLineInRightPaneOfAddLine(1, 4)
                .ShouldBeFalse("Proc Code does not have label");
            _preAuthCreator.IsLabelPresentInAddedLineInRightPaneOfAddLine(1, 5)
                .ShouldBeFalse("Proc code description not have label");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 6)
                .ShouldBeEqual("TN:", " Label should be TN");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 7)
                .ShouldBeEqual("TS:", " Label should be TS");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 8)
                .ShouldBeEqual("OC:", " Label should be OC");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 9)
                .ShouldBeEqual("Billed:", " Label should be Billed");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 10)
                .ShouldBeEqual("Paid:", " Label should be Paid");
            _preAuthCreator.GetLabelOfAddedLineInRightWindow(1, 11)
                .ShouldBeEqual("Scenario:", " Label should be Scenario");
        }

        private void VerifyPreAuthActionValues(Dictionary<string, string> listOfInputValuesInRightWindowForm, string authSeq, string tin, string group, string imageId, string preAuthId, string tn, string ts, string oc, string procCode, string billedValueInAddedLine, string paidValueInAddedLine, string scenario)
        {
            var patMember = listOfInputValuesInRightWindowForm["Member ID"];
            var patSeq = listOfInputValuesInRightWindowForm["Pat Seq"];
            var dob = listOfInputValuesInRightWindowForm["DOB"];
            var prvName = listOfInputValuesInRightWindowForm["Provider Name"];
            var prvSeq = listOfInputValuesInRightWindowForm["Provider Sequence"];
            _preAuthSearch = _preAuthCreator.NavigateToPreAuthSearch();
            _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);
            var dosList = _preAuthAction.GetLineItemsValueListByRowColumn(1, 3);
            var procCodeList = _preAuthAction.GetLineItemsValueListByRowColumn(1, 4);
            var tnList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 2);
            var tsList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 3);
            var ocList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 4);
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq").ShouldBeEqual(authSeq, string.Format("Auth Seq value should be equal to {0}.", authSeq));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Patient Seq").ShouldBeEqual(patSeq, string.Format("Patient Seq value should be euqal to {0}.", patSeq));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("DOB").ShouldBeEqual(dob, string.Format("Date of Birth value should be euqal to {0}.", dob));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual("Cotiviti Unreviewed", "Status should be equal to Cotiviti Unreviewed.");
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Seq").ShouldBeEqual(prvSeq, string.Format("Provider Sequence should be equal to {0}.", prvSeq));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Name").ShouldBeEqual(prvName, string.Format("Provider Name should be equal to {0}.", prvName));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("TIN").ShouldBeEqual(tin, string.Format("TIN value should be equal to {0}.", tin));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Group").ShouldBeEqual(group, string.Format("Group value should be equal to {0}.", group));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Pat No").ShouldBeEqual(patMember, string.Format("Patient Number value should be equal to {0}.", patMember));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Image ID").ShouldBeEqual(imageId, string.Format("Image ID value should be equal to {0}.", imageId));
            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Pre-Auth ID").ShouldBeEqual(preAuthId, string.Format("Pre-Auth ID value should be equal to {0}.", preAuthId));
            tnList[0].ShouldBeEqual(tn, string.Format("TN value should be equal to {0}.", tn));
            tsList[0].ShouldBeEqual(ts, string.Format("TS value should be equal to {0}.", ts));
            ocList[0].ShouldBeEqual(oc, string.Format("OC value should be equal to {0}.", oc));
            procCodeList[0].ShouldBeEqual(procCode, string.Format("ProcCode value should be equal to {0}.", procCode));
            //dosList[0].ShouldBeEqual(DateTime.Now.ToShortDateString(), string.Format("Date should be todays date {0}.", DateTime.Now));
            _preAuthAction.GetLineItemsValueByLabel("Billed:")[0].ShouldBeEqual(billedValueInAddedLine, string.Format("Billed value should be equal to {0}.", billedValueInAddedLine));
            _preAuthAction.GetLineItemsValueByLabel("Adj Paid:")[0].ShouldBeEqual(paidValueInAddedLine, string.Format("Adj Paid value should be equal to {0}.", paidValueInAddedLine));
            _preAuthAction.GetLineItemsValueByLabel("Scen:")[0].ShouldBeEqual(scenario, string.Format("Scenario value should be equal to {0}.", scenario));
        }
        #endregion
    }
}
