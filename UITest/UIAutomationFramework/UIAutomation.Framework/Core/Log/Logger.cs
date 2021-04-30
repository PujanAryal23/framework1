using System;
using log4net;

namespace UIAutomation.Framework.Core.Log
{
    /// <summary>
    /// Defines a class that implements <see cref="ILog"/>
    /// </summary>
    public class Logger : ILog
    {
        /// <summary>
        /// An instance of <see cref="log4net.ILog"/>
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Member of <see cref="log4net.LogManager"/>
        /// </summary>
        /// <param name="type">The object of <see cref="Type"/></param>
        public Logger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Debug(object message, Exception ex)
        {
            _log.Debug(message, ex);
        }

        public void Error(object message, Exception ex)
        {
            _log.Error(message, ex);
        }

        public void Fatal(object message, Exception ex)
        {
            _log.Fatal(message, ex);
        }

        public void Info(object message, Exception ex)
        {
            _log.Info(message, ex);
        }

        public void Warn(object message, Exception ex)
        {
            _log.Warn(message, ex);
        }
    }
}
