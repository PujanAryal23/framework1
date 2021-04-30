using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Nucleus.Service.PageObjects;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.References;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Reference
{
    public class RepositoryPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly RepositoryPageObjects _repositoryPage;
        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanel;
        #endregion

        #region CONSTRUCTOR

        public RepositoryPage(INewNavigator navigator, RepositoryPageObjects repositoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, repositoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _repositoryPage = (RepositoryPageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideBarPanel = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public SideBarPanelSearch GetSideBarPanelSearch()
        {
            return _sideBarPanel;
        }

        public GridViewSection GetGridViewSection()
        {
            return _gridViewSection;
        }
        public List<string> GetReferenceSubMenuList()
        {
            SiteDriver.FindElementAndClickOnCheckBox(RepositoryPageObjects.referenceLabelXpath, How.XPath);
            WaitForStaticTime(4000);
            return GetSubMenu.GetAllSubMenuListByHeaderMenu("Reference");
        }

        #endregion
        public void SelectUnbInReferenceTable()
        {
            _sideBarPanel.SelectDropDownListValueByLabel("Reference Table", "NCCI UNB Rules");
        }

        public List<string> GetSearchResults()
        {
            return JavaScriptExecutor.FindElements(RepositoryPageObjects.searchResultRowCSS, How.CssSelector, "Text");
        }

        public List<List<string>> GetAddonSearchList(List<string> procCodeFrom, List<string> procCodeTo)
        {
            var newList = new List<List<string>>();
            for (int i = 0; i < procCodeFrom.Count; i++)
            {

                var qaManagerList =
                    Executor.GetCompleteTable(string.Format(RepositorySqlScriptObject.GetAddOnTableValueList,
                        procCodeFrom[i], procCodeTo[i]));
                var t = qaManagerList.FirstOrDefault().ItemArray.Select(x => x.ToString()).ToList();
                newList.Add(t);
            }
            return newList;
        }
        public List<List<string>> GetAddonSearchList(string procCodeFrom, string procCodeTo)
        {
            var qaManagerList = Executor.GetCompleteTable(string.Format(RepositorySqlScriptObject.GetAddOnProcTableValueList,
                                                procCodeFrom, procCodeTo));
            return qaManagerList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }
        public List<string> GetUnbSearchList(string dataset)
        {
            // var dataSetVal = string.Format("'{0}'", string.Join("','", dataset.Select(i => i.Replace("'", "''"))));
            var result = Executor.GetCompleteTable(string.Format(RepositorySqlScriptObject.GetUnbTableValueList,
                       dataset));
            var dataRows = result as DataRow[] ?? result.ToArray();
            return dataRows[0].ItemArray.Select(x => x.ToString()).ToList();
        }

        public void ClickOnSearchResultsRow(int row = 1)
        {
            SiteDriver.FindElement(string.Format(RepositoryPageObjects.searchResultGridRowCSS, row), How.CssSelector)
                .Click();
        }

        public string GetDetailsLabel(int row = 1, int column = 1)
        {
            return SiteDriver
                .FindElement(String.Format(RepositoryPageObjects.addOnCodeDetailsLabelCSS, row, column),
                    How.CssSelector).Text;
        }

        public string GetDetailsValue(int row = 1, int column = 1)
        {
            return SiteDriver
                .FindElementAndGetAttribute(String.Format(RepositoryPageObjects.addOnCodeDetailsValueCSS, row, column),
                    How.CssSelector, "title");
        }

        public bool IsNoDataAvailablePresentInitially()
        {
            return _gridViewSection.IsNoDataMessagePresent();
        }

        public bool IsFindReferencePanelPresent()
        {
            return SiteDriver.IsElementPresent(RepositoryPageObjects.FindReferencePanel, How.XPath);
        }

        public bool IsFindReferencePanelControllable()
        {
            /*This function is designed to ensure Reference Panel is Controllable and 
             * to ensure the side bar panel is always open when the function ends
             *to avoid redundancy in subsequent cases. */

            if (_sideBarPanel.IsSideBarPanelOpen())
            {
                _sideBarPanel.ClickOnToggleSidebarPanelButton();
                var toreturn = !_sideBarPanel.IsSideBarPanelOpen();
                _sideBarPanel.ClickOnToggleSidebarPanelButton();
                return toreturn;
            }
            _sideBarPanel.ClickOnToggleSidebarPanelButton();
            return _sideBarPanel.IsSideBarPanelOpen();
        }

        public void FillSideBarOptionByLabel(string label, string value, bool multipleFields = false, int index = 0, bool search = false)
        {
            _sideBarPanel.SetInputFieldByLabel(label, value, multipleFields, index);
            if (search)
                _sideBarPanel.ClickOnFindButton();
        }

        public string GetSideBarValueByLabel(string label, int index)
        {
            SiteDriver.FindElement(
                    string.Format(SideBarPanelSearch.MultipleInputFieldByLabelXpathTemplate, label, index), How.XPath)
                .Click();
            return _sideBarPanel.GetMultipleInputValueByLabel(label, index);
        }

        public string GetSearchResultHeader()
        {
            return SiteDriver.FindElement(RepositoryPageObjects.SearchResultHeaderCssSector, How.CssSelector).Text;
        }

        public bool IsClearLinkPresent()
        {
            return SiteDriver.IsElementPresent(RepositoryPageObjects.ClearLinkXPath, How.XPath);
        }

        public List<List<string>> GetExpectedDataForPrimaryProcCodeFromDb(string primaryProcCode)
        {
            var dataList = Executor.GetCompleteTable(string.Format(RepositorySqlScriptObject.GetAddOnProcTableValueListForPrimaryProcCode,primaryProcCode));
            return dataList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }
    }
}
