using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ScoreBandEnum
    {
        [StringValue("0-499")]
        LOW = 0,
        [StringValue("500-699")]
        MODERATE = 1,
        [StringValue("700-899")]
        ELEVATED = 2,
        [StringValue("900-1000")]
        HIGH = 3
    }
}
