using System.Collections;

namespace UIAutomation.Framework.Core.Process
{
    /// <summary>
    /// An interface that provides process level operation.
    /// </summary>
    public interface IProcessObject
    {
        /// <summary>
        /// Get list of running processes.
        /// </summary>
        /// <returns></returns>
        ArrayList RunningProcesses();

        /// <summary>
        /// Terminate process.
        /// </summary>
        /// <param name="processName"></param>
        void TerminateProcess(string processName);
    }
}
