using UIAutomation.Framework.Utils;

namespace Legacy.Service.Support.Enum
{
    public enum FraudFinderProPageUrlEnum
    {
        [StringValue("FraudFinderPro/Product.aspx")]
        FraudFinderPro = 0,

        [StringValue("FraudFinderPro/Batchmenu.aspx")]
        Batchmenu = 1,

        [StringValue("FraudFinderPro/BatchMenu.aspx")]
        BatchMenu = 2,
        
        [StringValue("FraudFinderPro/BatchSummary.aspx")]
        BatchStatisticsReport = 3,

        [StringValue("FraudFinderPro/FlaggedClaims.aspx")]
        FlaggedClaims = 4,

        [StringValue("FraudFinderPro/ClaimHistory.aspx")]
        ClaimHistory = 5,

        
    }
}
