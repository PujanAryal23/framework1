using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.Support.Enum
{

    public enum BatchQuickSearchTypeEnum
    {
        /// <summary>
        /// All Batches
        /// </summary>
        [StringValue("All Batches")] AllBatches,

        /// <summary>
        /// Incomplete Batches
        /// </summary>
        [StringValue("Incomplete Batches")] IncompleteBatches,

        /// <summary>
        /// Received this Week
        /// </summary>
        [StringValue("Received this Week")] ReceivedThisWeek,


    }


}
