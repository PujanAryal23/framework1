using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using Nucleus.Service.PageServices.Base;
using OpenQA.Selenium;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public class PaginationService
    {
        #region PRIVATE PROPERTIES

        private readonly MethodInfo _waitForGridToLoad;
        private readonly PaginationPage _paginationPageObject;
        private readonly object _pageObject;
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;



        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// List of page sizes 
        /// </summary>
        public IList<int> PageSizes
        {
            get
            {
                var pageSizes = new List<int> { 10, 20, 50 };
                return pageSizes;
            }
        }

        /// <summary>
        /// Records info part
        /// </summary>
        public string RecordsInfo
        {
            get
            {
                SiteDriver.InitializePageElement(_paginationPageObject);
                var element = SiteDriver.FindElement(PaginationPage.RgInfoPartXPath, How.XPath);
                return element.Text;
            }
        }

        /// <summary>
        /// Total number of records
        /// </summary>
        public int TotalNoOfRecords
        {
            get
            {
                int first = RecordsInfo.IndexOf("of", StringComparison.Ordinal) + 2;
                int last = RecordsInfo.IndexOf("in", StringComparison.Ordinal);
                SiteDriver.WaitForIe(2000);
                return int.Parse(RecordsInfo.Substring(first, last - first).Trim());
            }
        }

        /// <summary>
        /// Total no of  pages in records info part
        /// </summary>
        public int NoOfPages
        {
            get
            {
                int first = RecordsInfo.IndexOf("in", StringComparison.Ordinal) + 3;
                int last = RecordsInfo.IndexOf("Page", StringComparison.Ordinal);
                return int.Parse(RecordsInfo.Substring(first, last - first).Trim());
            }
        }

        /// <summary>
        /// Start page in records info part
        /// </summary>
        public int StartPage
        {
            get
            {
                int first = RecordsInfo.IndexOf("Records", StringComparison.Ordinal) + 7;
                int last = RecordsInfo.IndexOf("-", StringComparison.Ordinal);
                return int.Parse(RecordsInfo.Substring(first, last - first).Trim());
            }
        }

        /// <summary>
        /// End page in records info part
        /// </summary>
        public int EndPage
        {
            get
            {
                int first = RecordsInfo.IndexOf("-", StringComparison.Ordinal) + 1;
                int last = RecordsInfo.IndexOf("of", StringComparison.Ordinal);
                return int.Parse(RecordsInfo.Substring(first, last - first).Trim());
            }
        }


        /// <summary>
        /// Selectd page number in page numbers part
        /// </summary>
        public int CurrentPage
        {
            get
            {
                int currentPage;
                SiteDriver.InitializePageElement(_paginationPageObject);
                int.TryParse(_paginationPageObject.PageCurrent.Text.Trim(), out currentPage);
                return currentPage;
            }
        }


        /// <summary>
        /// Number of pages in page numbers part 
        /// </summary>
        public IList<int> PageNumbers
        {
            get
            {
                var numParts = JavaScriptExecutor.FindElements(PaginationPage.NumPartXPath, How.XPath, "Text").Where(x => x != "...");
                return numParts.ToList().ConvertAll(Convert.ToInt32);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public PaginationService(object pageObject,ISiteDriver siteDriver,IJavaScriptExecutors javaScriptExecutor)
        {
            _pageObject = pageObject;
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutor;
            _paginationPageObject = new PaginationPage();
            SiteDriver.InitializePageElement(_paginationPageObject);
            _waitForGridToLoad = _pageObject.GetType().GetMethod(PaginationPage.WaitForGridToLoadMethod);

        }


        #endregion

        #region PUBLIC METHODS

        public void SelectPageSize(InputButton pageSizeComboArrow, SelectComboBox selectPageSize, int pageSize)
        {
            pageSizeComboArrow.Click();
            selectPageSize.SelectByText(pageSize.ToString());
            //wait is needed here direct click clicks on other page number in chrome
            //
            //SiteDriver.FindElement(string.Format(listPageSizeLocatorXPathTemplate, pageSize), How.XPath).Click();
            _waitForGridToLoad.Invoke(_pageObject, null);
            //_waitForGridToLoad.Invoke(_pageObject, null);
            Console.WriteLine("Selected Page Size : " + pageSize);
        }

        public int GetSelectedPageSize()
        {
            SiteDriver.InitializePageElement(_paginationPageObject);
            return int.Parse(_paginationPageObject.PageSize.GetAttribute("value"));
        }

        public bool CheckPageSize(int endPage, int pageSize, int totalRecords)
        {
            Console.WriteLine("Total No of Records: <" + totalRecords + ">");
            if (totalRecords < pageSize)
            {
                if (endPage <= pageSize)
                {
                    Console.Out.WriteLine("Expected Records Displayed: <Records 1 - number less than  " + pageSize + "> " + " Actual Records Displayed: <Records 1 - " + endPage + ">");
                    return true;
                }
            }
            else
            {
                if (endPage == pageSize)
                {
                    Console.Out.WriteLine("Expected Records Displayed: <Records 1 - " + pageSize + "> " + " Actual Records Displayed: <Records 1 - " + endPage + ">");
                    return true;
                }
            }
            return false;
        }

        public void ClickOnLastArrow()
        {
            SiteDriver.InitializePageElement(_paginationPageObject);
            _paginationPageObject.PageLast.Click();
            
            _waitForGridToLoad.Invoke(_pageObject, null);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
        }

        public void ClickOnFirstArrow()
        {
            SiteDriver.InitializePageElement(_paginationPageObject);
            _paginationPageObject.PageFirst.Click();
            _waitForGridToLoad.Invoke(_pageObject, null);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
        }

        public void ClickNextPage()
        {
            SiteDriver.InitializePageElement(_paginationPageObject);
            _paginationPageObject.PageNext.Click();
            _waitForGridToLoad.Invoke(_pageObject, null);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
        }

        public void ClickPreviousPage()
        {
            SiteDriver.InitializePageElement(_paginationPageObject);
            _paginationPageObject.PagePrevious.Click();
            _waitForGridToLoad.Invoke(_pageObject, null);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
        }

        /// <summary>
        /// Click on page number
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="continueLoop"></param>
        public PaginationService ClickOnPageNum(int currentPage, out bool continueLoop)
        {
            if (CurrentPage != currentPage)
            {
                SiteDriver.FindElement(String.Format(PaginationPage.NumPartTemplate, currentPage), How.XPath).
                    Click();
                Console.Out.WriteLine("Clicked on Page No. :{0}", currentPage);
                _waitForGridToLoad.Invoke(_pageObject, null);
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
                continueLoop = true;
            }
            else if (currentPage == 1)
            {
                Console.WriteLine("Is at Page No. :1");
                continueLoop = true;
            }
            else
            {
                continueLoop = false;
            }
            return new PaginationService(_pageObject,SiteDriver,JavaScriptExecutor);
        }

        public PaginationService ClickOnPageNum(int currentPage)
        {
            SiteDriver.FindElement(String.Format(PaginationPage.NumPartTemplate, currentPage), How.XPath).
                    Click();
            Console.Out.WriteLine("Clicked on Page No. :{0}", currentPage);
            _waitForGridToLoad.Invoke(_pageObject, null);
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(PaginationPage.RadAjaxCssLocator, How.CssSelector));
            return new PaginationService(_pageObject,SiteDriver,JavaScriptExecutor);
        }

        #endregion

        #region Private Methods

        #endregion

    }
}
