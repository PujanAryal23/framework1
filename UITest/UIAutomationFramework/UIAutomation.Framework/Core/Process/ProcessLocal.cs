using System;
using System.Management;
using System.Collections;

namespace UIAutomation.Framework.Core.Process
{
    /// <summary>
    /// A  class operates on local process.
    /// </summary>
    public class ProcessLocal : IProcessObject
    {
        #region FIELDS

        readonly ConnectionOptions _options;
        readonly ManagementScope _connectionScope;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProcessLocal()
        {
            _options = ProcessConnection.ProcessConnectionOptions();
            _connectionScope = ProcessConnection.ConnectionScope(
                                     Environment.MachineName, _options);
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
