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
    public enum AppealType
    {
        /// <summary>
        /// Appeal Type
        /// </summary>
        [StringValue("A", "Appeal")]
        Appeal = 0,

        /// <summary>
        /// Record Review Appeal Type
        /// </summary>
        [StringValue("RR", "Record Review")]
        RecordReview = 1,

        /// <summary>
        /// Dental Appeal Type
        /// </summary>
        [StringValue("D", "Dental Review")]
        DentalReview = 2
    }

    /// <summary>
    /// Appeal Status's 
    /// </summary>
    public enum AppealStatusEnum
    {
        [StringValue("", "None")]
        None = -1,

        [StringValue("N", "New")]
        New = 0,

        [StringValue("U", "State Consultant Required")]
        StateConsultantRequired = 1,

        [StringValue("S", "Supervisor")]
        Supervisor = 2,

        [StringValue("T", "Complete")]
        Complete = 3,

        [StringValue("O", "Open")]
        Open = 4,

        [StringValue("P", "Pending Other")]
        PendingOther = 5,

        [StringValue("D", "Documents Waiting")]
        DocumentsWaiting = 6,

        [StringValue("W", "Cotiviti Consultant Required")]
        HCIConsultantRequired = 7,

        [StringValue("K", "Cotiviti Consultant Complete")]
        HCIConsultantComplete = 8,

        [StringValue("V", "State Consultant Complete")]
        StateConsultantComplete = 9,

        [StringValue("C", "Closed")]
        Closed = 10,

        [StringValue("F", "First Review")]
        FirstReview = 11,

        [StringValue("R", "Deleted")]
        Delete = 12,

        [StringValue("M","Mentor Review")]
        MentorReview = 13
    }

    public enum AppealQuickSearchTypeEnum
    {
        /// <summary>
        /// My Appeals
        /// </summary>
        [StringValue("My Appeals")]
        MyAppeals,

        /// <summary>
        /// All Appeals
        /// </summary>
        [StringValue( "All Appeals")]
        AllAppeals,

        /// <summary>
        /// Outstanding Appeals
        /// </summary>
        [StringValue( "Outstanding Appeals")]
        OutstandingAppeals,

        /// <summary>
        /// Last 30 Days Appeals
        /// </summary>
        [StringValue("Last 30 Days")]
        Last30Days,

        /// <summary>
        /// Due Today Appeals
        /// </summary>
        [StringValue( "Appeals Due Today")]
        AppealsDueToday,

        /// <summary>
        /// All Urgent Appeals
        /// </summary>
        [StringValue("All Urgent Appeals")]
        AllUrgentAppeals,

        /// <summary>
        /// All Record Reviews Appeals
        /// </summary>
        [StringValue("All Record Reviews")]
        AllRecordReviews,

        /// <summary>
        /// Outstanding DCA Appeals
        /// </summary>
        [StringValue("Outstanding DCA Appeals")]
        OutstandingDCIAppeals


    }

    public enum PriorityEnum
    {
        [StringValue( "Normal")]
        Normal = 1,

        [StringValue("Urgent")]
        Urgent = 2
    }

    /// <summary>
    /// Appeal Result
    /// </summary>
    public enum AppealResult
    {
        /// <summary>
        /// Appeal Adjust Result
        /// </summary>
        [StringValue("A", "Adjust")]
        Adjust = 0,

        /// <summary>
        /// Appeal No Docs Result
        /// </summary>
        [StringValue("N", "No Docs")]
        NoDocs = 1,

        /// <summary>
        /// Appeal Pay Result
        /// </summary>
        [StringValue("P", "Pay")]
        Pay = 2,

        /// <summary>
        /// Appeal Deny Result
        /// </summary>
        [StringValue("D", "Deny")]
        Deny = 3,

        /// <summary>
        /// Appeal AdjustPend Result
        /// </summary>
        [StringValue("G", "AdjustPend")]
        AdjustPend = 4,

        /// <summary>
        /// Appeal AdjustPend Result
        /// </summary>
        [StringValue("O", "Open")]
        Open = 5
    }

    /// <summary>
    /// Appeal audit actions
    /// </summary>
    public enum AppealAuditAction
    {
        [StringValue("CREATE", "Create")]
        Create = 1,

        [StringValue("EDIT", "Edit")]
        Edit = 2,

        [StringValue("ADDLINE", "AddLine")]
        AddLine = 3,

        [StringValue("RMVLINE", "RemoveLine")]
        RemoveLine = 4,

        [StringValue("PAY", "Pay")]
        Pay = 5,

        [StringValue("DENY", "Deny")]
        Deny = 6,

        [StringValue("ADJUST", "Adjust")]
        Adjust = 7,

        [StringValue("ADJPEND", "AdjustPend")]
        AdjustPend = 8,

        [StringValue("STATUS", "Status")]
        Status = 9,

        [StringValue("VIEW", "View")]
        View = 10,

        [StringValue("CLOSE", "Close")]
        Close = 11,

        [StringValue("COMPLETE", "Complete")]
        Complete = 12,

        [StringValue("ASSIGN", "Assign")]
        Assign = 13,

        [StringValue("MANGDOCS", "ManageDocs")]
        ManageDocs = 14,

        [StringValue("EMAIL", "Email")]
        Email = 15,

        [StringValue("FAX", "Fax")]
        Fax = 16,

        [StringValue("DELETEDOC", "DeleteDoc")]
        DeleteDoc = 17,

        [StringValue("UPLOADDOC", "UploadDocs")]
        UploadDoc = 18,

        [StringValue("SAVE", "Save")]
        Save = 19,

        [StringValue("NODOCS", "NoDocs")]
        NoDocs = 20,

        [StringValue("DELETE", "Delete")]
        Delete = 21
    }

    public enum AppealLevel
    {
        [StringValue("1", "1st")]
        Level1 = 1,

        [StringValue("2", "2nd")]
        Level2 = 2,

        [StringValue("3", "3rd")]
        Level3 = 3,

        [StringValue("4", "4th or greater")]
        Level4 = 4
    }

    
    public enum QaAppealQuickSearchTypeEnum
    {
        /// <summary>
        /// All QA Claims
        /// </summary>
        [StringValue("All QA Appeals")]
        AllQaAppeals,

        /// <summary>
        /// Outstanding QA Claims
        /// </summary>
        [StringValue("Outstanding QA Appeals")]
        OutstandingQaAppeals,
    }

    public enum AppealCategoryManagerAnalystSectionLabels
    {
        /// <summary>
        /// All QA Claims
        /// </summary>
        [StringValue("Analyst (non-restricted claims)")]
        NonRestrictionAccessAnalysts = 0,

        /// <summary>
        /// Outstanding QA Claims
        /// </summary>
        [StringValue("Analyst (restricted claims)")]
        RestrictionAccessAnalysts = 1
    }


    public enum AppealDocTypeEnum
    {
        [StringValue("Other")]
        Other = 1,

        [StringValue("CMS-1500 Form")]
        Cms1500Form = 2,

        [StringValue("UB-04 Form")]
        Ub04Form = 4,

        [StringValue("Claim Summary")]
        ClaimSummary = 8,

        [StringValue("EOB")]
        EOB = 16,

        [StringValue("Provider Letter")]
        providerLetter = 32,

        [StringValue("Clinical Notes")]
        ClinicalNotes = 64,

       
        [StringValue("Procedure Report")]
        ProcedureReport = 128,

       
        [StringValue("Coding Manual")]
        CodingManual = 256,

        
        [StringValue("AMA CDT Form")]
        AmaCdtForm = 512,

       [StringValue("NCPDP Form")]
        NCPDPForm = 1024,

        [StringValue("Image")]
        Image = 2048,
        [StringValue("Subrogation Claim Search")]
        Subrogation = 4096
    }


    public enum DentalAppealDocTypeEnum
    {
        [StringValue("Claim Image")]
        ClaimImage = 8192,

        [StringValue("Chart Notes")]
        ChartNotes = 16384,

        [StringValue("Perio Chart")]
        PerioChart = 32768,

        [StringValue("Images")]
        Images = 65536,

        [StringValue("Narrative")]
        Narrative = 131072,

        [StringValue("Anesthesia Records")]
        AnesthesiaRecords = 262144,

        [StringValue("Operative Report")]
        OperativeReport = 524288,
        
        [StringValue("Other")]
        Other = 100000000,

    }
}
