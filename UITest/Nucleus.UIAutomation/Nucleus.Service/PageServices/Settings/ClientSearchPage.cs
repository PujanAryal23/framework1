using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Common;
using static System.String;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.Settings
{
    public class ClientSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly NewPagination _pagination;
        private readonly SideWindow _sideWindow;
        private readonly FileUploadPage _fileUploadPage;

        #endregion

        #region CONSTRUCTOR

        public ClientSearchPage(INewNavigator navigator, ClientSearchPageObjects clientsearchpage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, clientsearchpage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            clientsearchpage = (ClientSearchPageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _fileUploadPage = new FileUploadPage(SiteDriver,JavaScriptExecutor);

        }

        #endregion

        #region PROPERTIES

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public NewPagination GetPagination
        {
            get { return _pagination; }
        }

        public FileUploadPage GetFileUploadPage
        {
            get { return _fileUploadPage; }
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public List<string> GetAllClientList()
        {
            return
                Executor.GetTableSingleColumn(SettingsSqlScriptObject.GetAvailableClientCode);
        }



        public string GetdefaultValueInFilterList(string label)
        {
            return _sideBarPanelSearch.GetInputValueByLabel(label);
        }



        #endregion
        
        #region public methods

        public void UpdateEnableIpFilterSettingToFalse()
        {
            Executor.ExecuteQuery(SettingsSqlScriptObject.RevertIpFiltering);
        }

        #region ClientSettings

        #region Interop

        public string GetTimeUnitByLabel(string label) => JavaScriptExecutor
            .FindElement(Format(ClientSearchPageObjects.TimeUnitCssSelectorTemplate, label)).Text;

        public List<string> GetTimePickerInputFieldList(string label, int inputIndex = 1)
        {

            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.TimePickerInputFieldByLabelXPathTemplate, label, inputIndex), How.XPath);
            var list = JavaScriptExecutor.FindElements(string.Format(ClientSearchPageObjects.TimePickerListByLabelXpathTemplate, label), How.XPath, "Text");
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.TimePickerInputFieldByLabelXPathTemplate, label, inputIndex), How.XPath);
            return list;

        }

        public void SelectTimePickerInputFieldList(string label, string time, int inputIndex = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.TimePickerInputFieldByLabelXPathTemplate, label, inputIndex), How.XPath);
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.TimePickerValueByLabelXpathTemplate, label, time), How.XPath);

        }

        public string GetTimePickerInputValue(string label, int inputIndex = 1)
        {
            return SiteDriver.FindElement(
                string.Format(ClientSearchPageObjects.TimePickerInputFieldByLabelXPathTemplate, label, inputIndex),
                How.XPath).GetAttribute("value");

        }

        public bool IsRadioButtonDisabledInInteropTab(string label)
        {
            var result =
                SiteDriver.FindElementsAndGetAttributeByClass("is_disabled",
                    string.Format(ClientSearchPageObjects.RadioButtonByLabelOnInterop, label),
                    How.XPath);
            return result.Count == 2 && result.All(x => x.Equals(true));
        }

        public void ClickOnRadioButtonInInteropTab(string label, bool YesNo)
        {
            SiteDriver.FindElement(string.Format(ClientSearchPageObjects.RadioButtonByLabelOnInterop + (YesNo ? "[1]" : "[2]"), label),
                How.XPath).Click();
        }

        public bool GetRadioButtonCheckedValueOnInInteropTab(string label, bool YesNo)
        {
            return SiteDriver.FindElement(
                string.Format(ClientSearchPageObjects.RadioButtonByLabelOnInterop + (YesNo ? "[1]" : "[2]"), label),
                How.XPath).GetAttribute("class").Contains("is_active");
        }

        #endregion


        public string GetLastModifiedByWithDateFromForm() =>
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.ModifiedByWithDateCSSLocator).GetAttribute("textContent");

        public string GetFormHeaderLabel() => SiteDriver
            .FindElement(ClientSearchPageObjects.FormHeaderLabelCssLocator, How.CssSelector).Text;

        #region product/Appeals

        public bool IsNonAppealedFlagsSectionPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.NonAppealedFlagsDivXPathLocator, How.XPath);
        }

        public List<string> GetAllActiveProductsFromProductAppealsTab(string client, List<string> productList)
        {
            List<string> activeProducts = new List<string>();

            foreach (var product in productList)
            {
                if(IsRadioButtonOnOffByLabel(product))
                    activeProducts.Add(product);
            }

            return activeProducts;
        }

        public void ClickAutoCloseAppealRadioButton(bool check)
        {
            if (IsRadioButtonOnOffByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue()))
            {
                if (check) return;
                ClickOnRadioButtonByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue(), false);
                GetSideWindow.Save(waitForWorkingMessage: true);
            }
            else if (check)
            {
                ClickOnRadioButtonByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue());
                GetSideWindow.Save(waitForWorkingMessage: true);
            }
        }
        public void ClickOnRadioButtonToTurnOnOrOff(string label, bool check)
        {
            if (IsRadioButtonOnOffByLabel(label))
            {
                if (check) return;
                ClickOnRadioButtonByLabel(label, false);
                GetSideWindow.Save(waitForWorkingMessage: true);
            }
            else if (check)
            {
                ClickOnRadioButtonByLabel(label);
                GetSideWindow.Save(waitForWorkingMessage: true);
            }
        }
        public List<string> GetAppealDueDatesTableColumnName()
        {
            return JavaScriptExecutor.FindElements(ClientSearchPageObjects.AppealDueDatesTableColumnNameXPath, How.XPath, "Text");
        }

        public string GetTextAreaValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPathTemplate, label), How.XPath)
                .GetAttribute("value");
        }

        public void SetTextAreaValueByLabel(string label, string value)
        {
            var webElement = SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPathTemplate, label), How.XPath);
            webElement.ClearElementField();
            webElement.SendKeys(value);
        }

        public void ClearTextAreaValueByLabel(string label)
        {
            var webElement = SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPathTemplate, label), How.XPath);
            webElement.ClearElementField();
            webElement.SendKeys("a");
            SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPathTemplate, label), How.XPath).SendKeys(Keys.Backspace);

        }

        public bool IsRadioButtonOnOffByLabel(string label, bool active = true)
        {
            return SiteDriver.FindElementAndGetAttribute(
                Format(ClientSearchPageObjects.RadioButtonByLabelXPathTemplate, label, active ? 1 : 2),
                How.XPath, "class").Contains("is_active");
        }

        public bool IsRadioButtonPresentByLabel(string label) =>
            SiteDriver.IsElementPresent(
                Format(ClientSearchPageObjects.RadioButtonByLabelPresentXPathTemplate, label), How.XPath);

        public bool IsDivByLabelPresent(string label)
        {
            return SiteDriver.IsElementPresent(Format(ClientSearchPageObjects.DivByLabelXPathTemplate, label),
                How.XPath);
        }

        public void ClickOnRadioButtonByLabel(string label, bool active = true)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(ClientSearchPageObjects.RadioButtonByLabelXPathTemplate, label, active ? 1 : 2),
                How.XPath);
        }

        public List<string> GetActualProductList()
        {
            return JavaScriptExecutor.FindElements(ClientSearchPageObjects.ProductListXPath, How.XPath, "Text");
        }


        public List<string> GetAppealDueDatesInputFieldsByLabel(string productLabel, bool skipMRR = true)
        {
            var turnAroundDays = SiteDriver.FindElementsAndGetAttribute("value",
                Format(ClientSearchPageObjects.AppealDueDatesInputByLabelXPathTemplate, productLabel), How.XPath);

            if(skipMRR)
            {
                //Skipping MRR turnaround day
                turnAroundDays.RemoveAt(1);
                return turnAroundDays;
            }

            return turnAroundDays;
        }

        public int GetAppealDueDateProductRowCount()
        {
            return SiteDriver.FindElementsCount(ClientSearchPageObjects.AppealDueDateProductRowXPath, How.XPath);
        }

        public void SetAppealDueDatesAllInputTextBox(string value)
        {
            var elements =
                SiteDriver.FindElements(ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath);
            int i = 0;
            foreach (var webElement in elements)
            {
                if (webElement.Enabled) 
                {
                    webElement.ClearElementField();
                    webElement.SendKeys(value);
                }
                i++;
            }
        }


        public bool IsAppealDueDateCheckBoxPresent(string type)
        {
            return SiteDriver.IsElementPresent(string.Format(ClientSearchPageObjects.AppealDueDateCalculationCheckbox, type),How.XPath);
        }

        public string GetAppealDueDateSettingsTypeTooltip(string type)
        {
            return SiteDriver.FindElementAndGetAttribute(string.Format(
                ClientSearchPageObjects.AppealDueDateCalculationCheckbox, type),How.XPath,"title");
        }
        public string GetAppealDueDateSettingsTooltip()
        {
            return JavaScriptExecutor.FindElement(
                ClientSearchPageObjects.AppealDueDateSettingsTooltip
              ).GetAttribute("title");
        }

        

        public void ClearAppealDueDatesAllInputTextBox()
        {
            var element =
                SiteDriver.FindElements(ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath);
            int i = 0;
            foreach (var webElement in element)
            {
                if (webElement.Enabled)
                {
                    webElement.ClearElementField();
                }
                i++;
            }
        }

        public List<string> GetAppealDueDatesAllInputTextBox(bool skipDisabledDueDates = false)
        {
            if (skipDisabledDueDates)
            {
                var elements =
                    SiteDriver.FindElements(ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath);

                List<string> listOfDueDates = new List<string>();

                foreach (var element in elements)
                {
                    if (element.Enabled)
                        listOfDueDates.Add(element.GetAttribute("value"));
                }

                return listOfDueDates;
            }
            
            return
                SiteDriver.FindElementsAndGetAttribute("value", ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath);
        }

        public bool IsRecordReviewDisabledInAppealDueDateCalculation(bool shouldBeDisabled)
        {
            var elements =
                SiteDriver.FindElements(ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath).Where((x, j) => j % 4 == 0);
            
            return shouldBeDisabled?  elements.All(element => !element.Enabled) :  elements.All(element => element.Enabled);
        }

        public bool IsMedicalRecordReviewDisabledInAppealDueDateCalculation(bool shouldBeDisabled)
        {
            var elements =
                SiteDriver.FindElements(ClientSearchPageObjects.AppealDueDatesAllInputTextBoxXPath, How.XPath).Where((x, j) => (j - 1) % 4 == 0);

            return shouldBeDisabled ? elements.All(element => !element.Enabled) : elements.All(element => element.Enabled);
        }

        public bool IsMRRRecordReviewExpirationLabelPresent(string label) =>
            JavaScriptExecutor.IsElementPresent(
                Format(ClientSearchPageObjects.MRRRecordRequestExpirationLabelCssSelectorTemplate, label));

        public string GetMRRRecordReviewExpirationInputValue() => JavaScriptExecutor
            .FindElement(ClientSearchPageObjects.MRRRecordRequestExpirationInputBoxCssSelector)
            .GetAttribute("value");

        public void SetMRRRecordReviewExpirationInputValue(string value)
        {
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.MRRRecordRequestExpirationInputBoxCssSelector).Clear();
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.MRRRecordRequestExpirationInputBoxCssSelector)
                .SendKeys(value);
        }

        public string GetFWAVRecordReviewExpirationInputValue()
        {

            return JavaScriptExecutor
                .FindElement(ClientSearchPageObjects.FWAVRecordRequestExpirationInputBoxCssSelector)
                .GetAttribute("value");

        } 

        public void SetFWAVRecordReviewExpirationInputValue(string value)
        {
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.FWAVRecordRequestExpirationInputBoxCssSelector).Clear();
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.FWAVRecordRequestExpirationInputBoxCssSelector)
                .SendKeys(value);
        }
        public int GetAppealWorkaroundDays(string product = "CV")
        {


            // var days=   SiteDriver.FindElementsAndGetAttribute("value",
            //       string.Format(ClientSearchPageObjects.GetAppealDueDays, product), How.CssSelector);
            var days = JavaScriptExecutor.FindElement(string.Format(ClientSearchPageObjects.GetAppealDueDays, product))
                .GetAttribute("value");
            return Convert.ToInt32(days);
        }

        public bool IsAppealDueDatesInputFieldsByLabelPresent(string productLabel)
        {
            return SiteDriver.IsElementPresent(
                Format(ClientSearchPageObjects.AppealDueDatesInputByLabelXPathTemplate, productLabel), How.XPath);
        }
        public void SetAppealDueDatesInputByLabelAndColumn(string productLabel, int col, string value)
        {
            var webElement = SiteDriver.FindElement(
                Format(ClientSearchPageObjects.AppealDueDatesInputByLabelAndColumnXPathTemplate, productLabel, col),
                How.XPath);
            webElement.ClearElementField();
            webElement.SendKeys(value);

        }

        public string GetAppealDueDatesInputByLabelAndColumn(string productLabel, int col)
        {
            return SiteDriver.FindElementAndGetAttribute(Format(ClientSearchPageObjects.AppealDueDatesInputByLabelAndColumnXPathTemplate, productLabel,col), How.XPath,"value"
            );

        }

        public bool IsEditSettingsIconEnabled() =>
            SiteDriver.FindElement(DefaultPageObjects.EditSettingsIcon, How.CssSelector).GetAttribute("class").Contains("is_active");

        #endregion

        public string GetClientSettingsSidePaneHeaderText() =>
            SiteDriver.FindElement(ClientSearchPageObjects.ClientSettingsSidePaneHeaderCSSLocator, How.CssSelector).Text;

        #region CONFIGURATION TAB

        public string GetInfoHelpTooltipByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InfoHelpIconByLabelCSSLocator, label)).GetAttribute("title");

        public string GetInputTextBoxValueByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label)).GetAttribute("value");

        public void SetInputTextBoxValueByLabel(string label, string value)
        {
            WaitForStaticTime(1000);
            var element =
                JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label));
            element.ClearElementField();
            element.SendKeys(value);
        }


        public bool IsHolidayOptionSetToTrueInDatabase(string option)
        {
            var list = Executor.GetCompleteTable(SettingsSqlScriptObject.GetExcludeHolidayDataFromDb);
            var holidaylist = list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            switch (option)
            {
                case "Cotiviti":
                    return (holidaylist[0][0].Equals("T")) && (holidaylist[0][1].Equals("F"));
                case "Client":
                    return (holidaylist[0][0].Equals("F")) && (holidaylist[0][1].Equals("T"));
                default:
                    return (holidaylist[0][0].Equals("T")) && (holidaylist[0][1].Equals("T"));
            }
        }

        public List<string> GetDropDownListForClientSettingsByLabel(string label)
        {
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label)).Click();
            var list = JavaScriptExecutor.FindElements(Format(ClientSearchPageObjects.DropdownListConfigSettingsCSSLocator, label), "Text");
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label)).Click();

            return list;
        }

        public void SelectDropDownListForClientSettingsByLabel(string label, string value, bool directSelect = false)
        {
            JavaScriptExecutor.FindElement(string.Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label)).Click();
            var element = JavaScriptExecutor.FindElement(string.Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label));
            element.ClearElementField();
            element.SendKeys(value);
            if (!SiteDriver.IsElementPresent(string.Format(ClientSearchPageObjects.DropdownValueConfigSettingsCSSLocator, value), How.XPath))
            {
                JavaScriptExecutor.FindElement(string.Format(ClientSearchPageObjects.InputTextBoxCSSLocator, label)).Click();
            }

            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.DropdownValueConfigSettingsCSSLocator, value), How.XPath);
            SiteDriver.WaitToLoadNew(300);

            Console.WriteLine($"<{value}> Selected in <{label}>");
        }

        public string GetHeaderHelpIcon(string label) =>
            SiteDriver.FindElement(Format(ClientSearchPageObjects.HeaderHelpIconXPathTemplate, label),
                How.XPath).GetAttribute("title");

        #endregion

        #region Security tab

        public bool AreSecurityTabSpecificationsPresent(string passwordLabel, string clientAccessLabel, string whiteListingLabel)
        {
            return JavaScriptExecutor.IsElementPresent(ClientSearchPageObjects.DaysTextCssSelector)
                   &&
                   JavaScriptExecutor.IsElementPresent(
                       Format(ClientSearchPageObjects.SecurityTabOptionsByLabelCssSelector, passwordLabel))
                   &&
                   JavaScriptExecutor.IsElementPresent(
                       Format(ClientSearchPageObjects.SecurityTabOptionsByLabelCssSelector, clientAccessLabel))
                   &&
                   SiteDriver.IsElementPresent(ClientSearchPageObjects.SecurityTabWhiteListingHeaderCssSelector,
                       How.CssSelector)
                   &&
                   JavaScriptExecutor.IsElementPresent(
                       Format(ClientSearchPageObjects.SecurityTabOptionsByLabelCssSelector, whiteListingLabel));
        }

        public string GetDefaultValueForClientUserAccess()
        {
            return SiteDriver
                .FindElement(ClientSearchPageObjects.DefaultTextForClientUserAccessCssSelector,
                    How.CssSelector).GetAttribute("textContent");
        }

        public bool AreAllTextAreasDisabledForSecurityTab()
        {
            var result =
                SiteDriver.FindElements(ClientSearchPageObjects.SecurityTabInputFieldsXpath,
                    How.XPath).Select(x => x.Enabled).ToList();
            return result.All(c => c == false) && result.Count > 0;
        }

        public bool AreYesNoRadioButtonsPresent()
        {
            return JavaScriptExecutor.IsElementPresent(
                       Format(ClientSearchPageObjects.YesNoRadioBuutonByTextCssSelector, "YES"))
                   &&
                   JavaScriptExecutor.IsElementPresent(
                       Format(ClientSearchPageObjects.YesNoRadioBuutonByTextCssSelector, "NO"));
        }

        public void SetIp(string value)
        {
            var webElement = SiteDriver
                .FindElement(ClientSearchPageObjects.IpTextAreaCssLocator, How.CssSelector);
            webElement.ClearElementField();
            webElement.SendKeys(value);
        }

        public void SetEmptyIp()
        {
            var webElement = SiteDriver
                .FindElement(ClientSearchPageObjects.IpTextAreaCssLocator, How.CssSelector);
            webElement.ClearElementField();
            webElement.SendKeys("a");
            SiteDriver
                .FindElement(ClientSearchPageObjects.IpTextAreaCssLocator, How.CssSelector).SendKeys(Keys.Backspace);
        }

        public bool IsCotivitiAccessByIPAddressPresent()
        {
            return JavaScriptExecutor.IsElementPresent(
                Format(ClientSearchPageObjects.YesNoRadioBuutonByTextCssSelector, "Cotiviti access by IP address"));
        }

        public bool IsTextAreaPresentByLabel(string label)
        {
            return SiteDriver.IsElementPresent(Format(ClientSearchPageObjects.TextAreaByLabelXPath, label), How.XPath);
        }

        public string GetValueOfTextBox(string label)
        {
            return SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPath, label), How.XPath)
                .GetAttribute("value");
        }

        public bool IsTextAreaByLabelDisabled(string label)
        {
            return SiteDriver
                .FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPath, label), How.XPath)
                .GetAttribute("class").Contains("is_disabled");
        }

        public bool IsInputFieldByLabelDisabled(string label)
        {
            return !JavaScriptExecutor
                .FindElement(Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label)).Enabled;

        }

        public bool IsInputFieldByLabelPresent(string label)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label));
        }

        public List<string> GetIpList(string label)
        {
            var list = new List<string>();
            if (GetValueOfTextBox(label).Contains(','))
            {
                list = GetValueOfTextBox(label).Split(',').ToList();
            }
            else
                list.Add(GetValueOfTextBox(label));
            return list;
        }

        public List<string> GetWhiteListedIpFromDatabase() => GetCommonSql.GetWhiteListedIpFromDatabase();

        public void SetMultipleIpAddresses(List<string> clientIPList)
        {
            var textarea = SiteDriver.FindElement(Format(ClientSearchPageObjects.TextAreaByLabelXPath,
                SecurityTabEnum.LimitAccessByClientIPAddress.GetStringValue()), How.XPath);

            var ip = "";
            for (int i = 0; i < clientIPList.Count; i++)
            {
                ip += clientIPList[i] + ',';

            }
            ip = ip.Remove(ip.Length - 1);

            textarea.ClearElementField();
            textarea.SendKeys(ip);

            Console.Write("Comma separated ips: " + ip + " entered");
        }

        public bool IsClientSettingsFormReadOnly()
        {
            return SiteDriver
                .FindElement(ClientSearchPageObjects.ClientSettingsFormCssSelector, How.CssSelector)
                .GetAttribute("class").Contains("read_only");
        }

        #endregion

        #region WORKFLOW TAB

        public string GetToolTipTextForCompleteBatchWithPendedClaims(string label) =>
             JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.CompleteBatchWithPendedClaimsHelpIconCssLocator, label))
                .GetAttribute("title");

        public bool IsRowDisplayedByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(
                Format(ClientSearchPageObjects.FieldRowWhenAutomatedBatchReleaseIsYesCssLocator, label));

        public void SetInputTextBoxValueByLabelInWorkflowInteropTab(string label, string value)
        {
            WaitForStaticTime(1000);
            var element =
                JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label));
            element.ClearElementField();
            element.SendKeys(value);
        }

        public string GetInputTextBoxValueByLabelInWorkflowTab(string label) =>
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label)).GetAttribute("value");

        public List<string> GetListOfAllDataInWorkflowTab(List<string> productList, bool isAutomatedBatchReleaseOn = true)
        {
            var listOfAllData = new List<string>();

            listOfAllData.Add(IsRadioButtonOnOffByLabel(WorkflowTabEnum.AutomatedBatchRelease.GetStringValue()) ? "YES" : "NO");

            if (isAutomatedBatchReleaseOn)
            {
                listOfAllData.Add(GetSideWindow.GetDropDownInputFieldByLabel(WorkflowTabEnum.BeginClaimRelease.GetStringValue()));
                listOfAllData.Add(GetInputTextBoxValueByLabelInWorkflowTab(WorkflowTabEnum.ClaimReleaseInterval.GetStringValue()));
                listOfAllData.Add(GetSideWindow.GetDropDownInputFieldByLabel(WorkflowTabEnum.ReturnFileFrequency.GetStringValue()));
                listOfAllData.Add(GetSideWindow.GetDropDownInputFieldByLabel(WorkflowTabEnum.FailureAlert.GetStringValue()));
            }

            foreach (var product in productList)
                listOfAllData.Add(IsRadioButtonOnOffByLabel(product) ? "YES" : "NO");

            return listOfAllData;
        }

        public void SelectDropDownItemByLabelInWorkflowTab(string label, string value, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label));

            if (!GetSideWindow.GetDropDownInputFieldByLabel(label).Equals(value))
            {
                JavaScriptExecutor.FindElement(string.Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label)).Click();
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

                if (!SiteDriver.IsElementPresent(Format(ClientSearchPageObjects.DropDownInputListForTimePickerByLabelAndValueCssLocator, label, value), How.XPath))
                    JavaScriptExecutor.ClickJQuery(string.Format(ClientSearchPageObjects.InputFieldByLabelCssTemplate, label));

                JavaScriptExecutor.ExecuteClick(Format(ClientSearchPageObjects.DropDownInputListForTimePickerByLabelAndValueCssLocator, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
            }
        }

        public List<string> GetBatchCompleteStatusForProductsByClient(string client, List<string> productList)
        {
            var allProductColumnNames = "";
            var queryTrail = "_BATCH_COMPLETE_STATUS, 0)";

            foreach (var product in productList)
            {
                var prod = product == "CV" ? "PCI" : product;
                allProductColumnNames += "NVL(" + prod + queryTrail + ", ";
            }

            allProductColumnNames = allProductColumnNames.Remove(allProductColumnNames.LastIndexOf(','));

            return GetCommonSql.GetBatchCompleteStatusForProducts(allProductColumnNames, client);
        }


        public List<string> GetProductListInWorkflowTab() =>
            JavaScriptExecutor.FindElements(ClientSearchPageObjects.ProductListForWorklistTabCssSelector, "Text");

        public void UpdateRevertBatchCompleteStatus(string client) =>
            GetCommonSql.UpdateRevertBatchCompleteStatus(client);


        #endregion

        public bool IsAllRadioButtonDisabled()
        {
            var result =
                SiteDriver.FindElementsAndGetAttributeByClass("is_disabled", ClientSearchPageObjects.AllRadioButtonXPath,
                    How.XPath);
            return result.All(c => c) && result.Count > 0;
        }

        public bool IsAllTextAreaDisabled()
        {
            var result =
                SiteDriver.FindElements(ClientSearchPageObjects.AllTextAreaXPath,
                    How.XPath).Select(x => x.Enabled).ToList();
            return result.All(c => c == false) && result.Count > 0;
        }

        public bool IsAllTextBoxDisabled()
        {
            var result =
                SiteDriver.FindElements(ClientSearchPageObjects.AllTextBoxXPath,
                    How.XPath).Select(x => x.Enabled).ToList();
            return result.All(c => c == false) && result.Count > 0;
        }

        public List<string> GetClientSettingsTabList()
        {
            return JavaScriptExecutor.FindElements(ClientSearchPageObjects.ClientSettingsTabListCssLocator, How.CssSelector,
                "Text");
        }

        public string GetSelectedClientSettingTab()
        {
            return SiteDriver.FindElement(ClientSearchPageObjects.SelectedClientSettingTabCssLocator,
                How.CssSelector).Text;
        }

        public void ClickOnClientSettingTabByTabName(string tabName)
        {
            SiteDriver.FindElement(Format(ClientSearchPageObjects.ClientSettingTabXPath, tabName),
                How.XPath).Click();
            WaitForStaticTime(1000);
        }

        public void SelectCalculationType(string type)
        {
          JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.AppealCalculationType,
                    type)).Click();
        }
        public string GetAppealDayTypeFromDB()
        {
            return Executor.GetSingleStringValue(SettingsSqlScriptObject.GetDCIAppealDayType);
        }
        public bool IsClientSettingsTabDisabled(string tabName)
        {
            return SiteDriver.FindElement(Format(ClientSearchPageObjects.ClientSettingTabXPath, tabName),
                How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        public bool IsClientSettingsSectionDisabled()
        {
            return JavaScriptExecutor.FindElement(ClientSearchPageObjects.SettingsSectionCSSSeletor).GetAttribute("class").Contains("read_only");
        }

        public void ClickOnStatusRadioButton(bool active = true)
        {
            SiteDriver.FindElement(Format(ClientSearchPageObjects.StatusRadioButtonXPathTemplate, active ? 1 : 2),
                How.XPath).Click();
        }

        public string GetStatusRadioButtonLabel(bool active = true)
        {
            return SiteDriver.FindElement(
                  string.Format(ClientSearchPageObjects.StatusRadioButtonLabelXPathTemplate, active ? 2 : 3),
                  How.XPath).Text;
        }
        public bool IsActiveInactiveStatusRadioButtonClicked(bool active = true)
        {
            return SiteDriver.FindElement(
                string.Format(ClientSearchPageObjects.StatusRadioButtonXPathTemplate, active ? 1 : 2),
                How.XPath).GetAttribute("class").Contains("is_active");
        }

        public bool IsActiveInactiveStatusRadioButtonDisabled(bool active = true)
        {
            return SiteDriver.FindElement(
                string.Format(ClientSearchPageObjects.StatusRadioButtonXPathTemplate, active ? 1 : 2),
                How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        #endregion

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public void UpdateDefaultSchemaValue()
        {
            Executor.ExecuteQuery(SettingsSqlScriptObject.UpdateDefaultSchemaValue);
        }
        public List<List<string>> GetClientSettingAuditFromDatabase(string clientCode, int fetchRow = 4)
        {
            var list = Executor.GetCompleteTable(Format(SettingsSqlScriptObject.GetClientSettingAudit, clientCode, fetchRow));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }
        public List<List<string>> GetClientSettingAuditFromDatabase(string clientCode, string columnName, int fetchRow = 100)
        {
            var list = Executor.GetCompleteTable(Format(SettingsSqlScriptObject.GetClientSettingAuditByColumnForToday, clientCode, columnName, fetchRow));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }
        public List<List<string>> GetPrimaryDataResultFromDatabase(string clientName, string clientCode)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(string.Format(SettingsSqlScriptObject.GetPrimaryDataForClientSearch, clientName, clientCode));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList;
        }

        public int GetInactiveIconCount()
        {
            return SiteDriver.FindElementsCount(ClientSearchPageObjects.InactiveIcon, How.CssSelector);
        }



        public void SetAppealEmailInDB(string clientcode, string email = null)
        {
            Executor.ExecuteQuery(Format(SettingsSqlScriptObject.UpdateEmailAddressForClientInDB, clientcode, email));
        }

        public void SwitchSendAppealEmailvalue(string clientcode, bool status = true)
        {
            Executor.ExecuteQuery(Format(SettingsSqlScriptObject.UpdateSendAppealEmailStatus, clientcode, status ? 'Y' : 'N'));
        }

        public int GetResultFromDatabaseForPartialMatchOnClientName(string partialClientName)
        {
            return Convert.ToInt32(Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.GetClientCodeForPartialMatchesOnClientName, partialClientName)));
        }

        public List<string> GetClientFromDatabaseByProduct()
        {
            return Executor.GetTableSingleColumn(SettingsSqlScriptObject.GetClientCodeFromDatabaseByProduct);
        }

        //Don't hard delete appeals!
        /*public void DeleteAppealsOnClaim(string claseq)
        {
            var temp = string.Format(SettingsSqlScriptObject.DeleteAppealsOnAClaim, claseq);
            Executor.ExecuteQuery(temp);
        }*/

        public List<string> GetProductLabels()
        {
            int i = 4;
            List<string> list = new List<string>();
            for (var j = 0; j < 5; j++)
            {
                var text = GetGridViewSection.GetLabelInGridByColRow(i);
                text = Regex.Match(text, @"\w+").ToString();
                list.Add(text);
                i = i + 2;
            }
            return list;
        }

        public List<String> GetActiveProductListForClientDB()
        {
            var orderofProducts = new[] { "CV", "FFP", "FCI", "DCA", "NEG", "RXI", "COB" };
            var newList = new List<String>();
            var productList =
                Executor.GetCompleteTable(string.Format(SettingsSqlScriptObject.ActiveProductList));

            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }

            var finalListProducts = orderofProducts.Zip(newList, (order, list) => list == "T" ? order : "-1").ToList();
            finalListProducts.RemoveAll(x => x.Equals("-1"));
            return finalListProducts;
        }

        public bool IsCheckMarkPresent(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(ClientSearchPageObjects.CheckMarkIconCssSelectorByLabel, label), How.XPath);
        }


        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            var isDisabled = JavaScriptExecutor.ClickAndGetWithJquery(ClientSearchPageObjects.FindButtonCssLocator,
                ClientSearchPageObjects.DisabledFindButtonCssLocator, 10) != null;
            return isDisabled;
        }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(ClientSearchPageObjects.FindButtonCssLocator, How.CssSelector);
        }



        public void SearchByClientCodeToNavigateToClientProfileViewPage(string clientCode, string tab = "General", bool isInactive = false)
        {
            _sideBarPanelSearch.OpenSidebarPanel();
            if (isInactive)
                _sideBarPanelSearch.SelectDropDownListValueByLabel("Client Status", "Inactive");
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client Code", clientCode);
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
            GetGridViewSection.ClickOnGridRowByRow();
            if (tab == ClientSettingsTabEnum.General.GetStringValue()) return;
            ClickOnClientSettingTabByTabName(tab);
            if (tab == ClientSettingsTabEnum.Custom.GetStringValue())
                WaitForWorking();
            GetSideWindow.ClickOnEditIcon();
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(ClientSearchPageObjects.FilterOptionListByCss, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;
        }

        public void ClickOnClearSort()
        {
            ClickOnFilterOptionListRow(4);
            Console.WriteLine("Clicked on Clear sort option.");
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(String.Format(ClientSearchPageObjects.FilterOptionValueByCss, row)
                , How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(ClientSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }


        public List<string> GetSecondaryDataFromDataBase()
        {
            var secondaryData = Executor.GetCompleteTable(string.Format(SettingsSqlScriptObject.GetSecondaryDataForClientSearch));
            var infoList = new List<List<string>>();
            foreach (DataRow row in secondaryData)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];
        }

        public string GetClientDetailHeader()
        {
            return SiteDriver
                .FindElement(ClientSearchPageObjects.ClientDetailHeaderByCSS, How.CssSelector).Text;

        }

        public string GetSecondaryDetailsLabels(int row, int col)
        {
            return SiteDriver.FindElement(string.Format(ClientSearchPageObjects.DetailsLabelByCss, row, col), How.XPath).Text;
        }

        public string GetSecondaryDetailsValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(
                    string.Format(ClientSearchPageObjects.ClientSecondaryDetailsValueByCss, label), How.XPath)
                .Text;
        }



        public bool IsClientLogoUploadSectionPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.ClientLogoUploadSectionXpath, How.XPath);
        }

        public bool IsCIWUploadSectionPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.CIWUploadSectionXpath, How.XPath);
        }


        public string GetCIWLastUpdatedNameFromDb()
        {
            return Executor.GetSingleStringValue(SettingsSqlScriptObject.GetCIWLastUpdatedNameFromDb);
        }


        public string GetCIWLastUpdatedDateFromDb()
        {
            return Executor.GetSingleStringValue(SettingsSqlScriptObject.GetCIWLastUpdatedDateFromDb);
        }

        public string GetLastModifiedInformation()
        {
            return SiteDriver.FindElement(ClientSearchPageObjects.CIWLastModifiedByXpath, How.XPath).Text;
        }
        public string GetLastModifiedInformationForClientLogo()
        {
            return SiteDriver.FindElement(ClientSearchPageObjects.ClientLogolastModifiedByXpath, How.XPath).Text;
        }


        public bool IsChooseFileButtonDisabledInCIWFileUploadSection()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.ChooseFileButtonDisabledCssSelector, How.CssSelector);
        }
        public bool IsChooseFileButtonDisabledForClientLogoUploadSection()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.ChooseFileButtonByCSS, How.CssSelector);
        }



        public string ClickOnCIWIconAndGetFileName()
        {
            ClickOnCIWIcon();
            WaitForStaticTime(3000);
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

            return fileName;
        }

        public void ClickOnCIWIcon()
        {
            JavaScriptExecutor.FindElement(ClientSearchPageObjects.DownloadCIWIconCssSelector).Click();
            Console.WriteLine("Clicked on Download CIW icon.");
            ClickOkCancelOnConfirmationModal(true);
            WaitForStaticTime(2000);
        }


        public List<string> GetCIWModifiedDateInformationFromAuditTable()
        {
            var data = Executor.GetCompleteTable(string.Format(SettingsSqlScriptObject.GetCIWModifiedDateInformationFromAuditTable));
            var infoList = new List<List<string>>();
            foreach (DataRow row in data)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];
        }
        public List<string> GetlogoModifiedDateInformationFromAuditTable()
        {
            var data = Executor.GetCompleteTable(string.Format(SettingsSqlScriptObject.GetLogoModifiedDateInformationFromAuditTable));
            var infoList = new List<List<string>>();
            foreach (DataRow row in data)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList[0];
        }
        public void ClickOnGridRowByClientName(string value)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.GridRowByClientNameXpathTemplate, value), How.XPath);
            Console.WriteLine("Clicked on Grid With Client Name: {0}", value);
            WaitForWorking();
        }


        public List<String> GetClientGeneralSettingFromDatabase()
        {
            var settings =
                Executor.GetCompleteTable(SettingsSqlScriptObject.GetClientGeneralSettingFromDataBase);
            var newList = new List<String>();

            foreach (DataRow row in settings)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }

            return newList;
        }

        public string GetLogoModifiedDate(string clientcode)
        {
            return Executor.GetSingleStringValue(Format(SettingsSqlScriptObject.GetLogoModifiedDateFromDb, clientcode));
        }

        public string GetLogoModifiedByUser(string clientcode)
        {
            return Executor.GetSingleStringValue(Format(SettingsSqlScriptObject.GetLogoModifiedByFromDb, clientcode));
        }
        public string GetUserSequenceForUser(string userid)
        {
            return Executor.GetSingleStringValue(Format(SettingsSqlScriptObject.GetUserSeqFromUserId, userid));
        }

        public string GetRedirectURLForClient(string clientName)
        {
            return Executor.GetSingleStringValue(Format(SettingsSqlScriptObject.GetRedirectURLFromDatabase, clientName));
        }

        public bool IsExternalIDPLogoutURLSectionPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.ExternalIDPLogoutURLSectionXpath,How.XPath);
        }

        public void ScrollNonAppealFlagsIntoView()
        {
            GetSideWindow.ScrollIntoView(ClientSearchPageObjects.NonAppealedFlagsLabelCssLocator);
        }








        #endregion

        #region Custom Fields

        public bool IsCustomFieldsLabelPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.CustomFieldLabelXpathSelector,
                How.XPath);
        }
        public bool IsAvailableFieldsDropdownPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.AvailableFieldsXpathLocator, How.XPath);
        }

        public List<string> GetCustomFieldList()
        {
            var result = SiteDriver.FindElementsAndGetAttribute("value",
                ClientSearchPageObjects.CustomFieldsLabelCssSelector, How.CssSelector);

            return result;
        }
        public void ClickOnPHISelectorCheckbox(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.PHISelectorCheckboxByRowCssSelector, row), How.CssSelector);
        }

        public List<string> GetHeaderLabelInCustomField()
        {
            var dtList = SiteDriver.FindDisplayedElementsText(ClientSearchPageObjects.HeaderLabelCssLocator, How.CssSelector);
            return dtList = dtList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        }

        public int GetAvailableListCount()
        {
            return SiteDriver.FindElementsCount(ClientSearchPageObjects.AvailableListCssSelector, How.CssSelector);
        }

        public List<string> GetNumericalLabelInOrderColumn()
        {
            var list = SiteDriver.FindDisplayedElementsText(ClientSearchPageObjects.OrderColumnCssSelector, How.CssSelector);
            return list;
        }

        public int IsPHISelectorChecked()
        {
            return SiteDriver.FindElementsCount(ClientSearchPageObjects.PHISelectorSelectedCheckBoxCssSelector,
                How.CssSelector);

        }

        public string GetNoFieldSelectedMessage()
        {
            return SiteDriver.FindElement(ClientSearchPageObjects.EmptyMessageCssSelector, How.CssSelector)
                .Text;
        }

        public void ClickOnMoveUpCaretIcon(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.MoveUpCaretIconCssSelector, row), How.CssSelector);
        }

        public void ClickOnMoveDownCaretIcon(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.MoveDownCaretIconCssSelector, row), How.CssSelector);
        }

        public void ClickOnDeleteIcon(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClientSearchPageObjects.DeleteIconCssSelector, row), How.CssSelector);
        }

        public bool IsAllTextBoxPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.AllTextBoxXPath, How.XPath);
        }

        public List<List<string>> GetCustomFieldListFromDatabase(string clientCode)
        {
            var newList = new List<List<string>>();
            var list = Executor.GetCompleteTable(Format(SettingsSqlScriptObject.GetCustomFieldList, clientCode));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray
                    .Select(x => x.ToString()).ToList();
                newList.Add(t);

            }

            return newList;
        }

        public void DeleteCustomFields(string clientcode)
        {
            Executor.ExecuteQuery(string.Format(SettingsSqlScriptObject.DeleteCustomFieldList, clientcode));
        }

        public List<string> GetCustomFieldDataListByColumn(int col = 1)
        {
            return SiteDriver.FindDisplayedElementsText(
                string.Format(ClientSearchPageObjects.CustomFieldColumnCssSelector, col), How.CssSelector);
        }
        public int GetTotalDeleteIcon()
        {
            return SiteDriver.FindElements(ClientSearchPageObjects.AllDeleteIconCssSelector, How.CssSelector).Count();
        }
        public List<string> GetCustomFieldLabelInputList()
        {
            return JavaScriptExecutor.FindElements(ClientSearchPageObjects.CustomFieldLabelListCssSelector, How.CssSelector, "Attribute:value");
        }

        public List<string> GetPHISelectorCheckboxList()
        {
            var list = SiteDriver.FindElementAndGetAttributeByAttributeName(ClientSearchPageObjects.PHISelectorCheckboxCssSelector, How.CssSelector, "class");
            List<string> newlist = new List<string>();
            foreach (var val in list)
            {
                if (val.Contains("active"))
                    newlist.Add("Y");
                else
                    newlist.Add("N");

            }

            return newlist;
        }



        public void DeleteCustomLabelByRow(int row = 1)
        {
            Clear(string.Format(ClientSearchPageObjects.CustomFieldLabelByRowCssSelector, row),
                How.CssSelector);
        }

        public bool IsPHISelectorCheckboxDisabled()
        {
            var result =
                SiteDriver.FindElementsAndGetAttributeByClass("is_disabled",
                    ClientSearchPageObjects.PHISelectorCheckboxCssSelector,
                    How.CssSelector);
            return result.All(c => c) && result.Count > 0;
        }

        public void ChangeProcessingTypeOfClient(string client, string processingType)
        {
            _sideBarPanelSearch.SelectDropDownListValueByLabel("Client Code",
                client);
            _sideBarPanelSearch.ClickOnFindButton();
            WaitForWorkingAjaxMessage();
            _gridViewSection.ClickOnGridRowByRow();
            _sideWindow.ClickOnEditIcon();
            _sideWindow.SelectDropDownListValueByLabel(GeneralTabEnum.ProcessingType.GetStringValue(), processingType);
            _sideWindow.Save(waitForWorkingMessage: true);

        }

        public string GetEditSettingsToolTip() => SiteDriver.FindElement(DefaultPageObjects.EditSettingsIcon, How.CssSelector).Text;

        public bool IsAppealDueDateCheckBoxChecked(string type="Business")
        {
            return SiteDriver.IsElementPresent(String.Format(ClientSearchPageObjects.AppealDueDateCalculationTypeCheckMarkByProductandLabel,type),How.XPath);
        }

        public bool IsExcludeHolidaysLabelPresent()
        {
            return SiteDriver.IsElementPresent(ClientSearchPageObjects.ExcludeHolidaysLabel, How.XPath);
        }

        public List<string> GetExcludeHolidaysOptions()
        {
            return JavaScriptExecutor.FindElements(ClientSearchPageObjects.GetExcludeHolidayOptions, How.XPath,"Text");
        }

        public bool IsExcludeHolidaysOptionCheckBoxChecked(string option = "Cotiviti")
        {
            return SiteDriver.IsElementPresent(String.Format(ClientSearchPageObjects.ExcludeHolidaysOptionCheckMarkByCotivitiOrClient, option), How.XPath);
        }

        public void SelectHolidayOption(string option)
        {
            JavaScriptExecutor.FindElement(Format(ClientSearchPageObjects.ExcludeHolidayOptionCheckBox,
                option)).Click();
        }
    }



    //public bool IsCheckBoxAdjecentToProviderFlaggingChecked()
    //{
    //    return SiteDriver.FindElement(
    //            ProviderActionPageObjects.ProviderFlaggingCheckBoxCssSelector, How.CssSelector)
    //        .GetAttribute("class")
    //        .Contains("active");
    //}

    #endregion


}
