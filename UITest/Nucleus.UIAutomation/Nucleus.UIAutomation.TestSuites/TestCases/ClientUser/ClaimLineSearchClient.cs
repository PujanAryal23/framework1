using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class ClaimLineSearchClient : AutomatedBaseClient
    {

        #region PRIVATE FIELDS

        private ClaimLineSearchPage _claimLineSearch;
        private IDictionary<string, string> _quickSearchOptionsForClientUser;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE MEHTODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _claimLineSearch = QuickLaunch.NavigateToClaimLineSearch();
                _quickSearchOptionsForClientUser = DataHelper.GetMappingData(FullyQualifiedClassName,
                                                                             "quick_search_options_for_client_user");
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        #endregion

        #region TEST SUITES


        [Test]
        public void Verify_list_of_quickSearch_options_for_client_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimLineSearch.GetQuickSearchOptions().ShouldCollectionBeEqual(_quickSearchOptionsForClientUser.Values.ToList(), "QuickSearch Options");
        }

        #endregion

    }
}
