using System;
using System.Management;

namespace UIAutomation.Framework.Core.Process
{
    /// <summary>
    /// Provides connection options and scope for a process.
    /// </summary>
    internal class ProcessConnection
    {
        /// <summary>
        /// Initialize process connection options.
        /// </summary>
        /// <returns></returns>
        internal static ConnectionOptions ProcessConnectionOptions()
        {
            var options = new ConnectionOptions
                              {
                                  Impersonation = ImpersonationLevel.Impersonate,
                                  Authentication = AuthenticationLevel.Default,
                                  EnablePrivileges = true
                              };
            return options;
        }

        /// <summary>
        /// Initialize process connection scope.
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static ManagementScope ConnectionScope(string machineName,ConnectionOptions options)
        {
            var connectScope = new ManagementScope
                                   {
                                       Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2"),
                                       Options = options
                                   };

            try
            {
                connectScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message.ToString());
            }
            return connectScope;
        }
    }
}
