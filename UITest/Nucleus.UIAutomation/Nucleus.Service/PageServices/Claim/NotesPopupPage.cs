using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Claim
{
    public class NotesPopupPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private NotesPopupPageObjects _notesPopupPage;

        #endregion

        #region CONSTRUCTOR

        public NotesPopupPage(INewNavigator navigator, NotesPopupPageObjects notesPopupPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor
        )
            : base(navigator, notesPopupPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _notesPopupPage = (NotesPopupPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get options from Type ComboBox
        /// </summary>
        /// <returns></returns>
        public IList<string> GetOptionsFromTypeComboBox()
        {
            SiteDriver.FindElement(NotesPopupPageObjects.NoteTypeComboBoxId, How.Id).Click();
            return JavaScriptExecutor.FindElements(NotesPopupPageObjects.NoteTypeOptionXPath, How.XPath, "Text");
        }

        /// <summary>
        /// Get Grid Record Count
        /// </summary>
        /// <returns></returns>
        public int GetGridRecordCount()
        {
           return SiteDriver.FindElementsCount(NotesPopupPageObjects.NotesGrid, How.XPath);
        }

        /// <summary>
        /// Get total number of rows
        /// </summary>
        public int GetRowCount()
        {
            return SiteDriver.FindElementsCount(NotesPopupPageObjects.TblResultGridRowId, How.Id) - 1;
        }

        /// <summary>
        /// Click to collapse all notes
        /// </summary>
        /// <returns></returns>
        public void CollapseAllNotes()
        {
            if (SiteDriver.FindElement(NotesPopupPageObjects.ExpandAllToggleId, How.Id).GetAttribute("Class") == "expanded")
                SiteDriver.FindElement(NotesPopupPageObjects.ExpandAllToggleId, How.Id).Click();
        }

        /// <summary>
        /// Get Note Type
        /// </summary>
        /// <param name="rowNum"></param>
        /// <returns></returns>
        public string GetNoteType(int rowNum)
        {
          return SiteDriver.FindElement(string.Format(NotesPopupPageObjects.NoteTypeTemplate, rowNum), How.XPath).Text.Trim();
        }

        public void ClosePopupNoteAndSwitchToNewClaimActionPage()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
        }

        public string GetHeaderTextFromLabel()
        {
            return _notesPopupPage.HeaderLabel.Text;
        }

        public string GetValueFromLabel()
        {
          return _notesPopupPage.HeaderLabelValue.Text;
        }

        public string GetNoteTypeSelectedOptionValue()
        {
            return _notesPopupPage.NoteTypeSelectedOption.Text;
        }

        #endregion
    }
}