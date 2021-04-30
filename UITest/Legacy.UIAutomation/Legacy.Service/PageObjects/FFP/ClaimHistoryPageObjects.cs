using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;

namespace Legacy.Service.PageObjects.FFP
{
    public class ClaimHistoryPageObjects : PageBase
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.ClaimHistory.GetStringValue(),StartLegacy.Product.GetStringValue()); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public ClaimHistoryPageObjects()
            : base(FraudFinderProPageUrlEnum.ClaimHistory.GetStringValue())
        {

        }

        #endregion
    }
}
