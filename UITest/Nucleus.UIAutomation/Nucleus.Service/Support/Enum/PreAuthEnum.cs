using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum PreAuthQuickSearchEnum
    {
        /// <summary>
        /// All Pre-Auths
        /// </summary>
        [StringValue("All Pre-Auths")]
        AllPreAuths,

        /// <summary>
        /// Outstanding Pre-Auths
        /// </summary>
        [StringValue("Outstanding Pre-Auths")]
        OutstandingPreAuths,

        /// <summary>
        /// Consultant Review
        /// </summary>
        [StringValue("Consultant Review")]
        ConsultantReview,
    }

    public enum PreAuthQuickSearchClientEnum
    {
        /// <summary>
        /// All Pre-Auths
        /// </summary>
        [StringValue("All Pre-Auths")]
        AllPreAuths,

        /// <summary>
        /// Document Needed
        /// </summary>
        [StringValue("Documents Needed")]
        DocumentsNeeded,

        
    }
}
