
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Dashboard
{
    public class DashboardLogicRequestsDetailPageObjects : NewPageBase
    {
        #region FIELDS
        public const string LogicRequestsDetailPageHeaderCss = "label.page_title";
        public const string LogicRequestDetailGridHeaderCss = "span.component_title.widget_title";
        public const string ClientsCountCssLocator = "div.analytics_details.claim_summary_analytics_details>ul>li";

        public const string OverdueRequestsHeaderCss =
            "div.analytics_detail_header.widget_data > ul >li";
        public const string DueIn30MinsHeaderCss =
            "div.analytics_detail_header.widget_data > ul >li:nth-child(2)";
        public const string OpenRequestsHeaderCss =
           "div.analytics_detail_header.widget_data > ul >li:nth-child(3)";

        public const string GridLogicRequestCountValueTemplateCssTemplate = "li.analytics_widget.logics.analytics_detail_widget ul>li:nth-of-type({0})>li:nth-of-type({1})>span";
        
        public const string DashboardIconXPath = "//a[text()='Dashboard']";

        public const string ActiveClientCssLocator = "div.analytics_details.claim_summary_analytics_details>ul>li>li:nth-of-type(1)>span";
        
        
        public const string BoxWithArrowIconXPath = "//span[text()='Print Details']";

        public const string DownloadPDFLinkXPath =
            "//header[.//span[text()='Logic Requests By Client']]//span[contains(@class,'download')]";
        public const string SpinnerWrapperCssLocator = "div.spinner_wrap.large_spineer";

        #endregion

        #region PAGEOBJECT PROPERTIES

        #region ID

        #endregion

        #region XPATH

        
        

        #endregion

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.DashboardLogicRequestDetail.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region CONSTRUCTOR

        public DashboardLogicRequestsDetailPageObjects()
            : base(PageUrlEnum.LogicRequestsDetail.GetStringValue())
        {
        }

        public DashboardLogicRequestsDetailPageObjects(string url)
            : base(url)
        {
        }

        #endregion
    }
}
