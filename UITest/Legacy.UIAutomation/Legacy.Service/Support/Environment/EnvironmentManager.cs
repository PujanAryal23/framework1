using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Legacy.Service.Support.Environment
{
    [Serializable]
    public class EnvironmentManager
    {
        #region PRIVATE FIELDS

        private static readonly EnvironmentManager instance = new EnvironmentManager();

        #endregion
      
        #region PUBIC PROPERTIES

        public static EnvironmentManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region CONSRUCTOR


        #endregion

        #region ENVIRONMENT VARIABLES

        public string Browser
        {
            get { return ConfigurationManager.AppSettings["TestBrowser"].ToUpperInvariant(); }
        }

        #endregion
      
    }
}
