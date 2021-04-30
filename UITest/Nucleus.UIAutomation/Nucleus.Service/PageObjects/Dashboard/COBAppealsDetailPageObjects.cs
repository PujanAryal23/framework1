using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Dashboard
{
    public class COBAppealsDetailPageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR

        public COBAppealsDetailPageObjects()
            : base(PageUrlEnum.COBAppealsDetail.GetStringValue())
        {
        }

        public COBAppealsDetailPageObjects(string url)
            : base(url)
        {
        }

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.COBAppealsDetail.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region FIELDS

        public const string COBAppealsDetailPageheaderXPath = "//div[@id='analytics_header']/div/label";

        #endregion
    }
}
