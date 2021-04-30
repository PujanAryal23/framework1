using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum GeneralTabEnum
    {
        [StringValue("Client Code")]
        ClientCode,

        [StringValue("Status")]
        Status,

        [StringValue("Client Name")]
        ClientName,

        [StringValue("Client Type")]
        ClientType,

        [StringValue("Processing Type")]
        ProcessingType

     }
    public enum ConfigurationSettingsEnum
    {
        /// <summary>
        /// Allow All Users to Modify Core Flags
        /// </summary>
        [StringValue("Allow All Users to Modify Core Flags")]
        AllowAllUsersToModifyCoreFlags,

        /// <summary>
        /// Allow Switch Flags on Appeal Actions
        /// </summary>
        [StringValue("Allow Switch Flags on Appeal Action")]
        AllowSwitchFlagsonAppealAction,


        /// <summary>
        /// Allow all Cotiviti Users (Internal and Client)
        /// </summary>
        [StringValue("Allow Access by all Cotiviti User Type for this client")]
        AllowAccessByAllCotivitiUserTypeForThisClient,

        /// <summary>
        /// Limit Access to Nucleus by Client IP Address (enter valid IP address(s)below)
        /// </summary>
        [StringValue("Limit Access to Nucleus by Client IP Address (enter valid IP address(s)below)")]
        LimitAccessToNucleusByClientIPAddress,

        [StringValue("Paid Exposure Threshold")]
        PaidExposureThreshold,

        [StringValue("Provider Score Threshold")]
        ProviderScoreThreshold,

        [StringValue("Allow Switch of non-Reverse Flags")]
        NonReverseFlag,
        
        [StringValue("Allow Switch of Reverse Flags")]
        ReverseFlag,

        [StringValue("Switch Flag on Appeal Action")]
        AppealActionSwitchFlag,

        [StringValue("Provider Alert Indicator option")]
        ProviderAlertIndicatorOption,

        [StringValue("Provider Alert Label")]
        ProviderAlertLabel,

        [StringValue("Clients Can Modify AA flags")]
        ClientsCanModifyAAflags,

        [StringValue("Clients Can Quick Delete Flags")]
        QuickDeleteFlag,

        [StringValue("Scout Case Tracker")]
        ScoutCaseTracker,

        [StringValue("Claim Action Logics")]
        ClaimActionLogics,

        [StringValue("CORE flags can be modified")]
        ModifyCoreFlag
    }

    public enum ProductAppealsSectionHeadersEnum
    {
        [StringValue("Product Status")]
        ProductStatus,

        [StringValue("Appeal Settings")]
        AppealSettings,

        [StringValue("Appeal Due Date Calculation")]
        AppealDueDateCalculation
    }

    public enum ProductAppealsEnum
    {
        [StringValue("Appeals Active")]
        AppealsActive,

        [StringValue("Hide Appeals")]
        HideAppeals,

        [StringValue("Billable Appeals")]
        BillableAppeals,

        [StringValue("Appeal Core Flags")]
        AppealCoreFlags,

        [StringValue("Disable Record Reviews")]
        DisableRecordReviews,

        [StringValue("Enable Medical Record Reviews")]
        EnableMedicalRecordsReviews,

        [StringValue("Display Ext Doc ID Field")]
        DisplayExtDocIDField,

        [StringValue("Cotiviti Uploads Appeal Documents")]
        CotivitiUploadsAppealDocuments,

        [StringValue("Send Appeal Email")]
        SendAppealEmail,

        [StringValue("Appeal Email CC")]
        AppealEmailCC,

        [StringValue("Auto-Close Appeals")]
        AutoCloseAppeals
    }

    public enum SecurityTabEnum
    {
        [StringValue("Client User PHI/PII Access")]
        PhiPiiAccessToClient,

        [StringValue("Limit Access by client IP address")]
        LimitAccessByClientIPAddress,

        [StringValue("Cotiviti access by IP address")]
        CotivitiAccessByIPAddress

    }

    public enum WorkflowTabEnum
    {
        [StringValue("Automated Batch Release")]
        AutomatedBatchRelease,

        [StringValue("Begin Claim Release")]
        BeginClaimRelease,

        [StringValue("Claim Release Interval")]
        ClaimReleaseInterval,

        [StringValue("Return File Frequency")]
        ReturnFileFrequency,

        [StringValue("Failure Alert")]
        FailureAlert,

        [StringValue("Complete Batch with Pended Claims")]
        CompleteBatchWithPendedClaims
    }

    public enum InteropTabEnum
    {
        [StringValue("Retry soft match after")]
        RetrySoftMatchAfter,

        [StringValue("Fail through to DLQ after")]
        FailDLQ,

        [StringValue("Real-time CV Claims")]
        RealTimeClaims,

        [StringValue("FCI Claims")]
        FCIClaims,

        [StringValue("Include Claim Received Date")]
        IncludeClaimReceivedDate,

       
        [StringValue("M-F")]
        MF,

        [StringValue("Sat")]
        Sat,

        [StringValue("Sun")]
        Sun,

        [StringValue("Real-time COB Claims")]
        RealTimeCobClaims
        
    }

    public enum ProcessingTypes
    {
        [StringValue("Batch")]
        Batch,

        [StringValue("CVP Batch")]
        CVPBatch,

        [StringValue("PCA Batch")]
        PCABatch,

        [StringValue("PCA Real-Time")]
        PCARealTime,

        [StringValue("Real-Time")]
        RealTime,

    }
}

