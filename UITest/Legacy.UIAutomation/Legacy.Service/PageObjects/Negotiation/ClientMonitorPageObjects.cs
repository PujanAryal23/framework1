using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Negotiation
{
  public class ClientMonitorPageObjects : DefaultPageObjects
  {
    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Back.jpg')]")]
    public ImageButton BackBtn;

    public ClientMonitorPageObjects()
      : base("Negotiation/ClientMonitor.aspx")
    {
    }
  }
}
