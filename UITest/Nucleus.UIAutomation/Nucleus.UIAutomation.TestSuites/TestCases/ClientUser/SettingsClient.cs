using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Utils;
using Nucleus.Service.Support.Common.Constants;
using UIAutomation.Framework.Core.Driver;


namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    class SettingsClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private ClaimActionPage _claimAction;
        private ClaimSearchPage _claimSearch;
        private OldUserProfileSearchPage _userProfileSearch;
        private ProfileManagerPage _profileManager;


        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE METHODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
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

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = CurrentPage.Logout().LoginAsClientUser();
            }
            else 
            {
                CurrentPage.ClickOnQuickLaunch();
            }
            
            QuickLaunch.ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);


            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

      
        [Test]//US25238
        public void Verify_that_if_enable_quick_delete_is_selected_to_yes_option_the_quick_delete_icons_at_header_line_and_flag_level_will_all_be_displayed_and_operate_as_usual_for_client_user()
        {
            _claimAction = QuickLaunch.NavigateToCVClaimsWorkList();
            _claimAction.IsDeleteAllFlagsPresent().ShouldBeTrue("Delete Icon at header is present ");
            _claimAction.IsDeleteFlagsIconPresent().ShouldBeTrue("Delete Icon at flag is present");
            _claimAction.IsDeleteLineIconPresent().ShouldBeTrue("Delete Icon at line is present");

        }

        [Test,Category("SchemaDependent")]//US25238
        public void Verify_that_if_enable_quick_delete_is_selected_to_no_option_the_quick_delete_icons_at_header_line_and_flag_level_will_not_be_displayed_and_transfer_and_logic_icons_will_be_moved_to_left_for_client_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];

            IDictionary<string, string> classDictionary = DataHelper.GetMappingData(FullyQualifiedClassName, "default_class_values_for_transfer_and_logic_icons");
            string transferClass = classDictionary["TransferIconSpanClass"];
            string logicClass = classDictionary["LogicIconSpanClass"];

            CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);

            _claimSearch = QuickLaunch.NavigateToClaimSearch();
            _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
            _claimAction.IsDeleteAllFlagsPresent().ShouldBeFalse("Delete Icon at header is present ");
            _claimAction.IsDeleteFlagsIconPresent().ShouldBeFalse("Delete Icon at flag is present");
            _claimAction.IsDeleteLineIconPresent().ShouldBeFalse("Delete Icon at line is present");

            _claimAction.GetTransferIconTooltipMovedToLeft().ShouldBeEqual(transferClass);
            _claimAction.GetLogicIconMovedToLeft().ShouldBeEqual(logicClass);

        }

        [Test, Category("SmokeTest")] //CAR-2902(CAR-2744) + CAR-2914(CAR-2837)
        public void Verify_User_Name_Greeting_Menu_And_Dropdown()
        {
            var dropDownListOptions = Enum.GetValues(typeof(UserGreetingMenuEnum))
                .Cast<UserGreetingMenuEnum>().Select(x => x.GetStringValue()).ToList();
            var quickLinksMenuOptions = Enum.GetValues(typeof(QuickLinksEnum))
                .Cast<QuickLinksEnum>().Select(x => x.GetStringValue()).ToList();
            var cotivitiLinksMenuOptions = Enum.GetValues(typeof(CotivitiLinksEnum))
                .Cast<CotivitiLinksEnum>().Select(x => x.GetStringValue()).ToList();
            quickLinksMenuOptions.RemoveAt(2);
            CurrentPage.IsUserNameGreetingMenuPresent().ShouldBeTrue("User name Greeting Menu Should be present in the right corner of the page");
            StringFormatter.PrintMessage("Verify Dropdown label");
            CurrentPage.GetUserNameGreetingMenuDropdownLabel().ShouldBeEqual($"Hi, {CurrentPage.GetLoggedInUserFullName().Split(' ')[0]}");
            StringFormatter.PrintMessage("Verify Presence of Drop Down Options");
            CurrentPage.GetUserNameGreetingMenuDropdownOptions().ShouldCollectionBeEqual(dropDownListOptions, "Drop Down list Options Should Match");
            CurrentPage.GetQuickLinksMenuOptions().ShouldCollectionBeEqual(quickLinksMenuOptions, "Quick links list should match");
            CurrentPage.GetCotivitiLinksMenuOptions().ShouldCollectionBeEqual(cotivitiLinksMenuOptions, "Cotiviti links list should match");
            StringFormatter.PrintMessage("Verify Only Claim Search Menu is visible and Basic dropdown is visible to users without any privilege");
            CurrentPage.Logout().LoginAsClientUserWithNoAnyAuthority();
            dropDownListOptions.Remove("Dashboard");
            CurrentPage.GetTopNavigationMenuOptions().ShouldCollectionBeEqual(new List<string>() { "Claim" }, "Only Claim Search Menu should be present for user without any authority");
            CurrentPage.GetUserNameGreetingMenuDropdownOptions().ShouldCollectionBeEqual(dropDownListOptions, "Basic dropdown option present for users without any authority");
            StringFormatter.PrintMessage("Verify Client Flagged providers is not present for user without Provider Maintenance authority");
            CurrentPage.MouseOverQuickLinks();
            CurrentPage.GetQuickLinksMenuOptions().ShouldNotContain(QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue(), $"List should not contain {QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue()}");

        }



        //[Test,Category("CommonTableDependent")]//US67923
        //public void Verify_security_answers_are_displayed_for_client_users()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    ProfileManagerPage _profileManager = QuickLaunch.NavigateToProfileManager();
        //    CurrentPage = _profileManager;
        //    _profileManager.RestoreSecurityAnswersInDatabase(EnvironmentManager.Username);
        //    _profileManager.RefreshPage();            
        //    _profileManager.ClickOnSecurityTab();
        //    _profileManager.IsShowAnswer1Visible().ShouldBeTrue("Is Show Answer1 text visible?");
        //    _profileManager.IsShowAnswer2Visible().ShouldBeTrue("Is Show Answer2 text visible ? ");
        //    _profileManager.IsSecurityAnswer1Enabled().ShouldBeTrue("Is Security Answer1 textbox enabled?");
        //    _profileManager.IsSecurityAnswer2Enabled().ShouldBeTrue("Is Security Answer2 textbox enabled?");
        //    _profileManager.GetSecurityAnswer1().ShouldBeNullorEmpty("Initially Answer1 should not be displayed.");
        //    _profileManager.GetSecurityAnswer2().ShouldBeNullorEmpty("Initially Answer2 should not be displayed.");
        //    _profileManager.ClickOnShowAnswer1();
        //    _profileManager.GetSecurityAnswer1().ShouldNotBeEmpty("Client user should be able to view their own security answer1");
        //    _profileManager.ClickOnShowAnswer2();
        //    _profileManager.GetSecurityAnswer2().ShouldNotBeEmpty("Client user should be able to view their own security answer1");

        //    StringFormatter.PrintMessage("Update Own Security Answers");
        //    _profileManager.SetSecurityAnswer1("k");
        //    _profileManager.SetSecurityAnswer2("k");
        //    _profileManager.ClickSaveButton();
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnSecurityTab();
        //    _profileManager.ClickOnShowAnswer1();
        //    _profileManager.GetSecurityAnswer1().ShouldBeEqual("k", "Is Security Answer1 is equal?");
        //    _profileManager.ClickOnShowAnswer2();
        //    _profileManager.GetSecurityAnswer2().ShouldBeEqual("k", "Is Security Answer2 is equal?");
        //}

        //[Test] //TE-337
        //public void Verify_subscriberid_field_in_UserProfile_for_client_User()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    IDictionary<string, string> paramLists =
        //        DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
        //    ProfileManagerPage _profileManager = QuickLaunch.NavigateToProfileManager();

        //    _profileManager.RemoveSubscriberIdFromDatabase(EnvironmentManager.Username);
        //    SiteDriver.Refresh();
        //    StringFormatter.PrintMessage("verify subscriber id field present");
        //    _profileManager.IsSubscriberIdpresent().ShouldBeTrue("Is Subscriber Id present");
        //    _profileManager.GetSubscriberIdValue().ShouldBeNullorEmpty("Subscriber Id field value empty?");
        //    _profileManager.SetValueforSubscriberField(paramLists["SubscribeId"].Split(',')[0]);
        //    _profileManager.ClickSaveButton(true);
        //    _profileManager.GetSubscriberIdFromDatabase(EnvironmentManager.Username).ShouldBeEqual(
        //        paramLists["SubscribeId"].Split(',')[0],
        //        "verify correct data saved in database");
        //    StringFormatter.PrintMessage("verify edit of subscriber id field");
        //    _profileManager = CurrentPage.NavigateToProfileManager();
        //    _profileManager.GetSubscriberIdValue().ShouldBeEqual(paramLists["SubscribeId"].Split(',')[0],
        //        "Is subscriber id value valid?");
        //    _profileManager.SetValueforSubscriberField(paramLists["SubscribeId"].Split(',')[1]);
        //    _profileManager.ClickSaveButton(true);
        //    _profileManager = CurrentPage.NavigateToProfileManager();
        //    StringFormatter.PrintMessage("verify subscriber id is not a mandatory field");

        //    _profileManager.GetSubscriberIdValue().ShouldBeEqual(paramLists["SubscribeId"].Split(',')[1],
        //        "Is subscriber id value valid for new entry?");
        //    _profileManager.ClearSubscriberIdInputValue();
        //    _profileManager.SetValueforSubscriberField(string.Concat(Enumerable.Repeat("a1@_B", 21)));
        //    _profileManager.GetSubscriberIdValue().Length.ShouldBeEqual(100, "Maximum allowed value 100?");
        //    _profileManager.ClearSubscriberIdInputValue();
        //    _profileManager.ClickSaveButton(true);


        //}

        //[Test, Category("SchemaDependent")]//TE-347
        //public void Verify_Batch_Search_Option_Displayed_For_Client_User_With_NonBatch_Default_client()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    IDictionary<string, string> paramLists =
        //        DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

        //    var _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.GetProcessingTypeForClientfromDatabase(ClientEnum.SMTST.ToString()).ShouldBeEqual("B", "Batch client");
        //    _profileManager.GetProcessingTypeForClientfromDatabase(ClientEnum.RPE.ToString()).ShouldBeEqual("R", " Non Batch client?");
        //    _profileManager.SelectDefaultClient(paramLists["client"].Split(',').ToList()[0]);
        //    _profileManager.GetDefaultPageList().ShouldNotContain("Batch Search", "Is Batch search option Visible?");
        //    _profileManager.SelectDefaultClient(paramLists["client"].Split(',').ToList()[1]);
        //    _profileManager.GetDefaultPageList().ShouldContain("Batch Search", "Is Batch search option Visible?");
        //}

        //[Test] //TE-433

        //public void Verify_Default_Page_As_SuspectProvider_Page_For_Client_users()
        //{
        //    QuickLaunch.Logout().LoginClientUserWithDefaultSuspectProvidersPage();
        //    CurrentPage.WaitForPageToLoad();
        //    var suspectProvider = EnvironmentManager.Instance.ApplicationUrl +
        //                          PageUrlEnum.SuspectProviders.GetStringValue();
        //    CurrentPage.CurrentPageUrl.ShouldBeEqual(suspectProvider, "Page Url Should be Suspect Provider");
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),"Page Header should match");
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.WaitForPageToLoad();
        //    _profileManager.GetDefaultPagePreference().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),
        //        "default page set to suspect Provider?");


        //}

        //[Test] //TE-433

        //public void Verify_Default_Page_As_ProviderSearch_Page_For_Client_users()
        //{
        //    QuickLaunch.Logout().LoginClientUserwithdefaultProviderSearchPage();
        //    CurrentPage.WaitForPageToLoad();
        //    var provider = EnvironmentManager.Instance.ApplicationUrl +
        //                   PageUrlEnum.ProviderSearch.GetStringValue();
        //    CurrentPage.CurrentPageUrl.ShouldBeEqual(provider, "Page Url Should be Provider Search ");
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(), "Page Header should match");
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.WaitForPageToLoad();
        //    _profileManager.GetDefaultPagePreference().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
        //        "default page set to  Provider?");


        //}

        #endregion
    }
}
