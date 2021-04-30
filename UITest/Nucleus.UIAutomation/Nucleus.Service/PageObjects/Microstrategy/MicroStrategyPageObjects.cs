using Nucleus.Service.PageObjects.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.MicroStrategy 
{
    public class MicroStrategyPageObjects: NewDefaultPageObjects
    {
        public const string LodingSpinnerIconCssSelectyor = "div.mstr-LoadingIcon";//mstr-LoadingIcon
        public const string DashboardLabelCssSelector = "label.page_title";
        public const string ReportLocatorCss = ".table tr th:nth-of-type(1)";
        public const string SNoLocatorCss = ".table tr th:nth-of-type(2)";
        
        public const string ReportValuesLocatorCssLocator = ".table tr td:nth-of-type({0})";
        public const string PageHeadersByCssLocator = ".filter-list li:nth-of-type('{0}') a";
        public const string ViewReportsByXpath = "//tr/td/a";
        public const string pageheaderlocatorBycss = ".table tr th:nth-of-type({0})";//
        public const string ReportLinkByCss = "ul div:nth-of-type(3)>ul li span a";

        public const string MicrostrategyDashboardXpath =
            "//ul[contains(@id,'dashboard_products')]//a[text()='Microstrategy']";

        public const string MicrostratergyNoDataavailableByCss = "section p.empty_message ";

        #region CONSTRUCTOR

        public MicroStrategyPageObjects()
            : base(PageUrlEnum.Microstrategy.GetStringValue())
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
