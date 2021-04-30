using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using List = iTextSharp.text.List;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealSearch
    {
        
        /*#region PRIVATE FIELDS


        private AppealSearchPage _appealSearch;
        private AppealActionPage _appealAction;

        private AppealSummaryPage _appealSummary;
        private AppealManagerPage _appealManager;
        private List<string> _appealAssignableUserList;
        private List<string> _cvtyPlanList;
        private List<string> _pehpPlanList;
        private List<string> _smtstPlanList;
        private List<string> _ttreePlanList;
        private List<string> _assignedClientList;
        private List<string> _lineOfBusinessList;

        #endregion

        #region OVERRIDE METHODS
        protected override void ClassInit()
        {
            try
            {
                UserLoginIndex = 1;
                base.ClassInit();
              
                CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();

                try
                {
                    RetrieveListFromDatabase();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


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
            CurrentPage = _appealSearch;
        }

        protected override void TestCleanUp()
        {
            CurrentPage.CloseAnyPopupIfExist();
            if (CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
            {
                CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
            }
            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealSearch.GetStringValue()))
            {
                CurrentPage = _appealSearch = CurrentPage.NavigateToAppealSearch();
            }
            if (String.Compare(UserType.CurrentUserType, UserType.HCIADMIN1, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _appealSearch = _appealSearch.Logout().LoginAsHciAdminUser1().NavigateToAppealSearch();
            }

            if (!_appealSearch.IsFindAppealSectionPresent())
                _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();

            _appealSearch.ClearAll();
            _appealSearch.SelectAllAppeals();

            base.TestCleanUp();
        }


        #endregion
            
             #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {
            _appealAssignableUserList = _appealSearch.GetPrimaryReviewerAssignedToList();
            _cvtyPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
            _pehpPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.PEHP.ToString());
            _smtstPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
            _ttreePlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.TTREE.GetStringDisplayValue());
            _assignedClientList = _appealSearch.GetAssignedClientList(EnvironmentManager.HciAdminUsername1);
            _assignedClientList.Insert(0, "All");
            _lineOfBusinessList = _appealSearch.GetCommonSql.GetLineOfBusiness();
            _appealSearch.CloseDbConnection();
        }



        #endregion*/

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion



        #region TEST SUITES


        [Test] //CAR-2992(CAR-2962) + CAR-3121(CAR-3063)
        [Order(3)]
        public void Verify_appeal_level_count_should_be_unique_to_that_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex:1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SearchByClaimSequence(claimSequence, ClientEnum.SMTST.ToString());

                #region CAR-3121(CAR-3063)
                _appealSearch.GetSearchResultListByCol(6).Distinct().ShouldCollectionBeEquivalent(new List<string> { "A", "MRR" }, "List should contain MRR that have been created on that claim.");
                _appealSearch.IsAppealLevelBadgeValuePresentForMRRAppealType().ShouldBeTrue("Badge should show the count of appeal types of appeal and MRR that have been created on that claim.");
                #endregion

                _appealSearch.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealSearch.GetAppealLevel(claimSequence, ClientEnum.SMTST),
                    $"Is Correct Appeal Level display for {ClientEnum.SMTST}");
                _appealSearch.SearchByClaimSequence(claimSequence, ClientEnum.RPE.ToString());
                _appealSearch.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealSearch.GetAppealLevel(claimSequence, ClientEnum.RPE),
                    $"Is Correct Appeal Level display for {ClientEnum.RPE}");
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-93
        public void Verify_Appeal_Search_Find_Panel_Icon_And_Sort_Options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, TestName).Values
                        .ToList();

                StringFormatter.PrintMessage("Verify open/close of Find Appeals Panel");
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" By default Find Appeals panel displayed ?");
                _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse(" Find Appeals panel displayed ?");
                _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" Find Appeals panel displayed ?");

                StringFormatter.PrintMessage("Verify Sort Option List");
                _appealSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _appealSearch.GetGridViewSection.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                _appealSearch.GetGridViewSection.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Be Equal");
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("AppealDependent")] //TANT-93
        public void Verify_Appeal_Search_Results_And_Navigation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                StringFormatter.PrintMessage("Verify Results For Outstanding Appeals");
                _appealSearch.SelectSearchDropDownListValue("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
                _appealSearch.SelectClientSmtst();
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");
                var appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(2);
                appealAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue(),
                    "Appeal Action Page Should Be Presented");
                automatedBase.CurrentPage = _appealSearch = appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                StringFormatter.PrintMessage("Verify Results For All Appeals Quick Search Options");
                _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SelectSearchDropDownListValue("Status",
                    AppealStatusEnum.Complete.GetStringDisplayValue());
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");
                var appealSummary = _appealSearch.ClickOnAppealSequence(1);
                appealSummary.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue(),
                    "Page Header should be Appeal Summary");
                automatedBase.CurrentPage = _appealSearch = appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.WaitForWorking();
            }
        }

        [Test] //CAR-1108(CAR-637) +TE-800 + TE-889
        [Order(1)]
        [Category("EdgeNonCompatible")]
        public void Verify_dental_appeal_letter_for_second_level_and_higher_level()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;

                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ClaimSequence", "Value");
                const string expectedReviewDisclaimer =
                    "Cotiviti, Inc. Dental Consultant has reviewed the claim(s) listed below in response to your request for consideration of reimbursement{0}. Our review considered the following documents:";
                const string expectedApealLevelParagraph = ". This is a {0} level appeal";
                const string firstParagraph =
                    "Please contact Cotiviti, Inc with any questions. You or your representative have the right to inspect and request copies of all relevant documents that are related to this request free of charge. A copy of the criteria used to make this determination is available upon request.";
                const string secondParagraph =
                    "The claimant has the right to a review provided by an independent review organization (IRO). A request for a review by an IRO form must be completed by the enrollee, an individual acting on behalf of the enrollee, or the enrollee’s provider of record and be returned to Cotiviti within 60 days of the above review date. The covered person must provide written consent authorizing the independent review entity to obtain all necessary medical records. An IRO review request form must be signed by the enrollee or the enrollee’s legal guardian.{0}";
                const string thirdParagraph =
                    "Please submit all documentation supporting the requested procedure(s) to Cotiviti, Inc. The request for independent review and all other documentation will then be supplied to the IRO for consideration.";
                const string fouthParagraph =
                    "A determination will be made by the independent review entity within two (2) business days for expedited external requests from the receipt of all information required from the insurer. Expedited requests are allowed in the case of urgent or emergent health care services. External reviews that are not expedited will be conducted by the independent review entity and a determination made within ten (10) business days from the receipt of all information required from the insurer. There will be no fee charged to the patient or provider for the independent review. Please submit the request for IRO to:";
                var actualState =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ActualState",
                        "Value");
                var changedState =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ChangedState",
                        "Value");
                var providerSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ProviderSequence", "Value");
                var paraGraphListForFalseIroValue = new List<string>
                    {firstParagraph, string.Format(secondParagraph, ""), thirdParagraph, fouthParagraph};
                var contactDetail = new List<string>
                {
                    "Cotiviti, Inc | Payment Accuracy Division",
                    "10897 South Riverfront Parkway, Suite 200",
                    "South Jordan, UT 84095",
                    "Toll-free: (877) 554-5006",
                    "Fax: (801) 285-5801",
                    "Monday-Friday 6:00 am - 6:00 pm MST"
                };
                var footerNote = new List<string>
                {
                    "Reviewed by: Test Automation", "|", "Analyst", "|", "Cotiviti",
                    "clientservices@cotiviti.com", "|", "direct: 801.285.5800", "|", "fax: 801.285.5801",
                };
                try
                {
                    var letter = "";
                    var footer = new List<string>();
                    var fileName = "";
                    var appealLetterFullDetails = "";
                    _appealSearch.GetCommonSql.UpdateStateIroByStateCode("T", actualState);
                    _appealSearch.GetCommonSql.UpdateStateOfProviderByProviderSequence(actualState, providerSequence);
                    var expectedStateIroList = _appealSearch.GetCommonSql.GetStateIroByStateCode(actualState);
                    var expectedStateIroListForNullValues =
                        _appealSearch.GetCommonSql.GetStateIroByStateCode(changedState);
                    var paragraphListForNullColumn = new List<string>
                    {
                        firstParagraph,
                        string.Format(secondParagraph, " " + expectedStateIroListForNullValues[1].Trim()),
                        thirdParagraph,
                        fouthParagraph
                    };
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                    _appealSearch.ClickOnFindButtonAndWait();
                    StringFormatter.PrintMessageTitle(
                        "Verification of Review Disclaimer for appeal level 1 to 4 or greater than 4");
                    var i = 1;
                    for (; i < 6; i++)
                    {
                        var appealLetter = _appealSearch.ClickOnApealLetterIconByAppealLevel(i);
                        var reviewDisclaimer = appealLetter.GetReviewDisclaimer().Split('\r')[0];
                        switch (i)
                        {
                            case 2:
                                reviewDisclaimer.ShouldBeEqual(
                                    string.Format(expectedReviewDisclaimer,
                                        string.Format(expectedApealLevelParagraph, "2nd")),
                                    "Verification of First Paragraph that contains Appeal Level=2");
                                break;
                            case 3:
                                reviewDisclaimer.ShouldBeEqual(
                                    string.Format(expectedReviewDisclaimer,
                                        string.Format(expectedApealLevelParagraph, "3rd")),
                                    "Verification of First Paragraph that contains Appeal Level=3");
                                break;
                            case 4:
                                reviewDisclaimer.ShouldBeEqual(
                                    string.Format(expectedReviewDisclaimer,
                                        string.Format(expectedApealLevelParagraph, "4th or greater")),
                                    "Verification of First Paragraph that contains Appeal Level=4");
                                break;
                            case 5:
                                reviewDisclaimer.ShouldBeEqual(
                                    string.Format(expectedReviewDisclaimer,
                                        string.Format(expectedApealLevelParagraph, "4th or greater")),
                                    "Verification of First Paragraph that contains Appeal Level=5");

                                letter = appealLetter.GetAppealLetterClosing();
                                ValidateIroDetail(letter, new List<string>(expectedStateIroList));
                                footer = appealLetter.GetFooterDetail();
                                appealLetterFullDetails = appealLetter.GetAppealLetterFullDetail();
                                fileName = _appealSearch.ClickOnDownloadPDFAndGetFileName(appealLetter);
                                break;
                            default:
                                reviewDisclaimer.ShouldBeEqual(
                                    string.Format(expectedReviewDisclaimer.Replace("Technologies,", "Technologies"),
                                        ""),
                                    "Verification of First Paragraph that contains Appeal Level<2");
                                letter = appealLetter.GetAppealLetterClosing();
                                ValidateIroDetail(letter, new List<string>(expectedStateIroList), false);
                                break;
                        }

                        _appealSearch = appealLetter.CloseLetterPopUpPageAndBackToAppealSearch();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verify Each Paragraph including closing one with contact details and IRO Details");
                    var appealLetterList = ReadLetterAndParseIntoList(letter);
                    appealLetterList[0].ShouldBeEqual(firstParagraph, "Verify First Closing Paragraph");
                    appealLetterList[1].ShouldBeEqual(
                        string.Format(secondParagraph, " " + expectedStateIroList[1].Trim()),
                        "Verify Second Paragraph");
                    appealLetterList.GetRange(2, 10).ShouldCollectionBeEqual(expectedStateIroList.GetRange(2, 10),
                        "Verification of State IRO Information");
                    appealLetterList[12].ShouldBeEqual(thirdParagraph, "Verification of Third Paragraph");
                    appealLetterList[13].ShouldBeEqual(fouthParagraph, "Verification of Fourth Paragraph");
                    appealLetterList.GetRange(14, 6).ShouldCollectionBeEqual(contactDetail,
                        "Verification of Contact Detail");
                    footer.ShouldCollectionBeEqual(footerNote, "Verification of Footer Note");
                    StringFormatter.PrintMessageTitle(
                        "Update State IRO to False and Verify IRO Information Should not display");
                    _appealSearch.GetCommonSql.UpdateStateIroByStateCode("F", actualState);
                    appealLetterList = OpenLetterAndParseIntoList(_appealSearch, i - 1);
                    appealLetterList.GetRange(0, 4)
                        .ShouldCollectionBeEqual(paraGraphListForFalseIroValue, "Verify Appeal Letter for IRO='F'");
                    StringFormatter.PrintMessageTitle(
                        "Update State of Provider having null data in IRO and Verify IRO Information Should not display");
                    _appealSearch.GetCommonSql.UpdateStateOfProviderByProviderSequence(changedState, providerSequence);
                    appealLetterList = OpenLetterAndParseIntoList(_appealSearch, i - 1);
                    appealLetterList.GetRange(0, 4)
                        .ShouldCollectionBeEqual(paragraphListForNullColumn,
                            "Verify Appeal Letter for Null value in IRO Details");
                    StringFormatter.PrintMessageTitle("Verify Appeal Letter Page Content to the downloaded PDF");
                    ValidatePDFContents(fileName, appealLetterFullDetails);
                }
                finally
                {
                    _appealSearch.GetCommonSql.UpdateStateIroByStateCode("T", actualState);
                    _appealSearch.GetCommonSql.UpdateStateOfProviderByProviderSequence(actualState, providerSequence);
                    _appealSearch.CloseAnyTabIfExist();
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }
            }
        }

        [Test] // CAR-779 + TE-566
        [Order(6)]
        public void Verify_original_sort_order_when_working_from_appeal_to_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var appealSequence = paramLists["AppealSeq"];
                try
                {
                    _appealSearch.UpdateAppealStatusToIncomplete(appealSequence);
                    if (!_appealSearch.IsAdvancedSearchFilterIconSelected())
                        _appealSearch.ClickOnAdvancedSearchFilterIconForMyAppeal();

                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", paramLists["Status"]);
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To",
                        paramLists["AssignedTo"]);
                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("First Deny Code", paramLists["DenyCode"]);
                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("First Pay Code", paramLists["PayCode"]);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.ClickOnFilterOptionListRow(4);
                    var initialAppealList = _appealSearch.GetSearchResultListByCol(3);

                    _appealSearch.IsListStringSortedInAscendingOrder(4)
                        .ShouldBeTrue("Search result list is sorted in ascending order of claim no should be true");
                    _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                    var totalCount = initialAppealList.Count;
                    var lastAppeal = initialAppealList[totalCount - 1];

                    foreach (var value in initialAppealList)
                    {
                        var appealSeq = _appealAction.GetAppealSequence();
                        appealSeq.ShouldBeEqual(value,
                            "Appeal Seq should be on order to the appeal seq in Appeal Search page's sorted order");
                        _appealAction.CompleteAppeals(isAction: false, isCompleted: true);
                        if (!value.Equals(lastAppeal))
                            _appealAction.HandleAutomaticallyOpenedActionPopup();
                    }

                    _appealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue(),
                        "Appeal search page should be retained");

                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClickOnFilterOptionListRow(10);
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }
            }
        }


        [Test, Category("SmokeTestDeployment"),Category("EdgeNonCompatible")] //US67260 //TANT-93
        public void Verify_Appeal_letter_download_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                try
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                        AppealStatusEnum.Complete.GetStringDisplayValue());
                    _appealSearch.ClickOnFindButtonAndWait();

                    if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.AppealSearch.GetStringValue())
                    {
                        _appealSearch.ClickOnBrowserBackButton();
                    }

                    var appealLetter = _appealSearch.ClickOnAppealLetter(1);
                    appealLetter.GetAppealLetterPageHeader().ShouldBeEqual(PageHeaderEnum.AppealLetter.GetStringValue(),
                        "Appeal Letter Popup", true);

                    appealLetter.ClickOnClaimNoAndGetPageHeader().ShouldBeEqual(
                        PageHeaderEnum.ClaimAction.GetStringValue(),
                        "Claim Action Tab should display when click on claim Number");
                    appealLetter.SwitchWindowByTitle(PageTitleEnum.AppealLetter);

                    var fileName = _appealSearch.ClickOnDownloadPDFAndGetFileName(appealLetter);
                    _appealSearch.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + fileName);
                    File.Exists(@"C:/Users/uiautomation/Downloads/" + fileName)
                        .ShouldBeTrue("Appeal Letter should be downloaded");

                }
                finally
                {
                    _appealSearch.CloseAnyPopupIfExist();
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                }
            }
        }

        [Test] //US45658 + TE-200
        [Order(11)]
        public void Verify_sorting_of_appeal_search_result_for_different_sort_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                try
                {

                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectSearchDropDownListValue("Appeal Level", "All");
                    _appealSearch.SetDateFieldFrom(15, "02/1/2016", "Create Date");
                    _appealSearch.SetDateFieldTo(15, "03/1/2016", "Create Date");
                    //_appealSearch.SelectClientSmtst();
                    _appealSearch.SendEnterKeysOnClientTextField();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.GetAppealSearchResultRowCount()
                        .ShouldBeGreater(0, "Appeal Search Result found when hit <Enter> key");

                    _appealSearch.IsListDateSortedInAscendingOrder(2)
                        .ShouldBeTrue("Due Date should Sorted in Ascending Order by default");

                    ValidateAppealSearchRowSorted(_appealSearch, 7, 6, "Category");
                    ValidateAppealSearchRowSorted(_appealSearch, 8, 7, "Analyst");
                    ValidateAppealSearchRowSorted(_appealSearch, 9, 8, "First Deny Code");
                    ValidateAppealSearchRowSorted(_appealSearch, 11, 9, "Status");
                    ValidateAppealSearchRowSorted(_appealSearch, 3, 3, "Appeal Seq");
                    ValidateAppealSearchRowSorted(_appealSearch, 5, 5, "Client");

                    _appealSearch.ClickOnFilterOptionListRow(2);
                    _appealSearch.IsListDateSortedInDescendingOrder(2)
                        .ShouldBeTrue("Record Should Be Sorted Descending Order By Due Date");
                    _appealSearch.ClickOnFilterOptionListRow(2);
                    _appealSearch.IsListDateSortedInAscendingOrder(2)
                        .ShouldBeTrue("Record Should Be Sorted Ascending Order By Due Date");

                    _appealSearch.ClickOnFilterOptionListRow(1);
                    _appealSearch.GetUrgentList()
                        .IsInAscendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");
                    _appealSearch.ClickOnFilterOptionListRow(1);
                    _appealSearch.GetUrgentList()
                        .IsInDescendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");



                    _appealSearch.SelectMyAppeals();
                    _appealSearch.SendEnterKeysOnClientTextField();
                    ValidateAppealSearchRowSorted(_appealSearch, 4, 4, "Claim No");


                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClickOnFilterOptionListRow(10);
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }

            }
        }
        [NonParallelizable]
        [Test] //US45658 + CAR-3121(CAR-3063)
        public void Verify_search_result_are_correct()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ClaimSequence",
                        "Value");
                var lockBy =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "LockedBy", "Value");
                var appealSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "AppealSequence",
                        "Value");
                var altClaimNo =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "AltClaimNo",
                        "Value");
                AppealLetterPage appealLetter = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                try
                {
                    if (_appealSearch.IsAppealLockIconPresent(appealSeq))
                    {
                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                        automatedBase.CurrentPage =
                            _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    if (_appealSearch.GetSearchResultByRowCol(5, 11) != "New")
                    {
                        automatedBase.CurrentPage =
                            _appealManager = _appealSearch.NavigateToAppealManager();
                        _appealManager.ChangeStatusOfAppeal(null, appealSeq);
                        automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
                        _appealSearch.SelectAllAppeals();
                        _appealSearch.SelectClientSmtst();
                        _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                        _appealSearch.ClickOnFindButton();
                        _appealSearch.WaitForWorkingAjaxMessage();

                    }

                    _appealSearch.IsAppealLockIconPresent(appealSeq).ShouldBeFalse("Appeal Should not Locked");

                    automatedBase.CurrentPage = _appealAction = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    _appealSearch.IsAppealLockIconPresentAndGetLockIconToolTipMessage(appealSeq, pageUrl)
                        .ShouldBeEqual(string.Format("This appeal is locked by {0}", lockBy),
                            "Appeal Icon is locked and Tooltip Message");
                    _appealSearch.CloseAppealActionTabIfExists();


                    _appealSearch.IsBlackColorDueDatePresent(4)
                        .ShouldBeTrue("Black Color Due Date Present for Complete Status");
                    _appealSearch.IsBlackColorDueDatePresent(3)
                        .ShouldBeTrue("Black Color Due Date Present for Closed Status");
                    _appealSearch.OpenAppealManagerInNewTab(appealSeq);
                    _appealSearch.SwitchToNewTabAndChangeDueDate(DateTime.Now.AddDays(-1).ToString("MM/d/yyyy"));
                    _appealSearch.IsBoldRedColorDueDatePresent()
                        .ShouldBeTrue(
                            "Bold Red Color Due Date Present for due dates that are previous to the current date");

                    _appealSearch.SwitchToNewTabAndChangeDueDate(DateTime.Now.ToString("MM/d/yyyy"));
                    _appealSearch.IsNonBoldOnlyRedColorDueDatePresent()
                        .ShouldBeTrue(
                            "Non Bold Only Red Color Due Date Present for due dates that are equal to the current date");

                    _appealSearch.SwitchToNewTabAndChangeDueDate(DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
                    _appealSearch.IsOrangeColorDueDatePresent()
                        .ShouldBeTrue(
                            "Orange Color Due Date Present for due dates that are for tomorrows date to the current date");

                    _appealSearch.SwitchToNewTabAndChangeDueDate(DateTime.Now.AddDays(2).ToString("MM/d/yyyy"));
                    _appealSearch.IsBlackColorDueDatePresent(5)
                        .ShouldBeTrue("Black Color Due Date Present for Due dates that are after tomorrows");
                    VerifyThatDateIsInCorrectFormat(_appealSearch.GetSearchResultByRowCol(5, 2), "Due Date");

                    _appealSearch.GetSearchResultByRowCol(1, 11)
                        .ShouldBeEqual("Complete", "Status Should display Complete");
                    _appealSearch.GetSearchResultByRowCol(3, 11)
                        .ShouldBeEqual("Closed", "Status Should display Closed");

                    _appealSearch.SwitchToNewTabAndChangeStatus("Open");
                    _appealSearch.GetSearchResultByRowCol(5, 11).ShouldBeEqual("Open", "Status Should display New");

                    _appealSearch.SwitchToNewTabAndChangeStatus("New");
                    _appealSearch.GetSearchResultByRowCol(5, 11).ShouldBeEqual("New", "Status Should display New");

                    _appealSearch.SwitchToNewTabAndChangeStatus("Documents Waiting");
                    _appealSearch.GetSearchResultByRowCol(5, 11)
                        .ShouldBeEqual("Documents Waiting", "Status Should display New");
                    _appealSearch.SwitchToNewTabAndChangeStatus("New");
                    _appealSearch.CloseAppealManagerTabIfExists();
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();

                    _appealSearch.IsUrgentIconPresent(1).ShouldBeFalse("Urgent Icon should not display");
                    _appealSearch.IsUrgentIconPresent(5).ShouldBeTrue("Urgent Icon should not display");

                    _appealSearch.GetAdjustValueByRow(1).ShouldBeEqual("A", "Adjust Icon Should Present");
                    _appealSearch.GetDenyValueByRow(2).ShouldBeEqual("D", "Deny Icon Should Present");
                    _appealSearch.GetPayValueByRow(3).ShouldBeEqual("P", "Pay Icon Should Present");
                    _appealSearch.GetNoDocsValueByRow(4).ShouldBeEqual("N", "No Docs Icon Should Present");

                    _appealSearch.IsAppealLetterIconPrsentByRow(1)
                        .ShouldBeTrue("Appeal Icon Present for status Complete");
                    _appealSearch.IsAppealLetterIconPrsentByRow(3)
                        .ShouldBeTrue("Appeal Icon Present for status Closed");
                    _appealSearch.IsAppealLetterIconPrsentByRow(5)
                        .ShouldBeFalse("Appeal Icon should not Present for status New");


                    appealLetter = _appealSearch.ClickOnAppealLetter(1);
                    appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Appeal Letter Popup");
                    automatedBase.CurrentPage = _appealSearch = appealLetter.CloseLetterPopUpPageAndBackToAppealSearch();

                    _appealSearch.GetSearchResultByRowCol(1, 3).ShouldNotBeEmpty("Appeal Sequence Should display");


                    automatedBase.CurrentPage = _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(5);
                    _appealAction.GetPageHeader()
                        .ShouldBeEqual("Appeal Action", "Appeal Summary Page open for complete status");
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    _appealSearch.GetSearchResultByRowCol(1, 5)
                        .ShouldBeEqual(ClientEnum.SMTST.ToString(), "Client Code Should display");
                    _appealSearch.GetSearchResultByRowCol(1, 6).ShouldBeEqual("A", "Appeal Type Should display");
                    _appealSearch.GetSearchResultByRowCol(3, 6).ShouldBeEqual("RR", "Appeal Type Should display");
                    _appealSearch.GetSearchResultByRowCol(3, 7).ShouldNotBeEmpty("Category Id Should display");
                    _appealSearch.GetSearchResultByRowCol(5, 4)
                        .ShouldBeEqual(altClaimNo, "AltClaimno should be displayed");
                    VerifyThatNameIsInCorrectFormat(_appealSearch.GetSearchResultByRowCol(3, 8),
                        "Primary Reviewer Should be in <First Name><Last Name>");
                    var pay1 = _appealSearch.GetSearchResultByRowCol(3, 9);
                    Regex.IsMatch(pay1, @"^([a-zA-Z0-9_-]){5,5}$")
                        .ShouldBeTrue("The First Pay value '" + pay1 + "' is in alphanumeric format XXXXX");
                    var deny1 = _appealSearch.GetSearchResultByRowCol(3, 10);
                    Regex.IsMatch(deny1, @"^([a-zA-Z0-9_-]){5,5}$")
                        .ShouldBeTrue("The First Deny value '" + deny1 + "' is in alphanumeric format XXXXX");

                    #region CAR-3121(CAR-3063)
                    StringFormatter.PrintMessage("Verify Search Result when Medical Record Review is selected");
                    _appealSearch.ClickOnClearLink();
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.SelectDropDownListbyInputLabel("Type","Medical Record Review");
                    _appealSearch.SetDateFieldFrom(17, "09/20/2020", "Create Date");
                    _appealSearch.SetDateFieldTo(17, "09/26/2020", "Create Date");
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.GetSearchResultListByCol(3).ShouldCollectionBeEquivalent(_appealSearch.GetMedicalRecordReviewAppealsFromDatabase(), "Results should yield appeal records that are of the Medical Record Review type");
                    #endregion


                }
                finally
                {
                    _appealSearch.CloseAppealActionTabIfExists();
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                    if (_appealSearch.IsAppealLockIconPresent(appealSeq))
                    {
                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                }
            }
        }

        [Test] //US69724
        [Order(13)]
        public void Verify_the_secondary_details_for_an_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSequence);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.ClickOnSearchListRow(1);
                _appealSearch.IsAppealDetailSectionOpen().ShouldBeTrue("Appeal Details section should be opened");


                _appealSearch.GetAppealDetailsLabel(1, 1)
                    .ShouldBeEqual("Product:", "Appeal details product label present");
                _appealSearch.GetAppealDetailsContentValue(1, 1)
                    .ShouldBeEqual(paramLists["Product"], "Appeal details Product value present");

                _appealSearch.GetAppealDetailsLabel(1, 2).ShouldBeEqual("Flags:", "Appeal details Flags label present");
                _appealSearch.GetAppealDetailsContentValue(1, 2)
                    .ShouldBeEqual(paramLists["Flag"], "Appeal details Flag value present");

                _appealSearch.GetAppealDetailsLabel(2, 1)
                    .ShouldBeEqual("Claim Count:", "Appeal details ClaimCount label present");
                _appealSearch.GetAppealDetailsContentValue(2, 1)
                    .ShouldBeEqual(paramLists["ClaimCount"], "Appeal details Claim Count value present");

                _appealSearch.GetAppealDetailsLabel(2, 2)
                    .ShouldBeEqual("Prov Seq:", "Appeal details Prov Seq label present");
                _appealSearch.GetAppealDetailsContentValue(2, 2)
                    .ShouldBeEqual(paramLists["ProvSeq"], "Appeal details Prov Seq value present");

                _appealSearch.GetAppealDetailsLabel(3, 1)
                    .ShouldBeEqual("Create Date:", "Appeal details Create date label present");
                _appealSearch.GetAppealDetailsContentValue(3, 1).IsDateInFormat()
                    .ShouldBeTrue("Create Date is in mm / dd / yyyy format");

                _appealSearch.GetAppealDetailsLabel(3, 2)
                    .ShouldBeEqual("Created By:", "Appeal details Created By label present");
                _appealSearch.GetAppealDetailsContentValue(3, 2).DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Created By Should be in <First Name><Last Name>");

                _appealSearch.GetAppealDetailsLabel(4, 1)
                    .ShouldBeEqual("Complete Date:", "Appeal details Complete Date label present");
                _appealSearch.GetAppealDetailsContentValue(4, 1).IsDateInFormat()
                    .ShouldBeTrue("Complete Date is in mm / dd / yyyy format");

                _appealSearch.GetAppealDetailsLabel(4, 2)
                    .ShouldBeEqual("Assigned To:", "Appeal details Assigned To label present");
                _appealSearch.GetAppealDetailsContentValue(4, 2)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Assigned To Should be in <First Name><Last Name>");

            }
        }

        [Test] //US45657
        [Order(7)]
        public void Verify_proper_search_validation_with_real_time_field_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var product = paramLists["Product"];
                var client = ClientEnum.SMTST.ToString();
                var status = paramLists["Status"];
                var type = paramLists["Type"];
                var priority = paramLists["Priority"];
                var appealLevel = paramLists["AppealLevel"];
                var firstDenyCode = paramLists["FirstDenyCode"];
                var firstPayCode = paramLists["FirstPayCode"];
                var invalidAppealSeq = paramLists["InvalidAppealSeq"];
                try
                {
                    _appealSearch.RefreshPage();
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Popup Displayed when search criteria is not entered");
                    _appealSearch.GetPageErrorMessage()
                        .ShouldBeEqual("Search cannot be initiated without any criteria entered.",
                            "Popup Message when search criteria is not entered");
                    _appealSearch.ClosePageError();

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Product", 7, product,
                        "Search cannot be initiated with Product only. A date range, Appeal Seq, Claim Seq, Appeal #, Provider Seq, or Reviewer search criteria is required.");

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Client", 3, client,
                        "Search cannot be initiated with Client only. A date range, Status, Appeal Seq, Claim Seq, Appeal #, Provider Seq or Reviewer is required.");

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Status", 8, status,
                        "Search cannot be initiated for all appeals in a status of Closed. Please select a different Status, a Date Range, Reviewer or Provider Seq.");

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Type", 12, type,
                        "Search cannot be initiated with Type only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.");

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Priority", 13, priority,
                        "Search cannot be initiated with Priority only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.");

                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Appeal Level", 20, appealLevel,
                        "Search cannot be initiated with Appeal Level only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.");

                    ValidateFieldErrorMessageForTextField(_appealSearch, "First Deny Code", 17, firstDenyCode,
                        "Search cannot be initiated with First Deny Code only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.");

                    ValidateFieldErrorMessageForTextField(_appealSearch, "First Pay Code", 18, firstPayCode,
                        "Search cannot be initiated with First Pay Code only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required");

                    _appealSearch.SetAppealSequence(invalidAppealSeq);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.", "No matching record found Message");

                    _appealSearch.ScrollToRow(14);
                    ValidateFieldErrorMessageForDateRange(_appealSearch, "Due Date", 14,
                        "Search cannot be initiated for date ranges greater than 3 months.");
                    ValidateFieldErrorMessageForDateRange(_appealSearch, "Create Date", 15,
                        "Search cannot be initiated for date ranges greater than 3 months.");
                    ValidateFieldErrorMessageForDateRange(_appealSearch, "Complete Date", 16,
                        "Search cannot be initiated for date ranges greater than 3 months.");


                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }

            }
        }


        [Test] //US45657
        [Order(15)]
        public void Verify_that_the_clear_filters_clears_all_filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                try
                {


                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SetSearchInputField("AppealSequece", paramLists["AppealSequence"], 2);
                    _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 3);
                    _appealSearch.SetSearchInputField("ClaimSequence", paramLists["ClaimSequence"], 4);
                    _appealSearch.SetSearchInputField("Claim No", paramLists["ClaimNo"], 5);
                    _appealSearch.SetSearchInputField("Appeal No", paramLists["AppealNo"], 6);
                    _appealSearch.SelectSearchDropDownListValue("Product", paramLists["Product"], 7);
                    _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Plan", paramLists["Plan"], 8);
                    _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Line of Business",
                        paramLists["LineOfBusiness"], 9);
                    _appealSearch.SelectSearchDropDownListValue("Status", paramLists["Status"], 10);
                    _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Primary Reviewer",
                        paramLists["PrimaryReviewer"], 11);
                    _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Assigned To",
                        paramLists["AssignedTo"], 12);
                    _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Category", paramLists["Category"],
                        13);
                    _appealSearch.SelectSearchDropDownListValue("Type", paramLists["Type"], 14);
                    _appealSearch.SelectSearchDropDownListValue("Priority", paramLists["Priority"], 15);
                    _appealSearch.SetDateFieldFrom(16, DateTime.Now.ToString("MM/d/yyyy"), "Due Date");
                    _appealSearch.SetDateFieldFrom(17, DateTime.Now.ToString("MM/d/yyyy"), "Create Date");
                    _appealSearch.SetDateFieldFrom(18, DateTime.Now.ToString("MM/d/yyyy"), "Completed Date");
                    _appealSearch.SetSearchInputField("FirstDenyCode", paramLists["FirstDenyCode"], 19);
                    _appealSearch.SetSearchInputField("FirstPayCode", paramLists["FirstPayCode"], 20);
                    _appealSearch.SetSearchInputField("ProviderSequence", paramLists["ProviderSequence"], 21);
                    _appealSearch.SelectSearchDropDownListValue("AppealLevel", paramLists["AppealLevel"], 22);
                    _appealSearch.ClearAll();
                    StringFormatter.PrintMessageTitle("Verify Clear Filter clears all the search filters");
                    _appealSearch.GetSearchInputField(2).ShouldBeEqual("", "Appeal Sequence");
                    _appealSearch.GetSearchInputField(3).ShouldBeEqual("All", "Client");
                    _appealSearch.GetSearchInputField(4).ShouldBeEqual("", "Claim Sequence");
                    _appealSearch.GetSearchInputField(5).ShouldBeEqual("", "Claim No");
                    _appealSearch.GetSearchInputField(6).ShouldBeEqual("", "Appeal No");
                    _appealSearch.GetSearchInputField(7).ShouldBeEqual("All", "Product");
                    _appealSearch.GetSearchInputField(8).ShouldBeEqual("All", "Status");
                    _appealSearch.GetSearchInputField(9).ShouldBeEqual("", "Primary Reviewer");
                    _appealSearch.GetSearchInputField(10).ShouldBeEqual("", "Assigned To");
                    _appealSearch.GetSearchInputField(11).ShouldBeEqual("", "Category");
                    _appealSearch.GetSearchInputField(12).ShouldBeEqual("All", "Type");
                    _appealSearch.GetSearchInputField(13).ShouldBeEqual("All", "Priority");
                    _appealSearch.GetDateFieldFrom(14).ShouldBeEqual("", "Due Date From");
                    _appealSearch.GetDateFieldTo(14).ShouldBeEqual("", "Due Date To");
                    _appealSearch.GetDateFieldFrom(15).ShouldBeEqual("", "Create Date From");
                    _appealSearch.GetDateFieldTo(15).ShouldBeEqual("", "Create Date To");
                    _appealSearch.GetDateFieldFrom(16).ShouldBeEqual("", "Complete Date From");
                    _appealSearch.GetDateFieldTo(16).ShouldBeEqual("", "Complete Date To");
                    _appealSearch.GetSearchInputField(17).ShouldBeEqual("", "First Deny Code");
                    _appealSearch.GetSearchInputField(18).ShouldBeEqual("", "First Pay Code");
                    _appealSearch.GetSearchInputField(19).ShouldBeEqual("", "Provider Sequence");
                    _appealSearch.GetSearchInputField(20).ShouldBeEqual("All", "Appeal Level");

                    StringFormatter.PrintMessageTitle("Verify Clear Filter does not work for Quick Search");
                    _appealSearch.SelectMyAppeals();
                    _appealSearch.ClearAll();
                    _appealSearch.GetSearchInputField(1).ShouldBeEqual("My Appeals", "Quick Search");

                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }

            }
        }



        /// <summary>
        /// Test navigation to appeal summary is affected by user authority and not by client setting
        /// </summary>
        /// don't stop the test in mid run as it changes client settings, which is critical
        [NonParallelizable]
        [Test, Category("OnDemand")] //us45655

        public void Verify_navigation_to_appealSearch_vs_security_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;

                var disallowAppealProcess = false;

                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeTrue("Find appeal section is present in new appeal search page");
                _appealSearch.IsSearchListSectionPresent()
                    .ShouldBeTrue("Search List  section is present in new appeal search page");

                var appealsActiveLabel = ProductAppealsEnum.AppealsActive.GetStringValue();
                var displayDocIDLabel = ProductAppealsEnum.DisplayExtDocIDField.GetStringValue();

                var _newClientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                try
                {
                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _newClientSearch.ClickOnRadioButtonByLabel(appealsActiveLabel, false);
                    _newClientSearch.IsDivByLabelPresent(displayDocIDLabel)
                        .ShouldBeFalse($"'{displayDocIDLabel}' field should not be present");
                    _newClientSearch.GetSideWindow.Save();

                    automatedBase.CurrentPage = _newClientSearch.NavigateToAppealSearch();

                    _appealSearch.GetPageHeader()
                        .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                    _appealSearch.IsFindAppealSectionPresent()
                        .ShouldBeTrue("Find appeal section is present in new appeal search page");
                    _appealSearch.IsSearchListSectionPresent()
                        .ShouldBeTrue("Search List  section is present in new appeal search page");


                    automatedBase.CurrentPage = _newClientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();

                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _newClientSearch.ClickOnRadioButtonByLabel(appealsActiveLabel);
                    _newClientSearch.IsDivByLabelPresent(displayDocIDLabel)
                        .ShouldBeTrue($"'{displayDocIDLabel}' field should be present");
                    _newClientSearch.GetSideWindow.Save();

                    disallowAppealProcess = true;
                }
                finally
                {
                    if (!disallowAppealProcess)
                    {
                        automatedBase.CurrentPage = _newClientSearch.NavigateToAppealSearch();

                        _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(
                            ClientEnum.SMTST.GetStringValue(),
                            ClientSettingsTabEnum.Product.GetStringValue());
                        _newClientSearch.ClickOnRadioButtonByLabel(appealsActiveLabel);
                        _newClientSearch.ClickOnRadioButtonByLabel(displayDocIDLabel);
                        _newClientSearch.GetSideWindow.Save();

                        automatedBase.CurrentPage = _newClientSearch.NavigateToAppealSearch();
                    }

                }
            }
        }

        [Test] //US45661
        [Order(8)]
        public void Verify_return_to_appeal_search_from_appeal_summary()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verifying return to appeal search form appeal summary through return to appeal search icon");
                    var searchResult = _appealSearch.GetAppealSearchResult();
                    string quickSearchFilter = _appealSearch.GetQuickSearchFilterValue();
                    string clientSelectFilter = _appealSearch.GetClientFilterValue();
                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Complete");
                    _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Navigated to appeal summary page");
                    automatedBase.CurrentPage = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.GetAppealSearchResult().ShouldCollectionBeEqual(searchResult,
                        "Search Result is retained when returning from appeal summary page");
                    _appealSearch.GetQuickSearchFilterValue().ShouldBeEqual(quickSearchFilter,
                        "Quick Search filter is retained equal to " + quickSearchFilter);
                    _appealSearch.GetClientFilterValue().ShouldBeEqual(clientSelectFilter,
                        "Quick Search filter is retained equal to " + clientSelectFilter);
                }
                finally
                {


                }
            }
        }

        [Test] //US58179
        [Order(2)]
        public void Verify_search_result_retain_when_returning_back_from_appeal_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
               // var TestName = new StackFrame(true).GetMethod().Name;
                try
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectOutstandingAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.ScrollToRow(16);
                    _appealSearch.SetDateFieldFrom(16, "01/1/2016", "Created Date");
                    _appealSearch.SetDateFieldTo(16, "05/31/2016", "Created Date");
                    _appealSearch.ClickOnFindButtonAndWait();
                    var records = _appealSearch.GetAppealSearchResult();
                    _appealSearch.ClickOnLoadMore();
                    var row = _appealSearch.GetAppealSearchResultRowCount();
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(row);
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.GetAppealSearchResult()
                        .ShouldCollectionBeEqual(records,
                            "Search Result should be previous searched result not after clicking load more");

                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }


            }
        }


        [Test] //US61999 + TE-887
        [Order(17)]
        public void Validation_of_advance_search_filters_on_appeal_search_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "product_type").Values.ToList();
                var expectedAppealLevelList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_level").Values.ToList();
                var expectedAppealTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var expectedAppealPriorityList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_priority").Values.ToList();
                var expectedStatusList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_status").Values.ToList();
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                List<string> _smtstPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _pehpPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.PEHP.ToString());
                List<string> _cvtyPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
                List<string> _ttreePlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.TTREE.GetStringDisplayValue());
                List<string> _lineOfBusinessList = _appealSearch.GetCommonSql.GetLineOfBusiness();
                try
                {
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();

                    _appealSearch.GetSideBarPanelSearch.IsAdvancedSearchFilterIconDispalyed()
                        .ShouldBeTrue("Advanced search filter icon displayed should be true.");
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);

                    StringFormatter.PrintMessageTitle("Verification of Product Drop down field");
                    ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Product", expectedProductTypeList);

                    StringFormatter.PrintMessageTitle(
                        "Verification of Visibility of Input Fields when Client is not selected");
                    _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                        .ShouldBeEqual("All", "Client value defaults to All");
                    _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Plan")
                        .ShouldBeFalse("On non Specific Client selection Plan visibility should be false");
                    _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Line Of Business")
                        .ShouldBeFalse("On Non Specific Client selection LOB visibility should be false");
                    _appealSearch.GetSideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                        .ShouldBeTrue(
                            "On Specific Client selection Provider Sequence's disabled state should be true?");
                    _appealSearch.GetSideBarPanelSearch
                        .GetInputAttributeValueByLabel("Provider Sequence", "placeholder")
                        .ShouldBeEqual("Please Select Client", " Provider Sequence ask to select client");


                    _appealSearch.SelectClientSmtst();
                    _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Plan")
                        .ShouldBeTrue("On Specific Client selection Plan is visible");
                    _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Line Of Business")
                        .ShouldBeTrue("On Specific Client selection LOB visibility should be true");
                    _appealSearch.GetSideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                        .ShouldBeFalse(
                            "On Specific Client selection Provider Sequence's disabled state should be false?");
                    _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                        .ShouldBeNullorEmpty("Provider Sequence value defaults to blank on client selection");
                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence",
                        paramLists["ProviderSeq"]);

                    StringFormatter.PrintMessageTitle("Plan field Validation");
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, "Plan", _smtstPlanList);

                    _appealSearch.SelectDropDownListbyInputLabel("Client", ClientEnum.PEHP.ToString(), false);
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, "Plan", _pehpPlanList);

                    _appealSearch.SelectDropDownListbyInputLabel("Client", ClientEnum.CVTY.GetStringDisplayValue());
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, "Plan", _cvtyPlanList);
                    _appealSearch.SelectDropDownListbyInputLabel("Client", ClientEnum.TTREE.ToString());
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, "Plan", _ttreePlanList);

                    StringFormatter.PrintMessageTitle("LOB field Validation");
                    ValidateFieldSupportingMultipleValues(_appealSearch, "Line Of Business", _lineOfBusinessList);

                    StringFormatter.PrintMessageTitle("Primary Reviewer field Validation");
                    ValidatePrimaryReviewerAndAssignedTo(_appealSearch, "Primary Reviewer");

                    StringFormatter.PrintMessageTitle("Assigned To field Validation");
                    ValidatePrimaryReviewerAndAssignedTo(_appealSearch, "Assigned To");

                    StringFormatter.PrintMessageTitle("Category Field field Validation");
                    ValidateCategoryField(_appealSearch, "Category", paramLists["Category"].Split(';'));

                    ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Type", expectedAppealTypeList, false);
                    ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Priority", expectedAppealPriorityList);

                    var appealLevelDropDownList =
                        _appealSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Appeal Level");
                    appealLevelDropDownList.ShouldCollectionBeEqual(expectedAppealLevelList,
                        "Appeal Level List is As Expected");
                    _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Appeal Level")
                        .ShouldBeEqual("All", "Appeal Level value defaults to All");


                    _appealSearch.SelectAllAppeals();
                    ValidateDateRangePickerBehaviour(_appealSearch, "Due Date");
                    ValidateDateRangePickerBehaviour(_appealSearch, "Create Date");
                    ValidateDateRangePickerBehaviour(_appealSearch, "Complete Date");

                    _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("First Deny Code")
                        .ShouldBeNullorEmpty("First Deny Code value defaults to blank");
                    _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("First Pay Code")
                        .ShouldBeNullorEmpty("First Pay Code value defaults to blank");

                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("First Deny Code", "0!0a");
                    _appealSearch.GetPageErrorMessage()
                        .ShouldBeEqual("Only alphanumerics allowed.",
                            "Popup Error message is shown when non alphanumeric character is passed to First Deny Code.");
                    _appealSearch.ClosePageError();

                    ValidateAlphaNumeric(_appealSearch, "First Pay Code", "Only alphanumerics allowed.", "0!2a");
                    ValidateAlphaNumeric(_appealSearch, "First Deny Code", "Only alphanumerics allowed.", "0!2a");

                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();

                    StringFormatter.PrintMessageTitle("Status Field  Validation");
                    ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Status", expectedStatusList, false);

                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence", paramLists["ClaimSeq"]);

                    var firstDeny = paramLists["FirstDeny"];
                    var firstPay = paramLists["FirstPay"];
                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("First Deny Code", firstDeny);
                    _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel("First Pay Code", firstPay);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.GetSearchResultByRowCol(1, 9).ShouldBeEqual(firstDeny);
                    _appealSearch.GetSearchResultByRowCol(1, 10).ShouldBeEqual(firstPay);

                    Console.WriteLine(
                        "Navigate to appeal action to confirm first deny and first pay vode value is the value of deny proc code and trigger code for the first appealed line.");
                    automatedBase.CurrentPage =
                        _appealAction =
                            _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction(
                                _appealSearch.GetSearchResultByRowCol(2, 3));
                    _appealAction.GetProcCode()
                        .ShouldBeEqual(firstDeny,
                            "Appeal manager search result yields appeals that have first appealed line with first deny value: " +
                            firstDeny + " as the deny proc code.");
                    _appealAction.GetTrigCode()
                        .ShouldBeEqual(firstPay,
                            "Appeal manager search result yields appeals that have first appealed line with first pay value: " +
                            firstPay + "  as the deny trigger code.");

                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                        _appealAction.ClickOkCancelOnConfirmationModal(true);

                }
                finally
                {
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser1().NavigateToAppealSearch();
                }
            }
        }

        [Test] //US61556
        [Order(10)]
        public void Validation_of_basic_search_filters_on_appeal_search_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                AppealSummaryPage _appealSummary;
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage =
                            _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;

                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                List<string> _assignedClientList = _appealSearch.GetAssignedClientList(automatedBase.EnvironmentManager.Username);
                _assignedClientList.Insert(0, "All");
                var filtersList = paramLists["FiltersList"].Split(';');
                var defaultFilterValue = paramLists["DefaultValue"].Split(';');
                var expectedClientSearchList = _assignedClientList;
                var client = ClientEnum.SMTST.ToString();
                var claseq = paramLists["ClaimSeq"];

                _appealSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(AppealQuickSearchTypeEnum.AllAppeals.GetStringValue(),
                        "Quick search option defaults to all appeals");

                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeTrue("Find appeal section is present in new appeal search page");
                _appealSearch.IsSearchListSectionPresent()
                    .ShouldBeTrue("Search List section is present in new appeal search page");
                _appealSearch.ClickOnSearchButton();
                _appealSearch.WaitToOpenCloseWorkList();
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeFalse("Find section should be closed when clicked on search icon first time");
                _appealSearch.ClickOnSearchButton();
                _appealSearch.WaitToOpenCloseWorkList(false);
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeTrue("Find section should be displayed when clicked on search icon second time");
                _appealSearch.IsDefaultEmptySearchResultPresent()
                    .ShouldBeTrue("A blank page is displayed in the Appeal Search page");

                _appealSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual("All Appeals", "Quick search option defaults to all appeals");
                ValdiateForDefaultValueOfBasicFilter(_appealSearch);

                _appealSearch.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue(), false);
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.FilterCountByLabel().ShouldBeEqual(filtersList.Length, "No filters are hidden");
                ValidateNoFiltersArePreApplied(_appealSearch, filtersList, defaultFilterValue);
                _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                _appealSearch.FilterCountByLabel()
                    .ShouldBeEqual(5, "Basic filters for All Record Reviews are shown");
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
                _appealSearch.FilterCountByLabel()
                    .ShouldBeEqual(5, "Basic filters for Outstanding Appeals are shown");
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.MyAppeals.GetStringValue());
                _appealSearch.FilterCountByLabel()
                    .ShouldBeEqual(6, "Basic filters for Outstanding Appeals are shown");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Priority")
                    .ShouldBeTrue("On Specific Client selection Priority is  visible. Is True?");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Type")
                    .ShouldBeTrue("On Specific Client selection Type is  visible. Is True?");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Category")
                    .ShouldBeTrue("On Specific Client selection Category is  visible. Is True?");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Due Date")
                    .ShouldBeTrue("On Specific Client selection Due Date is  visible. Is True?");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Plan")
                    .ShouldBeFalse("On Specific Client selection Plan is not visible. Is false?");
                _appealSearch.SelectClientSmtst();
                _appealSearch.FilterCountByLabel()
                    .ShouldBeEqual(7, "Basic filters for Outstanding Appeals are shown");
                _appealSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Plan")
                    .ShouldBeTrue("On Specific Client selection Plan is visible");


                _appealSearch.GetAvailableDropDownList("Client")
                    .ShouldCollectionBeEquivalent(expectedClientSearchList, "Client Search List As Expected");
                _appealSearch.SelectDropDownListbyInputLabel("Client", client);
                _appealSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual(client, "Client Search bears type-ahead functionality");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                    .ShouldBeFalse("On No Specific Client selection, Claim Sequence should not be disabled. Is False?");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("On No Specific Client selection, Claim No should not be disabled. Is False?");


                Verify_search_result_for_outstanding_appeals_of_quick_search(_appealSearch);

                ValidateAlphaNumeric(_appealSearch, "Claim No", "Only alphanumerics allowed.", "0!2a");
                _appealSearch.SetInputFieldByInputLabel("Claim No", paramLists["50CharacterClaimNo"]);
                _appealSearch.GetInputValueByLabel("Claim No").Count()
                    .ShouldBeEqual(50, "Claim No should not be greater than 50 character");
                _appealSearch.ClickOnClearLink();
                ValdiateForDefaultValueOfBasicFilter(_appealSearch);
                _appealSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual("Outstanding Appeals", "Quick search option has not been cleared");
                StringFormatter.PrintMessageTitle(
                    "Validate For claim sequence, search can be initiated without clasub");
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                _appealSearch.SelectDropDownListbyInputLabel("Client", client);
                _appealSearch.SetInputFieldByInputLabel("Claim Sequence", claseq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetAppealSearchResultRowCount().ShouldBeGreater(0,
                    "Claim sequence does not require exact value, claim seq without clasub can be used.");
                var appealSeq = _appealSearch.GetAppealSequenceByStatus("Closed");

                automatedBase.CurrentPage = _appealSummary =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Closed");
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Navigated to appeal summary page");
                automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.GetPreviouslyViewedAppealSeq()
                    .ShouldBeEqual(appealSeq, "Previously viewed Appeal Sequence is present.");
                _appealSearch.IsReturnToAppealSectionPresent()
                    .ShouldBeTrue("return to previously viewed appeal section is present");

                automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnReturnToAppealToGoAppealSummaryPage();
                _appealSummary.GetPageHeader()
                    .ShouldBeEqual("Appeal Summary",
                        "Navigated to appeal summary page when click on Return to Appeal Link");
                automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();


                automatedBase.CurrentPage = _appealAction =
                    _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Action", "Navigated to appeal action page");
                automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                appealSeq = _appealSearch.GetAppealSequenceByStatus();
                _appealSearch.GetPreviouslyViewedAppealSeq()
                    .ShouldBeEqual(appealSeq, "Previously viewed Appeal Sequence is present.");
                _appealSearch.IsReturnToAppealSectionPresent()
                    .ShouldBeTrue("Return to previously viewed appeal section is present");

                automatedBase.CurrentPage = _appealAction = _appealSearch.ClickOnReturnToAppealToGoAppealActionPage();
                _appealSummary.GetPageHeader()
                    .ShouldBeEqual("Appeal Action",
                        "Navigated to appeal action page when click on Return to Appeal Link");
                automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                _appealSearch.ClickOnClearLink();
                _appealSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(AppealQuickSearchTypeEnum.AllAppeals.GetStringValue(),
                        "Quick search option has not been cleared");
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search", "All Appeals");
                _appealSearch.SetInputFieldByInputLabel("Appeal Sequence", appealSeq + "123");
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No matching record found Message");

                _appealSearch.FindByAppealSequenceToNavigateAppealAction(appealSeq);
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Action",
                    "Navigated to appeal action page, i.e. Appeal sequence requires exact value");
                _appealAction.HandleAutomaticallyOpenedActionPopup();
                automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.ClickOnClearLink();

            }
        }

        [Test] //TE-279
        [Order(12)]
        public void Verify_that_users_are_allowed_to_search_by_product_and_status_for_all_appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                try
                {
                    _appealSearch.RefreshPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectSearchDropDownListValue("Client", "All");
                    _appealSearch.SelectSearchDropDownListValue("Product", ProductEnum.DCA.GetStringValue());
                    _appealSearch.SelectSearchDropDownListValue("Status",
                        AppealStatusEnum.Open.GetStringDisplayValue());
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "User should be allowed to search for appeals when any product and any status other than Closed is selected");
                    _appealSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search should return data");

                    StringFormatter.PrintMessage(
                        "Validate Closed appeals can't be search with All Appeals and Product only");
                    _appealSearch.SelectSearchDropDownListValue("Status",
                        AppealStatusEnum.Closed.GetStringDisplayValue(),
                        8);
                    _appealSearch.GetInvalidInputToolTipByLabel("Status")
                        .ShouldBeEqual(
                            "Search cannot be initiated for all appeals in a status of Closed. Please select a different Status, a Date Range, Reviewer or Provider Seq.");
                    _appealSearch.GetInvalidInputToolTipByLabel("Product")
                        .ShouldBeEqual(
                            "Search cannot be initiated with Product only. A date range, Appeal Seq, Claim Seq, Appeal #, Provider Seq, or Reviewer search criteria is required.");
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.GetPageErrorMessage()
                        .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                            "Popup Message  when attempting to search when exclamation icon present");
                    _appealSearch.ClosePageError();
                    _appealSearch.ClearAll();


                    StringFormatter.PrintMessage(
                        "Validate  appeals can't be search with All Appeals, Product and any other search criteria other than none Closed status appeal");
                    _appealSearch.SelectSearchDropDownListValue("Product", ProductEnum.DCA.GetStringValue());
                    ValidateFieldErrorMessageForComboBox(_appealSearch, "Client", 3, ClientEnum.SMTST.ToString(),
                        "Search cannot be initiated with Client only. A date range, Status, Appeal Seq, Claim Seq, Appeal #, Provider Seq or Reviewer is required.");

                }
                finally
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                }

            }
        }


        [Test] //CAR-728
        [Order(4)]
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search", "Outstanding Appeals");
                _appealSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _appealSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find button should be enabled once the results are displayed");

            }
        }

        [Test] //TE-749
        [Retry(3)]
        [Order(16)]
        public void Verify_Outstanding_DCI_Appeals_Quick_Search_Filter_In_Appeal_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;

                automatedBase.CurrentPage =
                    _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                List<string> _assignedClientList = _appealSearch.GetAssignedClientList(automatedBase.EnvironmentManager.Username);
                _assignedClientList.Insert(0, "All");
                List<string> _smtstPlanList = _appealSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                var basicSearchFilterList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "basic_filters_outstanding_dci_appeals").Values.ToList();
                var expectedAppealLevelList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_level").Values.ToList();
                var expectedAppealTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var expectedAppealPriorityList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Appeal_priority")
                    .Values.ToList();
                var expectedClientSearchList = _assignedClientList;
                var advancedFiltersList = paramLists["FiltersList"].Split(';');
                var defaultFilterValue = paramLists["DefaultValue"].Split(';');
                var client = ClientEnum.SMTST.ToString();
                var outstandingDCIAppealsFromDb =
                    _appealSearch.GetOutstandingDCIAppealsFromDb(automatedBase.EnvironmentManager.HciAdminUsername1);
                var lineOfBusinessList = _appealSearch.GetLineOfBusinessFromDb();

                _appealSearch.RefreshPage();
                StringFormatter.PrintMessage("Verify Basic Search Filter For Outstanding DCA Appeals");
                _appealSearch.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue());
                _appealSearch.FilterCountByLabel()
                    .ShouldBeEqual(5, "Basic filters for Outstanding DCA Appeals are shown");
                _appealSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEquivalent(basicSearchFilterList, "Search Filters", true);
                ValdiateForDefaultValueOfBasicFilter(_appealSearch);
                _appealSearch.GetAvailableDropDownList("Client")
                    .ShouldCollectionBeEqual(expectedClientSearchList, "Client Search List As Expected");
                _appealSearch.SelectDropDownListbyInputLabel("Client", client);
                _appealSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual(client, "Client Search bears type-ahead functionality");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                    .ShouldBeFalse("On No Specific Client selection, Claim Sequence should not be disabled. Is False?");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("On No Specific Client selection, Claim No should not be disabled. Is False?");
                ValidateAlphaNumeric(_appealSearch, "Claim No", "Only alphanumerics allowed.", "0!2a");
                _appealSearch.SetInputFieldByInputLabel("Claim No", paramLists["50CharacterClaimNo"]);
                _appealSearch.GetInputValueByLabel("Claim No").Count()
                    .ShouldBeEqual(50, "Claim No should not be greater than 50 character");
                _appealSearch.ClickOnClearLink();
                _appealSearch.WaitForStaticTime(200);
                ValdiateForDefaultValueOfBasicFilter(_appealSearch);
                _appealSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue(),
                        "Quick search option has not been cleared");


                StringFormatter.PrintMessage("Verify Advanced Search Filter For Outstanding DCA Appeals");
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.FilterCountByLabel().ShouldBeEqual(advancedFiltersList.Length, "No filters are hidden");
                _appealSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(advancedFiltersList, "Search Filters", true);
                ValidateNoFiltersArePreApplied(_appealSearch, advancedFiltersList, defaultFilterValue);

                StringFormatter.PrintMessageTitle(
                    "Verification of Visibility of Input Fields when Client is not selected");
                _appealSearch.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client value defaults to All");
                _appealSearch.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeFalse("On non Specific Client selection, Plan visibility should be false");
                _appealSearch.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeFalse("On Non Specific Client selection, LOB visibility should be false");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeTrue("On Specific Client selection, Provider Sequence's disabled state should be true?");
                _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Provider Sequence", "placeholder")
                    .ShouldBeEqual("Please Select Client", " Provider Sequence ask to select client");

                _appealSearch.SelectSMTST();
                _appealSearch.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeTrue("On Specific Client selection,Plan is visible");
                _appealSearch.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeTrue("On Specific Client selection, LOB visibility should be true");
                _appealSearch.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeFalse("On Specific Client selection, Provider Sequence's disabled state should be false?");
                _appealSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence value defaults to blank on client selection");


                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, "Plan", _smtstPlanList);

                StringFormatter.PrintMessageTitle("LOB field Validation");
                ValidateFieldSupportingMultipleValues(_appealSearch, "Line Of Business", lineOfBusinessList);

                StringFormatter.PrintMessageTitle("Primary Reviewer field Validation");
                ValidatePrimaryReviewerAndAssignedTo(_appealSearch, "Primary Reviewer");

                StringFormatter.PrintMessageTitle("Assigned To field Validation");
                ValidatePrimaryReviewerAndAssignedTo(_appealSearch, "Assigned To");

                StringFormatter.PrintMessageTitle("Category Field Validation");
                ValidateCategoryField(_appealSearch, "Category", paramLists["Category"].Split(';'));

                ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Type", expectedAppealTypeList, false);
                ValidateSingleDropDownForDefaultValueAndExpectedList(_appealSearch, "Priority", expectedAppealPriorityList);

                var appealLevelDropDownList =
                    _appealSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Appeal Level");
                appealLevelDropDownList.ShouldCollectionBeEqual(expectedAppealLevelList,
                    "Appeal Level List is As Expected");
                _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Appeal Level")
                    .ShouldBeEqual("All", "Appeal Level value defaults to All");


                StringFormatter.PrintMessageTitle("Validate Date Field");
                ValidateDateRangePickerBehaviour(_appealSearch, "Due Date");
                ValidateDateRangePickerBehaviour(_appealSearch, "Create Date");
                ValidateDateRangePickerBehaviour(_appealSearch, "Complete Date");


                ValidateAlphaNumeric(_appealSearch, "First Pay Code", "Only alphanumerics allowed.", "0!2a");
                ValidateAlphaNumeric(_appealSearch, "First Deny Code", "Only alphanumerics allowed.", "0!2a");



                StringFormatter.PrintMessage("Verify Results For Outstanding Appeals");
                _appealSearch.ClickOnClearLink();
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                var nonOutstandingStatus = new List<String>
                {
                    AppealStatusEnum.Closed.GetStringValue(),
                    AppealStatusEnum.Complete.GetStringValue(),
                    AppealStatusEnum.DocumentsWaiting.GetStringValue(),
                    AppealStatusEnum.Open.GetStringValue(),
                    AppealStatusEnum.None.GetStringValue()
                };
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");
                _appealSearch.GetSearchResultListByCol(11)
                    .Distinct().ToList().ShouldCollectionBeNotEqual(nonOutstandingStatus,
                        "Not OutStanding Status should not display");
                _appealSearch.IsSearchInputFieldDispalyedByLabel("Status")
                    .ShouldBeFalse("Is Status filter displayed for Outstanding DCA Appeals?");
                _appealSearch.GetSearchResultListByCol(3)
                    .ShouldCollectionBeEquivalent(outstandingDCIAppealsFromDb, "Data Should Match");

            }
        }

        [Test]//TE-887
        [Order(5)]
        public void Verify_MentorAppeal_Opens_In_Appeal_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;


                automatedBase.CurrentPage =
                        _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var statusList = paramLists["AppealStatus"].Split(',').ToList();
                _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Appeals");
                _appealSearch.SelectSMTST();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.MentorReview.GetStringDisplayValue());
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetAppealSearchResultRowCount().ShouldBeGreater(0,
                    "Appeal with mentor review status should be populated in the grid");
                _appealSearch.GetGridViewSection.GetValueInGridByColRow(11)
                    .ShouldBeEqual("Mentor", "correct appeal status displayed in primary data?");
                _appealAction = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction("Mentor");
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue(),
                    $"Is {PageHeaderEnum.AppealAction.GetStringValue()} page open?");
                _appealAction.GetAppealStatus().ShouldBeEqual(AppealStatusEnum.MentorReview.GetStringDisplayValue(),
                    "Is Correct Status display?");
                _appealAction.ClickOnEditIcon();
                _appealAction.GetEditAppealInputListByLabel("Status").ShouldBeEqual(statusList, "Appeal Status Equal?");
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }


        [Test] //TE-926
        [Order(9)]
        [Retry(3)]
        public void Verify_Results_For_Advanced_Filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 1))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;



                automatedBase.CurrentPage =
                            _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var appealSeq = paramLists["Appealseq"];
                _appealSearch.UpdateAppealStatusToNew(paramLists["Claseq"]);
                _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                _appealSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Primary Reviewer",
                    paramLists["Analysts"].Split(',').ToList());
                _appealSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Assigned To",
                    paramLists["Analysts"].Split(',').ToList());
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.New.ToString());
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Type", paramLists["Appealtype"]);
                _appealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _appealSearch.WaitForWorking();
                _appealSearch.GetGridViewSection.ClickLoadMore();
                var appealresult = _appealSearch.GetAppealsUsingAnalysts(paramLists["Userid"].Split(',')[0],
                    paramLists["Userid"].Split(',')[1], automatedBase.EnvironmentManager.Username);
                _appealSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEquivalent(appealresult, "appeal result correct?");
                _appealAction =
                    _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByAppealSequence(appealSeq);
                _appealAction.CompleteAppeals(isCompleted: true, isAction: false);
                var nextAppealInworklist = _appealAction.GetAppealSequence();
                nextAppealInworklist.ShouldNotBeEqual(appealSeq, "New Appeal in worklist should get navigated");
                appealresult.ShouldContain(nextAppealInworklist, "Is Correct Appeal in the worklist displayed?");
                _appealSearch.CloseAnyPopupIfExist();
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }



        #endregion

        #region PRIVATE METHODS

        private void ValidateAppealSearchRowSorted(AppealSearchPage _appealSearch, int col, int sortOptionRow, string colName)
        {
            if (colName == "Appeal Seq")
            {
                _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                _appealSearch.IsListIntSortedInAscendingOrder(col)
                    .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                _appealSearch.IsListIntSortedInDescendingOrder(col)
                    .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
            }
            else
            {
                _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                _appealSearch.IsListStringSortedInAscendingOrder(col)
                    .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                _appealSearch.IsListStringSortedInDescendingOrder(col)
                    .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
            }

        }

        private void ValidateFieldErrorMessageForComboBox(AppealSearchPage _appealSearch, string fieldName, int row, string value, string message)
        {
            _appealSearch.SelectSearchDropDownListValue(fieldName, value, row);
            _appealSearch.IsInvalidInputPresentByLabel(fieldName).ShouldBeTrue($"'{fieldName}' should be surrounded by red highlight");

            _appealSearch.GetInvalidInputToolTipByLabel(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> is selected only", fieldName));
            _appealSearch.ClickOnFindButton();
            _appealSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealSearch.ClosePageError();
            _appealSearch.ClearAll();
        }

        private void ValidateFieldErrorMessageForTextField(AppealSearchPage _appealSearch, string fieldName, int row, string value, string message)
        {
            _appealSearch.SetSearchInputField(fieldName, value, row);

            _appealSearch.GetInvalidInputToolTipByLabel(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> is set", fieldName));
            _appealSearch.ClickOnFindButton();
            _appealSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealSearch.ClosePageError();
            _appealSearch.ClearAll();
        }

        private void ValidateFieldErrorMessageForDateRange(AppealSearchPage _appealSearch, string fieldName, int row, string message)
        {
            _appealSearch.SetDateFieldFrom(row, DateTime.Now.ToString("MM/d/yyyy"), fieldName);
            _appealSearch.SetDateFieldTo(row, DateTime.Now.AddMonths(3).AddDays(1).ToString("MM/d/yyyy"), fieldName);
            _appealSearch.GetInvalidInputToolTipByLabel(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", fieldName));
            _appealSearch.ClickOnFindButton();
            _appealSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealSearch.ClosePageError();
            _appealSearch.ClearAll();
        }


        //<First Name> <Last Name>
        private void VerifyThatNameIsInCorrectFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
        }

        //<First Name> <Last Name> (<username>)
        private void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
        }

        private void VerifyThatDateIsInCorrectFormat(string date, string message)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format MM/DD/YYYY");
        }

        private void ValidateFieldSupportingMultipleValues(AppealSearchPage _appealSearch, string label, IList<string> expectedDropDownList)
        {
            ValidateMultipleDropDownForDefaultValueAndExpectedList(_appealSearch, label, expectedDropDownList);
            _appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedDropDownList[0]);
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedDropDownList[0], label + "  single value selected");
            _appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedDropDownList[1]);
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "  multiple value selected");
        }
        private void ValidateCategoryField(AppealSearchPage _appealSearch, string label, IList<string> selectValue)
        {
            var actualDropDownList = _appealSearch.GetAvailableDropDownList(label, false);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");

            _appealSearch.IsListInAscendingOrder(actualDropDownList).ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
               .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
            _appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label,
               selectValue[0]);
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(selectValue[0], label + "  single value selected");
            _appealSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label,
                selectValue[1]);
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "  multiple value selected");
        }

        private void ValidatePrimaryReviewerAndAssignedTo(AppealSearchPage _appealSearch, string label)
        {
            List<string> _appealAssignableUserList = _appealSearch.GetPrimaryReviewerAssignedToList();
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");

            var reqDropDownList = _appealSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);

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
            /*-----------Since sorting is based on name rather than on name+ user id hence use substring to separate username only to check sorting order-,.,-------------*/
            var searchList = reqDropDownList.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList();
            searchList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            searchList.ShouldCollectionBeEqual(_appealAssignableUserList, label + " List As Expected");



        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(AppealSearchPage _appealSearch, string label, IList<string> collectionToEqual, bool order = true)
        {
            var actualDropDownList = _appealSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _appealSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
        }

        private void ValidateMultipleDropDownForDefaultValueAndExpectedList(AppealSearchPage _appealSearch, string label, IList<string> collectionToEqual)
        {
            var listedOptionsList = _appealSearch.GetAvailableDropDownList(label, false);
            listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            _appealSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                .ShouldBeTrue(
                    "A value of all displayed at the top of the list, followed by options sorted alphabetically.");
            listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            listedOptionsList.Remove("All");
            listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _appealSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
        }



        private void ValidateDateRangePickerBehaviour(AppealSearchPage _appealSearch, string label)
        {
            _appealSearch.GetSideBarPanelSearch.GetDateFieldPlaceholder(label, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (from) default value");
            _appealSearch.GetSideBarPanelSearch.GetDateFieldPlaceholder(label, 2).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (to) default value");
            _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, DateTime.Now.ToString("MM/d/yyyy")); //check numeric value can be typed
            _appealSearch.GetSideBarPanelSearch.GetDateFieldFrom(label).ShouldBeEqual(DateTime.Now.ToString("MM/d/yyyy"), label + " Checks numeric value is accepted");
            _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, "");
            _appealSearch.GetSideBarPanelSearch.SetDateFieldFrom(label, DateTime.Now.ToString("MM/d/yyyy"));
            _appealSearch.GetSideBarPanelSearch.GetDateFieldTo(label).ShouldBeEqual(_appealSearch.GetSideBarPanelSearch.GetDateFieldFrom(label), label + " From value populated in To field as well.");
            _appealSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddMonths(3).AddDays(2).ToString("MM/d/yyyy"));
        }
        private void ValidateAlphaNumeric(AppealSearchPage _appealSearch, string label, string message, string character)
        {
            _appealSearch.GetSideBarPanelSearch.SetInputFieldByLabel(label, character);

            _appealSearch.GetPageErrorMessage()
                .ShouldBeEqual(message,
                    "Popup Error message is shown when unexpected " + character + "is passed to " + label);
            _appealSearch.ClosePageError();
        }
        private void ValidateNoFiltersArePreApplied(AppealSearchPage _appealSearch, string[] filtersList, string[] defaultFilterValue)
        {
            if (_appealSearch.IsAdvancedSearchFilterIconDispalyed()) _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
            for (var i = 1; i < filtersList.Length; i++)
            {
                _appealSearch.GetInputValueByLabel(filtersList[i]).ShouldBeEqual(defaultFilterValue[i]);
            }

        }

        private void ValdiateForDefaultValueOfBasicFilter(AppealSearchPage _appealSearch)
        {
            StringFormatter.PrintMessageTitle("Validate For default behaviour of basic filter");
            _appealSearch.GetInputValueByLabel("Appeal Sequence")
                .ShouldBeNullorEmpty("Appeal Sequence value defaults to blank ");
            _appealSearch.GetInputValueByLabel("Claim Sequence")
                .ShouldBeNullorEmpty("Claim Sequence value defaults to blank ");
            _appealSearch.GetInputValueByLabel("Claim No")
                .ShouldBeNullorEmpty("Claim No value defaults to blank ");

            _appealSearch.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client Search default filter equals All");

            _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                .ShouldBeTrue("On No Specific Client selection, Claim Sequence should be disabled. Is True?");
            _appealSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                .ShouldBeTrue("On No Specific Client selection, Claim No should be disabled. Is True?");
        }


        private void Verify_search_result_for_outstanding_appeals_of_quick_search(AppealSearchPage _appealSearch)
        {
            _appealSearch.SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
            _appealSearch.SelectSMTST();
            _appealSearch.ClickOnFindButton();
            _appealSearch.WaitForWorking();

            var list = _appealSearch.GetSearchResultListByCol(11)
                      .Distinct().ToList();


            var nonOutstandingStatus = new List<String>
                    {
                        AppealStatusEnum.Closed.GetStringValue(),
                        AppealStatusEnum.Complete.GetStringValue(),
                        AppealStatusEnum.DocumentsWaiting.GetStringValue(),
                        AppealStatusEnum.Open.GetStringValue(),
                        AppealStatusEnum.None.GetStringValue()
                    };

            list.ShouldCollectionBeNotEqual(nonOutstandingStatus,
                    "Not OutStanding Status should not display");
            _appealSearch.IsSearchInputFieldDispalyedByLabel("Status").ShouldBeFalse("Status display for " + AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());

        }

        private List<string> ReadLetterAndParseIntoList(string letter)
        {
            var appealLetterList = new List<string>();
            using (var reader = new System.IO.StringReader(letter))
            {
                string lin;
                while ((lin = reader.ReadLine()) != null)
                {
                    appealLetterList.Add(lin);
                }
                appealLetterList = appealLetterList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }

            return appealLetterList;
        }

        private List<string> OpenLetterAndParseIntoList(AppealSearchPage _appealSearch, int appealLevel)
        {
            var appealLetterWithOutIro = _appealSearch.ClickOnApealLetterIconByAppealLevel(appealLevel);
            var letter = appealLetterWithOutIro.GetAppealLetterClosing();
            _appealSearch = appealLetterWithOutIro.CloseLetterPopUpPageAndBackToAppealSearch();
            return ReadLetterAndParseIntoList(letter);
        }

        private void ValidateIroDetail(string closingLetter, List<string> stateIroList, bool show = true)
        {
            stateIroList.RemoveAt(0);
            stateIroList.RemoveAt(0);
            if (!show)
            {
                foreach (var iroValue in stateIroList)
                {
                    closingLetter.AssertIsNotContained(iroValue, "Iro Value Should not display in Closing Letter");
                }
            }
            else
            {
                foreach (var iroValue in stateIroList)
                {
                    closingLetter.AssertIsContained(iroValue, "Iro Value Should display in Closing Letter");
                }
            }
        }
        private void ValidatePDFContents(string fileName, string appealLetterContent)
        {
            //please update the loctation Example: replace uiautomation to iNumber
            using (var reader = new PdfReader("C:/Users/uiautomation/Downloads/" + fileName))
            {
                ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                var actual = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);

                var index = actual.IndexOf("©");
                if (index > 0)
                    actual = actual.Substring(0, index);

                var expected = appealLetterContent;
                RemoveSpecialCharacters(actual).ShouldBeEqual(RemoveSpecialCharacters(expected), "Is PDF Content Expected?");
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        #endregion

    }
}
