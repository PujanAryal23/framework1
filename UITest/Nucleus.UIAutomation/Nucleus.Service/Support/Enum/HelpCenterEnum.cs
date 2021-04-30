using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum HelpCenterFormsEnum
    {
        [StringValue("New User Request Form")]
        NewUserRequestForm,

        [StringValue("User Termination Request Form")]
        UserTerminationRequestForm,

        [StringValue("Client Report Request Form")]
        ClientReportRequestForm,

        [StringValue("Client Customization Form")]
        ClientCustomizationForm,

        [StringValue("Supported Web Browser Policy")]
        SupportedWebBrowserPolicy,

        [StringValue("Nucleus Password Requirements")]
        NucleusPasswordRequirements
    }
}
