using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.ChromeDownLoad
{
    public class ChromeDownLoadPageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        public const string FileLocationCssLocator = "::shadow iron-list ::shadow div div#title-area>a";
        public const string FileNameForEdgeCssLocator = "div[id*='downloads-item'] button[id*='open_file']";
        public const string FileLocationForEdgeCssLocator = "div[id*='downloads-item'] button[id*='open_link']";
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return "Downloads"; }
        }

        #endregion
        
        #region CONSTRUCTOR

        public ChromeDownLoadPageObjects()
            :base(string.Empty, @"chrome://downloads/")
        {

        }

        #endregion
    }
}
