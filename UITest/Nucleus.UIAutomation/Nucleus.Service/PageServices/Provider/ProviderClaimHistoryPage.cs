using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Provider;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Provider
{
    public class ProviderClaimHistoryPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private ProviderClaimHistoryPageObjects _providerClaimHistory;
        #endregion

        #region CONSTRUCTOR

        public ProviderClaimHistoryPage(INewNavigator navigator, ProviderClaimHistoryPageObjects providerClaimHistory, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager,
                IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, providerClaimHistory, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _providerClaimHistory = (ProviderClaimHistoryPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public string GetAltClaimNoOfClaimsEquence(string claimSeq)
        {
            Console.WriteLine("Receiving Alt Claim Number of Claim Sequence {0}", claimSeq);
            return SiteDriver.FindElement(string.Format(ProviderClaimHistoryPageObjects.AltClaimNoXPathTemplate, claimSeq), How.XPath).Text;
        }

        /// <summary>
        /// Click on Page Number
        /// </summary>
        /// <param name="pageNo"></param>
        public void ClickOnPageNo(string pageNo)
        {
            Console.WriteLine("Clicked on page number: {0}", pageNo);
            SiteDriver.FindElement(string.Format(ProviderClaimHistoryPageObjects.PageNumberXPathTemplate, pageNo), How.XPath).Click();
        }

        ///// <summary>
        ///// Close provider claim history
        ///// </summary>
        ///// <returns></returns>
        //public ProviderSearchPage CloseProviderClaimHistoryAndSwitchToProviderSearch()
        //{
        //    var providerSearch = Navigator.Navigate<ProviderSearchPageObjects>(()=>
        //                                    {
        //                                        SiteDriver.CloseWindow();
        //                                        SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ProviderSearch));
        //                                    });
        //    return new ProviderSearchPage(Navigator, providerSearch);
        //}
        
        public ProviderSearchPage CloseProviderClaimHistoryAndSwitchToProviderSearch()
        {
            var providerSearch = Navigator.Navigate<ProviderSearchPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ProviderSearch));
            });
            return new ProviderSearchPage(Navigator, providerSearch,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }
        
        public void ScrollToColumnNo(string columnNo)
        {
            JavaScriptExecutor.ExecuteToScrollToView(ProviderClaimHistoryPageObjects.GridHeaderId, columnNo);//24 dx code, 29 : allowed 42: last
        }

        public void InsertDxCodeInDxCodeRange(string dxCode)
        {
            SiteDriver.FindElement(ProviderClaimHistoryPageObjects.DxCodeRangeId, How.Id).SendKeys(dxCode);
            SiteDriver.FindElement(ProviderClaimHistoryPageObjects.SearchButtonId, How.Id).Click();
            Console.WriteLine("Inserted {0} DxCode in dxcode range.", dxCode);
            SiteDriver.WaitForCondition(() => !SiteDriver.FindElement(ProviderClaimHistoryPageObjects.LoadingImageId, How.Id).Displayed);
            SiteDriver.WaitForCondition(() => JavaScriptExecutor.Execute("return $.active;").ToString() == "0");
        }

        public string GetValueOfDxCode(string claimSeq, int dxCodeIndex)
        {
            dxCodeIndex = 18 + dxCodeIndex*2;
            var claimSeqRowId = SiteDriver.FindElement(
                string.Format(ProviderClaimHistoryPageObjects.ClaimSequenceXPathTemplate, claimSeq), How.XPath).
                GetAttribute("id");
            return
                SiteDriver.FindElement(
                    string.Format(ProviderClaimHistoryPageObjects.DxCodeXPathTemplate, claimSeqRowId, dxCodeIndex), How.XPath).Text + " " +
                SiteDriver.FindElement(
                    string.Format(ProviderClaimHistoryPageObjects.DxCodeVersionXPathTemplate, claimSeqRowId, dxCodeIndex + 1), How.XPath)
                    .Text;
        }

        public string GetProviderSequenceFromProviderClaimHistoryPage()
        {
            return SiteDriver.FindElement(ProviderClaimHistoryPageObjects.ProviderSequenceId, How.Id).Text
                .Substring(21); 
               
        }

        public void ClickOnAllButton(bool isSeleniumClick=true)
        {
            Console.WriteLine("Click on All Button.");
            SiteDriver.FindElement(ProviderClaimHistoryPageObjects.AllButtonId,How.Id).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver
                .FindElement(ProviderClaimHistoryPageObjects.LoadingImageId, How.Id).Displayed);
            SiteDriver.WaitForCondition(() =>JavaScriptExecutor
                                                  .Execute("return $.active;").ToString() == "0");
        }

        public bool IsTwevleMonthButtonSelected()
        {
            return SiteDriver.FindElement(ProviderClaimHistoryPageObjects.TwelveMonthButtonId, How.Id).Selected;
            
        }

        public void ClickonDownLoadButton()
        {
            SiteDriver.FindElement(ProviderClaimHistoryPageObjects.DownloadId, How.Id).Click();
        }

        public int GetTotalDataCount()
        {
            var paginationPage = new PaginationService(_providerClaimHistory,SiteDriver,JavaScriptExecutor);
            return paginationPage.TotalNoOfRecords;
        }


        public void HoverOverColumn(int row, string columnValue)
        {
            JavaScriptExecutor.ExecuteMouseOver(string.Format(ProviderClaimHistoryPageObjects.ResultGridColumnXpathTemplateByRowAndValue, row, columnValue), How.XPath);
            if (!IsToolTipPresentAndVisbile())
                JavaScriptExecutor.ExecuteMouseOver(string.Format(ProviderClaimHistoryPageObjects.ResultGridColumnXpathTemplateByRowAndValue, row, columnValue), How.XPath);
            SiteDriver.WaitToLoadNew(400);
        }

        public void HoverOutColumn(int row, string columnValue)
        {
            JavaScriptExecutor.ExecuteMouseOut(string.Format(ProviderClaimHistoryPageObjects.ResultGridColumnXpathTemplateByRowAndValue, row, columnValue), How.XPath);
            if (IsToolTipPresentAndVisbile())
                JavaScriptExecutor.ExecuteMouseOut(string.Format(ProviderClaimHistoryPageObjects.ResultGridColumnXpathTemplateByRowAndValue, row, columnValue), How.XPath);
        }
        public void ClickOutSideBoxToRemoveHover()
        {
            SiteDriver.MouseOver(ProviderClaimHistoryPageObjects.NavPopUpClass, How.ClassName);
        }

        public string GetTextValueinLiTag(int row)
        {
            return SiteDriver.FindElement(string.Format(ProviderClaimHistoryPageObjects.ToolTipContentTemplate, row), How.CssSelector).Text.Trim();
        }

        public bool IsToolTipPresentAndVisbile()
        {
            return SiteDriver.IsElementPresent(string.Format(ProviderClaimHistoryPageObjects.ToolTipContentTemplate, 1),
                How.CssSelector);
        }

        public int GetTwevleMonthsProviderHistory(string prvSeq)
        {
            return Convert.ToInt16(Executor.GetSingleValue(string.Format(
                ProviderSqlScriptObjects.GetProviderHistoryCount, prvSeq, DateTime.Now.AddMonths(-12).ToString("dd-MMM-yy"), DateTime.Now.ToString("dd-MMM-yy"))));
        }


        public string GetPageHeader()
        {
            return SiteDriver.FindElement(ProviderClaimHistoryPageObjects.PageHeaderCssTemplate, How.CssSelector).Text;
        }
        #endregion
    }
}
