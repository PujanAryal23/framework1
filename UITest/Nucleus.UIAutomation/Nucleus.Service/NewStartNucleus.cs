using System;
using System.Data;
using System.Threading;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service
{
    public class NewStartNucleus : IDisposable,IStartNucleus
    {

        private IBrowserOptions _browserOptions;
        private INewNavigator _navigator;
        private ISiteDriver _siteDriver;
        private IJavaScriptExecutors _javaScriptExecutors;
        private IOracleStatementExecutor _executor;
       
        public NewStartNucleus(INewNavigator navigator,IJavaScriptExecutors javaScriptExecutors,IBrowserOptions browserOptions, IOracleStatementExecutor executor)
        {
            _navigator = navigator;
            _javaScriptExecutors = javaScriptExecutors;
            _browserOptions = browserOptions;
            _executor = executor;
        }


        public LoginPage StartNucleusApplication(IEnvironmentManager environmentManager,IDataHelper dataHelper)
        {
            _browserOptions.SetBrowserOptions(dataHelper.EnviromentVariables);
            _navigator.Start(_browserOptions);
            Thread.Sleep(3000);
            var nucleusLoginPage = _navigator.Open<LoginPageObjects>();
            UserType.CurrentUserType = dataHelper.EnviromentVariables["UserType"];
            _siteDriver = _navigator.SiteDriver;
            _javaScriptExecutors.Init(_siteDriver);
            return new LoginPage(_navigator, nucleusLoginPage,_siteDriver,_javaScriptExecutors, environmentManager, _browserOptions, _executor);
        }

        public void Dispose()
        {
            _navigator.Dispose();
        }

        public void CloseDatabaseConnection()
        {
            _executor.CloseConnection();
        }

    }
}