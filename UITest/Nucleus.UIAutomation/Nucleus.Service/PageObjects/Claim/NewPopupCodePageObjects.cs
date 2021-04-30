using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class NewPopupCodePageObjects:NewPageBase
    {
         #region PROPERTIES

        public const string ToolTipContentTemplate = "div.content>div>ul>li:nth-of-type({0}){1}";
        public const string PopupHeaderTextCssSelector = "label.title";
        public const string PopUpDescriptionXPath = "//li[3]";
        
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

        public NewPopupCodePageObjects()
            : base(PageUrlEnum.NewPopupCode.GetStringValue())
        {
        }

        #endregion



    }
}
