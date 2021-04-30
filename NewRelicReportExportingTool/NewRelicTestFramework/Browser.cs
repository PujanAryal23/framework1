using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Web.UI;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using NewRelicTestFramework.Repositories;
using System.Windows.Forms;
using OpenQA.Selenium.PhantomJS;

namespace NewRelicTestFramework
{
    public static class Browser
    {
        static IWebDriver webDriver;   
            
        static IList<ErrorsList> prodUATErrorList;       
        static IList<TransactionTraces> slowTransactionList;
        static RemoteWebDriver driver;
        public static void GenerateProductionErrorReport(string url,ReportType reportType,Environment environment, DateTime date)
        {
            try
            {
                InitializeWebDriver();
                webDriver.Url = url;
                LoginToNewRelic(date);
                NavigateToErrorsPage(environment);
                CollectErrorReport(reportType, environment, date);
                webDriver.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"Error");
            }           

        }
        

        public static void GenerateUATErrorReport(string url, ReportType reportType, Environment environment, DateTime date)
        {
            try
            {
                //var switches = new[] { "--start-maximized", "--disable-popup-blocking", "--ignore-certificate-errors", "--multi-profiles", "--profiling-flush", "--disable-extensions" };            
                //var options = new ChromeOptions();
                //options.AddArguments(switches);           
                //var _capabilities = options.ToCapabilities() as DesiredCapabilities;
                //webDriver = new RemoteWebDriver(
                //new Uri("http://172.27.4.247:4445/wd/hub"), _capabilities);
                InitializeWebDriver();
                webDriver.Url = url;
                LoginToNewRelic(date);
                NavigateToErrorsPage(environment);
                CollectErrorReport(reportType, environment, date);
                webDriver.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
           

        }

        public static void GenerateUATTransactionReport(string url, ReportType reportType, Environment environment,DateTime reportDate)
        {
            try
            {
                InitializeWebDriver();
                webDriver.Url = url;
                LoginToNewRelic(reportDate);
                NavigateToTransactionPage(environment);
                CollectTransactionReport(reportType, environment, reportDate);
                webDriver.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error");
            }          
          
        }
        public static void GenerateProdTransactionReport(string url, ReportType reportType, Environment environment,DateTime date)
        {
            try
            {
                InitializeWebDriver();
                webDriver.Url = url;
                LoginToNewRelic(date);
                NavigateToTransactionPage(environment);
                CollectTransactionReport(reportType, environment, date);
                webDriver.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error");
            }
           
        }

        private static void InitializeWebDriver()
        {
                #if DEBUG
                ChromeDriverService _driverService = ChromeDriverService.CreateDefaultService(Path.GetFullPath("Executable"));
                webDriver = new ChromeDriver(_driverService);                
                #else
                PhantomJSDriverService _drService = PhantomJSDriverService.CreateDefaultService(Path.GetFullPath("Executable"));
                webDriver = new PhantomJSDriver(_drService);
                #endif
        }



        #region Private Methods

        private static void CollectTransactionReport(ReportType reportType,Environment environment,DateTime reportDate)
        {
            try
            {
                IJavaScriptExecutor _executor = (IJavaScriptExecutor)webDriver;
                var link = WaitandReturnElementExists(By.CssSelector("footer#button_footer>nav.actionable>a#permalink.link.copyable_link"), webDriver, 2000).GetAttribute("href");
                var transactionList = webDriver.FindElements(By.CssSelector("table.combined.transaction_traces_summary.tablesorter>tbody>tr"));
                slowTransactionList = new List<TransactionTraces>();

                for (var count = 0; count < transactionList.Count - 1; count++)
                {

                    var transaction = transactionList[count];
                    var components = new List<string>();

                    WaitandReturnElementExists(By.CssSelector("td.tt_link.trans_name>a"), transaction, 2000);
                    var page = transaction.FindElement(By.CssSelector("td.tt_link.trans_name>a"));
                    Actions scrollTo = new Actions(webDriver);
                    scrollTo.MoveToElement(page).Build().Perform();

                    var pageName = page.Text;
                    var newTabLink = page.GetAttribute("href");

                    _executor.ExecuteScript("window.open()");
                    var windows = webDriver.WindowHandles.ToList();
                    webDriver.SwitchTo().Window(windows[1]);
                    webDriver.Navigate().GoToUrl(newTabLink);

                    WaitForCondition(() => IsElementPresent(By.CssSelector("h2.section.with_corner_button")), 10000);
                    Thread.Sleep(5000);
                    if (IsElementPresent(By.Id("ajax_error_flash_message")))
                    {
                        webDriver.SwitchTo().Window(windows[1]).Close();
                        webDriver.SwitchTo().Window(windows[0]);
                        continue;
                    }

                    WaitandReturnElementExists(By.CssSelector("div.summary_area>table>tbody>tr"), webDriver, 5000);
                    var slowComponents = webDriver.FindElements(By.CssSelector("div.summary_area>table>tbody>tr"));
                    for (var compCount = 0; compCount < slowComponents.Count; compCount++)
                    {
                        var _transactionTraces = new TransactionTraces();
                        var slowComponent = slowComponents[compCount];

                        string[] stringSeparators = new string[] { "\\s+" };
                        var duration = slowComponent.FindElement(By.CssSelector("td:nth-of-type(4)")).Text.Split(' ').First();

                        if (String.IsNullOrEmpty(duration))
                        {
                            continue;
                        }
                        if (ParseString(duration) < 3000)
                        {
                            break;
                        }

                        _transactionTraces.Count = 1;
                        _transactionTraces.SlowPage = pageName;
                        _transactionTraces.IssueType = slowComponent.FindElement(By.CssSelector("td:nth-of-type(2)")).Text.Split(' ').Last();
                        _transactionTraces.Notes = "";
                        if (IsSame(_transactionTraces.IssueType, _transactionTraces.SlowPage))
                        {
                            continue;
                        }
                        if (_transactionTraces.IssueType.Split('.').Last() == "aspx")
                        {
                            continue;
                        }

                        slowTransactionList.Add(_transactionTraces);
                    }
                    webDriver.SwitchTo().Window(windows[1]).Close();
                    webDriver.SwitchTo().Window(windows[0]);

                }

                TransactionReportRepository.SaveTransactionReport(TransformTransactionList(slowTransactionList), reportType, environment, link, reportDate);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"Error");
            }
          
            //Excel.ExportTransactionReportToExcel(TransformTransactionList(slowTransactionList));
            
        }

        private static List<TransactionTraces> TransformTransactionList(IList<TransactionTraces> slowTransactionList)
        {
            var transactionList = new List<TransactionTraces>();
            transactionList = slowTransactionList.GroupBy(x => new { x.SlowPage, x.IssueType })
            .Select(x => new TransactionTraces()
            {                
                Count = x.Count(),
                IssueType = x.Key.IssueType,
                SlowPage = x.Key.SlowPage,
                Notes = x.First().Notes
            })
            .OrderByDescending(x => x.Count).ToList();
            return transactionList;
        }

        private static bool IsSame(string issueType, string slowPage)
        {
            bool isSame = false;
            var sub = slowPage.Substring(0,1);
            if(sub=="/")
            {
                slowPage=slowPage.Remove(0,1);
            }
            slowPage=slowPage.Replace("/", ".");
            if(issueType==slowPage)
            {
                isSame = true;
            }
            return isSame;

        }

        private static double  ParseString(string duration)
        {
            double num;
            if (double.TryParse(duration, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out num))
            {
                return num;
            }
            else
            {
                return Convert.ToDouble(duration);
            }
        }

        private static void CollectErrorReport(ReportType reportType,Environment environment,DateTime reportDate)
        {
            try
            {
                IJavaScriptExecutor _executor = (IJavaScriptExecutor)webDriver;
                var link = WaitandReturnElementExists(By.CssSelector("footer#button_footer>nav.actionable>a#permalink.link.copyable_link"), webDriver, 2000).GetAttribute("href");
                var errorList = webDriver.FindElements(By.CssSelector("table#errors>tbody>tr"));
                prodUATErrorList = new List<ErrorsList>();

                for (var count = 0; count < errorList.Count; count++)
                {
                    var errorRecord = new ErrorsList();
                    var error = errorList[count];
                    WaitandReturnElementExists(By.CssSelector("td.numeric.sorted"), error, 2000);
                    var errorCount = error.FindElement(By.CssSelector("td.numeric.sorted")).Text;
                    var errorSource = error.FindElement(By.CssSelector("td.error_name"));
                    var beginDate = error.FindElement(By.CssSelector("td:nth-of-type(1)")).Text;
                    var endDate = error.FindElement(By.CssSelector("td:nth-of-type(2)")).Text;
                    var value = errorSource.Text;
                    string[] stringSeparators = new string[] { "\r\n" };
                    var sourceAndIssue = value.Split(stringSeparators, StringSplitOptions.None);
                    var issue = sourceAndIssue[1].Split('.').Last();
                    var errMgs = error.FindElement(By.CssSelector("td.error_message>a"));
                    _executor.ExecuteScript("arguments[0].click();", errMgs);
                    var permalink = "";
                    var perma = WaitandReturnElementExists(By.CssSelector("footer#button_footer>nav.actionable>a#permalink.link.copyable_link"), webDriver, 2000);
                    if (!webDriver.Title.Contains("Traced error"))
                    {
                        webDriver.Navigate().Back();
                        errorList = webDriver.FindElements(By.CssSelector("table#errors>tbody>tr"));
                        continue;
                    }

                    _executor.ExecuteScript("arguments[0].click();", perma);
                    permalink = perma.GetAttribute("href");
                    WaitandReturnElementExists(By.CssSelector("p.error_message"), webDriver, 2000);
                    var errorMessage = webDriver.FindElement(By.CssSelector("p.error_message")).Text;

                    webDriver.Navigate().Back();
                    errorList = webDriver.FindElements(By.CssSelector("table#errors>tbody>tr"));

                    int errCnt;
                    if (int.TryParse(errorCount, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out errCnt))
                    {
                        errorRecord.ErrorCount = errCnt;
                    }
                    else
                    {
                        errorRecord.ErrorCount = Convert.ToInt32(errorCount);
                    }
                    errorRecord.IsNew = "";
                    errorRecord.Note = "";
                    errorRecord.Status = "Open";
                    errorRecord.Source = sourceAndIssue[0].Replace("'", "''");
                    errorRecord.Issue = issue;
                    errorRecord.Description = errorMessage.Replace("'", "''");//excape quotes to insert string to database
                    errorRecord.ErrorPermalink = permalink.Replace("'", "''");
                    errorRecord.BeginDate = ConvertStringToOracleTimeStampFormat(beginDate);
                    errorRecord.EndDate = ConvertStringToOracleTimeStampFormat(endDate);

                    prodUATErrorList.Add(errorRecord);
                }
                ErrorReportRepository.SaveErrorReport(prodUATErrorList, reportType, environment, link, reportDate);
                //Excel.ExportErrorReportToExcel(prodUATErrorList);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }

        private static string ConvertStringToOracleTimeStampFormat(string str)
        {
            if (!String.IsNullOrEmpty(str))
                return DateTime.ParseExact(str, "MMM d, \\'yy h:mm tt", CultureInfo.InvariantCulture).ToString("dd-MMM-yy h:mm:ss tt");
            else
                return "";
               
           
        }
        private static bool IsElementPresent(By locator)
        {
            try
            {
                webDriver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        private static IWebElement WaitandReturnElementExists(By locator, ISearchContext context, int elementTimeOut = 2000)
        {
            if (elementTimeOut == 0)
                return context.FindElement(locator);

            var wait = new WebDriverWait(new SystemClock(), webDriver, TimeSpan.FromSeconds(120), TimeSpan.FromMilliseconds(5000));
            IWebElement webElement = null;
            wait.Until(driver =>
            {
                try
                {
                    webElement = context.FindElement(locator);
                    return webElement != null;

                }
                catch (Exception ex)
                {
                    return false;
                }
            });
            return webElement;
        }
       

        private static void WaitForCondition(Func<bool> f, int sec = 6000)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(sec));
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        return f();
                    }
                    catch (Exception)
                    {
                       return false;
                    }
                });
            }
            catch (UnhandledAlertException ex)
            {
                Console.Out.WriteLine("unhandled exception" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }


        private static void NavigateToErrorsPage(Environment environment)
        {
            try
            {
                if (environment == Environment.PROD)
                {
                    webDriver.FindElement(By.LinkText("Nucleus Prod")).Click();
                }
                else if (environment == Environment.UAT)
                {
                    webDriver.FindElement(By.LinkText("Nucleus UAT")).Click();
                }
                var eventsList = webDriver.FindElements(By.ClassName("second_level"));
                eventsList[1].FindElement(By.LinkText("Errors")).Click();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error");
            }
                     
        }

        private static void NavigateToTransactionPage(Environment environment)
        {
            try
            {
                if (environment == Environment.UAT)
                {
                    webDriver.FindElement(By.LinkText("Nucleus UAT")).Click();
                }
                else if (environment == Environment.PROD)
                {
                    webDriver.FindElement(By.LinkText("Nucleus Prod")).Click();
                }

                var eventsList = webDriver.FindElements(By.ClassName("second_level"));
                eventsList[0].FindElement(By.LinkText("Transactions")).Click();

                IJavaScriptExecutor _executor = (IJavaScriptExecutor)webDriver;
                _executor.ExecuteScript("document.body.scrollTop = document.body.scrollHeight;document.documentElement.scrollTop = document.documentElement.scrollHeight; ");
                WaitandReturnElementExists(By.CssSelector("div#transaction_traces_summary_container>table.combined.transaction_traces_summary.tablesorter>tfoot>tr>td.disrespect_visited.right>a.show_more"), webDriver, 2000);
                var expand = webDriver.FindElement(By.CssSelector("div#transaction_traces_summary_container>table.combined.transaction_traces_summary.tablesorter>tfoot>tr>td.disrespect_visited.right>a.show_more"));

                _executor.ExecuteScript("arguments[0].click()", expand);
                WaitForCondition(() => !(webDriver.FindElement(By.CssSelector("img.spinner")) == null ? false : true), 5000);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
           
        }
        private static void ClickGoButton(IJavaScriptExecutor _executor)
        {
            var goButtonElement = webDriver.FindElement(By.CssSelector("div#time_picker_set_button>#set_the_time"));
            _executor.ExecuteScript("arguments[0].click();", goButtonElement);
        }

        private static void SelectDayFromDatePicker(IJavaScriptExecutor _executor)
        {
            //select Day from Calendar
            var calendarValues = webDriver.FindElements(By.CssSelector("table.ui-datepicker-calendar>tbody>tr"));
            
            foreach (var tr in calendarValues)
            {
                var tds = tr.FindElements(By.CssSelector("td"));
                foreach (var item in tds)
                {
                    if(item.Text==2.ToString())
                    {
                        _executor.ExecuteScript("arguments[0].click();", item);
                        return;

                    }
                }
            }
            var element = webDriver.FindElement(By.CssSelector("table.ui-datepicker-calendar>tbody>tr:nth-of-type(1)>td"));
            _executor.ExecuteScript("arguments[0].click();", element);
        }

        private static void ClearTimeOfDatePicker(IJavaScriptExecutor calendarValues)
        {
            var timeElement= WaitandReturnElementExists(By.CssSelector("div>input.ui-timepicker-input"), webDriver, 2000);
            timeElement.SendKeys(OpenQA.Selenium.Keys.Control + "a");
            timeElement.SendKeys("");       
        }

        private static void LoginToNewRelic(DateTime date)
        {
            try
            {
                IJavaScriptExecutor _executor = (IJavaScriptExecutor)webDriver;
                var loginButton = webDriver.FindElement(By.CssSelector("span.text"));
                _executor.ExecuteScript("arguments[0].click();", loginButton);
                var submit = WaitandReturnElementExists(By.Id("login_submit"), webDriver, 2000);
                webDriver.Manage().Window.Maximize();
                // type | id=login_email | bindiya.singh@verscend.com
                webDriver.FindElement(By.Id("login_email")).Clear();
                webDriver.FindElement(By.Id("login_email")).SendKeys("bindiya.singh@verscend.com");
                // type | id=login_password | Welcome12!
                webDriver.FindElement(By.Id("login_password")).Clear();
                webDriver.FindElement(By.Id("login_password")).SendKeys("Welcome12!");
                // click | id=login_submit |         
                _executor.ExecuteScript("arguments[0].click();", submit);
                Thread.Sleep(1000);
                webDriver.Navigate().GoToUrl(String.Format("https://rpm.newrelic.com/set_time_window?tw%5Bfrom_local%5D=true&tw%5Bdur%5D=168&tw%5Bend%5D={0}+10%3A00+PM", date.ToString("yyyy-MM-dd")));
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"Error");
            }
            
        }

#endregion
    }
}
