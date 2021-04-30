using System;

namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Thrown when sitedriver fails to initialize webdriver.
    /// </summary>
    public class SiteDriverException : Exception
    {
        /// <summary>
        /// Creates a simple exception
        /// </summary>
        public SiteDriverException()
        {
        }

        /// <summary>
        /// Creates an exception with the specified message
        /// </summary>
        /// <param name="message">the message to add to the exception</param>
        public SiteDriverException(string message)
            : base(message)
        {
        }
    }
}
