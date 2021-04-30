using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Pre_Authorizations
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

        public SearchPage ClickSearchButton(out bool hitSearchbutton)
        {
            SiteDriver.FindElement<ImageButton>(SearchPageObjects.SearchXPath, How.XPath).Click();
            hitSearchbutton = true;
            SiteDriver.WaitToLoad(2000);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage ClickClearButton()
        {
            _searchPage.ClearButton.Click();
            SiteDriver.WaitToLoad(2000);
            return new SearchPage(Navigator, _searchPage);  
        }

        #region PCI

        public SearchPage SearchByAuthSequence(string authSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.AuthSeqName, How.Name).SetText(authSeq);
            return  new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDocNo(string docNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DocNoName, How.Name).SetText(docNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPreAuthId(string preAuthId)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PreAuthIdName, How.Name).SetText(preAuthId);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDob(string dob)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DobName, How.Name).SetText(dob);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPatientNumber(string patNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatNoName, How.Name).SetText(patNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPatientName(string patName)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatNameName, How.Name).SetText(patName);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPatientSequence(string patSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatSeqName, How.Name).SetText(patSeq);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByProviderName(string prvName)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.ProviderNameName, How.Name).SetText(prvName);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByTin(string tin)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.TinName, How.Name).SetText(tin);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByProviderNo(string prvNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNoName, How.Name).SetText(prvNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByProviderSequence(string prvSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PrvSeqName, How.Name).SetText(prvSeq);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchBySSN1(string patSSN1)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.SSN1Name, How.Name).SetText(patSSN1);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchBySSN2(string patSSN2)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.SSN2Name, How.Name).SetText(patSSN2);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchBySSN3(string patSSN3)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.SSN3Name, How.Name).SetText(patSSN3);
            return new SearchPage(Navigator, _searchPage);
        }

        public string GetAuthSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.AuthSeqName, How.Name).Text;
        }

        public string GetDocNo()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DocNoName, How.Name).Text;
        }

        public string GetPreAuthId()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PreAuthIdName, How.Name).Text;
        }

        public string GetDob()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DobName, How.Name).Text;
        }

        public string GetPatientNumber()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatNoName, How.Name).Text;
        }

        public string GetPatientName()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatNameName, How.Name).Text;
        }

        public string GetPatientSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatSeqName, How.Name).Text;
        }

        public string GetProviderName()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.ProviderNameName, How.Name).Text;
        }

        public string GetProviderNo()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNoName, How.Name).Text;
        }

        public string GetTin()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.TinName, How.Name).Text;
        }

        public string GetProviderSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PrvSeqName, How.Name).Text;
        }

        public string GetSSN1()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.SSN1Name, How.Name).Text;
        }

        public string GetSSN2()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.SSN2Name, How.Name).Text;
        }

        public string GetSSN3()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.SSN3Name, How.Name).Text;
        }
        #endregion

        #region DCI

        public SearchPage ClickDciSearchButton(out bool hitSearchbutton)
        {
            SiteDriver.FindElement<ImageButton>(SearchPageObjects.DciSearchName, How.Name).Click();
            hitSearchbutton = true;
            SiteDriver.WaitToLoad(2000);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByInsuredSsn1(string ssn)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn1, How.Name).SetText(ssn);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByInsuredSsn2(string ssn)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn2, How.Name).SetText(ssn);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByInsuredSsn3(string ssn)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn3, How.Name).SetText(ssn);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciPreAuthSequence(string authSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PreAuthSeqName, How.Name).SetText(authSeq);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciDocNo(string docNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DocNumberName, How.Name).SetText(docNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciPreAuthId(string preAuthId)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciAuthIdName, How.Name).SetText(preAuthId);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciDob(string dob)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatDobName, How.Name).SetText(dob);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciPatientNumber(string patNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatNumberName, How.Name).SetText(patNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPatientFirstName(string patName)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatFirstName, How.Name).SetText(patName);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByPatientLastName(string patName)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatLastName, How.Name).SetText(patName);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciPatientSequence(string patSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PatSequenceName, How.Name).SetText(patSeq);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciProviderName(string prvName)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNameName, How.Name).SetText(prvName);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciTin(string tin)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciTinName, How.Name).SetText(tin);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciProviderNo(string prvNo)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNumberName, How.Name).SetText(prvNo);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciProviderSequence(string prvSeq)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.PrvSequenceName, How.Name).SetText(prvSeq);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciSSN1(string patSSN1)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN1Name, How.Name).SetText(patSSN1);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciSSN2(string patSSN2)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN2Name, How.Name).SetText(patSSN2);
            return new SearchPage(Navigator, _searchPage);
        }

        public SearchPage SearchByDciSSN3(string patSSN3)
        {
            SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN3Name, How.Name).SetText(patSSN3);
            return new SearchPage(Navigator, _searchPage);
        }

        public string GetDciPreAuthSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PreAuthSeqName, How.Name).Text;
        }

        public string GetInsuredSsn()
        {
            return (SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn1, How.Name).Text +
                SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn2, How.Name).Text +
                    SiteDriver.FindElement<TextField>(SearchPageObjects.DciInsuredSsn3, How.Name).Text);
        }

        public string GetDciDocNo()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DocNumberName, How.Name).Text;
        }

        public string GetDciAuthId()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DciAuthIdName, How.Name).Text;
        }

        public string GetDciDob()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatDobName, How.Name).Text;
        }

        public string GetDciPatientNumber()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatNumberName, How.Name).Text;
        }

        public string GetDciPatientFirstName()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatFirstName, How.Name).Text;
        }

        public string GetDciPatientLastName()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatLastName, How.Name).Text;
        }

        public string GetDciPatientSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PatSequenceName, How.Name).Text;
        }

        public string GetDciProviderName()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNameName, How.Name).Text;
        }

        public string GetDciProviderNo()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PrvNumberName, How.Name).Text;
        }

        public string GetDciTin()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DciTinName, How.Name).Text;
        }

        public string GetDciProviderSequence()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.PrvSequenceName, How.Name).Text;
        }

        public string GetDciSSN1()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN1Name, How.Name).Text;
        }

        public string GetDciSSN2()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN2Name, How.Name).Text;
        }

        public string GetDciSSN3()
        {
            return SiteDriver.FindElement<TextField>(SearchPageObjects.DciSSN3Name, How.Name).Text;
        }

        #endregion

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNoRecordsFoundDivPresent()
        {
            return SiteDriver.IsElementPresent(SearchPageObjects.NoMatchingRecordsXPath, How.XPath);
        }

        #endregion
    }
}
