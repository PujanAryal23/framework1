using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.PreAuthorization
{
    public class PreAuthProcessingHistoryPageObjects : NewPageBase
    {
        #region Private Fields

        public const string PageHeaderCssLocator = "span.title";

        #endregion
        #region CSS

        public const string PreAuthHistoryRowCountCssLocator = "tbody.preauth_content>tr";
        public const string PreAuthHistoryColumnHeadersCssLocator = "thead.preauth_header>tr>th";

        public const string PreAuthHistoryGridValueByRowColCssLocator =
            "tbody.preauth_content>tr:nth-of-type({0})>td:nth-of-type({1})";
        public const string ListValueInGridByColumnCssTemplate = "tbody.preauth_content>tr>td:nth-of-type({0})";

        public const string PreAuthHistoryAuthSeqCssLocator = "span.title+strong";

        #endregion


        #region CONSTRUCTOR

        public PreAuthProcessingHistoryPageObjects()
            : base(PageUrlEnum.PreAuthProcessingHistory.GetStringValue())
        {
        }

        #endregion
    }
}
