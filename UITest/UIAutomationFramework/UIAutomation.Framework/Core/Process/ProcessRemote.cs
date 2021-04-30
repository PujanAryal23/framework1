using System.Collections;
using System.Management;

namespace UIAutomation.Framework.Core.Process
{
    /// <summary>
    /// A  class operates on remote process.
    /// </summary>
    public class ProcessRemote : IProcessObject
    {
        #region FIELDS

        private string _userName;
        private string _password;
        private string _domain;
        private string _machineName;
        private readonly ManagementScope _connectionScope;
        private readonly ConnectionOptions _options;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="machineName"></param>
        public ProcessRemote(string userName,string password,string domain,string machineName)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
            _machineName = machineName;
            _options = ProcessConnection.ProcessConnectionOptions();
            if (domain != null || userName != null)
            {
                _options.Username = domain + "\\" + userName;
                _options.Password = password;
            }
            _connectionScope = ProcessConnection.ConnectionScope(machineName, _options);
        }

        #endregion

        #region POLYMORPHIC METHODS

        /// <summary>
        /// Get an arraylist of running processes.
        /// </summary>
        /// <returns></returns>
        public ArrayList RunningProcesses()
        {
            return ProcessMethod.RunningProcesses(_connectionScope);
        }

        /// <summary>
        /// Terminate a specified process.
        /// </summary>
        /// <param name="processName"></param>
        public void TerminateProcess(string processName)
        {
            ProcessMethod.KillProcess(_connectionScope, processName);
        }

        #endregion
    }
}
