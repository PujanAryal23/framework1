using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MicrostrategyMaintenance

    {
        /*  # region PRIVATE
          MicrostrategyMaintenancePage _microstrategyMaintenance;
          private CommonValidations _commonValidation;
          private readonly string _userMaintenancePrivilege = RoleEnum.NucleusAdmin.GetStringValue();
          private List<string> RoleList=new List<string>();
          const string quadrant1 = "User Group";
          const string quadrant2 = "Nucleus Users";
          const string quadrant3 = "Microstrategy Reports";
          #endregion


          #region PROTECTED PROPERTIES



          protected override void ClassInit()
          {
              try
              {
                  base.ClassInit();
                  CurrentPage = _microstrategyMaintenance = QuickLaunch.NavigateToMicrostrategyMaintenance();
                  _commonValidation = new CommonValidations(CurrentPage);


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
              CurrentPage = _microstrategyMaintenance;
          }

          protected override void ClassCleanUp()
          {
               base.ClassCleanUp();
          }

          protected override void TestCleanUp()
          {

              if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
              {
                  _microstrategyMaintenance = _microstrategyMaintenance.Logout().LoginAsHciAdminUser().NavigateToMicrostrategyMaintenance();
              }

              //if (_microstrategyMaintenance.GetPageTitle() != Extensions.GetStringValue(PageHeaderEnum.MicrostrategyMaintenance))
              //{
              //    _microstrategyMaintenance.ClickOnQuickLaunch().NavigateToMicrostrategyMaintenance();
              //}
          }
          #endregion*/
        //#region OVERRIDE
        //protected  string GetType().FullName
        //{
        //    get { return GetType().FullName; }
        //}
        //#endregion

        [Test] //TE-295
        public void Validate_Security_And_Navigation_Of_Microstrategy_Maintenance_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                MicrostrategyMaintenancePage _microstrategyMaintenance;
                CommonValidations _commonValidation;
                string _userMaintenancePrivilege = RoleEnum.NucleusAdmin.GetStringValue();
                automatedBase.CurrentPage =
                    _microstrategyMaintenance = automatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Settings,
                    new List<string> {SubMenu.MicrostrategyMaintenance},
                    _userMaintenancePrivilege,
                    new List<string> {Extensions.GetStringValue(PageHeaderEnum.MicrostrategyMaintenance)},
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});

                _microstrategyMaintenance.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities()
                    .NavigateToMicrostrategyMaintenance();
                Verify_page_is_opened_in_View_mode(_microstrategyMaintenance);
            }
        }


        [Test] //TE-300
        public void Verify_add_edit_delete_and_view_of_microstrategy_reports_in_third_quadrant()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                MicrostrategyMaintenancePage _microstrategyMaintenance;
                List<string> RoleList = new List<string>();
                const string quadrant1 = "User Group";
                const string quadrant2 = "Nucleus Users";
                const string quadrant3 = "Microstrategy Reports";

                automatedBase.CurrentPage =
                    _microstrategyMaintenance = automatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                 var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var addDataset = paramList["AddDataset"].Split(';').ToList();
                var updateDataset = paramList["UpdateDataset"].Split(';').ToList();

                var expectedData =
                    _microstrategyMaintenance.GetMstrReportsByReportName(
                        _microstrategyMaintenance.GetValueInGridByRowCol(quadrant3));

                //soft delete report : only isactive is set F 

                DeleteRecordByValue(_microstrategyMaintenance,addDataset[0]);
                DeleteRecordByValue(_microstrategyMaintenance,updateDataset[0]);

                //hard delete reports
                _microstrategyMaintenance.HardDeleteMstrReportByReportName(addDataset[0]);
                _microstrategyMaintenance.HardDeleteMstrReportByReportName(updateDataset[0]);

                RoleList = _microstrategyMaintenance.GetRoleList();
                Random r = new Random();
                List<string> expectedLabels = paramList["Labels"].Split(';').ToList();
                _microstrategyMaintenance.GetQuadrantTitle(2).ShouldBeEqual(quadrant3, "Quadrant Title");
                _microstrategyMaintenance.IsAddReportIconEnabled()
                    .ShouldBeTrue("Is Add Microstrategy Report Icon enabled?");

                StringFormatter.PrintMessage("Verify View of existing report");
                var reportCount = _microstrategyMaintenance.GetGridRowCountByQuadrantName("Microstrategy Reports");
                reportCount.ToString().ShouldBeEqual(_microstrategyMaintenance.GetMstrReportsCountFromDatabase(),
                    "Report count should match with database value");

                int randRow = r.Next(1, reportCount);
                _microstrategyMaintenance.IsEditIconPresentInByRow(quadrant3, randRow)
                    .ShouldBeTrue(string.Format("Is Edit Icon present in Reports list random row {0}", randRow));
                _microstrategyMaintenance.IsDeleteIconPresentByRow(quadrant3, randRow)
                    .ShouldBeTrue(string.Format("Is Edit Icon present in Reports list random row {0}", randRow));
                _microstrategyMaintenance.GetGridLabelListByRow(quadrant3)
                    .ShouldCollectionBeEqual(expectedLabels, "Labels are as expected");
                _microstrategyMaintenance.GetGridValueListByRow(quadrant3).ShouldCollectionBeEqual(expectedData,
                    "Values displayed for  particular record is as expected");
                _microstrategyMaintenance.GetReportsGridListValueByCol().IsInAscendingOrder().ShouldBeTrue(
                    "Default sorting of Microstrategy Reports list must be Report name in ascending order?");

                StringFormatter.PrintMessage("Verifying create new Microstrategy Report");
                _microstrategyMaintenance.ClickOnAddIconByQuadrantName(quadrant3);
                _microstrategyMaintenance.IsAddReportIconDisabled()
                    .ShouldBeTrue("Is Add Microstrategy Report Icon disabled?");
                _microstrategyMaintenance.IsAddMicrostrategyReportFormDisplayed()
                    .ShouldBeTrue("Is Create Microstrategy Report form displayed");
                _microstrategyMaintenance.GetReportFormHeader().ShouldBeEqual("Add Microstrategy Report",
                    "Create new Microstrategy Report form Header");
                VerifyFieldsInAddMicrostrategyReportForm(_microstrategyMaintenance,expectedLabels);
                VerifyMstrReportValidationMessageInForm(_microstrategyMaintenance,expectedLabels);
                ValidateDuplicationOfMicrostrategyReport(_microstrategyMaintenance,quadrant3, expectedLabels, false);
                ValidateCancelButtonCloseTheFormAndRecordsAreNotSaved(_microstrategyMaintenance,quadrant3, expectedLabels);

                _microstrategyMaintenance.ClickOnAddIconByQuadrantName(quadrant3);
                var unassignedRole = _microstrategyMaintenance.GetAvailableDropDownList("Role")
                    .Except(_microstrategyMaintenance.GetReportsGridListValueByCol(5)).ToList();

                for (int i = 0; i < expectedLabels.Count; i++)
                {
                    if (expectedLabels[i].Equals("Role"))
                    {
                        _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel(expectedLabels[i],
                            unassignedRole[0]);
                        continue;
                    }

                    _microstrategyMaintenance.SetInputFieldByLabel(expectedLabels[i], addDataset[i]);
                }

                _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
                _microstrategyMaintenance.WaitForWorkingAjaxMessage();

                _microstrategyMaintenance.IsAddMicrostrategyReportFormDisplayed()
                    .ShouldBeFalse("Add Report Container should  close when save button is clicked.");
                _microstrategyMaintenance.GetGridRowCountByQuadrantName(quadrant3).ShouldBeEqual(reportCount + 1,
                    "Grid Row count should increase once the record is saved");
                //_microstrategyMaintenance.ClickOnAddIconByQuadrantName(quadrantName);

                //_microstrategyMaintenance.GetAvailableDropDownList("Role").ShouldNotContain(role, "Once the Role is mapped with report, that role should not be listed in Role dropdown");
                //RoleList.CollectionShouldNotBeSubsetOf(_microstrategyMaintenance.GetAvailableDropDownList("Role"),"");

                var newlyAddedReport = _microstrategyMaintenance.GetMstrReportsByReportName(addDataset[0]);
                _microstrategyMaintenance.IsRowPresentByValue(addDataset[0]).ShouldBeTrue("New record must be added");

                for (int i = 0; i < expectedLabels.Count; i++)
                {
                    if (expectedLabels[i].Equals("Role"))
                    {
                        _microstrategyMaintenance.GetGridColValueByValue(addDataset[0], i + 2).ShouldBeEqual(
                            unassignedRole[0],
                            String.Format("{0} value should be correct", expectedLabels[i]));
                        continue;
                    }

                    _microstrategyMaintenance.GetGridColValueByValue(addDataset[0], i + 2).ShouldBeEqual(
                        newlyAddedReport[i],
                        String.Format("{0} value should be correct", expectedLabels[i]));
                }

                _microstrategyMaintenance.IsEditIconPresentByValueInRow(addDataset[0])
                    .ShouldBeTrue("Is Edit icon present in newly added row");
                _microstrategyMaintenance.IsDeleteIconPresentByValueInRow(addDataset[0])
                    .ShouldBeTrue("Is Delete icon present in newly added row.");

                StringFormatter.PrintMessage("Verify edit of Microstrategy Report");
                if (_microstrategyMaintenance.IsAddMicrostrategyReportFormDisplayed())
                    _microstrategyMaintenance.ClickonCancelButton();


                _microstrategyMaintenance.ClickOnEditIconByRowValue(addDataset[0]);
                _microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed()
                    .ShouldBeTrue("Is Update Microstrategy Report form displayed");
                _microstrategyMaintenance.GetReportFormHeader().ShouldBeEqual("Edit Microstrategy Report",
                    "Edit Microstrategy Report form Header");
                VerifyFieldsInAddMicrostrategyReportForm(_microstrategyMaintenance,expectedLabels, true);
                _microstrategyMaintenance.ClickonCancelButton();
                _microstrategyMaintenance.ClickOnEditIconByRowValue(addDataset[0]);

                for (int i = 0; i < expectedLabels.Count; i++)
                {
                    _microstrategyMaintenance.GetInputFieldValueByLabel(expectedLabels[i]).ShouldBeEqual(
                        newlyAddedReport[i],
                        String.Format("Correct value should be auto displayed for '{0}' in Edit form ",
                            expectedLabels[i]));
                }

                VerifyMstrReportValidationMessageInForm(_microstrategyMaintenance,expectedLabels);
                _microstrategyMaintenance.ClickonCancelButton();

                ValidateDuplicationOfMicrostrategyReport(_microstrategyMaintenance,quadrant3, expectedLabels);
                var reportNameBefore = _microstrategyMaintenance.GetValueInGridByRowCol(quadrant3, 2, 2);
                _microstrategyMaintenance.ClickonCancelButton();
                _microstrategyMaintenance.ClickOnEditIconByRowCol(quadrant3, 2);
                _microstrategyMaintenance.SetInputFieldByLabel("Report Name", "aaaa");
                _microstrategyMaintenance.ClickonCancelButton();
                _microstrategyMaintenance.GetValueInGridByRowCol(quadrant3, 2, 2).ShouldBeEqual(reportNameBefore,
                    "Cancel operation does not update the Report Name value");
                _microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed()
                    .ShouldBeFalse("Is Edit form displayed?");

                UpdateAndValidateMstrReport(_microstrategyMaintenance,addDataset, expectedLabels, updateDataset);

                StringFormatter.PrintMessage("Verify delete of Microstrategy Report");
                _microstrategyMaintenance.ClickOnDeleteIconByRowValue(updateDataset[0]);
                _microstrategyMaintenance.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Confirmation popup must be displayed before the record is deleted.");
                _microstrategyMaintenance.GetPageErrorMessage()
                    .ShouldBeEqual("Do you want to delete this Microstrategy Report?");
                _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(false);
                _microstrategyMaintenance.GetGridRowCountByQuadrantName(quadrant3).ShouldBeEqual(reportCount + 1,
                    "Report must not be deleted so count must remain same");

                _microstrategyMaintenance.ClickOnDeleteIconByRowValue(updateDataset[0]);
                _microstrategyMaintenance.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Confirmation popup must be displayed before the record is deleted.");
                _microstrategyMaintenance.GetPageErrorMessage()
                    .ShouldBeEqual("Do you want to delete this Microstrategy Report?");
                _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(true);
                _microstrategyMaintenance.GetGridRowCountByQuadrantName(quadrant3).ShouldBeEqual(reportCount,
                    "Report must be deleted so count must reduce");
                _microstrategyMaintenance.IsRowPresentByValue(updateDataset[0])
                    .ShouldBeFalse("Deleted Record should not be displayed in Grid");


            }
        }




        [Test] //TE-299
        public void verify_add_edit_delete_view_microstrategy_UserGroups_in_first_quadrant()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                MicrostrategyMaintenancePage _microstrategyMaintenance;
                automatedBase.CurrentPage =
                    _microstrategyMaintenance = automatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                var TestName = new StackFrame(true).GetMethod().Name;
                var displayedLabels = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "FirstQuadrantLabel", "Value").Split(',').ToList();
                var addUserData = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AdduserGroupValue", "Value").Split(',').ToList();
                var editUserData = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "EditUserGroupValue", "Value").Split(',').ToList();

                try
                {

                    var expectedList = _microstrategyMaintenance.GetUserDetailsFromDatabase();
                    _microstrategyMaintenance.GetQuadrantTitle()
                        .ShouldBeEqual("User Group", "Is correct Quadrant Title displayed?");

                    _microstrategyMaintenance.GetGridValueByQuadrantName("User Group")
                        .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "Group Names equal?");

                    _microstrategyMaintenance.GetGridValueInfirstQuadrant()
                        .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "User Names equal?");
                    var rowcount = _microstrategyMaintenance.GetGridRowCountByQuadrantName("User Group");
                    rowcount.ShouldBeEqual(Convert.ToInt32(_microstrategyMaintenance.CountOfuserGrpups()));
                    _microstrategyMaintenance.IsAddIconPresent("User Group").ShouldBeTrue("Is add icon present?");
                    StringFormatter.PrintMessage("verify creation of new user group");
                    CreateUserGroup(_microstrategyMaintenance,addUserData, displayedLabels);

                    _microstrategyMaintenance.ClickOnGridRowByValue(addUserData[0]);
                    _microstrategyMaintenance.IsEmptyMessagePresent().ShouldBeTrue("Is empty message displayed");
                    _microstrategyMaintenance.GetEmptyMessage().ShouldBeEqual("No Users Available",
                        "Since newly created group has no any users mapped, empty message must be displayed");
                    StringFormatter.PrintMessage("verify added user group displayed");
                    _microstrategyMaintenance.GetGridRowCountByQuadrantName("User Group").ShouldBeEqual(rowcount + 1);
                    _microstrategyMaintenance.GetCreateOrEditedUserGroup(addUserData[0], addUserData[1])
                        .ShouldBeEqual(1, " Row inserted in database?");


                    _microstrategyMaintenance.IsRowPresentByValue(addUserData[0])
                        .ShouldBeTrue(" validate added groupname present?");
                    _microstrategyMaintenance.GetGridColValueByValue(addUserData[0], 3)
                        .ShouldBeEqual(addUserData[1], "validate added username");

                    StringFormatter.PrintMessage("Added Nuclues user to the addes user group");
                    _microstrategyMaintenance.ClickOnGridRowByValue(addUserData[0]);
                    _microstrategyMaintenance.ClickOnAddIconByQuadrantName("Nucleus Users");
                    _microstrategyMaintenance.SelectNucleusUser("Username", "MSTRUser automation (mstrclientuser11)");
                    _microstrategyMaintenance.MouseOutFromDropdown("Username");
                    _microstrategyMaintenance.SideWindow.Save();
                    _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users").ShouldBeEqual(1);
                    _microstrategyMaintenance.IsRowPresentByValue("MSTRUser automation")
                        .ShouldBeTrue("Record should  be listed in Grid once it is added");
                    _microstrategyMaintenance.GetAvailableDropDownList("Username").ShouldNotContain(
                        "MSTRUser automation (mstrclientuser11)",
                        "Dropdown list should not contain already mapped Nucleus User");
                    _microstrategyMaintenance.MouseOutFromDropdown("Username");



                    StringFormatter.PrintMessage("Verify update of user group");
                    var savedPassword =
                        _microstrategyMaintenance.GetPasswordValueFromDataBase(addUserData[0], addUserData[1]);
                    VerifyUpdateOfUserGroup(_microstrategyMaintenance,addUserData, displayedLabels, editUserData, rowcount);
                    _microstrategyMaintenance.GetPasswordValueFromDataBase(editUserData[0], editUserData[1])
                        .ShouldNotBeEqual(savedPassword, "password updated in database?");




                    VerifyValidationsInFirstQuadrant(_microstrategyMaintenance,editUserData, displayedLabels);
                    StringFormatter.PrintMessage("verify user-group deletion");
                    // VerifyDeleteOfUserGroup(editUserData, rowcount);
                    DeleteRecordByValue(_microstrategyMaintenance,editUserData[0]);
                    _microstrategyMaintenance.IsRowPresentByValue(editUserData[0])
                        .ShouldBeFalse("Deleted value displayed?");
                    _microstrategyMaintenance.GetGridRowCountByQuadrantName("User Group").ShouldBeEqual(rowcount);
                    _microstrategyMaintenance.GetUserGroupRecordStatusFromDatabase(editUserData[0], editUserData[1])
                        .ShouldBeFalse("Is report status set to false after deletion?");
                    _microstrategyMaintenance.GetAvailableDropDownList("Username").ShouldContain(
                        "MSTRUser automation (mstrclientuser11)",
                        "After deleting the Group that user was mapped into, the user should be available in the Dropdown list");


                }
                finally
                {
                    if (Convert.ToBoolean(
                        _microstrategyMaintenance.GetUserGroupDataFromDatabase(addUserData[0], addUserData[1])))
                        _microstrategyMaintenance.DeleteUserGroup(addUserData[0], addUserData[1]);
                    else if (Convert.ToBoolean(
                        _microstrategyMaintenance.GetUserGroupDataFromDatabase(editUserData[0], editUserData[1])))
                        _microstrategyMaintenance.DeleteUserGroup(editUserData[0], editUserData[1]);
                    else
                    {
                        StringFormatter.PrintMessage("No user Group was added or edited");
                    }


                }






            }
        }

        [Test] //TE-310
        public void Verify_Nucleus_Users_group_in_the_second_quadrant()

        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                MicrostrategyMaintenancePage _microstrategyMaintenance;
                automatedBase.CurrentPage =
                    _microstrategyMaintenance = automatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                      var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userIdList = paramLists["UserIdList"].Split(';').ToList();
                var userNameList = paramLists["UserNameList"].Split(';').ToList();

                List<string> userNames = new List<string>();
                userNames.Add(userNameList[0]);
                userNames.Add(userNameList[1]);

                List<string> userIds = new List<string>();
                userIds.Add(userIdList[0]);
                userIds.Add(userIdList[1]);

                automatedBase.CurrentPage = _microstrategyMaintenance.ClickOnQuickLaunch();
                _microstrategyMaintenance = automatedBase.CurrentPage.NavigateToMicrostrategyMaintenance();

                _microstrategyMaintenance.GetQuadrantTitle(1).ShouldBeEqual("Nucleus Users", "Quadrant Title");
                _microstrategyMaintenance.IsAddIconPresentQuadrantName("Nucleus Users")
                    .ShouldBeTrue("Add Icon should be present");
                _microstrategyMaintenance.IsAddIconDisabled("Nucleus Users").ShouldBeTrue(
                    "Add Icon should not be enabled until a row is selected in the Microstrategy User Groups");
                _microstrategyMaintenance.IsEmptyMessagePresent()
                    .ShouldBeTrue("Since non of the group is selected, No Group Selected message must be displayed ");
                _microstrategyMaintenance.GetEmptyMessage().ShouldBeEqual("No Group Selected");
                _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users")
                    .ShouldBeEqual(0, "Users should not be listed");

                Validate_user_ID_that_belong_to_a_particular_Group_Name(_microstrategyMaintenance,paramLists["GroupName"]);
                _microstrategyMaintenance.ClickOnAddIconByQuadrantName("Nucleus Users");
                _microstrategyMaintenance.IsAddFormPresent().ShouldBeTrue("Add form should be present");
                _microstrategyMaintenance.SideWindow.Save();
                _microstrategyMaintenance.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Pop up error message should be present");
                _microstrategyMaintenance.GetPageErrorMessage()
                    .ShouldBeEqual("At least one User selection is required before the User can be added.");
                _microstrategyMaintenance.ClosePageError();
                StringFormatter.PrintMessage("Username dropdown list verification");
                var expectedUsernameList = _microstrategyMaintenance.GetUserNameList();

                ValidateUsername(_microstrategyMaintenance,"Username", expectedUsernameList);
                DeleteUserMapping(_microstrategyMaintenance,userIds);
                Verify_Add_user_form_and_Remove_user_for_particular_group(_microstrategyMaintenance,userNameList, paramLists["GroupName"],
                    userIds);

            }
        }

        private void DeleteUserMapping(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> userIdList)
        {
            foreach (var userID in userIdList)
            {
                if (_microstrategyMaintenance.IsRowPresentByValue(userID))
                {
                    _microstrategyMaintenance.ClickOnDeleteIconByFullName(userID);
                    _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(true);
                }
            }
            
        }

        #region PRIVATE

       

        private void VerifyFieldsInAddMicrostrategyReportForm(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> inputFieldsListForAddReport,bool isEdit=false)
        {
            //List<string> EditRoleList = _microstrategyMaintenance.GetRoleList();
            List<string> RoleList  = _microstrategyMaintenance.GetRoleList();
            foreach (var inputField in inputFieldsListForAddReport)
            {
                _microstrategyMaintenance.IsInputFieldPresentInAddReport(inputField).ShouldBeTrue(
                    string.Format("'{0}' field must be present in Add Microstrategy Report form", inputField));

                if (inputField == "Role" && isEdit)
                {
                   
                    //EditRoleList.Add(_microstrategyMaintenance.GetInputFieldValueByLabel("Role"));
                    ValidateSingleDropDownForDefaultValueAndExpectedList(_microstrategyMaintenance,inputField, RoleList,false,true);
                }
                else if (inputField == "Role" && !isEdit)
                    ValidateSingleDropDownForDefaultValueAndExpectedList(_microstrategyMaintenance,inputField, RoleList,false);
                else if (!isEdit)
                    _microstrategyMaintenance.SideWindow.GetInputValueByLabel(inputField)
                        .ShouldBeNullorEmpty("Default value of {0} must be empty");
            }


            _microstrategyMaintenance.SetInputFieldByLabel("Report Name",string.Concat(Enumerable.Repeat("a1@_B",21)));
            _microstrategyMaintenance.GetLengthOfTheInputFieldByLabel("Report Name").ShouldBeEqual(100,
                "Maxinum character limit of Report Name field should be 100 alphanumeric characters");

            _microstrategyMaintenance.SetInputFieldByLabel("Project Id", string.Concat(Enumerable.Repeat("a1@_B", 21)));
            _microstrategyMaintenance.GetLengthOfTheInputFieldByLabel("Project Id").ShouldBeEqual(100,
                "Maxinum character limit of Project Id field should be 100 alphanumeric characters");

            _microstrategyMaintenance.SetInputFieldByLabel("Report Id", string.Concat(Enumerable.Repeat("a1@_B", 21)));
            _microstrategyMaintenance.GetLengthOfTheInputFieldByLabel("Report Id").ShouldBeEqual(100,
                "Maxinum character limit of Project Id field should be 100 alphanumeric characters");
        }

        private void Validate_user_ID_that_belong_to_a_particular_Group_Name(MicrostrategyMaintenancePage _microstrategyMaintenance,string groupName)
        {
            _microstrategyMaintenance.ClickOnGridRowByValue(groupName);
            _microstrategyMaintenance.IsAddIconDisabled("Nucleus Users")
                .ShouldBeFalse("Add Icon should be enabled when a row is selected in the Microstrategy User Groups");
            _microstrategyMaintenance.GetLabelInGridByColRow("Nucleus Users").ShouldBeEqual("Full Name");
            _microstrategyMaintenance.GetLabelInGridByColRow("Nucleus Users", 3).ShouldBeEqual("User Id");
            _microstrategyMaintenance.GetGridValueByQuadrantName("Nucleus Users", 3).
                ShouldCollectionBeEquivalent(_microstrategyMaintenance.GetMstrUserIdsPerGroupName(groupName),
                    "The list of usernames that belong to a particular group should match");
            _microstrategyMaintenance.GetNucleusUserGridListValueByCol(1).IsInAscendingOrder().ShouldBeTrue("Grid list must be sorted by Full Name in ascending order.");

        }


        private void ValidateFieldSupportingMultipleValues(MicrostrategyMaintenancePage _microstrategyMaintenance,string label, IList<string> expectedUserNameDropDownList)
        {
            //ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedUserNameDropDownList);
            _microstrategyMaintenance.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedUserNameDropDownList[0]);
            _microstrategyMaintenance.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedUserNameDropDownList[0], label + "  single value selected");
            _microstrategyMaintenance.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedUserNameDropDownList[1]);
            _microstrategyMaintenance.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "  multiple value selected");
        }

        private void VerifyMstrReportValidationMessageInForm(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> labels)
        {
            ClearAllFieldsInMstrReportForm(_microstrategyMaintenance,labels);
            _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("Error popup should be displayed when user clicks on save button with all fields empty");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Please enter missing fields.");
            _microstrategyMaintenance.ClosePageError();

            

        }

        private void ClearAllFieldsInMstrReportForm(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> labels)
        {
            foreach (var label in labels)
            {
                _microstrategyMaintenance.ClearInputField(label);
                Console.WriteLine("{0} text field cleared.");
            }
            
        }

        public void CreateUserGroup(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> adduserdata, List<string> displayedLabels)
        {

            _microstrategyMaintenance.ClickOnAddIconByQuadrantName("User Group");
            _microstrategyMaintenance.IsAddusergroupiconDisabled()
                .ShouldBeTrue("Is Add Microstrategy add group Icon disabled?");
            _microstrategyMaintenance.IsAddMicrostrategyUserGroupFormDisplayed()
                .ShouldBeTrue("Is Create Microstrategy Report form displayed");
            _microstrategyMaintenance.GetUserGroupFormHeader().ShouldBeEqual("Add Microstrategy User-Group",
                "Create new Microstrategy User Group form Header");


            for (int i = 0; i < displayedLabels.Count; i++)
            {
                _microstrategyMaintenance.SetInputFieldByLabel(displayedLabels[i], adduserdata[i]);
            }

            _microstrategyMaintenance.ClickOnSaveButton();
            _microstrategyMaintenance.WaitForWorkingAjaxMessage();
            _microstrategyMaintenance.IsAddMicrostrategyUserGroupFormDisplayed()
                .ShouldBeFalse("Is Create Microstrategy user group form displayed?");
            _microstrategyMaintenance.IsAddusergroupiconDisabled()
                .ShouldBeFalse("Is Add Microstrategy group Icon disabled?");
        }

        private void VerifyUpdateOfUserGroup(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> adduserdata, List<string> displayedLabels,List<string>editUserdata,int rowcount)
        {

            _microstrategyMaintenance.ClickOnEditIconByRowValue(adduserdata[0]);
            _microstrategyMaintenance.IsEditMicrostrategyAddUserGroupFormDisplayed().ShouldBeTrue("Is microstrategy edit user group displayed?");
            _microstrategyMaintenance.IsInputFieldPresentInAddusergroup(displayedLabels[2]).ShouldBeFalse("password field present?");
            _microstrategyMaintenance.IsInputFieldPresentInAddusergroup(displayedLabels[3]).ShouldBeFalse("Confirm password field present?");
            _microstrategyMaintenance.ClickOnUpdatePassword();
            _microstrategyMaintenance.IsInputFieldPresentInAddusergroup(displayedLabels[2]).ShouldBeTrue("password field present?");
            _microstrategyMaintenance.IsInputFieldPresentInAddusergroup(displayedLabels[3]).ShouldBeTrue("Confirm password field present?");
            for (int i = 0; i < displayedLabels.Count-2; i++)
            {
                
                _microstrategyMaintenance.GetInputFieldValueByLabel(displayedLabels[i]).ShouldBeEqual(adduserdata[i], "verify edit of user group");
                _microstrategyMaintenance.ClearInputField(displayedLabels[i]);
                _microstrategyMaintenance.SetInputFieldByLabel(displayedLabels[i],editUserdata[i]);
            }

            
            _microstrategyMaintenance.SetInputFieldByLabel(displayedLabels[2], editUserdata[2]);
            _microstrategyMaintenance.SetInputFieldByLabel(displayedLabels[3], editUserdata[3]);
            _microstrategyMaintenance.ClickOnSaveButton();
            _microstrategyMaintenance.WaitForWorkingAjaxMessage();
            _microstrategyMaintenance.IsEditMicrostrategyAddUserGroupFormDisplayed().ShouldBeFalse("Is microstrategy edit user group displayed?");
            _microstrategyMaintenance.GetGridRowCountByQuadrantName("User Group").ShouldBeEqual(rowcount + 1);
            _microstrategyMaintenance.GetCreateOrEditedUserGroup(editUserdata[0], editUserdata[1])
              .ShouldBeEqual(1, " Row updated in database?");
            _microstrategyMaintenance.IsRowPresentByValue(editUserdata[0]).ShouldBeTrue(" validate edited groupname present?");
            _microstrategyMaintenance.GetGridColValueByValue(editUserdata[0], 3).ShouldBeEqual(editUserdata[1],"validate edited username");
            StringFormatter.PrintMessage("Verify Nucleus User is retained when user group is edited");
            _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users").ShouldBeEqual(1);
            _microstrategyMaintenance.IsRowPresentByValue("MSTRUser automation").ShouldBeTrue("Record should  be listed in Grid once it is added");

        }

        private void VerifyDeleteOfUserGroup(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string>expectedList, int rowcount)
        {
            DeleteRecordByValue(_microstrategyMaintenance,expectedList[0]);
            _microstrategyMaintenance.GetGridRowCountByQuadrantName("User Group").ShouldBeEqual(rowcount);
           _microstrategyMaintenance.GetUserGroupRecordStatusFromDatabase(expectedList[0],expectedList[1]).ShouldBeFalse("Is report value set to false after deletion?");
            _microstrategyMaintenance.DeleteUserGroup(expectedList[0], expectedList[1]);

        }


        

        private void ValidateUsername(MicrostrategyMaintenancePage _microstrategyMaintenance,string label, IList<string> collectionToEqual)
        {
            _microstrategyMaintenance.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
            _microstrategyMaintenance.GetMultiSelectListedDropDownList(label).Contains("All")
                    .ShouldBeTrue(
                        "A value of all displayed at the top of the list, followed by options sorted alphabetically.");
            var reqDropDownList = _microstrategyMaintenance.GetAvailableDropDownList(label);

            reqDropDownList.Remove("All");
            reqDropDownList.Remove("Clear");
            if (reqDropDownList.Count != 0)
            {
                /*  check for first , last and randomly generated index */
                var randm = new Random();
                var i = randm.Next(1, reqDropDownList.Count - 2);
                Console.WriteLine("Verify the first element of the list for the proper format.");
                //check the first element of list
                reqDropDownList[0].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
                //check the last element of list
                Console.WriteLine("Verify the last element of the list for the proper format.");
                reqDropDownList[reqDropDownList.Count - 1].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
                Console.WriteLine("Verify the random element of the list for the proper format.");
                //check the random element
                reqDropDownList[i].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
            }

            var searchList = reqDropDownList.Select(s => s.Substring(0, s.LastIndexOf('(')-1).ToLower()).ToList();
            
            searchList.ShouldCollectionBeEquivalent(collectionToEqual, label + " List As Expected and sorted");

            ValidateFieldSupportingMultipleValues(_microstrategyMaintenance,label, reqDropDownList);

            _microstrategyMaintenance.SideWindow.Cancel();
        }

        private void Verify_Add_user_form_and_Remove_user_for_particular_group(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> userNames, string groupName, List<string> userIds)
        {
            var userNameRowCount = _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users");
            
            _microstrategyMaintenance.ClickOnAddIconByQuadrantName("Nucleus Users");
            _microstrategyMaintenance.SelectMultipleValuesInMultiSelectDropdownList("Username", userNames);
            _microstrategyMaintenance.SideWindow.Save();
            _microstrategyMaintenance.WaitForWorking();
            _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users").ShouldBeEqual(userNameRowCount + 2);
            _microstrategyMaintenance.GetAvailableDropDownList("Username").
                CollectionShouldNotBeSubsetOf(_microstrategyMaintenance.GetUserNameList(), "Once the user is added to the group the name should not be avialble in the dropdown list");
            _microstrategyMaintenance.MouseOutFromDropdown("Username");
            _microstrategyMaintenance.IsRowPresentByValue("Test Automation5").ShouldBeTrue("Record should  be listed in Grid once it is added");
            _microstrategyMaintenance.IsRowPresentByValue("Test Automation6").ShouldBeTrue("Record should  be listed in Grid once it is added");

            var profileManager = _microstrategyMaintenance.ClickOnUserIdAndNavigateToProfileManager("Test Automation5");
            profileManager.GetPageHeader().ShouldBeEqual("User Profile", "Page should redirect to Appeal Summary Page");
            profileManager.GetWindowHandlesCount()
                .ShouldBeEqual(1, "Profile Manager Page should be opened in the same page");
            profileManager.ClickOnBrowserBackButton();
            
            
            _microstrategyMaintenance.GetPageHeader()
                .ShouldBeEqual(PageHeaderEnum.MicrostrategyMaintenance.GetStringValue(),"User should navigate back to Microstrategy Setting page");
            _microstrategyMaintenance.ClickOnGridRowByValue(groupName);
            _microstrategyMaintenance.ClickOnDeleteIconByFullName(userIds[0]);
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("Is confirmation popup displayed?");
            _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(false);
            _microstrategyMaintenance.IsRowPresentByValue(userIds[0]).ShouldBeTrue("Row should not be deleted when confirmation is cancelled");

            _microstrategyMaintenance.ClickOnDeleteIconByFullName(userIds[0]);
            _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(true);

            _microstrategyMaintenance.ClickOnDeleteIconByFullName(userIds[1]);
            _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(true);

            _microstrategyMaintenance.GetGridRowCountByQuadrantName("Nucleus Users").ShouldBeEqual(userNameRowCount);

            _microstrategyMaintenance.IsRowPresentByValue(userIds[0]).ShouldBeFalse("Record should not be listed in Grid once it is deleted");
            _microstrategyMaintenance.IsRowPresentByValue(userIds[1]).ShouldBeFalse("Record should not be listed in Grid once it is deleted");

            _microstrategyMaintenance.ClickOnAddIconByQuadrantName("Nucleus Users");
            _microstrategyMaintenance.GetAvailableDropDownList("Username").ShouldContain(userNames[0], "Once the user mapping to group is deleted, user name should be displayed in dropdown again.");
            _microstrategyMaintenance.GetAvailableDropDownList("Username").ShouldContain(userNames[1], "Once the user mapping to group is deleted, user name should be displayed in dropdown again.");


        }

        private void ValidateDuplicationOfMicrostrategyReport(MicrostrategyMaintenancePage _microstrategyMaintenance,string quadrantName, List<string> labels,
            bool isEdit = true)
        {
            
            if (isEdit )
                _microstrategyMaintenance.ClickOnEditIconByRowCol(quadrantName);
            else
            {
                if (!_microstrategyMaintenance.IsAddMicrostrategyReportFormDisplayed())
                    _microstrategyMaintenance.ClickOnAddIconByQuadrantName(quadrantName);

            }
            for (int i = 1; i <= labels.Count; i++)
            {
                if (labels[i-1].Equals("Role"))
                {
                    _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel(labels[i - 1], _microstrategyMaintenance.GetValueInGridByRowCol(quadrantName, i + 1, 2));
                    continue;
                }
                _microstrategyMaintenance.SetInputFieldByLabel(labels[i-1],
                    _microstrategyMaintenance.GetValueInGridByRowCol(quadrantName, i + 1, 2));
            }
            _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Page Error popup displayed?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Report Name already exists, Please enter a unique value.");
            _microstrategyMaintenance.ClosePageError();

            _microstrategyMaintenance.SetInputFieldByLabel(labels[0],"Test Report Name");
            _microstrategyMaintenance.SetInputFieldByLabel(labels[2],_microstrategyMaintenance.GetValueInGridByRowCol(quadrantName, 4, 2));

            _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Page Error popup displayed?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Report ID already exists, Please enter a unique value.");
            _microstrategyMaintenance.ClosePageError();

            _microstrategyMaintenance.SetInputFieldByLabel(labels[2],"Test Report ID");
            _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel(labels[3],_microstrategyMaintenance.GetValueInGridByRowCol(quadrantName,5,2));

            _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Page Error popup displayed?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Given role is already assigned to existing user, Please select different role.");
            _microstrategyMaintenance.ClosePageError();
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(MicrostrategyMaintenancePage _microstrategyMaintenance,string label, IList<string> collectionToEqual, bool order = true,bool isEdit=false)
        {
            var actualDropDownList = _microstrategyMaintenance.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEquivalent(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            if (!isEdit)
            {
                _microstrategyMaintenance.SideWindow.GetInputValueByLabel(label)
                    .ShouldBeEqual("Please Select", label + " value defaults to Please Select");
            }
            _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _microstrategyMaintenance.SideWindow.SelectedDropDownOptionsByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
            

        }

        private void ValidateCancelButtonCloseTheFormAndRecordsAreNotSaved(MicrostrategyMaintenancePage _microstrategyMaintenance,string quadrantName,
            List<string> labels)
        {
            var reportCountBefore=_microstrategyMaintenance.GetGridRowCountByQuadrantName(quadrantName);
            _microstrategyMaintenance.ClickOnAddIconByQuadrantName(quadrantName);
            for (int i = 1; i < labels.Count; i++)
            {
                _microstrategyMaintenance.SetInputFieldByLabel(labels[i-1],
                    _microstrategyMaintenance.GetValueInGridByRowCol(quadrantName, i + 1, 2));
            }
            _microstrategyMaintenance.ClickonCancelButton();
            _microstrategyMaintenance.IsAddMicrostrategyReportFormDisplayed().ShouldBeFalse("Is Add Microstrategy Report for present?");
            _microstrategyMaintenance.GetGridRowCountByQuadrantName(quadrantName).ShouldBeEqual(reportCountBefore);
        }

        private void UpdateAndValidateMstrReport(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string> addDataset, List<string> expectedLabels, List<string> updateDataset)
        {
            if(_microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed())
                _microstrategyMaintenance.ClickonCancelButton();
            _microstrategyMaintenance.ClickOnEditIconByRowValue(addDataset[0]);
            var role = _microstrategyMaintenance.SideWindow.GetAvailableDropDownList("Role")[0];
            var unassignedRole = _microstrategyMaintenance.GetAvailableDropDownList("Role").Except(_microstrategyMaintenance.GetReportsGridListValueByCol(5)).ToList();

            for (int k = 0; k < expectedLabels.Count; k++)
            {
                if(expectedLabels[k]=="Role")
                    _microstrategyMaintenance.SideWindow.SelectDropDownListValueByLabel("Role", unassignedRole[0]);
                else
                _microstrategyMaintenance.SetInputFieldByLabel(expectedLabels[k], updateDataset[k]);
            }
            _microstrategyMaintenance.ClickonMicrostrategyReportSaveButton();
            _microstrategyMaintenance.WaitForWorkingAjaxMessage();
            _microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed().ShouldBeFalse("Edit form should be closed when save button is clickeds");
            _microstrategyMaintenance.IsRowPresentByValue(addDataset[0]).ShouldBeTrue("Is updated row present in grid list?");
            _microstrategyMaintenance.GetGridColValueByValue(unassignedRole[0], 2)
                .ShouldBeEqual(updateDataset[0], "Correct updated Report Name must be displayed");
            _microstrategyMaintenance.GetGridColValueByValue(unassignedRole[0], 3)
                .ShouldBeEqual(updateDataset[1], "Correct updated Project ID must be displayed");
            _microstrategyMaintenance.GetGridColValueByValue(unassignedRole[0], 4)
                .ShouldBeEqual(updateDataset[2], "Correct updated Report ID must be displayed");
            _microstrategyMaintenance.GetGridColValueByValue(unassignedRole[0], 5)
                .ShouldBeEqual(unassignedRole[0], "Correct updated Role must be displayed");
            _microstrategyMaintenance.ClickOnEditIconByRowValue(unassignedRole[0]);
            _microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed().ShouldBeTrue("Edit Form Should Display");
            _microstrategyMaintenance.ClickOnEditIconByRowValue(unassignedRole[0]);
            _microstrategyMaintenance.IsEditMicrostrategyReportFormDisplayed().ShouldBeFalse("Edit form should be closed when edit icon icon is clicked again");
            //_microstrategyMaintenance.GetAvailableDropDownList("Role")
            //    .ShouldNotContain(addDataset[3], "Previous Role must not be listed in dropdown");
            //_microstrategyMaintenance.GetAvailableDropDownList("Role")
            //    .ShouldContain(role, "Even though role is mapped to report but while editing role of editting record must be listed in dropdown");
        }

        private void DeleteRecordByValue(MicrostrategyMaintenancePage _microstrategyMaintenance,string value)
        {
            if (_microstrategyMaintenance.IsRowPresentByValue(value))
            {
                _microstrategyMaintenance.ClickOnDeleteIconByRowValue(value);
                _microstrategyMaintenance.ClickOkCancelOnConfirmationModal(true);
            }
        }


        private void Verify_page_is_opened_in_View_mode(MicrostrategyMaintenancePage _microstrategyMaintenance)
        {
            const string quadrant1 = "User Group";
            const string quadrant2 = "Nucleus Users";
            const string quadrant3 = "Microstrategy Reports";
            _microstrategyMaintenance.IsAddIconDisabled(quadrant2).ShouldBeTrue("Is add Nucleus Users Icon disabled?");

            _microstrategyMaintenance.ClickOnGridRowByValue("Test Automation");
            _microstrategyMaintenance.IsDeleteIconPresentByRow(quadrant2)
                .ShouldBeFalse("Is delete icon present in Nucleus Users qudrant?");

            _microstrategyMaintenance.IsAddIconDisabled(quadrant1).ShouldBeTrue("Is add User Group Icon disabled?");
            _microstrategyMaintenance.IsAddIconDisabled(quadrant3)
                .ShouldBeTrue("Is add Microstrategy Reports Users Icon disabled?");

            _microstrategyMaintenance.IsEditIconPresentInByRow(quadrant1).ShouldBeFalse("Is Edit icon present in Q1 quadrant");
            _microstrategyMaintenance.IsEditIconPresentInByRow(quadrant3).ShouldBeFalse("Is Edit icon present in Q3 quadrant");
            _microstrategyMaintenance.IsDeleteIconPresentByRow(quadrant1)
                .ShouldBeFalse("Is Delete icon present in Q1 quadrant");
            _microstrategyMaintenance.IsDeleteIconPresentByRow(quadrant3)
                .ShouldBeFalse("Is Delete icon present in Q3 quadrant");
        }

        private void VerifyValidationsInFirstQuadrant(MicrostrategyMaintenancePage _microstrategyMaintenance,List<string>createduser, List<string>labeList)
        {

            _microstrategyMaintenance.ClickOnAddIconByQuadrantName("User Group");
            _microstrategyMaintenance.ClickOnSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("validation message displayed when saved without any input?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Please enter missing fields.", "compare the error message");
            _microstrategyMaintenance.ClosePageError();
            for (int i = 0; i < labeList.Count()-2; i++)
            {
                _microstrategyMaintenance.SetInputFieldByLabel(labeList[i], string.Concat(Enumerable.Repeat("a1@_B", 21)));
                _microstrategyMaintenance.GetLengthOfTheInputFieldByLabel(labeList[i]).ShouldBeEqual(100, "Maxinum character limit of Group name field should be 100 alphanumeric characters");
            }
            // Insert different value in password fields
            _microstrategyMaintenance.SetInputFieldByLabel(labeList[2], createduser[0]);
            _microstrategyMaintenance.SetInputFieldByLabel(labeList[3], createduser[1]);
            _microstrategyMaintenance.ClickOnSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("validation message displayed when saved without matching password?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Passwords must match.", "validate the error message");
            _microstrategyMaintenance.ClosePageError();
            _microstrategyMaintenance.ClickonCancelButton();
            _microstrategyMaintenance.IsAddMicrostrategyAddUserGroupFormDisplayed().ShouldBeFalse("Is form displayed?");
            StringFormatter.PrintMessage("verify validation message for creation of duplicate user group");
            _microstrategyMaintenance.ClickOnAddIconByQuadrantName("User Group");
            for (int i = 0; i < labeList.Count; i++)
            {

                _microstrategyMaintenance.SetInputFieldByLabel(labeList[i], createduser[i]);

            }
            _microstrategyMaintenance.ClickOnSaveButton();
            _microstrategyMaintenance.IsPageErrorPopupModalPresent().ShouldBeTrue("validation message displayed when saved without matching password?");
            _microstrategyMaintenance.GetPageErrorMessage().ShouldBeEqual("Username or Group Name already exists, Please enter a unique value.", "validate the error message");
            _microstrategyMaintenance.ClosePageError();
            _microstrategyMaintenance.ClickonCancelButton();




        }


        #endregion


    }
}