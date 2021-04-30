using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.SqlScriptObjects.EditSettingsManager;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.Settings
{
    public class EditSettingsManagerPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private EditSettingsManagerPageObject _editSettingsManagerPage;
        private SideBarPanelSearch _sideBarPanelSearch;
        private GridViewSection _gridViewSection;
        private SideWindow _sideWindow;


        #endregion

        #region PUBLIC PROPERTIES

        public EditSettingsManagerPageObject EditSettingsManager
        {
            get { return _editSettingsManagerPage; }
        }

        #endregion


        #region CONSTRUCTOR

        public EditSettingsManagerPage(INewNavigator navigator, EditSettingsManagerPageObject editSettingsManagerPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, editSettingsManagerPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {

            _editSettingsManagerPage = (EditSettingsManagerPageObject)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);

        }


        #endregion

        #region PUBLIC METHODS

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public GridViewSection GetGridViewSection()
        {
            return _gridViewSection;
        }

        public SideWindow GetSideWindow()
        {
            return _sideWindow;
        }

        #region cob

        public List<string> GetAllLabelListFromUI(string edit) => 
            JavaScriptExecutor.FindElements(Format(EditSettingsManagerPageObject.GetAllLabelsXpathTemplate,edit),How.XPath,"Text");

        public string GetInfoIconByEditAndInputLabel(string edit, string label)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.InfoIconXPathTemplate, edit, label),
                    How.XPath).GetAttribute("title");
        }
        public void SetCOBInputByEditAndInputLabel(string edit, string label, string value)
        {
            SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.COBInputXPathTemplate, edit, label),
                    How.XPath).ClearElementField();
            SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.COBInputXPathTemplate, edit, label),
                    How.XPath).SendKeys(value);
        }
        public string GetCOBInputByEditAndInputLabel(string edit, string label)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.COBInputXPathTemplate, edit, label),
                    How.XPath).GetAttribute("value");
        }
        #endregion


        #region edit details secondary details

        public string GetValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(string.Format(EditSettingsManagerPageObject.ValueByLabelXpathTemplate, label),
                    How.XPath).Text;
        }
        public string GetToolTipByLabel(string label)
        {
            return SiteDriver
                .FindElement(string.Format(EditSettingsManagerPageObject.ToolTipByLabelXpathTemplate, label),
                    How.XPath).GetAttribute("title");
        }

        public string GetToolTipByLabelForDUP(string label)
        {
            return JavaScriptExecutor
                .FindElement(string.Format(EditSettingsManagerPageObject.DUPAnicllarySettingsTooltip, label)).GetAttribute("title");
        }
        #endregion
        #region Modify Edit Settings

        public void ClickOnEditIconByFlag(string flag)
        {
            JavaScriptExecutor.ExecuteClick(Format(EditSettingsManagerPageObject.EditIconByFlagXPathTemplate, flag),
                How.XPath);
            WaitForWorking();
            JavaScriptExecutor.ExecuteToScrollToView("form_component");
        }

        public string GetStatusOnOffLabel(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.StatusOnOffLabelXPathTemplate, index),
                    How.XPath).Text;
        }

        public string GetStatusOnOffTooltip(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.StatusOnOffLabelXPathTemplate, index),
                    How.XPath).GetAttribute("title");
        }

        public void ClickOnStatusRadioButton(int index = 1)
        {
            SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.StatusRadioButtonXPathTemplate, index),
                    How.XPath).Click();
        }

        public bool IsStatusRadioButtonPresent(int index = 3)
        {
            return SiteDriver.IsElementPresent(
                Format(EditSettingsManagerPageObject.StatusOnOffLabelXPathTemplate, index), How.XPath);
        }

        public bool IsStatusRadioButtonChecked(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.StatusRadioButtonXPathTemplate, index),
                    How.XPath).GetAttribute("class").Contains("is_active");
        }

        public bool IsStatusRadioButtonDisabled(int index)
        {
            return SiteDriver.FindElement(Format(EditSettingsManagerPageObject.StatusRadioButtonXPathTemplate, index),
                How.XPath).GetAttribute("class").Contains("is_disabled");
        }

        public string GetReviewStatusCheckBoxText(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxTextXPathTemplate, index),
                    How.XPath).Text;
        }

        public string GetReviewStatusCheckBoxTooltip(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxTextXPathTemplate, index),
                    How.XPath).GetAttribute("title");
        }

        public string GetReviewStatusLabel(int index = 1) => SiteDriver
            .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxTextXPathTemplate, index),
                How.XPath).Text;

        public void ClickOnReviewStatusCheckBox(int index = 1)
        {
            SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxXPathTemplate, index),
                    How.XPath).Click();
        }

        public bool IsReviewStatusCheckBoxChecked(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxXPathTemplate, index),
                    How.XPath).GetAttribute("class").Contains("active");
        }
        public bool IsReviewStatusCheckBoxDisabled(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxXPathTemplate, index),
                    How.XPath).GetAttribute("class").Contains("is_disabled");
        }
        public string GetReviewStatusCheckBoxTitle(int index = 1)
        {
            return SiteDriver
                .FindElement(Format(EditSettingsManagerPageObject.ReviewStatusCheckBoxXPathTemplate, index),
                    How.XPath).GetAttribute("title");
        }
        #endregion

        public void SetInputFieldByInputLabel(string label, string value)
        {
            _sideBarPanelSearch.SetInputFieldByLabel(label, value);
        }

        public ClientSearchPage ClickOnReturnToClientSearch()
        {
            SiteDriver.FindElement(EditSettingsManagerPageObject.ReturnToClientSearchCssSelector,
                How.CssSelector).Click();
            return new ClientSearchPage(Navigator, new ClientSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string VisitAndGetUrlByUrlForUnAuthorizedPage(string url)
        {
            var applicationUrl = BrowserOptions.ApplicationUrl;
            SiteDriver.Open(applicationUrl + url);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.ReturnToTheLastPageLinkCssSelector, How.XPath));
            return SiteDriver.Url;
        }

        public void SearchForEdit(string filter, string value)
        {
            GetSideBarPanelSearch.SelectDropDownListValueByLabel(filter, value);
            GetSideBarPanelSearch.SelectDropDownListValueByLabel("Edit Status", "All");
            GetSideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
        }

        public void ClickOnSidebarIcon(bool close = false)
        {
            if (close && _sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(EditSettingsManagerPageObject.SideBarIconCssSelector, How.CssSelector);
            else if (!close && !_sideBarPanelSearch.IsSideBarPanelOpen())
                JavaScriptExecutor.ExecuteClick(EditSettingsManagerPageObject.SideBarIconCssSelector, How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public IEnumerable<DataRow> GetAllResultsInEditSettingsManagerGrid()
        {
            var dataValues = Executor.GetCompleteTable(EditSettingsManagerSqlScriptObjects.AllResultsInEditSettingsManagerGrid).ToList();

            return dataValues;
        }
        public int GetTotalEditSettingAuditCount(string flag)
        {
            var dataValues = Executor.GetCompleteTable(Format(EditSettingsManagerSqlScriptObjects.EditSettingAudit, flag)).ToList();

            return dataValues.Count;
        }

        public string GetAncillarySettingsForDUP(bool matchOption=true)
        {
            if (matchOption)
                return Executor.GetSingleStringValue(EditSettingsManagerSqlScriptObjects.GetDUPMacthOptionsAncillarySettingFromDb);

            return Executor.GetSingleStringValue(EditSettingsManagerSqlScriptObjects.GetSPECoverrideAncillarySettingsFromDb);
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


        public List<String> GetEditListPerProductType(string productType)
        {
            var editFlagList = Executor.GetTableSingleColumn(Format(EditSettingsManagerSqlScriptObjects.EditFlagList, productType));
            if (editFlagList != null)
            { editFlagList.Insert(0, ""); }
            return editFlagList;
        }
        public List<string> GetReportingFlagList()
        {
            return Executor.GetTableSingleColumn(string.Format(EditSettingsManagerSqlScriptObjects.ReportingFlagList, GetAllEditsInSingleCommaSeparatedString()));
        }
        public string GetAllEditsInSingleCommaSeparatedString()
        {
            return Join(",", Executor.GetTableSingleColumn(EditSettingsManagerSqlScriptObjects.GetAllEdits).ToList());
        }
        public string GetClientCodeValue()
        {
            return SiteDriver.FindElement(EditSettingsManagerPageObject.ClientCodeValueCssSelector, How.CssSelector).Text;
        }


        #region Fall Back Order Settings
        public bool IsFallBackOrderSettingsLabelPresent() =>
             SiteDriver.IsElementPresent(EditSettingsManagerPageObject.FallBackOrderSettingsLabelXPathTemplate,
                How.XPath);

        public int GetCountOfActiveFallBackOrdersFromForm() =>
            JavaScriptExecutor.FindElements(EditSettingsManagerPageObject.GetActiveFallBackOrdersCssSelector, "Text").Count;

        public Dictionary<string, string> GetDataSetsWithFallBackOrderFromDB(string editFlag)
        {
            Dictionary<string, string> dataSetWithFallBackOrder = new Dictionary<string, string>();

            var dataSetList =
                Executor.GetCompleteTable(Format(EditSettingsManagerSqlScriptObjects.GetDataSetForFallBackOrderSettings, editFlag));

            foreach (var dataRow in dataSetList)
            {
                dataSetWithFallBackOrder.Add(dataRow.ItemArray[0].ToString(), dataRow.ItemArray[1].ToString());
            }

            return dataSetWithFallBackOrder;
        }

        public List<List<string>> GetEditDetailsByFlag(string editFlag)
        {
            var list =
                Executor.GetCompleteTable(Format(EditSettingsManagerSqlScriptObjects.EditDetailsByFlag, editFlag));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public List<List<string>> GetCOBEditFlagDetailsByFlag()
        {
            var list =
                Executor.GetCompleteTable(EditSettingsManagerSqlScriptObjects.COBEditFlagDetails);

            var returnList = list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return returnList;
        }

        public List<string> GetAncillarySettingsStatusFromDb()
        {
            var list =
                Executor.GetCompleteTable(EditSettingsManagerSqlScriptObjects.AncillarySettingsStatus);

            var returnList = list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return returnList[0];
        }

        public void UpdateAncillarySettingsStatus()
        {
            Executor.ExecuteQuery(EditSettingsManagerSqlScriptObjects.UpdateAncillarySettingsStatus);
        }

        public List<string> GetAllDataSetsFromModifyEditSettingsForm() =>
            JavaScriptExecutor.FindElements(EditSettingsManagerPageObject.GetAllDataSetsFromFormXPathTemplate, How.XPath, "Text");

        public bool IsDataSetInputFieldEnabled(string dataSet) =>
            SiteDriver.IsElementEnabled(Format(EditSettingsManagerPageObject.InputFieldForEachDataSetXpathTemplate, dataSet), How.XPath);

        public string GetFallBackOrderForDataSet(string dataSet)
        {
            return SiteDriver.FindElement(Format(EditSettingsManagerPageObject.InputFieldForEachDataSetXpathTemplate, dataSet), How.XPath).GetAttribute("value").Trim();
        }


        public void ClickOnCheckboxByDataSetName(string dataSet) =>
            JavaScriptExecutor.FindElement(Format(EditSettingsManagerPageObject.CheckBoxByDataSetCssSelector, dataSet)).Click();

        public void ClickSaveInModifyEditSettingsForm()
        {
            GetSideWindow().Save(waitForWorkingMessage: true);
        }

        public string GetEditorByEditName(string editName) =>
            Executor.GetSingleValue(Format(EditSettingsManagerSqlScriptObjects.GetEditorByFlag, editName)).ToString();

        public void ClickCancelInModifyEditSettingsForm() =>
            SiteDriver.FindElement(EditSettingsManagerPageObject.CancelInEditSettingsFormXPath, How.XPath).Click();

        public void SetFallBackorderForDataSet(string dataSet, string fallbackValue)
        {
            var setOrderElement = SiteDriver.FindElement(Format(EditSettingsManagerPageObject.InputFieldForEachDataSetXpathTemplate, dataSet), How.XPath);
            setOrderElement.ClearElementField();
            setOrderElement.SendKeys(fallbackValue);
            SiteDriver.FindElement(Format(EditSettingsManagerPageObject.InputFieldForEachDataSetXpathTemplate, dataSet), How.XPath).SendKeys(Keys.Tab);
        }

        public List<string> GetDefaultOrderListFromDB(string editName) =>
            Executor.GetTableSingleColumn(Format(EditSettingsManagerSqlScriptObjects.GetUseDefaultOrderList, editName));

        public void ClickUseDefaultOrCurrentButton() =>
            SiteDriver.FindElement(EditSettingsManagerPageObject.UseDefaultOrCurrentButtonXPathTemplate, How.XPath).Click();

        public string GetButtonTextOfUseDefaultOrCurrentBtn() =>
            SiteDriver.FindElement(EditSettingsManagerPageObject.UseDefaultOrCurrentButtonXPathTemplate, How.XPath).Text;

        public List<string> GetAllActiveDataSetsFromTheForm() =>
            JavaScriptExecutor.FindElements(EditSettingsManagerPageObject.ActiveDataSetsXPathTemplate, How.XPath, "Text");

        public Dictionary<string, string> GetLatestFallbackOrderSettingsAuditForDataSets(List<string> dataSetList, string username)
        {
            var result = Executor.GetCompleteTable(Format(EditSettingsManagerSqlScriptObjects.GetLatestFallbackOrderSettingsAuditForDataSets,
                 GetCommaDelimitedColumnNames(), username));

            var auditWithDateTime = new Dictionary<string, string>();

            foreach (var item in result)
            {
                auditWithDateTime.Add(item.ItemArray[0].ToString(), item.ItemArray[1].ToString());
            }


            string GetCommaDelimitedColumnNames()
            {
                var commaDelimited = dataSetList.Select(s => "'" + s + "',");

                string dataSetsForQuery = "";

                foreach (var element in commaDelimited)
                {
                    dataSetsForQuery += element;
                }

                return dataSetsForQuery.Remove(dataSetsForQuery.Length - 1, 1);
            }

            return auditWithDateTime;

        }

        #endregion

        #region Database

        public string GetSystemDateFromDatabase() =>
            Executor.GetSingleStringValue(CommonSQLObjects.GetSystemDateFromDatabase);

        public List<string> GetListOfEditsRequiringFallBackOrderFromDB() =>
            Executor.GetTableSingleColumn(EditSettingsManagerSqlScriptObjects.GetListOfEditsRequiringFallBackOrder);

        public string GetReportingSettingsFromDB() =>
            Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.ReportingSettings, GetGridViewSection().GetGridListValueByCol()[0]));

        public string GetStatusValueForFirstEditInGridFromDB()
        {
            var edit = GetGridViewSection().GetGridListValueByCol()[0].Replace(" ", string.Empty);
            var isNewEdit = Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.IsNewEditByEdit, edit));
            if (GetGridViewSection().GetGridListValueByCol(5)[0] == "COB" || isNewEdit == "T")
                return Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.StatusValueForCOB, edit));
            return Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.StatusValue, edit));
        }


        public List<string> GetTooltipLabelsForASpecificEdit(string edit)
        {
            var column_name = (edit == "CMPU" || edit == "CMPK") ? "MEMBER_LOOK_BACK_COMM" : "MEMBER_LOOK_BACK_MED";
            return Executor.GetTableSingleColumn(Format(EditSettingsManagerSqlScriptObjects.GetToolTipValueForLookBackPeriod,edit,column_name));
        }

        public void UpdateLOBvalueForClient(string lob, string client)
        {
            Executor.ExecuteQuery(Format(EditSettingsManagerSqlScriptObjects.UpdateLOBForClient,lob,client));
        }

        public List<string> GetFallbackSettingsLabel()
        {
            return SiteDriver.FindElements(EditSettingsManagerPageObject.FallBackOrderLaberByCss, How.CssSelector)
                .Select(x => x.Text).ToList();
        }



            #endregion

            #region Ancillary

            public bool IsLabelCorrectForAncillarySetting(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(EditSettingsManagerPageObject.AncillaryLabelCssSelector, label));

        public string AncillarySettingValuesForFOTandFREfromDB(string FOTorFRE)
        {
            var ancillaryValue = Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.GetAncillarySettingValueForFOTandFRE,
                FOTorFRE == "FOT" ? "FOTVAL" : "FREVAL"));


            return ancillaryValue;
        }

        public List<string> GetAncillarySettingsLabelForINVD()
        {
            return JavaScriptExecutor.FindElements(EditSettingsManagerPageObject.AncillarySettingsForINVDCssSelector);
        }

        public List<string> GetLabelToolTipForAncillarySetting(string attribute = "title")
        {
            bool isAncillaryPresent =
                SiteDriver.IsElementPresent(EditSettingsManagerPageObject.AncillarySettingsCssSelector,
                    How.CssSelector);
            if (isAncillaryPresent)
                return SiteDriver.FindElementAndGetAttributeByAttributeName(EditSettingsManagerPageObject.AncillaryLabelTooltipCssSelector, How.CssSelector, attribute).ToList();
            return null;
        }

        public void ClickAncillarySettingsRadioButtons(int button, int row = 1)
        {
            SiteDriver.FindElement(Format(EditSettingsManagerPageObject.AncillaryRadioButtons, button, row), How.CssSelector).Click();

        }


        public List<string> GetEditLabelAncillarySettings(string flag)
        {
            return Executor.GetTableSingleColumn(Format(EditSettingsManagerSqlScriptObjects.GetEditlabelAncillarySettings,
                  flag));
        }

        public bool IsAncillaryRadioButtonEnabled(int num, int row = 1) => SiteDriver
            .FindElement(Format(EditSettingsManagerPageObject.AncillaryRadioButtons, num, row), How.CssSelector).GetAttribute("class").Contains("is_active");

        public bool IsAncillaryRadioButtonEnabledByLabel(int option, string label) => SiteDriver
            .FindElement(Format(EditSettingsManagerPageObject.AncillaryRadioButtonsByLabel, option, label), How.CssSelector).GetAttribute("class").Contains("is_active");

        public List<string> GetAncillarySettingsDropDown(string option)
        {
            var selectoption = option;
            JavaScriptExecutor.FindElement(Format(EditSettingsManagerPageObject.DUPAncillarySettingslabel, selectoption)).Click();
           var element= JavaScriptExecutor.FindElements(Format(EditSettingsManagerPageObject.DUPAncillarySettings,
               selectoption), "Text");
           JavaScriptExecutor.FindElement(Format(EditSettingsManagerPageObject.DUPAncillarySettingslabel, selectoption)).Click();
           return element;
        }

        public void SelectAncillaryDropDownSetting(string label,string value)
        {
            JavaScriptExecutor.FindElement(Format(EditSettingsManagerPageObject.DUPAncillarySettingslabel, label)).Click();
            JavaScriptExecutor.FindElement(Format(EditSettingsManagerPageObject.AncillarysettingsvalueDropdown,label,value)).Click();

        }

        #endregion



        #endregion


    }

}

