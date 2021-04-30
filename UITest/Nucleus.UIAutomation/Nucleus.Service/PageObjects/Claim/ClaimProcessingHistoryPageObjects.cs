using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class ClaimProcessingHistoryPageObjects:NewPageBase
    {
        public ClaimProcessingHistoryPageObjects()
            : base(PageUrlEnum.ClaimProcessingHistory.GetStringValue())
        {
        }

        public override string PageTitle
        {
            get { return PageTitleEnum.ClaimProcessingHistoryPage.GetStringValue(); }
        }
        #region Public pageobject
       
        public const string ProcessingHistoryLabeByCss = "div[id='processingHistoryNav']>div>div>label.PopupPageTitle";
        public const string ClaimHistorylabelByCss = "div[id='claimHistoryNav']>div>div>label.PopupPageTitle";
        public const string processinghistoryHeader = "div[id='processingHistoryData']>table>thead>tr>th";
        public const string ClaimHistoryHeader = "div#claimHistoryData>table>thead>tr>th";
        public const string ClaimHistoryRowCountCssLocator = "div#body_view>table.claim_history_table>tbody>tr";
        public const string ProcessingHistoryDataByCSS = " div[id='processingHistoryData']>table>tbody>tr>td";
        public const string ClaimHistoryDataByCss = " div[id='claimHistoryData']>div>table>tbody>tr>td:nth-of-type({0})";

        public const string ClaimHistoryTableCssTemplate =
            " div[id='claimHistoryData']>div>table>tbody>tr:nth-of-type({0})>td:nth-of-type({1})";

        public const string ReviewTimeBycss =
            "table.claim_history_table> tbody>tr>td:Contains(Open)~td:nth-of-type(9)";

        #endregion
    }
}
