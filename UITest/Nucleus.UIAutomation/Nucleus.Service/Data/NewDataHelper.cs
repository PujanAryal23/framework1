using System;
using System.Collections.Generic;
using System.Linq;
using EncryptionBusiness.Core;
using UIAutomation.Framework.Data.AutomationData;
using UIAutomation.Framework.Data.AutomationMapping;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Data;

namespace Nucleus.Service.Data
{
    public class NewDataHelper : IDataHelper
    {
        #region PRIVATE FIELDS

        private Tests _testData;
        private TestSuite _testSuite;
        private Mappings _mappingData;

        #endregion

        #region PUBLIC PROPERTIES

        public Dictionary<string, string> Credentials { get;  set; }

        public Dictionary<string, string> EnviromentVariables { get;  set; }

        public string EncryptionKey;

        #endregion

        #region CONSTRUCTOR

        public NewDataHelper()
        {
            EnviromentVariables = new Dictionary<string, string>();
            Credentials = new Dictionary<string, string>();
            
        }

        #endregion

        #region PUBLIC METHODS

        public void LoadTestMappings(string xmlPath, bool reload = false)
        {
            _mappingData = DataParser.Instance.Deserialize<Mappings>(xmlPath, reload);
        }

        public void LoadTestData(string xmlPath, string testSuiteName = "", bool reload = false,string key="")
        {
            EncryptionKey = key;
            _testData = DataParser.Instance.Deserialize<Tests>(xmlPath, reload);
            _testSuite = (!string.IsNullOrEmpty(testSuiteName)) ? _testData.TestSuite.Where(x => x.Name == testSuiteName).First() : _testData.TestSuite.FirstOrDefault();
            if (reload || EnviromentVariables.Count == 0)
                EnviromentVariables = _testData.EnvironmentVariables.Parameters.ToDictionary(x => x.Name, x => x.Value);
            if (reload || Credentials.Count == 0)
                Credentials = _testData.Credentials.Parameters.ToDictionary(x => x.Name, y => _testData.Credentials.IsEncrypted ? DecryptData(y.Value) : y.Value);
        }

        public Dictionary<string, string> GetTestData(string className, string methodName)
        {
            var testParameters = GetTestParameters(className, methodName);
            return testParameters == null ? null : testParameters.ToDictionary(key => key.Name, value => value.Value);
        }

        public Dictionary<string, string> GetTestData(string className, string methodName, string paramName)
        {
            var testParameters = GetTestParameters(className, methodName, paramName);
            return testParameters == null ? null : testParameters.ToDictionary(key => key.Code, value => value.Value);
        }

        public Dictionary<string, string> GetMappingData(string className, string mappingName)
        {
            var mappingParameters = GetMappingParameters(className, mappingName);
            return mappingParameters == null ? null : (mappingParameters.ToDictionary(key => key.Name, value => value.Value));
        }

        public string GetSingleTestData(string className, string methodName, string paramName, string selector)
        {
            var testParameters = GetSingleTestData(className, methodName, paramName);
            switch (selector)
            {
                case "Name":
                    return testParameters.Name;
                case "Code":
                    return testParameters.Code;
                case "Type":
                    return testParameters.Type;
                case "Client":
                    return testParameters.Client;
                case "Value":
                    return testParameters.Value;
                default:
                    throw new Exception("Expected selector values 'Name', 'Code', 'Type', 'Client', 'Value' do not match.");
            }
        }

        private UIAutomation.Framework.Data.AutomationData.Param GetSingleTestData(string className, string methodName, string paramName)
        {
            var testParameters = GetTestParameters(className, methodName, paramName);
            return testParameters == null ? null : testParameters.SingleOrDefault();
        }

        #endregion


        #region PRIVATE METHODS

        private IEnumerable<UIAutomation.Framework.Data.AutomationData.Param> GetTestParameters(string className, string methodName, string param)
        {
            var testParameters = GetTestParameters(className, methodName);
            return testParameters == null ? null : testParameters.Where(x => x.Name == param || x.Client == param || x.Type == param && !String.IsNullOrEmpty(param));
        }

        private IEnumerable<UIAutomation.Framework.Data.AutomationData.Param> GetTestParameters(string className, string methodName)
        {
            var result = (from fqc in _testSuite.FullyQualifiedClass
                          where fqc.Name.Equals(className)
                          from tm in fqc.TestMethod
                          where tm.Name.Equals(methodName)
                          select tm.Parameters).FirstOrDefault();

            return result;

        }

        private IEnumerable<UIAutomation.Framework.Data.AutomationMapping.Param> GetMappingParameters(string className, string mappingName)
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
        private string DecryptData(string value)
        {
            try
            {
                return EncryptionAlgorithm.DecryptString(value, EncryptionKey);
            }
            catch
            {
                return value;
            }
        }

        #endregion

    }
}
