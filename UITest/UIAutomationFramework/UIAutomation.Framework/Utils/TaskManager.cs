using System;
using System.Collections.Generic;
using System.Configuration;
using UIAutomation.Framework.Core.Process;
using UIAutomation.Framework.Core.Service;
using System.Net;

namespace UIAutomation.Framework.Utils
{
    public class TaskManager
    {
        private string _testServer;
        private string _testBrowser;
        private Controller _seleniumHubService;
        private Controller _seleniumNodeService;
        private string _machineName;

        public void StartService()
        {
            _testServer = ConfigurationManager.AppSettings["TestServer"].Trim();
            _testBrowser = ConfigurationManager.AppSettings["TestBrowser"].Trim();

            if (!string.IsNullOrEmpty(_testServer))
            {
                _machineName = Dns.GetHostEntry(_testServer).HostName;

                _seleniumHubService = new Controller("Selenium Hub", _machineName);
                bool hubStatus = _seleniumHubService.IsServiceRunning() ? _seleniumHubService.RestartService() : _seleniumHubService.StartService();

                switch (_testBrowser.ToUpper())
                {
                    case "CHROME":
                        _seleniumNodeService = new Controller("Selenium Node - Google Chrome", _machineName);
                        break;
                    case "FIREFOX":
                        _seleniumNodeService = new Controller("Selenium Node - Mozilla Firefox", _machineName);
                        break;
                    default:
                        _seleniumNodeService = new Controller("Selenium Node - Internet Explorer", _machineName);
                        break;
                }

                if (hubStatus)
                    if (_seleniumNodeService.IsServiceRunning())
                        _seleniumNodeService.RestartService();
                    else _seleniumNodeService.StartService();
            }
            else
            {
                _machineName = Environment.MachineName;
                Console.Out.WriteLine("Test run in local machine : {0}", _machineName);
            }
        }

        public void StartCleanUpBrowserDrivers()
        {
            switch (_testBrowser.ToUpper())
            {
                case "CHROME":
                    CleanUpChrome();
                    break;
                default:
                    CleanUpInternetExplorer();
                    break;
            }
        }

        private void CleanUpChrome()
        {
            IList<string> processes = new List<string>() { "chromedriver.exe", "chrome.exe" };
            TerminateProcess(processes);
            Console.Out.WriteLine("Clear ChromeDriver and Chrome.");
        }

        private void CleanUpInternetExplorer()
        {
            IList<string> processes = new List<string>() { "IEDriverServer.exe", "iexplore.exe" };
            TerminateProcess(processes);
            Console.Out.WriteLine("Clear IEDriverServer and Internet Explorer.");
        }

        /// <summary>
        /// Delete all processes by name.
        /// </summary>
        /// <param name="processes"></param>
        private void TerminateProcess(IEnumerable<string> processes)
        {
            IProcessObject process;
            if (!string.IsNullOrEmpty(_testServer))
                process = new ProcessRemote(null, null, null, _machineName);
            else process = new ProcessLocal();
            try
            {
                foreach (var processName in processes)
                {
                    process.TerminateProcess(processName);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
