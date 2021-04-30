using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Password
{
    public class ChangePasswordPageObjects : DefaultPageObjects
    {
        [FindsBy(How = How.XPath, Using = "//input[contains(@src, '_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        public ChangePasswordPageObjects()
            : base("ChangePassword.aspx")
        {
        }
    }
}
