using System;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service
{
    public class StartNucleus : IDisposable
    {

        private IBrowserOptions _browserOptions;
        private INavigator _navigator;


        //public LoginPage StartNucleusApplication()
        //{
        //    _browserOptions = BrowserOptions.Create(DataHelper.EnviromentVariables);
        //    _navigator = new Navigator(_browserOptions);
        //    _navigator.Start(_browserOptions.ApplicationUrl);
        //    var nucleusLoginPage = _navigator.Open<LoginPageObjects>();
        //    UserType.CurrentUserType = DataHelper.EnviromentVariables["UserType"];
        //    return new LoginPage(_navigator, nucleusLoginPage);
        //}

        public void Dispose()
        {
            _navigator.Dispose();
        }

    }
}
