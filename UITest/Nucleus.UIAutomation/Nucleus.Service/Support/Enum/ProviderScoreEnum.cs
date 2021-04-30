using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ProviderScoreEnum
    {
        [StringValue("Condition Score")]
        ConditionScore = 0 ,
        
        [StringValue("Rule Engine Score")]
        RuleEngineScore = 1 ,

        [StringValue("Billing Activity Score")]
        BillingScore = 2,

        [StringValue("Specialty Comparison Score")]
        SpecialtyScore = 3,

        [StringValue("Geographic Score")]
        GeographicScore = 4,

    }
}
