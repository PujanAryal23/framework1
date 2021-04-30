using System.IO;
using System.Windows.Forms;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using System.Configuration;

namespace Nucleus.Service.Support.Utils
{
    public static class IEDownloadHelper
    {
        public static void ClickSaveButtonToDownloadFile(bool isRemoteServer,string testServer,string fileName)
        {
            if (isRemoteServer)
            {
                var network = new NetworkConnection(testServer.Substring(2), ConfigurationManager.AppSettings["ProxyUserName"],
                    ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]);
                network.SendCommandToTestServer(fileName);
            }
            else
            {
                SendKeys.SendWait(@"Save");
                SendKeys.SendWait(fileName);
                SendKeys.SendWait(@"{Enter}");
                SendKeys.SendWait(@"Close");
            }
        }
    }
}
