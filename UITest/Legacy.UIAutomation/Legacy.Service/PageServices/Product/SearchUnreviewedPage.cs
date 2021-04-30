using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;

namespace Legacy.Service.PageServices.Product
{
    public class SearchUnreviewedPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private SearchUnreviewedPageObjects _searchUnreviewedPage;
        private const int StartIndex = 21;

        #endregion

        #region CONSTRUCTOR

        public SearchUnreviewedPage(INavigator navigator, SearchUnreviewedPageObjects searchUnreviewedPageObjects)
            : base(navigator, searchUnreviewedPageObjects)
        {
            _searchUnreviewedPage = (SearchUnreviewedPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            _searchUnreviewedPage = Navigator.Navigate<SearchUnreviewedPageObjects>(() => _searchUnreviewedPage.BackButton.Click());
            return new SearchUnreviewedPage(Navigator, _searchUnreviewedPage);
        }

        /// <summary>
        /// Get search unreviewed window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SearchUnreviewedPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<SearchUnreviewedPage>(_searchUnreviewedPage, out handle);
        }

        public SearchUnreviewedPage ClickSearchButon(out bool hitSearchButton)
        {
            hitSearchButton = false;
            _searchUnreviewedPage = Navigator.Navigate<SearchUnreviewedPageObjects>(() =>
            {
                _searchUnreviewedPage.SearchBtn.Click();
                Console.Out.WriteLine("Click Search button.");
            });
            hitSearchButton = true;
            return new SearchUnreviewedPage(Navigator, _searchUnreviewedPage);
        }


        public void ClickOnDateCal(bool iSFrom = true)
        {
            var clientRecvDateCalLnk = iSFrom
                                           ? _searchUnreviewedPage.FromClientRecvdDateCalLnk
                                           : _searchUnreviewedPage.ToClientRecvdDateCalLnk;

             ClickToOpenACalendar(clientRecvDateCalLnk);
        }


        public SearchUnreviewedPage GetClienRecvDate(out DateTime clientRecvDate, bool iSFrom = true)
        {
            TextField clientRecvDateTxt = iSFrom ? _searchUnreviewedPage.FromClientRecvdDateTxt : _searchUnreviewedPage.ToClientRecvdDateTxt;
            return GetDate<SearchUnreviewedPage>(_searchUnreviewedPage, clientRecvDateTxt, out clientRecvDate);
        }

        #region SEARCH RESULTS

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNomatchingRecordsFound()
        {
            return SiteDriver.IsElementPresent(SearchUnreviewedPageObjects.NoMatchingRecords, How.Id);
        }

        /// <summary>
        /// Get total number of records from a specific table.
        /// </summary>
        /// <returns></returns>
        public int GetTotalNoOfRecords()
        {
            return _searchUnreviewedPage.SearchResultsTbl.GetRowCount(StartIndex);
        }

        /// <summary>
        /// Get list of data from a specific table.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="skipRow">21 is for PCI and 19 is for DCI</param>
        /// <returns></returns>
        public List<DateTime> GetRecordsList(int columnIndex,int skipRow)
        {
            int totalRecords = _searchUnreviewedPage.SearchResultsTbl.GetRowCount(skipRow);
            int realCount = totalRecords > 0 ? skipRow + totalRecords : 0;
            var results = new List<DateTime>();
            int breakAt = 1;
            for (int i = skipRow; i < realCount; i++)
            {
                if (breakAt > 3) break;
                results.Add(DateTime.Parse(_searchUnreviewedPage.SearchResultsTbl.GetRow(i).ElementAt(columnIndex)));
                breakAt++;
            }
            return results;
        }

        #endregion

        #endregion

        #region PRIVATE METHODS

        private int GetTotalRecordsCount()
        {
            return _searchUnreviewedPage.SearchResultsTbl.GetRowCount(1);
        }

        #endregion
    }
}
