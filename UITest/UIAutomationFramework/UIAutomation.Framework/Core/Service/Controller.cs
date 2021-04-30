using System;
using System.ServiceProcess;
using System.Threading;

namespace UIAutomation.Framework.Core.Service
{
    /// <summary>
    /// This class provides the control system for starting, stopping and restarting selenium hub and node services.
    /// </summary>
    public class Controller
    {
        #region PRIVATE FIELDS

        private const int RestartTimeout = 10000;

        private readonly ServiceController _service;

        private readonly string _machineName;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Controller constructor as default without service name.
        /// </summary>
        /// <param name="serviceName"></param>
        public Controller(string serviceName)
        {
            _service = new ServiceController(serviceName);
        }

        /// <summary>
        /// Contorller constructor having service and  machine name.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="machineName"></param>
        public Controller(string serviceName, string machineName)
        {

            _machineName = machineName;
            _service = new ServiceController(serviceName, _machineName);
        }

        #endregion

        #region PUBIC METHODS

        /// <summary>
        /// Confirm whether service is running or not.
        /// </summary>
        /// <returns></returns>
        public bool IsServiceRunning()
        {
            return _service.Status == ServiceControllerStatus.Running;
        }

        /// <summary>
        /// Start service.
        /// </summary>
        /// <returns></returns>
        public bool StartService()
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Stopped)
                {
                    _service.Refresh();
                    _service.Start();
                    Console.Out.WriteLine("{0} ---> started.",_service.DisplayName);
                    return true;
                }
                Console.Out.WriteLine("{0} ---> already started.", _service.DisplayName);

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString(), @"Error Starting Service.");
            }
            return false;
        }

        /// <summary>
        /// Stop service.
        /// </summary>
        /// <returns></returns>
        public bool StopService()
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Running)
                {
                    _service.Stop();
                    Console.Out.WriteLine("{0} ---> stopped.",_service.DisplayName);
                    return true;
                }
                Console.Out.WriteLine("{0} ---> already stopped.", _service.DisplayName);

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString(), @"Error Stopping Service.");
            }
            return false;
        }

        /// <summary>
        /// Restart service.
        /// </summary>
        /// <returns></returns>
        public bool RestartService()
        {
            try
            {
                int milliSecBefore = Environment.TickCount;
                TimeSpan timeOut = TimeSpan.FromMilliseconds(RestartTimeout);

                _service.Stop();
                _service.WaitForStatus(ServiceControllerStatus.Stopped, timeOut);
                int milliSecAfter = Environment.TickCount;
                timeOut = TimeSpan.FromMilliseconds(RestartTimeout - (milliSecAfter - milliSecBefore));

                _service.Start();
                _service.WaitForStatus(ServiceControllerStatus.Running, timeOut);
                Console.Out.WriteLine("{0} ---> restarted", _service.DisplayName);
                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString(), @"Error Restarting Service.");
            }
            return false;
        }
        #endregion
    }
}
