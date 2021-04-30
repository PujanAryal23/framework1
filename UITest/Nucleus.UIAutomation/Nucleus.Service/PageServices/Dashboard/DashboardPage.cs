using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageObjects.MicroStrategy;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Microstrategy;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Dashboard;
using OpenQA.Selenium.Interactions;
using Nucleus.Service.PageObjects.Microstrategy;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using static System.String;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class DashboardPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private DashboardPageObjects _dashboardPage;
        private readonly SwitchClientSection _switchClientSection;
        
        public class UserKpi
        {
            public string LegacyClaimsReviewed { get; set; }
            public string NucleusClaimsReviewed { get; set; }
            public string TotalClaimsReviewed { get; set; }
            public string LegacyAvgClaimsPerHour { get; set; }
            public string NucleusAvgClaimsPerHour { get; set; }
            public string TotalAvgClaimsPerHour { get; set; }
            public string LegacyAppealsReviewed { get; set; }
            public string NucleusAppealsReviewed { get; set; }
            public string AvgAppealsPerHour { get; set; }
            public string TotalAppealsCompleted { get; set; }
            public string WeightedAppealsCompleted { get; set; }
            public string WeightedClaimsCompleted { get; set; }
            public DateTime LastUpdated { get; set; }
        }


        #endregion

        #region PUBLIC PROPERTIES

        public new string PageTitle { get { return _dashboardPage.PageTitle; } }

        public SwitchClientSection SwitchClientSection
        {
            get { return _switchClientSection; }
        }
        #endregion

        #region CONSTRUCTOR

        public DashboardPage(INewNavigator navigator, DashboardPageObjects dashboardPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, dashboardPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _dashboardPage = (DashboardPageObjects)PageObject;
            _switchClientSection = new SwitchClientSection(SiteDriver,JavaScriptExecutor);
        }

        #endregion

        #region PUBLIC METHODS

        public bool IsDashboardPageOpened()
        {
            if (_dashboardPage.PageTitle.Equals(CurrentPageTitle))
                return true;
            return false;
        }

        public bool IsAppealOverdueIconPresent() =>
            SiteDriver.IsElementPresent(DashboardPageObjects.AppealOverdueIconCssSelector, How.CssSelector);

        public bool IsContainerHeaderClaimsOverviewPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ClaimsOverviewXPath, How.XPath);
        }

        public bool IsUnapprovedClaimsWidgetPresent()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DashboardPageObjects.UnapprovedClaimsOverviewWidgetXPath, How.XPath));
            return SiteDriver.IsElementPresent(DashboardPageObjects.UnapprovedClaimsOverviewWidgetXPath, How.XPath);
        }

        public bool IsLogicRequestOverviewDivPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.LogicRequestOverviewDivCss, How.CssSelector);
        }

        public bool IsLogicRequestOverdueGraphPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.LogicRequestHighChart1Css, How.CssSelector);
        }

        public bool IsLogicRequestDueIn30MinutesGraphPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.LogicRequestHighChart2Css, How.CssSelector);
        }

        public bool IsLogicRequestCurrentlyOpenGraphPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.LogicRequestHighChart3Css, How.CssSelector);
        }

        public string GetLogicRequestOverviewDivHeaderText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestOverviewDivHeaderCss, How.CssSelector).Text;
        }

        public string GetUnapprovedClaimsDivHeaderText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.UnapprovedClaimsDivHeaderCss, How.CssSelector).Text;
        }

        public IList<string> GetUnapprovedClaimsList()

        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.UnapprovedClaimsGridClmSequenceListCss, How.XPath, "Text");

        }
        public IList<string> GetUnapprovedAltClaimNumberList()

        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.UnapprovedClaimsGridAltClaimNumberListCss, How.XPath, "Text");

        }
        public IList<string> GetUnapprovedClaimsClientCodeList()
        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.UnapprovedClaimsGridClientCodeListCss, How.XPath, "Text");

        }

        public string GetUnapprovedClaimsProductText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.UnapprovedClaimsProductLabelCss, How.CssSelector).Text;
        }

        public string GetTotalUnapprovedClaimsCount()
        {
            var loadMoreText = SiteDriver.FindElement(DashboardPageObjects.UnapprovedClaimsLoadMoreCssSelector, How.CssSelector).Text.Split(' ');
            return loadMoreText[3];
        }


        public int GetTotalDisplayedUnapprovedClaimsCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.UnapprovedClaimsGridDataCss, How.CssSelector) - 1;
        }

        public void ClickOnLoadMoreFFPUnapprovedClaims()
        {
            JavaScriptExecutor.ExecuteClick(DashboardPageObjects.UnapprovedClaimsLoadMoreXpath, How.XPath);
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Clicked on Load More of Unapproved Claims widget");
        }

        public bool IsLoadMorePresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.UnapprovedClaimsLoadMoreXpath, How.XPath);
        }
        public void ClickOnUnapprovedClaimsRefreshIcon()
        {
            SiteDriver.FindElement(DashboardPageObjects.UnapprovedClaimsRefreshIconCss, How.XPath).Click();
            Console.WriteLine("Clicked on Unapproved Claims Refresh icon");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForPageToLoad();

        }

        public bool IsWidgetLoadingIconPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.waitForWidgetLoadingCSSLocator, How.CssSelector);
        }
        public void WaitForDashboardWidgetLoading()
        {

            SiteDriver.WaitForCondition(() => !IsWidgetLoadingIconPresent());
            SiteDriver.WaitForPageToLoad();

        }


        public bool IsExpandIconPresentInUnapprovedClaimsDiv()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.UnapprovedClaimsExpandIconCss, How.CssSelector);
        }

        public bool IsExpandElementPresentInOverViewWidgetByWidgetTitle(string widgetTitle) => JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate, widgetTitle));

        public string GetLogicRequestOverdueGraphYAxisLabel()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchartYAxisLabelCss, How.CssSelector).Text;
        }

        public string GetLogicRequestOverdueGraphYAxisText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchartYAxisTitleCss, How.CssSelector).Text;
        }

        public string GetLogicRequestOverdueGraphValue()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchartValueCss, How.CssSelector).Text;
        }

        public string GetLogicRequestDueIn30MinsGraphYAxisText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart2YAxisTitleCss, How.CssSelector).Text;
        }

        public string GetLogicRequestDueIn30MinsGraphYAxisLabel()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart2YAxisLabelCss, How.CssSelector).Text;
        }
        public string GetLogicRequestDueIn30MinsGraphValue()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart2ValueCss, How.CssSelector).Text;
        }
        public string GetLogicRequestCurrentlyOpenGraphYAxisText()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart3YAxisTitleCss, How.CssSelector).Text;
        }

        public string GetLogicRequestCurrentlyOpenGraphYAxisLabel()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart3YAxisLabelCss, How.CssSelector).Text;
        }

        public string GetLogicRequestCurrentlyOpenGraphValue()
        {
            return SiteDriver.FindElement(DashboardPageObjects.LogicRequestHighchart3ValueCss, How.CssSelector).Text;
        }

        public string GetAppealsOverviewDivHeader()
        {
            return SiteDriver.FindElement(DashboardPageObjects.AppealsOVerviewDivHeaderXPath, How.XPath).Text;
        }

        public bool IsAppealsOverviewHeaderPresent() =>
            SiteDriver.IsElementPresent(DashboardPageObjects.AppealsOVerviewDivHeaderXPath, How.XPath);

        public bool IsContainerHeaderClaimsOverviewPCIPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ContainerHeaderClaimsOverviewPHIXPath, How.XPath);
        }

        public bool IsContainerHeaderClaimsOverviewFFPPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ContainerHeaderClaimsOverviewFFPXPath, How.XPath);
        }
        public bool IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(string title) => JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.ContainerHeaderWidgetOverviewCOBTemplate, title));

        public bool IsContainerHeaderContainsRefreshIcon()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ClaimsOverviewRefreshIconXPath, How.XPath);
        }
        public bool IsContainerHeaderAppealsOverviewPCIPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ContainerHeaderAppealsOverviewPHIXPath, How.XPath);
        }

        public bool IsLastUpdatedTimeAppearsInLowerRightCorner()
        {

            return SiteDriver.IsElementPresent(DashboardPageObjects.LastUpdatedTimeXPath, How.XPath);
        }

        public string GetLastUpdatedTimeInLowerRightCorner()
        {
            String lastUpdated = SiteDriver.FindElement(DashboardPageObjects.LastUpdatedTimeXPath, How.XPath).Text;
            Console.WriteLine(lastUpdated);
            return lastUpdated;
        }

        public List<String> GetClaimsOverveiwDataLabels()
        {
            var claimsOverveiwDataLabels = new List<String>();
            //for (int i = 1; i <= GetDataLabelCount(); i++)
            //{
            //    claimsOverveiwDataLabels.Add(GetDataLabel(i));
            //}
            //for (int i = 1; i <= GetSubDataLabelCount(); i++)
            //{
            //    claimsOverveiwDataLabels.Add(GetSubDataLabel(i));
            //}

            claimsOverveiwDataLabels = JavaScriptExecutor.FindElements(Format(
                    DashboardPageObjects.MainLabelBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath, "Text").Select(x => x.Split('\r')[0]).ToList();
            claimsOverveiwDataLabels.AddRange(JavaScriptExecutor.FindElements(Format(
                DashboardPageObjects.SubLabelBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath, "Text"));
            claimsOverveiwDataLabels.AddRange(JavaScriptExecutor.FindElements(Format(
                DashboardPageObjects.SecondaryValueBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath, "Text").Select(x => x.Split(':')[0]));

            return claimsOverveiwDataLabels.Where(s => !IsNullOrEmpty(s)).ToList();
        }

        public List<String> GetClaimsOverveiwDataLabelsToolTip()
        {
            var claimsOverveiwDataLabels = new List<String>();
            //for (int i = 1; i <= GetDataLabelCount(); i++)
            //{
            //    claimsOverveiwDataLabels.Add(GetDataLabelToolTip(i));
            //}
            //for (int i = 1; i <= GetSubDataLabelCount(); i++)
            //{
            //    claimsOverveiwDataLabels.Add(GetSubDataLabelToolTip(i));
            //}
            claimsOverveiwDataLabels = SiteDriver.FindElementsAndGetAttribute("title", Format(
                DashboardPageObjects.MainLabelBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath);
            //claimsOverveiwDataLabels.AddRange(SiteDriver.FindElementsAndGetAttribute("title", Format(
            //    DashboardPageObjects.SubLabelBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath));
            claimsOverveiwDataLabels.AddRange(SiteDriver.FindElementsAndGetAttribute("title", Format(
                DashboardPageObjects.SecondaryValueBySectionHeaderXPathTemplate, "Claims Overview"), How.XPath));


            return claimsOverveiwDataLabels;
        }



        public string GetClaimOverviewMainValueToolTip(int col)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.ClaimOverviewMainValueCssTemplate, col), How.CssSelector).GetAttribute("title");
        }

        public string GetClaimOverviewSecondaryValueToolTip(int col)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.ClaimOverviewSecondaryValueCssTemplate, col), How.CssSelector).GetAttribute("title");
        }

        public List<string> GetClaimOverViewWidgetLabelForCOBByComponentTitle(string componentTitle) =>
            JavaScriptExecutor.FindElements(Format(DashboardPageObjects.COBDashboardWidgetHeaderTemplate, componentTitle));


        public List<string> GetClaimOverViewWidgetToolTipForCOBByComponentTitle(string componentTitle) => JavaScriptExecutor.FindElements(Format(DashboardPageObjects.COBDashboardWidgetHeaderTemplate, componentTitle), "title", true);

        public string GetDisplayedProductDashboard()
        {
            return SiteDriver.FindElementAndGetAttribute(DashboardPageObjects.CurrentDashboardView, How.CssSelector, "title");
        }

        public int GetCOBWidgetCountData(string label)
        {
            return int.Parse(JavaScriptExecutor.FindElement(Format(DashboardPageObjects.COBWidgetOverviewDivMainDataCssTemplate,
                label)).Text);

        }
        //public int GetCOBUnreviewedmembersData()
        //{
        //    return Convert.ToInt32(JavaScriptExecutor.FindElement(Format(DashboardPageObjects.COBWidgetOverviewDivMainDataCssTemplate,
        //        "Unreviewed Members")).Text);

        //}

        public int GetCOBWidgetCountDataFromDB(string fieldName, string table, string username)
        {
            return (int)Executor.GetSingleValue(Format(DashboardSqlScriptObjects.COBWidgetCountsForInternalUser, fieldName, table, username));
        }

        public int GetCOBUnreviewedClaimsDataForClientFromDB(string username)
        {
            return (int)Executor.GetSingleValue(Format(DashboardSqlScriptObjects.COBUnreviewedClaimsForClientUser, username));
        }
        public int GetCOBUnreviewedMembersDataFromDB(string username)
        {
            return (int)Executor.GetSingleValue(Format(DashboardSqlScriptObjects.COBUnreviewedMembersForInternalUser, username));
        }




        public int GetDataLabelCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.ClaimsOverviewDataLableCountXPath, How.XPath);
        }

        //public int ClaimsOverviewGridViewRowCount()
        //{
        //    return SiteDriver.FindElementsCount(DashboardPageObjects.ClaimsOverviewWidgetGridViewDataLabelCount,
        //        How.CssSelector);
        //}

        public string GetClaimsOverviewGridViewDataLabel(int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    Format(DashboardPageObjects.ClaimsOverviewWidgetGridViewDataLabelCssSelectorTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetClaimOverviewGridViewDataLabelToolTip(int row = 1, int col = 1)
        {
            return SiteDriver.FindElementAndGetAttribute(
                Format(DashboardPageObjects.ClaimsOverviewWidgetGridViewDataLabelCssSelectorTemplate, row, col),
                How.CssSelector, "title");
        }

        public string GetClaimOverviewGridViewTotalClaimCount(int row = 1, int col = 1)
        {
            return SiteDriver
                .FindElement(
                    Format(DashboardPageObjects.ClaimsOverviewWidgetGridViewTotalClaimCountCssSelectorTemplate, row, col),
                    How.CssSelector).Text;
        }

        public string GetClaimOverviewClaimCount(string header)
        {
            return SiteDriver
                .FindElement(
                    Format(DashboardPageObjects.ClaimsOverViewWidgetClaimCountXpathSelectorTemplate, header),
                    How.XPath).Text.Split('/')[0];
        }
        public string GetClaimOverviewTotalClaimCount(string header)
        {
            return SiteDriver
                .FindElement(
                    Format(DashboardPageObjects.ClaimsOverViewWidgetTotalClaimCountXpathSelectorTemplate, header),
                    How.XPath).Text.Split(':')[1].Trim();
        }

        public List<string> GetClaimsOverveiwGridViewDataLabels()
        {
            var claimsOverveiwGridViewDataLabels = new List<string>();

            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel());
            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel(1, 2));
            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel(2));
            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel(2, 2));
            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel(3));
            claimsOverveiwGridViewDataLabels.Add(GetClaimsOverviewGridViewDataLabel(3, 2));

            return claimsOverveiwGridViewDataLabels;
        }

        public List<string> ClaimsOverveiwGridViewDataLabelsToolTip()
        {
            var claimsOverveiwGridViewDataLabelsToolTip = new List<string>();

            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip());
            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip(1, 2));
            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip(2));
            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip(2, 2));
            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip(3));
            claimsOverveiwGridViewDataLabelsToolTip.Add(GetClaimOverviewGridViewDataLabelToolTip(3, 2));


            return claimsOverveiwGridViewDataLabelsToolTip;
        }

        public List<string> ClaimsOverviewGridViewClaimCount()
        {
            var claimsOverveiwGridViewClaimCount = new List<string>();

            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount());
            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount(1, 2));
            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount(2));
            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount(2, 2));
            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount(3));
            claimsOverveiwGridViewClaimCount.Add(GetClaimOverviewGridViewTotalClaimCount(3, 2));

            return claimsOverveiwGridViewClaimCount;

        }

        public List<string> GetRealTimeClaimsInWidget()
        {
            List<string> realTimeClaimsCount = new List<string>();
            realTimeClaimsCount.Add(GetClaimOverviewClaimCount("Real Time Claims"));
            realTimeClaimsCount.Add(GetClaimOverviewTotalClaimCount("Real Time Claims"));
            return realTimeClaimsCount;
        }

        public List<string> GetUnreviewedClaimsInWidget()
        {
            List<string> unreviewedClaimsCount = new List<string>();
            unreviewedClaimsCount.Add(GetClaimOverviewClaimCount("Unreviewed Claims"));
            unreviewedClaimsCount.Add(GetClaimOverviewTotalClaimCount("Unreviewed Claims"));
            return unreviewedClaimsCount;
        }

        public int GetAppealsOverviewDataLableCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.AppealsOverviewDataLableCountXPath, How.XPath);
        }

        public int GetSubDataLabelCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.ClaimsOverviewSubDataLableCountXPath, How.XPath);
        }

        public int GetAppealsOverviewSubDataLableCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.AppealsOverviewSubDataLableCountXPath, How.XPath);
        }

        public string GetDataLabel(int index)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.ClaimsOverviewDataLableXPath, index), How.XPath).Text;
        }

        public string GetDataLabelToolTip(int index)
        {
            return SiteDriver.FindElementAndGetAttribute(
                Format(DashboardPageObjects.ClaimsOverviewDataLableXPath, index), How.XPath, "title");
        }

        public string GetSubDataLabel(int index)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.ClaimsOverviewSubDataLableXPath, index), How.XPath).Text;
        }

        public string GetSubDataLabelToolTip(int index)
        {
            return SiteDriver.FindElementAndGetAttribute(Format(DashboardPageObjects.ClaimsOverviewSubDataLableXPath, index), How.XPath, "title");
        }

        public string GetAppealsOverviewDataLabel(int index)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.AppealsOverviewDataLableCssLocator, index), How.CssSelector).Text;
        }

        public int GetAppealsOverviewDataValuesByLabel(string label)
        {
             var value = SiteDriver.FindElement(Format(DashboardPageObjects.AppealsOverviewDataValueXpath, label),How.XPath).Text;
             return Convert.ToInt32(value);
        }

        public int GetTotalAppealCountsFromDatabaseByLabel(string label,string user)
        {
            switch (label)
            {
                case "Total Appeals":
                    return Convert.ToInt32(Executor.GetSingleStringValue(String.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayTotalCountFromDb,
                        user)));
                case "Urgent Appeals":
                    return Convert.ToInt32(Executor.GetSingleStringValue(String.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayTotalUrgentCountFromDb,
                        user)));
                case "Record Reviews":
                    return Convert.ToInt32(Executor.GetSingleStringValue(String.Format(DashboardSqlScriptObjects.GetCVAppealsDueTodayTotalRecordReviewCountFromDb,
                        user)));
                case "Restricted Appeals":
                    return Convert.ToInt32(Executor.GetSingleStringValue(String.Format(DashboardSqlScriptObjects.GetCVAppealsTotalRestrictedAppealsCountFromDb,
                        user)));
                case "Medical Record Reviews":
                    return Convert.ToInt32(Executor.GetSingleStringValue(String.Format(DashboardSqlScriptObjects.GetCVAppealsTotalMedicalRecordReviewsCountFromDb,
                        user)));
                default:
                    return -1;
            }
        }

        public string GetAppealsOverviewSubDataLabel(int index)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.AppealsOverviewSubDataLabelCssLocator, index), How.CssSelector).Text.TrimEnd().Substring(0, 9); ;
        }

        public bool IsClaimsOverveiwDataWidgetPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.ClaimsOverviewWidgetDataXPath, How.XPath);
        }

        public int GetDaysInColumnCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.DaysInColumnsCountXPath, How.XPath);
        }

        public string GetDaysInRow(int index)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.DaysInRowCssTemplate, index), How.CssSelector).Text;
        }

        public string GetActualDaysForColumn(int dayCount)
        {
            return DateTime.Now.AddDays(dayCount).ToString("ddd MM/d");
        }

        public List<string> GetNext5DaysColumnHeader()
        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.Next5DaysColumnHeaderCssLocator,
                How.CssSelector, "Text").Where(s => !IsNullOrEmpty(s)).ToList();
        }
        public List<string> GetNext5DaysValue()
        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.Next5DaysValueCssLocator,
                How.CssSelector, "Text");
        }

        public bool IsAppealsAssignedToEachAnalystPresent()
        {
            return SiteDriver.IsElementPresent(DashboardPageObjects.AppealsAssignedToEachAnalystXPath, How.XPath);
        }

        public int AppealsAssignedRowCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.AppealsAssignedRowCountCssLocator, How.CssSelector);
        }

        public string AppealsAssignedToAnalyst(int row)
        {
            return SiteDriver.FindElement(Format(DashboardPageObjects.UserFullNameInAppealsAssignedCssTemplate, row), How.CssSelector).Text;
        }

        public string AppealsAssignedToAnalystFullName(int row)
        {
            String fullName = SiteDriver.FindElement(Format(DashboardPageObjects.UserFullNameInAppealsAssignedCssTemplate, row), How.CssSelector).Text;
            return fullName.Substring(0, fullName.LastIndexOf('(') - 1);
        }

       
        public void ClickOnRefreshIconAndWait()
        {
            SiteDriver.FindElement(DashboardPageObjects.ClaimsOverviewRefreshIconXPath,How.XPath).Click();
            Console.WriteLine("Clicked on Refresh icon");
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(DefaultPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForPageToLoad();
        }

        /// <summary>
        /// Click on claims detial expand icon
        /// </summary>
        public ClaimsDetailPage ClickOnClaimsDetailExpandIcon()
        {
            var claimsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.ClaimsDetailExpandIconCssLocator, How.CssSelector);
                Console.WriteLine("Clicked Claims Detail Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue()));
                AssignPageTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
            }, () => new ClaimsDetailPageObjects(PageUrlEnum.ClaimsDetail.GetStringValue()));
            return new ClaimsDetailPage(Navigator, claimsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public COBClaimsDetailPage ClickOnCOBClaimsDetailExpandIcon()
        {
            var claimsDetailPage = Navigator.Navigate(() =>
            {

                JavaScriptExecutor.FindElement(DashboardPageObjects.COBClaimsOverViewExpandIconCssSelector).Click();
                Console.WriteLine("Clicked Claims Detail Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue()));
                //  AssignPageTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
            }, () => new COBClaimsDetailpageObject(PageUrlEnum.COBClaimsDetail.GetStringValue()));
            return new COBClaimsDetailPage(Navigator, claimsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public ClaimsDetailPage ClickOnFFPClaimsDetailExpandIcon()
        {
            var claimsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.ClaimsDetailExpandIconCssLocator, How.CssSelector);
                Console.WriteLine("Clicked Claims Detail Expand Icon");

                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.FFPDashboardClaimsDetail.GetStringValue()));
                AssignPageTitle(PageTitleEnum.FFPDashboardClaimsDetail.GetStringValue());
                CaptureScreenShot("FFP Dashboard Issue Capture");
            }, () => new ClaimsDetailPageObjects(PageUrlEnum.ClaimsDetail.GetStringValue()));
            return new ClaimsDetailPage(Navigator, claimsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Click on claims overview expand icon
        /// </summary>
        public ClaimsDetailPage ClickOnClaimsOverviewExpandIcon()
        {
            var claimsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate, DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue())).Click();
                Console.WriteLine("Clicked Claims Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue()));
                AssignPageTitle(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
            }, () => new ClaimsDetailPageObjects(PageUrlEnum.ClaimsDetail.GetStringValue()));
            return new ClaimsDetailPage(Navigator, claimsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Click on claims overview expand icon
        /// </summary>
        public COBAppealsDetailPage ClickOnAppealsOverviewExpandIcon()
        {
            var cobAppealsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.FindElement(Format(DashboardPageObjects.OverViewWidgetExpandIconTemplate, DashboardOverviewTitlesEnum.AppealsOverview.GetStringValue())).Click();
                Console.WriteLine("Clicked Appeals Overview Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.COBAppealsDetail.GetStringValue()));
                AssignPageTitle(PageTitleEnum.COBAppealsDetail.GetStringValue());
            }, () => new COBAppealsDetailPageObjects(PageUrlEnum.COBAppealsDetail.GetStringValue()));
            return new COBAppealsDetailPage(Navigator, cobAppealsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public static void AssignPageTitle(string title)
        {
            ClaimsDetailPageObjects.AssignPageTitle = title;
        }
        /// <summary>
        /// Click on appeals detial expand icon
        /// </summary>
        public AppealsDetailPage ClickOnAppealsDetailExpandIcon()
        {
            var appealsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.AppealsDetailExpandIconCSS, How.CssSelector);
                Console.WriteLine("Clicked Appeals Detail Expand Icon");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.DashboardAppealsDetail.GetStringValue()));
            }, () => new AppealsDetailPageObjects(PageUrlEnum.AppealsDetail.GetStringValue()));
            return new AppealsDetailPage(Navigator, appealsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Click on logic requests detail expand icon
        /// </summary>
        public DashboardLogicRequestsDetailsPage ClickOnLogicRequestsDetailExpandIcon()
        {
            var logicRequestsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.LogicRequestsDetailExpandIconCssSelector, How.CssSelector);
                Console.WriteLine("Clicked Logic Request Details Expand Icon");
                //SiteDriver.WaitForCondition(
                //    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Dashboard.GetStringValue()));
            }, () => new DashboardLogicRequestsDetailPageObjects(PageUrlEnum.LogicRequestsDetail.GetStringValue()));
            return new DashboardLogicRequestsDetailsPage(Navigator, logicRequestsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Click on logic requests detail expand icon for client dashboard
        /// </summary>
        public DashboardLogicRequestsDetailsPage ClickOnLogicRequestsDetailClientExpandIcon()
        {
            var logicRequestsDetailPage = Navigator.Navigate(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.LogicRequestDetailClientExpandIconCssSelector, How.CssSelector);
                Console.WriteLine("Clicked Logic Request Details Expand Icon");
                //SiteDriver.WaitForCondition(
                //    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Dashboard.GetStringValue()));
            }, () => new DashboardLogicRequestsDetailPageObjects(PageUrlEnum.LogicRequestsDetail.GetStringValue()));
            return new DashboardLogicRequestsDetailsPage(Navigator, logicRequestsDetailPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }
        public bool IsAppealValueInDaysColumnPresent()
        {
            for (var i = 2; i <= 4; i++)
            {
                for (var j = 1; j <= 5; j++)
                {
                    var appealValue = GetAppealValueInDaysColumn(j, i);
                    if (IsNullOrEmpty(appealValue))
                        return false;
                    Console.WriteLine("Appeal Value:<{0}>", appealValue);
                }
            }
            return true;
        }

        public string GetAppealValueInDaysColumn(int col, int row)
        {
            return
                SiteDriver.FindElement(
                    Format(DashboardPageObjects.AppealValueOnDayColumnCssLocator, col, row), How.CssSelector)
                    .Text;
        }

        public int GetAppealCreatorCount()
        {
            return SiteDriver.FindElementsCount(DashboardPageObjects.AppealCreatorAnalystCssLocator, How.CssSelector);
        }

        public string GetNoAppealMessage()
        {
            return SiteDriver.FindElement(DashboardPageObjects.AppealCreatorAnalystCssLocator, How.CssSelector).Text;
        }

        public bool IsAppealCreatorAppealValuesListInDescendingOrder()
        {
            var appealValue = JavaScriptExecutor.FindElements(DashboardPageObjects.AppealCreatorAppealValueCssLocator, How.CssSelector, "Text");
            return appealValue.Select(s => Convert.ToInt32(s)).ToList().IsInDescendingOrder();
        }

        public List<string> GetAppealCreatorAnalystList()
        {
            return JavaScriptExecutor.FindElements(DashboardPageObjects.AppealCreatorAnalystCssLocator, How.CssSelector, "Text");
        }

        public bool IsAppealCreatorAppealValueZero()
        {
            var appealValues = JavaScriptExecutor.FindElements(DashboardPageObjects.AppealCreatorAppealValueCssLocator, How.CssSelector, "Text");
            foreach (var appealValue in appealValues)
            {
                if (!IsNullOrEmpty(appealValue))
                {
                    if (Convert.ToInt32(appealValue) == 0)
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        public string GetTotalFFPUnreveiwedClaimsCount()
        {
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivMainDataCssTemplate, "Unreviewed Claims")).Text;

        }
        public string GetTotalFFPPendedClaimsCount()
        {
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivMainDataCssTemplate, "Pended Claims")).Text;

        }
        public string GetTotalFFPUnreleasedClaimsCount()
        {
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivMainDataCssTemplate, "Unreleased Claims")).Text;
        }

        public string GetTotalFFPUnapprovedClaimsCount()
        {
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivMainDataCssTemplate, "Unapproved Claims")).Text;
        }
        public string GetAllPendedClaimsCount()
        {
            var claimscount = SiteDriver.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivSecondaryDataCssTemplate, 2), How.CssSelector).Text;
            //int index = claimscount.IndexOf(":") + 1;
            var finalCount = claimscount.Remove(0, claimscount.IndexOf(":") + 1).Trim(' ');
            return finalCount;
        }

        public string GetReviewedYesterdayClaimsCount()
        {
            var claimscount = SiteDriver.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivSecondaryDataCssTemplate, 1), How.CssSelector).Text;
            // int index = claimscount.IndexOf(":")+1;
            var finalCount = claimscount.Remove(0, claimscount.IndexOf(":") + 1).Trim(' ');
            return finalCount;
        }
        public string GetReleasedYesterdayClaimsCount()
        {
            var claimscount = SiteDriver.FindElement(Format(DashboardPageObjects.FFPClaimsOverviewDivSecondaryDataCssTemplate, 3), How.CssSelector).Text;
            var finalCount = claimscount.Remove(0, claimscount.IndexOf(":") + 1).Trim(' ');
            return finalCount;
        }

        public List<string> GetAllFFPClientsList()
        {
            return Executor.GetTableSingleColumn(DashboardSqlScriptObjects.FFPClaimByClientForInternalUser);
        }

        public List<long> GetExpectedFFPClaimsCount(string userSeq)
        {
            List<long> expectedList = new List<long>();
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPUnreviewedClaimForInternalUser, userSeq)));
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPPendedClaimForInternalUser, userSeq)));
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPUnreleasedClaimForInternalUser, userSeq)));
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPAllPendedClaimForInternalUser, userSeq)));
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPApprovedYesterdayClaimForInternalUser, userSeq)));
            expectedList.Add(Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPReleasedYesterdayClaimForInternalUser, userSeq)));
            return expectedList;
        }


        public List<string> GetExpectedFFPClaimsCountForClient(string userID)
        {
            var table = Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.FFPClaimDataByClient, userID));
            var dataRows = table as DataRow[] ?? table.ToArray();
            return dataRows[0].ItemArray.Select(x => x.ToString()).ToList();


        }
        public void CloseConnection()
        {
            Executor.CloseConnection();
        }


        public IList<string> GetExpectedUnapprovedClaimsList(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.FFPUnapprovedClaimsForInternalUser, userSeq)).Take(50)
                .ToList();
        }
        public IList<string> GetExpectedFFPUnapprovedAltClaimNoForClientUserList(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.FFPUnapprovedAltClaimNoForClientUser, userSeq))
                .ToList();
        }
        public IList<string> GetPCIExpectedUnapprovedClaimsList(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.PCIUnapprovedClaimsForInternalUser, userSeq)).Take(50)
                .ToList();
        }

        public IList<string> GetExpectedUnapprovedClaimsClientCodeList(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.FFPUnapprovedClaimClientCodeForInternalUser, userSeq))
                .Take(50).ToList();
        }
        public IList<string> GetFFPExpectedUnapprovedClaimsClientCodeListForClientUser(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.FFPUnapprovedClaimClientCodeForClientUser, userSeq))
                .Take(50).ToList();
        }

        public IList<string> GetPCIExpectedUnapprovedClaimsClientCodeList(string userSeq)
        {
            return Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.PCIUnapprovedClaimClientCodeForInternalUser, userSeq))
                .Take(50).ToList();
        }

        public string GetExpectedFFPUnapprovedClaimsCount(string userSeq)
        {
            return Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPUnapporvedClaimCountForInternalUser, userSeq))
                .ToString();
        }
        public string GetFFPUnapporvedClaimCountForClientUserCount(string userSeq)
        {
            return Executor.GetSingleValue(Format(DashboardSqlScriptObjects.FFPUnapporvedClaimCountForClientUser, userSeq))
                .ToString();
        }
        public string GetPCIExpectedFFPUnapprovedClaimsCount(string userSeq)
        {
            return Executor.GetSingleValue(Format(DashboardSqlScriptObjects.PCIUnapporvedClaimCountForInternalUser, userSeq))
                .ToString();
        }

        public List<string> GetHolidays()
        {
            var list = Executor.GetTableSingleColumn(DashboardSqlScriptObjects.GetHolidays);
            return list; 
        }

        public UserKpi GetMyDashboardDetailsForUserFromMvUserKpiMaterializedView(string userid)
        {
            var newList = new List<UserKpi>();
            var mvuserkpiList =
                Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.MyDashboardMvUserKpiDetails, userid));
            // List<UserKpi> userKpiList = mvuserkpiList..ToList();
            if (mvuserkpiList == null)
            {
                newList.Add(new UserKpi
                {
                    LegacyClaimsReviewed = "0",
                    NucleusClaimsReviewed = "0",
                    TotalClaimsReviewed = "0",
                    LegacyAvgClaimsPerHour = "0",
                    NucleusAvgClaimsPerHour = "0",
                    TotalAvgClaimsPerHour = "0",
                    LegacyAppealsReviewed = "0",
                    NucleusAppealsReviewed = "0",
                    AvgAppealsPerHour = "0",
                    TotalAppealsCompleted = "0",
                    WeightedAppealsCompleted = "0",
                    WeightedClaimsCompleted = "0",
                    LastUpdated = DateTime.Now.AddYears(-2)
                });
            }
            else
                foreach (DataRow row in mvuserkpiList)
                {
                    newList.Add(new UserKpi
                    {
                        LegacyClaimsReviewed = Convert.ToString(row["LegacyClaimsReviewed"]),
                        NucleusClaimsReviewed = Convert.ToString(row["NucleusClaimsReviewed"]),
                        TotalClaimsReviewed = Convert.ToString(row["TotalClaimsReviewed"]),
                        LegacyAvgClaimsPerHour = Convert.ToString(row["LegacyAvgClaimsPerHour"]),
                        NucleusAvgClaimsPerHour = Convert.ToString(row["NucleusAvgClaimsPerHour"]),
                        TotalAvgClaimsPerHour = Convert.ToString(row["TotalAvgClaimsPerHour"]),
                        LegacyAppealsReviewed = Convert.ToString(row["LegacyAppealsReviewed"]),
                        NucleusAppealsReviewed = Convert.ToString(row["NucleusAppealsReviewed"]),
                        AvgAppealsPerHour = Convert.ToString(row["AvgAppealsPerHour"]),
                        TotalAppealsCompleted = Convert.ToString(row["TotalAppealsCompleted"]),
                        WeightedAppealsCompleted = Convert.ToString(row["WeightedAppealsCompleted"]),
                        WeightedClaimsCompleted = Convert.ToString(row["WeightedClaimsCompleted"]),
                        LastUpdated = Convert.ToDateTime(row["LastUpdated"])

                    });
                }
            return newList[0];
        }

        public string GetLastUpdatedFromMvUserKpiInMyDashboard()
        {
            return Executor.GetSingleValue(DashboardSqlScriptObjects.GetLastUpdatedFromMvUserKpiInMyDashboard)
                .ToString();
        }



        public void UpdateLastUpdpatedDateAndResfreshMvUuserKpi(string date)
        {
            Executor.ExecuteQuery(Format(DashboardSqlScriptObjects.UpdateLastUpdatedInUserKpiAndRefreshMaterizaliedView, date));
        }

        public void ClickonExportIconByWidget(string widgetName)
        {
            JavaScriptExecutor.FindElement(Format(DashboardPageObjects.ExportIconByCss,widgetName)).Click();
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<ClaimSearchPage>();
            return fileName;

        }
        public bool IsMydashboardOptionPresentInDashboardMenu()
        {
            return JavaScriptExecutor.IsElementPresent(DashboardPageObjects.MyDashboardCssLocator);
        }

        public bool IsMicrostrategyOptionPresentInDashboardMenu()
        {
            return JavaScriptExecutor.IsElementPresent(DashboardPageObjects.MicrostrategyDashboardCssLocator);
        }

        public string GetTitle()
        {
            return SiteDriver.Title;
        }
        public bool IsWidgetPresentInMydashboard(bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.MyDashboardWidgetCssLocatorTemplate, expectetdVal));
        }


        public bool IsMydashboardShownInRightCornerForWidget(bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.IsElementPresent(Format(DashboardPageObjects.MyDashboardOnRightWidgetCssLocatorTemplate, expectetdVal));
        }

        public void SelectMyDashboardFromFilterOptions()
        {


            JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MyDashboardCssLocator);
            Console.WriteLine("Clicked on My Dashboard Filter.");
            WaitForStaticTime(1000);
        }

        public MicrostrategyPage SelectMicrostrategyFilterOptions()
        {
            var microstrategyPage = Navigator.Navigate<MicroStrategyPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(DashboardPageObjects.FilterOptionsIconCssLocator, How.CssSelector);
                Console.WriteLine("Clicked on Filter Dashboard icon.");
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.IsElementPresent(DashboardPageObjects.MicrostrategyDashboardCssLocator));
                JavaScriptExecutor.ClickJQuery(DashboardPageObjects.MicrostrategyDashboardCssLocator);
                Console.WriteLine("Clicked on Microstrategy Filter.");
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.DashboardLabelCssSelector, How.CssSelector));
                SiteDriver.WaitToLoadNew(2000);
                SiteDriver.WaitForPageToLoad();
                //SiteDriver.SwitchFrameByCssLocator("div#mydossier>iframe");
                //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(MicroStrategyPageObjects.LodingSpinnerIconCssSelectyor, How.CssSelector));
                //SiteDriver.SwitchBackToMainFrame();
            });

            return new MicrostrategyPage(Navigator, microstrategyPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }
        public string GetValueOfTotalWeightedClaimsOrAppealThisWeek(bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardTotalThisWeekCssTemplate, expectetdVal)).Text;
        }

        public string GetValueOfAvgPerHourThisWeek(bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardAvgperHourCssTemplate, expectetdVal)).Text.Split(':').Last().Trim();
        }


        public string GetValueOfClaimsOrAppealThisWeek(string metricName)
        {
            var text = JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardClaimsOrAppealThisWeekCssTemplate, metricName)).Text;
            var value = Regex.Match(text, @"\d+").Value;
            return value;
        }

        public string GetValueOfWeightedClaimsOrAppealThisWeek(string metricName)
        {
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardWeightedClaimsOrAppealThisWeekCssTemplate, metricName)).Text;
        }
        public string GetValueOfSecondaryWidgetAvgPerHour(int row = 1, bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardSecondaryWidgetAvgPerHourCssTemplate, expectetdVal, row)).Text.Split(':').Last().Trim();
        }


        public string GetValueOfLasUpdatedDataInSecondaryWidget(bool isClaim = true)
        {
            var expectetdVal = isClaim ? "Claim Performance" : "Appeal Performance";
            return JavaScriptExecutor.FindElement(Format(DashboardPageObjects.MyDashboardWidgetRefreshDataCssTemplate, expectetdVal)).Text;
        }

        public List<string> GetProductFilterList()
        {

            return JavaScriptExecutor.FindElements(DashboardPageObjects.FilterOptionsListCssLocator, How.CssSelector, "Text");
        }

        public bool IsCorrectDashboard(string dashboard)
        {
            return SiteDriver.IsElementPresent(Format(DashboardPageObjects.DashboardTitleXPathLocator, dashboard), How.XPath);
        }

        public List<string> GetClaimsCountInClaimsOverviewWidgetFromDatabase(string userSeq)
        {
            //var newList = new string[6];
            List<string> newList1 = new List<string>();
            var claimCountList = Executor
                .GetCompleteTable(Format(
                    DashboardSqlScriptObjects.TotalClaimsCountInClaimsOverviewWidgetForInternalUser, userSeq));
            //return claimCountList;

            foreach (DataRow row in claimCountList)
            {
                newList1.Add(row[0].ToString()); //TotalPendedClaims
                newList1.Add(row[1].ToString()); //TotalAllPendedClaims
                newList1.Add(row[2].ToString()); //TotalAwaitingQAReviewClaims
                newList1.Add(row[3].ToString()); //TotalCompletedQAClaims
                newList1.Add(row[4].ToString()); //TotalUnreleasedClaims
                newList1.Add(row[5].ToString()); //TotalReleasedYesterdayClaims
            }
            return newList1;

        }

        public List<string> GetClaimsCountForRealTimeClient(string userSeq)
        {
            var newList = new List<string>();
            var realTimeClientClaim = Executor
                .GetCompleteTable(Format(DashboardSqlScriptObjects.TotalUnreviewedRealTimeClaims,
                    userSeq));
            foreach (DataRow row in realTimeClientClaim)
            {
                newList.Add(row[0].ToString());
                newList.Add(row[1].ToString());
            }
            return newList;
        }

        public List<string> GetUnreviewedClaimsCountInWidget(string userSeq)
        {
            var newList = new List<string>();
            var unreviewedClaim = Executor
                .GetCompleteTable(Format(DashboardSqlScriptObjects.TotalUnreviewedClaimsForPCIInternalUser,
                    userSeq));
            foreach (DataRow row in unreviewedClaim)
            {
                newList.Add(row[0].ToString()); //TotalUnreviewedClaims
                newList.Add(row[1].ToString()); //TotalReviewedYesterdayClaims
            }
            return newList;
        }

        public string GetNextRefreshForCVDashboard(string userId)
        {
            return Executor.GetSingleStringValue(Format(DashboardSqlScriptObjects.GetLastUpdatedCVDashboard, userId));
        }

        public string GetNextRefreshForFFPDashboard(string userId)
        {
            var time = Executor.GetSingleStringValue(Format(DashboardSqlScriptObjects.GetLastUpdatedFFPDashboard, userId));
            return time;
        }

        public string GetNextRefreshForCVDashboardClient(string userId)
        {
            var time = Executor.GetSingleStringValue(Format(DashboardSqlScriptObjects.GetLastUpdatedCVDashboardClient, userId));
            return time;
        }

        public string GetNextRefreshForFFPDashboardClient(string userId)
        {
            var time = Executor.GetSingleStringValue(Format(DashboardSqlScriptObjects.GetLastUpdatedFFPDashboardClient, userId));
            return time;
        }

        public bool IsNextRefreshTimePresent()
        {
            return SiteDriver.FindElement(DashboardPageObjects.NextRefreshTimeCssSelector, How.CssSelector).Text.Contains("Next Refresh :");
        }

        public bool IsDownloadIconPresentInWidget() =>
            SiteDriver.IsElementPresent(DashboardPageObjects.DownloadIconCssSelector, How.CssSelector);

        public void ClickDownloadIconInWidget()
        {
            SiteDriver
                .FindElement(DashboardPageObjects.DownloadIconCssSelector, How.CssSelector).Click();
            SiteDriver.WaitForPageToLoad();
        }

        public List<string> GetColumnHeaderListFromCOBWidgetDetailPage() =>
            JavaScriptExecutor.FindElements(DashboardPageObjects.COBWidgetDetailColumnsCssSelector, How.CssSelector, "Text");

        public List<string> GetAppealsDetailValueListByCol(int col) =>
            JavaScriptExecutor.FindElements(Format(DashboardPageObjects.COBAppealsDetailColumnValuesCssSelectorTemplate, col), How.CssSelector, "Text");

        public string GetAppealsDetailTotalValueByCol(int col) =>
            SiteDriver.FindElement(Format(DashboardPageObjects.COBAppealDetailsTotalValueCssSelectorTemplate, col), How.CssSelector).Text;

        public List<string> GetAppealDetailValueForInternalUserFromDbByCol(string username, int col = 0) =>
            Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.GetCOBAppealDetailForInternalUsersFromDb, username), col);

        public List<string> GetAppealDetailValueForClientUserFromDbByCol(string username, int col = 0) =>
            Executor.GetTableSingleColumn(Format(DashboardSqlScriptObjects.GetCOBAppealsDetailValueForClientUsersFromDb, username), col);
        public List<List<string>> GetExcelDataListForInternalUsers(string username)
        {
            var dataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.GetCOBAppealDetailForInternalUsersFromDb, username));
            dataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return dataList;
        }

        public List<List<string>> GetExcelDataListForClientUsers(string username)
        {
            var dataList = new List<List<string>>();
            var temp = Executor.GetCompleteTable(Format(DashboardSqlScriptObjects.GetCOBAppealsDetailValueForClientUsersFromDb, username));
            dataList = temp.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
            return dataList;
        }
        public bool IsCollapseIconPresent() =>
            SiteDriver.IsElementPresent(DashboardPageObjects.CollapseIconCssSelector, How.CssSelector);

        public void ClickCollapseIcon()
        {
            var element = SiteDriver.FindElement(DashboardPageObjects.CollapseIconCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForPageToLoad();
        }

        #endregion

    }
}
