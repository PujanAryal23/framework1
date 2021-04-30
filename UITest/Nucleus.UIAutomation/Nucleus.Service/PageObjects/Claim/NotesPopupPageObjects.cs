using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class NotesPopupPageObjects : NewPageBase
    {
        #region FIELDS

        public const string NoteTypeComboBoxId = "NoteType";
        public const string HeaderLabelXpath = ".//label/span[@class='DataLabel']";
        public const string HeaderLabelValueXpath = ".//label/span[@class ='DataValue']";
        public const string ExpandAllToggleId = "expand_all_toggle";

        public const string NotesGrid = "//table[@id='note_grid']/tbody/tr[contains(@id,'jqg')]";
        public const string NoteTypeTemplate = "//tr[contains(@id,'jqg{0}')]/td[@class='note_type']";
        public const string NoteTypeOptionXPath = "//select[@id='NoteType']/option";
        public const string NoteTypeSelectedOptionCssSelector = "select#NoteType > option[selected = 'selected']";

        public const string TblResultGridRowId = "note_grid";

        
        public const string CreateNoteSectionCssSelector = "div #add_note[style*=' display: block;']";//for note popup
        public const string LoadingImageIconCssLocator = "tbody>tr img";//for note popup

        #endregion

        #region PAGEOBJECT PROPERTIES

        #region XPATH

        [FindsBy(How = How.XPath, Using = HeaderLabelXpath)] 
        public TextLabel HeaderLabel;

        [FindsBy(How = How.XPath, Using = HeaderLabelValueXpath)] 
        public TextLabel HeaderLabelValue;
        #endregion

        #region CSS
        [FindsBy(How = How.CssSelector, Using = NoteTypeSelectedOptionCssSelector)]
        public TextLabel NoteTypeSelectedOption;
        #endregion

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealNotes.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region CONSTRUCTOR

        public NotesPopupPageObjects()
            : base(PageUrlEnum.PopupCode.GetStringValue())
        {
        }

        public NotesPopupPageObjects(string url)
            : base(url)
        {
        }

        #endregion
    }
}
