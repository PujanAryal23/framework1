using System;

namespace UIAutomation.Framework.Core.Log
{
    /// <summary>
    /// Defines an interface for logging mechanism.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        void Debug(object message);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        void Error(object message);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        void Fatal(object message);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        void Info(object message);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        void Warn(object message);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        /// <param name="ex">An object of <see cref="Exception"/></param>
        void Debug(object message, Exception ex);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        /// <param name="ex">An object of <see cref="Exception"/></param>
        void Error(object message, Exception ex);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        /// <param name="ex">An object of <see cref="Exception"/></param>
        void Fatal(object message, Exception ex);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        /// <param name="ex">An object of <see cref="Exception"/></param>
        void Info(object message, Exception ex);

        /// <summary>
        /// Member of <see cref="log4net.ILog"/>
        /// </summary>
        /// <param name="message">An object <see cref="message"/></param>
        /// <param name="ex">An object of <see cref="Exception"/></param>
        void Warn(object message, Exception ex);
    }
}
