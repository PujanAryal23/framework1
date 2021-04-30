using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.Support.Common
{
    public sealed class CalenderPage
    {
        #region PRIVATE FIELDS
        private readonly ISiteDriver SiteDriver;

        public const string MonthCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-lendar select.pika-select-month";

        public const string MonthValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// select[contains(@class,'pika-select-month')]/option[text()='{0}']";

        public const string YearCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-lendar select.pika-select-year";

        public const string YearValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// select[contains(@class,'pika-select-year')]/option[text()='{0}']";

        public const string DayValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-lendar')]// table[contains(@class,'pika-table')]//td/button[text()='{0}']";

        public const string HourCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-timepicker select.pika-select-hours";
        public const string HourValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-timepicker')]/select[contains(@class,'pika-select-hours')]/option[text()='{0}']";

        public const string MinuteCssLocator =
            "div.pika-single.is-bound:not(.is-hidden)>div.pika-timepicker select.pika-select-minutes";
        public const string MinutesValueXPathTemplate =
            "//div[contains(@class,'pika-single is-bound')][not(contains(@class,'is-hidden'))]/div[contains(@class,'pika-timepicker')]/select[contains(@class,'pika-select-minutes')]/option[text()='{0}']";



        #endregion


        #region PUBIC PROPERTIES
        #endregion

        #region CONSTRUCTOR

        public CalenderPage(ISiteDriver siteDriver)
        {
            SiteDriver = siteDriver;
        }


        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Date format: MM/DD/YYYY
        /// DD=2 (for single digit)
        /// DD=12 (for multiple digits)
        /// </summary>
        /// <param name="date"></param>
        public void SetDate(string date)
        {
            var splitDate = date.Split('/');
            var month = "";
            switch (splitDate[0])
            {
                case "01":
                    month = "January";
                    break;

                case "02":
                    month = "February";
                    break;

                case "03":
                    month = "March";
                    break;

                case "04":
                    month = "April";
                    break;

                case "05":
                    month = "May";
                    break;

                case "06":
                    month = "June";
                    break;

                case "07":
                    month = "July";
                    break;

                case "08":
                    month = "August";
                    break;

                case "09":
                    month = "September";
                    break;

                case "10":
                    month = "October";
                    break;

                case "11":
                    month = "November";
                    break;

                case "12":
                    month = "December";
                    break;
            }
            SiteDriver.FindElement(YearCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(YearValueXPathTemplate, splitDate[2]), How.XPath).Click();
            SiteDriver.FindElement(MonthCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MonthValueXPathTemplate, month), How.XPath).Click();
            SiteDriver.FindElement(string.Format(DayValueXPathTemplate, splitDate[1]), How.XPath).Click();
        }

        public void SetTime(string time)
        {
            var splitTime = time.Split(':');
            SiteDriver.FindElement(MinuteCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MinutesValueXPathTemplate, splitTime[1]), How.XPath).Click();
            SiteDriver.FindElement(HourCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(HourValueXPathTemplate, splitTime[0]), How.XPath).Click();

        }

        #endregion
    }
}
