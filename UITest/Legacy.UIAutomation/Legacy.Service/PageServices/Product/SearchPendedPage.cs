using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class SearchPendedPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private SearchPendedPageObjects _searchPendedPage;
        private const int StartIndex = 21;

        #endregion

        #region CONSTRUCTOR

        public SearchPendedPage(INavigator navigator, SearchPendedPageObjects searchPendedPageObjects)
            : base(navigator, searchPendedPageObjects)
        {
            _searchPendedPage = (SearchPendedPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            _searchPendedPage = Navigator.Navigate<SearchPendedPageObjects>(() => _searchPendedPage.BackButton.Click());
            return new SearchPendedPage(Navigator, _searchPendedPage);
        }

        /// <summary>
        /// Get search unreviewed window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SearchPendedPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<SearchPendedPage>(_searchPendedPage, out handle);
        }

        public SearchPendedPage ClickSearchButon(out bool hitSearchButton)
        {
            hitSearchButton = false;
            _searchPendedPage = Navigator.Navigate<SearchPendedPageObjects>(() =>
            {
                _searchPendedPage.SearchBtn.Click();
                Console.Out.WriteLine("Click Search button.");
            });
            hitSearchButton = true;
            return new SearchPendedPage(Navigator, _searchPendedPage);
        }

        public SearchPendedPage ClickClearButton()
        {
            _searchPendedPage = Navigator.Navigate<SearchPendedPageObjects>(() =>
            {
                _searchPendedPage.ClearBtn.Click();
                Console.Out.WriteLine("Click Clear button.");
            });
            return new SearchPendedPage(Navigator, _searchPendedPage);
        }


        public void ClickOnClientRecvCal(bool iSBegin = true)
        {
            var clientRecvDateCalLnk = iSBegin
                                           ? _searchPendedPage.ClientRecvdDateBeginCalLnk
                                           : _searchPendedPage.ClientRecvdDateEndCalLnk;

             ClickToOpenACalendar(clientRecvDateCalLnk);
        }

         public void ClickOnPendedCal(bool iSBegin = true)
        {
            var pendedDateCalLnk = iSBegin
                                           ? _searchPendedPage.PendDateBeginCalLnk
                                           : _searchPendedPage.PendDateEndCalLnk;

             ClickToOpenACalendar(pendedDateCalLnk);
        }


        public SearchPendedPage GetClienRecvDate(out DateTime clientRecvDate, bool iSBegin = true)
        {
            TextField clientRecvDateTxt = iSBegin ? _searchPendedPage.ClientRecvdDateBeginTxt : _searchPendedPage.ClientRecvdDateEndTxt;
            return GetDate<SearchPendedPage>(_searchPendedPage, clientRecvDateTxt, out clientRecvDate);
        }

        public SearchPendedPage GetPendedDate(out DateTime pendedDate, bool iSBegin = true)
        {
            TextField clientRecvDateTxt = iSBegin ? _searchPendedPage.PendDateBeginTxt : _searchPendedPage.PendDateEndTxt;
            return GetDate<SearchPendedPage>(_searchPendedPage, clientRecvDateTxt, out pendedDate);
        }

        #region SEARCH RESULTS

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNomatchingRecordsFound()
        {
            return SiteDriver.IsElementPresent(SearchPendedPageObjects.NoMatchingRecords, How.Id);
        }

        /// <summary>
        /// Get total number of records from a specific table.
        /// </summary>
        /// <returns></returns>
        public int GetTotalNoOfRecords()
        {
            return GetTotalRecordsCount(StartIndex);
        }

        /// <summary>
        /// Get list of data from a specific table.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="skipRow">21 is for PCI and 19 is for DCI</param>
        /// <returns></returns>
        public List<DateTime> GetRecordsList( int columnIndex,int skipRow)
        {
            int totalRecords = GetTotalRecordsCount(skipRow);
            int realCount = totalRecords > 0 ? skipRow + totalRecords : 0;
            var results = new List<DateTime>();
            var rnd = new Random(skipRow);
            int breakAt = 1;
            for (int i = skipRow; i < realCount; i++)
            {
                if (breakAt > 3) break;
               // int nextElement = rnd.Next(i, realCount);
                results.Add(DateTime.Parse(_searchPendedPage.SearchResultsTbl.GetRow(i).ElementAt(columnIndex)));
                breakAt++;
            }

            return results;
        }

        #endregion

        #endregion

        #region PRIVATE METHODS

        private int GetTotalRecordsCount(int skiprow)
        {
            return _searchPendedPage.SearchResultsTbl.GetRowCount(skiprow);
        }

        #endregion
}
}
