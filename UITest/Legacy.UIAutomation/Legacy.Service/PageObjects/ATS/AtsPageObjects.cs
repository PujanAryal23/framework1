using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.ATS
{
  public class AtsPageObjects : DefaultPageObjects
  {
    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_MainMenu.jpg')]")]
    public ImageButton WelcomePageBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_ATS_Dark.jpg')]")]
    public ImageButton AtsBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Search.jpg')]")]
    public ImageButton SearchBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Review.jpg')]")]
    public ImageButton ReviewListBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Assign.jpg')]")]
    public ImageButton AssignmentsBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Category_Codes.jpg')]")]
    public ImageButton CategoryCodesBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_HCI_Consultant.jpg')]")]
    public ImageButton HciConsultantBtn;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
    public ImageButton BackBtn;

    public AtsPageObjects()
      : base("ATS/ATS.aspx")
    {
    }
  }
}
