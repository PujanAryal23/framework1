using System;
using Legacy.Service.Data;
using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Login;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Legacy.Service.Support.Utils.Extensions;


namespace Legacy.Service.PageServices.Login
{
  public class LoginPage : BasePageService
  {
    private  LoginPageObjects _loginPage;
    
    public LoginPage(INavigator navigator, LoginPageObjects loginPage)
      :base(navigator, loginPage)
    {
      // Just for performance!
      _loginPage = (LoginPageObjects) PageObject;
    }

    public WelcomePage Login()
    {
        if (CurrentPageTitle == Extensions.GetStringValue(PageTitleEnum.WelcomePage))
        {
            var checkWelcomePage = new WelcomePage(Navigator, Navigator.Navigate<WelcomePageObjects>(() => { }));
            checkWelcomePage.Logout();
        }
        Console.WriteLine("Logging in with  UserName = {0}", DataHelper.Credentials["Username"]);
        var welcomePage = Navigator.Navigate<WelcomePageObjects>
        (() =>
           {
             _loginPage.UserName.SetText(DataHelper.Credentials["Username"]);
             _loginPage.Password.SetText(DataHelper.Credentials["Password"]);
             _loginPage.BtnLogin.Click();
           });
      return new WelcomePage(Navigator, welcomePage);
    }

    public void LoginWithInvalidUser(string invalidUserName , string invalidPassword)
    {
        Console.WriteLine("Logging in with  UserName = {0}", invalidUserName);
        Navigator.Navigate<LoginPageObjects>
            (() =>
                 {
                     _loginPage.UserName.SetText(invalidUserName);
                     _loginPage.Password.SetText(invalidPassword);
                     _loginPage.BtnLogin.Click();
                 });
    }

      public string GetUnSupportedBrowserMessage()
      {
          return SiteDriver.FindElement<TextLabel>(LoginPageObjects.UnSupportedBrowserCssLocator, How.CssSelector).Text;
      }

      public bool IsCurrentBrowserVersionIE9()
      {
          var val = JavaScriptExecutor.Execute("return navigator.userAgent").ToString();
          return val.Substring(val.IndexOf("Trident/", StringComparison.Ordinal) + 8, 1) == "5";
      }
  }
}
