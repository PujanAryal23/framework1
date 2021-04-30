using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.Console;
using static System.String;

namespace Nucleus.Service.Support.Common
{
    public sealed class NotePage
    {
        #region Page OBJECT Properties

        public const string AddNotesIndicatorCssSelector = "span.icon.toolbar_icon.add_notes";
        public const string ViewNotesIndicatorCssSelector = "span.icon.toolbar_icon.notes";
       
        public const string InputFieldInNotesHeaderByLabelCssTemplate = @"section:has(>ul>div>header:contains(Notes)) div:has(>label:contains({0})) input";
        public const string NoteTypeDropDownInputListByLabelXPathTemplate =
            "//span[text()='{0}']/../following-sibling::div/section//ul//li";
        public const string NoteRecordsListByColCssTemplate =
        "ul.component_item_list>div.note_row>ul>li:nth-of-type({0}) span";
        public const string NoteRecordsByRowColCssTemplate =
            "ul.component_item_list>div:nth-of-type({1}).note_row>ul>li:nth-of-type({0}) span";
        public const string NoteRecordsByRowColAttributeCssTemplate =
           "ul.component_item_list>div:nth-of-type({0}).note_row>ul>li:nth-of-type({1})";
        public const string NotePencilIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_edit_icon";
        public const string NotePencilIconByNameCssTemplate =
            "div.note_row:has(span:contains({0})) span.small_edit_icon";
        public const string NoteCarrotIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_right_icon";
        public const string NoteCarrotIconByNameCssTemplate =
           "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li span.small_caret_right_icon";
        public const string NoteCarrotDownIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_down_icon";
        public const string NoteCarrotDownIconByNameCssTemplate =
         "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li span.small_caret_down_icon";
        public const string VisibleToClientIconInNoteContainerByRowCssTemplate =
           "ul.component_item_list>div:nth-of-type({0})>ul>li.small_check_ok_icon";
        public const string VisibleToClientIconInNoteRowByNameCssTemplate =
         "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li.small_check_ok_icon";
        public const string VisibleToClientIconInNoteRowCss =
        "ul.component_item_list>div.note_row .small_check_ok_icon";
        public const string NoteContainerCssLocator = "section.note_component";
        public const string NotesListCssLocator =
           "ul.component_item_list>div.note_row";
        public const string NotesEditFormCssLocator =
        "ul.component_item_list>div.note_row>div.note_edit_form";
        public const string NotesSectionWithScrollBarCssLocator =
           "ul.note_list";
        public const string NotesTextAreaByRowBarCssLocator =
           "div:nth-of-type({0}).note_row .cke_wysiwyg_frame";
        public const string NotesEditFormByRowXpath =
        "//div[{0}][contains(@class,'note_row')]/div[contains(@class,'note_edit_form')]";
        public const string NotesEditFormByNameCssSelector =
        "div.note_row:has(span:contains({0}))>div.note_edit_form";
        public const string NoteVisibleToClientEnabledCssLocator =
        "ul.component_item_list>div:nth-of-type({0}).note_row>div.note_edit_form>div.form_text.is_enabled>div.component_checkbox.is_enabled";
        public const string NotesEditFormSaveButtonCssLocator = "div:nth-of-type({0}).note_row button.work_button";
        public const string NotesEditFormSaveButtonByNameCssLocator = "div.note_row:has(span:contains({0})) button.work_button";
        public const string NotesEditFormCancelButtonCssLocator = "div:nth-of-type({0}).note_row span.span_link";
        public const string NotesEditFormCancelButtonByNameCssLocator = "div.note_row:has(span:contains({0})) span.span_link";
        public const string VisibleToClientCheckBoxInNoteEditorByRowCssLocator = "div:nth-of-type({0}).note_row .checkbox";
        public const string NotesAddIconCssSelector = "li.add_icon";
        public const string AddNoteFormCssSelector = ".note_list > ul.component_group";
        public const string TypeInputXPath = "//label[text()='Type']/..//input";
        public const string TypeListValueXPathTemplate = "//label[text()='Type']/../section/ul/li[text()='{0}']";
        public const string SubTypeInputXPath = "//label[text()='Sub Type']/..//input";
        public const string SubTypeListValueXPathTemplate = "//label[text()='Sub Type']/../section/ul/li[text()='{0}']";
        public const string SubTypeListXpathTemplate = "//label[text()='Sub Type']/../section/ul/li";
        public const string AddNotesTextAreaCssLocator = ".note_component .cke_wysiwyg_frame";
        public const string VisibleToClientCssLocator = ".note_component .checkbox";
        public const string CheckedVisibleToClientXpath = "//div[text()='Visible To Client']/following-sibling::span[contains(@class,'active')]";
        public const string AddNotesFirstNameLastNameCssLocator = "//section[contains(@class,'note_component')]//span[text()='Name']/../../../div[4]";
        public const string AddNoteSaveButtonCssSelector = ".note_component .work_button";
        public const string AddNoteCancelLinkCssSelector = ".note_component .span_link";
        //public const string AddNoteExclamationXpath = "//section[contains(@class,'note_component')]//label[text()='Notes']//span[2]";
        public const string SubTypeEnabledXPath = "//label[text()='Sub Type' and contains(@class,'is_enabled')]";
        public const string SubTypeDisabledXPath = "//label[text()='Sub Type' and contains(@class,'is_disabled')]";
        public const string AddIconDisabledCssLocator = "li[title = 'Add Note'].is_disabled";
        public const string NameLabelXPath = "//span[text()='Name']/../..";
        public const string VisibleToClientCheckBoxInNoteEditorByNameCssLocator = "div.note_row:has(span:contains({0})) .checkbox";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator =
        "ul.component_item_list>div:nth-of-type({0}).note_row div.component_checkbox.selected";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator =
        "ul.component_item_list>div.note_row:has(span:contains({0})) div.component_checkbox.selected";
        //public const string EditNoteExclamationByNameCssLocator =
        //"div.note_row:has(span:contains({0})) label:contains(Notes) span.field_error";
        public const string NoNotesMessageCssLocator = ".note_component .empty_message";
        public const string NotesLinkXPath = "//span[text()='Notes']";
        public const string EmptyNoteMessageCssLocator = "ul>p.empty_message";
        public const string NoteTypeLabelXpath = "//label[text()='Type']/../li/span";
        public const string OpendNoteFormListCssLocator = "div.note_edit_form";

        [FindsBy(How = How.XPath, Using = NotesLinkXPath)]
        public Link NotesLink;

        //[FindsBy(How = How.CssSelector, Using = PageErrorModalPopupContentDivCssSelector)]
        //public TextLabel PageErrorModalPopupContentDiv;

        //[FindsBy(How = How.Id, Using = PageErrorCloseId)]
        //public ImageButton PageErrorCloseButton;
        #endregion

        private readonly INewNavigator _navigator;
        private ISiteDriver SiteDriver;
        private IJavaScriptExecutors JavaScriptExecutor;
        private IOracleStatementExecutor Executor;

        #region CONSTRUCTOR

        public NotePage(INewNavigator navigator, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor,IOracleStatementExecutor executor)
        {
            _navigator = navigator;
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
            Executor = executor;
        }

        #endregion

        #region PUBLIC METHODS

        #region notes

        public bool IsAddNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(AddNotesIndicatorCssSelector, How.CssSelector);
        }
        public bool IsViewNoteIndicatorPresent()
        {
            return SiteDriver.IsElementPresent(ViewNotesIndicatorCssSelector, How.CssSelector);
        }

        

        //public string NoOfClaimNotes()
        //{
        //    return !IsRedBadgeNoteIndicatorPresent() ? "0" : JavaScriptExecutors.FindElement(RedBadgeNotesIndicatorCssSelector).Text;
        //}

        public List<string> GetAvailableDropDownListInNoteType(string label)
        {
            JavaScriptExecutor.ClickJQuery(Format(InputFieldInNotesHeaderByLabelCssTemplate, label));
            WriteLine("Looking for <{0}> List", label);
            SiteDriver.WaitToLoadNew(200);
            var list = JavaScriptExecutor.FindElements(Format(NoteTypeDropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ClickJQuery(Format(InputFieldInNotesHeaderByLabelCssTemplate, label));
            WriteLine("<{0}> Drop down list count is {1} ", label, list.Count);
            return list;
        }

        public string GetDefaultValueOfNoteTypeOnHeader(string label)
        {
            return JavaScriptExecutor.FindElement(
                    Format(InputFieldInNotesHeaderByLabelCssTemplate, label))
                    .GetAttribute("value");
        }

        public string GetNoteRecordByRowColumn(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    Format(NoteRecordsByRowColCssTemplate, col, row),
                    How.CssSelector).Text;
        }

        public List<String> GetNoteRecordListByColumn()
        {

            return
               JavaScriptExecutor.FindElements(
                   Format(NoteRecordsListByColCssTemplate, 2),
                   How.CssSelector, "Text");

        }

        public bool IsPencilIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(NotePencilIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsPencilIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(NotePencilIconByNameCssTemplate, name));
        }


        public bool IsCarrotIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(NoteCarrotIconByRowCssTemplate, row),
                How.CssSelector);
        }
        public bool IsCarrotIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(NoteCarrotIconByNameCssTemplate, name));
        }
        public bool IsVisibletoClientIconPresentByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(VisibleToClientIconInNoteContainerByRowCssTemplate, row),
                How.CssSelector);
        }

        public bool IsVisibletoClientIconPresentByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(VisibleToClientIconInNoteRowByNameCssTemplate, name));
        }

        public bool IsNoteContainerPresent()
        {
            return SiteDriver.IsElementPresent(Format(NoteContainerCssLocator),
                How.CssSelector);
        }

        public void SelectNoteTypeInHeader(string label, string value)
        {
            var element = JavaScriptExecutor.FindElement(Format(InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ClickJQuery(Format(InputFieldInNotesHeaderByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
            if (
               !SiteDriver.IsElementPresent(
                    Format(NoteTypeDropDownInputListByLabelXPathTemplate, label, value), How.XPath))
                JavaScriptExecutor.ClickJQuery(Format(InputFieldInNotesHeaderByLabelCssTemplate, label));
            JavaScriptExecutor.ExecuteClick(Format(NoteTypeDropDownInputListByLabelXPathTemplate, label, value), How.XPath);
            WriteLine("<{0}> Selected in <{1}> ", value, label);

        }

        public int GetNoteEditFormCount()
        {
            return SiteDriver.FindElementsCount(
                        NotesEditFormCssLocator, How.CssSelector);
        }
        public int GetNoteListCount()
        {
            return SiteDriver.FindElementsCount(
                        NotesListCssLocator, How.CssSelector);

        }


        public bool IsVerticalScrollBarPresentInNoteSection()
        {
            const string select = NotesSectionWithScrollBarCssLocator;
            WriteLine("Scroll Height: " + GetScrollHeight(select));
            WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public bool IsVerticalScrollBarPresentInNoteAreaByRow(int row)
        {
            string select = Format(NotesTextAreaByRowBarCssLocator, row);
            WriteLine("Scroll Height: " + GetScrollHeight(select));
            WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }
        public int GetScrollHeight(string select)
        {
            return JavaScriptExecutor.ScrollHeight(select);
        }

        public int GetClientHeight(string select)
        {
            return JavaScriptExecutor.ClientHeight(select);
        }

        public string GetVisibleToClientTooltipInNotesList()
        {
            return JavaScriptExecutor.FindElement(VisibleToClientIconInNoteRowCss).GetAttribute("title");
        }
        public void ClickOnEditIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(NotePencilIconByRowCssTemplate, row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            WriteLine("Clicked On Small Edit Icon");
        }
        public void ClickOnEditIconOnNotesByName(string name, bool wait=true)
        {
            JavaScriptExecutor.ClickJQuery(Format(NotePencilIconByNameCssTemplate, name));
            if(wait)
                SiteDriver.WaitForCondition(() => IsNoteFormEditableByName(name));
            WriteLine("Clicked On Small Edit Icon");
        }

        public void ClickOnExpandIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(NoteCarrotIconByRowCssTemplate, row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            WriteLine("Clicked On Small Exapnd Icon");
        }

        public void ClickOnExpandIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(NoteCarrotIconByNameCssTemplate, name));
            SiteDriver.WaitForCondition(() => IsNoteEditFormDisplayedByName(name));
            WriteLine("Clicked On Small Exapnd Icon");
        }

        public void ClickOnCollapseIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(NoteCarrotDownIconByRowCssTemplate, row));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            WriteLine("Clicked On Small Collaspe Icon");
        }

        public void ClickOnCollapseIconOnNotesByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(NoteCarrotDownIconByNameCssTemplate, name));
            SiteDriver.WaitForCondition(IsNoteContainerPresent);
            WriteLine("Clicked On Small Collaspe Icon");
        }

        public bool IsNoteFormEditableByName(string name)
        {
            return (IsNoteTextAreaEditable(name));
        }
        public bool IsNoteTextAreaEditable(string name)
        {
            bool isEditable = false;
            JavaScriptExecutor.SwitchFrameByJQuery(Format("div.note_row:has(span:contains({0})) iframe.cke_wysiwyg_frame", name));
            if (SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable") == "true")
            {
                isEditable = true;
            }
            SiteDriver.SwitchBackToMainFrame();
            return isEditable;
        }

        public bool IsNoteEditFormDisplayedByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(NotesEditFormByRowXpath, row),
                                                 How.XPath);

        }
        public bool IsNoteEditFormDisplayedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(NotesEditFormByNameCssSelector, name));

        }

        public void ClickOnSaveButtonInNoteEditorByRow(int row)
        {
            SiteDriver.FindElement(Format(NotesEditFormSaveButtonCssLocator, row), How.CssSelector).Click();
        }
        public void ClickOnSaveButtonInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(NotesEditFormSaveButtonByNameCssLocator, name));
        }
        public void ClickOnCancelButtonInNoteEditorByRow(int row)
        {
            SiteDriver.FindElement(Format(NotesEditFormCancelButtonCssLocator, row), How.CssSelector).Click();
        }

        public void ClickOnCancelButtonInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(NotesEditFormCancelButtonByNameCssLocator, name));
        }

        public void CheckVisibleToClientInNoteEditorByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(Format(VisibleToClientCheckBoxInNoteEditorByRowCssLocator, row));
        }

        public void ClickVisibleToClientCheckboxInNoteEditorByName(string name)
        {
            JavaScriptExecutor.ClickJQuery(Format(VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));
        }

        public bool IsVisibleToClientInNoteEditorSelectedByRow(int row)
        {
            return SiteDriver.IsElementPresent(Format(SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator, row), How.CssSelector);

        }
        public bool IsVisibleToClientInNoteEditorSelectedByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }
        public bool IsVisibleToClientCheckboxPresentInNoteEditorByName(string name)
        {
            return JavaScriptExecutor.IsElementPresent(Format(VisibleToClientCheckBoxInNoteEditorByNameCssLocator, name));

        }

        public bool IsNoNotesMessageDisplayed()
        {
            return SiteDriver.IsElementPresent(NoNotesMessageCssLocator, How.CssSelector);

        }

        public string GetNoNotesMessage()
        {
            return JavaScriptExecutor.FindElement(NoNotesMessageCssLocator).Text;
        }

        public void ClickonAddNoteIcon()
        {

            SiteDriver.FindElement(NotesAddIconCssSelector, How.CssSelector).Click();
            WriteLine("Clicked on Add Icon");
        }

        public void ClickonAddNoteSaveButton()
        {

            SiteDriver.FindElement(AddNoteSaveButtonCssSelector, How.CssSelector).Click();
            WriteLine("Clicked on Save Button");
        }

        public void ClickOnAddNoteCancelLink()
        {
            SiteDriver.FindElement(AddNoteCancelLinkCssSelector, How.CssSelector).Click();
            WriteLine("Clicked on Cancel Link");
        }

        public void SetAddNote(String note,bool handlePopup=true)
        {
            SiteDriver.SwitchFrameByCssLocator(".note_component .cke_wysiwyg_frame");
            SendValuesOnTextArea("note", note,handlePopup);
        
        }


        public string GetAddNote()
        {
            SiteDriver.SwitchFrameByCssLocator(".note_component .cke_wysiwyg_frame");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;            
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        //public string GetNotesWarningMessage()
        //{
        //    return SiteDriver.FindElement(AddNoteExclamationXpath, How.XPath).GetAttribute("title");
        //}

        public bool IsAddNoteFormPresent()
        {
            return SiteDriver.IsElementPresent(AddNoteFormCssSelector, How.CssSelector);
        }
        public bool IsAddIconDisabled()
        {
            return SiteDriver.IsElementPresent(AddIconDisabledCssLocator, How.CssSelector);
        }

        public bool IsVisibleToClientChecked()
        {
            return SiteDriver.IsElementPresent(CheckedVisibleToClientXpath, How.XPath);
        }

        public void ClickVisibleToClient()
        {
            SiteDriver.FindElement(VisibleToClientCssLocator, How.CssSelector).Click();
        }

        public bool IsVisibleToClientCheckboxPresentInAddNoteForm()
        {
            return SiteDriver.IsElementPresent(VisibleToClientCssLocator, How.CssSelector);
        }

        public bool IsSubTypeDisabled()
        {
            return SiteDriver.IsElementPresent(SubTypeDisabledXPath, How.XPath);
        }

        //public bool IsNoteIndicatorpresent()
        //{
        //    return SiteDriver.IsElementPresent(AddNoteExclamationXpath, How.XPath);
        //}

        public void DeleteClaimNotesofNoteTypeClaimOnly(string claSeq, string userName)
        {
            Executor.ExecuteQuery(Format(ClaimSqlScriptObjects.DeleteClaimNotesofNoteTypeClaimOnly, claSeq.Split('-')[0], claSeq.Split('-')[1], userName));
            WriteLine("Delete Note of type claim from database if exists for claseq<{0}>  ", claSeq, userName);
        }

        public string TotalCountOfNotes(string claSeq, string prvSeq)
        {
            List<string> claseq = new List<string>() { claSeq.Split('-')[0], claSeq.Split('-')[1] };

            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfNotes, claseq[0], prvSeq));
            WriteLine("Total count of Claim and Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofClaimNotes(string claSeq)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfClaimAndPatientNotes, claSeq.Split('-')[0], claSeq.Split('-')[1]));
            WriteLine("Total count of Claim and Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofProviderClaimNotesInternalUsers(string prvseq)
        {
            var list = Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.TotalCountOfProviderClaimNoteForInternalUsers, prvseq));
            WriteLine("Total count of Provider's Claim notes is <{0}>", list[0].ToString());
            return list[0].ToString();

        }

        public string TotalCountofProviderAndProviderClaimNotesInternalUsers(string prvseq)
        {
            var list = Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.TotalCountOfProviderAndClaimNotesInternalUsers, prvseq));
            WriteLine("Total count of Provider's Claim notes is <{0}>", list[0].ToString());
            return list[0].ToString();
        }

        public string TotalCountofClaimDocsFromDatabase(string claSeq)
        {
            var count = Executor.GetSingleValue(Format(ClaimSqlScriptObjects.TotalCountOfClaimDocuments, claSeq.Split('-')[0], claSeq.Split('-')[1])).ToString();
            WriteLine("Total count of Provider notes is <{0}>", count);
            return count;
        }

        public string TotalCountofProviderNotes(string prvSeq)
        {
            var list = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.TotalCountOfProviderNotes, prvSeq));
            WriteLine("Total count of Provider notes is <{0}>", list[0].ToString());
            return list[0].ToString();
        }


        public bool IsEmptyNoteWarning()
        {
            SiteDriver.WaitToLoadNew(1000);
            bool firstCondition = SiteDriver.IsElementPresent(DefaultPageObjects.NoteDivCssLocator,
                How.CssSelector);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement(DefaultPageObjects.NoteDivCssLocator, How.CssSelector).GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }

        public string GetEmptyNoteTooltipInEditNoteForm()
        {
            return
                SiteDriver.FindElement(DefaultPageObjects.NoteDivCssLocator,
                    How.CssSelector).GetAttribute("title");
        }

        public void SelectNoteType(string noteType)
        {
            SiteDriver.FindElement(TypeInputXPath, How.XPath).Click();
            SiteDriver.WaitToLoadNew(500);
            if (!SiteDriver.IsElementPresent(Format(TypeListValueXPathTemplate, noteType), How.XPath))
                SiteDriver.FindElement(TypeInputXPath, How.XPath).Click();
            SiteDriver.FindElement(Format(TypeListValueXPathTemplate, noteType), How.XPath).Click();
            WriteLine("NoteType Selected: <{0}>", noteType);
        }

        public void SelectNoteSubType(string noteSubType)
        {

            var element = SiteDriver.FindElement(SubTypeInputXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitToLoadNew(500);
            if (!SiteDriver.IsElementPresent(Format(SubTypeListValueXPathTemplate, noteSubType), How.XPath))
            {
                element = SiteDriver.FindElement(SubTypeInputXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }

            element = SiteDriver.FindElement(Format(SubTypeListValueXPathTemplate, noteSubType), How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WriteLine("NoteSubType Selected: <{0}>", noteSubType);

        }
        
        public List<string> GetNoteSubTypeList()
        {
            JavaScriptExecutor.ExecuteClick(SubTypeInputXPath, How.XPath);           
            SiteDriver.WaitToLoadNew(200);
            var list = JavaScriptExecutor.FindElements(SubTypeListXpathTemplate, How.XPath, "Text");
            list.RemoveAll(IsNullOrEmpty);
            JavaScriptExecutor.ExecuteClick(SubTypeInputXPath, How.XPath);                       
            WriteLine("Note Sub Type Drop down list count is {0} ", list.Count);            
            return list;
        }

        public string GetNameLabel()
        {
            return SiteDriver.FindElement(NameLabelXPath, How.XPath).Text;
        }

        public string GetNoteType()
        {
            return SiteDriver.FindElement(NoteTypeLabelXpath, How.XPath).Text;
        }

        public void SetNoteInNoteEditorByName(String note, string name,bool handlePopup=true)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(Format("div.note_row:has(span:contains({0})) .cke_wysiwyg_frame", name));
            SendValuesOnTextArea(name, note,handlePopup);
        }

        public string GetNoteInNoteEditorByName(string name)
        {
            JavaScriptExecutor.SwitchFrameByJQuery(Format("div.note_row:has(span:contains({0})) .cke_wysiwyg_frame", name));
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public int GetOpenedNoteFormListCount()
        {
            return SiteDriver.FindElementsCount(OpendNoteFormListCssLocator, How.CssSelector);
        }


        public string GetEmptyNoteMessage()
        {
            return SiteDriver.FindElement(EmptyNoteMessageCssLocator, How.CssSelector).Text;
        }

        public void DeleteProviderNotesOnly(string prvSeq, string userName)
        {
            Executor.ExecuteQuery(Format(ProviderSqlScriptObjects.DeleteProviderNotesOnly,prvSeq, userName));
            WriteLine("Delete Note of type Provider from database if exists for prvseq<{0}>  ", prvSeq);
        }

        public List<string> GetProviderTriggeredConditions(string prvSeq)
        {
           return  Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.ProviderTriggeredConditions, prvSeq));
        }


        public void SendValuesOnTextArea(string note, string text, bool handlePopup = true)
        {
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField(true);
            SiteDriver.WaitToLoadNew(1000);
            if (IsNullOrEmpty(text))
            {
                SiteDriver.FindElement("body", How.CssSelector).SendKeys(Keys.Backspace);

            }

            else
            {
                if (text.Length >= 1990)
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(0, text.Length - 4), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);
                    SiteDriver.FindElement(
                            "body", How.CssSelector)
                        .SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(1000);//wait for removing last character which will take few ms
                    WriteLine("Note set to {0}", text);
                }
                else
                {
                    JavaScriptExecutor.SendKeysToInnerHTML(text.Substring(1, text.Length - 1), "body", How.CssSelector);
                    SiteDriver.WaitToLoadNew(300);
                    SiteDriver.FindElement("body", How.CssSelector).SendKeys(text.Substring(0, 1));
                    SiteDriver.WaitToLoadNew(300);
                }
            }
            WriteLine("{0} set to {1}", note, text);
            SiteDriver.SwitchBackToMainFrame();
            if (handlePopup && IsPageErrorPopupModalPresent())
                ClosePageError();
        }
        public bool IsPageErrorPopupModalPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id), 500);
            return SiteDriver.IsElementPresent(NewDefaultPageObjects.PageErrorPopupModelId, How.Id, 200);
        }
       
        public void ClosePageError()
        {
            JavaScriptExecutor.ExecuteClick(NewDefaultPageObjects.PageErrorCloseId, How.Id);
            WriteLine("Closed the modal popup");
        }
        #endregion

        #endregion
    }
}
