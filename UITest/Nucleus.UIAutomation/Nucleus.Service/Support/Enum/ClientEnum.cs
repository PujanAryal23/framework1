using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ClientEnum 
    {
        [StringValue("Public Employee Health Plans")]
        PEHP = 0,

        [StringValue("Reserved for Automated Tests (Keep Out!)")]
        RPE = 1,

        [StringValue("TallTree Administrator","Talltree")]
        TTREE = 2,

        [StringValue("EBMC")]
        EBMC = 3,

        [StringValue("Reserved for Automated Tests (Keep Out!)")]
        UITST = 4,

        [StringValue("Coventry","CVTY")]
        CVTY = 54,

        [StringValue("Selenium Test Client")]
        SMTST = 6,

        [StringValue("HCSC")]
        HCSC = 7,

        [StringValue("CTNSC")]
        CTNSC = 8,

        [StringValue("Reserved for Automated Tests (Keep Out!)")]
        LOADT = 9,

        [StringValue("NAT1")]
        NAT1 = 10

    }


    public enum ClientSettingsTabEnum
    {
        [StringValue("General")]
        General = 0,
        [StringValue("Configuration")]
        Configuration =1,
        [StringValue("Workflow")]
        WorkFlow =2,
        [StringValue("Security")]
        Security =3,
        [StringValue("Product/Appeals")]
        Product=4,
        [StringValue("Custom Fields")]
        Custom=5,
        [StringValue("Interop")]
        Interop=6,


    }
}
