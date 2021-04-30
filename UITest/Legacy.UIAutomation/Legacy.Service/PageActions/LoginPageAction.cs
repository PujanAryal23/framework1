using Legacy.Service.Data;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;


namespace Legacy.Service.PageActions
{
  public class LoginPageAction : BasePageAction
  {
    private  LoginPage _loginPage;
    
    public LoginPageAction(INavigator navigator, LoginPage loginPage)
      :base(navigator, loginPage)
    {
      // Just for performance!
      _loginPage = (LoginPage) _pageObject;
    }

    public WelcomePageAction Login()
    {
      var welcomePage = Navigator.Navigate<WelcomePage>
        (() =>
           {
             _loginPage.UserName.SetText(DataHelper.EnviromentVariables["Username"]);
             _loginPage.Password.SetText(DataHelper.EnviromentVariables["Password"]);
             _loginPage.BtnLogin.Click();
           });
      return new WelcomePageAction(Navigator, welcomePage);
    }
  }
}
