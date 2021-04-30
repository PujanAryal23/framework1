using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class InvoiceDataPageObjects: NewDefaultPageObjects
    {
        public InvoiceDataPageObjects()
            : base(PageUrlEnum.InvoiceData.GetStringValue())
        {
        }

        public override string PageTitle
        {
            get { return PageTitleEnum.InvoiceData.GetStringValue(); }
        }
        #region Public pageobject

        public const string InvoiceDataHeaderLabelByXpath = "//header/div/div/label";
        public const string GroupLabelByXpath="//header//label";

        public const string InvoiceDataHeadervalueBylabelXpath =
            "//header //label[text()='{0}']/following-sibling::div";
        public const string InvoiceLabelXpath = "//div[contains(@class,'invoice_content_block')][1] //div[contains(@class,'row')]//label";
        public const string InvoiceValueByLabelXpath = "//div[contains(@class,'invoice_content_block')][1] //div[contains(@class,'row')]//label[text()='{0}']/following-sibling::div";

        public const string InvoiceHeaderValueByXpath =
            "//div[contains(@class,'data_point')]//label[text()='{0}']/following-sibling::div";
        public const string TableHeaderByXpath = "//div[1]/table//tr/th";
        public const string InvoiceProductValue = "//div[1]/table/tbody /tr/td[contains(@class,'left_align')]";

        public const string InvoiceDetailsByProductXpath =
            "//div[1]/table/tbody /tr/ td[text()='{0}']/following-sibling::td";




        #endregion
    }
}
