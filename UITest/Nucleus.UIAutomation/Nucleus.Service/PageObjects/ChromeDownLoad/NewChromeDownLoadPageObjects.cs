using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.ChromeDownLoad
{
    public class NewChromeDownLoadPageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        public const string FileLocationCssLocator = "::shadow iron-list ::shadow div div#title-area>a";
        

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return "Downloads"; }
        }

        #endregion
        
        #region CONSTRUCTOR

        public NewChromeDownLoadPageObjects()
            :base(string.Empty, @"chrome://downloads/")
        {

        }

        #endregion
    }
}
