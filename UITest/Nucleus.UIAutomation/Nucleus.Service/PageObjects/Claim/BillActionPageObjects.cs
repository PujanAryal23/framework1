using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.Service.PageObjects.Claim
{
    public class BillActionPageObjects : NewDefaultPageObjects
    {


        public const string ClaSeqColumnCssLocator = "section.claim_action > div > ul:nth-of-type(1) > div > div:nth-of-type(1) > label";
       

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.BillAction2.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public BillActionPageObjects()
            : base( PageUrlEnum.ClaimAction.GetStringValue())
        {
        }

        #endregion
    }
}
