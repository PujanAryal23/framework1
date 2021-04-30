using System;
using System.Collections;
using System.Management;

namespace UIAutomation.Framework.Core.Process
{
    /// <summary>
    /// A class that provides methods for operating process level task.
    /// </summary>
    internal class ProcessMethod
    {
        internal static string StartProcess(string machineName, string processPath)
        {
            var processTask = new ManagementClass(@"\\" + machineName + @"\root\CIMV2", "Win32_Process", null);
            var methodParams = processTask.GetMethodParameters("Create");
            methodParams["CommandLine"] = processPath;
            var exitCode = processTask.InvokeMethod("Create", methodParams, null);
            return TranslateProcessStartExitCode(exitCode["ReturnValue"].ToString());
        }

        /// <summary>
        /// Get an arraylist of running processes.
        /// </summary>
        /// <param name="connectionScope">An instance of ManagementScope</param>
        /// <returns></returns>
        internal static ArrayList RunningProcesses(ManagementScope connectionScope)
        {
            var alProcesses = new ArrayList();
            var msQuery = new SelectQuery("SELECT * FROM Win32_Process");
            var searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);

            foreach (ManagementObject item in searchProcedure.Get())
            {
                alProcesses.Add(item["Name"].ToString());
            }
            return alProcesses;
        }

        /// <summary>
        /// Terminate a process.
        /// </summary>
        /// <param name="connectionScope">An instance of ManagementScope</param>
        /// <param name="processName">A process name</param>
        internal static void KillProcess(ManagementScope connectionScope, string processName)
        {
            var msQuery = new SelectQuery("SELECT * FROM Win32_Process Where Name = '" + processName + "'");
            var searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                try
                {
                    item.InvokeMethod("Terminate", null);
                }
                catch (SystemException e)
                {
                    Console.WriteLine("An Error Occurred: " + e.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Translate process start and exit code.
        /// </summary>
        /// <param name="exitCode">An exit code</param>
        /// <returns></returns>
        internal static string TranslateProcessStartExitCode(string exitCode)
        {
            string code = string.Empty;
            int eCode = Convert.ToInt32(exitCode);
            switch (eCode)
            {
                case 0: code = "Successful(Completion)";
                    break;
                case 2: code = "Access(Denied)";
                    break;
                case 3: code = "Insufficient(priviledge)";
                    break;
                case 8: code = "Uknown(Failure)";
                    break;
                case 9: code = "Path(Not Found)";
                    break;
                case 21: code = "Invalid(Parameter)";
                    break;
            }
            return code;
        }
    }
}
