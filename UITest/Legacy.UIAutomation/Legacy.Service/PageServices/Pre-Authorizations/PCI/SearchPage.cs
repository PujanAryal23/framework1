using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Pre_Authorizations.PCI;

namespace Legacy.Service.PageServices.Pre_Authorizations.PCI
{
    public class SearchPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private SearchPageObjects _searchPage;

        #endregion

        #region CONSTRUCTOR

        public SearchPage(INavigator navigator, SearchPageObjects searchPage)
            : base(navigator, searchPage)
        {
            _searchPage = (SearchPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _searchPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        public SearchPage GoBackToSamePage()
        {
            var searchPage = Navigator.Navigate<SearchPageObjects>(() => _searchPage.BackButton.Click());
            return new SearchPage(Navigator, searchPage);
        }

        public void ClickSearchButton(out bool hitSearchbutton)
        {
            _searchPage.SearchButton.Click();
            hitSearchbutton = true;
            
        }

        public void ClickClearButton()
        {
            _searchPage.ClearButton.Click();
            
        }

        public void SearchByAuthSequence(string authSeq)
        {
            _searchPage.AuthSeqTextField.SetText(authSeq);
        }

        public void SearchByDocNo(string docNo)
        {
            _searchPage.DocNoTextField.SetText(docNo);
        }

        public void SearchByPreAuthId(string preAuthId)
        {
           _searchPage.PreAuthIdTextField.SetText(preAuthId);
        }

        public void SearchByDob(string dob)
        {
            _searchPage.DobTextField.SetText(dob);
        }

        public void SearchByPatientNumber(string patNo)
        {
            _searchPage.PatNoTextField.SetText(patNo);
        }

        public void SearchByPatientName(string patName)
        {
            _searchPage.PatNameTextField.SetText(patName);
        }

        public void SearchByPatientSequence(string patSeq)
        {
            _searchPage.PatSeqTextField.SetText(patSeq);
        }

        public void SearchByProviderName(string prvName)
        {
            _searchPage.ProviderNameTextField.SetText(prvName);
        }

        public void SearchByTin(string tin)
        {
            _searchPage.TinTextField.SetText(tin);
        }

        public void SearchByProviderNo(string prvNo)
        {
            _searchPage.PrvNoTextField.SetText(prvNo);
        }

        public void SearchByProviderSequence(string prvSeq)
        {
            _searchPage.PrvSeqTextField.SetText(prvSeq);
        }

        public void SearchBySSN1(string patSSN1)
        {
            _searchPage.SSN1TextField.SetText(patSSN1);
        }

        public void SearchBySSN2(string patSSN2)
        {
            _searchPage.SSN2TextField.SetText(patSSN2);
        }

        public void SearchBySSN3(string patSSN3)
        {
            _searchPage.SSN3TextField.SetText(patSSN3);
        }

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNoRecordsFoundDivPresent()
        {
            return SiteDriver.IsElementPresent(SearchPageObjects.NoMatchingRecordsXPath, How.XPath);
        }

        public string GetAuthSequence()
        {
            return _searchPage.AuthSeqTextField.Text;
        }

        public string GetDocNo()
        {
            return _searchPage.DocNoTextField.Text;
        }

        public string GetPreAuthId()
        {
            return _searchPage.PreAuthIdTextField.Text;
        }

        public string GetDob()
        {
            return _searchPage.DobTextField.Text;
        }

        public string GetPatientNumber()
        {
            return _searchPage.PatNoTextField.Text;
        }

        public string GetPatientName()
        {
            return _searchPage.PatNameTextField.Text;
        }

        public string GetPatientSequence()
        {
            return _searchPage.PatSeqTextField.Text;
        }

        public string GetProviderName()
        {
            return _searchPage.ProviderNameTextField.Text;
        }

        public string GetProviderNo()
        {
            return _searchPage.PrvNoTextField.Text;
        }

        public string GetTin()
        {
            return _searchPage.TinTextField.Text;
        }

        public string GetProviderSequence()
        {
            return _searchPage.PrvSeqTextField.Text;
        }

        public string GetSSN1()
        {
            return _searchPage.SSN1TextField.Text;
        }

        public string GetSSN2()
        {
            return _searchPage.SSN2TextField.Text;
        }

        public string GetSSN3()
        {
            return _searchPage.SSN3TextField.Text;
        }

        #endregion
    }
}
