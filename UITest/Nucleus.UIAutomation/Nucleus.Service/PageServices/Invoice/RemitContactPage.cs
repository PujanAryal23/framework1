using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Invoice;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageServices.Appeal
{
    public class RemitContactPage : BasePageService
    {
        #region PRIVATE FIELDS

        private RemitContactPageObjects _remitContactPage;

        #endregion

        #region CONSTRUCTOR

        public RemitContactPage(INavigator navigator, RemitContactPageObjects remitContactPage)
            : base(navigator, remitContactPage)
        {
            _remitContactPage = (RemitContactPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS


        /// <summary>
        /// Close remit/contact popup page
        /// </summary>
        public InvoiceSearchPage CloseRemitContactPage()
        {
            var invoiceSearch = Navigator.Navigate<InvoiceSearchPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.InvoiceSearch.GetStringValue());
                
            });
            return new InvoiceSearchPage(Navigator, invoiceSearch);
        }


        /// <summary>
        /// Get Remit Div's text
        /// </summary>
        /// <returns></returns>
        public string GetRemitDivText()
        {
            return _remitContactPage.RemitDiv.Text;
        }

        /// <summary>
        /// Get Contact Div's text
        /// </summary>
        /// <returns></returns>
        public string GetContactDivText()
        {
            return _remitContactPage.ContactDiv.Text;
        }




        

        #endregion
    }
}
