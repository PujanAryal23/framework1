using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.Support.Common
{
    public class StandardGridFunctionalityPage
    {
        public const string GridHeaderXPathIE = "//table[@class='rgMasterTable rgClipCells']/thead/tr";

        public const string GridHeaderXPathChrome = "//table[@class='rgMasterTable rgClipCells']/thead/tr/th[5]";

        public const string GridHeaderListCss = "th.rgHeader";

        public const string SaveMenuXPath = ".//span[text()='Save Columns']";

        public const string ResetMenuXPath = "//span[text()='Reset Columns']";

        public const string GridColumnsXPath = ".//ul[@class='rmActive rmVertical rmGroup rmLevel1']/li/a/span[text()='Columns']";

        public const string ColumnToAddTemplate = ".//div[@class='rmScrollWrap rmGroup rmLevel0']/ul/li/div/span/label[text()='{0}']";

        [FindsBy(How = How.XPath, Using = ResetMenuXPath)] 
        public TextField ResetMenuOption;



    }
}
