using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Pre_Authorizations;
using Legacy.Service.PageServices.Pre_Authorizations.PCI;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Common.Constants;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class PreAuthorization : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private PreAuthPage _preAuth;
        private SearchPage _search;
        private LogicRequestsPage _logicRequests;
        private ClosedPage _closed;

        private string _calendarPopupHandle = null;
       
        private bool _hitSearchButton;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override ProductEnum TestProduct
        {
            get
            {
                return ProductEnum.PCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ProductPage = LoginPage.Login().GoToProductPage(TestProduct);
            BasePage = CurrentPage = _preAuth = ProductPage.NavigateToPreAuthorizationsPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            while (!BasePage.CurrentPageUrl.Contains(_preAuth.PageUrl))
            {
                CurrentPage.NavigateToBackPage();
                CurrentPage = _preAuth;
            }
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Unreviewed_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            UnreviewedPage unreviewed;
            CurrentPage = unreviewed = _preAuth.ClickUnreviewedButton();
            unreviewed
                .CurrentPageTitle.ShouldEqual(unreviewed.PageTitle, "Page Title");
            unreviewed
                .CurrentPageUrl
                .Contains(unreviewed.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", unreviewed.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)unreviewed.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Document_Reviewed_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            DocumentReviewPage documentReview;
            CurrentPage = documentReview = _preAuth.ClickDocumentReviewButton();
            documentReview
                .CurrentPageTitle.ShouldEqual(documentReview.PageTitle, "Page Title");
            documentReview
                .CurrentPageUrl
                .Contains(documentReview.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", documentReview.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)documentReview.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Pended_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            PendedPage pended;
            CurrentPage = pended = _preAuth.ClickPendedButton();
            pended
                .CurrentPageTitle.ShouldEqual(pended.PageTitle, "Page Title");
            pended
                .CurrentPageUrl
                .Contains(pended.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", pended.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)pended.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_PreAuth_aspx_page_open_when_clicked_on_PreAuthorization_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _preAuth
                .CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title", true);
            var expectedUrl =_preAuth.PageUrl;
            _preAuth
                .CurrentPageUrl.Contains(expectedUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", expectedUrl), true);
        }

        [Test]
        public void Verify_Clear_button_clear_every_search_field_in_Search_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            CurrentPage = _search = (SearchPage) _preAuth.ClickSearchButton();
            _search.SearchByAuthSequence(param["AuthSeq"]);
            _search.SearchByDob(param["Dob"]);
            _search.SearchByDocNo(param["DocNo"]);
            _search.SearchByPatientName(param["PatientName"]);
            _search.SearchByPatientNumber(param["PatientNumber"]);
            _search.SearchByPatientSequence(param["PatientSequence"]);
            _search.SearchByPreAuthId(param["PreAuthId"]);
            _search.SearchByProviderName(param["ProviderName"]);
            _search.SearchByProviderNo(param["ProviderNo"]);
            _search.SearchByProviderSequence(param["ProviderSeq"]);
            _search.SearchBySSN1(param["SSN1"]);
            _search.SearchBySSN2(param["SSN2"]);
            _search.SearchBySSN3(param["SSN3"]);
            _search.SearchByTin(param["Tin"]);
            _search.ClickSearchButton(out _hitSearchButton);
            _search.ClickClearButton();
            _search.GetAuthSequence().ShouldEqual("", "Auth Sequence");
            _search.GetDob().ShouldEqual("", "Date Of Birth");
            _search.GetDocNo().ShouldEqual("", "Doctor Ref Number");
            _search.GetPatientName().ShouldEqual("", "Patient Name");
            _search.GetPatientNumber().ShouldEqual("", "Patient Number");
            _search.GetPatientSequence().ShouldEqual("", "Patient Sequence");
            _search.GetPreAuthId().ShouldEqual("", "PreAuth Id");
            _search.GetProviderName().ShouldEqual("", "Provider Name");
            _search.GetProviderNo().ShouldEqual("", "Provider Number");
            _search.GetProviderSequence().ShouldEqual("", "Provider Sequence");
            string.Concat(_search.GetSSN1(), _search.GetSSN2(), _search.GetSSN3()).ShouldEqual("", "Social Security Number");
            _search.GetTin().ShouldEqual("", "Tin");
            if (_hitSearchButton)
            {
                _search = _search.GoBackToSamePage();
                _hitSearchButton = false;
            }
            CurrentPage = _preAuth = (PreAuthPage)_search.GoBack();
        }

        [Test]
        public void Verify_SearchLogic_page_opens_when_clicked_on_LogicRequests_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            LogicRequestsPage logicRequests;
            CurrentPage = logicRequests = _preAuth.ClickLogicRequestsButton();
            logicRequests.CurrentPageTitle.ShouldEqual(logicRequests.PageTitle, "Page Title ");
            logicRequests.CurrentPageUrl.AssertIsContained(logicRequests.PageUrl,"Page Url ");
            CurrentPage = _preAuth = (PreAuthPage) logicRequests.GoBack();
        }

        [Test]
        public void For_Name_field_of_patient_information_section_verify_search_results_are_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
           
            CurrentPage = _search = (SearchPage)_preAuth.ClickSearchButton();
            _search.SearchByPatientName(param["PatientName"]);
            _search.ClickSearchButton(out _hitSearchButton);
            _search.IsNoRecordsFoundDivPresent().ShouldBeTrue("No Records Found div");
            Console.WriteLine("Verified that No Records Found div is present");
            if (_hitSearchButton)
            {
                _search = _search.GoBackToSamePage();

                _hitSearchButton = false;
            }
            CurrentPage = _preAuth = (PreAuthPage) _search.GoBack();
        }

        [Test]
        public void Verify_Notify_Client_popup_appears_when_Notify_Client_button_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            LogicRequestsPage logicRequests = null;
            bool isNotifyClientPopupPresent = true;
            string notifyClientPopupHandle = null;
            NotifyClientPage notifyClient = null;
            try
            {
                CurrentPage = logicRequests = _preAuth.ClickLogicRequestsButton();
                notifyClient = logicRequests.NavigateToNotifyClient();
                notifyClient
                    .GetWindowHandle(out notifyClientPopupHandle)
                    .CurrentPageTitle
                    .ShouldEqual(notifyClient.PageTitle, "Page Title");

                notifyClient
                    .CurrentPageUrl
                    .ShouldEqual(notifyClient.PageUrl, "Page Url");

                logicRequests = notifyClient.ClickCloseButton<LogicRequestsPage>();
                isNotifyClientPopupPresent = logicRequests.IsPopupPresentWithHandleName(notifyClientPopupHandle);
                isNotifyClientPopupPresent.ShouldBeFalse("Notify Client Popup Present");
            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
            finally
            {
                if (notifyClient != null && isNotifyClientPopupPresent)
                    logicRequests.ClosePopupAndSwitchToOriginalHandle(notifyClientPopupHandle, OriginalWindowHandle);
                if (logicRequests != null) CurrentPage = _preAuth = (PreAuthPage)logicRequests.GoBack();
            }
        }

        

        [Test]
        public void Verify_Closed_aspx_page_open_when_clicked_on_Closed_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ClosedPage closed;
            StringFormatter.PrintMessageTitle("Closed Medical Pre-Authorizations");
            CurrentPage = closed = (ClosedPage)_preAuth.ClickClosedButton();
            closed
                .CurrentPageTitle.ShouldEqual(closed.PageTitle, "Page Title");
            string pageUrl = closed.PageUrl;
            closed
                .CurrentPageUrl
                .Contains(closed.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", pageUrl));
            StringFormatter.PrintLineBreak();
            CurrentPage = _preAuth = (PreAuthPage)closed.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        #endregion

        #region PRIVATE METHODS

       

        #endregion
    }
}
