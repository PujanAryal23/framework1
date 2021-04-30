using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class PopupCodePageObjects : NewPageBase
    {
        #region PROPERTIES

        public const string ToolTipContentTemplate = ".//div[@class='tooltip_content']/ul/li[{0}]{1}";
        public const string PopupHeaderTextXPath = "//label[@class='PopupPageTitle']";
        
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

        public PopupCodePageObjects()
            : base(PageUrlEnum.PopupCode.GetStringValue())
        {
        }

        #endregion
    }
}
