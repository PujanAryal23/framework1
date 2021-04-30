using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageObjects.Invoice;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Common;

namespace Nucleus.Service.PageServices.Invoice
{
    public class InvoiceDetailPage: DefaultPage
    {
        #region PRIVATE FIELDS

        private InvoiceDetailPageObjects _invoiceDetailPage;
        private readonly StandardGridFunctionalityService _standardGridFunctionality;

        #endregion
        
        #region CONSTRUCTOR

        public InvoiceDetailPage(INavigator navigator, InvoiceDetailPageObjects invoiceDetailPage)
            : base(navigator, invoiceDetailPage)
        {
            _invoiceDetailPage = (InvoiceDetailPageObjects)PageObject;
            _standardGridFunctionality = new StandardGridFunctionalityService();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Rest grid column 
        /// </summary>
        public void GridColumnReset()
        {
            _standardGridFunctionality.ResetColumns();
            WaitForGridToLoad();
        }

        /// <summary>
        /// Wait for grid to load completely (ajax request handler)
        /// </summary>
        public void WaitForGridToLoad()
        {
            SiteDriver.WaitForAjaxToLoad(InvoiceDetailPageObjects.IsRequestInProgress);
        }

        public InvoiceDetailPage ClickOnInvoiceNumber(string rowIndex)
        {
            var invoiceDetailPopup = Navigator.Navigate<InvoiceDetailPageObjects>(() =>
                                              {
                                                  string pagetitle = PageTitleEnum.InvoiceDetail.GetStringValue();
                                                  SiteDriver.FindElement<Link>(string.Format(InvoiceDetailPageObjects.InvoiceNoRowXPathTemplate, rowIndex), How.XPath).Click();
                                                  SiteDriver.SwitchWindow("NucleusPopup");
                                                  SiteDriver.WaitForCondition(() => SiteDriver.Title == pagetitle);
                                                  Console.WriteLine("Clicked on Invoice No of row '{0}'.", rowIndex);
                                              });
            return new InvoiceDetailPage(Navigator, invoiceDetailPopup);
        }

        public void ScrollToLastOfHeader(string colIndex)
        {
            JavaScriptExecutor.ExecuteToScrollToView(InvoiceDetailPageObjects.GridHeaderId, colIndex);
        }

        public List<string> GetAllClaimSequenceFromDataGrid()
        {
            return SiteDriver.FindElements(InvoiceDetailPageObjects.AllClaimSequenceXPath, How.XPath, "Text");
        }

        public void CloseInvoiceDetailPopup()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.InvoiceDetail.GetStringValue());
            
        }

        public string GetInvoiceDateLabelText()
        {
            return _invoiceDetailPage.InvoiceDateLabel.Text;
        }

        public string GetValueOfInvoiceDateForRow(string rowIndex)
        {
            return SiteDriver.FindElement<Link>(string.Format(InvoiceDetailPageObjects.InvoiceDateRowXPathTemplate, rowIndex), How.XPath).Text;
        }

        public string GetHeaderLabelAtIndex(string index)
        {
            ScrollToLastOfHeader(index);
            return SiteDriver.FindElement<TextLabel>(string.Format(InvoiceDetailPageObjects.InvoiceGridHeaderLabelXPathTemplate, index), How.XPath).Text;
        }

        public InvoiceSearchPage ClickOnBackButton()
        {
            var invoiceSearch = Navigator.Navigate<InvoiceSearchPageObjects>(() =>
            {

                JavaScriptExecutor.ExecuteClick("ctl00_MainContentPlaceHolder_lnkBtnBackNavigation",How.Id);
               // _invoiceDetailPage.BackButton.Click();
                Console.Out.WriteLine("Click on Back Link");
                
            });
            return new InvoiceSearchPage(Navigator, invoiceSearch);
        }

        public bool GroupIDLabelOrValueIsPresent()
        {
            return SiteDriver.IsElementPresent(
                 string.Format(InvoiceDetailPageObjects.GroupIdLabelXpath), How.XPath) || SiteDriver.IsElementPresent(
                 string.Format(InvoiceDetailPageObjects.GroupIdValueXpath), How.XPath);
        }

        #endregion
    }
}
