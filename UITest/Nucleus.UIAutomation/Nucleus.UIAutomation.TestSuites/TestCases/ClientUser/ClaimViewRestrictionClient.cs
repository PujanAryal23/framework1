using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    class ClaimViewRestrictionClient : AutomatedBaseClient

    {

        private ClaimSearchPage _claimSearch;

        private ClaimActionPage _claimAction;

        //private ProfileManagerPage _userProfileManager;
        private const string UnAuthorizedMessage =
            "It appears that you do not have the necessary authority to view this page.";

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
                UserLoginIndex = 1;
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
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENTCLAIMVIEWRESTRICTION,
                    StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = QuickLaunch.Logout().LoginAsClientUserWithClaimViewRestriction();
                //CheckTestClientAndSwitch();
            }
            //else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.QuickLaunch.GetStringValue()))
            //{
            //    CurrentPage.ClickOnQuickLaunch();
            //}
            CurrentPage.ClickOnLogo();
        }

        #endregion

        #region TEST SUITES

        [Test, Category("ClaimViewRestriction")]
        //CAR-1926 (CAR-2000) + CAR-2605(CAR-2695)
        public void Verify_restriction_notification_to_show_correctly_based_on_user_type_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var claSeqRestrictedForInternalUserOnly = testData["ClaSeqRestrictedForInternalUserOnly"];
            var claSeqRestrictedForClientUserOnly = testData["ClaSeqRestrictedForClientUserOnly"];
            var claSeqRestrictedForBothUsers = testData["ClaSeqRestrictedForBothUsers"];
            var claSeqWithNoRestriction = testData["ClaSeqWithNoRestriction"];
            var claSeqRestrictedForInternalAndBoth = testData["ClaSeqRestrictedForInternalAndBoth"];
            CurrentPage = _claimSearch = CurrentPage.Logout().LoginAsClientUser().NavigateToClaimSearch();

            StringFormatter.PrintMessageTitle("Verification of the restriction notification for client user");

            StringFormatter.PrintMessage(
                "Verification of the restriction notification for claim which is restricted to internal user only");
            VerifyRestrictionWarningNotification(claSeqRestrictedForInternalUserOnly,
                RestrictionGroup.InternalUser.GetStringValue(), false,
                $"Restriction warning notification should not be shown to client users for claims with {RestrictionGroup.InternalUser.GetStringValue()} restriction");
           
            StringFormatter.PrintMessage(
                "Verification of the restriction notification for claim which is restricted to client user only");
            VerifyRestrictionWarningNotification(claSeqRestrictedForClientUserOnly,
                RestrictionGroup.ClientUser.GetStringValue(), message:
                $"Restriction warning notification should not be shown to client users for claims with {RestrictionGroup.ClientUser.GetStringValue()} restriction");
     
            StringFormatter.PrintMessage(
                "Verification of the restriction notification for claim which is restricted to both internal and client users");
            VerifyRestrictionWarningNotification(claSeqRestrictedForBothUsers,
                RestrictionGroup.AllUser.GetStringValue(), message:
                $"Restriction warning notification should not be shown to client users for claims with {RestrictionGroup.AllUser.GetStringValue()} restriction");
            
            StringFormatter.PrintMessage(
                "Verification of the restriction notification for unrestricted claim");
            VerifyRestrictionWarningNotification(claSeqWithNoRestriction,"", false, "Restriction warning notification should not be shown to client users for unrestricted claims"
                );
            
            StringFormatter.PrintMessage("Verification of the restriction notification for claim which is restricted to both internal and both user type and comma separated access in the tooltip");
            VerifyRestrictionWarningNotification(claSeqRestrictedForInternalAndBoth,
                $"{RestrictionGroup.AllUser.GetStringValue()}, {RestrictionGroup.ClientUser.GetStringValue()}", true,
                $"Restriction warning notification should not be shown to client users for claims with {RestrictionGroup.AllUser.GetStringValue()},{RestrictionGroup.ClientUser.GetStringValue()} restriction");
          
        }

        //[Test, Category("ClaimViewRestriction")]//US68753 
        //public void Verify_claim_should_not_display_for_restricted_claim_for_restricted_client_user()
        //{
        //        TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //        var claimSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
        //            "ClaimSequence", "Value");
        //        CurrentPage = _claimSearch = QuickLaunch.NavigateToClaimSearch();          
        //        try
        //        {
        //            _claimSearch.SearchByClaimSequence(claimSequence);
        //            _claimSearch.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
        //                .ShouldBeEqual("No matching records were found.",
        //                   "Is No Matching Record Found and Text Equals?");
        //        }
        //        finally
        //        {
        //           CurrentPage = _claimSearch.GoToQuickLaunch();    
        //        }
        //}

        [Test, Category("ClaimViewRestriction")]
        //CAR-506,CAR-495,CAR-417,CAR-855
        public void Verify_visibility_of_claim_accordingly_to_claim_view_restriction_logic()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var claimSequence = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value")
                .Split(',').ToList();
            var claimNumber = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimNumber", "Value")
                .Split(',').ToList();
            var status = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Status", "Value").Split(',')
                .ToList();
            var patientSeq = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "patientSeq", "Value").Split(',')
                .ToList();
            var beginDos = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "beginDos", "Value").Split(',')
                .ToList();
            var endDos = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "endDos", "Value").Split(',')
                .ToList();
            var source =
                DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "source", "Value");
            var prvSeq = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "prvSeq", "Value").Split(',')
                .ToList();
            var range = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "range",
                "Value");
            CurrentPage = _claimSearch = CurrentPage.NavigateToClaimSearch();
            var url = new List<string>();
            var patientHistoryurl = new List<string>();


            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Internal User type but should be visible to client User");
            _claimSearch.GetCommonSql
                .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence[0],
                    EnvironmentManager.HciClaimViewRestrictionUsername)
                .ShouldBeFalse(string.Format("{0} is in restricted group for Internal", claimSequence[0]));

            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Client User type and should not visible to Client User");
            _claimSearch.GetCommonSql
                .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claimSequence[1],
                    EnvironmentManager.ClientUserWithClaimViewRestriction)
                .ShouldBeFalse(string.Format("{0} is in restricted group for Client", claimSequence[1]));

            StringFormatter.PrintMessageTitle(
                "This Claim is Restricted for Both User type and should not visble to restricted client User");
            _claimSearch.GetCommonSql
                .IsClaimAllowByClaimSequenceAndUserId(claimSequence[2],
                    EnvironmentManager.ClientUserWithClaimViewRestriction)
                .ShouldBeFalse(string.Format("{0} is in restricted group for both", claimSequence[2]));

            foreach (var claSeq in claimSequence)
            {
                var x = string.Format("app/#/clients/{0}/claims/{1}/{2}/lines/claim_lines?claimSearchType=findClaims",
                    ClientEnum.SMTST, claSeq.Split('-')[0], claSeq.Split('-')[1]);
                url.Add(x);
            }

            int i = 0;
            while (i < 3)
            {
                var x = string.Format(
                    @"clients/{0}/claimhistory?Client={0}&claSeq={1}&claSub={2}&status={3}&patientSeq={4}&beginDos={5}&endDos={6}&source={7}&prvSeq={8}&range={9}"
                    , ClientEnum.SMTST
                    , claimSequence[i].Split('-')[0]
                    , claimSequence[i].Split('-')[1]
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
                var currentPage = _claimAction =
                    _claimSearch.VisitAndReturnPageByUrlFoAuthorizedPage<ClaimActionPage>(url[0]);
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence[0], "Is Claim Action Page Open?");
                var claimDetailPatientSeq = currentPage.GetPatientSeq();
                var patHistPage =
                    _claimAction
                        .VisitAndReturnPageByUrlFoAuthorizedPage<ClaimHistoryPage>(patientHistoryurl[0]); /* CAR 417 */
                currentPage.GetPatientHistoryPatientSeq()
                    .ShouldBeEqual(claimDetailPatientSeq, "Patient seq should be equal");

                var currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url[1]);
                VerifyIfUrlIsUnauthorized(currentUrl);
                var patHistoryPage =
                    _claimAction.VisitAndGetUrlByUrlForUnAuthorizedPage(patientHistoryurl[1]); /* CAR 417 */
                VerifyIfUrlIsUnauthorized(patHistoryPage);

                currentUrl = _claimSearch.VisitAndGetUrlByUrlForUnAuthorizedPage(url[2]);
                VerifyIfUrlIsUnauthorized(currentUrl);
                patHistoryPage =
                    _claimAction.VisitAndGetUrlByUrlForUnAuthorizedPage(patientHistoryurl[2]); /* CAR 417 */
                VerifyIfUrlIsUnauthorized(patHistoryPage);
                _claimAction.SwitchBack();

                _claimSearch.NavigateToClaimSearch();
                _claimSearch.SearchByClaimSequence(claimSequence[0]);
                currentPage.GetClaimSequence().ShouldBeEqual(claimSequence[0], "Is Claim Action Page Open?");
                currentPage.ClickClaimSearchIcon();
                _claimSearch.SetClaimSequenceInFindClaimSection(claimSequence[1]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.ClientUser.GetStringValue()),
                    "Is Poppup Message Equal?");
                _claimSearch.ClosePageError();

                _claimSearch.SetClaimNoInFindClaimSection(claimNumber[0]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.ClientUser.GetStringValue()),
                    "Is Poppup Message Equal for Search by Claim Number?");
                _claimSearch.ClosePageError();

                _claimSearch.SetOtherClaimNoInFindClaimSection(claimNumber[1]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.ClientUser.GetStringValue()),
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



        [Test, Category("ClaimViewRestriction")]
        //CAR-491 + CAR - 426 + CAR 436 + CAR-1926
        public void Validate_claim_view_restriction_access_for_PCI_Worklist()
        {
            // insert into smtst.patient_restriction values(345021,1)



            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var claimSequence = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");
            try
            {
                _claimAction = QuickLaunch.Logout().LoginAsClientUser().NavigateToCVClaimsWorkList();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl)
                    .ShouldBeFalse("Claim with a restriction value opened should be false");

                _claimAction.ClickWorkListIcon();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                    RestrictionGroup.AllUser.GetStringValue());
                CurrentPage = _claimAction.ClickOnCreateButton();

                IsUrlUnauthorized(CurrentPage.CurrentPageUrl).ShouldBeFalse(
                    "Claim with a restriction value and unassigned access has been opened should be false");

                var authorizedClaseq = _claimAction.GetClaimSequence();
                _claimAction.IsClaimRestrictionIconPresent()
                    .ShouldBeFalse("The restriction icon should not be present for client users");
                
                _claimAction.IsClaimRestrictionIndicatorTooltipPresent("coder")
                    .ShouldBeFalse("An Icon indicating claim is coder type is present shold be false");
                _claimAction.IsClaimRestrictionIndicatorTooltipPresent("rn")
                    .ShouldBeFalse("An Icon indicating claim is rn type is present shold be false");
                _claimAction.GetCommonSql
                    .IsClaimAllowedByClaimSequenceUserIdAndAccessIncludingAll(authorizedClaseq,
                        EnvironmentManager.ClientUserName, "Offshore").ShouldBeTrue(
                        string.Format("{0} is allowed CV claim sequence for {1} user as access has been assigned",
                            authorizedClaseq, EnvironmentManager.ClientUserName));

                /* CAR 426 story starts here */
                SearchByClaimSeqFromWorkList(claimSequence);
                /* depends on script 
                    INSERT into SMTST.patient_restriction VALUES (840855 , 1); */

                /* CAR 426 story ends here */

                CurrentPage = _claimAction = CurrentPage.Logout().LoginAsClientUserWithClaimViewRestriction()
                    .NavigateToCVClaimsWorkList();
                IsUrlUnauthorized(CurrentPage.CurrentPageUrl).ShouldBeFalse(
                    "Claim with a restriction value and unassigned access has been opened should be false");

                var claseqFromMenu = _claimAction.GetClaimSequence();
                _claimAction.GetCommonSql
                    .IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(claseqFromMenu,
                        EnvironmentManager.ClientUserWithClaimViewRestriction)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for Client", claseqFromMenu));

                StringFormatter.PrintMessageTitle(
                    "This Claim is Restricted for Both User type and should not visble to restricted client User");
                _claimAction.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(claseqFromMenu,
                        EnvironmentManager.ClientUserWithClaimViewRestriction)
                    .ShouldBeFalse(string.Format("{0} is in restricted group for both", claseqFromMenu));
                _claimAction.ClickWorkListIcon();
                _claimAction.ClickSearchIcon();
                _claimAction.SetClaimSequenceInFindClaimSection(authorizedClaseq);
                _claimAction.ClickOnFindButton();
                _claimAction.GetPageErrorMessage().ShouldBeEqual(string.Format(
                        "This claim has {0} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.",
                        RestrictionGroup.AllUser.GetStringValue()),
                    "Is Poppup Message Equal?");
                _claimAction.ClosePageError();

            }
            finally
            {
                if (IsUrlUnauthorized(CurrentPage.CurrentPageUrl))
                    QuickLaunch =
                        CurrentPage.VisitAndReturnPageByUrlFoAuthorizedPage<QuickLaunchPage>(PageUrlEnum.QuickLaunch
                            .GetStringValue());
                //else
                //    CurrentPage = CurrentPage.ClickOnQuickLaunch();
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


        #endregion

        #region PRIVATE METHODS

        //private void VerifyRestrictionWarningNotification(string claseq, string message = "")
        //{
        //    _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
        //    var _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
        //    SiteDriver.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        //    _claimAction.IsClaimRestrictionIconPresent().ShouldBeFalse(message);

        //}

        private void VerifyRestrictionWarningNotification(string claseq, string restrictionType, bool shouldShowWarningNotification = true, string message = "")
        {
            _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            var _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
            _claimAction.IsClaimRestrictionIconPresent().ShouldBeFalse(message);

            if (shouldShowWarningNotification && restrictionType != null)
            {
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.IsRestrictionIconPresent().ShouldBeTrue("Is restriction icon present in Claim Search result?");
                _claimSearch.GetRestrictionIconToolTip().ShouldBeEqual(
                    $"This claim is allowed to be viewed only by users that have the {restrictionType} access.");
            }
            else
            {
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.IsRestrictionIconPresent().ShouldBeFalse("Is restriction icon present in Claim Search result?");
            }
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
            //_claimAction.ClickWorkListIcon();
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        }

        #endregion


    }
}

