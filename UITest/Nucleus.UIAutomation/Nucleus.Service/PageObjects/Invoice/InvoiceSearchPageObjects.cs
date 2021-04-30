using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Invoice
{
    public class InvoiceSearchPageObjects : NewDefaultPageObjects
    {
        #region PAGEOBJECTPROPERTIES
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string FindButtonCssLocator = "button.work_button";
        public const string ExportIconTitleXpathSelector = "//li[contains(@title,'Export')]";
        public const string ExportIconXpathSelector = "//li[contains(@title,'Export')]/span";
        public const string ExportOptionListXpath = "//li[contains(@title,'Export')]/ul/li/span";
        public const string ExportOptionXpathTemplate = "//li[contains(@title,'Export')]/ul/li[{0}]/span";
        public const string SearchListByInvocieNumberSelectorTemplate = "//section/ul//span[text()= '{0}']/../..";
        public const string InvoiceDetailsHeaderCssSector = "div:nth-of-type(1)>section.column_40 div.component_header_left>label";
        public const string RemitContactHeaderCssSector = "div:nth-of-type(2)>section.column_40 div.component_header_left>label";
        public const string InvoiceDetailsValueByLabelCssLocator = "li:has(>label:contains({0}))>span";
        public const string SelectedInvoiceSearchResultRowXpath = "//section/ul//span[text()= '{0}']/../..";
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string ClaimQuestionsXpath = "//label[text()='Remit/Contact']/../../../section[2]/div/ul[1]";
        public const string InvoicingQuestionsXpath = "//label[text()='Remit/Contact']/../../../section[2]/div/ul[2]";
        public const string VersendTechXpath = "//label[text()='Remit/Contact']/../../../section[2]/div/ul[3]";

        #endregion
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.InvoiceSearch.GetStringValue(); }
        }

        #endregion  
        #region CONSTRUCTOR
        public InvoiceSearchPageObjects()
            : base(PageUrlEnum.NewInvoiceSearch.GetStringValue())
        {
        }

        #endregion

    }
}
