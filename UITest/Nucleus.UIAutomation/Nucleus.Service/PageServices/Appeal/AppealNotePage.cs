using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealNotePage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private AppealNotePageObjects _appealNotePage;

        #endregion

        #region CONSTRUCTOR

        public AppealNotePage(INewNavigator navigator, AppealNotePageObjects appealNotePage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealNotePage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealNotePage = (AppealNotePageObjects)PageObject;
            WaitForLoadingImageIcon();
        }

        #endregion

        #region PUBLIC METHODS

        public void WaitForLoadingImageIcon()
        {
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(AppealNotePageObjects.LoadingImageIconCssLocator,How.CssSelector));
        }

        public AppealSummaryPage SwitchBackToAppealSummaryPage()
        {
            var newClaimActionPageObject = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
                StringFormatter.PrintMessage("Switch back to New Claim Action Page.");
            });
            return new AppealSummaryPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public int CountWindowHandles(string title)
        {
            return SiteDriver.WindowHandles.Count(handle => SiteDriver.GetHandleByTitle(title).Equals(handle));
        }
        
        
        /// <summary>
        /// Close note popup page
        /// </summary>
        public AppealActionPage CloseNotePopUpPageSwitchToAppealAction()
        {
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealAction));
            });
            Console.WriteLine("Appeal note pop up closed");
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Close note popup page and back to appeal summary page
        /// </summary>
        public AppealSummaryPage CloseNotePopUpPageAndBackToAppealSummary()
        {
            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealNotes.GetStringValue());
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealSummary.GetStringValue());
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        /// <summary>
        /// Get options from type ComboBox
        /// </summary>
        /// <returns></returns>
        public IList<string> GetOptionsFromTypeComboBox()
        {
            JavaScriptExecutor.ExecuteClick(AppealNotePageObjects.NoteTypeTextFieldId,How.Id);
            return JavaScriptExecutor.FindElements(AppealNotePageObjects.NoteTypeOptionXPath, How.XPath, "Text");
        }

        public string GetnoteTypeValue()
        {
            
            return SiteDriver.FindElement(AppealNotePageObjects.NoteTypeValueCssSelector, How.CssSelector).GetAttribute("Value");
        }

        public string GetnoteTypeValueForNewNote()
        {
            SiteDriver.WaitForCondition(
                () => SiteDriver.IsElementPresent(AppealNotePageObjects.NoteTypeValueForNewNoteCssSelector,
                    How.CssSelector), 5000);
            return SiteDriver.FindElement(AppealNotePageObjects.NoteTypeValueForNewNoteCssSelector, How.CssSelector).Text;
        }

        /// <summary>
        /// Get appeal sequence from label
        /// </summary>
        /// <returns></returns>
        public string GetAppealSeqFromLable()
        {
            return SiteDriver.FindElement(AppealNotePageObjects.HeaderLabelXpath, How.CssSelector).Text;
        }

        /// <summary>
        /// Get value from label
        /// </summary>
        /// <returns></returns>
        public string GetValueFromLabel()
        {
            return SiteDriver.FindElement(AppealNotePageObjects.HeaderLabelValueXpath, How.CssSelector).Text;
        }

        public string GetNoteValue()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note=SiteDriver.FindElement(AppealNotePageObjects.NoteValueCssSelector, How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return note;

        }

        public bool IsNoteFormDivPresent()
        {
           return  SiteDriver.IsElementPresent(AppealNotePageObjects.NoteFormDivCssLocator, How.CssSelector);
        }

        public bool IsNoteRowPresent()
        {
            return SiteDriver.IsElementPresent(AppealNotePageObjects.NoteRowId, How.Id);
        }
        #endregion
    }
}
