
namespace Nucleus.Service.Support.Common.Constants
{
    public static class UserType
    {
        public const string HCIADMIN8 = "HCIADMIN8";
        public const string HCIADMIN = "HCIADMIN";
        public const string HCIADMIN1 = "HCIADMIN1";
        public const string HCIADMIN2 = "HCIADMIN2";
        public const string HCIADMIN3 = "HCIADMIN3";
        public const string HCIADMIN4 = "HCIADMIN4";
        public const string HCIADMIN5 = "HCIADMIN5";
        public const string HCIADMINCLAIM5 = "HCIADMINCLAIM5";
        public const string HCICLAIMVIEWRESTRICTION = "HCICLAIMVIEWRESTRICTION";
        public const string HCIUSER = "HCIUSER";
        public const string HCIUSERFORMYPROFILE = "HCIUSERFORMYPROFILE";
        public const string HCILOGINCOMPLETE = "HCILOGINCOMPLETE";
        public const string CLIENTUSERFORMYPROFILE = "CLIENTUSERFORMYPROFILE";
        
        public const string HCIUSERWITHNOMANAGEEDITRIGHT = "HCIUSERWITHNOMANAGEAUDIT";

        public const string CLIENT = "CLIENT";
        public const string CLIENT1 = "CLIENT1";

        public const string CLIENT2 = "CLIENT2";
        public const string CLIENT7 = "CLIENT7";
        public const string CLIENTREADONLY = "CLIENTUSERREADONLY";

        public const string CLIENTLOGINCOMPLETE = "CLIENTLOGINCOMPLETE";

        public const string CLIENTWITHNOAUTHORITY = "CLIENTWITHNOAUTHORITY";
        public const string CLIENTCLAIMVIEWRESTRICTION = "CLIENTWITHCLAIMVIEWRESTRICTION";
        public const string Adminwithusermaintenanceauthority = "ADMINWITHUSERMAINTENANCEAUTHORITY";
        public const string Adminwithreadonlyusermaintenanceauthority = "ADMINWITHREADONLYUSERMAINTENANCEAUTHORITY";
        public const string Adminwithnousermaintenanceauthority = "ADMINWITHNOUSERMAINTENANCEAUTHORITY";
        public const string AdminWithManageEditAuthority = "ADMINWITHMANAGEEDITAUTHORITY";
        public const string SampleUser = "SAMPLEUSER";
        public const string MstrUser = "MstrUser";
        public const string ClientMstrUser = "ClientMstrUser";
        public const string MstrUserwithManagerRole = "MstrUserwithManagerRole";
        public const string ClientMstrUserwithManagerRole = "ClientMstrUserwithManagerRole";
        public const string ClientUserWithoutManageEditAuthority = "ClientUserWithoutManageEditAuthority";
        public const string HciUserWithDciWorkListAuthority = "HciUserWithDciWorkListAuthority";
        public const string ClientUserWithDciWorkListAuthority = "ClientUserWithDciWorkListAuthority";
        public const string HCIUserWithoutPCIProductAuthority = "HCIUserWithoutPCIProductAuthority";
        public const string HCIUserNameForSecurityCheck = "HCIUserNameForSecurityCheck";

        public const string PCI5USER = "PciTest5User";
        public const string PCI5CLIENTUSER = "PciTest5ClientUser";
        public static string CurrentUserType { get; set; }
    }
}
