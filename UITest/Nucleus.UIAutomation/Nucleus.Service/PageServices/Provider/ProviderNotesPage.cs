using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderNotesPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private ProviderNotesPageObjects _providerNotesPage;

        #endregion

        #region CONSTRUCTOR

        public ProviderNotesPage(INewNavigator navigator, ProviderNotesPageObjects providerNotesPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerNotesPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerNotesPage = (ProviderNotesPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Click to collapse all notes
        /// </summary>
        /// <returns></returns>
        public void CollapseAllNotes()
        {
            if (SiteDriver.FindElementAndGetAttribute(ProviderNotesPageObjects.ExpandAllToggleId,How.Id,"Class")== "expanded")
                SiteDriver.FindElement(ProviderNotesPageObjects.ExpandAllToggleId, How.Id).Click();
        }

        /// <summary>
        /// Get Grid Record Count
        /// </summary>
        /// <returns></returns>
        public int GetGridRecordCount()
        {
            return SiteDriver.FindElementsCount(ProviderNotesPageObjects.NoteGridXPath, How.XPath);
        }

        /// <summary>
        /// Get options from Type ComboBox
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public void GetOptionsFromTypeComboBox(ref IList<string> options)
        {
            JavaScriptExecutor.ExecuteClick(ProviderNotesPageObjects.NoteTypeTextFieldId, How.Id);
            options = JavaScriptExecutor.FindElements(ProviderNotesPageObjects.NoteTypeOptionXPath, How.XPath, "Text");
        }

        /// <summary>
        /// Get options from Sub Type ComboBox
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public void GetOptionsFromSubTypeComboBox(ref IList<string> options)
        {
            JavaScriptExecutor.ExecuteClick(ProviderNotesPageObjects.SubTypeTextFieldId, How.Id);
            options = JavaScriptExecutor.FindElements(ProviderNotesPageObjects.SubTypeOptionXPath, How.XPath, "Text");
        }

        /// <summary>
        /// Get Note Type
        /// </summary>
        /// <param name="rowNum"></param>
        /// <returns></returns>
        public string GetNoteType(int rowNum)
        {
            return SiteDriver.FindElement(string.Format(ProviderNotesPageObjects.NoteTypeTemplate, rowNum), How.XPath).Text.Trim();
        }

        /// <summary>
        /// Get type value from notes page
        /// </summary>
        /// <returns></returns>
        public string GetTypeValueFromNotesPage()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.TypeComboBoxXPath, How.XPath).Text.Trim();
        }

        /// <summary>
        /// Get sub type value from notes page
        /// </summary>
        /// <returns></returns>
        public string GetSubTypeValueFromNotesPage()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.SubTypeComboBoxXPath, How.XPath).Text.Trim();
        }

       

        /// <summary>
        /// Close provider notes
        /// </summary>
        /// <param name="pageTitleEnum"></param>
        /// <returns></returns>
        public void CloseProviderNotes<T>(PageTitleEnum pageTitleEnum)
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
        }

        ///// <summary>
        ///// Close provider notes
        ///// </summary>
        ///// <param name="pageTitleEnum"></param>
        ///// <returns></returns>
        //public ProviderSearchPage CloseProviderNotesAndSwitchToProviderSearch(PageTitleEnum pageTitleEnum)
        //{
        //    var providerSearchPage = Navigator.Navigate<ProviderSearchPageObjects>(() =>
        //    {
        //        SiteDriver.CloseWindow();
        //        Console.WriteLine("Provider Note closed");
        //        SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
        //    });
        //    return new ProviderSearchPage(Navigator, providerSearchPage);
        //}

        /// <summary>
        /// Check if enter new note detail is present
        /// </summary>
        /// <returns></returns>
        public bool IsEnterNewNoteDetailsPresent()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.NewnotedetaildivId, How.Id).Displayed;
        }

        /// <summary>
        /// Get row values of grid
        /// </summary>
        /// <param name="noOfRowsToVerifyInNotesPopup"></param>
        /// <param name="randomNo"></param>
        /// <returns></returns>
        public List<string>[] GetRowValuesGrid(int noOfRowsToVerifyInNotesPopup, int randomNo)
        {
            if (!SiteDriver.IsElementPresent(ProviderNotesPageObjects.NoteGridXPath, How.XPath)) return null;
            var notesResultGrid = GetNotesResultGridId().OrderBy(x => randomNo).Take(noOfRowsToVerifyInNotesPopup).ToList();
            int noOfRecordsGrid = notesResultGrid.Count;
            var rowValuesGrid = new List<String>[noOfRecordsGrid];
            int count = 0;
            foreach (var notesRowId in notesResultGrid)
            {
                rowValuesGrid[count] = (List<string>)GetRowValues(notesRowId);
                count++;
            }
            return rowValuesGrid;
        }

        public IList<string> GetNotesResultGridId()
        {
            return SiteDriver.FindElementAndGetNotNullIdAttribute(ProviderNotesPageObjects.NoteGridXPath, How.XPath);
        }

        public IList<string> GetRowValues(string rowId)
        {
            return JavaScriptExecutor.FindElements(string.Format(ProviderNotesPageObjects.NoteGridRowValuesTemplate, rowId),
                                           How.XPath, "Text");
        }

        public string GetHeaderTextFromLabel()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.HeaderLabelXpath, How.CssSelector).Text;
        }

        public string GetValueFromLabel()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.HeaderLabelValueXpath, How.CssSelector).Text;
        }

        public void WaitUntilAddNoteDisplay()
        {
            IsEnterNewNoteDetailsPresent();
            SiteDriver.WaitForCondition(IsEnterNewNoteDetailsPresent);
            IsEnterNewNoteDetailsPresent();
        }

        public IList<string> GetNotesCreatedDateList()
        {
            return SiteDriver.FindDisplayedElementsText(ProviderNotesPageObjects.NotesCreatedDateXPath, How.XPath);
        }

        public string GetMostRecentNotesCreatedDate()
        {
            return SiteDriver.FindElement(ProviderNotesPageObjects.RecentNotesCreatedDateCssSelector, How.CssSelector).Text;
        }

        /// <summary>
        /// Get Grid Record Count
        /// </summary>
        /// <returns></returns>
        public int GetGridRecordCountBySubTyped(string SubType)
        {
            return SiteDriver.FindElementsCount(string.Format(ProviderNotesPageObjects.SubNoteTypeTemplate,SubType), How.XPath);
        }

        
        public bool IsCreateNotesSectionDisplayed()
        {
            return SiteDriver.IsElementPresent(ProviderNotesPageObjects.CreateNoteSectionCssSelector, How.CssSelector);
        }

        public string GetCreatedDateOnGrid(string SubType,int row)
        {
            return SiteDriver.FindElement(string.Format(ProviderNotesPageObjects.GridDateXPathTemplate,SubType, row), How.XPath).Text.Trim();
        }
        public string GetSavedNotes()
        {
            SiteDriver.SwitchFrameByCssLocator(ProviderNotesPageObjects.SavedNoteIframeCssLocator);
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }


        #endregion
    }
}
