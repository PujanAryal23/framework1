using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum UserSettingsTabEnum
    {
        [StringValue("Profile")]
        Profile,

        [StringValue("Preferences")]
        Preferences,

        [StringValue("Security")]
        Security,

        [StringValue("Roles")]
        RolesAuthorities,

        [StringValue("Clients")]
        Clients,

        [StringValue("Notifications")]
        Notifications,
    }

    public enum UserProfileEnum
    {
        [StringValue("User ID")]
        UserId,

        [StringValue("User Type")]
        UserType,

        [StringValue("Status")]
        Status,

        [StringValue("Prefix")]
        Prefix,

        [StringValue("First Name")]
        FirstName,

        [StringValue("Last Name")]
        LastName,

        [StringValue("Suffix")]
        Suffix,

        [StringValue("Credentials")]
        Credentials,

        [StringValue("Job Title")]
        JobTitle,

        [StringValue("Subscriber ID")]
        SubscriberID,

        [StringValue("Phone")]
        Phone,

        [StringValue("Fax")]
        Fax,

        [StringValue("Alt Phone")]
        AltPhone,

        [StringValue("Email Address")]
        EmailAddress,

        [StringValue("Department")]
        Department
    }

    public enum UserPreferencesEnum
    {
        [StringValue("Default Page")]
        DefaultPage,

        [StringValue("Default Client")]
        DefaultClient,

        [StringValue("Default Dashboard")]
        DefaultDashboard,

        [StringValue("Auto Display Patient Claim History")]
        AutoDisplayPtClHx,

        [StringValue("Enable Claim Action Tooltips")]
        EnableClAction,
    }

    public enum AuthoritiesEnum
    {
        [StringValue("COB")]
        COB,

        [StringValue("Manage edits")]
        ManageEdits,
    }

    public enum NotificationsEnum
    {
        [StringValue("Batch complete/return file process cannot be initiated")]
        Batchcompletereturnfileprocesscannotbeinitiated,

        [StringValue("CV return file generated")]
        CVreturnfilegenerated,

        [StringValue("Claims released")]
        Claimsreleased,

        [StringValue("DCA return file generated")]
        DCIreturnfilegenerated,

        [StringValue("FCI return file generated")]
        FCIreturnfilegenerated,

        [StringValue("FFP return file generated")]
        FFPreturnfilegenerated,

        [StringValue("Invoice email notification")]
        Invoiceemailnotification,

        

        [StringValue("Return file generated")]
        Returnfilegenerated,
    }

    public enum SecurityEnum
    {
        [StringValue("Security Questions")]
        SecurityQuestions,

        [StringValue("Security Question 1")]
        SecurityQuestion1,

        [StringValue("Security Answer 1","Answer")]
        SecurityAnswer1,

        [StringValue("Security Question 2")]
        SecurityQuestion2,

        [StringValue("Security Answer 2","Answer")]
        SecurityAnswer2,

        [StringValue("Change Password")]
        ChangePassword,

        [StringValue("New Password")]
        NewPassword,

        [StringValue("Confirm New Password")]
        ConfirmNewPassword


    }

    public enum RolesEnum
    {
        [StringValue("Available Roles")] 
        AvailableRoles,

        [StringValue("Assigned Roles")] 
        AssignedRoles
    }
}
