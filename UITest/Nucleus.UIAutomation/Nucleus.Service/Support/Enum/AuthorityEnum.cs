using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum AuthorityAssignedEnum
    {
        /// <summary>
        /// Appeal Type
        /// </summary>
        [StringValue("Batch")]
        Batch = 0,
        
        [StringValue("Modify auto-reviewed flags.")]
        ModifyAutoReviewedFlags,
        
        [StringValue("HCI User can Add edits to Client History Claims.")]
        HCIUserCanAddEditsToClientHistoryClaims,

        [StringValue("Suspect Provider Work List")]
        SuspectProviderWorkList ,

        [StringValue("Release Claims")]
        ReleaseClaims ,

        [StringValue("Reports")]
        Reports,

        [StringValue("User maintenance")]
        UserMaintenance,

        [StringValue("Appeal Manager")]
        AppealManager,

        [StringValue("QA Manager")]
        QaManager,

        [StringValue("Product Dashboards")]
        ProductDashboards,

        [StringValue("My Dashboard")]
        MyDashboard,

        [StringValue("DCA Work List")]
        DCIWorkList,

        [StringValue("DCA Pre-authorization")]
        DCIPreAuthorization,

        [StringValue("Client maintenance")]
        ClientMaintenance,

        [StringValue("Product maintenance")]
        ProductMaintenance,

        [StringValue("CV QA Audit")]
        PciQaAudit

       


    }


}
