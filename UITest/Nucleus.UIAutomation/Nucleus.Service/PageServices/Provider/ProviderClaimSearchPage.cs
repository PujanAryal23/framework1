using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.SqlScriptObjects.Logic;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using How = UIAutomation.Framework.Common.How;
using Nucleus.Service.PageObjects.Claim;
using static System.String;
using static System.Console;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderClaimSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly ProviderClaimSearchPageObjects _providerClaimSearchPageObjects;
        private readonly SideWindow _sideWindow;
        private GridViewSection _gridViewSection;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly string _originalWindow;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CalenderPage _calenderPage;
        private readonly NewPagination _pagination;

        #endregion

        # region CONSTRUCTOR

        public ProviderClaimSearchPage(INewNavigator navigator, ProviderClaimSearchPageObjects providerClaimSearchPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerClaimSearchPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerClaimSearchPageObjects = (ProviderClaimSearchPageObjects)PageObject;
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _commonSqlObjects = new CommonSQLObjects(Executor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _calenderPage = new CalenderPage(SiteDriver);
            _pagination = new NewPagination(SiteDriver,JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public NewPagination GetPagination => _pagination;

        public ProviderSearchPage NavigateToProviderSearchFromProviderClaimSearchPage() => ClickOnSearchIconToReturnToProviderActionPage().ClickOnSearchIconAtHeaderReturnToProviderSearchPage();

        public ProviderActionPage ClickOnSearchIconToReturnToProviderActionPage()
        {
            var providerAction = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                SiteDriver.FindElement(ProviderClaimSearchPageObjects.SearchIconToReturnToProviderActionCssSelector, How.CssSelector).Click();
                Console.WriteLine("Clicked on Search Icon in Provider Claim Search page to navigate back to Provider Action page");
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderActionPage(Navigator, providerAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }


        public bool IsLabelPresent(string label)
        {
            switch (label)
            {
                case "Claim No:":
                case "Adj Stats:":
                case "Proc Code:":
                case "Rev Code:":
                    return SiteDriver.IsElementPresent(Format(ProviderClaimSearchPageObjects.ClaimLineDetailsLabelXPath, label), How.XPath);
                case "Modifiers":
                case "Dx Codes":
                    return SiteDriver.IsElementPresent(Format(ProviderClaimSearchPageObjects.ModifiersDxCodesLabelXPath, label), How.XPath);
                default:
                    return false;
            }
        }

        public string GetClaimDetailsValueByLabel(string label)
        {
            return SiteDriver.FindElement(
                            Format(ProviderClaimSearchPageObjects.ClaimLineDetailsValueByLabelXPath, label), How.XPath).Text;
        }

        public string GetProcCodeValueByLabel(string label)
        {
            return SiteDriver.FindElement(
                Format(ProviderClaimSearchPageObjects.ProcCodeValueByLabelXpath, label), How.XPath).Text;
        }

        public string GetDxCodesValueByLabel(string label, int index)
        {
            return SiteDriver
                .FindElement(Format(ProviderClaimSearchPageObjects.DxCodesValueByLabelXPath, label, index),
                    How.XPath).Text;
        }

        public string GetModifierValueByLabel(string label)
        {
            return SiteDriver
                .FindElement(Format(ProviderClaimSearchPageObjects.ModifiersValueByLabelXpath, label),
                    How.XPath).Text;
        }




        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(ProviderClaimSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public string GetFilterOptionTooltip()
        {
            return SiteDriver.FindElement(ProviderClaimSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector).GetAttribute("title");
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(ProviderClaimSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(ProviderClaimSearchPageObjects.FilterOptionListByCss, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(String.Format(ProviderClaimSearchPageObjects.FilterOptionValueByCss, row)
                , How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public void clickOnClearSort()
        {
            ClickOnFilterOptionListRow(8);
            Console.WriteLine("Clicked on Clear sort option.");
        }

        public int GetCountOfProviderClaimSearchResultByBeginDOSEndDOS(string provSeq, string beginDOS, string endDOS)
        {
            var resultList = new List<List<string>>();
            var list = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.ProviderClaimResultsFromDb, provSeq, beginDOS, endDOS));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                resultList.Add(t);
            }

            return resultList.Count;
        }

        public List<List<string>> GetClaimsearcResultInAscendingOrder(string provseq, string beginDos, string endDos)
        {
            var list = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.ProviderClaimResultsFromDb, provseq, beginDos, endDos));
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();

        }

        public List<List<string>> GetClaimsearcResultInDescendingOrder(string provseq, string beginDos, string endDos)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.ProviderClaimresultsFromDbDescOrder, provseq, beginDos, endDos));
            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList;
        }

        public string GetProviderSequenceFromHeader()
        {
            return JavaScriptExecutor.FindElement(ProviderClaimSearchPageObjects.ProviderSequenceFromHeaderCssLocator).Text;
        }

        public List<string> GetActiveFlagsFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.FlagList);
        }

        public ClaimActionPage ClickOnClaimSeqInGridToOpenClaimActionPopUp(int row, int col)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                GetGridViewSection.ClickOnGridByRowCol(row, col);
                WriteLine("Clicked on claim sequence, navigating to claim action popup");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public List<double> GetPaidListinSearchResult(int col = 10)
        {
            var list = _gridViewSection.GetGridListValueByCol(col).Select(s => double.Parse(s.Split('$')[1])).ToList();
            return list;

        }

        public bool IsListInAscendingOrderByPaid()
        {
            return GetPaidListinSearchResult().IsInAscendingOrder();
        }

        public bool IsListInDescendingOrderByPaid()
        {
            return GetPaidListinSearchResult().IsInDescendingOrder();
        }

        public List<string> GetSecondaryDetailsForClaimLineData(string provSeq, string claseq, string linno)
        {
            var resultList = new List<string>();
            var list = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.ProviderClaimLineDetailsSecondaryDataFromDb, provSeq, claseq, linno));
            foreach (DataRow row in list)
            {
                resultList = row.ItemArray.Select(x => x.ToString()).ToList();
            }
            return resultList;
        }

        public List<string> GetClaimSeqForMultipleClaSub(string prvseq, string claseq)
        {
            var resultList = new List<string>();
            var list = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetAllClasubForProvider, prvseq, claseq));
            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString());
                resultList.Add(t.ElementAt(0));
            }
            return resultList;
        }

        public void SearchByClaimSequence(string claseq)
        {
            GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claseq);
            GetSideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
        }

        public void SearchByBeginDOSAndEndDOS(string beginDOS, string endDOS)
        {
            GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            GetSideBarPanelSearch.SetDateFieldFrom("Begin DOS - End DOS", beginDOS);
            GetSideBarPanelSearch.SetDateFieldTo("Begin DOS - End DOS", endDOS);
            GetSideBarPanelSearch.ClickOnFindButton();
            WaitForWorking();
        }
        #endregion
    }
}
