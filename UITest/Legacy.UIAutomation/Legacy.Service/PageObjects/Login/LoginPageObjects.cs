using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageObjects.Login
{
  public class LoginPageObjects : PageBase
  {
      #region CSS

      public const string UnSupportedBrowserCssLocator = "span#BrowserText";
      #endregion
      #region PAGEOBJECTS PROPERTIES


      [FindsBy(How = How.Id, Using = "UserId")]
      public TextField UserName;

      [FindsBy(How = How.Id, Using = "Password")]
      public TextField Password;

      [FindsBy(How = How.Id, Using = "BtnLogin")]
      public ImageButton BtnLogin;

      #endregion

      #region CONSTRUCTOR

      public LoginPageObjects()
          :base("")
      {
          
      }
      
      #endregion

      #region OVERRIDE PROPERTIES

      public override string PageTitle
      {
          get { return PageTitleEnum.Login.GetStringValue(); }
      }

      #endregion

  }
}
