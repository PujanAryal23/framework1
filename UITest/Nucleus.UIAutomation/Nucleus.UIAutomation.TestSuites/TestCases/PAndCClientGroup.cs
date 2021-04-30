using System.Collections.Generic;
using System.Diagnostics;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    public class PAndCClientGroup : NewAutomatedBase
    {

        private string _pandcClient = string.Empty;
        private BillActionPage _billAction;
        //private ProviderSearchPage _providerSearch;
      
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
                base.ClassInit();
                _pandcClient = DataHelper.GetSingleTestData(FullyQualifiedClassName, "TestClient",
                                                    "PAndClient", "Value");
                QuickLaunch.ClickOnSwitchClient().SwitchClientTo((ClientEnum)Enum.Parse(typeof(ClientEnum), _pandcClient));

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
            try
            {
                base.TestCleanUp();
                
               
            }
            catch (InvalidOperationException ex)
            {
                var proc = Process.GetProcessesByName("IEXPLORE");
                if (string.Compare(proc[1].ProcessName, "IEXPLORE", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    proc[1].Kill();
                    StringFormatter.PrintMessage("Stop running this script ?");
                }
                TestExtensions.ThrowAssertionException(string.Empty, true, ex);
            }

        }

        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTest"),Category("IENonCompatible")]
        public void Verify_menu_options_for_PandC_client_Bill_should_appear_instead_of_Claim()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            bool fciworklistauthorizedFlag = Convert.ToBoolean(paramLists["FCIWorkListFlag"]);
            bool pciworklistauthorizedFlag = Convert.ToBoolean(paramLists["PCIWorkListFlag"]);
            _billAction = QuickLaunch.NavigateToPciBillWorkList();
            _billAction.HandleAutomaticallyOpenedPatientBillHistoryPopup();
            _billAction.GetMenuOption(1).ShouldBeEqual("Bill", "Header menu text for P and C client");
            _billAction.MouseOverBillMenu();
            var actualList = _billAction.GetAllPrimaryAndSecondarySubMenuListByHeaderMenu("Bill");
            actualList.ShouldContain("Bill Search", "1st submenu");
            actualList.ShouldContain("Bill Search (new)", "2nd submenu");
            actualList.ShouldContain("Bill Line Search", "3rd  submenu");
            actualList.ShouldContain("Logic Search", "4th submenu");
            if (fciworklistauthorizedFlag) actualList.ShouldContain("FCI Bills", "5th submenu");
            if (pciworklistauthorizedFlag) actualList.ShouldContain("CV Bills", "6th submenu");
            
        }

        [Test]
        public void If_client_type_is_PAndC_Bill_should_appear_instead_of_Claim()
        {
            _billAction = QuickLaunch.NavigateToPciBillWorkList();
            _billAction.HandleAutomaticallyOpenedPatientBillHistoryPopup();
            _billAction.GetPageHeader().ShouldBeEqual("Bill Action", "Page header text");
            _billAction.GetClaSeqTextLabel().ShouldBeEqual("Bill Seq", "Claseq text");
            
        }


        [Test,Category("Working")]
        public void Verify_pandc_claim_number_column_is_displayed_on_Provider_Bill_History_for_PAndC_client()
        {
            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //_providerSearch = QuickLaunch.NavigateToProviderSearch();
            //_providerSearch.SelectCotivitiFlaggedProviders();
            //ProviderBillHistoryPage providerBillHistoryPage = null;
            //try
            //{
            //    providerBillHistoryPage =
            //        _providerSearch.ClickOnGridHistoryIconToOpenProviderBillHistoryPopup(1);
            //    providerBillHistoryPage.ScrollToLastColumn();
            //    providerBillHistoryPage.IsExtraClaimNoColumnDisplayed().ShouldBeTrue("Extra Claim No Column Present?");
            //}
            //finally
            //{
            //    if (providerBillHistoryPage != null)
            //        providerBillHistoryPage.CloseProviderBillHistoryAndSwitchToProviderSearch();
            //}

        }

        #endregion
    }
}
