using System;
using System.Collections.Generic;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Diagnostics;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class ProviderProfile : NewAutomatedBase
    {
        #region PRIVATE FIELDS


        private ProviderProfilePage _providerProfile;
        //private ProviderSearchPage _providerSearch;
       
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                //CurrentPage = _providerSearch = QuickLaunch.NavigateToProviderSearch();
               
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
            //CurrentPage = _providerSearch;
        }
        protected override void TestCleanUp()
        {
            //if (_providerSearch.IsPageErrorPopupPresent())
            //    _providerSearch.CloseErrorPopup();

            //base.TestCleanUp();
            //if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            //{
            //    _providerSearch = _providerSearch.ClickOnQuickLaunch()
            //        .Logout()
            //        .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.Instance.TestClient).NavigateToProviderSearch();
               
            //}

        }
        protected override void ClassCleanUp()
        {
            try
            {
                //if (_providerProfile != null && !_providerSearch.IsProviderProfileClose())
                //{
                //    _providerProfile.CloseProviderProfileAndSwitchToProviderSearch();
                //}

                //if (CurrentPage.IsQuickLaunchIconPresent())
                //    _providerSearch.ClickOnQuickLaunch();
            }
            finally
            {
                base.ClassCleanUp();
            }
        }

        #endregion

        #region TEST SUITES

        [Test, Category("Working")] //US50625
        public void Verfiy_presence_of_respective_indicator_on_provider_profile()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName,
                TestExtensions.TestName);
            var provSeqMedReview = testData["ProviderSequenceMedReview"];
            var provSeqMed = testData["ProviderSequenceMed"];
            var provSeqReview = testData["ProviderSequenceReview"];
            var provSeq = testData["ProviderSequence"];
            var medAlert = testData["MedAlertToolTip"];
            var profileView = testData["ProfileView"];
            //var onReview = testData["OnReview"];
            try
            {
                StringFormatter.PrintMessageTitle("Provider with Red Exclamation Point");
                CheckforProfileTooltipTitle(provSeqMed, medAlert,  true);
                StringFormatter.PrintMessageTitle("Provider with Red Exclamation and Eyeball");
                CheckforProfileTooltipTitle(provSeqMedReview, medAlert,true, true);
                StringFormatter.PrintMessageTitle("Provider with Only Eyeball");
                CheckforProfileTooltipTitle(provSeqReview, profileView,false, true);
                StringFormatter.PrintMessageTitle("Provider withouth Red Exclamation and Eyeball");
                CheckforProfileTooltipTitle(provSeq, profileView, false);
            }
            finally
            {
                //if (!_providerSearch.GetPageTitle().Equals(PageTitleEnum.ProviderSearch.GetStringValue()))
                //{
                //    CurrentPage = _providerSearch = QuickLaunch.NavigateToProviderSearch();
                //}
            }
            
        }


        #endregion


        #region PRIVATE METHODS

        private void CheckforProfileTooltipTitle(string provSeq, string profileMessage, bool isMedAlert, bool isReview = false)
        {
            //const string onReview = "Provider is currently on review";
            //const string medAlert = "Medaware Alert Issued tst";
            //string providerActionMessage = (isMedAlert) ? medAlert : "View provider profile";
            //_providerSearch.SelectAllProviders();
            //_providerSearch.SearchByProviderSequence(provSeq);
            //if (_providerSearch.IsPageErrorPopupPresent()) _providerSearch.CloseErrorPopup();
            //_providerProfile = _providerSearch.ClickOnProviderIconInGridToOpenProviderProfile(1);
            //_providerProfile.GetProviderProfileIconTooltip(1)
            //    .ShouldBeEqual(profileMessage, string.Format("Tooltip with message {0} for flagged provider", profileMessage));
            //_providerProfile.GetWidgetProviderProfileIconTooltip()
            //    .ShouldBeEqual("View Provider Profile", "Tooltip message to view profile in widget present");
            //_providerProfile.GetProfileIndicatorTitle()
            //    .ShouldBeEqual(providerActionMessage, "Tooltip in provider action header as " + providerActionMessage);
            //if (isReview)
            //{
            //    _providerProfile.GetProviderProfileReviewTooltip(1)
            //        .ShouldBeEqual("Provider currently on review", "Tooltip for on review message in grid result");
            //    _providerProfile.GetWidgetProviderProfileReviewTooltip()
            //        .ShouldBeEqual(onReview, "Tooltip for on review message in widget present");
            //}
           
            //CurrentPage = QuickLaunch.NavigateToProviderSearch();
        }

      
        #endregion
    }
}
