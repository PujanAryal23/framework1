using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Text.RegularExpressions;
using static System.Console;
using static System.String;

namespace UIAutomation.Framework.Utils
{
    public class EmailReader
    {
        private static TcpClient _tcpc = null;
        private static SslStream _ssl = null;
        static int bytes = -1;
        static byte[] buffer;
        static byte[] data;

        public EmailReader()
        {
            InitializeConnection();
        }

        public static void InitializeConnection()
        {
            _tcpc = new TcpClient("outlook.office365.com", 993);
            _ssl = new SslStream(_tcpc.GetStream());
            _ssl.AuthenticateAsClient("outlook.office365.com");
            Response("");
        }

        public string AuthenticateUser(string username, string password)
        {
            return Response("$ LOGIN " + username + " " + password + "\r\n");
        }

        public void SelectInbox()
        {
             Response("$ SELECT INBOX\r\n");            
        }
        
        public int GetMailCount()
        {
            var res = Response("$ STATUS INBOX (MESSAGES)\r\n");
            Match m = Regex.Match(res, "[0-9]*[0-9]");
            return Convert.ToInt32(m.ToString());
        }

        public string GetMailSubject(int index)
        {
            return Response("$ FETCH " + index + " (body[header.fields (subject)])\r\n");
        }

        public string GetMailFrom(int index)
        {
            Response("$ FETCH " + index + " (body[header.fields (from)])\r\n");
            return Response("");
        }

        public string GetMailDate(int index)
        {
             var res = Response("$ FETCH " + index + " (body[header.fields (date)])\r\n");
            res = Response("");
            Match m = Regex.Match(res, "\\w+,\\s\\d\\s\\w+\\s\\d+");
             var date = Convert.ToString(m);
             return date;
        }

        public string GetMailHeader(int index)
        {
            return Response("$ FETCH " + index + " (body[header.fields (from date subject)])\r\n");
     
        }


        public void CloseConnection()
        {
            Response("$ LOGOUT\r\n");
            
            if (_ssl != null)
            {
                _ssl.Close();
                _ssl.Dispose();
            }
            if (_tcpc != null)
            {
                _tcpc.Close();
            }
        }




        private static string Response(string command)
        {
            try
            {
                if (command != "")
                {
                    if (_tcpc.Connected)
                    {
                        data = Encoding.ASCII.GetBytes(command);
                        _ssl.Write(data, 0, data.Length);
                    }
                    else
                    {
                        Console.WriteLine("TCP CONNECTION DISCONNECTED");
                    }
                }
                _ssl.Flush();


                buffer = new byte[_tcpc.ReceiveBufferSize];
                bytes = _ssl.Read(buffer, 0, 2048);
                return Encoding.ASCII.GetString(buffer).TrimEnd();
                          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
