using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Claim
{
   public class FlagPopupPageObjects:NewPageBase
    {
         #region PROPERTIES

        public const string ToolTipContentTemplate = "div.content>div>ul>li:nth-of-type({0}){1}";
        public const string PopupHeaderTextCssSelector = "label.title";
        
        #endregion

       

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return AssignPageTitle; }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        public static string AssignPageTitle;
        #endregion

        #region CONSTRUCTOR

        public FlagPopupPageObjects()
            : base(PageUrlEnum.FlagPopupCode.GetStringValue())
        {
        }

        #endregion

    }
}
