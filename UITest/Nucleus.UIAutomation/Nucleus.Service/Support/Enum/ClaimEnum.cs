using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ClaimQuickSearchTypeEnum
    {
        /// <summary>
        /// All Claims
        /// </summary>
        [StringValue("All Claims")]
        AllClaims,

        /// <summary>
        /// Unreviewed Claims
        /// </summary>
        [StringValue("Unreviewed Claims")]
        UnreviewedClaims,

        /// <summary>
        /// Pended Claims
        /// </summary>
        [StringValue("Pended Claims")]
        PendedClaims,

        /// <summary>
        /// Flagged Claims
        /// </summary>
        [StringValue("Flagged Claims")]
        FlaggedClaims,

        /// <summary>
        /// Flagged Claims
        /// </summary>
        [StringValue("Outstanding QC Claims")]
        OutstandingQCClaims,

    }

    public enum QaClaimQuickSearchTypeEnum
    {
        /// <summary>
        /// All QA Claims
        /// </summary>
        [StringValue("All QA Claims")]
        AllQaClaims,

        /// <summary>
        /// Outstanding QA Claims
        /// </summary>
        [StringValue("Outstanding QA Claims")]
        OutstandingQaClaims,
    }

    public enum QaClaimSearchFiltersEnum
    {
        /// <summary>
        /// Quick Search
        /// </summary>
        [StringValue("Quick Search")]
        QuickSearch,

        /// <summary>
        /// Analyst
        /// </summary>
        [StringValue("Analyst")]
        Analyst,

        /// <summary>
        /// Client
        /// </summary>
        [StringValue("Client")]
        Client,

        /// <summary>
        /// Restrictions
        /// </summary>
        [StringValue("Restrictions")]
        Restrictions,

    }

    public enum ClaimAuditActionEnum
    {
        /// <summary>
        /// All QA Claims
        /// </summary>
        [StringValue("QA Pass")]
        QaPass,

        /// <summary>
        /// QA Fail
        /// </summary>
        [StringValue("QA Fail")]
        QaFail,
    }

    public enum QAAuditIconEnum
    {
        /// <summary>
        /// All QA Claims
        /// </summary>
        [StringValue("QA Ready")]
        QaReady,

        /// <summary>
        /// QA Fail
        /// </summary>
        [StringValue("QA Done")]
        QaDone,
    }

    public enum ClaimStatusTypeEnum
    {
        /// <summary>
        /// Cotiviti Unreviewed
        /// </summary>
        [StringValue("Cotiviti Unreviewed")]
        CotivitiUnreviewed,

        /// <summary>
        /// Cotiviti Reviewed
        /// </summary>
        [StringValue("Cotiviti Reviewed")]
        CotivitiReviewed,

        /// <summary>
        /// Client Unreviewed
        /// </summary>
        [StringValue("Client Unreviewed")]
        ClientUnreviewed,

        /// <summary>
        /// Client Reviewed
        /// </summary>
        [StringValue("Client Reviewed")]
        ClientReviewed,

        [StringValue("Pended")]
        Pended,
    }

    public enum LogicQuickSearchTypeEnum
    {
        [StringValue("All Logics")]
        AllLogics,

        [StringValue("All Open Logics")]
        AllOpenLogics,

        [StringValue("CV Open Logics")]
        PCIOpenLogics,

        [StringValue("FFP Open Logics")]
        FFPOpenLogics,

        [StringValue("DCA Open Logics")]
        DCIOpenLogics,


        

    }

    public enum ClaimSubStatusTypeEnum
    {

        [StringValue("Documentation Requested")]
        DocumentationRequested,

        [StringValue("Documents Received")]
        DocumentationReceived,

        [StringValue("Documents Required")]
        DocumentationRequired,

        [StringValue("In Process (Negotiation)")]
        InProcess,

        [StringValue("Pending Agreement")]
        PendingAgreement,

        [StringValue("CV QA Audit 2")]
        PCIQAAudit,

        [StringValue("Request Refund")]
        RequestRefund,

        [StringValue("Revised Claims")]
        RevisedClaims,

        [StringValue("Logic Pend")]
        LogicPend,
    }

    public enum LogicStatusEnum
    {
        [StringValue("Open")]
        Open,

        [StringValue("Closed")]
        Closed,

    }
}
