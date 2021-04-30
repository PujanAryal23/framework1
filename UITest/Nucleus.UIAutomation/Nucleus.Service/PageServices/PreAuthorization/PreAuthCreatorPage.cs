using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.MicroStrategy;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.SqlScriptObjects.Pre_Auth;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.PreAuthorization
{
    public class PreAuthCreatorPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private PreAuthCreatorPageObjects _preAuthorizationPage;
        private readonly SideWindow _sideWindow;
        private GridViewSection _gridViewSection;
        private readonly FileUploadPage FileUploadPage;
        #endregion
        #region CONSTRUCTOR

        public PreAuthCreatorPage(INewNavigator navigator, PreAuthCreatorPageObjects preAuthorizationPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, preAuthorizationPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor
            )
        {
            _preAuthorizationPage = (PreAuthCreatorPageObjects)PageObject;
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            FileUploadPage = new FileUploadPage(SiteDriver, JavaScriptExecutor);

        }

        #endregion

        #region DB methods

        public List<string> GetBasicProviderInfo(string prvSeq)
        {
            var table = Executor
                .GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetBasicProviderInfo, prvSeq));

            var dataRows = table as DataRow[] ?? table.ToArray();
            return dataRows[0].ItemArray.Select(x => x.ToString()).ToList();
        }

        public string GetCreateAuditNote(string preauthSeq)
        {
            var note = Executor.GetSingleStringValue(string.Format(PreAuthSQLScriptObjects.PreAuthCreatedAudit, preauthSeq)).ToString();
            return note;
        }
        #endregion

        #region PUBLIC METHODS
        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }
        public FileUploadPage GetFileUploadPage
        {
            get { return FileUploadPage; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public void ClickOnSelectOptionTabButton(string optionName)
        {
            var tabButton = 1;
            switch (optionName)
            {
                case "Select Patient":
                    tabButton = 1;
                    break;
                case "Select Provider":
                    tabButton = 2;
                    break;
                case "Add Lines":
                    tabButton = 3;
                    break;
            }

            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthCreatorPageObjects.SelectOptionTabButtonCssTemplate, tabButton), How.CssSelector);
            SiteDriver.WaitToLoadNew(500);
        }


        public void SetInputFieldByLabel(string label, string value, bool tab = false)
        {
            SiteDriver.FindElement(
                    string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath).ClearElementField();

            SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath).SendKeys(value);
            if (tab)
            {
                SiteDriver.FindElement(
                    string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath).SendKeys(Keys.Tab);
            }
        }

        public string GetInputFieldValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPathTemplate, label),
                    How.XPath).GetAttribute("value");
        }


        public string GetValueOnLeftWindowGridByHeaderLabel(string label, int col)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.ValueOnLeftWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath).Text;

        }

        public string GetLabelOnLeftWindowGridByHeaderLabel(string label, int col)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.LabelOnLeftWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath).Text;

        }

        public bool IsLabelOnLeftWindowGridByHeaderLabelPresent(string label, int col)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthCreatorPageObjects.LabelOnLeftWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath);

        }

        public string GetValueOnRightWindowGridByHeaderLabel(string label, int col)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.ValueOnRightWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath).Text;

        }

        public string GetLabelOnRightWindowGridByHeaderLabel(string label, int col)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.LabelOnRightWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath).Text;

        }

        public bool IsLabelOnRightWindowGridByHeaderLabelPresent(string label, int col)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthCreatorPageObjects.LabelOnRightWindowGridByHeaderLabelXPathTemplate, label, col),
                How.XPath);

        }

        public string GetValueByInputLabel(string label)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.ValueByInputLabelXPathTemplate, label),
                How.XPath).Text;

        }

        public List<string> GetLabelsOfInputFieldOnLeftSideOfAddLine()
        {
            return JavaScriptExecutor.FindElements(PreAuthCreatorPageObjects.LabelsOfInputFieldOnLeftSideOfAddLineXPath,
                How.XPath, "Text");
        }

        public void ClickOnGridRowByHeaderLabel(string label)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(PreAuthCreatorPageObjects.GridRowByHeaderLabelXPathTemplate, label),
                How.XPath);

        }

        public bool IsGridRowByHeaderLabelPresent(string label)
        {
            return SiteDriver.IsElementPresent(
                 string.Format(PreAuthCreatorPageObjects.GridRowByHeaderLabelXPathTemplate, label),
                 How.XPath);

        }

        public void ClickOnButton(bool wait = false)
        {
            var element = SiteDriver.FindElement(PreAuthCreatorPageObjects.ButtonCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            if (wait)
                WaitForWorkingAjaxMessage();

        }

        public void ClickOnClearLink()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthCreatorPageObjects.ClearLinkCssLocator, How.CssSelector);

        }
        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthCreatorPageObjects.CancelLinkXPath, How.XPath);

        }

        public void SelectDateInAddLine(int row)
        {
            JavaScriptExecutor.ExecuteClick(String.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPathTemplate, "DOS"), How.XPath);
            SiteDriver.FindElement(String.Format(PreAuthCreatorPageObjects.CalendarSelectorXPathTemplate, row), How.XPath).Click();
        }

        public string GetNoResultFoundMessageByHeaderlabel(string headerLabel)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.NoResultFoundPTagXPathTemplate, headerLabel), How.XPath).Text;
        }


        //public bool IsPageErrorPopupModalPresent()
        //{


        public void ClickOnScenarioDropDown()
        {
            var element =
                SiteDriver.FindElement(PreAuthCreatorPageObjects.ScenarioDropDownCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }
        public List<string> GetScenarioListInAddLine()
        {
            var scenarioList =  JavaScriptExecutor.FindElements(PreAuthCreatorPageObjects.AddLineScenarioDropdownListCssSelector,
                How.CssSelector, "Text");

            scenarioList.Remove("");
            return scenarioList;
        }

        public void SelectScenarioInAddLine(int row)
        {
            SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.AddLineScenarioOptionSelectorXPathTemplate, row), How.XPath).Click();
        }

        public string GetScenarioInputFieldValue()
        {
            return SiteDriver
                .FindElement(PreAuthCreatorPageObjects.AddLineScenarioInputFieldXPath,
                    How.XPath).GetAttribute("value");
        }

        public string GetDefaultValueOfScenario() =>
            SiteDriver.FindElementAndGetAttribute(PreAuthCreatorPageObjects.AddLineScenarioInputFieldXPath,
                How.XPath, "placeholder");

        public void ClickOnSubmitButton()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthCreatorPageObjects.SubmitButtonXpath, How.XPath);
        }
        public string GetInputValueByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                        string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXpath, label), How.XPath)
                    .GetAttribute("value");
        }

       
        public bool IsDocumentUploadIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthCreatorPageObjects.DocumentUploadIcon, How.CssSelector);
        }

      


        #region Upload Pre Auth Document
        public string GetTitleOfUpperRightQuadrant()
        {
            return SiteDriver.FindElement(PreAuthCreatorPageObjects.UpperRightQuadrantTitleCssLocator, How.CssSelector).Text;
        }

        public string GetDocumentUploadFormTitle()
        {
            return SiteDriver.FindElement(PreAuthCreatorPageObjects.UploadDocumentFormHeader, How.CssSelector).Text;
        }

        public bool IsUploadNewDocumentFormPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthCreatorPageObjects.UploadDocumentFormCssSelector,
                How.CssSelector);
        }

        public string GetdocumentCountFromBadge()
        {
            return SiteDriver.FindElement(PreAuthCreatorPageObjects.documentBadgeCount,How.CssSelector).Text;
        }


        public bool IsAddIconDisabledInDocumentUploadForm()
        {
            return SiteDriver.FindElement(PreAuthCreatorPageObjects.AddIconCssSelector, How.CssSelector).GetAttribute("class").Contains("is_disabled");
        }

        public void ClickonAddIconInDocumentUploadForm()
        {
            SiteDriver.FindElement(PreAuthCreatorPageObjects.AddIconCssSelector, How.CssSelector).Click();
        }

        public void clickOnPreauthIcon()
        {
            SiteDriver.FindElement(PreAuthCreatorPageObjects.preauthIconCssSelector, How.CssSelector).Click();
        }
        public void ClickOnCancelLinkOnUploadDocumentForm()
        {
            JavaScriptExecutor.ExecuteClick(PreAuthCreatorPageObjects.CancelLinkCssSelector, How.CssSelector);
        }

        public void ClickOnDocumentUploader()
        {
            SiteDriver.FindElement(PreAuthCreatorPageObjects.DocumentUploadIcon, How.CssSelector).Click();
        }

        public bool IsPreAuthAuditAddedForDocumentUpload(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentUploaded,
                       authseq)).Count() > 0
                ? true
                : false;
        }

       
        public bool IsPreAuthDocumentDeleteIconPresent()
        {
            return SiteDriver.IsElementPresent(PreAuthCreatorPageObjects
                .PreAuthDocumentDeleteIconCssLocator,
                How.CssSelector);
        }

        public void ClickOnDocumentToViewAndStayOnPage(int docrow)
        {
            SiteDriver.FindElement(
                    Format(PreAuthActionPageObjects.PreAuthDocumentNameCssTemplate, docrow),
                    How.CssSelector)
                .Click();
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count - 1]);
        }

        public string GetOpenedDocumentText()
        {
            return SiteDriver.FindElement("body>pre", How.CssSelector).Text;
        }

        public PreAuthActionPage CloseDocumentTabPageAndBackToPreAuthAction()
        {
            var preAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
            });
            return new PreAuthActionPage(Navigator, preAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsAuditAddedForDocumentDownload(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentDownload,
                       authseq)).Count() > 0
                ? true
                : false;
        }

        public bool IsAuditAddedForDocumentDelete(string authseq)
        {
            return Executor.GetTableSingleColumn(Format(PreAuthSQLScriptObjects.PreAuthAuditDocumentDeleted,
                       authseq)).Count() > 0
                ? true
                : false;
        }


        public bool IsDocumentDeletedInDB(string authseq)
        {
            return Executor
                .GetSingleStringValue(Format(PreAuthSQLScriptObjects.GetDeletedDocumentsStatusFromDB, authseq))
                .Equals("T");
        }

        public List<List<string>> GetDocumentUploadInformationFromDb(string authseq)
        {
            List<List<string>> list = new List<List<string>>();
            var temp = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetDocumentUploadValuesFromDb, authseq));
            list = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return list;
        }

        public string UploadedDocumentValueByRowColumn(int docrow, int ulrow, int col)
        {
            return FileUploadPage.AppealDocumentsListAttributeValue(docrow, ulrow, col);
        }

        public bool IsPreAuthDocumentPresent(string fileName)
        {
            return FileUploadPage.IsFollowingAppealDocumentPresent(fileName);
        }
        #endregion
        public void SelectDropDownListValueByLabel(string label, string value, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXpath, label), How.XPath);
            if (!GetInputValueByLabel(label).Equals(value))
            {
                JavaScriptExecutor.ExecuteClick(string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXpath, label), How.XPath);
                SiteDriver.WaitToLoadNew(300);
                try
                {
                    element.ClearElementField();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if (!directSelect) element.SendKeys(value);
                if (!SiteDriver.IsElementPresent(string.Format(PreAuthCreatorPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath)) JavaScriptExecutor.ClickJQuery(string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXpath, label));
                JavaScriptExecutor.ExecuteClick(string.Format(PreAuthCreatorPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            }
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public string GetValueByLabel(string label)
        {
            return
                SiteDriver.FindElement(
                        string.Format(PreAuthCreatorPageObjects.ValueByLabelXPath, label), How.XPath)
                    .Text;
        }

        public void SetInputFieldCreateFormByLabel(string label, string value)
        {
            SiteDriver.FindElement(
                    string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPath, label), How.XPath).ClearElementField();

            SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPath, label), How.XPath).SendKeys(value);
        }

        public string GetInputFieldCreateFormByLabel(string label)
        {
            return SiteDriver.FindElement(
                    string.Format(PreAuthCreatorPageObjects.InputFieldByLabelXPath, label), How.XPath)
                .GetAttribute("value");
        }

        public bool IsCreatePreAuthSectionDisplayed()
        {
            return JavaScriptExecutor.IsElementPresent(PreAuthCreatorPageObjects.CreatePreAuthSectionCssSelector);
        }

        public bool IsLabelAllowedToBeEditedByLabel(string label)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(PreAuthCreatorPageObjects.IsLabelAllowedToBeEditedCssSelector, label));
        }

        public List<string> GetPatientInformationByFromDBByMemberID(string memberID)
        {
            var resultList = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetPatientInfoFromDBByMemberID, memberID));
            var patientInfoList = new List<string>();

            foreach (DataRow row in resultList)
            {
                patientInfoList = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return patientInfoList;
        }

        public void DeletePreauthFromDb(string preauthseq)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeletePreauth,preauthseq));
        }

        public void DeletePreAuthDocumentRecord(string authSeq, string audit_date)
        {
            Executor.ExecuteQuery(string.Format(PreAuthSQLScriptObjects.DeletePreAuthDocumentRecord, authSeq, audit_date));
            Console.WriteLine("Delete Pre Auth Document Record from database  for authSeq<{0}>", authSeq);
        }
        public List<string> GetProviderInformationByFromDBByMemberID(string provNum)
        {
            var resultList = Executor.GetCompleteTable(string.Format(PreAuthSQLScriptObjects.GetProviderInfoFromDBByProvNum, provNum));
            var providerInfoList = new List<string>();

            foreach (DataRow row in resultList)
            {
                providerInfoList = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return providerInfoList;
        }

        public bool IsLabelRequiredField(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(PreAuthCreatorPageObjects.IsLabelRequiredFieldXpath, label), How.XPath);
        }


        public bool IsElementPresent(int row, int col)
        {
            return SiteDriver.IsElementPresent(
                string.Format(GridViewSection.ValueInGridByRowColumnCssTemplate, row, col), How.CssSelector);
        }

        public bool IsLabelPresentInAddedLineInRightPaneOfAddLine(int row, int col)
        {
            return SiteDriver.IsElementPresent(
                string.Format(PreAuthCreatorPageObjects.IsLabelPresentCssSelectorTemplate, row, col), How.CssSelector);
        }

        public bool IsDeleteIconPresentInAddedLine(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(PreAuthCreatorPageObjects.AddLineDeleteIconCssSelectorBYRow, row), How.CssSelector);
        }

        public string GetValueOfAddedLineInRightWindow(int row, int col)
        {
            return SiteDriver
                .FindElement(string.Format(GridViewSection.ValueInGridByRowColumnCssTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetLabelOfAddedLineInRightWindow(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(PreAuthCreatorPageObjects.IsLabelPresentCssSelectorTemplate, row, col), How.CssSelector).Text;
        }

        public bool IsValueCurrency(int row, int col, int value)
        {
            var inputvalue = GetValueOfAddedLineInRightWindow(row, col);
            return inputvalue.Equals(String.Format("{0:C}", value));

        }

        public bool IsAddedLineNumberSorted()
        {
            return _gridViewSection.GetGridListValueByCol(2).IsInAscendingOrder();

        }

        public int GetRowCountOfAddedLine(int row)
        {
            return _gridViewSection.GetGridListValueByCol(row).Count;
        }

        public void DeleteLineNumber(int row, int col)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(GridViewSection.ValueInGridByRowColumnCssTemplate, row, col), How.CssSelector);

        }

        public List<string> GetAddedLineNumberList()
        {
            return _gridViewSection.GetGridListValueByCol(2);
        }

        public string GetProcCodeDescription(string code, string codeType = "DEN", bool excludeExpire = false)
        {
            if (!excludeExpire)
                return Executor.GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetProcDescFromDb, code,
                    codeType));
            return Executor.GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetAllProcDescFromDb, code,
                codeType));
        }

        public Dictionary<string, string> GetListOfAllValuesInTheRightWindowForm()
        {
            var listOfInputValues = new Dictionary<string, string>();
            listOfInputValues.Add("Member ID", GetValueByInputLabel("Member ID"));
            listOfInputValues.Add("First Name", GetValueByInputLabel("First Name"));
            listOfInputValues.Add("Last Name", GetValueByInputLabel("Last Name"));
            listOfInputValues.Add("Pat Seq", GetValueByInputLabel("Pat Seq"));
            listOfInputValues.Add("DOB", GetValueByInputLabel("DOB"));
            listOfInputValues.Add("Group", GetInputFieldValueByLabel("Group"));
            listOfInputValues.Add("Provider Number", GetValueByInputLabel("Provider Number"));
            listOfInputValues.Add("Provider Name", GetValueByInputLabel("Provider Name"));
            listOfInputValues.Add("TIN", GetValueByInputLabel("TIN"));
            listOfInputValues.Add("Provider Sequence", GetValueByInputLabel("Provider Sequence"));
            listOfInputValues.Add("Pre-Auth ID", GetInputFieldValueByLabel("Pre-Auth ID"));
            listOfInputValues.Add("Doc Ref Num", GetInputFieldValueByLabel("Doc Ref Num"));
            return listOfInputValues;
        }

        public bool IsReviewTypeProfessional(string procCode)
        {
            return Executor.GetSingleValue(String.Format(PreAuthSQLScriptObjects.CountOfDentalProfReview, procCode)) > 0;
        }
        #endregion
    }
}
