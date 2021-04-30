using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ProviderEnum
    {
        /// <summary>
        /// All Providers
        /// </summary>
        [StringValue("All Providers")]
        AllProviders,

        /// <summary>
        /// Suspect Providers
        /// </summary>
        [StringValue("Suspect Providers")]
        SuspectProviders,

        /// <summary>
        /// Cotiviti Flagged Providers
        /// </summary>
        [StringValue("Cotiviti Flagged Providers")]
        CotivitiFlaggedProviders,

        /// <summary>
        /// Flagged Claims
        /// </summary>
        [StringValue("Client Flagged Providers")]
        ClientFlaggedProviders
    }

    public enum ClientActionEnum
    {
        /// <summary>
        /// All
        /// </summary>
        [StringValue("All")]
        All,
        /// <summary>
        /// Review
        /// </summary>
        [StringValue("Review")]
        R,

        /// <summary>
        /// Monitor
        /// </summary>
        [StringValue("Monitor")]
        Q,

        /// <summary>
        /// Deny
        /// </summary>
        [StringValue("Deny")]
        D,

       
    }

    public enum ProviderClaimSearchEnum
    {
        /// <summary>
        /// Last 12 Months
        /// </summary>
        [StringValue("Last 12 Months")]
        Last12Months,

        /// <summary>
        /// Suspect Providers
        /// </summary>
        [StringValue("All Claims")]
        AllClaims,

        
    }
}
