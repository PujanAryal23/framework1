using System.Collections.Generic;
using System.Security.Policy;

namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Defines an interface in which intial parameters related to browser are set.
    /// </summary>
    public interface IBrowserOptions
    {
        /// <summary>
        /// Gets the URL the browser is currently displaying.
        /// </summary>
        /// <remarks>
        /// Setting the <see cref="Url"/> property will load a new web page in the current browser window. 
        /// This is done using an HTTP GET operation, and the method will block until the 
        /// load is complete. This will follow redirects issued either by the server or 
        /// as a meta-redirect from within the returned HTML. Should a meta-redirect "rest"
        /// for any duration of time, it is best to wait until this timeout is over, since 
        /// should the underlying page change while your test is executing the results of 
        /// future calls against this interface will be against the freshly loaded page. 
        /// </remarks>
        string ApplicationUrl { get; }

        /// <summary>
        /// Gets the browser to load a page.
        /// </summary>
        string Browser { get; }

        /// <summary>
        /// Gets the time till page to display.
        /// </summary>
        double PageLoadTimeout { get; }

        /// <summary>
        /// Gets the time till ajax request complete.
        /// </summary>
        double AjaxLoadTimeout { get; }

        /// <summary>
        /// Gets the time till ajax request complete.
        /// </summary>
        int ElementLoadTimeout { get; }

        /// <summary>
        /// Gets the time till ajax request complete.
        /// </summary>
        int SleepInterval { get; }

        /// <summary>
        /// Gets the list of arguments the options and switches to be use in current browser.
        /// </summary>
        string[] BrowserArguments { get; }

        void SetBrowserOptions(Dictionary<string, string> envVars);

    }
}
