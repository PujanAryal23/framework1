using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace UIAutomation.Framework.Utils
{
    public class NetworkConnection
    {
        private readonly string _serverName, _userName, _passWord, _domain;

        public NetworkConnection(string serverName, string userName, string password, string domain)
        {
            _serverName = serverName;
            _userName = userName;
            _passWord = password;
            _domain = domain;
        }

        public void ConnectToServer()
        {
            string command = @"net use " + _serverName + " /user:" + _domain + "\\" + _userName + " " + _passWord;
            ExecuteCommand(command, 5000);
        }
        public void DisConnectToServer()
        {
            string command = @"net use " + _serverName + " /del";
            ExecuteCommand(command, 5000);
        }

        public void SendCommandToTestServer(string msg)
        {
            string command = Path.GetFullPath("PsTools") + @"\PsExec.exe " + @"\\" + _serverName + " -u " + _domain + "\\" +
                _userName + " -p " + _passWord + @" cmd.exe /c C:\AutoIt\owexec.exe -c "
                + _serverName + @" -k C:\AutoIt\Automate_Save_Dialog_IE.exe -p " + msg + " -u "
                + _domain + "\\" + _userName + " -nowait";
            Console.Out.WriteLine(command);
            ExecuteCommand(command, 20000);
        }

        private static int? ExecuteCommand(string command, int timeout)
        {
            try
            {


                var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = "C:\\",
                };

                var process = Process.Start(processInfo);
                process.WaitForExit(timeout);
                var exitCode = process.ExitCode;
                process.Close();
                return exitCode;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                SendKeys.SendWait(@"{Enter}");
            }
            return null;
        }
    }
}
