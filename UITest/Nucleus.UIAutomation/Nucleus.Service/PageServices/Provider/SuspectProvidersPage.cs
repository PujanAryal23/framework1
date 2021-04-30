using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using OpenQA.Selenium.Support.UI;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.Provider
{
    public class SuspectProvidersPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS
        private readonly SuspectProvidersPageObjects _suspectProvidersPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly NewPagination _pagination;
        private readonly string _originalWindow;

        #endregion

        public SuspectProvidersPage(INewNavigator navigator, SuspectProvidersPageObjects suspectProvidersPageObject, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, suspectProvidersPageObject, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _suspectProvidersPage = (SuspectProvidersPageObjects) PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _pagination = new NewPagination(SiteDriver, JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
        }

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }


        #region Provider Decision Quadrant Graphic

        //Find Panel Provider RiskQuadrant
        public void ClickOnRiskProviderQuadrantInFindPanel(int quadrant = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(SuspectProvidersPageObjects.RiskProviderQuadrantInFindPanelCssTemplate, quadrant),
                How.CssSelector);
            WaitForStaticTime(500);
        }

        //Find Panel
        public bool IsQuadrantRectSelectedByQuadrantInFindPanel(int quadrant)
        {
            return SiteDriver.FindElement(
                    string.Format(SuspectProvidersPageObjects.RiskProviderQuadrantInFindPanelCssTemplate, quadrant),
                    How.CssSelector)
                .GetAttribute("class").Contains("selected");
        }

        public string GetRiskQuadrantText(int quadrant, int textrow)
        {
            var t= SiteDriver.FindElement(string.Format(SuspectProvidersPageObjects.RiskQuadrantTextCssTemplate,quadrant,textrow), How.CssSelector)
                .Text;
            return t;
        }

        public bool IsQuadrantRectSelectedByQuadrant(int quadrant )
        {
            return SiteDriver.IsElementPresent(
                string.Format(SuspectProvidersPageObjects.SelectedQuadrantRectCssTemplate, quadrant), How.CssSelector);
        }

        #endregion

        #region database query and action

        public void LockProviderFromDB(string getValueInGridByColRow, string userId)
        {
            Executor.ExecuteQuery(string.Format(ProviderSqlScriptObjects.AddProviderLockByProviderSeqUserId, getValueInGridByColRow, userId));
        }
        public void DeleteProviderLockByProviderSeq(string prvseq)
        {
            Executor.ExecuteQuery(string.Format(ProviderSqlScriptObjects.DeleteProviderLockByProviderSeq, prvseq));
        }

        public string TotalCountOfSuspectProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.SuspectProviderCount)[0];

        }

        public string TotalCountOfClientSuspectProvidersFromDatabase()
        {
            return Executor.GetTableSingleColumn(ProviderSqlScriptObjects.ClientSuspectProviderCount)[0];

        }

        public void UpdateVactionCactionInTriggeredCondition(string prvSeq, string conditionId, char vAction, char cAction)
        {
            Executor.ExecuteQuery(string.Format(ProviderSqlScriptObjects.UpdateVactionCactionInTriggeredCondition, vAction, cAction, prvSeq, conditionId));
        }


        public List<string> FullOrFacilityNameFromDatabase(string name, string type = "firstname")
        {
             if (type == "firstname")
            return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.GetFullOrFacilitynameFromFirstName,name));
            else
                return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.GetFullOrFacilitynameFromLastName,name));

        }

        public List<string> FullOrFacilityNameForClient(string providername,string type="firstname")
        {
            if (type == "firstname")
                return Executor.GetTableSingleColumn(
                    string.Format(ProviderSqlScriptObjects.GetFullorFacilitynameForClientFromFirstName, providername));
            
            else
                return Executor.GetTableSingleColumn(
                    string.Format(ProviderSqlScriptObjects.GetFullOrFacilitynameForClient, providername));

        }


        #endregion

        public List<string> GetRightSideSubHeader()
        {
            return JavaScriptExecutor.FindElements(SuspectProvidersPageObjects.RightSideSubHeaderCssLocator, How.CssSelector, "Text");
        }

        public bool IsListDateSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder();
        }

        public bool IsListDateSortedInDescendingOrder(int col)
        {
            var list = GetSearchResultListByCol(col).ToList().Select(DateTime.Parse).ToList();
            return list.IsInDescendingOrder();
        }
       
        public bool IsListIntSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).Select(x=>double.Parse(x, System.Globalization.NumberStyles.Currency)).ToList().IsInAscendingOrder();
        }

        public bool IsListIntSortedInDescendingOrder(int col)
        {
            var list = GetSearchResultListByCol(col).ToList().Select(x => double.Parse(x, System.Globalization.NumberStyles.Currency)).ToList();
            return list.IsInDescendingOrder();
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInAscendingOrder();

        }

        public bool IsListStringSortedInDescendingOrder(int col)
        {
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }
        

        public List<string> GetFilterLabelList()
        {
            //return SiteDriver.
            return null;
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public NewPagination GetPagination
        {
            get { return _pagination; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public bool IsSuspectProviderSubMenuPresent()
        {

            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.SuspectProviders), How.XPath);
        }

        public string GetDateFieldFrom(string dateName)
        {
            return _sideBarPanelSearch.GetDateFieldFrom(dateName);
        }

        public void SetDateFieldFrom(string dateName, string date)
        {
            _sideBarPanelSearch.SetDateFieldFrom(dateName, date);
        }

        public string GetDateFieldTo(string dateName)
        {
            return _sideBarPanelSearch.GetDateFieldTo(dateName);
        }

        public void SetDateFieldTo(string dateName, string date)
        {
            _sideBarPanelSearch.SetDateFieldTo(dateName, date);
        }

        public List<string> GetSearchResultListByCol(int col)
        {
            var list = GetGridViewSection.GetGridListValueByCol(col);
            list.RemoveAll(string.IsNullOrEmpty);
            return list;
        }


        public string GetProviderRiskXAxisLabel()
        {
            return SiteDriver.FindElement(string.Format(SuspectProvidersPageObjects.SuspectProviderRiskXAxisLabel,7),
                How.CssSelector).Text;
        }

        public string GetProviderRiskYAxisLabel()
        {
            return SiteDriver.FindElement(string.Format(SuspectProvidersPageObjects.SuspectProviderRiskXAxisLabel, 6),
                How.CssSelector).Text;
        }

        public string GetXAxisMidPointValue()
        {
            return SiteDriver.FindElement(string.Format(SuspectProvidersPageObjects.SuspectProviderRiskXAxisLabel, 5),
                How.CssSelector).Text;
        }

        public string GetYAxisMidPointValue()
        {
            return SiteDriver.FindElement(string.Format(SuspectProvidersPageObjects.SuspectProviderRiskXAxisLabel, 2),
                How.CssSelector).Text;
        }


        public bool IsNoDataAvailablePresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.NoDataAvailableXPathTemplate, How.XPath);
        }

        public List<string> GetDropdownValueList(string label)
        {
            return JavaScriptExecutor.FindElements(string.Format(SuspectProvidersPageObjects.DropdownListXPathTemplate, label), How.XPath, "Text");

        }

        public List<string> GetSpecialtyList()
        {
            return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.SpecialtyList));
        }

        public List<string> GetStateList()
        {
            return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.StateList));
        }

        public List<string> GetConditionIdList()
        {
            return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.ConditionIDList));
        }

        public List<string> GetPreviouslyViewedProviderSeqList()
        {
            return SiteDriver.FindDisplayedElementsText(SuspectProvidersPageObjects.PreviousViewedProviderLinkCssLocator, How.CssSelector);
        }

        public ProviderActionPage ClickOnPreviouslyViewedProviderSequenceAndNavigateToProviderActionPage()
        {
            var newClaimAction = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                SiteDriver.FindElement(SuspectProvidersPageObjects.PreviousViewedProviderLinkCssLocator, How.CssSelector).Click();
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                Console.WriteLine("Clicked on Provider Sequence");
            });
            return new ProviderActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool ShouldCosist(string expectedValue, string actualValue, string message, bool takeScreenshot = false)
        {
            try
            {
                if (expectedValue.Contains(actualValue))
                {
                    Console.WriteLine(message + " : Expected String contains <{0}>", actualValue);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SearchByProviderSequence(string prvSeq)
        {
            GetSideBarPanelSearch.ClickOnClearLink();
            GetSideBarPanelSearch.SetInputFieldByLabel("Provider Seq", prvSeq);
            GetSideBarPanelSearch.ClickOnFindButton();
            Console.WriteLine("Searched by PrvSeq<{0}>", prvSeq);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitToLoadNew(500);

        }

        public string GetProviderNpiFromDatabase(string prvSeq)
        {
            return Executor.GetTableSingleColumn(string.Format(ProviderSqlScriptObjects.GetNpiForProvider,prvSeq))[0];
        }

        public bool IsProviderLockIconPresent(string prvSeq)
        {
            return SiteDriver.IsElementPresent(
                string.Format(SuspectProvidersPageObjects.LockIconByProviderSequenceXPathTemplate, prvSeq), How.XPath);
        }

        public bool IsReviewIconByRowPresent(int i = 1)
        {
            return SiteDriver
                .IsElementPresent(string.Format(ProviderSearchPageObjects.ReviewIconByRowCssSelector, i),
                    How.CssSelector);
        }

        public string GetReviewIconTooltip()
        {
            return SiteDriver
                .FindElement(string.Format(ProviderSearchPageObjects.ReviewIconByRowCssSelector, 1),
                    How.CssSelector).GetAttribute("title");
        }
        
        public ProviderActionPage ClickOnProviderSequenceAndNavigateToProviderActionPage(string prvSeq)
        {

            var newClaimAction = Navigator.Navigate<ProviderActionPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(string.Format(SuspectProvidersPageObjects.ProviderSequenceValueXPathTemplate,prvSeq), How.XPath);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                Console.WriteLine("Clicked on Provider Sequence<{0}>", prvSeq);
            });
            return new ProviderActionPage(Navigator, newClaimAction,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }
        
        public string IsLockIconPresentAndGetLockIconToolTipMessage(string prvSeq, string pageUrl, bool isCotivitiUser=true)
        {

            JavaScriptExecutor.ExecuteScript("window.open()");
            var handles = SiteDriver.WindowHandles.ToList();

            var loginPage = Navigator.Navigate<LoginPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(handles[1]);
              SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));
            });

            var login = isCotivitiUser ? new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            ).LoginAsHciAdminUser() : new LoginPage(Navigator, loginPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            ).LoginAsClientUser();
            login.NavigateToSuspectProviders();
            WaitForStaticTime(1500);
            _sideBarPanelSearch.OpenSidebarPanel();
            SearchByProviderSequence(prvSeq);
            WaitForWorkingAjaxMessage();
            SiteDriver.WaitForCondition(() => IsProviderLockIconPresent(prvSeq), 3000);
            var isProviderLockIconPresent = IsProviderLockIconPresent(prvSeq);
            var prvLockToolTipMessage = "";
            if (isProviderLockIconPresent)
                prvLockToolTipMessage =
                    SiteDriver.FindElement(
                        string.Format(ProviderSearchPageObjects.LockIconByProviderSequenceXPathTemplate, prvSeq),
                        How.XPath).GetAttribute("title");
            return prvLockToolTipMessage;
        }        
        
        public int GetProviderScoreByScoreLevel(ScoreBandEnum riskScoreBandEnum)
        {
            switch (riskScoreBandEnum)
            {
                case ScoreBandEnum.HIGH:
                {
                    return Convert.ToInt16(SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderScoreByHighScoreLevelXPath,
                        How.XPath).Text);
                }
                    
                case ScoreBandEnum.ELEVATED:
                {
                    return Convert.ToInt16(SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderScoreByElevatedScoreLevelXPath,
                        How.XPath).Text);
                }
                    
                case ScoreBandEnum.MODERATE:
                {

                    return Convert.ToInt16(SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderScoreByModrateScoreLevelXPath,
                        How.XPath).Text);
                }
                case ScoreBandEnum.LOW:
                {
                    return Convert.ToInt16(SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderScoreByLowScoreLevelXPath,
                        How.XPath).Text);
                }
                default:
                    return -1;

            }
        }
        public List<int> GetGridListValueforScore()
        {
            var t = JavaScriptExecutor.FindElements(SuspectProvidersPageObjects.ProviderScoreGridResultListByCssSelector, "Text");
            return t.Select(s => Convert.ToInt32(s)).ToList();
            //return JavaScriptExecutor.FindElements(SuspectProvidersPageObjects.ProviderScoreGridResultListByCssSelector, "Text");
        }
        
        public ProviderActionPage NavigateToProviderAction(Action function)
        {
            var _providerActionPage = Navigator.Navigate<ProviderActionPageObjects>(function);
            return new ProviderActionPage(Navigator, _providerActionPage,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }
        
        public string GetRightComponentHeader(int n = 1)
        {
            return SiteDriver.FindElement(String.Format(SuspectProvidersPageObjects.RightHeaderCssLocator, n), How.CssSelector).Text;
        }

        public bool IsProviderIconPresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.ProviderProfileCssLocator, How.CssSelector);
        }

        public bool IsProviderEyeIconPresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.ProviderEyeIconCssLocator, How.CssSelector);
        }        
        
        public ProviderProfilePage ClickOnProviderIconToOpenProviderProfile()
        {
            var providerProfile = Navigator.Navigate<ProviderProfilePageObjects>(() =>
            {
                SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderProfileCssLocator, How.CssSelector).Click();
                Console.WriteLine("Clicked on Provider Icon");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.ProviderProfilePage.GetStringValue()));
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
            });
            return new ProviderProfilePage(Navigator, providerProfile,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public void ClickProviderProfilePopUp()
        {
            JavaScriptExecutor.ExecuteClick(SuspectProvidersPageObjects.ProviderProfileCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
            //SiteDriver.FindElement(SuspectProvidersPageObjects.ProviderEyeIconCssLocator, How.CssSelector).Click();
        }

        public bool CheckProfileIconOpensPopUpAndClose()
        {
            ClickOnProviderIconToOpenProviderProfile();
            var popUpName = GetPopupPageTitle().Contains(PageTitleEnum.ProviderProfilePage.GetStringValue());
            CloseAnyTabIfExist();
            return popUpName;
        }


        public bool IsDollarIconPresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.DollarIconCssLocator, How.CssSelector);
        }

        public bool IsPersonIconPresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.PersonIconCssLocator, How.CssSelector);
        }

        public bool IsGraphIconPresent()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.GraphIconCssLocator, How.CssSelector);
        }

        public bool IsLabelPresent(string label)
        {
            return SiteDriver.IsElementPresent(string.Format(SuspectProvidersPageObjects.ExposureTitleLabelXPathTemplate, label), How.XPath);
        }

        public List<string> GetProviderExposureDetails(string prvseq)
        {
            var t =
                Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetProviderExposure,
                    prvseq));
            var providerExposureDetails = t.FirstOrDefault().ItemArray.Select(x => x.ToString()).ToList();
            return providerExposureDetails;
        }

        public List<string> GetProviderSpecialtyAverage(string specialty)
        {
            var t = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetProviderSpecialityAverages,
                specialty));
            var providerSpecialtyAverage = t.FirstOrDefault().ItemArray.Select(x => x.ToString()).ToList();
            return providerSpecialtyAverage;
        }

        public List<string> GetSuspectProviderAverageForEachScore(string prvSpecialty)
        {
            var t = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetSuspectProviderAverageForEachScore,prvSpecialty));
            var providerSpecialtyAverage = t.FirstOrDefault().ItemArray.Select(x => x.ToString()).ToList();
            return providerSpecialtyAverage;
        }

        public List<string> GetProviderExposureValueList()
        {
            return SiteDriver.FindDisplayedElementsText(SuspectProvidersPageObjects.ProviderExposureValueCssLocator,
                How.CssSelector);
        }

        public List<string> GetProviderExposureAverageList()
        {
            return SiteDriver.FindDisplayedElementsText(
                SuspectProvidersPageObjects.ProviderExposureAverageValueCssLocator, How.CssSelector);
        }

        public List<string> GetFluctuationValueList()
        {
            return SiteDriver.FindDisplayedElementsText(SuspectProvidersPageObjects.FluctuationPercentageCssLocator,
                How.CssSelector);
        }

        public string GetAverageValue(int average)
        {
            if (average > 1000)
            {
                return average.ToString().Insert(average.ToString().Length, "K");
            }
            else
            {
                return average.ToString();
            }
        }

        public bool IsProviderScoreDisplayedInSideBar()
        {
            return SiteDriver.IsElementPresent(SuspectProvidersPageObjects.SuspectProviderProviderScoreLabel, How.XPath);
        }

        public List<string> GetSuspectProviderScoresFromDB(string prvSeq, string client)
        {
            var t = Executor.GetCompleteTable(string.Format(ProviderSqlScriptObjects.GetSuspectProviderScoresSQL, prvSeq,client));
            var providerScoreList = t.FirstOrDefault().ItemArray.Select(x => x.ToString()).ToList();
            return providerScoreList;
        }

        /**
         * Returns Total score from the sidebar if 'scoreName' is not passed. Else, returns the score for the passed 'scoreName'. 
         */
        public string GetScoreFromSideBar(string scoreName =null)
        {
            if (scoreName != null)
                return JavaScriptExecutor
                    .FindElement(string.Format(SuspectProvidersPageObjects.SuspectProviderConditionScore, scoreName))
                    .Text;

            else
                return SiteDriver
                    .FindElement(SuspectProvidersPageObjects.SuspectProviderTotalScoreFromSideBar, How.CssSelector)
                    .Text;
        }

        public string GetAverageProviderScoreFromSidebar(string scoreName)
        {
            var t = JavaScriptExecutor.FindElement(
                string.Format(SuspectProvidersPageObjects.SuspectProviderAverageScoreFromSidebar, scoreName)).Text;
            return t;
        }

        public int CalculateFluctuationPercentage(float scoreFromDB, float averageScoreFromDB)
        {
            if (averageScoreFromDB == 0) return (int)(averageScoreFromDB * 100);
            float fluctuationPercentage = (((scoreFromDB - averageScoreFromDB) / averageScoreFromDB) * 100);

            return (int)(Math.Round(fluctuationPercentage));
            
        }

        public bool CheckIfFindButtonIsEnabled()
        {
            return SiteDriver.IsElementEnabled(SuspectProvidersPageObjects.FindButtonXPathTemplate, How.XPath);
        }

        public void ClickOnFindButton()
        {
            JavaScriptExecutor.ExecuteClick(SuspectProvidersPageObjects.FindButtonXPathTemplate,
                How.XPath);
            Console.WriteLine("Find Button Clicked");
            WaitForWorkingAjaxMessage();
        }
        public bool ClickFindAndCheckIfFindButtonIsDisabled()
        {
            //ClickOnFindButton();
            var isDisabled = JavaScriptExecutor.ClickAndGet(SuspectProvidersPageObjects.FindButtonCssLocator,
                                 SuspectProvidersPageObjects.DisabledFindButtonCssLocator) != null;
            return isDisabled;
        }

        public void WaitForGridToLoad()
        {
            SiteDriver.WaitForCondition(()=>GetGridViewSection.GetGridRowCount()>1);
        }

        public List<List<string>> GetExcelDataListForSuspectProviders()
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.GetExcelExportForSuspectProviders));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;

        }



        public List<List<string>> GetExcelDataListForSuspectProvidersForClientUser()
        {
            var newdataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(ProviderSqlScriptObjects.GetExcelExportForSuspectProvidersClient));
            newdataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return newdataList;

        }

        public List<string> GetProviderExportAuditListFromDB(string username)
        {
            return Executor.GetTableSingleColumn(Format(ProviderSqlScriptObjects.GetExcelDownloadAudit, username));
        }

        public bool IsExportIconPresent()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.ProviderSearchExportIconCssSelector, How.CssSelector);
        }



        public void ClickOnExportIcon()
        {
            JavaScriptExecutor.FindElement(ProviderSearchPageObjects.ProviderSearchExportIconCssSelector).Click();
            Console.WriteLine("Clicked on Export option");
            WaitForStaticTime(100);
        }

        public bool IsExportIconDisabled()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.DisabledExportIconCss, How.CssSelector);
        }

        public bool IsExportIconEnabled()
        {
            return SiteDriver.IsElementPresent(ProviderSearchPageObjects.EnabledExportIconCss, How.CssSelector);
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<SuspectProvidersPage>();
            return fileName;

        }

    }
}