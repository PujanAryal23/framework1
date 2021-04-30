using System;
using System.Configuration;
using Legacy.Service.Data;
using Legacy.Service.PageObjects.Login;
using Legacy.Service.PageServices.Login;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Legacy.Service
{
    public class StartLegacy : IDisposable
    {

        private Navigator _navigator;
        private IBrowserOptions _browserOptions;
        public static ProductEnum Product { get; set; }
        public static string PreAuthorizationProduct { get; set; }

        private readonly TaskManager _taskManager;

        public StartLegacy()
        {
            _taskManager = new TaskManager();
        }

        public LoginPage StartLegacyApplication()
        {
            _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
            _navigator = new Navigator(_browserOptions);
            _navigator.Start(_browserOptions.ApplicationUrl);
            var legacyLoginPage = _navigator.Open<LoginPageObjects>();
            return new LoginPage(_navigator, legacyLoginPage);
        }


        public  void StartService()
        {
            _taskManager.StartService();
        }

        public void StartCleanUpBrowserDrivers()
        {
           _taskManager.StartCleanUpBrowserDrivers();
        }
        
        public void Dispose()
        {
            _navigator.Dispose();
        }
    }
}
