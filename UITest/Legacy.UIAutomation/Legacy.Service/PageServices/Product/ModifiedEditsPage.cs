using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Default;
using  Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;

namespace Legacy.Service.PageServices.Product
{
    public class ModifiedEditsPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private ModifiedEditsPageObjects _modifiedEditsPage;

        #endregion

        #region CONSTRUCTOR

        public ModifiedEditsPage(INavigator navigator, ModifiedEditsPageObjects modifiedEditsPage)
            : base(navigator, modifiedEditsPage)
        {
            _modifiedEditsPage = (ModifiedEditsPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get modified edits window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ModifiedEditsPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<ModifiedEditsPage>(_modifiedEditsPage, out handle);
        }

        /// <summary>
        /// Click calendar link
        /// </summary>
        /// <param name="iSFrom"></param>
        /// <returns></returns>
        public void ClickOnDateCal(bool iSFrom = true)
        {
            var modEditsDateCalLnk = iSFrom
                                           ? _modifiedEditsPage.ModBegDateCalendarLink
                                           : _modifiedEditsPage.ModEndDateCalendarLink;

            ClickToOpenACalendar(modEditsDateCalLnk);
        }

        public ModifiedEditsPage GetModEditDate(out DateTime modEditDate, bool iSFrom = true)
        {
            TextField modEditTxt = iSFrom
                                       ? _modifiedEditsPage.ModBegDateTextField
                                       : _modifiedEditsPage.ModEndDateTextField;
            return GetDate<ModifiedEditsPage>(_modifiedEditsPage, modEditTxt, out modEditDate);
        }

        /// <summary>
        /// Click search button
        /// </summary>
        /// <param name="hitSearchButton"></param>
        /// <returns></returns>
        public ModifiedEditsPage ClickSearchButon(out bool hitSearchButton)
        {
            hitSearchButton = false;
            _modifiedEditsPage = Navigator.Navigate<ModifiedEditsPageObjects>(() =>
            {
                _modifiedEditsPage.SearchButton.Click();
                Console.Out.WriteLine("Click Search button.");
            });
            hitSearchButton = true;
            return new ModifiedEditsPage(Navigator, _modifiedEditsPage);
        }

        public override IPageService GoBack()
        {
            _modifiedEditsPage = Navigator.Navigate<ModifiedEditsPageObjects>(() => _modifiedEditsPage.BackButton.Click());
            return new ModifiedEditsPage(Navigator, _modifiedEditsPage);
        }

        #endregion

        #region SEARCH RESULTS

        /// <summary>
        /// Check No Matching Records Found or not.
        /// </summary>
        /// <returns></returns>
        public bool IsNomatchingRecordsFound()
        {
            return SiteDriver.IsElementPresent(ModifiedEditsPageObjects.NoMatchingRecordsXPath, How.XPath);
        }

        /// <summary>
        /// Get list of data from a specific table.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="columnIndex"></param>
        /// <param name="skipRow"></param>
        /// <returns></returns>
        public void GetRecordsList(out IList<DateTime> results, int columnIndex, int skipRow)
        {
            int totalRecords = _modifiedEditsPage.SearchResultsTbl.GetRowCount(skipRow);
            results = new List<DateTime>();
            var rnd = new Random(skipRow);
            int breakAt = 1;
            for (int i = 1; i < totalRecords; i++)
            {
                if (breakAt > 1) break;
                int nextElement = rnd.Next(i, totalRecords);
                results.Add(DateTime.Parse(SiteDriver.FindElement<TextLabel>(
                    string.Format(ModifiedEditsPageObjects.SearchResultsTblXPathTemplate, nextElement, columnIndex), How.XPath).
                    Text));
                breakAt++;
            }
        }

        #endregion
    }
}
