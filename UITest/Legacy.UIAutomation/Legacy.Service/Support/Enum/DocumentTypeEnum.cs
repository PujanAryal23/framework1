using System;
using UIAutomation.Framework.Utils;
namespace Legacy.Service.Support.Enum
{
    public enum DocumentTypeEnum
    {
        /// <summary>
        /// Documents Required
        /// </summary>
        [StringValue("Required")]
        DocRequired = 0,

        /// <summary>
        /// Documents Requested
        /// </summary>
        [StringValue("Requested")]
        DocRequested = 1,

        /// <summary>
        /// Documents Received
        /// </summary>
        [StringValue("Received")]
        DocReceived = 3
    }
}
