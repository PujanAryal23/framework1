using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderNotesPageObjects : NewPageBase
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string NoteGridXPath = ".//table[@id='note_grid']/tbody/tr[contains(@id,'jqg')]";
        public const string NewnotedetaildivId = "add_note";
        public const string TypeComboBoxXPath = ".//div[@id='select_note_type']/span[@class='select']";
        public const string SubTypeComboBoxXPath = ".//div[select[@id='SubType']]/select/option[@selected='selected']";
        public const string NoteGridRowValuesTemplate = ".//table[@id='note_grid']/tbody/tr[@id='{0}']/td";
        //private const string ExpandedStateClass = "expanded";
        //private const string NotesGrid = "//table[@id='note_grid']/tbody/tr[contains(@id,'jqg')]";
        //private const string AddBtnId = "form_submit";


        public const string NoteTypeTextFieldId = "NoteType";
        public const string SubTypeTextFieldId = "SubType";
        public const string HeaderLabelXpath = ".//label/span[@class='DataLabel']";
        public const string HeaderLabelValueXpath = ".//label/span[@class ='DataValue']";

        public const string NoteTypeTemplate = "//tr[contains(@id,'jqg{0}')]/td[@class='note_type']";
        public const string NotesCreatedDateXPath = "//tr[contains(@id,'jqg')]/td[@class='created_date']";
        public const string RecentNotesCreatedDateCssSelector = "tr#jqg1 > td.created_date";
        public const string ExpandAllToggleId = "expand_all_toggle";
        public const string NoteTypeOptionXPath = "//select[@id='NoteType']/option";
        public const string SubTypeOptionXPath = "//select[@id='SubType']/option";
        public const string SubNoteTypeTemplate = "//table[@id='note_grid']/tbody/tr[ td[@class='note_sub_type' and text()='{0}']]";
        public const string CreateNoteSectionCssSelector = "div #add_note[style*=' display: block;']";
        public const string GridDateXPathTemplate = "//table[@id='note_grid']/tbody/tr[td[text()='{0}']][{1}]/ td[@class='created_date' ]";

        public const string LoadingImageIconCssLocator = "tbody>tr img";
        public const string SavedNoteIframeCssLocator = "#note_grid iframe";


        #endregion

        #region PAGEOBJECT PROPERTIES

        #region ID

        //[FindsBy(How = How.Id, Using = NewnotedetaildivId)]
        //public Link NewNoteDetailDivLink;

        //[FindsBy(How = How.XPath, Using = TypeComboBoxXPath)]
        //public TextLabel TypeCmboBox;

        //[FindsBy(How = How.XPath, Using = SubTypeComboBoxXPath)]
        //public TextLabel SubTypeCmboBox;

        //[FindsBy(How = How.Id, Using = NoteTypeTextFieldId)]
        //public SelectField NoteTypeTextField;

        //[FindsBy(How = How.Id, Using = ExpandAllToggleId)]
        //public ImageButton ExpandAllToggleButton;

        //[FindsBy(How = How.CssSelector, Using = RecentNotesCreatedDateCssSelector)]
        //public TextLabel RecentNotesCreatedDate;

        #endregion

        #region XPATH

        //[FindsBy(How = How.XPath, Using = HeaderLabelXpath)]
        //public TextLabel HeaderLabel;

        //[FindsBy(How = How.XPath, Using = HeaderLabelValueXpath)]
        //public TextLabel HeaderLabelValue;
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

        public ProviderNotesPageObjects()
            : base(PageUrlEnum.ProviderNotes.GetStringValue())
        {
        }

        #endregion
    }
}
