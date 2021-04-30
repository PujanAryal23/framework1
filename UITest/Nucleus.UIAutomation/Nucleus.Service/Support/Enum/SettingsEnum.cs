using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum ProcessingType
    {
        /// <summary>
        /// Batch Type
        /// </summary>
        [StringValue("Batch")]
        B,

        /// <summary>
        /// CVP Batch Type
        /// </summary>
        [StringValue("CVP Batch")]
        C,

        /// <summary>
        /// Real Time Type
        /// </summary>
        [StringValue("Real-Time")]
        R ,

        /// <summary>
        /// PCA Real Time Type
        /// </summary>
        [StringValue("PCA Real-Time")]
        PR,

        [StringValue("PCA Batch")]
        PB,


    }

    public enum MyProfilePersonalInformationEnum
    {
        [StringValue("Prefix")]
        Prefix,

        [StringValue("First Name")]
        FirstName,

        [StringValue("Last Name")]
        LastName,

        [StringValue("Suffix")]
        Suffix,

        [StringValue("Job Title")]
        JobTitle,

        [StringValue("Credentials")]
        Credentials,

    }

    public enum MyProfileContactInformationEnum
    {
        [StringValue("Phone Number")]
        PhoneNumber,

        [StringValue("Ext")]
        PhoneExt,

        [StringValue("Fax")]
        Fax,

        [StringValue("Ext")]
        FaxExt,

        [StringValue("Alt Phone")]
        AltPhone,

        [StringValue("Ext")]
        AltPhoneExt,

        [StringValue("Email Address")]
        EmailAddress
    }

    public enum MyProfileUserPreferencesEnum
    {
        [StringValue("Default Page")]
        DefaultPage,

        [StringValue("Default Client")]
        DefaultClient,

        [StringValue("Enable Tooltips on Claim Action")]
        EnableTooltipsOption,

        [StringValue("Automatically display Patient Claim History on Claim Action")]
        AutomaticallyDisplayOption,
    }
}
