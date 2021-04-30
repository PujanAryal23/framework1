using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Enum;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Provider;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common.Constants;
using CheckBox = UIAutomation.Framework.Elements.CheckBox;
using System.Drawing;
using System.Net.Mime;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.Support.Common;
using Nucleus.Service.SqlScriptObjects.Provider;
using static System.String;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderActionPage : NewDefaultPage
    {
        #region PRIVATE PROPERTIES

        private ProviderActionPageObjects _providerActionPage;
        private readonly string _originalWindow;
        private readonly CalenderPage _calenderPage;
        private readonly NotePage _notePage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;

        #endregion

        #region CONSTRUCTOR


        public ProviderActionPage(INewNavigator navigator, ProviderActionPageObjects providerActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerActionPage = (ProviderActionPageObjects)PageObject;
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _calenderPage = new CalenderPage(SiteDriver);
            _notePage = new NotePage(Navigator,SiteDriver,JavaScriptExecutor,Executor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public NotePage NotePage
        {
            get { return _notePage; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }
        #region sql

        public void CloseDBConnection()
        {
            Executor.CloseConnection();
        }

        public List<string> GetExpectedResonCodeList(bool isVercendUser = true)
        {
            var resonCodeList =
                Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.ReasonCodeInNewProviderAction, isVercendUser ? 'H' : 'C'));
            return resonCodeList;
        }

        public List<string> GetMasterCodeByConditionId(string conditionId)
        {
            var resonCodeList =
                Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.MasterCodeByConditionId, conditionId));
            return resonCodeList;
        }
        public List<string> GetHundredMasterCodeByConditionId(string conditionId)
        {
            var resonCodeList =
                Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.HundredMasterCodeByConditionId, conditionId));
            return resonCodeList;
        }

        public string GetPhoneNumberForSelectedUser(string username)
        {
            var phoneNum =
                Executor.GetSingleStringValue(string.Format(ProviderSqlScriptObjects.GetPhoneNumberForUser, username));

            return phoneNum;
        }
        public List<string> GetSuspectProviderList()
        {
            return
                Executor.GetTableSingleColumn(ProviderSqlScriptObjects.SuspectProviderList);
        }
        public void UpdateEdosDosForProvSeqProcAndClaseq(string provseq, string proc, string claseq)
        {
            var temp = string.Format(ProviderSqlScriptObjects.UpdateEdosDosForPrvSeqwithProcAndClaseqInHcilinTable, provseq, proc, claseq);
            Executor.ExecuteQuery(temp);
        }


        #endregion

        #region code selection manage code of concern

        public List<string> CodeOfConcernInCodeSelectionList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.CodeOfConcernInCodeSelectionCssLocator,
                How.CssSelector, "Text");
        }

        public bool IsConditionIdInManageCodeOfConcernByConditionIdPresent(string conditionId)
        {
            return SiteDriver.IsElementPresent(
                string.Format(ProviderActionPageObjects.ConditionIdInManageCodeOfConcernByConditionIdXPathTemplate,
                    conditionId), How.XPath);
        }

        public void searchByProcedureCodeOnCodeSelection(string procedureCode)
        {
            var obj = SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,
                How.CssSelector);
            obj.ClearElementField();
            obj.SendKeys(procedureCode);
            SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,
                How.CssSelector).SendKeys(Keys.Enter);
        }

        public void SearchByProcedureCodeListOnCodeSelection(List<string> procCodeList)
        {

            //var obj = SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,
            //   How.CssSelector);
            //foreach (var procedureCode in procCodeList)
            //{
            //    obj.ClearWithoutWait().SendKeys(procedureCode);
            //    obj.Enter();
            //}
            var procCodeArrary = "'" + string.Join(",", procCodeList.ToArray()).Replace(",", "','") + "'";
            JavaScriptExecutor.SetProcCode(procCodeArrary, ProviderActionPageObjects.SearchConditionInputBoxCssSelector, ProviderActionPageObjects.SearchConditionSmallCssSelector);

        }

        public bool IsCodeSelectionSearchBoxPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SearchConditionInputBoxCssSelector, How.CssSelector);
            //_providerActionPage.SearchConditionInput.Displayed;
        }
        public bool IsCodeSelectionSearchBoxEnabled()
        {
            return SiteDriver.IsElementEnabled(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,How.CssSelector);
            //_providerActionPage.SearchConditionInput.Enabled;
        }

        public string GetCodeSelectionLabel()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.CodeSelectionLabelXPath, How.XPath).Text;
        }

        public bool IsCodeOfConcernLabelPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.CodesOfConcernLabelXPath, How.XPath);
        }

        public bool IsLengthyMessageLabelPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionDescriptionCss, How.CssSelector);
        }

        public bool IsCodeSelectionSearchIconDisabled()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionSmallCssSelector,
                    How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public string GetTooltipOfSearchBoxCodeSelection()
        {
            return SiteDriver.FindElementAndGetAttribute(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,
                How.CssSelector,"title"); 
        }

        public bool IsCodeofConcernByCodePresent(string code)
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(ProviderActionPageObjects.CodesOfConcernByCodeXPathTemplate, code), How.XPath);
        }

        public void ClickOnCodeOfConcernByCode(string code)
        {
            SiteDriver.FindElementAndClickOnCheckBox(
                    string.Format(ProviderActionPageObjects.CodesOfConcernCheckBoxByCodeXPathTemplate, code),
                    How.XPath);
        }

        public int Get5ColumnCount()
        {
            var val = SiteDriver.FindElement(ProviderActionPageObjects.CodesOfConcern5ColumnCssLocator,
                How.XPath)
                .GetCssValue("-webkit-columns");
            if (string.IsNullOrEmpty(val))
                val = SiteDriver.FindElement(ProviderActionPageObjects.CodesOfConcern5ColumnCssLocator,
                How.XPath)
                .GetCssValue("columns");
            return Int32.Parse(Regex.Match(val, @"\d+").Value);

        }
        #endregion

        #region CIU Referrals

        public string GetNoCIUReferralMessage()
        {
            return JavaScriptExecutor.FindElement(ProviderActionPageObjects.NoCiuReferralCssSelector).Text;
        }

        public void InsertIdentifiedPattern(string text)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            SendValuesOnTextArea("Identified Pattern", text);
        }

        public bool IsCreateCIUReferralSectionDisplayed()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.CreateCIUReferralFormXPath, How.XPath);
        }

        public void ClickOnSaveCIUReferral()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SaveCIUReferralButtonCssSelector, How.CssSelector);
            Console.WriteLine("Click on Save Button");
            WaitForWorking();
        }

        public bool IsRequiredField(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(ProviderActionPageObjects.RequiredAsteriskXpath, label), How.XPath);
        }

        public void ClickOnCancelCIUReferral()
        {
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.CancelCIUReferralLinkCssSelector, How.CssSelector);
            Console.WriteLine("Click on Cancel Link");
            WaitForWorkingAjaxMessageForBothDisplayAndHide();
        }

        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void SetInputFieldOnCreateCIUReferralByLabel(string label, string text, bool isTabRequired = false)
        {
            if (isTabRequired)
                text += Keys.Tab;
            JavaScriptExecutor.FindElement(string.Format(
                ProviderActionPageObjects.InputCIUReferralCssTemplate, label)).SendKeys(text);
            Console.WriteLine("Set {0}:<{1}>", label, text);
        }

        public void SetLengthyValueInCiuReferralFormInputValueGeneric(string value, string fieldname, int length)
        {
            SiteDriver.FindElement(string.Format(ProviderActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                How.XPath).ClearElementField();
            JavaScriptExecutor.SendKeys(value.Substring(0, length - 11),
                string.Format(ProviderActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                How.XPath);//selenium couldnot perform sendkeys for long text which causes hung issue so javascript implemented

            SiteDriver.FindElement(
                    string.Format(ProviderActionPageObjects.CiuReferralInputFieldXpathTempalte, fieldname),
                    How.XPath)
                .SendKeys(value.Substring(length - 11, 11));
        }

        public void SelectPatternCategory(string pattern)
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.PatternCategoryInputCssSelector).Click();

            SetInputFieldOnCreateCIUReferralByLabel("Pattern", pattern);

            JavaScriptExecutor.FindElement(string.Format(ProviderActionPageObjects.PatternCategoryValueCssSelector, pattern)).Click();
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.PatternCategoryInputCssSelector);
            Console.WriteLine("Select Pattern :<{0}>", pattern);
        }

        public List<string> GetCIUCreatedDateList()
        {
            return
                JavaScriptExecutor.FindElements(
                    ProviderActionPageObjects.CIUReferralCreatedDeteCssTemplate,
                    How.CssSelector, "Text");
        }

        public string GetCIUReferralDetailsByRowLabel(int ciuRecordRow, string label)
        {

            return JavaScriptExecutor.FindElement(
                string.Format(ProviderActionPageObjects.CIUReferralDetailByRowLabelCssTemplate, ciuRecordRow, label)).Text.Trim();
        }

        public string GetCIUReferralToolTipDayByRowLabel(int ciuRecordRow, string label)
        {

            return JavaScriptExecutor.FindElement(
                string.Format(ProviderActionPageObjects.CIUReferralDetailByRowLabelCssTemplate, ciuRecordRow, label))
                .GetAttribute("title")
                .Trim();
        }

        public bool IsVerticalScrollBarPresentInProviderDetailSection()
        {
            const string select = ProviderActionPageObjects.ProviderDetailSectionCssSelector;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public void ClickOnDeleteCIUReferralIconByRecordRow(int ciuRecordRow)
        {
            SiteDriver.FindElementAndClickOnCheckBox(string.Format(ProviderActionPageObjects.DeleteCIUReferralIconCssTemplate, ciuRecordRow), How.CssSelector);
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent);
        }

        public void ClickOnAddCIUReferralRecordCssSelector()
        {
            WaitForWorkingAjaxMessageForBothDisplayAndHide();
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.AddCIUReferralRecordCssSelector, How.CssSelector);
            WaitForWorkingAjaxMessage();

            SiteDriver.WaitForCondition(IsCreateCIUReferralSectionDisplayed);

        }

        public int GetCIUReferralRecordRowCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.CIUReferralRecordRowCssSelector,
                How.CssSelector);
        }

        public bool IsAddCIUIconAdjacentToLabel()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.AddCIUReferralIconWithLabelXpath, How.XPath);
        }

        public string GetCIUInputValueByLabel(string label)
        {
            return JavaScriptExecutor.FindElement(string.Format(ProviderActionPageObjects.InputCIUReferralCssTemplate, label))
                                                                                                        .GetAttribute("value");
        }

        public List<string> GetSelectedPatternCategories()
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.PatternCategoryInputCssSelector).Click();
            var catchlist = JavaScriptExecutor.FindElements(ProviderActionPageObjects.SelectedPatternCategoryOptionsXpath, How.XPath, "Text");
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.PatternCategoryInputCssSelector).Click();
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.PatternCategoryInputCssSelector);
            return catchlist;
        }

        public string GetPatternCategoryPlaceholderValue(string label)
        {
            return JavaScriptExecutor.FindElement(string.Format(ProviderActionPageObjects.InputCIUReferralCssTemplate, label))
                .GetAttribute("placeholder");
        }

        #endregion

        public bool IsVerticalScrollBarPresentInProviderExposureSection()
        {
            const string select = ProviderActionPageObjects.ProviderExposureSectionCssSelector;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select).ToString());
            Console.WriteLine("Client Height:" + GetClientHeight(select).ToString());
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

        public string GetDecisionRationaleTextForCotivitiUser()
        {

            return SiteDriver.FindElement(
               ProviderActionPageObjects.DecisionRationaleCotivitiUserCssSelector,
                 How.CssSelector).Text;

        }


        // <summary>
        // dummy function to send keys to rationale pane for testing rationale cannot be edited from 3 column page
        // </summary>
        // <returns></returns>
        public bool IsDecisionRationaleTextEditable(string rationale)
        {
            var element = SiteDriver.FindElement(
                ProviderActionPageObjects.DecisionRationaleCotivitiUserCssSelector,
                How.CssSelector);
            element.SendKeys(rationale);
            bool result;
            result = (element != null) ? true : false;
            return result;
        }


        public bool IsExpectedHyperLinkPresent(string linkText)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            bool elementpresent =
               SiteDriver.IsElementPresent(string.Format(ProviderActionPageObjects.DecisionRationaleHyperlinkCssSelector, linkText),
                   How.CssSelector);
            SiteDriver.SwitchBackToMainFrame();
            return elementpresent;
        }

        public string GetActionDropDrownValue()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ActionComboBoxXPath, How.XPath)
                    .GetAttribute("value");
        }
        public string GetActiveRetriggerPeriodValue()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ActiveRetriggerPeriodValueCssLocator,
                    How.CssSelector).Text;
        }

        public bool IsRetriggerPeriodSelected()
        {
            return
               SiteDriver.IsElementPresent(ProviderActionPageObjects.ActiveRetriggerPeriodValueCssLocator,
                   How.CssSelector);
        }
        public string GetReasonCodeDropDrownValue()
        {
            return SiteDriver.FindElementAndGetAttribute(ProviderActionPageObjects.ReasonCodeComboBoxXPath, How.XPath,
                "value");
        }

        public void ClickOnQuickNoActionXIcon()
        {
            var element =
                SiteDriver.FindElement(ProviderActionPageObjects.QuickNoActionXIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.Out.WriteLine("Click on Quick No Action X Icon");
        }

        public string GetProviderExposureCountValue(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.ProviderExposureCountValueCssTemplate, col, row),
                How.CssSelector).Text;
        }

        public string GetProviderExposureAvgValue(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.ProviderExposureAvgValueCssTemplate, col, row),
                How.CssSelector).Text;
        }

        public int GetProviderExposureVisitAvgValueOnly(int row, int col)
        {
            return
                Int32.Parse(
                    Regex.Match(
                        SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.ProviderExposureAvgValueCssTemplate, col, row),
                How.CssSelector).Text,
                        @"\d+").Value);
        }
        public string GetProviderExposureVisitAvgValue()
        {
            return SiteDriver.FindElement(
                ProviderActionPageObjects.ProviderExposureVisitAvgValueCssLocator,
                How.CssSelector).Text;
        }
        public bool IsBackButtonPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.BackLinkCssSelector, How.CssSelector);
        }



        /// <summary>
        /// Get title of current page in WebDriver
        /// </summary>
        /// <returns></returns>
        public string GetTitle()
        {
            return SiteDriver.Title;
        }

        public string GetGeneratedRationaleNoteFieldValue(string rationaleNoteFirstLine = "PROVIDER")
        {
            SwitchFrameToGenerateRationaleIfarm();

            var value = SiteDriver.FindElement(
               string.Format(ProviderActionPageObjects.GeneratedRationaleNoteFieldXPathTemplate, rationaleNoteFirstLine),
                 How.XPath).Text;
            SwitchBackToMainFrame();
            return value;
        }


        public string GetPTagValueInGeneratedRationaleField(int pRow = 1)
        {
            SwitchFrameToGenerateRationaleIfarm();

            var pTag = SiteDriver.FindElement(
                 string.Format(ProviderActionPageObjects.GeneratedRationaleNoteFieldPTagXPathTemplate, pRow),
                 How.XPath).Text;
            SwitchBackToMainFrame();
            return pTag;
        }

        public void SwitchFrameToGenerateRationaleIfarm()
        {
            SiteDriver.SwitchFrameByCssLocator(ProviderActionPageObjects.GeneratedRationaleIframCssLocator);
        }

        public void SwitchBackToMainFrame()
        {
            SiteDriver.SwitchBackToMainFrame();
        }

        public bool IsGenerateRationaleInputFieldPresentByLabel(string label)
        {
            if (label == "License Status")
                return IsGenerateRationaleInputFieldPresentByLabelForDropDown(label);

            if (label == "User Rationale Summary")
                return IsGenerateRationaleFrameFieldPresentByLabel(label);
            return
                SiteDriver.IsElementPresent(
                    string.Format(ProviderActionPageObjects.GenerateRationaleInputFieldXPathTemplate, label),
                    How.XPath);
        }

        public bool IsGenerateRationaleInputFieldPresentByLabelForDropDown(string label)
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(ProviderActionPageObjects.GenerateRationaleSelectFieldXPathTemplate, label),
                    How.XPath);
        }

        public bool IsGenerateRationaleFrameFieldPresentByLabel(string label)
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(ProviderActionPageObjects.GenerateRationaleFrameInputXPathTemplate, label),
                    How.XPath);
        }

        public bool IsGenerateRationaleButtonDisabled()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.GenerateRationaleDisabledXPath, How
                 .XPath);
        }


        public void ClickOnRefreshContentButton()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.RefreshContentCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());


        }

        public void ClickOnFinishButton()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.FinishCssLocator, How.CssSelector);
            SiteDriver.WaitToLoadNew(1000);

        }

        public void ClickOnCancelOnGenerateRationale(bool confirmCancel = false)
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CancelLinkOnGenerateRationaleCssLocator, How.CssSelector);
            if (IsConfirmationPopupModalPresent())
                if (confirmCancel)
                    ClickOkCancelOnConfirmationModal(true);
                else ClickOkCancelOnConfirmationModal(false);

        }

        public void ClickOnCancelOnGenerateRationaleOnly()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CancelLinkOnGenerateRationaleCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(IsConfirmationPopupModalPresent);


        }

        public void SetGenerateRationaleFormInputFieldByLabel(string label, string value, bool lengthyValue = false)
        {

            if (label == "User Rationale Summary")
                SetUserRationaleSummary(value);
            else
            {

                if (lengthyValue)
                {
                    JavaScriptExecutor.SendKeys(value.Substring(0, value.Length - 5),
                                string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                                How.XPath);

                    SiteDriver.FindElement(
                               string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                               How.XPath)
                               .SendKeys(value.Substring(0, 5));
                }
                else
                {

                    var element = SiteDriver.FindElement(
                        string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                        How.XPath);
                    element.ClearElementField();
                    SiteDriver.WaitTillClear(element);
                    SiteDriver.WaitToLoadNew(200);
                    element.SendKeys(value);
                    //if (SiteDriver.FindElement(
                    //        string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                    //        How.XPath).ClearElementField().Text != value)
                    //    SiteDriver.FindElement(
                    //            string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                    //            How.XPath).ClearElementField().SetText(value);
                }

            }
            Console.WriteLine("{0} set to {1}", label, value);
            //}
            //else
            //{
            //    SiteDriver.FindElement(
            //            string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldByTabNoXPathTemplate, tabNo),
            //            How.XPath).ClearElementField().SetText(value);
            //    Console.WriteLine("{0}th element set to {1}", tabNo, value);
            //}

        }

        public void SetDateByCalendraPicker(string label, string date)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label), How.XPath);
            SiteDriver.WaitToLoadNew(500);
            _calenderPage.SetDate(date);
            SiteDriver.WaitToLoadNew(500);
            if (IsPageErrorPopupModalPresent())
                ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("{0} Selected:<{1}>", label, date);
        }

        public string GetGenerateRationaleFormInputFieldByLabel(string label)
        {

            if (label == "User Rationale Summary")
                return GetUserRationaleSummary();
            if (label == "License Status")
                return SiteDriver.FindElementAndGetAttribute(
                    string.Format(ProviderActionPageObjects.GenerateRationaleSelectFieldXPathTemplate, label),
                    How.XPath,"value");
            Console.WriteLine("Getting Input field  value of {0} element", label);
            var inputValue = SiteDriver.FindElementAndGetAttribute(
                   string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                   How.XPath,"value");
            if (inputValue.Contains('$'))
            {
                inputValue = inputValue.Replace("$", "");
                inputValue = inputValue.Replace(".00", "");
            }
            return inputValue;
        }

        public void ClearGenerateRationaleFormInputFieldByLabel(string label)
        {
            if (label == "User Rationale Summary")
            {
                SetUserRationaleSummary("");
                //
            }
            else SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label),
                How.XPath).ClearElementField();

            Console.WriteLine("Cleared input field for {0}", label);
        }

        public IWebElement CheckActiveGenerateRationaleField(string inputField)
        {
            //SiteDriver.WaitWithImplicitWait(500);
            SiteDriver.WaitToLoadNew(300);
            //SiteDriver.WaitForCondition(
            //   () =>
            //      SiteDriver.ReturnActiveElement().Location.Equals(ReturnWebElementLocation(inputField, selectField, tabNo)),3000);
            IWebElement webProps = SiteDriver.ReturnActiveElement();
            Console.WriteLine("Location of active element {0}", webProps.Location);
            return webProps;
        }

        public Point ReturnWebElementLocation(string inputField)
        {
            Point elementLocation = SiteDriver.GetElementIndex(inputField, How.XPath);

            Console.WriteLine("Location of provided element {0}", elementLocation);
            return elementLocation;
        }

        public bool CheckIfFocusIsInAnElement(string label)
        {
            var inputField = "";
            if (label != "License Status")

                inputField = string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label);


            else
            {
                inputField = string.Format(ProviderActionPageObjects.GenerateRationaleFormSelectFieldByTabNoXPathTemplate, 1);

            }
            //  ClickOnTabKey(inputField);
            return CheckActiveGenerateRationaleField(inputField).Location.Equals(ReturnWebElementLocation(inputField));
        }

        public void ClickOnTabKey(string label)
        {
            var inputField = (label != "License Status") ?
                string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate, label) :
                 string.Format(ProviderActionPageObjects.GenerateRationaleFormSelectFieldByTabNoXPathTemplate, 1);
            SiteDriver.FindElement(inputField, How.XPath).SendKeys(Keys.Tab);
            // SiteDriver.ClickTabKey(inputField, How.XPath);
        }

        public void SetLicenseStatus(string licenseStatus)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(
                    string.Format(ProviderActionPageObjects.GenerateRationaleSelectFieldXPathTemplate,
                        "License Status"), licenseStatus),
                How.XPath);
            var element = SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.GenerateRationaleSelectFieldXPathTemplate, "License Status"),
                How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(licenseStatus);

            JavaScriptExecutor.ExecuteClick(
               ProviderActionPageObjects.LicenseStatusActiveXPathTemplate,
                How.XPath);
            Console.WriteLine("License status set to {0}", licenseStatus);
        }
        public IList<String> GetLicenseStatusOptions()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.LicenseStatusDropDownXpath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(ProviderActionPageObjects.LicenseStatusDropDownXpath, How.XPath);
            IList<string> reasonCodeOptions =
                JavaScriptExecutor.FindElements(ProviderActionPageObjects.LicenseStatusListXapth,
                    How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.LicenseStatusDropDownXpath, How.XPath);
            return reasonCodeOptions;

        }


        public void SetUserRationaleSummary(String note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.ClearElementField();
            element.SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            //SiteDriver.ClickTabKey();
            element = SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.GenerateRationaleFormInputFieldXPathTemplate,
                    "Billing Summary"), How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            // JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.label, "License Status"), How.XPath);

        }
        public string GetUserRationaleSummary()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }


        public void ClickOnGenerateRationaleButton()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.GenerateRationaleXPath,How.XPath);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProviderActionPageObjects.GenerateRationaleSectionCssLocator + " header", How.CssSelector));

        }

        public bool IsGenerateRationaleSectionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.GenerateRationaleSectionCssLocator, How.CssSelector);
        }


        public bool IsConditionExposureDetailsSectionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionExposureDetailsSectionCssSelector,
                How.CssSelector);
        }

        public void ClickOnEditActionCondition()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.EditActionConditionCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.Out.WriteLine("Click on Edit Action Condition");
        }

        public void ClickOnFirstConditionEditIcon()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.FirstProviderConditionEditIcon, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitToLoadNew(1000);
        }

        public void ClickOnNextOnProviderAction()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.NextActionCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
            Console.Out.WriteLine("Click on Next Button");
        }

        public void WaitForGoogleMapToLoad()
        {
            SiteDriver.WaitForCondition(IsGoogleMapPopupPresent);
            if (IsGoogleMapPopupPresent()) return;
            RefreshPage(false);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Google Map is not clicked on first time lets try next one more time");
            ClickAddressHyperlinkInProviderDetailSection();
            SiteDriver.WaitForCondition(IsGoogleMapPopupPresent);
        }

        public bool IsNextIconDisabled()
        {
            return SiteDriver.FindElementAndGetClassAttribute(
                ProviderActionPageObjects.NextActionCssSelector, How.CssSelector).Contains("is_disabled");
        }

        public bool IsProvCondListEditActionConditionEnabled(int row)
        {
            var classList =
                SiteDriver.FindElementAndGetClassAttribute(
                     string.Format(ProviderActionPageObjects.ProvCondListEditIconXpathSelector, row), How.XPath);

            if (classList[0].Contains("is_active")) return true;
            return false;

        }
        public bool IsProvCondListEditActionConditionDisbaled(int row)
        {
            var classList =
                SiteDriver.FindElement(
                    string.Format(ProviderActionPageObjects.ProvCondListEditIconXpathSelector, row), How.XPath).GetAttribute("class");
            if (classList.Contains("is_disabled")) return true;
            return false;

        }
        public void ClickOnCancelActionCondition()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.CancelActionCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            if (IsConfirmationPopupModalPresent())
                ClickOkCancelOnConfirmationModal(true);
            Console.Out.WriteLine("Click on Cancel");//
        }


        public void ClickOnCancelLinkInActionCondition()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.CancelLinkInActionConditionCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            if (IsConfirmationPopupModalPresent())
                ClickOkCancelOnConfirmationModal(true);
            Console.Out.WriteLine("Click on Cancel");//CancelLinkInActionConditionCssSelector
        }
        public void ClickOnCancelActionConditionWithoutConfirmation()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.CancelActionCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.Out.WriteLine("Click on Cancel");
        }

        public void ClickOnSaveActionCondition()
        {
            //_providerActionPage.SaveActionCondition.Click();
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SaveActionButtonCssSelector, How.CssSelector);
            Console.WriteLine("Clicked on save action condition button.");
        }


        public void ClickOnSaveActionWaitForPopup()
        {
            //_providerActionPage.SaveActionCondition.Click();
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SaveActionButtonCssSelector, How.CssSelector);
            SiteDriver.WaitForCondition(IsConfirmationPopupModalPresent);
            Console.WriteLine("Clicked on save action condition button.");
        }

        public void ClickOnSearchCondition()
        {
            SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionCssSelector, How.CssSelector).Click(); 
            Console.Out.WriteLine("Click on Search Conditions");
        }

        public void ClickOnTop10ProcCodes(int option)
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.Top10ProcCodesIconCssLocator,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            JavaScriptExecutor.ExecuteClick(String.Format(ProviderActionPageObjects.Top10ProcCodesByOptionCssLocator, option), How.CssSelector);
            WaitForWorking();
            Console.WriteLine("Click on option " + option + " of Top 10 Proc Codes");
            JavaScriptExecutor.ExecuteMouseOut(String.Format(ProviderActionPageObjects.Top10ProcCodesByOptionCssLocator, option), How.CssSelector);

        }

        public void ClickOnProviderDetailsIcon()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ProviderDetailsIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
            Console.Out.WriteLine("Click on Provider Details");
        }

        public int GetTop10ProcCodeCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.Top10ProcCodesRow, How.XPath);
        }

        public string GetTop10ProcCodeProcedureValueByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeProcedureValueByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodeShortDescValueByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeShortDescValueByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodeCountLabelByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeCountLabelByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodeCountValueByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeCountValueByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodeBilledLabelByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeBilledLabelByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodeBilledValueByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodeBilledValueByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodePaidLabelByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodePaidLabelByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTop10ProcCodePaidValueByRow(int row)
        {
            return SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.Top10ProcCodePaidValueByRowCssLocator, row),
                How.CssSelector).Text;
        }

        public List<string> GetTop10ProcCodeCountValueList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.Top10ProcCodeCountValueCssLocator,
                How.CssSelector, "Text");
        }

        public List<string> GetTop10ProcCodeBilledValueList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.Top10ProcCodeBilledValueCssLocator,
                How.CssSelector, "Text");
        }

        public List<string> GetTop10ProcCodePaidValueList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.Top10ProcCodePaidValueCssLocator,
                How.CssSelector, "Text");
        }

        public string GetConditionExposureLabelByColRow(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionExposureLabelTemplate, col, row),
                    How.CssSelector).Text;
        }

        public double GetConditionExposureValueByColRow(int col, int row)
        {
            var t =
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionExposureValueTemplate, col, row),
                    How.CssSelector).Text;
            return double.Parse(t.Replace('$', ' ').Trim());
        }

        public string GetConditionExposureEmptyMessage()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ConditionExposureEmptyMessage,
                    How.CssSelector).Text;
        }

        #region User Specified Conditions

        public bool IsUserSpcifiedConditionFormPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.UserSpecifiedConditionFormCssSelector,
                How.CssSelector);
        }

        public bool IsActionConditionFormPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ActionConditionsFormSectionCssSelector,
                How.CssSelector);
        }

        public bool IsSelectedUserSpecifiedConditionComponentPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SelectedUserSpecifiedConditionComponent,
                How.CssSelector);
        }

        public void ClickSearchButtonInUserSpecifiedCondition(bool isClient = false)
        {
            JavaScriptExecutor
                .FindElement(isClient
                    ? ProviderActionPageObjects.AddButtonInUserSpecifiedConditionFormCssSelector
                    : ProviderActionPageObjects.SearchButtonInUserSpecifiedConditionFormCssSelector).Click();
            WaitForWorking();
            WaitForWorkingAjaxMessage();
            WaitForStaticTime(200);
        }

        public void ClickOnClearButtonInUserSpecifiedCondition()
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.ClearButtonInUserSpecifiedConditionFormCssSelector).Click();
        }

        public string GetMessageWhenFlagAllCodesIsSelected()
        {
            return SiteDriver
                .FindElement(ProviderActionPageObjects.UserSpecifiedConditionFlagAllCodesMessageCssSelector,
                    How.CssSelector).Text;
        }

        public string GetSelectedUserSpecifiedCondition()
        {
            return SiteDriver
                .FindElement(ProviderActionPageObjects.SelectedSpecifiedConditionCssSelector,
                    How.CssSelector).Text;
        }

        public string GetSelectedUserSpecifiedConditionFromDropdown()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.SelectedUserSpecifiedConditionFromDropdown, How.CssSelector).GetAttribute("innerText");
        }

        public bool IsResultSetContainerPresentForRangeOfCodes()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.RangeOfCodesContainerCssSelector, How.CssSelector);
        }

        public int GetResultSetContainerCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.RangeOfCodesContainerCssSelector,
                How.CssSelector);
        }

        public int GetResultSetProcCodeCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.RangeOfCodesCountXpathSelector, How.XPath);
        }

        public bool IsCheckboxPresentNextToTheCodeInUserSpecifiedCondition()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.UserSpecifiedProcCodeCheckboxCssSelector,
                How.CssSelector);
        }

        public int CountOfCheckboxNextToTheCode()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.UserSpecifiedProcCodeCheckboxCssSelector,
                How.CssSelector);
        }


        public void SelectConditionFromDropdownList(string condition)
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.SelectConditionDropdownCssSelector).Click();
            JavaScriptExecutor.FindElement(Format(ProviderActionPageObjects.SelectConditionOptionFromDropdownCssSelector, condition)).Click();
        }

        public void SetBeginCodeAndEndCode(string beginCode, string endCode = null, bool isTabRequired = false)//(string beginCode, string , bool endcode = false)
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            if (isTabRequired)
            {
                SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector, How.CssSelector).SendKeys(beginCode + Keys.Tab);
            }
            else
            {
                element = SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(beginCode);

                element = SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForEndProcCodeCssSelector, How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(endCode);

            }
        }

        public void SetEndCode(string endCode)
        {
            SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForEndProcCodeCssSelector, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForEndProcCodeCssSelector, How.CssSelector).SendKeys(endCode);
        }


        public void SearchConditionByCodeAndClickOnFirstResult(string condition = "SUSC", string beginCode = "99212", string endCode = null, bool isClient = false)
        {
            if (endCode == null) endCode = beginCode;
            SelectConditionFromDropdownList(condition);
            SetBeginCodeAndEndCode(beginCode, endCode);
            ClickSearchButtonInUserSpecifiedCondition(isClient);
            if (!isClient)
                JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchConditionFirstResultXPath, How.XPath);
        }

        public void SearchConditionByCode(string searchCode, bool isWait = true)
        {
            SiteDriver.FindElementAndSetText(ProviderActionPageObjects.SearchConditionInputBoxCssSelector,
                How.CssSelector,searchCode);
            var element = SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionSmallCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
            if (isWait)
                SiteDriver.WaitForCondition(
                    () =>
                        SiteDriver.IsElementPresent(ProviderActionPageObjects.SearchConditionFirstResultXPath, How.XPath));

        }
        public string GetValueofInputConditionCode(bool beginOrEnd = true)
        {
            return SiteDriver
                .FindElement(beginOrEnd
                    ? ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector
                    : ProviderActionPageObjects.InputFieldForEndProcCodeCssSelector, How.CssSelector).GetAttribute("value");
        }
        public void ClickOnMatchingConditionByConditionId(string conditionId)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.MatchingConditionListTemplate, conditionId), How.XPath);
            SiteDriver.WaitForCondition(
                () => !IsWorkingAjaxMessagePresent());

        }


        public void ClickonFirstResult()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchConditionFirstResultXPath, How.XPath);
        }

        public string GetFirstSelectedConditionText()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.SelectedConditionFirstXPath, How.XPath)
                    .Text;
        }

        public bool AreUserSpecifiedConditionsPresent(string dropdownValue = "Select Condition")
        {
            return JavaScriptExecutor.IsElementPresent(Format(ProviderActionPageObjects
                       .SelectConditionDropdownCssSelector, dropdownValue)) &&
                   SiteDriver.IsElementPresent(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector,
                       How.CssSelector)
                   && SiteDriver.IsElementPresent(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector,
                       How.CssSelector)
                   && JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                       .AddButtonInUserSpecifiedConditionFormCssSelector)
                   && JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                       .ClearButtonInUserSpecifiedConditionFormCssSelector)
                   && SiteDriver.IsElementPresent(ProviderActionPageObjects
                       .FlagAllCodesCheckBoxXPath, How.XPath)
                   && JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                       .CancelLinkCssSelector);
        }

        public bool IsActionConditionsFormDisabled(bool isClientUser = false)
        {

            var isActionInputDisabled = SiteDriver.IsElementPresent(ProviderActionPageObjects.ActionInputDisabledCssLocator, How.XPath);
            var isReasonCodeDisabled = SiteDriver.IsElementPresent(ProviderActionPageObjects.ReasonCodeInputDisabledCssLocator,
                        How.XPath);

            var isVisibleToClientCheckboxDisabled = SiteDriver.IsElementPresent(ProviderActionPageObjects
                       .VisibleToClientCheckboxDisabledCssLocator, How.XPath);
            var isSaveButtonDisabled = SiteDriver.IsElementPresent(ProviderActionPageObjects
                       .SaveButtonDisabledCssLocator, How.CssSelector);
            if (isClientUser)
                return isActionInputDisabled && isReasonCodeDisabled && isSaveButtonDisabled;
            else
                return isActionInputDisabled && isReasonCodeDisabled && isVisibleToClientCheckboxDisabled && isSaveButtonDisabled;

        }

        public void ClickOnPlusIconInSecondColumn()
        {

            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchConditionCssSelector, How.CssSelector);

        }

        public string GetToolTipForPlusIconInSecondColumn()
        {

            return SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionCssSelector, How.CssSelector).Text;

        }

        public bool IsAddButtonPresentInUserSpecifiedCondition()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                .AddButtonInUserSpecifiedConditionFormCssSelector);
        }


        public void ClickAddButtonInUserSpecifiedCondition()
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.AddButtonInUserSpecifiedConditionFormCssSelector).Click();
            WaitForSpinner();
            WaitForStaticTime(300);

        }
        public List<string> GetUserSpecifiedConditionList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.GetUserSpecifiedConditionList, How.XPath,
                "Text").Skip(1).ToList();
        }

        public void ClickOnSelectConditionFromDropdownIcon()
        {
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.SelectConditionDropdownCssSelector).Click();
        }

        public string GetTextIfAddOrSearchButton()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.GetTextIfAddOrSearchButtonXPath, How.XPath).Text;
        }

        public void ClickOnFlagAllCodes()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.FlagAllCodesCheckBoxXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool AreRangeSearchFieldsEnabled()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForBeginProcCodeCssSelector, How.CssSelector).Enabled &&
                   SiteDriver.FindElement(ProviderActionPageObjects.InputFieldForEndProcCodeCssSelector, How.CssSelector).Enabled;
        }

        public bool IsSearchResultsPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.UserSpecifiedSearchResultsCssSelector);
        }

        public string GetValueOfUserSpecifiedSearchResults()
        {
            return JavaScriptExecutor.FindElement(ProviderActionPageObjects.ValueOfUserSpecifiedSearchResultsCssSelector).Text;
        }

        public bool IsMactchingConditionsPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                .UserSpecifiedMatchingConditionCssSelector);
        }

        public List<string> GetMatchingConditionsRecords()
        {
            return JavaScriptExecutor.FindElements(
                ProviderActionPageObjects.UserSpecifiedMatchingConditionsRecordsCssSelector, "innerHTML");
        }

        public void ClickOnUserSepcifiedSearchResults()
        {
            WaitForStaticTime(200);
            JavaScriptExecutor.FindElement(ProviderActionPageObjects.SeletableUserSpecifiedSearchResultCssSelector).Click();
            WaitForStaticTime(200);
        }

        public bool IsSelectedConditionsPresentInActionConditions()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects
                .ActionConditionsSelectedConditionsCssSelector);
        }

        public bool AreSelectedUserSpecifiedConditionsPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.UserSpecifiedConditionInUserSpecifiedConditionSectionCssSelector, How.CssSelector)
            && SiteDriver.IsElementPresent(ProviderActionPageObjects.ActionConditionsSelectedUserSpecifiedCondition,
                    How.CssSelector) &&
                SiteDriver.IsElementPresent(ProviderActionPageObjects.UserSpecifiedProcCodeCheckboxCssSelector,
                    How.CssSelector) &&
                SiteDriver.IsElementPresent(ProviderActionPageObjects.SelectedProcCodeCssSelector, How.CssSelector);
        }

        public void SelectProcCodeUserSpecifiedCondition()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.UserSpecifiedProcCodeCheckboxCssSelector, How.CssSelector);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public void ClickSearchForConditionsByCode()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CreateUserSearchForConditionsIconCssSelector, How.CssSelector);
        }

        #endregion

        public void SelectAction(string action)
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ActionComboBoxXPath, How.XPath);
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.ActionXPathTemplate, action),
                How.XPath);
            SiteDriver.WaitToLoadNew(500);
            Console.WriteLine("Action Selected: <{0}>", action);
            if (IsConfirmationPopupModalPresent()) ClickOkCancelOnConfirmationModal(true);

        }

        public void SelectActionOnly(string action)
        {

            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ActionComboInputToggleXpathSelector,
                How.XPath);
            SiteDriver.FindElement(ProviderActionPageObjects.ActionComboInputXpathSelector,
                How.XPath).SendKeys(action);


            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.ActionComboInputValueXPathTemplate, action), How.XPath);
            SiteDriver.WaitToLoadNew(200);
        }

        public bool IsNoteTextAreaEditable()
        {
            bool isEditable = false;
            if (SiteDriver.IsElementPresent("iframe.cke_wysiwyg_frame", How.CssSelector))

            {
                JavaScriptExecutor.SwitchFrameByJQuery(
                    Format("iframe.cke_wysiwyg_frame"));
                if (SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable") == "true")
                {
                    isEditable = true;
                }
            }
            else
            {
                return false;
            }
            SiteDriver.SwitchBackToMainFrame();
            return isEditable;
        }

        public void WaitToLoadUserRationaleSummary()
        {
            SiteDriver.WaitForCondition(() => IsNoteTextAreaEditable());
        }


        public void SelectFirstReasonCode()
        {
            WaitForStaticTime(500);
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ReasonCodeComboBoxXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForStaticTime(500);
            if (!SiteDriver.IsElementPresent(ProviderActionPageObjects.FirstReasonCodeOptionSelectorsXPath, How.XPath))
            {
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }

            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.FirstReasonCodeOptionSelectorsXPath, How.XPath);
            Console.WriteLine("First available Reason Code selected");

            //JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.FirstReasonCodeCssSelector, How.XPath);
            Console.WriteLine("First available Reason Code selected");

            if (IsConfirmationPopupModalPresent()) ClickOkCancelOnConfirmationModal(true);


        }

        public bool IsVisibleToClientCheckBoxShown()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.VisibleToClientCheckBoxXpath,
                How.XPath);
        }

        public bool IsVisibleToClientCheckBoxChecked()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.VisibleToClientCheckBoxCheckedXpath,
                How.XPath);
        }

        public bool IsFirstCodeofConcernCheckBoxChecked()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.FirstCodeofConcernCheckBoxCheckedCssLocator,
                How.CssSelector);
        }

        public bool IsValidationNoticeModalPopupPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ValidationNoticeModalPopupId, How.Id);
        }

        public string GetValidationNoticePopupMessage()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ValidationNoticePopupContentDivId, How.Id);
            return element.Text;
        }

        public void ClickOnFirstCodeofConcernCheckBoxIfChecked()
        {
            if (IsFirstCodeofConcernCheckBoxChecked())
            {
                var element = SiteDriver.FindElement(ProviderActionPageObjects.FirstCodeofConcernCheckBoxCheckedCssLocator,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on first available code of concern to uncheck");
            }
        }

        public void InsertDecisionRationale(string text)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            SendValuesOnTextArea("Decision Rationale", text);
        }

        public void InsertDecisionRationaleUsingGenerateRationale(string text)
        {
            ClickOnGenerateRationaleButton();
            SetGenerateRationaleFormInputFieldByLabel("Billing Summary", "abc");
            InsertDecisionRationale(text);
            SetGenerateRationaleFormInputFieldByLabel("Billed Referral Exposure", "123");
            SetGenerateRationaleFormInputFieldByLabel("Paid Referral Exposure", "123");
            SetGenerateRationaleFormInputFieldByLabel("License #", "123");
            SetGenerateRationaleFormInputFieldByLabel("Provider Website", "123");
            ClickOnFinishButton();
            //if (IsValidationNoticeModalPopupPresent())
            //{
            //    CloseValidationNoticePopup();
            //    ClickOnFinishButton();
            //}
        }

        public string GetDecisionRationaleText()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var decisionRationaleText = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return decisionRationaleText;
        }

        public void CloseValidationNoticePopup()
        {
        //[FindsBy(How = How.Id, Using = ValidationNoticePopupCloseId)]
        //[FindsBy(How = How.Id, Using = ValidationNoticePopupCloseId)]
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ValidationNoticePopupCloseId, How.Id);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(() => !IsPageErrorPopupModalPresent());
            Console.WriteLine("Closed Validation Notice Popup");
        }

        public String GetBasicProviderDetailsData()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.BasicProviderDetailsDivCssSelector,
                    How.CssSelector).Text;
        }

        public string GetProviderSequence()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.ProviderSequenceCssSelector,
                How.CssSelector).Text;
        }

        public List<String> GetBasicProviderInformation(string prvSeq)
        {
            var table = Executor
                .GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetBasicProviderInformation, prvSeq)).ToList();
            List<string> data = new List<string>();
            for (int i = 0; i < table[0].Table.Columns.Count; i++)
            {
                data.Add(table[0][i].ToString());
            }
            return data;
        }


        public List<string> GetBasicProviderInformationValues()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.BasicProviderDetailsValue, How.XPath, "Text");
        }

        public List<string> GetBasicProviderInformationLabels()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.BasicProviderDetailsLabel, How.XPath, "Text");
        }


        public String GetProviderExposureData()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ProviderExposureCssSelector,
                    How.CssSelector).Text;
        }

        public String GetProviderDetailsData()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.ProviderDetailsId, How.Id).Text;
        }

        public bool IsQuickNoActionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.QuickNoActionIconCssSelector,
                How.CssSelector);
        }

        public void SelectFilterConditions(int option)
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.FilterConditionsIconCssSelector,
                How.CssSelector);
            JavaScriptExecutor.ExecuteClick(
                String.Format(ProviderActionPageObjects.FilterConditionsOptionTemplateXPath, option), How.XPath);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitToLoadNew(1000);
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.FilterConditionsOptionListXPath, How.XPath);
            Console.WriteLine("Selected filter option {0}", option);
        }

        public List<string> GetConditionIdListInProviderConditions()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.ConditionIdListInProviderConditionsCssLocator,
                How.CssSelector, "Text");
        }
        public string GetTriggeredDaeInProviderConditions()
        {
            return SiteDriver.FindElement(
                ProviderActionPageObjects.TriggeredDaeInProviderConditionsCssLocator,
                How.CssSelector).Text;
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public IList<string> GetConditionsClientAction()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.ConditionClientActionXPath, How.XPath, "Text");
        }

        public bool IsConditionDetailsSectionLoaded()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionDetailsSectionXPathLocator, How.XPath);
        }

        public void ClickOnConditionDetailsIcon()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ConditionDetailsIconCssSelector,
                How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent(), 10000);
            Console.WriteLine("Click On Condition Details Icon");
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionDetailsSectionXPathLocator, How.XPath));
        }
        public bool IsProviderConditionReasonLabelPresent(int row)
        {
            return SiteDriver.IsElementPresent(String.Format(ProviderActionPageObjects.ConditionDetailsReasonLabelXPathTemplate, row), How.XPath);
        }

        public void SelectFirstProviderConditionClientAction()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ConditionClientActionXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Click On First Provider Condition with Client Action");
        }

        public void ClickOnProviderDetailByRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.ProviderDetailSelectionByRowCssSelector, row), How.CssSelector);
        }

        public bool IsProviderDetailExpandedSectionPresent(int row)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(ProviderActionPageObjects.ProviderDetailExpandedSectionByRowCssSelector, row));
        }

        public void SelectProviderCondition(int row)
        {
            JavaScriptExecutor.ExecuteClick(
                String.Format(ProviderActionPageObjects.ProviderConditionCssSelectorByRowTemplate, row), How.CssSelector);
            Console.WriteLine("Click On Provider Condition Row: " + row);

        }

        public void SelectProviderConditionByConditionId(string conditionId)
        {
            JavaScriptExecutor.ExecuteClick(
                String.Format(ProviderActionPageObjects.PrivderCodntionSelectorByConditionIdXPathTemplate, conditionId), How.XPath);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Click On Provider Condition having Condition Id: " + conditionId);

        }
        public void SelectProviderConditionConditionCode(int row)
        {
            var element = SiteDriver.FindElement(
                String.Format(ProviderActionPageObjects.ProviderConditionConditionCodeCssSelectorByRowTemplate, row),
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Click On Provider Condition Code for popup display: " + row);

        }

        public string GetProviderConditionDescription()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ProviderConditionConditionCodeDescriptionXPath,
                    How.XPath)
                    .Text;
        }

        public string GetProviderConditionIdPopupContent(int row = 1)
        {
            return
                SiteDriver.FindElement(string.Format(ProviderActionPageObjects.ProviderCondtionIdPopupContentCssTemplate, row),
                    How.CssSelector)
                    .Text;
        }

        public bool IsLabelBoldInProviderConditionPopup(int row = 1)
        {
            return SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.ProviderCondtionIdPopupContentCssTemplate, row) + ">span",
                How.CssSelector).GetAttribute("class").Contains("bold");
        }

        public string GetProviderConditionDetailForFieldAndRow(string label, int row)
        {
            return
                SiteDriver.FindElement(string.Format(ProviderActionPageObjects.ProviderConditionLeftColumnValuesForFieldGivenAndRowTemplate, label, row),
                    How.XPath)
                    .Text;
        }

        public string GetProviderDetailByLabel(string label)
        {
            return SiteDriver
                .FindElement(
                    string.Format(ProviderActionPageObjects.ProviderDetailByLabelXPathTemplate, label), How.XPath)
                .Text;
        }

        public bool IsProviderConditionRowHaveActionRequiredBadge(int row)
        {
            return
                 SiteDriver.IsElementPresent(
                     String.Format(ProviderActionPageObjects.ActionRequiredBadgeIconCssSelector, row), How.CssSelector);
        }
        public bool IsProviderConditionSelectedActive(string conditionId)
        {
            var classString =
                SiteDriver.FindElementAndGetClassAttribute(
                    String.Format(ProviderActionPageObjects.ProviderConditionXPathTemplate, conditionId), How.XPath);
            return classString[0].Contains("active");
        }
        public string GetEmptyConditionNote()
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsEmptyConditionNoteXPathLocator),
                    How.XPath).Text;
        }
        public string GetConditionNoteInConditionDetail()
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsConditionNoteXpathLocator),
                    How.XPath).Text;
        }

        public string GetConditionDetailsEmptyRecordMessage()
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsEmptyAuditRecordXPathLocator),
                    How.XPath).Text;
        }
        public string GetConditionDetailsRow(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsRowXPathTemplate, row),
                    How.XPath).Text;
        }

        public bool IsConditionDetailAuditRowPresent(int row = 1)
        {
            return
                SiteDriver.IsElementPresent(
                    String.Format(ProviderActionPageObjects.ConditionDetailsRowXPathTemplate, row), How.XPath);
        }

        public string GetConditionDetailEllipseTitleByRow(int row = 1)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.EllipsesOnConditionDetailRowXPathTemplate, row),
                    How.XPath).GetAttribute("title");
        }

        /// <summary>
        /// Get  whole row text for provider details section's condition detail in audit trail
        /// </summary>
        /// <param name="row">specifies which audit trail row to select</param>
        /// <param name="innerRow">specifies which ineer row to select in the conidtion detail selected</param>
        /// <returns>whole string text selected</returns>
        public string GetConditionDetailsRowSelector(int row, int innerRow)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsRowSelectorXPathTemplate, row, innerRow),
                    How.XPath).Text;
        }
        public string GetConditionDetailsFirstSecondRow(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsFirstSecondRowSelectorXPathTemplate, row),
                    How.XPath).Text;
        }
        /// <summary>
        /// Returns codes of conern details displayed
        /// </summary>
        /// <param name="row">to specidy selection of which audit trail row</param>
        /// <returns>entire text populated in code of conern area</returns>
        public string GetConditionDetailsCodeOfConcernRow(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsCodeOfConcernXPathTemplate, row),
                    How.XPath).Text;
        }
        /// <summary>
        /// Returns list of associated proc codes under codes of conern details displayed
        /// </summary>
        /// <param name="row">to specidy selection of which audit trail row</param>
        /// <returns>list of associated proc codes</returns>
        public List<string> GetListOfAssociatedProcCodeCodeOfConcernRow(int row)
        {
            return
                JavaScriptExecutor.FindElements(String.Format(ProviderActionPageObjects.ConditionDetailsAssociatedProcCodeListXPathTemplate, row),
                    How.XPath, "Text");
        }

        public int GetConditionDetailsRowCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.ConditionDetailsRowsXPathTemplate,
                How.XPath);
        }

        public string GetActionDateStringFromConditionDetailsRow(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionDetailsActionDateValueXPathTemplate, row),
                    How.XPath)
                    .Text;
        }


        public IList<String> GetReasonCodeOptions()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ReasonCodeDropDownXPath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(ProviderActionPageObjects.ReasonCodeDropDownXPath, How.XPath);
            IList<string> reasonCodeOptions =
                JavaScriptExecutor.FindElements(ProviderActionPageObjects.ReasonCodeListOptionsXPath,
                    How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.ReasonCodeDropDownXPath, How.XPath);
            return reasonCodeOptions;

        }

        public IList<String> GetActionOptions()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ActionComboBoxXPath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(ProviderActionPageObjects.ActionComboBoxXPath, How.XPath);
            IList<string> reasonCodeOptions =
                JavaScriptExecutor.FindElements(ProviderActionPageObjects.ActionListOptionsXPath,
                    How.XPath, "Text");
            JavaScriptExecutor.ExecuteMouseOut(ProviderActionPageObjects.ActionComboBoxXPath, How.XPath);
            return reasonCodeOptions;

        }

        public void SelectCodeOfConcernForActioning(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.SelectCodeOfConcernForActioningCssLocatorTemplate, row),
                How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(() => IsManageCodesofConcernColumnPresent());
        }


        public void ClickOnCheckBoxProviderFlagging()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ProviderFlaggingCheckBoxCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            //_providerActionPage.ProviderFlaggingCheckBox.Click();

        }

        public void ClickOkCancelOnConfirmationModal(bool confirmation)
        {

            if (confirmation)
            {
                var element =
                    SiteDriver.FindElement(ProviderActionPageObjects.OkConfirmationCssSelector, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Ok Button is Clicked");

            }
            else
            {
                var element = SiteDriver.FindElement(ProviderActionPageObjects.CancelConfirmationCssSelector,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WaitForWorkingAjaxMessage();
                Console.WriteLine("Cancel Button is Clicked");

            }
            SiteDriver.WaitToLoadNew(200);

        }


        public bool IsExclamationProfileIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ExclamationProfileIndicatorCssSelector,
                How.CssSelector);
        }

        public string GetProfileIndicatorTitle()
        {

            return SiteDriver.FindElement(ProviderActionPageObjects.ProfileIndicatorCssSelector,
                How.CssSelector).GetAttribute("title");
        }

        public string GetProfileReviewIndicatorTitle()
        {

            return SiteDriver.FindElement(ProviderActionPageObjects.ProfileReviewIndicatorCssSelector,
                How.CssSelector).GetAttribute("title");
        }

        public bool IsConfirmationPopupModalPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConfirmationPopupModalId, How.Id);
        }

        public bool IsCancelActionConditionLinkBelowScroll()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConfirmationPopupModalId, How.Id);
        }
        public bool IsVerticalScrollBarPresentInManageCodeOfConcernsQuad()
        {
            const string select = ProviderActionPageObjects.ManageCodesofConcernColumnCssSelector;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }
        public string GetConfirmationMessage()
        {
            return
                SiteDriver.FindElement(
                    ProviderActionPageObjects.ConfirmationPopupModalMessageCssSelector, How.CssSelector).Text;
        }


        public string GetProviderFlaggingLabel()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.ProviderFlaggingLabelCssSelector,
                    How.CssSelector).Text;
        }

        public bool IsCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ProviderFlaggingCheckBoxCssSelector,
                How.CssSelector);
        }

        public bool IsCheckBoxAdjecentToProviderFlaggingChecked()
        {
            return SiteDriver.FindElement(
               ProviderActionPageObjects.ProviderFlaggingCheckBoxCssSelector, How.CssSelector)
               .GetAttribute("class")
               .Contains("active");
        }

        /// <summary>
        /// Click on small notes link
        /// </summary>
        public ProviderNotesPage ClickOnSmallProviderNotes()
        {

            var claimNotePage = Navigator.Navigate(() =>
            {
                var isSmallNoteIcon = IsSmallAddNoteIconPresent();
                var element =
                    SiteDriver.FindElement(ProviderActionPageObjects.SmallNoteIconCssSelector, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked Small Notes Link");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderNotes.GetStringValue()));
                if (isSmallNoteIcon)
                    SiteDriver.WaitForCondition(
                   () => SiteDriver.IsElementPresent(ProviderNotesPageObjects.CreateNoteSectionCssSelector, How.CssSelector));
                else
                    SiteDriver.WaitForCondition(
                       () =>
                           SiteDriver.IsElementPresent(ProviderNotesPageObjects.NoteGridXPath,
                               How.XPath));
                SiteDriver.WaitToLoadNew(500);
            }, () => new ProviderNotesPageObjects());
            return new ProviderNotesPage(Navigator, claimNotePage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public void ClosePopupNoteAndSwitchToNewProviderActionPage()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
        }

        public void CloseNewProviderActionPopupAndSwitchToProviderSearchPage()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderSearch.GetStringValue());
        }

        public bool IsSmallAddNoteIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SmallAddNoteIconCssSelector, How.CssSelector);
        }

        public bool IsSmallViewNoteIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SmallViewNoteIconCssSelector,
                How.CssSelector);
        }


        /// <summary>
        /// Click on  notes link on header section of container view
        /// </summary>e
        public ProviderNotesPage ClickOnHeaderProviderNotes()
        {

            var providerNotePage = Navigator.Navigate(() =>
            {
                var isSmallNoteIcon = IsAddNoteIconPresent();
                var element = SiteDriver.FindElement(ProviderActionPageObjects.NoteIconCssSelector, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked  Notes Link in Header Section");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderNotes.GetStringValue()));
                if (isSmallNoteIcon)
                    SiteDriver.WaitForCondition(
                        () =>
                            SiteDriver.IsElementPresent(ProviderNotesPageObjects.CreateNoteSectionCssSelector,
                                How.CssSelector));
                else
                {
                    SiteDriver.WaitForCondition(
                        () =>
                            SiteDriver.IsElementPresent(ProviderNotesPageObjects.NoteGridXPath,
                                How.XPath));
                }
                SiteDriver.WaitToLoadNew(500);
            }, () => new ProviderNotesPageObjects());
            return new ProviderNotesPage(Navigator, providerNotePage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);

        }

        public void ClickOnSearchIconAtHeader()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchIconCssSelector, How.CssSelector);
            WaitForWorking();
            WaitForStaticTime(3000);

        }

        public SuspectProvidersPage ClickOnSearchIconAtHeaderReturnToSuspectProvidersPage()
        {
            var SuspectProvidersPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchIconCssSelector, How.CssSelector);
                WaitForWorking();

            }, () => new SuspectProvidersPageObjects());
            return new SuspectProvidersPage(Navigator, SuspectProvidersPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            );

        }

        public ProviderSearchPage ClickOnSearchIconAtHeaderReturnToProviderSearchPage()
        {
            var NewProviderSearchPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SearchIconCssSelector, How.CssSelector);
                WaitForWorking();
                GetSideBarPanelSearch.WaitSideBarPanelOpen();
                CaptureScreenShot("Provider Action Search Icon Click Issue");

            }, () => new ProviderSearchPageObjects());
            return new ProviderSearchPage(Navigator, NewProviderSearchPage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);

        }


        public bool IsAddNoteIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.AddNoteIconCssSelector, How.CssSelector);
        }

        public bool IsViewNoteIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ViewNoteIconCssSelector, How.CssSelector);
        }

        public void ClickNoteIcon()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.AddNoteIconCssSelector, How.CssSelector);
        }

        public bool IsAddNoteFormSectionPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.AddNoteSectionCssSelector);
        }

        public int GetNumofConditionsinSelectConditionsforActioningColumn()
        {
            return
                SiteDriver.FindElementsCount(
                    ProviderActionPageObjects.ConditioninSelectConditionsForActioningColumnCssSelector,
                    How.CssSelector);
        }

        public bool IsManageCodesofConcernColumnPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ManageCodesofConcernColumnCssSelector,
                How.CssSelector);
        }

        public bool IsTriggerDatePresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.TriggerDateCssSelector, How.CssSelector);
        }

        public bool IsConditionTitlePresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionTitleCssSelector, How.CssSelector);
        }

        public bool IsConditionDescriptionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ConditionDescriptionCss, How.CssSelector);
        }

        public bool IsSelectAllCodesofConcernCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SelectAllCodesofConcernCheckBoxCssSelector,
                How.CssSelector);
        }

        public bool IsSelectAllCodesofConcernCheckBoxChecked()
        {
            return SiteDriver.FindElement(
                 ProviderActionPageObjects.SelectAllCodesofConcernCheckBoxCssSelector, How.CssSelector)
                 .GetAttribute("class")
                 .Contains("active");



        }

        public bool IsCodeofConcernSorted()
        {
            List<string> allCodesofConcerList =
                JavaScriptExecutor.FindElements(ProviderActionPageObjects.AllCodesofConcernCssSelector, How.CssSelector, "Text");
            return allCodesofConcerList.IsInAscendingOrder();
        }

        public void ClickOnActionAllConditionsCheckBox()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ActionAllConditionsCheckBoxCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Action all conditions Checkbox");
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnQuickNoActionAllConditions()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.QuickNoActionAllConditionsCssSelector, How.CssSelector);
        }

        public bool ToggleSelectAllCodesofConcern()
        {
            List<bool> beforeToggle = GetAllCodesofConcernCheckBoxValues();

            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.SelectAllCodesofConcernCheckBoxCssSelector,
                How.CssSelector);

            List<bool> afterToggle = GetAllCodesofConcernCheckBoxValues();
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.SelectAllCodesofConcernCheckBoxCssSelector,
                How.CssSelector);
            if (!beforeToggle.SequenceEqual(afterToggle))
            {
                return true;
            }
            return false;
        }


        public void ToggleFirstCodeofConcern()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.FirstCodeofConcernCheckBoxCheckedCssLocator,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Select all codes of concern");
        }

        public void SelectReTriggerPeriod(String period)
        {
            SiteDriver.FindElement(ProviderActionPageObjects.ReTriggerPeriodInputCssSelector,
                How.CssSelector)
                .SendKeys(period);
            Console.WriteLine("ReTrigger Period set to : " + period);
            if (IsConfirmationPopupModalPresent()) ClickOkCancelOnConfirmationModal(true);


        }

        public ProviderActionPage SaveActionConditions()
        {
            var newProviderActionPage = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {

                ClickOnSaveActionCondition();
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(ProviderActionPageObjects.BasicProviderDetailsDivCssSelector,
                        How.CssSelector));
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            });
            return new ProviderActionPage(Navigator, newProviderActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public void FillDecisionRationaleNote(String note, bool cleartext = false)
        {
            if (cleartext)
            {
                SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
                JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
                SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
                SiteDriver.SwitchBackToMainFrame();
            }

            WaitForStaticTime(3000);
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.DecisionRationaleCssSelector, How.CssSelector);
            SiteDriver.FindElement(ProviderActionPageObjects.DecisionRationaleCssSelector, How.CssSelector)
                .SendKeys(note);
            Console.WriteLine("Decision Rationale : " + note);
            WaitForStaticTime(3000);
        }

        public bool IsCheckBoxForEachCodeofConcernPresent()
        {
            for (int i = 1; i <= GetCodeofCodesofConcernCount(); i++)
            {
                if (!SiteDriver.IsElementPresent(string.Format(ProviderActionPageObjects.SingleCodesofConcernCheckBoxCssTemplate, i),
                    How.CssSelector))
                    return false;
            }
            return true;

        }

        public List<String> GetAllCodesofConcernList()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.AllCodesofConcernCssSelector, How.CssSelector, "Text");
        }

        public int GetAllCodesofConcernListCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.AllCodesofConcernCssSelector, How.CssSelector);
        }

        public string GetColorOfSelectedCodeOfConcern()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.SelectedCodeOfConcernDivCssTemplate, How.CssSelector)
                    .GetCssValue("color");
        }

        public List<bool> GetAllCodesofConcernCheckBoxValues()
        {
            return
                SiteDriver.FindElementAndGetCheckedAttribute(
                    ProviderActionPageObjects.AllCodesofConcernCheckCssSelector, How.CssSelector);
        }


        public bool IsIndividualCodesofConcernSelected()
        {
            List<bool> IndividualCodesofConcernCheckBoxValue =
                SiteDriver.FindElementAndGetCheckedAttribute(
                    ProviderActionPageObjects.AllCodesofConcernCheckCssSelector, How.CssSelector);
            return IndividualCodesofConcernCheckBoxValue.All(c => c);

        }


        public bool ToggleCodesofConcernCheckBox()
        {
            List<bool> beforeToggle = GetAllCodesofConcernCheckBoxValues();
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.AllCodesofConcernCheckCssSelector,
                How.CssSelector);

            List<bool> afterToggle = GetAllCodesofConcernCheckBoxValues();
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.AllCodesofConcernCheckCssSelector,
                How.CssSelector);

            if (!beforeToggle.SequenceEqual(afterToggle))
            {
                return true;
            }

            return false;
        }

        public bool IsScrollBarPresentinManageCodeOfConcern()
        {
            const string select = ProviderActionPageObjects.ManageCodesofConcernColumnScrollDivCssSelector;
            var scrollheight = GetScrollHeight(select);
            var clientheight = GetClientHeight(select);
            return GetScrollHeight(select) > GetClientHeight(select);
        }


        public int GetProviderConditionsCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.ProviderConditionCssSelector,
                How.CssSelector);
        }

        public string GetConditionIdFromProviderConditions(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionIdInProviderConditionsCssSelectorTemplate, row),
                    How.CssSelector).Text.Substring(0, 4);
        }
        public string GetConditionFromProviderConditions(int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionIdInProviderConditionsCssSelectorTemplate, row),
                    How.CssSelector).Text;
        }

        public string GetConditionIdFromLeftColumn(int row)
        {
            return SiteDriver.FindElement(
                String.Format(
                    ProviderActionPageObjects.ConditionIdInSelectConditionsForActioningColumnCssSelectorTemplate, row),
                How.CssSelector).Text.Substring(0, 4);
        }
        public string GetConditionFromEditProvideActionColumns(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.ConditionFromEditProviderActionCssTemplate, col, row),
                    How.CssSelector).Text;
        }
        public int GetSelectedConditionsCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.SelectedConditionCssSelector,
                How.CssSelector);
        }

        public void ClickOnSelectedConditionByCode(string code)
        {
            JavaScriptExecutor.ExecuteClick(
                 String.Format(ProviderActionPageObjects.SelectedConditionByCodeXPath, code), How.XPath);
        }

        public List<string> GetRetriggerTimePeriodList()
        {
            var retriggerTimePeriodList =
                JavaScriptExecutor.FindElements(ProviderActionPageObjects.RetriggerTimePeriodEnabledCssSelector,
                    How.CssSelector, "Text");
            var placeHolder =
                SiteDriver.FindElement(ProviderActionPageObjects.ReTriggerPeriodInputCssSelector,
                    How.CssSelector).GetAttribute("placeholder");
            retriggerTimePeriodList[0] = placeHolder + " " + retriggerTimePeriodList[0];
            return retriggerTimePeriodList;
        }

        public bool IsRetriggerTimePeriodDisbaled()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.RetriggerTimePeriodDisabledCssSelector,
                 How.CssSelector);
        }


        public string GetTextFromSelectedConditionsUsingConditionId(string code)
        {
            return
                SiteDriver.FindElement(
                    String.Format(ProviderActionPageObjects.SelectedConditionByCodeXPath, code), How.XPath)
                    .Text;
        }

        public bool IsActionSelectedInComboBox()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ActionSelectedOptionCssSelector,
                How.CssSelector);
        }

        public bool IsReasonCodeSelectedInComboBox()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ReasonCodeSelectedOptionCssSelector,
                How.CssSelector);
        }

        public List<string> GetDecisionRationaleFormattingOptions()
        {
            return JavaScriptExecutor.FindElements(ProviderActionPageObjects.DecisionRationaleFormattingOptionCssSelector,
                How.CssSelector, "Attribute:title");
        }

        public string GetConditionDescription()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.ConditionDescriptionCss, How.CssSelector).Text;
        }

        public int GetCodeofCodesofConcernCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.AllCodesofConcernCheckCssSelector,
                How.CssSelector);
        }

        public int GetCodeofConcernDisplayedColumn()
        {
            var col = "";
            if (string.Compare(EnvironmentManager.Browser, BrowserConstants.Iexplorer, StringComparison.OrdinalIgnoreCase) == 0)
            {
                col = SiteDriver.FindElement(ProviderActionPageObjects.ListCodeofColumCssLocator, How.CssSelector).GetCssValue("columns");
            }
            else
            {
                col = SiteDriver.FindElement(ProviderActionPageObjects.ListCodeofColumCssLocator, How.CssSelector).GetCssValue("-webkit-columns");
            }

            return Convert.ToInt32(Regex.Match(col, @"\d+").Value);
        }

        public void ClickOnSelectAllCheckBoxManageCodeofConcern()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SelectAllCodesofConcernCheckBoxCssSelector, How.CssSelector);

        }

        public void ClickOnCodeOfConcernCheckBox()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CodesofConcernRowCheckboxCssLocator, How.CssSelector);

        }
        public bool IsCodeOfConcernCheckBoxChecked()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.CodesofConcernRowCheckboxCssLocator, How.CssSelector)
                 .GetAttribute("class")
                 .Contains("active");
        }

        public void SelectReasonCode(string reasoncode)
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ReasonCodeComboBoxXPath, How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.SendKeys(reasoncode);
            if (!SiteDriver.IsElementPresent(
                string.Format(ProviderActionPageObjects.ReasonCodeListOptionSelectorsXPathTemplate, reasoncode),
                How.XPath))
            {
                element.Click();
            }
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.ReasonCodeListOptionSelectorsXPathTemplate, reasoncode), How.XPath);
            Console.WriteLine("First available Reason Code selected");
        }

        public void ClearReasonCode()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ReasonCodeComboBoxXPath, How.XPath);
            SiteDriver.WaitToLoadNew(300);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.SendKeys(Keys.ArrowDown);
            JavaScriptExecutor.ExecuteClick(string.Format(ProviderActionPageObjects.ReasonCodeListOptionSelectorsXPathTemplate, " "), How.XPath);
        }


        public string GetActionCondtionInputFieldValue(string fieldname)
        {
            return SiteDriver.FindElement(
                string.Format(ProviderActionPageObjects.ActionCondtionInputFieldByLabelXPathTemplate, fieldname), How.XPath)
                .GetAttribute("value");
        }
        public void ClickOnLastSelectedCondtion()
        {
            int lastrow = GetSelectedConditionsCount();
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.SelectedConditionCssSelector + ":nth-of-type(" + lastrow + ")", How.CssSelector);
        }



        public List<string> GetComponentTitlesFromPage()
        {
            var componentTitles = JavaScriptExecutor.FindElements(ProviderActionPageObjects.QuadrantTitleNameCssLocator, How.CssSelector, "Text");
            //8631componentTitles.RemoveAt(0); //First component title is Provider Name which is not required here
            return componentTitles;
        }

        public int GetComponentActionProviderConditionCount()
        {
            return SiteDriver.FindElementsCount(
                ProviderActionPageObjects.ComponentActionProviderConditionCssLocator, How.CssSelector);
        }

        public bool IsProviderConditionComponentPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ComponentActionProviderConditionCssLocator,
                How.CssSelector);
        }

        public string GetEmptyMessageOnUserAddedContion()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.EmptyMessageOnUserAddedContionCssLocator, How.CssSelector).Text;
        }

        public bool IsEmptyMessageOnUserAddedContion()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.EmptyMessageOnUserAddedContionCssLocator, How.CssSelector);
        }

        public string GetSearchConditionInputText()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.SearchConditionInputBoxCssSelector, How.CssSelector).Text;
        }

        public bool IsSearchResultSectionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SearchResultSectionCssLocator,
                How.CssSelector);
        }

        public string GetSearchResultValue()
        {
            return
                SiteDriver.FindElement(ProviderActionPageObjects.SearchResultValueCssLocator,
                    How.CssSelector).Text;
        }

        public bool IsSearchForCondtionByCodeIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SearchConditionCssSelector,
                How.CssSelector);
        }

        public bool IsUserAddedConditionSectionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.UserAddedConditionSectionCssLocator,
                How.CssSelector);
        }

        public bool IsMatchingConditonListPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.MatchingCondtionListCssLocator,
                How.CssSelector);
        }

        public bool IsNewConditionSectionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.NewConditiontionCodeSectionCssLocator, How.CssSelector);
        }

        public bool IsNewAddedCodeCheckBoxChecked()
        {
            return SiteDriver.FindElement(string.Format(
                 ProviderActionPageObjects.NewAddedCodeCheckboxCssTemplate, 1), How.CssSelector)
                 .GetAttribute("class")
                 .Contains("active");
        }

        public bool IsSelectAllCheckBoxPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SelectAllXPath, How.XPath);
        }

        public int GetNewAddedCodeCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.NewAddedCodeCssLocator, How.CssSelector);
        }

        public void ClickOnNewAddedCode(int row)
        {
            SiteDriver.FindElement(string.Format(
                ProviderActionPageObjects.NewAddedCodeCheckboxCssTemplate, row), How.CssSelector);
        }

        public void ClickOnNewAddedSelectedCondition()
        {
            SiteDriver.FindElementAndClickOnCheckBox(ProviderActionPageObjects.SelectedConditionFirstXPath, How.XPath);
        }

        public bool IsFirstSelectedConditionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.SelectedConditionFirstXPath, How.XPath);
        }


        public bool AreToolBarButtonsVisible()
        {

            bool yn = SiteDriver.FindElement(ProviderActionPageObjects.ToolbarButtonsClass, How.CssSelector).Displayed;
            Console.WriteLine(yn ? "ToolBar Buttons Visible ?: Y" : "ToolBar Buttons Visible ?: N");
            return yn;
        }

        public int GetProviderNameLengthInProviderAction()
        {
            return SiteDriver
                .FindElement(ProviderActionPageObjects.PrividerName, How.CssSelector).Text
                .Length;

        }

        public string GetProviderName()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.PrividerName, How.CssSelector).Text;
        }

        public Size ReturnWindowSize()
        {

            return SiteDriver.GetWindowSize();
        }

        public void SwitchToPopUpWindow()
        {
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
        }
        public void CloseAnyPopupIfExist(string windowHandle)
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!windowHandle.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
                }
            }
        }

        public void ClickOnClearUserAddedAction()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ClearUserAddedConditionSectionCssLocator,How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on clearn user added condition button.");
        }

        public void ClickOnCancelUserAddedAction()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CancelLinkOnManageCodeOfConcern, How.CssSelector);
        }



        public void ClickonProviderNotes()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.NoteIconCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
        }

        public bool IsConditionExposureButtonSelected()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.ConditionExposureButtonActiveXpath, How.XPath)
                .GetAttribute("class").Contains("is_selected");
        }

        public void ClickOnConditionExposure()
        {
            var element =
                SiteDriver.FindElement(ProviderActionPageObjects.ConditionExposureButtonActiveXpath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public string GetTooltipOfNotesIconInConditionExposureSection()
        {
            return SiteDriver.FindElement
               (ProviderActionPageObjects.ConditionNotesToolTipXpathLocator, How.XPath).GetAttribute("title");
        }

        public void ClickonProviderNotesinConditionExposureSection()
        {
            var element =
                SiteDriver.FindElement(ProviderActionPageObjects.ConditionNotesToolTipXpathLocator, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
        }

        public int GetConditionNotesAssociatedToTheCondition()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.ConditionNotesListCssSelector, How.CssSelector);

        }



        public string GetEmptyNoteMessage()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.EmptyNoteMessageCssLocator, How.CssSelector).Text;
        }

        public bool IsCaretIconPresentByRowInConditionNotes(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(ProviderActionPageObjects.ConditionNoteCaretIconByRowCssTemplate, row),
                How.CssSelector);
        }

        public String GetConditionNoteRecordByColumnRow(int col, int row = 1)
        {
            return
                SiteDriver.FindElement(
                    string.Format(ProviderActionPageObjects.ConditionNoteRecordsByRowColCssTemplate, row, col),
                    How.CssSelector).Text;
        }

        public bool IsVisibletoClientIconPresentByRowInConditionNotes(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(ProviderActionPageObjects.VisibleToClientIconInConditionNoteByRowCssTemplate, row),
                How.CssSelector);
        }

        public void ClickOnCaretIconOnConditionNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(ProviderActionPageObjects.ConditionNoteCaretIconByRowCssTemplate, row));
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(5000);
            Console.WriteLine("Clicked On Small Exapnd Icon");
        }

        public string GetNoteInConditionNotesByRow(int row)
        {
            return SiteDriver.FindElement(string.Format(ProviderActionPageObjects.ConditionNotesTextCssTemplate, row), How.CssSelector).Text;
        }

        public string GetNoteInConditionNotesByDate(string date)
        {
            return
                JavaScriptExecutor.FindElement(string.Format(
                    ProviderActionPageObjects.ConditionNoteTextCssTemplateByDate, date)).Text;
        }
        public bool IsNoteInConditionNotesPresentByDate(string date)
        {
            return
                JavaScriptExecutor.IsElementPresent(string.Format(
                    ProviderActionPageObjects.ConditionNoteTextCssTemplateByDate, date));
        }


        public string GetDetailsByRowColInConditionNotes(int col = 2, int row = 1)
        {
            return SiteDriver.FindElement(string.Format(ProviderActionPageObjects.ConditionNotesDetailsValueTextCssTemplate, row, col), How.CssSelector).Text;

        }
        public void ClickOnCollapseIconOnNotesByRow(int row)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(ProviderActionPageObjects.ConditionNoteCarrotDownIconByRowCssTemplate, row));

        }

        public bool IsProviderOpenedInViewMode()
        {
            return (SiteDriver.IsElementPresent(ProviderActionPageObjects.LockIconXpath, How.XPath) &&
                    SiteDriver.IsElementPresent(ProviderActionPageObjects.DisabledEditActionConditionCssSelector,
                        How.CssSelector));
        }

        public bool IsOpenScoutCaseSelected()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ScoutCaseSelectedXpath, How.XPath);
        }

        public bool IsOpenScoutCaseDisabled()
        {
            return SiteDriver.FindElement(ProviderActionPageObjects.ScoutCaseXpath, How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsOpenScoutCaseEnabled()
        {
            return !SiteDriver.FindElement(ProviderActionPageObjects.ScoutCaseXpath, How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsOpenScountCaseVisible()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ScoutCaseXpath, How.XPath);
        }

        public void SelectOpenScoutCase()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ScoutCaseXpath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Open Scout Case Checkbox selected");
        }


        public bool CheckForProviderConditionInSuspectProviderRulesTable(string conditionId)
        {
            var temp = string.Format(ProviderSqlScriptObjects.CheckForProviderConditionInSuspectProviderRulesTable, conditionId);
            var list = Executor.GetTableSingleColumn(temp);
            return list[0] == "0";
        }

        public string GetVisibileToForProvSeqAndDate(string prvseq, string date)
        {
            return Executor.GetSingleStringValue(
                string.Format(ProviderSqlScriptObjects.GetVisibleToInNotesForPrvNote, prvseq,
                    EnvironmentManager.ClientUserName, date));
        }

        public bool IsProviderDetailsSectionPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.ProviderDetailSectionToggleCssSelector);
        }

        public bool IsThreeColumnViewPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.TopSectionCollapsed);
        }

        public void ClearInputField(string label)
        {
            JavaScriptExecutor.FindElement(string.Format(
                ProviderActionPageObjects.InputCIUReferralCssTemplate, label)).ClearElementField();
        }

        public bool IsInvalidDecisionRationalePresent()
        {
            bool firstCondition = SiteDriver.IsElementPresent(ProviderActionPageObjects.DecisionRationaleCotivitiUserCssSelector, How.CssSelector);
            var secondCondition = false;
            if (firstCondition)
                secondCondition = SiteDriver.FindElement(
                    Format(ProviderActionPageObjects.DecisionRationaleCotivitiUserCssSelector), How.CssSelector).
                    GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");
            return firstCondition && secondCondition;
        }


        public void SetLengthyNoteToCiuReferral(string text,bool handlePoup=true)
        {
            JavaScriptExecutor.SwitchFrameByJQuery("div:has(>label:contains(Identified Pattern)) iframe.cke_wysiwyg_frame.cke_reset");
            SendValuesOnTextArea("CIU Referral", text,handlePoup);


        }

        public string GetNoteOfCiuReferral()
        {
            JavaScriptExecutor.SwitchFrameByJQuery("div:has(>label:contains(Identified Pattern)) iframe.cke_wysiwyg_frame.cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public bool IsNoCiuReferralMessagePresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.NoCiuReferralCssSelector);
        }

        public int GetCiuReferralRecordRowCount()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.CiuReferralRecordRowCssSelector,
                How.CssSelector);
        }

        public ProviderScoreCardPage ClickOnProviderScorecard()
        {
            var providerScoreCard = Navigator.Navigate<ProviderScoreCardPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(ProviderActionPageObjects.ProviderScoreWidgetXPath, How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on widget Provider score");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderScoreCard.GetStringValue()));
            });
            return new ProviderScoreCardPage(Navigator, providerScoreCard,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public ProviderProfilePage ClickOnWidgetProviderIconToOpenProviderProfile()
        {
            var providerProfile = Navigator.Navigate<ProviderProfilePageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ProviderProfileWidgetXPath, How.XPath);
                Console.WriteLine("Clicked on Provider Profile Icon in widgets");
                SiteDriver.SwitchToLastWindow();
                if (SiteDriver.Title.Contains("Connection request timed out"))
                {
                    Console.WriteLine("Connection request timed out");
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
                    JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.ProviderProfileWidgetXPath, How.XPath);
                    SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderProfilePage.GetStringValue()));
                }
                else
                {

                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderAction.GetStringValue());
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderProfilePage.GetStringValue()));
                }

                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderProfilePage(Navigator, providerProfile, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderProfilePage ClickOnProviderReviewIconToOpenProviderProfile()
        {
            var providerProfile = Navigator.Navigate<ProviderProfilePageObjects>(() =>
            {
                var element = SiteDriver.FindElement(ProviderActionPageObjects.ProfileReviewIndicatorCssSelector,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on Provider Under Review Icon in widgets");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderProfilePage.GetStringValue()));
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderProfilePage(Navigator, providerProfile, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderClaimHistoryPage ClickProviderClaimHistoryToOpenProviderHistory(int row)
        {
            var providerClaimHistory = Navigator.Navigate<ProviderClaimHistoryPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(ProviderActionPageObjects.ProviderClaimHistoryCssSelector,
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                var element2 = SiteDriver.FindElement(
                    string.Format(ProviderActionPageObjects.ClaimHistoryOptionsCssSelectorByRow, row), How.CssSelector);
                SiteDriver.ElementToBeClickable(element2);
                SiteDriver.WaitToLoadNew(500);
                element2.Click();
                Console.WriteLine("Clicked on Provider Claim History Icon in widgets");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimHistoryPopup.GetStringValue()));
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderClaimHistoryPage(Navigator, providerClaimHistory, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ProviderClaimSearchPage ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage()
        {
            var providerClaimSearch = Navigator.Navigate<ProviderClaimSearchPageObjects>(() =>
            {
                if (!IsHxIconClicked())
                    ClickOnProviderClaimHistoryIcon();

                var element =
                    SiteDriver.FindElement(Format(ProviderActionPageObjects.ClaimHistoryOptionsCssSelectorByRow, 3),
                        How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                Console.WriteLine("Clicked on New Provider Claim Search Icon");
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
                WaitForWorkingAjaxMessage();
            });
            return new ProviderClaimSearchPage(Navigator, providerClaimSearch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public void ClickOnExport12MonthsClaimHistory()
        {

            if (!IsHxIconClicked())
                ClickOnProviderClaimHistoryIcon();

            var element =
                SiteDriver.FindElement(Format(ProviderActionPageObjects.ClaimHistoryOptionsCssSelectorByRow, 1),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            ClickOkCancelOnConfirmationModal(true);
            Console.WriteLine("Clicked on Export 12 months History Icon");
            SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");

        }

        public void ClickOnExport3YearsClaimHistory()
        {

            if (!IsHxIconClicked())
                ClickOnProviderClaimHistoryIcon();

            var element =
                SiteDriver.FindElement(Format(ProviderActionPageObjects.ClaimHistoryOptionsCssSelectorByRow, 2),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Clicked on Export 3 years History Icon");
            SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");

        }

        public void ClickOnProviderClaimHistoryIcon()
        {
            var element = SiteDriver.FindElement(ProviderActionPageObjects.ProviderClaimHistoryCssSelector,
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsHxIconClicked()
        {
            return SiteDriver.IsElementPresent(ProviderActionPageObjects.ClaimHistoryIconSelectedCssSelector,
                How.CssSelector);
        }

        public List<string> GetAvailableHistoryOptions()
        {
            if (!IsHxIconClicked())
                ClickOnProviderClaimHistoryIcon();

            var list = JavaScriptExecutor.FindElements(ProviderActionPageObjects.ClaimHistoryOptionsCssSelector, How.CssSelector, "Text");
            return list;
        }


        public string GetFileName(string url = null)
        {
            var downloadPage = NavigateToChromeDownLoadPage();
            var fileName = downloadPage.GetFileName();

            SiteDriver.WaitForCondition(() =>
            {
                if (fileName == "")
                {
                    fileName = downloadPage.GetFileName();
                    return false;
                }
                else
                    return true;
            }, 5000);
            Action action;
            if (url != null)
                action = () => SiteDriver.Open(url);
            else
                action = SiteDriver.Back;
            Navigator.Navigate<ProviderActionPageObjects>(action);
            WaitForPageToLoad();
            WaitForWorkingAjaxMessage();
            WaitForStaticTime(3000);
            return fileName;
        }

        //public int GetWindowCount()
        //{

        //    return SiteDriver.WindowHandles.Count;

        //}

        #endregion

        #region PRIVATE METHODS



        #endregion

            
        public bool IsWorkListProviderSequenceConsistent(ProviderActionPage providerActionPage, string providerSequenceFromGrid)
        {
            Console.WriteLine("Does Provider Sequence {0} from ProviderActionPage equal to Provider Sequence {1} from Grid Section?", providerActionPage, providerSequenceFromGrid);
            return providerActionPage.GetProviderSequence().Equals(providerSequenceFromGrid);
        }

        public void LockProviderFromDB(string getValueInGridByColRow, string userId)
        {
            Executor.ExecuteQuery(string.Format(ProviderSqlScriptObjects.AddProviderLockByProviderSeqUserId, getValueInGridByColRow, userId));
        }

        public string GetProviderLockIconToolTip()
        {
            return SiteDriver.FindElement(
                string.Format(ProviderSearchPageObjects.ProviderLockIconCSSSelector),
                How.CssSelector).GetAttribute("title");
        }

        public int GetLockedProviderCountByUser(string clientName, string username)
        {
            return (int)Executor.GetSingleValue(Format(ProviderSqlScriptObjects.GetProviderCountByuser, ClientEnum.SMTST, username));
        }

        public int ProviderExposureDataItemsCountPresent()
        {
            return SiteDriver.FindElementsCount(ProviderActionPageObjects.ProviderExposureCssSelector, How.CssSelector);
        }

        public void ClickAddressHyperlinkInProviderDetailSection()
        {
            WaitForStaticTime(5000);
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.AddressHyperlinkInProviderDetailSectionCssSelector, How.CssSelector);
        }

        public bool IsGoogleMapPopupPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ProviderActionPageObjects.GoogleMapCssSelector);
        }

        public void CloseGoogleMapPopup()
        {
            JavaScriptExecutor.ExecuteClick(ProviderActionPageObjects.CloseGoogleMapPopupCssSelector, How.CssSelector);
        }

        public void ClickGoogleSearchIcon()
        {
            var element =
                SiteDriver.FindElement(ProviderActionPageObjects.GoogleSearchIconCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(IsGoogleSearchIconPresent);
        }

        public bool IsGoogleSearchIconPresent() => SiteDriver
            .IsElementPresent(ProviderActionPageObjects.GoogleSearchIconCssSelector, How.CssSelector);

        public bool IGoogleSearchPopUpPresent() =>
            SiteDriver.IsElementPresent(ProviderActionPageObjects.GooglePopupPageTitle, How.CssSelector);
        public string GetGoogleSearchInputFieldValue() => SiteDriver
            .FindElement(ProviderActionPageObjects.GoogleSearchInputCssSelector, How.CssSelector).GetAttribute("value");

        public string GetGooglePopupPageTitle() => SiteDriver.FindElement(ProviderActionPageObjects.GooglePopupPageTitle, How.CssSelector).GetAttribute("textContent");
        

        #region DataBase Interaction
        public string GetJobFlagValueFromDatabase(string client)
        {
            return Executor.GetSingleStringValue(string.Format(ProviderSqlScriptObjects.GetJobFlag, client));
        }

        public int GetRealTimeProviderQueueCount(string prvSeq, string conditionID)
        {
            return Convert.ToInt16(Executor.GetSingleValue(string.Format(ProviderSqlScriptObjects.GetRealTimeProviderClaimQueueCount, prvSeq, conditionID)));
        }

        public bool IsOpenCTICaseSetTrueInDatabase(string prvSeq)
        {
            return Executor.GetSingleStringValue(string.Format(ProviderSqlScriptObjects.GetOpenCTICaseForProvider,
                       prvSeq)) == "T";
        }


        public bool IsDocumentAuditActionPresent(string prvseq, string userId)
        {
            bool auditAction = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.SelectDocumentAuditAction, prvseq,
                userId)).Any();
            return auditAction;


        }

        public string GetSearchResultForSingleProcCodeInUserSpecifiedConditionFromDb(string procCode, string codeType)
        {
            return Executor.GetSingleStringValue(Format(
                ProviderSqlScriptObjects.GetSearchResultForSingleProcCodeInUserSpecifiedCondition, procCode, codeType));
        }

        public List<string> GetUserSpecifiedConditonEditList()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.GetEditsForUserSpecifiedCondition);
        }

        public bool IsProviderLocked(string clientName, string prvSeq, string userID)
        {
            var lockedProviderList = Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetProviderLockByUser, clientName, userID));
            return lockedProviderList.Any(x => x.Contains(prvSeq));

        }

        public string GetSearchInputOfGoogleDb(string prvseq) =>
            Executor.GetSingleStringValue(Format(ProviderSqlScriptObjects.GetSearchInputOfGoogle, prvseq)).Trim();

        #endregion


    }
}