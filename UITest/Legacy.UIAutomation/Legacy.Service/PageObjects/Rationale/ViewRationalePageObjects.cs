using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;

namespace Legacy.Service.PageObjects.Rationale
{
    public class ViewRationalePageObjects : PageBase
    {
        #region PUBLIC PROPERTIES

        public const string RuleDescId = "lblRuleDescription";

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ViewRationale.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ViewRationalePageObjects()
            : base(PageUrlEnum.ViewRationale.GetStringValue())
        {

        }

        #endregion
    }
}
