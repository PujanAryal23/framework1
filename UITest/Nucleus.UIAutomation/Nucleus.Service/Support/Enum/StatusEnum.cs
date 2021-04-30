using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{

    public enum EditStatusEnum
    {
        [StringValue("On")]
        On,

        [StringValue("Off")]
        Off,

        [StringValue("Reporting")]
        Reporting,

        [StringValue("All")]
        All,

        [StringValue("Prototype")]
        Prototype
    }

    public enum EditReviewStatus
    {
        [StringValue("Auto approve")]
        AutoApprove, 

        [StringValue("Client Review")]
        ClientReview,

        [StringValue("Internal Review")]
        InternalReview
    }
    
    /// <summary>
    /// Appeal Status's 
    /// </summary>
    public enum StatusEnum
    {
        [StringValue("", "None")]
        None,

        [StringValue("N", "New")]
        New,

        [StringValue("R", "Documents Requested")]
        DocumentsRequested,

        [StringValue("U", "Cotiviti Unreviewed")]
        CotivitiUnreviewed,

        [StringValue("B", "Documents Required")]
        DocumentsRequired,

        [StringValue("C", "Closed")]
        Closed,

        [StringValue("V", "Client Unreviewed")]
        ClientUnreviewed,

        [StringValue("W", "Cotiviti Consultant Required")]
        CotivitiConsultantRequired,

        [StringValue("K", "Cotiviti Consultant Complete")]
        CotivitiConsultantComplete,

        [StringValue("A", "State Consultant Required")]
        StateConsultantRequired,

        [StringValue("S", "State Consultant Complete")]
        StateConsultantComplete,

        [StringValue("D", "Document Review")]
        DocumentReview,

    }

    public enum UserProfileStatusEnum
    {
        [StringValue("Active")]
        Active,

        [StringValue("Inactive")]
        Inactive,

        [StringValue("Frozen")]
        Frozen,

        [StringValue("Locked")]
        Locked,
    }

}
