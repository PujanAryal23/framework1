using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealCategoryManager
    {
        //#region PRIVATE FIELDS

        //private AppealCategoryManagerPage _appealCategoryManager;
        //private ProfileManagerPage _profileManager;
        //private AppealCreatorPage _appealCreator;
        //private AppealManagerPage _appealManager;
        //private List<string> _categoryIDList;
        //private List<string> _clientList;
        //private List<string> _allProductList;
        //private List<string> _categoryOrderList;
        //private List<string> _analystList;
        //private CommonValidations _commonValidations;

        //#endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        _appealCategoryManager = QuickLaunch.NavigateToAppealCategoryManager();
        //        _commonValidations = new CommonValidations(CurrentPage);
        //        try
        //        {
        //            RetrieveListFromDatabase();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //        }
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
        //        _appealCategoryManager = _appealCategoryManager.Logout().LoginAsHciAdminUser().NavigateToAppealCategoryManager();
        //    }

        //    if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealCategoryManager.GetStringValue()))
        //    {
        //        CurrentPage = _appealCategoryManager = CurrentPage.NavigateToAppealCategoryManager();
        //    }
        //}
        //#endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        //private void RetrieveListFromDatabase()
        //{
        //    _analystList = _appealCategoryManager.GetExpectedAnalystList();
        //    _allProductList = (List<string>)_appealCategoryManager.GetCommonSql.GetAllProductList();
        //    _clientList = _appealCategoryManager.GetCommonSql.GetAssignedClientListForUser(EnvironmentManager.Username);
        //}

        #region TEST SUITES
        [Test, Category("AppealCategoryManager1")]//US36215 + CAR-2863(CAR-2783)
        [Retrying(Times = 3)]
        [Order(1)]
        [NonParallelizable]
        public void Verify_edit_delete_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var headers = testData["Headers"].Split(',').ToList();
                var categoryId = testData["CategoryId"];
                var procCodeFrom = testData["ProcCodeFrom"];
                var procCodeTo = testData["ProcCodeTo"];
                var categoryOrder = testData["CategoryOrder"];
                var trigProcFrom = testData["TrigProcFrom"];
                var trigProcTo = testData["TrigProcTo"];
                var product = testData["Product"];
                var analyst = testData["Analyst"];
                var note = testData["Note"];
                var productAbbreviation = testData["ProductAbbreviation"].Split(',').ToList();
                var assignedClients =
                    _appealCategoryManager.GetAssignedClientListFromDB(automatedBase.EnvironmentManager.HciAdminUsername);
                var errorMessage = testData["ErrorMessage"];
                
                string[] newAppealData =
                {
                    categoryId, procCodeFrom, procCodeTo, categoryOrder, product, trigProcFrom, trigProcTo, analyst,
                    note, productAbbreviation[0]
                };
                try
                {
                    StringFormatter.PrintMessageTitle(" Create New Appeal Category ");
                    _appealCategoryManager.CreateAppealCategory(newAppealData);

                    StringFormatter.PrintLineBreak();
                    var categoryIds = _appealCategoryManager.GetGridViewSection.GetGridLabeList();
                    int indexOfUITEST = categoryIds.IndexOf(categoryId);
                    var assignedClient =
                        _appealCategoryManager.GetGridViewSection.GetValueInGridByColRow(3, indexOfUITEST + 1);
                    _appealCategoryManager.IsEditIconDisplayedInEachRow()
                        .ShouldBeTrue("Edit Icon is displayed on each row");
                    _appealCategoryManager.IsDeleteIconDisplayedInEachRow()
                        .ShouldBeTrue("Delete Icon is displayed on each row");
                    var orderValue = _appealCategoryManager.GetOrderValue(productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);
                    _appealCategoryManager.IsProcCodeFromAllowOnlyFiveCharacter()
                        .ShouldBeTrue("User able to enter only 5-chracter alphanumeric procedure code from");
                    _appealCategoryManager.IsProcCodeToAllowOnlyFiveCharacter()
                        .ShouldBeTrue("User able to enter only 5-chracter alphanumeric procedure code to");
                    //_appealCategoryManager.GetActiveOrderValueonEditSection()
                    //    .ShouldBeEqual(orderValue, "Default Category Order equal to appeal category row order value");
                    //_appealCategoryManager.IsCategoryOrderRecordCorrect()
                    //    .ShouldBeTrue("The Category Order List Value is Correct");

                    _appealCategoryManager.IsTrigProcFromAllowOnlyFiveCharacter()
                        .ShouldBeTrue("User able to enter only 5-chracter alphanumeric Trigger code from");
                    _appealCategoryManager.IsTrigProcToAllowOnlyFiveCharacter()
                        .ShouldBeTrue("User able to enter only 5-chracter alphanumeric Trigger code to");
                    //_appealCategoryManager.SelectProductOnEditSection("Coding Validation");

                    #region CAR-2863(CAR-2783)

                    StringFormatter.PrintMessage("Verification that Clients are shown side by side");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[0])
                        .ShouldBeTrue($"Is {headers[0]} section present?");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[1])
                        .ShouldBeTrue($"Is {headers[1]} section present?");

                    StringFormatter.PrintMessage(
                        "Verification that ALL should be on the top and clients are listed in alphabetical order");
                    _appealCategoryManager.GetAvailableAssignedList()
                        .ShouldContain("ALL", $"Is ALL option present in {headers[0]} list?");
                    _appealCategoryManager.GetAvailableAssignedList(2, false)[0].ShouldBeEqual(assignedClient,
                        $"{headers[1]} should list the clients assigned to appeal category");
                    var assignedClientList = _appealCategoryManager.GetAvailableAssignedList(2, false);
                    var availableClientsList = _appealCategoryManager.GetAvailableAssignedList(2);
                    availableClientsList.ShouldCollectionBeSorted(false, "Clients should be listed in ascending order");
                    availableClientsList[0].ShouldBeEqual("ALL", "ALL should be listed at the top");
                    availableClientsList.Remove("ALL");

                    StringFormatter.PrintMessage("Verification that ALL will be disabled when client code is selected");
                    foreach (var client in availableClientsList)
                    {
                        _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], client)
                            .ShouldBeFalse($"Is {client} disabled?");
                    }

                    _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], "ALL")
                        .ShouldBeTrue("Is All disabled?");

                    StringFormatter.PrintMessage("Verification of multiple assignment of clients");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, assignedClients[0]);
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, assignedClients[1]);
                    var multipleAssignedClientsList = _appealCategoryManager.GetAvailableAssignedList(2, false);
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    
                    _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                    _appealCategoryManager.GetGridViewSection.GetValueInGridByColRow(3, indexOfUITEST + 1).Split(',')
                        .ToList().ShouldCollectionBeEqual(multipleAssignedClientsList, "Updated clients should match");

                    StringFormatter.PrintMessage("Verification of saving without any client");
                    _appealCategoryManager.ClickOnEditIconByCategoryId(categoryId);
                    _appealCategoryManager.TransferAllAvailableAssignedRow(2, false);
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is error pop up present on saving without assigning any clients");
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual(errorMessage, "Error pop message should match");
                    _appealCategoryManager.ClosePageError();

                    StringFormatter.PrintMessage(
                        "Verification that client codes will be disabled when ALL is selected");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, "ALL");
                    _appealCategoryManager.GetAvailableAssignedList(2).ShouldCollectionBeEquivalent(assignedClients,
                        $"{headers[0]} list should match with that of database");
                    foreach (var client in availableClientsList)
                    {
                        _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], client)
                            .ShouldBeTrue($"Is {client} disabled?");
                    }

                    #endregion

                    _appealCategoryManager.SelectAnalystOnEditSection(analyst, true);
                    _appealCategoryManager.SetNote("UI Tests");

                    StringFormatter.PrintMessageTitle("Changed Category Order and Save ");
                    //_appealCategoryManager.SelectCategoryOrderOnEditSection(
                    //    (Convert.ToInt32(_appealCategoryManager.GetAppealCategoryLastOrderNumber())).ToString());
                    _appealCategoryManager.SelectCategoryOrderOnEditSection(
                        _appealCategoryManager.GetAppealCategoryLastOrderNumber());
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();

                    _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                    var orderList = _appealCategoryManager.GetCategoryOrderList();

                    var beforeShifted = new List<string>[3];
                    beforeShifted[0] = _appealCategoryManager.GetProductListAppealCategoryRow(true);
                    beforeShifted[1] = _appealCategoryManager.GetProcCodeListAppealCategoryRow(true);
                    beforeShifted[2] = _appealCategoryManager.GetTrigProcListAppealCategoryRow(true);

                    _appealCategoryManager.ClickOnEditIconByCategoryId(categoryId);
                    //_appealCategoryManager.SelectCategoryOrderOnEditSection("0");
                    //_appealCategoryManager.GetPageErrorMessage()
                    //    .ShouldBeEqual("Value should be greater than 0");
                    //_appealCategoryManager.ClosePageError();

                    StringFormatter.PrintMessage("Resetting to default");
                    _appealCategoryManager.TransferAllAvailableAssignedRow(2, false);
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, assignedClientList[0]);
                    _appealCategoryManager.SelectCategoryOrderOnEditSection("1");
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();

                    _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                    var afterShifted = new List<string>[3];
                    afterShifted[0] = _appealCategoryManager.GetProductListAppealCategoryRow(true);
                    afterShifted[1] = _appealCategoryManager.GetProcCodeListAppealCategoryRow(true);
                    afterShifted[2] = _appealCategoryManager.GetTrigProcListAppealCategoryRow(true);
                    _appealCategoryManager.IsCategoryOrderValueSorted().ShouldBeTrue(
                        "List should shift and adjust i.e. Category order is in ascending should be true.");

                    if (orderList[0] == 0)
                    {
                        beforeShifted[0].RemoveAt(0);
                        beforeShifted[1].RemoveAt(0);
                        beforeShifted[2].RemoveAt(0);
                        afterShifted[0].RemoveAt(0);
                        afterShifted[1].RemoveAt(0);
                        afterShifted[2].RemoveAt(0);
                    }

                    _appealCategoryManager.IsListShifted(beforeShifted, afterShifted)
                        .ShouldBeTrue("Subsequent category order numbers shifted");

                    _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("The selected category code will be deleted. Click Ok to proceed or Cancel.",
                            "Delete Confirm message");
                    _appealCategoryManager.ClickOkCancelOnConfirmationModal(false); //cancel button clicked
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            productAbbreviation[0],
                            procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo)
                        .ShouldBeTrue("Cancel button clicked, Appeal row present.");
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);

                    _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            productAbbreviation[0],
                            procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo)
                        .ShouldBeFalse("Ok button Clicked, Appeal row present.");
                }

                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();

                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);

                    if(_appealCategoryManager.GetGridViewSection.IsLoadMorePresent())
                        _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation[0],
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo, "ALL", true);

                    if (_appealCategoryManager.GetGridViewSection.IsLoadMorePresent())
                        _appealCategoryManager.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation[1], "NA", "NA");
                }
            }
        }

        [Test, Category("AppealCategoryManager1")] //TE-65
        [Retrying(Times = 3)]
        [NonParallelizable]
        public void Verify_filter_option_and_search_result_for_client_filter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedAppealCategorySearchFilterList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_category_filter_list").Values.ToList();
                List<string> _clientList =
                    _appealCategoryManager.GetCommonSql.GetAssignedClientListForUser(automatedBase.EnvironmentManager.Username);
                var analystListForSidePanelValidation =
                    _appealCategoryManager.GetAnalysListWithoutRestrictionDescription();
                List<string> _allProductList = _appealCategoryManager.GetCommonSql.GetAllProductList();
                try
                {
                    _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.GetSearchFiltersList()
                        .ShouldCollectionBeEqual(expectedAppealCategorySearchFilterList,
                            "Correct Filter options are displayed.");

                    StringFormatter.PrintMessageTitle("Category ID field Validation");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Category ID",
                        _appealCategoryManager.GetCommonSql.GetAppealCategoryIDList());

                    StringFormatter.PrintMessageTitle("Client field Validation");
                    _clientList.Insert(0, "ALL");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Client", _clientList);
                    _allProductList.Remove(ProductEnum.NEG.GetStringValue());
                    _allProductList.Remove(ProductEnum.OCI.GetStringValue());
                    _allProductList.Remove(ProductEnum.RXI.GetStringValue());
                    StringFormatter.PrintMessageTitle("Product field Validation");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Product", _allProductList);
                    StringFormatter.PrintMessageTitle("Category Order field Validation");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Category Order",
                        _appealCategoryManager.GetCommonSql.GetAppealCategoryOrderList());

                    StringFormatter.PrintMessageTitle("Analyst field Validation");
                    ValidateSingleDropDownForDefaultValueAndExpectedList("Analyst", analystListForSidePanelValidation);

                    StringFormatter.PrintMessageTitle("Verify search by Client filter");
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnClearLink();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();

                    var clientListAfterSearch = _appealCategoryManager.GetGridViewSection.GetGridListValueByCol(3);
                    var distinctList = clientListAfterSearch.Distinct().ToList();
                    distinctList.ShouldContain(ClientEnum.SMTST.ToString(),
                        "" + ClientEnum.SMTST + " client appeal category should be displayed.");

                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "ALL");
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    var allclientListAfterSearch = _appealCategoryManager.GetGridViewSection.GetGridListValueByCol(3)
                        .Distinct().ToList();
                    allclientListAfterSearch.Count.ShouldBeEqual(1, "Only Client=<ALL> should be displayed");
                    allclientListAfterSearch[0].ShouldBeEqual("ALL", "Search Result are Corrected?");
                }

                finally
                {
                    if (!_appealCategoryManager.IsFindCategoryCodeSectionPresent())
                        _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.ClickOnSearchIcon();
                }

                void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, bool order = true, bool validateCollection = true)
                {
                    var actualDropDownList = _appealCategoryManager.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    actualDropDownList.RemoveAll(string.IsNullOrEmpty);
                    if (collectionToEqual != null && validateCollection)
                        actualDropDownList.ShouldCollectionBeEquivalent(collectionToEqual, label + " List As Expected");
                    if (order)
                    {
                        switch (label)
                        {
                            case "Client":
                                actualDropDownList.Remove("ALL");
                                break;
                            case "Analyst":
                                _appealCategoryManager.IsAnalystInAscendingOrder(actualDropDownList);
                                break;
                            case "Category Order":
                                actualDropDownList.ToList().Select(int.Parse).ToList().IsInAscendingOrder()
                                    .ShouldBeTrue(label + " should be sorted in descending order.");
                                break;
                            default:
                                actualDropDownList.IsInAscendingOrder()
                                    .ShouldBeTrue(label + " should be sorted in alphabetical order.");
                                break;
                        }
                    }


                    _appealCategoryManager.GetSideBarPanelSearch.GetInputValueByLabel(label)
                        .ShouldBeNullorEmpty(label + " value defaults to empty");
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0],
                        false); //check for type ahead functionality
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
                    _appealCategoryManager.GetSideBarPanelSearch.GetInputValueByLabel(label)
                        .ShouldBeEqual(actualDropDownList[1], "User can select only a single option");

                }
            }
        }

        [Test] //CAR(3026)CAR-3000
        [Retrying(Times = 3)]
        public void Verify_appeal_category_creation_and_edit_functionality_by_editflag()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var flags = testData["Flags"].Split(',').ToList();
                var labels = testData["Labels"].Split(',').ToList();
                var categoryIds = testData["CategoryIds"].Split(',').ToList();
                var analysts = testData["Analysts"].Split(',').ToList();
                var placeholders = testData["Placeholders"].Split(',').ToList();
                var flagList = _appealCategoryManager.GetCVFlagListForAddCategoryFromDb();

                try
                {
                    StringFormatter.PrintMessage("Delete pre-existing appeal categories if any");
                    foreach (var categoryId in categoryIds.Take(4))
                    {
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
                    }

                    StringFormatter.PrintMessage("Verification that Flag label is present in the primary display");
                    _appealCategoryManager.GetGridViewSection.GetLabelInGridByColRow(5).ShouldBeEqual("Flag:",
                        "Flag label should be present in the primary display");

                    _appealCategoryManager.ClickAddNewCategory(clickOnly: true);
                    var productDropDownList =
                        _appealCategoryManager.GetSideBarPanelSearch.GetAvailableDropDownList("Product");
                    productDropDownList.Remove("");

                    StringFormatter.PrintMessage("Verify if Flag menu is present only for CV");
                    foreach (var product in productDropDownList)
                    {
                        _appealCategoryManager.ClickAddNewCategory(true, product);
                        if (product.Equals(productDropDownList[0]))
                            _appealCategoryManager.IsFlagInAddCategorySectionPresent()
                                .ShouldBeTrue($"Is Flag input present for {product}?");
                        else
                            _appealCategoryManager.IsFlagInAddCategorySectionPresent()
                                .ShouldBeFalse($"Is Flag input present for {product}?");
                    }

                    StringFormatter.PrintMessage("Verification if default Flag value is empty in Add Category");
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.GetFlagInputValue()
                        .ShouldBeNullorEmpty("Is by default flag value empty?");
                    _appealCategoryManager.GetFlagPlaceholder()
                        .ShouldBeEqual(placeholders[0], "Default message should match");


                    StringFormatter.PrintMessage(
                        "Verification if Flag drop down contains all flags for CV product in alphabetical order");
                    _appealCategoryManager.GetFlagDropDownList().Skip(2).ToList()
                        .ShouldCollectionBeEqual(flagList,
                            "CV flag list should match and should be in alphabetic order");

                    StringFormatter.PrintMessage(
                        "Verification when flag is selected, Proc Code From, Proc Code To, Trig Code From, Trig Code To will be disabled and nulled");
                    _appealCategoryManager.SetFlagInAddAndEditCategorySection(flags[0]);

                    foreach (var label in labels)
                    {
                        _appealCategoryManager.GetSideBarPanelSearch
                            .IsInputFieldForRespectiveLabelDisabled(label)
                            .ShouldBeTrue($"Is {label} disabled?");
                        _appealCategoryManager.GetSideBarPanelSearch.GetInputValueByLabel(label)
                            .ShouldBeNullorEmpty($"Is {label} nulled?");
                    }

                    StringFormatter.PrintMessage("Verification of Cancel functionality");
                    _appealCategoryManager.ClickOnCancelButton();
                    _appealCategoryManager.IsAddCategoryFormPresent().ShouldBeFalse("Is Add Category Form present?");

                    StringFormatter.PrintMessage("Verification of Clear in Add Category");
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.SetFlagInAddAndEditCategorySection(flags[0]);
                    _appealCategoryManager.GetFlagPlaceholder()
                        .ShouldBeEqual(flags[0], "Flag value should not be empty");
                    _appealCategoryManager.ScrollToLastPosition();
                    _appealCategoryManager.ClickAllClearFlagOptionInAddCategoryByOption("Clear");
                    _appealCategoryManager.GetFlagPlaceholder().ShouldBeEqual(placeholders[0],
                        "Placeholder should equal to default after selecting cancel");

                    StringFormatter.PrintMessage("Verification of creation of Category by single flag");
                    _appealCategoryManager.SetFlagInAddAndEditCategorySection(flags[0]);
                    SetAddCategoryFields(categoryIds[0]);
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",
                        categoryIds[0]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.GetGridViewSection.GetValueInGridByValueAndLabel(categoryIds[0], "Flag:")
                        .ShouldBeEqual(flags[0], "Flag input should match for single flag");

                    StringFormatter.PrintMessage("Verification of creation of Category by Multiple flags");
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.SetMultipleFlagInAddAndEditCategorySection(flags);
                    _appealCategoryManager.GetFlagPlaceholder().ShouldBeEqual(placeholders[1],
                        "Display value for multiple flags selected should match");
                    SetAddCategoryFields(categoryIds[1]);

                    StringFormatter.PrintMessage(
                        "Verification that saved values are shown in comma separated list in primary display");
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",
                        categoryIds[1]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.GetGridViewSection.GetValueInGridByValueAndLabel(categoryIds[1], "Flag:")
                        .ShouldBeEqual($"{flags[0]},{flags[1]}", "Flag input should match for multiple flags");

                    StringFormatter.PrintMessage("Verification of creation of Category by All flags");
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.ClickAllClearFlagOptionInAddCategoryByOption("All");
                    SetAddCategoryFields(categoryIds[2]);
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",
                        categoryIds[2]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.GetGridViewSection.GetValueInGridByValueAndLabel(categoryIds[2], "Flag:")
                        .ShouldBeEqual("ALL", "Flag input should match for All flags");

                    StringFormatter.PrintMessage(
                        "Creating Category CV appeal category with proc code and trig proc values to verify for edit");
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.SetCategoryId(categoryIds[3]);
                    _appealCategoryManager.SetProcCodeFromOnAddSection("1");
                    _appealCategoryManager.SetProcCodeToOnAddSection("2");
                    _appealCategoryManager.SetTrigProcFromOnAddSection("3");
                    _appealCategoryManager.SetTrigProcToOnAddSection("4");
                    _appealCategoryManager.SelectClientOnAddSection();
                    _appealCategoryManager.SelectAnalystOnAddSection(analysts[0]);
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analysts[1]);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();

                    StringFormatter.PrintMessage("Verification of flag is present only for CV category in edit form");
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",
                        categoryIds[4]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.GetGridViewSection.ClickEditDeleteIconByValueAndIcon(categoryIds[4],
                        "Edit Record");
                    _appealCategoryManager.IsFlagPresentOnEditSection()
                        .ShouldBeFalse("Is flag present for dental category?");
                    _appealCategoryManager.ClickOnCancelButton();

                    StringFormatter.PrintMessage(
                        "Verification of presence of flag in edit form for CV category, flag should be listed in alphabetical order and default message should match");
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",
                        categoryIds[3]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.GetGridViewSection.ClickEditDeleteIconByValueAndIcon(categoryIds[3],
                        "Edit Record");
                    _appealCategoryManager.IsFlagPresentOnEditSection()
                        .ShouldBeTrue("Is flag present for CV category?");
                    _appealCategoryManager.GetFlagDropDownList(true).Skip(2).ToList().ShouldCollectionBeEqual(flagList,
                        "CV flag list should match and should be in alphabetic order");
                    _appealCategoryManager.GetFlagPlaceholder(true)
                        .ShouldBeEqual(placeholders[0], "Default message should match");

                    StringFormatter.PrintMessage(
                        "Verification that proc code from, proc code to, trig proc from and trig proc to are disabled and nulled on selecting flag");
                    foreach (var label in labels)
                    {
                        _appealCategoryManager.GetSideBarPanelSearch
                            .GetInputValueByLabel(label)
                            .ShouldNotBeEmpty($"Is {label} empty?");
                    }

                    _appealCategoryManager.SetFlagInAddAndEditCategorySection(flags[0], true);

                    foreach (var label in labels)
                    {
                        _appealCategoryManager.GetSideBarPanelSearch
                            .IsInputFieldForRespectiveLabelDisabled(label)
                            .ShouldBeTrue($"Is {label} disabled?");
                        _appealCategoryManager.GetSideBarPanelSearch.GetInputValueByLabel(label)
                            .ShouldBeNullorEmpty($"Is {label} nulled?");
                    }

                    _appealCategoryManager.ClickOnCancelButton();

                    StringFormatter.PrintMessage("Verification of edit functionality with single flag");
                    EditFlag(flags[1], flags[1], true);

                    StringFormatter.PrintMessage("Verification of edit functionality with multiple flags");
                    EditFlag(placeholders[1], $"{flags[0]},{flags[1]}", false, true);

                    StringFormatter.PrintMessage("Verification of edit functionality with All flags");
                    EditFlag(placeholders[1], "ALL", false);
                }

                finally
                {
                    if (_appealCategoryManager.IsAddCategoryFormPresent())
                        _appealCategoryManager.ClickOnCancelButton();

                    StringFormatter.PrintMessage("Deleting the created Category Ids");
                    foreach (var categoryId in categoryIds.Take(4))
                    {
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
                    }
                }

                #region LOCAL METHODS

                void SetAddCategoryFields(string categoryId)
                {
                    _appealCategoryManager.SelectClientOnAddSection();
                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.SelectAnalystOnAddSection(analysts[0]);
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analysts[1]);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                }

                void EditFlag(string placeholderValue, string valueToMatch, bool single, bool mul = false)
                {
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID", categoryIds[0]);
                    _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealCategoryManager.WaitForWorking();
                    var previousFlagValue =
                        _appealCategoryManager.GetGridViewSection
                            .GetValueInGridByValueAndLabel(categoryIds[0], "Flag:");
                    _appealCategoryManager.GetGridViewSection.ClickEditDeleteIconByValueAndIcon(categoryIds[0],
                        "Edit Record");
                    StringFormatter.PrintMessage("Verification that saved value will be shown when form is opened");
                    if (single || mul)
                        _appealCategoryManager.GetFlagPlaceholder(true).ShouldBeEqual(previousFlagValue,
                            "Saved value should be shown when the form is opened");
                    _appealCategoryManager.ScrollToLastPosition();
                    _appealCategoryManager.ClickAllClearFlagOptionInAddCategoryByOption("Clear", true);
                    if (single) _appealCategoryManager.SetFlagInAddAndEditCategorySection(flags[1], true);
                    else if (mul) _appealCategoryManager.SetMultipleFlagInAddAndEditCategorySection(flags, true);
                    else _appealCategoryManager.ClickAllClearFlagOptionInAddCategoryByOption("All", true);
                    _appealCategoryManager.GetFlagPlaceholder(true).ShouldBeEqual(placeholderValue,
                        $"Placeholder should be {placeholderValue}");
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForSpinner();
                    _appealCategoryManager.WaitForPageToLoad();
                    _appealCategoryManager.GetGridViewSection.GetValueInGridByValueAndLabel(categoryIds[0], "Flag:")
                        .ShouldNotBeEqual(previousFlagValue, "Edited flag value should not match");
                    _appealCategoryManager.GetGridViewSection.GetValueInGridByValueAndLabel(categoryIds[0], "Flag:")
                        .ShouldBeEqual(valueToMatch, $"Flag value should be updated to {valueToMatch}");
                }

                #endregion
            }
        }



        [Test] //US67149, US67148 (partial) + CAR-667
        [Retrying(Times = 3)]
        public void Verify_audit_record_show_multiple_assignments()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                const string procCodeFrom = "MUL12";
                const string procCodeTo = "MUL34";
                const string trigProcFrom = "MUL56";
                const string trigProcTo = "MUL78";
                const string categoryId = "UIMULTEST";
                const string productAbbreviation = "CV";
                const string product = "Coding Validation";
                const string newAnalyst = "Test Automation (uiautomation)";
                const string newAnalystForRestrictedClaims = "Test Automation (uiautomation)";
                const string note = "UI MULTIPLE ANALYST";
                var analystList = testData["AnalystList"].Split(',').ToList();
                var clientCode = ClientEnum.SMTST.ToString();
                List<string> analystListInAuditHistory = new List<string>();
                List<string> restrictedAnalystListInAuditHistory = new List<string>();

                string[] newAppealData =
                {
                    categoryId, procCodeFrom, procCodeTo, "1", product, trigProcFrom, trigProcTo, newAnalyst,
                    newAnalystForRestrictedClaims, note,
                    productAbbreviation
                };

                try
                {
                    var auditAnalyst = newAnalyst.Split('(')[1].Split(')')[0];
                    var auditAnalystRestrictedClaims = newAnalystForRestrictedClaims.Split('(')[1].Split(')')[0];
                    _appealCategoryManager.CreateAppealCategory(newAppealData, clientCode, true);
                    _appealCategoryManager.ClickOnAppealCategoryCatId(categoryId);
                    _appealCategoryManager.IsAppealCategoryAuditHistoryIconPresent()
                        .ShouldBeTrue("Is Appeal Category Audit History Icon Present?");
                    _appealCategoryManager.ClickAppealCategoryAuditHistoryIcon();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    _appealCategoryManager.GetAuditRowCount()
                        .ShouldBeEqual(1, "Newly Created Appeal Category Audit should be only one");
                    StringFormatter.PrintMessageTitle("Verify updated record and multiple analyst in audit section");
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation,
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo);

                    foreach (var analyst in analystList)
                    {
                        _appealCategoryManager.SelectAnalystOnEditSection(analyst, false);
                        auditAnalyst = auditAnalyst + ", " + analyst.Split('(')[1].Split(')')[0];

                        _appealCategoryManager.SelectAnalystWithRestrictionAccessOnEditSection(analyst, false);
                        auditAnalystRestrictedClaims =
                            auditAnalystRestrictedClaims + ", " + analyst.Split('(')[1].Split(')')[0];
                    }

                    analystList.Insert(0, newAnalyst);
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    _appealCategoryManager.ClickOnAppealCategoryRowByCatIdAndGoToAuditHistory(categoryId);
                    _appealCategoryManager.GetAuditRowCount()
                        .ShouldBeEqual(2,
                            "Audit Row Count should equal to 2 after the appeal category record is updated");
                    _appealCategoryManager.IsEllipsisPresentInAuditSection()
                        .ShouldBeTrue("Is Ellipsis Present in Audit Section");

                    _appealCategoryManager.GetAnalaystInAuditSection()
                        .ShouldBeEqual(auditAnalyst, "Audit Analyst value should be equal");
                    _appealCategoryManager.GetAnalaystToolTipInAuditSection()
                        .ShouldBeEqual(auditAnalyst, "Audit Analyst tooltip value should be equal");

                    _appealCategoryManager.GetAnalaystInAuditSection(true)
                        .ShouldBeEqual(auditAnalyst, "Audit Analyst value should be equal");
                    _appealCategoryManager.GetAnalaystToolTipInAuditSection(true)
                        .ShouldBeEqual(auditAnalyst, "Audit Analyst tooltip value should be equal");

                    _appealCategoryManager.ClickOnAppealCategoryAnalystDetailsIcon();

                    int i = 1;

                    while (_appealCategoryManager.IsReviewerPresentInAnalystAssignedSideWindow(i + 1))
                    {
                        analystListInAuditHistory.Add(
                            _appealCategoryManager.GetAnalystListOnAnalystAssignment(true, ++i)[0]);
                    }

                    i++;
                    while (_appealCategoryManager.IsReviewerPresentInAnalystAssignedSideWindow(i + 1))
                    {
                        restrictedAnalystListInAuditHistory.Add(
                            _appealCategoryManager.GetAnalystListOnAnalystAssignment(true, ++i)[0]);
                    }

                    analystListInAuditHistory.ShouldCollectionBeEqual(analystList,
                        "All analyst assignments should be shown in secondary info ");
                    restrictedAnalystListInAuditHistory.ShouldCollectionBeEqual(analystList,
                        "All restricted analyst assignments should be shown in secondary info ");

                }

                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom + "-" + procCodeTo,
                        trigProcFrom + "-" + trigProcTo, clientCode);
                }
            }
        }

        [Test] //US67146 + CAR-667 + CAR 713
        [Retrying (Times = 3)]        
        public void Verify_edit_existing_category_analyst_assignments()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                   automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                
                string procCodeFrom = testData["PCF"];
                string procCodeTo = testData["PCT"];
                string categoryOrder = testData["CategoryOrder"];
                string trigProcFrom = testData["TPF"];
                string trigProcTo = testData["TPT"];
                string note = testData["Note"];
                string analystToSelectInRestrictedClaimsSection = testData["AnalystToSelectInRestrictedClaimsSection"];

                List<string> analystList = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value").Split(';')
                    .ToList();
                List<string> analystListForAnalystsWithRestrictedAccess =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value")
                        .Split(';').ToList();
                var order = _appealCategoryManager.GetAppealCategoryOrderOfLastRowWithOutFlag();
                _appealCategoryManager.ClickOnEditIconByCategoryOrder(order);
                //_appealCategoryManager.GetHeaderEditAppealCategory().ShouldBeEqual("Edit Appeal Category",
                //    "Edit Appeal Category title Should be equal.");
                _appealCategoryManager.IsRestrictedAndNonRestrictedAnalystsSectionsPresent().ShouldBeTrue(
                    "The edit form should have separate sections for " +
                    "assigning analysts for appeals with claims restricted access");
                _appealCategoryManager.GetAnalystListOnEditOption()
                    .ShouldCollectionBeEquivalent(_appealCategoryManager.GetExpectedAnalystList(),
                        "Analyst drop down list should equal to database value having <Appeal Can Be Assigned> authority in  <FirstName LastName (usrname)> - (Assigned Access Values Comma Separated) format");
                _appealCategoryManager.EditClientOnEditSection();
                //_appealCategoryManager.SelectProductOnEditSection(product);
                _appealCategoryManager.SelectCategoryOrderOnEditSection(categoryOrder);
                _appealCategoryManager.SetProcCodeFromOnEditSection(procCodeFrom);
                _appealCategoryManager.SetProcCodeToOnEditSection(procCodeTo);
                _appealCategoryManager.SetTrigProcFromOnEditSection(trigProcFrom);
                _appealCategoryManager.SetTrigProcToOnEditSection(trigProcTo);
                _appealCategoryManager.SetNote(note);

                var selectedAnalyst = _appealCategoryManager.GetSelectedAnalystList();

                var i = 0;
                foreach (string analyst in selectedAnalyst)
                {
                    if (!analystList.Contains(analyst))
                    {
                        analystList.Insert(i, analyst);
                        i++;
                    }
                }

                _appealCategoryManager.SelectAnalystOnEditSection(analystList[i], false);
                _appealCategoryManager.SelectAnalystOnEditSectionByEnterKey(analystList[i + 1]);

                if (i != 0)
                    _appealCategoryManager.SelectAnalystOnEditSectionByEnterKey(analystList[0]);

                if (i == 0)
                {
                    var analystListAfterAddingAnalyst = selectedAnalyst.Union(analystList).ToList();
                    _appealCategoryManager.GetSelectedAnalystList().ShouldCollectionBeEqual(
                        analystListAfterAddingAnalyst,
                        "Selected Analyst should be in same order as the list is selected.");
                }
                else
                {
                    _appealCategoryManager.GetSelectedAnalystList().ShouldCollectionBeEqual(analystList,
                        "Selected Analyst should be in same order as the list is selected.");
                }

                StringFormatter.PrintMessageTitle("Verifying the Restriction Access Analyst Section");

                _appealCategoryManager.GetAnalystListOnEditOption(false)
                    .ShouldCollectionBeEquivalent(_appealCategoryManager.GetExpectedAnalystList(false),
                        "Analyst drop down list should equal to database value having <Appeal Can Be Assigned> authority in  " +
                        "<FirstName LastName (usrname)> - (Assigned Access Values Comma Separated) format");

                _appealCategoryManager.SelectAnalystWithRestrictionAccessOnEditSection(
                    analystToSelectInRestrictedClaimsSection, false);
                var selectedAnalystWithRestrictedAccess =
                    _appealCategoryManager.GetSelectedAnalystListFromRestrictedAnalystSection();

                var j = 0;
                foreach (string analystWithRestrictedAccess in selectedAnalystWithRestrictedAccess)
                {
                    if (!analystListForAnalystsWithRestrictedAccess.Contains(analystWithRestrictedAccess))
                    {
                        analystListForAnalystsWithRestrictedAccess.Insert(j, analystWithRestrictedAccess);
                        j++;
                    }
                }

                _appealCategoryManager.SelectAnalystWithRestrictionAccessOnEditSection(
                    analystListForAnalystsWithRestrictedAccess[j], false);
                _appealCategoryManager.SelectAnalystWithRestrictedAccessOnEditSectionByEnterKey(
                    analystListForAnalystsWithRestrictedAccess[j + 1]);

                if (j != 0)
                    _appealCategoryManager.SelectAnalystWithRestrictedAccessOnEditSectionByEnterKey(
                        analystListForAnalystsWithRestrictedAccess[0]);

                if (j == 0)
                {
                    var restrictedAnalystListAfterAddingAnalyst =
                        selectedAnalystWithRestrictedAccess.Union(analystListForAnalystsWithRestrictedAccess);
                    _appealCategoryManager.GetSelectedAnalystListFromRestrictedAnalystSection()
                        .ShouldCollectionBeEqual(restrictedAnalystListAfterAddingAnalyst,
                            "Selected Analyst should be in same order as the list is selected.");
                }
                else
                    _appealCategoryManager.GetSelectedAnalystListFromRestrictedAnalystSection().ShouldCollectionBeEqual(
                        analystListForAnalystsWithRestrictedAccess,
                        "Selected Analyst should be in same order as the list is selected.");

                _appealCategoryManager.ClickDeleteAnalystFromRestrictionAccessAnalystSection(2);
                _appealCategoryManager.ClickOnCancelButton();
            }
        }

        [Test, Category("SmokeTest")]//US36210
        public void Verify_all_required_columns_with_appropriate_labels_appear()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();

                StringFormatter.PrintMessageTitle(" Checking Enabled Icons ");
                _appealCategoryManager.IsEditIconEnabled().ShouldBeTrue("Edit Icon is present and enabled");
                _appealCategoryManager.IsDeleteIconEnabled().ShouldBeTrue("Delete Icon is present and enabled");

                StringFormatter.PrintMessageTitle(" Checking Column Labels ");
                _appealCategoryManager.GetFlagLabel().ShouldBeEqual("Flag:", "Flag label is present"); //CAR-3000
                _appealCategoryManager.GetOrderLabel().ShouldBeEqual("Order:", "Order label is present");
                //_appealCategoryManager.GetProductLabel().ShouldEqual("Prod:","Product label is present");
                _appealCategoryManager.GetProcLabel().ShouldBeEqual("Proc:", "Proc label is present");
                _appealCategoryManager.GetTrigLabel().ShouldBeEqual("Trig:", "Trig label is present");
                _appealCategoryManager.GetClientLabel().ShouldBeEqual("Client:", "Client label is present");

                StringFormatter.PrintMessageTitle(" Checking Format of Values ");
                _appealCategoryManager.GetCategoryCode().Length
                    .ShouldBeGreater(0, "Category Code length is greater than 0");
                _appealCategoryManager.GetProductValue().Length.ShouldBeEqual(3, "Length of Product is 3.");
                VerifyThatProcValueIsInCorrectFormat(_appealCategoryManager.GetFirstProcValueExcludingDci());
                VerifyThatTrigValueIsInCorrectFormat(_appealCategoryManager.GetFirstTrigValueExcludingDci());
                //VerifyThatNameIsInCorrectFormat(_appealCategoryManager.GetAnalystValue());

                StringFormatter.PrintMessageTitle(" Checking as user having read only authority ");
                _appealCategoryManager.Logout()
                    .LoginAsUserHavingManageCategoryReadOnlyAuthority()
                    .NavigateToAppealCategoryManager();
                _appealCategoryManager.IsEditIconDisabled().ShouldBeTrue("Edit icon is disabled");
                _appealCategoryManager.IsDeleteIconDisabled().ShouldBeTrue("Delete icon is disabled");

                void VerifyThatProcValueIsInCorrectFormat(string proc)
                {
                    Regex.IsMatch(proc, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Proc value '" + proc + "' is in format XXXXX-XXXXX");
                }

                void VerifyThatTrigValueIsInCorrectFormat(string trig)
                {
                    if (String.IsNullOrEmpty(trig))
                        Console.WriteLine("The trig value is null or empty");
                    else
                        Regex.IsMatch(trig, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Trig value '" + trig + "' is in format XXXXX-XXXXX");
                }
            }
        }

        [Test] //US26209
        [Retrying(Times = 3)]
        public void Verify_navigation_and_security_of_manage_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);

                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Manager,
                    new List<string> {SubMenu.AppealCategoryManager},
                    RoleEnum.OperationsManager.GetStringValue(),
                    new List<string> {PageHeaderEnum.AppealCategoryManager.GetStringValue()}, 
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});

                _appealCategoryManager.Logout().LoginAsClientUser();
                _appealCategoryManager.IsAppealCategoryAssignmentSubMenuPresent()
                    .ShouldBeFalse("Appeal Category Manager submenu present for client user.");


                //_profileManager = QuickLaunch.NavigateToProfileManager();
                //_profileManager.ClickOnPrivileges();
                //_profileManager.IsAuthorityAssigned("Appeals - Manage Category Codes")
                //    .ShouldBeTrue("'Appeals - Manage Category Codes' authority is assigned.");
                //_profileManager.NavigateToAppealCategoryManager();
                //_appealCategoryManager.IsAppealCategoryAssignmentSubMenuPresent().ShouldBeTrue("Appeal Category Manager submenu present for authorized user.");
                //_appealCategoryManager.GetPageInsideTitle().ShouldBeEqual("Appeal Category Manager","Correct Page Title displayed inside the page.");
                //_appealCategoryManager.Logout().LoginAsClientUser();
                //_appealCategoryManager.IsAppealCategoryAssignmentSubMenuPresent().ShouldBeFalse("Appeal Category Manager submenu present for client user.");
                //_appealCategoryManager.Logout().LoginAsUserHavingNoManageCategoryAuthority();
                //_appealCategoryManager.IsAppealCategoryAssignmentSubMenuPresent().ShouldBeFalse("Appeal Category Manager submenu present for unauthorized user.");
            }
        }

        [Test]//US36216
        [Retrying(Times = 3)]
        public void Verify_that_audit_trail_of_category_code_records()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();

                _appealCategoryManager.ClickOnAppealCategoryRowAndGoToAuditHistory(excludeDci: true);
                _appealCategoryManager.IsAuditTrailContainerPresent()
                    .ShouldBeTrue("Audit Trail Container View is Present on the right side of the page");

                if (_appealCategoryManager.GetAuditRowCount() > 1)
                    for (int row = 1; row < _appealCategoryManager.GetAuditRowCount(); row++)
                    {
                        var firstRowDate =
                            Convert.ToDateTime(_appealCategoryManager.GetLastModifiedDateAuditSection(row));
                        var secondRowDate =
                            Convert.ToDateTime(_appealCategoryManager.GetLastModifiedDateAuditSection(row + 1));
                        (firstRowDate >= secondRowDate).ShouldBeTrue(
                            "Audit Trail is sorted with the most recent changes at the top");
                        if (row > 5)
                            break;
                    }
                else
                {
                    Console.WriteLine("Audit Trail entries has only one row");
                }

                _appealCategoryManager.IsCategoryIdPresentAuditSection().ShouldBeTrue("Category Order is Present");
                var product = _appealCategoryManager.GetProductValueAuditSection();
                _appealCategoryManager.GetProductValueAuditSection().Length.ShouldBeEqual((product == "CV") ? 2 : 3,
                    $"Product Abbreviation {product} is Displayed correctly");
                VerifyThatProcValueIsInCorrectFormat(_appealCategoryManager.GetProcCodeAuditSection());
                VerifyThatTrigValueIsInCorrectFormat(_appealCategoryManager.GetTrigProcAuditSection());

                //var analyst = _appealCategoryManager.GetAnalystValue().Split('(');
                //_appealCategoryManager.GetUserIdAuditSection()
                //    .ShouldEqual(analyst[analyst.Length - 1].Split(')')[0], "UserID is Displayed in Analyst");
                VerifyThatLastModifiedDateIsInCorrectFormat(_appealCategoryManager.GetLastModifiedDateAuditSection(1));
                _appealCategoryManager.IsLastModifiedUserPresentAuditSection()
                    .ShouldBeTrue("Last Modified User is Present");

                void VerifyThatProcValueIsInCorrectFormat(string proc)
                {
                    Regex.IsMatch(proc, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Proc value '" + proc + "' is in format XXXXX-XXXXX");
                }

                void VerifyThatTrigValueIsInCorrectFormat(string trig)
                {
                    if (String.IsNullOrEmpty(trig))
                        Console.WriteLine("The trig value is null or empty");
                    else
                        Regex.IsMatch(trig, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Trig value '" + trig + "' is in format XXXXX-XXXXX");
                }

                void VerifyThatLastModifiedDateIsInCorrectFormat(string date)
                {
                    Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The Last Modified Date'" + date + "' is in format MM/DD/YYYY");
                }
            }
        }

        [Test, Category("SmokeTest")]//US36214 + CAR-718
        public void Verify_search_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();

                string categoryId = _appealCategoryManager.GetCategoryCode(excludeDci: true);
                string order = _appealCategoryManager.GetOrderValue(excludeDci: true);
                string product = _appealCategoryManager.GetProductValue(excludeDci: true);
                //string analyst = _appealCategoryManager.GetAnalystValue(2);
                var proCode = _appealCategoryManager.GetProcValue(excludeDci: true).Split('-');
                string procCodeFrom = proCode[0];
                string procCodeTo = proCode[1];
                string trigProcFrom = "";
                string trigProcTo = "";
                string trigProcTo1 = "123ab";
                string claimViewRestrictedUser = "Claim_View Restriction_User (ui_claim_restrict)";
                var trigValue =
                    _appealCategoryManager.GetTrigValue(excludeDci: true);
                if (trigValue.Length > 0)
                {
                    var trigProc = trigValue.Split('-');
                    trigProcFrom = trigProc[0];
                    trigProcTo = trigProc[1];
                    if (trigProcTo == trigProcTo1)
                        trigProcTo1 = "123";
                }

                product = GetProductNameByProductAbbreviation();
                int appealrowcount = _appealCategoryManager.GetAppealCategoryRowCount();
                _appealCategoryManager.IsSearchIconPresent().ShouldBeTrue("Search Icon Present");
                try
                {
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.IsFindCategoryCodeSectionPresent()
                        .ShouldBeTrue("Find Category Codes Section Present");
                    _appealCategoryManager.ClickOnFindButton();
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                    {
                        _appealCategoryManager.GetPageErrorMessage()
                            .ShouldBeEqual("Search cannot be initiated without any criteria entered.");
                        _appealCategoryManager.ClosePageError();
                    }

                    Check_Find_Category_Codes_Search_Field_Empty_Or_Not();
                    _appealCategoryManager.IsCategoryIdSearchFieldValueSorted(1)
                        .ShouldBeTrue("Category ID sorted in alphabetical order");
                    _appealCategoryManager.IsProductSearchFieldValueSorted(2)
                        .ShouldBeTrue("Product sorted in alphabetical order");
                    _appealCategoryManager.IsCategoryOrderValueSearchFieldSorted()
                        .ShouldBeTrue("Category Order sorted in alphabetical order");
                    _appealCategoryManager.IsAnalystSearchFieldValueSorted(5)
                        .ShouldBeTrue("Analyst sorted in alphabetical order");
                    _appealCategoryManager.DoesAnalystSearchListContainAnalyst(5, claimViewRestrictedUser)
                        .ShouldBeTrue("Analyst without any restriction should also be present in the dropdown list");
                    _appealCategoryManager.SelectCategoryIdOnFindSection(categoryId);
                    _appealCategoryManager.SelectProductOnFindSection(product);
                    _appealCategoryManager.SelectCategoryOrderOnFindSection(order);
                    //_appealCategoryManager.SelectAnalystOnFindSection(claimViewRestrictedUser);
                    _appealCategoryManager.SetProcCodeFromOnFindSection(procCodeFrom);
                    _appealCategoryManager.SetProcCodeToOnFindSection(procCodeTo);
                    _appealCategoryManager.SetTrigProcFromOnFindSection(trigProcFrom);
                    _appealCategoryManager.SetTrigProcToOnFindSection(trigProcTo1);
                    _appealCategoryManager.ClickOnFindButton();
                    _appealCategoryManager.GetEmptySearchMessage().ShouldBeEqual("No matching records were found.");
                    _appealCategoryManager.GetAppealCategoryRowCount().ShouldBeEqual(0, "Appeal Category Row Reset");
                    _appealCategoryManager.SetTrigProcToOnFindSection(trigProcTo);
                    _appealCategoryManager.ClickOnFindButton();
                    _appealCategoryManager.IsFindCategoryCodeSectionPresent()
                        .ShouldBeFalse("Find Category Codes Section Closed");
                    _appealCategoryManager.GetAppealCategoryRowCount()
                        .ShouldBeGreater(0, "Atlest One Matching Record Found");
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.GetAppealCategoryRowCount()
                        .ShouldBeEqual(appealrowcount, "Appeal Category Row Reset");
                    Check_Find_Category_Codes_Search_Field_Empty_Or_Not();
                    _appealCategoryManager.ClickOnSearchIcon();
                }
                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();
                    if (_appealCategoryManager.IsFindCategoryCodeSectionPresent())
                    {
                        _appealCategoryManager.ClickOnClearButton();
                        _appealCategoryManager.ClickOnSearchIcon();
                    }
                }

                void Check_Find_Category_Codes_Search_Field_Empty_Or_Not()
                {
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(1).ShouldBeTrue("Category ID is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(2).ShouldBeTrue("Client is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(3).ShouldBeTrue("Product is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(4).ShouldBeTrue("Ctegory Order is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(5).ShouldBeTrue("Analyst is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(6).ShouldBeTrue("Proc Code From is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(7).ShouldBeTrue("Proc Codee To is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(8).ShouldBeTrue("Trig Proc From is Empty by default");
                    _appealCategoryManager.IsSearchFieldFindSectionEmptyorNull(9).ShouldBeTrue("Trig Proc To is Empty by default");
                }

                string GetProductNameByProductAbbreviation()
                {
                    switch (product)
                    {
                        case "CV":
                            product = "Coding Validation";
                            break;
                        case "DCA":
                            product = "Dental Claim Accuracy";
                            break;
                        case "FCI":
                            product = "FacilityClaim Insight";
                            break;
                        case "FFP":
                            product = "FraudFinder Pro";
                            break;
                        case "RxI":
                            product = "PharmaClaim Insight";
                            break;
                        case "NEG":
                            product = "Negotiation";
                            break;
                        case "OCI":
                            product = "OncologyClaim Insight";
                            break;
                        case "COB":
                            product = "Coordination of Benefits";
                            break;
                    }
                    return product;
                }
            }
        }

        [Test]//US36211+US36212
        public void Verify_sorting_for_primary_secondary_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, TestExtensions.TestName).Values.ToList();
                try
                {
                    //_appealCategoryManager.GetFilterOptionList()
                    //    .ShouldCollectionBeEqual(filterOptions, "Filter Option List Equal");
                    _appealCategoryManager.IsCategoryOrderValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Category Order when Page Loaded");
                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(1);
                    _appealCategoryManager.IsCategoryValueSorted()
                        .ShouldBeTrue("Records are Sorted in Alphabetical Order By Category");
                    _appealCategoryManager.ClickOnFilterOptionList(2);
                    _appealCategoryManager.IsCategoryOrderValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Category Order ");
                    _appealCategoryManager.ClickOnFilterOptionList(3);
                    _appealCategoryManager.IsProductValueSorted()
                        .ShouldBeTrue("Records are Sorted in Alphabetical Order By Product");
                    _appealCategoryManager.ClickOnFilterOptionList(4);
                    _appealCategoryManager.IsProcCodeFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code From");
                    _appealCategoryManager.ClickOnFilterOptionList(5);
                    _appealCategoryManager.IsProcCodeToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code To");
                    _appealCategoryManager.ClickOnFilterOptionList(6);
                    _appealCategoryManager.IsTrigProcFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc From");
                    _appealCategoryManager.ClickOnFilterOptionList(7);
                    _appealCategoryManager.IsTrigProcToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc To");
                    _appealCategoryManager.ClickOnFilterOptionList(8);
                    //_appealCategoryManager.IsLastModifiedDateSorted()
                    //    .ShouldBeTrue("Records are in Descending Order By Last Modified Date");
                }
                finally
                {
                    _appealCategoryManager.ClickOnFilterOptionList(2);
                }
            }
        }

        [Test, Category("SmokeTest")]//US36213, US66666 + CAR-661 + CAR-2781[CAR-2857]
        public void Verify_user_can_create_new_category_codes_and_gets_proper_validation_message()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var categoryId = testData["CategoryId"];
                var headers = testData["Headers"].Split(',').ToList();
                var procCodeFrom = testData["ProcCodeFrom"];
                var procCodeTo = testData["ProcCodeTo"];
                var categoryOrder = testData["CategoryOrder"];
                var trigProcFrom = testData["TrigProcFrom"];
                var trigProcTo = testData["TrigProcTo"];
                var product = testData["Product"];
                var clientCode = testData["Client"];
                var analyst = testData["Analyst"];
                //var note = testData["Note"];
                var productAbbreviation = testData["ProductAbbreviation"];
                
                // var analystsForMultipleAssignment = testData["AnalystsForMultipleAssignment"].Split(';').ToList();
                List<string> analystListInAnalystAssignmentSideWindow = new List<string>();
                List<string> restrictedAnalystListInAnalystAssignmentSideWindow = new List<string>();
                List<string> _allProductList = (List<string>)_appealCategoryManager.GetCommonSql.GetAllProductList();
                string anotherClient = "";
                _appealCategoryManager.IsAddCategoryCodeIconEnabled().ShouldBeTrue("Add Category Code icon is enabled");

                try
                {

                    #region "Existing Test Case"

                    StringFormatter.PrintMessageTitle(" Delete UITEST category if already present ");

                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom + "-" + procCodeTo, trigProcFrom + "-" + trigProcTo, clientCode);
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode("DCA",
                        "NA", "NA", clientCode);

                    _appealCategoryManager.ClickAddNewCategory();

                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("A client must be selected before the record can be saved.",
                            "Error Message");
                    _appealCategoryManager.ClosePageError();

                    #region CAR-2781[CAR-2857]

                    StringFormatter.PrintMessageTitle("Verify Product List Does Not Contain NEG,OCI and RXI");
                    _allProductList.Remove(ProductEnum.NEG.GetStringValue());
                    _allProductList.Remove(ProductEnum.OCI.GetStringValue());
                    _allProductList.Remove(ProductEnum.RXI.GetStringValue());
                    var productDropDownList =
                        _appealCategoryManager.GetSideBarPanelSearch.GetAvailableDropDownList("Product");
                    productDropDownList.RemoveAll(string.IsNullOrEmpty);
                    productDropDownList.ShouldCollectionBeEquivalent(_allProductList, "Product dropdown As Expected ?");

                    StringFormatter.PrintMessage("Verification that Clients are shown side by side");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[0])
                        .ShouldBeTrue($"Is {headers[0]} section present?");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[1])
                        .ShouldBeTrue($"Is {headers[1]} section present?");

                    StringFormatter.PrintMessage(
                        "Verification that ALL should be on the top and clients are listed in alphabetical order");
                    var availableClientsList = _appealCategoryManager.GetAvailableAssignedList(2);
                    var assignedClientListFromDb =
                        _appealCategoryManager.GetAssignedClientListFromDB(automatedBase.EnvironmentManager.HciAdminUsername);
                    assignedClientListFromDb.Insert(0, "ALL");
                    availableClientsList.ShouldCollectionBeEqual(assignedClientListFromDb,
                        "Client Search List As Expected");
                    availableClientsList.ShouldCollectionBeSorted(false, "Clients should be listed in ascending order");
                    availableClientsList[0].ShouldBeEqual("ALL", "ALL should be listed at the top");
                    availableClientsList.Remove("ALL");


                    StringFormatter.PrintMessage("Verification that ALL will be disabled when client code is selected");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientsList[0]);
                    foreach (var client in availableClientsList)
                    {
                        if (client != availableClientsList[0])
                            _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], client)
                                .ShouldBeFalse($"Is {client} disabled?");
                    }

                    _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], "ALL")
                        .ShouldBeTrue("Is All disabled?");

                    StringFormatter.PrintMessage("Verification of multiple assignment of clients");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientsList[1]);
                    var multipleAssignedClientsList = _appealCategoryManager.GetAvailableAssignedList(2, false);
                    multipleAssignedClientsList.ShouldCollectionBeEqual(
                        new List<string>() { availableClientsList[0], availableClientsList[1] },
                        "Multiple Clients List Present In Assigned Client List ?");

                    StringFormatter.PrintMessage("Verification that client codes will be disabled when ALL is selected");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientsList[0], false);
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientsList[1], false);
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, "ALL");
                    _appealCategoryManager.GetAvailableAssignedList(2).ShouldCollectionBeEquivalent(availableClientsList,
                        $"{headers[0]} list should not contain ALL");
                    foreach (var client in availableClientsList)
                    {
                        _appealCategoryManager.IsAvailableAssignedClientsDisabled(headers[0], client)
                            .ShouldBeTrue($"Is {client} disabled?");
                    }

                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, "ALL", false);

                    #endregion

                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, ClientEnum.SMTST.ToString());
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("A category ID value must be entered before the record can be saved.",
                            "Error Message");
                    _appealCategoryManager.ClosePageError();

                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();

                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                            "Error Message");
                    _appealCategoryManager.ClosePageError();

                    _appealCategoryManager.SetProcCodeFromOnAddSection(procCodeFrom);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                            "Error Message");
                    _appealCategoryManager.ClosePageError();


                    _appealCategoryManager.SetProcCodeToOnAddSection(procCodeTo);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();

                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "An analyst must be selected before the record can be saved.",
                            "Error Message");
                    _appealCategoryManager.ClosePageError();


                    var defaultCategoryOrder = _appealCategoryManager.GetCategoryOrderOnAddOptionValue();

                    var loadMoreValue = _appealCategoryManager.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
                    var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                    for (var i = 0; i < count; i++)
                    {
                        _appealCategoryManager.ClickOnLoadMore();
                    }

                    defaultCategoryOrder.ShouldBeEqual(
                        (int.Parse(_appealCategoryManager.GetAppealCategoryLastOrderNumber()) + 1).ToString(),
                        "The Category Order field will default to show the next value available (all categories with product value other than DCA +1).");

                    #region CAR-909 : Update the category order selector from a drop down to a text field

                    _appealCategoryManager.SelectCategoryOrderOnAddSection("");
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("A category order must be selected before the record can be saved.");
                    _appealCategoryManager.ClosePageError();
                    //_appealCategoryManager.SelectCategoryOrderOnAddSection("0");
                    //_appealCategoryManager.GetPageErrorMessage()
                    //    .ShouldBeEqual("Value should be greater than 0");
                    //_appealCategoryManager.ClosePageError();
                    _appealCategoryManager.SelectCategoryOrderOnAddSection((int.Parse(defaultCategoryOrder) + 1)
                        .ToString());

                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual(string.Format("Order higher than {0} cannot be selected.", defaultCategoryOrder));
                    _appealCategoryManager.ClosePageError();

                    #endregion

                    _appealCategoryManager.SelectCategoryOrderOnAddSection(categoryOrder);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.WaitToLoadPageErrorPopupModal();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("An analyst must be selected before the record can be saved.", "Error Message");
                    _appealCategoryManager.ClosePageError();
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst);
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analyst);
                    _appealCategoryManager.SetTrigProcFromOnAddSection(trigProcFrom);
                    _appealCategoryManager.SetTrigProcToOnAddSection(trigProcTo);

                    _appealCategoryManager.SelectProductOnAddSection(product);


                    //_appealCategoryManager.SelectClientOnAddSection(clientCode);
                    //_appealCategoryManager.SetNote(note);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                        productAbbreviation, procCodeFrom + "-" + procCodeTo,
                        trigProcFrom + "-" + trigProcTo).ShouldBeTrue("Newly created Appeal Category is listed.");

                    StringFormatter.PrintMessageTitle(" Checking Category Order ");

                    //The category with order 5 should be present in row 6
                    var DCIAppealCategoryRowCount = _appealCategoryManager.GetAppealCategoryRowCountByProduct("DCA");
                    var rowForUiTest = int.Parse(categoryOrder) + DCIAppealCategoryRowCount + 1;
                    _appealCategoryManager.GetCategoryCode(rowForUiTest)
                        .ShouldBeEqual(categoryId, "Correct category at the given order/row.");
                    _appealCategoryManager.ClickOnAppealCategoryRow(rowForUiTest);
                    /*List<string> analystusername = analyst.Substring(analyst.IndexOf('(') + 1,
                        analyst.IndexOf(')') - analyst.IndexOf('(') - 1).Split(';').ToList();*/

                    int j = 1;

                    while (_appealCategoryManager.IsReviewerPresentInAnalystAssignedSideWindow(j + 1))
                    {
                        analystListInAnalystAssignmentSideWindow.Add(
                            _appealCategoryManager.GetAnalystListOnAnalystAssignment(true, ++j)[0]);
                    }

                    j++;
                    while (_appealCategoryManager.IsReviewerPresentInAnalystAssignedSideWindow(j + 1))
                    {
                        restrictedAnalystListInAnalystAssignmentSideWindow.Add(
                            _appealCategoryManager.GetAnalystListOnAnalystAssignment(true, ++j)[0]);
                    }

                    analystListInAnalystAssignmentSideWindow.ShouldCollectionBeEqual(analyst.Split(';').ToList<string>(),
                        "Assigned Analyst is showing up as expected" +
                        "in the Analyst Assignment side window");
                    restrictedAnalystListInAnalystAssignmentSideWindow.ShouldCollectionBeEqual(
                        analyst.Split(';').ToList<string>(), "Restricted Assigned Analyst is showing up as expected" +
                                                             "in the Analyst Assignment side window");

                    _appealCategoryManager.ClickAddNewCategory();

                    /***Removed the test for saving duplicate category with same client. This will now be verified by the analysts.***/
                    /*
                     StringFormatter.PrintMessageTitle(" Saving Duplicate Category with Same Client ");
                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.SetProcCodeFromOnAddSection(procCodeFrom);

                    _appealCategoryManager.SetProcCodeToOnAddSection(procCodeTo);
                    _appealCategoryManager.SetTrigProcFromOnAddSection(trigProcFrom);
                    _appealCategoryManager.SetTrigProcToOnAddSection(trigProcTo);

                    _appealCategoryManager.SelectProductOnAddSection(product);
                    _appealCategoryManager.SelectCategoryOrderOnAddSection(categoryOrder);
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst);
                    _appealCategoryManager.SelectClientOnAddSection(clientCode);

                    //_appealCategoryManager.SetNote(note);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("The values entered match an existing category code record.", "Error Message");
                    _appealCategoryManager.ClosePageError();
                    */

                    StringFormatter.PrintMessageTitle(" Saving Duplicate Category with Different Client ");
                    anotherClient =
                        assignedClientListFromDb.First(x => x != "ALL" && x != clientCode); //Select another client

                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.SetProcCodeFromOnAddSection(procCodeFrom);

                    _appealCategoryManager.SetProcCodeToOnAddSection(procCodeTo);
                    _appealCategoryManager.SetTrigProcFromOnAddSection(trigProcFrom);
                    _appealCategoryManager.SetTrigProcToOnAddSection(trigProcTo);

                    _appealCategoryManager.SelectProductOnAddSection(product);
                    _appealCategoryManager.SelectCategoryOrderOnAddSection(categoryOrder);
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst);

                    _appealCategoryManager.SelectClientOnAddSection(anotherClient);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.WaitForWorking();
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            productAbbreviation, procCodeFrom + "-" + procCodeTo,
                            trigProcFrom + "-" + trigProcTo, anotherClient)
                        .ShouldBeTrue("Newly created Appeal Category is listed for client " + anotherClient + ".");

                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.ClickOnCancelButton();
                    _appealCategoryManager.IsAddNewCategoryCodeSectionDisplayed()
                        .ShouldBeFalse("Add New Category section displayed");

                    #endregion
                }
                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();

                    //Delete the newly created categories
                    _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation, procCodeFrom + "-" + procCodeTo,
                        trigProcFrom + "-" + trigProcTo, clientCode);
                    _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);

                    //Delete the newly created categories for anotherClient
                    _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation, procCodeFrom + "-" + procCodeTo,
                        trigProcFrom + "-" + trigProcTo, anotherClient);
                    _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);
                }


            }
        }

        

        [Test] //CAR 816 + 819 + CAR-2781 + CAR-2863[CAR-2783]
        [Retrying(Times = 3)]
        public void Validate_create_and_edit_delete_DCI_appeal_category()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var analyst = testData["Analyst"];
                //var note = testData["Note"];
                var analystsForMultipleAssignment = testData["AnalystsForMultipleAssignment"].Split(';').ToList();
                var clientCode = testData["Client"];
                var headers = testData["Headers"].Split(',').ToList();

                try
                {
                    #region CAR-816 : Verification of Add Category for DCA Product

                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode("DCA", "NA", "NA");
                    //CAR-816 starts here. Verifies create new appeal category form when for DCA. 
                    StringFormatter.PrintMessageTitle("Verifying Input Fields for Add Category Form");
                    VerifyInputFieldsGettingDisplayedForDCIProductInAddCategory();

                    StringFormatter.PrintMessageTitle(
                        "Verifying input Client and Analyst Dropdown fields have expected values");
                    var availableClientList = _appealCategoryManager.GetAvailableAssignedList();
                    var clientListFromDB = _appealCategoryManager.GetClientListForDCI(automatedBase.EnvironmentManager.Username);

                    #region CAR-2781[CAR-2857]

                    StringFormatter.PrintMessage("Verification that Clients are shown side by side");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[0])
                        .ShouldBeTrue($"Is {headers[0]} section present?");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[1])
                        .ShouldBeTrue($"Is {headers[1]} section present?");

                    availableClientList.ShouldNotContain("ALL",
                        "'ALL' client should not be present in the client dropdown list for DCA");
                    StringFormatter.PrintMessage(
                        "Verify only a single client can be selected and  other clients are disabled ");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientList[0]);
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientList[1]);
                    foreach (var client in availableClientList)
                    {
                        if (client != availableClientList[0])
                            _appealCategoryManager.IsAvailableAssignedClientsDisabled("Available Clients", client)
                                .ShouldBeTrue($"Is {client} disabled?");
                    }

                    _appealCategoryManager.GetAvailableAssignedList(2, false).Count
                        .ShouldBeEqual(1, "Only Single Client Should Be Selected");
                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, availableClientList[0], false);

                    #endregion


                    availableClientList.ShouldCollectionBeEquivalent(clientListFromDB,
                        "The client dropdown list matches with that from the DB");

                    var analystListFromDropDown = _appealCategoryManager.GetAnalystListForDCIOnAddOption();
                    var analystListFromDB = _appealCategoryManager.GetExpectedAnalystList();

                    _appealCategoryManager.IsAnalystInAscendingOrder(analystListFromDropDown)
                        .ShouldBeTrue("Analyst Names in the dropdown is in alphabetical order");

                    analystListFromDropDown.ShouldCollectionBeEquivalent(analystListFromDB,
                        "Analysts in the dropdown are matching with the DB");

                    StringFormatter.PrintMessageTitle("Checking Required Fields and Validation");

                    _appealCategoryManager.SelectClientOnAddSection(clientCode);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("An analyst must be selected before the record can be saved.");
                    _appealCategoryManager.ClosePageError();

                    _appealCategoryManager.ClickOnAvailableAssignedRow(2, clientCode, false);
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst, true);
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetPageErrorMessage()
                        .ShouldBeEqual("A client must be selected before the record can be saved.");
                    _appealCategoryManager.ClosePageError();

                    StringFormatter.PrintMessageTitle(
                        "Verifying Moving the Analysts UP or DOWN in case of multiple analysts in the assigned field");

                    _appealCategoryManager.SelectClientOnAddSection(clientCode);

                    foreach (var analystToAssign in analystsForMultipleAssignment)
                    {
                        _appealCategoryManager.SelectAnalystOnAddSection(analystToAssign, true);
                    }

                    var originallyAssignedAnalystsOrder = _appealCategoryManager.GetSelectedAnalystListForDCI();

                    //The first analyst is moved down towards until it reaches the last position
                    foreach (var temp in originallyAssignedAnalystsOrder)
                    {
                        _appealCategoryManager.ClickMoveDownAnalystByAnalystName(originallyAssignedAnalystsOrder[0]);
                    }

                    var orderOfAnalysts = new List<int> {1, 2, 0};
                    var assignedAnalystsAfterOrderChange = _appealCategoryManager.GetSelectedAnalystListForDCI();

                    for (var i = 0; i < assignedAnalystsAfterOrderChange.Count; i++)
                    {
                        assignedAnalystsAfterOrderChange[i]
                            .ShouldBeEqual(originallyAssignedAnalystsOrder[orderOfAnalysts[i]]);
                    }

                    //The last analyst in the list is moved upward until it reaches the top position. Hence, making it the originally assigned order.
                    foreach (var temp in originallyAssignedAnalystsOrder)
                    {
                        _appealCategoryManager.ClickMoveUpAnalystByAnalystName(originallyAssignedAnalystsOrder[0]);
                    }

                    assignedAnalystsAfterOrderChange = _appealCategoryManager.GetSelectedAnalystListForDCI();
                    assignedAnalystsAfterOrderChange.ShouldCollectionBeEqual(originallyAssignedAnalystsOrder,
                        "The order of the analysts is as expected after the analysts are moved");

                    StringFormatter.PrintMessageTitle(
                        "Verifying saving the DCA Appeal Category and Audit History and Grid labels");
                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.GetCategoryCode().ShouldBeEqual("DENTAL",
                        "Newly Created DCA Appeal Category Has 'DENTAL' as Category ID");
                    _appealCategoryManager.GetOrderValue().ShouldBeEqual("NA",
                        "Newly Created DCA Appeal Category does not have any order assigned to it and shows 'NA' in the grid");

                    _appealCategoryManager.ClickOnDentalAppealCategoryRowByClient();
                    var analystList = _appealCategoryManager.GetAnalystListOnAnalystAssignment();
                    analystList.ShouldCollectionBeEqual(originallyAssignedAnalystsOrder,
                        "Assigned Analysts are appearing correctly 'Analyst Assignment side window'");
                    _appealCategoryManager.ClickAppealCategoryAuditHistoryIcon();
                    _appealCategoryManager.GetAuditRowCount().ShouldBeEqual(1,
                        "Newly created Appeal Category should have only one record in the audit history");

                    var analystListInAuditHistory = _appealCategoryManager.GetAnalaystInAuditSection().Split(',')
                        .ToList().Select(
                            x => x.Trim()).ToList();
                    var expectedAnalystUserIds = originallyAssignedAnalystsOrder.Select(s =>
                        s.Substring(s.LastIndexOf('(') + 1, s.LastIndexOf(')') - 1 - s.LastIndexOf('('))).ToList();

                    analystListInAuditHistory.ShouldCollectionBeEqual(expectedAnalystUserIds, "");
                    _appealCategoryManager.GetNotesValueAuditSection()
                        .ShouldBeNullorEmpty("Notes should be empty when category is initially created");
                    _appealCategoryManager.GetProductValueAuditSection()
                        .ShouldBeEqual(ProductEnum.DCA.ToString(), "Product value should be DCA");
                    _appealCategoryManager.GetCategoryIdAuditSection()
                        .ShouldBeEqual("DENTAL", "Category Id should be correct");

                    //TO DO
                    //Verify Category order, Proc and Tri 

                    #endregion

                    #region CAR-819 : View and Edit Dental Appeal Categories



                    StringFormatter.PrintMessage(
                        "Verifying the data entered while creating the category are showing up in the edit form");
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(
                        ProductEnum.DCA.ToString(), "NA", "NA");
                    var assignedAnalysts = _appealCategoryManager.GetSelectedAnalystListForDCI().Select(s =>
                        s.Split('-')[0].Trim()).ToList();
                    assignedAnalysts.ShouldCollectionBeEqual(analystsForMultipleAssignment,
                        "Assigned analysts are showing correctly in edit form");

                    #region CAR-2863[CAR-2783]

                    StringFormatter.PrintMessage(
                        "Verification that DCA product appeal categories client selection cannot be updated");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[1])
                        .ShouldBeFalse($"Is {headers[1]} component present?");
                    _appealCategoryManager.IsAvailableAssignedClientsComponentPresentByLabel(headers[0])
                        .ShouldBeFalse($"Is {headers[0]} component present?");

                    #endregion

                    StringFormatter.PrintMessageTitle(
                        "Changes are made in the edit form and verifying if the changes are saved");
                    _appealCategoryManager.SetNote("Test");
                    _appealCategoryManager.WaitForStaticTime(1000);
                    _appealCategoryManager.ClickDeleteAnalyst(1, true);
                    analystsForMultipleAssignment.RemoveAt(0);
                    //Removing the deleted analyst from the test data as well
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorking();

                    StringFormatter.PrintMessage("Verifying Analyst Assignment Side Window : After Update");
                    _appealCategoryManager.ClickOnAppealRowCategoryByProductProcCodeTrigCodePresent("DCA", "NA", "NA");
                    _appealCategoryManager.ClickOnAppealCategoryAnalystDetailsIcon();
                    _appealCategoryManager.GetAnalystListOnAnalystAssignment()
                        .ShouldCollectionBeEqual(analystsForMultipleAssignment,
                            "Assigned analysts is correctly shown in the Analyst Assignment side window after the update");

                    StringFormatter.PrintMessage("Verifying Audit History Side Window : After Update");
                    _appealCategoryManager.ClickAppealCategoryAuditHistoryIcon();
                    _appealCategoryManager.GetAuditRowCount().ShouldBeEqual(2,
                        "A row should be added showing the edit details in the audit history pane");

                    /* Extracting userid from comma separated values in Audit History */
                    var analystListInAuditHistoryAfterUpdate =
                        _appealCategoryManager.GetAnalaystInAuditSection().Split(',').ToList().Select(
                            x => x.Trim()).ToList();

                    /* Extracting userid from the testdata */
                    expectedAnalystUserIds = (analystsForMultipleAssignment.Select(s =>
                        s.Substring(s.LastIndexOf('(') + 1, s.LastIndexOf(')') - 1 - s.LastIndexOf('('))).ToList());
                    analystListInAuditHistoryAfterUpdate.ShouldCollectionBeEqual(expectedAnalystUserIds,
                        "Assigned analysts showing correctly in " +
                        "Appeal Category Audit History after the update");

                    StringFormatter.PrintMessage(
                        "Verifying the updated values are showing correctly in the edit form as well");
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent("DCA", "NA",
                        "NA");
                    assignedAnalysts = _appealCategoryManager.GetSelectedAnalystListForDCI()
                        .Select(s => s.Split('-')[0].Trim()).ToList();
                    assignedAnalysts.ShouldCollectionBeEqual(analystsForMultipleAssignment,
                        "The changed list of analyst reflects correctly in the edit form");
                    _appealCategoryManager.GetNote().ShouldBeEqual("Test");
                    _appealCategoryManager.ClickOnCancelButton();

                    StringFormatter.PrintMessageTitle("Verifying deleting the appeal category for DCA ");

                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode("DCA", "NA", "NA");
                    _appealCategoryManager
                        .IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent("DCA", "NA", "NA")
                        .ShouldBeFalse
                            ("Deleted appeal category should no longer be present in the grid");

                    #endregion
                }
                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();

                    //Delete the newly created DCA categories
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode("DCA", "NA", "NA");

                }

                void VerifyInputFieldsGettingDisplayedForDCIProductInAddCategory()
                {
                    var inputFieldsListForAddCategory = new List<string>
                    {
                        "Category ID",
                        "Proc Code From",
                        "Proc Code To",
                        "Category Order",
                        "Trig Proc From",
                        "Trig Proc To",
                        "Analyst (non-restricted claims)",
                        "Analyst (restricted claims)",
                        "Client",
                        "Analyst",
                        "Product"
                    };

                    StringFormatter.PrintMessage("Verifying that only the 'Product' field must be present when Add Category is clicked");

                    _appealCategoryManager.ClickAddNewCategory(clickOnly: true);

                    // Verifies Product needs to be selected prior to proceeding to the Add Category Form
                    foreach (var inputField in inputFieldsListForAddCategory)
                    {
                        if (inputField == "Product")
                            _appealCategoryManager.IsInputFieldPresentInAddCategory(inputField).ShouldBeTrue(string.Format("'{0}' field must be present in Add Category before selecting a product", inputField));
                        else
                            _appealCategoryManager.IsInputFieldPresentInAddCategory(inputField).ShouldBeFalse(string.Format("'{0}' field must not be present in Add Category before selecting a product", inputField));
                    }

                    StringFormatter.PrintMessage("Verifying that respective input fields must be shown up when DCA Product is selected in Add Category Form");

                    _appealCategoryManager.SelectProductOnAddSection(ProductEnum.DCA.GetStringValue());

                    //Loops through the list of input fields and checks whether they are not getting displayed for DCA
                    int i;
                    for (i = 0; i < 8; i++)
                    {
                        _appealCategoryManager.IsInputFieldPresentInAddCategory(inputFieldsListForAddCategory[i])
                            .ShouldBeFalse(string.Format("{0} field should not be present after DCA product is selected", inputFieldsListForAddCategory[i]));
                    }

                    //Loops through the list of input fields and checks whether the expected labels and input fields are getting displayed for DCA
                    for (; i < inputFieldsListForAddCategory.Count; i++)
                    {
                        if (inputFieldsListForAddCategory[i] == "Client")
                        {
                            _appealCategoryManager.IsValidInputPresentOnTransferComponentByLabel("Clients").ShouldBeTrue($"Clients should be present");
                            continue;
                        }

                        _appealCategoryManager.IsInputFieldPresentInAddCategory(inputFieldsListForAddCategory[i])
                            .ShouldBeTrue(string.Format("{0} field should be present after DCA product is selected", inputFieldsListForAddCategory[i]));
                    }
                }
            }
        }

        [Test]//US36215
        [Retrying(Times = 3)]
        public void Verify_proper_validation_message_shown_on_edit_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                string[] proCode = _appealCategoryManager.GetProcCodeValueAppealRowByRow(excludeDci: true);
                var trigProc = _appealCategoryManager.GetTrigProcValueAppealRowByRow(excludeDci: true);
                var client = _appealCategoryManager.GetClientValue(excludeDci: true);

                try
                {
                    _appealCategoryManager.ClickOnEditIconByCategoryOrder(_appealCategoryManager
                        .GetAppealCategoryOrderOfLastRowWithOutFlag());
                    _appealCategoryManager.IsEditFormPresent()
                        .ShouldBeTrue("Edit Category Code Section is displayed below the selected record");

                    _appealCategoryManager.SetProcCodeFromOnEditSection("!@#$%");
                    VerifyEditValidationMessage(
                        "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                        "Proper message is shown when Proc Code From Invalid ");
                    _appealCategoryManager.SetProcCodeFromOnEditSection("");
                    VerifyEditValidationMessage(
                        "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                        "Proper message is shown when Proc Code From Empty ");
                    _appealCategoryManager.SetProcCodeFromOnEditSection(proCode[0]);
                    _appealCategoryManager.SetProcCodeToOnEditSection("!@#$%");
                    VerifyEditValidationMessage(
                        "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                        "Proper message is shown when Proc Code To Invalid ");
                    _appealCategoryManager.SetProcCodeToOnEditSection("");
                    VerifyEditValidationMessage(
                        "Valid procedure codes must be entered in the Proc Code From, Proc Code To fields before the record can be saved.",
                        "Proper message is shown when Proc Code To Empty ");
                    _appealCategoryManager.SetProcCodeToOnEditSection(proCode[1]);
                    _appealCategoryManager.SelectCategoryOrderOnEditSection("");
                    const string catErr = "A Category Order must be selected before the record can be saved.";
                    VerifyEditValidationMessage(catErr,
                        "Proper message is shown when No Category Order is Selected ");
                    _appealCategoryManager.SelectCategoryOrderOnEditSection("1");
                    _appealCategoryManager.SelectAnalystOnEditSection("", true);
                    const string anaystEr = "An Analyst must be selected before the record can be saved.";
                    VerifyEditValidationMessage(anaystEr,
                        "Proper message is shown when No analyst is Selected ");
                    _appealCategoryManager.SelectAnalystOnEditSection("Test Automation (uiautomation)", true);
                    //_appealCategoryManager.SelectProductOnEditSection("");
                    //VerifyEditValidationMessage("A Product must be selected before the record can be saved.",
                    //  "Proper message is shown whenNo Category Order Selected ");
                    //_appealCategoryManager.SelectProductOnEditSection(
                    //  GetProductNameByProductAbbreviation(_appealCategoryManager.GetProductValueAppealRowByRow(2)));

                    if (trigProc.Length > 0)
                    {
                        string[] trigProcList = trigProc.Split('-');
                        _appealCategoryManager.SetTrigProcFromOnEditSection(trigProcList[0]);
                        _appealCategoryManager.SetTrigProcToOnEditSection(trigProcList[1]);
                    }
                    else
                    {
                        _appealCategoryManager.SetTrigProcFromOnEditSection("");
                        _appealCategoryManager.SetTrigProcToOnEditSection("");
                    }

                    //_appealCategoryManager.SelectClientOnEditSection(client);
                    //VerifyEditValidationMessage("The values entered match an existing category code record.",
                    //    "Proper message is shown when if combination Value of Product,Proc Code and Trig Code is same");
                    _appealCategoryManager.ClickOnCancelButton();
                    _appealCategoryManager.IsEditFormPresent()
                        .ShouldBeFalse("Edit Category Code Section is displayed below the selected record");
                }

                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();
                    if (_appealCategoryManager.IsEditFormPresent())
                        _appealCategoryManager.ClickOnCancelButton();

                }

                void VerifyEditValidationMessage(string message, string title)
                {
                    _appealCategoryManager.WaitForStaticTime(1000);
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    _appealCategoryManager.GetPageErrorMessage().ToLower().ShouldBeEqual(
                        message.ToLower(), title);
                    _appealCategoryManager.ClosePageError();
                }
            }
        }

        [Test] //US65706 +US65699
        //[Order(4)]
        [Retrying(Times = 3)]
        public void Verify_search_result_retains_and_edit_section_is_updated_after_editing_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var categoryId1 = testData["CategoryId1"];
                var procCodeFrom1 = testData["ProcCodeFrom1"];
                var procCodeTo1 = testData["ProcCodeTo1"];
                var categoryOrder1 = testData["CategoryOrder1"];
                var trigProcFrom1 = testData["TrigProcFrom1"];
                var trigProcTo1 = testData["TrigProcTo1"];
                var product1 = testData["Product1"];
                var analyst1 = testData["Analyst1"];
                var note1 = testData["Note1"];
                var productAbbreviation = testData["ProductAbbreviation1"];
                var procCodeFrom2 = testData["ProcCodeFrom2"];
                var procCodeTo2 = testData["ProcCodeTo2"];
                var trigProcFrom2 = testData["TrigProcFrom2"];
                var trigProcTo2 = testData["TrigProcTo2"];
                var product2 = testData["Product2"];
                var analyst2 = testData["Analyst2"];
                var note2 = testData["Note2"];
                //var productAbbreviation2 = testData["ProductAbbreviation2"];
                string[] newAppealData =
                {
                    categoryId1, procCodeFrom1, procCodeTo1, categoryOrder1, product1, trigProcFrom1, trigProcTo1,
                    analyst1, note1, productAbbreviation
                };
                string[] updateAppealData =
                {
                    categoryId1, procCodeFrom2, procCodeTo2, categoryOrder1, product2, trigProcFrom2, trigProcTo2,
                    analyst2, note2, productAbbreviation
                };
                try
                {
                    StringFormatter.PrintMessage("Create New Appeal Category Manager");
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom2 + "-" + procCodeTo2, trigProcFrom2 + "-" + trigProcTo2);
                    _appealCategoryManager.CreateAppealCategory(newAppealData);
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.IsFindCategoryCodeSectionPresent()
                        .ShouldBeTrue("Find Category Codes Section Present");
                    _appealCategoryManager.SelectCategoryOrderOnFindSection(categoryOrder1);

                    StringFormatter.PrintMessage("Search for Appeal Category Manager by Category Order");
                    _appealCategoryManager.ClickOnFindButton();
                    _appealCategoryManager.GetAppealCategoryRowCount()
                        .ShouldBeEqual(1, "Search Result must return 1 row");
                    var searchResultCountBeforeEdit = _appealCategoryManager.GetAppealCategoryRowCount();
                    _appealCategoryManager.UpdateAppealCategory(updateAppealData);

                    StringFormatter.PrintMessage("Search result should be equal before and after edit");
                    _appealCategoryManager.GetAppealCategoryRowCount().ShouldBeEqual(searchResultCountBeforeEdit,
                        "Search result has been retained after edit");
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.GetCategoryOrderOnFindSection().ShouldBeEqual(categoryOrder1,
                        "Search criteria has been retained after edit");
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(
                        productAbbreviation,
                        procCodeFrom2 + "-" + procCodeTo2, trigProcFrom2 + "-" + trigProcTo2);
                    //_appealCategoryManager.GetProductOnEditSection()
                    //  .ShouldBeEqual(product2, "Product must be updated after edit");
                    _appealCategoryManager.GetAnalystOnEditSection().Split('-')[0].Trim()
                        .ShouldBeEqual(analyst2, "Analyst must be updated aftere edit");
                    _appealCategoryManager.GetProcCodeFromOnEditSection()
                        .ShouldBeEqual(procCodeFrom2, "Proc Code From must be updated after edit");
                    _appealCategoryManager.GetProcCodeToOnEditSection()
                        .ShouldBeEqual(procCodeTo2, "Proc Code To must be updated after edit");
                    _appealCategoryManager.GetTrigCodeFromOnEditSection()
                        .ShouldBeEqual(trigProcFrom2, "Trig Code from must be updated after edit");
                    _appealCategoryManager.GetTrigCodeToOnEditSection()
                        .ShouldBeEqual(trigProcTo2, "Trig Code From must be updated after edit");
                    _appealCategoryManager.GetCategoryOrderOnEditSection()
                        .ShouldBeEqual("1", "Category Order must be same");
                    _appealCategoryManager.ClickOnCancelButton();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom2 + "-" + procCodeTo2, trigProcFrom2 + "-" + trigProcTo2);
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            productAbbreviation,
                            procCodeFrom2 + "-" + procCodeTo2, trigProcFrom2 + "-" + trigProcTo2)
                        .ShouldBeFalse("Ok button Clicked, Appeal row present.");
                }
                finally
                {
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom2 + "-" + procCodeTo2, trigProcFrom2 + "-" + trigProcTo2);
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation,
                        procCodeFrom1 + "-" + procCodeTo1, trigProcFrom1 + "-" + trigProcTo1);
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.ClickOnSearchIcon();

                }
            }
        }
    

        [Test]
        //[Order(3)]
        [Retrying(Times = 3)]
        public void Verify_order_of_values_in_row()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                // Checking in reference to the second row.
                var productAbbreviations = new List<string>(new[] {"CV", "DCA", "FCI", "FFP", "RxI", "NEG", "OCI"});

                // Check for client: (2nd in row and label present)
                var client = _appealCategoryManager.GetTableRawTextByRowColumn(2, 3);

                client.Split(':')[0].ShouldBeEqual("Client", "Client Label Should present");
                client.Split(':')[1].ShouldNotBeNull("Client Value should present");


                // Check for Product: (3rd in row and label not present)
                var product = _appealCategoryManager.GetTableRawTextByRowColumn(2, 4);
                product.Contains("Product:").ShouldBeFalse("Product Label Should present");
                product.AssertIsContainedInList(productAbbreviations, "Client Value should present");


                // Check for Order: (4th in row and label present)
                var order = (_appealCategoryManager.GetTableRawTextByRowColumn(2, 6));
                order.Split(':')[0].ShouldBeEqual("Order", "Order Label Should present");
                order.Split(':')[1].ShouldNotBeNull("Client Value should present");

                // Check for Proc Code: (5th in row and label present)
                var procFromTo = (_appealCategoryManager.GetTableRawTextByRowColumn(2, 7));
                procFromTo.Split(':')[0].ShouldBeEqual("Proc", "Proc Label Should present");
                procFromTo.Split(':')[1].ShouldNotBeNull("Client Value should present");


                // Check for Tric Code: (6th in row and label present)
                var trigTromTo = (_appealCategoryManager.GetTableRawTextByRowColumn(2, 8));
                trigTromTo.Split(':')[0].ShouldBeEqual("Trig", "Trig Label Should present");

                // Check for CategoryId Value: (1st in row and label not present)
                var categoryid = _appealCategoryManager.GetCategoryValueByRow(2);
                categoryid.Contains(":").ShouldBeFalse("Category Id Label Should not present");
                categoryid.Length.ShouldBeGreater(0, "Client Value should present");
            }
        }

        [Test]//US67143 + CAR-713
        [Retrying(Times = 3)]
        public void Verify_Analyst_input_field_and_selected_multiple_analyst_behviour()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var analyst = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value").Split(';')
                    .ToList();
                var analystForRestrictedClaims = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value").Split(';')
                    .ToList();

                try
                {
                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.GetAnalystListOnAddOption()
                        .ShouldCollectionBeEquivalent(_appealCategoryManager.GetExpectedAnalystList(),
                            "Analyst drop down list should be equal to database value having <Appeal Can Be Assigned> authority in  " +
                            "<FirstName LastName (usrname)> - (Restriction Access as comma separated values) format");
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst[0]);
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst[0]);
                    _appealCategoryManager.SelectAnalystOnAddSection(analyst[1]);
                    _appealCategoryManager.SelectAnalystOnAddSectionByEnterKey(analyst[2]);
                    _appealCategoryManager.GetAnalystInputDropDownValue()
                        .ShouldBeEqual("", "User should select a single value");
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldCollectionBeEqual(analyst,
                            "Selected Analyst should be same whether selected by click or by enter key and should not be repeated");
                    _appealCategoryManager.GetDeleteAnalystIconCount()
                        .ShouldBeEqual(analyst.Count, "Delete Icon Should be present in each selected analyst");
                    _appealCategoryManager.ClickDeleteAnalyst(2);
                    analyst.RemoveAt(1);
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldCollectionBeEqual(analyst, "Is Deleted Analyst Present in Selected Analyst");
                    _appealCategoryManager.ClickDeleteAnalyst();
                    _appealCategoryManager.GetDeleteAnalystIconCount()
                        .ShouldBeEqual(0, "Is Selected Analyst Present when each selected analyst is deleted?");

                    //Test Start for Analyst(restricted claims)

                    _appealCategoryManager.GetAnalystListOnAddOption(false).ShouldCollectionBeEquivalent(
                        _appealCategoryManager.GetExpectedAnalystList(false),
                        "Analyst(restricted claims) drop down list should be equal to database value having <Appeal Can Be Assigned> authority in  " +
                        "<FirstName LastName (usrname)> - (Restriction Access as comma separated values) format");
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analystForRestrictedClaims[0]);
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analystForRestrictedClaims[0]);
                    _appealCategoryManager.SelectAnalystRestrictedClaimOnAddSection(analystForRestrictedClaims[1]);
                    _appealCategoryManager.SelectAnalystRestrictedClaimsOnAddSectionByEnterKey(
                        analystForRestrictedClaims[2]);
                    _appealCategoryManager.GetAnalystInputDropDownValue(false)
                        .ShouldBeEqual("", "User should select a single value");
                    _appealCategoryManager.GetSelectedAnalystListFromRestrictedAnalystSection()
                        .ShouldCollectionBeEqual(analystForRestrictedClaims,
                            "Selected Analyst(restricted claims) should be same whether selected by click or by enter key and should not be repeated");
                    _appealCategoryManager.GetDeleteAnalystIconInRestrictedAccessAnalystSectionCount()
                        .ShouldBeEqual(analystForRestrictedClaims.Count,
                            "Delete Icon Should be present in each selected analyst");
                    _appealCategoryManager.ClickDeleteAnalystFromRestrictionAccessAnalystSection(2);
                    analystForRestrictedClaims.RemoveAt(1);
                    _appealCategoryManager.GetSelectedAnalystListFromRestrictedAnalystSection()
                        .ShouldCollectionBeEqual(analystForRestrictedClaims,
                            "Is Deleted Analyst Present in Selected Analyst");
                    _appealCategoryManager.ClickDeleteAnalystFromRestrictionAccessAnalystSection();
                    _appealCategoryManager.GetDeleteAnalystIconInRestrictedAccessAnalystSectionCount()
                        .ShouldBeEqual(0,
                            "Is Selected Analyst(restricted claims) Present when each selected analyst is deleted?");
                }

                finally
                {
                    if (_appealCategoryManager.IsAddNewCategoryCodeSectionDisplayed())
                        _appealCategoryManager.ClickOnCancelButton();
                }
            }
        }

       
        [Test] //US67147
        [Retrying(Times = 3)]
        public void Verify_search_appeal_categories_filter_by_analyst()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var analyst = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst",
                    "Value");
                try
                {
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.SelectAnalystOnFindSection(analyst);
                    _appealCategoryManager.ClickOnFindButton();
                    var count = _appealCategoryManager.GetAppealCategoryRowCount();
                    var loopCount = count >= 5 ? 5 : count;
                    for (var i = 1; i < loopCount; i++) //Check first 5 category rows only
                    {
                        _appealCategoryManager.ClickOnAppealCategoryRow(i);
                        var selectedCategoryCode = _appealCategoryManager.GetCategoryCode(i);
                        var analystAssignmentList = _appealCategoryManager.GetAnalystListOnAnalystAssignment();
                        analyst.AssertIsContainedInList(analystAssignmentList,
                            analyst + " should be assigned to the selected category " + selectedCategoryCode);
                    }

                }
                finally
                {
                    if (!_appealCategoryManager.IsFindCategoryCodeSectionPresent())
                        _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.ClickOnSearchIcon();
                }
            }
        }


        [Test] //CAR-2974
        [Retrying( Times = 3)]
        public void Verify_assignment_of_appeal_category_based_on_pto()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                AppealCreatorPage _appealCreator;
                AppealManagerPage _appealManager;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Claseq",
                    "Value").Split(';');

                var procRange = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ProcRange",
                    "Value").Split(';');
                var analystList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AnalystList",
                    "Value").Split(';');
                var categoryDetails = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "CategoryDetails",
                    "Value").Split(';');

                StringFormatter.PrintMessageTitle("Creating an Appeal Category with multiple clients");
                _appealCategoryManager.CreateAppealCategoryForMultipleAnalyst(categoryDetails[0], categoryDetails[1],
                    procRange[0], procRange[1], analystList, categoryDetails[2]);

                var _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Product.GetStringValue());
                //_clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.CV.ToString());

                var offsetValue = _clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.CV.ToString());
                var duedateA = automatedBase.CurrentPage.CalculateAndGetAppealDueDateFromDatabase(offsetValue[2]);
                var duedateR = automatedBase.CurrentPage.CalculateAndGetAppealDueDateFromDatabase(offsetValue[0]);

                var analystManager = _appealCategoryManager.NavigateToAnalystManager();
                analystManager.SearchByAnalyst(analystList[0].Split('(')[0].Trim());
                analystManager.GetGridViewSection.ClickOnGridRowByRow();
                analystManager.ClickOnPTOIcon();
                analystManager.GetSideWindow.ClickOnEditIcon();
                analystManager.DeleteAllPTO();
                analystManager.ClickOnAddNewEntryButton();
                analystManager.SetPTODateWithoutUsingCalendar(duedateA);

                if (duedateA != duedateR)
                {
                    analystManager.ClickOnAddNewEntryButton();
                    analystManager.SetPTODateWithoutUsingCalendar(duedateR, 2);
                }

                analystManager.ClickOnSaveButton();
                _appealCreator = _appealCategoryManager.NavigateToAppealCreator();

                _appealCreator.SearchByClaimSequence(claseq[0]);
                _appealCreator.WaitForWorking();
                _appealCreator.SelectClaimLine();
                StringFormatter.PrintMessage("Creating an Appeal");
                _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "123");

                _appealCreator.SearchByClaimSequence(claseq[1]);
                _appealCreator.WaitForWorking();
                _appealCreator.SelectClaimLine();
                StringFormatter.PrintMessage("Creating an Appeal");
                _appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "34xyz", appealType: "R");

                automatedBase.CurrentPage = _appealManager = _appealCreator.NavigateToAppealManager();
                _appealManager.SearchByClaimSequence(claseq[0]);
                _appealManager.GetGridViewSection.GetValueInGridByColRow(8)
                    .ShouldBeEqual(analystList[1].Split('(')[0].Trim(), "Is Assigned To Equals?");

                _appealManager.SearchByClaimSequence(claseq[1]);
                _appealManager.GetGridViewSection.GetValueInGridByColRow(8)
                    .ShouldBeEqual(analystList[1].Split('(')[0].Trim(), "Is Assigned To Equals?");

                void CreateAppeal(string exDocId, string product = "Coding Validation")
                {
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.SelectRecordReviewRecordType();
                    _appealCreator.SelectProduct(product, true);
                    _appealCreator.SetDocumentId(exDocId);
                    _appealCreator.ClickOnSaveBtn();
                    _appealCreator.WaitForWorkingAjaxMessage();
                }

                void CreateAppealCategoryForMultipleAnalyst(string categoryId, string product, string procFrom, string procTo, string[] analysts, string productAbbreviation, List<string> clientList, string catOrder = "1", string trigFrom = null, string trigTo = null)
                {
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation, procFrom + "-" + procTo, trigFrom + "-" + trigTo);

                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.SetProcCodeFromOnAddSection(procFrom);
                    _appealCategoryManager.SetProcCodeToOnAddSection(procTo);
                    _appealCategoryManager.SelectCategoryOrderOnAddSection(catOrder);

                    foreach (var client in clientList)
                    {
                        _appealCategoryManager.SelectClientOnAddSection(client);
                    }

                    _appealCategoryManager.SelectProductOnAddSection(product);
                    if (!String.IsNullOrEmpty(trigFrom)) _appealCategoryManager.SetTrigProcFromOnAddSection(trigFrom);
                    if (!String.IsNullOrEmpty(trigTo)) _appealCategoryManager.SetTrigProcToOnAddSection(trigTo);
                    foreach (var analyst in analysts)
                    {
                        _appealCategoryManager.SelectAnalystOnAddSection(analyst);
                    }

                    //US67970
                    _appealCategoryManager.ClickMoveDownAnalystByAnalystName(analysts[0]);
                    var order = new List<int> { 1, 0, 2 };
                    var result = order.Select(i => analysts[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[0] + " should be moved down to second.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analysts[0]);
                    order = new List<int> { 1, 0, 2 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[0] + " should be moved up to original places.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analysts[2]);
                    order = new List<int> { 0, 2, 1 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[2] + " should be moved up to second.");
                    _appealCategoryManager.ClickMoveDownAnalystByAnalystName(analysts[2]);
                    order = new List<int> { 0, 2, 1 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[2] + " should be moved down to third.");

                    // US67970 ends here

                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    Console.WriteLine("New Appeal Cateogry Created");

                }
            }
        }


        [Test, Category("SchemaDependent")] //US67963 + US67970 + CAR-2782[CAR-2862]
        [Retrying(Times = 3)]
        public void Verify_round_robin_appeal_assignment_of_appeals_for_same_cateogry_and_duedate()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                AppealCreatorPage _appealCreator;
                AppealManagerPage _appealManager;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Claseq",
                    "Value").Split(';');
                var rpeClaseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "RPEClaseq", "Value");
                var procRange = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ProcRange",
                    "Value").Split(';');
                var analystList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AnalystList",
                    "Value").Split(';');
                var categoryDetails = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "CategoryDetails",
                    "Value").Split(';');
                var clientList = new List<string> {ClientEnum.RPE.ToString(), ClientEnum.SMTST.ToString()};
                try
                {
                    DeleteAppealCateogryAndAssociatedAppeals(client: $"{clientList[0]},{clientList[1]}");
                    //_appealCategoryManager.CreateAppealCategoryForMultipleAnalyst(categoryDetails[0], categoryDetails[1],
                    //procRange[0], procRange[1], analystList, categoryDetails[2]);
                    StringFormatter.PrintMessageTitle("Creating an Appeal Category with multiple clients");
                    CreateAppealCategoryForMultipleAnalyst(categoryDetails[0], categoryDetails[1],
                        procRange[0], procRange[1], analystList, categoryDetails[2]);
                    _appealCreator = _appealCategoryManager.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claseq[0]);
                    _appealCreator.WaitForWorking();

                    StringFormatter.PrintMessage("Creating an Appeal");
                    CreateAppeal("12xyz");
                    _appealCreator.SearchByClaimSequence(claseq[1]);
                    _appealCreator.WaitForWorking();

                    StringFormatter.PrintMessage("Creating an Appeal");
                    CreateAppeal("34xyz");
                    automatedBase.CurrentPage = _appealManager = _appealCreator.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claseq[1]);
                    _appealManager.ChangeDueDateOfAppealByRow(dueDate: DateTime.Today.AddDays(3).ToString("MM/d/yyyy"));
                    automatedBase.CurrentPage = _appealCreator = _appealManager.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claseq[2]);
                    _appealCreator.WaitForWorking();

                    StringFormatter.PrintMessage("Creating an Appeal");
                    CreateAppeal("56xyz");

                    StringFormatter.PrintMessageTitle("Verification of appeal assigned to correct analysts");
                    var appealSearch = _appealCreator.NavigateToAppealSearch();
                    appealSearch.SelectAllAppeals();
                    appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Category",
                        categoryDetails[0]);
                    appealSearch.ClickOnFindButtonAndWait();
                    appealSearch.GetAppealSearchResultRowCount()
                        .ShouldBeEqual(claseq.Length - 1,
                            "Appeal's count should be as expected and equal to " + (claseq.Length - 1));
                    var appealList = appealSearch.GetSearchResultListByCol(3).Select(int.Parse).OrderBy(n => n);
                    var actualAssignedList = appealList
                        .Select(appeal => appealSearch.GetAssignedToByAppealSequence(appeal.ToString())).ToList();
                    var analystLists = analystList.ToList();
                    var analystListWithoutUsername = analystList.Select(s => s.Split('(')[0].Trim()).ToList();
                    actualAssignedList.ShouldCollectionBeEqual(analystListWithoutUsername,
                        "Appeals should be assigned in  round robin in the order the analyst were assigned in appeal category.");
                    appealSearch.NavigateToAppealCategoryManager();

                    StringFormatter.PrintMessage("Changing the order of analysts for the Appeal Category");
                    // US67970 starts here
                    _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(
                        categoryDetails[2],
                        procRange[0] + "-" + procRange[1], null, client: $"{clientList[0]},{clientList[1]}");
                    _appealCategoryManager.GetHeaderEditAppealCategory().ShouldBeEqual("Edit Appeal Category",
                        "Edit Appeal Category title Should be equal.");
                    _appealCategoryManager.GetSelectedAnalystList().ShouldBeEqual(analystLists,
                        "Expected analyst list should be present in edit section.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analystLists[1], false);
                    var order = new List<int> {1, 0, 2};
                    var assignedAnalystsToAppealCategory = order.Select(i => analystLists[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(assignedAnalystsToAppealCategory,
                            "Analyst should be reordered, " + analystLists[1] + " should be moved up to first, " +
                            analystLists[0] + " should be moved to second.");
                    _appealCategoryManager.ClickMoveDownAnalystByAnalystName(analystLists[0], false);
                    order = new List<int> {0, 2, 1};
                    assignedAnalystsToAppealCategory = order.Select(i => assignedAnalystsToAppealCategory[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(assignedAnalystsToAppealCategory,
                            "Analyst should be reordered, " + analystLists[0] + " should be moved down to third " +
                            analystLists[2] + " should be moved to second.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analystLists[2], false);
                    order = new List<int> {1, 0, 2};
                    assignedAnalystsToAppealCategory = order.Select(i => assignedAnalystsToAppealCategory[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(assignedAnalystsToAppealCategory,
                            "Analyst should be reordered, " + analystLists[2] + " should be moved up to first  " +
                            analystLists[1] + " should be moved to second.");
                    // US67970 ends here
                    _appealCategoryManager.ClickOnSaveButtonInEdit();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();

                    StringFormatter.PrintMessage("Creating an appeal");
                    _appealCategoryManager.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claseq[3]);
                    CreateAppeal("56xyz");

                    StringFormatter.PrintMessage(Format(
                        $"Validating newly created appeal should be assigned to the analyst: " +
                        $"{analystLists[1]} next in order after lastly assigned analyst: {analystLists[2]}."));
                    analystListWithoutUsername.Add(analystListWithoutUsername[1]);
                    _appealCreator.NavigateToAppealSearch();
                    appealSearch.SelectAllAppeals();
                    appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Category",
                        categoryDetails[0]);
                    appealSearch.ClickOnFindButtonAndWait();
                    appealSearch.GetAppealSearchResultRowCount()
                        .ShouldBeEqual(claseq.Length,
                            "Appeal's count should be as expected and equal to " + claseq.Length);
                    appealList = appealSearch.GetSearchResultListByCol(3).Select(int.Parse).OrderBy(n => n);
                    actualAssignedList = appealList
                        .Select(appeal => appealSearch.GetAssignedToByAppealSequence(appeal.ToString())).ToList();

                    actualAssignedList.ShouldCollectionBeEqual(analystListWithoutUsername,
                        "Appeals should be assigned in  round robin in the order the analyst were assigned in appeal category.");

                    appealSearch.ClickOnFilterOptionListRow(3);

                    #region CAR-2782

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether analyst assignment is rotated through the list regardless of clients assigned");
                    var lastAssignedAnalyst = appealSearch.GetGridViewSection.GetGridListValueByCol(8).Last();

                    StringFormatter.PrintMessage("Creating an appeal for RPE client");
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE).NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(rpeClaseq);
                    CreateAppeal("56xyz");

                    StringFormatter.PrintMessage(
                        $"Searching for the {ClientEnum.RPE.ToString()} appeal to verify the analyst");
                    appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    appealSearch.SelectAllAppeals();
                    appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Category",
                        categoryDetails[0]);
                    appealSearch.ClickOnFindButtonAndWait();

                    appealSearch.GetAppealSearchResultRowCount()
                        .ShouldBeEqual(claseq.Length + 1,
                            "Appeal's count should be as expected and equal to " + claseq.Length + 1);

                    appealSearch.ClickOnFilterOptionListRow(3);

                    var nextAnalystToBeAssignedWithAppeal = assignedAnalystsToAppealCategory[
                        assignedAnalystsToAppealCategory.Select(s => s.Split('(')[0].Trim()).ToList()
                            .IndexOf(lastAssignedAnalyst) + 1];

                    lastAssignedAnalyst = appealSearch.GetGridViewSection.GetGridListValueByCol(8).Last();
                    appealSearch.GetGridViewSection.GetGridListValueByCol(5).Last().ShouldBeEqual(
                        ClientEnum.RPE.ToString(),
                        $"Appeal created for {ClientEnum.RPE.ToString()} is listed in the Appeal Search");

                    lastAssignedAnalyst.ShouldBeEqual(nextAnalystToBeAssignedWithAppeal.Split('(')[0].Trim(),
                        "Analyst assignment should be rotated regardless of the clients assigned");

                    #endregion

                    StringFormatter.PrintMessage("Deleting the appeals linked to the newly created appeal category");

                    _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SelectAllAppeals();
                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Category", categoryDetails[0]);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorkingAjaxMessage();

                    if (!_appealManager.IsNoMatchingRecordFoundMessagePresent())
                    {
                        while (_appealManager.GetAppealSearchResultRowCount() != 0)
                        {
                            _appealManager.ClickOnDeleteIconByRowSelector(1);
                            _appealManager.ClickOkCancelOnConfirmationModal(true);
                        }
                    }

                    StringFormatter.PrintMessage("Deleting the newly created appeal category");
                    _appealCategoryManager = _appealManager.NavigateToAppealCategoryManager();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(categoryDetails[2],
                        procRange[0] + "-" + procRange[1], null, $"{clientList[0]},{clientList[1]}");
                }

                finally
                {
                    if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(ClientEnum.SMTST))
                    {
                        StringFormatter.PrintMessageTitle(
                            $"Finally Block : Switching back to {ClientEnum.SMTST.ToString()} client");
                        automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST)
                            .NavigateToAppealCategoryManager();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Finally Block : Deleting newly created Appeal Category if it is still present");
                    DeleteAppealCateogryAndAssociatedAppeals(
                        client: $"{clientList[0]},{clientList[1]}");
                    automatedBase.CurrentPage = _appealCategoryManager.ClickOnQuickLaunch().NavigateToAppealCategoryManager();
                }

                void CreateAppeal(string exDocId, string product = "Coding Validation")
                {
                    _appealCreator.ClickOnClaimLine(1);
                    _appealCreator.SelectRecordReviewRecordType();
                    _appealCreator.SelectProduct(product, true);
                    _appealCreator.SetDocumentId(exDocId);
                    _appealCreator.ClickOnSaveBtn();
                    _appealCreator.WaitForWorkingAjaxMessage();
                }

                void DeleteAppealCateogryAndAssociatedAppeals(string client = "SMTST")
                {

                    StringFormatter.PrintMessage(
                        "Delete appeals associated if there are any, and delete appeal category.");
                    if (
                        _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            categoryDetails[2], procRange[0] + "-" + procRange[1], null, client: client))
                    {
                        _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(categoryDetails[2], procRange[0] + "-" + procRange[1], null, client);
                        if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                            if (
                                _appealCategoryManager.GetPageErrorMessage()
                                    .Equals(
                                        "Please complete all appeals with the RRASSIGN category code prior to deleting it."))
                            {
                                _appealCategoryManager.ClosePageError();
                                _appealManager = _appealCategoryManager.NavigateToAppealManager();
                                _appealManager.SelectAllAppeals();
                                _appealManager.SelectSMTST();
                                _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                                _appealManager.SelectDropDownListbyInputLabel("Status", "New");
                                _appealManager.SelectSearchDropDownListForMultipleSelectValue("Category", categoryDetails[0]);
                                _appealManager.ClickOnFindButton();
                                _appealManager.WaitForWorkingAjaxMessage();
                                if (!_appealManager.IsNoMatchingRecordFoundMessagePresent())
                                {
                                    while (_appealManager.GetAppealSearchResultRowCount() != 0)
                                    {
                                        _appealManager.ClickOnDeleteIconByRowSelector(1);
                                        _appealManager.ClickOkCancelOnConfirmationModal(true);
                                    }
                                }
                                _appealManager.NavigateToAppealCategoryManager();
                                _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(categoryDetails[2], procRange[0] + "-" + procRange[1], null, client);
                            }
                        _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);
                    }
                }

                void CreateAppealCategoryForMultipleAnalyst(string categoryId, string product, string procFrom, string procTo, string[] analysts, string productAbbreviation, string catOrder = "1", string trigFrom = null, string trigTo = null)
                {
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation, procFrom + "-" + procTo, trigFrom + "-" + trigTo);

                    _appealCategoryManager.ClickAddNewCategory();
                    _appealCategoryManager.SetCategoryId(categoryId);
                    _appealCategoryManager.SetProcCodeFromOnAddSection(procFrom);
                    _appealCategoryManager.SetProcCodeToOnAddSection(procTo);
                    _appealCategoryManager.SelectCategoryOrderOnAddSection(catOrder);

                    foreach (var client in clientList)
                    {
                        _appealCategoryManager.SelectClientOnAddSection(client);
                    }

                    _appealCategoryManager.SelectProductOnAddSection(product);
                    if (!String.IsNullOrEmpty(trigFrom)) _appealCategoryManager.SetTrigProcFromOnAddSection(trigFrom);
                    if (!String.IsNullOrEmpty(trigTo)) _appealCategoryManager.SetTrigProcToOnAddSection(trigTo);
                    foreach (var analyst in analysts)
                    {
                        _appealCategoryManager.SelectAnalystOnAddSection(analyst);
                    }

                    //US67970
                    _appealCategoryManager.ClickMoveDownAnalystByAnalystName(analysts[0]);
                    var order = new List<int> { 1, 0, 2 };
                    var result = order.Select(i => analysts[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[0] + " should be moved down to second.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analysts[0]);
                    order = new List<int> { 1, 0, 2 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[0] + " should be moved up to original places.");
                    _appealCategoryManager.ClickMoveUpAnalystByAnalystName(analysts[2]);
                    order = new List<int> { 0, 2, 1 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[2] + " should be moved up to second.");
                    _appealCategoryManager.ClickMoveDownAnalystByAnalystName(analysts[2]);
                    order = new List<int> { 0, 2, 1 };
                    result = order.Select(i => result[i]).ToList();
                    _appealCategoryManager.GetSelectedAnalystList()
                        .ShouldBeEqual(result, "Analyst should be reordered, " + analysts[2] + " should be moved down to third.");

                    // US67970 ends here

                    _appealCategoryManager.ClickOnSaveButtonInCreateSection();
                    _appealCategoryManager.WaitForWorkingAjaxMessage();
                    Console.WriteLine("New Appeal Cateogry Created");

                }
            }
        }



        [Test]//US68889
        [Retrying( Times = 3)]
        public void Verify_previous_sorting_of_the_result_is_retained_when_user_edits_appeal_category_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var categoryId1 = testData["CategoryId1"];
                var procCodeFrom1 = testData["ProcCodeFrom1"];
                var procCodeTo1 = testData["ProcCodeTo1"];
                var categoryOrder1 = testData["CategoryOrder1"];
                var trigProcFrom1 = testData["TrigProcFrom1"];
                var trigProcTo1 = testData["TrigProcTo1"];
                var product1 = testData["Product1"];
                var analyst1 = testData["Analyst1"];
                var note1 = testData["Note1"];
                var productAbbreviation1 = testData["ProductAbbreviation1"];
                string[] newAppealData =
                {
                    categoryId1, procCodeFrom1, procCodeTo1, categoryOrder1, product1, trigProcFrom1, trigProcTo1,
                    analyst1, note1, productAbbreviation1
                };

                try
                {
                    StringFormatter.PrintMessage("Create New Appeal Category Manager");
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation1,
                        procCodeFrom1 + "-" + procCodeTo1, trigProcFrom1 + "-" + trigProcTo1);
                    _appealCategoryManager.CreateAppealCategory(newAppealData);
                    var i = 1;
                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(1);
                    _appealCategoryManager.IsCategoryValueSorted()
                        .ShouldBeTrue("Records are Sorted in Alphabetical Order By Category");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsCategoryValueSorted()
                        .ShouldBeTrue("Sort By Category Value is retained after edit.");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(2);
                    _appealCategoryManager.IsCategoryOrderValueSorted()
                        .ShouldBeTrue("Records are Sorted in Alphabetical Order By Order");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsCategoryOrderValueSorted()
                        .ShouldBeTrue("Records are Sorted in Alphabetical Order By Order ");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(3);
                    _appealCategoryManager.IsProductValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Category Product");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsProductValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Category Product");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(4);
                    _appealCategoryManager.IsProcCodeFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code From");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsProcCodeFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code From");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(5);
                    _appealCategoryManager.IsProcCodeToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code To");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsProcCodeToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Proc Code To");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(6);
                    _appealCategoryManager.IsTrigProcFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc From");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsTrigProcFromValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc From");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(7);
                    _appealCategoryManager.IsTrigProcToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc To");
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    _appealCategoryManager.IsTrigProcToValueSorted()
                        .ShouldBeTrue("Records are Sorted in Ascending Order By Trig Proc To");

                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.ClickOnFilterOptionList(8);
                    _appealCategoryManager.UpdateAppealCategoryNoteOfOrderZero("UINOTE" + i++);
                    //_appealCategoryManager.IsLastModifiedDateSorted()
                    //    .ShouldBeTrue("Records are in Descending Order By Last Modified Date");
                    //_appealCategoryManager.IsLastModifiedDateSorted()
                    //    .ShouldBeTrue("Records are in Descending Order By Last Modified Date");
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation1,
                        procCodeFrom1 + "-" + procCodeTo1, trigProcFrom1 + "-" + trigProcTo1);
                    _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                            productAbbreviation1,
                            procCodeFrom1 + "-" + procCodeTo1, trigProcFrom1 + "-" + trigProcTo1)
                        .ShouldBeFalse("Ok button Clicked, Appeal row present.");
                }
                finally
                {
                    if (!_appealCategoryManager.IsCategoryOrderValueSorted())
                        _appealCategoryManager.ClickOnFilterOptionList(2);
                    if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                        _appealCategoryManager.ClosePageError();
                    _appealCategoryManager.DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation1,
                        procCodeFrom1 + "-" + procCodeTo1, trigProcFrom1 + "-" + trigProcTo1);
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.ClickOnClearButton();
                    _appealCategoryManager.ClickOnSearchIcon();

                }
            }
        }


        [Test, Category("SmokeTestDeployment")] //TANT-97
        [Retrying(Times = 3)]
        public void Verify_appeal_category_manager_page_specifications()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage _appealCategoryManager;
                _appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Verify_sorting_for_primary_secondary_options").Values
                    .ToList();
                var analystAssignmentSubHeaderList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Analyst_assginment_sub_header").Values.ToList();

                try
                {
                    _appealCategoryManager.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual
                        (0, "Default search should show the most recent data");
                    _appealCategoryManager.IsSearchIconPresent().ShouldBeTrue("Search icon should be present");
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.GetSideBarPanelSearch.WaitSideBarPanelOpen();
                    _appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                        .ShouldBeTrue("Sidebar panel should open");
                    _appealCategoryManager.ClickOnSearchIcon();
                    _appealCategoryManager.GetSideBarPanelSearch.WaitSideBarPanelClosed();
                    _appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen()
                        .ShouldBeFalse("Side bar panel should close");

                    _appealCategoryManager.IsAddCategoryCodeIconPresent()
                        .ShouldBeTrue("Add category icon should be present");
                    _appealCategoryManager.ClickAddNewCategory(true);
                    _appealCategoryManager.IsAddNewCategoryCodeSectionDisplayed()
                        .ShouldBeTrue("Is Add Apeal Category Displayed?");
                    _appealCategoryManager.ClickOnDisabledCancelButton();
                    _appealCategoryManager.IsAddNewCategoryCodeSectionDisplayed()
                        .ShouldBeFalse("Is Add Apeal Category Displayed?");

                    _appealCategoryManager.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Option List Equal");
                    _appealCategoryManager.IsFilterOptionPresent().ShouldBeTrue("Sort options must be displayed");
                    _appealCategoryManager.ClickOnFilterOption();
                    _appealCategoryManager.IsFilterOptionPresent()
                        .ShouldBeFalse("Sort options should not be displayed");

                    _appealCategoryManager.ClickonEditIconByRow(1);
                    _appealCategoryManager.IsEditFormDisplayed("Edit Appeal Category")
                        .ShouldBeTrue("Edit Category Code Section is displayed below the selected record");
                    _appealCategoryManager.GetHeaderEditAppealCategory().ShouldBeEqual("Edit Appeal Category",
                        "Edit Appeal Category title Should be equal.");
                    _appealCategoryManager.ClickonEditIconByRow(1, true);
                    _appealCategoryManager.IsEditFormDisplayed("Edit Appeal Category")
                        .ShouldBeFalse("Edit Category Code Section should be compressed");

                    _appealCategoryManager.ClickDeleteIconByRow(1);
                    _appealCategoryManager.WaitToLoadPageErrorPopupModal(5000);
                    _appealCategoryManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Pop up message should be present");
                    if (_appealCategoryManager.GetPageErrorMessage().Contains("Please complete all appeals"))
                        _appealCategoryManager.ClosePageError();
                    else
                    {
                        _appealCategoryManager.GetPageErrorMessage()
                            .ShouldBeEqual("The selected category code will be deleted. Click Ok to proceed or Cancel.",
                                "Error message should match");
                        _appealCategoryManager.IsOkButtonPresent().ShouldBeTrue("Ok button should be present");
                        _appealCategoryManager.IsCancelLinkPresent().ShouldBeTrue("Cancel Link should be present");
                        _appealCategoryManager.ClickOkCancelOnConfirmationModal(false);
                    }

                    _appealCategoryManager.ClickOnAppealCategoryRow();
                    _appealCategoryManager.GetAnalystAssignmentHeader()
                        .ShouldBeEqual("Analyst Assignment", "header should match");
                    _appealCategoryManager.GetAnalystAssignmentSubHeaderList()
                        .ShouldBeEqual(analystAssignmentSubHeaderList, "Sub headers should match");
                    _appealCategoryManager.GetAnalystListOnAnalystAssignmentCount()
                        .ShouldBeGreaterOrEqual(1, "Analyst List should be displayed");
                }

                finally
                {
                    _appealCategoryManager.ClickOnQuickLaunch().NavigateToAppealCategoryManager();
                }
            }

        }
        [Test]//CAR-3263(CAR-3044)
        public void Verify_Appeal_Category_Assignment_Based_On_Analyst_Order()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage appealCategoryManager;
                AppealManagerPage _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var category = testData["category"];
                var claseq = testData["claseq"];
                _appealManager.SearchByClaimSequence(claseq);
                var appealList = _appealManager.GetGridViewSection.GetGridListValueByCol(5);
                foreach (var appeal in appealList)
                {
                    _appealManager.DeleteAppealByAppealSeq(appeal);
                }

                appealCategoryManager = _appealManager.NavigateToAppealCategoryManager();
                appealCategoryManager.DeleteAppealSpecificToAppealCategory(category);
                appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID",category);
                appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                appealCategoryManager.ClickOnAppealCategoryRow();
                var analysts = appealCategoryManager.GetAssignedAnalystByType(true);

                for (int i=0;i< analysts.Count;i++)
                {
                    AppealCreatorPage appealCreator = appealCategoryManager.NavigateToAppealCreator();
                    appealCreator.SearchByClaimSequence(claseq);
                    appealCreator.SelectClaimLine();
                    appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "123");

                    AppealManagerPage appealManager = appealCreator.NavigateToAppealManager();
                    appealManager.SearchOutstandingAppealByClaimSequence(claseq);
                    appealManager.GetGridViewSection.GetGridListValueByCol(8)[0].Trim().ShouldBeEqual(analysts[i].Trim(),$" Is appeal assigned to next analyst : {analysts[i]} next in order ?");
                    appealManager.ChangeStatusOfAppealByRowSelection(newStatus:AppealStatusEnum.Complete.ToString());
                }
            }

        }
        
        #endregion

        #region PRIVATE METHODS

        //private void DeletePreviousAppeals(string claimSeq)
        //{
        //    if (CurrentPage.GetPageHeader() != "Appeal Manager")
        //    {
        //        CurrentPage = _appealManager = CurrentPage.NavigateToAppealManager();
        //    }
        //    _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
        //    CurrentPage =
        //             _appealCreator = _appealManager.NavigateToAppealCreator();
        //}
        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectFormat(string name)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue("The Name '"+ name + "' is in format XXX XXX (XXX)");
        }

        private void VerifyThatProcValueIsInCorrectFormat(string proc)
        {
            Regex.IsMatch(proc, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Proc value '" + proc + "' is in format XXXXX-XXXXX");
        }

        private void VerifyThatTrigValueIsInCorrectFormat(string trig)
        {
            if(String.IsNullOrEmpty(trig))
                Console.WriteLine("The trig value is null or empty");
            else
                Regex.IsMatch(trig, @"^[0-9a-zA-Z]+-[0-9a-zA-Z]+$").ShouldBeTrue("The Trig value '" + trig + "' is in format XXXXX-XXXXX");
        }

        private void VerifyThatLastModifiedDateIsInCorrectFormat(string date)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The Last Modified Date'" + date + "' is in format MM/DD/YYYY");
        }


        /*private void DeleteAppealCateogryAndAssociatedAppeals(string[] procRange, string[] categoryDetails, string client = "SMTST")
        {

            StringFormatter.PrintMessage(
                "Delete appeals associated if there are any, and delete appeal category.");
            if (
                _appealCategoryManager.IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                    categoryDetails[2], procRange[0] + "-" + procRange[1], null, client: client))
            {
                _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(categoryDetails[2], procRange[0] + "-" + procRange[1], null, client);
                if (_appealCategoryManager.IsPageErrorPopupModalPresent())
                    if (
                        _appealCategoryManager.GetPageErrorMessage()
                            .Equals(
                                "Please complete all appeals with the RRASSIGN category code prior to deleting it."))
                    {
                        _appealCategoryManager.ClosePageError();
                        _appealManager = _appealCategoryManager.NavigateToAppealManager();
                        _appealManager.SelectAllAppeals();
                        //_appealManager.SelectSMTST();
                        _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                        _appealManager.SelectDropDownListbyInputLabel("Status", "All");
                        _appealManager.SelectSearchDropDownListForMultipleSelectValue("Category", categoryDetails[0]);
                        _appealManager.ClickOnFindButton();
                        _appealManager.WaitForWorkingAjaxMessage();
                        if (!_appealManager.IsNoMatchingRecordFoundMessagePresent())
                        {
                            while (_appealManager.GetAppealSearchResultRowCount() != 0)
                            {
                                _appealManager.ClickOnDeleteIconByRowSelector(1);
                                _appealManager.ClickOkCancelOnConfirmationModal(true);
                            }  
                        }
                        _appealManager.NavigateToAppealCategoryManager();
                        _appealCategoryManager.ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(categoryDetails[2], procRange[0] + "-" + procRange[1], null, client);
                    }
                _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);
            }
        }*/

        public void Verify_all_edit_fields_show_current_value_after_editing()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var categoryId = testData["CategoryId"];
            var procCodeFrom = testData["ProcCodeFrom"];
            var procCodeTo = testData["ProcCodeTo"];
            var categoryOrder = testData["CategoryOrder"];
            var trigProcFrom = testData["TrigProcFrom"];
            var trigProcTo = testData["TrigProcTo"];
            var product = testData["Product"];
            var analyst = testData["Analyst"];
            var note = testData["Note"];
            var productAbbreviation = testData["ProductAbbreviation"];
            string[] newAppealData = { categoryId, procCodeFrom, procCodeTo, categoryOrder, product, trigProcFrom, trigProcTo, analyst, note, productAbbreviation };
        }

        [Test] //CAR-3195(CAR-3242)
        [Author("Pujan Aryal")]
        public void Verify_Pagination_And_Load_More_Icon_Functionality_In_Appeal_Category_Manager_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealCategoryManagerPage appealCategoryManager;
                automatedBase.CurrentPage =
                    appealCategoryManager = automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                var rowCount = appealCategoryManager.GetGridViewSection.GetGridRowCount();
                rowCount.ShouldBeEqual(25, "Maximum 25 results displayed initially?");
                appealCategoryManager.GetGridViewSection.IsLoadMorePresent().ShouldBeTrue("Is load more icon present?");

                var loadMoreValue = appealCategoryManager.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                var scrollCount = numbers[1] / 25;

                for (int i = 1; i < scrollCount; i++)
                {
                    appealCategoryManager.GetGridViewSection.ClickLoadMore();
                    rowCount += 25;
                    appealCategoryManager.GetGridViewSection.GetGridRowCount()
                        .ShouldBeEqual(rowCount, "25 more results added?");
                }

                if (appealCategoryManager.IsLoadMoreLinkable())
                {
                    appealCategoryManager.GetGridViewSection.ClickLoadMore();
                    appealCategoryManager.GetGridViewSection.GetGridRowCount().ShouldBeEqual(numbers[1]);
                }
            }
        }

        #endregion
    }
}

