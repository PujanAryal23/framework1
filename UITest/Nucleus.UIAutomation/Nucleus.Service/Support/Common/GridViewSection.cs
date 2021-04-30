using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public class GridViewSection
    {
        #region PRIVATE FIELDS
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;
        #region MyRegion

        public const string FilterOptionsIconCssLocator = @"li[title^=""Sort""]";
        public const string SideBarIconCssLocator = "span.sidebar_icon.toolbar_icon";
        public const string FilterOptionsListCssLocator = @"li[title^=""Sort""] >ul>li";
        public const string FilterOptionsSelectorByFilterNameCssLocator = @"li[title^=""Sort""] >ul>li span:contains({0})";

        #endregion

        #region Grid

        public const string GridRowByValueXPathTemplate = "//ul[contains(@class,'component_item_row ') and  li/span[text()='{0}']]";
        public const string tooltipInGridByRowColumnCssTemplate = "ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})";
        public const string ValueInGridByRowColumnCssTemplate = "ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})>span";
        public const string LabelInGridByRowColumnCssTemplate = "ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})>label";
        public const string ListValueInGridByColumnCssTemplate = "ul.component_item_list>.component_item>ul>li:nth-of-type({0}) span";
        public const string ListLabelInGridByColumnCssSelector = "ul.component_item_list>.component_item>ul>li:nth-of-type(2) label";
        public const string ClaimSequenceXPathTemplate = "//span[text()='{0}']";
        public const string AppealSequenceXPathTemplate = "//span[text()='{0}']";
        public const string GridSectionCssLocator = "section.component_content";
        public const string ListIconPresentInGridCssTemplate = "ul.component_item_list ul li{0}"; //should append text for user
        public const string NoDataMessageCssSelector = ".appeal_secondary_view section p.empty_message";
        public const string NoDataMessageLeftComponentCssSelector = ".search_list p.empty_message";
        public const string GridRowCssSelector = ".component_item_list  ul.component_item_row";
        public const string TestForLinkOrNotCssTemplate = "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type({1})[class*='action_link']";
        public const string GridRowByRowCssSelector = ".component_item_list div:nth-of-type({0}).component_item>ul";
        public const string GridRowByRowInClaimSearchCssSelector = "li:nth-of-type({0}).component_item>ul";
        public const string GridRowInClaimSearchCssSelector = "li:nth-of-type({0}).component_item";
        public const string NoDataMessageCssSelectorGeneral = ".component_content p.empty_message";
        public const string EditIconCssLocator =
            "section.search_list > section.component_content  span.small_edit_icon";

        public const string EnabledEditIconCssLocator =
            "section.search_list > section.component_content  span.small_edit_icon:not([class*='is_disabled'])";
        public const string DeleteIconCssLocator =
            "section.search_list> section.component_content  span.small_delete_icon";

        public const string DeleteIconBySpanValueXPathTemplate =
            "//div[*//span[text()='{0}']][contains(@class,'component_item')]//span[contains(@class,'small_delete_icon')]";
        public const string EditRecordIconCssTemplate =
            "section.search_list> section.component_content  .component_item:nth-of-type({0}) ul.component_item_row  span.small_edit_icon";

        public const string EditDeleteRecordIconXPathTemplate =
            "//li/label[text()='{0}']/../../li/ul/li[@title='{1}']/span";
        public const string DeleteRecordIconCssTemplate =
            "section.search_list> section.component_content  .component_item:nth-of-type({0}) ul.component_item_row  span.small_delete_icon";
        public const string RecordRowCssSelector = "section.search_list > section.component_content >ul .component_item";
        public const string ValueInGridByLabelXpathTemplate =
            "(//label[text()= '{0}']/../span)[{1}]";
        public const string GridRowwCssSelector = ".component_item_list div:nth-of-type({0}).component_item";
        public const string GridRowProvideronReviewCssSelector =
            "div:nth-of-type({0}).component_item>ul>li>ul>li.eyeball";
        public const string LoadMoreCssSelector = "div.load_more_data span";
        public const string AppealCountXPathTemplate = "//li[contains(@title,'This claim has')]/span";

        public const string ValueInGridByValueandLabelXPathTemplate =
            "//li/label[text()='{0}']/../../li/label[text()='{1}']/following-sibling::span";

        public const string ValueInGridXPathTemplate = "//li/label[text()='{0}']";
        public const string lockIconCssLocator = "li.lock";

        #endregion

        #endregion

        #region Constructor

        public GridViewSection(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region  PUBLIC METHOD

        public bool IsLockIconPresentInGrid()
        {
            return SiteDriver.IsElementPresent(lockIconCssLocator, How.CssSelector);
        }

        public string GetLockToolTipText()
        {
            return SiteDriver.FindElementAndGetAttribute(lockIconCssLocator, How.CssSelector, "title");
        }

        public void ClickEditDeleteIconByValueAndIcon(string value, string icon)
        {
            SiteDriver
                .FindElement(Format(EditDeleteRecordIconXPathTemplate, value, icon), How.XPath).Click();
            WaitForWorkingAjaxMessage();

        }

        public int GetTotalInLoadMore()
        {
            var loadMoreValue = SiteDriver.FindElement(LoadMoreCssSelector, How.CssSelector).Text;
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                .Select(m => int.Parse(m.Trim())).ToList();
            return numbers[1];
        }

        public string GetLoadMoreText()
        {
            return SiteDriver.FindElement(LoadMoreCssSelector, How.CssSelector).Text;
        }

        public void ClickLoadMoreIterativelyToShowAllRecords()
        {
            var numbers = GetTotalInLoadMore();
            var count = numbers % 25 == 0 ? numbers / 25 - 1 : numbers / 25;
            for (var i = 0; i < count; i++)
            {
                ClickLoadMore();
            }
        }

        public bool IsGridViewSectionPresent()
        {
            return SiteDriver.IsElementPresent(GridSectionCssLocator, How.CssSelector);
        }

        public int GetGridRowCount()
        {
            return SiteDriver.FindElementsCount(GridRowCssSelector, How.CssSelector);
        }

        public List<string> GetGridAllRowData()
        {
            return JavaScriptExecutor.FindElements(GridRowCssSelector, How.CssSelector,"Text");
        }
        public int GetRecordRowsCountFromPage()
        {
            return SiteDriver.FindElementsCount(RecordRowCssSelector, How.CssSelector);
        }
        
        public bool IsEnabledEditIconPresent()
        {
            return SiteDriver.IsElementPresent(EnabledEditIconCssLocator, How.CssSelector);
        }

        public int GetEnabledEditIconListCount()
        {
            return SiteDriver.FindElementsCount(EnabledEditIconCssLocator, How.CssSelector);
        }


        public bool IsPencilIconPresentInEachRecord()
        {
            return SiteDriver.FindElementsCount(EditIconCssLocator, How.CssSelector)
                .Equals(GetRecordRowsCountFromPage());

        }
        public bool IsDeleteIconPresentInEachRecord()
        {
            return SiteDriver.FindElementsCount(DeleteIconCssLocator, How.CssSelector)
                .Equals(GetRecordRowsCountFromPage());

        }
        public bool IsPencilIconPresentInRecordForRow(int rowId = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(EditRecordIconCssTemplate, rowId),
                How.CssSelector);
        }

        public bool IsDeleteIconPresentInRecordForRow(int rowId = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(DeleteRecordIconCssTemplate, rowId),
                How.CssSelector);
        }

        public string GetValueInGridByColRow(int col = 2, int row = 1) //2:client type; 3: claim seq
        {
            var element = SiteDriver.FindElement(
                string.Format(ValueInGridByRowColumnCssTemplate, row, col), How.CssSelector);
            return element.Text;
        }

        public string GetToolTipInGridByColRow(int col = 2, int row = 1) //2:client type; 3: claim seq
        {
            var t= SiteDriver.FindElement(
                string.Format(tooltipInGridByRowColumnCssTemplate, row, col), How.CssSelector).GetAttribute("title");
            return t;
        }

        public string GetColorInGridByColRow(int col = 2, int row = 1) //2:client type; 3: claim seq
        {
            var t = SiteDriver.FindElement(
                    string.Format(tooltipInGridByRowColumnCssTemplate, row, col), How.CssSelector)
                .GetCssValue("background");
            return t;
        }

        public string GetValueInGridBylabelAndRow(string label, int row = 1) //2:flag; 3:source
        {
            var t = SiteDriver.FindElement(
                string.Format(ValueInGridByLabelXpathTemplate, label, row), How.XPath).Text;
            return SiteDriver.FindElement(
                string.Format(ValueInGridByLabelXpathTemplate, label, row), How.XPath).Text;

        }
        public string GetLabelInGridByColRow(int col = 2, int row = 1) //2:client type; 3: claim seq
        {
            return SiteDriver.FindElement(
                string.Format(LabelInGridByRowColumnCssTemplate, row, col), How.CssSelector).Text;
        }
     

        public bool IsLabelPresentByCol(int col = 2)
        {
            return SiteDriver.IsElementPresent(
                string.Format(LabelInGridByRowColumnCssTemplate, 1, col), How.CssSelector);
        }

        public List<string> GetGridLabeList() =>
            JavaScriptExecutor.FindElements(ListLabelInGridByColumnCssSelector, How.CssSelector,"Text");
        public List<string> GetGridListValueByCol(int col = 2)
        {
           
            return JavaScriptExecutor.FindElements(string.Format(ListValueInGridByColumnCssTemplate, col), "Text");
        }

        public List<string> GetGridListValueByColAndSort(int col = 2)
        {
            var list= JavaScriptExecutor.FindElements(string.Format(ListValueInGridByColumnCssTemplate, col), "Text");
            list.Sort();
            return list;
        }
        public bool DoesGridListValueByColHasValue(string value,int col = 2)
        {
            var list = JavaScriptExecutor.FindElements(string.Format(ListValueInGridByColumnCssTemplate, col), "Text");
            return list.All(x => x == value);
        }
        public bool IsNoDataMessagePresent()
        {
            return SiteDriver.IsElementPresent(NoDataMessageCssSelectorGeneral, How.CssSelector);
        }
        public bool IsNoDataMessagePresentInLeftSection()
        {
            return SiteDriver.IsElementPresent(NoDataMessageLeftComponentCssSelector, How.CssSelector);
        }
        public string GetNoDataMessageText()
        {
            return SiteDriver.FindElement(NoDataMessageLeftComponentCssSelector, How.CssSelector).Text;
        }

        public bool IsListIconPresentInGridForClassName(string className)
        {
            return SiteDriver.IsElementPresent(
                string.Format(ListIconPresentInGridCssTemplate, "."+className), How.CssSelector);
        }
        public string GetTitleOfListIconPresentInGridForClassName(string className)
        {
            return SiteDriver.FindElement(
                string.Format(ListIconPresentInGridCssTemplate, "." + className), How.CssSelector).GetAttribute("title");
        }

        public bool IsVerticalScrollBarPresentInGridSection()
        {
            var scrollHeight = GetScrollHeight(GridSectionCssLocator);
            var clientHeight = GetClientHeight(GridSectionCssLocator);
            Console.WriteLine("Scroll Height: " + scrollHeight);
            Console.WriteLine("Client Height:" +clientHeight);
            return scrollHeight >clientHeight;
        }

        public int GetScrollHeight(string select)
        {
            return JavaScriptExecutor.ScrollHeight(select);
        }

        public int GetClientHeight(string select)
        {
            return JavaScriptExecutor.ClientHeight(select);
        }

        public void ClickOnGridByRowCol(int row=1, int col=3)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ValueInGridByRowColumnCssTemplate, row,col), How.CssSelector);
            Console.WriteLine("Clicked on Grid row : {0}", row);
            WaitForWorkingAjaxMessage();           
        }


        public void ClickOnGridRowByRow(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(GridRowByRowCssSelector, row), How.CssSelector);
            Console.WriteLine("Clicked on Grid row : {0}", row);
            WaitForWorkingAjaxMessage();            
        }

        public void ClickOnGridRowByRowInClaimSearch(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(GridRowByRowInClaimSearchCssSelector, row), How.CssSelector);
            Console.WriteLine("Clicked on Grid row : {0}", row);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnGridRowByValue(string value)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(GridRowByValueXPathTemplate, value), How.XPath);
            Console.WriteLine("Clicked on Grid having value: {0}", value);
            WaitForWorkingAjaxMessage();           
        }

        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitToLoadNew(500);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(FilterOptionsIconCssLocator, How.CssSelector);
        }

        public bool IsFilterOptionIconPresent()
        {
            return SiteDriver.IsElementPresent(FilterOptionsIconCssLocator, How.CssSelector);
        }

        public bool IsSideBarIconPresent()
        {
            return SiteDriver.IsElementPresent(SideBarIconCssLocator, How.CssSelector);
        }

        public string GetFilterOptionTooltip()
        {
            return SiteDriver.FindElement(FilterOptionsIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            var list = JavaScriptExecutor.FindElements(FilterOptionsListCssLocator, How.CssSelector, "Text");
            ClickOnFilterOption();
            return list;
        }

        public void ClickOnFilterOptionListByFilterName(string filterName)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ClickJQuery(string.Format(FilterOptionsSelectorByFilterNameCssLocator ,filterName));
            Console.WriteLine("Click on {0} filter option", filterName);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }




        public bool IsClickableLink(int col, int row = 1)
        {
            return SiteDriver.IsElementPresent(String.Format(TestForLinkOrNotCssTemplate, row, col), How.CssSelector);
        }
        #endregion

        public bool IsRowHighlighted(int row=1)
        {
            return SiteDriver.FindElement(string.Format(GridRowwCssSelector, row), How.CssSelector).
            GetAttribute("class").Contains("is_active");
        }

        public bool IsListInRequiredOrder(int col,bool descending=false)
        {
            var list =GetGridListValueByCol(col);
            list.RemoveAll(string.IsNullOrEmpty);
            if (descending)
                return list.IsInDescendingOrder();
            return list.IsInAscendingOrder();
        }

        public void ClickOnGridRowWithReviewProvider()
        {
            int rowCount = GetGridRowCount();
            int row = 1;
            while (!SiteDriver.IsElementPresent(string.Format(GridRowProvideronReviewCssSelector, row), How.CssSelector))
            {
                row++;
            }
            if (SiteDriver.IsElementPresent(string.Format(GridRowProvideronReviewCssSelector, row), How.CssSelector))
            {
                JavaScriptExecutor.ExecuteClick(string.Format(GridRowByRowCssSelector, row), How.CssSelector);
            }
        }

        public void ClickOnDeleteIcon(int row = 1)
        {
            SiteDriver.FindElement(string.Format(DeleteRecordIconCssTemplate, row),
                How.CssSelector).Click();
        }
        public void ClickOnDeleteIconByRowValue(string value )
        {
            SiteDriver.FindElement(string.Format(DeleteIconBySpanValueXPathTemplate, value),
                How.XPath).Click();
        }
        public void ClickOnEditIcon(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(EditRecordIconCssTemplate, row), How.CssSelector);
        }

        public bool IsLoadMorePresent()
        {
            return SiteDriver.IsElementPresent(LoadMoreCssSelector, How.CssSelector);
        }

        public void ClickLoadMore()
        {
            SiteDriver.FindElement(LoadMoreCssSelector, How.CssSelector).Click();
        }

        public List<string> GetAppealCountList()
        {
            return JavaScriptExecutor.FindElements(AppealCountXPathTemplate, How.XPath, "Text");
        }

        public bool IsGridRowSelected(int row = 1)
        {
            return SiteDriver.FindElement(Format(GridRowInClaimSearchCssSelector, row), How.CssSelector)
                .GetAttribute("class").Contains("is_active");
        }

        public int GetGridRowNumberByColAndLabel(string label , int col = 2 )
        {
            var listOfColValues = GetGridListValueByCol(col);
            var indexOfLabel = listOfColValues.IndexOf(label);
            return indexOfLabel + 1;
        }

        public string GetValueInGridByValueAndLabel(string value, string label) => SiteDriver
            .FindElement(Format(ValueInGridByValueandLabelXPathTemplate, value, label), How.XPath).Text;

        public bool IsValuePresent(string value) => SiteDriver
            .IsElementPresent(Format(ValueInGridXPathTemplate, value), How.XPath);
    }

}
