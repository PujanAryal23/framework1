using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Legacy.Service;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Login;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.Support.Constants;
using Legacy.Service.Support.Enum;
using NUnit.Framework;
using Legacy.Service.PageServices.Product;
using System.Collections.Generic;
using System.Diagnostics;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Legacy.UIAutomation.TestSuites.Base
{
    [TestFixture]
    public abstract class AutomatedBase
    {
        private StartLegacy _startFlow;
        protected LoginPage LoginPage;
        protected DefaultPage CurrentPage;
        protected ProductPage ProductPage;
       
        protected string Environment;
        protected string TestDataFilePath;
        protected string TestMappingFilePath;
        protected BasePageService BasePage;
        protected string OriginalWindowHandle;
        protected string CurrentWindowHandle;

        protected virtual String FullyQualifiedClassName { get; set; }
        protected virtual ProductEnum TestProduct { get; set; }

        [OneTimeSetUp]
        protected virtual void FixtureSetUp()
        {
            const string dataDirectory = @".\Data\";
            SetTestDataFilePath(dataDirectory);
            TestMappingFilePath = Path.GetFullPath(string.Concat(dataDirectory,ConfigurationManager.AppSettings["TestMappingFile"]));
            DataHelper.LoadTestData(TestDataFilePath, TestProduct.ToString());
            DataHelper.LoadTestMappings(TestMappingFilePath);
            _startFlow = new StartLegacy();
            StartLegacy.Product = TestProduct;
            LoginPage = _startFlow.StartLegacyApplication();
        }

        [OneTimeTearDown]
        protected virtual void FixtureTearDown()
        {
            try
            {
                if (CurrentPage != null)
                {
                    CurrentPage.Logout();
                }
                else if (BasePage != null)
                {
                    if (BasePage.CurrentPageTitle == PageTitleEnum.WelcomePage.GetStringValue())
                    {
                        ((WelcomePage)BasePage).Logout();
                    }
                    else
                    {
                        SiteDriver.WaitForIe(20000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while trying to logout:" + ex.Message);
            }
            _startFlow.Dispose();
            //CleanupBrowserDriver();
        }

        private static void CleanupBrowserDriver()
        {
            IList<string> processes = new List<string>() { "IEDRIVERSERVER", "IEXPLORE", "CHROMEDRIVER", "CHROME" };
            try
            {
                foreach (var processName in processes)
                {
                    KillProcess(processName);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static void KillProcess(string processName)
        {
            var process = Process.GetProcessesByName(processName);
            foreach (var proc in
                process.Where(proc => string.Compare(proc.ProcessName, processName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                proc.Kill();
            }
        }

        [SetUp]
        protected virtual void TestInit()
        {
            Console.Out.WriteLine("Test Initialize");
        }

        [TearDown]
        protected virtual void TestCleanUp()
        {
            try
            {
                Console.Out.WriteLine("Test CleanUp");
            }
            catch (InvalidOperationException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Set environment specific test data file path
        /// </summary>
        /// <param name="dataDirectory">File directory</param>
        private void SetTestDataFilePath(string dataDirectory)
        {
            Environment = ConfigurationManager.AppSettings["TestEnvironment"];
            Console.WriteLine(ConfigurationManager.AppSettings["TestEnvironment"]);
            var dir = Path.GetDirectoryName(typeof(AutomatedBase).Assembly.Location);
            if (dir != null)
            {

                Directory.SetCurrentDirectory(dir);
            }
            else
                throw new Exception("Path.GetDirectoryName(typeof(AutomatedBase).Assembly.Location) returned null");
            Console.WriteLine(Environment);
            switch (Environment.ToUpperInvariant())
            {
                case TestEnvironment.Dev:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "LegacyDev.xml"));
                    break;
                case TestEnvironment.Qa:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "LegacyDBQA.xml"));
                    break;
                case TestEnvironment.Rpe:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "LegacyRPE.xml"));
                    break;
                default:
                    break;
            }
        }
    }
}
