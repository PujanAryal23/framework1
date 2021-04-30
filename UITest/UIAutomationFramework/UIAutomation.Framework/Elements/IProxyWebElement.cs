using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace UIAutomation.Framework.Elements
{
    interface IProxyWebElement
    {
        IWebElement ProxyWebElement { get; }
    }
}
