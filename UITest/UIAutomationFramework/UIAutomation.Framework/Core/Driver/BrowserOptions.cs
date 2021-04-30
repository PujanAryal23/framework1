using System.Collections.Generic;
using System.Configuration;

namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Defines a class in which intial parameters related to browser are set.
    /// </summary>
    public class BrowserOptions : IBrowserOptions
    {
        private const string ApplicationUrlKey = "ApplicationUrl";
        private const string BrowserKey = "TestBrowser";
        private const string PageTimeoutKey = "PageTimeout";
        private const string AjaxTimeoutKey = "AjaxTimeout";
        private const string BrowserArgumentsKey = "BrowserArguments";
        private const string ElementTimeoutKey = "ElementTimeout";
        private const string SleepIntervalKey = "SleepInterval";

        private const int DefaultElementTimeout = 2000;
        private const int DefaultSleepInterval = 2000;
        private const double DefaultPageTimeout = 60D;
        private const double DefaultAjaxTimeout = 30D;

        private static readonly char[] Separator = new[] { ' ' };

        public virtual string ApplicationUrl { get; private set; }
        public virtual string Browser { get; private set; }
        public virtual double PageLoadTimeout { get; private set; }
        public virtual double AjaxLoadTimeout { get; private set; }
        public virtual int ElementLoadTimeout { get; private set; }
        public virtual int SleepInterval { get; private set; }
        public virtual string[] BrowserArguments { get; private set; }

        /// <summary>
        /// Creates an options related to browser.
        /// </summary>
        /// <param name="envVars">An environment variables.</param>
        /// <returns>An object of broseroptions.</returns>
        public static IBrowserOptions Create(Dictionary<string, string> envVars)
        {
            var options = new BrowserOptions();
            double tmp;
            options.ApplicationUrl = envVars.ContainsKey(ApplicationUrlKey) ? envVars[ApplicationUrlKey] : null;
            options.Browser = !string.IsNullOrEmpty(ConfigurationManager.AppSettings[BrowserKey]) ? ConfigurationManager.AppSettings[BrowserKey].ToUpperInvariant() : null;
            options.PageLoadTimeout = envVars.ContainsKey(PageTimeoutKey) ? (double.TryParse(envVars[PageTimeoutKey], out tmp) ? tmp : DefaultPageTimeout) : DefaultPageTimeout;
            options.AjaxLoadTimeout = envVars.ContainsKey(AjaxTimeoutKey) ? (double.TryParse(envVars[AjaxTimeoutKey], out tmp) ? tmp : DefaultAjaxTimeout) : DefaultAjaxTimeout;
            options.BrowserArguments = envVars.ContainsKey(BrowserArgumentsKey) ? envVars[BrowserArgumentsKey].Split(Separator) : null;
            int tmp1;
            options.ElementLoadTimeout = envVars.ContainsKey(ElementTimeoutKey) ? (int.TryParse(envVars[ElementTimeoutKey], out tmp1) ? tmp1 : DefaultElementTimeout) : DefaultElementTimeout;
            options.SleepInterval = envVars.ContainsKey(SleepIntervalKey) ? (int.TryParse(envVars[SleepIntervalKey], out tmp1) ? tmp1 : DefaultSleepInterval) : DefaultSleepInterval;
            return options;
        }

        public void SetBrowserOptions(Dictionary<string, string> envVars)
        {
            throw new System.NotImplementedException();
        }
    }
}
