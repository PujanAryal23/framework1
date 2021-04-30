﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nucleus.Service;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.PageServices.SwitchClient;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using Unity;

namespace Nucleus.UIAutomation.TestSuites.Base
{
    public abstract class AutomatedBaseClient
    {
        protected IStartNucleus StartFlow;
        protected IDataHelper DataHelper;
        protected IEnvironmentManager EnvironmentManager;
        protected LoginPage Login;
        protected QuickLaunchPage QuickLaunch;
        protected NewDefaultPage CurrentPage;
        protected SwitchClientPage SwitchClient;
        protected AppealSearchPage AppealSearch;
        protected ProfileManagerPage ProfileManager;
        protected NewBasePageService BasePage;

        protected string Environment;
        protected string TestDataFilePath;
        protected string TestMappingFilePath;
        protected int UserLoginIndex = 0;
        protected const string UnAuthorizedMessage =
            "It appears that you do not have the necessary authority to view this page.";
        protected virtual String FullyQualifiedClassName { get; set; }


        [OneTimeSetUp]
        protected virtual void ClassInit()
        {
            StringFormatter.PrintMessageTitle("Class Initializing");
            BootStrapper.Init();
            EnvironmentManager = DependencyInjector.Retrieve<NewEnvironmentManager>();
            SetTestDataFilePath();
            TestMappingFilePath = @".\Data\Mapping.xml";
            EnvironmentManager.EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            DataHelper = DependencyInjector.Retrieve<NewDataHelper>();
            DataHelper.LoadTestData(TestDataFilePath, key: EnvironmentManager.EncryptionKey);
            DataHelper.LoadTestMappings(TestMappingFilePath);
            EnvironmentManager.Init(DataHelper.Credentials, DataHelper.EnviromentVariables);
            StartFlow = DependencyInjector.Retrieve<NewStartNucleus>();
            Login = StartFlow.StartNucleusApplication(EnvironmentManager, DataHelper);
            //Login.ClickOnCloseButton();
            switch (UserLoginIndex)
            {
                case 1:
                    BasePage = CurrentPage = QuickLaunch = Login.LoginAsClientUserWithClaimViewRestriction();
                    break;
                case 2:
                    BasePage = CurrentPage = QuickLaunch = Login.LoginAsClientUser4();
                    break;
                case 3:
                    BasePage = CurrentPage = QuickLaunch = Login.LoginAsClientMstrUser();
                    break;
                default:
                    BasePage = CurrentPage = QuickLaunch = Login.LoginAsClientUser();
                    break;
            }
            TestExtensions.DefaultPage = CurrentPage;

            //CheckTestClientAndSwitch();
        }

        protected void CheckTestClientAndSwitch()
        {
            
                if (!QuickLaunch.IsDefaultTestClientForEmberPage(
                       EnvironmentManager.TestClient))
                {
                    CurrentPage.ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);
                }
            
        }

       

        [OneTimeTearDown]
        protected virtual void ClassCleanUp()
        {
            try
            {
                CurrentPage.Logout();
            }
            finally
            {
                StartFlow.Dispose();
            }
            //CleanupBrowserDriver();
            StringFormatter.PrintMessageTitle("Class CleanUp");
        }

        [SetUp]
        protected virtual void TestInit()
        {
            StringFormatter.PrintMessageTitle("Test Initialize");
        }

        [TearDown]
        protected virtual void TestCleanUp()
        {
            try
            {
                StringFormatter.PrintMessageTitle("Test CleanUp");
            }
            catch (InvalidOperationException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        private void SetTestDataFilePath()
        {
            Environment = ConfigurationManager.AppSettings["TestEnvironment"];
            const string dataDirectory = @".\Data\";
            var dir = Path.GetDirectoryName(typeof(AutomatedBaseClient).Assembly.Location);
            if (dir != null)
            {

                Directory.SetCurrentDirectory(dir);
            }
            else
                throw new Exception("Path.GetDirectoryName(typeof(AutomatedBaseClient).Assembly.Location) returned null"); 
            switch (Environment.ToUpperInvariant())
            {
                case TestEnvironment.Dev:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "Dev.xml"));
                    break;
                case TestEnvironment.Qa:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "QA.xml"));
                    break;
                case TestEnvironment.Uat:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "UAT.xml"));
                    break;
                case TestEnvironment.QaRel:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "QAREL.xml"));
                    break;
                case TestEnvironment.Dev3:
                    TestDataFilePath = Path.GetFullPath(string.Concat(dataDirectory, "Dev3.xml"));
                    break;
                default:
                    break;
            }
        }

        private static void CleanupBrowserDriver()
        {
            IList<string> processes = new List<string>() { "IEDRIVERSERVER", "CHROMEDRIVER"};
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
        protected void VerifyIfUrlIsUnauthorized(string currentUrl)
        {
            currentUrl.ToLower().Contains("unauthorized").ShouldBeTrue("Is UnAuthorized Page Open?");
            QuickLaunch.GetUnAuthorizedMessage().Replace("\r\n", " ").ShouldBeEqual(UnAuthorizedMessage,
                "Is UnAuthorized Page Message Equal?");

        }

        protected bool IsUrlUnauthorized(string currentUrl)
        {
            return currentUrl.ToLower().Contains("unauthorized");


        }

        protected void VerifyUrlIsAuthorized(string currentUrl)
        {
            currentUrl.ToLower().Contains("unauthorized").ShouldBeFalse("Is UnAuthorized Page Open?");


        }

    }
}
