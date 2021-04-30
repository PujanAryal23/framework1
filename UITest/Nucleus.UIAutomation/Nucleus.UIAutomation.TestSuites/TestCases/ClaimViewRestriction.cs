using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class ClaimViewRestriction : NewAutomatedBase
    {
        private ClaimSearchPage _claimSearch;
        private ProfileManagerPage _userProfileManager;
        private AppealCategoryManagerPage _appealCategoryManager;
        private AppealCreatorPage _appealCreator;
        private AppealManagerPage _appealManager;
        private AppealSearchPage _appealSearch;
        private AppealSummaryPage _appealSummary;
        private AppealActionPage _appealAction;
        private ClaimActionPage _claimAction;
        


        #region OVERRIDE METHODS

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                UserLoginIndex = 99;
                base.ClassInit();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCICLAIMVIEWRESTRICTION,
                    StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = QuickLaunch.Logout().LoginAsClaimViewRestrictionUser();
                CheckTestClientAndSwitch();
            }
            //else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.QuickLaunch.GetStringValue()))
            //{
            //    CurrentPage.ClickOnQuickLaunch();
            //}
            CurrentPage.ClickOnLogo();
        }

        #endregion

        #region TEST SUITES

        [Test, Category("ClaimViewRestriction1")] //CAR-1926 (CAR-2000) + CAR-2605(CAR-2695)
        public void Verify_restriction_notification_to_show_correctly_based_on_user_type()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var claSeqRestrictedForInternalUserOnly = testData["ClaSeqRestrictedForInternalUserOnly"];
            var claSeqRestrictedForClientUserOnly = testData["ClaSeqRestrictedForClientUserOnly"];
            var claSeqRestrictedForBothUsers = testData["ClaSeqRestrictedForBothUsers"];
            var claSeqWithNoRestriction = testData["ClaSeqWithNoRestriction"];
            var claSeqRestrictedForInternalAndBoth = testData["ClaSeqRestrictedForInternalAndBoth"];

            
            CurrentPage = _claimSearch = CurrentPage.Logout().LoginAsHciAdminUser().NavigateToClaimSearch();

            StringFormatter.PrintMessageTitle("Verification of the restriction notification for internal user");

            StringFormatter.PrintMessage("Verification of the restriction notification for claim which is restricted to internal user only");
            VerifyRestrictionWarningNotification(claSeqRestrictedForInternalUserOnly, RestrictionGroup.InternalUser.GetStringValue());
         
            StringFormatter.PrintMessage("Verification of the restriction notification for claim which is restricted to client user only");
            VerifyRestrictionWarningNotification(claSeqRestrictedForClientUserOnly, RestrictionGroup.ClientUser.GetStringValue(),
                false, "Warning Message should not show for claims restricted to client users");

            StringFormatter.PrintMessage("Verification of the restriction notification for claim which is restricted to both internal and client users");
            VerifyRestrictionWarningNotification(claSeqRestrictedForBothUsers, RestrictionGroup.AllUser.GetStringValue());

            StringFormatter.PrintMessage("Verification of the restriction notification for claim which is restricted to both internal and both user type and comma separated access in the tooltip");
            VerifyRestrictionWarningNotification(claSeqRestrictedForInternalAndBoth, $"{RestrictionGroup.AllUser.GetStringValue()}, {RestrictionGroup.InternalUser.GetStringValue()}");

            StringFormatter.PrintMessage("Verification of the restriction notification for unrestricted claim");
            VerifyRestrictionWarningNotification(claSeqWithNoRestriction, null, false, "Warning Message should not show for unrestricted claims");
        }


        [Test, Category("ClaimViewRestriction1")] //US68749+US68752+US68753
        public void Verify_claim_should_not_display_for_restricted_claim_for_restricted_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence", "Value");
            var status =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Status", "Value");
            var patientSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "patientSeq", "Value");
            var beginDos =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "beginDos", "Value");
            var endDos =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "endDos", "Value");
            var source =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "source", "Value");
            var prvSeq =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "prvSeq", "Value");
            var range = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "range",
                "Value");

            CurrentPage = _claimSearch = CurrentPage.NavigateToClaimSearch();
            _claimSearch.WaitForWorking();
            _claimSearch.WaitForStaticTime(5000);
            _claimSearch.GetCommonSql
                .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence, EnvironmentManager.Username)
                .ShouldBeFalse(string.Format("{0} is in restricted group", claimSequence));

            var url = string.Format("app/#/clients/{0}/claims/{1}/{2}/lines/claim_lines?claimSearchType=findClaims",
                ClientEnum.SMTST, claimSequence.Split('-')[0], claimSequence.Split('-')[1]);
            var urlPatHist = string.Format(
                @"clients/{0}/claimhistory?Client={0}&claSeq={1}&claSub={2}&status={3}&patientSeq={4}&beginDos={5}&endDos={6}&source={7}prvSeq={8}&range={9}"
                , ClientEnum.SMTST
                , claimSequence.Split('-')[0]
                , claimSequence.Split('-')[1]
                , status
                , patientSeq
                , beginDos
                , endDos
                , source
                , prvSeq
                , range);
            try
            {
                var currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url);
                VerifyIfUrlIsUnauthorized(currentUrl);
                CurrentPage = _claimSearch = _claimSearch.ClickOnReturnToLastPage<ClaimSearchPage>();
                _claimSearch.SetClaimSequenceInFindClaimSection(claimSequence);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal?");
                _claimSearch.ClosePageError();
                currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(urlPatHist);
                VerifyIfUrlIsUnauthorized(currentUrl);
            }
            finally
            {
                try
                {
                    _claimSearch = _claimSearch.ClickOnReturnToLastPage<ClaimSearchPage>();
                }
                finally
                {
                    QuickLaunch =
                        CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<QuickLaunchPage>(PageUrlEnum.QuickLaunch
                            .GetStringValue());
                }
            }
        }


        [Test, Category("ClaimViewRestriction1")]//CAR-506,CAR-495,CAR-417,CAR-855
        public void Verify_visibility_of_claim_accordingly_to_claim_view_restriction_logic()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var claimSequence = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value")
                .Split(',').ToList();
            var claimNumber = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimNumber", "Value")
                .Split(',').ToList();
            var status = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Status", "Value").Split(',').ToList();
            var patientSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "patientSeq", "Value").Split(',').ToList();
            var beginDos = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "beginDos", "Value").Split(',').ToList();
            var endDos = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "endDos", "Value").Split(',').ToList();
            var source = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "source", "Value");
            var prvSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "prvSeq", "Value").Split(',').ToList();
            var range = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "range", "Value");
            CurrentPage = _claimSearch = CurrentPage.NavigateToClaimSearch();
            var url = new List<string>();
            var patientHistoryurl = new List<string>();
            
            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Client User type but should be visible to internal User");
            _claimSearch.GetCommonSql
                .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence[0],
                    EnvironmentManager.ClientUserWithClaimViewRestriction)
                .ShouldBeFalse(string.Format("{0} is in restricted group for client", claimSequence[0]));

            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Internal User type and should not visible to internal User");
            _claimSearch.GetCommonSql
                .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence[1],
                    EnvironmentManager.HciClaimViewRestrictionUsername)
                .ShouldBeFalse(string.Format("{0} is in restricted group for Internal", claimSequence[1]));

            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Both User type and should not visble to restricted internal User");
            _claimSearch.GetCommonSql
                .IsClaimAllowByClaimSequenceAndUserId(claimSequence[2],
                    EnvironmentManager.HciClaimViewRestrictionUsername)
                .ShouldBeFalse(string.Format("{0} is in restricted group for both", claimSequence[2]));

            foreach (var claSeq in claimSequence)
            {
                var x = string.Format("app/#/clients/{0}/claims/{1}/{2}/lines/claim_lines?claimSearchType=findClaims",
                    ClientEnum.SMTST, claSeq.Split('-')[0], claSeq.Split('-')[1]);
                url.Add(x);
            }
            var i = 0;
            while (i < 2)
            {
                var x = string.Format(@"clients/{0}/claimhistory?Client={0}&claSeq={1}&claSub={2}&status={3}&patientSeq={4}&beginDos={5}&endDos={6}&source={7}&prvSeq={8}&range={9}"
                                           , ClientEnum.SMTST
                                           , claimSequence[i + 1].Split('-')[0]
                                           , claimSequence[i + 1].Split('-')[1]
                                           , status[i]
                                           , patientSeq[i]
                                           , beginDos[i]
                                           , endDos[i]
                                           , source
                                           , prvSeq[i]
                                           , range);
                patientHistoryurl.Add(x);
                i++;

            }

            try
            {
                var currentPage = _claimAction = _claimSearch.VisitAndReturnPageByUrlFoAuthorizedPage<ClaimActionPage>(url[0]);
                currentPage.SwitchToSecondHandlePage();
                var urlPatHist = currentPage.GetCurrentUrl().Replace(EnvironmentManager.ApplicationUrl,"");
                currentPage.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence[0], "Is Claim Action Page Open?");
                var claimDetailPatientSeq = currentPage.GetPatientSeq();
                _claimAction = currentPage.SwitchBackToNewClaimActionPage();

                _claimAction.VisitAndReturnPageByUrlFoAuthorizedPage<ClaimHistoryPage>(urlPatHist); /* CAR 417 */
                currentPage.GetPatientHistoryPatientSeq().ShouldBeEqual(claimDetailPatientSeq, "Patient seq should be equal");

                var currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url[1]);
                VerifyIfUrlIsUnauthorized(currentUrl);
                               
                var patHistoryPage = _claimAction.VisitAndGetUrlByUrlForUnAuthorizedPage(patientHistoryurl[0]);/* CAR 417 */
                VerifyIfUrlIsUnauthorized(patHistoryPage);


                currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url[2]);
                VerifyIfUrlIsUnauthorized(currentUrl);

                patHistoryPage = _claimAction.VisitAndGetUrlByUrlForUnAuthorizedPage(patientHistoryurl[1]);/* CAR 417 */
                VerifyIfUrlIsUnauthorized(patHistoryPage);

                _claimAction.SwitchBack();
                currentPage.NavigateToClaimSearch();
                _claimSearch.SearchByClaimSequence(claimSequence[0]);
                currentPage.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence[0], "Is Claim Action Page Open?");
                currentPage.ClickClaimSearchIcon();
                _claimSearch.SetClaimSequenceInFindClaimSection(claimSequence[1]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal for Search by Claim Sequence?");
                _claimSearch.ClosePageError();
                //car 855 starts here...
                _claimSearch.SetClaimNoInFindClaimSection(claimNumber[0]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal for Search by Claim Number?");
                _claimSearch.ClosePageError();

                _claimSearch.SetOtherClaimNoInFindClaimSection(claimNumber[1]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal for Searchh by Other Claim Number?");
                _claimSearch.ClosePageError();

                _claimSearch.SetClaimSequenceInFindClaimSection(claimSequence[2]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.AllUser.GetStringValue()),
                    "Is Poppup Message Equal?");
                _claimSearch.ClosePageError();
            }
            finally
            {
                if (IsUrlUnauthorized(CurrentPage.CurrentPageUrl))
                {
                    QuickLaunch =
                        CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<QuickLaunchPage>(PageUrlEnum.QuickLaunch
                            .GetStringValue());
                }

                //else CurrentPage = CurrentPage.ClickOnQuickLaunch();
            }
        }


        [Test, Category("ClaimViewRestriction2")] //US68751 ; CAR-603 + CAR-588
        public void Validate_a_direct_URL_access_to_appeal_pages_not_allowed_for_restricted_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var appealSequenceWithInternalRestrictionsForAppealSummary = testData["AppealSequenceWithInternalRestrictionsForAppealSummary"].Split(';');
            var appealSequenceWithInternalRestrictionsForAppealAction = testData["AppealSequenceWithInternalRestrictionsForAppealAction"].Split(';');
            var appealSequenceWithClientRestrictions = testData["AppealSequenceWithClientRestrictions"].Split(';');
            var restrictionTypeList = new List<string> { "1", "2", "8" };

            var urlListForAppealsWithInternallyRestrictedClaims = new List<string>();
            var urlListForAppealsWithClientRestrictedClaims = new List<string>();
            var urlForClientUser=new List<string>();

            try
            {
                StringFormatter.PrintMessageTitle("Opening the Appeal Action and Summary pages for internal and client restricted claims by HCIADMIN");

                CurrentPage = QuickLaunch.Logout().LoginAsHciAdminUser();
                CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();
                CurrentPage.WaitForWorking();
                CurrentPage.WaitForStaticTime(5000);

                for (var i = 0; i < appealSequenceWithInternalRestrictionsForAppealSummary.Length; i++)
                {
                    var appealSeq = appealSequenceWithInternalRestrictionsForAppealSummary[i];
                    CurrentPage.GetCommonSql.GetRestrictionIDAndUserTypeForSelectedAppealSeq(appealSeq, restrictionTypeList[i])
                        .ShouldBeTrue(string.Format("{0} Appeal has usertype {1} restriction assigned to it.", appealSeq, restrictionTypeList[i]));
                }

                for (var i = 0; i < appealSequenceWithInternalRestrictionsForAppealAction.Length; i++)
                {
                    var appealSeq = appealSequenceWithInternalRestrictionsForAppealAction[i];
                    CurrentPage.GetCommonSql.GetRestrictionIDAndUserTypeForSelectedAppealSeq(appealSeq, restrictionTypeList[i])
                        .ShouldBeTrue(string.Format("{0} Appeal has usertype {1} restriction assigned to it.", appealSeq, restrictionTypeList[i]));
                }

                for (var i = 0; i < appealSequenceWithClientRestrictions.Length; i++)
                {
                    var appealSeq = appealSequenceWithClientRestrictions[i];
                    CurrentPage.GetCommonSql.GetRestrictionIDAndUserTypeForSelectedAppealSeq(appealSeq, restrictionTypeList[2])
                        .ShouldBeTrue(string.Format("{0} Appeal has usertype {1} restriction assigned to it.", appealSeq, restrictionTypeList[2]));
                }

                //Stores the URLs to appropriate lists to directly open them later by claim restricted user without required restricted access
                for (int i = 0; i < 2; i++)
                {
                    CurrentPage = _appealAction =
                        _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                            appealSequenceWithInternalRestrictionsForAppealAction[i]);
                    urlListForAppealsWithInternallyRestrictedClaims.Add(CurrentPage.CurrentPageUrl);
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    CurrentPage = _appealSummary =
                        _appealSearch.SearchByAppealSequence(
                            appealSequenceWithInternalRestrictionsForAppealSummary[i]);
                    urlListForAppealsWithInternallyRestrictedClaims.Add(CurrentPage.CurrentPageUrl);
                    urlForClientUser.Add(CurrentPage.CurrentPageUrl);
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    
                    if (i == 0)
                    {
                        CurrentPage = _appealAction =
                            _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                                appealSequenceWithClientRestrictions[i]);
                        urlListForAppealsWithClientRestrictedClaims.Add(CurrentPage.CurrentPageUrl);
                        _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                    else
                    {
                        CurrentPage = _appealSummary =
                            _appealSearch.SearchByAppealSequence(appealSequenceWithClientRestrictions[i]);
                        urlListForAppealsWithClientRestrictedClaims.Add(CurrentPage.CurrentPageUrl);
                        urlForClientUser.Add(CurrentPage.CurrentPageUrl);
                        _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                        
                    }
                }
                // Log in as Internal Claim View Restricted User
                StringFormatter.PrintMessage("Verify the search results for Appeal Search Page");
                CurrentPage = _appealSearch = QuickLaunch.Logout().LoginAsClaimViewRestrictionUser().NavigateToAppealSearch();
                CurrentPage.WaitForWorking();
                CurrentPage.WaitForStaticTime(5000);
                StringFormatter.PrintMessageTitle("Verify whether restricted user can open Appeal Action/Summary Pages directly from the URL");

                StringFormatter.PrintMessage("Verifying that the restricted internal user should not be able to directly navigate to an internally" +
                    "restricted appeal summary or action page");
                foreach (var urlForInternallyRestrictedAppeal in urlListForAppealsWithInternallyRestrictedClaims)
                {
                    var url = urlForInternallyRestrictedAppeal.Replace(EnvironmentManager.ApplicationUrl, "");
                    var currentUrl = _appealSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url);
                    currentUrl.Contains("unauthorized").ShouldBeTrue("Is UnAuthorized Page Open?");
                    _appealSearch.GetUnAuthorizedMessage()
                        .ShouldBeEqual(UnAuthorizedMessage, "Is UnAuthorized Page Message Equal?");
                    CurrentPage = _appealSearch = _appealSearch.ClickOnReturnToLastPage<AppealSearchPage>();
                }

                StringFormatter.PrintMessage(string.Format("Verifying that the restricted user {0} should not be able to search for the " +
                                                           "restricted appeal ",EnvironmentManager.Username));
                VerifyAppealSearchForRestrictedAppeals(_appealSearch,
                                                        appealSequenceWithInternalRestrictionsForAppealAction,
                                                        appealSequenceWithInternalRestrictionsForAppealSummary, 
                                                        appealSequenceWithClientRestrictions);

                StringFormatter.PrintMessage(
                    "Verifying that the restricted internal user should be able to directly navigate to a client" +
                    "restricted appeal summary or action page");
                foreach (var urlForClientRestrictedAppeal in urlListForAppealsWithClientRestrictedClaims)
                {
                    var url = urlForClientRestrictedAppeal.Replace(EnvironmentManager.ApplicationUrl, "");
                    var currentUrl = "";
                    if (url.Contains("appeal_action"))
                        currentUrl = CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<AppealActionPage>(url)
                            .CurrentPageUrl;
                    else
                        currentUrl = CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<AppealSummaryPage>(url)
                            .CurrentPageUrl;
                    CurrentPage.CloseAnyPopupIfExist();
                    currentUrl.ShouldBeEqual(urlForClientRestrictedAppeal,
                        "Internal Restricted User can view the Appeal Action/Summary Page by directly following URL" +
                        "for claims which are client restricted");
                }

                if (CurrentPage.GetPageHeader() == PageHeaderEnum.AppealSummary.GetStringValue())
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                if (CurrentPage.GetPageHeader() == PageHeaderEnum.AppealAction.GetStringValue())
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                // Log in as Claim Restricted Client User
                QuickLaunch.Logout().LoginAsClientUserWithClaimViewRestriction().NavigateToAppealSearch();
                CurrentPage.WaitForWorking();
                CurrentPage.WaitForStaticTime(5000);
                StringFormatter.PrintMessageTitle("Verify Direct URL access to Appeal Action/Summary Pages for Claim Restricted Client Users");

                var urlRestrict = _appealSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(urlForClientUser[0]
                    .Replace(EnvironmentManager.ApplicationUrl, ""));
                urlRestrict.Contains("unauthorized").ShouldBeTrue("Is UnAuthorized Page Open?");
                CurrentPage.GetUnAuthorizedMessage()
                    .ShouldBeEqual(UnAuthorizedMessage, "Is UnAuthorized Page Message Equal?");
                CurrentPage = _appealSearch = _appealSearch.ClickOnReturnToLastPage<AppealSearchPage>();

                urlRestrict = CurrentPage
                    .VisitAndReturnPageByUrlFoAuthorizedPage<AppealSummaryPage>(urlForClientUser[1]
                        .Replace(EnvironmentManager.ApplicationUrl, "")).CurrentPageUrl;
                urlRestrict.ShouldBeEqual(urlForClientUser[1],
                    "Internal Restricted User can view the Appeal Action/Summary Page by directly following URL" +
                    "for claims which are client restricted");
                _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                urlRestrict = _appealSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(urlForClientUser[2]
                    .Replace(EnvironmentManager.ApplicationUrl, ""));
                urlRestrict.Contains("unauthorized").ShouldBeTrue("Is UnAuthorized Page Open?");
                CurrentPage.GetUnAuthorizedMessage()
                    .ShouldBeEqual(UnAuthorizedMessage, "Is UnAuthorized Page Message Equal?");
                CurrentPage = _appealSearch = _appealSearch.ClickOnReturnToLastPage<AppealSearchPage>();

                StringFormatter.PrintMessage(string.Format("Verifying that the restricted user {0} should not be able to search for the restricted appeal ",
                    EnvironmentManager.Username));
                VerifyAppealSearchForRestrictedAppeals(_appealSearch,
                                                        appealSequenceWithInternalRestrictionsForAppealAction,
                                                        appealSequenceWithInternalRestrictionsForAppealSummary,
                                                        appealSequenceWithClientRestrictions, false);
            }

            finally
            {
                //if (CurrentPage.GetPageHeader() == PageHeaderEnum.AppealSummary.GetStringValue())
                //    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                //if (CurrentPage.GetPageHeader() == PageHeaderEnum.AppealAction.GetStringValue())
                //    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                //CurrentPage = QuickLaunch = CurrentPage.ClickOnQuickLaunch();
            }
        }


        //Verify_claim_restrictions_when_assigning_appeals

        [Test, Category("ClaimViewRestriction")] //US68754+CAR-599(CAR-413)
        // insert into smtst.patient_restriction values(412985,3)
        // insert into smtst.patient_restriction values(416643,1)
        public void Verify_claim_restrictions_when_assigning_appeals_for_trying_to_create_appeal()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var categoryId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "CategoryId", "Value");
            var client =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Client", "Value");
            var product =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Product", "Value");
            var productAbbrevation = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProductAbbrevation", "Value");
            var procCodeFrom = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProcCodeFrom", "Value");
            var procCodeTo = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProcCodeTo", "Value");
            var trigFrom = "";
            var trigTo = "";
            var categoryOrder = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "CategoryOrder", "Value");
            var analyst = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value").Split(';');
            var claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence", "Value");
            var claimSequenceBothRestrict = DataHelper.GetSingleTestData(FullyQualifiedClassName,
                TestExtensions.TestName,
                "ClaimSequenceBothRestrict", "Value");
            var claimSequenceClientRestrict = DataHelper.GetSingleTestData(FullyQualifiedClassName,
                TestExtensions.TestName,
                "ClaimSequenceClientRestrict", "Value");
            var documentId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "DocumentId", "Value");
            var note = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "Note", "Value");
            try
            {
                StringFormatter.PrintMessageTitle(
                    "Appeal Category cosisting multiple assigned analyst with claim restirctions and not");

                _appealCategoryManager = QuickLaunch.Logout().LoginAsHciAdminUser().NavigateToAppealCategoryManager();

                StringFormatter.PrintMessageTitle("This Claim is Restricted for Internal User type");
                _appealCategoryManager.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for Internal", claimSequence));

                StringFormatter.PrintMessageTitle("This Claim is Restricted for Client User type");
                _appealCategoryManager.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequenceClientRestrict,
                        EnvironmentManager.ClientUserWithClaimViewRestriction)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for Client", claimSequenceClientRestrict));

                StringFormatter.PrintMessageTitle("This Claim is Restricted for Both User type ");
                _appealCategoryManager.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(claimSequenceBothRestrict,
                        EnvironmentManager.ClientUserWithClaimViewRestriction)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for both", claimSequenceBothRestrict));

                _appealCategoryManager.DeleteAppealCateogryAndAssociatedAppeals(categoryId, productAbbrevation,
                    procCodeFrom, procCodeTo);
                _appealCategoryManager.CreateAppealCategoryForMultipleAnalyst(categoryId, product, procCodeFrom,
                    procCodeTo, analyst, productAbbrevation, client, categoryOrder, trigFrom, trigTo);
                SearchVerifyAndDeleteAppeal(claimSequence, product, documentId, analyst, "multiple", client);
                SearchVerifyAndDeleteAppeal(claimSequenceBothRestrict, product, documentId, analyst, "multiple",
                    client);
                SearchVerifyAndDeleteAppeal(claimSequenceClientRestrict, product, documentId, analyst, "clientRestrict",
                    client);
                StringFormatter.PrintMessageTitle("Appeal Category with unassigned analyst only");
                _appealManager.NavigateToAppealCategoryManager();
                _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(productAbbrevation,
                    procCodeFrom + "-" + procCodeTo, null);
                _appealCategoryManager.ClickDeleteAnalyst(2);
                _appealCategoryManager.ClickOnSaveButtonInEdit();
                _appealCategoryManager.WaitForWorking();
                SearchVerifyAndDeleteAppeal(claimSequence, product, documentId, analyst, "single", client);
                SearchVerifyAndDeleteAppeal(claimSequenceBothRestrict, product, documentId, analyst, "single", client);
                SearchVerifyAndDeleteAppeal(claimSequenceClientRestrict, product, documentId, analyst, "clientRestrict",
                    client);
                
            }
            finally
            {
                _appealManager.NavigateToAppealCategoryManager();
                _appealCategoryManager.DeleteAppealCateogryAndAssociatedAppeals(categoryId, productAbbrevation,
                    procCodeFrom, procCodeTo);
                //CurrentPage = QuickLaunch = _appealCategoryManager.ClickOnQuickLaunch();
            }
        }


        [Test, Category("ClaimViewRestriction")] //CAR-687
        public void Verify_claim_restrictions_when_assigning_appeals_for_trying_to_create_appeal_for_restricted_analyst()
        {           
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var categoryId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "CategoryId", "Value");
            var client =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Client", "Value");
            var product =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Product", "Value");
            var productAbbrevation = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProductAbbrevation", "Value");
            var procCodeFrom = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProcCodeFrom", "Value");
            var procCodeTo = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProcCodeTo", "Value");
            var trigFrom = "";
            var trigTo = "";
            var categoryOrder = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "CategoryOrder", "Value");
            var analyst = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Analyst", "Value").Split(';');
            var analystRestrictedClaim = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AnalystRestrictedClaim", "Value").Split(';');
            var claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence", "Value").Split(',').ToList();
            
            var documentId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "DocumentId", "Value");
            var note = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "Note", "Value");
            try
            {
                StringFormatter.PrintMessageTitle(
                    "Appeal Category cosisting multiple assigned analyst with claim restirctions and not");

                _appealCategoryManager = QuickLaunch.Logout().LoginAsHciAdminUser().NavigateToAppealCategoryManager();

                StringFormatter.PrintMessageTitle("This Claim is Restricted for Internal User type");
                foreach (var claseq in claimSequence)
                {
                    _appealCategoryManager.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(claseq,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for Internal", claseq));
                }

                _appealCategoryManager.DeleteAppealCateogryAndAssociatedAppeals(categoryId, productAbbrevation,
                    procCodeFrom, procCodeTo);
                _appealCategoryManager.CreateAppealCategoryForMultipleAnalyst(categoryId, product, procCodeFrom,
                    procCodeTo, analyst, productAbbrevation, client, categoryOrder, trigFrom, trigTo, analystRestrictedClaim);

                SearchVerifyAndDeleteAppeal(claimSequence[0], product, documentId, new[] {analystRestrictedClaim[1]},
                    "AssignedRestrict", client);
                SearchVerifyAndDeleteAppeal(claimSequence[1], product, documentId, new[] {analystRestrictedClaim[2]},
                    "AssignedRestrict", client);
                SearchVerifyAndDeleteAppeal(claimSequence[2], product, documentId, new[] {analystRestrictedClaim[1]},
                    "AssignedRestrict", client);
                
                StringFormatter.PrintMessageTitle("Appeal Category with unassigned analyst only");
                _appealManager.NavigateToAppealCategoryManager();
                _appealCategoryManager.ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(productAbbrevation,
                    procCodeFrom + "-" + procCodeTo, null);
                _appealCategoryManager.ClickDeleteAnalystFromRestrictionAccessAnalystSection(2);
                _appealCategoryManager.ClickDeleteAnalystFromRestrictionAccessAnalystSection(2);
                _appealCategoryManager.ClickOnSaveButtonInEdit();
                _appealCategoryManager.WaitForWorking();
                SearchVerifyAndDeleteAppeal(claimSequence[0], product, documentId, new[] { analystRestrictedClaim[1] },
                    "", client);

               
            }
            finally
            {
                _appealManager.NavigateToAppealCategoryManager();
                _appealCategoryManager.DeleteAppealCateogryAndAssociatedAppeals(categoryId, productAbbrevation,
                    procCodeFrom, procCodeTo);
                //CurrentPage = QuickLaunch = _appealCategoryManager.ClickOnQuickLaunch();
            }
            

        }

        [Test, Category("ClaimViewRestriction")] //US68756 + US68755 ; CAR-578 + CAR - 580
        public void Verify_appeals_not_assigned_to_restricted_users_during_edit_appeal()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var claseqList = testData["ClaimSequence"].Split(';');
            var appealSeqList = testData["AppealSequence"].Split(';');
            var restrictionTypeList = new List<string> { "1", "2", "8", "1", "2", "8" };
            var bothRestrictionAppealSeqForAppealAction = appealSeqList[0];
            var onlyInternalUserRestrictionAppealSeqForAppealAction = appealSeqList[1];
            var onlyClientUserRestrictionAppealSeqForAppealAction = appealSeqList[2];

            var bothRestrictionAppealSeqForAppealSummary = appealSeqList[3];
            var onlyInternalUserRestrictionAppealSeqForAppealSummary = appealSeqList[4];
            var onlyClientUserRestrictionAppealSeqForAppealSummary = appealSeqList[5];

            try
            {
                //Get and verify restriction and user type for each claim sequence used in the test
                StringFormatter.PrintMessage("Verifying the selected claims have the required restriction access");
                for(var i=0;i<appealSeqList.Length;i++)
                {
                    CurrentPage.GetCommonSql.GetRestrictionIDAndUserTypeForSelectedAppealSeq(appealSeqList[i], restrictionTypeList[i])
                        .ShouldBeTrue(string.Format("{0} Appeal has usertype {1} restriction assigned to it.", appealSeqList[i], restrictionTypeList[i]));
                }

                QuickLaunch.Logout().LoginAsHciAdminUser();

                CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Summary whose claim has a restriction applicable to both users " +
                    "[Restriction : Offshore]");
                CurrentPage = _appealSummary =
                    _appealSearch.SearchByAppealSequence(bothRestrictionAppealSeqForAppealSummary);

                VerifyAssigningToRestrictedAppeal();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Summary whose claim has a restriction applicable only to internal users " +
                    "[Restriction : A]");
                CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                CurrentPage = _appealSummary =
                    _appealSearch.SearchByAppealSequence(onlyInternalUserRestrictionAppealSeqForAppealSummary);
                VerifyAssigningToRestrictedAppeal();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Summary whose claim has a restriction applicable only to client users " +
                    "[Restriction : B]");
                CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                CurrentPage = _appealSummary =
                    _appealSearch.SearchByAppealSequence(onlyClientUserRestrictionAppealSeqForAppealSummary);
                VerifyAssigningToUnrestrictedAppealAppeal();


                StringFormatter.PrintMessageTitle(
                    "Verifying assigning of 'Primary Reviewer' and 'Assgined To' for claims with restricted access " +
                    "to claim restricted user in Appeal Action");
                CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Action whose claim has a restriction applicable to both users " +
                    "[Restriction : Offshore]");
                CurrentPage = _appealAction =
                    _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                        bothRestrictionAppealSeqForAppealAction);

                VerifyAssigningToRestrictedAppeal(false);

                CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Action whose claim has a restriction applicable only to internal users [Restriction : A]");
                CurrentPage = _appealAction =
                    _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                        onlyInternalUserRestrictionAppealSeqForAppealAction);
                VerifyAssigningToRestrictedAppeal(false);

                CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                StringFormatter.PrintMessage(
                    "Verifying the assignment for an appeal in Appeal Action whose claim has a restriction applicable only to client users [Restriction : B]");
                CurrentPage = _appealAction =
                    _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                        onlyClientUserRestrictionAppealSeqForAppealAction);
                VerifyAssigningToUnrestrictedAppealAppeal(false);
            }

            finally
            {
                if (CurrentPage.IsPageErrorPopupModalPresent()) CurrentPage.ClosePageError();
                //CurrentPage = QuickLaunch = CurrentPage.ClickOnQuickLaunch();
            }
        }

      
        [Test, Category("ClaimViewRestriction1")]//US68757+CAR-594(CAR-422)
        public void Verify_restricted_appeals_not_assigned_to_restricted_user_from_appeal_manager()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            //Comment : appealsList[0] and appealsList[1] are restricted appeals. appealsList[2] is unrestricted
            var appealsList = testData["AppealSequence"].Split(';');

            var primaryReviewer = testData["RestrictedUser"];
            var assignedTo = testData["NotRestrictedUser"];

            try
            {
                QuickLaunch.Logout().LoginAsHciAdminUser();
                _appealManager = QuickLaunch.NavigateToAppealManager();

                var expectedUserTypeList = new List<string> {"1", "2", "8"}; //1=All,2=Internal,8=Client
                //create date of all three appeal set to 12/14/2017 

                _appealManager.GetCommonSql.GetRestrictedUserTypeListByAppealSequence(testData["AppealSequence"]
                    .Replace(';', ',')).ShouldCollectionBeEqual(expectedUserTypeList,
                    "Verify Restricted User Type List of Appeal");

                _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.SelectAllAppeals();
                _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
                _appealManager.SetDateFieldFrom("Create Date", "12/14/2017");
                _appealManager.SetDateFieldTo("Create Date", "12/14/2017");
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                var totalRow = _appealManager.GetSearchResultCount();

                for (var i = 0; i < 3; i++)
                {
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealsList[2]);
                    if (_appealManager.GetAppealDetailsValueByLabel("Assigned To") ==
                        assignedTo.Split('(')[0].Trim())
                    {
                        var temp = assignedTo;
                        assignedTo = primaryReviewer;
                        primaryReviewer = temp;
                    }


                    var previousPrimaryReviewer = _appealManager.GetSearchResultListByCol(8);
                    var previousAssignedTo = new List<string>();

                    for (var j = 0; j < totalRow; j++)
                    {
                        _appealManager.ClickOnSearchListRow(j + 1);
                        _appealManager.WaitForWorkingAjaxMessage();
                        previousAssignedTo.Add(_appealManager.GetAppealDetailsValueByLabel("Assigned To"));
                    }

                    _appealManager.ClickOnSidebarIcon();
                    _appealManager.ClickOnHeaderEditIcon();

                    switch (i)
                    {
                        case 0:
                            _appealManager.ClickOnSelectAllCheckBox();
                            break;
                        case 1:
                            _appealManager.ClickOnSelectAppealListIn3ColumnPageByAppeal(appealsList[0]);
                            _appealManager.ClickOnSelectAppealListIn3ColumnPageByAppeal(appealsList[2]);
                            break;
                        case 2:
                            _appealManager.ClickOnSelectAppealListIn3ColumnPageByAppeal(appealsList[1]);
                            _appealManager.ClickOnSelectAppealListIn3ColumnPageByAppeal(appealsList[2]);
                            break;
                    }

                    _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", primaryReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", assignedTo);
                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitToReturnAppealManagerSearchPage();

                    switch (i)
                    {
                        case 0:
                            _appealManager.GetPageErrorMessage().ShouldBeEqual("Appeal Seq : " + appealsList[0] + "," +
                                                                           appealsList[1] +
                                                                           " cannot be assigned to a user due to restrictions " +
                                                                           "associated with the claims.");
                            break;
                        case 1:
                            _appealManager.GetPageErrorMessage().ShouldBeEqual("Appeal Seq : " + appealsList[0] +
                                                                           " cannot be assigned to a user due to restrictions " +
                                                                           "associated with the claims.");
                            break;
                        case 2:
                            _appealManager.GetPageErrorMessage().ShouldBeEqual("Appeal Seq : " + appealsList[1] +
                                                                           " cannot be assigned to a user due to restrictions " +
                                                                           "associated with the claims.");
                            break;
                    }

                    _appealManager.ClosePageError();
                    //primary reviewer and assignedto should updated for client restrcied claim
                    for (var j = 0; j < totalRow; j++)
                    {
                        if (_appealManager.GetSearchResultByColRow(4, j + 1) == appealsList[2])
                        {
                            _appealManager.ClickOnSearchListRow(j + 1);
                            _appealManager.WaitForWorkingAjaxMessage();
                            _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                                .ShouldBeEqual(assignedTo.Split('(')[0].Trim(), "Assigned To Should get updated");
                            _appealManager.GetSearchResultByColRow(8, j + 1)
                                .ShouldBeEqual(primaryReviewer, "Primary Reviewer Should get updated");
                            continue;
                        }

                        _appealManager.ClickOnSearchListRow(j + 1);
                        _appealManager.WaitForWorkingAjaxMessage();
                        _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                            .ShouldBeEqual(previousAssignedTo[j].Split('(')[0].Trim(),
                                "Assigned To Should be previous value");
                        _appealManager.GetSearchResultByColRow(8, j + 1).Split('(')[0].Trim()
                            .ShouldBeEqual(previousPrimaryReviewer[j], "Primary Reviewer Should be previous value");
                    }
                }
            }

            finally
            {
                if (_appealManager.IsPageErrorPopupModalPresent()) _appealManager.ClosePageError();
                //CurrentPage = QuickLaunch = CurrentPage.ClickOnQuickLaunch();
            }
        }


        [Test, Category("ClaimViewRestriction2")] //CAR-491 + CAR-426 + CAR 436
        public void Validate_claim_view_restriction_access_for_PCI_Claim_RN_Coder_Worklist_for_user_with_access()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var claimSequence = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value")
                .Split(',').ToList();
            try
            {
                _claimAction = QuickLaunch.Logout().LoginAsHciAdminUser().NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");

                _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions", RestrictionGroup.InternalUser.GetStringValue());
                CurrentPage = _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");
                var authorizedPciClaseq = _claimAction.GetClaimSequence();
                _claimAction.IsClaimRestrictionIconPresent()
                    .ShouldBeTrue("An Icon to indicate claim has restriction should  be present");
                IsClaimRestrictiontooltipAsExpected(RestrictionGroup.InternalUser.GetStringValue(), _claimAction.GetClaimRestrictionTooltip());
                _claimAction.GetCommonSql
                    .IsClaimAllowedByClaimSequenceUserIdAndAccessExcludingAll(authorizedPciClaseq,
                        EnvironmentManager.HciAdminUsername, RestrictionGroup.InternalUser.GetStringValue())
                    .ShouldBeTrue(string.Format(
                        "{0} is allowed CV claim sequence for {1} user as access has been assigned",
                        authorizedPciClaseq, EnvironmentManager.HciAdminUsername));


                /* CAR 426 story starts here */
                SearchByClaimSeqFromWorkList(claimSequence[2]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                /* depends on script 
                    INSERT into SMTST.patient_restriction VALUES (840669 , 1); */
                var restrcition = String.Join(", ",
                    _claimAction.GetCommonSql.GetAppliedRestrictionListInClaim(claimSequence[2]));
                IsClaimRestrictiontooltipAsExpected(restrcition, _claimAction.GetClaimRestrictionTooltip());

                /* CAR 426 story ends here */

                /* depends on script 
                    INSERT into SMTST.patient_restriction VALUES (840802 , 2); 
                    update smtst.hcicla set coder_review = 'T' where claseq = 1458907
                  1459146 388966 1459116
                    update smtst.hcicla set coder_review = 'T' where claseq = 1459116;/;/
                 */
                _claimAction.NavigateToCVCodersClaim();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");
                if (_claimAction.IsClaimRestrictionIconPresent()) _claimAction.ClickOnNextButton();
                _claimAction.ClickWorkListIcon();

                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions", RestrictionGroup.InternalUser.GetStringValue());
                CurrentPage = _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");
                var authorizedCoderClaseq = _claimAction.GetClaimSequence();
                _claimAction.IsClaimRestrictionIconPresent().ShouldBeTrue("An Icon to indicate claim has restriction should  be present");
                _claimAction.IsClaimRestrictionIndicatorTooltipPresent("coder").ShouldBeTrue("An Icon indicating claim is coder type is present shold be true");
                IsClaimRestrictiontooltipAsExpected("A", _claimAction.GetClaimRestrictionTooltip());
                _claimAction.GetCommonSql
                    .IsClaimAllowedByClaimSequenceUserIdAndAccessExcludingAll(authorizedCoderClaseq,
                        EnvironmentManager.HciAdminUsername, RestrictionGroup.InternalUser.GetStringValue())
                    .ShouldBeTrue(string.Format(
                        "{0} is allowed CV coder claim sequence for {1} user as access has been assigned",
                        authorizedCoderClaseq, EnvironmentManager.HciAdminUsername));

                _claimAction.NavigateToCVRnWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");
                _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions", RestrictionGroup.InternalUser.GetStringValue());
                CurrentPage = _claimAction.ClickOnCreateButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");
                var authorizedRnClaseq = _claimAction.GetClaimSequence();
                _claimAction.IsClaimRestrictionIconPresent().ShouldBeTrue("An Icon to indicate claim has restriction should  be present");
                _claimAction.IsClaimRestrictionIndicatorTooltipPresent("rn").ShouldBeTrue("An Icon indicating claim is coder type is present shold be true");
                IsClaimRestrictiontooltipAsExpected(RestrictionGroup.InternalUser.GetStringValue(), _claimAction.GetClaimRestrictionTooltip());
                _claimAction.GetCommonSql
                    .IsClaimAllowedByClaimSequenceUserIdAndAccessExcludingAll(authorizedRnClaseq,
                        EnvironmentManager.HciAdminUsername, RestrictionGroup.InternalUser.GetStringValue())
                    .ShouldBeTrue(string.Format(
                        "{0} is allowed CV RN claim sequence for {1} user as access has been assigned",
                        authorizedRnClaseq, EnvironmentManager.HciAdminUsername));

                _claimAction = _claimAction.Logout().LoginAsClaimViewRestrictionUser()
                    .NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl).ShouldBeFalse(
                    "Claim with a restriction value and unassigned access has been opened should be false");

                var pciClaseqFromMenu = _claimAction.GetClaimSequence();
                _claimAction.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(pciClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV Claim: {0} is in restricted group for Internal and allowed to view should be false",
                        pciClaseqFromMenu));

                _claimAction.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(pciClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV Claim: {0} is in restricted group for Internal and allowed to view should be false",
                        pciClaseqFromMenu));

                CurrentPage = _claimAction.ClickWorkListIcon().ClickSearchIcon();

                _claimAction.SetClaimSequenceInFindClaimSection(authorizedPciClaseq);
                _claimAction.ClickOnFindButton();
                _claimAction.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal when restricted claims is searched without respective assigned access.?");
                _claimAction.ClosePageError();





                _claimAction = _claimAction.NavigateToCVRnWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl).ShouldBeFalse(
                    "Claim with a restriction value and unassigned access has been opened should be false");
                var rnClaseqFromMenu = _claimAction.GetClaimSequence();
                _claimAction.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(rnClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV RN: {0} is in restricted group for Internal and allowed to view should be false",
                        rnClaseqFromMenu));

                _claimAction.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(rnClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV Claim: {0} is in restricted group for Internal and allowed to view should be false",
                        pciClaseqFromMenu));


                CurrentPage = _claimAction.ClickWorkListIcon().ClickSearchIcon();

                _claimAction.SetClaimSequenceInFindClaimSection(authorizedRnClaseq);
                _claimAction.ClickOnFindButton();
                _claimAction.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal when restricted claims is searched without respective assigned access.?");
                _claimAction.ClosePageError();

                StringFormatter.PrintMessageTitle("Update claimsequence coder review value for test data availability");
                _claimAction.GetCommonSql.UpdateCoderReviewForClaimSequence(claimSequence[0], "T");
                _claimAction.GetCommonSql.UpdateCoderReviewForClaimSequence(claimSequence[1], "T");


                _claimAction = _claimAction.NavigateToCVCodersClaim();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl).ShouldBeFalse(
                    "Claim with a restriction value and unassigned access has been opened should be false");
                var coderClaseqFromMenu = _claimAction.GetClaimSequence();
                _claimAction.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(coderClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV Coder: {0} is in restricted group for Internal and allowed to view should be false",
                        coderClaseqFromMenu));


                _claimAction.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(coderClaseqFromMenu,
                        EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(string.Format(
                        "CV Claim: {0} is in restricted group for Internal and allowed to view should be false",
                        pciClaseqFromMenu));

                CurrentPage = _claimAction.ClickWorkListIcon().ClickSearchIcon();

                _claimAction.SetClaimSequenceInFindClaimSection(authorizedCoderClaseq);
                _claimAction.ClickOnFindButton();
                _claimAction.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.InternalUser.GetStringValue()),
                    "Is Poppup Message Equal when restricted claims is searched without respective assigned access.?");
                _claimAction.ClosePageError();
            }
            finally
            {
                if (IsUrlUnauthorized(CurrentPage.CurrentPageUrl))
                {
                    QuickLaunch =
                        CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<QuickLaunchPage>(PageUrlEnum.QuickLaunch
                            .GetStringValue());
                }

                //else CurrentPage = CurrentPage.ClickOnQuickLaunch();
            }
        }


        #endregion

        #region PRIVATE METHODS

        private void VerifyRestrictionWarningNotification(string claseq, string restrictionType, bool shouldShowWarningNotification = true, string message = "")
        {
            _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            var _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

            if (shouldShowWarningNotification && restrictionType != null)
            {
                IsClaimRestrictiontooltipAsExpected(restrictionType, _claimAction.GetClaimRestrictionTooltip());
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.IsRestrictionIconPresent().ShouldBeTrue("Is restriction icon present in Claim Search result?");
                _claimSearch.GetRestrictionIconToolTip().ShouldBeEqual(
                    $"This claim is allowed to be viewed only by users that have the {restrictionType} access.");
            }
            else
            {
                _claimAction.IsClaimRestrictionIconPresent().ShouldBeFalse(message);
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.IsRestrictionIconPresent().ShouldBeFalse("Is restriction icon present in Claim Search result?");
            }
        }

        private void SearchVerifyAndDeleteAppeal(string claimSequence, string product, string documentId,
            string[] analyst, string analystcase, string client = "SMTST")
        {
            CurrentPage = _appealCreator = CurrentPage.NavigateToAppealCreator();
            _appealCreator.SearchByClaimSequence(claimSequence);
            _appealCreator.CreateAppealForClaimSequence(product, false, documentId);
            CurrentPage = _appealManager = CurrentPage.NavigateToAppealManager();
            _appealManager.SearchByClaimSequence(claimSequence, client);
            switch (analystcase)
            {
                case "multiple":
                    (_appealManager.GetSearchResultByColRow(8) + " (" + EnvironmentManager.Username + ")")
                        .ShouldBeEqual(analyst[1],
                            "Analyst without Claim restriction should be assigned as the primary reviewer.");
                    break;
                case "clientRestrict":
                    (_appealManager.GetSearchResultByColRow(8) + " (" + EnvironmentManager.HciClaimViewRestrictionUsername + ")")
                        .ShouldBeEqual(analyst[0],
                            "Analyst without Claim restriction should be assigned as the primary reviewer.");
                    break;
                case "AssignedRestrict":
                    _appealManager.GetSearchResultByColRow(8) 
                        .ShouldBeEqual(analyst[0].Split('(')[0].Trim(),
                            "Primary Reviewer Should Assiged to Assigend Claim Restriction");
                    break;
                default:
                    _appealManager.GetSearchResultByColRow(8).ShouldBeEqual("",
                        "Primary Reviewer should be empty if there is anlayst is claim restriction only");
                    break;
            }

            _appealManager.ClickOnDeleteIconByRowSelector(1);
            _appealManager.ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Deleting the appeal hence created for the test");
            _appealManager.ClickOnFindButton();
            _appealManager.WaitForWorking();
            _appealManager.GetNoMatchingRecordFoundMessage().ShouldBeEqual("No matching records were found.",
                "No Appeals should be present.");
            _appealManager.ClickOnClearLink();
        }

        private void VerifyAssigningToRestrictedAppeal(bool appealSummary = true)
        {
            if (appealSummary)
            {
                var originalPrimaryReviewer = _appealSummary.GetAppealSummaryValueByRowCol(2, 4);
                var originalAssignedTo = _appealSummary.GetAppealSummaryValueByRowCol(2, 5);

                List<string> selectedItems = InputAppealDataInAppealSummaryOrAction();
                ErrorMessageValidation(selectedItems[0], selectedItems[1]);
                _appealSummary.ClickOnCancelLinkOnEditAppeal();
                StringFormatter.PrintMessage("Verifying that the edits made should not be saved");
                VerifyEditsAreNotSavedInAppealSummary(originalPrimaryReviewer, originalAssignedTo);

                selectedItems = InputAppealDataInAppealSummaryOrAction(true, true, false);
                ErrorMessageValidation(selectedItems[0], selectedItems[1]);
                _appealSummary.ClickOnCancelLinkOnEditAppeal();
                StringFormatter.PrintMessage("Verifying that the edits made should not be saved");
                VerifyEditsAreNotSavedInAppealSummary(originalPrimaryReviewer, originalAssignedTo);

                selectedItems = InputAppealDataInAppealSummaryOrAction(true, false);
                ErrorMessageValidation(selectedItems[0], selectedItems[1]);
                _appealSummary.ClickOnCancelLinkOnEditAppeal();
                StringFormatter.PrintMessage("Verifying that the edits made should not be saved");
                VerifyEditsAreNotSavedInAppealSummary(originalPrimaryReviewer, originalAssignedTo);
            }

            else
            {
                var originalPrimaryReviewerAppealAction = _appealAction.GetAppealInformationByLabel("Primary Reviewer");
                var originalAssignedToAppealAction = _appealAction.GetAppealInformationByLabel("Assigned To");

                List<string> selectedItemsAppealAction = InputAppealDataInAppealSummaryOrAction(false);
                ErrorMessageValidation(selectedItemsAppealAction[0], selectedItemsAppealAction[1]);
                _appealAction.GetAppealInformationByLabel("Primary Reviewer")
                    .ShouldBeEqual(originalPrimaryReviewerAppealAction);
                _appealAction.GetAppealInformationByLabel("Assigned To").ShouldBeEqual(originalAssignedToAppealAction);

                selectedItemsAppealAction = InputAppealDataInAppealSummaryOrAction(false, true, false);
                ErrorMessageValidation(selectedItemsAppealAction[0], selectedItemsAppealAction[1]);
                _appealAction.GetAppealInformationByLabel("Primary Reviewer")
                    .ShouldBeEqual(originalPrimaryReviewerAppealAction);
                _appealAction.GetAppealInformationByLabel("Assigned To").ShouldBeEqual(originalAssignedToAppealAction);

                selectedItemsAppealAction = InputAppealDataInAppealSummaryOrAction(false, false);
                ErrorMessageValidation(selectedItemsAppealAction[0], selectedItemsAppealAction[1]);
                _appealAction.GetAppealInformationByLabel("Primary Reviewer")
                    .ShouldBeEqual(originalPrimaryReviewerAppealAction);
                _appealAction.GetAppealInformationByLabel("Assigned To").ShouldBeEqual(originalAssignedToAppealAction);
            }
        }

        private List<string> InputAppealDataInAppealSummaryOrAction(bool appealSummary = true,
            bool primaryReviewer = true, bool assignedTo = true)
        {
            string primaryReviewerValue;
            string assignedToValue = primaryReviewerValue ="Test Automation";
            string restrictedUser = "Claim_View Restriction_User";
            List<string> listOfPrimaryReviewerAndAssignedTo = new List<string>();
            if (appealSummary)
            {
                _appealSummary.ClickOnEditIcon();
                string selectedPrimaryReviewer = primaryReviewer ? restrictedUser : primaryReviewerValue;
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", selectedPrimaryReviewer);
                string selectedAssignedTo = assignedTo ? restrictedUser : assignedToValue;
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Assigned To", selectedAssignedTo);


                //Adding the selected Primary Reviewer and Assigned To to the list 'listOfPrimaryReviewerAndAssignedTo'
                listOfPrimaryReviewerAndAssignedTo.AddRange(new List<string>
                {
                    selectedPrimaryReviewer,
                    selectedAssignedTo
                });

                _appealSummary.ClickOnSaveButtonOnEditAppeal();

                return listOfPrimaryReviewerAndAssignedTo;
            }

            _appealAction.ClickOnEditIcon();
            string selectedPrimaryReviewerAction = primaryReviewer ? restrictedUser : primaryReviewerValue;
            _appealAction.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", selectedPrimaryReviewerAction);
            string selectedAssignedToAction = assignedTo ? restrictedUser : assignedToValue;
            _appealAction.SelectEditAppealFieldDropDownListByLabel("Assigned to", selectedAssignedToAction);

            //Adding the selected Primary Reviewer and Assigned To to the list 'listOfPrimaryReviewerAndAssignedTo'
            listOfPrimaryReviewerAndAssignedTo.AddRange(new List<string>
            {
                selectedPrimaryReviewerAction,
                selectedAssignedToAction
            });

            _appealAction.ClickOnSaveButton();

            return listOfPrimaryReviewerAndAssignedTo;
        }

        private void ErrorMessageValidation(string primaryReviewer, string assignedTo)
        {
            QuickLaunch.IsPageErrorPopupModalPresent().ShouldBeTrue(string.Format(
                "Error pops up when {0} is 'Primary Reviewer' and " +
                "{1} is 'Assigned To'", primaryReviewer, assignedTo));

            QuickLaunch.GetPageErrorMessage().ShouldBeEqual(
                "This appeal cannot be assigned to this user due to claim view restrictions. Please select a different Primary Reviewer " +
                "and/or Assigned To value.");
            QuickLaunch.ClosePageError();
        }

        private void VerifyAssigningToUnrestrictedAppealAppeal(bool appealSummary = true)
        {
            var dueDate = DateTime.Now.ToString("MM/d/yyyy");
            var primaryOroReviewerAssignedT = "Claim_View Restriction_User";
            if (appealSummary)
            {
                _appealSummary.ClickOnEditIcon();

                StringFormatter.PrintMessage("Verifying that the edits made should be saved");
                _appealSummary.SetDueDate(dueDate);
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", primaryOroReviewerAssignedT);
                _appealSummary.SelectEditAppealFieldDropDownListByLabel("Assigned To", primaryOroReviewerAssignedT);
                _appealSummary.ClickOnSaveButtonOnEditAppeal();

                //Verify whether the changes are saved in the Appeal Summary Page.
                _appealSummary.GetAppealSummaryValueByRowCol(1, 3).ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                    string.Format("Due Date is successfully saved as : {0}", dueDate));
                _appealSummary.GetAppealSummaryValueByRowCol(2, 4).ShouldBeEqual(primaryOroReviewerAssignedT,
                    string.Format("Primary Reviewer is successfully saved as : '{0}'", primaryOroReviewerAssignedT));
                _appealSummary.GetAppealSummaryValueByRowCol(2, 5).ShouldBeEqual(primaryOroReviewerAssignedT,
                    string.Format("Assigned To is successfully saved as : '{0}'", primaryOroReviewerAssignedT));
            }
            else
            {
                _appealAction.ClickOnEditIcon();

                StringFormatter.PrintMessage("Verifying that the edits made should be saved");
                _appealAction.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", primaryOroReviewerAssignedT);
                _appealAction.SelectEditAppealFieldDropDownListByLabel("Assigned to", primaryOroReviewerAssignedT);
                _appealAction.ClickOnSaveButton();

                //Verify whether the changes are saved in the Appeal Action Page.
                _appealAction.GetAppealInformationByLabel("Primary Reviewer").ShouldBeEqual(primaryOroReviewerAssignedT,
                    string.Format("Primary Reviewer is successfully saved as : '{0}'", primaryOroReviewerAssignedT));
                _appealAction.GetAppealInformationByLabel("Assigned To").ShouldBeEqual(primaryOroReviewerAssignedT,
                    string.Format("Assigned To is successfully saved as : '{0}'", primaryOroReviewerAssignedT));
            }
        }

        private void VerifyEditsAreNotSavedInAppealSummary(string originalPrimaryReviewer, string originalAssignedTo)
        {
            _appealSummary.GetAppealSummaryValueByRowCol(2, 4).ShouldBeEqual(originalPrimaryReviewer);
            _appealSummary.GetAppealSummaryValueByRowCol(2, 5).ShouldBeEqual(originalAssignedTo);
        }

        private void VerifyRestrictedClaimForRestrictedUser(List<string> claimList, bool restricted = true)
        {
            CurrentPage = _userProfileManager = QuickLaunch.NavigateToProfileManager();

            //Verifying whether the claims are in restricted group
            _userProfileManager.ClickOnRestrictionsTab();
            var assgndList = _userProfileManager.GetAssignedRestrictionsListFromSection();

            if (restricted)
            {
                foreach (string claim in claimList)
                {
                    _userProfileManager.GetRestrictedClaimCountForClaimSequence(claim, assgndList)
                        .ShouldBeGreater(0, string.Format("{0} is in restricted group", claim));
                }
            }
            else
            {
                foreach (string claim in claimList)
                {
                    _userProfileManager.GetRestrictedClaimCountForClaimSequence(claim, assgndList)
                        .ShouldBeEqual(0, string.Format("{0} is not in restricted group", claim));
                }
            }
        }


        public void IsClaimRestrictiontooltipAsExpected(string restriction, string actual)
        {
            var expected =
                string.Format(
                    "This claim is allowed to be viewed only by users that have the {0} access. Please contact management if you are viewing this claim in error.",
                    restriction);
            actual.ShouldBeEqual(expected,
                "Expected tool tip message should be present for restricted claim, accessible through access assigned");
        }

        void SearchByClaimSeqFromWorkList(string claimSeq)
        {
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }

            _claimAction.ClickSearchIcon()
                .SearchByClaimSequence(claimSeq);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        }

    
        public void VerifyAppealSearchForRestrictedAppeals(AppealSearchPage newAppealSearch,
                                                      string[] appealSequenceWithInternalRestrictionsForAppealAction,
                                                      string[] appealSequenceWithInternalRestrictionsForAppealSummary,
                                                      string[] appealSequenceWithClientRestrictions, bool internalUser = true)
        {
            if (internalUser)
            {
                for (var i = 0; i < 2; i++)
                {
                    newAppealSearch.SearchByAppealSequenceForRestrictedAppeal(appealSequenceWithInternalRestrictionsForAppealAction[i]);
                    newAppealSearch.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue(
                        "Appeals associated with restricted claims access for both users and internal users" +
                        "should not be displayed in Appeal Search Page when searched by a restricted internal user");

                    newAppealSearch.SearchByAppealSequenceForRestrictedAppeal(appealSequenceWithInternalRestrictionsForAppealSummary[i]);
                    newAppealSearch.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue(
                        "Appeals associated with restricted claims access for both users and internal users" +
                        "should not be displayed in Appeal Search Page when searched by a restricted internal user");

                    _appealManager = newAppealSearch.NavigateToAppealManager();
                    _appealManager.SelectAllAppeals();
                    _appealManager.SearchByAppealSequence(appealSequenceWithInternalRestrictionsForAppealAction[i]);
                    _appealManager.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue(
                        "Appeals associated with restricted claims access for both/internal users" +
                        "should not be displayed in Appeal Manager Page when searched by a restricted internal user");

                    _appealManager.SearchByAppealSequence(appealSequenceWithClientRestrictions[i]);
                    _appealManager.IsNoMatchingRecordFoundMessagePresent().ShouldBeFalse(
                        "Appeals associated with client restricted claims access" +
                        "should be displayed in Appeal Manager Page when searched by any internal user ");

                    newAppealSearch = _appealManager.NavigateToAppealSearch();
                }
                _appealManager = newAppealSearch.NavigateToAppealManager();
                _appealManager.SelectAllAppeals();
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        newAppealSearch.SearchByAppealSequenceForRestrictedAppeal(appealSequenceWithInternalRestrictionsForAppealAction[i]);
                        newAppealSearch.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue(
                            "Appeals associated with restricted claims access for both users" +
                            "should not be displayed in Appeal Search Page when searched by a client internal user");
                    }

                    newAppealSearch.SearchByAppealSequenceForRestrictedAppeal(appealSequenceWithClientRestrictions[i]);
                    newAppealSearch.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue("Appeals associated with client restricted claims access" +
                                                                                          "should be displayed in Appeal Search Page when searched by any internal user ");
                }
            }
        }

        #endregion
    }
}