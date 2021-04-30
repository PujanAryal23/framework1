using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.Service.PageObjects.Dashboard
{
    public class COBClaimsDetailpageObject : NewDefaultPageObjects
    {
        public COBClaimsDetailpageObject()
            : base(PageUrlEnum.COBClaimsDetail.GetStringValue())
        {
        }
        
        public COBClaimsDetailpageObject(string url)
            : base(url)
        {
        }

       

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.DashboardClaimsDetail.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region FIELDS
        public const string COBClaimsDetailPageheaderXPath = "//div[@id='analytics_header']/div/label";
        public const string DashboardWidgetByTitle = ".analytics_widget.cob div.component_header_left:contains({0})";
        public const string ExportIconInDashboardWidget = ".analytics_widget.cob div:contains({0})~div.component_header_right .print_button";

        public const string CollapseIconDashboardWidget =
            ".analytics_widget.cob div:contains({0})~div.component_header_right span.collapse";

        public const string DashboardTypeInWidgetByName =
            ".analytics_widget.cob div:contains({0})~div.component_header_right .widget_data_type";

        public const string AssignedClientsInDashboard =
            "header:has(div:contains({0})) ~ div.component_content tbody tr td:nth-of-type(1)";

        public const string LastFiveDaysByCSS =
            "header:has(div:contains({0})) ~ div.component_content thead td:not(.cob_detail_client)";

        public const string ClaimsCountByDay =
            "header:has(div:contains({0})) ~ div.component_content tbody tr td:nth-of-type({1})";

        public const string ColumnNamesByCssSelector =
            "header:has(div:contains({0})) ~ div.component_content thead td";

        public const string UnreviewedClaimsCountByCssSelector =
            "header:has(div:contains({0})) ~ div.component_content tbody tr:nth-of-type({1}) td:nth-of-type(2)";

        public const string ClientNameByRowCssSelector =
            "header:has(div:contains({0})) ~ div.component_content tbody tr:nth-of-type({1}) td:nth-of-type(1)";

        public const string CobClaimsDetailDataByWidgetAndColCssSelectorTemplate =
            "header:has(div:contains({0})) ~div tr.total_row td:nth-of-type({1}),header:has(div:contains({0})) ~div tbody>tr td:nth-of-type({1})";

        public const string TotalClaimByClientCssSelectorTemplate =
            "header:has(div:contains({0})) ~div>table>tbody>tr:has(td:contains({1}))>td";

        public const string TotalPaidClaimsDetailCssSelector = "li.by_flag table>tbody>tr:not([class=total_row])>td:nth-of-type(4)";

        public const string TotalOfTotalPaidClaimsDetailCssSelector = "li.by_flag table>tbody>tr.total_row>td:nth-of-type(4)";

        #endregion
    }
}
