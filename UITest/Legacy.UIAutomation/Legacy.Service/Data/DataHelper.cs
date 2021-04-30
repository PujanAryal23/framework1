using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EncryptionBusiness;
using EncryptionBusiness.Core;
using UIAutomation.Framework.Data.AutomationData;
using UIAutomation.Framework.Data.AutomationMapping;
using UIAutomation.Framework.Data;

namespace Legacy.Service.Data
{
    public static class DataHelper
    {
        #region PRIVATE FIELDS

        private static Tests _testData;

        private static TestSuite _testSuite;

        private static Mappings _mappingData;

        #endregion

        #region PUBLIC PROPERTIES

        public static Dictionary<string, string> Credentials { get; private set; }

        public static Dictionary<string, string> EnviromentVariables { get; private set; }

        #endregion

        #region CONSTRUCTOR

        static DataHelper()
        {
            EnviromentVariables = new Dictionary<string, string>();
            Credentials = new Dictionary<string, string>();
        }

        #endregion

        #region PUBLIC METHODS

        public static void LoadTestMappings(string xmlPath, bool reload = false)
        {
            _mappingData = DataParser.Instance.Deserialize<Mappings>(xmlPath, reload);
        }

        public static void LoadTestData(string xmlPath, string testSuiteName = "", bool reload = false)
        {
            _testData = DataParser.Instance.Deserialize<Tests>(xmlPath, reload);
            _testSuite = (!string.IsNullOrEmpty(testSuiteName)) ? _testData.TestSuite.Where(x => x.Name == testSuiteName).First() : _testData.TestSuite.FirstOrDefault();
            if (reload || EnviromentVariables.Count == 0)
                EnviromentVariables = _testData.EnvironmentVariables.Parameters.ToDictionary(x => x.Name, x => x.Value);
            if (reload || Credentials.Count == 0)
                Credentials = _testData.Credentials.Parameters.ToDictionary(x => x.Name, y => _testData.Credentials.IsEncrypted ? DecryptData(y.Value) : y.Value);
        }

        public static Dictionary<string, string> GetTestData(string className, string methodName)
        {
            var testParameters = GetTestParameters(className, methodName);
            return testParameters == null ? null : testParameters.ToDictionary(key => key.Name, value => value.Value);
        }

        public static Dictionary<string, string> GetTestData(string className, string methodName, string paramName)
        {
            var testParameters = GetTestParameters(className, methodName, paramName);
            return testParameters == null ? null : testParameters.ToDictionary(key => key.Code, value => value.Value);
        }

        public static Dictionary<string, string> GetMappingData(string className, string mappingName)
        {
            var mappingParameters = GetMappingParameters(className, mappingName);
            return mappingParameters == null ? null : (mappingParameters.ToDictionary(key => key.Name, value => value.Value));
        }

        public static string GetSingleTestData(string className, string methodName, string paramName)
        {
            var testParameters = GetTestParameters(className, methodName, paramName);
            return testParameters == null ? null : testParameters.SingleOrDefault().Value;
        }

        #endregion


        #region PRIVATE METHODS

        private static IEnumerable<UIAutomation.Framework.Data.AutomationData.Param> GetTestParameters(string className, string methodName, string param)
        {
            var testParameters = GetTestParameters(className, methodName);
            return testParameters == null ? null : testParameters.Where(x => x.Name == param || x.Client == param || x.Type == param && !String.IsNullOrEmpty(param));
        }

        private static IEnumerable<UIAutomation.Framework.Data.AutomationData.Param> GetTestParameters(string className, string methodName)
        {
            var result = (from fqc in _testSuite.FullyQualifiedClass
                          where fqc.Name.Equals(className)
                          from tm in fqc.TestMethod
                          where tm.Name.Equals(methodName)
                          select tm.Parameters).FirstOrDefault();

            return result;

        }

        private static IEnumerable<UIAutomation.Framework.Data.AutomationMapping.Param> GetMappingParameters(string className, string mappingName)
        {
            var result = (from fqc in _mappingData.FullyQualifiedClass
                          where fqc.Name.Equals(className)
                          from m in fqc.Mapping
                          where m.Name.Equals(mappingName)
                          select m.Parameters).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Decrypt encrypted data to plain text using encryption key
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string DecryptData(string value)
        {
            try
            {
                return EncryptionAlgorithm.DecryptString(value, ConfigurationManager.AppSettings["EncryptionKey"]);
            }
            catch
            {
                return value;
            }
        }

        #endregion

    }
}
