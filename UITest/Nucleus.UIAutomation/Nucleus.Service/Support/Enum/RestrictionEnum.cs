using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    /// <summary>
    /// Appeal Types
    /// </summary>
    public enum RestrictionGroup
    {
        /// <summary>
        /// Restriction offshore
        /// </summary>
        [StringValue("Offshore")]
        AllUser,

        /// <summary>
        /// Record Review Appeal Type
        /// </summary>
        [StringValue("A")]
        InternalUser,

        /// <summary>
        /// Record Review Appeal Type
        /// </summary>
        [StringValue("B")]
        ClientUser,
    }
}
