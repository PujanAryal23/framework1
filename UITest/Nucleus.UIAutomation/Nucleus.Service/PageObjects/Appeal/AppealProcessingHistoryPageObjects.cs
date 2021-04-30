using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealProcessingHistoryPageObjects : NewPageBase
    {

        #region Private Fields

        public const string PageHeaderCssLocator = "label.PageTitle";


        #region TEMPLATES

        public const string AppealAuditGridTableCssTemplate = "table tbody tr:nth-of-type({0}) td:nth-of-type({1})";
        //public const string AppealAuditGridTableColumnCssTemplate = "table tbody td:nth-of-type({0})";
        public const string TblGridHeaderId = "column_12";
        public const string AppealAuditGridTableColumnXPath = "//table/tbody//td[6]";
        public const string ModifiedByValueByAction = "//td/span[text()='QADone']/../../td[2]/span";
        #endregion


        #region CLASS

        public const string AppealAuditGridCountCssLocator = "table tbody tr";

        #endregion

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealProcessingHistory.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public AppealProcessingHistoryPageObjects()
            : base(PageUrlEnum.AppealProcessingHistory.GetStringValue())
        {
        }

        public AppealProcessingHistoryPageObjects(string url)
            : base(url)
        {
        }

        #endregion
    }
}
