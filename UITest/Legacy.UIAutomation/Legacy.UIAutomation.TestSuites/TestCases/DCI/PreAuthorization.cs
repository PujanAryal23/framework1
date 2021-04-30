using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Pre_Authorizations;
using Legacy.Service.PageServices.Pre_Authorizations.DCI;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Support.Common.Constants;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class PreAuthorization : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private PreAuthPage _preAuth;
        private SearchPage _search;
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
                return ProductEnum.DCI;
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
        public void Verify_PreAuth_aspx_page_open_when_clicked_on_PreAuthorization_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _preAuth
                .CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title", true);
            var expectedUrl = _preAuth.PageUrl;
            _preAuth
                .CurrentPageUrl.Contains(expectedUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", expectedUrl), true);
        }
        
        [Test]
        public void Verify_Clear_button_clear_every_search_field_in_Search_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            CurrentPage = _search = (SearchPage)_preAuth.ClickSearchButton();
            _search.SearchByPreAuthSequence(param["AuthSeq"]);
            _search.SearchByDob(param["Dob"]);
            _search.SearchByDocNo(param["DocNo"]);
            _search.SearchByPatientFirstName(param["FirstName"]);
            _search.SearchByPatientLastName(param["LastName"]);
            _search.SearchByPatientNumber(param["PatientNumber"]);
            _search.SearchByPatientSequence(param["PatientSequence"]);
            _search.SearchByPreAuthId(param["PreAuthId"]);
            _search.SearchByProviderName(param["ProviderName"]);
            _search.SearchByProviderNo(param["ProviderNo"]);
            _search.SearchByProviderSequence(param["ProviderSeq"]);
            _search.SearchBySsn1(param["SSN1"]);
            _search.SearchBySsn2(param["SSN2"]);
            _search.SearchBySsn3(param["SSN3"]);
            _search.SearchByTin(param["Tin"]);
            _search.SearchByInsuredSsn1(param["SSN1"]);
            _search.SearchByInsuredSsn2(param["SSN2"]);
            _search.SearchByInsuredSsn3(param["SSN3"]);
            _search.ClickSearchButton(out _hitSearchButton);
            _search.ClickClearButton();
            _search.GetPreAuthSequence().ShouldEqual("", "Auth Sequence");
            _search.GetDob().ShouldEqual("", "Date Of Birth");
            _search.GetDocNo().ShouldEqual("", "Doctor Ref Number");
            _search.GetPatientFirstName().ShouldEqual("", "Patient First Name");
            _search.GetPatientLastName().ShouldEqual("", "Patient Last Name");
            _search.GetPatientNumber().ShouldEqual("", "Patient Number");
            _search.GetPatientSequence().ShouldEqual("", "Patient Sequence");
            _search.GetAuthId().ShouldEqual("", "PreAuth Id");
            _search.GetProviderName().ShouldEqual("", "Provider Name");
            _search.GetProviderNo().ShouldEqual("", "Provider Number");
            _search.GetProviderSequence().ShouldEqual("", "Provider Sequence");
            string.Concat(_search.GetSsn1(), _search.GetSsn2(), _search.GetSsn3()).ShouldEqual("", "Social Security Number");
            _search.GetTin().ShouldEqual("", "Tin");
            _search.GetInsuredSsn().ShouldEqual("", "Insured SSN");
            if (_hitSearchButton)
            {
                _search = _search.GoBackToSamePage();
                _hitSearchButton = false;
            }
           
            CurrentPage = _preAuth = (PreAuthPage)_search.GoBack();
        }

        [Test]
        public void For_Name_field_of_patient_information_section_verify_search_results_are_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string firstName = param["FirstName"],
                   lastName = param["LastName"];
                   
            CurrentPage = _search = (SearchPage)_preAuth.ClickSearchButton();
            _search.SearchByPatientFirstName(firstName);
            _search.SearchByPatientLastName(lastName);
            _search.ClickSearchButton(out _hitSearchButton);
            
            switch (Convert.ToInt32(param["PatientNameFlag"]))
            {
                case 0:
                     _search.IsNoRecordsFoundDivPresent().ShouldBeTrue("No Records Found div");
                     Console.WriteLine("Verified that No Records Found div is present");
                    break;

                case 1:
                case 2:
                    string actualFirst =_search.GetTableResult(2, 7), // ColumnIndex 7 : Index for first name
                           actualLast = _search.GetTableResult(2, 8); // ColumnIndex 8 : Index for last name
                   (actualFirst.Contains(firstName) && actualLast.Contains(lastName))
                       .ShouldBeTrue(string.Format("'{0}' contains '{1}' \n '{2}' contains '{3}'", actualFirst, firstName, actualLast, lastName));
                    break;
            }
            if (_hitSearchButton)
            {
                _search = _search.GoBackToSamePage();
                _hitSearchButton = false;
            }
            CurrentPage = _preAuth = (PreAuthPage)_search.GoBack();
        }

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
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Consult_Required_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ConsultRequiredPage consultRequired;
            CurrentPage = consultRequired = _preAuth.ClickConsultRequiredButton();
            consultRequired
                .CurrentPageTitle.ShouldEqual(consultRequired.PageTitle, "Page Title");
            consultRequired
                .CurrentPageUrl
                .Contains(consultRequired.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", consultRequired.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)consultRequired.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Consult_Complete_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ConsultCompletePage consultComplete;
            CurrentPage = consultComplete = _preAuth.ClickConsultCompleteButton();
            consultComplete
                .CurrentPageTitle.ShouldEqual(consultComplete.PageTitle, "Page Title");
            consultComplete
                .CurrentPageUrl
                .Contains(consultComplete.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", consultComplete.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)consultComplete.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Hci_Consult_Complete_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            HCIConsultCompletePage hciConsultComplete;
            CurrentPage = hciConsultComplete = _preAuth.ClickHciConsultCompleteButton();
            hciConsultComplete
                .CurrentPageTitle.ShouldEqual(hciConsultComplete.PageTitle, "Page Title");
            hciConsultComplete
                .CurrentPageUrl
                .Contains(hciConsultComplete.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", hciConsultComplete.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)hciConsultComplete.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
        }

        [Test]
        public void Verify_ListPage_aspx_page_open_when_clicked_on_Hci_Consult_Required_button_And_verify_click_on_back_button_takes_to_PreAuthorization_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            HCIConsultRequiredPage hciConsultRequired;
            CurrentPage = hciConsultRequired = _preAuth.ClickHciConsultRequiredButton();
            hciConsultRequired
                .CurrentPageTitle.ShouldEqual(hciConsultRequired.PageTitle, "Page Title");
            hciConsultRequired
                .CurrentPageUrl
                .Contains(hciConsultRequired.PageUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", hciConsultRequired.PageUrl));
            CurrentPage = _preAuth = (PreAuthPage)hciConsultRequired.GoBack();
            _preAuth.CurrentPageTitle.ShouldEqual(_preAuth.PageTitle, "Page Title");
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

        [Test]
        public void Verify_PreAuthSearch_aspx_page_open_when_clicked_on_Search_button()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            CurrentPage = _search = (SearchPage)_preAuth.ClickSearchButton();
            _search
                .CurrentPageTitle.ShouldEqual(_search.PageTitle, "Page Title", true);
            var expectedUrl = _search.PageUrl;
            _search
                .CurrentPageUrl.Contains(expectedUrl).ShouldBeTrue(string.Format("Page Url Contains '{0}'", expectedUrl), true);
        }

        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
