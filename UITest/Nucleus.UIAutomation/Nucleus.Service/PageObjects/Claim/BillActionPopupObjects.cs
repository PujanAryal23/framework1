using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Claim
{
    public class BillActionPopupObjects : NewPageBase
    {
        #region PUBLIC PROPERTIES

        public const string BillSequenceDataId = "ctl00_MainContentPlaceHolder_ClaimSequenceDataLbl";

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.BillAction.GetStringValue(); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public BillActionPopupObjects()
            : base(PageUrlEnum.BillAction.GetStringValue())
        {
        }

        #endregion
    }
}
