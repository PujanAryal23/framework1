using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
   
    public enum NewUserAccountTabEnum
    {
        [StringValue("Profile")]
        Profile,

        [StringValue("Clients")]
        Clients,

        [StringValue("Roles/Notifications")]
        RolesNotifications
    }

    public enum ProfileEnum
    {
        [StringValue("Personal Information")]
        PersonalInformation,

        [StringValue("User Information")]
        UserInformation,

        [StringValue("Contact Information")]
        ContactInformation,

        [StringValue("Security")]
        Security
    }

    public enum PersonalInformationEnum
    {
        [StringValue("First Name")] 
        FirstName,

        [StringValue("Last Name")] 
        LastName
    }

    public enum UserInformationEnum
    {
        [StringValue("User ID")] 
        UserId,

        [StringValue("User Type")] 
        UserType,

        [StringValue("Status")] 
        Status
    }

    public enum ContactInformationEnum
    {
        [StringValue("Phone")] 
        Phone,

        [StringValue("Email Address")] 
        EmailAddress
    }

    public enum NewUserAccountSecurityEnum
    {

        [StringValue("Password")] 
        Password,

        [StringValue("Confirm Password")] 
        ConfirmPassword
    }

    public enum NewUserRolesEnum
    {
        [StringValue("Available Roles")] AvailableRoles,

        [StringValue("Assigned Roles")] AssignedRoles,

        [StringValue("Default Page")] DefaultPage
    }

    public enum NewUserNotificationsEnum
    {
        [StringValue("Batch complete/return file process cannot be initiated")]
        BatchFileCannotBeInitiated,

        [StringValue("Claims released")]
        ClaimsReleased,

        [StringValue("DCA return file generated")]
        DciReturnFileGenerated,

        [StringValue("FCI return file generated")]
        FciReturnFileGenerated,

        [StringValue("FFP return file generated")]
        FfpReturnFileGenerated,

        [StringValue("Invoice email notification")]
        InvoiceEmailNotification,

        [StringValue("CV return file generated")]
        PciReturnFileGenerated,

        [StringValue("Return file generated")]
        ReturnFileGenerated,
    }
    
    

}


