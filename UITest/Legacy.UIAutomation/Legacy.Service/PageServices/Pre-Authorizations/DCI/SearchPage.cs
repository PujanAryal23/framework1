using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Pre_Authorizations.DCI;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Pre_Authorizations.DCI
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

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNoRecordsFoundDivPresent()
        {
            return SiteDriver.IsElementPresent(SearchPageObjects.NoMatchingRecordsXPath, How.XPath);
        }

        public string GetTableResult(int rowIndex, int columnIndex)
        {
            return
                SiteDriver.FindElement<TextLabel>(
                    string.Format(SearchPageObjects.TableXpathTemplate, rowIndex, columnIndex), How.XPath).Text;
        }

        public SearchPage GoBackToSamePage()
        {
            var searchPage = Navigator.Navigate<SearchPageObjects>(() => _searchPage.BackButton.Click());
            return new SearchPage(Navigator, searchPage);
        }

        public void ClickClearButton()
        {
            _searchPage.ClearButton.Click();
             
        }

        public void ClickSearchButton(out bool hitSearchbutton)
        {
            _searchPage.SearchButton.Click();
            hitSearchbutton = true;
            
        }

        public void SearchByInsuredSsn1(string ssn)
        {
            _searchPage.InsuredSsn1TextField.SetText(ssn);
        }

        public void SearchByInsuredSsn2(string ssn)
        {
            _searchPage.InsuredSsn2TextField.SetText(ssn);
        }

        public void SearchByInsuredSsn3(string ssn)
        {
            _searchPage.InsuredSsn3TextField.SetText(ssn);
        }

        public void SearchByPreAuthSequence(string authSeq)
        {
            _searchPage.PreAuthSeqTextField.SetText(authSeq);
        }

        public void SearchByDocNo(string docNo)
        {
            _searchPage.DocNumberTextField.SetText(docNo);
        }

        public void SearchByPreAuthId(string preAuthId)
        {
            _searchPage.AuthIdTextField.SetText(preAuthId);
        }

        public void SearchByDob(string dob)
        {
            _searchPage.PatDobTextField.SetText(dob);
        }

        public void SearchByPatientNumber(string patNo)
        {
            _searchPage.PatNumberTextField.SetText(patNo);
        }

        public void SearchByPatientFirstName(string patName)
        {
            _searchPage.PatFirstTextField.SetText(patName);
        }

        public void SearchByPatientLastName(string patName)
        {
            _searchPage.PatLastTextField.SetText(patName);
        }

        public void SearchByPatientSequence(string patSeq)
        {
            _searchPage.PatSequenceTextField.SetText(patSeq);
        }

        public void SearchByProviderName(string prvName)
        {
            _searchPage.PrvNameTextField.SetText(prvName);
        }

        public void SearchByTin(string tin)
        {
            _searchPage.TinTextField.SetText(tin);
        }

        public void SearchByProviderNo(string prvNo)
        {
            _searchPage.PrvNumberTextField.SetText(prvNo);
        }

        public void SearchByProviderSequence(string prvSeq)
        {
            _searchPage.PrvSequenceTextField.SetText(prvSeq);
        }

        public void SearchBySsn1(string patSsn1)
        {
            _searchPage.Ssn1TextField.SetText(patSsn1);
        }

        public void SearchBySsn2(string patSsn2)
        {
            _searchPage.Ssn2TextField.SetText(patSsn2);
        }

        public void SearchBySsn3(string patSsn3)
        {
            _searchPage.Ssn3TextField.SetText(patSsn3);
        }

        public string GetPreAuthSequence()
        {
            return _searchPage.PreAuthSeqTextField.Text;
        }

        public string GetInsuredSsn()
        {
            return (_searchPage.InsuredSsn1TextField.Text +
                _searchPage.InsuredSsn2TextField.Text +
                    _searchPage.InsuredSsn3TextField.Text);
        }

        public string GetDocNo()
        {
            return _searchPage.DocNumberTextField.Text;
        }

        public string GetAuthId()
        {
            return _searchPage.AuthIdTextField.Text;
        }

        public string GetDob()
        {
            return _searchPage.PatDobTextField.Text;
        }

        public string GetPatientNumber()
        {
            return _searchPage.PatNumberTextField.Text;
        }

        public string GetPatientFirstName()
        {
            return _searchPage.PatFirstTextField.Text;
        }

        public string GetPatientLastName()
        {
            return _searchPage.PatLastTextField.Text;
        }

        public string GetPatientSequence()
        {
            return _searchPage.PatSequenceTextField.Text;
        }

        public string GetProviderName()
        {
            return _searchPage.PrvNameTextField.Text;
        }

        public string GetProviderNo()
        {
            return _searchPage.PrvNumberTextField.Text;
        }

        public string GetTin()
        {
            return _searchPage.TinTextField.Text;
        }

        public string GetProviderSequence()
        {
            return _searchPage.PrvSequenceTextField.Text;
        }

        public string GetSsn1()
        {
            return _searchPage.Ssn1TextField.Text;
        }

        public string GetSsn2()
        {
            return _searchPage.Ssn2TextField.Text;
        }

        public string GetSsn3()
        {
            return _searchPage.Ssn1TextField.Text;
        }

        #endregion
    }
}
