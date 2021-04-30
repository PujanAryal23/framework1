using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;

namespace Nucleus.Service.PageObjects.Invoice
{
    public  class InvoiceDetailPageObjects: DefaultPageObjects
    {
        #region PRIVATE FIELD

        public const string IsRequestInProgress = "($telerik.radControls[0]._manager == null) ? false : $telerik.radControls[0]._manager._isRequestInProgress";
        private const string BackButtonId = "ctl00_MainContentPlaceHolder_lnkBtnBackNavigation";
        public const string InvoiceGridHeaderLabelXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00_Header']/thead/tr[1]/th[{0}]";
        public const string GridHeaderId = "ctl00_MainContentPlaceHolder_ResultsGrid_ctl00_Header";
        public const string InvoiceNoRowXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[{0}]/td[25]/a";

        
        public const string InvoiceDateRowXPathTemplate = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[{0}]/td[24]";
        private const string InvoiceDateLabelId = "ctl00_MainContentPlaceHolder_lblDate";
        public const string AllClaimSequenceXPath = "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr/td[1]/a";
        public const string GroupIdLabelXpath = "//*[@id='ctl00_MainContentPlaceHolder_liGroupID']/label[1]";
        public const string GroupIdValueXpath = "//*[@id='ctl00_MainContentPlaceHolder_lblGoupID']";
        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = BackButtonId)]
        public Link BackButton;

        [FindsBy(How = How.Id, Using = InvoiceDateLabelId)]
        public TextLabel InvoiceDateLabel;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.InvoiceDetail.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public InvoiceDetailPageObjects()
            : base(PageUrlEnum.InvoiceDetail.GetStringValue())
        {
        }

        #endregion
    }
}
