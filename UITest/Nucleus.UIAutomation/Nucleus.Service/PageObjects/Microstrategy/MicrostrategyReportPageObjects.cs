using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using Nucleus.Service.PageObjects.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.PageObjects.Microstrategy
{
    public class MicrostrategyReportPageObjects : NewDefaultPageObjects
    {
        public const string PageHeadersByCssLocator = ".filter-list li:nth-of-type('{0}') a";
        public const string ReporthomeByXpath = "//div/div/ul/li/a";
        public const string SelectProductBycss = "div.component_header_right span";
        public const string MicrostrategyDashboardCssLocator = "ul#dashboard_products>li>a:contains(Microstrategy)";
        public const string DashboardLabelCssSelector = "label.page_title";
        public const string MicrostrategyProduct = "ul.option_list >li:last-child>a";

        public const string IframeId = "myReportIframe";
        public const string HomeButtonCssLocator = "button#homePage";








        #region CONSTRUCTOR

        public MicrostrategyReportPageObjects()
            : base(PageUrlEnum.MicrostrategyHome.GetStringValue())
        {
        }

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.Microstrategy.GetStringValue(); }
        }


        #endregion


    }
}

