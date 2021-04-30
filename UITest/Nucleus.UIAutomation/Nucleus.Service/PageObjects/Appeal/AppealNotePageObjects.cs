using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealNotePageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        public const string NoteTypeTextFieldId = "NoteType";
        public const string HeaderLabelXpath = ".//label/span[@class='DataLabel']";
        public const string HeaderLabelValueXpath = ".//label/span[@class ='DataValue']";
        public const string NoteTypeOptionXPath = "//div[@id='type_selector']/select[@id='NoteType']/option";

        public const string NoteTypeValueCssSelector =
            "#note_grid_jqg1 form  div.note_form_left div.field.note_type input#NoteType";

        public const string NoteTypeValueForNewNoteCssSelector = "form  div.note_form_left  div.field.note_type span.select";
        public const string NoteValueCssSelector = "body.cke_contents_ltr >p";

        public const string NoteFormDivCssLocator = "div.note_form ";

        public const string NoteRowId = "jqg1";
        public const string LoadingImageIconCssLocator = "div#note_grid_jqg1>img";

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealNotes.GetStringValue(); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public AppealNotePageObjects()
            : base(PageUrlEnum.AppealNotes.GetStringValue())
        {
        }

        #endregion
    }
}
