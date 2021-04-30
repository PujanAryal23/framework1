using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.Support.Common
{
    class JsonParser
    {
        #region Private Variable

        private readonly ISiteDriver SiteDriver;

        #endregion

        #region Constructor

        public JsonParser(ISiteDriver siteDriver)
        {
            SiteDriver = siteDriver;
        }

        #endregion

        public JObject GetJsonResponse(string selector="pre")
        {
            return JObject.Parse(SiteDriver.FindElement(selector, How.CssSelector).Text);
        }

        public List<string> GetClaseqListFromAPI(string client, string quantity, string requestAPI)
        {
           
            SiteDriver.Open(string.Format(requestAPI, client, quantity));
            return GetJsonResponse()
                ["claimWorkList"]["claims"].Select(x => (string)x["claSeq"]).Distinct().ToList();
           
        }

    }
}
