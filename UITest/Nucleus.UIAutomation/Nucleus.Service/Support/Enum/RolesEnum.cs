using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum RoleEnum
    {


        [StringValue("Operations Manager")]
        OperationsManager,

        [StringValue("Appeal Processor")]
        AppealsProcessor,

        [StringValue("Claims Processor")]
        ClaimsProcessor,

        [StringValue("Coding Validation Read Only")]
        ClinicalValidationReadOnly,

        [StringValue("Appeal Category Read Only")]
        AppealCategoryReadOnlyOnly,

        [StringValue("Coding Validation Analyst")]
        ClinicalValidationAnalyst,

        [StringValue("FCI Analyst")]
        FCIAnalyst,

        [StringValue("DCA Analyst")]
        DCIAnalyst,

        [StringValue("FFP Analyst")]
        FFPAnalyst,

        [StringValue("Nucleus Admin")]
        NucleusAdmin,


        [StringValue("Coder Work List")]
        CoderWorkList,

        [StringValue("Claims Read Only")]
        ClaimsReadOnly,


        [StringValue("COB Auditor")]
        COBAuditor,

        

        [StringValue("Claims Manager")]
        ClaimsManager,



        [StringValue("Product Admin")]
        ProductAdmin,

        [StringValue("Product Admin Read Only")]
        ProductAdminReadOnly,

        [StringValue("QA Analyst")]
        QAAnalyst,

        [StringValue("MicroStrategy")]
        MicroStrategy,





    }

    public enum CreateNewRoleFormEnum
    {
        [StringValue("Create New Role")]
        FormTitle,

        [StringValue("Role Name")]
        RoleName,

        [StringValue("User Type")]
        UserType,

        [StringValue("Role Description")]
        RoleDescription,

        [StringValue("Available Authorities")]
        AvailableAuthorities,

        [StringValue("Assigned Authorities")]
        AssignedAuthorities

    }


}
