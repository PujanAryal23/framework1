using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum CompleteYourProfileStepsEnum
    {
        [StringValue("Personal Information")]
        PersonalInformation = 0,
       
        [StringValue("Contact Information")]
        CompleteBasicProfileContactInformation = 1,
        
        [StringValue("Security Settings")]
        SecuritySettings = 2,

        [StringValue("Change Password")]
        ChangePassword = 3

    }
}
