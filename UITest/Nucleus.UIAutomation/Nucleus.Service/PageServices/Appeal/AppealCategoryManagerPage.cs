using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using Microsoft.SqlServer.Server;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Data.AutomationMapping;
using UIAutomation.Framework.Database;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealCategoryManagerPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private readonly AppealCategoryManagerPageObjects _appealCategoryManagerPage;

        private SearchFilter _searchFilter;
        private readonly QuickSearch _quickSearch;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;

        #endregion

        #region CONSTRUCTOR

        public AppealCategoryManagerPage(INewNavigator navigator, AppealCategoryManagerPageObjects appealCategoryManager, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealCategoryManager, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealCategoryManagerPage = (AppealCategoryManagerPageObjects) PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _commonSqlObjects = new CommonSQLObjects(executor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);

        }

        #endregion

        #region PUBLIC METHODS

        public string GetAssignedAnalyst(string label)
        {
            return SiteDriver
                .FindElement(
                    String.Format(AppealCategoryManagerPageObjects.AppealCategoryAssignedAnalystXPath, label),
                    How.XPath).Text;
        }

        public string GetNonRestrictedAssignedAnalyst()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.AppealCategoryAssinedNResAnalystCssTemplate,
                    How.CssSelector).Text;
        }

        public string GetRestrictedAssignedAnalyst()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.AppealCategoryAssinedResAnalystCssTemplate,
                    How.CssSelector).Text;
        }

        public List<string> GetRestrictedAssignedAnalystList()
        {
            return JavaScriptExecutor
                .FindElements(AppealCategoryManagerPageObjects.AppealCategoryAssinedResAnalystXpathTemplate, How.XPath,
                    "Text");
        }

        public List<string> GetNonRestrictedAssignedAnalystList()
        {
            return JavaScriptExecutor
                .FindElements(AppealCategoryManagerPageObjects.AppealCategoryAssinedNResAnalystXpathTemplate, How.XPath,
                    "Text");
        }

        public List<string> GetLatestAppealCategoryAuditHistoryFromDatabase(string category)
        {
            List<string> list = new List<string>();
            var data = Executor.GetCompleteTable(
                String.Format(AppealSqlScriptObjects.GetLatestAuditDataInAppealCategoryManager, category));
            list = (data.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList())[0];
            list.Add(Executor.GetSingleStringValue(
                String.Format(AppealSqlScriptObjects.GetLatestNonRestricetedAnalystAuditData, category)));
            list.Add(Executor.GetSingleStringValue(
                String.Format(AppealSqlScriptObjects.GetLatestRestrictedAnalystAuditData, category)));
            return list;
        }

        public List<string> GetLatestAuditHistoryInAppealCategoryManager()
        {
            var list = JavaScriptExecutor
                .FindElements(AppealCategoryManagerPageObjects.LatestAuditDataInAppealCategoryAuditHistory,
                    How.CssSelector, "Text");
            list.RemoveAt(0);
            return list;
        }

        public bool IsAnalystPresentInNonRestrictedAssignedAnalystList(string analystNameId)
        {
            return SiteDriver
                .IsElementPresent(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryAssignedNResAnalystByNameXpathTemplate,
                        analystNameId),
                    How.XPath);
        }

        public bool IsAnalystPresentInRestrictedAssignedAnalystList(string analystNameId)
        {
            return SiteDriver.IsElementPresent(
                string.Format(AppealCategoryManagerPageObjects.AppealCategoryAssinedResAnalystByNameXpathTemplate,
                    analystNameId),
                How.XPath);
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }


        public bool IsEditIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.EditIconEnabledCssLocator,
                How.CssSelector);
        }

        public bool IsEditIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.EditIconDisabledCssLocator,
                How.CssSelector);
        }

        public bool IsDeleteIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.DeleteIconEnabledCssLocator,
                How.CssSelector);
        }

        public bool IsDeleteIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.DeleteIconDisabledCssLocator,
                How.CssSelector);
        }

        public string GetCategoryCode(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return
                    JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.CategoryLabelExcludingDentalJqueryTemplate).Text;

            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.CategoryLabelCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetClientLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ClientLabelCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetClientValue(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return
                    JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.ClientValueExcludingDentalJqueryTemplate).Text;
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ClientValueCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetFlagLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.FlagLabelCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetOrderLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.OrderLabelCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetOrderValue(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return
                    JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.OrderValueExcludingDentalJqueryTemplate).Text;
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.OrderValueCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetOrderValue(string product, string procCode, string trigCode, string client = "SMTST")
        {
            return SiteDriver.FindElement(string.Format(
                AppealCategoryManagerPageObjects.OrderVaueByClientProductProcCodeTrigCodeXPathTemplate, client,
                product, procCode, trigCode), How.XPath).Text;
        }

        public string GetProductLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ProductLabelCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetProductValue(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return
                    JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.ProductValueExcludingDentalJqueryTemplate).Text;
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ProductValueCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetProcLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ProcLabelCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetProcValue(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return GetFirstProcValueExcludingDci();
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.ProcValueCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetFirstProcValueExcludingDci()
        {
            return JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.ProcValueExcludingDciJqueryTemplate)
                .Text;
        }

        public string GetTrigLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.TrigLabelCssLocator, row),
                How.CssSelector).Text;
        }

        public string GetTrigValue(int row = 1, bool excludeDci = false)
        {
            if (excludeDci)
                return GetFirstTrigValueExcludingDci();
            return SiteDriver.FindElement(String.Format(AppealCategoryManagerPageObjects.TrigValueCssTemplate, row),
                How.CssSelector).Text;
        }

        public string GetFirstTrigValueExcludingDci()
        {
            return JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.TrigValueExcludingDciJqueryTemplate)
                .Text;
        }

        public int GetAppealCategoryRowCount()
        {
            return SiteDriver.FindElementsCount(AppealCategoryManagerPageObjects.AppealCategoryRowCssLocator,
                How.CssSelector);
        }

        public string GetAppealCategoryOrderOfLastRowWithOutFlag()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.AppealCategoryOrderOfLastRowWithOutFlagXPath, How.XPath)
                .Text;
        }

        public string GetAppealCategoryLastOrderNumber()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.AppealCategoryLastOrderNumberCssSelector,
                    How.CssSelector).Text;
            //return Executor.GetSingleStringValue(AppealSqlScriptObjects.GetMaxCategoryOrderFromDb);
        }

        public int GetAppealCategoryRowCountByProduct(string product)
        {
            return JavaScriptExecutor.FindElementsCount(
                string.Format(AppealCategoryManagerPageObjects.GetAppealCategoryRowCountByProductCSSLocator, product));
        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
            WaitForWorkingAjaxMessage();
        }

        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }

        public void ClickOnAppealCategoryAnalystDetailsIcon()
        {
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AppealCategoryAnalystDetailsIconCssSelector, How.CssSelector);
        }

        public void ClickAppealCategoryAuditHistoryIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AppealCategoryAuditHistoryIconCssSelector,
                How.CssSelector);
            WaitForWorking();
            WaitForStaticTime(1000);
        }

        public bool IsAppealCategoryAuditHistoryIconPresent()
        {
            return SiteDriver.IsElementPresent(
                AppealCategoryManagerPageObjects.AppealCategoryAuditHistoryIconCssSelector, How.CssSelector);
        }

        public int GetAuditRowCount()
        {
            return SiteDriver.FindElementsCount(AppealCategoryManagerPageObjects.AuditRowCssLocator, How.CssSelector);
        }

        public bool IsAuditTrailContainerPresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AuditTrailContainerCssLocator,
                How.CssSelector);
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.WorkingAjaxMessageCssLocator,
                How.CssSelector);
        }

        public string GetLastModifiedDateAuditSection(int row)
        {
            return
                SiteDriver.FindElement(
                        String.Format(AppealCategoryManagerPageObjects.LastModifiedDateCssTemplate, row),
                        How.CssSelector)
                    .Text;
        }

        public bool IsCategoryIdPresentAuditSection()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.CategoryIDAuditSectionCssLocator,
                How.CssSelector);
        }

        public string GetCategoryIdAuditSection()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.CategoryIDAuditSectionCssLocator,
                How.CssSelector).Text;
        }

        public bool IsLastModifiedUserPresentAuditSection()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.LastModifiedUserAuditSectionCssLocator,
                How.CssSelector);
        }

        public string GetProcCodeAuditSection()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeAuditSectionCssLocator,
                How.CssSelector).Text;
        }

        public string GetTrigProcAuditSection()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcAuditSectionCssLocator,
                How.CssSelector).Text;
        }

        public string GetProductValueAuditSection()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProductValueAuditSectionCssLocator,
                How.CssSelector).Text;
        }

        public string GetNotesValueAuditSection()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.NotesValueAuditSectionCssLocator,
                How.CssSelector).Text;
        }

        public string GetAnalaystInAuditSection(bool restrictedAnalyst = false)
        {
            if (!restrictedAnalyst)
                return SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnAuditSectionCssLocator,
                    How.CssSelector).Text;
            else
                return SiteDriver
                    .FindElement(AppealCategoryManagerPageObjects.AnalystRestrictedOnAuditSectionCssLocator,
                        How.CssSelector).Text;
        }

        public string GetAnalaystToolTipInAuditSection(bool restrictedAnalyst = false)
        {
            if (!restrictedAnalyst)
                return SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnAuditSectionCssLocator,
                    How.CssSelector).GetAttribute("title");
            else
                return SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.AnalystRestrictedOnAuditSectionCssLocator,
                    How.CssSelector).GetAttribute("title");
        }

        public bool IsEllipsisPresentInAuditSection()
        {
            var element = SiteDriver.FindElement(AppealCategoryManagerPageObjects.AuditRowEachValueCssSelector,
                How.CssSelector);
            return element.GetCssValue("text-overflow").Equals("ellipsis") &&
                   element.GetCssValue("overflow").Equals("hidden");
        }

        public string GetUserIdAuditSection()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.UserIDAuditSectionCssLocator, How.CssSelector).Text;
        }

        public string GetLastModifiedUserAuditSection(int row = 1)
        {
            return
                SiteDriver.FindElement(
                        String.Format(AppealCategoryManagerPageObjects.LastModifiedUserAuditSectionCssLocator, row),
                        How.CssSelector)
                    .Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isProductSpecified">Should be true if specific product needs to be selected after clicking Add icon. Selects CV by default.</param>
        /// <param name="product">Value of the product name to be selected after clicking Add icon</param>
        /// <param name="clickOnly">Just perform Click action without selecting any product</param>
        public void ClickAddNewCategory(bool isProductSpecified = false, string product = null, bool clickOnly = false)
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AddCategoryCodeCssLocator,
                How.CssSelector);
            if (!clickOnly)
            {
                if (!isProductSpecified)
                {
                    SelectProductOnAddSection(ProductEnum.CV.GetStringValue());
                    Console.WriteLine("Click on Add Category Code Icon and select '{0}' as the product",
                        ProductEnum.CV.GetStringValue());
                }
                else if (isProductSpecified && product != null)
                {
                    SelectProductOnAddSection(product);
                    Console.WriteLine("Click on Add Category Code Icon and select '{0}' as the product", product);
                }
            }
        }

        public bool IsSearchIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.SearchCategoryCodeCssLocator,
                How.CssSelector);
        }

        public bool IsFindCategoryCodeSectionPresent()
        {

            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.FindCategoryCodesSectionCssLocator,
                How.CssSelector);
        }

        public void ClickOnSearchIcon()
        {
            SiteDriver.WaitForCondition(IsSearchIconPresent);
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.SearchCategoryCodeCssLocator,
                How.CssSelector);
            Console.WriteLine("Clicked on Search Category Code Icon");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(500);

        }

        public void SelectCategoryIdOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 1),
                How.CssSelector).SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListValueFindSectionXPathTemplate, 1, value),
                How.XPath);
            Console.WriteLine("Category ID Selected: <{0}>", value);
        }

        public void SelectProductOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 3),
                How.CssSelector).SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListValueFindSectionXPathTemplate, 3, value),
                How.XPath);
            Console.WriteLine("Product Selected: <{0}>", value);
        }

        public void SelectCategoryOrderOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4),
                How.CssSelector).SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListValueFindSectionXPathTemplate, 4, value),
                How.XPath);
            Console.WriteLine("Category Order Selected: <{0}>", value);
        }

        public string GetCategoryOrderOnFindSection()
        {
            return SiteDriver
                .FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4),
                    How.CssSelector).GetAttribute("value");
        }

        public void SelectAnalystOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 5),
                How.CssSelector).SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListValueFindSectionXPathTemplate, 5, value),
                How.XPath);
            Console.WriteLine("Analyst Selected: <{0}>", value);
        }

        public void SetProcCodeFromOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 6),
                How.CssSelector).SendKeys(value);
            Console.WriteLine("Proc Code From Entered: <{0}>", value);
        }

        public void SetProcCodeToOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 7),
                How.CssSelector).SendKeys(value);
            Console.WriteLine("Proc Code To Entered: <{0}>", value);
        }

        public void SetTrigProcFromOnFindSection(string value)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 8),
                How.CssSelector).SendKeys(value);
            Console.WriteLine("Trig Proc From Entered: <{0}>", value);
        }


        public void SetTrigProcToOnFindSection(string value)
        {
            SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 9),
                    How.CssSelector)
                .ClearElementField();
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 9),
                How.CssSelector).SendKeys(value);
            Console.WriteLine("Trig Proc To Entered: <{0}>", value);
        }

        public void ClickOnFindButton()
        {
            SiteDriver.WaitToLoadNew(500);
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.FindButtonCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Find Button");
            WaitForWorking();
            SiteDriver.WaitToLoadNew(500);

        }

        public void ClickOnClearButton()
        {

            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.ClearButtonCssLocator, How.CssSelector);
            Console.WriteLine("Click on Clear Button");
            WaitForWorking();

        }

        //public bool IsPageErrorPopupModalPresent()
        //{
        //    return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.PageErrorPopupModelId, How.Id);
        //}

        //public void ClosePageError()
        //{
        //    _appealCategoryManagerPage.PageErrorCloseButton.Click();
        //    SiteDriver.WaitForCondition(() => !IsPageErrorPopupModalPresent());
        //    Console.WriteLine("Closed the modal popup");

        //}

        public string GetPageErrorMessage()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.PageErrorMessageId, How.Id).Text;
        }

        public bool IsSearchFieldFindSectionEmptyorNull(int row)
        {
            if (string.IsNullOrEmpty(SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                    How.CssSelector)
                .Text))
                return true;
            return false;
        }


        public bool IsCategoryIdSearchFieldValueSorted(int row)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            var categoryIdList = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, row),
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteMouseOut(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            return categoryIdList.IsInAscendingOrder();
        }

        public bool IsProductSearchFieldValueSorted(int row)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            var productList = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, row),
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteMouseOut(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            return productList.IsInAscendingOrder();
        }

        public bool IsAnalystSearchFieldValueSorted(int row)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            var analystList = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, row),
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteMouseOut(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            analystList.RemoveAt(0);
            return IsAnalystInAscendingOrder(analystList);
        }

        public bool DoesAnalystSearchListContainAnalyst(int row, string analyst)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            var analystList = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, row),
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteMouseOut(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, row),
                How.CssSelector);
            analystList.RemoveAt(0);
            return analystList.Contains(analyst);
        }



        public bool IsCategoryOrderValueSearchFieldSorted()
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);
            List<String> categoryListValue = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, 4),
                How.CssSelector, "Text");
            categoryListValue.RemoveAt(0);
            List<int> searchIntListValue = categoryListValue.Select(int.Parse).ToList();
            return searchIntListValue.IsInAscendingOrder();
        }

        public string GetEmptySearchMessage()
        {
            return SiteDriver
                .FindElement(AppealCategoryManagerPageObjects.EmptySearchMessageCssLocator, How.CssSelector).Text;
        }

        public bool IsClickOnFilterOption()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.FilterIconCssLocator, How.CssSelector);
        }

        public void ClickOnFilterOption()
        {
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.FilterIconCssLocator, How.CssSelector).Click();
        }

        public List<string> GetFilterOptionList()
        {
            ClickOnFilterOption();
            List<String> filterOptionList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.FilterOptionListCssLocator,
                    How.CssSelector, "Text");
            return filterOptionList;

        }

        public bool IsFilterOptionPresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.FilterOptionSectionCssLocator,
                How.CssSelector);
        }

        public void ClickOnFilterOptionList(int row)
        {

            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.FilterOptionCssTemplate, row), How.CssSelector);
            SiteDriver.WaitForPageToLoad();
            JavaScriptExecutor.ExecuteMouseOut(
                string.Format(AppealCategoryManagerPageObjects.FilterOptionCssTemplate, row), How.CssSelector);
        }

        public bool IsCategoryValueSorted()
        {
            List<String> categoryValueList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.CategoryLabelCssLocator,
                    How.CssSelector, "Text");
            return categoryValueList.IsInAscendingOrder();

        }

        public bool IsCategoryOrderValueSorted()
        {
            List<String> categoryOrderList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.OrderValueCssLocator, How.CssSelector,
                    "Text");
            categoryOrderList.RemoveAll(item => item.Contains("NA"));
            List<int> categoryOrderIntList = categoryOrderList.Select(s => int.Parse(s)).ToList();
            return categoryOrderIntList.IsInAscendingOrder();

        }

        public bool IsProductValueSorted()
        {
            List<String> productList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.ProductValueCssLocator,
                    How.CssSelector, "Text");
            return productList.IsInAscendingOrder();

        }

        public bool IsProcCodeFromValueSorted()
        {
            List<String> procCodeList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.ProcValueCssLocator, How.CssSelector,
                    "Text");
            procCodeList.RemoveAll(item => item.Contains("NA"));
            procCodeList.RemoveAll(item => item.Contains(""));
            var procCodeFromList = procCodeList.Select(s => s.Split('-')[0]).ToList();
            return procCodeFromList.IsInAscendingOrder();

        }

        public bool IsProcCodeToValueSorted()
        {
            List<String> procCodeList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.ProcValueCssLocator, How.CssSelector,
                    "Text");
            procCodeList.RemoveAll(item => item.Contains("NA"));
            procCodeList.RemoveAll(item => item.Contains(""));
            var procCodeToList = procCodeList.Select(s => s.Split('-')[1]).ToList();
            return procCodeToList.IsInAscendingOrder();

        }

        public bool IsTrigProcFromValueSorted()
        {
            List<String> trigProcList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.TrigValueCssLocator, How.CssSelector,
                    "Text");
            trigProcList.RemoveAll(item => item.Contains("NA"));
            trigProcList.RemoveAll(item => item.Contains(""));
            var trigProcFromList = trigProcList.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Split('-')[0])
                .ToList();
            return trigProcFromList.IsInAscendingOrder();

        }

        public bool IsTrigProcToValueSorted()
        {
            List<String> trigProcList =
                JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.TrigValueCssLocator, How.CssSelector,
                    "Text");
            trigProcList.RemoveAll(item => item.Contains("NA"));
            trigProcList.RemoveAll(item => item.Contains(""));
            var trigProcToList = trigProcList.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Split('-')[1])
                .ToList();
            return trigProcToList.IsInAscendingOrder();

        }



        public bool IsAppealCategoryAssignmentSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.AppealCategoryManager), How.XPath);
        }

        public void ClickOnAppealCategoryRow(int row = 1)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowCssTemplate, row), How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on Appeal Category Row {0}", row);
        }

        public void ClickOnDentalAppealCategoryRowByClient(string client = "SMTST")
        {
            JavaScriptExecutor.ClickJQuery(
                string.Format(AppealCategoryManagerPageObjects.DentalAppealCategoryRowJQueryTemplate, client));
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on Dental Appeal Category for client=<{0}>", client);
        }

        public void ClickOnFirstAppealCategoryRowExcludingDci()
        {
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AppealCategoryRowExcludingDciJqueryTemplate, How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on an Appeal Category Row");
        }

        public void ClickOnAppealCategoryCatId(string catId)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.AppealCategoryCatIdCssLocator, catId), How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on Appeal Category Row {0}", catId);
        }

        public void ClickOnAppealCategoryRowAndGoToAuditHistory(int row = 1, bool excludeDci = false)
        {
            if (excludeDci) ClickOnFirstAppealCategoryRowExcludingDci();
            else ClickOnAppealCategoryRow(row);
            ClickAppealCategoryAuditHistoryIcon();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on Audit History Icon");
        }

        public void ClickOnAppealCategoryRowByCatIdAndGoToAuditHistory(string catId)
        {
            ClickOnAppealCategoryCatId(catId);
            ClickAppealCategoryAuditHistoryIcon();
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            Console.WriteLine("Click on Audit History Icon");
        }

        public string GetPageInsideTitle()
        {
            return
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.PageInsideTitleCssLocator,
                    How.CssSelector).Text;
        }

        public bool IsLastModifiedDateSorted()
        {
            ClickOnAppealCategoryRowAndGoToAuditHistory();
            var date1 = Convert.ToDateTime(GetLastModifiedDateAuditSection(1));
            for (int i = 2; i < 4; i++)
            {
                ClickOnAppealCategoryRowAndGoToAuditHistory(i);

                var date2 = Convert.ToDateTime(GetLastModifiedDateAuditSection(1));
                if (date1 < date2)
                    return false;

            }

            return true;
        }

        public bool IsEditIconDisplayedInEachRow()
        {
            List<bool> editList = SiteDriver.FindElementAndGetActiveAttribute(
                AppealCategoryManagerPageObjects.EditIconEnabledCssLocator,
                How.CssSelector);
            return editList.TrueForAll(c => c);
        }

        public bool IsDeleteIconDisplayedInEachRow()
        {
            List<bool> editList = SiteDriver.FindElementAndGetActiveAttribute(
                AppealCategoryManagerPageObjects.DeleteIconEnabledCssLocator,
                How.CssSelector);
            return editList.TrueForAll(c => c);
        }

        public bool IsAddCategoryCodeIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AddCategoryCodeCssLocator,
                How.CssSelector);
        }

        public bool IsAddCategoryCodeIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AddCategoryCodeEnabledCssLocator,
                How.CssSelector);
        }

        public bool IsAddCategoryCodeIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AddCategoryCodeDisabledCssLocator,
                How.CssSelector);
        }

        public bool IsAddNewCategoryCodeSectionDisplayed()
        {
            if (SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AddNewCategoryCodeSectionXPath,
                How.XPath))
                return SiteDriver.FindElement(AppealCategoryManagerPageObjects.AddNewCategoryCodeSectionXPath,
                    How.XPath).Displayed;
            return false;
        }

        public void SetCategoryId(string categoryId)
        {
            JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.CategoryIdAddCategorySectionCssLocator).SendKeys(categoryId);
            Console.WriteLine("Category ID Entered: <{0}>", categoryId);
            WaitForStaticTime(2000);

        }

        public void SelectProductOnAddSection(string value)
        {
            JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.ProductAddCategorySectionCssLocator).Click();
            JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.ProductAddCategorySectionCssLocator
                ).SendKeys(value);

            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.ProductOnAddSectionXpathTemplate, value), How.XPath);
            Console.WriteLine("Product Selected: <{0}>", value);

        }

        public void SelectAnalystOnAddSection(string value, bool isDCIProduct = false)
        {
            if (!isDCIProduct)
            {
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                    How.CssSelector);

                SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                    How.CssSelector).SendKeys(value.Split('-')[0].Trim());
                JavaScriptExecutor.ExecuteClick(
                    string.Format(AppealCategoryManagerPageObjects.AnalystOnAddSectionXpathTemplate,
                        value.Split('-')[0].Trim()), How.XPath);
                Console.WriteLine("Analyst Selected: <{0}>", value.Split('-')[0].Trim());
            }
            else
            {
                JavaScriptExecutor.ExecuteClick(
                    AppealCategoryManagerPageObjects.AnalystAddCategorySectionForDCICssLocator,
                    How.CssSelector);

                SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAddCategorySectionForDCICssLocator,
                    How.CssSelector).SendKeys(value.Split('-')[0].Trim());
                JavaScriptExecutor.ExecuteClick(
                    string.Format(AppealCategoryManagerPageObjects.AnalystOnAddForDCISectionXpathTemplate,
                        value.Split('-')[0].Trim()), How.XPath);
                Console.WriteLine("Analyst Selected: <{0}>", value.Split('-')[0].Trim());
            }


        }

        public void SelectAnalystRestrictedClaimOnAddSection(string value)
        {
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector);

            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector).SendKeys(value.Split('-')[0].Trim());
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.AnalystRestrictedClaimsOnAddSectionXpathTemplate,
                    value.Split('-')[0].Trim()), How.XPath);
            Console.WriteLine("Analyst Selected: <{0}>", value.Split('-')[0].Trim());

        }

        public void SelectAnalystOnAddSectionByEnterKey(string value)
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                How.CssSelector);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                How.CssSelector).SendKeys(value);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                How.CssSelector).SendKeys(Keys.Enter);
        }

        public void SelectAnalystRestrictedClaimsOnAddSectionByEnterKey(string value)
        {
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector).SendKeys(value);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector).SendKeys(Keys.Enter);
        }

        public void SelectAnalystOnEditSectionByEnterKey(string value)
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator,
                How.CssSelector);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator,
                How.CssSelector).SendKeys(value.Split('-')[0]);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator,
                How.CssSelector).SendKeys(Keys.Enter);
        }

        public void SelectAnalystWithRestrictedAccessOnEditSectionByEnterKey(string value)
        {
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator,
                How.CssSelector);
            SiteDriver.FindElement(
                AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator,
                How.CssSelector).SendKeys(value);
            SiteDriver.FindElement(
                AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator,
                How.CssSelector).SendKeys(Keys.Enter);
        }

        public string GetAnalystInputDropDownValue(bool isAnalystNonRestricedClaims = true)
        {
            if (isAnalystNonRestricedClaims)
                return
                    SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                            How.CssSelector)
                        .GetAttribute("value");
            else
                return
                    SiteDriver.FindElement(
                            AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                            How.CssSelector)
                        .GetAttribute("value");
        }

        public void SelectCategoryOrderOnAddSection(string value)
        {
            JavaScriptExecutor.FindElement(
                AppealCategoryManagerPageObjects.CategoryOrderAddCategorySectionCssLocator).ClearElementField();
            JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.CategoryOrderAddCategorySectionCssLocator)
                .SendKeys(value);

            Console.WriteLine("Category Order Selected: <{0}>", value);

        }

        public void SelectClientOnAddSection(string client = "SMTST")
        {
            ClickOnAvailableAssignedRow(2, client);

        }

        public void EditClientOnEditSection(string assignedClient = null, string client = "SMTST")
        {
            if (assignedClient != null)
            {
                ClickOnAvailableAssignedRow(2, assignedClient, false);
            }
            else
            {
                TransferAllAvailableAssignedRow(2, false);

            }

            ClickOnAvailableAssignedRow(2, client);

        }

        public void SelectClientOnEditSection(string client = "RPE")
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.ClientOnEditSectionFieldCssLocator,
                How.CssSelector);

            SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.ClientOnEditSectionFieldCssLocator, How.CssSelector)
                .SendKeys(client);
            //JavaScriptExecutor.SendKeys(value, AppealCategoryManagerPageObjects.CategoryOrderAddCategorySectionCssLocator, How.CssSelector);
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.ClientOnEditSectionXpathTemplate, client),
                How.XPath);
            Console.WriteLine("Client Selected: <{0}>", client);

        }

        public string GetCategoryOrderOnAddOptionValue()
        {

            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.CategoryOrderAddCategorySectionCssLocator,
                How.CssSelector).GetAttribute("value");

        }

        public List<string> GetClientOnAddOptionList()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.ClientAddCategorySectionCssLocator,
                How.CssSelector);
            WaitForStaticTime(1000);
            var clientOptionList = JavaScriptExecutor.FindElements(
                AppealCategoryManagerPageObjects.ClientListValueOnAddSectionFieldCssLocator, How.CssSelector, "Text");
            return clientOptionList;

        }

        public List<string> GetAnalystListOnAddOption(bool unRestrictedUser = true)
        {
            if (unRestrictedUser)
            {
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                    How.CssSelector);

                var analystOptionList = JavaScriptExecutor.FindElements(
                    AppealCategoryManagerPageObjects.AnalaystListValueOnAddSectionFieldCssLocator, How.CssSelector,
                    "Text");
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystAddCategorySectionCssLocator,
                    How.CssSelector);
                analystOptionList.RemoveAt(0);
                return analystOptionList;
            }

            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector);

            var restrictedAnalystOptionList = JavaScriptExecutor.FindElements(
                AppealCategoryManagerPageObjects.AnalaystRestrictedListValueOnAddSectionFieldCssLocator,
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteClick(
                AppealCategoryManagerPageObjects.AnalystRestrictedClaimsAddCategorySectionCssLocator,
                How.CssSelector);
            restrictedAnalystOptionList.RemoveAt(0);
            return restrictedAnalystOptionList;

        }

        public List<string> GetAnalystListForDCIOnAddOption()
        {
            JavaScriptExecutor.ClickJQuery(
                string.Format(AppealCategoryManagerPageObjects.InputFieldByLabelNameInAddCategoryCSSLocator,
                    "Analyst"));

            var analystOptionList = JavaScriptExecutor.FindElements(
                AppealCategoryManagerPageObjects.AnalaystListValueOnAddSectionFieldForDCICssLocator, How.CssSelector,
                "Text");

            JavaScriptExecutor.ClickJQuery(
                AppealCategoryManagerPageObjects.InputFieldByLabelNameInAddCategoryCSSLocator);
            analystOptionList.RemoveAt(0);
            return analystOptionList;
        }

        /// <summary>
        /// "analystNonRestricted = true" for selecting the dropdown for non-restricted analysts in edit appeal category section.
        /// "analystNonRestricted = false" for selecting the dropdown for restrictd analysts in edit appeal category section.
        /// </summary>
        /// <param name="analystNonRestricted"></param>
        /// <returns></returns>
        public List<string> GetAnalystListOnEditOption(bool analystNonRestricted = true)
        {
            //string analystDivIndex = analystNonRestricted ? "4" : "5";
            List<string> nonRestrictedAnalystOptionList = new List<string>();

            if (analystNonRestricted)
            {
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator,
                    How.CssSelector);

                nonRestrictedAnalystOptionList = JavaScriptExecutor.FindElements(
                    AppealCategoryManagerPageObjects.AnalystListValueOnEditSectionFieldCssLocator, How.CssSelector,
                    "Text");
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator,
                    How.CssSelector);

            }
            else
            {
                JavaScriptExecutor.ExecuteClick(
                    AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator,
                    How.CssSelector);

                nonRestrictedAnalystOptionList = JavaScriptExecutor.FindElements(
                    AppealCategoryManagerPageObjects.AnalystListValueOnEditSectionForRestrictedAnalystsFieldCssLocator,
                    How.CssSelector, "Text");
                JavaScriptExecutor.ExecuteClick(
                    AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator,
                    How.CssSelector);

            }

            nonRestrictedAnalystOptionList.RemoveAt(0);
            return nonRestrictedAnalystOptionList;
        }

        public string GetAnalystDropDownValueOnFindSection(int row)
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);

            var analystOption = SiteDriver
                .FindElement(
                    string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionDropDownValueCssTemplate,
                        4, row), How.CssSelector).Text;
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);
            return analystOption;
        }

        public List<string> GetAnalystListOnFindSection()
        {
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);

            var analystOptionList = JavaScriptExecutor.FindElements(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldListFindSectionCssTemplate, 4),
                How.CssSelector, "Text");
            JavaScriptExecutor.ExecuteClick(
                string.Format(AppealCategoryManagerPageObjects.SearchFieldFindSectionCssTemplate, 4), How.CssSelector);
            analystOptionList.RemoveAt(0);
            return analystOptionList;

        }

        public void SetProcCodeFromOnAddSection(string value)
        {
            JavaScriptExecutor.FindElement(
                AppealCategoryManagerPageObjects.ProcCodeFromAddCategorySectionCssLocator).ClearElementField();
            JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.ProcCodeFromAddCategorySectionCssLocator)
                .SendKeys(value);
            WaitForStaticTime(2000);
            Console.WriteLine("Proc Code From Entered: <{0}>", value);


        }

        public void SetProcCodeToOnAddSection(string value)
        {

            JavaScriptExecutor.FindElement(
                AppealCategoryManagerPageObjects.ProcCodeToAddCategorySectionCssLocator).ClearElementField();
            JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.ProcCodeToAddCategorySectionCssLocator)
                .SendKeys(value);
            WaitForStaticTime(2000);
            Console.WriteLine("Proc Code To Entered: <{0}>", value);

        }

        public void SetTrigProcFromOnAddSection(string value)
        {
            JavaScriptExecutor.FindElement(
                AppealCategoryManagerPageObjects.TrigProcFromAddCategorySectionCssLocator).ClearElementField();
            JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.TrigProcFromAddCategorySectionCssLocator)
                .SendKeys(value);
            WaitForStaticTime(2000);
            Console.WriteLine("Trig Proc From Entered: <{0}>", value);

        }

        public void SetTrigProcToOnAddSection(string value)
        {
            JavaScriptExecutor.FindElement(
                AppealCategoryManagerPageObjects.TrigProcTodAddCategorySectionCssLocator).ClearElementField();
            JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.TrigProcTodAddCategorySectionCssLocator)
                .SendKeys(value);
            WaitForStaticTime(2000);
            Console.WriteLine("Trig Proc To Entered: <{0}>", value);

        }

        public bool IsFlagInAddCategorySectionPresent() =>
            JavaScriptExecutor.IsElementPresent(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector);

        public void SetFlagInAddAndEditCategorySection(string value, bool edit = false)
        {
            if (edit)
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).Click();
                JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).ClearElementField();
                JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator)
                    .SendKeys(value);
            }

            else
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .Click();

                JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector).ClearElementField();
                JavaScriptExecutor.FindElement(
                        AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .SendKeys(value);
            }

            JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.FlagDropDownListXPathTemplate, value), How.XPath);
            SiteDriver.WaitToLoadNew(300);
        }

        public void SetMultipleFlagInAddAndEditCategorySection(List<string> multipleValues , bool edit = false)
        {
            if (edit)
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).Click();
                JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).ClearElementField();
                foreach (var value in multipleValues)
                {
                    JavaScriptExecutor.FindElement(
                            AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator)
                        .SendKeys(value);
                    JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.FlagDropDownListXPathTemplate, value), How.XPath);
                }
            }

            else
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .Click();

                JavaScriptExecutor.FindElement(
                    AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector).ClearElementField();
                foreach (var value in multipleValues)
                {
                    JavaScriptExecutor.FindElement(
                            AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                        .SendKeys(value);
                    JavaScriptExecutor.ExecuteClick(
                        string.Format(AppealCategoryManagerPageObjects.FlagDropDownListXPathTemplate, value),
                        How.XPath);
                }
            }

            SiteDriver.WaitToLoadNew(300);
        }

        public string GetFlagPlaceholder(bool edit = false)
        {
            if(edit)
                return
                    JavaScriptExecutor.FindElement(
                        string.Format(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator)
                    ).GetAttribute("placeholder");
            
            
            return
                    JavaScriptExecutor.FindElement(
                        string.Format(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    ).GetAttribute("placeholder");
            
        }

        public string GetFlagInputValue(bool edit = false)
        {
            if (edit)
                return
                    JavaScriptExecutor.FindElement(
                        string.Format(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator)).GetAttribute("value");
           
            return
                JavaScriptExecutor.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)).GetAttribute("value");
            
        }

        public List<string> GetFlagDropDownList(bool edit = false)
        {
            if(edit)
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).Click();

            }

            else
            {
                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .Click();
            }

            SiteDriver.WaitToLoadNew(500);
            var list = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.AvalilableFlagDropDownListXPath, How.XPath, "Text");
            if (list.Count == 0)
            {
                if (edit) JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).Click();

                JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .Click();
                SiteDriver.WaitToLoadNew(500);
                list = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.AvalilableFlagDropDownListXPath, How.XPath, "Text");
            }
            return list;
        }

        public void ClickAllClearFlagOptionInAddCategoryByOption(string option, bool edit=false)
        {
            if (edit) JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator).Click();
            
            else JavaScriptExecutor.FindElement(AppealCategoryManagerPageObjects.FlagFromAddCategorySectionCssSelector)
                    .Click();
            

            JavaScriptExecutor
                .ClickJQuery(string.Format(
                    AppealCategoryManagerPageObjects.AllClearFlagOptionsInAddCategorySectionCssTemplate, option));
        }

        public void ScrollToLastLiOfPrimaryView()
        {
            JavaScriptExecutor.ExecuteToScrollToLastLi(AppealCategoryManagerPageObjects.LastLiOfPrimaryViewCssSelector);
        }

        public void ScrollToLastPosition()
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDiv(AppealCategoryManagerPageObjects.SaveButtonCssSelector);
        }

        public bool IsAddCategoryFormPresent() =>
            SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.AddCategoryFormCssSelector, How.CssSelector);

        public List<string> GetCVFlagListForAddCategoryFromDb() =>
            Executor.GetTableSingleColumn(AppealSqlScriptObjects.GetCVFlagsForAddCategory);

        public bool IsEditFormDisplayed(string label)
        {
            return SiteDriver.IsElementPresent(
                string.Format(AppealCategoryManagerPageObjects.EditFormXPathByLabel, label), How.XPath);
        }

        public bool IsEditFormPresent()
        {
            return SiteDriver.IsElementPresent(AppealCategoryManagerPageObjects.EditFormCssLocator, How.CssSelector);
        }

        public void ClickonEditIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.EditIconEnabledCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(IsEditFormPresent);
        }

        public void ClickonEditIconOfOrderZero()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.EditIconOfOrderZeroXpath, How.XPath);
            SiteDriver.WaitForCondition(IsEditFormPresent);
        }
        public void ClickonEditIconOfOrderNotNaOrZero()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.EditIconOfOrderNonNaOrZeroXpath, How.XPath);
            SiteDriver.WaitForCondition(IsEditFormPresent);
        }


        public bool IsProcCodeFromAllowOnlyFiveCharacter()
        {
            return
                SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.ProcCodeFromOnEditSectionFieldCssLocator, How.CssSelector)
                    .GetAttribute("maxlength")
                    .Contains("5");
        }

        public bool IsProcCodeToAllowOnlyFiveCharacter()
        {
            return
                SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.ProcCodeToOnEditSectionFieldCssLocator, How.CssSelector)
                    .GetAttribute("maxlength")
                    .Contains("5");
        }

        public string GetActiveOrderValueonEditSection()
        {
            return
                SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.CategoryOrderListValueOnEditSectionFieldCssLocator, How.CssSelector)
                    .GetAttribute("placeholder");
        }

        public bool IsFlagPresentOnEditSection() =>
            JavaScriptExecutor.IsElementPresent(AppealCategoryManagerPageObjects.FlagOnEditSectionFieldCssLocator);

        public bool IsCategoryOrderRecordCorrect()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.CategoryOrderOnEditSectionFieldCssLocator, How.CssSelector);
            JavaScriptExecutor.ExecuteMouseOver(AppealCategoryManagerPageObjects.CategoryOrderOnEditSectionFieldCssLocator, How.CssSelector);
            List<string> orderValue =
                JavaScriptExecutor.FindElements(
                    AppealCategoryManagerPageObjects.CategoryOrderListValueOnEditSectionFieldCssLocator, How.CssSelector, "Text");
            orderValue.RemoveAt(0);
            JavaScriptExecutor.ExecuteMouseOut(AppealCategoryManagerPageObjects.CategoryOrderListValueOnEditSectionFieldCssLocator, How.CssSelector);
            var row = GetAppealCategoryRowCount();
            for (int i = 0; i < row; i++)
            {
                if (Convert.ToInt32(orderValue[i]) != i)
                    return false;

            }
            return true;

        }

        public bool IsTrigProcFromAllowOnlyFiveCharacter()
        {
            return
                SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.TrigProcFromOnEditSectionFieldCssLocator, How.CssSelector)
                    .GetAttribute("maxlength")
                    .Contains("5");
        }

        public bool IsTrigProcToAllowOnlyFiveCharacter()
        {
            return
                SiteDriver.FindElement(
                    AppealCategoryManagerPageObjects.TrigProcToOnEditSectionFieldCssLocator, How.CssSelector)
                    .GetAttribute("maxlength")
                    .Contains("5");
        }

        //public void SelectProductOnEditSection(string value)
        //{
        //    JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.ProductOnEditSectionFieldCssLocator, How.CssSelector);
        //    SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProductOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);
        //    if (value != "")
        //    {
        //        JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.ProductOnEditSectionXpathTemplate, value), How.XPath);
        //        Console.WriteLine("Product Selected: <{0}>", value);
        //    }
        //    else
        //    {
        //        JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.ProductListValueOnEditSectionFieldCssLocator, How.CssSelector);
        //        Console.WriteLine("No Product Selected");
        //    }

        //}

        //public string GetProductOnEditSection()
        //{
        //    return  SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProductOnEditSectionFieldCssLocator,
        //            How.CssSelector).Value;
        //}

        public string GetAnalystOnEditSection()
        {
            return JavaScriptExecutor.FindElement(string.Format(AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor,
                AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue())).Text.Trim();
            //return JavaScriptExecutor.FindElements(string.Format(AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor, AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()),"Text");
        }

        public List<string> GetSelectedAnalystList(bool analystListWithRestriction = false)
        {
            if(!analystListWithRestriction)
                return JavaScriptExecutor.FindElements(string.Format(
                        AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor,
                        AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()),"Text").Select(x => x.Split('-')[0].Trim()).ToList();
            else
                return JavaScriptExecutor.FindElements(string.Format(
                    AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor,
                    AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()), "Text");

        }

        public List<string> GetSelectedAnalystListFromRestrictedAnalystSection(bool analystListWithRestriction = false)
        {
            if (!analystListWithRestriction)
                return JavaScriptExecutor.FindElements(string.Format(AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor, AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue()),
                "Text").Select(x => x.Split('-')[0].Trim()).ToList();
            else
                return JavaScriptExecutor.FindElements(string.Format(AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor, AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue()),
                    "Text");
        }

        public List<string> GetSelectedAnalystListForDCI()
        {
            return JavaScriptExecutor.FindElements(string.Format(
                AppealCategoryManagerPageObjects.SelectedAnalystValueCssLoctor,"Analyst"), "Text").Select(x => x.Split('-')[0].Trim()).ToList();
        }

        public string GetCategoryOrderOnEditSection()
        {
            return SiteDriver.FindElementAndGetAttribute(AppealCategoryManagerPageObjects.CategoryOrderOnEditSectionFieldCssLocator,
                    How.CssSelector,"value");
        }

        public string GetProcCodeFromOnEditSection()
        {
            return
                SiteDriver.FindElementAndGetAttribute(
                    AppealCategoryManagerPageObjects.ProcCodeFromOnEditSectionFieldCssLocator, How.CssSelector,"value");
        }

        public string GetProcCodeToOnEditSection()
        {
            return
                SiteDriver.FindElementAndGetAttribute(
                    AppealCategoryManagerPageObjects.ProcCodeToOnEditSectionFieldCssLocator, How.CssSelector,"value");
        }

        public string GetTrigCodeFromOnEditSection()
        {
            return
                SiteDriver.FindElementAndGetAttribute(
                    AppealCategoryManagerPageObjects.TrigProcFromOnEditSectionFieldCssLocator, How.CssSelector,"value");
        }

        public string GetTrigCodeToOnEditSection()
        {
            return
                SiteDriver.FindElementAndGetAttribute(
                    AppealCategoryManagerPageObjects.TrigProcToOnEditSectionFieldCssLocator, How.CssSelector,"value");
        }
        public void SelectAnalystOnEditSection(string value, bool delete)
        {
            if (delete)
            {
                var count = GetDeleteAnalystIconCount();
                for (var i = 1; i <= count; i++)
                {
                    JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,
                        AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue(), 1));

                }
            }
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator, How.CssSelector);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value.Split('-')[0].Trim());
            if (value != "")
            {
                JavaScriptExecutor.ExecuteClick(
                    string.Format(AppealCategoryManagerPageObjects.AnalystOnEditSectionXpathTemplate, value.Split('-')[0].Trim()), How.XPath);
                Console.WriteLine("Analyst Selected: <{0}>", value.Split('-')[0].Trim());
            }
            else
            {
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystListValueOnEditSectionFieldCssLocator, How.CssSelector);
                Console.WriteLine("No Analyst Selected");
            }

        }

        public void SelectAnalystWithRestrictionAccessOnEditSection(string value, bool delete)
        {
            if (delete)
            {
                var count = GetDeleteAnalystIconInRestrictedAccessAnalystSectionCount();
                for (var i = 1; i <= count; i++)
                {
                    JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate, AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), 1));

/*                    JavaScriptExecutor.ExecuteClick(
                        string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate, AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), 1),
                        How.CssSelector);*/
                }
            }
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator, How.CssSelector);
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator, How.CssSelector).SendKeys(value.Split('-')[0].Trim());
            if (value != "")
            {
                JavaScriptExecutor.ExecuteClick(
                    string.Format(AppealCategoryManagerPageObjects.AnalystWithRestrictedAccessOnEditSectionXpathTemplate, value.Split('-')[0].Trim()), How.XPath);
                Console.WriteLine("Analyst Selected: <{0}>", value.Split('-')[0].Trim());
            }
            else
            {
                JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.AnalystListValueOnEditSectionForRestrictedAnalystsFieldCssLocator, How.CssSelector);
                Console.WriteLine("No Analyst Selected");
            }

        }

        public int GetDeleteAnalystIconCount()
        {
            /*return SiteDriver.FindElementsCount(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystListCssSelector, AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()),
                How.CssSelector);*/
            
            return JavaScriptExecutor.FindElementsCount(string.Format(
                AppealCategoryManagerPageObjects.DeleteAnalystListCssSelector,
                AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()));
        }

        public int GetDeleteAnalystIconInRestrictedAccessAnalystSectionCount()
        {
            /*return SiteDriver.FindElementsCount(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystListCssSelector, AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue()),
                How.CssSelector);*/

            return JavaScriptExecutor.FindElementsCount(string.Format(
                AppealCategoryManagerPageObjects.DeleteRestrictedAccessAnalystListCssSelector,
                AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue()));
        }

        /// <summary>
        /// if row==0 then delete all row otherwise selected row will be deleted
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isDCIProduct">Set 'true' if deleting analyst for DCA appeal category</param>
        public void ClickDeleteAnalyst(int row = 0, bool isDCIProduct = false)
        {
            string label = isDCIProduct ? "Analyst" : AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue();

            if (row == 0)
            {
                var count = GetDeleteAnalystIconCount();
                for (var i = count; i >= 1; i--)
                {
                    JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate, label, i));

                    /* JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue(), i),
                        How.CssSelector);*/
                }
            }
            else
                JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,
                    label, row));
        }

        public void ClickDeleteAnalystFromRestrictionAccessAnalystSection(int row = 0)
        {
            if (row == 0)
            {
                var count = GetDeleteAnalystIconInRestrictedAccessAnalystSectionCount();
                for (var i = count; i >= 1; i--)
                {
                    JavaScriptExecutor.ClickJQuery(string.Format(
                        AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,
                        AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), i));

                    /*JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), i),
                        How.CssSelector);*/
                }
            }
            else
                JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,
                    AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), row));
            /*JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.DeleteAnalystCssTemplate,AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue(), row),
                How.CssSelector);*/
        }
        /// <summary>
        /// move down the anlayst of given row
        /// </summary>
        /// <param name="row"></param>
        public void ClickMoveDownAnalystByRow(int row = 1)
        {

            JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.MoveDownAnalystByRowCssTemplate, row),
                How.CssSelector);
            
        }

        /// <summary>
        /// move up the anlayst of given row
        /// </summary>
        /// <param name="row"></param>
        public void ClickMoveUpAnalystByRow(int row)
        {

            JavaScriptExecutor.ExecuteClick(string.Format(AppealCategoryManagerPageObjects.MoveUpAnalystByRowCssTemplate, row),
                How.CssSelector);

        }

        /// <summary>
        /// move down the anlayst of given name
        /// </summary>
        /// <param name="analyst">name of the analyst</param>
        /// <param name="add">flag to verify create form or edit form</param>
        public void ClickMoveDownAnalystByAnalystName(string analyst, bool add = true)
        {
            string formType = add ? "Add" : "Edit";
            JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.MoveDownFollowingAnalystCssTemplate, formType, analyst));

        }

        /// <summary>
        /// move up the anlayst of given name
        /// </summary>
        /// <param name="analyst">name of the analyst</param>
        /// <param name="add">flag to verify create form or edit form</param>
        public void ClickMoveUpAnalystByAnalystName(string analyst, bool add =true)
        {
            string formType = add ? "Add" : "Edit";
            JavaScriptExecutor.ClickJQuery(string.Format(AppealCategoryManagerPageObjects.MoveUpFollowingAnalystCssTemplate, formType, analyst));

        }

        public void SelectCategoryOrderOnEditSection(string value)
        {
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.CategoryOrderOnEditSectionFieldCssLocator,
                How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealCategoryManagerPageObjects.CategoryOrderOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);            
        }

        public void SetNote(String note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            SiteDriver.FindElement("body", How.CssSelector).Click();
            SiteDriver.FindElement("body", How.CssSelector).ClearElementField();
            SiteDriver.FindElement("body", How.CssSelector).SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();

        }

        public string  GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("p", How.CssSelector).Text;
            Console.WriteLine("Note value is : {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }


        public void ClickOnSaveButtonInCreateSection()
        {
            SiteDriver.WaitToLoadNew(500);
            JavaScriptExecutor.ClickJQuery(AppealCategoryManagerPageObjects.SaveButtonInAddFormsCssLocator);
            WaitForWorking();
            Console.WriteLine("Clicked on Save Button in Add Section");
            
        }

        public void ClickOnSaveButtonInEdit()
        {
            JavaScriptExecutor.ClickJQuery(AppealCategoryManagerPageObjects.SaveButtonInEditCssLocator);
            Console.WriteLine("Clicked on Save Button in Edit Section");

        }

        //public void WaitForWorking()
        //{
            
        //    SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        //    SiteDriver.WaitForPageToLoad();
            
        //}

        public void ClickOnCancelButton()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.CancelButtonCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Cancel Button");
            WaitForWorking();
        }
        public void ClickOnDisabledCancelButton()
        {
            JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.DisabledCancelButtonCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Cancel Button");
            WaitForWorking();
        }

        public void SetProcCodeFromOnEditSection(string value)
        {
            if (value != "")
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeFromOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeFromOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);
                Console.WriteLine("Proc Code From Entered: <{0}>", value);
            }
            else
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeFromOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                Console.WriteLine("Proc Code From Cleared");
            }
        }

        public void SetProcCodeToOnEditSection(string value)
        {
            if (value != "")
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeToOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeToOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);
                Console.WriteLine("Proc Code To Entered: <{0}>", value);
            }
            else
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.ProcCodeToOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                Console.WriteLine("Proc Code From Cleared");
            }
        }

        public void SetTrigProcFromOnEditSection(string value)
        {
            if (value != "")
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcFromOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcFromOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);
                Console.WriteLine("Trig Proc From Entered: <{0}>", value);
            }
            else
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcFromOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                Console.WriteLine("Trig Proc From Cleared");
            }
        }

        public void SetTrigProcToOnEditSection(string value)
        {
            if (value != "")
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcToOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcToOnEditSectionFieldCssLocator, How.CssSelector).SendKeys(value);
                Console.WriteLine("Trig Proc To Entered: <{0}>", value);
            }
            else
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.TrigProcToOnEditSectionFieldCssLocator, How.CssSelector).ClearElementField();
                Console.WriteLine("Trig Proc From Cleared");
            }
        }

        public string GetProductValueAppealRowByRow(int row)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowColValueCssTemplate, row, 4),
                    How.CssSelector).Text;
        }

        public string[] GetProcCodeValueAppealRowByRow(int row=1, bool excludeDci= false)
        {
            if (excludeDci)
                return GetFirstProcValueExcludingDci().Split('-');
            return
                SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowColValueCssTemplate, row, 6),
                    How.CssSelector).Text.Split('-');
        }

        public String GetCategoryValueByRow(int row)
        {
            return SiteDriver.FindElement(
                string.Format(AppealCategoryManagerPageObjects.CategoryLabelCssLocatorByRow, row),
                How.CssSelector).Text;
        }

        public string GetTableValueByRowColumn(int row, int column)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowColValueCssTemplate, row, column),
                    How.CssSelector).Text;
        }

        public string GetTableRawTextByRowColumn(int row, int column)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowColValueCssTemplatewLabel, row, column),
                    How.CssSelector).Text;
        }


        public string GetAnalystAssignmentHeader()
        {
            return SiteDriver.FindElement(AppealCategoryManagerPageObjects.AnalystAssignmentHeaderCssLocator,
                How.CssSelector).Text;
        }

        public List<string> GetAssignedAnalystByType(bool restricted)
        {
            var type = restricted ? "Analysts (restricted)" : "Analysts (non-restricted)";
            
            return JavaScriptExecutor.FindWebElements(String.Format(AppealCategoryManagerPageObjects.AssignedAnalystByType,type)).Select(x=>x.Text.Split('\n')[1].Split('(')[0]).ToList();
        }

        public List<string> GetAnalystAssignmentSubHeaderList()
        {
            return JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.AnalystAssignmentSubHeaderCsslocator, How.CssSelector, "Text");
        }

        public List<string> GetAnalystListOnAnalystAssignment(bool isRestrictedAnalystsAssigned = false, int row = 1)
        {
            if (!isRestrictedAnalystsAssigned)
                return JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.AnalystAssignmentListItems, How.CssSelector, "Text");
            else
                return JavaScriptExecutor.FindElements(string.Format(AppealCategoryManagerPageObjects.RestrictedAnalystAssignmentListItems, row), How.CssSelector, "Text");
        }

        public string GetTrigProcValueAppealRowByRow(int row=1, bool excludeDci= false)
        {
            if (excludeDci)
                return GetFirstTrigValueExcludingDci();
            return
                SiteDriver.FindElement(
                    string.Format(AppealCategoryManagerPageObjects.AppealCategoryRowColValueCssTemplate, row, 7),
                    How.CssSelector).Text;
        }

        public void ClickonEditIconByRow(int row,bool closeEditForm=false)
        {
            JavaScriptExecutor.ExecuteClick($"li:nth-of-type({row}) " + AppealCategoryManagerPageObjects.EditIconEnabledCssLocator, How.CssSelector);
            if(closeEditForm)
                SiteDriver.WaitForCondition(()=>!IsEditFormPresent());
            else
                SiteDriver.WaitForCondition(IsEditFormPresent);
        }

        public void ClickOnEditIconByCategoryId(string categoryId, bool closeEditForm = false)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.EditIconByCategoryIdXPathTemplate, categoryId),How.XPath).Click();
            if (closeEditForm)
                SiteDriver.WaitForCondition(() => !IsEditFormPresent());
            else
                SiteDriver.WaitForCondition(IsEditFormPresent);
            
        }
        public void ClickOnEditIconByCategoryOrder(string categoryOrder)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.EditIconByCategoryOrderXPathTemplate, categoryOrder),How.XPath).Click();
            SiteDriver.WaitForCondition(IsEditFormPresent);
            
        }
        public string GetHeaderEditAppealCategory()
        {
            return
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.EditAppealCategoryHeaderCssSelector,
                    How.CssSelector).Text;
        }

        public List<int> GetCategoryOrderList()
        {
            List<String> categoryOrderList = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.OrderValueCssLocator, How.CssSelector, "Text");
            categoryOrderList.RemoveAll(item => item.Contains("NA"));
            return categoryOrderList.Select(s => int.Parse(s)).ToList();
        }

        public List<string> GetProductListAppealCategoryRow(bool excludeDci = false)
        {
            var list = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.ProductValueCssLocator, How.CssSelector, "Text"); 
            if(excludeDci)

                list.RemoveAll(item => item.Contains("DCA"));
            return list;

        }
        public List<string> GetProcCodeListAppealCategoryRow(bool excludeDci = false)
        {
            var list = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.ProcValueCssLocator, How.CssSelector, "Text");
            if (excludeDci)

                list.RemoveAll(item => item.Contains("NA"));
            return list;

        }
        public List<string> GetTrigProcListAppealCategoryRow(bool excludeDci = false)
        {
            var list = JavaScriptExecutor.FindElements(AppealCategoryManagerPageObjects.TrigValueCssLocator, How.CssSelector, "Text");
            if (excludeDci)

                list.RemoveAll(item => item.Contains("NA"));
            return list;

        }

        public bool IsListShifted(List<string>[] beforeShifted, List<string>[] aferShifted)
        {
            for (int i = 0; i < beforeShifted[0].Count-1; i++)
            {
                if (i == beforeShifted[0].Count - 1)
                {
                    if (Comparer<string>.Default.Compare(beforeShifted[0][i], aferShifted[0][0]) != 0
                        || Comparer<string>.Default.Compare(beforeShifted[1][i], aferShifted[1][0]) != 0
                        || Comparer<string>.Default.Compare(beforeShifted[2][i], aferShifted[2][0]) != 0)
                   return false;
                }
                if (Comparer<string>.Default.Compare(beforeShifted[0][i], aferShifted[0][i + 1]) != 0
                    || Comparer<string>.Default.Compare(beforeShifted[1][i], aferShifted[1][i + 1]) != 0
                    || Comparer<string>.Default.Compare(beforeShifted[2][i], aferShifted[2][i + 1]) != 0)
                    return false;
                    

            }
            return true;
        }

        public bool IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(string product,string procCode,string trigCode,string client="SMTST")
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(
                        AppealCategoryManagerPageObjects.AppealRowCategoryByClientProductProcCodeTrigCodeXPathTemplate,client,
                        product, procCode, trigCode), How.XPath);
        }
        public void ClickOnAppealRowCategoryByProductProcCodeTrigCodePresent(string product, string procCode, string trigCode, string client = "SMTST")
        {
            SiteDriver.FindElement(string.Format(
                        AppealCategoryManagerPageObjects.AppealRowCategoryByClientProductProcCodeTrigCodeXPathTemplate, client,
                        product, procCode, trigCode), How.XPath).Click();
            Console.WriteLine("Click On Appeal Category with Product: {0}, Proc: {1}, Trig: {2}, Client: {3}", product, procCode, trigCode, client);
            WaitForWorking();
        }

        public string GetCategoryIdByProductProcCodeTrigCodePresent(string product, string procCode, string trigCode, string client = "SMTST")
        {
           return SiteDriver.FindElement(string.Format(
                AppealCategoryManagerPageObjects.GetCategoryIdByProductProcCodeTrigCodePresent, client,
                product, procCode, trigCode), How.XPath).Text;
        }

        public void ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(string product, string procCode, string trigCode,string client="SMTST")
        {
            JavaScriptExecutor.ExecuteClick(string.Format(
                        AppealCategoryManagerPageObjects.DeleteIconByClientProductProcCodeTrigCodeXPathTemplate,client,
                        product, procCode, trigCode), How.XPath);
            Console.WriteLine("Deleted Appeal Category with Product: {0}, Proc: {1}, Trig: {2}, Client: {3}", product, procCode, trigCode, client);
            WaitForWorking();
        }

        public void ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(string product, string procCode, string trigCode, string client = "SMTST")
	    {
            WaitForWorkingAjaxMessage();
		    JavaScriptExecutor.ExecuteClick(string.Format(
			    AppealCategoryManagerPageObjects.EditIconByClientProductProcCodeTrigCodeXPathTemplate, client,
			    product, procCode, trigCode), How.XPath);
		    Console.WriteLine("Edit Appeal Category with Product: {0}, Proc: {1}, Trig: {2}, Client: {3}", product, procCode, trigCode, client);
		    WaitForWorking();
	    }


        public void DeleteAppealCateogryAndAssociatedAppeals(string categoryId,string product,string procCodeFrom,string procCodeTo)
        {

            StringFormatter.PrintMessage(
                "Delete appeals associated if there are any, and delete appeal category.");
            if (
                IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(
                    product, procCodeFrom + "-" + procCodeTo, null))
            {
                ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(product, procCodeFrom + "-" + procCodeTo, null);
                if (CheckPopupAndDeleteAppeal(categoryId,procCodeFrom))
                {
                    if(IsPageErrorPopupModalPresent())
                        ClosePageError();

                    NavigateToAppealCategoryManager();
                    ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(product,
                        procCodeFrom + "-" + procCodeTo, null);

                }

                ClickOkCancelOnConfirmationModal(true);

            }
        }

        public void DeleteAppealSpecificToAppealCategory(string category)
        {
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.deleteCategorySpecificAppeal,category));
        }

        public bool CheckPopupAndDeleteAppeal(string categoryId,string procCodeFrom)
        {
            if (IsPageErrorPopupModalPresent())
            {
                if (
                    GetPageErrorMessage()
                        .Equals(string.Format(
                            "Please complete all appeals with the {0} category code prior to deleting it.",
                            categoryId)))
                {
                    ClosePageError();
                    var _appealManager = NavigateToAppealManager();
                    _appealManager.SelectAllAppeals();
                    _appealManager.SelectSMTST();
                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                    _appealManager.SelectDropDownListbyInputLabel("Status", "New");
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Category", categoryId);
                    //_appealManager.SetInputFieldByInputLabel("First Pay Code", procCodeFrom);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorking();
                    if (!_appealManager.IsNoMatchingRecordFoundMessagePresent())
                    {
                        while (_appealManager.GetAppealSearchResultRowCount() != 0)
                        {
                            _appealManager.ClickOnDeleteIconByRowSelector(1);
                            _appealManager.ClickOkCancelOnConfirmationModal(true);
                        }
                    }

                    
                }
                return true;
            }
            else
                return false;
        }

        public void ClickOkCancelOnConfirmationModal(bool confirmation)
        {

            if (confirmation)
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.OkConfirmationCssSelector, How.CssSelector).Click();
                WaitForWorking();
                Console.WriteLine("Ok Button is Clicked");

            }
            else
            {
                SiteDriver.FindElement(AppealCategoryManagerPageObjects.CancelConfirmationCssSelector, How.CssSelector).Click();
                WaitForWorking();
                Console.WriteLine("Cancel Button is Clicked");
            }

        }

        /// <summary>
            /// Check whether values are in ascending order or not by Ordinal
            /// </summary>
            /// <param name="values"></param>
            /// <returns></returns>
            public  bool IsInAscendingOrderByOrdinal(List<String> values)
        {
            var sorted = new List<string>(values);
            sorted.Sort(StringComparer.Ordinal);
            for (var i = 0; i < values.Count - 1; i++)
            {
                if (values[i].Equals(sorted[i], StringComparison.Ordinal)) continue;
                Console.WriteLine("{0} and {1}", sorted[i], sorted[i]);
                return false;
            }
            return true;


        }

        /// <summary>
        /// Check whether values are in ascending order or not only for analyst
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool IsAnalystInAscendingOrder(List<String> searchListValue)
        {
            var searchListValues = searchListValue.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList();
            var sorted = new List<string>(searchListValues);
            sorted.Sort();
            for (var i = 0; i < sorted.Count; i++)
            {
                if (searchListValues[i].Equals(sorted[i])) continue;
                Console.WriteLine("{0} and {1}", searchListValues[i], sorted[i]);
                return false;
            }
            return true;
        }

        //Don't hard delete an appeal
        /*public void DeleteAppealsOnCategory(string category)
        {
            var temp = string.Format(AppealSqlScriptObjects.DeleteAppealOnACategory, category);
            Executor.ExecuteQuery(temp);
        }*/

        public void ClickDeleteIconByRow(int row)
        {
            SiteDriver.FindElement(string.Format(AppealCategoryManagerPageObjects.DeleteIconCssTemplate, row), How.CssSelector).Click();
        }

        public void CreateAppealCategoryForMultipleAnalyst(string categoryId, string product, string procFrom, string procTo, string[] analysts, string productAbbreviation, string client="SMTST", string catOrder = "0", string trigFrom = null, string trigTo = null, string[] analystForRestrictedClaims = null)
        {
            //DeleteAppealsOnCategory(categoryId);
            if (string.IsNullOrEmpty( trigFrom) && string.IsNullOrEmpty(trigTo))
            {
                DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation, procFrom + "-" + procTo, null,categoryId:categoryId);
            }
            else
            {
                DeleteAppealCategoryByProductProcCodeTrigCode(productAbbreviation, procFrom + "-" + procTo, trigFrom + "-" + trigTo);
            }
            ClickAddNewCategory();
            SetCategoryId(categoryId);
            SetProcCodeFromOnAddSection(procFrom);
            SetProcCodeToOnAddSection(procTo);
            SelectCategoryOrderOnAddSection(catOrder);
            SelectClientOnAddSection(client);
            SelectProductOnAddSection(product);
            if (!String.IsNullOrEmpty(trigFrom)) SetTrigProcFromOnAddSection(trigFrom);
            if (!String.IsNullOrEmpty(trigTo)) SetTrigProcToOnAddSection(trigTo);
            foreach (var analyst in analysts)
            {
                SelectAnalystOnAddSection(analyst);
            }

            if (analystForRestrictedClaims != null)
            {
                foreach (var analyst in analystForRestrictedClaims)
                {
                    SelectAnalystRestrictedClaimOnAddSection(analyst);
                }
            }

            
            ClickOnSaveButtonInCreateSection();
            WaitForWorking();
            Console.WriteLine("New Appeal Cateogry Created");
        }

        public void CreateAppealCategoryForMultipleAnalystForDci(List<string> analystList, string clientCode= "SMTST", bool deletePriorRecord = true)
        {
            //Deletes any prior DCA appeal category record 
            if (deletePriorRecord)
            {
                DeleteAppealCategoryByProductProcCodeTrigCode("DCA", "NA", "NA", clientCode);
                WaitForWorking();
            }

            ClickAddNewCategory(true,  "Dental Claim Accuracy");
            SelectClientOnAddSection();

            foreach (var analyst in analystList)
            {
                SelectAnalystOnAddSection(analyst, true);
            }

            ClickOnSaveButtonInCreateSection();
            WaitForWorking();
            Console.WriteLine("New Appeal Category Created for DCA product");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">0:categoryid,1: proc code from, 2:proc code to, 3:category oder, 4:product, 5:trigger from, 6:trigger to,7:analyst, 8:note, 9:product abbreaviation</param>
        /// <param name="client"></param>
        public void CreateAppealCategory(String[] data, string client = "SMTST", bool restrictedAnalyst = false, bool categoryOrder = true)
        {
            if (data[5] == "" && data[6] == "")
            {
                DeleteAppealCategoryByProductProcCodeTrigCode(data[9], data[1] + "-" + data[2],
                    null);
            }
            else
            {
                DeleteAppealCategoryByProductProcCodeTrigCode(data[9], data[1] + "-" + data[2],
                    data[5] + "-" + data[6]);
            }
            ClickAddNewCategory();
            SelectProductOnAddSection(data[4]);
            SelectClientOnAddSection(client);
            SetCategoryId(data[0]);
            SetProcCodeFromOnAddSection(data[1]);
            SetProcCodeToOnAddSection(data[2]);
            if (categoryOrder)
            {
                SelectCategoryOrderOnAddSection(data[3]);
            }
            SetTrigProcFromOnAddSection(data[5]);
            SetTrigProcToOnAddSection(data[6]);
            SelectAnalystOnAddSection(data[7]);
            //SetNote(data[8]);

            if (restrictedAnalyst)
                SelectAnalystRestrictedClaimOnAddSection(data[8]);

            ClickOnSaveButtonInCreateSection();
            WaitForWorking();
            Console.WriteLine("New Appeal Cateogry Created");
        }


        public void UpdateAppealCategory(String[] data)
        {
            ClickonEditIcon();
            SetProcCodeFromOnEditSection(data[1]);
            SetProcCodeToOnEditSection(data[2]);
            //SelectProductOnEditSection(data[4]);
            SetTrigProcFromOnEditSection(data[5]);
            SetTrigProcToOnEditSection(data[6]);
            SetNote(data[8]);
            SelectAnalystOnEditSection(data[7], true);
            ClickOnSaveButtonInEdit();
            WaitForWorking();
            WaitForStaticTime(2000);
            CaptureScreenShot("Appeal Category Issue");
            WaitForStaticTime(2000);
            Console.WriteLine("Appeal Category Updated");
        }

        public void UpdateAppealCategoryNoteOfOrderZero(string note)
        {
            var loadMoreValue = GetLoadMoreText();
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
            var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
            for (var i = 0; i < count; i++)
            {
                ClickOnLoadMore();
            }
            ClickonEditIconOfOrderZero();           
            SetNote(note);
            ClickOnSaveButtonInEdit();
            WaitForWorking();
            Console.WriteLine("Appeal Category Updated");
        }


	    public void UpdateAppealCategoryNoteByProductProcCodeTrigCode(string product, string procCode, string trigCode, string note, string clientCode = "SMTST")
		{
			ClickOnEditAppealRowCategoryByProductProcCodeTrigCodePresent(product,procCode,trigCode);
		    SetNote(note);
            ClickOnSaveButtonInEdit();
		    WaitForWorking();
		    Console.WriteLine("Appeal Category Updated for category having having Product<{0}>\t ProcCode<{1}>\t TrigCode<{2} Client {3}>", product, procCode, trigCode, clientCode);
		}




		public void DeleteAppealCategoryByProductProcCodeTrigCode(string product, string procCode, string trigCode, string clientCode = "SMTST",bool isDeleteAll=false,string categoryId=null)
        {
            while (IsAppealRowCategoryByProductProcCodeTrigCodeClientCodePresent(product, procCode, trigCode, clientCode))
            {
                ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(product, procCode, trigCode, clientCode);
                if (categoryId!=null && CheckPopupAndDeleteAppeal(categoryId, procCode))
                {
                    if (IsPageErrorPopupModalPresent())
                        ClosePageError();
                    NavigateToAppealCategoryManager();
                    ClickOnDeleteAppealRowCategoryByProductProcCodeTrigCodePresent(product, procCode, trigCode, clientCode);

                }
                ClickOkCancelOnConfirmationModal(true);//ok button clicked
                Console.WriteLine("Appeal Category Deleted having Product<{0}>\t ProcCode<{1}>\t TrigCode<{2} Client {3}>", product, procCode, trigCode, clientCode);
                if(!isDeleteAll)
                    break;
            }
        }

        public bool IsReviewerPresentInAnalystAssignedSideWindow(int row)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealCategoryManagerPageObjects.RestrictedAnalystAssignmentListItems, row), How.CssSelector);
        }

        public string GetSelectedAppealCategoryValueByRow(int col = 2)
        {
            return SiteDriver.FindElement(
                string.Format(AppealCategoryManagerPageObjects.GetSelectedAppealCategoryLabelByColumnXpath, col),
                How.XPath).Text;
        }

        public int GetAnalystListOnAnalystAssignmentCount()
        {
           return SiteDriver.FindElementsCount(AppealCategoryManagerPageObjects.AnalystAssignmentListItems, How.CssSelector);
        }

        #region sql

        public List<string> GetAssignedClientListFromDB(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetAssignedClientList, userId));
        }

        public List<string> GetExpectedAnalystList(bool nonRestrictedAnalyst = true)
        {
            List<string> userList = new List<string>();

            if (nonRestrictedAnalyst)
                userList =
                    Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.UserListHavingAppealCanBeAssignedAuthority));
            else
                userList =
                    Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.UserListHavingAppealCanBeAssignedAuthorityForRestrictedAppeals));

            userList.Sort();
            Console.WriteLine("The list of users with 'Appeals can be assigned to user' authority ");
            return userList;
        }

        public List<string> GetAnalysListWithoutRestrictionDescription()
        {
            List<string> analystList = new List<string>();

            analystList = Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetAnalysListWithoutRestrictionDescription));
            analystList.Sort();

            return analystList;
        }

        public bool IsRestrictedAndNonRestrictedAnalystsSectionsPresent()
        {
            bool nonRestrictedAnalystSectionPresent = false;
            bool restrictedAnalystSectionPresent = false;
            
            nonRestrictedAnalystSectionPresent = JavaScriptExecutor.IsElementPresent(string.Format(
                AppealCategoryManagerPageObjects.EditFormLabelCSSLocator, AppealCategoryManagerAnalystSectionLabels.NonRestrictionAccessAnalysts.GetStringValue()));

            restrictedAnalystSectionPresent = JavaScriptExecutor.IsElementPresent(string.Format(AppealCategoryManagerPageObjects.EditFormLabelCSSLocator,
                AppealCategoryManagerAnalystSectionLabels.RestrictionAccessAnalysts.GetStringValue()));

            return (nonRestrictedAnalystSectionPresent && restrictedAnalystSectionPresent);
        }

        public bool IsInputFieldPresentInAddCategory(string inputFieldLabel)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(AppealCategoryManagerPageObjects.InputFieldByLabelNameInAddCategoryCSSLocator, 
                inputFieldLabel));
        }

        public List<string> GetClientListForDCI(string userID)
        {
            return 
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetAssignedClientListForDCI, userID));
        }

        #endregion
        #endregion

        #region PRIVATE METHODS
        #endregion


    }
}
