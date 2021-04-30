using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Invoicing
{
   public class InvoicingPageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        public InvoicingPageObjects()
            : base("Invoicing/Invoicing.aspx")
        {
        }
    }
    
}
