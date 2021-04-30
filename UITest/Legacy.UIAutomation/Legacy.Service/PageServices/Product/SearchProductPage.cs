using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class SearchProductPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private SearchProductPageObjects _searchProductPage;

        #endregion

        #region CONSTRUCTOR

        public SearchProductPage(INavigator navigator, SearchProductPageObjects searchProductPageObjects)
            : base(navigator, searchProductPageObjects)
        {
            _searchProductPage = (SearchProductPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get search product window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SearchProductPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<SearchProductPage>(_searchProductPage,out handle);
        }

        public SearchProductPage ClickSearchButon()
        {
            _searchProductPage = Navigator.Navigate<SearchProductPageObjects>(() =>
            {
                _searchProductPage.SearchBtn.Click();
                Console.Out.WriteLine("Click Search button.");
            });
            return new SearchProductPage(Navigator, _searchProductPage);
        }

        public SearchProductPage ClickClearButon()
        {
            _searchProductPage = Navigator.Navigate<SearchProductPageObjects>(() =>
            {
                _searchProductPage.ClearBtn.Click();
                Console.Out.WriteLine("Click Clear button.");
            });
            return new SearchProductPage(Navigator, _searchProductPage);
        }

        public void ClickOnPatientDobCal()
        {
             ClickToOpenACalendar(_searchProductPage.PatientDobCalLnk);
        }

        public void ClickOnDosCal(bool iSBegin = true)
        {
            var dateOfServiceCal = iSBegin ? _searchProductPage.BeginDosCalLnk : _searchProductPage.EndDosCallLnk;
            
             ClickToOpenACalendar(dateOfServiceCal);
        }

        public SearchProductPage GetPatientDob(out DateTime patientDob)
        {
            DateTime.TryParse(_searchProductPage.PatientDobTxt.GetAttribute("value"), out patientDob);
            return GetDate<SearchProductPage>(_searchProductPage,_searchProductPage.PatientDobTxt, out patientDob);
        }

        public SearchProductPage GetDos(out DateTime dateOfService, bool iSBegin = true)
        {
            TextField dateOfServiceTxt = iSBegin ? _searchProductPage.BeginDosTxt : _searchProductPage.EndDosTxt;
            return GetDate<SearchProductPage>(_searchProductPage,dateOfServiceTxt, out dateOfService);
        }

        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
