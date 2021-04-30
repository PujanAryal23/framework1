using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.Service.PageServices.Claim
{
    public class InvoiceDataPage : NewDefaultPage
    {

        private readonly InvoiceDataPageObjects _invoiceDataPageObjects;
      
        public InvoiceDataPage(INewNavigator navigator,
            InvoiceDataPageObjects _invoiceDataPageObjects, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, _invoiceDataPageObjects, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _invoiceDataPageObjects = (InvoiceDataPageObjects) PageObject;
        }

        public List<string> GetInvoiceheaderlabel()
        {
            return SiteDriver.FindDisplayedElementsText(InvoiceDataPageObjects.InvoiceDataHeaderLabelByXpath,
                How.XPath);
        }

        public string GetInvoiceHeaderValueBylabel(string label)
        {
            return SiteDriver.FindElement(Format(InvoiceDataPageObjects.InvoiceDataHeadervalueBylabelXpath, label),
                How.XPath).Text;
        }

        public string GetGroupValue()
        {
            return SiteDriver.FindElement(InvoiceDataPageObjects.GroupLabelByXpath,How.XPath).Text;
        }

        public List<string> GetInvoiceLabel()
        {
            return SiteDriver.FindDisplayedElementsText(InvoiceDataPageObjects.InvoiceLabelXpath,
                How.XPath);
        }


        public string GetInvoiceValueBylabel(string label)
        {
            return SiteDriver.FindElement(Format(InvoiceDataPageObjects.InvoiceValueByLabelXpath, label),
                How.XPath).Text;
        }

        public string GetInvoiceheaderBylabel(string label)
        {
            return SiteDriver.FindElement(Format(InvoiceDataPageObjects.InvoiceHeaderValueByXpath, label),
                How.XPath).Text;
           
        }

        public List<string> GetInvoiceGridHeader()
        {
            return SiteDriver.FindDisplayedElementsText(InvoiceDataPageObjects.TableHeaderByXpath,
                How.XPath);
        }

        public List<string> GetInvoiceGridProducts()
        {
            return SiteDriver.FindDisplayedElementsText(InvoiceDataPageObjects.InvoiceProductValue,
                How.XPath);
        }

        public List<string> GetInvoiceGridValueByProduct(string label)
        {
            return SiteDriver.FindDisplayedElementsText(Format(InvoiceDataPageObjects.InvoiceDetailsByProductXpath, label),
                How.XPath);
        }

        public List<string> GetProductSpecificInvoiceDataFromDb(string product,string claseq)
        {
            var productList=Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.ProductSpecificInvoiceData, product, claseq));

            return productList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList()[0];
        }

        public List<string> GetGeneralInvoiceDataFromDb(string claseq)
        {
            var productList = Executor.GetCompleteTable(Format(ClaimSqlScriptObjects.InvoiceDateFromDb, claseq));

            return productList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList()[0];
        }

        public ClaimActionPage CloseInvoiceDataPageAndBackToClaimActionPage()
        {
            var claimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealSearch));
            });
            return new ClaimActionPage(Navigator, claimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
    }

}
