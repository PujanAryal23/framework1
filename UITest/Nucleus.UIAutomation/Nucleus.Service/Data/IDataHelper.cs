using Nucleus.Service.Support.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Service.Data
{
    public interface IDataHelper
    {
        void LoadTestData(string xmlPath, string testSuiteName = "", bool reload = false,string key="");
        void LoadTestMappings(string xmlPath, bool reload = false);
        Dictionary<string, string> GetTestData(string className, string methodName);
       
        Dictionary<string, string> EnviromentVariables { get;  set; }

        Dictionary<string, string> Credentials { get; set; }
        string GetSingleTestData(string className, string methodName, string paramName, string selector);

        Dictionary<string, string> GetMappingData(string className, string mappingName);
        Dictionary<string, string> GetTestData(string className, string methodName, string paramName);
    }
}
