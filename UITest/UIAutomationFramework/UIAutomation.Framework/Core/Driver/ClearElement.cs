using System;
using System.Configuration;
using System.Threading;
using OpenQA.Selenium;
using UIAutomation.Framework.Common.Constants;
using static System.String;

namespace UIAutomation.Framework.Core.Driver
{
    public static class ClearElement
    {
        public static void ClearElementField(this IWebElement element,bool isTextarea=false)
        {
            //if (Compare(BrowserConstants.Chrome, ConfigurationManager.AppSettings["TestBrowser"].ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) == 0)
                element.Clear();

             if (Compare(BrowserConstants.Edge, ConfigurationManager.AppSettings["TestBrowser"].ToUpperInvariant(),
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                Thread.Sleep(500);
                try
                {
                    element.Click();
                    element.Click();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
                if (isTextarea)
                    element.SendKeys("1" + Keys.Control + "a" + Keys.Delete);
                else
                    element.SendKeys(Keys.Control + "a" + Keys.Delete);




            }
        }
    }
}
