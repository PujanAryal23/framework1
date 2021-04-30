using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Claim
{
    public class ClaimActionPopupObjects : NewPageBase
    {
        #region PUBLIC PROPERTIES

        public const string ClaimSequenceDataId = "ctl00_MainContentPlaceHolder_ClaimSequenceDataLbl";

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ClaimAction.GetStringValue(); }
        }

        //private static string _PageUrl;

        //public static string Pageurl
        //{
        //    set { _PageUrl = value; }
        //}

        #endregion

        #region CONSTRUCTOR

        public ClaimActionPopupObjects()
            : base(PageUrlEnum.ClaimAction.GetStringValue())
        {
        }


        #endregion
    }
}
